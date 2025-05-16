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
            //AssetManager.DumpLocalizationKeysList();
        }

        public static void OnGameBoot()
        {
            ReimplementConsole();
            //AssetManager.DumpAddressablesContent();
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
            if(InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.K))
            {
                SpawnPointEditor.ToggleSpawnPointEditor();
            }
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.L))
            {
                if(CanvasUI.m_SpawnPointEditor && CanvasUI.m_SpawnPointEditor.activeSelf)
                {
                    SpawnPointEditor.AddSpawnPoint();
                }
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

        public static void SetupSurvivalSettings(string GameMode, int Seed, string Region, string SceneToSpawn = "")
        {
            ExperienceModeManager EMM = GameManager.GetExperienceModeManagerComponent();
            GameModeConfig SelectedMode = null;
            RegionSpecification SelectedRegion = null;
            foreach (GameModeConfig Mode in EMM.m_AvailableGameModes)
            {
                if(GameMode == Mode.name)
                {
                    SelectedMode = Mode;
                    break;
                }
            }

            Panel_SelectRegion_Map Panel_Regions = InterfaceManager.GetPanel<Panel_SelectRegion_Map>();

            foreach (SelectRegionItem R in Panel_Regions.m_Items)
            {
                if(R.name == Region)
                {
                    SelectedRegion = R.m_RegionSpec;
                    break;
                }
            }

            EMM.SetGameModeConfig(SelectedMode);
            GameManager.m_SceneTransitionData.m_GameRandomSeed = Seed;
            GameManager.m_StartRegion = SelectedRegion;
            Minimalizer.s_SceneSpawnOverride = SceneToSpawn;
            GameManager.m_Instance.LaunchSandbox();
        }

        public static string GetNickName()
        {
            return SteamFriends.GetPersonaName();
        }

        [HarmonyLib.HarmonyPatch(typeof(GameManager), "OnApplicationFocus")]
        public class GameManager_OnApplicationFocus
        {
            public static bool Prefix(GameManager __instance, bool focusStatus)
            {
                s_AppFocus = focusStatus;
                SkyCoop.Logger.Log("OnApplicationFocus " + focusStatus);
                return false;
            }
        }
    }
}
