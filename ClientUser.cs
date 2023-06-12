using MelonLoader;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class ClientUser
    {
        public static int dataBufferSize = 4096;

        public static string ip = "127.0.0.1";
        public static string LastConnectedIp = "";
        public static string PendingConnectionIp = "";
        public static int ConnectPort = 26950;
        public static int LocalPort = 26951;
        public static string SubNetworkClientGUID = "";
        public static int myId = 0;
        public static UDP udp;

        public delegate void PacketHandler(Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        public class PingData
        {
            public string ServerData = "";
            public int PlayersMax = 0;
            public int Players = 0;
            public bool CanJoin = false;
            public string Error = "";
        }

        public static PingData Ping(string IP, int PORT)
        {
            PingData Dat = new PingData();
            // Request of server status should be done via UDP.
            // Use same port as game server use.
            // Default port of Sky Co-op Server is 26950 (if not listed otherwise).
            // Example: Game server with address 87.254.159.105:26950
            // Will respond with status data by same 26950 port,
            // aswell as servers that listed as 87.254.159.105
            // will respond by port 26950, but I think is obvious.

            // Send to server (signed int) -2 message to request servers status.

            UdpClient Pinger = new UdpClient();
            Pinger.Client.ReceiveTimeout = 5000;
            Pinger.Client.SendTimeout = 5000;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);

            try
            {
                Pinger.Connect(endPoint);
                byte[] Buffer = BitConverter.GetBytes(-2);
                Pinger.Send(Buffer.ToArray(), Buffer.Length);

                byte[] ReturnData = Pinger.Receive(ref endPoint);
                // First 4 bytes is header, always should be 147
                // Next 4 bytes is server name length.
                // Next Unicode formated string of server name.
                // Next 4 bytes is Mod's version string length
                // Next UTF8 formated string of version.
                // Next 4 bytes after version string is current amout of players on server.
                // Next 4 bytes is max players slots on server.
                // Next 4 bytes is server information json string length.
                // Next Unicode formated string of server's information.
                if (ReturnData == null || ReturnData.Length < 4)
                {
                    Dat.CanJoin = false;
                    Dat.Error = "The server is not responding.";
                    return Dat;
                }

                int i = 0;
                int IntHeader = BitConverter.ToInt32(ReturnData, i);
                if (IntHeader == 147)
                {
                    MelonLogger.Msg("Correct header!");
                    Dat.CanJoin = true;
                } else
                {
                    MelonLogger.Msg("Wrong header!");
                    Dat.CanJoin = false;
                    Dat.Error = "Server respond incorrect data, make sure client and server use same version of the mod.";
                    return Dat;
                }
                i += 4;
                int ServerNameLength = BitConverter.ToInt32(ReturnData, i);
                i += 4;
                string ServerName = Encoding.Unicode.GetString(ReturnData, i, ServerNameLength);
                i += ServerNameLength;
                int ServerVersionStringLength = BitConverter.ToInt32(ReturnData, i);
                i += 4;
                string ServerVersion = Encoding.UTF8.GetString(ReturnData, i, ServerVersionStringLength);
                i += ServerVersionStringLength;
                int Players = BitConverter.ToInt32(ReturnData, i);
                i += 4;
                int PlayersMax = BitConverter.ToInt32(ReturnData, i);
                i += 4;
                int ServerConfigStringLength = BitConverter.ToInt32(ReturnData, i);
                i += 4;
                string ConfigString = Encoding.Unicode.GetString(ReturnData, i, ServerConfigStringLength);

                Dat.ServerData = ConfigString;
                Dat.Players = Players;
                Dat.PlayersMax = PlayersMax;

                MelonLogger.Msg(ServerName + " version " + ServerVersion + " Players: " + Players + "/" + PlayersMax + " Server json:");
                MelonLogger.Msg(ConfigString);
                Pinger.Close();

                if(Players >= PlayersMax)
                {
                    Dat.CanJoin = false;
                    Dat.Error = "Server is full!";
                }
                if(ServerVersion != MyMod.BuildInfo.Version)
                {
                    Dat.CanJoin = false;
                    Dat.Error = "Server using "+ ServerVersion+" version of the mod!";
                }
                MyMod.MaxPlayers = PlayersMax;
                Shared.InitAllPlayers();
                return Dat;
            }
            catch (Exception e)
            {
                Dat.Error = e.Message;
                Dat.CanJoin = false;
                return Dat;
            }
            Dat.Error = "Wasn't able to reach server.";
            return Dat;
        }

        public static void ConnectToServer()
        {
            myId = 0;
            SubNetworkClientGUID = MPSaveManager.GetSubNetworkGUID();

            udp = new UDP();

            if (ip != "")
            {
                MelonLogger.Msg("My SubNetworkGUID " + SubNetworkClientGUID);
                InitializeClientData();
                udp.Connect(LocalPort, ip);
            }
        }
        public static void DoConnectToIp(string _ip)
        {
            string newPort = "";
            string newIP = "";
            if (_ip.Contains(":"))
            {
                char seperator = Convert.ToChar(":");

                string[] sliced = _ip.Split(seperator);
                newIP = sliced[0];
                newPort = sliced[1];
            }

            if (newIP == "" && newPort == "")
            {
                PendingConnectionIp = _ip;
                ip = _ip;
                MelonLogger.Msg("Going to connect to " + ip + ":" + ConnectPort);
            }
            else
            {
                PendingConnectionIp = newIP;
                ip = newIP;
                ConnectPort = Convert.ToInt32(newPort);
                MelonLogger.Msg("Going to connect to " + ip + ":" + ConnectPort);
            }
            MyMod.DoWaitForConnect(false);
            PingData D = Ping(ip, ConnectPort);

            if (D.CanJoin)
            {
                ConnectToServer();
            } else
            {
                MyMod.RemoveWaitForConnect();
                MyMod.DoOKMessage("Connection failed", D.Error);
                MelonLogger.Msg("Connection failed " + D.Error);
            }
        }
        public class UDP
        {
            public UdpClient socket;
            public IPEndPoint endPoint;

            public UDP()
            {
                endPoint = new IPEndPoint(IPAddress.Parse(ip), ConnectPort);
            }

            /// <summary>Attempts to connect to the server via UDP.</summary>
            /// <param name="_localPort">The port number to bind the UDP socket to.</param>
            public void Connect(int _localPort, string ip)
            {
                endPoint = new IPEndPoint(IPAddress.Parse(ip), ConnectPort);
                socket = new UdpClient(_localPort);
                socket.Connect(endPoint);
                MelonLogger.Msg("Open socket for " + endPoint.Address + ":" + endPoint.Port +" local port is "+ _localPort);
                //IAsyncResult result = socket.BeginReceive(ReceiveCallback, null);
                socket.BeginReceive(ReceiveCallback, null);
                //bool success = result.AsyncWaitHandle.WaitOne(5000, true);
                using (Packet _packet = new Packet())
                {
                    _packet.Write(myId);
                    //_packet.Write(SubNetworkClientGUID);

                    SendData(_packet);
                }
            }

            /// <summary>Sends data to the client via UDP.</summary>
            /// <param name="_packet">The packet to send.</param>
            public void SendData(Packet _packet, bool IgnoreQuit = false)
            {
                if (!IgnoreQuit && MyMod.QuitWhenSaveOperationFinished)
                {
                    return;
                }

                try
                {
                    _packet.WriteLength();
                    _packet.InsertClientInfo(myId, SubNetworkClientGUID);
                    

                    if (socket != null)
                    {
                        socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    MelonLogger.Msg($"Error sending data to server via UDP: {_ex}");
                }
            }

            /// <summary>Receives incoming UDP data.</summary>
            private void ReceiveCallback(IAsyncResult _result)
            {
                if (MyMod.QuitWhenSaveOperationFinished)
                {
                    return;
                }

                try
                {
                    byte[] _data = socket.EndReceive(_result, ref endPoint);
                    socket.BeginReceive(ReceiveCallback, null);
                    if (_data.Length < 4)
                    {
                        endPoint = null;
                        socket.Close();

                        MyMod.Disconnect();
                        MelonLogger.Msg("Connection failed");
                        return;
                    }
                    HandleData(_data);
                }
                catch
                {
                    endPoint = null;
                    socket.Close();
                    MyMod.Disconnect();
                    MelonLogger.Msg("Connection failed");
                }
            }

            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="_data">The recieved data.</param>
            private void HandleData(byte[] _data)
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetLength = _packet.ReadInt();
                    _data = _packet.ReadBytes(_packetLength);
                }

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_data))
                    {
                        int _packetId = _packet.ReadInt();
                        if (MyMod.DebugTrafficCheck == true)
                        {
                            MelonLogger.Msg(ConsoleColor.Yellow, "[DebugTrafficCheck] Got packet ID " + _packetId);
                            MelonLogger.Msg(ConsoleColor.Yellow, "[DebugTrafficCheck] Packet size " + _packet.ReturnSize() + " bytes");
                        }
                        packetHandlers[_packetId](_packet); // Call appropriate method to handle the packet
                    }
                });
            }

            /// <summary>Disconnects from the server and cleans up the UDP connection.</summary>
            private void Disconnect()
            {
                Disconnect();

                endPoint = null;
                socket = null;
            }
        }
        public static void InitializeClientData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.XYZ, ClientHandle.XYZ},
            { (int)ServerPackets.XYZW, ClientHandle.XYZW},
            { (int)ServerPackets.BLOCK, ClientHandle.BLOCK},
            { (int)ServerPackets.XYZDW, ClientHandle.XYZDW},
            { (int)ServerPackets.LEVELID, ClientHandle.LEVELID},
            { (int)ServerPackets.GOTITEM, ClientHandle.GOTITEM},
            { (int)ServerPackets.GAMETIME, ClientHandle.GAMETIME},
            { (int)ServerPackets.LIGHTSOURCENAME, ClientHandle.LIGHTSOURCENAME},
            { (int)ServerPackets.LIGHTSOURCE, ClientHandle.LIGHTSOURCE},
            { (int)ServerPackets.ANIMSTATE, ClientHandle.ANIMSTATE},
            { (int)ServerPackets.SLEEPHOURS, ClientHandle.SLEEPHOURS},
            { (int)ServerPackets.SYNCWEATHER, ClientHandle.SYNCWEATHER},
            { (int)ServerPackets.REVIVE, ClientHandle.REVIVE},
            { (int)ServerPackets.REVIVEDONE, ClientHandle.REVIVEDONE},
            { (int)ServerPackets.ANIMALROLE, ClientHandle.ANIMALROLE},
            { (int)ServerPackets.DARKWALKERREADY, ClientHandle.DARKWALKERREADY},
            { (int)ServerPackets.HOSTISDARKWALKER, ClientHandle.HOSTISDARKWALKER},
            { (int)ServerPackets.WARDISACTIVE, ClientHandle.WARDISACTIVE},
            { (int)ServerPackets.DWCOUNTDOWN, ClientHandle.DWCOUNTDOWN},
            { (int)ServerPackets.ANIMALSYNCTRIGG, ClientHandle.ANIMALSYNCTRIGG},
            { (int)ServerPackets.SHOOTSYNC, ClientHandle.SHOOTSYNC},
            { (int)ServerPackets.PIMPSKILL, ClientHandle.PIMPSKILL},
            { (int)ServerPackets.HARVESTINGANIMAL, ClientHandle.HARVESTINGANIMAL},
            { (int)ServerPackets.SAVEDATA, ClientHandle.SAVEDATA},
            { (int)ServerPackets.BULLETDAMAGE, ClientHandle.BULLETDAMAGE},
            { (int)ServerPackets.MULTISOUND, ClientHandle.MULTISOUND},
            { (int)ServerPackets.CONTAINEROPEN, ClientHandle.CONTAINEROPEN},
            { (int)ServerPackets.LUREPLACEMENT, ClientHandle.LUREPLACEMENT},
            { (int)ServerPackets.LUREISACTIVE, ClientHandle.LUREISACTIVE},
            { (int)ServerPackets.ALIGNANIMAL, ClientHandle.ALIGNANIMAL},
            { (int)ServerPackets.ASKFORANIMALPROXY, ClientHandle.ASKFORANIMALPROXY},
            { (int)ServerPackets.CARRYBODY, ClientHandle.CARRYBODY},
            { (int)ServerPackets.BODYWARP, ClientHandle.BODYWARP},
            { (int)ServerPackets.ANIMALDELETE, ClientHandle.ANIMALDELETE},
            { (int)ServerPackets.KEEPITALIVE, ClientHandle.KEEPITALIVE},
            { (int)ServerPackets.RQRECONNECT, ClientHandle.RQRECONNECT},
            { (int)ServerPackets.EQUIPMENT, ClientHandle.EQUIPMENT},
            { (int)ServerPackets.CHAT, ClientHandle.CHAT},
            { (int)ServerPackets.PLAYERSSTATUS, ClientHandle.PLAYERSSTATUS},
            { (int)ServerPackets.CHANGENAME, ClientHandle.CHANGENAME},
            { (int)ServerPackets.CLOTH, ClientHandle.CLOTH},
            { (int)ServerPackets.ASKSPAWNDATA, ClientHandle.ASKSPAWNDATA},
            { (int)ServerPackets.LEVELGUID, ClientHandle.LEVELGUID},
            { (int)ServerPackets.FURNBROKEN, ClientHandle.FURNBROKEN},
            { (int)ServerPackets.FURNBROKENLIST, ClientHandle.FURNBROKENLIST},
            { (int)ServerPackets.FURNBREAKINGGUID, ClientHandle.FURNBREAKINGGUID},
            { (int)ServerPackets.FURNBREAKINSTOP, ClientHandle.FURNBREAKINSTOP},
            { (int)ServerPackets.GEARPICKUP, ClientHandle.GEARPICKUP},
            { (int)ServerPackets.GEARPICKUPLIST, ClientHandle.GEARPICKUPLIST},
            { (int)ServerPackets.ROPE, ClientHandle.ROPE},
            { (int)ServerPackets.ROPELIST, ClientHandle.ROPELIST},
            { (int)ServerPackets.CONSUME, ClientHandle.CONSUME},
            { (int)ServerPackets.SERVERCFG, ClientHandle.SERVERCFG},
            { (int)ServerPackets.STOPCONSUME, ClientHandle.STOPCONSUME},
            { (int)ServerPackets.HEAVYBREATH, ClientHandle.HEAVYBREATH},
            { (int)ServerPackets.BLOODLOSTS, ClientHandle.BLOODLOSTS},
            { (int)ServerPackets.APPLYACTIONONPLAYER, ClientHandle.APPLYACTIONONPLAYER},
            { (int)ServerPackets.DONTMOVEWARNING, ClientHandle.DONTMOVEWARNING},
            { (int)ServerPackets.INFECTIONSRISK, ClientHandle.INFECTIONSRISK},
            { (int)ServerPackets.CANCLEPICKUP, ClientHandle.CANCLEPICKUP},
            { (int)ServerPackets.CONTAINERINTERACT, ClientHandle.CONTAINERINTERACT},
            { (int)ServerPackets.LOOTEDCONTAINER, ClientHandle.LOOTEDCONTAINER},
            { (int)ServerPackets.LOOTEDCONTAINERLIST, ClientHandle.LOOTEDCONTAINERLIST},
            { (int)ServerPackets.HARVESTPLANT, ClientHandle.HARVESTPLANT},
            { (int)ServerPackets.LOOTEDHARVESTABLE, ClientHandle.LOOTEDHARVESTABLE},
            { (int)ServerPackets.LOOTEDHARVESTABLEALL, ClientHandle.LOOTEDHARVESTABLEALL},
            { (int)ServerPackets.SELECTEDCHARACTER, ClientHandle.SELECTEDCHARACTER},
            { (int)ServerPackets.ADDSHELTER, ClientHandle.ADDSHELTER},
            { (int)ServerPackets.REMOVESHELTER, ClientHandle.REMOVESHELTER},
            { (int)ServerPackets.ALLSHELTERS, ClientHandle.ALLSHELTERS},
            { (int)ServerPackets.USESHELTER, ClientHandle.USESHELTER},
            { (int)ServerPackets.FIRE, ClientHandle.FIRE},
            { (int)ServerPackets.CUSTOM, ClientHandle.CUSTOM},
            { (int)ServerPackets.KICKMESSAGE, ClientHandle.KICKMESSAGE},
            { (int)ServerPackets.GOTITEMSLICE, ClientHandle.GOTITEMSLICE},
            { (int)ServerPackets.VOICECHAT, ClientHandle.VOICECHAT},
            { (int)ServerPackets.SLICEDBYTES, ClientHandle.SLICEDBYTES},
            { (int)ServerPackets.ANIMALDAMAGE, ClientHandle.ANIMALDAMAGE},
            { (int)ServerPackets.FIREFUEL, ClientHandle.FIREFUEL},
            { (int)ServerPackets.DROPITEM, ClientHandle.DROPITEM},
            { (int)ServerPackets.PICKDROPPEDGEAR, ClientHandle.PICKDROPPEDGEAR},
            { (int)ServerPackets.GETREQUESTEDITEMSLICE, ClientHandle.GETREQUESTEDITEMSLICE},
            { (int)ServerPackets.GETREQUESTEDFORPLACESLICE, ClientHandle.GETREQUESTEDFORPLACESLICE},
            { (int)ServerPackets.GOTCONTAINERSLICE, ClientHandle.GOTCONTAINERSLICE},
            { (int)ServerPackets.OPENEMPTYCONTAINER, ClientHandle.OPENEMPTYCONTAINER},
            { (int)ServerPackets.MARKSEARCHEDCONTAINERS, ClientHandle.MARKSEARCHEDCONTAINERS},
            { (int)ServerPackets.READYSENDNEXTSLICE, ClientHandle.READYSENDNEXTSLICE},
            { (int)ServerPackets.CHANGEAIM, ClientHandle.CHANGEAIM},
            { (int)ServerPackets.LOADINGSCENEDROPSDONE, ClientHandle.LOADINGSCENEDROPSDONE},
            { (int)ServerPackets.GEARNOTEXIST, ClientHandle.GEARNOTEXIST},
            { (int)ServerPackets.USEOPENABLE, ClientHandle.USEOPENABLE},
            { (int)ServerPackets.TRYDIAGNISISPLAYER, ClientHandle.TRYDIAGNISISPLAYER},
            { (int)ServerPackets.SENDMYAFFLCTIONS, ClientHandle.SENDMYAFFLCTIONS},
            { (int)ServerPackets.CUREAFFLICTION, ClientHandle.CUREAFFLICTION},
            { (int)ServerPackets.ANIMALTEST, ClientHandle.ANIMALTEST},
            { (int)ServerPackets.ANIMALCORPSE, ClientHandle.ANIMALCORPSE},
            { (int)ServerPackets.REQUESTANIMALCORPSE, ClientHandle.REQUESTANIMALCORPSE},
            { (int)ServerPackets.QUARTERANIMAL, ClientHandle.QUARTERANIMAL},
            { (int)ServerPackets.ANIMALAUDIO, ClientHandle.ANIMALAUDIO},
            { (int)ServerPackets.GOTRABBIT, ClientHandle.GOTRABBIT},
            { (int)ServerPackets.RELEASERABBIT, ClientHandle.RELEASERABBIT},
            { (int)ServerPackets.HITRABBIT, ClientHandle.HITRABBIT},
            { (int)ServerPackets.RABBITREVIVED, ClientHandle.RABBITREVIVED},
            { (int)ServerPackets.MELEESTART, ClientHandle.MELEESTART},
            { (int)ServerPackets.TRYBORROWGEAR, ClientHandle.TRYBORROWGEAR},
            { (int)ServerPackets.CHALLENGEINIT, ClientHandle.CHALLENGEINIT},
            { (int)ServerPackets.CHALLENGEUPDATE, ClientHandle.CHALLENGEUPDATE},
            { (int)ServerPackets.CHALLENGETRIGGER, ClientHandle.CHALLENGETRIGGER},
            { (int)ServerPackets.ADDDEATHCONTAINER, ClientHandle.ADDDEATHCONTAINER},
            { (int)ServerPackets.DEATHCREATEEMPTYNOW, ClientHandle.DEATHCREATEEMPTYNOW},
            { (int)ServerPackets.SPAWNREGIONBANCHECK, ClientHandle.SPAWNREGIONBANCHECK},
            { (int)ServerPackets.CAIRNS, ClientHandle.CAIRNS},
            { (int)ServerPackets.BENEFITINIT, ClientHandle.BENEFITINIT},
            { (int)ServerPackets.MODSLIST, ClientHandle.MODSLIST},
            { (int)ServerPackets.ADDDOORLOCK, ClientHandle.ADDDOORLOCK},
            { (int)ServerPackets.DOORLOCKEDMSG, ClientHandle.DOORLOCKEDMSG},
            { (int)ServerPackets.ENTERDOOR, ClientHandle.ENTERDOOR},
            { (int)ServerPackets.REMOVEDOORLOCK, ClientHandle.REMOVEDOORLOCK},
            { (int)ServerPackets.LOCKPICK, ClientHandle.LOCKPICK},
            { (int)ServerPackets.VERIFYSAVE, ClientHandle.VERIFYSAVE},
            { (int)ServerPackets.RPC, ClientHandle.RPC},
            { (int)ServerPackets.REQUESTLOCKSMITH, ClientHandle.REQUESTLOCKSMITH},
            { (int)ServerPackets.KNOCKKNOCK, ClientHandle.KNOCKKNOCK},
            { (int)ServerPackets.KNOCKENTER, ClientHandle.KNOCKENTER},
            { (int)ServerPackets.PEEPHOLE, ClientHandle.PEEPHOLE},
            { (int)ServerPackets.RESTART, ClientHandle.RESTART},
            { (int)ServerPackets.READYSENDNEXTSLICEGEAR, ClientHandle.READYSENDNEXTSLICEGEAR},
            { (int)ServerPackets.DEDICATEDWEATHER, ClientHandle.DEDICATEDWEATHER},
            { (int)ServerPackets.WEATHERVOLUNTEER, ClientHandle.WEATHERVOLUNTEER},
            { (int)ServerPackets.REREGISTERWEATHER, ClientHandle.REREGISTERWEATHER},
            { (int)ServerPackets.REMOVEKEYBYSEED, ClientHandle.REMOVEKEYBYSEED},
            { (int)ServerPackets.ADDHUDMSG, ClientHandle.ADDHUDMSG},
            { (int)ServerPackets.CHANGECONTAINERSTATE, ClientHandle.CHANGECONTAINERSTATE},
            { (int)ServerPackets.FINISHEDSENDINGCONTAINER, ClientHandle.FINISHEDSENDINGCONTAINER},
            { (int)ServerPackets.TRIGGEREMOTE, ClientHandle.TRIGGEREMOTE},
            { (int)ServerPackets.EXPEDITIONSYNC, ClientHandle.EXPEDITIONSYNC},
            { (int)ServerPackets.EXPEDITIONRESULT, ClientHandle.EXPEDITIONRESULT},
            { (int)ServerPackets.READYSENDNEXTSLICEPHOTO, ClientHandle.READYSENDNEXTSLICEPHOTO},
            { (int)ServerPackets.REQUESTEXPEDITIONINVITES, ClientHandle.REQUESTEXPEDITIONINVITES},
            { (int)ServerPackets.NEWPLAYEREXPEDITION, ClientHandle.NEWPLAYEREXPEDITION},
            { (int)ServerPackets.NEWEXPEDITIONINVITE, ClientHandle.NEWEXPEDITIONINVITE},
            { (int)ServerPackets.BASE64SLICE, ClientHandle.BASE64SLICE},
            { (int)ServerPackets.ADDROCKCACH, ClientHandle.ADDROCKCACH},
            { (int)ServerPackets.REMOVEROCKCACH, ClientHandle.REMOVEROCKCACH},
            { (int)ServerPackets.ADDUNIVERSALSYNCABLE, ClientHandle.ADDUNIVERSALSYNCABLE},
            { (int)ServerPackets.REMOVEUNIVERSALSYNCABLE, ClientHandle.REMOVEUNIVERSALSYNCABLE},
            { (int)ServerPackets.CUSTOMSOUNDEVENT, ClientHandle.CUSTOMSOUNDEVENT},
            { (int)ServerPackets.REQUESTSPECIALEXPEDITION, ClientHandle.REQUESTSPECIALEXPEDITION},
            { (int)ServerPackets.REQUESTSPHOTOAGAIN, ClientHandle.REQUESTSPHOTOAGAIN},
        };
            MelonLogger.Msg("Initialized packets.");
        }
    }
}
