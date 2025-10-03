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
            uConsole.RegisterCommand("sv_cmd", new Action(SV_CMD));
        }

        // Keeping old command, just in case if it still being used by force of habit.
        public static void RecursiveDebug()
        {
            ClientSend.SendSV_CMD("mimic");
        }

        public static void SV_CMD()
        {
            ClientSend.SendSV_CMD(uConsole.GetString());
        }
    }
}
