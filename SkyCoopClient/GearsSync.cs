using HarmonyLib;
using Il2Cpp;
using Il2CppRewired.HID;
using Il2CppTLD.Gear;
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
                int variant = 0;
                if ((__instance.gameObject.GetComponent<FoodItem>() != null && __instance.gameObject.GetComponent<FoodItem>().m_Opened == true) ||
                    (__instance.gameObject.GetComponent<SmashableItem>() != null && __instance.gameObject.GetComponent<SmashableItem>().m_HasBeenSmashed == true))
                {
                    variant = 1;
                }
                if (__instance.gameObject.GetComponent<KeroseneLampItem>() != null && __instance.gameObject.GetComponent<KeroseneLampItem>().m_On)
                {
                    variant = 1;
                }

                int left = Had - ShouldDrop;
                if (left > 0)
                {
                    SendDropItem(__instance, ShouldDrop, Had, false, variant);
                }
                else
                {
                    SendDropItem(__instance, 0, 0, false, variant);
                }

                if (__result != null && __result.gameObject != null && __result != __instance)
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
                if (s_InteractiveObjectUnderCrosshair)
                {
                    Comps.DroppedGearVisual Visual = s_InteractiveObjectUnderCrosshair.GetComponent<Comps.DroppedGearVisual>();
                    if (Visual)
                    {
                        TryPickUp(Visual.m_GUID);
                    }
                }
            }
        }

        //[HarmonyLib.HarmonyPatch(typeof(PlayerManager), "GetInteractiveObjectUnderCrosshairs")]
        //internal class PlayerManager_GetInteractiveObjectUnderCrosshairs
        //{
            //internal static void Postfix(PlayerManager __instance, float maxRange, ref GameObject __result)
            //{
                //int layerMask = vp_Layer.Gear | vp_Layer.NoCollidePlayer;
                //RaycastHit hit;
                //if (Physics.Raycast(GameManager.GetMainCamera().transform.position, GameManager.GetMainCamera().transform.forward, out hit, maxRange, layerMask))
                //{
                    //if (hit.collider.gameObject != null)
                    //{
                        //GameObject hitObj = hit.collider.transform.gameObject;
                        //if (hitObj.GetComponent<Comps.DroppedGearVisual>() != null)
                        //{
                            //__result = hitObj;
                        //}
                    //}
                //}

                //s_InteractiveObjectUnderCrosshair = __result;
                ////if (s_InteractiveObjectUnderCrosshair)
                ////{
                ////    SkyCoop.Logger.Log($"GetInteractiveObjectUnderCrosshairs __result {s_InteractiveObjectUnderCrosshair.name}");
                ////}
            //}
        //}

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
                s_InteractiveObjectUnderCrosshair = interactiveObj;
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

                if (nums == 0)
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
                //SkyCoop.Logger.Log(ConsoleColor.Green, "Going to deserialize...");

                GearItemSaveDataProxy DataProxy = Utils.DeserializeObject<GearItemSaveDataProxy>(JSON);
                GearItem Gi = GearObject.GetComponent<GearItem>();
                //SkyCoop.Logger.Log(ConsoleColor.Green, "JSON " + JSON);

                Gi.Deserialize(DataProxy, true);
                //SkyCoop.Logger.Log(ConsoleColor.Green, "Gear deserialized!");
                GameManager.GetPlayerManagerComponent().EnterInspectGearMode(Gi);
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
            HUDMessage.AddMessage("Picking gear failed, it no longer exist!", true, true);
            CanclePickingUp();
        }

        public static void PickUpFailedSilent()
        {
            GameAudioManager.PlayGUIError();
            CanclePickingUp();
        }

        public static void TryPickUp(string GUID)
        {
            Panel_HUD Panel;
            if(InterfaceManager.TryGetPanel<Panel_HUD>(out Panel))
            {
                s_ControlModeBeforePickingUp = GameManager.GetPlayerManagerComponent().m_ControlMode;
                GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);
                Panel.StartItemProgressBar(10, "Picking Up...", null, new System.Action(PickUpFailedSilent));
            }
            ClientSend.SendGearPickUp(GUID);
        }
    }
}
