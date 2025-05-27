using HarmonyLib;
using Il2Cpp;
using Il2CppRewired.HID;
using Il2CppTLD.Gear;
using Il2CppTLD.Interactions;
using Il2CppTLD.PDID;
using MelonLoader;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoopClient
{
    public class GearsSync
    {
        public static GameObject s_InteractiveObjectUnderCrosshair = null;
        public static PlayerControlMode s_ControlModeBeforePickingUp = PlayerControlMode.Normal;
        public static bool s_PlaceModeAfterPickup = false;

        [HarmonyLib.HarmonyPatch(typeof(GearItem), "Drop")]
        public class GearItem_Drop
        {
            private static int ShouldDrop = 0;
            private static int Had = 0;
            public static bool Prefix(GearItem __instance, int numUnits)
            {
                ShouldDrop = numUnits;

                if (__instance.m_StackableItem != null)
                {
                    Had = __instance.m_StackableItem.m_Units;
                }
                else
                {
                    Had = 1;
                }
                return true;
            }
            public static void Postfix(GearItem __instance, int numUnits, GearItem __result)
            {
                if (__instance.gameObject.GetComponent<Bed>() != null && __instance.gameObject.GetComponent<Bed>().m_BedRollState == BedRollState.Placed)
                {
                    return;
                }

                int left = Had - ShouldDrop;
                if (left > 0)
                {
                    SendDropItem(__instance, ShouldDrop, Had, false);
                }
                else
                {
                    SendDropItem(__instance, 0, 0, false);
                }

                if (__result && __result != __instance)
                {
                    UnityEngine.Object.Destroy(__result.gameObject);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InteractiveObjectsProcessInteraction")]
        public class PlayerManager_InteractiveObjectsProcessInteraction
        {
            internal static void Postfix(PlayerManager __instance)
            {
                if (__instance.ActiveInteraction != null)
                {
                    GameObject OBJ = __instance.ActiveInteraction.GetInteractiveObject();
                    if (OBJ)
                    {
                        Comps.DroppedGearVisual Visual = OBJ.GetComponent<Comps.DroppedGearVisual>();
                        if (Visual)
                        {
                            TryPickUp(Visual.m_GUID, false);
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InteractiveObjectsProcessAltFire")]
        public class PlayerManager_InteractiveObjectsProcessAltFire
        {
            internal static void Postfix(PlayerManager __instance)
            {
                SkyCoop.Logger.Log("InteractiveObjectsProcessAltFire");
                if (__instance.ActiveInteraction != null)
                {
                    GameObject OBJ = __instance.ActiveInteraction.GetInteractiveObject();
                    if (OBJ)
                    {
                        Comps.DroppedGearVisual Visual = OBJ.GetComponent<Comps.DroppedGearVisual>();
                        if (Visual)
                        {
                            TryPickUp(Visual.m_GUID, true);
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "FindInteractiveObject", new System.Type[] { typeof(RaycastHit), typeof(GearItem), typeof(GameObject) }, new ArgumentType[] {ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Ref})]
        internal class PlayerManager_FindInteractiveObject
        {
            internal static void Postfix(PlayerManager __instance, RaycastHit hit, ref GearItem gi, ref GameObject interactiveObj)
            {
                if (hit.collider && hit.collider.gameObject)
                {
                    GameObject hitObj = hit.collider.transform.gameObject;
                    if (hitObj.GetComponent<Comps.DroppedGearVisual>() != null)
                    {
                        interactiveObj = hitObj;
                        gi = null;
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InstantiateItemAtPlayersFeet", new System.Type[] { typeof(GearItem), typeof(int) })] // Once
        internal static class PlayerManager_InstantiateItemAtPlayersFeet
        {
            private static void Postfix(GearItem gearItemPrefab, int numUnits, GearItem __result)
            {
                if (__result)
                {
                    SendDropItem(__result, 0, 0, false);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InstantiateItemAtPlayersFeet", new System.Type[] { typeof(AssetReferenceGearItem), typeof(int) })]
        internal static class PlayerManager_InstantiateItemAtPlayersFeet2
        {
            private static void Postfix(AssetReferenceGearItem assetReference, int numUnits, GearItem __result)
            {
                if (__result && __result.name.Contains("GEAR_RevolverAmmoCasing"))
                {
                    SendDropItem(__result, 0, 0, false);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InstantiateItemAtLocation", new System.Type[] { typeof(GearItem), typeof(int), typeof(Vector3), typeof(bool) })]
        internal static class PlayerManager_InstantiateItemAtLocation
        {
            private static void Postfix(PlayerManager __instance, GearItem gearItemPrefab, int numUnits, Vector3 position, bool stickToGround, GearItem __result)
            {
                SkyCoop.Logger.Log($"InstantiateItemAtLocation {gearItemPrefab.name} numUnits {numUnits}");
                SendDropItem(__result, 0, 0, true);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InstantiateItemAtLocation", new System.Type[] { typeof(AssetReferenceGearItem), typeof(int), typeof(Vector3), typeof(bool) })]
        internal static class PlayerManager_InstantiateItemAtLocation2
        {
            private static void Postfix(PlayerManager __instance, AssetReferenceGearItem assetReference, int numUnits, Vector3 position, bool stickToGround, GearItem __result)
            {
                SkyCoop.Logger.Log($"InstantiateItemAtLocation GUID {assetReference.AssetGUID} numUnits {numUnits}");
                SendDropItem(__result, 0, 0, true);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "RestoreTransform")] // Once
        private static class PlayerManager_RestoreTransform
        {
            private static GameObject saveObj;
            internal static void Prefix(PlayerManager __instance)
            {
                saveObj = __instance.m_ObjectToPlace;
            }
            internal static void Postfix(PlayerManager __instance)
            {
                if (saveObj)
                {
                    GearItem gi = saveObj.GetComponent<GearItem>();
                    if (gi)
                    {
                        SkyCoop.Logger.Log("RestoreTransform");
                        SendDropItem(gi, 0, 0, true);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "PlaceMeshInWorld")] // Once
        private static class PlayerManager_PlaceMeshInWorld
        {
            private static GameObject saveObj;
            internal static void Prefix(PlayerManager __instance)
            {
                saveObj = __instance.m_ObjectToPlace;
            }
            internal static void Postfix(PlayerManager __instance)
            {
                if (saveObj)
                {
                    GearItem gi = saveObj.GetComponent<GearItem>();
                    if (gi)
                    {
                        SkyCoop.Logger.Log("PlaceMeshInWorld");
                        SendDropItem(gi, 0, 0, true);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ExitInspectGearMode")] // It supposed to check "Leave it" action to drop gear to server.
        private static class PlayerManager_ExitInspectGearMode
        {
            private static GearItem gear;
            internal static void Prefix(PlayerManager __instance)
            {
                gear = __instance.m_Gear;
            }
            internal static void Postfix(PlayerManager __instance)
            {
                //Broken. It leads to dublication bug. Because ExitInspectGearMode called even if you pickup item.
                // and for some reason, it can't find it in inventory, and I can't check if player actaully left gear or not.

                //if (gear && !gear.m_InPlayerInventory && !gear.m_InsideContainer)
                //{
                //    foreach (GearItem item in GameManager.GetInventoryComponent().m_Items)
                //    {
                //        if(item == gear)
                //        {
                //            return;
                //        }
                //    }
                //    SkyCoop.Logger.Log("ExitInspectGearMode");
                //    SendDropItem(gear, 0, 0, true);
                //}
                //__instance.m_Gear = null;
            }
        }

        public static void CookpotHelmetPatch(GearItem __instance)
        {
            if (__instance.m_CookingPotItem)
            {
                __instance.m_CookingPotItem = null;
            }
            if (__instance.name == "GEAR_CookingPot" && __instance.m_ClothingItem == null)
            {
                ClothingItem CLTH = __instance.gameObject.AddComponent<ClothingItem>();
                CLTH.m_Region = ClothingRegion.Head;
                CLTH.m_MinLayer = ClothingLayer.Mid;
                CLTH.m_MaxLayer = ClothingLayer.Mid;
                CLTH.m_DailyHPDecayWhenWornInside = 0;
                CLTH.m_DailyHPDecayWhenWornOutside = 0;
                CLTH.m_Warmth = -5f;
                CLTH.m_WarmthWhenWet = -10f;
                CLTH.m_Windproof = 50;
                CLTH.m_Toughness = 25;
                CLTH.m_SprintBarReductionPercent = 0;
                CLTH.m_Waterproofness = 1;
                CLTH.m_DryPercentPerHour = 0;
                CLTH.m_DryPercentPerHourNoFire = 0;
                CLTH.m_FreezePercentPerHour = 0;
                CLTH.m_DryBonusWhenNotWorn = 0;
                CLTH.m_PaperDollTextureName = "PaperDoll_POT";
                CLTH.m_PaperDollBlendmapName = "";
                CLTH.m_EquippedLayer = ClothingLayer.Mid;
                __instance.m_ClothingItem = CLTH;
                CLTH.m_GearItem = __instance;
            }
        }

        public static void GearManualPatch(GearItem __instance)
        {
            MeleeManager.MeeleWeaponPatch(__instance);
            CookpotHelmetPatch(__instance);
            //SkyCoop.Logger.Log($"GearManualPatch {__instance.name}");
        }


        [HarmonyLib.HarmonyPatch(typeof(GearItem), "ManualStart")]
        private static class GearItem_ManualStart
        {
            private static void Postfix(GearItem __instance)
            {
                //SkyCoop.Logger.Log($"ManualStart {__instance.name}");
                GearManualPatch(__instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GearItem), "SetDamageBlendValue")]
        private static class GearItem_SetDamageBlendValue
        {
            private static void Postfix(GearItem __instance)
            {
                if (__instance.m_ClothingItem)
                {
                    if (__instance.m_ClothingItem.IsWearing())
                    {
                        PlayersManager.s_ForceUpdateClothing = true;
                        SkyCoop.Logger.Log(ConsoleColor.Magenta, $"Gear {__instance.name} triggered s_ForceUpdateClothing");
                    }
                }
            }
        }

        public static void SendDropItem(GearItem gear, int nums = 0, int total = 0, bool samepose = false, int variant = 0, GameObject Around = null)
        {
            if (gear != null && gear.gameObject != null)
            {
                GameObject obj = gear.gameObject;

                if (samepose == false)
                {
                    if (Around == null)
                    {
                        gear.StickToGroundAtPlayerFeet(GameManager.GetPlayerTransform().position);
                    }
                    else
                    {
                        Vector3 pos = Around.transform.position;
                        float num = UnityEngine.Random.Range(0, 1.1f);
                        Vector3 vector3 = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0, 359), 0.0f) * Vector3.forward;
                        gear.StickToGroundAndOrientOnSlope(pos + vector3 * num, NavMeshCheck.IgnoreNavMesh, 0.5f);
                    }
                }

                Vector3 v3 = gear.gameObject.transform.position;
                Quaternion rot = gear.gameObject.transform.rotation;

                GearItemSaveDataProxy DataProxy;
                if (nums > 0)
                {
                    if (gear.m_StackableItem != null)
                    {
                        gear.m_StackableItem.m_Units = nums;
                    }
                }

                DataProxy = gear.Serialize();
                string JSON = Utils.SerializeObject(DataProxy);

                ClientSend.SendGear(gear.name, v3, rot, JSON);

                if (total < 2)
                {
                    UnityEngine.Object.Destroy(obj);
                }
                else
                {
                    if (gear.m_StackableItem != null)
                    {
                        gear.m_StackableItem.m_Units = total - nums;
                        GameManager.GetInventoryComponent().AddGear(obj.GetComponent<GearItem>());
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(obj);
                    }
                }
            }
        }

        public static void HandleGearDropped(DataStr.GearDataVisual Visual)
        {
            //SkyCoop.Logger.Log(ConsoleColor.Green, $"HandleGearSync {Visual.m_GearName}");
            string LocalizedGearName = "InvalidGearName";
            GameObject GearObject = AssetManager.CreateLocalizedBogusGear(Visual.m_GearName, out LocalizedGearName);
            if (GearObject != null)
            {
                //SkyCoop.Logger.Log(ConsoleColor.Green, $"Bogus created!");
                GearObject.transform.position = Visual.m_Position.ConvertToUnity();
                GearObject.transform.rotation = Visual.m_Rotation.ConvertToUnity();
                GearObject.name = Visual.m_GearName;
                Utils.SetObjectAndChildrenLayer(GearObject, vp_Layer.Gear, vp_Layer.Gear);
                ObjectGuid GUIDObj = GearObject.GetComponent<ObjectGuid>();
                if (GUIDObj == null)
                {
                    GUIDObj = GearObject.AddComponent<ObjectGuid>();
                }
                Comps.DroppedGearVisual VisualComp = GearObject.AddComponent<Comps.DroppedGearVisual>();
                VisualComp.m_GUID = Visual.m_GUID;
                VisualComp.m_LocalizedName = LocalizedGearName;

                PdidTable.RuntimeRegister(GUIDObj, Visual.m_GUID);
            }
        }

        public static void HandleGearRemove(string GUID)
        {
            GameObject GearObject = PdidTable.GetGameObject(GUID);
            if (GearObject)
            {
                //SkyCoop.Logger.Log(ConsoleColor.Green, $"HandleGearRemove {GUID} found and deleted");
                UnityEngine.Object.Destroy(GearObject);
            }
            else
            {
                //SkyCoop.Logger.Log(ConsoleColor.Red, $"HandleGearRemove {GUID} not found!");
            }
        }

        public static void HandleGearPickUp(string GearName, string JSON)
        {
            CanclePickingUp();
            //SkyCoop.Logger.Log(ConsoleColor.Green, $"HandleGearPickUp {GearName}");
            GameObject reference = AssetManager.GetAssetFromGame<GameObject>(GearName);
            if (reference)
            {
                GameObject GearObject = UnityEngine.Object.Instantiate(reference);
                GearObject.name = GearName;
                //SkyCoop.Logger.Log(ConsoleColor.Green, "Going to deserialize...");

                GearItemSaveDataProxy DataProxy = Utils.DeserializeObject<GearItemSaveDataProxy>(JSON);
                GearItem Gi = GearObject.GetComponent<GearItem>();
                //SkyCoop.Logger.Log(ConsoleColor.Green, "JSON " + JSON);
                Gi.Deserialize(DataProxy, true);
                GearManualPatch(Gi);
                //SkyCoop.Logger.Log(ConsoleColor.Green, "Gear deserialized!");

                if (s_PlaceModeAfterPickup)
                {
                    GameManager.GetPlayerManagerComponent().StartPlaceMesh(Gi.gameObject, PlaceMeshFlags.None);
                }
                else
                {
                    GameManager.GetPlayerManagerComponent().EnterInspectGearMode(Gi);
                }
            }
        }

        public static void CanclePickingUp()
        {
            GameManager.GetPlayerManagerComponent().SetControlMode(s_ControlModeBeforePickingUp);
            Panel_HUD Panel;
            if (InterfaceManager.TryGetPanel<Panel_HUD>(out Panel))
            {
                Panel.CancelItemProgressBar();
            }
        }

        public static void PickUpFailed()
        {
            GameAudioManager.PlayGUIError();
            HUDMessage.AddMessage("Failed, gear no longer exist!", true, true);
            CanclePickingUp();
        }

        public static void PickUpFailedSilent()
        {
            GameAudioManager.PlayGUIError();
            CanclePickingUp();
        }

        public static void TryPickUp(string GUID, bool PlaceMode = false)
        {
            Panel_HUD Panel;
            if(InterfaceManager.TryGetPanel<Panel_HUD>(out Panel))
            {
                s_ControlModeBeforePickingUp = GameManager.GetPlayerManagerComponent().m_ControlMode;
                GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);
                Panel.StartItemProgressBar(10, "Picking Up...", null, new System.Action(PickUpFailedSilent));
            }
            s_PlaceModeAfterPickup = PlaceMode;
            ClientSend.SendGearPickUp(GUID);
        }
    }
}
