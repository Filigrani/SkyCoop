using Il2Cpp;
using Il2CppSystem;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace SkyCoopClient
{
    public static class SaveSystemHook
    {
        public static Dictionary<string, float> m_BooksProgress = new Dictionary<string, float>();
        public static Dictionary<string, bool> m_CollectedDupes = new Dictionary<string, bool>();

        public static Il2CppSystem.Collections.Generic.List<string> Save_Dupes()
        {
            Il2CppSystem.Collections.Generic.List<string> List = new Il2CppSystem.Collections.Generic.List<string>();
            foreach (var item in m_CollectedDupes)
            {
                List.Add(item.Key);
            }
            foreach (string GUID in List)
            {
                SkyCoop.Logger.Log(GUID);
            }

            return List;
        }

        public static void Load_Dupes(Il2CppSystem.Collections.Generic.List<string> List)
        {
            m_CollectedDupes.Clear();
            if (List != null)
            {
                foreach (string GUID in List)
                {
                    m_CollectedDupes.Add(GUID, true);
                }
            }
        }

        public static Il2CppSystem.Collections.Generic.Dictionary<string, float> Save_BooksProgress()
        {
            Il2CppSystem.Collections.Generic.Dictionary<string, float> Dict = new Il2CppSystem.Collections.Generic.Dictionary<string, float>();
            foreach (var item in m_BooksProgress)
            {
                Dict.Add(item.Key, item.Value);
            }

            return Dict;
        }

        public static void Load_Books(Il2CppSystem.Collections.Generic.Dictionary<string, float> Dict)
        {
            m_BooksProgress.Clear();
            if (Dict != null)
            {
                foreach (var item in Dict)
                {
                    m_BooksProgress.Add(item.Key, item.Value);
                }
            }
        }

        public static bool IsDupeAlreadyCollected(string GUID)
        {
            return m_CollectedDupes.ContainsKey(GUID);
        }

        public static float GetBooksProgress(string GUID)
        {
            float Progress = 0;

            if(m_BooksProgress.TryGetValue(GUID, out Progress))
            {
                return Progress;
            }

            return Progress;
        }

        public static void AddBookProgress(string GUID, float Progress)
        {
            if (m_BooksProgress.ContainsKey(GUID))
            {
                m_BooksProgress.Remove(GUID);
            }
            m_BooksProgress.Add(GUID, Progress);
        }

        public static void AddCollectedDupe(string GUID)
        {
            if (m_CollectedDupes.ContainsKey(GUID))
            {
                return;
            }
            m_CollectedDupes.Add(GUID, true);
        }

        [HarmonyLib.HarmonyPatch(typeof(SaveGameSystem), "SaveGlobalData")]
        private static class SaveGameSystem_SaveGlobalData
        {
            private static void Postfix(SlotData slot)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                slot.TrySetData("skycoop_dupes", Save_Dupes());
                slot.TrySetData("skycoop_books", Save_BooksProgress());
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SaveGameSystem), "RestoreGlobalData")]
        private static class SaveGameSystem_RestoreGlobalData
        {
            private static void Postfix(string name)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                SlotData slot = SaveGameSlots.GetSaveSlotFromName(name);

                if (slot != null)
                {
                    SkyCoop.Logger.Log("[CustomSaves] Keys save have:");
                    Il2CppSystem.Collections.Generic.List<string> Keys = slot.GetAllDataKeys();
                    foreach (string key in Keys)
                    {
                        SkyCoop.Logger.Log(key);
                    }

                    Il2CppSystem.Collections.Generic.List<string> DupesData = null;
                    if (slot.TryGetData("skycoop_dupes", out DupesData))
                    {
                        Load_Dupes(DupesData);
                    }

                    Il2CppSystem.Collections.Generic.Dictionary<string, float> BooksData = null;
                    if (slot.TryGetData("skycoop_books", out BooksData))
                    {
                        Load_Books(BooksData);
                    }
                }
            }
        }
    }
}
