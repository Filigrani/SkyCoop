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
    public class CustomWeatherManager
    {
        public static CustomWeatherManager s_Instance;

        public int m_Seed = -5;

        public int m_Increment = 0;

        public struct WeatherTypeWeightPair
        {
            public WeatherStage m_Type;
            public int m_Weight;

            public WeatherTypeWeightPair() { }
            public WeatherTypeWeightPair(WeatherStage Type, int Weight)
            {
                m_Type = Type;
                m_Weight = Weight;
            }

        }

        public List<WeatherTypeWeightPair> GetChances(WeatherStage WeatherType)
        {
            Weather Weather = GameManager.GetWeatherComponent();

            List<WeatherTypeWeightPair> Weights = new List<WeatherTypeWeightPair>();
            
            if (Weather)
            {
                foreach (WeatherSetData Set in Weather.m_WeatherSetsForScene)
                {
                    if(Set.m_CharacterizingType == WeatherType)
                    {
                        Weights.Add(new WeatherTypeWeightPair(WeatherStage.DenseFog, Set.m_DenseFogAsNextSelectionWeight));
                        Weights.Add(new WeatherTypeWeightPair(WeatherStage.LightSnow, Set.m_LightSnowAsNextSelectionWeight));
                        Weights.Add(new WeatherTypeWeightPair(WeatherStage.HeavySnow, Set.m_HeavySnowAsNextSelectionWeight));
                        Weights.Add(new WeatherTypeWeightPair(WeatherStage.PartlyCloudy, Mathf.RoundToInt(Set.m_CloudyAsNextSelectionWeight / 2)));
                        Weights.Add(new WeatherTypeWeightPair(WeatherStage.Clear, Set.m_ClearAsNextSelectionWeight));
                        Weights.Add(new WeatherTypeWeightPair(WeatherStage.Cloudy, Mathf.RoundToInt(Set.m_CloudyAsNextSelectionWeight / 2)));
                        Weights.Add(new WeatherTypeWeightPair(WeatherStage.LightFog, Set.m_LightFogAsNextSelectionWeight));
                        Weights.Add(new WeatherTypeWeightPair(WeatherStage.Blizzard, Mathf.RoundToInt(Set.m_BlizzardAsNextSelectionWeight * GameManager.GetExperienceModeManagerComponent().GetChanceOfBlizzardScale())));
                        Weights.Add(new WeatherTypeWeightPair(WeatherStage.ClearAurora, 0));
                        Weights.Add(new WeatherTypeWeightPair(WeatherStage.ToxicFog, Set.m_ToxicFogAsNextSelectionWeight));
                        Weights.Add(new WeatherTypeWeightPair(WeatherStage.ElectrostaticFog, Set.m_ElectrostaticFogAsNextSelectionWeight));
                        return Weights;
                    }
                }
            }
            return Weights;
        }

        public WeatherStage GetWeatherStage(int Increment)
        {
            return WeatherStage.Clear;
        }

        public WeatherStage GetNextStage()
        {
            return GetWeatherStage((m_Increment + 1));
        }

        public WeatherStage GetPreviousStage()
        {
            return GetWeatherStage((m_Increment - 1));
        }

        public WeatherStage GetCurrentStage()
        {
            return GetWeatherStage(m_Increment);
        }

        public static void Set(int value)
        {
            if (s_Instance != null)
            {
                s_Instance.m_Increment = value;
            }
        }

        public static int GetSeed()
        {
            if(s_Instance != null)
            {
                return s_Instance.m_Seed;
            }
            return -1;
        }


        [HarmonyLib.HarmonyPatch(typeof(Weather), "GenerateTempHigh")]
        private static class Weather_GenerateTempHigh
        {
            private static void Postfix(Weather __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                System.Random RNG = new System.Random(GetSeed());

                __instance.m_TempHigh = RNG.Range(__instance.m_HighTempMinCelsius, __instance.m_HighTempMaxCelsius);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Weather), "GenerateTempLow")]
        private static class Weather_GenerateTempLow
        {
            private static void Postfix(Weather __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                System.Random RNG = new System.Random(GetSeed());

                __instance.m_TempHigh = RNG.Range(__instance.m_LowTempMinCelsius, __instance.m_LowTempMaxCelsius);
            }
        }
    }
}
