using Il2Cpp;
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
    public static class WeatherHook
    {
        public static int s_LastLowTempSeed = 0;
        public static int s_LastHighTempSeed = 0;
        public static int s_LastWindSeed = 0;
        public static float s_LastWindDirection = -1;

        public static void Update()
        {
            bool CanUseWeather = !ModMain.IsMultiplayer() || (ModMain.Client != null && ModMain.Client.m_IsReady && ModMain.Client.m_Rules.m_Weather);


            if (GameManager.m_Weather)
            {
                GameManager.m_Weather.enabled = CanUseWeather;
            }
            if (GameManager.m_WeatherTransition)
            {
                if (!CanUseWeather)
                {
                    GameManager.m_WeatherTransition.m_DefaultStartWeather = WeatherStage.Clear;
                    if (GameManager.m_WeatherTransition.m_CurrentWeatherSet)
                    {
                        GameManager.m_WeatherTransition.m_CurrentWeatherSet.SetDirty();
                    }
                    GameManager.m_WeatherTransition.ActivateDefaultWeatherSet();
                    WeatherTransition.m_WeatherTransitionTimeScalar = 1;
                }
            }
            if (GameManager.m_Wind)
            {
                GameManager.m_Wind.enabled = CanUseWeather;
                if (!CanUseWeather)
                {
                    GameManager.m_Wind.enabled = false;
                    GameManager.m_Wind.m_CurrentAngleDeg = 0;
                    GameManager.m_Wind.m_CurrentAngleDeg_Base = 0;
                    GameManager.m_Wind.m_CurrentMPH = 0;
                    GameManager.m_Wind.m_CurrentMPH_Base = 0;
                    GameManager.m_Wind.m_CurrentDirection = Vector3.zero;
                }
            }
        }
        
        
        [HarmonyLib.HarmonyPatch(typeof(Weather), "GenerateTempHigh")]
        private static class Weather_GenerateTempHigh
        {
            private static bool Prefix(Weather __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                UnityEngine.Random.InitState(s_LastHighTempSeed);
                __instance.m_TempHigh = UnityEngine.Random.Range(__instance.m_HighTempMinCelsius, __instance.m_HighTempMaxCelsius);

                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Weather), "GenerateTempLow")]
        private static class Weather_GenerateTempLow
        {
            private static bool Prefix(Weather __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                UnityEngine.Random.InitState(s_LastLowTempSeed);
                __instance.m_TempHigh = UnityEngine.Random.Range(__instance.m_LowTempMinCelsius, __instance.m_LowTempMaxCelsius);

                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Wind), "StartRandomPhase")]
        private static class Wind_StartRandomPhase
        {
            private static void Postfix(Wind __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                UnityEngine.Random.InitState(s_LastWindSeed);
                WindStrength strength;
                if (GameManager.GetWeatherComponent().IsBlizzard())
                {
                    strength = WindStrength.Blizzard;
                }
                else if (__instance.m_NeverCalmWind)
                {
                    strength = (WindStrength)UnityEngine.Random.Range(1, 4);
                }
                else
                {
                    strength = (WindStrength)UnityEngine.Random.Range(0, 4);
                }
                __instance.StartPhase(strength, s_LastWindDirection);
            }
        }

        // Возращяем false, что бы клиент не пытался выбрать следующую погоду сам,
        // Ибо при сне во время ускореения он может замёрзнуть/согреться от фейковой погоды
        [HarmonyLib.HarmonyPatch(typeof(WeatherTransition), "ChooseNextWeatherSet")]
        private static class WeatherTransition_ChooseNextWeatherSet
        {
            private static bool Prefix(WeatherTransition __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return false;
            }
        }

        public static void HandleWeatherSync(DataStr.WeatherSyncData Data)
        {
            Weather Weather = GameManager.GetWeatherComponent();
            if (Weather)
            {
                WeatherTransition WeatherTransition = GameManager.GetWeatherTransitionComponent();

                if (WeatherTransition)
                {
                    WeatherStage NewType = (WeatherStage)Data.m_CurrentWeatherType;
                    WeatherStage PreviousType = (WeatherStage)Data.m_PreviousWeatherType;

                    UniStormWeatherSystem Uni = GameManager.GetUniStorm();

                    if (Uni && Uni.m_CurrentRegion)
                    {
                        string CurrentRegion = Uni.m_CurrentRegion.name;

                        if(CurrentRegion != "MiningRegion" && CurrentRegion != "AirfieldRegion")
                        {
                            if(PreviousType == WeatherStage.ElectrostaticFog)
                            {
                                PreviousType = WeatherStage.DenseFog;
                            }
                            if (NewType == WeatherStage.ElectrostaticFog)
                            {
                                NewType = WeatherStage.DenseFog;
                            }
                        }
                    }

                    WeatherTransition.m_PreviousWeatherSetType = PreviousType;
                    if (WeatherTransition.m_CurrentWeatherSet && WeatherTransition.m_CurrentWeatherSet.m_CharacterizingType != NewType)
                    {
                        WeatherTransition.m_CurrentWeatherSet.Deactivate();
                    }

                    foreach (WeatherSetData Set in Weather.m_WeatherSetsForScene)
                    {
                        if(Set.m_CharacterizingType == NewType)
                        {
                            WeatherTransition.m_CurrentWeatherSet = Set;
                            break;
                        }
                    }


                    if (WeatherTransition.m_CurrentWeatherSet)
                    {
                        WeatherTransition.m_CurrentWeatherSet.m_CurrentSetDuration = Data.m_Duration;

                        if(WeatherTransition.m_CurrentWeatherSet.m_WeatherStages.Count == 1)
                        {
                            WeatherTransition.m_CurrentWeatherSet.m_WeatherStages[0].m_CurrentDuration = Data.m_Duration;
                            WeatherTransition.m_CurrentWeatherSet.m_WeatherStages[0].m_CurrentTransitionTime = Data.m_TransitionTime;
                        }
                        else
                        {
                            float FreeTimeLeft = Data.m_Duration;
                            UnityEngine.Random.InitState(Data.m_WeatherSeed);
                            bool RerollSimplified = false;
                            foreach (WeatherSetStage Stage in WeatherTransition.m_CurrentWeatherSet.m_WeatherStages)
                            {
                                if(FreeTimeLeft == 0)
                                {
                                    RerollSimplified = true;
                                    break;
                                }
                                
                                float DurationForStage = UnityEngine.Random.Range(Stage.m_DurationMinMax.x, Stage.m_DurationMinMax.y);
                                if(DurationForStage > FreeTimeLeft)
                                {
                                    DurationForStage = Stage.m_DurationMinMax.x;
                                    if (DurationForStage > FreeTimeLeft)
                                    {
                                        DurationForStage = FreeTimeLeft;
                                    }
                                }
                                FreeTimeLeft -= DurationForStage;
                                Stage.m_CurrentDuration = DurationForStage;
                                Stage.m_CurrentTransitionTime = Stage.m_TransitionTimeMinMax.x;
                            }

                            if(FreeTimeLeft > 0)
                            {
                                float ExtraTime = FreeTimeLeft / WeatherTransition.m_CurrentWeatherSet.m_WeatherStages.Count;

                                foreach (WeatherSetStage Stage in WeatherTransition.m_CurrentWeatherSet.m_WeatherStages)
                                {
                                    Stage.m_CurrentDuration += ExtraTime;
                                }
                            }

                            if (RerollSimplified)
                            {
                                FreeTimeLeft = Data.m_Duration;
                                UnityEngine.Random.InitState(Data.m_WeatherSeed);
                                float NewMax = FreeTimeLeft / Data.m_Duration;
                                foreach (WeatherSetStage Stage in WeatherTransition.m_CurrentWeatherSet.m_WeatherStages)
                                {
                                    float NewMin = Stage.m_DurationMinMax.x;

                                    if(NewMin > NewMax)
                                    {
                                        NewMin = NewMax/2;
                                    }

                                    float DurationForStage = UnityEngine.Random.Range(NewMin, NewMax);
                                    Stage.m_CurrentDuration = DurationForStage;
                                    Stage.m_CurrentTransitionTime = Stage.m_TransitionTimeMinMax.x;
                                    FreeTimeLeft -= DurationForStage;
                                }

                                if(FreeTimeLeft > 0)
                                {
                                    float ExtraTime = FreeTimeLeft / WeatherTransition.m_CurrentWeatherSet.m_WeatherStages.Count;

                                    foreach (WeatherSetStage Stage in WeatherTransition.m_CurrentWeatherSet.m_WeatherStages)
                                    {
                                        Stage.m_CurrentDuration += ExtraTime;
                                    }
                                }
                            }
                        }

                        WeatherTransition.m_CurrentWeatherSet.m_CurrentIndex = 0;
                        if (Data.m_NormalizedTime > 0f)
                        {
                            float num = WeatherTransition.m_CurrentWeatherSet.m_CurrentSetDuration * Data.m_NormalizedTime;
                            float num2 = 0f;
                            WeatherTransition.m_CurrentWeatherSet.m_CurrentIndex = WeatherTransition.m_CurrentWeatherSet.m_WeatherStages.Length - 1;
                            for (int i = 0; i < WeatherTransition.m_CurrentWeatherSet.m_WeatherStages.Length; i++)
                            {
                                float num3 = num2 + WeatherTransition.m_CurrentWeatherSet.m_WeatherStages[i].m_CurrentDuration;
                                if (num < num3)
                                {
                                    Data.m_NormalizedTime = Mathf.Clamp01((num - num2) / WeatherTransition.m_CurrentWeatherSet.m_WeatherStages[i].m_CurrentDuration);
                                    WeatherTransition.m_CurrentWeatherSet.m_CurrentIndex = i;
                                    break;
                                }
                                WeatherTransition.m_CurrentWeatherSet.m_WeatherStages[i].m_ElapsedTime = WeatherTransition.m_CurrentWeatherSet.m_WeatherStages[i].m_CurrentDuration;
                                num2 = num3;
                            }
                        }
                        WeatherTransition.m_CurrentWeatherSet.ActivateStage(Data.m_NormalizedTime, PreviousType);
                    }


                    if (s_LastLowTempSeed != Data.m_LowTempSeed)
                    {
                        s_LastLowTempSeed = Data.m_LowTempSeed;
                        Weather.GenerateTempLow();
                    }

                    if(s_LastHighTempSeed != Data.m_HighTempSeed)
                    {
                        s_LastHighTempSeed = Data.m_HighTempSeed;
                        Weather.GenerateTempHigh();
                    }
                }
            }
            Wind Wind = GameManager.GetWindComponent();

            if (Wind)
            {
                Wind.m_PhaseDurationHours = Data.m_WindDuration;
                Wind.m_PhaseElapsedTODSeconds = Data.m_WindElapsedHours * 60 * 60;

                if(s_LastWindSeed != Data.m_WindSeed)
                {
                    s_LastWindSeed = Data.m_WindSeed;
                    s_LastWindDirection = Data.m_WindDirection;
                    Wind.StartRandomPhase(false);
                }
            }
        }
    }
}
