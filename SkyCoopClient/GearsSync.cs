using HarmonyLib;
using Il2Cpp;
using Il2CppRewired;
using Il2CppRewired.HID;
using Il2CppTLD.Cooking;
using Il2CppTLD.Gear;
using Il2CppTLD.IntBackedUnit;
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
using UnityEngine.AddressableAssets;

namespace SkyCoopClient
{
    public class GearsSync
    {
        public static GameObject s_InteractiveObjectUnderCrosshair = null;
        public static PlayerControlMode s_ControlModeBeforePickingUp = PlayerControlMode.Normal;
        public static bool s_PlaceModeAfterPickup = false;
        public static bool s_NoSyncFlag = false;
        public static Vector3 s_LastPickedGearPosition = Vector3.zero;
        public static Quaternion s_LastPickedGearRotation = Quaternion.identity;
        public static List<Vector3> s_SpawnersMarkers = new List<Vector3>();
        public static List<GameObject> s_SpawnersMarkersObjects = new List<GameObject>();

        public static List<GearPickedElement> s_GearQueue = new List<GearPickedElement>();
        public static CookingSlot s_LastCookingSlotGearPickedFrom = null;
        public static float s_LastGearTimeBeingCooked = 0;
        public static bool s_LiquidCookingDebug = false;
        public static string s_LastPickedGearGUID = string.Empty;

        public class GearPickedElement
        {
            public string m_GearName = "";
            public string m_JSON = "";
            public bool m_DropAround = false;
            public bool m_SpawnLoaded = false;
            public float m_TimeBeingCooked = 0;
            public string m_CookingResult = "";
            public float m_Volume = 1;

            public GearPickedElement(string gearName, string json, bool dropAround = false, bool spawnLoaded = false, float timebeingcooked = 0, string cookingresult = "", float volume = 1)
            {
                m_GearName = gearName;
                m_JSON = json;
                m_DropAround = dropAround;
                m_SpawnLoaded = spawnLoaded;
                m_TimeBeingCooked = timebeingcooked;
                m_CookingResult = cookingresult;
                m_Volume = volume;
            }
        }

        public static void Update()
        {
            if(s_GearQueue.Count > 0)
            {
                if(GameManager.m_PlayerManager != null)
                {
                    if (!GameManager.m_PlayerManager.IsInspectModeActive())
                    {
                        GearPickedElement Element = s_GearQueue[0];

                        ProcessGearPickUpQueue(Element);
                        s_GearQueue.RemoveAt(0);
                    }
                }
            }
        }


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
                if (!ModMain.IsMultiplayer()) { return; }

                if (s_NoSyncFlag)
                {
                    return;
                }

                if (ModMain.Client.m_IsReady && !ModMain.Client.m_Rules.m_CanDropItems)
                {
                    UnityEngine.Object.Destroy(__result.gameObject);
                    return;
                }

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

