using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;

        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            Console.WriteLine($"Server started on port {Port}.");
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
                { (int)ClientPackets.HASRIFLE, ServerHandle.HASRIFLE},
                { (int)ClientPackets.HASREVOLVER, ServerHandle.HASREVOLVER},
                { (int)ClientPackets.SLEEPHOURS, ServerHandle.SLEEPHOURS},
                { (int)ClientPackets.SYNCWEATHER, ServerHandle.SYNCWEATHER},
                { (int)ClientPackets.REVIVE, ServerHandle.REVIVE},
                { (int)ClientPackets.REVIVEDONE, ServerHandle.REVIVEDONE},
                { (int)ClientPackets.HASAXE, ServerHandle.HASAXE},
                { (int)ClientPackets.HISARROWS, ServerHandle.HISARROWS},
                { (int)ClientPackets.HASMEDKIT, ServerHandle.HASMEDKIT},
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
                
            };
            Console.WriteLine("Initialized packets.");
        }
    }
}