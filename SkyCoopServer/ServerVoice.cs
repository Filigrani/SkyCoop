using LiteNetLib;
using LiteNetLib.Utils;

namespace SkyCoopServer
{
    public class ServerVoice : IDisposable
    {
        public int m_Port = 37850;
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
                foreach (NetPeer Peer in m_Instance.ConnectedPeerList)
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
                foreach (NetPeer Peer in m_Instance.ConnectedPeerList)
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

        public void StartServer()
        {
            StartServer(m_Port, m_GameServer.m_Config.m_MaxPlayers);
        }

        public void ExecuteVoice(NetPeer Peer, NetDataReader Reader)
        {
            if(Reader.GetInt() == 0)
            {
                int clientId = Reader.GetInt();
                byte[] Data = new byte[Reader.GetInt()];
                Reader.GetBytes(Data, Data.Length);

                SendVoiceToAll(Peer, Data, clientId);
            }
        }

        public void SendVoiceToAll(NetPeer Peer, byte[] Data, int ClientId)
        {
            foreach (NetPeer _Peer in m_Instance.ConnectedPeerList)
            {
                DataStr.PlayerHearing HearingMode = m_GameServer.m_PlayersData.PlayerCanHearOtherPlayer(ClientId, _Peer.Id);


                if (HearingMode != DataStr.PlayerHearing.None)
                {
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put(0);
                    writer.Put(ClientId);
                    writer.Put((int)HearingMode);
                    writer.Put(Data.Length);
                    writer.Put(Data);
                    _Peer.Send(writer, DeliveryMethod.Unreliable);
                }
            }
        }

        public void SendWelcomeToClient(NetPeer Peer)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(1); //Welcome
            writer.Put($"Welcome to VoiceServer Client№{Peer.Id}");
            Peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void StartServer(int port, int maxPlayers, string key = "voice")
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

                SendWelcomeToClient(peer);
            };

            m_Listener.PeerDisconnectedEvent += (peer, message) =>
            {
                Logger.Log($"[ServerVoice] Voice Client {peer.Id} disconnected {message.Reason.ToString()}");
            };

            m_Listener.NetworkLatencyUpdateEvent += (peer, ping) =>
            {
                //Logger.Log("[ServerVoice] Ping to Client {peer.Id}: {ping}");
            };
            m_Listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                ExecuteVoice(fromPeer, dataReader);

                dataReader.Recycle();
            };

            m_IsReady = true;
            Logger.Log($"[ServerVoice] Voice server is started port={port}");
            m_NetworkHelper = new NetworkHelper(m_Port, "SkyCoopServerVoice");

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
            m_NetworkHelper.Dispose();
        }
    }
}
