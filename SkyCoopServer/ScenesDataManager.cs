using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static SkyCoopServer.DataStr;

namespace SkyCoopServer
{
    public class ScenesDataManager
    {
        public Server m_ServerInstance;
        public Dictionary<string, SceneData> m_LoadedScenes = new Dictionary<string, SceneData>();
        public ulong m_Time = 0; // Сколько сервер запущен в реальных секундах

        public float m_ElapsedInGameHours = 0; // Сколько игровых часов сервер запущен

        public float m_TODTimeNormalized = 0.5f; // 0 - 1, 0.5 = 12:00
        public int m_TOD = 3600; // 12:00 

        // Сколько реальных секунд длиться день в TLD
        public const int с_SecondsInCycle = 7200; // (24 * 60) * 60) / 12 Время в TLD идёт в 12 раз быстрее чем в реале.

        // Сколько реальных секунд длиться час в TLD
        public const int c_SecondsInHour = 300; // (60 * 60) / 12

        // Сколько реальных секунд длиться минута в TLD
        public const int c_SecondsInMinute = 5; // 60 / 12

        public ScenesDataManager(Server Server)
        {
            m_ServerInstance = Server;
        }

        public void LoadScene(MapData MapData)
        {
            if(MapData == null)
            {
                return;
            }
            
            LoadScene(MapData.Scene, MapData);
        }

        public SceneData GetSceneData(string SceneName)
        {
            SceneData sceneData = null;
            if (m_LoadedScenes.TryGetValue(SceneName, out sceneData))
            {
                return sceneData;
            }
            
            return null;
        }

        public void LoadScene(string SceneName, MapData MapData = null)
        {
            if (!m_LoadedScenes.ContainsKey(SceneName))
            {
                SkyCoopServer.Logger.Log($"Trying to load Scene {SceneName}");
                //TODO load from file.
                SceneData sceneData = new SceneData();
                sceneData.m_SceneName = SceneName;

                m_LoadedScenes.Add(SceneName, sceneData);

                if (MapData != null)
                {
                    sceneData.LoadMapData(m_ServerInstance, MapData);
                }
            }
        }

        public void UnloadScene(Server ServerInstance, string SceneName)
        {
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                //TODO Тут ещё нужно будет удалять файл сохранения сцены, но пока мы не сохраняем ни чего.
                SceneData Data = m_LoadedScenes[SceneName];

                if (Data != null)
                {
                    Data.Unload();
                }

                m_LoadedScenes.Remove(SceneName);
            }
        }

