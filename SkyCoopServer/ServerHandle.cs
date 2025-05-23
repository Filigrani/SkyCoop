using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.VisualBasic;
using System;
using System.Numerics;
using static SkyCoopServer.DataStr;

namespace SkyCoopServer
{
    public class ServerHandle
    {
        public static void Welcome(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string PlayerName = Reader.GetString();
            Logger.Log(ConsoleColor.Green, $"[ServerHandle] Сlient {Client.Id} connected under name: {PlayerName}");
            ServerInstance.m_PlayersData.SetPlayerName(Client.Id, PlayerName);
            ServerSend.ServerConfig(Client, ServerInstance.m_Config, ServerInstance.m_Rules);

            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
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
            Vector3 Position = Reader.GetVector3();
            ServerInstance.m_PlayersData.PlayerMoved(Client.Id, Position);
        }

        public static void ClientRotation(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            Quaternion Rotation = Reader.GetQuaternion();
            ServerInstance.m_PlayersData.PlayerRotated(Client.Id, Rotation);
        }

        public static void ClientScene(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string Scene = Reader.GetString();
            Logger.Log($"[ServerHandle] (ClientScene) Client {Client.Id} sent Scene {Scene}");
            ServerInstance.m_PlayersData.PlayerChangeScene(Client.Id, Scene);
        }

        public static void ClientHoldingGear(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GearName = Reader.GetString();
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

            if (!ServerInstance.m_Rules.m_PVP && Killer != Victim)
            {
                return;
            }

            if (ServerInstance.m_PlayersData.m_Players[Victim].m_GamePlayState == DataStr.PlayerData.GamePlayState.Alive)
            {
                ServerSend.SendDamageToPlayer(ServerInstance.GetClient(Victim), Damage, Killer, BodyPart, WeaponName);
                ServerSend.SendGettingDamage(Victim, ServerInstance);

                ServerInstance.m_PlayersData.m_Players[Victim].DealDamage(Killer, Damage, DamageType);
            }
        }
        public static void ClientProjectile(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            Vector3 Pos = Reader.GetVector3();
            Quaternion Rot = Reader.GetQuaternion();
            string ProjectileName = Reader.GetString();
            float ExtaFloat = Reader.GetFloat();
            ServerSend.SendProjectile(Client, Pos, Rot, ProjectileName, ExtaFloat, ServerInstance);
        }
        public static void ClientDied(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            int DamageI = Reader.GetInt();
            DataStr.DamageType DamageType = (DataStr.DamageType)DamageI;
            bool Knocked = Reader.GetBool();
            bool HeadShot = Reader.GetBool();
            Logger.Log($"[ServerHandle] ClientDied {DamageType.ToString()} Knocked {Knocked} HeadShot {HeadShot}");
            ServerInstance.GetPlayerDataByNetPeer(Client).ConfirmKill(ServerInstance, DamageType, Knocked, HeadShot);
        }
        public static void ClientRevived(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            int Reviver = Reader.GetInt();
            ServerInstance.GetPlayerDataByNetPeer(Client).Revived(Reviver);
            if(Reviver == -2)
            {
                ServerSend.SendRemoveAllInjectedItem(Client.Id, ServerInstance);
            }
        }
        public static void ClientProjectileThrow(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            Vector3 Pos = Reader.GetVector3();
            Quaternion Rot = Reader.GetQuaternion();
            string ProjectileName = Reader.GetString();
            Vector3 Velocity = Reader.GetVector3();
            Vector3 AngularVelocity = Reader.GetVector3();
            float Fuse = Reader.GetFloat();
            ServerSend.SendProjectileThrow(Client, Pos, Rot, ProjectileName, Velocity, AngularVelocity, Fuse, ServerInstance);
        }
        public static void ClientRequestRespawn(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            Logger.Log($"[ServerHandle] ClientRequestRespawn PlayerID {Client.Id} m_GamePlayState: {ServerInstance.GetPlayerDataByNetPeer(Client).m_GamePlayState.ToString()}");
            if (ServerInstance.m_PlayersData.m_Players[Client.Id].m_GamePlayState == DataStr.PlayerData.GamePlayState.Dead)
            {
                DataStr.V3Quat Point = ServerInstance.m_ScenesData.GetSpawnPoint(ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene);
                ServerSend.SendPlayerRespawn(Client, Point.m_Position, Point.m_Rotation);
            }
        }

        public static void ClientInjectedItem(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            int PlayerID = Reader.GetInt();
            string GearName = Reader.GetString();
            int ObjectID = Reader.GetInt();
            int DamageZone = Reader.GetInt();
            Vector3 Position = Reader.GetVector3();
            Quaternion Rotation = Reader.GetQuaternion();

            if(!ServerInstance.m_Rules.m_PVP && Client.Id != PlayerID)
            {
                return;
            }

            DataStr.InjectedItem injectedItem = new DataStr.InjectedItem();
            injectedItem.m_GearName = GearName;
            injectedItem.m_ObjectID = ObjectID;
            injectedItem.m_DamageZone = DamageZone;
            injectedItem.m_Position = Position;
            injectedItem.m_Rotation = Rotation;

            ServerInstance.GetPlayerDataByNetPeer(ServerInstance.GetClient(PlayerID)).m_VisualData.m_InjectedItems.Add(injectedItem);

            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                if (Peer.Id != PlayerID || ServerInstance.m_PlayersData.m_RecursiveDebug)
                {
                    ServerSend.SendInjectedItem(Peer, PlayerID, GearName, ObjectID, Position, Rotation);
                }
            }
        }

