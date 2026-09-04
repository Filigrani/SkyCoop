using Il2Cpp;
using Il2CppTLD.Gear;
using SkyCoop;
using SkyCoopServer;
using System.Text.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static SkyCoopServer.DataStr;

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
            uConsole.RegisterCommand("campfire", new Action(GiveCampfireKit));
            uConsole.RegisterCommand("liquidboildebug", new Action(LiquiadBoilDebug));
            uConsole.RegisterCommand("removepleasewait", new Action(RemovePleaseWait));
            uConsole.RegisterCommand("rip", new Action(RIP));
            uConsole.RegisterCommand("ripmodded", new Action(RIPModded));
            uConsole.RegisterCommand("ripstop", new Action(RIPSTOP));
            uConsole.RegisterCommand("checksave", new Action(CheckSave));
            uConsole.RegisterCommand("ripnaratives", new Action(RIPNaratives));
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

        public static void GiveCampfireKit()
        {
            uConsole.RunCommandSilent("add tinder");
            uConsole.RunCommandSilent("add firestriker");
            uConsole.RunCommandSilent("add tinder 3");
            uConsole.RunCommandSilent("add softwood");
            uConsole.RunCommandSilent("add softwood");
            uConsole.RunCommandSilent("add softwood");
            uConsole.RunCommandSilent("add accelerant");
        }

        public static void LiquiadBoilDebug()
        {
            GearsSync.s_LiquidCookingDebug = !GearsSync.s_LiquidCookingDebug;
        }

        public static void RIP()
        {
            if(!GearSpawnsRipper.s_Active && !ModMain.IsMultiplayer())
            {
                GearSpawnsRipper.Start();
            }
        }
        public static void RIPModded()
        {
            if (!GearSpawnsRipper.s_Active && !ModMain.IsMultiplayer())
            {
                GearSpawnsRipper.Start(true);
            }
        }
        public static void RIPSTOP()
        {
            if (!GearSpawnsRipper.s_Active && !ModMain.IsMultiplayer())
            {
                GearSpawnsRipper.s_ScenesToSave.Clear();
            }
        }
        public static void RIPNaratives()
        {
            uConsole.RunCommandSilent("add_all_gear");

            List<string> Dupples = new List<string>();

            if (GameManager.m_Inventory)
            {
                foreach (GearItemObject item in GameManager.m_Inventory.m_Items)
                {
                    if (item.m_GearItem && item.m_GearItem.m_NarrativeCollectibleItem)
                    {
                        if (!Dupples.Contains(item.m_GearItemName))
                        {
                            Dupples.Add(item.m_GearItemName);
                        }
                    }
                }
            }
            JsonSerializerOptions Options = new JsonSerializerOptions();
            Options.WriteIndented = true;
            string JSON = JsonSerializer.Serialize<List<string>>(Dupples, Options);

            if (!Directory.Exists($"{FilesManager.s_DataDirectory}/DupeLists"))
            {
                Directory.CreateDirectory($"{FilesManager.s_DataDirectory}/DupeLists");
            }

            try
            {
                File.WriteAllText($"{FilesManager.s_DataDirectory}/DupeLists/Naratives", JSON);
                return;
            }
            catch (Exception e)
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"Cant save file: {e.Message}");
            }
        }

        public static void CheckSave()
        {
            int Seed = uConsole.GetInt();
            MenuHook.FindSaveForSeed(Seed);
        }

        public static void RemovePleaseWait()
        {
            MenuHook.RemovePleaseWait();
        }


        [HarmonyLib.HarmonyPatch(typeof(uConsole), "Update")]
        private static class uConsole_Update
        {
            private static void Prefix(uConsole __instance)
            {
                if (ModMain.Client != null && ModMain.Client.m_IsReady && !ModMain.Client.m_Config.m_CheatsAllowed)
                {
                    uConsole.m_On = false;
                }
            }
        }
    }
}
