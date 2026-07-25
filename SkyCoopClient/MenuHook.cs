using Il2Cpp;
using Il2CppSteamworks;
using MelonLoader;
using SkyCoopClient;
using System.Text;
using UnityEngine;
using static Il2Cpp.Utils;
using static Il2CppMono.Security.X509.X520;

namespace SkyCoop
{
    public class MenuHook
    {
        public static string s_CurrenetMenuOverride = "Original";
        public static bool s_SkyCoopSettingsForced = false;
        public static string s_PendingSquadInvite = "";

        public static Comps.TexasHoldEmPlay s_RaisBetHook;

        public static void AddButton(BasicMenu Menu, string Text, string Description, int order, Action Exec = null, bool Locked = false)
        {
            BasicMenu.BasicMenuItemModel basicMenuItemModel = new BasicMenu.BasicMenuItemModel("", order, order, Localization.Get(Text), Localization.Get(Description), "", Exec, Color.gray, Color.white);
            basicMenuItemModel.m_IsLocked = Locked;

            Menu.m_ItemModelList.Insert(order, basicMenuItemModel);

            foreach (IBasicMenuExtension basicMenuExtension in Menu.m_MenuExtensions)
            {
                basicMenuExtension.ItemAdded(basicMenuItemModel);
            }
        }

        public static void ShowMultiplayerSettings()
        {
            s_SkyCoopSettingsForced = true;
            GameAudioManager.PlayGUIButtonClick();
            Settings.ForceToShow();
        }

        public static void ShowServerSettings()
        {
            s_SkyCoopSettingsForced = true;
            GameAudioManager.PlayGUIButtonClick();
            Settings.ForceToShow(true);
        }

        public static void SetMenuOverrideMode(string mode)
        {
            Panel_Sandbox Panel = InterfaceManager.GetPanel<Panel_Sandbox>();

            s_CurrenetMenuOverride = mode;

            Logger.Log("[UI] ChangeMenuItems s_CurrenetMenuOverride " + s_CurrenetMenuOverride);
        }

        public static void UpdateSandboxMainWindow()
        {
            UpdateSandboxMainWindow(InterfaceManager.GetPanel<Panel_Sandbox>().m_MainWindow);
        }

        public static void UpdateSandboxMainWindow(GameObject Obj)
        {
            bool FoundReborn = false;
            for (int i = 0; i < Obj.transform.childCount; i++)
            {
                Transform T = Obj.transform.GetChild(i);
                if (T)
                {
                    if (T.name.EndsWith("Title_Texture") || T.name.EndsWith("Update_Title_Texture") || T.name.EndsWith("_Update_Title"))
                    {
                        T.gameObject.SetActive(false);
                    }

                    if(T.name == "SkyCoopRebornTitle_Texture")
                    {
                        FoundReborn = true;
                        T.gameObject.SetActive(s_CurrenetMenuOverride == "Multiplayer");
                    }
                }
            }
            if(!FoundReborn)
            {
                Transform VictimForClone = Obj.transform.FindChild("SurvivalTitle_Texture");
                if (VictimForClone)
                {
                    GameObject Clone = UnityEngine.Object.Instantiate(VictimForClone.gameObject, VictimForClone.parent);
                    Clone.name = "SkyCoopRebornTitle_Texture";
                    Clone.GetComponent<UITexture>().mainTexture = AssetManager.GetAssetFromGame<Texture2D>("Titles_SkyCoopReborn_Texture");
                    Clone.SetActive(s_CurrenetMenuOverride == "Multiplayer");
                }
            }
        }

        public static void OnMultiplayerPressed()
        {
            SetMenuOverrideMode("Multiplayer");
            InterfaceManager.TrySetPanelEnabled<Panel_MainMenu>(false);
            InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(true);

            GameAudioManager.PlayGUIButtonClick();

            UpdateSandboxMainWindow(InterfaceManager.GetPanel<Panel_Sandbox>().m_MainWindow);
        }

        public static void OnMuliplayerBackPressed()
        {
            SetMenuOverrideMode("Original");
            InterfaceManager.TrySetPanelEnabled<Panel_MainMenu>(true);
            InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(false);
            Transform T = InterfaceManager.GetPanel<Panel_Sandbox>().m_MainWindow.transform.FindChild("SkyCoopRebornTitle_Texture");
            if (T)
            {
                T.gameObject.SetActive(false);
            }
            GameAudioManager.PlayGUIButtonClick();
        }

