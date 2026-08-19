using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkyCoopServer
{
    public class WeatherManager
    {
        public int m_Seed = Guid.NewGuid().GetHashCode();
        public int m_LowTemperatureSeed = Guid.NewGuid().GetHashCode();
        public int m_HighTemperatureSeed = Guid.NewGuid().GetHashCode();
        public int m_WindSeed = Guid.NewGuid().GetHashCode();

        public WeatherType m_CurrentWeatherSetType = WeatherType.Clear;
        public WeatherType m_PreviousWeatherSetType = WeatherType.Clear;
        public float m_WindDirection = 0;

        public float m_HourCoolingBegins = 0.625f; // 15 / 24
        public float m_HourWarmingBegins = 0.291f; // 7 / 24

        public float m_HoursBetweenWindChangeMin = 1;
        public float m_HoursBetweenWindChangeMax = 3;


        // Время
        public float m_DurationHours = 0;
        public float m_ElapsedHours = 0;
        public float m_TransitionTime = 0;

        public float m_ElapsedWindHours = 0;
        public float m_WindDurationHours = 0;

        // Конфиг типов
        public WeatherSettingsConfig m_Config;
        public Dictionary<WeatherType, WeatherTypeSetting> m_WeatherSets = new Dictionary<WeatherType, WeatherTypeSetting>();

        // Аврора
        public bool m_NextNightAurora = false;
        public float m_AuroraEarlyWindowProbability = 10f;
        public float m_AuroraLateWindowProbability = 5f;
        public float m_AuroraActivationWindowEnd = 0.16f; // 4 / 24
        public float m_AuroraActivationWindowStart = 0.66f; // 14 / 24

        // Электростатический туман
        public float m_ElectrostaticFogProbability = 15f;
        public float m_ElectrostaticFogActivationWindowEnd = 0.75f; // 18 / 24
        public float m_ElectrostaticFogActivationWindowStart = 0.16f; // 4 / 24

        public Server m_ServerInstance;

        public class WeatherSettingsConfig
        {
            public List<WeatherTypeSetting> Types { get; set; }
        }

        public class WeatherTypeTransitionsWeights
        {
            public string Type { get; set; }
            public float Weight { get; set; }
        }

        public class WeatherTypeSetting
        {
            public string Type { get; set; }
            public float DurationMin { get; set; }
            public float DurationMax { get; set; }

            public float TransitionMin { get; set; }
            public float TransitionMax { get; set; }

            public List<WeatherTypeTransitionsWeights> Weights { get; set; }
        }

        public enum WeatherType
        {
            DenseFog = 0,
            LightSnow = 1,
            HeavySnow = 2,
            PartlyCloudy = 3,
            Clear = 4,
            Cloudy = 5,
            LightFog = 6,
            Blizzard = 7,
            ClearAurora = 8,
            ToxicFog = 9,
            ElectrostaticFog = 10,
        }

        public float GetBilzardScaler()
        {
            switch (m_ServerInstance.m_Config.m_ExperienceMode)
            {
                case "Pilgrim":
                    return 0.75f;
                case "Voyageur":
                    return 1f;
                case "Stalker":
                      return 1.25f;
                case "Interloper":
                case "Misery":
                    return 2f;
                default:
                    return 1;
            }
        }

        public WeatherType GetWeatherTypeFromString(string Type)
        {
            return Enum.Parse<WeatherType>(Type);
        }

        public void LoadConfig(WeatherSettingsConfig Config)
        {
            m_Config = Config;
            m_WeatherSets.Clear();


            if (m_Config != null)
            {
                foreach (WeatherTypeSetting Setting in m_Config.Types)
                {
                    WeatherType Type = GetWeatherTypeFromString(Setting.Type);
                    //Logger.Log(ConsoleColor.Yellow, $"WeatherTypeSetting Type {Type} DurationMin {Setting.DurationMin} DurationMax {Setting.DurationMax} TransitionMin {Setting.TransitionMin} TransitionMax {Setting.TransitionMax} StageTransitions {Setting.StageTransitions}");

                    if (!m_WeatherSets.ContainsKey(Type))
                    {
                        m_WeatherSets.Add(Type, Setting);
                    }
                }
            }
        }

        public WeatherTypeSetting GetWeatherSet(WeatherType Type)
        {
            WeatherTypeSetting Set = null;

            if(m_WeatherSets.TryGetValue(Type, out Set))
            {
                return Set;
            }
            return null;
        }


        public WeatherManager(Server Server, SaveData saveData = null)
        {
            m_ServerInstance = Server;

            if(saveData != null)
            {
                Load(saveData);
            }
        }

        public class SaveData
        {
            public int WeatherSeed { get; set; }
            public int LowTemperatureSeed { get; set; }
            public int HighTemperatureSeed { get; set; }
            public int WindSeed { get; set; }
            public int CurrentWeatherSetType { get; set; }
            public int PreviousWeatherSetType { get; set; }
            public float WindDirection { get; set; }
            public float DurationHours { get; set; }
            public float ElapsedHours { get; set; }
            public float TransitionTime { get; set; }
            public float ElapsedWindHours { get; set; }
            public float WindDurationHours { get; set; }
        }

        public SaveData Save()
        {
            SaveData data = new SaveData();

            data.WeatherSeed = m_Seed;
            data.LowTemperatureSeed = m_LowTemperatureSeed;
            data.HighTemperatureSeed = m_HighTemperatureSeed;
            data.WindSeed = m_WindSeed;
            data.CurrentWeatherSetType = (int)m_CurrentWeatherSetType;
            data.PreviousWeatherSetType = (int)m_PreviousWeatherSetType;
            data.WindDirection = m_WindDirection;
            data.DurationHours = m_DurationHours;
            data.ElapsedHours = m_ElapsedHours;
            data.TransitionTime = m_TransitionTime;
            data.ElapsedWindHours = m_ElapsedWindHours;
            data.WindDurationHours = m_WindDurationHours;

            return data;
        }

        public void Load(SaveData data)
        {
            m_Seed = data.WeatherSeed;
            m_LowTemperatureSeed = data.LowTemperatureSeed;
            m_HighTemperatureSeed = data.HighTemperatureSeed;
            m_WindSeed = data.WindSeed;
            m_CurrentWeatherSetType = (WeatherType)data.CurrentWeatherSetType;
            m_PreviousWeatherSetType = (WeatherType)data.PreviousWeatherSetType;
            m_WindDirection = data.WindDirection;
            m_DurationHours = data.DurationHours;
            m_ElapsedHours = data.ElapsedHours;
            m_TransitionTime = data.TransitionTime;
            m_ElapsedWindHours = data.ElapsedWindHours;
            m_WindDirection = data.WindDirection;
        }

        public bool IsEarlyNightWindowForAuroraActivation(float NormalizedTOD)
        {
            return NormalizedTOD > m_AuroraActivationWindowStart;
        }

        public bool IsLateNightWindowForAuroraActivation(float NormalizedTOD)
        {
            return NormalizedTOD < m_AuroraActivationWindowEnd;
        }

        public bool IsWindowForElectrostaticFogActivation(float NormalizedTOD)
        {
            return NormalizedTOD > m_ElectrostaticFogActivationWindowStart && NormalizedTOD < m_ElectrostaticFogActivationWindowEnd;
        }

        public void RerollHighTemp()
        {
            m_HighTemperatureSeed = Guid.NewGuid().GetHashCode();
        }

        public void RerollLowTemp()
        {
            m_LowTemperatureSeed = Guid.NewGuid().GetHashCode();
        }

        public void MayRerollTemperature(float NormalizedTOD)
        {
            if (NormalizedTOD >= m_HourWarmingBegins && NormalizedTOD < m_HourCoolingBegins)
            {
                RerollHighTemp();
            }
            else
            {
                RerollLowTemp();
            }
        }

        public void SetNewWindSeed(float ElapsedHours = 0)
        {
            int NewWindSeed = Guid.NewGuid().GetHashCode();
            Random RNG = new Random(NewWindSeed);

            m_ElapsedWindHours = ElapsedHours;
            m_WindDurationHours = RNG.Range(m_HoursBetweenWindChangeMin, m_HoursBetweenWindChangeMax);
            switch (RNG.Next(0, 8))
            {
                case 0:
                    m_WindDirection = 180f;
                    break;
                case 1:
                    m_WindDirection = 0f;
                    break;
                case 2:
                    m_WindDirection = 90f;
                    break;
                case 3:
                    m_WindDirection = 270f;
                    break;
                case 4:
                    m_WindDirection = 135f;
                    break;
                case 5:
                    m_WindDirection = 225f;
                    break;
                case 6:
                    m_WindDirection = 45f;
                    break;
                case 7:
                    m_WindDirection = 315f;
                    break;
                default:
                    m_WindDirection = 0f;
                    break;
            }

            Logger.Log(ConsoleColor.Green, $"New Wind seed {m_WindSeed} and direction {m_WindDirection} will last for {m_WindDurationHours} hours");
        }

        public void SetNewWeatherSet(WeatherType NewWeatherSetType, float ElapsedHours = 0)
        {
            Random RNG = new Random(Guid.NewGuid().GetHashCode());

            WeatherTypeSetting SetSetting = GetWeatherSet(NewWeatherSetType);

            if(SetSetting != null)
            {
                m_ElapsedHours = ElapsedHours;
                m_DurationHours = RNG.Range(SetSetting.DurationMin, SetSetting.DurationMax);
                m_TransitionTime = RNG.Range(SetSetting.TransitionMin, SetSetting.TransitionMax);

                m_PreviousWeatherSetType = m_CurrentWeatherSetType;
                m_CurrentWeatherSetType = NewWeatherSetType;
                RerollLowTemp();
                RerollHighTemp();
            }
            Logger.Log(ConsoleColor.Green, $"New WeatherSet {m_CurrentWeatherSetType} that will last for {m_DurationHours+m_TransitionTime} hours");
        }

        public void ForceNextWeather()
        {
            m_ElapsedHours = m_DurationHours + m_TransitionTime;
        }

        public WeatherType GetNextWeatherType()
        {
            Random RNG = new Random(Guid.NewGuid().GetHashCode());
            if (m_ServerInstance != null && m_ServerInstance.m_Timeline != null)
            {
                float RandomValue = RNG.Range(0f, 100f);
                float NormalizedTime = m_ServerInstance.m_Timeline.m_TODTimeNormalized;

                if (IsEarlyNightWindowForAuroraActivation(NormalizedTime))
                {
                    if(RandomValue < m_AuroraEarlyWindowProbability || m_NextNightAurora)
                    {
                        if (m_NextNightAurora)
                        {
                            m_NextNightAurora = false;
                        }
                        return WeatherType.ClearAurora;
                    }
                } else if (IsLateNightWindowForAuroraActivation(NormalizedTime))
                {
                    if (RandomValue < m_AuroraLateWindowProbability)
                    {
                        return WeatherType.ClearAurora;
                    }
                } else if (IsWindowForElectrostaticFogActivation(NormalizedTime))
                {
                    if (RandomValue < m_ElectrostaticFogProbability)
                    {
                        return WeatherType.ElectrostaticFog;
                    }
                }
            }
            
            WeatherTypeSetting CurrentSet = GetWeatherSet(m_CurrentWeatherSetType);

            if (CurrentSet != null)
            {
                List<WeatherTypeTransitionsWeights> Modified = new List<WeatherTypeTransitionsWeights>();

                float TotalWeight = 0;

                // Нам нужен ещё один список который мы модифицируем для перебора. Ибо на некоторых сложностях шансы другие.
                foreach (WeatherTypeTransitionsWeights Transition in CurrentSet.Weights)
                {
                    WeatherTypeTransitionsWeights NewTransition = new WeatherTypeTransitionsWeights();

                    NewTransition.Type = Transition.Type;
                    NewTransition.Weight = Transition.Weight;

                    // TODO Проверка на арвору

                    WeatherType Type = GetWeatherTypeFromString(Transition.Type);
                    if(Type == WeatherType.Blizzard)
                    {
                        NewTransition.Weight = NewTransition.Weight * GetBilzardScaler();
                    }

                    if(CurrentSet.Type == NewTransition.Type) // Не повторяем
                    {
                        NewTransition.Weight = 0;
                    }

                    Modified.Add(NewTransition);

                    TotalWeight += NewTransition.Weight;
                }

                if (TotalWeight <= 0)
                {
                    return WeatherType.Clear;
                }
                float RandomValue = (float)RNG.NextDouble() * TotalWeight;
                float CumulativeWeight = 0;

                foreach (WeatherTypeTransitionsWeights Transition in Modified)
                {
                    CumulativeWeight += Transition.Weight;
                    if (RandomValue <= CumulativeWeight)
                    {
                        WeatherType Choosed = GetWeatherTypeFromString(Transition.Type);

                        if(Choosed == WeatherType.Cloudy)
                        {
                            if(RNG.Range(0, 2) == 0)
                            {
                                Choosed = WeatherType.PartlyCloudy;
                            }
                        }
                        return Choosed;
                    }
                }
            }

            return WeatherType.Clear;
        }

        public void AddTime(float ElapsedHours)
        {
            m_ElapsedHours += ElapsedHours;
            m_ElapsedWindHours += ElapsedHours;

            if(m_ElapsedHours > m_DurationHours+m_TransitionTime)
            {
                float Overstock = (m_DurationHours + m_TransitionTime) - m_ElapsedHours;
                SetNewWeatherSet(GetNextWeatherType(), Overstock);
            }
            if(m_ElapsedWindHours > m_WindDurationHours)
            {
                float Overstock = m_WindDurationHours - m_ElapsedWindHours;
                SetNewWindSeed(Overstock);
            }
        }

        public DataStr.WeatherSyncData GetData()
        {
            DataStr.WeatherSyncData Data = new DataStr.WeatherSyncData();
            Data.m_WeatherSeed = m_Seed;
            Data.m_LowTempSeed = m_LowTemperatureSeed;
            Data.m_HighTempSeed = m_HighTemperatureSeed;
            Data.m_Duration = m_DurationHours;
            Data.m_TransitionTime = m_TransitionTime;
            Data.m_NormalizedTime = m_ElapsedHours / (m_DurationHours+m_TransitionTime);
            Data.m_CurrentWeatherType = (int)m_CurrentWeatherSetType;
            Data.m_PreviousWeatherType = (int)m_PreviousWeatherSetType;

            return Data;
        }

        public void UpdateEverySecond()
        {
            ServerSend.SendWeather(m_ServerInstance, GetData());
        }
    }
}
