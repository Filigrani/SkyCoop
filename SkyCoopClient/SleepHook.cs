using Il2Cpp;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppSystem.Globalization.HebrewNumber;
using static UnityEngine.Rendering.DebugUI;

namespace SkyCoopClient
{
    public static class SleepHook
    {
        public static bool s_LastEveryoneIsSleeping = false;
        public static float s_DesiredTimeToSleep = 0;

        // Rest.m_SleepFadeOutSeconds это 8 секунд
        // TimeOfDay.m_DayLengthScale когда спим 12 часов 0.0083f


        [HarmonyLib.HarmonyPatch(typeof(Bed), "Awake")]
        private static class Bed_Awake
        {
            private static void Postfix(Bed __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if(ModMain.Client != null && ModMain.Client.m_IsReady)
                {
                    __instance.enabled = ModMain.Client.m_Rules.m_CanUseBeds;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Fatigue), "Update")]
        private static class Fatigue_Update
        {
            private static void Postfix(Fatigue __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (ModMain.Client != null && ModMain.Client.m_IsReady)
                {
                    if (!ModMain.Client.m_Rules.m_Fatigue)
                    {
                        __instance.m_CurrentFatigue = 0;
                    }
                }
            }
        }

        public static void PatchPanel(Panel_Rest Panel)
        {
            Transform ControllOffset = Panel.gameObject.transform.GetChild(3);
            if (ControllOffset)
            {
                Transform Shared = ControllOffset.GetChild(1);
                if (Shared)
                {
                    Transform Stats = Shared.GetChild(3);

                    if (Stats)
                    {
                        for (int i = 0; i < Stats.childCount; i++)
                        {
                            Transform Child = Stats.GetChild(i);

                            if (Child)
                            {
                                if (i == 0 || i == 1 || i == 5 || i == 8 || i == 9)
                                {
                                    Child.gameObject.SetActive(!ModMain.IsMultiplayer());
                                }
                            }
                        }
                    }
                }
                Transform RestOnly = ControllOffset.GetChild(2);

                if (RestOnly)
                {
                    Transform Lable_RestDuration = RestOnly.GetChild(3);

                    if (Lable_RestDuration)
                    {
                        Lable_RestDuration.gameObject.SetActive(!ModMain.IsMultiplayer());
                    }

                    Transform Lable_Description = RestOnly.GetChild(2);

                    if (Lable_Description)
                    {
                        UILocalize Loca = Lable_Description.gameObject.GetComponent<UILocalize>();

                        if (Loca)
                        {
                            if (ModMain.IsMultiplayer())
                            {
                                Loca.key = "GAMEPLAY_RestDescription_Multiplayer";
                            }
                            else
                            {
                                Loca.key = "GAMEPLAY_RestDescription";
                            }
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "Enable", new System.Type[] { typeof(bool)})]
        private static class Panel_Rest_Enable
        {
            private static bool Prefix(Panel_Rest __instance, bool enable)
            {
                PatchPanel(__instance);
                if (!ModMain.IsMultiplayer()) { return true; }

                if (enable)
                {
                    if (ModMain.Client != null && ModMain.Client.m_IsReady)
                    {
                        if (!ModMain.Client.m_Rules.m_CanUseBeds)
                        {
                            return false;
                        }
                    }

                    __instance.SetPassTimeAllowed(false);
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "Enable", new System.Type[] { typeof(bool), typeof(bool)})]
        private static class Panel_Rest_Enable2
        {
            private static bool Prefix(Panel_Rest __instance, bool enable)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (enable)
                {
                    if (ModMain.Client != null && ModMain.Client.m_IsReady)
                    {
                        if (!ModMain.Client.m_Rules.m_CanUseBeds)
                        {
                            return false;
                        }
                    }

                    __instance.SetPassTimeAllowed(false);
                    PatchPanel(__instance);
                }

                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "SetPassTimeAllowed")]
        private static class Panel_Rest_SetPassTimeAllowed
        {
            private static void Postfix(Panel_Rest __instance, bool allowed)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                __instance.m_PassTimeIsAllowed = false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Rest), "BeginSleeping", new System.Type[] { typeof(Bed), typeof(int), typeof(int) })]
        private static class Rest_BeginSleeping
        {
            private static bool Prefix(Rest __instance, Bed b, int durationHours, int maxHours)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                OnStartSleeping(__instance, b);
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Rest), "BeginSleeping", new System.Type[] { typeof(Bed), typeof(int), typeof(int), typeof(float), typeof(Rest.PassTimeOptions), typeof(Il2CppSystem.Action) })]
        private static class Rest_BeginSleeping2
        {
            private static bool Prefix(Rest __instance, Bed b, int durationHours, int maxHours, float fadeOutDuration, Rest.PassTimeOptions options, Il2CppSystem.Action onSleepEnd)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                OnStartSleeping(__instance, b);
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Rest), "EndSleeping")]
        private static class Rest_EndSleeping
        {
            private static bool Prefix(Rest __instance, bool interrupted)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (__instance.m_Bed)
                {
                    if (!interrupted)
                    {
                        __instance.m_Bed.PlayCloseAudio();
                    }
                }
                __instance.m_Sleeping = false;
                __instance.m_Bed = null;
                GameManager.GetFatigueComponent().HeavyBreathingInit();
                OnWakeUp();
                Panel_HUD Panel = InterfaceManager.GetPanel<Panel_HUD>();

                if (Panel)
                {
                    Panel.EnableForcedTimeOfDayScaleDisplay(false);
                }
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "OnRest", new System.Type[] {})]
        private static class Panel_Rest_OnRest
        {
            public static bool s_ByPass = false;
            private static bool Prefix(Panel_Rest __instance)
            {
                if (!ModMain.IsMultiplayer() || s_ByPass) { return true; }

                if (__instance.m_Bed)
                {
                    string GUID = "";

                    ObjectGuid ObjGUID = __instance.m_Bed.gameObject.GetComponent<ObjectGuid>();

                    if (ObjGUID)
                    {
                        GUID = ObjGUID.Get();
                    }

                    if (!string.IsNullOrEmpty(GUID))
                    {
                        PlayersManager.s_LastTryInteractionObject = __instance.m_Bed.gameObject;
                        ClientSend.SendTryInteract(GUID, true);
                    }
                }

                s_DesiredTimeToSleep = __instance.m_SleepHours * 60 * 60;


                return false;
            }
        }

        public static bool DoRest()
        {
            Panel_Rest Panel = InterfaceManager.GetPanel<Panel_Rest>();

            if (Panel)
            {
                Panel_Rest_OnRest.s_ByPass = true;
                Panel.DoRest(12, true);
                Panel_Rest_OnRest.s_ByPass = false;

                if (GameManager.m_Rest && GameManager.m_Rest.m_Bed) // Значит уснул
                {
                    return true;
                }
            }
            return false;
        }

        //[HarmonyLib.HarmonyPatch(typeof(Rest), "ShouldInterruptSleep")]
        //private static class Rest_ShouldInterruptSleep
        //{
        //    private static void Postfix(Rest __instance, ref bool __result)
        //    {
        //        if (!ModMain.IsMultiplayer()) { return; }

        //        __result = false;
        //    }
        //}

        //[HarmonyLib.HarmonyPatch(typeof(Rest), "AllowUnlimitedSleep")]
        //private static class Rest_AllowUnlimitedSleep
        //{
        //    private static void Postfix(Rest __instance, ref bool __result)
        //    {
        //        if (!ModMain.IsMultiplayer()) { return; }

        //        __result = true;
        //    }
        //}

        //[HarmonyLib.HarmonyPatch(typeof(Rest), "UpdateCondition")]
        //private static class Rest_UpdateCondition
        //{
        //    private static void Postfix(Rest __instance, float todSeconds)
        //    {
        //        if (!ModMain.IsMultiplayer()) { return; }

        //        SkyCoop.Logger.Log($"UpdateCondition {todSeconds} seconds");
        //    }
        //}

        //[HarmonyLib.HarmonyPatch(typeof(Rest), "UpdateFatigue")]
        //private static class Rest_UpdateFatigue
        //{
        //    private static void Postfix(Rest __instance, float todSeconds)
        //    {
        //        if (!ModMain.IsMultiplayer()) { return; }

        //        SkyCoop.Logger.Log($"UpdateFatigue {todSeconds} seconds");
        //    }
        //}

        //[HarmonyLib.HarmonyPatch(typeof(Rest), "UpdateRestForCures")]
        //private static class Rest_UpdateRestForCures
        //{
        //    private static void Postfix(Rest __instance, float todSeconds)
        //    {
        //        if (!ModMain.IsMultiplayer()) { return; }

        //        SkyCoop.Logger.Log($"UpdateRestForCures {todSeconds} seconds");
        //    }
        //}

        [HarmonyLib.HarmonyPatch(typeof(Rest), "UpdateWhenSleeping")]
        private static class Rest_UpdateWhenSleeping
        {
            private static void Postfix(Rest __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (!__instance.m_TimeAccelerated)
                {
                    float todseconds = GameManager.GetTimeOfDayComponent().GetTODSeconds(Time.deltaTime) * 2;
                    __instance.UpdateCondition(todseconds);
                    __instance.UpdateRestForCures(todseconds);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Rest), "AllowedToSleepAmount")]
        private static class Rest_AllowedToSleepAmount
        {
            private static void Postfix(Rest __instance, int amount, ref bool __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                __result = true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_GenericProgressBar), "CanUserCancelAction")]
        private static class Panel_GenericProgressBar_CanUserCancelAction
        {
            private static void Postfix(Panel_GenericProgressBar __instance, ref bool __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if(GameManager.m_Rest && GameManager.m_Rest.IsSleeping())
                {
                    __result = true;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_HUD), "ItemProgressBarCheck")]
        private static class Panel_HUD_ItemProgressBarCheck
        {
            private static void Postfix(Panel_HUD __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (GameManager.m_Rest && GameManager.m_Rest.IsSleeping())
                {
                    if (__instance.m_AccelTimePopup && __instance.m_AccelTimePopup.m_BackButtonObject)
                    {
                        __instance.m_AccelTimePopup.m_BackButtonObject.SetActive(true);
                        __instance.m_GenericButtonLegendContainer.UpdateButton("Escape", "GAMEPLAY_Cancel", true, 0, true);
                        __instance.m_TimePopupButtonLegendContainer.UpdateButton("Escape", "GAMEPLAY_Cancel", true, 0, true);
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "OnPickUp")]
        private static class Panel_Rest_OnPickUp
        {
            private static Bed s_Bed = null;
            private static void Prefix(Panel_Rest __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                s_Bed = __instance.m_Bed;
            }
            private static void Postfix(Panel_Rest __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (s_Bed)
                {
                    Comps.DroppedGearVisual GearVisual = s_Bed.GetComponent<Comps.DroppedGearVisual>();

                    if (GearVisual)
                    {
                        GearsSync.TryPickUp(GearVisual, false, true);
                    }
                }
            }
        }

        public static void OnCancleSleeping()
        {
            if (GameManager.m_Rest && GameManager.m_Rest.IsSleeping())
            {
                GameManager.m_Rest.EndSleeping(false);
            }
        }

        public static void OnStartSleeping(Rest Rest, Bed b)
        {
            Rest.m_NumSecondsSleeping = 0;
            Rest.MaybeTriggerAchievement(b);
            Rest.m_Bed = b;
            Rest.m_ShouldInterruptWhenFreezing = false;
            Rest.m_Sleeping = true;
            Rest.m_WakeUpAtFullRest = true;
            Rest.m_SleepDurationHours = 12;
            Rest.m_SleepDurationSeconds = 43200;
            GameManager.GetLogComponent().AddItem(Localization.Get("GAMEPLAY_Wenttosleep"));

            Panel_HUD Panel = InterfaceManager.GetPanel<Panel_HUD>();

            if (Panel)
            {
                Panel.EnableForcedTimeOfDayScaleDisplay(true);

                if (Panel.m_AccelTimePopup && Panel.m_AccelTimePopup.m_RestingObject)
                {
                    Panel.m_AccelTimePopup.m_BackButtonObject.SetActive(true);
                    Panel.m_GenericButtonLegendContainer.UpdateButton("Escape", "GAMEPLAY_Cancel", true, 0, true);
                    Panel.m_TimePopupButtonLegendContainer.UpdateButton("Escape", "GAMEPLAY_Cancel", true, 0, true);
                    Transform RestingLable = Panel.m_AccelTimePopup.m_RestingObject.transform.GetChild(1);
                    if (RestingLable)
                    {
                        UILocalize Localize = RestingLable.gameObject.GetComponent<UILocalize>();

                        if (Localize)
                        {
                            Localize.key = "GAMEPLAY_RestingProgress_RT";
                            Localize.OnLocalize();
                        }
                    }
                    Action CancleSleep = new Action(() => OnCancleSleeping());
                    Panel.m_AccelTimePopup.m_CancelCallback = CancleSleep;
                }
            }
        }

        public static void OnWakeUp()
        {
            vp_FPSCamera Camera = GameManager.GetVpFPSCamera();

            if (Camera)
            {
                Camera.enabled = true;
            }
            ClientSend.SendFinishInteract(); // Особождаем кровать
        }

        public static void LateUpdate()
        {
            if (!ModMain.IsMultiplayer())
            {
                return;
            }

            if (GameManager.m_Rest)
            {
                Bed Bed = GameManager.GetRestComponent().m_Bed;
                if (Bed)
                {
                    vp_FPSCamera Camera = GameManager.GetVpFPSCamera();

                    if (Camera)
                    {
                        Transform bedTransform = Bed.m_BodyPlacementTransform;
                        Quaternion targetRotation = bedTransform.rotation * Quaternion.Euler(0, -90, 0);

                        Vector3 offset = targetRotation * Vector3.forward * -0.45f;

                        if (Bed.gameObject.name.StartsWith("GEAR_"))
                        {
                            offset.y += 0.25f;
                        }

                        Camera.transform.position = bedTransform.position + offset;
                        Camera.transform.rotation = targetRotation;
                        Camera.enabled = false;
                    }
                }



                if (GameManager.m_Rest.IsSleeping())
                {
                    if (InputManager.GetEscapePressed(InputManager.m_CurrentContext) || (Utils.IsGamepadActive() && InputManager.GetPutBackPressed(InputManager.m_CurrentContext)) || GameManager.m_Rest.m_NumSecondsSleeping > s_DesiredTimeToSleep)
                    {
                        OnCancleSleeping();
                    }
                }

                if (!s_LastEveryoneIsSleeping)
                {
                    if (GameManager.m_Rest.m_TimeAccelerated)
                    {
                        GameManager.m_Rest.RestoreTimeOfDay();
                    }
                }
                else
                {
                    if (GameManager.m_Rest.IsSleeping())
                    {
                        if (!GameManager.m_Rest.m_TimeAccelerated)
                        {
                            GameManager.m_Rest.AccelerateTimeOfDay(720, 30, false);
                            Panel_HUD Panel = InterfaceManager.GetPanel<Panel_HUD>();

                            if (Panel)
                            {
                                if (Panel.m_AccelTimePopup && Panel.m_AccelTimePopup.m_RestingObject)
                                {
                                    Transform RestingLable = Panel.m_AccelTimePopup.m_RestingObject.transform.GetChild(1);
                                    if (RestingLable)
                                    {
                                        UILocalize Localize = RestingLable.gameObject.GetComponent<UILocalize>();

                                        if (Localize)
                                        {
                                            Localize.key = "GAMEPLAY_RestingProgress_Accelerated";
                                            Localize.OnLocalize();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
