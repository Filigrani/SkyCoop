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

        public struct BlueprintOverrideDataMinMax
        {
            public Vector2Int m_UnmodifiedDuration;
            public Vector2Int m_ModifiedDuration;

            public BlueprintOverrideDataMinMax(Vector2Int unmodifiedDuration, Vector2Int modifiedDuration)
            {
                m_UnmodifiedDuration = unmodifiedDuration;
                m_ModifiedDuration = modifiedDuration;
            }
        }

        public static Dictionary<string, BlueprintOverrideData> s_BlueprintsOverrides = new Dictionary<string, BlueprintOverrideData>();
        public static Dictionary<string, BlueprintOverrideData> s_HarvestOverrides = new Dictionary<string, BlueprintOverrideData>();
        public static Dictionary<string, BlueprintOverrideData> s_RepariableOverrides = new Dictionary<string, BlueprintOverrideData>();
        public static Dictionary<string, BlueprintOverrideDataMinMax> s_SharpingOverrides = new Dictionary<string, BlueprintOverrideDataMinMax>();
        public static Dictionary<string, BlueprintOverrideDataMinMax> s_ClenableOverrides = new Dictionary<string, BlueprintOverrideDataMinMax>();

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

                MayOverrideBlueprint(Blueprint);
            }
        }

        public static void MayOverrideBlueprint(HarvestBase Blueprint)
        {
            BlueprintOverrideData OverrideData;
            if (s_HarvestOverrides.TryGetValue(Blueprint.name, out OverrideData))
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
                s_HarvestOverrides.Add(Blueprint.name, OverrideData);

                MayOverrideBlueprint(Blueprint);
            }
        }

        public static void MayOverrideBlueprint(Repairable Blueprint)
        {
            BlueprintOverrideData OverrideData;
            if (s_RepariableOverrides.TryGetValue(Blueprint.name, out OverrideData))
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
                s_RepariableOverrides.Add(Blueprint.name, OverrideData);

                MayOverrideBlueprint(Blueprint);
            }
        }

        public static void MayOverrideBlueprint(Sharpenable Blueprint)
        {
            BlueprintOverrideDataMinMax OverrideData;
            if (s_SharpingOverrides.TryGetValue(Blueprint.name, out OverrideData))
            {
                if (ModMain.IsMultiplayer())
                {
                    Blueprint.m_DurationMinutesMin = OverrideData.m_ModifiedDuration.x;
                    Blueprint.m_DurationMinutesMax = OverrideData.m_ModifiedDuration.y;
                }
                else
                {
                    Blueprint.m_DurationMinutesMin = OverrideData.m_UnmodifiedDuration.x;
                    Blueprint.m_DurationMinutesMax = OverrideData.m_UnmodifiedDuration.y;
                }
            }
            else
            {
                OverrideData = new BlueprintOverrideDataMinMax(new Vector2Int(Blueprint.m_DurationMinutesMin, Blueprint.m_DurationMinutesMax),
                    new Vector2Int((int)Math.Round((float)Blueprint.m_DurationMinutesMin * 0.2f), (int)Math.Round((float)Blueprint.m_DurationMinutesMax * 0.2f)));
                s_SharpingOverrides.Add(Blueprint.name, OverrideData);

                MayOverrideBlueprint(Blueprint);
            }
        }

        public static void MayOverrideBlueprint(Cleanable Blueprint)
        {
            BlueprintOverrideDataMinMax OverrideData;
            if (s_ClenableOverrides.TryGetValue(Blueprint.name, out OverrideData))
            {
                if (ModMain.IsMultiplayer())
                {
                    Blueprint.m_DurationMinutesMin = OverrideData.m_ModifiedDuration.x;
                    Blueprint.m_DurationMinutesMax = OverrideData.m_ModifiedDuration.y;
                }
                else
                {
                    Blueprint.m_DurationMinutesMin = OverrideData.m_UnmodifiedDuration.x;
                    Blueprint.m_DurationMinutesMax = OverrideData.m_UnmodifiedDuration.y;
                }
            }
            else
            {
                OverrideData = new BlueprintOverrideDataMinMax(new Vector2Int(Blueprint.m_DurationMinutesMin, Blueprint.m_DurationMinutesMax),
                    new Vector2Int((int)Math.Round((float)Blueprint.m_DurationMinutesMin * 0.2f), (int)Math.Round((float)Blueprint.m_DurationMinutesMax * 0.2f)));
                s_ClenableOverrides.Add(Blueprint.name, OverrideData);

                MayOverrideBlueprint(Blueprint);
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

        public static void ManualPatchHarvest(GearItem gi)
        {
            if (gi.m_Harvest)
            {
                MayOverrideBlueprint(gi.m_Harvest);
            }
            if (gi.m_Repairable)
            {
                MayOverrideBlueprint(gi.m_Repairable);
            }
            if (gi.m_Sharpenable)
            {
                MayOverrideBlueprint(gi.m_Sharpenable);
            }
            if (gi.m_Cleanable)
            {
                MayOverrideBlueprint(gi.m_Cleanable);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Inventory_Examine), "AccelerateTimeOfDay")]
        private static class Panel_Inventory_Examine_AccelerateTimeOfDay
        {
            private static bool Prefix(Panel_Inventory_Examine __instance, int minutes, bool doFadeout)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                TimeOfDay timeOfDayComponent = GameManager.GetTimeOfDayComponent();
                __instance.m_DayLengthScaleBeforeRepair = timeOfDayComponent.GetDayLengthScale();

                float RealTimeSeconds = (minutes * 60) / 12;

                __instance.m_ProgressBarTimeSeconds = RealTimeSeconds;

                timeOfDayComponent.Accelerate(RealTimeSeconds * 1.003f, (float)minutes / 60f, true);

                __instance.m_TimeAccelerated = true;
                Logger.Log(ConsoleColor.Red, "Panel_Inventory_Examine.AccelerateTimeOfDay");
                return false;
            }
        }
    }
}
