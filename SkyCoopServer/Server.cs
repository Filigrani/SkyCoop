using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static SkyCoopServer.DataStr.PlayerData;

namespace SkyCoopServer
{
    public class Server : IDisposable
    {
        public int m_Port = 37855;
        public NetworkHelper m_NetworkHelper;

        public DataStr.ServerConfig m_Config = new DataStr.ServerConfig();
        public DataStr.GameRules m_Rules = new DataStr.GameRules();
        public EventBasedNetListener m_Listener;
        public NetManager m_Instance;
        public bool m_IsReady = false;
        public ServerVoice m_VoiceServer = null;
        public int m_PendingGameModeOverTimer = 0;
        public string m_NextMapName = "";
        public string m_NextGameModeName = "";
        public List<DataStr.MinimalPlayersAndGameMode> m_AvailableGameModes = new List<DataStr.MinimalPlayersAndGameMode>();
        public bool m_TimePaused = false;

        // Data Sync Instances
        public PlayersDataManager m_PlayersData;
        public ScenesDataManager m_ScenesData;
        public Timeline m_Timeline;
        public WeatherManager m_Weather;

        public delegate void LogEvent(Logger.LogData Data);
        public static event LogEvent? OnLogEvent;

        public static void OnLog(Logger.LogData Data)
        {
            OnLogEvent?.Invoke(Data);
        }

        // Time
        private DateTime s_NextSecondCall;
        private DateTime s_PreviousTickTime;
        private float s_DeltaTime = 0;


