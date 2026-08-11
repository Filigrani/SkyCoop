using Il2Cpp;
using Il2CppTLD.Cooking;
using Il2CppTLD.IntBackedUnit;
using Il2CppTLD.PDID;
using Il2CppTLD.Stats;
using NAudio.CoreAudioApi;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using static SkyCoop.Comps;
using static UnityEngine.Rendering.DebugUI;

namespace SkyCoopClient
{
    public static class FireHook
    {
        public static PlayerControlMode s_ControlModeBeforeTakingTorch = PlayerControlMode.Normal;
        public static string s_PendingCookingAction = string.Empty;
        public static GameObject s_PendingCookingFireObject = null;
        public static GearItem s_PendingCookingItem = null;
        public static string s_PendingCookingInteractionGearGUID = string.Empty;
        public static CookingCloneData s_PedningCookingCloneData = new CookingCloneData("", "");
        public static GearItem s_ActiveCookignClone = null;
        public static bool s_AnySlotsMode = true;

        public struct CookingCloneData
        {
            public string GearName;
            public string JSON;

            public CookingCloneData(string gearname, string json)
            {
                GearName = gearname;
                JSON = json;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Fire), "ExitFireStarting")]
        private static class Fire_ExitFireStarting
        {
            public static float m_FireStarterFuel = 0;
            public static float m_FireStarterHeat = 0;
            
            private static void Prefix(Fire __instance, bool success, bool playerCancel, float progress)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (__instance.m_FuelUsedToStart)
                {
                    m_FireStarterFuel = (__instance.m_FuelUsedToStart.m_BurnDurationHours * 60) * 60;
                    m_FireStarterHeat = __instance.m_FuelUsedToStart.m_HeatIncrease;

                    if (success)
                    {
                        SkyCoop.Logger.Log($"Fire going to be started with {__instance.m_FuelUsedToStart.gameObject.name}");
                    }
                }
            }

            private static void Postfix(Fire __instance, bool success, bool playerCancel, float progress)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                SkyCoop.Logger.Log($"ExitFireStarting success {success} playerCancel {playerCancel} progress {progress}");

