using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using static SkyCoopServer.DataStr;

namespace SkyCoopServer
{
    public class FilesManager
    {
        public static string s_ExecutingLocation = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";
        // Main Data folder
        public static string s_DataDirectory = $"{s_ExecutingLocation}/_rebornTemp";

        // InDev data
        public static string s_OverrideDataDirectory = $"{s_ExecutingLocation}/RebornData";

        // Save Data folder
        public static string s_SaveDirectory = "";

        // Sub folders
        public static string s_GameModesDirectory = "GameModes";
        public static string s_LootTablesDirectory = "LootTables";
        public static string s_MapsDirectory = "Maps";
        public static string s_WeatherDirectory = "Weather";
        public static string s_DupeListsDirectory = "DupeLists";
        public static string s_GearSpawnsDirectory = "GearSpawns";
        public static string s_WildlifeSpawnsDirectory = "WildlifeSpawns";

        public static string s_ServerConfigLocation = $"{s_ExecutingLocation}/Server.json";

        // PreLoaded Assets
        public static List<string> s_Dupes = new List<string>();
        public static Dictionary<string, SceneAnimalSpawns> s_AnimalSpawns = new Dictionary<string, SceneAnimalSpawns>();
        public static Dictionary<string, Dictionary<string, SpawnRegionData>> s_SpawnRegionBySceneByGUID = new Dictionary<string, Dictionary<string, SpawnRegionData>>();

