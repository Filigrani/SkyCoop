using System;
using UnityEngine;
using MelonLoader;
using UnhollowerBaseLib;
using Steamworks;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime;
using GameServer;
using System.Collections.Generic;

namespace SkyCoop
{
    class SteamConnect
    {
        public static bool CanUseSteam = false;
        public static string SteamName = "";
        public static bool PiningMaster = false;
        public static bool IsMyLobby = false;

        public static void Init()
        {
            try
            {
                if (SteamAPI.RestartAppIfNecessary(new AppId_t(305620)))
                {
                    Application.Quit();
                    return;
                }
            }
            catch (DllNotFoundException e)
            {
                MelonLogger.Msg("[Steamworks.NET] Could not load [lib]steam_api32.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e);

                Application.Quit();
                return;
            }
            if (!Packsize.Test())
            {
                MelonLogger.Msg("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
            }
            if (!DllCheck.Test())
            {
                MelonLogger.Msg("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
            }
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
            private static CallResult<LobbyMatchList_t> OnLobbyMatchListCallResult;
            
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
                OnLobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create(new CallResult<LobbyMatchList_t>.APIDispatchDelegate(OnLobbyMatchList));
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
            public static void ProcessLobbyUi()
            {
                MyMod.m_Panel_Sandbox.Enable(true);
                MyMod.m_Panel_Sandbox.m_TitleLocalizationId = "MULTIPLAYER";
                MyMod.m_Panel_Sandbox.m_BasicMenu.UpdateTitle("MULTIPLAYER");
                MenuChange.ChangeMenuItems("Lobby");
            }


            public static void OnEnterLobby(LobbyEnter_t request)
            {
                MyMod.RemovePleaseWait();
                if(request.m_EChatRoomEnterResponse == 1)
                {
                    Transform Align = MyMod.m_Panel_Sandbox.gameObject.transform.GetChild(0).GetChild(0).GetChild(5);
                    Align.GetChild(1).gameObject.SetActive(true); //SelectIcon
                    Align.GetChild(2).gameObject.SetActive(true); //Grid
                    Align.GetChild(4).gameObject.SetActive(true); //Description
                    Align.GetChild(5).gameObject.SetActive(true); //Linebreaker
                    MyMod.ServerBrowser.SetActive(false);
                    MelonLogger.Msg("[SteamWorks.NET] Jointed to Lobby "+ request.m_ulSteamIDLobby);
                    MyMod.MyLobby = request.m_ulSteamIDLobby.ToString();
                    if (SteamMatchmaking.GetLobbyOwner(new CSteamID(request.m_ulSteamIDLobby)) != SteamUser.GetSteamID())
                    {
                        SteamFriends.SetRichPresence("steam_player_group", request.m_ulSteamIDLobby.ToString());
                        SteamFriends.SetRichPresence("steam_player_group_size", SteamMatchmaking.GetNumLobbyMembers(new CSteamID(request.m_ulSteamIDLobby)).ToString());

                        ulong MySteamID = ulong.Parse(SteamUser.GetSteamID().ToString());

                        if (!MyMod.LobbyContains(MySteamID))
                        {
                            int Descripter = SteamFriends.GetMediumFriendAvatar(SteamUser.GetSteamID());
                            if (Descripter != 0)
                            {
                                Texture2D Avatar = GetImageFromDescripter(Descripter, 64, 64);
                                MyMod.AddPersonToLobby(SteamFriends.GetPersonaName(), MySteamID, Avatar);
                            }else{
                                MelonLogger.Msg(ConsoleColor.Yellow, "[SteamWorks.NET] Player has not avatar ");
                            }
                        }

                        int InLobby = SteamMatchmaking.GetNumLobbyMembers(new CSteamID(request.m_ulSteamIDLobby));
                        int LobbyLimit = SteamMatchmaking.GetLobbyMemberLimit(new CSteamID(request.m_ulSteamIDLobby));
                        SteamFriends.SetRichPresence("status", "Playing Online " + InLobby + "/" + LobbyLimit);

                        uint _;
                        ushort __;
                        CSteamID server;
                        if(SteamMatchmaking.GetLobbyGameServer(new CSteamID(request.m_ulSteamIDLobby), out _, out __, out server) == true)
                        {
                            if (server != null)
                            {
                                MelonLogger.Msg("[SteamWorks.NET] Going to connect to server of the lobby");
                                SteamFriends.SetRichPresence("connect", "+connect_lobby " + request.m_ulSteamIDLobby);
                                MyMod.ConnectedSteamWorks = true;
                                MyMod.SteamServerWorks = server.ToString();
                                MyMod.RemovePleaseWait();
                                MyMod.DoWaitForConnect();
                                ConnectToHost(MyMod.SteamServerWorks);
                                MyMod.LobbyUI.SetActive(false);
                            }
                        }else{
                            MyMod.RemovePleaseWait();
                            MelonLogger.Msg("[SteamWorks.NET] Server isn't started yet, processing lobby");
                            ProcessLobbyUi();
                        }
                    }
                }else{
                    MelonLogger.Msg(ConsoleColor.Red, "[SteamWorks.NET] Can't joing to lobby!");
                    MyMod.RemovePleaseWait();
                    if (MyMod.m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
                    {
                        InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, "Can't join this lobby", "\n" + "Server is no more available to join", Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null, null);
                    }
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
                        SteamFriends.SetRichPresence("steam_player_group", pCallback.m_ulSteamIDLobby.ToString());
                        SteamFriends.SetRichPresence("steam_player_group_size", "1");
                        MyMod.MyLobby = pCallback.m_ulSteamIDLobby.ToString();
                        IsMyLobby = true;
                        SetSpawnStyle();
                        SetLobbyName();
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

                if (MyMod.LobbyUI)
                {
                    ulong MySteamID = ulong.Parse(SteamUser.GetSteamID().ToString());

                    if (!MyMod.LobbyContains(MySteamID))
                    {
                        int Descripter = SteamFriends.GetMediumFriendAvatar(SteamUser.GetSteamID());
                        if (Descripter != 0)
                        {
                            Texture2D Avatar = GetImageFromDescripter(Descripter, 64, 64);
                            MyMod.AddPersonToLobby(SteamFriends.GetPersonaName(), MySteamID, Avatar);
                        }else{
                            MelonLogger.Msg(ConsoleColor.Yellow, "[SteamWorks.NET] Player has not avatar ");
                        }
                    }
                }
            }

            public static string GetStateString(string State)
            {
                if(State == "StartingGame")
                {
                    return "Starting game";
                }else if(State == "SelectedSave")
                {
                    return "Going to load save";
                }
                else if (State == "SelectedNewSave")
                {
                    return "Going to create new save";
                }

                return State;
            }

            public static List<CSteamID> FoundServers = new List<CSteamID>();
            public static Dictionary<CSteamID, SteamLobbyElement> ServersData = new Dictionary<CSteamID, SteamLobbyElement>();
            public static List<GameObject> ServerObjects = new List<GameObject>();
            public class SteamLobbyElement
            {
                public CSteamID m_LobbyID;
                public CSteamID m_Owner;
                public string m_OwnerName;
                public int m_Players = 0;
                public int m_PlayersMax = 0;
                public string m_State = "";
            }

            public static void ClickJoin(CSteamID LobbyID)
            {
                MyMod.DoPleaseWait("Connecting","Trying to join...");
                if (MyMod.SteamServerWorks == "" && MyMod.MyLobby == "" && MyMod.iAmHost == false && MyMod.sendMyPosition == false)
                {
                    SteamMatchmaking.JoinLobby(LobbyID);
                }
            }

            public static void AddServerToList(SteamLobbyElement Data)
            {
                GameObject LoadedAssetsElement = MyMod.LoadedBundle.LoadAsset<GameObject>("MP_Server");
                GameObject Element = GameObject.Instantiate(LoadedAssetsElement, MyMod.ServerBrowser.transform.GetChild(1).GetChild(0).GetChild(0));

                UnityEngine.UI.Button Button = Element.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Button>();
                UnityEngine.UI.Text Name = Element.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>();
                UnityEngine.UI.Text Players = Element.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>();
                UnityEngine.UI.Text Status = Element.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Text>();

                Name.text = Data.m_OwnerName;
                Players.text = "Players: " + Data.m_Players + "/" + Data.m_PlayersMax;
                Status.text = "Status: " + Data.m_State;
                ServerObjects.Add(Element);

                Action actJoin = new Action(() => ClickJoin(Data.m_LobbyID));

                Button.onClick.AddListener(actJoin);    
            }


            public static void OnLobbyMatchList(LobbyMatchList_t pCallback, bool bIOFailure)
            {
                if (!bIOFailure)
                {
                    MelonLogger.Msg("[SteamWorks.NET] " + pCallback.m_nLobbiesMatching+" games found");
                    FoundServers = new List<CSteamID>();
                    ServersData = new Dictionary<CSteamID, SteamLobbyElement>();


                    if(ServerObjects.Count > 0)
                    {
                        for (int i = ServerObjects.Count; i > 0; i--)
                        {
                            GameObject obj = ServerObjects[i];
                            ServerObjects.RemoveAt(i);
                            UnityEngine.Object.Destroy(obj);
                        }
                    }

                    if(pCallback.m_nLobbiesMatching > 0)
                    {
                        for (int i = 0; i < pCallback.m_nLobbiesMatching; i++)
                        {
                            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
                            FoundServers.Add(lobbyID);
                            SteamMatchmaking.RequestLobbyData(lobbyID);
                        }

                        foreach (CSteamID server in FoundServers)
                        {
                            SteamLobbyElement ServerData = new SteamLobbyElement();
                            ServerData.m_LobbyID = server;
                            ServerData.m_Owner = SteamMatchmaking.GetLobbyOwner(server);
                            ServerData.m_Players = SteamMatchmaking.GetNumLobbyMembers(server);
                            ServerData.m_PlayersMax = SteamMatchmaking.GetLobbyMemberLimit(server);
                            ServerData.m_State = SteamMatchmaking.GetLobbyData(server, "State");
                            ServerData.m_OwnerName = SteamMatchmaking.GetLobbyData(server, "Name");

                            AddServerToList(ServerData);
                        }
                    }
                }
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
                    MenuChange.ChangeMenuItems("Original");
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
                }else{
                    MelonLogger.Msg("[SteamWorks.NET] Player disconnected from lobby " + request.m_ulSteamIDUserChanged);
                    if (SteamMatchmaking.GetLobbyOwner(new CSteamID(request.m_ulSteamIDLobby)) == SteamUser.GetSteamID())
                    {
                        SteamNetworking.CloseP2PSessionWithUser(new CSteamID(request.m_ulSteamIDUserChanged));
                    }
                    if (MyMod.LobbyUI)
                    {
                        ulong PersonSteamID = request.m_ulSteamIDUserChanged;
                        MyMod.RemovePersonFromLobby(PersonSteamID);
                    }
                }

                SteamFriends.SetRichPresence("steam_player_group", request.m_ulSteamIDLobby.ToString());
                SteamFriends.SetRichPresence("steam_player_group_size", SteamMatchmaking.GetNumLobbyMembers(new CSteamID(request.m_ulSteamIDLobby)).ToString());
                int InLobby = SteamMatchmaking.GetNumLobbyMembers(new CSteamID(request.m_ulSteamIDLobby));
                int LobbyLimit = SteamMatchmaking.GetLobbyMemberLimit(new CSteamID(request.m_ulSteamIDLobby));
                SteamFriends.SetRichPresence("status", "Playing Online " + InLobby + "/" + LobbyLimit);
            }
            public static bool VoiceDebug = false;
            public static bool VoiceRadioDebug = false;
            public static void TestMyVoice(byte[] DestBuffer, uint BytesWritten, float RecordTime)
            {
                MyMod.ProcessVoiceChatData(0, DestBuffer, BytesWritten, RecordTime, VoiceRadioDebug);
            }

            public static byte[] DecompressVoice(byte[] DestBuffer, uint BytesWritten)
            {
                byte[] DestBuffer2 = new byte[22050 * 2];
                uint BytesWritten2;
                EVoiceResult ret = SteamUser.DecompressVoice(DestBuffer, BytesWritten, DestBuffer2, (uint)DestBuffer2.Length, out BytesWritten2, 22050);
                if (ret == EVoiceResult.k_EVoiceResultOK && BytesWritten2 > 0)
                {
                    return DestBuffer2;
                }
                return null;
            }

            public static int StartRecordTime = 0;

            public static void VoiceUpdate()
            {
                if (MyMod.DoingRecord || MyMod.RecordReleseButtonHold > Time.time)
                {
                    SteamUser.StartVoiceRecording();
                }else{
                    SteamUser.StopVoiceRecording();
                }

                if (MyMod.DoingRecord || MyMod.RecordReleseButtonHold > Time.time)
                {
                    uint Compressed;
                    EVoiceResult ret = SteamUser.GetAvailableVoice(out Compressed);
                    if (ret == EVoiceResult.k_EVoiceResultOK && Compressed > 1024)
                    {
                        byte[] DestBuffer = new byte[1024];
                        uint BytesWritten;
                        ret = SteamUser.GetVoice(true, DestBuffer, 1024, out BytesWritten);
                        if (ret == EVoiceResult.k_EVoiceResultOK && BytesWritten > 0)
                        {
                            float Start = MyMod.RecordStartTime;
                            float Now = Time.time;
                            MyMod.RecordStartTime = Now;
                            float BeenRecording = Now - Start;

                            if (VoiceDebug == true)
                            {
                                MelonLogger.Msg("DestBuffer[" + BytesWritten + "]");
                                MelonLogger.Msg("Been recoding "+ BeenRecording);
                                TestMyVoice(DestBuffer, BytesWritten, BeenRecording);
                            }else{
                                MyMod.SendMyVoice(DestBuffer, int.Parse(BytesWritten.ToString()), BeenRecording);
                            }
                        }
                    }
                }
            }

            public static void ClearVotesObjects()
            {
                if (MyMod.LobbyRegion)
                {
                    Transform t = MyMod.LobbyRegion.transform.GetChild(0).GetChild(0).GetChild(0);
                    for (int i = 0; i < t.childCount; i++)
                    {
                        t.GetChild(i).gameObject.SetActive(false);
                    }
                }
                if (MyMod.LobbyExperience)
                {
                    Transform t = MyMod.LobbyExperience.transform.GetChild(0).GetChild(0).GetChild(0);
                    for (int i = 0; i < t.childCount; i++)
                    {
                        t.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            public static void UpdateVoteObjects(List<int> Regions, List<int> ExpModes)
            {
                int NextRegionObj = 0;
                int NextExpObj = 0;
                for (int i = 0; i < Regions.Count; i++)
                {
                    if(Regions[i] > 0)
                    {
                        GameObject Element = MyMod.LobbyRegion.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(NextRegionObj).gameObject;
                        string RegionName;
                        if ((GameRegion)i == GameRegion.RandomRegion)
                        {
                            RegionName = Localization.Get("GAMEPLAY_RandomRegion");
                        }else{
                            RegionName = Utils.GetLocalizedRegion((GameRegion)i);
                        }
                        Element.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = RegionName+ ": " + Regions[i];
                        Element.SetActive(true);
                        NextRegionObj++;
                    }
                }
                for (int i = 0; i < ExpModes.Count; i++)
                {
                    if (ExpModes[i] > 0)
                    {
                        GameObject Element = MyMod.LobbyExperience.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(NextExpObj).gameObject;
                        Element.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = Utils.GetLocalizedExperienceMode((ExperienceModeType)i) + ": " + ExpModes[i];
                        Element.SetActive(true);
                        NextExpObj++;
                    }
                }
            }

            public static void ProcessVotePick(List<int> Regions, List<int> ExpModes)
            {
                int MaxVotesRegions = -1;
                int BestRegion = 0;
                for (int i = 0; i < Regions.Count; i++)
                {
                    if (Regions[i] > MaxVotesRegions)
                    {
                        BestRegion = i;
                        MaxVotesRegions = Regions[i];
                    }
                }
                int MaxVotesExp = -1;
                int BestExp = 0;
                for (int i = 0; i < ExpModes.Count; i++)
                {
                    if (ExpModes[i] > MaxVotesExp)
                    {
                        BestExp = i;
                        MaxVotesExp = ExpModes[i];
                    }
                }
                MyMod.LobbyStartingRegion = BestRegion;
                MyMod.LobbyStartingExperience = BestExp;

                if(MyMod.ServerConfig.m_PlayersSpawnType == 2)
                {
                    MyMod.LobbyStartingRegion = (int)GameRegion.RandomRegion;
                }else if(MyMod.ServerConfig.m_PlayersSpawnType == 1)
                {
                    MyMod.LobbyStartingRegion = (int)GameRegion.RandomRegion;
                }

                SetNewGameSettings(MyMod.LobbyStartingRegion, MyMod.LobbyStartingExperience);
                OnSaveSlotChanged();
            }

            public static void OnLobbyStateChanged()
            {
                if (MyMod.LobbyUI.activeSelf)// If lobby ui active
                {
                    if (MenuChange.MenuMode == "Lobby")
                    {
                        MenuChange.ChangeMenuItems("Lobby");
                    }
                }
                MelonLogger.Msg("LobbyState " + MyMod.LobbyState);
            }
            public static void ProcessVoteUpdate(CSteamID lobbyID, List<int> Regions, List<int> ExpModes)
            {
                if (!IsMyLobby)
                {
                    string TimeLeft = SteamMatchmaking.GetLobbyData(lobbyID, "VoteTime");
                    if (TimeLeft != null && TimeLeft != "")
                    {
                        MyMod.LobbyVoteLeft = int.Parse(TimeLeft);
                    }
                }else{
                    MyMod.LobbyVoteLeft--;
                    SteamMatchmaking.SetLobbyData(new CSteamID(ulong.Parse(MyMod.MyLobby)), "VoteTime", MyMod.LobbyVoteLeft.ToString());
                }

                if (MyMod.DefaultIsRussian)
                {
                    MyMod.StartGOLOSOVANIE(); // Play flex cs 1.6 music
                    MyMod.LobbyUI.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "GOLOSOVANIE: " + MyMod.LobbyVoteLeft;
                }else{
                    MyMod.LobbyUI.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "VOTING: " + MyMod.LobbyVoteLeft;
                }

                if (MyMod.LobbyVoteLeft <= 0)
                {
                    SetLobbyState("SelectedNewSave");
                    if (MyMod.Golosovanie)
                    {
                        UnityEngine.Object.Destroy(MyMod.Golosovanie);
                    }
                    if (IsMyLobby)
                    {
                        ProcessVotePick(Regions, ExpModes);
                    }
                }
            }
            public static void OnSaveSlotChanged()
            {
                if (MyMod.LobbyUI.activeSelf)// If lobby ui active
                {
                    if (MenuChange.MenuMode == "Lobby")
                    {
                        MenuChange.ChangeMenuItems("Lobby");
                    }
                }
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
                if (MyMod.MyLobby != "")
                {
                    CSteamID lobbyID = new CSteamID(ulong.Parse(MyMod.MyLobby));
                    int Peoples = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
                    int Limit = SteamMatchmaking.GetLobbyMemberLimit(lobbyID);
                    if (!IsMyLobby)
                    {

                        string RegIDState = SteamMatchmaking.GetLobbyData(lobbyID, "RegionID");
                        if(RegIDState != null && RegIDState != "")
                        {
                            int NewVal = int.Parse(RegIDState);

                            if(NewVal != MyMod.LobbyStartingRegion)
                            {
                                MyMod.LobbyStartingRegion = NewVal;
                                OnSaveSlotChanged();
                            }
                        }

                        string ExpIDState = SteamMatchmaking.GetLobbyData(lobbyID, "ExpID");
                        if (ExpIDState != null && ExpIDState != "")
                        {
                            int NewVal = int.Parse(ExpIDState);

                            if(NewVal != MyMod.LobbyStartingExperience)
                            {
                                MyMod.LobbyStartingExperience = NewVal;
                                OnSaveSlotChanged();
                            }
                        }

                        string LobbyState = SteamMatchmaking.GetLobbyData(lobbyID, "State");
                        string Previous = MyMod.LobbyState;
                        if (LobbyState != null && LobbyState != "")
                        {
                            SetLobbyState(LobbyState);
                        }
                        string SpawnStyle = SteamMatchmaking.GetLobbyData(lobbyID, "SpawnStyle");
                        if(SpawnStyle != null && SpawnStyle != "")
                        {
                            MyMod.ServerConfig.m_PlayersSpawnType = int.Parse(SpawnStyle);
                        }else{
                            MyMod.ServerConfig.m_PlayersSpawnType = 0;
                        }

                        string SeedVal = SteamMatchmaking.GetLobbyData(lobbyID, "SaveSeed");
                        if (SeedVal != null && SeedVal != "")
                        {
                            int NewSeed = int.Parse(SeedVal);
                            if (MyMod.SelectedSaveSeed != NewSeed)
                            {
                                MyMod.SelectedSaveSeed = NewSeed;
                                OnSaveSlotChanged();
                            }
                        }else{
                            
                            if(MyMod.SelectedSaveSeed != 0)
                            {
                                MyMod.SelectedSaveSeed = 0;
                                OnSaveSlotChanged();
                            }
                        }
                    }
                    List<int> Regions = new List<int>();
                    for (int i = 0; i < Enum.GetNames(typeof(GameRegion)).Length; i++){Regions.Add(0);}

                    List<int> ExpModes = new List<int>();
                    for (int i = 0; i < (int)ExperienceModeType.NUM_MODES; i++){ExpModes.Add(0);}

                    for (int i = 0; i < Peoples; i++)
                    {
                        ulong playerID = ulong.Parse(SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i).ToString());
                        MayUpdatePlayerInLobby(playerID);
                        if (MyMod.LobbyState == "Vote")
                        {
                            string Region = SteamMatchmaking.GetLobbyMemberData(lobbyID, new CSteamID(playerID), "Region");
                            if(Region != null && Region != "")
                            {
                                Regions[int.Parse(Region)]++;
                            }
                            string ExpMode = SteamMatchmaking.GetLobbyMemberData(lobbyID, new CSteamID(playerID), "Exp");
                            if (ExpMode != null && ExpMode != "")
                            {
                                ExpModes[int.Parse(ExpMode)]++;
                            }
                        }
                    }

                    if(MyMod.LobbyState == "Vote")
                    {
                        ClearVotesObjects();
                        UpdateVoteObjects(Regions, ExpModes);
                        ProcessVoteUpdate(lobbyID, Regions, ExpModes);
                    }else{
                        MyMod.LobbyUI.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "LOBBY " + Peoples + "/" + Limit;
                    }
                }
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

                IsMyLobby = true;
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
            public static void LoadSaveForHosting()
            {
                MenuChange.ChangeMenuItems("Original");
                MyMod.HostFromLobbyAfterLoader = true;
                MyMod.ForceLoadSlotByName(MyMod.SelectedSaveName);
            }
            public static void CreateSaveForHosting()
            {
                MenuChange.ChangeMenuItems("Original");
                MyMod.HostFromLobbyAfterLoader = true;
                MyMod.ForceCreateSlotForPlaying(MyMod.LobbyStartingRegion, MyMod.LobbyStartingExperience);
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
            public static void CopyInviteLink()
            {
                //steam://joinlobby/305620/idlobby/idprofile
                string inviteLink = "steam://joinlobby/305620/" + MyMod.MyLobby+"/"+ SteamUser.GetSteamID();
                GUIUtility.systemCopyBuffer = inviteLink;
            }
            public static void SetSaveSlotSeed(int SaveSeed)
            {
                if(MyMod.MyLobby != "")
                {
                    SteamMatchmaking.SetLobbyData(new CSteamID(ulong.Parse(MyMod.MyLobby)), "SaveSeed", SaveSeed.ToString());
                }else{
                    SteamMatchmaking.DeleteLobbyData(new CSteamID(ulong.Parse(MyMod.MyLobby)), "SaveSeed");
                }
            }
            public static bool IsLobbyOwner()
            {
                if (MyMod.MyLobby != "")
                {
                    return false;
                }else{
                    if (SteamMatchmaking.GetLobbyOwner(new CSteamID(ulong.Parse(MyMod.MyLobby))) != SteamUser.GetSteamID())
                    {
                        return false;
                    }else{
                        return true;
                    }
                }
            }

            public static void SetSpawnStyle()
            {
                SteamMatchmaking.SetLobbyData(new CSteamID(ulong.Parse(MyMod.MyLobby)), "SpawnStyle", MyMod.ServerConfig.m_PlayersSpawnType.ToString());
            }
            public static void SetLobbyName(string CustomName = "")
            {
                if (CustomName == "")
                {
                    SteamMatchmaking.SetLobbyData(new CSteamID(ulong.Parse(MyMod.MyLobby)), "Name", SteamFriends.GetPersonaName());
                    SteamMatchmaking.SetLobbyData(new CSteamID(ulong.Parse(MyMod.MyLobby)), "State", "WaitForPlayers");
                }else{
                    SteamMatchmaking.SetLobbyData(new CSteamID(ulong.Parse(MyMod.MyLobby)), "Name", CustomName);
                }
            }
            public static void SetLobbyServer()
            {
                SteamMatchmaking.SetLobbyGameServer(new CSteamID(ulong.Parse(MyMod.MyLobby)), 0, 0, SteamUser.GetSteamID());
            }
            public static void SetLobbyState(string s)
            {
                if (s != "")
                {
                    if (IsMyLobby)
                    {
                        SteamMatchmaking.SetLobbyData(new CSteamID(ulong.Parse(MyMod.MyLobby)), "State", s);
                    }

                    if (MyMod.LobbyState != s)
                    {
                        MyMod.LobbyState = s;
                        if (s == "Vote")
                        {
                            MyMod.LobbyVoteLeft = 62;
                        }
                        OnLobbyStateChanged();
                    }
                }else{
                    if (IsMyLobby)
                    {
                        SteamMatchmaking.DeleteLobbyData(new CSteamID(ulong.Parse(MyMod.MyLobby)), "State");
                    }
                    MyMod.LobbyState = "";
                }
            }
            public static void SetNewGameSettings(int RegionID, int ExpID)
            {
                SteamMatchmaking.SetLobbyData(new CSteamID(ulong.Parse(MyMod.MyLobby)), "RegionID", RegionID.ToString());
                SteamMatchmaking.SetLobbyData(new CSteamID(ulong.Parse(MyMod.MyLobby)), "ExpID", ExpID.ToString());
            }

            public static void VoteForExperienceMode(int expMode)
            {
                SteamMatchmaking.SetLobbyMemberData(new CSteamID(ulong.Parse(MyMod.MyLobby)), "Exp", expMode.ToString());
                MelonLogger.Msg("I am voted for ExperienceMode " + expMode);
            }
            public static void VoteForRegion(int Region)
            {
                SteamMatchmaking.SetLobbyMemberData(new CSteamID(ulong.Parse(MyMod.MyLobby)), "Region", Region.ToString());
                MelonLogger.Msg("I am voted for Region " + Region);
            }

            public static void ConnectToHost(string hostid)
            {
                if (MyMod.m_Panel_Sandbox && MyMod.m_Panel_Sandbox.isActiveAndEnabled)
                {
                    MyMod.m_Panel_Sandbox.Enable(false);
                }
                if (MyMod.ServerBrowser)
                {
                    MyMod.ServerBrowser.SetActive(false);
                }
                if (MyMod.LobbyUI)
                {
                    MyMod.LobbyUI.SetActive(false);
                }
                
                MyMod.instance.myId = 0;
                CSteamID reciver = new CSteamID(ulong.Parse(hostid));
                MyMod.InitializeClientData();

                SteamNetworking.AcceptP2PSessionWithUser(reciver);
                MelonLogger.Msg("[SteamWorks.NET] Trying connecting to " + hostid);
                MyMod.DoSteamWorksConnect(SteamUser.GetSteamID().ToString());
            }

            public static void BrowseServers()
            {
                MyMod.ServerBrowser.SetActive(true);
                SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);

                SteamAPICall_t handle = SteamMatchmaking.RequestLobbyList();
                OnLobbyMatchListCallResult.Set(handle);
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
