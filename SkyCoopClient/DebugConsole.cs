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
            uConsole.RegisterCommand("radial_spawn_add", new Action(RadialSpawnAdd));
            uConsole.RegisterCommand("radial_spawn_remove", new Action(RadialSpawnRemove));
            uConsole.RegisterCommand("radial_spawn_save", new Action(RadialSpawnRemove));
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
        public static void RadialSpawnAdd()
        {
            RadialLootSpawnersEditor.CreateSpawner();
            RadialLootSpawnersEditor.Vizualize();
        }
        public static void RadialSpawnRemove()
        {
            RadialLootSpawnersEditor.Remove();
            RadialLootSpawnersEditor.Vizualize();
        }
        public static void RadialSpawnSave()
        {
            RadialLootSpawnersEditor.Save();
        }
    }
}