        public static void CopyOverrideData()
        {
            if (!Directory.Exists(s_OverrideDataDirectory))
            {
                return;
            }

            foreach (string filePath in Directory.EnumerateFiles(s_OverrideDataDirectory, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(s_OverrideDataDirectory, filePath);
                string destPath = Path.Combine(s_DataDirectory, relativePath);
                string destDir = Path.GetDirectoryName(destPath);
                if (!string.IsNullOrEmpty(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                File.Copy(filePath, destPath, true);
            }
        }

        public static void ExtractAllArchives()
        {
            foreach (string FilePath in Directory.GetFiles(s_ExecutingLocation))
            {
                string FileName = Path.GetFileName(FilePath);
                if(!string.IsNullOrEmpty(FileName))
                {
                    string LowerName = FileName.ToLower();
                    if (LowerName.EndsWith(".reborndata"))
                    {
                        try
                        {
                            ZipFile.ExtractToDirectory(FilePath, s_DataDirectory, overwriteFiles: true);
                            Logger.Log($"[FilesManager] Unpacked {FilePath}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"[FilesManager] Unpacking failed! {FilePath}");
                        }
                    }
                }
            }
        }

        public static void DeleteTempFolder()
        {
            if (Directory.Exists(s_DataDirectory))
            {
                Directory.Delete(s_DataDirectory, true);
            }
        }

        public static void CreateTempFolder()
        {
            if (!Directory.Exists(s_DataDirectory))
            {
                Directory.CreateDirectory(s_DataDirectory);
            }
            if (!Directory.Exists($"{s_DataDirectory}/{s_GameModesDirectory}"))
            {
                Directory.CreateDirectory($"{s_DataDirectory}/{s_GameModesDirectory}");
            }
            if (!Directory.Exists($"{s_DataDirectory}/{s_MapsDirectory}"))
            {
                Directory.CreateDirectory($"{s_DataDirectory}/{s_MapsDirectory}");
            }
            if (!Directory.Exists($"{s_DataDirectory}/{s_LootTablesDirectory}"))
            {
                Directory.CreateDirectory($"{s_DataDirectory}/{s_LootTablesDirectory}");
            }
            if (!Directory.Exists($"{s_DataDirectory}/{s_DupeListsDirectory}"))
            {
                Directory.CreateDirectory($"{s_DataDirectory}/{s_DupeListsDirectory}");
            }
            if (!Directory.Exists($"{s_DataDirectory}/{s_WildlifeSpawnsDirectory}"))
            {
                Directory.CreateDirectory($"{s_DataDirectory}/{s_WildlifeSpawnsDirectory}");
            }
            if (!Directory.Exists($"{s_DataDirectory}/{s_GearSpawnsDirectory}"))
            {
                Directory.CreateDirectory($"{s_DataDirectory}/{s_GearSpawnsDirectory}");
            }
        }

        public static void PreloadAssets()
        {
            s_Dupes.Clear();
            s_AnimalSpawns.Clear();
            s_SpawnRegionBySceneByGUID.Clear();

            s_Dupes = GetAllDupes();
            foreach (ScenesAnimalSpawns AnimalsSpawnsData in GetAllAnimalsSpawnsData())
            {
                if (AnimalsSpawnsData != null)
                {
                    foreach (SceneAnimalSpawns SpawnsData in AnimalsSpawnsData.Scenes)
                    {
                        if (SpawnsData != null)
                        {
                            SceneAnimalSpawns Combined = null;
                            if (s_AnimalSpawns.TryGetValue(SpawnsData.SceneName, out Combined))
                            {
                                Combined.Join(SpawnsData);
                            }
                            else
                            {
                                SceneAnimalSpawns New = new SceneAnimalSpawns();
                                New.Spawns = new List<SpawnRegionData>();
                                New.RandomSpawnObjects = new List<RandomAnimalSpawnObjectData>();
                                New.Clone(SpawnsData);
                                s_AnimalSpawns.Add(SpawnsData.SceneName, New);
                            }
                        }
                    }
                }
            }
            // Оптимизация для будущиего поиска для респавнов предметов
            foreach (var item in s_AnimalSpawns)
            {
                string SceneName = item.Key;
                SceneAnimalSpawns Data = item.Value;

                Dictionary<string, SpawnRegionData> DictSpawnsByGUID = null;
                if (!s_SpawnRegionBySceneByGUID.TryGetValue(SceneName, out DictSpawnsByGUID))
                {
                    DictSpawnsByGUID = new Dictionary<string, SpawnRegionData>();
                    s_SpawnRegionBySceneByGUID.Add(SceneName, DictSpawnsByGUID);
                }
                foreach (SpawnRegionData Spawner in Data.Spawns)
                {
                    if (!string.IsNullOrEmpty(Spawner.GUID) && !DictSpawnsByGUID.ContainsKey(Spawner.GUID))
                    {
                        DictSpawnsByGUID.Add(Spawner.GUID, Spawner);
                    }
                }
                foreach (RandomAnimalSpawnObjectData Base in Data.RandomSpawnObjects)
                {
                    foreach (AnimalSpawnElementData Element in Base.SpawnRegions)
                    {
                        SpawnRegionData Spawner = Element.Spawn;
                        if (!string.IsNullOrEmpty(Spawner.GUID) && !DictSpawnsByGUID.ContainsKey(Spawner.GUID))
                        {
                            DictSpawnsByGUID.Add(Spawner.GUID, Spawner);
                        }
                    }
                }
            }
        }

        public static void InitFolders(bool SkipDefaultSaveFolder = false)
        {
            DeleteTempFolder();
            CreateTempFolder();
            ExtractAllArchives();
            CopyOverrideData();

            if (!SkipDefaultSaveFolder)
            {
                if (string.IsNullOrEmpty(s_SaveDirectory))
                {
                    s_SaveDirectory = $"{s_DataDirectory}/ServerSaves";
                }

                if (!Directory.Exists(s_SaveDirectory))
                {
                    Directory.CreateDirectory(s_SaveDirectory);
                }
            }
            PreloadAssets();
        }

        public static void SetSavesFolder(string Path)
        {
            s_SaveDirectory = $"{Path}/ServerSaves";
            Logger.Log($"[FilesManager] Saves folder set to {s_SaveDirectory}");
            if (!Directory.Exists(s_SaveDirectory))
            {
                Directory.CreateDirectory(s_SaveDirectory);
            }
        }

        public static List<DataStr.MinimalPlayersAndGameMode> GetGameModesList()
        {
            List <DataStr.MinimalPlayersAndGameMode> GameModes = new List<MinimalPlayersAndGameMode>();
            string _Path = $"{s_DataDirectory}/{s_GameModesDirectory}";

            if (Directory.Exists(_Path))
            {
                foreach (string FilePath in Directory.GetFiles(_Path))
                {
                    string FileName = Path.GetFileName(FilePath);
                    DataStr.GameRules Rule = GetRules(FileName);
                    DataStr.MinimalPlayersAndGameMode GameMode = new MinimalPlayersAndGameMode();

                    GameMode.GameModeName = Rule.m_HUDMode;
                    GameMode.MinimalPlayers = Rule.m_MinimalPlayersToPlay;

                    GameModes.Add(GameMode);
                }
            }
            return GameModes;
        }

        public static List<GameRules> GetGameRulesList()
        {
            List<GameRules> Rules = new List<GameRules>();
            string _Path = $"{s_DataDirectory}/{s_GameModesDirectory}";

            if (Directory.Exists(_Path))
            {
                foreach (string FilePath in Directory.GetFiles(_Path))
                {
                    string FileName = Path.GetFileName(FilePath);
                    Rules.Add(GetRules(FileName));
                }
            }
            return Rules;
        }

        public static GameRules GetRules(string GameMode)
        {
            string Path = $"{s_DataDirectory}/{s_GameModesDirectory}/{GameMode}";
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {Path}");
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return new GameRules();
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {Path} not exist");
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {Path} is empty");
                return new GameRules();
            }
            GameRulesJson JsonData = JsonSerializer.Deserialize<GameRulesJson>(JSON);
            return JsonData == null ? new GameRules() : JsonData.Load(GameMode);
        }


        public static List<string> GetMapsList()
        {
            string path = $"{s_DataDirectory}/{s_MapsDirectory}";

            if (Directory.Exists(path))
            {
                List<string> FileNames = new List<string>(Directory.GetFiles(path));
                for (int i = 0; i < FileNames.Count; i++)
                {
                    FileNames[i] = Path.GetFileName(FileNames[i]);
                }
                return FileNames;
            }
            return new List<string>();
        }

        public static MapData GetMapData(string MapName)
        {
            string Path = $"{s_DataDirectory}/{s_MapsDirectory}/{MapName}";
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {Path}");
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return null;
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {Path} not exist");
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {Path} is empty");
                return null;
            }
            return JsonSerializer.Deserialize<MapData>(JSON);
        }

        public static string GetSceneByMapName(string MapName)
        {
            string Path = $"{s_DataDirectory}/{s_MapsDirectory}/{MapName}";
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {Path}");
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return "";
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {Path} not exist");
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {Path} is empty");
                return "";
            }
            MapData JsonData = JsonSerializer.Deserialize<MapData>(JSON);
            return JsonData == null ? "" : JsonData.Scene;
        }

        public static Dictionary<string, PrefabTableJSON> GetAllLootTables()
        {
            Dictionary<string, PrefabTableJSON> Dict = new Dictionary<string, PrefabTableJSON>();
            string _Path = $"{s_DataDirectory}/{s_LootTablesDirectory}";
            if (Directory.Exists(_Path))
            {
                string[] AllFiles = Directory.GetFiles(_Path, "*", SearchOption.AllDirectories);

                foreach (string FilePath in AllFiles)
                {
                    PrefabTableJSON Table = GetLootTable(FilePath);
                    if (Table != null)
                    {
                        string Name = Path.GetFileNameWithoutExtension(FilePath);
                        if (!Dict.ContainsKey(Name))
                        {
                            Dict.Add(Name, Table);
                        }
                    }
                }
            }
            return Dict;
        }

        public static PrefabTableJSON GetLootTable(string Path)
        {
            string JSON = "";

            if (LootTableManager.c_DebugLogs)
            {
                Logger.Log($"[FilesManager] Loading loot table {Path}");
            }

            
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return null;
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {Path} not exist");
                return null;
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {Path} is empty");
                return null;
            }
            return JsonSerializer.Deserialize<PrefabTableJSON>(JSON);
        }

