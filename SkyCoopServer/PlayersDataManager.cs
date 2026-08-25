using LiteNetLib;
using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using static SkyCoopServer.DataStr;

namespace SkyCoopServer
{
    public class PlayersDataManager
    {
        public List<DataStr.PlayerData> m_Players = new List<DataStr.PlayerData>();
        public Dictionary<string, PlayersSquad> m_Squads = new Dictionary<string, PlayersSquad>();

        public const int c_SquadLimit = 3;

        public bool m_RecursiveDebug = false;

        private Server s_Server;

        public PlayersDataManager(Server ServerInstance) 
        {
            s_Server = ServerInstance;
        }

        public void InitilizePlayers(int MaxPlayers)
        {
            m_Players.Clear(); // Clear instead of creating new.
            for (int i = 0; i < MaxPlayers; i++)
            {
                m_Players.Add(new DataStr.PlayerData(i));
            }
        }

        public DataStr.PlayerData GetPlayer(int Index)
        {
            if(Index < 0)
            {
                return null;
            }
            if(Index > m_Players.Count - 1)
            {
                return null;
            }
            
            return m_Players[Index];
        }

        public bool NameIsFree(string PlayerName)
        {
            foreach (PlayerData Player in m_Players)
            {
                if(Player != null)
                {
                    if(Player.m_PlayerName == PlayerName)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool SetPlayerName(int Index, string Name)
        {
            DataStr.PlayerData Player = m_Players[Index];
            if (Player != null)
            {
                if (NameIsFree(Name))
                {
                    Player.m_PlayerName = Name;
                }
                else
                {
                    SkyCoopServer.Logger.Log(ConsoleColor.Red,$"Client {Index} trying to log in as {Name}, but name is already busy");

                    int Num = 2;
                    while (!NameIsFree($"{Name} ({Num})"))
                    {
                        Num++;

                        if(Num > 100)
                        {
                            Name = "Ilegal name";
                            s_Server.DisconnectPlayer(Index, "Bad nickname!");
                            return false;
                        }
                    }
                    Player.m_PlayerName = $"{Name} ({Num})";
                    SkyCoopServer.Logger.Log(ConsoleColor.Yellow, $"Client {Index} renamed to {Name}");
                }
            }
            return true;
        }

        public List<DataStr.PlayerData> GetPlayersOnScene(string Scene)
        {
            List<DataStr.PlayerData> ScenePlayers = new List<DataStr.PlayerData>();
            if (Scene == "Empty" || Scene == "" || Scene == "Boot" || Scene.StartsWith("MainMenu"))
            {
                return ScenePlayers;
            }

            if(s_Server != null)
            {
                List<NetPeer> peers = new List<NetPeer>();
                s_Server.m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Peer in peers.ToArray())
                {
                    DataStr.PlayerData Data = s_Server.GetPlayerDataByNetPeer(Peer);
                    if (Data != null && Data.m_Scene == Scene)
                    {
                        ScenePlayers.Add(Data);
                    }
                }
            }
            return ScenePlayers;
        }

        public void PlayerMoved(int Index, Vector3 Position, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if(Player != null )
            {
                if(Player.m_GamePlayState != PlayerData.GamePlayState.Alive)
                {
                    return;
                }

                Player.m_Position = Position;

                if (Broadcast)
                {
                    if (s_Server != null)
                    {
                        List<DataStr.PlayerData> Players = GetPlayersOnScene(Player.m_Scene);

                        foreach (DataStr.PlayerData OnScenePlayer in Players)
                        {
                            if(OnScenePlayer.m_PlayerID != Player.m_PlayerID || m_RecursiveDebug)
                            {
                                NetPeer Client = s_Server.GetClient(OnScenePlayer.m_PlayerID);

                                if (Client != null)
                                {
                                    ServerSend.SendPosition(Client, Position, Player.m_PlayerID);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void PlayerRotated(int Index, Quaternion Rotation, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if (Player != null)
            {
                Player.m_Rotation = Rotation;

                if (Player.m_GamePlayState != PlayerData.GamePlayState.Alive)
                {
                    return;
                }

                if (Broadcast)
                {
                    if (s_Server != null)
                    {
                        List<DataStr.PlayerData> Players = GetPlayersOnScene(Player.m_Scene);

                        foreach (DataStr.PlayerData OnScenePlayer in Players)
                        {
                            if (OnScenePlayer.m_PlayerID != Player.m_PlayerID || m_RecursiveDebug)
                            {
                                NetPeer Client = s_Server.GetClient(OnScenePlayer.m_PlayerID);

                                if(Client != null)
                                {
                                    ServerSend.SendRotation(Client, Rotation, Player.m_PlayerID);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void PlayerTilted(int Index, float Tilt, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if (Player != null)
            {
                Player.m_Tilt = Tilt;

                if (Player.m_GamePlayState != PlayerData.GamePlayState.Alive)
                {
                    return;
                }

                if (Broadcast)
                {
                    if (s_Server != null)
                    {
                        List<DataStr.PlayerData> Players = GetPlayersOnScene(Player.m_Scene);

                        foreach (DataStr.PlayerData OnScenePlayer in Players)
                        {
                            if (OnScenePlayer.m_PlayerID != Player.m_PlayerID || m_RecursiveDebug)
                            {
                                NetPeer Client = s_Server.GetClient(OnScenePlayer.m_PlayerID);

                                if(Client != null)
                                {
                                    ServerSend.SendTilt(Client, Tilt, Player.m_PlayerID);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void PlayerChangeGear(int Index, string GearName, int GearVariant, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if (Player != null)
            {
                Player.m_VisualData.m_GearInHands = GearName;
                Player.m_VisualData.m_GearVariant = GearVariant;

                if(GearName != "" && Player.m_GamePlayState != PlayerData.GamePlayState.Alive)
                {
                    return;
                }

                if (Broadcast)
                {
                    if (s_Server != null)
                    {
                        foreach (int OtherPlayerID in s_Server.GetClientsIndexs())
                        {
                            if (OtherPlayerID != Index || m_RecursiveDebug)
                            {
                                DataStr.PlayerData OtherPlayer = GetPlayer(OtherPlayerID);

                                if(OtherPlayer.m_Scene == Player.m_Scene)
                                {
                                    NetPeer Client = s_Server.GetClient(OtherPlayerID);

                                    if(Client != null)
                                    {
                                        ServerSend.SendPlayerChangeGear(s_Server.GetClient(OtherPlayerID), GearName, GearVariant, Index);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SendAllPlayersOnScene(NetPeer Client, string SceneName)
        {
            if (s_Server != null)
            {
                foreach (int OtherPlayerID in s_Server.GetClientsIndexs())
                {
                    if (Client != null && OtherPlayerID != Client.Id || m_RecursiveDebug)
                    {
                        DataStr.PlayerData OtherPlayer = GetPlayer(OtherPlayerID);
                        if (OtherPlayer != null && OtherPlayer.m_GamePlayState == PlayerData.GamePlayState.Alive)
                        {
                            ServerSend.SendPlayerSceneNotification(Client, OtherPlayer.m_Scene == SceneName, OtherPlayerID);
                        }
                    }
                }
            }
        }
        public void SetGameplayState(int Index, DataStr.PlayerData.GamePlayState State)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if (Player != null)
            {
                Player.SetGameplayState(State, s_Server);
            }
        }

        public void PlayerChangeScene(int Index, string Scene, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if (Player != null)
            {
                Player.m_Scene = Scene;
                Player.m_InteractionGUID = "";
                SetPlayerCarSeatGUID(Player, "");
                Player.m_IsWorking = false;

                if (Broadcast)
                {
                    if (s_Server != null)
                    {
                        foreach (int OtherPlayerID in s_Server.GetClientsIndexs())
                        {
                            if(OtherPlayerID != Index || m_RecursiveDebug)
                            {
                                DataStr.PlayerData OtherPlayer = GetPlayer(OtherPlayerID);
                                ServerSend.SendPlayerSceneNotification(s_Server.GetClient(OtherPlayerID), OtherPlayer.m_Scene == Player.m_Scene, Index);
                            }
                        }
                    }
                }
            }
        }
        public void PlayerChangeCrouch(int Index, bool CrouchState, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if (Player != null)
            {
                Player.m_VisualData.m_Crouch = CrouchState;

                if (Player.m_GamePlayState != PlayerData.GamePlayState.Alive)
                {
                    return;
                }

                if (Broadcast)
                {
                    if (s_Server != null)
                    {
                        foreach (int OtherPlayerID in s_Server.GetClientsIndexs())
                        {
                            if (OtherPlayerID != Index || m_RecursiveDebug)
                            {
                                DataStr.PlayerData OtherPlayer = GetPlayer(OtherPlayerID);

                                if (OtherPlayer.m_Scene == Player.m_Scene)
                                {
                                    ServerSend.SendPlayerCrouch(s_Server.GetClient(OtherPlayerID), CrouchState, Index);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void PlayerChangeVehicleState(int Index, bool InVehicle, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if (Player != null)
            {
                Player.m_VisualData.m_InVehicle = InVehicle;

                if (Player.m_GamePlayState != PlayerData.GamePlayState.Alive)
                {
                    return;
                }

                if (Broadcast)
                {
                    if (s_Server != null)
                    {
                        foreach (int OtherPlayerID in s_Server.GetClientsIndexs())
                        {
                            if (OtherPlayerID != Index || m_RecursiveDebug)
                            {
                                DataStr.PlayerData OtherPlayer = GetPlayer(OtherPlayerID);

                                if (OtherPlayer.m_Scene == Player.m_Scene)
                                {
                                    ServerSend.SendPlayerInVehicle(s_Server.GetClient(OtherPlayerID), InVehicle, Index);
                                }
                            }
                        }
                    }
                }
            }
        }

        public PlayerHearing PlayerCanHearOtherPlayer(int SpeakerID, int ListenerID)
        {
            if(SpeakerID == ListenerID)
            {
                return m_RecursiveDebug ? PlayerHearing.Proximity : PlayerHearing.None;
            }
            
            DataStr.PlayerData Speaker = GetPlayer(SpeakerID);
            DataStr.PlayerData Listener = GetPlayer(ListenerID);
            if (Speaker != null && Listener != null)
            {
                if((Speaker.m_GamePlayState == PlayerData.GamePlayState.Unassigned || Speaker.m_GamePlayState == PlayerData.GamePlayState.Spectator) && 
                    Listener.m_GamePlayState == PlayerData.GamePlayState.Unassigned || Listener.m_GamePlayState == PlayerData.GamePlayState.Spectator)
                {
                    return PlayerHearing.Global;
                }

                if (Speaker.m_Scene == Listener.m_Scene)
                {
                    if(Vector3.Distance(Speaker.m_Position, Listener.m_Position) < ServerVoice.c_MaxProximityChatDistance)
                    {
                        return PlayerHearing.Proximity;
                    }
                    else
                    {
                        string SpeakerSquad = GetPlayerNameSquadIn(SpeakerID);
                        string ListnerSquad = GetPlayerNameSquadIn(ListenerID);

                        if(!string.IsNullOrEmpty(SpeakerSquad) && !string.IsNullOrEmpty(ListnerSquad) && SpeakerSquad == ListnerSquad)
                        {
                            return PlayerHearing.Radio;
                        }
                    }
                }
            }

            return PlayerHearing.None;
        }
        public void PlayerChangeAction(int Index, int Action, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if (Player != null)
            {
                Player.m_VisualData.m_LastAction = Action;

                if (Player.m_GamePlayState != PlayerData.GamePlayState.Alive)
                {
                    return;
                }

                if (Broadcast)
                {
                    if (s_Server != null)
                    {
                        foreach (int OtherPlayerID in s_Server.GetClientsIndexs())
                        {
                            if (OtherPlayerID != Index || m_RecursiveDebug)
                            {
                                DataStr.PlayerData OtherPlayer = GetPlayer(OtherPlayerID);

                                if (OtherPlayer.m_Scene == Player.m_Scene)
                                {
                                    ServerSend.SendPlayerAction(s_Server.GetClient(OtherPlayerID), Action, Index);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void PlayerFire(int Index, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if (Player != null)
            {
                if (Player.m_GamePlayState != PlayerData.GamePlayState.Alive)
                {
                    return;
                }

                if (Broadcast)
                {
                    if (s_Server != null)
                    {
                        foreach (int OtherPlayerID in s_Server.GetClientsIndexs())
                        {
                            if (OtherPlayerID != Index || m_RecursiveDebug)
                            {
                                DataStr.PlayerData OtherPlayer = GetPlayer(OtherPlayerID);

                                if (OtherPlayer.m_Scene == Player.m_Scene)
                                {
                                    ServerSend.SendPlayerFire(s_Server.GetClient(OtherPlayerID), Index);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OnPlayerDisconnect(int PlayerID)
        {
            PlayersSquad LeftSquad = GetSquadPlayerIn(PlayerID);
            m_Players[PlayerID] = new PlayerData(PlayerID);

            List<NetPeer> peers = new List<NetPeer>();
            s_Server.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers.ToArray())
            {
                if (Peer.Id != PlayerID)
                {
                    ServerSend.SendPlayerSceneNotification(Peer, false, PlayerID);
                }
            }

            if(LeftSquad != null)
            {
                RemovePlayerFromSquad(LeftSquad.m_Name, PlayerID);
                if (LeftSquad.m_Players.Count == 0)
                {
                    ServerSend.SendSquadEliminated(s_Server, LeftSquad.m_Name);
                    Logger.Log(ConsoleColor.Cyan, $"[Squads] Squad {LeftSquad.m_Name} was dismembered, no players left.");
                    m_Squads.Remove(LeftSquad.m_Name);
                }
            }
            RemoveAllInviteOfPlayer(PlayerID);
            DoSquadsCheck();
            ServerSend.SendClientStatus(PlayerID, 0, s_Server);
            s_Server.OnPlayersCountChanged();
        }

        public DataStr.Score GetScore(int PlayerID)
        {
            DataStr.PlayerData Player = GetPlayer(PlayerID);

            if(s_Server.m_Rules.m_HUDMode == "GunGame")
            {
                return new DataStr.Score(Player.m_PlayerID, Player.m_Kills, 0, 0);
            }

            return new DataStr.Score(Player.m_PlayerID, Player.m_Kills, Player.m_Assists, Player.m_Deaths);
        }

        public string GetPlayerScoreString(int PlayerID)
        {
            DataStr.PlayerData Player = GetPlayer(PlayerID);
            List<int> Leaders = GetLeadersIDs(true);
            int Score = GetScore(PlayerID).GetFinalScore();
            return $"{Score} Place: {Leaders.IndexOf(PlayerID)+1}/{Leaders.Count}";
        }

        public List<DataStr.LeaderData> GetLeaders(bool Unlimited = false)
        {
            List<int> GetLeadersIDs = this.GetLeadersIDs(Unlimited);

            List<DataStr.LeaderData> Data = new List<LeaderData>();

            foreach (int PlayerID in GetLeadersIDs)
            {
                PlayerData PlayerData = GetPlayer(PlayerID);

                if(PlayerData != null)
                {
                    Data.Add(new LeaderData(PlayerData, GetScore(PlayerID).GetFinalScore()));
                }
            }
            return Data;
        }

        public List<int> GetLeadersIDs(bool Unlimited = false)
        {
            List<int> Leaders = new List<int>();
            List<DataStr.Score> Scores = new List<Score>();

            if(s_Server.m_Config.m_GameMode != "Shrink")
            {
                List<NetPeer> peers = new List<NetPeer>();
                s_Server.m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Peer in peers.ToArray())
                {
                    Scores.Add(GetScore(Peer.Id));
                }
            }
            else
            {
                bool FoundSquad = false;
                foreach (PlayersSquad Squad in m_Squads.Values.ToArray())
                {
                    if (Squad.IsAlive(s_Server))
                    {
                        foreach (int PlayerID in Squad.m_Players)
                        {
                            Scores.Add(GetScore(PlayerID));
                        }
                        FoundSquad = true;
                        break;
                    }
                }
                if (!FoundSquad)
                {
                    List<NetPeer> peers = new List<NetPeer>();
                    s_Server.m_Instance.GetConnectedPeers(peers);
                    foreach (NetPeer Peer in peers.ToArray())
                    {
                        DataStr.PlayerData PlayerData = GetPlayer(Peer.Id);
                        if(PlayerData != null && PlayerData.m_GamePlayState == PlayerData.GamePlayState.Alive)
                        {
                            Scores.Add(GetScore(Peer.Id));
                        }
                    }
                }
            }


            Scores.Sort();

            int Count = 0;

            if (Unlimited)
            {
                Count = Scores.Count;
            }
            else
            {
                Count = Scores.Count < 3 ? Scores.Count : 3;
            }

            for (int i = 0; i < Count; i++)
            {
                Leaders.Add(Scores[i].PlayerID);
            }

            return Leaders;
        }

        public int GetSquads()
        {
            int Squads = 0;
            foreach (PlayersSquad Squad in m_Squads.Values.ToArray())
            {
                Squads++;
            }

            List<NetPeer> peers = new List<NetPeer>();
            s_Server.m_Instance.GetConnectedPeers(peers);

            foreach (NetPeer Peer in peers)
            {
                if (GetPlayerNameSquadIn(Peer.Id) == "")
                {
                    Squads++;
                }
            }
            return Squads;
        }

        public int GetSquadsAlive()
        {
            int Squads = 0;
            foreach (PlayersSquad Squad in m_Squads.Values.ToArray())
            {
                if (Squad.IsAlive(s_Server))
                {
                    Squads++;
                }
            }
            foreach (PlayerData Player in m_Players)
            {
                if (Player.m_GamePlayState == PlayerData.GamePlayState.Alive)
                {
                    if(GetPlayerNameSquadIn(Player.m_PlayerID) == "")
                    {
                        Squads++;
                    }
                }
            }
            return Squads;
        }

        public PlayersSquad GetSquad(string SquadName)
        {
            if(m_Squads.ContainsKey(SquadName))
            {
                return m_Squads[SquadName];
            }

            return null;
        }

        public int GetPlayersAlive()
        {
            int Alive = 0;
            foreach (PlayerData Player in m_Players)
            {
                if (Player.m_GamePlayState == PlayerData.GamePlayState.Alive)
                {
                    Alive++;
                }
            }
            return Alive;
        }

        public int GetPlayers()
        {
            List<NetPeer> peers = new List<NetPeer>();
            s_Server.m_Instance.GetConnectedPeers(peers);
            return peers.Count;
        }

        public void DoSquadsCheck()
        {
            if (s_Server.m_Rules.m_HUDMode == "Shrink")
            {
                if (GetSquadsAlive() <= 1)
                {
                    s_Server.ForceToOver();
                }
            }
        }

        public string GetPlayersString()
        {
            int Squads = GetSquads();
            int Players = GetPlayers();

            string s = (Squads > 1 || Squads == 0) ? "s" : "";

            return $"{Players} ({Squads} Squad{s})";
        }

        public string GetShrinkModeString()
        {
            int Squads = GetSquadsAlive();
            int Players = GetPlayersAlive();

            string s = (Squads > 1 || Squads == 0) ? "s" : "";

            return $"{Players} ({Squads} Squad{s})";
        }

        public void ResetGameScores()
        {
            foreach (DataStr.PlayerData Player in m_Players)
            {
                Player.m_Kills = 0;
                Player.m_Deaths = 0;
                Player.m_Assists = 0;
                Player.m_Tier = 0;
                Player.m_TierProgress = 0;
            }
        }

        public void SetPlayerInteractionGUID(int PlayerID, string GUID)
        {
            DataStr.PlayerData Player = GetPlayer(PlayerID);
            if (Player != null)
            {
                SetPlayerInteractionGUID(Player, GUID);
            }
        }

        public void SetPlayerInteractionGUID(DataStr.PlayerData Player, string GUID)
        {
            if (Player != null)
            {
                Player.m_InteractionGUID = GUID;
            }
        }

        public void SetPlayerCarSeatGUID(int PlayerID, string GUID)
        {
            DataStr.PlayerData Player = GetPlayer(PlayerID);
            if (Player != null)
            {
                SetPlayerCarSeatGUID(Player, GUID);
            }
        }

        public void SetPlayerCarSeatGUID(DataStr.PlayerData Player, string GUID)
        {
            if (Player != null)
            {
                Player.m_CarSeat = GUID;
                PlayerChangeVehicleState(Player.m_PlayerID, !string.IsNullOrEmpty(Player.m_CarSeat));
            }
        }

        public void AddPlayerToSquad(string SquadName, int PlayerID)
        {
            if (m_Squads.ContainsKey(SquadName))
            {
                PlayersSquad Squad = m_Squads[SquadName];
                if (Squad.AddPlayer(PlayerID, s_Server))
                {
                    Logger.Log(ConsoleColor.Cyan, $"[Squads] Player {PlayerID} added to squad {SquadName}");
                }
                else
                {
                    Logger.Log(ConsoleColor.Yellow, $"[Squads] Wasn't able to add player {PlayerID} to squad {SquadName}");
                }
            }
        }

        public void RemovePlayerFromSquad(string SquadName, int PlayerID)
        {
            PlayersSquad Squad = GetSquad(SquadName);

            if(Squad != null)
            {
                if (Squad.HasPlayer(PlayerID))
                {
                    Squad.RemovePlayer(PlayerID, s_Server);
                    Logger.Log(ConsoleColor.Cyan, $"[Squads] Player {PlayerID} removed from squad {SquadName}");

                    NetPeer LeftPlayer = s_Server.GetClient(PlayerID);

                    if (LeftPlayer != null)
                    {
                        ServerSend.SendAssignSquad(LeftPlayer, false);
                    }

                    if (s_Server.m_Rules.m_HUDMode == "Shrink")
                    {
                        List<NetPeer> peers = new List<NetPeer>();
                        s_Server.m_Instance.GetConnectedPeers(peers);
                        foreach (NetPeer Peer in peers.ToArray())
                        {
                            ServerSend.SendHUDSideBarUpdate(Peer, 1, s_Server.m_PlayersData.GetShrinkModeString(), s_Server);
                        }
                    }
                    if (s_Server.m_Rules.m_HUDMode == "Lobby")
                    {
                        List<NetPeer> peers = new List<NetPeer>();
                        s_Server.m_Instance.GetConnectedPeers(peers);
                        foreach (NetPeer Peer in peers.ToArray())
                        {
                            ServerSend.SendHUDSideBarUpdate(Peer, 2, s_Server.m_PlayersData.GetPlayersString(), s_Server);
                        }
                    }
                }
            }
        }

        public bool PlayerIsInvitedBySomeone(int PlayerID)
        {
            foreach (PlayersSquad Squad in m_Squads.Values.ToArray())
            {
                if (Squad.PlayerIsInvited(PlayerID))
                {
                    return true;
                }
            }
            return false;
        }

        public void InvitePlayerToSquad(string SquadName, int PlayerID)
        {
            if (m_Squads.ContainsKey(SquadName))
            {
                PlayersSquad Squad = m_Squads[SquadName];

                Squad.AddInvite(PlayerID);

                NetPeer InvitedPlayer = s_Server.GetClient(PlayerID);

                if (InvitedPlayer != null)
                {
                    ServerSend.SendSquadInvite(InvitedPlayer, SquadName);
                }
            }
        }

        public void RefuseSquadInvite(string SquadName, int PlayerID)
        {
            if (m_Squads.ContainsKey(SquadName))
            {
                PlayersSquad Squad = m_Squads[SquadName];
                Squad.RemoveInvite(PlayerID);
            }
        }

        public void AcceptInviteToSquad(string SquadName, int PlayerID)
        {
            PlayersSquad PlayerInSquad = GetSquadPlayerIn(PlayerID);

            if(PlayerInSquad == null)
            {
                if (m_Squads.ContainsKey(SquadName))
                {
                    PlayersSquad Squad = m_Squads[SquadName];

                    if (Squad.PlayerIsInvited(PlayerID))
                    {
                        if (Squad.m_Players.Count >= c_SquadLimit)
                        {
                            NetPeer Client = s_Server.GetClient(PlayerID);

                            if (Client != null)
                            {
                                ServerSend.SendSquadResponce(Client, Packet.SquadResponce.SquadIsFull);
                            }
                        }
                        else
                        {
                            AddPlayerToSquad(SquadName, PlayerID);
                        }
                    }

                    Squad.RemoveInvite(PlayerID);
                }
            }
            else
            {
                ServerSend.SendSquadResponce(s_Server.GetClient(PlayerID), Packet.SquadResponce.YouAlreadyInSquad);
            }
        }

        public bool CanAddPlayerToSquad(string SquadName, int PlayerID)
        {
            foreach (PlayersSquad Squad in m_Squads.Values.ToArray())
            {
                if (Squad.HasPlayer(PlayerID))
                {
                    return false;
                }
            }
            return true;
        }

        public string GetPlayerNameSquadIn(int PlayerID)
        {
            foreach (PlayersSquad Squad in m_Squads.Values.ToArray())
            {
                if (Squad.HasPlayer(PlayerID))
                {
                    return Squad.m_Name;
                }
            }
            return "";
        }

        public PlayersSquad GetSquadPlayerIn(int PlayerID)
        {
            foreach (PlayersSquad Squad in m_Squads.Values.ToArray())
            {
                if (Squad.HasPlayer(PlayerID))
                {
                    return Squad;
                }
            }
            return null;
        }

        public void CreateRandomSquadForPlayer(int PlayerID)
        {
            Logger.Log(ConsoleColor.Cyan, $"[Squads] Player {PlayerID} requested to create random squad");
            PlayersSquad OldSquad = GetSquadPlayerIn(PlayerID);
            if (OldSquad == null)
            {
                PlayersSquad Squad = CreateSquad(s_Server);

                if(Squad != null)
                {
                    AddPlayerToSquad(Squad.m_Name, PlayerID);
                }
            }
            else
            {
                Logger.Log(ConsoleColor.Yellow, $"[Squads] Player {PlayerID} already in another squad ({OldSquad.m_Name})");
            }
        }

        public void JoinRandomSquad(int PlayerID)
        {
            Logger.Log(ConsoleColor.Cyan, $"[Squads] Player {PlayerID} requested to join random squad");
            PlayersSquad OldSquad = GetSquadPlayerIn(PlayerID);
            if (OldSquad == null)
            {
                PlayersSquad Squad = GetRandomJoinableSquad(PlayerID);

                if (Squad != null && Squad.m_Players.Count < c_SquadLimit)
                {
                    AddPlayerToSquad(Squad.m_Name, PlayerID);
                }
                else
                {
                    Logger.Log(ConsoleColor.Yellow, $"[Squads] Wasn't able to add player {PlayerID} to any squad. There no joinable squads avalaible");
                }
            }
            else
            {
                Logger.Log(ConsoleColor.Yellow, $"[Squads] Player {PlayerID} already in another squad ({OldSquad.m_Name})");
            }
        }

        public void RemoveAllInviteOfPlayer(int PlayerID)
        {
            foreach (PlayersSquad Squad in m_Squads.Values)
            {
                if(Squad != null && Squad.PlayerIsInvited(PlayerID))
                {
                    Squad.RemoveInvite(PlayerID);
                }
            }
        }

        public static List<string> GetPossibleSquadNames()
        {
            return new List<string>()
            {
                "Alpha",
                "Bravo",
                "Charlie",
                "Delta",
                "Echo",
                "Foxtrot",
                "Golf",
                "Hotel",
                "India",
                "Juliet",
                "Kilo",
                "Lima",
                "Mike",
                "November",
                "Oscar",
                "Papa",
                "Quebec",
                "Romeo",
                "Sierra",
                "Tango",
                "Uniform",
                "Victor",
                "Whiskey",
                "X-ray",
                "Yankee",
                "Zulu",
                "Fijma",
                "Shpingalets",
                "Dogma",
                "Cinema",
                "Sintarians",
                "UwU",
            };
        }

        public string GetRandomSquadName()
        {
            int MaxAttempts = 5;
            int CurrentAttempt = 1;
            List<string> PossibleNames = GetPossibleSquadNames();
            System.Random RNG = new System.Random(Guid.NewGuid().GetHashCode());
            while (CurrentAttempt <= MaxAttempts)
            {
                string SquadName = PossibleNames[RNG.Next(0, PossibleNames.Count)];
                if (!m_Squads.ContainsKey(SquadName))
                {
                    return SquadName;
                }
                CurrentAttempt++;
            }
            return Guid.NewGuid().ToString();
        }

        public PlayersSquad CreateSquad(Server ServerInstance)
        {
            return CreateSquad(ServerInstance, GetRandomSquadName());
        }

        public PlayersSquad CreateSquad(Server ServerInstance, string SquadName)
        {
            if (!m_Squads.ContainsKey(SquadName))
            {
                PlayersSquad NewSquad = new PlayersSquad(SquadName);
                m_Squads.Add(SquadName, NewSquad);

                Logger.Log(ConsoleColor.Cyan, $"[Squads] Squad {SquadName} created");
                ServerSend.SendSquadCreated(ServerInstance, SquadName);
                return NewSquad;
            }
            Logger.Log(ConsoleColor.Yellow, $"[Squads] Failed to created Squad. Too many squads!");
            return null;
        }

        public PlayersSquad GetRandomJoinableSquad(int PlayerID)
        {
            List<PlayersSquad> Joinables = new List<PlayersSquad> ();

            foreach (PlayersSquad Squad in m_Squads.Values.ToArray())
            {
                if(Squad.HasPlayer(PlayerID))
                {
                    continue;
                }
                Joinables.Add(Squad);
            }

            if(Joinables.Count > 0)
            {
                if(Joinables.Count == 1)
                {
                    return Joinables[0];
                }
                else
                {
                    System.Random RNG = new System.Random(Guid.NewGuid().GetHashCode());
                    return Joinables[RNG.Next(0, Joinables.Count)];
                }
            }
            
            return null;
        }

        public bool PlayerCanBeTrusted(PlayerData Player)
        {
            // Main idea is to check if player is operator that can use cheats.
            return true;
        }

        public static List<PlayerData> GetPlayersOnScene(string SceneName, Server ServerInstance, bool IncludeDead = false)
        {
            List<PlayerData> PlayerDatas = new List<PlayerData>();
            List<NetPeer> peers = new List<NetPeer>();
            ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers.ToArray())
            {
                PlayerData Data = ServerInstance.GetPlayerDataByNetPeer(Peer);
                if (Data != null)
                {
                    if (Data.m_Scene == SceneName)
                    {
                        if (!IncludeDead && (Data.m_GamePlayState == PlayerData.GamePlayState.Dead || Data.m_GamePlayState == PlayerData.GamePlayState.Spectator || Data.m_GamePlayState == PlayerData.GamePlayState.Unassigned))
                        {
                            continue;
                        }
                        PlayerDatas.Add(Data);
                    }
                }
            }
            return PlayerDatas;
        }

        public void UpdateScorePlace()
        {
            if (s_Server != null)
            {
                int SideBarIndex = -1;
                if(s_Server.m_Rules.m_HUDMode == "DM")
                {
                    SideBarIndex = 3;
                }
                else if(s_Server.m_Rules.m_HUDMode == "GunGame")
                {
                    SideBarIndex = 2;
                }

                if(SideBarIndex != -1)
                {
                    List<NetPeer> peers = new List<NetPeer>();
                    s_Server.m_Instance.GetConnectedPeers(peers);
                    foreach (NetPeer Peer in peers.ToArray())
                    {
                        ServerSend.SendHUDSideBarUpdate(Peer, SideBarIndex, GetPlayerScoreString(Peer.Id), s_Server);
                    }
                }
            }
        }
    }
}
