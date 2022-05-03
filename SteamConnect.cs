using System;
using UnityEngine;
using MelonLoader;
using UnhollowerBaseLib;
using Steamworks;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

using GameServer;
using System.Collections.Generic;

namespace SkyCoop
{
    class SteamConnect
    {
        public static bool CanUseSteam = false;
        public static string SteamName = "";
        public static bool PiningMaster = false;

        public static void Init()
        {
            MelonLogger.Msg("[SteamWorks.NET] Trying to Init SteamAPI");
            SteamAPI.Init();
            StartSteam();
            MelonLogger.Msg("[SteamWorks.NET] SteamAPI is initialized");
        }

        public static bool StartSteam()
        {
            CanUseSteam = true;
            Main.Run();
            return true;
        }

        public static void DoUpdate()
        {
            if (!CanUseSteam)
                return;
            Main.ListenData();
            SteamAPI.RunCallbacks();
        }
        public class Main
        {
            private static Callback<GameRichPresenceJoinRequested_t> _GameRichPresenceJoinRequested_t;
            private static Callback<GameLobbyJoinRequested_t> _GameLobbyJoinRequested_t;
            private static Callback<LobbyEnter_t> _LobbyEnter_t;
            private static CallResult<LobbyCreated_t> OnLobbyCreatedCallResult;
            private static Callback<LobbyGameCreated_t> _LobbyGameCreated_t;
            private static Callback<LobbyChatUpdate_t> _LobbyChatUpdate_t;
            public static void Run()
            {
                _GameRichPresenceJoinRequested_t = Callback<GameRichPresenceJoinRequested_t>.Create(new Callback<GameRichPresenceJoinRequested_t>.DispatchDelegate(OnP2PSessionAccept));
                _LobbyEnter_t = Callback<LobbyEnter_t>.Create(new Callback<LobbyEnter_t>.DispatchDelegate(OnEnterLobby));
                _GameLobbyJoinRequested_t = Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(ShouldJoinLobby));
                OnLobbyCreatedCallResult = CallResult<LobbyCreated_t>.Create(new CallResult<LobbyCreated_t>.APIDispatchDelegate(OnLobbyCreated));
                _LobbyGameCreated_t = Callback<LobbyGameCreated_t>.Create(new Callback<LobbyGameCreated_t>.DispatchDelegate(OnLobbyServerCreated));
                _LobbyChatUpdate_t = Callback<LobbyChatUpdate_t>.Create(new Callback<LobbyChatUpdate_t>.DispatchDelegate(OnLobbyPlayersUpdate));
                
                string personaName = SteamFriends.GetPersonaName();
                MelonLogger.Msg("[SteamWorks.NET] Logins as " + personaName + " SteamID " + SteamUser.GetSteamID().ToString());
                MyMod.LoadChatName(personaName);
            }

