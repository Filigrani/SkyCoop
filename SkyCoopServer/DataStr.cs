using LiteNetLib;
using System;
using System.IO.Compression;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Linq;
using static SkyCoopServer.DataStr;

namespace SkyCoopServer
{
    public class DataStr
    {
        public class ServerConfig
        {
            public int m_MaxPlayers = 4;
            public string m_StartingRegion = "ForlornMuskeg";
            public int m_Seed = 777777;
            public int m_VoicePort = 37850;
            //public int m_VoicePort = 0;
            public string m_ExperienceMode = "Stalker";
            public string m_SceneToSpawn = "HuntingLodgeA";
            public string m_GameMode = "Lobby";
        }

        public class GameRulesSave
        {
            public bool Knockdowns { get; set; }
            public bool PVP { get; set; }
            public List<StartingGearData> StartingGear { get; set; }
            public int Time { get; set; }
            public string HUDMode { get; set; }
            public bool DeathPacks { get; set; }
            public bool Respawns { get; set; }
        }

        public class GameRules
        {
            public bool m_PlayerCanBeKnocked = false;
            public bool m_PVP = true;
            public List<StartingGearData> m_StartingItems = new List<StartingGearData>();
            public int m_Time = 0;
            public string m_HUDMode = "";
            public bool m_DeathPacks = false;
            public bool m_Respawns = false;
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
            public StartingGearData() { }
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

            public string m_Scene = "";

            public List<Damager> m_Damagers = new List<Damager>();
            public int m_LastDamager = -1;
            public int m_PreLastDamager = -1;

            public int m_Kills = 0;
            public int m_Deaths = 0;
            public int m_Assists = 0;

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
                        m_GamePlayState = GamePlayState.Dead;
                    }
                    else
                    {
                        m_GamePlayState = GamePlayState.Spectator;
                    }
                    Logger.Log("[DataStr] PlayerID " + m_PlayerID + " m_GamePlayState: " + m_GamePlayState.ToString());

