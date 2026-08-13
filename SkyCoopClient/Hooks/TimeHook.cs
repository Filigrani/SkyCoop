using Il2Cpp;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace SkyCoopClient
{
    public static class TimeHook
    {
        //[HarmonyLib.HarmonyPatch(typeof(TimeOfDay), "Accelerate")]
        //private static class TimeOfDay_Accelerate
        //{
        //    private static bool Prefix(TimeOfDay __instance, float realTimeSeconds, float gameTimeHours, bool doFadeToBlack)
        //    {
        //        if (doFadeToBlack)
        //        {
        //            Color black = Color.black;
        //            float fadeDuration = 1f;
        //            if (!GameManager.GetRestComponent().IsSleeping())
        //            {
        //                Panel_HUD Panel = InterfaceManager.GetPanel<Panel_HUD>();

        //                if (Panel)
        //                {
        //                    black.a = Panel.m_AccelTimePopup.m_NonFullFadeValue;
        //                    fadeDuration = Panel.m_AccelTimePopup.m_NonFullFadeDuration;
        //                }
        //            }
        //            CameraFade.Fade(black.a, fadeDuration, 0.1f);
        //        }
        //        return false;
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(TimeOfDay), "AccelerateCo")]
        //private static class TimeOfDay_AccelerateCo
        //{
        //    private static bool Prefix(TimeOfDay __instance)
        //    {
        //        return false;
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(TimeOfDay), "AccelerateTime")]
        //private static class TimeOfDay_AccelerateTime
        //{
        //    private static bool Prefix(TimeOfDay __instance)
        //    {
        //        return false;
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(TimeOfDay), "SetDayLengthScale")]
        //private static class TimeOfDay_SetDayLengthScale
        //{
        //    private static bool Postfix(TimeOfDay __instance, float scale)
        //    {
        //        scale = 1;
        //        __instance.m_DayLengthScale = scale;
        //        __instance.m_WeatherSystem.m_DayLengthScale = __instance.m_DayLengthScale * __instance.m_DayLengthScaleDebug;
        //        GameManager.GetHighResolutionTimerManager().UpdateTimeScale((double)scale);

        //        Panel_HUD Panel = InterfaceManager.GetPanel<Panel_HUD>();

        //        if (__instance.IsTimeLapseActive())
        //        {
        //            Panel.EnableTimeOfDayScaleDisplay(true);
        //            return false;
        //        }
        //        Panel.EnableTimeOfDayScaleDisplay(false);
        //        return false;
        //    }
        //}
    }
}
