using Il2Cpp;
using Il2CppSystem;
using Il2CppTLD.PDID;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoopClient
{
    public static class FireHook
    {
        public static PlayerControlMode s_ControlModeBeforeTakingTorch = PlayerControlMode.Normal;

        [HarmonyLib.HarmonyPatch(typeof(Fire), "ExitFireStarting")]
        private static class Fire_ExitFireStarting
        {
            private static void Postfix(Fire __instance, bool success, bool playerCancel, float progress)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                SkyCoop.Logger.Log($"ExitFireStarting success {success} playerCancel {playerCancel} progress {progress}");

                if (success)
                {
                    float Fuel = __instance.m_MaxOnTODSeconds;
                    float Heat = __instance.m_FuelHeatIncrease;
                    float InnerRadius = 0;
                    float OuterRadius = 0;
                    float HeatingSped = 0;
                    string GUID = "";
                    ObjectGuid GUIDOJB = __instance.gameObject.GetComponent<ObjectGuid>();
                    if (GUIDOJB == null)
                    {
                        GUIDOJB = __instance.gameObject.AddComponent<ObjectGuid>();
                        GUID = System.Guid.NewGuid().ToString();
                        PdidTable.RuntimeRegister(GUIDOJB, GUID);
                    }
                    else
                    {
                        GUID = GUIDOJB.Get();
                    }

                    if (__instance.m_ApplyToHeatSource && __instance.m_HeatSource)
                    {
                        InnerRadius = __instance.m_HeatSource.m_MaxTempIncreaseInnerRadius;
                        OuterRadius = __instance.m_HeatSource.m_MaxTempIncreaseOuterRadius;
                        HeatingSped = __instance.m_HeatSource.m_TimeToReachMaxTempMinutes * 60;
                    }

                    SkyCoop.Logger.Log($"Send starting fire {GUID} Fuel {Fuel} Heat {Heat} InnerRadius {InnerRadius} OuterRadius {OuterRadius}");

                    if (__instance.m_Campfire == null)
                    {
                        ClientSend.SendStartFire(GUID, Fuel, Heat, InnerRadius, OuterRadius, HeatingSped);
                    }
                    else
                    {
                        ClientSend.SendStartFire(GUID, Fuel, Heat, InnerRadius, OuterRadius, HeatingSped, __instance.gameObject.transform.position, __instance.gameObject.transform.rotation);
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
                    ObjectGuid GUIDOJB = __instance.gameObject.GetComponent<ObjectGuid>();
                    if (GUIDOJB != null)
                    {
                        string GUID = GUIDOJB.Get();

                        if (!string.IsNullOrEmpty(GUID))
                        {
                            SkyCoop.Logger.Log($"Send add fuel to {GUID} Fuel {Fuel} Heat {Heat} InnerRadius {InnerRadius} OuterRadius {OuterRadius}");
                            ClientSend.SendAddFuel(GUID, (Fuel * 60) * 60, Heat, InnerRadius, OuterRadius);
                        }
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

                if (ModMain.Client != null && ModMain.Client.m_Config.m_GameMode == "Lobby")
                {
                    __instance.enabled = false;
                    __instance.gameObject.AddComponent<Comps.ForcedFire>().m_Fire = __instance.Fire;
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
                        ObjectGuid GUIDOJB = __instance.m_FireplaceInteraction.gameObject.GetComponent<ObjectGuid>();
                        if (GUIDOJB != null)
                        {
                            string GUID = GUIDOJB.Get();

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
                        }
                        else
                        {
                            SkyCoop.Logger.Log($"Can' take torch from fire that has no GUID!");
                        }
                    }
                    else
                    {
                        SkyCoop.Logger.Log($"Can' take torch from fire that not exist!");
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
                    ObjectGuid GUIDOJB = __instance.m_ObjectInteractedWith.GetComponent<ObjectGuid>();
                    if (GUIDOJB != null)
                    {
                        string GUID = GUIDOJB.Get();

                        if (!string.IsNullOrEmpty(GUID))
                        {
                            __instance.Enable(false);
                            MenuHook.RemovePleaseWait();
                            MenuHook.DoPleaseWait("Please wait...", "Dismantle campfire");
                            SkyCoop.Logger.Log($"Send dismantle campfire {GUID}");
                            ClientSend.SendDismantleCampfire(GUID);
                        }
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
                    ObjectGuid GUIDOJB = __instance.m_ObjectInteractedWith.GetComponent<ObjectGuid>();
                    if (GUIDOJB != null)
                    {
                        string GUID = GUIDOJB.Get();

                        if (!string.IsNullOrEmpty(GUID))
                        {
                            __instance.Enable(false);
                            MenuHook.RemovePleaseWait();
                            MenuHook.DoPleaseWait("Please wait...", "Taking charcoal...");
                            SkyCoop.Logger.Log($"Send taking charcoal request {GUID}");
                            ClientSend.SendCharcoalCollect(GUID);
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

        [HarmonyLib.HarmonyPatch(typeof(Panel_FireStart), "OnCharcoalHarvest")]
        private static class Panel_FireStart_OnCharcoalHarvest
        {

            private static bool Prefix(Panel_FireStart __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (__instance.m_FireplaceInteraction && __instance.m_FireplaceInteraction.gameObject)
                {
                    ObjectGuid GUIDOJB = __instance.m_FireplaceInteraction.gameObject.GetComponent<ObjectGuid>();
                    if (GUIDOJB != null)
                    {
                        string GUID = GUIDOJB.Get();

                        if (!string.IsNullOrEmpty(GUID))
                        {
                            __instance.Enable(false);
                            MenuHook.RemovePleaseWait();
                            MenuHook.DoPleaseWait("Please wait...", "Taking charcoal...");
                            SkyCoop.Logger.Log($"Send taking charcoal request {GUID}");
                            ClientSend.SendCharcoalCollect(GUID);
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

        public static void HandleAddFire(string GUID)
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
    }
}
