using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SkyCoop.ExpeditionManager;
using System.Security.Policy;
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
        public static Dictionary<int, List<ExpeditionTaskTemplate>> m_ExpeditionTasks = new Dictionary<int, List<ExpeditionTaskTemplate>>();

        public static bool Initilized = false;
        public class ExpeditionGearSpawner
        {
            public string m_GUID = "";
            public List<string> m_GearsVariant = new List<string>();
            public float m_Chance = 0;
            public Vector3 m_Possition = new Vector3(0,0,0);
            public Quaternion m_Rotation = new Quaternion(0,0,0,0);
            public DataStr.ExtraDataForDroppedGear m_Extra = new DataStr.ExtraDataForDroppedGear();

            public bool RollChance(int PlayersInExpedition = 1)
            {
                System.Random RNG = new System.Random(Guid.NewGuid().GetHashCode());

                float Chance = m_Chance;

                if(PlayersInExpedition > 1)
                {
                    int Multiplyer = PlayersInExpedition - 1;
                    Chance += (Multiplyer * 5);
                }

                return RNG.NextDouble() < Chance;
            }

            public string PickGear()
            {
                if(m_GearsVariant.Count > 2)
                {
                    System.Random RNG = new System.Random(Guid.NewGuid().GetHashCode());
                    int Idx = RNG.Next(0, m_GearsVariant.Count - 1);
                    return m_GearsVariant[Idx];
                } else if (m_GearsVariant.Count == 1)
                {
                    return m_GearsVariant[0];
                }
                return "GEAR_Tinder";
            }
        }

        public class ExpeditionTaskTemplate
        {
            public ExpeditionTaskType m_TaskType = ExpeditionTaskType.ENTERSCENE;
            public string m_Alias = "";
            public string m_TaskText = "";
            public int m_RegionBelong = 0;
            public string m_SceneName = "";
            public List<ExpeditionGearSpawner> m_GearSpawners = new List<ExpeditionGearSpawner>();
            public List<string> m_Containers = new List<string>();
            public List<string> m_Plants = new List<string>();
            public List<string> m_Breakdowns = new List<string>();
            public bool m_RestockSceneContainers = false;
            public Vector3 m_ZoneCenter = new Vector3(0, 0, 0);
            public float m_ZoneRadius = 1f;
            public string m_NextTaskAlias = "";
            public bool m_CanBeTaken = true;
            public string m_ObjectiveGearGUID = "";
            public ExpeditionCompleteOrder m_CompleatOrder = ExpeditionCompleteOrder.LINEAL;
            public int m_RandomTasksAmout = 0;
            public int m_Time = 3600;
            public bool m_TimeAdd = true;
            public int m_StaySeconds = 300;
            public List<DataStr.UniversalSyncableObjectSpawner> m_Objects = new List<DataStr.UniversalSyncableObjectSpawner>();
        }

        public static void RefrashTemplatesList()
        {
            m_ExpeditionTasks.Clear();
            MPSaveManager.CreateFolderIfNotExist(MPSaveManager.GetBaseDirectory() + "Mods");
            MPSaveManager.CreateFolderIfNotExist(MPSaveManager.GetBaseDirectory() + "Mods" + MPSaveManager.GetSeparator() + "ExpeditionTemplates");

            DirectoryInfo d = new DirectoryInfo(MPSaveManager.GetBaseDirectory() + "Mods" + MPSaveManager.GetSeparator() + "ExpeditionTemplates");
            FileInfo[] Files = d.GetFiles("*.json");
            foreach (FileInfo file in Files)
            {
                byte[] FileData = File.ReadAllBytes(file.FullName);

                string JSONString = UTF8Encoding.UTF8.GetString(FileData);
                if (string.IsNullOrEmpty(JSONString))
                {
                    continue;
                }
                JSONString = MPSaveManager.VectorsFixUp(JSONString);

                ExpeditionTaskTemplate Task = JSON.Load(JSONString).Make<ExpeditionTaskTemplate>();
                if (!m_ExpeditionTasks.ContainsKey(Task.m_RegionBelong))
                {
                    m_ExpeditionTasks.Add(Task.m_RegionBelong, new List<ExpeditionTaskTemplate>());
                }
                m_ExpeditionTasks[Task.m_RegionBelong].Add(Task);

            }
        }

        public static void Init(bool Force = false)
        {
            if (Initilized && !Force)
            {
                return;
            }
            int OneHour = 3600;
            System.Random RNG = new System.Random();
            NextCrashSiteIn = RNG.Next(OneHour * 3, OneHour * 6);
            RefrashTemplatesList();
            Initilized = true;
        }

        public static string GetRandomCrashSiteName(int Index = -1)
        {
            List<string> CrashSites = new List<string>();
            if (Directory.Exists(MPSaveManager.GetBaseDirectory() + "Mods"))
            {
                string ExpeditionFolder = MPSaveManager.GetBaseDirectory() + "Mods" + MPSaveManager.GetSeparator() + "ExpeditionTemplates";

                if (Directory.Exists(ExpeditionFolder))
                {
                    DirectoryInfo d = new DirectoryInfo(ExpeditionFolder);
                    FileInfo[] Files = d.GetFiles("*.json");
                    foreach (FileInfo file in Files)
                    {
                        if (!file.Name.StartsWith("Crashsite"))
                        {
                            continue;
                        }
                        byte[] FileData = File.ReadAllBytes(file.FullName);
                        string JSONString = UTF8Encoding.UTF8.GetString(FileData);
                        if (string.IsNullOrEmpty(JSONString))
                        {
                            continue;
                        }
                        CrashSites.Add(file.Name.Replace(".json", ""));
                    }
                }

                if (CrashSites.Count > 0)
                {
                    if (Index == -1)
                    {
                        if (CrashSites.Count > 1)
                        {
                            return CrashSites[new System.Random().Next(0, CrashSites.Count - 1)];
                        } else
                        {
                            return CrashSites[0];
                        }
                    } else
                    {
                        if (CrashSites.Count - 1 >= Index)
                        {
                            return CrashSites[Index];
                        } else
                        {
                            return "";
                        }
                    }
                } else
                {
                    return "";
                }
            }
            return "";
        }


        public static Expedition BuildBasicExpedition(int Region, string Alias = "", bool DebugFlag = false, bool NoMulty = false)
        {
            bool FoundValid = false;
            int TargetRegion = 0;
            System.Random RNG = new System.Random();
            List<ExpeditionTaskTemplate> Tasks = new List<ExpeditionTaskTemplate>();
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

                Tasks = GetTasksForRegion(TargetRegion);
                DebugLog("Region " + TargetRegion + " has " + Tasks.Count + " Avaliable Tasks");
                if (Tasks.Count > 0)
                {
                    FoundValid = true;
                }

                if (!FoundValid && CanTryNextRegion)
                {
                    int EndIf = RegionIndex;
                    DebugLog("This region invalid for expedition, trying next one, but stop if we once again meet index " + EndIf);
                    int NextTryRegion = RegionIndex + 1;

                    while (true)
                    {
                        TargetRegion = PossibleRegions[NextTryRegion];
                        DebugLog("Next region is Region " + TargetRegion + " with index " + NextTryRegion);
                        Tasks = GetTasksForRegion(TargetRegion);
                        DebugLog("Region " + TargetRegion + " has " + Tasks.Count + " Avaliable Tasks");
                        if (Tasks.Count == 0)
                        {
                            NextTryRegion++;
                            if (NextTryRegion > PossibleRegions.Count - 1)
                            {
                                NextTryRegion = 0;
                            }
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

            ExpeditionTaskTemplate SelectedTask = null;

            if (!string.IsNullOrEmpty(Alias))
            {
                if (NoMulty)
                {
                    foreach (Expedition Active in m_ActiveExpeditions)
                    {
                        if(Active.m_Alias == Alias)
                        {
                            return null;
                        }
                    }
                }
                string ExpeditonJSON = GetExpeditionJsonByAlias(Alias);
                SelectedTask = JSON.Load(ExpeditonJSON).Make<ExpeditionTaskTemplate>();
                FoundValid = true;
            }

            if (FoundValid)
            {
                if (SelectedTask == null)
                {
                    if (Tasks.Count > 1)
                    {
                        SelectedTask = Tasks[RNG.Next(0, Tasks.Count - 1)];
                    } else
                    {
                        SelectedTask = Tasks[0];
                    }
                }

                Expedition Exp = new Expedition();
                Exp.m_Alias = SelectedTask.m_Alias;
                Exp.m_RegionBelong = SelectedTask.m_RegionBelong;
                Exp.m_Name = GetRegionString(SelectedTask.m_RegionBelong) + " Expedition";

                List<ExpeditionTaskTemplate> Templates = new List<ExpeditionTaskTemplate>() { SelectedTask };
                while (true)
                {
                    if(SelectedTask.m_NextTaskAlias == "")
                    {
                        break;
                    } else
                    {
                        string NextTaskAlias;
                        if (SelectedTask.m_NextTaskAlias.Contains("#")) // Example_TaskVar#1-5
                        {
                            string BaseName = SelectedTask.m_NextTaskAlias.Split('#')[0];// Example_TaskVar
                            string Options = SelectedTask.m_NextTaskAlias.Split('#')[1]; // 1-5

                            if (Options.Contains("-")) // 1-5
                            {
                                List<string> Variants = new List<string>();
                                int Min = int.Parse(Options.Split('-')[0]); // 1
                                int Max = int.Parse(Options.Split('-')[1]); // 5
                                int RandomVariantsAmout = SelectedTask.m_RandomTasksAmout; // 3

                                for (int i = Min; i <= Max; i++) // i = 1; i <= 5
                                {
                                    string MayAddAlias = BaseName + i;
                                    bool AlreadyBusy = false;
                                    foreach (Expedition item in m_ActiveExpeditions)
                                    {
                                        foreach (ExpeditionTask item2 in item.m_Tasks)
                                        {
                                            if (item2.m_Alias == MayAddAlias) // Example_TaskVar1
                                            {
                                                AlreadyBusy = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (!AlreadyBusy)
                                    {
                                        Variants.Add(MayAddAlias);
                                    }
                                }

                                for (int i = 1; i <= RandomVariantsAmout; i++)
                                {
                                    int VariantIndex = RNG.Next(0, Variants.Count);
                                    string VariantAlias = Variants[VariantIndex];
                                    Variants.RemoveAt(VariantIndex);
                                    string VariantExpeditionJSON = GetExpeditionJsonByAlias(VariantAlias);
                                    if (string.IsNullOrEmpty(VariantExpeditionJSON))
                                    {
                                        DebugLog("CAN'T FIND EXPEDITION VARIANT TASK " + VariantAlias + "!!!!!!!!!!!");
                                        break;
                                    } else
                                    {
                                        SelectedTask = JSON.Load(VariantExpeditionJSON).Make<ExpeditionTaskTemplate>();
                                        Templates.Add(SelectedTask);
                                    }
                                }
                                continue;
                            } else
                            {
                                NextTaskAlias = SelectedTask.m_NextTaskAlias;
                            }
                        } else
                        {
                            NextTaskAlias = SelectedTask.m_NextTaskAlias;
                        }
                        
                        string ExpeditonJSON = GetExpeditionJsonByAlias(NextTaskAlias);
                        if (string.IsNullOrEmpty(ExpeditonJSON))
                        {
                            DebugLog("CAN'T FIND EXPEDITION TASK " + NextTaskAlias + "!!!!!!!!!!!");
                            break;
                        } else
                        {
                            SelectedTask = JSON.Load(ExpeditonJSON).Make<ExpeditionTaskTemplate>();
                            Templates.Add(SelectedTask);
                        }
                    }
                }

                if(Templates.Count > 0)
                {
                    Exp.m_TimeLeft = Templates[0].m_Time;
                }

                Exp.m_GUID = MPSaveManager.GetNewUGUID();

                foreach (ExpeditionTaskTemplate Template in Templates)
                {
                    ExpeditionTask Task = new ExpeditionTask();
                    Task.m_Debug = DebugFlag;
                    Task.m_Alias = Template.m_Alias;
                    Task.m_Type = Template.m_TaskType;
                    Task.m_Scene = Template.m_SceneName;
                    Task.m_ZoneCenter = Template.m_ZoneCenter;
                    Task.m_ZoneRadius = Template.m_ZoneRadius;
                    Task.m_Text = Template.m_TaskText;
                    Task.m_SpecificContrainers = Template.m_Containers;
                    Task.m_GearSpawners = Template.m_GearSpawners;
                    Task.m_RestockSceneContainers = Template.m_RestockSceneContainers;
                    Task.m_ObjectiveGearSpawnerGUID = Template.m_ObjectiveGearGUID;
                    Task.m_CompleteOrder = Template.m_CompleatOrder;
                    Task.m_Time = Template.m_Time;
                    Task.m_TimeAdd = Template.m_TimeAdd;
                    Task.m_SpecificPlants = Template.m_Plants;
                    Task.m_SpecificBreakdowns = Template.m_Breakdowns;
                    Task.m_StayInZoneSeconds = Template.m_StaySeconds;
                    Task.m_ObjectSpawners = Template.m_Objects;

                    Task.m_ExpeditionGUID = Exp.m_GUID;

                    Exp.m_Tasks.Add(Task);
                }

                return Exp;
            } else
            {
                return null;
            }
        }
        
        public static void MayAddTaskToList(List<ExpeditionTaskTemplate> List, ExpeditionTaskTemplate Task)
        {
            if (!Task.m_CanBeTaken)
            {
                return;
            }
            
            
            foreach (Expedition Exp in m_ActiveExpeditions)
            {
                if (Exp.m_Alias == Task.m_Alias)
                {
                    return;
                }
            }

            List.Add(Task);
        }

        public static List<ExpeditionTaskTemplate> GetTasksForRegion(int Region)
        {
            List<ExpeditionTaskTemplate> Tasks = new List<ExpeditionTaskTemplate>();
            List<ExpeditionTaskTemplate> RawList;
            if (m_ExpeditionTasks.TryGetValue(Region, out RawList))
            {
                foreach (ExpeditionTaskTemplate Task in RawList)
                {
                    MayAddTaskToList(Tasks, Task);
                }

                return Tasks;
            }
            return Tasks;
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
                    Neighbors.Add((int)Shared.GameRegion.Ravine);
                    Neighbors.Add((int)Shared.GameRegion.WindingRiver);
                    Neighbors.Add((int)Shared.GameRegion.BleakInlet);
                    break;
                case Shared.GameRegion.CoastalHighWay:
                    Neighbors.Add((int)Shared.GameRegion.MysteryLake);
                    Neighbors.Add((int)Shared.GameRegion.PlesantValley);
                    Neighbors.Add((int)Shared.GameRegion.DesolationPoint);
                    Neighbors.Add((int)Shared.GameRegion.BleakInlet);
                    Neighbors.Add((int)Shared.GameRegion.CrumblingHighWay);
                    Neighbors.Add((int)Shared.GameRegion.Ravine);
                    break;
                case Shared.GameRegion.DesolationPoint:
                    Neighbors.Add((int)Shared.GameRegion.CoastalHighWay);
                    Neighbors.Add((int)Shared.GameRegion.CrumblingHighWay);
                    Neighbors.Add((int)Shared.GameRegion.Ravine);
                    break;
                case Shared.GameRegion.PlesantValley:
                    Neighbors.Add((int)Shared.GameRegion.MysteryLake);
                    Neighbors.Add((int)Shared.GameRegion.CoastalHighWay);
                    Neighbors.Add((int)Shared.GameRegion.TimberwolfMountain);
                    Neighbors.Add((int)Shared.GameRegion.Blackrock);
                    Neighbors.Add((int)Shared.GameRegion.KeepersPassNorth);
                    Neighbors.Add((int)Shared.GameRegion.KeepersPassSouth);
                    Neighbors.Add((int)Shared.GameRegion.WindingRiver);
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
                    Neighbors.Add((int)Shared.GameRegion.BrokenRailroad);
                    break;
                case Shared.GameRegion.BrokenRailroad:
                    Neighbors.Add((int)Shared.GameRegion.ForlornMuskeg);
                    Neighbors.Add((int)Shared.GameRegion.MysteryLake);
                    Neighbors.Add((int)Shared.GameRegion.MountainTown);
                    Neighbors.Add((int)Shared.GameRegion.Ravine);
                    break;
                case Shared.GameRegion.HushedRiverValley:
                    Neighbors.Add((int)Shared.GameRegion.MountainTown);
                    Neighbors.Add((int)Shared.GameRegion.MysteryLake);
                    Neighbors.Add((int)Shared.GameRegion.ForlornMuskeg);
                    break;
                case Shared.GameRegion.BleakInlet:
                    Neighbors.Add((int)Shared.GameRegion.ForlornMuskeg);
                    Neighbors.Add((int)Shared.GameRegion.MysteryLake);
                    Neighbors.Add((int)Shared.GameRegion.WindingRiver);
                    Neighbors.Add((int)Shared.GameRegion.CoastalHighWay);
                    Neighbors.Add((int)Shared.GameRegion.Ravine);
                    break;
                case Shared.GameRegion.AshCanyon:
                    Neighbors.Add((int)Shared.GameRegion.TimberwolfMountain);
                    Neighbors.Add((int)Shared.GameRegion.Blackrock);
                    Neighbors.Add((int)Shared.GameRegion.PlesantValley);
                    break;
                case Shared.GameRegion.Blackrock:
                    Neighbors.Add((int)Shared.GameRegion.TimberwolfMountain);
                    Neighbors.Add((int)Shared.GameRegion.PlesantValley);
                    Neighbors.Add((int)Shared.GameRegion.KeepersPassSouth);
                    Neighbors.Add((int)Shared.GameRegion.KeepersPassNorth);
                    break;
                case Shared.GameRegion.CrumblingHighWay:
                    Neighbors.Add((int)Shared.GameRegion.CoastalHighWay);
                    Neighbors.Add((int)Shared.GameRegion.DesolationPoint);
                    Neighbors.Add((int)Shared.GameRegion.Ravine);
                    break;
                case Shared.GameRegion.WindingRiver:
                    Neighbors.Add((int)Shared.GameRegion.MysteryLake);
                    Neighbors.Add((int)Shared.GameRegion.PlesantValley);
                    Neighbors.Add((int)Shared.GameRegion.Ravine);
                    Neighbors.Add((int)Shared.GameRegion.ForlornMuskeg);
                    break;
                case Shared.GameRegion.KeepersPassNorth:
                    Neighbors.Add((int)Shared.GameRegion.Blackrock);
                    Neighbors.Add((int)Shared.GameRegion.PlesantValley);
                    Neighbors.Add((int)Shared.GameRegion.WindingRiver);
                    Neighbors.Add((int)Shared.GameRegion.TimberwolfMountain);
                    break;
                case Shared.GameRegion.KeepersPassSouth:
                    Neighbors.Add((int)Shared.GameRegion.Blackrock);
                    Neighbors.Add((int)Shared.GameRegion.PlesantValley);
                    Neighbors.Add((int)Shared.GameRegion.WindingRiver);
                    Neighbors.Add((int)Shared.GameRegion.TimberwolfMountain);
                    break;
                case Shared.GameRegion.Ravine:
                    Neighbors.Add((int)Shared.GameRegion.BleakInlet);
                    Neighbors.Add((int)Shared.GameRegion.MysteryLake);
                    Neighbors.Add((int)Shared.GameRegion.CoastalHighWay);
                    Neighbors.Add((int)Shared.GameRegion.CrumblingHighWay);
                    Neighbors.Add((int)Shared.GameRegion.WindingRiver);
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
                case Shared.GameRegion.CrumblingHighWay:
                    return "Crumbling Highway";
                case Shared.GameRegion.Ravine:
                    return "Ravine";
                case Shared.GameRegion.WindingRiver:
                    return "Winding River";
                case Shared.GameRegion.KeepersPassSouth:
                    return "Keepers Pass South";
                case Shared.GameRegion.KeepersPassNorth:
                    return "Keepers Pass North";
                default:
                    return "Unknown " + Region;
            }
        }

        public static string GetExpeditionJsonByAlias(string Alias)
        {
            if(Directory.Exists(MPSaveManager.GetBaseDirectory() + "Mods"))
            {
                if(Directory.Exists(MPSaveManager.GetBaseDirectory() + "Mods" + MPSaveManager.GetSeparator() + "ExpeditionTemplates"))
                {
                    byte[] FileData = File.ReadAllBytes(MPSaveManager.GetBaseDirectory() + "Mods" + MPSaveManager.GetSeparator() + "ExpeditionTemplates" + MPSaveManager.GetSeparator() + Alias + ".json");
                    string JSONString = UTF8Encoding.UTF8.GetString(FileData);
                    if (string.IsNullOrEmpty(JSONString))
                    {
                        return "";
                    }
                    JSONString = MPSaveManager.VectorsFixUp(JSONString);
                    return JSONString;
                } else
                {
                    Directory.CreateDirectory(MPSaveManager.GetBaseDirectory() + "Mods" + MPSaveManager.GetSeparator() + "ExpeditionTemplates");
                }
            } else
            {
                Directory.CreateDirectory(MPSaveManager.GetBaseDirectory() + "Mods");
                Directory.CreateDirectory(MPSaveManager.GetBaseDirectory() + "Mods" + MPSaveManager.GetSeparator() + "ExpeditionTemplates");
            }
            return "";
        }
    }
}
