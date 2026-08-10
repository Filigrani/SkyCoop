using Il2Cpp;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoopClient
{
    public static class SleepHook
    {
        [HarmonyLib.HarmonyPatch(typeof(Bed), "Awake")]
        private static class Bed_Awake
        {
            private static void Postfix(Bed __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                //__instance.enabled = false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Fatigue), "Update")]
        private static class Fatigue_Update
        {
            private static void Postfix(Fatigue __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                //__instance.m_CurrentFatigue = 0;
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

                    Transform HoursSelect = Shared.GetChild(4);

                    if (HoursSelect)
                    {
                        HoursSelect.gameObject.SetActive(false);
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
            private static void Prefix(Panel_Rest __instance, bool enable)
            {
                PatchPanel(__instance);
                if (!ModMain.IsMultiplayer()) { return; }

                if (enable)
                {
                    __instance.SetPassTimeAllowed(false);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "Enable", new System.Type[] { typeof(bool), typeof(bool)})]
        private static class Panel_Rest_Enable2
        {
            private static void Prefix(Panel_Rest __instance, bool enable)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (enable)
                {
                    __instance.SetPassTimeAllowed(false);
                    PatchPanel(__instance);
                }
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

                __instance.MaybeTriggerAchievement(b);
                __instance.m_Bed = b;
                __instance.m_ShouldInterruptWhenFreezing = false;
                __instance.m_Sleeping = true;
                __instance.m_WakeUpAtFullRest = true;
                __instance.m_SleepDurationHours = 12;
                __instance.m_SleepDurationSeconds = 8640f;
                GameManager.GetLogComponent().AddItem(Localization.Get("GAMEPLAY_Wenttosleep"));

                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Rest), "BeginSleeping", new System.Type[] { typeof(Bed), typeof(int), typeof(int), typeof(float), typeof(Rest.PassTimeOptions), typeof(Il2CppSystem.Action) })]
        private static class Rest_BeginSleeping2
        {
            private static bool Prefix(Rest __instance, Bed b, int durationHours, int maxHours, float fadeOutDuration, Rest.PassTimeOptions options, Il2CppSystem.Action onSleepEnd)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                __instance.MaybeTriggerAchievement(b);
                __instance.m_Bed = b;
                __instance.m_ShouldInterruptWhenFreezing = false;
                __instance.m_Sleeping = true;
                __instance.m_WakeUpAtFullRest = true;
                __instance.m_SleepDurationHours = 12;
                __instance.m_SleepDurationSeconds = 8640f;
                GameManager.GetLogComponent().AddItem(Localization.Get("GAMEPLAY_Wenttosleep"));

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
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Rest), "ShouldInterruptSleep")]
        private static class Rest_ShouldInterruptSleep
        {
            private static void Postfix(Rest __instance, ref bool __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                __result = false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Rest), "AllowUnlimitedSleep")]
        private static class Rest_AllowUnlimitedSleep
        {
            private static void Postfix(Rest __instance, ref bool __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                __result = true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_PauseMenu), "Enable")]
        private static class Panel_PauseMenu_Enable
        {
            private static bool Prefix(Panel_PauseMenu __instance, bool enable)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                Rest Rest = GameManager.m_Rest;

                if (Rest && Rest.m_Bed && Rest.IsSleeping())
                {
                    if (enable)
                    {
                        Rest.EndSleeping(false);
                        return false;
                    }
                }
                return true;
            }
        }

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

                float todseconds = GameManager.GetTimeOfDayComponent().GetTODSeconds(Time.deltaTime) * 2;
                __instance.UpdateCondition(todseconds);
                __instance.UpdateRestForCures(todseconds);
            }
        }

        public static void OnWakeUp()
        {
            vp_FPSCamera Camera = GameManager.GetVpFPSCamera();

            if (Camera)
            {
                Camera.enabled = true;
            }
        }

        public static void LateUpdate()
        {
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

                        Camera.transform.position = bedTransform.position + offset;
                        Camera.transform.rotation = targetRotation;
                        Camera.enabled = false;
                    }
                }
            }
        }
    }
}
