using System;
using Harmony;
using MelonLoader;
using UnityEngine;
using SkyCoop;


namespace SkyCoop
{
    //[HarmonyPatch(typeof(BreakDown))]
    //[HarmonyPatch("Start")]
    [HarmonyPatch(typeof(DarkWalker), "Invoke")]
    internal static class Patches
    {
        static void Postfix(DarkWalker __instance)
        {
            //__instance.
            MelonLogger.Log("Is exist now");
        }
    }
}