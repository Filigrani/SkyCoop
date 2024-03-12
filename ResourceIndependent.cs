using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using GameServer;
using static SkyCoop.DataStr;
#if (DEDICATED)
using TinyJSON;
using System.Numerics;
#else
using MelonLoader.TinyJSON;
using UnityEngine;
#endif

namespace SkyCoop
{
    public class ResourceIndependent
    {
        public static Dictionary<string, string> Templates = new Dictionary<string, string>();
        public static bool IsInit = false;
        public static DataStr.BufferedGearSpawners GearSpawners = new DataStr.BufferedGearSpawners();
        public static bool GearSpawnersReady = false;

        public static void Log(string TXT, Shared.LoggerColor Color = Shared.LoggerColor.White)
        {
#if (DEDICATED)
            Logger.Log(TXT, Color);
#endif
        }

        public static void LoadGearSpawners()
        {
            if (GearSpawnersReady)
            {
                return;
            }

            string JSONString = "";
            if (Directory.Exists(MPSaveManager.GetBaseDirectory() + "Mods") && File.Exists(MPSaveManager.GetBaseDirectory() + "Mods" + MPSaveManager.GetSeparator() + "IndependantGearSpawners.json"))
            {
                byte[] FileData = File.ReadAllBytes(MPSaveManager.GetBaseDirectory() + "Mods" + MPSaveManager.GetSeparator() + "IndependantGearSpawners.json");
                JSONString = UTF8Encoding.UTF8.GetString(FileData);
                if (!string.IsNullOrEmpty(JSONString))
                {
                    JSONString = MPSaveManager.VectorsFixUp(JSONString);
                }
            } else
            {
                if (File.Exists(MPSaveManager.GetBaseDirectory() + "IndependantGearSpawners.json"))
                {
                    byte[] FileData = File.ReadAllBytes(MPSaveManager.GetBaseDirectory() + "IndependantGearSpawners.json");
                    JSONString = UTF8Encoding.UTF8.GetString(FileData);
                    if (!string.IsNullOrEmpty(JSONString))
                    {
                        JSONString = MPSaveManager.VectorsFixUp(JSONString);
                    }
                }
            }
            if (!string.IsNullOrEmpty(JSONString))
            {
                GearSpawners = JSON.Load(JSONString).Make<DataStr.BufferedGearSpawners>();
                Log("IndependantGearSpawners loaded!", Shared.LoggerColor.Green);
                GearSpawnersReady = true;
            }
        }

        public static void Init()
        {
            if (IsInit)
            {
                return;
            }
            string BasicBlank = @"{""m_Position"":[],""m_Rotation"":[],""m_InstanceIDProxy"":-1,""m_CurrentHPProxy"":100,""m_NormalizedCondition"":1,""m_RolledSpawnChanceProxy"":true,""m_WeightKG"":0.1,""m_GearItemSaveVersion"":4,""m_InspectSerialized"":""{}""}";
            string Key = @"{""m_Position"":[],""m_Rotation"":[],""m_InstanceIDProxy"":-1,""m_CurrentHPProxy"":100,""m_NormalizedCondition"":1,""m_RolledSpawnChanceProxy"":true,""m_ObjectGuidSerialized"":""#KEYNAME_#KEYSEED"",""m_WeightKG"":0.1,""m_GearItemSaveVersion"":4,""m_InspectSerialized"":""{}""}";
            AddTemplate("gear_sclockpick", BasicBlank);
            AddTemplate("gear_scmetalblanksmall", BasicBlank);
            AddTemplate("gear_scdoorkeytemp", BasicBlank);
            AddTemplate("gear_scdoorkeyleadtemp", BasicBlank);
            AddTemplate("gear_scdoorkey", Key);
            AddTemplate("gear_scdoorkeylead", Key);
            AddTemplate("gear_rabbitcarcass", @"{""m_Position"":[],""m_Rotation"":[],""m_InstanceIDProxy"":-1,""m_CurrentHPProxy"":9.693987,""m_NormalizedCondition"":0.9693987,""m_RolledSpawnChanceProxy"":true,""m_WeightKG"":3,""m_BodyHarvestSerialized"":""{\""m_MeatAvailableKG\"":#KGVAL,\""m_HideAvailableUnits\"":1,\""m_GutAvailableUnits\"":1,\""m_Condition\"":96.93987,\""m_HoursPlayed\"":5.815399,\""m_QuarterBagWasteMultiplier\"":1,\""m_DamageSide\"":\""DamageSideLeft\""}"",""m_GearItemSaveVersion"":4,""m_InspectSerialized"":""{}""}");
            AddTemplate("gear_snare_3", @"{""m_HoursPlayed"":0,""m_Position"":[],""m_Rotation"":[],""m_InstanceIDProxy"":-1,""m_CurrentHPProxy"":100,""m_NormalizedCondition"":1,""m_BeenInPlayerInventoryProxy"":true,""m_BeenInspectedProxy"":true,""m_HasBeenOwnedByPlayer"":true,""m_SnareItemSerialized"":""{\""m_HoursPlayed\"":0,\""m_HoursAtLastRoll\"":0,\""m_State\"":\""WithRabbit\""}"",""m_WeightKG"":0.3,""m_GearItemSaveVersion"":4,""m_InspectSerialized"":""{}""}");
            AddTemplate("gear_snare_2", @"{""m_HoursPlayed"":0,""m_Position"":[],""m_Rotation"":[],""m_InstanceIDProxy"":-1,""m_CurrentHPProxy"":100,""m_NormalizedCondition"":1,""m_BeenInPlayerInventoryProxy"":true,""m_BeenInspectedProxy"":true,""m_HasBeenOwnedByPlayer"":true,""m_SnareItemSerialized"":""{\""m_HoursPlayed\"":0,\""m_HoursAtLastRoll\"":0,\""m_State\"":\""Broken\""}"",""m_WeightKG"":0.3,""m_GearItemSaveVersion"":4,""m_InspectSerialized"":""{}""}");
            AddTemplate("gear_scwoodform", BasicBlank);
            AddTemplate("gear_scwoodformb", BasicBlank);
            LoadGearSpawners();
            IsInit = true;
        }


