using LiteNetLib;
using LiteNetLib.Utils;
using SkyCoopServer;

namespace SkyCoop
{
    public class Client
    {
        public static int m_ConnectPort = 37855;
        public static int m_LocalPort = 37856;
        public static int m_Protocol = 1;

        public class DelayedPackage
        {
            public int m_PackID = 0;
            public NetDataReader m_Reader = null;

            public DelayedPackage(int PackID, NetDataReader Reader)
            {
                m_PackID = PackID;
                m_Reader = Reader;
            }
        }

        public delegate void PacketHandler(NetDataReader Reader);
        public static Dictionary<int, PacketHandler> s_packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)Packet.Type.Welcome, ClientHandle.Welcome },
            { (int)Packet.Type.CFG, ClientHandle.ServerConfig },
            { (int)Packet.Type.ClientPosition, ClientHandle.ClientPosition },
            { (int)Packet.Type.ClientRotation, ClientHandle.ClientRotation },
            { (int)Packet.Type.ClientScene, ClientHandle.ClientSceneNotification },
            { (int)Packet.Type.ClientHoldigGear, ClientHandle.ClientHoldingGear },
            { (int)Packet.Type.ClientCrouch, ClientHandle.ClientCrouch },
            { (int)Packet.Type.ClientAction, ClientHandle.ClientAction },
            { (int)Packet.Type.ClientFire, ClientHandle.ClientFire },
            { (int)Packet.Type.ClientDamageOtherClient, ClientHandle.ClientDamagesMe },
            { (int)Packet.Type.ClientProjectile, ClientHandle.ClientProjectile },
            { (int)Packet.Type.KillFeedMessage, ClientHandle.KillFeedMessage },
            { (int)Packet.Type.ClientProjectileThrow, ClientHandle.ClientProjectileThrow },
            { (int)Packet.Type.ClientName, ClientHandle.ClientName },
            { (int)Packet.Type.ClientRequestRespawn, ClientHandle.ClientRequestRespawn },
            { (int)Packet.Type.ClientInjectedItem, ClientHandle.ClientInjectedItem },
            { (int)Packet.Type.ClientRemoveInjectedItem, ClientHandle.ClientRemoveInjectedItem },
            { (int)Packet.Type.ClientGettingDamage, ClientHandle.ClientGettingDamage },
            { (int)Packet.Type.ClientSendGear, ClientHandle.ClientSendGear },
            { (int)Packet.Type.ClientPickUpGear, ClientHandle.ClientPickUpGear },
            { (int)Packet.Type.ClientRemoveGear, ClientHandle.ClientRemoveGear },
            { (int)Packet.Type.ClientOpenableInteraction, ClientHandle.ClientOpenableInteraction },
            { (int)Packet.Type.ClientClothing, ClientHandle.ClientClothing },
        };

        public static void ExecutePacketEvent(int PacketID, NetDataReader Reader)
        {
            PacketHandler Handle;

            if (s_packetHandlers.TryGetValue(PacketID, out Handle))
            {
                Handle(Reader);
            }
        }

        public NetPacketProcessor m_PacketProcessor = new NetPacketProcessor();
        public EventBasedNetListener m_Listener;
        public NetManager m_Instance;
        public NetPeer m_HostEndPoint;
        public NetPeer m_MyEndPoint;
        public List<DelayedPackage> m_DelayedPackage = new List<DelayedPackage> { };
        public DataStr.ServerConfig m_Config = new DataStr.ServerConfig();
        public DataStr.GameRules m_Rules = new DataStr.GameRules();

        public bool m_IsReady = false;
        public int GetMyId()
        {
            if (m_MyEndPoint == null)
            {
                return -1;
            }
            return m_MyEndPoint.RemoteId;
        }


        public Client()
        {
            m_Listener = new EventBasedNetListener();
            m_Instance = new NetManager(m_Listener);
            m_Config = new DataStr.ServerConfig();

            m_Listener.NetworkErrorEvent += (fromPeer, error) =>
            {
                Logger.Log(ConsoleColor.Red, "Connection failed: " + error);
                MenuHook.RemovePleaseWait();
                MenuHook.DoOKMessage("Connection failed", error.ToString());
                m_IsReady = false;
                m_Instance.Stop();
            };
            m_Listener.NetworkLatencyUpdateEvent += (peer, ping) =>
            {
                //Logger.Log(ConsoleColor.Cyan, "Ping to host: " + ping);
            };

            m_Listener.PeerDisconnectedEvent += (peer, message) =>
            {
                Logger.Log(ConsoleColor.Red, "Disconnected: " + message.Reason);
                Logger.Log(ConsoleColor.Red, message.AdditionalData);

                if (peer.RemoteId == 0)
                {
                    string Message = "Unknown reason";

                    if (message.Reason == DisconnectReason.RemoteConnectionClose)
                    {
                        //TODO: Print Host message
                    } else
                    {
                        switch (message.Reason)
                        {
                            case DisconnectReason.ConnectionFailed:
                                Message = "Wasn't able to connect to the server.";
                                break;
                            case DisconnectReason.Timeout:
                                Message = "Disconnected doe timeout.";
                                break;
                            case DisconnectReason.HostUnreachable:
                                Message = "Server is unreachable.";
                                break;
                            case DisconnectReason.NetworkUnreachable:
                                Message = "Network is unreachable.";
                                break;
                            case DisconnectReason.RemoteConnectionClose:
                                break;
                            case DisconnectReason.DisconnectPeerCalled:
                                Message = "Disconnected by my request.";
                                break;
                            case DisconnectReason.ConnectionRejected:
                                break;
                            case DisconnectReason.InvalidProtocol:
                                Message = "Invalid connection protocol.";
                                break;
                            case DisconnectReason.UnknownHost:
                                Message = "Unknown host.";
                                break;
                            case DisconnectReason.Reconnect:
                                Message = "Reconnect";
                                break;
                            case DisconnectReason.PeerToPeerConnection:
                                Message = "Peer to Peer Connection";
                                break;
                            case DisconnectReason.PeerNotFound:
                                Message = "Peer not found.";
                                break;
                            default:
                                break;
                        }
                    }
                    m_IsReady = false;
                    m_Instance.Stop();
                    MenuHook.RemovePleaseWait();
                    MenuHook.DoOKMessage("Disconnected", Message);
                }
            };

            m_Listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                m_HostEndPoint = fromPeer;
                int PacketID = dataReader.GetInt();
                //SkyCoop.Logger.Log(ConsoleColor.Cyan, "PacketID " + PacketID);
                if (m_IsReady)
                {
                    ExecutePacketEvent(PacketID, dataReader);
                    dataReader.Recycle();
                    return;
                }
                else
                {
                    if (PacketID == 0 || PacketID == 1)
                    {
                        ExecutePacketEvent(PacketID, dataReader);
                        dataReader.Recycle();
                    }
                    else
                    {
                        m_DelayedPackage.Add(new DelayedPackage(PacketID, dataReader));
                        SkyCoop.Logger.Log(ConsoleColor.Yellow, "Pushing "+PacketID+" to delay");
                    }
                }
            };
        }

        public void ProcessAllDelayedPackages()
        {
            for (int i = 0; i < m_DelayedPackage.Count; i++)
            {
                DelayedPackage Pack = m_DelayedPackage[i];
                SkyCoop.Logger.Log(ConsoleColor.Green, "Processing delayed package " + Pack.m_PackID);
                ExecutePacketEvent(Pack.m_PackID, Pack.m_Reader);
            }
            m_DelayedPackage.Clear();
        }

        public void SendToHost(NetDataWriter writer)
        {
            if (m_Instance != null)
            {
                m_HostEndPoint.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }

        public void ConnectToServer(string address)
        {
            int Port = m_ConnectPort;
            string IP = "127.0.0.1";

            if (!string.IsNullOrEmpty(address))
            {
                if (address.Contains(":"))
                {
                    char seperator = Convert.ToChar(":");

                    string[] sliced = address.Split(seperator);
                    IP = sliced[0];
                    Port = int.Parse(sliced[1]);
                } else
                {
                    IP = address;
                    Port = m_ConnectPort;
                }
            }
            ConnectToServer(IP, Port);
        }

        public void ConnectToServer(string ip, int port, string key = "key")
        {
            MenuHook.DoPleaseWait("Connecting...", "Trying to connect to "+ip+":"+port);
            Logger.Log($"Trying to connect to {ip}:{port} with key: {key}");
            Logger.Log("m_Instance.DisconnectTimeout "+ m_Instance.DisconnectTimeout);
            m_Instance.Start();
            m_MyEndPoint = m_Instance.Connect(ip, port, key);
        }

        public void ConnectToServerVoice(int port)
        {
            ModMain.ClientVoice = new SkyCoopClient.ClientVoice();
            ModMain.ClientVoice.Connect(ModMain.Client.m_HostEndPoint.Address, port);
        }
    }
}
