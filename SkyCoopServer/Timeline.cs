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


        public Timeline(Server Server)
        {
            m_ServerInstance = Server;

            m_TODInHours = m_StartingTime;
            m_TODTimeNormalized = m_TODInHours / 24f;
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

            ServerSend.SendTime(m_ServerInstance, m_TODTimeNormalized, m_ElapsedInGameHours, EveryoneIsSleeping());
        }

        public bool EveryoneIsSleeping()
        {
            if (m_RTSleepOnly)
            {
                return false;
            }
            
            if (m_ServerInstance != null)
            {
                int PlayersExist = 0;
                int PlayersSleep = 0;
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
                                if(Player.m_VisualData.m_LastAction == 7) // Sleeping
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

        public void UpdateEverySecond()
        {
            m_Time++; // Обновляем реальное количество секунд.

            float ElapsedInGameHours = 1f / (float)c_SecondsInHour; // Сколько игровых часов прошло.

            float TimeScale = 1;

            bool EveryoneIsSleepingRightNow = EveryoneIsSleeping();

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
                TimeScale = 100;
            }

            ElapsedInGameHours = ElapsedInGameHours * TimeScale;

            m_TODInHours += ElapsedInGameHours;
            m_ElapsedInGameHours += ElapsedInGameHours;

            if (m_TODInHours > 24)
            {
                m_TODInHours -= 24f; // таким образом останеться остаток.
            }
            m_TODTimeNormalized = m_TODInHours / 24f;

            ServerSend.SendTime(m_ServerInstance, m_TODTimeNormalized, m_ElapsedInGameHours, EveryoneIsSleepingRightNow);

            string TimeToLog = FormatGameTime(m_StartingTime + m_ElapsedInGameHours);

            if (m_LastLoggedTime != TimeToLog)
            {
                m_LastLoggedTime = TimeToLog;
                //Logger.Log(ConsoleColor.Green, $"{TimeToLog}");
            }
        }
    }
}
