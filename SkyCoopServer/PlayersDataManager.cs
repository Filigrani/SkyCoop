using LiteNetLib;
using System.Numerics;
using System.Reflection;
using static SkyCoopServer.DataStr;
using System.Text.Json;
using System.Drawing;

namespace SkyCoopServer
{
    public class PlayersDataManager
    {
        public List<DataStr.PlayerData> m_Players = new List<DataStr.PlayerData>();

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
            foreach (DataStr.PlayerData Player in m_Players)
            {
                if(Player.m_Scene == Scene)
                {
                    ScenePlayers.Add(Player);
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
            Console.WriteLine("[GameServer] SceneAlign");
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

        public bool PlayerCanHearOtherPlayer(int SpeakerID, int ListenerID)
        {
            if(SpeakerID == ListenerID)
            {
                return m_RecursiveDebug;
            }
            
            DataStr.PlayerData Speaker = GetPlayer(SpeakerID);
            DataStr.PlayerData Listener = GetPlayer(ListenerID);
            if (Speaker != null && Listener != null)
            {

                //TODO: If Speaker and Listener are dead, always return true, because spectators must be able to hear each other.


                if (Speaker.m_Scene == Listener.m_Scene) // For now mostly just to prevent player for hearing others during loading.
                {
                    return Vector3.Distance(Speaker.m_Position, Listener.m_Position) < ServerVoice.c_MaxProximityChatDistance;
                }
            }

            return false;
        }
        public void PlayerChangeAction(int Index, int Action, bool Broadcast = true)
        {
            DataStr.PlayerData Player = GetPlayer(Index);
            if (Player != null)
            {
                Player.m_VisualData.m_LatAction = Action;

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

        public List<int> GetDMLeaders()
        {
            List<int> Leaders = new List<int>();
            List<DataStr.DMScore> Scores = new List<DMScore>();

            foreach (NetPeer Peer in s_Server.m_Instance.ConnectedPeerList.ToArray())
            {
                DataStr.PlayerData Player = GetPlayer(Peer.Id);
                Player.m_Kills = 0;
                Player.m_Deaths = 0;
                Player.m_Assists = 0;
                Scores.Add(new DataStr.DMScore(Player.m_PlayerID, Player.m_Kills, Player.m_Assists, Player.m_Deaths));
            }
            Scores.Sort();

            for (int i = 0; i < (Scores.Count < 3 ? Scores.Count : 3); i++)
            {
                Leaders.Add(Scores[i].PlayerID);
            }

            return Leaders;
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
    }
}
