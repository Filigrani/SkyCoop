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
using static Il2Cpp.PlayerVoice;
using static Il2Cpp.Utils;
using static SkyCoop.Comps;

namespace SkyCoopClient
{
    public static class AnimalsManager
    {        
        public static GameObject SpawnAnimal(string PrefabName, string GUID, string SpawnerGUID, Vector3 Position, Quaternion Rotation, int AiState, int PreviousState, bool IsActor)
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
                    Comps.BaseAiHook Hook = Obj.AddComponent<Comps.BaseAiHook>();
                    Hook.m_GUID = GUID;
                    BaseAi Ai = Obj.GetComponent<BaseAi>();
                    WildlifeItem Item = Obj.GetComponent<WildlifeItem>();
                    if (Ai)
                    {
                        Hook.m_Ai = Ai;
                        if (AiState == -1)
                        {
                            Ai.m_CurrentMode = Ai.m_StartMode;
                            Ai.m_PreviousMode = AiMode.None;
                        }
                        else
                        {
                            Ai.m_CurrentMode = (AiMode)AiState;
                            Ai.m_PreviousMode = (AiMode)PreviousState;
                        }

                        GameObject SpawnRegionObj = PdidTable.GetGameObject(SpawnerGUID);
                        if (SpawnRegionObj)
                        {
                            SpawnRegion SpawnRegion = SpawnRegionObj.GetComponent<SpawnRegion>();
                            if (SpawnRegion)
                            {
                                Ai.SetSpawnRegionParent(SpawnRegion);
                                Hook.UpdateSpawner();
                            }
                        }
                        Hook.SetIsActor(IsActor);
                    }
                    if (Item)
                    {
                        Hook.m_Item = Item;
                    }