                    if(ServerInstance.m_Rules.m_HUDMode == "Shrink")
                    {
                        foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
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
                    m_GamePlayState = GamePlayState.Alive;
                    if (ServerInstance.m_Rules.m_HUDMode == "Shrink")
                    {
                        foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
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
        }

        public class PlayerVisualData
        {
            public bool m_Crouch = false;
            public bool m_InVehicle = false;
            public string m_GearInHands = "";
            public int m_GearVariant = 0;
            public int m_LatAction = 0;
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
            public DataStr.DangerCircleData m_ActiveZone = null;
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

        public class SpawnPoint
        {
            public float posx { get; set; }
            public float posy { get; set; }
            public float posz { get; set; }

            public float rotx { get; set; }
            public float roty { get; set; }
            public float rotz { get; set; }
            public float rotw { get; set; }
        }
        public class SpawnPointSave
        {
            public List<SpawnPoint> points { get; set; }
        }

        public class V3Quat
        {
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Quaternion m_Rotation = new Quaternion(0,0,0,0);

            public V3Quat(float posx, float posy, float posz, float rotx, float roty, float rotz, float rotw)
            {
                m_Position = new Vector3(posx, posy, posz);
                m_Rotation = new Quaternion(rotx, roty, rotz, rotw);
            }

            public V3Quat() { }
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
            public float ShrinkSpeed { get; set; }
            public float DamagePerSecond { get; set; }
            public int StageTime { get; set; }
        }

        public class DangerCircleData
        {
            public DangerCircleConfig m_Config = new DangerCircleConfig();
            public int m_CurrentStage = 0;
            public float m_CurrentRadius = 0;
            public float m_TargetRadius = 0;
            public Vector3 m_CurrentCenter = Vector3.Zero;
            public Vector3 m_TargetCenter = Vector3.Zero;
            public float m_MovementSpeed = 0;

            private float s_NextStageIn = 0;
            private bool s_Started = false;
            private bool s_Finished = false;
            private bool s_Static = false;
            private DateTime s_NextStageCall;
            private bool s_NextStageTimerActive = false;
            private string s_SceneName = "";
            private Server s_ServerInstance;

            public string GetTimerPrefix()
            {
                if (!s_Finished)
                {
                    return "Next zone movement";
                }else if (!s_NextStageTimerActive)
                {
                    return "Time Remaining";
                }
                return "Time Remaining";
            }

            public DataStr.ShrinkStage GetCurrentStage()
            {
                return m_Config.Stages[m_CurrentStage];
            }

            public DataStr.ShrinkStage GetNextStage()
            {
                int Index = m_CurrentStage+1;
                if (Index < m_Config.Stages.Count)
                {
                    return m_Config.Stages[Index];
                }
                else
                {
                    return null;
                }
            }

            public DangerCircleData(){}

            public DangerCircleData(DangerCircleConfig Config, string SceneName, Server Server)
            {
                m_Config = Config;
                s_SceneName = SceneName;
                s_ServerInstance = Server;
            }

            public void Start()
            {
                if (!s_Started)
                {
                    DoNextStage();
                    s_Started = true;
                }
            }

            public int GetApproximateShrinkingTime()
            {
                if (m_CurrentRadius <= m_TargetRadius)
                {
                    return 0;
                }

                if (m_CurrentRadius > m_TargetRadius)
                {
                    float RemaningRadius = m_CurrentRadius - m_TargetRadius;
                    float RemaningSecods = RemaningRadius / GetCurrentStage().ShrinkSpeed;
                    return (int) MathF.Ceiling(RemaningSecods);
                }
                return 0;
            }

            public Vector3 GetNewRandomCenter()
            {
                // Legal bounds for zone that ever can be is radius from ActualCenter to MaximumRadius
                Vector3 ActualCenter = new Vector3(m_Config.ActualCenter.x, m_Config.ActualCenter.y, m_Config.ActualCenter.z);
                float MaximumRadius = m_Config.StartingRadius;

                // Current bounds of zone
                Vector3 CurrentCenter = m_CurrentCenter;
                float CurrentRadius = m_CurrentRadius;
                float TargetRadius = m_TargetRadius;

                // How much distance new center can be from previous center
                float MaximumDistanceFromOldCenter = CurrentRadius - TargetRadius;

                System.Random RNG = new System.Random(Guid.NewGuid().GetHashCode());

                // Generate random direction vector
                Vector3 randomDirection = new Vector3(
                    (float)(RNG.NextDouble() * 2 - 1),  // -1 to 1
                    0,
                    (float)(RNG.NextDouble() * 2 - 1)   // -1 to 1
                );

                // Normalize the direction vector manually
                float length = MathF.Sqrt(randomDirection.X * randomDirection.X +
                                         randomDirection.Y * randomDirection.Y +
                                         randomDirection.Z * randomDirection.Z);

                if (length > 0)
                {
                    randomDirection = new Vector3(
                        randomDirection.X / length,
                        randomDirection.Y / length,
                        randomDirection.Z / length
                    );
                }

                // Generate random distance within allowed range
                float randomDistance = (float)RNG.NextDouble() * MaximumDistanceFromOldCenter;

                // Calculate new center position
                Vector3 NewRandomCenter = CurrentCenter + randomDirection * randomDistance;

                // Ensure the new center stays within the maximum allowed radius from ActualCenter
                Vector3 offsetFromActual = NewRandomCenter - ActualCenter;
                float distanceFromActualCenter = MathF.Sqrt(offsetFromActual.X * offsetFromActual.X +
                                                          offsetFromActual.Y * offsetFromActual.Y +
                                                          offsetFromActual.Z * offsetFromActual.Z);

                if (distanceFromActualCenter > MaximumRadius - TargetRadius)
                {
                    // Adjust to stay within bounds
                    if (distanceFromActualCenter > 0)
                    {
                        Vector3 directionFromActual = new Vector3(
                            offsetFromActual.X / distanceFromActualCenter,
                            offsetFromActual.Y / distanceFromActualCenter,
                            offsetFromActual.Z / distanceFromActualCenter
                        );
                        NewRandomCenter = ActualCenter + directionFromActual * (MaximumRadius - TargetRadius);
                    }
                }

                return NewRandomCenter;
            }

            public void Update()
            {
                //SkyCoopServer.Logger.Log($"DangerCircleData Update() Radius {m_CurrentRadius}/{m_TargetRadius} Stage: {m_CurrentStage+1}/{m_Config.Stages.Count}");
                if (s_Started)
                {
                    if (!s_Static)
                    {
                        if (s_NextStageTimerActive)
                        {
                            if(s_NextStageCall <= DateTime.Now)
                            {
                                OnNextStage();
                            }
                        }

                        bool NeedSendUpdate = false;

                        if (m_CurrentRadius > m_TargetRadius)
                        {
                            m_CurrentRadius -= GetCurrentStage().ShrinkSpeed;
                            if (m_CurrentRadius <= m_TargetRadius)
                            {
                                m_CurrentRadius = m_TargetRadius;
                                StartNextStageTimer();
                            }
                            NeedSendUpdate = true;
                            ServerSend.SendZoneUpdate(s_SceneName, m_CurrentCenter, m_CurrentRadius, s_ServerInstance);
                        }

                        if (m_CurrentCenter != m_TargetCenter)
                        {
                            float Distance = Vector3.Distance(m_CurrentCenter, m_TargetCenter);
                            Vector3 direction = m_TargetCenter - m_CurrentCenter;
                            Vector3 movement = direction * MathF.Min(m_MovementSpeed, Distance);
                            m_CurrentCenter += movement;
                            if (Distance < m_MovementSpeed)
                            {
                                m_CurrentCenter = m_TargetCenter;
                            }
                            NeedSendUpdate = true;
                        }


                        if (NeedSendUpdate)
                        {
                            ServerSend.SendZoneUpdate(s_SceneName, m_CurrentCenter, m_CurrentRadius, s_ServerInstance);
                        }

                        if (!s_Finished)
                        {
                            if (s_ServerInstance != null && s_ServerInstance.m_Rules.m_HUDMode == "Shrink")
                            {
                                if (s_NextStageTimerActive)
                                {
                                    TimeSpan remainingTime = s_NextStageCall - DateTime.Now;
                                    ServerSend.ClientGameModeTimer(remainingTime.Seconds, s_ServerInstance);
                                }
                                else
                                {
                                    ShrinkStage NextStage = GetNextStage();
                                    if (NextStage == null)
                                    {
                                        ServerSend.ClientGameModeTimer(GetApproximateShrinkingTime(), s_ServerInstance);
                                    }
                                    else
                                    {
                                        ServerSend.ClientGameModeTimer(GetApproximateShrinkingTime() + NextStage.StageTime, s_ServerInstance);
                                    }
                                }
                            }
                        }
                    }
                    DamageCheck();
                }
            }

            public void DamageCheck()
            {
                foreach (NetPeer Peer in s_ServerInstance.m_Instance.ConnectedPeerList.ToList())
                {
                    PlayerData PlayerData = s_ServerInstance.GetPlayerDataByNetPeer(Peer);
                    if(PlayerData.m_GamePlayState == PlayerData.GamePlayState.Alive)
                    {
                        float Distance = Vector2.Distance(new Vector2(PlayerData.m_Position.X, PlayerData.m_Position.Z), new Vector2(m_Config.ActualCenter.x, m_Config.ActualCenter.z)) * 100;
                        //SkyCoopServer.Logger.Log($"DangerCircleData PlayerID {Peer.Id} Distance {Distance}/{m_CurrentRadius}");
                        if (Distance > m_CurrentRadius)
                        {
                            ServerSend.SendDamageToPlayer(Peer, GetCurrentStage().DamagePerSecond, Peer.Id, 1, "ZONE");
                        }
                    }
                }
            }

            void StartNextStageTimer()
            {
                s_NextStageCall = DateTime.Now.AddSeconds(GetCurrentStage().StageTime);
                s_NextStageTimerActive = true;
            }

            void ClearStageTimer()
            {
                s_NextStageTimerActive = false;
            }

            void DoNextStage()
            {
                if(m_CurrentStage == 0)
                {
                    m_CurrentRadius = m_Config.StartingRadius;
                    m_TargetRadius = m_Config.StartingRadius;
                    m_CurrentCenter = new Vector3(m_Config.ActualCenter.x, m_Config.ActualCenter.y, m_Config.ActualCenter.z);
                    m_TargetCenter = m_CurrentCenter;
                    s_Static = GetCurrentStage().StageTime <= 0;
                    if (!s_Static)
                    {
                        StartNextStageTimer();
                    }
                }
                else
                {
                    int Stage = m_CurrentStage+1;
                    int MaxStage = m_Config.Stages.Count;
                    m_TargetRadius = m_Config.StartingRadius-(m_Config.StartingRadius*Stage/MaxStage);
                    //m_TargetCenter = GetNewRandomCenter();
                    //m_MovementSpeed = Vector3.Distance(m_CurrentCenter, m_TargetCenter) / GetApproximateShrinkingTime();
                    SkyCoopServer.Logger.Log($"GetNewRandomCenter {m_TargetCenter}");
                    ClearStageTimer();
                }
            }

            void OnNextStage()
            {
                if(m_CurrentStage == m_Config.Stages.Count-1)
                {
                    s_Finished = true;
                    ClearStageTimer();
                }
                else
                {
                    m_CurrentStage++;
                    DoNextStage();
                }
                if (s_ServerInstance != null && s_ServerInstance.m_Rules.m_HUDMode == "Shrink")
                {
                    ServerSend.UpdateTimerPrefix(GetTimerPrefix(), s_ServerInstance);
                    if (s_Finished)
                    {
                        s_ServerInstance.m_Rules.m_Time = 120;
                    }
                }
            }
        }

        public class DangerCircleCenter
        {
            public float x { get; set; }
            public float y { get; set; }
            public float z { get; set; }
        }

        public class DangerCircleConfig
        {
            public DangerCircleCenter ActualCenter { get; set; }
            public float StartingRadius { get; set; }
            public List<ShrinkStage> Stages { get; set; }
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

            public PlayersSquad(string SquadName)
            {
                m_Name = SquadName;
            }

            public bool HasPlayer(int PlayerID)
            {
                return m_Players.Contains(PlayerID);
            }

            public void AddPlayer(int PlayerID)
            {
                if (!m_Players.Contains(PlayerID))
                {
                    m_Players.Add(PlayerID);
                }
            }

            public void RemovePlayer(int PlayerID)
            {
                m_Players.Remove(PlayerID);
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
        public class PropDataSave
        {
            public List<PropData> props { get; set; }
        }
        public class PropData
        {
            public string prefabname { get; set; }
            public bool frombundle { get; set; }
            public float posx { get; set; }
            public float posy { get; set; }
            public float posz { get; set; }

            public float rotx { get; set; }
            public float roty { get; set; }
            public float rotz { get; set; }
            public float rotw { get; set; }
            public string guid { get; set; }
        }
    }
}
