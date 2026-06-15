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
    public class ZoneEditor
    {
        public static GameObject m_Visualizer = null;
        public static DangerCircleConfig m_Config = null;

        public static void DeleteVizualization()
        {
            if (m_Visualizer)
            {
                UnityEngine.Object.Destroy(m_Visualizer);
            }
        }

        public static void PreviewStage()
        {

        }

        public static void SetStartingRadius(float Radius)
        {
            if (m_Config != null)
            {
                m_Config.StartingRadius = Radius;
            }
        }

        public static void UpdateVizualization()
        {
            if (m_Visualizer == null && m_Config != null)
            {
                GameObject Reference = AssetManager.GetAssetFromBundle<GameObject>("Zone");
                if (Reference)
                {
                    GameObject Obj = UnityEngine.Object.Instantiate(Reference, m_Config.ActualCenter.GetVector3Unity(), Quaternion.identity);
                    if (Obj)
                    {
                        m_Visualizer = Obj;
                    }
                }
            }
            if(m_Config == null)
            {
                DeleteVizualization();
            }else if (m_Visualizer)
            {
                m_Visualizer.transform.localScale = new Vector3(m_Config.StartingRadius, 4300, m_Config.StartingRadius);
            }
        }

        public static DangerCircleConfig Save()
        {
            if(m_Config == null)
            {
                return null;
            }
            else
            {
                return m_Config;
            }
        }

        public static void Load(DangerCircleConfig Config)
        {
            if(Config == null)
            {
                m_Config = null;
            }
            else
            {
                m_Config = Config;
            }
            UpdateVizualization();
        }

        public static void SetZone()
        {
            Vector3 position = GameManager.GetPlayerTransform().position;

            if(m_Config == null)
            {
                m_Config = new DangerCircleConfig();
                m_Config.StartingRadius = 3000;
                m_Config.Stages = new List<ShrinkStage>();

                ShrinkStage Stage = new ShrinkStage();
                Stage.ShrinkSpeed = 0;
                Stage.DamagePerSecond = 35;
                Stage.StageTime = 0;
                m_Config.Stages.Add(Stage);
            }
            m_Config.ActualCenter = new Vector3JSON(position.x, position.y, position.z);

            UpdateVizualization();
        }
    }
}
