using Harmony;
using Il2Cpp;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Text.Json;

namespace SkyCoopClient
{
    public class SpawnPointEditor
    {
        public static List<Vector3> m_Vectors = new List<Vector3>();
        public static List<Quaternion> m_Quaternions = new List<Quaternion>();
        public static List<GameObject> m_Visualizers = new List<GameObject>();


        [Serializable]
        public struct SpawnPoint
        {
            public float posx;
            public float posy;
            public float posz;

            public float rotx;
            public float roty;
            public float rotz;
            public float rotw;
        }

        [Serializable]
        public struct SpawnPointSave
        {
            public List<SpawnPoint> points;
        }


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

            string JSON = JsonSerializer.Serialize(Save);
            SkyCoop.Logger.Log(JSON);

        }

        public static void Load()
        {

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
