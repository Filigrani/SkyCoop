using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using SkyCoop;

namespace GameServer
{
    class Client
    {
        public static int dataBufferSize = 4096;

        public int id;
        public UDP udp;
        public int TimeOutTime = 0;
        public bool RCON = false;
        public string SubNetworkGUID = "";
        public bool Ready = false;

        public Client(int _clientId)
        {
            id = _clientId;
            udp = new UDP(id);
            udp.client = this;
        }
        public bool IsBusy()
        {
            if(udp.endPoint != null || udp.sid != "")
            {
                return true;
            }
            else{
                return false;
            }  
        }
        public static void Log(string LOG)
        {
#if (DEDICATED)
            Logger.Log(LOG, Shared.LoggerColor.Blue);
#else
            MelonLoader.MelonLogger.Msg(ConsoleColor.Blue, LOG);
#endif
        }

        public class UDP
        {
            public IPEndPoint endPoint;

            private int id;
            public string sid = "";
            public Client client;

            public UDP(int _id)
            {
                id = _id;
            }

            /// <summary>Initializes the newly connected client's UDP-related info.</summary>
            /// <param name="_endPoint">The IPEndPoint instance of the newly connected client.</param>
            public void Connect(IPEndPoint _endPoint)
            {
                Log($"Incoming connection from {_endPoint.Address}...");
                endPoint = _endPoint;
                ServerSend.Welcome(id, Server.MaxPlayers);
            }
            public void ConnectSteamWorks(string _sid)
            {
                sid = _sid;
                ServerSend.Welcome(id, Server.MaxPlayers);
            }

            /// <summary>Sends data to the client via UDP.</summary>
            /// <param name="_packet">The packet to send.</param>
            public void SendData(Packet _packet, bool IgnoreReady = true)
            {
                if (!IgnoreReady && !client.Ready)
                {
                    return;
                }
                
                if(Server.UsingSteamWorks == false)
                {
                    Server.SendUDPData(endPoint, _packet);
                }else{
                    Server.SendP2PData(sid, _packet);
                }
            }

            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="_packetData">The packet containing the recieved data.</param>
            public void HandleData(Packet _packetData)
            {
                int _packetLength = _packetData.ReadInt();
                byte[] _packetBytes = _packetData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        if (MyMod.DebugTrafficCheck == true)
                        {
                            Log("[DebugTrafficCheck] Got packet ID " + _packetId);
                            Log("[DebugTrafficCheck] Packet contains " + _packet.ReturnSize() + " bytes");
                        }
                        Server.packetHandlers[_packetId](id, _packet); // Call appropriate method to handle the packet
                    }
                });
            }

            /// <summary>Cleans up the UDP connection.</summary>
            public void Disconnect()
            {
                endPoint = null;
                sid = "";
                client.Ready = false;
            }
        }
    }
}