        public static string GetAirDrop(string DropPath)
        {
            string Path = $"{s_DataDirectory}/{DropPath}";
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {Path}");
            if (File.Exists(Path))
            {
                try
                {
                    return File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return null;
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {Path} not exist");
                return null;
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {Path} is empty");
                return null;
            }
            return null;
        }

        public static WeatherManager.WeatherSettingsConfig GetWeatherConfig(string Profile = "Default")
        {
            if (string.IsNullOrEmpty(Profile))
            {
                Profile = "Default";
            }
            
            string Path = $"{s_DataDirectory}/{s_WeatherDirectory}/{Profile}";
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {Path}");
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return null;
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {Path} not exist");
                return null;
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {Path} is empty");
                return null;
            }
            return JsonSerializer.Deserialize<WeatherManager.WeatherSettingsConfig>(JSON);
        }

        public static Dictionary<string , DataStr.ScenesLootSpawns> GetAllGearSpawns()
        {
            Dictionary<string, DataStr.ScenesLootSpawns> Result = new Dictionary<string, DataStr.ScenesLootSpawns>();

            string _Path = $"{s_DataDirectory}/{s_GearSpawnsDirectory}";

            if (Directory.Exists(_Path))
            {
                foreach (string FilePath in Directory.GetFiles(_Path))
                {
                    DataStr.ScenesLootSpawns Data = GetGearsSpawnsData(FilePath);
                    if (Data != null)
                    {
                        Result.Add(Path.GetFileNameWithoutExtension(FilePath), Data);
                    }
                }
            }
            return Result;
        }

