using Il2Cpp;
using Il2CppTLD.PDID;
using MelonLoader;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace SkyCoopClient
{
    public class OpenablesSync
    {
        public const float c_AudiableRange = 2f;

        public static void MaybePlayerAudio(string AudioName, GameObject Obj)
        {
            if (Obj)
            {
                float Distance = Vector3.Distance(GameManager.GetPlayerTransform().position, Obj.transform.position);
                if(Distance < c_AudiableRange)
                {
                    GameAudioManager.PlaySound(AudioName, Obj);
                }
            }
        }
        public static void HandleOpenableSync(string GUID, bool OpenState, bool AllowAudio = true)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if (Obj)
            {
                OpenClose Openable = Obj.GetComponent<OpenClose>();
                if (Openable)
                {
                    if(!Openable.IsOpen() && OpenState)
                    {
                        Openable.Open(true);
                        if (AllowAudio)
                        {
                            MaybePlayerAudio(Openable.m_OpenAudio, Openable.gameObject);
                        }
                    }else if(Openable.IsOpen() && !OpenState)
                    {
                        Openable.Close(true);
                        if (AllowAudio)
                        {
                            MaybePlayerAudio(Openable.m_CloseAudio, Openable.gameObject);
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(OpenClose), "Open", new System.Type[] { typeof(bool), typeof(bool) })]
        internal static class OpenClose_Open
        {
            private static void Postfix(OpenClose __instance, bool isImmediate, bool fromLink)
            {
                if (isImmediate == true)
                {
                    return;
                }

                if (__instance.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    ClientSend.SendOpenableState(__instance.gameObject.GetComponent<ObjectGuid>().Get(), false);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(OpenClose), "Close", new System.Type[] { typeof(bool), typeof(bool) })]
        internal static class OpenClose_Close
        {
            private static void Postfix(OpenClose __instance, bool isImmediate, bool fromLink)
            {
                if (isImmediate == true)
                {
                    return;
                }

                if (__instance.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    ClientSend.SendOpenableState(__instance.gameObject.GetComponent<ObjectGuid>().Get(), true);
                }
            }
        }
    }
}