        [HarmonyLib.HarmonyPatch(typeof(ItemDescriptionPage), "CanDrop", new System.Type[] { typeof(GearItem) })]
        private static class ItemDescriptionPage_CanDrop
        {
            private static void Postfix(ItemDescriptionPage __instance, GearItem gi, ref bool __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (s_NoSyncFlag)
                {
                    return;
                }

                if (gi != null)
                {
                    if(ModMain.Client.m_IsReady && !ModMain.Client.m_Rules.m_CanDropItems)
                    {
                        __result = false;
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Container), "ItemPassesFilter", new System.Type[] { typeof(GearItem), typeof(string) })]
        private static class Panel_Container_ItemPassesFilter
        {
            private static void Postfix(Panel_Container __instance, GearItem pi, ref bool __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (s_NoSyncFlag)
                {
                    return;
                }

                if (ModMain.Client.m_IsReady && !ModMain.Client.m_Rules.m_CanDropItems)
                {
                    __result = false;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InteractiveObjectsProcessInteraction")]
        public class PlayerManager_InteractiveObjectsProcessInteraction
        {
            internal static bool Prefix(PlayerManager __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (__instance.ActiveInteraction != null)
                {
                    GameObject OBJ = __instance.ActiveInteraction.GetInteractiveObject();
                    if (OBJ)
                    {
                        VehicleDoor door = OBJ.GetComponent<VehicleDoor>();

                        if (door && PlayersManager.TryInteract(door))
                        {
                            return false;
                        }

                        Container container = OBJ.GetComponent<Container>();

                        if (container && container.enabled && PlayersManager.TryInteract(container))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            internal static void Postfix(PlayerManager __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (__instance.ActiveInteraction != null)
                {
                    GameObject OBJ = __instance.ActiveInteraction.GetInteractiveObject();
                    if (OBJ)
                    {
                        Comps.DroppedGearVisual Visual = OBJ.GetComponent<Comps.DroppedGearVisual>();
                        if (Visual)
                        {
                            TryPickUp(Visual, false);
                        }
                        Comps.CardGameProp CardGameProp = OBJ.GetComponent<Comps.CardGameProp>();
                        if(CardGameProp)
                        {
                            CardGameProp.TryUse();
                        }
                        Comps.TexasHoldEmJoin JoinGame = OBJ.GetComponent<Comps.TexasHoldEmJoin>();
                        if (JoinGame)
                        {
                            JoinGame.TryUse();
                        }
                        Comps.TexasHoldEmPlay Play = OBJ.GetComponent<Comps.TexasHoldEmPlay>();
                        if (Play)
                        {
                            Play.TryUse();
                        }
                        Comps.PropsEditorVisuzlier Vizual = OBJ.GetComponent<Comps.PropsEditorVisuzlier>();
                        if (Vizual)
                        {
                            Vizual.Place();
                        }
                        Comps.NetworkPlayer Player = OBJ.GetComponent<Comps.NetworkPlayer>();
                        if (Player)
                        {
                            
                            if(Player.m_Action == Comps.NetworkPlayer.Actions.Knocked)
                            {
                                if (!GameManager.GetBrokenBody().HasAffliction)
                                {
                                    PlayersManager.TryReviveOtherPlayer(Player);
                                }
                                else
                                {
                                    HUDMessage.AddMessage("Revive yourself first, dummy!", true, true);
                                }
                            }
                            else
                            {
                                if(ModMain.Client.m_Config.m_GameMode == "Lobby")
                                {
                                    if (PlayersManager.s_InSquad)
                                    {
                                        ClientSend.SendInviteToSquad(Player.m_PlayerID);
                                    }
                                }
                            }
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
                if (!ModMain.IsMultiplayer()) { return; }

                if (__instance.ActiveInteraction != null)
                {
                    GameObject OBJ = __instance.ActiveInteraction.GetInteractiveObject();
                    if (OBJ)
                    {
                        Comps.DroppedGearVisual Visual = OBJ.GetComponent<Comps.DroppedGearVisual>();
                        if (Visual)
                        {
                            TryPickUp(Visual, true);
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
                if (!ModMain.IsMultiplayer()) { return; }

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
                if (!ModMain.IsMultiplayer()) { return; }

                if (s_NoSyncFlag)
                {
                    return;
                }

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
                if (!ModMain.IsMultiplayer()) { return; }

                if (s_NoSyncFlag)
                {
                    return;
                }

                if (__result && __result.name.Contains("GEAR_RevolverAmmoCasing"))
                {
                    SendDropItem(__result, 0, 0, false);
                }
                SkyCoop.Logger.Log($"InstantiateItemAtPlayersFeet numUnits {numUnits}");
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InstantiateItemAtLocation", new System.Type[] { typeof(GearItem), typeof(int), typeof(Vector3), typeof(bool) })]
        internal static class PlayerManager_InstantiateItemAtLocation
        {
            private static void Postfix(PlayerManager __instance, GearItem gearItemPrefab, int numUnits, Vector3 position, bool stickToGround, GearItem __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (s_NoSyncFlag)
                {
                    return;
                }

                if (__result && __result.name == "GEAR_CookingPotDummy")
                {
                    SkyCoop.Logger.Log($"InstantiateItemAtLocation {gearItemPrefab.name} cancled");
                    UnityEngine.Object.Destroy(__result.gameObject);
                    return;
                }

                SkyCoop.Logger.Log($"InstantiateItemAtLocation {gearItemPrefab.name} numUnits {numUnits}");
                SendDropItem(__result, 0, 0, true);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InstantiateItemAtLocation", new System.Type[] { typeof(AssetReferenceGearItem), typeof(int), typeof(Vector3), typeof(bool) })]
        internal static class PlayerManager_InstantiateItemAtLocation2
        {
            private static void Postfix(PlayerManager __instance, AssetReferenceGearItem assetReference, int numUnits, Vector3 position, bool stickToGround, GearItem __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (s_NoSyncFlag)
                {
                    return;
                }


                if (__result && __result.name == "GEAR_CookingPotDummy")
                {
                    UnityEngine.Object.Destroy(__result.gameObject);
                    return;
                }

                SkyCoop.Logger.Log($"InstantiateItemAtLocation2 {__result.name} numUnits {numUnits}");
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
                if (!ModMain.IsMultiplayer()) { return; }

                if (saveObj)
                {
                    GearItem gi = saveObj.GetComponent<GearItem>();
                    if (gi)
                    {
                        SkyCoop.Logger.Log($"RestoreTransform {gi.name}");

                        if (s_LastCookingSlotGearPickedFrom)
                        {
                            FireHook.DoCookingAction("DoFirePickerAction", gi, s_LastCookingSlotGearPickedFrom.gameObject, s_LastGearTimeBeingCooked);
                        }
                        else
                        {
                            SendDropItem(gi, 0, 0, true);
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "PlaceMeshInWorld")] // Once
        private static class PlayerManager_PlaceMeshInWorld
        {
            private static GameObject saveObj;
            private static bool s_SkipPost = false;
            internal static bool Prefix(PlayerManager __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }
                saveObj = __instance.m_ObjectToPlace;

                if (saveObj)
                {
                    GearItem gi = saveObj.GetComponent<GearItem>();
                    if (gi)
                    {
                        if (gi)
                        {
                            if (__instance.m_IsPlacingCookableOnCookingSlot && __instance.m_LastGearPlacePoint)
                            {
                                CookingSlot Slot = FireHook.GetCookingSlotFromPlacePoint(__instance.m_LastGearPlacePoint);
                                if (Slot)
                                {
                                    // Где то внутри кода игры этот предмет будет уничтожен после размещения на слот готовки!
                                    // я долго копался и не нашёл где именно, по этому позволю ему удалиться но создам клон.

                                    // Скорее всего этот как то связанно с фейковыми ёмкостями для готовки, которые ванила спавнит

                                    gi.transform.position = Vector3.zero;

                                    GearItemSaveDataProxy DataProxy = gi.Serialize();

                                    GameObject Reference = AssetManager.GetAssetFromGame<GameObject>(gi.name);

                                    if (Reference)
                                    {
                                        GameObject CloneObj = UnityEngine.Object.Instantiate(Reference, gi.transform.position, gi.transform.rotation);
                                        if (CloneObj)
                                        {
                                            CloneObj.name = gi.name;
                                            GearItem CloneGi = CloneObj.GetComponent<GearItem>();

                                            if (CloneGi)
                                            {
                                                CloneGi.Deserialize(DataProxy, true);
                                                FireHook.DoCookingAction("DoFirePickerAction", CloneGi, Slot.gameObject, s_LastGearTimeBeingCooked);
                                                s_SkipPost = true;
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            internal static void Postfix(PlayerManager __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (s_SkipPost)
                {
                    s_SkipPost = false;
                    return;
                }

                if (saveObj)
                {
                    GearItem gi = saveObj.GetComponent<GearItem>();
                    if (gi)
                    {
                        SkyCoop.Logger.Log($"PlaceMeshInWorld {gi.name}");
                        if (gi)
                        {
                            SendDropItem(gi, 0, 0, true);
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ExitInspectGearMode")]
        private static class PlayerManager_ExitInspectGearMode
        {
            public static GearItem Gear = null;
            
            internal static void Prefix(PlayerManager __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }


                Gear = __instance.m_Gear;
            }

            internal static void Postfix(PlayerManager __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (__instance.m_Gear)
                {
                    SkyCoop.Logger.Log($"ExitInspectGearMode {__instance.m_Gear.name}");

                    if(__instance.m_Gear && __instance.m_Gear.m_CookingPotItem)
                    {
                        if (__instance.m_Gear.m_InPlayerInventory)
                        {
                            if (__instance.m_Gear.m_CookingPotItem.m_LitersSnowBeingMelted.m_Units > 0 || __instance.m_Gear.m_CookingPotItem.m_LitersWaterBeingBoiled.m_Units > 0)
                            {
                                if (__instance.m_Gear.m_CookingPotItem.m_LitersSnowBeingMelted.m_Units > 0)
                                {
                                    SkyCoop.Logger.Log($"Took water {FireHook.ConvertLiquidVolume(__instance.m_Gear.m_CookingPotItem.m_LitersSnowBeingMelted * __instance.m_Gear.m_CookingPotItem.m_PercentCooked)}l of bad water (cooking progress {__instance.m_Gear.m_CookingPotItem.m_PercentCooked} cooking state {__instance.m_Gear.m_CookingPotItem.m_CookingState})");
                                }
                                if (__instance.m_Gear.m_CookingPotItem.m_LitersWaterBeingBoiled.m_Units > 0)
                                {
                                    SkyCoop.Logger.Log($"Took water {FireHook.ConvertLiquidVolume(__instance.m_Gear.m_CookingPotItem.m_LitersWaterBeingBoiled * __instance.m_Gear.m_CookingPotItem.m_PercentCooked)}l of good water (cooking progress {__instance.m_Gear.m_CookingPotItem.m_PercentCooked} cooking state {__instance.m_Gear.m_CookingPotItem.m_CookingState})");

                                    GameManager.GetInventoryComponent().GetPotableWaterSupply(); // Из-за нашей системы стартового лута, у игрока может не оказаться бутылки под воду, вызвав этот метод игра создаст бутылку если её нет.
                                }

                                __instance.m_Gear.m_CookingPotItem.PickUpCookedItem();
                                __instance.m_Gear.m_CookingPotItem.m_LitersSnowBeingMelted = new ItemLiquidVolume(0);
                                __instance.m_Gear.m_CookingPotItem.m_LitersWaterBeingBoiled = new ItemLiquidVolume(0);

                                return;
                            }
                        }
                        else
                        {
                            SkyCoop.Logger.Log($"Is left m_LitersSnowBeingMelted {__instance.m_Gear.m_CookingPotItem.m_LitersSnowBeingMelted.m_Units} m_LitersWaterBeingBoiled {__instance.m_Gear.m_CookingPotItem.m_LitersWaterBeingBoiled.m_Units}");

                            if (__instance.m_Gear.m_CookingPotItem.m_LitersSnowBeingMelted.m_Units > 0 || __instance.m_Gear.m_CookingPotItem.m_LitersWaterBeingBoiled.m_Units > 0)
                            {
                                string ResultGear = "";
                                float CookingTime = 0;
                                float Volume = 0;
                                if (__instance.m_Gear.m_CookingPotItem.m_LitersSnowBeingMelted.m_Units > 0)
                                {
                                    ResultGear = "BadWater";
                                    Volume = FireHook.ConvertLiquidVolume(__instance.m_Gear.m_CookingPotItem.m_LitersSnowBeingMelted);
                                    CookingTime = (__instance.m_Gear.m_CookingPotItem.m_CookSettings.m_MinutesToMeltSnowPerLiter * Volume) / 60f;
                                }
                                if (__instance.m_Gear.m_CookingPotItem.m_LitersWaterBeingBoiled.m_Units > 0)
                                {
                                    ResultGear = "GoodWater";
                                    Volume = FireHook.ConvertLiquidVolume(__instance.m_Gear.m_CookingPotItem.m_LitersWaterBeingBoiled);
                                    CookingTime = (__instance.m_Gear.m_CookingPotItem.m_CookSettings.m_MinutesToBoilWaterPerLiter * Volume) / 60f;
                                }
                                __instance.m_Gear.m_CookingPotItem.m_CookingState = CookingPotItem.CookingState.Cooking;
                                __instance.m_Gear.m_CookingPotItem.m_LitersSnowBeingMelted = new ItemLiquidVolume(0);
                                __instance.m_Gear.m_CookingPotItem.m_LitersWaterBeingBoiled = new ItemLiquidVolume(0);
                                Comps.GearCookingTarget Target = __instance.m_Gear.gameObject.GetComponent<Comps.GearCookingTarget>();
                                if (Target == null)
                                {
                                    Target = __instance.m_Gear.gameObject.AddComponent<Comps.GearCookingTarget>();
                                }
                                Target.m_Volume = Volume;
                                Target.m_CookingResult = ResultGear;
                            }
                        }
                    }


                    if (!__instance.m_Gear.m_InPlayerInventory)
                    {
                        __instance.m_Gear.transform.position = __instance.m_RestorePos;
                        __instance.m_Gear.transform.rotation = __instance.m_RestoreRot;

                        // Проблема в том что если этот предмет состакан - то __instance.m_Gear будет помечен для уничтожения.
                        // по факту он не попал в инвентарь, потому что вместо него самого, просто прибавилось цифра в стаке.
                        // Так как Unity удаляет помеченные предметы только в следующем Update цикле (да блин даже если мы создали компонент
                        // в текущем цикле его Update всё ещё вызовиться в этом цикле), лепив на него компонент, если он сможет вызвать Update
                        // значит он так и остался валяться.


                        Comps.SendGearIfNotDestoryed Hook = __instance.m_Gear.gameObject.GetComponent<Comps.SendGearIfNotDestoryed>();
                        if(Hook == null)
                        {
                            Hook = __instance.m_Gear.gameObject.AddComponent<Comps.SendGearIfNotDestoryed>();
                            Hook.m_Gear = __instance.m_Gear;
                        }
                    }
                    else
                    {
                        s_LastPickedGearGUID = string.Empty;
                    }
                }
            }
        }

        public static void CookpotHelmetPatch(GearItem __instance)
        {
            //if (__instance.m_CookingPotItem)
            //{
            //    __instance.m_CookingPotItem = null;
            //}
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

        public static void ApplyTextureDoner(GameObject Obj, string GearName = "")
        {
            if (string.IsNullOrEmpty(GearName))
            {
                GearName = Obj.name;
            }
            if (GearName == "GEAR_FishKnife")
            {
                GameObject TextureDoner = AssetManager.GetAssetFromGame<GameObject>("GEAR_RawCohoSalmon");

                if (TextureDoner)
                {
                    Obj.transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material = TextureDoner.transform.GetChild(0).GetComponent<Renderer>().material;
                }
            }
        }

        public static void CanLauncherPatch(GearItem __instance)
        {
            if (__instance.name == "GEAR_CanLauncher")
            {
                GameObject FlareGunPrefab = AssetManager.GetAssetFromGame<GameObject>("GEAR_FlareGun");


                GameObject AmmoPrefab = AssetManager.GetAssetFromGame<GameObject>("GEAR_NoiseMaker");
                GameObject AmmoObject = UnityEngine.Object.Instantiate(AmmoPrefab);

                GunItem GunItemDoner = FlareGunPrefab.GetComponent<GunItem>();
                FirstPersonItem FPIDoner = FlareGunPrefab.GetComponent<FirstPersonItem>();

                
                GunItem GunItem = __instance.gameObject.AddComponent<GunItem>();
                GunItem.m_AccuracyRange = GunItemDoner.m_AccuracyRange;
                GunItem.m_AmmoSpriteName = "ico_units_noisemaker";
                GunItem.m_AimButtonLabel = GunItemDoner.m_AimButtonLabel;

                UnityEngine.AddressableAssets.AssetReferenceT < GearItemData > G = new UnityEngine.AddressableAssets.AssetReferenceT<GearItemData>(AmmoObject.GetComponent<GearItem>().m_GearItemData.PrefabReference.AssetGUID);
                GunItem.m_AmmoReferences = new Il2CppSystem.Collections.Generic.List<AssetReferenceT<GearItemData>>();
                GunItem.m_AmmoReferences.Add(G);
                GunItem.m_CasingAudio = "";

                GunItem.m_ClipSize = 1;
                GunItem.m_DamageHP = 100;
                GunItem.m_FireAudio = GunItemDoner.m_FireAudio;
                GunItem.m_FireDelayAfterReload = 1.5f;
                GunItem.m_FireDelayOnAim = 0.25f;
                GunItem.m_FiringRateSeconds = 1f;

                GunItem.m_DryFireAudio = GunItemDoner.m_DryFireAudio;
                GunItem.m_FireDelayOnAim = GunItemDoner.m_FireDelayOnAim;
                GunItem.m_GunType = GunType.FlareGun;
                GunItem.m_MultiplierFire = 1;

                __instance.m_GunItem = GunItem;

                FirstPersonItem FPI = __instance.gameObject.AddComponent<FirstPersonItem>();
                FPI.m_FirstPersonObjectName = "FlareGun";
                FPI.m_ItemData = FPIDoner.m_ItemData;
                FPI.m_PlayerStateTransitions = FPIDoner.m_PlayerStateTransitions;
                FPI.m_UnwieldAudioEvent = FPIDoner.m_UnwieldAudioEvent;
                FPI.m_WieldAudioEvent = FPIDoner.m_WieldAudioEvent;

                __instance.m_FirstPersonItem = FPI;
            }else if(__instance.name == "GEAR_NoiseMaker")
            {
                AmmoItem Ammo = __instance.gameObject.AddComponent<AmmoItem>();

                Ammo.m_LoadedHudIconSpriteName = "ico_units_noisemaker";
                Ammo.m_AmmoForGunType = GunType.FlareGun;

                __instance.m_AmmoItem = Ammo;
            }
        }

        public static void GearManualPatch(GearItem __instance)
        {
            MeleeManager.MeeleWeaponPatch(__instance);
            CookpotHelmetPatch(__instance);
            CanLauncherPatch(__instance);
            UncookedGearsFix.UncookedGearPatch(__instance.gameObject);
            CraftingHook.ManualPatchHarvest(__instance);
            //SkyCoop.Logger.Log($"GearManualPatch {__instance.name}");
        }


        [HarmonyLib.HarmonyPatch(typeof(GearItem), "ManualStart")]
        private static class GearItem_ManualStart
        {
            private static void Postfix(GearItem __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                //SkyCoop.Logger.Log($"ManualStart {__instance.name}");
                GearManualPatch(__instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GearItem), "SetDamageBlendValue")]
        private static class GearItem_SetDamageBlendValue
        {
            private static void Postfix(GearItem __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

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

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "TransferGearFromInspectToContainer")]
        private static class PlayerManager_TransferGearFromInspectToContainer
        {
            private static bool Prefix(PlayerManager __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (__instance.m_Gear)
                {
                    SkyCoop.Logger.Log($"Gear {__instance.m_Gear.name} refused and dropped near container");

                    if (__instance.m_Container)
                    {
                        __instance.m_Container.RemoveGear(__instance.m_Gear);
                    }
                    SendDropItem(__instance.m_Gear, 0, 0, false, 0, GameManager.GetPlayerTransform().gameObject);
                    return false;    
                }
                return true;
            }
        }

        public static void SendDropItem(GearItem gear, int nums = 0, int total = 0, bool samepose = false, int variant = 0, GameObject Around = null)
        {
            if (!ModMain.IsMultiplayer()) { return; }
            if (gear != null && gear.gameObject != null)
            {
                //if(gear.m_CookingPotItem && gear.m_CookingPotItem.IsDummyPot())
                //{
                //    CookingSlot DummysCookingSlot = null;
                //    if (gear.m_CookingPotItem.m_GearPlacePointAttachedTo != null)
                //    {
                //        SkyCoop.Logger.Log(ConsoleColor.Magenta, $"Dummy cookingpot has cooking slot");
                //        if (gear.m_CookingPotItem.m_GearPlacePointAttachedTo)
                //        {
                //            DummysCookingSlot = FireHook.GetCookingSlotFromPlacePoint(gear.m_CookingPotItem.m_GearPlacePointAttachedTo);
                //        }
                //    }

                //    GameObject Dummy = gear.m_CookingPotItem.gameObject;
                //    gear = gear.m_CookingPotItem.m_GearItemBeingCooked;

                //    if(gear && gear.GetComponent<Comps.GearCookingTarget>() == null && DummysCookingSlot)
                //    {
                //        UnityEngine.Object.Destroy(Dummy);
                //        FireHook.DoCookingAction("DoFirePickerAction", gear, DummysCookingSlot.gameObject, s_LastGearTimeBeingCooked);
                //        return;
                //    }

                //    UnityEngine.Object.Destroy(Dummy);

                //    if (gear == null)
                //    {
                //        return;
                //    }
                //}


                GameObject obj = gear.gameObject;

                string FireGUID = "";
                int CookingSlotIndex = -1;
                string CookingResult = "";;
                float Volume = 1;
                float BeingCookedTime = 0;
                float NormalizedCondition = gear.GetNormalizedCondition();
                int Style = 0;
                string CookpotGUID = "";

                if (gear.m_FoodItem && gear.m_FoodItem.m_Opened)
                {
                    Style = 1;
                }
                if (gear.m_SmashableItem && gear.m_SmashableItem.m_HasBeenSmashed)
                {
                    Style = 1;
                }
                if (gear.m_Bed && gear.m_Bed.m_BedRollState == BedRollState.Placed)
                {
                    Style = 1;
                }
                if (gear.m_FlareItem)
                {
                    switch (gear.m_FlareItem.m_State)
                    {
                        case FlareState.Fresh:
                            Style = 0;
                            break;
                        case FlareState.Burning:
                            Style = 1;
                            break;
                        case FlareState.Paused:
                        case FlareState.BurnedOut:
                        case FlareState.Wet:
                            Style = 2;
                            break;
                        default:
                            break;
                    }
                }
                if (gear.m_TorchItem)
                {
                    switch (gear.m_TorchItem.m_State)
                    {
                        case TorchState.Fresh:
                            Style = 0;
                            break;
                        case TorchState.Burning:
                            Style = 1;
                            break;
                        case TorchState.Paused:
                        case TorchState.BurnedOut:
                        case TorchState.Wet:
                            Style = 2;
                            break;
                        default:
                            break;
                    }
                }

                Vector3 v3 = gear.gameObject.transform.position;
                Quaternion rot = gear.gameObject.transform.rotation;

                Comps.GearCookingTarget CookingTarget = obj.GetComponent<Comps.GearCookingTarget>();

                if (CookingTarget)
                {
                    FireGUID = CookingTarget.m_FireGUID;
                    CookingSlotIndex = CookingTarget.m_CookingIndex;
                    CookingResult = CookingTarget.m_CookingResult;
                    Volume = CookingTarget.m_Volume;
                    BeingCookedTime = CookingTarget.m_TimeBeingCooked;
                    CookpotGUID = CookingTarget.m_CookpotGUID;

                    if (CookingTarget.m_PlacePoint)
                    {
                        obj.transform.position = CookingTarget.m_PlacePoint.transform.position;
                        obj.transform.rotation = CookingTarget.m_PlacePoint.transform.rotation;
                    }

                    v3 = gear.gameObject.transform.position;
                    rot = gear.gameObject.transform.rotation;
                    samepose = true;
                }

                if (string.IsNullOrEmpty(CookpotGUID))
                {
                    if (!string.IsNullOrEmpty(s_LastPickedGearGUID))
                    {
                        GameObject StillExistGear = PdidTable.GetGameObject(s_LastPickedGearGUID);
                        if (StillExistGear)
                        {
                            Comps.GearCookingVisual CookingVisual = StillExistGear.GetComponent<Comps.GearCookingVisual>();
                            if (CookingVisual && CookingVisual.m_IsCookPot)
                            {
                                CookpotGUID = s_LastPickedGearGUID;

                                if (gear.m_Cookable)
                                {
                                    if (gear.m_FoodItem)
                                    {
                                        Volume = gear.m_FoodItem.m_CaloriesRemaining;
                                    }
                                    if (gear.m_Cookable.m_CookedPrefab)
                                    {
                                        CookingResult = gear.m_Cookable.m_CookedPrefab.name;
                                    }
                                    else
                                    {
                                        CookingResult = "Warming";
                                    }
                                }
                                BeingCookedTime = s_LastGearTimeBeingCooked;
                            }
                        }
                        s_LastPickedGearGUID = string.Empty;
                    }
                }

                if (gear.m_InProgressCraftItem)
                {
                    if (gear.m_InProgressCraftItem.m_PercentComplete > 0)
                    {
                        float Scaler = gear.m_InProgressCraftItem.m_PercentComplete / 100f;

                        if (gear.m_Cookable)
                        {
                            float TimeToCook = gear.m_Cookable.m_CookTimeMinutes / 60f;

                            BeingCookedTime = TimeToCook * Scaler;
                        }
                    }
                }

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
                        v3 = gear.gameObject.transform.position;
                        rot = gear.gameObject.transform.rotation;
                    }
                }

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

                if (ModMain.Client.m_IsReady && ModMain.Client.m_Rules.m_CanDropItems)
                {
                    ClientSend.SendGear(gear.name, v3, rot, JSON, NormalizedCondition, Style, FireGUID, CookingSlotIndex, CookingResult, Volume, BeingCookedTime, CookpotGUID);
                }

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

        public static void HandleGearCooking(string GUID, float Cooking)
        {
            //SkyCoop.Logger.Log(ConsoleColor.Green, $"HandleGearCooking {GUID} Progress {Cooking}");
            GameObject GearObject = PdidTable.GetGameObject(GUID);
            if (GearObject)
            {
                Comps.GearCookingVisual CookingVisual = GearObject.GetComponent<Comps.GearCookingVisual>();
                if (CookingVisual)
                {
                    CookingVisual.m_BeingCookedTime = Cooking;
                }
            }
        }

        public static void HandleGearDropped(DataStr.GearDataVisual Data)
        {
            GameObject GearObject = PdidTable.GetGameObject(Data.m_GUID);

            if(GearObject == null)
            {
                string LocalizedGearName = "InvalidGearName";
                bool IsCookpotItem = false;
                bool IsCookable = false;
                GearObject = AssetManager.CreateLocalizedBogusGear(Data.m_GearName, out LocalizedGearName, Data.m_Volume, Data.m_ConditionNormalized, Data.m_Style);
                if (GearObject != null)
                {
                    //SkyCoop.Logger.Log(ConsoleColor.Green, $"Bogus created!");
                    GearObject.transform.position = Data.m_Position.ConvertToUnity();
                    GearObject.transform.rotation = Data.m_Rotation.ConvertToUnity();
                    GearObject.name = Data.m_GearName;
                    Utils.SetObjectAndChildrenLayer(GearObject, vp_Layer.Gear, vp_Layer.Gear);
                    ObjectGuid GUIDObj = GearObject.GetComponent<ObjectGuid>();
                    if (GUIDObj == null)
                    {
                        GUIDObj = GearObject.AddComponent<ObjectGuid>();
                    }
                    Comps.DroppedGearVisual GearComp = GearObject.AddComponent<Comps.DroppedGearVisual>();
                    GearComp.m_PrefabName = Data.m_GearName;
                    GearComp.m_GUID = Data.m_GUID;
                    GearComp.m_LocalizedName = LocalizedGearName;
                    GearComp.m_Style = Data.m_Style;

                    GearComp.m_CookingVisual = GearObject.GetComponent<Comps.GearCookingVisual>();
                    if (GearComp.m_CookingVisual)
                    {
                        GearComp.m_CookingVisual.m_Gear = GearComp;
                        GearComp.m_CookingVisual.m_FireGUID = Data.m_FireGUID;
                        GearComp.m_CookingVisual.m_CookingSlotIndex = Data.m_CookingSlot;
                        GearComp.m_CookingVisual.RelinkCookingSlot();
                        GearComp.m_CookingVisual.m_BeingCookedTime = Data.m_BeingCookedTime;
                        GearComp.m_CookingVisual.m_CookingResult = Data.m_CookingResult;
                        GearComp.m_CookingVisual.m_Volume = Data.m_Volume;

                        GearComp.m_CookingVisual.SetupGrubMesh(GearComp.m_CookingVisual.GetState());
                    }

                    GearsSync.ApplyTextureDoner(GearObject);

                    PdidTable.RuntimeRegister(GUIDObj, Data.m_GUID);
                }
            }
            else
            {                
                Comps.DroppedGearVisual GearComp = GearObject.GetComponent<Comps.DroppedGearVisual>();

                if(GearComp.gameObject.name != Data.m_GearName)
                {
                    PdidTable.RuntimeUnregister(Data.m_GUID);
                    UnityEngine.Object.Destroy(GearComp.gameObject);
                    HandleGearDropped(Data);
                    return;
                }

                GearComp.m_Style = Data.m_Style;

                if (GearComp.m_CookingVisual)
                {
                    GearComp.m_CookingVisual.m_Gear = GearComp;
                    GearComp.m_CookingVisual.m_FireGUID = Data.m_FireGUID;
                    GearComp.m_CookingVisual.m_CookingSlotIndex = Data.m_CookingSlot;
                    GearComp.m_CookingVisual.RelinkCookingSlot();
                    GearComp.m_CookingVisual.m_BeingCookedTime = Data.m_BeingCookedTime;
                    GearComp.m_CookingVisual.m_CookingResult = Data.m_CookingResult;
                    GearComp.m_CookingVisual.m_Volume = Data.m_Volume;

                    GearComp.m_CookingVisual.SetupGrubMesh(GearComp.m_CookingVisual.GetState());
                }
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

        public enum CookedState
        {
            Raw,
            Cooked,
            Overcooked,
        }

        public static float CalculateWaterVolume_Debug(float TimeBeingCooked, float Volume)
        {
            float FinalVolume = Volume;
            float MinutesToMeltSnowPerLiter = 37.5f;
            float TimeToCook = (MinutesToMeltSnowPerLiter * Volume) / 60f;

            if (TimeBeingCooked <= TimeToCook)
            {
                FinalVolume = (TimeBeingCooked * 60f) / MinutesToMeltSnowPerLiter;
            }
            return FinalVolume;
        }

        public static void GetCookngTime(GearItem Gear, float Calories, out float CookingTime, out float BurningTime)
        {
            CookingTime = 0;
            BurningTime = 0;
            if (Gear)
            {
                GetCookngTime(Gear.m_Cookable, Gear.m_FoodItem, Gear.m_FoodWeight, Calories, out CookingTime, out BurningTime);
            }
        }

        public static void GetCookngTime(Cookable Cookable, FoodItem FoodItem, FoodWeight FoodWeight, float Calories, out float CookingTime, out float BurningTime)
        {
            CookingTime = 0;
            BurningTime = 0;
            if (Cookable)
            {
                if (FoodItem && Calories > 0)
                {
                    float Val = FoodItem.m_CaloriesTotal;
                    if (FoodWeight)
                    {
                        Val = FoodWeight.m_CaloriesPerKG * FireHook.ConvertWeightVolume(FoodWeight.m_MaxWeight);
                    }
                    CookingTime = (Calories / Val * Cookable.m_CookTimeMinutes) / 60f;
                }
                else
                {
                    CookingTime = Cookable.m_CookTimeMinutes / 60f;
                }
                BurningTime = Cookable.m_ReadyTimeMinutes / 60f;
            }
        }

        public static void GetCookngTime(string GearName, float Calories, out float CookingTime, out float BurningTime)
        {
            CookingTime = 0;
            BurningTime = 0;
            GameObject reference = AssetManager.GetAssetFromGame<GameObject>(GearName);

            if (reference)
            {
                GetCookngTime(reference.GetComponent<GearItem>(), Calories, out CookingTime, out BurningTime);
            }
        }

        public static float GetNormalizedCookingProgress(float CookingTime, float TimeBeingCooked)
        {
            float NormalizedCookngProgress = TimeBeingCooked / CookingTime;

            return NormalizedCookngProgress;
        }

        public static float GetNormalizedCookingProgress(GearItem Gear, float TimeBeingCooked, float Caloreis = 1)
        {
            if (Gear)
            {
                if (Gear.m_Cookable)
                {
                    float CookingTime = 0;
                    float BurningTime = 0;
                    GetCookngTime(Gear, Caloreis, out CookingTime, out BurningTime);

                    return GetNormalizedCookingProgress(CookingTime, TimeBeingCooked);
                }
            }

            return 0f;
        }

        public static CookedState GetCookingState(GearItem Gear, float TimeBeingCooked, float Caloreis)
        {
            if (Gear)
            {
                float CookingTime = 0;
                float BurningTime = 0;
                GetCookngTime(Gear, Caloreis, out CookingTime, out BurningTime);

                float NormalizedProgress = GetNormalizedCookingProgress(CookingTime, TimeBeingCooked);

                if (NormalizedProgress < 1)
                {
                    return CookedState.Raw;
                }
                else if (NormalizedProgress > 1)
                {
                    float Overcooked = TimeBeingCooked - CookingTime;

                    if (Overcooked / BurningTime < 1)
                    {
                        return CookedState.Cooked;
                    }
                    else
                    {
                        return CookedState.Overcooked;
                    }
                }
            }
            
            return CookedState.Raw;
        }

        public static CookedState GetCookingState(string GearName, float TimeBeingCooked, float Caloreis)
        {
            GameObject reference = AssetManager.GetAssetFromGame<GameObject>(GearName);

            if (reference)
            {
                return GetCookingState(reference.GetComponent<GearItem>(), TimeBeingCooked, Caloreis);
            }
            return CookedState.Raw;
        }

        public static void ProcessGearPickUpQueue(GearPickedElement Data)
        {
            CanclePickingUp();
            //SkyCoop.Logger.Log(ConsoleColor.Green, $"HandleGearPickUp {GearName}");

            bool Explosive = false;

            if (Data.m_GearName.EndsWith("_Boom"))
            {
                Data.m_GearName = Data.m_GearName.Replace("_Boom", "");
                Explosive = true;
            }

            SkyCoop.Logger.Log(ConsoleColor.Green, $"ProcessGearPickUpQueuer {Data.m_GearName} CookingResult {Data.m_CookingResult} TimeBeingCooked {Data.m_TimeBeingCooked} Volume {Data.m_Volume}");

            GameObject reference = AssetManager.GetAssetFromGame<GameObject>(Data.m_GearName);
            if (reference)
            {
                GameObject GearObject = UnityEngine.Object.Instantiate(reference, s_LastPickedGearPosition, s_LastPickedGearRotation);

                GearObject.name = Data.m_GearName;
                //SkyCoop.Logger.Log(ConsoleColor.Green, "Going to deserialize...");

                GearItemSaveDataProxy DataProxy = Utils.DeserializeObject<GearItemSaveDataProxy>(Data.m_JSON);

                GearItem Gi = GearObject.GetComponent<GearItem>();

                bool SkipWIPOverride = false;

                if(Data.m_GearName.StartsWith("GEAR_Uncooked") && Data.m_TimeBeingCooked == 0)
                {
                    SkipWIPOverride = true; // пусть грузит из JSON данных

                    if (Gi.m_InProgressCraftItem == null)
                    {
                        Gi.m_InProgressCraftItem = GearObject.AddComponent<InProgressCraftItem>();
                    }
                }

                //SkyCoop.Logger.Log(ConsoleColor.Green, "JSON " + JSON);
                Gi.Deserialize(DataProxy, true);

                if (Gi.m_Bed)
                {
                    Gi.m_Bed.SetState(BedRollState.Rolled);
                }

                if (Data.m_GearName.StartsWith("GEAR_Uncooked") && !SkipWIPOverride)
                {
                    if (Gi.m_InProgressCraftItem == null)
                    {
                        Gi.m_InProgressCraftItem = GearObject.AddComponent<InProgressCraftItem>();
                    }

                    if (Gi.m_InProgressCraftItem)
                    {
                        if (Gi)
                        {
                            if (Gi.m_Cookable)
                            {
                                float NormalizedCookingProgress = GetNormalizedCookingProgress(Gi, Data.m_TimeBeingCooked);

                                if (NormalizedCookingProgress >= 1)
                                {
                                    Gi.m_InProgressCraftItem.m_PercentComplete = 100f;
                                }
                                else
                                {
                                    Gi.m_InProgressCraftItem.m_PercentComplete = NormalizedCookingProgress * 100f;
                                }
                            }
                        }
                    }
                }

            Post_Deserialize:

                if (Data.m_DropAround)
                {
                    Gi.StickToGroundAtPlayerFeet(GameManager.GetPlayerTransform().position);
                }
                else
                {
                    Gi.transform.position = s_LastPickedGearPosition;
                    Gi.transform.rotation = s_LastPickedGearRotation;
                }

                if (Explosive)
                {
                    if (Gi.m_NoiseMakerItem)
                    {
                        Gi.m_NoiseMakerItem.Ignite();
                        Gi.SetNormalizedHP(0.3f);
                    }
                }
                if (Data.m_SpawnLoaded)
                {
                    if (Gi.m_GunItem)
                    {
                        Gi.m_GunItem.FillClipAtCondition(100);
                    }
                }

                GearManualPatch(Gi);
                //SkyCoop.Logger.Log(ConsoleColor.Green, "Gear deserialized!");

                bool Ruined = false;

                if (Gi.m_CookingPotItem)
                {
                    Gi.m_CookingPotItem.m_CookingState = CookingPotItem.CookingState.Cooking;
                    Gi.m_CookingPotItem.m_PercentCooked = 0;
                    Gi.m_CookingPotItem.m_LitersSnowBeingMelted = new ItemLiquidVolume(0);
                    Gi.m_CookingPotItem.m_LitersWaterBeingBoiled = new ItemLiquidVolume(0);
                    if (Data.m_CookingResult == "BadWater")
                    {
                        float TimeToCook = (Gi.m_CookingPotItem.m_CookSettings.m_MinutesToMeltSnowPerLiter * Data.m_Volume) / 60f;
                        float TimeBeingCooked = Data.m_TimeBeingCooked;

                        if (TimeBeingCooked >= TimeToCook)
                        {
                            TimeBeingCooked = TimeToCook;
                            Gi.m_CookingPotItem.m_CookingState = CookingPotItem.CookingState.Ready;
                        }
                        else
                        {
                            Gi.m_CookingPotItem.m_CookingState = CookingPotItem.CookingState.Cooking;
                        }

                        Gi.m_CookingPotItem.m_PercentCooked = TimeBeingCooked / TimeToCook;
                        Gi.m_CookingPotItem.m_LitersSnowBeingMelted = new ItemLiquidVolume(FireHook.ConvertVolumeToUnits(Data.m_Volume));
                        Gi.m_CookingPotItem.SetUpWaterMesh();
                    }
                    else if(Data.m_CookingResult == "GoodWater")
                    {
                        float TimeToCook = (Gi.m_CookingPotItem.m_CookSettings.m_MinutesToBoilWaterPerLiter * Data.m_Volume) / 60f;
                        float TimeBeingCooked = Data.m_TimeBeingCooked;

                        if (TimeBeingCooked < TimeToCook * 2) // Ещё не выкипело к чертям собачим
                        {
                            if (TimeBeingCooked < TimeToCook) // Недокепитил
                            {
                                Gi.m_CookingPotItem.m_CookingState = CookingPotItem.CookingState.Cooking;
                                Gi.m_CookingPotItem.m_MinutesUntilCooked = (TimeToCook - TimeBeingCooked) / 60;
                            }
                            else
                            {
                                
                                Gi.m_CookingPotItem.m_CookingState = CookingPotItem.CookingState.Ready;
                                Gi.m_CookingPotItem.m_MinutesUntilCooked = 0;
                            }
                            Gi.m_CookingPotItem.m_PercentCooked = 1;
                            Gi.m_CookingPotItem.m_LitersWaterBeingBoiled = new ItemLiquidVolume(FireHook.ConvertVolumeToUnits(Data.m_Volume));
                            Gi.m_CookingPotItem.SetUpWaterMesh();
                            if (Gi.m_CookingPotItem.m_CookingState == CookingPotItem.CookingState.Ready)
                            {
                                Gi.m_CookingPotItem.m_GrubMeshRenderer.sharedMaterials = Gi.m_CookingPotItem.m_BoilWaterReadyMaterialsList;
                            }
                        }
                    }
                }
                else
                {
                    if(Data.m_CookingResult == "Warming")
                    {
                        if (Gi.m_FoodItem)
                        {
                            CookedState CookingState = GetCookingState(Gi, Data.m_TimeBeingCooked, Gi.m_FoodItem.m_CaloriesRemaining);
                            if (CookingState == CookedState.Cooked)
                            {
                                Gi.m_FoodItem.m_HeatPercent = 100;
                                if (Gi.m_FoodItem.m_NumTimesHeatedUp == 0)
                                {
                                    GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Cooking, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                                }
                                Gi.m_FoodItem.m_NumTimesHeatedUp++;
                            }else if(CookingState == CookedState.Overcooked)
                            {
                                Ruined = true;
                            }
                        }
                    }
                    else
                    {
                        if (Gi.m_Cookable && Gi.m_Cookable.m_CookedPrefab)
                        {
                            float Calories = 0;

                            if (Gi.m_FoodItem)
                            {
                                Calories = Gi.m_FoodItem.m_CaloriesRemaining;
                            }
                            else
                            {
                                Calories = 0;
                            }
                            CookedState CookingState = GetCookingState(Gi, Data.m_TimeBeingCooked, Calories);
                            if (CookingState == CookedState.Cooked)
                            {
                                GameObject DummyCookPot = UnityEngine.Object.Instantiate(AssetManager.GetAssetFromGame<GameObject>("GEAR_CookingPot"), s_LastPickedGearPosition, s_LastPickedGearRotation);
                                if (DummyCookPot)
                                {
                                    CookingPotItem Pot = DummyCookPot.GetComponent<CookingPotItem>();

                                    if (Pot)
                                    {
                                        GameObject CookedReference = AssetManager.GetAssetFromGame<GameObject>(Data.m_CookingResult);

                                        if (CookedReference)
                                        {
                                            GameObject CookedInstance = UnityEngine.Object.Instantiate(CookedReference, s_LastPickedGearPosition, s_LastPickedGearRotation);

                                            if (CookedInstance)
                                            {
                                                GearItem CookedGear = CookedInstance.GetComponent<GearItem>();

                                                CookedGear.m_InitialDecayApplied = true;
                                                Pot.SetCookedGearProperties(Gi, CookedGear);

                                                UnityEngine.Object.Destroy(Gi.gameObject);
                                                UnityEngine.Object.Destroy(DummyCookPot);
                                                Gi = CookedGear;
                                                GearObject = Gi.gameObject;
                                                GearObject.name = Data.m_CookingResult;
                                                GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Cooking, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);

                                                // TODO add oil
                                                goto Post_Deserialize;
                                            }
                                        }
                                    }
                                    UnityEngine.Object.Destroy(DummyCookPot);
                                    return;
                                }
                            }else if(CookingState == CookedState.Overcooked)
                            {
                                Ruined = true;
                            }
                        }
                    }
                }


                if (Ruined)
                {
                    GameAudioManager.Play3DSound("Play_RemoveRuined", GameManager.GetPlayerTransform().gameObject);
                    UnityEngine.Object.Destroy(GearObject);
                    s_LastPickedGearGUID = string.Empty;
                    FireHook.FinishCookingAction();
                }
                else
                {
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
        }

        public static void HandleGearPickUp(GearPickedElement Data)
        {
            s_GearQueue.Add(Data);
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

        public static void TryPickUp(Comps.DroppedGearVisual Visual, bool PlaceMode = false, bool IgnoreActionPicker = false)
        {
            if (Visual)
            {
                if (ModMain.Client != null && ModMain.Client.m_IsReady)
                {
                    if (ModMain.Client.m_Rules.m_CanUseBeds)
                    {
                        if (!IgnoreActionPicker && Visual.m_Bed && Visual.m_Bed.m_BedRollState == BedRollState.Placed)
                        {
                            Visual.m_Bed.PerformInteraction();
                            return;
                        }
                    }
                }
                
                if (!IgnoreActionPicker && Visual.m_CookingVisual && Visual.m_CookingVisual.IsCooking() && string.IsNullOrEmpty(Visual.m_CookingVisual.m_CookingResult))
                {
                    Panel_ActionPicker Panel = InterfaceManager.GetPanel<Panel_ActionPicker>();
                    if (Panel)
                    {
                        Panel.Enable(true);
                        Panel.m_ActionPickerItemDataList.Clear();
                        Action PickupDelegate = new Action(() => TryPickUp(Visual, PlaceMode, true));
                        Action CookDelegate = new Action(() => FireHook.HandleCookFromPicker(Visual));
                        Action BoilDeleagte = new Action(() => FireHook.HandleBoilFromPicker(Visual));

                        Panel.m_ActionPickerItemDataList.Add(new ActionPickerItemData("ico_climb", "GAMEPLAY_PickUp", PickupDelegate));
                        Panel.m_ActionPickerItemDataList.Add(new ActionPickerItemData("ico_cooking_pot", "GAMEPLAY_Cook", CookDelegate));
                        Panel.m_ActionPickerItemDataList.Add(new ActionPickerItemData("ico_water_prep", "GAMEPLAY_Water", BoilDeleagte));

                        Panel.m_ObjectInteractedWith = null;
                        Panel.EnableWithCurrentList();
                    }
                }
                else
                {
                    Panel_HUD Panel;
                    if (InterfaceManager.TryGetPanel<Panel_HUD>(out Panel))
                    {
                        s_ControlModeBeforePickingUp = GameManager.GetPlayerManagerComponent().m_ControlMode;
                        GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);
                        Panel.StartItemProgressBar(10, "Picking Up...", null, new System.Action(PickUpFailedSilent));
                    }
                    s_PlaceModeAfterPickup = PlaceMode;
                    s_LastPickedGearPosition = Visual.transform.position;
                    s_LastPickedGearRotation = Visual.transform.rotation;

                    if (Visual.m_CookingVisual)
                    {
                        s_LastCookingSlotGearPickedFrom = Visual.m_CookingVisual.m_CookingSlot;
                        s_LastGearTimeBeingCooked = Visual.m_CookingVisual.m_BeingCookedTime;
                    }
                    else
                    {
                        s_LastCookingSlotGearPickedFrom = null;
                        s_LastGearTimeBeingCooked = 0;
                    }
                    s_LastPickedGearGUID = Visual.m_GUID;
                    ClientSend.SendGearPickUp(Visual.m_GUID);
                }
            }
        }
    }
}
