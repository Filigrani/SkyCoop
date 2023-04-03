using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SkyCoop.DataStr;
using GameServer;
#if (!DEDICATED)
using UnityEngine;
using Il2Cpp;
#else
using System.Numerics;
#endif

namespace SkyCoop
{
    public class SafeZoneManager
    {
        public static List<string> SafeScenes = new List<string>();
        public static Dictionary<string, List<SafeZoneSpace>> SafeZones = new Dictionary<string, List<SafeZoneSpace>>();
        public static bool isReady = false;

        public class SafeZoneSpace
        {
            public Vector3 m_Center = new Vector3(0,0,0);
            public float m_Radius = 1;

            public SafeZoneSpace(Vector3 center, float radius)
            {
                m_Center = center;
                m_Radius = radius;
            }
        }

        public static void DebugRenderZones()
        {
#if (!DEDICATED)            
            List<SafeZoneSpace> Zones;
            if (SafeZones.TryGetValue(MyMod.level_guid, out Zones))
            {
                foreach (SafeZoneSpace Zone in Zones)
                {
                    GameObject Ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    UnityEngine.Object.Destroy(Ball.GetComponent<SphereCollider>());
                    Ball.transform.position = Zone.m_Center;
                    Ball.transform.localScale = new Vector3(Zone.m_Radius * 2, Zone.m_Radius * 2, Zone.m_Radius * 2);
                }
            }
#endif
        }

        public static void InitZones()
        {
            // Broken railroad
            AddSafeZone("TracksRegion", new Vector3(582.3058f, 198.95f, 560.3252f), 130);
            AddSafeScene("MaintenanceShedA");

            // Milton
            AddSafeZone("MountainTownRegion", new Vector3(1104.269f, 260.8032f, 1716.49f), 137);
            AddSafeScene("GreyMothersHouseA");

            // Inlet
            AddSafeZone("CanneryRegion", new Vector3(-389.7225f, 31.96249f, -597.4935f), 50);
            AddSafeScene("MaintenanceShedB");
            AddSafeZone("CanneryRegion", new Vector3(-71.42158f, 138.4941f, 23.34852f), 30);
            AddSafeScene("RadioControlHutB");

            //Lake
            AddSafeZone("LakeRegion", new Vector3(1671.796f, 35.21933f, 1341.215f), 70);
            AddSafeScene("Dam");
            AddSafeScene("DamTransitionZone");
            AddSafeZone("LakeRegion", new Vector3(1019.397f, 26.49471f, 444.3498f), 70);
            AddSafeScene("CampOffice");

            // Highway
            AddSafeZone("CoastalRegion", new Vector3(766.1833f, 24.32027f, 643.6183f), 50);
            AddSafeScene("QuonsetGasStation");

            // Disolation Point
            AddSafeZone("WhalingStationRegion", new Vector3(695.0294f, 38.12838f, 769.0448f), 100);
            AddSafeScene("LighthouseA");
            AddSafeZone("WhalingStationRegion", new Vector3(1032.46f, 23.80079f, 998.7527f), 70);
            AddSafeScene("WhalingShipA");

            // Plesent Valley
            AddSafeZone("RuralRegion", new Vector3(2317.808f, 53.10027f, 2243.66f), 230);
            AddSafeScene("CommunityHallA");
            AddSafeZone("RuralRegion", new Vector3(1453.856f, 48.80129f, 1028.714f), 73);
            AddSafeScene("FarmHouseA");
            AddSafeScene("FarmHouseABasement");

            //Black Rock
            AddSafeZone("BlackrockRegion", new Vector3(-185.7764f, 225.7132f, 6.317502f), 50);
            AddSafeScene("BlackrockPrisonSurvivalZone");
            AddSafeScene("BlackrockInteriorASurvival");

            // Timberwolf mountain
            AddSafeZone("CrashMountainRegion", new Vector3(888.7276f, 160.72f, 342.1175f), 50);
        }

        public static bool SceneIsSafe(string Scene)
        {
            return SafeScenes.Contains(Scene);
        }
        public static bool InsideSafeZone(string Scene, Vector3 Position)
        {
            List<SafeZoneSpace> Zones;
            if (SafeZones.TryGetValue(Scene, out Zones))
            {
                foreach (SafeZoneSpace Zone in Zones)
                {
                    if(Vector3.Distance(Position, Zone.m_Center) <= Zone.m_Radius)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public static void UpdatePlayersSafeZoneStatus()
        {
            if (!isReady)
            {
                InitZones();
                isReady = true;
            }
            int PlayerID = 0;

#if (!DEDICATED)
            if(GameManager.m_PlayerObject)
            {
                bool SafeStatus = SceneIsSafe(MyMod.level_guid) || InsideSafeZone(MyMod.level_guid, GameManager.GetPlayerTransform().position);

                if(SafeStatus != MyMod.LastSafeStatus)
                {
                    MyMod.LastSafeStatus = SafeStatus;
                    if (SafeStatus)
                    {
                        HUDMessage.AddMessage("You are entering safe zone!");
                    } else
                    {
                        HUDMessage.AddMessage("You are leaving safe zone!");
                    }
                }
            }
#endif

            foreach (MultiPlayerClientData player in MyMod.playersData)
            {
                bool SafeStatus = false;
                string MSG = "You are leaving safe zone!";
                if (SceneIsSafe(player.m_LevelGuid) || InsideSafeZone(player.m_LevelGuid, player.m_Position))
                {
                    SafeStatus = true;
                    MSG = "You are entering safe zone!";
                }

                if(player.m_IsSafe != SafeStatus)
                {
                    player.m_IsSafe = SafeStatus;
                    ServerSend.ADDHUDMESSAGE(PlayerID, MSG);
                }
                PlayerID++;
            }
        }

        public static void AddSafeZone(string Scene, Vector3 Center, float Radius)
        {
            SafeZoneSpace NewZone = new SafeZoneSpace(Center, Radius);
            List<SafeZoneSpace> Zones;
            if (!SafeZones.ContainsKey(Scene))
            {
                Zones = new List<SafeZoneSpace>();
                Zones.Add(NewZone);
                SafeZones.Add(Scene, Zones);
                return;
            } else
            {
                if (SafeZones.TryGetValue(Scene, out Zones))
                {
                    Zones.Add(NewZone);
                    SafeZones.Remove(Scene);
                    SafeZones.Add(Scene, Zones);
                }
            }
        }
        public static void AddSafeScene(string Scene)
        {
            if(!SceneIsSafe(Scene))
            {
                SafeScenes.Add(Scene);
            }
        }
    }
}
