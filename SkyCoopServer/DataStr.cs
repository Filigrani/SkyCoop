using LiteNetLib;
using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using static SkyCoopServer.DataStr.DangerCircleData;

namespace SkyCoopServer
{
    public class DataStr
    {
        public class ServerConfig
        {
            public int m_MaxPlayers = 4;
            public string m_StartingRegion = "MarshRegion";
            public int m_Seed = 777777;
            //public int m_VoicePort = 37850;
            public int m_VoicePort = 0;
            public string m_ExperienceMode = "Stalker";
            public string m_SceneToSpawn = "MarshRegion";
            public string m_GameMode = "Lobby";
        }

        public class MapData
        {
            public string Scene { get; set; }
            public List<V3QuatJSON> SpawnPoints { get; set; }
            public V3QuatJSON VictoryPoint { get; set; }

            public DangerCircleConfig ZoneConfig { get; set; }

            public List<RadialLootSpawner> RadialLootSpawners { get; set; }

            public List<PropData> Props { get; set; }

            public MapData() 
            {
                Scene = "";
                SpawnPoints = new List<V3QuatJSON>();
                VictoryPoint = null;
                ZoneConfig = null;
                RadialLootSpawners = new List<RadialLootSpawner>();
                Props = new List<PropData>();
            }
        }

        public class GameRules
        {
            public List<string> m_Maps = new List<string>();
            public bool m_PlayerCanBeKnocked = false;
            public bool m_PVP = true;
            public List<StartingGearData> m_StartingItems = new List<StartingGearData>();
            public List<List<StartingGearData>> m_StartingItemsByTier = new List<List<StartingGearData>>();
            public int m_Time = 0;
            public int m_LootPerRadialSpawn = 5;
            public string m_HUDMode = "";
            public bool m_DeathPacks = false;
            public bool m_Respawns = false;
            public bool m_Clothing = false;
            public bool m_CanDropItems = true;
            public bool m_CanUseContainers = true;
            public bool m_CanUseMap = false;

            public string GetRandomMap(string CurrentMap = "")
            {
                List<string> MapPool = new List<string>(m_Maps);

                MapPool.Remove(CurrentMap);

                if(MapPool.Count == 0)
                {
                    return CurrentMap;
                }else if(MapPool.Count == 1)
                {
                    return MapPool[0];
                }
                else
                {
                    return MapPool[new System.Random(Guid.NewGuid().GetHashCode()).Next(0, MapPool.Count)];
                }
            }
        }

        public class GameRulesJson
        {
            public List<string> Maps { get; set; }
            public bool Knockdowns { get; set; }
            public bool PVP { get; set; }
            public List<StartingGearData> StartingGear { get; set; }
            public List<List<StartingGearData>> StartingGearByTier { get; set; }
            public int Time { get; set; }
            public int LootPerRadialSpawn { get; set; }
            public string HUDMode { get; set; }
            public bool DeathPacks { get; set; }
            public bool Respawns { get; set; }
            public bool Clothing { get; set; }
            public bool CanDropItems { get; set; }
            public bool CanUseContainers { get; set; }
            public bool CanUseMap { get; set; }

            public GameRules Load()
            {
                GameRules Inst = new GameRules();


                if (Maps != null)
                {
                    foreach (string Map in Maps)
                    {
                        Logger.Log($"    {Map}");
                        Inst.m_Maps.Add(Map);
                    }
                }

                Inst.m_PlayerCanBeKnocked = Knockdowns;

                if (StartingGear != null)
                {
                    foreach (StartingGearData GearData in StartingGear)
                    {
                        Inst.m_StartingItems.Add(GearData);
                    }
                }
                if (StartingGearByTier != null)
                {
                    foreach (List<StartingGearData> Tier in StartingGearByTier)
                    {
                        Inst.m_StartingItemsByTier.Add(new List<StartingGearData>(Tier));
                    }
                }
                if(Time != null)
                {
                    Inst.m_Time = Time;
                }
                if(LootPerRadialSpawn != null)
                {
                    Inst.m_LootPerRadialSpawn = LootPerRadialSpawn;
                }
                if (HUDMode != null)
                {
                    Inst.m_HUDMode = HUDMode;
                }

                Inst.m_DeathPacks = DeathPacks;
                Inst.m_Respawns = Respawns;
                Inst.m_Clothing = Clothing;
                Inst.m_CanDropItems = CanDropItems;
                Inst.m_CanUseContainers = CanUseContainers;
                Inst.m_CanUseMap = CanUseMap;


                return Inst;
            }
        }

        public class StartingGearData
        {
            public List<string> Variants { get; set; }
            public int Units { get; set; }

            public string Get()
            {
                int Count = Variants.Count;
                if (Count == 0)
                {
                    return "";
                }
                else if (Count == 1)
                {
                    return Variants[0];
                }
                else
                {
                    return Variants[new Random(Guid.NewGuid().GetHashCode()).Next(0, Count)];
                }
            }
            public StartingGearData() 
            {
                Variants = new List<string>();
                Units = 0;
            }
            public StartingGearData(string GearName, int Units = 1)
            {
                Variants = new List<string> { GearName };
                this.Units = Units;
            }
            public StartingGearData(List<string> GearVariants)
            {
                Variants = GearVariants;
            }
        }
        public class PlayerData
        {
            public string m_PlayerName = "Player";
            public int m_PlayerID = 0;
            public PlayerVisualData m_VisualData = new PlayerVisualData();

            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Quaternion m_Rotation = new Quaternion(0, 0, 0, 0);
            public float m_Tilt = 0;

            public string m_Scene = "";

            public List<Damager> m_Damagers = new List<Damager>();
            public int m_LastDamager = -1;
            public int m_PreLastDamager = -1;

            public int m_Kills = 0;
            public int m_Deaths = 0;
            public int m_Assists = 0;
            public int m_Tier = 0;
            public int m_TierProgress = 0;

            public string m_CarSeat = "";
            public string m_InteractionGUID = "";


            public GamePlayState m_GamePlayState = GamePlayState.Unassigned;

            public enum GamePlayState
            {
                Unassigned,
                Alive,
                Dead,
                Spectator,
            }

            public PlayerData(int PlayerID)
            {
                m_PlayerID = PlayerID;
            }

