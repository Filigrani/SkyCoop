using Harmony;
using Il2Cpp;
using Il2CppTLD.AddressableAssets;
using Il2CppTLD.Gear;
using Il2CppTLD.Scenes;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using static SkyCoopServer.DataStr;
using Il2CppCollection = Il2CppSystem.Collections.Generic;

namespace SkyCoopClient
{
    public static class GearSpawnsRipper
    {
        public static DataStr.ScenesLootSpawns s_ScenesLootSpawns = new DataStr.ScenesLootSpawns();
        public static List<string> s_ScenesToSave = new List<string>();
        public static bool s_Active = false;
        public static Stopwatch s_StopWatch = null;
        //public static int m_FramesDelay = 0;
        public static string s_NameForGearsFiles = "GearSpawns";

        public static void SceneLoaded()
        {
            if (s_Active)
            {
                //m_FramesDelay = 120;
                SaveThisScene();
            }
        }

        public static void Update()
        {
            //if (s_Active)
            //{
            //    if (m_FramesDelay > 0)
            //    {
            //        m_FramesDelay--;

            //        if (m_FramesDelay == 0)
            //        {
            //            SaveThisScene();
            //        }
            //    }
            //}
        }

        public static void Start(bool ModdedOnly = false)
        {
            s_ScenesLootSpawns = new DataStr.ScenesLootSpawns();
            s_ScenesLootSpawns.Scenes = new List<DataStr.SceneLootSpawns>();

            s_StopWatch = new Stopwatch();
            s_StopWatch.Start();
            s_ScenesToSave = new List<string>();

            Il2CppCollection.List<IResourceLocation> Scenes = AssetHelper.FindAllAssetsLocations<SceneSet>().Cast<Il2CppCollection.List<IResourceLocation>>();

            foreach (IResourceLocation sceneResource in Scenes)
            {
                if(ModdedOnly && !sceneResource.PrimaryKey.ToLower().StartsWith("mod"))
                {
                    continue;
                }
                s_ScenesToSave.Add(sceneResource.PrimaryKey);
                SkyCoop.Logger.Log($"[GearSpawnsRipper] Scene added to queue {sceneResource.PrimaryKey}");
            }
            s_NameForGearsFiles = !ModdedOnly ? "GearSpawns" : "GearSpawnsModded";
            s_Active = true;
            Next();
        }

        public static void Next()
        {
            if (s_Active)
            {
                if(s_ScenesToSave.Count > 0)
                {
                    string SceneToRip = s_ScenesToSave[0];
                    s_ScenesToSave.RemoveAt(0);
                    SkyCoop.Logger.Log(ConsoleColor.Blue, $"[GearSpawnsRipper] Start loading {SceneToRip}");
                    uConsole.RunCommandSilent($"scene {SceneToRip}");
                }
                else
                {
                    s_Active = false;
                    s_StopWatch.Stop();
                    SkyCoop.Logger.Log(ConsoleColor.Green, $"[GearSpawnsRipper] Finished! It took {s_StopWatch.Elapsed}");
                    s_StopWatch = null;
                    SaveToFile();
                }
            }
        }

        public static string FixName(string Name)
        {
            if (Name.EndsWith(')'))
            {
                if(Name.Contains(' '))
                {
                    string[] Slices = Name.Split(' ');
                    return Slices[0];
                }
            }
            return Name;
        }

