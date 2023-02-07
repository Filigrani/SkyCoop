using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer;
using System.Security.Policy;
using static SkyCoop.DataStr;
using static SkyCoop.ExpeditionBuilder;
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
    public class ExpeditionManager
    {
        public static Dictionary<string, bool> m_TrackableGears = new Dictionary<string, bool>();
        public static List<Expedition> m_ActiveExpeditions = new List<Expedition>();
        public static Dictionary<string, int> m_UnavailableGearSpawners = new Dictionary<string, int>();
        public static Dictionary<int, string> m_GearSpawnerGears = new Dictionary<int, string>();

        public static bool Debug = true;

        public static void Log(string LOG)
        {
#if (DEDICATED)
            Logger.Log("[MPSaveManager] " +LOG, Shared.LoggerColor.Blue);
#else
            MelonLoader.MelonLogger.Msg(ConsoleColor.Blue, "[ExpeditionManager] " + LOG);
#endif
        }
        public static void DebugLog(string LOG)
        {
            if (!Debug)
            {
                return;
            }
#if (DEDICATED)
            Logger.Log("[MPSaveManager] " +LOG, Shared.LoggerColor.Blue);
#else
            MelonLoader.MelonLogger.Msg(ConsoleColor.Blue, "[ExpeditionManager] " + LOG);
#endif
        }

        public enum ExpeditionTaskType
        {
            ENTERSCENE,
            ENTERZONE,
            COLLECT,
        }

        public enum ExpeditionCompleteOrder
        {
            LINEAL,
            LINEALHIDDEN,
            ANYORDER,
        }

        public static void StartNewExpedition(string LeaderMAC, int Region, string Alias = "")
        {
            DebugLog("Client with MAC "+LeaderMAC+" trying start expedition on region "+Region);

            int LeaderID = Server.GetIDByMAC(LeaderMAC);

            for (int i = 0; i < m_ActiveExpeditions.Count; i++)
            {
                if (m_ActiveExpeditions[i].m_Players.Contains(LeaderMAC))
                {
                    ServerSend.ADDHUDMESSAGE(LeaderID, "You are already in expedition!");
                    return;
                }
            }

            Expedition Exp = new Expedition();
            ExpeditionTemplate Template = BuildBasicExpedition(Region, Alias);

            if(Template != null)
            {
                Exp.m_Template = Template;
            } else
            {
                return;
            }

            DebugLog("Expedition created");
            Exp.m_Players.Add(LeaderMAC);
            Exp.m_GUID = MPSaveManager.GetNewUGUID();
            m_ActiveExpeditions.Add(Exp);
        }

        public static void InviteToExpedition(string LeaderMAC, string InviteMAC)
        {
            Expedition MyExpedition = null;
            int LeaderID = Server.GetIDByMAC(LeaderMAC);
            int InviteID = Server.GetIDByMAC(InviteMAC);

            for (int i = 0; i < m_ActiveExpeditions.Count; i++)
            {
                if (m_ActiveExpeditions[i].m_Players.Contains(LeaderMAC))
                {
                    MyExpedition = m_ActiveExpeditions[i];
                }
            }

            if (MyExpedition != null)
            {
                bool AlreadyInExpedition = false;
                for (int i = 0; i < m_ActiveExpeditions.Count; i++)
                {
                    if (m_ActiveExpeditions[i].m_Players.Contains(InviteMAC))
                    {
                        AlreadyInExpedition = false;
                    }
                }

                if (!AlreadyInExpedition)
                {
                    if (!MyExpedition.m_Players.Contains(InviteMAC))
                    {
                        MyExpedition.m_Players.Add(InviteMAC);
                    } else
                    {
                        ServerSend.ADDHUDMESSAGE(LeaderID, "This player already in your expedition!");
                        return;
                    }
                } else{
                    ServerSend.ADDHUDMESSAGE(LeaderID, "This player already in another expedition!");
                    return;
                }
            } else
            {
                ServerSend.ADDHUDMESSAGE(LeaderID, "You are not in expedition!");
                return;
            }
        }

        public static void CompleteExpedition(string GUID, bool WithReward = true)
        {
            int RemoveID = -1;
            for (int i = 0; i < m_ActiveExpeditions.Count; i++)
            {
                if (m_ActiveExpeditions[i].m_GUID == GUID)
                {
                    RemoveID = i; 
                    break;
                }
            }
            if(RemoveID != -1)
            {
                CompleteExpedition(RemoveID, WithReward);
            }
        }
        public static void CompleteExpedition(int RemoveID, bool WithReward = true)
        {
            string RewardScene = m_ActiveExpeditions[RemoveID].m_Template.m_RewardScene;
            List<string> RewardContainers = m_ActiveExpeditions[RemoveID].m_Template.m_SpecificContrainers;
            string RewardContainerScene = m_ActiveExpeditions[RemoveID].m_Template.m_SceneForSpecificContrainersZone;
            List<int> PlayersIDs = m_ActiveExpeditions[RemoveID].GetExpeditionPlayersIDs();
            List<ExpeditionGearSpawner> Spawners = m_ActiveExpeditions[RemoveID].m_Template.m_GearSpawners;
            bool DebugFlag = m_ActiveExpeditions[RemoveID].m_Template.m_Debug;
            if (RemoveID != -1)
            {
                int FinishState = 0;
                if (WithReward)
                {
                    if (!string.IsNullOrEmpty(RewardScene))
                    {
                        MPSaveManager.AddLootToScene(RewardScene);
                    }
                    if (RewardContainers.Count > 0 && !string.IsNullOrEmpty(RewardContainerScene))
                    {
                        foreach (string ContainerGUID in RewardContainers)
                        {
                            MPSaveManager.AddLootToContainerOnScene(ContainerGUID, RewardContainerScene);
                        }
                    }

                    if (!string.IsNullOrEmpty(RewardScene))
                    {
                        foreach (ExpeditionGearSpawner spawn in Spawners)
                        {
                            CreateRewardGear(spawn, RewardScene, DebugFlag);
                        }
                    } else if (!string.IsNullOrEmpty(RewardContainerScene))
                    {
                        foreach (ExpeditionGearSpawner spawn in Spawners)
                        {
                            CreateRewardGear(spawn, RewardContainerScene, DebugFlag);
                        }
                    }

                    FinishState = 1;
                }

                foreach (int ClientID in PlayersIDs)
                {
                    if (ClientID == 0)
                    {
#if (!DEDICATED)
                        MyMod.DoExpeditionState(FinishState);
#endif
                    } else
                    {
                        ServerSend.EXPEDITIONRESULT(ClientID, FinishState);
                    }
                }

                m_ActiveExpeditions.RemoveAt(RemoveID);
            }
        }

        public static void UpdateExpeditions()
        {
            //DebugLog("UpdateExpeditions() m_ActiveExpeditions.Count "+ m_ActiveExpeditions.Count);
            for (int i = m_ActiveExpeditions.Count - 1; i > -1; i--)
            {
                m_ActiveExpeditions[i].UpdateTasks();
                if (m_ActiveExpeditions[i].m_Completed)
                {
                    CompleteExpedition(i);
                }
            }
        }

        public class ExpeditionTemplate
        {
            public ExpeditionCompleteOrder m_CompleteOrder = ExpeditionCompleteOrder.LINEAL;
            public string m_Name = "";
            public List<ExpeditionTask> m_Tasks = new List<ExpeditionTask>();
            public int m_RegionBelong = 0;
            public string m_RewardScene = "";
            public int m_RewardZoneID = 0;
            public string m_SceneForSpecificContrainersZone = "";
            public List<string> m_SpecificContrainers = new List<string>();
            public List<ExpeditionGearSpawner> m_GearSpawners = new List<ExpeditionGearSpawner>();
            public bool m_Debug = false;
        }

        public class Expedition
        {
            public ExpeditionTemplate m_Template = new ExpeditionTemplate();
            public List<string> m_Players = new List<string>();
            public string m_GUID = "";
            public bool m_Completed = false;
            public int m_TimeLeft = 7200;

            public List<DataStr.MultiPlayerClientData> GetExpeditionPlayersData()
            {
                List<DataStr.MultiPlayerClientData> Data = new List<DataStr.MultiPlayerClientData>();
                foreach (string MAC in m_Players)
                {
                    int ClientID = Server.GetIDByMAC(MAC);
                    if (ClientID != -1 && ClientID != 0)
                    {
                        if (MyMod.playersData[ClientID] != null)
                        {
                            Data.Add(MyMod.playersData[ClientID]);
                        }
                    }else if(ClientID == 0)
                    {
#if (!DEDICATED)
                        MultiPlayerClientData P = new MultiPlayerClientData();
                        P.m_LevelGuid = MyMod.level_guid;
                        P.m_Position = GameManager.GetPlayerTransform().position;
                        Data.Add(P);
#endif
                    }
                }

                return Data;
            }
            public List<int> GetExpeditionPlayersIDs()
            {
                List<int> IDs = new List<int>();
                foreach (string MAC in m_Players)
                {
                    int ClientID = Server.GetIDByMAC(MAC);
                    if (ClientID != -1)
                    {
                        if (MyMod.playersData[ClientID] != null)
                        {
                            IDs.Add(ClientID);
                        }
                    }
                }

                return IDs;
            }

            public void UpdateTasks()
            {
                int NeedToComplete = m_Template.m_Tasks.Count;
                int Completed = 0;
                m_TimeLeft--;
                if(m_TimeLeft <= 0)
                {
                    CompleteExpedition(m_GUID, false);
                }

                List<DataStr.MultiPlayerClientData> PlayersData = GetExpeditionPlayersData();
                string Text = "";

                foreach (ExpeditionTask Task in m_Template.m_Tasks)
                {
                    if (Task.m_IsComplete)
                    {
                        Completed++;
                    } else
                    {
                        Task.Update(PlayersData);
                    }
                    Text = Task.m_TaskText;

                    if ((m_Template.m_CompleteOrder == ExpeditionCompleteOrder.LINEAL || m_Template.m_CompleteOrder == ExpeditionCompleteOrder.LINEALHIDDEN) && !Task.m_IsComplete) // If Lineal do not update later tasks.
                    {
                        break;
                    }
                }

                foreach (int Client in GetExpeditionPlayersIDs())
                {
                    if(Client == 0)
                    {
#if (!DEDICATED)
                        MyMod.OnExpedition = true;
                        MyMod.ExpeditionLastName = m_Template.m_Name;
                        MyMod.ExpeditionLastTaskText = Text;
                        MyMod.ExpeditionLastTime = m_TimeLeft;
#endif
                    } else
                    {
                        ServerSend.EXPEDITIONSYNC(Client, m_Template.m_Name, Text, m_TimeLeft);
                    }
                }

                
                //DebugLog("UpdateTasks() " + Completed + "/" + NeedToComplete);

                if (NeedToComplete == Completed)
                {
                    m_Completed = true;
                }
            }
        }

        public class ExpeditionTask
        {
            public ExpeditionTaskType m_Type = ExpeditionTaskType.ENTERSCENE;
            public string m_Scene = "";
            public Vector3 m_ZoneCenter = new Vector3(0, 0, 0);
            public float m_ZoneRadius = 0;
            public bool m_IsComplete = false;
            public string m_GearTrackCode = "";
            public bool m_GearSpawned = false;
            public string m_TaskText = "";

            public void Update(List<DataStr.MultiPlayerClientData> PlayersData)
            {
                //DebugLog("[ExpeditionTask] Update()");
                if (m_Type == ExpeditionTaskType.COLLECT && m_GearSpawned)
                {
                    if (!m_TrackableGears.ContainsKey(m_GearTrackCode))
                    {
                        m_IsComplete = true;
                        return;
                    }
                } else{
                    foreach (DataStr.MultiPlayerClientData PlayerData in PlayersData)
                    {
                        //DebugLog("[ExpeditionTask] PlayerData.m_LevelGuid " + PlayerData.m_LevelGuid);
                        if (m_Type == ExpeditionTaskType.ENTERSCENE)
                        {
                            if (PlayerData.m_LevelGuid == m_Scene)
                            {
                                m_IsComplete = true;
                                return;
                            }
                        } else if (m_Type == ExpeditionTaskType.ENTERZONE)
                        {
                            if (PlayerData.m_LevelGuid == m_Scene && Vector3.Distance(PlayerData.m_Position, m_ZoneCenter) <= m_ZoneRadius)
                            {
                                m_IsComplete = true;
                                return;
                            }
                        }
                    }
                }
            }
        }

        public static void RemoveGearSpawnerGear(int SearchKey)
        {
            if (m_GearSpawnerGears.ContainsKey(SearchKey))
            {
                string SpawnerGUID = m_GearSpawnerGears[SearchKey];
                if (m_UnavailableGearSpawners.ContainsKey(SpawnerGUID))
                {
                    m_UnavailableGearSpawners.Remove(SpawnerGUID);
                }
                m_GearSpawnerGears.Remove(SearchKey);
            }
        }

        public static void CreateRewardGear(ExpeditionGearSpawner Spawner, string Scene, bool DebugFlag = false)
        {
            if (!m_UnavailableGearSpawners.ContainsKey(Spawner.m_GUID))
            {
                if (Spawner.RollChance() || DebugFlag)
                {
                    
                    string Prefab = Spawner.PickGear();
                    Vector3 PlaceV3 = Spawner.m_Possition;
                    Quaternion Rotation = Spawner.m_Rotation;
                    int SearchKey;

                    SlicedJsonDroppedGear NewGear = new SlicedJsonDroppedGear();
                    NewGear.m_GearName = Prefab.ToLower();
                    NewGear.m_Extra.m_DroppedTime = MyMod.MinutesFromStartServer;
                    NewGear.m_Extra.m_Dropper = "";
                    NewGear.m_Extra.m_GearName = NewGear.m_GearName;
                    NewGear.m_Extra.m_Variant = 0;

                    int hashV3 = Shared.GetVectorHash(PlaceV3);
                    int hashRot = Shared.GetQuaternionHash(Rotation);
                    int hashLevelKey = Scene.GetHashCode();
                    SearchKey = hashV3 + hashRot + hashLevelKey;

                    DroppedGearItemDataPacket GearVisual = new DroppedGearItemDataPacket();
                    GearVisual.m_Extra = NewGear.m_Extra;
                    GearVisual.m_GearID = -1;
                    GearVisual.m_Hash = SearchKey;
                    GearVisual.m_LevelGUID = Scene;
                    GearVisual.m_Position = PlaceV3;
                    GearVisual.m_Rotation = Rotation;
                    NewGear.m_Json = "";
                    MPSaveManager.AddGearData(Scene, SearchKey, NewGear);
                    MPSaveManager.AddGearVisual(Scene, GearVisual);


                    m_UnavailableGearSpawners.Add(Spawner.m_GUID, SearchKey);

                    if (m_GearSpawnerGears.ContainsKey(SearchKey)) // Who knows, shit happends.
                    {
                        m_GearSpawnerGears.Remove(SearchKey);
                    }
                    m_GearSpawnerGears.Add(SearchKey, Spawner.m_GUID);

                    Shared.FakeDropItem(GearVisual, true);
                    ServerSend.DROPITEM(0, GearVisual, true);
                }
            }
        }
    }
}
