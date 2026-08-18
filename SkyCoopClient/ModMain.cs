using Il2Cpp;
using Il2CppSteamworks;
using Il2CppTLD.Gameplay;
using Il2CppTLD.Interactions;
using Il2CppTLD.Scenes;
using MelonLoader;
using SkyCoopClient;
using SkyCoopServer;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;

namespace SkyCoop
{
    internal sealed class ModMain : MelonMod
    {
        public static Server Server;
        public static Client Client;
        public static ClientVoice ClientVoice;
        public static bool s_MapEditor = false;

        public static bool s_ModBooted = false;
        public static bool s_MenuEverLoaded = false;

        public static bool s_AppFocus = true;

        public override void OnInitializeMelon()
        {
            Server = new Server();
            Client = new Client();
            DebugConsole.RegisterCommands();
            Settings.Init();
            FilesManager.InitFolders();
            AudioHook.Init();

            Server.OnLogEvent += Logger.HandleServerLog;
        }

        public static void SetAppBackgroundMode()
        {
            if (Application.runInBackground == false)
            {
                Application.runInBackground = true; // Always running in bg
            }
            GameManager.m_IsPaused = false;
        }
        [HarmonyLib.HarmonyPatch(typeof(InputManager), "PauseGame")]
        public static class InputManager_DuckYOuPause
        {
            public static void Postfix()
            {
                GameManager.m_IsPaused = false;
            }
        }

        public static bool IsMultiplayer()
        {
            if(MenuHook.s_CurrenetMenuOverride == "Multiplayer" || (Client != null && Client.m_IsReady) || s_MapEditor)
            {
                return true;
            }
            return false;
        }

        [Obsolete]
        public override void OnApplicationStart()
        {
            Comps.RegisterComponents();
            AssetManager.PreloadMainBundle();
            WeaponsManager.InitDescriptors();
        }

        [Obsolete]
        public override void OnApplicationQuit()
        {
            if(Server != null && Server.m_IsReady)
            {
                Server.DisconnectAllPlayers("Server shutdown", true);
            }
            base.OnApplicationQuit();
        }

        [Obsolete]
        public override void OnLevelWasInitialized(int level)
        {
            MeleeManager.ReintilizeViewModels();
            GameModeHUD.Reintilize();
            //AssetManager.DumpLocalizationKeysList();
        }

        public static void OnGameBoot()
        {
            DebugConsole.ReimplementConsole();
            //AssetManager.DumpAddressablesContent();
            if (!MaterialsContainer.s_Intilized)
            {
                MaterialsContainer.PreloadMaterials();
                MaterialsContainer.s_Intilized = true;
            }
        }

        public override void OnGUI()
        {
            DebugGUI.Render();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            GearsSync.Update();
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();

            SleepHook.LateUpdate();
        }

        public override void OnUpdate()
        {
            SetAppBackgroundMode();
            if (Client != null && Client.m_Instance != null)
            {
                Client.m_Instance.PollEvents();

                if(Client.m_IsReady)
                {
                    PlayersManager.UpdateLocalPlayer();
                }
            }

            if(Server != null && Server.m_IsReady)
            {
                Server.Update();
            }

            if (!InputManager.m_InteractedWithItemThisFrame && InputManager.GetFirePressed(InputManager.m_CurrentContext))
            {
                if (GameManager.m_NewPlayerAnimation)
                {
                    
                    if (GameManager.m_PlayerManager)
                    {
                        IInteraction Inter = GameManager.m_PlayerManager.ActiveInteraction;

                        bool CanHit = true;

                        if(Inter == null)
                        {
                            CanHit = true;
                        }
                        else
                        {
                            GameObject Obj = Inter.GetInteractiveObject();
                            if (Obj)
                            {
                                Comps.NetworkPlayer Player = Obj.GetComponent<Comps.NetworkPlayer>();
                                if (Player)
                                {
                                    CanHit = true;
                                }
                                else
                                {
                                    CanHit = false;
                                }
                            }
                        }

                        if (CanHit && GameManager.m_NewPlayerAnimation.CanTransitionToState(PlayerAnimation.State.Throwing))
                        {
                            MeleeManager.TryToAttack();
                        }
                    }
                }
            }
            if (InputManager.GetAltFirePressed(InputManager.m_CurrentContext))
            {
                if (GameManager.m_PlayerManager)
                {
                    IInteraction Inter = GameManager.m_PlayerManager.ActiveInteraction;

                    if (Inter != null)
                    {
                        GameObject Obj = Inter.GetInteractiveObject();
                        if (Obj)
                        {
                            Comps.DroppedGearVisual Gear = Obj.GetComponent<Comps.DroppedGearVisual>();
                            if (Gear)
                            {
                                if(Gear.m_CookingVisual == null || Gear.m_CookingVisual.m_CookingSlot == null)
                                {
                                    GameManager.GetPlayerManagerComponent().InteractiveObjectsProcessAltFire();
                                }
                            }
                        }
                    }
                }
            }

            MeleeManager.Update();

            if (CanvasUI.s_SpeakingIndicator)
            {
                ClientVoice.IsSpeaking();
            }

            CanvasUI.Update();

            //MeleeManager.FishTalkRollChane();

            if (InputManager.GetReloadPressed(InputManager.m_CurrentContext))
            {
                MeleeManager.OnFishStartTalking();
            }

            if (IsGameplayScene() && !GameManager.s_IsGameplaySuspended)
            {
                PlayersManager.SpectatorControls();
            }

            WeatherHook.Update();

            GearSpawnsRipper.Update();
        }