            public static void OnP2PSessionAccept(GameRichPresenceJoinRequested_t request)
            {
                if(MyMod.level_name == "MainMenu")
                {
                    MelonLogger.Msg("[SteamWorks.NET] Got accpeted invite message");
                    if (request.m_rgchConnect != null)
                    {
                        MelonLogger.Msg("[SteamWorks.NET] m_rgchConnect " + request.m_rgchConnect);
                        if (request.m_rgchConnect.Contains("+connect_lobby"))
                        {
                            JoinToLobby(request.m_rgchConnect.Replace("+connect_lobby ", ""));
                        }
                    }else{
                        if (request.m_steamIDFriend != null)
                        {
                            CSteamID clientId = request.m_steamIDFriend;
                            string IDstr = clientId.ToString();
                            MyMod.ConnectedSteamWorks = true;
                            MyMod.SteamServerWorks = IDstr;
                            MyMod.DoWaitForConnect();
                            ConnectToHost(MyMod.SteamServerWorks);
                        }
                    }
                }
            }
            public static void OnEnterLobby(LobbyEnter_t request)
            {
                if(request.m_EChatRoomEnterResponse == 1)
                {
                    MelonLogger.Msg("[SteamWorks.NET] Jointed to Lobby "+ request.m_ulSteamIDLobby);
                    MyMod.MyLobby = request.m_ulSteamIDLobby.ToString();
                    if (SteamMatchmaking.GetLobbyOwner(new CSteamID(request.m_ulSteamIDLobby)) != SteamUser.GetSteamID())
                    {
                        MelonLogger.Msg("[SteamWorks.NET] Going to connect to server of the lobby");
                        SteamFriends.SetRichPresence("connect", "+connect_lobby " + request.m_ulSteamIDLobby);
                        SteamFriends.SetRichPresence("steam_player_group", request.m_ulSteamIDLobby.ToString());
                        SteamFriends.SetRichPresence("steam_player_group_size", SteamMatchmaking.GetNumLobbyMembers(new CSteamID(request.m_ulSteamIDLobby)).ToString());

                        //ulong MySteamID = ulong.Parse(SteamUser.GetSteamID().ToString());

                        //if (!MyMod.LobbyContains(MySteamID))
                        //{
                        //    int Descripter = SteamFriends.GetMediumFriendAvatar(SteamUser.GetSteamID());
                        //    if (Descripter != 0)
                        //    {
                        //        Texture2D Avatar = GetImageFromDescripter(Descripter, 64, 64);
                        //        MyMod.AddPersonToLobby(SteamFriends.GetPersonaName(), MySteamID, Avatar);
                        //    }else{
                        //        MelonLogger.Msg(ConsoleColor.Yellow, "[SteamWorks.NET] Player has not avatar ");
                        //    }
                        //}
                        //MyMod.LobbyUI.SetActive(true);

                        uint _;
                        ushort __;
                        CSteamID server;
                        SteamMatchmaking.GetLobbyGameServer(new CSteamID(request.m_ulSteamIDLobby), out _, out __, out server);
                        if (server != null)
                        {
                            int InLobby = SteamMatchmaking.GetNumLobbyMembers(new CSteamID(request.m_ulSteamIDLobby));
                            int LobbyLimit = SteamMatchmaking.GetLobbyMemberLimit(new CSteamID(request.m_ulSteamIDLobby));
                            SteamFriends.SetRichPresence("status", "Playing Online " + InLobby + "/" + LobbyLimit);
                            MyMod.ConnectedSteamWorks = true;
                            MyMod.SteamServerWorks = server.ToString();
                            MyMod.DoWaitForConnect();
                            ConnectToHost(MyMod.SteamServerWorks);
                        }
                    }
                }else{
                    MelonLogger.Msg(ConsoleColor.Red, "[SteamWorks.NET] Can't joing to lobby!");
                }
            }
            public static void ShouldJoinLobby(GameLobbyJoinRequested_t request)
            {
                MelonLogger.Msg("[SteamWorks.NET] Trying to join to lobby from invite " + request.m_steamIDLobby);
                if (MyMod.SteamServerWorks == "" && MyMod.MyLobby == "" && MyMod.iAmHost == false && MyMod.sendMyPosition == false)
                {
                    SteamMatchmaking.JoinLobby(request.m_steamIDLobby);
                }else{
                    if (MyMod.m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
                    {
                        InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, "YOU ALREADY ON SERVER", "\n" + "You already on the server, restart the game if you want to join to another server", Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null, null);
                    }
                }
            }

            public static Texture2D GetImageFromDescripter(int AvatarDescripter, int width, int height)
            {
                int ImageSizeInBytes = width * height * 4; 
                byte[] Avatar = new byte[ImageSizeInBytes];
                SteamUtils.GetImageRGBA(AvatarDescripter, Avatar, ImageSizeInBytes);
                Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(Avatar);
                texture.Apply();
                return texture;
            }

