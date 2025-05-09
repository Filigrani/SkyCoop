using LiteNetLib;
using LiteNetLib.Utils;
using System.Numerics;

namespace SkyCoopServer
{
    public class ServerHandle
    {
        public static void Welcome(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string Message = Reader.ReadString();
            Console.WriteLine("[GameServer] Сlient " + Client .Id+ " responced with message: " + Message);
            ServerSend.ServerConfig(Client, ServerInstance.m_Config);
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
            int PlayerID = Reader.GetInt();
            int BodyPart = Reader.GetInt();
            bool Melee = Reader.GetBool();
            string GunType = Reader.GetString();
            ServerSend.SendDamageToPlayer(ServerInstance.GetClient(PlayerID), Damage, PlayerID, BodyPart, Melee, GunType);

            ServerInstance.m_PlayersData.m_Players[PlayerID].DealDamage(Client.Id, Damage, GunType);
        }
        public static void ClientProjectile(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            Vector3 Pos = Reader.ReadVector3();
            Quaternion Rot = Reader.ReadQuaternion();
            string ProjectileName = Reader.GetString();
            ServerSend.SendProjectile(Client, Pos, Rot, ProjectileName, ServerInstance);
        }
    }
}
