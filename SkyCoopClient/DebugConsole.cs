using Il2Cpp;
using SkyCoop;
using UnityEngine;

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
            uConsole.RegisterCommand("recursiveDebug", new Action(RecursiveDebug));
            uConsole.RegisterCommand("mimic", new Action(RecursiveDebug));
            uConsole.RegisterCommand("sv_cmd", new Action(SV_CMD));
            uConsole.RegisterCommand("spawn", new Action(Spawn));
            uConsole.RegisterCommand("give", new Action(GiveIlegalGear));
        }

        public static void GiveIlegalGear()
        {
            GameObject reference = AssetManager.GetAssetFromGame<GameObject>(uConsole.GetString());
            if (reference)
            {
                GameObject GearObject = UnityEngine.Object.Instantiate(reference);
                GearItem item = GearObject.GetComponent<GearItem>();
                if (item != null)
                {
                    item.CompleteSpawnFromCONSOLE();
                    GameManager.GetInventoryComponent().AddGear(item);
                }
            }
        }

        public static void Spawn()
        {
            GameObject reference = AssetManager.GetAssetFromGame<GameObject>(uConsole.GetString());
            if (reference)
            {
                Vector3 position = GameManager.GetPlayerTransform().position;
                Quaternion rotation = GameManager.GetPlayerTransform().rotation;
                GameObject Object = UnityEngine.Object.Instantiate(reference, position, rotation);
            }
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
