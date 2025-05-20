using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static SkyCoopServer.DataStr;
using static System.Formats.Asn1.AsnWriter;

namespace SkyCoopServer
{
    public class FilesManager
    {
        public static string s_SpawnPointsDirectory = "SpawnPoints";
        public static string s_ZoneConfigDirectory = "ZoneConfig";
        public static string s_StartingGearFileName = "StartingGear";
        public static string s_VictoryPlaceDirectory = "Victory";
        public static string s_RulesFileName = "Rules";
        public static string s_DataDirectory = "";

        public static void SetDataDirectory(string Path)
        {
            s_DataDirectory = Path;
        }

        public static GameRules GetRules(string GameMode)
        {
            GameRules Rules = new GameRules();
            string Path = $"{s_DataDirectory}/{GameMode}/{s_RulesFileName}";
            string JSON = "";
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"FilesManager failed to load {Path}: {e.Message}");
                    return Rules;
                }
            }
            if (string.IsNullOrEmpty(JSON))
            {
                return Rules;
            }
            GameRulesSave Save = JsonSerializer.Deserialize<GameRulesSave>(JSON);
            Rules.m_PlayerCanBeKnocked = Save.Knockdowns;
            Rules.m_PVP = Save.PVP;
            Rules.m_StartingItems = Save.StartingGear;
            Rules.m_Time = Save.Time;
            Rules.m_HUDMode = Save.HUDMode;
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
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"FilesManager failed to load {Path}: {e.Message}");
                    return Points;
                }
            }
            if (string.IsNullOrEmpty(JSON))
            {
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
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"FilesManager failed to load {Path}: {e.Message}");
                    return null;
                }
            }
            else
            {
                return null;
            }
            if (string.IsNullOrEmpty(JSON))
            {
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
            if (File.Exists(Path))
            {
                try
                {
                    JSON = File.ReadAllText(Path);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"FilesManager failed to load {Path}: {e.Message}");
                    return Position;
                }
            }
            if (string.IsNullOrEmpty(JSON))
            {
                return Position;
            }
            DangerCircleCenter Save = JsonSerializer.Deserialize<DangerCircleCenter>(JSON);

            return new Vector3(Save.x, Save.y, Save.z);
        }
    }
}
