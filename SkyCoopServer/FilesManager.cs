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
        public static string s_DataDirectory = "";

        public static void SetDataDirectory(string Path)
        {
            s_DataDirectory = Path;
        }

        public static List<V3Quat> GetSpawnPoints(string GameMode, string Scene)
        {
            List<V3Quat> Points = new List<V3Quat>();
            string Path = $"{s_DataDirectory}/{s_SpawnPointsDirectory}/{GameMode}/{Scene}";
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
