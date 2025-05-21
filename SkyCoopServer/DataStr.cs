using LiteNetLib;
using System.Numerics;

namespace SkyCoopServer
{
    public class DataStr
    {
        public class ServerConfig
        {
            public int m_MaxPlayers = 4;
            public string m_StartingRegion = "ForlornMuskeg";
            public int m_Seed = 777777;
            //public int m_VoicePort = 37850;
            public int m_VoicePort = 0;
            public string m_ExperienceMode = "Stalker";
            public string m_SceneToSpawn = "LakeRegion";
            public string m_GameMode = "DM";
        }

        public class GameRulesSave
        {
            public bool Knockdowns { get; set; }
            public bool PVP { get; set; }
            public List<StartingGearData> StartingGear { get; set; }
            public int Time { get; set; }
            public string HUDMode { get; set; }
        }

        public class GameRules
        {
            public bool m_PlayerCanBeKnocked = false;
            public bool m_PVP = true;
            public List<StartingGearData> m_StartingItems = new List<StartingGearData>();
            public int m_Time = 0;
            public string m_HUDMode = "";
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
                    m_GamePlayState = GamePlayState.Dead;
                    Logger.Log("[DataStr] PlayerID " + m_PlayerID + " m_GamePlayState: " + m_GamePlayState.ToString());
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

            public void Revived(int Reviver)
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
                }

                m_Damagers.Clear();
            }

            public void AddKill(Server ServerInstance)
            {
                m_Kills++;
                if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "DMStats")
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
            public string m_GearInHands = "";
            public int m_GearVariant = 0;
            public int m_LatAction = 0;
            public List<InjectedItem> m_InjectedItems = new List<InjectedItem>();
            public string m_HeadGear = "";
            public string m_BodyGear = "";

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

        public class DangerCircleData : IDisposable
        {
            public DangerCircleConfig m_Config = new DangerCircleConfig();
            public int m_CurrentStage = 0;
            public float m_CurrentRadius = 0;
            public float m_TargetRadius = 0;

            private float s_NextStageIn = 0;
            private bool s_Started = false;
            private bool s_Finished = false;
            private bool s_Static = false;
            private Timer s_StageTimer;
            private Timer s_SyncTimer;
            private Timer s_DamageTimer;
            private string s_SceneName = "";
            private Server s_ServerInstance;

            public DataStr.ShrinkStage GetCurrentStage()
            {
                return m_Config.Stages[m_CurrentStage];
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
                    s_DamageTimer = new Timer(DamageCheck, null, 1000, 1000);
                    if (!s_Static)
                    {
                        s_SyncTimer = new Timer(Update, null, 500, 500);
                    }
                }
            }

            public void Update(object obj)
            {
                if (s_Started)
                {
                    if(m_CurrentRadius > m_TargetRadius)
                    {
                        m_CurrentRadius -= GetCurrentStage().ShrinkSpeed;
                        if(m_CurrentRadius <= m_TargetRadius)
                        {
                            m_CurrentRadius = m_TargetRadius;
                            if (s_StageTimer != null)
                            {
                                s_StageTimer.Dispose();
                            }
                            s_StageTimer = new Timer(OnNextStage, null, GetCurrentStage().StageTime * 1000, GetCurrentStage().StageTime * 1000);
                        }
                        ServerSend.SendZoneUpdate(s_SceneName, m_Config.ActualCenter, m_CurrentRadius, s_ServerInstance);
                    }
                }
            }

            public void DamageCheck(object obj)
            {
                foreach (NetPeer Peer in s_ServerInstance.m_Instance.ConnectedPeerList.ToList())
                {
                    PlayerData PlayerData = s_ServerInstance.GetPlayerDataByNetPeer(Peer);
                    if(PlayerData.m_GamePlayState == PlayerData.GamePlayState.Alive)
                    {
                        float Distance = Vector3.Distance(PlayerData.m_Position, new Vector3(m_Config.ActualCenter.x, m_Config.ActualCenter.y, m_Config.ActualCenter.z)) * 100;
                        if (Distance > m_CurrentRadius)
                        {
                            ServerSend.SendDamageToPlayer(Peer, GetCurrentStage().DamagePerSecond, Peer.Id, 1, "ZONE");
                        }
                    }
                }
            }

            void DoNextStage()
            {
                if(m_CurrentStage == 0)
                {
                    m_CurrentRadius = m_Config.StartingRadius;
                    m_TargetRadius = m_Config.StartingRadius;
                    if (s_StageTimer != null)
                    {
                        s_StageTimer.Dispose();
                    }
                    s_Static = GetCurrentStage().StageTime <= 0;
                    if (!s_Static)
                    {
                        s_StageTimer = new Timer(OnNextStage, null, GetCurrentStage().StageTime * 1000, GetCurrentStage().StageTime * 1000);
                    }
                }
                else
                {
                    int Stage = m_CurrentStage+1;
                    int MaxStage = m_Config.Stages.Count;
                    m_TargetRadius = m_Config.StartingRadius-(m_Config.StartingRadius*Stage/MaxStage);
                    if (s_StageTimer != null)
                    {
                        s_StageTimer.Dispose();
                    }
                }
            }

            void OnNextStage(object obj)
            {
                if(m_CurrentStage == m_Config.Stages.Count-1)
                {
                    s_Finished = true;
                }
                else
                {
                    m_CurrentStage++;
                    DoNextStage();
                }
            }
            public void Dispose()
            {
                if (s_StageTimer != null)
                {
                    s_StageTimer.Dispose();
                }
                if (s_SyncTimer != null)
                {
                    s_SyncTimer.Dispose();
                }
                if (s_DamageTimer != null)
                {
                    s_DamageTimer.Dispose();
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
    }
}
