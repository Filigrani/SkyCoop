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
            public int m_PlayerID = 0;
            public PlayerVisualData m_VisualData = new PlayerVisualData();

            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Quaternion m_Rotation = new Quaternion(0, 0, 0, 0);

            public string m_Scene = "";

            public List<Damager> m_Damagers = new List<Damager>();

            public PlayerData(int PlayerID)
            {
                m_PlayerID = PlayerID;
            }

            public void DealDamage(int ClientID, float Damage, string DType)
            {
                DamageType dType;
                switch (DType)
                {
                    case "Revolver":
                        dType = DamageType.Revolver; 
                        break;
                    case "Rifle":
                        dType = DamageType.Rifle;
                        break;
                    case "Flaregun":
                        dType = DamageType.Flaregun;
                        break;
                    case "Bow":
                        dType = DamageType.Bow;
                        break;
                    default:
                        dType = DamageType.Bloodloss;
                        break;
                }

                m_Damagers.Add(new Damager(ClientID, Damage, dType));
            }
            public void ConfirmKill(int ClientID) 
            {
                if(m_Damagers.Count > 0)
                {
                    m_Damagers.Sort();
                    if (m_Damagers[0].m_ClientID == ClientID) 
                    {
                        Console.WriteLine($"Player {m_Damagers[0].m_ClientID} and wepon is {m_Damagers[0].m_DamageType} with assistance {m_Damagers[1].m_ClientID} and wepon is {m_Damagers[1].m_DamageType} kill player {m_PlayerID}");
                    }
                    else
                    {
                        Console.WriteLine($"Player {ClientID} and wepon is ? with assistance {m_Damagers[0].m_ClientID} and wepon is {m_Damagers[0].m_DamageType} kill player {m_PlayerID}");
                    }
                    m_Damagers.Clear();
                }
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
            public readonly int m_ClientID;
            public readonly float m_Damage;
            public readonly DamageType m_DamageType;

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

        public enum DamageType
        {
            Revolver,
            Rifle,
            Flaregun,
            Bow,
            Bloodloss
        }
    }
}
