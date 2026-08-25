using LiteNetLib;
using LiteNetLib.Utils;

namespace SkyCoopServer
{
    public class ServerVoice : IDisposable
    {
        public const int c_DefaultPort = 37850;
        public NetworkHelper m_NetworkHelper;

        public EventBasedNetListener m_Listener;
        public const float c_MaxProximityChatDistance = 30; // Voice3d AudioSource has it set to 25, but keep it a bit higher, to catch up with movement sync.
        public NetManager m_Instance;
        public bool m_IsReady = false;

        public Server m_GameServer = null;

        public ServerVoice(Server GameServer)
        {
            m_Listener = new EventBasedNetListener();
            m_Instance = new NetManager(m_Listener);
            m_GameServer = GameServer;
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

        public NetPeer GetClient(int Index)
        {
            if (m_Instance != null)
            {
                List<NetPeer> peers = new List<NetPeer>();
                m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Peer in peers.ToArray())
                {
                    if (Peer.Id == Index)
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
        }

        public void ExecuteVoice(NetPeer Peer, NetDataReader Reader)
        {
            if(m_GameServer != null && m_GameServer.m_IsReady)
            {
                DataStr.PlayerData Player = m_GameServer.GetPlayerDataByVoiceID(Peer.Id);

                if(Player != null)
                {
                    byte[] Data = new byte[Reader.GetInt()];
                    Reader.GetBytes(Data, Data.Length);
                    SendVoiceToWhoCanHearIt(Data, Player.m_PlayerID);
                }
            }
        }

        public void SendVoiceToWhoCanHearIt(byte[] Data, int SpeakerID)
        {
            List<NetPeer> peers = new List<NetPeer>();
            m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer VoiceClient in peers.ToArray())
            {
                if(m_GameServer != null && m_GameServer.m_IsReady)
                {
                    DataStr.PlayerData Player = m_GameServer.GetPlayerDataByVoiceID(VoiceClient.Id);

                    if(Player != null)
                    {
                        DataStr.PlayerHearing HearingMode = m_GameServer.m_PlayersData.PlayerCanHearOtherPlayer(SpeakerID, Player.m_PlayerID);

                        if (HearingMode != DataStr.PlayerHearing.None)
                        {
                            NetDataWriter writer = new NetDataWriter();
                            writer.Put((int)Packet.TypeVoice.Voice);
                            writer.Put(SpeakerID);
                            writer.Put((int)HearingMode);
                            writer.Put(Data.Length);
                            writer.Put(Data);
                            VoiceClient.Send(writer, DeliveryMethod.Unreliable);
                        }
                    }
                }
            }
        }

        public void SendVerificationRequestToClient(NetPeer Peer)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.TypeVoice.Verification);
            Logger.Log($"[ServerVoice] Sent voice verification to client {Peer.Id}");
            Peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void SendWelcomeToClient(NetPeer Peer)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put((int)Packet.TypeVoice.Welcome); //Welcome
            writer.Put($"Welcome to VoiceServer Client {Peer.Id}");
            Peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void StartServer(int port, int maxPlayers, string key = Packet.c_Key)
        {
            Logger.Log("[ServerVoice] Starting voice server");
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
                Logger.Log($"[ServerVoice] We got connection: {peer} assigned them as {peer.Id}");

                SendVerificationRequestToClient(peer);
            };

            m_Listener.PeerDisconnectedEvent += (peer, message) =>
            {
                Logger.Log($"[ServerVoice] Voice Client {peer.Id} disconnected {message.Reason.ToString()}");

                if(m_GameServer != null && m_GameServer.m_IsReady)
                {
                    m_GameServer.ClearVoiceID(peer.Id);
                }
            };

            m_Listener.NetworkLatencyUpdateEvent += (peer, ping) =>
            {
                //Logger.Log("[ServerVoice] Ping to Client {peer.Id}: {ping}");
            };
            m_Listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                int PacketID = dataReader.GetInt();

                switch ((Packet.TypeVoice)PacketID)
                {
                    case Packet.TypeVoice.Verification:

                        if (m_GameServer != null && m_GameServer.m_IsReady)
                        {
                            m_GameServer.SetVoiceIDForPlayer(dataReader.GetInt(), fromPeer.Id);
                            SendWelcomeToClient(fromPeer);
                        }
                        else
                        {
                            Logger.Log(ConsoleColor.Red, $"[ServerVoice] Client trying to verify themself, but game server is down, while voice chat server is up!");
                        }
                        break;
                    case Packet.TypeVoice.Voice:
                        ExecuteVoice(fromPeer, dataReader);
                        break;
                    default:
                        break;
                }

                dataReader.Recycle();
            };

            m_IsReady = true;
            Logger.Log($"[ServerVoice] Voice server is started port={port}");
            m_NetworkHelper = new NetworkHelper(port, "SkyCoopServerVoice");

            Task.Run(() => {
                while (m_GameServer.m_IsReady) 
                { 
                    Update(); 
                } 
            });
        }

        public void Dispose()
        {
            Logger.Log(ConsoleColor.Red, "[ServerVoice] Stopping VoiceServer");

            m_IsReady = false;
            m_Instance.Stop();
            if (m_NetworkHelper != null)
            {
                m_NetworkHelper.Dispose();
            }
        }
    }
}
