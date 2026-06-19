using LiteNetLib;
using System;
using System.Collections.Generic;
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

        public V3Quat GetSpawnPoint(string SceneName)
        {
            SceneData SceneData = GetSceneData(SceneName);
            if (SceneData == null)
            {
                Logger.Log(ConsoleColor.Red, $"[GetSpawnPoint] called on scene {SceneName} that not exist!");
                return new V3Quat();
            }

            List<V3Quat> SpawnPoints = SceneData.m_SpawnPoints;
            if (SpawnPoints.Count > 0)
            {
                if (SpawnPoints.Count == 1)
                {
                    return SpawnPoints[0];
                }
                else
                {
                    return SpawnPoints[new Random(Guid.NewGuid().GetHashCode()).Next(0, SpawnPoints.Count)];
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
                ServerSend.SendZoneUpdate(Client, SceneData.m_ActiveZone.m_CurrentCenter, SceneData.m_ActiveZone.m_CurrentRadius, m_ServerInstance);
                ServerSend.SendTimerPrefix(Client, SceneData.m_ActiveZone.GetTimerPrefix());
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
            }
        }

        public void UpdateZone()
        {
            foreach (SceneData Data in m_LoadedScenes.Values.ToList())
            {
                if (Data.m_ActiveZone != null)
                {
                    Data.m_ActiveZone.Update();
                }
            }
        }
    }
}
