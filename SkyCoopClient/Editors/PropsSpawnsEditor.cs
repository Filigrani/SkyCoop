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
using Il2CppTMPro;

namespace SkyCoopClient
{
    public class PropsSpawnsEditor
    {
        public static List<DataStr.PropData> m_Data = new List<DataStr.PropData>();
       
        public static List<GameObject> m_Visualizers = new List<GameObject>();

        public static void UpdateList()
        {
            m_Visualizers.Clear();

            for (int i = 0; i < m_Data.Count; i++)
            {
                DataStr.PropData Data = m_Data[i];

                GameObject Viszualizer;

                if (!Data.prefabname.StartsWith("Deco"))
                {
                    if (Data.frombundle)
                    {
                        Viszualizer = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromBundle<GameObject>(Data.prefabname), Data.position.GetVector3Unity(), Data.rotation.GetQuaternionUnity());
                        Viszualizer.name = Data.prefabname;
                    }
                    else
                    {
                        Viszualizer = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromGame<GameObject>(Data.prefabname), Data.position.GetVector3Unity(), Data.rotation.GetQuaternionUnity());
                        Viszualizer.name = Data.prefabname;
                    }
                }
                else
                {
                    int DecoIndex;
                    if(!int.TryParse(Data.prefabname.Replace("Deco", "").Trim(), out DecoIndex))
                    {
                        DecoIndex = 0;
                    }
                    Viszualizer = MaterialsContainer.GetDecoByIndex(DecoIndex);
                    Viszualizer.transform.position = Data.position.GetVector3Unity();
                    Viszualizer.transform.rotation = Data.rotation.GetQuaternionUnity();
                }
                Viszualizer.AddComponent<Comps.PropsEditorVisuzlier>().m_IndexHandler = i;

                m_Visualizers.Add(Viszualizer);
            }
        }

        public static void Delete(GameObject Obj)
        {
            int Index = int.Parse(Obj.name);
            SkyCoop.Logger.Log("Delete " + Index);
            m_Data.RemoveAt(Index);
            UpdateList();
        }

        public static void Teleport(int Index)
        {
            SkyCoop.Logger.Log("Teleport to index " + Index);
            GameManager.GetPlayerManagerComponent().TeleportPlayer(m_Data[Index].position.GetVector3Unity(), m_Data[Index].rotation.GetQuaternionUnity());
        }

        public static void PlaceMode(GameObject Obj)
        {
            int Index = int.Parse(Obj.name);
            SkyCoop.Logger.Log("Place mode for index " + Index);
            m_Visualizers[Index].GetComponent<Comps.PropsEditorVisuzlier>().Place();
        }

        public static string GetFileName()
        {
            return ModMain.GetCurrentSceneName();
        }

        public static List<DataStr.PropData> Save()
        {
            return new List<DataStr.PropData>(m_Data);
        }

        public static void Load(List<DataStr.PropData> Props)
        {
            m_Data.Clear();
            m_Data = new List<DataStr.PropData>(Props);
            UpdateList();
        }

        public static void ApplyTransformFromVizualizer(Transform VizualizerTransform, int PropIndex)
        {
            DataStr.PropData Prop = m_Data[PropIndex];

            Prop.position.x = VizualizerTransform.position.x;
            Prop.position.y = VizualizerTransform.position.y;
            Prop.position.z = VizualizerTransform.position.z;

            Prop.rotation.x = VizualizerTransform.rotation.eulerAngles.x;
            Prop.rotation.y = VizualizerTransform.rotation.eulerAngles.y;
            Prop.rotation.y = VizualizerTransform.rotation.eulerAngles.z;
        }

        public static void ApplyTransformFromVizualizer(GameObject Vizualizer)
        {
            if (Vizualizer)
            {
                Comps.PropsEditorVisuzlier Comp = Vizualizer.GetComponent<Comps.PropsEditorVisuzlier>();
                if (Comp)
                {
                    ApplyTransformFromVizualizer(Vizualizer.transform, Comp.m_IndexHandler);
                }
            }
        }

        public static void AddProp(string PrefabName, bool IsFromBundle)
        {
            DataStr.PropData Data = new DataStr.PropData();
            Vector3 pos = GameManager.GetPlayerTransform().position;
            Vector3 rot = GameManager.GetPlayerTransform().rotation.eulerAngles;

            Data.position.x = pos.x;
            Data.position.y = pos.y;
            Data.position.z = pos.z;

            Data.rotation.x = rot.x;
            Data.rotation.y = rot.y;
            Data.rotation.z = rot.z;

            Data.prefabname = PrefabName;
            Data.frombundle = IsFromBundle;
            Data.guid = Guid.NewGuid().ToString();

            m_Data.Add(Data);

            UpdateList();
        }
    }
}
