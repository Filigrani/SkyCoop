using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using SkyCoop;
#if (!DEDICATED)
using MelonLoader;
#endif

namespace GameServer
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public static Dictionary<int, string> p2pclients = new Dictionary<int, string>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;
        public static bool UsingSteamWorks = false;
        public static UdpClient udpListener;

        public static void Log(string TXT, Shared.LoggerColor Color = Shared.LoggerColor.White)
        {
#if (!DEDICATED)
            MelonLogger.Msg(MyMod.ConvertLoggerColor(Color),TXT);
#else
            Logger.Log(TXT, Color);
#endif
        }

        public static void Start(int _maxPlayers, int _port = 26950)
        {
            MaxPlayers = _maxPlayers;
            MyMod.MaxPlayers = MaxPlayers;
            Port = _port;

            Log("Starting server...");
            InitializeServerData();

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Log($"Server started on port {Port}.");

            MPStats.Start();
#if(!DEDICATED)
            MPStats.AddPlayer(MPSaveManager.GetSubNetworkGUID(), MyMod.MyChatName);
#endif
        }
        public static void StartSteam(int _maxPlayers, string[] whitelist = null)
        {
#if(!DEDICATED)
            MPSaveManager.LoadNonUnloadables();
            MaxPlayers = _maxPlayers;
            MyMod.MaxPlayers = MaxPlayers;
            Log("[SteamWorks.NET] Starting multiplayer...");
            InitializeServerData();
            UsingSteamWorks = true;
            Shared.InitAllPlayers(); // Prepare players objects based on amount of max players
            MyMod.iAmHost = true;
            MyMod.OverridedHourse = GameManager.GetTimeOfDayComponent().GetHour();
            MyMod.OverridedMinutes = GameManager.GetTimeOfDayComponent().GetMinutes();
            MyMod.OveridedTime = MyMod.OverridedHourse + ":" + MyMod.OverridedMinutes;
            MyMod.NeedSyncTime = true;
            //MyMod.LoadAllDropsForScene();
            //MyMod.LoadAllOpenableThingsForScene();
            MyMod.DisableOriginalAnimalSpawns(true);
            MyMod.SetFixedSpawn();
            MyMod.KillConsole();
            SteamConnect.Main.SetLobbyServer();
            SteamConnect.Main.SetLobbyState("Playing");
            Log("Server has been runned with InGame time: " + GameManager.GetTimeOfDayComponent().GetHour() + ":" + GameManager.GetTimeOfDayComponent().GetMinutes() + " seed " + GameManager.m_SceneTransitionData.m_GameRandomSeed);
            MPStats.Start();
            MPStats.AddPlayer(MPSaveManager.GetSubNetworkGUID(), MyMod.MyChatName);
#endif
        }

        private static void UDPReceiveCallback(IAsyncResult _result)
        {
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
                    string SubNetworkGUID = "";

                    if(_clientId != -2)
                    {
                        if (_packet.UnreadLength() > 12)
                        {
                            SubNetworkGUID = _packet.ReadString();

                            string Reason;
                            if (MPSaveManager.BannedUsers.TryGetValue(SubNetworkGUID, out Reason))
                            {
                                if (string.IsNullOrEmpty(Reason))
                                {
                                    ServerSend.KICKMESSAGE(_clientEndPoint, "You has been banned from the server.");
                                    return;
                                } else
                                {
                                    ServerSend.KICKMESSAGE(_clientEndPoint, "You has been banned from the server." + "\nReason: " + Reason);
                                    return;
                                }
                            }
                        } else
                        {
                            Log("Client without SubNetworkGUID trying to connect, rejecting, _clientId " + _clientId);
                            ServerSend.KICKMESSAGE(_clientEndPoint, "Your version of the mod, isn't supported\nHost using version " + MyMod.BuildInfo.Version);
                            return;
                        }
                        if (MyMod.DebugTrafficCheck)
                        {
                            Log("[DebugTrafficCheck] _clientId " + _clientId);
                            Log("[DebugTrafficCheck] SubNetworkGUID " + SubNetworkGUID);
                        }
                    }

                    if (_clientId == 0)
                    {
                        int freeSlot = 1;
                        bool ReConnection = false;

                        Log("[UDP] Checking all slots for " + _clientEndPoint.Address.ToString() +" subnetwork GUID "+ SubNetworkGUID);

                        for (int i = 1; i <= MaxPlayers; i++)
                        {
                            if (clients[i].udp != null && clients[i].udp.endPoint != null && clients[i].udp.endPoint.Address.ToString() == _clientEndPoint.Address.ToString() && SubNetworkGUID == clients[i].SubNetworkGUID)
                            {
                                ReConnection = true;
                                clients[i].RCON = false;
                                Log("[UDP] Reconnecting " + _clientEndPoint.Address +" " + clients[i].SubNetworkGUID + " as client " + i);
                                clients[i].TimeOutTime = 0;
                                clients[i].udp.endPoint = null;
                                freeSlot = i;
                                break;
                            }
                        }
                        if (ReConnection == false)
                        {
                            Log("[UDP] Got new connection " + _clientEndPoint.Address + " subnetwork GUID " + SubNetworkGUID);
                            for (int i = 1; i <= MaxPlayers; i++)
                            {
                                if (clients[i].udp.endPoint == null)
                                {
                                    clients[i].RCON = false;
                                    Log("[UDP] Here an empty slot " + i);
                                    freeSlot = i;
                                    break;
                                }
                            }
                        }
                        _clientId = freeSlot;
                    }else if(_clientId == -1)
                    {
                        Log("[UDP] Attempt to get RCON access for " + _clientEndPoint.Address.ToString());
                        int RCONSLOT = MaxPlayers+1;
                        if (clients[RCONSLOT].udp.endPoint == null)
                        {
                            _clientId = RCONSLOT;
                        }else{
                            if(clients[RCONSLOT].udp.endPoint.Address.ToString() == _clientEndPoint.Address.ToString())
                            {
                                Log("[UDP] RCON Operator reconnecting...");
                                _clientId = RCONSLOT;
                                ServerSend.RCONCONNECTED(_clientId);
                                return;
                            }
                            else{
                                Log("[UDP] RCON Slot currently busy");
                                ServerSend.KICKMESSAGE(_clientEndPoint, "RCON Operator slot is busy!");
                                return;
                            }
                        }
                    }else if(_clientId == -2)
                    {
                        ServerSend.PINGSERVER(_clientEndPoint);
                        return;
                    }

                    if (clients[_clientId].udp.endPoint == null)
                    {
                        // If this is a new connection
                        clients[_clientId].SubNetworkGUID = SubNetworkGUID;
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
                Log($"Error receiving UDP data: {_ex}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet, bool IgnoreReady = true)
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
                Log($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
            }
        }
        public static void SendP2PData(string _client, Packet _packet)
        {
#if (!DEDICATED)
            if(_client != "")
            {
                if(UsingSteamWorks == true && SteamConnect.CanUseSteam == true)
                {
                    SteamConnect.Main.SendUDPData(_packet, _client);
                }
            }
#endif
        }

        public static string GetMACByID(int ID)
        {
            if(ID == 0)
            {
                return MPSaveManager.GetSubNetworkGUID();
            }
            
            Client c;
            if(clients.TryGetValue(ID, out c))
            {
                return c.SubNetworkGUID;
            }
            return "";
        }

        public static List<string> GetMACsOfPlayers()
        {
            List<string> All = new List<string>();
#if (!DEDICATED)
            All.Add(MPSaveManager.GetSubNetworkGUID());
#endif

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].IsBusy())
                {
                    All.Add(clients[i].SubNetworkGUID);
                }
            }
            return All;
        }

        public static int GetIDByMAC(string MAC)
        {
            if (MAC == MPSaveManager.GetSubNetworkGUID())
            {
                return 0;
            }
            foreach (var item in clients)
            {
                if(item.Value.SubNetworkGUID == MAC)
                {
                    return item.Key;
                }
            }
            return -1;
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }
            if (MyMod.DedicatedServerAppMode)
            {
                clients.Add(MaxPlayers+1, new Client(MaxPlayers + 1));
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
                { (int)ClientPackets.ANIMSTATE, ServerHandle.ANIMSTATE},
                { (int)ClientPackets.SLEEPHOURS, ServerHandle.SLEEPHOURS},
                { (int)ClientPackets.SYNCWEATHER, ServerHandle.SYNCWEATHER},
                { (int)ClientPackets.REVIVE, ServerHandle.REVIVE},
                { (int)ClientPackets.REVIVEDONE, ServerHandle.REVIVEDONE},
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
                { (int)ClientPackets.CUSTOM, ServerHandle.CUSTOM},
                { (int)ClientPackets.GOTITEMSLICE, ServerHandle.GOTITEMSLICE},
                { (int)ClientPackets.VOICECHAT, ServerHandle.VOICECHAT},
                { (int)ClientPackets.SLICEDBYTES, ServerHandle.SLICEDBYTES},
                { (int)ClientPackets.ANIMALDAMAGE, ServerHandle.ANIMALDAMAGE},
                { (int)ClientPackets.FIREFUEL, ServerHandle.FIREFUEL},
                { (int)ClientPackets.DROPITEM, ServerHandle.DROPITEM},
                { (int)ClientPackets.GOTDROPSLICE, ServerHandle.GOTDROPSLICE},
                { (int)ClientPackets.REQUESTPICKUP, ServerHandle.REQUESTPICKUP},
                { (int)ClientPackets.REQUESTDROPSFORSCENE, ServerHandle.REQUESTDROPSFORSCENE},
                { (int)ClientPackets.REQUESTPLACE, ServerHandle.REQUESTPLACE},
                { (int)ClientPackets.GOTCONTAINERSLICE, ServerHandle.GOTCONTAINERSLICE},
                { (int)ClientPackets.REQUESTOPENCONTAINER, ServerHandle.REQUESTOPENCONTAINER},
                { (int)ClientPackets.CHANGEAIM, ServerHandle.CHANGEAIM},
                { (int)ClientPackets.USEOPENABLE, ServerHandle.USEOPENABLE},
                { (int)ClientPackets.TRYDIAGNISISPLAYER, ServerHandle.TRYDIAGNISISPLAYER},
                { (int)ClientPackets.SENDMYAFFLCTIONS, ServerHandle.SENDMYAFFLCTIONS},
                { (int)ClientPackets.CUREAFFLICTION, ServerHandle.CUREAFFLICTION},
                { (int)ClientPackets.ANIMALTEST, ServerHandle.ANIMALTEST},
                { (int)ClientPackets.ANIMALKILLED, ServerHandle.ANIMALKILLED},
                { (int)ClientPackets.REQUESTANIMALCORPSE, ServerHandle.REQUESTANIMALCORPSE},
                { (int)ClientPackets.QUARTERANIMAL, ServerHandle.QUARTERANIMAL},
                { (int)ClientPackets.ANIMALAUDIO, ServerHandle.ANIMALAUDIO},
                { (int)ClientPackets.PICKUPRABBIT, ServerHandle.PICKUPRABBIT},
                { (int)ClientPackets.RELEASERABBIT, ServerHandle.RELEASERABBIT},
                { (int)ClientPackets.HITRABBIT, ServerHandle.HITRABBIT},
                { (int)ClientPackets.CHANGEDFREQUENCY, ServerHandle.CHANGEDFREQUENCY},
                { (int)ClientPackets.MELEESTART, ServerHandle.MELEESTART},
                { (int)ClientPackets.TRYBORROWGEAR, ServerHandle.TRYBORROWGEAR},
                { (int)ClientPackets.ADDDEATHCONTAINER, ServerHandle.ADDDEATHCONTAINER},
                { (int)ClientPackets.DEATHCREATEEMPTYNOW, ServerHandle.DEATHCREATEEMPTYNOW},
                { (int)ClientPackets.SPAWNREGIONBANCHECK, ServerHandle.SPAWNREGIONBANCHECK},
                { (int)ClientPackets.CHALLENGETRIGGER, ServerHandle.CHALLENGETRIGGER},
                { (int)ClientPackets.RCONCOMMAND, ServerHandle.RCONCOMMAND},
                { (int)ClientPackets.ADDDOORLOCK, ServerHandle.ADDDOORLOCK},
                { (int)ClientPackets.TRYOPENDOOR, ServerHandle.TRYOPENDOOR},
                { (int)ClientPackets.LOCKPICK, ServerHandle.LOCKPICK},
                { (int)ClientPackets.VERIFYSAVE, ServerHandle.VERIFYSAVE},
                { (int)ClientPackets.SAVEHASH, ServerHandle.SAVEHASH},
                { (int)ClientPackets.FORCELOADING, ServerHandle.FORCELOADING},
                { (int)ClientPackets.REQUESTLOCKSMITH, ServerHandle.REQUESTLOCKSMITH},
                { (int)ClientPackets.APPLYTOOLONBLANK, ServerHandle.APPLYTOOLONBLANK},
                { (int)ClientPackets.LETENTER, ServerHandle.LETENTER},
                { (int)ClientPackets.PEEPHOLE, ServerHandle.PEEPHOLE},
                { (int)ClientPackets.KNOCKKNOCK, ServerHandle.KNOCKKNOCK},
                { (int)ClientPackets.RESTART, ServerHandle.RESTART},
                { (int)ClientPackets.WEATHERVOLUNTEER, ServerHandle.WEATHERVOLUNTEER},
                { (int)ClientPackets.REREGISTERWEATHER, ServerHandle.REREGISTERWEATHER},
                { (int)ClientPackets.CHANGECONTAINERSTATE, ServerHandle.CHANGECONTAINERSTATE},
                { (int)ClientPackets.TRIGGEREMOTE, ServerHandle.TRIGGEREMOTE},
                { (int)ClientPackets.PHOTOREQUEST, ServerHandle.PHOTOREQUEST},
                { (int)ClientPackets.GOTPHOTOSLICE, ServerHandle.GOTPHOTOSLICE},
                { (int)ClientPackets.STARTEXPEDITION, ServerHandle.STARTEXPEDITION},
                { (int)ClientPackets.ACCEPTEXPEDITIONINVITE, ServerHandle.ACCEPTEXPEDITIONINVITE},
                { (int)ClientPackets.REQUESTEXPEDITIONINVITES, ServerHandle.REQUESTEXPEDITIONINVITES},
                { (int)ClientPackets.CREATEEXPEDITIONINVITE, ServerHandle.CREATEEXPEDITIONINVITE},
                { (int)ClientPackets.ADDROCKCACH, ServerHandle.ADDROCKCACH},
                { (int)ClientPackets.REMOVEROCKCACH, ServerHandle.REMOVEROCKCACH},
                { (int)ClientPackets.REMOVEROCKCACHFINISHED, ServerHandle.REMOVEROCKCACHFINISHED},
                { (int)ClientPackets.CHARCOALDRAW, ServerHandle.CHARCOALDRAW},
                { (int)ClientPackets.CHATCOMMAND, ServerHandle.CHATCOMMAND},
                { (int)ClientPackets.REQUESTCONTAINERSTATE, ServerHandle.REQUESTCONTAINERSTATE},
            };
            Log("Initialized packets.");
        }
    }
}