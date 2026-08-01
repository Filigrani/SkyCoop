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

        public float m_TimeScale = 1f; // Скейлер ИГРОВОГО времени

        public float m_ElapsedInGameHours = 0; // Сколько игровых часов сервер запущен
        public float m_TODInHours = 12; // 12:00
        public float m_TODTimeNormalized = 0.5f; // Диапазон 0 - 1, 0.5 = 12:00

        // Сколько реальных секунд длиться день в TLD
        public const int с_SecondsInCycle = 7200; // (24 * 60) * 60) / 12    Время в TLD идёт в 12 раз быстрее чем в реале.

        // Сколько реальных секунд длиться час в TLD
        public const int c_SecondsInHour = 300; // (60 * 60) / 12

        // Сколько реальных секунд длиться минута в TLD
        public const int c_SecondsInMinute = 5; // 60 / 12

        // Сколько реальных секунд длиться секунда в TLD
        public const float c_SecondsInSecond = 0.083f; // 1 / 12

        public Server m_ServerInstance;


        public Timeline(Server Server)
        {
            m_ServerInstance = Server;
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

            ServerSend.SendTime(m_ServerInstance, m_TODTimeNormalized, m_ElapsedInGameHours);
        }

        public void UpdateEverySecond()
        {
            m_Time++; // Обновляем реальное количество секунд.

            float ElapsedInGameHours = 1f / (float)c_SecondsInHour; // Сколько игровых часов прошло.

            ElapsedInGameHours = ElapsedInGameHours * m_TimeScale;

            m_TODInHours += ElapsedInGameHours;
            m_ElapsedInGameHours += ElapsedInGameHours;

            if (m_TODInHours > 24)
            {
                m_TODInHours -= 24f; // таким образом останеться остаток.
            }
            m_TODTimeNormalized = m_TODInHours / 24f;

            ServerSend.SendTime(m_ServerInstance, m_TODTimeNormalized, m_ElapsedInGameHours);
        }
    }
}
