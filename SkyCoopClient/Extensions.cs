using Harmony;
using Il2Cpp;
using Il2CppTLD.Gear;
using LiteNetLib.Utils;
using SkyCoopClient;
using SkyCoopServer;
using System.Numerics;
using UnityEngine;
using static Il2Cpp.Utils;

namespace SkyCoop
{
    public static class Extensions
    {
        public static void Put(this NetDataWriter Writer, UnityEngine.Quaternion quat)
        {
            Writer.Put(quat.x);
            Writer.Put(quat.y);
            Writer.Put(quat.z);
            Writer.Put(quat.w);
        }

        public static void Put(this NetDataWriter Writer, UnityEngine.Vector3 v3)
        {
            Writer.Put(v3.x);
            Writer.Put(v3.y);
            Writer.Put(v3.z);
        }

        public static void Put(this NetDataWriter Writer, UnityEngine.Vector2 v2)
        {
            Writer.Put(v2.x);
            Writer.Put(v2.y);
        }

        public static UnityEngine.Vector3 GetVector3Unity(this NetDataReader Reader)
        {
            UnityEngine.Vector3 v3 = new UnityEngine.Vector3(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
            return v3;
        }

        public static UnityEngine.Vector2 GetVector2Unity(this NetDataReader Reader)
        {
            UnityEngine.Vector2 v2 = new UnityEngine.Vector2(Reader.GetFloat(), Reader.GetFloat());
            return v2;
        }

        public static UnityEngine.Quaternion GetQuaternionUnity(this NetDataReader Reader)
        {
            UnityEngine.Quaternion quat = new UnityEngine.Quaternion(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
            return quat;
        }

        public static UnityEngine.Vector3 ConvertToUnity(this System.Numerics.Vector3 V3)
        {
            UnityEngine.Vector3 Vector = new UnityEngine.Vector3(V3.X, V3.Y, V3.Z);
            return Vector;
        }
        public static UnityEngine.Quaternion ConvertToUnity(this System.Numerics.Quaternion Quat)
        {
            UnityEngine.Quaternion Quaternion = new UnityEngine.Quaternion(Quat.X, Quat.Y, Quat.Z, Quat.W);
            return Quaternion;
        }

        public static System.Numerics.Vector3 ConvertToSystem(this UnityEngine.Vector3 V3)
        {
            System.Numerics.Vector3 Vector = new System.Numerics.Vector3(V3.x, V3.y, V3.z);
            return Vector;
        }
        public static System.Numerics.Quaternion ConvertToSystem(this UnityEngine.Quaternion Quat)
        {
            System.Numerics.Quaternion Quaternion = new System.Numerics.Quaternion(Quat.x, Quat.y, Quat.z, Quat.w);
            return Quaternion;
        }

        public static UnityEngine.Vector3 GetVector3Unity(this DataStr.Vector3JSON Data)
        {
            UnityEngine.Vector3 v3 = new UnityEngine.Vector3(Data.x, Data.y, Data.z);
            return v3;
        }

        public static UnityEngine.Quaternion GetQuaternionUnity(this DataStr.QuaternionJSON Data)
        {
            UnityEngine.Quaternion q = new UnityEngine.Quaternion(Data.x, Data.y, Data.z, Data.w);
            return q;
        }

        public static DataStr.PrefabSpawnData GetData(this PrefabSpawn Spawn)
        {
            DataStr.PrefabSpawnData Data = new DataStr.PrefabSpawnData();
            Data.Gears = new List<DataStr.GearSpawnElementData>();
            Data.m_NumToSpawnMin = Spawn.m_NumToSpawnMin;
            Data.m_NumToSpawnMax = Spawn.m_NumToSpawnMax;
            Data.m_ChanceOfNoSpawn = Spawn.m_ChanceOfNoSpawn;
            Data.DisabledForXP = new List<string>();
            Data.EnabledForXP = new List<string>();

            Data.IsDLC = Spawn.gameObject.scene.name.Contains("_DLC");

            Component[] allComponents = Spawn.gameObject.GetComponentsInChildren<Component>(true);

            foreach (Component comp in allComponents)
            {
                if(comp is EnableObjectForXPMode)
                {
                    EnableObjectForXPMode EnableForXP = comp as EnableObjectForXPMode;
                    if (EnableForXP)
                    {
                        foreach (ExperienceModeType XP in EnableForXP.m_XPModesToEnable)
                        {
                            Data.EnabledForXP.Add(XP.ToString());
                        }
                    }
                }
                else if(comp is DisableObjectForXPMode)
                {
                    DisableObjectForXPMode DisableForXP = comp as DisableObjectForXPMode;

                    if (DisableForXP)
                    {
                        foreach (ExperienceModeType XP in DisableForXP.m_XPModesToDisable)
                        {
                            Data.DisabledForXP.Add(XP.ToString());
                        }
                    }
                }
            }


            foreach (PrefabSpawn.Element Element in Spawn.m_PrefabList)
            {
                if(Element.m_Prefab && Element.m_Prefab.name.StartsWith("GEAR_"))
                {
                    DataStr.GearSpawnElementData ElementData = new DataStr.GearSpawnElementData();
                    ElementData.GearName = Element.m_Prefab.name;
                    ElementData.SpawnWeight = Element.m_SpawnWeight;

                    UnityEngine.Vector3 worldPosition = Spawn.transform.TransformPoint(Element.m_Offset);

                    ElementData.Position = new DataStr.Vector3JSON(worldPosition.x, worldPosition.y, worldPosition.z);

                    UnityEngine.Quaternion localRotation = UnityEngine.Quaternion.Euler(Element.m_Rotation);
                    UnityEngine.Quaternion worldRotation = Spawn.transform.rotation * localRotation;

                    ElementData.Rotation = new DataStr.QuaternionJSON(worldRotation.x, worldRotation.y, worldRotation.z, worldRotation.w);

                    ElementData.DisabledForXP = new List<string>();
                    ElementData.EnabledForXP = new List<string>();

                    EnableObjectForXPMode EnableForXP = Element.m_Prefab.GetComponent<EnableObjectForXPMode>();

                    if (EnableForXP)
                    {
                        foreach (ExperienceModeType XP in EnableForXP.m_XPModesToEnable)
                        {
                            ElementData.EnabledForXP.Add(XP.ToString());
                        }
                    }

                    DisableObjectForXPMode DisableForXP = Element.m_Prefab.GetComponent<DisableObjectForXPMode>();

                    if (DisableForXP)
                    {
                        foreach (ExperienceModeType XP in DisableForXP.m_XPModesToDisable)
                        {
                            ElementData.DisabledForXP.Add(XP.ToString());
                        }
                    }

                    ElementData.Chance = 100f;
                    GearItem Gear = Element.m_Prefab.GetComponent<GearItem>();
                    if (Gear)
                    {
                        ElementData.Chance = Gear.m_SpawnChance;
                    }

                    Data.Gears.Add(ElementData);
                }
            }

            if (Data.Gears.Count == 0)
            {
                return null;
            }

            return Data;
        }

        public static DataStr.RandomSpawnObjectData GetData(this RandomSpawnObject Spawn, out List<GameObject> GearsToIgnore)
        {
            GearsToIgnore = new List<GameObject>();


            DataStr.RandomSpawnObjectData Data = new DataStr.RandomSpawnObjectData();
            Data.Gears = new List<DataStr.GearSpawnElementData>();
            Data.NumObjectsToSpawnPilgrim = Spawn.m_NumObjectsToEnablePilgrim;
            Data.NumObjectsToSpawnVoyageur = Spawn.m_NumObjectsToEnableVoyageur;
            Data.NumObjectsToSpawnStalker = Spawn.m_NumObjectsToEnableStalker;
            Data.NumObjectsToSpawnInterloper = Spawn.m_NumObjectsToEnableInterloper;
            Data.RerollAfterGameHours = Spawn.m_RerollAfterGameHours;
            Data.DisabledForXP = new List<string>();
            Data.EnabledForXP = new List<string>();

            Data.IsDLC = Spawn.gameObject.scene.name.Contains("_DLC");

            Component[] allComponents = Spawn.gameObject.GetComponentsInChildren<Component>(true);

            foreach (Component comp in allComponents)
            {
                if (comp is EnableObjectForXPMode)
                {
                    EnableObjectForXPMode EnableForXP = comp as EnableObjectForXPMode;
                    if (EnableForXP)
                    {
                        foreach (ExperienceModeType XP in EnableForXP.m_XPModesToEnable)
                        {
                            Data.EnabledForXP.Add(XP.ToString());
                        }
                    }
                }
                else if (comp is DisableObjectForXPMode)
                {
                    DisableObjectForXPMode DisableForXP = comp as DisableObjectForXPMode;

                    if (DisableForXP)
                    {
                        foreach (ExperienceModeType XP in DisableForXP.m_XPModesToDisable)
                        {
                            Data.DisabledForXP.Add(XP.ToString());
                        }
                    }
                }
            }

            for (int i = 0; i < Spawn.m_ObjectList.Count; i++)
            {
                GameObject Object = Spawn.m_ObjectList[i];
                int Weight = Spawn.m_Weights[i];
                if (Object && Object.name.StartsWith("GEAR_"))
                {
                    DataStr.GearSpawnElementData ElementData = new DataStr.GearSpawnElementData();
                    ElementData.GearName = GearSpawnsRipper.FixName(Object.name);
                    ElementData.SpawnWeight = Weight;
                    ElementData.Position = new DataStr.Vector3JSON(Object.transform.position.x, Object.transform.position.y, Object.transform.position.z);
                    ElementData.Rotation = new DataStr.QuaternionJSON(Object.transform.rotation.x, Object.transform.rotation.y, Object.transform.rotation.z, Object.transform.rotation.w);
                    ElementData.Chance = 100f;
                    GearItem Gear = Object.gameObject.GetComponent<GearItem>();
                    if (Gear)
                    {
                        ElementData.Chance = Gear.m_SpawnChance;
                    }

                    ElementData.EnabledForXP = new List<string>();
                    ElementData.DisabledForXP = new List<string>();

                    EnableObjectForXPMode EnableForXP = Object.GetComponent<EnableObjectForXPMode>();

                    if (EnableForXP)
                    {
                        foreach (ExperienceModeType XP in EnableForXP.m_XPModesToEnable)
                        {
                            ElementData.EnabledForXP.Add(XP.ToString());
                        }
                    }

                    DisableObjectForXPMode DisableForXP = Object.GetComponent<DisableObjectForXPMode>();

                    if (DisableForXP)
                    {
                        foreach (ExperienceModeType XP in DisableForXP.m_XPModesToDisable)
                        {
                            ElementData.DisabledForXP.Add(XP.ToString());
                        }
                    }

                    GearsToIgnore.Add(Object);
                    Data.Gears.Add(ElementData);
                }
            }

            if (Data.Gears.Count == 0)
            {
                return null;
            }

            return Data;
        }

        public static DataStr.RadialObjectSpawnerData GetData(this RadialObjectSpawner Spawn)
        {
            DataStr.RadialObjectSpawnerData Data = new DataStr.RadialObjectSpawnerData();
            Data.Gears = new List<DataStr.RadialObjectSpawnerElementData>();
            Data.MinToSpawn = Spawn.m_MinToSpawn;
            Data.MaxToSpawn = Spawn.m_MaxToSpawn;

            Data.MinRespawnTimeGameHours = Spawn.m_MinRespawnTimeGameHours;
            Data.MaxRespawnTimeGameHours = Spawn.m_MaxRespawnTimeGameHours;
            Data.PossiblePoints = new List<DataStr.Vector3JSON>();
            Data.EnabledForXP = new List<string>();
            Data.DisabledForXP = new List<string>();

            Data.IsDLC = Spawn.gameObject.scene.name.Contains("_DLC");

            Component[] allComponents = Spawn.gameObject.GetComponentsInChildren<Component>(true);

            foreach (Component comp in allComponents)
            {
                if (comp is EnableObjectForXPMode)
                {
                    EnableObjectForXPMode EnableForXP = comp as EnableObjectForXPMode;
                    if (EnableForXP)
                    {


                        foreach (ExperienceModeType XP in EnableForXP.m_XPModesToEnable)
                        {
                            Data.EnabledForXP.Add(XP.ToString());
                        }
                    }
                }
                else if (comp is DisableObjectForXPMode)
                {
                    DisableObjectForXPMode DisableForXP = comp as DisableObjectForXPMode;

                    if (DisableForXP)
                    {
                        foreach (ExperienceModeType XP in DisableForXP.m_XPModesToDisable)
                        {
                            Data.DisabledForXP.Add(XP.ToString());
                        }
                    }
                }
            }

            if (Spawn.m_LootTableData == null)
            {
                if (Spawn.m_ObjectToSpawn)
                {
                    if (Spawn.m_ObjectToSpawn.name.StartsWith("GEAR_"))
                    {
                        DataStr.RadialObjectSpawnerElementData ElementData = new DataStr.RadialObjectSpawnerElementData();
                        ElementData.GearName = GearSpawnsRipper.FixName(Spawn.m_ObjectToSpawn.name);
                        ElementData.SpawnWeight = 100;
                        ElementData.Chance = 100f;
                        GearItem Gear = Spawn.m_ObjectToSpawn.GetComponent<GearItem>();
                        if (Gear)
                        {
                            ElementData.Chance = Gear.m_SpawnChance;
                        }
                        Data.Gears.Add(ElementData);
                    }
                }
            }
            else
            {
                for (int i = 0; i < Spawn.m_LootTableData.m_BaseEntries.Count; i++)
                {
                    var Entery = Spawn.m_LootTableData.m_BaseEntries[i];

                    if (Entery != null && Entery.m_Item != null)
                    {
                        int Weight = Entery.m_Weight;
                        GameObject Prefab = Spawn.m_LootTableData.LoadPrefab(Entery.m_Item);
                        if (Prefab && Prefab.name.StartsWith("GEAR_"))
                        {
                            DataStr.RadialObjectSpawnerElementData ElementData = new DataStr.RadialObjectSpawnerElementData();
                            ElementData.GearName = GearSpawnsRipper.FixName(Prefab.name);
                            ElementData.SpawnWeight = Weight;
                            ElementData.Chance = 100f;
                            GearItem Gear = Prefab.GetComponent<GearItem>();
                            if (Gear)
                            {
                                ElementData.Chance = Gear.m_SpawnChance;
                            }

                            Data.Gears.Add(ElementData);
                        }
                    }
                }
                Minimalizer.RadialObjectSpawner_SpawnObjectAttempt.s_ByPass = true;
            }

            for (int i = 0; i < 40; i++)
            {
                UnityEngine.Vector3 vector = UnityEngine.Vector3.zero;

                UnityEngine.Vector3 origin = Spawn.transform.position;
                if (Spawn.m_Spline)
                {
                    origin = Spawn.m_Spline.GetPositionOnSpline(UnityEngine.Random.Range(0f, 1f));
                }
                int navmeshArea = AiUtils.GetNavmeshArea(Spawn.transform.position);
                bool testNavMeshSurfaceOnly = true;
                if (!AiUtils.GetRandomPointOnNavmesh(out vector, origin, Spawn.m_MinRadius, Spawn.m_MaxRadius, navmeshArea, testNavMeshSurfaceOnly, 0.2f))
                {
                    continue;
                }
                RaycastHit raycastHit;
                if (!Physics.Raycast(vector + new UnityEngine.Vector3(0f, 2f, 0f) + Spawn.m_RaycastOffset, UnityEngine.Vector3.down, out raycastHit, float.PositiveInfinity, Utils.m_PhysicalCollisionLayerMask))
                {
                    continue;
                }
                vector = raycastHit.point;
                vector += raycastHit.normal * Spawn.m_FloatHeight;

                Data.PossiblePoints.Add(new DataStr.Vector3JSON(vector.x, vector.y, vector.z));
            }

            if (Data.Gears.Count == 0)
            {
                return null;
            }
            if(Data.PossiblePoints.Count == 0)
            {
                return null;
            }

            return Data;
        }
    }
}
