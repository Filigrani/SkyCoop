using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using UnhollowerRuntimeLib;
using UnhollowerBaseLib;
using MelonLoader.TinyJSON;
using System.Diagnostics;
using GameServer;
using static SkyCoop.DataStr;
using static SkyCoop.Comps;
using static Utils;
using static SkyCoop.MyMod;
using AK;

namespace SkyCoop
{
    public class Pathes
    {
        public static void SendTCPData(Packet _packet)
        {
            MyMod.SendUDPData(_packet);
        }

        [HarmonyLib.HarmonyPatch(typeof(GearItem), "Drop")] // Once
        public class GearItemDrop
        {
            private static int ShouldDrop = 0;
            private static int Had = 0;
            public static bool Prefix(GearItem __instance, int numUnits)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_GearName == "GEAR_SCHeatPack" || __instance.m_GearName == "GEAR_SCHeatPackC" || __instance.m_GearName == "GEAR_SCEmergencyRation" || __instance.m_GearName == "GEAR_SCLiquidLead")
                {
                    GameAudioManager.PlayGUIError();
                    HUDMessage.AddMessage("YOU CAN'T DROP THIS", true, true);
                    return false;
                }
                MelonLogger.Msg("Item dropped " + __instance.m_GearName);
                if (MyMod.InOnline() == true && __instance.gameObject.GetComponent<Comps.IgnoreDropOverride>() == null)
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
                }
                return true;
            }
            public static void Postfix(GearItem __instance, int numUnits, GearItem __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_GearName == "GEAR_SCHeatPack" || __instance.m_GearName == "GEAR_SCHeatPackC" || __instance.m_GearName == "GEAR_SCEmergencyRation" || __instance.m_GearName == "GEAR_SCLiquidLead")
                {
                    return;
                }
                if (MyMod.InOnline() == true && __instance.gameObject.GetComponent<Comps.IgnoreDropOverride>() == null)
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
                        MyMod.SendDropItem(__instance, ShouldDrop, Had, false, variant);
                    }
                    else
                    {
                        MyMod.SendDropItem(__instance, 0, 0, false, variant);
                    }

                    if (__result != null && __result.gameObject != null && __result != __instance)
                    {
                        UnityEngine.Object.Destroy(__result.gameObject);
                    }
                }
                else
                {
                    if (MyMod.InOnline() == true && __instance.gameObject.GetComponent<Comps.IgnoreDropOverride>() != null)
                    {
                        MelonLogger.Msg("[Postfix]Gear has IgnoreDropOverride doing drop without sync");
                        UnityEngine.Object.Destroy(__instance.gameObject.GetComponent<Comps.IgnoreDropOverride>());
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_GearSelect), "DoFirePickerAction")] // Once
        private static class Panel_GearSelect_DoFirePickerAction
        {
            internal static void Prefix(Panel_GearSelect __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                GearItem selectedItem = __instance.GetSelectedItem();
                if (selectedItem != null)
                {
                    MelonLogger.Msg("Gear going to be placed adding IgnoreDropOverride");
                    selectedItem.gameObject.AddComponent<Comps.IgnoreDropOverride>();
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(CookingPotItem), "StartCooking")] // Once
        private static class CookingPotItem_StartCooking
        {
            internal static void Prefix(CookingPotItem __instance, GearItem gearItemToCook)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (gearItemToCook != null)
                {
                    MelonLogger.Msg("Going to cook " + gearItemToCook.m_GearName + " IgnoreDropOverride");
                    gearItemToCook.gameObject.AddComponent<Comps.IgnoreDropOverride>();
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "RestoreTransform")] // Once
        private static class PlayerManager_RestoreTransform
        {
            private static GameObject saveObj;
            internal static void Prefix(PlayerManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == true)
                {
                    MelonLogger.Msg("Going to cancle placement");
                    if (__instance.m_ObjectToPlace != null)
                    {
                        if (__instance.m_ObjectToPlace.GetComponent<Comps.DropFakeOnLeave>() != null)
                        {
                            saveObj = __instance.m_ObjectToPlace;
                        }
                    }
                }
            }
            internal static void Postfix(PlayerManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == true)
                {
                    if (saveObj != null)
                    {
                        if (saveObj.GetComponent<ObjectGuid>())
                        {
                            foreach (var item in ExpeditionEditor.m_Objects)
                            {
                                if (item.m_GUID == saveObj.GetComponent<ObjectGuid>().Get())
                                {
                                    ExpeditionEditor.ToggleColisionOfObject(saveObj, true);
                                    MelonLogger.Msg("Expedition editor object placed!");
                                    return;
                                }
                            }
                        }

                        MelonLogger.Msg("Return gear to fake one");
                        int variant = 0;
                        if (saveObj.GetComponent<Bed>() != null && saveObj.GetComponent<Bed>().m_BedRollState == BedRollState.Placed)
                        {
                            variant = 1;
                        }
                        if ((saveObj.gameObject.GetComponent<FoodItem>() != null && saveObj.gameObject.GetComponent<FoodItem>().m_Opened == true) ||
                            (saveObj.gameObject.GetComponent<SmashableItem>() != null && saveObj.gameObject.GetComponent<SmashableItem>().m_HasBeenSmashed == true))
                        {
                            variant = 1;
                        }
                        if (saveObj.gameObject.GetComponent<KeroseneLampItem>() != null && saveObj.gameObject.GetComponent<KeroseneLampItem>().m_On)
                        {
                            variant = 1;
                        }
                        MyMod.SendDropItem(saveObj.gameObject.GetComponent<GearItem>(), 0, 0, true, variant);
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
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == true)
                {
                    if (__instance.m_ObjectToPlace != null)
                    {
                        MelonLogger.Msg("Going to place mesh!");
                        saveObj = __instance.m_ObjectToPlace;
                    }
                }
            }
            internal static void Postfix(PlayerManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == true)
                {
                    if (saveObj != null)
                    {
                        if (saveObj.GetComponent<ObjectGuid>())
                        {
                            foreach (var item in ExpeditionEditor.m_Objects)
                            {
                                if(item.m_GUID == saveObj.GetComponent<ObjectGuid>().Get())
                                {
                                    item.m_Position = saveObj.transform.position;
                                    item.m_Rotation = saveObj.transform.rotation;
                                    ExpeditionEditor.ToggleColisionOfObject(saveObj, true);

                                    MelonLogger.Msg("Expedition editor object placed!");
                                    return;
                                }
                            }
                        }


                        MelonLogger.Msg("Gear placed!");
                        int variant = 0;
                        if (saveObj.GetComponent<Bed>() != null && saveObj.GetComponent<Bed>().m_BedRollState == BedRollState.Placed)
                        {
                            variant = 1;
                        }
                        if ((saveObj.gameObject.GetComponent<FoodItem>() != null && saveObj.gameObject.GetComponent<FoodItem>().m_Opened == true) ||
                            (saveObj.gameObject.GetComponent<SmashableItem>() != null && saveObj.gameObject.GetComponent<SmashableItem>().m_HasBeenSmashed == true))
                        {
                            variant = 1;
                        }
                        if(saveObj.gameObject.GetComponent<KeroseneLampItem>() != null && saveObj.gameObject.GetComponent<KeroseneLampItem>().m_On)
                        {
                            variant = 1;
                        }

                        Comps.DropFakeOnLeave DFL = saveObj.GetComponent<Comps.DropFakeOnLeave>();
                        if (DFL != null)
                        {
                            DFL.m_OldPossition = saveObj.transform.position;
                            DFL.m_OldRotation = saveObj.transform.rotation;
                            MyMod.SendDropItem(saveObj.gameObject.GetComponent<GearItem>(), 0, 0, true, variant);
                        }
                        else
                        {
                            GearItem gi = saveObj.gameObject.GetComponent<GearItem>();
                            if(gi == null)
                            {
                                MelonLogger.Msg(saveObj.gameObject.name + " has not GearItem");
                                return;
                            }
                            if (gi.m_Cookable == null || gi.m_Cookable.IsNearFire() == false)
                            {
                                DFL = saveObj.AddComponent<Comps.DropFakeOnLeave>();
                                DFL.m_OldPossition = saveObj.transform.position;
                                DFL.m_OldRotation = saveObj.transform.rotation;
                                MyMod.SendDropItem(saveObj.gameObject.GetComponent<GearItem>(), 0, 0, true, variant);
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InteractiveObjectsProcessAltFire")] // Once
        private static class PlayerManager_InteractiveObjectsProcessAltFire
        {
            internal static void Prefix(PlayerManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                if (__instance.m_InteractiveObjectUnderCrosshair)
                {
                    if (__instance.m_InteractiveObjectUnderCrosshair.GetComponent<LoadScene>())
                    {
                        LoadScene Door = __instance.m_InteractiveObjectUnderCrosshair.GetComponent<LoadScene>();

                        if (GameManager.GetWeatherComponent().IsIndoorEnvironment())
                        {
                            if (MyMod.iAmHost)
                            {
                                MyMod.ShowKnockersPicker(Door.gameObject, MPSaveManager.GetKnocksOnScene(MyMod.level_guid));
                            }
                            if (MyMod.sendMyPosition)
                            {
                                MyMod.TempDoor = Door.gameObject;
                                MyMod.DoPleaseWait("Looking through the peephole", "Please wait...");
                                SetRepeatPacket(ResendPacketType.Cancel, 10);
                                using (Packet _packet = new Packet((int)ClientPackets.PEEPHOLE))
                                {
                                    _packet.Write(MyMod.level_guid);
                                    SendTCPData(_packet);
                                }
                            }
                            return;
                        }


                        if (Door.m_SceneCanBeInstanced && !string.IsNullOrEmpty(Door.m_GUID) && Door.m_Active)
                        {
                            if (Door.GetComponent<Comps.DoorLockedOnKey>() == null)
                            {
                                MyMod.SelectKeys(__instance.m_InteractiveObjectUnderCrosshair, MyMod.KeysAction.LOCK);
                            }
                        } else
                        {
                            HUDMessage.AddMessage("This door can't be locked!");
                        }
                    }else if(MyMod.ExpeditionEditorUI != null && MyMod.ExpeditionEditorUI.activeSelf)
                    {
                        if (__instance.m_InteractiveObjectUnderCrosshair.GetComponent<Container>())
                        {
                            ExpeditionEditor.AddContainer(__instance.m_InteractiveObjectUnderCrosshair.GetComponent<ObjectGuid>().Get());
                        } else if(__instance.m_InteractiveObjectUnderCrosshair.GetComponent<Harvestable>())
                        {
                            ExpeditionEditor.AddPlant(__instance.m_InteractiveObjectUnderCrosshair.GetComponent<ObjectGuid>().Get());
                        } else if (__instance.m_InteractiveObjectUnderCrosshair.GetComponent<BreakDown>())
                        {
                            ExpeditionEditor.AddBreakdown(__instance.m_InteractiveObjectUnderCrosshair.GetComponent<ObjectGuid>().Get());
                        }
                    } else{
                        MyMod.PlaceDroppedGear(__instance.m_InteractiveObjectUnderCrosshair);
                        MelonLogger.Msg("Going to place object");
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "Update")] // When open
        private static class Panel_Rest_Update
        {
            internal static void Postfix(Panel_Rest __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MyMod.MyCycleSkip = __instance.m_SleepHours;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "OnCancel")] // Once
        private static class Panel_Rest_Close
        {
            internal static void Postfix(Panel_Rest __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("Sleeping menu close.");
                if (MyMod.SleepingButtons != null)
                {
                    MyMod.SleepingButtons.SetActive(true);
                }
                if (MyMod.WaitForSleepLable != null)
                {
                    MyMod.WaitForSleepLable.SetActive(false);
                }
                if (__instance.m_Bed != null && __instance.m_Bed.gameObject != null && __instance.m_Bed.gameObject.GetComponent<Comps.FakeBedDummy>() != null)
                {
                    UnityEngine.Object.Destroy(__instance.m_Bed.gameObject);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "OnPickUp")] // Once
        private static class Panel_Rest_Close2
        {
            internal static bool Prefix(Panel_Rest __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_Bed != null && __instance.m_Bed.gameObject != null && __instance.m_Bed.gameObject.GetComponent< Comps.FakeBedDummy >() != null)
                {
                    if (__instance.m_Bed.gameObject.GetComponent<Comps.FakeBedDummy >().m_LinkedFakeObject != null)
                    {
                        MyMod.PickupDroppedGear(__instance.m_Bed.gameObject.GetComponent< Comps.FakeBedDummy >().m_LinkedFakeObject);
                    }
                    UnityEngine.Object.Destroy(__instance.m_Bed.gameObject);
                    __instance.m_Bed = null;
                    __instance.Enable(false);
                    return false;
                }
                return true;
            }
            internal static void Postfix(Panel_Rest __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("Sleeping menu close.");
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "OnRest")] // Once
        private static class Panel_Rest_Close3
        {
            public static Bed saveObj;
            internal static void Prefix(Panel_Rest __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                saveObj = __instance.m_Bed;
            }
            internal static void Postfix(Panel_Rest __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("Sleeping menu close.");
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PassTime), "End")] // Once
        private static class Panel_Rest_Close4
        {
            public static Bed saveObj;
            internal static void Prefix(PassTime __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                saveObj = __instance.m_Bed;
            }
            internal static void Postfix(PassTime __instance)
            {
                if (saveObj != null && saveObj.gameObject.GetComponent<Comps.FakeBedDummy>() != null)
                {
                    if (MyMod.CrazyPatchesLogger == true)
                    {
                        StackTrace st = new StackTrace(new StackFrame(true));
                        MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                        MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                    }
                    UnityEngine.Object.Destroy(saveObj.gameObject);
                    MelonLogger.Msg("Dummy bed removed");
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Confirmation), "OnCancel")] // Once
        private static class Panel_Confirmation_OnCancel
        {
            internal static void Postfix(Panel_Confirmation __instance)
            {
                if (CharcoalItem.m_CharcoalItemInUseForSurvey != null)
                {
                    CharcoalItem.m_CharcoalItemInUseForSurvey.StopSurveyAudio();
                    GameManager.GetPlayerManagerComponent().UnequipItemInHands();
                    CharcoalItem.m_CharcoalItemInUseForSurvey = null;
                }
                if (Shared.CloseContainerOnCancle)
                {
                    Shared.CloseContainerOnCancle = false;
                    DataStr.ContainerOpenSync pendingContainer = new DataStr.ContainerOpenSync();
                    pendingContainer.m_Guid = "NULL";
                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.CONTAINERINTERACT))
                        {
                            _packet.Write(pendingContainer);
                            SendTCPData(_packet);
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Confirmation), "OnConfirm")] // Once
        private static class Panel_Confirmation_Ok
        {
            internal static void Postfix(Panel_Confirmation __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INPUT SERVER ADDRESS")
                {
                    string text = __instance.m_CurrentGroup.m_InputField.GetText();
                    ClientUser.DoConnectToIp(text);
                }
                if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INVALID CONTAINER DATA")
                {
                    Shared.CloseContainerOnCancle = false;
                    if (MyMod.GoingToOpenContinaer != null)
                    {
                        MyMod.OpenFakeContainer(MyMod.GoingToOpenContinaer);
                    }
                }
                if (__instance.m_CurrentGroup != null && 
                    (__instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "YOU GOT A NEW FLAIR!" ||
                     __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "YOU GOT NEW FLAIRS!"))
                {
                    // Open Flairs Menu
                    Supporters.SetFlairsUpdateData();
                    MyMod.NotificationString = "";
                    MyMod.m_Panel_Sandbox.Enable(true);
                    MenuChange.ChangeMenuItems("MultiProfileSettings");
                    MenuChange.OpenFlairsMenu();

                }
                if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "HOW DO YOU WANT TO BE CALLED?")
                {
                    string text = __instance.m_CurrentGroup.m_InputField.GetText();
                    if (!MyMod.ValidNickName(text))
                    {
                        if (MyMod.m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
                        {
                            InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, "ERROR", "\n" + "Sorry nickname should not contain non latin symbols", Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null, null);
                        }
                    }else{
                        MyMod.MyChatName = text;
                        MPSaveManager.SaveMyName(text);
                    }
                }
                if (__instance.m_CurrentGroup != null)
                {
                    string text = __instance.m_CurrentGroup.m_InputField.GetText();
                    string Error = "";
                    bool Seed = true;
                    bool RightThing = true;
                    string ErrorAlias = "seed";

                    if (__instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INPUT SECRET SEED FOR KEY")
                    {
                        Seed = true;
                        ErrorAlias = "seed";
                    }
                    else if(__instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INPUT NAME FOR KEY")
                    {
                        Seed = false;
                        ErrorAlias = "name";
                    }else{
                        RightThing = false;
                    }

                    if (RightThing)
                    {
                        if (text.Contains('_'))
                        {
                            Error = "Sorry " + ErrorAlias + " should not contain _ symbol";
                        }
                        else if (!MyMod.ValidNickName(text))
                        {
                            Error = "Sorry " + ErrorAlias + " should not contain non latin symbols";
                        }

                        if (Error != "")
                        {
                            if (MyMod.m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
                            {
                                InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, "ERROR", "\n" + Error, Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null, null);
                            }
                        }else{
                            if (Seed)
                            {
                                MyMod.PendingKeySeed = text;
                                InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input name for key", "", Panel_Confirmation.ButtonLayout.Button_2, "Next", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                            }else{
                                MyMod.PendingKeyName = text;


                                if (MyMod.PendingRegisterKey)
                                {
                                    MyMod.LocksmithWork(MyMod.PendingRegisterKey, 1);
                                }
                            }
                        }
                    }
                }
                if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "DATA FOR DEBUG KEY")
                {
                    string text = __instance.m_CurrentGroup.m_InputField.GetText();

                    if (text.Contains('_'))
                    {
                        string[] texts = text.Split('_');
                        MPSaveManager.AlignKey(GameManager.GetPlayerManagerComponent().AddItemCONSOLE("GEAR_SCDoorKey", 1), texts[0], texts[1]);
                    } else
                    {
                        InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, "ERROR", "\n" + "It should be seed_name format!", Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null, null);
                    }
                }
                if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "NOTE MESSAGE")
                {
                    string text = __instance.m_CurrentGroup.m_InputField.GetText();
                    if (CharcoalItem.m_CharcoalItemInUseForSurvey != null)
                    {
                        CharcoalItem.m_CharcoalItemInUseForSurvey.StopSurveyAudio();
                        GameManager.GetPlayerAnimationComponent().Trigger_Generic_Unstow();
                        CharcoalItem.m_CharcoalItemInUseForSurvey.ConsumeCharcoal();
                        CharcoalItem.m_CharcoalItemInUseForSurvey = null;
                        MyMod.CreateCustomNote(text);
                    }
                }
                if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INPUT GUID TO TELEPORT TO")
                {
                    string text = __instance.m_CurrentGroup.m_InputField.GetText();
                    bool found = false;
                    for (int index = 0; index < BaseAiManager.m_BaseAis.Count; ++index)
                    {
                        GameObject animal = BaseAiManager.m_BaseAis[index].gameObject;

                        if (animal != null && animal.GetComponent<ObjectGuid>() != null)
                        {
                            if (animal.GetComponent<ObjectGuid>().Get() == text)
                            {
                                found = true;
                                GameManager.GetPlayerManagerComponent().TeleportPlayer(animal.transform.position, animal.transform.rotation);
                                break;
                            }
                        }
                    }

                    if (found == false)
                    {
                        HUDMessage.AddMessage("Animal with GUID " + text + " not exist!");
                    }
                }

                if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INPUT ID OF PLAYER TELEPORT TO")
                {
                    string text = __instance.m_CurrentGroup.m_InputField.GetText();

                    int ID = int.Parse(text);

                    if (ID > MyMod.MaxPlayers - 1 || ID < 0)
                    {
                        HUDMessage.AddMessage("Invalid ID of player!");
                    }
                    else
                    {
                        if (MyMod.playersData[ID].m_Levelid != MyMod.levelid)
                        {
                            HUDMessage.AddMessage("You and player on different scenes!");
                        }
                        else
                        {
                            GameManager.GetPlayerManagerComponent().TeleportPlayer(MyMod.playersData[ID].m_Position, MyMod.playersData[ID].m_Rotation);
                            HUDMessage.AddMessage("You has been teleported to " + MyMod.playersData[ID].m_Name);
                        }
                    }
                }
                if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "DISCONNECT TIMEOUT")
                {
                    Application.Quit();
                }
                if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "DISCONNECTED")
                {
                    if (MyMod.QuitWithoutSaving)
                    {
                        Application.Quit();
                    }
                }
                if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INPUT GUID TO TRACK")
                {
                    string text = __instance.m_CurrentGroup.m_InputField.GetText();
                    bool found = false;
                    for (int index = 0; index < BaseAiManager.m_BaseAis.Count; ++index)
                    {
                        GameObject animal = BaseAiManager.m_BaseAis[index].gameObject;

                        if (animal != null && animal.GetComponent<ObjectGuid>() != null)
                        {
                            if (animal.GetComponent<ObjectGuid>().Get() == text)
                            {
                                found = true;
                                MyMod.DebugAnimalGUID = text;
                                MyMod.DebugAnimalGUIDLast = text;
                                MyMod.DebugLastAnimal = animal;
                                HUDMessage.AddMessage("Found animal starting tracking");
                                break;
                            }
                        }
                    }

                    if (found == false)
                    {
                        HUDMessage.AddMessage("Animal with GUID " + text + " not exist!");
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(FirstPersonLightSource), "TurnOnEffects")] // Once
        internal class FirstPersonLightSource_Start
        {
            public static void Prefix(FirstPersonLightSource __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MyMod.MyLightSource = true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(FirstPersonLightSource), "TurnOffEffects")] // Once
        internal class FirstPersonLightSource_End
        {
            public static void Prefix(FirstPersonLightSource __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MyMod.MyLightSource = false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(ObjectAnim), "Play")] // Once
        internal class ObjectAnim_Hack
        {
            public static void Postfix(ObjectAnim __instance, string name)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                //MelonLogger.Msg("ObjectAnim last played anim " + name);
                if (__instance.gameObject != null && __instance.gameObject.GetComponent<Comps.ContainersSync>() != null)
                {
                    if (__instance.gameObject.GetComponent<Comps.ContainersSync>().m_LastAnim != name)
                    {
                        __instance.gameObject.GetComponent<Comps.ContainersSync>().m_LastAnim = name;
                        __instance.gameObject.GetComponent<Comps.ContainersSync>().CallSync();
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BowItem), "ReleaseFire")] // Once
        internal class BowItem_Shoot
        {
            public static void Prefix(BowItem __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (!(bool)(UnityEngine.Object)__instance.m_GearArrow)
                    return;

                if (__instance.m_BowState == BowState.Aim)
                {
                    Transform transform = GameManager.GetPlayerAnimationComponent().m_ArrowFirePropPoint.transform;
                    Transform playerTransform = GameManager.GetPlayerTransform();

                    MelonLogger.Msg("[BowItem] Arrow Fire! " + __instance.m_GearArrow.gameObject.name);
                    DataStr.ShootSync shoot = new DataStr.ShootSync();
                    shoot.m_projectilename = "GEAR_Arrow";
                    shoot.m_position = playerTransform.TransformPoint(transform.position);
                    shoot.m_rotation = playerTransform.rotation * transform.rotation;
                    shoot.m_sceneguid = MyMod.level_guid;

                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.SHOOTSYNC))
                        {
                            _packet.Write(shoot);
                            SendTCPData(_packet);
                        }
                    }
                    if (MyMod.iAmHost == true)
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.SHOOTSYNC))
                        {
                            ServerSend.SHOOTSYNC(0, shoot, true);
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(ArrowItem), "HandleCollisionWithObject")] // Once, but be triggered a lot
        internal class ArrowItem_Shoot
        {
            public static void Postfix(ArrowItem __instance, GameObject collider, Vector3 collisionPoint, Vector3 collisionNormal)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                bool SendDropSync = true;

                if(__instance.m_EmbeddedInAi != null)
                {
                    BaseAi _Ai = __instance.m_EmbeddedInAi;
                    GameObject Animal = _Ai.gameObject;
                    if(Animal.GetComponent<Comps.AnimalUpdates >() != null)
                    {
                        Comps.AnimalUpdates AU = Animal.GetComponent<Comps.AnimalUpdates>();
                        ArrowSaveData asd = __instance.Serialize();
                        DataStr.AnimalArrow Arrow = new DataStr.AnimalArrow();
                        Arrow.m_Position = asd.m_LocalPosition;
                        Arrow.m_Angle = asd.m_LocalAngles;
                        Arrow.m_Depth = asd.m_EmbeddedDepth;
                        Arrow.m_LocaName = asd.m_LocalName;
                        AU.m_Arrows.Add(Arrow);
                        MyMod.AddFakeArrowToAnimal(AU, AU.m_Arrows.Count - 1);
                        SendDropSync = false;
                        UnityEngine.Object.Destroy(__instance.gameObject);
                        return;
                    }
                    else if(Animal.GetComponent<Comps.AnimalActor>() != null)
                    {
                        SendDropSync = false;
                        UnityEngine.Object.Destroy(__instance.gameObject);
                    }
                }
                else
                {
                    ArrowItem.SurfaceResponseInfo surfaceResponseInfo = __instance.GetSurfaceResponseInfo(Utils.GetMaterialTagForObjectAtPosition(collider, collisionPoint), false);
                    if(surfaceResponseInfo.impactEffect == ArrowImpactEffectType.ArrowImpactEffect_Flesh)
                    {
                        Transform t = collider.transform;
                        while (t.gameObject.GetComponent<Comps.AnimalActor>() == null && t.parent != null)
                        {
                            t = t.parent;
                        }
                        if(t.gameObject.GetComponent<Comps.AnimalActor>() != null)
                        {
                            SendDropSync = false;
                            UnityEngine.Object.Destroy(__instance.gameObject);
                        }
                    }
                }

                if (__instance.gameObject.GetComponent<Comps.DestoryArrowOnHit>() != null)
                {
                    UnityEngine.Object.Destroy(__instance.gameObject);
                } else if(SendDropSync == true)
                {
                    //MyMod.SendDropItem(__instance.m_GearItem, 0, 0, true);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(vp_Bullet), "Start")] // Once
        internal class vp_Bullet_Start
        {
            public static bool Prefix(vp_Bullet __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance != null && __instance.gameObject != null)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(MyMod.GetGearItemObject("GEAR_Arrow"), new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                    GearItem componentArrow = gameObject.GetComponent<GearItem>();
                    componentArrow.m_ArrowItem.Fire();
                    UnityEngine.Object.Destroy(gameObject);

                    bool MyBullet = true;
                    int ShooterID = 0;
                    if (__instance.gameObject.GetComponent<Comps.ClientProjectile>() != null)
                    {
                        MyBullet = false;
                        ShooterID = __instance.gameObject.GetComponent<Comps.ClientProjectile>().m_ClientID;
                    }

                    float RandomRotate = 0;
                    float RandomForce = 0;
                    Vector3 RandomTorque = new Vector3(0, 0, 0);

                    float maxAngleDegrees = 0.0f;
                    if (__instance.m_GunType == GunType.Rifle)
                    {
                        double num = (double)StatsManager.IncrementValue(StatID.RifleShot);
                        maxAngleDegrees = GameManager.GetSkillRifle().GetAimAssistAngleDegrees();
                    }
                    else if (__instance.m_GunType == GunType.Revolver)
                    {
                        double num = (double)StatsManager.IncrementValue(StatID.RevolverShot);
                        maxAngleDegrees = GameManager.GetSkillRevolver().GetAimAssistAngleDegrees();
                    }
                    Vector3 position = __instance.transform.position;
                    Vector3 p2 = position + __instance.transform.forward * 100f;
                    int layerMask = Utils.m_WeaponProjectileCollisionLayerMask | 134217728;
                    RaycastHit hit;
                    MelonLogger.Msg("Doing raycast for bullet");
                    if (AiUtils.RaycastWithAimAssist(__instance.transform.position, __instance.transform.forward, out hit, __instance.Range, __instance.MinDistanceForAimAssist, __instance.Accuracy, maxAngleDegrees, layerMask))
                    {
                        p2 = hit.point;
                        Vector3 localScale = __instance.transform.localScale;
                        __instance.transform.parent = hit.transform;
                        __instance.transform.localPosition = hit.transform.InverseTransformPoint(hit.point);
                        __instance.transform.rotation = Quaternion.LookRotation(hit.normal);
                        if (hit.transform.lossyScale == Vector3.one)
                        {
                            RandomRotate = (float)UnityEngine.Random.Range(0, 360);
                            __instance.transform.Rotate(Vector3.forward, RandomRotate, Space.Self);
                        }
                        else
                        {
                            __instance.transform.parent = (Transform)null;
                            __instance.transform.localScale = localScale;
                            __instance.transform.parent = hit.transform;
                        }
                        if ((bool)(UnityEngine.Object)hit.collider)
                            __instance.SpawnImpactEffects(hit);
                        BaseAi baseAiFromObject = AiUtils.GetBaseAiFromObject(hit.collider.gameObject);
                        Comps.AnimalActor ActorFromObject;
                        if (!hit.collider.gameObject)
                        {
                            ActorFromObject = null;
                        }
                        else if(hit.collider.gameObject.layer == 16)
                        {
                            ActorFromObject = hit.collider.gameObject.GetComponent<Comps.AnimalActor>();
                        }else{
                            if(hit.collider.gameObject.layer == 27)
                            {
                                ActorFromObject = hit.collider.gameObject.transform.GetComponentInParent<Comps.AnimalActor>();
                            }else{
                                ActorFromObject = null;
                            }
                        }


                        Comps.PlayerBulletDamage PlayerDamage = hit.collider.gameObject.GetComponent<Comps.PlayerBulletDamage>();
                        MelonLogger.Msg("Trying understand what hit point is");
                        if (baseAiFromObject != null)
                        {
                            MelonLogger.Msg("This is baseAI animal");
                            float num1 = Vector3.Distance(GameManager.GetPlayerTransform().position, hit.collider.transform.position);
                            LocalizedDamage component = hit.collider.GetComponent<LocalizedDamage>();
                            if (__instance.m_GunType == GunType.Rifle)
                            {
                                double num2 = (double)StatsManager.IncrementValue(StatID.SuccessfulHits_Rifle);
                            }
                            else if (__instance.m_GunType == GunType.Revolver)
                            {
                                double num3 = (double)StatsManager.IncrementValue(StatID.SuccessfulHits_Revolver);
                            }
                            BodyDamage.Weapon bodyDamageWeapon = GunTypeMethods.ToBodyDamageWeapon(__instance.m_GunType);
                            float bleedOutMinutes = component.GetBleedOutMinutes(bodyDamageWeapon);
                            float num4 = __instance.Damage * component.GetDamageScale(bodyDamageWeapon);
                            if ((double)num1 < (double)__instance.Accuracy)
                            {
                                if (!baseAiFromObject.m_IgnoreCriticalHits && component.RollChanceToKill(bodyDamageWeapon))
                                    num4 = float.PositiveInfinity;
                            }
                            else
                            {
                                float num5 = (num1 - __instance.Accuracy) * __instance.DamageFalloffPerMeterBeyondEffectiveRange;
                                num4 = Mathf.Max(__instance.MinimumDamageFalloffBeyondEffectiveRange, num4 - num5);
                            }
                            if (!Utils.IsZero(num4) || baseAiFromObject.ForceApplyDamage())
                            {
                                if (baseAiFromObject.GetAiMode() != AiMode.Dead)
                                {
                                    if (MyBullet == true)
                                    {
                                        if (__instance.m_GunType == GunType.Rifle)
                                        {
                                            GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Rifle, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                                            MelonLogger.Msg("Got skill upgrade from your own shoot Rifle");
                                        }
                                        else if (__instance.m_GunType == GunType.Revolver)
                                        {
                                            GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Revolver, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                                            MelonLogger.Msg("Got skill upgrade from your own shoot Revolver");
                                        }
                                    }
                                }
                                if (MyBullet == true)
                                {
                                    baseAiFromObject.SetupDamageForAnim(hit.collider.transform.position, GameManager.GetPlayerTransform().position, component);
                                    baseAiFromObject.ApplyDamage(num4, bleedOutMinutes, DamageSource.Player, hit.collider.name);
                                }
                            }
                        }
                        else if (ActorFromObject != null)
                        {
                            MelonLogger.Msg("This is AnimalActor animal");
                            BodyDamage.Weapon bodyDamageWeapon = GunTypeMethods.ToBodyDamageWeapon(__instance.m_GunType);
                            float num1 = Vector3.Distance(GameManager.GetPlayerTransform().position, hit.collider.transform.position);
                            LocalizedDamage component = hit.collider.GetComponent<LocalizedDamage>();
                            float num4 = __instance.Damage * component.GetDamageScale(bodyDamageWeapon);
                            if ((double)num1 < (double)__instance.Accuracy)
                            {
                                if (!component.RollChanceToKill(bodyDamageWeapon))
                                    num4 = float.PositiveInfinity;
                            }else{
                                float num5 = (num1 - __instance.Accuracy) * __instance.DamageFalloffPerMeterBeyondEffectiveRange;
                                num4 = Mathf.Max(__instance.MinimumDamageFalloffBeyondEffectiveRange, num4 - num5);
                            }
                            if (MyMod.iAmHost)
                            {
                                ServerSend.ANIMALDAMAGE(0, ActorFromObject.gameObject.GetComponent<ObjectGuid>().Get(), num4);
                            }
                            else if (MyMod.sendMyPosition)
                            {
                                using (Packet _packet = new Packet((int)ClientPackets.ANIMALDAMAGE))
                                {
                                    _packet.Write(ActorFromObject.gameObject.GetComponent<ObjectGuid>().Get());
                                    _packet.Write(num4);
                                    SendTCPData(_packet);
                                }
                            }

                            if (num4 > 0 && ActorFromObject.m_Hp > 0 && MyBullet == true)
                            {
                                if (__instance.m_GunType == GunType.Rifle)
                                {
                                    GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Rifle, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                                    MelonLogger.Msg("Got skill upgrade from your own shoot Rifle");
                                }
                                else if (__instance.m_GunType == GunType.Revolver)
                                {
                                    GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Revolver, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                                    MelonLogger.Msg("Got skill upgrade from your own shoot Revolver");
                                }
                            }
                        }
                        else if (PlayerDamage != null && MyBullet == true)
                        {
                            MelonLogger.Msg("You damaged other player on " + PlayerDamage.m_Damage);
                            int bodypart = PlayerDamage.m_Type;

                            //GameAudioManager.Play3DSound("Play_ClothesTearing", PlayerDamage.m_Player);
                            if (MyMod.sendMyPosition == true)
                            {
                                using (Packet _packet = new Packet((int)ClientPackets.BULLETDAMAGE))
                                {
                                    _packet.Write((float)PlayerDamage.m_Damage);
                                    _packet.Write(bodypart);
                                    _packet.Write(PlayerDamage.m_ClientId);
                                    _packet.Write(false);
                                    SendTCPData(_packet);
                                }
                            }
                            if (MyMod.iAmHost == true)
                            {
                                using (Packet _packet = new Packet((int)ServerPackets.BULLETDAMAGE))
                                {
                                    ServerSend.BULLETDAMAGE(PlayerDamage.m_ClientId, (float)PlayerDamage.m_Damage,bodypart, 0);
                                }
                            }
                        }
                        else
                        {
                            MelonLogger.Msg("Bullet hit something different");
                            GearItem gearItemFromObject = Utils.GetGearItemFromObject(hit.collider.gameObject);
                            if ((bool)(UnityEngine.Object)gearItemFromObject)
                            {
                                RandomForce = UnityEngine.Random.Range(0.0f, __instance.m_GearImpactUpwardForce);
                                RandomTorque = new Vector3(UnityEngine.Random.Range(-__instance.m_GearImpactTorqueForce, __instance.m_GearImpactTorqueForce), UnityEngine.Random.Range(-__instance.m_GearImpactTorqueForce, __instance.m_GearImpactTorqueForce), UnityEngine.Random.Range(-__instance.m_GearImpactTorqueForce, __instance.m_GearImpactTorqueForce));
                                Vector3 force = -hit.normal * __instance.m_GearImpactForce + Vector3.up * RandomForce;
                                gearItemFromObject.ApplyForce(force, RandomTorque);
                            }
                        }
                        Renderer component1 = __instance.gameObject.GetComponent<Renderer>();
                        if ((UnityEngine.Object)component1 != (UnityEngine.Object)null && component1.enabled)
                            vp_DecalManager.Add(__instance.gameObject);
                        else
                            UnityEngine.Object.Destroy((UnityEngine.Object)__instance.gameObject);
                    }
                    else
                    {
                        UnityEngine.Object.Destroy((UnityEngine.Object)__instance.gameObject);
                        Utils.DebugBulletHit(position, p2);
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(vp_FPSShooter), "Start")] // Once
        internal class vp_FPSShooter_Start
        {
            public static void Postfix(vp_FPSShooter __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance != null && __instance.gameObject != null && __instance.ProjectilePrefab != null)
                {
                    if (__instance.gameObject.name == "Rifle" && __instance.ProjectilePrefab.name == "PistolBullet")
                    {
                        MyMod.PistolBulletPrefab = __instance.ProjectilePrefab;
                    }
                    if (__instance.gameObject.name == "Revolver" && __instance.ProjectilePrefab.name == "RevolverBullet")
                    {
                        MyMod.RevolverBulletPrefab = __instance.ProjectilePrefab;
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(vp_FPSShooter), "Fire")] // Once
        internal class vp_FPSShooter_End
        {
            public static void Prefix(vp_FPSShooter __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if ((UnityEngine.Object)__instance.m_Weapon == (UnityEngine.Object)null || (double)Time.time < (double)__instance.m_NextAllowedFireTime || (__instance.m_Weapon.ReloadInProgress() || !GameManager.GetPlayerAnimationComponent().IsAllowedToFire(__instance.m_Weapon.m_GunItem.m_AllowHipFire)) || GameManager.GetPlayerAnimationComponent().IsReloading())
                {
                    //MelonLogger.Msg("[vp_FPSShooter] Can't shoot now!");
                    return;
                }
                if (__instance.m_Weapon.GetAmmoCount() < 1)
                {
                    //MelonLogger.Msg("[vp_FPSShooter] Dry fire!");
                    MyMod.SendMultiplayerAudio("PLAY_RIFLE_DRY_3D");
                    return;
                }
                else
                {
                    if (__instance.m_Weapon.m_GunItem.m_IsJammed)
                    {
                        //MelonLogger.Msg("[vp_FPSShooter] Jammed!");
                        MyMod.SendMultiplayerAudio("PLAY_RIFLE_DRY_3D");
                        return;
                    }
                }
                Vector3 vector3 = __instance.m_Camera.transform.position;
                Quaternion quaternion = __instance.m_Camera.transform.rotation;

                for (int index = 0; index < __instance.ProjectileCount; ++index)
                {
                    if ((UnityEngine.Object)__instance.ProjectilePrefab != (UnityEngine.Object)null)
                    {
                        if (__instance.ProjectileCustomPrefab)
                        {
                            MelonLogger.Msg("[vp_FPSShooter] Flaregun projectile spawn! " + __instance.ProjectilePrefab.name);
                        }
                        else
                        {
                            MelonLogger.Msg("[vp_FPSShooter] Bullet projectile spawn " + __instance.ProjectilePrefab.name);
                        }

                        DataStr.ShootSync shoot = new DataStr.ShootSync();

                        shoot.m_projectilename = __instance.ProjectilePrefab.name;
                        shoot.m_position = vector3;
                        shoot.m_rotation = quaternion;
                        shoot.m_sceneguid = MyMod.level_guid;

                        if (__instance.ProjectilePrefab.name == "PistolBullet")
                        {
                            shoot.m_skill = GameManager.GetSkillRifle().GetEffectiveRange();
                            if (MyMod.playersData[0].m_Mimic == true && MyMod.players[0] != null)
                            {
                                if (MyMod.players[0] != null && MyMod.players[0].GetComponent<Comps.MultiplayerPlayerAnimator>() != null)
                                {
                                    string shootStrhing = "RifleShoot";
                                    if (MyMod.playersData[0] != null && MyMod.playersData[0].m_AnimState == "Ctrl")
                                    {
                                        shootStrhing = "RifleShoot_Sit";
                                    }

                                    MyMod.players[0].GetComponent<Comps.MultiplayerPlayerAnimator>().m_PreAnimStateHands = shootStrhing;
                                }
                            }
                        }

                        if (MyMod.sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.SHOOTSYNC))
                            {
                                _packet.Write(shoot);
                                SendTCPData(_packet);
                            }
                        }
                        if (MyMod.iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.SHOOTSYNC))
                            {
                                ServerSend.SHOOTSYNC(0, shoot, true);
                            }
                            if (shoot.m_projectilename == "GEAR_FlareGunAmmoSingle")
                            {
                                ExpeditionManager.RegisterFlaregunShot(MyMod.level_guid, GameManager.GetPlayerTransform().position);
                            }
                        }

                        if (MyMod.ExpeditionEditorUI != null && MyMod.ExpeditionEditorUI.activeSelf && shoot.m_projectilename == "GEAR_FlareGunAmmoSingle")
                        {
                            ExpeditionEditor.AddGear("GEAR_FlareGunShoot", vector3, quaternion, "");
                        }
                        MyMod.PushActionToMyDoll("Shoot");
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GearItem), "ManualStart")] // Once
        private static class GearItem_ManualStart
        {
            internal static void Postfix(GearItem __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.name == "GEAR_MedicalSupplies_hangar")
                {
                    __instance.m_WeightKG = 0.5f;
                }
                __instance.m_DailyHPDecay = 0;

                if (MyMod.IsCustomHandItem(__instance.name) || MyMod.IsUserGeneratedHandItem(__instance.name))
                {
                    if(__instance.name == "GEAR_SCPhoto")
                    {
                        if (__instance.gameObject.GetComponent<FirstPersonItem>() == null)
                        {
                            FirstPersonItem FPI = __instance.gameObject.AddComponent<FirstPersonItem>();
                            FPI.m_FirstPersonObjectName = "Stone";
                            FPI.m_FPSMeshID = (int)GameManager.GetVpFPSCamera().GetWeaponIDFromName(FPI.m_FirstPersonObjectName);
                            FPI.m_FPSWeapon = GameManager.GetVpFPSCamera().GetWeaponFromID(FPI.m_FPSMeshID);
                            FPI.m_UnWieldAudio = "Play_Photo";
                            FPI.m_WieldAudio = "Play_Photo";
                            GameObject reference = MyMod.GetGearItemObject("GEAR_Stone");
                            if (reference != null && reference.GetComponent<FirstPersonItem>() != null)
                            {
                                FPI.m_PlayerStateTransitions = reference.GetComponent<FirstPersonItem>().m_PlayerStateTransitions;
                            }
                            __instance.m_FirstPersonItem = FPI;
                        }
                    }
                    else if (__instance.name == "GEAR_SCMapPiece")
                    {
                        if (__instance.gameObject.GetComponent<FirstPersonItem>() == null)
                        {
                            FirstPersonItem FPI = __instance.gameObject.AddComponent<FirstPersonItem>();
                            FPI.m_FirstPersonObjectName = "Stone";
                            FPI.m_FPSMeshID = (int)GameManager.GetVpFPSCamera().GetWeaponIDFromName(FPI.m_FirstPersonObjectName);
                            FPI.m_FPSWeapon = GameManager.GetVpFPSCamera().GetWeaponFromID(FPI.m_FPSMeshID);
                            FPI.m_UnWieldAudio = "Play_MapPageFlip";
                            FPI.m_WieldAudio = "Play_MapPageFlip";
                            GameObject reference = MyMod.GetGearItemObject("GEAR_Stone");
                            if (reference != null && reference.GetComponent<FirstPersonItem>() != null)
                            {
                                FPI.m_PlayerStateTransitions = reference.GetComponent<FirstPersonItem>().m_PlayerStateTransitions;
                            }
                            __instance.m_FirstPersonItem = FPI;
                        }
                    } else if(__instance.name != "GEAR_Hacksaw")
                    {
                        if (__instance.gameObject.GetComponent<FirstPersonItem>() == null)
                        {
                            FirstPersonItem FPI = __instance.gameObject.AddComponent<FirstPersonItem>();

                            FPI.m_FirstPersonObjectName = "Flare";
                            FPI.m_FPSMeshID = (int)FPSMeshID.Flare;
                            GameObject FlareVp = GameObject.Find("/CHARACTER_FPSPlayer/WeaponView/FlareTransform/Flare");
                            if (FlareVp != null && FlareVp.GetComponent<vp_FPSWeapon>() != null)
                            {
                                FPI.m_FPSWeapon = FlareVp.GetComponent<vp_FPSWeapon>();
                            }
                            FPI.m_UnWieldAudio = "Play_UnwieldItemFlare";
                            FPI.m_WieldAudio = "Play_WieldItemFlare";
                            GameObject reference = MyMod.GetGearItemObject("GEAR_FlareA");
                            if (reference != null && reference.GetComponent<FirstPersonItem>() != null)
                            {
                                FPI.m_PlayerStateTransitions = reference.GetComponent<FirstPersonItem>().m_PlayerStateTransitions;
                            }
                            __instance.m_FirstPersonItem = FPI;
                        }
                    }else{
                        if (__instance.gameObject.GetComponent<FirstPersonItem>() == null)
                        {
                            FirstPersonItem FPI = __instance.gameObject.AddComponent<FirstPersonItem>();

                            FPI.m_FirstPersonObjectName = "Revolver";
                            FPI.m_FPSMeshID = (int)FPSMeshID.Revolver;
                            GameObject FlareVp = GameObject.Find("/CHARACTER_FPSPlayer/WeaponView/RevolverTransform/Revolver");
                            if (FlareVp != null && FlareVp.GetComponent<vp_FPSWeapon>() != null)
                            {
                                FPI.m_FPSWeapon = FlareVp.GetComponent<vp_FPSWeapon>();
                            }
                            FPI.m_UnWieldAudio = "Play_UnwieldItemFlare";
                            FPI.m_WieldAudio = "Play_WieldItemFlare";
                            GameObject reference = MyMod.GetGearItemObject("GEAR_Revolver");
                            if (reference != null && reference.GetComponent<FirstPersonItem>() != null)
                            {
                                FPI.m_PlayerStateTransitions = reference.GetComponent<FirstPersonItem>().m_PlayerStateTransitions;
                            }
                            __instance.m_FirstPersonItem = FPI;
                        }
                    }
                } else
                {
                    if (__instance.name == "GEAR_SCNote")
                    {
                        if (__instance.gameObject.GetComponent<FirstPersonItem>() == null)
                        {
                            FirstPersonItem FPI = __instance.gameObject.AddComponent<FirstPersonItem>();
                            FPI.m_FirstPersonObjectName = "Stone";
                            FPI.m_FPSMeshID = (int)GameManager.GetVpFPSCamera().GetWeaponIDFromName(FPI.m_FirstPersonObjectName);
                            FPI.m_FPSWeapon = GameManager.GetVpFPSCamera().GetWeaponFromID(FPI.m_FPSMeshID);
                            FPI.m_UnWieldAudio = "Play_SndInvPaper";
                            FPI.m_WieldAudio = "Play_SndInvPaper";
                            GameObject reference = MyMod.GetGearItemObject("GEAR_Stone");
                            if (reference != null && reference.GetComponent<FirstPersonItem>() != null)
                            {
                                FPI.m_PlayerStateTransitions = reference.GetComponent<FirstPersonItem>().m_PlayerStateTransitions;
                            }
                            //Harvest HAR = __instance.m_Harvest;
                            //if(HAR == null)
                            //{
                            //    HAR = __instance.gameObject.AddComponent<Harvest>();
                            //    __instance.m_Harvest = HAR;
                            //    HAR.m_
                            //}
                            __instance.m_FirstPersonItem = FPI;
                        }
                    } else if(__instance.name == "GEAR_ScrapLead")
                    {
                        //Harvest Harvy = __instance.gameObject.AddComponent<Harvest>();
                        //Harvy.m_Audio = "Play_HarvestingMetalSaw";
                        //Harvy.m_DurationMinutes = 3;
                        //Harvy.m_YieldGear = new GearItem[] { GetGearItemPrefab("GEAR_ScrapLead") };
                        //Harvy.m_YieldGearUnits = new int[] { 1 };
                        //__instance.m_Harvest = Harvy;
                    } else if (__instance.name == "GEAR_Softwood" || __instance.name == "GEAR_Hardwood")
                    {
                        Harvest Harvy = __instance.gameObject.AddComponent<Harvest>();
                        Harvy.m_Audio = "PLAY_HARVESTINGGENERIC";
                        Harvy.m_DurationMinutes = 15;
                        Harvy.m_YieldGear = new GearItem[] { GetGearItemPrefab("GEAR_SCWoodForm") };
                        Harvy.m_YieldGearUnits = new int[] { 3 };
                        Harvy.m_RequiredTools = new ToolsItem[] { GetGearItemPrefab("GEAR_Knife").GetComponent<ToolsItem>() };
                        __instance.m_Harvest = Harvy;
                    } else if (__instance.name == "GEAR_SCWoodFormB")
                    {
                        EvolveItem Evo = __instance.gameObject.AddComponent<EvolveItem>();
                        Evo.m_GearItem = __instance;
                        Evo.m_GearItemToBecome = GetGearItemPrefab("GEAR_SCDoorKeyLeadTemp");
                        Evo.m_RequireIndoors = false;
                        Evo.m_TimeToEvolveGameDays = 0.04f;
                        __instance.m_EvolveItem = Evo;
                    }
                }
                if(MyMod.IsUserGeneratedHandItem(__instance.name))
                {
                    if (__instance.m_ObjectGuid != null)
                    {
                        MelonLogger.Msg("Going render photo " + __instance.m_ObjectGuid.m_Guid);
                        Texture2D tex = MPSaveManager.GetPhotoTexture(__instance.m_ObjectGuid.m_Guid, __instance.m_GearName);
                        if (tex)
                        {
                            __instance.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = tex;
                        }
                    }
                }
                else if (__instance.name == "GEAR_CookingPot")
                {
                    if (__instance.gameObject.GetComponent<ClothingItem>() == null)
                    {
                        ClothingItem CLTH = __instance.gameObject.AddComponent<ClothingItem>();
                        CLTH.m_Region = ClothingRegion.Head;
                        CLTH.m_MinLayer = ClothingLayer.Mid;
                        CLTH.m_MaxLayer = ClothingLayer.Mid;
                        CLTH.m_WornMovementSoundCategory = ClothingMovementSound.None;
                        CLTH.m_FootwearType = FootwearType.None;
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
                        __instance.gameObject.AddComponent<Comps.CookpotHelmet>();
                        __instance.gameObject.GetComponent<Comps.CookpotHelmet>().m_GearItem =  __instance;
                        __instance.gameObject.GetComponent<Comps.CookpotHelmet>().m_ClothingItem = CLTH;
                    }
                } else if (__instance.name == "GEAR_SCHeatPack" || __instance.name == "GEAR_SCHeatPackC")
                {
                    if (__instance.gameObject.GetComponent<HeatSource>() == null)
                    {
                        HeatSource HTS = __instance.gameObject.AddComponent<HeatSource>();
                        HTS.m_TempIncrease = 10;
                        HTS.m_MaxTempIncrease = 10;
                        HTS.m_StartingTemp = 10;
                        HTS.m_MaxTempIncreaseInnerRadius = 2;
                        HTS.m_MaxTempIncreaseOuterRadius = 1;
                        HTS.m_StartOn = true;
                        HTS.m_TimeToReachMaxTempMinutes = 1;
                    }
                }
                if (__instance.m_ResearchItem)
                {
                    MyMod.PatchBookReadTime(__instance);
                }
                if(__instance.name == "GEAR_SCDoorKey" || __instance.name == "GEAR_SCDoorKeyLead")
                {
                    if (__instance.m_ObjectGuid == null)
                    {
                        __instance.m_ObjectGuid = __instance.gameObject.AddComponent<ObjectGuid>();
                    }else if (!string.IsNullOrEmpty(__instance.m_ObjectGuid.m_Guid) && __instance.m_ObjectGuid.m_Guid.Contains('_'))
                    {
                        __instance.m_LocalizedDisplayName.m_LocalizationID = __instance.m_ObjectGuid.m_Guid.Split('_')[0];
                    }
                }
                if (ExpeditionManager.IsClueGear(__instance.m_GearName))
                {
                    if (__instance.m_NarrativeCollectibleItem == null)
                    {
                        NarrativeCollectibleItem N = __instance.gameObject.AddComponent<NarrativeCollectibleItem>();
                        N.m_HudMessageOnPickup = new LocalizedString();
                        N.m_HudMessageOnPickup.m_LocalizationID = "Youn found [50C878]special item[-] use it to start advanced expedition via handheld radio.";
                        N.m_JournalEntryNumber = -1;
                        N.m_ShowDuringInspect = true;
                        __instance.m_NarrativeCollectibleItem = N;
                    }

                    if(__instance.m_GearName == "GEAR_SCHopeless")
                    {
                        __instance.m_NarrativeCollectibleItem.m_NarrativeTextLocID = "I found out that a civilian plane crashed on Timberwolf Mountain, you can rummage around there yourself. I won't go to this godforsaken hole teeming with wolves.";
                    }else if (__instance.m_GearName == "GEAR_SCHopeless")
                    {

                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(ResearchItem), "Read")] // Once
        private static class ResearchItem_Read
        {
            internal static void Postfix(ResearchItem __instance, float timeOfDayHours)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.gameObject.GetComponent<GearItem>() != null)
                {
                    GearItem gi = __instance.gameObject.GetComponent<GearItem>();

                    if (gi.m_ObjectGuid)
                    {
                        string Key = gi.m_ObjectGuid.Get();

                        MelonLogger.Msg("Have read book " + gi.m_GearName + " GUID " + Key + " Progress "+__instance.m_ElapsedHours+"/"+ __instance.m_TimeRequirementHours);

                        if(MyMod.BooksResearched.ContainsKey(Key))
                        {
                            MyMod.BooksResearched.Remove(Key);
                        }
                        MyMod.BooksResearched.Add(Key, __instance.m_ElapsedHours);
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(ClothingSlot), "SetPaperDollTexture")] // Once
        private static class ClothingSlot_SetPaperDollTexture
        {
            internal static void Postfix(ClothingSlot __instance, GearItem gi)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                if (gi && gi.m_ClothingItem && !string.IsNullOrEmpty(gi.m_ClothingItem.m_PaperDollTextureName)  && gi.m_GearName == "GEAR_CookingPot")
                    {
                    for (int index = 0; index < __instance.m_PaperDollSlotWidgets.Count; ++index)
                    {
                        __instance.m_PaperDollSlotWidgets[index].enabled = true;

                        string TxtName = "PaperDoll_POT";
                        if (GameManager.GetPlayerManagerComponent().m_VoicePersona == VoicePersona.Female)
                        {
                            TxtName = TxtName + "_F";
                        }
                        Texture2D Txt = Utils.GetCachedTexture(TxtName);
                        if (!Txt)
                        {
                            Txt = MyMod.LoadedBundle.LoadAsset(TxtName).Cast<Texture2D>();
                            Utils.CacheTexture(TxtName, Txt);
                        }
                        __instance.m_PaperDollSlots[index].mainTexture = (Texture)Txt;
                    }
                }

                ////Silly hook

                //if (gi && gi.m_ClothingItem && !string.IsNullOrEmpty(gi.m_ClothingItem.m_PaperDollTextureName))
                //{
                //    for (int index = 0; index < __instance.m_PaperDollSlotWidgets.Count; ++index)
                //    {
                //        __instance.m_PaperDollSlotWidgets[index].enabled = true;


                //        bool Male = GameManager.GetPlayerManagerComponent().m_VoicePersona == VoicePersona.Male;
                //        AssetBundleRef Ref;
                //        string TxtName = gi.m_ClothingItem.m_PaperDollTextureName;

                //        if (Male)
                //        {
                //            Ref = InterfaceManager.m_Panel_Clothing.m_PaperDollBundleFemale;
                //            TxtName = TxtName + "_F";
                //        } else
                //        {
                //            Ref = InterfaceManager.m_Panel_Clothing.m_PaperDollBundleMale;
                //            if (TxtName.EndsWith("_F"))
                //            {
                //                TxtName = TxtName.Remove(TxtName.Length - 2, 2);
                //            }
                //        }
                //        __instance.m_PaperDollSlots[index].material.SetTexture("_MainTex", (Texture)Ref.LoadAsset<Texture2D>(TxtName));

                //    }
                //}
            }
        }

        public static bool DuppableGearItem(string GearName)
        {
            if (GearName.Contains("TechnicalBackpack") == true
                || GearName.Contains("Crampons") == true
                || GearName.Contains("CanneryCodeNote") == true
                || GearName.Contains("BallisticVest") == true
                || GearName.Contains("BlackrockCodeNote") == true
                || GearName.Contains("MountainTownFarmKey") == true)
            {
                MelonLogger.Msg(ConsoleColor.Blue, "Item " + GearName + " is can be picked by other player");
                return true;
            }else{
                return false;
            }
        }
        public static bool GarbadgeFilter(string GearName)
        {
            if (GearName == "GEAR_CrowFeather"
                || GearName == "GEAR_Stone"
                || GearName == "GEAR_Stick")
            {
                return true;
            }

            return false;
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ProcessInspectablePickupItem")] // Once
        private static class Inventory_Pickup
        {
            internal static void Prefix(PlayerManager __instance, GearItem pickupItem)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (pickupItem.m_BeenInPlayerInventory == false)
                {
                    MelonLogger.Msg("Pickedup " + pickupItem.m_GearName);
                    pickupItem.m_BeenInPlayerInventory = true;
                    Vector3 V3 = pickupItem.gameObject.transform.position;
                    int ISNTID = pickupItem.m_InstanceID;
                    string GearName = pickupItem.m_GearName;
                    NarrativeCollectibleItem Narrative = pickupItem.GetComponent<NarrativeCollectibleItem>();

                    if (GearName == "GEAR_Knife")
                    {
                        if (Supporters.ConfiguratedBenefits.m_Knife)
                        {
                            pickupItem.m_GearName = "GEAR_JeremiahKnife";
                            string Copy = pickupItem.Serialize();
                            UnityEngine.Object.Destroy(pickupItem.gameObject);
                            GameObject reference = MyMod.GetGearItemObject("GEAR_JeremiahKnife");
                            GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(reference, V3, pickupItem.gameObject.transform.rotation);
                            newGear.GetComponent<GearItem>().Deserialize(Copy);
                            newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;
                            Narrative = newGear.GetComponent<NarrativeCollectibleItem>();
                            __instance.ProcessInspectablePickupItem(newGear.GetComponent<GearItem>());
                        }
                    }

                    if (DuppableGearItem(GearName) || Narrative != null)
                    {
                        return;
                    }
                    if (GarbadgeFilter(GearName))
                    {
                        return;
                    }
                    Shared.AddPickedGear(V3, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent, ClientUser.myId, ISNTID, GearName, true);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ProcessPickupWithNoInspectScreen")] // Once
        private static class Inventory_Pickup2
        {
            internal static void Prefix(PlayerManager __instance, GearItem pickupItem)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (pickupItem.m_BeenInPlayerInventory == false)
                {
                    MelonLogger.Msg("Pickedup " + pickupItem.m_GearName);
                    pickupItem.m_BeenInPlayerInventory = true;
                    Vector3 V3 = pickupItem.gameObject.transform.position;
                    int ISNTID = pickupItem.m_InstanceID;
                    string GearName = pickupItem.m_GearName;
                    NarrativeCollectibleItem Narrative = pickupItem.GetComponent<NarrativeCollectibleItem>();

                    if (GearName == "GEAR_Knife")
                    {
                        if (Supporters.ConfiguratedBenefits.m_Knife)
                        {
                            pickupItem.m_GearName = "GEAR_JeremiahKnife";
                            string Copy = pickupItem.Serialize();
                            UnityEngine.Object.Destroy(pickupItem.gameObject);
                            GameObject reference = MyMod.GetGearItemObject("GEAR_JeremiahKnife");
                            GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(reference, V3, pickupItem.gameObject.transform.rotation);
                            newGear.GetComponent<GearItem>().Deserialize(Copy);
                            newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;
                            Narrative = newGear.GetComponent<NarrativeCollectibleItem>();
                            __instance.ProcessPickupWithNoInspectScreen(newGear.GetComponent<GearItem>(), true);
                        }
                    }

                    if (DuppableGearItem(GearName) || Narrative != null)
                    {
                        return;
                    }
                    if (GarbadgeFilter(GearName))
                    {
                        return;
                    }
                    Shared.AddPickedGear(V3, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent, ClientUser.myId, ISNTID, GearName, true);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InitializeObjectToPlace")] // Once
        private static class Inventory_Pickup3
        {
            public static void Prefix(PlayerManager __instance, GameObject go)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (go != null && go.GetComponent<GearItem>() != null)
                {
                    GearItem pickupItem = go.GetComponent<GearItem>();
                    if (pickupItem.m_BeenInPlayerInventory == false)
                    {
                        MelonLogger.Msg("Pickedup " + pickupItem.m_GearName);
                        pickupItem.m_BeenInPlayerInventory = true;
                        Vector3 V3 = pickupItem.gameObject.transform.position;
                        int ISNTID = pickupItem.m_InstanceID;
                        string GearName = pickupItem.m_GearName;
                        NarrativeCollectibleItem Narrative = pickupItem.GetComponent<NarrativeCollectibleItem>();

                        if (GearName == "GEAR_Knife")
                        {
                            if (Supporters.ConfiguratedBenefits.m_Knife)
                            {
                                pickupItem.m_GearName = "GEAR_JeremiahKnife";
                                string Copy = pickupItem.Serialize();
                                UnityEngine.Object.Destroy(pickupItem.gameObject);
                                GameObject reference = MyMod.GetGearItemObject("GEAR_JeremiahKnife");
                                GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(reference, V3, pickupItem.gameObject.transform.rotation);
                                newGear.GetComponent<GearItem>().Deserialize(Copy);
                                newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;
                                Narrative = newGear.GetComponent<NarrativeCollectibleItem>();
                                __instance.InitializeObjectToPlace(newGear);
                            }
                        }

                        if (DuppableGearItem(GearName) || Narrative != null)
                        {
                            return;
                        }
                        if (GarbadgeFilter(GearName))
                        {
                            return;
                        }
                        Shared.AddPickedGear(V3, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent, ClientUser.myId, ISNTID, GearName, true);
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Container), "Start")] // Once
        private static class Container_Start
        {
            internal static void Prefix(Container __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                if (__instance.gameObject.GetComponent<ContainersSync>() == null)
                {
                    __instance.gameObject.AddComponent<Comps.ContainersSync>().m_Obj = __instance.gameObject;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "OnSelectFeatsBack")] // Once
        internal class Panel_MainMenu_FeatBack
        {
            public static bool Prefix(Panel_MainMenu __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.PendingSave != null)
                {
                    MyMod.SelectGenderForConnection();
                    return false;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "OnSelectFeatsContinue")] // Once
        internal class Panel_MainMenu_FeatContinue
        {
            public static bool Prefix(Panel_MainMenu __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.PendingSave != null)
                {
                    GameAudioManager.PlayGuiConfirm();
                    FeatEnabledTracker.m_FeatsEnabledThisSandbox = new Il2CppSystem.Collections.Generic.List<FeatType>();
                    for (int index1 = 0; index1 < __instance.m_SelectedFeats.Count; ++index1)
                    {
                        for (int index2 = 0; index2 < GameManager.GetFeatsManager().GetNumFeats(); ++index2)
                        {
                            Feat featFromIndex = GameManager.GetFeatsManager().GetFeatFromIndex(index2);
                            if (__instance.m_SelectedFeats[index1] == featFromIndex.m_LocalizedDisplayName.m_LocalizationID)
                            {
                                FeatEnabledTracker.m_FeatsEnabledThisSandbox.Add(featFromIndex.m_FeatType);
                            }
                        }
                    }
                    if (!MyMod.ShouldCreateSaveForHost)
                    {
                        MyMod.ForcedCreateSave(MyMod.PendingSave);
                    }else{
                        MyMod.SelectNameForHostSaveFile();
                    }
                    
                    return false;
                }
                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "OnSelectSlotNameContinue")] // Once
        internal class Panel_MainMenu_OnSelectSlotNameContinue
        {
            public static bool Prefix(Panel_MainMenu __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.PendingSave != null && MyMod.ShouldCreateSaveForHost)
                {
                    string inputFieldText = InterfaceManager.m_Panel_Confirmation.GetInputFieldText();
                    MyMod.ForcedCreateSave(MyMod.PendingSave, inputFieldText);
                    return false;
                }

                return true;
            }
        }
        

        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectRegion_Map), "OnSelectRegionContinue")] // Once
        internal class Panel_SelectRegion_Map_Done
        {
            public static bool Prefix(Panel_SelectRegion_Map __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.PendingSave != null || MenuChange.MenuMode == "Vote" || MenuChange.MenuMode == "NewGameSelect")
                {
                    return false;
                }
                return true;
            }
            public static void Postfix(Panel_SelectRegion_Map __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.PendingSave != null)
                {
                    GameManager.m_StartRegion = __instance.m_SelectedItem.m_Region;
                    MyMod.PendingSave.m_Location = (int)__instance.m_SelectedItem.m_Region;
                    __instance.Enable(false);
                    MyMod.SelectGenderForConnection();
                }
                if(MenuChange.MenuMode == "Vote")
                {
                    if (SteamConnect.CanUseSteam)
                    {
                        SteamConnect.Main.VoteForRegion((int)__instance.m_SelectedItem.m_Region);
                    }
                    __instance.Enable(false);
                    MyMod.m_Panel_Sandbox.Enable(true);
                    MenuChange.ChangeMenuItems("Lobby");
                }
                if(MenuChange.MenuMode == "NewGameSelect")
                {
                    MyMod.LobbyStartingRegion = (int)__instance.m_SelectedItem.m_Region;
                    MyMod.LobbyStartingExperience = MenuChange.TempExperience;
                    SteamConnect.Main.SetNewGameSettings(MyMod.LobbyStartingRegion, MyMod.LobbyStartingExperience);
                    SteamConnect.Main.SetLobbyState("SelectedNewSave");
                    __instance.Enable(false);
                    MyMod.m_Panel_Sandbox.Enable(true);
                    MenuChange.ChangeMenuItems("Lobby");
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectRegion_Map), "OnClickBack")] // Once
        internal class Panel_SelectRegion_Map_OnClickBack
        {
            public static bool Prefix(Panel_SelectRegion_Map __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.PendingSave != null || MenuChange.MenuMode == "Vote" || MenuChange.MenuMode == "NewGameSelect")
                {
                    if(MenuChange.MenuMode == "Vote")
                    {
                        __instance.Enable(false);
                        MyMod.m_Panel_Sandbox.Enable(true);
                        MenuChange.ChangeMenuItems("Lobby");
                    }
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectRegion), "OnClickBack")] // Once
        internal class Panel_SelectRegion_OnClickBack
        {
            public static bool Prefix(Panel_SelectRegion __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.PendingSave != null || MenuChange.MenuMode == "Vote")
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectSurvivor), "OnSelectSurvivorMale")] // Once
        internal class Panel_SelectSurvivor_Select1
        {
            public static bool Prefix(Panel_SelectSurvivor __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.PendingSave != null)
                {
                    InterfaceManager.m_Panel_OptionsMenu.m_State.m_VoicePersona = VoicePersona.Male;
                    __instance.Enable(false);
                    MyMod.SelectBagesForConnection();

                    return false;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectSurvivor), "OnSelectSurvivorFemale")] // Once
        internal class Panel_SelectSurvivor_Select2
        {
            public static bool Prefix(Panel_SelectSurvivor __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.PendingSave != null)
                {
                    InterfaceManager.m_Panel_OptionsMenu.m_State.m_VoicePersona = VoicePersona.Female;
                    __instance.Enable(false);
                    MyMod.SelectBagesForConnection();

                    return false;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BaseAi), "AnimSetTrigger")] // Calls Sometimes
        private static class GetID
        {
            internal static void Postfix(BaseAi __instance, int id)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance != null && __instance.gameObject != null && __instance.gameObject.GetComponent<ObjectGuid>() != null && __instance.gameObject.activeSelf == true)
                {
                    if (MyMod.iAmHost == true && MyMod.AnimalsController == true)
                    {
                        GameObject animal = __instance.gameObject;
                        //MelonLogger.Msg("Animal with GUID " + animal.GetComponent<ObjectGuid>().Get() + " used trigger with hash name " + id);

                        DataStr.AnimalTrigger trigg = new DataStr.AnimalTrigger();

                        trigg.m_Guid = animal.GetComponent<ObjectGuid>().Get();
                        trigg.m_Trigger = id;

                        if (MyMod.iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.ANIMALSYNCTRIGG))
                            {
                                ServerSend.ANIMALSYNCTRIGG(0, trigg, true);
                            }
                        }

                        if (MyMod.sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.ANIMALSYNCTRIGG))
                            {
                                _packet.Write(trigg);
                                SendTCPData(_packet);
                            }
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BaseAi), "Awake")] // Once
        private static class AI_Hack
        {
            internal static void Postfix(BaseAi __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                if (__instance != null && __instance.gameObject != null)
                {
                    GameObject animal = __instance.gameObject;
                    Comps.AnimalUpdates au = animal.GetComponent<Comps.AnimalUpdates>();
                    if (au == null)
                    {
                        animal.AddComponent<Comps.AnimalUpdates>();
                        au = animal.GetComponent<Comps.AnimalUpdates>();
                        au.m_Animal = animal;
                    }
                }
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(BaseAi), "Stun")] // Once
        //private static class BaseAi_Stun
        //{
        //    internal static void Postfix(BaseAi __instance)
        //    {
        //        if (MyMod.CrazyPatchesLogger == true)
        //        {
        //            StackTrace st = new StackTrace(new StackFrame(true));
        //            MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
        //            MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
        //        }

        //        if (__instance.gameObject)
        //        {
        //            GameObject reference = MyMod.GetGearItemObject("gear_rabbitcarcass");
        //            GameObject obj = UnityEngine.Object.Instantiate<GameObject>(reference, __instance.gameObject.transform.position, __instance.gameObject.transform.rotation);

        //            if (obj.GetComponent<GearItem>())
        //            {
        //                MyMod.SendDropItem(obj.GetComponent<GearItem>(), 0, 0, true);
        //            }
        //            if (MyMod.iAmHost == true)
        //            {
        //                using (Packet _packet = new Packet((int)ServerPackets.ANIMALDELETE))
        //                {
        //                    ServerSend.ANIMALDELETE(0, __instance.gameObject.GetComponent<ObjectGuid>().Get());
        //                }
        //            }

        //            if (MyMod.sendMyPosition == true)
        //            {
        //                using (Packet _packet = new Packet((int)ClientPackets.ANIMALDELETE))
        //                {
        //                    _packet.Write(__instance.gameObject.GetComponent<ObjectGuid>().Get());
        //                    SendTCPData(_packet);
        //                }
        //            }
        //            UnityEngine.Object.Destroy(__instance.gameObject);
        //        }
        //    }
        //}
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ProcessBodyHarvestInteraction")] // Once
        private static class BodyHarvest_OutOfSomeoneAlready
        {
            internal static bool Prefix(BodyHarvest bh, bool playBookEndAnim)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                GameObject shokal = bh.gameObject;

                if (shokal.GetComponent<ObjectGuid>() != null)
                {
                    for (int i = 0; i < MyMod.playersData.Count; i++)
                    {
                        if (MyMod.playersData[i] != null)
                        {
                            string otherAnimlGuid = MyMod.playersData[i].m_HarvestingAnimal;
                            if (otherAnimlGuid == shokal.GetComponent<ObjectGuid>().Get())
                            {
                                HUDMessage.AddMessage(MyMod.playersData[i].m_Name + " IS BUSY WITH THIS ALREADY");
                                return false;
                            }
                        }
                    }
                    if (MyMod.InOnline())
                    {
                        MyMod.RequestAnimalCorpseInteration(bh);
                        return false;
                    }else{
                        return true;
                    }
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_BodyHarvest), "HarvestSuccessful")] // Once
        private static class Panel_BodyHarvest_Hack
        {
            internal static void Prefix(Panel_BodyHarvest __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance != null && __instance.m_BodyHarvest != null && __instance.m_BodyHarvest.gameObject != null && __instance.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    MelonLogger.Msg("Harvested meat " + __instance.m_MenuItem_Meat.m_HarvestAmount);
                    MelonLogger.Msg("Harvested guts " + __instance.m_MenuItem_Gut.m_HarvestAmount);
                    MelonLogger.Msg("Harvested hide " + __instance.m_MenuItem_Hide.m_HarvestAmount);

                    DataStr.HarvestStats Harvey = new DataStr.HarvestStats();
                    Harvey.m_Meat = __instance.m_MenuItem_Meat.m_HarvestAmount;
                    Harvey.m_Guts = (int)__instance.m_MenuItem_Gut.m_HarvestAmount;
                    Harvey.m_Hide = (int)__instance.m_MenuItem_Hide.m_HarvestAmount;
                    Harvey.m_Guid = "";

                    if (__instance.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>() != null)
                    {
                        Harvey.m_Guid = __instance.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>().Get();
                    }

                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.DONEHARVASTING))
                        {
                            _packet.Write(Harvey);
                            SendTCPData(_packet);
                        }
                    }
                    else if (MyMod.iAmHost == true)
                    {
                        Shared.OnAnimalCorpseChanged(Harvey.m_Guid, Harvey.m_Meat, Harvey.m_Guts, Harvey.m_Hide);
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BaseAi), "ApplyDamage", new System.Type[] { typeof(float), typeof(float), typeof(DamageSource), typeof(string) })] // Once
        private static class AI_Hack_Damage
        {
            internal static bool Prefix(BaseAi __instance, float damage)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                Comps.AnimalUpdates au = __instance.gameObject.GetComponent<Comps.AnimalUpdates>();
                bool NeedApplyDamage = false;
                if (MyMod.AnimalsController == true || au.m_MyControlled)
                {
                    NeedApplyDamage = true;
                }

                return NeedApplyDamage;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(ArrowItem), "InflictDamage", new System.Type[] { typeof(GameObject), typeof(float), typeof(bool), typeof(string), typeof(Vector3) })] // Once
        private static class ArrowItem_DamageFix
        {            
            internal static bool Prefix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                return false;
            }
            internal static void Postfix(ArrowItem __instance, GameObject victim, float damageScalar, bool stickIn, string collider, Vector3 collisionPoint, BaseAi __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                BaseAi baseAi = null;
                if (victim.layer == 16)
                    baseAi = victim.GetComponent<BaseAi>();
                else if (victim.layer == 27)
                    baseAi = victim.transform.GetComponentInParent<BaseAi>();
                if (baseAi == null)
                {
                    __result = null;
                    return;
                }

                LocalizedDamage component = victim.GetComponent<LocalizedDamage>();
                double num = (double)StatsManager.IncrementValue(StatID.SuccessfulHits_Bow);
                float bleedOutMinutes = component.GetBleedOutMinutes(BodyDamage.Weapon.Arrow);
                float damage = __instance.m_VictimDamage * damageScalar * component.GetDamageScale(BodyDamage.Weapon.Arrow);
                if (!baseAi.m_IgnoreCriticalHits && component.RollChanceToKill(BodyDamage.Weapon.Arrow))
                {
                    damage = float.PositiveInfinity;
                }

                if (baseAi.GetAiMode() != AiMode.Dead)
                {
                    if (__instance.gameObject != null && __instance.gameObject.GetComponent<Comps.DestoryArrowOnHit>() == null)
                    {
                        MelonLogger.Msg("I am hit target with bow");
                        GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Archery, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                    }
                    else
                    {
                        MelonLogger.Msg("Other player hit targer with bow");
                    }
                }
                baseAi.SetupDamageForAnim(collisionPoint, GameManager.GetPlayerTransform().position, component);
                baseAi.ApplyDamage(damage, !stickIn ? 0.0f : bleedOutMinutes, DamageSource.Player, collider);

                if (__instance.m_EmbeddedDepth > 0.0)
                {
                    __instance.m_EmbeddedInAi = baseAi;
                }

                __result = baseAi;
                return;
            }
        }

        // LEFT OVERS OF CARRYING BODIES, HOOK TO NOT FOURCE DEATH AFTER LOADED
        //[HarmonyLib.HarmonyPatch(typeof(LoadScene), "LoadLevelWhenFadeOutComplete")]
        //public class LoadScene_Load
        //{
        //    public static bool Prefix(LoadScene __instance)
        //    {
        //        MyMod.NotNeedToPauseUntilLoaded = true;
        //        //if (GameManager.GetPlayerManagerComponent().PlayerIsDead() && IsCarringMe == false )
        //        //    return false;
        //        string str = (string)null;
        //        if (__instance.m_SceneCanBeInstanced)
        //            str = GameManager.StripOptFromSceneName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        //        string sceneName = __instance.m_SceneToLoad;
        //        if (GameManager.m_SceneTransitionData.m_ForceNextSceneLoadTriggerScene != null)
        //            sceneName = GameManager.m_SceneTransitionData.m_ForceNextSceneLoadTriggerScene;
        //        GameManager.m_SceneTransitionData.m_SpawnPointName = __instance.m_ExitPointName;
        //        GameManager.m_SceneTransitionData.m_SpawnPointAudio = __instance.m_SoundDuringFadeIn;
        //        GameManager.m_SceneTransitionData.m_ForceSceneOnNextNavMapLoad = (string)null;
        //        GameManager.m_SceneTransitionData.m_ForceNextSceneLoadTriggerScene = str;
        //        GameManager.m_SceneTransitionData.m_SceneLocationLocIDToShow = __instance.m_SceneLocationLocIDToShow;
        //        GameManager.m_SceneTransitionData.m_Location = (string)null;
        //        GameRegion UselessDummy = new GameRegion();
        //        if (RegionManager.GetRegionFromString(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, out UselessDummy))
        //            GameManager.m_SceneTransitionData.m_PosBeforeInteriorLoad = __instance.gameObject.transform.position;
        //        if (__instance.m_SceneCanBeInstanced)
        //        {
        //            GameManager.m_SceneTransitionData.m_PosBeforeInteriorLoad = __instance.gameObject.transform.position;
        //            GameManager.m_SceneTransitionData.m_SceneSaveFilenameNextLoad = sceneName + "_" + __instance.m_GUID;
        //        }else{
        //            GameManager.m_SceneTransitionData.m_SceneSaveFilenameNextLoad = sceneName;
        //        }

        //        MelonLogger.Msg(ConsoleColor.Yellow, "GameManager.m_SceneTransitionData.m_SceneSaveFilenameNextLoad " + GameManager.m_SceneTransitionData.m_SceneSaveFilenameNextLoad);


        //        GameManager.LoadScene(sceneName, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent);
        //        return false;
        //    }
        //}
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ReleaseThrownObject")] // Once
        public class PlayerManager_Throw
        {
            public static void Prefix(PlayerManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_ThrownItem != null)
                {
                    //MelonLogger.Msg("[PlayerManager][PreFix] ReleaseThrownObject " + __instance.m_ThrownItem.m_GearName);
                    MyMod.SaveThrowingItem = __instance.m_ThrownItem;
                }
                else
                {
                    MelonLogger.Msg("[PlayerManager][PreFix] Trying throw NULL somehow, wot?");
                }
            }
            public static void Postfix(PlayerManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.SaveThrowingItem != null)
                {
                    MelonLogger.Msg("[PlayerManager][Postfix] ReleaseThrownObject SaveThrowingItem " + MyMod.SaveThrowingItem.name);

                    if (MyMod.SaveThrowingItem.name.StartsWith("GEAR_Stone") || MyMod.SaveThrowingItem.name.StartsWith("GEAR_NoiseMaker"))
                    {
                        Vector3 V3 = MyMod.SaveThrowingItem.transform.position;
                        Quaternion Qu = MyMod.SaveThrowingItem.transform.rotation;

                        MelonLogger.Msg("[PlayerManager][Postfix] Throwing stone " + V3.x + " y " + V3.y + " z " + V3.z);
                        if (MyMod.SaveThrowingItem.name.StartsWith("GEAR_NoiseMaker"))
                        {
                            if (!MyMod.ServerConfig.m_PVP)
                            {
                                MyMod.SaveThrowingItem.m_NoiseMakerItem.m_PlayerDamageInflictionInRadius = 15;
                                MyMod.SaveThrowingItem.m_NoiseMakerItem.m_PlayerDamageRadius = MyMod.SaveThrowingItem.m_NoiseMakerItem.m_AIDamageRadius;
                            }
                        }

                        DataStr.ShootSync stone = new DataStr.ShootSync();
                        stone.m_position = V3;
                        stone.m_rotation = Qu;
                        if (MyMod.SaveThrowingItem.name.StartsWith("GEAR_Stone"))
                        {
                            stone.m_projectilename = "GEAR_Stone";
                            //Comps.DestoryStoneOnStop StoneComp = MyMod.SaveThrowingItem.gameObject.AddComponent<Comps.DestoryStoneOnStop>();
                            //StoneComp.m_Obj = MyMod.SaveThrowingItem.gameObject;
                            //StoneComp.m_RB = MyMod.SaveThrowingItem.GetComponent<Rigidbody>();
                            //StoneComp.m_Gear = MyMod.SaveThrowingItem.GetComponent<GearItem>();
                            //StoneComp.m_ShouldSendDrop = true;
                        }
                        if (MyMod.SaveThrowingItem.name.StartsWith("GEAR_NoiseMaker"))
                        {
                            stone.m_projectilename = "GEAR_NoiseMaker";
                        }
                        stone.m_skill = 0;
                        stone.m_camera_forward = GameManager.GetVpFPSCamera().transform.forward;
                        stone.m_camera_right = GameManager.GetVpFPSCamera().transform.right;
                        stone.m_camera_up = GameManager.GetVpFPSCamera().transform.up;
                        stone.m_sceneguid = MyMod.level_guid;
                        if (MyMod.sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.SHOOTSYNC))
                            {
                                _packet.Write(stone);
                                SendTCPData(_packet);
                            }
                        }
                        if (MyMod.iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.SHOOTSYNC))
                            {
                                ServerSend.SHOOTSYNC(0, stone, true);
                            }
                        }
                        MyMod.PushActionToMyDoll("Shoot");
                    }
                }
                else
                {
                    MelonLogger.Msg("[PlayerManager][PostFix] SaveThrowingItem is NULL");
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(NoiseMakerItem), "ProcessItemInHandDetonated")] // Once
        public class NoiseMakerItem_ProcessItemInHandDetonated
        {
            public static void Postfix(NoiseMakerItem __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                DataStr.ShootSync bomb = new DataStr.ShootSync();
                bomb.m_position = GameManager.GetPlayerObject().transform.position;
                bomb.m_rotation = GameManager.GetPlayerObject().transform.rotation;
                bomb.m_projectilename = "GEAR_NoiseMaker";
                bomb.m_skill = 1; //Will make it blow in hands
                bomb.m_camera_forward = GameManager.GetVpFPSCamera().transform.forward;
                bomb.m_camera_right = GameManager.GetVpFPSCamera().transform.right;
                bomb.m_camera_up = GameManager.GetVpFPSCamera().transform.up;
                bomb.m_sceneguid = MyMod.level_guid;
                MelonLogger.Msg("[ProcessItemInHandDetonated] NoiseMaker burn explotion sync");

                if (MyMod.sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.SHOOTSYNC))
                    {
                        _packet.Write(bomb);
                        SendTCPData(_packet);
                    }
                }
                if (MyMod.iAmHost == true)
                {
                    ServerSend.SHOOTSYNC(0, bomb, true);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(UIButton), "OnClick")] // Once
        public class UIButton_OnClick
        {
            public static void Postfix(UIButton __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.gameObject != null && __instance.gameObject.transform.parent != null && __instance.gameObject.transform.parent != null && __instance.hoverSprite == "genericButton_over 1")
                {
                    if (__instance.gameObject.transform.parent.name == "WaitForEveryoneButton")
                    {
                        if (MyMod.CanSleep(false) == false)
                        {
                            MyMod.CanSleep(true);
                        }
                        else
                        {
                            if (MyMod.SleepingButtons != null)
                            {
                                MyMod.SleepingButtons.SetActive(false);
                            }
                            if (MyMod.WaitForSleepLable != null)
                            {
                                MyMod.WaitForSleepLable.SetActive(true);
                            }
                        }
                    }
                    else if (__instance.gameObject.transform.parent.name == "LieDownButton")
                    {
                        if (MyMod.AtBed == false)
                        {
                            MyMod.OutOfBedPosition = GameManager.GetPlayerTransform().position;
                            if (InterfaceManager.m_Panel_Rest.m_Bed)
                            {
                                GameManager.GetPlayerManagerComponent().TeleportPlayer(InterfaceManager.m_Panel_Rest.m_Bed.m_BodyPlacementTransform.position, InterfaceManager.m_Panel_Rest.m_Bed.m_BodyPlacementTransform.rotation);
                                MyMod.AtBed = true;
                            }
                        }
                        else
                        {
                            GameManager.GetPlayerManagerComponent().TeleportPlayer(MyMod.OutOfBedPosition, GameManager.GetPlayerTransform().rotation);
                            MyMod.AtBed = false;
                        }
                        InterfaceManager.m_Panel_Rest.OnCancel();
                    }
                }
            }
        }

        public static bool NeedSkipCauseConnect()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            for (int i = 0; i < arguments.Length; i++)
            {
                //MelonLogger.Msg("Argument ["+i+"] Parameter ["+arguments[i]+"]");

                if (arguments[i] == "+connect_lobby" || arguments[i] == "connect_lobby")
                {
                    if (i + 1 <= arguments.Length - 1)
                    {
                        MelonLogger.Msg("[STEAMWORKS.NET] Skip everything to connect to: " + arguments[i + 1]);
                        return true;
                    }
                }
                if (arguments[i] == "skip" || arguments[i] == "-skip")
                {
                    if (i + 1 <= arguments.Length - 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool AutoSaveAllTheCost = false;

        [HarmonyLib.HarmonyPatch(typeof(GameManager), "LoadScene", new Type[] { typeof(string), typeof(string) })] // Once
        public class GameManager_LoadSceneOverLoad1
        {
            public static void Prefix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg(ConsoleColor.Yellow, "Loading other scene...");
                MyMod.ApplyOtherCampfires = false;
                AutoSaveAllTheCost = true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GameManager), "LoadScene", new Type[] { typeof(string) })] // Once
        public class GameManager_LoadSceneOverLoad2
        {
            public static void Prefix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg(ConsoleColor.Yellow, "Loading other scene...");
                MyMod.ApplyOtherCampfires = false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SceneManager), "OnSceneLoaded")] // Once
        public class SceneManager_Load
        {
            public static void Postfix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg(ConsoleColor.Yellow, "[SceneManager] OnSceneLoaded");
                if (MyMod.level_name == "Boot")
                {
                    if (uConsole.m_Instance == null)
                    {
                        MelonLogger.Msg("No uConsole present, creating one.");
                        UnityEngine.Object.Instantiate(Resources.Load("uConsole"));
                    }

                    string v_type = "";
                    if (GameManager.GetVersionString().Contains("S"))
                    {
                        v_type = "Steam";
                    }
                    else if (GameManager.GetVersionString().Contains("E"))
                    {
                        v_type = "EGS";
                        MyMod.LoadChatName();
                    }else{
                        v_type = "Unknown";
                        MyMod.LoadChatName();
                    }
                    if (v_type != "Unknown")
                    {
                        MelonLogger.Msg(v_type + " version");
                    }
                    else
                    {
                        MelonLogger.Msg("Unknown build of game, not using SteamWorks.NET");
                    }
                    bool HasJoin = false;
                    if (v_type == "Steam")
                    {
                        string[] arguments = Environment.GetCommandLineArgs();
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            //MelonLogger.Msg("Argument ["+i+"] Parameter ["+arguments[i]+"]");

                            if (arguments[i] == "connect_lobby" || arguments[i] == "+connect_lobby")
                            {
                                if (i + 1 <= arguments.Length - 1)
                                {
                                    MyMod.SteamLobbyToJoin = arguments[i + 1];
                                    MyMod.ConnectedSteamWorks = true;
                                    MyMod.NeedConnectAfterLoad = 3;
                                    HasJoin = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (HasJoin == false)
                    {
                        string[] arguments = Environment.GetCommandLineArgs();
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            if (arguments[i] == "cmd" || arguments[i] == "-cmd")
                            {
                                if (i + 1 <= arguments.Length - 1)
                                {
                                    string cmd = arguments[i + 1].Replace("@", " ");
                                    MyMod.AutoCMDs.Add(cmd);
                                }
                            }
                        }
                    }

                    if (MyMod.DedicatedServerAppMode)
                    {
                        MyMod.StartDSAfterLoad = 3;
                    }
                }
                if (MyMod.level_name != "Empty" && MyMod.level_name != "Boot" && MyMod.level_name != "MainMenu")
                {
                    MelonLogger.Msg("Loading scene finished " + MyMod.level_name + " health is: " + GameManager.GetConditionComponent().m_CurrentHP);


                    MyMod.NotNeedToPauseUntilLoaded = false;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BreakDown), "DoBreakDown")] // Once
        private static class DoBreakDown_Sync
        {
            internal static void Postfix(BreakDown __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                string breakGuid = "";
                string breakParentGuid = "";
                if (__instance.gameObject != null)
                {
                    ObjectGuid BreakGuidComp = __instance.gameObject.GetComponent<ObjectGuid>();
                    if (BreakGuidComp != null)
                    {
                        breakGuid = BreakGuidComp.Get();
                    }
                    if (__instance.gameObject.transform.parent != null)
                    {
                        ObjectGuid BreakGuidParentComp = __instance.gameObject.transform.parent.GetComponent<ObjectGuid>();
                        if (BreakGuidParentComp != null)
                        {
                            breakParentGuid = BreakGuidParentComp.Get();
                        }
                    }
                }
                MyMod.BakePreSpawnedGearsList();
                if (breakGuid == "" && breakParentGuid == "")
                {
                    MelonLogger.Msg("Deny listing that furn, cause have not any GUIDs, so it should be duppable");
                    return;
                }
                MyMod.SendBrokeFurniture(breakGuid, breakParentGuid, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(LoadScene), "Start")] // Once
        private static class LoadScene_SeededGUIDHook
        {
            internal static bool Prefix(LoadScene __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                //MelonLogger.Msg("[LoadScene] SceneCanBeInstanced " + __instance.m_SceneCanBeInstanced + " ExitPoint " + __instance.m_ExitPointName + " SceneToLoad " + __instance.m_SceneToLoad);
                if (!__instance.m_Active || __instance.m_StartHasBeenCalled)
                    return false;
                __instance.m_StartHasBeenCalled = true;
                __instance.m_FadeOutStarted = false;
                if (__instance.m_LoadSceneParent != null)
                {
                    __instance.m_LoadSceneParent.Start();
                    __instance.m_GUID = __instance.m_LoadSceneParent.m_GUID;
                }
                else
                {
                    //MelonLogger.Msg("[LoadScene] Have not LoadSceneParent generating custom seeded GUID");
                    __instance.m_GUID = MyMod.GenerateSeededGUID(GameManager.m_SceneTransitionData.m_GameRandomSeed, __instance.gameObject.transform.position);

                    //MelonLogger.Msg("[LoadScene] Got new m_GUID " + __instance.m_GUID);
                    //__instance.gameObject.name = "InteriorLoadTrigger" + __instance.m_GUID;
                }
                if (__instance.m_TransitionOnContact)
                {
                    __instance.GetComponent<Collider>().isTrigger = true;
                    vp_Layer.Set(__instance.gameObject, 21);
                }
                else if (__instance.gameObject.layer != 21)
                {
                    vp_Layer.Set(__instance.gameObject, 19);
                }
                if (__instance.m_LoadSceneParent != null)
                {
                    __instance.m_LoadSceneParent.Register(__instance);
                }
                __instance.m_Lock = __instance.gameObject.GetComponent<Lock>();
                if (!__instance.m_Lock)
                    return false;
                __instance.m_Lock.RollLockedState();
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(LoadSceneParent), "Start")] // Once
        private static class LoadSceneParent_SeededGUIDHook
        {
            internal static bool Prefix(LoadSceneParent __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                //MelonLogger.Msg("[LoadSceneParent] Start");
                if (__instance.m_StartHasBeenCalled)
                    return false;
                __instance.m_StartHasBeenCalled = true;
                //MelonLogger.Msg("[LoadSceneParent] Generating custom seeded GUID");
                //__instance.m_GUID = Utils.GetGuid();
                __instance.m_GUID = MyMod.GenerateSeededGUID(GameManager.m_SceneTransitionData.m_GameRandomSeed, __instance.gameObject.transform.position);
                //MelonLogger.Msg("[LoadSceneParent] Got new m_GUID " + __instance.m_GUID);
                return false;
            }
        }
        public static void SaveDeathCreates(SaveSlotType gameMode, string name)
        {
            //MelonLogger.Msg("[Saving][SaveDeathCreates] Saving...");
            DataStr.DeathContainerData[] saveProxy = MyMod.DeathCreates.ToArray();
            string data = JSON.Dump(saveProxy);
            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_DeathCreates", data);
            if (ok == true)
            {
                //MelonLogger.Msg("[Saving][SaveDeathCreates] Successfully!");
            }else{
                //MelonLogger.Msg("[Saving][SaveDeathCreates] Fail!");
            }
        }
        public static void SaveUGUID(SaveSlotType gameMode, string name)
        {
            string[] saveProxy = { MyMod.MyUGUID };
            string data = JSON.Dump(saveProxy);
            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "UserGUID", data);
        }
        public static void SaveDeployedRopes(SaveSlotType gameMode, string name)
        {
            //MelonLogger.Msg("[Saving][ClimbingRopeSync] Saving...");
            DataStr.ClimbingRopeSync[] saveProxy = MyMod.DeployedRopes.ToArray();
            string data = JSON.Dump(saveProxy);
            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_ropes", data);
            if (ok == true)
            {
                //MelonLogger.Msg("[Saving][ClimbingRopeSync] Successfully!");
            }
            else
            {
                //MelonLogger.Msg("[Saving][ClimbingRopeSync] Fail!");
            }
        }

        public static void SaveServerConfig(SaveSlotType gameMode, string name)
        {
            //MelonLogger.Msg("[Saving][ServerConfig] Saving...");
            if (MyMod.sendMyPosition == true)
            {
                //MelonLogger.Msg("[Saving][ServerConfig] You on server being client, not need save this.");
                return;
            }
            string data = JSON.Dump(MyMod.ServerConfig);
            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_cfg", data);
            if (ok == true)
            {
                //MelonLogger.Msg("[Saving][ServerConfig] Successfully!");
            }
            else
            {
                //MelonLogger.Msg("[Saving][ServerConfig] Fail!");
            }
        }

        public static void SaveSnowShelters(SaveSlotType gameMode, string name)
        {
            //MelonLogger.Msg("[Saving][SnowShelters] Saving...");
            DataStr.ShowShelterByOther[] saveProxy = MyMod.ShowSheltersBuilded.ToArray();
            string data = JSON.Dump(saveProxy);
            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_shelters", data);
            if (ok == true)
            {
                //MelonLogger.Msg("[Saving][SnowShelters] Successfully!");
            }
            else
            {
                //MelonLogger.Msg("[Saving][SnowShelters] Fail!");
            }
        }

        public static void SaveGenVersion(SaveSlotType gameMode, string name)
        {
            int[] saveProxy = { MyMod.BuildInfo.RandomGenVersion };
            string data = JSON.Dump(saveProxy);

            if (MyMod.CantBeUsedForMP == false)
            {
                bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_genversion", data);
                if (ok == true)
                {
                    MyMod.LastLoadedGenVersion = MyMod.BuildInfo.RandomGenVersion;
                }
            }
        }
        public static void SaveSeedRadioFQ(SaveSlotType gameMode, string name)
        {
            float[] saveProxy = { MyMod.RadioFrequency };
            string data = JSON.Dump(saveProxy);

            SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_FQ", data);
        }

        public static void SaveRealtimeTime(SaveSlotType gameMode, string name)
        {
            int[] saveProxy = { MyMod.MinutesFromStartServer };
            string data = JSON.Dump(saveProxy);

            SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_rtt", data);
        }
        public static void SaveFixedSpawn(SaveSlotType gameMode, string name)
        {
            string[] saveProxy = { MyMod.SavedSceneForSpawn };
            string data = JSON.Dump(saveProxy);

            SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_fixedS", data);
        }
        public static void SaveFixedSpawnPosition(SaveSlotType gameMode, string name)
        {
            Vector3[] saveProxy = { MyMod.SavedPositionForSpawn };
            string data = JSON.Dump(saveProxy);

            SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_fixedP", data);
        }
        public static void SaveBookReaded(SaveSlotType gameMode, string name)
        {
            //MelonLogger.Msg("[Saving][BooksResearched] Saving...");
            string data = JSON.Dump(MyMod.BooksResearched);
            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_books", data);
            if (ok == true)
            {
                //MelonLogger.Msg("[Saving][BooksResearched] Successfully!");
            }
            else
            {
                //MelonLogger.Msg("[Saving][BooksResearched] Fail!");
            }
        }
        public static void SaveCustomSkills(SaveSlotType gameMode, string name)
        {
            string data = JSON.Dump(MyMod.SaveCustomSkills());
            SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_customskills", data);
        }


        [HarmonyLib.HarmonyPatch(typeof(SaveGameSystem), "SaveGlobalData")] // Once
        public static class SaveGameSystemPatch_SaveSceneData
        {
            public static void Postfix(SaveSlotType gameMode, string name)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                SaveDeployedRopes(gameMode, name);
                SaveSnowShelters(gameMode, name);
                SaveGenVersion(gameMode, name);
                //SaveSeedInt(gameMode, name);
                SaveRealtimeTime(gameMode, name);

                if (MyMod.ServerConfig.m_PlayersSpawnType != 3 && MyMod.iAmHost) // Fixed spawn only
                {
                    SaveFixedSpawn(gameMode, name);
                    SaveFixedSpawnPosition(gameMode, name);
                }
                SaveBookReaded(gameMode, name);
                SaveSeedRadioFQ(gameMode, name);
                SaveDeathCreates(gameMode, name);
                SaveCustomSkills(gameMode, name);
                SaveUGUID(gameMode, name);
            }
        }

        public static void LoadDeathCreates(string name)
        {
            //MelonLogger.Msg("[Saving][DeathCreates] Loading...");
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_DeathCreates");
            if (data != null)
            {
                DataStr.DeathContainerData[] saveProxy = JSON.Load(data).Make<DataStr.DeathContainerData[]>();
                List<DataStr.DeathContainerData> loadedData = saveProxy.ToList<DataStr.DeathContainerData>();

                MyMod.DeathCreates = loadedData;

                //for (int i = 0; i < loadedData.Count; i++)
                //{
                //    MyMod.DeathContainerData ToAdd = loadedData[i];
                //    if (!MyMod.DeathCreates.Contains(ToAdd))
                //    {
                //        MyMod.DeathCreates.Add(ToAdd);
                //    }
                //}
            }
        }
        public static void LoadUGUID(string name)
        {
            string data = SaveGameSlots.LoadDataFromSlot(name, "UserGUID");
            if (data != null)
            {
                string[] saveProxy = JSON.Load(data).Make<string[]>();
                MyMod.MyUGUID = saveProxy[0];
            }
        }
        public static void LoadDeployedRopes(string name)
        {
            //MelonLogger.Msg("[Saving][ClimbingRopeSync] Loading...");
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_ropes");
            if (data != null)
            {
                DataStr.ClimbingRopeSync[] saveProxy = JSON.Load(data).Make<DataStr.ClimbingRopeSync[]>();
                List<DataStr.ClimbingRopeSync> loadedData = saveProxy.ToList<DataStr.ClimbingRopeSync>();

                for (int i = 0; i < loadedData.Count; i++)
                {
                    DataStr.ClimbingRopeSync ToAdd = loadedData[i];
                    if (MyMod.DeployedRopes.Contains(ToAdd) == false)
                    {
                        MyMod.DeployedRopes.Add(ToAdd);
                    }
                }
                //MelonLogger.Msg("[Saving][ClimbingRopeSync] Loaded Entries: " + loadedData.Count);
                //MelonLogger.Msg("[Saving][ClimbingRopeSync] Total Entries: " + MyMod.DeployedRopes.Count);
            }
            else
            {
                //MelonLogger.Msg("[Saving][ClimbingRopeSync] No saves found!");
            }
        }

        public static void LoadSnowShelters(string name)
        {
            //MelonLogger.Msg("[Saving][SnowShelters] Loading...");
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_shelters");
            if (data != null)
            {
                DataStr.ShowShelterByOther[] saveProxy = JSON.Load(data).Make<DataStr.ShowShelterByOther[]>();
                List<DataStr.ShowShelterByOther> loadedData = saveProxy.ToList<DataStr.ShowShelterByOther>();

                for (int i = 0; i < loadedData.Count; i++)
                {
                    DataStr.ShowShelterByOther ToAdd = loadedData[i];
                    if (MyMod.ShowSheltersBuilded.Contains(ToAdd) == false)
                    {
                        MyMod.ShowSheltersBuilded.Add(ToAdd);
                    }
                }
                //MelonLogger.Msg("[Saving][SnowShelters] Loaded Entries: " + loadedData.Count);
                //MelonLogger.Msg("[Saving][SnowShelters] Total Entries: " + MyMod.ShowSheltersBuilded.Count);
            }
            else
            {
                //MelonLogger.Msg("[Saving][SnowShelters] No saves found!");
            }
        }

        public static void LoadServerConfig(string name)
        {
            //MelonLogger.Msg("[Saving][ServerConfig] Loading...");

            if (MyMod.sendMyPosition == true)
            {
                //MelonLogger.Msg("[Saving][ServerConfig] You on server being client, not need load this.");
                return;
            }

            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_cfg");
            if (data != null)
            {
                DataStr.ServerConfigData saveProxy = JSON.Load(data).Make<DataStr.ServerConfigData>();
                MyMod.ServerConfig = saveProxy;
                //MelonLogger.Msg("[Saving][ServerConfig] m_FastConsumption: " + saveProxy.m_FastConsumption);
                //MelonLogger.Msg("[Saving][ServerConfig] m_DuppedSpawns: " + saveProxy.m_DuppedSpawns);
            }
            else
            {
                //MelonLogger.Msg("[Saving][ServerConfig] No saves found!");
            }
        }
        public static void LoadGenVersion(string name)
        {
            //MelonLogger.Msg("[Saving][MultiplayerDeath] Loading...");
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_genversion");
            if (data != null)
            {
                int[] saveProxy = JSON.Load(data).Make<int[]>();
                MyMod.LastLoadedGenVersion = saveProxy[0];
                if (MyMod.LastLoadedGenVersion != MyMod.BuildInfo.RandomGenVersion)
                {
                    MyMod.CantBeUsedForMP = true;
                    MelonLogger.Msg(ConsoleColor.DarkRed, "This save file can't be use for multiplayer, because we created on old version of the mod, with Generation version " + MyMod.LastLoadedGenVersion + ". Release of mod you using right now has Generation version " + MyMod.BuildInfo.RandomGenVersion);
                }
            }
            else
            {
                MyMod.LastLoadedGenVersion = 0;
                MyMod.CantBeUsedForMP = true;
                MelonLogger.Msg(ConsoleColor.DarkRed, "This save file can't be use for multiplayer, because was created on old version of mod or without mod at all.");
            }
        }
        public static float LoadRadioFQ(string name)
        {
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_FQ");
            if (data != null)
            {
                float[] saveProxy = JSON.Load(data).Make<float[]>();
                return saveProxy[0];
            }else{
                return UnityEngine.Random.Range(-25,25);
            }
        }

        public static void LoadRealtimeTime(string name)
        {
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_rtt");
            if (data != null)
            {
                int[] saveProxy = JSON.Load(data).Make<int[]>();
                MyMod.MinutesFromStartServer = saveProxy[0];
            }
        }

        public static void LoadFixedSpawn(string name)
        {
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_fixedS");
            if (data != null)
            {
                string[] saveProxy = JSON.Load(data).Make<string[]>();
                MyMod.SavedSceneForSpawn = saveProxy[0];
                MyMod.FixedPlaceLoaded = true;
            }
        }
        public static void LoadFixedSpawnPosition(string name)
        {
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_fixedP");
            if (data != null)
            {
                Vector3[] saveProxy = JSON.Load(data).Make<Vector3[]>();
                MyMod.SavedPositionForSpawn = saveProxy[0];
                MyMod.FixedPlaceLoaded = true;
            }
        }
        public static void LoadReadedBooks(string name)
        {
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_books");
            if (data != null)
            {
                Dictionary<string, float> loadedData = JSON.Load(data).Make<Dictionary<string, float>>();
                int loadedCount = 0;
                int overrideCount = 0;

                foreach (var item in loadedData)
                {
                    if (!MyMod.BooksResearched.ContainsKey(item.Key))
                    {
                        MyMod.BooksResearched.Add(item.Key, item.Value);
                        loadedCount++;
                    }else{
                        MyMod.BooksResearched.Remove(item.Key);
                        MyMod.BooksResearched.Add(item.Key, item.Value);
                        overrideCount++;
                    }
                }
            }
        }

        public static void LoadCustomSkills(string name)
        {
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_customskills");
            if (data != null)
            {
                MyMod.LoadCustomSkills(JSON.Load(data).Make<List<int>>());
            }
        }

        public static void clearFolder(string FolderName)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(FolderName);

            foreach (System.IO.FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (System.IO.DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SaveGameSystem), "RestoreGlobalData")]  // Once
        public static class SaveGameSystemPatch_RestoreGlobalData
        {
            public static void Postfix(string name)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                MelonLogger.Msg(ConsoleColor.Yellow, "[Saving] Loading " + name + "...");
                LoadDeployedRopes(name);
                LoadSnowShelters(name);
                LoadGenVersion(name);
                LoadRealtimeTime(name);
                LoadFixedSpawn(name);
                LoadFixedSpawnPosition(name);
                LoadReadedBooks(name);
                LoadCustomSkills(name);
                float FQ = LoadRadioFQ(name);
                MyMod.RadioFrequency = Mathf.Round(FQ * 10.0f) * 0.1f;
                LoadDeathCreates(name);
                LoadUGUID(name);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GameManager), "LoadMainMenu")] // Once
        public static class GameManager_BackToMenu
        {
            public static void Prefix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                //FlushAllSavable();
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_BreakDown), "OnBreakDown")] // Once
        public static class Panel_BreakDown_AudioStart
        {
            public static void Postfix(Panel_BreakDown __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_BreakDown != null)
                {
                    GameObject gameObject = __instance.m_BreakDown.gameObject;
                    string breakGuid = "";
                    string breakParentGuid = "";
                    if (gameObject != null)
                    {
                        ObjectGuid BreakGuidComp = gameObject.GetComponent<ObjectGuid>();
                        if (BreakGuidComp != null)
                        {
                            breakGuid = BreakGuidComp.Get();
                        }
                        if (gameObject.transform.parent != null)
                        {
                            ObjectGuid BreakGuidParentComp = gameObject.transform.parent.GetComponent<ObjectGuid>();
                            if (BreakGuidParentComp != null)
                            {
                                breakParentGuid = BreakGuidParentComp.Get();
                            }
                        }
                    }

                    DataStr.BrokenFurnitureSync furn = new DataStr.BrokenFurnitureSync();
                    furn.m_Guid = breakGuid;
                    furn.m_ParentGuid = breakParentGuid;
                    furn.m_LevelID = MyMod.levelid;
                    furn.m_LevelGUID = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;

                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.FURNBREAKINGGUID))
                        {
                            _packet.Write(furn);
                            SendTCPData(_packet);
                        }
                    }

                    if (MyMod.iAmHost == true)
                    {
                        ServerSend.FURNBREAKINGGUID(0, furn, true);
                    }

                    MelonLogger.Msg("Starting braking object " + furn.m_Guid + " audio");
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_BreakDown), "StopAudioAndRumbleEffects")] // Once
        public static class Panel_BreakDown_AudioStop
        {
            public static void Postfix(Panel_BreakDown __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.FURNBREAKINSTOP))
                    {
                        _packet.Write(true);
                        SendTCPData(_packet);
                    }
                }

                if (MyMod.iAmHost == true)
                {
                    ServerSend.FURNBREAKINSTOP(0, true, true);
                }

                MelonLogger.Msg("Stop braking object");
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BreakDown), "ProcessInteraction")] // Once
        public static class BreakDown_ProcessInteraction
        {
            public static bool Prefix(BreakDown __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                GameObject gameObject = __instance.gameObject;
                string breakGuid = "";
                string breakParentGuid = "";
                if (gameObject != null)
                {
                    ObjectGuid BreakGuidComp = gameObject.GetComponent<ObjectGuid>();
                    if (BreakGuidComp != null)
                    {
                        breakGuid = BreakGuidComp.Get();
                    }
                    if (gameObject.transform.parent != null)
                    {
                        ObjectGuid BreakGuidParentComp = gameObject.transform.parent.GetComponent<ObjectGuid>();
                        if (BreakGuidParentComp != null)
                        {
                            breakParentGuid = BreakGuidParentComp.Get();
                        }
                    }
                }

                DataStr.BrokenFurnitureSync furn = new DataStr.BrokenFurnitureSync();
                furn.m_Guid = breakGuid;
                furn.m_ParentGuid = breakParentGuid;
                furn.m_LevelID = MyMod.levelid;
                furn.m_LevelGUID = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;

                for (int i = 0; i < MyMod.playersData.Count; i++)
                {
                    if (MyMod.playersData[i] != null)
                    {
                        DataStr.BrokenFurnitureSync otherFurn = MyMod.playersData[i].m_BrakingObject;
                        if (otherFurn.m_Guid == furn.m_Guid && otherFurn.m_ParentGuid == furn.m_ParentGuid && otherFurn.m_LevelID == MyMod.levelid && otherFurn.m_LevelGUID == MyMod.level_guid)
                        {
                            HUDMessage.AddMessage(MyMod.playersData[i].m_Name + " IS BREAKING THIS");
                            GameAudioManager.PlayGUIError();
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ProcessContainerInteraction")] // Once
        public static class PlayerManager_ProcessContainerInteraction
        {
            public static bool Prefix(PlayerManager __instance, Container c)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                string boxGUID = "";

                if (c != null && c.gameObject != null && c.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    boxGUID = c.gameObject.GetComponent<ObjectGuid>().Get();
                }

                DataStr.ContainerOpenSync box = new DataStr.ContainerOpenSync();
                box.m_Guid = boxGUID;
                box.m_LevelID = MyMod.levelid;
                box.m_LevelGUID = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;

                for (int i = 0; i < MyMod.playersData.Count; i++)
                {
                    if (MyMod.playersData[i] != null)
                    {
                        if (MyMod.playersData[i].m_Container != null)
                        {
                            DataStr.ContainerOpenSync otherBox = MyMod.playersData[i].m_Container;
                            if (otherBox.m_Guid == box.m_Guid && otherBox.m_LevelID == MyMod.levelid && otherBox.m_LevelGUID == MyMod.level_guid)
                            {
                                HUDMessage.AddMessage(MyMod.playersData[i].m_Name + " IS USING THIS");
                                GameAudioManager.PlayGUIError();
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(RopeAnchorPoint), "Start")] // Once
        public static class RopeAnchorPoint_Hook
        {
            public static void Postfix(RopeAnchorPoint __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.IsDead == true || GameManager.GetPlayerStruggleComponent().InStruggle() == true)
                {
                    return;
                }
                if (__instance.m_StartHasBeenCalled)
                    return;
                MelonLogger.Msg("Rope Start");
                Shared.AddDeployedRopes(__instance.gameObject.transform.position, __instance.m_RopeDeployed, __instance.m_RopeSnapped, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent, true);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(RopeAnchorPoint), "SnapRope")] // REALLY WEIRD ONE!!!
        public static class RopeAnchorPoint_Snapped
        {
            public static void Postfix(RopeAnchorPoint __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.IsDead == true || GameManager.GetPlayerStruggleComponent().InStruggle() == true)
                {
                    return;
                }
                MelonLogger.Msg("Rope Snapped");
                Shared.AddDeployedRopes(__instance.gameObject.transform.position, __instance.m_RopeDeployed, __instance.m_RopeSnapped, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent, true);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(RopeAnchorPoint), "ActionFinished")] // REALLY WEIRD ONE!!!
        public static class RopeAnchorPoint_TakeOrDeploy
        {
            public static void Prefix(RopeAnchorPoint __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.IsDead == true || GameManager.GetPlayerStruggleComponent().InStruggle() == true)
                {
                    return;
                }
                if (__instance.m_ActionState == RopeAnchorActionState.None)
                {
                    return;
                }
                if (GameManager.m_PlayerManager != null)
                {
                    PlayerControlMode Plc = GameManager.GetPlayerManagerComponent().GetControlMode();
                    if (Plc == PlayerControlMode.DeployRope || Plc == PlayerControlMode.TakeRope)
                    {
                        bool deployed = false;
                        if (Plc == PlayerControlMode.DeployRope)
                        {
                            MelonLogger.Msg("Rope Deployed");
                            deployed = true;
                        }
                        else
                        {
                            MelonLogger.Msg("Rope Taken");
                            deployed = false;
                        }
                        Shared.AddDeployedRopes(__instance.gameObject.transform.position, deployed, __instance.m_RopeSnapped, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent, true);
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(RopeAnchorPoint), "Update")]
        public static class RopeAnchorPoint_Update
        {
            public static void Prefix(RopeAnchorPoint __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_RopeDeployed)
                {
                    __instance.enabled = false;
                }
            }
        }

        //MAKING RANDOM GEARS SEEDED!


        [HarmonyLib.HarmonyPatch(typeof(GameManager), "RollSpawnChance", new System.Type[] { typeof(GameObject), typeof(float) })] // Once, can be triggered a lot
        public static class GameManager_SeededRandom
        {
            public static bool Prefix(GameObject go, float spawnChance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                return false;
            }
            public static void Postfix(GameObject go, float spawnChance, ref bool __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                int _x = (int)go.transform.position.x;
                int _y = (int)go.transform.position.y;
                int _z = (int)go.transform.position.z;
                //int seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + _x + _y + _z;
                int seed = GameManager.GetRandomSeed(_x + _y + _z);
                spawnChance = Mathf.Clamp(spawnChance, 0.0f, 100f);

                System.Random RNG = new System.Random(seed);

                int num = MyMod.RollChanceSeeded(spawnChance, RNG) ? 1 : 0;
                if (num == 0)
                    go.SetActive(false);
                __result = num != 0;
                //MelonLogger.Msg("[RollSpawnChance Seeded] Gear " + go.name + " Chance " + spawnChance + "% " + " Success " + __result + " Position: X " + _x + " Y " + _y + " Z " + _z);
            }
        }

        public static int EmuGenVersion = -1;
        public static bool EmulateOldRandom = false;

        [HarmonyLib.HarmonyPatch(typeof(GameManager), "GetRandomSeed")] // Once, can be triggered a lot.
        public static class GameManager_GetRandomSeed
        {
            public static bool Prefix(int seed)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                return false;
            }
            public static void Postfix(int seed, ref int __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                //MelonLogger.Msg("Getting random seed for "+ GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent);
                //GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent

                if (EmulateOldRandom)
                {
                    if (EmuGenVersion == 1)
                    {
                        __result = seed ^ GameManager.m_SceneTransitionData.m_GameRandomSeed;
                    }
                }
                else
                {
                    string UseString = "";

                    if (GameManager.m_SceneTransitionData.m_SceneSaveFilenameNextLoad != null && (GameManager.m_SceneTransitionData.m_SceneSaveFilenameNextLoad != "" || GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent != ""))
                    {
                        if (GameManager.m_SceneTransitionData.m_SceneSaveFilenameNextLoad != "")
                        {
                            if (GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent != GameManager.m_SceneTransitionData.m_SceneSaveFilenameNextLoad)
                            {
                                UseString = GameManager.m_SceneTransitionData.m_SceneSaveFilenameNextLoad;
                            }
                            else if (GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent == GameManager.m_SceneTransitionData.m_SceneSaveFilenameNextLoad)
                            {
                                UseString = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;
                            }
                        }
                        else
                        {
                            UseString = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;
                        }
                    }
                    else
                    {
                        if (GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent != null)
                        {
                            UseString = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;
                        }
                        else
                        {

                            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene() != null)
                            {
                                UseString = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                            }
                            else
                            {
                                UseString = "SomethingWrong!!!";
                            }
                        }
                    }
                    //MelonLogger.Msg("RANDOMFORLOAD        "+ UseString);
                    int DuppableSceneHash = UseString.GetHashCode();
                    __result = seed + GameManager.m_SceneTransitionData.m_GameRandomSeed + DuppableSceneHash;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(RandomSpawnObject), "ActivateRandomObject")] // Once
        public static class RandomSpawnObject_SeededFix
        {
            public static bool Prefix(RandomSpawnObject __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                return false;
            }
            public static void Postfix(RandomSpawnObject __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                //MelonLogger.Msg("[RandomSpawnObject] ActivateRandomObject started for "+__instance.gameObject.name+" Position X " + __instance.gameObject.transform.position.x + "Y " + __instance.gameObject.transform.position.y + " Z " + __instance.gameObject.transform.position.z);
                List<GameObject> gameObjectList = new List<GameObject>((IEnumerable<GameObject>)__instance.m_ObjectList);
                List<int> intList = new List<int>((IEnumerable<int>)__instance.m_Weights);
                float hoursPlayedNotPaused = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                UnityEngine.Random.State state = UnityEngine.Random.state;

                UnityEngine.Random.InitState(GameManager.GetRandomSeed(__instance.transform.position.GetHashCode()));
                int enableCurrentXpMode = __instance.GetNumObjectsToEnableCurrentXPMode();
                for (int index1 = 0; index1 < enableCurrentXpMode; ++index1)
                {
                    int num1 = 0;
                    foreach (int num2 in intList)
                    {
                        num1 += num2;
                    }
                    if (num1 == 0)
                    {
                        UnityEngine.Random.state = state;
                        return;
                    }
                    List<MapDetail> mapDetailList = new List<MapDetail>();
                    using (List<GameObject>.Enumerator enumerator = gameObjectList.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            GameObject current = enumerator.Current;
                            if (current)
                            {
                                MapDetail[] componentsInChildren = (MapDetail[])current.GetComponentsInChildren<MapDetail>();
                                mapDetailList.AddRange((IEnumerable<MapDetail>)componentsInChildren);
                            }
                        }
                    }
                    int num3 = UnityEngine.Random.Range(0, num1);
                    int num4 = 0;
                    int index2 = -1;
                    for (int index3 = 0; index3 < intList.Count; ++index3)
                    {
                        int num2 = num4 + intList[index3];
                        if (num3 >= num4 && num3 < num2)
                        {
                            if (gameObjectList[index3])
                            {
                                //MelonLogger.Msg("[RandomSpawnObject] Spawns " + gameObjectList[index3].name+" on "+ gameObjectList[index3].transform.position.x+" " + gameObjectList[index3].transform.position.y + " " + gameObjectList[index3].transform.position.z);
                                gameObjectList[index3].SetActive(true);
                                foreach (MapDetail componentsInChild in (MapDetail[])gameObjectList[index3].GetComponentsInChildren<MapDetail>())
                                    mapDetailList.Remove(componentsInChild);
                                __instance.RecheckDisableObjectForXPMode(gameObjectList[index3]);
                            }
                            index2 = index3;
                            break;
                        }
                        num4 = num2;
                    }
                    if (index2 >= 0)
                    {
                        intList.RemoveAt(index2);
                        gameObjectList.RemoveAt(index2);
                    }
                    else
                    {
                        //Debug.LogError((object)("RandomSpawnObject did not activate an object:" + ((Object)((Component)this).get_gameObject()).get_name()));
                    }
                }
                UnityEngine.Random.state = state;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PrefabSpawn), "SpawnObjects")] // Once
        private static class SpawnObjectCrap
        {
            private static bool Prefix(PrefabSpawn __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                return false;
            }
            private static void Postfix(PrefabSpawn __instance, ref GameObject __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_PrefabList == null)
                {
                    __result = (GameObject)null;
                    return;
                }

                int CallCounter = 0;
                int GuidHash = 0;

                if (__instance.gameObject.GetComponent<ObjectGuid>() != null && __instance.gameObject.GetComponent<ObjectGuid>().Get() != "")
                {
                    GuidHash = __instance.gameObject.GetComponent<ObjectGuid>().Get().GetHashCode();
                }
                else
                {
                    GuidHash = __instance.gameObject.transform.position.GetHashCode();
                }

                int num1 = 0;
                for (int index = 0; index < __instance.m_PrefabList.Length; ++index)
                {
                    num1 = Mathf.Max(num1, __instance.m_PrefabList[index].m_SetId);
                }

                List<List<PrefabSpawn.Element>> elementListList = new List<List<PrefabSpawn.Element>>();
                for (int index = 0; index <= num1; ++index)
                    elementListList.Add(new List<PrefabSpawn.Element>());
                for (int index = 0; index < __instance.m_PrefabList.Length; ++index)
                {
                    PrefabSpawn.Element prefab = __instance.m_PrefabList[index];
                    elementListList[prefab.m_SetId].Add(prefab);
                }
                int num2 = 0;
                List<int> intList = new List<int>();
                for (int index1 = 0; index1 < elementListList.Count; ++index1)
                {
                    intList.Add(0);
                    int num3 = 0;
                    for (int index2 = 0; index2 < elementListList[index1].Count; ++index2)
                        num3 += elementListList[index1][index2].m_SpawnWeight;
                    intList[index1] = num3;
                    num2 += num3;
                }

                CallCounter = CallCounter + 1;

                //int seedminMax = GameManager.m_SceneTransitionData.m_GameRandomSeed + GuidHash + CallCounter;
                int seedminMax = GameManager.GetRandomSeed(GuidHash + CallCounter);
                System.Random RNGminMax = new System.Random(seedminMax);
                int num4 = RNGminMax.Next(__instance.m_NumToSpawnMin, __instance.m_NumToSpawnMax + 1);
                int num5 = Mathf.Min(num1 == 0 ? __instance.m_PrefabList.Length : elementListList.Count, num4);
                int num6 = num5;
                if (__instance.m_NumToSpawnMin != 1 || __instance.m_NumToSpawnMax != 1)
                    num6 = Mathf.Max(0, num6 - GameManager.GetExperienceModeManagerComponent().GetReduceMaxItemsInContainer());
                float percent = (float)__instance.m_ChanceOfNoSpawn;
                if (num6 == 0 && num5 != 0)
                {
                    num6 = num5;
                    percent = Mathf.Min(75f, percent);
                }
                else if ((double)percent > 0.0)
                {
                    percent = (float)(100.0 - (100.0 - (double)percent) * (double)GameManager.GetExperienceModeManagerComponent().GetGearSpawnChanceScale());
                }

                //int seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + GuidHash;
                int seed = GameManager.GetRandomSeed(GuidHash);
                System.Random RNG = new System.Random(seed);

                if (MyMod.RollChanceSeeded(percent, RNG))
                {
                    __result = (GameObject)null;
                    return;
                }
                List<GameObject> gameObjectList = new List<GameObject>();
                for (; num6 > 0; --num6)
                {
                    CallCounter = CallCounter + 1;
                    //int seedNum3 = GameManager.m_SceneTransitionData.m_GameRandomSeed + GuidHash + CallCounter;
                    int seedNum3 = GameManager.GetRandomSeed(GuidHash + CallCounter);
                    System.Random RNGnum3 = new System.Random(seedNum3);
                    int num3 = RNGnum3.Next(0, num2);
                    int num7 = 0;
                    for (int index1 = 0; index1 < elementListList.Count; ++index1)
                    {
                        int num8 = num7 + intList[index1];
                        if (num3 < num8)
                        {
                            int num9 = num7;
                            for (int index2 = 0; index2 < elementListList[index1].Count; ++index2)
                            {
                                PrefabSpawn.Element spawnElement = elementListList[index1][index2];
                                int num10 = num9 + spawnElement.m_SpawnWeight;
                                if (num3 < num10)
                                {
                                    GameObject gameObject = __instance.SpawnObject(spawnElement);
                                    if (gameObject != null)
                                    {
                                        gameObjectList.Add(gameObject);
                                    }
                                    if (elementListList.Count == 1)
                                    {
                                        num2 -= spawnElement.m_SpawnWeight;
                                        elementListList[index1].RemoveAt(index2);
                                        break;
                                    }
                                    break;
                                }
                                num9 = num10;
                            }
                            if (elementListList.Count > 1)
                            {
                                elementListList.RemoveAt(index1);
                                num2 -= intList[index1];
                                intList.RemoveAt(index1);
                                break;
                            }
                            break;
                        }
                        num7 = num8;
                    }
                }
                __result = (GameObject)null;
                return;
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(PrefabSpawn), "SpawnObject")] //SpawnObject and SpawnObjects is different methods, S you see? Nice names (no).
        //private static class SpawnObjectCrapLogs
        //{
        //    private static void Postfix(PrefabSpawn __instance, PrefabSpawn.Element spawnElement, ref GameObject __result)
        //    {
        //        if(__result != null)
        //        {
        //            //MelonLogger.Msg("[PrefabSpawn Seeded] Spawned " + __result.gameObject.name+" X "+__result.gameObject.transform.position.x + " Y " + __result.gameObject.transform.position.y + " Z " + __result.gameObject.transform.position.z);
        //        }
        //    }
        //}

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "DrinkFromWaterSupply")] //Part of code from TLD_RelativeConsumptionTime
        internal class PlayerManager_DrinkFromWaterSupply_Patch
        {
            internal static float restoreTimeToDrink; //Thank you, Remodor, now I know I can have varible right in patch class!
            internal static void Prefix(PlayerManager __instance, WaterSupply ws, float volumeAvailable)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                restoreTimeToDrink = ws.m_TimeToDrinkSeconds;
                if (MyMod.ServerConfig.m_FastConsumption == false)
                {
                    ws.m_TimeToDrinkSeconds = 10;
                }
            }
            internal static void Postfix(PlayerManager __instance, WaterSupply ws)
            {
                ws.m_TimeToDrinkSeconds = restoreTimeToDrink;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "UseFoodInventoryItem")]//Part of code from TLD_RelativeConsumptionTime
        internal class PlayerManager_UseFoodInventoryItem_Patch
        {
            internal static float restoreTimeToEat;
            internal static void Prefix(PlayerManager __instance, GearItem gi)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                restoreTimeToEat = gi.m_FoodItem.m_TimeToEatSeconds;
                if (MyMod.ServerConfig.m_FastConsumption == false)
                {
                    gi.m_FoodItem.m_TimeToEatSeconds = 11;
                }
            }
            internal static void Postfix(PlayerManager __instance, GearItem gi)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                gi.m_FoodItem.m_TimeToEatSeconds = restoreTimeToEat;
            }
        }

        public static string DropTimeFormat(int minutesLeft)
        {
            int hours = minutesLeft / 60;
            if (hours >= 24)
            {
                int days = hours / 24;
                if (days != 1)
                {
                    return days + " days";
                } else
                {
                    return days + " day";
                }
            } else
            {
                if (hours >= 1)
                {
                    if (hours != 1)
                    {
                        return hours + " hours";
                    } else
                    {
                        return hours + " hour";
                    }
                } else
                {
                    if (minutesLeft >= 1)
                    {
                        if (minutesLeft != 1)
                        {
                            return minutesLeft + " minutes";
                        } else
                        {
                            return minutesLeft + " minute";
                        }
                    } else
                    {
                        return "";
                    }
                }
            }
        }

        public static string GetLampHoverString(DroppedGearDummy DGD)
        {
            string GearName = DGD.m_LocalizedDisplayName;
            string Dropper = DGD.m_Extra.m_Dropper;
            string ActionString = "Dropped";
            int minutesLeft = DGD.m_Extra.m_GoalTime - MyMod.MinutesFromStartServer;
            string DropTime = DropTimeFormat(minutesLeft);
            string FuelLeft = "Fuel left for " + DropTime;
            bool Lit = DGD.m_Extra.m_Variant != 0;
            if (Lit)
            {
                ActionString = "Placed";
                if (string.IsNullOrEmpty(DropTime))
                {
                    FuelLeft = "No Fuel left " + ActionString + " by " + Dropper;
                } else
                {
                    FuelLeft = "Fuel left for "+ DropTime + ActionString + " by " + Dropper;
                }
                return GearName + "\n" + FuelLeft;
            } else
            {
                return GearName +"\n" + ActionString + " by " + Dropper;
            }
        }
        public static string GetSnareHoverString(DroppedGearDummy DGD)
        {
            string GearName = DGD.m_LocalizedDisplayName;
            string Dropper = DGD.m_Extra.m_Dropper;
            string ActionString = "Dropped";
            int minutesOnDry = MyMod.MinutesFromStartServer - DGD.m_Extra.m_DroppedTime;
            int minutesLeft = MyMod.TimeToDry("GEAR_Snare") - minutesOnDry + 1;
            string DropTime = DropTimeFormat(minutesLeft);
            bool IsSetup = DGD.m_Extra.m_Variant != 0;
            if (IsSetup)
            {
                ActionString = "Setup";

                if(DGD.m_Extra.m_GoalTime == -1)
                {
                    return GearName + "\n" + "Cannot snare here " + ActionString + " by " + Dropper;
                } else
                {
                    if (string.IsNullOrEmpty(DropTime))
                    {
                        DropTime = "As soon as no one see";
                    }
                    if (DGD.m_Extra.m_Variant == 1)
                    {
                        return GearName + "\n" + "May catch in: " + DropTime + " " + ActionString + " by " + Dropper;
                    } else
                    {
                        return GearName + "\n" + "Will catch in: " + DropTime + " " + ActionString + " by " + Dropper;
                    }
                }
            } else
            {
                return GearName + "\n" + ActionString + " by " + Dropper;
            }
        }

        public static string GetCureableHoverString(DroppedGearDummy DGD, string Name)
        {
            string GearName = DGD.m_LocalizedDisplayName;
            string Dropper = DGD.m_Extra.m_Dropper;
            string ActionString = "dropped";
            int minutesOnDry = MyMod.MinutesFromStartServer - DGD.m_Extra.m_DroppedTime;
            int minutesLeft = MyMod.TimeToDry(Name) - minutesOnDry + 1;
            string DropTime = DropTimeFormat(minutesLeft);

            string Done = "Cured";
            string WillBeDone = "Cures in";

            if (Name.Contains("GEAR_SCWoodFormB"))
            {
                Done = "Cooled";
                WillBeDone = "Cools in";
            }

            if (DGD.m_Extra.m_GoalTime == -1)
            {
                return GearName + "\n" + "Cannot be cured here, " + ActionString + " by " + Dropper;
            }

            if (string.IsNullOrEmpty(DropTime))
            {
                return GearName + "\n" + Done + ", " + ActionString + " by " + Dropper; ;
            } else
            {
                return GearName + "\n" + WillBeDone + ": " + DropTime + ", " + ActionString + " by " + Dropper;
            }
        }



        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "GetInteractiveObjectDisplayText")] // Almost always
        internal class PlayerManager_GetInteractiveObjectDisplayText
        {
            internal static void Postfix(PlayerManager __instance, GameObject interactiveObject, ref string __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (interactiveObject != null)
                {
                    if (interactiveObject.GetComponent<Comps.MultiplayerPlayer>() != null)
                    {
                        Comps.MultiplayerPlayer mP = interactiveObject.GetComponent<Comps.MultiplayerPlayer>();

                        int m_LevelId = 0;
                        string m_LevelGUID = "";
                        if (MyMod.playersData[mP.m_ID] != null)
                        {
                            m_LevelId = MyMod.playersData[mP.m_ID].m_Levelid;
                            m_LevelGUID = MyMod.playersData[mP.m_ID].m_LevelGuid;
                        }


                        if (MyMod.levelid != m_LevelId && MyMod.level_guid != m_LevelGUID)
                        {
                            return;
                        }
                        string actString = GetPriorityActionForPlayer(mP.m_ID, mP).m_DisplayText;
                        string PlayerName = "";
                        if (MyMod.playersData[mP.m_ID] != null && MyMod.playersData[mP.m_ID].m_Name != "")
                        {
                            PlayerName = MyMod.playersData[mP.m_ID].m_Name;
                        } 
                        else
                        {
                            PlayerName = "Player";
                        }
                        if (GameManager.GetPlayerManagerComponent().m_ItemInHands && MyMod.IsCustomHandItem(GameManager.GetPlayerManagerComponent().m_ItemInHands.m_GearName))
                        {
                            __result = PlayerName;
                            return;
                        }
                        __result = PlayerName + "\n" + actString;
                    } 
                    else if (interactiveObject.GetComponent<Comps.DroppedGearDummy>() != null)
                    {
                        Comps.DroppedGearDummy DGD = interactiveObject.GetComponent<Comps.DroppedGearDummy>();
                        string Dropper = DGD.m_Extra.m_Dropper;


                        if (string.IsNullOrEmpty(Dropper))
                        {
                            string ColorPrefix = "[87DBF5]";
                            string ColorAffix = "[-]";
                            __result = ColorPrefix + DGD.m_LocalizedDisplayName + ColorAffix;
                        } else
                        {
                            if (KeyboardUtilities.InputManager.GetKey(KeyCode.LeftShift))
                            {
                                string DropTime = DropTimeFormat(MyMod.MinutesFromStartServer - DGD.m_Extra.m_DroppedTime);
                                
                                if (string.IsNullOrEmpty(DropTime))
                                {
                                    __result = DGD.m_LocalizedDisplayName + "\n" + "Dropped just now";
                                    return;
                                }
                                __result = DGD.m_LocalizedDisplayName + "\n" + "Dropped " + DropTime + " ago";
                                return;
                            }
                            string ActionString = "Dropped";
                            if (interactiveObject.name.Contains("GEAR_KeroseneLampB"))
                            {
                                __result = GetLampHoverString(DGD);
                                return;
                            }
                            if (interactiveObject.name.Contains("GEAR_Snare"))
                            {
                                __result = GetSnareHoverString(DGD);
                                return;
                            }

                            string DroppedString = " " + ActionString + " by " + DGD.m_Extra.m_Dropper;
                            if (DGD.m_Extra.m_GoalTime == 0)
                            {
                                if (DroppedString != "")
                                {
                                    __result = DGD.m_LocalizedDisplayName + "\n" + DroppedString;
                                } else
                                {
                                    __result = DGD.m_LocalizedDisplayName;
                                }
                            } else if (DGD.m_Extra.m_GoalTime == -1)
                            {
                                __result = DGD.m_LocalizedDisplayName + "\n" + "Cannot be cured here" + DroppedString;
                            } else
                            {
                                __result = GetCureableHoverString(DGD, interactiveObject.name);
                                return;
                            }
                        }
                    } 
                    else if (interactiveObject.gameObject.name.Contains("Rabbit") && interactiveObject.GetComponent<Comps.AnimalCorpseObject>() != null)
                    {
                        __result = "Pickup";
                    } 
                    else if(interactiveObject.gameObject.GetComponent<Comps.DeathDropContainer>() != null)
                    {
                        Comps.DeathDropContainer DC = interactiveObject.gameObject.GetComponent<Comps.DeathDropContainer>();

                        if(DC.m_DeathTime == -1)
                        {
                            __result = DC.m_Owner + "'s Backpack\nDropped long time ago";
                        } else
                        {
                            __result = DC.m_Owner + "'s Backpack\nDropped " + DropTimeFormat(MyMod.MinutesFromStartServer - DC.m_DeathTime);
                        }
                    } else if (interactiveObject.gameObject.GetComponent<FakeRockCache>() != null)
                    {
                        __result = interactiveObject.gameObject.GetComponent<FakeRockCache>().m_Owner + "'s " + Localization.Get("GAMEPLAY_RadialRockCache");
                    } else if (interactiveObject.GetComponent<ExpeditionInteractive>())
                    {
                        __result = interactiveObject.GetComponent<ExpeditionInteractive>().m_Data.m_ObjectText;
                    }
                }
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(PlayerManager), "GetInteractiveObjectUnderCrosshairs")] // Almost always
        //internal class PlayerManager_GetInteractiveObjectUnderCrosshairs
        //{
        //    internal static void Postfix(PlayerManager __instance, float maxRange, ref GameObject __result)
        //    {
        //        if (MyMod.CrazyPatchesLogger == true)
        //        {
        //            StackTrace st = new StackTrace(new StackFrame(true));
        //            MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
        //            MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
        //        }
        //        int layerMask = vp_Layer.Default;
        //        RaycastHit hit;

        //        if (Physics.Raycast(GameManager.GetMainCamera().transform.position, GameManager.GetMainCamera().transform.forward, out hit, maxRange))
        //        {
        //            if (hit.collider.gameObject != null)
        //            {
        //                GameObject hitObj = hit.collider.transform.gameObject;
        //                //MelonLogger.Msg("Found something "+ hitObj.name);
        //                if (hitObj.GetComponent<MyMod.PlayerBulletDamage>() != null)
        //                {
        //                    if (hitObj.GetComponent<MyMod.PlayerBulletDamage>().m_Player != null)
        //                    {

        //                    }
        //                }
        //                //else if (hitObj.GetComponent<MyMod.DroppedGearDummy>() != null)
        //                //{
        //                //    __result = hitObj;
        //                //}
        //            }
        //        }
        //    }
        //}

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "FindInteractiveObject")] // Almsot always
        internal class PlayerManager_FindInteractiveObject
        {
            internal static void Postfix(RaycastHit hit, ref GearItem gi, ref GameObject interactiveObj)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (hit.transform.gameObject != null)
                {
                    if(hit.transform.gameObject.GetComponent<Comps.DroppedGearDummy>() != null)
                    {
                        gi = new GearItem();
                        interactiveObj = hit.transform.gameObject;
                    }
                    else if (hit.transform.gameObject.GetComponent<Comps.PlayerBulletDamage>() != null)
                    {
                        Comps.PlayerBulletDamage bulletZone = hit.transform.gameObject.GetComponent<Comps.PlayerBulletDamage>();
                        int m_ID = bulletZone.m_ClientId;

                        if (MyMod.playersData[m_ID] != null && bulletZone.m_Player != null)
                        {
                            int m_LevelId = MyMod.playersData[m_ID].m_Levelid;
                            string m_LevelGUID = MyMod.playersData[m_ID].m_LevelGuid;
                            if (MyMod.levelid == m_LevelId && MyMod.level_guid == m_LevelGUID)
                            {
                                gi = new GearItem();
                                interactiveObj = bulletZone.m_Player;
                            }
                        }
                    }
                    else if(hit.transform.gameObject.GetComponent<FakeRockCache>() != null)
                    {
                        gi = new GearItem();
                        interactiveObj = hit.transform.gameObject;
                    }
                    else if(hit.transform.gameObject.GetComponent<ExpeditionInteractive>() != null)
                    {
                        gi = new GearItem();
                        interactiveObj = hit.transform.gameObject;
                    }
                }

                if (hit.collider && hit.collider.gameObject)
                {
                    if (hit.collider.gameObject.layer == 16)
                    {
                        if (hit.collider.gameObject.GetComponent<Comps.AnimalCorpseObject>())
                        {
                            gi = new GearItem();
                            interactiveObj = hit.collider.gameObject.GetComponent<Comps.AnimalCorpseObject>().gameObject;
                        }
                    }else{
                        if (hit.collider.gameObject.layer == 27)
                        {
                            if (hit.collider.gameObject.transform.GetComponentInParent<Comps.AnimalCorpseObject>())
                            {
                                gi = new GearItem();
                                interactiveObj = hit.collider.gameObject.transform.GetComponentInParent<Comps.AnimalCorpseObject>().gameObject;
                            }
                        }
                    }
                }
            }
        }

        //[HarmonyLib.HarmonyPatch(typeof(PlayerManager), "IsClickHoldActive")]
        //internal class PlayerManager_IsClickHoldActive
        //{
        //    internal static void Postfix(ref bool __result)
        //    {
        //        if (MyMod.CrazyPatchesLogger == true)
        //        {
        //            StackTrace st = new StackTrace(new StackFrame(true));
        //            MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
        //            MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
        //        }
        //        if (__result == false)
        //        {
        //            if (MyMod.PlayerInteractionWith != null)
        //            {
        //                __result = true;
        //            }
        //        }
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(PlayerManager), "IsCancelableActionInProgress")] // Don't remember
        //internal class PlayerManager_IsCancelableActionInProgress
        //{
        //    internal static void Postfix(ref bool __result)
        //    {
        //        if (MyMod.CrazyPatchesLogger == true)
        //        {
        //            StackTrace st = new StackTrace(new StackFrame(true));
        //            MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
        //            MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
        //        }
        //        if (__result == false)
        //        {
        //            if (MyMod.PlayerInteractionWith != null)
        //            {
        //                __result = true;
        //            }
        //        }
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(InputManager), "MaybeCancelClickHold")] // Don't remember
        //internal class InputManager_MaybeCancelClickHold
        //{
        //    internal static void Postfix(ref bool __result)
        //    {
        //        if (MyMod.CrazyPatchesLogger == true)
        //        {
        //            StackTrace st = new StackTrace(new StackFrame(true));
        //            MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
        //            MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
        //        }
        //        if (__result == false)
        //        {
        //            if (MyMod.PlayerInteractionWith != null)
        //            {
        //                MyMod.LongActionCanceled(MyMod.PlayerInteractionWith);
        //                __result = true;
        //            }
        //        }
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(InputManager), "IsClickHoldActive")] // Always
        //internal class InputManager_IsClickHoldActive
        //{
        //    internal static void Postfix(ref bool __result)
        //    {
        //        if (MyMod.CrazyPatchesLogger == true)
        //        {
        //            StackTrace st = new StackTrace(new StackFrame(true));
        //            MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
        //            MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
        //        }
        //        if (__result == false)
        //        {
        //            if (MyMod.PlayerInteractionWith != null)
        //            {
        //                __result = true;
        //            }
        //        }
        //    }
        //}

        public static DataStr.PriorityActionForOtherPlayer GetPriorityActionForPlayer(int m_ID, Comps.MultiplayerPlayer mP)
        {
            DataStr.MultiPlayerClientData pData = MyMod.playersData[m_ID];
            DataStr.PriorityActionForOtherPlayer act = new DataStr.PriorityActionForOtherPlayer();
            if (GameManager.m_PlayerManager != null && GameManager.m_PlayerManager.m_ItemInHands != null && GameManager.m_PlayerManager.m_ItemInHands.m_HandheldShortwaveItem != null)
            {
                act = MyMod.GetActionForOtherPlayer("Invite");
            } else if (GameManager.m_PlayerManager != null && GameManager.m_PlayerManager.m_ItemInHands != null && GameManager.m_PlayerManager.m_ItemInHands.m_EmergencyStim != null)
            {
                act = MyMod.GetActionForOtherPlayer("Stim");
            }
            else if (GameManager.m_PlayerManager != null && GameManager.m_PlayerManager.PlayerHoldingTorchThatCanBeLit() == true && mP.m_TorchIgniter != null && mP.m_LightSourceOn == true)
            {
                act = MyMod.GetActionForOtherPlayer("Lit");
            }
            else
            {
                act = MyMod.GetActionForOtherPlayer("Examine");
            }
            return act;
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InteractiveObjectsProcessInteraction")] // Once
        internal class PlayerManager_InteractiveObjectsProcessInteraction
        {
            internal static bool Prefix(PlayerManager __instance)
            {
                if (__instance.m_InteractiveObjectUnderCrosshair != null)
                {
                    GameObject obj = __instance.m_InteractiveObjectUnderCrosshair;
                    if (obj.GetComponent<FakeRockCache>() != null)
                    {
                        return false;
                    }
                }
                return true;
            }
            internal static void Postfix(PlayerManager __instance, ref bool __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_InteractiveObjectUnderCrosshair != null)
                {
                    GameObject obj = __instance.m_InteractiveObjectUnderCrosshair;
                    if (obj.GetComponent<Comps.MultiplayerPlayer>() != null)
                    {
                        Comps.MultiplayerPlayer mP = obj.GetComponent<Comps.MultiplayerPlayer>();

                        string PAction = GetPriorityActionForPlayer(mP.m_ID, mP).m_Action;
                        string ProcessText = GetPriorityActionForPlayer(mP.m_ID, mP).m_ProcessText;

                        if(GameManager.GetPlayerManagerComponent().m_ItemInHands && MyMod.IsCustomHandItem(GameManager.GetPlayerManagerComponent().m_ItemInHands.m_GearName))
                        {
                            __result = false;
                            return;
                        }

                        if (PAction == "Invite")
                        {
                            MyMod.SendInvietToPlayer(mP.m_ID);
                            //MyMod.DoLongAction(mP, ProcessText, PAction);
                            __result = true;
                        }
                        else if (PAction == "Stim")
                        {
                            if (GameManager.m_PlayerManager != null && GameManager.m_PlayerManager.m_ItemInHands != null && GameManager.m_PlayerManager.m_ItemInHands.m_EmergencyStim != null)
                            {
                                MyMod.EmergencyStimBeforeUse = GameManager.m_PlayerManager.m_ItemInHands;
                                GameManager.GetPlayerManagerComponent().UseInventoryItem(GameManager.m_PlayerManager.m_ItemInHands);// Unequip GEAR_EmergencyStim
                                MyMod.DoLongAction(mP, ProcessText, PAction);
                                __result = true;
                            }
                        }
                        else if (PAction == "Lit")
                        {
                            if (GameManager.m_PlayerManager != null && GameManager.m_PlayerManager.PlayerHoldingTorchThatCanBeLit() == true && mP.m_TorchIgniter != null && mP.m_LightSourceOn == true)
                            {
                                GameManager.GetPlayerManagerComponent().m_ItemInHands.m_TorchItem.IgniteDelayed(2f, "", true);
                                GameManager.GetPlayerAnimationComponent().LookAt(mP.m_TorchIgniter.transform.position);
                                __result = true;
                            }
                        }
                        else if (PAction == "Examine")
                        {
                            MyMod.DoLongAction(mP, ProcessText, PAction);
                            __result = true;
                        }
                    }
                    else if (obj.GetComponent<Comps.DroppedGearDummy>() != null)
                    {
                        MelonLogger.Msg("Trying interact with fake gear");
                        if (obj.GetComponent<Comps.FakeBed>() == null)
                        {
                            MelonLogger.Msg("Trying pickup fake drop");
                            MyMod.PickupDroppedGear(obj);
                            __result = true;
                        }
                        else
                        {
                            MyMod.UseFakeBed(obj);
                            __result = true;
                        }
                    }
                    else if(obj.GetComponent<Comps.AnimalCorpseObject>() != null && obj.name.Contains("Rabbit"))
                    {
                        if (obj.GetComponent<ObjectGuid>() != null)
                        {
                            MyMod.AttemptToPickupRabbit(obj.GetComponent<ObjectGuid>().Get());
                        }
                        __result = true;
                    } else if(obj.GetComponent<FakeRockCache>() != null)
                    {
                        MyMod.ShowRockStashActionPicker(obj);
                        __result = true;
                    } else if (obj.GetComponent<ExpeditionInteractive>() != null)
                    {
                        __result = obj.GetComponent<ExpeditionInteractive>().TryInteract();
                    }
                    
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Container), "Close")] // Once
        public static class Container_UsingSyncClose
        {
            public static void Postfix(Container __instance, ref bool __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__result == true)
                {
                    if (MyMod.MyContainer != null)
                    {
                        MyMod.MyContainer = null;
                        //MelonLogger.Msg("Stop interacting wtih container");
                        DataStr.ContainerOpenSync pendingContainer = new DataStr.ContainerOpenSync();
                        pendingContainer.m_Guid = "NULL";
                        if (MyMod.sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.CONTAINERINTERACT))
                            {
                                _packet.Write(pendingContainer);
                                SendTCPData(_packet);
                            }
                        }
                        if (MyMod.iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.CONTAINERINTERACT))
                            {
                                ServerSend.CONTAINERINTERACT(0, pendingContainer, true);
                            }
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Container), "Open")] // Once
        public static class Container_UsingSyncOpen
        {
            public static void Postfix(Container __instance, ref bool __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__result == true)
                {
                    DataStr.ContainerOpenSync pendingContainer = new DataStr.ContainerOpenSync();
                    pendingContainer.m_LevelID = MyMod.levelid;
                    pendingContainer.m_LevelGUID = MyMod.level_guid;
                    pendingContainer.m_Inspected = __instance.m_Inspected;

                    if (__instance.gameObject != null)
                    {
                        GameObject contObj = __instance.gameObject;
                        if (contObj.GetComponent<ObjectGuid>() != null)
                        {
                            pendingContainer.m_Guid = contObj.GetComponent<ObjectGuid>().Get();
                        }

                        for (int index = 0; index < __instance.m_Items.Count; ++index)
                        {
                            GearItem gearItem = __instance.m_Items[index];
                            if (gearItem != null)
                            {
                                if (gearItem.m_GearName.Contains("MountainTownFarmKey") == true)
                                {
                                    return;
                                }
                            }
                        }
                    }
                    if (MyMod.MyContainer == null || MyMod.MyContainer.Equals(pendingContainer) == false)
                    {
                        MyMod.MyContainer = pendingContainer;
                        //MelonLogger.Msg("Interacting wtih container");
                        if (MyMod.sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.CONTAINERINTERACT))
                            {
                                _packet.Write(MyMod.MyContainer);
                                SendTCPData(_packet);
                            }
                        }
                        if (MyMod.iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.CONTAINERINTERACT))
                            {
                                ServerSend.CONTAINERINTERACT(0, MyMod.MyContainer, true);
                            }
                        }

                        if (!__instance.m_Inspected)
                        {
                            Shared.AddLootedContainer(MyMod.MyContainer, true, ClientUser.myId, 0);
                            if (__instance.GetComponent<ContainersSync>() != null)
                            {
                                __instance.GetComponent<ContainersSync>().m_Empty = true;
                            }
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Container), "GetInteractiveDisplayText")] // Once
        public static class Container_GetInteractiveDisplayText
        {
            public static void Postfix(Container __instance, ref string __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                
                if(__instance.m_Inspected && __instance.gameObject && __instance.gameObject.GetComponent<ContainersSync>() != null)
                {
                    bool Empty = __instance.gameObject.GetComponent<ContainersSync>().m_Empty;

                    if (!Empty)
                    {
                        __result = __instance.GetInteractiveActionText() + "\n" + Localization.Get("GAMEPLAY_SearchedPostfix");
                    } else{
                        __result = __instance.GetInteractiveActionText() + "\n" + Localization.Get("GAMEPLAY_EmptyPostfix");
                    }
                }
            }
        }

        public static void SendHarvestPlantState(string state, Harvestable plant)
        {
            DataStr.HarvestableSyncData harvData = new DataStr.HarvestableSyncData();
            harvData.m_State = state;
            if (plant.gameObject != null)
            {
                GameObject plantObj = plant.gameObject;
                if (plantObj.GetComponent<ObjectGuid>() != null)
                {
                    harvData.m_Guid = plantObj.GetComponent<ObjectGuid>().Get();
                }
            }
            if (MyMod.sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.HARVESTPLANT))
                {
                    _packet.Write(harvData);
                    SendTCPData(_packet);
                }
            }
            if (MyMod.iAmHost == true || MyMod.InOnline() == false)
            {
                using (Packet _packet = new Packet((int)ServerPackets.HARVESTPLANT))
                {
                    ServerSend.HARVESTPLANT(0, harvData, true);
                }

                if (state == "Done")
                {
                    MPSaveManager.AddHarvestedPlant(harvData.m_Guid, MyMod.level_guid);
                    ServerSend.LOOTEDHARVESTABLE(0, harvData.m_Guid, MyMod.level_guid, MyMod.MinutesFromStartServer, false);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Harvestable), "DoHarvest")] // Once
        public static class Harvestable_DoHarvest
        {
            public static void Postfix(Harvestable __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                SendHarvestPlantState("Start", __instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Harvestable), "CancelHarvest")] // Once
        public static class Harvestable_CancelHarvest
        {
            public static void Postfix(Harvestable __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                SendHarvestPlantState("Cancel", __instance);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Harvestable), "RollSpawnChance")] // Once
        public static class Harvestable_RollSpawnChance
        {
            public static bool Prefix(Harvestable __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                int seed = 0;
                if (__instance.gameObject.GetComponent<ObjectGuid>())
                {
                    seed = Shared.GetVectorHash(__instance.gameObject.transform.position);
                }

                System.Random RNG = new System.Random(seed);

                if (!__instance.m_IgnoreCustomModeSpawnChance && GameManager.InCustomMode())
                    __instance.m_SpawnChance *= GameManager.GetExperienceModeManagerComponent().GetCustomPlantSpawnModifier();
                if (Utils.Approximately(__instance.m_SpawnChance, 100f, 0.0001f) || MyMod.RollChanceSeeded(__instance.m_SpawnChance, RNG))
                {
                    return false;
                }
                    
                __instance.m_Harvested = true;
                __instance.gameObject.SetActive(false);
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Harvestable), "CompletedHarvest")] // Once
        public static class Harvestable_CompletedHarvest
        {
            public static void Postfix(Harvestable __instance, bool success)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (success == true)
                {
                    SendHarvestPlantState("Done", __instance);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Harvestable), "ProcessInteraction")] // Once
        public static class Harvestable_ProcessInteraction
        {
            public static bool Prefix(Harvestable __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                string ObjGUID = "";
                if (__instance.gameObject != null)
                {
                    GameObject contObj = __instance.gameObject;
                    if (contObj.GetComponent<ObjectGuid>() != null)
                    {
                        ObjGUID = contObj.GetComponent<ObjectGuid>().Get();
                    }

                    for (int i = 0; i < MyMod.playersData.Count; i++)
                    {
                        if (MyMod.playersData[i] != null)
                        {
                            string otherGuid = MyMod.playersData[i].m_HarvestingAnimal;
                            if (otherGuid == ObjGUID)
                            {
                                HUDMessage.AddMessage(MyMod.playersData[i].m_Name + " IS ALREADY COLLECTING THIS");
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BootUpdate), "Update")] // Once
        public class BootUpdate_Update
        {
            public static bool Prefix(BootUpdate __instance)
            {
                if (MyMod.KillOnUpdate)
                {
                    return false;
                }else{
                    return true;
                }
            }
            public static void Postfix(BootUpdate __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                if (MyMod.KillOnUpdate)
                {
                    if (InputManager.instance && InputManager.GetEscapePressed(InputManager.m_CurrentContext))
                    {
                        Application.Quit();
                    }
                }else{
                    if (NeedSkipCauseConnect() == false && MyMod.DedicatedServerAppMode == false)
                    {
                        return;
                    }
                    __instance.m_Label_Continue.gameObject.SetActive(false);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BootUpdate), "Start")] // Once
        public class BootUpdate_Start
        {
            public static void Postfix(BootUpdate __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if(PlayerPrefs.GetString(nameof(Localization.Language), "English") == "Russian")
                {
                    MyMod.DefaultIsRussian = true;
                }

                if (MyMod.KillOnUpdate)
                {
                    UnityEngine.Object.Destroy(__instance.m_Label_Continue.gameObject.GetComponent<UILocalize>());
                    __instance.m_Label_Continue.text = "Press Esc to Exit";
                    __instance.m_Label_Continue.mText = "Press Esc to Exit";
                    __instance.m_Label_Continue.ProcessText();
                    __instance.m_Label_Continue.gameObject.SetActive(true);
                    for (int i = 1; i <= 3; i++)
                    {
                        Transform LabT = __instance.gameObject.transform.Find("Label_Disclaimer_" + i);
                        GameObject Lab;
                        if (LabT)
                        {
                            Lab = LabT.gameObject;

                            if(i < 3)
                            {
                                UILabel LabCom = Lab.GetComponent<UILabel>();
                                UILocalize LabLoca = Lab.GetComponent<UILocalize>();
                                if (LabLoca)
                                {
                                    UnityEngine.Object.Destroy(LabLoca);
                                }

                                string txt = "Installation of missing plugins finished!";
                                if(i == 2)
                                {
                                    txt = "The missing plugins for the mod have been installed. Please restart the game.\nIf this your first installation, autoupating plugin will download dependencies, so it can take a while.";
                                }

                                if (LabCom)
                                {
                                    LabCom.mText = txt;
                                    LabCom.text = txt;
                                    LabCom.ProcessText();
                                }
                                Lab.gameObject.SetActive(true);
                            }else{
                                Lab.gameObject.SetActive(false);
                            }
                        }
                    }
                }else{
                    if (NeedSkipCauseConnect() == false && !MyMod.DedicatedServerAppMode)
                    {
                        return;
                    }
                    for (int i = 1; i <= 3; i++)
                    {
                        __instance.gameObject.transform.Find($"Label_Disclaimer_{i}")?.gameObject.SetActive(false);
                    }
                    __instance.LoadMainMenu();
                }
            }
        }

        public static void RemoveSinglePlayer(Panel_MainMenu __instance)
        {
            Transform Grid = MyMod.m_Panel_MainMenu.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(5).GetChild(2);
            for (int i = 0; i < 3; i++)
            {
                Grid.GetChild(i).gameObject.SetActive(false);
            }
            if (MyMod.MyLobby != "")
            {
                MenuChange.OverrideMenuButton(Grid, 3, "LOBBY", false);
            }else{
                MenuChange.OverrideMenuButton(Grid, 3, "MULTIPLAYER", false);
            }
        }


        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "UpdateFading")] // Always
        internal class Panel_MainMenu_UpdateFading
        {
            private static void Postfix(Panel_MainMenu __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                RemoveSinglePlayer(__instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "Enable")] // Once
        internal class Panel_MainMenu_Enable
        {
            private static void Postfix(Panel_MainMenu __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                Transform Grid = MyMod.m_Panel_MainMenu.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(5).GetChild(2);
                RemoveSinglePlayer(__instance);
                __instance.m_BasicMenu.SetItemSelected(4);
                if (NeedSkipCauseConnect() == false && !MyMod.DedicatedServerAppMode)
                {
                    return;
                }
                MoviePlayer.m_HasIntroPlayedForMainMenu = true;
                __instance?.m_HinterlandMailingListWidget?.gameObject?.SetActive(false);
                
                for (int i = 0; i < 4; i++)
                {
                    Grid.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "OnMainMenuTop")] // Once
        internal class Panel_MainMenu_OnMainMenuTop
        {
            private static void Postfix(Panel_MainMenu __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                RemoveSinglePlayer(__instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(TextInputField), "Start")] // Once
        internal class TextInputField_MoreLetters
        {
            private static void Postfix(TextInputField __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_Input.characterLimit < 100)
                {
                    __instance.m_MaxLength = 100;
                    __instance.m_Input.characterLimit = (int)__instance.m_MaxLength;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SnowShelter), "Start")] // Once
        internal class SnowShelter_Serialize
        {
            private static bool Prefix(SnowShelter __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_StartHasBeenCalled)
                {
                    return false;
                }
                __instance.m_StartHasBeenCalled = true;
                if (__instance.gameObject != null && __instance.gameObject.GetComponent<Comps.DoNotSerializeThis>() == null)
                {
                    SnowShelterManager.Add(__instance);
                    //MelonLogger.Msg("Registered an snowshelter, cause have not DoNotSerializeThis component.");
                }
                else
                {
                    //MelonLogger.Msg("Skip snowshelter, cause have DoNotSerializeThis component.");
                }
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_SnowShelterBuild), "BuildFinished")] // Once
        public static class Panel_SnowShelterBuild_BuildFinished
        {
            public static void Prefix(Panel_SnowShelterBuild __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_SnowShelter != null)
                {
                    MelonLogger.Msg("Shelter builded!");
                    GameObject shelter = __instance.m_SnowShelter.gameObject;
                    Shared.ShelterCreated(shelter.transform.position, shelter.transform.rotation, MyMod.levelid, MyMod.level_guid, true);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SnowShelter), "DismantleFinished")] // Once
        public static class SnowShelter_DismantleFinished
        {
            public static void Postfix(SnowShelter __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("Shelter removed!");
                GameObject shelter = __instance.gameObject;
                Shared.ShelterRemoved(shelter.transform.position, MyMod.levelid, MyMod.level_guid, true);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_SnowShelterInteract), "OnUse")] // Once
        public static class Panel_SnowShelterInteract_OnUse
        {
            public static void Postfix(Panel_SnowShelterInteract __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_SnowShelter != null)
                {
                    MelonLogger.Msg("Entering shelter");

                    DataStr.ShowShelterByOther shelter = new DataStr.ShowShelterByOther();
                    shelter.m_Position = __instance.m_SnowShelter.gameObject.transform.position;
                    shelter.m_Rotation = __instance.m_SnowShelter.gameObject.transform.rotation;
                    shelter.m_LevelID = MyMod.levelid;
                    shelter.m_LevelGUID = MyMod.level_guid;
                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.USESHELTER))
                        {
                            _packet.Write(shelter);
                            SendTCPData(_packet);
                        }
                    }
                    if (MyMod.iAmHost == true)
                    {
                        ServerSend.USESHELTER(0, shelter, true);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SnowShelterManager), "EnterShelter")] // Once
        public static class SnowShelterManager_EnterShelter
        {
            public static void Postfix(SnowShelterManager __instance, SnowShelter ss)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (ss != null)
                {
                    MelonLogger.Msg("Entering shelter");

                    DataStr.ShowShelterByOther shelter = new DataStr.ShowShelterByOther();
                    shelter.m_Position = ss.gameObject.transform.position;
                    shelter.m_Rotation = ss.gameObject.transform.rotation;
                    shelter.m_LevelID = MyMod.levelid;
                    shelter.m_LevelGUID = MyMod.level_guid;
                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.USESHELTER))
                        {
                            _packet.Write(shelter);
                            SendTCPData(_packet);
                        }
                    }
                    if (MyMod.iAmHost == true)
                    {
                        ServerSend.USESHELTER(0, shelter, true);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SnowShelterManager), "ExitShelter")] // Once
        public static class SnowShelterManager_ExitShelter
        {
            public static void Postfix(SnowShelterManager __instance, SnowShelter ss)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (ss != null)
                {
                    MelonLogger.Msg("Exiting shelter");
                    DataStr.ShowShelterByOther shelter = new DataStr.ShowShelterByOther();
                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.USESHELTER))
                        {
                            _packet.Write(shelter);
                            SendTCPData(_packet);
                        }
                    }
                    if (MyMod.iAmHost == true)
                    {
                        ServerSend.USESHELTER(0, shelter, true);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_SnowShelterInteract), "OnDismantle")] // Once
        public static class Panel_SnowShelterInteract_OnDismantle
        {
            public static bool Prefix(Panel_SnowShelterInteract __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                DataStr.ShowShelterByOther FindData = new DataStr.ShowShelterByOther();
                if (__instance.m_SnowShelter != null && __instance.m_SnowShelter.gameObject != null)
                {
                    FindData.m_Position = __instance.m_SnowShelter.gameObject.transform.position;
                    FindData.m_Rotation = __instance.m_SnowShelter.gameObject.transform.rotation;
                    FindData.m_LevelID = MyMod.levelid;
                    FindData.m_LevelGUID = MyMod.level_guid;
                }
                for (int i = 0; i < MyMod.playersData.Count; i++)
                {
                    if (MyMod.playersData[i] != null)
                    {
                        DataStr.ShowShelterByOther shelter = MyMod.playersData[i].m_Shelter;
                        if (FindData == shelter)
                        {
                            HUDMessage.AddMessage(MyMod.playersData[i].m_Name + " INSIDE, CAN'T DISMANTLE THIS!");
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Fire), "AddFuel")] // Once can be triggered often
        public static class Fire_AddFuel
        {
            public static void Postfix(Fire __instance, GearItem fuel)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.gameObject != null && fuel != null)
                {
                    MelonLogger.Msg("[Fire][AddFuel] " + __instance.gameObject.name + " fuel is " + fuel.m_GearName);
                }
                else if (__instance.gameObject != null && fuel == null)
                {
                    MelonLogger.Msg("[Fire][AddFuel] " + __instance.gameObject.name + " fuel is null");
                }
                else if (__instance.gameObject == null)
                {
                    MelonLogger.Msg("[Fire][AddFuel] firesource is null");
                }

                if (__instance.m_StartedByPlayer == false)
                {
                    MelonLogger.Msg("Assign firesource of other player to myself");
                    __instance.m_StartedByPlayer = true;
                    MyMod.SendMyFire(__instance);
                }
                if (__instance.m_StartedByPlayer == true)
                {
                    MelonLogger.Msg("Added fuel to fire " + fuel.m_GearName);
                    MyMod.SendMyFire(__instance, fuel.m_GearName);
                }
            }
        }

        //[HarmonyLib.HarmonyPatch(typeof(PlacePoints), "ShouldShow")] // Unknown
        //public static class PlacePoints_ShouldShow
        //{
        //    public static void Postfix(PlacePoints __instance, bool __result)
        //    {
        //        if (MyMod.CrazyPatchesLogger == true)
        //        {
        //            StackTrace st = new StackTrace(new StackFrame(true));
        //            MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
        //            MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
        //        }
        //        if (__instance.m_AttachedFire != null)
        //        {
        //            if (__instance.m_AttachedFire.IsBurning() && __instance.m_AttachedFire.m_StartedByPlayer == false)
        //            {
        //                __result = false;
        //            }
        //        }
        //    }
        //}

        [HarmonyLib.HarmonyPatch(typeof(CookingSlot), "CanBeInteractedWith")] // Unknown
        public static class CookingSlot_CanBeInteractedWith
        {
            public static bool Prefix(CookingSlot __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_GearPlacePoint != null
                    && __instance.m_GearPlacePoint.m_FireToAttach != null
                    && __instance.m_GearPlacePoint.m_FireToAttach.IsBurning()
                    && __instance.m_GearPlacePoint.m_FireToAttach.m_StartedByPlayer == false)
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(CookingPotItem), "CanOpenCookingInterface")] // Unknown
        public static class CookingPotItem_CanOpenCookingInterface
        {
            public static bool Prefix(CookingPotItem __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_FireBeingUsed != null
                    && __instance.m_FireBeingUsed.IsBurning()
                    && __instance.m_FireBeingUsed.m_StartedByPlayer == false)
                {
                    return false;
                }
                return true;
            }
        }

        //[HarmonyLib.HarmonyPatch(typeof(CookingPotItem), "AttachedFireIsBurning")] // Unknown
        //public static class CookingPotItem_AttachedFireIsBurning
        //{
        //    public static bool Prefix(CookingPotItem __instance)
        //    {
        //        if (MyMod.CrazyPatchesLogger == true)
        //        {
        //            StackTrace st = new StackTrace(new StackFrame(true));
        //            MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
        //            MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
        //        }
        //        if (__instance.m_FireBeingUsed != null
        //            && __instance.m_FireBeingUsed.IsBurning()
        //            && __instance.m_FireBeingUsed.m_StartedByPlayer == false)
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
        //}
        [HarmonyLib.HarmonyPatch(typeof(CookingPotItem), "AttachToFire")] // Once
        public static class CookingPotItem_AttachToFire
        {
            public static bool Prefix(CookingPotItem __instance, Fire fire, GearPlacePoint gpp)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (fire != null
                    && fire.IsBurning()
                    && fire.m_StartedByPlayer == false)
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(WoodStove), "GetHoverText")] // Always when looking at object
        public static class WoodStove_GetHoverText
        {
            public static void Postfix(WoodStove __instance, ref string __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_Fire != null
                    && __instance.m_Fire.IsBurning()
                    && __instance.m_Fire.m_StartedByPlayer == false)
                {
                    __result = __instance.m_LocalizedDisplayName.Text() + "\nADD YOUR OWN FUEL\nTO BE ABLE TO\nCOOK AND WARM YOURSELF";
                    //MelonLogger.Msg(__result);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Campfire), "GetHoverText")] // Always when looking at object
        public static class Campfire_GetHoverText
        {
            public static void Postfix(Campfire __instance, ref string __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance.m_Fire != null
                    && __instance.m_Fire.IsBurning()
                    && __instance.m_Fire.m_StartedByPlayer == false)
                {
                    __result = __instance.m_DisplayName + "\nADD YOUR OWN FUEL\nTO BE ABLE TO\nCOOK AND WARM YOURSELF";
                    //MelonLogger.Msg(__result);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(InterfaceManager), "Awake")] // Once
        public static class UI_InterfaceManager
        {
            public static void Prefix(InterfaceManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance != null)
                {
                    MyMod.m_InterfaceManager = __instance;
                    MelonLogger.Msg("[Legacy UIs] Captured link on InterfaceManager!");
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectSurvivor), "Initialize")] // Once
        public static class UI_Panel_SelectSurvivor
        {
            public static void Prefix(Panel_SelectSurvivor __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance != null)
                {
                    MyMod.m_Panel_SelectSurvivor = __instance;
                    MelonLogger.Msg("[Legacy UIs] Captured link on Panel_SelectSurvivor!");
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Sandbox), "Initialize")] // Once
        public static class Panel_Sandbox_Initialize
        {
            public static void Prefix(Panel_Sandbox __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance != null)
                {
                    MyMod.m_Panel_Sandbox = __instance;
                    MelonLogger.Msg("[Legacy UIs] Captured link on Panel_Sandbox!");
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "Initialize")] // Once
        public static class UI_Panel_MainMenu
        {
            public static void Prefix(Panel_MainMenu __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance != null)
                {
                    MyMod.m_Panel_MainMenu = __instance;
                    MelonLogger.Msg("[Legacy UIs] Captured link on Panel_MainMenu!");
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_ChooseSandbox), "Initialize")] // Once
        public static class Panel_ChooseSandbox_Initialize
        {
            public static void Prefix(Panel_ChooseSandbox __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (__instance != null)
                {
                    MyMod.m_Panel_ChooseSandbox = __instance;
                    MelonLogger.Msg("[Legacy UIs] Captured link on Panel_ChooseSandbox!");
                }
            }
        }
        
        [HarmonyLib.HarmonyPatch(typeof(Condition), "KillPlayer")] // Once
        public class Condition_Test
        {
            public static bool Prefix(Condition __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("[Condition] PlayerDeath");
                if (MyMod.InOnline() == true)
                {
                    __instance.ForceKill();
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public static GameObject FakeBedRoll = null;
        [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "DoRest")] // Once
        private static class Panel_Rest_DoRest
        {
            internal static void Prefix(Panel_Rest __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("[Panel_Rest] DoRest");
                if (__instance.m_Bed != null && __instance.m_Bed.gameObject != null && __instance.m_Bed.gameObject.GetComponent<Comps.FakeBedDummy>() != null)
                {
                    MelonLogger.Msg("[Rest] Saving link on fake bed");
                    FakeBedRoll = __instance.m_Bed.gameObject;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Rest), "EndSleeping")] // Once
        public class Rest_StopSleep
        {
            public static void Postfix(Rest __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("[Rest] EndSleeping");
                if (MyMod.IsDead == true)
                {
                    GameManager.GetPlayerManagerComponent().m_God = false;
                }
                if (FakeBedRoll != null)
                {
                    MelonLogger.Msg("Dummy bed removed");
                    UnityEngine.Object.Destroy(FakeBedRoll);
                }
            }
        }

        public static WeatherVolunteerData GetWeatherVolunteerData(bool DoNext = false)
        {     
            WeatherVolunteerData Data = new WeatherVolunteerData();

            if (GameManager.m_Weather != null && GameManager.m_WeatherTransition != null && GameManager.m_WeatherTransition.m_CurrentWeatherSet != null && GameManager.GetUniStorm() != null)
            {
                Weather Weather = GameManager.GetWeatherComponent();
                WeatherStage PreviousStage = Weather.m_CurrentWeatherStage;
                WeatherTransition WeatherTransition = GameManager.GetWeatherTransitionComponent();
                if (DoNext)
                {
                    WeatherTransition.ChooseNextWeatherSet(WeatherTransition.GetCustomWeightsForCurrentXpMode());
                }
                Data.HighMin = Weather.m_HighTempMinCelsius;
                Data.HighMax = Weather.m_HighTempMaxCelsius;
                Data.LowMin = Weather.m_LowTempMinCelsius;
                Data.LowMax = Weather.m_LowTempMaxCelsius;
                Data.CoolingHours = Weather.m_HourCoolingBegins;
                Data.WarmingHours = Weather.m_HourWarmingBegins;
                WeatherSet Set = GameManager.GetWeatherTransitionComponent().m_CurrentWeatherSet;
                Set.m_WeatherStages[0].m_PreviousType = PreviousStage;
                Data.WeatherType = (int)Set.m_CharacterizingType;
                Data.WeatherDuration = Set.m_CurrentSetDuration;
                Data.CurrentRegion = (int)GameManager.GetUniStorm().m_CurrentRegion;
                Data.PreviousStage = (int)PreviousStage;
                for (int i = 0; i < Set.m_WeatherStages.Count; i++)
                {
                    Data.StageDuration.Add(Set.m_WeatherStages[i].m_CurrentDuration);
                    Data.StageTransition.Add(Set.m_WeatherStages[i].m_CurrentTransitionTime);
                }

                for (int i = 0; i < Weather.m_WeatherSetsForScene.Count; i++)
                {
                    if (Weather.m_WeatherSetsForScene[i].gameObject.name == Set.gameObject.name)
                    {
                        Data.SetIndex = i;
                        break;
                    }
                }
            } else
            {
                Data.WeatherType = (int)WeatherStage.Clear;
                Data.WeatherDuration = 1f;
                if (GameManager.GetUniStorm() != null)
                {
                    Data.CurrentRegion = (int)GameManager.GetUniStorm().m_CurrentRegion;
                } else
                {
                    Data.CurrentRegion = (int)GameRegion.LakeRegion;
                }
                Data.StageDuration.Add(1f);
                Data.StageTransition.Add(0);
            }
            return Data;
        }



        public static void SendWeatherVolunteerData()
        {
            using (Packet _packet = new Packet((int)ClientPackets.REREGISTERWEATHER))
            {
                _packet.Write(GetWeatherVolunteerData(true));
                SendTCPData(_packet);
            }
        }


        public static void RequestDropsForScene()
        {
            MyMod.DelayedGearsPickup = true;
            using (Packet _packet = new Packet((int)ClientPackets.REQUESTDROPSFORSCENE))
            {
                _packet.Write(MyMod.levelid);
                _packet.Write(MyMod.level_guid);
                _packet.Write(GetWeatherVolunteerData());
                _packet.Write((int)MyMod.ConvertGameRegion((GameRegion)GetWeatherVolunteerData().CurrentRegion));
                MyMod.SetRepeatPacket(MyMod.ResendPacketType.Scene);
                SendTCPData(_packet);
            }
        }

        public static void LoadEverything()
        {
            MyMod.BakePreSpawnedGearsList();
            MelonLogger.Msg(ConsoleColor.Yellow, "Loading everything...");
            MyMod.SendSpawnData();
            MyMod.UpdateDeployedRopes();
            MyMod.LoadAllSnowSheltersByOther();
            MyMod.ApplyOtherCampfires = true;
            MelonLogger.Msg(ConsoleColor.Yellow, "Loading done!");
            MyMod.DroppedGearsObjs.Clear();
            MyMod.TrackableDroppedGearsObjs.Clear();
            MyMod.OpenableThings.Clear();
            MyMod.DoorsObjs.Clear();
            MPSaveManager.TrySpawnFestives(MyMod.level_name);

            if (!MyMod.DedicatedServerAppMode && (MyMod.iAmHost == true || MyMod.InOnline() == false))
            {
                MyMod.PickedGearsBackup.Clear();
                
                Dictionary<long, PickedGearSync> PickedGears = MPSaveManager.LoadPickedGearsData(MyMod.level_guid);
                if (PickedGears != null)
                {
                    foreach (var item in PickedGears)
                    {
                        MyMod.PickedGearsBackup.Add(item.Value);
                        MyMod.RemovePickedGear(MPSaveManager.GetPickedGearKey(item.Value));
                    }
                }
                Dictionary<string, BrokenFurnitureSync> Furns = MPSaveManager.LoadFurnsData(MyMod.level_guid);
                MyMod.FoundSomethingToBreak = false;
                if (Furns != null)
                {
                    foreach (var item in Furns)
                    {
                        if (item.Value.m_Broken)
                        {
                            MyMod.RemoveBrokenFurniture(item.Value.m_Guid, item.Value.m_ParentGuid, false);
                        } else
                        {
                            MyMod.RepairBrokenFurniture(item.Value.m_Guid, item.Value.m_ParentGuid);
                        }
                    }
                }
                MyMod.BakePreSpawnedGearsList();
                if (MyMod.FoundSomethingToBreak)
                {
                    MyMod.RemoveAttachedGears = 2;
                } else
                {
                    MyMod.PickedGearsBackup.Clear();
                }

                Dictionary<string, UniversalSyncableObject> UniversalObjects = MPSaveManager.GetUniversalSyncablesForScene(MyMod.level_guid);
                foreach (var item in UniversalObjects)
                {
                    MyMod.SpawnUniversalSyncableObject(item.Value);
                }

                Dictionary<string, int> LootedBoxes = MPSaveManager.LoadLootedContainersData(MyMod.level_guid);
                if (LootedBoxes != null)
                {
                    foreach (var item in LootedBoxes)
                    {
                        int State = item.Value;

                        if (MPSaveManager.ContainerNotEmpty(MyMod.level_guid, item.Key))
                        {
                            State = 1;
                        }

                        MyMod.RemoveLootFromContainer(item.Key, State);
                    }
                }
                Dictionary<string, int> Plants = MPSaveManager.LoadHarvestedPlants(MyMod.level_guid);
                if (Plants != null)
                {
                    foreach (var item in Plants)
                    {
                        int HarvestTime = item.Value;
                        if(HarvestTime == -1)
                        {
                            MyMod.AddHarvastedPlant(item.Key);
                        } else
                        {
                            MyMod.RemoveHarvastedPlant(item.Key);
                        }
                    }
                }
                Dictionary<string, FakeRockCacheVisualData> RockCaches = MPSaveManager.GetRockCaches(MyMod.level_guid);
                foreach (var item in RockCaches)
                {
                    MyMod.AddRockCache(item.Value);
                }

                MyMod.LoadAllDropsForScene();
                MyMod.LoadAllOpenableThingsForScene();
                MyMod.LoadAniamlCorpsesForScene();
                foreach (DataStr.DeathContainerData create in MyMod.DeathCreates)
                {
                    if (create.m_LevelKey == MyMod.level_guid)
                    {
                        MyMod.MakeDeathCreate(create);
                    }
                }
                Dictionary<string, string> Doors = MPSaveManager.GetDoorsOnScene(MyMod.level_guid);
                foreach (var item in Doors)
                {
                    string GUID = item.Key.Split('_')[1];
                    MyMod.AddLocksToDoorsByGUID(GUID);
                }
                MyMod.CreateCairnsSearchList();

                if (MyMod.InOnline())
                {
                    Shared.RegisterWeatherSetForRegion(0, GetWeatherVolunteerData());
                }
            }
            if (MyMod.sendMyPosition == true)
            {
                MyMod.DoPleaseWait("Please wait...", "Downloading scene drops...");
                RequestDropsForScene();
            }
        }


        [HarmonyLib.HarmonyPatch(typeof(Panel_Loading), "Enable")] // Once
        internal static class Panel_Loading_Toggle
        {
            private static void Postfix(Panel_Loading __instance, bool enable)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (enable == false)
                {
                    MelonLogger.Msg(ConsoleColor.Yellow, "[Scene Load] Everything finished loading!");
                    MyMod.SendAfterLoadingFinished = 2;
                }
                MyMod.LoadingScreenIsOn = enable;


                //if(enable == false)
                //{

                //    Il2CppArrayBase<GameObject> obj = GameObject.FindObjectsOfType<GameObject>();

                //    MelonLogger.Msg(ConsoleColor.Blue, "Searching by " + obj.Length+" objects");

                //    for (int i = 0; i < obj.Length; i++)
                //    {
                //        if (obj[i] != null)
                //        {
                //            if (obj[i].name.Contains("ref_man"))
                //            {
                //                MelonLogger.Msg(ConsoleColor.Blue, "MUJIK FOUND!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Scene name " + MyMod.level_name);
                //                MyMod.CurrentSearchIndex++;
                //                MyMod.ContinuePoiskMujikov();
                //                return;
                //            }
                //        }
                //    }
                //    if (MyMod.sceneCount != -1)
                //    {
                //        MyMod.CurrentSearchIndex++;
                //        MyMod.ContinuePoiskMujikov();
                //    }
                //}
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(MissionServicesManager), "SceneLoadCompleted")] // Once
        public static class MissionServicesManager_Loaded
        {
            public static void Postfix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg(ConsoleColor.Yellow, "[MissionServicesManager] SceneLoadCompleted!");
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SaveGameSystem), "LoadSceneData")] // Once
        public static class SaveGameSystemPatch_LoadSceneData
        {
            public static void Postfix(string name)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg(ConsoleColor.Yellow, "[Scene Load] Saved data for scene is loaded!");
                MyMod.UpdateEverything = 2;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Loading), "OnQuoteFinished")] // Once
        public static class Panel_Loading_OnQuoteFinished
        {
            public static void Postfix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg(ConsoleColor.Yellow, "[Cold Start] Scene loaded!");
                MyMod.UpdateEverything = 2;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(InputManager), "PauseGame")] // Once
        public static class InputManager_DuckYOuPause
        {
            public static void Postfix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == true)
                {
                    GameManager.m_IsPaused = false;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Diagnosis), "OnBack")] // Once
        public static class Panel_Diagnosis_OnBack
        {
            public static void Postfix(Panel_Diagnosis __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("Stop diagnosis");
                if (MyMod.DiagnosisDummy != null)
                {
                    UnityEngine.Object.Destroy(MyMod.DiagnosisDummy);
                }
            }
        }

        public static bool ShouldKeepAfterTreatment(AfflictionType type)
        {
            if (type == AfflictionType.FoodPoisioning
                || type == AfflictionType.Dysentery
                || type == AfflictionType.BrokenRib
                || type == AfflictionType.Infection
                || type == AfflictionType.IntestinalParasites)
            {
                return true;
            }
            return false;
        }

        public static void ImproveFirstAidSkill(int Aff)
        {
            if (Aff == (int)AfflictionType.SprainedAnkle
                || Aff == (int)AfflictionType.SprainedWrist
                || Aff == (int)AfflictionType.SprainedWristMajor
                || Aff == (int)AfflictionType.SprainPain)

            {
                return;
            }

            IncressCustomSkill(CustomSkills.FirstAid, 2);

            if (GameManager.m_Inventory && GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_MedicalSupplies_hangar"))
            {
                GearItem Gi = GameManager.GetInventoryComponent().GetLowestConditionGearThatMatchesName("GEAR_MedicalSupplies_hangar");
                if(Gi != null)
                {
                    Gi.Degrade(1);

                    if(Gi.m_CurrentHP <= 1)
                    {
                        GameManager.GetInventoryComponent().DestroyGear(Gi.gameObject);
                    }
                }
            }
        }


        [HarmonyLib.HarmonyPatch(typeof(Panel_Log), "RefreshSelectedSkillDescriptionView")] // Once
        public static class Panel_Log_Initialize
        {
            public static void Postfix(Panel_Log __instance)
            {
                if (__instance.m_SkillImageLarge.mainTexture == null)
                {
                    //LoadedBundle is your custom bundle with all your stuff.
                    __instance.m_SkillImageLarge.mainTexture = LoadedBundle.LoadAsset<Texture2D>(__instance.m_SkillsDisplayList[__instance.m_SkillListSelectedIndex].m_Skill.m_SkillImage);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SkillsManager), "Awake")] // Once
        public static class SkillsManagerInitialize
        {
            public static void Postfix(SkillsManager __instance)
            {
                AddNewSkills(__instance);
            }
        }


        [HarmonyLib.HarmonyPatch(typeof(Panel_Diagnosis), "PostTreatment")] // Once
        public static class Panel_Diagnosis_PostTreatment
        {
            public static void Prefix(Panel_Diagnosis __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.DiagnosisDummy != null)
                {
                    MelonLogger.Msg("Cured other player");
                    DataStr.AffictionSync aff = new DataStr.AffictionSync();

                    aff.m_Location = (int)__instance.GetSelectedAffliction().m_Location;
                    aff.m_Type = (int)__instance.GetSelectedAfflictionType();
                    if (ShouldKeepAfterTreatment(__instance.GetSelectedAfflictionType()) == true)
                    {
                        __instance.GetSelectedAffliction().m_Remedy1Complete = false;
                        __instance.GetSelectedAffliction().m_RestHours = 1;
                    }
                    MyMod.SendCureAffliction(aff);
                    ImproveFirstAidSkill(aff.m_Type);
                }

                List<string> MakenzyDiagnosis = new List<string>();
                MakenzyDiagnosis.Add("PLAY_SNDVOSMMAC2150"); // Good to see you’re still breathing. I found some supplies that should help.
                MakenzyDiagnosis.Add("PLAY_SNDVOSMMACE422950_01"); // That doesn't look good...
                MakenzyDiagnosis.Add("PLAY_SNDVOSMMAC1770"); // What can I do?
                MakenzyDiagnosis.Add("PLAY_SNDVOSMMAC1820"); // You’re losing blood. Just stay awake. We’ll get there.
                MakenzyDiagnosis.Add("PLAY_SNDVOSMMAC1750"); // You ok?



                ///PLAY_SURVIVORDIAGNOSIS CHECK BEGIN

                if (GameManager.GetPlayerManagerComponent().m_VoicePersona == VoicePersona.Male) // If Makenzy
                {
                    List<string> MakenzyCure = new List<string>();
                    MakenzyCure.Add("PLAY_SNDVOSMMAC590"); // I know how hard it’s been...
                    MakenzyCure.Add("PLAY_SNDVOSMMAC1175UNCLESAM"); // Eh Thank you uncle Sam...
                    MakenzyCure.Add("PLAY_SNDVOSMMAC2250"); // Rest up. I’ll look around.
                    MakenzyCure.Add("PLAY_SNDVOSMMAC2300"); // I see you’re feeling better. Good.
                    MakenzyCure.Add("PLAY_SNDVOSMMAC1980"); // Here...take these. They should help with the pain.
                    System.Random rnd = new System.Random();
                    __instance.m_TreatmentSuccessVO = MakenzyCure[rnd.Next(MakenzyCure.Count)];
                }else{
                    __instance.m_TreatmentSuccessVO = "Play_SurvivorTreat"; // Astrid default
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "TreatAffliction")] // Once
        public static class PlayerManager_TreatAffliction
        {
            public static void Postfix(PlayerManager __instance, FirstAidItem firstAidItem, AfflictionType afflictionType, AfflictionBodyArea location, int afflictionId)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (afflictionType == AfflictionType.SprainedAnkle
                    || afflictionType == AfflictionType.SprainedWrist
                    || afflictionType == AfflictionType.SprainedWristMajor
                    || afflictionType == AfflictionType.SprainPain)

                {
                    return;
                }

                int Tier = GetFirstAidSkillTier();
                if (Tier == 4)
                {
                    ApplyTreatmentRegeneration(33, 0.25f, "Self Treatment");
                    GameManager.GetConditionComponent().AddHealth(5, DamageSource.FirstAid);
                } else if (Tier == 5)
                {
                    ApplyTreatmentRegeneration(35, 0.25f, "Self Treatment");
                    GameManager.GetConditionComponent().AddHealth(5, DamageSource.FirstAid);
                }
                IncressCustomSkill(CustomSkills.FirstAid, 1);
            }
        }

        public static string OverrideConditionOverTimeCause = "";
        public static string OverrideHeadacheCause = "";

        //[HarmonyLib.HarmonyPatch(typeof(Panel_Affliction), "GetCurrentAffliction")] // Once
        //public static class Panel_Affliction_GetCurrentAffliction
        //{
        //    public static void Postfix(ref Affliction __result)
        //    {
        //        if (__result.m_AfflictionType == AfflictionType.ConditionOverTimeBuff)
        //        {
        //            if (!string.IsNullOrEmpty(OverrideConditionOverTimeCause))
        //            {
        //                __result.m_Cause = OverrideConditionOverTimeCause;
        //            }
        //        } else if (__result.m_AfflictionType == AfflictionType.Headache)
        //        {
        //            if (!string.IsNullOrEmpty(OverrideHeadacheCause))
        //            {
        //                __result.m_Cause = OverrideHeadacheCause;
        //            }
        //        }
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(ConditionOverTimeBuff), "Apply")] // Once
        //public static class ConditionOverTimeBuff_Apply
        //{
        //    public static void Postfix(ConditionOverTimeBuff __instance, float normalizedValue)
        //    {
        //        if (MyMod.CrazyPatchesLogger == true)
        //        {
        //            StackTrace st = new StackTrace(new StackFrame(true));
        //            MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
        //            MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
        //        }
        //        OverrideConditionOverTimeCause = "";
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(EnergyBoost), "ApplyEnergyBoostExitEffects")] // Once
        //public static class EnergyBoost_ApplyEnergyBoostExitEffects
        //{
        //    public static void Postfix()
        //    {
        //        if (MyMod.CrazyPatchesLogger == true)
        //        {
        //            StackTrace st = new StackTrace(new StackFrame(true));
        //            MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
        //            MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
        //        }
        //        OverrideHeadacheCause = "";
        //    }
        //}
        [HarmonyLib.HarmonyPatch(typeof(Panel_Diagnosis), "RefreshRightPage")] // Once
        public static class Panel_Diagnosis_RefreshRightPage
        {
            public static void Postfix(Panel_Diagnosis __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                __instance.m_LabelCause.color = Color.gray;
                if (__instance.m_SelectedAffButton != null)
                {
                    AfflictionType AffType = __instance.GetSelectedAffliction().m_Definition.m_AfflictionType;
                    if (__instance.GetSelectedAffliction().m_RestHours == 1 && AffType != AfflictionType.Frostbite && AffType != AfflictionType.CabinFever)
                    {
                        NGUITools.SetActive(__instance.m_RightPageObject, false);
                        if (AffType == AfflictionType.InfectionRisk || Affliction.IsBadAffliction(AffType))
                        {
                            __instance.m_LabelCause.text = "AFFLICTION TREATED\nPATIENT NEEDS TIME TO HEAL";
                            __instance.m_LabelCause.color = new Color(0.25f, 1, 0);
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SpawnRegionManager), "Deserialize")] // Once
        public static class SpawnRegionManager_Deserialize
        {
            public static bool Prefix(SpawnRegionManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("[SpawnRegionManager] Deserialize");
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BaseAiManager), "Deserialize")] // Once
        public static class BaseAiManager_Deserialize
        {
            public static bool Prefix(BaseAiManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("[BaseAiManager] Deserialize");
                if (MyMod.AnimalsController == true)
                {
                    //MyMod.TestLoadAnimals(MyMod.level_name);
                }
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BaseAiManager), "Serialize")] // Once
        public static class BaseAiManager_Serialize
        {
            public static bool Prefix(BaseAiManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("[BaseAiManager] Serialize");
                if (MyMod.AnimalsController == true)
                {
                    //MyMod.TestSaveAnimals(MyMod.level_name);
                }
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BodyHarvestManager), "Serialize")] // Once
        public static class BodyHarvestManager_Serialize
        {
            public static bool Prefix(BodyHarvestManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("[BodyHarvestManager] Prefix Serialize");
                return false;
            }
            public static void Postfix(BodyHarvestManager __instance, ref string __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("[BodyHarvestManager] Postfix Serialize");
                BodyHarvestSaveList bodyHarvestSaveList = new BodyHarvestSaveList();
                bodyHarvestSaveList.m_SerializedBodyHarvests.Clear();
                for (int index = 0; index < BodyHarvestManager.m_BodyHarvestList.Count; ++index)
                {
                    BodyHarvest bh = BodyHarvestManager.m_BodyHarvestList[index];
                    if (bh != null && bh.gameObject.activeSelf && bh.enabled && bh.gameObject.GetComponent<GearItem>() == null && bh.m_Ravaged && bh.m_StartRavaged && bh.gameObject.GetComponent<Comps.AnimalCorpseObject>() == null)
                        bodyHarvestSaveList.m_SerializedBodyHarvests.Add(new BodyHarvestSaveData()
                        {
                            m_Position = bh.transform.position,
                            m_Rotation = bh.transform.rotation,
                            m_PrefabName = bh.gameObject.name,
                            m_Guid = Utils.GetGuidFromGameObject(bh.gameObject),
                            m_SerializedBodyHarvest = bh.Serialize()
                        });
                }
                __result = Utils.SerializeObject(bodyHarvestSaveList);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BodyHarvestManager), "Deserialize")] // Once
        public static class BodyHarvestManager_Deserialize
        {
            public static bool Prefix(BodyHarvestManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("[BodyHarvestManager] Prefix Deserialize");
                return false;
            }
            public static void Postfix(BodyHarvestManager __instance, string text)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MelonLogger.Msg("[BodyHarvestManager] Postfix Deserialize");
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }
                foreach (BodyHarvestSaveData serializedBodyHarvest in Utils.DeserializeObject<BodyHarvestSaveList>(text).m_SerializedBodyHarvests)
                {
                    GameObject gameObject = ObjectGuidManager.Lookup(serializedBodyHarvest.m_Guid);
                    BodyHarvest bh = null;
                    if (gameObject != null)
                    {
                        bh = gameObject.GetComponent<BodyHarvest>();
                    }
                    if (bh == null)
                    {
                        bh = BodyHarvestManager.InstantiateBodyHarvestGameObject(serializedBodyHarvest);
                        if (bh != null)
                            Utils.SetGuidForGameObject(bh.gameObject, serializedBodyHarvest.m_Guid);
                        else
                            continue;
                    } else
                    {
                        bh.transform.position = serializedBodyHarvest.m_Position;
                        bh.transform.rotation = serializedBodyHarvest.m_Rotation;
                    }
                    bh.Deserialize(serializedBodyHarvest.m_SerializedBodyHarvest, true);
                    BodyHarvestManager.MaybeRestoreCorpseState(bh);
                    BodyHarvestManager.MaybeRestoreCarcassSite(bh);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_BodyHarvest), "QuarterSuccessful")]
        internal static class Panel_BodyHarvest_QuarterSuccessful
        {
            private static void Prefix(Panel_BodyHarvest __instance)
            {
                if (MyMod.InOnline())
                {
                    if (__instance.m_BodyHarvest != null && __instance.m_BodyHarvest.gameObject != null && __instance.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>())
                    {
                        string GUID = __instance.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>().Get();
                        if (MyMod.iAmHost)
                        {
                            Shared.OnAnimalQuarted(GUID);
                            ServerSend.QUARTERANIMAL(0, GUID, true);
                        }
                        else if (MyMod.sendMyPosition)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.QUARTERANIMAL))
                            {
                                _packet.Write(GUID);
                                SendTCPData(_packet);
                            }
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_BodyHarvest), "SpawnQuarters")]
        internal static class Panel_BodyHarvest_SpawnQuarters
        {
            private static bool Prefix(Panel_BodyHarvest __instance)
            {
                if (MyMod.InOnline())
                {
                    int ofQuartersToSpawn = __instance.CalculateNumberOfQuartersToSpawn();
                    BodyHarvest[] quarterBodyHarvestArray = new BodyHarvest[ofQuartersToSpawn];
                    float num1 = (float)(-0.5 * (double)ofQuartersToSpawn * 0.259999990463257);
                    for (int index = 0; index < ofQuartersToSpawn; ++index)
                    {
                        GameObject quarterObjectPrefab = __instance.GetQuarterObjectPrefab();
                        if (!quarterObjectPrefab)
                            return false;
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(quarterObjectPrefab);
                        if (!gameObject)
                            return false;
                        GearItem component1 = gameObject.GetComponent<GearItem>();
                        component1.SkipSpawnChanceRollInitialDecayAndAutoEvolve();
                        BodyHarvest component2 = gameObject.GetComponent<BodyHarvest>();
                        component2.SetAllowDecay(true);
                        gameObject.name = quarterObjectPrefab.name;
                        
                        float num2 = UnityEngine.Random.Range(-0.2f * __instance.m_BodyHarvest.m_QuarterPrefabSpawnAngle, 0.2f * __instance.m_BodyHarvest.m_QuarterPrefabSpawnAngle);
                        float num3 = num1 + __instance.m_BodyHarvest.m_QuarterPrefabSpawnAngle * (float)index + num2;
                        float num4 = __instance.m_BodyHarvest.m_QuarterPrefabSpawnRadius + UnityEngine.Random.Range(-0.2f * __instance.m_BodyHarvest.m_QuarterPrefabSpawnRadius, 0.2f * __instance.m_BodyHarvest.m_QuarterPrefabSpawnRadius);
                        Vector3 vector3 = new Vector3(num4 * Mathf.Cos(num3), 1f, num4 * Mathf.Sin(num3));

                        component1.StickToGroundAndOrientOnSlope(__instance.m_BodyHarvest.transform.position + vector3, NavMeshCheck.IgnoreNavMesh, 0.1f);
                        __instance.RandomRotateQuarter(gameObject.transform);
                        component1.m_CurrentHP = __instance.m_BodyHarvest.GetCondition() / 100f * component1.m_MaxHP;
                        quarterBodyHarvestArray[index] = component2;
                    }
                    __instance.TransferMeatToQuarters(quarterBodyHarvestArray);

                    foreach (var item in quarterBodyHarvestArray)
                    {
                        if(item && item.gameObject && item.GetComponent<GearItem>())
                        {
                            MyMod.SendDropItem(item.gameObject.GetComponent<GearItem>(), 0, 0, false);
                        }
                    }

                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_BodyHarvest), "RandomRotateGutsOrHide")]
        internal static class Panel_BodyHarvest_RandomRotateGutsOrHide
        {
            private static void Postfix(Panel_BodyHarvest __instance, Transform transform)
            {
                if (MyMod.InOnline())
                {
                    if(transform != null && transform.gameObject != null)
                    {
                        if (transform.gameObject.GetComponent<GearItem>())
                        {
                            MyMod.SendDropItem(transform.gameObject.GetComponent<GearItem>(), 0, 0, false);
                        }
                    }
                }
            }
        }

        public static void Play3dAudioOnAnimal(string GUID, int soundID)
        {
            GameObject Animal = ObjectGuidManager.Lookup(GUID);
            if (Animal)
            {
                GameAudioManager.Play3DSound((uint)soundID, Animal);
            }
        }
        public static void Play3dAudioOnAnimal(string GUID, string soundID)
        {
            GameObject Animal = ObjectGuidManager.Lookup(GUID);
            if (Animal)
            {
                GameAudioManager.Play3DSound(soundID, Animal);
                //if (Animal.GetComponent<MyMod.AnimalActor>())
                //{
                //    MyMod.AnimalActor Actor = Animal.GetComponent<MyMod.AnimalActor>();
                //    if(soundID == Actor.m_EnterFleeModeAudio)
                //    {
                //        if(Actor.m_FleeAudioId == 0U)
                //        {
                //            Actor.m_FleeAudioId = GameAudioManager.Play3DSound(soundID, Animal);
                //        }
                //    }
                //}
            }
        }

        public static void SendAnimalAudio(int soundID, string GUID)
        {
            //MelonLogger.Msg("SendAnimalAudio "+ soundID);
            if (MyMod.iAmHost)
            {
                ServerSend.ANIMALAUDIO(0, soundID, GUID, MyMod.level_guid);
            }
            else if (MyMod.sendMyPosition)
            {
                using (Packet _packet = new Packet((int)ClientPackets.ANIMALAUDIO))
                {
                    _packet.Write(true);
                    _packet.Write(GUID);
                    _packet.Write(soundID);
                    SendTCPData(_packet);
                }
            }
        }
        public static void SendAnimalAudio(string soundID, string GUID)
        {
            //MelonLogger.Msg("SendAnimalAudio " + soundID);
            if (MyMod.iAmHost)
            {
                ServerSend.ANIMALAUDIO(0, soundID, GUID, MyMod.level_guid);
            }
            else if (MyMod.sendMyPosition)
            {
                using (Packet _packet = new Packet((int)ClientPackets.ANIMALAUDIO))
                {
                    _packet.Write(false);
                    _packet.Write(GUID);
                    _packet.Write(soundID);
                    SendTCPData(_packet);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BaseAi), "PlayStruggleAudio")] // Rarly
        internal static class BaseAi_PlayStruggleAudio
        {
            private static void Prefix(BaseAi __instance, string audioEventName)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == true)
                {
                    if(__instance.gameObject && __instance.gameObject.GetComponent<ObjectGuid>())
                    {
                        SendAnimalAudio(audioEventName, __instance.gameObject.GetComponent<ObjectGuid>().Get());
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BaseAi), "OnAudioEvent")] // Rarly
        internal static class BaseAi_OnAudioEvent
        {
            private static void Prefix(BaseAi __instance, string audioEventName)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == true)
                {
                    if (__instance.gameObject && __instance.gameObject.GetComponent<ObjectGuid>())
                    {
                        SendAnimalAudio(audioEventName, __instance.gameObject.GetComponent<ObjectGuid>().Get());
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BaseAi), "UpdateAttackingAudio")] // Often
        internal static class BaseAi_UpdateAttackingAudio
        {
            private static void Prefix(BaseAi __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == true)
                {
                    if (__instance.m_CurrentTarget || !__instance.m_CurrentTarget.IsPlayer() || (__instance.m_AttackingLoopAudioID != 0U || __instance.m_TimeInModeSeconds <= 1.5))
                    {

                    }else{
                        if (__instance.gameObject && __instance.gameObject.GetComponent<ObjectGuid>())
                        {
                            SendAnimalAudio(__instance.m_ChasingAudio, __instance.gameObject.GetComponent<ObjectGuid>().Get());
                        }
                    }
                }
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(GearItem), "DecayOverTODHours")] // Called often
        //internal static class GearItem_DecayOverTODHours
        //{
        //    private static bool Prefix(GearItem __instance, float deltaTODHours, float scale)
        //    {
        //        if (MyMod.CrazyPatchesLogger == true)
        //        {
        //            StackTrace st = new StackTrace(new StackFrame(true));
        //            MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
        //            MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
        //        }
        //        if (MyMod.InOnline() == true)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //}
        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "Spawn")] // Called often
        internal static class SpawnRegion_Spawn
        {
            private static bool Prefix(SpawnRegion __instance, WildlifeMode wildlifeMode)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == false)
                {
                    return true;
                }
                else
                {
                    return MyMod.AnimalsController;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(ConsoleManager), "spawn_common")] // Once
        internal static class ConsoleManager_Spawn
        {
            private static bool Prefix(ConsoleManager __instance, string prefabName, float dist)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == false)
                {
                    return true;
                }
                else
                {
                    return MyMod.AnimalsController;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(ConsoleManager), "CONSOLE_next_weather")] // Once
        internal static class ConsoleManager_CONSOLE_next_weather
        {
            private static bool Prefix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == false)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
            private static void Postfix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.iAmHost)
                {
                    Shared.ForceNextWeather();
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(ConsoleManager), "CONSOLE_next_weatherset")] // Once
        internal static class ConsoleManager_CONSOLE_next_weatherset
        {
            private static bool Prefix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == false)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
            private static void Postfix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.iAmHost)
                {
                    Shared.ForceNextWeatherSet();
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "MaybeReRollActive")] // Unknown
        internal static class SpawnRegion_MaybeReRollActive
        {
            private static bool Prefix(SpawnRegion __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline())
                {
                    if (__instance.gameObject != null && __instance.gameObject.GetComponent<Comps.SpawnRegionSimple>() != null)
                    {
                        __instance.gameObject.GetComponent<Comps.SpawnRegionSimple>().UpdateFromManager();
                    }
                }
                return MyMod.InOnline();
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Container), "OnOpenComplete")] // Once
        internal static class Container_OnOpenComplete
        {
            private static bool Prefix(Container __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == true)
                {
                    MelonLogger.Msg("Opening container, request fake container");
                    MyMod.OpenFakeContainer(__instance);
                    return false;
                }
                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Container), "OnDone")] // Once
        internal static class Panel_Container_Close
        {
            private static bool Prefix(Panel_Container __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                Container box = __instance.m_Container;
                if (MyMod.InOnline() == true)
                {
                    if (__instance.ShouldEnterSectionNav() && __instance.m_Container != null && !__instance.m_Container.m_FilterLocked)
                    {
                        __instance.EnterSectionNav();
                    }
                    else
                    {
                        if (__instance.m_DraggedItem != null)
                        {
                            __instance.m_DraggedItem.TryForceDrop(false);
                            __instance.FinishDragDrop();
                        }
                        MyMod.CloseFakeContainer(box);
                    }
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InstantiateItemAtPlayersFeet", new System.Type[] { typeof(GameObject), typeof(int) })] // Once
        internal static class PlayerManager_InstantiateItemAtPlayersFeet
        {
            private static void Postfix(GameObject prefab, int numUnits, GearItem __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == true)
                {
                    MyMod.SendDropItem(__result, 0, 0, false);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InstantiateItemAtPlayersFeet", new System.Type[] { typeof(string), typeof(int) })] // Once
        internal static class PlayerManager_InstantiateItemAtPlayersFeet2
        {
            private static void Postfix(string itemName, int numUnits, GearItem __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == true)
                {
                    if(itemName.Contains("GEAR_RevolverAmmoCasing") == true)
                    {
                        MyMod.SendDropItem(__result, 0, 0, false);
                    }
                }
            }
        }

        public static bool QuitOnSave = false;

        [HarmonyLib.HarmonyPatch(typeof(Panel_PauseMenu), "DoQuitGame")]  // Once
        internal static class Panel_PauseMenu_Quit
        {
            private static bool Prefix(Panel_PauseMenu __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InOnline() == true)
                {
                    //Application.Quit();
                    MyMod.DoPleaseWait("Saving","Saving before exit...");
                    QuitOnSave = true;
                    MyMod.QuitWithoutSaving = true;
                    if (MyMod.iAmHost)
                    {
                        MPSaveManager.SaveRecentStuff();
                    }
                    SaveGameSystem.SetAsyncEnabled(false);
                    GameManager.ForceSaveGame();
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GameManager), "OnApplicationFocus")] // Once
        internal static class GameManager_OnApplicationFocus
        {
            private static bool Prefix(bool focusStatus)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(OpenClose), "Open", new System.Type[] { typeof(bool), typeof(bool) })] // Once
        internal static class OpenClose_Open
        {
            private static void Postfix(OpenClose __instance, bool isImmediate, bool fromLink)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (isImmediate == true)
                {
                    return;
                }

                string _GUID = "";

                if (__instance.gameObject != null && __instance.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    _GUID = __instance.gameObject.GetComponent<ObjectGuid>().Get();
                }

                if (MyMod.InOnline() == true)
                {
                    if (MyMod.iAmHost == true)
                    {
                        Shared.ChangeOpenableThingState(MyMod.level_guid, _GUID, true);
                    }else{
                        MyMod.SendOpenableThing(MyMod.level_guid, _GUID, true);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(OpenClose), "Close", new System.Type[] { typeof(bool), typeof(bool) })] // Once
        internal static class OpenClose_Close
        {
            private static void Postfix(OpenClose __instance, bool isImmediate, bool fromLink)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (isImmediate == true)
                {
                    return;
                }

                string _GUID = "";

                if (__instance.gameObject != null && __instance.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    _GUID = __instance.gameObject.GetComponent<ObjectGuid>().Get();
                }

                if (MyMod.InOnline() == true)
                {
                    if (MyMod.iAmHost == true)
                    {
                        Shared.ChangeOpenableThingState(MyMod.level_guid, _GUID, false);
                    }else{
                        MyMod.SendOpenableThing(MyMod.level_guid, _GUID, false);
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GameManager), "LoadSceneWithLoadingScreen")] // Once
        private static class GameManager_LoadSceneWithLoadingScreen
        {
            private static void Prefix(ref string sceneName)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.InterloperHook == false && MyMod.OverridedSceneForSpawn == "")
                {
                    return;
                }

                if (MyMod.InterloperHook == true)
                {
                    MyMod.InterloperHook = false;
                    GameRegion startRegion = GameManager.m_StartRegion;
                    if (startRegion != GameRegion.RandomRegion && startRegion != GameRegion.FutureRegion)
                    {
                        sceneName = GameManager.m_StartRegion.ToString();
                    }
                }
                if (MyMod.OverridedSceneForSpawn != "")
                {
                    sceneName = MyMod.OverridedSceneForSpawn;
                    MyMod.OverridedSceneForSpawn = "";
                }
                MelonLogger.Msg(ConsoleColor.Magenta, "[LoadSceneWithLoadingScreen] " + sceneName);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "TeleportPlayerAfterSceneLoad")] // Once
        private static class PlayerManager_TeleportPlayerAfterSceneLoad
        {
            private static void Postfix(PlayerManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.OverridedPositionForSpawn != Vector3.zero)
                {
                    __instance.TeleportPlayer(MyMod.OverridedPositionForSpawn, GameManager.GetPlayerTransform().rotation);
                    MelonLogger.Msg(ConsoleColor.Magenta, "[TeleportPlayerAfterSceneLoad] X " + MyMod.OverridedPositionForSpawn.x+" Y "+ MyMod.OverridedPositionForSpawn.y+" Z "+ MyMod.OverridedPositionForSpawn.z);
                    MyMod.OverridedPositionForSpawn = Vector3.zero;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GameManager), "Start")]
        internal static class GameManager_Start
        {
            private static void Postfix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MyMod.UpdateSceneGUID();
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GameManager), "LoadSlotOnStart")]
        internal static class GameManager_LoadSlotOnStart
        {
            private static void Postfix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MyMod.UpdateSceneGUID();
            }
        }

        //[HarmonyPatch(typeof(GearItem), "RollSpawnChance")]
        //private static class GearItem_RollSpawnChance
        //{
        //    private static bool Prefix(GearItem __instance)
        //    {
        //        MelonLogger.Msg(ConsoleColor.Blue, "[GearItem][RollSpawnChance] " + __instance.gameObject.name + " Gear trying to roll chance!");
        //        if (__instance.m_BeenInPlayerInventory || __instance.m_BeenInContainer || __instance.m_RolledSpawnChance)
        //        {
        //            MelonLogger.Msg(ConsoleColor.Blue, "[GearItem][RollSpawnChance] " + __instance.gameObject.name + " Refuse to do so ");
        //            return false;
        //        }
        //        __instance.m_RolledSpawnChance = true;
        //        if (Utils.Approximately(__instance.m_SpawnChance, 100f, 0.0001f))
        //        {
        //            MelonLogger.Msg(ConsoleColor.Blue, "[GearItem][RollSpawnChance] " + __instance.gameObject.name + " Refuse to do so ");
        //            return false;
        //        }

        //        float spawnChance = __instance.m_SpawnChance * GameManager.GetExperienceModeManagerComponent().GetGearSpawnChanceScale();
        //        bool ok = GameManager.RollSpawnChance(__instance.gameObject, spawnChance);
        //        MelonLogger.Msg(ConsoleColor.Blue, "[GearItem][RollSpawnChance] "+ __instance.gameObject.name + " Rolling result: ShouldSpawn = "+ ok);
        //        return false;
        //    }
        //}


        //[HarmonyLib.HarmonyPatch(typeof(GearItem), "Deserialize")]
        //public class GearItem_LoadDebug
        //{
        //    public static bool Prefix(GearItem __instance, string text, bool applyPositioningFix = true)
        //    {
        //        MelonLogger.Msg(ConsoleColor.Blue, "[GearItem][Deserialize] Starting...");
        //        if (text == null)
        //            return false;
        //        GearItemSaveDataProxy itemSaveDataProxy = Utils.DeserializeObject<GearItemSaveDataProxy>(text);
        //        MelonLogger.Msg(ConsoleColor.Green, "[GearItem][Deserialize] itemSaveDataProxy.m_NormalizedCondition " + itemSaveDataProxy.m_NormalizedCondition);
        //        MelonLogger.Msg(ConsoleColor.Green, "[GearItem][Deserialize] itemSaveDataProxy.m_CurrentHPProxy " + itemSaveDataProxy.m_CurrentHPProxy);
        //        MelonLogger.Msg(ConsoleColor.Green, "[GearItem][Deserialize] itemSaveDataProxy.m_MaxHP" + __instance.m_MaxHP);
        //        bool flag1 = false;
        //        if (float.IsNaN((float)itemSaveDataProxy.m_Position.x) || float.IsNaN((float)itemSaveDataProxy.m_Position.y) || float.IsNaN((float)itemSaveDataProxy.m_Position.z))
        //        {
        //            itemSaveDataProxy.m_Position = GameManager.GetPlayerTransform().position;
        //            flag1 = true;
        //        }
        //        if (float.IsNaN((float)itemSaveDataProxy.m_Rotation.x) || float.IsNaN((float)itemSaveDataProxy.m_Rotation.y) || (float.IsNaN((float)itemSaveDataProxy.m_Rotation.z) || float.IsNaN((float)itemSaveDataProxy.m_Rotation.w)))
        //            itemSaveDataProxy.m_Rotation = Quaternion.identity;
        //        __instance.transform.position = itemSaveDataProxy.m_Position;
        //        __instance.transform.rotation = itemSaveDataProxy.m_Rotation;
        //        Physics.SyncTransforms();
        //        __instance.m_InstanceID = itemSaveDataProxy.m_InstanceIDProxy;
        //        __instance.m_CurrentHP = itemSaveDataProxy.m_CurrentHPProxy;
        //        __instance.m_CurrentHP = Mathf.Clamp(__instance.m_CurrentHP, 0.0f, __instance.m_MaxHP);
        //        MelonLogger.Msg(ConsoleColor.Yellow, "[GearItem][Deserialize] __instance.m_CurrentHP (after clamp) " + __instance.m_CurrentHP);
        //        if (!Utils.IsZero(itemSaveDataProxy.m_NormalizedCondition, 0.0001f))
        //            __instance.m_CurrentHP = itemSaveDataProxy.m_NormalizedCondition * __instance.m_MaxHP;
        //            MelonLogger.Msg(ConsoleColor.DarkYellow, "[GearItem][Deserialize] __instance.m_CurrentHP (After normalizing) " + __instance.m_CurrentHP);
        //        __instance.m_BeenInPlayerInventory = itemSaveDataProxy.m_BeenInPlayerInventoryProxy;
        //        __instance.m_BeenInContainer = itemSaveDataProxy.m_BeenInContainerProxy;
        //        __instance.m_BeenInspected = itemSaveDataProxy.m_BeenInspectedProxy;
        //        __instance.m_ItemLooted = itemSaveDataProxy.m_ItemLootedProxy;
        //        __instance.m_HasBeenOwnedByPlayer = itemSaveDataProxy.m_HasBeenOwnedByPlayer;
        //        __instance.m_RolledSpawnChance = itemSaveDataProxy.m_RolledSpawnChanceProxy;
        //        __instance.m_WornOut = itemSaveDataProxy.m_WornOut;
        //        __instance.m_HarvestedByPlayer = itemSaveDataProxy.m_HarvestedByPlayer;
        //        __instance.m_IsInSatchel = itemSaveDataProxy.m_IsInSatchel;
        //        __instance.m_SatchelIndex = itemSaveDataProxy.m_SatchelIndex;
        //        __instance.m_LockedInContainer = itemSaveDataProxy.m_LockedInContainer;
        //        __instance.m_NonInteractive = itemSaveDataProxy.m_NonInteractive;
        //        __instance.DeserializeRigidBody(itemSaveDataProxy.m_RigidBodySerialized);
        //        if (__instance.m_StackableItem && itemSaveDataProxy.m_StackableItemSerialized != null)
        //            __instance.m_StackableItem.Deserialize(itemSaveDataProxy.m_StackableItemSerialized);
        //        if (__instance.m_FoodItem && itemSaveDataProxy.m_FoodItemSerialized != null)
        //        {
        //            __instance.m_FoodItem.Deserialize(itemSaveDataProxy.m_FoodItemSerialized);
        //            if (__instance.m_FoodItem.WasHarvested())
        //                __instance.m_HarvestedByPlayer = true;
        //        }
        //        if (__instance.m_LiquidItem && itemSaveDataProxy.m_LiquidItemSerialized != null)
        //            __instance.m_LiquidItem.Deserialize(itemSaveDataProxy.m_LiquidItemSerialized);
        //        if (__instance.m_FlareItem && itemSaveDataProxy.m_FlareItemSerialized != null)
        //            __instance.m_FlareItem.Deserialize(itemSaveDataProxy.m_FlareItemSerialized);
        //        if (__instance.m_FlashlightItem && itemSaveDataProxy.m_FlashlightItemSerialized != null)
        //            __instance.m_FlashlightItem.Deserialize(itemSaveDataProxy.m_FlashlightItemSerialized);
        //        if (__instance.m_KeroseneLampItem && itemSaveDataProxy.m_KeroseneLampItemSerialized != null)
        //            __instance.m_KeroseneLampItem.Deserialize(itemSaveDataProxy.m_KeroseneLampItemSerialized);
        //        if (__instance.m_ClothingItem && itemSaveDataProxy.m_ClothingItemSerialized != null)
        //            __instance.m_ClothingItem.Deserialize(itemSaveDataProxy.m_ClothingItemSerialized);
        //        if (__instance.m_GunItem && itemSaveDataProxy.m_WeaponItemSerialized != null)
        //            __instance.m_GunItem.Deserialize(itemSaveDataProxy.m_WeaponItemSerialized);
        //        if (__instance.m_WaterSupply && itemSaveDataProxy.m_WaterSupplySerialized != null)
        //            __instance.m_WaterSupply.Deserialize(itemSaveDataProxy.m_WaterSupplySerialized);
        //        if (__instance.m_Bed && itemSaveDataProxy.m_BedSerialized != null)
        //            __instance.m_Bed.Deserialize(itemSaveDataProxy.m_BedSerialized);
        //        if (__instance.m_SmashableItem && itemSaveDataProxy.m_SmashableItemSerialized != null)
        //            __instance.m_SmashableItem.Deserialize(itemSaveDataProxy.m_SmashableItemSerialized);
        //        if (__instance.m_MatchesItem && itemSaveDataProxy.m_MatchesItemSerialized != null)
        //            __instance.m_MatchesItem.Deserialize(itemSaveDataProxy.m_MatchesItemSerialized);
        //        if (__instance.m_SnareItem && itemSaveDataProxy.m_SnareItemSerialized != null)
        //            __instance.m_SnareItem.Deserialize(itemSaveDataProxy.m_SnareItemSerialized);
        //        if (itemSaveDataProxy.m_InProgressItemSerialized != null)
        //        {
        //            if (__instance.m_InProgressCraftItem == null)
        //                __instance.m_InProgressCraftItem = __instance.gameObject.AddComponent<InProgressCraftItem>();
        //            __instance.m_InProgressCraftItem.Deserialize(itemSaveDataProxy.m_InProgressItemSerialized);
        //        }
        //        if (__instance.m_TorchItem && itemSaveDataProxy.m_TorchItemSerialized != null)
        //            __instance.m_TorchItem.Deserialize(itemSaveDataProxy.m_TorchItemSerialized);
        //        if (__instance.m_EvolveItem && itemSaveDataProxy.m_EvolveItemSerialized != null)
        //            __instance.m_EvolveItem.Deserialize(itemSaveDataProxy.m_EvolveItemSerialized);
        //        if (__instance.m_BodyHarvest && itemSaveDataProxy.m_BodyHarvestSerialized != null)
        //            __instance.m_BodyHarvest.Deserialize(itemSaveDataProxy.m_BodyHarvestSerialized, true);
        //        if (__instance.m_CookingPotItem && itemSaveDataProxy.m_CookingPotItemSerialized != null)
        //            __instance.m_CookingPotItem.Deserialize(itemSaveDataProxy.m_CookingPotItemSerialized);
        //        if (itemSaveDataProxy.m_ObjectGuidSerialized != null)
        //        {
        //            if (__instance.m_ObjectGuid == null)
        //                __instance.m_ObjectGuid = __instance.GetComponent<ObjectGuid>();
        //            if (__instance.m_ObjectGuid == null)
        //                __instance.m_ObjectGuid = __instance.gameObject.AddComponent<ObjectGuid>();
        //            ObjectGuidManager.UnRegisterGuid(itemSaveDataProxy.m_ObjectGuidSerialized);
        //            __instance.m_ObjectGuid.Set(itemSaveDataProxy.m_ObjectGuidSerialized);
        //        }
        //        if (itemSaveDataProxy.m_MissionObjectSerialized != null)
        //        {
        //            __instance.AssumeMissionObjectResponsibility(itemSaveDataProxy.m_MissionObjectSerialized);
        //            __instance.MaybeDestroyGearItemAfterMission();
        //        }
        //        if (__instance.m_ResearchItem && itemSaveDataProxy.m_ResearchItemSerialized != null)
        //            __instance.m_ResearchItem.Deserialize(itemSaveDataProxy.m_ResearchItemSerialized);
        //        if (__instance.m_StoneItem)
        //            __instance.m_StoneItem.SetThrown(itemSaveDataProxy.m_StoneItemThrown);
        //        if (itemSaveDataProxy.m_OwnershipOverrideSerialized != null)
        //        {
        //            __instance.m_OwnershipOverrideItem = __instance.gameObject.AddComponent<OwnershipOverride>();
        //            __instance.m_OwnershipOverrideItem.Deserialize(itemSaveDataProxy.m_OwnershipOverrideSerialized);
        //        }
        //        if (__instance.m_FoodWeight && (double)itemSaveDataProxy.m_WeightKG > 0.0)
        //        {
        //            __instance.m_WeightKG = itemSaveDataProxy.m_WeightKG;
        //            if (__instance.m_FoodItem)
        //                __instance.m_FoodItem.m_CaloriesTotal = __instance.m_WeightKG * __instance.m_FoodWeight.m_CaloriesPerKG;
        //        }
        //        if (__instance.m_Inspect && itemSaveDataProxy.m_InspectSerialized != null)
        //            __instance.m_Inspect.Deserialize(itemSaveDataProxy.m_InspectSerialized);
        //        if (__instance.m_PowderItem && itemSaveDataProxy.m_PowderItemSerialized != null)
        //            __instance.m_PowderItem.Deserialize(itemSaveDataProxy.m_PowderItemSerialized);
        //        if (__instance.m_Millable && itemSaveDataProxy.m_MillableSerialized != null)
        //            __instance.m_Millable.Deserialize(itemSaveDataProxy.m_MillableSerialized);
        //        if (__instance.m_SprayPaintCan && itemSaveDataProxy.m_SprayPaintCanSerialized != null)
        //            __instance.m_SprayPaintCan.Deserialize(itemSaveDataProxy.m_SprayPaintCanSerialized);
        //        MelonLogger.Msg(ConsoleColor.Yellow, "[GearItem][Deserialize] __instance.m_CurrentHP (Before TOD & DecayOverTODHours) " + __instance.m_CurrentHP);
        //        __instance.InitializeLastUpdatedTodHours();
        //        MelonLogger.Msg(ConsoleColor.DarkYellow, "[GearItem][Deserialize] __instance.m_CurrentHP (After TOD Init) " + __instance.m_CurrentHP);
        //        MelonLogger.Msg(ConsoleColor.DarkYellow, "[GearItem][Deserialize] __instance.m_DecayScalar " + __instance.m_DecayScalar);
        //        __instance.DecayOverTODHours(__instance.m_LastUpdatedTODHours - itemSaveDataProxy.m_HoursPlayed, __instance.m_DecayScalar);
        //        MelonLogger.Msg(ConsoleColor.DarkYellow, "[GearItem][Deserialize] __instance.m_CurrentHP (After DecayOverTODHours) " + __instance.m_CurrentHP);
        //        __instance.m_InitialDecayApplied = true;
        //        if (__instance.IsUndegradableAccelerant())
        //            __instance.m_CurrentHP = __instance.m_MaxHP;
        //        if (__instance.m_StackableItem == null && itemSaveDataProxy.m_StackableItemSerialized != null)
        //        {
        //            int unitsProxy = Utils.DeserializeObject<StackableItemSaveDataProxy>(itemSaveDataProxy.m_StackableItemSerialized).m_UnitsProxy;
        //            for (int index = 1; index < unitsProxy; ++index)
        //            {
        //                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.gameObject);
        //                DisableObjectForXPMode.RemoveDisabler(gameObject);
        //                gameObject.name = __instance.gameObject.name;
        //                if (__instance.m_InPlayerInventory)
        //                {
        //                    GameManager.GetInventoryComponent().AddGear(gameObject);
        //                    flag1 = false;
        //                }
        //                else if (__instance.IsInsideContainer())
        //                {
        //                    __instance.AddGearToContainer(__instance, (GearItem)gameObject.GetComponent<GearItem>());
        //                    flag1 = false;
        //                }
        //                else
        //                {
        //                    gameObject.transform.position = __instance.gameObject.transform.position;
        //                    gameObject.transform.rotation = __instance.gameObject.transform.rotation;
        //                }
        //            }
        //        }
        //        if (itemSaveDataProxy.m_GearItemSaveVersion < 1 && (__instance.name == "GEAR_Cloth" && __instance.m_StackableItem == null))
        //        {
        //            StackableItem stackableItem = __instance.m_StackableItem;
        //            stackableItem.m_Units = stackableItem.m_Units;
        //        }
        //        __instance.m_PlacePointGuid = itemSaveDataProxy.m_PlacePointGuidSerialized;
        //        __instance.m_PlacePointName = itemSaveDataProxy.m_PlacePointNameSerialized;
        //        __instance.m_HasBeenEquippedAndUsed = itemSaveDataProxy.m_HasBeenEquippedAndUsed;
        //        bool flag2 = __instance.MaybePlaceGear();
        //        if (flag1)
        //        {
        //            InaccessibleGearContainer.AddGearToNearestContainer(__instance);
        //        }
        //        else
        //        {
        //            applyPositioningFix &= __instance.gameObject.name != "GEAR_Tinder";
        //            if (applyPositioningFix && itemSaveDataProxy.m_GearItemSaveVersion < 4 && !flag2 && (__instance.m_BeenInPlayerInventory || GameManager.GetWeatherComponent().IsIndoorEnvironment()))
        //                __instance.MaybeAddToLostAndFound();
        //        }
        //        if (__instance.m_StartHasBeenCalled)
        //            return false;
        //        __instance.ManualStart();

        //        return false;
        //    }
        //}
        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "Start")] // Once
        private static class SpawnRegion_Start
        {
            private static void Postfix(SpawnRegion __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                if(MyMod.InOnline())
                {
                    __instance.SetActive(false);
                }
                if(__instance.gameObject != null && __instance.gameObject.GetComponent<Comps.SpawnRegionSimple>() == null)
                {
                    Comps.SpawnRegionSimple SRS = __instance.gameObject.AddComponent<Comps.SpawnRegionSimple>();
                    SRS.m_Region = __instance;
                    SRS.m_GUID = __instance.gameObject.GetComponent<ObjectGuid>().Get();
                }                
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "UpdateFromManager")] // Once
        private static class SpawnRegion_UpdateFromManager
        {
            private static void Postfix(SpawnRegion __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                if (MyMod.InOnline())
                {
                    __instance.SetActive(false);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "DisableMainMenuButtonColliders")] // Once
        private static class Panel_MainMenu_DisableMainMenuButtonColliders
        {
            private static void Postfix(Panel_MainMenu __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MenuChange.ChangeMenuItems("Original");
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(WildlifeItem), "OnReleased")] // Once
        private static class WildlifeItem_OnReleased
        {
            internal static bool Prefix(WildlifeItem __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                if (MyMod.InOnline())
                {
                    if(MyMod.AnimalsController == true)
                    {
                        MyMod.OnReleaseRabbit(ClientUser.myId);
                    }else{
                        if (MyMod.iAmHost)
                        {
                            ServerSend.RELEASERABBIT(0, MyMod.level_guid);
                        }else if (MyMod.sendMyPosition)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.RELEASERABBIT))
                            {
                                _packet.Write(MyMod.level_guid);
                                SendTCPData(_packet);
                            }
                        }
                    }
                    __instance.ResetControls();
                    if (__instance.gameObject)
                    {
                        
                        UnityEngine.Object.Destroy(__instance.gameObject);
                    }
                    return false;
                }
                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(StoneItem), "OnCollisionEnter")] // Once
        private static class StoneItem_OnCollisionEnter
        {
            private static bool Prefix(StoneItem __instance, Collision collisionInfo)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                if (MyMod.InOnline())
                {
                    Il2CppStructArray<ContactPoint> contacts = collisionInfo.contacts;
                    for (int index = 0; index < contacts.Length; ++index)
                    {
                        ContactPoint contact = contacts[index];
                        Comps.AnimalActor Actor =  contact.otherCollider.GetComponentInParent<Comps.AnimalActor>();
                        if (Actor != null)
                        {
                            if (Actor.gameObject.GetComponent<ObjectGuid>())
                            {
                                if (MyMod.sendMyPosition)
                                {
                                    using (Packet _packet = new Packet((int)ClientPackets.HITRABBIT))
                                    {
                                        _packet.Write(Actor.gameObject.GetComponent<ObjectGuid>().Get());
                                        _packet.Write(Actor.m_ClientController);
                                        SendTCPData(_packet);
                                    }
                                } else if (MyMod.iAmHost)
                                {
                                    ServerSend.HITRABBIT(Actor.m_ClientController, Actor.gameObject.GetComponent<ObjectGuid>().Get());
                                }
                            }
                        }
                    }
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "UseFPSMeshItem")] // Once
        private static class PlayerManager_UseFPSMeshItem
        {
            private static bool Prefix(PlayerManager __instance, GearItem gi, ref bool __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                int fpsMeshId = gi.GetFPSMeshID();
                if(fpsMeshId == -1)
                {
                    __result = false;
                    return false;
                }else{
                    if (GameManager.GetVpFPSCamera().CurrentWeaponID == fpsMeshId)
                    {
                        if(GameManager.GetPlayerManagerComponent().m_ItemInHands != null)
                        {
                            if(GameManager.GetPlayerManagerComponent().m_ItemInHands.m_GearName == gi.m_GearName)
                            {
                                __result = false;
                                return false;
                            }else{
                                __instance.EquipItem(gi, false);
                                GameManager.GetPlayerAnimationComponent().SetItemEquippedByPlayer(true);
                                __result = true;
                                return false;
                            }
                        }
                    }
                }
                
                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "UnequipItemInHands")] // Once
        private static class PlayerManager_UnequipItemInHands
        {
            private static bool Prefix(PlayerManager __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                if (!MyMod.VanilaRadio && __instance.m_ItemInHands && __instance.m_ItemInHands.GetFPSMeshID() == (int)FPSMeshID.HandledShortwave)
                {
                    GameManager.GetPlayerManagerComponent().UnequipItemInHandsSkipAnimation();
                    return false;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "EquipItem")] // Once
        private static class PlayerManager_EquipItem
        {
            private static void Prefix(PlayerManager __instance, GearItem gi)
            {
                if (MyMod.IsUserGeneratedHandItem(gi.m_GearName) || gi.m_GearName == "GEAR_SCNote")
                {
                    StoneItem SI = gi.gameObject.AddComponent<StoneItem>();
                    SI.m_RigidBody = gi.gameObject.AddComponent<Rigidbody>();
                    SI.m_RigidBody.isKinematic = true;
                    SI.m_GearItem = gi;
                    SI.m_CanThrow = false;
                    gi.m_StoneItem = SI;

                    if (gi.m_ObjectGuid && !string.IsNullOrEmpty(gi.m_ObjectGuid.m_Guid))
                    {
                        if (gi.m_GearName == "GEAR_SCPhoto")
                        {
                            if (MyMod.ViewModelPhoto)
                            {
                                MyMod.ViewModelPhoto.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = gi.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture;
                            }
                        }
                        else if (gi.m_GearName == "GEAR_SCMapPiece")
                        {
                            if (MyMod.ViewModelMap)
                            {
                                MyMod.ViewModelMap.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = gi.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture;
                            }
                        }
                    }
                }
            }
            private static void Postfix(PlayerManager __instance, GearItem gi)
            {
                if (MyMod.IsUserGeneratedHandItem(gi.m_GearName) || gi.m_GearName == "GEAR_SCNote")
                {
                    StoneItem SI = gi.gameObject.GetComponent<StoneItem>();
                    if (SI)
                    {
                        if (SI.m_RigidBody)
                        {
                            UnityEngine.Object.Destroy(SI.m_RigidBody);
                        }
                        UnityEngine.Object.Destroy(SI);
                    }
                    gi.m_StoneItem = null;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(ItemDescriptionPage), "GetEquipButtonLocalizationId")] // Once
        private static class ItemDescriptionPage_GetEquipButtonLocalizationId
        {
            private static void Postfix(ItemDescriptionPage __instance, GearItem gi, ref string __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                if (gi != null)
                {
                    if (MyMod.IsCustomHandItem(gi.m_GearName) || MyMod.IsUserGeneratedHandItem(gi.m_GearName) || gi.m_GearName == "GEAR_SCHeatPackB" || gi.m_GearName == "GEAR_SCPresent")
                    {
                        __result = "GAMEPLAY_Use";
                    } else if (gi.m_GearName == "GEAR_SCNote")
                    {
                        __result = "GAMEPLAY_Read";
                    }
                    //}else if (Shared.IsLocksmithItem(gi.m_GearName))
                    //{
                    //    __result = "GAMEPLAY_Place";
                    //}
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(ItemDescriptionPage), "CanDrop")] // Once
        private static class ItemDescriptionPage_CanDrop
        {
            private static void Postfix(ItemDescriptionPage __instance, GearItem gi, ref bool __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                if (gi != null)
                {
                    if(gi.m_GearName == "GEAR_SCHeatPack" || gi.m_GearName == "GEAR_SCHeatPackC" || gi.m_GearName == "GEAR_SCEmergencyRation" || gi.m_GearName == "GEAR_SCLiquidLead")
                    {
                        __result = false;
                    }
                }
            }
        }

        public static bool HaveReadFakeNote = false;

        public static void DisplayNote(GearItem gi)
        {
            HaveReadFakeNote = true;
            string NoteText = "Gived with console notes doesnt store any text.";

            if (gi.GetComponent<ObjectGuid>() != null)
            {
                NoteText = Shared.DecompressString(gi.GetComponent<ObjectGuid>().Get());
            }
            Panel_HUD Panel = InterfaceManager.m_Panel_HUD;

            Panel.m_CollectibleNoteObject.SetActive(true);
            Panel.m_CollectibleNoteObjectText.text = NoteText;
            Panel.m_CollectibleNoteObjectText.alignment = NGUIText.Alignment.Center;
            Panel.m_CollectibleNoteObjectTitle.text = gi.m_DisplayName;
            Panel.m_InspectMode_StandardElementsParent.SetActive(false);
            Panel.m_InspectMode_InventoryStatusSprite.gameObject.SetActive(false);
            Panel.m_InspectMode_Equip.gameObject.SetActive(false);
            Panel.m_InspectMode_Take.gameObject.SetActive(false);
            Panel.m_InspectMode_PutBack.gameObject.SetActive(false);
            if (Panel.m_CollectibleNoteObjectText.height > Panel.m_ScrollPanelHeight)
            {
                Panel.m_CollectibleNoteObjectTitle.transform.localPosition = new Vector3(0.0f, Panel.m_TitleY, 0.0f);
                Panel.m_CollectibleNoteScrollObject.SetActive(true);
                Panel.m_CollectibleNoteScrollView.Scroll(100f);
                UIButton componentInChildren = Panel.m_CollectibleNoteScrollObject.GetComponentInChildren<UIButton>();
                if (componentInChildren)
                {
                    componentInChildren.defaultColor = Panel.m_SliderDefaultColor;
                }
            } else
            {
                Panel.m_CollectibleNoteObjectText.transform.position = new Vector3(Panel.m_CollectibleNoteObjectText.transform.position.x, 0.0f, 0.0f);
                Panel.m_CollectibleNoteObjectTitle.transform.localPosition = new Vector3(0.0f, 0.5f * (float)Panel.m_CollectibleNoteObjectText.height + Panel.m_TitleYOffset, 0.0f);
                Panel.m_CollectibleNoteScrollObject.SetActive(false);
            }
            gi.gameObject.SetActive(false);
            if (Panel.m_HudDisplayMode == HudDisplayMode.Off)
            {
                NGUITools.SetActive(Panel.m_NonEssentialHud, true);
            }
            Panel.UpdateCollectibleNote();
        }


        [HarmonyLib.HarmonyPatch(typeof(ItemDescriptionPage), "OnEquip")] // Once
        private static class ItemDescriptionPage_OnEquip
        {
            private static void Postfix(ItemDescriptionPage __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MyMod.ProcessGivingItem(true);

                GearItem gi = InterfaceManager.m_Panel_Inventory.GetCurrentlySelectedGearItem();

                if (gi != null)
                {
                    if (gi.m_GearName == "GEAR_SCHeatPackB")
                    {
                        OpenHeatPack(gi);
                    }else if(gi.m_GearName == "GEAR_SCPresent")
                    {
                        OpenPresent(gi);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_HUD), "OnCollectibleNoteReadingClickBack")] // Once
        private static class Panel_HUD_OnCollectibleNoteReadingClickBack
        {
            private static void Postfix(Panel_HUD __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                if (HaveReadFakeNote)
                {
                    __instance.m_InspectMode_StandardElementsParent.SetActive(false);
                    __instance.m_InspectMode_InventoryStatusSprite.gameObject.SetActive(false);
                    __instance.m_InspectMode_Equip.gameObject.SetActive(false);
                    __instance.m_InspectMode_Take.gameObject.SetActive(false);
                    __instance.m_InspectMode_PutBack.gameObject.SetActive(false);
                    HaveReadFakeNote = false;
                    GameManager.GetPlayerManagerComponent().m_Gear = null;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerAnimation), "OnAnimationEvent_Generic_HiddenComplete")] // Once
        private static class PlayerAnimation_OnAnimationEvent_Generic_HiddenComplete
        {
            private static void Postfix(PlayerAnimation __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                MyMod.RetakeItem();
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerAnimation), "OnAnimationEvent_Generic_Throw_ReleaseItem")] // Once
        private static class PlayerAnimation_OnAnimationEvent_Generic_Throw_ReleaseItem
        {
            private static void Postfix(PlayerAnimation __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.ShouldPerformAttack)
                {
                    MyMod.ShouldPerformAttack = false;
                    MyMod.MeleeHit();
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerAnimation), "UpdateAnimatorSpeed")] // Once
        private static class PlayerAnimation_UpdateAnimatorSpeed
        {
            private static bool Prefix(PlayerAnimation __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                bool Pass = true;

                if (MyMod.ShouldReEquipFaster || MyMod.ShouldPerformAttack)
                {
                    Pass = false;
                }

                if (__instance.GetState() == PlayerAnimation.State.Showing)
                {
                    MyMod.ShouldReEquipFaster = false;
                }

                return Pass;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Utils), "GetInventoryIconTexture")] // A lot
        private static class Utils_GetInventoryIconTexture
        {
            private static void Postfix(Utils __instance, GearItem gi, ref Texture2D __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (!MyMod.VanilaRadio && gi.m_GearName == "GEAR_HandheldShortwave")
                {
                    __result = MyMod.LoadedBundle.LoadAsset<Texture2D>("ico_GearItem__HandheldShortwave");
                }
                if (gi.m_GearName == "GEAR_Shovel")
                {
                    __result = MyMod.LoadedBundle.LoadAsset<Texture2D>("ico_GearItem__Shovel");
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Utils), "GetInventoryGridIconTexture")] // A lot
        private static class Utils_GetInventoryGridIconTexture
        {
            private static void Postfix(Utils __instance, string name, ref Texture2D __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (!MyMod.VanilaRadio && name == "ico_GearItem__HandheldShortwave")
                {
                    __result = MyMod.LoadedBundle.LoadAsset<Texture2D>("ico_GearItem__HandheldShortwave");
                }
                if (name == "ico_GearItem__Shovel")
                {
                    __result = MyMod.LoadedBundle.LoadAsset<Texture2D>("ico_GearItem__Shovel");
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(StartGear), "AddAllToInventory")]
        private static class StartGear_AddAllToInventory
        {
            private static void Postfix()
            {
                GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_HandheldShortwave");
                GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_SCHeatPack");
                GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_SCEmergencyRation");
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Log), "UpdateMissionsPage")]
        private static class Panel_Log_UpdateMissionsPage
        {
            private static void Postfix(Panel_Log __instance)
            {
                if(MyMod.CurrentCustomChalleng.m_Started)
                {
                    __instance.m_TimerObject.SetActive(MyMod.CurrentCustomChalleng.m_Time != 0);
                    __instance.m_MissionNameLabel.text = MyMod.CurrentChallengeRules.m_Name;
                    __instance.m_MissionNameHeaderLabel.text = MyMod.CurrentChallengeRules.m_Name;
                    __instance.m_TimerLabel.text = InterfaceManager.m_Panel_ActionsRadial.m_MissionTimerLabel.text;
                    __instance.m_ChallengeTexture.mainTexture = Utils.GetLargeTexture("challenge_HopelessRescue");
                    Utils.SetActive(__instance.m_ObjectiveTransform.gameObject, false);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(InterfaceManager), "IsUsingSurvivalTabs")]
        private static class InterfaceManager_IsUsingSurvivalTabs
        {
            private static void Postfix(InterfaceManager __instance, ref bool __result)
            {
                if (MyMod.CurrentCustomChalleng.m_Started)
                {
                    __result = false;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(KeroseneLampItem), "ReduceFuel")]
        private static class KeroseneLampItem_ReduceFuel
        {
            private static bool Prefix(KeroseneLampItem __instance, float hoursBurned)
            {
                if(MyMod.OverrideLampReduceFuel == -1)
                {
                    return true;
                }else{
                    if (KeroseneLampItem.m_InfiniteLampOn)
                    {
                        MyMod.OverrideLampReduceFuel = -1;
                        return true;
                    }

                    float FuelPerMinute = __instance.GetModifiedFuelBurnLitersPerHour() / 60;

                    __instance.m_CurrentFuelLiters -= MyMod.OverrideLampReduceFuel * FuelPerMinute;

                    __instance.m_CurrentFuelLiters = Mathf.Clamp(__instance.m_CurrentFuelLiters, 0.0f, __instance.m_MaxFuelLiters);
                    MelonLogger.Msg("[KeroseneLampItem][ReduceFuel] Override lamp fuel. Patchedup time " + __instance.m_CurrentFuelLiters);
                    MelonLogger.Msg("[KeroseneLampItem][ReduceFuel] Game wanted remove " + hoursBurned+" hours but we replace it on "+ MyMod.OverrideLampReduceFuel+" minutes");
                    MyMod.OverrideLampReduceFuel = -1;
                    return false;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "OnCompletedDecalPlaceDown")]
        private static class PlayerManager_OnCompletedDecalPlaceDown
        {
            private static void Prefix(PlayerManager __instance)
            {
                if(__instance.m_DecalToPlace != null)
                {
                    DecalProjectorInstance Decal = __instance.m_DecalToPlace;
                    DeBugMenu.Replica = Decal;
                    DeBugMenu.Replica.m_ColorTint = __instance.GetColourForDecalColour(__instance.m_DecalToPlaceColour);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_ChallengeComplete), "Initialize")]
        private static class Panel_ChallengeComplete_Initialize
        {
            private static void Prefix(Panel_ChallengeComplete __instance)
            {
                MyMod.m_Panel_ChallengeComplete = __instance;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Log), "OnQuitToMainMenu")]
        private static class Panel_Log_OnQuitToMainMenu
        {
            private static bool Prefix(Panel_Log __instance)
            {
                MyMod.BeginRespawn();
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GameManager), "HandlePlayerDeath")]
        private static class GameManager_HandlePlayerDeath
        {
            private static bool Prefix(GameManager __instance)
            {

                if (InterfaceManager.m_Panel_Log.IsEnabled())
                {
                    InterfaceManager.m_Panel_Log.ExitInterfaceOnDeath();
                }
                if (GameManager.m_Rest.IsSleeping())
                {
                    GameManager.m_Rest.EndSleeping(true);
                }
                GameManager.m_PlayerManager.UnequipImmediate(false);
                GameManager.m_PlayerManager.SetControlMode(PlayerControlMode.Dead);
                foreach (AudioCallback componentsInChild in (AudioCallback[])((Component)GameManager.GetPlayerObject().transform.parent).GetComponentsInChildren<AudioCallback>())
                    componentsInChild.ToggleDisableAudioEvents(true);
                switch (ExperienceModeManager.GetCurrentExperienceModeType())
                {
                    case ExperienceModeType.ChallengeRescue:
                    case ExperienceModeType.ChallengeHunted:
                    case ExperienceModeType.ChallengeWhiteout:
                    case ExperienceModeType.ChallengeNomad:
                    case ExperienceModeType.ChallengeHuntedPart2:
                    case ExperienceModeType.ChallengeArchivist:
                    case ExperienceModeType.ChallengeDeadManWalking:
                    case ExperienceModeType.ChallengeNowhereToHide:
                        BaseAiManager.ResetAudioLoops();
                        UIInput.selection = (UIInput)null;
                        GameManager.m_Log.WriteLogToFile();
                        GameManager.CancelPendingSave();
                        InterfaceManager.m_Panel_OptionsMenu.ApplyHudType();
                        if (ExperienceModeManager.IsCurrentEpisodeExperienceMode())
                            break;
                        //SaveGameSystem.DeleteSaveFilesForGameId(SaveGameSystem.m_CurrentGameId);
                        break;
                    default:
                        if (ExperienceModeManager.IsCurrentEpisodeExperienceMode())
                        {
                            InterfaceManager.LoadPanel<Panel_ChallengeComplete>().ShowPanel(Panel_ChallengeComplete.Options.None);
                            goto case ExperienceModeType.ChallengeRescue;
                        }
                        else
                        {
                            InterfaceManager.m_Panel_Log.EnableDeathView();
                            goto case ExperienceModeType.ChallengeRescue;
                        }
                }
                return false;
            }
        }

        public static void OnSaveCompleted(string name = "")
        {            
            if (MyMod.PendingRespawn)
            {
                SaveGameSystem.SetAsyncEnabled(true);
                MyMod.PendingRespawn = false;
                MelonLogger.Msg("[SaveGameSystem] SaveCompletedInternal PendingRespawn");
                SaveSlotInfo SSI = SaveGameSlotHelper.GetCurrentSaveSlotInfo();

                if (SSI == null)
                {
                    MelonLogger.Msg("Wooh Save info is null, trying other way...");
                    MelonLogger.Msg("SaveGameSystem.GetCurrentSaveName() = " + SaveGameSystem.GetCurrentSaveName());
                    GameManager.LoadSaveGameSlot(SaveGameSystem.GetCurrentSaveName(), 0);
                }else{
                    MelonLogger.Msg("[SaveGameSystem] Saving done, loading");
                    GameManager.LoadSaveGameSlot(SSI);
                }
            }
            SaveSlotInfo Sav = SaveGameSlotHelper.GetCurrentSaveSlotInfo();
            if (Sav != null)
            {
                MelonLogger.Msg("[SaveGameSystem] " + Sav.m_SaveSlotName + " just has been saved");
                long Hash = Shared.GetDeterministicId(SaveGameSlots.LoadDataFromSlot(Sav.m_SaveSlotName, "global"));
                MelonLogger.Msg("[SaveGameSystem] Save hash: " + Hash);
                if (QuitOnSave)
                {
                    MyMod.PendingSaveHashToSend = Hash;
                    SaveGameSlots.WriteSlotToDisk(name);
                    MyMod.QuitWhenSaveOperationFinished = true;
                }else{
                    if (MyMod.sendMyPosition && MyMod.ServerConfig.m_SaveScamProtection && MyMod.MyUGUID != "")
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.SAVEHASH))
                        {
                            _packet.Write(MyMod.MyUGUID);
                            _packet.Write(Hash);
                            _packet.Write(false);
                            SendTCPData(_packet);
                        }
                    }
                }
            }else{
                MelonLogger.Msg("[SaveGameSystem] " + name + " just has been saved");
                long Hash = Shared.GetDeterministicId(SaveGameSlots.LoadDataFromSlot(name, "global"));
                MelonLogger.Msg("[SaveGameSystem] Save hash: " + Hash);

                if (QuitOnSave)
                {
                    MyMod.PendingSaveHashToSend = Hash;
                    SaveGameSlots.WriteSlotToDisk(name);
                    MyMod.QuitWhenSaveOperationFinished = true;
                }else{
                    if (MyMod.sendMyPosition && MyMod.ServerConfig.m_SaveScamProtection && MyMod.MyUGUID != "")
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.SAVEHASH))
                        {
                            _packet.Write(MyMod.MyUGUID);
                            _packet.Write(Hash);
                            _packet.Write(false);
                            SendTCPData(_packet);
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SaveGameSystem), "SaveGame")]
        private static class SaveGameSlots_SaveCompletedInternal
        {
            private static void Postfix(SaveGameSystem __instance, string name)
            {
                MelonLogger.Msg(ConsoleColor.Green, "SaveGame(" + name+")");
                OnSaveCompleted(name);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Log), "EnableDeathView")]
        private static class Panel_Log_EnableDeathView
        {
            private static void Postfix(Panel_Log __instance)
            {
                MyMod.IsDead = true;
                MyMod.DropAll();
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Log), "GetOnBackText")]
        private static class Panel_Log_GetOnBackText
        {
            private static void Postfix(Panel_Log __instance, ref string __result)
            {
                if (__instance.QuitToMainMenuOnBack())
                {
                    __result = "Respawn";
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Cairn), "ProcessInteraction")]
        private static class Cairn_ProcessInteraction
        {
            private static void Postfix(Cairn __instance)
            {
                MyMod.AddFoundCairn(__instance.m_JournalEntryNumber);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(MillingMachine), "ProcessInteraction")]
        private static class MillingMachine_ProcessInteraction
        {
            private static bool Prefix(MillingMachine __instance)
            {
                if (__instance.CanBeUsed() && Supporters.ConfiguratedBenefits.m_Knife)
                {
                    if (GameManager.GetPlayerManagerComponent().m_ItemInHands)
                    {
                        GearItem Gear = GameManager.GetPlayerManagerComponent().m_ItemInHands;
                        string InHandName = Gear.m_GearName;
                        if(InHandName == "GEAR_KnifeImprovised")
                        {
                            DataStr.PriorityActionForOtherPlayer act = MyMod.GetCustomAction("Excision");
                            MyMod.DoLongAction(Gear.gameObject, act.m_ProcessText, act.m_Action, act.m_ActionDuration, act.m_Hold);
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public static void EGSHook()
        {
            Il2CppArrayBase<EpicOnlineServicesManager> All = Resources.FindObjectsOfTypeAll<EpicOnlineServicesManager>();

            if (All.Count != 0)
            {
                MelonLogger.Msg("[EpicOnlineServicesManager] m_ClientId " + All[0].m_ClientId);
                Supporters.SetID(All[0].m_ClientId);
            }else{
                MelonLogger.Msg("[EpicOnlineServicesManager] null");
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Keypad), "ProcessInteraction")]
        private static class Keypad_ProcessInteraction
        {
            private static void Prefix(Keypad __instance)
            {
                MelonLogger.Msg(ConsoleColor.Blue, "[Papers codes] Interact with keypad that has code: " + __instance.m_Code);
                MyMod.RestoreCodeFromGears();
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Lock), "ForceLockBegin")]
        private static class LoadScene_ForceLockBegin
        {
            private static bool Prefix(Lock __instance)
            {
                if (__instance.gameObject.GetComponent<Comps.DoorLockedOnKey>())
                {
                    MyMod.SelectKeys(__instance.gameObject, MyMod.KeysAction.OPEN);
                    return false;
                }else{
                    return true;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(uConsoleLog), "Add")]
        internal static class uConsoleLog_Add
        {
            private static void Postfix(string text)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.DedicatedServerAppMode)
                {
                    MelonLogger.Msg(ConsoleColor.DarkGray,"[InGameConsole] " + text);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GameManager), "ForceSaveGame")]
        internal static class GameManager_ForceSaveGame
        {
            private static void Prefix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.DedicatedServerAppMode)
                {
                    SaveGameSystem.AbortScreenshot();
                }
            }
            private static void Postfix()
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (MyMod.DedicatedServerAppMode)
                {
                    SaveGameSystem.AbortScreenshot();
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(CharcoalItem), "StartDetailSurvey")]
        internal static class CharcoalItem_StartDetailSurvey
        {
            private static bool Prefix(CharcoalItem __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }

                MyMod.ShowCharCoalPicker(__instance.gameObject, __instance);

                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Log), "ExitInterface")]
        internal static class Panel_Log_ExitInterface
        {
            private static void Postfix(Panel_Log __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                string text = __instance.m_NotesTextField.GetText();
                if (!string.IsNullOrEmpty(text))
                {
                    
                    if (CharcoalItem.m_CharcoalItemInUseForSurvey != null)
                    {
                        CharcoalItem.m_CharcoalItemInUseForSurvey.StopSurveyAudio();
                        GameManager.GetPlayerAnimationComponent().Trigger_Generic_Unstow();
                        CharcoalItem.m_CharcoalItemInUseForSurvey.ConsumeCharcoal();
                        CharcoalItem.m_CharcoalItemInUseForSurvey = null;
                        MyMod.CreateCustomNote(text);
                        __instance.m_NotesTextField.SetText("");
                        __instance.Enable(false);
                    }
                } else
                {
                    if (CharcoalItem.m_CharcoalItemInUseForSurvey != null)
                    {
                        CharcoalItem.m_CharcoalItemInUseForSurvey.StopSurveyAudio();
                        GameManager.GetPlayerManagerComponent().UnequipItemInHands();
                        CharcoalItem.m_CharcoalItemInUseForSurvey = null;
                        __instance.m_NotesTextField.SetText("");
                        __instance.Enable(false);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(CabinFever), "DisabledForXPMode")]
        internal static class CabinFever_DisabledForXPMode
        {
            private static void Postfix(CabinFever __instance, ref bool __result)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                __result = true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(IntestinalParasites), "IntestinalParasitesStart")]
        internal static class IntestinalParasites_IntestinalParasitesStart
        {
            private static bool Prefix(IntestinalParasites __instance, bool nofx)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                if (GameManager.GetPlayerManagerComponent().PlayerIsDead() || InterfaceManager.IsPanelEnabled<Panel_ChallengeComplete>() || __instance.GetNumDosesRequired() == 0)
                {
                    return false;
                }

                float Damage = -5 * __instance.m_NumPiecesEatenThisRiskCycle;
                float Fatigue = 8 * __instance.m_NumPiecesEatenThisRiskCycle;
                float Thirst = 9 * __instance.m_NumPiecesEatenThisRiskCycle;

                GameManager.GetConditionComponent().AddHealth(Damage, DamageSource.IntestinalParasites);
                FoodPoisoning Poison = GameManager.GetFoodPoisoningComponent();
                Poison.FoodPoisoningStart("GAMEPLAY_TaintedFood", true);
                GameManager.GetFatigueComponent().AddFatigue(Fatigue);
                GameManager.GetThirstComponent().AddThirst(Thirst);
                GameManager.GetHungerComponent().AddReserveCalories(-300);

                Poison.m_DurationHours = Poison.m_DurationHoursMin * __instance.m_NumPiecesEatenThisRiskCycle;
                MelonLogger.Msg(ConsoleColor.Blue, "Intestinal Parasites risk evolved to food poison, m_NumPiecesEatenThisRiskCycle " + __instance.m_NumPiecesEatenThisRiskCycle + " Damage " + Damage);
                __instance.m_NumPiecesEatenThisRiskCycle = 0;
                __instance.m_HasParasiteRisk = false;
                __instance.m_HasParasites = false;

                StatsManager.IncrementValue(StatID.IntestinalParasites);
                if (nofx)
                {
                    return false;
                }
                    
                GameManager.GetPlayerVoiceComponent().Play(__instance.m_ParasitesVO, Voice.Priority.Critical);


                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GameManager), "AllowedToSave")]
        internal static class GameManager_AllowedToSave
        {
            private static void Postfix(GameManager __instance, SaveState state, ref bool __result)
            {
                if(state == SaveState.InMemory)
                {
                    MelonLogger.Msg("[GameManager][AllowedToSave] state " + state);
                    MelonLogger.Msg("s_SaveGameDelayTime " + GameManager.s_SaveGameDelayTime);
                    bool g = (double)GameManager.s_SaveGameDelayTime <= (double)Time.time;
                    MelonLogger.Msg("(double)GameManager.s_SaveGameDelayTime <= (double)Time.time = " + g);
                    MelonLogger.Msg("AllowedToLoadActiveGame() " + GameManager.AllowedToLoadActiveGame());
                    MelonLogger.Msg("m_AllowSaveWithoutGrounding " + GameManager.m_AllowSaveWithoutGrounding);
                    MelonLogger.Msg("GameManager.m_PendingSave " + GameManager.m_PendingSave);

                    if (__result == false)
                    {
                        if (AutoSaveAllTheCost)
                        {
                            MelonLogger.Msg(ConsoleColor.Red, "Can't autosave now, but we really need to, saving... ");
                            AutoSaveAllTheCost = false;
                            __result = true;
                        }
                    } else{
                        AutoSaveAllTheCost = false;
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(WeatherSet), "Update")]
        internal static class WeatherSet_Update
        {
            private static void Prefix(WeatherSet __instance)
            {
                WeatherTransition.m_WeatherTransitionTimeScalar = 0;
            }
            private static void Postfix(WeatherSet __instance)
            {
                WeatherTransition.m_WeatherTransitionTimeScalar = 1;
            }
        }
        public static bool ShowInvitesAfterPicker = false;
        public static bool ShowCluesAfterPicker = false;
        [HarmonyLib.HarmonyPatch(typeof(Panel_ActionPicker), "OnSelect")]
        internal static class Panel_ActionPicker_OnSelect
        {
            private static void Postfix(Panel_ActionPicker __instance)
            {
                if (ShowInvitesAfterPicker)
                {
                    ShowInvitesAfterPicker = false;
                    if (MyMod.iAmHost)
                    {
                        MyMod.ShowInvitesPicker(null, ExpeditionManager.GetInviteForClient(Shared.GetMacAddress()));
                    } else
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.REQUESTEXPEDITIONINVITES))
                        {
                            MyMod.DoPleaseWait("Please wait...", "Calling to coordinator...");
                            MyMod.SetRepeatPacket(MyMod.ResendPacketType.ExpeditionInvites);
                            _packet.Write(true);
                            MyMod.SendUDPData(_packet);
                        }
                    }
                }else if (ShowCluesAfterPicker)
                {
                    ShowCluesAfterPicker = false;
                    ShowCluesPicker();
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(EquipItemPopup), "UpdateAmmoStatus")]
        internal static class EquipItemPopup_UpdateAmmoStatus
        {
            private static void Postfix(EquipItemPopup __instance)
            {
                GearItem Gear = GameManager.GetPlayerManagerComponent().m_ItemInHands;
                if (Gear && (MyMod.IsCustomHandItem(Gear.m_GearName) || Gear.m_GearName == "GEAR_HandheldShortwave"))
                {
                    __instance.HideAmmoPopup();
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(EquipItemPopup), "ShowEquippedItem")]
        internal static class EquipItemPopup_ShowEquippedItem
        {
            private static void Postfix(EquipItemPopup __instance, GearItem gi)
            {
                if (gi)
                {
                    if (MyMod.IsCustomHandItem(gi.m_GearName))
                    {
                        __instance.HideAmmoPopup();
                    }
                    string PrimaryFire = "";
                    string AltFire = "";
                    bool Pad = Utils.IsGamepadActive();
                    bool Show = false;
                    if (gi.m_GearName == "GEAR_HandheldShortwave")
                    {
                        __instance.m_ButtonPromptScrollWheel.ShowPromptForKey("Change frequency", "Scroll");
                        __instance.m_ButtonPromptReload.ShowPromptForKey(Localization.Get("GAMEPLAY_Help"),"Reload");
                        AltFire = "Expeditions";
                        Show = true;
                    } else if (gi.m_GearName == "GEAR_SCNote")
                    {
                        PrimaryFire = "Read";
                        Show = true;
                    }

                    if (Show)
                    {
                        __instance.ShowItemIcons(PrimaryFire, AltFire, false);
                        __instance.m_ButtonPromptFire.ShowPromptForKey(Pad ? PrimaryFire : AltFire, Pad ? "Fire" : "AltFire");
                        __instance.MaybeRepositionFireButtonPrompt(Pad ? AltFire : PrimaryFire);
                        __instance.m_ButtonPromptAltFire.ShowPromptForKey(Pad ? AltFire : PrimaryFire, Pad ? "AltFire" : "Interact");
                        __instance.MaybeRepositionAltFireButtonPrompt(Pad ? PrimaryFire : AltFire);
                    }
                    __instance.m_GearItem = gi;
                }
            }
        }


        [HarmonyLib.HarmonyPatch(typeof(RockCache), "SetupInWorld")]
        internal static class RockCache_SetupInWorld
        {
            private static bool Prefix(RockCache __instance)
            {
                GameObject Obj = __instance.gameObject;
                FakeRockCache Cach = Obj.GetComponent<FakeRockCache>();
                if (Cach == null)
                {
                    Cach = Obj.AddComponent<FakeRockCache>();
                }
                string GUID = ObjectGuidManager.GenerateNewGuidString();
                ObjectGuid GUIDobj = Obj.GetComponent<ObjectGuid>();
                if (GUIDobj == null)
                {
                    GUIDobj = Obj.AddComponent<ObjectGuid>();
                }
                GUIDobj.Set(GUID);
                Cach.m_GUID = GUID;
                Cach.m_Owner = MyMod.MyChatName;
                Cach.m_Rocks = __instance.m_NumRocksFromDismantle;
                Cach.Created();
                UnityEngine.Object.Destroy(__instance);
                return false;
            }
        }
        public static FakeRockCache FakeRockCacheCallback = null;
        public static GearItem HeatPackOpenCallback = null;
        public static GearItem PresentOpenCallback = null;
        public static bool SmeltingLead = false;
        [HarmonyLib.HarmonyPatch(typeof(Panel_GenericProgressBar), "ProgressBarEnded")]
        internal static class Panel_GenericProgressBar_ProgressBarEnded
        {
            private static void Postfix(Panel_GenericProgressBar __instance, bool success, bool playerCancel)
            {
                if(FakeRockCacheCallback != null)
                {
                    GameManager.s_IsAISuspended = false;
                    if (success)
                    {
                        FakeRockCacheCallback.DismantleFinished();
                    }
                    GameManager.GetPlayerManagerComponent().m_RockCacheInProgress = null;
                    FakeRockCacheCallback = null;

                    if (MyMod.sendMyPosition)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.FURNBREAKINSTOP))
                        {
                            _packet.Write(true);
                            SendTCPData(_packet);
                        }
                    } else
                    {
                        ServerSend.FURNBREAKINSTOP(0, true, true);
                    }
                }else if(HeatPackOpenCallback != null)
                {
                    if (success)
                    {
                        OpenHeatPackFinished(HeatPackOpenCallback);
                    }
                    HeatPackOpenCallback = null;
                } else if (PresentOpenCallback != null)
                {
                    if (success)
                    {
                        OpenPresentFinished(PresentOpenCallback);
                    }
                    PresentOpenCallback = null;
                } else if (SmeltingLead)
                {
                    if (success)
                    {
                        GearItem CanGear = GameManager.GetInventoryComponent().GetBestGearItemWithName("GEAR_RecycledCan");
                        if (CanGear)
                        {
                            GameManager.GetInventoryComponent().DestroyGear(CanGear.gameObject);
                        }
                        GearItem LeadGear = GameManager.GetInventoryComponent().GetBestGearItemWithName("GEAR_ScrapLead");
                        if (LeadGear)
                        {
                            if (LeadGear.m_StackableItem && LeadGear.m_StackableItem.m_Units > 1)
                            {
                                LeadGear.m_StackableItem.m_Units--;
                            } else
                            {
                                GameManager.GetInventoryComponent().DestroyGear(LeadGear.gameObject);
                            }
                        }
                        GameManager.GetPlayerVoiceComponent().Play("PLAY_FIRESUCCESS", Voice.Priority.Critical);
                        GearItem Liquid = GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_SCLiquidLead");
                        string text = Localization.Get("GAMEPLAY_ItemAddedToInventory").Replace("{item-name}", Liquid.m_DisplayName);
                        GameManager.GetLogComponent().AddItem(text);
                        GearMessage.AddMessage(Liquid, Localization.Get("GAMEPLAY_Added"), Liquid.m_DisplayName);
                    }
                    SmeltingLead = false;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Map), "DoDetailSurvey")]
        internal static class Panel_Map_DoDetailSurvey
        {
            private static void Postfix(Panel_Map __instance, SurveyType surveyType)
            {
                if(surveyType == SurveyType.Charcoal)
                {
                    Vector3 Position = GameManager.GetPlayerTransform().position;

                    if (MyMod.sendMyPosition)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.CHARCOALDRAW))
                        {
                            _packet.Write(MyMod.level_guid);
                            _packet.Write(Position);
                            SendTCPData(_packet);
                        }
                    } else
                    {
                        ExpeditionManager.RegisterCharcoalDrawing(MyMod.level_guid, Position);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Container), "PopulateContents")]
        internal static class Container_PopulateContents
        {
            private static void Postfix(Container __instance)
            {
                string Gear = GetClueGear();
                if (!string.IsNullOrEmpty(Gear))
                {
                    string PrefabName = Gear;
                    int NarativeID = -1;
                    if (PrefabName.Contains('#'))
                    {
                        string[] Slices = PrefabName.Split('#');
                        PrefabName = Slices[0];
                        NarativeID = int.Parse(Slices[1]);
                    }
                    
                    GameObject reference = GetGearItemObject(PrefabName);

                    if(reference == null) 
                    {
                        return;
                    }
                    GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(reference, __instance.gameObject.transform);
                    newGear.name = reference.name;
                    GearItem Gi = newGear.GetComponent<GearItem>();
                    __instance.AddGear(Gi);
                    newGear.transform.parent = __instance.gameObject.transform;
                    Gi.ManualStart();
                    if(Gi.m_NarrativeCollectibleItem != null)
                    {
                        Gi.m_NarrativeCollectibleItem.m_JournalEntryNumber = NarativeID;
                    }
                    newGear.SetActive(false);
                    MelonLogger.Msg("Special Gear "+ Gear+" spawned in "+__instance.gameObject.name);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(InputManager), "GetOpenActionsPanelHeldDown")]
        internal static class InputManager_GetOpenActionsPanelHeldDown
        {
            private static void Postfix(InputManager __instance, bool __result)
            {
                if(ForcedShowExpeditionHUDSeconds > 0)
                {
                    __result = true;
                }
            }
        }

        //public static UILabel CordsText = null;

        //[HarmonyLib.HarmonyPatch(typeof(Panel_Map), "Initialize")]
        //internal static class Panel_Map_Initialize
        //{
        //    private static void Postfix(Panel_Map __instance)
        //    {
        //        if(CordsText == null)
        //        {
        //            GameObject obj = new GameObject();
        //            obj.transform.parent = __instance.gameObject.transform;
        //            CordsText = obj.AddComponent<UILabel>();
        //            UILabel Donor = __instance.gameObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<UILabel>();
        //            CordsText.font = Donor.font;
        //            CordsText.keepCrispWhenShrunk = UILabel.Crispness.Always;
        //            CordsText.depth = 1;
        //        }
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(Panel_Map), "Update")]
        //internal static class Panel_Map_Update
        //{
        //    private static void Postfix(Panel_Map __instance)
        //    {
        //        if (CordsText != null && __instance.m_Crosshair != null)
        //        {
        //            CordsText.gameObject.transform.localPosition = __instance.m_Crosshair.gameObject.transform.localPosition;
        //            //Vector2 mapPos = new Vector2(CordsText.gameObject.transform.localPosition.x, CordsText.gameObject.transform.localPosition.y);
        //            Vector2 mapPos = new Vector2(__instance.m_PlayerIcon.localPosition.x, __instance.m_PlayerIcon.localPosition.y);
        //            Vector3 worldPos = Vector2.zero;
        //            float W = 0;
        //            __instance.MapPositionToWorldPosition(__instance.m_UnlockedRegionNames[__instance.m_RegionSelectedIndex], mapPos, 0, out worldPos, out W);
        //            string Text = "X " + worldPos.x + " Z " + worldPos.z;
        //            CordsText.text = Text;
        //            CordsText.mText = Text;
        //            CordsText.mProcessedText = Text;
        //        }
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(GearItem), "ProcessPickupItemInteraction")]
        //internal static class GearItem_ProcessPickupItemInteraction
        //{
        //    private static void Postfix(GearItem __instance)
        //    {
        //        if(__instance.m_GearName == "GEAR_SCMapReveal" && __instance.m_ObjectGuid != null)
        //        {
        //            string RevealRegion = __instance.m_ObjectGuid.Get();

        //            Panel_Map Map = InterfaceManager.m_Panel_Map;
        //            if (!string.IsNullOrEmpty(RevealRegion))
        //            {
        //                if (Map != null)
        //                {
        //                    if (Map.m_MapElementData.ContainsKey(RevealRegion))
        //                    {
        //                        var mapElementSaveDataList = Map.m_MapElementData[RevealRegion];
        //                        for (int index = 0; index < mapElementSaveDataList.Count; ++index)
        //                        {
        //                            mapElementSaveDataList[index].m_NameIsKnown = true;
        //                        } 
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        [HarmonyLib.HarmonyPatch(typeof(GearItem), "ManualUpdate")] // Called often
        internal static class GearItem_ManualUpdate
        {
            private static void Postfix(GearItem __instance)
            {
                if (MyMod.CrazyPatchesLogger == true)
                {
                    StackTrace st = new StackTrace(new StackFrame(true));
                    MelonLogger.Msg(ConsoleColor.Blue, "----------------------------------------------------");
                    MelonLogger.Msg(ConsoleColor.Gray, " Stack trace for current level: {0}", st.ToString());
                }
                bool IsHeatPack = __instance.m_GearName == "GEAR_SCHeatPack" || __instance.m_GearName == "GEAR_SCHeatPackC";
                bool IsLiquidLead = __instance.m_GearName == "GEAR_SCLiquidLead";
                if (IsHeatPack || IsLiquidLead)
                {
                    __instance.m_CurrentHP--;
                    if (IsHeatPack)
                    {
                        __instance.gameObject.SetActive(true);
                        __instance.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    }
                    if (__instance.m_CurrentHP <= 0)
                    {
                        string text = Localization.Get("GAMEPLAY_Ruined").Replace("{item-name}", __instance.m_DisplayName);
                        GameManager.GetLogComponent().AddItem(text);
                        GearMessage.AddMessage(__instance, Localization.Get("GAMEPLAY_RuinedPopup"), __instance.m_DisplayName);
                        GameManager.GetInventoryComponent().DestroyGear(__instance.gameObject);
                    }
                }
            }
        }

        public static void OpenHeatPackFinished(GearItem Heatpack)
        {
            GameManager.GetInventoryComponent().DestroyGear(Heatpack.gameObject);
            GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_SCHeatPackC");
        }

        public static void OpenHeatPack(GearItem Heatpack)
        {
            HeatPackOpenCallback = Heatpack;
            InterfaceManager.m_Panel_GenericProgressBar.Launch(Localization.Get("GAMEPLAY_OpeningProgress"), 1f, 0.0f, 0.0f, "Play_SndInvJerky", (string)null, true, true, null);
        }

        public enum PresentRarity
        {
            Shit,
            Low,
            Medium,
            High,
            Epic,
            Legend,
        }

        public static List<string> ShitTier = new List<string>() 
        { 
            "GEAR_Stick",
            "GEAR_Tinder",
            "GEAR_Stone",
            "GEAR_Cloth",
            "GEAR_NewsprintRoll",
            "GEAR_PaperStack",
            "GEAR_ReclaimedWoodB",
            "GEAR_LeatherShoes",
            "GEAR_BasicGloves",
            "GEAR_TeeShirt",
            "GEAR_BaseballCap",
            "GEAR_RecycledCan",
            "GEAR_SkiBoots",
        };
        public static List<string> LowTier = new List<string>()
        {
            "GEAR_CarBattery",
            "GEAR_DustingSulfur",
            "GEAR_ScrapLead",
            "GEAR_ScrapMetal",
            "GEAR_StumpRemover",
            "GEAR_Rope",
            "GEAR_Snare",
            "GEAR_Water500ml",
            "GEAR_DogFood",
            "GEAR_EnergyBar",
            "GEAR_KetchupChips",
            "GEAR_CottonHoodie",
            "GEAR_Leather",
            "GEAR_CottonSocks",
            "GEAR_MackinawJacket",
            "GEAR_BasicWinterCoat",
            "GEAR_CanOpener",
            "GEAR_SimpleTools",
            "GEAR_HeavyBandage",
            "GEAR_BottlePainKillers",
            "GEAR_BottleHydrogenPeroxide",
            "GEAR_BottleAntibiotics",
            "GEAR_RoseHipTea",
            "GEAR_ReishiTea",
            "GEAR_WaterPurificationTablets",
            "GEAR_BasicWoolHat",
            "GEAR_FleeceMittens",
            "GEAR_CoffeeCup",
            "GEAR_Jeans",
            "GEAR_BasicWoolScarf",
            "GEAR_LongUnderwear",
        };
        public static List<string> MediumTier = new List<string>()
        {
            "GEAR_GreenTeaPackage",
            "GEAR_PeanutButter",
            "GEAR_Peaches",
            "GEAR_Soda",
            "GEAR_SodaEnergy",
            "GEAR_SodaGrape",
            "GEAR_SodaOrange",
            "GEAR_TomatoSoupCan",
            "GEAR_SewingKit",
            "GEAR_LampFuel",
            "GEAR_JerryCanRusty",
            "GEAR_CoffeeTin",
            "GEAR_BirchbarkPrepared",
            "GEAR_Firestriker",
            "GEAR_Mittens",
            "GEAR_WorkGloves",
            "GEAR_DownSkiJacket",
            "GEAR_WoolSocks",
            "GEAR_HighQualityTools",
            "GEAR_CookingPot",
            "GEAR_Knife",
            "GEAR_FlareA",
            "GEAR_BlueFlare",
            "GEAR_BirchbarkTea",
            "GEAR_Toque",
            "GEAR_CargoPants",
            "GEAR_InsulatedPants",
            "GEAR_WoolWrap",
            "GEAR_FishermanSweater",
        };
        public static List<string> HighTier = new List<string>()
        {
            "GEAR_QualityWinterCoat",
            "GEAR_PremiumWinterCoat",
            "GEAR_MilitaryParka",
            "GEAR_HeavyParka",
            "GEAR_Arrow",
            "GEAR_RifleAmmoSingle",
            "GEAR_RevolverAmmoSingle",
            "GEAR_GunpowderCan",
            "GEAR_KeroseneLampB",
            "GEAR_Hacksaw",
            "GEAR_Hatchet",
            "GEAR_HomeMadeSoup",
            "GEAR_ClimbingSocks",
            "GEAR_Gauntlets",
            "GEAR_MRE",
            "GEAR_CombatPants",
            "GEAR_Balaclava",
        };
        public static List<string> EpicTier = new List<string>()
        {
            "GEAR_RevolverAmmoBox",
            "GEAR_RifleAmmoBox",
            "GEAR_FlareGunAmmoSingle",
            "GEAR_EmergencyStim",
            "GEAR_RabbitSkinMittens",
            "GEAR_RabbitskinHat",
            "GEAR_BearHide",
            "GEAR_MooseHide",
            "GEAR_SCHeatPackB",
        };
        public static List<string> LegendTier = new List<string>()
        {
            "GEAR_Bow",
            "GEAR_Revolver",
            "GEAR_Rifle",
            "GEAR_BearSkinCoat",
            "GEAR_MooseHideCloak",
            "GEAR_WolfSkinCape",
            "GEAR_EarMuffs",
            "GEAR_MooseHideBag",
            "GEAR_LongUnderwearWool",
        };


        public static void SpawnAndTakeGiftGear()
        {
            InterfaceManager.m_Panel_Inventory.Enable(false);
            PresentRarity Rarity = PresentRarity.Shit;
            System.Random random = new System.Random();
            string GearName = "GEAR_Stone";

            int Rolled = random.Next(0, 100);
            if(Rolled <= 5)
            {
                Rarity = PresentRarity.Legend;
                GearName = LegendTier[random.Next(0, LegendTier.Count)];
            } else if(Rolled <= 13)
            {
                Rarity = PresentRarity.Epic;
                GearName = EpicTier[random.Next(0, EpicTier.Count)];
            } else if (Rolled <= 16)
            {
                Rarity = PresentRarity.High;
                GearName = HighTier[random.Next(0, HighTier.Count)];
            } else if (Rolled <= 20)
            {
                Rarity = PresentRarity.Medium;
                GearName = MediumTier[random.Next(0, MediumTier.Count)];
            } else if (Rolled <= 22)
            {
                Rarity = PresentRarity.Low;
                GearName = LowTier[random.Next(0, LowTier.Count)];
            } else
            {
                Rarity = PresentRarity.Shit;
                GearName = ShitTier[random.Next(0, ShitTier.Count)];
            }
            //if (ExperienceModeManager.s_CurrentModeType == ExperienceModeType.Interloper)
            //{
            //    GearName = Shared.GetInterloperReplace(GearName);
            //}
            GearItem Gift = GameManager.GetPlayerManagerComponent().InstantiateItemAtPlayersFeet(GearName, 1);
            if(Gift == null)
            {
                return;
            }
            Comps.DropFakeOnLeave DFL = Gift.gameObject.AddComponent<Comps.DropFakeOnLeave>();
            DFL.m_OldPossition = Gift.gameObject.transform.position;
            DFL.m_OldRotation = Gift.gameObject.transform.rotation;
            GameManager.GetPlayerManagerComponent().ProcessInspectablePickupItem(Gift);

            if(Rarity == PresentRarity.Shit)
            {
                AkSoundEngine.SetSwitch(SWITCHES.URGENCY.GROUP, SWITCHES.URGENCY.SWITCH.HIGH, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
                GameManager.GetPlayerVoiceComponent().Play("PLAY_FIREFAIL", Voice.Priority.Critical);
            }else if(Rarity == PresentRarity.Low)
            {
                AkSoundEngine.SetSwitch(SWITCHES.URGENCY.GROUP, SWITCHES.URGENCY.SWITCH.LOW, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
                GameManager.GetPlayerVoiceComponent().Play("PLAY_VOINSPECTOBJECT", Voice.Priority.Critical);
            } else if (Rarity == PresentRarity.Medium)
            {
                AkSoundEngine.SetSwitch(SWITCHES.URGENCY.GROUP, SWITCHES.URGENCY.SWITCH.MED, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
                GameManager.GetPlayerVoiceComponent().Play("PLAY_VOINSPECTOBJECT", Voice.Priority.Critical);
            } else if (Rarity == PresentRarity.High)
            {
                AkSoundEngine.SetSwitch(SWITCHES.URGENCY.GROUP, SWITCHES.URGENCY.SWITCH.MED, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
                GameManager.GetPlayerVoiceComponent().Play("PLAY_VOINSPECTOBJECT", Voice.Priority.Critical);
            } else if (Rarity == PresentRarity.Epic)
            {
                AkSoundEngine.SetSwitch(SWITCHES.URGENCY.GROUP, SWITCHES.URGENCY.SWITCH.HIGH, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
                GameManager.GetPlayerVoiceComponent().Play("PLAY_VOINSPECTOBJECT", Voice.Priority.Critical);
            } else if (Rarity == PresentRarity.Legend)
            {
                GameManager.GetPlayerVoiceComponent().Play("PLAY_ENTITYDEATHVO", Voice.Priority.Critical);
            }
        }

        public static void OpenPresentFinished(GearItem Box)
        {
            GameManager.GetPlayerManagerComponent().ConsumeUnitFromInventory(Box.gameObject);
            SpawnAndTakeGiftGear();
        }

        public static void OpenPresent(GearItem Box)
        {
            PresentOpenCallback = Box;
            InterfaceManager.m_Panel_GenericProgressBar.Launch(Localization.Get("GAMEPLAY_OpeningProgress"), 3f, 0.0f, 0.0f, "Play_HarvestingCardboard", (string)null, true, true, null);
        }

        public static void TrySmeltLead(GameObject interactionObject)
        {
            CookingSlot CookingSlot = interactionObject.GetComponent<CookingSlot>();
            bool Lead = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_ScrapLead");
            bool Can = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_RecycledCan");
            if (Lead && Can)
            {
                //GearItem CanGear = GameManager.GetInventoryComponent().GetBestGearItemWithName("GEAR_RecycledCan");
                //GearItem LeadGear = GameManager.GetInventoryComponent().GetBestGearItemWithName("GEAR_ScrapLead");
                //if (LeadGear.m_StackableItem && LeadGear.m_StackableItem.m_Units > 1)
                //{
                //    LeadGear.m_StackableItem.m_Units--;
                //} else
                //{
                //    GameManager.GetInventoryComponent().DestroyGear(LeadGear.gameObject);
                //}
                //if (CanGear)
                //{
                //    CanGear.gameObject.AddComponent<Comps.IgnoreDropOverride>();
                //    CookingSlot.m_GearPlacePoint.DropAndPlaceItem(CanGear);

                //    Cookable CanCookable = CanGear.m_Cookable;

                //    GameObject reference = GetGearItemObject("GEAR_RosehipsPrepared");
                //    Cookable referenceGi = reference.GetComponent<Cookable>();

                //    GameObject LeadObj = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("GEAR_ScrapLead"), new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                //    GearItem LeadGi = LeadObj.GetComponent<GearItem>();
                //    Cookable CookableLead = LeadObj.AddComponent<Cookable>();
                //    CookableLead.m_CookableType = Cookable.CookableType.Liquid;
                //    CookableLead.m_CookAudio = "Play_BoilingLiquidLight";
                //    CookableLead.m_CookedPrefab = GetGearItemPrefab("GEAR_ScrapLead");

                //    CookableLead.m_CookTimeMinutes = 10f;
                //    CookableLead.m_DoNotCookWhenDropped = true;
                //    CookableLead.m_ReadyTimeMinutes = float.PositiveInfinity;

                //    CookableLead.m_CookingPotMaterialsList = new Material[] { referenceGi.m_CookingPotMaterialsList[0]};
                //    CookableLead.m_CookingPotRawMaterialsList = new Material[] { };
                //    CookableLead.m_MeshPotStyle = referenceGi.m_MeshPotStyle;
                //    CookableLead.m_MeshCanStyle = referenceGi.m_MeshCanStyle;

                //    CookableLead.m_PutInPotAudio = "Play_AddWaterToPot";

                //    LeadGi.m_Cookable = CookableLead;
                //    CanGear.m_CookingPotItem.AttachToFire(GameManager.GetFireManagerComponent().GetClosestFire(CookingSlot.gameObject.transform.position), CookingSlot.m_GearPlacePoint);
                //    CanGear.m_CookingPotItem.StartCooking(LeadGi);
                //}
                SmeltingLead = true;
                InterfaceManager.m_Panel_GenericProgressBar.Launch("Smelting...", 10f, 10, 0.0f, "PLAY_GASFIRE", null, false, false, null);

            } else if (!Lead && !Can)
            {
                HUDMessage.AddMessage("Requires " + Utils.GetGearDisplayName("GEAR_ScrapLead") + " and " + Utils.GetGearDisplayName("GEAR_RecycledCan"));
            } else if(!Lead && Can)
            {
                HUDMessage.AddMessage("Requires " + Utils.GetGearDisplayName("GEAR_ScrapLead"));
            }else if(Lead && !Can)
            {
                HUDMessage.AddMessage("Requires " + Utils.GetGearDisplayName("GEAR_RecycledCan"));
            }
        }

        //public static void ShowHookedCookingSlotPicker(GameObject objectInteractedWith)
        //{
        //    Panel_ActionPicker __instance = InterfaceManager.m_Panel_ActionPicker;
        //    __instance.Enable(true);
        //    __instance.m_ActionPickerItemDataList.Clear();
        //    __instance.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_cooking_pot", "GAMEPLAY_Cook", new System.Action(__instance.CookingSlotCookCallback)));
        //    __instance.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_water_prep", "GAMEPLAY_Water", new System.Action(__instance.CookingSlotWaterCallback)));
        //    Action act = new Action(() => TrySmeltLead(objectInteractedWith));
        //    __instance.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_forge", "Smelt lead", act));
        //    __instance.m_ObjectInteractedWith = objectInteractedWith;
        //    __instance.EnableWithCurrentList();
        //}
        //[HarmonyLib.HarmonyPatch(typeof(Panel_ActionPicker), "ShowActionPicker", new Type[] { typeof(GameObject), typeof(Il2CppSystem.Collections.Generic.IEnumerable<Panel_ActionPicker.ActionPickerItemData>) })]
        //internal static class Panel_ActionPicker_ShowActionPicker
        //{
        //    private static bool Prefix(Panel_ActionPicker __instance, GameObject interactionObject)
        //    {
        //        MelonLogger.Msg("Panel_ActionPicker Prefix ShowActionPicker");
        //        if(interactionObject != null && interactionObject.GetComponent<CookingSlot>())
        //        {
        //            ShowHookedCookingSlotPicker(interactionObject);
        //            return false;
        //        }
        //        return true;
        //    }
        //}
        public static void ShowHookedCookingSlotPicker(GameObject objectInteractedWith)
        {
            Panel_ActionPicker __instance = InterfaceManager.m_Panel_ActionPicker;
            Action act = new Action(() => TrySmeltLead(objectInteractedWith));
            bool FireIsActive = false;

            CookingSlot CS = objectInteractedWith.GetComponent<CookingSlot>();
            Campfire CF = objectInteractedWith.GetComponent<Campfire>();
            WoodStove WS = objectInteractedWith.GetComponent<WoodStove>();

            if (CS != null && CS.m_GearPlacePoint != null && CS.m_GearPlacePoint.m_FireToAttach != null)
            {
                FireIsActive = CS.m_GearPlacePoint.m_FireToAttach.IsBurning();
            } else if(CF != null && CF.m_Fire != null)
            {
                FireIsActive = CF.m_Fire.IsBurning();
            }else if(WS != null && WS.m_Fire != null)
            {
                FireIsActive = WS.m_Fire.IsBurning();
            }
            if (FireIsActive)
            {
                __instance.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_forge", "Smelt lead", act));
                __instance.m_ObjectInteractedWith = objectInteractedWith;
                __instance.EnableWithCurrentList();
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_ActionPicker), "ShowActionPicker", new Type[] { typeof(GameObject), typeof(Il2CppSystem.Collections.Generic.IEnumerable<Panel_ActionPicker.ActionPickerItemData>) })]
        internal static class Panel_ActionPicker_ShowActionPicker
        {
            private static void Postfix(Panel_ActionPicker __instance, GameObject interactionObject)
            {
                MelonLogger.Msg("Panel_ActionPicker Postfix ShowActionPicker");
                if (interactionObject != null && (interactionObject.GetComponent<CookingSlot>() || interactionObject.GetComponent<Campfire>() || interactionObject.GetComponent<WoodStove>()))
                {
                    ShowHookedCookingSlotPicker(interactionObject);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_ActionsRadial), "CanPlaceFromRadial")]
        private static class Panel_ActionsRadial_CanPlaceFromRadial
        {
            private static void Postfix(Panel_ActionsRadial __instance, GearItem gi, ref bool __result)
            {
                if (__result)
                { 
                    if(gi.m_GearName == "GEAR_SCEmergencyRation" || gi.m_GearName == "GEAR_SCLiquidLead")
                    {
                        __result = false;
                        return;
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Container), "CanMoveItemToContainerInMoveAll")]
        private static class Panel_Container_CanMoveItemToContainerInMoveAll
        {
            private static void Postfix(Panel_Container __instance, GearItem gearItem, ref bool __result)
            {
                if (__result)
                {
                    if (gearItem.m_GearName == "GEAR_SCEmergencyRation" || gearItem.m_GearName == "GEAR_HeatPack" || gearItem.m_GearName == "GEAR_HeatPackC" || gearItem.m_GearName == "GEAR_SCLiquidLead")
                    {
                        __result = false;
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Container), "ItemPassesFilter")]
        private static class Panel_Container_ItemPassesFilter
        {
            private static void Postfix(Panel_Container __instance, GearItem pi, ref bool __result)
            {
                if (__result)
                {
                    if (pi.m_GearName == "GEAR_SCEmergencyRation" || pi.m_GearName == "GEAR_SCHeatPack" || pi.m_GearName == "GEAR_SCHeatPackC" || pi.m_GearName == "GEAR_SCLiquidLead")
                    {
                        __result = false;
                    }
                }
            }
        }




        //[HarmonyLib.HarmonyPatch(typeof(Panel_ActionsRadial), "GetGearItemsForRadial")]
        //private static class Panel_ActionsRadial_GetGearItemsForRadial
        //{
        //    private static void Postfix(Panel_ActionsRadial __instance, ref Il2CppSystem.Collections.Generic.List<GearItem> __result)
        //    {
        //        if (__result.Count > 8)
        //        {
        //            List<GearItem> _List = new List<GearItem>(){
        //                __result[0],
        //                __result[1],
        //                __result[2],
        //                __result[3],
        //                __result[4],
        //                __result[5],
        //                __result[6],
        //                __result[7]};
        //            __result.Clear();
        //            for (int i = 0; i < _List.Count; i++)
        //            {
        //                __result.Insert(i, _List[i]);
        //            }
        //        };
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(Panel_ActionsRadial), "Initialize")]
        //private static class Panel_ActionsRadial_Initialize
        //{
        //    private static void Postfix(Panel_ActionsRadial __instance)
        //    {
        //        __instance.m_WeaponRadialOrder = new string[] {
        //            "GEAR_FlareGun",
        //            "GEAR_Rifle",
        //            "GEAR_Bow",
        //            "GEAR_Stone",
        //            "GEAR_RifleHuntingLodge",
        //            "GEAR_BearSpear",
        //            "GEAR_BearSpearStory",
        //            "GEAR_Revolver",
        //            "GEAR_NoiseMaker",
        //            //Now non vanila stuff
        //            "GEAR_Knife",
        //            "GEAR_KnifeImprovised",
        //            "GEAR_KnifeScrapMetal",
        //            "GEAR_JeremiahKnife",
        //            "GEAR_Hatchet",
        //            "GEAR_HatchetImprovised",
        //            "GEAR_FireAxe",
        //            "GEAR_Hammer",
        //            "GEAR_Prybar",
        //            "GEAR_Shovel"
        //        };
        //    }
        //}
    }
}