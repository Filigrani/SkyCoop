using LiteNetLib;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Xml.Linq;

namespace SkyCoopServer
{
    public class DataStr
    {
        public class ServerConfig
        {
            public int m_MaxPlayers = 32;
            public string m_StartingRegion = "";
            public int m_Seed = 0;
            //public int m_VoicePort = 37850;
            public int m_VoicePort = 0;
            public string m_ExperienceMode = "Stalker";
            public string m_SceneToSpawn = "";
            public string m_GameMode = "Sandbox";
            public bool m_CheatsAllowed = true;
        }

        public const int c_SpeedUpHours = 12;
        public const int c_SpeedUpHoursMinutes = 720;
        public const int c_SpeedUpRealSecondsDuration = 30;
        public const int c_SpeedUpTimeScale = 100;

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
            public int m_MinimalPlayersToPlay = 0;
            public bool m_PlayerCanBeKnocked = false;
            public bool m_PVP = true;
            public List<StartingGearData> m_StartingItems = new List<StartingGearData>();
            public List<List<StartingGearData>> m_StartingItemsByTier = new List<List<StartingGearData>>();
            public int m_Time = 0;
            public int m_LootPerRadialSpawn = 5;
            public string m_HUDMode = "";
            public bool m_DeathPacks = true;
            public int m_Respawns = 1;
            public bool m_Clothing = true;
            public bool m_CanDropItems = true;
            public bool m_CanUseContainers = true;
            public bool m_CanUseMap = true;
            public AirDropJson m_AirDrop = null;
            public bool m_AdvancedSpawnPoints = false;
            public bool m_Fatigue = true;
            public bool m_Hunger = true;
            public bool m_Thirst = true;
            public bool m_Cold = true;
            public bool m_CanUseBeds = true;
            public bool m_CanStartFire = true;
            public bool m_CanUseTransitions = true;
            public string m_SceneUnload = "";
            public bool m_Weather = true;
            public bool m_CanCraft = true;

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

        public class AirDropJson
        {
            public string Prefab { get; set; }
            public string Path { get; set; }
            public float Altitude { get; set; }
            public float FallTime { get; set; }
        }

        public class GameRulesJson
        {
            public List<string> Maps { get; set; }
            public int MinimalPlayers;
            public bool Knockdowns { get; set; }
            public bool PVP { get; set; }
            public List<StartingGearData> StartingGear { get; set; }
            public List<List<StartingGearData>> StartingGearByTier { get; set; }
            public int Time { get; set; }
            public int LootPerRadialSpawn { get; set; }
            public string HUDMode { get; set; }
            public bool DeathPacks { get; set; }
            public int Respawns { get; set; }
            public bool Clothing { get; set; }
            public bool CanDropItems { get; set; }
            public bool CanUseContainers { get; set; }
            public bool CanUseMap { get; set; }
            public AirDropJson AirDrop { get; set; }
            public bool AdvancedSpawnPoints { get; set; }
            public bool Fatigue { get; set; }
            public bool Hunger { get; set; }
            public bool Thirst { get; set; }
            public bool Cold { get; set; }
            public bool CanUseBeds { get; set; }
            public bool CanStartFire { get; set; }
            public bool CanUseTransitions { get; set; }
            public string SceneUnload { get; set; }
            public bool Weather { get; set; }
            public bool CanCraft { get; set; }

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

                if(MinimalPlayers == null || MinimalPlayers == 0)
                {
                    Inst.m_MinimalPlayersToPlay = 0;
                }
                else
                {
                    Inst.m_MinimalPlayersToPlay = MinimalPlayers;
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

                if (AirDrop != null)
                {
                    Inst.m_AirDrop = AirDrop;
                }

                Inst.m_DeathPacks = DeathPacks;
                Inst.m_Respawns = Respawns;
                Inst.m_Clothing = Clothing;
                Inst.m_CanDropItems = CanDropItems;
                Inst.m_CanUseContainers = CanUseContainers;
                Inst.m_CanUseMap = CanUseMap;
                Inst.m_AdvancedSpawnPoints = AdvancedSpawnPoints;

                Inst.m_Fatigue = Fatigue;
                Inst.m_Hunger = Hunger;
                Inst.m_Thirst = Thirst;
                Inst.m_Cold = Cold;
                Inst.m_CanUseBeds = CanUseBeds;
                Inst.m_CanStartFire = CanStartFire;
                Inst.m_CanUseTransitions = CanUseTransitions;
                Inst.m_SceneUnload = SceneUnload;
                Inst.m_Weather = Weather;
                Inst.m_CanCraft = CanCraft;

                return Inst;
            }
        }

        public class MinimalPlayersAndGameMode
        {
            public string GameModeName = "";
            public int MinimalPlayers = 0;
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
            public int m_VoiceChatID = -1;
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
            public int m_BloodLosses = 0;

            public string m_CarSeat = "";
            public string m_InteractionGUID = "";

            public int m_SquadInvitesSent = 0;
            public DateTime m_LastInviteTime;
            public DateTime m_LastRespawn;

            public GamePlayState m_GamePlayState = GamePlayState.Unassigned;

            public bool m_IsWorking = false;

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
                Logger.Log($"[DataStr.PlayerData] Client {m_PlayerName} new gamepaly state {m_GamePlayState}");
                m_InteractionGUID = "";
                m_CarSeat = "";

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
                                    ServerSend.SendRemoveAllInjectedItem(m_PlayerID, ServerInstance);
                                    break;
                                case GamePlayState.Dead:
                                    ServerSend.SendPlayerAction(OtherPlayer, 5, m_PlayerID);
                                    ServerSend.SendRemoveAllInjectedItem(m_PlayerID, ServerInstance);
                                    ServerInstance.m_PlayersData.PlayerChangeGear(m_PlayerID, "", 0, true);
                                    break;
                                case GamePlayState.Spectator:
                                    ServerSend.SendPlayerAction(OtherPlayer, 5, m_PlayerID);
                                    ServerSend.SendRemoveAllInjectedItem(m_PlayerID, ServerInstance);
                                    ServerInstance.m_PlayersData.PlayerChangeGear(m_PlayerID, "", 0, true);
                                    //ServerSend.SendPlayerSceneNotification(OtherPlayer, false, m_PlayerID); // Пусть игрок валяеться чисто для фана
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
                    
                    if (ServerInstance.m_Rules.m_Respawns != 0)
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
                    PlayersSquad Squad = ServerInstance.m_PlayersData.GetSquadPlayerIn(m_PlayerID);

                    if (Squad != null)
                    {
                        if (Squad.IsAlive(ServerInstance))
                        {
                            ServerSend.SendSquadEliminated(ServerInstance, Squad.m_Name);
                        }
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
                    m_LastRespawn = DateTime.UtcNow;
                }

                m_Damagers.Clear();
            }

