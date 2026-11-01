using Il2Cpp;
using Il2CppTLD.PDID;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using static Il2Cpp.Utils;
using static SkyCoop.Comps;

namespace SkyCoopClient
{
    public static class AnimalsManager
    {        
        public static void HandleControlRequest(string PrefabName, string GUID, string SpawnerGUID, Vector3 Position, Quaternion Rotation, int AiState)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if (Obj)
            {
                BaseAi Ai = Obj.GetComponent<BaseAi>();
                if(Ai == null)
                {
                    UnityEngine.Object.Destroy(Obj);
                    Obj = null;
                }
            }


            if (Obj == null)
            {
                GameObject Prefab = AssetManager.GetAssetFromGame<GameObject>(PrefabName);
                if (Prefab)
                {
                    Obj = UnityEngine.Object.Instantiate(Prefab, Position, Rotation);
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
                            Ai.SetAiMode((AiMode)AiState);
                        }

                        if (ObjGUID)
                        {
                            PdidTable.RuntimeRegister(ObjGUID, GUID);
                        }
                    }
                }
            }
        }

        public static void HandleRemove(string GUID)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if (Obj)
            {
                UnityEngine.Object.Destroy(Obj);
            }
        }

        public static void HandleSync(string PrefabName, string GUID, string SpawnerGUID, Vector3 Position, Quaternion Rotation, int AiState)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if (Obj == null)
            {
                GameObject Prefab = AssetManager.GetAssetFromGame<GameObject>(PrefabName);
                if (Prefab)
                {
                    Obj = UnityEngine.Object.Instantiate(Prefab, Position, Rotation);
                    ObjectGuid ObjGUID = Obj.GetComponent<ObjectGuid>();
                    if (ObjGUID == null)
                    {
                        ObjGUID = Obj.AddComponent<ObjectGuid>();
                    }

                    if (ObjGUID)
                    {
                        PdidTable.RuntimeRegister(ObjGUID, GUID);
                    }
                }
            }
            if (Obj)
            {
                BaseAi Ai = Obj.GetComponent<BaseAi>();
                if (Ai)
                {
                    UnityEngine.Object.Destroy(Ai.m_NavMeshAgent);
                    UnityEngine.Object.Destroy(Ai.m_MoveAgent);
                    UnityEngine.Object.Destroy(Ai);
                }
                AnimalActor Actor = Obj.GetComponent<AnimalActor>();
                if(Actor == null)
                {
                    Actor = Obj.AddComponent<AnimalActor>();
                }
                if (Actor)
                {
                    Actor.m_Position = Position;
                    Actor.m_Rotation = Rotation;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BaseAi), "Update")]
        private static class BaseAi_Update
        {
            private static void Postfix(BaseAi __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (__instance.enabled) // Вроде бы даже с выклюенным, игра его вызывает из некоторых методов на прямую.
                {
                    if (ModMain.IsGameplayScene())
                    {
                        string GUID = "";
                        ObjectGuid ObjGUID = __instance.gameObject.GetComponent<ObjectGuid>();
                        if (ObjGUID)
                        {
                            GUID = ObjGUID.GetPDID();
                        }
                        ClientSend.SendAnimalSync(GUID, __instance.gameObject.transform.position, __instance.gameObject.transform.rotation, (int)__instance.GetAiMode());
                    }
                }
            }
        }
    }
}
