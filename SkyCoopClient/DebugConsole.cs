using Il2Cpp;
using SkyCoop;

namespace SkyCoopClient
{
    public class DebugConsole
    {
        public static void ConsoleLog(string message)
        {
            //uConsoleLog.Add($"[SkyCoop] {message}");
        }

        public static void RegisterCommands()
        {
            uConsole.RegisterCommand("RecursiveDebug", new Action(RecursiveDebug));
        }

        public static void RecursiveDebug()
        {
            if (ModMain.Server.m_PlayersData.m_RecursiveDebug)
            {
                ModMain.Server.m_PlayersData.m_RecursiveDebug = false;
                Logger.Log("[DebugConsole] RecursiveDebug off");
            }
            else
            {
                ModMain.Server.m_PlayersData.m_RecursiveDebug = true;
                Logger.Log("[DebugConsole] RecursiveDebug on");
            }
        }
    }
}