            public void SetGameplayState(GamePlayState State, Server ServerInstance)
            {
                m_GamePlayState = State;
                Logger.Log($"[DataStr.PlayerData] Client {m_GamePlayState} new gamepaly state {State}");

                foreach (PlayerData OtherPlayerData in ServerInstance.m_PlayersData.GetPlayersOnScene(m_Scene))
                {
                    if(OtherPlayerData != null)
                    {
                        NetPeer Player = ServerInstance.GetClient(m_PlayerID);
                        NetPeer OtherPlayer = ServerInstance.GetClient(OtherPlayerData.m_PlayerID);

                        if(Player != OtherPlayer || ServerInstance.m_PlayersData.m_RecursiveDebug)
                        {
                            switch (m_GamePlayState)
                            {
                                case GamePlayState.Unassigned:
                                    ServerSend.SendPlayerSceneNotification(OtherPlayer, false, m_PlayerID);
                                    break;
                                case GamePlayState.Alive:
                                    ServerSend.SendPlayerSceneNotification(OtherPlayer, true, m_PlayerID);
                                    ServerSend.SendPlayerAction(OtherPlayer, 0, m_PlayerID);
                                    ServerSend.SendPlayerCrouch(OtherPlayer, false, m_PlayerID);
                                    break;
                                case GamePlayState.Dead:
                                    ServerSend.SendPlayerAction(OtherPlayer, 5, m_PlayerID);
                                    ServerSend.SendRemoveAllInjectedItem(m_PlayerID, ServerInstance);
                                    ServerInstance.m_PlayersData.PlayerChangeGear(m_PlayerID, "", 0, true);
                                    break;
                                case GamePlayState.Spectator:
                                    ServerSend.SendPlayerAction(OtherPlayer, 5, m_PlayerID);
                                    ServerInstance.m_PlayersData.PlayerChangeGear(m_PlayerID, "", 0, true);
                                    ServerSend.SendPlayerSceneNotification(OtherPlayer, false, m_PlayerID);
                                    ServerSend.SendRemoveAllInjectedItem(m_PlayerID, ServerInstance);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            public void DealDamage(int Killer, float Damage, DamageType DamageType)
            {
                if(m_GamePlayState != GamePlayState.Alive)
                {
                    return;
                }
                for (int i = 0; i < m_Damagers.Count; i++)
                {
                    Damager damager = m_Damagers[i];
                    if (damager.m_ClientID == Killer)
                    {
                        damager.m_Damage += Damage;
                        damager.m_DamageType = DamageType;
                        m_Damagers.RemoveAt(i);
                        m_Damagers.Add(damager);
                        return;
                    }
                }
                
                m_Damagers.Add(new Damager(Killer, Damage, DamageType));
            }

            public void KillFeedDebugLog(List<Damager> Damagers)
            {
                for (int i = 0; i < m_Damagers.Count; i++)
                {
                    Damager Dmg = m_Damagers[i];
                    Logger.Log("[DataStr] "+i+". PlayerID " + Dmg.m_ClientID+" Damage: "+ Dmg.m_Damage +" Type "+Dmg.m_DamageType.ToString());
                }
            }

            public void ConfirmKill(Server ServerInstance, DamageType DamageType, bool Knocked = false, bool HeadShot = false) 
            {
                if (m_GamePlayState != GamePlayState.Alive)
                {
                    return;
                }
                if (!Knocked)
                {
                    
                    if (ServerInstance.m_Rules.m_Respawns)
                    {
                        SetGameplayState(GamePlayState.Dead, ServerInstance);
                    }
                    else
                    {
                        SetGameplayState(GamePlayState.Spectator, ServerInstance);
                    }

                    if(ServerInstance.m_Rules.m_HUDMode == "Shrink")
                    {
                        List<NetPeer> peers = new List<NetPeer>();
                        ServerInstance.m_Instance.GetConnectedPeers(peers);
                        foreach (NetPeer Peer in peers.ToArray())
                        {
                            ServerSend.SendHUDSideBarUpdate(Peer, 1, ServerInstance.m_PlayersData.GetShrinkModeString(), ServerInstance);
                        }
                    }
                }
                DataStr.KillFeedMessage Message = new KillFeedMessage();
                Message.m_Victim = m_PlayerID;
                Message.m_DeathReason = DamageType;

                if (Knocked)
                {
                    Message.m_Flags.Add(KillFeedFlag.Knocked);
                }
                if (HeadShot)
                {
                    Message.m_Flags.Add(KillFeedFlag.HeadShot);
                }

                if (!Knocked)
                {
                    PlayersSquad Squad = ServerInstance.m_PlayersData.GetPlayerSquadIn(m_PlayerID);

                    if (Squad != null)
                    {
                        ServerSend.SendSquadEliminated(ServerInstance, Squad.m_Name);
                    }
                    ServerInstance.m_PlayersData.DoSquadsCheck();
                }

                if (m_Damagers.Count > 0)
                {
                    Damager LastDamager = m_Damagers[m_Damagers.Count - 1];
                    Damager[] Unordered = new Damager[m_Damagers.Count];
                    m_Damagers.CopyTo(Unordered);
                    m_Damagers.Sort();

                    // If player bleeds to death, or finish himself, confirm kill, only for last damager.
                    if (DamageType == DamageType.BloodLoss)
                    {
                        Message.m_Killer = LastDamager.m_ClientID;
                        if (!Knocked)
                        {
                            KillFeedDebugLog(m_Damagers);
                            m_Damagers.Clear();
                        }
                        else
                        {
                            m_Damagers = Unordered.ToList();
                        }
                        ServerSend.SendKillFeed(Message, ServerInstance);
                        return;
                    }
                    else if(DamageType == DamageType.Unknown)
                    {
                        Message.m_Killer = LastDamager.m_ClientID;
                        Message.m_Flags.Add(KillFeedFlag.HelpedToDie);
                        if (!Knocked)
                        {
                            KillFeedDebugLog(m_Damagers);
                            m_Damagers.Clear();
                        }
                        else
                        {
                            m_Damagers = Unordered.ToList();
                        }
                        ServerSend.SendKillFeed(Message, ServerInstance);
                        return;
                    }

                    Damager HighestDamage = m_Damagers[0];

                    if (m_Damagers.Count > 1)
                    {
                        if(HighestDamage.m_ClientID == LastDamager.m_ClientID)
                        {
                            Message.m_Killer = LastDamager.m_ClientID;
                            Message.m_Assist = m_Damagers[1].m_ClientID;
                        }
                        else
                        {
                            Message.m_Killer = LastDamager.m_ClientID;
                            Message.m_Assist = HighestDamage.m_ClientID;
                        }
                    }
                    else
                    {
                        Message.m_Killer = LastDamager.m_ClientID;
                    }

                    if (!Knocked)
                    {
                        KillFeedDebugLog(m_Damagers);
                        m_Damagers.Clear();
                    }
                    else
                    {
                        m_Damagers = Unordered.ToList();
                    }
                    ServerSend.SendKillFeed(Message, ServerInstance);
                }
                else
                {
                    Message.m_Killer = m_PlayerID;
                    Message.m_DeathReason = DamageType.Unknown;
                    ServerSend.SendKillFeed(Message, ServerInstance);
                    Logger.Log("[DataStr] Suicide, nothing to log");
                }
            }

            public void Revived(int Reviver, Server ServerInstance)
            {
                //if(Reviver == m_PlayerID)
                //{
                //    Logger.log($"[DataStr] Player {m_PlayerID} revived himself.");
                //}else if(Reviver == -1)
                //{
                //    Logger.log($"[DataStr] Player {m_PlayerID} respawned.");
                //}else
                //{
                //    Logger.log($"[DataStr] Player {m_PlayerID} revived by Player {Reviver}");
                //}

                if (Reviver == -2)
                {
                    SetGameplayState(GamePlayState.Alive, ServerInstance);
                    if (ServerInstance.m_Rules.m_HUDMode == "Shrink")
                    {
                        List<NetPeer> peers = new List<NetPeer>();
                        ServerInstance.m_Instance.GetConnectedPeers(peers);
                        foreach (NetPeer Peer in peers.ToArray())
                        {
                            ServerSend.SendHUDSideBarUpdate(Peer, 1, ServerInstance.m_PlayersData.GetShrinkModeString(), ServerInstance);
                        }
                    }
                }

                m_Damagers.Clear();
            }

            public void AddKill(Server ServerInstance)
            {
                m_Kills++;
                if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "DMStats" || ServerInstance.m_Rules.m_HUDMode == "Shrink")
                {
                    ServerSend.SendHUDSideBarUpdate(ServerInstance.GetClient(m_PlayerID), 0, m_Kills.ToString(), ServerInstance);
                }
                if(ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "GunGame")
                {
                    m_TierProgress += 1;

                    if (m_TierProgress > 2)
                    {
                        m_TierProgress = 0;
                        AddTier(ServerInstance);
                    }
                    else
                    {
                        ServerSend.SendHUDSideBarUpdate(ServerInstance.GetClient(m_PlayerID), 1, GetTierProgressString(ServerInstance), ServerInstance);
                    }
                }
            }

            public void RemoveKill(Server ServerInstance)
            {
                m_Kills--;
                if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "DMStats")
                {
                    ServerSend.SendHUDSideBarUpdate(ServerInstance.GetClient(m_PlayerID), 0, m_Kills.ToString(), ServerInstance);
                }
            }

            public void AddDeath(Server ServerInstance)
            {
                m_Deaths++;
                if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "DMStats")
                {
                    ServerSend.SendHUDSideBarUpdate(ServerInstance.GetClient(m_PlayerID), 1, m_Deaths.ToString(), ServerInstance);
                }
            }

            public void AddAssist(Server ServerInstance)
            {
                m_Assists++;
                if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "DMStats")
                {
                    ServerSend.SendHUDSideBarUpdate(ServerInstance.GetClient(m_PlayerID), 2, m_Assists.ToString(), ServerInstance);
                }
            }

            public void AddTier(Server ServerInstance)
            {
                if(ServerInstance.m_Rules.m_StartingItemsByTier != null)
                {
                    int MaxTier = ServerInstance.m_Rules.m_StartingItemsByTier.Count - 1;

                    if(m_Tier < MaxTier)
                    {
                        m_Tier++;

                        ServerSend.SendTier(ServerInstance.GetClient(m_PlayerID), m_Tier);

                        if(ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "GunGame")
                        {
                            ServerSend.SendHUDSideBarUpdate(ServerInstance.GetClient(m_PlayerID), 0, GetTierString(ServerInstance), ServerInstance);
                            ServerSend.SendHUDSideBarUpdate(ServerInstance.GetClient(m_PlayerID), 1, GetTierProgressString(ServerInstance), ServerInstance);
                        }
                    }
                    else
                    {
                        if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "GunGame")
                        {
                            ServerInstance.ForceToOver();
                        }
                    }
                }
            }

