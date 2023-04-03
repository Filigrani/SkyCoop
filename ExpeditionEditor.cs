using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static SkyCoop.DataStr;
using static SkyCoop.ExpeditionBuilder;
using static SkyCoop.ExpeditionManager;
using MelonLoader.TinyJSON;
using MelonLoader;
using System.Security.Policy;
using static SkyCoop.Comps;
using Harmony;
using Il2Cpp;

namespace SkyCoop
{
    public class ExpeditionEditor
    {
        public static List<ExpeditionGearSpawner> m_Spawns = new List<ExpeditionGearSpawner>();
        public static Vector3 m_Center = new Vector3(0, 0, 0);
        public static List<string> m_Containers = new List<string>();
        public static List<string> m_Plants = new List<string>();
        public static List<string> m_Breakdowns = new List<string>();
        public static List<UniversalSyncableObjectSpawner> m_Objects = new List<UniversalSyncableObjectSpawner>();
        public static List<GameObject> m_VisualizeObjects = new List<GameObject>();
        public static string m_LastSpawnerGUID = "";
        public static string m_LastObjectGUID = "";
        public static float m_LastChance = 0;
        public static int m_LastType = 0;
        public static float m_LastRadious = 0;
        public static GameObject m_InnerSphere = null;
        public static GameObject m_OutterSphere = null;
        public static int LastSelectedRegion = 6;

#if (DEDICATED_LINUX)
        public static string Seperator = @"/";
#else
        public static string Seperator = @"\";
#endif

        public static void DisableRadiousSpheres()
        {
            if (m_InnerSphere != null)
            {
                m_InnerSphere.SetActive(false);
            }
            if (m_OutterSphere != null)
            {
                m_OutterSphere.SetActive(false);
            }
        }

        public static void SetRadiousSpheres()
        {
            if(m_LastType == (int) ExpeditionTaskType.ENTERSCENE)
            {
                if (m_InnerSphere != null)
                {
                    m_InnerSphere.SetActive(false);
                }
                if (m_OutterSphere != null)
                {
                    m_OutterSphere.SetActive(false);
                }
                return;
            }
            
            float Radious = MyMod.ExpeditionEditorUI.transform.GetChild(6).gameObject.GetComponent<UnityEngine.UI.Slider>().m_Value;
            if (m_InnerSphere == null)
            {
                m_InnerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                m_OutterSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                UnityEngine.Object.Destroy(m_InnerSphere.GetComponent<SphereCollider>());
                UnityEngine.Object.Destroy(m_OutterSphere.GetComponent<SphereCollider>());
            }
            
            m_InnerSphere.transform.position = m_Center;
            m_InnerSphere.transform.localScale = new Vector3(Radious * 2, Radious * 2, Radious * 2);
            m_InnerSphere.SetActive(true);
            m_OutterSphere.transform.position = m_Center;
            m_OutterSphere.transform.localScale = new Vector3(-Radious * 2, -Radious * 2, -Radious * 2);
            m_OutterSphere.SetActive(true);
        }

        public static void DisableAllPanels()
        {
            MyMod.ExpeditionEditorUI.transform.GetChild(13).gameObject.SetActive(false);
            MyMod.ExpeditionEditorUI.transform.GetChild(14).gameObject.SetActive(false);
            MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(7).gameObject.SetActive(false);
            MyMod.ExpeditionEditorUI.transform.GetChild(27).gameObject.SetActive(false);
            MyMod.ExpeditionEditorUI.transform.GetChild(28).gameObject.SetActive(false);
            MyMod.ExpeditionEditorUI.transform.GetChild(31).gameObject.SetActive(false);
            MyMod.ExpeditionEditorUI.transform.GetChild(31).GetChild(4).gameObject.SetActive(false);
        }

        public static void BackToSelect(int Region)
        {
            RefreshExpeditionsList(Region);
            DisableAllPanels();
            MyMod.ExpeditionEditorUI.SetActive(false);
            MyMod.ExpeditionEditorSelectUI.SetActive(true);
            DisableRadiousSpheres();
            DisableVisualizeObjects();
        }

        public static void ExpeditionTypeChanged()
        {
            MyMod.ExpeditionEditorUI.transform.GetChild(17).GetComponent<UnityEngine.UI.Toggle>().Set(m_LastType == (int)ExpeditionTaskType.ENTERSCENE);
            SetRadiousSpheres();
        }
        public static void ExpeditionRadiousChanged()
        {
            SetRadiousSpheres();
        }

        public static void SetObjectiveGear()
        {
            MyMod.ExpeditionEditorUI.transform.GetChild(20).GetComponent<UnityEngine.UI.InputField>().SetText(m_LastSpawnerGUID);
        }

