using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Numerics;
using static SkyCoopServer.DataStr;
using static System.Formats.Asn1.AsnWriter;

namespace SkyCoopServer
{
    public class ServerHandle
    {
        public static void Welcome(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string PlayerName = Reader.GetString();
            if(!ServerInstance.m_PlayersData.SetPlayerName(Client.Id, PlayerName))
            {
                return;
            }

            string NewPlayerName = ServerInstance.m_PlayersData.GetPlayer(Client.Id).m_PlayerName;
            Logger.Log(ConsoleColor.Green, $"[ServerHandle] Сlient {Client.Id} connected under name: {NewPlayerName}");
            ServerSend.ServerConfig(Client, ServerInstance.m_Config, ServerInstance.m_Rules);
            ServerSend.SendClientName(Client, Client.Id, NewPlayerName);


            ServerInstance.OnPlayersCountChanged();

            List<NetPeer> peers = new List<NetPeer>();
            ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer OtherPeer in peers.ToArray())
            {
                if(OtherPeer.Id != Client.Id)
                {
                    // Старым клиентам от нового
                    ServerSend.SendClientName(OtherPeer, Client.Id, NewPlayerName);
                    // Новоему клиенту от старых клиентов
                    ServerSend.SendClientName(Client, OtherPeer.Id, ServerInstance.m_PlayersData.GetPlayer(OtherPeer.Id).m_PlayerName);
                }
            }
            ServerSend.SendClientStatus(Client.Id, 1, ServerInstance);
            ServerSend.SendAssignSquad(Client, !string.IsNullOrEmpty(ServerInstance.m_PlayersData.GetPlayerNameSquadIn(Client.Id)));
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
            Logger.Log(ConsoleColor.Red, $"[ServerHandle] (ClientScene) Client {Client.Id} sent Legacy scene change packet!");
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
            int VictimID = Reader.GetInt();
            int BodyPart = Reader.GetInt();
            string WeaponName = Reader.GetString();
            DataStr.DamageType DamageType = (DataStr.DamageType)Reader.GetInt(); // Just for server, won't send it back to clients.
            int KillerID = Reader.GetInt();
            if(KillerID == -1)
            {
                KillerID = Client.Id;
            }

            if (!ServerInstance.m_Rules.m_PVP && KillerID != VictimID)
            {
                return;
            }

            PlayerData Victim = ServerInstance.m_PlayersData.GetPlayer(VictimID);
            PlayerData Killer = ServerInstance.m_PlayersData.GetPlayer(KillerID);

            if(Victim != null && Victim.m_GamePlayState == PlayerData.GamePlayState.Alive && Killer != null && Killer.m_GamePlayState == PlayerData.GamePlayState.Alive)
            {
                if(Victim.m_LastRespawn.AddSeconds(3) > DateTime.UtcNow)
                {
                    return;
                }
                if(Killer.m_LastRespawn.AddSeconds(3) > DateTime.UtcNow)
                {
                    return;
                }


                ServerSend.SendDamageToPlayer(ServerInstance.GetClient(VictimID), Damage, KillerID, BodyPart, WeaponName);
                ServerSend.SendGettingDamage(VictimID, ServerInstance); // Анимация

                Victim.DealDamage(KillerID, Damage, DamageType);
            }
        }

        public static void ClientProjectile(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            if (ServerInstance.GetPlayerDataByNetPeer(Client).m_GamePlayState != PlayerData.GamePlayState.Alive && !ServerInstance.CanRespawn())
            {
                return;
            }

            Vector3 Pos = Reader.GetVector3();
            Quaternion Rot = Reader.GetQuaternion();
            string ProjectileName = Reader.GetString();
            float ExtaFloat = Reader.GetFloat();
            bool PlaySound = Reader.GetBool();

            if(ProjectileName == "GEAR_FlareGunAmmoSingle" && ServerInstance.m_Rules.m_AirDrop != null)
            {
                PlayerData PlayerData = ServerInstance.GetPlayerDataByNetPeer(Client);
                if(PlayerData != null)
                {
                    if(!string.IsNullOrEmpty(ServerInstance.m_Rules.m_AirDrop.Prefab) && !string.IsNullOrEmpty(ServerInstance.m_Rules.m_AirDrop.Path))
                    {
                        string AirDropJSON = FilesManager.GetAirDrop(ServerInstance.m_Rules.m_AirDrop.Path);

                        if (!string.IsNullOrEmpty(AirDropJSON))
                        {
                            Vector3 Position = new Vector3(PlayerData.m_Position.X, PlayerData.m_Position.Y+ ServerInstance.m_Rules.m_AirDrop.Altitude, PlayerData.m_Position.Z);
                            Vector3 LandPosition = new Vector3(PlayerData.m_Position.X, PlayerData.m_Position.Y, PlayerData.m_Position.Z);

                            ServerInstance.m_ScenesData.SummonAirDrop(PlayerData.m_Scene, ServerInstance.m_Rules.m_AirDrop.Prefab, AirDropJSON, Position, LandPosition, ServerInstance.m_Rules.m_AirDrop.FallTime);
                        }
                    }
                }
            }

            ServerSend.SendProjectile(Client, Pos, Rot, ProjectileName, ExtaFloat, PlaySound, ServerInstance);
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
            ServerInstance.GetPlayerDataByNetPeer(Client).Revived(Reviver, ServerInstance);
            if(Reviver == -2)
            {
                ServerSend.SendRemoveAllInjectedItem(Client.Id, ServerInstance);
            }
        }
        public static void ClientProjectileThrow(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            if (ServerInstance.GetPlayerDataByNetPeer(Client).m_GamePlayState != PlayerData.GamePlayState.Alive && !ServerInstance.CanRespawn())
            {
                return;
            }
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
            DataStr.PlayerData PlayerData = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (PlayerData != null)
            {
                DataStr.PlayerData.GamePlayState State = PlayerData.m_GamePlayState;

                Logger.Log($"[ServerHandle] ClientRequestRespawn PlayerID {Client.Id} m_GamePlayState: {State.ToString()}");
                if (State == DataStr.PlayerData.GamePlayState.Dead)
                {
                    DataStr.V3Quat Point = ServerInstance.m_ScenesData.GetSpawnPoint(PlayerData.m_Scene, Client.Id);
                    ServerInstance.m_PlayersData.PlayerMoved(Client.Id, Point.m_Position);
                    ServerInstance.m_PlayersData.PlayerRotated(Client.Id, Point.m_Rotation);
                    PlayerData.m_Position = Point.m_Position; // Так мы запишим позицию ДО того как игрок загрузиться. 
                    ServerSend.SendPlayerRespawn(Client, Point.m_Position, Point.m_Rotation);
                }
                else if (State == PlayerData.GamePlayState.Spectator)
                {
                    ServerSend.SendPlayerBecomeSpectator(Client);
                }
            }
        }

