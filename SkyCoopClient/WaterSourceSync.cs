using Il2Cpp;
using Il2CppTLD.IntBackedUnit;
using Il2CppTLD.PDID;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoopClient
{
    public static class WaterSourceSync
    {
        public static void HandleWaterSource(string GUID, float Current, bool IsGood)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);

            if (Obj)
            {
                WaterSource Source = Obj.GetComponent<WaterSource>();

                if(Source)
                {
                    Source.m_CurrentLiquidQuality = IsGood ? LiquidQuality.Potable : LiquidQuality.NonPotable;
                    Source.m_CurrentLiters = new Il2CppTLD.IntBackedUnit.ItemLiquidVolume(FireHook.ConvertVolumeToUnits(Current));

                    Panel_PickWater Panel = InterfaceManager.GetPanel<Panel_PickWater>();
                    if (Panel)
                    {
                        Panel.m_WaterSource = Source;
                        GearItem Gi = GameManager.GetInventoryComponent().GetWaterSupply(Source.m_CurrentLiquidQuality);
                        if (Gi && Gi.m_WaterSupply)
                        {
                            Panel.Enable(true);
                            Panel.m_EnablePanelOnExit = EnablePanelOnExit.None;
                            Panel.SetWaterSourceForTaking(Source, Gi.m_WaterSupply);
                        }
                        else
                        {
                            ClientSend.SendFinishInteract();
                        }
                    }
                }
                else
                {
                    ClientSend.SendFinishInteract();
                }
            }
            else
            {
                ClientSend.SendFinishInteract();
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(WaterSource), "PerformInteraction")]
        private static class WaterSource_PerformInteraction
        {
            private static bool Prefix(WaterSource __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                ObjectGuid ObjGUID = __instance.gameObject.GetComponent<ObjectGuid>();

                if (ObjGUID)
                {
                    ClientSend.SendWaterSource(ObjGUID.Get(), FireHook.ConvertLiquidVolume(__instance.m_MinStarting), FireHook.ConvertLiquidVolume(__instance.m_MinStarting), __instance.m_ChanceContaminated);
                }
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_PickWater), "Enable")]
        private static class Panel_PickWater_Enable
        {
            private static void Postfix(Panel_PickWater __instance, bool enable)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (!enable)
                {
                    ClientSend.SendFinishInteract();
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_PickWater), "OnTakeWaterComplete")]
        private static class Panel_PickWater_OnTakeWaterComplete
        {
            public static long s_WaterBeforeTaking;
            public static WaterSource s_WaterSourceBeforeTaking;
            private static void Prefix(Panel_PickWater __instance, bool success, bool playerCancel, float progress)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if(__instance.m_WaterSource)
                {
                    s_WaterBeforeTaking = __instance.m_WaterSource.m_CurrentLiters.m_Units;
                    s_WaterSourceBeforeTaking = __instance.m_WaterSource;
                }
            }

            private static void Postfix(Panel_PickWater __instance, bool success, bool playerCancel, float progress)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (s_WaterSourceBeforeTaking)
                {
                    ObjectGuid ObjGUID = s_WaterSourceBeforeTaking.GetComponent<ObjectGuid>();

                    if (ObjGUID)
                    {
                        long CurrentWaterLeft = s_WaterSourceBeforeTaking.m_CurrentLiters.m_Units;
                        float Took = FireHook.ConvertVolume(s_WaterBeforeTaking - CurrentWaterLeft);
                        ClientSend.SendTookWaterFromWaterSource(ObjGUID.Get(), FireHook.ConvertVolume(s_WaterBeforeTaking - CurrentWaterLeft));
                    }
                }
            }
        }
    }
}