        public static void AutoSelectRegion()
        {
            if (GameManager.GetUniStorm() != null)
            {
                int Region = (int)MyMod.ConvertGameRegion(GameManager.GetUniStorm().m_CurrentRegion);
                Region += Shared.GameRegionNegativeOffset;

                MyMod.ExpeditionEditorUI.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Dropdown>().Set(Region);
            }
        }
        public static void AutoSelectScene()
        {
            MyMod.ExpeditionEditorUI.transform.GetChild(4).gameObject.GetComponent<UnityEngine.UI.InputField>().SetText(MyMod.level_guid);
        }
        public static void ToggleContainersList()
        {
            bool OldState = MyMod.ExpeditionEditorUI.transform.GetChild(13).gameObject.activeSelf;
            DisableAllPanels();
            MyMod.ExpeditionEditorUI.transform.GetChild(13).gameObject.SetActive(!OldState);
        }
        public static void ToggleGearsList()
        {
            bool OldState = MyMod.ExpeditionEditorUI.transform.GetChild(14).gameObject.activeSelf;
            DisableAllPanels();
            MyMod.ExpeditionEditorUI.transform.GetChild(14).gameObject.SetActive(!OldState);
        }
        public static void ToggleHarvestablesList()
        {
            bool OldState = MyMod.ExpeditionEditorUI.transform.GetChild(27).gameObject.activeSelf;
            DisableAllPanels();
            MyMod.ExpeditionEditorUI.transform.GetChild(27).gameObject.SetActive(!OldState);
        }
        public static void ToggleBreakdownsList()
        {
            bool OldState = MyMod.ExpeditionEditorUI.transform.GetChild(28).gameObject.activeSelf;
            DisableAllPanels();
            MyMod.ExpeditionEditorUI.transform.GetChild(28).gameObject.SetActive(!OldState);
        }
        public static void ToggleExtraPanel()
        {
            MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(7).gameObject.SetActive(!MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(7).gameObject.activeSelf);
        }
        public static void ToggleObjectSettingsPanel()
        {
            MyMod.ExpeditionEditorUI.transform.GetChild(31).GetChild(4).gameObject.SetActive(!MyMod.ExpeditionEditorUI.transform.GetChild(31).GetChild(4).gameObject.activeSelf);
        }
        public static void ToggleObjectsList()
        {
            bool OldState = MyMod.ExpeditionEditorUI.transform.GetChild(31).gameObject.activeSelf;
            DisableAllPanels();
            MyMod.ExpeditionEditorUI.transform.GetChild(31).gameObject.SetActive(!OldState);
        }

        public static void AutoPositionSelect()
        {
            m_Center = GameManager.GetPlayerTransform().position;
            SetRadiousSpheres();
        }
        public static void SetContainerContent()
        {
            if (!string.IsNullOrEmpty(m_LastObjectGUID))
            {
                string ContainerData = MPSaveManager.LoadContainer(MyMod.level_guid, m_LastObjectGUID);

                if (!string.IsNullOrEmpty(ContainerData))
                {
                    MyMod.ExpeditionEditorUI.transform.GetChild(31).GetChild(4).GetChild(1).gameObject.GetComponent<UnityEngine.UI.InputField>().SetText(ContainerData);
                    UpdateObjectSettings();
                }
            }
        }

