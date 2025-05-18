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
                        string AudioName = Openable.m_OpenAudio;
                        Openable.m_OpenAudio = ""; // Sloppy fix, so Open() won't make a sound. 
                        Openable.Open(!AllowAudio);
                        Openable.m_OpenAudio = AudioName;
                        if (AllowAudio)
                        {
                            MaybePlayerAudio(AudioName, Openable.gameObject);
                        }
                    }else if(Openable.IsOpen() && !OpenState)
                    {
                        string AudioName = Openable.m_CloseAudio;
                        Openable.Close(!AllowAudio);
                        Openable.m_CloseAudio = AudioName;
                        if (AllowAudio)
                        {
                            MaybePlayerAudio(AudioName, Openable.gameObject);
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(OpenClose), "PerformInteraction")]
        internal static class PlayerManager_PerformInteraction
        {
            private static void Postfix(OpenClose __instance)
            {
                ObjectGuid GUIDObj = __instance.GetComponent<ObjectGuid>();
                if (GUIDObj)
                {
                    ClientSend.SendOpenableState(GUIDObj.Get(), __instance.IsOpen());
                }
            }
        }
    }
}
