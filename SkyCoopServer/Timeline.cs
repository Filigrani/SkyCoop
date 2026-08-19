using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCoopServer
{
    public class Timeline
    {
        public ulong m_Time = 0; // Сколько сервер запущен в реальных секундах

        public float m_StartingTime = 12; // 12:00

        public float m_ElapsedInGameHours = 0; // Сколько игровых часов сервер запущен
        public float m_TODInHours = 0; 
        public float m_TODTimeNormalized = 0;

        // Сколько реальных секунд длиться день в TLD
        public const int с_SecondsInCycle = 7200; // (24 * 60) * 60) / 12    Время в TLD идёт в 12 раз быстрее чем в реале.

        // Сколько реальных секунд длиться час в TLD
        public const int c_SecondsInHour = 300; // (60 * 60) / 12

        // Сколько реальных секунд длиться минута в TLD
        public const int c_SecondsInMinute = 5; // 60 / 12

        // Сколько реальных секунд длиться секунда в TLD
        public const float c_SecondsInSecond = 0.083f; // 1 / 12

        public Server m_ServerInstance;

        public bool m_RTSleepOnly = false;

        private string m_LastLoggedTime = string.Empty;

        private bool m_LastEveryoneIsSleeping = false;
        private float m_TimeBeforeLastAcceleration = 0;

        public TODStatus m_CurrentTODStatus = TODStatus.MiddayToAfternoon;

        public class SaveData
        {
            public ulong Time { get; set; }
            public float StartingTime { get; set; }
            public float ElapsedInGameHours { get; set; }
            public float TODInHours { get; set; }
        }

        public SaveData Save()
        {
            SaveData data = new SaveData();

            data.Time = m_Time;
            data.StartingTime = m_StartingTime;
            data.ElapsedInGameHours = m_ElapsedInGameHours;
            data.TODInHours = m_TODInHours;

            return data;
        }

        public void Load(SaveData data)
        {
            m_Time = data.Time;
            m_StartingTime = data.StartingTime;
            m_ElapsedInGameHours = data.ElapsedInGameHours;
            m_TODInHours = data.TODInHours;

            m_TODTimeNormalized = m_TODInHours / 24f;

            m_CurrentTODStatus = GetCurrentTODStatus();
        }

        public enum TODStatus
        {
            NightEndToDawn,
            DawnToMorning,
            MorningToMidday,
            MiddayToAfternoon,
            AfternoonToDusk,
            DuskToNightStart,
            NightStartToNightEnd,
        }

        public static List<TODStatusWithTime> m_TimeOfDayStatusTable = new List<TODStatusWithTime>
        {
            new TODStatusWithTime(TODStatus.NightEndToDawn, 5),
            new TODStatusWithTime(TODStatus.DawnToMorning, 6),
            new TODStatusWithTime(TODStatus.MorningToMidday, 7),
            new TODStatusWithTime(TODStatus.MiddayToAfternoon, 12),
            new TODStatusWithTime(TODStatus.AfternoonToDusk, 16.5f),
            new TODStatusWithTime(TODStatus.DuskToNightStart, 18),
            new TODStatusWithTime(TODStatus.NightStartToNightEnd, 19.5f),
        };

        public struct TODStatusWithTime
        {
            public TODStatus m_Status;
            public float m_NormalizedTime;

            public TODStatusWithTime(TODStatus Status, float Hour)
            {
                m_Status = Status;
                m_NormalizedTime = Hour / 24f;
            }
        }

        public Timeline(Server Server, SaveData saveData = null)
        {
            m_ServerInstance = Server;

            if(saveData != null)
            {
                Load(saveData);
            }
            else
            {
                m_TODInHours = m_StartingTime;
                m_TODTimeNormalized = m_TODInHours / 24f;
            }
        }

        public void SkipHours(float Hours)
        {
            float ElapsedInGameHours = Hours; // Сколько игровых часов прошло.

            m_TODInHours += ElapsedInGameHours;
            m_ElapsedInGameHours += ElapsedInGameHours;

            if (m_TODInHours > 24)
            {
                m_TODInHours -= 24f; // таким образом останеться остаток.
            }
            m_TODTimeNormalized = m_TODInHours / 24f;

            UpdateTimeOfDayState();

            if (m_ServerInstance.m_Weather.m_Config != null)
            {
                m_ServerInstance.m_Weather.AddTime(ElapsedInGameHours);
            }

            int PlayersExist = 0;
            int PlayersSleep = 0;
            bool EveryoneIsSleep = EveryoneIsSleeping(out PlayersExist, out PlayersSleep);

            ServerSend.SendTime(m_ServerInstance, m_TODTimeNormalized, m_ElapsedInGameHours, EveryoneIsSleep, PlayersSleep, PlayersExist);
        }

        public bool EveryoneIsSleeping(out int PlayersExist, out int PlayersSleep)
        {
            PlayersExist = 0;
            PlayersSleep = 0;

            if (m_RTSleepOnly)
            {
                return false;
            }
            
            if (m_ServerInstance != null)
            {
                List<NetPeer> peers = new List<NetPeer>();
                m_ServerInstance.m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Peer in peers.ToArray())
                {
                    if (Peer != null)
                    {
                        DataStr.PlayerData Player = m_ServerInstance.GetPlayerDataByNetPeer(Peer);
                        if(Player != null)
                        {
                            if(Player.m_GamePlayState == DataStr.PlayerData.GamePlayState.Alive)
                            {
                                PlayersExist++;
                                if(Player.m_VisualData.m_LastAction == 7 || Player.m_IsWorking) // 7 - Sleeping
                                //if (Player.m_VisualData.m_LastAction == 7) // 7 - Sleeping
                                {
                                    PlayersSleep++;
                                }
                            }
                        }
                    }
                }

                if(PlayersExist == 0)
                {
                    return false;
                }

                return PlayersSleep == PlayersExist;
            }
            return false;
        }

        public static string FormatGameTime(float elapsedHours)
        {
            int days = (int)(elapsedHours / 24f);
            int hours = (int)(elapsedHours % 24f);
            int minutes = (int)((elapsedHours % 1f) * 60f);

            return $"Day: {days+1} Time: {hours:D2}:{minutes:D2}";
        }

        public void SetNewTimeOfDayStatus(TODStatus NewStatus)
        {
            m_CurrentTODStatus = NewStatus;
            Logger.Log(ConsoleColor.Green, $"New TOD status {NewStatus}");

            m_ServerInstance.m_Weather.MayRerollTemperature(m_TODTimeNormalized);
        }

        public void UpdateTimeOfDayState()
        {
            TODStatus newStatus = GetCurrentTODStatus();

            if (m_CurrentTODStatus != newStatus)
            {
                SetNewTimeOfDayStatus(newStatus);
            }
        }

        private TODStatus GetCurrentTODStatus()
        {
            for (int i = 0; i < m_TimeOfDayStatusTable.Count - 1; i++)
            {
                TODStatusWithTime current = m_TimeOfDayStatusTable[i];
                TODStatusWithTime next = m_TimeOfDayStatusTable[i + 1];

                if (m_TODTimeNormalized >= current.m_NormalizedTime &&
                    m_TODTimeNormalized < next.m_NormalizedTime)
                {
                    return current.m_Status;
                }
            }

            TODStatusWithTime last = m_TimeOfDayStatusTable[m_TimeOfDayStatusTable.Count - 1];
            if (m_TODTimeNormalized >= last.m_NormalizedTime)
            {
                return last.m_Status;
            }

            return m_TimeOfDayStatusTable[m_TimeOfDayStatusTable.Count - 1].m_Status;
        }

        public void UpdateEverySecond()
        {
            m_Time++; // Обновляем реальное количество секунд.

            float ElapsedInGameHours = 1f / (float)c_SecondsInHour; // Сколько игровых часов прошло.

            float TimeScale = 1;

            int PlayersExist = 0;
            int PlayersSleep = 0;

            bool EveryoneIsSleepingRightNow = EveryoneIsSleeping(out PlayersExist, out PlayersSleep);

            if (m_LastEveryoneIsSleeping != EveryoneIsSleepingRightNow)
            {
                m_LastEveryoneIsSleeping = EveryoneIsSleepingRightNow;

                if (EveryoneIsSleepingRightNow)
                {
                    m_TimeBeforeLastAcceleration = m_ElapsedInGameHours;
                }
                else
                {
                    Logger.Log(ConsoleColor.Green, $"Time acceleration finished, {m_ElapsedInGameHours - m_TimeBeforeLastAcceleration} hours skipped in total");
                }
            }
            
            if (EveryoneIsSleepingRightNow)
            {
                TimeScale = DataStr.c_SpeedUpTimeScale;
            }

            ElapsedInGameHours = ElapsedInGameHours * TimeScale;

            m_TODInHours += ElapsedInGameHours;
            m_ElapsedInGameHours += ElapsedInGameHours;

            if (m_TODInHours > 24)
            {
                m_TODInHours -= 24f; // таким образом останеться остаток.
            }
            m_TODTimeNormalized = m_TODInHours / 24f;
            UpdateTimeOfDayState();

            if (m_ServerInstance.m_Weather.m_Config != null)
            {
                m_ServerInstance.m_Weather.AddTime(ElapsedInGameHours);
            }

            ServerSend.SendTime(m_ServerInstance, m_TODTimeNormalized, m_ElapsedInGameHours, EveryoneIsSleepingRightNow, PlayersSleep, PlayersExist);

            string TimeToLog = FormatGameTime(m_StartingTime + m_ElapsedInGameHours);

            if (m_LastLoggedTime != TimeToLog)
            {
                m_LastLoggedTime = TimeToLog;
                //Logger.Log(ConsoleColor.Green, $"{TimeToLog}");
            }
        }
    }
}
