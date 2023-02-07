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

namespace SkyCoop
{
    public class ExpeditionEditor
    {
        public static List<ExpeditionGearSpawner> m_Spawns = new List<ExpeditionGearSpawner>();
        public static Vector3 m_Center = new Vector3(0, 0, 0);
        public static List<string> m_Containers = new List<string>();
        public static string m_LastSpawnerGUID = "";
        public static float m_LastChance = 0;

#if (DEDICATED_LINUX)
        public static string Seperator = @"/";
#else
        public static string Seperator = @"\";
#endif

        public class ExpeditionTaskTemplate
        {
            public ExpeditionTaskType m_TaskType = ExpeditionTaskType.ENTERSCENE;

        }

        public static void BackToSelect()
        {
            RefreshExpeditionsList();
            MyMod.ExpeditionEditorUI.transform.GetChild(14).gameObject.SetActive(false);
            MyMod.ExpeditionEditorUI.transform.GetChild(13).gameObject.SetActive(false);
            MyMod.ExpeditionEditorUI.SetActive(false);
            MyMod.ExpeditionEditorSelectUI.SetActive(true);
        }

        public static void SaveExpeditionTemplate()
        {
            int Type = MyMod.ExpeditionEditorUI.transform.GetChild(3).GetComponent<UnityEngine.UI.Dropdown>().m_Value;

            MPSaveManager.CreateFolderIfNotExist("Mods");
            MPSaveManager.CreateFolderIfNotExist(@"Mods\ExpeditionTemplates");
            string Alias = MyMod.ExpeditionEditorUI.transform.GetChild(9).GetComponent<UnityEngine.UI.InputField>().text;
            if (Type == 0)
            {
                ExpeditionRewardScene RewardScene = new ExpeditionRewardScene();
                RewardScene.m_SceneName = MyMod.ExpeditionEditorUI.transform.GetChild(4).GetComponent<UnityEngine.UI.InputField>().text;
                RewardScene.m_Task = MyMod.ExpeditionEditorUI.transform.GetChild(10).GetComponent<UnityEngine.UI.InputField>().text;
                RewardScene.m_GearSpawners = m_Spawns;
                RewardScene.m_BelongRegion = MyMod.ExpeditionEditorUI.transform.GetChild(1).GetComponent<UnityEngine.UI.Dropdown>().m_Value;
                MPSaveManager.SaveData(Alias + ".json", JSON.Dump(RewardScene), 0, @"Mods\ExpeditionTemplates\" + Alias + ".json");
            } else if(Type == 1)
            {
                ExpeditionZone Zone = new ExpeditionZone();
                Zone.m_ZoneScene = MyMod.ExpeditionEditorUI.transform.GetChild(4).GetComponent<UnityEngine.UI.InputField>().text;
                Zone.m_ZoneTaskText = MyMod.ExpeditionEditorUI.transform.GetChild(10).GetComponent<UnityEngine.UI.InputField>().text;
                Zone.m_ZoneCenter = m_Center;
                Zone.m_ZoneRadius = MyMod.ExpeditionEditorUI.transform.GetChild(6).gameObject.GetComponent<UnityEngine.UI.Slider>().m_Value;
                Zone.m_BelongRegion = MyMod.ExpeditionEditorUI.transform.GetChild(1).GetComponent<UnityEngine.UI.Dropdown>().m_Value;
                Zone.m_ZoneContainers = m_Containers;
                Zone.m_ZoneGearSpawners = m_Spawns;

                MPSaveManager.SaveData(Alias + ".json", JSON.Dump(Zone), 0, @"Mods\ExpeditionTemplates\" + Alias + ".json");
            }
        }
        public static void AddContainer(string GUID)
        {
            int Type = MyMod.ExpeditionEditorUI.transform.GetChild(3).GetComponent<UnityEngine.UI.Dropdown>().m_Value;
            if (Type == 1)
            {
                if (!m_Containers.Contains(GUID))
                {
                    m_Containers.Add(GUID);
                    RecontructContainersList();
                    HUDMessage.AddMessage("Added Container "+GUID);
                } else
                {
                    HUDMessage.AddMessage("Container " + GUID+" is already added!");

                }
            }
        }

        public static void RemoveContainerFromList(GameObject Element)
        {
            m_Containers.Remove(Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text);
            RecontructContainersList();
        }

        public static void ClearSelectedGear()
        {
            MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = "GUID:";
            MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(3).gameObject.GetComponent<UnityEngine.UI.Slider>().Set(0, false);
            MyMod.ExpeditionEditorUI.transform.GetChild(14).GetChild(4).gameObject.GetComponent<UnityEngine.UI.Dropdown>().ClearOptions();
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
                        return;
                    }
                }
            }

        }


        public static void RemoveGearFromList(GameObject Element)
        {
            string GUID = Element.transform.GetChild(0).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text;
            for (int i = m_Spawns.Count - 1; i >= 0; i--)
            {
                if (m_Spawns[i].m_GUID == GUID)
                {
                    if(m_LastSpawnerGUID == GUID)
                    {
                        ClearSelectedGear();
                    }
                    m_Spawns.RemoveAt(i);
                    RecontructContainersList();
                    return;
                }
            }
        }
        public static void SelectGearFromList(GameObject Element)
        {
            string GUID = Element.transform.GetChild(0).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text;
            for (int i = m_Spawns.Count - 1; i >= 0; i--)
            {
                if (m_Spawns[i].m_GUID == GUID)
                {
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

        public static void AddGear(string GearPrefab, Vector3 Pos, Quaternion Rot)
        {
            ExpeditionGearSpawner Spawner = new ExpeditionGearSpawner();
            Spawner.m_GUID = Guid.NewGuid().ToString();
            Spawner.m_Chance = 0.33f;
            Spawner.m_GearsVariant.Add(GearPrefab);
            Spawner.m_Possition = Pos;
            Spawner.m_Rotation = Rot;
            m_Spawns.Add(Spawner);
            HUDMessage.AddMessage("Added Gear " + Spawner.m_GUID);
            RecontructGearsList();
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
                Element.transform.GetChild(0).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = Spawner.m_GUID;
                Action act = new Action(() => RemoveGearFromList(Element));
                Element.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);
                Action act2 = new Action(() => SelectGearFromList(Element));
                Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act2);
            }
        }