        public static void AddTemplate(string GearName, string JsonTemplate)
        {
            if(!Templates.ContainsKey(GearName))
            {
                Templates.Add(GearName, JsonTemplate);
            } 
        }

        public static string OverrideTransform(string JSON, Vector3 pos, Quaternion rot)
        {
#if (DEDICATED)
            string Pos = "[" +pos.X.ToString(CultureInfo.InvariantCulture) + "," + pos.Y.ToString(CultureInfo.InvariantCulture) + "," + pos.Z.ToString(CultureInfo.InvariantCulture) + "]";
            string Rot = "[" + rot.X.ToString(CultureInfo.InvariantCulture) + "," + rot.Y.ToString(CultureInfo.InvariantCulture) + "," + rot.Z.ToString(CultureInfo.InvariantCulture) + "," + rot.W.ToString(CultureInfo.InvariantCulture) + "]";

            JSON = JSON.Replace(@"""m_Position"":[]", @"""m_Position"":" + Pos);
            JSON = JSON.Replace(@"""m_Rotation"":[]", @"""m_Rotation"":" + Rot);
#else
            string Pos = "[" + pos.x.ToString(CultureInfo.InvariantCulture) + "," + pos.y.ToString(CultureInfo.InvariantCulture) + "," + pos.z.ToString(CultureInfo.InvariantCulture) + "]";
            string Rot = "[" + rot.x.ToString(CultureInfo.InvariantCulture) + "," + rot.y.ToString(CultureInfo.InvariantCulture) + "," + rot.z.ToString(CultureInfo.InvariantCulture) + "," + rot.w.ToString(CultureInfo.InvariantCulture) + "]";

            JSON = JSON.Replace(@"""m_Position"":[]", @"""m_Position"":" + Pos);
            JSON = JSON.Replace(@"""m_Rotation"":[]", @"""m_Rotation"":" + Rot);
#endif

            return JSON;
        }
        public static string OverrideKey(string JSON, string KeyName, string KeySeed)
        {
            string GUID = KeyName + "_" + KeySeed;
            JSON = JSON.Replace("#KEYNAME_#KEYSEED", GUID);

            return JSON;
        }

        public static string GetRabbit(Vector3 pos, Quaternion rot)
        {
            string JSON;
            if (Templates.TryGetValue("gear_rabbitcarcass", out JSON))
            {
                JSON = OverrideTransform(JSON, pos, rot);

                JSON = JSON.Replace("#KGVAL", Shared.GetBodyHarvestUnits("WILDLIFE_Rabbit").m_Meat.ToString(CultureInfo.InvariantCulture));
                Log("gear_rabbitcarcass JSON:");
                Log(JSON);

                return JSON;
            } else
            {
                return "";
            }
        }
        public static string GetSnare(Vector3 pos, Quaternion rot, int State)
        {
            string JSON;
            if (Templates.TryGetValue("gear_snare_"+ State, out JSON))
            {
                JSON = OverrideTransform(JSON, pos, rot);
                Log("gear_snare JSON:");
                Log(JSON);

                return JSON;
            } else
            {
                return "";
            }
        }

        public static string GetLocksmithGear(string GearName, Vector3 pos, Quaternion rot, string KeyName = "", string KeySeed = "")
        {
            GearName = GearName.ToLower();
            string JSON;
            if(Templates.TryGetValue(GearName, out JSON))
            {
                JSON = OverrideTransform(JSON, pos, rot);
                if(!string.IsNullOrEmpty(KeyName) && !string.IsNullOrEmpty(KeySeed))
                {
                    JSON = OverrideKey(JSON, KeyName, KeySeed);
                }
                Log(GearName + " JSON:");
                Log(JSON);

                return JSON;
            } else{
                return "";
            }
        }
        public static void SpawnAllGearsOnScene(string Scene, string LayerAffix = "", string PrefabOverride = "")
        {
            LoadGearSpawners();
            if (GearSpawnersReady)
            {
                SceneGearSpawners SceneData;
                string EntryName = Scene;
                if (!string.IsNullOrEmpty(LayerAffix))
                {
                    EntryName = Scene + "_" + LayerAffix;
                }
                if (GearSpawners.GearSpawners.TryGetValue(EntryName, out SceneData))
                {
                    foreach (PrefabSpawnDescriptor item in SceneData.PrefabSpawns)
                    {
                        foreach (GearSpawnerElement gear in item.Gears)
                        {
                            string Prefab = PrefabOverride;
                            if (string.IsNullOrEmpty(Prefab))
                            {
                                Prefab = gear.GearName;
                            }
                            SpawnGear(Prefab, Scene, gear.Position, gear.Rotation, 0, "PrefabSpawnDescriptor");
                        }
                    }
                    foreach (DataStr.RandomSpawnObjectDescriptor item in SceneData.RandomSpawns)
                    {
                        foreach (DataStr.GearSpawnerElement gear in item.Gears)
                        {
                            string Prefab = PrefabOverride;
                            if (string.IsNullOrEmpty(Prefab))
                            {
                                Prefab = gear.GearName;
                            }
                            SpawnGear(Prefab, Scene, gear.Position, gear.Rotation, 0, "RandomSpawnObjectDescriptor");
                        }
                    }
                }
            }
        }

        public static List<int> CalculateSetWeights(List<GearSpawnerElement> PrefabList, out int sumOfAllWeights)
        {
            sumOfAllWeights = 0;
            List<int> setWeights = new List<int>();
            for (int i = 0; i < PrefabList.Count; i++)
            {
                GearSpawnerElement prefab = PrefabList[i];
                setWeights.Add(prefab.Weight);
                sumOfAllWeights += prefab.Weight;

            }
            return setWeights;
        }

        public static void SpawnSomeGearsOnScene(string Scene, string LayerAffix = "", string PrefabOverride = "")
        {
            LoadGearSpawners();
            if (GearSpawnersReady)
            {
                SceneGearSpawners SceneData;
                string EntryName = Scene;
                if (!string.IsNullOrEmpty(LayerAffix))
                {
                    EntryName = Scene + "_" + LayerAffix;
                }
                System.Random RNG = new System.Random();
                if (GearSpawners.GearSpawners.TryGetValue(EntryName, out SceneData))
                {
                    foreach (PrefabSpawnDescriptor item in SceneData.PrefabSpawns)
                    {
                        int Rolled = RNG.Next(0, 100);
                        if (item.ChanceOfNoSpawn < Rolled)
                        {
                            continue;
                        }

                        int Amount = RNG.Next(item.Min, item.Max);
                        if (item.Min == 0 || item.Max == 0 || Amount == 0)
                        {
                            continue;
                        }
                        List<int> IndexesToSpawn = new List<int>();
                        int FailAttemps = 20;
                        while (Amount != IndexesToSpawn.Count)
                        {
                            if(FailAttemps == 0)
                            {
                                break;
                            } else
                            {
                                int RandomIndex = RNG.Next(0, item.Gears.Count);
                                if (!IndexesToSpawn.Contains(RandomIndex))
                                {
                                    IndexesToSpawn.Add(RandomIndex);
                                } else
                                {
                                    FailAttemps--;
                                }
                            }
                        }

                        foreach (int GearIndex in IndexesToSpawn)
                        {
                            GearSpawnerElement gear = item.Gears[GearIndex];
                            string Prefab = PrefabOverride;
                            if (string.IsNullOrEmpty(Prefab))
                            {
                                Prefab = gear.GearName;
                            }
                            SpawnGear(Prefab, Scene, gear.Position, gear.Rotation);
                        }
                    }
                    foreach (DataStr.RandomSpawnObjectDescriptor item in SceneData.RandomSpawns)
                    {
                        int Amount = 0;
                        if (Shared.IsPilgrimExpereinceMode())
                        {
                            Amount = item.SpawnOnPiligrim;
                        } else if(Shared.IsVoyageurExpereinceMode())
                        {
                            Amount = item.SpawnOnVoyageur;
                        } else if (Shared.IsStalkerExpereinceMode())
                        {
                            Amount = item.SpawnOnStalker;
                        } else if (Shared.IsInterloperExpereinceMode())
                        {
                            Amount = item.SpawnOnInterloper;
                        }

                        List<int> IndexesToSpawn = new List<int>();
                        int FailAttemps = 20;
                        while (Amount != IndexesToSpawn.Count)
                        {
                            if (FailAttemps == 0)
                            {
                                break;
                            } else
                            {
                                int RandomIndex = RNG.Next(0, item.Gears.Count);
                                if (!IndexesToSpawn.Contains(RandomIndex))
                                {
                                    IndexesToSpawn.Add(RandomIndex);
                                } else
                                {
                                    FailAttemps--;
                                }
                            }
                        }
                        foreach (int GearIndex in IndexesToSpawn)
                        {
                            GearSpawnerElement gear = item.Gears[GearIndex];
                            int Rolled = RNG.Next(0, 100);
                            if(Rolled < gear.Chance)
                            {
                                string Prefab = PrefabOverride;
                                if (string.IsNullOrEmpty(Prefab))
                                {
                                    Prefab = gear.GearName;
                                }
                                SpawnGear(Prefab, Scene, gear.Position, gear.Rotation, 0);
                            }
                        }
                    }
                }
            }
        }

        public static void SpawnGear(string Prefab, string Scene, Vector3 PlaceV3, Quaternion Rotation, int Variant = 0, string DroppedName = "", string PhotoGUID = "", string ExpeditionNote = "")
        {
            if (string.IsNullOrEmpty(Prefab))
            {
                return;
            }
            SlicedJsonDroppedGear NewGear = new SlicedJsonDroppedGear();
            NewGear.m_GearName = Prefab.ToLower();
            NewGear.m_Extra.m_DroppedTime = MyMod.MinutesFromStartServer;
            NewGear.m_Extra.m_Dropper = DroppedName;
            NewGear.m_Extra.m_GearName = NewGear.m_GearName;
            NewGear.m_Extra.m_Variant = Variant;
            NewGear.m_Extra.m_PhotoGUID = PhotoGUID;
            NewGear.m_Extra.m_ExpeditionNote = ExpeditionNote;

            int hashV3 = Shared.GetVectorHash(PlaceV3);
            int hashRot = Shared.GetQuaternionHash(Rotation);
            int hashLevelKey = Scene.GetHashCode();
            int SearchKey = hashV3 + hashRot + hashLevelKey;

            DroppedGearItemDataPacket GearVisual = new DroppedGearItemDataPacket();
            GearVisual.m_Extra = NewGear.m_Extra;
            GearVisual.m_GearID = -1;
            GearVisual.m_Hash = SearchKey;
            GearVisual.m_LevelGUID = Scene;
            GearVisual.m_Position = PlaceV3;
            GearVisual.m_Rotation = Rotation;
            NewGear.m_Json = "";
            Dictionary<int, DroppedGearItemDataPacket> Dic = MPSaveManager.LoadDropVisual(Scene);

            if (Dic != null && Dic.ContainsKey(SearchKey))
            {
                return;
            }
            MPSaveManager.AddGearData(Scene, SearchKey, NewGear);
            MPSaveManager.AddGearVisual(Scene, GearVisual);
#if (!DEDICATED)
            Shared.FakeDropItem(GearVisual, true);
#endif
            ServerSend.DROPITEM(0, GearVisual, true);
        }
    }
}
