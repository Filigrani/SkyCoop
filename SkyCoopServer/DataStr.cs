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
            public string m_GameMode = "Stalker";
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

            public GamePlayState m_GamePlayState = GamePlayState.Alive;

            public enum GamePlayState
            {
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
                for(int i = 0;i < m_Damagers.Count; i++)
                {
                    Damager Dmg = m_Damagers[i];
                    Console.WriteLine(i+". PlayerID " + Dmg.m_ClientID+" Damage: "+ Dmg.m_Damage +" Type "+Dmg.m_DamageType.ToString());
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
                    Console.WriteLine("Suicide, nothing to log");
                }
            }

            public void Revived(int Reviver)
            {
                //if(Reviver == m_PlayerID)
                //{
                //    Console.WriteLine($"Player {m_PlayerID} revived himself.");
                //}else if(Reviver == -1)
                //{
                //    Console.WriteLine($"Player {m_PlayerID} respawned.");
                //}else
                //{
                //    Console.WriteLine($"Player {m_PlayerID} revived by Player {Reviver}");
                //}

                if(Reviver == -2)
                {
                    m_GamePlayState = GamePlayState.Alive;
                }

                m_Damagers.Clear();
            }
        }

        public class PlayerVisualData
        {
            public bool m_Crouch = false;
            public string m_GearInHands = "";
            public int m_GearVariant = 0;
            public int m_LatAction = 0;
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
    }
}
