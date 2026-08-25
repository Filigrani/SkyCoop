using Il2Cpp;
using Il2CppEasyRoads3Dv3;
using Il2CppTLD.PDID;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoopClient
{
    public static class BreakDownHook
    {
        [HarmonyLib.HarmonyPatch(typeof(BreakDown), "Awake")]
        private static class BreakDown_Harvest
        {
            private static void Postfix(BreakDown __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                __instance.m_TimeCostHours = __instance.m_TimeCostHours * 0.2f;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_BreakDown), "Enable")]
        private static class Panel_BreakDown_Enable
        {
            private static void Postfix(Panel_BreakDown __instance, bool enable)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (!enable)
                {
                    ClientSend.SendFinishInteract();

                    if (__instance.m_BreakDown && !__instance.m_BreakDown.gameObject.active)
                    {
                        ObjectGuid ObjGUID = __instance.m_BreakDown.gameObject.GetComponent<ObjectGuid>();

                        if (ObjGUID != null)
                        {
                            ClientSend.SendBreakDown(ObjGUID.Get());
                        }
                        __instance.m_BreakDown = null;
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_BreakDown), "OnBreakDown")]
        private static class Panel_BreakDown_OnBreakDown
        {
            private static void Prefix(Panel_BreakDown __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                float RealTimeSeconds = (__instance.m_DurationHours * 60 * 60) / 12;

                __instance.m_SecondsToBreakDown = RealTimeSeconds * 1.003f;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BreakDown), "PerformInteraction")]
        private static class HarvestableManagern_PerformInteraction
        {
            public static bool s_ByPass = false;
            private static bool Prefix(BreakDown __instance)
            {
                if (!ModMain.IsMultiplayer() || s_ByPass) { return true; }

                PlayersManager.TryInteract(__instance);
                return false;
            }
        }


        [HarmonyLib.HarmonyPatch(typeof(BreakDown), "DeserializeAll")]
        private static class HarvestableManagern_DeserializeAll
        {
            private static bool Prefix()
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BreakDown), "SerializeAll")]
        private static class HarvestableManagern_SerializeAll
        {
            private static bool Prefix()
            {
                if (!ModMain.IsMultiplayer()) { return true; }
                return false;
            }
        }

        public static void HandleRemove(string GUID)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if (Obj != null)
            {
                BreakDown BreakDown = Obj.GetComponent<BreakDown>();

                if (BreakDown != null)
                {
                    BreakDown.DoBreakDown(false);
                }
            }
        }

        public static void DoBreak(BreakDown BreakDown)
        {
            if (BreakDown != null)
            {
                HarvestableManagern_PerformInteraction.s_ByPass = true;
                BreakDown.PerformInteraction();
                HarvestableManagern_PerformInteraction.s_ByPass = false;
            }
        }

        public static void SpeedUp()
        {
            Panel_BreakDown Panel = InterfaceManager.GetPanel<Panel_BreakDown>();
            if (Panel && Panel.IsBreakingDown())
            {
                Panel_HUD HUD = InterfaceManager.GetPanel<Panel_HUD>();

                if (HUD)
                {
                    if (HUD.m_AccelTimePopup)
                    {
                        if (HUD.m_AccelTimePopup.m_Slider)
                        {
                            float RealTimeSeconds = (Panel.m_DurationHours * 60 * 60) / 12;
                            if (Panel.m_SecondsToBreakDown != RealTimeSeconds / (float)DataStr.c_SpeedUpTimeScale)
                            {
                                float CurrentProgress = HUD.m_AccelTimePopup.m_Slider.value;

                                GameManager.GetTimeOfDayComponent().Accelerate(DataStr.c_SpeedUpRealSecondsDuration, DataStr.c_SpeedUpHours, false);
                                Panel.m_SecondsToBreakDown = RealTimeSeconds / (float)DataStr.c_SpeedUpTimeScale;
                                Panel.m_TimeSpentBreakingDown = Panel.m_SecondsToBreakDown * CurrentProgress;
                                SkyCoop.Logger.Log($"Breakdown speedup");
                            }
                        }
                    }
                }
            }
        }

        public static void SlowDown()
        {
            if (GameManager.m_TimeOfDay && GameManager.m_TimeOfDay.m_DayLengthScale >= 1)
            {
                return;
            }

            Panel_BreakDown Panel = InterfaceManager.GetPanel<Panel_BreakDown>();
            if (Panel && Panel.IsBreakingDown())
            {
                Panel_HUD HUD = InterfaceManager.GetPanel<Panel_HUD>();

                if (HUD)
                {
                    if (HUD.m_AccelTimePopup)
                    {
                        if (HUD.m_AccelTimePopup.m_Slider)
                        {
                            float RealTimeSeconds = ((Panel.m_DurationHours * 60 * 60) / 12) * 1.003f;

                            if (Panel.m_SecondsToBreakDown != RealTimeSeconds)
                            {
                                float CurrentProgress = HUD.m_AccelTimePopup.m_Slider.value;

                                Panel.m_SecondsToBreakDown = RealTimeSeconds;

                                GameManager.GetTimeOfDayComponent().Accelerate(Panel.m_SecondsToBreakDown, Panel.m_DurationHours, false);
                                Panel.m_TimeSpentBreakingDown = Panel.m_SecondsToBreakDown * CurrentProgress;
                                SkyCoop.Logger.Log($"Breakdown slowdown");
                            }
                        }
                    }
                }
            }
        }
    }
}