        public static void SaveExpeditionTemplate()
        {
            int Type = MyMod.ExpeditionEditorUI.transform.GetChild(3).GetComponent<UnityEngine.UI.Dropdown>().m_Value;

            MPSaveManager.CreateFolderIfNotExist("Mods");
            MPSaveManager.CreateFolderIfNotExist(@"Mods\ExpeditionTemplates");
            string Alias = MyMod.ExpeditionEditorUI.transform.GetChild(9).GetComponent<UnityEngine.UI.InputField>().text;

            ExpeditionTaskTemplate Task = new ExpeditionTaskTemplate();
            Task.m_TaskType = (ExpeditionTaskType)Type;
            Task.m_Alias = Alias;
            Task.m_RegionBelong = MyMod.ExpeditionEditorUI.transform.GetChild(1).GetComponent<UnityEngine.UI.Dropdown>().m_Value-Shared.GameRegionNegativeOffset;
            Task.m_SceneName = MyMod.ExpeditionEditorUI.transform.GetChild(4).GetComponent<UnityEngine.UI.InputField>().text;
            Task.m_TaskText = MyMod.ExpeditionEditorUI.transform.GetChild(10).GetComponent<UnityEngine.UI.InputField>().text;
            Task.m_GearSpawners = m_Spawns;
            Task.m_Containers = m_Containers;
            Task.m_ZoneCenter = m_Center;
            Task.m_ZoneRadius = MyMod.ExpeditionEditorUI.transform.GetChild(6).gameObject.GetComponent<UnityEngine.UI.Slider>().m_Value;
            Task.m_RestockSceneContainers = MyMod.ExpeditionEditorUI.transform.GetChild(17).GetComponent<UnityEngine.UI.Toggle>().isOn;
            Task.m_CanBeTaken = MyMod.ExpeditionEditorUI.transform.GetChild(18).GetComponent<UnityEngine.UI.Toggle>().isOn;
            Task.m_NextTaskAlias = MyMod.ExpeditionEditorUI.transform.GetChild(19).GetComponent<UnityEngine.UI.InputField>().text;
            Task.m_ObjectiveGearGUID = MyMod.ExpeditionEditorUI.transform.GetChild(20).GetComponent<UnityEngine.UI.InputField>().text;
            Task.m_CompleatOrder = (ExpeditionCompleteOrder)MyMod.ExpeditionEditorUI.transform.GetChild(21).GetComponent<UnityEngine.UI.Dropdown>().m_Value;
            Task.m_RandomTasksAmout = int.Parse(MyMod.ExpeditionEditorUI.transform.GetChild(22).GetComponent<UnityEngine.UI.InputField>().text);
            Task.m_Time = int.Parse(MyMod.ExpeditionEditorUI.transform.GetChild(23).GetComponent<UnityEngine.UI.InputField>().text);
            Task.m_TimeAdd = MyMod.ExpeditionEditorUI.transform.GetChild(24).GetComponent<UnityEngine.UI.Dropdown>().m_Value == 0;
            Task.m_Plants = m_Plants;
            Task.m_Breakdowns = m_Breakdowns;
            Task.m_StaySeconds = int.Parse(MyMod.ExpeditionEditorUI.transform.GetChild(29).GetComponent<UnityEngine.UI.InputField>().text);
            Task.m_Objects = m_Objects;

            MPSaveManager.SaveData(Alias + ".json", JSON.Dump(Task), 0, @"Mods\ExpeditionTemplates\" + Alias + ".json");
        }
        public static void AddContainer(string GUID)
        {
            if (!m_Containers.Contains(GUID))
            {
                m_Containers.Add(GUID);
                RecontructContainersList();
                HUDMessage.AddMessage("Added Container " + GUID);
            } else
            {
                HUDMessage.AddMessage("Container " + GUID + " is already added!");
            }
        }
        public static void AddPlant(string GUID)
        {
            if (!m_Plants.Contains(GUID))
            {
                m_Plants.Add(GUID);
                RecontructHarvestablesList();
                HUDMessage.AddMessage("Added Harvestable " + GUID);
            } else
            {
                HUDMessage.AddMessage("Harvestable " + GUID + " is already added!");
            }
        }
        public static void AddBreakdown(string GUID)
        {
            if (!m_Breakdowns.Contains(GUID))
            {
                m_Breakdowns.Add(GUID);
                RecontructBreakdownsList();
                HUDMessage.AddMessage("Added Breakdown " + GUID);
            } else
            {
                HUDMessage.AddMessage("Breakdown " + GUID + " is already added!");
            }
        }

        public static void RemoveContainerFromList(GameObject Element)
        {
            m_Containers.Remove(Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text);
            RecontructContainersList();
        }
        public static void RemoveHarvestableFromList(GameObject Element)
        {
            m_Plants.Remove(Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text);
            RecontructHarvestablesList();
        }
        public static void RemoveBreakdownFromList(GameObject Element)
        {
            m_Breakdowns.Remove(Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text);
            RecontructBreakdownsList();
        }
        public static void ClearSelectedGear()
        {
            MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = "GUID:";
            MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(3).gameObject.GetComponent<UnityEngine.UI.Slider>().Set(0, false);
            MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(4).gameObject.GetComponent<UnityEngine.UI.Dropdown>().ClearOptions();
        }
        public static void ClearSelectedObject()
        {
            MyMod.ExpeditionEditorUI.transform.GetChild(31).GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = "GUID:";
        }

        public static void UpdateExtra()
        {
            for (int i = m_Spawns.Count - 1; i >= 0; i--)
            {
                if (m_Spawns[i].m_GUID == m_LastSpawnerGUID)
                {
                    m_Spawns[i].m_Extra.m_Dropper = MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(7).GetChild(1).GetComponent<UnityEngine.UI.InputField>().text;
                    m_Spawns[i].m_Extra.m_PhotoGUID = MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(7).GetChild(2).GetComponent<UnityEngine.UI.InputField>().text;
                    m_Spawns[i].m_Extra.m_ExpeditionNote = MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(7).GetChild(4).GetComponent<UnityEngine.UI.InputField>().text;
                    return;
                }
            }
        }

        public static void UpdateObjectSettings()
        {
            for (int i = m_Objects.Count - 1; i >= 0; i--)
            {
                if (m_Objects[i].m_GUID == m_LastObjectGUID)
                {
                    m_Objects[i].m_Content = MyMod.ExpeditionEditorUI.transform.GetChild(31).GetChild(4).GetChild(1).gameObject.GetComponent<UnityEngine.UI.InputField>().text;
                    return;
                }
            }
        }


        public static void UpdateGear()
        {
            ClearSelectedGear();
            if (!string.IsNullOrEmpty(m_LastSpawnerGUID))
            {
                for (int i = m_Spawns.Count - 1; i >= 0; i--)
                {
                    if (m_Spawns[i].m_GUID == m_LastSpawnerGUID)
                    {
                        MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = "GUID: "+ m_Spawns[i].m_GUID;
                        MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(3).gameObject.GetComponent<UnityEngine.UI.Slider>().Set(m_Spawns[i].m_Chance, false);
                        m_LastChance = m_Spawns[i].m_Chance;
                        UnityEngine.UI.Dropdown Drop = MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(4).gameObject.GetComponent<UnityEngine.UI.Dropdown>();
                        for (int i2 = 0; i2 < m_Spawns[i].m_GearsVariant.Count; i2++)
                        {
                            Drop.options.Add(new UnityEngine.UI.Dropdown.OptionData(m_Spawns[i].m_GearsVariant[i2]));
                        }

                        if(m_Spawns[i].m_GearsVariant.Count == 1)
                        {
                            Drop.Set(1);
                        } else
                        {
                            Drop.Set(m_Spawns[i].m_GearsVariant.Count - 1);
                        }

                        MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(7).GetChild(1).GetComponent<UnityEngine.UI.InputField>().SetText(m_Spawns[i].m_Extra.m_Dropper);
                        MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(7).GetChild(2).GetComponent<UnityEngine.UI.InputField>().SetText(m_Spawns[i].m_Extra.m_PhotoGUID);
                        MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(7).GetChild(4).GetComponent<UnityEngine.UI.InputField>().SetText(m_Spawns[i].m_Extra.m_ExpeditionNote);

                        return;
                    }
                }
            }
        }

        public static void UpdateObject()
        {
            ClearSelectedObject();
            if (!string.IsNullOrEmpty(m_LastObjectGUID))
            {
                for (int i = m_Objects.Count - 1; i >= 0; i--)
                {
                    if (m_Objects[i].m_GUID == m_LastObjectGUID)
                    {
                        MyMod.ExpeditionEditorUI.transform.GetChild(31).GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = "GUID: " + m_LastObjectGUID;
                        MyMod.ExpeditionEditorUI.transform.GetChild(31).GetChild(4).GetChild(1).gameObject.GetComponent<UnityEngine.UI.InputField>().SetText(m_Objects[i].m_Content);
                        return;
                    }
                }
            }
        }

        public static void RemoveObjectFromList(GameObject Element)
        {
            string GUID = Element.GetComponent<LocalVariablesKit>().m_String;
            for (int i = m_Objects.Count - 1; i >= 0; i--)
            {
                if (m_Objects[i].m_GUID == GUID)
                {
                    if (m_LastObjectGUID == GUID)
                    {
                        ClearSelectedObject();
                    }
                    m_Objects.RemoveAt(i);
                    RecontructObjectsList();
                    VisualizeObjects();
                    return;
                }
            }
        }
        public static void SelectObjectFromList(GameObject Element)
        {
            string GUID = Element.GetComponent<LocalVariablesKit>().m_String;
            for (int i = m_Objects.Count - 1; i >= 0; i--)
            {
                if (m_Objects[i].m_GUID == GUID)
                {
                    UpdateObjectSettings();
                    m_LastObjectGUID = GUID;
                    UpdateObject();
                    return;
                }
            }
        }


        public static void RemoveGearFromList(GameObject Element)
        {
            string GUID = Element.GetComponent<LocalVariablesKit>().m_String;

            DebugLog("RemoveGearFromList " + GUID);
            for (int i = m_Spawns.Count - 1; i >= 0; i--)
            {
                if (m_Spawns[i].m_GUID == GUID)
                {
                    if(m_LastSpawnerGUID == GUID)
                    {
                        ClearSelectedGear();
                    }
                    m_Spawns.RemoveAt(i);
                    RecontructGearsList();
                    return;
                }
            }
        }
        public static void SelectGearFromList(GameObject Element)
        {
            string GUID = Element.GetComponent<LocalVariablesKit>().m_String;

            DebugLog("SelectGearFromList "+ GUID);
            for (int i = m_Spawns.Count - 1; i >= 0; i--)
            {
                if (m_Spawns[i].m_GUID == GUID)
                {
                    UpdateExtra();
                    m_LastSpawnerGUID = GUID;
                    UpdateGear();
                    return;
                }
            }
        }

        public static void RecontructContainersList()
        {
            GameObject Content = MyMod.ExpeditionEditorUI.transform.GetChild(13).GetChild(1).GetChild(0).GetChild(0).gameObject;

            for (int i = Content.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(Content.transform.GetChild(i).gameObject);
            }
            GameObject LoadedAssets = MyMod.LoadedBundle.LoadAsset<GameObject>("MP_ContainerElement");
            foreach (string GUID in m_Containers)
            {
                GameObject Element = GameObject.Instantiate(LoadedAssets, Content.transform);
                Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = GUID;
                Action act = new Action(() => RemoveContainerFromList(Element));
                Element.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);
            }
        }