        public static void OnHostPressed()
        {            
            if (ModMain.Server.m_IsReady)
            {
                OnShutdownPressed();
            }
            else
            {
                if (!Environment.GetCommandLineArgs().Contains("-JoeBiden"))
                {
                    DoOKMessage("Stop it!", "No, you can't host yourself in this build.\nWait when we host.");
                    return;
                }

                ModMain.Server.StartServer();
                Thread.Sleep(15);
                ModMain.Client.ConnectToServer("localhost");
                SetMenuOverrideMode("Original");
                //OpenSandbox();
            }
        }

        public static void OnShutdownConfirmed()
        {
            RemovePleaseWait();

            ModMain.Server.DisconnectAllPlayers("Server shutdown", true);
            ModMain.Server = new SkyCoopServer.Server();

            DoOKMessage("", "GAMEPLAY_ShutdownServerDone");

            InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(false);
            InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(true);
            UpdateSandboxMainWindow();
        }

        public static void OnShutdownPressed()
        {
            RemovePleaseWait();
            InterfaceManager.GetPanel<Panel_Confirmation>().AddConfirmation(Panel_Confirmation.ConfirmationType.Confirm, Localization.Get("GAMEPLAY_ShutdownServerConfirmation"), Panel_Confirmation.ButtonLayout.Button_2, "GAMEPLAY_ShutdownServer", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, new Action(OnShutdownConfirmed), null);
        }

        public static void OnDisconnectConfirmed()
        {
            ModMain.s_MapEditor = false;
            DebugGUI.s_Open = false;
            PlayersManager.DestoryPlayers();
            if (ModMain.Client != null && ModMain.Client.m_IsReady)
            {
                ModMain.Client.m_Instance.DisconnectAll();
                ModMain.Client.m_Instance.Stop();
                ModMain.Client.Dispose();
                ModMain.Client = new Client();
            }
            if (ModMain.ClientVoice != null && ModMain.ClientVoice.m_IsReady)
            {
                ModMain.ClientVoice.m_Instance.DisconnectAll();
                ModMain.ClientVoice.m_Instance.Stop();
                ModMain.ClientVoice.Dispose();
                ModMain.ClientVoice = new ClientVoice();
            }

            InterfaceManager.GetPanel<Panel_PauseMenu>().DoQuitGame();
        }

        public static void OnDisconnectPressed()
        {
            RemovePleaseWait();
            Panel_Confirmation Con = InterfaceManager.GetPanel<Panel_Confirmation>();
            string TextLocID = "";
            string DisconnectLocID = "";

            if (ModMain.Client.m_IsReady)
            {
                if (ModMain.Server.m_IsReady)
                {
                    TextLocID = "GAMEPLAY_DisconnectConfirmationHost";
                }
                else
                {
                    TextLocID = "GAMEPLAY_DisconnectConfirmation";
                }
                DisconnectLocID = "GAMEPLAY_Disconnect";
            }
            else
            {
                TextLocID = "GAMEPLAY_CloseMapEditorConfirmation";
                DisconnectLocID = "GAMEPLAY_Quit";
            }
            InterfaceManager.GetPanel<Panel_Confirmation>().AddConfirmation(Panel_Confirmation.ConfirmationType.Confirm, TextLocID, Panel_Confirmation.ButtonLayout.Button_2, DisconnectLocID, "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, new Action(OnDisconnectConfirmed), null);
        }

        public static void OnDisconnected(string Reason = "Unknown")
        {
            RemovePleaseWait();

            Panel_Confirmation Con = InterfaceManager.GetPanel<Panel_Confirmation>();
            Con.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, $"Server shutdown\nReason: {Reason}", Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, new Action(OnDisconnectConfirmed), null);
        }

        public static void OnJoinConfirm()
        {
            string text = InterfaceManager.GetPanel<Panel_Confirmation>().m_CurrentGroup.m_InputField.GetText();
            ModMain.Client.ConnectToServer(text);
        }

