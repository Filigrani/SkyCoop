using System;
using UnityEngine;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using MelonLoader;
using Harmony;
using UnhollowerRuntimeLib;
using UnhollowerBaseLib;
using GameServer;
using MelonLoader.TinyJSON;
using IL2CPP = Il2CppSystem.Collections.Generic;
using static SkyCoop.Comps;
using static SkyCoop.DataStr;
using static ParadoxNotion.Services.Logger;

namespace SkyCoop
{
    public class MyMod : MelonMod
    {

        public static class BuildInfo
        {
            public const string Name = "Sky Co-op LTS";
            public const string Description = "Multiplayer mod";
            public const string Author = "Filigrani";
            public const string Company = null;
            public const string Version = "0.11.3";
            public const string DownloadLink = null;
            public const int RandomGenVersion = 5;
        }
        public static int LastLoadedGenVersion = 0;
        public static bool CantBeUsedForMP = false;
        public static int SelectedSaveSeed = 0;
        public static string SelectedSaveName = "";
        public static int LobbyStartingRegion = -1;
        public static int LobbyStartingExperience = -1;
        public static bool HostFromLobbyAfterLoader = false;
        public static int LobbyVoteLeft = 0;
        public static string LobbyState = "Idle";
        public static bool StartServerAfterSelectSave = false;
        public static int PortsToHostOn = 26950;
        public static bool LastSafeStatus = false;
        public static bool OnExpedition = false;
        public static string ExpeditionLastTaskText = "";
        public static string ExpeditionLastName = "";
        public static int ExpeditionLastTime = 0;

        //VARS
        #region VARS
        public static bool isRuning = false;
        public static bool sendMyPosition = false;
        public static bool iAmHost = false;
        public static bool IamShatalker = false;
        public static string MyUGUID = "";
        public static GameObject playerbody = null;
        public static List<GameObject> players = new List<GameObject>();
        public static List<DataStr.MultiPlayerClientData> playersData = new List<DataStr.MultiPlayerClientData>();
        public static GameObject MyPlayerDoll = null;
        public static GameObject DollCameraDummy = null;
        public static bool IsDead = false;
        public static int levelid = 0;
        public static string level_name = "";
        public static string level_guid = "";
        public static string previous_level_guid = "";
        public static bool NeedSyncTime = false;
        public static bool LightSource = false;
        public static bool PreviousLightSource = false;
        public static bool MyLightSource = false;
        public static string MyLightSourceName = "";
        public static bool MyLastLightSource = false;
        public static string MyLastLightSourceName = "";
        public static string LastSelectedGearName = "";
        public static GearItem LastSelectedGear = null;
        public static bool NeedRefreshInv = false;
        public static string MyAnimState = "Idle";
        public static DataStr.MultiplayerEmote MyEmote = null;
        public static string MyPreviousAnimState = "Idle";
        public static bool MyHasRifle = false;
        public static bool HasRevolver = false;
        public static bool MyHasRevolver = false;
        public static bool MyHasAxe = false;
        public static bool MyHasMedkit = false;
        public static bool MyIsAiming = false;
        public static int CycleSkip = 0;
        public static int MyCycleSkip = 0;
        public static bool IsSleeping = false;
        public static bool PreviousSleeping = false;
        public static bool IsCycleSkiping = false;
        public static int MyArrows = 0;
        public static int MyFlares = 0;
        public static int MyBlueFlares = 0;
        public static int StepState = 0;
        public static bool AnimalsController = true;
        public static int MyTicksOnScene = 0;
        public static int DeltaAnimalsMultiplayer = 4;
        public static string DebugAnimalGUID = "";
        public static string DebugAnimalGUIDLast = "";
        public static GameObject DebugLastAnimal = null;
        public static string HarvestingAnimal = "";
        public static string PreviousHarvestingAnimal = "";
        public static bool ALWAYS_FUCKING_CURSOR_ON = false;
        public static GameObject PistolBulletPrefab = null;
        public static GameObject RevolverBulletPrefab = null;
        public static bool NeedTryReconnect = false;
        public static bool TryingReconnect = false;
        public static int NoHostResponceSeconds = 0;
        public static GearItem SaveThrowingItem = null;
        public static int OverridedHourse = 12;
        public static int OverridedMinutes = 0;
        public static string OveridedTime = "12:0";
        public static int PlayedHoursInOnline = 0;
        public static bool ConnectedSteamWorks = false;
        public static string SteamServerWorks = "";
        public static string SteamLobbyToJoin = "";
        public static AssetBundle LoadedBundle = null;
        public static int MaxPlayers = 1;
        public static int GiveItemTo = -1;
        public static string MyHat = "";
        public static string MyTop = "";
        public static string MyBottom = "";
        public static string MyBoots = "";
        public static string MyScarf = "";
        public static string MyBalaclava = "";
        public static List<DataStr.PickedGearSync> RecentlyPickedGears = new List<DataStr.PickedGearSync>();
        public static List<DataStr.ClimbingRopeSync> DeployedRopes = new List<DataStr.ClimbingRopeSync>();
        public static List<DataStr.DeathContainerData> DeathCreates = new List<DataStr.DeathContainerData>();
        public static List<DataStr.ShowShelterByOther> ShowSheltersBuilded = new List<DataStr.ShowShelterByOther>();
        public static bool IsDrinking = false;
        public static bool PreviousIsDrinking = false;
        public static bool IsEating = false;
        public static bool PreviousIsEating = false;
        public static DataStr.ServerConfigData ServerConfig = new DataStr.ServerConfigData();
        public static bool NowHeavyBreath = false;
        public static bool PreviousNowHeavyBreath = false;
        public static int BloodLosts = 0;
        public static int PreviousBloodLosts = 0;
        public static PlayerControlMode PreviousControlModeBeforeAction = PlayerControlMode.Normal;
        public static Comps.MultiplayerPlayer PlayerInteractionWith = null;
        public static GearItem EmergencyStimBeforeUse = null;
        public static bool HasInfecitonRisk = false;
        public static bool PreviousHasInfectionRisk = false;
        public static DataStr.ContainerOpenSync MyContainer = null;
        public static int UpdatePickedPlants = -1;
        public static int NeedConnectAfterLoad = -1;
        public static int NeedLoadSaveAfterLoad = -1;
        public static int UpdateSnowshelters = -1;
        public static int UpdateRopesAndFurns = -1;
        public static int UpdateCampfires = -1;
        public static int UpdateEverything = -1;
        public static int RemoveAttachedGears = -1;
        public static int StartDSAfterLoad = -1;
        public static int SendAfterLoadingFinished = -1;
        public static int TryMakeLobbyAgain = -1;
        public static int RegularUpdateSeconds = 7;
        public static int MinutesFromStartServer = 0;
        public static int TimeOutSeconds = 300;
        public static int TimeOutSecondsForLoaders = 300;
        public static int TimeToWorryAboutLastRequest = 10;
        public static bool SkipEverythingForConnect = false;
        public static int PlayersOnServer = 0;
        public static GameObject UIHostMenu = null;
        public static GameObject MicrophoneIdicator = null;
        public static GameObject RadioIdicator = null;
        public static GameObject LobbyUI = null;
        public static GameObject LobbyRegion = null;
        public static GameObject LobbyExperience = null;
        public static GameObject LobbyDeitails = null;
        public static GameObject ServerBrowser = null;
        public static UtilsPanelChoose.DetailsObjets LobbyDeitailsStrct = null;
        public static GameObject LobbyNewGame = null;
        public static GameObject EmoteWheel = null;
        public static GameObject NewFlairNotification = null;
        public static GameObject CustomizeUi = null;
        public static UnityEngine.UI.Text CustomizeUiTabLable = null;
        public static Transform CustomizeUiScrollContentRoot = null;
        public static int TargetFlairSlot = 1;
        public static string NotificationString = "";
        public static GameObject ExpeditionEditorUI = null;
        public static GameObject ExpeditionEditorSelectUI = null;

        public static string CustomServerName = "";
        public static string MyLobby = "";
        public static bool ApplyOtherCampfires = false;
        public static Dictionary<long, string> SlicedJsonDataBuffer = new Dictionary<long, string>();
        public static Dictionary<int, List<byte>> SlicedBytesDataBuffer = new Dictionary<int, List<byte>>();
        public static bool DebugTrafficCheck = false;
        public static Dictionary<string, bool> OpenableThings = new Dictionary<string, bool>();
        public static Dictionary<int, GameObject> DroppedGearsObjs = new Dictionary<int, GameObject>();
        public static Dictionary<int, GameObject> TrackableDroppedGearsObjs = new Dictionary<int, GameObject>();
        public static int OverrideLampReduceFuel = -1;
        public static Dictionary<string, float> BooksResearched = new Dictionary<string, float>();
        public static Dictionary<string, LoadScene> DoorsObjs = new Dictionary<string, LoadScene>();
        public static bool DelayedGearsPickup = false;
        public static List<DataStr.PickedGearSync> PickedGearsBackup = new List<DataStr.PickedGearSync>();
        public static List<DataStr.BrokenFurnitureSync> BrokenFurnsBackup = new List<DataStr.BrokenFurnitureSync>();
        public static Vector3 LastPickedVisualPosition = new Vector3(0, 0, 0);
        public static Quaternion LastPickedVisualRotation = new Quaternion(0, 0, 0, 0);

        //Voice chat
        public static bool DoingRecord = false;
        //Other mods support
        public static GameObject ModdedHandsBook = null;
        //Misc
        public static GameObject ViewModelRadio = null;
        public static GameObject ViewModelRadioNeedle = null;
        public static GameObject ViewModelRadioLED = null;
        public static GameObject MyRadioAudio = null;
        public static GameObject ViewModelHatchet = null;
        public static GameObject ViewModelHatchet2 = null;//mesh_Hatchet_improvised
        public static GameObject ViewModelKnife = null; //mesh_knife
        public static GameObject ViewModelKnife2 = null; //mesh_knife_improvised
        public static GameObject ViewModelJeremiahKnife = null;
        public static GameObject ViewModelPrybar = null; //mesh_prybar
        public static GameObject ViewModelHammer = null; //mesh_hammer
        public static GameObject ViewModelShovel = null;
        public static GameObject ViewModelBolt = null;
        public static GameObject ViewModelStone = null;
        public static GameObject ViewModelFireAxe = null;
        public static GameObject ViewModelHackSaw = null;
        public static GameObject ViewModelPhoto = null;
        public static GameObject ViewModelMap = null;
        public static GameObject ViewModelNote = null;
        public static bool UseBoltInsteadOfStone = false;
        public static bool OriginalRadioSeaker = false;
        public static bool VanilaRadio = false;
        public static GameObject RefMan = null;
        public static bool SeenRefMan = false;


        public static GameObject ViewModelDummy = null;
        public static float RadioFrequency = 0;
        public static float PreviousRadioFrequency = 0;
        #endregion

        public static Vector3 LastSleepV3 = new Vector3(0, 0, 0);
        public static Quaternion LastSleepQuat = new Quaternion(0, 0, 0, 0);
        public static bool AtBed = false;
        public static Vector3 OutOfBedPosition = new Vector3(0, 0, 0);
        public static Dictionary<string, int> GearIDList = new Dictionary<string, int>();
        public static bool GearIDListReady = false;
        //public static bool AntiCheat = false;
        public static bool InterloperHook = false;
        public static string OverridedSceneForSpawn = "";
        public static Vector3 OverridedPositionForSpawn = Vector3.zero;
        public static string SavedSceneForSpawn = "";
        public static Vector3 SavedPositionForSpawn = Vector3.zero;
        public static bool FixedPlaceLoaded = false;
        public static float InteractTimer = 0.0f;
        public static GameObject ObjectInteractWith = null;
        public static bool InteractionInprocess = false;
        public static string InteractionType = "";
        public static bool InteractHold = false;


        public static string AutoStartSlot = "";
        public static List<string> AutoCMDs = new List<string>();
        public static bool CrazyPatchesLogger = false;
        public static bool KillOnUpdate = false;
        public static bool KillEverySecond = false;
        public static bool KillEveryInGameMinute = false;
        public static bool NoAnimalSync = false;
        public static string RCON = "12345";
        public static bool SwearOnLockpickingDone = false;
        public static bool DedicatedServerAppMode = false;


        // Other stuff
        public static List<BoardGameSession> BoardGamesSessions = new List<BoardGameSession>();

        public static void KillConsole()
        {
            if (ServerConfig.m_CheatsMode == 2 || (iAmHost == true && ServerConfig.m_CheatsMode == 1) || InOnline() == false || DedicatedServerAppMode)
            {
                return;
            }

            if (uConsole.m_Instance != null)
            {
                uConsole.m_Instance.m_Activate = KeyCode.None;
            }
        }


        public static void AddSlicedJsonData(DataStr.SlicedJsonData jData)
        {
            //MelonLogger.Msg(ConsoleColor.Yellow, "Got Slice for hash:"+jData.m_Hash+" DATA: "+jData.m_Str);
            if (SlicedJsonDataBuffer.ContainsKey(jData.m_Hash))
            {
                string previousString = "";
                if (SlicedJsonDataBuffer.TryGetValue(jData.m_Hash, out previousString) == true)
                {
                    string wholeString = previousString + jData.m_Str;
                    SlicedJsonDataBuffer.Remove(jData.m_Hash);
                    SlicedJsonDataBuffer.Add(jData.m_Hash, wholeString);
                } else {
                    SlicedJsonDataBuffer.Add(jData.m_Hash, jData.m_Str);
                }
            } else {
                SlicedJsonDataBuffer.Add(jData.m_Hash, jData.m_Str);
            }

            if (jData.m_Last)
            {
                string finalJsonData = "";
                if (SlicedJsonDataBuffer.TryGetValue(jData.m_Hash, out finalJsonData) == true)
                {
                    SlicedJsonDataBuffer.Remove(jData.m_Hash);
                    DataStr.GearItemDataPacket gData = new DataStr.GearItemDataPacket();
                    gData.m_GearName = jData.m_GearName;
                    gData.m_DataProxy = finalJsonData;
                    GiveRecivedItem(gData);
                }
            }
        }

        public static void PatchBookReadTime(GearItem __instance)
        {
            if (__instance.m_ResearchItem == null)
            {
                return;
            }

            if (__instance.m_ObjectGuid == null)
            {
                __instance.m_ObjectGuid = __instance.gameObject.AddComponent<ObjectGuid>();
                __instance.m_ObjectGuid.Generate();
                //MelonLogger.Msg("Added GUID for Book " + __instance.m_GearName + " GUID " + __instance.m_ObjectGuid.Get());
            } else {
                string Key = __instance.m_ObjectGuid.Get();
                //MelonLogger.Msg("There Book " + __instance.m_GearName + " GUID " + Key);

                if (BooksResearched.ContainsKey(Key))
                {
                    float GotProg;

                    if (BooksResearched.TryGetValue(Key, out GotProg))
                    {
                        __instance.m_ResearchItem.m_ElapsedHours = GotProg;
                        //MelonLogger.Msg("Have information about it Progress " + __instance.m_ResearchItem.m_ElapsedHours + "/" + __instance.m_ResearchItem.m_TimeRequirementHours);
                    }
                } else {
                    __instance.m_ResearchItem.m_ElapsedHours = 0;
                }

                if (__instance.m_ResearchItem.IsResearchComplete())
                {
                    __instance.m_ResearchItem.UpdateItemType(GearTypeEnum.Firestarting);
                } else {
                    __instance.m_ResearchItem.UpdateItemType(GearTypeEnum.Tool);
                }
            }
        }

        public static void AddSlicedJsonDataForPicker(DataStr.SlicedJsonData jData, bool place)
        {
            //MelonLogger.Msg(ConsoleColor.Yellow, "Got Requested Item Slice for hash:" + jData.m_Hash + " DATA: " + jData.m_Str);
            if (SlicedJsonDataBuffer.ContainsKey(jData.m_Hash))
            {
                string previousString = "";
                if (SlicedJsonDataBuffer.TryGetValue(jData.m_Hash, out previousString) == true)
                {
                    string wholeString = previousString + jData.m_Str;
                    SlicedJsonDataBuffer.Remove(jData.m_Hash);
                    SlicedJsonDataBuffer.Add(jData.m_Hash, wholeString);
                } else {
                    SlicedJsonDataBuffer.Add(jData.m_Hash, jData.m_Str);
                }
            } else {
                SlicedJsonDataBuffer.Add(jData.m_Hash, jData.m_Str);
            }

            if (jData.m_Last)
            {
                string finalJsonData = "";
                DiscardRepeatPacket();
                RemovePleaseWait();
                if (SlicedJsonDataBuffer.TryGetValue(jData.m_Hash, out finalJsonData) == true)
                {
                    SlicedJsonDataBuffer.Remove(jData.m_Hash);
                    MelonLogger.Msg(ConsoleColor.Green, "Finished getting data for:" + jData.m_Hash);

                    string gearName = jData.m_Extra.m_GearName;
                    MelonLogger.Msg(ConsoleColor.Cyan, "Going to get " + jData.m_Extra.m_GearName);

                    GameObject reference = GetGearItemObject(gearName);

                    if (reference == null)
                    {
                        MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
                        return;
                    }


                    GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(reference, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                    DisableObjectForXPMode.RemoveDisabler(newGear.gameObject);
                    if (newGear == null)
                    {
                        MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
                        return;
                    }

                    newGear.name = CloneTrimer(newGear.name);

                    DataStr.ExtraDataForDroppedGear Extra = jData.m_Extra;

                    if (newGear.GetComponent<KeroseneLampItem>() != null && Extra != null)
                    {
                        int minutesDroped = MinutesFromStartServer - Extra.m_DroppedTime;
                        OverrideLampReduceFuel = minutesDroped;
                        MelonLogger.Msg(ConsoleColor.Cyan, "Lamp been dropped " + minutesDroped + " minutes");
                    }
                    bool UseLastVisualDropPos = false;
                    if (!string.IsNullOrEmpty(finalJsonData))
                    {
                        newGear.GetComponent<GearItem>().Deserialize(finalJsonData);
                    } else
                    {
                        UseLastVisualDropPos = true;
                    }
                    MelonLogger.Msg(ConsoleColor.Cyan, "UseLastVisualDropPos " + UseLastVisualDropPos);

                    if (!string.IsNullOrEmpty(jData.m_Extra.m_PhotoGUID))
                    {
                        GearItem gi = newGear.GetComponent<GearItem>();
                        if (gi.m_ObjectGuid == null)
                        {
                            string GUID = jData.m_Extra.m_PhotoGUID;
                            ObjectGuid GUIDComp = newGear.AddComponent<ObjectGuid>();
                            GUIDComp.m_Guid = GUID;
                            gi.m_ObjectGuid = GUIDComp;
                            MelonLogger.Msg("Going render photo " + GUID);
                            Texture2D tex = MPSaveManager.GetPhotoTexture(GUID, gi.m_GearName);
                            if (tex)
                            {
                                newGear.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = tex;
                            }
                        }
                    }
                    newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;

                    Comps.DropFakeOnLeave DFL = newGear.AddComponent<Comps.DropFakeOnLeave>();

                    if (!UseLastVisualDropPos)
                    {
                        DFL.m_OldPossition = newGear.gameObject.transform.position;
                        DFL.m_OldRotation = newGear.gameObject.transform.rotation;
                    } else
                    {
                        DFL.m_OldPossition = LastPickedVisualPosition;
                        DFL.m_OldRotation = LastPickedVisualRotation;
                    }


                    if (Extra != null)
                    {
                        if (Extra.m_GoalTime != 0 && Extra.m_GoalTime != -1)
                        {
                            GearItem gear = newGear.GetComponent<GearItem>();
                            if (gear.m_EvolveItem != null)
                            {
                                int minutesOnDry = MinutesFromStartServer - Extra.m_DroppedTime;

                                gear.m_EvolveItem.m_TimeSpentEvolvingGameHours = (float)minutesOnDry / 60;
                                MelonLogger.Msg(ConsoleColor.Blue, "Saving minutesOnDry " + minutesOnDry);
                                MelonLogger.Msg(ConsoleColor.Blue, "m_TimeSpentEvolvingGameHours " + gear.m_EvolveItem.m_TimeSpentEvolvingGameHours);
                            }
                        }
                        if (!string.IsNullOrEmpty(Extra.m_ExpeditionNote))
                        {
                            CreateCustomNote(Extra.m_ExpeditionNote, newGear.GetComponent<GearItem>());
                        }
                    }

                    if (place == false)
                    {
                        bool skipPickup = false;
                        if (newGear.GetComponent<GearItem>().m_Bed != null)
                        {
                            if (newGear.GetComponent<GearItem>().m_Bed.GetState() == BedRollState.Placed)
                            {
                                newGear.GetComponent<GearItem>().m_Bed.SetState(BedRollState.Rolled);
                                skipPickup = true;
                            }
                        }
                        if (skipPickup == false)
                        {
                            GameManager.GetPlayerManagerComponent().ProcessInspectablePickupItem(newGear.GetComponent<GearItem>());
                        } else {
                            GameManager.GetPlayerManagerComponent().ProcessPickupItemInteraction(newGear.GetComponent<GearItem>(), false, false);
                        }

                    } else {
                        newGear.GetComponent<GearItem>().PlayPickUpClip();
                        GameManager.GetPlayerManagerComponent().StartPlaceMesh(newGear, PlaceMeshFlags.None);
                    }

                    PatchBookReadTime(newGear.GetComponent<GearItem>());
                }
            }
        }

        private const int CHUNK_SIZE = 500;

        public static void ShowCFGData()
        {
            MelonLogger.Msg(ConsoleColor.Green, "[Server Config Data] Data updated");
            MelonLogger.Msg(ConsoleColor.Blue, "m_FastConsumption: " + ServerConfig.m_FastConsumption);
            MelonLogger.Msg(ConsoleColor.Blue, "m_DuppedSpawns: " + ServerConfig.m_DuppedSpawns);
            MelonLogger.Msg(ConsoleColor.Blue, "m_DuppedContainers: " + ServerConfig.m_DuppedContainers);
            MelonLogger.Msg(ConsoleColor.Blue, "m_PlayersSpawnType: " + ServerConfig.m_PlayersSpawnType);
            MelonLogger.Msg(ConsoleColor.Blue, "m_FireSync: " + ServerConfig.m_FireSync);
            MelonLogger.Msg(ConsoleColor.Blue, "m_CheatsMode: " + ServerConfig.m_CheatsMode);
            MelonLogger.Msg(ConsoleColor.Blue, "m_CheckModsValidation: " + ServerConfig.m_CheckModsValidation);
            MelonLogger.Msg(ConsoleColor.Blue, "m_SaveScamProtection: " + ServerConfig.m_SaveScamProtection);
            MelonLogger.Msg(ConsoleColor.Blue, "m_PVP: " + ServerConfig.m_PVP);
        }

        //STRUCTS
        #region STRUCTS

        public static DataStr.PriorityActionForOtherPlayer GetCustomAction(string ActionName)
        {
            DataStr.PriorityActionForOtherPlayer act = new DataStr.PriorityActionForOtherPlayer();


            if (ActionName == "Lockpick")
            {
                act.m_Action = "Lockpick";
                act.m_DisplayText = act.m_Action;
                act.m_ProcessText = "Lockpicking...";
                act.m_CancleOnMove = false;
                act.m_ActionDuration = 15;
                act.m_Hold = false;
                act.m_EscCancle = true;
            }
            else if (ActionName == "Excision")
            {
                act.m_Action = "Excision";
                act.m_DisplayText = act.m_Action;
                act.m_ProcessText = "Excision...";
                act.m_CancleOnMove = false;
                act.m_ActionDuration = 10;
            }
            else if (ActionName == "Locksmith0")
            {
                act.m_Action = "Locksmith0";
                act.m_DisplayText = "Sawing";
                act.m_ProcessText = "Sawing...";
                act.m_CancleOnMove = false;
                act.m_ActionDuration = 10;
                act.m_Hold = false;
            }
            else if (ActionName == "Locksmith1")
            {
                act.m_Action = "Locksmith1";
                act.m_DisplayText = "Grinding";
                act.m_ProcessText = "Grinding...";
                act.m_CancleOnMove = false;
                act.m_ActionDuration = 10;
                act.m_Hold = false;
            }

            return act;
        }

        public static DataStr.PriorityActionForOtherPlayer GetActionForOtherPlayer(string ActName)
        {
            DataStr.PriorityActionForOtherPlayer act = new DataStr.PriorityActionForOtherPlayer();
            if (ActName == "Revive")
            {
                act.m_Action = "Revive";
                act.m_DisplayText = act.m_Action;
                act.m_ProcessText = "Reviving...";
                act.m_CancleOnMove = false;
                act.m_ActionDuration = 5;
            }
            else if (ActName == "Stim")
            {
                act.m_Action = "Stim";
                act.m_DisplayText = "Inject";
                act.m_ProcessText = "Injecting...";
                act.m_CancleOnMove = false;
                act.m_ActionDuration = 1;
            } else if (ActName == "Invite")
            {
                act.m_Action = "Invite";
                act.m_DisplayText = "Invite to expedition";
                act.m_ProcessText = "Inviting...";
                act.m_CancleOnMove = false;
                act.m_ActionDuration = 1;
            } else if (ActName == "Lit")
            {
                act.m_Action = "Lit";
                act.m_DisplayText = "Ignite from player's fire";
                act.m_ProcessText = "";
                act.m_CancleOnMove = false;
                act.m_ActionDuration = 0;
            }
            else if (ActName == "Examine")
            {
                act.m_Action = "Examine";
                act.m_DisplayText = "Examine";
                act.m_ProcessText = "Diagnosing...";
                act.m_CancleOnMove = true;
                act.m_ActionDuration = 1;
            } else if (ActName == "Borrow")
            {
                act.m_Action = "Borrow";
                act.m_DisplayText = "Borrow";
                act.m_ProcessText = "Borrowing...";
                act.m_CancleOnMove = true;
                act.m_ActionDuration = 1;
            }

            return act;
        }
        #endregion

        public static void LoadChatName(string _name = "")
        {
            string NameFromFile = MPSaveManager.LoadMyName();

            if (ValidNickName(NameFromFile))
            {
                MyChatName = NameFromFile;
            } else {
                if (ValidNickName(_name))
                {
                    MyChatName = _name;
                } else {
                    MyChatName = "Player";
                }
            }
            MelonLogger.Msg("Your chat name is " + MyChatName);
        }

        public static void ForceLoadSlotByName(string searchname)
        {
            SaveSlotInfo SaveToLoad = null;
            Il2CppSystem.Collections.Generic.List<SaveSlotInfo> Slots = SaveGameSystem.GetSortedSaveSlots(Episode.One, SaveSlotType.SANDBOX);
            for (int i = 0; i < Slots.Count; i++)
            {
                SaveSlotInfo Slot = Slots[i];

                if (Slot.m_SaveSlotName == searchname)
                {
                    SaveToLoad = Slot;
                    break;
                }
            }
            if (SaveToLoad != null)
            {
                SaveGameSystem.SetCurrentSaveInfo(SaveToLoad.m_Episode, SaveToLoad.m_GameMode, SaveToLoad.m_GameId, SaveToLoad.m_SaveSlotName);
                GameManager.LoadSaveGameSlot(SaveToLoad.m_SaveSlotName, SaveToLoad.m_SaveChangelistVersion);
            }
        }

        public static void ForceLoadSlotForDs(string searchname, int Region, int Expereince, int Seed)
        {
            SaveGameSlots.LoadAllSavedGameFiles();
            bool HaveSaveFile = false;
            SaveSlotInfo SaveToLoad = null;

            Il2CppSystem.Collections.Generic.List<SaveSlotInfo> Slots = SaveGameSystem.GetSortedSaveSlots(Episode.One, SaveSlotType.SANDBOX);
            for (int i = 0; i < Slots.Count; i++)
            {
                SaveSlotInfo Slot = Slots[i];

                if (Slot.m_UserDefinedName == searchname)
                {
                    HaveSaveFile = true;
                    SaveToLoad = Slot;
                    break;
                }
            }
            if (HaveSaveFile == false)
            {
                MelonLogger.Msg(ConsoleColor.Yellow, "[Dedicated server] No save file with name " + searchname + " found!");
                MelonLogger.Msg(ConsoleColor.Yellow, "[Dedicated server] Found " + SaveGameSlots.m_SaveSlots.Count + " saves");
                ForceCreateSlotForPlayingSilently(Region, Expereince, Seed, searchname);
            }
            else{
                SaveGameSlots.SetBaseNameForSave(SaveToLoad.m_SaveSlotName, SaveToLoad.m_SaveSlotName);
                MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Save found!");
                MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Save slot base name is " + SaveGameSlots.GetBaseNameForSave(SaveToLoad.m_SaveSlotName));
                MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Save slot name " + SaveToLoad.m_SaveSlotName);
                MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Save slot user defined name " + SaveGameSlots.GetUserDefinedSlotName(SaveToLoad.m_SaveSlotName));
                SaveGameSystem.SetCurrentSaveInfo(SaveToLoad.m_Episode, SaveToLoad.m_GameMode, SaveToLoad.m_GameId, SaveToLoad.m_SaveSlotName);
                MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Selecting slot " + SaveGameSystem.GetCurrentSaveName());
                MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Save slot hash " + SaveGameSlots.LoadDataFromSlot(SaveToLoad.m_SaveSlotName, "global").GetHashCode());
                GameManager.LoadSaveGameSlot(SaveToLoad.m_SaveSlotName, SaveToLoad.m_SaveChangelistVersion);
            }
        }

        public static void ForceCreateSlotForPlayingSilently(int Reg, int Expereince, int Seed, string SlotName)
        {
            ExperienceModeType ExpType = (ExperienceModeType)Expereince;
            GameRegion Region = (GameRegion)Reg;

            if(Seed == 0)
            {
                MelonLogger.Msg(ConsoleColor.Green, "[Dedicated server] Creating save slot " + SlotName + " with random seed");
            }else{
                MelonLogger.Msg(ConsoleColor.Green, "[Dedicated server] Creating save slot " + SlotName + " with seed " + Seed);
            }

            SaveGameSystem.SetCurrentSaveInfo(Episode.One, SaveSlotType.SANDBOX, SaveGameSlots.GetUnusedGameId(), null);
            GameManager.GetExperienceModeManagerComponent().SetExperienceModeType(ExpType);
            GameManager.m_StartRegion = Region;
            SaveGameSlots.SetSlotDisplayName(SaveGameSystem.GetCurrentSaveName(), SlotName);

            GameManager.Instance().LaunchSandbox();
            if (Seed != 0)
            {
                GameManager.m_SceneTransitionData.m_GameRandomSeed = Seed;
            }
        }

        public static void ForceCreateSlotForPlaying(int Region, int Experience)
        {
            m_Panel_Sandbox.Enable(false);
            DataStr.SaveSlotSync SaveFile = new DataStr.SaveSlotSync();
            SaveFile.m_Episode = (int)Episode.One;
            SaveFile.m_SaveSlotType = (int)SaveSlotType.SANDBOX;
            SaveFile.m_Seed = 0;
            SaveFile.m_ExperienceMode = Experience;
            SaveFile.m_Location = Region;
            SaveFile.m_FixedSpawnScene = "";
            SaveFile.m_FixedSpawnPosition = Vector3.zero;
            PendingSave = SaveFile;
            ShouldCreateSaveForHost = true;
            HostFromLobbyAfterLoader = true;
            GameManager.GetExperienceModeManagerComponent().SetExperienceModeType((ExperienceModeType)Experience);
            LetChooseSpawnForClient(PendingSave);
        }

        public static void ForceLoadSlotForPlaying(string searchname)
        {
            bool HaveSaveFile = false;
            SaveSlotInfo SaveToLoad = null;

            Il2CppSystem.Collections.Generic.List<SaveSlotInfo> Slots = SaveGameSystem.GetSortedSaveSlots(Episode.One, SaveSlotType.SANDBOX);

            for (int i = 0; i < Slots.Count; i++)
            {
                SaveSlotInfo Slot = Slots[i];

                if (Slot.m_UserDefinedName == searchname)
                {
                    HaveSaveFile = true;
                    SaveToLoad = Slot;
                    break;
                }
            }
            if (HaveSaveFile == false)
            {
                MelonLogger.Msg(ConsoleColor.Red, "[-slot] No save file with name " + searchname + " found!");
            } else {
                MelonLogger.Msg(ConsoleColor.Magenta, "[-slot] Save found! Loading...");
                SaveGameSlots.SetBaseNameForSave(SaveToLoad.m_SaveSlotName, SaveToLoad.m_SaveSlotName);
                SaveGameSystem.SetCurrentSaveInfo(SaveToLoad.m_Episode, SaveToLoad.m_GameMode, SaveToLoad.m_GameId, SaveToLoad.m_SaveSlotName);
                GameManager.LoadSaveGameSlot(SaveToLoad.m_SaveSlotName, SaveToLoad.m_SaveChangelistVersion);
            }
        }

        public static void FinishStartingDsServer()
        {
            SteamConnect.Main.SetLobbyName(CustomServerName);
            Server.StartSteam(MaxPlayers);
            MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Server is ready! Have fun!");
        }

        public static bool SetP2PToLobbyForDSAfterLoad = false;
        public static bool HostUDDSAfterLoad = false;
        public static int PortForDSLoad = 0;
        public static int DsSavePerioud = 60;
        public static int RestartPerioud = 10800;

        public static void StartDedicatedServer(bool ds = true)
        {
            string ServStr = "[Dedicated server]";
            if (ds == false)
            {
                ServStr = "[AutoStart server]";
            }

            DataStr.DedicatedServerData ServerData = Shared.LoadDedicatedServerConfig();

            if (ds == true)
            {
                MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Trying to load save file...");
                MyChatName = "DedicatedServer";
            }

            if (SteamConnect.CanUseSteam == false && ServerData.UsingSteam == true)
            {
                ServerData.UsingSteam = false;
                MelonLogger.Msg(ConsoleColor.Red, ServStr + " In server.json 'UsingSteam' set to true, but you using version of the game that not support Hosting with using Steam! If you use licence version of the game, but still see this, reboot steam.");
                MelonLogger.Msg(ConsoleColor.Magenta, ServStr + " Forced changing 'UsingSteam' to false, and running regular server.");
            }

            if (ServerData.UsingSteam == false)
            {
                if (ds == true)
                {
                    PortForDSLoad = ServerData.Ports;
                    HostUDDSAfterLoad = true;
                    Shared.InitAllPlayers();
                } else
                {
                    Shared.HostAServer(ServerData.Ports);
                }
            } else
            {
                CustomServerName = ServerData.ServerName;
                if (ds == true)
                {
                    Shared.InitAllPlayers();
                    SteamConnect.Main.MakeLobby(ServerData.SteamServerAccessibility, ServerData.MaxPlayers);
                    SetP2PToLobbyForDSAfterLoad = true;
                } else
                {
                    Server.StartSteam(MaxPlayers);
                }
            }
            if (ds == true)
            {
                ForceLoadSlotForDs(ServerData.SaveSlot.ToUpper(), ServerData.StartRegion, ServerData.ExperienceMode, ServerData.Seed);
            }
        }

        public static void Disable()
        {
            KillOnUpdate = true;
            KillEveryInGameMinute = true;
            KillEverySecond = true;
            HarmonyLib.Harmony.UnpatchAll();
        }

        public override void OnApplicationStart()
        {
            SetAppBackgroundMode();
            bool DedicatedServerWithRender = Environment.GetCommandLineArgs().Contains("-dedicated");
            DedicatedServerAppMode = DedicatedServerWithRender || Application.isBatchMode;

            ModsValidation.GetModsHash(true);

            bool ForceNoSteam = Environment.GetCommandLineArgs().Contains("-nosteam");
            bool ForceNoEgs = Environment.GetCommandLineArgs().Contains("-noegs");

            if (typeof(SteamManager).GetMethod("Awake") == null || ForceNoSteam)
            {
                if (ForceNoSteam)
                {
                    MelonLogger.Msg(ConsoleColor.DarkMagenta, "[SteamWorks.NET] Force no steam enabled");
                }
                MelonLogger.Msg("[SteamWorks.NET] This game version has not SteamManager");
                var original = typeof(EpicOnlineServicesManager).GetMethod("Start");
                if (original == null || ForceNoEgs)
                {
                    MelonLogger.Msg("[EpicOnlineServicesManager] This game version has not EpicOnlineServicesManager");
                } else {
                    MelonLogger.Msg("[EpicOnlineServicesManager] This game version has EpicOnlineServicesManager");
                    var postfix = typeof(Pathes).GetMethod("EGSHook");
                    HarmonyInstance.Patch(original, null, new HarmonyLib.HarmonyMethod(postfix));
                    MelonLogger.Msg("[EpicOnlineServicesManager] Patching EpicOnlineServicesManager complete!");
                }
            } else {
                MelonLogger.Msg("[SteamWorks.NET] This game version has SteamManager");
                var original = typeof(SteamManager).GetMethod("Awake");
                var postfix = typeof(SteamConnect).GetMethod("Init");
                HarmonyInstance.Patch(original, null, new HarmonyLib.HarmonyMethod(postfix));
                MelonLogger.Msg("[SteamWorks.NET] Patching SteamManager complete!");
            }

            Supporters.GetSupportersList();

            Debug.Log($"[{InfoAttribute.Name}] Version {InfoAttribute.Version} loaded!");


            LoadedBundle = AssetBundle.LoadFromFile("Mods\\multiplayerstuff.unity3d");
            if (LoadedBundle == null)
            {
                MelonLogger.Msg("Have problems with loading multiplayerstuff.unity3d!!");
            } else {
                MelonLogger.Msg("Models loaded.");
            }

            if (DedicatedServerAppMode)
            {
                MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Please wait...");
                InputManager.m_InputDisableTime = float.PositiveInfinity;
                MPSaveManager.SaveRecentTimer = 30;
            }
            uConsole.RegisterCommand("oleg", new Action(LootEverything));
            uConsole.RegisterCommand("gimmekey", new Action(CreateDebugKey));
            uConsole.RegisterCommand("givemekey", new Action(CreateDebugKey));
            Comps.RegisterComponents();
        }

        public static void LootEverything()
        {
            int lootedByOleg = 0;
            BreakDown[] objectsOfType = (BreakDown[])UnityEngine.Object.FindObjectsOfType<BreakDown>();
            if (objectsOfType != null)
            {
                int num = 0;
                foreach (BreakDown breakDown in objectsOfType)
                {
                    if (breakDown.enabled == true)
                    {
                        for (int i = 0; i < breakDown.m_YieldObjectUnits.Count; i++)
                        {
                            lootedByOleg = lootedByOleg + breakDown.m_YieldObjectUnits[i];
                        }
                        breakDown.DoBreakDown(true);
                        ++num;
                    }
                }
            }
            for (int i = 0; i < GearManager.m_Gear.Count; i++)
            {
                lootedByOleg = lootedByOleg + 1;
                GearItem currentGear = GearManager.m_Gear[i];
                GameManager.GetPlayerManagerComponent().ProcessPickupItemInteraction(currentGear, false, false);
            }
            for (int i = 0; i < ContainerManager.m_Containers.Count; i++)
            {
                Container currentBox = ContainerManager.m_Containers[i];
                currentBox.InstantiateContents();
                currentBox.m_Inspected = true;

                Il2CppSystem.Collections.Generic.List<GearItem> m_Gears = new Il2CppSystem.Collections.Generic.List<GearItem>();
                for (int index = 0; index < currentBox.m_Items.Count; ++index)
                {
                    GearItem gearItem = currentBox.m_Items[index];
                    if (gearItem != null)
                    {
                        lootedByOleg = lootedByOleg + 1;
                        GameManager.GetPlayerManagerComponent().ProcessPickupItemInteraction(gearItem, false, false);
                    }
                }
                currentBox.m_Items.Clear();
            }
            MelonLogger.Msg("Oleg found and looted " + lootedByOleg + " gears");
            Debug.Log("Oleg found and looted " + lootedByOleg + " gears");
        }

        public static bool QuitWithoutSaving = false;
        public static bool IReallyWantToQuit = false;
        public static bool NoMelonQuit = false;

        public override void OnPreferencesSaved()
        {
            if (NoMelonQuit)
            {
                Application.CancelQuit();
            }
        }

        public override void OnApplicationQuit()
        {
            if (IReallyWantToQuit || DedicatedServerAppMode)
            {
                if (InOnline())
                {
                    MelonLogger.Msg("[CLIENT] Disconnect cause quit game");
                    Disconnect();
                }
                return;
            }

            if (InOnline())
            {                
                if (!QuitWithoutSaving)
                {
                    if (level_name != "MainMenu")
                    {
                        Pathes.QuitOnSave = true;
                        QuitWithoutSaving = true;
                        NoMelonQuit = true;
                        SaveGameSystem.SetAsyncEnabled(false);
                        GameManager.ForceSaveGame();
                        Application.CancelQuit();
                    }
                } else {
                    MelonLogger.Msg("[CLIENT] Disconnect cause quit game");
                    Disconnect();
                }
            }
        }

        public static Container MakeDeathCreate(DataStr.DeathContainerData data)
        {
            if (ObjectGuidManager.Lookup(data.m_Guid) != null)
            {
                MelonLogger.Msg("Death container  " + data.m_Guid + " already exist");
                return null;
            }

            ///GameObject reference = GetGearItemObject("CONTAINER_InaccessibleGear");
            GameObject reference = GetGearItemObject(data.m_ContainerPrefab);
            if (reference == null)
            {
                return null;
            }
            GameObject box = UnityEngine.Object.Instantiate<GameObject>(reference, data.m_Position, data.m_Rotation);
            InaccessibleGearContainer trash = box.GetComponent<InaccessibleGearContainer>();
            if (trash)
            {
                UnityEngine.Object.Destroy(trash);
            }
            if (box.GetComponent<GearItem>() != null)
            {
                UnityEngine.Object.Destroy(box.GetComponent<GearItem>());
            }
            if (box.GetComponent<Inspect>() != null)
            {
                UnityEngine.Object.Destroy(box.GetComponent<Inspect>());
            }
            if (box.GetComponent<CarryingCapacityBuff>() != null)
            {
                UnityEngine.Object.Destroy(box.GetComponent<CarryingCapacityBuff>());
            }
            if (box.GetComponent<NarrativeCollectibleItem>() != null)
            {
                UnityEngine.Object.Destroy(box.GetComponent<NarrativeCollectibleItem>());
            }
            if (box.GetComponent<Rigidbody>() != null)
            {
                UnityEngine.Object.Destroy(box.GetComponent<Rigidbody>());
            }
            ObjectGuid gu = box.GetComponent<ObjectGuid>();
            if (gu == null)
            {
                gu = box.AddComponent<ObjectGuid>();
            }
            gu.Set(data.m_Guid);
            box.SetActive(true);
            Container Con = box.GetComponent<Container>();
            if (Con == null)
            {
                Con = box.AddComponent<Container>();
                Con.m_CloseAudio = "Play_SndGenCanvasBagZipperClose1";
                Con.m_SearchAudio = "Play_SearchCloth";
                Con.m_OpenAudio = "Play_SndGenCanvasBagZipperOpen1";
                Con.m_LocalizedDisplayName = new LocalizedString();
                Con.m_LocalizedDisplayName.m_LocalizationID = "GAMEPLAY_TechnicalBackpack";
            }
            Con.m_CapacityKG = 0;
            Con.m_Inspected = true;
            Con.m_StartInspected = true;
            if (box.GetComponent<Comps.DeathDropContainer>() == null)
            {
                box.AddComponent<Comps.DeathDropContainer>().m_Owner = data.m_Owner;
            }
            BoxCollider BoxCol = box.GetComponent<BoxCollider>();
            if (BoxCol)
            {
                BoxCol.isTrigger = true;
            }

            return Con;
        }

        public static void RemoveDeathContainer(string GUID, string Scene)
        {
            GameObject Box = ObjectGuidManager.Lookup(GUID);
            if (Box == null)
            {
                MelonLogger.Msg("Death container " + GUID + " not exist, can't be deleted");
            } else {
                UnityEngine.Object.Destroy(Box);
            }


            if (iAmHost)
            {
                foreach (DataStr.DeathContainerData create in DeathCreates)
                {
                    if (create.m_Guid == GUID)
                    {
                        DeathCreates.Remove(create);
                        return;
                    }
                }
            }
        }

        public static void DropAll()
        {
            DataStr.DeathContainerData ObjSync = new DataStr.DeathContainerData();
            ObjSync.m_Guid = ObjectGuidManager.GenerateNewGuidString();
            ObjSync.m_LevelKey = level_guid;
            ObjSync.m_Position = GameManager.GetPlayerTransform().position;
            ObjSync.m_Rotation = GameManager.GetPlayerTransform().rotation;
            ObjSync.m_Owner = MyChatName;
            ObjSync.m_ContainerPrefab = "CONTAINER_BackPack";
            bool SetFullWet = false;
            if (GameManager.GetIceCrackingManager().FellInRecently())
            {
                MelonLogger.Msg("Died from falling in water");
                ObjSync.m_Position = GameManager.GetIceCrackingManager().GetRespawnLocation();
                SetFullWet = true;
            }

            Container Box = MakeDeathCreate(ObjSync);
            if (Box)
            {
                Inventory Inv = GameManager.GetInventoryComponent();
                List<GearItem> Gears = new List<GearItem>();
                for (int index = 0; index < Inv.m_Items.Count; ++index)
                {
                    GearItem gearItem = (GearItem)Inv.m_Items[index];
                    if (gearItem)
                    {
                        if (SetFullWet && gearItem.m_ClothingItem)
                        {
                            gearItem.m_ClothingItem.SetFullyWet();
                        }

                        Gears.Add(gearItem);

                        if (GameManager.GetPlayerManagerComponent().m_ItemInHands && GameManager.GetPlayerManagerComponent().m_ItemInHands == gearItem)
                        {
                            GameManager.GetPlayerManagerComponent().UnequipImmediate(false);
                        }
                    }
                }

                foreach (GearItem Gear in Gears)
                {
                    if (!Gear.m_HandheldShortwaveItem)
                    {
                        if (Gear.m_WaterSupply)
                        {
                            InterfaceManager.m_Panel_PickWater.TransferAllWaterInventoryToContainer(Box, Gear.m_WaterSupply);
                        }
                        else if (Gear.m_StackableItem && Box.AddToExistingStackable(Gear.name, Gear.GetNormalizedCondition(), Gear.m_StackableItem.m_Units, Gear))
                        {
                            Inv.DestroyGear(Gear.gameObject);
                        } else {
                            Box.AddGear(Gear);
                            Inv.RemoveGear(Gear.gameObject, true);
                        }
                    }
                }

                string Content = Box.Serialize();

                if (iAmHost)
                {
                    MPSaveManager.SaveContainer(ObjSync.m_LevelKey, ObjSync.m_Guid, Shared.CompressString(Content));
                    DeathCreates.Add(ObjSync);
                    ServerSend.ADDDEATHCONTAINER(ObjSync, ObjSync.m_LevelKey, 0);
                }
                else if (sendMyPosition)
                {
                    DoPleaseWait("Please wait...", "Sending container data...");
                    Shared.SendContainerData(Shared.CompressString(Content), ObjSync.m_LevelKey, ObjSync.m_Guid, Content);
                    using (Packet _packet = new Packet((int)ClientPackets.ADDDEATHCONTAINER))
                    {
                        _packet.Write(ObjSync);
                        SendUDPData(_packet);
                    }
                }

                Box.DestroyAllGear();
                MelonLogger.Msg("Everything dropped");
            }
        }


        public override void OnLevelWasInitialized(int level)
        {
            if (KillOnUpdate == true)
            {
                return;
            }

            OpenablesObjs.Clear();
            MelonLogger.Msg("Level initialized: " + level);
            levelid = level;

            if (levelid == 0 && sendMyPosition)
            {
                using (Packet _packet = new Packet((int)ClientPackets.FORCELOADING))
                {
                    _packet.Write(true);
                    SendUDPData(_packet);
                }
            }

            MelonLogger.Msg("Level name: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            level_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            MyTicksOnScene = 0;

            if (level_name != "MainMenu" && level_name != "Boot" && level_name != "Empty")
            {
                CheckHaveBookMod();
            }

            DisableOriginalAnimalSpawns();
            Shared.InitAllPlayers();
        }

        public static GameObject MakeModObject(string _name, Transform newparent = null)
        {
            if (LoadedBundle == null)
            {
                MelonLogger.Msg("[Object loader] Bundle is null ");
            }

            GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>(_name);

            if (LoadedAssets == null)
            {
                MelonLogger.Msg("[Object loader] Can't load asset. Has try load " + _name);
            }

            GameObject _Obj = null;

            if (newparent == null)
            {
                _Obj = GameObject.Instantiate(LoadedAssets);
            } else {
                _Obj = GameObject.Instantiate(LoadedAssets, newparent);
            }

            if (_Obj == null)
            {
                MelonLogger.Msg("[Object loader] Object is null  " + _name);
            }

            return _Obj;
        }
        public static bool IsZero(float a)
        {
            return Mathf.Abs(a) <= 0.0001f;
        }

        public static void SimUDPHandle(int _packetId, Packet _packet, int _from)
        {
            //MelonLogger.Msg("[UDP SIM HOST] Trying sim packet " + _packetId);
            Server.packetHandlers[_packetId](_from, _packet);
        }
        public static void SimUDPHandle_Client(int _packetId, Packet _packet)
        {
            //MelonLogger.Msg("[UDP SIM CLIENT] Trying sim packet " + _packetId);
            ClientUser.packetHandlers[_packetId](_packet);
        }

        public static void NoSyncFurtitureDestory(BreakDown breakDown, string GUID, bool ReBake)
        {
            breakDown.gameObject.SetActive(false);
            if (breakDown.gameObject.transform.parent && breakDown.gameObject.transform.parent.GetComponent<RadialObjectSpawner>())
            {
                BreakDown.m_BreakDownObjects.Remove(breakDown);
                breakDown.gameObject.transform.parent.GetComponent<RadialObjectSpawner>().RemoveFromSpawns(breakDown.gameObject);
                RadialSpawnManager.ReturnToObjectPool(breakDown.gameObject);
            }
            breakDown.StickSurfaceObjectsToGround();
            if (ReBake)
            {
                BakePreSpawnedGearsList();
            }
            MissionUtils.PostObjectEvent(breakDown.gameObject, MissionTypes.MissionObjectEvent.ObjectBrokenDown);
            MelonLogger.Msg("Furn " + GUID+" removed");
        }
        public static void NoSyncFurtitureRepair(BreakDown breakDown, string GUID)
        {
            breakDown.gameObject.SetActive(true);
            MelonLogger.Msg("Furn " + GUID + " repaired");
        }

        public static string GetBreakDownSound(DataStr.BrokenFurnitureSync FindData)
        {
            GameObject furn = ObjectGuidManager.Lookup(FindData.m_Guid);
            if (furn != null )
            {
                if (furn.GetComponent<BreakDown>())
                {
                    if (FindData.m_ParentGuid != "")
                    {
                        if (furn.transform.parent != null && furn.transform.parent.GetComponent<ObjectGuid>() != null && furn.transform.parent.GetComponent<ObjectGuid>().Get() == FindData.m_ParentGuid)
                        {
                            return furn.GetComponent<BreakDown>().m_BreakDownAudio;
                        }
                    } else
                    {
                        return furn.GetComponent<BreakDown>().m_BreakDownAudio;
                    }
                } else if(furn.GetComponent<FakeRockCache>())
                {
                    return "Play_RockCache";
                }
            }
            return "";
        }
        public static bool FoundSomethingToBreak = false;

        public static void RemoveBrokenFurniture(string GUID, string ParentGUID, bool ReBake = true)
        {
            MelonLogger.Msg("Going to remove furn "+ GUID+ ParentGUID);
            GameObject furn = ObjectGuidManager.Lookup(GUID);
            if (furn != null)
            {
                if (ParentGUID != "")
                {
                    if (furn.transform.parent != null && furn.transform.parent.GetComponent<ObjectGuid>() != null && furn.transform.parent.GetComponent<ObjectGuid>().Get() == ParentGUID)
                    {
                        NoSyncFurtitureDestory(furn.GetComponent<BreakDown>(), GUID+ParentGUID, ReBake);
                        FoundSomethingToBreak = true;
                    }
                } else
                {
                    NoSyncFurtitureDestory(furn.GetComponent<BreakDown>(), GUID + ParentGUID, ReBake);
                    FoundSomethingToBreak = true;
                }
            }
        }
        public static void RepairBrokenFurniture(string GUID, string ParentGUID)
        {
            MelonLogger.Msg("Going to repair furn " + GUID + ParentGUID);
            GameObject furn = ObjectGuidManager.Lookup(GUID);
            if (furn != null)
            {
                if (ParentGUID != "")
                {
                    if (furn.transform.parent != null && furn.transform.parent.GetComponent<ObjectGuid>() != null && furn.transform.parent.GetComponent<ObjectGuid>().Get() == ParentGUID)
                    {
                        NoSyncFurtitureRepair(furn.GetComponent<BreakDown>(), GUID + ParentGUID);
                        FoundSomethingToBreak = true;
                    }
                } else
                {
                    NoSyncFurtitureRepair(furn.GetComponent<BreakDown>(), GUID + ParentGUID);
                    FoundSomethingToBreak = true;
                }
            }
        }

        public static Dictionary<long, GameObject> PreSpawnedGears = new Dictionary<long, GameObject>();
        public static void BakePreSpawnedGearsList()
        {
            PreSpawnedGears.Clear();
            for (int i = 0; i < GearManager.m_Gear.Count; i++)
            {
                GearItem currentGear = GearManager.m_Gear[i];
                GameObject obj = currentGear.gameObject;
                long Key = MPSaveManager.GetPickedGearKey(obj.transform.position);

                if (currentGear.m_BeenInPlayerInventory == false && obj.activeSelf == true && currentGear.m_InsideContainer == false)
                {
                    //MelonLogger.Msg("[BakePreSpawnedGearsList] " + currentGear.m_GearName + " Key " + Key);
                    if (!PreSpawnedGears.ContainsKey(Key))
                    {
                        PreSpawnedGears.Add(Key, obj);
                    } else
                    {
                        //MelonLogger.Msg(ConsoleColor.Red,"[BakePreSpawnedGearsList] GEAR ALREADY IN THE LIST! " + currentGear.m_GearName + " Key " + Key);
                    }
                }
            }
        }

        public static void RemovePickedGear(long Key)
        {
            MelonLogger.Msg("[RemovePickedGear] Key " + Key);
            if (ServerConfig.m_DuppedSpawns == true)
            {
                return;
            }
            GameObject Gear;
            if (PreSpawnedGears.TryGetValue(Key, out Gear))
            {
                if(Gear.GetComponent<GearItem>() && Gear.GetComponent<GearItem>().m_BeenInPlayerInventory == false && Gear.gameObject.activeSelf == true)
                {
                    Gear.gameObject.SetActive(false);
                }
            }
        }

        public static void UpdateDeployedRopes()
        {
            //MelonLogger.Msg("UpdateDeployedRopes called");
            for (int i = 0; i < RopeAnchorPoint.m_RopeAnchorPoints.Count; i++)
            {
                RopeAnchorPoint rope = RopeAnchorPoint.m_RopeAnchorPoints[i];
                DataStr.ClimbingRopeSync FindData = new DataStr.ClimbingRopeSync();
                FindData.m_LevelID = levelid;
                FindData.m_LevelGUID = level_guid;
                FindData.m_Position = rope.gameObject.transform.position;
                if (DeployedRopes.Contains(FindData) == true)
                {
                    DataStr.ClimbingRopeSync FoundData = new DataStr.ClimbingRopeSync();
                    for (int n = 0; n < DeployedRopes.Count; n++)
                    {
                        DataStr.ClimbingRopeSync currRope = DeployedRopes[n];
                        if (currRope.m_Position == FindData.m_Position && currRope.m_LevelID == levelid && currRope.m_LevelGUID == level_guid)
                        {
                            FoundData = currRope;
                            break;
                        }
                    }
                    if (rope.gameObject.transform.position == FoundData.m_Position)
                    {
                        MelonLogger.Msg("Processing synced rope " + FoundData.m_Position.x + " Y " + FoundData.m_Position.y + " Z " + FoundData.m_Position.z);
                        if (FoundData.m_Deployed == rope.m_RopeDeployed && FoundData.m_Snapped == rope.m_RopeSnapped)
                        {
                            MelonLogger.Msg("Rope already has desired state");
                        } else {
                            if (FoundData.m_Deployed == true)
                            {
                                rope.SetRopeActiveState(FoundData.m_Deployed, false);
                                rope.SetRopeActiveState(true, true);
                                rope.m_RopeDeployed = true;
                                if (FoundData.m_Snapped == true)
                                {
                                    if (rope.m_RopeSnapped == false)
                                    {
                                        rope.SetRopeActiveState(false, false);
                                        if (rope.m_RopeShort != null && rope.m_RopeShort.gameObject != null)
                                        {
                                            rope.m_RopeShort.gameObject.SetActive(false);
                                        }
                                        rope.m_RopeSnapped = true;
                                    }
                                }
                            } else if (FoundData.m_Deployed == false)
                            {
                                rope.m_RopeDeployed = false;
                                rope.SetRopeActiveState(false, true);
                                if (rope.m_RopeSnapped == true && FoundData.m_Snapped == false)
                                {
                                    if (rope.m_RopeShort != null && rope.m_RopeShort.gameObject != null)
                                    {
                                        rope.m_RopeShort.gameObject.SetActive(false);
                                    }
                                    rope.m_RopeSnapped = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void CheckHaveBookMod()
        {
            GameObject hBook = GameObject.Find("/CHARACTER_FPSPlayer/WeaponView/WeaponCamera/hands_with_book(Clone)");
            if (hBook)
            {
                ModdedHandsBook = hBook;
            }
            string Path = "/CHARACTER_FPSPlayer/NEW_FPHand_Rig/GAME_DATA/Origin/HipJoint/Chest_Joint/Camera_Weapon_Offset/Shoulder_Joint/Shoulder_Joint_Offset/Right_Shoulder_Joint_Offset/RightClavJoint/RightShoulderJoint/RightElbowJoint/RightWristJoint/RightPalm/right_prop_point/";
            GameObject RadioTransform = GameObject.Find(Path);
            if (!VanilaRadio && RadioTransform)
            {
                if (ViewModelRadio == null)
                {
                    GameObject reference = GetGearItemObject("GEAR_HandheldShortwave");
                    //OBJ_CellPhoneA_Prefab

                    if (reference == null)
                    {
                        return;
                    }

                    ViewModelRadio = UnityEngine.Object.Instantiate<GameObject>(reference, RadioTransform.transform.position, RadioTransform.transform.rotation, RadioTransform.transform);
                    ViewModelRadio.name = "FPH_HandheldRadio";
                    ViewModelRadio.SetActive(false);
                    ViewModelRadioNeedle = ViewModelRadio.transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
                    if (ViewModelRadio.GetComponent<GearItem>() != null)
                    {
                        UnityEngine.Object.Destroy(ViewModelRadio.GetComponent<GearItem>());
                    }
                    if (ViewModelRadio.GetComponent<FirstPersonItem>() != null)
                    {
                        UnityEngine.Object.Destroy(ViewModelRadio.GetComponent<FirstPersonItem>());
                    }
                    if (ViewModelRadio.GetComponent<HoverIconsToShow>() != null)
                    {
                        UnityEngine.Object.Destroy(ViewModelRadio.GetComponent<HoverIconsToShow>());
                    }
                    if (ViewModelRadio.GetComponent<Inspect>() != null)
                    {
                        UnityEngine.Object.Destroy(ViewModelRadio.GetComponent<Inspect>());
                    }
                    if (ViewModelRadio.GetComponent<Rigidbody>() != null)
                    {
                        UnityEngine.Object.Destroy(ViewModelRadio.GetComponent<Rigidbody>());
                    }
                    if (ViewModelRadio.GetComponent<LODGroup>() != null)
                    {
                        UnityEngine.Object.Destroy(ViewModelRadio.GetComponent<LODGroup>());
                    }
                    GameObject obj = new GameObject("LIGHT");
                    obj.transform.SetParent(ViewModelRadio.transform);
                    Light L = obj.AddComponent<Light>();
                    L.color = Color.green;
                    obj.SetActive(false);
                    ViewModelRadioLED = obj;
                }
            }

            ViewModelHatchet = GameObject.Find(Path + "mesh_hatchet");
            ViewModelHatchet2 = GameObject.Find(Path + "mesh_Hatchet_improvised");
            ViewModelKnife = GameObject.Find(Path + "mesh_knife");
            ViewModelKnife2 = GameObject.Find(Path + "mesh_knife_improvised");
            ViewModelPrybar = GameObject.Find(Path + "mesh_prybar");
            ViewModelHammer = GameObject.Find(Path + "mesh_hammer");
            ViewModelStone = GameObject.Find(Path + "FPH_Stone(Clone)");

            ViewModelDummy = GameObject.Find(Path + "FPH_Flare(Clone)");

            if (ViewModelShovel == null)
            {
                GameObject reference = GetGearItemObject("GEAR_Shovel");

                if (reference == null)
                {
                    return;
                }

                ViewModelShovel = UnityEngine.Object.Instantiate<GameObject>(reference, RadioTransform.transform.position, RadioTransform.transform.rotation, RadioTransform.transform);
                ViewModelShovel.name = "FPH_Shovel";
                ViewModelShovel.SetActive(false);

                if (ViewModelShovel.GetComponent<GearItem>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelShovel.GetComponent<GearItem>());
                }
                if (ViewModelShovel.GetComponent<FirstPersonItem>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelShovel.GetComponent<FirstPersonItem>());
                }
                if (ViewModelShovel.GetComponent<HoverIconsToShow>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelShovel.GetComponent<HoverIconsToShow>());
                }
                if (ViewModelShovel.GetComponent<Inspect>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelShovel.GetComponent<Inspect>());
                }
                if (ViewModelShovel.GetComponent<Rigidbody>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelShovel.GetComponent<Rigidbody>());
                }
                if (ViewModelShovel.GetComponent<LODGroup>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelShovel.GetComponent<LODGroup>());
                }
                ViewModelShovel.transform.localEulerAngles = new Vector3(0, 0, 90);
            }
            if (ViewModelFireAxe == null)
            {
                GameObject reference = GetGearItemObject("GEAR_FireAxe");

                if (reference == null)
                {
                    return;
                }

                ViewModelFireAxe = UnityEngine.Object.Instantiate<GameObject>(reference, RadioTransform.transform.position, RadioTransform.transform.rotation, RadioTransform.transform);
                ViewModelFireAxe.name = "FPH_FireAxe";
                ViewModelFireAxe.SetActive(false);
                if (ViewModelFireAxe.GetComponent<GearItem>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelFireAxe.GetComponent<GearItem>());
                }
                if (ViewModelFireAxe.GetComponent<ForageItem>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelFireAxe.GetComponent<ForageItem>());
                }
                if (ViewModelFireAxe.GetComponent<HoverIconsToShow>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelFireAxe.GetComponent<HoverIconsToShow>());
                }
                if (ViewModelFireAxe.GetComponent<Inspect>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelFireAxe.GetComponent<Inspect>());
                }
                if (ViewModelFireAxe.GetComponent<DegradeOnUse>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelFireAxe.GetComponent<DegradeOnUse>());
                }
                if (ViewModelFireAxe.GetComponent<Repairable>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelFireAxe.GetComponent<Repairable>());
                }
                if (ViewModelFireAxe.GetComponent<BodyHarvestItem>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelFireAxe.GetComponent<BodyHarvestItem>());
                }
                if (ViewModelFireAxe.GetComponent<Rigidbody>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelFireAxe.GetComponent<Rigidbody>());
                }
                if (ViewModelFireAxe.GetComponent<CanOpeningItem>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelFireAxe.GetComponent<CanOpeningItem>());
                }
                if (ViewModelFireAxe.GetComponent<LODGroup>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelFireAxe.GetComponent<LODGroup>());
                }
                ViewModelFireAxe.transform.localEulerAngles = new Vector3(0, 0, 90);
                ViewModelFireAxe.transform.localPosition = new Vector3(0, 0.15f, 0.01f);
            }
            if (ViewModelJeremiahKnife == null)
            {
                GameObject reference = GetGearItemObject("GEAR_JeremiahKnife");

                if (reference == null)
                {
                    return;
                }
                ViewModelJeremiahKnife = UnityEngine.Object.Instantiate<GameObject>(ViewModelKnife, ViewModelKnife.transform.position, ViewModelKnife.transform.rotation, RadioTransform.transform);
                ViewModelJeremiahKnife.name = "FPH_JeremiahKnife";
                ViewModelJeremiahKnife.SetActive(false);
                ViewModelJeremiahKnife.GetComponent<MeshRenderer>().SetMaterial(reference.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0]);
            }

            if (ViewModelBolt == null)
            {
                GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>("Bolt");
                ViewModelBolt = GameObject.Instantiate(LoadedAssets, RadioTransform.transform.position, RadioTransform.transform.rotation, RadioTransform.transform);
                ViewModelBolt.name = "FPH_Bolt";
                ViewModelBolt.transform.localPosition = new Vector3(0.02f, 0.02f, -0.01f);
                ViewModelBolt.transform.localRotation = new Quaternion(0.3304f, 0.9077f, -0.0885f, -0.2432f);
                ViewModelBolt.transform.localScale = new Vector3(0.045f, 0.045f, 0.045f);
                ViewModelBolt.SetActive(false);
            }
            if (ViewModelHackSaw == null)
            {
                GameObject reference = GetGearItemObject("GEAR_Hacksaw");

                if (reference == null)
                {
                    return;
                }

                ViewModelHackSaw = UnityEngine.Object.Instantiate<GameObject>(reference, RadioTransform.transform.position, RadioTransform.transform.rotation, RadioTransform.transform);
                ViewModelHackSaw.name = "FPH_Hacksaw";
                ViewModelHackSaw.SetActive(false);
                if (ViewModelHackSaw.GetComponent<GearItem>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelHackSaw.GetComponent<GearItem>());
                }
                if (ViewModelHackSaw.GetComponent<ForageItem>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelHackSaw.GetComponent<ForageItem>());
                }
                if (ViewModelHackSaw.GetComponent<HoverIconsToShow>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelHackSaw.GetComponent<HoverIconsToShow>());
                }
                if (ViewModelHackSaw.GetComponent<Inspect>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelHackSaw.GetComponent<Inspect>());
                }
                if (ViewModelHackSaw.GetComponent<DegradeOnUse>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelHackSaw.GetComponent<DegradeOnUse>());
                }
                if (ViewModelHackSaw.GetComponent<Repairable>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelHackSaw.GetComponent<Repairable>());
                }
                if (ViewModelHackSaw.GetComponent<BodyHarvestItem>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelHackSaw.GetComponent<BodyHarvestItem>());
                }
                if (ViewModelHackSaw.GetComponent<Rigidbody>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelHackSaw.GetComponent<Rigidbody>());
                }
                if (ViewModelHackSaw.GetComponent<CanOpeningItem>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelHackSaw.GetComponent<CanOpeningItem>());
                }
                if (ViewModelHackSaw.GetComponent<LODGroup>() != null)
                {
                    UnityEngine.Object.Destroy(ViewModelHackSaw.GetComponent<LODGroup>());
                }
                ViewModelHackSaw.transform.localEulerAngles = new Vector3(0, 0, 90);
                ViewModelHackSaw.transform.localPosition = new Vector3(0, 0.15f, 0.01f);
            }
            if (ViewModelPhoto == null)
            {
                GameObject reference = GetGearItemObject("GEAR_SCPhoto");

                if (reference == null)
                {
                    return;
                }

                ViewModelPhoto = UnityEngine.Object.Instantiate<GameObject>(reference, RadioTransform.transform.position, RadioTransform.transform.rotation, RadioTransform.transform);
                ViewModelPhoto.name = "FPH_Photo";
                ViewModelPhoto.SetActive(false);

                foreach (Component Com in ViewModelPhoto.GetComponents<Component>())
                {
                    string ComName = Com.GetIl2CppType().Name;
                    if (ComName != PhysicMaterial.Il2CppType.Name
                        && ComName != LODGroup.Il2CppType.Name
                        && ComName != Transform.Il2CppType.Name
                        && ComName != MeshRenderer.Il2CppType.Name
                        && ComName != SkinnedMeshRenderer.Il2CppType.Name)
                    {
                        UnityEngine.Object.Destroy(Com);
                    }
                }
                ViewModelPhoto.transform.localEulerAngles = new Vector3(325, 300, 60);
                ViewModelPhoto.transform.localPosition = new Vector3(0.045f, 0.085f, 0.007f);
            }
            if (ViewModelMap == null)
            {
                GameObject reference = GetGearItemObject("GEAR_SCMapPiece");

                if (reference == null)
                {
                    return;
                }

                ViewModelMap = UnityEngine.Object.Instantiate<GameObject>(reference, RadioTransform.transform.position, RadioTransform.transform.rotation, RadioTransform.transform);
                ViewModelMap.name = "FPH_Map";
                ViewModelMap.SetActive(false);

                foreach (Component Com in ViewModelMap.GetComponents<Component>())
                {
                    string ComName = Com.GetIl2CppType().Name;
                    if (ComName != PhysicMaterial.Il2CppType.Name
                        && ComName != LODGroup.Il2CppType.Name
                        && ComName != Transform.Il2CppType.Name
                        && ComName != MeshRenderer.Il2CppType.Name
                        && ComName != SkinnedMeshRenderer.Il2CppType.Name)
                    {
                        UnityEngine.Object.Destroy(Com);
                    }
                }
                ViewModelMap.transform.localEulerAngles = new Vector3(325, 300, 60);
                ViewModelMap.transform.localPosition = new Vector3(0.045f, 0.085f, 0.007f);
                ViewModelBolt.transform.localScale = new Vector3(85, 85, 85);
            }
            if (ViewModelNote == null)
            {
                GameObject reference = GetGearItemObject("GEAR_SCNote");

                if (reference == null)
                {
                    return;
                }

                ViewModelNote = UnityEngine.Object.Instantiate<GameObject>(reference, RadioTransform.transform.position, RadioTransform.transform.rotation, RadioTransform.transform);
                ViewModelNote.name = "FPH_Note";
                ViewModelNote.SetActive(false);

                foreach (Component Com in ViewModelNote.GetComponents<Component>())
                {
                    string ComName = Com.GetIl2CppType().Name;
                    if (ComName != PhysicMaterial.Il2CppType.Name
                        && ComName != LODGroup.Il2CppType.Name
                        && ComName != Transform.Il2CppType.Name
                        && ComName != MeshRenderer.Il2CppType.Name
                        && ComName != SkinnedMeshRenderer.Il2CppType.Name)
                    {
                        UnityEngine.Object.Destroy(Com);
                    }
                }
                ViewModelNote.transform.localEulerAngles = new Vector3(325, 300, 60);
                ViewModelNote.transform.localPosition = new Vector3(0.03f, 0.067f, 0.01f);
            }
        }

        public static void AddHarvastedPlant(string harvGUID)
        {
            GameObject obj = ObjectGuidManager.Lookup(harvGUID);

            if (obj)
            {
                Harvestable Plant = obj.GetComponent<Harvestable>();
                if (Plant == null)
                {
                    return;
                }

                Plant.gameObject.SetActive(true);
                if (Plant.m_ActivateObjectPostHarvest != null)
                {
                    Plant.m_ActivateObjectPostHarvest.SetActive(false);
                }
                Plant.m_Harvested = false;
            }
        }

        public static void RemoveHarvastedPlant(string harvGUID)
        {
            GameObject obj = ObjectGuidManager.Lookup(harvGUID);

            if (obj)
            {
                Harvestable Plant = obj.GetComponent<Harvestable>();
                if(Plant == null)
                {
                    return;
                }

                if (Plant.m_DestroyObjectOnHarvest == true)
                {
                    UnityEngine.Object.Destroy(Plant.gameObject);
                } else
                {
                    Plant.gameObject.SetActive(false);
                    if (Plant.m_ActivateObjectPostHarvest != null)
                    {
                        Plant.m_ActivateObjectPostHarvest.SetActive(true);
                    }
                }
            }
        }
        public static void RemoveLootFromContainer(GameObject obj, int State = 0)
        {
            if (obj)
            {
                string GUID = "";
                if (obj.GetComponent<ObjectGuid>())
                {
                    GUID = obj.GetComponent<ObjectGuid>().Get();
                }
                //MelonLogger.Msg("RemoveLootFromContainer "+GUID+" State "+State);
                
                Container box = obj.GetComponent<Container>();
                if (box)
                {
                    box.DestroyAllGear();
                    box.m_Inspected = true;


                    ContainersSync Con = obj.GetComponent<Comps.ContainersSync>();
                    if (Con == null)
                    {
                        Con = obj.AddComponent<Comps.ContainersSync>();
                        Con.m_Obj = obj;
                    }
                    if (State == 0)
                    {
                        Con.m_Empty = true;
                    } else if (State == 1)
                    {
                        Con.m_Empty = false;
                    } else if (State == 2 || State == -1)
                    {
                        Con.m_Empty = true;
                        box.m_Inspected = false;
                        box.PopulateWithRandomGear();
                    }
                }
            }
        }


        public static void RemoveLootFromContainer(string GUID, int State = 0)
        {
            GameObject obj = ObjectGuidManager.Lookup(GUID);
            if (obj)
            {
                RemoveLootFromContainer(obj, State);
            } else
            {
               //MelonLogger.Msg(ConsoleColor.Red,"Can't find container " + GUID + " for State " + State);
            }
        }

        public static void SendBrokeFurniture(string guid, string parentguid, int lvl, string lvlguid)
        {
            DataStr.BrokenFurnitureSync furn = new DataStr.BrokenFurnitureSync();
            furn.m_Guid = guid;
            furn.m_ParentGuid = parentguid;
            furn.m_LevelID = lvl;
            furn.m_LevelGUID = lvlguid;

            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.FURNBROKEN))
                {
                    _packet.Write(furn);
                    SendUDPData(_packet);
                }
            }

            if (iAmHost == true)
            {
                ServerSend.FURNBROKEN(0, furn, true);
                MPSaveManager.AddBrokenFurn(furn);
            }
        }

        public static List<GameObject> SpawnedByOtherShelters = new List<GameObject>();

        public static void SpawnSnowShelterByOther(DataStr.ShowShelterByOther shelterData)
        {
            for (int i = 0; i < SpawnedByOtherShelters.Count; i++)
            {
                GameObject checking = SpawnedByOtherShelters[i];
                if (checking != null)
                {
                    if (checking.transform.position == shelterData.m_Position)
                    {
                        return;
                    }
                }
            }
            GameObject shelter = UnityEngine.Object.Instantiate<GameObject>(GameManager.GetSnowShelterManager().m_SnowShelterPrefab.gameObject);
            shelter.name = GameManager.GetSnowShelterManager().m_SnowShelterPrefab.name;
            shelter.transform.position = shelterData.m_Position;
            shelter.transform.rotation = shelterData.m_Rotation;
            shelter.AddComponent<Comps.DoNotSerializeThis>();
            SpawnedByOtherShelters.Add(shelter);
        }

        public static void RemoveSnowShelterByOther(DataStr.ShowShelterByOther shelterData)
        {
            for (int i = 0; i < SpawnedByOtherShelters.Count; i++)
            {
                GameObject shelter = SpawnedByOtherShelters[i];
                if (shelter != null)
                {
                    if (shelter.transform.position == shelterData.m_Position)
                    {
                        SpawnedByOtherShelters.Remove(shelter);
                        UnityEngine.Object.Destroy(shelter);
                        return;
                    }
                }
            }
            for (int i = 0; i < SnowShelterManager.m_SnowShelters.Count; i++)
            {
                GameObject shelter = SnowShelterManager.m_SnowShelters[i].gameObject;
                if (shelter != null)
                {
                    if (shelter.transform.position == shelterData.m_Position)
                    {
                        SnowShelterManager.m_SnowShelters.RemoveAt(i);
                        UnityEngine.Object.Destroy(shelter);
                        return;
                    }
                }
            }
        }

        public static void LoadAllSnowSheltersByOther()
        {
            //MelonLogger.Msg("LoadAllSnowSheltersByOther called");
            for (int i = 0; i < ShowSheltersBuilded.Count; i++)
            {
                DataStr.ShowShelterByOther shelter = ShowSheltersBuilded[i];
                if (shelter != null)
                {
                    SpawnSnowShelterByOther(shelter);
                }
            }
        }

        public static Animator RemoveAnimalComponents(GameObject animal)
        {
            if (animal == null)
            {
                return null;
            }
            if (animal.transform.parent != null && animal.transform.parent.GetComponent<MoveAgent>() != null)
            {
                UnityEngine.Object.Destroy(animal.transform.parent.GetComponent<MoveAgent>());
            }
            if (animal.transform.parent != null && animal.transform.parent.GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
            {
                UnityEngine.Object.Destroy(animal.transform.parent.GetComponent<UnityEngine.AI.NavMeshAgent>());
            }
            if (animal.GetComponent<AiTarget>() != null)
            {
                UnityEngine.Object.Destroy(animal.GetComponent<AiTarget>());
            }
            if (animal.GetComponent<AiWolf>() != null)
            {
                UnityEngine.Object.Destroy(animal.GetComponent<AiWolf>());
            }
            if (animal.GetComponent<AiStag>() != null)
            {
                UnityEngine.Object.Destroy(animal.GetComponent<AiStag>());
            }
            if (animal.GetComponent<AiRabbit>() != null)
            {
                UnityEngine.Object.Destroy(animal.GetComponent<AiRabbit>());
            }
            //MelonLogger.Msg("AiRabbit yahooo");
            if (animal.GetComponent<AiMoose>() != null)
            {
                UnityEngine.Object.Destroy(animal.GetComponent<AiMoose>());
            }

            if (animal.GetComponent<AiBear>() != null)
            {
                UnityEngine.Object.Destroy(animal.GetComponent<AiBear>());
            }
            if (animal.GetComponent<CharacterController>() != null)
            {
                UnityEngine.Object.Destroy(animal.GetComponent<CharacterController>());
            }
            //MelonLogger.Msg("CharacterController ANUS SEBE CONTROLIRUI");
            if (animal.GetComponent<NodeCanvas.Framework.Blackboard>() != null)
            {
                UnityEngine.Object.Destroy(animal.GetComponent<NodeCanvas.Framework.Blackboard>());
            }
            //MelonLogger.Msg("Blackboard DA BECAUSE DA");
            if (animal.GetComponent<TLDBehaviourTreeOwner>() != null)
            {
                UnityEngine.Object.Destroy(animal.GetComponent<TLDBehaviourTreeOwner>());
            }
            if (animal.GetComponent<Comps.AnimalUpdates>() != null)
            {
                UnityEngine.Object.Destroy(animal.GetComponent<Comps.AnimalUpdates>());
            }
            if (animal.GetComponent<BaseAi>() != null)
            {
                Animator anim = animal.GetComponent<BaseAi>().m_Animator;
                UnityEngine.Object.Destroy(animal.GetComponent<BaseAi>());
                return anim;
            }
            return null;
        }

        public static void LongActionCancleCauseMoved(Comps.MultiplayerPlayer mP)
        {
            if (mP.m_InteractTimer < 2 && mP.m_IsBeingInteractedWith == true && PlayerInteractionWith == mP && GetActionForOtherPlayer(mP.m_ActionType).m_CancleOnMove == true)
            {
                if (MyMod.playersData[mP.m_ID] != null)
                {
                    HUDMessage.AddMessage(MyMod.playersData[mP.m_ID].m_Name + " MOVED, ACTION CANCELED");
                }
                LongActionCanceled(mP);
            }
        }
        public static void DoLongAction(GameObject Obj, string actionString, string actionType)
        {
            float Duration = GetCustomAction(actionType).m_ActionDuration;
            ObjectInteractWith = Obj;
            InteractHold = GetCustomAction(actionType).m_Hold;
            InteractionType = actionType;
            InteractTimer = Duration;
            PreviousControlModeBeforeAction = GameManager.GetPlayerManagerComponent().GetControlMode();
            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);
            InteractionInprocess = true;
            InterfaceManager.m_Panel_HUD.StartItemProgressBar(Duration, actionString, (GearItem)null, new System.Action(EmptyFn));

            if (actionType == "Excision")
            {
                if (InterfaceManager.m_Panel_Inventory_Examine && Obj && Obj.GetComponent<GearItem>() && Obj.GetComponent<GearItem>().m_Sharpenable)
                {
                    InterfaceManager.m_Panel_Inventory_Examine.m_ProgressBarAudio = GameAudioManager.PlaySound(Obj.GetComponent<GearItem>().m_Sharpenable.m_SharpenAudio, InterfaceManager.GetSoundEmitter());
                }
            } else if (actionType == "Lockpick")
            {
                if (InterfaceManager.m_Panel_Inventory_Examine)
                {
                    InterfaceManager.m_Panel_Inventory_Examine.m_ProgressBarAudio = GameAudioManager.PlaySound("PLAY_CRAFTINGGENERIC", InterfaceManager.GetSoundEmitter());
                }
            }
            else if (actionType == "Locksmith0")
            {
                if (InterfaceManager.m_Panel_Inventory_Examine)
                {
                    InterfaceManager.m_Panel_Inventory_Examine.m_ProgressBarAudio = GameAudioManager.PlaySound("PLAY_HARVESTINGMETALSAW", InterfaceManager.GetSoundEmitter());
                }
            }
            else if (actionType == "Locksmith1")
            {
                if (InterfaceManager.m_Panel_Inventory_Examine)
                {
                    InterfaceManager.m_Panel_Inventory_Examine.m_ProgressBarAudio = GameAudioManager.PlaySound("PLAY_CRAFTINGARROWS", InterfaceManager.GetSoundEmitter());
                }
            }
        }


        public static void DoLongAction(Comps.MultiplayerPlayer mP, string actionString, string actionType)
        {
            mP.m_ActionType = actionType;
            mP.m_IsBeingInteractedWith = true;
            mP.m_InteractTimer = GetActionForOtherPlayer(mP.m_ActionType).m_ActionDuration;
            PlayerInteractionWith = mP;
            PreviousControlModeBeforeAction = GameManager.GetPlayerManagerComponent().GetControlMode();
            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);
            InterfaceManager.m_Panel_HUD.StartItemProgressBar(mP.m_InteractTimer, actionString, (GearItem)null, new System.Action(EmptyFn));

            if (actionType != "Revive")
            {
                if (sendMyPosition == true) // CLIENT
                {
                    using (Packet _packet = new Packet((int)ClientPackets.DONTMOVEWARNING))
                    {
                        _packet.Write(true);
                        _packet.Write(mP.m_ID);
                        SendUDPData(_packet);
                    }
                }
                if (iAmHost == true) // HOST
                {
                    using (Packet _packet = new Packet((int)ServerPackets.DONTMOVEWARNING))
                    {
                        ServerSend.DONTMOVEWARNING(0, true, false, mP.m_ID);
                    }
                }
            }
        }

        public static void LongActionCanceled(Comps.MultiplayerPlayer mP)
        {
            if (GameManager.GetPlayerManagerComponent().GetControlMode() == PlayerControlMode.Locked)
                GameManager.GetPlayerManagerComponent().SetControlMode(PreviousControlModeBeforeAction);

            if (mP.m_ActionType == "Stim" && EmergencyStimBeforeUse != null)
            {
                GameManager.GetPlayerManagerComponent().UseInventoryItem(EmergencyStimBeforeUse);// Equip GEAR_EmergencyStim
                EmergencyStimBeforeUse = null;
            }
            mP.m_ActionType = "";
            mP.m_IsBeingInteractedWith = false;
            mP.m_InteractTimer = 0.0f;
            PlayerInteractionWith = null;
            InterfaceManager.m_Panel_HUD.CancelItemProgressBar();
        }

        public static void LongActionCanceled()
        {
            if (GameManager.GetPlayerManagerComponent().GetControlMode() == PlayerControlMode.Locked)
            {
                GameManager.GetPlayerManagerComponent().SetControlMode(PreviousControlModeBeforeAction);
            }
            ObjectInteractWith = null;
            InteractionInprocess = false;
            InteractTimer = 0.0f;
            InteractHold = false;
            InterfaceManager.m_Panel_HUD.CancelItemProgressBar();
            if (InteractionType == "Excision" || InteractionType == "Lockpick" || InteractionType == "Locksmith0" || InteractionType == "Locksmith1")
            {
                if (InterfaceManager.m_Panel_Inventory_Examine)
                {
                    InterfaceManager.m_Panel_Inventory_Examine.StopProgressBarAudio();
                }
            }
        }
        public static void LongActionFinished(GameObject obj, string ActionType)
        {
            if (ActionType == "Excision" && obj)
            {
                GearItem new_gear = GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_JeremiahKnife");
                DisableObjectForXPMode.RemoveDisabler(new_gear.gameObject);
                new_gear.m_RolledSpawnChance = true;
                new_gear.m_BeenInPlayerInventory = true;
                new_gear.ManualStart();
                GameManager.GetPlayerManagerComponent().EquipItem(new_gear, false);
                GameManager.GetInventoryComponent().RemoveGear(obj);
            } else if (ActionType == "Lockpick")
            {
                if (SwearOnLockpickingDone)
                {
                    GameAudioManager.PlaySound("Play_SndInvCrowbarPrybar", InterfaceManager.GetSoundEmitter());
                    GameManager.GetPlayerVoiceComponent().Play("PLAY_FIREFAIL", Voice.Priority.Critical);
                } else {
                    GameAudioManager.PlaySound("PLAY_SNDMECHSAFETUMBLERFALL", InterfaceManager.GetSoundEmitter());
                    GameManager.GetPlayerVoiceComponent().Play("PLAY_FIRESUCCESS", Voice.Priority.Critical);
                }
            }else if(ActionType == "Locksmith0" || ActionType == "Locksmith1")
            {
                if(obj && obj.GetComponent<Comps.DroppedGearDummy>())
                {
                    int Hash = obj.GetComponent<Comps.DroppedGearDummy>().m_SearchKey;
                    int tool = 0;
                    if (ActionType == "Locksmith0")
                    {
                        tool = 0;
                    }
                    else if (ActionType == "Locksmith1")
                    {
                        tool = 1;
                    }

                    if (iAmHost)
                    {
                        if (PendingRegisterKey)
                        {
                            MPSaveManager.ApplyToolOnBlank(Hash, tool, PendingKeyName, PendingKeySeed);
                        }else{
                            MPSaveManager.ApplyToolOnBlank(Hash, tool);
                        }
                    }
                    if(sendMyPosition)
                    {
                        if (PendingRegisterKey)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.APPLYTOOLONBLANK))
                            {
                                _packet.Write(Hash);
                                _packet.Write(tool);
                                _packet.Write(true);
                                _packet.Write(PendingKeyName);
                                _packet.Write(PendingKeySeed);
                                SendUDPData(_packet);
                            }
                        }else{
                            using (Packet _packet = new Packet((int)ClientPackets.APPLYTOOLONBLANK))
                            {
                                _packet.Write(Hash);
                                _packet.Write(tool);
                                _packet.Write(false);
                                SendUDPData(_packet);
                            }
                        }
                    }
                }
            }
        }

        public static void LongActionFinished(Comps.MultiplayerPlayer mP, string ActionType)
        {
            if (sendMyPosition == true) // CLIENT
            {
                using (Packet _packet = new Packet((int)ClientPackets.APPLYACTIONONPLAYER))
                {
                    _packet.Write(ActionType);
                    _packet.Write(mP.m_ID);
                    SendUDPData(_packet);
                }
            }
            if (iAmHost == true) // HOST
            {
                using (Packet _packet = new Packet((int)ServerPackets.APPLYACTIONONPLAYER))
                {
                    ServerSend.APPLYACTIONONPLAYER(0, ActionType, false, mP.m_ID);
                }
            }

            if (ActionType == "Bandage")
            {
                GameManager.GetInventoryComponent().RemoveGearFromInventory("GEAR_HeavyBandage", 1);
            }
            if (ActionType == "Sterilize")
            {
                GearItem antiseptic = GameManager.GetInventoryComponent().GetBestGearItemWithName("GEAR_BottleHydrogenPeroxide");
                antiseptic.m_LiquidItem.m_LiquidLiters = antiseptic.m_LiquidItem.m_LiquidLiters - 0.1f;

                if (antiseptic.m_LiquidItem.m_LiquidLiters < 0.1f)
                {
                    GameManager.GetInventoryComponent().DestroyGear(antiseptic.gameObject);
                }
            }
            else if (ActionType == "Revive")
            {
                GameManager.GetInventoryComponent().RemoveGearFromInventory("GEAR_MedicalSupplies_hangar", 1);
            }
            else if (ActionType == "Stim")
            {
                if (GameManager.m_PlayerManager != null && GameManager.m_PlayerManager.m_ItemInHands != null && GameManager.m_PlayerManager.m_ItemInHands.m_EmergencyStim != null)
                {
                    GameManager.GetPlayerManagerComponent().UseInventoryItem(GameManager.m_PlayerManager.m_ItemInHands);// Unequip GEAR_EmergencyStim
                }
                GameManager.GetInventoryComponent().RemoveGearFromInventory("GEAR_EmergencyStim", 1);
                EmergencyStimBeforeUse = null;
            }
            else if (ActionType == "Examine")
            {
                TryToCheckPlayer(mP.m_ID);
            }
        }

        public static void OtherPlayerApplyActionOnMe(string ActionType, int FromId)
        {
            if (ActionType == "Bandage")
            {
                BloodLoss bloodL = GameManager.GetBloodLossComponent();

                if (bloodL.m_ElapsedHoursList.Count > 0)
                {
                    bloodL.ApplyBandage(bloodL.m_ElapsedHoursList.Count - 1);
                }
                GameManager.GetPlayerManagerComponent().ApplyConditionOverTimeBuff(10, 0.3f, 0.3f);
            }
            if (ActionType == "Sterilize")
            {
                InfectionRisk Infect = GameManager.GetInfectionRiskComponent();

                if (Infect.m_ElapsedHoursList.Count > 0)
                {
                    Infect.TakeAntiseptic(Infect.m_ElapsedHoursList.Count - 1);
                }
            }
            else if (ActionType == "Revive")
            {
                SimRevive();
            }
            else if (ActionType == "Stim")
            {
                GearItem EmergencySt = GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_EmergencyStim", 1);
                EmergencySt.m_EmergencyStim.OnInject();
            }
        }

        public static void EmptyFn()
        {

        }

        public static void PlayersUpdateManager()
        {
            if (playersData.Count > 0)
            {
                for (int i = 0; i < MaxPlayers; i++)
                {
                    if(playersData[i] == null || players[i] == null)
                    {
                        continue;
                    }
                    GameObject plObj = players[i];
                    DataStr.MultiPlayerClientData plDat = playersData[i];
                    if (levelid == plDat.m_Levelid && level_guid == plDat.m_LevelGuid && !plDat.m_IsLoading) // Player locates on same scene with you.
                    {
                        plObj.SetActive(true);
                    } else
                    {
                        plObj.SetActive(false);
                    }
                }
            }
        }

        public static void ApplyDamageZones(GameObject p, Comps.MultiplayerPlayer mP)
        {
            if (p != null)
            {
                Transform root = p.transform.GetChild(3).GetChild(8);
                GameObject chest = root.GetChild(0).GetChild(0).GetChild(0).gameObject;
                GameObject arm_r1 = chest.transform.GetChild(0).GetChild(0).gameObject;
                GameObject arm_r2 = chest.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
                GameObject arm_l1 = chest.transform.GetChild(1).GetChild(0).gameObject;
                GameObject arm_l2 = chest.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
                GameObject head = chest.transform.GetChild(2).GetChild(0).gameObject;
                GameObject leg_l = root.GetChild(2).gameObject;
                GameObject leg_r = root.GetChild(1).gameObject;

                chest.AddComponent<Comps.PlayerBulletDamage>();
                arm_r1.AddComponent<Comps.PlayerBulletDamage>();
                arm_r2.AddComponent<Comps.PlayerBulletDamage>();
                arm_l1.AddComponent<Comps.PlayerBulletDamage>();
                arm_l2.AddComponent<Comps.PlayerBulletDamage>();
                head.AddComponent<Comps.PlayerBulletDamage>();
                leg_r.AddComponent<Comps.PlayerBulletDamage>();
                leg_l.AddComponent<Comps.PlayerBulletDamage>();
                // 0 - Head, 1 - Chest, 2 - Rigth Arm, 3 - Left Arm, 4 - Right Leg, 5 - Left Leg
                chest.GetComponent<Comps.PlayerBulletDamage>().SetLocaZone(chest, mP, 1);
                arm_r1.GetComponent<Comps.PlayerBulletDamage>().SetLocaZone(arm_r1, mP, 2);
                arm_r2.GetComponent<Comps.PlayerBulletDamage>().SetLocaZone(arm_r2, mP, 2);
                arm_l1.GetComponent<Comps.PlayerBulletDamage>().SetLocaZone(arm_l1, mP, 3);
                arm_l2.GetComponent<Comps.PlayerBulletDamage>().SetLocaZone(arm_l2, mP, 3);
                head.GetComponent<Comps.PlayerBulletDamage>().SetLocaZone(head, mP, 0);
                leg_r.GetComponent<Comps.PlayerBulletDamage>().SetLocaZone(leg_r, mP, 4);
                leg_l.GetComponent<Comps.PlayerBulletDamage>().SetLocaZone(leg_l, mP, 5);
            }
        }

        public static void AddLocalizedSprain(AfflictionBodyArea location, string causeID)
        {
            bool Legs = false;
            if (location == AfflictionBodyArea.LegLeft)
            {
                location = AfflictionBodyArea.FootLeft;
                Legs = true;
            }
            else if (location == AfflictionBodyArea.LegRight)
            {
                location = AfflictionBodyArea.FootRight;
                Legs = true;
            } else if (location == AfflictionBodyArea.ArmLeft)
            {
                location = AfflictionBodyArea.HandLeft;
            } else if (location == AfflictionBodyArea.ArmRight)
            {
                location = AfflictionBodyArea.HandRight;
            }

            if (Legs)
            {
                SprainedAnkle Spr = GameManager.GetSprainedAnkleComponent();

                Spr.m_CausesLocIDs.Add(causeID);
                Spr.m_Locations.Add((int)location);
                Spr.m_ElapsedHoursList.Add(0.0f);
                Spr.m_DurationHoursList.Add(UnityEngine.Random.Range(Spr.m_DurationHoursMin, Spr.m_DurationHoursMax));
                Spr.m_ElapsedRestList.Add(0.0f);
                Spr.m_SecondsSinceLastPainAudio = 0.0f;
                Spr.DoStumbleEffects();
                PlayerDamageEvent.SpawnDamageEvent(Spr.m_LocalizedDisplayName.m_LocalizationID, "GAMEPLAY_Affliction", "ico_injury_sprainedAnkle", InterfaceManager.m_FirstAidRedColor, true, InterfaceManager.m_Panel_HUD.m_DamageEventDisplaySeconds, InterfaceManager.m_Panel_HUD.m_DamageEventFadeOutSeconds);
                GameManager.GetPlayerVoiceComponent().Play(Spr.m_SprainedAnkleVO, Voice.Priority.Critical);
                GameAudioManager.PlaySound(Spr.m_SprainedAnkleSFX, Spr.gameObject);

                GameManager.GetLogComponent().AddAffliction(AfflictionType.SprainedAnkle, causeID);
                StatsManager.IncrementValue(StatID.Sprains_Ankle);
            } else {
                SprainedWrist Spr = GameManager.GetSprainedWristComponent();

                Spr.m_CausesLocIDs.Add(causeID);
                Spr.m_Locations.Add((int)location);
                Spr.m_ElapsedHoursList.Add(0.0f);
                Spr.m_DurationHoursList.Add(UnityEngine.Random.Range(Spr.m_DurationHoursMin, Spr.m_DurationHoursMax));
                Spr.m_ElapsedRestList.Add(0.0f);
                Spr.DoStumbleEffects();
                PlayerDamageEvent.SpawnDamageEvent(Spr.m_LocalizedDisplayName.m_LocalizationID, "GAMEPLAY_Affliction", "ico_injury_sprainedWrist", InterfaceManager.m_FirstAidRedColor, true, InterfaceManager.m_Panel_HUD.m_DamageEventDisplaySeconds, InterfaceManager.m_Panel_HUD.m_DamageEventFadeOutSeconds);
                GameManager.GetPlayerVoiceComponent().Play(Spr.m_SprainedWristVO, Voice.Priority.Critical);
                GameAudioManager.PlaySound(Spr.m_SprainedWristSFX, Spr.gameObject);
                GameManager.GetLogComponent().AddAffliction(AfflictionType.SprainedWrist, causeID);
                StatsManager.IncrementValue(StatID.Sprains_Wrist);
            }
            GameManager.GetSprainPainComponent().ApplyAffliction(location, causeID, AfflictionOptions.PlayFX);
        }

        public static void DamageByBullet(float damage, int from, int bodypart, bool Meele, string MeleeWeapon)
        {
            if (damage <= 0)
            {
                return;
            }

            DataStr.MeleeDescripter DmgInfo;

            if (Meele)
            {
                DmgInfo = GetMeelePlayerInfo(MeleeWeapon);
            } else {
                DmgInfo = new DataStr.MeleeDescripter();
                DmgInfo.m_PlayerDamage = damage;
                DmgInfo.m_AnimalDamage = 0;
                DmgInfo.m_BloodLoss = true;
                DmgInfo.m_ClothingTearing = true;
                DmgInfo.m_Pain = false;
            }

            string DamageCase = "Other player";
            if (playersData[from] != null)
            {
                DamageCase = playersData[from].m_Name;
                string Extra = " shoot you";
                if (Meele)
                {
                    Extra = " hit you";
                }
                DamageCase = DamageCase + Extra;
            }

            bool HasArmor = false;
            bool HasHelemet = false;

            if (bodypart == 1) // If Chest
            {
                HasArmor = GameManager.GetDamageProtection().HasBallisticVest();
                if (HasArmor)
                {
                    damage = damage / 10;
                }
            }

            if (bodypart == 0) // If Head
            {
                if (GetClothForSlot(ClothingRegion.Head, ClothingLayer.Mid) == "GEAR_CookingPot")
                {
                    damage = (20 * damage) / 100;
                    HasHelemet = true;
                }
            }



            GameManager.GetConditionComponent().AddHealth(-damage, DamageSource.BulletWound);
            AfflictionBodyArea BodyArea = AfflictionBodyArea.Head;

            switch (bodypart)
            {
                case 0:
                    BodyArea = AfflictionBodyArea.Head;
                    break;
                case 1:
                    BodyArea = AfflictionBodyArea.Chest;
                    break;
                case 2:
                    BodyArea = AfflictionBodyArea.ArmRight;
                    break;
                case 3:
                    BodyArea = AfflictionBodyArea.ArmLeft;
                    break;
                case 4:
                    BodyArea = AfflictionBodyArea.LegRight;
                    break;
                case 5:
                    BodyArea = AfflictionBodyArea.LegLeft;
                    break;
            }


            if (!HasArmor && !HasHelemet)
            {
                if (DmgInfo.m_BloodLoss)
                {
                    GameManager.GetBloodLossComponent().BloodLossStartOverrideArea(BodyArea, DamageCase, true, AfflictionOptions.PlayFX);
                }
                if (DmgInfo.m_Pain)
                {
                    if (BodyArea == AfflictionBodyArea.Head)
                    {
                        GameManager.GetHeadacheComponent().ApplyHeadache(0.5f, 5, 0.3f);
                    }
                    else if (BodyArea == AfflictionBodyArea.ArmRight
                        || BodyArea == AfflictionBodyArea.ArmLeft
                        || BodyArea == AfflictionBodyArea.LegLeft
                        || BodyArea == AfflictionBodyArea.LegRight)
                    {
                        AddLocalizedSprain(BodyArea, DamageCase);
                    }
                }

                ClothingRegion Region = ClothingRegion.Chest;

                switch (BodyArea)
                {
                    case AfflictionBodyArea.Head:
                        Region = ClothingRegion.Head;
                        break;
                    case AfflictionBodyArea.ArmLeft:
                        Region = ClothingRegion.Hands;
                        break;
                    case AfflictionBodyArea.ArmRight:
                        Region = ClothingRegion.Hands;
                        break;
                    case AfflictionBodyArea.Chest:
                        Region = ClothingRegion.Chest;
                        break;
                    case AfflictionBodyArea.LegLeft:
                        Region = ClothingRegion.Legs;
                        break;
                    case AfflictionBodyArea.LegRight:
                        Region = ClothingRegion.Legs;
                        break;
                }
                if (DmgInfo.m_ClothingTearing)
                {
                    var RNG = new System.Random(); int clothingRNG = RNG.Next(20, 40);
                    GameManager.GetPlayerManagerComponent().ApplyDamageToWornClothingRegion(Region, clothingRNG);
                }
            } else {
                var RNG = new System.Random(); int ribBroke = RNG.Next(0, 100);
                if (!Meele && ribBroke <= 5)
                {
                    GameManager.GetBrokenRibComponent().BrokenRibStart(DamageCase, true, false, true, false);
                }
                if (HasHelemet)
                {
                    GameManager.GetHeadacheComponent().ApplyHeadache(0.5f, 15, 15);
                }
            }

            GameManager.GetPlayerVoiceComponent().Play("PLAY_PLAYERDAMAGE", Voice.Priority.Critical, PlayerVoice.Options.None);

            Transform V3 = GameManager.GetPlayerTransform();
            GameObject Player = GameManager.GetPlayerObject();

            GameAudioManager.SetMaterialSwitch("Flesh", Player);
            int num = (int)AkSoundEngine.PostEvent(AK.EVENTS.PLAY_BULLETIMPACTS, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
            GameAudioManager.SetAudioSourceTransform(Player, V3);
        }

        public static void DoColisionForArrows()
        {
            Il2CppSystem.Collections.Generic.List<ArrowItem> ArrowsList = ArrowManager.m_ArrowItems;

            bool IsNeedToBeTrigger = true;

            for (int index = 0; index < ArrowsList.Count; ++index)
            {
                if (ArrowsList[index] != null && ArrowsList[index].InFlight(false))
                {
                    IsNeedToBeTrigger = false;
                    break;
                }
            }
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (players[i] != null && players[i].GetComponent<Comps.MultiplayerPlayer>() != null)
                {
                    List<GameObject> PlayerColiders = players[i].GetComponent<Comps.MultiplayerPlayer>().m_DamageColiders;

                    for (int index = 0; index < PlayerColiders.Count; ++index)
                    {
                        if (PlayerColiders[index] != null && PlayerColiders[index].GetComponent<BoxCollider>() != null)
                        {
                            PlayerColiders[index].GetComponent<BoxCollider>().isTrigger = IsNeedToBeTrigger;
                        }
                    }
                }
            }
        }

        public static void DisableOriginalAnimalSpawns(bool host = false)
        {
            if (GameManager.m_SpawnRegionManager == null)
            {
                return;
            }

            Il2CppSystem.Collections.Generic.List<SpawnRegion> Regions = GameManager.GetSpawnRegionManager().m_SpawnRegions;
            for (int i = 0; i < Regions.Count; i++)
            {
                if (Regions[i] != null)
                {
                    SpawnRegion region = Regions[i];
                    region.SetActive(false);
                }
            }
            if (host)
            {
                for (int index = 0; index < BaseAiManager.m_BaseAis.Count; ++index)
                {
                    GameObject animal = BaseAiManager.m_BaseAis[index].gameObject;
                    if (animal != null)
                    {
                        ObjectGuidManager.UnRegisterGuid(animal.GetComponent<ObjectGuid>().Get());
                        UnityEngine.Object.Destroy(animal);
                    }
                }
            }
        }
        public static void SimulateSpawnFromRegionSpawn(string GUID, SpawnRegion spRobj)
        {
            if (spRobj != null && spRobj.GetComponent<SpawnRegion>() != null)
            {
                SpawnRegion spR = spRobj.GetComponent<SpawnRegion>();
                Vector3 zero = Vector3.zero;
                Quaternion identity = Quaternion.identity;
                BaseAi newAnimal = null;

                if (spR.TryGetSpawnPositionAndRotation(ref zero, ref identity) == true)
                {
                    if (spR.PositionValidForSpawn(zero) == true)
                    {
                        newAnimal = spR.InstantiateSpawnInternal(WildlifeMode.Normal, zero, identity);
                    }
                }
                if (newAnimal != null)
                {
                    GameObject animal = newAnimal.gameObject;
                    if (animal.GetComponent<ObjectGuid>() == null)
                    {
                        animal.AddComponent<ObjectGuid>();
                    }
                    string animalGUID = ObjectGuidManager.GenerateNewGuidString();
                    animal.GetComponent<ObjectGuid>().Set(animalGUID);
                    if (animal.GetComponent<Comps.AnimalUpdates>() != null)
                    {
                        animal.GetComponent<Comps.AnimalUpdates>().m_RegionGUID = GUID;
                    } else {
                        Comps.AnimalUpdates au = animal.AddComponent<Comps.AnimalUpdates>();
                        au.m_RegionGUID = GUID;
                    }
                    if (spRobj.GetComponent<Comps.SpawnRegionSimple>())
                    {
                        spRobj.GetComponent<Comps.SpawnRegionSimple>().m_Spawned++;
                    }
                    //MelonLogger.Msg("Region " + GUID + " spawned animal " + animalGUID);
                    BaseAi Bai = animal.GetComponent<BaseAi>();
                    if (Bai != null)
                    {
                        Bai.TeleportToRandomWaypointAndPathfind();
                    }
                }
            }
        }

        public static void SetAnimationDataForAnimal(Animator AN, DataStr.AnimalAnimsSync obj)
        {
            int m_AnimParameter_TurnAngle = Animator.StringToHash("TurnAngle");
            int m_AnimParameter_TurnSpeed = Animator.StringToHash("TurnSpeed");
            int m_AnimParameter_Speed = Animator.StringToHash("Speed");
            int m_AnimParameter_Wounded = Animator.StringToHash("Wounded");
            int m_AnimParameter_Roll = Animator.StringToHash("Roll");
            int m_AnimParameter_Pitch = Animator.StringToHash("Pitch");
            int m_AnimParameter_TargetHeading = Animator.StringToHash("TargetHeading");
            int m_AnimParameter_TargetHeadingSmooth = Animator.StringToHash("TargetHeadingSmooth");
            int m_AnimParameter_TapMeter = Animator.StringToHash("TapMeter");
            int m_AnimParameter_AiState = Animator.StringToHash("AiState");
            int m_AnimParameter_Corpse = Animator.StringToHash("Corpse");
            int m_AnimParameter_Dead = Animator.StringToHash("Dead");
            int m_AnimParameter_DamageSide = Animator.StringToHash("DamageSide");
            int m_AnimParameter_DamageBodyPart = Animator.StringToHash("DamageBodyPart");
            int m_AnimParameter_AttackId = Animator.StringToHash("AttackId");

            AN.SetFloat(m_AnimParameter_TurnAngle, obj.AP_TurnAngle);
            AN.SetFloat(m_AnimParameter_TurnSpeed, obj.AP_TurnSpeed);
            AN.SetFloat(m_AnimParameter_Speed, obj.AP_Speed);
            AN.SetFloat(m_AnimParameter_Wounded, obj.AP_Wounded);
            AN.SetFloat(m_AnimParameter_Roll, obj.AP_Roll);
            AN.SetFloat(m_AnimParameter_Pitch, obj.AP_Pitch);
            AN.SetFloat(m_AnimParameter_TargetHeading, obj.AP_TargetHeading);
            AN.SetFloat(m_AnimParameter_TargetHeadingSmooth, obj.AP_TargetHeadingSmooth);
            AN.SetFloat(m_AnimParameter_TapMeter, obj.AP_TapMeter);
            AN.SetInteger(m_AnimParameter_AiState, obj.AP_AiState);
            AN.SetBool(m_AnimParameter_Corpse, obj.AP_Corpse);
            AN.SetBool(m_AnimParameter_Dead, obj.AP_Dead);
            AN.SetInteger(m_AnimParameter_DamageSide, obj.AP_DeadSide);
            AN.SetInteger(m_AnimParameter_DamageBodyPart, obj.AP_DamageBodyPart);
            AN.SetInteger(m_AnimParameter_AttackId, obj.AP_AttackId);
        }

        public static void LoadAniamlCorpsesForScene()
        {
            List<string> ToRemove = new List<string>();

            foreach (var item in Shared.AnimalsKilled)
            {
                int DespawnTime = item.Value.m_CreatedTime + 14400;
                if (DespawnTime < MinutesFromStartServer)
                {
                    ToRemove.Add(item.Key);
                } else {
                    if (item.Value.m_LevelGUID == level_guid)
                    {
                        ProcessAnimalCorpseSync(item.Value);
                    }
                }
            }
            foreach (var item in ToRemove)
            {
                Shared.AnimalsKilled.Remove(item);
            }
        }

        public static void FinallyPickupRabbit(int result)
        {
            RemovePleaseWait();
            if (result == 0)
            {
                HUDMessage.AddMessage("Can't pick this up");
            } else if (result == 1)
            {
                GameObject reference = GetGearItemObject("gear_rabbitcarcass");
                if (reference)
                {
                    GameObject obj = UnityEngine.Object.Instantiate<GameObject>(reference, GameManager.GetPlayerTransform().transform.position, GameManager.GetPlayerTransform().transform.rotation);
                    obj.name = reference.name;
                    if (obj && obj.GetComponent<GearItem>())
                    {
                        Comps.DropFakeOnLeave DFL = obj.AddComponent<Comps.DropFakeOnLeave>();
                        DFL.m_OldPossition = obj.transform.position;
                        DFL.m_OldRotation = obj.transform.rotation;

                        GameManager.GetPlayerManagerComponent().ProcessInspectablePickupItem(obj.GetComponent<GearItem>());
                    }
                }
            } else if (result == 2)
            {
                GameObject reference = GetGearItemObject("WILDLIFE_Rabbit");
                if (reference)
                {
                    GameObject obj = UnityEngine.Object.Instantiate<GameObject>(reference, GameManager.GetPlayerTransform().transform.position, GameManager.GetPlayerTransform().transform.rotation);
                    Comps.AnimalUpdates au = obj.GetComponent<Comps.AnimalUpdates>();
                    if (au == null)
                    {
                        obj.AddComponent<Comps.AnimalUpdates>();
                        au = obj.GetComponent<Comps.AnimalUpdates>();
                        au.m_Animal = obj;
                        au.m_PickedUp = true;
                    } else {
                        au.m_PickedUp = true;
                    }
                    if (obj.GetComponent<WildlifeItem>())
                    {
                        obj.GetComponent<WildlifeItem>().ProcessInteraction();
                    }
                }
            }
        }

        public static void OnHitRabbit(string GUID)
        {
            GameObject Animal = ObjectGuidManager.Lookup(GUID);
            if (Animal)
            {
                if (Animal.GetComponent<BaseAi>())
                {
                    Animal.GetComponent<BaseAi>().Stun(Animal.transform.position);
                }
            }
        }
        public static void OnRabbitRevived(Vector3 v3)
        {
            if (AnimalsController == true)
            {
                GameObject reference = GetGearItemObject("WILDLIFE_Rabbit");
                if (reference == null)
                    return;
                GameObject Rabbit = UnityEngine.Object.Instantiate<GameObject>(reference);
                Rabbit.transform.position = v3;
                BaseAi bai = Rabbit.GetComponent<BaseAi>();
                if (bai == null)
                    return;
                if (bai.CreateMoveAgent((Transform)null) && !bai.GetMoveAgent().Warp(v3, 1f, true, -1))
                {
                    bai.Despawn();
                } else {
                    bai.m_SpawnPos = v3;
                    Utils.SetGuidForGameObject(bai.gameObject, ObjectGuidManager.GenerateNewGuidString());
                }
                bai.FleeFrom(v3);
            }
        }

        public static void OnReleaseRabbit(int from)
        {
            if (AnimalsController == true)
            {
                GameObject reference = GetGearItemObject("WILDLIFE_Rabbit");
                if (reference == null)
                    return;
                GameObject Rabbit = UnityEngine.Object.Instantiate<GameObject>(reference);
                if (Rabbit == null)
                    return;
                Rabbit.name = reference.name;
                float dist = 5;

                Vector3 Camera = GameManager.GetMainCamera().transform.position;
                Vector3 CameraForward = GameManager.GetMainCamera().transform.forward;

                if (from != ClientUser.myId)
                {
                    if (players[from] != null)
                    {
                        Transform root = players[from].transform.GetChild(3).GetChild(8);
                        Transform chest = root.GetChild(0).GetChild(0).GetChild(0);
                        Transform head = chest.transform.GetChild(2).GetChild(0);
                        Camera = head.position;
                        CameraForward = head.forward;
                    }
                }

                RaycastHit raycastHit;
                if (!Physics.Raycast(((Camera + (CameraForward * dist)) + 1000f * Vector3.up), -Vector3.up, out raycastHit, float.PositiveInfinity, Utils.m_PhysicalCollisionLayerMask))
                    return;
                Rabbit.transform.position = raycastHit.point;
                BaseAi bai = Rabbit.GetComponent<BaseAi>();
                if (bai == null)
                    return;
                if (bai.CreateMoveAgent((Transform)null) && !bai.GetMoveAgent().Warp(raycastHit.point, 1f, true, -1))
                {
                    bai.Despawn();
                } else {
                    bai.m_SpawnPos = raycastHit.point;
                    Utils.SetGuidForGameObject(bai.gameObject, ObjectGuidManager.GenerateNewGuidString());
                }
                bai.FleeFrom(raycastHit.point);
            }
        }

        public static void AttemptToPickupRabbit(string GUID)
        {
            if (sendMyPosition == true)
            {
                DoPleaseWait("Trying to pickup little thing", "Please wait...");
                using (Packet _packet = new Packet((int)ClientPackets.PICKUPRABBIT))
                {
                    _packet.Write(GUID);
                    SendUDPData(_packet);
                }
            }
            else if (iAmHost == true)
            {
                FinallyPickupRabbit(Shared.PickUpRabbit(GUID));
            }
        }

        public static void SpawnQuartedMess(string GUID)
        {
            GameObject AnimalCorpse = ObjectGuidManager.Lookup(GUID);
            if (AnimalCorpse)
            {
                if (AnimalCorpse.GetComponent<Comps.AnimalCorpseObject>() != null && AnimalCorpse.GetComponent<BodyHarvest>() != null)
                {
                    BodyHarvest bh = AnimalCorpse.GetComponent<BodyHarvest>();
                    if (bh.CanSpawnCarcassSite())
                    {
                        bh.MaybeSpawnOrRefreshCarcassSite();
                        if (bh.m_CarcassSite != null)
                        {
                            bh.m_CarcassSite.SpawnQuarteringMess();
                        }
                    }
                    UnityEngine.Object.Destroy(AnimalCorpse);
                }
            }
        }

        public static void OpenBodyHarvest(BodyHarvest bh)
        {
            RemovePleaseWait();
            if (GameManager.GetPlayerManagerComponent().GetControlMode() == PlayerControlMode.InSnowShelter)
            {
                return;
            }
            InterfaceManager.m_Panel_BodyHarvest.Enable(true, bh, false, ComingFromScreenCategory.NotUI);
        }

        public static BodyHarvest GoingToHarvest = null;

        public static void RequestAnimalCorpseInteration(BodyHarvest bh)
        {
            if (bh.gameObject.GetComponent<ObjectGuid>())
            {
                string GUID = bh.gameObject.GetComponent<ObjectGuid>().Get();

                if (iAmHost == true)
                {
                    DataStr.AnimalKilled Animal;
                    if (Shared.AnimalsKilled.TryGetValue(GUID, out Animal))
                    {
                        bh.m_MeatAvailableKG = Animal.m_Meat;
                        bh.m_GutAvailableUnits = Animal.m_Guts;
                        bh.m_HideAvailableUnits = Animal.m_Hide;
                        OpenBodyHarvest(bh);
                    } else {
                        HUDMessage.AddMessage("Can't interact with this");
                    }
                } else if (sendMyPosition == true)
                {
                    DoPleaseWait("Downloading animal carcass", "Please wait...");
                    GoingToHarvest = bh;
                    using (Packet _packet = new Packet((int)ClientPackets.REQUESTANIMALCORPSE))
                    {
                        _packet.Write(GUID);
                        SendUDPData(_packet);
                    }
                }
            } else {
                HUDMessage.AddMessage("Can't interact with this");
            }
        }

        public static DataStr.AnimalCompactData GetCompactDataForAnimal(BaseAi AI, List<DataStr.AnimalArrow> Arrows)
        {
            if (AI != null && AI.gameObject != null && AI.gameObject.GetComponent<ObjectGuid>() != null)
            {
                GameObject animal = AI.gameObject;
                DataStr.AnimalCompactData Dat = new DataStr.AnimalCompactData();

                Dat.m_PrefabName = GetAnimalPrefabName(animal.name);
                Dat.m_GUID = AI.gameObject.GetComponent<ObjectGuid>().Get();
                Dat.m_Position = animal.transform.position;
                Dat.m_Rotation = animal.transform.rotation;
                Dat.m_LastSeen = MinutesFromStartServer;
                Dat.m_Health = AI.m_CurrentHP;
                Dat.m_Bleeding = AI.m_BleedingOut;
                Dat.m_TimeOfBleeding = MinutesFromStartServer + (int)AI.m_ElapsedBleedingOutMinutes;
                Dat.m_LastAiMode = (int)AI.GetAiMode();
                Dat.m_ArrowsCount = Arrows.Count;
                Dat.m_Arrows = Arrows;

                if (animal.GetComponent<Comps.AnimalUpdates>() != null)
                {
                    Dat.m_RegionGUID = animal.GetComponent<Comps.AnimalUpdates>().m_RegionGUID;
                } else {
                    Dat.m_RegionGUID = "";
                }
                return Dat;
            }
            return null;
        }

        public static int IntToTrigger(int i, BaseAi _AI)
        {
            if (i == 1) { return _AI.m_AnimParameter_Attack_Trigger; }
            else if (i == 2) { return _AI.m_AnimParameter_DamageImpact_Trigger; }
            else if (i == 3) { return _AI.m_AnimParameter_Flinch_Trigger; }
            else if (i == 4) { return _AI.m_AnimParameter_Howl_Trigger; }
            else if (i == 5) { return _AI.m_AnimParameter_MooseStomp_Trigger; }
            else if (i == 6) { return _AI.m_AnimParameter_RandomId_Trigger; }
            else if (i == 7) { return _AI.m_AnimParameter_Roar_Trigger; }
            else if (i == 8) { return _AI.m_AnimParameter_Stunned_Trigger; }
            else if (i == 9) { return _AI.m_AnimParameter_Trigger_PassingAttack; }
            else if (i == 10) { return _AI.m_AnimParameter_Trigger_PassingAttackNpc; }
            else if (i == 11) { return _AI.m_AnimParameter_Trigger_Spear_Exit_Fail; }
            else if (i == 12) { return _AI.m_AnimParameter_Trigger_Spear_Exit_Success; }
            else if (i == 13) { return _AI.m_AnimParameter_Trigger_Spear_Exit_Success_Death; }
            else if (i == 14) { return _AI.m_AnimParameter_Trigger_Spear_Struggle_Entry; }
            else if (i == 15) { return _AI.m_AnimParameter_Trigger_Timberwolf_Attack; }
            else { return 0; }
        }

        public static int GetClosestPlayerToAnimal(GameObject animal, int ReTakeChill, int CurrentController, Vector3 V3_Controller, int LevelID_Controller)
        {
            Vector3 V3 = animal.transform.position;
            BaseAi _AI = animal.GetComponent<BaseAi>();
            float MinimalDistance = 15;
            //float LastFoundDistance = float.PositiveInfinity;
            //int LastFoundID = -1;

            //Need input current controller data as last found, because we gonna skip current controller when search new one! This what was previous mistake.
            float LastFoundDistance = Vector3.Distance(V3, V3_Controller);
            int LastFoundID = CurrentController;

            if (_AI.GetAiMode() == AiMode.Dead || _AI.GetAiMode() == AiMode.Struggle || _AI.m_CurrentHP <= 0 || _AI.m_HasEnteredStruggleOnLastAttack == true || ReTakeChill > 0)
            {
                return CurrentController;
            }

            for (int i = 0; i < playersData.Count; ++i)
            {
                Vector3 otherPlayerV3;
                int otherPlayerLevel;

                if (i != CurrentController)
                {
                    if (playersData[i] != null)
                    {
                        if (iAmHost && i == 0)
                        {
                            otherPlayerV3 = GameManager.GetPlayerTransform().position;
                            otherPlayerLevel = levelid;
                        } else if (i == ClientUser.myId) {
                            otherPlayerV3 = GameManager.GetPlayerTransform().position;
                            otherPlayerLevel = levelid;
                        } else {
                            otherPlayerV3 = playersData[i].m_Position;
                            otherPlayerLevel = playersData[i].m_Levelid;
                        }

                        float distanceToCurrentController = Vector3.Distance(V3_Controller, otherPlayerV3);
                        float distanceToAnimal = Vector3.Distance(V3, otherPlayerV3);

                        if (distanceToCurrentController > MinimalDistance)
                        {
                            if (playersData[i].m_AnimState != "Knock" && otherPlayerLevel == LevelID_Controller && otherPlayerV3 != new Vector3(0, 0, 0))
                            {
                                if (distanceToAnimal < LastFoundDistance)
                                {
                                    LastFoundDistance = distanceToAnimal;
                                    LastFoundID = i;
                                    //MelonLogger.Msg("Pick nearest player " + LastFoundID + " " + otherPlayerV3.x + " " + otherPlayerV3.y + " " + otherPlayerV3.z);
                                }
                            }
                        }
                    }
                }
            }

            return LastFoundID;
        }

        public static void SendAnimalForValidPlayers(DataStr.AnimalCompactData data, DataStr.AnimalAnimsSync anim)
        {
            if (NoAnimalSync)
            {
                return;
            }

            for (int i = 0; i < playersData.Count; i++)
            {
                if (playersData[i] != null)
                {
                    if (i != ClientUser.myId)
                    {
                        if (playersData[i].m_Levelid == levelid && playersData[i].m_LevelGuid == level_guid)
                        {
                            if (iAmHost == true)
                            {
                                ServerSend.ANIMALTEST(0, data, anim, i);
                            }
                            if (sendMyPosition == true)
                            {
                                //MelonLogger.Msg("Client sends ANIMALTEST " + data.m_GUID + " for " + i+" Prefab "+data.m_PrefabName+" X "+data.m_Position.x + " Y " + data.m_Position.y+" Z " + data.m_Position.z);
                                using (Packet _packet = new Packet((int)ClientPackets.ANIMALTEST))
                                {
                                    _packet.Write(data);
                                    _packet.Write(anim);
                                    _packet.Write(i);
                                    SendUDPData(_packet);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static DataStr.AnimalAnimsSync GetAnimationDataFromAnimal(BaseAi _AI)
        {
            if (_AI)
            {
                Animator AN = _AI.m_Animator;
                if (AN)
                {
                    DataStr.AnimalAnimsSync sync = new DataStr.AnimalAnimsSync();
                    sync.AP_TurnAngle = AN.GetFloat(_AI.m_AnimParameter_TurnAngle);
                    sync.AP_TurnSpeed = AN.GetFloat(_AI.m_AnimParameter_TurnSpeed);
                    sync.AP_Speed = AN.GetFloat(_AI.m_AnimParameter_Speed);
                    sync.AP_Wounded = AN.GetFloat(_AI.m_AnimParameter_Wounded);
                    sync.AP_Roll = AN.GetFloat(_AI.m_AnimParameter_Roll);
                    sync.AP_Pitch = AN.GetFloat(_AI.m_AnimParameter_Pitch);
                    sync.AP_TargetHeading = AN.GetFloat(_AI.m_AnimParameter_TargetHeading);
                    sync.AP_TargetHeadingSmooth = AN.GetFloat(_AI.m_AnimParameter_TargetHeadingSmooth);
                    sync.AP_TapMeter = AN.GetFloat(_AI.m_AnimParameter_TapMeter);
                    sync.AP_AiState = AN.GetInteger(_AI.m_AnimParameter_AiState);
                    sync.AP_Corpse = AN.GetBool(_AI.m_AnimParameter_Corpse);
                    sync.AP_Dead = AN.GetBool(_AI.m_AnimParameter_Dead);
                    sync.AP_DeadSide = AN.GetInteger(_AI.m_AnimParameter_DamageSide);
                    sync.AP_DamageBodyPart = AN.GetInteger(_AI.m_AnimParameter_DamageBodyPart);
                    sync.AP_AttackId = AN.GetInteger(_AI.m_AnimParameter_AttackId);
                    sync.AP_Stunned = AN.GetBool(_AI.m_AnimParameter_Stunned);
                    return sync;
                } else {
                    return null;
                }
            }
            return null;
        }

        public static void RecreateAnimalToActor(GameObject animal, List<DataStr.AnimalArrow> Arrows)
        {
            string prefabName = GetAnimalPrefabName(animal.name);
            string RegionGUID = animal.GetComponent<Comps.AnimalUpdates>().m_RegionGUID;

            Vector3 v3 = animal.transform.position;
            Quaternion rot = animal.transform.rotation;

            string GUID = "";

            if (animal.GetComponent<ObjectGuid>() != null)
            {
                GUID = animal.GetComponent<ObjectGuid>().Get();
            }
            ObjectGuidManager.UnRegisterGuid(GUID);

            //MelonLogger.Msg(ConsoleColor.Cyan, "Starting recrating animal to actor " + GUID);

            SpawnAnimalActor(prefabName, v3, rot, GUID, RegionGUID, Arrows);
        }

        public static void AddFakeArrowToAnimal(Comps.AnimalUpdates AU, int index)
        {
            if (AU != null && AU.m_Arrows[index] != null)
            {
                DataStr.AnimalArrow Arrow = AU.m_Arrows[index];
                GameObject reference = GetGearItemObject("GEAR_Arrow");

                if (reference != null)
                {
                    GameObject obj = UnityEngine.Object.Instantiate<GameObject>(reference, Vector3.zero, Quaternion.identity);

                    ArrowItem ArrowIt = obj.GetComponent<ArrowItem>();
                    if (ArrowIt != null)
                    {
                        ArrowIt.ParentToObject(AU.m_Animal.transform.Find(Arrow.m_LocaName));
                        obj.transform.localPosition = Arrow.m_Position;
                        obj.transform.localEulerAngles = Arrow.m_Angle;
                        Utils.SetIsKinematic(ArrowIt.m_Rigidbody, true);
                    }
                    UnityEngine.Object.Destroy(obj.GetComponent<ArrowItem>());
                    UnityEngine.Object.Destroy(obj.GetComponent<GearItem>());
                }
            }
        }
        public static void AddFakeArrowToAnimal(Comps.AnimalActor AU, int index)
        {
            if (AU != null && AU.m_Arrows[index] != null)
            {
                DataStr.AnimalArrow Arrow = AU.m_Arrows[index];
                GameObject reference = GetGearItemObject("GEAR_Arrow");

                if (reference != null)
                {
                    GameObject obj = UnityEngine.Object.Instantiate<GameObject>(reference, Vector3.zero, Quaternion.identity);

                    ArrowItem ArrowIt = obj.GetComponent<ArrowItem>();
                    if (ArrowIt != null)
                    {
                        ArrowIt.ParentToObject(AU.m_Animal.transform.Find(Arrow.m_LocaName));
                        obj.transform.localPosition = Arrow.m_Position;
                        obj.transform.localEulerAngles = Arrow.m_Angle;
                        Utils.SetIsKinematic(ArrowIt.m_Rigidbody, true);
                    }
                    UnityEngine.Object.Destroy(obj.GetComponent<ArrowItem>());
                    UnityEngine.Object.Destroy(obj.GetComponent<GearItem>());
                }
            }
        }

        public static void SwitchToAnimalController()
        {
            Dictionary<string, int> AnimalsInRegions = new Dictionary<string, int>();
            if (ActorsList.Count > 0)
            {
                foreach (var item in ActorsList)
                {
                    item.Value.m_ClientController = ClientUser.myId;
                    string RegionGUID = item.Value.m_RegionGUID;
                    if (RegionGUID != "")
                    {
                        int val;
                        if (!AnimalsInRegions.TryGetValue(RegionGUID, out val))
                        {
                            AnimalsInRegions.Add(RegionGUID, 1);
                        } else {
                            AnimalsInRegions.Remove(RegionGUID);
                            AnimalsInRegions.Add(RegionGUID, val + 1);
                        }
                    }
                }
            }
            if (BaseAiManager.m_BaseAis.Count > 0)
            {
                foreach (var item in BaseAiManager.m_BaseAis)
                {
                    if (item != null && item.gameObject != null)
                    {
                        if (item.gameObject.GetComponent<Comps.AnimalUpdates>())
                        {
                            string RegionGUID = item.gameObject.GetComponent<Comps.AnimalUpdates>().m_RegionGUID;
                            if (RegionGUID != "")
                            {
                                int val;
                                if (!AnimalsInRegions.TryGetValue(RegionGUID, out val))
                                {
                                    AnimalsInRegions.Add(RegionGUID, 1);
                                } else {
                                    AnimalsInRegions.Remove(RegionGUID);
                                    AnimalsInRegions.Add(RegionGUID, val + 1);
                                }
                            }
                        }
                    }
                }
            }
            if (AnimalCorplesList.Count > 0)
            {
                foreach (var item in AnimalCorplesList)
                {
                    string RegionGUID = item.Value.m_RegionGUID;
                    if (RegionGUID != "")
                    {
                        int val;
                        if (!AnimalsInRegions.TryGetValue(RegionGUID, out val))
                        {
                            AnimalsInRegions.Add(RegionGUID, 1);
                        } else {
                            AnimalsInRegions.Remove(RegionGUID);
                            AnimalsInRegions.Add(RegionGUID, val + 1);
                        }
                    }
                }
            }
            // Final
            if (AnimalsInRegions.Count > 0)
            {
                foreach (var item in AnimalsInRegions)
                {
                    GameObject Region = ObjectGuidManager.Lookup(item.Key);
                    if (Region && Region.GetComponent<Comps.SpawnRegionSimple>() != null)
                    {
                        Region.GetComponent<Comps.SpawnRegionSimple>().m_Spawned = item.Value;
                    }
                }
            }
        }

        public static void RecreateAnimalToSyncable(GameObject animal, string RegionGUID, float health, List<DataStr.AnimalArrow> Arrows)
        {

            string GUID = "";
            if (animal.GetComponent<ObjectGuid>() != null)
            {
                GUID = animal.GetComponent<ObjectGuid>().Get();
            }
            ObjectGuidManager.UnRegisterGuid(GUID);

            string prefabName = GetAnimalPrefabName(animal.name);
            Vector3 v3 = animal.transform.position;
            Quaternion rot = animal.transform.rotation;
            //MelonLogger.Msg(ConsoleColor.Cyan, "Starting recrating animal to syncable " + GUID+" with region GUID "+ RegionGUID);

            if (!AiUtils.IsNavmeshPosValid(v3, 0.5f, 1f))
            {
                return;
            }

            BaseAi bai;
            GameObject newAnimal;
            GameObject reference = GetGearItemObject(prefabName);

            if (reference == null)
            {
                MelonLogger.Msg(ConsoleColor.Red, "Can't re-create animal with prefab name " + prefabName);
                return;
            }

            newAnimal = UnityEngine.Object.Instantiate(reference, v3, rot);
            bai = newAnimal.GetComponent<BaseAi>();
            if (newAnimal.GetComponent<ObjectGuid>() == null)
            {
                newAnimal.AddComponent<ObjectGuid>();
                newAnimal.GetComponent<ObjectGuid>().Set(GUID);
            } else {
                newAnimal.GetComponent<ObjectGuid>().Set(GUID);
            }
            BaseAiManager.CreateMoveAgent(bai.transform, bai, v3);


            Comps.AnimalUpdates au = newAnimal.GetComponent<Comps.AnimalUpdates>();

            if (au == null)
            {
                newAnimal.AddComponent<Comps.AnimalUpdates>();

                au = newAnimal.GetComponent<Comps.AnimalUpdates>();
            }
            au.m_ToGo = v3;
            if (health != -1)
            {
                au.m_Hp = health;
                bai.m_CurrentHP = health;
            }
            au.m_Animal = newAnimal;
            au.m_MyControlled = true;
            au.m_RegionGUID = RegionGUID;
            au.m_Arrows = Arrows;
            au.ProcessArrows();
            newAnimal.transform.position = v3;
        }

        public static Dictionary<string, Comps.AnimalActor> ActorsList = new Dictionary<string, Comps.AnimalActor>();
        public static Dictionary<string, Comps.AnimalCorpseObject> AnimalCorplesList = new Dictionary<string, Comps.AnimalCorpseObject>();

        public static int CalculateTargetPopulation(SpawnRegion m_Region)
        {
            //bool IsDay = GameManager.GetTimeOfDayComponent().IsDay();

            //if (!IsDay)
            //{
            //    return m_Region.GetMaxSimultaneousSpawnsNight();
            //}else{
            //    return m_Region.GetMaxSimultaneousSpawnsDay();
            //}

            return 1;
        }

        public static bool SpawnInBlizzard(SpawnRegion m_Region)
        {
            bool Blizzard = GameManager.GetWeatherComponent().IsBlizzard();

            if (Blizzard)
            {
                return m_Region.m_CanSpawnInBlizzard;
            } else {
                return true;
            }
        }

        public static Dictionary<string, int> BannedSpawnRegions = new Dictionary<string, int>();

        public static void ExitHarvesting()
        {
            Panel_BodyHarvest PBH = InterfaceManager.m_Panel_BodyHarvest;

            if (PBH.m_CurrentHarvestAction != Panel_BodyHarvest.HarvestAction.None)
            {
                PBH.OnCancel();
            } else {
                PBH.OnBack();
            }
        }

        public static GearItem GetGearItemPrefab(string name) => Resources.Load(name).Cast<GameObject>().GetComponent<GearItem>();
        public static GameObject GetGearItemObject(string name)
        {
            if (Resources.Load(name) == null)
            {
                return null;
            }
            if (Resources.Load(name).Cast<GameObject>() != null)
            {
                return Resources.Load(name).Cast<GameObject>();
            }
            return null;
        }

        //PACKETS

        private static void DoneDisconnect(IAsyncResult _result)
        {
            if (ClientUser.udp != null && ClientUser.udp.socket != null)
            {
                ClientUser.udp.endPoint = null;
                ClientUser.udp.socket.Client.Close();
            }
        }

        public static void Disconnect()
        {
            if (iAmHost == true || sendMyPosition == true)
            {
                if (SteamServerWorks != "")
                {
                    if (SteamConnect.CanUseSteam == true)
                    {
                        MelonLogger.Msg("[Steamworks.NET] Disconnecting...");
                        SteamConnect.Main.Disconnect(SteamServerWorks);
                        sendMyPosition = false;
                        ClientUser.myId = 0;
                        return;
                    }
                }
                if (ClientUser.udp == null)
                {
                    MelonLogger.Msg("UDP is already closed.");
                    return;
                }
                if (ClientUser.udp.socket == null)
                {
                    MelonLogger.Msg("Socket of udp is closed.");
                    return;
                }
                if (ClientUser.udp != null && ClientUser.udp.socket != null)
                {
                    if (ClientUser.udp.socket.Client != null)
                    {
                        ClientUser.udp.socket.Client.BeginDisconnect(true, DoneDisconnect, null);
                    }
                }
                HUDMessage.AddMessage("DISCONNECTED FROM SERVER");
                MelonLogger.Msg("[UDP] Disconnected from server");
                sendMyPosition = false;
                ClientUser.myId = 0;
            }
        }

        public static void SendUDPData(Packet _packet, bool IgnoreQuit = false)
        {
            if (SteamConnect.CanUseSteam == true && ConnectedSteamWorks == true)
            {
                _packet.WriteLength();
                _packet.InsertClientInfo(ClientUser.myId, ClientUser.SubNetworkClientGUID);
                SteamConnect.Main.SendUDPData(_packet, SteamServerWorks);
            } else {
                ClientUser.udp.SendData(_packet, IgnoreQuit);
            }
        }

        public static bool IsEquippable(GearItem gi)
        {
            if (gi.m_MatchesItem || gi.m_KeroseneLampItem || gi.m_FlareItem || gi.m_NoiseMakerItem || gi.m_TorchItem || gi.m_EmergencyStim || gi.m_FlashlightItem || gi.m_FirstPersonItem)
            {
                return true;
            }
            return false;
        }

        public static void GiveRecivedItem(DataStr.GearItemDataPacket gearData)
        {
            //MelonLogger.Msg(ConsoleColor.Blue, "Got gear with name ["+ gearData.m_GearName + "] DATA: "+ gearData.m_DataProxy);

            GearItemSaveDataProxy itemSaveDataProxy = Utils.DeserializeObject<GearItemSaveDataProxy>(gearData.m_DataProxy);
            string dummy_name = gearData.m_GearName;
            string give_name = "";

            string say = "";
            bool watermode = false;
            LiquidQuality water_q = LiquidQuality.Potable;

            if (dummy_name.Contains("(Clone)")) //If it has ugly (Clone), cutting it.
            {
                int L = dummy_name.Length - 7;
                give_name = dummy_name.Remove(L, 7);
            } else {
                give_name = dummy_name;
            }

            if (give_name == "GEAR_WaterSupplyPotable")
            {
                watermode = true;
            }
            if (give_name == "GEAR_WaterSupplyNotPotable")
            {
                watermode = true;
                water_q = LiquidQuality.NonPotable;
            }

            if (watermode == false) // If this is water we not give new item, but just add to supply.
            {
                GearItem closestMatchStackable = GameManager.GetInventoryComponent().GetClosestMatchStackable(give_name, itemSaveDataProxy.m_NormalizedCondition);
                if (closestMatchStackable == null)
                {
                    MelonLogger.Msg("Not stack for item " + give_name + ", creating new item");
                    GearItem new_gear = GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory(give_name);
                    DisableObjectForXPMode.RemoveDisabler(new_gear.gameObject);
                    GearItemSaveDataProxy Proxy = Utils.DeserializeObject<GearItemSaveDataProxy>(gearData.m_DataProxy);
                    new_gear.m_LastUpdatedTODHours = 0;
                    new_gear.Deserialize(gearData.m_DataProxy);
                    say = new_gear.m_LocalizedDisplayName.Text();
                    if (GameManager.GetPlayerManagerComponent().m_ItemInHands == null)
                    {
                        if (IsEquippable(new_gear) == true)
                        {
                            GameManager.GetPlayerManagerComponent().EquipItem(new_gear, false);
                        }
                    }

                    PatchBookReadTime(new_gear.GetComponent<GearItem>());

                } else {
                    closestMatchStackable.m_StackableItem.m_Units += 1;
                    MelonLogger.Msg("Found stack for " + give_name + ", now units " + closestMatchStackable.m_StackableItem.m_Units);
                    if (GameManager.GetPlayerManagerComponent().m_ItemInHands == null)
                    {
                        if (IsEquippable(closestMatchStackable) == true)
                        {
                            GameManager.GetPlayerManagerComponent().EquipItem(closestMatchStackable, false);
                        }
                    }
                }
            } else {
                string bottlename = Resources.Load(give_name).Cast<GameObject>().GetComponent<GearItem>().m_LocalizedDisplayName.Text();

                MelonLogger.Msg("Got water " + gearData.m_Water);
                if (gearData.m_Water == 0.5f)
                {
                    say = "half liter of " + bottlename;
                } else {
                    say = gearData.m_Water + " of " + bottlename;
                }
                GameManager.GetInventoryComponent().AddToWaterSupply(gearData.m_Water, water_q);
            }

            HUDMessage.AddMessage("Other player gave you " + say + ".");

            MelonLogger.Msg("Other player gave you item " + give_name);
        }

        public static void SimRevive()
        {
            InterfaceManager.m_Panel_Log.Enable(false);
            SetRevivedStats();
            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Normal);
        }

        public static void DeleteAnimal(string _guid)
        {
            GameObject animal = ObjectGuidManager.Lookup(_guid);

            if (animal != null)
            {
                UnityEngine.Object.Destroy(animal);
            }
        }

        public static void DoAnimalDamage(string guid, float damage)
        {
            MelonLogger.Msg("Other player damage animal " + guid + " on " + damage);
            GameObject animal = ObjectGuidManager.Lookup(guid);
            if (animal)
            {
                BaseAi bai = animal.GetComponent<BaseAi>();
                if (bai)
                {
                    bai.ApplyDamage(damage, DamageSource.Player, "");
                    MelonLogger.Msg("After apply damage current HP Is " + bai.m_CurrentHP);
                }
            }
        }

        public static void SetAnimationParams(GameObject animal, DataStr.AnimalAnimsSync obj)
        {
            Comps.AnimalActor au = animal.GetComponent<Comps.AnimalActor>();
            if (!au)
            {
                return;
            }

            au.AP_TurnAngle = obj.AP_TurnAngle;
            au.AP_TurnSpeed = obj.AP_TurnSpeed;
            au.AP_Speed = obj.AP_Speed;
            au.AP_Wounded = obj.AP_Wounded;
            au.AP_Roll = obj.AP_Roll;
            au.AP_Pitch = obj.AP_Pitch;
            au.AP_TargetHeading = obj.AP_TargetHeading;
            au.AP_TargetHeadingSmooth = obj.AP_TargetHeadingSmooth;
            au.AP_TapMeter = obj.AP_TapMeter;
            au.AP_AiState = obj.AP_AiState;
            au.AP_Corpse = obj.AP_Corpse;
            au.AP_Dead = obj.AP_Dead;
            au.AP_DeadSide = obj.AP_DeadSide;
            au.AP_DamageBodyPart = obj.AP_DamageBodyPart;
            au.AP_AttackId = obj.AP_AttackId;
            au.AP_Stunned = obj.AP_Stunned;
        }

        public static void SetAnimalPosition(GameObject animal, Vector3 xyz, Quaternion xyzw)
        {
            Comps.AnimalActor au = animal.GetComponent<Comps.AnimalActor>();
            if (!au)
            {
                return;
            }
            au.m_ToGo = xyz;
            au.m_ToRotate = xyzw;
        }

        public static void DoAnimalSync(DataStr.AnimalCompactData dat, DataStr.AnimalAnimsSync anim)
        {
            if (NoAnimalSync)
            {
                return;
            }
            GameObject animal = ObjectGuidManager.Lookup(dat.m_GUID);
            if (animal)
            {
                if (animal.GetComponent<Comps.AnimalActor>() != null)
                {
                    Comps.AnimalActor Actor = animal.GetComponent<Comps.AnimalActor>();
                    if (Actor.m_MarkToDestroy == false)
                    {
                        Actor.NoResponce = 5;
                        Actor.m_Hp = dat.m_Health;
                        Actor.m_RegionGUID = dat.m_RegionGUID;
                        Actor.m_ClientController = dat.m_LastController;

                        SetAnimationParams(animal, anim);
                        SetAnimalPosition(animal, dat.m_Position, dat.m_Rotation);

                        Actor.m_NextMode = (AiMode)dat.m_LastAiMode;
                        Actor.SetAiMode();
                        int OldAmount = Actor.m_Arrows.Count;
                        if (OldAmount < dat.m_Arrows.Count)
                        {
                            Actor.m_Arrows = dat.m_Arrows;
                            for (int i = OldAmount; i < Actor.m_Arrows.Count; i++)
                            {
                                AddFakeArrowToAnimal(Actor, i);
                            }
                        }
                    }
                }
            } else {
                animal = SpawnAnimalActor(dat.m_PrefabName, dat.m_Position, dat.m_Rotation, dat.m_GUID, dat.m_RegionGUID, dat.m_Arrows);
                if (animal && animal.GetComponent<Comps.AnimalActor>())
                {
                    Comps.AnimalActor Actor = animal.GetComponent<Comps.AnimalActor>();
                    Actor.NoResponce = 5;
                    Actor.m_Hp = dat.m_Health;
                    Actor.m_RegionGUID = dat.m_RegionGUID;
                    Actor.m_ClientController = dat.m_LastController;

                    SetAnimationParams(animal, anim);
                    SetAnimalPosition(animal, dat.m_Position, dat.m_Rotation);

                    Actor.m_NextMode = (AiMode)dat.m_LastAiMode;
                    Actor.SetAiMode();
                }
            }
        }

        public static void SetAnimalTriggers(DataStr.AnimalTrigger obj)
        {
            string _guid = obj.m_Guid;
            int trigg = obj.m_Trigger;

            GameObject animal = ObjectGuidManager.Lookup(_guid);

            if (animal != null)
            {
                if (animal.GetComponent<Comps.AnimalActor>() != null)
                {
                    Comps.AnimalActor _AI = animal.GetComponent<Comps.AnimalActor>();
                    Animator AN = _AI.m_Animator;

                    if (trigg == Animator.StringToHash("StruggleEnd"))
                    {
                        //MelonLogger.Msg("Going to end struggle, so reseting StruggleStart");
                        AN.ResetTrigger(Animator.StringToHash("StruggleStart"));
                    }
                    else if (trigg == Animator.StringToHash("StruggleStart"))
                    {
                        //MelonLogger.Msg("Going to start struggle, so reseting StruggleEnd");
                        AN.ResetTrigger(Animator.StringToHash("StruggleEnd"));
                    }
                    AN.ResetTrigger(trigg);
                    //MelonLogger.Msg("Set trigger for animal " + _guid + " trigger hash " + trigg);
                }
            }
        }

        public static void DoShootFX(Vector3 pos)
        {
            GameObject LightFX = new GameObject();
            LightFX.transform.position = pos;
            Light LightComp = LightFX.AddComponent<Light>();
            LightComp.type = LightType.Point;
            LightComp.range = 5;
            LightComp.intensity = 5;
            LightComp.color = new Color(1, 0.5623099f, 0.3268814f, 1);
            UnityEngine.Object.Destroy(LightFX, 0.1f);
        }

        public static void DoShootSync(DataStr.ShootSync shoot, int from)
        {
            if (shoot.m_sceneguid != level_guid)
            {
                return;
            }
            if (shoot.m_projectilename == "GEAR_FlareGunAmmoSingle")
            {
                FlareGunRoundItem.SpawnAndFire(GetGearItemObject("GEAR_FlareGunAmmoSingle"), shoot.m_position, shoot.m_rotation);
                if (shoot.m_lookat)
                {
                    GameManager.GetPlayerAnimationComponent().LookAt(shoot.m_position);
                }
            }
            else if (shoot.m_projectilename == "GEAR_Arrow")
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("GEAR_Arrow"), shoot.m_position, shoot.m_rotation);
                GearItem component = gameObject.GetComponent<GearItem>();
                gameObject.name = "GEAR_Arrow";
                gameObject.transform.parent = (Transform)null;
                component.m_InPlayerInventory = false;
                component.m_StackableItem.m_Units = 1;
                component.m_CurrentHP = 100;
                component.m_ArrowItem.SetPlacementHelperEnabled(true);

                gameObject.AddComponent<Comps.DestoryArrowOnHit>();

                Utils.ChangeLayersForGearItem(gameObject, 17);
                component.m_ArrowItem.Fire();
            }
            else if (shoot.m_projectilename == "GEAR_Stone")
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("GEAR_Stone"), shoot.m_position, shoot.m_rotation);
                float throwForce = GameManager.m_PlayerManager.m_ThrowForce;
                float num = GameManager.m_PlayerManager.m_ThrowTorque;
                GearItem component = gameObject.GetComponent<GearItem>();
                component.m_StoneItem.PrepareForThrow();
                component.m_StoneItem.SetThrown(true);
                throwForce = component.m_StoneItem.m_ThrowForce;
                num = component.m_StoneItem.m_ThrowForce;

                Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
                component2.isKinematic = false;
                component2.velocity = shoot.m_camera_forward * throwForce;

                Vector3 vector3 = shoot.m_camera_right + UnityEngine.Random.Range(-0.2f, 0.2f) * shoot.m_camera_up;
                vector3.Normalize();
                component2.angularVelocity = vector3 * num;
                component2.angularDrag = 0.0f;
                component2.drag = 0.0f;
                Comps.DestoryStoneOnStop StoneComp = gameObject.AddComponent<Comps.DestoryStoneOnStop>();
                StoneComp.m_Obj = gameObject;
                StoneComp.m_RB = component2;
                StoneComp.m_Gear = component;
            }
            else if (shoot.m_projectilename == "GEAR_NoiseMaker")
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("GEAR_NoiseMaker"), shoot.m_position, shoot.m_rotation);
                float throwForce = GameManager.m_PlayerManager.m_ThrowForce;
                float num = GameManager.m_PlayerManager.m_ThrowTorque;
                GearItem component = gameObject.GetComponent<GearItem>();
                if (component)
                {
                    NoiseMakerItem component1 = component.m_NoiseMakerItem;

                    if (component1)
                    {
                        component1.m_CanThrow = false;
                        component1.Ignite();
                        component1.m_PlayerDamageInflictionInRadius = 15;
                        component1.m_PlayerDamageRadius = component1.m_AIDamageRadius;
                        if (shoot.m_skill == 0)
                        {
                            MelonLogger.Msg("GEAR_NoiseMaker Throw sync");
                            component1.PrepareForThrow();
                            component1.m_Thrown = true;
                            throwForce = component1.m_ThrowForce;
                            num = component1.m_ThrowTorque;
                            Rigidbody component2 = component.GetComponent<Rigidbody>();
                            if (component2 == null)
                            {
                                return;
                            }
                            Utils.SetIsKinematic(component2, false);
                            component2.velocity = shoot.m_camera_forward * throwForce;
                            Rigidbody rigidbody = component2;
                            Vector3 vector3 = shoot.m_camera_right + UnityEngine.Random.Range(-0.2f, 0.2f) * shoot.m_camera_up;
                            vector3.Normalize();
                            component2.angularVelocity = vector3 * num;
                            component2.angularDrag = 0.0f;
                            component2.drag = 0.0f;
                        } else {
                            MelonLogger.Msg("GEAR_NoiseMaker in hand detonation sync");
                            component1.PerformDetonation((ContactPoint[])null);
                        }
                    } else {
                        MelonLogger.Msg(ConsoleColor.Red, "GEAR_NoiseMaker NoiseMakerItem!!!");
                    }
                } else {
                    MelonLogger.Msg(ConsoleColor.Red, "GEAR_NoiseMaker Has not GearItem!!!");
                }
            } else if (shoot.m_projectilename == "Melee")
            {
                DoMeleeHitFX(shoot.m_position, shoot.m_camera_forward, shoot.m_rotation, players[from]);
            } else {
                //MelonLogger.Msg("Got remote shoot event " + shoot.m_projectilename);

                GameObject gameObject = null;
                GearItem itemInHands = null;

                if (shoot.m_projectilename == "PistolBullet")
                {
                    itemInHands = GetGearItemPrefab("GEAR_Rifle");
                    gameObject = UnityEngine.Object.Instantiate<GameObject>(PistolBulletPrefab, shoot.m_position, shoot.m_rotation);
                    gameObject.AddComponent<Comps.ClientProjectile>();
                    Comps.ClientProjectile ClientP = gameObject.GetComponent<Comps.ClientProjectile>();
                    ClientP.m_ClientID = from;
                    PlayMultiplayer3dAduio("PLAY_RIFLE_SHOOT_3D", from);
                    DoShootFX(shoot.m_position);
                    if (players[from] != null && players[from].GetComponent<Comps.MultiplayerPlayerAnimator>() != null)
                    {
                        string shootStrhing = "RifleShoot";
                        if (playersData[from] != null && playersData[from].m_AnimState == "Ctrl")
                        {
                            shootStrhing = "RifleShoot_Sit";
                        }
                        players[from].GetComponent<Comps.MultiplayerPlayerAnimator >().m_PreAnimStateHands = shootStrhing;
                    }
                }
                else if (shoot.m_projectilename == "RevolverBullet")
                {
                    itemInHands = GetGearItemPrefab("GEAR_Revolver");
                    gameObject = UnityEngine.Object.Instantiate<GameObject>(RevolverBulletPrefab, shoot.m_position, shoot.m_rotation);
                    gameObject.AddComponent<Comps.ClientProjectile>();
                    Comps.ClientProjectile ClientP = gameObject.GetComponent<Comps.ClientProjectile>();
                    ClientP.m_ClientID = from;
                    PlayMultiplayer3dAduio("PLAY_REVOLVER_SHOOT_3D", from);
                    DoShootFX(shoot.m_position);
                }

                if (itemInHands == null || gameObject == null)
                {
                    return;
                }

                gameObject.hideFlags = HideFlags.HideInHierarchy;
                vp_Bullet component1 = gameObject.GetComponent<vp_Bullet>();

                if ((bool)(UnityEngine.Object)itemInHands)
                {
                    GunItem component2 = itemInHands.GetComponent<GunItem>();
                    if ((bool)(UnityEngine.Object)component2)
                    {
                        component1.m_GunType = component2.m_GunType;
                        component1.MinDistanceForAimAssist = component2.m_MinDistanceForAimAssist;
                        //component1.Damage = component2.m_DamageHP;
                        component1.Damage = 0;
                        component1.Accuracy = component1.m_GunType != GunType.Rifle ? component2.m_AccuracyRange : shoot.m_skill;
                        component1.m_ImpactAudio = component2.m_ImpactAudio;
                    }
                }
                //GameObject emitterFromGameObject = GameAudioManager.GetSoundEmitterFromGameObject(gameObject);
                //AkSoundEngine.PostEvent(AK.EVENTS.PLAY_RIFLEFIRE, emitterFromGameObject);
                //GameAudioManager.SetAudioSourceTransform(emitterFromGameObject, emitterFromGameObject.transform);

                if (players[from] != null)
                {
                    GameAudioManager.NotifyAiAudioEvent(players[from], players[from].transform.position, GameAudioAiEvent.Gunshot);
                }
            }
        }

        public static void DoSyncContainer(DataStr.ContainerOpenSync sync)
        {
            Il2CppSystem.Collections.Generic.List<Container> Boxes = ContainerManager.m_Containers;

            for (int i = 0; i < Boxes.Count; i++)
            {
                Container box = Boxes[i];
                if (box != null && box.GetComponent<Comps.ContainersSync>() != null && box.GetComponent<Comps.ContainersSync>().m_Guid == sync.m_Guid)
                {
                    if (sync.m_State == true)
                    {
                        box.GetComponent<Comps.ContainersSync>().Open();
                    }
                    else
                    {
                        box.GetComponent<Comps.ContainersSync>().Close();
                    }
                    break;
                }
            }
        }

        private static void MainThread()
        {

        }

        private static Vector3 previoustickpos;
        private static Quaternion previoustickrot;

        public static Vector3 SnappedPosition(Vector3 pointToSnap, Vector3 blockCenterPosition)
        {
            Vector3 relativePosition = pointToSnap - blockCenterPosition;

            if (relativePosition.x == 0.5f) return blockCenterPosition + Vector3.right;
            else if (relativePosition.x == -0.5f) return blockCenterPosition + Vector3.left;
            else if (relativePosition.y == 0.5f) return blockCenterPosition + Vector3.up;
            else if (relativePosition.y == -0.5f) return blockCenterPosition + Vector3.down;
            else if (relativePosition.z == 0.5f) return blockCenterPosition + Vector3.forward;
            else if (relativePosition.z == -0.5f) return blockCenterPosition + Vector3.back;
            else return Vector3.zero;  // error (should not occur)
        }

        //GameManager.GetHungerComponent().RemoveReserveCalories(15f); -15 calories

        public static void MaybeLeaveFootPrint(Vector3 footPos, GameObject ply, bool bothFeet = false, float forceDeepFrac = 0.0f, bool leftfoot = false)
        {

            bool m_IgnoreNextFootStep = false;
            bool m_LeftFootStep = leftfoot;
            float m_FootPrintSideOffset = 0;

            if (GameManager.GetWeatherComponent().IsIndoorEnvironment())
                return;
            if (m_IgnoreNextFootStep && !bothFeet)
            {
                m_IgnoreNextFootStep = false;
            }
            else
            {
                ++footPos.y;
                Transform playerTransform = ply.transform;
                Transform vpfpsplayer = ply.transform;
                int num = !bothFeet ? 1 : 2;
                bool flip = m_LeftFootStep;
                Vector3 vector3 = playerTransform.right * (!m_LeftFootStep ? m_FootPrintSideOffset : -m_FootPrintSideOffset);

                for (int index = 0; index < num; ++index)
                {
                    Vector3 offset = playerTransform.forward * 0;
                    Vector3 heelPos = footPos - offset + vector3;
                    if (index > 0)
                        heelPos -= offset * (UnityEngine.Random.value * 0.25f);
                    Vector3 point;
                    Vector3 normal;
                    if (GameManager.GetFootstepTrailManager().IsFootprintPositionValid(heelPos, offset, 0, out point, out normal))
                    {
                        if (SnowPatchManager.m_Active)
                            GameManager.GetFootstepTrailManager().AddPlayerFootstep(vpfpsplayer.position, point, normal, playerTransform.rotation.eulerAngles.y, flip, forceDeepFrac);

                        vector3 = -vector3;
                        flip = !flip;
                    }
                }
                m_IgnoreNextFootStep = bothFeet;
            }
        }

        public static void SimpleSleepWithNoSleep(int hours)
        {
            Rest RestCon = GameManager.GetRestComponent();
            GameManager.GetPlayerManagerComponent().m_God = true;
            RestCon.m_Sleeping = true;
            RestCon.m_ForceSleepLength = true;
            RestCon.m_ForcedSleepLength = hours;
            RestCon.m_PassTimeOptions = Rest.PassTimeOptions.None;
            RestCon.AccelerateTimeOfDay((int)((double)hours * 60.0), RestCon.m_SleepFadeOutSeconds, false);
            RestCon.m_NumSecondsSleeping = 0.0f;
            RestCon.m_SleepDurationHours = (float)hours;
            RestCon.m_SleepDurationSeconds = (float)((double)hours * 60.0 * 60.0);
            RestCon.m_ShouldInterruptWhenFreezing = false;
            GameManager.GetRestComponent().m_WakeUpAtFullRest = false;
            MelonLogger.Msg("Called fake sleep for "+ hours+ " hours");
        }

        public static void SendConsume(bool IsDrink)
        {
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.CONSUME))
                {
                    _packet.Write(IsDrink);
                    SendUDPData(_packet);
                }
            }

            if (iAmHost == true)
            {
                ServerSend.CONSUME(0, IsDrink, true);
            }

            if (IsDrink)
            {
                PushActionToMyDoll("Drink");
            } else {
                PushActionToMyDoll("Eat");
            }
        }

        public static void SendStopConsume()
        {
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.STOPCONSUME))
                {
                    _packet.Write(MyAnimState);
                    SendUDPData(_packet);
                }
            }

            if (iAmHost == true)
            {
                ServerSend.STOPCONSUME(0, MyAnimState, true);
            }
        }

        public static void MakeFakeFire(Fire fire)
        {
            fire.m_StartedByPlayer = false;
            if (fire.m_FireState != FireState.FullBurn)
            {
                fire.FireStateSet(FireState.FullBurn);
            }
            fire.m_HeatSource.TurnOn();
            fire.m_FX.TriggerStage(FireState.FullBurn, true, true);
            fire.m_FuelHeatIncrease = fire.m_HeatSource.m_MaxTempIncrease;
            fire.m_ElapsedOnTODSeconds = 0.0f;
            fire.m_ElapsedOnTODSecondsUnmodified = 0.0f;
            fire.ForceBurnTimeInMinutes(5);
            fire.PlayFireLoop(100f);

            if (fire.m_Campfire != null)
            {
                Campfire campFire = fire.m_Campfire.GetComponent<Campfire>();
                if (campFire.m_State != CampfireState.Lit)
                {
                    campFire.SetState(CampfireState.Lit);
                }
            }
        }

        public static void MakeFakeCampfire(DataStr.FireSourcesSync SyncData)
        {
            GameObject campfireObj = UnityEngine.Object.Instantiate<GameObject>(GameManager.GetFireManagerComponent().m_CampFirePrefab);
            campfireObj.name = GameManager.GetFireManagerComponent().m_CampFirePrefab.name;
            campfireObj.transform.position = SyncData.m_Position;
            campfireObj.transform.rotation = SyncData.m_Rotation;
            Fire cfFire = campfireObj.GetComponent<Fire>();
            MakeFakeFire(cfFire);
            Campfire campFire = campfireObj.GetComponent<Campfire>();
            if (campFire.m_State != CampfireState.Lit)
            {
                campFire.SetState(CampfireState.Lit);
            }
        }

        public static bool IsSameFire(DataStr.FireSourcesSync FindData, DataStr.FireSourcesSync SyncData)
        {
            if (SyncData.m_IsCampfire == true)
            {
                if (SyncData.m_Position == FindData.m_Position)
                {
                    return true;
                }
            } else {
                if (FindData.m_Guid != "")
                {
                    if (SyncData.m_Guid == FindData.m_Guid)
                    {
                        return true;
                    }
                } else {
                    if (SyncData.m_Position == FindData.m_Position)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void ApplyOtherFireSource(DataStr.FireSourcesSync SyncData)
        {
            //MelonLogger.Msg("[FireSourcesSync] ApplyOtherFireSource Should apply "+ ApplyOtherCampfires);
            if (ApplyOtherCampfires == true && ServerConfig.m_FireSync != 0)
            {
                if (FireManager.m_Fires.Count > 0)
                {
                    bool FoundSource = false;
                    for (int i = 0; i < FireManager.m_Fires.Count; i++)
                    {
                        Fire curfire = FireManager.m_Fires[i];
                        //MelonLogger.Msg("[FireSourcesSync] ApplyOtherFireSource index " + i+"/"+ FireManager.m_Fires.Count);
                        if (curfire != null)
                        {
                            //MelonLogger.Msg("[FireSourcesSync] Object exist "+ curfire.gameObject.name);
                            DataStr.FireSourcesSync FindData = new DataStr.FireSourcesSync();
                            FindData.m_LevelId = levelid;
                            FindData.m_LevelGUID = level_guid;
                            FindData.m_Position = curfire.gameObject.transform.position;
                            FindData.m_Rotation = curfire.gameObject.transform.rotation;
                            FindData.m_Guid = "";

                            if (curfire.gameObject.GetComponent<ObjectGuid>() != null)
                            {
                                FindData.m_Guid = curfire.gameObject.GetComponent<ObjectGuid>().Get();
                                //MelonLogger.Msg("[FireSourcesSync] Has GUID " + FindData.m_Guid);
                            }

                            if (IsSameFire(FindData, SyncData) == true)
                            {
                                //MelonLogger.Msg("Found same fire");
                                if (curfire.m_FireState == FireState.Off || curfire.m_StartedByPlayer == false)
                                {
                                    //MelonLogger.Msg("Apply fire on existen object");
                                    MakeFakeFire(curfire);
                                    FoundSource = true;
                                } else {
                                    //MelonLogger.Msg("Found object, but won't apply fire on it");
                                    FoundSource = true;
                                }
                                break;
                            } else {
                                //MelonLogger.Msg("Object is not same");
                            }
                        } else {
                            //MelonLogger.Msg("[FireSourcesSync] Object in array not exist!");
                        }
                    }
                    if (FoundSource == false)
                    {
                        //MelonLogger.Msg("Not found fire object");
                        if (SyncData.m_IsCampfire == true)
                        {
                            //MelonLogger.Msg("But is campfire, so creating an new one");
                            MakeFakeCampfire(SyncData);
                        }
                    }
                }
            }
        }

        public static void AddFuelNoSync(Fire fire, GearItem fuel)
        {
            if (fire.gameObject != null && fuel != null)
            {
                MelonLogger.Msg("[Fire][AddFuelNoSync] " + fire.gameObject.name + " fuel is " + fuel.m_GearName);
            }
            else if (fire.gameObject != null && fuel == null)
            {
                MelonLogger.Msg("[Fire][AddFuelNoSync] " + fire.gameObject.name + " fuel is null");
            }
            else if (fire.gameObject == null)
            {
                MelonLogger.Msg("[Fire][AddFuelNoSync] firesource is null");
            }
            fire.OnFuelBurnt(fuel);
            if (fire.IsEmbers())
            {
                fire.m_FX.TriggerStage(FireState.FullBurn, true);
                fire.m_HeatSource.TurnOn();
            }
            float num1 = (float)((double)fuel.m_FuelSourceItem.GetModifiedBurnDurationHours(fuel.GetNormalizedCondition()) * 60.0 * 60.0);
            if (!fire.m_IsPerpetual)
            {
                fire.m_MaxOnTODSeconds += num1;
                float num2 = fire.m_MaxOnTODSeconds - (float)((double)GameManager.GetFireManagerComponent().m_MaxDurationHoursOfFire * 60.0 * 60.0);
                if ((double)num2 > 0.0)
                {
                    fire.m_ElapsedOnTODSeconds -= num2;
                    fire.m_ElapsedOnTODSeconds = Mathf.Clamp(fire.m_ElapsedOnTODSeconds, 0.0f, float.PositiveInfinity);
                }
                fire.m_MaxOnTODSeconds = Mathf.Clamp(fire.m_MaxOnTODSeconds, 0.0f, (float)((double)GameManager.GetFireManagerComponent().m_MaxDurationHoursOfFire * 60.0 * 60.0));
            }
            fire.m_FuelHeatIncrease += Mathf.Min(Mathf.Clamp(!false || (double)fuel.m_FuelSourceItem.m_FireAgeMinutesBeforeAdding <= 0.0 ? GameManager.GetFireManagerComponent().m_MaxHeatIncreaseOfFire - fire.m_FuelHeatIncrease : GameManager.GetFireManagerComponent().m_MaxHeatIncreaseOfFireInForge - fire.m_FuelHeatIncrease, 0.0f, float.PositiveInfinity), fuel.m_FuelSourceItem.m_HeatIncrease * fuel.GetNormalizedCondition());
            if (fire.m_ApplyToHeatSource)
            {
                fire.m_HeatSource.m_MaxTempIncrease = fire.m_FuelHeatIncrease;
                fire.m_HeatSource.m_MaxTempIncreaseInnerRadius = Mathf.Max(fuel.m_FuelSourceItem.m_HeatInnerRadius, fire.m_HeatSource.m_MaxTempIncreaseInnerRadius);
                fire.m_HeatSource.m_MaxTempIncreaseOuterRadius = Mathf.Max(fuel.m_FuelSourceItem.m_HeatOuterRadius * fire.GetFireOuterRadiusScale(), fire.m_HeatSource.m_MaxTempIncreaseOuterRadius);
            }
            fire.m_FX.TriggerFlareupLarge();
            fire.m_StartedByPlayer = true;

            if (fuel != null && fuel.gameObject)
            {
                UnityEngine.Object.Destroy(fuel.gameObject);
            }
        }


        public static void AddOtherFuel(DataStr.FireSourcesSync SyncData, string fuelName)
        {
            if (ApplyOtherCampfires == true)
            {
                if (FireManager.m_Fires.Count > 0)
                {
                    for (int i = 0; i < FireManager.m_Fires.Count; i++)
                    {
                        Fire curfire = FireManager.m_Fires[i];
                        if (curfire != null)
                        {
                            DataStr.FireSourcesSync FindData = new DataStr.FireSourcesSync();
                            FindData.m_LevelId = levelid;
                            FindData.m_LevelGUID = level_guid;
                            FindData.m_Position = curfire.gameObject.transform.position;
                            FindData.m_Rotation = curfire.gameObject.transform.rotation;
                            FindData.m_Guid = "";

                            if (curfire.gameObject.GetComponent<ObjectGuid>() != null)
                            {
                                FindData.m_Guid = curfire.gameObject.GetComponent<ObjectGuid>().Get();
                            }

                            if (IsSameFire(FindData, SyncData) == true)
                            {
                                if (curfire.m_FireState == FireState.FullBurn)
                                {
                                    GameObject gear = UnityEngine.Object.Instantiate(Resources.Load(fuelName)).Cast<GameObject>();
                                    if (gear)
                                    {
                                        GearItem gearItem = gear.GetComponent<GearItem>();
                                        if (gearItem)
                                        {
                                            AddFuelNoSync(curfire, gearItem);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static List<DataStr.FireSourcesSync> FireSources = new List<DataStr.FireSourcesSync>();
        public static void MayAddFireSources(DataStr.FireSourcesSync fire)
        {
            //MelonLogger.Msg("[FireSourcesSync] MayAddFireSources "+ fire.m_LevelId+" "+ fire.m_LevelGUID);
            if (fire.m_LevelId != levelid || fire.m_LevelGUID != level_guid)
            {
                return;
            }
            //MelonLogger.Msg("MayAddFireSources for SyncData " + fire.m_LevelId + " m_LevelGUID " + fire.m_LevelGUID + " m_Position" + fire.m_Position.x + " " + fire.m_Position.y + " " + fire.m_Position.z);
            //MelonLogger.Msg("FireSources Size" + FireSources.Count);
            fire.m_RemoveIn = 5;
            for (int i = 0; i < FireSources.Count; i++)
            {
                DataStr.FireSourcesSync currFire = FireSources[i];
                //MelonLogger.Msg("FireSources["+i+ "] m_LevelId" + currFire.m_LevelId + " m_LevelGUID "+ currFire.m_LevelGUID + " m_Position" + currFire.m_Position.x+" "+ currFire.m_Position.y+" "+ currFire.m_Position.z);
                bool UpdateCurr = false;
                if (currFire.m_LevelId == fire.m_LevelId && currFire.m_LevelGUID == fire.m_LevelGUID)
                {
                    if (currFire.m_Guid != "")
                    {
                        if (currFire.m_Guid == fire.m_Guid)
                        {
                            UpdateCurr = true;
                        }
                    } else {
                        if (currFire.m_Position == fire.m_Position)
                        {
                            UpdateCurr = true;
                        }
                    }
                }
                if (UpdateCurr == true)
                {
                    FireSources[i].m_Fuel = fire.m_Fuel;
                    FireSources[i].m_RemoveIn = fire.m_RemoveIn;
                    return;
                }
            }
            FireSources.Add(fire);
        }

        public static void SendMyFire(Fire fireTosend, string fuel = "")
        {
            if (ServerConfig.m_FireSync == 0)
            {
                return;
            }
            if (fuel != "" && ServerConfig.m_FireSync == 1)
            {
                return;
            }


            //MelonLogger.Msg("SendMyFire");
            DataStr.FireSourcesSync SendData = new DataStr.FireSourcesSync();
            SendData.m_Fuel = fireTosend.GetRemainingLifeTimeSeconds();
            SendData.m_Guid = "";
            if (fireTosend.gameObject.GetComponent<ObjectGuid>() != null)
            {
                SendData.m_Guid = fireTosend.gameObject.GetComponent<ObjectGuid>().Get();
            }
            SendData.m_LevelGUID = level_guid;
            SendData.m_LevelId = levelid;
            SendData.m_Position = fireTosend.gameObject.transform.position;
            SendData.m_Rotation = fireTosend.gameObject.transform.rotation;

            if (fireTosend.m_Campfire == null)
            {
                SendData.m_IsCampfire = false;
            } else {
                SendData.m_IsCampfire = true;
                SendData.m_Guid = "";
            }

            if (fuel == "")
            {
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.FIRE))
                    {
                        _packet.Write(SendData);
                        SendUDPData(_packet);
                    }
                }

                if (iAmHost == true)
                {
                    ServerSend.FIRE(0, SendData, true);
                }
            } else {
                SendData.m_FuelName = fuel;
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.FIREFUEL))
                    {
                        _packet.Write(SendData);
                        SendUDPData(_packet);
                    }
                }

                if (iAmHost == true)
                {
                    ServerSend.FIREFUEL(0, SendData, true);
                }
            }
        }

        public static bool DsServerIsUp = false;
        public static int SecondsWithoutSaving = 0;
        public static int SecondsWithoutRestart = 0;
        public static bool NeedApplyAutoCMDs = false;
        public static bool AutoHostWhenLoaded = false;
        public static int ApplyAutoThingsAfterLoaed = 0;

        public static float MinimalDistanceForSpawn = 300;
        public static float MaximalDistanceForAnimalRender = 260;

        public static int AnyOneClose(float minimalDistance, Vector3 point) // Returns true if someone locates near with this point.
        {
            // First checking if I am myself close to this point, if so end up this very quick, without even checking other.
            float MyDis = Vector3.Distance(GameManager.GetPlayerTransform().position, point);
            if (MyDis <= minimalDistance)
            {
                return ClientUser.myId;
            }

            for (int i = 0; i < playersData.Count; i++)
            {
                if (playersData[i] != null)
                {
                    if (playersData[i].m_Levelid == levelid && playersData[i].m_LevelGuid == level_guid)
                    {
                        float plDis = Vector3.Distance(MyMod.playersData[i].m_Position, point);
                        if (plDis <= minimalDistance)
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }
        public static float GetClosestDistanceFromEveryone(Vector3 point) // Returns minimal distance to this point from most close player.
        {
            float result = float.PositiveInfinity; // Will be always bigger than anything we gonna compare with until we set it for first time.
            // Do not forget about myself, maybe exactly me is closet one.
            float MyDis = Vector3.Distance(GameManager.GetPlayerTransform().position, point);
            if (MyDis <= result)
            {
                result = MyDis;
            }

            for (int i = 0; i < playersData.Count; i++)
            {
                if (playersData[i] != null)
                {
                    if (playersData[i].m_Levelid == levelid && playersData[i].m_LevelGuid == level_guid)
                    {
                        float plDis = Vector3.Distance(MyMod.playersData[i].m_Position, point);
                        if (plDis < result)
                        {
                            result = plDis;
                        }
                    }
                }
            }
            return result;
        }

        public static float LastResponceTime = 0;

        public static int GetAnimalCount()
        {
            return BaseAiManager.m_BaseAis.Count + ActorsList.Count;
        }

        public static bool RebootSceneAfterSave = false;
        public static bool CloseGameOnNextDSSave = false;
        public static bool NoDsSaves = false;

        public static void EverySecond()
        {
            if (DsServerIsUp == true)
            {
                SecondsWithoutSaving++;

                if(RestartPerioud != -1)
                {
                    SecondsWithoutRestart++;
                }
                
                if (SecondsWithoutSaving > DsSavePerioud)
                {
                    SecondsWithoutSaving = 0;

                    if (!CloseGameOnNextDSSave)
                    {
                        if (RestartPerioud != -1 && SecondsWithoutRestart >= RestartPerioud)
                        {
                            CloseGameOnNextDSSave = true;
                            DataStr.MultiplayerChatMessage MSG = new DataStr.MultiplayerChatMessage();
                            MSG.m_Type = 0;
                            MSG.m_By = "";
                            MSG.m_Message = "[SERVER] Server reboot in 30 seconds!";
                            Shared.SendMessageToChat(MSG, true);
                            DataStr.MultiplayerChatMessage MSG2 = new DataStr.MultiplayerChatMessage();
                            MSG2.m_Type = 0;
                            MSG2.m_By = "";
                            MSG2.m_Message = "[SERVER] You can stay on server to being auto-reconnect, once server is up again. Or you can quit to process auto-save.";
                            Shared.SendMessageToChat(MSG, true);
                            Shared.SendMessageToChat(MSG2, true);
                            DsSavePerioud = 30;
                        }

                        if (!NoDsSaves)
                        {
                            GameManager.ForceSaveGame();
                            MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Server saved! Next save in " + DsSavePerioud + " seconds later!");
                        }
                    }else{
                        MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Restarting...");
                        ServerSend.RESTART();
                        QuitWithoutSaving = true;
                        Application.Quit();
                    }
                }
            }
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.KEEPITALIVE))
                {
                    _packet.Write(true);
                    SendUDPData(_packet);
                }
            }

            if (CurrentCustomChalleng.m_Started)
            {
                CustomChallengeUpdate();
                if (iAmHost == true)
                {
                    if (CurrentCustomChalleng.m_Time > 0)
                    {
                        CurrentCustomChalleng.m_Time--;
                    }
                    ServerSend.CHALLENGEUPDATE(CurrentCustomChalleng);
                }
            }
            if (MyLobby != "")
            {
                SteamConnect.Main.OnRegularUpdate();
            }

            if (RegularUpdateSeconds > 0)
            {
                RegularUpdateSeconds = RegularUpdateSeconds - 1;

                if (RegularUpdateSeconds == 0)
                {
                    RegularUpdateSeconds = 7;
                    SendRegularAlignData();
                }
            }
            if (SendAfterLoadingFinished > 0)
            {
                SendAfterLoadingFinished = SendAfterLoadingFinished - 1;
                if (SendAfterLoadingFinished == 0)
                {
                    FinishedLoading();
                }
            }
            if (SecondsLeftUntilWorryAboutPacket != -1)
            {
                if (SecondsLeftUntilWorryAboutPacket > 0)
                {
                    SecondsLeftUntilWorryAboutPacket = SecondsLeftUntilWorryAboutPacket - 1;
                    if (SecondsLeftUntilWorryAboutPacket == 0)
                    {
                        RepeatLastRequest();
                    }
                }
            }
            if (TryMakeLobbyAgain > 0)
            {
                TryMakeLobbyAgain = TryMakeLobbyAgain - 1;
                if (TryMakeLobbyAgain == 0 && MyLobby == "")
                {
                    SteamConnect.Main.MakeLobby();
                }
            }

            if (!DedicatedServerAppMode)
            {
                PlayersUpdateManager();
                for (int i = 0; i < FireSources.Count; i++)
                {
                    if (FireSources[i] != null)
                    {
                        if (FireSources[i].m_RemoveIn > 0)
                        {
                            FireSources[i].m_RemoveIn = FireSources[i].m_RemoveIn - 1;

                            if (FireSources[i].m_LevelId == levelid && FireSources[i].m_LevelGUID == level_guid)
                            {
                                ApplyOtherFireSource(FireSources[i]);
                            }
                        }else{
                            FireSources.RemoveAt(i);
                        }
                    }
                }
                if (FireManager.m_Fires.Count > 0)
                {
                    for (int i = 0; i < FireManager.m_Fires.Count; i++)
                    {
                        Fire fireCur = FireManager.m_Fires[i];
                        if (fireCur != null)
                        {
                            if (fireCur.m_FireState == FireState.FullBurn && fireCur.m_StartedByPlayer == true)
                            {
                                SendMyFire(fireCur);
                            }
                        }
                    }
                }
            }

           
            if (level_name == "MainMenu")
            {
                if (NeedConnectAfterLoad > 0)
                {
                    NeedConnectAfterLoad = NeedConnectAfterLoad - 1;
                    if (NeedConnectAfterLoad == 0)
                    {
                        NeedConnectAfterLoad = -1;
                        if (SteamConnect.CanUseSteam == true)
                        {
                            SteamConnect.Main.JoinToLobby(SteamLobbyToJoin);
                        }
                    }
                }
                if (NeedLoadSaveAfterLoad > 0)
                {
                    NeedLoadSaveAfterLoad = NeedLoadSaveAfterLoad - 1;
                    if (NeedLoadSaveAfterLoad == 0)
                    {
                        NeedLoadSaveAfterLoad = -1;
                        ForceLoadSlotForPlaying(AutoStartSlot);
                    }
                }
                if (StartDSAfterLoad > 0)
                {
                    StartDSAfterLoad = StartDSAfterLoad - 1;
                    if (StartDSAfterLoad == 0)
                    {
                        StartDSAfterLoad = -1;
                        StartDedicatedServer();
                    }
                }
            }

            if (DsServerIsUp && DedicatedServerAppMode)
            {
                levelid = 0;
                level_guid = "null";
            }

            if (level_name != "Empty" && level_name != "Boot" && level_name != "MainMenu")
            {

                if (DedicatedServerAppMode)
                {
                    if(DsServerIsUp == false)
                    {
                        DsServerIsUp = true;

                        if (SetP2PToLobbyForDSAfterLoad)
                        {
                            SetP2PToLobbyForDSAfterLoad = false;
                            if(MyLobby != "")
                            {
                                FinishStartingDsServer();
                            }
                        }
                        if (HostUDDSAfterLoad)
                        {
                            HostUDDSAfterLoad = false;
                            Shared.HostAServer(PortForDSLoad);
                            MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Server is ready! Have fun!");
                        }
                    }
                }else{
                    UpdateMyClothing();
                    ApplyOpenables();
                }

                if (ApplyAutoThingsAfterLoaed == 0 && (NeedApplyAutoCMDs == true || AutoHostWhenLoaded == true))
                {
                    if (NeedApplyAutoCMDs == true || AutoHostWhenLoaded == true)
                    {
                        MelonLogger.Msg(ConsoleColor.Magenta, "[Start-ups] Going to apply some parameters after load save");
                    }

                    ApplyAutoThingsAfterLoaed = 3;
                }
                if (ApplyAutoThingsAfterLoaed > 0)
                {
                    ApplyAutoThingsAfterLoaed = ApplyAutoThingsAfterLoaed - 1;
                    if (ApplyAutoThingsAfterLoaed == 0)
                    {
                        if (NeedApplyAutoCMDs == true)
                        {
                            NeedApplyAutoCMDs = false;
                            if (AutoCMDs.Count > 0)
                            {
                                MelonLogger.Msg(ConsoleColor.Magenta, "[-cmd] Going to apply auto commands...");
                                for (int i = 0; i < AutoCMDs.Count; i++)
                                {
                                    MelonLogger.Msg(ConsoleColor.Magenta, "[-cmd] " + AutoCMDs[i]);
                                    uConsole.RunCommandSilent(AutoCMDs[i]);
                                }
                            }
                        }
                        if (AutoHostWhenLoaded == true)
                        {
                            AutoHostWhenLoaded = false;
                            MelonLogger.Msg(ConsoleColor.Magenta, "[-host] Going to start server with using server.json");
                            StartDedicatedServer(false);
                        }
                    }
                }

                if (UpdateSnowshelters > 0)
                {
                    UpdateSnowshelters = UpdateSnowshelters - 1;
                    if (UpdateSnowshelters == 0)
                    {
                        UpdateSnowshelters = -1;
                        //MelonLogger.Msg(ConsoleColor.Blue, "Apply all showshelters");
                        LoadAllSnowSheltersByOther();
                    }
                }

                if (UpdateRopesAndFurns > 0)
                {
                    UpdateRopesAndFurns = UpdateRopesAndFurns - 1;
                    //MelonLogger.Msg(ConsoleColor.Blue, "Apply other campfires");
                    ApplyOtherCampfires = true;

                    if (UpdateRopesAndFurns == 0)
                    {
                        //MelonLogger.Msg(ConsoleColor.Blue, "Apply ropes and furns");
                        UpdateDeployedRopes();
                    }
                }
                if (UpdateCampfires > 0)
                {
                    UpdateCampfires = UpdateCampfires - 1;

                    if (UpdateCampfires == 0)
                    {
                        //MelonLogger.Msg(ConsoleColor.Blue, "Apply other campfires");
                        ApplyOtherCampfires = true;
                    }
                }
                if (UpdateEverything > 0)
                {
                    UpdateEverything = UpdateEverything - 1;
                    if (UpdateEverything == 0)
                    {
                        UpdateEverything = -1;
                        Pathes.LoadEverything();
                    }
                }
                if (RemoveAttachedGears > 0)
                {
                    RemoveAttachedGears--;
                    if (RemoveAttachedGears == 0)
                    {
                        BakePreSpawnedGearsList();
                        foreach (DataStr.PickedGearSync gear in PickedGearsBackup)
                        {
                            Shared.AddPickedGear(gear.m_Spawn, gear.m_LevelID, gear.m_LevelGUID, -1, gear.m_MyInstanceID, gear.m_GearName, false);
                        }
                        PickedGearsBackup.Clear();
                    }                    
                }

                if (GameManager.m_Thirst != null)
                {
                    Thirst th = GameManager.GetThirstComponent();
                    if (th.m_LitersLeftToDrink > 0)
                    {
                        IsDrinking = true;
                    } else {
                        IsDrinking = false;
                    }
                    if (PreviousIsDrinking != IsDrinking)
                    {
                        PreviousIsDrinking = IsDrinking;
                        if (IsDrinking == true)
                        {
                            MelonLogger.Msg("Drinking");
                            SendConsume(true);
                        } else {
                            MelonLogger.Msg("Finished Drinking");
                            SendStopConsume();
                        }
                    }
                }
                if (GameManager.m_Hunger != null)
                {
                    Hunger hun = GameManager.GetHungerComponent();
                    if (hun.m_CaloriesLeftToAdd > 0)
                    {
                        IsEating = true;
                    } else {
                        IsEating = false;
                    }
                    if (PreviousIsEating != IsEating)
                    {
                        PreviousIsEating = IsEating;
                        if (IsEating == true)
                        {
                            if (hun.m_FoodItemProvidingCalories != null)
                            {
                                MelonLogger.Msg("Eating " + hun.m_FoodItemProvidingCalories.m_GearItem.m_GearName);
                                SendConsume(hun.m_FoodItemProvidingCalories.m_IsDrink);
                            }
                        } else {
                            MelonLogger.Msg("Finished Eating");
                            SendStopConsume();
                        }
                    }
                }
                if (GameManager.m_PlayerObject)
                {
                    GearItem Item = GameManager.GetPlayerManagerComponent().m_ItemInHands;
                    if (Item && Item.m_HandheldShortwaveItem && CairnsSearchActive() == true)
                    {
                        DoRadioSearch();
                    }
                }
            }

            if (chatInput != null && chatInput.gameObject.activeSelf == false && ChatObject != null && ChatObject.activeSelf == true)
            {
                HideChatTimer = HideChatTimer - 1;
                if (HideChatTimer <= 0)
                {
                    ChatObject.SetActive(false);
                }
            }

            if (InOnline())
            {
                int character = 0;

                if (GameManager.m_PlayerManager != null)
                {
                    character = (int)GameManager.GetPlayerManagerComponent().m_VoicePersona;
                }

                if (iAmHost == true)
                {
                    ServerSend.SELECTEDCHARACTER(0, character, true);
                }

                if (sendMyPosition == true)
                {
                    if (LastResponceTime == 0)
                    {
                        LastResponceTime = Time.time;
                    } else {
                        //double Ping = Math.Round(Time.time - MyMod.LastResponceTime, 2) * 100;
                        //MelonLogger.Msg("Ping " + Ping + "ms");
                    }
                    using (Packet _packet = new Packet((int)ClientPackets.SELECTEDCHARACTER))
                    {
                        _packet.Write(character);
                        SendUDPData(_packet);
                    }
                }
            }
        }

        private static void EveryInGameMinute()
        {
            if (iAmHost == true)
            {
                if (IsCycleSkiping == true && GameManager.m_PlayerManager != null)
                {
                    if (GameManager.GetPlayerManagerComponent().PlayerIsSleeping() == false || IsDead == true)
                    {
                        IsCycleSkiping = false;
                    }
                }

                //if (level_name != "Boot" && level_name != "Empty" && GameManager.m_Wind != null && GameManager.m_Wind.m_ActiveSettings != null && GameManager.m_Weather != null && GameManager.m_WeatherTransition != null)
                //{
                //    DataStr.WeatherProxies weather = new DataStr.WeatherProxies();
                //    weather.m_WeatherProxy = GameManager.GetWeatherComponent().Serialize();
                //    weather.m_WeatherTransitionProxy = GameManager.GetWeatherTransitionComponent().Serialize();
                //    weather.m_WindProxy = GameManager.GetWindComponent().Serialize();
                //    ServerSend.SYNCWEATHER(weather);

                //    //MPSaveManager.SaveJsonSnapshot("Weather", weather.m_WeatherProxy);
                //    //MPSaveManager.SaveJsonSnapshot("WeatherTransition", weather.m_WeatherTransitionProxy);
                //    //MPSaveManager.SaveJsonSnapshot("Wind", weather.m_WindProxy);
                //}
            }
            if (TrackableDroppedGearsObjs.Count > 0)
            {
                foreach (var item in TrackableDroppedGearsObjs)
                {
                    if (item.Value && item.Value.GetComponent<Comps.DroppedGearDummy>())
                    {
                        Comps.DroppedGearDummy DGD = item.Value.GetComponent<Comps.DroppedGearDummy>();
                        int minutesLeft = DGD.m_Extra.m_GoalTime - MinutesFromStartServer;
                        if (minutesLeft <= 0)
                        {
                            item.Value.transform.GetChild(0).gameObject.SetActive(false);
                        }
                    }
                }
            }
            MaySpawnRefMan();
        }

        public static float nextActionTime = 0.0f;
        public static float period = 5f;
        public static float nextActionTimeAniamls = 0.0f;
        public static float periodAniamls = 0.3f;
        public static float nextActionTimeSecond = 0.0f;
        public static float periodSecond = 1f;

        public static bool InOnline() // If you are host or client
        {
            if (iAmHost == true || sendMyPosition == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //WILDLIFE_Wolf
        //WILDLIFE_Wolf_Aurora
        //WILDLIFE_Bear
        //WILDLIFE_Bear_Aurora
        //WILDLIFE_Stag
        //WILDLIFE_Rabbit
        //WILDLIFE_Moose

        public static void WriteDownMesh(GameObject animal)
        {
        }

        public static string GetAnimalPrefabName(string _name)
        {
            if (_name.StartsWith("WILDLIFE_Wolf"))
            {
                if (_name.StartsWith("WILDLIFE_Wolf_Aurora"))
                {
                    return "WILDLIFE_Wolf_Aurora";
                } else {
                    if (_name.StartsWith("WILDLIFE_Wolf_grey_aurora"))
                    {
                        return "WILDLIFE_Wolf_grey_aurora";
                    } else {
                        if (_name.StartsWith("WILDLIFE_Wolf_grey"))
                        {
                            return "WILDLIFE_Wolf_grey";
                        } else {
                            return "WILDLIFE_Wolf";
                        }
                    }
                }
            }

            if (_name.StartsWith("WILDLIFE_Bear"))
            {
                if (_name.StartsWith("WILDLIFE_Bear_Aurora"))
                {
                    return "WILDLIFE_Bear_Aurora";
                } else {
                    return "WILDLIFE_Bear";
                }
            }

            if (_name.StartsWith("WILDLIFE_Stag"))
            {
                return "WILDLIFE_Stag";
            }
            if (_name.StartsWith("WILDLIFE_Rabbit"))
            {
                return "WILDLIFE_Rabbit";
            }
            if (_name.StartsWith("WILDLIFE_Moose"))
            {
                return "WILDLIFE_Moose";
            }
            else {
                return "WILDLIFE_Wolf";
            }
        }

        public static void MakeAnimalActive(GameObject animal, bool active)
        {
            //MelonLogger.Msg("Nachinayem kuhat");
            if (animal == null)
            {
                return;
            }

            //MelonLogger.Msg("Animal narmalna");
            if (animal.transform.parent != null && animal.transform.parent.GetComponent<MoveAgent>() != null)
            {
                animal.transform.parent.GetComponent<MoveAgent>().enabled = active;
                //MelonLogger.Msg("[MoveAgent]-> off");
            }
            if (animal.transform.parent != null && animal.transform.parent.GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
            {
                animal.transform.parent.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = active;
            }
            //MelonLogger.Msg("MoveAgent nevonayet");
            if (animal.GetComponent<AiTarget>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<AiTarget>());
                animal.GetComponent<AiTarget>().enabled = active;
                //MelonLogger.Msg("[AiTarget]-> off");
            }
            //MelonLogger.Msg("AiTarget vkusna");
            if (animal.GetComponent<AiWolf>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<AiWolf>());
                animal.GetComponent<AiWolf>().enabled = active;
                //MelonLogger.Msg("[AiWolf]-> off");
            }
            //MelonLogger.Msg("AiWolf ogon");
            if (animal.GetComponent<AiStag>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<AiStag>());
                animal.GetComponent<AiStag>().enabled = active;
                //MelonLogger.Msg("[AiStag]-> off");
            }
            //MelonLogger.Msg("AiStag alright");
            if (animal.GetComponent<AiRabbit>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<AiRabbit>());
                animal.GetComponent<AiRabbit>().enabled = active;
                //MelonLogger.Msg("[AiRabbit]-> off");
            }
            //MelonLogger.Msg("AiRabbit yahooo");
            if (animal.GetComponent<AiMoose>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<AiMoose>());
                animal.GetComponent<AiMoose>().enabled = active;
                //MelonLogger.Msg("[AiMoose]-> off");
            }
            //MelonLogger.Msg("AiMoose DA");
            if (animal.GetComponent<AiBear>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<AiBear>());
                animal.GetComponent<AiBear>().enabled = active;
                //MelonLogger.Msg("[AiBear]-> off");
            }
            //MelonLogger.Msg("AiBear PIVO");
            if (animal.GetComponent<CharacterController>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<CharacterController>());
                animal.GetComponent<CharacterController>().enabled = active;
                //MelonLogger.Msg("[CharacterController]-> off");
            }
            //MelonLogger.Msg("CharacterController ANUS SEBE CONTROLIRUI");
            if (animal.GetComponent<NodeCanvas.Framework.Blackboard>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<NodeCanvas.Framework.Blackboard>());
                animal.GetComponent<NodeCanvas.Framework.Blackboard>().enabled = active;
                //MelonLogger.Msg("[NodeCanvas.Framework.Blackboard]-> off");
            }
            //MelonLogger.Msg("Blackboard DA BECAUSE DA");
            if (animal.GetComponent<TLDBehaviourTreeOwner>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<TLDBehaviourTreeOwner>());
                animal.GetComponent<TLDBehaviourTreeOwner>().enabled = active;
                //MelonLogger.Msg("TLDBehaviourTreeOwner]-> off");
            }
            //MelonLogger.Msg("TLDBehaviourTreeOwner ZDRAVSVUI DEREVO");
            if (animal.GetComponent<BaseAi>() != null)
            {
                animal.GetComponent<BaseAi>().enabled = active;
            }
        }
        public static GameObject SpawnAnimalActor(string prefabName, Vector3 v3, Quaternion rot, string GUID, string RegionGUID, List<DataStr.AnimalArrow> Arrows)
        {
            GameObject reference = GetGearItemObject(prefabName);

            if (reference == null)
            {
                MelonLogger.Msg(ConsoleColor.Red, "Can't create animal actor with prefab name " + prefabName);
                return null;
            }

            GameObject obj = UnityEngine.Object.Instantiate(reference, v3, rot);
            Comps.AnimalActor act = obj.AddComponent<Comps.AnimalActor>();
            act.m_Animal = obj;
            act.m_Animator = RemoveAnimalComponents(obj);
            act.m_ToGo = v3;
            act.m_ToRotate = rot;
            act.m_Arrows = Arrows;
            act.ProcessArrows();
            Light[] componentsInChildren = (Light[])obj.GetComponentsInChildren<Light>(true);
            if (componentsInChildren != null)
            {
                for (int index = 0; index < componentsInChildren.Length; ++index)
                    componentsInChildren[index].enabled = false;
            }

            if (obj.GetComponent<ObjectGuid>() == null)
            {
                obj.AddComponent<ObjectGuid>();
                obj.GetComponent<ObjectGuid>().Set(GUID);
            } else {
                obj.GetComponent<ObjectGuid>().Set(GUID);
            }

            if (!ActorsList.ContainsKey(GUID))
            {
                ActorsList.Add(GUID, act);
            }

            return obj;
        }

        public static void SpawnAnimalCorpse(string prefab, Vector3 v3, Quaternion rot, string GUID, string SpawnRegionGUID)
        {
            MelonLogger.Msg("Going to spawn animal corpse " + GUID + " for Region " + SpawnRegionGUID);

            GameObject reference = MyMod.GetGearItemObject(prefab);

            if (reference == null)
            {
                MelonLogger.Msg(ConsoleColor.Red, "Can't create animal body prefab name " + prefab);
                return;
            }
            GameObject oldObj = ObjectGuidManager.Lookup(GUID);
            if (oldObj)
            {
                if (oldObj.GetComponent<Comps.AnimalCorpseObject>() != null)
                {
                    MelonLogger.Msg("Corpse already exist, because been spawned myself");
                    return;
                } else
                {
                    MelonLogger.Msg("Replace original animal with corpse " + GUID);

                    if (oldObj.GetComponent<Comps.AnimalActor>() != null)
                    {
                        oldObj.GetComponent<Comps.AnimalActor>().m_MarkToDestroy = true;
                    } else if (oldObj.GetComponent<Comps.AnimalUpdates>() != null)
                    {
                        oldObj.GetComponent<Comps.AnimalUpdates>().m_MarkToDestroy = true;
                    }
                }
            }
            GameObject obj = UnityEngine.Object.Instantiate(reference, v3, rot); // Create new animal
            obj.name = reference.name;
            obj.layer = 19;
            if (obj.GetComponent<BaseAi>())
            {
                obj.GetComponent<BaseAi>().ChangeCollisionCapsulesLayer(19);
            }
            Animator _AN = MyMod.RemoveAnimalComponents(obj); // Remove all components

            if (obj.name.Contains("Rabbit"))
            {
                if (obj.GetComponent<BodyHarvest>())
                {
                    UnityEngine.Object.Destroy(obj.GetComponent<BodyHarvest>());
                }
            }

            _AN.SetBool(Animator.StringToHash("Dead"), true);
            _AN.SetInteger(Animator.StringToHash("DamageSide"), 1);
            _AN.SetTrigger(Animator.StringToHash("DamageImpact"));
            Comps.AnimalCorpseObject CorpseComp = obj.AddComponent<Comps.AnimalCorpseObject>();
            CorpseComp.m_Animal = obj;
            CorpseComp.m_RegionGUID = SpawnRegionGUID;
            CorpseComp.Update();
            if (obj.GetComponent<BodyHarvest>())
            {
                obj.GetComponent<BodyHarvest>().enabled = true;
            }

            if (obj.GetComponent<ObjectGuid>() == null) // Override GUID with old GUID
            {
                obj.AddComponent<ObjectGuid>();
                obj.GetComponent<ObjectGuid>().Set(GUID);
            } else
            {
                obj.GetComponent<ObjectGuid>().Set(GUID);
            }
            Light[] componentsInChildren2 = (Light[])obj.GetComponentsInChildren<Light>(true);
            if (componentsInChildren2 != null)
            {
                for (int index = 0; index < componentsInChildren2.Length; ++index)
                {
                    componentsInChildren2[index].enabled = false;
                }
            }
            MatchTransform.EnableCollidersForAllActive(true);
            if (!MyMod.AnimalCorplesList.ContainsKey(GUID))
            {
                MyMod.AnimalCorplesList.Add(GUID, CorpseComp);
            }
            MelonLogger.Msg("Animal corpse spawned " + GUID);
        }
        public static void ProcessAnimalCorpseSync(DataStr.AnimalKilled Sync)
        {
            if (Sync.m_LevelGUID == MyMod.level_guid)
            {
                SpawnAnimalCorpse(Sync.m_PrefabName, Sync.m_Position, Sync.m_Rotation, Sync.m_GUID, Sync.m_RegionGUID);
            }
        }

        public static bool CheckDistanceAgnistEveryPlayer(Vector3 pos, float dis, bool lesser)
        {
            for (int i = 0; i < MyMod.playersData.Count; i++)
            {
                if (MyMod.playersData[i] != null)
                {
                    if (playersData[i].m_Levelid == levelid && playersData[i].m_LevelGuid == level_guid)
                    {
                        float plDis = Vector3.Distance(playersData[i].m_Position, pos);

                        if (lesser)
                        {
                            if (plDis > dis)
                            {
                                return false;
                            }
                        } else {
                            if (plDis < dis)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public static DataStr.SaveSlotSync PendingSave = null;
        public static bool ShouldCreateSaveForHost = false;

        public static void SelectNameForHostSaveFile()
        {
            if (m_Panel_MainMenu != null)
            {
                m_Panel_MainMenu.ShowNameSaveSlotPopup();
                InterfaceManager.m_Panel_Confirmation.m_CurrentGroup.m_InputField.SetText(DateTime.Now.ToString());
            }
        }

        public static void SelectBagesForConnection()
        {
            GameAudioManager.PlayGuiConfirm();
            if (m_Panel_MainMenu != null)
            {
                if (PendingSave.m_SaveSlotType == (int)SaveSlotType.SANDBOX && m_Panel_MainMenu.GetNumUnlockedFeats() > 0)
                {
                    m_Panel_MainMenu.SelectWindow(m_Panel_MainMenu.m_SelectFeatWindow);
                } else {
                    if (!ShouldCreateSaveForHost)
                    {
                        ForcedCreateSave(PendingSave);
                    } else {
                        SelectNameForHostSaveFile();
                    }
                }
            }
        }

        public static void SelectGenderForConnection()
        {
            if (m_Panel_SelectSurvivor != null)
            {
                m_Panel_SelectSurvivor.Enable(true);
                m_Panel_SelectSurvivor.m_BasicMenu.m_OnClickBackAction = null;
            }
        }

        public static void VerifiedSave(bool Valid)
        {
            if (Valid)
            {
                if(PendingSaveSlotInfo == null)
                {
                    LetChooseSpawnForClient(PendingSave);
                }else{
                    SaveSlotInfo SaveToLoad = PendingSaveSlotInfo;
                    SaveGameSlots.SetBaseNameForSave(SaveToLoad.m_SaveSlotName, SaveToLoad.m_SaveSlotName);
                    MelonLogger.Msg("Save slot base name is " + SaveGameSlots.GetBaseNameForSave(SaveToLoad.m_SaveSlotName));
                    MelonLogger.Msg("Save slot name " + SaveToLoad.m_SaveSlotName);
                    MelonLogger.Msg("Save slot user defined name " + SaveGameSlots.GetUserDefinedSlotName(SaveToLoad.m_SaveSlotName));

                    SaveGameSystem.SetCurrentSaveInfo(SaveToLoad.m_Episode, SaveToLoad.m_GameMode, SaveToLoad.m_GameId, SaveToLoad.m_SaveSlotName);
                    MelonLogger.Msg("Selecting slot " + SaveGameSystem.GetCurrentSaveName());
                    GameManager.LoadSaveGameSlot(SaveToLoad.m_SaveSlotName, SaveToLoad.m_SaveChangelistVersion);
                    PendingSaveSlotInfo = null;
                }
            }else{

                if (PendingSaveSlotInfo != null)
                {
                    SaveGameSlotHelper.DeleteSaveSlotInfo(PendingSaveSlotInfo);
                    PendingSaveSlotInfo = null;
                }

                LetChooseSpawnForClient(PendingSave);
            }
        }

        public static void SendVerifySave(string UGUID, long Hash)
        {
            DoPleaseWait("Verifying...", " Host checking my save file...");
            using (Packet _packet = new Packet((int)ClientPackets.VERIFYSAVE))
            {
                _packet.Write(UGUID);
                _packet.Write(Hash);
                SendUDPData(_packet);
            }
        }

        public static SaveSlotInfo PendingSaveSlotInfo = null;
        public static void CheckHaveSaveFileToJoin(DataStr.SaveSlotSync Data)
        {
            AnimalsController = false;
            bool HaveSaveFile = false;
            Episode Ep = (Episode)Data.m_Episode;
            SaveSlotType SST = (SaveSlotType)Data.m_SaveSlotType;
            int Seed = Data.m_Seed;
            SaveSlotInfo SaveToLoad = null;

            Il2CppSystem.Collections.Generic.List<SaveSlotInfo> Slots = SaveGameSystem.GetSortedSaveSlots(Ep, SST);

            for (int i = 0; i < Slots.Count; i++)
            {
                SaveSlotInfo Slot = Slots[i];

                if (Slot.m_UserDefinedName == Seed + "")
                {
                    int GenVersion = MenuChange.GetRNGGen(Slot.m_SaveSlotName);
                    if (GenVersion == BuildInfo.RandomGenVersion)
                    {
                        MelonLogger.Msg("Found slot to load");
                        MelonLogger.Msg("Loading save file with seed: " + Seed);
                        HaveSaveFile = true;
                        SaveToLoad = Slot;
                        break;
                    } else
                    {
                        MelonLogger.Msg(ConsoleColor.DarkRed, "Found slot to load but it saved on outdated version of the game");
                        MelonLogger.Msg(ConsoleColor.DarkRed, "Generation version of this slot " + GenVersion + ". Release of mod you using right now has Generation version " + MyMod.BuildInfo.RandomGenVersion);
                    }
                }
            }

            if (HaveSaveFile == true)
            {
                KillConsole();
                MelonLogger.Msg("Trying loading save slot...");
                long Hash = Shared.GetDeterministicId(SaveGameSlots.LoadDataFromSlot(SaveToLoad.m_SaveSlotName, "global"));
                MelonLogger.Msg("Save slot hash " + Hash);
                string data = SaveGameSlots.LoadDataFromSlot(SaveToLoad.m_SaveSlotName, "UserGUID");
                string UGUID = "";
                if (data != null)
                {
                    string[] saveProxy = JSON.Load(data).Make<string[]>();
                    UGUID = saveProxy[0];
                }
                MelonLogger.Msg("My UGUID " + UGUID);
                PendingSaveSlotInfo = SaveToLoad;
                if (string.IsNullOrEmpty(UGUID))
                {
                    SendVerifySave("", 0);
                } else {
                    SendVerifySave(UGUID, Hash);
                }
            } else {
                SendVerifySave("", 0);
            }
        }

        public static void LetChooseSpawnForClient(DataStr.SaveSlotSync Data)
        {
            MelonLogger.Msg("Prepare ui for creating save slot");
            InterfaceManager.TrySetPanelEnabled<Panel_Sandbox>(false);
            InterfaceManager.TrySetPanelEnabled<Panel_ChooseSandbox>(false);
            InterfaceManager.TrySetPanelEnabled<Panel_Story>(false);
            InterfaceManager.TrySetPanelEnabled<Panel_ChooseSandbox>(false);

            if (ServerConfig.m_PlayersSpawnType == 0) // Same as host.
            {
                SelectGenderForConnection();
            }
            else if (ServerConfig.m_PlayersSpawnType == 1) // Can select
            {
                InterfaceManager.TrySetPanelEnabled<Panel_SelectRegion_Map>(true);
            }
            else if (ServerConfig.m_PlayersSpawnType == 2) // Random
            {
                Data.m_Location = (int)GameRegion.RandomRegion;
                SelectGenderForConnection();
            }
            else if (ServerConfig.m_PlayersSpawnType == 3) // Fixed place
            {
                SelectGenderForConnection();
            }
        }


        public static void ForcedCreateSave(DataStr.SaveSlotSync Data, string Name = "")
        {
            Episode Ep = (Episode)Data.m_Episode;
            SaveSlotType SST = (SaveSlotType)Data.m_SaveSlotType;
            int Seed = Data.m_Seed;
            ExperienceModeType ExpType = (ExperienceModeType)Data.m_ExperienceMode;
            GameRegion Region = (GameRegion)Data.m_Location;

            KillConsole();
            MelonLogger.Msg("Creating save slot " + Seed);

            GameManager.GetExperienceModeManagerComponent().SetExperienceModeType(ExpType);
            if (ExpType == ExperienceModeType.Custom)
            {
                MelonLogger.Msg("Custom Experience Mode Key is " + Data.m_CustomExperienceStr);
                bool ok = GameManager.GetExperienceModeManagerComponent().SetCurrentCustomModeString(Data.m_CustomExperienceStr);
                if (ok)
                {
                    MelonLogger.Msg("Custom Experience Mode recreated from string!");
                } else
                {
                    MelonLogger.Msg(ConsoleColor.Red, "Failed to recreate custom experience from string!");
                }
            }
            
            string UserDefinedName;
            if(Seed != 0)
            {
                UserDefinedName = Seed.ToString();
            } else
            {
                UserDefinedName = Name;
            }
            SaveGameSystem.SetCurrentSaveInfo(Ep, SST, SaveGameSlots.GetUnusedGameId(), null);


            SaveGameSlots.SetSlotDisplayName(SaveGameSystem.GetCurrentSaveName(), UserDefinedName);
            //SaveGameSlots.SetUserDefinedSlotName(SaveGameSystem.GetCurrentSaveName(), UserDefinedName);


            MelonLogger.Msg("Save slot created!");
            MelonLogger.Msg("Save slot current name " + SaveGameSystem.GetCurrentSaveName());
            MelonLogger.Msg("Save slot userdefined name " + SaveGameSlots.GetUserDefinedSlotName(SaveGameSystem.GetCurrentSaveName()));


            GameManager.m_StartRegion = Region;
            InterloperHook = true;
            OverridedSceneForSpawn = Data.m_FixedSpawnScene;
            OverridedPositionForSpawn = Data.m_FixedSpawnPosition;

            GameManager.Instance().LaunchSandbox();
            if (Seed != 0)
            {
                GameManager.m_SceneTransitionData.m_GameRandomSeed = Seed;
            }

            PendingSave = null;
            ShouldCreateSaveForHost = false;
        }

        public static void PlayMultiplayer3dAduio(string sound, int from)
        {
            if (playersData.Count > 0 && playersData[from] != null && playersData[from].m_Levelid == levelid && players[from] != null && playersData[from].m_LevelGuid == level_guid)
            {
                GameAudioManager.Play3DSound(sound, players[from]);
            }
        }
        public static void SendMultiplayerAudio(string sound)
        {
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.MULTISOUND))
                {
                    _packet.Write(sound);
                    SendUDPData(_packet);
                }
            }
            if (iAmHost == true)
            {
                using (Packet _packet = new Packet((int)ServerPackets.MULTISOUND))
                {
                    ServerSend.MULTISOUND(0, sound, true);
                }
            }
        }

        public static GameObject Golosovanie;
        public static bool DefaultIsRussian = false;

        public static void StartGOLOSOVANIE()
        {
            if (Golosovanie == null)
            {
                GameObject obj = new GameObject("Golosovanie");
                Golosovanie = obj;
                AudioClip LoadedAssets = LoadedBundle.LoadAsset<AudioClip>("Golosovanie");
                AudioSource ASs = obj.AddComponent<AudioSource>();
                ASs.volume = 0.03f;
                ASs.PlayOneShot(LoadedAssets);
            }
        }

        public static bool NotNeedToPauseUntilLoaded = false;

        public static Canvas UiCanvas = null;
        public static List<DataStr.MultiplayerChatMessage> ChatMessages = new List<DataStr.MultiplayerChatMessage>();
        public static int MaxChatMessages = 50;
        public static string MyChatName = "Player";

        public static UnityEngine.UI.InputField chatInput = null;
        public static GameObject chatPanel = null;
        public static GameObject ChatObject = null;
        public static GameObject StatusPanel = null;
        public static GameObject StatusObject = null;
        public static UnityEngine.UI.ScrollRect chatScroller = null;
        public static int HideChatTimer = 0;
        public static bool ValidNickName(string name)
        {
            if (name == "" || name == " " || name == "Player" || Shared.HasNonASCIIChars(name) == true)
            {
                return false;
            } else {
                return true;
            }
        }
        public static bool PendingRespawn = false;
        public static void BeginRespawn()
        {
            MelonLogger.Msg("BeginRespawn");
            GameManager.GetConditionComponent().m_CurrentHP = GameManager.GetConditionComponent().GetAdjustedMaxHP();
            GameManager.GetFreezingComponent().m_CurrentFreezing = 0;
            GameManager.GetHungerComponent().m_CurrentReserveCalories = GameManager.GetHungerComponent().m_MaxReserveCalories;
            GameManager.GetThirstComponent().m_CurrentThirst = 0;
            GameManager.GetFatigueComponent().m_CurrentFatigue = 0;
            if (GameManager.m_PlayerManager)
            {
                GameManager.m_PlayerManager.SetControlMode(PlayerControlMode.Normal);
            } else {
                MelonLogger.Msg("m_PlayerManager is null");
            }
            InterfaceManager.m_Panel_Log.ExitInterface();
            GameManager.GetInventoryComponent().DestroyAllGear();
            GameManager.GetPlayerManagerComponent().m_StartGear.AddAllToInventory();

            GameManager.GetSkillArchery().SetPoints(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillRifle().SetPoints(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillFireStarting().SetPoints(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillCarcassHarvesting().SetPoints(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillCooking().SetPoints(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillIceFishing().SetPoints(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillClothingRepair().SetPoints(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillRevolver().SetPoints(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillGunsmithing().SetPoints(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);

            GameManager.GetSkillArchery().SetTier(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillRifle().SetTier(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillFireStarting().SetTier(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillCarcassHarvesting().SetTier(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillCooking().SetTier(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillIceFishing().SetTier(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillClothingRepair().SetTier(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillRevolver().SetTier(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
            GameManager.GetSkillGunsmithing().SetTier(0, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);


            ConsoleManager.CONSOLE_afflictions_cure();

            GameManager.GetVpFPSCamera().enabled = true;


            if (!GameManager.IsOutDoorsScene(GameManager.m_ActiveScene))
            {
                string OutDoorToLoad = GameManager.m_SceneTransitionData.m_LastOutdoorScene;
                GameObject DummyLoader = new GameObject();
                LoadScene Loader = DummyLoader.AddComponent<LoadScene>();

                Loader.m_SceneToLoad = OutDoorToLoad;
                Loader.Activate();
            } else {
                GameObject sp = PlayerManager.PickRandomSpawnPoint();
                if (sp)
                {
                    GameManager.GetPlayerManagerComponent().TeleportPlayer(sp.transform.position, sp.transform.rotation);
                }
                SaveGameSystem.SetAsyncEnabled(false);
                PendingRespawn = true;
                GameManager.ForceSaveGame();
            }
        }

        public static void FindPlayerToTrade()
        {
            if (GameManager.m_PlayerObject == null)
            {
                //MelonLogger.Msg("Gaben trade is over player object is null, are all is gonna sus till the death");
                GiveItemTo = -1;
                return;
            }

            Vector3 myXYZ = GameManager.GetPlayerTransform().position;

            float LastDistance = float.PositiveInfinity;
            int LastPlayerFoundID = -1;
            for (int i = 0; i < playersData.Count; i++)
            {
                if (playersData[i] != null && playersData[i].m_Levelid == levelid && playersData[i].m_LevelGuid == level_guid && Vector3.Distance(myXYZ, playersData[i].m_Position) < 15 && i != ClientUser.myId)
                {
                    float checkDis = Vector3.Distance(myXYZ, playersData[i].m_Position);

                    if (checkDis < LastDistance)
                    {
                        LastPlayerFoundID = i;
                        LastDistance = checkDis;
                    }
                }
            }
            GiveItemTo = LastPlayerFoundID;
        }

        public static List<GameObject> StatusTexes = new List<GameObject>();
        public static Dictionary<ulong, GameObject> LobbyElements = new Dictionary<ulong, GameObject>();


        public static void AddPersonToLobby(string name, ulong SteamID, Texture2D Avatar)
        {
            if (DedicatedServerAppMode)
            {
                return;
            }
            
            if (!LobbyElements.ContainsKey(SteamID))
            {
                GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>("MP_PlayerLobby");
                GameObject Element = GameObject.Instantiate(LoadedAssets, LobbyUI.transform.GetChild(0).GetChild(0));
                Sprite sprite = Sprite.Create(Avatar, new Rect(0, 0, 64, -64), new Vector2(0, 0));
                Element.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>().overrideSprite = sprite;
                LobbyElements.Add(SteamID, Element);
                Element.AddComponent<Comps.LobbyHoverNickname>();
                Comps.LobbyHoverNickname LHN = Element.GetComponent<Comps.LobbyHoverNickname>();
                LHN.m_Btn = Element.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Button>();
            }
        }

        public static bool LobbyContains(ulong SteamID)
        {
            return LobbyElements.ContainsKey(SteamID);
        }

        public static void RemovePersonFromLobby(ulong SteamID)
        {
            if (DedicatedServerAppMode)
            {
                return;
            }
            GameObject Element;
            if (LobbyElements.TryGetValue(SteamID, out Element))
            {
                LobbyElements.Remove(SteamID);
                UnityEngine.Object.Destroy(Element);
            }
        }
        public static void SetPersonNameFromLobby(ulong SteamID, string name)
        {
            if (DedicatedServerAppMode)
            {
                return;
            }
            
            GameObject Element;
            if (LobbyElements.TryGetValue(SteamID, out Element))
            {
                Element.GetComponent<Comps.LobbyHoverNickname>().m_Name = name;
            }
        }

        public static void UpdatePlayerStatusMenu(List<DataStr.MultiPlayerClientStatus> MPs)
        {
            if (StatusPanel != null && MPs.Count > 0)
            {
                for (int i = 0; i < MaxPlayers; i++)
                {
                    if (StatusTexes.Count < MaxPlayers)
                    {
                        GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>("MP_PlayerText");
                        GameObject newText = GameObject.Instantiate(LoadedAssets, StatusPanel.transform);
                        UnityEngine.UI.Text Comp = newText.GetComponent<UnityEngine.UI.Text>();
                        Comp.text = i + ".";
                        StatusTexes.Add(newText);
                    }
                }
                for (int i = 0; i < StatusTexes.Count; i++)
                {
                    StatusTexes[i].SetActive(false);
                }
                for (int i = 0; i < MPs.Count; i++)
                {
                    if (StatusTexes[i] != null)
                    {
                        UnityEngine.UI.Text Comp = StatusTexes[i].GetComponent<UnityEngine.UI.Text>();
                        Comp.text = MPs[i].m_ID + ". " + MPs[i].m_Name;

                        if (MPs[i].m_IsLoading)
                        {
                            Comp.text = Comp.text + "(Loading)";
                        }

                        StatusTexes[i].SetActive(true);
                    }
                }
            }
        }
        public static GameObject new_button = null;
        public static GameObject new_button2 = null;
        public static GameObject SleepingButtons = null;
        public static GameObject WaitForSleepLable = null;

        public static void SendMyEQ()
        {
            DataStr.PlayerEquipmentData Edata = new DataStr.PlayerEquipmentData();
            Edata.m_HasAxe = MyHasAxe;
            Edata.m_HasMedkit = MyHasMedkit;
            Edata.m_HasRevolver = MyHasRevolver;
            Edata.m_HasRifle = MyHasRifle;
            Edata.m_Arrows = MyArrows;
            Edata.m_Flares = MyFlares;
            Edata.m_BlueFlares = MyBlueFlares;

            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.EQUIPMENT))
                {
                    _packet.Write(Edata);
                    SendUDPData(_packet);
                }
            }
            if (iAmHost == true)
            {
                using (Packet _packet = new Packet((int)ServerPackets.EQUIPMENT))
                {
                    ServerSend.EQUIPMENT(0, Edata, true);
                }
            }
        }

        public static bool HasWaitForConnect = false;
        //Legacy UI
        public static InterfaceManager m_InterfaceManager;
        public static Panel_SelectSurvivor m_Panel_SelectSurvivor;
        public static Panel_MainMenu m_Panel_MainMenu;
        public static Panel_SelectRegion m_Panel_SelectRegion;
        public static Panel_Sandbox m_Panel_Sandbox;
        public static Panel_ChooseSandbox m_Panel_ChooseSandbox;
        public static Panel_ChallengeComplete m_Panel_ChallengeComplete;

        public static void DoWaitForConnect(bool Steam = false)
        {
            if (HasWaitForConnect == false)
            {
                if (m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
                {
                    if (m_Panel_MainMenu != null)
                    {
                        if (m_Panel_MainMenu != null && m_Panel_MainMenu.m_Sprite_FadeOverlay != null)
                        {
                            m_Panel_MainMenu.m_Sprite_FadeOverlay.gameObject.SetActive(false);
                        }
                    }
                    string Txt;

                    if (Steam)
                    {
                        Txt = "\nPlease wait a moment...";
                    } else {
                        if (DefaultIsRussian)
                        {
                            Txt = "\nЕсли вы используйте hamachi/radmin или другой эмулятор локалки, и у вас проблемы с подключением, не простите о помощи нас.";
                        } else {
                            Txt = "\nPlease wait, if you using hamachi/radmin or any other lan emulator and you have connection problems, please don't ask us for help.";
                        }
                    }


                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Waiting, "Connecting...", Txt, Panel_Confirmation.ButtonLayout.Button_0, Panel_Confirmation.Background.Transperent, null, null);
                    HasWaitForConnect = true;
                }
            }
        }

        public enum ResendPacketType
        {
            None = 0,
            Scene = 1,
            Container = 2,
            GearPlace = 3,
            Examine = 4,
            GearPickup = 5,
            ServerRestart = 6,
            ExpeditionInvites = 7,
        }
        public static ResendPacketType LastPacketForRepeat = ResendPacketType.None;
        public static int SecondsLeftUntilWorryAboutPacket = -1;
        public static int ResendAttempts = 0;
        public static string ResendGearLvlKey = "";
        public static int ResendGearSearchKey = 0;
        public static void SetRepeatPacket(ResendPacketType Pak, int TimeOut = -1)
        {
            LastPacketForRepeat = Pak;

            if (TimeOut == -1)
            {
                TimeOut = TimeToWorryAboutLastRequest;
            }

            SecondsLeftUntilWorryAboutPacket = TimeOut;
        }
        public static void DiscardRepeatPacket()
        {
            SecondsLeftUntilWorryAboutPacket = -1;
            LastPacketForRepeat = ResendPacketType.None;
            ResendGearLvlKey = "";
            ResendGearSearchKey = 0;
            ResendAttempts = 0;
        }

        public static void SimulateAnotherRequest(ResendPacketType request)
        {
            if (request == ResendPacketType.Scene)
            {
                Pathes.RequestDropsForScene();
            }
            else if (request == ResendPacketType.Examine)
            {
                DiscardRepeatPacket();
                RemovePleaseWait();
                HUDMessage.AddMessage("Diagnosis failed!");
            }
            else if (request == ResendPacketType.GearPlace)
            {
                using (Packet _packet = new Packet((int)ClientPackets.REQUESTPLACE))
                {
                    _packet.Write(ResendGearSearchKey);
                    _packet.Write(ResendGearLvlKey);
                    SetRepeatPacket(ResendPacketType.GearPlace);

                    SendUDPData(_packet);
                }
            }
            else if (request == ResendPacketType.GearPickup)
            {
                using (Packet _packet = new Packet((int)ClientPackets.REQUESTPICKUP))
                {
                    _packet.Write(ResendGearSearchKey);
                    _packet.Write(ResendGearLvlKey);
                    SetRepeatPacket(ResendPacketType.GearPickup);
                    SendUDPData(_packet);
                }
            } 
            else if(request == ResendPacketType.ServerRestart)
            {
                using (Packet _packet = new Packet((int)ClientPackets.RESTART))
                {
                    SetRepeatPacket(ResendPacketType.ServerRestart);
                    SendUDPData(_packet);
                }
            }
            else if(request == ResendPacketType.ExpeditionInvites)
            {
                using (Packet _packet = new Packet((int)ClientPackets.REQUESTEXPEDITIONINVITES))
                {
                    SetRepeatPacket(ResendPacketType.ExpeditionInvites);
                    _packet.Write(true);
                    SendUDPData(_packet);
                }
            }
            else
            {
                MelonLogger.Msg("Can't simulate request " + request);
                DiscardRepeatPacket();
                DoPleaseWait("Request failed", "Something wrong with #" + request + " request, please reconnect to the server!");
            }
        }

        public static void RepeatLastRequest()
        {
            GameManager.CancelPendingSave();
            ResendAttempts++;
            RemovePleaseWait();

            if (ResendAttempts >= 11)
            {
                QuitWithoutSaving = true;
                GameManager.CancelPendingSave();
                if (m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
                {
                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, "DISCONNECT TIMEOUT", "\n" + "Host did not responced after 10 Attempts, you can safly exit the game.\nGame won't be saves to keep save valid for host on reconnection!", Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null, null);
                }
                return;
            }


            if (LastPacketForRepeat != ResendPacketType.None)
            {
                SlicedJsonDataBuffer.Clear();
                SecondsLeftUntilWorryAboutPacket = TimeToWorryAboutLastRequest;

                string title;
                string text;
                
                
                if(LastPacketForRepeat != ResendPacketType.ServerRestart)
                {
                    title = "Something wrong";
                    text = "Timed out on response from host for last request. Trying to repeat request to correct the situation, please wait...\nAttempt #" + ResendAttempts;
                }
                else
                {
                    title = "Waiting for server restart";
                    text = "Please wait...\nPing Attempt #" + ResendAttempts;
                }
                DoPleaseWait(title, text);

                SimulateAnotherRequest(LastPacketForRepeat);
            }
        }

        public static void DoPleaseWait(string title, string text)
        {
            if (m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
            {
                InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Waiting, title, "\n" + text, Panel_Confirmation.ButtonLayout.Button_0, Panel_Confirmation.Background.Transperent, null, null);
            }
        }
        public static void RemovePleaseWait()
        {
            if (m_InterfaceManager != null && m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
            {
                InterfaceManager.m_Panel_Confirmation.OnCancel();
            }
        }

        public static void RemoveWaitForConnect()
        {
            HasWaitForConnect = false;
            if (m_InterfaceManager != null && m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
            {
                InterfaceManager.m_Panel_Confirmation.OnCancel();
            }
        }
        public static void DoKickMessage(string txt)
        {
            RemoveWaitForConnect();
            if (m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
            {
                InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, "DISCONNECTED", "\n" + txt, Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null, null);
            }

            if(level_name != "MainMenu")
            {
                QuitWithoutSaving = true;
                GameManager.CancelPendingSave();
            }

            MelonLogger.Msg("Kicked by server, message from host: " + txt);
            ClientUser.LastConnectedIp = "";
            Disconnect();
        }

        public static void CancleDismantling()
        {
            DataStr.ShowShelterByOther FindData = new DataStr.ShowShelterByOther();

            Panel_SnowShelterInteract panel = InterfaceManager.m_Panel_SnowShelterInteract;
            if (panel.m_SnowShelter != null && panel.m_SnowShelter.gameObject != null)
            {
                FindData.m_Position = panel.m_SnowShelter.gameObject.transform.position;
                FindData.m_Rotation = panel.m_SnowShelter.gameObject.transform.rotation;
                FindData.m_LevelID = levelid;
                FindData.m_LevelGUID = level_guid;

                if (panel.m_IsDismantling == true)
                {
                    for (int i = 0; i < playersData.Count; i++)
                    {
                        if (playersData[i] != null)
                        {
                            DataStr.ShowShelterByOther shelter = MyMod.playersData[i].m_Shelter;
                            if (level_guid == shelter.m_LevelGUID && levelid == shelter.m_LevelID && FindData.m_Position == shelter.m_Position)
                            {
                                panel.OnCancel();
                                HUDMessage.AddMessage(playersData[i].m_Name + " INSIDE, CAN'T DISMANTLE THIS!");
                                return;
                            }
                        }
                    }
                }
            }
        }

        public static void PlayVoiceFromPlayerObject(AudioSource audioSource, byte[] voiceData, bool RadioEffect = false)
        {
            if (audioSource != null)
            {
                audioSource.clip = AudioClip.Create(UnityEngine.Random.Range(100, 1000000).ToString(), 22050, 1, 22050, false);
                float[] DecodedData = new float[22050];
                for (int i = 0; i < DecodedData.Length; i++)
                {
                    float value = BitConverter.ToInt16(voiceData, i * 2);
                    DecodedData[i] = (float)value / short.MaxValue;
                }
                audioSource.clip.SetData(DecodedData, 0);
                audioSource.Play();
            }
        }

        public static void ProcessRadioChatData(byte[] CompressedData, uint WriteSize, float RecordTime)
        {
            byte[] decompressedData = SteamConnect.Main.DecompressVoice(CompressedData, WriteSize);
            if (decompressedData != null)
            {
                if (MyRadioAudio != null)
                {
                    Comps.MultiplayerPlayerVoiceChatPlayer mPVoice = MyRadioAudio.transform.GetChild(1).gameObject.GetComponent<Comps.MultiplayerPlayerVoiceChatPlayer>();
                    DataStr.VoiceChatQueueElement DataForQueue = new DataStr.VoiceChatQueueElement();
                    DataForQueue.m_VoiceData = decompressedData;
                    DataForQueue.m_Length = RecordTime;
                    mPVoice.VoiceQueue.Add(DataForQueue);
                }
            }
        }

        public static void ProcessVoiceChatData(int from, byte[] CompressedData, uint WriteSize, float RecordTime, bool Radio = false)
        {
            byte[] decompressedData = SteamConnect.Main.DecompressVoice(CompressedData, WriteSize);

            if (decompressedData != null)
            {
                if (!MyMod.DedicatedServerAppMode && players[from] != null)
                {
                    if (playersData[from].m_Levelid == levelid && playersData[from].m_LevelGuid == level_guid)
                    {
                        Comps.MultiplayerPlayerVoiceChatPlayer mPVoice;
                        if (!Radio)
                        {
                            mPVoice = players[from].transform.GetChild(4).GetChild(1).gameObject.GetComponent<Comps.MultiplayerPlayerVoiceChatPlayer>();
                        } else {
                            mPVoice = players[from].transform.GetChild(4).GetChild(2).gameObject.GetComponent<Comps.MultiplayerPlayerVoiceChatPlayer>();
                        }
                        if (mPVoice)
                        {
                            DataStr.VoiceChatQueueElement DataForQueue = new DataStr.VoiceChatQueueElement();
                            DataForQueue.m_VoiceData = decompressedData;
                            DataForQueue.m_Length = RecordTime;
                            mPVoice.VoiceQueue.Add(DataForQueue);
                        }
                    }
                }
            }
        }

        public static byte[] Compress(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }

        public static float RecordStartTime;
        public static float RecordReleseButtonHold;

        public static void PlayRadioOver()
        {
            if (MyRadioAudio == null)
            {
                return;
            }
            GameObject Generic = MyRadioAudio.transform.GetChild(0).gameObject;
            AudioSource AudioSo = Generic.GetComponent<AudioSource>();
            AudioClip LoadedAssets = LoadedBundle.LoadAsset<AudioClip>("RadioOver");

            if (!AudioSo.isPlaying)
            {
                AudioSo.PlayOneShot(LoadedAssets);
            }
        }
        //public static void PlayChangeFrequency()
        //{
        //    if (MyRadioAudio == null)
        //    {
        //        return;
        //    }
        //    GameObject Generic = MyRadioAudio.transform.GetChild(0).gameObject;
        //    AudioSource AudioSo = Generic.GetComponent<AudioSource>();
        //    AudioClip LoadedAssets = LoadedBundle.LoadAsset<AudioClip>("RadioOver");
        //    AudioSo.PlayOneShot(LoadedAssets);
        //}

        public static void TrackWhenRecordOver(bool Radio)
        {
            if (GameManager.m_PlayerObject == null)
            {
                return;
            }
            bool PreviousRecord = DoingRecord;
            DoingRecord = KeyboardUtilities.InputManager.GetKey(KeyCode.V);

            if ((chatInput != null && chatInput.gameObject.activeSelf == true) || (uConsole.m_Instance != null && uConsole.m_On == true))
            {
                DoingRecord = false;
            }

            if (PreviousRecord != DoingRecord)
            {
                if (DoingRecord == false)
                {
                    RecordReleseButtonHold = Time.time + 0.5f;
                    if (ViewModelRadio && ViewModelRadio.activeSelf)
                    {
                        //PlayRadioOver();
                    }
                } else {
                    RecordStartTime = Time.time;
                }
            }
            SteamConnect.Main.VoiceUpdate();

            if (MicrophoneIdicator != null)
            {
                UnityEngine.UI.Image Img = MicrophoneIdicator.GetComponent<UnityEngine.UI.Image>();
                float SetA = 1f;
                if (!DoingRecord)
                {
                    float CurrA = Img.color.a;
                    SetA = Mathf.Lerp(CurrA, 0, 5 * Time.deltaTime);
                }
                else if (Radio)
                {
                    SetA = 0;
                }
                Img.color = new Color(Img.color.r, Img.color.g, Img.color.b, SetA);
            }

            if (RadioIdicator != null)
            {
                UnityEngine.UI.Image Img = RadioIdicator.GetComponent<UnityEngine.UI.Image>();
                float SetA = 1f;
                if (!DoingRecord)
                {
                    float CurrA = Img.color.a;
                    SetA = Mathf.Lerp(CurrA, 0, 5 * Time.deltaTime);
                }
                else if (!Radio)
                {
                    SetA = 0;
                }
                Img.color = new Color(Img.color.r, Img.color.g, Img.color.b, SetA);
                UnityEngine.UI.Text text = RadioIdicator.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>();
                text.color = new Color(text.color.r, text.color.g, text.color.b, SetA);
                text.text = GetRadioFrequency(RadioFrequency);
            }
        }

        public static void SendMyRadioAudio(byte[] CompressedArray, int ArraySize, float RecordTime, int Sender)
        {
            if (sendMyPosition == true)
            {
                if (ConnectedSteamWorks == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.VOICECHAT))
                    {
                        _packet.Write(ArraySize);
                        _packet.Write(CompressedArray.Length);
                        _packet.Write(CompressedArray);
                        _packet.Write(RecordTime);
                        _packet.Write(Sender);
                        SendUDPData(_packet);
                    }
                }
            }
        }

        public static void SendMyVoice(byte[] CompressedArray, int ArraySize, float RecordTime)
        {
            if (iAmHost == true)
            {
                if (Server.UsingSteamWorks == true)
                {
                    ServerSend.VOICECHAT(0, CompressedArray, ArraySize, RecordTime, level_guid, RadioFrequency, 0);
                }
            }
            if (sendMyPosition == true)
            {
                if (ConnectedSteamWorks == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.VOICECHAT))
                    {
                        _packet.Write(ArraySize);
                        _packet.Write(CompressedArray.Length);
                        _packet.Write(CompressedArray);
                        _packet.Write(RecordTime);
                        _packet.Write(ClientUser.myId);
                        SendUDPData(_packet);
                    }
                }
            }
        }

        public static void FinishedLoading()
        {
            RegularUpdateSeconds = 2;
            if (HostFromLobbyAfterLoader)
            {
                HostFromLobbyAfterLoader = false;
                Server.StartSteam(MaxPlayers);
            }
            if (StartServerAfterSelectSave)
            {
                StartServerAfterSelectSave = false;
                Shared.HostAServer(PortsToHostOn);
            }
        }


        public static void DedicatedServerUpdate()
        {
            if(InterfaceManager.m_Panel_Loading != null)
            {
                Panel_Loading Panel = InterfaceManager.m_Panel_Loading;

                Panel.m_HoldScreenAfterLoad = false;
                Panel.m_ContinueToGame = true;
                Panel.m_ShowQuoteAfterLoad = false;
            }
            
            
            if (GameManager.m_PlayerManager != null)
            {
                GameManager.m_PlayerManager.m_God = true;
                GameManager.m_PlayerManager.m_Ghost = true;
                GameManager.m_PlayerObject.transform.position = new Vector3(0, -300, 0);
                IsDead = true;
                GameAudioManager.SetRTPCValue(AK.GAME_PARAMETERS.GLOBALVOLUME, 0 * 100f, (GameObject)null);
            }
        }

        public static DataStr.FireSourcesSync DebugFireSource = null;

        public static string CloneTrimer(string name)
        {
            string r = name;
            if (name.Contains("(Clone)")) //If it has ugly (Clone), cutting it.
            {
                int L = name.Length - 7;
                r = name.Remove(L, 7);
            }
            if (name.Contains(" ")) //If it has ugly (1) (2) (3), cause of hinderlands's copypaste, cutting it.
            {
                char sperator = Convert.ToChar(" ");
                string[] slices = name.Split(sperator);
                r = slices[0];
            }
            return r;
        }

        public static bool SnareCanTrap(Vector3 pos, string prefabname)
        {
            //MelonLogger.Msg(ConsoleColor.Yellow, "Snare looking for region with "+ prefabname + "...");
            SpawnRegionManager SRM = GameManager.GetSpawnRegionManager();
            for (int index = 0; index < SRM.m_SpawnRegions.Count; ++index)
            {
                float dis = Vector3.Distance(pos, SRM.m_SpawnRegions[index].m_Center);
                float mindis = SRM.m_SpawnRegions[index].m_Radius;
                string spPrefab = SRM.m_SpawnRegions[index].m_SpawnablePrefab.name;
                //MelonLogger.Msg(index + ". Distance to snare " + dis + " maximal allowed distance " + mindis+" animal prefab "+ spPrefab);
                if (dis <= mindis && prefabname == spPrefab)
                {
                    //MelonLogger.Msg(ConsoleColor.Yellow, "Snare inside spawnregion " + SRM.m_SpawnRegions[index].gameObject.name);
                    return true;
                }
            }
            //MelonLogger.Msg(ConsoleColor.Red, "Snare cannot catch here! ");
            return false;
        }


        public static void FakeDropItem(int GearID, Vector3 v3, Quaternion rot, int Hash, DataStr.ExtraDataForDroppedGear extra)
        {
            if (DedicatedServerAppMode)
            {
                return;
            }
            
            string gearName = extra.m_GearName;

            if (DroppedGearsObjs.ContainsKey(Hash) == true)
            {
                MelonLogger.Msg(ConsoleColor.Red, "Gear with hash " + Hash + " already exist!");
                return;
            }

            GameObject reference = GetGearItemObject(gearName);

            if (reference == null)
            {
                MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
                return;
            }

            GameObject obj = UnityEngine.Object.Instantiate<GameObject>(reference, v3, rot);

            if (obj != null)
            {
                DisableObjectForXPMode.RemoveDisabler(obj);
                string _DisName = "";
                bool _FakeBed = false;
                GearItem GI = obj.GetComponent<GearItem>();
                if (GI != null)
                {
                    GI.m_RolledSpawnChance = true;
                    GI.m_StartHasBeenCalled = true;
                    GI.m_MarkedForNextUpdateDestroy = false;
                    if (GI.m_FoodItem != null)
                    {
                        if (extra.m_Variant == 1)
                        {
                            if (obj.GetComponent<MeshSwapItem>() != null)
                            {
                                obj.GetComponent<MeshSwapItem>().m_MeshObjOpened.SetActive(true);
                                obj.GetComponent<MeshSwapItem>().m_MeshObjUnopened.SetActive(false);
                            }
                        }
                    }
                    _DisName = GI.m_LocalizedDisplayName.Text();
                }

                if (obj.GetComponent<Bed>() != null)
                {
                    if (extra.m_Variant == 1)
                    {
                        obj.GetComponent<Bed>().SetState(BedRollState.Placed);
                        _FakeBed = true;
                    }
                }
                if (obj.GetComponent<KeroseneLampItem>() != null)
                {
                    if (extra.m_Variant == 1)
                    {
                        MelonLogger.Msg("Ignited lamp dropped");
                        KeroseneLampItem Lamp = obj.GetComponent<KeroseneLampItem>();
                        if (GameManager.GetWeatherComponent().UseOutdoorLightingForLightSources())
                        {
                            Lamp.m_LightOutdoor.enabled = true;
                            Lamp.m_LightIndoor.enabled = false;
                            Lamp.m_LightIndoorCore.enabled = false;
                        } else {
                            Lamp.m_LightIndoor.enabled = true;
                            Lamp.m_LightIndoorCore.enabled = true;
                            Lamp.m_LightOutdoor.enabled = false;
                        }

                        int minutesLeft = extra.m_GoalTime - MinutesFromStartServer;
                        if (minutesLeft <= 0)
                        {
                            obj.transform.GetChild(0).gameObject.SetActive(false);
                        } else {
                            obj.transform.GetChild(0).gameObject.SetActive(true);
                        }

                        TrackableDroppedGearsObjs.Add(Hash, obj);
                    }
                }
                if (obj.GetComponent<SnareItem>() != null)
                {
                    int stateToSent = 0;
                    if (extra.m_Variant == 4)
                    {
                        stateToSent = 1;
                    } else {
                        stateToSent = extra.m_Variant;
                    }

                    obj.GetComponent<SnareItem>().SetState((SnareState)stateToSent);
                }

                foreach (Component Com in obj.GetComponents<Component>())
                {
                    string ComName = Com.GetIl2CppType().Name;
                    if (ComName != BoxCollider.Il2CppType.Name
                        && ComName != SphereCollider.Il2CppType.Name
                        && ComName != CapsuleCollider.Il2CppType.Name
                        && ComName != MeshCollider.Il2CppType.Name
                        && ComName != PhysicMaterial.Il2CppType.Name
                        && ComName != MeshFilter.Il2CppType.Name
                        && ComName != LODGroup.Il2CppType.Name
                        && ComName != Transform.Il2CppType.Name
                        && ComName != Rigidbody.Il2CppType.Name
                        && ComName != MeshRenderer.Il2CppType.Name
                        && ComName != SkinnedMeshRenderer.Il2CppType.Name)
                    {
                        UnityEngine.Object.Destroy(Com);
                    }
                }

                if (!string.IsNullOrEmpty(extra.m_PhotoGUID))
                {
                    MelonLogger.Msg("Photo "+ extra.m_PhotoGUID);
                    Texture2D tex = MPSaveManager.GetPhotoTexture(extra.m_PhotoGUID, extra.m_GearName);
                    if (tex)
                    {
                        obj.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = tex;
                    } else
                    {
                        Shared.RequestPhoto(extra.m_PhotoGUID);
                        MelonLogger.Msg("Don't have texture for it, request from host");
                    }
                }

                Comps.DroppedGearDummy DGD = obj.AddComponent<Comps.DroppedGearDummy>();
                DGD.m_SearchKey = Hash;
                DGD.m_Extra = extra;
                DGD.m_LocalizedDisplayName = _DisName;
                if (_FakeBed)
                {
                    obj.AddComponent<Comps.FakeBed>();
                }
                obj.SetActive(true);
                DroppedGearsObjs.Add(Hash, obj);
            }
            else
            {
                MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
            }
        }

        public static void PlaceDroppedGear(GameObject obj)
        {
            string OriginalName = obj.name;
            string GearName = CloneTrimer(OriginalName).ToLower();
            string GiveGear = CloneTrimer(OriginalName);

            Vector3 v3 = obj.transform.position;
            Quaternion rot = obj.transform.rotation;
            int SearchKey = 0;
            string lvlKey = level_guid;

            LastPickedVisualPosition = obj.transform.position;
            LastPickedVisualRotation = obj.transform.rotation;

            if (obj.GetComponent<Comps.DroppedGearDummy>() != null)
            {
                SearchKey = obj.GetComponent<Comps.DroppedGearDummy>().m_SearchKey;
            } else {
                //MelonLogger.Msg(ConsoleColor.Red, "DroppedGearDummy is not exist by somereason...");


                GearItem GI = obj.GetComponent<GearItem>();

                if (GI != null && !GI.m_BeenInPlayerInventory && !ServerConfig.m_DuppedSpawns)
                {
                    Comps.DropFakeOnLeave DFL = obj.AddComponent<Comps.DropFakeOnLeave>();
                    DFL.m_OldPossition = obj.transform.position;
                    DFL.m_OldRotation = obj.transform.rotation;
                    MelonLogger.Msg("Trying place pre-spawned gear, so dropping fake on cancle");
                }

                return;
            }

            MelonLogger.Msg("Searching for " + GearName + " with hash " + SearchKey);

            if (sendMyPosition == true)
            {
                DoPleaseWait("Downloading Gear", "Please wait..");
                using (Packet _packet = new Packet((int)ClientPackets.REQUESTPLACE))
                {
                    _packet.Write(SearchKey);
                    _packet.Write(lvlKey);
                    ResendGearSearchKey = SearchKey;
                    ResendGearLvlKey = lvlKey;
                    SetRepeatPacket(ResendPacketType.GearPlace);

                    SendUDPData(_packet);
                }
                return;
            }

            DataStr.SlicedJsonDroppedGear DataProxy = MPSaveManager.RequestSpecificGear(SearchKey, level_guid, true);
            if (DataProxy != null)
            {
                MelonLogger.Msg("Found " + SearchKey);

                string gearName = GiveGear;

                if (gearName == "gear_knife")
                {
                    if (Supporters.ConfiguratedBenefits.m_Knife)
                    {
                        gearName = "GEAR_JeremiahKnife";
                    }
                }

                GameObject reference = GetGearItemObject(gearName);

                if (reference == null)
                {
                    MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
                    return;
                }

                GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(reference, v3, rot);
                DisableObjectForXPMode.RemoveDisabler(newGear.gameObject);

                DataStr.ExtraDataForDroppedGear Extra = obj.GetComponent<Comps.DroppedGearDummy>().m_Extra;

                if (newGear.GetComponent<KeroseneLampItem>() != null && obj != null)
                {
                    int minutesDroped = MinutesFromStartServer - Extra.m_DroppedTime;
                    OverrideLampReduceFuel = minutesDroped;
                    MelonLogger.Msg(ConsoleColor.Cyan, "Lamp been dropped " + minutesDroped + " minutes");
                }

                if (newGear == null)
                {
                    MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
                    return;
                }

                newGear.name = CloneTrimer(newGear.name);
                if (!string.IsNullOrEmpty(DataProxy.m_Json))
                {
                    newGear.GetComponent<GearItem>().Deserialize(DataProxy.m_Json);
                }

                if (!string.IsNullOrEmpty(Extra.m_PhotoGUID))
                {
                    GearItem gi = newGear.GetComponent<GearItem>();
                    if (gi.m_ObjectGuid == null)
                    {
                        string GUID = Extra.m_PhotoGUID;
                        ObjectGuid GUIDComp = newGear.AddComponent<ObjectGuid>();
                        GUIDComp.m_Guid = GUID;
                        gi.m_ObjectGuid = GUIDComp;
                        MelonLogger.Msg("Going render photo " + GUID);
                        Texture2D tex = MPSaveManager.GetPhotoTexture(GUID, gi.m_GearName);
                        if (tex)
                        {
                            newGear.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = tex;
                        }
                    }
                }

                newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;

                Comps.DropFakeOnLeave DFL = newGear.AddComponent<Comps.DropFakeOnLeave>();
                DFL.m_OldPossition = newGear.gameObject.transform.position;
                DFL.m_OldRotation = newGear.gameObject.transform.rotation;
                newGear.GetComponent<GearItem>().PlayPickUpClip();
                GameManager.GetPlayerManagerComponent().StartPlaceMesh(newGear, PlaceMeshFlags.None);
                DroppedGearsObjs.Remove(SearchKey);
                TrackableDroppedGearsObjs.Remove(SearchKey);
                UnityEngine.Object.Destroy(obj);
                ServerSend.PICKDROPPEDGEAR(0, SearchKey, true);
            } else {
                MelonLogger.Msg("Gear with hash " + SearchKey + " is missing!");
            }
        }

        public static void UseFakeBed(GameObject obj)
        {
            string OriginalName = obj.name;
            string GearName = CloneTrimer(OriginalName).ToLower();
            Vector3 v3 = obj.transform.position;
            Quaternion rot = obj.transform.rotation;
            int SearchKey = 0;
            string lvlKey = level_guid;

            if (obj.GetComponent<Comps.DroppedGearDummy>() != null)
            {
                SearchKey = obj.GetComponent<Comps.DroppedGearDummy>().m_SearchKey;
            } else {
                MelonLogger.Msg(ConsoleColor.Red, "DroppedGearDummy by somereason...");
                return;
            }

            MelonLogger.Msg("Searching for " + GearName + " with hash " + SearchKey);

            string gearName = GearName;
            GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject(gearName), v3, rot);
            newGear.name = CloneTrimer(newGear.name);
            newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;
            newGear.GetComponent<GearItem>().m_Bed.SetState(BedRollState.Placed);
            newGear.AddComponent<Comps.FakeBedDummy>().m_LinkedFakeObject = obj;
            GameManager.GetPlayerManagerComponent().ProcessBedInteraction(newGear.GetComponent<GearItem>().m_Bed);
        }

        public static void PickupDroppedGear(GameObject obj, bool SkipLocksmith = false)
        {
            string OriginalName = obj.name;
            string GearName = CloneTrimer(OriginalName).ToLower();
            string GearToGive = CloneTrimer(OriginalName);

            if (GearName == "gear_technicalbackpack" && GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_TechnicalBackpack"))
            {
                HUDMessage.AddMessage("You already have technical backpack!");
                return;
            }

            Vector3 v3 = obj.transform.position;
            Quaternion rot = obj.transform.rotation;
            int SearchKey = 0;

            if (obj.GetComponent<Comps.DroppedGearDummy>() != null)
            {
                SearchKey = obj.GetComponent<Comps.DroppedGearDummy>().m_SearchKey;
            } else {
                MelonLogger.Msg(ConsoleColor.Red, "DroppedGearDummy by somereason...");
                return;
            }

            if (!SkipLocksmith && Shared.IsLocksmithItem(GearName) && obj.GetComponent<Comps.DroppedGearDummy>().m_Extra.m_Variant == 4)
            {
                ShowBlankworkingPicker(obj);
                return;
            }

            MelonLogger.Msg("Searching for " + GearName + " with hash " + SearchKey);
            LastPickedVisualPosition = obj.transform.position;
            LastPickedVisualRotation = obj.transform.rotation;
            if (sendMyPosition == true)
            {
                DoPleaseWait("Please wait...", "Downloading gear...");
                using (Packet _packet = new Packet((int)ClientPackets.REQUESTPICKUP))
                {
                    _packet.Write(SearchKey);
                    _packet.Write(level_guid);
                    ResendGearSearchKey = SearchKey;
                    ResendGearLvlKey = level_guid;
                    SetRepeatPacket(ResendPacketType.GearPickup);
                    SendUDPData(_packet);
                }
                return;
            }

            DataStr.SlicedJsonDroppedGear DataProxy = MPSaveManager.RequestSpecificGear(SearchKey, level_guid, true);

            if (DataProxy != null)
            {
                MelonLogger.Msg("Found " + SearchKey);
                string gearName = GearToGive;
                if (gearName == "gear_knife")
                {
                    if (Supporters.ConfiguratedBenefits.m_Knife)
                    {
                        gearName = "GEAR_JeremiahKnife";
                    }
                }

                GameObject reference = GetGearItemObject(gearName);

                if (reference == null)
                {
                    MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
                    return;
                }

                GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(reference, v3, rot);
                DisableObjectForXPMode.RemoveDisabler(newGear.gameObject);

                if (newGear == null)
                {
                    MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
                    return;
                }

                newGear.name = CloneTrimer(newGear.name);
                Comps.DroppedGearDummy DGD = obj.GetComponent<Comps.DroppedGearDummy>();
                if (newGear.GetComponent<KeroseneLampItem>() != null)
                {
                    int minutesDroped = MinutesFromStartServer - DGD.m_Extra.m_DroppedTime;
                    OverrideLampReduceFuel = minutesDroped;
                    MelonLogger.Msg(ConsoleColor.Cyan, "Lamp been dropped " + minutesDroped + " minutes");
                }
                if (!string.IsNullOrEmpty(DataProxy.m_Json))
                {
                    newGear.GetComponent<GearItem>().Deserialize(DataProxy.m_Json);
                }

                if (!string.IsNullOrEmpty(DGD.m_Extra.m_PhotoGUID))
                {
                    GearItem gi = newGear.GetComponent<GearItem>();
                    if (gi.m_ObjectGuid == null)
                    {
                        string GUID = DGD.m_Extra.m_PhotoGUID;
                        ObjectGuid GUIDComp = newGear.AddComponent<ObjectGuid>();
                        GUIDComp.m_Guid = GUID;
                        gi.m_ObjectGuid = GUIDComp;
                        MelonLogger.Msg("Going render photo " + GUID);
                        Texture2D tex = MPSaveManager.GetPhotoTexture(GUID, DGD.m_Extra.m_GearName);
                        if (tex)
                        {
                            newGear.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = tex;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(DGD.m_Extra.m_ExpeditionNote))
                {
                    CreateCustomNote(DGD.m_Extra.m_ExpeditionNote, newGear.GetComponent<GearItem>());
                }

                newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;
                Comps.DropFakeOnLeave DFL = newGear.AddComponent<Comps.DropFakeOnLeave>();
                DFL.m_OldPossition = newGear.gameObject.transform.position;
                DFL.m_OldRotation = newGear.gameObject.transform.rotation;
                if (DGD != null)
                {
                    if (DGD.m_Extra.m_GoalTime != 0 && DGD.m_Extra.m_GoalTime != -1)
                    {
                        GearItem gear = newGear.GetComponent<GearItem>();
                        if (gear.m_EvolveItem != null)
                        {
                            int minutesOnDry = MyMod.MinutesFromStartServer - DGD.m_Extra.m_DroppedTime;

                            gear.m_EvolveItem.m_TimeSpentEvolvingGameHours = (float)minutesOnDry / 60;
                            MelonLogger.Msg(ConsoleColor.Blue, "Saving minutesOnDry " + minutesOnDry);
                            MelonLogger.Msg(ConsoleColor.Blue, "m_TimeSpentEvolvingGameHours " + gear.m_EvolveItem.m_TimeSpentEvolvingGameHours);
                        }
                    }
                }

                bool SkipPickup = false;
                if (newGear.GetComponent<GearItem>().m_Bed != null)
                {
                    newGear.GetComponent<GearItem>().m_Bed.SetState(BedRollState.Rolled);
                    SkipPickup = true;
                }

                if (SkipPickup == false)
                {
                    GameManager.GetPlayerManagerComponent().ProcessInspectablePickupItem(newGear.GetComponent<GearItem>());
                } else {
                    GameManager.GetPlayerManagerComponent().ProcessPickupItemInteraction(newGear.GetComponent<GearItem>(), false, false);
                }
                PatchBookReadTime(newGear.GetComponent<GearItem>());

                DroppedGearsObjs.Remove(SearchKey);
                TrackableDroppedGearsObjs.Remove(SearchKey);
                UnityEngine.Object.Destroy(obj);
                ServerSend.PICKDROPPEDGEAR(0, SearchKey, true);
            } else {
                MelonLogger.Msg("Gear with hash " + SearchKey + " is missing!");
            }
        }

        public static void PickDroppedItem(int Hash, int Picker)
        {
            GameObject gearObj;
            DroppedGearsObjs.TryGetValue(Hash, out gearObj);
            if (gearObj != null)
            {
                DroppedGearsObjs.Remove(Hash);
                TrackableDroppedGearsObjs.Remove(Hash);
                UnityEngine.Object.Destroy(gearObj);
            }
            if (!MyMod.DedicatedServerAppMode && players[Picker] != null && players[Picker].GetComponent<Comps.MultiplayerPlayerAnimator>() != null)
            {
                players[Picker].GetComponent<Comps.MultiplayerPlayerAnimator>().Pickup();
            }
        }

        public static GameObject DiagnosisDummy = null;
        public static int CurePlayerID = 0;

        public static List<DataStr.AffictionSync> BuildMyAfflictionList()
        {
            List<Affliction> Affs = new List<Affliction>();

            Panel_Affliction.RefreshtListOfShownAfflictionTypes();
            foreach (AfflictionType shownAfflictionType in Panel_Affliction.s_ShownAfflictionTypes)
            {
                int afflictionsCount = Panel_Affliction.GetAfflictionsCount(shownAfflictionType);
                for (int localAfflictionIndex = 0; localAfflictionIndex < afflictionsCount; ++localAfflictionIndex)
                {
                    Affliction currentAffliction = Panel_Affliction.GetCurrentAffliction(shownAfflictionType, localAfflictionIndex);
                    Affs.Add(currentAffliction);
                    //MelonLogger.Msg(currentAffliction.m_AfflictionType);
                }
            }
            List<DataStr.AffictionSync> ToReturn = new List<DataStr.AffictionSync>();
            for (int index = 0; index < Affs.Count; ++index)
            {
                DataStr.AffictionSync AffSync = new DataStr.AffictionSync();
                AffSync.m_Type = (int)Affs[index].m_AfflictionType;
                AffSync.m_Location = (int)Affs[index].m_Location;
                if (Affs[index].m_Cause == null)
                {
                    AffSync.m_Case = "Unknown";
                } else {
                    AffSync.m_Case = Affs[index].m_Cause;
                }

                AffSync.m_Case = Affs[index].m_Cause;
                AffSync.m_ShouldBeTreated = Panel_Affliction.CanBeTreated(Affs[index]);
                ToReturn.Add(AffSync);
            }

            return ToReturn;
        }

        public static void SendMyAffictions(int PlayerSendTo, float hp)
        {
            List<DataStr.AffictionSync> Affs = BuildMyAfflictionList();

            if (iAmHost == true)
            {
                ServerSend.SENDMYAFFLCTIONS(PlayerSendTo, Affs, hp, 0);
            }
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.SENDMYAFFLCTIONS))
                {
                    _packet.Write(PlayerSendTo);
                    _packet.Write(Affs.Count);
                    _packet.Write(hp);
                    for (int index = 0; index < Affs.Count; ++index)
                    {
                        _packet.Write(Affs[index]);
                    }
                    SendUDPData(_packet);
                }
            }
        }

        public static void OtherPlayerCuredMyAffiction(DataStr.AffictionSync Aff)
        {
            GameManager.GetPlayerManagerComponent().ApplyConditionOverTimeBuff(10, 0.3f, 0.3f);
            if (Aff.m_Type == (int)AfflictionType.FoodPoisioning)
            {
                GameManager.GetFoodPoisoningComponent().TakeAntibiotics();
                return;
            }
            else if (Aff.m_Type == (int)AfflictionType.Dysentery)
            {
                GameManager.GetDysenteryComponent().TakeAntibiotics();
                GameManager.GetDysenteryComponent().DrinkCleanWater(2f);
                return;
            }
            else if (Aff.m_Type == (int)AfflictionType.SprainedAnkle)
            {
                for (int i = 0; i < GameManager.GetSprainedAnkleComponent().m_Locations.Count; i++)
                {
                    if (GameManager.GetSprainedAnkleComponent().m_Locations[i] == Aff.m_Location)
                    {
                        GameManager.GetSprainedAnkleComponent().ApplyBandage(i);
                        return;
                    }
                }
            }
            else if (Aff.m_Type == (int)AfflictionType.SprainedWrist)
            {
                for (int i = 0; i < GameManager.GetSprainedWristComponent().m_Locations.Count; i++)
                {
                    if (GameManager.GetSprainedWristComponent().m_Locations[i] == Aff.m_Location)
                    {
                        GameManager.GetSprainedWristComponent().ApplyBandage(i);
                        return;
                    }
                }
            }
            else if (Aff.m_Type == (int)AfflictionType.SprainPain)
            {
                for (int i = 0; i < GameManager.GetSprainPainComponent().m_ActiveInstances.Count; i++)
                {
                    if (GameManager.GetSprainPainComponent().m_ActiveInstances[i].m_Location == (AfflictionBodyArea)Aff.m_Location)
                    {
                        GameManager.GetSprainPainComponent().TakePainKillers(i);
                        return;
                    }
                }
            }
            else if (Aff.m_Type == (int)AfflictionType.Burns)
            {
                GameManager.GetBurnsComponent().ApplyBandage();
                GameManager.GetBurnsComponent().TakePainKillers();
                return;
            }
            else if (Aff.m_Type == (int)AfflictionType.BurnsElectric)
            {
                GameManager.GetBurnsElectricComponent().ApplyBandage();
                GameManager.GetBurnsElectricComponent().TakePainKillers();
                return;
            }
            else if (Aff.m_Type == (int)AfflictionType.BloodLoss)
            {
                for (int i = 0; i < GameManager.GetBloodLossComponent().m_Locations.Count; i++)
                {
                    if (GameManager.GetBloodLossComponent().m_Locations[i] == Aff.m_Location)
                    {
                        GameManager.GetBloodLossComponent().ApplyBandage(i);
                        return;
                    }
                }
            }
            else if (Aff.m_Type == (int)AfflictionType.BrokenRib)
            {
                for (int i = 0; i < GameManager.GetBrokenRibComponent().m_Locations.Count; i++)
                {
                    if (GameManager.GetBrokenRibComponent().m_Locations[i] == Aff.m_Location)
                    {
                        GameManager.GetBrokenRibComponent().ApplyBandage(i, 2);
                        GameManager.GetBrokenRibComponent().TakePainKillers(i, 4);
                        return;
                    }
                }
            }
            else if (Aff.m_Type == (int)AfflictionType.InfectionRisk)
            {
                GameManager.GetInfectionRiskComponent().ApplyAntisepticToLocation((AfflictionBodyArea)Aff.m_Location);
                return;
            }
            else if (Aff.m_Type == (int)AfflictionType.Infection)
            {
                for (int i = 0; i < GameManager.GetInfectionComponent().m_Locations.Count; i++)
                {
                    if (GameManager.GetInfectionComponent().m_Locations[i] == (int)Aff.m_Location)
                    {
                        GameManager.GetInfectionComponent().TakeAntibiotics(i);
                        return;
                    }
                }
            }
            else if (Aff.m_Type == (int)AfflictionType.IntestinalParasites)
            {
                GameManager.GetIntestinalParasitesComponent().TakeAntibiotics();
            }
        }
        public static void SendCureAffliction(DataStr.AffictionSync Aff)
        {
            if (DebugDiagnosis == true)
            {
                OtherPlayerCuredMyAffiction(Aff);
            } else {
                if (iAmHost == true)
                {
                    ServerSend.CUREAFFLICTION(CurePlayerID, Aff);
                }
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.CUREAFFLICTION))
                    {
                        _packet.Write(Aff);
                        _packet.Write(CurePlayerID);
                        SendUDPData(_packet);
                    }
                }
            }
        }

        public static void TryToCheckPlayer(int PlayerID) //Examine
        {
            SetRepeatPacket(ResendPacketType.Examine, 10);
            DoPleaseWait("Diagnosis player", "Please wait...");
            MelonLogger.Msg("Trying diagnosis client " + PlayerID);
            if (iAmHost == true)
            {
                ServerSend.TRYDIAGNISISPLAYER(PlayerID, ClientUser.myId);
            }
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.TRYDIAGNISISPLAYER))
                {
                    _packet.Write(PlayerID);
                    SendUDPData(_packet);
                }
            }
        }

        public static bool DebugDiagnosis = false;

        public static void CheckOtherPlayer(List<DataStr.AffictionSync> Affs, int PlayerID, float health)
        {
            if (DiagnosisDummy != null)
            {
                UnityEngine.Object.Destroy(DiagnosisDummy);
            }
            CurePlayerID = PlayerID;
            DiagnosisDummy = new GameObject();
            NPC m_NPC = DiagnosisDummy.AddComponent<NPC>();
            NPCAfflictions m_NPCaff = DiagnosisDummy.AddComponent<NPCAfflictions>();
            NPCCondition m_NPCcon = DiagnosisDummy.AddComponent<NPCCondition>();
            CarryableBody m_BodyCarry = DiagnosisDummy.AddComponent<CarryableBody>();
            NPCThirst m_Thirst = DiagnosisDummy.AddComponent<NPCThirst>();
            NPCVoice m_Voice = DiagnosisDummy.AddComponent<NPCVoice>();
            NPCFreezing m_Cold = DiagnosisDummy.AddComponent<NPCFreezing>();
            m_NPC.m_AfflictionsComponent = m_NPCaff;
            m_NPC.m_Condition = m_NPCcon;
            m_NPC.m_Body = m_BodyCarry;
            m_NPC.m_Thirst = m_Thirst;
            m_NPC.m_Voice = m_Voice;
            m_NPC.m_Freezing = m_Cold;
            m_NPCaff.m_NPC = m_NPC;
            m_Thirst.m_NPC = m_NPC;
            m_Cold.m_NPC = m_NPC;

            m_NPCaff.m_Afflictions.Clear();

            bool HasBloodLoss = false;
            bool HasAnyBadOnes = false;

            for (int i = 0; i < Affs.Count; i++)
            {
                //m_NPCaff.AddAffliction((AfflictionType)Affs[i].m_Type, Affs[i].m_Case, (AfflictionBodyArea)Affs[i].m_Location);

                AfflictionDefinition definitionByType = GameManager.GetAfflictionDefinitionTable().GetAfflictionDefinitionByType((AfflictionType)Affs[i].m_Type);

                NPCAffliction npcAff = new NPCAffliction();
                if (Affs[i].m_ShouldBeTreated == false)
                {
                    npcAff.m_RestHours = 1;
                } else {
                    HasAnyBadOnes = true;
                }
                npcAff.m_Definition = definitionByType;
                npcAff.m_Location = (AfflictionBodyArea)Affs[i].m_Location;
                npcAff.m_CauseLocId = Affs[i].m_Case;

                m_NPCaff.m_Afflictions.Add(npcAff);
                if (definitionByType.m_AfflictionType == AfflictionType.BloodLoss)
                {
                    HasBloodLoss = true;
                }
            }

            m_NPCcon.m_CurrentHP = health;
            m_NPCcon.m_MaxHP = 100;
            Panel_Diagnosis Panel = InterfaceManager.m_Panel_Diagnosis;
            RemovePleaseWait();
            DiscardRepeatPacket();

            Panel.Enable(true, m_NPC);
            GameObject thirstBar = Panel.gameObject.transform.GetChild(2).gameObject;
            GameObject TemertureBar = Panel.gameObject.transform.GetChild(3).gameObject;
            thirstBar.SetActive(false);
            TemertureBar.SetActive(false);

            string Speach = "PLAY_SURVIVORDIAGNOSIS"; // Astrid Default
            if (GameManager.GetPlayerManagerComponent().m_VoicePersona == VoicePersona.Male) // If Makenzy
            {
                if (!HasAnyBadOnes) // All is okay
                {
                    Speach = "PLAY_SNDVOSMMAC1750"; // You ok?
                } else if (HasBloodLoss) // Has blood loss
                {
                    Speach = "PLAY_SNDVOSMMAC1820"; // You’re losing blood. Just stay awake. We’ll get there
                } else { // Anything else
                    List<string> MakenzyDiagnosis = new List<string>();
                    MakenzyDiagnosis.Add("PLAY_SNDVOSMMAC2150"); // Good to see you’re still breathing. I found some supplies that should help.
                    MakenzyDiagnosis.Add("PLAY_SNDVOSMMACE422950_01"); // That doesn't look good...
                    MakenzyDiagnosis.Add("PLAY_SNDVOSMMAC1770"); // What can I do?
                    System.Random rnd = new System.Random();
                    Speach = MakenzyDiagnosis[rnd.Next(MakenzyDiagnosis.Count)];
                }
            }
            GameManager.GetPlayerVoiceComponent().Play(Speach, Voice.Priority.Critical);
        }

        public static int TimeToDry(string gearName)
        {
            if (gearName.Contains("GEAR_WolfPelt"))
            {
                return 10080;
            }
            else if (gearName.Contains("GEAR_BearHide"))
            {
                return 17280;
            }
            else if (gearName.Contains("GEAR_Gut"))
            {
                return 7200;
            }
            else if (gearName.Contains("GEAR_MooseHide"))
            {
                return 14400;
            }
            else if (gearName.Contains("GEAR_LeatherHide"))
            {
                return 7200;
            }
            else if (gearName.Contains("GEAR_RabbitPelt"))
            {
                return 4320;
            }
            else if (gearName.Contains("GEAR_Leather"))
            {
                return 7200;
            }
            else if (gearName.Contains("GEAR_MapleSapling"))
            {
                return 8640;
            }
            else if (gearName.Contains("GEAR_BirchSapling"))
            {
                return 5760;
            }
            else if (gearName.Contains("GEAR_Snare"))
            {
                return 720;
            }

            return 7200;
        }

        public static void BuildStoneStash()
        {

        }

        public static void SendDropItem(GearItem gear, int nums = 0, int total = 0, bool samepose = false, int variant = 0, GameObject Around = null)
        {
            if (gear != null && gear.gameObject != null)
            {
                GameObject obj = gear.gameObject;
                string PhotoHash = "";

                if (samepose == false)
                {
                    if (Around == null)
                    {
                        gear.StickToGroundAtPlayerFeet(GameManager.GetPlayerTransform().position);
                    } else {
                        Vector3 pos = Around.transform.position;
                        float num = UnityEngine.Random.Range(0, 1.1f);
                        Vector3 vector3 = Quaternion.Euler(0.0f, UnityEngine.Random.Range(0, 359), 0.0f) * Vector3.forward;
                        gear.StickToGroundAndOrientOnSlope(pos + vector3 * num, NavMeshCheck.IgnoreNavMesh, 0.5f);
                    }
                } else {
                    if (obj.GetComponent<Comps.DropFakeOnLeave>() != null)
                    {
                        Comps.DropFakeOnLeave DFL = obj.GetComponent<Comps.DropFakeOnLeave>();
                        obj.transform.position = DFL.m_OldPossition;
                        obj.transform.rotation = DFL.m_OldRotation;
                    }
                }

                Vector3 v3 = gear.gameObject.transform.position;
                Quaternion rot = gear.gameObject.transform.rotation;

                if (Shared.IsLocksmithItem(gear.m_GearName.ToLower()))
                {
                    RaycastHit hit;
                    if (gear.m_GearName.ToLower().Contains("gear_scmetalblank"))
                    {
                        obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y + 0.15f, obj.transform.position.z);
                    }
                    variant = 0;
                    if (Physics.Raycast(obj.transform.position, obj.transform.TransformDirection(Vector3.down), out hit, 8))
                    {
                        if (hit.collider)
                        {
                            if (hit.collider.gameObject.GetComponent<WorkBench>())
                            {
                                variant = 4;
                            }
                        }
                    }
                }
                if(IsUserGeneratedHandItem(gear.m_GearName))
                {
                    if (gear.m_ObjectGuid != null && !string.IsNullOrEmpty(gear.m_ObjectGuid.m_Guid))
                    {
                        PhotoHash = gear.m_ObjectGuid.m_Guid;
                        MelonLogger.Msg("Dropped photo " + PhotoHash);
                    }
                }

                string OriginalName = obj.name;
                string GearName = CloneTrimer(OriginalName).ToLower();
                string GearGiveName = CloneTrimer(OriginalName);
                int hashV3 = Shared.GetVectorHash(v3);
                int hashRot = Shared.GetQuaternionHash(rot);
                int hashLevelKey = level_guid.GetHashCode();
                int SearchKey = hashV3 + hashRot + hashLevelKey;
                string LevelKey = level_guid;

                //MelonLogger.Msg("hashGearID " + hashGearID);
                //MelonLogger.Msg("hashV3 " + hashV3);
                //MelonLogger.Msg("hashRot " + hashRot);
                //MelonLogger.Msg("hashLvl " + hashLvl);
                //MelonLogger.Msg("hashLvlGUID " + hashLvlGUID);
                string DataProxy;
                if (nums > 0)
                {
                    if (gear.m_StackableItem != null)
                    {
                        gear.m_StackableItem.m_Units = nums;
                    }
                }

                DataProxy = gear.Serialize();

                int NeedToDry = 0;
                int MinuteToSkip = 0;

                if (gear.m_EvolveItem != null)
                {
                    if (gear.m_EvolveItem.CanEvolve() == true)
                    {
                        int days = Convert.ToInt32(gear.m_EvolveItem.m_TimeToEvolveGameDays);
                        int hours = days * 24;
                        int minutes = hours * 60;

                        if (gear.m_EvolveItem.m_TimeSpentEvolvingGameHours != 0)
                        {
                            MinuteToSkip = (int)gear.m_EvolveItem.m_TimeSpentEvolvingGameHours * 60;
                        }
                        MelonLogger.Msg("m_TimeSpentEvolvingGameHours " + gear.m_EvolveItem.m_TimeSpentEvolvingGameHours);
                        MelonLogger.Msg("MinuteToSkip " + MinuteToSkip);

                        NeedToDry = minutes;
                    } else {
                        NeedToDry = -1;
                    }
                }

                if (gear.m_SnareItem != null)
                {
                    variant = (int)gear.m_SnareItem.m_State;

                    if (variant == 1)
                    {
                        if (SnareCanTrap(v3, gear.m_SnareItem.m_RabbitPrefab.name) == true)
                        {
                            NeedToDry = 720;
                            if (GameManager.GetFeatExpertTrapper().IsUnlockedAndEnabled() == true)
                            {
                                variant = 4;
                            }
                        } else {
                            NeedToDry = -1;
                        }
                    }
                }

                MelonLogger.Msg("Dropping " + GearName + " Hash " + SearchKey + " variant " + variant);

                if (nums == 0)
                {
                    UnityEngine.Object.Destroy(obj);
                } else {
                    if (gear.m_StackableItem != null)
                    {
                        gear.m_StackableItem.m_Units = total - nums;
                        GameManager.GetInventoryComponent().AddGear(obj);
                    }
                }

                DataStr.DroppedGearItemDataPacket SyncData = new DataStr.DroppedGearItemDataPacket();
                SyncData.m_GearID = -1;
                SyncData.m_Position = v3;
                SyncData.m_Rotation = rot;
                SyncData.m_LevelID = levelid;
                SyncData.m_LevelGUID = level_guid;
                SyncData.m_Hash = SearchKey;

                DataStr.ExtraDataForDroppedGear Extra = new DataStr.ExtraDataForDroppedGear();
                Extra.m_Dropper = MyChatName;
                Extra.m_DroppedTime = MinutesFromStartServer - MinuteToSkip;
                Extra.m_Variant = variant;
                Extra.m_PhotoGUID = PhotoHash;
                if (GearGiveName != "")
                {
                    Extra.m_GearName = GearGiveName;
                }

                if (NeedToDry != 0 && NeedToDry != -1)
                {
                    Extra.m_GoalTime = MinutesFromStartServer + NeedToDry;
                } else {
                    Extra.m_GoalTime = NeedToDry;
                }

                if (gear.m_KeroseneLampItem != null)
                {
                    if (variant == 1)
                    {
                        float fuel = gear.m_KeroseneLampItem.m_CurrentFuelLiters;
                        float fuelPerMinute = gear.m_KeroseneLampItem.m_FuelBurnLitersPerHour / 60;
                        float minuteLeft = fuel / fuelPerMinute;

                        Extra.m_GoalTime = MinutesFromStartServer + (int)Math.Round(minuteLeft);
                    } else {
                        Extra.m_GoalTime = 0;
                    }
                }

                SyncData.m_Extra = Extra;

                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.DROPITEM))
                    {
                        _packet.Write(SyncData);
                        SendUDPData(_packet);
                    }

                    byte[] bytesToSlice = Encoding.UTF8.GetBytes(DataProxy);

                    if (bytesToSlice.Length > 500)
                    {
                        List<byte> BytesBuffer = new List<byte>();
                        BytesBuffer.AddRange(bytesToSlice);

                        while (BytesBuffer.Count >= 500)
                        {
                            byte[] sliceOfBytes = BytesBuffer.GetRange(0, 499).ToArray();
                            BytesBuffer.RemoveRange(0, 499);

                            string jsonStringSlice = Encoding.UTF8.GetString(sliceOfBytes);
                            DataStr.SlicedJsonData SlicedPacket = new DataStr.SlicedJsonData();
                            SlicedPacket.m_GearName = LevelKey;
                            SlicedPacket.m_SendTo = -1;
                            SlicedPacket.m_Hash = SearchKey;
                            SlicedPacket.m_Str = jsonStringSlice;

                            if (BytesBuffer.Count != 0)
                            {
                                SlicedPacket.m_Last = false;
                            } else {
                                SlicedPacket.m_Last = true;
                            }
                            AddGearCarefulSlice(SlicedPacket);
                        }

                        if (BytesBuffer.Count < 500 && BytesBuffer.Count != 0)
                        {
                            byte[] LastSlice = BytesBuffer.GetRange(0, BytesBuffer.Count).ToArray();
                            BytesBuffer.RemoveRange(0, BytesBuffer.Count);

                            string jsonStringSlice = Encoding.UTF8.GetString(LastSlice);
                            DataStr.SlicedJsonData SlicedPacket = new DataStr.SlicedJsonData();
                            SlicedPacket.m_GearName = LevelKey;
                            SlicedPacket.m_SendTo = -1;
                            SlicedPacket.m_Hash = SearchKey;
                            SlicedPacket.m_Str = jsonStringSlice;
                            SlicedPacket.m_Last = true;
                            SlicedPacket.m_Extra = Extra;

                            //MelonLogger.Msg(ConsoleColor.Yellow, "Sending slice " + SlicedPacket.m_Hash + " DATA: " + SlicedPacket.m_Str);
                            if (sendMyPosition == true)
                            {
                                AddGearCarefulSlice(SlicedPacket);
                            }
                            SendNextGearCarefulSlice();
                        }
                    } else {
                        DataStr.SlicedJsonData DropPacket = new DataStr.SlicedJsonData();
                        DropPacket.m_GearName = LevelKey;
                        DropPacket.m_SendTo = -1;
                        DropPacket.m_Hash = SearchKey;
                        DropPacket.m_Str = DataProxy;
                        DropPacket.m_Last = true;
                        DropPacket.m_Extra = Extra;
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.GOTDROPSLICE))
                            {
                                _packet.Write(DropPacket);
                                SendUDPData(_packet);
                            }
                        }
                    }
                }
                if (iAmHost == true)
                {
                    Shared.AddDroppedGear(-1, SearchKey, DataProxy, LevelKey, Extra);
                    ServerSend.DROPITEM(0, SyncData, true);
                    Shared.FakeDropItem(SyncData);
                }
            }
        }

        public static void CreateFolderIfNotExist(string path)
        {
            bool exists = Directory.Exists(path);
            if (!exists)
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void FakeClose(OpenClose OpCl)
        {
            if (OpCl.IsOpen() == false)
                return;
            if (OpCl.m_Animator != null)
            {
                OpCl.SetAnimationSpeed(OpCl.m_DefaultOpenCloseSpeedMultiplierValue);
                OpCl.AnimateOpen(false);
            }
            if (OpCl.m_ObjectAnim != null)
            {
                if (!OpCl.m_ObjectAnim.Play(OpCl.m_AnimsReversed ? "open" : "close"))
                    return;
                OpCl.m_Animating = true;
                if (OpCl.m_DisableCollisionDuringAnimation)
                    OpCl.SetCollisionEnabled(false);
            }

            bool sound = true;
            if (OpCl.gameObject != null)
            {
                Vector3 doorV3 = OpCl.gameObject.transform.position;
                Vector3 playerV3 = GameManager.GetPlayerObject().transform.position;
                float dis = Vector3.Distance(playerV3, doorV3);
                if (dis > 50)
                {
                    sound = false;
                }
            }

            if (sound == true)
            {
                int num = (int)GameAudioManager.PlaySound(OpCl.m_CloseAudio, OpCl.gameObject);
            }
            OpCl.m_IsOpen = false;
            MelonLogger.Msg("Forcing openable thing to close");
        }

        public static void FakeOpen(OpenClose OpCl)
        {
            if (OpCl.IsOpen())
                return;
            if (OpCl.m_Safe != null && !OpCl.m_Safe.m_Cracked)
            {
                OpCl.m_Safe.EnableSafeCrackingInterface();
            }
            else
            {
                if (OpCl.m_Animator != null)
                {
                    OpCl.SetAnimationSpeed(OpCl.m_DefaultOpenCloseSpeedMultiplierValue);
                    OpCl.AnimateOpen(true);
                }
                if (OpCl.m_ObjectAnim != null)
                {
                    OpCl.m_AnimsReversed = OpCl.PlayAnimInReverse();
                    if (!OpCl.m_ObjectAnim.Play(OpCl.m_AnimsReversed ? "close" : "open"))
                        return;
                    OpCl.m_Animating = true;
                    if (OpCl.m_DisableCollisionDuringAnimation)
                        OpCl.SetCollisionEnabled(false);
                }

                bool sound = true;
                if (OpCl.gameObject != null)
                {
                    Vector3 doorV3 = OpCl.gameObject.transform.position;
                    Vector3 playerV3 = GameManager.GetPlayerObject().transform.position;
                    float dis = Vector3.Distance(playerV3, doorV3);
                    if (dis > 50)
                    {
                        sound = false;
                    }
                }

                if (sound == true)
                {
                    int num = (int)GameAudioManager.PlaySound(OpCl.m_OpenAudio, OpCl.gameObject);
                }
                OpCl.m_IsOpen = true;
            }
            MelonLogger.Msg("Forcing openable thing to open");
        }


        public static Dictionary<string, GameObject> OpenablesObjs = new Dictionary<string, GameObject>();
        public static void ApplyOpenables()
        {
            if (OpenCloseManager.s_ActiveOpenClosers != null && OpenablesObjs.Count < OpenCloseManager.s_ActiveOpenClosers.Count)
            {
                OpenablesObjs.Clear();
                for (int i = 0; i < OpenCloseManager.s_ActiveOpenClosers.Count; i++)
                {
                    GameObject curObj;
                    OpenClose OpCl = OpenCloseManager.s_ActiveOpenClosers[i];
                    string _Guid = "";
                    if (OpCl != null && OpCl.gameObject != null)
                    {
                        curObj = OpCl.gameObject;
                        if (curObj.GetComponent<ObjectGuid>() != null)
                        {
                            _Guid = curObj.GetComponent<ObjectGuid>().Get();

                            if (OpenablesObjs.ContainsKey(_Guid) == false)
                            {
                                //MelonLogger.Msg(ConsoleColor.Yellow, "[OpenableThingsUnloader] Added " + _Guid);
                                OpenablesObjs.Add(_Guid, curObj);
                            }
                        }
                    }
                }
                //MelonLogger.Msg(ConsoleColor.Blue, "[OpenableThingsUnloader] Finished convert list to dictionary List("+ OpenCloseManager.s_ActiveOpenClosers.Count+ ") Dictionary(" + OpenablesObjs.Count+")");
            }
            foreach (var cur in OpenableThings)
            {
                GameObject Openable;
                if (OpenablesObjs.TryGetValue(cur.Key, out Openable) == true)
                {
                    if (Openable != null)
                    {
                        OpenClose OpCl = Openable.GetComponent<OpenClose>();
                        if (OpCl.IsOpen() != cur.Value)
                        {
                            if (cur.Value == false)
                            {
                                FakeClose(OpCl);
                            } else {
                                FakeOpen(OpCl);
                            }
                        }
                    }
                }
            }
        }

        public static void SendOpenableThing(string Scene, string GUID, bool state)
        {
            using (Packet _packet = new Packet((int)ClientPackets.USEOPENABLE))
            {
                _packet.Write(Scene);
                _packet.Write(GUID);
                _packet.Write(state);

                SendUDPData(_packet);
            }
        }

        public static List<DataStr.SlicedJsonData> CarefulSlicesBuffer = new List<DataStr.SlicedJsonData>();
        public static List<DataStr.SlicedJsonData> CarefulSlicesGearBuffer = new List<DataStr.SlicedJsonData>();
        public static List<DataStr.SlicedJsonData> CarefulSlicesPhotoBuffer = new List<DataStr.SlicedJsonData>();
        public static void AddCarefulSlice(DataStr.SlicedJsonData slice)
        {
            CarefulSlicesBuffer.Add(slice);
        }
        public static void AddGearCarefulSlice(DataStr.SlicedJsonData slice)
        {
            CarefulSlicesGearBuffer.Add(slice);
        }
        public static void AddPhotoCarefulSlice(DataStr.SlicedJsonData slice)
        {
            CarefulSlicesPhotoBuffer.Add(slice);
        }
        public static int CarefulSlicesSent = 0;
        public static void SendNextCarefulSlice()
        {
            if (CarefulSlicesBuffer.Count > 0)
            {
                DataStr.SlicedJsonData slice = CarefulSlicesBuffer[0];
                using (Packet _packet = new Packet((int)ClientPackets.GOTCONTAINERSLICE))
                {
                    _packet.Write(slice);
                    SendUDPData(_packet);
                }
                CarefulSlicesBuffer.Remove(CarefulSlicesBuffer[0]);
                CarefulSlicesSent++;
            } else {
                MelonLogger.Msg("Finished sending all " + CarefulSlicesSent + " slices");
                CarefulSlicesSent = 0;
            }
        }
        public static void SendNextGearCarefulSlice()
        {
            if (CarefulSlicesGearBuffer.Count > 0)
            {
                DataStr.SlicedJsonData slice = CarefulSlicesGearBuffer[0];
                using (Packet _packet = new Packet((int)ClientPackets.GOTDROPSLICE))
                {
                    _packet.Write(slice);
                    SendUDPData(_packet);
                }
                CarefulSlicesGearBuffer.Remove(CarefulSlicesGearBuffer[0]);
                CarefulSlicesSent++;
            } else
            {
                MelonLogger.Msg("Finished sending all " + CarefulSlicesSent + " slices");
                CarefulSlicesSent = 0;
            }
        }

        public static void SendNextPhotoCarefulSlice()
        {
            if (CarefulSlicesPhotoBuffer.Count > 0)
            {
                DataStr.SlicedJsonData slice = CarefulSlicesPhotoBuffer[0];
                using (Packet _packet = new Packet((int)ClientPackets.GOTPHOTOSLICE))
                {
                    _packet.Write(slice);
                    SendUDPData(_packet);
                }
                CarefulSlicesPhotoBuffer.Remove(CarefulSlicesPhotoBuffer[0]);
                CarefulSlicesSent++;
            } else
            {
                MelonLogger.Msg("Finished sending all " + CarefulSlicesSent + " slices");
                CarefulSlicesSent = 0;
            }
        }

        public static void FinishOpeningFakeContainer(string CompressedData)
        {
            MelonLogger.Msg("Finish Opening Fake Container");
            string Data = "";
            if (CompressedData != "")
            {
                Data = Shared.DecompressString(CompressedData);
            }

            Container box = GoingToOpenContinaer;
            GameManager.GetPlayerManagerComponent().SetControlMode(box.m_RestoreControlMode);
            GameManager.GetPlayerManagerComponent().m_ContainerBeingSearched = null;
            InterfaceManager.m_Panel_HUD.CancelItemProgressBar();
            box.m_SearchInProgress = false;
            box.m_OpenInProgress = false;
            box.m_Inspected = true;
            box.m_StartInspected = true;
            if (Data == "")
            {
                MelonLogger.Msg("Opening empty");
                box.DestroyAllGear();
            } else {
                MelonLogger.Msg("Opening with loot, loading loot to the container");
                Il2CppSystem.Collections.Generic.List<GearItem> loadedlist = new Il2CppSystem.Collections.Generic.List<GearItem>();
                box.Deserialize(Data, loadedlist);
            }
            MelonLogger.Msg("All done, removing hint");
            //box.m_CapacityKG = 1000f;
            RemovePleaseWait();
            InterfaceManager.m_Panel_Container.SetContainer(box, box.m_LocalizedDisplayName.Text());
            InterfaceManager.m_Panel_Container.Enable(true);
            box.m_Inspected = true;
            box.m_StartInspected = true;
            GoingToOpenContinaer = null;
        }

        public static Container GoingToOpenContinaer = null;

        public static void OpenFakeContainer(Container box)
        {
            string boxGUID = box.GetComponent<ObjectGuid>().Get();

            if (sendMyPosition == true)
            {
                GoingToOpenContinaer = box;
                DoPleaseWait("Please wait...", "Downloading container data...");
                using (Packet _packet = new Packet((int)ClientPackets.REQUESTOPENCONTAINER))
                {
                    _packet.Write(level_guid);
                    _packet.Write(boxGUID);
                    SetRepeatPacket(ResendPacketType.Container);
                    SendUDPData(_packet);
                }
                return;
            }

            string CompressedData = MPSaveManager.LoadContainer(level_guid, boxGUID);
            string Data = "";
            if (CompressedData != "")
            {
                Data = Shared.DecompressString(CompressedData);
            }

            GameManager.GetPlayerManagerComponent().SetControlMode(box.m_RestoreControlMode);
            GameManager.GetPlayerManagerComponent().m_ContainerBeingSearched = null;
            InterfaceManager.m_Panel_HUD.CancelItemProgressBar();
            box.m_SearchInProgress = false;
            box.m_OpenInProgress = false;
            box.m_Inspected = true;
            box.m_StartInspected = true;

            if (Data == "")
            {
                box.DestroyAllGear();
            } else {
                Il2CppSystem.Collections.Generic.List<GearItem> loadedlist = new Il2CppSystem.Collections.Generic.List<GearItem>();
                box.Deserialize(Data, loadedlist);
            }
            box.m_Inspected = true;
            box.m_StartInspected = true;
            //box.m_CapacityKG = 1000f;
            InterfaceManager.m_Panel_Container.SetContainer(box, box.m_LocalizedDisplayName.Text());
            InterfaceManager.m_Panel_Container.Enable(true);
        }

        public static void CloseFakeContainer(Container box)
        {
            string boxGUID = box.GetComponent<ObjectGuid>().Get();
            box.m_Inspected = true;
            box.m_StartInspected = true;
            string Data = box.Serialize();
            string CompressedData = Shared.CompressString(Data);

            if (!string.IsNullOrEmpty(DeBugMenu.ContainerOverride))
            {
                CompressedData = DeBugMenu.ContainerOverride;
            }
            bool IsEmpty;
            bool IsDeathCreate = false;
            int State = 0;

            if (box.gameObject.GetComponent<Comps.DeathDropContainer>() != null)
            {
                if (box.m_Items != null && box.m_Items.Count <= 2)
                {
                    for (int i = box.m_Items.Count - 1; i >= 0; i--)
                    {
                        GearItemObject item = box.m_Items[i];
                        if (item.m_GearItem && item.m_GearItem.m_WaterSupply && item.m_GearItem.m_WaterSupply.m_VolumeInLiters == 0)
                        {
                            box.RemoveGear(item.m_GearItem);
                        }
                    }
                }
                IsDeathCreate = true;
            }
            IsEmpty = box.IsEmpty();
            box.DestroyAllGear();
            if (!IsEmpty)
            {
                State = 1;
            }
            MelonLogger.Msg("[CloseFakeContainer] " + boxGUID + " Is Empty " + IsEmpty);
            int Bags = GameManager.GetInventoryComponent().GetNumGearWithName("GEAR_TechnicalBackpack");
            if (Bags > 1)
            {
                IL2CPP.List<GearItemObject> GI = GameManager.GetInventoryComponent().m_Items;
                for (int i = 0; i < GI.Count; i++)
                {
                    if (GI[i].m_GearItemName == "GEAR_TechnicalBackpack")
                    {
                        GI[i].m_GearItem.Drop(1);
                        break;
                    }
                }
            }

            DataStr.ContainerOpenSync BoxSync = new DataStr.ContainerOpenSync();
            BoxSync.m_Guid = boxGUID;
            BoxSync.m_LevelGUID = level_guid;
            BoxSync.m_Inspected = true;

            Shared.AddLootedContainer(BoxSync, true, ClientUser.myId, State);
            if (box != null)
            {
                if (box.GetComponent<Comps.ContainersSync>() != null)
                {
                    box.GetComponent<Comps.ContainersSync>().m_Empty = IsEmpty;
                }
            }

            if (IsEmpty)
            {
                MelonLogger.Msg("[CloseFakeContainer] Removing container");

                if (IsDeathCreate)
                {
                    RemoveDeathContainer(boxGUID, level_guid);
                }
                if (sendMyPosition)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.DEATHCREATEEMPTYNOW))
                    {
                        _packet.Write(boxGUID);
                        _packet.Write(level_guid);
                        _packet.Write(IsDeathCreate);
                        SendUDPData(_packet);
                    }
                } else
                {
                    MPSaveManager.RemoveContainer(level_guid, boxGUID);
                }
            } else 
            {
                MelonLogger.Msg("[CloseFakeContainer] Saving container");
                if (sendMyPosition == true)
                {
                    DoPleaseWait("Please wait...", "Sending container data...");
                    Shared.SendContainerData(CompressedData, level_guid, boxGUID, Data);
                    return;
                }
                MPSaveManager.SaveContainer(level_guid, boxGUID, CompressedData);
            }
            if (box != null)
            {
                box.Close();
                if (box.m_CloseAudio.Length == 0)
                {
                    GameAudioManager.PlayGUIButtonBack();
                }
                if(box.GetComponent<Comps.ContainersSync>() != null)
                {
                    box.GetComponent<Comps.ContainersSync>().m_Empty = IsEmpty;
                }
            }
            GameManager.GetPlayerManagerComponent().MaybeRevealPolaroidDiscoveryOnClose();
            InterfaceManager.m_Panel_Container.Enable(false);
        }

        public static void InitAudio()
        {
            Camera Cam = GameManager.GetMainCamera();

            if (Cam)
            {
                if (Cam.GetComponent<AudioListener>())
                {
                    Cam.gameObject.AddComponent<AudioListener>();
                }
            }
        }

        public static void UpdateAPIStates()
        {
            if (iAmHost == true)
            {
                API.m_ClientState = API.SkyCoopClientState.HOST;
            }
            else if (sendMyPosition == true)
            {
                API.m_ClientState = API.SkyCoopClientState.CLIENT;
            } else {
                API.m_ClientState = API.SkyCoopClientState.NONE;
            }
            API.m_MyClientID = ClientUser.myId;

            if (iAmHost)
            {
                API.IsP2PServer = Server.UsingSteamWorks;
            }else if (sendMyPosition)
            {
                API.IsP2PServer = ConnectedSteamWorks;
            }
            API.Port = Server.Port;
        }

        public static void BuildSleepScreen()
        {
            if (SleepingButtons == null)
            {
                SleepingButtons = InterfaceManager.m_Panel_Rest.gameObject.transform.GetChild(3).gameObject;
                WaitForSleepLable = UnityEngine.Object.Instantiate(SleepingButtons.transform.GetChild(2).GetChild(1).gameObject, InterfaceManager.m_Panel_Rest.gameObject.transform);
                UnityEngine.Object.Destroy(WaitForSleepLable.GetComponent<UILocalize>());
                WaitForSleepLable.GetComponent<UILabel>().text = "WAITING FOR OTHER PLAYERS TO SLEEP";
                WaitForSleepLable.SetActive(false);
            }

            if (new_button == null)
            {
                new_button = UnityEngine.Object.Instantiate(InterfaceManager.m_Panel_Rest.m_SleepButton, InterfaceManager.m_Panel_Rest.m_SleepButton.transform.parent);

                if (new_button2 == null)
                {
                    new_button.transform.position = new Vector3(0, -0.59f, 0);
                } else {
                    new_button.transform.position = new Vector3(-0.2777778f, -0.59f, 0);
                }

                new_button.name = "WaitForEveryoneButton";
                GameObject Labl = new_button.transform.GetChild(0).GetChild(0).gameObject;
                UnityEngine.Object.Destroy(Labl.GetComponent<UILocalize>());
                Labl.GetComponent<UILabel>().text = "WAIT FOR EVERYONE";
                new_button.GetComponent<GenericButtonMouseSpawner>().onClick = null;
                new_button.transform.GetChild(0).gameObject.GetComponent<UIButton>().onClick = null;
            }
        }
        public static void UpdateSleepScreen()
        {
            if (WaitForSleepLable != null && WaitForSleepLable.activeSelf == true && CanSleep(false) == false)
            {
                CanSleep(true);
                WaitForSleepLable.SetActive(false);
                if (SleepingButtons != null)
                {
                    SleepingButtons.SetActive(true);
                }
            }
        }

        public static void SelectFlairSlot(int SlotIndex)
        {
            TargetFlairSlot = SlotIndex;
            CustomizeUiPanel("Flairs");
        }
        public static void EquipFlair(int FlairIndex)
        {
            Supporters.EquipFlair(TargetFlairSlot, FlairIndex);
            CustomizeUiPanel("Main");
        }

        public static void AddFlairToList(int ID, Transform Content)
        {
            GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>("MP_FlairGrid");
            GameObject Element = GameObject.Instantiate(LoadedAssets, Content);
            Texture2D Txt = LoadedBundle.LoadAsset("FlairIcon" + ID).Cast<Texture2D>();
            Sprite Sp = Sprite.Create(Txt, new Rect(0, 0, 128, 128), new Vector2(0, 0));
            Element.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>().overrideSprite = Sp;
            Action act = new Action(() => EquipFlair(ID));
            Element.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);
            MelonLogger.Msg("Add flair to the list " + ID);
        }

        public static void CreateFlairsList()
        {
            Transform Content = CustomizeUiScrollContentRoot;

            if (Content.childCount == 0)
            {

                AddFlairToList(-1, Content);

                for (int i = 0; i < Supporters.FlairsIDs.Count; i++)
                {
                    if (Supporters.AvailableBenefits.m_Flairs.Contains(i) || Supporters.IsFlairForEveryone(i))
                    {
                        AddFlairToList(i, Content);
                    }
                }
            }
        }

        public static void CustomizeUiPanel(string Panel)
        {
            if (Panel == "Main")
            {
                TargetFlairSlot = -1;
                CustomizeUi.SetActive(true);
                CustomizeUiTabLable.text = "Customization";
                CustomizeUi.transform.GetChild(1).gameObject.SetActive(true); // Main
                CustomizeUi.transform.GetChild(2).gameObject.SetActive(false); // Flairs Scroll

                if (Supporters.IsLoaded())
                {
                    for (int i = 1; i <= Supporters.FlairSpots; i++)
                    {
                        Texture2D Txt = LoadedBundle.LoadAsset("FlairIcon" + Supporters.ConfiguratedBenefits.m_Flairs[i - 1]).Cast<Texture2D>();
                        Sprite Sp = Sprite.Create(Txt, new Rect(0, 0, 128, 128), new Vector2(0, 0));
                        CustomizeUi.transform.GetChild(1).GetChild(i).GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>().overrideSprite = Sp;
                    }
                }
            } else if (Panel == "Flairs")
            {
                CustomizeUi.SetActive(true);
                CustomizeUiTabLable.text = "Flair Slot #" + TargetFlairSlot;
                CustomizeUi.transform.GetChild(1).gameObject.SetActive(false); // Main
                CustomizeUi.transform.GetChild(2).gameObject.SetActive(true); // Flairs Scroll
                CreateFlairsList();
            }
            else if (Panel == "Close")
            {
                CustomizeUi.SetActive(false);
            }
        }

        public static void BuildCanvasUIs()
        {
            if (uConsole.m_Instance != null && uConsole.m_Instance.gameObject != null && uConsole.m_Instance.gameObject.transform.childCount > 0 && uConsole.m_Instance.gameObject.transform.GetChild(0) != null)
            {
                MelonLogger.Msg("[UI] Got Canvas");
                UiCanvas = uConsole.m_Instance.gameObject.transform.GetChild(0).gameObject.GetComponent<Canvas>();
                GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>("MP_Chat");
                ChatObject = GameObject.Instantiate(LoadedAssets, UiCanvas.transform);
                chatScroller = ChatObject.transform.GetChild(1).GetComponent<UnityEngine.UI.ScrollRect>();
                chatInput = ChatObject.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.InputField>();
                chatPanel = ChatObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
                ChatObject.SetActive(false);
                chatInput.gameObject.SetActive(false);
                MelonLogger.Msg("[UI] Chat object created!");
                GameObject LoadedAssets2 = LoadedBundle.LoadAsset<GameObject>("MP_Status");
                StatusObject = GameObject.Instantiate(LoadedAssets2, UiCanvas.transform);
                StatusPanel = StatusObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
                StatusObject.SetActive(false);
                MelonLogger.Msg("[UI] Status object created!");
                GameObject LoadedAssets3 = LoadedBundle.LoadAsset<GameObject>("MP_VoiceChat");
                MicrophoneIdicator = GameObject.Instantiate(LoadedAssets3, UiCanvas.transform);
                if (MicrophoneIdicator != null)
                {
                    MelonLogger.Msg("[UI] Microphone Indicator created!");
                    MicrophoneIdicator.SetActive(true);
                    UnityEngine.UI.Image Img = MicrophoneIdicator.GetComponent<UnityEngine.UI.Image>();
                    Img.color = new Color(Img.color.r, Img.color.g, Img.color.b, 0f);
                }
                GameObject LoadedAssets4 = LoadedBundle.LoadAsset<GameObject>("MP_Lobby");
                LobbyUI = GameObject.Instantiate(LoadedAssets4, UiCanvas.transform);
                if (LobbyUI != null)
                {
                    MelonLogger.Msg("[UI] Lobby panel created!");
                    LobbyUI.SetActive(false);
                }
                GameObject LoadedAssets6 = LoadedBundle.LoadAsset<GameObject>("MP_LobbyVoteRegion");
                LobbyRegion = GameObject.Instantiate(LoadedAssets6, UiCanvas.transform);
                if (LobbyRegion != null)
                {
                    LobbyRegion.SetActive(false);
                    int Regions = Enum.GetNames(typeof(GameRegion)).Length;
                    for (int i = 0; i < Regions; i++)
                    {
                        GameObject LoadedAssetsElement = MyMod.LoadedBundle.LoadAsset<GameObject>("MP_LobbyVoteElement");
                        GameObject Element = GameObject.Instantiate(LoadedAssetsElement, MyMod.LobbyRegion.transform.GetChild(0).GetChild(0).GetChild(0));
                        Element.SetActive(false);
                    }
                }
                GameObject LoadedAssets7 = LoadedBundle.LoadAsset<GameObject>("MP_LobbyVoteExperience");
                LobbyExperience = GameObject.Instantiate(LoadedAssets7, UiCanvas.transform);
                if (LobbyExperience != null)
                {
                    LobbyExperience.SetActive(false);
                    int ExpModes = (int)ExperienceModeType.NUM_MODES;
                    for (int i = 0; i < ExpModes; i++)
                    {
                        GameObject LoadedAssetsElement = LoadedBundle.LoadAsset<GameObject>("MP_LobbyVoteElement");
                        GameObject Element = GameObject.Instantiate(LoadedAssetsElement, LobbyExperience.transform.GetChild(0).GetChild(0).GetChild(0));
                        Element.SetActive(false);
                    }
                }
                GameObject LoadedAssets8 = LoadedBundle.LoadAsset<GameObject>("MP_ServerBrowser");
                ServerBrowser = GameObject.Instantiate(LoadedAssets8, UiCanvas.transform);
                if (ServerBrowser != null)
                {
                    ServerBrowser.SetActive(false);
                }
                GameObject LoadedAssets9 = LoadedBundle.LoadAsset<GameObject>("MP_VoiceChatRadio");
                RadioIdicator = GameObject.Instantiate(LoadedAssets9, UiCanvas.transform);
                if (RadioIdicator != null)
                {
                    MelonLogger.Msg("[UI] Radio Indicator created!");
                    RadioIdicator.SetActive(true);
                    UnityEngine.UI.Image Img = RadioIdicator.GetComponent<UnityEngine.UI.Image>();
                    Img.color = new Color(Img.color.r, Img.color.g, Img.color.b, 0f);
                    UnityEngine.UI.Text text = RadioIdicator.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>();
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
                }

                GameObject LoadedAssets10 = LoadedBundle.LoadAsset<GameObject>("MP_EmoteWheel");
                EmoteWheel = GameObject.Instantiate(LoadedAssets10, UiCanvas.transform);
                if (EmoteWheel != null)
                {
                    EmoteWheel.SetActive(false);

                    for (int i = 0; i <= 7; i++)
                    {
                        int AnimID = i;
                        GameObject btnObj = EmoteWheel.transform.GetChild(i).GetChild(0).gameObject;
                        UnityEngine.UI.Button btn = btnObj.GetComponent<UnityEngine.UI.Button>();
                        Action act = new Action(() => PerformEmote(AnimID));
                        btn.onClick.AddListener(act);
                    }
                }
                GameObject LoadedAssets11 = LoadedBundle.LoadAsset<GameObject>("MP_NewFlair");
                NewFlairNotification = GameObject.Instantiate(LoadedAssets11, UiCanvas.transform);
                if (NewFlairNotification != null)
                {
                    NewFlairNotification.SetActive(false);
                    Action act = new Action(() => ShowNotiy());
                    NewFlairNotification.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);
                }
                GameObject LoadedAssets12 = LoadedBundle.LoadAsset<GameObject>("MP_Customization");
                CustomizeUi = GameObject.Instantiate(LoadedAssets12, UiCanvas.transform);
                if (CustomizeUi != null)
                {
                    CustomizeUi.SetActive(false);
                    CustomizeUiTabLable = CustomizeUi.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>();
                    CustomizeUiScrollContentRoot = CustomizeUi.transform.GetChild(2).GetChild(0).GetChild(0);

                    Transform Main = CustomizeUi.transform.GetChild(1);
                    for (int i = 1; i <= 4; i++)
                    {
                        int INdex = i;
                        Action act = new Action(() => SelectFlairSlot(INdex));
                        Main.GetChild(i).GetChild(2).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);
                    }
                }
                GameObject LoadedAssets13 = LoadedBundle.LoadAsset<GameObject>("MP_ExpeditionEditor");
                ExpeditionEditorUI = GameObject.Instantiate(LoadedAssets13, UiCanvas.transform);
                if (ExpeditionEditorUI != null)
                {
                    ExpeditionEditorUI.SetActive(false);

                    Action act = new Action(() => ExpeditionEditor.AutoSelectRegion());
                    ExpeditionEditorUI.transform.GetChild(2).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act);


                    Action act2 = new Action(() => ExpeditionEditor.AutoSelectScene());
                    ExpeditionEditorUI.transform.GetChild(5).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act2);

                    Action act3 = new Action(() => ExpeditionEditor.AutoPositionSelect());
                    ExpeditionEditorUI.transform.GetChild(8).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act3);

                    Action act4 = new Action(() => ExpeditionEditor.ToggleContainersList());
                    ExpeditionEditorUI.transform.GetChild(12).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act4);

                    Action act5 = new Action(() => ExpeditionEditor.ToggleGearsList());
                    ExpeditionEditorUI.transform.GetChild(11).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act5);

                    Action act6 = new Action(() => ExpeditionEditor.RemoveGearVariant());
                    ExpeditionEditorUI.transform.GetChild(14).GetChild(5).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act6);

                    Action act7 = new Action(() => ExpeditionEditor.SaveExpeditionTemplate());
                    ExpeditionEditorUI.transform.GetChild(15).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act7);

                    Action act8 = new Action(() => ExpeditionEditor.BackToSelect(MyMod.ExpeditionEditorUI.transform.GetChild(1).GetComponent<UnityEngine.UI.Dropdown>().m_Value - Shared.GameRegionNegativeOffset));
                    ExpeditionEditorUI.transform.GetChild(16).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act8);

                    Action act9 = new Action(() => ExpeditionEditor.ToggleExtraPanel());
                    ExpeditionEditorUI.transform.GetChild(14).GetChild(6).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act9);

                    Action act10 = new Action(() => ExpeditionEditor.SetObjectiveGear());
                    ExpeditionEditorUI.transform.GetChild(14).GetChild(7).GetChild(3).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act10);

                    Action act11 = new Action(() => ExpeditionEditor.ToggleHarvestablesList());
                    ExpeditionEditorUI.transform.GetChild(25).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act11);

                    Action act12 = new Action(() => ExpeditionEditor.ToggleBreakdownsList());
                    ExpeditionEditorUI.transform.GetChild(26).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act12);

                    Action act13 = new Action(() => ExpeditionEditor.ToggleObjectsList());
                    ExpeditionEditorUI.transform.GetChild(30).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act13);

                    Action act14 = new Action(() => ExpeditionEditor.ToggleObjectSettingsPanel());
                    ExpeditionEditorUI.transform.GetChild(31).GetChild(3).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act14);

                    Action act15 = new Action(() => ExpeditionEditor.AddObject());
                    ExpeditionEditorUI.transform.GetChild(31).GetChild(6).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act15);

                    Action act16 = new Action(() => ExpeditionEditor.VisualizeObjects());
                    ExpeditionEditorUI.transform.GetChild(31).GetChild(7).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act16);

                    Action act17 = new Action(() => ExpeditionEditor.PlaceVisualizedObject());
                    ExpeditionEditorUI.transform.GetChild(31).GetChild(8).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act17);

                    Action act18 = new Action(() => ExpeditionEditor.SetContainerContent());
                    ExpeditionEditorUI.transform.GetChild(31).GetChild(4).GetChild(2).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act18);

                    UnityEngine.UI.Dropdown Drop = ExpeditionEditorUI.transform.GetChild(3).GetComponent<UnityEngine.UI.Dropdown>();
                    Drop.ClearOptions();
                    Drop.options.Add(new UnityEngine.UI.Dropdown.OptionData("Scene"));
                    Drop.options.Add(new UnityEngine.UI.Dropdown.OptionData("Zone"));
                    Drop.options.Add(new UnityEngine.UI.Dropdown.OptionData("Collect"));
                    Drop.options.Add(new UnityEngine.UI.Dropdown.OptionData("Flaregun"));
                    Drop.options.Add(new UnityEngine.UI.Dropdown.OptionData("Charcoal"));
                    Drop.options.Add(new UnityEngine.UI.Dropdown.OptionData("Stay"));
                    Drop.options.Add(new UnityEngine.UI.Dropdown.OptionData("Crashsit"));

                    UnityEngine.UI.Dropdown Drop2 = ExpeditionEditorUI.transform.GetChild(1).GetComponent<UnityEngine.UI.Dropdown>();
                    Drop2.ClearOptions();
                    for (int i = -Shared.GameRegionNegativeOffset; i <= Shared.GameRegionPositiveOffset; i++)
                    {
                        Drop2.options.Add(new UnityEngine.UI.Dropdown.OptionData(ExpeditionBuilder.GetRegionString(i)));
                    }
                }

                GameObject LoadedAssets14 = LoadedBundle.LoadAsset<GameObject>("MP_ExpeditionSelect");
                ExpeditionEditorSelectUI = GameObject.Instantiate(LoadedAssets14, UiCanvas.transform);
                if (ExpeditionEditorSelectUI != null)
                {
                    ExpeditionEditorSelectUI.SetActive(false);
                    Action act1 = new Action(() => ExpeditionEditor.LoadExpedition(new ExpeditionBuilder.ExpeditionTaskTemplate()));
                    ExpeditionEditorSelectUI.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act1);

                    Action act2 = new Action(() => ExpeditionEditor.RefreshExpeditionsList());
                    ExpeditionEditorSelectUI.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act2);

                    Action act3 = new Action(() => ExpeditionEditor.BackToSelect(6));
                    ExpeditionEditorSelectUI.transform.GetChild(4).GetComponent<UnityEngine.UI.Button>().onClick.AddListener(act3);
                }
            }
        }

        public static bool IsHoldingRadio()
        {
            if(GameManager.m_PlayerManager && GameManager.GetPlayerManagerComponent().m_ItemInHands && GameManager.GetPlayerManagerComponent().m_ItemInHands.m_GearName == "GEAR_HandheldShortwave")
            {
                return true;
            }
            return false;
        }


        public static void UpdateCanvasUis()
        {
            if (uConsole.m_Instance != null && uConsole.m_On == false && Cursor.visible == false)
            {
                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Return))
                {
                    if (chatInput.gameObject.activeSelf == false)
                    {
                        ChatObject.SetActive(true);
                        chatInput.gameObject.SetActive(true);
                        chatInput.ActivateInputField();
                        HideChatTimer = 5;
                        if (GameManager.GetPlayerManagerComponent().GetControlMode() == PlayerControlMode.Normal)
                        {
                            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);
                        }
                    } else {
                        if (chatInput.text != "")
                        {
                            DataStr.MultiplayerChatMessage message = new DataStr.MultiplayerChatMessage();
                            message.m_By = MyChatName;
                            message.m_Type = 1;
                            message.m_Message = chatInput.text;
                            message.m_Global = IsHoldingRadio();
                            Shared.SendMessageToChat(message, true);
                            chatInput.text = "";
                        }
                        chatInput.gameObject.SetActive(false);
                        if (GameManager.GetPlayerManagerComponent().GetControlMode() == PlayerControlMode.Locked)
                        {
                            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Normal);
                        }
                    }
                }

                if (InterfaceManager.m_Panel_PauseMenu.isActiveAndEnabled && level_name != "MainMenu")
                {
                    if (InOnline())
                    {
                        if (MyLobby == "")
                        {
                            StatusObject.SetActive(true);
                        } else {
                            LobbyUI.SetActive(true);
                        }
                    } else {
                        StatusObject.SetActive(false);
                        LobbyUI.SetActive(false);
                    }
                } else {
                    if (level_name != "MainMenu")
                    {
                        LobbyUI.SetActive(false);
                    }
                    StatusObject.SetActive(false);
                }
            }
            if (LobbyUI != null)
            {
                LobbyUI.transform.GetChild(3).gameObject.SetActive(false);
            }

            if (UIHostMenu != null)
            {
                GameObject IsSteamHost = UIHostMenu.transform.GetChild(4).gameObject;
                //GameObject PublicSteamServer = UIHostMenu.transform.GetChild(5).gameObject;
                GameObject PortsObject = UIHostMenu.transform.GetChild(8).gameObject;
                GameObject SteamLobbyType = UIHostMenu.transform.GetChild(11).gameObject;
                if (IsSteamHost.GetComponent<UnityEngine.UI.Toggle>().isOn == true)
                {
                    //if(PublicSteamServer.activeSelf == false)
                    //{
                    //    PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().Set(false);
                    //}
                    //PublicSteamServer.SetActive(true);
                    PortsObject.SetActive(false);
                    SteamLobbyType.SetActive(true);
                } else {
                    //PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().Set(false);
                    //PublicSteamServer.SetActive(false);
                    PortsObject.SetActive(true);
                    SteamLobbyType.SetActive(false);
                }
            }
            if(ExpeditionEditorUI != null)
            {
                int Type = ExpeditionEditorUI.transform.GetChild(3).GetComponent<UnityEngine.UI.Dropdown>().m_Value;
                ExpeditionEditorUI.transform.GetChild(6).gameObject.SetActive(Type != (int)ExpeditionManager.ExpeditionTaskType.ENTERSCENE);
                ExpeditionEditorUI.transform.GetChild(7).gameObject.SetActive(Type != (int)ExpeditionManager.ExpeditionTaskType.ENTERSCENE);
                ExpeditionEditorUI.transform.GetChild(8).gameObject.SetActive(Type != (int)ExpeditionManager.ExpeditionTaskType.ENTERSCENE);
                ExpeditionEditorUI.transform.GetChild(6).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = ExpeditionEditorUI.transform.GetChild(6).gameObject.GetComponent<UnityEngine.UI.Slider>().m_Value.ToString();
                ExpeditionEditorUI.transform.GetChild(20).gameObject.SetActive(Type == (int)ExpeditionManager.ExpeditionTaskType.COLLECT);
                ExpeditionEditorUI.transform.GetChild(7).gameObject.GetComponent<UnityEngine.UI.Text>().text = ExpeditionEditor.m_Center.x.ToString(CultureInfo.InvariantCulture) + ", " + ExpeditionEditor.m_Center.y.ToString(CultureInfo.InvariantCulture) + ", " + ExpeditionEditor.m_Center.z.ToString(CultureInfo.InvariantCulture);
                ExpeditionEditorUI.transform.GetChild(14).GetChild(3).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = ExpeditionEditorUI.transform.GetChild(14).GetChild(3).gameObject.GetComponent<UnityEngine.UI.Slider>().m_Value * 100 + "%";

                if(ExpeditionEditor.m_LastChance != ExpeditionEditorUI.transform.GetChild(14).GetChild(3).gameObject.GetComponent<UnityEngine.UI.Slider>().m_Value)
                {
                    ExpeditionEditor.m_LastChance = ExpeditionEditorUI.transform.GetChild(14).GetChild(3).gameObject.GetComponent<UnityEngine.UI.Slider>().m_Value;
                    ExpeditionEditor.ChangeChance();
                }
                if (ExpeditionEditor.m_LastRadious != ExpeditionEditorUI.transform.GetChild(6).gameObject.GetComponent<UnityEngine.UI.Slider>().m_Value)
                {
                    ExpeditionEditor.m_LastRadious = ExpeditionEditorUI.transform.GetChild(6).gameObject.GetComponent<UnityEngine.UI.Slider>().m_Value;
                    ExpeditionEditor.ExpeditionRadiousChanged();
                }
                ExpeditionEditorUI.transform.GetChild(14).GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = "GUID: "+ExpeditionEditor.m_LastSpawnerGUID;

                if (ExpeditionEditor.m_LastType != ExpeditionEditorUI.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Dropdown>().m_Value)
                {
                    ExpeditionEditor.m_LastType = ExpeditionEditorUI.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Dropdown>().m_Value;
                    ExpeditionEditor.ExpeditionTypeChanged();
                }

                ExpeditionEditorUI.transform.GetChild(29).gameObject.SetActive(Type == (int)ExpeditionManager.ExpeditionTaskType.STAYINZONE);

                if(ExpeditionEditor.LastOnlyShowFirstTasksState != ExpeditionEditorSelectUI.transform.GetChild(5).gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn)
                {
                    ExpeditionEditor.LastOnlyShowFirstTasksState = ExpeditionEditorSelectUI.transform.GetChild(5).gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn;
                    ExpeditionEditor.RefreshExpeditionsList(ExpeditionEditor.LastSelectedRegion);
                }
            }
        }

        public static void DebugCrap()
        {
            if (InputManager.GetReloadPressed(InputManager.m_CurrentContext))
            {
                if (ExpeditionEditorUI != null && ExpeditionEditorUI.activeSelf)
                {
                    if (GameManager.m_PlayerManager && GameManager.GetPlayerManagerComponent().m_InteractiveObjectUnderCrosshair)
                    {
                        if (GameManager.GetPlayerManagerComponent().m_InteractiveObjectUnderCrosshair.GetComponent<Comps.DroppedGearDummy>())
                        {
                            Comps.DroppedGearDummy Gear = GameManager.GetPlayerManagerComponent().m_InteractiveObjectUnderCrosshair.GetComponent<Comps.DroppedGearDummy>();
                            ExpeditionEditor.AddGear(Gear.m_Extra.m_GearName, Gear.gameObject.transform.position, Gear.gameObject.transform.rotation, Gear.m_Extra.m_PhotoGUID);
                        }
                    }
                }
            }
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.H))
            {
                if (ExpeditionEditorUI != null && ExpeditionEditorUI.activeSelf)
                {
                    if (GameManager.m_PlayerManager && GameManager.GetPlayerManagerComponent().m_InteractiveObjectUnderCrosshair)
                    {
                        if (GameManager.GetPlayerManagerComponent().m_InteractiveObjectUnderCrosshair.GetComponent<Comps.DroppedGearDummy>())
                        {
                            Comps.DroppedGearDummy Gear = GameManager.GetPlayerManagerComponent().m_InteractiveObjectUnderCrosshair.GetComponent<Comps.DroppedGearDummy>();
                            ExpeditionEditor.AddGearVariant(Gear.m_Extra.m_GearName);
                        } else if(ExpeditionEditor.m_LastObjectGUID != "")
                        {
                            ExpeditionEditor.PlaceVisualizedObject();
                        }
                    } else if (ExpeditionEditor.m_LastObjectGUID != "")
                    {
                        ExpeditionEditor.PlaceVisualizedObject();
                    }
                }
            }
            if (!DebugGUI)
            {
                return;
            }
            RaycastHit hit;
            Transform transform = GameManager.GetMainCamera().transform;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 50))
            {
                if (hit.collider.gameObject.GetComponent<BaseAi>() && hit.collider.gameObject.GetComponent<ObjectGuid>())
                {
                    DebugAnimalGUID = hit.collider.gameObject.GetComponent<ObjectGuid>().Get();
                    DebugAnimalGUIDLast = DebugAnimalGUID;
                    DebugLastAnimal = hit.collider.gameObject;
                }
            } else {
                DebugAnimalGUID = "";
            }
            if (ALWAYS_FUCKING_CURSOR_ON == true)
            {
                InputManager.m_CursorState = InputManager.CursorState.Show;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            if (DebugBind == true)
            {
                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.B))
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(GameManager.GetMainCamera().transform.position, GameManager.GetMainCamera().transform.forward, out hitInfo, 50))
                    {
                        Vector3 instantiatePosition = SnappedPosition(hitInfo.point, hitInfo.collider.transform.position);

                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = instantiatePosition;
                        //cube.GetComponent<MeshRenderer>().SetMaterial(null);
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.BLOCK))
                            {
                                _packet.Write(cube.transform.position);
                                SendUDPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.BLOCK))
                            {
                                ServerSend.BLOCK(0, cube.transform.position, true);
                            }
                        }
                    }
                }

                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.K))
                {
                    SimRevive();
                }

                ALWAYS_FUCKING_CURSOR_ON = KeyboardUtilities.InputManager.GetKey(KeyCode.L);

                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.LeftAlt))
                {
                    GameManager.GetVpFPSPlayer().Controller.Jump();
                }
            }
        }

        public static string PreviousEquipmentCheck = "";

        public static void UpdateMyEquipment()
        {
            string EQCheck = "";
            MyHasRifle = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_Rifle");
            MyArrows = GameManager.GetInventoryComponent().NumGearInInventory("GEAR_Arrow");
            MyFlares = GameManager.GetInventoryComponent().NumGearInInventory("GEAR_FlareA");
            MyBlueFlares = GameManager.GetInventoryComponent().NumGearInInventory("GEAR_FlareBlue");
            MyHasRevolver = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_Revolver");
            MyHasMedkit = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_MedicalSupplies_hangar");

            bool axe = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_Hatchet"); // Axe
            bool f_axe = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_FireAxe"); // Fire axe
            bool h_axe = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_HatchetImprovised"); // Handmade axe
            if (axe == true || f_axe == true || h_axe)
            {
                MyHasAxe = true;
            } else {
                MyHasAxe = false;
            }

            EQCheck = MyHasRifle.ToString() + MyArrows.ToString() + MyFlares.ToString() + MyBlueFlares.ToString() + MyHasRevolver.ToString() + MyHasMedkit.ToString() + MyHasAxe.ToString();
            if (PreviousEquipmentCheck != EQCheck)
            {
                PreviousEquipmentCheck = EQCheck;
                SendMyEQ();
            }

            if (MyLightSourceName != MyLastLightSourceName)
            {
                if (SkipItemEvent > 0)
                {
                    SkipItemEvent--;
                } else {
                    MyLastLightSourceName = MyLightSourceName;
                    if (playersData[0].m_Mimic == true && players[0] != null)
                    {
                        playersData[0].m_PlayerEquipmentData.m_HoldingItem = MyLightSourceName;
                    }
                    MelonLogger.Msg("Holding item: " + MyLastLightSourceName);
                    if (sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.LIGHTSOURCENAME))
                        {
                            _packet.Write(MyLightSourceName);
                            SendUDPData(_packet);
                        }
                    }
                    if (iAmHost == true)
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.LIGHTSOURCENAME))
                        {
                            ServerSend.LIGHTSOURCENAME(0, MyLightSourceName, true);
                        }
                    }
                }
            }
            if (MyLightSource != MyLastLightSource)
            {
                MelonLogger.Msg("Lightchanged " + MyLightSource);
                MyLastLightSource = MyLightSource;
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.LIGHTSOURCE))
                    {
                        _packet.Write(MyLightSource);
                        SendUDPData(_packet);
                    }
                }
                if (iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.LIGHTSOURCE))
                    {
                        ServerSend.LIGHTSOURCE(0, MyLightSource, true);
                    }
                }
            }
            if (InterfaceManager.m_Panel_Map.IsEnabled() == false)
            {
                if (GameManager.GetPlayerManagerComponent().m_ItemInHands != null)
                {
                    MyLightSourceName = GameManager.GetPlayerManagerComponent().m_ItemInHands.name;
                } else {

                    if (ModdedHandsBook == null)
                    {
                        MyLightSourceName = "";
                    } else {
                        if (!ModdedHandsBook.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Offscreen"))
                        {
                            MyLightSourceName = "Book";
                        } else {
                            MyLightSourceName = "";
                        }
                    }
                }
            } else {
                MyLightSourceName = "Map";
            }
        }

        public static void GiveBorrowedItem(string GearName, int Borrower)
        {
            if (GameManager.GetInventoryComponent())
            {
                for (int i = 0; i < GameManager.GetInventoryComponent().m_Items.Capacity; i++)
                {
                    GearItem curItem = GameManager.GetInventoryComponent().m_Items[i];
                    if (curItem.m_GearName == GearName && curItem != GameManager.GetPlayerManagerComponent().m_ItemInHands)
                    {
                        string saveProxyData = curItem.Serialize();
                        SendGivingItem(saveProxyData, curItem, Borrower, true);
                        break;
                    }
                }
            }
        }

        public static void SendGivingItem(string saveProxyData, GearItem _gear, int GiveTo, bool Borrow)
        {
            DataStr.GearItemDataPacket GearDataPak = new DataStr.GearItemDataPacket();
            GearDataPak.m_GearName = _gear.m_GearName;
            GearDataPak.m_SendedTo = GiveTo;
            bool waterMode = false;
            float waterGave = 0;

            WaterSupply bottle = null;

            if (LastSelectedGearName == "GEAR_WaterSupplyPotable" || LastSelectedGearName == "GEAR_WaterSupplyNotPotable")
            {
                waterMode = true;
                if (LastSelectedGearName == "GEAR_WaterSupplyPotable")
                {
                    bottle = GameManager.GetInventoryComponent().m_WaterSupplyPotable.m_WaterSupply;
                }
                if (LastSelectedGearName == "GEAR_WaterSupplyNotPotable")
                {
                    bottle = GameManager.GetInventoryComponent().m_WaterSupplyNotPotable.m_WaterSupply;
                }
                float Liters = bottle.m_VolumeInLiters;

                if (Liters >= 0.5f)
                {
                    waterGave = 0.5f;
                } else {
                    waterGave = Liters;
                }
                GearDataPak.m_Water = waterGave;
            } else {
                GearDataPak.m_DataProxy = saveProxyData;

                //MelonLogger.Msg("SaveProxy: " + saveProxyData);
                //MelonLogger.Msg("UTF8 bytes: " + Encoding.UTF8.GetBytes(saveProxyData).Length);
            }

            if (waterMode == true)
            {
                if (iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.GOTITEM))
                    {
                        ServerSend.GOTITEM(GiveItemTo, GearDataPak);
                    }
                }
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.GOTITEM))
                    {
                        _packet.Write(GearDataPak);
                        SendUDPData(_packet);
                    }
                }
            } else {
                byte[] bytesToSlice = Encoding.UTF8.GetBytes(saveProxyData);
                //MelonLogger.Msg(ConsoleColor.Green, "Gonna send json" + saveProxyData.GetHashCode() + " DATA: " + saveProxyData);
                if (bytesToSlice.Length > 500)
                {
                    List<byte> BytesBuffer = new List<byte>();
                    BytesBuffer.AddRange(bytesToSlice);

                    while (BytesBuffer.Count >= 500)
                    {
                        byte[] sliceOfBytes = BytesBuffer.GetRange(0, 499).ToArray();
                        BytesBuffer.RemoveRange(0, 499);

                        string jsonStringSlice = Encoding.UTF8.GetString(sliceOfBytes);
                        DataStr.SlicedJsonData SlicedPacket = new DataStr.SlicedJsonData();
                        SlicedPacket.m_GearName = _gear.m_GearName;
                        SlicedPacket.m_SendTo = GiveItemTo;
                        SlicedPacket.m_Hash = saveProxyData.GetHashCode();
                        SlicedPacket.m_Str = jsonStringSlice;

                        if (BytesBuffer.Count != 0)
                        {
                            SlicedPacket.m_Last = false;
                        } else {
                            SlicedPacket.m_Last = true;
                        }
                        //MelonLogger.Msg(ConsoleColor.Yellow, "Sending slice " + SlicedPacket.m_Hash + " DATA: " + SlicedPacket.m_Str);

                        if (iAmHost == true)
                        {
                            ServerSend.GOTITEMSLICE(GiveItemTo, SlicedPacket);
                        }
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.GOTITEMSLICE))
                            {
                                _packet.Write(SlicedPacket);
                                SendUDPData(_packet);
                            }
                        }
                    }

                    if (BytesBuffer.Count < 500 && BytesBuffer.Count != 0)
                    {
                        byte[] LastSlice = BytesBuffer.GetRange(0, BytesBuffer.Count).ToArray();
                        BytesBuffer.RemoveRange(0, BytesBuffer.Count);

                        string jsonStringSlice = Encoding.UTF8.GetString(LastSlice);
                        DataStr.SlicedJsonData SlicedPacket = new DataStr.SlicedJsonData();
                        SlicedPacket.m_GearName = _gear.m_GearName;
                        SlicedPacket.m_SendTo = GiveItemTo;
                        SlicedPacket.m_Hash = saveProxyData.GetHashCode();
                        SlicedPacket.m_Str = jsonStringSlice;
                        SlicedPacket.m_Last = true;

                        if (iAmHost == true)
                        {
                            ServerSend.GOTITEMSLICE(GiveItemTo, SlicedPacket);
                        }
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.GOTITEMSLICE))
                            {
                                _packet.Write(SlicedPacket);
                                SendUDPData(_packet);
                            }
                        }
                    }
                } else {
                    DataStr.SlicedJsonData DropPacket = new DataStr.SlicedJsonData();
                    DropPacket.m_GearName = _gear.m_GearName;
                    DropPacket.m_SendTo = GiveItemTo;
                    DropPacket.m_Hash = saveProxyData.GetHashCode();
                    DropPacket.m_Str = saveProxyData;
                    DropPacket.m_Last = true;
                    if (iAmHost == true)
                    {
                        ServerSend.GOTITEMSLICE(GiveItemTo, DropPacket);
                    }
                    if (sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.GOTITEMSLICE))
                        {
                            _packet.Write(DropPacket);
                            SendUDPData(_packet);
                        }
                    }
                }
            }

            if (waterMode == true)
            {
                string say = "half liter of " + _gear.m_LocalizedDisplayName.Text();

                if (bottle.m_VolumeInLiters == waterGave)
                {
                    bottle.m_VolumeInLiters = 0;
                }
                else
                {
                    bottle.m_VolumeInLiters = bottle.m_VolumeInLiters - waterGave;
                    say = waterGave + " liter of " + _gear.m_LocalizedDisplayName.Text();
                }
                if (!Borrow)
                {
                    HUDMessage.AddMessage("You gave " + say + " to " + playersData[GiveItemTo].m_Name);
                }
            } else {
                if (!Borrow)
                {
                    HUDMessage.AddMessage("You gave " + _gear.m_LocalizedDisplayName.Text() + " to " + playersData[GiveItemTo].m_Name);
                }
                GameManager.GetInventoryComponent().RemoveUnits(_gear, 1);
            }

            if (!Borrow)
            {
                MelonLogger.Msg("You gave " + LastSelectedGearName + " to " + playersData[GiveItemTo].m_Name);
            }

            InterfaceManager.m_Panel_Inventory.m_IsDirty = true;
            InterfaceManager.m_Panel_Inventory.Update();
        }

        public static void ProcessGivingItem(bool SelectOnly = false)
        {
            if (m_InterfaceManager != null && InterfaceManager.m_Panel_Inventory != null && InterfaceManager.m_Panel_Inventory.IsEnabled())
            {
                Panel_Inventory Panel = InterfaceManager.m_Panel_Inventory;
                if (Panel.m_SelectedItemIndex != -1)
                {
                    int seelecteditem = Panel.m_SelectedItemIndex;
                    seelecteditem = seelecteditem + Panel.m_FirstItemDisplayedIndex;
                    Il2CppSystem.Collections.Generic.List<GearItem> itemlist = Panel.m_FilteredInventoryList;

                    if (Panel.m_FilteredInventoryList.Count > 0)
                    {
                        GearItem gear = Panel.m_FilteredInventoryList[seelecteditem];

                        if (gear != null)
                        {
                            string itemname = gear.m_GearName;

                            if (itemname.Contains("(Clone)"))
                            {
                                int L = itemname.Length - 7;
                                LastSelectedGearName = itemname.Remove(L, 7);
                            }
                            else
                            {
                                LastSelectedGearName = itemname;
                            }

                            if (LastSelectedGearName != "gg")
                            {
                                LastSelectedGear = gear;
                            }
                        } else {
                            LastSelectedGearName = "";
                        }
                    } else {
                        LastSelectedGearName = "";
                    }
                }
            } else {
                return;
            }

            if (SelectOnly)
            {
                return;
            }

            if (LastSelectedGearName != "" && LastSelectedGear != null && InterfaceManager.m_Panel_Inventory.IsEnabled() == true && GiveItemTo != -1)
            {
                Il2CppSystem.Collections.Generic.List<GearItemObject> items = GameManager.GetInventoryComponent().m_Items;
                GearItem _gear = null;
                string saveProxyData = "";

                for (int i = 0; i < items.Count; i++)
                {
                    GearItem gear_ = items[i].m_GearItem;

                    if (gear_ == LastSelectedGear)
                    {
                        _gear = items[i].m_GearItem;

                        //Prepare
                        if (_gear.m_ClothingItem != null && _gear.m_ClothingItem.IsWearing())
                        {
                            _gear.m_ClothingItem.TakeOff();
                        }
                        if (_gear.m_TorchItem != null)
                        {
                            if (GiveItemTo != -1 && playersData[GiveItemTo] != null)
                            {
                                if (playersData[GiveItemTo].m_PlayerEquipmentData.m_HoldingItem != "")
                                {
                                    _gear.m_TorchItem.Extinguish(TorchState.Extinguished);
                                }
                            }
                        }
                        if (GameManager.GetPlayerManagerComponent().m_ItemInHands == _gear)
                        {
                            GameManager.GetPlayerManagerComponent().UnequipItemInHandsSkipAnimation();
                        }

                        //Saving actual values.
                        int units = 0;
                        bool wasInPlayerInv = false;
                        if (_gear.m_StackableItem != null)
                        {
                            units = _gear.m_StackableItem.m_Units;
                        }
                        wasInPlayerInv = _gear.m_InPlayerInventory;

                        //Patching item
                        _gear.m_InPlayerInventory = false;
                        if (_gear.m_StackableItem != null)
                        {
                            _gear.m_StackableItem.m_Units = 1;
                        }

                        //Saving patched item.
                        saveProxyData = _gear.Serialize();

                        //Restore actual values.
                        if (_gear.m_StackableItem != null)
                        {
                            _gear.m_StackableItem.m_Units = units;
                        }
                        _gear.m_InPlayerInventory = wasInPlayerInv;

                        //GearItemSaveDataProxy Proxy = Utils.DeserializeObject<GearItemSaveDataProxy>(saveProxyData);
                        //MelonLogger.Msg(ConsoleColor.Green, "[GearItem] Sending gear with m_CurrentHP " + cloneGear.m_CurrentHP);
                        //MelonLogger.Msg(ConsoleColor.Green, "[GearItem] Sending gear with m_LastUpdatedTODHours " + cloneGear.m_LastUpdatedTODHours);
                        //MelonLogger.Msg(ConsoleColor.Green, "[GearItem] Sending gear with m_HoursPlayed " + Proxy.m_HoursPlayed);
                        //MelonLogger.Msg(ConsoleColor.Green, "[GearItem] Sending gear with m_DecayScalar " + cloneGear.m_DecayScalar);

                        //UnityEngine.Object.Destroy(cloneObj);
                        break;
                    }
                }

                if (_gear != null)
                {
                    SendGivingItem(saveProxyData, _gear, GiveItemTo, false);
                }
            }
        }

        public static void UpdateVisualSyncs()
        {
            if (GameManager.GetFatigueComponent().GetHeavyBreathingState() == HeavyBreathingState.Heavy)
            {
                NowHeavyBreath = true;
            } else {
                NowHeavyBreath = false;
            }

            BloodLosts = GameManager.GetBloodLossComponent().GetAfflictionsCount();

            if (PreviousBloodLosts != BloodLosts)
            {
                PreviousBloodLosts = BloodLosts;
                if (sendMyPosition == true) // CLIENT
                {
                    using (Packet _packet = new Packet((int)ClientPackets.BLOODLOSTS))
                    {
                        _packet.Write(BloodLosts);
                        SendUDPData(_packet);
                    }
                }
                if (iAmHost == true) // HOST
                {
                    using (Packet _packet = new Packet((int)ServerPackets.BLOODLOSTS))
                    {
                        ServerSend.BLOODLOSTS(0, BloodLosts, true);
                    }
                }
            }

            if (PreviousNowHeavyBreath != NowHeavyBreath)
            {
                PreviousNowHeavyBreath = NowHeavyBreath;

                if (sendMyPosition == true) // CLIENT
                {
                    using (Packet _packet = new Packet((int)ClientPackets.HEAVYBREATH))
                    {
                        _packet.Write(NowHeavyBreath);
                        SendUDPData(_packet);
                    }
                }
                if (iAmHost == true) // HOST
                {
                    using (Packet _packet = new Packet((int)ServerPackets.HEAVYBREATH))
                    {
                        ServerSend.HEAVYBREATH(0, NowHeavyBreath, true);
                    }
                }
            }
        }

        public static void SyncMovement(bool InFight, Transform target)
        {
            if (IsDead == false && InFight == false)
            {
                if (GameManager.GetPlayerManagerComponent().PlayerIsClimbing() == false)
                {
                    if (GameManager.GetPlayerManagerComponent().PlayerCanSprint() == true && GameManager.GetPlayerManagerComponent().PlayerIsSprinting())
                    {
                        MyAnimState = "Run";
                    } else {
                        if (GameManager.GetPlayerManagerComponent().PlayerIsCrouched() == true)
                        {
                            MyAnimState = "Ctrl";
                        } else {
                            MyAnimState = "Walk";
                        }
                    }
                } else {
                    MyAnimState = "RopeMoving";
                }
            }

            Vector3 v3 = target.position;

            if (GameManager.GetRestComponent().IsSleeping() == true && GameManager.GetRestComponent().m_Bed != null)
            {
                Bed _Bed = GameManager.GetRestComponent().m_Bed;
                if (_Bed != null && _Bed.m_BodyPlacementTransform != null)
                {
                    v3 = _Bed.m_BodyPlacementTransform.position;
                }
            }

            if (sendMyPosition == true) // CLIENT
            {
                using (Packet _packet = new Packet((int)ClientPackets.XYZ))
                {
                    _packet.Write(v3);
                    SendUDPData(_packet);
                }
            }

            if (iAmHost == true)
            {
                ServerSend.XYZ(0, v3, true);
            }
        }

        public static void SyncRotation(Transform target)
        {
            Quaternion rot = target.rotation;

            if (GameManager.GetRestComponent().IsSleeping() == true && GameManager.GetRestComponent().m_Bed != null)
            {
                Bed _Bed = GameManager.GetRestComponent().m_Bed;
                if (_Bed != null)
                {
                    rot = GetBedRotation(_Bed);
                }
            }

            if (sendMyPosition == true) // CLIENT
            {
                using (Packet _packet = new Packet((int)ClientPackets.XYZW))
                {
                    _packet.Write(rot);
                    SendUDPData(_packet);
                }
            }
            if (iAmHost == true) // HOST
            {
                using (Packet _packet = new Packet((int)ServerPackets.XYZW))
                {
                    ServerSend.XYZW(0, rot, true);
                }
            }
        }

        public static void SetCurrentAnimation(bool InFight)
        {
            if (IsDead == true || InFight == true)
            {
                if (IsDead == true)
                {
                    MyAnimState = "Knock";
                } else {
                    MyAnimState = "Fight";
                }
            } else {

                if (MyEmote != null)
                {
                    MyAnimState = MyEmote.m_Animation;
                    return;
                }

                if (GameManager.GetPlayerManagerComponent().PlayerIsClimbing() == false)
                {
                    if (GameManager.GetPlayerManagerComponent().PlayerIsSleeping() == true)
                    {
                        MyAnimState = "Sleep";
                    }
                    else
                    {
                        if (GameManager.GetPlayerInVehicle().m_InVehicle == true)
                        {
                            MyAnimState = "Sit";
                        }
                        else
                        {
                            if (InterfaceManager.m_Panel_Map.IsEnabled() == false)
                            {
                                if (InterfaceManager.m_Panel_BreakDown.IsBreakingDown() == true)
                                {
                                    BreakDown bk = InterfaceManager.m_Panel_BreakDown.m_BreakDown;
                                    if (bk != null && bk.gameObject != null)
                                    {
                                        GameObject bkObj = InterfaceManager.m_Panel_BreakDown.m_BreakDown.gameObject;
                                        float CamY = GameManager.GetMainCamera().transform.position.y;
                                        if (bkObj.transform.position.y > CamY)
                                        {
                                            MyAnimState = "HarvestingStanding";
                                        }
                                        else
                                        {
                                            MyAnimState = "Harvesting";
                                        }
                                    }
                                    else
                                    {
                                        MyAnimState = "Harvesting";
                                    }
                                }
                                else if (PlayerInteractionWith != null || (InterfaceManager.m_Panel_Diagnosis != null && InterfaceManager.m_Panel_Diagnosis.TreatmentInProgress() == true))
                                {
                                    if (GameManager.GetPlayerManagerComponent().PlayerIsCrouched() == true)
                                    {
                                        MyAnimState = "Harvesting";
                                    }
                                    else
                                    {
                                        MyAnimState = "HarvestingStanding";
                                    }
                                }
                                else if (InterfaceManager.m_Panel_BodyHarvest != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject != null)
                                {
                                    MyAnimState = "Harvesting";
                                }
                                else if ((InterfaceManager.m_Panel_SnowShelterBuild != null && InterfaceManager.m_Panel_SnowShelterBuild.m_IsBuilding == true) || (InterfaceManager.m_Panel_SnowShelterInteract != null && InterfaceManager.m_Panel_SnowShelterInteract.m_IsDismantling == true))
                                {
                                    MyAnimState = "Harvesting";
                                } else if (Pathes.FakeRockCacheCallback != null)
                                {
                                    MyAnimState = "Harvesting";
                                } else if (GameManager.GetPlayerManagerComponent().m_HarvestableInProgress != null)
                                {
                                    if (GameManager.GetPlayerManagerComponent().PlayerIsCrouched() == true)
                                    {
                                        MyAnimState = "Harvesting";
                                    }
                                    else
                                    {
                                        GameObject bkObj = GameManager.GetPlayerManagerComponent().m_HarvestableInProgress.gameObject;
                                        if (bkObj.name.Contains("Cattail"))
                                        {
                                            MyAnimState = "HarvestingStanding";
                                        } else {
                                            float CamY = GameManager.GetMainCamera().transform.position.y;
                                            if (bkObj.transform.position.y > CamY)
                                            {
                                                MyAnimState = "HarvestingStanding";
                                            } else {
                                                MyAnimState = "Harvesting";
                                            }
                                        }
                                    }
                                } else {
                                    PlayerControlMode Plc = GameManager.GetPlayerManagerComponent().GetControlMode();
                                    if (GameManager.GetPlayerManagerComponent().PlayerIsCrouched() == true)
                                    {
                                        if (Plc == PlayerControlMode.AimRevolver)
                                        {
                                            MyAnimState = "HoldGun_Sit";
                                        }
                                        else if (Plc == PlayerControlMode.StartingFire)
                                        {
                                            MyAnimState = "Igniting";
                                        }
                                        else if (Plc == PlayerControlMode.DeployRope || Plc == PlayerControlMode.TakeRope)
                                        {
                                            MyAnimState = "Harvesting";
                                        } else {
                                            MyAnimState = "Ctrl";
                                        }
                                    } else {
                                        if (Plc == PlayerControlMode.AimRevolver)
                                        {
                                            MyAnimState = "HoldGun";
                                        }
                                        else if (Plc == PlayerControlMode.StartingFire)
                                        {
                                            MyAnimState = "Igniting";
                                        }
                                        else if (Plc == PlayerControlMode.DeployRope || Plc == PlayerControlMode.TakeRope)
                                        {
                                            MyAnimState = "HarvestingStanding";
                                        } else {
                                            MyAnimState = "Idle";
                                        }
                                    }
                                }
                            } else {
                                MyAnimState = "Map";
                            }
                        }
                    }
                } else {
                    MyAnimState = "RopeIdle";
                }
            }
        }

        public static void UpdateSceneGUID()
        {
            if (GameManager.m_SceneTransitionData != null)
            {
                level_guid = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;
                if (level_guid != previous_level_guid)
                {
                    previous_level_guid = level_guid;
                    MelonLogger.Msg("Scene GUID " + GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent);
                }
            }
        }

        public static void SendMyAnimation()
        {
            if (MyPreviousAnimState != MyAnimState)
            {
                MyPreviousAnimState = MyAnimState;
                if (playersData[0].m_Mimic == true && players[0] != null)
                {
                    playersData[0].m_AnimState = MyAnimState;
                }
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.ANIMSTATE))
                    {
                        _packet.Write(MyAnimState);
                        SendUDPData(_packet);
                    }
                }
                if (iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.ANIMSTATE))
                    {
                        ServerSend.ANIMSTATE(0, MyAnimState, true);
                    }
                }
            }
            bool IsAiming = false;
            if (GameManager.m_PlayerManager != null && GameManager.m_PlayerManager.m_ItemInHands != null && GameManager.m_PlayerManager.m_ItemInHands.m_GunItem != null && GameManager.m_PlayerManager.m_ItemInHands.m_GunItem.IsAiming() == true)
            {
                IsAiming = true;
            }

            if (MyIsAiming != IsAiming)
            {
                MyIsAiming = IsAiming;
                if (playersData[0].m_Mimic == true && players[0] != null)
                {
                    playersData[0].m_Aiming = MyIsAiming;
                }
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.CHANGEAIM))
                    {
                        _packet.Write(MyIsAiming);
                        SendUDPData(_packet);
                    }
                }
                if (iAmHost == true)
                {
                    ServerSend.CHANGEAIM(0, MyIsAiming, true);
                }
            }
        }

        public static Quaternion GetBedRotation(Bed bed)
        {
            GameObject obj = bed.gameObject;
            Quaternion rot = new Quaternion(0, 0, 0, 0);
            if (obj)
            {
                if (obj.GetComponent<Bed>().m_Bedroll == null)
                {
                    rot = Quaternion.Inverse(obj.GetComponent<Bed>().m_BodyPlacementTransform.rotation);
                } else {
                    rot = obj.transform.rotation;
                }
            }

            return rot;
        }

        public static void SyncSleeping()
        {
            if (WaitForSleepLable != null && WaitForSleepLable.activeSelf == true)
            {
                IsSleeping = true;
            } else {
                IsSleeping = GameManager.GetPlayerManagerComponent().PlayerIsSleeping();
            }
            if (PreviousSleeping != IsSleeping)
            {
                PreviousSleeping = IsSleeping;
                if (GameManager.GetRestComponent().IsSleeping() == true || (WaitForSleepLable != null && WaitForSleepLable.activeSelf == true))
                {
                    MelonLogger.Msg("Going to sleep");
                    MelonLogger.Msg("Skiping Cycle time " + MyCycleSkip);

                    Bed bed = GameManager.GetRestComponent().m_Bed;

                    if (bed != null)
                    {
                        if (bed.m_BodyPlacementTransform != null)
                        {
                            SyncRotation(GameManager.GetPlayerObject().transform);
                            SyncMovement(false, GameManager.GetPlayerObject().transform);
                        }
                    }
                } else {
                    MyCycleSkip = 0;
                    MelonLogger.Msg("Has wakeup or cancle sleep");
                    SyncRotation(GameManager.GetPlayerObject().transform);
                    SyncMovement(false, GameManager.GetPlayerObject().transform);
                }

                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.SLEEPHOURS))
                    {
                        _packet.Write(MyCycleSkip);
                        SendUDPData(_packet);
                    }
                }
            }
        }

        public static void SendHarvestingAnimal()
        {
            if (PreviousHarvestingAnimal != HarvestingAnimal)
            {
                PreviousHarvestingAnimal = HarvestingAnimal;
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.HARVESTINGANIMAL))
                    {
                        _packet.Write(HarvestingAnimal);
                        SendUDPData(_packet);
                    }
                }
                if (iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.HARVESTINGANIMAL))
                    {
                        ServerSend.HARVESTINGANIMAL(0, HarvestingAnimal, false);
                    }
                }
            }
        }

        public static void CancleIfSomeoneDoSame()
        {
            string boxGUID = null;
            string harvestGUID = null;
            string harvestAnimalGUID = null;
            string breakGuid = "";
            string breakParentGuid = "";
            bool NeedCheckBreakDown = false;
            //If I am looting box
            Container c = GameManager.GetPlayerManagerComponent().m_ContainerBeingSearched;
            if (c != null && c.gameObject != null && c.gameObject.GetComponent<ObjectGuid>() != null)
            {
                boxGUID = c.gameObject.GetComponent<ObjectGuid>().Get();
            }
            //If I am harvesting plant
            Harvestable h = GameManager.GetPlayerManagerComponent().m_HarvestableInProgress;
            if (h != null && h.gameObject != null && h.gameObject.GetComponent<ObjectGuid>() != null)
            {
                harvestGUID = h.gameObject.GetComponent<ObjectGuid>().Get();
            }
            //If I am harvest animal
            Panel_BodyHarvest PBH = InterfaceManager.m_Panel_BodyHarvest;
            if (PBH != null && PBH.isActiveAndEnabled == true)
            {
                if (PBH.m_BodyHarvest != null && PBH.m_BodyHarvest.gameObject != null && PBH.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    harvestAnimalGUID = PBH.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>().Get();
                }
            }
            // If I am breakdown something
            Panel_BreakDown PBD = InterfaceManager.m_Panel_BreakDown;
            if (PBD != null && PBD.isActiveAndEnabled == true && PBD.m_BreakDown != null && PBD.m_BreakDown.gameObject != null)
            {
                if (PBD.m_BreakDown.gameObject.activeSelf == true)
                {
                    GameObject gameObject = PBD.m_BreakDown.gameObject;
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
                    NeedCheckBreakDown = true;
                } else {
                    if (PBD.m_IsBreakingDown == true)
                    {
                        PBD.m_IsBreakingDown = false;
                        PBD.StopAudioAndRumbleEffects();
                    }
                    PBD.OnCancel();
                }
            }

            for (int i = 0; i < playersData.Count; i++)
            {
                if (playersData[i] != null && i != ClientUser.myId)
                {
                    if (boxGUID != null || harvestGUID != null || harvestAnimalGUID != null || (breakGuid != null || breakParentGuid != null))
                    {
                        if (playersData[i].m_Container != null)
                        {
                            DataStr.ContainerOpenSync otherBox = playersData[i].m_Container;
                            if (otherBox.m_Guid == boxGUID && otherBox.m_LevelID == levelid && otherBox.m_LevelGUID == level_guid)
                            {
                                boxGUID = null;
                                c.CancelSearch();
                            }
                        }
                        string otherHarvestPlant = playersData[i].m_Plant;

                        if (harvestGUID != null)
                        {
                            if (otherHarvestPlant == harvestGUID)
                            {
                                HUDMessage.AddMessage(playersData[i].m_Name + " IS ALREADY COLLECTING THIS");
                                harvestGUID = null;
                                h.CancelHarvest();
                            }
                        }

                        string otherAnimlGuid = playersData[i].m_HarvestingAnimal;

                        if (harvestAnimalGUID != null)
                        {
                            if (otherAnimlGuid == harvestAnimalGUID)
                            {
                                MelonLogger.Msg("OtherPlayerGUID " + otherAnimlGuid);
                                MelonLogger.Msg("harvestAnimalGUID " + harvestAnimalGUID);
                                MelonLogger.Msg("index " + i);
                                harvestAnimalGUID = null;
                                ExitHarvesting();
                            }
                        }
                        if (NeedCheckBreakDown)
                        {
                            DataStr.BrokenFurnitureSync otherFurn = playersData[i].m_BrakingObject;
                            if (otherFurn.m_Guid == breakGuid && otherFurn.m_ParentGuid == breakParentGuid && otherFurn.m_LevelID == levelid && otherFurn.m_LevelGUID == level_guid)
                            {
                                NeedCheckBreakDown = false;
                                HUDMessage.AddMessage(playersData[i].m_Name + " IS BREAKING THIS");
                                GameAudioManager.PlayGUIError();
                                if (PBD.m_IsBreakingDown == true)
                                {
                                    PBD.m_IsBreakingDown = false;
                                    PBD.StopAudioAndRumbleEffects();
                                }
                                PBD.OnCancel();
                            }
                        }
                    } else {
                        return;
                    }
                }
            }
        }

        public static void SendNewRadioFrequency()
        {
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.CHANGEDFREQUENCY))
                {
                    _packet.Write(RadioFrequency);
                    SendUDPData(_packet);
                }
            }
            //PlayRadioOver();
        }

        public static string GetRadioFrequency(float ID)
        {
            float Correct = 325 + ID;
            return Correct.ToString();
        }
        public static float nextLEDBlink = 0.0f;
        public static float LastSearchDistance = MaxRadioSearchDistance;
        public static float MaxRadioSearchDistance = 400;
        public static float BlinkRateConstant = 210;
        public static float MinimalBlink = 0.03f;

        public static void DoRadioSearch()
        {
            LastSearchDistance = GetDistanceToCairn(GameManager.GetPlayerTransform().position);
        }

        public static void DoRadioBeep()
        {
            AudioClip LoadedAssets = LoadedBundle.LoadAsset<AudioClip>("RadioBeep");
            if (MyRadioAudio)
            {
                GameObject Generic = MyRadioAudio.transform.GetChild(0).gameObject;
                AudioSource AudioSo = Generic.GetComponent<AudioSource>();
                AudioSo.PlayOneShot(LoadedAssets);
            }
        }
        public static bool SearchModeActive = false;
        public static bool CairnsSearchActive()
        {
            if (CurrentCustomChalleng.m_Started && CurrentChallengeRules.m_Name == "Lost in action" && CurrentCustomChalleng.m_CurrentTask == 0 && SearchModeActive)
            {
                return true;
            } else {
                return false;
            }
        }

        public static void UpdateRadio()
        {
            if (ViewModelRadioNeedle)
            {
                Quaternion localRotation = ViewModelRadioNeedle.transform.localRotation;
                localRotation.eulerAngles = new Vector3(localRotation.eulerAngles.x, localRotation.eulerAngles.y, RadioFrequency);
                ViewModelRadioNeedle.transform.localRotation = localRotation;
            }

            float WheelScroll = InputManager.GetAxisScrollWheel(InputManager.m_CurrentContext);

            if (WheelScroll < 0 && RadioFrequency > -25)
            {
                RadioFrequency -= 0.1f;
                RadioFrequency = Mathf.Round(RadioFrequency * 10.0f) * 0.1f;

                HUDMessage.HUDMessageInfo msg = new HUDMessage.HUDMessageInfo();
                msg.m_Text = "Frequency " + GetRadioFrequency(RadioFrequency);
                HUDMessage.ShowMessage(msg);
                SendNewRadioFrequency();
            }
            if (WheelScroll > 0 && RadioFrequency < 25)
            {
                RadioFrequency += 0.1f;
                RadioFrequency = Mathf.Round(RadioFrequency * 10.0f) * 0.1f;

                HUDMessage.HUDMessageInfo msg = new HUDMessage.HUDMessageInfo();
                msg.m_Text = "Frequency " + GetRadioFrequency(RadioFrequency);
                HUDMessage.ShowMessage(msg);
                SendNewRadioFrequency();
            }
            bool LEDState = ViewModelRadioLED.activeSelf;

            if (InputManager.GetAltFirePressed(InputManager.m_CurrentContext))
            {
                

                Panel_ActionPicker Panel = InterfaceManager.m_Panel_ActionPicker;
                if(Panel && !Panel.IsEnabled())
                {
                    //ShowRadioActionPicker(null);
                }
            }

            if (LastSearchDistance != float.PositiveInfinity && CairnsSearchActive())
            {
                float Rate = GetRadioBeepRate(LastSearchDistance);
                if (nextLEDBlink == 0.0f)
                {
                    nextLEDBlink = Time.time + Rate;
                }

                if (Time.time > nextLEDBlink)
                {
                    nextLEDBlink = Time.time + Rate;

                    if (LEDState == false)
                    {
                        DoRadioBeep();
                        LEDState = true;
                    } else {
                        LEDState = false;
                    }
                }
            } else {
                LEDState = false;
            }

            ViewModelRadioLED.SetActive(LEDState);
        }

        public static bool IsCustomHandItem(string item)
        {
            if (item == "GEAR_Hatchet"
                || item == "GEAR_HatchetImprovised"
                || item == "GEAR_Hammer"
                || item == "GEAR_Knife"
                || item == "GEAR_KnifeImprovised"
                || item == "GEAR_KnifeScrapMetal"
                || item == "GEAR_JeremiahKnife"
                || item == "GEAR_Prybar"
                || item == "GEAR_FireAxe"
                || item == "GEAR_Shovel")
            {
                return true;
            }
            return false;
        }
        public static bool IsUserGeneratedHandItem(string item)
        {
            if (item == "GEAR_SCPhoto"
                || item == "GEAR_SCMapPiece")
            {
                return true;
            }
            return false;
        }
        public static bool ShouldPerformAttack = false;
        public static bool ShouldReEquipFaster = false;
        public static int SkipItemEvent = 0;

        public static void RetakeItem()
        {
            GearItem gi = GameManager.GetPlayerManagerComponent().m_ItemInHands;
            if (gi)
            {
                SkipItemEvent = 2;
                GameManager.GetPlayerManagerComponent().UnequipItemInHandsSkipAnimation();
                GameManager.GetPlayerManagerComponent().UseInventoryItem(gi);
                PlayerAnimation PL = GameManager.GetPlayerAnimationComponent();
                ShouldReEquipFaster = true;
                float Speed = GetMeelePlayerInfo(gi.m_GearName).m_RetakeTime;
                PL.SetFloat(PL.m_AnimParameter_PlaybackSpeedMultiplier, Speed);
                if (PL.m_EquippedFirstPersonWeaponRightHand && PL.m_EquippedFirstPersonWeaponRightHand.m_Animator)
                {
                    PL.m_EquippedFirstPersonWeaponRightHand.m_Animator.speed = Speed;
                }
                PL.m_Animator.speed = Speed;
            }
        }
        private static BulletImpactEffectType GetImpactEffectTypeBasedOnMaterial(
          string tag)
        {
            if (tag == "Snow")
                return BulletImpactEffectType.BulletImpactEffect_Snow;
            if (tag == "Wood")
                return BulletImpactEffectType.BulletImpactEffect_Wood;
            if (tag == "Straw")
                return BulletImpactEffectType.BulletImpactEffect_Fabric;
            if (tag == "Ice")
                return BulletImpactEffectType.BulletImpactEffect_Ice;
            if (tag == "Rug")
                return BulletImpactEffectType.BulletImpactEffect_Fabric;
            if (tag == "Stone")
                return BulletImpactEffectType.BulletImpactEffect_Stone;
            if (tag == "Metal")
                return BulletImpactEffectType.BulletImpactEffect_Metal;
            if (tag == "WoodSolid")
                return BulletImpactEffectType.BulletImpactEffect_Wood;
            if (tag == "Road")
                return BulletImpactEffectType.BulletImpactEffect_Stone;
            if (tag == "Flesh" || tag == "BaseAi")
                return BulletImpactEffectType.BulletImpactEffect_Flesh;
            return tag == "Glass" ? BulletImpactEffectType.BulletImpactEffect_Glass : BulletImpactEffectType.BulletImpactEffect_Untagged;
        }

        public static DataStr.MeleeDescripter GetMeelePlayerInfo(string weapon)
        {
            DataStr.MeleeDescripter Info = new DataStr.MeleeDescripter();

            if (weapon == "GEAR_Hatchet" || weapon == "GEAR_HatchetImprovised")
            {
                Info.m_PlayerDamage = 20;
                Info.m_AnimalDamage = 45;
                Info.m_BloodLoss = true;
                Info.m_Pain = false;
                Info.m_ClothingTearing = true;
                Info.m_AttackSpeed = 1.1f;
                Info.m_RetakeTime = 1.1f;
                return Info;
            }
            if (weapon == "GEAR_FireAxe")
            {
                Info.m_PlayerDamage = 40;
                Info.m_AnimalDamage = 70;
                Info.m_BloodLoss = true;
                Info.m_Pain = true;
                Info.m_ClothingTearing = true;
                Info.m_AttackSpeed = 1.3f;
                Info.m_RetakeTime = 1.3f;
                return Info;
            }
            if (weapon == "GEAR_Hammer")
            {
                Info.m_PlayerDamage = 25;
                Info.m_AnimalDamage = 55;
                Info.m_BloodLoss = false;
                Info.m_Pain = true;
                Info.m_ClothingTearing = false;
                Info.m_AttackSpeed = 1f;
                Info.m_RetakeTime = 1f;
                return Info;
            }
            if (weapon == "GEAR_Prybar")
            {
                Info.m_PlayerDamage = 15;
                Info.m_AnimalDamage = 40;
                Info.m_BloodLoss = false;
                Info.m_Pain = true;
                Info.m_ClothingTearing = false;
                Info.m_AttackSpeed = 1.2f;
                Info.m_RetakeTime = 1f;
                return Info;
            }
            if (weapon == "GEAR_Knife" || weapon == "GEAR_KnifeImprovised" || weapon == "GEAR_KnifeScrapMetal" || weapon == "GEAR_JeremiahKnife")
            {
                Info.m_PlayerDamage = 17;
                Info.m_AnimalDamage = 57;
                Info.m_BloodLoss = true;
                Info.m_Pain = false;
                Info.m_ClothingTearing = true;
                Info.m_AttackSpeed = 10f;
                Info.m_RetakeTime = 8f;
                return Info;
            }
            if (weapon == "GEAR_Shovel")
            {
                Info.m_PlayerDamage = 18;
                Info.m_AnimalDamage = 48;
                Info.m_BloodLoss = false;
                Info.m_Pain = true;
                Info.m_ClothingTearing = true;
                Info.m_AttackSpeed = 1f;
                Info.m_RetakeTime = 1f;
            }
            return Info;
        }

        public static bool MouseActionIsAllowed()
        {
            PlayerManager managerComponent = GameManager.GetPlayerManagerComponent();
            if (InterfaceManager.IsOverlayActiveImmediate()
                || (InterfaceManager.m_Panel_HUD.m_TwoButtonsChoiceUI && InterfaceManager.m_Panel_HUD.m_TwoButtonsChoiceUI.IsManagingInput())
                || (UICamera.currentScheme != UICamera.ControlScheme.Controller && managerComponent.m_InteractiveObjectUnderCrosshair && !managerComponent.PlayerIsZooming() && managerComponent.m_InteractiveObjectUnderCrosshair.GetComponent<BreakDown>())
                || managerComponent.IsInPlacementMode()
                || InputManager.IsClickHoldActive()
                || InteractionInprocess)
            {
                return false;
            }

            return true;
        }

        public static void PerformMeleeAttack(string Weapon)
        {
            if (GameManager.GetPlayerAnimationComponent().CanTransitionToState(PlayerAnimation.State.Throwing) && MouseActionIsAllowed())
            {
                ShouldPerformAttack = true;
                PlayerAnimation PL = GameManager.GetPlayerAnimationComponent();
                float Speed = GetMeelePlayerInfo(Weapon).m_AttackSpeed;
                PL.Trigger_Generic_Throw(null, null);

                PL.SetFloat(PL.m_AnimParameter_PlaybackSpeedMultiplier, Speed);
                if (PL.m_EquippedFirstPersonWeaponRightHand && PL.m_EquippedFirstPersonWeaponRightHand.m_Animator)
                {
                    PL.m_EquippedFirstPersonWeaponRightHand.m_Animator.speed = Speed;
                }
                PL.m_Animator.speed = Speed;
                if (iAmHost)
                {
                    ServerSend.MELEESTART(0);
                }
                if (sendMyPosition)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.MELEESTART))
                    {
                        _packet.Write(true);
                        SendUDPData(_packet);
                    }
                }
            }
        }

        public static void ReadNoteInHands()
        {
            if (MouseActionIsAllowed() && InterfaceManager.m_Panel_HUD.m_CollectibleNoteObject.activeSelf == false)
            {
                Pathes.DisplayNote(GameManager.GetPlayerManagerComponent().m_ItemInHands);
            }
        }

        public static float MeleeRange = 1.9f;

        public static void DoMeleeHitFX(Vector3 pos, Vector3 forward, Quaternion rot, GameObject PlayerObj)
        {
            int layerMask = Utils.m_WeaponProjectileCollisionLayerMask | 134217728;
            RaycastHit hit;
            if (AiUtils.RaycastWithAimAssist(pos, forward, out hit, MeleeRange, MeleeRange, MeleeRange, 30, layerMask))
            {
                string surfaceTag = Utils.GetMaterialTagForObjectAtPosition(hit.collider.gameObject, hit.point);
                GameAudioManager.SetMaterialSwitch(surfaceTag, PlayerObj);
                GameAudioManager.Play3DSound("Play_StoneImpacts", PlayerObj);
                GameAudioManager.NotifyAiAudioEvent(PlayerObj, hit.point, GameAudioAiEvent.Generic);
                MaterialEffectType materialEffectType = ImpactDecals.MapSurfaceTagToMaterialEffectType(surfaceTag);
                GameManager.GetDynamicDecalsManager().AddImpactDecal(ProjectileType.Arrow, materialEffectType, hit.point, hit.normal);

                BulletImpactEffectType typeBasedOnMaterial = GetImpactEffectTypeBasedOnMaterial(surfaceTag);
                BulletImpactEffectPool impactEffectPool = GameManager.GetEffectPoolManager().GetBulletImpactEffectPool();
                if (typeBasedOnMaterial == BulletImpactEffectType.BulletImpactEffect_Untagged)
                    impactEffectPool.SpawnUntilParticlesDone(BulletImpactEffectType.BulletImpactEffect_Stone, hit.point, rot);
                else
                    impactEffectPool.SpawnUntilParticlesDone(typeBasedOnMaterial, hit.point, rot);
                MaterialEffectType materialEffectType2 = ImpactDecals.MapBulletImpactEffectTypeToMaterialEffectType(typeBasedOnMaterial);
                GameManager.GetDynamicDecalsManager().AddImpactDecal(ProjectileType.Bullet, materialEffectType2, hit.point, forward);
                if (!hit.collider || !hit.collider.gameObject)
                    return;
                GameAudioManager.SetMaterialSwitch(surfaceTag, hit.collider.gameObject);
                Transform V3 = PlayerObj.transform;
                GameObject Player = PlayerObj;
                int num = (int)AkSoundEngine.PostEvent(AK.EVENTS.PLAY_BULLETIMPACTS, GameAudioManager.GetSoundEmitterFromGameObject(PlayerObj));
                GameAudioManager.SetAudioSourceTransform(Player, V3);
            }
        }

        public static void MeleeHit()
        {
            PushActionToMyDoll("Melee");
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(MyMod.GetGearItemObject("GEAR_Arrow"), new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            GearItem componentArrow = gameObject.GetComponent<GearItem>();
            componentArrow.m_ArrowItem.Fire();
            UnityEngine.Object.Destroy(gameObject);
            DataStr.ShootSync HitSync = new DataStr.ShootSync();
            HitSync.m_position = GameManager.GetVpFPSCamera().transform.position;
            HitSync.m_rotation = GameManager.GetVpFPSCamera().transform.rotation;
            HitSync.m_projectilename = "Melee";
            HitSync.m_skill = 0;
            HitSync.m_camera_forward = GameManager.GetVpFPSCamera().transform.forward;
            HitSync.m_camera_right = GameManager.GetVpFPSCamera().transform.right;
            HitSync.m_camera_up = GameManager.GetVpFPSCamera().transform.up;
            HitSync.m_sceneguid = level_guid;
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.SHOOTSYNC))
                {
                    _packet.Write(HitSync);
                    SendUDPData(_packet);
                }
            }
            if (iAmHost == true)
            {
                ServerSend.SHOOTSYNC(0, HitSync, true);
            }

            int layerMask = Utils.m_WeaponProjectileCollisionLayerMask | 134217728;
            RaycastHit hit;

            string Weapon = "";

            if (GameManager.GetPlayerManagerComponent().m_ItemInHands != null)
            {
                Weapon = GameManager.GetPlayerManagerComponent().m_ItemInHands.m_GearName;
            }
            if (AiUtils.RaycastWithAimAssist(GameManager.GetVpFPSCamera().transform.position, GameManager.GetVpFPSCamera().transform.forward, out hit, MeleeRange, MeleeRange, MeleeRange, 30, layerMask))
            {
                string surfaceTag = Utils.GetMaterialTagForObjectAtPosition(hit.collider.gameObject, hit.point);
                GameAudioManager.SetMaterialSwitch(surfaceTag, GameManager.GetPlayerObject());
                GameAudioManager.Play3DSound("Play_StoneImpacts", GameManager.GetPlayerObject());
                GameAudioManager.NotifyAiAudioEvent(GameManager.GetPlayerObject(), hit.point, GameAudioAiEvent.Generic);
                MaterialEffectType materialEffectType = ImpactDecals.MapSurfaceTagToMaterialEffectType(surfaceTag);
                GameManager.GetDynamicDecalsManager().AddImpactDecal(ProjectileType.Arrow, materialEffectType, hit.point, hit.normal);

                BulletImpactEffectType typeBasedOnMaterial = GetImpactEffectTypeBasedOnMaterial(surfaceTag);
                BulletImpactEffectPool impactEffectPool = GameManager.GetEffectPoolManager().GetBulletImpactEffectPool();
                if (typeBasedOnMaterial == BulletImpactEffectType.BulletImpactEffect_Untagged)
                    impactEffectPool.SpawnUntilParticlesDone(BulletImpactEffectType.BulletImpactEffect_Stone, hit.point, GameManager.GetVpFPSCamera().transform.rotation);
                else
                    impactEffectPool.SpawnUntilParticlesDone(typeBasedOnMaterial, hit.point, GameManager.GetVpFPSCamera().transform.rotation);
                MaterialEffectType materialEffectType2 = ImpactDecals.MapBulletImpactEffectTypeToMaterialEffectType(typeBasedOnMaterial);
                GameManager.GetDynamicDecalsManager().AddImpactDecal(ProjectileType.Bullet, materialEffectType2, hit.point, GameManager.GetVpFPSCamera().transform.forward);
                if (!hit.collider || !hit.collider.gameObject)
                    return;
                GameAudioManager.SetMaterialSwitch(surfaceTag, hit.collider.gameObject);
                Transform V3 = GameManager.GetPlayerTransform();
                GameObject Player = GameManager.GetPlayerObject();
                int num = (int)AkSoundEngine.PostEvent(AK.EVENTS.PLAY_BULLETIMPACTS, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
                GameAudioManager.SetAudioSourceTransform(Player, V3);
                Comps.PlayerBulletDamage PlayerDamage = hit.collider.gameObject.GetComponent<Comps.PlayerBulletDamage >();
                if (PlayerDamage != null)
                {
                    float Damage = PlayerDamage.m_Damage / 3 + GetMeelePlayerInfo(Weapon).m_PlayerDamage;
                    MelonLogger.Msg("You damaged other player on " + Damage);
                    int bodypart = PlayerDamage.m_Type;
                    if (sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.BULLETDAMAGE))
                        {
                            _packet.Write(Damage);
                            _packet.Write(bodypart);
                            _packet.Write(PlayerDamage.m_ClientId);
                            _packet.Write(true);
                            _packet.Write(Weapon);
                            SendUDPData(_packet);
                        }
                    }
                    if (iAmHost == true)
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.BULLETDAMAGE))
                        {
                            ServerSend.BULLETDAMAGE(PlayerDamage.m_ClientId, Damage, bodypart, 0, true, Weapon);
                        }
                    }
                    return;
                }
                BaseAi baseAiFromObject = AiUtils.GetBaseAiFromObject(hit.collider.gameObject);
                Comps.AnimalActor ActorFromObject;
                if (!hit.collider.gameObject)
                {
                    ActorFromObject = null;
                }
                else if (hit.collider.gameObject.layer == 16)
                {
                    ActorFromObject = hit.collider.gameObject.GetComponent<Comps.AnimalActor>();
                } else {
                    if (hit.collider.gameObject.layer == 27)
                    {
                        ActorFromObject = hit.collider.gameObject.transform.GetComponentInParent<Comps.AnimalActor>();
                    } else {
                        ActorFromObject = null;
                    }
                }

                if (baseAiFromObject != null)
                {
                    MelonLogger.Msg("This is baseAI animal");
                    LocalizedDamage component = hit.collider.GetComponent<LocalizedDamage>();
                    float bleedOutMinutes = component.GetBleedOutMinutes(BodyDamage.Weapon.Rifle);
                    float Damage = GetMeelePlayerInfo(Weapon).m_AnimalDamage * component.GetDamageScale(BodyDamage.Weapon.Rifle);

                    if (!Utils.IsZero(Damage) || baseAiFromObject.ForceApplyDamage())
                    {
                        if (baseAiFromObject.GetAiMode() != AiMode.Dead)
                        {
                            GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.CarcassHarvesting, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                        }
                        baseAiFromObject.SetupDamageForAnim(hit.collider.transform.position, GameManager.GetPlayerTransform().position, component);
                        baseAiFromObject.ApplyDamage(Damage, bleedOutMinutes, DamageSource.Player, hit.collider.name);
                    }
                    return;
                }
                if (ActorFromObject != null)
                {
                    LocalizedDamage component = hit.collider.GetComponent<LocalizedDamage>();
                    float bleedOutMinutes = component.GetBleedOutMinutes(BodyDamage.Weapon.Rifle);
                    float Damage = GetMeelePlayerInfo(Weapon).m_AnimalDamage * component.GetDamageScale(BodyDamage.Weapon.Rifle);

                    if (Damage > 0 && ActorFromObject.m_Hp > 0)
                    {
                        GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.CarcassHarvesting, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                    }
                    if (iAmHost)
                    {
                        ServerSend.ANIMALDAMAGE(0, ActorFromObject.gameObject.GetComponent<ObjectGuid>().Get(), Damage);
                    }
                    else if (sendMyPosition)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.ANIMALDAMAGE))
                        {
                            _packet.Write(ActorFromObject.gameObject.GetComponent<ObjectGuid>().Get());
                            _packet.Write(Damage);
                            SendUDPData(_packet);
                        }
                    }
                    return;
                }
            }
        }
        public static bool IgnoreEmoteKeyDownUntilReleased = false;

        public static DataStr.MultiplayerEmote GetEmoteByID(int ID)
        {
            DataStr.MultiplayerEmote Emote = new DataStr.MultiplayerEmote();
            if (ID == 0)
            {
                Emote.m_Name = "Maraschino";
                Emote.m_Animation = "Cringe1";
                Emote.m_ForceStandup = true;
            }
            else if (ID == 1)
            {
                Emote.m_Name = "Breakdance";
                Emote.m_Animation = "Cringe2";
                Emote.m_ForceStandup = true;
            }
            else if (ID == 2)
            {
                Emote.m_Name = "Samba";
                Emote.m_Animation = "Flex";
                Emote.m_ForceStandup = true;
            } 
            else if(ID == 3)
            {
                Emote.m_Name = "Thumbs Up";
                Emote.m_Animation = "DoThumbsUp";
                Emote.m_LeftHandEmote = true;
            } else if (ID == 4)
            {
                Emote.m_Name = "Middle Finger";
                Emote.m_Animation = "DoMiddleFinger";
                Emote.m_LeftHandEmote = true;
            } else if(ID == 5)
            {
                Emote.m_Name = "Point";
                Emote.m_Animation = "DoPoint";
                Emote.m_LeftHandEmote = true;
            } else if(ID == 6)
            {
                Emote.m_Name = "Wave";
                Emote.m_Animation = "DoWave";
                Emote.m_LeftHandEmote = true;
            } else
            {
                Emote.m_Name = "Maraschino";
                Emote.m_Animation = "Cringe1";
                Emote.m_ForceStandup = true;
            }

            return Emote;
        }

        public static void PerformEmote(int EmoteID)
        {
            if (MyPlayerDoll)
            {
                MyPlayerDoll.transform.rotation = GameManager.GetPlayerTransform().rotation;
            }
            IgnoreEmoteKeyDownUntilReleased = true;
            DataStr.MultiplayerEmote Emote = GetEmoteByID(EmoteID);
            if (!Emote.m_LeftHandEmote)
            {
                MyEmote = GetEmoteByID(EmoteID);
            } else
            {
                if (MyPlayerDoll)
                {
                    MyPlayerDoll.GetComponent<Comps.MultiplayerPlayerAnimator>().DoLeftHandEmote(Emote.m_Animation);
                }
                if (iAmHost)
                {
                    ServerSend.TRIGGEREMOTE(0, EmoteID);
                }
                if (sendMyPosition)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.TRIGGEREMOTE))
                    {
                        _packet.Write(EmoteID);
                        SendUDPData(_packet);
                    }
                }
            }
        }

        public static void UpdateEmoteWheel()
        {
            if (EmoteWheel)
            {
                bool IsDown = KeyboardUtilities.InputManager.GetKey(KeyCode.T);

                if ((chatInput != null && chatInput.gameObject.activeSelf == true) || (uConsole.m_Instance != null && uConsole.m_On == true))
                {
                    IsDown = false;
                }


                if (IsDown && IgnoreEmoteKeyDownUntilReleased == false)
                {
                    EmoteWheel.SetActive(true);
                    InputManager.m_CursorState = InputManager.CursorState.Show;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    int Selected = -1;
                    for (int i = 0; i <= 7; i++)
                    {
                        UnityEngine.UI.Button btn = EmoteWheel.transform.GetChild(i).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Button>();
                        if (btn._isPointerInside_k__BackingField)
                        {
                            Selected = i;
                            break;
                        }
                    }
                    if (Selected != -1)
                    {
                        EmoteWheel.transform.GetChild(8).gameObject.SetActive(true);
                        EmoteWheel.transform.GetChild(8).GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = GetEmoteByID(Selected).m_Name;
                    } else {
                        EmoteWheel.transform.GetChild(8).gameObject.SetActive(false);
                    }
                }
                else {
                    EmoteWheel.SetActive(false);
                }

                if (IgnoreEmoteKeyDownUntilReleased && !IsDown)
                {
                    IgnoreEmoteKeyDownUntilReleased = false;
                }
            }
        }

        public static void PushActionToMyDoll(string Act)
        {
            if (MyPlayerDoll)
            {
                Comps.MultiplayerPlayerAnimator Anim = MyPlayerDoll.GetComponent<Comps.MultiplayerPlayerAnimator>();
                if (Anim)
                {
                    if (Act == "Pickup")
                    {
                        Anim.Pickup();
                    } else if (Act == "Eat" || Act == "Drink")
                    {
                        Anim.m_IsDrink = Act == "Drink";
                        Anim.Consumption();
                    } else if (Act == "Shoot")
                    {
                        if (MyLightSourceName == "GEAR_Rifle")
                        {
                            string shootStrhing = "RifleShoot";
                            if (MyAnimState == "Ctrl")
                            {
                                shootStrhing = "RifleShoot_Sit";
                            }
                            Anim.m_PreAnimStateHands = shootStrhing;
                        }
                        if (MyPlayerDoll.activeSelf)
                        {
                            DoShootFX(MyPlayerDoll.transform.position);
                        }
                    } else if (Act == "Melee")
                    {
                        Anim.MeleeAttack();
                    }
                }
            }
        }

        public static void UpdateDoll()
        {
            if (MyPlayerDoll && GameManager.m_PlayerObject != null)
            {
                if (CustomizeUi && CustomizeUi.activeSelf)
                {
                    MyPlayerDoll.SetActive(true);
                }
                else
                {
                    MyPlayerDoll.SetActive(FlyMode.m_Enabled);
                    if (FlyMode.m_Enabled)
                    {
                        MyPlayerDoll.transform.position = GameManager.GetPlayerTransform().position;
                        MyPlayerDoll.transform.rotation = GameManager.GetPlayerTransform().rotation;
                    }
                }
            }
        }

        public static bool QuitWhenSaveOperationFinished = false;
        public static long PendingSaveHashToSend = 0;


        public static void SaveCheck()
        {
            if (QuitWhenSaveOperationFinished)
            {
                if (SaveGameData.m_PendingSaveGameOperations.Count == 0)
                {
                    MelonLogger.Msg("Saving on disk done");
                    if (sendMyPosition)
                    {
                        if (PendingSaveHashToSend != 0)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.SAVEHASH))
                            {
                                _packet.Write(MyUGUID);
                                _packet.Write(PendingSaveHashToSend);
                                _packet.Write(true);
                                SendUDPData(_packet, true);
                            }
                        }
                        MelonLogger.Msg("Done sending, now quit");
                    }
                    QuitWhenSaveOperationFinished = false;
                    IReallyWantToQuit = true;
                    Application.Quit();
                }
            }
        }

        public static void UIsWorking()
        {
            if (UiCanvas == null)
            {
                BuildCanvasUIs(); // Creating canvas based UI
            }else{
                UpdateCanvasUis(); // Upating text and other UI
            }
            if (m_InterfaceManager != null)
            {
                if (NeedConnectAfterLoad != -1 && InterfaceManager.m_Panel_Confirmation != null)
                {
                    DoWaitForConnect(true); // Showing connection message after startup connection
                }
                if (InterfaceManager.m_Panel_SnowShelterInteract != null)
                {
                    CancleDismantling(); // Cancle Break-down of snowshelter where any other player is inside
                }
                if (InterfaceManager.m_Panel_Rest != null && InterfaceManager.m_Panel_Rest.isActiveAndEnabled == true)
                {
                    if (SleepingButtons == null)
                    {
                        BuildSleepScreen(); // Patching up sleeping screen
                    }else{
                        UpdateSleepScreen(); // Upating sleepers numbers text
                    }
                }
                if (InterfaceManager.m_Panel_BodyHarvest != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    HarvestingAnimal = InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>().Get();
                }else{
                    HarvestingAnimal = "";
                }
                SendHarvestingAnimal(); // Send harvesting animal if it changed 

                if (GameManager.m_PlayerManager)
                {
                    if (InOnline())
                    {
                        CancleIfSomeoneDoSame(); // Stop actions if another player doing same with that object.
                    }
                }

                if (CurrentCustomChalleng.m_Started || OnExpedition)
                {
                    if (InterfaceManager.m_Panel_Actions != null && InterfaceManager.m_Panel_Actions.isActiveAndEnabled)
                    {
                        InterfaceManager.m_Panel_Actions.m_MissionObjectWithTimer.SetActive(true);
                        InterfaceManager.m_Panel_Actions.m_MissionObjectiveWithTimerLabel.gameObject.SetActive(true);
                    }
                    if (OnExpedition)
                    {
                        ExpeditionUI(ExpeditionLastName, ExpeditionLastTaskText, ExpeditionLastTime);
                    } else if (CurrentCustomChalleng.m_Started)
                    {
                        PatchChallange();
                    }
                }
                UpdateEmoteWheel();
            }
            if (MyEmote != null)
            {
                if (MyEmote.m_FollowDollCamera)
                {
                    vp_FPSCamera vpFpsCamera = GameManager.GetVpFPSCamera();
                    Quaternion quaternion = DollCameraDummy.transform.rotation;
                    float pitch = quaternion.eulerAngles.x + vpFpsCamera.m_BasePitch;
                    float y = quaternion.eulerAngles.y;
                    vpFpsCamera.SetAngle(y, pitch);
                }
                bool IsCrouched = GameManager.GetPlayerManagerComponent().PlayerIsCrouched();
                if (MyEmote.m_ForceStandup)
                {
                    if (IsCrouched)
                    {
                        GameManager.GetVpFPSPlayer().InputCrouch();
                    }
                }
                else if (MyEmote.m_ForceCrouch)
                {
                    if (!IsCrouched)
                    {
                        GameManager.GetVpFPSPlayer().InputCrouch();
                    }
                }
            }
            if (level_name == "MainMenu")
            {
                if (NewFlairNotification)
                {
                    if (m_Panel_MainMenu && m_Panel_MainMenu.m_MainPanel && m_Panel_MainMenu.m_MainPanel.gameObject.activeSelf && InterfaceManager.m_Panel_Confirmation && !InterfaceManager.m_Panel_Confirmation.isActiveAndEnabled)
                    {
                        NewFlairNotification.SetActive(NotificationString != "");
                        if (NotificationString != "")
                        {
                            NewFlairNotification.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = NotificationString;
                        }
                    }else{
                        NewFlairNotification.SetActive(false);
                    }
                }
            }
            if (CustomizeUi && CustomizeUi.activeSelf)
            {
                if (level_name == "MainMenu")
                {
                    UpdateDoll();
                }
            }
        }

        public static void CustomWeapons()
        {
            if (GameManager.m_PlayerManager != null && GameManager.GetPlayerManagerComponent().m_ItemInHands)
            {
                string InHandName = GameManager.GetPlayerManagerComponent().m_ItemInHands.m_GearName;

                if (ViewModelRadio)
                {
                    if (GameManager.GetPlayerManagerComponent().m_ItemInHands.GetFPSMeshID() == (int)FPSMeshID.HandledShortwave)
                    {
                        ViewModelRadio.SetActive(true);
                        if (!OriginalRadioSeaker)
                        {
                            UpdateRadio();
                        }
                    } else
                    {
                        ViewModelRadio.SetActive(false);
                        SearchModeActive = false;
                    }
                } else
                {
                    MelonLogger.Msg("No ViewModelRadio mesh");
                }

                if (IsCustomHandItem(InHandName))
                {
                    if (ViewModelDummy)
                    {
                        ViewModelDummy.SetActive(false);
                    } else
                    {
                        MelonLogger.Msg("No ViewModelDummy mesh");
                    }

                    if (InputManager.GetFirePressed(InputManager.m_CurrentContext))
                    {
                        bool ShouldAttack = true;
                        if (ShouldReEquipFaster && (InHandName == "GEAR_Knife" || InHandName == "GEAR_KnifeImprovised" || InHandName == "GEAR_KnifeScrapMetal" || InHandName == "GEAR_JeremiahKnife"))
                        {
                            ShouldAttack = false;
                        }
                        if (ShouldAttack)
                        {
                            PerformMeleeAttack(InHandName);
                        }
                    }
                }else if(InHandName == "GEAR_SCNote")
                {
                    if (InputManager.GetFirePressed(InputManager.m_CurrentContext))
                    {
                        ReadNoteInHands();
                    }
                }

                if (ViewModelHatchet)
                {
                    ViewModelHatchet.SetActive(InHandName == "GEAR_Hatchet");
                }
                if (ViewModelHatchet2)
                {
                    ViewModelHatchet2.SetActive(InHandName == "GEAR_HatchetImprovised");
                }
                if (ViewModelHammer)
                {
                    ViewModelHammer.SetActive(InHandName == "GEAR_Hammer");
                }
                if (ViewModelKnife)
                {
                    ViewModelKnife.SetActive(InHandName == "GEAR_Knife");
                }
                if (ViewModelJeremiahKnife)
                {
                    ViewModelJeremiahKnife.SetActive(InHandName == "GEAR_JeremiahKnife");
                }
                if (ViewModelKnife2)
                {
                    ViewModelKnife2.SetActive(InHandName == "GEAR_KnifeImprovised" || InHandName == "GEAR_KnifeScrapMetal");
                }
                if (ViewModelPrybar)
                {
                    ViewModelPrybar.SetActive(InHandName == "GEAR_Prybar");
                }
                if (ViewModelShovel)
                {
                    ViewModelShovel.SetActive(InHandName == "GEAR_Shovel");
                }
                if (ViewModelFireAxe)
                {
                    ViewModelFireAxe.SetActive(InHandName == "GEAR_FireAxe");
                }
                if (ViewModelHackSaw)
                {
                    ViewModelHackSaw.SetActive(InHandName == "GEAR_Hacksaw");
                }
                if (ViewModelPhoto)
                {
                    ViewModelPhoto.SetActive(InHandName == "GEAR_SCPhoto");
                }
                if (ViewModelMap)
                {
                    ViewModelMap.SetActive(InHandName == "GEAR_SCMapPiece");
                }
                if (ViewModelStone)
                {
                    ViewModelStone.SetActive(InHandName == "GEAR_Stone");
                }
                if (ViewModelNote)
                {
                    ViewModelNote.SetActive(InHandName == "GEAR_SCNote");
                }

                if (UseBoltInsteadOfStone)
                {
                    if (ViewModelStone)
                    {
                        ViewModelStone.SetActive(false);
                    }
                    if (ViewModelBolt)
                    {
                        ViewModelBolt.SetActive(InHandName == "GEAR_Stone");
                    }
                }
            }
        }
        public static void ProcessVoice()
        {
            if (SteamServerWorks != "" || Server.UsingSteamWorks == true)
            {
                bool HoldRadio = false;
                if (GameManager.m_PlayerManager && GameManager.m_PlayerManager.m_ItemInHands && GameManager.m_PlayerManager.m_ItemInHands.GetFPSMeshID() == (int)FPSMeshID.HandledShortwave)
                {
                    HoldRadio = true;
                }

                TrackWhenRecordOver(HoldRadio); // Processing voice chat
            }
        }

        public void UpdateDeathFlag()
        {
            if (GameManager.m_Condition)
            {
                IsDead = GameManager.m_Condition.m_CurrentHP <= 0;
            }
        }

        public void SendPlayerData()
        {
            if(GameManager.m_PlayerObject != null)
            {
                Transform transf = GameManager.GetPlayerTransform();
                bool InFight = GameManager.GetPlayerStruggleComponent().InStruggle();
                UpdateMyEquipment(); // Resend my current equipment if needed.
                UpdateVisualSyncs(); // Resend some effects if needed. (Bloodloss, heavy breath).

                if (transf)
                {
                    if (previoustickpos != transf.position) // If moving
                    {
                        MyEmote = null;
                        previoustickpos = GameManager.GetPlayerTransform().position;
                        SyncMovement(InFight, transf); // Send movement position and sets runing/walking animation.
                    }else{
                        SetCurrentAnimation(InFight); // Sets animations when not moving, actions and almost everything.
                    }
                    if (previoustickrot != transf.rotation) // If rotating
                    {
                        previoustickrot = GameManager.GetPlayerTransform().rotation;
                        SyncRotation(transf); // Send rotation of player model.
                    }
                }
                SendMyAnimation(); //Send current animation if changed.
                SyncSleeping(); //Send sleeping hours and sleep position, if changed.

                if (InputManager.GetInteractReleased(InputManager.m_CurrentContext))
                {
                    if (PlayerInteractionWith != null)
                    {
                        LongActionCanceled(PlayerInteractionWith);
                    }
                    if (InteractionInprocess && InteractHold)
                    {
                        LongActionCanceled();
                    }
                }
            }
        }

        public static void InputActions()
        {
            if (GameManager.m_PlayerManager == null)
            {
                return;
            }
            PlayerManager __PM = GameManager.m_PlayerManager;


            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.G))
            {
                FindPlayerToTrade(); // Find closest player to give item to.
                ProcessGivingItem(); // Attempt to give item to nearest player.
            }
            if (InputManager.GetPutBackPressed(InputManager.m_CurrentContext))
            {
                if (__PM.m_Gear != null && __PM.m_InspectModeActive == true)
                {
                    GearItem GI = __PM.m_Gear;
                    GameObject GIobj = GI.gameObject;
                    int variant = 0;
                    if ((GIobj.GetComponent<FoodItem>() != null && GIobj.GetComponent<FoodItem>().m_Opened == true) ||
                        (GIobj.GetComponent<SmashableItem>() != null && GIobj.GetComponent<SmashableItem>().m_HasBeenSmashed == true))
                    {
                        variant = 1;
                    }

                    if (__PM.m_Container != null)
                    {
                        MelonLogger.Msg("Refuse taking gear, from container, make it fake");

                        SendDropItem(GI, 0, 0, false, variant);
                    }else{
                        if (GI.gameObject.GetComponent<Comps.DropFakeOnLeave>() != null)
                        {
                            MelonLogger.Msg("Refuse from picking fake gear, make it fake again");

                            SendDropItem(GI, 0, 0, true, variant);
                        }
                    }
                }
            }
            if (InteractionInprocess)
            {
                InteractTimer -= Time.deltaTime;

                if (ObjectInteractWith == null)
                {
                    LongActionCanceled();
                    return;
                }

                if (InteractTimer <= 0.0)
                {
                    if (ObjectInteractWith)
                    {
                        LongActionFinished(ObjectInteractWith, InteractionType);
                    }
                    LongActionCanceled();
                }
            }
            if (Pathes.FakeRockCacheCallback != null)
            {
                for (int i = 0; i < playersData.Count; i++)
                {
                    if (playersData[i] != null)
                    {
                        if (playersData[i].m_BrakingObject != null)
                        {
                            if (Pathes.FakeRockCacheCallback.m_GUID == playersData[i].m_BrakingObject.m_Guid)
                            {
                                HUDMessage.AddMessage(playersData[i].m_Name + " IS BREAKING THIS!");
                                GameAudioManager.PlayGUIError();
                                InterfaceManager.m_Panel_GenericProgressBar.Cancel();
                                break;
                            }
                        }
                        if (playersData[i].m_Container != null)
                        {
                            if (Pathes.FakeRockCacheCallback.m_GUID == playersData[i].m_Container.m_Guid)
                            {
                                HUDMessage.AddMessage(playersData[i].m_Name + " IS USING THIS!");
                                GameAudioManager.PlayGUIError();
                                InterfaceManager.m_Panel_GenericProgressBar.Cancel();
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static void SetTime()
        {
            string todString = OveridedTime;
            float normalizedTOD;
            if (Utils.TryParseTOD(todString, out normalizedTOD))
            {
                GameManager.GetTimeOfDayComponent().SetNormalizedTime(normalizedTOD);
            }
            if (GameManager.m_ExperienceModeManager != null)
            {
                GameManager.m_ExperienceModeManager.GetCurrentExperienceMode().m_DecayScale = 0;
            }
        }
        public static void SetAppBackgroundMode()
        {
            if (Application.runInBackground == false) 
            { 
                Application.runInBackground = true; // Always running in bg, to not lost any sync packets
            } 
        }

        public override void OnUpdate()
        {
            SetAppBackgroundMode();
            if (!KillOnUpdate)
            {
                ModUpdate();
                Shared.OnUpdate();
            }

            if (!KillEverySecond)
            {
                if (Time.time > nextActionTimeSecond)
                {
                    nextActionTimeSecond += periodSecond;
                    if (!KillEverySecond)
                    {
                        EverySecond(); // Triggering this code very second
                        Shared.EverySecond();
                    }
                }
            }
            if (!KillEveryInGameMinute)
            {
                if (Time.time > nextActionTime)
                {
                    nextActionTime += period;
                    if (NeedSyncTime == true)
                    {
                        EveryInGameMinute(); // Trigger actions that happends every in game minute (5 seconds) and recalculate realtime cycle and resend weather.
                        if (iAmHost)
                        {
                            Shared.EveryInGameMinute();
                        }
                    }
                }
            }
            CheckSeeingRefMan();
        }

        public static bool Flag1 = true;
        public static bool Flag2 = true;
        public static bool Flag3 = true;
        public static bool Flag4 = true;
        public static bool Flag5 = true;
        public static bool Flag6 = true;
        public static bool Flag7 = true;
        public static bool Flag8 = true;
        public static bool Flag9 = true;
        public static bool Flag10 = true;
        public static bool Flag11 = true;
        public static bool Flag12 = true;
        public static bool Flag13 = true;
        public void ModUpdate()
        {
            SaveCheck();
            if (!DedicatedServerAppMode) 
            {
                if (Flag1)
                {
                    UIsWorking();
                }
                if (Flag2)
                {
                    DebugCrap();
                }
                if (Flag3)
                {
                    CustomWeapons();
                }
                if (Flag4)
                {
                    InitAudio(); // Adding audio listener if needed
                }
                if (Flag5)
                {
                    ProcessVoice();
                }
                if (Flag6)
                {
                    UpdateDeathFlag();
                }
                if (level_name != "Empty" && level_name != "Boot" && level_name != "MainMenu")
                {
                    if (Flag7)
                    {
                        UpdateDoll();
                    }
                    if (Flag8)
                    {
                        SendPlayerData();
                    }
                    if (Flag9)
                    {
                        InputActions();
                    }
                }
            }
            else // If Dedicated mode, doing specific code for it
            { 
                DedicatedServerUpdate(); // Updating dedicated settings
            }
            if (InOnline())
            {
                if (Flag10)
                {
                    SetTime();
                }
            }
            if (Flag11)
            {
                ThreadManager.UpdateMain(); // Updating sync tread
            }
            if (Flag12)
            {
                UpdateAPIStates(); // Updating ID and Client/Host state for API
            }
            if (Flag13)
            {
                SteamConnect.DoUpdate(); // Start tracking of incomming data from other players
            }
        }

        public static bool NoUI = false;
        public static bool ForcedUiOn = true;


        public static bool AtHostMenu = false;

        public static void LoadAllOpenableThingsForScene()
        {
            string Scene = level_guid;
            MelonLogger.Msg(ConsoleColor.Blue, "Trying to load openables for scene " + Scene);
            Dictionary<string, bool> Opens = MPSaveManager.LoadOpenableThings(Scene);
            if (Opens != null)
            {
                foreach (var item in Opens)
                {
                    if (!OpenableThings.ContainsKey(item.Key))
                    {
                        OpenableThings.Add(item.Key, item.Value);
                    }
                }
            }
        }


        public static void LoadAllDropsForScene()
        {
            string Scene = level_guid;
            MelonLogger.Msg(ConsoleColor.Blue, "Trying to load drops for scene " + Scene);
            Shared.ModifyDynamicGears(Scene);
            Dictionary<int, DataStr.DroppedGearItemDataPacket> Visuals = MPSaveManager.LoadDropVisual(Scene);
            Dictionary<int, DataStr.SlicedJsonDroppedGear> Drops = MPSaveManager.LoadDropData(Scene);


            if (Drops != null && Visuals != null)
            {
                foreach (var item in Visuals)
                {
                    Shared.FakeDropItem(item.Value, true);
                }
            }
        }

        public static void SetFixedSpawn()
        {
            if (ServerConfig.m_PlayersSpawnType != 3) // Fixed spawn only
            {
                return;
            }

            MelonLogger.Msg("[FixedPlaceSpawns] SetFixedSpawn()");
            if (FixedPlaceLoaded == false)
            {
                SavedSceneForSpawn = level_name;
                SavedPositionForSpawn = GameManager.GetPlayerTransform().position;
                MelonLogger.Msg("[FixedPlaceSpawns] Isn't saved yet, set current position");
            }
            MelonLogger.Msg("[FixedPlaceSpawns] Scene: " + SavedSceneForSpawn + " position X " + SavedPositionForSpawn.x + " Y " + SavedPositionForSpawn.y + " Z " + SavedPositionForSpawn.z);
        }

        public static void DoIPConnectWindow()
        {
            if (sendMyPosition != true)
            {
                if (SteamServerWorks == "")
                {
                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input server address", "127.0.0.1", Panel_Confirmation.ButtonLayout.Button_2, "Connect", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                }
                else {
                    if (SteamConnect.CanUseSteam == true)
                    {
                        SteamConnect.Main.JoinToLobby(SteamServerWorks);
                    }
                }
            }
            else
            {
                HUDMessage.AddMessage("YOU ALREADY CONNECTED!!!!");
            }
        }

        public static bool AtDebug = false;
        public static string UIDebugType = "";
        public static List<string> ItemsForDebug = new List<string>();
        public static int ItemForDebug = -1;
        public static string AdvancedDebugMode = "";
        public static bool DebugGUI = false;
        public static bool DebugBind = false;

        public override void OnGUI()
        {
            DeBugMenu.Render();
        }

        public static void SetRevivedStats(bool health = true)
        {
            if (health == true)
            {
                GameManager.GetConditionComponent().m_CurrentHP = 25;
            }
            IsDead = false;
            GameManager.GetPlayerManagerComponent().m_Ghost = false;
            GameManager.GetFatigueComponent().m_CurrentFatigue = 25;
            GameManager.GetThirstComponent().m_CurrentThirst = 25;
            GameManager.GetHungerComponent().m_CurrentReserveCalories = GameManager.GetHungerComponent().m_MaxReserveCalories / 4;
        }

        public static bool RollChanceSeeded(float percent, System.Random RNG) => !Utils.IsZero(percent, 0.0001f) && (Utils.Approximately(percent, 100f, 0.0001f) || (double)RNG.Next(0, 100) < (double)percent);

        public static string GenerateSeededGUID(int gameSeed, Vector3 v3)
        {
            int _x = (int)v3.x;
            int _y = (int)v3.y;
            int _z = (int)v3.z;
            int v3Int = _x + _y + _z;
            int newSeed = gameSeed + v3Int;
            //MelonLogger.Msg("[Seeded GUIDs] Input Seed " + gameSeed + " v3 Seed " + v3Int+" Combined seed "+ newSeed);
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
            //MelonLogger.Msg("[Seeded GUIDs] Final GUID "+ newGUID);
            return newGUID;
        }

        public static void AddLocksToDoorsByGUID(string GUID)
        {
            foreach (LoadScene Door in LoadScene.m_LoadScenesList)
            {
                if (Door.m_GUID == GUID)
                {
                    AddLock(Door);
                }
            }
        }
        public static void EnterDoorsByGUID(string GUID)
        {
            foreach (LoadScene Door in LoadScene.m_LoadScenesList)
            {
                if (Door.m_GUID == GUID)
                {
                    Door.Activate();
                    break;
                }
            }
        }
        public static void EnterDoorsByScene(string Scene)
        {
            foreach (LoadScene Door in LoadScene.m_LoadScenesList)
            {
                if (Door.m_SceneToLoad + "_" + Door.m_GUID == Scene)
                {
                    Door.Activate();
                    break;
                }
            }
        }
        public static void RemoveLocksFromDoorsByGUID(string GUID)
        {
            foreach (LoadScene Door in LoadScene.m_LoadScenesList)
            {
                if (Door.m_GUID == GUID)
                {
                    if (Door.m_Lock)
                    {
                        UnityEngine.Object.Destroy(Door.gameObject.GetComponent<Lock>());
                    }
                    if (Door.gameObject.GetComponent<HoverIconsToShow>())
                    {
                        UnityEngine.Object.Destroy(Door.gameObject.GetComponent<HoverIconsToShow>());
                    }
                    if (Door.gameObject.GetComponent<Comps.DoorLockedOnKey>())
                    {
                        UnityEngine.Object.Destroy(Door.gameObject.GetComponent<Comps.DoorLockedOnKey>());
                    }
                }
            }
        }

        public static void TryLockDoor(LoadScene Door, string KeySeed)
        {
            if (Door)
            {
                if (Door.m_SceneCanBeInstanced && !string.IsNullOrEmpty(Door.m_GUID) && Door.m_Active)
                {
                    GameAudioManager.PlaySound("PLAY_SNDMECHSAFETUMBLERFALL", InterfaceManager.GetSoundEmitter());
                    if (iAmHost)
                    {
                        TryingLockDoor(Door, KeySeed, level_guid);
                    }
                    else if (sendMyPosition)
                    {
                        DoPleaseWait("Please wait...", "Locking door...");
                        using (Packet _packet = new Packet((int)ClientPackets.ADDDOORLOCK))
                        {
                            _packet.Write(Door.GetSceneToLoad() + "_" + Door.m_GUID);
                            _packet.Write(KeySeed);
                            _packet.Write(level_guid);
                            SendUDPData(_packet);
                        }
                    }
                }
            }
        }

        public static bool AddLock(LoadScene Door)
        {
            GameObject DoorOBJ = Door.gameObject;
            if (DoorOBJ)
            {
                Lock L = DoorOBJ.AddComponent<Lock>();
                Door.m_Lock = L;
                L.m_LockStateRolled = true;
                L.m_LockState = LockState.Locked;
                L.m_BreakOnUse = false;
                L.m_ModeFilter = GameModeFilter.All;
                L.m_HideToolRequiredToForceOpenHintText = false;
                L.m_GearPrefabToForceLock = null;
                L.m_LockedAudio = "Play_SndMechDoorWoodLocked01";
                Comps.DoorLockedOnKey DoorLock = DoorOBJ.AddComponent<Comps.DoorLockedOnKey>();
                DoorLock.m_DoorKey = Door.GetSceneToLoad() + "_" + Door.m_GUID;
                return true;
            }
            return false;
        }
        public static string PendingKeySeed = "";
        public static string PendingKeyName = "";
        public static GameObject PendingRegisterKey = null;
        public static void BeginRegisterKey(GameObject key, bool Force = false)
        {
            if (key)
            {
                PendingRegisterKey = key;
            }

            if (!Force)
            {
                if (!GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_SharpeningStone"))
                {
                    HUDMessage.AddMessage("You need whetstone!");
                    GameAudioManager.PlayGUIError();
                    return;
                }
            }

            InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input secret seed for key", "", Panel_Confirmation.ButtonLayout.Button_2, "Next", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
        }
        public static void CreateDebugKey()
        {
            InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "DATA FOR DEBUG KEY", "", Panel_Confirmation.ButtonLayout.Button_2, "CRAFT", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
        }

        public static void TryingLockDoor(LoadScene Door, string KeySeed, string Scene)
        {
            string DoorKey = Door.GetSceneToLoad() + "_" + Door.m_GUID;
            MPSaveManager.UseKeyStatus Status = MPSaveManager.AddLockedDoor(Scene, DoorKey, KeySeed);
            if (Status == MPSaveManager.UseKeyStatus.Done)
            {
                ServerSend.ADDDOORLOCK(-1, Door.m_GUID, Scene);
                AddLocksToDoorsByGUID(Door.m_GUID);
            } else if (Status == MPSaveManager.UseKeyStatus.KeyUsed)
            {
                HUDMessage.AddMessage("This key already used for other door!");
            } else if (Status == MPSaveManager.UseKeyStatus.DoorAlreadyLocked)
            {
                HUDMessage.AddMessage("This door is already locked!");
            }
        }

        public static bool LoadingScreenIsOn = true;

        public static void SendRegularAlignData()
        {
            if (level_name == "Empty" || level_name == "Boot" || level_name == "MainMenu" || LoadingScreenIsOn == true)
            {
                return;
            }

            Vector3 v3 = GameManager.GetPlayerTransform().position;
            Quaternion rot = GameManager.GetPlayerTransform().rotation;

            if (GameManager.GetRestComponent().IsSleeping() == true && GameManager.GetRestComponent().m_Bed != null)
            {
                Bed _Bed = GameManager.GetRestComponent().m_Bed;
                if (_Bed != null)
                {
                    rot = GetBedRotation(_Bed);
                    v3 = _Bed.m_BodyPlacementTransform.position;
                }
            }

            if (iAmHost == true)
            {
                ServerSend.XYZ(0, v3, true);
                ServerSend.XYZW(0, rot, true);
                ServerSend.LEVELID(0, levelid, true);
                ServerSend.LEVELGUID(0, level_guid, true);
                ServerSend.ANIMSTATE(0, MyAnimState, true);
            }
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.XYZ))
                {
                    _packet.Write(v3);
                    SendUDPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.XYZW))
                {
                    _packet.Write(rot);
                    SendUDPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.LEVELID))
                {
                    _packet.Write(levelid);
                    SendUDPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.LEVELGUID))
                {
                    _packet.Write(level_guid);
                    SendUDPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.ANIMSTATE))
                {
                    _packet.Write(MyAnimState);
                    SendUDPData(_packet);
                }
            }
        }

        public static void SendSpawnData(bool AskResponce = false)
        {
            if (DedicatedServerAppMode)
            {
                return;
            }
            if (LoadingScreenIsOn == false)
            {
                return;
            }
            if (AskResponce == false)
            {
                MelonLogger.Msg("Sending my spawn data to all players");
            } else {
                MelonLogger.Msg("Sending spawn data as responce");
            }
            DataStr.PlayerClothingData Cdata = new DataStr.PlayerClothingData();
            Cdata.m_Hat = MyHat;
            Cdata.m_Top = MyTop;
            Cdata.m_Bottom = MyBottom;
            Cdata.m_Boots = MyBoots;
            int character = (int)GameManager.GetPlayerManagerComponent().m_VoicePersona;
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.SELECTEDCHARACTER))
                {
                    _packet.Write(character);
                    SendUDPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.LEVELID))
                {
                    _packet.Write(levelid);
                    SendUDPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.LEVELGUID))
                {
                    _packet.Write(level_guid);
                    SendUDPData(_packet);
                }
                SendMyEQ();
                using (Packet _packet = new Packet((int)ClientPackets.LIGHTSOURCE))
                {
                    _packet.Write(MyLightSource);
                    SendUDPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.LIGHTSOURCENAME))
                {
                    _packet.Write(MyLightSourceName);
                    SendUDPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.XYZ))
                {
                    _packet.Write(GameManager.GetPlayerTransform().position);
                    SendUDPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.XYZW))
                {
                    _packet.Write(GameManager.GetPlayerTransform().rotation);
                    SendUDPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.ANIMSTATE))
                {
                    _packet.Write(MyAnimState);
                    SendUDPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.CLOTH))
                {
                    _packet.Write(Cdata);
                    SendUDPData(_packet);
                }
                if (AskResponce == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.ASKSPAWNDATA))
                    {
                        _packet.Write(levelid);
                        SendUDPData(_packet);
                    }
                }
            }
            if (iAmHost == true)
            {
                SendMyEQ();
                ServerSend.SELECTEDCHARACTER(0, character, true);
                ServerSend.LIGHTSOURCE(0, MyLightSource, true);
                ServerSend.LIGHTSOURCENAME(0, MyLightSourceName, true);
                ServerSend.XYZ(0, GameManager.GetPlayerTransform().position, true);
                ServerSend.XYZW(0, GameManager.GetPlayerTransform().rotation, true);
                ServerSend.ANIMSTATE(0, MyAnimState, true);
                ServerSend.LEVELID(0, levelid, true);
                ServerSend.LEVELGUID(0, level_guid, true);
                ServerSend.CLOTH(0, Cdata, true);
                if (AskResponce == true)
                {
                    ServerSend.ASKSPAWNDATA(0, levelid, true);
                }
            }
        }

        public static void DoSteamWorksConnect(string sid)
        {
            ClientUser.PendingConnectionIp = SteamServerWorks;
            using (Packet _packet = new Packet((int)ClientPackets.CONNECTSTEAM))
            {
                _packet.Write(sid);
                SendUDPData(_packet);
            }
        }

        public static bool CanSleep(bool domessage)
        {
            Panel_Rest rest = InterfaceManager.m_Panel_Rest;

            if (rest == null)
            {
                return false;
            }

            if (GameManager.m_BlockAbilityToRest)
            {
                if (string.IsNullOrEmpty(GameManager.m_BlockedRestLocID))
                {
                    return false;
                }
                if (domessage == true)
                {
                    GameAudioManager.PlayGUIError();
                    HUDMessage.AddMessage(Localization.Get(GameManager.m_BlockedRestLocID));
                }
                return false;
            }
            else if (GameManager.GetCabinFeverComponent().HasCabinFever() && GameManager.GetPlayerManagerComponent().InHibernationPreventionIndoorEnvironment())
            {
                if (domessage == true)
                {
                    GameAudioManager.PlayGUIError();
                    HUDMessage.AddMessage(Localization.Get("GAMEPLAY_NoSleepIndoorsCabinFever"));
                }
                return false;
            }
            else if (!GameManager.GetRestComponent().AllowUnlimitedSleep() && (double)GameManager.GetFatigueComponent().m_CurrentFatigue <= (double)rest.m_AllowRestFatigueThreshold && !GameManager.GetRestComponent().RestNeededForAffliction())
            {
                if (domessage == true)
                {
                    GameAudioManager.PlayGUIError();
                    HUDMessage.AddMessage(Localization.Get("GAMEPLAY_YouAreNotTired"));
                }
                return false;
            }
            else if (GameManager.GetIceCrackingManager().IsInsideTrigger())
            {
                if (domessage == true)
                {
                    GameAudioManager.PlayGUIError();
                    HUDMessage.AddMessage(Localization.Get("GAMEPLAY_NoRestOnThinIce"));
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string GetClothForSlot(ClothingRegion s, ClothingLayer l)
        {
            if (GameManager.m_PlayerManager == null || GameManager.m_PlayerManager.m_Clothing == null)
            {
                return "";
            }

            GearItem gi = GameManager.GetPlayerManagerComponent().GetClothingInSlot(s, l);
            if (gi != null)
            {
                string dummyN = gi.m_GearName;
                string finalN = "";
                if (dummyN.Contains("(Clone)")) //If it has ugly (Clone), cutting it.
                {
                    int L = dummyN.Length - 7;
                    finalN = dummyN.Remove(L, 7);
                } else {
                    finalN = dummyN;
                }
                return finalN;
            } else {
                return "";
            }
        }

        public static string GetClothToDisplay(string bottom, string mid, string top = "", string top2 = "")
        {
            //2 layers select
            if (top == "" && top2 == "")
            {
                if (mid == "")
                {
                    return bottom;
                } else {
                    return mid;
                }
            } else {
                if (top2 != "")
                {
                    return top2;
                } else if (top != "")
                {
                    return top;
                } else if (mid != "")
                {
                    return mid;
                } else if (bottom != "")
                {
                    return bottom;
                } else {
                    return "";
                }
            }
        }

        public static bool IsScarf(string name)
        {
            if (name.Contains("Scarf"))
            {
                return true;
            } else {
                return false;
            }
        }
        public static bool IsBalaclava(string name)
        {
            if (name.Contains("Balaclava"))
            {
                return true;
            } else {
                return false;
            }
        }

        public static void HostMenuClose()
        {
            if (UIHostMenu != null)
            {
                UnityEngine.Object.Destroy(UIHostMenu);
                HostMenuHints = new List<GameObject>();
            }
            if (m_Panel_Sandbox && m_Panel_Sandbox.isActiveAndEnabled)
            {
                MenuChange.MenuMode = "Multiplayer";
                Transform Align = m_Panel_Sandbox.gameObject.transform.GetChild(0).GetChild(0).GetChild(5);
                Align.GetChild(1).gameObject.SetActive(true); //SelectIcon
                Align.GetChild(2).gameObject.SetActive(true); //Grid
                Align.GetChild(4).gameObject.SetActive(true); //Description
                Align.GetChild(5).gameObject.SetActive(true); //Linebreaker
            }
        }


        public static void HostMenuHost()
        {
            if (UIHostMenu != null)
            {
                GameObject dupesCheckbox = UIHostMenu.transform.GetChild(0).gameObject;
                GameObject dupesBoxesCheckbox = UIHostMenu.transform.GetChild(1).gameObject;
                bool DupesIsChecked = dupesCheckbox.GetComponent<UnityEngine.UI.Toggle>().isOn;
                bool BoxDupesIsChecked = dupesBoxesCheckbox.GetComponent<UnityEngine.UI.Toggle>().isOn;

                GameObject SpawnStyleList = UIHostMenu.transform.GetChild(2).gameObject;
                int spawnStyle = SpawnStyleList.GetComponent<UnityEngine.UI.Dropdown>().m_Value;

                GameObject PlayersMaxList = UIHostMenu.transform.GetChild(3).gameObject;
                int slotsMax = PlayersMaxList.GetComponent<UnityEngine.UI.Dropdown>().m_Value + 2;

                GameObject IsSteamHost = UIHostMenu.transform.GetChild(4).gameObject;
                bool ShouldUseSteam = IsSteamHost.GetComponent<UnityEngine.UI.Toggle>().isOn;

                //GameObject PublicSteamServer = UIHostMenu.transform.GetChild(5).gameObject;
                //bool IsPub = PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().isOn;

                GameObject PortsObject = UIHostMenu.transform.GetChild(8).gameObject;
                int PortToHost = Convert.ToInt32(PortsObject.GetComponent<UnityEngine.UI.InputField>().text);

                GameObject FireSyncObj = UIHostMenu.transform.GetChild(9).gameObject;
                int FireSyncMode = FireSyncObj.GetComponent<UnityEngine.UI.Dropdown>().m_Value;

                GameObject CheatsListObj = UIHostMenu.transform.GetChild(10).gameObject;
                int CheatsMode = CheatsListObj.GetComponent<UnityEngine.UI.Dropdown>().m_Value;

                GameObject SteamLobbyType = UIHostMenu.transform.GetChild(11).gameObject;
                int LobbyType = SteamLobbyType.GetComponent<UnityEngine.UI.Dropdown>().m_Value;
                ServerConfig.m_DuppedSpawns = DupesIsChecked;
                ServerConfig.m_DuppedContainers = BoxDupesIsChecked;
                ServerConfig.m_PlayersSpawnType = spawnStyle;
                ServerConfig.m_FireSync = FireSyncMode;
                ServerConfig.m_CheatsMode = CheatsMode;
                ServerConfig.m_PVP = true;
                MaxPlayers = slotsMax;
                ApplyOtherCampfires = true;

                Shared.InitAllPlayers();
                Transform Align = m_Panel_Sandbox.gameObject.transform.GetChild(0).GetChild(0).GetChild(5);
                Align.GetChild(1).gameObject.SetActive(true); //SelectIcon
                Align.GetChild(2).gameObject.SetActive(true); //Grid
                Align.GetChild(4).gameObject.SetActive(true); //Description
                Align.GetChild(5).gameObject.SetActive(true); //Linebreaker     

                if (ShouldUseSteam == false)
                {
                    StartServerAfterSelectSave = true;
                    PortsToHostOn = PortToHost;
                    MenuChange.ChangeMenuItems("LobbySettings");
                } else {
                    SteamConnect.Main.MakeLobby(LobbyType, MaxPlayers);
                    MenuChange.ChangeMenuItems("Lobby");
                }

                UnityEngine.Object.Destroy(UIHostMenu);
                HostMenuHints = new List<GameObject>();
                GameManager.m_IsPaused = false;


                DataStr.ServerSettingsData SaveData = new DataStr.ServerSettingsData();
                SaveData.m_CFG = ServerConfig;
                SaveData.m_MaxPlayers = MaxPlayers;
                SaveData.m_P2P = ShouldUseSteam;
                SaveData.m_Accessibility = LobbyType;
                SaveData.m_Port = PortToHost;

                MPSaveManager.SaveServerCFG(SaveData);
            }
        }

        public static void ShowNotiy()
        {
            if (m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
            {
                string Title = "YOU GOT A NEW FLAIR!";
                if (int.Parse(NotificationString) > 1)
                {
                    Title = "YOU GOT NEW FLAIRS!";
                }

                InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Confirm, Title, "\n" + "Open flairs customization menu?", Panel_Confirmation.ButtonLayout.Button_2, Panel_Confirmation.Background.Transperent, null, null);
            }
        }

        public static void OpenHint(GameObject obj)
        {
            if (obj.activeSelf == false)
            {
                for (int i = 0; i < HostMenuHints.Count; i++)
                {
                    HostMenuHints[i].SetActive(false);
                }
                obj.SetActive(true);
            } else {
                obj.SetActive(false);
            }
        }

        public static void AddHintScript(GameObject obj)
        {
            //MelonLogger.Msg("[AddHintScript] Started...");
            if (obj == null)
            {
                MelonLogger.Msg("[AddHintScript] Got null object. Why?");
                return;
            }
            GameObject hintButn = obj.transform.GetChild(0).gameObject;
            if (hintButn == null)
            {
                MelonLogger.Msg("[AddHintScript] " + obj.name + " have not hint button!");
                return;
            }
            GameObject hintObj = hintButn.transform.GetChild(0).gameObject;
            if (hintObj == null)
            {
                MelonLogger.Msg("[AddHintScript] " + obj.name + " have not hint panel!");
                return;
            }
            //MelonLogger.Msg("[AddHintScript] No errors found! Adding hints for "+ obj.name);
            HostMenuHints.Add(hintObj);
            Action actHint = new Action(() => OpenHint(hintObj));
            hintButn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(actHint);
        }

        public static List<GameObject> HostMenuHints = new List<GameObject>();

        public static void HostMenu(bool Ignore = false)
        {
            //if (CantBeUsedForMP == true || LastLoadedGenVersion != BuildInfo.RandomGenVersion || SaveGameSystem.m_CurrentGameMode == SaveSlotType.STORY)
            if (Ignore == false && (CantBeUsedForMP == true || LastLoadedGenVersion != BuildInfo.RandomGenVersion))
            {
                MelonLogger.Msg("CantBeUsedForMP " + CantBeUsedForMP + " LastLoadedGenVersion " + LastLoadedGenVersion + " RandomGenVersion " + BuildInfo.RandomGenVersion);
                return;
            }

            if (UiCanvas != null && UIHostMenu == null)
            {
                UIHostMenu = MakeModObject("MP_HostSettings", UiCanvas.transform);

                DataStr.ServerSettingsData SavedSettings = MPSaveManager.RequestServerCFG();
                if (SavedSettings != null)
                {
                    ServerConfig = SavedSettings.m_CFG;
                } else {
                    SavedSettings = new DataStr.ServerSettingsData();
                }

                GameObject CloseButton = UIHostMenu.transform.GetChild(6).gameObject;
                Action actBack = new Action(() => HostMenuClose());
                CloseButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(actBack);

                GameObject HostButton = UIHostMenu.transform.GetChild(7).gameObject;
                Action actHost = new Action(() => HostMenuHost());
                HostButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(actHost);

                GameObject dupesCheckbox = UIHostMenu.transform.GetChild(0).gameObject;
                GameObject dupesBoxesCheckbox = UIHostMenu.transform.GetChild(1).gameObject;
                dupesCheckbox.GetComponent<UnityEngine.UI.Toggle>().Set(ServerConfig.m_DuppedSpawns);
                dupesBoxesCheckbox.GetComponent<UnityEngine.UI.Toggle>().Set(ServerConfig.m_DuppedContainers);
                GameObject SpawnStyleList = UIHostMenu.transform.GetChild(2).gameObject;
                SpawnStyleList.GetComponent<UnityEngine.UI.Dropdown>().Set(ServerConfig.m_PlayersSpawnType);


                GameObject IsSteamHost = UIHostMenu.transform.GetChild(4).gameObject;
                GameObject PortsObject = UIHostMenu.transform.GetChild(8).gameObject;
                GameObject FireSyncObj = UIHostMenu.transform.GetChild(9).gameObject;
                GameObject CheatModeListObj = UIHostMenu.transform.GetChild(10).gameObject;
                GameObject SteamLobbyType = UIHostMenu.transform.GetChild(11).gameObject;
                FireSyncObj.GetComponent<UnityEngine.UI.Dropdown>().Set(ServerConfig.m_FireSync);
                CheatModeListObj.GetComponent<UnityEngine.UI.Dropdown>().Set(ServerConfig.m_CheatsMode);

                if (SteamConnect.CanUseSteam == false)
                {
                    IsSteamHost.GetComponent<UnityEngine.UI.Toggle>().Set(false);
                    IsSteamHost.SetActive(false);
                    SteamLobbyType.SetActive(false);
                    PortsObject.SetActive(true);
                } else {

                    IsSteamHost.GetComponent<UnityEngine.UI.Toggle>().Set(SavedSettings.m_P2P);

                    if (SavedSettings.m_P2P)
                    {
                        SteamLobbyType.SetActive(true);
                        SteamLobbyType.GetComponent<UnityEngine.UI.Dropdown>().Set(SavedSettings.m_Accessibility);
                        PortsObject.SetActive(false);
                    } else {
                        SteamLobbyType.SetActive(false);
                        PortsObject.SetActive(true);
                        PortsObject.GetComponent<UnityEngine.UI.InputField>().SetText(SavedSettings.m_Port.ToString());
                    }
                }
                //PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().Set(false);

                AddHintScript(dupesCheckbox);
                AddHintScript(dupesBoxesCheckbox);
                AddHintScript(SpawnStyleList);
                AddHintScript(FireSyncObj);
                AddHintScript(IsSteamHost);
                AddHintScript(PortsObject);
                AddHintScript(CheatModeListObj);
                AddHintScript(SteamLobbyType);
            }
        }


        public static void UpdateMyClothing()
        {
            string m_Hat = "";
            string m_Top = "";
            string m_Bottom = "";
            string m_Boots = "";
            string m_Scarf = "";
            string m_Balaclava = "";
            bool NeedUpdate = true;

            //MelonLogger.Msg("UpdateMyClothing");

            if (GameManager.m_PlayerManager == null || GameManager.m_Inventory == null)
            {
                //MelonLogger.Msg("UpdateMyClothing PlayerManager or Inventory is null");
                return;
            }
            //Selecting Hat
            string Hat1 = GetClothForSlot(ClothingRegion.Head, ClothingLayer.Base);
            string Hat2 = GetClothForSlot(ClothingRegion.Head, ClothingLayer.Mid);

            string hatCandidat1 = "";
            string hatCandidat2 = "";

            if (IsBalaclava(Hat1) == true)
            {
                m_Balaclava = Hat1;
            }
            else if (IsScarf(Hat1))
            {
                m_Scarf = Hat1;
            } else {
                hatCandidat1 = Hat1;
            }

            if (IsScarf(Hat2))
            {
                m_Scarf = Hat2;
            } else {
                hatCandidat2 = Hat2;
            }

            m_Hat = GetClothToDisplay(hatCandidat1, hatCandidat2);

            //Selecting Top
            string Shirt1 = GetClothForSlot(ClothingRegion.Chest, ClothingLayer.Base);
            string Shirt2 = GetClothForSlot(ClothingRegion.Chest, ClothingLayer.Mid);
            string Top1 = GetClothForSlot(ClothingRegion.Chest, ClothingLayer.Top);
            string Top2 = GetClothForSlot(ClothingRegion.Chest, ClothingLayer.Top2);
            m_Top = GetClothToDisplay(Shirt1, Shirt2, Top1, Top2);
            //Selecting Hands
            string Hands = GetClothForSlot(ClothingRegion.Hands, ClothingLayer.Base);
            //Selecting Bottom
            string Underpants1 = GetClothForSlot(ClothingRegion.Legs, ClothingLayer.Base);
            string Underpants2 = GetClothForSlot(ClothingRegion.Legs, ClothingLayer.Mid);
            string Pants1 = GetClothForSlot(ClothingRegion.Legs, ClothingLayer.Top);
            string Pants2 = GetClothForSlot(ClothingRegion.Legs, ClothingLayer.Top2);
            m_Bottom = GetClothToDisplay(Underpants1, Underpants2, Pants1, Pants2);
            //Selecting Boots
            string Socks1 = GetClothForSlot(ClothingRegion.Feet, ClothingLayer.Base);
            string Socks2 = GetClothForSlot(ClothingRegion.Feet, ClothingLayer.Mid);
            string Boots = GetClothForSlot(ClothingRegion.Feet, ClothingLayer.Top);

            m_Boots = GetClothToDisplay(Socks1, Socks2, Boots, "");

            //if((MyHat != m_Hat) || (MyTop != m_Top) || (MyBottom != m_Bottom) || (MyBoots != m_Boots) || (MyScarf != m_Scarf))
            //{
            //    NeedUpdate = true;
            //}

            if (NeedUpdate == true)
            {
                MyHat = m_Hat;
                MyTop = m_Top;
                MyBottom = m_Bottom;
                MyBoots = m_Boots;
                MyScarf = m_Scarf;
                MyBalaclava = m_Balaclava;
                //MelonLogger.Msg("[MyClothing] Hat " + m_Hat);
                //MelonLogger.Msg("[MyClothing] Torso " + m_Top);
                //MelonLogger.Msg("[MyClothing] Legs " + m_Bottom);
                //MelonLogger.Msg("[MyClothing] Feets " + m_Boots);

                DataStr.PlayerClothingData Cdata = new DataStr.PlayerClothingData();
                Cdata.m_Hat = MyHat;
                Cdata.m_Top = MyTop;
                Cdata.m_Bottom = MyBottom;
                Cdata.m_Boots = MyBoots;
                Cdata.m_Scarf = MyScarf;
                Cdata.m_Balaclava = MyBalaclava;

                if (sendMyPosition == true) // CLIENT
                {
                    using (Packet _packet = new Packet((int)ClientPackets.CLOTH))
                    {
                        _packet.Write(Cdata);
                        SendUDPData(_packet);
                    }
                }
                if (iAmHost == true) // HOST
                {
                    using (Packet _packet = new Packet((int)ServerPackets.CLOTH))
                    {
                        ServerSend.CLOTH(0, Cdata, true);
                    }
                }
            }
        }

        public class BoardGameSession
        {
            public string m_GameType = "";
            public string m_GUID = "";
            public int m_Turn = 0;
            public int m_CurrentPlayer = 0;
            public bool m_Started = false;
            public List<int> m_Players = new List<int>();
            public List<List<int>> m_Decks = new List<List<int>>();

            public BoardGameSession(string guid, string gt)
            {
                m_GameType = gt;
                m_GUID = guid;
            }

            public bool Equals(BoardGameSession other)
            {
                if (other == null)
                {
                    return false;
                }

                if (this.m_GUID == other.m_GUID)
                {
                    return true;
                } else {
                    return false;
                }
            }

            public List<int> ShuffleDeck(List<int> Deck)
            {
                System.Random RNG = new System.Random();
                for (int i = 0; i < Deck.Count; i++)
                {
                    var temp = Deck[i];
                    var index = RNG.Next(0, Deck.Count);
                    Deck[i] = Deck[index];
                    Deck[index] = temp;
                }
                return Deck;
            }

            public void InitDecks()
            {
                m_Decks = new List<List<int>>();
                if (m_GameType == "GoldSapper")
                {
                    m_Decks.Add(ShuffleDeck(CreateDeck(36)));
                }
            }

            public List<int> CreateDeck(int numCards)
            {
                List<int> Deck = new List<int>();
                for (int i = 1; i <= numCards; i++)
                {
                    Deck.Add(i);
                }
                return Deck;
            }

            public bool AddPlayers(int ID)
            {
                if (!m_Players.Contains(ID))
                {
                    m_Players.Add(ID);
                    return true;
                } else {
                    return false;
                }
            }
            public void RemovePlayer(int ID)
            {
                m_Players.Remove(ID);
            }
            public void StartGame()
            {
                m_Turn = 0;
                m_CurrentPlayer = 0;
                m_Started = true;
            }

            public void NextTurn()
            {
                m_Turn++;
                m_CurrentPlayer++;
                if (m_Players.Count - 1 < m_CurrentPlayer)
                {
                    m_CurrentPlayer = 0;
                }
            }
        }

        public static BoardGameSession GetBoardGameSession(string GUID)
        {
            BoardGameSession Reference = new BoardGameSession(GUID, "");
            if (BoardGamesSessions.Contains(Reference))
            {
                return null;
            }

            for (int i = 0; i < BoardGamesSessions.Count; i++)
            {
                if (BoardGamesSessions[i] != null)
                {
                    if (BoardGamesSessions[i].m_GUID == GUID)
                    {
                        return BoardGamesSessions[i];
                    }
                }
            }

            return null;
        }

        public static void StartBoardGameSessions(string GameType, string GUID)
        {
            if (GetBoardGameSession(GUID) == null)
            {
                BoardGamesSessions.Add(new BoardGameSession(GUID, GameType));
            } else {
                MelonLogger.Msg("Boardgame session with GUID " + GUID + " already exist");
            }
        }

        public static Dictionary<int, bool> FoundCairns = new Dictionary<int, bool>();
        public static Dictionary<int, Vector3> PossibleCairns = new Dictionary<int, Vector3>();

        public static void CreateCairnsSearchList()
        {
            Il2CppArrayBase<Cairn> All = Resources.FindObjectsOfTypeAll<Cairn>();
            PossibleCairns = new Dictionary<int, Vector3>();
            for (int i = 0; i < All.Count; i++)
            {
                if (All[i].gameObject)
                {
                    Vector3 pos = All[i].gameObject.transform.position;
                    int ID = All[i].m_JournalEntryNumber;
                    if (!FoundCairns.ContainsKey(ID))
                    {
                        PossibleCairns.Add(ID, pos);
                    }
                }
            }
        }
        public static void RemoveCairnFromSearch(int ID)
        {
            if (PossibleCairns.ContainsKey(ID))
            {
                PossibleCairns.Remove(ID);
                DoRadioSearch();
            }
        }
        public static float GetDistanceToCairn(Vector3 playerPos)
        {
            float LastFoundDistance = float.PositiveInfinity;
            foreach (var cairn in PossibleCairns)
            {
                float Dist = Vector3.Distance(playerPos, cairn.Value);
                if (Dist < MaxRadioSearchDistance)
                {
                    if (LastFoundDistance > Dist)
                    {
                        LastFoundDistance = Dist;
                    }
                }
            }
            return LastFoundDistance;
        }
        public static float GetRadioBeepRate(float Distance)
        {
            float BeepRate = Distance / BlinkRateConstant;
            if (BeepRate < MinimalBlink)
            {
                BeepRate = MinimalBlink;
            }
            return BeepRate;
        }



        public static void AddFoundCairn(int CairnID, bool sync = true)
        {
            if (!FoundCairns.ContainsKey(CairnID))
            {
                FoundCairns.Add(CairnID, true);
                RemoveCairnFromSearch(CairnID);
            }

            if (CurrentCustomChalleng.m_Started && CurrentChallengeRules.m_Name == "Lost in action" && CurrentCustomChalleng.m_CurrentTask == 0)
            {
                if (iAmHost)
                {
                    CurrentCustomChalleng.m_Done[0] = FoundCairns.Count;
                }

                if (sync)
                {
                    SendCustomChallengeTrigger("CairnID" + CairnID);
                }
            }
        }

        public static DataStr.CustomChallengeRules GetCustomChallengeByID(int ID)
        {
            DataStr.CustomChallengeRules c = new DataStr.CustomChallengeRules();
            if (ID == 1)
            {
                c.m_Name = "Cold Allergic";
                c.m_Description = "This island is worst nightmare for you. Getting crash on frozen hell with having Cold urticaria, how ironic.";
                c.m_Tasks.Add(new DataStr.CustomChallengeTaskData("Get to the Desolation Point without getting risk of Hypothermia", 5400, 1));
                c.m_Tasks.Add(new DataStr.CustomChallengeTaskData("Enter any building", 600, 1));
                c.m_Lineal = true;
                c.m_CompetitiveMode = false;
                c.m_ID = 1;
            }
            else if (ID == 2)
            {
                c.m_Name = "Lost in action";
                c.m_Description = "Find cairn of poor souls who had bad luck to end up here for forever.";
                c.m_Tasks.Add(new DataStr.CustomChallengeTaskData("Find cairns", 5400, 30));
                c.m_Tasks.Add(new DataStr.CustomChallengeTaskData("Meet with other survivors on Camp Office", 2200, 1));
                c.m_Lineal = true;
                c.m_CompetitiveMode = false;
                c.m_ID = 2;
            }
            return c;
        }

        public static void LoadCustomChallenge(int ID, int CurrentTask)
        {
            DataStr.CustomChallengeRules rules = GetCustomChallengeByID(ID);
            CurrentChallengeRules = rules;
            CurrentCustomChalleng.m_CurrentTask = CurrentTask;
            CurrentCustomChalleng.m_Time = rules.m_Tasks[CurrentTask].m_Timer;
            CurrentCustomChalleng.m_Started = false;
            CurrentCustomChalleng.m_Done = new List<int>();
            for (int i = 0; i < rules.m_Tasks.Count; i++)
            {
                CurrentCustomChalleng.m_Done.Add(0);
            }

            if (iAmHost)
            {
                ServerSend.CHALLENGEINIT(ID, CurrentTask);
            }
        }
        public static void StartCustomChallenge()
        {
            CurrentCustomChalleng.m_Started = true;
        }
        public static void NextCustomTask()
        {
            CurrentCustomChalleng.m_CurrentTask++;
            CurrentCustomChalleng.m_Time = CurrentChallengeRules.m_Tasks[CurrentCustomChalleng.m_CurrentTask].m_Timer;
        }
        public static void CustomChallengeUpdate()
        {
            if (CurrentChallengeRules.m_Name == "Cold Allergic")
            {
                if (CurrentCustomChalleng.m_CurrentTask == 0)
                {
                    if (RegionManager.GetCurrentRegion() == GameRegion.WhalingStationRegion)
                    {
                        NextCustomTask();
                    }
                }
                else if (CurrentCustomChalleng.m_CurrentTask == 1)
                {
                    if (!GameManager.IsOutDoorsScene(level_name) && !level_name.Contains("Cave") && !level_name.Contains("Mine"))
                    {
                        CurrentCustomChalleng.m_Started = false;
                        ProcessCustomChallengeTrigger("Win");
                        SendCustomChallengeTrigger("Win");
                    }
                }
                if (level_name != "Boot" && level_name != "Empty" && level_name != "MainMenu")
                {
                    if (GameManager.GetFreezingComponent().m_CurrentFreezing >= 100)
                    {
                        CurrentCustomChalleng.m_Started = false;
                        ProcessCustomChallengeTrigger("Lose");
                        SendCustomChallengeTrigger("Lose");
                    }
                }
            } else if (CurrentChallengeRules.m_Name == "Lost in action")
            {
                if (CurrentCustomChalleng.m_CurrentTask == 1)
                {
                    if (iAmHost == true)
                    {
                        int HowManyOnCamp = 0;
                        int HowManyOnServer = 1;
                        if (levelid == 10) // If I in Camp Office
                        {
                            HowManyOnCamp++;
                        }

                        for (int i = 1; i <= Server.MaxPlayers; i++)
                        {
                            if (Server.clients[i].IsBusy())
                            {
                                HowManyOnServer++;
                                if (Shared.ClientIsLoading(i) == false && playersData[i].m_Levelid == 10)
                                {
                                    HowManyOnCamp++;
                                }
                            }
                        }

                        if (HowManyOnCamp == HowManyOnServer)
                        {
                            CurrentCustomChalleng.m_Started = false;
                            ProcessCustomChallengeTrigger("Win");
                            SendCustomChallengeTrigger("Win");
                        }
                    }
                }
            }
        }


        public static void ProcessCustomChallengeTrigger(string TRIGGER)
        {
            MelonLogger.Msg("[ProcessCustomChallengeTrigger] " + TRIGGER);

#if (!DEDICATED)

            if (TRIGGER == "Lose")
            {
                if (m_Panel_ChallengeComplete == null)
                {
                    m_Panel_ChallengeComplete = InterfaceManager.LoadPanel<Panel_ChallengeComplete>();
                }
                m_Panel_ChallengeComplete.ShowPanel(Panel_ChallengeComplete.Options.ShowFailStatInfo);
                m_Panel_ChallengeComplete.SetStatInfoText("Ехать ты лох конечно", "Оставшиеся время:", CurrentCustomChalleng.m_Time, "Обкакался раз:", 99);
            }
            else if (TRIGGER == "Win")
            {
                if (m_Panel_ChallengeComplete == null)
                {
                    m_Panel_ChallengeComplete = InterfaceManager.LoadPanel<Panel_ChallengeComplete>();
                }
                m_Panel_ChallengeComplete.ShowPanel(Panel_ChallengeComplete.Options.Succeeded);
                m_Panel_ChallengeComplete.SetStatInfoText("Ну молодец", "Оставшиеся время:", CurrentCustomChalleng.m_Time, "Обкакался раз:", 0);
            }
#endif
            if (TRIGGER.StartsWith("CairnID"))
            {
                int ID = int.Parse(TRIGGER.Replace("CairnID", ""));
                AddFoundCairn(ID, false);
            }
        }
        public static void SendCustomChallengeTrigger(string TRIGGER)
        {
            if (iAmHost)
            {
                ServerSend.CHALLENGETRIGGER(TRIGGER);
            }
            if (sendMyPosition)
            {
                using (Packet _packet = new Packet((int)ClientPackets.CHALLENGETRIGGER))
                {
                    _packet.Write(TRIGGER);
                    SendUDPData(_packet);
                }
            }
        }

        public static DataStr.CustomChallengeData CurrentCustomChalleng = new DataStr.CustomChallengeData();
        public static DataStr.CustomChallengeRules CurrentChallengeRules = new DataStr.CustomChallengeRules();

        public static string GetFormatedTextForChallange()
        {
            TimeSpan span = TimeSpan.FromSeconds(CurrentCustomChalleng.m_Time);

            return span.ToString(@"hh\:mm\:ss");
        }
        public static string GetFormatedTimeForExpedition(int Seconds)
        {
            TimeSpan span = TimeSpan.FromSeconds(Seconds);

            return span.ToString(@"hh\:mm\:ss");
        }
        public static int CalculateHowManyTaskDone()
        {
            int done = 0;
            for (int i = 0; i < CurrentCustomChalleng.m_Done.Count; i++)
            {
                if (CurrentCustomChalleng.m_Done[i] >= CurrentChallengeRules.m_Tasks[i].m_GoalVal)
                {
                    done++;
                }
            }
            return done;
        }

        public static void ExpeditionUI(string ExpeditionName, string desc, int TimeLeft)
        {
            Panel_Log Log = InterfaceManager.m_Panel_Log;
            Panel_ActionsRadial Rad = InterfaceManager.m_Panel_ActionsRadial;
            Panel_Actions Act = InterfaceManager.m_Panel_Actions;
            string Time = GetFormatedTimeForExpedition(TimeLeft);

            desc = ExpeditionName + "\n" + desc;

            if (Rad)
            {
                Rad.m_MissionObjectiveLabel.text = desc;
                Rad.m_MissionTimerLabel.text = Time;
            }
            if (Act)
            {
                Act.m_MissionObjectiveWithTimerLabel.text = desc;
                Act.m_MissionTimerLabel.text = Time;
                Act.m_MissionTimerLabel.gameObject.SetActive(true);
            }
            if (Log)
            {
                Log.m_MissionNameHeaderLabel.text = ExpeditionName;
                Log.m_MissionNameLabel.text = ExpeditionName;

                Log.m_MissionDescriptionLabel.text = desc;
                Log.m_TimerLabel.text = Time;
            }
        }


        public static void PatchChallange()
        {
            if (CurrentChallengeRules.m_ID == 0)
            {
                return;
            }

            string Cname = CurrentChallengeRules.m_Name;
            string Cdesc;
            string Ctime = GetFormatedTextForChallange();
            int DisplayIndex;

            if (CurrentChallengeRules.m_Lineal)
            {
                Cdesc = CurrentChallengeRules.m_Tasks[CurrentCustomChalleng.m_CurrentTask].m_Task;
                DisplayIndex = CurrentCustomChalleng.m_CurrentTask + 1;

                int NeedToGoal = CurrentChallengeRules.m_Tasks[CurrentCustomChalleng.m_CurrentTask].m_GoalVal;
                int CurrentResult = CurrentCustomChalleng.m_Done[CurrentCustomChalleng.m_CurrentTask];

                if (NeedToGoal > 1)
                {
                    Cdesc = Cdesc + " " + CurrentResult + "/" + NeedToGoal;
                }
            } else {
                Cdesc = "";

                for (int i = 0; i < CurrentChallengeRules.m_Tasks.Count; i++)
                {

                    int NeedToGoal = CurrentChallengeRules.m_Tasks[i].m_GoalVal;
                    int CurrentResult = CurrentCustomChalleng.m_Done[i];
                    string TaskText = CurrentChallengeRules.m_Tasks[i].m_Task;

                    string element = "[";
                    if (i != 0)
                    {
                        element = element + "\n";
                    }
                    element = element + "[";
                    if (CurrentResult >= NeedToGoal)
                    {
                        element = element + "X] ";
                    } else {
                        element = element + " ] ";
                    }
                    element = element + TaskText;
                    Cdesc = Cdesc + element;
                }

                DisplayIndex = CalculateHowManyTaskDone();
            }

            Panel_Log Log = InterfaceManager.m_Panel_Log;
            Panel_ActionsRadial Rad = InterfaceManager.m_Panel_ActionsRadial;
            Panel_Actions Act = InterfaceManager.m_Panel_Actions;
            if (Rad)
            {
                Rad.m_MissionObjectiveLabel.text = Cdesc;
                Rad.m_MissionTimerLabel.text = Ctime;
            }
            if (Act)
            {
                Act.m_MissionObjectiveWithTimerLabel.text = Cdesc;
                Act.m_MissionTimerLabel.text = Ctime;
                Act.m_MissionTimerLabel.gameObject.SetActive(true);
            }
            if (Log)
            {
                Log.m_MissionNameHeaderLabel.text = Cname;
                Log.m_MissionNameLabel.text = Cname;

                if (CurrentChallengeRules.m_Tasks.Count > 1)
                {
                    Log.m_MissionNameHeaderLabel.text = Cname + " " + DisplayIndex + "/" + CurrentChallengeRules.m_Tasks.Count;
                }

                Log.m_MissionDescriptionLabel.text = Cdesc;
                Log.m_TimerLabel.text = Ctime;
            }
        }

        public static void AddSpray(DecalProjectorInstance Replica)
        {
            DynamicDecalsManager Manager = GameManager.GetDynamicDecalsManager();
            DecalProjectorInstance Decal = Manager.AddSprayPaintDecal(Replica.m_ProjectileType, MaterialEffectType.MaterialEffect_Untagged, Replica.m_Pos, Replica.m_Normal);

            Decal.m_Guid = Replica.m_Guid;
            Decal.m_DecalName = Replica.m_DecalName;
            Decal.m_Indoors = Replica.m_Indoors;
            Decal.m_Yaw = Replica.m_Yaw;
            Decal.m_Alpha = Replica.m_Alpha;
            Decal.m_ColorTint = Replica.m_ColorTint;
            Decal.m_RevealAmount = Replica.m_RevealAmount;
            Decal.m_RevealedOnMap = Replica.m_RevealedOnMap;

            Decal.m_IsRevealing = true;
            Decal.m_IsPlacing = false;

            Manager.PlaceDownSprayPaintDecal(Decal);
        }

        public static void ContinuePoiskMujikov()
        {
            if (CurrentSearchIndex < sceneCount)
            {
                string Load = scenes[CurrentSearchIndex];
                GameManager.LoadSceneWithLoadingScreen(Load);
                MelonLogger.Msg(ConsoleColor.Green, "[PoiskMujikov] Loading scene " + Load + " Left scenes " + CurrentSearchIndex + "/" + sceneCount);
            }
        }

        public static string[] scenes;
        public static int sceneCount = -1;
        public static int CurrentSearchIndex = 5;

        public static void PoiskMujikov()
        {
            sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            MelonLogger.Msg(ConsoleColor.Blue, "[PoiskMujikov] Scenes in game " + sceneCount);
            scenes = new string[sceneCount];
            for (int i = 0; i < sceneCount; i++)
            {
                scenes[i] = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
            }
            ContinuePoiskMujikov();
        }

        public static int GetPasswordFromGear(string name)
        {
            if (name.Contains("CanneryCodeNote"))
            {
                return 1540;
            } else if (name.Contains("BlackrockCodeNote"))
            {
                return 3008;
            }
            return 0;
        }
        public static void RestoreCodeFromGears()
        {
            MelonLogger.Msg(ConsoleColor.Blue, "[Papers codes] Checking all papers to make sure if we have code");
            foreach (GearItemObject item in GameManager.GetInventoryComponent().m_Items)
            {
                int Code = GetPasswordFromGear(item.m_GearItemName);
                if (Code != 0)
                {
                    RememoberCode(Code);
                }
            }
        }

        public static void RememoberCode(int code)
        {
            if (GameManager.GetPlayerManagerComponent().m_KnownCodes.Contains(code))
            {
                MelonLogger.Msg(ConsoleColor.Yellow, "[Papers codes] Code " + code + " is already known");
            } else {
                MelonLogger.Msg(ConsoleColor.Green, "[Papers codes] Code " + code + " added to the player's memory");
                GameManager.GetPlayerManagerComponent().m_KnownCodes.Add(code);
            }
        }

        public static void UseKey(string KeySeed, GameObject Door, bool LeadKey)
        {
            if (Door && Door.GetComponent<LoadScene>())
            {
                if (PendingKeysAction == KeysAction.LOCK)
                {
                    TryLockDoor(Door.GetComponent<LoadScene>(), KeySeed);
                }
                else if (PendingKeysAction == KeysAction.OPEN)
                {
                    if (iAmHost)
                    {
                        if (LeadKey && !MPSaveManager.TryUseLeadKey())
                        {
                            RemoveKey(KeySeed);
                            HUDMessage.AddMessage("The key broke!");
                            return;
                        }
                        bool Correct = MPSaveManager.TryUseKey(level_guid, Door.GetComponent<Comps.DoorLockedOnKey >().m_DoorKey, KeySeed);
                        if (Correct)
                        {
                            Door.GetComponent<LoadScene>().Activate();
                        } else {
                            HUDMessage.AddMessage("Incorrect key!");
                            Door.GetComponent<Lock>().PlayLockedAudio();
                        }
                    }
                    else if (sendMyPosition)
                    {
                        DoPleaseWait("Please wait...", "Trying open this door...");
                        using (Packet _packet = new Packet((int)ClientPackets.TRYOPENDOOR))
                        {
                            _packet.Write(Door.GetComponent<Comps.DoorLockedOnKey>().m_DoorKey);
                            _packet.Write(KeySeed);
                            _packet.Write(level_guid);
                            _packet.Write(LeadKey);
                            SendUDPData(_packet);
                        }
                    }
                } else if (PendingKeysAction == KeysAction.UNLOCK)
                {
                    if (iAmHost)
                    {
                        bool Correct = MPSaveManager.TryUseKey(level_guid, Door.GetComponent<Comps.DoorLockedOnKey>().m_DoorKey, KeySeed);
                        if (Correct)
                        {
                            MPSaveManager.RemoveLockedDoor(level_guid, Door.GetComponent<Comps.DoorLockedOnKey>().m_DoorKey);
                        } else {
                            HUDMessage.AddMessage("Incorrect key!");
                            Door.GetComponent<Lock>().PlayLockedAudio();
                        }
                    } else if (sendMyPosition) {
                        DoPleaseWait("Please wait...", "Trying unlock this door...");
                        using (Packet _packet = new Packet((int)ClientPackets.TRYOPENDOOR))
                        {
                            _packet.Write(Door.GetComponent<Comps.DoorLockedOnKey>().m_DoorKey);
                            _packet.Write(KeySeed);
                            _packet.Write(level_guid);
                            _packet.Write(LeadKey);
                            SendUDPData(_packet);
                        }
                    }
                }
            }
        }

        public static List<DataStr.DoorKey> GetKeysList()
        {
            List<DataStr.DoorKey> KeysList = new List<DataStr.DoorKey>();

            foreach (GearItemObject Gear in GameManager.GetInventoryComponent().m_Items)
            {
                if (Gear.m_GearItemName == "GEAR_SCDoorKey" || Gear.m_GearItemName == "GEAR_SCDoorKeyLead")
                {
                    if (Gear.m_GearItem.m_ObjectGuid && !string.IsNullOrEmpty(Gear.m_GearItem.m_ObjectGuid.m_Guid))
                    {
                        string GUID = Gear.m_GearItem.m_ObjectGuid.m_Guid;
                        if (GUID.Contains('_'))
                        {
                            string[] D = GUID.Split('_');
                            KeysList.Add(new DataStr.DoorKey(D[1], D[0], Gear.m_GearItemName == "GEAR_SCDoorKeyLead"));
                        }
                    }
                }
            }

            return KeysList;
        }
        public static void RemoveKey(string Seed)
        {
            foreach (GearItemObject Gear in GameManager.GetInventoryComponent().m_Items)
            {
                if (Gear.m_GearItemName == "GEAR_SCDoorKeyLead")
                {
                    if (Gear.m_GearItem.m_ObjectGuid && !string.IsNullOrEmpty(Gear.m_GearItem.m_ObjectGuid.m_Guid))
                    {
                        string GUID = Gear.m_GearItem.m_ObjectGuid.m_Guid;
                        if (GUID.Contains('_'))
                        {
                            string[] D = GUID.Split('_');
                            if(D[1] == Seed)
                            {
                                UnityEngine.Object.Destroy(Gear.m_GearItem.gameObject);
                                return;
                            }
                        }
                    }
                }
            }
        }
        public static int GetLockpicks()
        {
            return GameManager.GetInventoryComponent().GetNumGearWithName("GEAR_SCLockpick");
        }


        public enum KeysAction
        {
            OPEN,
            LOCK,
            UNLOCK,
        }

        public static KeysAction PendingKeysAction = KeysAction.LOCK;

        public static void SelectKeys(GameObject Door, KeysAction Act)
        {
            PendingKeysAction = Act;
            ShowKeysActionsPicker(Door, Act);
        }
        public static void LockPick(GameObject objectInteractedWith)
        {
            DataStr.PriorityActionForOtherPlayer act = GetCustomAction("Lockpick");
            DoLongAction(objectInteractedWith, act.m_ProcessText, act.m_Action);
            GameManager.GetInventoryComponent().RemoveUnits(GameManager.GetInventoryComponent().GetBestGearItemWithName("GEAR_SCLockpick"), 1);

            if (objectInteractedWith.GetComponent<Comps.DoorLockedOnKey>())
            {
                if (sendMyPosition)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.LOCKPICK))
                    {
                        _packet.Write(objectInteractedWith.GetComponent<Comps.DoorLockedOnKey>().m_DoorKey);
                        _packet.Write(level_guid);
                        SendUDPData(_packet);
                    }
                } else {
                    MPSaveManager.TryLockPick(level_guid, objectInteractedWith.GetComponent<Comps.DoorLockedOnKey>().m_DoorKey, 0);
                }
            }
        }

        public static void RequestExpedition()
        {
            PlayRadioOver();
            Shared.GameRegion Region = ConvertGameRegion(GameManager.GetUniStorm().m_CurrentRegion);
            MelonLogger.Msg("Trying to get expedition on "+ExpeditionBuilder.GetRegionString((int)Region));
            if (iAmHost)
            {
                ExpeditionManager.StartNewExpedition(MPSaveManager.GetSubNetworkGUID(), (int)Region);
            } else
            {
                using (Packet _packet = new Packet((int)ClientPackets.STARTEXPEDITION))
                {
                    _packet.Write((int)Region);
                    SendUDPData(_packet);
                }
            }
        }

        public static void ShowRadioActionPicker(GameObject objectInteractedWith)
        {
            Panel_ActionPicker Panel = InterfaceManager.m_Panel_ActionPicker;
            if (Panel)
            {
                Panel.Enable(true);
                Panel.m_ActionPickerItemDataList.Clear();
                Action act1 = new Action(() => RequestExpedition());
                Action act2 = new Action(() => ShowInvitesAfterPicker());
                
                Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_map", "Start Expedition", act1));
                Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_knowledge_people", "Expedition Invites", act2));
                
                Panel.m_ObjectInteractedWith = objectInteractedWith;
                Panel.EnableWithCurrentList();
            }
        }

        public static void SendInvietToPlayer(int ClientID)
        {
            if (iAmHost)
            {
                if (ClientID != -1)
                {
                    ExpeditionManager.CreateInviteToExpedition(Shared.GetMacAddress(), Server.GetMACByID(ClientID));
                }
            } else
            {
                using (Packet _packet = new Packet((int)ClientPackets.CREATEEXPEDITIONINVITE))
                {
                    _packet.Write(ClientID);
                    SendUDPData(_packet);
                }
            }
        }

        public static void AcceptInvite(ExpeditionManager.ExpeditionInvite Invite)
        {
            if (iAmHost)
            {
                ExpeditionManager.AcceptInvite(Invite.m_PersonToInviteMAC, Invite.m_InviterMAC);
            }
            if (sendMyPosition)
            {
                using (Packet _packet = new Packet((int)ClientPackets.ACCEPTEXPEDITIONINVITE))
                {
                    _packet.Write(Invite);
                    SendUDPData(_packet);
                }
            }
        }

        public static void ShowInvitesPicker(GameObject objectInteractedWith, List<ExpeditionManager.ExpeditionInvite> Invites)
        {
            if (Invites.Count == 0)
            {
                HUDMessage.AddMessage("You have not any invites.");
                return;
            }

            Panel_ActionPicker Panel = InterfaceManager.m_Panel_ActionPicker;
            if (Panel)
            {
                string Icon = "ico_knowledge_people";
                Panel.Enable(true);
                Panel.m_ActionPickerItemDataList.Clear();

                foreach (ExpeditionManager.ExpeditionInvite Invite in Invites)
                {
                    Action act = new Action(() => AcceptInvite(Invite));
                    Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData(Icon, "Invite by " + Invite.m_InviterName, act));
                }

                Panel.m_ObjectInteractedWith = objectInteractedWith;
                Panel.EnableWithCurrentList();
            }
        }
        public static void ShowInvitesAfterPicker()
        {
            Pathes.ShowInvitesAfterPicker = true;
        }

        public static void KnockKnock(GameObject Door)
        {
            if (Door)
            {
                LoadScene LS = Door.GetComponent<LoadScene>();
                string Scene;
                if (LS)
                {
                    Scene = Door.GetComponent<LoadScene>().m_SceneToLoad + "_" + Door.GetComponent<LoadScene>().m_GUID;
                }else{
                    return;
                }

                if (sendMyPosition)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.KNOCKKNOCK))
                    {
                        _packet.Write(Scene);
                        SendUDPData(_packet);
                    }
                }
                if (iAmHost)
                {
                    MPSaveManager.AddKnockDoorRequest(0, Scene);
                }
            }
        }

        public static void PickupBlank(GameObject objectInteractedWith)
        {
            PickupDroppedGear(objectInteractedWith, true);
        }

        public static int PendingLocksmithAction = 0;
        public static GameObject PendingLocksmithObject = null;

        public static void LocksmithWork(GameObject objectInteractedWith, int Tool)
        {
            if(Tool == 0 && !GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_Hacksaw"))
            {
                HUDMessage.AddMessage("You need hacksaw!");
                GameAudioManager.PlayGUIError();
                return;
            }
            else if (Tool == 1 && !GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_SharpeningStone"))
            {
                HUDMessage.AddMessage("You need whetstone!");
                GameAudioManager.PlayGUIError();
                return;
            }

            if (objectInteractedWith.GetComponent<Comps.DroppedGearDummy>())
            {
                int Hash = objectInteractedWith.GetComponent<Comps.DroppedGearDummy>().m_SearchKey;
                if (iAmHost)
                {
                    if (!MPSaveManager.CanWorkOnBlank(Hash))
                    {
                        HUDMessage.AddMessage("Someone already work on this!");
                        GameAudioManager.PlayGUIError();
                    }else{
                        MPSaveManager.ChangeBlankState(Hash, -1);
                    }
                }
                if (sendMyPosition)
                {
                    PendingLocksmithAction = Tool;
                    PendingLocksmithObject = objectInteractedWith;
                    DoPleaseWait("Please wait...","Requesting data about blank...");
                    using (Packet _packet = new Packet((int)ClientPackets.REQUESTLOCKSMITH))
                    {
                        _packet.Write(Hash);
                        SendUDPData(_packet);
                    }
                    return;
                }
            }

            DataStr.PriorityActionForOtherPlayer act = GetCustomAction("Locksmith"+Tool);
            DoLongAction(objectInteractedWith, act.m_ProcessText, act.m_Action);
        }

        public static void ShowBlankworkingPicker(GameObject objectInteractedWith)
        {
            Panel_ActionPicker Panel = InterfaceManager.m_Panel_ActionPicker;
            if (Panel)
            {
                Panel.Enable(true);
                Panel.m_ActionPickerItemDataList.Clear();
                Panel.m_ObjectInteractedWith = objectInteractedWith;
                PendingRegisterKey = null;
                Action act = new Action(() => PickupBlank(objectInteractedWith));
                Action Saw = new Action(() => LocksmithWork(objectInteractedWith, 0));
                Action Grind = new Action(() => LocksmithWork(objectInteractedWith, 1));

                if (objectInteractedWith.name.ToLower().StartsWith("gear_scdoorkeytemp") || objectInteractedWith.name.ToLower().StartsWith("gear_scdoorkeyleadtemp"))
                {
                    Grind = new Action(() => BeginRegisterKey(objectInteractedWith));
                }


                //Action Flattening = new Action(() => LocksmithWork(objectInteractedWith, 2));

                Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_BearSpear", "Sawing", Saw));
                Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_injury_majorBruising", "Grinding", Grind));
                //Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_crafting", "Flattening", Flattening));
                Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_climb", "Pickup", act));

                Panel.EnableWithCurrentList();
            }
        }

        public static void LetThemEnter(int ClientID, string Scene)
        {
            if (iAmHost)
            {
                MPSaveManager.ApplyEnterFromKnock(ClientID, Scene);
            }
            if (sendMyPosition)
            {
                using (Packet _packet = new Packet((int)ClientPackets.LETENTER))
                {
                    _packet.Write(ClientID);
                    _packet.Write(Scene);
                    SendUDPData(_packet);
                }
            }
        }

        public static GameObject TempDoor = null;

        public static void CharCoalDraw(CharcoalItem Coal)
        {
            if (Coal.m_IsActive || !GameManager.GetPlayerAnimationComponent().CanTransitionToState(PlayerAnimation.State.Stowing))
            {
                return;
            }
            if (GameManager.GetWeatherComponent().IsIndoorScene())
            {
                GameAudioManager.PlayGUIError();
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_CannotMapLocalAreaIndoors"));
            } else if (!CharcoalItem.HasSurveyVisibility(Coal.GetSurveyExtendedTimeSkillBonus()))
            {
                GameAudioManager.PlayGUIError();
                HUDMessage.AddMessage(Localization.Get("GAMEPLAY_DetailSurveyNoVisibility"));
            } else
            {
                Coal.m_IsActive = true;
                CharcoalItem.m_CharcoalItemInUseForSurvey = Coal;
                Coal.m_TimeSpentSurveying = 0.0f;
                Coal.AccelerateTimeOfDay();
                Coal.m_SurveyAudioID = GameAudioManager.PlaySound(Coal.m_SurveyLoopAudio, InterfaceManager.GetSoundEmitter());
            }
        }
        public static void CreateCustomNote(string Message, GearItem Note = null)
        {
            string Compressed = Shared.CompressString(Message);

            if (Note == null)
            {
                Note = GameManager.GetPlayerManagerComponent().AddItemCONSOLE("GEAR_SCNote", 1);
            }

            if (Note.m_ObjectGuid == null)
            {
                Note.m_ObjectGuid = Note.gameObject.AddComponent<ObjectGuid>();
            }
            Note.m_ObjectGuid.m_Guid = Compressed;
        }


        public static void WriteNote(CharcoalItem Coal)
        {
            CharcoalItem.m_CharcoalItemInUseForSurvey = Coal;
            GameManager.GetPlayerAnimationComponent().Trigger_Generic_Stow();
            Coal.m_SurveyAudioID = GameAudioManager.PlaySound(Coal.m_SurveyLoopAudio, InterfaceManager.GetSoundEmitter());
            //InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "NOTE MESSAGE", "", Panel_Confirmation.ButtonLayout.Button_2, "GAMEPLAY_Done", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
            InterfaceManager.m_Panel_Log.EnableStatsView();
            InterfaceManager.m_Panel_Log.OnEnterGeneralNotes();
            InterfaceManager.m_Panel_Log.m_SectionNav.SetActive(false);
            InterfaceManager.m_Panel_Log.m_TabsNotesObject.SetActive(false);
            InterfaceManager.m_Panel_Log.m_NotesTextField.SetText("");
        }

        public static void ShowCharCoalPicker(GameObject objectInteractedWith, CharcoalItem Coal)
        {
            Panel_ActionPicker Panel = InterfaceManager.m_Panel_ActionPicker;
            if (Panel)
            {
                Panel.Enable(true);
                Panel.m_ActionPickerItemDataList.Clear();

                Action Map = new Action(() => CharCoalDraw(Coal));
                Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_map", "Survey local area", Map));
                Action Note = new Action(() => WriteNote(Coal));
                Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_log_Notes", "Write a note", Note));

                Panel.m_ObjectInteractedWith = objectInteractedWith;
                Panel.EnableWithCurrentList();
            }
        }

        public static void ShowKnockersPicker(GameObject objectInteractedWith, List<int> Knockers)
        {
            if (Knockers.Count == 0)
            {
                HUDMessage.AddMessage("No body knocking.");
                return;
            }
            
            Panel_ActionPicker Panel = InterfaceManager.m_Panel_ActionPicker;
            if (Panel)
            {
                string Icon = "ico_unlocked";
                Panel.Enable(true);
                Panel.m_ActionPickerItemDataList.Clear();

                foreach (int Idx in Knockers)
                {
                    Action act = new Action(() => LetThemEnter(Idx, level_guid));
                    Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData(Icon, "Invite " + playersData[Idx].m_Name, act));
                }

                Panel.m_ObjectInteractedWith = objectInteractedWith;
                Panel.EnableWithCurrentList();
            }
        }

        public static void ShowKeysActionsPicker(GameObject objectInteractedWith, KeysAction ActType)
        {
            Panel_ActionPicker Panel = InterfaceManager.m_Panel_ActionPicker;
            if (Panel)
            {
                List<DataStr.DoorKey> KeysList = GetKeysList();
                int LockPicks = GetLockpicks();
                string Icon = "ico_locked";
                bool CanLockPick = false;

                if (ActType != KeysAction.LOCK)
                {
                    Icon = "ico_unlocked";
                    CanLockPick = true;
                }

                if (!CanLockPick)
                {
                    if(KeysList.Count == 0)
                    {
                        HUDMessage.AddMessage("You don't have any keys to lock this door!");
                        return;
                    }
                }
                Panel.Enable(true);
                Panel.m_ActionPickerItemDataList.Clear();


                foreach (DataStr.DoorKey Key in KeysList)
                {
                    Action act = new Action(() => UseKey(Key.m_Seed, objectInteractedWith, Key.m_Lead));
                    Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData(Icon, Key.m_Name, act));
                }

                if (CanLockPick)
                {
                    if (LockPicks > 0)
                    {
                        Action act = new Action(() => LockPick(objectInteractedWith));
                        Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_Radial_tools", "Lockpick ("+ LockPicks+")", act));
                    }
                    Action knock = new Action(() => KnockKnock(objectInteractedWith));
                    Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_fist", "Knock", knock));
                }

                Panel.m_ObjectInteractedWith = objectInteractedWith;
                Panel.EnableWithCurrentList();
            }
        }
        public static int GetCurrentRegionIfPossible()
        {
            int Region = -1;

            if (GameManager.m_Weather != null && GameManager.m_WeatherTransition != null && GameManager.m_WeatherTransition.m_CurrentWeatherSet != null && GameManager.GetUniStorm() != null)
            {
                Region = (int)GameManager.GetUniStorm().m_CurrentRegion;
            }

            return Region;
        }
        public static ConsoleColor ConvertLoggerColor(Shared.LoggerColor Color)
        {
            switch (Color)
            {
                case Shared.LoggerColor.Red:
                    return ConsoleColor.Red;
                case Shared.LoggerColor.Green:
                    return ConsoleColor.Green;
                case Shared.LoggerColor.Blue:
                    return ConsoleColor.Green;
                case Shared.LoggerColor.Yellow:
                    return ConsoleColor.Yellow;
                case Shared.LoggerColor.Magenta:
                    return ConsoleColor.Magenta;
                case Shared.LoggerColor.White:
                    return ConsoleColor.White;
                default:
                    return ConsoleColor.White;
            }
        }
        public static void DoExpeditionState(int State)
        {
            if (State == 0 || State == 1 || State == -1 || State == 4 || State == -2)
            {
                OnExpedition = false;
                if (InterfaceManager.m_Panel_Actions != null)
                {
                    InterfaceManager.m_Panel_Actions.m_MissionObjectWithTimer.SetActive(false);
                    InterfaceManager.m_Panel_Actions.m_MissionObjectiveWithTimerLabel.gameObject.SetActive(false);
                    InterfaceManager.m_Panel_Actions.m_MissionTimerLabel.gameObject.SetActive(false);
                }
            }

            if(m_InterfaceManager && InterfaceManager.m_Panel_HUD)
            {
                if (State == 0)
                {
                    InterfaceManager.m_Panel_HUD.ShowBuffLossNotification("Expedition Cancled", "Time Over", "ico_map");
                } else if (State == 1)
                {
                    InterfaceManager.m_Panel_HUD.ShowBuffNotification("Expedition Finished", "Loot Your Reward", "ico_map");
                } else if (State == 2)
                {
                    InterfaceManager.m_Panel_HUD.ShowBuffNotification("Expedition Started", "Objective Updated", "ico_map");
                } else if (State == 3)
                {
                    InterfaceManager.m_Panel_HUD.ShowBuffLossNotification("Task Completed", "Objective Updated", "ico_map");
                } else if (State == -1)
                {
                    InterfaceManager.m_Panel_HUD.ShowBuffLossNotification("Crash Site Found", "You Late", "icoMap_willAirplane");
                } else if (State == 4)
                {
                    InterfaceManager.m_Panel_HUD.ShowBuffNotification("Crash Site Found", "Loot Your Reward", "icoMap_willAirplane");
                } else if (State == 5)
                {
                    InterfaceManager.m_Panel_HUD.ShowBuffNotification("Find Crash Site", "Objective Updated", "icoMap_willAirplane");
                }else if(State == -2)
                {
                    InterfaceManager.m_Panel_HUD.ShowBuffLossNotification("Time Over", "You Late", "icoMap_willAirplane");
                }
            }
        }

        public static void NewPlayerInExpedition(string PlayerName)
        {
            GearMessage.AddMessage("ico_knowledge_people", "Joined Expedition", PlayerName, true);
            PlayRadioOver();
        }
        public static void NewExpeditionInvite(string PlayerName)
        {
            GearMessage.AddMessage("ico_map", "Expedition Invite", PlayerName, true);
            DoRadioBeep();
        }

        public const int PhotoQuality = 60;
        public static DataStr.Vector2Int GetGearResolution(string GearName)
        {
            if(GearName == "GEAR_SCPhoto")
            {
                return new DataStr.Vector2Int(320, 200);
            } else if(GearName == "GEAR_SCMapPiece")
            {
                return new DataStr.Vector2Int(200, 200);
            }
            return new DataStr.Vector2Int(320, 200);
        }

        public static void LogScreenshotData()
        {
            CameraGlobalRT cameraGlobalRt = GameManager.GetCameraGlobalRT();
            if (cameraGlobalRt && cameraGlobalRt.GetRenderTexture())
            {
                DataStr.Vector2Int Resolution = GetGearResolution("GEAR_SCPhoto");
                RenderTexture smallTarget = RenderTexture.GetTemporary(Resolution.X, Resolution.Y, 0, (RenderTextureFormat)0);
                Texture2D t = new Texture2D(((Texture)smallTarget).width, ((Texture)smallTarget).height, (TextureFormat)4, false);
                Graphics.Blit(cameraGlobalRt.GetRenderTexture(), smallTarget);
                RenderTexture.active = smallTarget;
                t.ReadPixels(new Rect(0.0f, 0.0f, (float)((Texture)smallTarget).width, (float)((Texture)smallTarget).height), 0, 0, false);
                t.Apply();
                string Base64 = Convert.ToBase64String(ImageConversion.EncodeToJPG(t, PhotoQuality));
                string GUID = MPSaveManager.AddPhoto(Base64, true);
                MelonLogger.Msg("Made photo "+ GUID);

                RenderTexture.ReleaseTemporary(smallTarget);
                UnityEngine.Object.Destroy(t);


                if (iAmHost)
                {
                    MPSaveManager.AddPhoto(Base64);
                } else
                {
                    if (sendMyPosition == true)
                    {

                        byte[] bytesToSlice = Encoding.UTF8.GetBytes(Base64);
                        int HashForSend = GUID.GetHashCode();

                        if (bytesToSlice.Length > 500)
                        {
                            List<byte> BytesBuffer = new List<byte>();
                            BytesBuffer.AddRange(bytesToSlice);

                            while (BytesBuffer.Count >= 500)
                            {
                                byte[] sliceOfBytes = BytesBuffer.GetRange(0, 499).ToArray();
                                BytesBuffer.RemoveRange(0, 499);

                                string jsonStringSlice = Encoding.UTF8.GetString(sliceOfBytes);
                                DataStr.SlicedJsonData SlicedPacket = new DataStr.SlicedJsonData();
                                SlicedPacket.m_GearName = GUID;
                                SlicedPacket.m_SendTo = -1;
                                SlicedPacket.m_Hash = HashForSend;
                                SlicedPacket.m_Str = jsonStringSlice;

                                if (BytesBuffer.Count != 0)
                                {
                                    SlicedPacket.m_Last = false;
                                } else
                                {
                                    SlicedPacket.m_Last = true;
                                }
                                AddPhotoCarefulSlice(SlicedPacket);
                            }

                            if (BytesBuffer.Count < 500 && BytesBuffer.Count != 0)
                            {
                                byte[] LastSlice = BytesBuffer.GetRange(0, BytesBuffer.Count).ToArray();
                                BytesBuffer.RemoveRange(0, BytesBuffer.Count);

                                string jsonStringSlice = Encoding.UTF8.GetString(LastSlice);
                                DataStr.SlicedJsonData SlicedPacket = new DataStr.SlicedJsonData();
                                SlicedPacket.m_GearName = GUID;
                                SlicedPacket.m_SendTo = -1;
                                SlicedPacket.m_Hash = HashForSend;
                                SlicedPacket.m_Str = jsonStringSlice;
                                SlicedPacket.m_Last = true;

                                AddPhotoCarefulSlice(SlicedPacket);
                                SendNextPhotoCarefulSlice();
                            }
                        } else
                        {
                            DataStr.SlicedJsonData PhotoPacket = new DataStr.SlicedJsonData();
                            PhotoPacket.m_GearName = GUID;
                            PhotoPacket.m_SendTo = -1;
                            PhotoPacket.m_Hash = HashForSend;
                            PhotoPacket.m_Str = Base64;
                            PhotoPacket.m_Last = true;
                            using (Packet _packet = new Packet((int)ClientPackets.GOTPHOTOSLICE))
                            {
                                _packet.Write(PhotoPacket);
                                SendUDPData(_packet);
                            }
                        }
                    }
                }
                GearItem Photo = GameManager.GetPlayerManagerComponent().AddItemCONSOLE("GEAR_SCPhoto", 1);

                if (Photo)
                {
                    if (Photo.m_ObjectGuid == null)
                    {
                        Photo.m_ObjectGuid = Photo.gameObject.AddComponent<ObjectGuid>();
                    }
                    Photo.m_ObjectGuid.m_Guid = GUID;

                    Texture2D tex = new Texture2D(Resolution.X, Resolution.Y);
                    ImageConversion.LoadImage(tex, Convert.FromBase64String(Base64));
                    Photo.gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = tex;
                }
                //GameManager.GetPlayerManagerComponent().EquipItem(Photo, false);
            }
        }

        public static void ShowRockStashActionPicker(GameObject objectInteractedWith)
        {
            Panel_ActionPicker Panel = InterfaceManager.m_Panel_ActionPicker;
            if (Panel)
            {
                Comps.FakeRockCache Stash = null;

                if (objectInteractedWith != null && objectInteractedWith.GetComponent<Comps.FakeRockCache>() != null)
                {
                    Stash = objectInteractedWith.GetComponent<Comps.FakeRockCache>();
                }

                if(Stash == null)
                {
                    return;
                }
                
                Panel.Enable(true);
                Panel.m_ActionPickerItemDataList.Clear();
                Action act1 = new Action(() => Stash.Open());
                Action act2 = new Action(() => Stash.Remove());

                Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_rockCache", "GAMEPLAY_Open", act1));
                Panel.m_ActionPickerItemDataList.Add(new Panel_ActionPicker.ActionPickerItemData("ico_dismantle", "GAMEPLAY_Dismantle", act2));
                //Play_RockCache

                Panel.m_ObjectInteractedWith = objectInteractedWith;
                Panel.EnableWithCurrentList();
            }
        }

        public static Comps.FakeRockCache PendingRockCahceRemove = null;
        public static void ContinueRemovingRockCache()
        {
            if(PendingRockCahceRemove != null)
            {
                GameManager.s_IsAISuspended = true;
                Pathes.FakeRockCacheCallback = PendingRockCahceRemove;
                InterfaceManager.m_Panel_GenericProgressBar.Launch(Localization.Get("GAMEPLAY_BreakingDownProgress"), 2f, 10, 0.0f, "Play_RockCache", null, false, false, null);
                PendingRockCahceRemove = null;
            }
        }

        public static void SpawnRockCache(DataStr.FakeRockCacheVisualData Data)
        {
            if(ObjectGuidManager.Lookup(Data.m_GUID) != null)
            {
                MelonLogger.Msg(ConsoleColor.Yellow, "RockCache with GUID "+Data.m_GUID+" already exist!");
                return;
            }
            
            
            GameObject reference = GameManager.GetRockCacheManager().m_RockCachePrefab.gameObject;
            GameObject obj = UnityEngine.Object.Instantiate<GameObject>(reference, Data.m_Position, Data.m_Rotation);
            FakeRockCache Cach = obj.GetComponent<FakeRockCache>();
            if (Cach == null)
            {
                Cach = obj.AddComponent<FakeRockCache>();
            }
            Cach.m_GUID = Data.m_GUID;
            Cach.m_Owner = Data.m_Owner;
            Cach.m_Rocks = reference.GetComponent<RockCache>().m_NumRocksFromDismantle;
            Cach.m_Sticks = reference.GetComponent<RockCache>().m_NumSticksFromDismantle;

            Cach.m_VisualData.m_GUID = Cach.m_GUID;
            Cach.m_VisualData.m_LevelGUID = level_guid;
            Cach.m_VisualData.m_Owner = Cach.m_Owner;
            Cach.m_VisualData.m_Position = obj.transform.position;
            Cach.m_VisualData.m_Rotation = obj.transform.rotation;


            ObjectGuid GuidObj = obj.GetComponent<ObjectGuid>();
            if (GuidObj == null)
            {
                GuidObj = obj.AddComponent<ObjectGuid>();
            }
            GuidObj.Set(Cach.m_GUID);


            UnityEngine.Object.Destroy(obj.GetComponent<RockCache>());
        }

        public static void AddRockCache(DataStr.FakeRockCacheVisualData Data)
        {
            if(Data.m_LevelGUID == level_guid && GameManager.GetRockCacheManager() != null)
            {
                SpawnRockCache(Data);
            }
        }
        public static void RemoveRockCache(DataStr.FakeRockCacheVisualData Data)
        {
            if (Data.m_LevelGUID == level_guid)
            {
                GameObject obj = ObjectGuidManager.Lookup(Data.m_GUID);

                if (obj != null)
                {
                    UnityEngine.Object.Destroy(obj);
                }
            }
        }

        public static Shared.GameRegion GetCurrentRegion()
        {
            if (GameManager.m_Weather != null && GameManager.m_WeatherTransition != null && GameManager.m_WeatherTransition.m_CurrentWeatherSet != null && GameManager.GetUniStorm() != null)
            {
                return ConvertGameRegion(GameManager.GetUniStorm().m_CurrentRegion);
            } else
            {
                return ConvertGameRegion(GameRegion.RandomRegion);
            }
        }

        public static Shared.GameRegion ConvertGameRegion(GameRegion Reg)
        {
            if(Reg == GameRegion.RandomRegion)
            {
                if(m_InterfaceManager && InterfaceManager.m_Panel_Map && InterfaceManager.m_Panel_Map.m_MapObjects.Count > 0)
                {
                    Panel_Map Panel = InterfaceManager.m_Panel_Map;
                    string RegionName = "";
                    string SceneMapName = Panel.GetMapNameOfCurrentScene();

                    foreach (RegionMap item in Panel.m_MapObjects)
                    {
                        if(item.m_RegionName == SceneMapName)
                        {
                            RegionName = item.m_RegionName;
                        }
                    }
                    if (string.IsNullOrEmpty(RegionName))
                    { 
                        int RegionIndex = Panel.GetIndexOfCurrentScene();
                        if (Panel.m_UnlockedRegionNames.Count-1 >= RegionIndex)
                        {
                            SceneMapName = Panel.m_UnlockedRegionNames[RegionIndex];
                            foreach (RegionMap item in Panel.m_MapObjects)
                            {
                                if (item.m_RegionName == SceneMapName)
                                {
                                    RegionName = item.m_RegionName;
                                }
                            }
                        }
                    }  

                    if (string.IsNullOrEmpty(RegionName))
                    {
                        if(GameManager.m_Weather != null && GameManager.m_WeatherTransition != null && GameManager.m_WeatherTransition.m_CurrentWeatherSet != null && GameManager.GetUniStorm() != null)
                        {
                            return (Shared.GameRegion)GameManager.GetUniStorm().m_CurrentRegion;
                        }
                    } else
                    {
                        if(RegionName == "BlackrockTransitionZone")
                        {
                            return Shared.GameRegion.KeepersPassNorth;
                        } else if(RegionName == "CanyonRoadTransitionZone")
                        {
                            return Shared.GameRegion.KeepersPassSouth;
                        } else if (RegionName == "BlackrockRegion")
                        {
                            return Shared.GameRegion.Blackrock;
                        } else if (RegionName == "RavineTransitionZone")
                        {
                            return Shared.GameRegion.Ravine;
                        } else if (RegionName == "DamRiverTransitionZoneB")
                        {
                            return Shared.GameRegion.WindingRiver;
                        } else if (RegionName == "DamTransitionZone")
                        {
                            return Shared.GameRegion.MysteryLake;
                        } else if (RegionName == "HighwayTransitionZone")
                        {
                            return Shared.GameRegion.CrumblingHighWay;
                        } else
                        {
                            return Shared.GameRegion.RandomRegion;
                        }
                    }
                }
            } else
            {
                return (Shared.GameRegion)Reg;
            }


            return Shared.GameRegion.RandomRegion;
        }

        public static GameRegion ConvertGameRegion(Shared.GameRegion Reg)
        {
            if((int)Reg < 0)
            {
                return GameRegion.RandomRegion;
            } else
            {
                return (GameRegion)Reg;
            }
        }

        public static void MaySpawnRefMan()
        {
            System.Random RNG = new System.Random();
            if (GameManager.m_TimeOfDay != null && GameManager.m_TimeOfDay.IsNight() && RNG.NextDouble() < 0.001f)
            {
                SpawnRefMan();
            }
        }

        public static void CheckSeeingRefMan()
        {
            if(RefMan != null)
            {
                if(RefMan.GetComponent<MeshRenderer>().isVisible && !SeenRefMan)
                {
                    SeenRefMan = true;
                    PlayerDamageEvent.SpawnAfflictionEvent("GAMEPLAY_AfflictionFearAfraid", "GAMEPLAY_Affliction", "ico_injury_eventEntity2", InterfaceManager.m_FirstAidRedColor);
                    GameObject emitterFromGameObject = GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject());
                    AkSoundEngine.StopPlayingID(AkSoundEngine.PostEvent("Play_FearAffliction", emitterFromGameObject, 4, null, null), 40000);
                }
            } else
            {
                SeenRefMan = false;
            }
        }

        public static void SpawnRefMan()
        {
            GameObject reference = Resources.Load<GameObject>("ref_man_1_85_Prefab");

            if (reference)
            {
                Vector3 v3 = GameManager.GetPlayerTransform().transform.position;
                v3 = v3 - GameManager.GetPlayerTransform().transform.forward * 2;

                RefMan = UnityEngine.Object.Instantiate<GameObject>(reference, v3, GameManager.GetPlayerTransform().transform.rotation);
                UnityEngine.Object.Destroy(RefMan, 5);
            }
        }

        public static void CustomPrefabWorkAround(GameObject Obj, string Prefab)
        {
            if(Prefab == "Smoke")
            {
                UnityEngine.Object.Destroy(Obj.GetComponent<Fire>());
                UnityEngine.Object.Destroy(Obj.GetComponent<HeatSource>());
                UnityEngine.Object.Destroy(Obj.GetComponent<Campfire>());
                UnityEngine.Object.Destroy(Obj.GetComponent<EffectsControllerFire>());
                GameObject ParticleObj = Obj.transform.GetChild(7).GetChild(1).gameObject;
                ParticleSystem PS = ParticleObj.GetComponent<ParticleSystem>();
                ParticleSystemParasite PSP = ParticleObj.AddComponent<ParticleSystemParasite>();
                PSP.m_ParticleSystem = PS;
                PS.main.startLifetime = 17f;
                PS.main.startSize = 2.1f;
                Obj.transform.GetChild(0).gameObject.SetActive(false);
                Obj.transform.GetChild(7).gameObject.SetActive(true);
                Obj.transform.GetChild(26).gameObject.SetActive(false);
                Obj.transform.GetChild(27).gameObject.SetActive(false);
            }else if(Prefab == "CORPSE_Convict_001" || Prefab == "CORPSE_Convict_002" || Prefab == "CORPSE_Convict_003")
            {
                if (Obj.GetComponent<Container>() == null)
                {
                    Obj.AddComponent<Container>();
                }
            }else if(Prefab == "INTERACTIVE_CampFireBurntOut")
            {
                UnityEngine.Object.Destroy(Obj.GetComponent<Campfire>());
            }else if(Prefab == "CONTAINER_MetalLockerA")
            {
                Container Con = Obj.GetComponentInChildren<Container>();
                if(Con != null)
                {
                    Con.m_CanNeverBeOpened = true;
                }
            }
        }

        public static GameObject SpawnUniversalSyncableObject(DataStr.UniversalSyncableObject Data)
        {
            GameObject OldObj = ObjectGuidManager.Lookup(Data.m_GUID);
            string Prefab = Data.m_Prefab;
            bool Custom = false;
            if (Data.m_Prefab == "Smoke")
            {
                Prefab = "INTERACTIVE_CampFire";
                Custom = true;
            }
            if (Prefab == "CORPSE_Convict_001" || Prefab == "CORPSE_Convict_002" || Prefab == "CORPSE_Convict_003")
            {
                Custom = true;
            }
            if(Prefab == "INTERACTIVE_CampFireBurntOut")
            {
                Custom = true;
            }
            if (Prefab == "CONTAINER_MetalLockerA")
            {
                Custom = true;
            }

            GameObject reference = LoadedBundle.LoadAsset<GameObject>(Prefab);
            if(reference == null)
            {
                reference = Resources.Load<GameObject>(Prefab);
            }

            if (reference)
            {
                GameObject Obj;
                if(OldObj != null)
                {
                    Obj = OldObj;
                } else
                {
                    Obj = UnityEngine.Object.Instantiate<GameObject>(reference, Data.m_Position, Data.m_Rotation);
                }
                Obj.name = Data.m_Prefab;
                ObjectGuid ObjGUID = Obj.GetComponent<ObjectGuid>();
                if (ObjGUID == null)
                {
                    ObjGUID = Obj.AddComponent<ObjectGuid>();
                }
                ObjGUID.Set(Data.m_GUID);

                if (Custom)
                {
                    CustomPrefabWorkAround(Obj, Data.m_Prefab);
                }

                if(Obj.GetComponent<Lock>() != null)
                {
                    Obj.GetComponent<Lock>().m_ChanceLocked = 0;
                    Obj.GetComponent<Lock>().m_LockStateRolled = false;
                    Obj.GetComponent<Lock>().RollLockedState();
                }

                if (Obj.GetComponent<Container>() != null)
                {
                    Obj.GetComponent<Container>().m_RolledSpawnChance = false;
                    Obj.GetComponent<Container>().m_SpawnChance = 100;
                    Obj.GetComponent<Container>().Start();
                    if (sendMyPosition) 
                    {
                        MelonLogger.Msg("REQUESTCONTAINERSTATE GUID "+ Data.m_GUID);
                        using (Packet _packet = new Packet((int)ClientPackets.REQUESTCONTAINERSTATE))
                        {
                            _packet.Write(Data.m_Scene);
                            _packet.Write(Data.m_GUID);
                            SendUDPData(_packet);
                        }
                    }
                    if (iAmHost)
                    {
                        int State = MPSaveManager.GetContainerState(Data.m_Scene, Data.m_GUID);
                        RemoveLootFromContainer(Obj, State);
                    }
                }

                Obj.SetActive(true);

                return Obj;
            }
            return null;
        }
        public static void RemoveObjectByGUID(string GUID)
        {
            GameObject Obj = ObjectGuidManager.Lookup(GUID);

            if (Obj)
            {
                UnityEngine.Object.Destroy(Obj);
            }
        }

        public static void SetInteractiveZone(ExpeditionInteractiveData Data)
        {
            RemoveObjectByGUID(Data.m_GUID);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ExpeditionInteractive Comp = cube.AddComponent<ExpeditionInteractive>();
            Comp.Load(Data);
            cube.AddComponent<ObjectGuid>().Set(Data.m_GUID);
        }
    }
}
