using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyCoopServer;
using UnityEngine;
using static SkyCoopServer.Packet;
using Il2Cpp;

namespace SkyCoop
{
    public class ClientSend
    {
        public static void SendToHost(NetDataWriter writer)
        {
            if (ModMain.Client != null && ModMain.Client.m_Instance != null)
            {
                ModMain.Client.SendToHost(writer);
            }
        }

        public static void Welcome()
        {
            //TODO: Send here MAC address and nick name.

            string Message = "I am connected!";
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.Welcome);
            writer.Write(Message);
            SendToHost(writer);
        }

        public static void SendPosition(Vector3 Position)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientPosition);
            writer.Write(Position);
            SendToHost(writer);
        }

        public static void SendRotation(Quaternion Rotation)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientRotation);
            writer.Write(Rotation);
            SendToHost(writer);
        }

        public static void SendScene(string Scene)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientScene);
            writer.Write(Scene);

            SendToHost(writer);
        }

        public static void SendHoldingGear(string GearName, int GearVariant)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientHoldigGear);
            writer.Write(GearName);
            writer.Put(GearVariant);
            SendToHost(writer);
        }

        public static void SendCrouch(bool Crouch)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientCrouch);
            writer.Put(Crouch);
            SendToHost(writer);
        }
        public static void SendAction(int Action)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientAction);
            writer.Put(Action);
            SendToHost(writer);
        }
        public static void SendFire()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientFire);
            SendToHost(writer);
        }

        public static void SendVoice(byte[] VoiceSample)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(VoiceSample);
            SendToHost(writer);
        }
    }
}
