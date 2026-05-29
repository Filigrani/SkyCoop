using Il2Cpp;
using Il2CppTMPro;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Il2CppRewired.Controller;
using static SkyCoopServer.DataStr;

namespace SkyCoopClient
{
    // Это едитор для раставления точек спавна для нашего режима *[REDACTED]*
    // Так как на выделеном сервере нет ассетов игры, нам нужно записывать возможнные точки спавна на ландшавте самим.
    // По этому мы и кидаем кучу рейкастов что бы тупо "пощупать" ландшафт и сохранить там где можно ставить лут.
    
    public class RadialLootSpawnersEditor
    {
        public static string s_RadialSpawnersDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/SkyModData/Editor/RadialLootSpawners";
        public static List<DataStr.RadialLootSpawner> s_Spawners = new List<DataStr.RadialLootSpawner>();
        public static List<GameObject> s_Vizualizers = new List<GameObject>();

        public static float s_UpwardRaycastLength = 3f;
        public static float s_SpawnerRange = 3.5f;
        public static string s_SpawnerLootTable = "";
        public const float c_MaxPointsPerSpawner = 30;


        public static GameObject CreateDebugPrimitive(PrimitiveType Type, string Name, UnityEngine.Color Color, Transform Parent = null)
        {
            GameObject Obj = GameObject.CreatePrimitive(Type);
            Obj.name = Name;
            if (Parent)
            {
                Obj.transform.SetParent(Parent);
            }
            Obj.GetComponent<Renderer>().material.color = Color;
            UnityEngine.Object.Destroy(Obj.GetComponent<Collider>());
            return Obj;
        }

        public static void Delete(int Index)
        {
            s_Spawners.RemoveAt(Index);
            UpdateVizualization();
        }

