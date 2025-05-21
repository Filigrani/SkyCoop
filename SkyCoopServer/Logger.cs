
namespace SkyCoopServer
{
    public class Logger
    {
        public static List<LogData> Logsbuffer = new List<LogData>();
        public struct LogData
        {
            public ConsoleColor m_Color = ConsoleColor.White;
            public string m_Message;
            public LogData(ConsoleColor Color, string Message)
            {
                m_Color = Color;
                m_Message = Message;
            }
            public LogData(string Message)
            {
                m_Message = Message;
            }
        }

        public static void Log(ConsoleColor Color, string Message)
        {
            Logsbuffer.Add(new LogData(Color, Message));
        }
        public static void Log(string Message)
        {
            Logsbuffer.Add(new LogData(Message));
        }
    }
}
