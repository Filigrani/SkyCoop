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
            return m_Players[Index];
        }

        public void SetPlayerName(int Index, string Name)
        {
            DataStr.PlayerData Player = m_Players[Index];
            if (Player != null)
            {
                Player.m_PlayerName = Name;
            }
        }

        public List<DataStr.PlayerData> GetPlayersOnScene(string Scene)
        {
            List<DataStr.PlayerData> ScenePlayers = new List<DataStr.PlayerData>();
            if (Scene == "Empty" || Scene == "" || Scene == "Boot" || Scene == "MainMenu")
            {
                return ScenePlayers;
            }

            if(s_Server != null)
            {
                foreach (NetPeer Peer in s_Server.m_Instance.ConnectedPeerList.ToArray())
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
                Player.m_Position = Position;

                if(Player.m_GamePlayState != PlayerData.GamePlayState.Alive)
                {
                    return;
                }

                if(Broadcast)
                {
                    if (s_Server != null)
                    {
                        List<DataStr.PlayerData> Players = GetPlayersOnScene(Player.m_Scene);

                        foreach (DataStr.PlayerData OnScenePlayer in Players)
                        {
                            if(OnScenePlayer.m_PlayerID != Player.m_PlayerID || m_RecursiveDebug)
                            {
                                ServerSend.SendPosition(s_Server.GetClient(OnScenePlayer.m_PlayerID), Position, Player.m_PlayerID);
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
                                ServerSend.SendRotation(s_Server.GetClient(OnScenePlayer.m_PlayerID), Rotation, Player.m_PlayerID);
                            }
                        }
                    }
                }
            }
        }

        public void SceneAlign()
        {
            Logger.Log("[PlayersDataManager] SceneAlign");
            List<int> PlayerIndexes = s_Server.GetClientsIndexs();

            foreach (int PlayerID in PlayerIndexes)
            {
                DataStr.PlayerData Player = GetPlayer(PlayerID);
                foreach (int OtherPlayerID in PlayerIndexes)
                {
                    if (OtherPlayerID != PlayerID || m_RecursiveDebug)
                    {
                        DataStr.PlayerData OtherPlayer = GetPlayer(OtherPlayerID);

                        NetPeer OtherPlayerClient = s_Server.GetClient(OtherPlayerID);
                        NetPeer PlayerClient = s_Server.GetClient(PlayerID);

                        ServerSend.SendPlayerSceneNotification(OtherPlayerClient, OtherPlayer.m_Scene == Player.m_Scene, PlayerID);
                        ServerSend.SendPlayerSceneNotification(PlayerClient, OtherPlayer.m_Scene == Player.m_Scene, OtherPlayerID);
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

                                if(OtherPlayer.m_Scene == Player.m_Scene)
                                {
                                    ServerSend.SendPlayerChangeGear(s_Server.GetClient(OtherPlayerID), GearName, GearVariant, Index);
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
                    if (OtherPlayerID != Client.Id || m_RecursiveDebug)
                    {
                        DataStr.PlayerData OtherPlayer = GetPlayer(OtherPlayerID);
                        if (OtherPlayer.m_GamePlayState == PlayerData.GamePlayState.Alive)
                        {
                            ServerSend.SendPlayerSceneNotification(Client, OtherPlayer.m_Scene == SceneName, OtherPlayerID);
                        }
                    }
                }
            }
        }

        public void PlayerChangeScene(int Index, string Scene, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if (Player != null)
            {
                Player.m_Scene = Scene;

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
                return PlayerHearing.Proximity;
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
                        string SpeakerSquad = GetPlayerSquadIn(SpeakerID);
                        string ListnerSquad = GetPlayerSquadIn(ListenerID);

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
                Player.m_VisualData.m_LatAction = Action;

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
            m_Players[PlayerID] = new PlayerData(PlayerID);

            foreach (NetPeer Peer in s_Server.m_Instance.ConnectedPeerList.ToArray())
            {
                if (Peer.Id != PlayerID)
                {
                    ServerSend.SendPlayerSceneNotification(Peer, false, PlayerID);
                }
            }
            List<string> SquadsToDismember = new List<string>();
            foreach (PlayersSquad Squad in m_Squads.Values.ToArray())
            {
                Squad.RemovePlayer(PlayerID);
                if(Squad.m_Players.Count == 0)
                {
                    SquadsToDismember.Add(Squad.m_Name);
                }
            }
            foreach (string SquadNameToDismember in SquadsToDismember)
            {
                m_Squads.Remove(SquadNameToDismember);
            }
            ServerSend.SendClientStatus(PlayerID, 0, s_Server);
        }

        public DataStr.DMScore GetScore(int PlayerID)
        {
            DataStr.PlayerData Player = GetPlayer(PlayerID);
            return new DataStr.DMScore(Player.m_PlayerID, Player.m_Kills, Player.m_Assists, Player.m_Deaths);
        }

        public string GetPlayerScoreString(int PlayerID)
        {
            DataStr.PlayerData Player = GetPlayer(PlayerID);
            List<int> Leaders = GetDMLeaders();
            int Score = GetScore(PlayerID).GetFinalScore();
            return $"{Score} Place: {Leaders.IndexOf(PlayerID)+1}/{Leaders.Count}";
        }

        public List<int> GetDMLeaders()
        {
            List<int> Leaders = new List<int>();
            List<DataStr.DMScore> Scores = new List<DMScore>();

            foreach (NetPeer Peer in s_Server.m_Instance.ConnectedPeerList.ToArray())
            {
                Scores.Add(GetScore(Peer.Id));
            }
            Scores.Sort();

            for (int i = 0; i < (Scores.Count < 3 ? Scores.Count : 3); i++)
            {
                Leaders.Add(Scores[i].PlayerID);
            }

            return Leaders;
        }

        public int GetSquadsAlive()
        {
            int Squads = 0;
            foreach (PlayersSquad Squad in m_Squads.Values.ToArray())
            {
                foreach (int PlayerID in Squad.m_Players)
                {
                    PlayerData Player = GetPlayer(PlayerID);
                    if(Player.m_GamePlayState == PlayerData.GamePlayState.Alive)
                    {
                        Squads++;
                        break;
                    }
                }
            }
            foreach (PlayerData Player in m_Players)
            {
                if (Player.m_GamePlayState == PlayerData.GamePlayState.Alive)
                {
                    if(GetPlayerSquadIn(Player.m_PlayerID) == "")
                    {
                        Squads++;
                    }
                }
            }
            return Squads;
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

        public string GetShrinkModeString()
        {
            int Squads = GetSquadsAlive();
            string s = (Squads > 1 || Squads == 0) ? "s" : "";

            return $"{GetPlayersAlive()} ({Squads} Squad{s})";
        }

        public void ResetFrags()
        {
            foreach (DataStr.PlayerData Player in m_Players)
            {
                Player.m_Kills = 0;
                Player.m_Deaths = 0;
                Player.m_Assists = 0;
            }
        }

        public void SetPlayerInteractionGUID(int PlayerID, string GUID)
        {
            DataStr.PlayerData Player = GetPlayer(PlayerID);
            if (Player != null)
            {
                Player.m_InteractionGUID = GUID;
            }
        }

        public void AddPlayerToSquad(string SquadName, int PlayerID)
        {
            if (!m_Squads.ContainsKey(SquadName))
            {
                m_Squads[SquadName].AddPlayer(PlayerID);
            }
        }

        public void RemovePlayerFromSquad(string SquadName, int PlayerID)
        {
            if (!m_Squads.ContainsKey(SquadName))
            {
                m_Squads[SquadName].AddPlayer(PlayerID);
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

        public string GetPlayerSquadIn(int PlayerID)
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

        public string GetRandomSquadName()
        {
            int MaxAttempts = 5;
            int CurrentAttempt = 1;
            List<string> PossibleNames = new List<string>()
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
            };
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

        public PlayersSquad CreateSquad()
        {
            return CreateSquad(GetRandomSquadName());
        }

        public PlayersSquad CreateSquad(string SquadName)
        {
            if (!m_Squads.ContainsKey(SquadName))
            {
                PlayersSquad NewSquad = new PlayersSquad(SquadName);
                m_Squads.Add(SquadName, NewSquad);
                return NewSquad;
            }
            return null;
        }
    }
}