            public void RemoveTier(Server ServerInstance)
            {
                if (ServerInstance.m_Rules.m_StartingItemsByTier != null)
                {
                    if (m_Tier > 0)
                    {
                        m_Tier--;
                        m_TierProgress = 0;
                        ServerSend.SendTier(ServerInstance.GetClient(m_PlayerID), m_Tier);

                        if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "GunGame")
                        {
                            ServerSend.SendHUDSideBarUpdate(ServerInstance.GetClient(m_PlayerID), 0, GetTierString(ServerInstance), ServerInstance);
                            ServerSend.SendHUDSideBarUpdate(ServerInstance.GetClient(m_PlayerID), 1, GetTierProgressString(ServerInstance), ServerInstance);
                        }
                    }
                }
            }

            public string GetTierString(Server ServerInstance)
            {
                string Tier = (m_Tier+1) + "/";

                if(ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_StartingItemsByTier.Count > 0)
                {
                    return (m_Tier+1) + "/" + ServerInstance.m_Rules.m_StartingItemsByTier.Count;
                }

                return Tier.ToString();
            }
            public string GetTierProgressString(Server ServerInstance)
            {
                if(m_TierProgress == 0)
                {
                    return "[707070]OOO[-]";
                }else if(m_TierProgress == 1)
                {
                    return "X[707070]OO[-]";
                }else if(m_TierProgress == 2)
                {
                    return "XX[707070]O[-]";
                }
                else
                {
                    return "[707070]OOO[-]";
                }
            }
        }

        public class PlayerVisualData
        {
            public bool m_Crouch = false;
            public bool m_InVehicle = false;
            public string m_GearInHands = "";
            public int m_GearVariant = 0;
            public int m_LastAction = 0;
            public List<InjectedItem> m_InjectedItems = new List<InjectedItem>();
            public DataStr.ClothingData m_ClothingData = new DataStr.ClothingData();

        }

        public class GearDataVisual
        {
            public string m_GearName = "";
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Quaternion m_Rotation = new Quaternion(0, 0, 0, 0);
            public string m_GUID = "";
        }
        public class GearData
        {
            public string m_GUID = "";
            public string m_JSON = "";
        }

        public class GearDataContainer
        {
            public GearDataVisual m_Visual = new GearDataVisual();
            public GearData m_Data = new GearData();
        }

        public class SceneData
        {
            public string m_SceneName = "";
            public Dictionary<string, GearDataContainer> m_Gears = new Dictionary<string, GearDataContainer>();
            public Dictionary<string, bool> m_Openables = new Dictionary<string, bool>();
            public Dictionary<string, DeathPack> m_DeathPacks = new Dictionary<string, DeathPack>();
            public Dictionary<string, string> m_Containers = new Dictionary<string, string>();
            public Dictionary<string, int> m_ContainerStats = new Dictionary<string, int>();
            
            public Dictionary<string, PropData> m_Props = new Dictionary<string, PropData>();
            public List<V3Quat> m_SpawnPoints = new List<V3Quat>();
            public List<RadialLootSpawner> m_RadialLootSpawners = new List<RadialLootSpawner>();

            public DangerCircleConfig m_ZoneConfig = null;
            public DangerCircleData m_ActiveZone = null;
            public V3Quat m_VictoryPoint = new V3Quat();

            public void Unload()
            {
                if (m_ActiveZone != null)
                {
                    // TO DO Диспоснуть текущую зону, ибо следующая карта может не иметь зоны.
                    // Нужно ещё отправить клиенту сигнла что бы он снёс зону у себя тоже.
                }
            }

            public List<Vector3> GetGearSpawnersMarkers()
            {
                List<Vector3> Points = new List<Vector3>();
                if(m_RadialLootSpawners != null)
                {
                    foreach (RadialLootSpawner Spawner in m_RadialLootSpawners)
                    {
                        if (Spawner != null)
                        {
                            Points.Add(Spawner.center.ToVector());
                        }
                    }
                }
                return Points;
            }

