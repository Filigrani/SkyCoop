using System;
using UnityEngine;
using MelonLoader;
using UnhollowerBaseLib;
using Steamworks;
using System.IO;
using System.Net;

using GameServer;

namespace SkyCoop
{
    class SteamConnect
    {
        public static bool CanUseSteam = false;
        public static string SteamName = "";
        public static bool PiningMaster = false;

        public static bool StartSteam()
        {
            if(SteamManager.Initialized == false)
            {
                MelonLogger.Msg(ConsoleColor.Red,"STEAM IN OFFLINE MODE! Please shutdown your game, and run steam/log in to steam freinds, to be able use STEAM P2P multiplayer.");
                return false;
            }
            CanUseSteam = true;
            Main.Run();
            return true;
        }
        public static void DoUpdate()
        {
            if (CanUseSteam == false)
            {
                return;
            }
            Main.ListenData();
        }

        public class Main : MelonMod
        {
            //private static Callback<P2PSessionRequest_t> _p2PSessionRequestCallback;
            //public delegate void P2PRQ_Delegate(P2PSessionRequest_t request);
            //public static P2PRQ_Delegate del = new P2PRQ_Delegate(OnP2PSessionRequest);


            public static void Run()
            {
                string name = SteamFriends.GetPersonaName();
                //_p2PSessionRequestCallback = Steamworks.Callback<P2PSessionRequest_t>.Create()

                //SteamNetworking.AllowP2PPacketRelay(true);
                //IntPtr point = del.Method.MethodHandle.GetFunctionPointer();
                //Il2CppSystem.Type _Type = new Il2CppSystem.Type(point);
                //Il2CppSystem.Object _Obj = new Il2CppSystem.Object(point);
                //Il2CppSystem.Reflection.MethodInfo _Method = new Il2CppSystem.Reflection.MethodInfo(point);
                //MelonLogger.Msg("[SteamWorks.NET] Creating Delecate...");
                //Callback<P2PSessionRequest_t>.DispatchDelegate del_ = new Callback<P2PSessionRequest_t>.DispatchDelegate(_Obj, point);
                //Callback<P2PSessionRequest_t>.Create(del_);
                MelonLogger.Msg("[SteamWorks.NET] Logins as " + name + " SteamID " + SteamUser.GetSteamID().ToString());
                MyMod.LoadChatName(name);
            }

            public static string GetFriends()
            {
                string str = "";

                int friends = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);

                for (int i = 0; i < friends; i++)
                {
                    CSteamID sid = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
                    str = str + i + ". " + SteamFriends.GetFriendPersonaName(sid) + "\n";
                }
                return str;
            }
            public static void ProcessWhitelist(string[] whitelist)
            {
                for (int i = 0; i < whitelist.Length; i++)
                {
                    ulong id = Convert.ToUInt64(whitelist[i]);
                    CSteamID sid = new CSteamID(id);
                    SteamNetworking.AcceptP2PSessionWithUser(sid);
                    MelonLogger.Msg(ConsoleColor.Blue, "[Dedicated server] Adding user with SID "+ id+" to the whitelist!");
                }
            }

            public static void ClickInvitePerson(CSteamID sid)
            {
                InviteFriendBySid(sid);
                UnityEngine.Object.Destroy(MyMod.UISteamFreindsMenuObj);
            }
            public static void ClickCloseFriendList()
            {
                UnityEngine.Object.Destroy(MyMod.UISteamFreindsMenuObj);
            }

