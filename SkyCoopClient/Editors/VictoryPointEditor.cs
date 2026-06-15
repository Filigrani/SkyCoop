using Il2Cpp;
using Il2CppTLD.Scenes;
using Il2CppTMPro;
using SkyCoop;
using System.Reflection;
using System.Text.Json;
using UnityEngine;
using UnityEngine.UI;
using static Il2Cpp.Utils;
using static SkyCoopServer.DataStr;

namespace SkyCoopClient
{
    public class VictoryPointEditor
    {
        public static V3Quat m_Point = null;
        public static GameObject m_Visualizer = null;

        public static void DeleteVizualization()
        {
            if (m_Visualizer)
            {
                UnityEngine.Object.Destroy(m_Visualizer);
            }
        }

        public static void UpdateVizualization()
        {
            if (m_Visualizer == null && m_Point != null)
            {
                GameObject Reference = AssetManager.GetAssetFromBundle<GameObject>("Victory");
                if (Reference)
                {
                    GameObject Obj = UnityEngine.Object.Instantiate(Reference, m_Point.m_Position.ConvertToUnity(), m_Point.m_Rotation.ConvertToUnity());
                    if (Obj)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            GameObject VictoryDoll = Obj.transform.GetChild(i).gameObject;
                            if (VictoryDoll)
                            {
                                VictoryDoll.gameObject.SetActive(true);
                                VictoryDoll.GetComponent<Animator>().SetInteger("VictoryPlace", i + 1);
                            }
                            Obj.transform.GetChild(i + 3).gameObject.SetActive(true);
                        }
                        Transform Cam = Obj.transform.FindChild("Camera");
                        Cam.GetComponent<Camera>().enabled = false;
                        Cam.GetComponent<Animator>().enabled = false;
                        m_Visualizer = Obj;
                    }
                }
            }
            if(m_Point == null)
            {
                DeleteVizualization();
            }else if(m_Visualizer)
            {
                m_Visualizer.transform.position = m_Point.m_Position.ConvertToUnity();
                m_Visualizer.transform.rotation = m_Point.m_Rotation.ConvertToUnity();
            }
        }

        public static V3QuatJSON Save()
        {
            if(m_Point == null)
            {
                return null;
            }
            else
            {
                V3QuatJSON Point = new V3QuatJSON();
                Point.position.x = m_Point.m_Position.X;
                Point.position.y = m_Point.m_Position.Y;
                Point.position.z = m_Point.m_Position.Z;

                Point.rotation.x = m_Point.m_Rotation.X;
                Point.rotation.y = m_Point.m_Rotation.Y;
                Point.rotation.z = m_Point.m_Rotation.Z;
                Point.rotation.w = m_Point.m_Rotation.W;
                return Point;
            }
        }

        public static void Load(V3QuatJSON Point)
        {
            if(Point == null)
            {
                m_Point = null;
            }
            else
            {
                m_Point = new V3Quat(Point.position, Point.rotation);
            }
            UpdateVizualization();
        }

        public static void SetPoint()
        {
            Vector3 position = GameManager.GetPlayerTransform().position;
            Quaternion rotation = GameManager.GetPlayerTransform().rotation;


            m_Point = new V3Quat(position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w);
            UpdateVizualization();
        }
    }
}
