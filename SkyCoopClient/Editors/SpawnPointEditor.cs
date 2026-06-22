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
        public static List<V3Quat> m_Points = new List<V3Quat>();
        public static List<GameObject> m_Visualizers = new List<GameObject>();
        public static List<GameObject> m_MapMarkers = new List<GameObject>();

        public static void DeleteVizualization()
        {
            for (int i = m_Visualizers.Count - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(m_Visualizers[i]);
            }
            m_Visualizers.Clear();
        }

        public static void UpdateVizualization()
        {
            DeleteVizualization();
            for (int i = 0; i < m_Points.Count; i++)
            {
                Vector3 position = m_Points[i].m_Position.ConvertToUnity();
                Quaternion rotation = m_Points[i].m_Rotation.ConvertToUnity();
                GameObject Viszualizer = UnityEngine.Object.Instantiate<GameObject>(GameManager.GetPlayerManagerComponent().m_OscarPrefab, position, rotation);
                m_Visualizers.Add(Viszualizer);
            }
        }

        public static void Delete(int Index)
        {
            m_Points.RemoveAt(Index);
            UpdateVizualization();
        }

        public static void Teleport(int Index)
        {
            V3Quat Point = m_Points[Index];
            Vector3 position = Point.m_Position.ConvertToUnity();
            Quaternion rotation = Point.m_Rotation.ConvertToUnity();
            GameManager.GetPlayerManagerComponent().TeleportPlayer(position, rotation);
        }

        public static List<V3QuatJSON> Save()
        {
            List<V3QuatJSON> SpawnPoints = new List<V3QuatJSON>();

            for (int i = 0; i < m_Points.Count; i++)
            {
                V3Quat PointToSave = m_Points[i];

                V3QuatJSON Point = new V3QuatJSON();
                Point.position.x = PointToSave.m_Position.X;
                Point.position.y = PointToSave.m_Position.Y;
                Point.position.z = PointToSave.m_Position.Z;

                Point.rotation.x = PointToSave.m_Rotation.X;
                Point.rotation.y = PointToSave.m_Rotation.Y;
                Point.rotation.z = PointToSave.m_Rotation.Z;

                SpawnPoints.Add(Point);
            }
            return SpawnPoints;
        }

        public static void Load(List<V3QuatJSON> Points)
        {
            m_Points.Clear();
            for (int i = 0; i < Points.Count; i++)
            {
                V3QuatJSON Point = Points[i];
                m_Points.Add(new V3Quat(Point.position, Point.rotation));
            }
            UpdateVizualization();
        }

        public static void AddSpawnPoint()
        {
            Vector3 position = GameManager.GetPlayerTransform().position;
            Quaternion rotation = GameManager.GetPlayerTransform().rotation;


            V3Quat Point = new V3Quat(position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w);


            m_Points.Add(Point);
            UpdateVizualization();
        }
    }
}