            public static void MakeFriendListUI()
            {      
                if(MyMod.UiCanvas != null && MyMod.UISteamFreindsMenuObj == null)
                {
                    MyMod.UISteamFreindsMenuObj = MyMod.MakeModObject("MP_FriendList", MyMod.UiCanvas.transform);
                    int friends = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);

                    Transform ButtonsParent = MyMod.UISteamFreindsMenuObj.transform.GetChild(0).GetChild(0).gameObject.transform;
                    GameObject CloseButton = MyMod.UISteamFreindsMenuObj.transform.GetChild(3).gameObject;
                    Action actBack = new Action(() => ClickCloseFriendList());
                    CloseButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(actBack);

                    for (int i = 0; i < friends; i++)
                    {
                        CSteamID sid = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
                        string fName = SteamFriends.GetFriendPersonaName(sid);
                        GameObject newButton = MyMod.MakeModObject("MP_FriendButtonInvite", ButtonsParent);
                        newButton.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = fName;

                        Action act = new Action(() => ClickInvitePerson(sid));
                        newButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);
                    }
                }
            }

            public static string InviteFriendByIndex(int index)
            {
                int friends = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);

                if(index < 0 || index > friends-1)
                {
                    return "Invalid friend index!";
                }else{
                    CSteamID sid = SteamFriends.GetFriendByIndex(index, EFriendFlags.k_EFriendFlagImmediate);
                    InviteFriendBySid(sid);
                    return SteamFriends.GetFriendPersonaName(sid) + " invited to your server!";
                }
                return "Error";
            }

            public static void InviteFriendBySid(CSteamID reciver)
            {
                MelonLogger.Msg("[SteamWorks.NET] Inviting friend " + SteamFriends.GetFriendPersonaName(reciver));
                SteamFriends.InviteUserToGame(reciver, "join " + SteamUser.GetSteamID().ToString());
                SteamNetworking.AcceptP2PSessionWithUser(reciver);
            }
            public static void ConnectToHost(string hostid)
            {
                MyMod.instance.myId = 0;
                CSteamID reciver = new CSteamID(ulong.Parse(hostid));
                MyMod.InitializeClientData();

                SteamNetworking.AcceptP2PSessionWithUser(reciver);
                MelonLogger.Msg("[SteamWorks.NET]Trying connecting to " + hostid);
                MyMod.DoSteamWorksConnect(SteamUser.GetSteamID().ToString());
            }
            public static void SendUDPData(Packet _packet, CSteamID receiver)
            {
                //MelonLogger.Msg("[SteamWorks.NET] Sending packet to "+ receiver.ToString());
                byte[] _data = _packet.ToArray();
                Il2CppStructArray<byte> _dataCpp = _data;

                bool Result = SteamNetworking.SendP2PPacket(receiver, _dataCpp, (uint)_packet.Length(), EP2PSend.k_EP2PSendReliable, 0);

                //MelonLogger.Msg("[SteamWorks.NET] Sending " + Result);
            }
            public static void SendUDPData(Packet _packet, string receiver)
            {
                CSteamID sid = new Steamworks.CSteamID(ulong.Parse(receiver));

                SendUDPData(_packet, sid);
            }

            public static void Detail_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
            {
                PiningMaster = false;
                if (e.Error != null)
                {
                    //MelonLogger.Msg(ConsoleColor.Blue, "[Master server] Error " + e.Error.Message);
                    return;
                }
                if(e.Result != "")
                {
                    string[] newClinets = e.Result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    for (int i = 0; i < newClinets.Length; i++)
                    {
                        CSteamID sid = new CSteamID(ulong.Parse(newClinets[i]));
                        MyMod.MultiplayerChatMessage msg = new MyMod.MultiplayerChatMessage();
                        msg.m_Type = 0;
                        msg.m_By = MyMod.MyChatName;
                        msg.m_Message = "Someone connecting from master server";
                        MyMod.SendMessageToChat(msg, true);
                        SteamNetworking.AcceptP2PSessionWithUser(sid);
                    }
                }
                //MelonLogger.Msg(ConsoleColor.Blue, "[Master server] Responce " + e.Result);
            }
            public static void PingMasterServer()
            {
                if(PiningMaster == true || MyMod.PlayersOnServer >= Server.MaxPlayers)
                {
                    //MelonLogger.Msg("PiningMaster "+ PiningMaster+ " PlayersOnServer" + MyMod.PlayersOnServer+ " Server.MaxPlayers "+ Server.MaxPlayers);
                    return;
                }
                PiningMaster = true;
                WebClient web = new WebClient();
                string url = "http://168.119.36.188:35131/app/servers/SkyCoopServer.php";
                string sid = SteamUser.GetSteamID().ToString();
                string userName = SteamFriends.GetPersonaName();
                int slots = Server.MaxPlayers;
                int players = MyMod.PlayersOnServer;
                int xpmode = 1;
                int currxp = (int)ExperienceModeManager.s_CurrentModeType;
                    
                if (currxp == 0) // Pilgrim
                {
                    xpmode = 2;
                }else if(currxp == 1){ // Voyageur
                    xpmode = 1;
                }
                else if (currxp == 2){ // Stalker
                    xpmode = 3;
                }
                else if (currxp == 9){ // Interloper
                    xpmode = 4;
                }

                if(MyMod.CustomServerName != "")
                {
                    userName = MyMod.CustomServerName;
                }

                string data = "?sid="+sid+"&name="+userName+"&players="+players+"&slots="+slots+"&xpmode="+xpmode;

                if(MyMod.HadEverPingedMaster == false)
                {
                    data = data + "&firststart=1";
                    MyMod.HadEverPingedMaster = true;
                }

                url = url + data;
                Uri uri = new Uri(url);
                //MelonLogger.Msg(ConsoleColor.Blue, "[Master server] Pinging...");
                web.DownloadStringCompleted += new DownloadStringCompletedEventHandler(Detail_DownloadStringCompleted);
                web.DownloadStringAsync(uri);
            }

            public static void OnP2PSessionRequest(P2PSessionRequest_t request)
            {
                if (CanUseSteam == false)
                {
                    return;
                }
                CSteamID clientId = request.m_steamIDRemote;

                MelonLogger.Msg("[SteamWorks.NET] Client " + clientId.ToString() + " sent to you session invite");
            }
            public static void OnP2PSessionRequest_Fail(P2PSessionConnectFail_t request)
            {
                MelonLogger.Msg("[SteamWorks.NET] Request failed ");
            }

            public static void Disconnect(string hostid)
            {
                CSteamID reciver = new CSteamID(ulong.Parse(hostid));
                SteamNetworking.CloseP2PSessionWithUser(reciver);
                MelonLogger.Msg("[SteamWorks.NET] Disconnected!");
            }

            public static void HandleData(byte[] _data)
            {
                if(MyMod.iAmHost == true)
                {
                    int _clientID = 0;
                    using (Packet _packet = new Packet(_data))
                    {
                        _clientID = _packet.ReadInt();
                        int _packetLength = _packet.ReadInt();
                        _data = _packet.ReadBytes(_packetLength);
                    }
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_data))
                        {
                            int _packetId = _packet.ReadInt();
                            MyMod.SimUDPHandle(_packetId, _packet, _clientID);
                        }
                    });
                }else{
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
                            MyMod.SimUDPHandle_Client(_packetId, _packet);
                        }
                    });
                }
            }
            public static void ListenData()
            {
                if (CanUseSteam == false)
                {
                    return;
                }
                uint size;
                while (SteamNetworking.IsP2PPacketAvailable(out size))
                {
                    Il2CppStructArray<byte> _data = new byte[size];
                    uint bytesRead;

                    CSteamID remoteId;

                    if (SteamNetworking.ReadP2PPacket(_data, size, out bytesRead, out remoteId, 0))
                    {
                        //MelonLogger.Msg("[SteamWorks.NET] Got data packet from " + remoteId.ToString());
                        //MelonLogger.Msg("[SteamWorks.NET] _data.Length " + _data.Length);
                        //string datString = "";

                        //for (int i = 0; i < _data.Length; i++)
                        //{
                        //    datString = datString + _data[i].ToString();
                        //}

                        //MelonLogger.Msg("Bytes: " + datString);

                        if (_data.Length > 4)
                        {
                            //MelonLogger.Msg("[SteamWorks.NET] Starting Handle...");
                            HandleData(_data);
                        }
                        else{
                            //MelonLogger.Msg("[SteamWorks.NET] _data is null");
                        }
                    }
                }
            }
        }
    }
}
