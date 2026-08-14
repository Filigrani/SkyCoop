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
        public static Dictionary<string, BlueprintOverrideData> s_ResearchItemOverrides = new Dictionary<string, BlueprintOverrideData>();

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

        public static void MayOverrideBlueprint(ResearchItem Blueprint)
        {
            BlueprintOverrideData OverrideData;
            if (s_ResearchItemOverrides.TryGetValue(Blueprint.name, out OverrideData))
            {
                if (ModMain.IsMultiplayer())
                {
                    Blueprint.m_TimeRequirementHours = OverrideData.m_ModifiedDuration;
                }
                else
                {
                    Blueprint.m_TimeRequirementHours = OverrideData.m_UnmodifiedDuration;
                }
            }
            else
            {
                int ToResearch = Blueprint.m_TimeRequirementHours;
                int ModifiedToSearch = ToResearch;
                switch (ToResearch)
                {
                    case 4:
                        ModifiedToSearch = 2;
                        break;
                    case 5:
                        ModifiedToSearch = 3;
                        break;
                    case 10:
                        ModifiedToSearch = 5;
                        break;
                    case 15:
                        ModifiedToSearch = 7;
                        break;
                    case 25:
                        ModifiedToSearch = 8;
                        break;
                    default:
                        break;
                }
                OverrideData = new BlueprintOverrideData(ToResearch, ModifiedToSearch);
                s_ResearchItemOverrides.Add(Blueprint.name, OverrideData);

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
            if (gi.m_ResearchItem)
            {
                MayOverrideBlueprint(gi.m_ResearchItem);
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

        public static void PatchPanel(Panel_Inventory_Examine Panel)
        {
            if(Panel && Panel.m_ReadPanel)
            {
                Transform GameObject = Panel.m_ReadPanel.transform.GetChild(1);

                if (GameObject)
                {
                    Transform TimeLables = GameObject.GetChild(6);

                    if (TimeLables)
                    {
                        Transform Header = TimeLables.GetChild(3);

                        if (Header)
                        {
                            UILocalize Loca = Header.gameObject.GetComponent<UILocalize>();

                            if (Loca)
                            {
                                if (ModMain.IsMultiplayer())
                                {
                                    Loca.key = "GAMEPLAY_minutes";
                                }
                                else
                                {
                                    Loca.key = "GAMEPLAY_hours";
                                }
                                Loca.OnLocalize();
                            }
                        }
                    }
                }
            }
            if (ModMain.IsMultiplayer())
            {
                Panel.m_HoursToRead = 10;
            }
            else
            {
                Panel.m_HoursToRead = 1;
            }
            Panel.RefreshReadPanel();
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Inventory_Examine), "Enable", new System.Type[] { typeof(bool) })]
        private static class Panel_Inventory_Examine_Enable
        {
            private static void Postfix(Panel_Inventory_Examine __instance)
            {
                PatchPanel(__instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Inventory_Examine), "Enable", new System.Type[] { typeof(bool), typeof(ComingFromScreenCategory) })]
        private static class Panel_Inventory_Examine_Enable2
        {
            private static void Postfix(Panel_Inventory_Examine __instance)
            {
                PatchPanel(__instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Inventory_Examine), "OnReadHoursIncrease")]
        private static class Panel_Inventory_Examine_OnReadHoursIncrease
        {
            private static bool Prefix(Panel_Inventory_Examine __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if(__instance.m_GearItem && __instance.m_GearItem.m_ResearchItem)
                {
                    float ElapsedMinutes = __instance.m_GearItem.m_ResearchItem.m_ElapsedHours * 60;
                    float LeftToReadMinutes = (__instance.m_GearItem.m_ResearchItem.m_TimeRequirementHours * 60) - ElapsedMinutes;

                    int SelectedMinutes = __instance.m_HoursToRead;

                    if (SelectedMinutes >= LeftToReadMinutes)
                    {
                        __instance.m_HoursToRead = (int)Math.Round(ElapsedMinutes);
                        GameAudioManager.PlayGUIError();
                    }
                    __instance.m_HoursToRead += 10;
                    GameAudioManager.PlayGUIScroll();
                    __instance.RefreshHoursToRead();
                }
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Inventory_Examine), "OnReadHoursDecrease")]
        private static class Panel_Inventory_Examine_OnReadHoursDecrease
        {
            private static bool Prefix(Panel_Inventory_Examine __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                float ElapsedMinutes = __instance.m_GearItem.m_ResearchItem.m_ElapsedHours * 60;
                float LeftToReadMinutes = (__instance.m_GearItem.m_ResearchItem.m_TimeRequirementHours * 60) - ElapsedMinutes;

                int SelectedMinutes = __instance.m_HoursToRead;

                if (SelectedMinutes < 20)
                {
                    GameAudioManager.PlayGUIError();
                    return false;
                }
                __instance.m_HoursToRead -= 10;
                GameAudioManager.PlayGUIScroll();
                __instance.RefreshHoursToRead();
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Inventory_Examine), "RefreshHoursToRead")]
        private static class Panel_Inventory_Examine_RefreshHoursToRead
        {
            private static bool Prefix(Panel_Inventory_Examine __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                float ElapsedMinutes = __instance.m_GearItem.m_ResearchItem.m_ElapsedHours * 60;
                float LeftToReadMinutes = (__instance.m_GearItem.m_ResearchItem.m_TimeRequirementHours * 60) - ElapsedMinutes;

                int SelectedMinutes = __instance.m_HoursToRead;

                __instance.m_TimeToReadLabel.text = __instance.m_HoursToRead.ToString();
                __instance.m_ReadHoursDecrease.gameObject.SetActive(SelectedMinutes >= 20);
                __instance.m_ReadHoursIncrease.gameObject.SetActive(SelectedMinutes != LeftToReadMinutes);

                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Inventory_Examine), "StartRead")]
        private static class Panel_Inventory_Examine_StartRead
        {
            private static bool Prefix(Panel_Inventory_Examine __instance, int durationMinutes, string readAudio)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                __instance.SelectWindow(__instance.m_ActionInProgressWindow);
                __instance.SetReadInProgress(true);
                __instance.m_Slider_ActionProgress.value = 0f;
                __instance.m_ElapsedProgressBarSeconds = 0f;
                TimeOfDay timeOfDayComponent = GameManager.GetTimeOfDayComponent();
                __instance.m_DayLengthScaleBeforeRepair = timeOfDayComponent.GetDayLengthScale();

                float RealTimeSeconds = (__instance.m_HoursToRead * 60) / 12;

                __instance.m_ProgressBarTimeSeconds = RealTimeSeconds;

                //timeOfDayComponent.Accelerate(RealTimeSeconds * 1.003f, (float)__instance.m_HoursToRead / 60f, true);

                //__instance.m_TimeAccelerated = true;
                __instance.m_ProgressBarAudio = GameAudioManager.PlaySound(readAudio, InterfaceManager.GetSoundEmitter());

                Panel_Inventory Panel = InterfaceManager.GetPanel<Panel_Inventory>();

                if (Panel)
                {
                    Panel.GetComponent<UIPanel>().alpha = 0f;
                }

                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Inventory_Examine), "UpdateActionProgressBar")]
        private static class Panel_Inventory_Examine_UpdateActionProgressBar
        {
            private static void Postfix(Panel_Inventory_Examine __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (__instance.IsReading())
                {
                    float RealTimeSeconds = (__instance.m_HoursToRead * 60) / 12;

                    __instance.m_ProgressBarTimeSeconds = RealTimeSeconds;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Inventory_Examine), "ReadComplete")]
        private static class Panel_Inventory_Examine_ReadComplete
        {
            private static bool Prefix(Panel_Inventory_Examine __instance, float normalizedProgress)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (__instance.m_GearItem == null || __instance.m_GearItem.m_ResearchItem == null)
                {
                    return false;
                }
                __instance.m_GearItem.m_ResearchItem.Read((normalizedProgress * __instance.m_HoursToRead) / 60);
                __instance.SetReadInProgress(false);
                if (__instance.m_GearItem.m_ResearchItem.IsResearchComplete())
                {
                    __instance.OnBack();
                    return false;
                }
                __instance.SelectWindow(__instance.m_MainWindow);
                __instance.RefreshReadPanel();

                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Inventory_Examine), "RefreshReadPanel")]
        private static class Panel_Inventory_Examine_RefreshReadPanel
        {
            private static void Postfix(Panel_Inventory_Examine __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if(__instance.m_GearItem && __instance.m_GearItem.m_ResearchItem)
                {
                    float ElapsedMinutes = __instance.m_GearItem.m_ResearchItem.m_ElapsedHours * 60;
                    float LeftToReadMinutes = (__instance.m_GearItem.m_ResearchItem.m_TimeRequirementHours * 60) - ElapsedMinutes;
                    float TotalMinutes = (__instance.m_GearItem.m_ResearchItem.m_TimeRequirementHours * 60);

                    int SelectedMinutes = __instance.m_HoursToRead;

                    string Research = Localization.Get("GAMEPLAY_ResearchAlreadyCompleted");
                    string Minutes = Localization.Get("GAMEPLAY_minutes");

                    string text = $"{ElapsedMinutes.ToString("F0")} / {TotalMinutes.ToString()} {Minutes} {Research}";
                    __instance.m_TimeToReadRemainingLabel.text = text;
                }
            }
        }
    }
}
