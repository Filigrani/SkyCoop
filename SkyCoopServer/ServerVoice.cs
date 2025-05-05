using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;

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
                if (_Peer.Id != Peer.Id)
                {
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put(0);
                    writer.Put(ClientId);
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
            Console.WriteLine("[VoiceServer] Starting voice server");
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
                Console.WriteLine("[VoiceServer] We got connection: {0}", peer + " assigned them as " + peer.Id);

                SendWelcomeToClient(peer);
            };

            m_Listener.PeerDisconnectedEvent += (peer, message) =>
            {
                Console.WriteLine("[VoiceServer] Voice Client", peer.Id + " disconnected " + message.Reason.ToString());
            };

            m_Listener.NetworkLatencyUpdateEvent += (peer, ping) =>
            {
                //Console.WriteLine("[VoiceServer] Ping to Client "+peer.Id+": " + ping);
            };
            m_Listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                ExecuteVoice(fromPeer, dataReader);

                dataReader.Recycle();
            };

            m_IsReady = true;
            Console.WriteLine($"[VoiceServer] Voice server is started port={port}");

            Task.Run(() => {
                while (m_GameServer.m_IsReady) 
                { 
                    Update(); 
                } 
            });
        }
    }
}