            public void LoadMapData(Server ServerInstance, MapData MapData)
            {
                SkyCoopServer.Logger.Log($"Trying to load Map");
                if (MapData != null)
                {
                    if(MapData.Scene != null)
                    {
                        m_SceneName = MapData.Scene;
                    }

                    m_SpawnPoints.Clear();
                    if (MapData.SpawnPoints != null)
                    {
                        foreach (V3QuatJSON Point in MapData.SpawnPoints)
                        {
                            m_SpawnPoints.Add(new V3Quat(Point.position, Point.rotation));
                        }
                    }
                    else
                    {
                        // Если не задали ни одного спавно, добавляем одну точку на нулях. Если игрок получит 0 0 0 он сам выберет точку.
                        m_SpawnPoints.Add(new V3Quat());
                    }


                    SkyCoopServer.Logger.Log($"Trying to load ZoneConfig");
                    m_ZoneConfig = MapData.ZoneConfig;

                    if(m_ZoneConfig != null)
                    {
                        SkyCoopServer.Logger.Log($"ZoneConfig found, creating zone...");
                        m_ActiveZone = new DangerCircleData(m_ZoneConfig, m_SceneName, ServerInstance);
                        m_ActiveZone.Start();
                    }
                    else
                    {
                        SkyCoopServer.Logger.Log($"ZoneConfig is null");
                    }

                    m_Props.Clear();
                    if (MapData.Props != null)
                    {
                        foreach (PropData Prop in MapData.Props)
                        {
                            m_Props.Add(Prop.guid, Prop);
                            List<NetPeer> peers = new List<NetPeer>();
                            ServerInstance.m_Instance.GetConnectedPeers(peers);
                            foreach (NetPeer Peer in peers.ToArray())
                            {
                                if (ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == m_SceneName)
                                {
                                    ServerSend.SendPropCreated(Peer, Prop);
                                }
                            }
                        }
                    }

                    m_RadialLootSpawners.Clear();
                    if (MapData.RadialLootSpawners != null)
                    {
                        foreach (RadialLootSpawner LootSpawner in MapData.RadialLootSpawners)
                        {
                            m_RadialLootSpawners.Add(LootSpawner);
                        }
                    }

                    if(ServerInstance.m_Rules.m_LootPerRadialSpawn != null && ServerInstance.m_Rules.m_LootPerRadialSpawn > 0)
                    {
                        PopulateLoot(ServerInstance, ServerInstance.m_Rules.m_LootPerRadialSpawn);
                    }
                    else
                    {
                        PopulateLoot(ServerInstance);
                    }


                    if (MapData.VictoryPoint != null)
                    {
                        m_VictoryPoint = new V3Quat(MapData.VictoryPoint.position, MapData.VictoryPoint.rotation);
                    }
                }
            }

            public void PopulateLoot(Server ServerInstance, int LootPerPoint = 5)
            {
                if (LootPerPoint == null || LootPerPoint == 0)
                {
                    return;
                }

                //SkyCoopServer.Logger.Log(ConsoleColor.Cyan, $"Trying populate loot on {m_SceneName}");

                int PointIndex = 0;
                foreach (RadialLootSpawner Spawner in m_RadialLootSpawners)
                {
                    if (Spawner != null)
                    {
                        List<Vector3JSON> AvaliablePoints = new List<Vector3JSON>();

                        if (Spawner.points != null)
                        {
                            if (Spawner.points.Count < LootPerPoint)
                            {
                                LootPerPoint = Spawner.points.Count;
                            }

                            AvaliablePoints = Spawner.points;

                            Random RNG = new Random(Guid.NewGuid().GetHashCode());

                            for (int i = 1; i <= LootPerPoint; i++)
                            {
                                int Index = RNG.Range(0, AvaliablePoints.Count);

                                Vector3 Point = AvaliablePoints[Index].ToVector();

                                string LootTableName = "Main"; // Full random.

                                if (!string.IsNullOrEmpty(Spawner.loottable))
                                {
                                    LootTableName = Spawner.loottable;
                                }

                                string GearName = LootTableManager.GetRandomLoot(LootTableName);

                                //SkyCoopServer.Logger.Log($"[PopulateLoot] {m_SceneName} Point {PointIndex}({i}/{LootPerPoint}) picked {GearName}");

                                ServerInstance.m_ScenesData.AddGear(m_SceneName, GearName, Point, Extensions.Euler(0, RNG.Range(0, 360), 0), string.Empty);
                                AvaliablePoints.RemoveAt(Index);
                            }
                        }
                    }
                    PointIndex++;
                }
            }
        }

        public struct DMScore : IComparable<DMScore>
        {
            public int PlayerID;
            
            public int Kills;
            public int Assits;
            public int Deaths;
            public int Bonus;

            public DMScore(int ID, int kills, int assists, int deaths, int bonus = 0)
            {
                PlayerID = ID;
                Kills = kills;
                Assits = assists;
                Deaths = deaths;
                Bonus = 0;
            }

            public int GetFinalScore()
            {
                return Kills + ((int)MathF.Floor(Assits * 0.5f)) - Deaths + Bonus;
            }

            public int CompareTo(DMScore other)
            {
                return other.GetFinalScore().CompareTo(GetFinalScore());
            }
        }

        public struct Damager : IComparable<Damager>
        {
            public int m_ClientID;
            public float m_Damage;
            public DamageType m_DamageType;

            public Damager(int ClientID, float Damage,  DamageType DamageType)
            {
                m_ClientID = ClientID;
                m_Damage = Damage;
                m_DamageType = DamageType;
            }

            public int CompareTo(Damager other)
            {
                return other.m_Damage.CompareTo(m_Damage);
            }
        }

        public enum KillFeedFlag
        {
            HeadShot = 0,
            Knocked = 1,
            HelpedToDie = 2,
        }

        public class KillFeedMessage
        {
            public int m_Killer = -1;
            public int m_Victim = -1;
            public int m_Assist = -1;
            public DamageType m_DeathReason = DamageType.Unknown;
            public List<KillFeedFlag> m_Flags = new List<KillFeedFlag>();
        }

        public enum DamageType
        {
            Unknown,
            Revolver,
            Rifle,
            FlareGun,
            Bow,
            BloodLoss,
            Hatchet,
            Knife,
            Prybar,
            Hammer,
            NoiseMaker,
            Stone,
        }

        public class V3Quat
        {
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Quaternion m_Rotation = new Quaternion(0, 0, 0, 0);

            public V3Quat(float posx, float posy, float posz, float rotx, float roty, float rotz, float rotw)
            {
                m_Position = new Vector3(posx, posy, posz);
                m_Rotation = new Quaternion(rotx, roty, rotz, rotw);
            }

            public V3Quat(Vector3JSON Position, QuaternionJSON Rotation)
            {
                m_Position = Position.ToVector();
                m_Rotation = Rotation.ToQuaternion();
            }

            public V3Quat() { }
        }

