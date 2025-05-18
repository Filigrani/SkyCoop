using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static SkyCoopServer.DataStr;

namespace SkyCoopServer
{
    public class FilesManager
    {
        public static string s_SpawnPointsDirectory = "SpawnPoints";
        public static string s_StartingGearFileName = "StartingGear";
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
            return Rules;
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
    }
}