            public static void OnLobbyCreated(LobbyCreated_t pCallback, bool bIOFailure)
            {
                if (!bIOFailure)
                {
                    if(pCallback.m_eResult == EResult.k_EResultOK)
                    {
                        MelonLogger.Msg("[SteamWorks.NET] Lobby created " + pCallback.m_ulSteamIDLobby);
                        SteamMatchmaking.SetLobbyGameServer(new CSteamID(pCallback.m_ulSteamIDLobby), 0, 0, SteamUser.GetSteamID());
                        SteamFriends.SetRichPresence("steam_player_group", pCallback.m_ulSteamIDLobby.ToString());
                        SteamFriends.SetRichPresence("steam_player_group_size", "1");
                        MyMod.MyLobby = pCallback.m_ulSteamIDLobby.ToString();
                    }else{
                        MelonLogger.Msg(ConsoleColor.Red, "[SteamWorks.NET] Can't create lobby: Error " + pCallback.m_eResult);
                        MelonLogger.Msg(ConsoleColor.Green, "[SteamWorks.NET] Going to try again in 5 seconds");
                        MyMod.TryMakeLobbyAgain = 5;
                    }
                }else{
                    MelonLogger.Msg(ConsoleColor.Red, "[SteamWorks.NET] Can't create lobby: Error bIOFailure");
                    MelonLogger.Msg(ConsoleColor.Green, "[SteamWorks.NET] Going to try again in 5 seconds");
                    MyMod.TryMakeLobbyAgain = 5;
                }

                //if (MyMod.LobbyUI)
                //{
                //    MyMod.LobbyUI.SetActive(true);

                //    ulong MySteamID = ulong.Parse(SteamUser.GetSteamID().ToString());

                //    if (!MyMod.LobbyContains(MySteamID))
                //    {
                //        int Descripter = SteamFriends.GetMediumFriendAvatar(SteamUser.GetSteamID());
                //        if (Descripter != 0)
                //        {
                //            Texture2D Avatar = GetImageFromDescripter(Descripter, 64, 64);
                //            MyMod.AddPersonToLobby(SteamFriends.GetPersonaName(), MySteamID, Avatar);
                //        }else{
                //            MelonLogger.Msg(ConsoleColor.Yellow, "[SteamWorks.NET] Player has not avatar ");
                //        }
                //    }
                //}
            }

            public static void OnLobbyServerCreated(LobbyGameCreated_t request)
            {
                MelonLogger.Msg("[SteamWorks.NET] Lobby server set to " + request.m_ulSteamIDGameServer);
                int InLobby = SteamMatchmaking.GetNumLobbyMembers(new CSteamID(request.m_ulSteamIDLobby));
                int LobbyLimit = SteamMatchmaking.GetLobbyMemberLimit(new CSteamID(request.m_ulSteamIDLobby));
                SteamFriends.SetRichPresence("status", "Playing Online "+ InLobby + "/"+ LobbyLimit);
                SteamFriends.SetRichPresence("connect", "+connect_lobby "+ request.m_ulSteamIDLobby);
                if (SteamMatchmaking.GetLobbyOwner(new CSteamID(request.m_ulSteamIDLobby)) != SteamUser.GetSteamID())
                {
                    MyMod.ConnectedSteamWorks = true;
                    MyMod.SteamServerWorks = request.m_ulSteamIDGameServer.ToString();
                    MyMod.DoWaitForConnect();
                    ConnectToHost(MyMod.SteamServerWorks);
                }
            }
            public static void OnLobbyPlayersUpdate(LobbyChatUpdate_t request)
            {
                if (request.m_rgfChatMemberStateChange == 0x0001) // Joined
                {
                    MelonLogger.Msg("[SteamWorks.NET] Played joined lobby " + request.m_ulSteamIDUserChanged);
                    if (SteamMatchmaking.GetLobbyOwner(new CSteamID(request.m_ulSteamIDLobby)) == SteamUser.GetSteamID())
                    {
                        SteamNetworking.AcceptP2PSessionWithUser(new CSteamID(request.m_ulSteamIDUserChanged));
                    }
                    //if (MyMod.LobbyUI)
                    //{
                    //    MyMod.LobbyUI.SetActive(true);
                    //}
                }else{
                    MelonLogger.Msg("[SteamWorks.NET] Player disconnected from lobby " + request.m_ulSteamIDUserChanged);
                    if (SteamMatchmaking.GetLobbyOwner(new CSteamID(request.m_ulSteamIDLobby)) == SteamUser.GetSteamID())
                    {
                        SteamNetworking.CloseP2PSessionWithUser(new CSteamID(request.m_ulSteamIDUserChanged));
                    }
                    //if (MyMod.LobbyUI)
                    //{
                    //    MyMod.LobbyUI.SetActive(true);
                    //    ulong PersonSteamID = request.m_ulSteamIDUserChanged;
                    //    MyMod.RemovePersonFromLobby(PersonSteamID);
                    //}
                }

                SteamFriends.SetRichPresence("steam_player_group", request.m_ulSteamIDLobby.ToString());
                SteamFriends.SetRichPresence("steam_player_group_size", SteamMatchmaking.GetNumLobbyMembers(new CSteamID(request.m_ulSteamIDLobby)).ToString());
                int InLobby = SteamMatchmaking.GetNumLobbyMembers(new CSteamID(request.m_ulSteamIDLobby));
                int LobbyLimit = SteamMatchmaking.GetLobbyMemberLimit(new CSteamID(request.m_ulSteamIDLobby));
                SteamFriends.SetRichPresence("status", "Playing Online " + InLobby + "/" + LobbyLimit);
            }

