using Il2Cpp;
using Il2CppSystem;
using Il2CppTLD.PDID;
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
            private static bool Prefix(Panel_GearSelect __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if(__instance.m_OnSelectAction != null)
                {
                    string MethodName = __instance.m_OnSelectAction.Method.Name;

                    switch (MethodName)
                    {
                        case "DoFirePickerAction":
                        case "DoBoilPickerAction":

                            DoCookingAction(MethodName, __instance.GetScrolllistCurrentItem(), __instance.m_CookingGameObject);
                            return false;
                        default:
                            return true;
                    }
                }
                return true;
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

                //if (ModMain.Client != null && ModMain.Client.m_Config.m_GameMode == "Lobby")
                //{
                //    __instance.enabled = false;
                //    __instance.gameObject.AddComponent<Comps.ForcedFire>().m_Fire = __instance.Fire;
                //}
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_RecipeBook), "Enable")]
        private static class Panel_RecipeBook_Enable
        {
            private static bool Prefix(Panel_RecipeBook __instance, bool enable)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if(!enable)
                {
                    if (s_ActiveCookignClone)
                    {
                        UnityEngine.Object.Destroy(s_ActiveCookignClone);
                    }
                }

                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_CookWater), "Enable")]
        private static class Panel_CookWater_Enable
        {
            private static void Prefix(Panel_CookWater __instance, bool enable)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (!enable)
                {
                    if (s_ActiveCookignClone)
                    {
                        UnityEngine.Object.Destroy(s_ActiveCookignClone);
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


            SkyCoop.Logger.Log($"DropAndPlaceItem {Gear.name} Slot {CookingIndex} FireGUID {FireGUID}");
            GearItemSaveDataProxy DataProxy = Gear.Serialize();

            s_PedningCookingCloneData = new CookingCloneData(Gear.name, Utils.SerializeObject(DataProxy));

            MenuHook.RemovePleaseWait();
            MenuHook.DoPleaseWait("Please wait...", "Placing gear to cooking slot...");
            GearCookingTarget CookingTarget = Gear.gameObject.AddComponent<GearCookingTarget>();
            CookingTarget.m_CookingIndex = CookingIndex;
            CookingTarget.m_FireGUID = FireGUID;
            CookingTarget.m_PlacePoint = PlacePoint;
            Gear.Drop(1, false, true);
        }

        public static void DoCookingAction(string Action, GearItem SelectedItem, GameObject FireObj)
        {
            bool IsCookingSlot = false;
            int SlotIndex = -1;

            if(FireObj != null)
            {
                SkyCoop.Logger.Log($"DoCookingAction True FireObj name {FireObj.name}");
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
            s_PendingCookingAction = Action;
            s_PendingCookingItem = SelectedItem;

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
                        s_ActiveCookignClone = GetCookingClone(s_PedningCookingCloneData.GearName, s_PedningCookingCloneData.JSON);

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

        public static GearItem GetCookingClone(string GearNanem, string JSON)
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

                GearObject.AddComponent<Comps.GearCookingDummy>();
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

        }

        public static void HandleBoilFromPicker(DroppedGearVisual Visual)
        {
            s_ActiveCookignClone = GetCookingClone(Visual.m_PrefabName, "");

            if (s_ActiveCookignClone)
            {
                if (s_ActiveCookignClone.m_CookingPotItem)
                {
                    Panel_CookWater Panel = InterfaceManager.GetPanel<Panel_CookWater>();

                    if (Panel)
                    {
                        if(Visual.m_CookingSlot != null)
                        {
                            FireplaceInteraction FirePlace = Visual.m_CookingSlot.GetFireplaceHost();
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
