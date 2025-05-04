using LiteNetLib;
using LiteNetLib.Utils;

namespace SkyCoopServer
{
    public class ServerVoice
    {
        public int m_Port = 37850;


        public EventBasedNetListener m_Listener;
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
            byte[] Data = new byte[Reader.GetInt()];
            Reader.GetBytes(Data, Data.Length);

            SendVoiceToAll(Peer, Data);
        }

        public void SendVoiceToAll(NetPeer Peer, byte[] Data)
        {
            foreach (NetPeer _Peer in m_Instance.ConnectedPeerList)
            {
                //if (_Peer.Address != Peer.Address)
                //{
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put(Data.Length);
                    writer.Put(Data);
                    _Peer.Send(writer, DeliveryMethod.Unreliable);
                //}
            }
        }

        public void StartServer(int port, int maxPlayers, string key = "voice")
        {
            Console.WriteLine("Starting voice server");
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
                Console.WriteLine("We got connection: {0}", peer + " assigned them as " + peer.Id);
                ServerSend.Welcome(peer, "Welcome client " + peer.Id);
            };

            m_Listener.PeerDisconnectedEvent += (peer, message) =>
            {
                Console.WriteLine("Voice Client", peer.Id + " disconnected " + message.Reason.ToString());
            };

            m_Listener.NetworkLatencyUpdateEvent += (peer, ping) =>
            {
                //Console.WriteLine("Ping to Client "+peer.Id+": " + ping);
            };
            m_Listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                ExecuteVoice(fromPeer, dataReader);

                dataReader.Recycle();
            };

            m_IsReady = true;
            Console.WriteLine($"Voice server is started port={port}");
        }
    }
}