        public static string GetCurrentSceneName()
        {
            if (GameManager.m_SceneTransitionData != null)
            {
                if (string.IsNullOrEmpty(GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent))
                {
                    return "Empty";
                }
                return GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;
            }

            return "Empty";
        }

        public static bool IsGameplayScene(string Scene = "")
        {
            if(Scene == "")
            {
                Scene = GetCurrentSceneName();
            }
            if(Scene == "Empty" || Scene == "Boot" || Scene.StartsWith("MainMenu"))
            {
                return false;
            }
            return true;
        }

        public static void ChangeMap()
        {
            DataStr.ServerConfig CFG = Client.m_Config;
            EmptyScene.s_SceneLoadFromEmpty = Client.m_Config.m_SceneToSpawn;
            Minimalizer.s_SceneSpawnOverride = Client.m_Config.m_SceneToSpawn;
            SceneManager.LoadEmptyScene();
        }

        public static void ChangeMap(string Scene)
        {
            EmptyScene.s_SceneLoadFromEmpty = Scene;
            Minimalizer.s_SceneSpawnOverride = Scene;
            SceneManager.LoadEmptyScene();
        }

        public static void SetupSurvivalSettings(string ExperienceMode, int Seed, string Region = "", string SceneToSpawn = "")
        {
            ExperienceModeManager EMM = GameManager.GetExperienceModeManagerComponent();
            GameModeConfig SelectedMode = null;
            RegionSpecification SelectedRegion = null;

            Il2CppSystem.Collections.Generic.IList<GameModeConfig> GameMods = EMM.GetAvailableGameModes();
            for (int i = 0; GameMods[i] != null; i++)
            {
                if (ExperienceMode == GameMods[i].name)
                {
                    SelectedMode = GameMods[i];
                    break;
                }
            }
            EMM.SetGameModeConfig(SelectedMode);
            MenuHook.s_LastMultiplayerWorldSeed = Seed;
            MenuHook.s_LastMultiplayerGameMode = SelectedMode;
            SkyCoop.Logger.Log($"SelectedMode {SelectedMode.name}");

            if (!string.IsNullOrEmpty(Region) && !string.IsNullOrEmpty(SceneToSpawn))
            {
                Panel_SelectRegion_Map Panel_Regions = null;

                if (InterfaceManager.TryGetPanel<Panel_SelectRegion_Map>(out Panel_Regions))
                {
                    foreach (SelectRegionItem R in Panel_Regions.m_Items)
                    {
                        if (R.name == Region)
                        {
                            SelectedRegion = R.m_RegionSpec;
                            break;
                        }
                    }
                    GameManager.m_StartRegion = SelectedRegion;
                }
                Minimalizer.s_SceneSpawnOverride = SceneToSpawn;
                GameManager.m_Instance.LaunchSandbox();
                GameManager.m_SceneTransitionData.m_GameRandomSeed = Seed;
            }
            else
            {
                Panel_Sandbox Panel = InterfaceManager.GetPanel<Panel_Sandbox>();
                if (Panel)
                {
                    Panel.OnClickNew();
                }
                Panel_SelectExperience Panel2 = InterfaceManager.GetPanel<Panel_SelectExperience>();
                if (Panel2)
                {
                    Panel2.OnExperienceClicked();
                }
                InterfaceManager.TrySetPanelEnabled<Panel_MainMenu>(true);
                InterfaceManager.TrySetPanelEnabled<Panel_SelectExperience>(false);
                InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(false);
                GameManager.GetExperienceModeManagerComponent().SetGameModeConfig(SelectedMode);
            }
        }

        public static string GetNickName()
        {
#warning TODO: fix this to
            //string UserName = Settings.m_Options.m_UserName;
            string UserName = "";

            if (string.IsNullOrEmpty(UserName))
            {
                return SteamFriends.GetPersonaName();
            }

            return UserName;
        }

        public static string GenerateSeededGUID(int gameSeed, Vector3 v3)
        {
            int _x = (int)v3.x;
            int _y = (int)v3.y;
            int _z = (int)v3.z;
            int v3Int = _x + _y + _z;
            int newSeed = gameSeed + v3Int;
            string _chars = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            System.Random newRNG = new System.Random(newSeed);
            string newGUID = "";
            for (int i = 1; i < 36; i++)
            {
                if (i == 9 || i == 14 || i == 19 || i == 24)
                {
                    newGUID = newGUID + "-";
                }
                int charIndex = newRNG.Next(0, _chars.Length);
                newGUID = newGUID + _chars[charIndex];
            }
            return newGUID;
        }

        [HarmonyLib.HarmonyPatch(typeof(GameManager), "OnApplicationFocus")]
        public class GameManager_OnApplicationFocus
        {
            public static bool Prefix(GameManager __instance, bool focusStatus)
            {
                s_AppFocus = focusStatus;
                //SkyCoop.Logger.Log("OnApplicationFocus " + focusStatus);
                return false;
            }
        }
    }
}
