using System;
using System.Reflection;
using Harmony;
using UnityEngine;
using IL2CPP = Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

using GameServer;

namespace SkyCoop
{
    public class MenuChange
    {
        public static void MoveUpMainMenuWordmark(int numOfMainMenuItems)
        {
            GameObject.Find("Panel_MainMenu/MainPanel/Main/TLD_wordmark").transform.localPosition += new Vector3(0, (numOfMainMenuItems - 6) * 30, 0);
        }

        public static void AddButton(Panel_MainMenu __instance, string name, int order, int plus)
        {
            Panel_MainMenu.MainMenuItem mainMenuItem = new Panel_MainMenu.MainMenuItem();
            mainMenuItem.m_LabelLocalizationId = name;
            mainMenuItem.m_Type = Panel_MainMenu.MainMenuItem.MainMenuItemType.Story + plus;
            __instance.m_MenuItems.Insert(order, mainMenuItem);

            string id = __instance.m_MenuItems.get_Item(order).m_Type.ToString();
            int type = (int)__instance.m_MenuItems.get_Item(order).m_Type;

            __instance.m_BasicMenu.AddItem(id, type, order, name, "", "", new Action(__instance.OnSandbox), Color.gray, Color.white);
        }

        public static void FixUpButtonStrings(Panel_PauseMenu __instance)
        {
            if (__instance.gameObject != null)
            {
                if (__instance.gameObject.transform.GetChild(1) != null && __instance.gameObject.transform.GetChild(1).GetChild(0) != null && __instance.m_BasicMenu.m_ItemModelList.Count >= 4)
                {
                    Transform Menu = __instance.gameObject.transform.GetChild(1).GetChild(0);
                    if (Menu.transform.GetChild(2) != null && Menu.transform.GetChild(2).GetChild(0) != null)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            GameObject Button = Menu.transform.GetChild(2).GetChild(i).gameObject;
                            GameObject Lable = Button.transform.GetChild(0).gameObject;

                            string text = "";

                            if(i == 0)
                            {
                                text = "HOST";
                                if(__instance.m_BasicMenu.m_ItemModelList.get_Item(0) != null)
                                {
                                    if (MyMod.iAmHost == false && MyMod.sendMyPosition == false)
                                    {
                                        __instance.m_BasicMenu.m_ItemModelList.get_Item(0).m_Selectable = true;
                                    }else{
                                        __instance.m_BasicMenu.m_ItemModelList.get_Item(0).m_Selectable = false;
                                    }
                                }
                            }else if(i == 1)
                            {
                                text = "RECONNECT";
                                if (__instance.m_BasicMenu.m_ItemModelList.get_Item(1) != null)
                                {
                                    if (MyMod.iAmHost == false && MyMod.sendMyPosition == false)
                                    {
                                        __instance.m_BasicMenu.m_ItemModelList.get_Item(1).m_Selectable = true;
                                    }else{
                                        __instance.m_BasicMenu.m_ItemModelList.get_Item(1).m_Selectable = false;
                                    }
                                }
                            }
                            else if (i == 2)
                            {
                                text = "DISCONNECT";

                                if(__instance.m_BasicMenu.m_ItemModelList.get_Item(2) != null)
                                {
                                    if (MyMod.sendMyPosition == true)
                                    {
                                        __instance.m_BasicMenu.m_ItemModelList.get_Item(2).m_Selectable = true;
                                    }else{
                                        __instance.m_BasicMenu.m_ItemModelList.get_Item(2).m_Selectable = false;
                                    }
                                }
                            }else if (i == 3)
                            {
                                if(SteamConnect.CanUseSteam == false)
                                {
                                    text = "INVITE (ONLY STEAM)";
                                    if (__instance.m_BasicMenu.m_ItemModelList.get_Item(3) != null)
                                    {
                                        __instance.m_BasicMenu.m_ItemModelList.get_Item(3).m_Selectable = false;
                                    }
                                }else{
                                    text = "INVITE";
                                    if(__instance.m_BasicMenu.m_ItemModelList.get_Item(3) != null)
                                    {
                                        if (MyMod.iAmHost == true && Server.UsingSteamWorks == true)
                                        {
                                            __instance.m_BasicMenu.m_ItemModelList.get_Item(3).m_Selectable = true;
                                        }else{
                                            __instance.m_BasicMenu.m_ItemModelList.get_Item(3).m_Selectable = false;
                                        }
                                    }
                                }
                            }

                            if (Lable.GetComponent<UILabel>() != null)
                            {
                                UILabel UiLab = Lable.GetComponent<UILabel>();
                                UiLab.mText = text;
                                UiLab.text = text;
                                UiLab.ProcessText();
                            }
                            if (Button.GetComponent<UIButton>() != null)
                            {
                                if(Button.GetComponent<MyMod.UiButtonPressHook>() == null)
                                {
                                    Button.AddComponent<MyMod.UiButtonPressHook>();
                                    Button.GetComponent<MyMod.UiButtonPressHook>().m_CustomId = i;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void CustomButtonClick(int _ID)
        {
            MelonLogger.Msg("New hook, clicked "+_ID);
        }

        public static void AddButtonPause(Panel_PauseMenu __instance, string name, int order)
        {
            Panel_PauseMenu.PauseMenuItemType mainMenuItem = Panel_PauseMenu.PauseMenuItemType.BackToGame;
            __instance.m_MenuItems.Insert(order, mainMenuItem-order);
            Action act = new Action(() => CustomButtonClick(order));
            //__instance.m_BasicMenu.AddItem("", 0, order, name, "", "", new Action(act), Color.gray, Color.white);
            string id = __instance.m_MenuItems.get_Item(order).ToString();
            int menuItem = (int)__instance.m_MenuItems.get_Item(order);

            __instance.m_BasicMenu.AddItem(id, menuItem, order, name, name, (string)null, act, Color.green, Color.white);
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "Start", null)]
        public class Panel_MainMenu_Start
        {
            public static void Postfix(Panel_MainMenu __instance)
            {
                if (!InterfaceManager.IsMainMenuActive()) return;

                AddButton(__instance, "Donaters", 1, 10);
                AddButton(__instance, "Connect", 5, 9);

                MoveUpMainMenuWordmark(Convert.ToInt16(__instance.m_BasicMenu.m_MenuItems.Count.ToString()));
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_PauseMenu), "Start", null)]
        public class Panel_PauseMenu_Start
        {
            public static void Postfix(Panel_PauseMenu __instance)
            {
                AddButtonPause(__instance, "HOST A SERVER", 0);
                AddButtonPause(__instance, "RECONNECT", 1);
                AddButtonPause(__instance, "DISCONNECT", 2);
                AddButtonPause(__instance, "INVITE", 3);

                if(__instance.gameObject != null && __instance.gameObject.transform.GetChild(4))
                {
                    GameObject Icons = __instance.gameObject.transform.GetChild(4).gameObject;
                    Icons.transform.position = new Vector3(Icons.transform.position.x, 0.27f, Icons.transform.position.z);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_PauseMenu), "Update", null)]
        public class Panel_PauseMenu_Update
        {
            public static void Postfix(Panel_PauseMenu __instance)
            {
                FixUpButtonStrings(__instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BasicMenu), "InternalClickAction", null)]
        internal class BasicMenu_InternalClickAction_Pre
        {
            private static void Prefix(int index, BasicMenu __instance)
            {
                IL2CPP.List<BasicMenu.BasicMenuItemModel> list = __instance.m_ItemModelList;

                if (list.get_Item(index).m_LabelText == "Donaters")
                {
                    Application.OpenURL("https://filigrani.github.io/SkyCoop/");
                }
                if (list.get_Item(index).m_LabelText == "Connect")
                {
                    //For Debug Alone
                    bool DebugAlone = false;
                    bool ConnectedScreenTest = false;
                    bool KickTest = false;

                    if(ConnectedScreenTest == true)
                    {
                        MyMod.DoWaitForConnect();
                        if(KickTest == true)
                        {
                            MyMod.DoKickMessage("Wrong mod version! Server using version " + MyMod.BuildInfo.Version);
                        }
                    }else{
                        if (DebugAlone == true)
                        {
                            MyMod.SaveSlotSync SaveData = new MyMod.SaveSlotSync();
                            SaveData.m_SaveSlotType = (int)SaveSlotType.SANDBOX;
                            SaveData.m_Episode = (int)Episode.One;
                            SaveData.m_ExperienceMode = (int)ExperienceModeType.Stalker;
                            SaveData.m_Location = (int)GameRegion.LakeRegion;
                            SaveData.m_Seed = 22566665;
                            //SaveData.m_Seed = -1541615651;
                            MyMod.PendingSave = SaveData;
                            MyMod.CheckHaveSaveFileToJoin(SaveData);
                        }else{
                            InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input server address", "127.0.0.1", Panel_Confirmation.ButtonLayout.Button_2, "Connect", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(UIButton), "OnClick")]
        internal class UIButton_Press
        {
            private static bool Prefix(UIButton __instance)
            {
                if (__instance.gameObject != null && __instance.gameObject.GetComponent<MyMod.UiButtonPressHook>() != null)
                {
                    int CustomId = __instance.gameObject.GetComponent<MyMod.UiButtonPressHook>().m_CustomId;

                    if(InterfaceManager.m_Panel_PauseMenu.m_BasicMenu.m_ItemModelList.get_Item(CustomId).m_Selectable == false)
                    {
                        return false;
                    }


                    if (CustomId == 0)
                    {
                        //if (SteamConnect.CanUseSteam == true)
                        //{
                        //    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Confirm, "LOCAL OR STEAM?", "You can use local mode to connect with using LAN network emulators, or port forwarding 26950 port. In Steam mode players can join only by invites of host.", Panel_Confirmation.ButtonLayout.Button_2, "LOCAL", "STEAM", Panel_Confirmation.Background.Transperent, null, null);
                        //}else{
                        //    MyMod.HostAServer();
                        //}
                        MyMod.HostMenu();
                    }else if (CustomId == 1)
                    {
                        MyMod.DoIPConnectWindow();
                    }else if (CustomId == 2)
                    {
                        MelonLogger.Msg("Disconnect case pressed disconnect button");
                        MyMod.LastConnectedIp = "";
                        MyMod.Disconnect();
                    }else if (CustomId == 3)
                    {
                        if (Server.UsingSteamWorks == true)
                        {
                            SteamConnect.Main.MakeFriendListUI();
                        }
                    }
                    return false;
                }
                return true;
            }
        }
    }
}

