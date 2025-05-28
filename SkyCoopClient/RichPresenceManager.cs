using Harmony;
using HarmonyLib;
using Il2Cpp;
using Il2CppSteamworks;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoopClient
{
    public class RichPresenceManager
    {
        [HarmonyLib.HarmonyPatch(typeof(SteamRichPresenceProvider), "ChangePresence", null)]
        public class SteamRichPresenceProvider_Initialize
        {
            public static bool Prefix(RichPresenceProviderBase __instance)
            {
                SteamFriends.SetRichPresence("status_value", "Sky Co-op Reborn");
                SteamFriends.SetRichPresence("steam_display", "#Status");

                return false;
            }
        }
    }
}