        public static void ClientRemoveInjectedItem(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            int PlayerID = Reader.GetInt();
            string GearName = Reader.GetString();
            int DamageZone = Reader.GetInt();

            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                if (Peer.Id != PlayerID || ServerInstance.m_PlayersData.m_RecursiveDebug)
                {
                    ServerSend.SendRemoveInjectedItem(Peer, PlayerID, GearName, DamageZone);
                }
            }

            PlayerData Data = ServerInstance.GetPlayerDataByNetPeer(ServerInstance.GetClient(PlayerID));

            for (int i = 0; i < Data.m_VisualData.m_InjectedItems.Count; i++)
            {
                if (Data.m_VisualData.m_InjectedItems[i].m_GearName == GearName && Data.m_VisualData.m_InjectedItems[i].m_DamageZone == DamageZone)
                {
                    Data.m_VisualData.m_InjectedItems.RemoveAt(i);
                    break;
                }
            }
        }
        public static void ClientEraceAllInjectedItems(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            ServerSend.SendRemoveAllInjectedItem(Client.Id, ServerInstance);
        }

        public static void ClientSendGear(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GearName = Reader.GetString();
            Vector3 Position = Reader.GetVector3();
            Quaternion Rotation = Reader.GetQuaternion();
            string JSON = Reader.GetString();

            ServerInstance.m_ScenesData.AddGear(ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene, GearName, Position, Rotation, JSON);
        }

        public static void ClientPickUpGear(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GUID = Reader.GetString();

            DataStr.GearDataContainer GearData = ServerInstance.m_ScenesData.GetGear(ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene, GUID, true);

            if(GearData == null)
            {
                ServerSend.SendPickUpGearFailed(Client);
            }
            else
            {
                ServerSend.SendPickUpGear(Client, GearData.m_Visual.m_GearName, GearData.m_Data.m_JSON);
            }
        }

        public static void ClientLoadedScene(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string SceneName = Reader.GetString();
            ServerInstance.m_ScenesData.SendAllGears(SceneName, Client);
            ServerInstance.m_ScenesData.SendAllOpenables(SceneName, Client);
            ServerInstance.m_ScenesData.SendZone(SceneName, Client);
            ServerInstance.m_PlayersData.SendAllPlayersOnScene(Client, SceneName);

            if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_HUDMode == "DMStats")
            {
                PlayerData Data = ServerInstance.GetPlayerDataByNetPeer(Client);
                ServerSend.SendHUDSideBar(Client, 0, "ico_Reload", $"Kills:", Data.m_Kills.ToString(), ServerInstance);
                ServerSend.SendHUDSideBar(Client, 1, "icoMap_grave", $"Deaths:", Data.m_Deaths.ToString(), ServerInstance);
                ServerSend.SendHUDSideBar(Client, 2, "ico_Status_BuffPlus", $"Assists:", Data.m_Assists.ToString(), ServerInstance);
            }

            if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_Time > 0)
            {
                ServerSend.ClientGameModeTimer(ServerInstance.m_Rules.m_Time, ServerInstance);
            }
            DataStr.V3Quat Point = ServerInstance.m_ScenesData.GetSpawnPoint(SceneName);
            ServerSend.SendPlayerRespawn(Client, Point.m_Position, Point.m_Rotation, false);
        }

        public static void ClientOpenableInteraction(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GUID = Reader.GetString();
            bool OpenState = Reader.GetBool();

            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                if (Peer.Id != Client.Id)
                {
                    ServerSend.SendOpenableState(Peer, GUID, OpenState);
                }
            }
            ServerInstance.m_ScenesData.AddOpenableState(ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene, GUID, OpenState);
        }

        public static void ClientClothing(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            int ClothingRegion = Reader.GetInt();
            string GearName = Reader.GetString();
            PlayerData Data = ServerInstance.GetPlayerDataByNetPeer(Client);
            if(ClothingRegion == 0)
            {
                Data.m_VisualData.m_HeadGear = GearName;
            }else if(ClothingRegion == 1)
            {
                Data.m_VisualData.m_BodyGear = GearName;
            }

            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                if (Peer.Id != Client.Id || ServerInstance.m_PlayersData.m_RecursiveDebug)
                {
                    ServerSend.SendClothing(Peer, ClothingRegion, GearName, Client.Id);
                }
            }
        }
        public static void ClientTryInteract(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GUID = Reader.GetString();
            foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
            {
                PlayerData Data = ServerInstance.GetPlayerDataByNetPeer(Peer);
                if (Peer.Id != Client.Id)
                {
                    if(Data.m_CarSeat == GUID)
                    {
                        ServerSend.SendInteractResult(Client, false);
                        return;
                    }
                }
            }
            ServerSend.SendInteractResult(Client, true);
        }
        public static void ClientVehicleSeat(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GUID = Reader.GetString();
            PlayerData Data = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (!string.IsNullOrEmpty(GUID))
            {
                foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
                {
                    PlayerData OtherData = ServerInstance.GetPlayerDataByNetPeer(Peer);

                    if (Peer.Id != Client.Id)
                    {
                        if (OtherData.m_CarSeat == GUID)
                        {
                            return;
                        }
                    }
                }
            }
            Data.m_CarSeat = GUID;
        }
        public static void ClientInVehicle(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            bool IsInVehicle = Reader.GetBool();
            ServerInstance.m_PlayersData.PlayerChangeVehicleState(Client.Id, IsInVehicle);
        }
    }
}