            public void AddKill(Server ServerInstance)
            {
                m_Kills++;
                if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "DM" || ServerInstance.m_Rules.m_HUDMode == "Shrink")
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
                if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "DM")
                {
                    ServerSend.SendHUDSideBarUpdate(ServerInstance.GetClient(m_PlayerID), 0, m_Kills.ToString(), ServerInstance);
                }
            }

            public void AddDeath(Server ServerInstance)
            {
                m_Deaths++;
                if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "DM")
                {
                    ServerSend.SendHUDSideBarUpdate(ServerInstance.GetClient(m_PlayerID), 1, m_Deaths.ToString(), ServerInstance);
                }
            }

            public void AddAssist(Server ServerInstance)
            {
                m_Assists++;
                if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "DM")
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
            public float m_ConditionNormalized = 1;
            public int m_Style = 0;

            public bool m_HasCookingSlot = false;
            public string m_FireGUID = "";
            public int m_CookingSlot = -1;

            public bool m_HasCookingRecipe = false;
            public string m_CookingResult = "";
            public float m_Volume = 0;

            public float m_BeingCookedTime = 0;

            public bool m_IsCooking = false;

            public string m_CookpotGUID = "";
            public string m_ProductGUID = "";

            public void SetCookingSlot(string FireGUID, int SlotIndex)
            {
                if(SlotIndex < 0 || string.IsNullOrEmpty(FireGUID))
                {
                    m_FireGUID = "";
                    m_CookingSlot = -1;

                    m_HasCookingSlot = false;
                }
                else
                {
                    m_FireGUID = FireGUID;
                    m_CookingSlot = SlotIndex;

                    m_HasCookingSlot = true;
                }
            }

            public void StartCooking(float CurrentTime)
            {
                m_IsCooking = true;
            }

            public void StopCooking()
            {
                m_IsCooking = false;
            }

            public void AddCookingHours(float HoursCooked)
            {
                m_BeingCookedTime += HoursCooked;
            }

            public void SetRecipe(string Result, float Volume, float TimeBeingCooked)
            {
                if (string.IsNullOrEmpty(Result))
                {
                    m_CookingResult = "";
                    m_Volume = 0;
                    m_BeingCookedTime = 0;

                    m_HasCookingRecipe = false;
                }
                else
                {
                    m_CookingResult = Result;
                    m_Volume = Volume;
                    m_BeingCookedTime = TimeBeingCooked;

                    m_HasCookingRecipe = true;
                }
            }
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

            public GearDataContainer() { }
            public GearDataContainer(SaveData Data)
            {
                Load(Data);
            }

            public class SaveData
            {
                public string GUID { get; set; }
                public string JSON { get; set; }
                public string GearName { get; set; }
                public Vector3JSON Position { get; set; }
                public QuaternionJSON Rotation { get; set; }
                public float Condition { get; set; }
                public int Style { get; set; }
                public string FireGUID { get; set; }
                public int CookingSlot { get; set; }
                public string CookingResult { get; set; }
                public float Volume { get; set; }
                public float BeingCookingTime { get; set; }
                public string CookingPotGUID { get; set; }
                public string ProductGUID { get; set; }
            }

            public SaveData Save()
            {
                SaveData data = new SaveData();

                data.GUID = m_Data.m_GUID;
                data.JSON = m_Data.m_JSON;
                data.GearName = m_Visual.m_GearName;
                data.Position = new Vector3JSON(m_Visual.m_Position.X, m_Visual.m_Position.Y, m_Visual.m_Position.Z);
                data.Rotation = new QuaternionJSON(m_Visual.m_Rotation.X, m_Visual.m_Rotation.Y, m_Visual.m_Rotation.Z, m_Visual.m_Rotation.W);
                data.Condition = m_Visual.m_ConditionNormalized;
                data.Style = m_Visual.m_Style;
                data.FireGUID = m_Visual.m_FireGUID;
                data.CookingSlot = m_Visual.m_CookingSlot;
                data.CookingResult = m_Visual.m_CookingResult;
                data.Volume = m_Visual.m_Volume;
                data.BeingCookingTime = m_Visual.m_BeingCookedTime;
                data.CookingPotGUID = m_Visual.m_CookpotGUID;
                data.ProductGUID = m_Visual.m_ProductGUID;

                return data;
            }

            public void Load(SaveData data)
            {
                m_Data.m_GUID = data.GUID; m_Visual.m_GUID = data.GUID;
                m_Data.m_JSON = data.JSON;
                m_Visual.m_GearName = data.GearName;
                m_Visual.m_Position = data.Position.ToVector();
                m_Visual.m_Rotation = data.Rotation.ToQuaternion();
                m_Visual.m_ConditionNormalized = data.Condition;
                m_Visual.m_Style = data.Style;

                m_Visual.SetCookingSlot(data.FireGUID, data.CookingSlot);
                m_Visual.SetRecipe(data.CookingResult, data.Volume, data.BeingCookingTime);

                m_Visual.m_CookpotGUID = data.CookingPotGUID;
                m_Visual.m_ProductGUID = data.ProductGUID;
            }
        }

        public class HarvestableData
        {
            public string m_GUID = "";
            public float m_HarvestTime = 0;
            public float m_RespawnIn = 0;

            public HarvestableData() { }
            public HarvestableData(string GUID, float HarvestTime, float RespawnIn)
            {
                m_GUID = GUID;
                m_HarvestTime = HarvestTime;
                m_RespawnIn = RespawnIn;
            }
            public HarvestableData(SaveData data)
            {
                Load(data);
            }

            public void Load(SaveData data)
            {
                m_GUID = data.GUID;
                m_HarvestTime = data.HarvestTime;
                m_RespawnIn = data.RespawnIn;
            }

            public SaveData Save()
            {
                SaveData data = new SaveData();

                data.GUID = m_GUID;
                data.HarvestTime = m_HarvestTime;
                data.RespawnIn = m_RespawnIn;

                return data;
            }

            public class SaveData
            {
                public string GUID { get; set; }
                public float HarvestTime { get; set; }
                public float RespawnIn { get; set; }
            }
        }

        public class SceneData
        {
            public string m_SceneName = "";
            public Dictionary<string, GearDataContainer> m_Gears = new Dictionary<string, GearDataContainer>();
            public Dictionary<string, bool> m_Openables = new Dictionary<string, bool>();
            public Dictionary<string, DeathPack> m_DeathPacks = new Dictionary<string, DeathPack>();
            public Dictionary<string, string> m_Containers = new Dictionary<string, string>();
            public Dictionary<string, int> m_ContainerStats = new Dictionary<string, int>();
            public Dictionary<string, FireSyncData> m_Fires = new Dictionary<string, FireSyncData>();
            public Dictionary<string, bool> m_BreakDowns = new Dictionary<string, bool>();
            public Dictionary<string, HarvestableData> m_Harvestables = new Dictionary<string, HarvestableData>();

            public Dictionary<string, PropData> m_Props = new Dictionary<string, PropData>();
            public List<V3Quat> m_SpawnPoints = new List<V3Quat>();
            public List<RadialLootSpawner> m_RadialLootSpawners = new List<RadialLootSpawner>();
            public Dictionary<string, FallingProp> m_FallingProps = new Dictionary<string, FallingProp>();

            public DangerCircleConfig m_ZoneConfig = null;
            public DangerCircleData m_ActiveZone = null;
            public V3Quat m_VictoryPoint = new V3Quat();

            public SceneData() { }
            public SceneData(SaveData data)
            {
                Load(data);
            }

            public class SaveData
            {
                public string SceneName { get; set; }
                public List<GearDataContainer.SaveData> Gears { get; set; }
                public Dictionary<string, bool> Openables { get; set; }
                public List<DeathPack.SaveData> DeathPacks { get; set; }
                public Dictionary<string, string> Containers { get; set; }
                public Dictionary<string, int> ContainersStats { get; set; }
                public List<FireSyncData.SaveData> Fires { get; set; }
                public Dictionary<string, bool> BreakDowns { get; set; }
                public List<HarvestableData.SaveData> Harvestables { get; set; }
            }

            public SaveData Save()
            {
                SaveData data = new SaveData();
                data.SceneName = m_SceneName;
                data.Gears = new List<GearDataContainer.SaveData>();
                foreach (GearDataContainer Gear in m_Gears.Values.ToList())
                {
                    data.Gears.Add(Gear.Save());
                }
                data.Openables = new Dictionary<string, bool>();
                foreach (var openable in m_Openables.ToList())
                {
                    data.Openables.Add(openable.Key, openable.Value);
                }
                data.DeathPacks = new List<DeathPack.SaveData>();
                foreach (var pack in m_DeathPacks.ToList())
                {
                    data.DeathPacks.Add(pack.Value.Save());
                }
                data.Containers = new Dictionary<string, string>();
                foreach (var box in m_Containers.ToList())
                {
                    data.Containers.Add(box.Key, box.Value);
                }
                data.ContainersStats = new Dictionary<string, int>();
                foreach (var box in m_ContainerStats.ToList())
                {
                    data.ContainersStats.Add(box.Key, box.Value);
                }
                data.Fires = new List<FireSyncData.SaveData>();
                foreach (FireSyncData fire in m_Fires.Values.ToList())
                {
                    data.Fires.Add(fire.Save());
                }
                data.BreakDowns = new Dictionary<string, bool>();
                foreach (var breakdown in m_BreakDowns.ToList())
                {
                    data.BreakDowns.Add(breakdown.Key, breakdown.Value);
                }
                data.Harvestables = new List<HarvestableData.SaveData>();
                foreach (HarvestableData harvestable in m_Harvestables.Values.ToList())
                {
                    data.Harvestables.Add(harvestable.Save());
                }
                return data;
            }

            public void Load(SaveData data)
            {
                m_SceneName = data.SceneName;
                if(data.Gears != null)
                {
                    foreach (GearDataContainer.SaveData saveData in data.Gears)
                    {
                        GearDataContainer Gear = new GearDataContainer(saveData);
                        m_Gears.Add(Gear.m_Data.m_GUID, Gear);
                    }
                }
                if (data.Openables != null)
                {
                    foreach (var saveData in data.Openables)
                    {
                        m_Openables.Add(saveData.Key, saveData.Value);
                    }
                }
                if (data.DeathPacks != null)
                {
                    foreach (DeathPack.SaveData saveData in data.DeathPacks)
                    {
                        DeathPack Pack = new DeathPack(saveData);
                        m_DeathPacks.Add(Pack.m_GUID, Pack);
                    }
                }
                if (data.Containers != null)
                {
                    foreach (var saveData in data.Containers)
                    {
                        m_Containers.Add(saveData.Key, saveData.Value);
                    }
                }
                if (data.ContainersStats != null)
                {
                    foreach (var saveData in data.ContainersStats)
                    {
                        m_ContainerStats.Add(saveData.Key, saveData.Value);
                    }
                }
                if (data.Fires != null)
                {
                    foreach (FireSyncData.SaveData saveData in data.Fires)
                    {
                        FireSyncData Fire = new FireSyncData(saveData);
                        m_Fires.Add(Fire.m_GUID, Fire);
                    }
                }
                if (data.BreakDowns != null)
                {
                    foreach (var saveData in data.BreakDowns)
                    {
                        m_BreakDowns.Add(saveData.Key, saveData.Value);
                    }
                }
                if (data.Harvestables != null)
                {
                    foreach (HarvestableData.SaveData saveData in data.Harvestables)
                    {
                        HarvestableData Harvestable = new HarvestableData(saveData);
                        m_Harvestables.Add(Harvestable.m_GUID, Harvestable);
                    }
                }
            }

            public void SaveToFile(Server ServerInstance)
            {
                SaveData Data = Save();

                FilesManager.SaveSceneToFile(Data, ServerInstance);
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

            public void SpawnVanilaLoot(Server ServerInstance)
            {
                DataStr.SceneLootSpawns LootData = ServerInstance.m_ScenesData.GetSceneLoot(m_SceneName);
                if(LootData != null)
                {
                    Random RNG = new Random(Guid.NewGuid().GetHashCode());

                    float LootSpawnChanceScaler = 1f;

                    switch (ServerInstance.m_Config.m_ExperienceMode)
                    {
                        case "Pilgrim":
                            LootSpawnChanceScaler = 1f;
                            break;
                        case "Voyageur":
                            LootSpawnChanceScaler = 0.9f;
                            break;
                        case "Stalker":
                            LootSpawnChanceScaler = 0.6f;
                            break;
                        case "Interloper":
                        case "Misery":
                            LootSpawnChanceScaler = 0.1f;
                            break;
                        default:
                            LootSpawnChanceScaler = 0.6f;
                            break;
                    }

                    foreach (PrefabSpawnData Spawner in LootData.PrefabSpawns)
                    {
                        if(Spawner.EnabledForXP != null && Spawner.EnabledForXP.Count > 0)
                        {
                            if (!Spawner.EnabledForXP.Contains(ServerInstance.m_Config.m_ExperienceMode))
                            {
                                continue;
                            }
                        }
                        if (Spawner.DisabledForXP != null && Spawner.DisabledForXP.Count > 0)
                        {
                            if (Spawner.DisabledForXP.Contains(ServerInstance.m_Config.m_ExperienceMode))
                            {
                                continue;
                            }
                        }

                        if(Spawner.m_ChanceOfNoSpawn > 0)
                        {
                            if(RNG.Range(0, 100) < Spawner.m_ChanceOfNoSpawn)
                            {
                                continue;
                            }
                        }

                        int NumToSpawn = RNG.Range(Spawner.m_NumToSpawnMin, Spawner.m_NumToSpawnMax);
                        List<GearSpawnElementData> Gears = new List<GearSpawnElementData>(Spawner.Gears);
                        if (NumToSpawn > 0)
                        {
                            for (int i = 1; i <= NumToSpawn; i++)
                            {
                                float TotalWeight = 0;

                                foreach (GearSpawnElementData Element in Gears)
                                {
                                    TotalWeight += Element.SpawnWeight;
                                }
                                float RandomValue = (float)RNG.NextDouble() * TotalWeight;
                                float CumulativeWeight = 0;
                                int IndexToRemove = -1;

                                for (int i2 = 0; i < Gears.Count; i2++)
                                {
                                    GearSpawnElementData Gear = Gears[i2];
                                    CumulativeWeight += Gear.SpawnWeight;
                                    if (RandomValue <= CumulativeWeight)
                                    {
                                        IndexToRemove = i2;

                                        bool Spawn = true;

                                        if (Gear.Chance < 99f)
                                        {
                                            Spawn = RNG.Range(0f, 100) < Gear.Chance * LootSpawnChanceScaler;
                                        }

                                        if (Spawn)
                                        {

                                            ServerInstance.m_ScenesData.AddGear(m_SceneName, Gear.GearName, Gear.Position.ToVector(), Gear.Rotation.ToQuaternion(), string.Empty, 1, 0);
                                        }
                                        break;
                                    }
                                }
                                if (IndexToRemove != -1)
                                {
                                    Gears.RemoveAt(IndexToRemove);
                                }
                            }
                        }
                    }
                    foreach (RandomSpawnObjectData Spawner in LootData.RandomSpawnObjects)
                    {
                        if (Spawner.EnabledForXP != null && Spawner.EnabledForXP.Count > 0)
                        {
                            if (!Spawner.EnabledForXP.Contains(ServerInstance.m_Config.m_ExperienceMode))
                            {
                                continue;
                            }
                        }
                        if (Spawner.DisabledForXP != null && Spawner.DisabledForXP.Count > 0)
                        {
                            if (Spawner.DisabledForXP.Contains(ServerInstance.m_Config.m_ExperienceMode))
                            {
                                continue;
                            }
                        }
                        int NumToSpawn = 0;

                        switch (ServerInstance.m_Config.m_ExperienceMode)
                        {
                            case "Pilgrim":
                                NumToSpawn = Spawner.NumObjectsToSpawnPilgrim;
                                break;
                            case "Voyageur":
                                NumToSpawn = Spawner.NumObjectsToSpawnVoyageur;
                                break;
                            case "Stalker":
                                NumToSpawn = Spawner.NumObjectsToSpawnStalker;
                                break;
                            case "Interloper":
                            case "Misery":
                                NumToSpawn = Spawner.NumObjectsToSpawnInterloper;
                                break;
                            default:
                                NumToSpawn = Spawner.NumObjectsToSpawnStalker;
                                break;
                        }


                        List<GearSpawnElementData> Gears = new List<GearSpawnElementData>(Spawner.Gears);
                        if (NumToSpawn > 0)
                        {
                            for (int i = 1; i <= NumToSpawn; i++)
                            {
                                float TotalWeight = 0;

                                foreach (GearSpawnElementData Element in Gears)
                                {
                                    TotalWeight += Element.SpawnWeight;
                                }
                                float RandomValue = (float)RNG.NextDouble() * TotalWeight;
                                float CumulativeWeight = 0;
                                int IndexToRemove = -1;

                                for (int i2 = 0; i < Gears.Count; i2++)
                                {
                                    GearSpawnElementData Gear = Gears[i2];
                                    CumulativeWeight += Gear.SpawnWeight;
                                    if (RandomValue <= CumulativeWeight)
                                    {
                                        IndexToRemove = i2;
                                        bool Spawn = true;

                                        if(Gear.Chance < 99f)
                                        {
                                            Spawn = RNG.Range(0f, 100) < Gear.Chance * LootSpawnChanceScaler;
                                        }

                                        if (Spawn)
                                        {
                                            ServerInstance.m_ScenesData.AddGear(m_SceneName, Gear.GearName, Gear.Position.ToVector(), Gear.Rotation.ToQuaternion(), string.Empty, 1, 0);
                                        }
                                        break;
                                    }
                                }
                                if (IndexToRemove != -1)
                                {
                                    Gears.RemoveAt(IndexToRemove);
                                }
                            }
                        }
                    }
                    foreach (LooseGearSpawn Spawner in LootData.LooseGearSpawns)
                    {
                        bool Spawn = true;
                        if (Spawner.Chance < 99f)
                        {
                            Spawn = RNG.Range(0f, 100) < Spawner.Chance * LootSpawnChanceScaler;
                        }

                        if (Spawn)
                        {
                            ServerInstance.m_ScenesData.AddGear(m_SceneName, Spawner.GearName, Spawner.Position.ToVector(), Spawner.Rotation.ToQuaternion(), string.Empty, 1, 0);
                        }
                    }
                    int IndexForLog = -1;
                    foreach (RadialObjectSpawnerData Spawner in LootData.RadialSpawns)
                    {
                        IndexForLog++;
                        if (Spawner.EnabledForXP != null && Spawner.EnabledForXP.Count > 0)
                        {
                            if (!Spawner.EnabledForXP.Contains(ServerInstance.m_Config.m_ExperienceMode))
                            {
                                continue;
                            }
                        }
                        if (Spawner.DisabledForXP != null && Spawner.DisabledForXP.Count > 0)
                        {
                            if (Spawner.DisabledForXP.Contains(ServerInstance.m_Config.m_ExperienceMode))
                            {
                                continue;
                            }
                        }
                        int NumToSpawn = RNG.Range(Spawner.MinToSpawn, Spawner.MaxToSpawn);
                        List<Vector3JSON> PossiblePoints = new List<Vector3JSON>(Spawner.PossiblePoints);
                        if (NumToSpawn > 0)
                        {
                            for (int i = 1; i <= NumToSpawn; i++)
                            {
                                float TotalWeight = 0;

                                foreach (RadialObjectSpawnerElementData Element in Spawner.Gears)
                                {
                                    TotalWeight += Element.SpawnWeight;
                                }
                                float RandomValue = (float)RNG.NextDouble() * TotalWeight;
                                float CumulativeWeight = 0;

                                RadialObjectSpawnerElementData GearToSpawn = null;

                                for (int i2 = 0; i < Spawner.Gears.Count; i2++)
                                {
                                    RadialObjectSpawnerElementData Gear = Spawner.Gears[i2];
                                    CumulativeWeight += Gear.SpawnWeight;
                                    if (RandomValue <= CumulativeWeight)
                                    {
                                        GearToSpawn = Gear;
                                        break;
                                    }
                                }

                                if(GearToSpawn == null)
                                {
                                    GearToSpawn = Spawner.Gears[0];
                                }

                                if (GearToSpawn != null)
                                {
                                    bool Spawn = true;

                                    if (GearToSpawn.Chance < 99f)
                                    {
                                        Spawn = RNG.Range(0f, 100) < GearToSpawn.Chance * LootSpawnChanceScaler;
                                    }
                                    if (Spawn)
                                    {
                                        if (PossiblePoints.Count > 0)
                                        {
                                            int RandomPointIndex = RNG.Range(0, PossiblePoints.Count);
                                            Vector3JSON Point = PossiblePoints[RandomPointIndex];
                                            PossiblePoints.RemoveAt(RandomPointIndex);
                                            ServerInstance.m_ScenesData.AddGear(m_SceneName, GearToSpawn.GearName, Point.ToVector(), Extensions.Euler(0, RNG.Range(0, 360), 0), string.Empty, 1, 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    foreach (SpawnGearVariantData Spawn in LootData.SpawnGearVariants)
                    {
                        if(Spawn.Gears.Count > 0)
                        {
                            int RandomIndex = RNG.Range(0, Spawn.Gears.Count);

                            SpawnGearVariantElementData Element = Spawn.Gears[RandomIndex];
                            ServerInstance.m_ScenesData.AddGear(m_SceneName, Element.GearName, Element.Position.ToVector(), Element.Rotation.ToQuaternion(), string.Empty, 1, 0);
                        }
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

                                ServerInstance.m_ScenesData.AddGear(m_SceneName, GearName, Point, Extensions.Euler(0, RNG.Range(0, 360), 0), string.Empty, 1, 0);
                                AvaliablePoints.RemoveAt(Index);
                            }
                        }
                    }
                    PointIndex++;
                }
            }
        }

        public struct Score : IComparable<Score>
        {
            public int PlayerID;
            
            public int Kills;
            public int Assits;
            public int Deaths;
            public int Bonus;

            public Score(int ID, int kills, int assists, int deaths, int bonus = 0)
            {
                PlayerID = ID;
                Kills = kills;
                Assits = assists;
                Deaths = deaths;
                Bonus = bonus;
            }

            public Score(int ID, int bonus = 0)
            {
                PlayerID = ID;
                Kills = 0;
                Assits = 0;
                Deaths = 0;
                Bonus = bonus;
            }

            public int GetFinalScore()
            {
                return Kills + ((int)MathF.Floor(Assits * 0.5f)) + Bonus;
            }

            public int CompareTo(Score other)
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

        public class DangerCircleShrinkStateData
        {
            public DateTime m_StartShrinkingTime;
            public DateTime m_EndShrinkingTime;

            public float m_PreviousRadius = 0;
            public float m_NewRadius = 0;

            public Vector3 m_PreviousCenter = Vector3.Zero;
            public Vector3 m_NewCenter = Vector3.Zero;

            public float GetProgress()
            {
                float totalDuration = (float)(m_EndShrinkingTime - m_StartShrinkingTime).TotalSeconds;
                float elapsed = (float)(DateTime.UtcNow - m_StartShrinkingTime).TotalSeconds;


                float progress = elapsed / totalDuration;

                if(progress < 0)
                {
                    return 0;
                }else if(progress > 1)
                {
                    return 1;
                }
                return progress;
            }

            public float GetCurrentRadius()
            {
                return Lerp(m_PreviousRadius, m_NewRadius, GetProgress());
            }

            public Vector3 GetCenter()
            {
                return Lerp(m_PreviousCenter, m_NewCenter, GetProgress());
            }
        }


        public class DangerCircleData
        {
            public DangerCircleConfig m_Config = new DangerCircleConfig();
            public int m_CurrentStageIndex = 0;
            public ShrinkStage m_CurrentStage = null;
            public float m_RadiusAfterWait = 0;
            public Vector3 m_CenterAfterWait = Vector3.Zero;

            public State m_State = State.Waiting;

            public bool m_DebugNoDamage = false;

            public DangerCircleShrinkStateData m_Data = new DangerCircleShrinkStateData();

            public enum State
            {
                Waiting,
                Shrinking,
                Finished,
            }

            private DateTime s_StateTimer;
            private bool s_StateTimerActive = false;
            private string s_SceneName = "";
            private Server s_ServerInstance;

            public string GetTimerPrefix()
            {
                switch (m_State)
                {
                    case State.Waiting:
                        return "GAMEPLAY_TimeRemainingZone";
                    case State.Shrinking:
                        return "GAMEPLAY_ZoneShrinking";
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
                        return (int)(s_StateTimer - DateTime.UtcNow).TotalSeconds;
                    case State.Shrinking:
                        if(m_CurrentStageIndex == m_Config.Stages.Count - 1)
                        {
                            return 0;
                        }
                        else
                        {
                            return (int)(s_StateTimer - DateTime.UtcNow).TotalSeconds;
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
                if(m_Config.MapScale == null || m_Config.MapScale.x == 0)
                {
                    m_Config.MapScale = new Vector2JSON(0.018f, 0.018f);
                }
                s_SceneName = SceneName;
                s_ServerInstance = Server;
            }

            public void NextState()
            {
                switch (m_State)
                {
                    case State.Waiting:
                        m_State = State.Shrinking;
                        m_Data.m_PreviousCenter = m_Data.m_NewCenter;
                        m_Data.m_PreviousRadius = m_Data.m_NewRadius;

                        m_Data.m_NewCenter = m_CenterAfterWait;
                        m_Data.m_NewRadius = m_RadiusAfterWait;

                        m_Data.m_StartShrinkingTime = DateTime.UtcNow;
                        SetNextStage();
                        m_Data.m_EndShrinkingTime = DateTime.UtcNow.AddSeconds(m_CurrentStage.ShrinkTime);

                        s_StateTimer = DateTime.UtcNow.AddSeconds(m_CurrentStage.ShrinkTime);
                        s_StateTimerActive = true;
                        break;
                    case State.Shrinking:
                        if(m_CurrentStage.StageTime <= 0)
                        {
                            m_State = State.Finished;
                            s_StateTimerActive = false;
                        }
                        else
                        {
                            m_State = State.Waiting;
                            m_RadiusAfterWait = m_Config.Stages[m_CurrentStageIndex + 1].Radius;
                            m_CenterAfterWait = GetNewRandomCenter(m_Data.m_NewCenter, m_Data.m_NewRadius, m_RadiusAfterWait);

                            s_StateTimer = DateTime.UtcNow.AddSeconds(m_CurrentStage.StageTime);
                            s_StateTimerActive = true;
                        }
                        break;
                }
            }

            public void Start()
            {
                SetStage(0);

                m_Data.m_PreviousCenter = m_Config.ActualCenter.ToVector();
                m_Data.m_NewCenter = m_Config.ActualCenter.ToVector();

                m_Data.m_PreviousRadius = m_CurrentStage.Radius;
                m_Data.m_NewRadius = m_CurrentStage.Radius;


                m_Data.m_StartShrinkingTime = DateTime.UtcNow;
                m_Data.m_EndShrinkingTime = DateTime.UtcNow;

                s_StateTimerActive = false;

                if (m_Config.Stages.Count == 1)
                {
                    m_State = State.Finished;
                }
                else
                {
                    m_State = State.Waiting;
                    s_StateTimer = DateTime.UtcNow.AddSeconds(m_CurrentStage.StageTime);
                    m_RadiusAfterWait = m_Config.Stages[m_CurrentStageIndex + 1].Radius;
                    m_CenterAfterWait = GetNewRandomCenter(m_Data.m_NewCenter, m_Data.m_NewRadius, m_RadiusAfterWait);

                    s_StateTimerActive = true;
                }
                ServerSend.SendZoneUpdate(s_SceneName, m_Data, GetNextCenter(), GetNextRadius(), m_Config.MapScale.ToVector(), s_ServerInstance);
            }

            public float GetNextRadius()
            {
                switch (m_State)
                {
                    case State.Waiting:
                        return m_RadiusAfterWait;
                    case State.Shrinking:
                        return m_Data.m_NewRadius;
                    case State.Finished:
                        return 0;
                }
                return 0;
            }

            public Vector3 GetNextCenter()
            {
                switch (m_State)
                {
                    case State.Waiting:
                        return m_CenterAfterWait;
                    case State.Shrinking:
                        return m_Data.m_NewCenter;
                    case State.Finished:
                        return Vector3.Zero;
                }
                return Vector3.Zero;
            }

            public void ForceNextZone(bool withtime)
            {
                if (s_StateTimerActive)
                {
                    if (!withtime)
                    {
                        s_StateTimer = DateTime.UtcNow;
                    }
                    else
                    {
                        s_StateTimer = DateTime.UtcNow.AddSeconds(3);
                        m_Data.m_EndShrinkingTime = s_StateTimer;
                    }
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

            public void EverySecond()
            {
                DamageCheck();
                if (s_ServerInstance.m_Rules.m_Time == 0)
                {
                    ServerSend.ClientGameModeTimer(GetTimerSeconds(), s_ServerInstance);
                }

                if (s_StateTimerActive)
                {
                    if (s_StateTimer < DateTime.UtcNow)
                    {
                        NextState();
                        ServerSend.UpdateTimerPrefix(GetTimerPrefix(), s_ServerInstance);
                        ServerSend.ClientGameModeTimer(GetTimerSeconds(), s_ServerInstance);
                        ServerSend.SendZoneUpdate(s_SceneName, m_Data, GetNextCenter(), GetNextRadius(), m_Config.MapScale.ToVector(), s_ServerInstance);
                    }
                }
            }

            public bool IsInsideZone(Vector3 Point)
            {
                Vector3 CurrentCenter = m_Data.GetCenter();
                float CurrentRadius = m_Data.GetCurrentRadius();

                float Distance = Vector2.Distance(new Vector2(Point.X, Point.Z), new Vector2(CurrentCenter.X, CurrentCenter.Z));
                return Distance < CurrentRadius / 2;
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
                        if (!IsInsideZone(PlayerData.m_Position))
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
            public Vector2JSON MapScale { get; set; }

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

            public DeathPack() { }
            public DeathPack(SaveData data) 
            {
                Load(data);
            }

            public class SaveData
            {
                public string Prefab { get; set; }
                public string GUID { get; set; }
                public string Owner { get; set; }
                public Vector3JSON Position { get; set; }
                public QuaternionJSON Rotation { get; set; }
            }

            public SaveData Save()
            {
                SaveData data = new SaveData();

                data.Prefab = m_Prefab;
                data.GUID = m_GUID;
                data.Owner = m_Owner;
                data.Position = new Vector3JSON(m_Position.X, m_Position.Y, m_Position.Z);
                data.Rotation = new QuaternionJSON(m_Rotation.X, m_Rotation.Y, m_Rotation.Z, m_Rotation.W);

                return data;
            }

            public void Load(SaveData data)
            {
                m_Prefab = data.Prefab;
                m_GUID = data.GUID;
                m_Owner = data.Owner;
                m_Position = data.Position.ToVector();
                m_Rotation = data.Rotation.ToQuaternion();
            }
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

            public bool IsAlive(Server ServerInstance)
            {
                if (ServerInstance != null)
                {
                    foreach (int PlayerID in m_Players)
                    {
                        PlayerData Player = ServerInstance.m_PlayersData.GetPlayer(PlayerID);
                        if(Player != null)
                        {
                            if(Player.m_GamePlayState == PlayerData.GamePlayState.Alive)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
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
                if(m_Players.Count >= PlayersDataManager.c_SquadLimit)
                {
                    return false;
                }
                
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
                    if (ServerInstance.m_Rules.m_HUDMode == "Lobby")
                    {
                        List<NetPeer> peers = new List<NetPeer>();
                        ServerInstance.m_Instance.GetConnectedPeers(peers);
                        foreach (NetPeer Peer in peers.ToArray())
                        {
                            ServerSend.SendHUDSideBarUpdate(Peer, 2, ServerInstance.m_PlayersData.GetPlayersString(), ServerInstance);
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

        public class FallingProp
        {
            public string m_GUID = "";
            public Vector3 m_SpawnPosition = Vector3.Zero;
            public Vector3 m_LandPosition = Vector3.Zero;
            public DateTime m_LandTime;
            public DateTime m_StartFallTime;

            public Vector3 GetVelocityPerSecond()
            {
                TimeSpan fallDuration = m_LandTime - m_StartFallTime;
                float totalSeconds = (float)fallDuration.TotalSeconds;

                if (totalSeconds <= 0)
                    return Vector3.Zero;

                Vector3 displacement = m_LandPosition - m_SpawnPosition;

                return displacement / totalSeconds;
            }
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

        public class Vector2JSON
        {
            public float x { get; set; }
            public float y { get; set; }

            public Vector2 ToVector()
            {
                return new Vector2(x, y);
            }

            public Vector2JSON()
            {
                x = 0;
                y = 0;
            }

            public Vector2JSON(float X, float Y)
            {
                x = X;
                y = Y;
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

        public class CookingSlotData
        {
            public string m_GearGUID = string.Empty;

            public bool IsEmpty()
            {
                return string.IsNullOrEmpty(m_GearGUID);
            }
        }

        public class FireSyncData
        {
            public string m_GUID = string.Empty;
            public Vector3 m_Position = Vector3.Zero;
            public Quaternion m_Rotation = Quaternion.Identity;
            public int m_FireState = 0;

            public bool m_IsForge = false;
            public bool m_IsDynamic = false;

            public float m_LastUpdateTime = 0;
            public float m_MaxOnTODSeconds = 0;
            public float m_ElapsedOnTODSeconds = 0;
            public float m_FuelHeatIncrease = 0;
            public float m_Heat = 0;
            public float m_MaxHeat = 0;

            public bool m_EmbersActive = false;
            public float m_EmberTimer = 0;
            public int m_NumGeneratedCharcoalPieces = 0;
            public float m_HeatInnerRadius = 0;
            public float m_HeatOuterRadius = 0;
            public float m_TimeToReachMaxTempInSeconds = 0;

            public const float c_EmbersDuration = 300;

            public List<CookingSlotData> m_CookingSlots = new List<CookingSlotData>();

            public FireSyncData() { }
            public FireSyncData(SaveData data)
            {
                Load(data);
            }

            public class SaveData
            {
                public string GUID { get; set; }
                public bool IsDynamic { get; set; }
                public Vector3JSON Position { get; set; }
                public QuaternionJSON Rotation { get; set; }
                public int FireState { get; set; }
                public bool IsFroge { get; set; }
                public float LastUpdateTime { get; set; }
                public float MaxOnTODSeconds { get; set; }
                public float ElapsedOnTODSeconds { get; set; }
                public float FuelHeatIncrease { get; set; }
                public float Heat { get; set; }
                public float MaxHeat { get; set; }
                public bool EmbersActive { get; set; }
                public float EmbersTimer { get; set; }
                public int Coals { get; set; }
                public float HeatInner { get; set; }
                public float HeatOuter { get; set; }
                public float TimeToReachMaxTempSeconds { get; set; }
            }

            public SaveData Save()
            {
                SaveData data = new SaveData();

                data.GUID = m_GUID;
                data.IsDynamic = m_IsDynamic;

                if (m_IsDynamic)
                {
                    data.Position = new Vector3JSON(m_Position.X, m_Position.Y, m_Position.Z);
                    data.Rotation = new QuaternionJSON(m_Rotation.X, m_Rotation.Y, m_Rotation.Z, m_Rotation.W);
                }
                data.FireState = m_FireState;
                data.IsFroge = m_IsForge;
                data.LastUpdateTime = m_LastUpdateTime;
                data.MaxOnTODSeconds = m_MaxOnTODSeconds;
                data.ElapsedOnTODSeconds = m_ElapsedOnTODSeconds;
                data.FuelHeatIncrease = m_FuelHeatIncrease;
                data.Heat = m_Heat;
                data.MaxHeat = m_MaxHeat;
                data.EmbersActive = m_EmbersActive;
                data.EmbersTimer = m_EmberTimer;
                data.Coals = m_NumGeneratedCharcoalPieces;
                data.HeatInner = m_HeatInnerRadius;
                data.HeatOuter = m_HeatOuterRadius;
                data.TimeToReachMaxTempSeconds = m_TimeToReachMaxTempInSeconds;
                return data;
            }

            public void Load(SaveData data)
            {
                m_GUID = data.GUID;
                m_IsDynamic = data.IsDynamic;
                if (m_IsDynamic)
                {
                    m_Position = data.Position.ToVector();
                    m_Rotation = data.Rotation.ToQuaternion();
                }
                m_FireState = data.FireState;
                m_IsForge = data.IsFroge;
                m_LastUpdateTime = data.LastUpdateTime;
                m_MaxOnTODSeconds = data.MaxOnTODSeconds;
                m_ElapsedOnTODSeconds = data.ElapsedOnTODSeconds;
                m_FuelHeatIncrease = data.FuelHeatIncrease;
                m_Heat = data.Heat;
                m_MaxHeat = data.MaxHeat;
                m_EmbersActive = data.EmbersActive;
                m_EmberTimer = data.EmbersTimer;
                m_NumGeneratedCharcoalPieces = data.Coals;
                m_HeatInnerRadius = data.HeatInner;
                m_HeatOuterRadius = data.HeatOuter;
                m_TimeToReachMaxTempInSeconds = data.TimeToReachMaxTempSeconds;

            }

            public void AddFuel(float BurnTime, float Heat, float InnerRadius, float OuterRadius)
            {
                if(m_ElapsedOnTODSeconds > m_MaxOnTODSeconds)
                {
                    m_ElapsedOnTODSeconds = m_MaxOnTODSeconds;
                }

                m_MaxOnTODSeconds += BurnTime;

                if(m_MaxOnTODSeconds > 43200)
                {
                    m_MaxOnTODSeconds = 43200;
                }

                m_FuelHeatIncrease += Heat;

                if(m_FuelHeatIncrease > m_MaxHeat)
                {
                    m_FuelHeatIncrease = m_MaxHeat;
                }

                if(InnerRadius > m_HeatInnerRadius)
                {
                    m_HeatInnerRadius = InnerRadius;
                }

                if(OuterRadius > m_HeatOuterRadius)
                {
                    m_HeatOuterRadius = OuterRadius;
                }
            }

            public float RemaningTime()
            {
                float TimeLeft = m_MaxOnTODSeconds - m_ElapsedOnTODSeconds;

                if(TimeLeft < 0) // Игра использует минусовое значение для просчёта тления
                {
                    TimeLeft = 0;
                }
                return TimeLeft;
            }

            public void Unlit(string SceneName, Server ServerInstance)
            {
                m_NumGeneratedCharcoalPieces += (int)MathF.Floor((m_MaxOnTODSeconds / 60) / 60); // Игра даёт 1 уголь за каждый час горения огня.
                m_FireState = 0;
                m_MaxOnTODSeconds = 0;
                m_ElapsedOnTODSeconds = 0;
                m_EmbersActive = false;
                m_EmberTimer = 0;
                m_Heat = 0;
                m_FuelHeatIncrease = 0;

                SceneData SceneData = ServerInstance.m_ScenesData.GetSceneData(SceneName);

                if (SceneData != null)
                {
                    foreach (CookingSlotData Slot in m_CookingSlots)
                    {
                        if (Slot != null)
                        {
                            if (!string.IsNullOrEmpty(Slot.m_GearGUID))
                            {
                                GearDataContainer GearData = null;

                                if (SceneData.m_Gears.TryGetValue(Slot.m_GearGUID, out GearData))
                                {
                                    GearData.m_Visual.StopCooking();
                                    ServerSend.SendGearVisual(GearData.m_Visual, SceneName, ServerInstance);
                                }
                            }
                        }
                    }
                }
            }

            public bool TakeTorch()
            {
                float SecondsOfBurnTime = RemaningTime();

                if(SecondsOfBurnTime > 600) // Факел требует (и потом отнимит) 10 минут. от горения
                {
                    m_ElapsedOnTODSeconds += 600; // Добавляет что прошло 10 минут горения. Да это так всрато в игре работает.
                    m_FuelHeatIncrease--; // отнимает 1 грдус. Да, забирание факела влеяет и на это.
                    return true;
                }
                return false;
            }

            public int TakeCharcoal()
            {
                int Charcoal = m_NumGeneratedCharcoalPieces;
                m_NumGeneratedCharcoalPieces = 0;
                return Charcoal;
            }

            public int GetFreeCookingSlot()
            {
                if(m_CookingSlots.Count == 0)
                {
                    return -1;
                }
                for (int i = 0; i < m_CookingSlots.Count; i++)
                {
                    if (CheckSlotIsFree(i))
                    {
                        return i;
                    }
                }
                return -1;
            }

            public bool CheckSlotIsFree(int SlotIndex)
            {
                if(SlotIndex < 0)
                {
                    return false;
                }

                if(SlotIndex > m_CookingSlots.Count-1)
                {
                    return false;
                }

                CookingSlotData Slot = m_CookingSlots[SlotIndex];

                if (Slot == null)
                {
                    return false;
                }
                else
                {
                    return Slot.IsEmpty();
                }
            }

            public void ClearCookingSlot(int SlotIndex)
            {
                if (SlotIndex < 0)
                {
                    return;
                }
                if (SlotIndex > m_CookingSlots.Count - 1)
                {
                    return;
                }
                CookingSlotData Slot = m_CookingSlots[SlotIndex];

                if (Slot != null)
                {
                    Slot.m_GearGUID = string.Empty;
                }
            }

            public bool SetGearForCooking(GearDataVisual GearVisual, int SlotIndex)
            {
                if(GearVisual == null)
                {
                    return false;
                }
                if(SlotIndex < 0)
                {
                    return false;
                }
                if(SlotIndex > m_CookingSlots.Count - 1)
                {
                    return false;
                }
                CookingSlotData Slot = m_CookingSlots[SlotIndex];

                if (Slot != null)
                {
                    Slot.m_GearGUID = GearVisual.m_GUID;

                    if(m_FireState == 6) // Full burn
                    {
                        if (GearVisual.m_HasCookingRecipe)
                        {
                            GearVisual.StartCooking(m_LastUpdateTime);
                        }
                    }

                    return true;
                }
                return false;
            }

            public CookingSlotData GetSlot(int SlotIndex)
            {
                if (SlotIndex < 0)
                {
                    return null;
                }
                if (SlotIndex > m_CookingSlots.Count - 1)
                {
                    return null;
                }
                return m_CookingSlots[SlotIndex];
            }

            public void Ignite(float Fuel, float Heat, float InnerRadius, float OuterRadius, float CurrentTime, string SceneName, Server ServerInstance)
            {
                if(m_FireState == 0)
                {
                    m_FireState = 6; // FullBurn
                    m_ElapsedOnTODSeconds = 0;
                    m_MaxOnTODSeconds = Fuel;
                    m_FuelHeatIncrease = Heat;
                    m_EmbersActive = false;
                    m_EmberTimer = 0;
                }
                else
                {
                    m_MaxOnTODSeconds += Fuel;
                    m_FuelHeatIncrease += Heat;
                }
                m_HeatInnerRadius = InnerRadius;
                m_HeatOuterRadius = OuterRadius;
                m_LastUpdateTime = CurrentTime;

                SceneData SceneData = ServerInstance.m_ScenesData.GetSceneData(SceneName);

                if(SceneData != null)
                {
                    foreach (CookingSlotData Slot in m_CookingSlots)
                    {
                        if (Slot != null)
                        {
                            if (string.IsNullOrEmpty(Slot.m_GearGUID))
                            {
                                GearDataContainer GearData = null;

                                if(SceneData.m_Gears.TryGetValue(Slot.m_GearGUID, out GearData))
                                {
                                    if (GearData.m_Visual.m_HasCookingRecipe)
                                    {
                                        GearData.m_Visual.StartCooking(CurrentTime);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public static FireSyncData Create(string GUID, float Fuel, float Heat, float InnerRadius, float OuterRadius, float HeatingSpeed, bool IsForge, int CookingSlots, bool IsDynamic, float CurrentTime, string SceneName, Server ServerInstance)
            {
                FireSyncData Fire = new FireSyncData();

                Fire.m_GUID = GUID;
                Fire.m_IsDynamic = IsDynamic;

                Fire.m_TimeToReachMaxTempInSeconds = HeatingSpeed;
                Fire.m_MaxHeat = IsForge ? 200 : 80;

                Fire.Ignite(Fuel, Heat, InnerRadius, OuterRadius, CurrentTime, SceneName, ServerInstance);

                Fire.m_CookingSlots = new List<CookingSlotData>();

                for (int i = 0; i < CookingSlots; i++)
                {
                    Fire.m_CookingSlots.Add(new CookingSlotData());
                }

                return Fire;
            }
        }

        public class WeatherSyncData
        {
            public int m_WeatherSeed = 0;
            public int m_LowTempSeed = 0;
            public int m_HighTempSeed = 0;
            public int m_WindSeed = 0;

            public int m_CurrentWeatherType = 0;
            public int m_PreviousWeatherType = 0;
            public float m_WindDirection = 0;

            public float m_Duration = 0;
            public float m_TransitionTime = 0;
            public float m_NormalizedTime = 0;
            public float m_WindDuration = 0;
            public float m_WindElapsedHours = 0;
        }

        public class ScenesLootSpawns
        {
            public List<SceneLootSpawns> Scenes { get; set; }
        }

        public class SceneLootSpawns
        {
            public string SceneName { get; set; }
            public List<PrefabSpawnData> PrefabSpawns { get; set; }
            public List<RandomSpawnObjectData> RandomSpawnObjects { get; set; }
            public List<LooseGearSpawn> LooseGearSpawns { get; set; }
            public List<RadialObjectSpawnerData> RadialSpawns { get; set; }
            public List<SpawnGearVariantData> SpawnGearVariants { get; set; }
        }

        public class PrefabSpawnData
        {
            public List<GearSpawnElementData> Gears { get; set; }
            public int m_NumToSpawnMin { get; set; }
            public int m_NumToSpawnMax { get; set; }
            public int m_ChanceOfNoSpawn { get; set; }
            public bool IsDLC { get; set; }
            public List<string> DisabledForXP { get; set; }
            public List<string> EnabledForXP { get; set; }
        }

        public class GearSpawnElementData
        {
            public string GearName { get; set; }
            public Vector3JSON Position { get; set; }
            public QuaternionJSON Rotation { get; set; }
            public int SpawnWeight { get; set; }
            public float Chance { get; set; }

            public List<string> DisabledForXP { get; set; }
            public List<string> EnabledForXP { get; set; }
        }

        public class RandomSpawnObjectData
        {
            public List<GearSpawnElementData> Gears { get; set; }
            public int NumObjectsToSpawnPilgrim { get; set; }
            public int NumObjectsToSpawnVoyageur { get; set; }
            public int NumObjectsToSpawnStalker { get; set; }
            public int NumObjectsToSpawnInterloper { get; set; }
            public float RerollAfterGameHours { get; set; }

            public bool IsDLC { get; set; }

            public List<string> DisabledForXP { get; set; }
            public List<string> EnabledForXP { get; set; }
        }

        public class LooseGearSpawn
        {
            public string GearName { get; set; }
            public Vector3JSON Position { get; set; }
            public QuaternionJSON Rotation { get; set; }
            public float Chance { get; set; }
            public bool IsDLC { get; set; }

            public List<string> DisabledForXP { get; set; }
            public List<string> EnabledForXP { get; set; }
        }

        public class RadialObjectSpawnerElementData
        {
            public string GearName { get; set; }
            public int SpawnWeight { get; set; }
            public float Chance { get; set; }
        }

        public class RadialObjectSpawnerData
        {
            public List<RadialObjectSpawnerElementData> Gears { get; set; }
            public int MinToSpawn { get; set; }
            public int MaxToSpawn { get; set; }
            public float MinRespawnTimeGameHours { get; set; }
            public float MaxRespawnTimeGameHours { get; set; }
            public bool IsDLC { get; set; }

            public List<Vector3JSON> PossiblePoints { get; set; }

            public List<string> DisabledForXP { get; set; }
            public List<string> EnabledForXP { get; set; }
        }

        public class SpawnGearVariantData
        {
            public List<SpawnGearVariantElementData> Gears { get; set; }
        }

        public class SpawnGearVariantElementData
        {
            public string GearName { get; set; }
            public Vector3JSON Position { get; set; }
            public QuaternionJSON Rotation { get; set; }
        }
    }
}
