using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using static SkyCoopServer.DataStr;

namespace SkyCoopServer
{
    public class FilesManager
    {
        public static string s_SpawnPointsDirectory = "SpawnPoints";
        public static string s_ZoneConfigDirectory = "ZoneConfig";
        public static string s_StartingGearFileName = "StartingGear";
        public static string s_VictoryPlaceDirectory = "Victory";
        public static string s_PropsDirectory = "Props";
        public static string s_RulesFileName = "Rules";
        public static string s_LootTablesDirectory = "LootTables";
        public static string s_RadialLootSpawnersDirectory = "RadialLootSpawners";
        public static string s_DataDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/SkyModData";

        public static GameRules GetRules(string GameMode)
        {
            GameRules Rules = new GameRules();
            string Path = $"{s_DataDirectory}/{GameMode}/{s_RulesFileName}";
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {GameMode}/{s_RulesFileName}");
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return Rules;
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {GameMode}/{s_RulesFileName} not exist");
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {GameMode}/{s_RulesFileName} is empty");
                return Rules;
            }
            GameRulesSave Save = JsonSerializer.Deserialize<GameRulesSave>(JSON);
            Rules.m_PlayerCanBeKnocked = Save.Knockdowns;
            Rules.m_PVP = Save.PVP;
            Rules.m_StartingItems = Save.StartingGear;
            Rules.m_StartingItemsByTier = Save.StartingGearByTier;
            // TODO: ADD PROPER CHECK OF CFG FOR ALL PARAMETERS!!!!
            if(Rules.m_StartingItemsByTier == null)
            {
                Rules.m_StartingItemsByTier = Rules.m_StartingItemsByTier = new List<List<StartingGearData>>();
            }
            Rules.m_Time = Save.Time;
            Rules.m_HUDMode = Save.HUDMode;
            Rules.m_Respawns = Save.Respawns;
            Rules.m_DeathPacks = Save.DeathPacks;
            Rules.m_Clothing = Save.Clothing;
            return Rules;
        }

        public static string GetRandomSceneForGameMode(string GameMode)
        {
            string _Path = $"{s_DataDirectory}/{GameMode}/{s_SpawnPointsDirectory}";
            if (Directory.Exists(_Path))
            {
                string[] Scenes = Directory.GetFiles(_Path);
                if(Scenes.Length == 0)
                {
                    return "";
                }else if(Scenes.Length == 1)
                {
                    return Path.GetFileNameWithoutExtension(Scenes[0]);
                }
                else
                {
                    return Path.GetFileNameWithoutExtension(Scenes[new System.Random(Guid.NewGuid().GetHashCode()).Next(0, Scenes.Length)]);
                }
            }
            return "";
        }

        public static List<V3Quat> GetSpawnPoints(string GameMode, string Scene)
        {
            List<V3Quat> Points = new List<V3Quat>();
            string Path = $"{s_DataDirectory}/{GameMode}/{s_SpawnPointsDirectory}/{Scene}";
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {GameMode}/{s_SpawnPointsDirectory}/{Scene}");
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return Points;
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {GameMode}/{s_SpawnPointsDirectory}/{Scene} not exist");
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {GameMode}/{s_SpawnPointsDirectory}/{Scene} is empty");
                return Points;
            }
            SpawnPointSave Save = JsonSerializer.Deserialize<SpawnPointSave>(JSON);
            for (int i = 0; i < Save.points.Count; i++)
            {
                SpawnPoint Point = Save.points[i];
                Points.Add(new V3Quat(Point.posx, Point.posy, Point.posz, Point.rotx, Point.roty, Point.rotz, Point.rotw));
            }
            return Points;
        }

        public static DangerCircleConfig GetZoneConfig(string GameMode, string Scene)
        {
            string Path = $"{s_DataDirectory}/{GameMode}/{s_ZoneConfigDirectory}/{Scene}";
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {GameMode}/{s_ZoneConfigDirectory}/{Scene}");
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
                Logger.Log($"[FilesManager] File {GameMode}/{s_ZoneConfigDirectory}/{Scene} not exist");
                return null;
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {GameMode}/{s_ZoneConfigDirectory}/{Scene} is empty");
                return null;
            }
            DangerCircleConfig CFG = JsonSerializer.Deserialize<DangerCircleConfig>(JSON);
            return CFG;
        }

        public static Vector3 GetVictoryPosition(string GameMode, string SceneName)
        {
            Vector3 Position = new Vector3(0,0,0);
            string Path = $"{s_DataDirectory}/{GameMode}/{s_VictoryPlaceDirectory}/{SceneName}";
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {GameMode}/{s_VictoryPlaceDirectory}/{SceneName}");
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Logger.Log($"[FilesManager] Failed to load {Path}: {e.Message}");
                    return Position;
                }
            }
            else
            {
                Logger.Log($"[FilesManager] File {GameMode}/{s_VictoryPlaceDirectory}/{SceneName} not exist");
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {GameMode}/{s_VictoryPlaceDirectory}/{SceneName} is empty");
                return Position;
            }
            DangerCircleCenter Save = JsonSerializer.Deserialize<DangerCircleCenter>(JSON);

            return new Vector3(Save.x, Save.y, Save.z);
        }

        public static PropDataSave GetProps(string GameMode, string Scene)
        {
            PropDataSave Save = new PropDataSave();
            string Path = $"{s_DataDirectory}/{GameMode}/{s_PropsDirectory}/{Scene}";
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {GameMode}/{s_PropsDirectory}/{Scene}");
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
                Logger.Log($"[FilesManager] File {GameMode}/{s_SpawnPointsDirectory}/{Scene} not exist");
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {GameMode}/{s_SpawnPointsDirectory}/{Scene} is empty");
                return null;
            }
            Save = JsonSerializer.Deserialize<PropDataSave>(JSON);
            return Save;
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

            Logger.Log($"[FilesManager] Loading loot table {Path}");
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
        public static RadialLootSpawnerSave GetRadialLootSpawners(string GameMode, string Scene)
        {
            RadialLootSpawnerSave Save = new RadialLootSpawnerSave();
            string Path = $"{s_DataDirectory}/{GameMode}/{s_RadialLootSpawnersDirectory}/{Scene}";
            string JSON = "";

            Logger.Log($"[FilesManager] Loading file {GameMode}/{s_RadialLootSpawnersDirectory}/{Scene}");
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
                Logger.Log($"[FilesManager] File {GameMode}/{s_RadialLootSpawnersDirectory}/{Scene} not exist");
            }

            if (string.IsNullOrEmpty(JSON))
            {
                Logger.Log($"[FilesManager] File {GameMode}/{s_RadialLootSpawnersDirectory}/{Scene} is empty");
                return null;
            }
            Save = JsonSerializer.Deserialize<RadialLootSpawnerSave>(JSON);
            return Save;
        }
    }
}