        public static void OnJoinPressed()
        {
            GameAudioManager.PlayGUIButtonClick();
            if (ModMain.Client.m_IsReady)
            {
                RemovePleaseWait();
                DoOKMessage("", "GAMEPLAY_AlreadyConnected");
            }
            else
            {
                InterfaceManager.GetPanel<Panel_Confirmation>().AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, Localization.Get("GAMEPLAY_ServerAddressField"), "127.0.0.1", Panel_Confirmation.ButtonLayout.Button_2, "GAMEPLAY_Connect", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, new Action(OnJoinConfirm), null);
            }
        }

        public static void OnSettingsPressed()
        {
            Panel_Sandbox Panel = InterfaceManager.GetPanel<Panel_Sandbox>();
            Panel.OnClickOptions();
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "ConfigureMenu", null)]
        public class Panel_MainMenu_ConfigureMenu
        {
            public static void Postfix(Panel_MainMenu __instance)
            {
                AddButton(__instance.m_BasicMenu, "GAMEPLAY_Multiplayer", "GAMEPLAY_MultiplayerDescription", __instance.m_BasicMenu.m_ItemModelList.Count-1, new Action(OnMultiplayerPressed));

                if (!ModMain.s_MenuEverLoaded)
                {
                    ModMain.s_MenuEverLoaded = true;
                }
            }
        }

        public static void OnMapEditorTools()
        {
            DebugGUI.Toggle();
        }

        public static void SendSquadName()
        {
            ClientSend.SendCreateSquadRequest(InterfaceManager.GetPanel<Panel_Confirmation>().m_CurrentGroup.m_InputField.GetText());
        }

        public static void OnCreateSquad()
        {
            InterfaceManager.TrySetPanelEnabled<Panel_PauseMenu>(false);
            RemovePleaseWait();
            InterfaceManager.GetPanel<Panel_Confirmation>().AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input name for squad", "", Panel_Confirmation.ButtonLayout.Button_2, "GAMEPLAY_Confirm", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, new Action(SendSquadName), null);
        }

        public static void OnLeaveSquad()
        {
            InterfaceManager.TrySetPanelEnabled<Panel_PauseMenu>(false);
            ClientSend.SendLeaveSquadRequest();
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_PauseMenu), "ConfigureMenu", null)]
        public class Panel_PauseMenu_ConfigureMenu
        {
            public static void Postfix(Panel_PauseMenu __instance)
            {
                if (ModMain.s_MapEditor)
                {
                    AddButton(__instance.m_BasicMenu, "MAP EDITOR", "Opens map editor tools", 0, new Action(OnMapEditorTools));
                }else if (ModMain.Client.m_IsReady && ModMain.Client.m_Config.m_GameMode == "Lobby")
                {
                    if (!PlayersManager.s_InSquad)
                    {
                        AddButton(__instance.m_BasicMenu, "Create Squad", "Create Squad", 1, new Action(OnCreateSquad));

                    }
                    else
                    {
                        AddButton(__instance.m_BasicMenu, "Leave Squad", "Leave Squad", 1, new Action(OnLeaveSquad));
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Sandbox), "ConfigureMenu", null)]
        public class Panel_Sandbox_ConfigureMenu
        {
            public static void Postfix(Panel_Sandbox __instance)
            {
                if (s_CurrenetMenuOverride == "Multiplayer")
                {
                    __instance.m_BasicMenu.Reset();
                    __instance.m_BasicMenu.UpdateTitle("", "", Vector3.zero);

                    if(ModMain.Server != null && ModMain.Server.m_IsReady)
                    {
                        AddButton(__instance.m_BasicMenu, "GAMEPLAY_ShutdownServer", "GAMEPLAY_ShutdownServerDescription", 0, new Action(OnHostPressed));
                    }
                    else
                    {
                        AddButton(__instance.m_BasicMenu, "GAMEPLAY_Host", "GAMEPLAY_HostDescription", 0, new Action(OnHostPressed), !Environment.GetCommandLineArgs().Contains("-JoeBiden"));
                    }

                    AddButton(__instance.m_BasicMenu, "GAMEPLAY_Join", "GAMEPLAY_JoinDescription", 1, new Action(OnJoinPressed));
                    AddButton(__instance.m_BasicMenu, "GAMEPLAY_MapEditor", "GAMEPLAY_MapEditorDescription", 2, new Action(GoToMapEditor));
                    AddButton(__instance.m_BasicMenu, "GAMEPLAY_Options", "GAMEPLAY_OptionsMultiplayerDescription", 3, new Action(OnSettingsPressed));

                    __instance.m_BasicMenu.SetBackAction(new Action(OnMuliplayerBackPressed));
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_OptionsMenu), "ExitOptions", null)]
        public class Panel_OptionsMenu_CloseSelf
        {
            public static void Postfix(Panel_OptionsMenu __instance)
            {
                if (!ModMain.IsGameplayScene())
                {
                    SetMenuOverrideMode(s_CurrenetMenuOverride);
                    UpdateSandboxMainWindow();
                }
            }
        }

        public static void GoToMapEditor()
        {
            InterfaceManager.TrySetPanelEnabled<Panel_OptionsMenu>(false);
            ModMain.s_MapEditor = true;
            ModMain.SetupSurvivalSettings("Stalker", -666, "CoastalRegion");
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_OptionsMenu), "ConfigureMenu", null)]
        public class Panel_OptionsMenu_ConfigureMenu
        {
            public static void Postfix(Panel_OptionsMenu __instance)
            {
                AddButton(__instance.m_BasicMenu, "GAMEPLAY_SkyCoopSettings", "GAMEPLAY_SkyCoopSettingsDescription", 6, new Action(ShowMultiplayerSettings));
                //AddButton(__instance.m_BasicMenu, "Server Setting Test", "Server Setting Test", 7, new Action(ShowServerSettings));
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_OptionsMenu), "OnCancel", null)]
        public class Panel_OptionsMenu_OnCancel
        {
            public static void Postfix(Panel_OptionsMenu __instance)
            {
                if (s_SkyCoopSettingsForced)
                {
                    s_SkyCoopSettingsForced = false;
                    Settings.BackFromForcedMenu();
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Boot), "Update")]
        internal static class Panel_Boot_Update
        {
            private static void Prefix(Panel_Boot __instance)
            {
                if (!ModMain.s_ModBooted)
                {
                    ModMain.s_ModBooted = true;
                    ModMain.OnGameBoot();
                }
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
            Con.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, Localization.Get(title), "\n" + Localization.Get(txt), Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null, null);
        }

        public static void OpenSandbox()
        {
            SetMenuOverrideMode("Original");
            InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(true);
        }

        public static void AcceptSquadInvite()
        {
            ClientSend.SendAcceptSquadInvite(s_PendingSquadInvite);
            s_PendingSquadInvite = "";
        }

        public static void RefuseSquadInvite()
        {
            ClientSend.SendRefuseJoinSquad(s_PendingSquadInvite);
            s_PendingSquadInvite = "";
        }

        public static void DoInviteSquadMessage(string SquadName)
        {
            RemovePleaseWait();
            s_PendingSquadInvite = SquadName;
            InterfaceManager.GetPanel<Panel_Confirmation>().AddConfirmation(Panel_Confirmation.ConfirmationType.Confirm, $"Do you want to join squad {SquadName}?", Panel_Confirmation.ButtonLayout.Button_2, "GAMEPLAY_Join", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, new Action(AcceptSquadInvite), null);
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

        [HarmonyLib.HarmonyPatch(typeof(Panel_PauseMenu), "OnQuitGame", null)]
        public class Panel_PauseMenu_OnQuitGame
        {
            public static bool Prefix(Panel_PauseMenu __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                OnDisconnectPressed();

                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Confirmation), "CancelConfirmation", null)]
        public class Panel_Confirmation_CancelConfirmation
        {
            public static void Prefix(Panel_Confirmation __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (!string.IsNullOrEmpty(s_PendingSquadInvite))
                {
                    RefuseSquadInvite();
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Il2Cpp.InputManager), "ProcessInput")]
        private static class InputManager_ProcessInput
        {
            private static bool Prefix()
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (CanvasUI.m_ChatIsOpen)
                {
                    return false;
                }
                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Il2Cpp.InputManager), "GetPlayerMovement")]
        private static class InputManager_GetPlayerMovement
        {
            private static void Postfix(ref Vector2 __result)
            {
                if (CanvasUI.m_ChatIsOpen)
                {
                    __result = Vector2.zero;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Il2Cpp.InputManager), "GetCameraMovementMouse")]
        private static class InputManager_GetCameraMovementMouset
        {
            private static void Postfix(ref Vector2 __result)
            {
                if (CanvasUI.m_ChatIsOpen)
                {
                    __result = Vector2.zero;
                }
            }
        }
    }
}
