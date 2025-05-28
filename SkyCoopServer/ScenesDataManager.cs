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

        public void LoadScene(string SceneName)
        {
            if (!m_LoadedScenes.ContainsKey(SceneName))
            {
                //TODO load from file.
                SceneData sceneData = new SceneData();
                sceneData.m_SpawnPoints = FilesManager.GetSpawnPoints(m_ServerInstance.m_Config.m_GameMode, SceneName);

                DataStr.DangerCircleConfig ZoneCFG = FilesManager.GetZoneConfig(m_ServerInstance.m_Config.m_GameMode, SceneName);
                if(ZoneCFG != null)
                {
                    sceneData.m_ActiveZone = new DangerCircleData(ZoneCFG, SceneName, m_ServerInstance);
                    sceneData.m_ActiveZone.Start();
                }

                sceneData.m_SceneName = SceneName;
                m_LoadedScenes.Add(SceneName, sceneData);
            }
        }

        public void UnloadScene(string SceneName)
        {
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                //TODO flag WipeSave, to delete save file.
                SceneData Data = m_LoadedScenes[SceneName];

                if (Data != null && Data.m_ActiveZone != null)
                {
                    Data.m_ActiveZone.Dispose();
                }

                m_LoadedScenes.Remove(SceneName);
            }
        }

        public void UnloadSceneNobodyOn(Server ServerInstance)
        {
            foreach (string LoadedSceneName in m_LoadedScenes.Keys.ToArray())
            {
                bool CanUnload = true;
                foreach (NetPeer Peer in ServerInstance.m_Instance.ConnectedPeerList.ToArray())
                {
                    if (ServerInstance.GetPlayerDataByNetPeer(Peer).m_Scene == LoadedSceneName)
                    {
                        CanUnload = false;
                        break;
                    }
                }
                if (CanUnload)
                {
                    UnloadScene(LoadedSceneName);
                }
            }
        }

        public V3Quat GetSpawnPoint(string SceneName)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                List<V3Quat> SpawnPoints = m_LoadedScenes[SceneName].m_SpawnPoints;
                if(SpawnPoints.Count > 0)
                {
                    if(SpawnPoints.Count == 1)
                    {
                        return SpawnPoints[0];
                    }
                    else
                    {
                        return SpawnPoints[new Random(Guid.NewGuid().GetHashCode()).Next(0, SpawnPoints.Count)];
                    }
                }
            }
            return new V3Quat();
        }

        public void AddGear(string SceneName, GearDataContainer DataContainer)
        {
            LoadScene(SceneName);
            if(m_LoadedScenes.ContainsKey(SceneName))
            {
                m_LoadedScenes[SceneName].m_Gears.Add(DataContainer.m_Data.m_GUID, DataContainer);

                ServerSend.SendGearVisual(DataContainer.m_Visual, SceneName, m_ServerInstance);
            }
        }

        public GearDataContainer GetGear(string SceneName, string GUID, bool Remove = false)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];
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
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];
                if (SceneData.m_Openables.ContainsKey(GUID))
                {
                    SceneData.m_Openables[GUID] = OpenState;
                }
                else
                {
                    SceneData.m_Openables.Add(GUID, OpenState);
                }
            }
        }

        public void ChangeGameMode(string GameMode)
        {
            foreach (string Key in m_LoadedScenes.Keys.ToList())
            {
                if (m_LoadedScenes.ContainsKey(Key))
                {
                    m_LoadedScenes[Key].m_SpawnPoints = FilesManager.GetSpawnPoints(GameMode, Key);
                }
            }
        }

        public void SendZone(string SceneName, NetPeer Client)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];
                if (SceneData.m_ActiveZone != null)
                {
                    ServerSend.SendZoneUpdate(Client, SceneName, SceneData.m_ActiveZone.m_Config.ActualCenter, SceneData.m_ActiveZone.m_CurrentRadius, m_ServerInstance);
                }
            }
        }

        public void AddContainer(string GUID, string CompressedJSON, string SceneName)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];
                if (SceneData.m_Containers.ContainsKey(GUID))
                {
                    SceneData.m_Containers.Remove(GUID);
                }
                SceneData.m_Containers.Add(GUID, CompressedJSON);
            }
        }

        public void RemoveContainer(string GUID, string SceneName)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];
                if (SceneData.m_Containers.ContainsKey(GUID))
                {
                    SceneData.m_Containers.Remove(GUID);
                }
            }
        }

        public void AddDeathPack(DataStr.DeathPack Pack, string SceneName)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];
                if (SceneData.m_DeathPacks.ContainsKey(Pack.m_GUID))
                {
                    SceneData.m_DeathPacks.Remove(Pack.m_GUID);
                }
                SceneData.m_DeathPacks.Add(Pack.m_GUID, Pack);
            }
        }

        public void RemoveDeathPack(string GUID, string SceneName)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];
                if (SceneData.m_DeathPacks.ContainsKey(GUID))
                {
                    SceneData.m_DeathPacks.Remove(GUID);
                }
            }
        }

        public string GetContainerContent(string GUID, string SceneName)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];
                if (SceneData.m_Containers.ContainsKey(GUID))
                {
                    return SceneData.m_Containers[GUID];
                }
                return "";
            }
            return "";
        }

        public int GetContainerState(string GUID, string SceneName)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];
                if (SceneData.m_ContainerStats.ContainsKey(GUID))
                {
                    return SceneData.m_ContainerStats[GUID];
                }
                return 0;
            }
            return 0;
        }

        public void SetContainerState(string GUID, int State, string SceneName)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];
                if (SceneData.m_ContainerStats.ContainsKey(GUID))
                {
                    SceneData.m_ContainerStats[GUID] = State;
                }
            }
        }

        public void SendAllOpenables(string SceneName, NetPeer Client)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];

                foreach (string GUID in SceneData.m_Openables.Keys.ToList())
                {
                    ServerSend.SendOpenableState(Client, GUID, SceneData.m_Openables[GUID], false);
                }
            }
        }

        public void SendAllGears(string SceneName, NetPeer Client)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];

                foreach (GearDataContainer Data in SceneData.m_Gears.Values.ToList())
                {
                    ServerSend.SendGearVisual(Data.m_Visual, Client);
                }
            }
        }

        public void SendAllDeathContainers(string SceneName, NetPeer Client)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];

                foreach (string GUID in SceneData.m_DeathPacks.Keys.ToList())
                {
                    ServerSend.SendDeathPack(Client, SceneData.m_DeathPacks[GUID], m_ServerInstance);
                }
            }
        }
        public void SendAllContainerStates(string SceneName, NetPeer Client)
        {
            LoadScene(SceneName);
            if (m_LoadedScenes.ContainsKey(SceneName))
            {
                SceneData SceneData = m_LoadedScenes[SceneName];

                foreach (string GUID in SceneData.m_ContainerStats.Keys.ToList())
                {
                    ServerSend.SendContainerState(Client, GUID, SceneData.m_ContainerStats[GUID], m_ServerInstance);
                }
            }
        }
    }
}
