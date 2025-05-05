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
    }
}
