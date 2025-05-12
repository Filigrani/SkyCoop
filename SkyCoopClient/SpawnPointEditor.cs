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

namespace SkyCoopClient
{
    public class SpawnPointEditor
    {
        public static List<Vector3> m_Vectors = new List<Vector3>();
        public static List<Quaternion> m_Quaternions = new List<Quaternion>();
        public static List<GameObject> m_Visualizers = new List<GameObject>();


        public static void UpdateList()
        {
            for (int i = m_Visualizers.Count-1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(m_Visualizers[i]);
            }
            for (int i = CanvasUI.m_SpawnPointEditorScrollParnet.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(CanvasUI.m_SpawnPointEditorScrollParnet.GetChild(i).gameObject);
            }
            m_Visualizers.Clear();

            for (int i = 0; i < m_Vectors.Count; i++)
            {
                GameObject Viszualizer = UnityEngine.Object.Instantiate<GameObject>(GameManager.GetPlayerManagerComponent().m_OscarPrefab, m_Vectors[i], m_Quaternions[i]);
                m_Visualizers.Add(Viszualizer);
                GameObject NewElemenet = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromBundle<GameObject>("SpawnPointEditorElement"), CanvasUI.m_SpawnPointEditorScrollParnet);

                Action act = new Action(() => Delete(i));
                NewElemenet.transform.GetChild(0).GetComponent<Button>().m_OnClick.AddListener(act);

                Action act2 = new Action(() => Teleport(i));
                NewElemenet.transform.GetChild(1).GetComponent<Button>().m_OnClick.AddListener(act2);
            }
        }

        public static void Delete(int i)
        {
            m_Vectors.RemoveAt(i);
            m_Quaternions.RemoveAt(i);
            UpdateList();
        }

        public static void Teleport(int i)
        {
            GameManager.GetPlayerManagerComponent().TeleportPlayer(m_Vectors[i], m_Quaternions[i]);
        }

        public static void Save()
        {

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
