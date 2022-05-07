using System;
using System.Reflection;
using Harmony;
using UnityEngine;
using IL2CPP = Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

using GameServer;
using System.Collections.Generic;

namespace SkyCoop
{
    public class MenuChange
    {
        public static string MenuMode = "Original";
        
        public static void MoveUpMainMenuWordmark(int numOfMainMenuItems)
        {
            GameObject.Find("Panel_MainMenu/MainPanel/Main/TLD_wordmark").transform.localPosition += new Vector3(0, (numOfMainMenuItems - 6) * 30, 0);
        }
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
                    //__instance.m_DescriptionLabel.text = "Host or join the server.";
                    __instance.m_DescriptionLabel.text = "Connect to local or interent server by IP address.";
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
                    __instance.m_DescriptionLabel.text = "Invite your steam friend to this P2P server. Once you invite person, they firstly will join lobby, and then will be connected to this server.";
                }

                if(__instance.m_ItemModelList[buttonIndex].m_Id == "Quit")
                {
                    if(MenuMode == "Multiplayer")
                    {
                        __instance.m_DescriptionLabel.text = "Return to main menu.";
                    }
                    else if(MenuMode == "Join")
                    {
                        __instance.m_DescriptionLabel.text = "Return to multiplayer menu.";
                    }
                }
                if (MenuMode == "Multiplayer")
                {
                    if (__instance.m_ItemModelList[buttonIndex].m_Id == "Extras")
                    {
                        __instance.m_DescriptionLabel.text = "Configure and host the server.";
                    }
                    if (__instance.m_ItemModelList[buttonIndex].m_Id == "Options")
                    {
                        __instance.m_DescriptionLabel.text = "Find server or join by IP address.";
                    }
                    if (__instance.m_ItemModelList[buttonIndex].m_Id == "Quit")
                    {
                        __instance.m_DescriptionLabel.text = "Return to main menu.";
                    }
                }
                if (MenuMode == "Join")
                {
                    if (__instance.m_ItemModelList[buttonIndex].m_Id == "31")
                    {
                        __instance.m_DescriptionLabel.text = "Browse public servers.";
                    }
                    if (__instance.m_ItemModelList[buttonIndex].m_Id == "Extras")
                    {
                        __instance.m_DescriptionLabel.text = "Connect to local or interent server by IP address.";
                    }
                    if (__instance.m_ItemModelList[buttonIndex].m_Id == "Options")
                    {
                        __instance.m_DescriptionLabel.text = "Opens steam friends overlay.";
                    }
                    if (__instance.m_ItemModelList[buttonIndex].m_Id == "Quit")
                    {
                        __instance.m_DescriptionLabel.text = "Return to multiplayer menu";
                    }
                }
            }
        }

        public static Dictionary<int, string> OldMenuData = new Dictionary<int, string>();
        public static bool OldMenuSaved = false;


        public static void SaveOldMenuStuff()
        {
            if (MyMod.m_Panel_MainMenu)
            {
                // Panel_MainMenu/MainPanel/MenuRoot/Menu/Left_Align/Grid
                Transform Grid = MyMod.m_Panel_MainMenu.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(5).GetChild(2);
                for (int i = 0; i <= 6; i++)
                {
                    if (!OldMenuData.ContainsKey(i))
                    {
                        string text = Grid.GetChild(i).GetChild(0).GetComponent<UILabel>().text;
                        OldMenuData.Add(i, text);
                    }
                }
            }
        }

        public static void BackToOriginalMenu()
        {
            if (MyMod.m_Panel_MainMenu)
            {
                Transform Grid = MyMod.m_Panel_MainMenu.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(5).GetChild(2);
                BasicMenu Basic = MyMod.m_Panel_MainMenu.m_BasicMenu;
                for (int i = 0; i <= 6; i++)
                {
                    UILabel Label = Grid.GetChild(i).GetChild(0).GetComponent<UILabel>();
                    string Old;
                    if(OldMenuData.TryGetValue(i, out Old))
                    {
                        Label.mText = Old;
                        Label.text = Old;
                        Label.ProcessText();
                        UnityEngine.Object.Destroy(Grid.GetChild(i).gameObject.GetComponent<MyMod.UiButtonPressHook>());
                        Grid.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
        }

        public static void OverrideMenuButton(Transform Grid, int index, string txt)
        {
            UILabel Label = Grid.GetChild(index).GetChild(0).GetComponent<UILabel>();
            Label.mText = txt;
            Label.text = txt;
            Label.ProcessText();
            Grid.GetChild(index).gameObject.SetActive(true);
        }

        public static void ClearMenuButtons(Transform Grid)
        {
            for (int i = 0; i <= 6; i++)
            {
                Grid.GetChild(i).gameObject.SetActive(false);
            }
        }

        // Panel_MainMenu/MainPanel/MenuRoot/Menu/Left_Align/Grid
        //0 Winter mute
        //1 Survival mode
        //2 Challange
        //3 Multiplayer
        //4 Extra
        //5 Options
        //6 Quit

        public static void GoToMultiplayer()
        {
            if (MyMod.m_Panel_MainMenu)
            {
                Transform Grid = MyMod.m_Panel_MainMenu.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(5).GetChild(2);

                ClearMenuButtons(Grid);
                OverrideMenuButton(Grid, 4, "HOST SERVER");
                OverrideMenuButton(Grid, 5, "JOIN SERVER");
                OverrideMenuButton(Grid, 6, "BACK TO MENU");
            }
        }
        public static void GoToJoinServer()
        {
            if (MyMod.m_Panel_MainMenu)
            {
                Transform Grid = MyMod.m_Panel_MainMenu.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(5).GetChild(2);

                ClearMenuButtons(Grid);
                OverrideMenuButton(Grid, 3, "BROWSE SERVERS");
                OverrideMenuButton(Grid, 4, "CONNECT BY IP");
                OverrideMenuButton(Grid, 5, "CONNECT TO FRIEND");
                OverrideMenuButton(Grid, 6, "BACK ");
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
                                        if (MyMod.MyLobby != "")
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
                //AddButton(__instance, "Donaters", 1, 0);
                //AddButton(__instance, "MULTIPLAYER", 4, 1);
                AddButton(__instance, "CONNECT BY IP", 4, 1);

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

        //public static bool SkipButtonActionOnce = false;
        //[HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "OnStory", null)]
        //public class Panel_MainMenu_OnStory
        //{
        //    public static bool Prefix()
        //    {
        //        if (SkipButtonActionOnce)
        //        {
        //            SkipButtonActionOnce = false;
        //            return false;
        //        }
        //        return true;
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "OnSandbox", null)]
        //public class Panel_MainMenu_OnSandbox
        //{
        //    public static bool Prefix()
        //    {
        //        if (SkipButtonActionOnce)
        //        {
        //            SkipButtonActionOnce = false;
        //            return false;
        //        }
        //        return true;
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "OnChallenges", null)]
        //public class Panel_MainMenu_OnChallenges
        //{
        //    public static bool Prefix()
        //    {
        //        if (SkipButtonActionOnce)
        //        {
        //            SkipButtonActionOnce = false;
        //            return false;
        //        }
        //        return true;
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "OnExtras", null)]
        //public class Panel_MainMenu_OnExtras
        //{
        //    public static bool Prefix()
        //    {
        //        if (SkipButtonActionOnce)
        //        {
        //            SkipButtonActionOnce = false;
        //            return false;
        //        }
        //        return true;
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "OnOptions", null)]
        //public class Panel_MainMenu_OnOptions
        //{
        //    public static bool Prefix()
        //    {
        //        if (SkipButtonActionOnce)
        //        {
        //            SkipButtonActionOnce = false;
        //            return false;
        //        }
        //        return true;
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "OnQuit", null)]
        //public class Panel_MainMenu_OnQuit
        //{
        //    public static bool Prefix()
        //    {
        //        if (SkipButtonActionOnce)
        //        {
        //            SkipButtonActionOnce = false;
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        public static void ConnectByIp()
        {
            //For Debug Alone
            bool DebugAlone = false;
            bool ConnectedScreenTest = false;
            bool KickTest = false;

            if (ConnectedScreenTest == true)
            {
                MyMod.DoWaitForConnect();
                if (KickTest == true)
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

        //[HarmonyLib.HarmonyPatch(typeof(BasicMenu), "InternalClickAction", null)]
        //internal class BasicMenu_InternalClickAction_Pre
        //{
        //    private static bool Prefix(BasicMenu __instance, int index)
        //    {
        //        IL2CPP.List<BasicMenu.BasicMenuItemModel> list = __instance.m_ItemModelList;
        //        Transform Grid;
        //        UILabel Label;
        //        if (list != null)
        //        {
        //            Grid = MyMod.m_Panel_MainMenu.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(5).GetChild(2);
        //            Label = Grid.GetChild(index).GetChild(0).GetComponent<UILabel>();
        //        }else{
        //            return true;
        //        }

        //        if(Label.mText == "CONNECT BY IP")
        //        {
        //            ConnectByIp();
        //        }
        //        return true; // Future code  will be added later.

        //        //MelonLogger.Msg("Clicked " + Label.mText);
        //        if (Label.mText == "Donaters")
        //        {
        //            Application.OpenURL("https://filigrani.github.io/SkyCoop/");
        //            return false;
        //        }
        //        else if(Label.mText == "MULTIPLAYER")
        //        {
        //            if (!OldMenuSaved)
        //            {
        //                SaveOldMenuStuff();
        //            }
        //            GoToMultiplayer();
        //            MenuMode = "Multiplayer";
        //            return false;
        //        }
        //        else if(Label.text == "BACK TO MENU")
        //        {
        //            BackToOriginalMenu();
        //            MenuMode = "Original";
        //            GameAudioManager.PlayGUIButtonClick();
        //            return false;
        //        }
        //        else if(Label.text == "BACK ")
        //        {
        //            GoToMultiplayer();
        //            MenuMode = "Multiplayer";
        //            GameAudioManager.PlayGUIButtonClick();
        //            return false;
        //        }
        //        else if(Label.text == "JOIN SERVER")
        //        {
        //            GoToJoinServer();
        //            MenuMode = "Join";
        //            GameAudioManager.PlayGUIButtonClick();
        //            return false;
        //        }
        //        else if(Label.text == "CONNECT TO FRIEND")
        //        {
        //            if (SteamConnect.CanUseSteam)
        //            {
        //                SteamConnect.Main.OpenFriends();
        //            }
        //            GameAudioManager.PlayGUIButtonClick();
        //            return false;
        //        }
        //        else if(Label.text == "CONNECT BY IP")
        //        {
        //            ConnectByIp();
        //            return false;
        //        }
        //        return true;
        //    }
        //}
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
                if (list[index].m_LabelText == "CONNECT BY IP")
                {
                    ConnectByIp();
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
                    //MelonLogger.Msg("Clicked m_CustomId " + CustomId);
                    if (CustomId != -1)
                    {
                        if (CustomId < 4)
                        {
                            if (InterfaceManager.m_Panel_PauseMenu.m_BasicMenu.m_ItemModelList[CustomId].m_Selectable == false)
                            {
                                return false;
                            }
                            if (CustomId == 0)
                            {
                                MyMod.HostMenu();
                            }
                            else if (CustomId == 1)
                            {
                                MyMod.DoIPConnectWindow();
                            }
                            else if (CustomId == 2)
                            {
                                MelonLogger.Msg("Disconnect case pressed disconnect button");
                                MyMod.LastConnectedIp = "";
                                MyMod.Disconnect();
                            }
                            else if (CustomId == 3)
                            {
                                if (MyMod.MyLobby != "")
                                {
                                    SteamConnect.Main.MakeFriendListUI();
                                }
                            }
                        }
                    }
                    return false;
                }
                return true;
            }
        }
    }
}

