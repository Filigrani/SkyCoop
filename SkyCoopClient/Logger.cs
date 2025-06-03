using MelonLoader;
using SkyCoopClient;

namespace SkyCoop
{
    public class Logger
    {
        public static void Log(System.ConsoleColor Color, string Message)
        {
            MelonLogger.Msg(Color, Message);
            DebugConsole.ConsoleLog(Message);
        }
        public static void Log(string Message)
        {
            MelonLogger.Msg(Message);
            DebugConsole.ConsoleLog(Message);
        }

        public static void Log(object obj)
        {
            MelonLogger.Msg(obj);
            DebugConsole.ConsoleLog(obj.ToString());
        }

        public static void Log(System.ConsoleColor Color, object obj)
        {
            MelonLogger.Msg(Color, obj);
            DebugConsole.ConsoleLog(obj.ToString());
        }

    }
}
