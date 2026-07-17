using Il2Cpp;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoopClient
{
    public static class AudioHook
    {
        public const bool c_RedirectAllSoundsTo3D = true;
        public static Dictionary<string, float> s_HearingRanges = new Dictionary<string, float>();
        public static void Init()
        {
            RegisterAudioHearingRange("Play_NoisemakerDetonate", 150);
            RegisterAudioHearingRange("Play_NoisemakerCollide", 30);
            RegisterAudioHearingRange("Play_SndMechDoorWoodClose1", 30);
            RegisterAudioHearingRange("Play_SndMechDoorWoodOpen1", 30);
        }


        public static void RegisterAudioHearingRange(string AudioEventName, float MaxDistance)
        {
            s_HearingRanges.Add(AudioEventName, MaxDistance);
        }

        public static bool PlayerCanHearIt(Vector3 EmitterPosition, float MaxDistance)
        {
            if (GameManager.GetPlayerTransform())
            {
                Transform T = GameManager.GetPlayerTransform();

                return Vector3.Distance(T.position, EmitterPosition) < MaxDistance;
            }
            return false;
        }

        public static bool PlayerCanHearIt(Vector3 EmitterPosition, string AudioEventName)
        {
            if (s_HearingRanges.ContainsKey(AudioEventName))
            {
                return PlayerCanHearIt(EmitterPosition, s_HearingRanges[AudioEventName]);
            }
            return true;
        }
        
        public static void KillAudio(uint AudioID)
        {
            if(AudioID != 0U)
            {
                AkSoundEngine.StopPlayingID(AudioID, 0);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GameAudioManager), "PlaySound", new System.Type[] { typeof(Il2CppAK.Wwise.Event), typeof(GameObject) })]
        public class GameAudioManager_PlaySound_Event
        {
            public static bool Prefix(Il2CppAK.Wwise.Event soundEvent, GameObject go, ref uint __result)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (c_RedirectAllSoundsTo3D)
                {
                    if (go && soundEvent != null)
                    {
                        __result = GameAudioManager.Play3DSound(soundEvent, go);
                        return false;
                    }
                }
                else
                {
                    if (go && soundEvent != null)
                    {
                        if (PlayerCanHearIt(go.transform.position, soundEvent.Name))
                        {
                            return true;
                        }
                        else
                        {
                            __result = 0U;
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GameAudioManager), "PlaySound", new System.Type[] { typeof(string), typeof(GameObject) })]
        public class GameAudioManager_PlaySound_String
        {
            public static bool Prefix(string soundID, GameObject go, ref uint __result)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (c_RedirectAllSoundsTo3D)
                {
                    if (go)
                    {
                        __result = GameAudioManager.Play3DSound(soundID, go);
                        return false;
                    }
                }
                else
                {
                    if (go)
                    {
                        if (PlayerCanHearIt(go.transform.position, soundID))
                        {
                            return true;
                        }
                        else
                        {
                            __result = 0U;
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GameAudioManager), "Play3DSound", new System.Type[] { typeof(string), typeof(GameObject) })]
        public class GameAudioManager_Play3DSound_String
        {
            public static bool Prefix(string soundID, GameObject go, ref uint __result)
            {
                if (!ModMain.IsMultiplayer()) { return true; }
                if (go)
                {
                    if (PlayerCanHearIt(go.transform.position, soundID))
                    {
                        return true;
                    }
                    else
                    {
                        __result = 0U;
                        return false;
                    }
                }

                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GameAudioManager), "Play3DSound", new System.Type[] { typeof(Il2CppAK.Wwise.Event), typeof(GameObject) })]
        public class GameAudioManager_Play3DSound_Event
        {
            public static bool Prefix(Il2CppAK.Wwise.Event soundEvent, GameObject go, ref uint __result)
            {
                if (!ModMain.IsMultiplayer()) { return true; }
                if (go && soundEvent != null)
                {
                    if (PlayerCanHearIt(go.transform.position, soundEvent.Name))
                    {
                        return true;
                    }
                    else
                    {
                        __result = 0U;
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
