using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using MelonLoader.TinyJSON;

namespace SkyCoop
{
    public class MPSaveManager
    {
        public static void Log(string LOG)
        {
            MelonLoader.MelonLogger.Msg(ConsoleColor.Blue,"[MPSaveManager] "+LOG);
        }
        public static void Error(string LOG)
        {
            MelonLoader.MelonLogger.Msg(ConsoleColor.Red, "[MPSaveManager] " + LOG);
        }
        public static Dictionary<string, Dictionary<int, MyMod.DroppedGearItemDataPacket>> RecentVisual = new Dictionary<string, Dictionary<int, MyMod.DroppedGearItemDataPacket>>();
        public static Dictionary<string, Dictionary<int, MyMod.SlicedJsonDroppedGear>> RecentData = new Dictionary<string, Dictionary<int, MyMod.SlicedJsonDroppedGear>>();
        public static Dictionary<string, Dictionary<string, bool>> RecentOpenableThings = new Dictionary<string, Dictionary<string, bool>>();

        public static void SaveRecentStuff()
        {
            int SaveSeed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            ValidateRootExits();
            foreach (var item in RecentVisual)
            {
                SaveData(GetKeyTemplate(SaveKeyTemplateType.DropsVisual, item.Key), JSON.Dump(item.Value), SaveSeed);
            }
            foreach (var item in RecentData)
            {
                SaveData(GetKeyTemplate(SaveKeyTemplateType.DropsData, item.Key), JSON.Dump(item.Value), SaveSeed);
            }
            foreach (var item in RecentOpenableThings)
            {
                SaveData(GetKeyTemplate(SaveKeyTemplateType.Openables, item.Key), JSON.Dump(item.Value), SaveSeed);
            }
            RecentVisual = new Dictionary<string, Dictionary<int, MyMod.DroppedGearItemDataPacket>>();
            RecentData = new Dictionary<string, Dictionary<int, MyMod.SlicedJsonDroppedGear>>();
            RecentOpenableThings = new Dictionary<string, Dictionary<string, bool>>();
        }

        public static string LoadData(string name, int Seed = 0)
        {
            Log("Attempt to load " + name);
            string fullPath = GetPathForName(name, Seed);
            if (!File.Exists(fullPath))
            {
                Log("File " + fullPath+" not exist");
                return "";
            }else{
                byte[] FileData = File.ReadAllBytes(fullPath);
                string Result = UTF8Encoding.UTF8.GetString(FileData);
                Log("Loaded with no problems");

                return Result;
            }
        }

        public static string GetPathForName(string name, int Seed = 0)
        {
            if(Seed != 0)
                return PersistentDataPath.m_Path + PersistentDataPath.m_PathSeparator + Seed + PersistentDataPath.m_PathSeparator + name;
            
            
            return PersistentDataPath.m_Path + PersistentDataPath.m_PathSeparator + name;
        }

        public static bool SaveData(string name, string content, int Seed = 0)
        {
            Log("Attempt to save " + name);
            string pathAndFilename = GetPathForName(name, Seed);
            string tempFile = pathAndFilename + "_temp";
            if (File.Exists(tempFile))
                File.Delete(tempFile);
            Stream stream = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            if (stream == null)
            {
                Error("Save failed, writing stream wasn't created");
                return false;
            }
            byte[] data = new UTF8Encoding(true).GetBytes(content);
            stream.Write(data, 0, data.Length);
            stream.Dispose();
            if (File.Exists(pathAndFilename))
                File.Delete(pathAndFilename);
            File.Copy(tempFile, pathAndFilename, true);
            if (File.Exists(tempFile))
                File.Delete(tempFile);
            Log("Everything alright! File saved!");

            return true;
        }
        public static void DeleteData(string name, int Seed = 0)
        {
            Log("Attempt to delete " + name);
            try
            {
                string fullPath = GetPathForName(name, Seed);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
                Log("File deleted!");
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
            }
        }

        public enum SaveKeyTemplateType
        {
            Container = 0,
            DropsVisual = 1,
            DropsData = 2,
            Openables = 3,
        }

