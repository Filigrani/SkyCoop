using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using SkyCoop;

namespace GameServer
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        public static bool UsingSteamWorks = false;
        public static Dictionary<int, string> p2pclients = new Dictionary<int, string>();

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            MyMod.MaxPlayers = MaxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            //tcpListener = new TcpListener(IPAddress.Any, Port);
            //tcpListener.Start();
            //tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"Server started on port {Port}.");
        }
        public static void StartSteam(int _maxPlayers)
        {
            MaxPlayers = _maxPlayers;
            MyMod.MaxPlayers = MaxPlayers;
            Console.WriteLine("Starting server...");
            InitializeServerData();
            Console.WriteLine("[SteamWorks.NET] Server started!");
            UsingSteamWorks = true;
            MyMod.iAmHost = true;
            MyMod.OverridedHourse = GameManager.GetTimeOfDayComponent().GetHour();
            MyMod.OverridedMinutes = GameManager.GetTimeOfDayComponent().GetMinutes();
            MyMod.OveridedTime = MyMod.OverridedHourse + ":" + MyMod.OverridedMinutes;
            MyMod.NeedSyncTime = true;
            MyMod.RealTimeCycleSpeed = true;
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            //MelonLoader.MelonLogger.Log("[UDP] UDPReceiveCallback");
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = null;
                try
                {
                    _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                }
                catch (System.Net.Sockets.SocketException)
                {

                    //MelonLoader.MelonLogger.Log("[UDP] Client disconnected!");
                    udpListener.BeginReceive(UDPReceiveCallback, null);
                    return;
                }
                if (_data == null)
                {
                    return;
                }

                udpListener.BeginReceive(UDPReceiveCallback, null);


                if (_data.Length < 4)
                {
                    return;
                }
                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    if (_clientId == 0)
                    {
                        int freeSlot = 1;
                        bool ReConnection = false;

                        MelonLoader.MelonLogger.Msg("[UDP] Checking all slots for " + _clientEndPoint.Address.ToString());

                        for (int i = 1; i <= MaxPlayers; i++)
                        {
                            if (clients[i].udp != null && clients[i].udp.endPoint != null && clients[i].udp.endPoint.Address.ToString() == _clientEndPoint.Address.ToString())
                            {
                                ReConnection = true;
                                MelonLoader.MelonLogger.Msg("[UDP] Reconnecting " + _clientEndPoint.Address + " as client " + i);
                                clients[i].TimeOutTime = 0;
                                clients[i].udp.endPoint = null;
                                freeSlot = i;
                                break;
                            }
                        }
                        if (ReConnection == false)
                        {
                            MelonLoader.MelonLogger.Msg("[UDP] Got new connection " + _clientEndPoint.Address);
                            for (int i = 1; i <= MaxPlayers; i++)
                            {
                                if (clients[i].udp.endPoint == null)
                                {
                                    MelonLoader.MelonLogger.Msg("[UDP] Here an empty slot " + i);
                                    freeSlot = i;
                                    break;
                                }
                            }
                        }
                        _clientId = freeSlot;
                    }

                    if (clients[_clientId].udp.endPoint == null)
                    {
                        // If this is a new connection
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        // Ensures that the client is not being impersonated by another by sending a false clientID
                        clients[_clientId].udp.HandleData(_packet);
                    }
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
            }
        }
        public static void SendP2PData(string _client, Packet _packet)
        {
            if(_client != "")
            {
                if(UsingSteamWorks == true && SteamConnect.CanUseSteam == true)
                {
                    ulong sid = ulong.Parse(_client);
                    SteamConnect.Main.SendUDPData(_packet, _client);
                }
            }
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.XYZ, ServerHandle.XYZ },
                { (int)ClientPackets.XYZW, ServerHandle.XYZW },
                { (int)ClientPackets.BLOCK, ServerHandle.BLOCK },
                { (int)ClientPackets.XYZDW, ServerHandle.XYZDW },
                { (int)ClientPackets.LEVELID, ServerHandle.LEVELID },
                { (int)ClientPackets.GOTITEM, ServerHandle.GOTITEM },
                { (int)ClientPackets.GAMETIME, ServerHandle.GAMETIME},
                { (int)ClientPackets.LIGHTSOURCENAME, ServerHandle.LIGHTSOURCENAME},
                { (int)ClientPackets.LIGHTSOURCE, ServerHandle.LIGHTSOURCE},
                ///{ (int)ClientPackets.MAKEFIRE, ServerHandle.MAKEFIRE},
                { (int)ClientPackets.ANIMSTATE, ServerHandle.ANIMSTATE},
                { (int)ClientPackets.SLEEPHOURS, ServerHandle.SLEEPHOURS},
                { (int)ClientPackets.SYNCWEATHER, ServerHandle.SYNCWEATHER},
                { (int)ClientPackets.REVIVE, ServerHandle.REVIVE},
                { (int)ClientPackets.REVIVEDONE, ServerHandle.REVIVEDONE},
                { (int)ClientPackets.ANIMALSYNC, ServerHandle.ANIMALSYNC},
                { (int)ClientPackets.ANIMALSYNCTRIGG, ServerHandle.ANIMALSYNCTRIGG},
                { (int)ClientPackets.DARKWALKERREADY, ServerHandle.DARKWALKERREADY},
                { (int)ClientPackets.WARDISACTIVE, ServerHandle.WARDISACTIVE},
                { (int)ClientPackets.REQUESTDWREADYSTATE, ServerHandle.REQUESTDWREADYSTATE},
                { (int)ClientPackets.DWCOUNTDOWN, ServerHandle.DWCOUNTDOWN},
                { (int)ClientPackets.SHOOTSYNC, ServerHandle.SHOOTSYNC},
                { (int)ClientPackets.PIMPSKILL, ServerHandle.PIMPSKILL},
                { (int)ClientPackets.HARVESTINGANIMAL, ServerHandle.HARVESTINGANIMAL},
                { (int)ClientPackets.DONEHARVASTING, ServerHandle.DONEHARVASTING},
                { (int)ClientPackets.BULLETDAMAGE, ServerHandle.BULLETDAMAGE},
                { (int)ClientPackets.MULTISOUND, ServerHandle.MULTISOUND},
                { (int)ClientPackets.CONTAINEROPEN, ServerHandle.CONTAINEROPEN},
                { (int)ClientPackets.LUREPLACEMENT, ServerHandle.LUREPLACEMENT},
                { (int)ClientPackets.LUREISACTIVE, ServerHandle.LUREISACTIVE},
                { (int)ClientPackets.ALIGNANIMAL, ServerHandle.ALIGNANIMAL},
                { (int)ClientPackets.ASKFORANIMALPROXY, ServerHandle.ASKFORANIMALPROXY},
                { (int)ClientPackets.CARRYBODY, ServerHandle.CARRYBODY},
                { (int)ClientPackets.BODYWARP, ServerHandle.BODYWARP},
                { (int)ClientPackets.ANIMALDELETE, ServerHandle.ANIMALDELETE},
                { (int)ClientPackets.KEEPITALIVE, ServerHandle.KEEPITALIVE},
                { (int)ClientPackets.EQUIPMENT, ServerHandle.EQUIPMENT},
                { (int)ClientPackets.CHAT, ServerHandle.CHAT},
                { (int)ClientPackets.CONNECTSTEAM, ServerHandle.CONNECTSTEAM},
                { (int)ClientPackets.CHANGENAME, ServerHandle.CHANGENAME},
                { (int)ClientPackets.CLOTH, ServerHandle.CLOTH},
                { (int)ClientPackets.ASKSPAWNDATA, ServerHandle.ASKSPAWNDATA},
                { (int)ClientPackets.LEVELGUID, ServerHandle.LEVELGUID},
                { (int)ClientPackets.FURNBROKEN, ServerHandle.FURNBROKEN},
                { (int)ClientPackets.FURNBREAKINGGUID, ServerHandle.FURNBREAKINGGUID},
                { (int)ClientPackets.FURNBREAKINSTOP, ServerHandle.FURNBREAKINSTOP},
                { (int)ClientPackets.GEARPICKUP, ServerHandle.GEARPICKUP},
                { (int)ClientPackets.ROPE, ServerHandle.ROPE},
                { (int)ClientPackets.CONSUME, ServerHandle.CONSUME},
                { (int)ClientPackets.STOPCONSUME, ServerHandle.STOPCONSUME},
                { (int)ClientPackets.HEAVYBREATH, ServerHandle.HEAVYBREATH},
                { (int)ClientPackets.BLOODLOSTS, ServerHandle.BLOODLOSTS},
                { (int)ClientPackets.APPLYACTIONONPLAYER, ServerHandle.APPLYACTIONONPLAYER},
                { (int)ClientPackets.DONTMOVEWARNING, ServerHandle.DONTMOVEWARNING},
                { (int)ClientPackets.INFECTIONSRISK, ServerHandle.INFECTIONSRISK},
                { (int)ClientPackets.CONTAINERINTERACT, ServerHandle.CONTAINERINTERACT},
                { (int)ClientPackets.HARVESTPLANT, ServerHandle.HARVESTPLANT},
                { (int)ClientPackets.SELECTEDCHARACTER, ServerHandle.SELECTEDCHARACTER},
                { (int)ClientPackets.ADDSHELTER, ServerHandle.ADDSHELTER},
                { (int)ClientPackets.REMOVESHELTER, ServerHandle.REMOVESHELTER},
                { (int)ClientPackets.USESHELTER, ServerHandle.USESHELTER},
                { (int)ClientPackets.FIRE, ServerHandle.FIRE},
            };
            Console.WriteLine("Initialized packets.");
        }
    }
}