        public static void SaveThisScene()
        {
            if (s_Active)
            {
                DataStr.SceneLootSpawns SceneData = new DataStr.SceneLootSpawns();
                SceneData.PrefabSpawns = new List<DataStr.PrefabSpawnData>();
                SceneData.RandomSpawnObjects = new List<DataStr.RandomSpawnObjectData>();
                SceneData.LooseGearSpawns = new List<DataStr.LooseGearSpawn>();
                SceneData.SceneName = ModMain.GetCurrentSceneName();
                SceneData.RadialSpawns = new List<DataStr.RadialObjectSpawnerData>();
                SceneData.SpawnGearVariants = new List<DataStr.SpawnGearVariantData>();

                List<GameObject> GearsToIgnore = new List<GameObject>();

                foreach (PrefabSpawn PS in UnityEngine.Object.FindObjectsOfType<PrefabSpawn>(true))
                {
                    PrefabSpawnData Data = PS.GetData();
                    if(Data != null)
                    {
                        SceneData.PrefabSpawns.Add(Data);
                    }
                }

                foreach (RandomSpawnObject RSO in UnityEngine.Object.FindObjectsOfType<RandomSpawnObject>(true))
                {
                    List<GameObject> BelongToRadial = null;
                    RandomSpawnObjectData Data = RSO.GetData(out BelongToRadial);

                    if(BelongToRadial != null && BelongToRadial.Count > 0)
                    {
                        GearsToIgnore.AddRange(BelongToRadial);
                    }

                    if(Data != null)
                    {
                        SceneData.RandomSpawnObjects.Add(Data);
                    }
                }

                if(SpawnGearVariant.s_ActiveSpawners.Count > 0)
                {
                    DataStr.SpawnGearVariantData VariantBase = new SpawnGearVariantData();
                    foreach (SpawnGearVariant SGV in SpawnGearVariant.s_ActiveSpawners)
                    {
                        DataStr.SpawnGearVariantElementData ElementData = new SpawnGearVariantElementData();
                        ElementData.GearName = FixName(SGV.m_SpawnedItem.name);
                        ElementData.Position = new Vector3JSON(SGV.m_SpawnedItem.transform.position.x, SGV.m_SpawnedItem.transform.position.y, SGV.m_SpawnedItem.transform.position.z);
                        ElementData.Rotation = new QuaternionJSON(SGV.m_SpawnedItem.transform.rotation.x, SGV.m_SpawnedItem.transform.rotation.y, SGV.m_SpawnedItem.transform.rotation.z, SGV.m_SpawnedItem.transform.rotation.w);

                        GearsToIgnore.Add(SGV.m_SpawnedItem.gameObject);
                        VariantBase.Gears.Add(ElementData);
                    }
                    SceneData.SpawnGearVariants.Add(VariantBase);
                }

                foreach (GearItem Gear in UnityEngine.Object.FindObjectsOfType<GearItem>(true))
                {
                    if (Gear)
                    {
                        if (GearsToIgnore.Contains(Gear.gameObject))
                        {
                            continue;
                        }
                    }
                    
                    if(!Gear.m_InPlayerInventory && !Gear.m_InsideContainer && Gear.m_RadialObjectSpawnerParent == null)
                    {
                        if (Gear.gameObject)
                        {
                            bool isDontDestroy = false;
                            bool isDLC = false;

                            if (Gear.name.StartsWith("GEAR_Placeholder")) // Да-да не удивляйтесь, есть и такая хрень.
                            {
                                continue;
                            }

                            // Сейвовая проверка сцены
                            try
                            {
                                if (Gear.gameObject.scene.IsValid())
                                {
                                    string sceneName = Gear.gameObject.scene.name;

                                    if (sceneName == "DontDestroyOnLoad")
                                    {
                                        isDontDestroy = true;
                                    }
                                    isDLC = sceneName.Contains("_DLC");
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            catch // Если объект скрыт или сцена недоступна - пропускаем
                            {
                                continue;
                            }

                            // Пропускаем объекты из DontDestroyOnLoad
                            if (isDontDestroy)
                            {
                                continue;
                            }

                            DataStr.LooseGearSpawn LooseGear = new LooseGearSpawn();

                            LooseGear.GearName = FixName(Gear.name);
                            LooseGear.Chance = Gear.m_SpawnChance;
                            LooseGear.Position = new Vector3JSON(Gear.transform.position.x, Gear.transform.position.y, Gear.transform.position.z);
                            LooseGear.Rotation = new QuaternionJSON(Gear.transform.rotation.x, Gear.transform.rotation.y, Gear.transform.rotation.z, Gear.transform.rotation.w);

                            LooseGear.DisabledForXP = new List<string>();
                            LooseGear.EnabledForXP = new List<string>();

                            EnableObjectForXPMode EnableForXP = Gear.gameObject.GetComponent<EnableObjectForXPMode>();

                            if (EnableForXP)
                            {
                                foreach (ExperienceModeType XP in EnableForXP.m_XPModesToEnable)
                                {
                                    LooseGear.EnabledForXP.Add(XP.ToString());
                                }
                            }

                            DisableObjectForXPMode DisableForXP = Gear.gameObject.GetComponent<DisableObjectForXPMode>();

                            if (DisableForXP)
                            {
                                foreach (ExperienceModeType XP in DisableForXP.m_XPModesToDisable)
                                {
                                    LooseGear.DisabledForXP.Add(XP.ToString());
                                }
                            }

                            SceneData.LooseGearSpawns.Add(LooseGear);
                        }
                    }
                }

                foreach (RadialObjectSpawner Radial in UnityEngine.Object.FindObjectsOfType<RadialObjectSpawner>(true))
                {
                    RadialObjectSpawnerData Data = Radial.GetData();

                    if (Data != null)
                    {
                        SceneData.RadialSpawns.Add(Data);
                    }
                }

                s_ScenesLootSpawns.Scenes.Add(SceneData);
                SkyCoop.Logger.Log(ConsoleColor.Cyan, $"[GearSpawnsRipper] Scene {SceneData.SceneName} saved, {s_ScenesToSave.Count} Scenes left");

                Next();
            }
        }

        public static void SaveToFile()
        {
            JsonSerializerOptions Options = new JsonSerializerOptions();
            Options.WriteIndented = true;
            string JSON = JsonSerializer.Serialize<DataStr.ScenesLootSpawns>(s_ScenesLootSpawns, Options);
            try
            {
                File.WriteAllText($"{FilesManager.s_DataDirectory}/{s_NameForGearsFiles}", JSON);
                return;
            }
            catch (Exception e)
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"Cant save file: {e.Message}");
            }
        }
    }
}
