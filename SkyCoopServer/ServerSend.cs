using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.VisualBasic;
using System;
using System.Numerics;
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

        public static void ServerConfig(NetPeer Client, DataStr.ServerConfig CFG, DataStr.GameRules Rules)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.CFG);
            writer.Put(CFG);
            writer.Put(Rules);

            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendPosition(NetPeer Client, Vector3 Position, int FromClient)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientPosition);
            writer.Put(FromClient);
            writer.Put(Position);
            Client.Send(writer, DeliveryMethod.Unreliable);
        }

        public static void SendRotation(NetPeer Client, Quaternion Rotation, int FromClient)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientRotation);
            writer.Put(FromClient);
            writer.Put(Rotation);
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
            writer.Put(GearName);
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
        public static void SendDamageToPlayer(NetPeer Client, float Damage, int Killer, int BodyPart, string WeaponName)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientDamageOtherClient);
            writer.Put(Damage);
            writer.Put(Killer);
            writer.Put(BodyPart);
            writer.Put(WeaponName);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }
        public static void SendProjectile(NetPeer Client, Vector3 Position, Quaternion Rotation, string ProjectileName, float ExtaFloat, Server ServerInstance)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientProjectile);
            writer.Put(Client.Id);
            writer.Put(Position);
            writer.Put(Rotation);
            writer.Put(ProjectileName);
            writer.Put(ExtaFloat);

            DataStr.PlayerData Shooter = ServerInstance.m_PlayersData.GetPlayer(Client.Id);
            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                if(Peer.Id != Shooter.m_PlayerID || ServerInstance.m_PlayersData.m_RecursiveDebug)
                {
                    if(Shooter.m_Scene == ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene)
                    {
                        Peer.Send(writer, DeliveryMethod.ReliableOrdered);
                    }
                }
            }
        }
        public static void SendKillFeed(DataStr.KillFeedMessage Message, Server ServerInstance)
        {
            if(Message.m_Killer != -1 && Message.m_Victim != -1)
            {
                if (Message.m_Killer != Message.m_Victim)
                {
                    ServerInstance.m_PlayersData.GetPlayer(Message.m_Killer).AddKill(ServerInstance);
                }
                else
                {
                    ServerInstance.m_PlayersData.GetPlayer(Message.m_Killer).RemoveKill(ServerInstance);
                }
                ServerInstance.m_PlayersData.GetPlayer(Message.m_Victim).AddDeath(ServerInstance);
            }

            if(Message.m_Assist != -1)
            {
                if (Message.m_Assist != Message.m_Victim)
                {
                    ServerInstance.m_PlayersData.GetPlayer(Message.m_Assist).AddAssist(ServerInstance);
                }
            }


            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.KillFeedMessage);
            writer.Put(Message);

            string frags = "\n";
            foreach (DataStr.KillFeedFlag Flag in Message.m_Flags)
            {
                frags += $"{Flag.ToString()}\n";
            }
            Logger.Log($"[ServerSend] SendKillFeed\n- m_Killer: {Message.m_Killer}\n- m_Victim: {Message.m_Victim}\n- m_Assist: {Message.m_Assist}\n - m_DeathReason: {Message.m_DeathReason.ToString()}\n- m_Flags:{frags}");

            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                Peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }
        public static void SendProjectileThrow(NetPeer Client, Vector3 Position, Quaternion Rotation, string ProjectileName, Vector3 Velocity, Vector3 AngVelocity, float Fuse, Server ServerInstance)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientProjectileThrow);
            writer.Put(Client.Id);
            writer.Put(Position);
            writer.Put(Rotation);
            writer.Put(ProjectileName);
            writer.Put(Velocity);
            writer.Put(AngVelocity);
            writer.Put(Fuse);

            DataStr.PlayerData Shooter = ServerInstance.m_PlayersData.GetPlayer(Client.Id);

            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                if (Peer.Id != Shooter.m_PlayerID || ServerInstance.m_PlayersData.m_RecursiveDebug)
                {
                    if (Shooter.m_Scene == ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene)
                    {
                        if (Peer != null)
                        {
                            Peer.Send(writer, DeliveryMethod.ReliableOrdered);
                        }
                    }
                }
            }
        }

        public static void SendClientName(NetPeer Client, int ClientID, string Name)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientName);
            writer.Put(Name);
            writer.Put(ClientID);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendPlayerRespawn(NetPeer Client, Vector3 Position, Quaternion Rotation, bool RespawnAnim = true)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientRequestRespawn);
            writer.Put(Position);
            writer.Put(Rotation);
            writer.Put(RespawnAnim);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendInjectedItem(NetPeer Client, int PlayerID, string GearName, int ObjectIndex, Vector3 Position, Quaternion Rotation)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientInjectedItem);

            writer.Put(PlayerID);
            writer.Put(GearName);
            writer.Put(ObjectIndex);
            writer.Put(Position);
            writer.Put(Rotation);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendRemoveInjectedItem(NetPeer Client, int PlayerID, string GearName, int DamageZone)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientRemoveInjectedItem);

            writer.Put(PlayerID);
            writer.Put(GearName);
            writer.Put(DamageZone);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendRemoveAllInjectedItem(int PlayerID, Server ServerInstance)
        {
            DataStr.PlayerData Data = ServerInstance.m_PlayersData.GetPlayer(PlayerID);
            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                if (Peer.Id != PlayerID || ServerInstance.m_PlayersData.m_RecursiveDebug)
                {
                    for (int i = 0; i < Data.m_VisualData.m_InjectedItems.Count; i++)
                    {
                        DataStr.InjectedItem Item = Data.m_VisualData.m_InjectedItems[i];
                        ServerSend.SendRemoveInjectedItem(Peer, PlayerID, Item.m_GearName, Item.m_DamageZone);
                    }
                }
            }
            Data.m_VisualData.m_InjectedItems.Clear();
        }

        public static void SendGettingDamage(int PlayerID, Server ServerInstance)
        {
            DataStr.PlayerData Data = ServerInstance.m_PlayersData.GetPlayer(PlayerID);

            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientGettingDamage);
            writer.Put(PlayerID);

            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                if (Peer.Id != PlayerID || ServerInstance.m_PlayersData.m_RecursiveDebug)
                {
                    Peer.Send(writer, DeliveryMethod.ReliableOrdered);
                }
            }
        }
        public static void SendGearVisual(DataStr.GearDataVisual Visual, string SceneName, Server ServerInstance)
        {
            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                if(ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == SceneName)
                {
                    SendGearVisual(Visual, Peer);
                }
            }
        }

        public static void SendGearVisual(DataStr.GearDataVisual Visual, NetPeer Client)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientSendGear);
            writer.Put(Visual);
            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendPickUpGear(NetPeer Client, string GearName, string JSON)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientPickUpGear);

            writer.Put(true); // First bool here because ClientPickUpGear also re-use failed pickup packet, so, if client reads first bool as false, it's indicated this is failed pickup package.
            writer.Put(GearName);
            writer.Put(JSON);

            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendPickUpGearFailed(NetPeer Client)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientPickUpGear);

            writer.Put(false); // Read about it in SendPickUpGear method.

            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendGearRemoved(string GUID, string SceneName, Server ServerInstance)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientRemoveGear);

            writer.Put(GUID);

            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                if(ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == SceneName)
                {
                    Peer.Send(writer, DeliveryMethod.ReliableOrdered);
                }
            }
        }
        public static void SendOpenableState(NetPeer Client, string GUID, bool OpenState, bool AllowAudio = true)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientOpenableInteraction);

            writer.Put(GUID);
            writer.Put(OpenState);
            writer.Put(AllowAudio);

            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendZoneUpdate(NetPeer Client, string SceneName, DataStr.DangerCircleCenter Center, float Radius, Server ServerInstance)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientZoneUpdated);

            writer.Put(Center.x);
            writer.Put(Center.y);
            writer.Put(Center.z);
            writer.Put(Radius);

            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendZoneUpdate(string SceneName, DataStr.DangerCircleCenter Center, float Radius, Server ServerInstance)
        {
            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                if (ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == SceneName)
                {
                    SendZoneUpdate(Peer, SceneName, Center, Radius, ServerInstance);
                }
            }
        }

        public static void ClientGameModeTimer(int Seconds, Server ServerInstance)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientGameModeTimer);

            writer.Put(Seconds);
            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                Peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }
        public static void SendHUDSideBar(NetPeer Client, int SideBarIndex, string Icon, string Prefix, string Afix, Server ServerInstance)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientHUDSideBar);

            writer.Put(SideBarIndex);
            writer.Put(Icon);
            writer.Put(Prefix);
            writer.Put(Afix);

            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }
        public static void SendHUDSideBarUpdate(NetPeer Client, int SideBarIndex, string Afix, Server ServerInstance)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientHUDSideBarUpdate);

            writer.Put(SideBarIndex);
            writer.Put(Afix);

            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendFreeze(NetPeer Client)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientFreeze);

            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendConfigUpdated(Server ServerInstance)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ServerConfigUpdated);

            writer.Put(ServerInstance.m_Config);
            writer.Put(ServerInstance.m_Rules);

            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToList())
            {
                Peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }
        public static void SendChangeMap(Server ServerInstance)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ServerChangesMap);

            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToList())
            {
                Peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }
        public static void SendLeaders(List<int> Leaders, Vector3 Position ,Server ServerInstance)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ServerLeaders);

            writer.Put(Leaders.Count);

            for (int i = 0; i < Leaders.Count; i++)
            {
                writer.Put(Leaders[i]);
            }

            writer.Put(Position);

            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToList())
            {
                Peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }
        public static void SendInteractResult(NetPeer Client, bool Success)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.Type.ClientTryInteract);

            writer.Put(Success);

            Client.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public static void SendPlayerInVehicle(NetPeer Client, bool State, int FromClient)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientInVehicle);
            writer.Put(FromClient);
            writer.Put(State);
            Client.Send(writer, DeliveryMethod.Unreliable);
        }

        public static void SendClothing(NetPeer Client, int Region, string GearName, int FromID)
        {
            NetDataWriter writer = new NetDataWriter();

            writer.Put((int)Packet.Type.ClientClothing);
            writer.Put(Region);
            writer.Put(GearName);
            writer.Put(FromID);
            Client.Send(writer, DeliveryMethod.Unreliable);
        }
    }
}
