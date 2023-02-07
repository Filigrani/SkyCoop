using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SkyCoop.ExpeditionManager;
#if (DEDICATED)
using System.Numerics;
using TinyJSON;
#else
using MelonLoader.TinyJSON;
using MelonLoader;
using UnityEngine;
#endif

namespace SkyCoop
{
    public class ExpeditionBuilder
    {
        public static int NextZoneID = 1;
        public static Dictionary<int, Dictionary<int, ExpeditionZone>> m_ExpeditionZones = new Dictionary<int, Dictionary<int, ExpeditionZone>>();
        public static Dictionary<int, List<ExpeditionRewardScene>> m_ExpeditionRewardScenes = new Dictionary<int, List<ExpeditionRewardScene>>();
        public static Dictionary<int, ExpeditionZone> m_ExpeditionZonesByID = new Dictionary<int, ExpeditionZone>();

#if (DEDICATED_LINUX)
        public static string Seperator = @"/";
#else
        public static string Seperator = @"\";
#endif

        public static bool Initilized = false;
        public class ExpeditionZone
        {
            public Vector3 m_ZoneCenter = new Vector3(0, 0, 0);
            public float m_ZoneRadius = 0;
            public string m_ZoneScene = "";
            public List<string> m_ZoneContainers = new List<string>();
            public List<ExpeditionGearSpawner> m_ZoneGearSpawners = new List<ExpeditionGearSpawner>();
            public string m_ZoneTaskText = "";
            public int m_BelongRegion = 0;
            
        }
        public class ExpeditionGearSpawner
        {
            public string m_GUID = "";
            public List<string> m_GearsVariant = new List<string>();
            public float m_Chance = 0;
            public Vector3 m_Possition = new Vector3(0,0,0);
            public Quaternion m_Rotation = new Quaternion(0,0,0,0);

            public bool RollChance()
            {
                System.Random RNG = new System.Random();

                return RNG.NextDouble() < m_Chance;
            }

            public string PickGear()
            {
                if(m_GearsVariant.Count > 2)
                {
                    System.Random RNG = new System.Random();
                    int Idx = RNG.Next(0, m_GearsVariant.Count - 1);
                    return m_GearsVariant[Idx];
                } else if (m_GearsVariant.Count == 1)
                {
                    return m_GearsVariant[0];
                }
                return "GEAR_Tinder";
            }
        }

        public static void AddZone(ExpeditionZone Zone)
        {
            if (!m_ExpeditionZones.ContainsKey(Zone.m_BelongRegion))
            {
                m_ExpeditionZones.Add(Zone.m_BelongRegion, new Dictionary<int, ExpeditionZone>());
            }
            m_ExpeditionZones[Zone.m_BelongRegion].Add(NextZoneID, Zone);
            m_ExpeditionZonesByID[NextZoneID] = Zone;
            NextZoneID++;
        }
        public static void AddRewardScene(ExpeditionRewardScene RewardScene)
        {
            if (!m_ExpeditionRewardScenes.ContainsKey(RewardScene.m_BelongRegion))
            {
                m_ExpeditionRewardScenes.Add(RewardScene.m_BelongRegion, new List<ExpeditionRewardScene>());
            }
            m_ExpeditionRewardScenes[RewardScene.m_BelongRegion].Add(RewardScene);
        }