        public static DataStr.ScenesLootSpawns GetGearsSpawnsData(string Path)
        {

            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {Path}");
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return null;
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {Path} not exist");
                return null;
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {Path} is empty");
                return null;
            }
            return JsonSerializer.Deserialize<ScenesLootSpawns>(JSON, options);
        }

        public static List<DataStr.ScenesAnimalSpawns> GetAllAnimalsSpawnsData()
        {
            List<DataStr.ScenesAnimalSpawns> Result = new List<ScenesAnimalSpawns>();

            string _Path = $"{s_DataDirectory}/{s_WildlifeSpawnsDirectory}";

            if (Directory.Exists(_Path))
            {
                foreach (string FilePath in Directory.GetFiles(_Path))
                {
                    DataStr.ScenesAnimalSpawns AnimalSpawns = GetAnimalsSpawnsData(FilePath);
                    if (AnimalSpawns != null)
                    {
                        Result.Add(AnimalSpawns);
                    }
                }
            }
            return Result;
        }

        public static DataStr.ScenesAnimalSpawns GetAnimalsSpawnsData(string Path)
        {

            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {Path}");
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return null;
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {Path} not exist");
                return null;
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {Path} is empty");
                return null;
            }
            return JsonSerializer.Deserialize<ScenesAnimalSpawns>(JSON, options);
        }

        public static void SaveSceneToFile(SceneData.SaveData SaveData, Server ServerInstance)
        {
            JsonSerializerOptions Options = new JsonSerializerOptions();
            Options.WriteIndented = true;
            string JSON = JsonSerializer.Serialize<SceneData.SaveData>(SaveData, Options);
            try
            {
                string SeedName = ServerInstance.m_Config.m_Seed.ToString();

                if (!Directory.Exists($"{s_SaveDirectory}/{SeedName}"))
                {
                    Directory.CreateDirectory($"{s_SaveDirectory}/{SeedName}");
                }
                if (!Directory.Exists($"{s_SaveDirectory}/{SeedName}/Scenes"))
                {
                    Directory.CreateDirectory($"{s_SaveDirectory}/{SeedName}/Scenes");
                }
                
                File.WriteAllText($"{s_SaveDirectory}/{SeedName}/Scenes/{SaveData.SceneName}", JSON);
                return;
            }
            catch (Exception e)
            {
                Logger.Log($"[FilesManager] Failed to save scene {SaveData.SceneName}: {e.Message}");
            }
        }

        public static SceneData.SaveData LoadSceneFromFile(string SceneName, Server ServerInstance)
        {
            string Path = $"{s_SaveDirectory}/{ServerInstance.m_Config.m_Seed.ToString()}/Scenes/{SceneName}";
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {Path}");
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return null;
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {Path} not exist");
                return null;
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {Path} is empty");
                return null;
            }
            return JsonSerializer.Deserialize<SceneData.SaveData>(JSON);
        }

        public static void SaveServerToFile(Server.SaveData SaveData, Server ServerInstance)
        {
            JsonSerializerOptions Options = new JsonSerializerOptions();
            Options.WriteIndented = true;
            string JSON = JsonSerializer.Serialize<Server.SaveData>(SaveData, Options);
            try
            {
                string SeedName = ServerInstance.m_Config.m_Seed.ToString();

                if (!Directory.Exists($"{s_SaveDirectory}/{SeedName}"))
                {
                    Directory.CreateDirectory($"{s_SaveDirectory}/{SeedName}");
                }

                File.WriteAllText($"{s_SaveDirectory}/{SeedName}/ServerData", JSON);
                return;
            }
            catch (Exception e)
            {
                Logger.Log($"[FilesManager] Failed to save server data: {e.Message}");
            }
        }

        public static Server.SaveData LoadServerFromFile(int Seed)
        {
            string Path = $"{s_SaveDirectory}/{Seed.ToString()}/ServerData";
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {Path}");
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return null;
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {Path} not exist");
                return null;
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {Path} is empty");
                return null;
            }
            return JsonSerializer.Deserialize<Server.SaveData>(JSON);
        }

        public class SaveDataAndSeed
        {
            public int Seed;
            public Server.SaveData Save;

            public SaveDataAndSeed(int seed, Server.SaveData save)
            {
                Seed = seed;
                Save = save;
            }
        }

        public static List<SaveDataAndSeed> GetServerSavesList()
        {
            List<SaveDataAndSeed> Saves = new List<SaveDataAndSeed>();
            if (Directory.Exists(s_SaveDirectory))
            {
                List<string> FilesPaths = new List<string>(Directory.GetDirectories(s_SaveDirectory));
                for (int i = 0; i < FilesPaths.Count; i++)
                {
                    string FileName = Path.GetFileName(FilesPaths[i]);
                    int Seed = int.Parse(FileName);
                    Server.SaveData ServerSaveData = LoadServerFromFile(Seed);
                    if(ServerSaveData != null)
                    {
                        Saves.Add(new SaveDataAndSeed(Seed, ServerSaveData));
                    }
                }
            }
            return Saves;
        }

        public static void DeleteSave(string SaveName)
        {
            Logger.Log($"[FilesManager] Delete save {$"{s_SaveDirectory}/{SaveName}"}");
            if (Directory.Exists($"{s_SaveDirectory}/{SaveName}"))
            {
                Directory.Delete($"{s_SaveDirectory}/{SaveName}", true);
            }
        }

        public static DataStr.ServerConfig LoadServerCFG()
        {
            string Path = s_ServerConfigLocation;
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {Path}");
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return null;
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {Path} not exist");
                return null;
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {Path} is empty");
                return null;
            }

            ServerConfigJSON Proxy = JsonSerializer.Deserialize<DataStr.ServerConfigJSON>(JSON);

            return Proxy.Load();
        }

        public static List<string> GetAllDupes()
        {
            List<string> Gears = new List<string>();
            string _Path = $"{s_DataDirectory}/{s_DupeListsDirectory}";
            if (Directory.Exists(_Path))
            {
                foreach (string FilePath in Directory.GetFiles(_Path))
                {
                    List<string> DupeList = GetDupeList(FilePath);
                    if (DupeList != null)
                    {
                        foreach (string Gear in DupeList)
                        {
                            if (!Gears.Contains(Gear))
                            {
                                Gears.Add(Gear);
                            }
                        }
                    }
                }
            }
            return Gears;
        }

        public static List<string> GetDupeList(string Path)
        {
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {Path}");
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return null;
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {Path} not exist");
                return null;
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {Path} is empty");
                return null;
            }
            return JsonSerializer.Deserialize<List<string>>(JSON);
        }
    }
}
