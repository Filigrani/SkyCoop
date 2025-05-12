using LiteNetLib.Utils;
using SkyCoopServer;
using UnityEngine;
using static SkyCoopServer.Packet;

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

            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.Welcome);
            writer.Put(ModMain.GetNickName());
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

        public static void SendDamageToPlayer(float Damage, int Victim, Comps.PlayerDamageColider.DamageZone BodyPart, string Weapon, DataStr.DamageType DamageType, int Killer = -1)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientDamageOtherClient);
            writer.Put(Damage);
            writer.Put(Victim);
            writer.Put((int)BodyPart);
            writer.Put(Weapon);
            writer.Put((int)DamageType); // Only for server, this won't be send back to clients.
            writer.Put(Killer);
            SendToHost(writer);
        }
        public static void SendProjectile(Vector3 Position, Quaternion Rotation, string ProjectileName)
        {
            //SkyCoop.Logger.Log("SendProjectile " + ProjectileName);
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientProjectile);
            writer.Write(Position);
            writer.Write(Rotation);
            writer.Put(ProjectileName);
            SendToHost(writer);
        }
        public static void SendDeath(DataStr.DamageType DamageType, bool Knocked, bool HeadShot)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientDied);
            writer.Put((int)DamageType);
            writer.Put(Knocked);
            writer.Put(HeadShot);
            SendToHost(writer);
        }
        public static void SendRevived(int Reviver)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientRevived);
            writer.Put(Reviver);
            SendToHost(writer);
        }
        public static void SendProjectileThrow(Vector3 Position, Quaternion Rotation, string ProjectileName, Vector3 Velocity, Vector3 AngularVelocity, float fuseTime)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientProjectileThrow);
            writer.Write(Position);
            writer.Write(Rotation);
            writer.Put(ProjectileName);
            writer.Write(Velocity);
            writer.Write(AngularVelocity);
            writer.Put(fuseTime);
            SendToHost(writer);
        }
    }
}