                if (success)
                {
                    string GUID = GetGUID(__instance.gameObject);

                    if (!string.IsNullOrEmpty(GUID))
                    {
                        float InnerRadius = 0;
                        float OuterRadius = 0;
                        float HeatingSped = 0;
                        int CookingSlots = 0;

                        if (__instance.m_ApplyToHeatSource && __instance.m_HeatSource)
                        {
                            InnerRadius = __instance.m_HeatSource.m_MaxTempIncreaseInnerRadius;
                            OuterRadius = __instance.m_HeatSource.m_MaxTempIncreaseOuterRadius;
                            HeatingSped = __instance.m_HeatSource.m_TimeToReachMaxTempMinutes * 60;
                        }

                        if (__instance.m_Campfire)
                        {
                            CookingSlots = __instance.m_Campfire.m_CookingSlots.Count;
                        }
                        else
                        {
                            FireplaceInteraction Fireplace = __instance.gameObject.GetComponent<FireplaceInteraction>();

                            if (Fireplace == null)
                            {
                                if (__instance.gameObject.transform.parent != null)
                                {
                                    Fireplace = __instance.gameObject.transform.parent.gameObject.GetComponent<FireplaceInteraction>();
                                }
                            }

                            if (Fireplace != null)
                            {
                                CookingSlots = Fireplace.m_CookingSlots.Count;
                            }
                        }

                        SkyCoop.Logger.Log($"Send starting fire {GUID} Fuel {m_FireStarterFuel} Heat {m_FireStarterHeat} InnerRadius {InnerRadius} OuterRadius {OuterRadius}");

                        if (CookingSlots == 0)
                        {
                            SkyCoop.Logger.Log($"Somehow this fire has no cooking slots!!!!!!!!!!!!");
                        }

                        if (__instance.m_Campfire == null)
                        {
                            ClientSend.SendStartFire(GUID, m_FireStarterFuel, m_FireStarterHeat, InnerRadius, OuterRadius, HeatingSped, CookingSlots);
                        }
                        else
                        {
                            ClientSend.SendStartFire(GUID, m_FireStarterFuel, m_FireStarterHeat, InnerRadius, OuterRadius, HeatingSped, CookingSlots, __instance.gameObject.transform.position, __instance.gameObject.transform.rotation);
                        }
                    }
                    else
                    {
                        SkyCoop.Logger.Log($"Can't start fire on object with no GUID");
                    }


                    if (__instance.m_Campfire)
                    {
                        UnityEngine.Object.Destroy(__instance.gameObject);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Fire), "AddFuel")]
        private static class Fire_AddFuel
        {
            private static void Prefix(Fire __instance, GearItem fuel, bool inForge)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (fuel && fuel.m_FuelSourceItem)
                {
                    float Fuel = fuel.m_FuelSourceItem.m_BurnDurationHours * fuel.GetNormalizedCondition();
                    float Heat = fuel.m_FuelSourceItem.m_HeatIncrease;
                    float InnerRadius = fuel.m_FuelSourceItem.m_HeatInnerRadius;
                    float OuterRadius = fuel.m_FuelSourceItem.m_HeatOuterRadius;

                    string GUID = GetGUID(__instance.gameObject);

                    if (!string.IsNullOrEmpty(GUID))
                    {
                        SkyCoop.Logger.Log($"Send add fuel to {GUID} Fuel {Fuel} Heat {Heat} InnerRadius {InnerRadius} OuterRadius {OuterRadius}");
                        ClientSend.SendAddFuel(GUID, (Fuel * 60) * 60, Heat, InnerRadius, OuterRadius);
                    }
                    else
                    {
                        SkyCoop.Logger.Log($"Can't add fuel to fire with no GUID!");
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_FeedFire), "OnTakeTorch")]
        private static class Panel_FeedFire_OnTakeTorch
        {
            public static bool s_ByPass = false;

            private static bool Prefix(Panel_FeedFire __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (s_ByPass)
                {
                    return true;
                }
                else
                {
                    if (__instance.m_FireplaceInteraction && __instance.m_FireplaceInteraction.gameObject)
                    {
                        string GUID = GetGUID(__instance.m_FireplaceInteraction.gameObject);

                        if (!string.IsNullOrEmpty(GUID))
                        {
                            Panel_HUD Panel;
                            if (InterfaceManager.TryGetPanel<Panel_HUD>(out Panel))
                            {
                                s_ControlModeBeforeTakingTorch = GameManager.GetPlayerManagerComponent().m_ControlMode;
                                GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);
                                Panel.StartItemProgressBar(10, "Taking torch...", null, new System.Action(TakeTorchFailedSilent));
                            }
                            SkyCoop.Logger.Log($"Send take torch request {GUID}");
                            ClientSend.SendTakeTorch(GUID);
                            __instance.ExitFeedFireInterface();
                        }
                        else
                        {
                            SkyCoop.Logger.Log($"Can't take torch from fire that has no GUID!");
                        }
                    }
                    else
                    {
                        SkyCoop.Logger.Log($"Can't take torch from fire that not exist!");
                    }
                }
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_ActionPicker), "DismantleFireCallback")]
        private static class Panel_ActionPicker_DismantleFireCallback
        {
            private static bool Prefix(Panel_ActionPicker __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (__instance.m_ObjectInteractedWith)
                {
                    string GUID = GetGUID(__instance.m_ObjectInteractedWith.gameObject);

                    if (!string.IsNullOrEmpty(GUID))
                    {
                        __instance.Enable(false);
                        MenuHook.RemovePleaseWait();
                        MenuHook.DoPleaseWait("Please wait...", "Dismantle campfire");
                        SkyCoop.Logger.Log($"Send dismantle campfire {GUID}");
                        ClientSend.SendDismantleCampfire(GUID);
                    }
                    else
                    {
                        SkyCoop.Logger.Log($"Can't dismantle campfire that has no GUID!");
                    }
                }

                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_ActionPicker), "TakeCharcoalCallback")]
        private static class Panel_ActionPicker_TakeCharcoalCallback
        {

            private static bool Prefix(Panel_ActionPicker __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (__instance.m_ObjectInteractedWith)
                {
                    string GUID = GetGUID(__instance.m_ObjectInteractedWith.gameObject);

                    if (!string.IsNullOrEmpty(GUID))
                    {
                        __instance.Enable(false);
                        MenuHook.RemovePleaseWait();
                        MenuHook.DoPleaseWait("Please wait...", "Taking charcoal...");
                        SkyCoop.Logger.Log($"Send taking charcoal request {GUID}");
                        ClientSend.SendCharcoalCollect(GUID);
                    }
                    else
                    {
                        SkyCoop.Logger.Log($"Can't take charcoal from fire that has no GUID!");
                    }
                }

                return false;
            }
        }



        [HarmonyLib.HarmonyPatch(typeof(Panel_GearSelect), "SelectGear")]
        private static class Panel_GearSelect_SelectGear
        {
            private static GearItem s_SelectedGear = null;
            private static GameObject s_CookingObject = null;
            private static string s_MethodName = string.Empty;
            private static bool s_DoCookingAction = false;
            
            private static void Prefix(Panel_GearSelect __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                s_SelectedGear = __instance.GetScrolllistCurrentItem();
                s_CookingObject = __instance.m_CookingGameObject;
                s_DoCookingAction = false;

                if (__instance.m_OnSelectAction != null)
                {
                    string MethodName = __instance.m_OnSelectAction.Method.Name;

                    switch (MethodName)
                    {
                        case "DoFirePickerAction":
                        case "DoBoilPickerAction":
                            s_DoCookingAction = true;
                            s_MethodName = __instance.m_OnSelectAction.Method.Name;
                            __instance.m_OnSelectAction = null; // Не вызываем действие готовки, но что бы сам Panel_GearSelect.SelectGear() закончил своё выполнение
                            break;
                        default:
                            break;
                    }
                }
                return;
            }
            private static void Postfix(Panel_GearSelect __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (s_DoCookingAction)
                {
                    DoCookingAction(s_MethodName, s_SelectedGear, s_CookingObject);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_FireStart), "OnCharcoalHarvest")]
        private static class Panel_FireStart_OnCharcoalHarvest
        {

            private static bool Prefix(Panel_FireStart __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (__instance.m_FireplaceInteraction && __instance.m_FireplaceInteraction.gameObject)
                {
                    string GUID = GetGUID(__instance.m_FireplaceInteraction.gameObject);

                    if (!string.IsNullOrEmpty(GUID))
                    {
                        __instance.Enable(false);
                        MenuHook.RemovePleaseWait();
                        MenuHook.DoPleaseWait("Please wait...", "Taking charcoal...");
                        SkyCoop.Logger.Log($"Send taking charcoal request {GUID}");
                        ClientSend.SendCharcoalCollect(GUID);
                        WoodStove Stove = __instance.m_FireplaceInteraction.gameObject.GetComponent<WoodStove>();
                        if(Stove && Stove.m_Open)
                        {
                            Stove.Close();
                        }
                    }
                    else
                    {
                        SkyCoop.Logger.Log($"Can't take charcoal from fire that has no GUID!");
                    }
                }

                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Fire), "GetAvailableCharcoalPieces")]
        private static class Fire_GetAvailableCharcoalPieces
        {
            private static void Postfix(Fire __instance, ref int __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }


                if (__instance.m_StartedByPlayer)
                {
                    // На клиенте мы не знаем сколько угля создалось, делаем кнопку всегда активной,
                    // что бы при нажатии игрок мог кинуть реквест на сервер.

                    // проверка на m_StartedByPlayer добавленна ибо эти гении шейрят UI для розжига и для добавления топлива
                    // из-за этого появляеться кнопка для доставания угля из ещё не зажёноого огня
                    __result = 1;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(CookingSlot), "Awake")]
        private static class CookingSlot_Awake
        {
            private static void Postfix(CookingSlot __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                Comps.CookingSlotVisual VisualHook = __instance.gameObject.GetComponent<Comps.CookingSlotVisual>();

                if(VisualHook == null)
                {
                    VisualHook = __instance.gameObject.AddComponent<Comps.CookingSlotVisual>();
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(CookingSlot), "CanCookingSlotBeUsed")]
        private static class CookingSlot_CanCookingSlotBeUsed
        {
            private static void Postfix(CookingSlot __instance, ref bool __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (ModMain.Client != null && ModMain.Client.m_IsReady && !ModMain.Client.m_Rules.m_CanStartFire)
                {
                    __result = false;
                    return;
                }

                Comps.CookingSlotVisual VisualHook = __instance.gameObject.GetComponent<Comps.CookingSlotVisual>();

                if (VisualHook)
                {
                    if (VisualHook.m_Gear)
                    {
                        __result = false;
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(WoodStove), "Awake")]
        private static class WoodStove_Awake
        {
            private static void Postfix(WoodStove __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (ModMain.Client != null && ModMain.Client.m_IsReady && !ModMain.Client.m_Rules.m_CanStartFire)
                {
                    __instance.enabled = false;
                    if(ModMain.Client != null && ModMain.Client.m_Config.m_GameMode == "Lobby")
                    {
                        __instance.gameObject.AddComponent<Comps.ForcedFire>().m_Fire = __instance.Fire;
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_RecipeBook), "Enable")]
        private static class Panel_RecipeBook_Enable
        {
            private static bool Prefix(Panel_RecipeBook __instance, bool enable)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (enable)
                {
                    if(ModMain.Client != null && ModMain.Client.m_IsReady && !ModMain.Client.m_Rules.m_CanStartFire)
                    {
                        return false;
                    }
                }

                if (!enable)
                {
                    if (s_ActiveCookignClone)
                    {
                        UnityEngine.Object.Destroy(s_ActiveCookignClone.gameObject);
                    }
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_CookWater), "Enable")]
        private static class Panel_CookWater_Enable
        {
            private static bool Prefix(Panel_CookWater __instance, bool enable)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (enable)
                {
                    if (ModMain.Client != null && ModMain.Client.m_IsReady && !ModMain.Client.m_Rules.m_CanStartFire)
                    {
                        return false;
                    }
                }

                if (!enable)
                {
                    if (s_ActiveCookignClone)
                    {
                        UnityEngine.Object.Destroy(s_ActiveCookignClone.gameObject);
                    }
                }

                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Cooking), "Enable")]
        private static class Panel_Cooking_Enable
        {
            public static bool s_DontDestoryClone = false;
            
            private static bool Prefix(Panel_Cooking __instance, bool enable)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (enable)
                {
                    if (ModMain.Client != null && ModMain.Client.m_IsReady && !ModMain.Client.m_Rules.m_CanStartFire)
                    {
                        return false;
                    }
                }

                if (!enable)
                {
                    if (s_ActiveCookignClone)
                    {
                        if (!s_DontDestoryClone)
                        {
                            UnityEngine.Object.Destroy(s_ActiveCookignClone.gameObject);
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "GetGearPlacePoint")]
        private static class PlayerManagere_GetGearPlacePoint
        {
            private static void Postfix(PlayerManager __instance, GameObject go, Vector3 searchPos, ref GearPlacePoint __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (go)
                {
                    CookingSlotVisual Slot = go.GetComponent<CookingSlotVisual>();

                    if (Slot && Slot.m_Gear)
                    {
                        __result = null;
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "DoPositionCheck")]
        private static class PlayerManagere_DoPositionCheck
        {
            private static void Postfix(PlayerManager __instance, ref MeshLocationCategory __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (ModMain.Client != null && ModMain.Client.m_IsReady && !ModMain.Client.m_Rules.m_CanStartFire)
                {
                    if(__instance.m_Gear && __instance.m_Gear.m_CookingPotItem)
                    
                    __result = MeshLocationCategory.Invalid;
                    return;
                }

                if (__instance.m_LastGearPlacePoint)
                {
                    CookingSlot Slot = GetCookingSlotFromPlacePoint(__instance.m_LastGearPlacePoint);

                    if (Slot)
                    {
                        CookingSlotVisual SlotVisual = Slot.GetComponent<CookingSlotVisual>();

                        if(SlotVisual && SlotVisual.m_Gear)
                        {
                            __result = MeshLocationCategory.Invalid;
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(CookingPotItem), "UpdateCookingTimeAndState")]
        private static class CookingPotIteme_UpdateCookingTimeAndState
        {
            private static bool Prefix(CookingPotItem __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return false;
            }
        }


        public static float ConvertVolume(long Units)
        {
            //1000000000 - is 1 liter.
            const long CONVERSION = 1000000000;

            // Use double for intermediate calculation to maintain precision
            double Val = Units / (double)CONVERSION;

            return (float)Val;
        }

        public static float ConvertLiquidVolume(ItemLiquidVolume Volume)
        {
            return ConvertVolume(Volume.m_Units);
        }

        public static float ConvertWeightVolume(ItemWeight Volume)
        {
            return ConvertVolume(Volume.m_Units);
        }

        public static long ConvertVolumeToUnits(float Liters)
        {
            const long CONVERSION = 1000000000;

            // Round to nearest whole unit to minimize floating point errors
            long Units = (long)Math.Round(Liters * CONVERSION, MidpointRounding.AwayFromZero);

            return Units;
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_CookWater), "OnMeltSnow")]
        private static class Panel_CookWater_OnMeltSnow
        {
            private static void Prefix(Panel_CookWater __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (s_ActiveCookignClone)
                {
                    GearCookingDummy Dummy = s_ActiveCookignClone.GetComponent<GearCookingDummy>();
                    if (Dummy)
                    {
                        CookingPotItem PotClone = s_ActiveCookignClone.GetComponent<CookingPotItem>();

                        if (PotClone)
                        {
                            GameAudioManager.Play3DSound(PotClone.m_CookSettings.m_PutSnowInPotAudio, GameManager.GetPlayerTransform().gameObject);
                        }
                        if (!string.IsNullOrEmpty(Dummy.m_RealGearGUID))
                        {
                            float Liters = ConvertLiquidVolume(__instance.m_MeltSnowLiters);

                            if (Liters > 0)
                            {
                                ClientSend.SendGearRecipe(Dummy.m_RealGearGUID, "BadWater", Liters);
                            }
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_CookWater), "OnBoil")]
        private static class Panel_CookWater_OnBoil
        {
            private static bool Prefix(Panel_CookWater __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (s_ActiveCookignClone)
                {
                    GearCookingDummy Dummy = s_ActiveCookignClone.GetComponent<GearCookingDummy>();
                    if (Dummy)
                    {
                        CookingPotItem PotClone = s_ActiveCookignClone.GetComponent<CookingPotItem>();

                        if (PotClone)
                        {
                            GameAudioManager.Play3DSound(PotClone.m_CookSettings.m_PutWaterInPotAudio, GameManager.GetPlayerTransform().gameObject);
                        }
                        if (!string.IsNullOrEmpty(Dummy.m_RealGearGUID))
                        {

                            float Liters = ConvertLiquidVolume(__instance.m_BoilWaterLiters);

                            GearItem Bottle = GameManager.GetInventoryComponent().GetNonPotableWaterSupply();

                            if (Bottle)
                            {
                                Bottle.m_WaterSupply.m_VolumeInLiters -= __instance.m_MeltSnowLiters;
                                if (Bottle.m_WaterSupply.m_VolumeInLiters.m_Units < 0)
                                {
                                    Bottle.m_WaterSupply.m_VolumeInLiters = new ItemLiquidVolume(0);
                                }
                                //string message = Localization.Get("GAMEPLAY_WaterNonPotable") + " (" + Liters + ")";
                                //GearMessage.AddMessage(Bottle, Localization.Get("GAMEPLAY_Dropped"), message, false, true);
                            }

                            if (Liters > 0)
                            {
                                float TimeToCook = (s_ActiveCookignClone.m_CookingPotItem.m_CookSettings.m_MinutesToBoilWaterPerLiter * Liters) / 60f;

                                ClientSend.SendGearRecipe(Dummy.m_RealGearGUID, "GoodWater", Liters);
                            }
                        }
                    }
                }
                __instance.Enable(false);
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Cooking), "OnCookNormalFood")]
        private static class Panel_Cooking_OnCookNormalFood
        {
            private static bool Prefix(Panel_Cooking __instance, GearItem gi)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (s_ActiveCookignClone)
                {
                    if (gi)
                    {
                        if (gi.m_FoodItem && gi.m_FoodItem.m_GearRequiredToOpen)
                        {
                            GameManager.GetPlayerManagerComponent().UseInventoryItem(gi);
                            gi.m_FoodItem.m_ReturnToCookingAfterOpen = true;
                            return false;
                        }
                        GearCookingDummy Dummy = s_ActiveCookignClone.GetComponent<GearCookingDummy>();
                        if (Dummy)
                        {
                            if (!string.IsNullOrEmpty(Dummy.m_RealGearGUID))
                            {
                                string CookingResult = "Warming";
                                float TimeToCook = 0;
                                float BurntTime = 0;
                                float Volume = 0;
                                if (gi.m_Cookable)
                                {
                                    if (gi.m_Cookable.m_CookedPrefab)
                                    {
                                        CookingResult = gi.m_Cookable.m_CookedPrefab.name;
                                    }
                                    TimeToCook = gi.m_Cookable.m_CookTimeMinutes / 60f;
                                    BurntTime = gi.m_Cookable.m_ReadyTimeMinutes / 60f;

                                    if (gi.m_FoodItem)
                                    {
                                        Volume = gi.m_FoodItem.m_CaloriesRemaining;
                                    }
                                    GearCookingTarget Target = gi.gameObject.GetComponent<GearCookingTarget>();

                                    if (Target == null)
                                    {
                                        Target = gi.gameObject.AddComponent<GearCookingTarget>();
                                    }
                                    Target.m_CookpotGUID = Dummy.m_RealGearGUID;
                                    Target.m_Volume = Volume;
                                    Target.m_CookingResult = CookingResult;

                                    if (gi.m_Cookable.m_PotableWaterRequired.m_Units > 0)
                                    {
                                        GearItem Bottle = GameManager.GetInventoryComponent().GetPotableWaterSupply();

                                        if (Bottle)
                                        {
                                            Bottle.m_WaterSupply.m_VolumeInLiters -= gi.m_Cookable.m_PotableWaterRequired;
                                            if (Bottle.m_WaterSupply.m_VolumeInLiters.m_Units < 0)
                                            {
                                                Bottle.m_WaterSupply.m_VolumeInLiters = new ItemLiquidVolume(0);
                                            }
                                        }
                                    }

                                    gi.Drop(1);
                                }
                            }
                        }
                    }
                }
                __instance.Enable(false);
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Cooking), "OnCookRecipe")]
        private static class Panel_Cooking_OnCookRecipe
        {
            private static void Postfix(Panel_Cooking __instance, RecipeData recipe)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                GameManager.GetTimeOfDayComponent().SetDayLengthScale(1.003f);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_CookWater), "OnDoActionSecondary")]
        private static class Panel_CookWater_OnDoActionSecondary
        {
            private static bool Prefix(Panel_CookWater __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (s_ActiveCookignClone)
                {
                    GearCookingDummy Dummy = s_ActiveCookignClone.GetComponent<GearCookingDummy>();
                    if (Dummy)
                    {
                        if (!string.IsNullOrEmpty(Dummy.m_RealGearGUID))
                        {
                            GameObject RealGear = PdidTable.GetGameObject(Dummy.m_RealGearGUID);

                            if (RealGear)
                            {
                                DroppedGearVisual Visual = RealGear.GetComponent<DroppedGearVisual>();
                                if (Visual)
                                {
                                    GearsSync.TryPickUp(Visual, false);
                                }
                            }
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Cooking), "OnDoActionSecondary")]
        private static class Panel_Cooking_OnDoActionSecondary
        {
            private static bool Prefix(Panel_Cooking __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (s_ActiveCookignClone)
                {
                    GearCookingDummy Dummy = s_ActiveCookignClone.GetComponent<GearCookingDummy>();
                    if (Dummy)
                    {
                        if (!string.IsNullOrEmpty(Dummy.m_RealGearGUID))
                        {
                            GameObject RealGear = PdidTable.GetGameObject(Dummy.m_RealGearGUID);

                            if (RealGear)
                            {
                                DroppedGearVisual Visual = RealGear.GetComponent<DroppedGearVisual>();
                                if (Visual)
                                {
                                    GearsSync.TryPickUp(Visual, false);
                                }
                            }
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "OnFoodOpeningComplete")]
        private static class PlayerManager_OnFoodOpeningComplete
        {
            private static bool s_ReturnToCookingAfterOpen = false;
            private static GearItem s_Gear;
            private static void Prefix(PlayerManager __instance, bool success, bool playerCancel, float progress)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                s_ReturnToCookingAfterOpen = false;
                s_Gear = __instance.m_FoodItemOpened;
                if (success)
                {
                    if (s_Gear && s_Gear.m_FoodItem)
                    {
                        s_ReturnToCookingAfterOpen = s_Gear.m_FoodItem.m_ReturnToCookingAfterOpen;
                    }
                }
            }
            private static void Postfix(PlayerManager __instance, bool success, bool playerCancel, float progress)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (success)
                {
                    Panel_Cooking Panel = InterfaceManager.GetPanel<Panel_Cooking>();
                    if (Panel && Panel.IsEnabled())
                    {
                        if (s_Gear && success)
                        {
                            Panel.OnCookNormalFood(s_Gear);
                        }
                        else
                        {
                            FinishCookingAction();
                        }
                    }
                    else
                    {
                        if (s_ReturnToCookingAfterOpen && success)
                        {
                            DoCookingAction("DoFirePickerAction", s_Gear, s_PendingCookingFireObject);
                        }
                        else
                        {
                            FinishCookingAction();
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "OnSmashComplete")]
        private static class PlayerManager_OnSmashComplete
        {
            private static bool s_ReturnToCookingAfterOpen = false;
            private static GearItem s_Gear;
            private static void Prefix(PlayerManager __instance, bool success, bool playerCancel, float progress)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                s_ReturnToCookingAfterOpen = false;
                s_Gear = __instance.m_SmashableItemUsed;
                if (success)
                {
                    if (s_Gear && s_Gear.m_FoodItem)
                    {
                        s_ReturnToCookingAfterOpen = s_Gear.m_FoodItem.m_ReturnToCookingAfterOpen;
                    }
                }
            }

            private static void Postfix(PlayerManager __instance, bool success, bool playerCancel, float progress)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                Panel_Cooking Panel = InterfaceManager.GetPanel<Panel_Cooking>();
                if (Panel && Panel.IsEnabled())
                {
                    if (s_Gear && success)
                    {
                        Panel.OnCookNormalFood(s_Gear);
                    }
                    else
                    {
                        FinishCookingAction();
                    }
                }
                else
                {
                    if (s_ReturnToCookingAfterOpen && success)
                    {
                        DoCookingAction("DoFirePickerAction", s_Gear, s_PendingCookingFireObject);
                    }
                    else
                    {
                        FinishCookingAction();
                    }
                }
            }
        }

        public static void DropAndPlaceItem(GearPlacePoint PlacePoint, GearItem Gear, int CookingIndex, string FireGUID)
        {
            if(PlacePoint == null)
            {
                SkyCoop.Logger.Log($"GearPlacePoint doesn't exist");
                FinishCookingAction();
                return;
            }
            if(Gear == null)
            {
                SkyCoop.Logger.Log($"Gear for cooking doesn't exist");
                FinishCookingAction();
                return;
            }
            Gear.MaybePlayCookingSlotPlacementAudio(PlacePoint);

            //SkyCoop.Logger.Log($"DropAndPlaceItem {Gear.name} Slot {CookingIndex} FireGUID {FireGUID} s_LastGearTimeBeingCooked {GearsSync.s_LastGearTimeBeingCooked}");
            GearItemSaveDataProxy DataProxy = Gear.Serialize();

            s_PedningCookingCloneData = new CookingCloneData(Gear.name, Utils.SerializeObject(DataProxy));

            MenuHook.RemovePleaseWait();
            MenuHook.DoPleaseWait("Please wait...", "Placing gear to cooking slot...");
            GearCookingTarget CookingTarget = Gear.gameObject.GetComponent<GearCookingTarget>();

            if(CookingTarget == null)
            {
                CookingTarget = Gear.gameObject.AddComponent<GearCookingTarget>();
            }

            CookingTarget.m_CookingIndex = CookingIndex;
            CookingTarget.m_FireGUID = FireGUID;
            CookingTarget.m_PlacePoint = PlacePoint;
            CookingTarget.m_TimeBeingCooked = GearsSync.s_LastGearTimeBeingCooked;

            if (Gear.m_Cookable)
            {
                if (Gear.m_Cookable.m_CookedPrefab)
                {
                    CookingTarget.m_CookingResult = Gear.m_Cookable.m_CookedPrefab.name;
                }
                else
                {
                    CookingTarget.m_CookingResult = "Warming";
                }

                if (Gear.m_FoodItem)
                {
                    CookingTarget.m_Volume = Gear.m_FoodItem.m_CaloriesRemaining;
                }
            }

            Gear.Drop(1, false, true);
        }

        public static void DoCookingAction(string Action, GearItem SelectedItem, GameObject FireObj, float TimeBeingCooked = 0)
        {
            s_PendingCookingAction = Action;
            s_PendingCookingItem = SelectedItem;
            GearsSync.s_LastGearTimeBeingCooked = TimeBeingCooked;

            if (SelectedItem)
            {
                if(SelectedItem.m_FoodItem && SelectedItem.m_FoodItem.m_GearRequiredToOpen)
                {
                    GameManager.GetPlayerManagerComponent().UseInventoryItem(SelectedItem);
                    SelectedItem.m_FoodItem.m_ReturnToCookingAfterOpen = true;
                    Panel_GearSelect _Panel = InterfaceManager.GetPanel<Panel_GearSelect>();

                    if (_Panel)
                    {
                        _Panel.Enable(false, Panel_GearSelect.ListItemFilter.None, false);
                        _Panel.m_FeedFireGameObject = null;
                        _Panel.m_OnSelectAction = null;
                        _Panel.m_FeedFireGameObject = null;
                    }
                    s_PendingCookingFireObject = FireObj;
                    return;
                }
            }
            else
            {
                // Когда пытаешься разложить "Иструмента нет"

                s_PendingCookingFireObject = FireObj;
                Panel_Cooking PanelC = InterfaceManager.GetPanel<Panel_Cooking>();

                if (PanelC)
                {
                    PanelC.SetCookingPot(null);
                    PanelC.SetFilterBasedOnCookingPot();
                    PanelC.Enable(true);
                    return;
                }
            }

            bool IsCookingSlot = false;
            int SlotIndex = -1;

            if (FireObj != null)
            {
                SkyCoop.Logger.Log($"DoCookingAction True FireObj name {FireObj.name} TimeBeingCooked {TimeBeingCooked}");
                CookingSlot Slot = FireObj.GetComponent<CookingSlot>();

                if (Slot)
                {
                    IsCookingSlot = true;
                    SlotIndex = GetCookingSlotIndex(Slot);

                    FireplaceInteraction FirePlace = Slot.GetFireplaceHost();

                    if (FirePlace)
                    {
                        s_PendingCookingFireObject = FirePlace.gameObject;
                    }
                    else
                    {
                        SkyCoop.Logger.Log($"DoCookingAction cooking slot's fireplaceInteraction has no GUID");
                        s_PendingCookingFireObject = null;
                    }
                }
                else
                {
                    s_PendingCookingFireObject = FireObj;
                }
            }
            else
            {
                s_PendingCookingFireObject = null;
            }

            //if(SelectedItem == null)
            //{
            //    SkyCoop.Logger.Log($"DoCookingAction {Action} SelectedItem null");
            //}
            //else
            //{
            //    SkyCoop.Logger.Log($"DoCookingAction {Action} SelectedItem {SelectedItem.name}");

            //}

            if (s_PendingCookingFireObject != null)
            {
                SkyCoop.Logger.Log($"DoCookingAction Fixed FireObj name {s_PendingCookingFireObject.name}");
                string GUID = GetGUID(s_PendingCookingFireObject);

                if (IsCookingSlot)
                {
                    if (!string.IsNullOrEmpty(GUID))
                    {
                        RequestCookingSlotIsEmpty(GUID, SlotIndex);
                    }
                    else
                    {
                        SkyCoop.Logger.Log($"DoCookingAction cooking slot's fireplaceInteraction has no GUID");
                        FinishCookingAction();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(GUID))
                    {
                        RequestFreeCookingSlot(GUID);
                    }
                    else
                    {
                        SkyCoop.Logger.Log($"DoCookingAction FireObj has no GUID");
                        FinishCookingAction();
                    }
                }
            }
            else
            {
                SkyCoop.Logger.Log($"DoCookingAction FireObj is null");
                FinishCookingAction();
            }

            Panel_GearSelect Panel = InterfaceManager.GetPanel<Panel_GearSelect>();

            if (Panel)
            {
                Panel.Enable(false, Panel_GearSelect.ListItemFilter.None, false);
                Panel.m_FeedFireGameObject = null;
                Panel.m_OnSelectAction = null;
                Panel.m_FeedFireGameObject = null;
            }
        }

        public static void ContinueCookingAction(int CookingSlotIndex)
        {
            if (s_PendingCookingFireObject)
            {
                FireplaceInteraction FirePlace = s_PendingCookingFireObject.GetComponent<FireplaceInteraction>();

                if(FirePlace == null)
                {
                    Fire Fire = s_PendingCookingFireObject.GetComponent<Fire>();
                    CookingSlot CookingSlot = s_PendingCookingFireObject.GetComponent<CookingSlot>();

                    if (Fire == null)
                    {
                        SkyCoop.Logger.Log($"Pending fire object does not contains fire or fireplaceinteraction or cookingslot component!");
                        FinishCookingAction();
                        return;
                    }
                    else if(CookingSlot != null)
                    {
                        FirePlace = CookingSlot.GetFireplaceHost();
                    }
                    else
                    {
                        FirePlace = GetFireplaceFromFire(Fire);

                        if (FirePlace == null)
                        {
                            SkyCoop.Logger.Log($"Pending fire object does not contains fireplaceinteraction, failed to find fireplace by fire!");
                            FinishCookingAction();
                            return; 
                        }
                    }
                }

                CookingSlot Slot = GetCookingSlotByIndex(FirePlace, CookingSlotIndex);

                if (Slot == null)
                {
                    SkyCoop.Logger.Log($"Slot index provoided by server {CookingSlotIndex} is not exist on the client!");
                }
                else
                {
                    PlaceGearForCookingAction(Slot, CookingSlotIndex, GetGUID(s_PendingCookingFireObject));
                }
            }
            else
            {
                SkyCoop.Logger.Log($"Fire for ContinueCookingAction does not exist!");
            }
        }

        public static void PlaceGearForCookingAction(CookingSlot Slot, int SlotIndex, string FireGUID)
        {
            
            if (Slot == null)
            {
                SkyCoop.Logger.Log($"Can't place gear on cooking slot, cooking slot not exist!");
                FinishCookingAction();
                return;
            }
            if(SlotIndex == -1)
            {
                SkyCoop.Logger.Log($"Can't place gear on cooking slot, invalid slot index {SlotIndex}!");
                FinishCookingAction();
                return;
            }
            if (string.IsNullOrEmpty(FireGUID))
            {
                SkyCoop.Logger.Log($"Can't place gear on fire with no GUID!");
                FinishCookingAction();
                return;
            }
            
            DropAndPlaceItem(Slot.m_GearPlacePoint, s_PendingCookingItem, SlotIndex, FireGUID);
        }

        public static void FinishCookingAction(string GearGUID = "", string FireGUID = "")
        {
            if (string.IsNullOrEmpty(GearGUID) || string.IsNullOrEmpty(FireGUID))
            {
                s_PendingCookingAction = string.Empty; // Отменяем действие
            }
            else
            {
                SkyCoop.Logger.Log($"FinishCookingAction GearGUID {GearGUID} FireGUID {FireGUID} Action {s_PendingCookingAction}");
            }
            
            switch (s_PendingCookingAction)
            {
                case "DoFirePickerAction":
                    break;
                case "DoBoilPickerAction":
                    if (!string.IsNullOrEmpty(s_PedningCookingCloneData.GearName))
                    {
                        s_ActiveCookignClone = GetCookingClone(s_PedningCookingCloneData.GearName, s_PedningCookingCloneData.JSON, GearGUID);

                        if (s_ActiveCookignClone)
                        {
                            if (s_ActiveCookignClone.m_CookingPotItem)
                            {
                                Panel_CookWater Panel = InterfaceManager.GetPanel<Panel_CookWater>();

                                if (Panel)
                                {
                                    Panel.SetFireContainer(s_PendingCookingFireObject);
                                    Panel.SetCookingPot(s_ActiveCookignClone.m_CookingPotItem);
                                    Panel.Enable(true);
                                }
                                break;
                            }
                            else
                            {
                                UnityEngine.Object.Destroy(s_ActiveCookignClone.gameObject);
                                break;
                            }
                        }
                        else
                        {
                            SkyCoop.Logger.Log($"Failed to create cooking clone {s_PedningCookingCloneData.GearName}");
                            break;
                        }
                    }
                    else
                    {
                        SkyCoop.Logger.Log($"Failed to create cooking clone there no data for clone");
                        break;
                    }
                default:
                    break;
            }
            s_PendingCookingAction = string.Empty;
            s_PendingCookingFireObject = null;
            s_PendingCookingItem = null;
            s_PendingCookingInteractionGearGUID = string.Empty;
            s_PedningCookingCloneData = new CookingCloneData("", "");
        }

        public static GearItem GetCookingClone(string GearNanem, string JSON, string RealGUID = "")
        {
            GameObject reference = AssetManager.GetAssetFromGame<GameObject>(GearNanem);
            if (reference)
            {
                GameObject GearObject = UnityEngine.Object.Instantiate(reference);

                GearObject.name = GearNanem;

                GearItemSaveDataProxy DataProxy = Utils.DeserializeObject<GearItemSaveDataProxy>(JSON);
                GearItem Gi = GearObject.GetComponent<GearItem>();
                Gi.Deserialize(DataProxy, true);

                GearsSync.GearManualPatch(Gi);
                Gi.ManualStart();
                Gi.ManualUpdate();

                Comps.GearCookingDummy Hook = GearObject.AddComponent<Comps.GearCookingDummy>();

                if (Hook)
                {
                    Hook.m_RealGearGUID = RealGUID;
                }

                GearObject.SetActive(false);
                return Gi;
            }
            return null;
        }

        public static void RequestFreeCookingSlot(string GUID)
        {
            if (!string.IsNullOrEmpty(GUID))
            {
                s_AnySlotsMode = true;
                MenuHook.RemovePleaseWait();
                MenuHook.DoPleaseWait("Please wait...", "Looking for cooking slot");
                ClientSend.SendRequestFreeCookingSlot(GUID);
            }
        }

        public static void RequestCookingSlotIsEmpty(string GUID, int SlotIndex)
        {
            if (!string.IsNullOrEmpty(GUID))
            {
                s_AnySlotsMode = false;
                MenuHook.RemovePleaseWait();
                MenuHook.DoPleaseWait("Please wait...", "Checking cooking slot");
                ClientSend.SendRequestFreeCookingSlot(GUID, SlotIndex);
            }
        }

        public static void RequestFreeCookingSlot(GameObject Obj)
        {
            string GUID = GetGUID(Obj);
            RequestFreeCookingSlot(GUID);
        }

        public static CookingSlot GetCookingSlotByIndex(FireplaceInteraction FirePlace, int Index)
        {
            if (FirePlace == null)
            {
                return null;
            }

            if(Index < 0)
            {
                return null;
            }

            if(FirePlace.m_CookingSlots.Count-1 < Index)
            {
                return null;
            }
            return FirePlace.m_CookingSlots[Index];
        }

        public static CookingSlot GetCookingSlotByIndex(string GUID, int Index)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);

            if (Obj != null)
            {
                Fire Fire = Obj.GetComponent<Fire>();
                if (Fire)
                {
                    FireplaceInteraction FirePlace = GetFireplaceFromFire(Fire);
                    if(FirePlace != null)
                    {
                        return GetCookingSlotByIndex(FirePlace, Index);
                    }
                }
            }

            return null;
        }

        public static int GetCookingSlotIndex(CookingSlot Slot)
        {
            if (Slot == null)
            {
                return -1;
            }

            FireplaceInteraction Fireplace = Slot.GetFireplaceHost();

            if (Fireplace)
            {
                for (int i = 0; i < Fireplace.m_CookingSlots.Count; i++)
                {
                    CookingSlot OtherSlot = Fireplace.m_CookingSlots[i];

                    if (OtherSlot != null)
                    {
                        if (OtherSlot == Slot)
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        public static FireplaceInteraction GetFireplaceFromFire(Fire Fire)
        {
            if(Fire == null)
            {
                return null;
            }
            if (Fire.m_Campfire)
            {
                return Fire.m_Campfire;
            }
            FireplaceInteraction FirePlace = Fire.gameObject.GetComponent<FireplaceInteraction>();

            if (FirePlace)
            {
                return FirePlace;
            }
            else
            {
                if (Fire.transform.parent)
                {
                    FirePlace = Fire.transform.parent.gameObject.GetComponent<FireplaceInteraction>();
                }
            }
            return FirePlace;
        }

        public static CookingSlot GetCookingSlotFromPlacePoint(GearPlacePoint Point)
        {
            if(Point == null)
            {
                return null;
            }
            if (Point.m_FireToAttach == null)
            {
                return null;
            }
            FireplaceInteraction FirePlace = GetFireplaceFromFire(Point.m_FireToAttach);

            if (FirePlace == null)
            {
                return null;
            }

            foreach (CookingSlot Slot in FirePlace.m_CookingSlots)
            {
                if(Slot.m_GearPlacePoint == Point)
                {
                    return Slot;
                }
            }
            return null;
        }

        public static string GetGUID(GameObject Obj)
        {
            Fire Fire = Obj.GetComponent<Fire>();

            if (Fire == null)
            {
                FireplaceInteraction FirePlace = Obj.GetComponent<FireplaceInteraction>();

                if (FirePlace)
                {
                    Fire = FirePlace.Fire;
                }
            }

            if (Fire)
            {
                ObjectGuid GUIDOJB = Fire.gameObject.GetComponent<ObjectGuid>();

                if (GUIDOJB == null)
                {
                    GUIDOJB = Fire.gameObject.AddComponent<ObjectGuid>();
                    string GUID = System.Guid.NewGuid().ToString();
                    PdidTable.RuntimeRegister(GUIDOJB, GUID);
                }
                else
                {
                    return GUIDOJB.Get();
                }
            }

            return "";
        }

        public static void TakeTorch()
        {
            CancleTakingTorch();
            Panel_FeedFire_OnTakeTorch.s_ByPass = true;
            Panel_FeedFire Panel = InterfaceManager.GetPanel<Panel_FeedFire>();
            if (Panel)
            {
                Panel.OnTakeTorch();
            }
            Panel_FeedFire_OnTakeTorch.s_ByPass = false;
        }

        public static void CancleTakingTorch()
        {
            GameManager.GetPlayerManagerComponent().SetControlMode(s_ControlModeBeforeTakingTorch);
            Panel_HUD Panel;
            if (InterfaceManager.TryGetPanel<Panel_HUD>(out Panel))
            {
                Panel.CancelItemProgressBar();
            }
        }

        public static void TakeTorchFailed()
        {
            GameAudioManager.PlayGUIError();
            HUDMessage.AddMessage("Failed, can't take torch!", true, true);
            CancleTakingTorch();
        }

        public static void TakeTorchFailedSilent()
        {
            CancleTakingTorch();
        }

        public static bool FireExist(string GUID)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);

            return Obj != null;
        }

        public static bool FireIsBurning(string GUID)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);

            if(Obj != null)
            {
                Fire Fire = Obj.GetComponent<Fire>();
                if (Fire)
                {
                    return Fire.m_FireState == FireState.FullBurn;
                }
            }

            return false;
        }

        public static void CreateCampfire(string GUID, Vector3 Position, Quaternion Rotation)
        {
            if (!FireExist(GUID))
            {
                FireManager FireManager = GameManager.GetFireManagerComponent();

                if (FireManager)
                {
                    Fire FireInstance = GameManager.GetFireManagerComponent().InstantiateCampFire();

                    if (FireInstance)
                    {
                        GameObject CampfireInstance = FireInstance.gameObject;
                        if (CampfireInstance)
                        {
                            CampfireInstance.transform.position = Position;
                            CampfireInstance.transform.rotation = Rotation;
                            ObjectGuid GUIDOJB = CampfireInstance.GetComponent<ObjectGuid>();
                            if (GUIDOJB == null)
                            {
                                GUIDOJB = CampfireInstance.AddComponent<ObjectGuid>();
                            }
                            PdidTable.RuntimeRegister(GUIDOJB, GUID);

                            if (FireInstance.m_Campfire)
                            {
                                // Камни которые подстраиваються под ландшавт не имеют отдельного метода для пересчёта
                                // и пересчитываються только в режиме растановки. Я придумал такой костыль.
                                FireInstance.m_Campfire.m_LastPosition = Vector3.zero;
                                FireInstance.m_Campfire.m_IsBeingPlaced = true;
                                FireInstance.m_Campfire.Update();
                                FireInstance.m_Campfire.m_IsBeingPlaced = false;
                            }
                        }
                        else
                        {
                            SkyCoop.Logger.Log($"CampfireInstance could not be spawned");
                        }
                    }
                }
                else
                {
                    SkyCoop.Logger.Log($"Fire manager not exist");
                }
            }
        }

        public static void HandleFireSync(string GUID, float MaxBurnTime, float ElapsedBurnTime, float FuelHeatIncrees, float Heat, float InnerRadius, float OutterRadius, FireState State)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);

            if (Obj != null)
            {
                Fire Fire = Obj.GetComponent<Fire>();

                if (Fire)
                {
                    //SkyCoop.Logger.Log($"HandleFireSync {GUID} ElapsedBurnTime {ElapsedBurnTime} MaxBurnTime {MaxBurnTime} Heat {Heat} State {State}");
                    Fire.m_FuelHeatIncrease = Heat;
                    Fire.m_MaxOnTODSeconds = MaxBurnTime;
                    Fire.m_ElapsedOnTODSeconds = ElapsedBurnTime;
                    Fire.m_StartedByPlayer = true;

                    if (Fire.m_ApplyToHeatSource)
                    {
                        if (Fire.m_HeatSource)
                        {
                            Fire.m_HeatSource.m_MaxTempIncrease = Fire.m_FuelHeatIncrease;
                            Fire.m_HeatSource.m_TempIncrease = Heat;
                            Fire.m_HeatSource.m_MaxTempIncreaseInnerRadius = Mathf.Max(InnerRadius, Fire.m_HeatSource.m_MaxTempIncreaseInnerRadius);
                            Fire.m_HeatSource.m_MaxTempIncreaseOuterRadius = Mathf.Max(OutterRadius * Fire.GetFireOuterRadiusScale(), Fire.m_HeatSource.m_MaxTempIncreaseOuterRadius);
                        }

                        Fire.m_HeatSource.m_TurnedOn = State != FireState.FullBurn;
                    }

                    if (Fire.m_FireState == FireState.FullBurn && State == FireState.Off)
                    {
                        Fire.TurnOffImmediate();
                    }

                    if(State == FireState.FullBurn)
                    {
                        if(Fire.m_ElapsedOnTODSeconds > Fire.m_MaxOnTODSeconds)
                        {
                            Fire.m_EmberTimer = 100;
                        }
                        else
                        {
                            Fire.m_EmberTimer = 0;
                        }
                    }
                    else
                    {
                        Fire.m_EmberTimer = 0;
                    }
                    Fire.FireStateSet(State);

                    if (Fire.m_FX)
                    {
                        Fire.m_FX.TriggerStage(State, true, true);
                    }

                    if (Fire.m_Campfire)
                    {
                        switch (State)
                        {
                            case FireState.Off:
                            case FireState.Blownout:
                                Fire.m_Campfire.SetState(CampfireState.BurntOut);
                                break;
                            case FireState.FullBurn:
                                Fire.m_Campfire.SetState(CampfireState.Lit);
                                break;
                        }
                    }
                }
            }
        }

        public static void HandleAddFuel(string GUID)
        {
            SkyCoop.Logger.Log($"HandleFireSync {GUID}");
            GameObject Obj = PdidTable.GetGameObject(GUID);

            if (Obj)
            {
                Fire Fire = Obj.GetComponent<Fire>();

                if (Fire && Fire.m_FX)
                {
                    Fire.m_FX.TriggerFlareupLarge();
                }
            }
        }

        public static void HandleRemoveFire(string GUID)
        {
            SkyCoop.Logger.Log($"HandleRemoveFire {GUID}");
            GameObject Obj = PdidTable.GetGameObject(GUID);

            if (Obj)
            {
                UnityEngine.Object.Destroy(Obj);
            }
        }

        public static void HandleCharcoal(int Charcoal)
        {
            SkyCoop.Logger.Log($"HandleCharcoal {Charcoal}");

            Panel_FireStart Panel = InterfaceManager.GetPanel<Panel_FireStart>();

            if (Panel)
            {
                for (int i = 1; i <= Charcoal; i++)
                {
                    GameObject CharcoalObject = UnityEngine.Object.Instantiate<GameObject>(Panel.m_CharcoalItemPrefab);
                    if (CharcoalObject)
                    {
                        CharcoalObject.name = Panel.m_CharcoalItemPrefab.name;
                        GearItem Gear = CharcoalObject.GetComponent<GearItem>();

                        bool forceEquip = i == Charcoal;
                        bool skipAudio = i == Charcoal;
                        GameManager.GetPlayerManagerComponent().ProcessPickupItemInteraction(Gear, forceEquip, false, skipAudio);
                        GearMessage.AddMessage(Gear, Localization.Get("GAMEPLAY_Harvested"), string.Concat(new object[]
                        {
                            Gear.DisplayName,
                            " (",
                            Charcoal,
                            ")"
                        }), false, true);
                    }
                }
            }
        }
        public static void HandleFreeCookingSlot(int SlotIndex)
        {
            SkyCoop.Logger.Log($"HandleFreeCookingSlot {SlotIndex}");
            ContinueCookingAction(SlotIndex);
        }

        public static void HandleCookingInteraction(string GearGUID, string FireGUID)
        {
            SkyCoop.Logger.Log($"HandleCookingInteraction Gear {GearGUID} Fire {FireGUID}");

            FinishCookingAction(GearGUID, FireGUID);
        }

        public static void HandleCookFromPicker(DroppedGearVisual Visual)
        {
            SkyCoop.Logger.Log($"HandleCookFromPicker");
            s_ActiveCookignClone = GetCookingClone(Visual.m_PrefabName, "", Visual.m_GUID);

            if (s_ActiveCookignClone)
            {
                if (s_ActiveCookignClone.m_CookingPotItem)
                {
                    Panel_Cooking Panel = InterfaceManager.GetPanel<Panel_Cooking>();

                    if (Panel)
                    {
                        Panel.SetCookingPot(s_ActiveCookignClone.m_CookingPotItem);

                        if (Visual.m_CookingVisual)
                        {
                            Panel.SetCookingSlot(Visual.m_CookingVisual.m_CookingSlot);
                            Panel.SetFilterBasedOnCookingPot();

                            if (Visual.m_CookingVisual.m_CookingSlot)
                            {
                                FireplaceInteraction FirePlace = Visual.m_CookingVisual.m_CookingSlot.GetFireplaceHost();

                                if (FirePlace)
                                {
                                    Panel.m_Fire = FirePlace.Fire;
                                }
                            }
                        }
                        Panel.Enable(true);
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(s_ActiveCookignClone.gameObject);
                    }
                }
                else
                {
                    UnityEngine.Object.Destroy(s_ActiveCookignClone.gameObject);
                }
            }
        }

        public static void HandleBoilFromPicker(DroppedGearVisual Visual)
        {
            s_ActiveCookignClone = GetCookingClone(Visual.m_PrefabName, "", Visual.m_GUID);

            if (s_ActiveCookignClone)
            {
                if (s_ActiveCookignClone.m_CookingPotItem)
                {
                    Panel_CookWater Panel = InterfaceManager.GetPanel<Panel_CookWater>();

                    if (Panel)
                    {
                        if(Visual.m_CookingVisual && Visual.m_CookingVisual.m_CookingSlot)
                        {
                            FireplaceInteraction FirePlace = Visual.m_CookingVisual.m_CookingSlot.GetFireplaceHost();
                            if (FirePlace)
                            {
                                Panel.SetFireContainer(FirePlace.gameObject);
                            }
                        }
                        Panel.SetCookingPot(s_ActiveCookignClone.m_CookingPotItem);
                        Panel.Enable(true);
                    }
                }
                else
                {
                    UnityEngine.Object.Destroy(s_ActiveCookignClone.gameObject);
                }
            }
        }
    }
}