        public class V3QuatJSON
        {
            public Vector3JSON position { get; set; }

            public QuaternionJSON rotation { get; set; }

            public V3QuatJSON() 
            {
                position = new Vector3JSON();
                rotation = new QuaternionJSON();
            }
        }

        public class InjectedItem
        {
            public string m_GearName = "";
            public int m_DamageZone = 0;
            public int m_ObjectID = 0;
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Quaternion m_Rotation = new Quaternion(0, 0, 0, 0);
        }

        public class ShrinkStage
        {
            public float Radius { get; set; }
            public float ShrinkTime { get; set; }
            public int StageTime { get; set; }
            public float DamagePerSecond { get; set; }

            public ShrinkStage()
            {
                Radius = 0;
                ShrinkTime = 0;
                StageTime = 0;
                DamagePerSecond = 35;
            }
        }

        public class DangerCircleData
        {
            public DangerCircleConfig m_Config = new DangerCircleConfig();
            public int m_CurrentStageIndex = 0;
            public ShrinkStage m_CurrentStage = null;

            public float m_CurrentRadius = 0;
            public Vector3 m_CurrentCenter = Vector3.Zero;
            public Vector3 m_NextCenter = Vector3.Zero;
            public Vector3 m_NewCenterToReach = Vector3.Zero;

            public State m_State = State.Waiting;

            public bool m_DebugNoDamage = false;

            private DateTime s_NextDamageCheck;

            public enum State
            {
                Waiting,
                Shrinking,
                Finished,
            }

            private DateTime s_ShrinkStarted;
            private DateTime s_StateTimer;
            private bool s_StateTimerActive = false;
            private string s_SceneName = "";
            private Server s_ServerInstance;
            private float s_PreviousStateRadius = 0;
            private Vector3 s_PreviousStateCenter = Vector3.Zero;

            public string GetTimerPrefix()
            {
                switch (m_State)
                {
                    case State.Waiting:
                    case State.Shrinking:
                        return "GAMEPLAY_TimeRemainingZone";
                    case State.Finished:
                        return "GAMEPLAY_TimeRemaining";
                    default:
                        return "GAMEPLAY_TimeRemaining";
                }
            }

            public int GetTimerSeconds()
            {
                switch (m_State)
                {
                    case State.Waiting:
                        return (int)(s_StateTimer - DateTime.Now).TotalSeconds;
                    case State.Shrinking:
                        if(m_CurrentStageIndex == m_Config.Stages.Count - 1)
                        {
                            return 0;
                        }
                        else
                        {
                            return (int)(s_StateTimer - DateTime.Now).TotalSeconds + m_Config.Stages[m_CurrentStageIndex+1].StageTime;
                        }
                    case State.Finished:
                        return 0;
                    default:
                        return 0;
                }
            }

            public DangerCircleData(){}

            public DangerCircleData(DangerCircleConfig Config, string SceneName, Server Server)
            {
                m_Config = Config;
                s_SceneName = SceneName;
                s_ServerInstance = Server;
                s_NextDamageCheck = DateTime.Now.AddSeconds(1);
                m_CurrentCenter = Config.ActualCenter.ToVector();
            }

            public void NextState()
            {
                switch (m_State)
                {
                    case State.Waiting:
                        s_PreviousStateRadius = m_CurrentRadius;
                        s_PreviousStateCenter = m_CurrentCenter;
                        m_NewCenterToReach = m_NextCenter;
                        s_ShrinkStarted = DateTime.Now;
                        SetNextStage();
                        m_State = State.Shrinking;
                        s_StateTimer = DateTime.Now.AddSeconds(m_CurrentStage.ShrinkTime);
                        s_StateTimerActive = true;
                        break;
                    case State.Shrinking:
                        m_CurrentCenter = GetNextCenter();
                        m_CurrentRadius = GetNextRadius();
                        if(m_CurrentStage.StageTime <= 0)
                        {
                            m_State = State.Finished;
                            s_StateTimerActive = false;
                        }
                        else
                        {
                            m_State = State.Waiting;
                            s_StateTimer = DateTime.Now.AddSeconds(m_CurrentStage.StageTime);
                            m_NextCenter = GetNewRandomCenter(m_CurrentCenter, m_CurrentRadius, m_Config.Stages[m_CurrentStageIndex + 1].Radius);
                            s_StateTimerActive = true;
                        }
                        break;
                }
            }

            public void Start()
            {
                SetStage(0);

                m_CurrentRadius = m_CurrentStage.Radius;
                s_PreviousStateRadius = m_CurrentStage.Radius;

                m_CurrentCenter = m_Config.ActualCenter.ToVector();
                m_NewCenterToReach = m_CurrentCenter;
                s_PreviousStateCenter = m_CurrentCenter;
                s_StateTimerActive = false;

                if (m_Config.Stages.Count == 1)
                {
                    m_State = State.Finished;
                }
                else
                {
                    m_State = State.Waiting;
                    s_StateTimer = DateTime.Now.AddSeconds(m_CurrentStage.StageTime);
                    m_NextCenter = GetNewRandomCenter(m_CurrentCenter, m_CurrentRadius, m_Config.Stages[m_CurrentStageIndex + 1].Radius);
                    s_StateTimerActive = true;
                }
                ServerSend.SendZoneUpdate(s_SceneName, m_CurrentCenter, m_CurrentRadius, GetNextCenter(), GetNextRadius(), s_ServerInstance);
            }

            public static float Lerp(float a, float b, float t)
            {
                return a + (b - a) * t;
            }

            public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
            {
                return new Vector3(
                    Lerp(a.X, b.X, t),
                    Lerp(a.Y, b.Y, t),
                    Lerp(a.Z, b.Z, t)
                );
            }

            public float GetNextRadius()
            {
                if (m_State == State.Shrinking)
                {
                    return m_CurrentStage.Radius;
                } else if(m_State == State.Waiting)
                {
                    if(m_CurrentStageIndex == m_Config.Stages.Count-1)
                    {
                        return 0;
                    }
                    else
                    {
                        return m_Config.Stages[m_CurrentStageIndex+1].Radius;
                    }
                }
                else
                {
                    return 0;
                }
            }

            public Vector3 GetNextCenter()
            {
                if(m_State == State.Waiting)
                {
                    return m_NextCenter;
                }
                else if(m_State == State.Shrinking)
                {
                    return m_NewCenterToReach;
                }
                else
                {
                    return Vector3.Zero;
                }
            }

            public void ForceNextZone()
            {
                if (s_StateTimerActive)
                {
                    s_StateTimer = DateTime.Now;
                }
            }

            public void ToggleNoDamage()
            {
                m_DebugNoDamage = !m_DebugNoDamage;
            }

            public void Restart()
            {
                Start();
            }