        public static void Init()
        {
            if (Initilized)
            {
                return;
            }
#if (DEDICATED_LINUX)
            string Seperator = @"/";
#else
            string Seperator = @"\";
#endif
            MPSaveManager.CreateFolderIfNotExist("Mods");
            MPSaveManager.CreateFolderIfNotExist("Mods" + Seperator + "ExpeditionTemplates");

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
                JSONString = MPSaveManager.VectorsFixUp(JSONString);
                string Alias = file.Name.Replace(".json", "");
                bool ThisIsZone = JSONString.Contains("+ExpeditionZone");

                if (ThisIsZone)
                {
                    ExpeditionZone Zone = JSON.Load(JSONString).Make<ExpeditionZone>();
                    AddZone(Zone);

                } else
                {
                    ExpeditionRewardScene RewardScene = JSON.Load(JSONString).Make<ExpeditionRewardScene>();
                    AddRewardScene(RewardScene);
                }
            }
            Initilized = true;
        }
        
        
        public static ExpeditionTemplate BuildBasicExpedition(int Region, string Alias = "", bool DebugFlag = false)
        {
            bool FoundValid = false;
            int TargetRegion = 0;
            bool NoIndoors = false;
            bool NoZoneSpawns = false;
            System.Random RNG = new System.Random();
            List<ExpeditionRewardScene> RewardScenes = new List<ExpeditionRewardScene>();
            List<int> RewardZones = new List<int>();
            if (string.IsNullOrEmpty(Alias))
            {
                DebugLog("Searching neighbors for Region " + Region);
                bool CanTryNextRegion = false;
                List<int> PossibleRegions = GetNeighborRegions(Region);
                DebugLog("Found " + PossibleRegions.Count + " neighbors");

                int RegionIndex = 0;
                if (PossibleRegions.Count > 2)
                {
                    RegionIndex = RNG.Next(0, PossibleRegions.Count - 1);
                    TargetRegion = PossibleRegions[RegionIndex];
                    CanTryNextRegion = true;
                    DebugLog("Randomly select Region " + TargetRegion + " with index " + RegionIndex);
                } else if (PossibleRegions.Count == 1)
                {
                    TargetRegion = PossibleRegions[0];
                    DebugLog("Selecting Region " + TargetRegion);
                }

                RewardScenes = GetRewardScenesForRegion(TargetRegion);
                DebugLog("Region " + TargetRegion + " has " + RewardScenes.Count + " Reward Scenes");
                RewardZones = GetRewardZonesForRegion(TargetRegion);
                DebugLog("Region " + TargetRegion + " has " + RewardZones.Count + " Reward Zones");
                if (RewardScenes.Count == 0)
                {
                    NoIndoors = true;
                } else
                {
                    FoundValid = true;
                }

                if (RewardZones.Count == 0)
                {
                    NoZoneSpawns = true;
                } else
                {
                    FoundValid = true;
                }

                if (NoIndoors && NoZoneSpawns && CanTryNextRegion && string.IsNullOrEmpty(Alias))
                {
                    int EndIf = RegionIndex;
                    DebugLog("This region invalid for expedition, trying next one, but stop if we once again meet index " + EndIf);
                    int NextTryRegion = RegionIndex + 1;

                    while (true)
                    {
                        if (NextTryRegion > PossibleRegions.Count - 1)
                        {
                            NextTryRegion = 0;
                        }
                        TargetRegion = PossibleRegions[NextTryRegion];
                        DebugLog("Next region is Region " + TargetRegion + " with index " + NextTryRegion);
                        RewardScenes = GetRewardScenesForRegion(TargetRegion);
                        DebugLog("Region " + TargetRegion + " has " + RewardScenes.Count + " Reward Scenes");
                        RewardZones = GetRewardZonesForRegion(TargetRegion);
                        DebugLog("Region " + TargetRegion + " has " + RewardZones.Count + " Reward Zones");
                        if (RewardScenes.Count == 0)
                        {
                            NoIndoors = true;
                        } else
                        {
                            NoIndoors = false;
                        }

                        if (RewardZones.Count == 0)
                        {
                            NoZoneSpawns = true;
                        } else
                        {
                            NoZoneSpawns = false;
                        }
                        if (NoIndoors && NoZoneSpawns)
                        {
                            NextTryRegion++;
                            if (NextTryRegion == EndIf)
                            {
                                break;
                            } else
                            {
                                continue;
                            }
                        } else
                        {
                            FoundValid = true;
                            DebugLog("This region is valid for expedition");
                            break;
                        }
                    }
                }
            }

            ExpeditionRewardScene RewardScene = null;
            ExpeditionZone Zone = null;
            bool PreDefined = false;
            bool ShouldUseIndoor = false;

            if (!string.IsNullOrEmpty(Alias))
            {
                string ExpeditonJSON = GetExpeditionJsonByAlias(Alias);
                if(!ThisIsJsonOfZone(ExpeditonJSON))
                {
                    RewardScene = JSON.Load(ExpeditonJSON).Make<ExpeditionRewardScene>();
                    TargetRegion = RewardScene.m_BelongRegion;
                    ShouldUseIndoor = true;
                } else
                {
                    Zone = JSON.Load(ExpeditonJSON).Make<ExpeditionZone>();
                    TargetRegion = Zone.m_BelongRegion;
                }
                PreDefined = true;
                FoundValid = true;
            }


            if (FoundValid)
            {
                int RewardZone = -1;

                if (!PreDefined)
                {
                    if (!NoIndoors && !NoZoneSpawns)
                    {
                        DebugLog("This region has indoors and zone spawns, rolling 50/50");
                        if (RNG.NextDouble() > 0.5)
                        {
                            ShouldUseIndoor = true;
                            DebugLog("Going to use indoors");
                        } else
                        {
                            DebugLog("Going to use spawn zones");
                        }
                    } else if (!NoIndoors && NoZoneSpawns)
                    {
                        ShouldUseIndoor = true;
                        DebugLog("This region has only indoors");
                    } else
                    {
                        DebugLog("This region has only spawn zones");
                    }
                }

                if (ShouldUseIndoor)
                {
                    if(RewardScene == null)
                    {
                        if (RewardScenes.Count > 1)
                        {
                            RewardScene = RewardScenes[RNG.Next(0, RewardScenes.Count - 1)];
                        } else
                        {
                            RewardScene = RewardScenes[0];
                        }
                    }

                    ExpeditionTemplate Exp = new ExpeditionTemplate();
                    Exp.m_Debug = DebugFlag;
                    Exp.m_CompleteOrder = ExpeditionCompleteOrder.LINEAL;
                    Exp.m_Tasks.Add(GetEnterSceneTask(RewardScene.m_SceneName, RewardScene.m_Task));
                    Exp.m_RewardScene = RewardScene.m_SceneName;
                    Exp.m_RegionBelong = TargetRegion;
                    Exp.m_Name = GetRegionString(TargetRegion) + " Expedition";
                    Exp.m_GearSpawners = RewardScene.m_GearSpawners;
                    DebugLog("Done, RewardScene is "+ RewardScene.m_SceneName);
                    return Exp;
                } else
                {
                    if(Zone == null)
                    {
                        if (RewardZones.Count > 1)
                        {
                            RewardZone = RewardZones[RNG.Next(0, RewardZones.Count - 1)];
                        } else
                        {
                            RewardZone = RewardZones[0];
                        }
                        Zone = m_ExpeditionZonesByID[RewardZone];
                    }
                    
                    if(Zone != null)
                    {
                        ExpeditionTemplate Exp = new ExpeditionTemplate();
                        Exp.m_Debug = DebugFlag;
                        Exp.m_CompleteOrder = ExpeditionCompleteOrder.LINEAL;
                        Exp.m_Tasks.Add(GetEnterZoneTask(Zone.m_ZoneScene, Zone.m_ZoneTaskText, Zone.m_ZoneCenter, Zone.m_ZoneRadius));
                        Exp.m_SpecificContrainers = Zone.m_ZoneContainers;
                        Exp.m_SceneForSpecificContrainersZone = Zone.m_ZoneScene;
                        Exp.m_RewardZoneID = RewardZone;
                        Exp.m_RegionBelong = TargetRegion;
                        Exp.m_Name = GetRegionString(TargetRegion) + " Expedition";
                        Exp.m_GearSpawners = Zone.m_ZoneGearSpawners;

                        DebugLog("Done, RewardZone is  " + RewardZone);
                        return Exp;
                    }

                    return null;
                }
            } else
            {
                return null;
            }
        }

