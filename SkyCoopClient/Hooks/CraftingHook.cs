using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2Cpp;
using Il2CppSteamworks;
using Il2CppTLD.Gear;
using MelonLoader;
using SkyCoopClient;
using UnityEngine;

namespace SkyCoop
{
    public static class CraftingHook
    {
        public struct BlueprintOverrideData
        {
            public int m_UnmodifiedDuration;
            public int m_ModifiedDuration;

            public BlueprintOverrideData(int unmodifiedDuration, int modifiedDuration)
            {
                m_UnmodifiedDuration = unmodifiedDuration;
                m_ModifiedDuration = modifiedDuration;
            }
        }

        public static Dictionary<string, BlueprintOverrideData> s_BlueprintsOverrides = new Dictionary<string, BlueprintOverrideData>();

        public static void MayOverrideBlueprint(BlueprintData Blueprint)
        {
            BlueprintOverrideData OverrideData;
            if (s_BlueprintsOverrides.TryGetValue(Blueprint.name, out OverrideData))
            {
                if (ModMain.IsMultiplayer())
                {
                    Blueprint.m_DurationMinutes = OverrideData.m_ModifiedDuration;
                }
                else
                {
                    Blueprint.m_DurationMinutes = OverrideData.m_UnmodifiedDuration;
                }
            }
            else
            {
                OverrideData = new BlueprintOverrideData(Blueprint.m_DurationMinutes, (int)Math.Round((float)Blueprint.m_DurationMinutes * 0.2f));
                s_BlueprintsOverrides.Add(Blueprint.name, OverrideData);

                Blueprint.m_DurationMinutes = OverrideData.m_ModifiedDuration;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Crafting), "ApplyFilter")]
        private static class Panel_Crafting_ApplyFilter
        {
            private static void Postfix(Panel_Crafting __instance)
            {
                foreach (BlueprintData Blueprint in __instance.m_Blueprints)
                {
                    MayOverrideBlueprint(Blueprint);
                }
            }
        }


        [HarmonyLib.HarmonyPatch(typeof(Panel_Crafting), "Enable", new System.Type[] { typeof(bool) })]
        private static class Panel_Crafting_Enable
        {
            private static bool Prefix(Panel_Crafting __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return ModMain.Client != null && ModMain.Client.m_IsReady && ModMain.Client.m_Rules.m_CanCraft;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Crafting), "Enable", new System.Type[] { typeof(bool), typeof(bool) })]
        private static class Panel_Crafting_Enable2
        {
            private static bool Prefix(Panel_Crafting __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return ModMain.Client != null && ModMain.Client.m_IsReady && ModMain.Client.m_Rules.m_CanCraft;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Crafting), "CraftingStart")]
        private static class Panel_Crafting_CraftingStart
        {
            private static void Prefix(Panel_Crafting __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                int InGameMinutes = __instance.GetFinalCraftingTimeWithAllModifiers();

                if (__instance.m_RequirementContainer.m_TimeSelect.gameObject.activeSelf)
                {
                    InGameMinutes = __instance.m_RequirementContainer.m_TimeSelect.m_DisplayedCraftingTime;
                }

                float RealTimeSeconds = (InGameMinutes * 60) / 12;

                __instance.m_CraftingDisplayTimeSeconds = RealTimeSeconds * 1.003f;
            }
        }
    }
}
