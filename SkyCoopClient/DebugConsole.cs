using Il2Cpp;
using SkyCoop;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SkyCoopClient
{
    public class DebugConsole
    {
        public static void ConsoleLog(string message)
        {
            //uConsoleLog.Add($"[SkyCoop] {message}");
        }

        public static void ReimplementConsole()
        {
            if (uConsole.m_Instance == null)
            {
                SkyCoop.Logger.Log("uConsole has been reimplemented!");
                GameObject ConsoleReference = Addressables.LoadAssetAsync<GameObject>("uConsole").WaitForCompletion();
                if (ConsoleReference != null)
                {
                    GameObject ConsoleObj = UnityEngine.Object.Instantiate(ConsoleReference);
                    if (ConsoleObj)
                    {
                        uConsole.m_Instance = ConsoleObj.GetComponent<uConsole>();
                    }
                    else
                    {
                        SkyCoop.Logger.Log(System.ConsoleColor.Red, "Can't assign uConsole!");
                    }
                }
                else
                {
                    SkyCoop.Logger.Log(System.ConsoleColor.Red, "Can't load uConsole!");
                }
            }
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


        [HarmonyLib.HarmonyPatch(typeof(uConsole), "Update")]
        private static class uConsole_Update
        {
            private static void Prefix(uConsole __instance)
            {
                if (ModMain.Client.m_IsReady && !ModMain.Client.m_Config.m_CheatsAllowed)
                {
                    uConsole.m_On = false;
                }
            }
        }
    }
}