            public Vector3 GetNewRandomCenter(Vector3 CurrentCenter, float CurrentRadius, float FutureRadius)
            {
                float maxDistance = CurrentRadius - FutureRadius;
                if (maxDistance <= 0)
                {
                    return CurrentCenter;
                }

                maxDistance = maxDistance / 2;

                //SkyCoopServer.Logger.Log($"CurrentCenter X {CurrentCenter.X}, {CurrentCenter.Y}, {CurrentCenter.Z}");
                //SkyCoopServer.Logger.Log($"CurrentRadius {CurrentRadius}");
                //SkyCoopServer.Logger.Log($"FutureRadius {FutureRadius}");

                Random RND = new Random(Guid.NewGuid().GetHashCode());

                float distance = maxDistance * (float)Math.Pow(RND.NextDouble(), 1.0 / 3.0);
                float theta = RND.Range(0f, (float)(2 * Math.PI));
                float phi = RND.Range(0f, (float)Math.PI);

                float offsetX = distance * (float)Math.Sin(phi) * (float)Math.Cos(theta);
                float offsetY = distance * (float)Math.Sin(phi) * (float)Math.Sin(theta);
                float offsetZ = distance * (float)Math.Cos(phi);

                //SkyCoopServer.Logger.Log($"offsetX {offsetX}");
                //SkyCoopServer.Logger.Log($"offsetY {offsetY}");
                //SkyCoopServer.Logger.Log($"offsetZ {offsetZ}");
                //SkyCoopServer.Logger.Log($"distance {distance}");

                Vector3 NewCenter = new Vector3(
                    CurrentCenter.X + offsetX,
                    CurrentCenter.Y + offsetY,
                    CurrentCenter.Z + offsetZ
                );

                float actualDistance = (float)Math.Sqrt(
                    Math.Pow(NewCenter.X - CurrentCenter.X, 2) +
                    Math.Pow(NewCenter.Y - CurrentCenter.Y, 2) +
                    Math.Pow(NewCenter.Z - CurrentCenter.Z, 2)
                );

                //SkyCoopServer.Logger.Log($"Actual distance from old center: {actualDistance}");
                //SkyCoopServer.Logger.Log($"Max allowed distance: {maxDistance}");
                //SkyCoopServer.Logger.Log($"NewCenter X {NewCenter.X}, {NewCenter.Y}, {NewCenter.Z}");

                return NewCenter;
            }

            public void Update(float dt)
            {
                //Logger.Log($"Zone Update: SceneName {s_SceneName}");

                if (s_NextDamageCheck < DateTime.Now)
                {
                    
                    s_NextDamageCheck = DateTime.Now.AddSeconds(1);
                    DamageCheck();
                    if (s_ServerInstance.m_Rules.m_Time == 0)
                    {
                        ServerSend.ClientGameModeTimer(GetTimerSeconds(), s_ServerInstance);
                    }
                }

                if (s_StateTimerActive)
                {
                    if (s_StateTimer < DateTime.Now)
                    {
                        NextState();
                        ServerSend.UpdateTimerPrefix(GetTimerPrefix(), s_ServerInstance);
                        ServerSend.ClientGameModeTimer(GetTimerSeconds(), s_ServerInstance);
                        ServerSend.SendZoneUpdate(s_SceneName, m_CurrentCenter, m_CurrentRadius, GetNextCenter(), GetNextRadius(), s_ServerInstance);
                    }
                }

                float OldRadius = m_CurrentRadius;

                if (m_State == State.Shrinking)
                {
                    float totalDuration = (float)(s_StateTimer - s_ShrinkStarted).TotalSeconds;
                    float elapsed = (float)(DateTime.Now - s_ShrinkStarted).TotalSeconds;
                    float progress = elapsed / totalDuration;

                    m_CurrentRadius = Lerp(s_PreviousStateRadius, m_CurrentStage.Radius, progress);
                    m_CurrentCenter = Lerp(s_PreviousStateCenter, m_NewCenterToReach, progress);
                }
                else
                {
                    m_CurrentRadius = m_CurrentStage.Radius;
                    m_CurrentCenter = m_NewCenterToReach;
                }

                if (OldRadius != m_CurrentRadius)
                {
                    //Logger.Log($"Zone New Radius {m_CurrentRadius} m_CurrentStage.Radius {m_CurrentStage.Radius}");
                    ServerSend.SendZoneUpdate(s_SceneName, m_CurrentCenter, m_CurrentRadius, GetNextCenter(), GetNextRadius(), s_ServerInstance);
                }
            }

            public void DamageCheck()
            {
                if (m_DebugNoDamage)
                {
                    return;
                }
                
                List<NetPeer> peers = new List<NetPeer>();
                s_ServerInstance.m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Peer in peers.ToList())
                {
                    PlayerData PlayerData = s_ServerInstance.GetPlayerDataByNetPeer(Peer);
                    if(PlayerData.m_GamePlayState == PlayerData.GamePlayState.Alive)
                    {
                        float Distance = Vector2.Distance(new Vector2(PlayerData.m_Position.X, PlayerData.m_Position.Z), new Vector2(m_CurrentCenter.X, m_CurrentCenter.Z));
                        //SkyCoopServer.Logger.Log($"DangerCircleData PlayerID {Peer.Id} Distance {Distance}/{m_CurrentRadius/2}");
                        if (Distance  > m_CurrentRadius/2)
                        {
                            ServerSend.SendDamageToPlayer(Peer, m_CurrentStage.DamagePerSecond, Peer.Id, 1, "ZONE");
                        }
                    }
                }
            }

            void SetStage(int Index)
            {
                m_CurrentStageIndex = Index;
                m_CurrentStage = m_Config.Stages[Index];
            }

