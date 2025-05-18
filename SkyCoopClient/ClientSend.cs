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
            writer.Put(Position);
            SendToHost(writer);
        }

        public static void SendRotation(Quaternion Rotation)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientRotation);
            writer.Put(Rotation);
            SendToHost(writer);
        }

        public static void SendScene(string Scene)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientScene);
            writer.Put(Scene);

            SendToHost(writer);
        }

        public static void SendHoldingGear(string GearName, int GearVariant)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientHoldigGear);
            writer.Put(GearName);
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
            if(ModMain.Client != null && !ModMain.Client.m_Rules.m_PVP)
            {
                if(ModMain.Client.GetMyId() != Victim)
                {
                    return;
                }
            }
            
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
        public static void SendProjectile(Vector3 Position, Quaternion Rotation, string ProjectileName, float ExtaFloat = 1)
        {
            //SkyCoop.Logger.Log("SendProjectile " + ProjectileName);
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientProjectile);
            writer.Put(Position);
            writer.Put(Rotation);
            writer.Put(ProjectileName);
            writer.Put(ExtaFloat);
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
            writer.Put(Position);
            writer.Put(Rotation);
            writer.Put(ProjectileName);
            writer.Put(Velocity);
            writer.Put(AngularVelocity);
            writer.Put(fuseTime);
            SendToHost(writer);
        }
        public static void SendRespawnRequest()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientRequestRespawn);
            SendToHost(writer);
        }

        public static void SendInjectedItem(int PlayerID, string GearName, int ObjectIndex, Comps.PlayerDamageColider.DamageZone DamageZone, Vector3 Position, Quaternion Rotation)
        {
            if (ModMain.Client != null && !ModMain.Client.m_Rules.m_PVP)
            {
                if (ModMain.Client.GetMyId() != PlayerID)
                {
                    return;
                }
            }
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientInjectedItem);

            writer.Put(PlayerID);
            writer.Put(GearName);
            writer.Put(ObjectIndex);
            writer.Put((int)DamageZone);
            writer.Put(Position);
            writer.Put(Rotation);

            SendToHost(writer);
        }
        public static void SendRemoveInjectedItem(int PlayerID, string GearName, Comps.PlayerDamageColider.DamageZone DamageZone)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientRemoveInjectedItem);

            writer.Put(PlayerID);
            writer.Put(GearName);
            writer.Put((int)DamageZone);

            SendToHost(writer);
        }

        public static void SendEraceAllInjectedItems()
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientEraceAllInjectedItems);
            SendToHost(writer);
        }

        public static void SendGear(string GearName, Vector3 Position, Quaternion Rotation, string JSON)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientSendGear);

            writer.Put(GearName);
            writer.Put(Position);
            writer.Put(Rotation);
            writer.Put(JSON);


            //SkyCoop.Logger.Log(ConsoleColor.Green, $"ClientSend.SendGear {GearName}");
            //SkyCoop.Logger.Log(ConsoleColor.Green, $"JSON: {JSON}");

            SendToHost(writer);
        }

        public static void SendGearPickUp(string GUID)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientPickUpGear);

            writer.Put(GUID);

            SkyCoop.Logger.Log(ConsoleColor.Green, $"ClientSend.SendGearPickUp {GUID}");

            SendToHost(writer);
        }
        public static void SendNewScene(string SceneName)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientLoadedScene);

            writer.Put(SceneName);

            SendToHost(writer);
        }

        public static void SendOpenableState(string GUID, bool OpenState)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientOpenableInteraction);

            writer.Put(GUID);
            writer.Put(OpenState);

            SkyCoop.Logger.Log($"SendOpenableState {GUID} {OpenState}");

            SendToHost(writer);
        }

        public static void SendClothing(int ClothingRegion, string GearName)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientClothing);

            writer.Put(ClothingRegion);
            writer.Put(GearName);

            SendToHost(writer);
        }
    }
}