        public static void MaybeAdvanceExpedition(ExpeditionTemplate Exp)
        {
            //if (Exp.m_Tasks[0].m_Type == ExpeditionTaskType.ENTERSCENE)
            //{
            //    Exp.m_Tasks.Insert(0, GetFindGearTask("", new Vector3(0,0,0), 0));
            //    Exp.m_CompleteOrder = ExpeditionCompleteOrder.LINEALHIDDEN;
            //}
        }

        public class ExpeditionRewardScene
        {
            public string m_SceneName = "";
            public string m_Task = "";
            public string m_TaskAlt = "";
            public List<ExpeditionGearSpawner> m_GearSpawners = new List<ExpeditionGearSpawner>();
            public int m_BelongRegion = 0;
        }
        
        public static void MayAddSceneToList(List<ExpeditionRewardScene> List, ExpeditionRewardScene RewardScene)
        {
            foreach (Expedition Exp in m_ActiveExpeditions)
            {
                if (Exp.m_Template.m_RewardScene == RewardScene.m_SceneName)
                {
                    return;
                }
            }

            List.Add(RewardScene);
        }

        public static void MayAddZoneToList(int ZoneID, List<int> List)
        {
            foreach (Expedition Exp in m_ActiveExpeditions)
            {
                if (Exp.m_Template.m_RewardZoneID == ZoneID)
                {
                    return;
                }
            }
            List.Add(ZoneID);
        }

