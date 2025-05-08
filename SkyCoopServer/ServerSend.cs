using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace SkyCoopServer
{
    public class ServerSend
    {
        public static void Welcome(NetPeer Client, int id)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.Welcome);
            writer.Put(id);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void ServerConfig(NetPeer Client, DataStr.ServerConfig CFG)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.CFG);
            writer.Put(CFG.m_MaxPlayers);
            writer.Put(CFG.m_Seed);
            writer.Write(CFG.m_StartingRegion);
            writer.Write(CFG.m_GameMode);
            writer.Put(CFG.m_VoicePort);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendPosition(NetPeer Client, Vector3 Position, int FromClient)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientPosition);
            writer.Put(FromClient);
            writer.Write(Position);
            Client.Send(writer, DeliveryMethod.Unreliable);
        }

        public static void SendRotation(NetPeer Client, Quaternion Rotation, int FromClient)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientRotation);
            writer.Put(FromClient);
            writer.Write(Rotation);
            Client.Send(writer, DeliveryMethod.Unreliable);
        }

        public static void SendPlayerSceneNotification(NetPeer Client, bool Present, int FromClient)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientScene);
            writer.Put(FromClient);
            writer.Put(Present);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendPlayerChangeGear(NetPeer Client, string GearName, int GearVariant, int FromClient)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientHoldigGear);
            writer.Put(FromClient);
            writer.Write(GearName);
            writer.Put(GearVariant);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }
        public static void SendPlayerCrouch(NetPeer Client, bool CrouchState, int FromClient)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientCrouch);
            writer.Put(FromClient);
            writer.Put(CrouchState);
            Client.Send(writer, DeliveryMethod.Unreliable);
        }

        public static void SendPlayerAction(NetPeer Client, int Action, int FromClient)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientAction);
            writer.Put(FromClient);
            writer.Put(Action);
            Client.Send(writer, DeliveryMethod.Unreliable);
        }

        public static void SendPlayerFire(NetPeer Client, int FromClient)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientFire);
            writer.Put(FromClient);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }
        public static void SendDamageToPlayer(NetPeer Client, float Damage, int PlayerID, int BodyPart, bool Melee, string MeleeWeapon = "")
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientDamageOtherClient);
            writer.Put(Damage);
            writer.Put(PlayerID);
            writer.Put(BodyPart);
            writer.Put(Melee);
            writer.Put(MeleeWeapon);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }
        public static void SendProjectile(NetPeer Client, Vector3 Position, Quaternion Rotation, string ProjectileName, Server ServerInstance)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientProjectile);
            writer.Write(Position);
            writer.Write(Rotation);
            writer.Put(ProjectileName);

            DataStr.PlayerData Shooter = ServerInstance.m_PlayersData.GetPlayer(Client.Id);

            foreach (DataStr.PlayerData p in ServerInstance.m_PlayersData.m_Players)
            {
                if(p.m_PlayerID != Shooter.m_PlayerID || ServerInstance.m_PlayersData.m_RecursiveDebug)
                {
                    if(Shooter.m_Scene == p.m_Scene)
                    {
                        NetPeer Peer = ServerInstance.GetClient(p.m_PlayerID);
                        if (Peer != null)
                        {
                            Peer.Send(writer, DeliveryMethod.ReliableOrdered);
                        }
                    }
                }
            }
        }
    }
}