        public static void RecontructHarvestablesList()
        {
            GameObject Content = MyMod.ExpeditionEditorUI.transform.GetChild(27).GetChild(1).GetChild(0).GetChild(0).gameObject;

            for (int i = Content.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(Content.transform.GetChild(i).gameObject);
            }
            GameObject LoadedAssets = MyMod.LoadedBundle.LoadAsset<GameObject>("MP_ContainerElement");
            foreach (string GUID in m_Plants)
            {
                GameObject Element = GameObject.Instantiate(LoadedAssets, Content.transform);
                Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = GUID;
                Action act = new Action(() => RemoveHarvestableFromList(Element));
                Element.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);
            }
        }
        public static void RecontructBreakdownsList()
        {
            GameObject Content = MyMod.ExpeditionEditorUI.transform.GetChild(28).GetChild(1).GetChild(0).GetChild(0).gameObject;

            for (int i = Content.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(Content.transform.GetChild(i).gameObject);
            }
            GameObject LoadedAssets = MyMod.LoadedBundle.LoadAsset<GameObject>("MP_ContainerElement");
            foreach (string GUID in m_Breakdowns)
            {
                GameObject Element = GameObject.Instantiate(LoadedAssets, Content.transform);
                Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = GUID;
                Action act = new Action(() => RemoveBreakdownFromList(Element));
                Element.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);
            }
        }

        public static void RecontructObjectsList()
        {
            GameObject Content = MyMod.ExpeditionEditorUI.transform.GetChild(31).GetChild(1).GetChild(0).GetChild(0).gameObject;

            for (int i = Content.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(Content.transform.GetChild(i).gameObject);
            }
            GameObject LoadedAssets = MyMod.LoadedBundle.LoadAsset<GameObject>("MP_GearElement");
            foreach (UniversalSyncableObjectSpawner Object in m_Objects)
            {
                GameObject Element = GameObject.Instantiate(LoadedAssets, Content.transform);
                Element.AddComponent<LocalVariablesKit>().m_String = Object.m_GUID;
                Element.transform.GetChild(0).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = Object.m_Prefab;
                Action act = new Action(() => RemoveObjectFromList(Element));
                Element.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);
                Action act2 = new Action(() => SelectObjectFromList(Element));
                Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act2);
            }
        }


        public static void AddGear(string GearPrefab, Vector3 Pos, Quaternion Rot, string PHOTOGUID)
        {
            ExpeditionGearSpawner Spawner = new ExpeditionGearSpawner();
            Spawner.m_GUID = Guid.NewGuid().ToString();
            Spawner.m_Chance = 0.33f;
            Spawner.m_GearsVariant.Add(GearPrefab);
            Spawner.m_Possition = Pos;
            Spawner.m_Rotation = Rot;
            Spawner.m_Extra.m_PhotoGUID = PHOTOGUID;
            m_Spawns.Add(Spawner);

            if (!string.IsNullOrEmpty(PHOTOGUID))
            {
                MPSaveManager.CopyPhotoToExpeditionCache(PHOTOGUID);
            }

            HUDMessage.AddMessage("Added Gear " + Spawner.m_GUID);
            RecontructGearsList();
            UpdateExtra();
            m_LastSpawnerGUID = Spawner.m_GUID;
            UpdateGear();
        }

        public static void AddGearVariant(string GearPrefab)
        {
            if (!string.IsNullOrEmpty(m_LastSpawnerGUID))
            {
                for (int i = m_Spawns.Count - 1; i >= 0; i--)
                {
                    if (m_Spawns[i].m_GUID == m_LastSpawnerGUID)
                    {

                        if (!m_Spawns[i].m_GearsVariant.Contains(GearPrefab))
                        {
                            m_Spawns[i].m_GearsVariant.Add(GearPrefab);
                            HUDMessage.AddMessage("Added" + GearPrefab + " Variant");
                        } else
                        {
                            HUDMessage.AddMessage(GearPrefab+" Already Added!");
                        }
                        UpdateGear();
                        return;
                    }
                }
            }
        }
        public static void RemoveGearVariant()
        {
            if (!string.IsNullOrEmpty(m_LastSpawnerGUID))
            {
                UnityEngine.UI.Dropdown Drop = MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(4).gameObject.GetComponent<UnityEngine.UI.Dropdown>();
                

                for (int i = m_Spawns.Count - 1; i >= 0; i--)
                {
                    if (m_Spawns[i].m_GUID == m_LastSpawnerGUID)
                    {
                        if(m_Spawns[i].m_GearsVariant.Count > 1)
                        {
                            m_Spawns[i].m_GearsVariant.RemoveAt(Drop.m_Value);
                        }
                        UpdateGear();
                        return;
                    }
                }
            }
        }

        public static void ChangeChance()
        {
            if (!string.IsNullOrEmpty(m_LastSpawnerGUID))
            {
                for (int i = m_Spawns.Count - 1; i >= 0; i--)
                {
                    if (m_Spawns[i].m_GUID == m_LastSpawnerGUID)
                    {
                        m_Spawns[i].m_Chance = MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(3).gameObject.GetComponent<UnityEngine.UI.Slider>().value;
                        return;
                    }
                }
            }
        }

        public static void RecontructGearsList()
        {
            GameObject Content = MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(1).GetChild(0).GetChild(0).gameObject;

            for (int i = Content.transform.childCount-1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(Content.transform.GetChild(i).gameObject);
            }
            GameObject LoadedAssets = MyMod.LoadedBundle.LoadAsset<GameObject>("MP_GearElement");
            foreach (ExpeditionGearSpawner Spawner in m_Spawns)
            {
                GameObject Element = GameObject.Instantiate(LoadedAssets, Content.transform);
                Element.AddComponent<LocalVariablesKit>().m_String = Spawner.m_GUID;
                Element.transform.GetChild(0).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = Spawner.m_GearsVariant[0];
                Action act = new Action(() => RemoveGearFromList(Element));
                Element.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);
                Action act2 = new Action(() => SelectGearFromList(Element));
                Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act2);
            }
        }



        public static void LoadExpedition(ExpeditionTaskTemplate Data)
        {
            MyMod.ExpeditionEditorUI.transform.GetChild(1).GetComponent<UnityEngine.UI.Dropdown>().Set(Data.m_RegionBelong+ Shared.GameRegionNegativeOffset);
            MyMod.ExpeditionEditorUI.transform.GetChild(3).GetComponent<UnityEngine.UI.Dropdown>().Set((int)Data.m_TaskType);
            MyMod.ExpeditionEditorUI.transform.GetChild(4).GetComponent<UnityEngine.UI.InputField>().SetText(Data.m_SceneName);
            MyMod.ExpeditionEditorUI.transform.GetChild(6).GetComponent<UnityEngine.UI.Slider>().Set(Data.m_ZoneRadius);
            m_Center = Data.m_ZoneCenter;
            MyMod.ExpeditionEditorUI.transform.GetChild(9).GetComponent<UnityEngine.UI.InputField>().SetText(Data.m_Alias);
            MyMod.ExpeditionEditorUI.transform.GetChild(10).GetComponent<UnityEngine.UI.InputField>().SetText(Data.m_TaskText);
            MyMod.ExpeditionEditorUI.transform.GetChild(18).GetComponent<UnityEngine.UI.Toggle>().Set(Data.m_CanBeTaken);
            m_Spawns = Data.m_GearSpawners;
            m_Containers = Data.m_Containers;
            MyMod.ExpeditionEditorUI.transform.GetChild(20).GetComponent<UnityEngine.UI.InputField>().SetText(Data.m_ObjectiveGearGUID);
            MyMod.ExpeditionEditorUI.transform.GetChild(19).GetComponent<UnityEngine.UI.InputField>().SetText(Data.m_NextTaskAlias);
            MyMod.ExpeditionEditorUI.transform.GetChild(21).GetComponent<UnityEngine.UI.Dropdown>().Set((int)Data.m_CompleatOrder);
            MyMod.ExpeditionEditorUI.transform.GetChild(22).GetComponent<UnityEngine.UI.InputField>().SetText(Data.m_RandomTasksAmout.ToString());
            MyMod.ExpeditionEditorUI.transform.GetChild(23).GetComponent<UnityEngine.UI.InputField>().SetText(Data.m_Time.ToString());
            MyMod.ExpeditionEditorUI.transform.GetChild(29).GetComponent<UnityEngine.UI.InputField>().SetText(Data.m_StaySeconds.ToString());
            int TimeType = 0;
            if (!Data.m_TimeAdd)
            {
                TimeType = 1;
            }
            MyMod.ExpeditionEditorUI.transform.GetChild(24).GetComponent<UnityEngine.UI.Dropdown>().Set(TimeType);
            m_Plants = Data.m_Plants;
            m_Breakdowns = Data.m_Breakdowns;
            m_Objects = Data.m_Objects;

            RecontructContainersList();
            RecontructGearsList();
            RecontructHarvestablesList();
            RecontructBreakdownsList();
            RecontructObjectsList();
            UpdateGear();
            UpdateObject();

            SetRadiousSpheres();
            

            MyMod.ExpeditionEditorSelectUI.SetActive(false);
            MyMod.ExpeditionEditorUI.SetActive(true);
        }

        public static void SelectExpedition(GameObject Element)
        {
            LocalVariablesKit Kit = Element.GetComponent<LocalVariablesKit>();
            string Alias = Kit.m_String;
            string JSONString = GetExpeditionJsonByAlias(Alias);
            LoadExpedition(JSON.Load(JSONString).Make<ExpeditionTaskTemplate>());
        }
        public static void SelectRegion(GameObject Element)
        {
            LocalVariablesKit Kit = Element.GetComponent<LocalVariablesKit>();
            int Region = Kit.m_Int;
            LastSelectedRegion = Region;
            RefreshExpeditionsList(Region);
        }
        public static void TestExpedition(GameObject Element)
        {
            string FileName = Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text;
            string Alias = FileName.Replace(".json", "");
            string MyMac = MPSaveManager.GetSubNetworkGUID();
            for (int i = m_ActiveExpeditions.Count-1; i >= 0; i--)
            {
                if (m_ActiveExpeditions[i].m_Players.Contains(MyMac))
                {
                    CompleteExpedition(m_ActiveExpeditions[i].m_GUID, 0);
                }
            }
            StartNewExpedition(MyMac, 0, Alias);
            DisableRadiousSpheres();
        }

        public static void DisableVisualizeObjects()
        {
            for (int i = m_VisualizeObjects.Count - 1; i >= 0; i--)
            {
                if(m_VisualizeObjects[i] != null && m_VisualizeObjects[i].GetComponent<ObjectGuid>())
                {
                    bool Found = false;
                    for (int i2 = 0; i2 < m_Objects.Count; i2++)
                    {
                        if (m_Objects[i2].m_GUID == m_VisualizeObjects[i].GetComponent<ObjectGuid>().Get())
                        {
                            Found = true;
                            break;
                        }
                    }
                    if (!Found)
                    {
                        UnityEngine.Object.Destroy(m_VisualizeObjects[i]);
                        m_VisualizeObjects.RemoveAt(i);
                    }
                }
            }
        }

        public static void VisualizeObjects()
        {
            DisableVisualizeObjects();

            foreach (UniversalSyncableObjectSpawner Spawner in m_Objects)
            {
                UniversalSyncableObject Obj = new UniversalSyncableObject();
                Obj.m_Prefab = Spawner.m_Prefab;
                Obj.m_GUID = Spawner.m_GUID;
                Obj.m_Position = Spawner.m_Position;
                Obj.m_Rotation = Spawner.m_Rotation;

                MPSaveManager.RemoveContainer(MyMod.level_guid, Spawner.m_GUID);
                if (!string.IsNullOrEmpty(Spawner.m_Content))
                {
                    MPSaveManager.SaveContainer(MyMod.level_guid, Spawner.m_GUID, Spawner.m_Content);
                    MPSaveManager.SetConstainerState(MyMod.level_guid, Spawner.m_GUID, 1);
                } else
                {
                    MPSaveManager.SetConstainerState(MyMod.level_guid, Spawner.m_GUID, 2);
                }

                Obj.m_ExpeditionBelong = "";
                Obj.m_Scene = MyMod.level_guid;
                Obj.m_CreationTime = MyMod.MinutesFromStartServer;
                GameObject newObject = MyMod.SpawnUniversalSyncableObject(Obj);
                if (newObject)
                {
                    m_VisualizeObjects.Add(newObject);
                }
            }
        }

        public static void ToggleColisionOfObject(GameObject obj, bool Status)
        {
            List<GameObject> list = new List<GameObject>();
            list.Add(obj);
            Transform Parent = obj.transform;
            foreach (Transform child in Parent.GetComponentsInChildren<Transform>())
            {
                list.Add(child.gameObject);
            }

            foreach (GameObject obji in list)
            {
                foreach (BoxCollider Com in obji.GetComponents<BoxCollider>())
                {
                    Com.enabled = Status;

                }
                foreach (SphereCollider Com in obji.GetComponents<SphereCollider>())
                {
                    Com.enabled = Status;
                }
                foreach (MeshCollider Com in obji.GetComponents<MeshCollider>())
                {
                    Com.enabled = Status;
                }
                foreach (Rigidbody Com in obji.GetComponents<Rigidbody>())
                {
                    Com.isKinematic = true;
                }
            }
        }

        public static void PlaceVisualizedObject()
        {
            string GUID = m_LastObjectGUID;
            GameObject Object = PdidTable.GetGameObject(GUID);
            if(Object)
            {
                GameManager.GetPlayerManagerComponent().StartPlaceMesh(Object, PlaceMeshFlags.None);
                ToggleColisionOfObject(Object, false);
            }
        }

        public static void AddObject()
        {
            string Prefab = MyMod.ExpeditionEditorUI.transform.GetChild(31).GetChild(5).GetComponent<UnityEngine.UI.InputField>().text;
            UniversalSyncableObjectSpawner Spawner = new UniversalSyncableObjectSpawner();
            Spawner.m_Prefab = Prefab;
            Spawner.m_Position = GameManager.GetPlayerTransform().transform.position;
            Spawner.m_Rotation = GameManager.GetPlayerTransform().transform.rotation;
            Spawner.m_GUID = ObjectGuidManager.GenerateNewGuidString();
            m_Objects.Add(Spawner);
            m_LastObjectGUID = Spawner.m_GUID;
            VisualizeObjects();
            RecontructObjectsList();
            UpdateObject();
            UpdateObjectSettings();
        }

        public static bool LastOnlyShowFirstTasksState = false;

        public static void RefreshExpeditionsList(int RegionMode = 6)
        {

            MPSaveManager.CreateFolderIfNotExist("Mods");
            MPSaveManager.CreateFolderIfNotExist("Mods"+Seperator+ "ExpeditionTemplates");

            DirectoryInfo d = new DirectoryInfo("Mods" + Seperator + "ExpeditionTemplates");

            ExpeditionBuilder.Init(true);
            List<string> Names = new List<string>();
            List<int> RegionBelong = new List<int>();

            foreach (var item in m_ExpeditionTasks)
            {
                int Region = item.Key;
                if(RegionMode == 6)
                {
                    Names.Add(GetRegionString(Region));
                    RegionBelong.Add(Region);
                }else if(Region == RegionMode)
                {
                    foreach (ExpeditionTaskTemplate task in item.Value)
                    {
                        if (!LastOnlyShowFirstTasksState || task.m_CanBeTaken)
                        {
                            Names.Add(task.m_Alias);
                            RegionBelong.Add(task.m_RegionBelong);
                        }
                    }
                }
            }

            MyMod.ExpeditionEditorSelectUI.transform.GetChild(3).gameObject.SetActive(RegionMode == 6);
            MyMod.ExpeditionEditorSelectUI.transform.GetChild(4).gameObject.SetActive(RegionMode != 6);
            MyMod.ExpeditionEditorSelectUI.transform.GetChild(5).gameObject.SetActive(RegionMode != 6);

            GameObject Content = MyMod.ExpeditionEditorSelectUI.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;

            for (int i = Content.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(Content.transform.GetChild(i).gameObject);
            }
            GameObject LoadedAssets = MyMod.LoadedBundle.LoadAsset<GameObject>("MP_ExpeditionElement");

            for (int i = 0; i < Names.Count; i++)
            {
                string Name = Names[i];
                int Region = RegionBelong[i];

                GameObject Element = GameObject.Instantiate(LoadedAssets, Content.transform);
                LocalVariablesKit Kit = Element.AddComponent<LocalVariablesKit>();
                Kit.m_Int = Region;
                Kit.m_String = Name;
                Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = Name;

                if(RegionMode != 6)
                {
                    Action act = new Action(() => SelectExpedition(Element));
                    Element.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);
                    Action act2 = new Action(() => TestExpedition(Element));
                    Element.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act2);
                } else
                {
                    Action act = new Action(() => SelectRegion(Element));
                    Element.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);
                    Element.transform.GetChild(1).gameObject.SetActive(false);
                    Element.transform.GetChild(2).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Open";
                }
            }
        }
    }
}