        public static void ClientInjectedItem(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            if (ServerInstance.GetPlayerDataByNetPeer(Client).m_GamePlayState != PlayerData.GamePlayState.Alive)
            {
                return;
            }
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

            List<NetPeer> peers = new List<NetPeer>();
            ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers.ToArray())
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

            List<NetPeer> peers = new List<NetPeer>();
            ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers.ToArray())
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
            if (ServerInstance.GetPlayerDataByNetPeer(Client).m_GamePlayState != PlayerData.GamePlayState.Alive && !ServerInstance.CanRespawn())
            {
                return;
            }
            string GearName = Reader.GetString();
            Vector3 Position = Reader.GetVector3();
            Quaternion Rotation = Reader.GetQuaternion();
            string JSON = Reader.GetString();
            float NormalizedCondition = Reader.GetFloat();
            int Style = Reader.GetInt();
            string CookpotGUID = Reader.GetString();

            string FireGUID = "";
            int CookingSlotIndex = -1;

            bool HasCookingSlot = Reader.GetBool();

            if (HasCookingSlot)
            {
                FireGUID = Reader.GetString();
                CookingSlotIndex = Reader.GetInt();
            }

            string RecipeResult = "";
            float Volume = 1;
            float BeingCooked = 0;

            bool HasRecipe = Reader.GetBool();

            if (HasRecipe || !string.IsNullOrEmpty(CookpotGUID))
            {
                RecipeResult = Reader.GetString();
                Volume = Reader.GetFloat();
                BeingCooked = Reader.GetFloat();
            }


            SkyCoopServer.Logger.Log(ConsoleColor.Green, $"ServerHandle.ClientSendGear {GearName} FireGUID {FireGUID} CookingSlot {CookingSlotIndex} CookpotGUID {CookpotGUID}");

            if (!string.IsNullOrEmpty(FireGUID))
            {
                bool CookingSlotIsAvaliable = ServerInstance.m_ScenesData.CookingSlotIsFree(ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene, FireGUID, CookingSlotIndex);

                if (!CookingSlotIsAvaliable)
                {
                    // Слот занят, делаем рефанд
                    SkyCoopServer.Logger.Log(ConsoleColor.Yellow, $"CookingSlot {CookingSlotIndex} is busy, refunding gear");
                    ServerSend.SendPickUpGear(Client, GearName, JSON, true, BeingCooked, RecipeResult, Volume);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(CookpotGUID))
            {
                GearDataContainer CookpotData = ServerInstance.m_ScenesData.GetGear(ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene, CookpotGUID);

                if (CookpotData == null || !string.IsNullOrEmpty(CookpotData.m_Visual.m_CookingResult) || !string.IsNullOrEmpty(CookpotData.m_Visual.m_ProductGUID))
                {
                    // Слот занят, делаем рефанд
                    SkyCoopServer.Logger.Log(ConsoleColor.Yellow, $"CookingPot {CookpotGUID} is busy, refunding gear");
                    ServerSend.SendPickUpGear(Client, GearName, JSON, true, BeingCooked, RecipeResult, Volume);
                    return;
                }
            }

            ScenesDataManager.AddedGearData AddGearResult = ServerInstance.m_ScenesData.AddGear(ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene, GearName, Position, Rotation, JSON, NormalizedCondition, Style, FireGUID, CookingSlotIndex, RecipeResult, Volume, BeingCooked, CookpotGUID);

            if(!string.IsNullOrEmpty(AddGearResult.FireGUID))
            {
                SkyCoopServer.Logger.Log(ConsoleColor.Green, $"Gear linked to fire, sending cooking interation");
                ServerSend.SendCookingInteraction(Client, AddGearResult.GUID, AddGearResult.FireGUID);
            }
            else
            {
                if(!string.IsNullOrEmpty(FireGUID))
                {
                    SkyCoopServer.Logger.Log(ConsoleColor.Red, $"Gear expected to be added to fire, but it did not happend, deleting gear from server and doing refund!");
                    DataStr.GearDataContainer RefundGear = ServerInstance.m_ScenesData.GetGear(ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene, AddGearResult.GUID, true);
                    ServerSend.SendPickUpGear(Client, RefundGear.m_Visual.m_GearName, RefundGear.m_Data.m_JSON, true, BeingCooked, RecipeResult, Volume);
                }
            }
        }

        public static void ClientPickUpGear(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            if (ServerInstance.GetPlayerDataByNetPeer(Client).m_GamePlayState != PlayerData.GamePlayState.Alive)
            {
                ServerSend.SendPickUpGearFailed(Client);
                return;
            }
            string GUID = Reader.GetString();

            List<NetPeer> peers = new List<NetPeer>();
            ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers)
            {
                if (Peer != null)
                {
                    DataStr.PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Peer);

                    if (Player != null)
                    {
                        if(Player.m_InteractionGUID == GUID)
                        {
                            ServerSend.SendPickUpGearFailed(Client);
                            return;
                        }
                    }
                }
            }

            DataStr.GearDataContainer GearData = ServerInstance.m_ScenesData.GetGear(ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene, GUID);

            if(GearData == null)
            {
                ServerSend.SendPickUpGearFailed(Client);
            }
            else
            {
                Logger.Log($"[ServerHandle] (ClientPickUpGear) GearData.m_Visual.m_ProductGUID {GearData.m_Visual.m_ProductGUID} ");
                if (!string.IsNullOrEmpty(GearData.m_Visual.m_ProductGUID)) // Нельзя забрать котелок до того пока внутри него что-то есть, берём то что внутри.
                {
                    DataStr.GearDataContainer ProductData = ServerInstance.m_ScenesData.GetGear(ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene, GearData.m_Visual.m_ProductGUID);

                    if(ProductData != null)
                    {
                        ServerSend.SendPickUpGear(Client, ProductData.m_Visual.m_GearName, ProductData.m_Data.m_JSON, true, GearData.m_Visual.m_BeingCookedTime, ProductData.m_Visual.m_CookingResult, ProductData.m_Visual.m_Volume);
                        ServerInstance.m_ScenesData.RemoveGear(ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene, ProductData.m_Visual.m_GUID);
                        return;
                    }
                }
                ServerSend.SendPickUpGear(Client, GearData.m_Visual.m_GearName, GearData.m_Data.m_JSON, false, GearData.m_Visual.m_BeingCookedTime, GearData.m_Visual.m_CookingResult, GearData.m_Visual.m_Volume);
                ServerInstance.m_ScenesData.RemoveGear(ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene, GUID);
            }
        }

        public static void ClientLoadedScene(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string SceneName = Reader.GetString();

            PlayerData Data = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (Data != null)
            {
                string OldScene = Data.m_Scene;
                ServerInstance.m_PlayersData.PlayerChangeScene(Client.Id, SceneName);

                bool CanUnloadOldScene = ServerInstance.m_ScenesData.IsNoBodyOnThisScene(OldScene, ServerInstance);

                Logger.Log($"{Data.m_PlayerName} transitions from {OldScene} to {SceneName} Should unload old scene? {CanUnloadOldScene}");

                if (ServerInstance.m_Rules.m_CanUseTransitions && CanUnloadOldScene && ServerInstance.m_ScenesData.CanUnloadScene())
                {
                    ServerInstance.m_ScenesData.UnloadScene(ServerInstance, SceneName);
                }
            }
            else
            {
                return;
            }

            if(SceneName == "" || SceneName == "Empty" || SceneName.StartsWith("Menu"))
            {
                ServerInstance.m_PlayersData.SetGameplayState(Client.Id, PlayerData.GamePlayState.Unassigned);
                return;
            }

            if (ServerInstance.m_Rules.m_CanUseTransitions)
            {
                if (!ServerInstance.m_ScenesData.m_LoadedScenes.ContainsKey(SceneName))
                {
                    Logger.Log($"Loading scene {SceneName} for {Data.m_PlayerName}");
                    ServerInstance.m_ScenesData.LoadScene(SceneName);
                }
            }

            ServerInstance.m_ScenesData.SendAllGears(SceneName, Client);
            ServerInstance.m_ScenesData.SendAllOpenables(SceneName, Client);
            ServerInstance.m_ScenesData.SendZone(SceneName, Client);
            ServerInstance.m_ScenesData.SendAllDeathContainers(SceneName, Client);
            ServerInstance.m_ScenesData.SendAllContainerStates(SceneName, Client);
            ServerInstance.m_ScenesData.SendAllProps(SceneName, Client);
            ServerInstance.m_PlayersData.SendAllPlayersOnScene(Client, SceneName);
            ServerInstance.m_ScenesData.SendAllHarvested(SceneName, Client);
            ServerInstance.m_ScenesData.SendAllBreakDown(SceneName, Client);

            SceneData SceneData = ServerInstance.m_ScenesData.GetSceneData(SceneName);

            if(SceneData != null)
            {
                ServerSend.SendSpawnersMarkers(Client, SceneData.GetGearSpawnersMarkers());
            }

            List<NetPeer> peers = new List<NetPeer>();
            ServerInstance.m_Instance.GetConnectedPeers(peers);

            ServerSend.SendTier(Client, Data.m_Tier);

            if (ServerInstance.m_Rules != null)
            {
                if (ServerInstance.m_Rules.m_HUDMode == "DM")
                {
                    ServerSend.SendHUDSideBar(Client, 0, "ico_Reload", "GAMEPLAY_SideBarKills", Data.m_Kills.ToString(), ServerInstance);
                    ServerSend.SendHUDSideBar(Client, 1, "icoMap_grave", "GAMEPLAY_SideBarDeaths", Data.m_Deaths.ToString(), ServerInstance);
                    ServerSend.SendHUDSideBar(Client, 2, "ico_Status_BuffPlus", "GAMEPLAY_SideBarAssists", Data.m_Assists.ToString(), ServerInstance);
                    ServerSend.SendHUDSideBar(Client, 3, "", "GAMEPLAY_SideBarScore", ServerInstance.m_PlayersData.GetPlayerScoreString(Client.Id), ServerInstance);
                    
                    ServerSend.SendTimerPrefix(Client, "GAMEPLAY_TimeRemaining");
                }
                else if (ServerInstance.m_Rules.m_HUDMode == "Shrink")
                {
                    ServerSend.SendHUDSideBar(Client, 0, "ico_Reload", "GAMEPLAY_SideBarKills", Data.m_Kills.ToString(), ServerInstance);
                    ServerSend.SendHUDSideBar(Client, 1, "ico_knowledge_people", "GAMEPLAY_SideBarPlayersAlive", ServerInstance.m_PlayersData.GetShrinkModeString(), ServerInstance);
                    ServerSend.SendHUDSideBarClear(Client, 2, ServerInstance);
                    ServerSend.SendHUDSideBarClear(Client, 3, ServerInstance);
                }
                else if (ServerInstance.m_Rules.m_HUDMode == "GunGame")
                {
                    ServerSend.SendHUDSideBar(Client, 0, "ico_Reload", "GAMEPLAY_SideBarWeaponTier", Data.GetTierString(ServerInstance), ServerInstance);
                    ServerSend.SendHUDSideBar(Client, 1, "ico_xpModeInterloper", "GAMEPLAY_SideBarKillsRequired", Data.GetTierProgressString(ServerInstance), ServerInstance);
                    ServerSend.SendHUDSideBar(Client, 2, "", "GAMEPLAY_SideBarScore", ServerInstance.m_PlayersData.GetPlayerScoreString(Client.Id), ServerInstance);
                    ServerSend.SendHUDSideBarClear(Client, 3, ServerInstance);

                    ServerSend.SendTimerPrefix(Client, "GAMEPLAY_TimeRemaining");
                }
                else if(ServerInstance.m_Rules.m_HUDMode == "Lobby")
                {
                    ServerSend.SendHUDSideBar(Client, 0, "", "GAMEPLAY_SideNextGameMode", ServerInstance.GetNextGameModeName(), ServerInstance);
                    ServerSend.SendHUDSideBar(Client, 1, "", "GAMEPLAY_SideNextMap", ServerInstance.GetNextMapName(), ServerInstance);
                    ServerSend.SendHUDSideBar(Client, 2, "", "GAMEPLAY_SideBarPlayersAlive", ServerInstance.m_PlayersData.GetPlayersString(), ServerInstance);
                    ServerSend.SendHUDSideBarClear(Client, 3, ServerInstance);
                }
                else
                {
                    ServerSend.SendHUDSideBarClear(Client, 0, ServerInstance);
                    ServerSend.SendHUDSideBarClear(Client, 1, ServerInstance);
                    ServerSend.SendHUDSideBarClear(Client, 2, ServerInstance);
                    ServerSend.SendHUDSideBarClear(Client, 3, ServerInstance);
                    ServerSend.SendTimerPrefix(Client, "");
                    ServerSend.ClientGameModeTimer(Client, 0);
                }
            }

            foreach (NetPeer Peer in peers.ToArray())
            {
                if (Peer.Id != Client.Id)
                {
                    DataStr.PlayerData PeerData = ServerInstance.GetPlayerDataByNetPeer(Peer);

                    ServerSend.SendPlayerSceneNotification(Client, PeerData.m_Scene == SceneName, Peer.Id);
                    if (PeerData.m_Scene == SceneName)
                    {
                        ServerSend.SendPlayerAction(Client, PeerData.m_VisualData.m_LastAction, Peer.Id);
                        ServerSend.SendPlayerCrouch(Client, PeerData.m_VisualData.m_Crouch, Peer.Id);
                        ServerSend.SendClothing(Client, PeerData.m_VisualData.m_ClothingData, Peer.Id);
                        ServerSend.SendPosition(Client, PeerData.m_Position, Peer.Id);
                        ServerSend.SendRotation(Client, PeerData.m_Rotation, Peer.Id);
                        ServerSend.SendPlayerChangeGear(Client, PeerData.m_VisualData.m_GearInHands, PeerData.m_VisualData.m_GearVariant, Peer.Id);
                    }
                }
            }

            if (ServerInstance.m_Rules != null && ServerInstance.m_Rules.m_Time > 0)
            {
                ServerSend.ClientGameModeTimer(ServerInstance.m_Rules.m_Time, ServerInstance);
            }

            DataStr.V3Quat Point = ServerInstance.m_ScenesData.GetSpawnPoint(SceneName, Client.Id);
            ServerInstance.m_PlayersData.PlayerMoved(Client.Id, Point.m_Position);
            ServerInstance.m_PlayersData.PlayerRotated(Client.Id, Point.m_Rotation);

            if (ServerInstance.m_Rules.m_HUDMode == "Shrink")
            {
                if(SceneData != null && SceneData.m_ActiveZone != null)
                {
                    if(SceneData.m_ActiveZone.m_CurrentStageIndex >= 1)
                    {
                        ServerInstance.m_PlayersData.SetGameplayState(Client.Id, PlayerData.GamePlayState.Spectator);
                        ServerSend.SendPlayerRespawn(Client, Point.m_Position, Point.m_Rotation, false);
                        ServerSend.SendPlayerBecomeSpectator(Client);
                    }
                    else
                    {
                        ServerSend.SendPlayerRespawn(Client, Point.m_Position, Point.m_Rotation, false);
                    }
                }
            }
            else
            {
                ServerSend.SendPlayerRespawn(Client, Point.m_Position, Point.m_Rotation, false);
            }
        }

        public static void ClientOpenableInteraction(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GUID = Reader.GetString();
            bool OpenState = Reader.GetBool();

            List<NetPeer> peers = new List<NetPeer>();
            ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers.ToArray())
            {
                if (Peer.Id != Client.Id)
                {
                    if (ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene)
                    {
                        ServerSend.SendOpenableState(Peer, GUID, OpenState);
                    }
                }
            }
            ServerInstance.m_ScenesData.AddOpenableState(ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene, GUID, OpenState);
        }

        public static void ClientClothing(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            if (ServerInstance.GetPlayerDataByNetPeer(Client).m_GamePlayState != PlayerData.GamePlayState.Alive)
            {
                return;
            }
            DataStr.ClothingData ClothingData = Reader.GetClothingData();
            PlayerData Data = ServerInstance.GetPlayerDataByNetPeer(Client);
            Data.m_VisualData.m_ClothingData = ClothingData;

            List<NetPeer> peers = new List<NetPeer>();
            ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers.ToArray())
            {
                if (Peer.Id != Client.Id || ServerInstance.m_PlayersData.m_RecursiveDebug)
                {
                    if(ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene)
                    {
                        ServerSend.SendClothing(Peer, ClothingData, Client.Id);
                    }
                }
            }
        }
        public static void ClientTryInteract(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);
            if (Player.m_GamePlayState != PlayerData.GamePlayState.Alive)
            {
                ServerSend.SendInteractResult(Client, false);
                return;
            }
            string GUID = Reader.GetString();
            bool BindItNow = Reader.GetBool();

            List<NetPeer> peers = new List<NetPeer>();
            ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers.ToArray())
            {
                PlayerData Data = ServerInstance.GetPlayerDataByNetPeer(Peer);
                if (Peer.Id != Client.Id)
                {
                    if(Data.m_CarSeat == GUID || Data.m_InteractionGUID == GUID)
                    {
                        ServerSend.SendInteractResult(Client, false);
                        ServerInstance.m_PlayersData.SetPlayerInteractionGUID(Player, "");
                        return;
                    }
                }
            }

            if (BindItNow)
            {
                ServerInstance.m_PlayersData.SetPlayerInteractionGUID(Player, GUID);
            }

            //ServerInstance.m_ScenesData.UseProp(Player.m_Scene, GUID, true);
            ServerSend.SendInteractResult(Client, true);
        }
        public static void ClientVehicleSeat(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GUID = Reader.GetString();
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (!string.IsNullOrEmpty(GUID))
            {
                List<NetPeer> peers = new List<NetPeer>();
                ServerInstance.m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Peer in peers.ToArray())
                {
                    PlayerData OtherData = ServerInstance.GetPlayerDataByNetPeer(Peer);

                    if (Peer.Id != Client.Id)
                    {
                        if (OtherData.m_CarSeat == GUID)
                        {
                            return;
                        }
                        if (OtherData.m_InteractionGUID == GUID)
                        {
                            return;
                        }
                    }
                }
            }
            ServerInstance.m_PlayersData.SetPlayerCarSeatGUID(Player, GUID);
            ServerInstance.m_PlayersData.SetPlayerInteractionGUID(Player, "");
        }
        public static void ClientInVehicle(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            bool IsInVehicle = Reader.GetBool();
            ServerInstance.m_PlayersData.PlayerChangeVehicleState(Client.Id, IsInVehicle);
        }

        public static void ClientDeathPackAdded(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            DataStr.DeathPack Pack = Reader.GetDeathPack();
            string JSONCompressed = Reader.GetString();
            ServerInstance.m_ScenesData.AddDeathPack(Pack, ServerInstance.m_PlayersData.GetPlayer(Client.Id).m_Scene);
            ServerInstance.m_ScenesData.AddContainer(Pack.m_GUID, JSONCompressed, ServerInstance.m_PlayersData.GetPlayer(Client.Id).m_Scene);

            List<NetPeer> peers = new List<NetPeer>();
            ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers.ToArray())
            {
                if (Peer.Id != Client.Id)
                {
                    if (ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene)
                    {
                        ServerSend.SendDeathPack(Peer, Pack, ServerInstance);
                    }
                }
            }
        }

        public static void ClientDeathPackRemoved(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GUID = Reader.GetString();
            ServerInstance.m_ScenesData.RemoveDeathPack(GUID, ServerInstance.m_PlayersData.GetPlayer(Client.Id).m_Scene);
            ServerInstance.m_ScenesData.RemoveContainer(GUID, ServerInstance.m_PlayersData.GetPlayer(Client.Id).m_Scene);

            List<NetPeer> peers = new List<NetPeer>();
            ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers.ToArray())
            {
                if (ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == ServerInstance.GetPlayerDataByNetPeer(Client).m_Scene)
                {
                    ServerSend.SendDeathPackRemoved(Peer, GUID, ServerInstance);
                }
            }
        }

        public static void ClientContainerOpen(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GUID = Reader.GetString();

            string JSON = ServerInstance.m_ScenesData.GetContainerContent(GUID, ServerInstance.m_PlayersData.GetPlayer(Client.Id).m_Scene);
            ServerSend.SendContainerData(Client, JSON, ServerInstance);
        }

        public static void ClientUpdateContainerData(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GUID = Reader.GetString();
            string JSONCompressed = Reader.GetString();
            ServerInstance.m_ScenesData.AddContainer(GUID, JSONCompressed, ServerInstance.m_PlayersData.GetPlayer(Client.Id).m_Scene);
            ServerSend.SendContainerDataArrived(Client, ServerInstance);
        }

        public static void ClientFinishInteract(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            ServerInstance.m_PlayersData.SetPlayerInteractionGUID(Client.Id, "");
        }

        public static void ClientSetInteraction(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GUID = Reader.GetString();
            ServerInstance.m_PlayersData.SetPlayerInteractionGUID(Client.Id, GUID);
        }

        public static void ClientContainerStateUpdated(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GUID = Reader.GetString();
            int State = Reader.GetInt();
            string Scene = ServerInstance.m_PlayersData.GetPlayer(Client.Id).m_Scene;
            ServerInstance.m_ScenesData.SetContainerState(GUID, State, Scene);

            List<NetPeer> peers = new List<NetPeer>();
            ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers.ToArray())
            {
                if (ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == Scene)
                {
                    ServerSend.SendContainerState(Peer, GUID, State, ServerInstance);
                }
            }
        }

        public static void ClientCardGameAction(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string GUID = Reader.GetString();
            int State = Reader.GetInt();
            int GamePlayerID = Reader.GetInt(); // NOT A CLINET ID!!!

            if(State == 0)
            {
                CardGamesManager.TryJoinGame(Client, GUID, Client.Id, GamePlayerID, ServerInstance);
            }else if(State == 1)
            {
                CardGamesManager.TryDoAction(GUID, GamePlayerID, "fold");
            }
            else if (State == 2)
            {
                CardGamesManager.TryDoAction(GUID, GamePlayerID, "check");
            }
            else if (State == 3)
            {
                CardGamesManager.TryDoAction(GUID, GamePlayerID, "call");
            }
            else if (State == 4)
            {
                int Raised = Reader.GetInt();
                CardGamesManager.TryDoAction(GUID, GamePlayerID, "raise", Raised);
            }
        }

        public static void ClientFishTalk(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string Scene = ServerInstance.m_PlayersData.GetPlayer(Client.Id).m_Scene;

            List<NetPeer> peers = new List<NetPeer>();
            ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers.ToArray())
            {
                if (ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == Scene)
                {
                    if(Peer.Id != Client.Id || ServerInstance.m_PlayersData.m_RecursiveDebug)
                    {
                        ServerSend.SendFishTalk(Peer, Client.Id);
                    }
                }
            }
        }

        public static void ClientGetTier(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);
            if (Player != null)
            {
                ServerSend.SendTier(Client, Player.m_Tier);
            }
        }

        public static void ProcessCMD(Server ServerInstance, string CMD, PlayerData Player = null)
        {
            if (Player != null)
            {
                Logger.Log(ConsoleColor.Cyan, $"Player {Player.m_PlayerName} (ID {Player.m_PlayerID}) sent CMD {CMD}");
            }

            switch (CMD)
            {
                case "recurs":
                case "recursive":
                case "mimic":
                    ServerInstance.m_PlayersData.m_RecursiveDebug = !ServerInstance.m_PlayersData.m_RecursiveDebug;
                    Logger.Log(ConsoleColor.Green, $"New m_RecursiveDebug flag is {ServerInstance.m_PlayersData.m_RecursiveDebug}");
                    List<NetPeer> peers = new List<NetPeer>();
                    ServerInstance.m_Instance.GetConnectedPeers(peers);
                    foreach (NetPeer Peer in peers.ToArray())
                    {
                        ServerSend.SendPlayerSceneNotification(Peer, true, Peer.Id);
                    }
                    break;
                case "addtier":
                    if (Player != null)
                    {
                        Player.AddTier(ServerInstance);
                    }
                    break;
                case "removetier":
                    if (Player != null)
                    {
                        Player.RemoveTier(ServerInstance);
                    }
                    break;
                case "addkill":
                case "addscore":
                    if (Player != null)
                    {
                        Player.AddKill(ServerInstance);
                    }
                    break;
                case "removekill":
                case "removescore":
                    if (Player != null)
                    {
                        Player.RemoveKill(ServerInstance);
                    }
                    break;
                case "adddeath":
                    if (Player != null)
                    {
                        Player.AddDeath(ServerInstance);
                    }
                    break;
                case "addassist":
                    if (Player != null)
                    {
                        Player.AddAssist(ServerInstance);
                    }
                    break;
                case "nextmap":
                    ServerInstance.ForceToOver();
                    break;
                case "squad":
                    if (Player != null)
                    {
                        ServerInstance.m_PlayersData.CreateRandomSquadForPlayer(Player.m_PlayerID);
                    }
                    break;
                case "join":
                    if (Player != null)
                    {
                        ServerInstance.m_PlayersData.JoinRandomSquad(Player.m_PlayerID);
                    }
                    break;
                case "pvp":
                    ServerInstance.m_Rules.m_PVP = !ServerInstance.m_Rules.m_PVP;
                    ServerSend.SendConfigUpdated(ServerInstance);
                    Logger.Log(ConsoleColor.Green, $"New m_Rules.m_PVP flag is {ServerInstance.m_Rules.m_PVP}");
                    break;
                case "gungame":
                    if(ServerInstance.m_Rules.m_HUDMode != "Lobby")
                    {
                        ServerInstance.ForceToOver();
                    }
                    ServerInstance.SetNextGameMode("GunGame");
                    break;
                case "dm":
                    if (ServerInstance.m_Rules.m_HUDMode != "Lobby")
                    {
                        ServerInstance.ForceToOver();
                    }
                    ServerInstance.SetNextGameMode("DM");
                    break;
                case "shrink":
                    if (ServerInstance.m_Rules.m_HUDMode != "Lobby")
                    {
                        ServerInstance.ForceToOver();
                    }
                    ServerInstance.SetNextGameMode("Shrink");
                    break;
                case "nextzone":
                    ServerInstance.ForceNextZone();
                    break;
                case "skipzone":
                    ServerInstance.ForceNextZone(true);
                    break;
                case "zonenodamage":
                    ServerInstance.ForceZoneNoDamage();
                    break;
                case "zonerestart":
                    ServerInstance.ZoneRestart();
                    break;
                case "lobby":
                    ServerInstance.ForceToOver();
                    ServerInstance.SetNextGameMode("Lobby");
                    break;
                case "newpoint":
                    NetPeer Client = ServerInstance.GetClient(Player.m_PlayerID);
                    if (Client != null)
                    {
                        DataStr.V3Quat Point = ServerInstance.m_ScenesData.GetSpawnPoint(Player.m_Scene, Player.m_PlayerID);
                        ServerInstance.m_PlayersData.PlayerMoved(Client.Id, Point.m_Position);
                        ServerInstance.m_PlayersData.PlayerRotated(Client.Id, Point.m_Rotation);
                        ServerSend.SendPlayerRespawn(Client, Point.m_Position, Point.m_Rotation);
                    }
                    break;
                case "start":
                    if(ServerInstance.m_Rules.m_HUDMode == "Lobby")
                    {
                        ServerInstance.ForceToOver();
                    }
                    break;
                case "cheats":
                case "cheat":
                case "console":
                    ServerInstance.m_Config.m_CheatsAllowed = !ServerInstance.m_Config.m_CheatsAllowed;
                    ServerSend.SendConfigUpdated(ServerInstance);
                    Logger.Log(ConsoleColor.Green, $"New m_Config.m_CheatsAllowed flag is {ServerInstance.m_Config.m_CheatsAllowed}");
                    break;
                case "timepause":
                case "stoptimer":
                case "timer":
                    ServerInstance.m_TimePaused = !ServerInstance.m_TimePaused;
                    Logger.Log(ConsoleColor.Green, $"New ServerInstance.m_TimePaused flag is {ServerInstance.m_TimePaused}");
                    break;
                case "spectate":
                case "spectator":
                case "spectators":
                    NetPeer SpecPlayer = ServerInstance.GetClient(Player.m_PlayerID);
                    if (SpecPlayer != null)
                    {
                        if (Player != null)
                        {
                            if(Player.m_GamePlayState != PlayerData.GamePlayState.Spectator)
                            {
                                ServerInstance.m_PlayersData.SetGameplayState(Player.m_PlayerID, PlayerData.GamePlayState.Spectator);
                                ServerSend.SendPlayerBecomeSpectator(SpecPlayer);
                            }
                            else
                            {
                                ServerInstance.m_PlayersData.SetGameplayState(Player.m_PlayerID, PlayerData.GamePlayState.Alive);
                                DataStr.V3Quat Point = ServerInstance.m_ScenesData.GetSpawnPoint(Player.m_Scene, Player.m_PlayerID);
                                ServerInstance.m_PlayersData.PlayerMoved(SpecPlayer.Id, Point.m_Position);
                                ServerInstance.m_PlayersData.PlayerRotated(SpecPlayer.Id, Point.m_Rotation);
                                ServerSend.SendPlayerRespawn(SpecPlayer, Point.m_Position, Point.m_Rotation);
                            }
                        }
                    }
                    break;
                case "skip":
                case "skiptime":
                    ServerInstance.m_Timeline.SkipHours(1f);
                    break;
                case "rtsleep":
                    ServerInstance.m_Timeline.m_RTSleepOnly = !ServerInstance.m_Timeline.m_RTSleepOnly;
                    break;
                case "nextweather":
                case "next_weather":
                    if(ServerInstance.m_Weather != null && ServerInstance.m_Weather.m_Config != null)
                    {
                        ServerInstance.m_Weather.ForceNextWeather();
                    }
                    break;
                default:
                    Logger.Log(ConsoleColor.Yellow, $"Unknown CMD {CMD}");
                    break;
            }
        }

        public static void ClientSV_CMD(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);
            if (Player != null && ServerInstance.m_PlayersData.PlayerCanBeTrusted(Player))
            {
                ProcessCMD(ServerInstance, Reader.GetString(), Player);
            }
        }

        public static void ClientSquadHealth(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);
            float Health = Reader.GetFloat();
            bool Debuffs = Reader.GetBool();
            bool KnockedDown = Reader.GetBool();
            if (Player != null)
            {
                PlayersSquad Squad = ServerInstance.m_PlayersData.GetSquadPlayerIn(Player.m_PlayerID);
                if (Squad != null)
                {
                    foreach (int TeammateID in Squad.m_Players)
                    {
                        if(TeammateID != Player.m_PlayerID || ServerInstance.m_PlayersData.m_RecursiveDebug)
                        {
                            NetPeer OtherClient = ServerInstance.GetClient(TeammateID);

                            if (OtherClient != null)
                            {
                                ServerSend.SendSquadMemberUpdate(OtherClient, Player.m_PlayerID, Health, Debuffs, KnockedDown);
                            }
                        }
                    }
                }
            }
        }
        public static void ClientTilt(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            float Tilt = Reader.GetFloat();
            ServerInstance.m_PlayersData.PlayerTilted(Client.Id, Tilt);
        }

        public static void ClientRequestPresent(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            bool Bool = Reader.GetBool();

            string GearName = LootTableManager.GetRandomLoot("Present");

            Logger.Log(ConsoleColor.Magenta, $"Client {Client.Id} requrest gear from present giving them {GearName}");

            if (!string.IsNullOrEmpty(GearName))
            {
                ServerSend.SendPickUpGear(Client, GearName, "", true);
            }
            else
            {
                ServerSend.SendPickUpGearFailed(Client);
            }
        }

        public static void ClientRequestNewSquad(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string SquadName = Reader.GetString();

            Logger.Log(ConsoleColor.Magenta, $"Client {Client.Id} requrest creating new squad {SquadName}");

            PlayersSquad Squad = ServerInstance.m_PlayersData.CreateSquad(ServerInstance, SquadName);

            if(Squad == null)
            {
                ServerSend.SendSquadResponce(Client, Packet.SquadResponce.CantCreateSquad);
            }
            else
            {
                ServerSend.SendSquadResponce(Client, Packet.SquadResponce.SquadCreated);
                ServerInstance.m_PlayersData.AddPlayerToSquad(SquadName, Client.Id);
            }
        }

        public static void ClientRequestLeaveSquad(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            bool Bool = Reader.GetBool();

            Logger.Log(ConsoleColor.Magenta, $"Client {Client.Id} requrest leaving squad");

            PlayersSquad Squad = ServerInstance.m_PlayersData.GetSquadPlayerIn(Client.Id);

            if (Squad == null)
            {
                ServerSend.SendSquadResponce(Client, Packet.SquadResponce.YouNotInSquad);
            }
            else
            {
                ServerSend.SendSquadResponce(Client, Packet.SquadResponce.YouLeftSquad);

                string SquadNameToDismember = Squad.m_Name;
                ServerInstance.m_PlayersData.RemovePlayerFromSquad(Squad.m_Name, Client.Id);

                if (Squad.m_Players.Count == 0)
                {
                    ServerSend.SendSquadEliminated(ServerInstance, SquadNameToDismember);
                    Logger.Log(ConsoleColor.Cyan, $"[Squads] Squad {SquadNameToDismember} was dismembered, no players left.");
                    ServerInstance.m_PlayersData.m_Squads.Remove(SquadNameToDismember);
                }
            }
        }

        public static void ClientInviteToSquad(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            int OtherClient_ID = Reader.GetInt();


            PlayersSquad Squad = ServerInstance.m_PlayersData.GetSquadPlayerIn(Client.Id);

            if (Squad == null)
            {
                ServerSend.SendSquadResponce(Client, Packet.SquadResponce.YouNotInSquad);
            }
            else
            {
                Logger.Log(ConsoleColor.Magenta, $"Client {Client.Id} trying to invite {OtherClient_ID} to squad {Squad.m_Name}");

                PlayersSquad OtherSquad = ServerInstance.m_PlayersData.GetSquadPlayerIn(OtherClient_ID);

                if(OtherSquad != null)
                {
                    ServerSend.SendSquadResponce(Client, Packet.SquadResponce.TheyAlreadyInSquad);
                }
                else
                {
                    if (ServerInstance.m_PlayersData.PlayerIsInvitedBySomeone(OtherClient_ID))
                    {
                        ServerSend.SendSquadResponce(Client, Packet.SquadResponce.YouCantInviteThemATM);
                    }
                    else
                    {
                        PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);

                        if (Player != null)
                        {
                            if (Player.m_SquadInvitesSent > 0)
                            {
                                if(Player.m_LastInviteTime.AddSeconds(5) < DateTime.UtcNow)
                                {
                                    Player.m_SquadInvitesSent = 0;
                                }
                            }
                            Player.m_LastInviteTime = DateTime.UtcNow;
                            Player.m_SquadInvitesSent++;

                            if(Player.m_SquadInvitesSent <= 3)
                            {
                                if (Squad.m_Players.Count < PlayersDataManager.c_SquadLimit)
                                {
                                    ServerInstance.m_PlayersData.InvitePlayerToSquad(Squad.m_Name, OtherClient_ID);
                                }
                                else
                                {
                                    ServerSend.SendSquadResponce(Client, Packet.SquadResponce.SquadIsFull);
                                }
                            }
                            else
                            {
                                ServerSend.SendSquadResponce(Client, Packet.SquadResponce.YouInvitedTooMuch);
                            }
                        }
                    }
                }
            }
        }

        public static void ClientAcceptInviteToSquad(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string SquadName = Reader.GetString();

            Logger.Log(ConsoleColor.Magenta, $"Client {Client.Id} trying accept invite to squad {SquadName}");

            PlayersSquad Squad = ServerInstance.m_PlayersData.GetSquad(SquadName);

            if (Squad == null)
            {
                Logger.Log(ConsoleColor.Magenta, $"Squad {SquadName} not exist");
                ServerSend.SendSquadResponce(Client, Packet.SquadResponce.SquadNotExist);
            }
            else
            {
                if (!Squad.PlayerIsInvited(Client.Id))
                {
                    ServerSend.SendSquadResponce(Client, Packet.SquadResponce.YouAreNotInvited);
                }
                else
                {
                    ServerInstance.m_PlayersData.AcceptInviteToSquad(SquadName, Client.Id);
                }
            }
        }

        public static void ClientRefuseJoinToSquad(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string SquadName = Reader.GetString();

            Logger.Log(ConsoleColor.Magenta, $"Client {Client.Id} refuse invite to squad {SquadName}");

            PlayersSquad Squad = ServerInstance.m_PlayersData.GetSquad(SquadName);

            if (Squad == null)
            {
                Logger.Log(ConsoleColor.Magenta, $"Squad {SquadName} not exist");
                ServerSend.SendSquadResponce(Client, Packet.SquadResponce.SquadNotExist);
            }
            else
            {
                if (Squad.PlayerIsInvited(Client.Id))
                {
                    Squad.RemoveInvite(Client.Id);
                }
            }
        }

        public static void ClientBloodLosses(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            int BloodLosses = Reader.GetInt();
            PlayerData PlayerData = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (PlayerData != null)
            {
                PlayerData.m_BloodLosses = BloodLosses;

                List<NetPeer> peers = new List<NetPeer>();
                ServerInstance.m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Peer in peers.ToArray())
                {
                    if (ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == PlayerData.m_Scene)
                    {
                        if (Peer.Id != Client.Id || ServerInstance.m_PlayersData.m_RecursiveDebug)
                        {
                            ServerSend.SendBloodLosses(Peer, Client.Id, BloodLosses);
                        }
                    }
                }
            }
        }

        public static void ClientReviveRequest(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            int PlayerIDToRevive = Reader.GetInt();
            PlayerData PlayerData = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (PlayerData != null)
            {

                if(PlayerData.m_GamePlayState == PlayerData.GamePlayState.Alive)
                {
                    PlayerData PlayerDataToRevive = ServerInstance.m_PlayersData.GetPlayer(PlayerIDToRevive);

                    if(PlayerDataToRevive != null)
                    {
                        if(PlayerData.m_GamePlayState == PlayerData.GamePlayState.Alive && PlayerDataToRevive.m_VisualData.m_LastAction == 6)
                        {
                            ServerSend.SendRevivedBySomeone(ServerInstance.GetClient(PlayerIDToRevive), Client.Id);
                        }
                    }
                }
            }
        }

        public static void ClientChatMessage(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            string ChatMessage = Reader.GetString();

            ServerSend.SendChatMessage(ServerInstance, ChatMessage, Client.Id);
        }

        public static void ClientStartFire(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (Player != null)
            {
                string GUID = Reader.GetString();
                float Fuel = Reader.GetFloat();
                float Heat = Reader.GetFloat();
                float InnerRadius = Reader.GetFloat();
                float OutterRadius = Reader.GetFloat();
                float HeatingSpeed = Reader.GetFloat();
                bool IsForge = Reader.GetBool();
                int CookingSlots = Reader.GetInt();
                bool IsDynamic = Reader.GetBool();

                Vector3 Position = Vector3.Zero;
                Quaternion Rotation = Quaternion.Identity;

                if (IsDynamic)
                {
                    Position = Reader.GetVector3();
                    Rotation = Reader.GetQuaternion();
                }

                FireSyncData FireData = ServerInstance.m_ScenesData.GetFire(GUID, Player.m_Scene);

                if(FireData == null)
                {
                    FireData = FireSyncData.Create(GUID, Fuel, Heat, InnerRadius, OutterRadius, HeatingSpeed, IsForge, CookingSlots, IsDynamic, ServerInstance.m_Timeline.m_ElapsedInGameHours, Player.m_Scene, ServerInstance);

                    if (IsDynamic)
                    {
                        FireData.m_Position = Position;
                        FireData.m_Rotation = Rotation;
                    }
                    ServerInstance.m_ScenesData.AddFire(FireData, Player.m_Scene);
                }
                else
                {
                    FireData.Ignite(Fuel, Heat, InnerRadius, OutterRadius, ServerInstance.m_Timeline.m_ElapsedInGameHours, Player.m_Scene, ServerInstance);
                }
            }
        }

        public static void ClientAddFuel(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (Player != null)
            {
                string GUID = Reader.GetString();
                float Fuel = Reader.GetFloat();
                float Heat = Reader.GetFloat();
                float InnerRadius = Reader.GetFloat();
                float OutterRadius = Reader.GetFloat();

                ServerInstance.m_ScenesData.AddFuel(Player.m_Scene, GUID, Fuel, Heat, InnerRadius, OutterRadius);
            }
        }

        public static void ClientTakeTorch(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (Player != null)
            {
                string GUID = Reader.GetString();
                ServerSend.SendTakeTorch(Client, ServerInstance.m_ScenesData.TakeTorch(Player.m_Scene, GUID));
            }
        }

        public static void ClientDismantleCampfire(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (Player != null)
            {
                string GUID = Reader.GetString();
                int Charcoal = ServerInstance.m_ScenesData.RemoveFire(Player.m_Scene, GUID);

                ServerSend.SendCharcoalCollected(Client, Charcoal);
            }
        }

        public static void ClientCharcoalCollect(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (Player != null)
            {
                string GUID = Reader.GetString();
                int Charcoal = ServerInstance.m_ScenesData.TakeCharcoal(Player.m_Scene, GUID);

                ServerSend.SendCharcoalCollected(Client, Charcoal);
            }
        }

        public static void ClientRequestFreeCookingSlot(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (Player != null)
            {
                string GUID = Reader.GetString();
                int DesiredSlot = Reader.GetInt();
                if (DesiredSlot == -1)
                {
                    int Slot = ServerInstance.m_ScenesData.GetFreeCookingSlot(Player.m_Scene, GUID);
                    ServerSend.SendFreeCookingSlot(Client, Slot);
                }
                else
                {
                    bool IsEmpty = ServerInstance.m_ScenesData.CookingSlotIsFree(Player.m_Scene, GUID, DesiredSlot);

                    if (!IsEmpty)
                    {
                        ServerSend.SendFreeCookingSlot(Client, -1);
                    }
                    else
                    {
                        ServerSend.SendFreeCookingSlot(Client, DesiredSlot);
                    }
                }
            }
        }
        public static void ClientGearSetRecipe(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (Player != null)
            {
                string GUID = Reader.GetString();
                string RecipeResult = Reader.GetString();
                float Volume = Reader.GetFloat();

                GearDataContainer GearData = ServerInstance.m_ScenesData.GetGear(Player.m_Scene, GUID, false);

                if(GearData != null)
                {
                    if (string.IsNullOrEmpty(GearData.m_Visual.m_CookingResult))
                    {
                        GearData.m_Visual.SetRecipe(RecipeResult, Volume, 0);
                        ServerInstance.m_ScenesData.SetGearForCooking(Player.m_Scene, GearData.m_Visual);

                        ServerSend.SendGearVisual(GearData.m_Visual, Player.m_Scene, ServerInstance);
                    }
                    else
                    {
                        if(RecipeResult == "GoodWater")
                        {
                            ServerSend.SendWaterRefund(Client, Volume, false);
                        }
                    }
                }
            }
        }

        public static void ClientHarvest(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (Player != null)
            {
                string GUID = Reader.GetString();
                float RespawnMin = Reader.GetFloat();
                float RespawnMax = Reader.GetFloat();

                ServerInstance.m_ScenesData.AddHarvested(Player.m_Scene, GUID, RespawnMin, RespawnMax);
            }
        }

        public static void ClientBreakDown(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (Player != null)
            {
                string GUID = Reader.GetString();

                ServerInstance.m_ScenesData.AddBreakDown(Player.m_Scene, GUID);
            }
        }

        public static void ClientIsWorking(NetPeer Client, NetDataReader Reader, Server ServerInstance)
        {
            PlayerData Player = ServerInstance.GetPlayerDataByNetPeer(Client);

            if (Player != null)
            {
                bool IsWorking = Reader.GetBool();

                Player.m_IsWorking = IsWorking;
            }
        }
    }
}