        public static List<int> GetRewardZonesForRegion(int Region)
        {
            List<int> Zones = new List<int>();
            Dictionary<int, ExpeditionZone> Dict;
            if(m_ExpeditionZones.TryGetValue(Region, out Dict))
            {
                foreach (var item in Dict)
                {
                    MayAddZoneToList(item.Key, Zones);
                }

                return Zones;
            }
            return Zones;
        }

        public static List<ExpeditionRewardScene> GetRewardScenesForRegion(int Region)
        {
            List<ExpeditionRewardScene> Scenes = new List<ExpeditionRewardScene>();
            List<ExpeditionRewardScene> RawList;
            if (m_ExpeditionRewardScenes.TryGetValue(Region, out RawList))
            {
                foreach (ExpeditionRewardScene RewardScene in RawList)
                {
                    MayAddSceneToList(Scenes, RewardScene);
                }

                return Scenes;
            }
            return Scenes;
        }
        public static List<int> GetNeighborRegions(int Region)
        {
            List<int> Neighbors = new List<int>();

            switch ((Shared.GameRegion)Region)
            {
                case Shared.GameRegion.MysteryLake:
                    Neighbors.Add((int)Shared.GameRegion.ForlornMuskeg);
                    Neighbors.Add((int)Shared.GameRegion.MountainTown);
                    Neighbors.Add((int)Shared.GameRegion.PlesantValley);
                    Neighbors.Add((int)Shared.GameRegion.CoastalHighWay);
                    break;
                case Shared.GameRegion.CoastalHighWay:
                    Neighbors.Add((int)Shared.GameRegion.MysteryLake);
                    Neighbors.Add((int)Shared.GameRegion.PlesantValley);
                    Neighbors.Add((int)Shared.GameRegion.DesolationPoint);
                    Neighbors.Add((int)Shared.GameRegion.BleakInlet);
                    break;
                case Shared.GameRegion.DesolationPoint:
                    Neighbors.Add((int)Shared.GameRegion.CoastalHighWay);
                    break;
                case Shared.GameRegion.PlesantValley:
                    Neighbors.Add((int)Shared.GameRegion.MysteryLake);
                    Neighbors.Add((int)Shared.GameRegion.CoastalHighWay);
                    Neighbors.Add((int)Shared.GameRegion.TimberwolfMountain);
                    Neighbors.Add((int)Shared.GameRegion.Blackrock);
                    break;
                case Shared.GameRegion.TimberwolfMountain:
                    Neighbors.Add((int)Shared.GameRegion.PlesantValley);
                    Neighbors.Add((int)Shared.GameRegion.AshCanyon);
                    Neighbors.Add((int)Shared.GameRegion.Blackrock);
                    break;
                case Shared.GameRegion.ForlornMuskeg:
                    Neighbors.Add((int)Shared.GameRegion.MountainTown);
                    Neighbors.Add((int)Shared.GameRegion.BrokenRailroad);
                    Neighbors.Add((int)Shared.GameRegion.BleakInlet);
                    Neighbors.Add((int)Shared.GameRegion.MysteryLake);
                    break;
                case Shared.GameRegion.MountainTown:
                    Neighbors.Add((int)Shared.GameRegion.ForlornMuskeg);
                    Neighbors.Add((int)Shared.GameRegion.MysteryLake);
                    Neighbors.Add((int)Shared.GameRegion.HushedRiverValley);
                    break;
                case Shared.GameRegion.BrokenRailroad:
                    Neighbors.Add((int)Shared.GameRegion.ForlornMuskeg);
                    break;
                case Shared.GameRegion.HushedRiverValley:
                    Neighbors.Add((int)Shared.GameRegion.MountainTown);
                    break;
                case Shared.GameRegion.BleakInlet:
                    Neighbors.Add((int)Shared.GameRegion.ForlornMuskeg);
                    Neighbors.Add((int)Shared.GameRegion.MysteryLake);
                    Neighbors.Add((int)Shared.GameRegion.CoastalHighWay);
                    break;
                case Shared.GameRegion.AshCanyon:
                    Neighbors.Add((int)Shared.GameRegion.TimberwolfMountain);
                    break;
                case Shared.GameRegion.Blackrock:
                    Neighbors.Add((int)Shared.GameRegion.TimberwolfMountain);
                    Neighbors.Add((int)Shared.GameRegion.PlesantValley);
                    break;
                default:
                    break;
            }
            return Neighbors;
        }

