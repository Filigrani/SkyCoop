using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Text.Json;
using System.Reflection;
using UnityEngine;
using Il2Cpp;
using SkyCoop;
using SkyCoopServer;

namespace SkyCoopClient
{
    public class PropsSpawnsEditor
    {
        public static List<DataStr.PropData> m_Data = new List<DataStr.PropData>();
       
        public static List<GameObject> m_Visualizers = new List<GameObject>();
        public static string PropsDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/SkyModData/Editor/Props";

        public static void UpdateList()
        {
            for (int i = m_Visualizers.Count - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(m_Visualizers[i]);
            }
            for (int i = CanvasUI.m_PropsEditorScrollParnet.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(CanvasUI.m_PropsEditorScrollParnet.GetChild(i).gameObject);
            }
            m_Visualizers.Clear();

            for (int i = 0; i < m_Data.Count; i++)
            {
                DataStr.PropData Data = m_Data[i];

                GameObject Viszualizer;

                if (Data.frombundle)
                {
                    Viszualizer = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromBundle<GameObject>(Data.prefabname), Data.GetVector3Unity(), Data.GetQuaternionUnity());
                }
                else
                {
                    Viszualizer = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromGame<GameObject>(Data.prefabname), Data.GetVector3Unity(), Data.GetQuaternionUnity());
                }

                m_Visualizers.Add(Viszualizer);
                GameObject NewElemenet = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromBundle<GameObject>("SpawnPointEditorElement"), CanvasUI.m_PropsEditorScrollParnet);

                int NewIndex = CanvasUI.m_PropsEditorScrollParnet.transform.childCount - 1;
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
            m_Data.RemoveAt(Index);
            UpdateList();
        }

        public static void Teleport(GameObject Obj)
        {
            int Index = int.Parse(Obj.name);
            SkyCoop.Logger.Log("Teleport to index " + Index);
            GameManager.GetPlayerManagerComponent().TeleportPlayer(m_Data[Index].GetVector3Unity(), m_Data[Index].GetQuaternionUnity());
        }

        public static string GetFileName()
        {
            return ModMain.GetCurrentSceneName();
        }

        public static void Save()
        {
            DataStr.PropDataSave Save = new DataStr.PropDataSave();
            Save.props = new List<DataStr.PropData>(m_Data);

            string JSON = JsonSerializer.Serialize<DataStr.PropDataSave>(Save);
            SkyCoop.Logger.Log(JSON);

            string FileName = GetFileName();

            SkyCoop.Logger.Log(ConsoleColor.Blue, $"Saving PropDataSave for scene {FileName}");
            try
            {
                if (!Directory.Exists(PropsDirectory))
                    Directory.CreateDirectory(PropsDirectory);
                File.WriteAllText($"{PropsDirectory}/{FileName}", JSON);
            }
            catch (Exception e)
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"Cant save file! Error: {e.Message}");
            }
        }

        public static void LoadCurrentSceneFile()
        {
            string FileName = GetFileName();
            string PropsJSON;

            SkyCoop.Logger.Log(ConsoleColor.Blue, $"Loading Props for scene {FileName}");
            try
            {
                if (!Directory.Exists(PropsDirectory))
                {
                    SkyCoop.Logger.Log(ConsoleColor.Red, $"Directory {PropsDirectory} not exists");
                    return;
                }
                PropsJSON = File.ReadAllText($"{PropsDirectory}/{FileName}");
            }
            catch (Exception e)
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"Cant save file! Error: {e.Message}");
                return;
            }

            if (string.IsNullOrEmpty(PropsJSON))
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"File {FileName} is empty");
                return;
            }
            else
            {
                Load(PropsJSON);
            }
        }

        public static void Load(string JSON)
        {
            m_Data.Clear();

            DataStr.PropDataSave Save = JsonSerializer.Deserialize<DataStr.PropDataSave>(JSON);
            for (int i = 0; i < Save.props.Count; i++)
            {
                m_Data.Add(Save.props[i]);
            }
            UpdateList();
        }

        public static void TogglePropsEditor()
        {
            if (CanvasUI.m_PropsEditor)
            {
                CanvasUI.m_PropsEditor.SetActive(!CanvasUI.m_PropsEditor.activeSelf);
            }
            if (CanvasUI.m_SpawnPointEditor)
            {
                CanvasUI.m_SpawnPointEditor.SetActive(false);
            }
        }

        public static void AddProp()
        {
            DataStr.PropData Data = new DataStr.PropData();
            Vector3 pos = GameManager.GetPlayerTransform().position;
            Quaternion rot = GameManager.GetPlayerTransform().rotation;
            Data.posx = pos.x;
            Data.posy = pos.y;
            Data.posz = pos.z;
            Data.rotx = rot.x;
            Data.roty = rot.y;
            Data.rotz = rot.z;
            Data.prefabname = CanvasUI.s_PropsEditorPrefabName.text;
            Data.frombundle = CanvasUI.s_PropsEditorIsFromBundle.isOn;
            Data.guid = Guid.NewGuid().ToString();

            m_Data.Add(Data);

            UpdateList();
        }
    }
}