        public static string GetKeyTemplate(SaveKeyTemplateType T, string Scene, string GUID = "")
        {            
            switch (T)
            {
                case SaveKeyTemplateType.Container:
                    return Scene + "_"+ GUID;
                case SaveKeyTemplateType.DropsVisual:
                    return Scene+ "_DropVisual";
                case SaveKeyTemplateType.Openables:
                    return Scene + "_Open";
                case SaveKeyTemplateType.DropsData:
                    return Scene + "_DropsData";
                default:
                    return "_UNKNOWN";
            }
        }

        public static void CreateFolderIfNotExist(string path)
        {
            bool exists = Directory.Exists(path);
            if (!exists)
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void ValidateRootExits()
        {
            int SaveSeed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            CreateFolderIfNotExist(GetPathForName(SaveSeed + ""));
        }

        public static void SaveContainer(string scene, string GUID, string Content)
        {
            int SaveSeed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            string Key = GetKeyTemplate(SaveKeyTemplateType.Container, scene, GUID);
            ValidateRootExits();
            SaveData(Key, Content, SaveSeed);
        }
        public static string LoadContainer(string scene, string GUID)
        {
            int SaveSeed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            string Key = GetKeyTemplate(SaveKeyTemplateType.Container, scene, GUID);
            ValidateRootExits();
            return LoadData(Key, SaveSeed);
        }
        public static void RemoveContainer(string scene, string GUID)
        {
            Log("Got request to remove "+ GUID);
            int SaveSeed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            string Key = GetKeyTemplate(SaveKeyTemplateType.Container, scene, GUID);
            DeleteData(Key, SaveSeed);
        }
        public static Dictionary<string, bool> LoadOpenableThings(string scene)
        {
            int SaveSeed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            string Key = GetKeyTemplate(SaveKeyTemplateType.Openables, scene);

            Dictionary<string, bool> Dict;
            if (RecentOpenableThings.TryGetValue(scene, out Dict))
                return Dict;
            

            string LoadedContent = LoadData(Key, SaveSeed);
            if (LoadedContent != "")
                return JSON.Load(LoadedContent).Make< Dictionary<string, bool>>();
    
            return null;
        }
        public static void ChangeOpenableThingState(string scene, string GUID, bool state)
        {
            int SaveSeed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            string Key = GetKeyTemplate(SaveKeyTemplateType.Openables, scene);
            Dictionary<string, bool> Dict;
            if (RecentOpenableThings.TryGetValue(scene, out Dict))
            {
                if (Dict.ContainsKey(GUID))
                    Dict.Remove(GUID);

                Dict.Add(GUID, state);
                RecentOpenableThings.Remove(scene);
                RecentOpenableThings.Add(scene, Dict);
                return;

            }else{
                Dict = LoadOpenableThings(scene);
            }

            if (Dict == null)
            {
                Dict = new Dictionary<string, bool>();
            }
            Dict.Remove(GUID);
            Dict.Add(GUID, state);
            RecentOpenableThings.Add(scene, Dict);
            ValidateRootExits();
            SaveData(Key, JSON.Dump(Dict), SaveSeed);
        }


        public static Dictionary<int, MyMod.DroppedGearItemDataPacket> LoadDropVisual(string scene)
        {
            int SaveSeed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            string Key = GetKeyTemplate(SaveKeyTemplateType.DropsVisual, scene);

            Dictionary<int, MyMod.DroppedGearItemDataPacket> Dict;
            if(RecentVisual.TryGetValue(scene, out Dict))
                return Dict;

            string LoadedContent = LoadData(Key, SaveSeed);
            if (LoadedContent != "")
                return JSON.Load(LoadedContent).Make<Dictionary<int, MyMod.DroppedGearItemDataPacket>>();

            return null;
        }
        public static Dictionary<int, MyMod.SlicedJsonDroppedGear> LoadDropData(string scene)
        {
            int SaveSeed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            string Key = GetKeyTemplate(SaveKeyTemplateType.DropsData, scene);

            Dictionary<int, MyMod.SlicedJsonDroppedGear> Dict;
            if(RecentData.TryGetValue(scene, out Dict))
                return Dict;

            string LoadedContent = LoadData(Key, SaveSeed);
            if (LoadedContent != "")
                return JSON.Load(LoadedContent).Make<Dictionary<int, MyMod.SlicedJsonDroppedGear>>();

            return null;
        }
        public static void AddGearVisual(string scene, MyMod.DroppedGearItemDataPacket gear)
        {
            int SaveSeed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            string Key = GetKeyTemplate(SaveKeyTemplateType.DropsVisual, scene);
            Dictionary<int, MyMod.DroppedGearItemDataPacket> Dict;
            if (RecentVisual.TryGetValue(scene, out Dict))
            {
                Dict.Remove(gear.m_Hash);
                Dict.Add(gear.m_Hash, gear);
                RecentVisual.Remove(scene);
                RecentVisual.Add(scene, Dict);
                return;
            }else{
                Dict = LoadDropVisual(scene);
            }

            if (Dict == null)
            {
                Dict = new Dictionary<int, MyMod.DroppedGearItemDataPacket>();
            }
            Dict.Remove(gear.m_Hash);
            Dict.Add(gear.m_Hash, gear);
            ValidateRootExits();
            SaveData(Key, JSON.Dump(Dict), SaveSeed);
            RecentVisual.Add(scene, Dict);
        }
        public static void AddGearData(string scene, int hash, MyMod.SlicedJsonDroppedGear GearData)
        {
            int SaveSeed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            string Key = GetKeyTemplate(SaveKeyTemplateType.DropsData, scene);
            Dictionary<int, MyMod.SlicedJsonDroppedGear> Dict;

            if (RecentData.TryGetValue(scene, out Dict))
            {
                Dict.Remove(hash);
                Dict.Add(hash, GearData);
                RecentData.Remove(scene);
                RecentData.Add(scene, Dict);
                return;
            }else{
                Dict = LoadDropData(scene);
            }

            if (Dict == null)
            {
                Dict = new Dictionary<int, MyMod.SlicedJsonDroppedGear>();
            }
            Dict.Remove(hash);
            Dict.Add(hash, GearData);
            ValidateRootExits();
            SaveData(Key, JSON.Dump(Dict), SaveSeed);
            RecentData.Add(scene, Dict);
        }

        public static void RemovSpecificGear(int Hash, string Scene)
        {
            Log("[RemovSpecificGear] Trying to remove " + Hash);
            Dictionary<int, MyMod.SlicedJsonDroppedGear> Dict;
            if (!RecentData.TryGetValue(Scene, out Dict))
            {
                Dict = LoadDropData(Scene);
            }

            if(Dict != null)
            {
                Dict.Remove(Hash);
                RecentData.Remove(Scene);
                RecentData.Add(Scene, Dict);
            }

            Dictionary<int, MyMod.DroppedGearItemDataPacket> Dict2;
            if (!RecentVisual.TryGetValue(Scene, out Dict2))
            {
                Dict2 = LoadDropVisual(Scene);
            }

            if(Dict2 != null)
            {
                Dict2.Remove(Hash);
                RecentVisual.Remove(Scene);
                RecentVisual.Add(Scene, Dict2);
            }
        }

        public static MyMod.SlicedJsonDroppedGear RequestSpecificGear(int Hash, string Scene, bool Remove = true)
        {
            Dictionary<int, MyMod.SlicedJsonDroppedGear> Dict = LoadDropData(Scene);
            MyMod.SlicedJsonDroppedGear Gear = null;
            if (Dict != null)
            {
                if(Dict.TryGetValue(Hash, out Gear))
                {
                    if (Remove)
                    {
                        RemovSpecificGear(Hash, Scene);
                    }
                }
            }
            return Gear;
        }

        public static bool SaveServerCFG(MyMod.ServerSettingsData CFG)
        {
            return SaveData("ServerSettingsData", JSON.Dump(CFG));
        }
        public static MyMod.ServerSettingsData RequestServerCFG()
        {
            string Data = LoadData("ServerSettingsData");
            if (Data != "")
            {
                return JSON.Load(Data).Make<MyMod.ServerSettingsData>();
            }else{
                return null;
            }
        }
    }
}