            void SetNextStage()
            {
                if (m_Config != null && m_Config.Stages != null)
                {
                    if (m_CurrentStageIndex + 1 < m_Config.Stages.Count)
                    {
                        m_CurrentStageIndex++;
                    }
                    SetStage(m_CurrentStageIndex);
                }
            }
        }

        public class DangerCircleConfig
        {
            public Vector3JSON ActualCenter { get; set; }
            public List<ShrinkStage> Stages { get; set; }

            public DangerCircleConfig()
            {
                ActualCenter = new Vector3JSON();
                Stages = new List<ShrinkStage>();
            }
        }

        public class ClothingData
        {
            public string m_Hat1 = "";
            public string m_Hat2 = "";
            public string m_Body = "";
            public string m_Gloves = "";
            public string m_Pants = "";
            public string m_Boots = "";

            public string m_Accs1 = "";
            public string m_Accs2 = "";

            public float m_Hat1Damage = 0;
            public float m_Hat2Damage = 0;
            public float m_BodyDamage = 0;
            public float m_GlovesDamage = 0;
            public float m_PantsDamage = 0;
            public float m_BootsDamage = 0;

            public bool m_TechPack = false;

            public ClothingData GetCopy()
            {
                ClothingData Clone = new ClothingData();

                Clone.m_Hat1 = m_Hat1;
                Clone.m_Hat2 = m_Hat2;
                Clone.m_Body = m_Body;
                Clone.m_Gloves = m_Gloves;
                Clone.m_Pants = m_Pants;
                Clone.m_Boots = m_Boots;

                Clone.m_Accs1 = m_Accs1;
                Clone.m_Accs2 = m_Accs2;

                Clone.m_Hat1Damage = m_Hat1Damage;
                Clone.m_Hat2Damage = m_Hat2Damage;
                Clone.m_BodyDamage = m_BodyDamage;
                Clone.m_GlovesDamage = m_GlovesDamage;
                Clone.m_PantsDamage = m_PantsDamage;
                Clone.m_BootsDamage = m_BootsDamage;

                Clone.m_TechPack = m_TechPack;

                return Clone;
            }

            public bool Equals(ClothingData Other)
            {
                if(m_Hat1 == Other.m_Hat1 
                    && m_Hat2 == Other.m_Hat2
                    && m_Body == Other.m_Body
                    && m_Gloves == Other.m_Gloves
                    && m_Pants == Other.m_Pants
                    && m_Boots == Other.m_Boots
                    && m_Accs1 == Other.m_Accs1
                    && m_Accs2 == Other.m_Accs2
                    && m_TechPack == Other.m_TechPack)
                {
                    return true;
                }
                return false;
            }

            public bool HasThis(string GearName)
            {
                if (m_Hat1 == GearName
                    || m_Hat2 == GearName
                    || m_Body == GearName
                    || m_Gloves == GearName
                    || m_Pants == GearName
                    || m_Boots == GearName
                    || m_Accs1 == GearName
                    || m_Accs2 == GearName)
                {
                    return true;
                }
                return false;
            }
        }

        public class DeathPack
        {
            public string m_Prefab = "";
            public string m_GUID = "";
            public string m_Owner = "";
            public Vector3 m_Position;
            public Quaternion m_Rotation;
        }

        public enum PlayerHearing
        {
            None = 0,
            Proximity = 1,
            Global = 2,
            Radio = 3,
            Anoncer = 4,
        }

        public class PlayersSquad
        {
            public string m_Name = "";
            public List<int> m_Players = new List<int>();
            public List<int> m_Invites = new List<int>();

            public PlayersSquad(string SquadName)
            {
                m_Name = SquadName;
            }

            public bool HasPlayer(int PlayerID)
            {
                return m_Players.Contains(PlayerID);
            }

            public bool PlayerIsInvited(int PlayerID)
            {
                return m_Invites.Contains(PlayerID);
            }

            public bool AddPlayer(int PlayerID, Server ServerInstance)
            {
                if (!m_Players.Contains(PlayerID))
                {
                    m_Players.Add(PlayerID);

                    RemoveInvite(PlayerID);

                    ServerSend.SendAssignSquad(ServerInstance.GetClient(PlayerID), true);

                    foreach (int TeammateID in m_Players.ToList())
                    {
                        NetPeer TeamatePeer = ServerInstance.GetClient(TeammateID);

                        if (TeamatePeer != null)
                        {
                            ServerSend.SendSquadHealthRequest(TeamatePeer);
                        }
                    }
                    if (ServerInstance.m_Rules.m_HUDMode == "Shrink")
                    {
                        List<NetPeer> peers = new List<NetPeer>();
                        ServerInstance.m_Instance.GetConnectedPeers(peers);
                        foreach (NetPeer Peer in peers.ToArray())
                        {
                            ServerSend.SendHUDSideBarUpdate(Peer, 1, ServerInstance.m_PlayersData.GetShrinkModeString(), ServerInstance);
                        }
                    }
                    return true;
                }
                return false;
            }

            public void RemovePlayer(int PlayerID, Server ServerInstance)
            {
                m_Players.Remove(PlayerID);

                foreach (int TeammateID in m_Players)
                {
                    NetPeer TematePeer = ServerInstance.GetClient(TeammateID);

                    if(TematePeer != null)
                    {
                        ServerSend.SendSquadMemberLeft(TematePeer, PlayerID);
                    }
                }
            }

            public void AddInvite(int PlayerID)
            {
                if (!m_Invites.Contains(PlayerID))
                {
                    m_Invites.Add(PlayerID);
                }
            }

            public void RemoveInvite(int PlayerID)
            {
                if (m_Invites.Contains(PlayerID))
                {
                    m_Invites.Remove(PlayerID);
                }
            }
        }

        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        public static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
        public static bool IsBase64String(string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }
        public static long GetDeterministicId(string m)
        {
            return (long)m.ToCharArray().Select((c, i) => Math.Pow(i, c % 5) * Math.Max(Math.Sqrt(c), i)).Sum();
        }

        public enum CardType
        {
            Hidden = -2,
            Empty = -1,
            Two = 0,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
            Ace,

            Count,
        }

        public enum CardSuit
        {
            Hidden = -1,
            Clubs = 0,
            Spades,
            Hearts,
            Diamonds,

            Count,
        }

        public enum HandRank
        {
            HighCard,
            Pair,
            TwoPair,
            ThreeOfAKind,
            Straight,
            Flush,
            FullHouse,
            FourOfAKind,
            StraightFlush,
            RoyalFlush
        }

        public class PlayingCard
        {
            public CardType m_Type = CardType.Two;
            public CardSuit m_Suit = CardSuit.Clubs;

            public PlayingCard(CardType type, CardSuit suit)
            {
                m_Type = type;
                m_Suit = suit;
            }
        }

        public class CardsDeck
        {
            public List<PlayingCard> m_Cards = new List<PlayingCard>();

            public List<PlayingCard> ShuffleDeck(List<PlayingCard> Deck)
            {
                System.Random RNG = new System.Random();
                for (int i = 0; i < Deck.Count; i++)
                {
                    var temp = Deck[i];
                    var index = RNG.Next(0, Deck.Count);
                    Deck[i] = Deck[index];
                    Deck[index] = temp;
                }
                return Deck;
            }
            public void ShuffleDeck()
            {
                m_Cards = ShuffleDeck(m_Cards);
            }

            public void LogAllCards()
            {
                for (int i = 0;i < m_Cards.Count; i++)
                {
                    SkyCoopServer.Logger.Log($"{i}. {m_Cards[i].m_Type} of {m_Cards[i].m_Suit}");
                }
            }

            public void AddCard(CardType Type, CardSuit Suit)
            {
                m_Cards.Add(new PlayingCard(Type, Suit));
            }
            public void AddCard(int Type, int Suit)
            {
                AddCard((CardType)Type, (CardSuit)Suit);
            }

            public void PopulateCards()
            {
                m_Cards.Clear();
                for (int iCardType = 0; iCardType < (int)CardType.Count; iCardType++)
                {
                    for (int iSuit = 0; iSuit < (int)CardSuit.Count; iSuit++)
                    {
                        AddCard(iCardType, iSuit);
                    }
                }
            }
        }
        public class PropData
        {
            public string prefabname { get; set; }
            public bool frombundle { get; set; }
            public Vector3JSON position { get; set; }
            public QuaternionJSON rotation { get; set; }
            public string guid { get; set; }
        }

        public class Vector3JSON {
            public float x { get; set; }
            public float y { get; set; }
            public float z { get; set; }

            public Vector3 ToVector()
            {
                return new Vector3(x, y, z);
            }

            public Vector3JSON()
            {
                x = 0;
                y = 0;
                z = 0;
            }

            public Vector3JSON(float X, float Y, float Z) 
            {
                x = X; 
                y = Y; 
                z = Z;
            }
        }

        public class QuaternionJSON
        {
            public float x { get; set; }
            public float y { get; set; }
            public float z { get; set; }
            public float w { get; set; }

            public Quaternion ToQuaternion()
            {
                return new Quaternion(x, y, z, w);
            }

            public QuaternionJSON() 
            {
                x = 0;
                y = 0;
                z = 0;
                w = 0;
            }

            public QuaternionJSON(float X, float Y, float Z, float W)
            {
                x = X;
                y = Y;
                z = Z;
                w = W;
            }
        }

        public class RadialLootSpawner
        {
            public Vector3JSON center { get; set; }
            public float top { get; set; }
            public string loottable { get; set; }
            public List<Vector3JSON> points { get; set; }

            public RadialLootSpawner()
            {
                center = new Vector3JSON();
                points = new List<Vector3JSON>();
                loottable = "";
                points = new List<Vector3JSON>();
            }
        }

        public class PrefabTable
        {
            public List<Loot> Items = new List<Loot>();
            public List<LootTableInLootTable> LootTables = new List<LootTableInLootTable>();

            private float m_TotalWeights = -1f;

            public void CalculateWeights()
            {
                if (Items.Count == 0)
                {
                    CalculateLootTableWeights();
                }
                if (LootTables.Count == 0)
                {
                    CalculateItemWeights();
                }
                CalculateCombinedWeights();
            }

            public string GetRandomItemPrefab(int Seed = -1)
            {
                if (Items.Count == 0 && LootTables.Count == 0)
                {
                    return null;
                }

                if (Seed == -1)
                {
                    Seed = System.Guid.NewGuid().GetHashCode();
                }

                System.Random RNG = new System.Random(Seed);

                if (Items.Count == 0)
                {
                    return GetRandomFromLootTables(RNG);
                }
                if (LootTables.Count == 0)
                {
                    return GetRandomFromItems(RNG);
                }
                return GetCombinedWeightedRandom(RNG);
            }

            private string GetRandomFromItems(System.Random RNG)
            {
                if (Items.Count == 1)
                {
                    if (RandomCheck(RNG, Items[0].Chance))
                        return Items[0].Prefab;
                    return null;
                }
                return GetWeightedRandomItem(RNG);
            }

            private string GetRandomFromLootTables(System.Random RNG)
            {
                if (LootTables.Count == 1)
                {
                    if (RandomCheck(RNG, LootTables[0].Chance))
                        return LootTables[0].GetItem();
                    return null;
                }
                return GetWeightedRandomLootTableItem(RNG);
            }

            private string GetCombinedWeightedRandom(System.Random RNG)
            {
                if (m_TotalWeights <= 0)
                    return null;

                float randomValue = RNG.Range(0f, m_TotalWeights);
                float cumulativeWeight = 0f;

                // Check items first
                foreach (var loot in Items)
                {
                    cumulativeWeight += loot.Chance;
                    if (randomValue <= cumulativeWeight)
                    {
                        return loot.Prefab;
                    }
                }

                // Then check loot tables
                foreach (var lootTable in LootTables)
                {
                    cumulativeWeight += lootTable.Chance;
                    if (randomValue <= cumulativeWeight)
                    {
                        return lootTable.GetItem();
                    }
                }

                // Fallback in case of floating point precision issues
                if (Items.Count > 0)
                    return Items[Items.Count - 1].Prefab;
                else
                    return LootTables[LootTables.Count - 1].GetItem();
            }

            private string GetWeightedRandomItem(System.Random RNG)
            {
                if (m_TotalWeights <= 0)
                    return null;

                float randomValue = RNG.Range(0f, m_TotalWeights);
                float cumulativeWeight = 0f;

                foreach (var loot in Items)
                {
                    cumulativeWeight += loot.Chance;
                    if (randomValue <= cumulativeWeight)
                    {
                        return loot.Prefab;
                    }
                }

                return Items[Items.Count - 1].Prefab;
            }

            private string GetWeightedRandomLootTableItem(System.Random RNG)
            {
                if (m_TotalWeights <= 0)
                    return null;

                float randomValue = RNG.Range(0f, m_TotalWeights);
                float cumulativeWeight = 0f;

                foreach (var lootTable in LootTables)
                {
                    cumulativeWeight += lootTable.Chance;
                    if (randomValue <= cumulativeWeight)
                    {
                        return lootTable.GetItem();
                    }
                }

                return LootTables[LootTables.Count - 1].GetItem();
            }

            private void CalculateCombinedWeights()
            {
                m_TotalWeights = 0f;
                foreach (var loot in Items)
                {
                    m_TotalWeights += loot.Chance;
                }
                foreach (var lootTable in LootTables)
                {
                    m_TotalWeights += lootTable.Chance;
                }
            }

            private void CalculateItemWeights()
            {
                m_TotalWeights = 0f;
                foreach (var loot in Items)
                {
                    m_TotalWeights += loot.Chance;
                }
            }

            private void CalculateLootTableWeights()
            {
                m_TotalWeights = 0f;
                foreach (var lootTable in LootTables)
                {
                    m_TotalWeights += lootTable.Chance;
                }
            }

            private bool RandomCheck(System.Random RNG, float chance)
            {
                return RNG.NextSingle() <= chance;
            }
        }

        public class PrefabTableJSON
        {
            public List<Loot> Items { get; set; }
            public List<LootTableInLootTableJSON> LootTables { get; set; }

            public PrefabTableJSON()
            {
                Items = new List<Loot>();
                LootTables = new List<LootTableInLootTableJSON>();
            }
        }

        public class Loot
        {
            public string Prefab { get; set; }
            public float Chance { get; set; } // 0 - 1

            public Loot()
            {
                Prefab = "";
                Chance = 0;
            }
        }

        public class LootTableInLootTableJSON
        {
            public string LootTable { get; set; }
            public float Chance { get; set; } // 0 - 1

            public LootTableInLootTableJSON()
            {
                LootTable = "";
                Chance = 0;
            }
        }

        public class LootTableInLootTable
        {
            public string Name = string.Empty;
            public PrefabTable LootTable { get; set; }
            public float Chance { get; set; } // 0 - 1

            public string GetItem()
            {
                if (LootTable != null)
                {
                    return LootTable.GetRandomItemPrefab();
                }
                return string.Empty;
            }
        }

        public class LeaderData
        {
            public int m_ID = 0;
            public int m_Score = 0;
            public ClothingData m_ClothingData = new ClothingData();

            public LeaderData(){}

            public LeaderData(PlayerData PlayerData, int Score = 0)
            {
                m_ID = PlayerData.m_PlayerID;
                m_Score = Score;
                m_ClothingData = PlayerData.m_VisualData.m_ClothingData.GetCopy();
            }
        }
    }
}
