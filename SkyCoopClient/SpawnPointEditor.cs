using Il2Cpp;
using SkyCoop;
using UnityEngine;
using UnityEngine.UI;
using System.Text.Json;
using Il2CppTLD.Scenes;
using System.Reflection;
using static SkyCoopServer.DataStr;

namespace SkyCoopClient
{
    public class SpawnPointEditor
    {
        public static List<Vector3> m_Vectors = new List<Vector3>();
        public static List<Quaternion> m_Quaternions = new List<Quaternion>();
        public static List<GameObject> m_Visualizers = new List<GameObject>();
        public static string SpawnPointsDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/SkyModData/Editor/SpawnPoints";

        public static void UpdateList()
        {
            for (int i = m_Visualizers.Count-1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(m_Visualizers[i]);
            }
            for (int i = CanvasUI.m_SpawnPointEditorScrollParnet.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(CanvasUI.m_SpawnPointEditorScrollParnet.GetChild(i).gameObject);
            }
            m_Visualizers.Clear();

            for (int i = 0; i < m_Vectors.Count; i++)
            {
                GameObject Viszualizer = UnityEngine.Object.Instantiate<GameObject>(GameManager.GetPlayerManagerComponent().m_OscarPrefab, m_Vectors[i], m_Quaternions[i]);
                m_Visualizers.Add(Viszualizer);
                GameObject NewElemenet = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromBundle<GameObject>("SpawnPointEditorElement"), CanvasUI.m_SpawnPointEditorScrollParnet);
                
                int NewIndex = CanvasUI.m_SpawnPointEditorScrollParnet.transform.childCount - 1;
                NewElemenet.name = NewIndex.ToString();

                Action act = new Action(() => Delete(NewElemenet));
                NewElemenet.transform.GetChild(0).GetComponent<Button>().m_OnClick.AddListener(act);

                Action act2 = new Action(() => Teleport(NewElemenet));
                NewElemenet.transform.GetChild(1).GetComponent<Button>().m_OnClick.AddListener(act2);
            }
        }

        public static void Delete(GameObject Obj)
        {
            int Index = int.Parse(Obj.name);
            SkyCoop.Logger.Log("Delete " + Index);
            m_Vectors.RemoveAt(Index);
            m_Quaternions.RemoveAt(Index);
            UpdateList();
        }

        public static void Teleport(GameObject Obj)
        {
            int Index = int.Parse(Obj.name);
            SkyCoop.Logger.Log("Teleport to index "+Index);
            GameManager.GetPlayerManagerComponent().TeleportPlayer(m_Vectors[Index], m_Quaternions[Index]);
        }

        public static string GetFileName()
        {
            return ModMain.GetCurrentSceneName();
        }

        public static void Save()
        {
            SpawnPointSave Save = new SpawnPointSave();
            List<SpawnPoint> points = new List<SpawnPoint>();

            for (int i = 0; i < m_Vectors.Count; i++)
            {
                Vector3 v3 = m_Vectors[i];
                Quaternion quat = m_Quaternions[i];

                SpawnPoint Point = new SpawnPoint();
                Point.posx = v3.x;
                Point.posy = v3.y;
                Point.posz = v3.z;

                Point.rotx = quat.x;
                Point.roty = quat.y;
                Point.rotz = quat.z;
                Point.rotw = quat.w;

                points.Add(Point);
            }

            Save.points = points;

            string JSON = JsonSerializer.Serialize<SpawnPointSave>(Save);
            SkyCoop.Logger.Log(JSON);

            string FileName = GetFileName();

            SkyCoop.Logger.Log(ConsoleColor.Blue, $"Saving SpawnPointsFile for region {FileName}");
            try
            {
                if (!Directory.Exists(SpawnPointsDirectory))
                    Directory.CreateDirectory(SpawnPointsDirectory);
                File.WriteAllText($"{SpawnPointsDirectory}/{FileName}", JSON);
                SkyCoop.Logger.Log(ConsoleColor.Green, $"Save was successful");
            }
            catch (Exception e) 
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"Cant save file because has error: {e.Message}");
            }
        }

        public static void LoadCurrentSceneFile()
        {
            string FileName = GetFileName();
            string SpawnPoints;

            SkyCoop.Logger.Log(ConsoleColor.Blue, $"Loading SpawnPointsFile for region {FileName}");
            try
            {
                if (!Directory.Exists(SpawnPointsDirectory))
                {
                    SkyCoop.Logger.Log(ConsoleColor.Red, $"Directory {SpawnPointsDirectory} not exists");
                    return;
                }
                SpawnPoints = File.ReadAllText($"{SpawnPointsDirectory}/{FileName}");
                SkyCoop.Logger.Log(ConsoleColor.Green, $"Load was successful");
            }
            catch (Exception e)
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"Cant save file because has error: {e.Message}");
                return;
            }

            if (string.IsNullOrEmpty(SpawnPoints))
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"File {FileName} is empty");
                return;
            }
            else
            {
                Load(SpawnPoints);
            }
        }

        public static void Load(string JSON)
        {
            m_Vectors.Clear();
            m_Quaternions.Clear();
            
            SpawnPointSave Save = JsonSerializer.Deserialize<SpawnPointSave>(JSON);
            for (int i = 0; i < Save.points.Count; i++)
            {
                SpawnPoint Point = Save.points[i];
                m_Vectors.Add(new Vector3(Point.posx, Point.posy, Point.posz));
                m_Quaternions.Add(new Quaternion(Point.rotx, Point.roty, Point.rotz, Point.rotw));
            }
            UpdateList();
        }

        public static void ToggleSpawnPointEditor()
        {
            if (CanvasUI.m_SpawnPointEditor)
            {
                CanvasUI.m_SpawnPointEditor.SetActive(!CanvasUI.m_SpawnPointEditor.activeSelf);
            }
        }

        public static void AddSpawnPoint()
        {
            m_Vectors.Add(GameManager.GetPlayerTransform().position);
            m_Quaternions.Add(GameManager.GetPlayerTransform().rotation);
            UpdateList();
        }
    }
}
