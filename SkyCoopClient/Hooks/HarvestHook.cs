using Il2Cpp;
using Il2CppTLD.PDID;
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
    public static class HarvestHook
    {
        public static void CancleHarvest(Harvestable Harvestable)
        {
            if (GameManager.GetPlayerManagerComponent().m_Harvestable == Harvestable)
            {
                GameManager.GetPlayerManagerComponent().TryCancelHoldInteraction();
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Harvestable), "Harvest")]
        private static class Harvestable_Harvest
        {
            private static void Postfix(Harvestable __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                ClientSend.SendFinishInteract();

                ObjectGuid ObjGUID = __instance.gameObject.GetComponent<ObjectGuid>();
                if (ObjGUID)
                {
                    ClientSend.SendHarvest(ObjGUID.Get(), __instance.m_RefreshHoursMin, __instance.m_RefreshHoursMax);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Harvestable), "EnterInspectMode")]
        private static class Harvestable_EnterInspectMode
        {
            private static void Prefix(Harvestable __instance, GearItem gearPrefab)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                GearsSync.s_NoSyncFlag = true;
            }

            private static void Postfix(Harvestable __instance, GearItem gearPrefab)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                GearsSync.s_NoSyncFlag = false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(HarvestableInteraction), "PerformHold")]
        private static class HarvestableInteraction_PerformHold
        {
            private static void Postfix(HarvestableInteraction __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                PlayersManager.TryInteract(__instance);
            }
        }

        public static void HandleRemove(string GUID)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if(Obj != null)
            {
                Harvestable Harvest = Obj.GetComponent<Harvestable>();

                if(Harvest != null)
                {
                    Harvest.m_Harvested = true;
                    Harvest.gameObject.SetActive(false);

                    if (Harvest.m_ActivateObjectPostHarvest)
                    {
                        Harvest.m_ActivateObjectPostHarvest.SetActive(true);
                    }
                }
            }
        }

        public static void HandleRegrow(string GUID)
        {
            GameObject Obj = PdidTable.GetGameObject(GUID);
            if (Obj != null)
            {
                Harvestable Harvest = Obj.GetComponent<Harvestable>();

                if (Harvest != null)
                {
                    Harvest.m_Harvested = false;
                    Harvest.gameObject.SetActive(true);
                }
            }
        }


        [HarmonyLib.HarmonyPatch(typeof(HarvestableManager), "DeserializeAll")]
        private static class HarvestableManagern_DeserializeAll
        {
            private static bool Prefix(HarvestableManager __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(HarvestableManager), "SerializeAll")]
        private static class HarvestableManagern_SerializeAll
        {
            private static bool Prefix(HarvestableManager __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return false;
            }
        }
    }
}
