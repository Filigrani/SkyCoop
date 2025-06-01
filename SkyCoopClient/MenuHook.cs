using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace SkyCoop
{
    public class MenuHook
    {
        public static string s_CurrenetMenuOverride = "Original";

        public static Comps.TexasHoldEmPlay s_RaisBetHook;

        public static void AddButton(Panel_MainMenu __instance, string name, int order, int plus)
        {
            Panel_MainMenu.MainMenuItem mainMenuItem = new Panel_MainMenu.MainMenuItem();
            mainMenuItem.m_LabelLocalizationId = name;
            mainMenuItem.m_Type = (Panel_MainMenu.MainMenuItem.MainMenuItemType)30 + plus;
            __instance.m_MenuItems.Insert(order, mainMenuItem);

            string id = __instance.m_MenuItems[order].m_Type.ToString();
            int type = (int)__instance.m_MenuItems[order].m_Type;

            __instance.m_BasicMenu.AddItem(id, type, order, name, "", "", new Action(__instance.OnSandbox), Color.gray, Color.white);
        }

        public static void OverrideMenuButton(Transform Grid, int index, string txt, bool onClickHack = true)
        {
            UILabel Label = Grid.GetChild(index).GetChild(0).GetComponent<UILabel>();
            UIButton Button = Grid.GetChild(index).GetComponent<UIButton>();
            DoubleClickButton DoubleClickButton = Grid.GetChild(index).GetComponent<DoubleClickButton>();

            Comps.UiButtonKeyboardPressSkip Skip = Grid.GetChild(index).gameObject.GetComponent<Comps.UiButtonKeyboardPressSkip>();

            if (onClickHack)
            {
                if(Skip == null)
                {
                    Skip = Grid.GetChild(index).gameObject.AddComponent<Comps.UiButtonKeyboardPressSkip>();
                }
                Skip.m_Click = Button.onClick;
                Button.onClick = null;
                if (DoubleClickButton)
                {
                    Skip.m_DoubleClick = DoubleClickButton.m_OnClick;
                    Skip.m_DoubleDoubleClick = DoubleClickButton.m_OnDoubleClick;
                    DoubleClickButton.m_OnClick = null;
                    DoubleClickButton.m_OnDoubleClick = null;
                }
            }

            Label.mText = txt;
            Label.text = txt;
            Label.ProcessText();
            Grid.GetChild(index).gameObject.SetActive(true);
        }

        public static void ReturnOriginalButtons(Transform Grid, int index)
        {
            UIButton Button = Grid.GetChild(index).GetComponent<UIButton>();
            DoubleClickButton DoubleClick = Grid.GetChild(index).GetComponent<DoubleClickButton>();
            if (Grid.GetChild(index).GetComponent<Comps.UiButtonKeyboardPressSkip>() != null)
            {
                Comps.UiButtonKeyboardPressSkip Skip = Grid.GetChild(index).gameObject.GetComponent<Comps.UiButtonKeyboardPressSkip>();
                
                if(Button.onClick == null)
                {
                    Button.onClick = Skip.m_Click;
                }
                if (DoubleClick)
                {
                    if(DoubleClick.m_OnClick == null)
                    {
                        DoubleClick.m_OnClick = Skip.m_DoubleClick;
                    }
                    if(DoubleClick.m_OnDoubleClick == null)
                    {
                        DoubleClick.m_OnDoubleClick = Skip.m_DoubleDoubleClick;
                    }
                }
            }
        }

        public static void ClearMenuButtons(Transform Grid)
        {
            for (int i = 0; i <= 6; i++)
            {
                Grid.GetChild(i).gameObject.SetActive(false);
            }
        }

        public static void ChangeMenuItems(string mode)
        {
            Panel_Sandbox Panel = InterfaceManager.GetPanel<Panel_Sandbox>();

            s_CurrenetMenuOverride = mode;

            Logger.Log("[UI] ChangeMenuItems s_CurrenetMenuOverride " + s_CurrenetMenuOverride);

            //                                                   RootMenu    Menu        Left_Align  Grid
            Transform Grid = Panel.gameObject.transform.GetChild(0).GetChild(0).GetChild(6).GetChild(3);

            if (mode == "Original")
            {
                ReturnOriginalButtons(Grid, 0);
                ReturnOriginalButtons(Grid, 1);
                ReturnOriginalButtons(Grid, 2);
                ReturnOriginalButtons(Grid, 3);
                Panel.gameObject.transform.GetChild(3).GetChild(2).gameObject.SetActive(true);//SurvivalTitle_Texture
                return;
            }

            InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(true);

            Panel.gameObject.transform.GetChild(3).GetChild(2).gameObject.SetActive(false);//SurvivalTitle_Texture
            ClearMenuButtons(Grid);

            if (mode == "Multiplayer")
            {
                OverrideMenuButton(Grid, 1, "HOST SERVER");
                OverrideMenuButton(Grid, 2, "JOIN SERVER");
                OverrideMenuButton(Grid, 3, "OPTIONS");
            }else if (mode == "MultiProfileSettings")
            {
                OverrideMenuButton(Grid, 1, "CHANGE NAME");
                OverrideMenuButton(Grid, 2, "CUSTOMIZATION");
                OverrideMenuButton(Grid, 3, "COPY ID");
                OverrideMenuButton(Grid, 4, "GAME SETTINGS");
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BasicMenu), "UpdateDescription", null)]
        internal class BasicMenu_UpdateDescription_Post
        {
            public static void Postfix(BasicMenu __instance, int buttonIndex)
            {
                if (__instance == null || buttonIndex >= __instance.m_ItemModelList.Count)
                {
                    return;
                }
                BasicMenu.BasicMenuItemModel Button = __instance.m_ItemModelList[buttonIndex];
                if (Button == null)
                {
                    return;
                }

                if(s_CurrenetMenuOverride == "" || s_CurrenetMenuOverride == "Original")
                {
                    if (Button.m_DescriptionText == "GAMEPLAY_Description31")
                    {
                        __instance.m_DescriptionLabel.text = "Host or join a multiplayer session.";
                    }
                }else if (s_CurrenetMenuOverride == "Multiplayer")
                {
                    if (Button.m_Id == "GAMEPLAY_Resume")
                    {
                        __instance.m_DescriptionLabel.text = "Configure and host a session.";
                    } else if (Button.m_Id == "GAMEPLAY_Load")
                    {
                        //if (SteamConnect.CanUseSteam)
                        //{
                        //    __instance.m_DescriptionLabel.text = "Find a server or join by IP address.";
                        //} else
                        //{
                            __instance.m_DescriptionLabel.text = "Join by IP address.";
                        //}
                    } else if (Button.m_Id == "GAMEPLAY_Challenges")
                    {
                        __instance.m_DescriptionLabel.text = "Base game settings and multiplayer settings.";
                    }
                }else if (s_CurrenetMenuOverride == "Join")
                {
                    if (Button.m_Id == "GAMEPLAY_Resume")
                    {
                        __instance.m_DescriptionLabel.text = "Browse public servers.";
                    } else if (Button.m_Id == "GAMEPLAY_Load")
                    {
                        __instance.m_DescriptionLabel.text = "Connect to a server by IP address.";
                    } else if (Button.m_Id == "GAMEPLAY_Challenges")
                    {
                        __instance.m_DescriptionLabel.text = "Opens steam friends overlay.";
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "Initialize", null)]
        public class Panel_MainMenu_Start
        {
            public static void Postfix(Panel_MainMenu __instance)
            {
                Logger.Log("[UI] Trying modify main menu...");
                AddButton(__instance, "MULTIPLAYER", 3, 1);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(UIButton), "OnClick")]
        internal class UIButton_Press
        {
            private static bool Prefix(UIButton __instance)
            {
                //Logger.Log("UIButton OnClick");
                if (__instance.gameObject != null && __instance.gameObject.GetComponent<Comps.UiButtonPressHook>() != null)
                {
                    Comps.UiButtonPressHook Hook = __instance.gameObject.GetComponent<Comps.UiButtonPressHook>();
                    Logger.Log("Clicked m_CustomId " + Hook.m_CustomId);
                    Logger.Log("Clicked m_PanelHandle " + Hook.m_PanelHandle);
                    Logger.Log("s_CurrenetMenuOverride " + s_CurrenetMenuOverride);

                    if (Hook.m_PanelHandle == "Panel_MainMenu") 
                    {
                        if(Hook.m_CustomId == 3) // MULTIPLAYER MAIN MENU
                        {
                            ChangeMenuItems("Multiplayer");
                            InterfaceManager.TrySetPanelEnabled<Panel_MainMenu>(false);
                        }
                    }else if (Hook.m_PanelHandle == "Panel_Sandbox")
                    {
                        if(s_CurrenetMenuOverride == "Multiplayer")
                        {
                            if (Hook.m_CustomId == 1) // Host server
                            {
                                if (ModMain.Server.m_IsReady)
                                {
                                    RemovePleaseWait();
                                    DoOKMessage("Server already up!", "You already hosting server!");
                                } else
                                {
                                    ModMain.Server.StartServer();
                                    Thread.Sleep(15);
                                    ModMain.Client.ConnectToServer("localhost");
                                    OpenSandbox();
                                }
                            } else if (Hook.m_CustomId == 2) // Join server
                            {
                                if (ModMain.Client.m_IsReady)
                                {
                                    RemovePleaseWait();
                                    DoOKMessage("", "You already connected to the server!");
                                } else
                                {
                                    InterfaceManager.GetPanel<Panel_Confirmation>().AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "INPUT SERVER ADDRESS", "127.0.0.1", Panel_Confirmation.ButtonLayout.Button_2, "Connect", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                                }
                            } else if (Hook.m_CustomId == 3)
                            {
                                Panel_Sandbox Panel = InterfaceManager.GetPanel<Panel_Sandbox>();
                                Panel.OnClickOptions();
                            }
                        }
                    }
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Sandbox), "OnClickBack", null)]
        public class Panel_Sandbox_OnClickBack
        {
            public static bool Prefix(Panel_Sandbox __instance)
            {
                if (s_CurrenetMenuOverride == "Multiplayer")
                {
                    ChangeMenuItems("Original");
                    InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(false);
                    InterfaceManager.TrySetPanelEnabled<Panel_MainMenu>(true);
                    return true;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_OptionsMenu), "ExitOptions", null)]
        public class Panel_OptionsMenu_CloseSelf
        {
            public static void Postfix(Panel_OptionsMenu __instance)
            {
                ChangeMenuItems(s_CurrenetMenuOverride);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "Update", null)]
        public class Panel_MainMenu_Update
        {
            public static void Postfix(Panel_MainMenu __instance)
            {
                // MainPanel/MenuRoot/Menu/Left_Align/Grid
                // 0        /0       /0   /6         /3
                Transform Grid = __instance.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(6).GetChild(3);
                for (int i = 0; i <= 6; i++)
                {
                    GameObject Button = Grid.GetChild(i).gameObject;

                    if (Button.GetComponent<UIButton>() != null)
                    {
                        if (Button.GetComponent<Comps.UiButtonPressHook>() == null)
                        {
                            Button.AddComponent<Comps.UiButtonPressHook>();
                            Button.GetComponent<Comps.UiButtonPressHook>().m_CustomId = i;
                            Button.GetComponent<Comps.UiButtonPressHook>().m_PanelHandle = __instance.GetType().Name;
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Sandbox), "Update", null)]
        public class Panel_Sandbox_Update
        {
            public static void Postfix(Panel_Sandbox __instance)
            {
                // RootMenu/Menu/Left_Align/Grid
                // 0       /0   /6         /3
                Transform Grid = __instance.gameObject.transform.GetChild(0).GetChild(0).GetChild(6).GetChild(3);
                for (int i = 0; i <= 6; i++)
                {
                    GameObject Button = Grid.GetChild(i).gameObject;

                    if (Button.GetComponent<UIButton>() != null)
                    {
                        if (Button.GetComponent<Comps.UiButtonPressHook>() == null)
                        {
                            Button.AddComponent<Comps.UiButtonPressHook>();
                            Button.GetComponent<Comps.UiButtonPressHook>().m_CustomId = i;
                            Button.GetComponent<Comps.UiButtonPressHook>().m_PanelHandle = __instance.GetType().Name;
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Confirmation), "OnConfirm")]
        private static class Panel_Confirmation_OnConfirm
        {
            internal static void Postfix(Panel_Confirmation __instance)
            {
                if (__instance.m_CurrentGroup != null)
                {
                    MelonLogger.Msg(ConsoleColor.Blue, "__instance.m_CurrentGroup");

                    string Message = __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text;
                    string text = __instance.m_CurrentGroup.m_InputField.GetText();
                    MelonLogger.Msg(ConsoleColor.Blue, "__instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text " + Message);
                    switch (Message)
                    {
                        case "INPUT SERVER ADDRESS":
                            ModMain.Client.ConnectToServer(text);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BootUpdate), "Start")]
        internal static class InitializePatch
        {
            private static void Prefix()
            {
                ModMain.OnGameBoot();
            }
        }

        public static void DoPleaseWait(string title, string text)
        {
            Panel_Confirmation Con = InterfaceManager.GetPanel<Panel_Confirmation>();
            if (Con == null)
            {
                return;
            }
            Con.AddConfirmation(Panel_Confirmation.ConfirmationType.Waiting, title, "\n" + text, Panel_Confirmation.ButtonLayout.Button_0, Panel_Confirmation.Background.Transperent, null, null);
        }
        public static void RemovePleaseWait()
        {
            Panel_Confirmation Con = InterfaceManager.GetPanel<Panel_Confirmation>();
            if (Con == null)
            {
                return;
            }
            Con.OnCancel();
        }

        public static void DoOKMessage(string title, string txt)
        {
            Panel_Confirmation Con = InterfaceManager.GetPanel<Panel_Confirmation>();
            if (Con == null)
            {
                return;
            }
            Con.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, title, "\n" + txt, Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null, null);
        }

        public static void OpenSandbox()
        {
            ChangeMenuItems("Original");
            InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(true);
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_PickUnits), "Refresh", null)]
        public class Panel_PickUnits_Refresh
        {
            public static bool Prefix(Panel_PickUnits __instance)
            {
                if (s_RaisBetHook != null)
                {
                    __instance.m_Label_NumUnits.text = __instance.m_numUnits.ToString() + "/" + __instance.m_maxUnits.ToString();
                    __instance.m_GearIcon.mainTexture = Utils.GetInventoryIconTextureFromPrefabName("GEAR_CashBundle");
                    __instance.m_Label_Description.text = "How much to bet?";

                    if(s_RaisBetHook.m_Player)
                    {
                        int CurrentBet = s_RaisBetHook.m_Player.m_Bet;
                        int MaxBet = s_RaisBetHook.m_Player.m_Game.GetMaxBet();
                        if (__instance.m_numUnits+ CurrentBet <= MaxBet)
                        {
                            int MinBet = (MaxBet+1)-(__instance.m_numUnits + CurrentBet);
                            __instance.m_Label_Description.text +=  $"\n[FF0000]You need to bet at least {MinBet} more![-]";
                        }
                    }

                    Utils.GetComponentInChildren<UILabel>(__instance.m_Execute_Button).text = Localization.Get("Bet");
                    Utils.GetComponentInChildren<UILabel>(__instance.m_ExecuteAll_Button).text = Localization.Get("ALL-IN");
                    __instance.m_ExecuteAction = PickUnitsExecuteAction.Drop;
                    __instance.m_ButtonLegendContainer.BeginUpdate();
                    __instance.m_ButtonLegendContainer.UpdateButton("Inventory_Examine", Utils.GetComponentInChildren<UILabel>(__instance.m_ExecuteAll_Button).text, true, 2, false);
                    __instance.m_ButtonLegendContainer.UpdateButton("Inventory_Equip", Utils.GetComponentInChildren<UILabel>(__instance.m_Execute_Button).text, true, 1, false);
                    __instance.m_ButtonLegendContainer.UpdateButton("Escape", "GAMEPLAY_ButtonBack", true, 0, true);
                    __instance.m_ButtonLegendContainer.EndUpdate();
                    __instance.m_ButtonIncrease.SetActive(__instance.m_numUnits < __instance.m_maxUnits);
                    __instance.m_ButtonDecrease.SetActive(__instance.m_numUnits > 0);
                    return false;
                }

                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_PickUnits), "OnExecute", null)]
        public class Panel_PickUnits_OnExecute
        {
            public static bool Prefix(Panel_PickUnits __instance)
            {
                if (s_RaisBetHook != null)
                {
                    int CurrentBet = s_RaisBetHook.m_Player.m_Bet;
                    int MaxBet = s_RaisBetHook.m_Player.m_Game.GetMaxBet();
                    if (__instance.m_numUnits + CurrentBet <= MaxBet)
                    {
                        HUDMessage.AddMessage($"[FF0000]You need to bet at least {MaxBet+1}![-]", true, true);
                        GameAudioManager.PlayGUIError();
                    }
                    else
                    {
                        s_RaisBetHook.SendActionRaise(__instance.m_numUnits);
                    }

                    
                    s_RaisBetHook = null;
                    __instance.ExitInterface();
                    return false;
                }

                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_PickUnits), "OnExecuteAll", null)]
        public class Panel_PickUnits_OnExecuteAll
        {
            public static bool Prefix(Panel_PickUnits __instance)
            {
                if (s_RaisBetHook != null)
                {
                    s_RaisBetHook.SendActionAllIN();
                    s_RaisBetHook = null;
                    __instance.ExitInterface();
                    return false;
                }

                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_PickUnits), "ExitInterface", null)]
        public class Panel_PickUnits_ExitInterface
        {
            public static bool Prefix(Panel_PickUnits __instance)
            {
                s_RaisBetHook = null;

                return true;
            }
        }

        //TODO: Move it to different class
        [HarmonyLib.HarmonyPatch(typeof(vp_FPSShooter), "Fire", null)]
        public class vp_FPSShooter_Fire
        {
            public static void Prefix(vp_FPSShooter __instance)
            {
                if (__instance.m_Weapon == null || (double)Time.time < (double)__instance.m_NextAllowedFireTime || (__instance.m_Weapon.ReloadInProgress() || !GameManager.GetPlayerAnimationComponent().IsAllowedToFire(__instance.m_Weapon.m_GunItem.m_AllowHipFire)) || GameManager.GetPlayerAnimationComponent().IsReloading())
                {
                    return;
                }
                if (__instance.m_Weapon.GetAmmoCount() < 1)
                {
                    //TODO: Dry fire sound sync
                    //SendMultiplayerAudio("PLAY_RIFLE_DRY_3D");
                    return;
                } else
                {
                    if (__instance.m_Weapon.m_GunItem.m_IsJammed)
                    {
                        //TODO: Jammed sound sync
                        //SendMultiplayerAudio("PLAY_RIFLE_DRY_3D");
                        return;
                    }
                }

                //TODO: Projectile fire sync
                if (__instance.ProjectilePrefab.name == "PistolBullet")
                {
                    ClientSend.SendFire();
                }
            }
        }
    }
}