        public delegate void PacketHandler(NetPeer Client, NetDataReader Reader, Server ServerInstance);
        public static Dictionary<int, PacketHandler> s_packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)Packet.Type.Welcome, ServerHandle.Welcome },
            { (int)Packet.Type.ClientPosition, ServerHandle.ClientPosition },
            { (int)Packet.Type.ClientRotation, ServerHandle.ClientRotation },
            { (int)Packet.Type.ClientScene, ServerHandle.ClientScene },
            { (int)Packet.Type.ClientHoldigGear, ServerHandle.ClientHoldingGear },
            { (int)Packet.Type.ClientCrouch, ServerHandle.ClientCrouch },
            { (int)Packet.Type.ClientAction, ServerHandle.ClientAction },
            { (int)Packet.Type.ClientFire, ServerHandle.ClientFire },
            { (int)Packet.Type.ClientDamageOtherClient, ServerHandle.ClientDamageOtherClient },
            { (int)Packet.Type.ClientProjectile, ServerHandle.ClientProjectile },
            { (int)Packet.Type.ClientDied, ServerHandle.ClientDied },
            { (int)Packet.Type.ClientRevived, ServerHandle.ClientRevived },
            { (int)Packet.Type.ClientProjectileThrow, ServerHandle.ClientProjectileThrow },
            { (int)Packet.Type.ClientRequestRespawn, ServerHandle.ClientRequestRespawn },
            { (int)Packet.Type.ClientInjectedItem, ServerHandle.ClientInjectedItem },
            { (int)Packet.Type.ClientRemoveInjectedItem, ServerHandle.ClientRemoveInjectedItem },
            { (int)Packet.Type.ClientEraceAllInjectedItems, ServerHandle.ClientEraceAllInjectedItems },
            { (int)Packet.Type.ClientSendGear, ServerHandle.ClientSendGear },
            { (int)Packet.Type.ClientPickUpGear, ServerHandle.ClientPickUpGear },
            { (int)Packet.Type.ClientLoadedScene, ServerHandle.ClientLoadedScene },
            { (int)Packet.Type.ClientOpenableInteraction, ServerHandle.ClientOpenableInteraction },
            { (int)Packet.Type.ClientClothing, ServerHandle.ClientClothing },
            { (int)Packet.Type.ClientTryInteract, ServerHandle.ClientTryInteract },
            { (int)Packet.Type.ClientVehicleSeat, ServerHandle.ClientVehicleSeat },
            { (int)Packet.Type.ClientInVehicle, ServerHandle.ClientInVehicle },
            { (int)Packet.Type.ClientDeathPackAdded, ServerHandle.ClientDeathPackAdded },
            { (int)Packet.Type.ClientDeathPackRemoved, ServerHandle.ClientDeathPackRemoved },
            { (int)Packet.Type.ClientContainerOpen, ServerHandle.ClientContainerOpen },
            { (int)Packet.Type.ClientUpdateContainerData, ServerHandle.ClientUpdateContainerData },
            { (int)Packet.Type.ClientFinishInteract, ServerHandle.ClientFinishInteract },
            { (int)Packet.Type.ClientSetInteraction, ServerHandle.ClientSetInteraction },
            { (int)Packet.Type.ClientContainerStateUpdated, ServerHandle.ClientContainerStateUpdated },
            { (int)Packet.Type.ClientCardGameAction, ServerHandle.ClientCardGameAction },
            { (int)Packet.Type.ClientFishTalk, ServerHandle.ClientFishTalk },
            { (int)Packet.Type.ClientGetTier, ServerHandle.ClientGetTier },
            { (int)Packet.Type.ClientSV_CMD, ServerHandle.ClientSV_CMD },
            { (int)Packet.Type.ClientSquadHealth, ServerHandle.ClientSquadHealth },
            { (int)Packet.Type.ClientTilt, ServerHandle.ClientTilt },
            { (int)Packet.Type.ClientRequestPresent, ServerHandle.ClientRequestPresent },
            { (int)Packet.Type.ClientRequestNewSquad, ServerHandle.ClientRequestNewSquad },
            { (int)Packet.Type.ClientRequestLeaveSquad, ServerHandle.ClientRequestLeaveSquad },
            { (int)Packet.Type.ClientInviteToSquad, ServerHandle.ClientInviteToSquad },
            { (int)Packet.Type.ClientAcceptInviteToSquad, ServerHandle.ClientAcceptInviteToSquad },
            { (int)Packet.Type.ClientRefuseJoinToSquad, ServerHandle.ClientRefuseJoinToSquad },
            { (int)Packet.Type.ClientBloodLosses, ServerHandle.ClientBloodLosses },
            { (int)Packet.Type.ClientReviveRequest, ServerHandle.ClientReviveRequest },
            { (int)Packet.Type.ClientChatMessage, ServerHandle.ClientChatMessage },
            { (int)Packet.Type.ClientStartFire, ServerHandle.ClientStartFire },
            { (int)Packet.Type.ClientAddFuel, ServerHandle.ClientAddFuel },
            { (int)Packet.Type.ClientTakeTorch, ServerHandle.ClientTakeTorch },
            { (int)Packet.Type.ClientDismantleCampfire, ServerHandle.ClientDismantleCampfire },
            { (int)Packet.Type.ClientCharcoalCollect, ServerHandle.ClientCharcoalCollect },
            { (int)Packet.Type.ClientRequestFreeCookingSlot, ServerHandle.ClientRequestFreeCookingSlot },
            { (int)Packet.Type.ClientGearSetRecipe, ServerHandle.ClientGearSetRecipe },
            { (int)Packet.Type.ClientHarvest, ServerHandle.ClientHarvest },
            { (int)Packet.Type.ClientBreakDown, ServerHandle.ClientBreakDown },
            { (int)Packet.Type.ClientIsWorking, ServerHandle.ClientIsWorking },
        };

        public void ExecutePacketEvent(int PacketID, NetPeer Client, NetDataReader Reader)
        {
            PacketHandler Handle;
            if (s_packetHandlers.TryGetValue(PacketID, out Handle))
            {
                Handle(Client, Reader, this);
            }
        }

        public Server()
        {
            m_Listener = new EventBasedNetListener();
            m_Instance = new NetManager(m_Listener);

            //TODO: Loading Config
            m_Config = new DataStr.ServerConfig();

            // Data Sync Instances
            m_PlayersData = new PlayersDataManager(this);
            m_ScenesData = new ScenesDataManager(this);
            m_Timeline = new Timeline(this);
            m_Weather = new WeatherManager(this);

            s_NextSecondCall = DateTime.UtcNow.AddSeconds(1);
            LootTableManager.Load();
            m_AvailableGameModes = FilesManager.GetGameModesList();
        }

        public string GetGameModeForPlayersCount(string CurrentGameModeName, int PlayersCount)
        {
            DataStr.MinimalPlayersAndGameMode CurrentGameMode = null;
            foreach (DataStr.MinimalPlayersAndGameMode GameMode in m_AvailableGameModes)
            {
                if(GameMode.GameModeName == CurrentGameModeName)
                {
                    CurrentGameMode = GameMode;
                    break;
                }
            }
            if(CurrentGameMode == null)
            {
                // Я устал. Нужна сортировка DataStr.MinimalPlayersAndGameMode по MinimalPlayersCount
            }
            return null;
        }

        public List<int> GetClientsIndexs()
        {
            List<int> Indexes = new List<int>();
            if (m_Instance != null)
            {
                List<NetPeer> peers = new List<NetPeer>();
                m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Peer in peers.ToArray())
                {
                    Indexes.Add(Peer.Id);
                }
            }
            return Indexes;
        }

        public DataStr.PlayerData GetPlayerDataByNetPeer(NetPeer Peer)
        {
            if (m_Instance != null)
            {
                return m_PlayersData.GetPlayer(Peer.Id);
            }
            return null;
        }

        public DataStr.PlayerData GetPlayerDataByVoiceID(int VoiceID)
        {
            foreach (DataStr.PlayerData Player in m_PlayersData.m_Players)
            {
                if (Player != null)
                {
                    if (Player.m_VoiceChatID == VoiceID)
                    {
                        return Player;
                    }
                }
            }
            return null;
        }

        public NetPeer GetClient(int Index)
        {
            if (m_Instance != null)
            {
                List<NetPeer> peers = new List<NetPeer>();
                m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Peer in peers.ToArray())
                {
                    if(Peer.Id == Index)
                    {
                        return Peer;
                    }
                }
            }
            return null;
        }

        public void Update()
        {
            s_DeltaTime = (float)(DateTime.UtcNow - s_PreviousTickTime).TotalSeconds;

            if (m_Instance != null && m_IsReady)
            {
                m_Instance.PollEvents();
            }
            if(DateTime.UtcNow >= s_NextSecondCall)
            {
                s_NextSecondCall = DateTime.UtcNow.AddSeconds(1);
                EverySecond();
                m_Timeline.UpdateEverySecond();
                m_ScenesData.UpdateEverySecond();

                if (m_Rules.m_Weather)
                {
                    if(m_Weather.m_Config == null)
                    {
                        Logger.Log(ConsoleColor.Yellow, $"Trying to load weather profile...");
                        WeatherManager.WeatherSettingsConfig Data = FilesManager.GetWeatherConfig();
                        if (Data != null)
                        {
                            m_Weather.LoadConfig(Data);
                            Logger.Log(ConsoleColor.Green, $"Weather profile loaded!");
                        }
                        else
                        {
                            Logger.Log(ConsoleColor.Red, $"Weather profile not exist!!!!!!!!");
                        }
                    }
                    else
                    {
                        m_Weather.UpdateEverySecond();
                    }
                }
            }

            s_PreviousTickTime = DateTime.UtcNow;
        }

        public void DisconnectPlayer(string PlayerName, string Reason = "")
        {
            List<NetPeer> peers = new List<NetPeer>();
            m_Instance.GetConnectedPeers(peers);

            foreach (NetPeer Peer in peers)
            {
                DataStr.PlayerData Player = m_PlayersData.GetPlayer(Peer.Id);
                if (Player != null && Player.m_PlayerName == PlayerName)
                {
                    DisconnectPlayer(Peer.Id, Reason);
                    return;
                }
            }
            Logger.Log(ConsoleColor.Red, $"There no player with name {PlayerName}");
        }

        public void DisconnectPlayer(int PlayerID, string Reason = "")
        {
            NetPeer Peer = GetClient(PlayerID);

            if(Peer != null)
            {
                DisconnectPlayer(Peer, Reason);
                return;
            }
            Logger.Log(ConsoleColor.Red, $"There no player with ID {PlayerID}");
        }

        public void DisconnectPlayer(NetPeer Client, string Reason = "")
        {
            if (Client != null)
            {
                m_Instance.DisconnectPeer(Client, GetDisconnectMessage(Reason));
            }
        }

        public void DisconnectAllPlayers(string Reason = "", bool StopServer = false)
        {
            byte[] ReasonBytes = GetDisconnectMessage(Reason);
            m_Instance.DisconnectAll(ReasonBytes, 0, ReasonBytes.Length);

            if (StopServer)
            {
                m_Instance.Stop();
                m_IsReady = false;
                Dispose();
            }
        }

        public byte[] GetDisconnectMessage(string Reason)
        {
            if (string.IsNullOrEmpty(Reason))
            {
                Reason = "Unknown reason";
            }

            List<byte> Buffer = new List<byte>();
            Buffer.AddRange(BitConverter.GetBytes((ushort)Reason.Length));
            Buffer.AddRange(Encoding.UTF8.GetBytes(Reason));

            return Buffer.ToArray();
        }

        public string GetNextMapName()
        {
            return m_NextMapName;
        }

        public string GetNextGameModeName()
        {
            return m_NextGameModeName;
        }

        public bool CanRespawn()
        {
            return m_Rules != null && m_Rules.m_Respawns != 0;
        }

        public void ForceToOver()
        {
            m_Rules.m_Time = 1;
        }

        public void ForceNextZone(bool withtime = false)
        {
            m_ScenesData.ForceNextZone(withtime);
        }

        public void ForceZoneNoDamage()
        {
            m_ScenesData.ForceZoneNoDamage();
        }

        public void ZoneRestart()
        {
            m_ScenesData.ZoneRestart();
        }

        public void OnPlayersCountChanged()
        {
            List<NetPeer> peers = new List<NetPeer>();
            m_Instance.GetConnectedPeers(peers);

            if (m_Rules.m_HUDMode == "Lobby")
            {
                if (peers.Count > 1)
                {
                    if (m_Rules.m_Time == 0)
                    {
                        m_Rules.m_Time = 180;
                        ServerSend.SendTimerPrefix("GAMEPLAY_GameStartsIn", this);
                        ServerSend.ClientGameModeTimer(m_Rules.m_Time, this);
                    }
                }
                else
                {
                    m_Rules.m_Time = 0;
                    ServerSend.SendTimerPrefix("GAMEPLAY_NeedMorePlayers", this);
                    ServerSend.ClientGameModeTimer(0, this);
                }
                foreach (NetPeer Client in peers)
                {
                    ServerSend.SendHUDSideBarUpdate(Client, 2, m_PlayersData.GetPlayersString(), this);
                }
            }
            else if(m_Rules.m_HUDMode == "Shrink")
            {
                foreach (NetPeer Client in peers)
                {
                    ServerSend.SendHUDSideBarUpdate(Client, 1, m_PlayersData.GetShrinkModeString(), this);
                }
            }
        }

        public void EverySecond()
        {
            //SkyCoopServer.Logger.Log("EverySecond");
            if(m_Rules != null)
            {
                if(m_Rules.m_Time > 0)
                {
                    if(!m_TimePaused)
                    {
                        m_Rules.m_Time = m_Rules.m_Time - 1;
                    }
                    
                    ServerSend.ClientGameModeTimer(m_Rules.m_Time, this);
                    if (m_Rules.m_Time == 0)
                    {

                        List<NetPeer> peers = new List<NetPeer>();
                        m_Instance.GetConnectedPeers(peers);

                        if (m_Rules.m_HUDMode == "Lobby")
                        {
                            m_PendingGameModeOverTimer = 3;
                            foreach (NetPeer Peer in peers.ToArray())
                            {
                                ServerSend.SendFreeze(Peer);

                                DataStr.PlayerData PlayerData = m_PlayersData.GetPlayer(Peer.Id);
                                string PlayerScene = PlayerData.m_Scene;

                                PlayerData.SetGameplayState(GamePlayState.Unassigned, this);

                                PlayerData.m_IsWorking = false;
                                m_PlayersData.PlayerChangeScene(Peer.Id, "");
                                m_PlayersData.SetPlayerCarSeatGUID(Peer.Id, "");
                                m_PlayersData.SetPlayerInteractionGUID(Peer.Id, "");
                            }
                        }
                        else
                        {
                            m_PendingGameModeOverTimer = 25;
                            List<DataStr.LeaderData> Leaders = m_PlayersData.GetLeaders();
                            string SquadName = "";

                            if (m_Rules.m_HUDMode == "Shrink")
                            {
                                if (Leaders.Count > 0)
                                {
                                    DataStr.PlayersSquad Squad = m_PlayersData.GetSquadPlayerIn(Leaders[0].m_ID);

                                    if (Squad != null)
                                    {
                                        SquadName = Squad.m_Name;
                                    }
                                }
                            }

                            foreach (NetPeer Peer in peers.ToArray())
                            {
                                ServerSend.SendFreeze(Peer);
                                DataStr.PlayerData PlayerData = m_PlayersData.GetPlayer(Peer.Id);
                                string PlayerScene = PlayerData.m_Scene;

                                PlayerData.SetGameplayState(GamePlayState.Unassigned, this);
                                DataStr.SceneData SceneData = m_ScenesData.GetSceneData(PlayerScene);

                                if (SceneData != null && SceneData.m_VictoryPoint != null)
                                {
                                    ServerSend.SendLeaders(Peer, Leaders, SceneData.m_VictoryPoint.m_Position, SceneData.m_VictoryPoint.m_Rotation, SquadName, this);
                                }
                                PlayerData.m_IsWorking = false;
                                m_PlayersData.PlayerChangeScene(Peer.Id, "");
                                m_PlayersData.SetPlayerCarSeatGUID(Peer.Id, "");
                                m_PlayersData.SetPlayerInteractionGUID(Peer.Id, "");
                            }
                        }
                        m_ScenesData.UnloadSceneNobodyOn(this);
                    }
                }
            }
            if(m_PendingGameModeOverTimer > 0)
            {
                m_PendingGameModeOverTimer--;
                if(m_PendingGameModeOverTimer == 0)
                {
                    if(m_Rules.m_HUDMode == "Lobby")
                    {
                        ChangeGameMode(m_NextGameModeName, m_NextMapName);
                    }
                    else
                    {
                        ChangeGameMode("Lobby");
                    }
                }
            }
        }

        public string GetRandomMap(string GameMode, string CurrentMap = "")
        {
            DataStr.GameRules GameModeRules = FilesManager.GetRules(GameMode);

            if(GameModeRules == null)
            {
                return "";
            }

            return GameModeRules.GetRandomMap(CurrentMap);
        }

        public string GetRandomMap(DataStr.GameRules Rules, string CurrentMap = "")
        {
            if (Rules == null)
            {
                return "";
            }

            return Rules.GetRandomMap(CurrentMap);
        }

        public void SetNextMap(string MapName)
        {
            m_NextMapName = MapName;
            SkyCoopServer.Logger.Log($"SetNextMap {MapName}");

            if (m_Rules.m_HUDMode == "Lobby")
            {
                List<NetPeer> peers = new List<NetPeer>();
                m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Client in peers)
                {
                    ServerSend.SendHUDSideBar(Client, 1, "", "GAMEPLAY_SideNextMap", GetNextMapName(), this);
                }
            }
        }

        public void SetNextGameMode(string GameModeName)
        {
            m_NextGameModeName = GameModeName;
            SkyCoopServer.Logger.Log($"SetNextGameMode {GameModeName}");

            if (m_Rules.m_HUDMode == "Lobby")
            {
                List<NetPeer> peers = new List<NetPeer>();
                m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Client in peers)
                {
                    ServerSend.SendHUDSideBar(Client, 0, "", "GAMEPLAY_SideNextGameMode", GetNextGameModeName(), this);
                }

                if(m_NextGameModeName == "")
                {
                    SetNextMap("");
                }
                else
                {
                    SetNextMap(GetRandomMap(m_NextGameModeName));
                }
            }
        }

        public void ChangeGameMode(string GameMode, string NewMap = "")
        {
            SkyCoopServer.Logger.Log($"ChangeGameMode {GameMode}");
            m_Config.m_GameMode = GameMode;

            // Грузим по новой даже если режим тот же, файл режима мог быть отредактирован.
            m_Rules = FilesManager.GetRules(GameMode);

            string MapName = NewMap;

            if (string.IsNullOrEmpty(MapName))
            {
                MapName = GetRandomMap(m_Rules);
            }

            if (!string.IsNullOrEmpty(MapName))
            {
                DataStr.MapData MapData = FilesManager.GetMapData(MapName);

                if (MapData != null)
                {
                    m_Config.m_SceneToSpawn = MapData.Scene;
                    m_PlayersData.ResetGameScores();
                    m_ScenesData.UnloadSceneNobodyOn(this);
                    m_ScenesData.LoadScene(MapData);

                    ServerSend.SendConfigUpdated(this);
                    ServerSend.SendChangeMap(this);
                }
                else
                {
                    Logger.Log(ConsoleColor.Red, "Server can't load map for the server!!!!!!!!!!!!!");
                }
            }

            if (GameMode == "Lobby")
            {
                List<NetPeer> peers = new List<NetPeer>();
                m_Instance.GetConnectedPeers(peers);

                OnPlayersCountChanged();
                SetNextGameMode("Shrink");
            }
        }

        public void StartServer()
        {
            StartServer(m_Port, m_Config.m_MaxPlayers);

            ChangeGameMode(m_Config.m_GameMode);

            m_NetworkHelper = new NetworkHelper(m_Port, "SkyCoopServer");
        }

        public void StartServer(int port, int maxPlayers, string key = Packet.c_Key)
        {
            m_PlayersData.InitilizePlayers(maxPlayers);
            Logger.Log(ConsoleColor.Green, "[Server] Starting server");
            m_Instance.Start(port);

            m_Listener.ConnectionRequestEvent += request =>
            {
                if (m_Instance.ConnectedPeersCount < maxPlayers)
                    request.AcceptIfKey(key);
                else
                    request.Reject();
            };

            m_Listener.PeerConnectedEvent += peer =>
            {
                Logger.Log(ConsoleColor.Green, $"[Server] We got connection: {peer} assigned them as {peer.Id}");
                ServerSend.Welcome(peer, peer.Id);
            };

            m_Listener.PeerDisconnectedEvent += (peer, message) =>
            {
                Logger.Log(ConsoleColor.Red, $"[GameServer] Client {peer.Id} disconnected {message.Reason.ToString()}");

                m_PlayersData.OnPlayerDisconnect(peer.Id);
            };

            m_Listener.NetworkLatencyUpdateEvent += (peer, ping) =>
            {
                //Logger.Log(ConsoleColor.Gray, $"[Server] Ping to Client {peer.Id}: {ping}");
            };

            m_Listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                int PacketID = dataReader.GetInt();

                ExecutePacketEvent(PacketID, fromPeer, dataReader);

                dataReader.Recycle();
            };

            m_IsReady = true;
            Logger.Log(ConsoleColor.Green,$"[Server] Server is started port={port}");

            if(m_Config.m_VoicePort != 0)
            {
                Task.Run(StartServerVoice);
            }
        }

        public void StartServerVoice()
        {
            m_VoiceServer = new ServerVoice(this);
            m_VoiceServer.m_Port = m_Config.m_VoicePort;
            m_VoiceServer.StartServer();
        }

        public bool SetVoiceIDForPlayer(int ClientID, int VoiceID)
        {
            NetPeer Client = GetClient(ClientID);

            if (Client != null)
            {
                DataStr.PlayerData Player = GetPlayerDataByNetPeer(Client);
                if(Player != null)
                {
                    Player.m_VoiceChatID = VoiceID;
                    return true;
                }
            }
            return false;
        }

        public void ClearVoiceID(int ClientID)
        {
            foreach (DataStr.PlayerData Player in m_PlayersData.m_Players)
            {
                if(Player != null)
                {
                    if(Player.m_VoiceChatID == ClientID)
                    {
                        Player.m_VoiceChatID = -1;
                    }
                }
            }
        }

        public void Dispose()
        {
            Logger.Log(ConsoleColor.Red, "[Server] Stopping Server");

            m_IsReady = false;
            if (m_VoiceServer != null)
                m_VoiceServer.Dispose();

            if(m_NetworkHelper != null)
            {
                m_NetworkHelper.Dispose();
            }
            m_Instance.Stop();
            GC.Collect();
        }
    }
}
