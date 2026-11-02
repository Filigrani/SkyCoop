using Il2Cpp;
using Il2CppTLD.PDID;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using static SkyCoop.Comps;

namespace SkyCoopClient
{
    public static class AnimalsManager
    {        
        public static bool IsActor(GameObject Obj)
        {
            if (Obj)
            {
                return Obj.GetComponent<AnimalActor>();
            }
            return false;
        }
        
        
        public static GameObject SpawnActor(string PrefabName, string GUID, Vector3 Position, Quaternion Rotation, int AiState, int PreviousState)
        {
            GameObject Prefab = AssetManager.GetAssetFromGame<GameObject>(PrefabName);
            if (Prefab)
            {
                GameObject Obj = UnityEngine.Object.Instantiate(Prefab, Position, Rotation);
                ObjectGuid ObjGUID = Obj.GetComponent<ObjectGuid>();
                if (ObjGUID == null)
                {
                    ObjGUID = Obj.AddComponent<ObjectGuid>();
                }

                if (ObjGUID)
                {
                    PdidTable.RuntimeRegister(ObjGUID, GUID);
                }

                BaseAi Ai = Obj.GetComponent<BaseAi>();

                if (Ai)
                {
                    AnimalActor Actor = Obj.AddComponent<AnimalActor>();
                    Actor.m_Position = Position;
                    Actor.m_Rotation = Rotation;
                    Actor.m_Animator = Ai.m_Animator;
                    Actor.m_BloodTrail = Ai.m_BloodTrail;
                    Actor.m_AiState = AiState;
                    Actor.m_AiStatePrevious = PreviousState;
                    Actor.SetupAudio(Ai);
                    Obj.transform.SetParent(null);
                    if (Ai.m_PathTargetTransform)
                    {
                        UnityEngine.Object.Destroy(Ai.m_PathTargetTransform);
                    }
                    if (Ai.m_NavMeshAgent)
                    {
                        UnityEngine.Object.Destroy(Ai.m_NavMeshAgent.gameObject);
                    }

                    UnityEngine.Object.Destroy(Ai);
                }
                Obj.name = PrefabName;
                SkyCoop.Logger.Log(ConsoleColor.Cyan, $"{PrefabName} {GUID} spawned actor");
                return Obj;
            }
            return null;
        }

        public static GameObject SpawnAnimal(string PrefabName, string GUID, string SpawnerGUID, Vector3 Position, Quaternion Rotation, int AiState, bool IsBleeding, float Health, float BleedingMinutes, int PreviousState, Vector3 SpawnPos)
        {
            GameObject Prefab = AssetManager.GetAssetFromGame<GameObject>(PrefabName);
            if (Prefab)
            {
                GameObject Obj = UnityEngine.Object.Instantiate(Prefab, Position, Rotation);
                if (Obj)
                {
                    ObjectGuid ObjGUID = Obj.GetComponent<ObjectGuid>();
                    if (ObjGUID == null)
                    {
                        ObjGUID = Obj.AddComponent<ObjectGuid>();
                    }
                    BaseAi Ai = Obj.GetComponent<BaseAi>();
                    if (Ai)
                    {
                        if(AiState == -1)
                        {
                            Ai.m_CurrentMode = Ai.m_StartMode;
                            Ai.m_PreviousMode = AiMode.None;
                        }
                        else
                        {
                            Ai.m_CurrentMode = (AiMode)AiState;
                            Ai.m_PreviousMode = (AiMode)PreviousState;
                        }
                        Ai.m_SpawnPos = SpawnPos;
                        Ai.m_CurrentHP = Health;
                        Ai.m_BleedingOut = IsBleeding;
                        Ai.m_ElapsedBleedingOutMinutes = BleedingMinutes;
                        GameObject SpawnRegionObj = PdidTable.GetGameObject(SpawnerGUID);
                        if (SpawnRegionObj)
                        {
                            SpawnRegion SpawnRegion = SpawnRegionObj.GetComponent<SpawnRegion>();
                            if (SpawnRegion)
                            {
                                SpawnRegion.SetActive(true);
                                Ai.SetSpawnRegionParent(SpawnRegion);
                            }
                        }
                    }

                    if (ObjGUID)
                    {
                        PdidTable.RuntimeRegister(ObjGUID, GUID);
                    }
                    Comps.BaseAiHook Hook = Obj.AddComponent<Comps.BaseAiHook>();
                    Hook.m_Ai = Ai;
                    Hook.m_GUID = GUID;
                    Obj.name = PrefabName;
                    SkyCoop.Logger.Log(ConsoleColor.Cyan, $"{PrefabName} {GUID} spawned controlled");
                    return Obj;
                }
            }
            return null;
        }
        
