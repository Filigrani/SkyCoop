using LiteNetLib;
using LiteNetLib.Utils;
using System.Numerics;

namespace SkyCoopServer
{
    public class ServerHandle
    {
        public static void Welcome(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string PlayerName = Reader.GetString();
            Console.WriteLine("[GameServer] Сlient " + Client.Id+ " connected under name: " + PlayerName);
            ServerInstance.m_PlayersData.SetPlayerName(Client.Id, PlayerName);
            ServerSend.ServerConfig(Client, ServerInstance.m_Config);

            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList)
            {
                ServerSend.SendClientName(Client, Peer.Id, ServerInstance.m_PlayersData.GetPlayer(Peer.Id).m_PlayerName);
                if(Peer.Id != Client.Id)
                {
                    ServerSend.SendClientName(Peer, Client.Id, ServerInstance.m_PlayersData.GetPlayer(Client.Id).m_PlayerName);
                }
            }
        }

        public static void ClientPosition(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            Vector3 Position = Reader.ReadVector3();
            ServerInstance.m_PlayersData.PlayerMoved(Client.Id, Position);
        }

        public static void ClientRotation(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            Quaternion Rotation = Reader.ReadQuaternion();
            ServerInstance.m_PlayersData.PlayerRotated(Client.Id, Rotation);
        }

        public static void ClientScene(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string Scene = Reader.ReadString();
            Console.WriteLine("[GameServer] (ClientScene) Client " + Client.Id + " sent Scene "+Scene);
            ServerInstance.m_PlayersData.PlayerChangeScene(Client.Id, Scene);
        }

        public static void ClientHoldingGear(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GearName = Reader.ReadString();
            int GearVariant = Reader.GetInt();
            ServerInstance.m_PlayersData.PlayerChangeGear(Client.Id, GearName, GearVariant);
        }

        public static void ClientCrouch(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            bool IsCrouch = Reader.GetBool();
            ServerInstance.m_PlayersData.PlayerChangeCrouch(Client.Id, IsCrouch);
        }

        public static void ClientAction(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            int Action = Reader.GetInt();
            ServerInstance.m_PlayersData.PlayerChangeAction(Client.Id, Action);
        }

        public static void ClientFire(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            ServerInstance.m_PlayersData.PlayerFire(Client.Id);
        }
        public static void ClientDamageOtherClient(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            float Damage = Reader.GetFloat();
            int Victim = Reader.GetInt();
            int BodyPart = Reader.GetInt();
            string WeaponName = Reader.GetString();
            DataStr.DamageType DamageType = (DataStr.DamageType)Reader.GetInt(); // Just for server, won't send it back to clients.
            int Killer = Reader.GetInt();
            if(Killer == -1)
            {
                Killer = Client.Id;
            }

            if(ServerInstance.m_PlayersData.m_Players[Victim].m_GamePlayState == DataStr.PlayerData.GamePlayState.Alive)
            {
                ServerSend.SendDamageToPlayer(ServerInstance.GetClient(Victim), Damage, Killer, BodyPart, WeaponName);

                ServerInstance.m_PlayersData.m_Players[Victim].DealDamage(Killer, Damage, DamageType);
            }
        }
        public static void ClientProjectile(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            Vector3 Pos = Reader.ReadVector3();
            Quaternion Rot = Reader.ReadQuaternion();
            string ProjectileName = Reader.GetString();
            ServerSend.SendProjectile(Client, Pos, Rot, ProjectileName, ServerInstance);
        }
        public static void ClientDied(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            int DamageI = Reader.GetInt();
            DataStr.DamageType DamageType = (DataStr.DamageType)DamageI;
            Console.WriteLine("[GameServer] DamageI " + DamageI + " DamageType " + DamageType.ToString());
            bool Knocked = Reader.GetBool();
            bool HeadShot = Reader.GetBool();
            ServerInstance.GetPlayerDataByNetPeer(Client).ConfirmKill(ServerInstance, DamageType, Knocked, HeadShot);
        }
        public static void ClientRevived(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            int Reviver = Reader.GetInt();
            ServerInstance.GetPlayerDataByNetPeer(Client).Revived(Reviver);
        }
        public static void ClientProjectileThrow(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            Vector3 Pos = Reader.ReadVector3();
            Quaternion Rot = Reader.ReadQuaternion();
            string ProjectileName = Reader.GetString();
            Vector3 Velocity = Reader.ReadVector3();
            Vector3 AngularVelocity = Reader.ReadVector3();
            float Fuse = Reader.GetFloat();
            ServerSend.SendProjectileThrow(Client, Pos, Rot, ProjectileName, Velocity, AngularVelocity, Fuse, ServerInstance);
        }
        public static void ClientRequestRespawn(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            if (ServerInstance.m_PlayersData.m_Players[Client.Id].m_GamePlayState == DataStr.PlayerData.GamePlayState.Dead)
            {
                string FileName = ServerInstance.m_PlayersData.m_Players[Client.Id].m_Scene;

                DataStr.V3Quat Point = PlayersDataManager.GetSpawnPoint(FileName);
                ServerSend.SendPlayerRespawn(Client, Point.m_Position, Point.m_Rotation);
            }
        }
    }
}
