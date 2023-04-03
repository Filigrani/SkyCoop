using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer;

namespace SkyCoop
{
    public class API
    {
        public enum SkyCoopClientState
        {
            NONE = -1,
            CLIENT,
            HOST,
        }
        public static SkyCoopClientState m_ClientState = SkyCoopClientState.NONE; // -1 Not playing online, 0 Client, 1 Host.
        public static bool m_AllowMods = true; // Using mods allowed.
        public static int m_MyClientID = 0; // ID of player on server, (this related to slot what player use).
        public static bool m_Debug = false;
        public static bool IsP2PServer = false;
        public static int Port = 0;

        public static void CustomEventCallback(Packet _pak, int from)
        { 
            if (m_Debug)
            {
                MelonLoader.MelonLogger.Msg(ConsoleColor.Blue, "[SkyCoop API] ");
            }
        }

        //HOST
        public static void SendDataToEveryone(Packet _pak, int from, bool ignoreSender)
        {
            if(m_ClientState == SkyCoopClientState.HOST)
            {
                _pak.Write(from);

                if (ignoreSender)
                {
                    ServerSend.SendUDPDataToAllButNotSender(_pak, from);
                }else{
                    ServerSend.SendUDPDataToAll(_pak);
                }
            }
            else{
                if(m_ClientState == SkyCoopClientState.CLIENT)
                {
                    MelonLoader.MelonLogger.Msg(ConsoleColor.Red, "[SkyCoop API] Client trying call SendDataToEveryone, but this host only method.");
                }
            }
        }
        public static void SendDataToClient(Packet _pak, int from, int to)
        {
            if (m_ClientState == SkyCoopClientState.HOST)
            {
                _pak.Write(from);
                ServerSend.SendUDPData(to, _pak);
            }else{
                if (m_ClientState == SkyCoopClientState.CLIENT)
                {
                    MelonLoader.MelonLogger.Msg(ConsoleColor.Red, "[SkyCoop API] Client trying call SendDataToClient, but this host only method.");
                }
            }
        }
        //Client
        public static void SendToHost(Packet _pak)
        {
            if (m_ClientState == SkyCoopClientState.CLIENT)
            {
                MyMod.SendUDPData(_pak);
            }else{
                if (m_ClientState == SkyCoopClientState.HOST)
                {
                    MelonLoader.MelonLogger.Msg(ConsoleColor.Red, "[SkyCoop API] Host trying call SendToHost, but this client only method.");
                }
            }
        }
        public static void TestCustom()
        {
            using (Packet _packet = new Packet((int)ServerPackets.CUSTOM))
            {
                _packet.Write("MODNAME_EVENTNAME"); //Name of event. This to next time you would be able to recognize what event has been received. String type.
                _packet.Write("Hello this is test message"); //Data to send. Check out Write method's overloads there almost every basic types of data like bool, int, string, and etc.
                
                if(m_AllowMods)
                {
                    if(m_ClientState == SkyCoopClientState.HOST)
                    {
                        SendDataToEveryone(_packet, m_MyClientID, true); //Send data to everyone.
                    }else if(m_ClientState == SkyCoopClientState.CLIENT)
                    {
                        SendToHost(_packet);
                    }
                }
            }
        }
    }
}
