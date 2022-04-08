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

        //public static void AddButton(Panel_MainMenu __instance, string name, int order, int plus)
        //{
        //    Panel_MainMenu.MainMenuItem mainMenuItem = new Panel_MainMenu.MainMenuItem();
        //    mainMenuItem.m_LabelLocalizationId = name;
        //    mainMenuItem.m_Type = Panel_MainMenu.MainMenuItem.MainMenuItemType.Story + plus;
        //    __instance.m_MenuItems.Insert(order, mainMenuItem);

        //    string id = __instance.m_MenuItems[order].m_Type.ToString();
        //    int type = (int)__instance.m_MenuItems[order].m_Type;

        //    __instance.m_BasicMenu.AddItem(id, type, order, name, "", "", new Action(__instance.OnSandbox), Color.gray, Color.white);
        //}
        public static void AddButton(Panel_MainMenu __instance, string name, int order, int plus)
        {
            Panel_MainMenu.MainMenuItem mainMenuItem = new Panel_MainMenu.MainMenuItem();
            mainMenuItem.m_LabelLocalizationId = name;
            mainMenuItem.m_Type = (Panel_MainMenu.MainMenuItem.MainMenuItemType) 30+plus;
            __instance.m_MenuItems.Insert(order, mainMenuItem);

            string id = __instance.m_MenuItems[order].m_Type.ToString();
            int type = (int)__instance.m_MenuItems[order].m_Type;

            __instance.m_BasicMenu.AddItem(id, type, order, name, "", "", new Action(__instance.OnSandbox), Color.gray, Color.white);
        }

        [HarmonyLib.HarmonyPatch(typeof(BasicMenu), "UpdateDescription", null)]
        internal class BasicMenu_UpdateDescription_Post
        {
            private static void Postfix(BasicMenu __instance, int buttonIndex)
            {
                if(buttonIndex >= __instance.m_ItemModelList.Count && __instance.m_ItemModelList[buttonIndex] == null )
                {
                    return;
                }
                if (__instance.m_ItemModelList[buttonIndex].m_DescriptionText == "GAMEPLAY_Description30")
                {
                    __instance.m_DescriptionLabel.text = "People who supported the project.";
                }
                if (__instance.m_ItemModelList[buttonIndex].m_DescriptionText == "GAMEPLAY_Description31")
                {
                    __instance.m_DescriptionLabel.text = "Connect by IP adress. If you plan to join by steam invite, quit the game first, game should not be runned, and then press join from steam chat.";
                }
                if (__instance.m_ItemModelList[buttonIndex].m_DescriptionText == "GAMEPLAY_Description32")
                {
                    __instance.m_DescriptionLabel.text = "Configure and host the server.";
                }
                if (__instance.m_ItemModelList[buttonIndex].m_DescriptionText == "GAMEPLAY_Description33")
                {
                    __instance.m_DescriptionLabel.text = "Reconnect to server. NOT CONNECT! Never use this option for first connect! Because you will miss tone of inital information about sync.";
                }
                if (__instance.m_ItemModelList[buttonIndex].m_DescriptionText == "GAMEPLAY_Description34")
                {
                    __instance.m_DescriptionLabel.text = "Disconnect from current server. Make sure you still alive, cause if you does not. Your savefile will be deleted same as in singleplayer game when you die.";
                }
                if (__instance.m_ItemModelList[buttonIndex].m_DescriptionText == "GAMEPLAY_Description35")
                {
                    __instance.m_DescriptionLabel.text = "Invite your steam friend to your P2P server. Only invites from this menu will work, invites from steam overlay won't work at all.";
                }
            }
        }

        public static void FixUpButtonStrings(Panel_PauseMenu __instance)
        {
            if (__instance.gameObject != null)
            {
                //Panel
                //GetChild(1) //MenuRoot
                //GetChild(1).GetChild(0) //Menu
                //GetChild(1).GetChild(0).GetChild(5) //Left_Align
                //GetChild(1).GetChild(0).GetChild(5).GetChild(2) //Grid


                if (__instance.gameObject.transform.GetChild(1) != null && __instance.gameObject.transform.GetChild(1).GetChild(0) != null && __instance.m_BasicMenu.m_ItemModelList.Count >= 4)
                {
                    Transform Menu = __instance.gameObject.transform.GetChild(1).GetChild(0);
                    Transform Left_Align = Menu.GetChild(5);
                    Transform Grid = Left_Align.GetChild(2);
                    if (Grid != null)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            GameObject Button = Grid.GetChild(i).gameObject;
                            GameObject Lable = Button.transform.GetChild(0).gameObject;

                            string text = "";

                            if(i == 0)
                            {
                                text = "HOST";
                                if(__instance.m_BasicMenu.m_ItemModelList[0] != null)
                                {
                                    if (MyMod.iAmHost == false && MyMod.sendMyPosition == false)
                                    {
                                        __instance.m_BasicMenu.m_ItemModelList[0].m_Selectable = true;
                                    }else{
                                        __instance.m_BasicMenu.m_ItemModelList[0].m_Selectable = false;
                                    }
                                }
                            }else if(i == 1)
                            {
                                text = "RECONNECT";
                                if (__instance.m_BasicMenu.m_ItemModelList[1] != null)
                                {
                                    if (MyMod.iAmHost == false && MyMod.sendMyPosition == false)
                                    {
                                        __instance.m_BasicMenu.m_ItemModelList[1].m_Selectable = true;
                                    }else{
                                        __instance.m_BasicMenu.m_ItemModelList[1].m_Selectable = false;
                                    }
                                }
                            }
                            else if (i == 2)
                            {
                                text = "DISCONNECT";

                                if(__instance.m_BasicMenu.m_ItemModelList[2] != null)
                                {
                                    if (MyMod.sendMyPosition == true)
                                    {
                                        __instance.m_BasicMenu.m_ItemModelList[2].m_Selectable = true;
                                    }else{
                                        __instance.m_BasicMenu.m_ItemModelList[2].m_Selectable = false;
                                    }
                                }
                            }else if (i == 3)
                            {
                                if(SteamConnect.CanUseSteam == false)
                                {
                                    text = "INVITE (ONLY STEAM)";
                                    if (__instance.m_BasicMenu.m_ItemModelList[3] != null)
                                    {
                                        __instance.m_BasicMenu.m_ItemModelList[3].m_Selectable = false;
                                    }
                                }else{
                                    text = "INVITE";
                                    if(__instance.m_BasicMenu.m_ItemModelList[3] != null)
                                    {
                                        if (MyMod.iAmHost == true && Server.UsingSteamWorks == true)
                                        {
                                            __instance.m_BasicMenu.m_ItemModelList[3].m_Selectable = true;
                                        }else{
                                            __instance.m_BasicMenu.m_ItemModelList[3].m_Selectable = false;
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

        public static void AddButtonPause(Panel_PauseMenu __instance, string name, int order, int plus)
        {
            Panel_PauseMenu.PauseMenuItemType mainMenuItem = (Panel_PauseMenu.PauseMenuItemType) 30+ plus;
            __instance.m_MenuItems.Insert(order, mainMenuItem);

            Action act = new Action(() => CustomButtonClick(order));
            string id = __instance.m_MenuItems[order].ToString();
            int menuItem = (int)__instance.m_MenuItems[order];

            __instance.m_BasicMenu.AddItem(id, menuItem, order, name, "", "", act, Color.green, Color.white);
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "Initialize", null)]
        public class Panel_MainMenu_Start
        {
            public static void Postfix(Panel_MainMenu __instance)
            {
                MelonLogger.Msg("[UI] Trying modify main menu...");
                //if (!InterfaceManager.IsMainMenuEnabled()) return;
                //MelonLogger.Msg("[UI] Main main enabled starting modify...");
                AddButton(__instance, "Donaters", 1, 0);
                AddButton(__instance, "Connect", 5, 1);

                MoveUpMainMenuWordmark(Convert.ToInt16(__instance.m_BasicMenu.m_MenuItems.Count.ToString()));
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_PauseMenu), "Initialize", null)]
        public class Panel_PauseMenu_Start
        {
            public static void Postfix(Panel_PauseMenu __instance)
            {
                AddButtonPause(__instance, "HOST A SERVER", 0, 2);
                AddButtonPause(__instance, "RECONNECT", 1, 3);
                AddButtonPause(__instance, "DISCONNECT", 2, 4);
                AddButtonPause(__instance, "INVITE", 3, 5);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_PauseMenu), "Update", null)]
        public class Panel_PauseMenu_Update
        {
            public static void Postfix(Panel_PauseMenu __instance)
            {
                FixUpButtonStrings(__instance);
                if (__instance.gameObject != null && __instance.gameObject.transform.GetChild(4))
                {
                    GameObject Icons = __instance.gameObject.transform.GetChild(4).gameObject;
                    Icons.transform.position = new Vector3(Icons.transform.position.x, 0.27f, Icons.transform.position.z);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BasicMenu), "InternalClickAction", null)]
        internal class BasicMenu_InternalClickAction_Pre
        {
            private static void Prefix(int index, BasicMenu __instance)
            {
                IL2CPP.List<BasicMenu.BasicMenuItemModel> list = __instance.m_ItemModelList;

                if (list[index].m_LabelText == "Donaters")
                {
                    Application.OpenURL("https://filigrani.github.io/SkyCoop/");
                }
                if (list[index].m_LabelText == "Connect")
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
                            SaveData.m_ExperienceMode = (int)ExperienceModeType.Custom;
                            SaveData.m_Location = (int)GameRegion.LakeRegion;
                            SaveData.m_Seed = -1294300353;
                            SaveData.m_CustomExperienceStr = "gsHMbj8PKxsjmaGO98IB";
                            SaveData.m_FixedSpawnScene = "";
                            MyMod.PendingSave = SaveData;
                            MyMod.InterloperHook = true;
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

                    if(InterfaceManager.m_Panel_PauseMenu.m_BasicMenu.m_ItemModelList[CustomId].m_Selectable == false)
                    {
                        return false;
                    }


                    if (CustomId == 0)
                    {
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