        public static void Save()
        {
            RadialLootSpawnerSave Save = new RadialLootSpawnerSave();
            Save.spawners = new List<RadialLootSpawner>(s_Spawners);

            string JSON = JsonSerializer.Serialize<RadialLootSpawnerSave>(Save);
            SkyCoop.Logger.Log(JSON);

            string FileName = ModMain.GetCurrentSceneName();

            SkyCoop.Logger.Log(ConsoleColor.Blue, $"Saving RadialLootSpawners for scene {FileName}");
            try
            {
                if (!Directory.Exists(s_RadialSpawnersDirectory))
                    Directory.CreateDirectory(s_RadialSpawnersDirectory);
                File.WriteAllText($"{s_RadialSpawnersDirectory}/{FileName}", JSON);
            }
            catch (Exception e)
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"Cant save file! Error: {e.Message}");
            }
        }

        public static string GetFileName()
        {
            return ModMain.GetCurrentSceneName();
        }

        public static void LoadCurrentSceneFile()
        {
            string FileName = GetFileName();
            string JSON;

            SkyCoop.Logger.Log(ConsoleColor.Blue, $"Loading RadialLootSpawners for scene {FileName}");
            try
            {
                if (!Directory.Exists(s_RadialSpawnersDirectory))
                {
                    SkyCoop.Logger.Log(ConsoleColor.Red, $"Directory {s_RadialSpawnersDirectory} not exists");
                    return;
                }
                JSON = File.ReadAllText($"{s_RadialSpawnersDirectory}/{FileName}");
                SkyCoop.Logger.Log(ConsoleColor.Green, $"Load was successful");
            }
            catch (Exception e)
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"Cant save file because of an error: {e.Message}");
                return;
            }

            if (string.IsNullOrEmpty(JSON))
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"File {FileName} is empty");
                return;
            }
            else
            {
                Load(JSON);
            }
        }

        public static void Load(string JSON)
        {
            s_Spawners.Clear();

            RadialLootSpawnerSave Save = JsonSerializer.Deserialize<RadialLootSpawnerSave>(JSON);
            for (int i = 0; i < Save.spawners.Count; i++)
            {
                RadialLootSpawner Spawner = Save.spawners[i];
                s_Spawners.Add(Spawner);
            }
            UpdateVizualization();
        }


        public static void CreateSpawner()
        {
            if (GameManager.m_PlayerObject)
            {
                Vector3 centerPos = GameManager.GetPlayerTransform().position;

                float Top = s_UpwardRaycastLength;

                LayerMask RayCastMask = (1 << vp_Layer.Default) |
                (1 << vp_Layer.InteractiveProp) |
                (1 << vp_Layer.InteractivePropNoCollidePlayer) |
                (1 << vp_Layer.Container) |
                (1 << vp_Layer.Buildings) |
                (1 << vp_Layer.Decoration) |
                (1 << vp_Layer.Ground) |
                (1 << vp_Layer.GroundNoNavmesh) |
                (1 << vp_Layer.Water) |
                (1 << vp_Layer.TerrainObject);

                List<JSONPoint> Points = new List<JSONPoint>();

                if(Physics.Raycast(centerPos, Vector3.up, out RaycastHit _hit, Top, RayCastMask))
                {
                    Top = Vector3.Distance(centerPos, _hit.point);
                }

                Vector3 rayStart = centerPos;
                Vector3 peakPoint = rayStart + Vector3.up * Top;

                for (int i = 0; i < c_MaxPointsPerSpawner; i++)
                {
                    float angle = UnityEngine.Random.Range(0f, 360f);
                    float distance = UnityEngine.Random.Range(0f, s_SpawnerRange);

                    // Создаем направление от пиковой точки
                    Vector3 horizontalDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                    Vector3 rayOrigin = peakPoint + horizontalDirection * distance;

                    float MaxRaycastLength = s_SpawnerRange + s_UpwardRaycastLength + 0.5f;

                    // Луч вниз от случайной точки вокруг пика
                    if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, MaxRaycastLength, RayCastMask))
                    {
                        Vector3 surfaceNormal = hit.normal;
                        bool isWall = Mathf.Abs(surfaceNormal.y) < 0.1f;
                        bool isTooSteep = surfaceNormal.y < 0.7f;

                        if (!isWall && !isTooSteep)
                        {
                            //Vector3 spawnPos = hit.point + Vector3.up * 0.1f;
                            Points.Add(new JSONPoint(hit.point.x, hit.point.y, hit.point.z));
                        }
                    }
                    else
                    {
                        SkyCoop.Logger.Log(ConsoleColor.Yellow, $"[RadialLootSpawnersEditor] Raycast failed for point {i}");
                    }
                }
                DataStr.RadialLootSpawner Spawner = new DataStr.RadialLootSpawner();
                Spawner.center = new JSONPoint(centerPos.x, centerPos.y, centerPos.z);
                Spawner.points = new List<JSONPoint>(Points);
                Spawner.top = Top;
                s_Spawners.Add(Spawner);
            }
            UpdateVizualization();
        }

        public static void DeleteVizualization()
        {
            //                                    да-да мин-максим проверяя только -1, и в тоже время пишим говно код, оптимизатор я от бога конечно.
            for (int i = s_Vizualizers.Count - 1; i != -1; i--)
            {
                GameObject Obj = s_Vizualizers[i];
                if (Obj) // Если удалить объект он всё ещё будет налом в листе. Хер его знает, сцену там поменяешь, да ты и сам потом будешь орать когда удалишь в эксплорере.
                {
                    UnityEngine.Object.Destroy(Obj);
                }
            }
            s_Vizualizers.Clear();

        }

        public static void UpdateVizualization(int Index)
        {
            DataStr.RadialLootSpawner Spawner = s_Spawners[Index];

            if (Spawner != null)
            {
                Transform Dummy = new GameObject("RadialLootSpawnerPoint").transform;

                GameObject UpwardRay = CreateDebugPrimitive(PrimitiveType.Cube, "UpwardRay", UnityEngine.Color.green, Dummy);
                UpwardRay.transform.localScale = new Vector3(0.3f, Spawner.top, 0.3f);

                // У всех примитивов пивот в центре. По этому все "лучи" которые мы будем визиуализировать мы присираем в цетре а не в их начале.
                Vector3 rayStart = Spawner.center.GetVector3Unity();
                Vector3 rayEnd = rayStart + Vector3.up * Spawner.top;
                UpwardRay.transform.position = (rayStart + rayEnd) * 0.5f;

                foreach (JSONPoint Point in Spawner.points)
                {
                    Vector3 pointUnity = Point.GetVector3Unity();
                    Vector3 pointRayEnd = pointUnity + Vector3.up * Spawner.top;

                    Transform SubDummy = new GameObject("Point").transform;
                    SubDummy.position = pointUnity;

                    GameObject SpawnerPoint = CreateDebugPrimitive(PrimitiveType.Sphere, "Point", UnityEngine.Color.red, SubDummy);
                    SpawnerPoint.transform.localScale = Vector3.one * 0.3f; // Учись пока я жив. Это быстрее чем new Vector3(0.3f, 0.3f, 0.3f);
                    SpawnerPoint.transform.position = pointUnity;

                    GameObject RayToPeak = CreateDebugPrimitive(PrimitiveType.Cube, "Ray", UnityEngine.Color.red, SubDummy);

                    float Distance = Vector3.Distance(pointUnity, rayEnd);
                    Vector3 direction = (rayEnd - pointUnity).normalized;

                    RayToPeak.transform.position = pointUnity + direction * (Distance * 0.5f);
                    RayToPeak.transform.rotation = Quaternion.LookRotation(direction);
                    RayToPeak.transform.Rotate(90, 0, 0);
                    RayToPeak.transform.localScale = new Vector3(0.2f, Distance, 0.2f);

                    SubDummy.SetParent(Dummy);
                }

                if (s_Vizualizers.Count - 1 < Index)
                {
                    s_Vizualizers.Add(Dummy.gameObject);
                }
                else
                {
                    UnityEngine.Object.Destroy(s_Vizualizers[Index]);
                    s_Vizualizers[Index] = Dummy.gameObject;
                }
            }
        }

        public static void UpdateVizualization()
        {
            DeleteVizualization();
            for (int i = 0; i < s_Spawners.Count; i++)
            {
                UpdateVizualization(i);
            }
        }
        public static void Teleport(int Index)
        {
            GameManager.GetPlayerManagerComponent().TeleportPlayer(s_Spawners[Index].center.GetVector3Unity(), Quaternion.identity);
        }
    }
}
