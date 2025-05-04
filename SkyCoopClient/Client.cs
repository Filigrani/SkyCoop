using LiteNetLib;
using LiteNetLib.Utils;
using MelonLoader;
using System.Net;
using SkyCoopServer;
using MonoMod.Utils;
using UnityEngine;
using SkyCoopClient;

namespace SkyCoop
{
    public class Client
    {
        public static int m_ConnectPort = 26950;
        public static int m_LocalPort = 26951;
        public static int m_Protocol = 1;

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

        public bool m_IsReady = false;


        public Client()
        {
            m_Listener = new EventBasedNetListener();
            m_Instance = new NetManager(m_Listener);

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

                ExecutePacketEvent(PacketID, dataReader);

                dataReader.Recycle();
            };
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
            m_Instance.Connect(ip, port, key);
        }
    }
}
