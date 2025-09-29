using Il2Cpp;
using Il2CppParadoxNotion.Services;
using Il2CppSteamworks;
using Il2CppTLD.Gameplay;
using Il2CppTLD.SaveState;
using Il2CppTLD.Scenes;
using MelonLoader;
using SkyCoopClient;
using SkyCoopServer;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;
using AssetsTools.NET.Extra;

namespace SkyCoop
{
    internal sealed class ModMain : MelonMod
    {
        public static Server Server;
        public static Client Client;
        public static ClientVoice ClientVoice;

        public static bool s_AppFocus = true;

        public override void OnInitializeMelon()
        {
            Server = new Server();
            Client = new Client();
            DebugConsole.RegisterCommands();
            Settings.Init();
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
            if(MenuHook.s_CurrenetMenuOverride == "Multiplayer" || (Client != null && Client.m_IsReady))
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
            AssetManager.RegisterIlegalGearsCommand();
            WeaponsManager.InitDescriptors();
        }

        [Obsolete]
        public override void OnLevelWasInitialized(int level)
        {
            MeleeManager.ReintilizeViewModels();
            GameModeHUD.Reintilize();
            Minimalizer.ToggleStats();
            //AssetManager.DumpLocalizationKeysList();
        }

        public static void OnGameBoot()
        {
            ReimplementConsole();
            //AssetManager.DumpAddressablesContent();
            if (!MaterialsContainer.s_Intilized)
            {
                MaterialsContainer.PreloadMaterials();
                MaterialsContainer.s_Intilized = true;
            }
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

                if(SkyCoopServer.Logger.Logsbuffer.Count > 0)
                {
                    SkyCoopServer.Logger.LogData log = SkyCoopServer.Logger.Logsbuffer[0];
                    SkyCoopServer.Logger.Logsbuffer.Remove(log);
                    Logger.Log(log.m_Color, log.m_Message);
                }
            }

            if (InputManager.GetFirePressed(InputManager.m_CurrentContext))
            {
                if (GameManager.m_NewPlayerAnimation)
                {
                    if (GameManager.m_NewPlayerAnimation.CanTransitionToState(PlayerAnimation.State.Throwing))
                    {
                      MeleeManager.TryToAttack();
                    }
                }
            }
            if(InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.F6))
            {
                SpawnPointEditor.ToggleSpawnPointEditor();
            }
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.F9))
            {
                PropsSpawnsEditor.TogglePropsEditor();
            }
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.P))
            {
                if (CanvasUI.m_SpawnPointEditor && CanvasUI.m_SpawnPointEditor.activeSelf)
                {
                    SpawnPointEditor.AddSpawnPoint();
                }
                if (CanvasUI.m_PropsEditor && CanvasUI.m_PropsEditor.activeSelf)
                {
                    PropsSpawnsEditor.AddProp();
                }
            }

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
        }

        public static void ReimplementConsole()
        {
            if (uConsole.m_Instance == null)
            {
                Logger.Log("No uConsole present, creating one.");
                GameObject ConsoleReference = Addressables.LoadAssetAsync<GameObject>("uConsole").WaitForCompletion();
                if (ConsoleReference != null)
                {
                    GameObject ConsoleObj = UnityEngine.Object.Instantiate(ConsoleReference);
                    if (ConsoleObj)
                    {
                        uConsole.m_Instance = ConsoleObj.GetComponent<uConsole>();
                    } else
                    {
                        Logger.Log(System.ConsoleColor.Red, "Can't assign uConsole!");
                    }
                } else
                {
                    Logger.Log(System.ConsoleColor.Red, "Can't load uConsole!");
                }
            }
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
            if(Scene == "Empty" || Scene == "Boot" || Scene == "MainMenu")
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

        public static void SetupSurvivalSettings(string ExperienceMode, int Seed, string Region, string SceneToSpawn = "")
        {
            ExperienceModeManager EMM = GameManager.GetExperienceModeManagerComponent();
            GameModeConfig SelectedMode = null;
            RegionSpecification SelectedRegion = null;
            foreach (GameModeConfig Mode in EMM.m_AvailableGameModes)
            {
                if(ExperienceMode == Mode.name)
                {
                    SelectedMode = Mode;
                    break;
                }
            }

            Panel_SelectRegion_Map Panel_Regions = null;

            if(InterfaceManager.TryGetPanel<Panel_SelectRegion_Map>(out Panel_Regions))
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

            EMM.SetGameModeConfig(SelectedMode);
            Minimalizer.s_SceneSpawnOverride = SceneToSpawn;
            GameManager.m_Instance.LaunchSandbox();
            GameManager.m_SceneTransitionData.m_GameRandomSeed = Seed;
            Panel_Loading Panel = null;
            if(InterfaceManager.TryGetPanel<Panel_Loading>(out Panel))
            {
                Panel.m_ShowQuoteAfterLoad = false;
            }
        }

        public static string GetNickName()
        {
            string UserName = Settings.m_Options.m_UserName;

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