        public void UnloadSceneNobodyOn(Server ServerInstance)
        {
            foreach (string LoadedSceneName in m_LoadedScenes.Keys.ToArray())
            {
                bool CanUnload = true;
                List<NetPeer> peers = new List<NetPeer>();
                ServerInstance.m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Peer in peers.ToArray())
                {
                    if (ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == LoadedSceneName)
                    {
                        CanUnload = false;
                        break;
                    }
                }
                if (CanUnload)
                {
                    UnloadScene(ServerInstance, LoadedSceneName);
                    SkyCoopServer.Logger.Log($"Scene unloaded because no body on there {LoadedSceneName}");
                }
            }
        }

        public List<V3Quat> FilterSpawnPointsByZone(List<V3Quat> SpawnPoints, DataStr.DangerCircleData Zone)
        {
            List<V3Quat> FilteredSpawnPoints = new List<V3Quat>();
            foreach (V3Quat Point in SpawnPoints)
            {
                if (Zone.IsInsideZone(Point.m_Position))
                {
                    FilteredSpawnPoints.Add(Point);
                }
            }
            return FilteredSpawnPoints;
        }

        public float GetApproximatelyMapSize(List<V3Quat> SpawnPoints)
        {
            if(SpawnPoints.Count < 2)
            {
                return 0;
            }

            Vector3 firstPoint = SpawnPoints[0].m_Position;

            Vector3 farthestFromFirst = SpawnPoints[0].m_Position;
            float BestDistance = float.NegativeInfinity;

            foreach (var spawn in SpawnPoints)
            {
                Vector3 currentPos = spawn.m_Position;

                float Dist = Vector3.Distance(firstPoint, currentPos);
                if (Dist > BestDistance)
                {
                    BestDistance = Dist;
                    farthestFromFirst = currentPos;
                }
            }

            Vector3 farthestFromSecond = farthestFromFirst;
            float FinalDistance = 0f;

            foreach (var spawn in SpawnPoints)
            {
                Vector3 currentPos = spawn.m_Position;
                float Dist = Vector3.Distance(farthestFromFirst, currentPos);
                if (Dist > FinalDistance)
                {
                    FinalDistance = Dist;
                    farthestFromSecond = currentPos;
                }
            }

            return FinalDistance;
        }

        public V3Quat GetSpawnPoint(string SceneName, int PlayerID)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[GetSpawnPoint] called on scene {SceneName} that not exist!");
                return new V3Quat();
            }

            List<V3Quat> SpawnPoints = SceneData.m_SpawnPoints;

            if (m_ServerInstance.m_Rules.m_AdvancedSpawnPoints)
            {
                if (SceneData.m_ActiveZone != null)
                {
                    SpawnPoints = FilterSpawnPointsByZone(SpawnPoints, SceneData.m_ActiveZone);
                }
            }

            if (SpawnPoints.Count > 0)
            {
                if (SpawnPoints.Count == 1)
                {
                    return SpawnPoints[0];
                }
                else
                {
                    List<V3Quat> FilteredSpawnPoints = new List<V3Quat>();
                    List<Vector3> PlayersPositions = new List<Vector3>();

                    float AproximatedMapSize = GetApproximatelyMapSize(SpawnPoints);
                    float MinimalSafeDistance = 15f;

                    if (AproximatedMapSize > 0)
                    {
                        float SafeSpace = AproximatedMapSize * 0.1f;

                        MinimalSafeDistance = Math.Clamp(SafeSpace, 15, 350);
                    }

                    if (m_ServerInstance.m_Rules.m_AdvancedSpawnPoints)
                    {
                        PlayersSquad Squad = m_ServerInstance.m_PlayersData.GetSquadPlayerIn(PlayerID);

                        if (Squad != null)
                        {
                            //Logger.Log(ConsoleColor.Green, $"[GetSpawnPoint] Trying to find point close to teamate of squad {Squad.m_Name}");
                            List<Vector3> TeammatePoints = new List<Vector3>();
                            foreach (PlayerData PlayerData in PlayersDataManager.GetPlayersOnScene(SceneName, m_ServerInstance, false))
                            {
                                if (PlayerData != null && (PlayerData.m_PlayerID != PlayerID || m_ServerInstance.m_PlayersData.m_RecursiveDebug) && Squad.HasPlayer(PlayerData.m_PlayerID) && PlayerData.m_GamePlayState == PlayerData.GamePlayState.Alive)
                                {
                                    //Logger.Log(ConsoleColor.Green, $"[GetSpawnPoint] Player {PlayerData.m_PlayerName} is potential teamate");
                                    TeammatePoints.Add(PlayerData.m_Position);
                                }
                            }

                            if (TeammatePoints.Count > 0)
                            {
                                V3Quat BestPoint = null;
                                float BestDistance = float.PositiveInfinity;

                                foreach (V3Quat Point in SceneData.m_SpawnPoints)
                                {
                                    float ClosestDistance = float.PositiveInfinity;
                                    foreach (Vector3 TeamatePoint in TeammatePoints)
                                    {
                                        float Dist = Vector3.Distance(Point.m_Position, TeamatePoint);
                                        if (Dist < ClosestDistance)
                                        {
                                            ClosestDistance = Dist;
                                        }
                                    }

                                    if (ClosestDistance < BestDistance)
                                    {
                                        BestDistance = ClosestDistance;
                                        BestPoint = Point;
                                    }
                                }
                                if (BestPoint != null)
                                {
                                    //Logger.Log(ConsoleColor.Green, $"[GetSpawnPoint] Found closest point to teammate");
                                    return BestPoint;
                                }
                            }
                            else
                            {
                                //Logger.Log(ConsoleColor.Green, $"[GetSpawnPoint] No alive squad teamates, going back to regular spawn search");
                            }
                        }

                        //Logger.Log(ConsoleColor.Green, $"[GetSpawnPoint] Searching point away from everyone else...");


                        foreach (PlayerData PlayerData in PlayersDataManager.GetPlayersOnScene(SceneName, m_ServerInstance, false))
                        {
                            if (PlayerData != null && (PlayerData.m_PlayerID != PlayerID || m_ServerInstance.m_PlayersData.m_RecursiveDebug))
                            {
                                if (Squad != null && Squad.HasPlayer(PlayerData.m_PlayerID)) // Друг, но всё ровно почему то не нашли его в преведущем шаге.
                                {
                                    continue;
                                }
                                PlayersPositions.Add(PlayerData.m_Position);
                            }
                        }

                        foreach (V3Quat Point in SceneData.m_SpawnPoints)
                        {
                            bool IsSafe = true;
                            foreach (Vector3 OtherPlayerPosition in PlayersPositions)
                            {
                                if (Vector3.Distance(Point.m_Position, OtherPlayerPosition) < MinimalSafeDistance)
                                {
                                    IsSafe = false;
                                    break;
                                }
                            }
                            if (IsSafe)
                            {
                                FilteredSpawnPoints.Add(Point);
                            }
                        }
                    }

                    if (FilteredSpawnPoints.Count > 0)
                    {
                        return FilteredSpawnPoints[new Random(Guid.NewGuid().GetHashCode()).Next(0, FilteredSpawnPoints.Count)];
                    }
                    else
                    {
                        return SceneData.m_SpawnPoints[new Random(Guid.NewGuid().GetHashCode()).Next(0, SceneData.m_SpawnPoints.Count)];
                    }
                }
            }
            return new V3Quat();
        }

        public void AddGear(string SceneName, GearDataContainer DataContainer)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[AddGear] called on scene {SceneName} that not exist!");
                return;
            }

            SceneData.m_Gears.Add(DataContainer.m_Data.m_GUID, DataContainer);

            ServerSend.SendGearVisual(DataContainer.m_Visual, SceneName, m_ServerInstance);
        }

        public GearDataContainer GetGear(string SceneName, string GUID, bool Remove = false)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[GetGear] called on scene {SceneName} that not exist!");
                return null;
            }

            if (SceneData.m_Gears.ContainsKey(GUID))
            {
                GearDataContainer Data = SceneData.m_Gears[GUID];
                if (Remove)
                {
                    SceneData.m_Gears.Remove(GUID);
                    ServerSend.SendGearRemoved(GUID, SceneName, m_ServerInstance);
                }
                return Data;
            }
            return null;
        }

        public void AddGear(string SceneName, string GearName, Vector3 Position, Quaternion Rotation, string JSON)
        {
            string NewGUID = Guid.NewGuid().ToString();

            GearDataContainer DataContainer = new GearDataContainer();

            DataContainer.m_Data.m_GUID = NewGUID;
            DataContainer.m_Data.m_JSON = JSON;

            DataContainer.m_Visual.m_GUID = NewGUID;
            DataContainer.m_Visual.m_GearName = GearName;
            DataContainer.m_Visual.m_Position = Position;
            DataContainer.m_Visual.m_Rotation = Rotation;

            AddGear(SceneName, DataContainer);
        }

        public void AddOpenableState(string SceneName, string GUID, bool OpenState)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[AddOpenableState] called on scene {SceneName} that not exist!");
                return;
            }

            if (SceneData.m_Openables.ContainsKey(GUID))
            {
                SceneData.m_Openables[GUID] = OpenState;
            }
            else
            {
                SceneData.m_Openables.Add(GUID, OpenState);
            }
        }

        public void SendZone(string SceneName, NetPeer Client)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[SendZone] called on scene {SceneName} that not exist!");
                return;
            }

            if (SceneData.m_ActiveZone != null)
            {
                ServerSend.SendZoneUpdate(Client, SceneData.m_ActiveZone.m_Data, SceneData.m_ActiveZone.GetNextCenter(), SceneData.m_ActiveZone.GetNextRadius(), SceneData.m_ActiveZone.m_Config.MapScale.ToVector(), m_ServerInstance);
                ServerSend.SendTimerPrefix(Client, SceneData.m_ActiveZone.GetTimerPrefix());
                ServerSend.ClientGameModeTimer(Client, SceneData.m_ActiveZone.GetTimerSeconds());
            }
        }

        public void AddContainer(string GUID, string CompressedJSON, string SceneName)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[AddContainer] called on scene {SceneName} that not exist!");
                return;
            }

            if (SceneData.m_Containers.ContainsKey(GUID))
            {
                SceneData.m_Containers.Remove(GUID);
            }
            SceneData.m_Containers.Add(GUID, CompressedJSON);
        }

        public void RemoveContainer(string GUID, string SceneName)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[RemoveContainer] called on scene {SceneName} that not exist!");
                return;
            }

            if (SceneData.m_Containers.ContainsKey(GUID))
            {
                SceneData.m_Containers.Remove(GUID);
            }
        }

        public void AddProp(DataStr.PropData Data, string SceneName)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[AddProp] called on scene {SceneName} that not exist!");
                return;
            }

            if (SceneData.m_Props.ContainsKey(Data.guid))
            {
                SceneData.m_Props.Remove(Data.guid);
            }
            SceneData.m_Props.Add(Data.guid, Data);
        }

        public void AddDeathPack(DataStr.DeathPack Pack, string SceneName)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[AddDeathPack] called on scene {SceneName} that not exist!");
                return;
            }

            if (SceneData.m_DeathPacks.ContainsKey(Pack.m_GUID))
            {
                SceneData.m_DeathPacks.Remove(Pack.m_GUID);
            }
            SceneData.m_DeathPacks.Add(Pack.m_GUID, Pack);
        }

        public void RemoveDeathPack(string GUID, string SceneName)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[RemoveDeathPack] called on scene {SceneName} that not exist!");
                return;
            }

            if (SceneData.m_DeathPacks.ContainsKey(GUID))
            {
                SceneData.m_DeathPacks.Remove(GUID);
            }
        }

        public string GetContainerContent(string GUID, string SceneName)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[GetContainerContent] called on scene {SceneName} that not exist!");
                return "";
            }
            if (SceneData.m_Containers.ContainsKey(GUID))
            {
                return SceneData.m_Containers[GUID];
            }
            return "";
        }

        public int GetContainerState(string GUID, string SceneName)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[GetContainerState] called on scene {SceneName} that not exist!");
                return 0;
            }
            if (SceneData.m_ContainerStats.ContainsKey(GUID))
            {
                return SceneData.m_ContainerStats[GUID];
            }
            return 0;
        }

        public void SetContainerState(string GUID, int State, string SceneName)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[SetContainerState] called on scene {SceneName} that not exist!");
                return;
            }
            if (SceneData.m_ContainerStats.ContainsKey(GUID))
            {
                SceneData.m_ContainerStats[GUID] = State;
            }
            else
            {
                SceneData.m_ContainerStats.Add(GUID, State);
            }
        }

        public void SendAllOpenables(string SceneName, NetPeer Client)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[SendAllOpenables] called by Client {Client.Id} on scene {SceneName} that not exist!");
                return;
            }
            foreach (string GUID in SceneData.m_Openables.Keys.ToList())
            {
                ServerSend.SendOpenableState(Client, GUID, SceneData.m_Openables[GUID], false);
            }
        }

        public void SendAllGears(string SceneName, NetPeer Client)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[SendAllGears] called by Client {Client.Id} on scene {SceneName} that not exist!");
                return;
            }
            foreach (GearDataContainer Data in SceneData.m_Gears.Values.ToList())
            {
                ServerSend.SendGearVisual(Data.m_Visual, Client);
            }
        }

        public void SendAllDeathContainers(string SceneName, NetPeer Client)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[SendAllDeathContainers] called by Client {Client.Id} on scene {SceneName} that not exist!");
                return;
            }
            foreach (string GUID in SceneData.m_DeathPacks.Keys.ToList())
            {
                ServerSend.SendDeathPack(Client, SceneData.m_DeathPacks[GUID], m_ServerInstance);
            }
        }
        public void SendAllContainerStates(string SceneName, NetPeer Client)
        {
            SceneData SceneData = GetSceneData(SceneName);

            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[SendAllContainerStates] called by Client {Client.Id} on scene {SceneName} that not exist!");
                return;
            }
            foreach (string GUID in SceneData.m_ContainerStats.Keys.ToList())
            {
                ServerSend.SendContainerState(Client, GUID, SceneData.m_ContainerStats[GUID], m_ServerInstance);
            }
        }

        public void RemoveProp(string SceneName, string GUID)
        {
            SceneData SceneData = GetSceneData(SceneName);

            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[RemoveProp] called on scene {SceneName} that not exist!");
                return;
            }

            if (SceneData.m_Props.ContainsKey(GUID))
            {
                SceneData.m_Props.Remove(GUID);
            }
        }

        public void UseProp(string SceneName, string GUID, bool Remove = false)
        {
            SceneData SceneData = GetSceneData(SceneName);

            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[UseProp] called on scene {SceneName} that not exist!");
                return;
            }

            if (SceneData.m_Props.ContainsKey(GUID))
            {
                PropData Data = SceneData.m_Props[GUID];
                OnPropUsed(SceneName, Data);
                if (Remove)
                {
                    SceneData.m_Props.Remove(GUID);
                    List<NetPeer> peers = new List<NetPeer>();
                    m_ServerInstance.m_Instance.GetConnectedPeers(peers);
                    foreach (NetPeer Peer in peers.ToArray())
                    {
                        if (m_ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == SceneName)
                        {
                            ServerSend.SendPropRemoved(Peer, GUID);
                        }
                    }
                }
            }
        }

        public void OnPropUsed(string SceneName, PropData PropData)
        {
            if(PropData.prefabname == "CardGameTablePrefab")
            {
                PropData NewProp = new PropData();
                NewProp.prefabname = "TexasHoldEmGamePrefab";
                NewProp.frombundle = true;

                NewProp.position = PropData.position;

                NewProp.rotation = PropData.rotation;

                NewProp.guid = Guid.NewGuid().ToString();

                CardGamesManager.StartNewGame(NewProp.guid, SceneName, m_ServerInstance);

                List<NetPeer> peers = new List<NetPeer>();
                m_ServerInstance.m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Peer in peers.ToArray())
                {
                    if (m_ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == SceneName)
                    {
                        ServerSend.SendPropCreated(Peer, NewProp);
                    }
                }
            }
        }

        public void SendAllProps(string SceneName, NetPeer Client)
        {
            SceneData SceneData = GetSceneData(SceneName);

            if(SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[SendAllProps] called by Client {Client.Id} on scene {SceneName} that not exist!");
                return;
            }

            foreach (string GUID in SceneData.m_Props.Keys.ToList())
            {
                ServerSend.SendPropCreated(Client, SceneData.m_Props[GUID]);

                FallingProp FallingProp = null;

                // На случай если игрок зайдёт на сцену, когда аэр дроп уже был создан, но всё ещё падает
                if (SceneData.m_FallingProps.TryGetValue(GUID, out FallingProp))
                {
                    ServerSend.SendPropMoved(Client, FallingProp.m_GUID, SceneData.m_Props[GUID].position.ToVector(), FallingProp.m_LandPosition, FallingProp.GetVelocityPerSecond());
                }
            }
        }

        public void SkipHour()
        {
            int RealTimeSeconds = c_SecondsInHour;

            m_Time += (ulong)RealTimeSeconds;
            m_TOD += RealTimeSeconds;

            if (m_TOD > с_SecondsInCycle)
            {
                m_TOD = 0;
            }
            m_TODTimeNormalized = (float)m_TOD / с_SecondsInCycle;

            m_ElapsedInGameHours += c_SecondsInHour;
        }

        public void SkipHours(int Hours)
        {
            for (int i = 1; i <= Hours; i++)
            {
                SkipHour();
            }
        }

        public void UpdateTime()
        {
            m_Time++;

            m_TOD++;

            if(m_TOD > с_SecondsInCycle)
            {
                m_TOD = 0;
            }
            m_TODTimeNormalized = (float) m_TOD / с_SecondsInCycle;

            m_ElapsedInGameHours += (c_SecondsInHour / 60) / 60;

            ServerSend.SendTime(m_ServerInstance, m_TODTimeNormalized, m_ElapsedInGameHours);
        }

        public void UpdateEverySecond()
        {
            UpdateTime();
            foreach (SceneData Data in m_LoadedScenes.Values.ToList())
            {
                if (Data.m_ActiveZone != null)
                {
                    //Logger.Log($"[UpdateZone] Scene {Data.m_SceneName} has active zone updating it...");
                    Data.m_ActiveZone.EverySecond();
                }

                FallingProp[] FallingProps = Data.m_FallingProps.Values.ToArray();

                for (int i = FallingProps.Length-1; i >= 0; i--)
                {
                    FallingProp FallingProp = FallingProps[i];

                    if(FallingProp != null)
                    {
                        PropData PropData = null;
                        
                        if (Data.m_Props.TryGetValue(FallingProp.m_GUID, out PropData))
                        {
                            float totalDuration = (float)(FallingProp.m_LandTime - FallingProp.m_StartFallTime).TotalSeconds;
                            float elapsed = (float)(DateTime.UtcNow - FallingProp.m_StartFallTime).TotalSeconds;
                            float progress = elapsed / totalDuration;
                            Vector3 NewPosition = DataStr.Lerp(FallingProp.m_SpawnPosition, FallingProp.m_LandPosition, progress);

                            PropData.position = new Vector3JSON(NewPosition.X, NewPosition.Y, NewPosition.Z);

                            List<NetPeer> peers = new List<NetPeer>();
                            m_ServerInstance.m_Instance.GetConnectedPeers(peers);

                            if (DateTime.UtcNow > FallingProp.m_LandTime)
                            {
                                NewPosition = FallingProp.m_LandPosition;
                                foreach (NetPeer Peer in peers.ToArray())
                                {
                                    if (m_ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == Data.m_SceneName)
                                    {
                                        ServerSend.SendPropMoved(Peer, PropData.guid, NewPosition);
                                    }
                                }
                                Data.m_FallingProps.Remove(FallingProp.m_GUID);
                            }
                        }
                        else
                        {
                            Data.m_FallingProps.Remove(FallingProp.m_GUID);
                        }
                    }
                }
            }
        }

        public void ForceNextZone(bool withtime = false)
        {
            foreach (SceneData Data in m_LoadedScenes.Values.ToList())
            {
                if (Data.m_ActiveZone != null)
                {
                    Data.m_ActiveZone.ForceNextZone(withtime);
                }
            }
        }

        public void ForceZoneNoDamage()
        {
            foreach (SceneData Data in m_LoadedScenes.Values.ToList())
            {
                if (Data.m_ActiveZone != null)
                {
                    Data.m_ActiveZone.ToggleNoDamage();
                }
            }
        }

        public void ZoneRestart()
        {
            foreach (SceneData Data in m_LoadedScenes.Values.ToList())
            {
                if (Data.m_ActiveZone != null)
                {
                    Data.m_ActiveZone.Restart();
                }
            }
        }

        public void SummonAirDrop(string SceneName, string PropPrefab, string JSON, Vector3 SpawnPosition, Vector3 LandPosition, float FallingTime)
        {
            string NewGUID = System.Guid.NewGuid().ToString();
            SceneData SceneData = GetSceneData(SceneName);

            if (SceneData != null)
            {

                DataStr.PropData PropData = new PropData();
                PropData.guid = NewGUID;
                PropData.position = new Vector3JSON(SpawnPosition.X, SpawnPosition.Y, SpawnPosition.Z);
                PropData.rotation = new QuaternionJSON(0, 0, 0, 0);
                PropData.frombundle = false;
                PropData.prefabname = PropPrefab;

                AddProp(PropData, SceneName);

                AddContainer(NewGUID, DataStr.CompressString(JSON), SceneName);
                SetContainerState(NewGUID, 1, SceneName);


                DataStr.FallingProp FallingProp = new FallingProp();
                FallingProp.m_GUID = NewGUID;   
                FallingProp.m_LandTime = DateTime.UtcNow.AddSeconds(FallingTime);
                FallingProp.m_StartFallTime = DateTime.UtcNow;
                FallingProp.m_SpawnPosition = SpawnPosition;
                FallingProp.m_LandPosition = LandPosition;

                List<NetPeer> peers = new List<NetPeer>();
                m_ServerInstance.m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Peer in peers.ToArray())
                {
                    if (m_ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == SceneName)
                    {
                        ServerSend.SendPropCreated(Peer, PropData);
                        ServerSend.SendContainerState(Peer, NewGUID, 1, m_ServerInstance);
                        ServerSend.SendPropMoved(Peer, PropData.guid, SpawnPosition, LandPosition, FallingProp.GetVelocityPerSecond());
                    }
                }


                SceneData.m_FallingProps.Add(NewGUID, FallingProp);

                Logger.Log($"[SummonAirDrop] AirDrop summoned {NewGUID} on {SceneName}!");
            }
        }
    }
}