        public static void LoadExpedition(ExpeditionZone Data, string Alias)
        {
            MyMod.ExpeditionEditorUI.transform.GetChild(1).GetComponent<UnityEngine.UI.Dropdown>().Set(Data.m_BelongRegion);
            MyMod.ExpeditionEditorUI.transform.GetChild(3).GetComponent<UnityEngine.UI.Dropdown>().Set(1);
            MyMod.ExpeditionEditorUI.transform.GetChild(4).GetComponent<UnityEngine.UI.InputField>().SetText(Data.m_ZoneScene);
            MyMod.ExpeditionEditorUI.transform.GetChild(6).GetComponent<UnityEngine.UI.Slider>().Set(Data.m_ZoneRadius);
            m_Center = Data.m_ZoneCenter;
            MyMod.ExpeditionEditorUI.transform.GetChild(9).GetComponent<UnityEngine.UI.InputField>().SetText(Alias);
            MyMod.ExpeditionEditorUI.transform.GetChild(10).GetComponent<UnityEngine.UI.InputField>().SetText(Data.m_ZoneTaskText);
            m_Spawns = Data.m_ZoneGearSpawners;
            m_Containers = Data.m_ZoneContainers;

            RecontructContainersList();
            RecontructGearsList();
            UpdateGear();

            MyMod.ExpeditionEditorSelectUI.SetActive(false);
            MyMod.ExpeditionEditorUI.SetActive(true);
        }
        public static void LoadExpedition(ExpeditionRewardScene Data, string Alias)
        {
            MyMod.ExpeditionEditorUI.transform.GetChild(1).GetComponent<UnityEngine.UI.Dropdown>().Set(Data.m_BelongRegion);
            MyMod.ExpeditionEditorUI.transform.GetChild(3).GetComponent<UnityEngine.UI.Dropdown>().Set(0);
            MyMod.ExpeditionEditorUI.transform.GetChild(4).GetComponent<UnityEngine.UI.InputField>().SetText(Data.m_SceneName);
            MyMod.ExpeditionEditorUI.transform.GetChild(6).GetComponent<UnityEngine.UI.Slider>().Set(0);
            m_Center = new Vector3(0,0,0);
            MyMod.ExpeditionEditorUI.transform.GetChild(9).GetComponent<UnityEngine.UI.InputField>().SetText(Alias);
            MyMod.ExpeditionEditorUI.transform.GetChild(10).GetComponent<UnityEngine.UI.InputField>().SetText(Data.m_Task);
            m_Spawns = Data.m_GearSpawners;
            m_Containers = new List<string>();

            RecontructContainersList();
            RecontructGearsList();
            UpdateGear();

            MyMod.ExpeditionEditorSelectUI.SetActive(false);
            MyMod.ExpeditionEditorUI.SetActive(true);
        }

        public static void SelectExpedition(GameObject Element)
        {
            string FileName = Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text;
            string Alias = FileName.Replace(".json", "");
            string JSONString = GetExpeditionJsonByAlias(Alias);

            bool ThisIsZone = ThisIsJsonOfZone(JSONString);

            if (ThisIsZone)
            {
                LoadExpedition(JSON.Load(JSONString).Make<ExpeditionZone>(), Alias);
            } else
            {
                LoadExpedition(JSON.Load(JSONString).Make<ExpeditionRewardScene>(), Alias);
            }
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
                    CompleteExpedition(m_ActiveExpeditions[i].m_GUID, false);
                }
            }
            StartNewExpedition(MyMac, 0, Alias);
        }

        public static void RefreshExpeditionsList()
        {

            MPSaveManager.CreateFolderIfNotExist("Mods");
            MPSaveManager.CreateFolderIfNotExist("Mods"+Seperator+ "ExpeditionTemplates");

            DirectoryInfo d = new DirectoryInfo("Mods" + Seperator + "ExpeditionTemplates");
            FileInfo[] Files = d.GetFiles("*.json");
            List<string> Names = new List<string>();
            foreach (FileInfo file in Files)
            {
                byte[] FileData = File.ReadAllBytes(file.FullName);
                
                string JSONString = UTF8Encoding.UTF8.GetString(FileData);
                if (string.IsNullOrEmpty(JSONString))
                {
                    continue;
                }
                Names.Add(file.Name);
            }
            GameObject Content = MyMod.ExpeditionEditorSelectUI.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;

            for (int i = Content.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(Content.transform.GetChild(i).gameObject);
            }
            GameObject LoadedAssets = MyMod.LoadedBundle.LoadAsset<GameObject>("MP_ExpeditionElement");
            foreach (string Name in Names)
            {
                GameObject Element = GameObject.Instantiate(LoadedAssets, Content.transform);
                Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = Name;
                Action act = new Action(() => SelectExpedition(Element));
                Element.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);
                Action act2 = new Action(() => TestExpedition(Element));
                Element.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act2);
            }
        }
    }
}