        public static void HandleControlRequest(string PrefabName, string GUID, string SpawnerGUID, Vector3 Position, Quaternion Rotation, int AiState, bool IsBleeding, float Health, float BleedingMinutes, int PreviousState, Vector3 SpawnPose)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if (Obj)
            {
                if (IsActor(Obj))
                {
                    HandleRemove(Obj, "(retarget)");
                    Obj = null;
                }
            }

            if (Obj == null)
            {
                SpawnAnimal(PrefabName, GUID, SpawnerGUID, Position, Rotation, AiState, IsBleeding, Health, BleedingMinutes, PreviousState, SpawnPose);
            }
            else
            {
                BaseAiHook Hook = Obj.GetComponent<BaseAiHook>();
                if (Hook)
                {
                    Hook.ResetNoResponceTimer();
                }
                Obj.SetActive(true);
            }
        }

        public static void HandleRemove(GameObject Obj, string Reason = "")
        {
            if (Obj)
            {
                ObjectGuid ObjGUID = Obj.GetComponent<ObjectGuid>();
                string GUID = "";
                if (ObjGUID)
                {
                    GUID = ObjGUID.GetPDID();
                    PdidTable.RuntimeUnregister(ObjGUID);
                }

                if (IsActor(Obj))
                {
                    SkyCoop.Logger.Log(ConsoleColor.Cyan, $"{Obj.name} {GUID} removed actor {Reason}");
                    UnityEngine.Object.Destroy(Obj);
                }
                else
                {
                    SkyCoop.Logger.Log(ConsoleColor.Cyan, $"{Obj.name} {GUID} removed controlled {Reason}");
                    BaseAi Ai = Obj.GetComponent<BaseAi>();
                    // Удалять именно в таком порядке важно! Ибо они связанны и сносят сам объект.
                    GameObject NavMeshAgentObj = null;
                    if (Ai.m_NavMeshAgent)
                    {
                        NavMeshAgentObj = Ai.m_NavMeshAgent.gameObject;
                    }
                    UnityEngine.Object.Destroy(Ai.m_NavMeshAgent);
                    UnityEngine.Object.Destroy(Ai.m_MoveAgent);
                    if (Ai.m_NavMeshAgent.gameObject)
                    {
                        if (Obj.transform.parent == Ai.m_NavMeshAgent.gameObject)
                        {
                            Obj.transform.SetParent(null);
                        }
                        UnityEngine.Object.Destroy(Ai.m_NavMeshAgent.gameObject);
                    }
                    if (Ai.m_PathTargetTransform)
                    {
                        UnityEngine.Object.Destroy(Ai.m_PathTargetTransform);
                    }
                    BaseAiManager.Destroy(Ai);
                }
            }
        }

        public static void HandleRemove(string GUID, string Reason = "")
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if (Obj)
            {
                HandleRemove(Obj, Reason);
            }
        }

        public static void HandleSync(string PrefabName, string GUID, string SpawnerGUID, Vector3 Position, Quaternion Rotation, DataStr.AnimatorData AnimatorData, bool IsBleeding, int AiState, int PreviousState, Vector3 Velocity)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if (Obj)
            {
                if (!IsActor(Obj))
                {
                    HandleRemove(Obj, "(retarget)");
                    Obj = null;
                }
            }

            if (Obj == null)
            {
                Obj = SpawnActor(PrefabName, GUID, Position, Rotation, AiState, PreviousState);
            }
            if (Obj)
            {
                Obj.SetActive(true);
                AnimalActor Actor = Obj.GetComponent<AnimalActor>();
                if (Actor)
                {
                    Actor.ResetNoResponceTimer();
                    Actor.m_Position = Position;
                    Actor.m_Rotation = Rotation;
                    Actor.m_Velocity = Velocity;

                    if (Actor.m_Animator)
                    {
                        foreach (DataStr.AnimatorData.ParameterData Parameter in AnimatorData.m_Parameters)
                        {
                            switch ((AnimatorControllerParameterType)Parameter.m_Type)
                            {
                                case AnimatorControllerParameterType.Float:
                                    Actor.m_Animator.SetFloat(Parameter.m_Hash, Parameter.m_Float);
                                    break;
                                case AnimatorControllerParameterType.Int:
                                    Actor.m_Animator.SetInteger(Parameter.m_Hash, Parameter.m_Int);
                                    break;
                                case AnimatorControllerParameterType.Bool:
                                    Actor.m_Animator.SetBool(Parameter.m_Hash, Parameter.m_Bool);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    if (Actor.m_BloodTrail)
                    {
                        Actor.m_BloodTrail.enabled = IsBleeding;
                    }
                    Actor.SetState(AiState);
                }
            }
        }
    }
}
