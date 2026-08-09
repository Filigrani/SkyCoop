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

        public struct AddedGearData
        {
            public string GUID;
            public string FireGUID;

            public AddedGearData(string guid,  string fireguid)
            {
                GUID = guid;
                FireGUID = fireguid;
            }
        }

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

        public AddedGearData AddGear(string SceneName, GearDataContainer DataContainer)
        {
            bool IsCooking = false;
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[AddGear] called on scene {SceneName} that not exist!");
                return new AddedGearData("", "");
            }

            if(!string.IsNullOrEmpty(DataContainer.m_Visual.m_FireGUID) && DataContainer.m_Visual.m_CookingSlot != -1)
            {
                IsCooking = SetGearForCooking(SceneName, DataContainer.m_Visual);

                // Такова не должно быть, но просто подстраховка
                if (!IsCooking)
                {
                    DataContainer.m_Visual.SetCookingSlot("", -1);
                }
            }

            SceneData.m_Gears.Add(DataContainer.m_Data.m_GUID, DataContainer);

            ServerSend.SendGearVisual(DataContainer.m_Visual, SceneName, m_ServerInstance);
            return new AddedGearData(DataContainer.m_Data.m_GUID, DataContainer.m_Visual.m_FireGUID);
        }

        public void RemoveGear(string SceneName, string GUID)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[RemoveGear] called on scene {SceneName} that not exist!");
                return;
            }
            if (SceneData.m_Gears.ContainsKey(GUID))
            {
                GearDataContainer Data = null;

                if (SceneData.m_Gears.TryGetValue(GUID, out Data))
                {
                    if (Data.m_Visual.m_HasCookingSlot)
                    {
                        FireSyncData FireData = null;

                        if (SceneData.m_Fires.TryGetValue(Data.m_Visual.m_FireGUID, out FireData))
                        {
                            if (FireData != null)
                            {
                                FireData.ClearCookingSlot(Data.m_Visual.m_CookingSlot);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(Data.m_Visual.m_CookpotGUID))
                    {
                        GearDataContainer CookpotData = null;

                        if (SceneData.m_Gears.TryGetValue(Data.m_Visual.m_CookpotGUID, out CookpotData))
                        {
                            if (CookpotData != null)
                            {
                                CookpotData.m_Visual.m_ProductGUID = "";
                                CookpotData.m_Visual.SetRecipe("", 0, 0);
                                ServerSend.SendGearVisual(CookpotData.m_Visual, SceneName, m_ServerInstance);
                            }
                        }
                        Data.m_Visual.m_CookpotGUID = "";
                    }

                    if (!string.IsNullOrEmpty(Data.m_Visual.m_ProductGUID))
                    {
                        GearDataContainer ProductData = null;

                        if (SceneData.m_Gears.TryGetValue(Data.m_Visual.m_ProductGUID, out ProductData))
                        {
                            if (ProductData != null)
                            {
                                ProductData.m_Visual.m_CookpotGUID = "";
                                ServerSend.SendGearVisual(ProductData.m_Visual, SceneName, m_ServerInstance);
                            }
                        }
                        Data.m_Visual.m_ProductGUID = "";
                    }

                    SceneData.m_Gears.Remove(GUID);
                    ServerSend.SendGearRemoved(GUID, SceneName, m_ServerInstance);
                }
            }
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
                GearDataContainer Data = null;

                if(SceneData.m_Gears.TryGetValue(GUID, out Data))
                {
                    if (Remove)
                    {
                        RemoveGear(SceneName, GUID);
                    }
                }


                return Data;
            }
            return null;
        }

        public AddedGearData AddGear(string SceneName, string GearName, Vector3 Position, Quaternion Rotation, string JSON, float Condition, int Style, string FireGUID = "", int CookingSlot = -1, string RecipeResult = "", float Volume = 0, float TimeBeingCooked = 0, string CookpotGUID = "")
        {
            string NewGUID = Guid.NewGuid().ToString();

            GearDataContainer DataContainer = new GearDataContainer();

            DataContainer.m_Data.m_GUID = NewGUID;
            DataContainer.m_Data.m_JSON = JSON;

            DataContainer.m_Visual.m_Style = Style;
            DataContainer.m_Visual.m_ConditionNormalized = Condition;
            DataContainer.m_Visual.m_GUID = NewGUID;
            DataContainer.m_Visual.m_GearName = GearName;
            DataContainer.m_Visual.m_Position = Position;
            DataContainer.m_Visual.m_Rotation = Rotation;
            DataContainer.m_Visual.SetCookingSlot(FireGUID, CookingSlot);
            DataContainer.m_Visual.SetRecipe(RecipeResult, Volume, TimeBeingCooked);

            if (!string.IsNullOrEmpty(CookpotGUID))
            {
                GearDataContainer CookingPot = GetGear(SceneName, CookpotGUID);

                if(CookingPot != null)
                {
                    DataContainer.m_Visual.m_CookpotGUID = CookpotGUID;
                    CookingPot.m_Visual.m_ProductGUID = NewGUID;

                    CookingPot.m_Visual.SetRecipe(GearName, Volume, TimeBeingCooked);

                    ServerSend.SendGearVisual(CookingPot.m_Visual, SceneName, m_ServerInstance);
                }
            }


            return AddGear(SceneName, DataContainer);
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

        public void UpdateEverySecond()
        {
            List<NetPeer> peers = new List<NetPeer>();
            m_ServerInstance.m_Instance.GetConnectedPeers(peers);

            float CurrentTime = m_ServerInstance.m_Timeline.m_ElapsedInGameHours;

            foreach (SceneData Data in m_LoadedScenes.Values.ToList())
            {
                if (Data.m_ActiveZone != null)
                {
                    //Logger.Log($"[UpdateZone] Scene {Data.m_SceneName} has active zone updating it...");
                    Data.m_ActiveZone.EverySecond();
                }

                foreach (FireSyncData FireData in Data.m_Fires.Values)
                {
                    if(FireData.m_FireState == 6)
                    {
                        float ElapsedHoursFromLastUpdate = CurrentTime - FireData.m_LastUpdateTime;
                        float ElapsedSecondsFromLastUpdate = (ElapsedHoursFromLastUpdate * 60) * 60;

                        // Сколько огню осталось ещё гореть после последнего обновления
                        float RemaningSecondsFireCanBeActive = (FireData.m_MaxOnTODSeconds + FireSyncData.c_EmbersDuration)- FireData.m_ElapsedOnTODSeconds;

                        // Сколько огонь по факту горел. Мы же загружаем сцену, и могло пройти очень много времени нужно знать сколько из этого времени огонь горел
                        float ElapsedSecondsWithFireActive;

                        if (ElapsedSecondsFromLastUpdate > RemaningSecondsFireCanBeActive)
                        {
                            ElapsedSecondsWithFireActive = RemaningSecondsFireCanBeActive;
                        }
                        else
                        {
                            ElapsedSecondsWithFireActive = ElapsedSecondsFromLastUpdate;
                        }
                        float ElapseHoursWithFireActive = (ElapsedSecondsWithFireActive / 60f) / 60f;


                        FireData.m_ElapsedOnTODSeconds += ElapsedSecondsFromLastUpdate;
                        //Logger.Log(ConsoleColor.Green, $"ElapsedSecondsWithFireActive {ElapsedSecondsWithFireActive}");
                        //Logger.Log(ConsoleColor.Green, $"ElapseHoursWithFireActive {ElapseHoursWithFireActive}");
                        if (ElapseHoursWithFireActive > 0)
                        {
                            foreach (CookingSlotData Slot in FireData.m_CookingSlots)
                            {
                                if (!string.IsNullOrEmpty(Slot.m_GearGUID))
                                {
                                    GearDataContainer GearData = null;

                                    if (Data.m_Gears.TryGetValue(Slot.m_GearGUID, out GearData))
                                    {
                                        GearData.m_Visual.AddCookingHours(ElapseHoursWithFireActive);
                                        //Logger.Log(ConsoleColor.Green, $"{GearData.m_Visual.m_GearName} {GearData.m_Visual.m_CookingResult} {GearData.m_Visual.m_BeingCookedTime}");

                                        bool SendProgress = true;

                                        if (GearData.m_Visual.m_CookingResult == "BadWater")
                                        {
                                            float TimeToCook = (37.5f / 60f) * GearData.m_Visual.m_Volume;

                                            if (GearData.m_Visual.m_BeingCookedTime > TimeToCook)
                                            {
                                                float Overcooked = GearData.m_Visual.m_BeingCookedTime - TimeToCook;

                                                GearData.m_Visual.m_CookingResult = "GoodWater";

                                                GearData.m_Visual.m_BeingCookedTime = Overcooked;
                                                ServerSend.SendGearVisual(GearData.m_Visual, Data.m_SceneName, m_ServerInstance);
                                                SendProgress = false;
                                            }
                                        }

                                        if (SendProgress)
                                        {
                                            ServerSend.SendGearCookingUpdate(Slot.m_GearGUID, GearData.m_Visual.m_BeingCookedTime, Data.m_SceneName, m_ServerInstance);
                                        }
                                    }
                                }
                            }
                        }



                        if(FireData.m_Heat < FireData.m_FuelHeatIncrease)
                        {
                            float heatToAdd = FireData.m_FuelHeatIncrease / FireData.m_TimeToReachMaxTempInSeconds * ElapsedSecondsFromLastUpdate;

                            FireData.m_Heat += heatToAdd;

                            if(FireData.m_Heat > FireData.m_FuelHeatIncrease)
                            {
                                FireData.m_Heat = FireData.m_FuelHeatIncrease;
                            }
                        }else if(FireData.m_Heat > FireData.m_FuelHeatIncrease)
                        {
                            FireData.m_Heat = FireData.m_FuelHeatIncrease;
                        }

                        if(FireData.RemaningTime() <= 0)
                        {
                            if (!FireData.m_EmbersActive)
                            {
                                float EmbersTimeRemaning = FireData.m_MaxOnTODSeconds + FireSyncData.c_EmbersDuration - FireData.m_ElapsedOnTODSeconds;

                                // Если времени промотанно больше чем вся длительность золы, скипаем её просто.
                                if (EmbersTimeRemaning <= 0)
                                {
                                    FireData.Unlit(Data.m_SceneName, m_ServerInstance);
                                }
                                else
                                {
                                    FireData.m_EmbersActive = true;
                                    FireData.m_EmberTimer = EmbersTimeRemaning;
                                }
                            }
                            else
                            {
                                if(FireData.m_EmberTimer > 0)
                                {
                                    FireData.m_EmberTimer -= ElapsedSecondsFromLastUpdate;

                                    if(FireData.m_EmberTimer <= 0)
                                    {
                                        FireData.Unlit(Data.m_SceneName, m_ServerInstance);
                                    }
                                }
                            }
                        }
                        FireData.m_LastUpdateTime = CurrentTime;
                    }
                    
                    
                    foreach (NetPeer Peer in peers.ToArray())
                    {
                        if (m_ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == Data.m_SceneName)
                        {
                            ServerSend.SendFire(Peer, FireData);
                        }
                    }
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


        public void AddFire(DataStr.FireSyncData FireData, string SceneName)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[AddFire] called on scene {SceneName} that not exist!");
                return;
            }

            if (SceneData.m_Fires.ContainsKey(FireData.m_GUID))
            {
                SceneData.m_Fires.Remove(FireData.m_GUID);
            }
            Logger.Log(ConsoleColor.Magenta, $"Created fire GUID {FireData.m_GUID} Dynamic {FireData.m_IsDynamic}");
            SceneData.m_Fires.Add(FireData.m_GUID, FireData);

            List<NetPeer> peers = new List<NetPeer>();
            m_ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers.ToArray())
            {
                if (m_ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == SceneName)
                {
                    ServerSend.SendFire(Peer, FireData);
                }
            }
        }

        public FireSyncData GetFire(string GUID, string SceneName)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[GetFire] called on scene {SceneName} that not exist!");
                return null;
            }

            FireSyncData TempFire = null;

            if (SceneData.m_Fires.TryGetValue(GUID, out TempFire))
            {
                return TempFire;
            }
            return null;
        }

        public int RemoveFire(string SceneName, string GUID)
        {
            int Charcoal = 0;
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[RemoveFire] called on scene {SceneName} that not exist!");
                return Charcoal;
            }

            FireSyncData FireData = null;

            if (SceneData.m_Fires.TryGetValue(GUID, out FireData))
            {
                Charcoal = FireData.TakeCharcoal();


                foreach (CookingSlotData Slot in FireData.m_CookingSlots)
                {
                    if(Slot != null)
                    {
                        if (!string.IsNullOrEmpty(Slot.m_GearGUID))
                        {
                            GearDataContainer GearData = null;

                            if(SceneData.m_Gears.TryGetValue(Slot.m_GearGUID, out GearData))
                            {
                                GearData.m_Visual.SetCookingSlot("", -1);
                            }
                        }
                    }
                }


                SceneData.m_Fires.Remove(GUID);

                List<NetPeer> peers = new List<NetPeer>();
                m_ServerInstance.m_Instance.GetConnectedPeers(peers);
                foreach (NetPeer Peer in peers.ToArray())
                {
                    if (m_ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == SceneName)
                    {
                        ServerSend.SendRemoveCampfire(Peer, GUID);
                    }
                }
            }
            return Charcoal;
        }

        public void AddFuel(string SceneName, string GUID, float Fuel, float Heat, float InnerRadius, float OutterRadius)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[AddFuel] called on scene {SceneName} that not exist!");
                return;
            }
            DataStr.FireSyncData Fire = null;

            if (SceneData.m_Fires.TryGetValue(GUID, out Fire))
            {
                if (Fire != null)
                {
                    Fire.AddFuel(Fuel, Heat, InnerRadius, OutterRadius);
                }
            }
            else
            {
                return;
            }

            List<NetPeer> peers = new List<NetPeer>();
            m_ServerInstance.m_Instance.GetConnectedPeers(peers);
            foreach (NetPeer Peer in peers.ToArray())
            {
                if (m_ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == SceneName)
                {
                    ServerSend.SendAddFuel(Peer, GUID);
                }
            }
        }

        public bool TakeTorch(string SceneName, string GUID)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[TakeTorch] called on scene {SceneName} that not exist!");
                return false;
            }
            DataStr.FireSyncData Fire = null;

            if (SceneData.m_Fires.TryGetValue(GUID, out Fire))
            {
                if (Fire != null)
                {
                    return Fire.TakeTorch();
                }
            }
            return false;
        }

        public int TakeCharcoal(string SceneName, string GUID)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[TakeTorch] called on scene {SceneName} that not exist!");
                return 0;
            }
            DataStr.FireSyncData Fire = null;

            if (SceneData.m_Fires.TryGetValue(GUID, out Fire))
            {
                if (Fire != null)
                {
                    return Fire.TakeCharcoal();
                }
            }
            return 0;
        }

        public int GetFreeCookingSlot(string SceneName, string GUID)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[GetFreeCookingSlot] called on scene {SceneName} that not exist!");
                return -1;
            }
            DataStr.FireSyncData Fire = null;

            if (SceneData.m_Fires.TryGetValue(GUID, out Fire))
            {
                if (Fire != null)
                {
                    return Fire.GetFreeCookingSlot();
                }
            }
            return -1;
        }

        public bool CookingSlotIsFree(string SceneName, string GUID, int CookingSlotIndex)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[CookingSlotIsFree] called on scene {SceneName} that not exist!");
                return false;
            }
            DataStr.FireSyncData Fire = null;

            if (SceneData.m_Fires.TryGetValue(GUID, out Fire))
            {
                if (Fire != null)
                {
                    return Fire.CheckSlotIsFree(CookingSlotIndex);
                }
            }
            return false;
        }

        public bool SetGearForCooking(string SceneName, GearDataVisual GearVisual)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[SetGearForCooking] called on scene {SceneName} that not exist!");
                return false;
            }
            DataStr.FireSyncData Fire = null;

            if (SceneData.m_Fires.TryGetValue(GearVisual.m_FireGUID, out Fire))
            {
                if (Fire != null)
                {
                    return Fire.SetGearForCooking(GearVisual, GearVisual.m_CookingSlot);
                }
            }
            return false;
        }
    }
}