        public static string GetRegionString(int Region)
        {
            switch ((Shared.GameRegion)Region)
            {
                case Shared.GameRegion.MysteryLake:
                    return "Mystery Lake";
                case Shared.GameRegion.CoastalHighWay:
                    return "Coastal Highway";
                case Shared.GameRegion.DesolationPoint:
                    return "Desolation Point";
                case Shared.GameRegion.PlesantValley:
                    return "Plesant Valley";
                case Shared.GameRegion.TimberwolfMountain:
                    return "Timberwolf Mountain";
                case Shared.GameRegion.ForlornMuskeg:
                    return "Forlorn Muskeg";
                case Shared.GameRegion.MountainTown:
                    return "Mountain Town";
                case Shared.GameRegion.BrokenRailroad:
                    return "Broken Railroad";
                case Shared.GameRegion.HushedRiverValley:
                    return "Hushed River Valley";
                case Shared.GameRegion.BleakInlet:
                    return "Bleak Inlet";
                case Shared.GameRegion.AshCanyon:
                    return "Ash Canyon";
                case Shared.GameRegion.Blackrock:
                    return "Blackrock";
                default:
                    return "Unknown " + Region;
            }
        }

        public static ExpeditionTask GetFindGearTask(string Scene, string TaskText, Vector3 ZoneCenter, float ZoneRadius)
        {
            ExpeditionTask Task = new ExpeditionTask();
            Task.m_Type = ExpeditionTaskType.COLLECT;
            Task.m_Scene = Scene;
            Task.m_ZoneCenter = ZoneCenter;
            Task.m_ZoneRadius = ZoneRadius;
            Task.m_TaskText = TaskText;
            return Task;
        }
        public static ExpeditionTask GetEnterSceneTask(string Scene, string TaskTex)
        {
            ExpeditionTask Task = new ExpeditionTask();
            Task.m_Type = ExpeditionTaskType.ENTERSCENE;
            Task.m_Scene = Scene;
            Task.m_TaskText = TaskTex;
            return Task;
        }
        public static ExpeditionTask GetEnterZoneTask(string Scene, string TaskTex, Vector3 ZoneCenter, float ZoneRadius)
        {
            ExpeditionTask Task = new ExpeditionTask();
            Task.m_Type = ExpeditionTaskType.ENTERZONE;
            Task.m_Scene = Scene;
            Task.m_ZoneCenter = ZoneCenter;
            Task.m_ZoneRadius = ZoneRadius;
            Task.m_TaskText = TaskTex;
            return Task;
        }

        public static string GetExpeditionJsonByAlias(string Alias)
        {
            byte[] FileData = File.ReadAllBytes("Mods" + Seperator + "ExpeditionTemplates" + Seperator + Alias+".json");
            string JSONString = UTF8Encoding.UTF8.GetString(FileData);
            if (string.IsNullOrEmpty(JSONString))
            {
                return "";
            }
            JSONString = MPSaveManager.VectorsFixUp(JSONString);
            return JSONString;
        }

        public static bool ThisIsJsonOfZone(string JSONString)
        {
            return JSONString.Contains("+ExpeditionZone");
        }
    }
}
