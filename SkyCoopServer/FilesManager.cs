using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using static SkyCoopServer.DataStr;
using System.IO;

namespace SkyCoopServer
{
    public class FilesManager
    {
        // Main Data folder
        public static string s_DataDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/SkyModData";

        // Sub folders
        public static string s_GameModesDirectory = "GameModes";
        public static string s_LootTablesDirectory = "LootTables";
        public static string s_MapsDirectory = "Maps";

        public static void InitFolders()
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
            return JsonData == null ? new GameRules() : JsonData.Load();
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
    }
}
