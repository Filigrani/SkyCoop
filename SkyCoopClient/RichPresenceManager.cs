using Harmony;
using HarmonyLib;
using Il2Cpp;
using Il2CppSteamworks;
using SkyCoop;

namespace SkyCoopClient
{
    public class RichPresenceManager
    {
        [HarmonyLib.HarmonyPatch(typeof(SteamRichPresenceProvider), "ChangePresence", null)]
        public class SteamRichPresenceProvider_Initialize
        {
            public static bool Prefix()
            {
                SteamFriends.SetRichPresence("status_value", "Sky Co-op Reborn");
                SteamFriends.SetRichPresence("steam_display", "#Status");

                //if (ModMain.Client != null && ModMain.Client.m_IsReady)
                //{
                //    SteamFriends.SetRichPresence("connect", $"+connect {ModMain.Client.GetServerAddress()}");
                //    Logger.Log(ConsoleColor.Green, $"[SetRichPresence] +connect {ModMain.Client.GetServerAddress()}");
                //}
                //else
                //{
                //    SteamFriends.SetRichPresence("connect", null);
                //}

                return false;
            }
        }
    }
}
