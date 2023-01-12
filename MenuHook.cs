using System;
using System.Reflection;
using Harmony;
using UnityEngine;
using IL2CPP = Il2CppSystem.Collections.Generic;
using MelonLoader;
using MelonLoader.TinyJSON;
using GameServer;
using System.Collections.Generic;

namespace SkyCoop
{
    public class MenuChange
    {
        public static string MenuMode = "Original";
        public static int TempExperience = 0;

        public static void SetDefaultCamera()
        {
            GameManager.GetPlayerManagerComponent().TeleportPlayer(new Vector3(48.08f, 3.33f, 63.81f), GameManager.GetCurrentCamera().transform.rotation);
        }
        public static void SetFlairsCamera()
        {
            GameManager.GetPlayerManagerComponent().TeleportPlayer(new Vector3(-7.71f, 16.85f, 11.47f), GameManager.GetCurrentCamera().transform.rotation);
        }
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
                if(__instance == null || buttonIndex >= __instance.m_ItemModelList.Count || __instance.m_ItemModelList[buttonIndex] == null )
                {
                    return;
                }
                if (__instance.m_ItemModelList[buttonIndex].m_DescriptionText == "GAMEPLAY_Description30")
                {
                    __instance.m_DescriptionLabel.text = "People who supported the project.";
                }
                if (__instance.m_ItemModelList[buttonIndex].m_DescriptionText == "GAMEPLAY_Description31")
                {
                    if(MyMod.MyLobby == "")
                    {
                        __instance.m_DescriptionLabel.text = "Host or join a multiplayer session.";
                    }else{
                        __instance.m_DescriptionLabel.text = "Show lobby you are currently in.";
                    }
                }
                if (__instance.m_ItemModelList[buttonIndex].m_DescriptionText == "GAMEPLAY_Description32")
                {
                    __instance.m_DescriptionLabel.text = "Invite your steam friend to this P2P session. Once you invite someone, they will join the lobby first and then will be connected to your server.";
                }
                if (MenuMode == "Multiplayer")
                {
                    if (__instance.m_ItemModelList[buttonIndex].m_Id == "NewSurvival")
                    {
                        __instance.m_DescriptionLabel.text = "Configure and host a session.";
                    }
                    else if(__instance.m_ItemModelList[buttonIndex].m_Id == "LoadSurvival")
                    {
                        if (SteamConnect.CanUseSteam)
                        {
                            __instance.m_DescriptionLabel.text = "Find a server or join by IP address.";
                        }else{
                            __instance.m_DescriptionLabel.text = "Join by IP address.";
                        }
                    }
                    else if (__instance.m_ItemModelList[buttonIndex].m_Id == "Feats")
                    {
                        __instance.m_DescriptionLabel.text = "Change your nickname and customize your appeal.";
                    }
                }
                else if(MenuMode == "Join")
                {
                    if (__instance.m_ItemModelList[buttonIndex].m_Id == "NewSurvival")
                    {
                        __instance.m_DescriptionLabel.text = "Browse public servers.";
                    }
                    else if(__instance.m_ItemModelList[buttonIndex].m_Id == "LoadSurvival")
                    {
                        __instance.m_DescriptionLabel.text = "Connect to a server by IP address.";
                    }
                    else if(__instance.m_ItemModelList[buttonIndex].m_Id == "Feats")
                    {
                        __instance.m_DescriptionLabel.text = "Opens steam friends overlay.";
                    }
                }
                else if(MenuMode == "MultiProfileSettings")
                {
                    if (__instance.m_ItemModelList[buttonIndex].m_Id == "NewSurvival")
                    {
                        __instance.m_DescriptionLabel.text = "Change your character name. Other players will see it ingame.";
                    }
                    else if (__instance.m_ItemModelList[buttonIndex].m_Id == "LoadSurvival")
                    {
                        __instance.m_DescriptionLabel.text = "Toggle supporter bonuses and select flairs.";
                    }
                    else if (__instance.m_ItemModelList[buttonIndex].m_Id == "Feats")
                    {
                        __instance.m_DescriptionLabel.text = "Copy to clipboard EGS or Steam Account ID.";
                    }
                }
                else if(MenuMode == "Lobby")
                {
                    if (__instance.m_ItemModelList[buttonIndex].m_Id == "ResumeSurvival")
                    {
                        __instance.m_DescriptionLabel.text = "Start your session if everyone is ready, or if you do not want to wait for other players.\n(Players will be able to join, even after the session is already started).";
                    }
                    else if (__instance.m_ItemModelList[buttonIndex].m_Id == "NewSurvival")
                    {
                        if (MyMod.LobbyState != "Vote")
                        {
                            __instance.m_DescriptionLabel.text = "Create or load a sandbox game.";
                        }else{
                            __instance.m_DescriptionLabel.text = "Vote for session settings.";
                        }
                    }
                    else if (__instance.m_ItemModelList[buttonIndex].m_Id == "LoadSurvival")
                    {
                        __instance.m_DescriptionLabel.text = "Invite your steam friend to this lobby.";
                    }
                    else if(__instance.m_ItemModelList[buttonIndex].m_Id == "Feats")
                    {
                        __instance.m_DescriptionLabel.text = "Copy invite link to clipboard.";
                    }
                }
                else if(MenuMode == "NewGameSelect")
                {
                    if (__instance.m_ItemModelList[buttonIndex].m_Id == "LoadSurvival")
                    {
                        __instance.m_DescriptionLabel.text = "Start vote for session settings.";
                    }
                    else if (__instance.m_ItemModelList[buttonIndex].m_Id == "Feats")
                    {
                        __instance.m_DescriptionLabel.text = "Create new sandbox game without voting.";
                    }
                }
                else if(MenuMode == "Vote")
                {
                    if (__instance.m_ItemModelList[buttonIndex].m_Id == "LoadSurvival")
                    {
                        __instance.m_DescriptionLabel.text = "Vote for a starting region.";
                    }
                    else if (__instance.m_ItemModelList[buttonIndex].m_Id == "Feats")
                    {
                        __instance.m_DescriptionLabel.text = "Vote for experience mode.";
                    }
                }
            }
        }

        public static void SetLobbyState(string state)
        {
            SteamConnect.Main.SetLobbyState(state);
        }

        public static void ReturnOriginalButtons(Transform Grid, int index)
        {
            UILabel Label = Grid.GetChild(index).GetChild(0).GetComponent<UILabel>();
            UIButton Button = Grid.GetChild(index).GetComponent<UIButton>();
            if (Grid.GetChild(index).gameObject.GetComponent<Comps.UiButtonKeyboardPressSkip>() != null)
            {
                Comps.UiButtonKeyboardPressSkip Skip = Grid.GetChild(index).gameObject.GetComponent<Comps.UiButtonKeyboardPressSkip>();
                Button.onClick = Skip.m_Click;
            }
        }

        public static void OverrideMenuButton(Transform Grid, int index, string txt, bool onClickHack = true)
        {
            UILabel Label = Grid.GetChild(index).GetChild(0).GetComponent<UILabel>();
            UIButton Button = Grid.GetChild(index).GetComponent<UIButton>();

            if (onClickHack)
            {
                if (Grid.GetChild(index).gameObject.GetComponent<Comps.UiButtonKeyboardPressSkip>() == null)
                {
                    Comps.UiButtonKeyboardPressSkip Skip = Grid.GetChild(index).gameObject.AddComponent<Comps.UiButtonKeyboardPressSkip>();
                    Skip.m_Click = Button.onClick;
                    Button.onClick = null;
                }
                else if (Grid.GetChild(index).gameObject.GetComponent<Comps.UiButtonKeyboardPressSkip>() != null)
                {
                    Comps.UiButtonKeyboardPressSkip Skip = Grid.GetChild(index).gameObject.GetComponent<Comps.UiButtonKeyboardPressSkip>();
                    Skip.m_Click = Button.onClick;
                    Button.onClick = null;
                }
            }

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

                            if (i == 0)
                            {
                                if(SteamConnect.CanUseSteam == false)
                                {
                                    text = "INVITE (STEAM ONLY)";
                                    if (__instance.m_BasicMenu.m_ItemModelList[0] != null)
                                    {
                                        __instance.m_BasicMenu.m_ItemModelList[0].m_Selectable = false;
                                    }
                                }else{
                                    text = "INVITE";
                                    if(__instance.m_BasicMenu.m_ItemModelList[0] != null)
                                    {
                                        if (MyMod.MyLobby != "")
                                        {
                                            __instance.m_BasicMenu.m_ItemModelList[0].m_Selectable = true;
                                        }else{
                                            __instance.m_BasicMenu.m_ItemModelList[0].m_Selectable = false;
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
                                    if (Button.GetComponent<Comps.UiButtonPressHook>() == null)
                                    {
                                        Button.AddComponent<Comps.UiButtonPressHook>();
                                        Button.GetComponent<Comps.UiButtonPressHook>().m_CustomId = i;
                                    }
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
                AddButton(__instance, "MULTIPLAYER", 4, 1);

                //AddButton(__instance, "CONNECT BY IP", 4, 1);

                MoveUpMainMenuWordmark(Convert.ToInt16(__instance.m_BasicMenu.m_MenuItems.Count.ToString()));
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_PauseMenu), "Initialize", null)]
        public class Panel_PauseMenu_Start
        {
            public static void Postfix(Panel_PauseMenu __instance)
            {
                //AddButtonPause(__instance, "HOST A SERVER", 0, 2);
                //AddButtonPause(__instance, "RECONNECT", 1, 3);
                //AddButtonPause(__instance, "DISCONNECT", 2, 4);
                AddButtonPause(__instance, "INVITE", 0, 2);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_PauseMenu), "Update", null)]
        public class Panel_PauseMenu_Update
        {
            public static void Postfix(Panel_PauseMenu __instance)
            {
                FixUpButtonStrings(__instance);
                //if (__instance.gameObject != null && __instance.gameObject.transform.GetChild(4))
                //{
                //    GameObject Icons = __instance.gameObject.transform.GetChild(4).gameObject;
                //    Icons.transform.position = new Vector3(Icons.transform.position.x, 0.27f, Icons.transform.position.z);
                //}
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Sandbox), "Update", null)]
        public class Panel_Sandbox_Update
        {
            public static void Postfix(Panel_PauseMenu __instance)
            {
                Transform Grid = __instance.gameObject.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(2);
                for (int i = 0; i <= 6; i++)
                {
                    GameObject Button = Grid.GetChild(i).gameObject;

                    if (Button.GetComponent<UIButton>() != null)
                    {
                        if (Button.GetComponent<Comps.UiButtonPressHook>() == null)
                        {
                            Button.AddComponent<Comps.UiButtonPressHook>();
                            Button.GetComponent<Comps.UiButtonPressHook>().m_CustomId = i;
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Sandbox), "OnClickBack", null)]
        public class Panel_Sandbox_OnClickBack
        {
            public static bool Prefix(Panel_Sandbox __instance)
            {
                if (MenuMode == "Join")
                {
                    ChangeMenuItems("Multiplayer");
                    return false;
                }else if (MenuMode == "MultiProfileSettings")
                {
                    ChangeMenuItems("Multiplayer");
                    return false;
                }
                else if(MenuMode == "Nothing")
                {
                    MyMod.HostMenuClose();
                    return false;
                }else if(MenuMode == "Lobby")
                {
                    MyMod.LobbyUI.SetActive(false);
                    MyMod.LobbyRegion.SetActive(false);
                    MyMod.LobbyExperience.SetActive(false);
                }else if(MenuMode == "LobbySettings")
                {
                    if (!MyMod.StartServerAfterSelectSave)
                    {
                        ChangeMenuItems("Lobby");
                    }else{
                        MyMod.StartServerAfterSelectSave = false;
                        ChangeMenuItems("Multiplayer");
                    }
                    return false;
                }else if(MenuMode == "NewGameSelect")
                {
                    ChangeMenuItems("LobbySettings");
                    return false;
                }
                else if(MenuMode == "Vote")
                {
                    ChangeMenuItems("Lobby");
                    return false;
                }
                else if(MenuMode == "Multiplayer")
                {
                    ChangeMenuItems("Original");
                    return true;
                }else if(MenuMode == "Browser")
                {
                    MenuMode = "Join";
                    Transform Align = MyMod.m_Panel_Sandbox.gameObject.transform.GetChild(0).GetChild(0).GetChild(5);
                    Align.GetChild(1).gameObject.SetActive(true); //SelectIcon
                    Align.GetChild(2).gameObject.SetActive(true); //Grid
                    Align.GetChild(4).gameObject.SetActive(true); //Description
                    Align.GetChild(5).gameObject.SetActive(true); //Linebreaker
                    MyMod.ServerBrowser.SetActive(false);
                    return false;
                }else if(MenuMode == "Customize")
                {
                    if(MyMod.TargetFlairSlot == -1)
                    {
                        MyMod.CustomizeUiPanel("Close");
                        MenuMode = "MultiProfileSettings";
                        Transform Align = MyMod.m_Panel_Sandbox.gameObject.transform.GetChild(0).GetChild(0).GetChild(5);
                        Align.GetChild(1).gameObject.SetActive(true); //SelectIcon
                        Align.GetChild(2).gameObject.SetActive(true); //Grid
                        Align.GetChild(4).gameObject.SetActive(true); //Description
                        Align.GetChild(5).gameObject.SetActive(true); //Linebreaker
                        MyMod.m_Panel_Sandbox.gameObject.transform.GetChild(0).GetChild(0).GetChild(3).gameObject.SetActive(true);
                        SetDefaultCamera();
                    }else{
                        MyMod.CustomizeUiPanel("Main");
                    }
                    return false;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_ChooseSandbox), "OnClickBack", null)]
        public class Panel_ChooseSandbox_OnClickBack
        {
            public static bool Prefix(Panel_ChooseSandbox __instance)
            {
                if (MenuMode == "LobbySettings")
                {
                    __instance.Enable(false);
                    InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(true);
                    ChangeMenuItems("LobbySettings");
                    return false;
                }

                return true;
            }
        }

        public static bool SetSaveSlotInfo(int Seed, string name)
        {
            SaveSlotInfo SaveToLoad = null;
            Il2CppSystem.Collections.Generic.List<SaveSlotInfo> Slots = SaveGameSystem.GetSortedSaveSlots(Episode.One, SaveSlotType.SANDBOX);
            for (int i = 0; i < Slots.Count; i++)
            {
                SaveSlotInfo Slot = Slots[i];

                if(Seed != 0)
                {
                    if (Slot.m_UserDefinedName == Seed.ToString())
                    {
                        SaveToLoad = Slot;
                        break;
                    }
                }else{
                    if (Slot.m_SaveSlotName == name)
                    {
                        SaveToLoad = Slot;
                        break;
                    }
                }
            }
            if (SaveToLoad != null)
            {
                UtilsPanelChoose.RefreshDetails(SaveToLoad, MyMod.LobbyDeitailsStrct, UtilsPanelChoose.DetailsType.Sandbox);
                return true;
            }
            return false;
        }

        public static int GetSaveSeed(int index)
        {
            SaveSlotInfo saveSlotInfo = SaveGameSlotHelper.GetSaveSlotInfo(SaveSlotType.SANDBOX, index);
            if (saveSlotInfo == null)
            {
                MelonLogger.Msg(ConsoleColor.Red, "Can't load save on step 0");
                return 0;
            }else{
                string name = saveSlotInfo.m_SaveSlotName;
                string text = SaveGameSlots.LoadDataFromSlot(name, "global");
                if (string.IsNullOrEmpty(text))
                {
                    MelonLogger.Msg(ConsoleColor.Red, "Can't load save on step 1");
                    return 0;
                }
                GlobalSaveGameFormat GSF = Utils.DeserializeObject<GlobalSaveGameFormat>(text);
                if (string.IsNullOrEmpty(GSF.m_GameManagerSerialized))
                {
                    MelonLogger.Msg(ConsoleColor.Red, "Can't load save on step 2");
                    return 0;
                }
                GameManagerSaveDataProxy managerSaveDataProxy = Utils.DeserializeObject<GameManagerSaveDataProxy>(GSF.m_GameManagerSerialized);
                if (managerSaveDataProxy != null)
                {
                    if (!string.IsNullOrEmpty(managerSaveDataProxy.m_SceneTransitionDataSerialized))
                    {
                        SceneTransitionData TData = Utils.DeserializeObject<SceneTransitionData>(managerSaveDataProxy.m_SceneTransitionDataSerialized);
                        return TData.m_GameRandomSeed;
                    }else{
                        MelonLogger.Msg(ConsoleColor.Red, "Can't load save on step 4 " + managerSaveDataProxy.m_SceneTransitionDataSerialized);
                        return 0;
                    }
                }else{
                    MelonLogger.Msg(ConsoleColor.Red, "Can't load save on step 3 "+ managerSaveDataProxy.m_SceneTransitionDataSerialized);
                    return 0;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "OnLoadSaveSlot", null)]
        public class Panel_MainMenu_OnLoadSaveSlot
        {
            public static bool Prefix(Panel_MainMenu __instance, SaveSlotType saveSlotType, int slotIndex)
            {
                if (MenuMode == "LobbySettings" || MyMod.StartServerAfterSelectSave)
                {
                    SaveSlotInfo SaveInfo = SaveGameSlotHelper.GetSaveSlotInfo(SaveSlotType.SANDBOX, slotIndex);
                    if (CheckSaveRNG(SaveInfo.m_SaveSlotName) == false)
                    {
                        MelonLogger.Msg("[Panel_MainMenu][OnLoadSaveSlot] CheckSaveRNG == false");
                        InterfaceManager.TrySetPanelEnabled<Panel_ChooseSandbox>(true);
                        return false;
                    }
                    MelonLogger.Msg("[Panel_MainMenu][OnLoadSaveSlot] CheckSaveRNG == true");
                }


                if (MenuMode == "LobbySettings" && !MyMod.StartServerAfterSelectSave)
                {
                    SaveSlotInfo SaveInfo = SaveGameSlotHelper.GetSaveSlotInfo(SaveSlotType.SANDBOX, slotIndex);
                    
                    InterfaceManager.TrySetPanelEnabled<Panel_ChooseSandbox>(false);
                    MyMod.SelectedSaveSeed = GetSaveSeed(slotIndex);

                    
                    MyMod.SelectedSaveName = SaveInfo.m_SaveSlotName;

                    if (Utils.SceneIsTransition(SaveInfo.m_Location))
                    {
                        SaveInfo.m_Region = Utils.GetHardcodedRegionForLocation(SaveInfo.m_Location);
                    }

                    string Test = "";
                    
                    if (string.IsNullOrEmpty(SaveInfo.m_Region))
                    {
                        string regionForLocation = Utils.GetHardcodedRegionForLocation(SaveInfo.m_Location);
                        if (regionForLocation != "")
                        {
                            Test = regionForLocation;
                        }
                        else if (SaveInfo.m_Location == "MineTransitionZone" || SaveInfo.m_Location == "HighwayMineTransitionZone")
                        {
                            Test = SaveInfo.m_Location;
                        }
                        else
                        {
                            Test = SaveInfo.m_Location;
                        }
                    }else{
                        Test = SaveInfo.m_Region;
                    }

                    GameRegion Reg;
                    RegionManager.GetRegionFromString(Test, out Reg);

                    MyMod.LobbyStartingRegion = (int)Reg;
                    MyMod.LobbyStartingExperience = (int)SaveInfo.m_XPMode;

                    SteamConnect.Main.SetSaveSlotSeed(MyMod.SelectedSaveSeed);
                    SteamConnect.Main.SetNewGameSettings(MyMod.LobbyStartingRegion, MyMod.LobbyStartingExperience);
                    MelonLogger.Msg("Selected save with seed "+ MyMod.SelectedSaveSeed);
                    InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(true);
                    ChangeMenuItems("Lobby");
                    SetLobbyState("SelectedSave");
                    return false;
                }

                return true;
            }
        }
        
        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectExperience), "OnClickBack", null)]
        public class Panel_SelectExperience_OnClickBack
        {
            public static bool Prefix(Panel_SelectExperience __instance)
            {
                if (MenuMode == "NewGameSelect")
                {
                    __instance.Enable(false);
                    InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(true);
                    ChangeMenuItems("NewGameSelect");
                    return false;
                }else if(MenuMode == "Vote")
                {
                    __instance.Enable(false);
                    InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(true);
                    ChangeMenuItems("Lobby");
                    return false;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectExperience), "OnExperienceClicked", null)]
        public class Panel_SelectExperience_OnExperienceClicked
        {
            public static bool Prefix(Panel_SelectExperience __instance)
            {
                if (MenuMode == "Vote")
                {
                    Panel_SelectExperience.XPModeMenuItem selectedMenuItem = __instance.GetSelectedMenuItem();
                    if (selectedMenuItem == null)
                    {
                        return false;
                    }
                    if (SteamConnect.CanUseSteam)
                    {
                        SteamConnect.Main.VoteForExperienceMode((int)selectedMenuItem.m_Type);
                    }
                    __instance.Enable(false);
                    InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(true);
                    ChangeMenuItems("Lobby");
                    return false;
                }else if (MenuMode == "NewGameSelect")
                {
                    Panel_SelectExperience.XPModeMenuItem selectedMenuItem = __instance.GetSelectedMenuItem();
                    if (selectedMenuItem == null)
                    {
                        return false;
                    }
                    TempExperience = (int)selectedMenuItem.m_Type;
                    __instance.Enable(false);

                    if(MyMod.ServerConfig.m_PlayersSpawnType != 2)
                    {
                        InterfaceManager.TrySetPanelEnabled<Panel_SelectRegion_Map>(true);
                    }else{
                        MyMod.LobbyStartingRegion = (int)GameRegion.RandomRegion;
                        MyMod.LobbyStartingExperience = TempExperience;
                        SteamConnect.Main.SetNewGameSettings(MyMod.LobbyStartingRegion, MyMod.LobbyStartingExperience);
                        SteamConnect.Main.SetLobbyState("SelectedNewSave");
                        __instance.Enable(false);
                        MyMod.m_Panel_Sandbox.Enable(true);
                        ChangeMenuItems("Lobby");
                    }
                    
                    return false;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Badges), "OnCancel", null)]
        public class Panel_Badges_OnCancel
        {
            public static bool Prefix(Panel_PauseMenu __instance)
            {
                if (MenuMode == "LobbySettings")
                {
                    __instance.Enable(false);
                    InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(true);
                    ChangeMenuItems("LobbySettings");
                    return false;
                }

                return true;
            }
        }

        public static void ConnectByIp()
        {
            //For Debug Alone
            bool DebugAlone = false;
            bool ConnectedScreenTest = false;
            bool KickTest = false;

            if (ConnectedScreenTest == true)
            {
                MyMod.DoWaitForConnect(true);
                if (KickTest == true)
                {
                    MyMod.DoKickMessage("Wrong mod version! Server using version " + MyMod.BuildInfo.Version);
                }
            }else{
                if (DebugAlone == true)
                {
                    DataStr.SaveSlotSync SaveData = new DataStr.SaveSlotSync();
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

        public static bool CanVoteForRegion()
        {
            int T = MyMod.ServerConfig.m_PlayersSpawnType;

            if(T == 0 || T == 3)
            {
                return true;
            }else{
                return false;
            }
        }

        public static bool CanStartServer()
        {
            if (MyMod.LobbyState == "SelectedSave" || MyMod.LobbyState == "SelectedNewSave")
            {
                return true;
            }else{
                return false;
            }
        }
        public static bool VoteInProcess()
        {
            if (MyMod.LobbyState == "Vote")
            {
                return true;
            }else{
                return false;
            }
        }

        public static void SetUILableText(UILabel lab, string Txt)
        {
            lab.mText = Txt;
            lab.text = Txt;
            lab.ProcessText();
            if(lab.gameObject.GetComponent<UILocalize>() != null)
            {
                UnityEngine.Object.Destroy(lab.gameObject.GetComponent<UILocalize>());
            }
        }

        public static void SetNewGameInfo(int Region, int ExpMode)
        {
            Transform Texts = MyMod.LobbyNewGame.transform.GetChild(1);
            SetUILableText(MyMod.LobbyNewGame.transform.GetChild(0).GetChild(0).gameObject.GetComponent<UILabel>(), Localization.Get("GAMEPLAY_NewGame"));

            SetUILableText(Texts.GetChild(2).GetChild(0).gameObject.GetComponent<UILabel>(), "INFO");
            SetUILableText(Texts.GetChild(2).GetChild(1).gameObject.GetComponent<UILabel>(), "NEW SAVE WILL BE CREATED");

            SetUILableText(Texts.GetChild(3).GetChild(0).gameObject.GetComponent<UILabel>(), Localization.Get("GAMEPLAY_XPMode"));
            SetUILableText(Texts.GetChild(3).GetChild(1).gameObject.GetComponent<UILabel>(), Utils.GetLocalizedExperienceMode((ExperienceModeType)ExpMode));

            SetUILableText(Texts.GetChild(4).GetChild(0).gameObject.GetComponent<UILabel>(), Localization.Get("GAMEPLAY_Region"));
            

            if((GameRegion)Region == GameRegion.RandomRegion)
            {
                if(MyMod.ServerConfig.m_PlayersSpawnType != 1)
                {
                    SetUILableText(Texts.GetChild(4).GetChild(1).gameObject.GetComponent<UILabel>(), Localization.Get("GAMEPLAY_RandomRegion"));
                }else{
                    SetUILableText(Texts.GetChild(4).GetChild(1).gameObject.GetComponent<UILabel>(), Localization.Get("GAMEPLAY_Custom"));
                }
            }else{
                SetUILableText(Texts.GetChild(4).GetChild(1).gameObject.GetComponent<UILabel>(), Utils.GetLocalizedRegion((GameRegion)Region));
            }

            Texts.GetChild(5).gameObject.SetActive(false);
            Texts.GetChild(6).gameObject.SetActive(false);
            Texts.GetChild(7).gameObject.SetActive(false);
            Texts.GetChild(8).gameObject.SetActive(false);
            Texts.GetChild(9).gameObject.SetActive(false);

            GameRegion GRegion = (GameRegion)Region;
            string RegionTxtName = UtilsPanelChoose.GetRegionSaveSlotTextureName(GRegion.ToString());
            string RegionTexture = UtilsPanelChoose.GetRegionLargeTextureName(GRegion.ToString());
            MyMod.LobbyNewGame.transform.GetChild(2).gameObject.GetComponent<UISprite>().spriteName = RegionTxtName;
            MyMod.LobbyNewGame.transform.GetChild(3).gameObject.GetComponent<UITexture>().mainTexture = Utils.GetLargeTexture(RegionTexture);

            Texture LoadedAssets = MyMod.LoadedBundle.LoadAsset<Texture>("NewGamePreview");
            Texts.GetChild(1).gameObject.GetComponent<UITexture>().mainTexture = LoadedAssets;
        }

        public static void OpenFlairsMenu()
        {
            Transform Align = MyMod.m_Panel_Sandbox.gameObject.transform.GetChild(0).GetChild(0).GetChild(5);
            Align.GetChild(1).gameObject.SetActive(false); //SelectIcon
            Align.GetChild(2).gameObject.SetActive(false); //Grid
            Align.GetChild(4).gameObject.SetActive(false); //Description
            Align.GetChild(5).gameObject.SetActive(false); //Linebreaker
            MyMod.m_Panel_Sandbox.gameObject.transform.GetChild(0).GetChild(0).GetChild(3).gameObject.SetActive(false);
            MenuMode = "Customize";
            MyMod.CustomizeUiPanel("Main");
            SetFlairsCamera();
            if (MyMod.MyPlayerDoll)
            {
                MyMod.MyPlayerDoll.transform.position = new Vector3(-9f , 17.2f, 10.8f);
                MyMod.MyPlayerDoll.transform.rotation = new Quaternion(0, 0.866f, 0, -0.5f);
            }else{
                MelonLogger.Msg(ConsoleColor.Red, "MyPlayerDoll does not exist");
            }
        }

        public static void ChangeMenuItems(string mode)
        {
            Transform Grid = MyMod.m_Panel_Sandbox.gameObject.transform.GetChild(0).GetChild(0).GetChild(5).GetChild(2);

            if(mode == "Original")
            {
                ReturnOriginalButtons(Grid, 0);
                ReturnOriginalButtons(Grid, 1);
                ReturnOriginalButtons(Grid, 2);
                ReturnOriginalButtons(Grid, 3);
                MenuMode = mode;
                return;
            }

            ClearMenuButtons(Grid);
            MenuMode = mode;
            if(mode == "Multiplayer")
            {
                OverrideMenuButton(Grid, 1, "HOST SERVER");
                OverrideMenuButton(Grid, 2, "JOIN SERVER");
                OverrideMenuButton(Grid, 3, "SETTINGS");
            }
            else if(mode == "Join")
            {
                OverrideMenuButton(Grid, 1, "BROWSE SERVERS");
                OverrideMenuButton(Grid, 2, "CONNECT BY IP");
                OverrideMenuButton(Grid, 3, "CONNECT TO FRIEND");
            }else if(mode == "MultiProfileSettings")
            {
                OverrideMenuButton(Grid, 1, "CHANGE NICKNAME");
                OverrideMenuButton(Grid, 2, "CUSTOMIZATION");
                OverrideMenuButton(Grid, 3, "COPY ACCOUNT ID");
            }
            else if(mode == "Lobby")
            {
                if (!VoteInProcess())
                {
                    if (SteamConnect.IsMyLobby)
                    {
                        if (SteamConnect.IsMyLobby == true)
                        {
                            if (CanStartServer())
                            {
                                OverrideMenuButton(Grid, 0, "START SERVER");
                            }
                            OverrideMenuButton(Grid, 1, "SELECT SAVE");
                        }
                    }
                }else{
                    OverrideMenuButton(Grid, 1, "VOTE");
                }

                OverrideMenuButton(Grid, 2, "INVITE FRIEND");
                OverrideMenuButton(Grid, 3, "COPY INVITE LINK");
                MyMod.LobbyUI.SetActive(true);
                MyMod.LobbyRegion.SetActive(VoteInProcess() && CanVoteForRegion());
                MyMod.LobbyExperience.SetActive(VoteInProcess());

                if(MyMod.LobbyDeitails == null)
                {
                    MelonLogger.Msg("[UI] Trying to copy details panel...");

                    if (MyMod.m_Panel_ChooseSandbox.m_DetailObjects.m_Details)
                    {
                        GameObject DObj = UnityEngine.Object.Instantiate<GameObject>(MyMod.m_Panel_ChooseSandbox.m_DetailObjects.m_Details, MyMod.m_Panel_Sandbox.transform);
                        MelonLogger.Msg("[UI] Details copied!");
                        MyMod.LobbyDeitails = DObj;
                        MyMod.LobbyDeitailsStrct = new UtilsPanelChoose.DetailsObjets();

                        GameObject DObj2 = UnityEngine.Object.Instantiate<GameObject>(MyMod.m_Panel_ChooseSandbox.m_DetailObjects.m_Details, MyMod.m_Panel_Sandbox.transform);
                        MyMod.LobbyNewGame = DObj2;
                        MyMod.LobbyNewGame.SetActive(false);
                        UtilsPanelChoose.DetailsObjets st = MyMod.LobbyDeitailsStrct;
                        GameObject obj = MyMod.LobbyDeitails;
                        Transform Texts = obj.transform.GetChild(1);
                        st.m_TitleLabel = obj.transform.GetChild(0).GetChild(0).gameObject.GetComponent<UILabel>();
                        st.m_ThumbnailTexture = Texts.GetChild(1).gameObject.GetComponent<UITexture>();
                        st.m_BackgroundTexture = obj.transform.GetChild(3).gameObject.GetComponent<UITexture>();
                        st.m_DateLabel = Texts.GetChild(2).GetChild(1).gameObject.GetComponent<UILabel>();
                        st.m_SurvivorLabel = Texts.GetChild(3).GetChild(1).gameObject.GetComponent<UILabel>();
                        st.m_ConditionLabel = Texts.GetChild(4).GetChild(1).gameObject.GetComponent<UILabel>();
                        st.m_XPLabel = Texts.GetChild(5).GetChild(1).gameObject.GetComponent<UILabel>();
                        st.m_SurvivedLabel = Texts.GetChild(6).GetChild(1).gameObject.GetComponent<UILabel>();
                        st.m_ExploredLabel = Texts.GetChild(7).GetChild(1).gameObject.GetComponent<UILabel>();
                        st.m_RegionLabel = Texts.GetChild(8).GetChild(1).gameObject.GetComponent<UILabel>();
                        st.m_LocationLabel = Texts.GetChild(9).GetChild(1).gameObject.GetComponent<UILabel>();
                        st.m_RegionSprite = obj.transform.GetChild(2).gameObject.GetComponent<UISprite>();
                        st.m_CenteredTexture = null;

                        for (int i = 0; i < MyMod.LobbyDeitails.transform.GetChild(4).GetChild(1).childCount; i++)
                        {
                            GameObject ObjActive = MyMod.LobbyDeitails.transform.GetChild(4).GetChild(1).GetChild(i).gameObject;
                            GameObject BG = ObjActive.transform.GetChild(1).gameObject;
                            if (BG.GetComponent<AnimatedAlpha>())
                            {
                                UnityEngine.GameObject.Destroy(BG.GetComponent<AnimatedAlpha>());
                            }
                            if (BG.GetComponent<UITexture>())
                            {
                                BG.GetComponent<UITexture>().alpha = 0.4f;
                            }
                        }
                        for (int i = 0; i < MyMod.LobbyNewGame.transform.GetChild(4).GetChild(1).childCount; i++)
                        {
                            GameObject ObjActive = MyMod.LobbyNewGame.transform.GetChild(4).GetChild(1).GetChild(i).gameObject;
                            GameObject BG = ObjActive.transform.GetChild(1).gameObject;
                            if (BG.GetComponent<AnimatedAlpha>())
                            {
                                UnityEngine.GameObject.Destroy(BG.GetComponent<AnimatedAlpha>());
                            }
                            if (BG.GetComponent<UITexture>())
                            {
                                BG.GetComponent<UITexture>().alpha = 0.4f;
                            }
                        }
                    }
                }

                bool ShowDeitails = false;
                bool ShowNewSave = false;
                if(MyMod.LobbyDeitails && MyMod.LobbyNewGame)
                {
                    if (MyMod.LobbyState == "SelectedSave")
                    {
                        bool Found = false;

                        if (SteamConnect.IsMyLobby)
                        {
                            Found = SetSaveSlotInfo(0, MyMod.SelectedSaveName);
                        }else{
                            Found = SetSaveSlotInfo(MyMod.SelectedSaveSeed, "");
                        }

                        if (Found)
                        {
                            ShowDeitails = true;
                        }else{
                            SetNewGameInfo(MyMod.LobbyStartingRegion, MyMod.LobbyStartingExperience);
                            ShowNewSave = true;
                        }
                    }else if(MyMod.LobbyState == "SelectedNewSave")
                    {
                        SetNewGameInfo(MyMod.LobbyStartingRegion, MyMod.LobbyStartingExperience);
                        ShowNewSave = true;
                    }

                    MyMod.LobbyDeitails.SetActive(ShowDeitails);
                    MyMod.LobbyNewGame.SetActive(ShowNewSave);

                    GameObject ObjToUse = null;

                    if (ShowDeitails)
                    {
                        ObjToUse = MyMod.LobbyDeitails;
                    }
                    else if (ShowNewSave)
                    {
                        ObjToUse = MyMod.LobbyNewGame;
                    }

                    if (ObjToUse)
                    {
                        ExperienceModeType expMode = (ExperienceModeType)MyMod.LobbyStartingExperience;
                        int SelectID = 0;
                        if(expMode == ExperienceModeType.Pilgrim)
                        {
                            SelectID = 0;
                        }else if (expMode == ExperienceModeType.Voyageur)
                        {
                            SelectID = 1;
                        }
                        else if (expMode == ExperienceModeType.Stalker)
                        {
                            SelectID = 2;
                        }
                        else if (expMode == ExperienceModeType.Interloper)
                        {
                            SelectID = 3;
                        }

                        for (int i = 0; i < ObjToUse.transform.GetChild(4).GetChild(1).childCount; i++)
                        {
                            ObjToUse.transform.GetChild(4).GetChild(1).GetChild(i).gameObject.SetActive(i == SelectID);
                        }
                    }
                }
            }
            else if (mode == "LobbySettings")
            {
                OverrideMenuButton(Grid, 1, "NEW GAME");

                bool EverPlayer = SaveGameSlotHelper.GetNumSaveSlots(SaveSlotType.SANDBOX) > 0;
                if (EverPlayer)
                {
                    OverrideMenuButton(Grid, 2, "LOAD SAVE");
                    OverrideMenuButton(Grid, 3, "FEATS");
                }
            }
            else if (mode == "NewGameSelect")
            {
                OverrideMenuButton(Grid, 2, "START VOTE");
                OverrideMenuButton(Grid, 3, "CHOOSE MYSELF");
            }
            else if (mode == "Vote")
            {
                if(CanVoteForRegion() == true)
                {
                    OverrideMenuButton(Grid, 2, "REGION");
                }
                
                OverrideMenuButton(Grid, 3, "EXPERIENCE MODE");
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
                else if (list[index].m_LabelText == "MULTIPLAYER")
                {
                    
                    MyMod.m_Panel_Sandbox = InterfaceManager.LoadPanel<Panel_Sandbox>();
                    bool result = InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(true);
                    if (result)
                    {
                        MyMod.m_Panel_Sandbox.m_TitleLocalizationId = "MULTIPLAYER";
                        MyMod.m_Panel_Sandbox.m_BasicMenu.UpdateTitle("MULTIPLAYER");
                    }else{
                        return;
                    }


                    if(MyMod.MyLobby == "")
                    {
                        ChangeMenuItems("Multiplayer");
                    }else{
                        ChangeMenuItems("Lobby");
                    }
                }
                else if(list[index].m_LabelText == "LOBBY")
                {
                    MyMod.m_Panel_Sandbox.Enable(true);
                    MyMod.m_Panel_Sandbox.m_TitleLocalizationId = "MULTIPLAYER";
                    MyMod.m_Panel_Sandbox.m_BasicMenu.UpdateTitle("MULTIPLAYER");
                    ChangeMenuItems("Lobby");
                }
            }
        }

        public static void CloseLobbyUI()
        {
            MyMod.LobbyUI.SetActive(false);
            MyMod.LobbyRegion.SetActive(false);
            MyMod.LobbyExperience.SetActive(false);
        }

        [HarmonyLib.HarmonyPatch(typeof(UIButton), "OnClick")]
        internal class UIButton_Press
        {
            private static bool Prefix(UIButton __instance)
            {
                if (__instance.gameObject != null && __instance.gameObject.GetComponent<Comps.UiButtonPressHook>() != null)
                {
                    int CustomId = __instance.gameObject.GetComponent<Comps.UiButtonPressHook>().m_CustomId;
                    //MelonLogger.Msg("Clicked m_CustomId " + CustomId);
                    if (CustomId != -1)
                    {
                        if (InterfaceManager.m_Panel_PauseMenu.isActiveAndEnabled)
                        {
                            if (CustomId == 0)
                            {
                                if (MyMod.MyLobby != "")
                                {
                                    SteamConnect.Main.MakeFriendListUI();
                                }
                            }
                        }
                        else if(MyMod.m_Panel_Sandbox != null && MyMod.m_Panel_Sandbox.isActiveAndEnabled && MenuMode != "Original")
                        {
                            GameAudioManager.PlayGUIButtonClick();
                            if (MenuMode == "Multiplayer")
                            {
                                if (CustomId == 1)
                                {
                                    MyMod.HostMenu(true);
                                    Transform Align = MyMod.m_Panel_Sandbox.gameObject.transform.GetChild(0).GetChild(0).GetChild(5);
                                    Align.GetChild(1).gameObject.SetActive(false); //SelectIcon
                                    Align.GetChild(2).gameObject.SetActive(false); //Grid
                                    Align.GetChild(4).gameObject.SetActive(false); //Description
                                    Align.GetChild(5).gameObject.SetActive(false); //Linebreaker
                                    MenuMode = "Nothing";
                                }
                                else if (CustomId == 2)
                                {
                                    if (SteamConnect.CanUseSteam)
                                    {
                                        ChangeMenuItems("Join");
                                    }else{
                                        ConnectByIp();
                                    }
                                }
                                else if (CustomId == 3)
                                {
                                    ChangeMenuItems("MultiProfileSettings");
                                }
                            }
                            else if(MenuMode == "MultiProfileSettings")
                            {
                                if (CustomId == 1)
                                {
                                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "HOW DO YOU WANT TO BE CALLED?", MyMod.MyChatName, Panel_Confirmation.ButtonLayout.Button_2, "GAMEPLAY_Apply", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                                }
                                else if (CustomId == 2)
                                {
                                    OpenFlairsMenu();
                                }
                                else if(CustomId == 3)
                                {
                                    if (Supporters.IsLoaded())
                                    {
                                        InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, "Your ID was copied to clipboard", Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null);
                                        GUIUtility.systemCopyBuffer = Supporters.MyID;
                                    }else{
                                        InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, "Can't detect your account ID", Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null);
                                    }
                                }
                            }
                            else if(MenuMode == "Join")
                            {
                                if (CustomId == 1)
                                {
                                    Transform Align = MyMod.m_Panel_Sandbox.gameObject.transform.GetChild(0).GetChild(0).GetChild(5);
                                    Align.GetChild(1).gameObject.SetActive(false); //SelectIcon
                                    Align.GetChild(2).gameObject.SetActive(false); //Grid
                                    Align.GetChild(4).gameObject.SetActive(false); //Description
                                    Align.GetChild(5).gameObject.SetActive(false); //Linebreaker                                    
                                    MenuMode = "Browser";
                                    SteamConnect.Main.BrowseServers();
                                }
                                else if (CustomId == 2)
                                {
                                    ConnectByIp();
                                }
                                else if (CustomId == 3)
                                {
                                    if (SteamConnect.CanUseSteam)
                                    {
                                        SteamConnect.Main.OpenFriends();
                                    }
                                }
                            }
                            else if (MenuMode == "Lobby")
                            {
                                if (CustomId == 0)
                                {
                                    CloseLobbyUI();
                                    if (SteamConnect.CanUseSteam)
                                    {
                                        if(MyMod.LobbyState == "SelectedNewSave")
                                        {
                                            SteamConnect.Main.CreateSaveForHosting();
                                        }
                                        else if(MyMod.LobbyState == "SelectedSave")
                                        {
                                            SteamConnect.Main.LoadSaveForHosting();
                                        }
                                        SetLobbyState("StartingGame");
                                    }
                                }
                                else if (CustomId == 1)
                                {
                                    if (MyMod.LobbyState != "Vote")
                                    {
                                        ChangeMenuItems("LobbySettings");
                                    }else{
                                        ChangeMenuItems("Vote");
                                    }
                                }
                                else if (CustomId == 2)
                                {
                                    if (SteamConnect.CanUseSteam)
                                    {
                                        SteamConnect.Main.MakeFriendListUI();
                                    }
                                }
                                else if (CustomId == 3)
                                {
                                    if (SteamConnect.CanUseSteam)
                                    {
                                        SteamConnect.Main.CopyInviteLink();
                                        InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, "Link copied to clipboard.", Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null);
                                    }
                                }
                            }
                            else if (MenuMode == "LobbySettings")
                            {
                                if (CustomId == 1)
                                {
                                    if (!MyMod.StartServerAfterSelectSave)
                                    {
                                        ChangeMenuItems("NewGameSelect");
                                    }else{
                                        MyMod.m_Panel_Sandbox.OnClickNew();
                                    }
                                }
                                else if (CustomId == 2)
                                {
                                    CloseLobbyUI();
                                    MyMod.m_Panel_Sandbox.OnClickLoad();
                                }
                                else if (CustomId == 3)
                                {
                                    CloseLobbyUI();
                                    MyMod.m_Panel_Sandbox.OnClickFeats();
                                }
                            }
                            else if (MenuMode == "NewGameSelect")
                            {
                                if (CustomId == 2)
                                {
                                    MyMod.SelectedSaveName = "";
                                    MyMod.SelectedSaveSeed = 0;
                                    SteamConnect.Main.SetSaveSlotSeed(0);
                                    SetLobbyState("Vote");
                                    ChangeMenuItems("Lobby");
                                }
                                else if (CustomId == 3)
                                {
                                    CloseLobbyUI();
                                    MyMod.m_Panel_Sandbox.OnClickNew();
                                }
                            }
                            else if (MenuMode == "Vote")
                            {
                                if (CustomId == 2)
                                {
                                    CloseLobbyUI();
                                    MyMod.m_Panel_Sandbox.Enable(false);
                                    InterfaceManager.TrySetPanelEnabled<Panel_SelectRegion_Map>(true);
                                }
                                else if (CustomId == 3)
                                {
                                    CloseLobbyUI();
                                    MyMod.m_Panel_Sandbox.Enable(false);
                                    InterfaceManager.TrySetPanelEnabled<Panel_SelectExperience>(true);
                                }
                            }
                        }else{
                            return true;
                        }
                    }
                    return false;
                }
                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Sandbox), "ConfigureMenu")]
        private static class Panel_Sandbox_ConfigureMenu
        {
            private static bool Prefix(Panel_Sandbox __instance)
            {
                bool EverPlayer = SaveGameSlotHelper.GetNumSaveSlots(SaveSlotType.SANDBOX) > 0;
                if (!EverPlayer)
                {
                    MelonLogger.Msg(ConsoleColor.Yellow, "No saves found, patching menu to not shitup ui");
                    if (__instance.m_BasicMenu == null)
                    {
                        MelonLogger.Msg(ConsoleColor.Red, "OH NO __instance.m_BasicMenu null");
                        return false;
                    }

                    __instance.m_BasicMenu.Reset();
                    __instance.m_BasicMenu.UpdateTitle(__instance.m_TitleLocalizationId, "", Vector3.zero);
                    for (int index = 0; index < __instance.m_MenuItems.Count; ++index)
                    {
                        __instance.AddMenuItem(index);
                    }
                    __instance.m_BasicMenu.SetBackAction(new System.Action(__instance.OnClickBack));
                    __instance.m_BasicMenu.EnableConfirm(false, "GAMEPLAY_Select");
                    return false;
                }
                return true;
            }
        }

        public static bool CheckSaveRNG(string name)
        {
            int GenVersion = 0;
            string textToShow;
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_genversion");
            if (data != null)
            {
                int[] saveProxy = JSON.Load(data).Make<int[]>();
                GenVersion = saveProxy[0];
                if (GenVersion != MyMod.BuildInfo.RandomGenVersion)
                {
                    MelonLogger.Msg(ConsoleColor.DarkRed, "This save file can't be use for multiplayer, because we created on old version of the mod, with Generation version " + GenVersion + ". Release of mod you using right now has Generation version " + MyMod.BuildInfo.RandomGenVersion);
                }
            }else{
                MelonLogger.Msg(ConsoleColor.DarkRed, "This save file can't be use for multiplayer, because was created on old version of mod or without mod at all.");
            }

            if (GenVersion != MyMod.BuildInfo.RandomGenVersion)
            {
                if (GenVersion == 0)
                {
                    textToShow = "You can't use this save file for hosting multiplayer! Because this save file has been created before mod has been installed, or on old version of the mod that isn't compatible with current one!";
                }else{
                    textToShow = "You can't use this save file for hosting multiplayer! Because this save file has been created on old version of the mod that isn't compatible with current one! Save file Generation version " + GenVersion + ". Current one mod use now " + MyMod.BuildInfo.RandomGenVersion + "!";
                }

                if (MyMod.m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
                {
                    //if (SaveGameSystem.m_CurrentGameMode == SaveSlotType.STORY)
                    //{
                    //    textToShow = "Story mode never has been planned to be synced. Mod works only in SANDBOX game mode. I not know why you even try to host story mode, we never announced it will ever work! Please play regular sandbox!";
                    //}

                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, textToShow, Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null);
                }
                return false;
            }
            return true;
        }
    }
}

