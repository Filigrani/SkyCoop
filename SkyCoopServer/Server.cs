using LiteNetLib;
using LiteNetLib.Utils;
using System;

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

        // Data Sync Instances
        public PlayersDataManager m_PlayersData;
        public ScenesDataManager m_ScenesData;

        private DateTime s_NextSecondCall;


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

            s_NextSecondCall = DateTime.Now.AddSeconds(1);
            LootTableManager.Load();
        }

        public List<int> GetClientsIndexs()
        {
            List<int> Indexes = new List<int>();
            if (m_Instance != null)
            {
                foreach (NetPeer Peer in m_Instance.ConnectedPeerList.ToArray())
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

        public NetPeer GetClient(int Index)
        {
            if (m_Instance != null)
            {
                foreach (NetPeer Peer in m_Instance.ConnectedPeerList.ToArray())
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
            if (m_Instance != null && m_IsReady)
            {
                m_Instance.PollEvents();
            }
            if(DateTime.Now >= s_NextSecondCall)
            {
                s_NextSecondCall = DateTime.Now.AddSeconds(1);
                EverySecond();
            }
        }

        public bool CanRespawn()
        {
            return m_Rules != null && m_Rules.m_Respawns;
        }

        public void ForceToOver()
        {
            m_Rules.m_Time = 1;
        }

        public void EverySecond()
        {
            //SkyCoopServer.Logger.Log("EverySecond");
            if(m_Rules != null)
            {
                if(m_Rules.m_Time > 0)
                {
                    m_Rules.m_Time = m_Rules.m_Time - 1;
                    ServerSend.ClientGameModeTimer(m_Rules.m_Time, this);
                    if (m_Rules.m_Time == 0)
                    {
                        m_PendingGameModeOverTimer = 25;

                        foreach (NetPeer Peer in m_Instance.ConnectedPeerList.ToArray())
                        {
                            ServerSend.SendFreeze(Peer);
                            m_PlayersData.GetPlayer(Peer.Id).m_GamePlayState = DataStr.PlayerData.GamePlayState.Unassigned;
                            ServerSend.SendLeaders(m_PlayersData.GetDMLeaders(), FilesManager.GetVictoryPosition(m_Config.m_GameMode, m_Config.m_SceneToSpawn), this);
                        }
                        m_ScenesData.UnloadScene(m_Config.m_SceneToSpawn);
                    }
                }
            }
            if(m_PendingGameModeOverTimer > 0)
            {
                m_PendingGameModeOverTimer--;
                if(m_PendingGameModeOverTimer == 0)
                {
                    m_ScenesData.UnloadScene(m_Config.m_SceneToSpawn);
                    ChangeGameMode(m_Config.m_GameMode, true);
                    m_PlayersData.ResetFrags();
                }
            }
            //m_ScenesData.UnloadSceneNobodyOn(this);
            m_ScenesData.UpdateZone();
        }

        public string GetRandomSceneForGameMode(string GameMode)
        {
            return FilesManager.GetRandomSceneForGameMode(GameMode);
        }

        public void ChangeGameMode(string GameMode, bool RollRandomMap = false)
        {
            m_Config.m_GameMode = GameMode;
            //m_ScenesData.UnloadScene(m_Config.m_SceneToSpawn);

            if (string.IsNullOrEmpty(m_Config.m_SceneToSpawn) || RollRandomMap)
            {
                m_Config.m_SceneToSpawn = GetRandomSceneForGameMode(m_Config.m_GameMode);
            }
            m_Rules = FilesManager.GetRules(GameMode);
            m_ScenesData.ChangeGameMode(GameMode);
            ServerSend.SendConfigUpdated(this);
            ServerSend.SendChangeMap(this);

            m_ScenesData.PopulateLoot(m_Config.m_SceneToSpawn, m_Rules.m_LootPerRadialSpawn);
        }

        public void StartServer()
        {
            StartServer(m_Port, m_Config.m_MaxPlayers);

            ChangeGameMode(m_Config.m_GameMode);

            m_NetworkHelper = new NetworkHelper(m_Port, "SkyCoopServer");
        }

        public void StartServer(int port, int maxPlayers, string key = "key")
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

        public void Dispose()
        {
            Logger.Log(ConsoleColor.Red, "[Server] Stopping Server");

            m_IsReady = false;
            if (m_VoiceServer != null)
                m_VoiceServer.Dispose();
            m_NetworkHelper.Dispose();
            m_Instance.Stop();
            GC.Collect();
        }
    }
}