                    if (ObjGUID)
                    {
                        PdidTable.RuntimeRegister(ObjGUID, GUID);
                    }
                    Obj.name = PrefabName;
                    SkyCoop.Logger.Log(ConsoleColor.Cyan, $"{PrefabName} {GUID} spawned");
                    ClientSend.SendAnimalArrowRequest(GUID);
                    return Obj;
                }
            }
            return null;
        }

        public static GameObject SpawnAnimal(string PrefabName, string GUID, string SpawnerGUID, Vector3 Position, Quaternion Rotation, int AiState, bool IsBleeding, float Health, float BleedingMinutes, int PreviousState, Vector3 SpawnPos)
        {
            GameObject Obj = SpawnAnimal(PrefabName, GUID, SpawnerGUID, Position, Rotation, AiState, PreviousState, false);
            if (Obj)
            {
                BaseAi Ai = Obj.GetComponent<BaseAi>();
                if (Ai)
                {
                    Ai.m_SpawnPos = SpawnPos;

                    if (AiState == -1) // Животное ещё не было никогда загруженно, а хост макс хп не знает по этому игнорим его запрос и задём макс
                    {
                        Ai.m_CurrentHP = Ai.m_MaxHP;
                    }
                    else
                    {
                        Ai.m_CurrentHP = Health;
                    }
                    Ai.m_BleedingOut = IsBleeding;
                    Ai.m_ElapsedBleedingOutMinutes = BleedingMinutes;
                }
            }
            return null;
        }

        public static GameObject SpawnCorpse(string PrefabName, string GUID, Vector3 Position, Quaternion Rotation, bool PlaySound = false)
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
                    if (ObjGUID)
                    {
                        PdidTable.RuntimeRegister(ObjGUID, GUID);
                    }
                    Obj.name = PrefabName;
                    SkyCoop.Logger.Log(ConsoleColor.Cyan, $"{PrefabName} {GUID} corpse spawned");
                    ClientSend.SendAnimalArrowRequest(GUID);

                    BaseAi Ai = Obj.GetComponent<BaseAi>();

                    if (Ai)
                    {
                        Ai.m_CurrentHP = 0;
                        Ai.SetAiMode(AiMode.Dead);
                        Ai.enabled = false;
                        if (Ai.m_BodyHarvest)
                        {
                            Ai.m_BodyHarvest.enabled = false;
                        }
                    }

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
                if(Obj.name != PrefabName)
                {
                    HandleRemove(Obj, "(Prefab Change)");
                    Obj = null;
                }
                else
                {
                    BaseAiHook Hook = Obj.GetComponent<BaseAiHook>();
                    if (Hook)
                    {
                        if (Hook.m_IsActor)
                        {
                            SkyCoop.Logger.Log(ConsoleColor.Cyan, $"{Obj.name} {GUID} controlled now (retarget)");
                            Hook.SetIsActor(false);
                        }
                    }
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

        public static void HandleSync(string PrefabName, string GUID, string SpawnerGUID, Vector3 Position, Quaternion Rotation, DataStr.AnimatorData AnimatorData, bool IsBleeding, int AiState, int PreviousState, Vector3 Velocity, bool IsBeingHeld)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if (Obj)
            {
                if(Obj.name != PrefabName)
                {
                    HandleRemove(Obj, "(Prefab Change)");
                    Obj = null;
                }
                else
                {
                    BaseAiHook Hook = Obj.GetComponent<BaseAiHook>();
                    if (Hook)
                    {
                        if (!Hook.m_IsActor)
                        {
                            SkyCoop.Logger.Log(ConsoleColor.Cyan, $"{Obj.name} {GUID} become actor (retarget)");
                            Hook.SetIsActor(true);
                        }
                    }
                }
            }

            if (Obj == null)
            {
                Obj = SpawnAnimal(PrefabName, GUID, SpawnerGUID, Position, Rotation, AiState, PreviousState, true);
            }
            if (Obj)
            {
                Obj.SetActive(true);
                BaseAiHook Hook = Obj.GetComponent<BaseAiHook>();
                if (Hook)
                {
                    Hook.ResetNoResponceTimer();
                    Hook.m_Position = Position;
                    Hook.m_Rotation = Rotation;
                    Hook.m_Velocity = Velocity;

                    Hook.SetAnimationData(AnimatorData);
                    Hook.SetBleeding(IsBleeding);
                    Hook.SetState(AiState);
                    Hook.SetHolding(IsBeingHeld);
                }
            }
        }

        public static void HandleRemove(GameObject Obj, string Reason = "")
        {
            if (Obj)
            {
                GameAudioManager.StopAllSoundsFromGameObject(Obj);
                ObjectGuid ObjGUID = Obj.GetComponent<ObjectGuid>();
                string GUID = "";
                if (ObjGUID)
                {
                    GUID = ObjGUID.GetPDID();
                    PdidTable.RuntimeUnregister(ObjGUID);
                }

                BaseAi Ai = Obj.GetComponent<BaseAi>();

                Obj.transform.SetParent(null);
                if (Ai.m_NavMeshAgent)
                {
                    UnityEngine.Object.Destroy(Ai.m_NavMeshAgent.gameObject);
                }
                if (Ai.m_PathTargetTransform)
                {
                    UnityEngine.Object.Destroy(Ai.m_PathTargetTransform);
                }
                SkyCoop.Logger.Log(ConsoleColor.Cyan, $"{Obj.name} {GUID} removed {Reason}");
                BaseAiManager.Destroy(Ai);
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

        public static void HandleCorpse(string GUID, string Prefab, Vector3 Position, Quaternion Rotation)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if (Obj && Obj.GetComponent<BaseAi>())
            {
                HandleRemove(Obj, "(death)");
                Obj = null;
            }
            if (Obj == null)
            {
                SpawnCorpse(Prefab, GUID, Position, Rotation);
            }
        }

        public static void HandleDamage(string GUID, float Damage, float BleedingMinutes, int DamageSource, string Colider)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if (Obj)
            {
                BaseAiHook Hook = Obj.GetComponent<BaseAiHook>();
                if (Hook && !Hook.m_IsActor)
                {
                    if (Hook.m_Ai)
                    {
                        Hook.m_Ai.ApplyDamage(Damage, BleedingMinutes, (DamageSource)DamageSource, Colider);
                    }
                }
            }
        }
        public static void HandleArrow(string GUID, DataStr.AnimalEntity.ArrowData Arrow)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if (Obj)
            {
                SkyCoop.Logger.Log(ConsoleColor.Cyan, $"Sync Arrow {Obj.name} {GUID}");
                BaseAiHook Hook = Obj.GetComponent<BaseAiHook>();
                if (Hook)
                {
                    if (Hook.m_Ai)
                    {
                        Transform Bone = Obj.transform.FindChild(Arrow.m_Bone);
                        if (Bone)
                        {
                            GameObject BogusArrow = AssetManager.CreateBogusGear(Arrow.m_ArrowPrefab, Bone, true);
                            if (BogusArrow)
                            {
                                BogusArrow.transform.SetLocalPositionAndRotation(Arrow.m_Position.ConvertToUnity(), Arrow.m_Rotation.ConvertToUnity());
                            }
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "UpdateFromManager")]
        private static class SpawnRegion_UpdateFromManager
        {
            private static bool Prefix(SpawnRegion __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                __instance.gameObject.SetActive(true);

                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "MaybeReRollActive")]
        private static class SpawnRegion_MaybeReRollActive
        {
            private static bool Prefix(SpawnRegion __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                __instance.gameObject.SetActive(true);

                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "SpawningSupppressedByExperienceMode")]
        private static class SpawnRegion_SpawningSupppressedByExperienceMode
        {
            private static void Postfix(SpawnRegion __instance, bool __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                __result = true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "Spawn")]
        private static class SpawnRegion_Spawn
        {
            private static bool Prefix(SpawnRegion __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BaseAi), "ApplyDamage", new System.Type[] { typeof(float), typeof(float), typeof(DamageSource), typeof(string) })]
        public static class BaseAi_ApplyDamage
        {
            public static bool s_Skip = false;
            public static bool s_Sync = true;
            private static bool Prefix(BaseAi __instance, float damage, float bleedOutMintues, DamageSource damageSource, string collider)
            {
                if (s_Skip)
                {
                    return false;
                }
                
                if (!ModMain.IsMultiplayer()) { return true; }

                Comps.BaseAiHook Hook = __instance.gameObject.GetComponent<Comps.BaseAiHook>();
                if(Hook)
                {
                    if (Hook.m_IsActor)
                    {
                        SkyCoop.Logger.Log($"{__instance.gameObject.name} {Hook.m_GUID} actor damaged {damage} bleedOutMintues {bleedOutMintues} damageSource {damageSource} collider {collider}");

                        if(damageSource != DamageSource.NoiseMaker) // Снаряд нанесёт урон у контроллера так что синхронить урон не надо.
                        {
                            if (s_Sync)
                            {
                                ClientSend.SendAnimalDamage(Hook.m_GUID, damage, bleedOutMintues, (int)damageSource, collider);
                            }
                        }

                        return false;
                    }
                    else
                    {
                        SkyCoop.Logger.Log($"{__instance.gameObject.name} {Hook.m_GUID} controlled damaged {damage} bleedOutMintues {bleedOutMintues} damageSource {damageSource} collider {collider}");
                    }
                }
                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BaseAi), "IgnoreAudioEvent")]
        private static class SpawnRegion_IgnoreAudioEvent
        {
            private static bool Prefix(BaseAi __instance, GameObject sender, Vector3 pos)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                Comps.BaseAiHook Hook = __instance.gameObject.GetComponent<Comps.BaseAiHook>();
                if(Hook && Hook.m_IsActor)
                {
                    return false;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(MoveAgent), "CanFindPath")]
        private static class SpawnRegion_CanFindPath
        {
            private static bool Prefix(MoveAgent __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (!__instance.IsOnNavMesh() || !__instance.enabled)
                {
                    return false;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BaseAi), "Stun")]
        private static class SpawnRegion_Stun
        {
            private static bool Prefix(BaseAi __instance, Vector3 position)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                Comps.BaseAiHook Hook = __instance.gameObject.GetComponent<Comps.BaseAiHook>();
                if (Hook && Hook.m_IsActor)
                {
                    return false;
                }
                else
                {
                    //TODO: пока что так.
                    __instance.m_CurrentHP = 0;
                }
                return true;
            }
        }
    }
}