            public static void MayUpdatePlayerInLobby(ulong playerID)
            {
                if (!MyMod.LobbyContains(playerID))
                {
                    int Descripter = SteamFriends.GetMediumFriendAvatar(new CSteamID(playerID));
                    if (Descripter != 0)
                    {
                        Texture2D Avatar = GetImageFromDescripter(Descripter, 64, 64);
                        MyMod.AddPersonToLobby(SteamFriends.GetFriendPersonaName(new CSteamID(playerID)), playerID, Avatar);
                    }
                }else{
                    MyMod.SetPersonNameFromLobby(playerID, SteamFriends.GetFriendPersonaName(new CSteamID(playerID)));
                }
            }

            public static void OnRegularUpdate()
            {
                //if (MyMod.MyLobby != "")
                //{
                //    CSteamID lobbyID = new CSteamID(ulong.Parse(MyMod.MyLobby));
                //    int Peoples = SteamMatchmaking.GetNumLobbyMembers(lobbyID);

                //    for (int i = 0; i < Peoples; i++)
                //    {
                //        ulong playerID = ulong.Parse(SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i).ToString());
                //        MayUpdatePlayerInLobby(playerID);
                //    }
                //}
            }

            private static int LastLobbyType = 0;
            private static int LastLobbyLimit = 0;

            public static void MakeLobby(int type, int limit)
            {
                LastLobbyType = type;
                LastLobbyLimit = limit;

                ELobbyType LobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;

                switch (type)
                {
                    case 0:
                        LobbyType = ELobbyType.k_ELobbyTypePrivate;
                        break;
                    case 1:
                        LobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
                        break;
                    case 2:
                        LobbyType = ELobbyType.k_ELobbyTypePublic;
                        break;
                    default:
                        LobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
                        break;
                }

                MelonLogger.Msg("[SteamWorks.NET] Creating new lobby");
                SteamAPICall_t handle = SteamMatchmaking.CreateLobby(LobbyType, limit);
                OnLobbyCreatedCallResult.Set(handle);
            }
            public static void MakeLobby()
            {
                MelonLogger.Msg("[SteamWorks.NET] Retrying to creating new lobby");
                ELobbyType LobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;

                switch (LastLobbyType)
                {
                    case 0:
                        LobbyType = ELobbyType.k_ELobbyTypePrivate;
                        break;
                    case 1:
                        LobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
                        break;
                    case 2:
                        LobbyType = ELobbyType.k_ELobbyTypePublic;
                        break;
                    default:
                        LobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
                        break;
                }

                MelonLogger.Msg("[SteamWorks.NET] Creating new lobby");

                SteamAPICall_t handle = SteamMatchmaking.CreateLobby(LobbyType, LastLobbyLimit);
                OnLobbyCreatedCallResult.Set(handle);
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

            public static void TestLobbyUI()
            {

            }

            public static void MakeFriendListUI()
            {
                if(MyMod.MyLobby != "")
                {
                    SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID(ulong.Parse(MyMod.MyLobby)));
                }
            }
            public static void OpenFriends()
            {
                SteamFriends.ActivateGameOverlay("friends");
            }


            public static void ConnectToHost(string hostid)
            {
                MyMod.instance.myId = 0;
                CSteamID reciver = new CSteamID(ulong.Parse(hostid));
                MyMod.InitializeClientData();

                SteamNetworking.AcceptP2PSessionWithUser(reciver);
                MelonLogger.Msg("[SteamWorks.NET] Trying connecting to " + hostid);
                MyMod.DoSteamWorksConnect(SteamUser.GetSteamID().ToString());
            }
            public static void JoinToLobby(string hostid)
            {
                MyMod.instance.myId = 0;
                CSteamID reciver = new CSteamID(ulong.Parse(hostid));
                MyMod.InitializeClientData();

                SteamMatchmaking.JoinLobby(reciver);
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
                    byte[] _data = new byte[size];
                    uint bytesRead;

                    CSteamID remoteId;

                    if (SteamNetworking.ReadP2PPacket(_data, size, out bytesRead, out remoteId, 0))
                    {
                        if (_data.Length > 4)
                        {
                            HandleData(_data);
                        }
                    }
                }
            }
        }
    }
}
