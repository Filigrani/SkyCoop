using Il2Cpp;
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

        [HarmonyLib.HarmonyPatch(typeof(Panel_BreakDown), "BreakDownFinished")]
        private static class Panel_BreakDown_BreakDownFinished
        {
            private static void Prefix(Panel_BreakDown __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (__instance.m_BreakDown)
                {
                    ObjectGuid ObjGUID = __instance.m_BreakDown.gameObject.GetComponent<ObjectGuid>();

                    if(ObjGUID != null)
                    {
                        ClientSend.SendBreakDown(ObjGUID.Get());
                    }
                }
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


        [HarmonyLib.HarmonyPatch(typeof(BreakDown), "Deserialize")]
        private static class HarvestableManagern_Deserialize
        {
            private static bool Prefix(BreakDown __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BreakDown), "Serialize")]
        private static class HarvestableManagern_Serialize
        {
            private static bool Prefix(BreakDown __instance)
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
    }
}
