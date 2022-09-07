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
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.IO.Compression;
using MelonLoader;
using Harmony;
using UnhollowerRuntimeLib;
using UnhollowerBaseLib;
using GameServer;
using MelonLoader.TinyJSON;
using UnityEngine.Networking;
using KeyboardUtilities;
using System.Runtime.InteropServices;
using IL2CPP = Il2CppSystem.Collections.Generic;

namespace SkyCoop
{
    public class MyMod : MelonMod
    {

        public static class BuildInfo
        {
            public const string Name = "Sky Co-op";
            public const string Description = "Multiplayer mod";
            public const string Author = "Filigrani";
            public const string Company = null;
            public const string Version = "0.9.9";
            public const string DownloadLink = null;
            public const int RandomGenVersion = 3;
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

        //VARS
        #region VARS
        public static bool isRuning = false;
        public static bool sendMyPosition = false;
        public static bool iAmHost = false;
        public static bool IamShatalker = false;
        public static GameObject playerbody = null;
        public static List<GameObject> players = new List<GameObject>();
        public static List<MultiPlayerClientData> playersData = new List<MultiPlayerClientData>();
        public static GameObject MyPlayerDoll = null;
        public static GameObject DollCameraDummy = null;
        public static bool IsDead = false;
        public static InvisibleEntityManager ShatalkerObject = null;
        public static HUDNowhereToHide DarkWalkerHUD = null;
        public static GameObject WardWidget = null;
        public static GameObject LureWidget = null;
        public static GameObject DistanceWidget = null;
        public static UILabel DistanceLable = null;
        public static bool WardIsActive = false;
        public static bool LureIsActive = false;
        public static bool PreviousDarkWalkerReady = false;
        public static bool DarkWalkerIsReady = false;
        public static bool ShatalkerModeClient = false;
        public static int levelid = 0;
        public static string level_name = "";
        public static string level_guid = "";
        public static string previous_level_guid = "";
        public static Vector3 PreviousBlock = new Vector3(0, 0, 0);
        public static Vector3 LastRecivedShatalkerVector = new Vector3(0, 0, 0);
        public static Vector3 LastRecivedOtherPlayerVector = new Vector3(0, 0, 0);
        public static Quaternion LastRecivedOtherPlayerQuatration = new Quaternion(0, 0, 0, 0);
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
        public static MultiplayerEmote MyEmote = null;
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
        public static List<string> KnownAnimals = new List<string>();
        public static int MyArrows = 0;
        public static int MyFlares = 0;
        public static int MyBlueFlares = 0;
        public static int StepState = 0;
        public static GameObject LastObjectUnderCrosshair = null;
        public static Vector3 V3BeforeSleep = new Vector3(0, 0, 0);
        public static bool NeedV3BeforeSleep = false;
        public static GameObject MenuStuffSpawned = null;
        public static bool AnimalsController = true;
        public static int MyTicksOnScene = 0;
        public static float MaxAniamlsSyncDistance = 245f;
        public static int MaxAnimalsSyncCount = 11;
        public static int MaxAnimalsSyncCountOnConnect = 2;
        public static int MaxAnimalsSyncNeed = 0;
        public static int DeltaAnimalsMultiplayer = 4;
        public static string DebugAnimalGUID = "";
        public static string DebugAnimalGUIDLast = "";
        public static GameObject DebugLastAnimal = null;
        public static bool InDarkWalkerMode = false;
        public static bool RealTimeCycleSpeed = true;
        public static string HarvestingAnimal = "";
        public static string PreviousHarvestingAnimal = "";
        public static List<WalkTracker> SurvivorWalks = new List<WalkTracker>();
        public static WalkTracker LastLure = new WalkTracker();
        public static bool ALWAYS_FUCKING_CURSOR_ON = false;
        public static bool AnimalSyncOption = true;
        public static GameObject PistolBulletPrefab = null;
        public static GameObject RevolverBulletPrefab = null;
        public static ShootSync PendingShoot = null;
        public static float OverridenStartCountDown = 900f;
        public static bool NeedTryReconnect = false;
        public static bool TryingReconnect = false;
        public static int NoHostResponceSeconds = 0;
        public static int AttempsToReconnect = 0;
        public static bool SendRQEvent = false;
        public static bool NeedDoBearDamage = false;
        public static bool NeedDoMooseDamage = false;
        public static GearItem SaveThrowingItem = null;
        public static string LastStruggleAnimalName = "";
        public static int OverridedHourse = 12;
        public static int OverridedMinutes = 0;
        public static string OveridedTime = "12:0";
        public static int PlayedHoursInOnline = 0;
        public static bool ConnectedSteamWorks = false;
        public static string SteamServerWorks = "";
        public static string SteamLobbyToJoin = "";
        public static AssetBundle LoadedBundle = null;
        public static int MaxPlayers = 1;
        public static GameObject GiveItemLableObj = null;
        public static UILabel GiveItemLable = null;
        public static int GiveItemTo = -1;
        public static string MyHat = "";
        public static string MyTop = "";
        public static string MyBottom = "";
        public static string MyBoots = "";
        public static string MyScarf = "";
        public static string MyBalaclava = "";
        public static List<BrokenFurnitureSync> BrokenFurniture = new List<BrokenFurnitureSync>();
        public static List<PickedGearSync> PickedGears = new List<PickedGearSync>();
        public static List<PickedGearSync> RecentlyPickedGears = new List<PickedGearSync>();
        public static List<ClimbingRopeSync> DeployedRopes = new List<ClimbingRopeSync>();
        public static List<ContainerOpenSync> LootedContainers = new List<ContainerOpenSync>();
        public static List<DeathContainerData> DeathCreates = new List<DeathContainerData>();
        public static Dictionary<string, Dictionary<string, string>> LootedContainersNew = new Dictionary<string, Dictionary<string, string>>();
        public static List<string> HarvestedPlants = new List<string>();
        public static List<ShowShelterByOther> ShowSheltersBuilded = new List<ShowShelterByOther>();
        public static bool IsDrinking = false;
        public static bool PreviousIsDrinking = false;
        public static bool IsEating = false;
        public static bool PreviousIsEating = false;
        public static ServerConfigData ServerConfig = new ServerConfigData();
        public static bool NowHeavyBreath = false;
        public static bool PreviousNowHeavyBreath = false;
        public static int BloodLosts = 0;
        public static int PreviousBloodLosts = 0;
        public static PlayerControlMode PreviousControlModeBeforeAction = PlayerControlMode.Normal;
        public static MultiplayerPlayer PlayerInteractionWith = null;
        public static GearItem EmergencyStimBeforeUse = null;
        public static bool HasInfecitonRisk = false;
        public static bool PreviousHasInfectionRisk = false;
        public static ContainerOpenSync MyContainer = null;
        public static int UpdateLootedContainers = -1;
        public static int UpdatePickedGears = -1;
        public static int UpdatePickedPlants = -1;
        public static int NeedConnectAfterLoad = -1;
        public static int NeedLoadSaveAfterLoad = -1;
        public static int UpdateSnowshelters = -1;
        public static int UpdateRopesAndFurns = -1;
        public static int UpdateCampfires = -1;
        public static int UpdateEverything = -1;
        public static int StartDSAfterLoad = -1;
        public static int SendAfterLoadingFinished = -1;
        public static int TryMakeLobbyAgain = -1;
        public static int RegularUpdateSeconds = 7;
        public static int MinutesFromStartServer = 0;
        public static int TimeOutSeconds = 30;
        public static int TimeOutSecondsForLoaders = 300;
        public static int TimeToWorryAboutLastRequest = 35;
        public static bool SkipEverythingForConnect = false;
        public static GameObject UISteamFreindsMenuObj = null;
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

        public static string CustomServerName = "";
        public static string MyLobby = "";
        public static bool ApplyOtherCampfires = false;
        public static bool HadEverPingedMaster = false;
        public static Dictionary<int, string> SlicedJsonDataBuffer = new Dictionary<int, string>();
        public static Dictionary<int, List<byte>> SlicedBytesDataBuffer = new Dictionary<int, List<byte>>();
        public static bool DebugTrafficCheck = false;
        public static Dictionary<string, bool> OpenableThings = new Dictionary<string, bool>();
        public static Dictionary<int, GameObject> DroppedGearsObjs = new Dictionary<int, GameObject>();
        public static Dictionary<int, GameObject> TrackableDroppedGearsObjs = new Dictionary<int, GameObject>();
        public static int OverrideLampReduceFuel = -1;
        public static Dictionary<string, float> BooksResearched = new Dictionary<string, float>();
        public static GameObject GasMaskOverlay = null;

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
        public static Borrowable LastBorrowable = null;
        public static bool UseBoltInsteadOfStone = false;
        public static bool UseGasMaskOverlay = false;
        public static bool OriginalRadioSeaker = false;
        public static bool VanilaRadio = false;


        public static GameObject ViewModelDummy = null;
        public static float RadioFrequency = 0;
        public static float PreviousRadioFrequency = 0;
        #endregion

        public static Vector3 LastSleepV3 = new Vector3(0, 0, 0);
        public static Quaternion LastSleepQuat = new Quaternion(0, 0, 0, 0);
        public static bool AtBed = false;
        public static Vector3 OutOfBedPosition = new Vector3(0, 0, 0);

        public static bool GearIDListReady = false;
        public static Dictionary<string, int> GearIDList = new Dictionary<string, int>();
        //public static bool AntiCheat = false;
        public static bool InterloperHook = false;
        public static string OverridedSceneForSpawn = "";
        public static Vector3 OverridedPositionForSpawn = Vector3.zero;
        public static string SavedSceneForSpawn = "";
        public static Vector3 SavedPositionForSpawn = Vector3.zero;
        public static bool FixedPlaceLoaded = false;

        public static string AutoStartSlot = "";
        public static List<string> AutoCMDs = new List<string>();
        public static bool CrazyPatchesLogger = false;
        public static bool KillOnUpdate = false;
        public static bool KillEverySecond = false;
        public static bool NoAnimalSync = false;

        // Other stuff
        public static List<BoardGameSession> BoardGamesSessions = new List<BoardGameSession>();

        public static void KillConsole()
        {
            if(ServerConfig.m_CheatsMode == 2 || (iAmHost == true && ServerConfig.m_CheatsMode == 1) || InOnline() == false)
            {
                return;
            }
                        
            if(uConsole.m_CommandsDict.Count > 0)
            {
                uConsole.m_CommandsDict.Clear();
                uConsole.m_CommandsList.Clear();
                uConsole.m_CommandsHelp.Clear();
            }
            if(uConsole.m_Instance != null)
            {
                uConsole.m_Instance.m_Activate = KeyCode.None;
            }
        }

        public static void InitGearsIDS()
        {
            for (int index = 0; index < ConsoleManager.m_AllGearItemNames.Count; ++index)
            {
                int exist;
                if(GearIDList.TryGetValue(ConsoleManager.m_AllGearItemNames[index], out exist) == false)
                {
                    GearIDList.Add(ConsoleManager.m_AllGearItemNames[index], index);
                }
            }
            GearIDListReady = true;
        }
        public static string GetGearNameByID(int index)
        {
            if (index >= 0 && index < ConsoleManager.m_AllGearItemNames.Count - 1)
            {
                return ConsoleManager.m_AllGearItemNames[index];
            }
            return "";
        }
        public static int GetGearIDByName(string name)
        {
            int ID;
            if(GearIDList.TryGetValue(name, out ID) == true)
            {
                return ID;
            }
            return -1;
        }

        public static bool HasNonASCIIChars(string str)
        {
            return (Encoding.UTF8.GetByteCount(str) != str.Length);
        }

        public static void AddSlicedJsonData(SlicedJsonData jData)
        {
            //MelonLogger.Msg(ConsoleColor.Yellow, "Got Slice for hash:"+jData.m_Hash+" DATA: "+jData.m_Str);
            if (SlicedJsonDataBuffer.ContainsKey(jData.m_Hash))
            {
                string previousString = "";
                if(SlicedJsonDataBuffer.TryGetValue(jData.m_Hash, out previousString) == true)
                {
                    string wholeString = previousString + jData.m_Str;
                    SlicedJsonDataBuffer.Remove(jData.m_Hash);
                    SlicedJsonDataBuffer.Add(jData.m_Hash, wholeString);
                }else{
                    SlicedJsonDataBuffer.Add(jData.m_Hash, jData.m_Str);
                }
            }else{
                SlicedJsonDataBuffer.Add(jData.m_Hash, jData.m_Str);
            }

            if (jData.m_Last)
            {
                string finalJsonData = "";
                if (SlicedJsonDataBuffer.TryGetValue(jData.m_Hash, out finalJsonData) == true)
                {
                    SlicedJsonDataBuffer.Remove(jData.m_Hash);
                    GearItemDataPacket gData = new GearItemDataPacket();
                    gData.m_GearName = jData.m_GearName;
                    gData.m_DataProxy = finalJsonData;
                    MyMod.GiveRecivedItem(gData);
                }
            }
        }

        public static void AddSlicedJsonDataForDrop(SlicedJsonData jData)
        {
            //MelonLogger.Msg(ConsoleColor.Yellow, "Got Dropped Item Slice for hash:"+jData.m_Hash+" DATA: "+jData.m_Str);
            if (SlicedJsonDataBuffer.ContainsKey(jData.m_Hash))
            {
                string previousString = "";
                if (SlicedJsonDataBuffer.TryGetValue(jData.m_Hash, out previousString) == true)
                {
                    string wholeString = previousString + jData.m_Str;
                    SlicedJsonDataBuffer.Remove(jData.m_Hash);
                    SlicedJsonDataBuffer.Add(jData.m_Hash, wholeString);
                }
                else
                {
                    SlicedJsonDataBuffer.Add(jData.m_Hash, jData.m_Str);
                }
            }else{
                SlicedJsonDataBuffer.Add(jData.m_Hash, jData.m_Str);
            }

            if (jData.m_Last)
            {
                string finalJsonData = "";
                if (SlicedJsonDataBuffer.TryGetValue(jData.m_Hash, out finalJsonData) == true)
                {
                    SlicedJsonDataBuffer.Remove(jData.m_Hash);
                    AddDroppedGear(jData.m_SendTo, jData.m_Hash, finalJsonData, jData.m_GearName, jData.m_Extra);
                    MelonLogger.Msg(ConsoleColor.Green, "Finished adding data for:" + jData.m_Hash);
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
            }else{
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
                }else{
                    __instance.m_ResearchItem.m_ElapsedHours = 0;
                }

                if (__instance.m_ResearchItem.IsResearchComplete())
                {
                    __instance.m_ResearchItem.UpdateItemType(GearTypeEnum.Firestarting);
                }else{
                    __instance.m_ResearchItem.UpdateItemType(GearTypeEnum.Tool);
                }
            }
        }

        public static void AddSlicedJsonDataForPicker(SlicedJsonData jData, bool place)
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
                }else{
                    SlicedJsonDataBuffer.Add(jData.m_Hash, jData.m_Str);
                }
            }else{
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

                    string gearName = "";

                    if (jData.m_SendTo != -1)
                    {
                        gearName = GetGearNameByID(jData.m_SendTo);
                    }else{
                        gearName = jData.m_Extra.m_GearName;
                    }

                    GameObject reference = GetGearItemObject(gearName);

                    if (reference == null)
                    {
                        MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
                        return;
                    }


                    GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(reference, new Vector3(0,0,0), new Quaternion(0,0,0,0));

                    if(newGear == null)
                    {
                        MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
                        return;
                    }

                    newGear.name = CloneTrimer(newGear.name);

                    ExtraDataForDroppedGear Extra = jData.m_Extra;

                    if(newGear.GetComponent<KeroseneLampItem>() != null && Extra != null)
                    {
                        int minutesDroped = MinutesFromStartServer - Extra.m_DroppedTime;
                        OverrideLampReduceFuel = minutesDroped;
                        MelonLogger.Msg(ConsoleColor.Cyan, "Lamp been dropped " + minutesDroped + " minutes");
                    }
                    

                    newGear.GetComponent<GearItem>().Deserialize(finalJsonData);
                    newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;

                    DropFakeOnLeave DFL = newGear.AddComponent<DropFakeOnLeave>();
                    DFL.m_OldPossition = newGear.gameObject.transform.position;
                    DFL.m_OldRotation = newGear.gameObject.transform.rotation;
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
                    }

                    if (place == false)
                    {
                        bool skipPickup = false;
                        if (newGear.GetComponent<GearItem>().m_Bed != null)
                        {
                            if(newGear.GetComponent<GearItem>().m_Bed.GetState() == BedRollState.Placed)
                            {
                                newGear.GetComponent<GearItem>().m_Bed.SetState(BedRollState.Rolled);
                                skipPickup = true;
                            }
                        }
                        if(skipPickup == false)
                        {
                            GameManager.GetPlayerManagerComponent().ProcessInspectablePickupItem(newGear.GetComponent<GearItem>());
                        }else{
                            GameManager.GetPlayerManagerComponent().ProcessPickupItemInteraction(newGear.GetComponent<GearItem>(), false, false);
                        }
                        
                    }else{
                        newGear.GetComponent<GearItem>().PlayPickUpClip();
                        GameManager.GetPlayerManagerComponent().StartPlaceMesh(newGear, PlaceMeshFlags.None);
                    }

                    PatchBookReadTime(newGear.GetComponent<GearItem>());
                }
            }
        }

        public static void AddSlicedJsonDataForContainer(SlicedJsonData jData, int From = -1)
        {
            if (SlicedJsonDataBuffer.ContainsKey(jData.m_Hash))
            {
                string previousString = "";
                if (SlicedJsonDataBuffer.TryGetValue(jData.m_Hash, out previousString) == true)
                {
                    string wholeString = previousString + jData.m_Str;
                    SlicedJsonDataBuffer.Remove(jData.m_Hash);
                    SlicedJsonDataBuffer.Add(jData.m_Hash, wholeString);
                }
                else
                {
                    SlicedJsonDataBuffer.Add(jData.m_Hash, jData.m_Str);
                }
            }else{
                SlicedJsonDataBuffer.Add(jData.m_Hash, jData.m_Str);
            }

            if (jData.m_Last)
            {
                string finalJsonData = "";
                if (SlicedJsonDataBuffer.TryGetValue(jData.m_Hash, out finalJsonData) == true)
                {
                    SlicedJsonDataBuffer.Remove(jData.m_Hash);

                    string OriginalData = jData.m_GearName;
                    string Scene = OriginalData.Split(Convert.ToChar("|"))[0];
                    string GUID = OriginalData.Split(Convert.ToChar("|"))[1];

                    MelonLogger.Msg(ConsoleColor.Green, "Finished loading container data for " + jData.m_Hash);

                    if(iAmHost == true)
                    {
                        MPSaveManager.SaveContainer(Scene, GUID, finalJsonData);
                    }
                    if(sendMyPosition == true)
                    {
                        DiscardRepeatPacket();
                        FinishOpeningFakeContainer(finalJsonData);
                    }
                }
            }

            if(From != -1)
            {
                ServerSend.READYSENDNEXTSLICE(From, true);
            }
        }

        public static void AddSlicedBytesData(SlicedBytesData bytesData, int from)
        {
            //MelonLogger.Msg(ConsoleColor.Yellow, "Got Bytes Slice for hash:" + bytesData.m_Hash);
            if (SlicedBytesDataBuffer.ContainsKey(bytesData.m_Hash))
            {
                List<byte> Buffer;
                if (SlicedBytesDataBuffer.TryGetValue(bytesData.m_Hash, out Buffer) == true)
                {
                    Buffer.AddRange(bytesData.m_Data);
                    SlicedBytesDataBuffer.Remove(bytesData.m_Hash);
                    SlicedBytesDataBuffer.Add(bytesData.m_Hash, Buffer);
                }else{
                    List<byte> Buffer2 = new List<byte>();
                    Buffer2.AddRange(bytesData.m_Data);
                    SlicedBytesDataBuffer.Add(bytesData.m_Hash, Buffer2);
                }
            }else{
                List<byte> Buffer2 = new List<byte>();
                Buffer2.AddRange(bytesData.m_Data);
                SlicedBytesDataBuffer.Add(bytesData.m_Hash, Buffer2);
            }

            if (bytesData.m_Last)
            {
                //MelonLogger.Msg(ConsoleColor.Yellow, "Got last slice for hash:" + bytesData.m_Hash);
                List<byte> FinalBuffer;
                if (SlicedBytesDataBuffer.TryGetValue(bytesData.m_Hash, out FinalBuffer) == true)
                {
                    SlicedBytesDataBuffer.Remove(bytesData.m_Hash);
                    //GotLargeDataArray(FinalBuffer.ToArray(), bytesData.m_Action, from, bytesData.m_ExtraInt);
                }
            }
        }

        public static void SendSlicedData(byte[] array, int startIdx, int length, int ClientId, string action, bool LastSlice, int ExtraInt)
        {
            //MelonLogger.Msg(ConsoleColor.Yellow, "Sending slice for array with hash:" + array.GetHashCode());
            SlicedBytesData Data = new SlicedBytesData();
            Data.m_Hash = array.GetHashCode();
            Data.m_Last = LastSlice;
            Data.m_SendTo = ClientId;
            Data.m_Action = action;

            byte[] sendData = new byte[CHUNK_SIZE];

            int ToWrite = 0;

            for (int i = startIdx; i < length; i++)
            {
                sendData[ToWrite] = array[i];
                ToWrite = ToWrite + 1;
            }
            Data.m_Data = sendData;
            Data.m_Length = sendData.Length;
            if(LastSlice == true)
            {
                Data.m_ExtraInt = ExtraInt;
            }
            if (iAmHost == true)
            {
                using (Packet _packet = new Packet((int)ServerPackets.SLICEDBYTES))
                {
                    ServerSend.SLICEDBYTES(0, Data, false, ClientId);
                }
            }
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.SLICEDBYTES))
                {
                    _packet.Write(Data);
                    SendTCPData(_packet);
                }
            }
        }

        private const int CHUNK_SIZE = 500;

        private static void SendLargeArray(byte[] array, int ForClient, string action, int ExtraInt = 0)
        {
            MelonLogger.Msg(ConsoleColor.Yellow, "SendLargeArray with hash:" + array.GetHashCode());
            for (int startIdx = 0; startIdx < array.Length; startIdx += CHUNK_SIZE)
            {
                bool IsLast = false;
                if(startIdx+CHUNK_SIZE > array.Length)
                {
                    IsLast = true;
                }
                int length = Math.Min(array.Length - startIdx, CHUNK_SIZE);
                SendSlicedData(array, startIdx, length, ForClient, action, IsLast, ExtraInt);
            }
        }
        private static void SendLargeArrayToAll(byte[] array, string action, int ExtraInt = 0)
        {
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (Server.clients[i].IsBusy() == true)
                {
                    SendLargeArray(array, i, action, ExtraInt);
                }
            }
        }

        //STRUCTS
        #region STRUCTS
        public class ServerConfigData
        {
            public bool m_FastConsumption = true;
            public bool m_DuppedSpawns = false;
            public bool m_DuppedContainers = false;
            public int m_PlayersSpawnType = 0;
            public int m_FireSync = 2;
            public int m_CheatsMode = 2;
        }
        public class ServerSettingsData
        {
            public ServerConfigData m_CFG = new ServerConfigData();
            public int m_MaxPlayers = 2;
            public int m_Port = 26950;
            public int m_Accessibility = 0;
            public bool m_P2P = true;
        }
        public class SlicedJsonData
        {
            public bool m_Last = false;
            public string m_Str = "";
            public int m_Hash = 0;
            public string m_GearName = "";
            public int m_SendTo = 0;
            public ExtraDataForDroppedGear m_Extra = new ExtraDataForDroppedGear();
        }
        public class SlicedBytesData
        {
            public bool m_Last = false;
            public byte[] m_Data;
            public int m_Length = 0; 
            public int m_Hash = 0;
            public int m_SendTo = 0;
            public string m_Action = "";
            public int m_ExtraInt = 0;
        }
        public class SlicedJsonDroppedGear
        {
            public string m_GearName = "";
            public string m_Json = "";
            public ExtraDataForDroppedGear m_Extra = new ExtraDataForDroppedGear();
        }
        public class ExtraDataForDroppedGear
        {
            public int m_DroppedTime = 0;
            public int m_GoalTime = 0;
            public string m_Dropper = "";
            public int m_Variant = 0;
            public string m_GearName = "";
        }
        public class AnimalCompactData
        {
            public string m_PrefabName = "";
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Quaternion m_Rotation = new Quaternion(0, 0, 0, 0);
            public string m_GUID = "";
            public int m_LastSeen = 0;
            public float m_Health = 100;
            public bool m_Bleeding = false;
            public int m_TimeOfBleeding = 0;
            public string m_RegionGUID = "";
            public int m_LastController = 0;
            public int m_LastAiMode = 0;
        }
        public class PlayerEquipmentData //: MelonMod
        {
            public string m_HoldingItem = "";
            public bool m_LightSourceOn = false;
            public bool m_HasAxe = false;
            public bool m_HasRifle = false;
            public bool m_HasRevolver = false;
            public bool m_HasMedkit = false;
            public int m_Arrows = 0;
            public int m_Flares = 0;
            public int m_BlueFlares = 0;
        }
        public class PlayerClothingData //: MelonMod
        {
            public string m_Hat = "";
            public string m_Top = "";
            public string m_Bottom = "";
            public string m_Boots = "";
            public string m_Scarf = "";
            public string m_Balaclava = "";
        }
        public class MultiPlayerClientData //: MelonMod
        {
            public PlayerEquipmentData m_PlayerEquipmentData = new PlayerEquipmentData();
            public PlayerClothingData m_PlayerClothingData = new PlayerClothingData();
            public string m_Name = "";
            public Vector3 m_Position = new Vector3();
            public Quaternion m_Rotation = new Quaternion();
            public int m_Levelid = 0;
            public string m_AnimState = "Idle";
            public int m_SleepHours = 0;
            public string m_HarvestingAnimal = "";
            public bool m_Mimic = false;
            public bool m_Used = false;
            public bool m_Dead = false;
            public int m_TicksOnScene = 0;
            public int m_PreviousLevelId = 0;
            public string m_LevelGuid = "";
            public string m_BrakingSounds = "";
            public BrokenFurnitureSync m_BrakingObject = new BrokenFurnitureSync();
            public bool m_HeavyBreath = false;
            public int m_BloodLosts = 0;
            public bool m_NeedAntiseptic = false;
            public ContainerOpenSync m_Container = null;
            public string m_Plant = "";
            public bool m_Female = false;
            public int m_Character = 0;
            public ShowShelterByOther m_Shelter = null;
            public bool m_Aiming = false;
            public float m_RadioFrequency = 0;
        }
        public class MultiPlayerClientStatus //: MelonMod
        {
            public int m_ID = 0;
            public string m_Name = "";
            public bool m_Sleep = false;
            public bool m_Dead = false;
        }

        public class DeathContainerData
        {
            public string m_Guid = "";
            public string m_LevelKey = "";
            public string m_Owner = "";
            public string m_ContainerPrefab = "CONTAINER_BackPack";
            public Vector3 m_Position = Vector3.zero;
            public Quaternion m_Rotation = Quaternion.identity;
            public bool Equals(DeathContainerData other)
            {
                if (other == null)
                    return false;

                if (m_Guid == other.m_Guid)
                {
                    return true;
                }else{
                    return false;
                }
            }
        }

        public class ContainerOpenSync : IEquatable<ContainerOpenSync>
        {
            public string m_Guid = "";
            public bool m_State = false;
            public int m_LevelID = 0;
            public string m_LevelGUID = "";
            public bool m_Inspected = true;

            public bool Equals(ContainerOpenSync other)
            {
                if (other == null)
                    return false;

                if (this.m_Guid == other.m_Guid &&
                    this.m_LevelID == other.m_LevelID &&
                    this.m_LevelGUID == other.m_LevelGUID)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public class ShootSync //: MelonMod
        {
            public Vector3 m_position = new Vector3(0, 0, 0);
            public Quaternion m_rotation = new Quaternion(0, 0, 0, 0);
            public string m_projectilename = "";
            public float m_skill = 0;
            public Vector3 m_camera_forward = new Vector3(0, 0, 0);
            public Vector3 m_camera_right = new Vector3(0, 0, 0);
            public Vector3 m_camera_up = new Vector3(0, 0, 0);
        }
        public class HarvestStats //: MelonMod
        {
            public float m_Meat = 0;
            public int m_Guts = 0;
            public int m_Hide = 0;
            public string m_Guid = "";
        }
        public class AnimalSync //: MelonMod
        {
            public Vector3 m_position = new Vector3(0, 0, 0);
            public Quaternion m_rotation = new Quaternion(0, 0, 0, 0);
            public string m_guid = "";
            public string m_name = "";
            public float m_Hp = 100;
            public bool m_Bleeding = false;

            public int m_Controller = 0;
            public string m_ProxySave = "";
            public int m_LevelD = 0;
            public string m_SpawnRegionGUID = "";
        }
        public class AnimalAnimsSync
        {
            public float AP_TurnAngle; //0
            public float AP_TurnSpeed; //1
            public float AP_Speed; //2
            public float AP_Wounded; //3
            public float AP_Roll; //4
            public float AP_Pitch; //5
            public float AP_TargetHeading; //6
            public float AP_TargetHeadingSmooth; //7
            public float AP_TapMeter; //8
            //AE_Trigger_Branchpoint; //9
            public int AP_AiState; //10
            //StruggleStart //11
            //StruggleEnd //12
            //DamageImpact //13
            public bool AP_Corpse; //14
            public bool AP_Dead; //15
            public int AP_DeadSide; //16
            //AE_IsInStruggle //17
            //public int AP_AE_NavigationState; //18
            public int AP_DamageBodyPart; //19
            //public int AP_AE_CorpseID; //20
            //ScriptedSequence_Hostile //21
            //ScriptedSequence_Feeding //22
            public int AP_AttackId; //23
            //Attack_Trigger //24
            public bool AP_Stunned;
        }

        public class WalkTracker //: MelonMod
        {
            public int m_levelid = 0;
            public Vector3 m_V3 = new Vector3(0, 0, 0);
        }
        public class WeatherProxies //: MelonMod
        {
            public string m_WeatherProxy = "";
            public string m_WeatherTransitionProxy = "";
            public string m_WindProxy = "";
        }
        public class AnimalAligner //: MelonMod
        {
            public string m_Proxy = "";
            public string m_Guid = "";
        }
        public class SaveSlotSync //: MelonMod
        {
            public int m_Episode;
            public int m_SaveSlotType;
            public int m_Seed;
            public int m_ExperienceMode;
            public int m_Location;
            public string m_FixedSpawnScene;
            public Vector3 m_FixedSpawnPosition;
            public string m_CustomExperienceStr;
        }
        public class MultiplayerChatMessage //: MelonMod
        {
            public int m_Type = 0;
            public string m_Message = "";
            public string m_By = "";
            public UnityEngine.UI.Text m_TextObj = null;
        }
        public class GearItemDataPacket
        {
            public string m_GearName = "";
            public string m_DataProxy = "";
            public float m_Water = 0;
            public int m_SendedTo = -1;
        }
        public class DroppedGearItemDataPacket
        {
            public int m_GearID = -1;
            public Vector3 m_Position;
            public Quaternion m_Rotation;
            public int m_LevelID = 0;
            public string m_LevelGUID = "";
            public int m_Hash = 0;
            public ExtraDataForDroppedGear m_Extra = new ExtraDataForDroppedGear();
        }
        public class BrokenFurnitureSync : IEquatable<BrokenFurnitureSync>
        {
            public string m_Guid = "";
            public string m_ParentGuid = "";
            public int m_LevelID = 0;
            public string m_LevelGUID = "";

            public bool Equals(BrokenFurnitureSync other)
            {
                if (other == null)
                    return false;

                if (this.m_Guid == other.m_Guid &&
                    this.m_ParentGuid == other.m_ParentGuid &&
                    this.m_LevelID == other.m_LevelID &&
                    this.m_LevelGUID == other.m_LevelGUID)
                {
                    return true;
                }else{
                    return false;
                } 
            }
        }
        public class PickedGearSync : IEquatable<PickedGearSync>
        {
            public Vector3 m_Spawn = new Vector3(0,0,0);
            public int m_LevelID = 0;
            public string m_LevelGUID = "";
            public int m_PickerID = 0;
            public int m_Recently = 0;
            public int m_MyInstanceID = 0;

            public bool Equals(PickedGearSync other)
            {
                if (other == null)
                    return false;

                if (this.m_LevelID == other.m_LevelID && this.m_LevelGUID == other.m_LevelGUID && this.m_Spawn == other.m_Spawn)
                {
                    return true;
                }else{
                    return false;
                }
            }
        }
        public class ClimbingRopeSync : IEquatable<ClimbingRopeSync>
        {
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public int m_LevelID = 0;
            public string m_LevelGUID = "";
            public bool m_Deployed = false;
            public bool m_Snapped = false;

            public bool Equals(ClimbingRopeSync other)
            {
                if (other == null)
                    return false;

                if (this.m_LevelID == other.m_LevelID && this.m_LevelGUID == other.m_LevelGUID && this.m_Position == other.m_Position)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public class HarvestableSyncData : IEquatable<HarvestableSyncData>
        {
            public string m_Guid = "";
            public string m_State = "";
            public bool Equals(HarvestableSyncData other)
            {
                if (other == null)
                    return false;

                if (this.m_Guid == other.m_Guid)
                {
                    return true;
                }else{
                    return false;
                }
            }
        }
        public class ShowShelterByOther : IEquatable<ShowShelterByOther>
        {
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Quaternion m_Rotation = new Quaternion(0, 0, 0, 0);
            public int m_LevelID = 0;
            public string m_LevelGUID = "";
            public bool Equals(ShowShelterByOther other)
            {
                if (other == null)
                    return false;

                if (this.m_LevelID == other.m_LevelID && this.m_LevelGUID == other.m_LevelGUID && this.m_Position == other.m_Position)
                {
                    return true;
                }else{
                    return false;
                }
            }
        }
        public class FireSourcesSync : IEquatable<FireSourcesSync>
        {
            public string m_Guid = "";
            public int m_LevelId = 0;
            public string m_LevelGUID = "";
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Quaternion m_Rotation = new Quaternion(0, 0, 0, 0);
            public float m_Fuel = 0;
            public int m_RemoveIn = 0;
            public bool m_IsCampfire = false;
            public string m_FuelName = "";

            public bool Equals(FireSourcesSync other)
            {
                if (other == null)
                    return false;

                if ((this.m_LevelId == other.m_LevelId && this.m_LevelGUID == other.m_LevelGUID) 
                 && ((this.m_Guid != "" && this.m_Guid == other.m_Guid) || this.m_Position == other.m_Position))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public class PriorityActionForOtherPlayer
        {
            public string m_Action = "";
            public string m_DisplayText = "";
            public string m_ProcessText = "";
            public bool m_CancleOnMove = false;
            public float m_ActionDuration = 3;
        }

        public static PriorityActionForOtherPlayer GetActionForOtherPlayer(string ActName)
        {
            PriorityActionForOtherPlayer act = new PriorityActionForOtherPlayer();
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
            }
            else if (ActName == "Lit")
            {
                act.m_Action = "Lit";
                act.m_DisplayText = "Ignite from player's fire";
                act.m_ProcessText = "";
                act.m_CancleOnMove = false;
                act.m_ActionDuration = 0;
            }
            else if(ActName == "Examine")
            {
                act.m_Action = "Examine";
                act.m_DisplayText = "Examine";
                act.m_ProcessText = "Diagnosing...";
                act.m_CancleOnMove = true;
                act.m_ActionDuration = 1;
            }else if(ActName == "Borrow")
            {
                act.m_Action = "Borrow";
                act.m_DisplayText = "Borrow";
                act.m_ProcessText = "Borrowing...";
                act.m_CancleOnMove = true;
                act.m_ActionDuration = 1;
            }

            return act;
        }
        public class VoiceChatQueueElement
        {
            public byte[] m_VoiceData;
            public float m_Length = 0; 
        }
        #endregion

        public static void LoadChatName(string _name = "")
        {
            bool UseSteamName = false;
            bool HaveFile = false;
            if (System.IO.File.Exists("Mods\\nickname.txt"))
            {
                string readText = System.IO.File.ReadAllText("Mods\\nickname.txt");
                if (ValidNickName(readText) == false)
                {
                    UseSteamName = true;
                    MyChatName = "Player";
                }else{
                    MyChatName = readText;
                }
                HaveFile = true;
            }else{
                MyChatName = "Player";
                UseSteamName = true;
                HaveFile = false;
            }

            if(ValidNickName(_name) == false)
            {
                UseSteamName = false;
            }

            if(UseSteamName == true)
            {
                MyChatName = _name;
            }else{
                if (ValidNickName(MyChatName) == false)
                {
                    string _word = "";
                    if (HaveFile == true)
                    {
                        _word = "edit";
                    }
                    else
                    {
                        _word = "create";
                    }
                    MelonLogger.Msg(ConsoleColor.Magenta, "You have default name, if you want to change your name, " + _word + " nickname.txt file in Mods folder!");
                }
            }
            MelonLogger.Msg("Your chat name is "+ MyChatName);
        }

        public class DedicatedServerData
        {
            public string SaveSlot;
            public bool ItemDupes;
            public bool ContainersDupes;
            public int SpawnStyle;
            public int MaxPlayers;
            public bool UsingSteam;
            public int Ports;
            public string[] WhiteList;
            public string ServerName;
            public int Cheats;
            public int SteamServerAccessibility;
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

        public static void ForceLoadSlotForDs(string searchname)
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
            if(HaveSaveFile == false)
            {
                MelonLogger.Msg(ConsoleColor.Red, "[Dedicated server] No save file with name "+ searchname+" found!");
            }else{
                SaveGameSlots.SetBaseNameForSave(SaveToLoad.m_SaveSlotName, SaveToLoad.m_SaveSlotName);
                MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Save slot base name is " + SaveGameSlots.GetBaseNameForSave(SaveToLoad.m_SaveSlotName));
                MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Save slot name " + SaveToLoad.m_SaveSlotName);
                MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Save slot user defined name " + SaveGameSlots.GetUserDefinedSlotName(SaveToLoad.m_SaveSlotName));
                SaveGameSystem.SetCurrentSaveInfo(SaveToLoad.m_Episode, SaveToLoad.m_GameMode, SaveToLoad.m_GameId, SaveToLoad.m_SaveSlotName);
                MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Selecting slot " + SaveGameSystem.GetCurrentSaveName());
                GameManager.LoadSaveGameSlot(SaveToLoad.m_SaveSlotName, SaveToLoad.m_SaveChangelistVersion);
            }
        }

        public static void ForceCreateSlotForPlaying(int Region, int Experience)
        {
            MyMod.m_Panel_Sandbox.Enable(false);
            SaveSlotSync SaveFile = new SaveSlotSync();
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
            }else{
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
        }

        public static bool SetP2PToLobbyForDSAfterLoad = false;

        public static void StartDedicatedServer(bool ds = true)
        {
            string ServStr = "[Dedicated server]";
            if (ds == false)
            {
                ServStr = "[AutoStart server]";
            }
            
            if (System.IO.File.Exists("Mods\\server.json"))
            {
                MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Reading server.json...");
                string readText = System.IO.File.ReadAllText("Mods\\server.json");
                DedicatedServerData ServerData = JSON.Load(readText).Make<DedicatedServerData>();
                MelonLogger.Msg(ConsoleColor.Magenta, ServStr+" Server settings: ");
                if(ds == true)
                {
                    MelonLogger.Msg(ConsoleColor.Blue, "[Dedicated server] SaveSlot: " + ServerData.SaveSlot);
                }
                MelonLogger.Msg(ConsoleColor.Blue, ServStr + " ItemDupes: " + ServerData.ItemDupes);
                MelonLogger.Msg(ConsoleColor.Blue, ServStr + " ContainersDupes: " + ServerData.ContainersDupes);
                MelonLogger.Msg(ConsoleColor.Blue, ServStr + " SpawnStyle: " + ServerData.SpawnStyle);
                MelonLogger.Msg(ConsoleColor.Blue, ServStr + " MaxPlayers: " + ServerData.MaxPlayers);
                MelonLogger.Msg(ConsoleColor.Blue, ServStr + " UsingSteam: " + ServerData.UsingSteam);
                MelonLogger.Msg(ConsoleColor.Blue, ServStr + " Ports: " + ServerData.Ports);
                MelonLogger.Msg(ConsoleColor.Blue, ServStr + " Cheats: " + ServerData.Cheats);
                MelonLogger.Msg(ConsoleColor.Blue, ServStr + " SteamServerAccessibility: " + ServerData.SteamServerAccessibility);
                MelonLogger.Msg(ConsoleColor.Magenta, ServStr + " No problems with server.json found!");

                ServerConfig.m_DuppedContainers = ServerData.ContainersDupes;
                ServerConfig.m_DuppedSpawns = ServerData.ItemDupes;
                ServerConfig.m_PlayersSpawnType = ServerData.SpawnStyle;
                ServerConfig.m_CheatsMode = ServerData.Cheats;
                MaxPlayers = ServerData.MaxPlayers;
                if (ds == true)
                {
                    MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Trying to load save file...");
                    MyChatName = "DedicatedServer";
                }

                if(SteamConnect.CanUseSteam == false && ServerData.UsingSteam == true)
                {
                    ServerData.UsingSteam = false;
                    MelonLogger.Msg(ConsoleColor.Red, ServStr + " In server.json 'UsingSteam' set to true, but you using version of the game that not support Hosting with using Steam! If you use licence version of the game, but still see this, reboot steam.");
                    MelonLogger.Msg(ConsoleColor.Magenta, ServStr + " Forced changing 'UsingSteam' to false, and running regular server.");
                }

                if (ServerData.UsingSteam == false)
                {
                    HostAServer(ServerData.Ports);
                }else{
                    CustomServerName = ServerData.ServerName;
                    if(ds == true)
                    {
                        InitAllPlayers();
                        SteamConnect.Main.MakeLobby(ServerData.SteamServerAccessibility, ServerData.MaxPlayers);
                        SetP2PToLobbyForDSAfterLoad = true;
                    }else{
                        Server.StartSteam(MaxPlayers);
                    }
                }
                if(ds == true)
                {
                    ForceLoadSlotForDs(ServerData.SaveSlot);
                }
            }else{
                MelonLogger.Msg(ConsoleColor.Red, ServStr + " Can't find server.json!");
            }
        }

        private static void CreateZipFromText(string text)
        {
            MelonLogger.Msg("[Plugins] Preparing to unzip virual archive with plugins...");
            byte[] zipBytes = Convert.FromBase64String(text);
            using (var memoryStream = new MemoryStream(zipBytes))
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory("Plugins");
                }
            }
            MelonLogger.Msg("[Plugins] Unzip finished! Now need to restart the game!");
        }

        public static bool CheckPlugins()
        {
            MelonLogger.Msg("[Plugins] Checking plugins...");
            
            if (Directory.Exists("Plugins"))
            {
                if (!File.Exists("Plugins\\AutoUpdatingPlugin.dll"))
                {
                    MelonLogger.Msg("[Plugins] There no AutoUpdatingPlugin.dll");
                    return false;
                }

                if (!File.Exists("Plugins\\AudioCore.dll"))
                {
                    MelonLogger.Msg("[Plugins] There no AudioCore.dll");
                    return false;
                }
                return true;
            }else{
                MelonLogger.Msg("[Plugins] No plugins folder, creating one");
                Directory.CreateDirectory("Plugins");
                return false;
            }
        }

        public override void OnApplicationStart()
        {
            if (CheckPlugins())
            {
                MelonLogger.Msg(ConsoleColor.Green, "[Plugins] Everything is alright!");
            }else{
                MelonLogger.Msg(ConsoleColor.Yellow, "[Plugins] Need install plugins!");
                CreateZipFromText(Base64Files.PluginsZip);
                KillOnUpdate = true;
                return;
            }

            bool ForceNoSteam = Environment.GetCommandLineArgs().Contains("-nosteam");
            bool ForceNoEgs= Environment.GetCommandLineArgs().Contains("-noegs");
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
                    MelonLogger.Msg("[EpicOnlineServicesManager] This probably pirated GOG version, can't verify, so good bye");
                    //Watch out this is GOG
                    Application.OpenURL("https://youtu.be/-2ySzV-HGxM");
                    Application.OpenURL("https://www.google.com/search?q=how+to+buy+The+Long+Dark+on+steam+if+I+am+pirate");
                    Application.Quit();
                }else{
                    MelonLogger.Msg("[EpicOnlineServicesManager] This game version has EpicOnlineServicesManager");
                    var postfix = typeof(Pathes).GetMethod("EGSHook");
                    HarmonyInstance.Patch(original, null, new HarmonyLib.HarmonyMethod(postfix));
                    MelonLogger.Msg("[EpicOnlineServicesManager] Patching EpicOnlineServicesManager complete!");
                }
            }else{
                MelonLogger.Msg("[SteamWorks.NET] This game version has SteamManager");
                var original = typeof(SteamManager).GetMethod("Awake");
                var postfix = typeof(SteamConnect).GetMethod("Init");
                HarmonyInstance.Patch(original, null, new HarmonyLib.HarmonyMethod(postfix));
                MelonLogger.Msg("[SteamWorks.NET] Patching SteamManager complete!");
            }

            Debug.Log($"[{InfoAttribute.Name}] Version {InfoAttribute.Version} loaded!");
            ClassInjector.RegisterTypeInIl2Cpp<AnimalUpdates>();
            ClassInjector.RegisterTypeInIl2Cpp<DestoryArrowOnHit>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerBulletDamage>();
            ClassInjector.RegisterTypeInIl2Cpp<ClientProjectile>();
            ClassInjector.RegisterTypeInIl2Cpp<MultiplayerPlayer>();
            ClassInjector.RegisterTypeInIl2Cpp<ContainersSync>();
            ClassInjector.RegisterTypeInIl2Cpp<Outline>();
            ClassInjector.RegisterTypeInIl2Cpp<UiButtonPressHook>();
            ClassInjector.RegisterTypeInIl2Cpp<DestoryStoneOnStop>();
            ClassInjector.RegisterTypeInIl2Cpp<GearSpawnPointSave>();
            ClassInjector.RegisterTypeInIl2Cpp<MultiplayerPlayerAnimator>();
            ClassInjector.RegisterTypeInIl2Cpp<MultiplayerPlayerClothingManager>();
            ClassInjector.RegisterTypeInIl2Cpp<FakeFire>();
            ClassInjector.RegisterTypeInIl2Cpp<FakeFireLight>();
            ClassInjector.RegisterTypeInIl2Cpp<DoNotSerializeThis>();
            ClassInjector.RegisterTypeInIl2Cpp<MultiplayerPlayerVoiceChatPlayer>();
            ClassInjector.RegisterTypeInIl2Cpp<DroppedGearDummy>();
            ClassInjector.RegisterTypeInIl2Cpp<IgnoreDropOverride>();
            ClassInjector.RegisterTypeInIl2Cpp<DropFakeOnLeave>();
            ClassInjector.RegisterTypeInIl2Cpp<FakeBed>();
            ClassInjector.RegisterTypeInIl2Cpp<FakeBedDummy>();
            ClassInjector.RegisterTypeInIl2Cpp<AnimalActor>();
            ClassInjector.RegisterTypeInIl2Cpp<AnimalCorpseObject>();
            ClassInjector.RegisterTypeInIl2Cpp<SpawnRegionSimple>();
            ClassInjector.RegisterTypeInIl2Cpp<LobbyHoverNickname>();
            ClassInjector.RegisterTypeInIl2Cpp<CookpotHelmet>();
            ClassInjector.RegisterTypeInIl2Cpp<Borrowable>();
            ClassInjector.RegisterTypeInIl2Cpp<UiButtonKeyboardPressSkip>();
            ClassInjector.RegisterTypeInIl2Cpp<LeftHandHelper>();
            ClassInjector.RegisterTypeInIl2Cpp<DeathDropContainer>();

            if (instance == null)
            {
                instance = this;
                tcp = new TCP();
                udp = new UDP();
            }

            LoadedBundle = AssetBundle.LoadFromFile("Mods\\multiplayerstuff.unity3d");
            if (LoadedBundle == null)
            {
                MelonLogger.Msg("Have problems with loading multiplayerstuff.unity3d!!");
            }else{
                MelonLogger.Msg("Models loaded.");
            }

            if (Application.isBatchMode)
            {
                MelonLogger.Msg(ConsoleColor.Magenta,"[Dedicated server] Please wait...");
                InputManager.m_InputDisableTime = float.PositiveInfinity;
            }
            uConsole.RegisterCommand("oleg", new Action(LootEverything));


            //SaveGameData.LoadData("MultiplayerGUID");

            //System.Random RNG = new System.Random();
            //int GUIDseed = RNG.Next(1, 999999);
            //int GUIDx = RNG.Next(-7000, 999999);
            //int GUIDy = RNG.Next(-3000, 999999);
            //int GUIDz = RNG.Next(-5370, 999999);

            //string MyMpGUID = GenerateSeededGUID(GUIDseed, new Vector3(GUIDx, GUIDy, GUIDz));
            //SaveGameData.SaveData("MultiplayerGUID", SaveGameSystem.PROFILE_DISPLAYNAME, MyMpGUID);
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

        public override void OnApplicationQuit()
        {
            MelonLogger.Msg("[CLIENT] Disconnect cause quit game");
            Disconnect();
        }

        public static Container MakeDeathCreate(DeathContainerData data)
        {
            if(ObjectGuidManager.Lookup(data.m_Guid) != null)
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
            if(box.GetComponent<GearItem>() != null)
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
            if(Con == null)
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
            if(box.GetComponent<DeathDropContainer>() == null)
            {
                box.AddComponent<DeathDropContainer>().m_Owner = data.m_Owner;
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
            if(Box == null)
            {
                MelonLogger.Msg("Death container "+ GUID + " not exist, can't be deleted");
            }else{
                UnityEngine.Object.Destroy(Box);
            }
            

            if (iAmHost)
            {
                foreach (DeathContainerData create in DeathCreates)
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
            DeathContainerData ObjSync = new DeathContainerData();
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
                List<GearItem> Gears = new List<GearItem>() ;
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

                        if(GameManager.GetPlayerManagerComponent().m_ItemInHands && GameManager.GetPlayerManagerComponent().m_ItemInHands == gearItem)
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
                        }else{
                            Box.AddGear(Gear);
                            Inv.RemoveGear(Gear.gameObject, true);
                        }
                    }
                }

                string Content = Box.Serialize();

                if (iAmHost)
                {
                    MPSaveManager.SaveContainer(ObjSync.m_LevelKey, ObjSync.m_Guid, CompressString(Content));
                    DeathCreates.Add(ObjSync);
                    ServerSend.ADDDEATHCONTAINER(ObjSync, ObjSync.m_LevelKey, 0);
                }
                else if (sendMyPosition)
                {
                    DoPleaseWait("Please wait...", "Sending container data...");
                    SendContainerData(CompressString(Content), ObjSync.m_LevelKey, ObjSync.m_Guid);
                    using (Packet _packet = new Packet((int)ClientPackets.ADDDEATHCONTAINER))
                    {
                        _packet.Write(ObjSync);
                        SendTCPData(_packet);
                    }
                }

                Box.DestroyAllGear();
                MelonLogger.Msg("Everything dropped");
            }
        }


        public override void OnLevelWasInitialized(int level)
        {
            if(KillOnUpdate == true)
            {
                return;
            }
            
            OpenablesObjs.Clear();
            MelonLogger.Msg("Level initialized: " + level);
            levelid = level;
            MelonLogger.Msg("Level name: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            level_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            MyTicksOnScene = 0;
            if (IamShatalker == true)
            {
                uConsole.RunCommandSilent("Ghost");
                uConsole.RunCommandSilent("God");
            }

            if(level_name != "MainMenu" && level_name != "Boot" && level_name != "Empty")
            {
                CheckHaveBookMod();
            }

            DisableOriginalAnimalSpawns();
            if (level_name == "MainMenu" && GearIDListReady == false)
            {
                InitGearsIDS(); // Compilating List of Gear names by ID, to reference on it to shorten names in sync packets
            }
            InitAllPlayers();
        }

        public static GameObject MakeModObject(string _name, Transform newparent = null)
        {
            if(LoadedBundle == null)
            {
                MelonLogger.Msg("[Object loader] Bundle is null ");
            }

            GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>(_name);

            if(LoadedAssets == null)
            {
                MelonLogger.Msg("[Object loader] Can't load asset. Has try load "+ _name);
            }

            GameObject _Obj = null;

            if (newparent == null)
            {
                _Obj = GameObject.Instantiate(LoadedAssets);
            }else{
                _Obj = GameObject.Instantiate(LoadedAssets, newparent);
            }

            if (_Obj == null)
            {
                MelonLogger.Msg("[Object loader] Object is null  "+ _name);
            }

            return _Obj;
        }

        private void Start()
        {
            tcp = new TCP();
            udp = new UDP();
        }
        public static bool IsZero(float a)
        {
            return Mathf.Abs(a) <= 0.0001f;
        }

        public static MyMod instance;
        public static int dataBufferSize = 4096;

        public string ip = "127.0.0.1";
        public int port = 26950;
        public int myId = 0;
        public TCP tcp;
        public UDP udp;

        private delegate void PacketHandler(Packet _packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        public static void SimUDPHandle(int _packetId, Packet _packet, int _from)
        {
            //MelonLogger.Msg("[UDP SIM HOST] Trying sim packet " + _packetId);
            Server.packetHandlers[_packetId](_from, _packet);
        }
        public static void SimUDPHandle_Client(int _packetId, Packet _packet)
        {
            //MelonLogger.Msg("[UDP SIM CLIENT] Trying sim packet " + _packetId);
            packetHandlers[_packetId](_packet);
        }

        public static void NoSyncFurtitureDestory(BreakDown breakDown)
        {
            breakDown.gameObject.SetActive(false);
            if (breakDown.gameObject.transform.parent && breakDown.gameObject.transform.parent.GetComponent<RadialObjectSpawner>())
            {
                BreakDown.m_BreakDownObjects.Remove(breakDown);
                breakDown.gameObject.transform.parent.GetComponent<RadialObjectSpawner>().RemoveFromSpawns(breakDown.gameObject);
                RadialSpawnManager.ReturnToObjectPool(breakDown.gameObject);
            }
            breakDown.StickSurfaceObjectsToGround();
            MissionUtils.PostObjectEvent(breakDown.gameObject, MissionTypes.MissionObjectEvent.ObjectBrokenDown);
        }

        //public static void DestoryBrokenFurniture()
        //{
        //    for (int i = 0; i < BreakDown.m_BreakDownObjects.Count; i++)
        //    {
        //        GameObject gameObject = BreakDown.m_BreakDownObjects[i].gameObject;
        //        string breakGuid = "";
        //        string breakParentGuid = "";
        //        if (gameObject != null)
        //        {
        //            ObjectGuid BreakGuidComp = gameObject.GetComponent<ObjectGuid>();
        //            if (BreakGuidComp != null)
        //            {
        //                breakGuid = BreakGuidComp.Get();
        //            }
        //            if (gameObject.transform.parent != null)
        //            {
        //                ObjectGuid BreakGuidParentComp = gameObject.transform.parent.GetComponent<ObjectGuid>();
        //                if (BreakGuidParentComp != null)
        //                {
        //                    breakParentGuid = BreakGuidParentComp.Get();
        //                }
        //            }
        //        }
        //        BrokenFurnitureSync furntest = new BrokenFurnitureSync();
        //        furntest.m_Guid = breakGuid;
        //        furntest.m_ParentGuid = breakParentGuid;
        //        furntest.m_LevelID = levelid;
        //        furntest.m_LevelGUID = level_guid;

        //        MelonLogger.Msg("BrokenFurniture.Count " + BrokenFurniture.Count);

        //        for (int n = 0; n < BrokenFurniture.Count; n++)
        //        {
        //            BrokenFurnitureSync furnCurr = BrokenFurniture[n];
        //            if (furnCurr.m_Guid == furntest.m_Guid &&
        //                furnCurr.m_ParentGuid == furntest.m_ParentGuid &&
        //                furnCurr.m_LevelID == furntest.m_LevelID &&
        //                furnCurr.m_LevelGUID == furntest.m_LevelGUID)
        //            {
        //                MelonLogger.Msg("Found " + breakGuid);
        //                NoSyncFurtitureDestory(BreakDown.m_BreakDownObjects[i]);
        //            }
        //        }
        //    }
        //}

        public static string GetBreakDownSound(BrokenFurnitureSync FindData)
        {
            GameObject furn = ObjectGuidManager.Lookup(FindData.m_Guid);
            if (furn != null)
            {
                if (FindData.m_ParentGuid != "")
                {
                    if (furn.transform.parent != null && furn.transform.parent.GetComponent<ObjectGuid>() != null && furn.transform.parent.GetComponent<ObjectGuid>().Get() == FindData.m_ParentGuid)
                    {
                        return furn.GetComponent<BreakDown>().m_BreakDownAudio;
                    }
                }else{
                    return furn.GetComponent<BreakDown>().m_BreakDownAudio;
                }
            }
            return "";
        }

        public static bool RemoveAttachedObjectsAfterSecond = false;

        public static void DestoryBrokenFurniture()
        {
            bool FoundSomethingToBreak = false;
            for (int n = 0; n < BrokenFurniture.Count; n++)
            {
                BrokenFurnitureSync FindData = BrokenFurniture[n];

                if(FindData.m_LevelID == levelid && FindData.m_LevelGUID == level_guid)
                {
                    GameObject furn = ObjectGuidManager.Lookup(FindData.m_Guid);
                    if (furn != null)
                    {
                        if (FindData.m_ParentGuid != "")
                        {
                            if (furn.transform.parent != null && furn.transform.parent.GetComponent<ObjectGuid>() != null && furn.transform.parent.GetComponent<ObjectGuid>().Get() == FindData.m_ParentGuid)
                            {
                                NoSyncFurtitureDestory(furn.GetComponent<BreakDown>());
                                FoundSomethingToBreak = true;
                            }
                        }else{
                            NoSyncFurtitureDestory(furn.GetComponent<BreakDown>());
                            FoundSomethingToBreak = true;
                        }
                    }
                }
            }
            if(FoundSomethingToBreak == true)
            {
                RemoveAttachedObjectsAfterSecond = true; //Because some of gears may be was on that broken prop.
            }
        }

        public static void DestoryJustOneGear()
        {

        }

        public static void DestoryAnSpecificGear(GearItem currentGear)
        {
            if (ServerConfig.m_DuppedSpawns == true)
            {
                return;
            }

            if (GearManager.m_Gear.Count == 0 || PickedGears.Count == 0)
            {
                return;
            }

            PickedGearSync FindData = new PickedGearSync();
            FindData.m_LevelID = levelid;
            FindData.m_LevelGUID = level_guid;
            FindData.m_Spawn = currentGear.gameObject.transform.position;

            if (currentGear.m_BeenInPlayerInventory == false && currentGear.gameObject.activeSelf == true)
            {
                SaveGearSpawn(currentGear.gameObject);
            }

            if (PickedGears.Contains(FindData) == true)
            {
                if (currentGear.m_BeenInPlayerInventory == false && currentGear.gameObject.activeSelf == true)
                {
                    if (currentGear.gameObject.transform.position == FindData.m_Spawn)
                    {
                        MelonLogger.Msg("Destroy Picked Gear X " + currentGear.transform.position.x + " Y " + currentGear.transform.position.y + " Z " + currentGear.transform.position.z);
                        currentGear.gameObject.SetActive(false);
                    }
                }
            }
        }

        public static void DestoryPickedGears()
        {
            MelonLogger.Msg("DestoryPickedGears called");
            //float d = Time.time;
            //MelonLogger.Msg("DestoryPickedGears Time before "+ d);

            if (ServerConfig.m_DuppedSpawns == true)
            {
                return;
            }

            if(GearManager.m_Gear.Count == 0 || PickedGears.Count == 0)
            {
                return;
            }

            for (int i = 0; i < GearManager.m_Gear.Count; i++)
            {
                GearItem currentGear = GearManager.m_Gear[i];
                PickedGearSync FindData = new PickedGearSync();
                FindData.m_LevelID = levelid;
                FindData.m_LevelGUID = level_guid;
                FindData.m_Spawn = currentGear.gameObject.transform.position;

                if (currentGear.m_BeenInPlayerInventory == false && currentGear.gameObject.activeSelf == true)
                {
                    SaveGearSpawn(currentGear.gameObject);
                }

                if(PickedGears.Contains(FindData) == true)
                {
                    if (currentGear.m_BeenInPlayerInventory == false && currentGear.gameObject.activeSelf == true)
                    {
                        if (currentGear.gameObject.transform.position == FindData.m_Spawn)
                        {
                            MelonLogger.Msg("Destroy Picked Gear X " + currentGear.transform.position.x + " Y " + currentGear.transform.position.y + " Z " + currentGear.transform.position.z);
                            currentGear.gameObject.SetActive(false);
                        }
                    }
                }
            }
            //float r = Time.time - d;
            //MelonLogger.Msg("[DestoryPickedGears] Time after: " + Time.time + " Result " + r + "ms");
        }

        public static void UpdateDeployedRopes()
        {
            //MelonLogger.Msg("UpdateDeployedRopes called");
            for (int i = 0; i < RopeAnchorPoint.m_RopeAnchorPoints.Count; i++)
            {
                RopeAnchorPoint rope = RopeAnchorPoint.m_RopeAnchorPoints[i];
                ClimbingRopeSync FindData = new ClimbingRopeSync();
                FindData.m_LevelID = levelid;
                FindData.m_LevelGUID = level_guid;
                FindData.m_Position = rope.gameObject.transform.position;
                if (DeployedRopes.Contains(FindData) == true)
                {
                    ClimbingRopeSync FoundData = new ClimbingRopeSync();
                    for (int n = 0; n < DeployedRopes.Count; n++)
                    {
                        ClimbingRopeSync currRope = DeployedRopes[n];
                        if (currRope.m_Position == FindData.m_Position && currRope.m_LevelID == levelid && currRope.m_LevelGUID == level_guid)
                        {
                            FoundData = currRope;
                            break;
                        }
                    }
                    if (rope.gameObject.transform.position == FoundData.m_Position)
                    {
                        MelonLogger.Msg("Processing synced rope " + FoundData.m_Position.x + " Y " + FoundData.m_Position.y + " Z " + FoundData.m_Position.z);
                        if(FoundData.m_Deployed == rope.m_RopeDeployed && FoundData.m_Snapped == rope.m_RopeSnapped)
                        {
                            MelonLogger.Msg("Rope already has desired state");
                        }else{
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
                            }else if(FoundData.m_Deployed == false)
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
                if(ViewModelRadio == null)
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
            
            ViewModelHatchet = GameObject.Find(Path+"mesh_hatchet");
            ViewModelHatchet2 = GameObject.Find(Path + "mesh_Hatchet_improvised");
            ViewModelKnife = GameObject.Find(Path + "mesh_knife");
            ViewModelKnife2 = GameObject.Find(Path + "mesh_knife_improvised");
            ViewModelPrybar = GameObject.Find(Path + "mesh_prybar");
            ViewModelHammer = GameObject.Find(Path + "mesh_hammer");
            ViewModelStone = GameObject.Find(Path + "FPH_Stone(Clone)");

            ViewModelDummy = GameObject.Find(Path+"FPH_Flare(Clone)");

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
            if(ViewModelFireAxe == null)
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

            if(ViewModelBolt == null)
            {
                GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>("Bolt");
                ViewModelBolt = GameObject.Instantiate(LoadedAssets, RadioTransform.transform.position, RadioTransform.transform.rotation, RadioTransform.transform);
                ViewModelBolt.name = "FPH_Bolt";
                ViewModelBolt.transform.localPosition = new Vector3(0.02f, 0.02f, -0.01f);
                ViewModelBolt.transform.localRotation = new Quaternion(0.3304f, 0.9077f, -0.0885f, -0.2432f);
                ViewModelBolt.transform.localScale = new Vector3(0.045f, 0.045f, 0.045f);
                ViewModelBolt.SetActive(false);
            }
        }

        public static void AddDeployedRopes(Vector3 position, bool deployed, bool snapped, int lvl, string lvlguid, bool needSync)
        {
            ClimbingRopeSync rope = new ClimbingRopeSync();
            rope.m_Position = position;
            rope.m_Deployed = deployed;
            rope.m_Snapped = snapped;
            rope.m_LevelID = lvl;
            rope.m_LevelGUID = lvlguid;

            if (DeployedRopes.Contains(rope) == false)
            {
                MelonLogger.Msg("Add Synced rope " + position.x + " Y " + position.y + " Z " + position.z + " LevelGuid " + lvlguid+" Deployed "+ deployed+ " Snapped "+ snapped);
                DeployedRopes.Add(rope);
            }else{
                for (int n = 0; n < DeployedRopes.Count; n++)
                {
                    ClimbingRopeSync currRope = DeployedRopes[n];
                    if(currRope.m_Position == position && currRope.m_LevelID == lvl && currRope.m_LevelGUID == lvlguid)
                    {
                        currRope.m_Deployed = deployed;
                        currRope.m_Snapped = snapped;
                        MelonLogger.Msg("Updated Synced rope " + position.x + " Y " + position.y + " Z " + position.z + " LevelGuid " + lvlguid + " Deployed " + deployed + " Snapped " + snapped);
                        break;
                    }
                }
            }
            if (needSync == true)
            {
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.ROPE))
                    {
                        _packet.Write(rope);
                        SendTCPData(_packet);
                    }
                }

                if (iAmHost == true)
                {
                    ServerSend.ROPE(0, rope, true);
                }
            }
            else
            {
                if (lvl == levelid && lvlguid == level_guid)
                {
                    UpdateDeployedRopes();
                }
            }
        }

        public static void AddRecentlyPickedGear(PickedGearSync data)
        {
            data.m_Recently = 3;
            RecentlyPickedGears.Add(data);
        }

        public static void RemoveHarvastedPlants()
        {
            //MelonLogger.Msg("RemoveHarvastedPlants called");
            if (HarvestableManager.m_Harvestables.Count == 0 || HarvestedPlants.Count == 0)
            {
                return;
            }

            for (int i = 0; i < HarvestableManager.m_Harvestables.Count; i++)
            {
                Harvestable currentPlant = HarvestableManager.m_Harvestables[i];
                string harvGUID = "";

                if(currentPlant.gameObject != null && currentPlant.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    harvGUID = currentPlant.gameObject.GetComponent<ObjectGuid>().Get();
                }

                if (HarvestedPlants.Contains(harvGUID) == true)
                {
                    if(currentPlant.m_DestroyObjectOnHarvest == true)
                    {
                        UnityEngine.Object.Destroy(currentPlant.gameObject);
                    }else{
                        currentPlant.gameObject.SetActive(false);
                        if (currentPlant.m_ActivateObjectPostHarvest != null)
                        {
                            currentPlant.m_ActivateObjectPostHarvest.SetActive(true);
                        }
                    }
                }
            }
        }

        public static void AddHarvastedPlant(string plantGUID, int from)
        {
            if(HarvestedPlants.Contains(plantGUID) == false)
            {
                HarvestedPlants.Add(plantGUID);

                if(iAmHost == true)
                {
                    ServerSend.LOOTEDHARVESTABLE(from, plantGUID, true);
                }
            }

            if (from == instance.myId)
            {
                return;
            }

            if (playersData[from] != null)
            {
                if(playersData[from].m_Levelid == levelid && playersData[from].m_LevelGuid == level_guid)
                {
                    RemoveHarvastedPlants();
                }
            }
        }

        public static void CanclePickOfOtherClient(PickedGearSync data)
        {
            if(data.m_PickerID == 0)
            {
                MelonLogger.Msg(ConsoleColor.Blue, "Other shokal has pickup item before me! I need to delete my picked gear with IID "+ data.m_MyInstanceID);
                int _IID = data.m_MyInstanceID;

                Il2CppSystem.Collections.Generic.List<GearItemObject> invItems = GameManager.GetInventoryComponent().m_Items;
                for (int i = 0; i < invItems.Count; i++)
                {
                    GearItemObject currGear = invItems[i];
                    if (currGear != null)
                    {
                        if (currGear.m_GearItem.m_InstanceID == _IID)
                        {
                            HUDMessage.AddMessage("THIS ALREADY PICKED!");
                            GameAudioManager.PlayGUIError();
                            GameManager.GetInventoryComponent().DestroyGear(currGear.m_GearItem.gameObject);
                            return;
                        }
                    }
                }
            }else{
                ServerSend.CANCLEPICKUP(0, data, true);
            }
        }

        public static void ApplyLootedContainers()
        {
            //MelonLogger.Msg("ApplyLootedContainers called");
            if(ServerConfig.m_DuppedContainers == true)
            {
                return;
            }
            if (ContainerManager.m_Containers.Count == 0 || LootedContainers.Count == 0)
            {
                return;
            }
            for (int i = 0; i < ContainerManager.m_Containers.Count; i++)
            {
                if (ContainerManager.m_Containers[i] != null)
                {
                    Container currentBox = ContainerManager.m_Containers[i];

                    if (currentBox.m_Inspected == false && currentBox.m_SearchInProgress == false)
                    {
                        ContainerOpenSync FindData = new ContainerOpenSync();
                        FindData.m_LevelID = levelid;
                        FindData.m_LevelGUID = level_guid;
                        FindData.m_Guid = "";

                        if (currentBox.gameObject != null && currentBox.gameObject.GetComponent<ObjectGuid>() != null)
                        {
                            FindData.m_Guid = currentBox.gameObject.GetComponent<ObjectGuid>().Get();
                        }

                        if (LootedContainers.Contains(FindData) == true)
                        {
                            currentBox.DestroyAllGear();
                            currentBox.m_Inspected = true;
                        }
                    }
                }
            }
        }

        public static void AddLootedContainer(ContainerOpenSync box, bool needSync, int Looter = 0)
        {
            if (LootedContainers.Contains(box) == false)
            {
                MelonLogger.Msg("Added looted container " + box.m_Guid + " Scene " + box.m_LevelGUID);
                LootedContainers.Add(box);
            }

            if (needSync == true)
            {
                if (iAmHost == true)
                {
                    if(Looter == 0)
                    {
                        ServerSend.LOOTEDCONTAINER(0, box, true);
                    }else{
                        ServerSend.LOOTEDCONTAINER(Looter, box, false);
                    }
                }
            }else{
                if (box.m_LevelID == levelid && box.m_LevelGUID == level_guid)
                {
                    ApplyLootedContainers();
                }
            }
        }

        public static void AddPickedGear(Vector3 spawn, int lvl, string lvlguid, int pickerId, int isntID, bool needSync)
        {
            PickedGearSync picked = new PickedGearSync();
            picked.m_Spawn = spawn;
            picked.m_LevelID = lvl;
            picked.m_LevelGUID = lvlguid;
            picked.m_PickerID = pickerId;
            picked.m_MyInstanceID = isntID;

            if (PickedGears.Contains(picked) == false)
            {
                MelonLogger.Msg("Added picked gear from X " + spawn.x+" Y "+spawn.y+" Z "+spawn.z+" LevelGuid "+lvlguid);
                PickedGears.Add(picked);
            }
            
            if (iAmHost == true)
            {
                if (RecentlyPickedGears.Contains(picked) == false)
                {
                    AddRecentlyPickedGear(picked);
                }else{
                    MelonLogger.Msg(ConsoleColor.Blue,"Other shokal has pickup item later! Picker ID "+ picked.m_PickerID+" Should delete gear with IID "+picked.m_MyInstanceID);
                    CanclePickOfOtherClient(picked);
                }
            }
            if (needSync == true)
            {
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.GEARPICKUP))
                    {
                        _packet.Write(picked);
                        SendTCPData(_packet);
                    }
                }

                if (iAmHost == true)
                {
                    ServerSend.GEARPICKUP(0, picked, true);
                }
            }else{
                if (lvl == levelid && lvlguid == level_guid)
                {
                    DestoryPickedGears();
                }
            }
        }

        public static void OnFurnitureDestroyed(string guid, string parentguid, int lvl, string lvlguid, bool needSync)
        {
            BrokenFurnitureSync furn = new BrokenFurnitureSync();
            furn.m_Guid = guid;
            furn.m_ParentGuid = parentguid;
            furn.m_LevelID = lvl;
            furn.m_LevelGUID = lvlguid;
           
            if (BrokenFurniture.Contains(furn) == false)
            {
                MelonLogger.Msg("Added breakdown to list "+ guid);
                MelonLogger.Msg("BreakDown GUID " + guid + " Parent GUID " + parentguid + " Scene ID " + lvl + " Scene GUID " + lvlguid);
                BrokenFurniture.Add(furn);
            }

            if(InOnline() == false)
            {
                return;
            }

            if(lvl == levelid && lvlguid == level_guid)
            {
                DestoryBrokenFurniture();
            }

            if(needSync == true)
            {
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.FURNBROKEN))
                    {
                        _packet.Write(furn);
                        SendTCPData(_packet);
                    }
                }

                if (iAmHost == true)
                {
                    ServerSend.FURNBROKEN(0, furn, true);
                }
            }
        }

        public static List<GameObject> SpawnedByOtherShelters = new List<GameObject>();

        public static void SpawnSnowShelterByOther(ShowShelterByOther shelterData)
        {
            for (int i = 0; i < SpawnedByOtherShelters.Count; i++)
            {
                GameObject checking = SpawnedByOtherShelters[i];
                if (checking != null)
                {
                    if(checking.transform.position == shelterData.m_Position)
                    {
                        return;
                    }
                }
            }
            GameObject shelter = UnityEngine.Object.Instantiate<GameObject>(GameManager.GetSnowShelterManager().m_SnowShelterPrefab.gameObject);
            shelter.name = GameManager.GetSnowShelterManager().m_SnowShelterPrefab.name;
            shelter.transform.position = shelterData.m_Position;
            shelter.transform.rotation = shelterData.m_Rotation;
            shelter.AddComponent<DoNotSerializeThis>();
            SpawnedByOtherShelters.Add(shelter);
        }

        public static void RemoveSnowShelterByOther(ShowShelterByOther shelterData)
        {
            for (int i = 0; i < SpawnedByOtherShelters.Count; i++)
            {
                GameObject shelter = SpawnedByOtherShelters[i];
                if (shelter != null)
                {
                    if (shelter.transform.position == shelterData.m_Position)
                    {
                        SpawnedByOtherShelters.Remove(shelter);
                        UnityEngine.Object.DestroyImmediate(shelter);
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
                        UnityEngine.Object.DestroyImmediate(shelter);
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
                ShowShelterByOther shelter = ShowSheltersBuilded[i];
                if (shelter != null)
                {
                    SpawnSnowShelterByOther(shelter);
                }
            }
        }

        public static void ShelterCreated(Vector3 spawn, Quaternion rot, int lvl, string lvlguid, bool needSync)
        {
            ShowShelterByOther shelter = new ShowShelterByOther();
            shelter.m_Position = spawn;
            shelter.m_Rotation = rot;
            shelter.m_LevelID = lvl;
            shelter.m_LevelGUID = lvlguid;

            if (ShowSheltersBuilded.Contains(shelter) == false)
            {
                MelonLogger.Msg("Added shelter X " + spawn.x + " Y " + spawn.y + " Z " + spawn.z + " LevelGuid " + lvlguid);
                ShowSheltersBuilded.Add(shelter);
            }
            if (needSync == true)
            {
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.ADDSHELTER))
                    {
                        _packet.Write(shelter);
                        SendTCPData(_packet);
                    }
                }

                if (iAmHost == true)
                {
                    ServerSend.ADDSHELTER(0, shelter, true);
                }
            }else{
                if (lvl == levelid && lvlguid == level_guid)
                {
                    SpawnSnowShelterByOther(shelter);
                }
            }
        }

        public static void ShelterRemoved(Vector3 spawn, int lvl, string lvlguid, bool needSync)
        {
            ShowShelterByOther shelter = new ShowShelterByOther();
            shelter.m_Position = spawn;
            shelter.m_LevelID = lvl;
            shelter.m_LevelGUID = lvlguid;

            if (ShowSheltersBuilded.Contains(shelter) == true)
            {
                MelonLogger.Msg("Remove shelter X " + spawn.x + " Y " + spawn.y + " Z " + spawn.z + " LevelGuid " + lvlguid);
                ShowSheltersBuilded.Remove(shelter);
            }
            if (needSync == true)
            {
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.REMOVESHELTER))
                    {
                        _packet.Write(shelter);
                        SendTCPData(_packet);
                    }
                }

                if (iAmHost == true)
                {
                    ServerSend.REMOVESHELTER(0, shelter, true);
                }
            }else{
                if (lvl == levelid && lvlguid == level_guid)
                {
                    RemoveSnowShelterByOther(shelter);
                }
            }
        }
        public class DroppedGearDummy : MonoBehaviour
        {
            public DroppedGearDummy(IntPtr ptr) : base(ptr) { }

            public string m_LocalizedDisplayName;
            public int m_SearchKey = 0;
            public ExtraDataForDroppedGear m_Extra = new ExtraDataForDroppedGear();
            void Update()
            {
            }
        }
        public class IgnoreDropOverride : MonoBehaviour
        {
            public IgnoreDropOverride(IntPtr ptr) : base(ptr) { }
            void Update()
            {

            }
        }
        public class DropFakeOnLeave : MonoBehaviour
        {
            public DropFakeOnLeave(IntPtr ptr) : base(ptr) { }
            public Vector3 m_OldPossition = new Vector3(0, 0, 0);
            public Quaternion m_OldRotation = new Quaternion(0, 0, 0, 0);

            void Update()
            {

            }
        }
        public class FakeBed : MonoBehaviour
        {
            public FakeBed(IntPtr ptr) : base(ptr) { }

            void Update()
            {

            }
            public void ProcessPickup()
            {

            }
        }
        public class FakeBedDummy : MonoBehaviour
        {
            public FakeBedDummy(IntPtr ptr) : base(ptr) { }
            public GameObject m_LinkedFakeObject;

            void Update()
            {

            }
        }
        

        public class DestoryArrowOnHit : MonoBehaviour
        {
            public DestoryArrowOnHit(IntPtr ptr) : base(ptr) { }

            void Update()
            {

            }
        }
        public class DestoryStoneOnStop : MonoBehaviour
        {
            public DestoryStoneOnStop(IntPtr ptr) : base(ptr) { }
            public GameObject m_Obj = null;
            public Rigidbody m_RB = null;

            void Update()
            {
                if(m_Obj != null && m_RB != null)
                {
                    if(m_RB.isKinematic == true)
                    {
                        UnityEngine.Object.Destroy(m_Obj);
                    }
                }
            }
        }

        public class MultiplayerPlayerVoiceChatPlayer : MonoBehaviour
        {
            public MultiplayerPlayerVoiceChatPlayer(IntPtr ptr) : base(ptr) { }
            public AudioSource aSource;
            public AudioSource aSourceBgNoise;
            public List<VoiceChatQueueElement> VoiceQueue = new List<VoiceChatQueueElement>();
            public int m_ID = 0;
            public bool m_RadioFilter = false;
            public static float PlayNextIn = 0;
            public static float Trim = 0.03f;
            public bool PlayBgNoise = false;
            public bool IsLastClip = false;
            void Update()
            {
                if (aSource != null)
                {
                    if(Time.time > PlayNextIn)
                    {
                        if (IsLastClip)
                        {
                            IsLastClip = false;
                            PlayBgNoise = false;
                        }
                        if (VoiceQueue.Count > 0)
                        {
                            PlayBgNoise = true;
                            VoiceChatQueueElement Voice = VoiceQueue[0];
                            PlayNextIn = Time.time + Voice.m_Length;
                            PlayVoiceFromPlayerObject(aSource, Voice.m_VoiceData, m_RadioFilter);
                            VoiceQueue.RemoveAt(0);
                            if(VoiceQueue.Count == 0)
                            {
                                IsLastClip = true;
                            }
                        }
                    }
                }
                if (aSourceBgNoise != null)
                {
                    aSourceBgNoise.enabled = PlayBgNoise;
                }
            }
        }

        public class MultiplayerPlayerAnimator : MonoBehaviour
        {
            public MultiplayerPlayerAnimator(IntPtr ptr) : base(ptr) { }
            public Animator m_Animer = null;
            public string m_AnimStateHands = "No";
            public string m_PreAnimStateHands = "";
            public string m_AnimStateFingers = "No";
            public bool m_IsDrink = false;
            public bool m_MyDoll = false;

            void Update()
            {
                if (m_Animer != null)
                {
                    GameObject m_Player = m_Animer.gameObject;
                    int m_ID = instance.myId;
                    string HoldingItem;
                    string m_AnimState;
                    if (!m_MyDoll)
                    {
                        m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                        if (playersData[m_ID] != null && playersData[m_ID].m_Levelid != levelid)
                        {
                            return;
                        }
                        m_AnimState = playersData[m_ID].m_AnimState;
                        HoldingItem = m_Player.GetComponent<MultiplayerPlayer>().m_HoldingItem;
                    }else{
                        m_AnimState = MyAnimState;
                        HoldingItem = MyLightSourceName;
                    }
                    int currentTagHash = m_Animer.GetCurrentAnimatorStateInfo(0).tagHash; // This what tag is now
                    int neededTagHash = Animator.StringToHash(m_AnimState); // This is what tag we need.
                    m_Animer.cullingMode = AnimatorCullingMode.AlwaysAnimate;

                    if (m_AnimState == "Fight")
                    {
                        neededTagHash = Animator.StringToHash("Knock");
                    }

                    // MAIN LAYER
                    if (currentTagHash != neededTagHash || (currentTagHash == Animator.StringToHash("Idle") && HoldingItem == "Book"))
                    {
                        if (m_AnimState == "Walk")
                        {
                            m_Animer.Play("Walk", 0);
                        }
                        if (m_AnimState == "Idle")
                        {
                            //string current_anim = m_Animer.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                            //MelonLogger.Msg("Ctrl current animation " + current_anim);
                            if (currentTagHash == Animator.StringToHash("Harvesting"))
                            {
                                m_Animer.Play("StopHarvesting", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("HarvestingStanding"))
                            {
                                m_Animer.Play("StopHarvestingStanding", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("Igniting"))
                            {
                                m_Animer.Play("StopIgniting", 0);
                                m_Player.GetComponent<MultiplayerPlayer>().UpdateHeldItem();
                            }
                            else if (currentTagHash == Animator.StringToHash("Ctrl"))
                            {
                                m_Animer.Play("Sit_To_Idle", 0);
                            }else{ 
                                if(HoldingItem != "Book")
                                {
                                    m_Animer.Play("Idle", 0);
                                }else{
                                    if(currentTagHash != Animator.StringToHash("ReadingBook"))
                                    {
                                        if(currentTagHash == Animator.StringToHash("Ctrl"))
                                        {
                                            m_Animer.Play("StartReadSit", 0);
                                        }else{
                                            m_Animer.Play("StartRead", 0);
                                        }
                                    }
                                }
                            }
                        }
                        if (m_AnimState == "Run")
                        {
                            m_Animer.Play("Run", 0);
                        }
                        if (m_AnimState == "Ctrl")
                        {
                            string current_anim = m_Animer.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                            //MelonLogger.Msg("Ctrl current animation " + current_anim);
                            if (currentTagHash == Animator.StringToHash("Harvesting"))
                            {
                                m_Animer.Play("StopHarvestingSit", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("HarvestingStanding"))
                            {
                                m_Animer.Play("StopHarvestingStandingSit", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("Igniting"))
                            {
                                m_Animer.Play("StopIgnitingSit", 0);
                                m_Player.GetComponent<MultiplayerPlayer>().UpdateHeldItem();
                            }
                            else if (current_anim != "Male Crouch Pose" && current_anim != "Idle_To_Sit")
                            {
                                m_Animer.Play("Idle_To_Sit", 0);
                            }else{
                                m_Animer.Play("Ctrl", 0);
                            }
                        }
                        if (m_AnimState == "Sit")
                        {
                            m_Animer.Play("Sitting Idle", 0);
                        }
                        if (m_AnimState == "Flex")
                        {
                            m_Animer.Play("Samba Dancing", 0);
                        }
                        if (m_AnimState == "Cringe2")
                        {
                            m_Animer.Play("Cringe2", 0);
                        }
                        if (m_AnimState == "Cringe1")
                        {
                            m_Animer.Play("Cringe1", 0);
                        }
                        if (m_AnimState == "Knock" || m_AnimState == "Fight")
                        {
                            m_Animer.Play("Knock", 0);
                        }
                        if (m_AnimState == "Map")
                        {
                            m_Animer.Play("Map", 0);
                        }

                        if (m_AnimState == "Sleep")
                        {
                            m_Animer.Play("Sleep", 0);
                        }
                        if (m_AnimState == "RopeIdle")
                        {
                            string current_anim = m_Animer.GetCurrentAnimatorClipInfo(0)[0].clip.name;

                            if (current_anim == "RopeMove" && current_anim != "RopeStop")
                            {
                                m_Animer.Play("RopeStop", 0);
                            }else{
                                m_Animer.Play("RopeIdle", 0);
                            }
                        }
                        if (m_AnimState == "RopeMoving")
                        {
                            string current_anim = m_Animer.GetCurrentAnimatorClipInfo(0)[0].clip.name;

                            if (current_anim == "RopeIdle" && current_anim != "RopeStart")
                            {
                                m_Animer.Play("RopeStart", 0);
                            }else{
                                m_Animer.Play("RopeMoving", 0);
                            }
                        }
                        if(m_AnimState == "Eating")
                        {
                            if(ServerConfig.m_FastConsumption == false)
                            {
                                m_Animer.Play("Eating", 0);
                            }else{
                                m_Animer.Play("EatingFast", 0);
                            }
                        }
                        if (m_AnimState == "Drinking")
                        {
                            if (ServerConfig.m_FastConsumption == false)
                            {
                                m_Animer.Play("Drinking", 0);
                            }else{
                                m_Animer.Play("DrinkingFast", 0);
                            }
                        }
                        if (m_AnimState == "Harvesting")
                        {
                            if(currentTagHash == Animator.StringToHash("Idle"))
                            {
                                m_Animer.Play("StartHarvesting", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("Ctrl"))
                            {
                                m_Animer.Play("StartHarvestingSit", 0);
                            }else{
                                m_Animer.Play("Harvesting", 0);
                            }
                        }
                        if (m_AnimState == "HarvestingStanding")
                        {
                            if (currentTagHash == Animator.StringToHash("Idle"))
                            {
                                m_Animer.Play("StartHarvestingStanding", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("Ctrl"))
                            {
                                m_Animer.Play("StartHarvestingStandingSit", 0);
                            }else{
                                m_Animer.Play("HarvestingStanding", 0);
                            }
                        }
                        if (m_AnimState == "Igniting")
                        {
                            m_Player.GetComponent<MultiplayerPlayer>().UpdateHeldItem();
                            if (currentTagHash == Animator.StringToHash("Idle"))
                            {
                                m_Animer.Play("StartIgniting", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("Ctrl"))
                            {
                                m_Animer.Play("StartIgnitingSit", 0);
                            }else{
                                m_Animer.Play("StartIgniting", 0);
                            }
                        }
                    }

                    // HANDS LAYER

                    if(m_AnimStateHands == "UnAimGunDone")
                    {
                        m_AnimStateHands = "No";
                    }

                    if(m_AnimState == "Drinking" || m_AnimState == "Eating" || m_AnimState == "Harvesting" || m_AnimState == "HarvestingStanding" || m_AnimState == "Igniting")
                    {
                        m_PreAnimStateHands = "";
                        m_AnimStateHands = "No";
                    }else{
                        if (HoldingItem == "GEAR_Rifle")
                        {
                            if (m_AnimState != "Ctrl")
                            {
                                if (m_AnimStateHands != "Rifle" && m_AnimStateHands != "Rifle_Sit" && m_AnimStateHands != "RifleAim")
                                {
                                    m_PreAnimStateHands = "Pick";
                                }
                                if((m_MyDoll && MyIsAiming) || (!m_MyDoll && playersData[m_ID].m_Aiming))
                                {
                                    m_AnimStateHands = "RifleAim";
                                }else{
                                    m_AnimStateHands = "Rifle";
                                }
                            }else{
                                if (m_AnimStateHands != "Rifle" && m_AnimStateHands != "Rifle_Sit" && m_AnimStateHands != "RifleAim_Sit")
                                {
                                    m_PreAnimStateHands = "Pick_Sit";
                                }
                                if ((m_MyDoll && MyIsAiming) || (!m_MyDoll && playersData[m_ID].m_Aiming))
                                {
                                    m_AnimStateHands = "RifleAim_Sit";
                                }else{
                                    m_AnimStateHands = "Rifle_Sit";
                                }
                            }
                        }
                        else if (HoldingItem.StartsWith("GEAR_Revolver"))
                        {
                            if (m_AnimState == "HoldGun")
                            {
                                if (m_AnimStateHands != "HoldGun" && m_AnimStateHands != "HoldGun_Sit")
                                {
                                    m_PreAnimStateHands = "AimGun";
                                }
                                m_AnimStateHands = "HoldGun";
                            }
                            else if (m_AnimState == "HoldGun_Sit")
                            {
                                if (m_AnimStateHands != "HoldGun" && m_AnimStateHands != "HoldGun_Sit")
                                {
                                    m_PreAnimStateHands = "AimGun_Sit";
                                }
                                m_AnimStateHands = "HoldGun_Sit";
                            }else{
                                if (m_AnimStateHands == "HoldGun")
                                {
                                    m_PreAnimStateHands = "UnAimGun";
                                    m_AnimStateHands = "No";
                                }
                                else
                                {
                                    m_AnimStateHands = "No";
                                }
                            }
                        }
                        else if (HoldingItem.StartsWith("GEAR_KeroseneLamp"))
                        {
                            m_PreAnimStateHands = "";
                            m_AnimStateHands = "HoldLantern";
                        }
                        else if (HoldingItem == "GEAR_Bow")
                        {
                            m_AnimStateHands = "Bow";
                        }else{
                            m_PreAnimStateHands = "";
                            m_AnimStateHands = "No";
                        }
                    }

                    int handsTagHash = m_Animer.GetCurrentAnimatorStateInfo(1).tagHash; // This what tag is now
                    int handsNeededTagHash = Animator.StringToHash(m_AnimStateHands); // This is what tag we need.

                    if (handsTagHash != handsNeededTagHash)
                    {
                        if (m_PreAnimStateHands == "")
                        {                        
                            if(handsTagHash == Animator.StringToHash("Rifle") && m_AnimStateHands == "RifleAim")
                            {
                                m_Animer.Play("StartRifleAim", 1);
                            }else if(handsTagHash == Animator.StringToHash("RifleAim") && m_AnimStateHands == "Rifle")
                            {
                                m_Animer.Play("EndRifleAim", 1);
                            }else if(handsTagHash == Animator.StringToHash("Rifle_Sit") && m_AnimStateHands == "RifleAim_Sit")
                            {
                                m_Animer.Play("StartRifleAim_Sit", 1);
                            }else if(handsTagHash == Animator.StringToHash("RifleAim_Sit") && m_AnimStateHands == "Rifle_Sit")
                            {
                                m_Animer.Play("EndRifleAim_Sit", 1);
                            }else{
                                m_Animer.Play(m_AnimStateHands, 1);
                            }
                        }else{
                            m_Animer.Play(m_PreAnimStateHands, 1);
                            m_PreAnimStateHands = "";
                        }
                    }
                    // FINGERS LAYER

                    if(m_AnimState == "Drinking" || m_AnimState == "Eating" || m_AnimState == "Harvesting" || m_AnimState == "HarvestingStanding" || m_AnimState == "Igniting")
                    {
                        m_AnimStateFingers = "No";
                    }else{
                        if (HoldingItem == "GEAR_Revolver" || HoldingItem == "GEAR_FlareGun")
                        {
                            m_AnimStateFingers = "HoldRevolver";
                        }
                        else if (HoldingItem == "GEAR_Stone" || HoldingItem == "GEAR_FlareA" || HoldingItem == "GEAR_BlueFlare" || HoldingItem == "GEAR_Torch" || HoldingItem == "GEAR_NoiseMaker")
                        {
                            m_AnimStateFingers = "Hold";
                        }
                        else if (HoldingItem.StartsWith("GEAR_SprayPaint"))
                        {
                            m_AnimStateFingers = "HoldCan";
                        }
                        else if (HoldingItem == "GEAR_PackMatches" || HoldingItem == "GEAR_WoodMatches")
                        {
                            m_AnimStateFingers = "HoldMatch";
                        }else{
                            m_AnimStateFingers = "No";
                        }
                    }

                    int fingersTagHash = m_Animer.GetCurrentAnimatorStateInfo(2).tagHash; // This what tag is now
                    int fingersNeededTagHash = Animator.StringToHash(m_AnimStateFingers); // This is what tag we need.

                    if (fingersTagHash != fingersNeededTagHash)
                    {
                        m_Animer.Play(m_AnimStateFingers, 2);
                    }

                    if (!m_MyDoll && (m_AnimState == "Walk" || m_AnimState == "Run"))
                    {
                        GameObject foot_l = m_Player.transform.GetChild(3).GetChild(8).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;
                        GameObject foot_r = m_Player.transform.GetChild(3).GetChild(8).GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject;

                        float main_y = m_Player.transform.position.y; // Y of player object.
                        float leg_y_r = foot_r.transform.position.y;
                        float leg_y_l = foot_l.transform.position.y;
                        float fixed_y_r = main_y - leg_y_r;
                        float fixed_y_l = main_y - leg_y_l;

                        double max = -0.05;
                        double min = -0.057;

                        if (fixed_y_r >= min && fixed_y_r <= max && StepState != 1)
                        {
                            StepState = 1;
                            MaybeLeaveFootPrint(foot_r.transform.position, m_Player, false, 0.0f, false);
                            string ground_Tag = Utils.GetMaterialTagForObjectAtPosition(m_Player, foot_r.transform.position);
                            GameAudioManager.Play3DSound(AK.EVENTS.PLAY_FOOTSTEPSWOLFWALK, m_Player);
                        }
                        if (fixed_y_l >= min && fixed_y_l <= max && StepState != 2)
                        {
                            StepState = 2;
                            MaybeLeaveFootPrint(foot_l.transform.position, m_Player, false, 0.0f, true);
                            string ground_Tag = Utils.GetMaterialTagForObjectAtPosition(m_Player, foot_l.transform.position);
                            GameAudioManager.Play3DSound(AK.EVENTS.PLAY_FOOTSTEPSWOLFWALK, m_Player);
                        }
                    }else{
                        StepState = 0;
                    }
                }
            }
            public void Pickup()
            {
                GameObject m_Player = m_Animer.gameObject;
                int m_ID = instance.myId;
                string m_AnimState = MyAnimState;
                int armTagHash = m_Animer.GetCurrentAnimatorStateInfo(3).tagHash;
                int armNeededTagHash = Animator.StringToHash("Pickup");

                if (!m_MyDoll)
                {
                    m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                    if (playersData[m_ID] == null || playersData[m_ID].m_Levelid != levelid || playersData[m_ID].m_LevelGuid != level_guid)
                    {
                        return;
                    }
                    m_AnimState = playersData[m_ID].m_AnimState;
                }


                //MelonLogger.Msg("Current state of  " + m_ID + " is "+ m_AnimState);
                //MelonLogger.Msg("ArmTagHash  " + armTagHash + " TagHash we need " + armNeededTagHash);
                if (armTagHash != armNeededTagHash)
                {
                    if (m_AnimState != "Ctrl")
                    {
                        m_Animer.Play("DoPickup", 3);
                        //MelonLogger.Msg("Playing DoPickup");
                    }else{
                        m_Animer.Play("DoSitPickup", 3);
                        //MelonLogger.Msg("Playing DoSitPickup");
                    }
                }
            }
            public void MeleeAttack()
            {
                GameObject m_Player = m_Animer.gameObject;
                int m_ID = instance.myId;
                string m_AnimState = MyAnimState;
                if (!m_MyDoll)
                {
                    m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                    if (playersData[m_ID] == null || playersData[m_ID].m_Levelid != levelid || playersData[m_ID].m_LevelGuid != level_guid)
                    {
                        return;
                    }
                    m_AnimState = playersData[m_ID].m_AnimState;
                }

                int armTagHash = m_Animer.GetCurrentAnimatorStateInfo(4).tagHash;
                int armNeededTagHash = Animator.StringToHash("Melee");
                if (armTagHash != armNeededTagHash)
                {
                    m_Animer.Play("Melee", 4);
                }
            }
            public void Consumption()
            {
                GameObject m_Player = m_Animer.gameObject;
                int m_ID = instance.myId;

                if (!m_MyDoll)
                {
                    m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                    if (playersData[m_ID] == null || playersData[m_ID].m_Levelid != levelid || playersData[m_ID].m_LevelGuid != level_guid)
                    {
                        return;
                    }
                }

                if(m_IsDrink == true)
                {
                    playersData[m_ID].m_AnimState = "Drinking";
                    m_Player.GetComponent<MultiplayerPlayer>().m_HoldingFood = "Water";
                }else{
                    playersData[m_ID].m_AnimState = "Eating";
                    m_Player.GetComponent<MultiplayerPlayer>().m_HoldingFood = "Food";
                }

                if (!m_MyDoll)
                {
                    m_Player.GetComponent<MultiplayerPlayer>().UpdateHeldItem();
                }
            }
            public void StopConsumption()
            {
                GameObject m_Player = m_Animer.gameObject;
                int m_ID = instance.myId;

                if (!m_MyDoll)
                {
                    m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                    if (playersData[m_ID] == null || playersData[m_ID].m_Levelid != levelid || playersData[m_ID].m_LevelGuid != level_guid)
                    {
                        return;
                    }
                    if (m_Player.GetComponent<MultiplayerPlayer>().m_HoldingFood != "")
                    {
                        m_Player.GetComponent<MultiplayerPlayer>().m_HoldingFood = "";
                        m_Player.GetComponent<MultiplayerPlayer>().UpdateHeldItem();
                    }
                }
            }
        }

        public static void SaveGearSpawn(GameObject obj)
        {
            if(obj.GetComponent<GearSpawnPointSave>() == null)
            {
                GearSpawnPointSave sv = obj.AddComponent<GearSpawnPointSave>();
                sv.m_Obj = obj;
                sv.SaveMe();
            }
        }

        public class GearSpawnPointSave : MonoBehaviour
        {
            public GearSpawnPointSave(IntPtr ptr) : base(ptr) { }
            public GameObject m_Obj = null;
            public Vector3 m_SpawnPoint = new Vector3(0, 0, 0);

            public void SaveMe()
            {
                if(m_Obj != null)
                {
                    m_SpawnPoint = m_Obj.transform.position;
                }
            }

            void Update()
            {

            }
        }

        public class MultiplayerPlayerClothingManager : MonoBehaviour
        {
            public MultiplayerPlayerClothingManager(IntPtr ptr) : base(ptr) { }

            public string m_Hat = "";
            public string m_Top = "";
            public string m_Bottom = "";
            public string m_Boots = "";
            public string m_Scarf = "";
            public string m_Balaclava = "";
            public int m_DebugB = 0;
            public int m_DebugT = 0;
            public bool m_Debug = false;

            public GameObject m_Player = null;
            public GameObject clothing = null;
            public GameObject arms_middle = null;
            public GameObject arms_short = null;
            public GameObject arms_tiny = null;
            public GameObject arms_long = null;

            void Update()
            {
                if(m_Player != null)
                {
                    bool firstUpdate = false;
                    if(clothing == null)
                    {
                        int clothingChild = 0;
                        clothing = m_Player.transform.GetChild(clothingChild).gameObject;

                        arms_middle = m_Player.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
                        arms_short = m_Player.transform.GetChild(1).GetChild(0).GetChild(1).gameObject;
                        arms_tiny = m_Player.transform.GetChild(1).GetChild(0).GetChild(2).gameObject;
                        arms_long = m_Player.transform.GetChild(1).GetChild(0).GetChild(3).gameObject;
                        firstUpdate = true;
                    }
                    int m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                    PlayerClothingData Cdata = playersData[m_ID].m_PlayerClothingData;
                    if ((Cdata.m_Hat != m_Hat) || (Cdata.m_Top != m_Top) || (Cdata.m_Bottom != m_Bottom) || (Cdata.m_Boots != m_Boots) || (Cdata.m_Scarf != m_Scarf) || (Cdata.m_Balaclava != m_Balaclava) || firstUpdate == true)
                    {
                        m_Hat = Cdata.m_Hat;
                        m_Top = Cdata.m_Top;
                        m_Bottom = Cdata.m_Bottom;
                        m_Boots = Cdata.m_Boots;
                        m_Scarf = Cdata.m_Scarf;
                        m_Balaclava = Cdata.m_Balaclava;

                        //MelonLogger.Msg("[Clothing] Updating model of client " + m_ID);
                        //MelonLogger.Msg("[Clothing] Client Model " + m_ID + " Hat " + m_Hat);
                        //MelonLogger.Msg("[Clothing] Client Model " + m_ID + " Torso " + m_Top);
                        //MelonLogger.Msg("[Clothing] Client Model " + m_ID + " Legs " + m_Bottom);
                        //MelonLogger.Msg("[Clothing] Client Model " + m_ID + " Feets " + m_Boots);
                        UpdateClothing();
                    }
                }
            }
            public void UpdateClothing()
            {
                if (clothing != null)
                {
                    int m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                    int hatsCount = clothing.transform.GetChild(0).childCount;
                    int topsCount = clothing.transform.GetChild(1).childCount;
                    int pantsCount = clothing.transform.GetChild(2).childCount;
                    int bootsCount = clothing.transform.GetChild(3).childCount;
                    int Hat = -1;
                    int Top = 0;
                    int Pants = 0;
                    int Boots = 0;
                    int Scarf = 0;
                    string ArmsType = "";
                    string HairVariant = "Full";
                    bool nakedBodyAtBottom = false;
                    int HeadVariant = playersData[m_ID].m_Character;

                    GameObject HairShort = null;
                    GameObject HairLong = null;

                    if(m_Balaclava != "")
                    {
                        if(HeadVariant == 0)
                        {
                            HeadVariant = 2;
                        }
                        else if (HeadVariant == 1)
                        {
                            HeadVariant = 3;
                        }
                    }

                    for (int i = 0; i < m_Player.transform.GetChild(1).GetChild(1).childCount; i++)
                    {
                        m_Player.transform.GetChild(1).GetChild(1).GetChild(i).gameObject.SetActive(false);
                    }
                    m_Player.transform.GetChild(1).GetChild(1).GetChild(HeadVariant).gameObject.SetActive(true);

                    if (HeadVariant == 0)
                    {
                        HairLong = m_Player.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(2).gameObject;
                        HairShort = m_Player.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(3).gameObject;
                    }

                    for (int i = 0; i < hatsCount; i++)
                    {
                        clothing.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(false);
                    }
                    for (int i = 0; i < topsCount; i++)
                    {
                        clothing.transform.GetChild(1).transform.GetChild(i).gameObject.SetActive(false);
                    }
                    for (int i = 0; i < pantsCount; i++)
                    {
                        clothing.transform.GetChild(2).transform.GetChild(i).gameObject.SetActive(false);
                    }
                    for (int i = 0; i < bootsCount; i++)
                    {
                        clothing.transform.GetChild(3).transform.GetChild(i).gameObject.SetActive(false);
                    }

                    //Hats
                    if (m_Hat == "")
                    {
                        Hat = -1;
                    }
                    else if (m_Hat.Contains("BaseballCap"))
                    {
                        Hat = 0;
                        if (HeadVariant == 0)
                        {
                            HairVariant = "Short";
                        }
                    }
                    else if (m_Hat.Contains("BasicWoolHat") || m_Hat.Contains("GEAR_WillToque"))
                    {
                        Hat = 1;
                    }
                    else if (m_Hat.Contains("Toque"))
                    {
                        Hat = 4;
                    }
                    else if (m_Hat.Contains("RabbitskinHat"))
                    {
                        Hat = 5;
                    }
                    else if (m_Hat.Contains("ImprovisedHat"))
                    {
                        Hat = 6;
                    }
                    else if (m_Hat.Contains("WoolWrapCap"))
                    {
                        Hat = 7;
                        if (HeadVariant == 0) // Is Makenzy
                        {
                            HairVariant = "Short";
                        }
                    }else{
                        Hat = 1;
                    }

                    //Torso
                    if (m_Top == "")
                    {
                        Top = 0;
                        ArmsType = "None";
                    }
                    else if (m_Top.Contains("CottonShirt"))
                    {
                        Top = 1;
                        ArmsType = "Middle";
                    }
                    else if (m_Top.Contains("AstridSweater") || m_Top.Contains("FleeceSweater"))
                    {
                        Top = 2;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("WillSweater"))
                    {
                        Top = 3;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("FishermanSweater"))
                    {
                        Top = 4;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("HeavyWoolSweater"))
                    {
                        Top = 5;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("PlaidShirt") || m_Top.Contains("MackinawJacket"))
                    {
                        Top = 6;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("CottonHoodie"))
                    {
                        Top = 7;
                        ArmsType = "Tiny";
                    }
                    else if (m_Top.Contains("CowichanSweater"))
                    {
                        Top = 8;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("WillShirt"))
                    {
                        Top = 9;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("WoolShirt"))
                    {
                        Top = 10;
                        ArmsType = "Short";
                    }
                    else if ((m_Top.Contains("WoolSweater") && m_Top.Contains("HeavyWoolSweater") == false))
                    {
                        Top = 11;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("DownVest") || m_Top.Contains("InsulatedVest"))
                    {
                        Top = 12;
                        ArmsType = "Long";
                    }
                    else if (m_Top.Contains("LightParka"))
                    {
                        Top = 13;
                        ArmsType = "Tiny";
                    }
                    else if ((m_Top.Contains("SkiJacket") && m_Top.Contains("Down") == false) || m_Top.Contains("PremiumWinterCoat"))
                    {
                        Top = 14;
                        ArmsType = "Tiny";
                    }
                    else if (m_Top.Contains("DownSkiJacket"))
                    {
                        Top = 15;
                        ArmsType = "Tiny";
                    }
                    else if (m_Top.Contains("BearSkinCoat"))
                    {
                        Top = 16;
                        ArmsType = "Tiny";
                    }
                    else if (m_Top.Contains("MooseHideCloak"))
                    {
                        Top = 17;
                        ArmsType = "Tiny";
                    }
                    else if (m_Top.Contains("WolfSkinCape"))
                    {
                        Top = 18;
                        ArmsType = "Tiny";
                    }
                    else // If missing clotching
                    {
                        Top = 7;
                        ArmsType = "Tiny";
                    }
                    arms_middle.SetActive(false);
                    arms_short.SetActive(false);
                    arms_tiny.SetActive(false);
                    arms_long.SetActive(false);

                    if(ArmsType != "None")
                    {
                        if (ArmsType == "Tiny")
                        {
                            arms_tiny.SetActive(true);
                        }
                        else if (ArmsType == "Short")
                        {
                            arms_short.SetActive(true);
                        }
                        else if(ArmsType == "Middle")
                        {
                            arms_middle.SetActive(true);
                        }
                        else if (ArmsType == "Long")
                        {
                            arms_long.SetActive(true);
                        }
                    }

                    //Pants
                    if (m_Bottom == "")
                    {
                        Pants = 0;
                    }
                    else if (m_Bottom.Contains("LongUnderwear"))
                    {
                        if (m_Bottom.Contains("Wool"))
                        {
                            Pants = 2;
                        }else{
                            Pants = 1;
                        }
                    }
                    else if (m_Bottom.Contains("Jeans") || m_Bottom.Contains("GEAR_WillPants"))
                    {
                        Pants = 3;
                    }
                    else if (m_Bottom.Contains("CombatPants"))
                    {
                        Pants = 4;
                    }
                    else if (m_Bottom.Contains("InsulatedPants"))
                    {
                        Pants = 5;
                    }
                    else if (m_Bottom.Contains("CargoPants"))
                    {
                        Pants = 6;
                    }
                    else if (m_Bottom.Contains("WorkPants"))
                    {
                        Pants = 7;
                    }
                    else if (m_Bottom.Contains("DeerSkinPants"))
                    {
                        Pants = 8;
                    }
                    else // If missing clotching
                    {
                        Pants = 3;
                    }

                    //Boots
                    if (m_Boots == "")
                    {
                        Boots = 0;
                    }
                    else if(m_Boots.Contains("LeatherShoes"))
                    {
                        Boots = 1;
                    }
                    else if (m_Boots.Contains("CombatBoots"))
                    {
                        Boots = 2;
                    }
                    else if (m_Boots.Contains("WorkBoots") || m_Boots.Contains("GEAR_BasicBoots") || m_Boots.Contains("GreyMotherBoots") || m_Boots.Contains("WillBoots") || m_Boots.Contains("AstridBoots") || m_Boots.Contains("MuklukBoots") || m_Boots.Contains("DeerSkinBoots"))
                    {
                        Boots = 3;
                    }
                    else if (m_Boots.Contains("InsulatedBoots"))
                    {
                        Boots = 4;
                    }
                    else if (m_Boots.Contains("CottonSocks"))
                    {
                        Boots = 5;
                    }
                    else if (m_Boots.Contains("WoolSocks"))
                    {
                        Boots = 6;
                    }
                    else if (m_Boots.Contains("ClimbingSocks"))
                    {
                        Boots = 7;
                    }
                    else if (m_Boots.Contains("BasicShoes"))
                    {
                        Boots = 8;
                    }
                    else // If missing clotching
                    {
                        Boots = 3;
                    }

                    if (m_Debug == true)
                    {
                        Top = m_DebugT;
                        Pants = m_DebugB;
                        arms_tiny.SetActive(true);
                    }

                    if (m_Scarf != "")
                    {
                        if(Top == 0)
                        {
                            Scarf = 2;
                        }else{
                            Scarf = 3;
                        }
                    }else{
                        Scarf = -1;
                    }

                    if (Hat != -1)
                    {
                        clothing.transform.GetChild(0).transform.GetChild(Hat).gameObject.SetActive(true);
                    }
                    if(Scarf != -1)
                    {
                        clothing.transform.GetChild(0).transform.GetChild(Scarf).gameObject.SetActive(true);
                    }


                    if(nakedBodyAtBottom == true)
                    {
                        clothing.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
                    }

                    if (HairShort != null && HairLong != null)
                    {
                        if (HairVariant == "Full")
                        {
                            HairShort.SetActive(false);
                            HairLong.SetActive(true);
                        }else if(HairVariant == "Short") {
                            HairShort.SetActive(true);
                            HairLong.SetActive(false);
                        }
                    }

                    clothing.transform.GetChild(1).transform.GetChild(Top).gameObject.SetActive(true);
                    clothing.transform.GetChild(2).transform.GetChild(Pants).gameObject.SetActive(true);
                    clothing.transform.GetChild(3).transform.GetChild(Boots).gameObject.SetActive(true);
                }
            }
        }

        public static void LongActionCancleCauseMoved(MultiplayerPlayer mP)
        {
            if(mP.m_InteractTimer < 2 && mP.m_IsBeingInteractedWith == true && PlayerInteractionWith == mP && GetActionForOtherPlayer(mP.m_ActionType).m_CancleOnMove == true)
            {
                if (MyMod.playersData[mP.m_ID] != null)
                {
                    HUDMessage.AddMessage(MyMod.playersData[mP.m_ID].m_Name + " MOVED, ACTION CANCELED");
                }
                LongActionCanceled(mP);
            }
        }
        public static void DoLongAction(MultiplayerPlayer mP, string actionString, string actionType)
        {
            mP.m_ActionType = actionType;
            mP.m_IsBeingInteractedWith = true;
            mP.m_InteractTimer = GetActionForOtherPlayer(mP.m_ActionType).m_ActionDuration;
            PlayerInteractionWith = mP;
            PreviousControlModeBeforeAction = GameManager.GetPlayerManagerComponent().GetControlMode();
            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);
            InterfaceManager.m_Panel_HUD.StartItemProgressBar(mP.m_InteractTimer, actionString, (GearItem)null, new System.Action(EmptyFn));

            if(actionType != "Revive")
            {
                if (sendMyPosition == true) // CLIENT
                {
                    using (Packet _packet = new Packet((int)ClientPackets.DONTMOVEWARNING))
                    {
                        _packet.Write(true);
                        _packet.Write(mP.m_ID);
                        SendTCPData(_packet);
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

        public static void LongActionCanceled(MultiplayerPlayer mP)
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
        public static void LongActionFinished(MultiplayerPlayer mP, string ActionType)
        {
            if (sendMyPosition == true) // CLIENT
            {
                using (Packet _packet = new Packet((int)ClientPackets.APPLYACTIONONPLAYER))
                {
                    _packet.Write(ActionType);
                    _packet.Write(mP.m_ID);
                    SendTCPData(_packet);
                }
            }
            if (iAmHost == true) // HOST
            {
                using (Packet _packet = new Packet((int)ServerPackets.APPLYACTIONONPLAYER))
                {
                    ServerSend.APPLYACTIONONPLAYER(0, ActionType, false, mP.m_ID);
                }
            }

            if(ActionType == "Bandage")
            {
                GameManager.GetInventoryComponent().RemoveGearFromInventory("GEAR_HeavyBandage", 1);
            }
            if (ActionType == "Sterilize")
            {
                GearItem antiseptic = GameManager.GetInventoryComponent().GetBestGearItemWithName("GEAR_BottleHydrogenPeroxide");
                antiseptic.m_LiquidItem.m_LiquidLiters = antiseptic.m_LiquidItem.m_LiquidLiters - 0.1f;

                if(antiseptic.m_LiquidItem.m_LiquidLiters < 0.1f)
                {
                    GameManager.GetInventoryComponent().DestroyGear(antiseptic.gameObject);
                }
            }
            else if (ActionType == "Revive")
            {
                GameManager.GetInventoryComponent().RemoveGearFromInventory("GEAR_MedicalSupplies_hangar", 1);
            }
            else if(ActionType == "Stim")
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
            else if(ActionType == "Borrow")
            {
                TryToBorrowThing(mP.m_ID);
            }
        }

        public static void OtherPlayerApplyActionOnMe(string ActionType, int FromId)
        {
            if(ActionType == "Bandage")
            {
                BloodLoss bloodL = GameManager.GetBloodLossComponent();

                if(bloodL.m_ElapsedHoursList.Count > 0)
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
            else if(ActionType == "Revive")
            {
                SimRevive();
            }
            else if(ActionType == "Stim")
            {
                GearItem EmergencySt = GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_EmergencyStim", 1);
                EmergencySt.m_EmergencyStim.OnInject();
            }
        }

        public static void EmptyFn()
        {
            
        }

        public static void TogglePlayerInstance(GameObject plObj, bool enable)
        {
            if (plObj != null)
            {
                plObj.SetActive(enable);
            }
        }

        public static void PlayersUpdateManager()
        {
            if (playersData.Count > 0)
            {
                for (int i = 0; i < MaxPlayers; i++)
                {
                    if (playersData[i] != null)
                    {
                        if(players[i] != null)
                        {
                            GameObject plObj = players[i];
                            MultiPlayerClientData plDat = playersData[i];
                            if (levelid == plDat.m_Levelid && level_guid == plDat.m_LevelGuid) // Player locates on same scene with you.
                            {
                                TogglePlayerInstance(plObj, true);
                            }else{
                                TogglePlayerInstance(plObj, false);
                            }
                        }
                    }
                }
            }
        }

        public static void MakeBorrowble(GameObject obj, string GearName, int ID, MultiplayerPlayer mP)
        {
            Borrowable Bor = obj.AddComponent<Borrowable>();
            Bor.m_PlayerID = ID;
            Bor.m_mP = mP;
            Bor.m_GearName = GearName;
            Bor.m_Obj = obj;
            obj.layer = vp_Layer.Gear;
        }

        public class MultiplayerPlayer : MonoBehaviour
        {
            public MultiplayerPlayer(IntPtr ptr) : base(ptr) { }

            public int m_ID = 0;
            public List<GameObject> m_DamageColiders = new List<GameObject>();

            //Equipment
            public string m_HoldingItem = "";
            public string m_HoldingFood = "";
            public bool m_LightSourceOn = false;
            public bool m_HasAxe = false;
            public bool m_HasRifle = false;
            public bool m_HasRevolver = false;
            public bool m_HasMedkit = false;
            public int m_Arrows = 0;
            public int m_Flares = 0;
            public int m_BlueFlares = 0;
            public GameObject m_Player = null;
            public string m_BreakingSound = "";
            public uint m_BreakingSoundReference = 0U;
            public int m_BloodLosts = 0;
            public bool m_NeedAntiseptic = false;
            public uint m_HeavyBreathSoundReference = 0U;
            public int m_Character = -1;
            public GameObject m_TorchIgniter = null;

            //Shortcuts for optimized  
            public GameObject hand_r = null;
            public GameObject hand_l = null;
            public GameObject rifle = null;
            public GameObject revolver = null;
            public GameObject quiver = null;
            public GameObject axe = null;
            public GameObject medkit = null;
            public GameObject flares = null;
            public GameObject body = null;
            public GameObject clothing = null;
            public GameObject hip = null;
            public GameObject root = null;
            public GameObject extras = null;
            public GameObject MakenzyHead = null;
            public GameObject AstridHead = null;
            public GameObject bookOpen = null;
            public GameObject bookClosed = null;
            public GameObject meleeDummy = null;

            //Timers
            public float nextActionBloodDrop = 0.0f;
            public float blooddrop_period = 1.3f;

            //Actions
            public bool m_IsBeingInteractedWith = false;
            public float m_InteractTimer = 0.0f;
            public string m_ActionType = "";

            void Start()
            {
                nextActionBloodDrop = Time.time;
            }

            public void UpdateVisualItems()
            {
                if (m_HasRifle == true && m_HoldingItem != "GEAR_Rifle")
                {
                    rifle.SetActive(true);
                }else{
                    rifle.SetActive(false);
                }
                if (m_HasRevolver == true && m_HoldingItem != "GEAR_Revolver")
                {
                    revolver.SetActive(true);
                }else{
                    revolver.SetActive(false);
                }

                extras.transform.GetChild(2).gameObject.SetActive(m_HasRevolver);
                axe.SetActive(m_HasAxe);
                medkit.SetActive(m_HasMedkit);

                if (m_Arrows > 0)
                {
                    quiver.SetActive(true);
                    quiver.transform.GetChild(0).gameObject.SetActive(false);
                    quiver.transform.GetChild(1).gameObject.SetActive(false);
                    quiver.transform.GetChild(2).gameObject.SetActive(false);
                    quiver.transform.GetChild(3).gameObject.SetActive(false);
                    quiver.transform.GetChild(4).gameObject.SetActive(false);
                    for (int i = 0; i < 4; i++)
                    {
                        quiver.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
                else
                {
                    quiver.SetActive(false);
                }

                if(m_Flares == 0 && m_BlueFlares == 0)
                {
                    flares.SetActive(false);
                }else{
                    flares.SetActive(true);
                    GameObject redFlares = flares.transform.GetChild(1).gameObject;
                    GameObject blueFlares = flares.transform.GetChild(2).gameObject;

                    int HaveR = m_Flares;
                    int HaveB = m_BlueFlares;

                    //If we hold flare remove 1 from how many do we have.
                    if(m_HoldingItem == "GEAR_FlareA")
                    {
                        HaveR = HaveR - 1;
                    }
                    if(m_HoldingItem == "GEAR_BlueFlare")
                    {
                        HaveB = HaveB - 1;
                    }

                    if(HaveR == 0)
                    {
                        redFlares.SetActive(false);
                    }else{
                        redFlares.SetActive(true);
                    }
                    if (HaveB == 0)
                    {
                        blueFlares.SetActive(false);
                    }else{
                        blueFlares.SetActive(true);
                    }

                    redFlares.transform.GetChild(0).gameObject.SetActive(false);
                    redFlares.transform.GetChild(1).gameObject.SetActive(false);
                    blueFlares.transform.GetChild(0).gameObject.SetActive(false);
                    blueFlares.transform.GetChild(1).gameObject.SetActive(false);

                    if (HaveR > 0 && HaveB > 0)
                    {
                        redFlares.transform.GetChild(0).gameObject.SetActive(true);
                        blueFlares.transform.GetChild(1).gameObject.SetActive(true);
                    }else{
                        if(HaveR > 0)
                        {
                            if(HaveR == 1)
                            {
                                redFlares.transform.GetChild(0).gameObject.SetActive(true);
                            }
                            else if (HaveR > 1)
                            {
                                redFlares.transform.GetChild(0).gameObject.SetActive(true);
                                redFlares.transform.GetChild(1).gameObject.SetActive(true);
                            }
                        }
                        if (HaveB > 0)
                        {
                            if (HaveB == 1)
                            {
                                blueFlares.transform.GetChild(0).gameObject.SetActive(true);
                            }
                            else if (HaveB > 1)
                            {
                                blueFlares.transform.GetChild(0).gameObject.SetActive(true);
                                blueFlares.transform.GetChild(1).gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }

            public void SetupMapMarker()
            {
                int Markers = hand_l.transform.GetChild(2).GetChild(0).childCount;
                Transform MakersRoot = hand_l.transform.GetChild(2).GetChild(0);
                for (int i = 0; i < Markers; i++)
                {
                    MakersRoot.GetChild(i).gameObject.SetActive(false);
                }
                GameRegion Reg = RegionManager.GetCurrentRegion();
                if(Reg == GameRegion.LakeRegion)
                {
                    MakersRoot.GetChild(0).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.RuralRegion)
                {
                    MakersRoot.GetChild(1).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.RiverValleyRegion)
                {
                    MakersRoot.GetChild(3).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.CoastalRegion)
                {
                    MakersRoot.GetChild(4).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.WhalingStationRegion)
                {
                    MakersRoot.GetChild(6).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.TracksRegion)
                {
                    MakersRoot.GetChild(7).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.MarshRegion)
                {
                    MakersRoot.GetChild(8).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.MountainTownRegion)
                {
                    MakersRoot.GetChild(9).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.MarshRegion)
                {
                    MakersRoot.GetChild(10).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.CrashMountainRegion)
                {
                    MakersRoot.GetChild(11).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.AshCanyonRegion)
                {
                    MakersRoot.GetChild(12).gameObject.SetActive(true);
                }
            }

            public void UpdateHeldItem()
            {
                //Flare
                hand_r.transform.GetChild(0).gameObject.SetActive(false);
                //Matches
                hand_r.transform.GetChild(1).gameObject.SetActive(false); hand_l.transform.GetChild(0).gameObject.SetActive(false);
                //Flare blue
                hand_r.transform.GetChild(2).gameObject.SetActive(false);
                //Rifle
                hand_r.transform.GetChild(3).gameObject.SetActive(false);
                //Revolver
                hand_r.transform.GetChild(4).gameObject.SetActive(false);
                //SprayCan
                hand_r.transform.GetChild(5).gameObject.SetActive(false);
                //Stone 
                hand_r.transform.GetChild(6).gameObject.SetActive(false);
                // Lamp
                hand_r.transform.GetChild(7).gameObject.SetActive(false);
                //Tourch
                hand_r.transform.GetChild(8).gameObject.SetActive(false);
                //Flaregun
                hand_r.transform.GetChild(9).gameObject.SetActive(false);
                //NoiseMaker
                hand_r.transform.GetChild(10).gameObject.SetActive(false);
                //Melee Dummy
                //hand_r.transform.GetChild(11).gameObject.SetActive(false);
                //Map
                hand_l.transform.GetChild(2).gameObject.SetActive(false);
                //Bow
                hand_l.transform.GetChild(3).gameObject.SetActive(false);
                //Bottle
                hand_l.transform.GetChild(4).gameObject.SetActive(false);
                //Food can
                hand_l.transform.GetChild(5).gameObject.SetActive(false);

                string m_AnimState = playersData[m_ID].m_AnimState;

                if (m_HoldingFood == "" && m_AnimState != "Igniting")
                {
                    if (m_HoldingItem.StartsWith("GEAR_Flare") != m_HoldingItem.Contains("Gun")) // By some reason, it called GEAR_FlareA, i not know exist FlareB but better check it.
                    {
                        hand_r.transform.GetChild(0).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_WoodMatches" || m_HoldingItem == "GEAR_PackMatches")
                    {
                        hand_r.transform.GetChild(1).gameObject.SetActive(true); hand_l.transform.GetChild(0).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_BlueFlare")
                    {
                        hand_r.transform.GetChild(2).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_Rifle")
                    {
                        hand_r.transform.GetChild(3).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_Revolver")
                    {
                        hand_r.transform.GetChild(4).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem.StartsWith("GEAR_SprayPaintCan")) // By some reason, it called GEAR_SprayPaintCanGlyphA, i not know exist GEAR_SprayPaintCanGlyphB but better check it.
                    {
                        hand_r.transform.GetChild(5).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_Stone")
                    {
                        hand_r.transform.GetChild(6).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem.StartsWith("GEAR_KeroseneLamp"))
                    {
                        hand_r.transform.GetChild(7).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_Torch")
                    {
                        hand_r.transform.GetChild(8).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_Bow")
                    {
                        hand_l.transform.GetChild(3).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "Map")
                    {
                        hand_l.transform.GetChild(2).gameObject.SetActive(true);
                        //SetupMapMarker();
                    }
                    if (m_HoldingItem == "GEAR_FlareGun")
                    {
                        hand_r.transform.GetChild(9).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_NoiseMaker")
                    {
                        hand_r.transform.GetChild(10).gameObject.SetActive(true);
                    }
                }else{
                    
                    if(m_HoldingFood != "")
                    {
                        if (m_HoldingFood == "Water")
                        {
                            hand_l.transform.GetChild(4).gameObject.SetActive(true);
                        }else{
                            hand_l.transform.GetChild(5).gameObject.SetActive(true);
                        }
                    }
                    if(m_AnimState == "Igniting")
                    {
                        hand_r.transform.GetChild(1).gameObject.SetActive(true); hand_l.transform.GetChild(0).gameObject.SetActive(true);
                    }
                }

                //Additionals
                if (m_LightSourceOn == true)
                {
                    hand_r.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); //Flare cap
                    hand_r.transform.GetChild(0).GetChild(2).gameObject.SetActive(true); //Flare fx
                    hand_r.transform.GetChild(1).GetChild(0).gameObject.SetActive(false); // Matche cap normal
                    hand_r.transform.GetChild(1).GetChild(1).gameObject.SetActive(true); // Matche cap burt
                    hand_r.transform.GetChild(1).GetChild(3).gameObject.SetActive(true); // Matche fire
                    hand_r.transform.GetChild(2).GetChild(1).gameObject.SetActive(false); //Blue Flare cap
                    hand_r.transform.GetChild(2).GetChild(2).gameObject.SetActive(true); //Blue Flare fx
                    hand_r.transform.GetChild(7).GetChild(0).gameObject.SetActive(true); //Lamp Fire
                    hand_r.transform.GetChild(8).GetChild(1).gameObject.SetActive(true); //Tourch Fire
                }else{
                    hand_r.transform.GetChild(0).GetChild(1).gameObject.SetActive(true); //Flare cap
                    hand_r.transform.GetChild(0).GetChild(2).gameObject.SetActive(false); //Flare fx
                    hand_r.transform.GetChild(1).GetChild(0).gameObject.SetActive(true); // Matche cap normal
                    hand_r.transform.GetChild(1).GetChild(1).gameObject.SetActive(false); // Matche cap burt
                    hand_r.transform.GetChild(1).GetChild(3).gameObject.SetActive(false); // Matche fire
                    hand_r.transform.GetChild(2).GetChild(1).gameObject.SetActive(true); //Blue Flare cap
                    hand_r.transform.GetChild(2).GetChild(2).gameObject.SetActive(false); //Blue Flare fx
                    hand_r.transform.GetChild(7).GetChild(0).gameObject.SetActive(false); //Lamp Fire
                    hand_r.transform.GetChild(8).GetChild(1).gameObject.SetActive(false); //Tourch Fire
                }
                //Lights
                hand_r.transform.GetChild(0).GetChild(3).gameObject.SetActive(m_LightSourceOn); //Flare Red Light
                hand_r.transform.GetChild(1).GetChild(4).gameObject.SetActive(m_LightSourceOn); //Matche Light
                hand_r.transform.GetChild(2).GetChild(3).gameObject.SetActive(m_LightSourceOn); //Flare Blue Light
                hand_r.transform.GetChild(7).GetChild(3).gameObject.SetActive(m_LightSourceOn); //Lamp Light
                hand_r.transform.GetChild(8).GetChild(0).gameObject.SetActive(m_LightSourceOn); //Tourch Light
            }

            void UpdateOtherAffictions()
            {
                MultiPlayerClientData pD = playersData[m_ID];
                m_NeedAntiseptic = pD.m_NeedAntiseptic;
            }

            void BloodLostUpdate()
            {
                MultiPlayerClientData pD = playersData[m_ID];
                m_BloodLosts = pD.m_BloodLosts;

                if (m_BloodLosts <= 1)
                {
                    blooddrop_period = 1.3f;
                }
                else if (m_BloodLosts == 2)
                {
                    blooddrop_period = 1f;
                }
                else if (m_BloodLosts == 3)
                {
                    blooddrop_period = 0.7f;
                }
                else if (m_BloodLosts >= 4)
                {
                    blooddrop_period = 0.5f;
                }

                if (Time.time > nextActionBloodDrop)
                {
                    nextActionBloodDrop += blooddrop_period;
                    int m_LevelId = pD.m_Levelid;
                    string m_LevelGUID = pD.m_LevelGuid;
                    if (m_BloodLosts > 0 && levelid == m_LevelId && level_guid == m_LevelGUID)
                    {
                        PlayMultiplayer3dAduio("PLAY_BLOODDROPS_3D", m_ID);
                        Vector3 pos = root.transform.position;
                        ++pos.y;
                        Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                        insideUnitCircle.Normalize();
                        Vector2 vector2 = insideUnitCircle * UnityEngine.Random.Range(0.0f, 0.75f);
                        pos.x += vector2.x;
                        pos.z += vector2.y;
                        pos -= root.transform.forward * 0.5f;
                        RaycastHit hitInfo;
                        if (!Physics.Raycast(pos, Vector3.down, out hitInfo, float.PositiveInfinity, Utils.m_PhysicalCollisionLayerMask) || (UnityEngine.Object)hitInfo.collider == (UnityEngine.Object)null)
                            return;
                        Vector3 scale = new Vector3(0.05f, 2f, 0.05f) * UnityEngine.Random.Range(0.5f, 2f);
                        int uvRectangleIndex = 7;
                        if (Utils.RollChance(50f))
                            uvRectangleIndex = 6;
                        DecalProjectorInstance decalCreated = GameManager.GetDynamicDecalsManager().CreateDecal(hitInfo.point, m_Player.transform.rotation.eulerAngles.y, hitInfo.normal, uvRectangleIndex, scale, DecalProjectorType.PlayerBlood, GameManager.GetWeatherComponent().IsIndoorEnvironment());
                    }
                }
            }

            void DoneAction()
            {
                LongActionFinished(this, m_ActionType);
                LongActionCanceled(this);
            }

            void UpdateHead()
            {
                MultiPlayerClientData pD = playersData[m_ID];

                if(m_Character != pD.m_Character)
                {
                    MakenzyHead.SetActive(false);
                    AstridHead.SetActive(false);
                    if (pD.m_Character == 0)
                    {
                        MakenzyHead.SetActive(true);
                    }
                    if (pD.m_Character == 1)
                    {
                        AstridHead.SetActive(true);
                    }
                }
            }
            void Update()
            {
                if(m_Player != null)
                {
                    // If hand_r is null, this means all other objects shortcuts null too need write them down.
                    if (hand_r == null)
                    {
                        int clothingChild = 0;
                        int bodyChild = 1;
                        int extrasChild = 2;
                        int hipsChild = 3;
                        int rootChild = 8;
                        clothing = m_Player.transform.GetChild(clothingChild).gameObject;
                        body = m_Player.transform.GetChild(bodyChild).gameObject;
                        extras = m_Player.transform.GetChild(extrasChild).gameObject;
                        hip = m_Player.transform.GetChild(hipsChild).gameObject;
                        root = hip.transform.GetChild(rootChild).gameObject;
                        hand_r = root.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;
                        hand_l = root.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
                        rifle = root.transform.GetChild(0).GetChild(1).gameObject;
                        revolver = root.transform.GetChild(2).GetChild(1).gameObject;
                        quiver = root.transform.GetChild(0).GetChild(2).gameObject;
                        axe = root.transform.GetChild(0).GetChild(3).gameObject;
                        medkit = root.transform.GetChild(0).GetChild(4).gameObject;
                        flares = root.transform.GetChild(0).GetChild(5).gameObject;

                        for (int i = 0; i <= 1; i++)
                        {
                            GameObject Fla = flares.transform.GetChild(1).GetChild(i).gameObject;
                            MakeBorrowble(Fla, "GEAR_FlareA", m_ID, m_Player.GetComponent<MultiplayerPlayer>());
                        }
                        for (int i = 0; i <= 1; i++)
                        {
                            GameObject Fla = flares.transform.GetChild(2).GetChild(i).gameObject;
                            MakeBorrowble(Fla, "GEAR_BlueFlare", m_ID, m_Player.GetComponent<MultiplayerPlayer>());
                        }

                        MakeBorrowble(medkit, "GEAR_MedicalSupplies_hangar", m_ID, m_Player.GetComponent<MultiplayerPlayer>());
                        MakeBorrowble(axe, "GEAR_Hatchet", m_ID, m_Player.GetComponent<MultiplayerPlayer>());
                        MakeBorrowble(rifle, "GEAR_Rifle", m_ID, m_Player.GetComponent<MultiplayerPlayer>());
                        MakeBorrowble(revolver, "GEAR_Revolver", m_ID, m_Player.GetComponent<MultiplayerPlayer>());

                        bookOpen = hand_l.transform.GetChild(6).gameObject;
                        bookClosed = hand_l.transform.GetChild(7).gameObject;
                        meleeDummy = hand_r.transform.GetChild(11).gameObject;

                        MakenzyHead = body.transform.GetChild(1).GetChild(0).gameObject;
                        AstridHead = body.transform.GetChild(1).GetChild(1).gameObject;

                        //GameObject reference = GetGearItemObject("GEAR_PryBar");

                        //if (reference == null)
                        //{
                        //    return;
                        //}
                        //UnityEngine.Object.Instantiate<GameObject>(reference, meleeDummy.transform.position, meleeDummy.transform.rotation, meleeDummy.transform);
                        //meleeDummy.transform.localScale = new Vector3(0.007f, 0.007f, 0.007f);
                    }

                    MultiPlayerClientData pD = playersData[m_ID];

                    BloodLostUpdate();
                    UpdateOtherAffictions();
                    //UpdateHead();

                    if (m_IsBeingInteractedWith == true)
                    {
                        m_InteractTimer -= Time.deltaTime;

                        if(m_InteractTimer <= 0.0)
                        {
                            DoneAction();
                        }
                    }

                    PlayerEquipmentData Edata = pD.m_PlayerEquipmentData;
                    if((Edata.m_HoldingItem != m_HoldingItem) || (Edata.m_LightSourceOn != m_LightSourceOn))
                    {
                        m_HoldingItem = Edata.m_HoldingItem;
                        m_LightSourceOn = Edata.m_LightSourceOn;
                        UpdateHeldItem();
                        UpdateVisualItems();
                    }
                    if((Edata.m_HasAxe != m_HasAxe) || (Edata.m_HasMedkit != m_HasMedkit) || (Edata.m_HasRifle != m_HasRifle) || (Edata.m_HasRevolver != m_HasRevolver) || (Edata.m_Arrows != m_Arrows) || (Edata.m_Flares != m_Flares) || (Edata.m_BlueFlares != m_BlueFlares))
                    {
                        m_HasMedkit = Edata.m_HasMedkit;
                        m_HasRifle = Edata.m_HasRifle;
                        m_HasRevolver = Edata.m_HasRevolver;
                        m_Arrows = Edata.m_Arrows;
                        m_Flares = Edata.m_Flares;
                        m_BlueFlares = Edata.m_BlueFlares;
                        UpdateVisualItems();
                    }
                    Vector3 m_XYZ = pD.m_Position;
                    Quaternion m_XYZW = pD.m_Rotation;
                    if (pD.m_Female == false)
                    {
                        m_XYZ = pD.m_Position;
                        m_Player.transform.localScale = new Vector3(1, 1, 1);
                    }else{
                        m_XYZ = new Vector3(pD.m_Position.x, pD.m_Position.y - 0.194f, pD.m_Position.z);
                        m_Player.transform.localScale = new Vector3(0.867f, 1, 0.867f);
                    }
                    if(m_HoldingItem == "Book")
                    {
                        if(pD.m_AnimState == "Idle")
                        {
                            bookOpen.SetActive(true);
                            bookClosed.SetActive(false);
                        }else{
                            bookOpen.SetActive(false);
                            bookClosed.SetActive(true);
                        }
                    }else{
                        bookOpen.SetActive(false);
                        bookClosed.SetActive(false);
                    }
                    
                    Vector3 togo = Vector3.Lerp(m_Player.transform.position, m_XYZ, Time.deltaTime * 20);
                    Quaternion toRot = Quaternion.Lerp(m_Player.transform.rotation, m_XYZW, Time.deltaTime * 20);
                    m_Player.transform.position = togo;
                    m_Player.transform.rotation = toRot;

                    string BreakingAudioData = pD.m_BrakingSounds;

                    if (m_BreakingSound != BreakingAudioData)
                    {
                        m_BreakingSound = BreakingAudioData;

                        if (m_BreakingSoundReference != 0U)
                        {
                            AkSoundEngine.StopPlayingID(m_BreakingSoundReference, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                            m_BreakingSoundReference = 0U;
                        }

                        if (m_BreakingSound != "")
                        {
                            m_BreakingSoundReference = GameAudioManager.PlaySound(BreakingAudioData, m_Player);
                        }
                    }

                    bool m_HeavyBreath = pD.m_HeavyBreath;

                    if (m_HeavyBreath == true)
                    {
                        if (m_HeavyBreathSoundReference == 0U)
                        {
                            m_HeavyBreathSoundReference = GameAudioManager.PlaySound("PLAY_BREATH_3D", m_Player);
                        }
                    }else{
                        if (m_HeavyBreathSoundReference != 0U)
                        {
                            AkSoundEngine.StopPlayingID(m_HeavyBreathSoundReference, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                            m_HeavyBreathSoundReference = 0U;
                        }
                    }
                }
            }
        }

        public class ClientProjectile : MonoBehaviour
        {
            public ClientProjectile(IntPtr ptr) : base(ptr) { }
            public int m_ClientID = 0;
        }
        public class UiButtonPressHook : MonoBehaviour
        {
            public UiButtonPressHook(IntPtr ptr) : base(ptr) { }
            public int m_CustomId = 0;
        }
        public class UiButtonKeyboardPressSkip : MonoBehaviour
        {
            public UiButtonKeyboardPressSkip(IntPtr ptr) : base(ptr) { }
            public IL2CPP.List<EventDelegate> m_Click;
        }
        public class LeftHandHelper: MonoBehaviour
        {
            public LeftHandHelper(IntPtr ptr) : base(ptr) { }
            public GameObject MirrorThis;
            void LateUpdate()
            {
                if (MirrorThis)
                {
                    MirrorThis.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }
        public class DeathDropContainer : MonoBehaviour
        {
            public DeathDropContainer(IntPtr ptr) : base(ptr) { }
            public string m_Owner = "UNKNOWN";
        }
        

        public class ContainersSync : MonoBehaviour
        {
            public ContainersSync(IntPtr ptr) : base(ptr) { }
            public GameObject m_Obj = null;
            public Container m_Cont = null;
            public string m_Guid = "";
            public string m_LastAnim = "";


            void Update()
            {
                if(m_Obj != null)
                {
                    if(m_Cont == null && m_Obj.GetComponent<Container>() != null)
                    {
                        m_Cont = m_Obj.GetComponent<Container>();
                    }
                    if (m_Guid == "" && m_Obj.GetComponent<ObjectGuid>() != null)
                    {
                        m_Guid = m_Obj.GetComponent<ObjectGuid>().Get();
                    }

                    if(m_Guid != "" && m_Cont != null)
                    {
                        if(m_Cont.m_IsCorpse == true)
                        {
                            UnityEngine.Object.Destroy(m_Obj.GetComponent<ContainersSync>());
                        }
                    }
                }
            }

            public void CallSync()
            {
                ContainerOpenSync sync = new ContainerOpenSync();
                sync.m_Guid = m_Guid;

                if (m_LastAnim == "close")
                {
                    sync.m_State = false;
                }else{
                    sync.m_State = true;
                }

                if (iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.CONTAINEROPEN))
                    {
                        ServerSend.CONTAINEROPEN(0, sync, true);
                    }
                }
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.CONTAINEROPEN))
                    {
                        _packet.Write(sync);
                        SendTCPData(_packet);
                    }
                }
            }

            public void Open()
            {
                if (m_Cont != null && m_Cont.m_ObjectAnims != null && m_LastAnim != "open")
                {
                    if (m_Cont.m_PendingClose)
                        return;
                    for (int index = 0; index < m_Cont.m_ObjectAnims.Length; ++index)
                    {
                        ObjectAnim objectAnim = m_Cont.m_ObjectAnims[index];
                        if ((bool)(UnityEngine.Object)objectAnim && !objectAnim.Play("open"))
                            return;
                    }

                    if(m_Cont.gameObject != null && GameManager.GetPlayerObject() != null)
                    {
                        float dist = Vector3.Distance(GameManager.GetPlayerObject().transform.position, m_Cont.gameObject.transform.position);
                        if(dist < 30)
                        {
                            m_Cont.PlayContainerOpenSound();
                        }
                    }
                }
            }
            public void Close()
            {
                if (m_Cont != null && m_Cont.m_ObjectAnims != null && m_LastAnim != "close")
                {
                    for (int index = 0; index < m_Cont.m_ObjectAnims.Length; ++index)
                    {
                        ObjectAnim objectAnim = m_Cont.m_ObjectAnims[index];
                        if ((bool)(UnityEngine.Object)objectAnim && !objectAnim.Play("close"))
                        {
                            m_Cont.m_PendingClose = true;
                            return;
                        }
                    }
                    if (m_Cont.gameObject != null && GameManager.GetPlayerObject() != null)
                    {
                        float dist = Vector3.Distance(GameManager.GetPlayerObject().transform.position, m_Cont.gameObject.transform.position);
                        if (dist < 30)
                        {
                            m_Cont.PlayContainerCloseSound();
                        }
                    }
                    m_Cont.m_PendingClose = false;
                    return;
                }
            }
        }

        public static void ApplyDamageZones(GameObject p, MultiplayerPlayer mP)
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

                chest.AddComponent<PlayerBulletDamage>();
                arm_r1.AddComponent<PlayerBulletDamage>();
                arm_r2.AddComponent<PlayerBulletDamage>();
                arm_l1.AddComponent<PlayerBulletDamage>();
                arm_l2.AddComponent<PlayerBulletDamage>();
                head.AddComponent<PlayerBulletDamage>();
                leg_r.AddComponent<PlayerBulletDamage>();
                leg_l.AddComponent<PlayerBulletDamage>();
                // 0 - Head, 1 - Chest, 2 - Rigth Arm, 3 - Left Arm, 4 - Right Leg, 5 - Left Leg
                chest.GetComponent<PlayerBulletDamage>().SetLocaZone(chest, mP, 1);
                arm_r1.GetComponent<PlayerBulletDamage>().SetLocaZone(arm_r1, mP, 2);
                arm_r2.GetComponent<PlayerBulletDamage>().SetLocaZone(arm_r2, mP, 2);
                arm_l1.GetComponent<PlayerBulletDamage>().SetLocaZone(arm_l1, mP, 3);
                arm_l2.GetComponent<PlayerBulletDamage>().SetLocaZone(arm_l2, mP, 3);
                head.GetComponent<PlayerBulletDamage>().SetLocaZone(head, mP, 0);
                leg_r.GetComponent<PlayerBulletDamage>().SetLocaZone(leg_r, mP, 4);
                leg_l.GetComponent<PlayerBulletDamage>().SetLocaZone(leg_l, mP, 5);
            }
        }

        public static void AddLocalizedSprain(AfflictionBodyArea location, string causeID)
        {
            bool Legs = false;
            if(location == AfflictionBodyArea.LegLeft)
            {
                location = AfflictionBodyArea.FootLeft;
                Legs = true;
            }
            else if(location == AfflictionBodyArea.LegRight)
            {
                location = AfflictionBodyArea.FootRight;
                Legs = true;
            }else if(location == AfflictionBodyArea.ArmLeft)
            {
                location = AfflictionBodyArea.HandLeft;
            }else if(location == AfflictionBodyArea.ArmRight)
            {
                location = AfflictionBodyArea.HandRight;
            }

            if(Legs)
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
            }else{
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
            if(damage <= 0)
            {
                return;
            }

            MeleeDescripter DmgInfo;

            if (Meele)
            {
                DmgInfo = GetMeelePlayerInfo(MeleeWeapon);
            }else{
                DmgInfo = new MeleeDescripter();
                DmgInfo.m_PlayerDamage = damage;
                DmgInfo.m_AnimalDamage = 0;
                DmgInfo.m_BloodLoss = true;
                DmgInfo.m_ClothingTearing = true;
                DmgInfo.m_Pain = false;
            }
            
            string DamageCase = "Other player";
            if(playersData[from] != null)
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

            if(bodypart == 1) // If Chest
            {
                HasArmor = GameManager.GetDamageProtection().HasBallisticVest();
                if (HasArmor)
                {
                    damage = damage / 10;
                }
            }

            if(bodypart == 0) // If Head
            {
                if(GetClothForSlot(ClothingRegion.Head, ClothingLayer.Mid) == "GEAR_CookingPot")
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
                    if(BodyArea == AfflictionBodyArea.Head)
                    {
                        GameManager.GetHeadacheComponent().ApplyHeadache(0.5f, 5, 0.3f);
                    }
                    else if(BodyArea == AfflictionBodyArea.ArmRight 
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
                if(DmgInfo.m_ClothingTearing)
                {
                    var RNG = new System.Random(); int clothingRNG = RNG.Next(20, 40);
                    GameManager.GetPlayerManagerComponent().ApplyDamageToWornClothingRegion(Region, clothingRNG);
                }
            }else{
                var RNG = new System.Random(); int ribBroke = RNG.Next(0, 100);
                if(!Meele && ribBroke <= 5)
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
                if (players[i] != null && players[i].GetComponent<MultiplayerPlayer>() != null)
                {
                    List<GameObject> PlayerColiders = players[i].GetComponent<MultiplayerPlayer>().m_DamageColiders;

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

        public class PlayerBulletDamage : MonoBehaviour
        {
            public PlayerBulletDamage(IntPtr ptr) : base(ptr) { }
            public int m_Damage = 0;
            public GameObject m_Obj = null;
            public int m_ClientId = 0;
            public GameObject m_Player = null;
            public int m_Type = 0; // 0 - Head, 1 - Chest, 2 - Rigth Arm, 3 - Left Arm, 4 - Right Leg, 5 - Left Leg

            public void OnCollisionEnter(Collision col)
            {
                if (col.gameObject.GetComponent<ArrowItem>() != null)
                {
                    ArrowItem ARR = col.gameObject.GetComponent<ArrowItem>();
                    if (col.gameObject.GetComponent<DestoryArrowOnHit>() == null)
                    {
                        ARR.m_ArrowMesh.GetComponent<BoxCollider>().enabled = false;
                        MelonLogger.Msg("Arrow colided other player, and dealing damage " + m_Damage);
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.BULLETDAMAGE))
                            {
                                _packet.Write((float)m_Damage);
                                _packet.Write(m_Type);
                                _packet.Write(m_ClientId);
                                _packet.Write(false);
                                SendTCPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.BULLETDAMAGE))
                            {
                                ServerSend.BULLETDAMAGE(m_ClientId, (float)m_Damage, m_Type, 0);
                            }
                        }
                    }
                }
            }

            public void SetLocaZone(GameObject t, MultiplayerPlayer pl, int bodyPart)
            {
                m_Obj = t;
                m_Obj.tag = "Flesh";
                m_Obj.layer = vp_Layer.Container;
                m_Type = bodyPart;

                if (m_Obj.name == "Chest")
                {
                    m_Damage = 50;
                } else if (m_Obj.name.StartsWith("arm"))
                {
                    m_Damage = 30;
                } else if (m_Obj.name.StartsWith("Head"))
                {
                    m_Damage = 70;
                } else if (m_Obj.name.StartsWith("Thigh"))
                {
                    m_Damage = 20;
                }
                m_Player = pl.m_Player;
                pl.m_DamageColiders.Add(m_Obj);
                m_ClientId = pl.m_ID;
                //MelonLogger.Msg(m_Obj.name + " = "+ m_Damage);
            }
        }

        public static void DisableOriginalAnimalSpawns(bool host = false)
        {
            if(GameManager.m_SpawnRegionManager == null)
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
            if(spRobj != null && spRobj.GetComponent<SpawnRegion>() != null)
            {
                SpawnRegion spR = spRobj.GetComponent<SpawnRegion>();
                Vector3 zero = Vector3.zero;
                Quaternion identity = Quaternion.identity;
                BaseAi newAnimal = null;

                if (spR.TryGetSpawnPositionAndRotation(ref zero, ref identity) == true)
                {
                    if(spR.PositionValidForSpawn(zero) == true)
                    {
                        newAnimal = spR.InstantiateSpawnInternal(WildlifeMode.Normal, zero, identity);
                    }
                }
                if(newAnimal != null)
                {
                    GameObject animal = newAnimal.gameObject;
                    if (animal.GetComponent<ObjectGuid>() == null)
                    {
                        animal.AddComponent<ObjectGuid>();
                    }
                    string animalGUID = ObjectGuidManager.GenerateNewGuidString();
                    animal.GetComponent<ObjectGuid>().Set(animalGUID);
                    if(animal.GetComponent<AnimalUpdates>() != null)
                    {
                        animal.GetComponent<AnimalUpdates>().m_RegionGUID = GUID;
                    }else{
                        AnimalUpdates au = animal.AddComponent<AnimalUpdates>();
                        au.m_RegionGUID = GUID;
                    }
                    if (spRobj.GetComponent<SpawnRegionSimple>())
                    {
                        spRobj.GetComponent<SpawnRegionSimple>().m_Spawned++;
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

        public static void SetAnimationDataForAnimal(Animator AN, AnimalAnimsSync obj)
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

        public static Dictionary<string, AnimalKilled> AnimalsKilled = new Dictionary<string, AnimalKilled>();
        public static Dictionary<string, int> StunnedRabbits = new Dictionary<string, int>();

        public class AnimalKilled
        {
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Quaternion m_Rotation = new Quaternion(0, 0, 0, 0);
            public string m_PrefabName = "";
            public string m_GUID = "";
            public string m_LevelGUID = "";
            public int m_CreatedTime = 0;

            public float m_Meat = 0;
            public int m_Guts = 0;
            public int m_Hide = 0;

            public bool m_Knocked = false;

            public string m_RegionGUID = "";
        }

        public static void LoadAniamlCorpsesForScene()
        {
            List<string> ToRemove = new List<string>();

            foreach (var item in AnimalsKilled)
            {
                int DespawnTime = item.Value.m_CreatedTime + 14400;
                if (DespawnTime < MinutesFromStartServer)
                {
                    ToRemove.Add(item.Key);
                }else{
                    if (item.Value.m_LevelGUID == level_guid)
                    {
                        ProcessAnimalCorpseSync(item.Value);
                    }
                }
            }
            foreach (var item in ToRemove)
            {
                AnimalsKilled.Remove(item);
            }
        }

        public static void OnAnimalStunned(string GUID)
        {
            if (!StunnedRabbits.ContainsKey(GUID))
            {
                StunnedRabbits.Add(GUID, 5);
            }
        }
        public static void ReviveRabbit(string GUID)
        {
            ServerSend.ANIMALDELETE(0, GUID);
            GameObject Animal = ObjectGuidManager.Lookup(GUID);
            Vector3 V3;
            string LevelGUID;
            if (AnimalsController == true)
            {
                if (Animal) // If I am is animal controller and I have this rabbit, this means I can revive it.
                {
                    V3 = Animal.transform.position;
                    UnityEngine.Object.Destroy(Animal);
                    AnimalsKilled.Remove(GUID);
                    OnRabbitRevived(V3);
                }else{ // Else this means I should send this to controller.
                    AnimalKilled Body;
                    if(AnimalsKilled.TryGetValue(GUID, out Body))
                    {
                        V3 = Body.m_Position;
                        LevelGUID = Body.m_LevelGUID;
                        AnimalsKilled.Remove(GUID);
                        ServerSend.RABBITREVIVED(0, V3, LevelGUID);
                    }
                }
            }else{
                AnimalKilled Body;
                if (AnimalsKilled.TryGetValue(GUID, out Body))
                {
                    V3 = Body.m_Position;
                    LevelGUID = Body.m_LevelGUID;
                    AnimalsKilled.Remove(GUID);
                    ServerSend.RABBITREVIVED(0, V3, LevelGUID);
                }
            }
        }
        public class ElementToModfy
        {
            public string m_GUID;
            public int m_Time;
        }

        public static void UpdateStunnedRabbits()
        {
            if(StunnedRabbits.Count > 0)
            {
                List<ElementToModfy> Modify = new List<ElementToModfy>();
                foreach (var item in StunnedRabbits)
                {
                    ElementToModfy e = new ElementToModfy();
                    e.m_GUID = item.Key;
                    e.m_Time = item.Value;
                    Modify.Add(e);
                }
                foreach (var item in Modify)
                {
                    int SecondsLeft = item.m_Time;
                    SecondsLeft--;
                    if (SecondsLeft == 0)
                    {
                        ReviveRabbit(item.m_GUID);
                        StunnedRabbits.Remove(item.m_GUID);
                    }else{
                        StunnedRabbits.Remove(item.m_GUID);
                        StunnedRabbits.Add(item.m_GUID, SecondsLeft);
                    }
                }
            }
        }

        public static void FinallyPickupRabbit(int result)
        {
            RemovePleaseWait();
            if(result == 0)
            {
                HUDMessage.AddMessage("Can't pick this up");
            }else if(result == 1)
            {
                GameObject reference = GetGearItemObject("gear_rabbitcarcass");
                if (reference)
                {
                    GameObject obj = UnityEngine.Object.Instantiate<GameObject>(reference, GameManager.GetPlayerTransform().transform.position, GameManager.GetPlayerTransform().transform.rotation);
                    obj.name = reference.name;
                    if (obj && obj.GetComponent<GearItem>())
                    {
                        DropFakeOnLeave DFL = obj.AddComponent<DropFakeOnLeave>();
                        DFL.m_OldPossition = obj.transform.position;
                        DFL.m_OldRotation = obj.transform.rotation;

                        GameManager.GetPlayerManagerComponent().ProcessInspectablePickupItem(obj.GetComponent<GearItem>());
                    }
                }
            }else if(result == 2)
            {
                GameObject reference = GetGearItemObject("WILDLIFE_Rabbit");
                if (reference)
                {
                    GameObject obj = UnityEngine.Object.Instantiate<GameObject>(reference, GameManager.GetPlayerTransform().transform.position, GameManager.GetPlayerTransform().transform.rotation);
                    AnimalUpdates au = obj.GetComponent<AnimalUpdates>();
                    if (au == null)
                    {
                        obj.AddComponent<AnimalUpdates>();
                        au = obj.GetComponent<AnimalUpdates>();
                        au.m_Animal = obj;
                        au.m_PickedUp = true;
                    }else{
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
            if(AnimalsController == true)
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
                }else{
                    bai.m_SpawnPos = v3;
                    Utils.SetGuidForGameObject(bai.gameObject, ObjectGuidManager.GenerateNewGuidString());
                }
                bai.FleeFrom(v3);
            }
        }

        public static void OnReleaseRabbit(int from)
        {
            if(AnimalsController == true)
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

                if(from != instance.myId)
                {
                    if(players[from] != null)
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
                }else{
                    bai.m_SpawnPos = raycastHit.point;
                    Utils.SetGuidForGameObject(bai.gameObject, ObjectGuidManager.GenerateNewGuidString());
                }
                bai.FleeFrom(raycastHit.point);
            }
        }

        public static void AttemptToPickupRabbit(string GUID)
        {
            if(sendMyPosition == true)
            {
                DoPleaseWait("Trying to pickup little thing", "Please wait...");
                using (Packet _packet = new Packet((int)ClientPackets.PICKUPRABBIT))
                {
                    _packet.Write(GUID);
                    SendTCPData(_packet);
                }
            }
            else if(iAmHost == true)
            {
                FinallyPickupRabbit(PickUpRabbit(GUID));
            }
        }

        public static int PickUpRabbit(string GUID)
        {
            if (StunnedRabbits.ContainsKey(GUID)) // 0 is Nothing. 1 is Dead. 2 is Alive
            {
                StunnedRabbits.Remove(GUID);
                if (AnimalsKilled.ContainsKey(GUID))
                {
                    AnimalsKilled.Remove(GUID);
                }
                DeleteAnimal(GUID);
                ServerSend.ANIMALDELETE(0, GUID);
                return 2;
            }else{
                if (AnimalsKilled.ContainsKey(GUID))
                {
                    AnimalsKilled.Remove(GUID);
                    DeleteAnimal(GUID);
                    ServerSend.ANIMALDELETE(0, GUID);
                    return 1;
                }else{
                    return 0;
                }
            }
        }

        public static void OnAnimalKilled(string prefab, Vector3 v3, Quaternion rot, string GUID, string LevelGUID, string RegionGUID, bool knocked = false)
        {
            if (!AnimalsKilled.ContainsKey(GUID))
            {
                AnimalKilled Animal = new AnimalKilled();
                Animal.m_Position = v3;
                Animal.m_Rotation = rot;
                Animal.m_PrefabName = prefab;
                Animal.m_GUID = GUID;
                Animal.m_LevelGUID = LevelGUID;
                Animal.m_CreatedTime = MinutesFromStartServer;
                Animal.m_RegionGUID = RegionGUID;

                BodyHarvestUnits bh = GetBodyHarvestUnits(prefab);

                Animal.m_Meat = bh.m_Meat;
                Animal.m_Guts = bh.m_Guts;
                Animal.m_Hide = 1;
                AnimalsKilled.Add(GUID, Animal);

                if (LevelGUID == level_guid) // If I am on same scene
                {
                    ProcessAnimalCorpseSync(Animal);
                }
                if (knocked)
                {
                    OnAnimalStunned(GUID);
                }else{
                    BanSpawnRegion(RegionGUID);
                }
            }
        }

        public static void OnAnimalQuarted(string GUID)
        {
            AnimalKilled Animal;
            if (AnimalsKilled.TryGetValue(GUID, out Animal))
            {
                AnimalsKilled.Remove(GUID);
            }
        }

        public static void SpawnQuartedMess(string GUID)
        {
            GameObject AnimalCorpse = ObjectGuidManager.Lookup(GUID);
            if (AnimalCorpse)
            {
                if (AnimalCorpse.GetComponent<AnimalCorpseObject>() != null && AnimalCorpse.GetComponent<BodyHarvest>() != null)
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
                    UnityEngine.Object.DestroyImmediate(AnimalCorpse);
                }
            }
        }

        public static void OnAnimalCorpseChanged(string GUID, float MeatTaken, int GutsTaken, int HideTaken)
        {
            AnimalKilled Animal;
            if (AnimalsKilled.TryGetValue(GUID, out Animal))
            {
                Animal.m_Meat = Animal.m_Meat - MeatTaken;
                Animal.m_Guts = Animal.m_Guts - GutsTaken;
                Animal.m_Hide = Animal.m_Hide - HideTaken;
                AnimalsKilled.Remove(GUID);
                AnimalsKilled.Add(GUID, Animal);
            }
        }

        public static void ProcessAnimalCorpseSync(AnimalKilled Sync)
        {
            if(Sync.m_LevelGUID == level_guid)
            {
                SpawnAnimalCorpse(Sync.m_PrefabName, Sync.m_Position, Sync.m_Rotation, Sync.m_GUID, Sync.m_RegionGUID);
            }
        }

        public static void OpenBodyHarvest(BodyHarvest bh)
        {
            RemovePleaseWait();
            if(GameManager.GetPlayerManagerComponent().GetControlMode() == PlayerControlMode.InSnowShelter)
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

                if(iAmHost == true)
                {
                    AnimalKilled Animal;
                    if (AnimalsKilled.TryGetValue(GUID, out Animal))
                    {
                        bh.m_MeatAvailableKG = Animal.m_Meat;
                        bh.m_GutAvailableUnits = Animal.m_Guts;
                        bh.m_HideAvailableUnits = Animal.m_Hide;
                        OpenBodyHarvest(bh);
                    }else{
                        HUDMessage.AddMessage("Can't interact with this");
                    }
                }else if(sendMyPosition == true)
                {
                    DoPleaseWait("Downloading animal carcass", "Please wait...");
                    GoingToHarvest = bh;
                    using (Packet _packet = new Packet((int)ClientPackets.REQUESTANIMALCORPSE))
                    {
                        _packet.Write(GUID);
                        SendTCPData(_packet);
                    }
                }
            }else{
                HUDMessage.AddMessage("Can't interact with this");
            }
        }

        public static void SpawnAnimalCorpse(string prefab, Vector3 v3, Quaternion rot, string GUID, string SpawnRegionGUID)
        {
            MelonLogger.Msg("Going to spawn animal corpse "+ GUID +" for Region "+ SpawnRegionGUID);
            GameObject reference = GetGearItemObject(prefab);

            if (reference == null)
            {
                MelonLogger.Msg(ConsoleColor.Red, "Can't create animal body prefab name " + prefab);
                return;
            }

            GameObject oldObj = ObjectGuidManager.Lookup(GUID);
            if (oldObj)
            {
                if(oldObj.GetComponent<AnimalCorpseObject>() != null)
                {
                    MelonLogger.Msg("Corpse already exist, because been spawned myself");
                    return;
                }else{
                    MelonLogger.Msg("Replace original animal with corpse " + GUID);

                    if(oldObj.GetComponent<AnimalActor>() != null)
                    {
                        oldObj.GetComponent<AnimalActor>().m_MarkToDestroy = true;
                    }
                    else if (oldObj.GetComponent<AnimalUpdates>() != null)
                    {
                        oldObj.GetComponent<AnimalUpdates>().m_MarkToDestroy = true;
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

            Animator _AN = RemoveAnimalComponents(obj); // Remove all components

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
            AnimalCorpseObject CorpseComp = obj.AddComponent<AnimalCorpseObject>();
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
            }else{
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
            if (!AnimalCorplesList.ContainsKey(GUID))
            {
                AnimalCorplesList.Add(GUID, CorpseComp);
            }
            MelonLogger.Msg("Animal corpse spawned "+ GUID);
        }

        public static AnimalCompactData GetCompactDataForAnimal(BaseAi AI)
        {
            if(AI != null && AI.gameObject != null && AI.gameObject.GetComponent<ObjectGuid>() != null)
            {
                GameObject animal = AI.gameObject;
                AnimalCompactData Dat = new AnimalCompactData();

                Dat.m_PrefabName = GetAnimalPrefabName(animal.name);
                Dat.m_GUID = AI.gameObject.GetComponent<ObjectGuid>().Get();
                Dat.m_Position = animal.transform.position;
                Dat.m_Rotation = animal.transform.rotation;
                Dat.m_LastSeen = MinutesFromStartServer;
                Dat.m_Health = AI.m_CurrentHP;
                Dat.m_Bleeding = AI.m_BleedingOut;
                Dat.m_TimeOfBleeding = MinutesFromStartServer + (int)AI.m_ElapsedBleedingOutMinutes;
                Dat.m_LastAiMode = (int)AI.GetAiMode();

                if (animal.GetComponent<AnimalUpdates>() != null)
                {
                    Dat.m_RegionGUID = animal.GetComponent<AnimalUpdates>().m_RegionGUID;
                }else{
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

        public class AnimalTrigger : MonoBehaviour
        {
            public string m_Guid = "";
            public int m_Trigger = 0;
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

                if(i != CurrentController)
                {
                    if (playersData[i] != null)
                    {
                        if (iAmHost && i == 0)
                        {
                            otherPlayerV3 = GameManager.GetPlayerTransform().position;
                            otherPlayerLevel = levelid;
                        }else if (i == instance.myId){
                            otherPlayerV3 = GameManager.GetPlayerTransform().position;
                            otherPlayerLevel = levelid;
                        }else{
                            otherPlayerV3 = playersData[i].m_Position;
                            otherPlayerLevel = playersData[i].m_Levelid;
                        }

                        float distanceToCurrentController = Vector3.Distance(V3_Controller, otherPlayerV3);
                        float distanceToAnimal = Vector3.Distance(V3, otherPlayerV3);

                        if(distanceToCurrentController > MinimalDistance)
                        {
                            if(playersData[i].m_AnimState != "Knock" && otherPlayerLevel == LevelID_Controller && otherPlayerV3 != new Vector3(0, 0, 0))
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

        public static void SendAnimalForValidPlayers(AnimalCompactData data, AnimalAnimsSync anim)
        {
            if (NoAnimalSync)
            {
                return;
            }

            for (int i = 0; i < playersData.Count; i++)
            {
                if (playersData[i] != null)
                {
                    if (i != instance.myId)
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
                                    SendTCPData(_packet);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static AnimalAnimsSync GetAnimationDataFromAnimal(BaseAi _AI)
        {
            if (_AI)
            {
                Animator AN = _AI.m_Animator;
                if (AN)
                {
                    AnimalAnimsSync sync = new AnimalAnimsSync();
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
                }else{
                    return null;
                }
            }
            return null;
        }

        public static void RecreateAnimalToActor(GameObject animal)
        {
            string prefabName = GetAnimalPrefabName(animal.name);
            string RegionGUID = animal.GetComponent<AnimalUpdates>().m_RegionGUID;

            Vector3 v3 = animal.transform.position;
            Quaternion rot = animal.transform.rotation;

            string GUID = "";

            if (animal.GetComponent<ObjectGuid>() != null)
            {
                GUID = animal.GetComponent<ObjectGuid>().Get();
            }
            ObjectGuidManager.UnRegisterGuid(GUID);

            //MelonLogger.Msg(ConsoleColor.Cyan, "Starting recrating animal to actor " + GUID);

            SpawnAnimalActor(prefabName, v3, rot, GUID, RegionGUID);
        }

        public class AnimalUpdates : MonoBehaviour
        {
            public AnimalUpdates(IntPtr ptr) : base(ptr) { }
            public GameObject m_Animal = null;
            public int NoResponce = 5;
            public int ReTakeCoolDown = 5;
            public float nextActionTimeNR = 0.0f;
            public float noresponce_perioud = 1f;
            public float nextActionSync = 0.0f;
            public float actionSync_perioud = 0.3f;
            public float nextActionBloodDrop = 0.0f;
            public float blooddrop_period = 0.15f;
            public float nextActionDampingOn = 0.0f;
            public float dampingOn_perioud = 1.5f;
            public Vector3 m_ToGo = new Vector3(0, 0, 0);
            public Quaternion m_ToRotate = new Quaternion(0, 0, 0, 0);
            public bool m_CanSync = false;
            public float m_Hp = 100;
            public bool m_Banned = false;
            public bool m_DampingIgnore = false;
            public int LastFoundPlayer = -1;
            public bool m_MyControlled = false;
            public bool m_PickedUp = false;
            public string m_RegionGUID = "";
            public bool m_AddedToRegion = false;
            public bool m_MarkToDestroy = false;



            void Start()
            {
                nextActionTimeNR = Time.time;
                nextActionSync = Time.time;
                nextActionBloodDrop = Time.time;
                nextActionDampingOn = Time.time + dampingOn_perioud;
            }

            public void CallSync()
            {
                if (m_Animal != null && m_Banned == false && m_Animal.activeSelf == true && m_PickedUp == false)
                {
                    if (m_Animal.GetComponent<BaseAi>() != null)
                    {
                        BaseAi _AI = m_Animal.GetComponent<BaseAi>();

                        AnimalCompactData Dat = GetCompactDataForAnimal(_AI);
                        AnimalAnimsSync AnimDat = GetAnimationDataFromAnimal(_AI);

                        if(Dat.m_GUID == "")
                        {
                            return;
                        }

                        int newController = GetClosestPlayerToAnimal(m_Animal, ReTakeCoolDown, instance.myId, GameManager.GetPlayerTransform().position, levelid);
                        Dat.m_LastController = newController;
                        if (newController != instance.myId)
                        {
                            SendAnimalForValidPlayers(Dat, AnimDat);
                            m_Banned = true;
                            m_MarkToDestroy = true;
                            RecreateAnimalToActor(m_Animal);
                        }else{
                            SendAnimalForValidPlayers(Dat, AnimDat);
                        }
                    }
                }
            }
            void OnDestroy()
            {
                if (m_Animal)
                {
                    if (m_Animal.GetComponent<ObjectGuid>() != null)
                    {
                        if(m_RegionGUID != "")
                        {
                            GameObject RegionSpawnObj = ObjectGuidManager.Lookup(m_RegionGUID);

                            if (RegionSpawnObj != null)
                            {
                                RegionSpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                            }
                        }
                    }
                }
            }
            void MaybeAddToSpawnRegion()
            {
                if (!m_AddedToRegion && m_RegionGUID != "")
                {
                    m_AddedToRegion = true;
                    GameObject RegionSpawnObj = ObjectGuidManager.Lookup(m_RegionGUID);
                    if (RegionSpawnObj)
                    {
                        if (m_Animal.GetComponent<ObjectGuid>() != null && RegionSpawnObj.GetComponent<SpawnRegionSimple>() != null)
                        {
                            RegionSpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                            RegionSpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Add(m_Animal.GetComponent<ObjectGuid>().Get(), m_Animal);
                        }
                    }
                }
            }

            void Update()
            {
                if(m_MarkToDestroy == true)
                {
                    UnityEngine.Object.Destroy(m_Animal);
                    return;
                }
                
                
                if (m_Animal != null && m_PickedUp == false && m_Banned == false)
                {
                    MaybeAddToSpawnRegion();
                    if (Time.time > nextActionSync)
                    {
                        nextActionSync += actionSync_perioud;
                        if (level_name != "Empty" && level_name != "MainMenu")
                        {
                            if(AnimalsController == true || m_MyControlled == true)
                            {
                                CallSync();
                            }
                        }
                    }
                    if (Time.time > nextActionTimeNR)
                    {
                        nextActionTimeNR += noresponce_perioud;
                        ReTakeCoolDown--;
                        if (ReTakeCoolDown <= 0)
                        {
                            ReTakeCoolDown = 0;
                        }
                        if(AnimalsController == false && m_MyControlled == false)
                        {
                            m_MarkToDestroy = true;
                        }
                    }
                    if (InOnline() && m_Animal.GetComponent<BaseAi>() && m_Animal.GetComponent<ObjectGuid>() && (AnimalsController == true || m_MyControlled == true))
                    {
                        BaseAi bai = m_Animal.GetComponent<BaseAi>();
                        string RightName = GetAnimalPrefabName(m_Animal.name);
                        if (bai.m_CurrentHP <= 0 || bai.m_CurrentMode == AiMode.Dead || bai.m_CurrentMode == AiMode.Stunned)
                        {
                            AnimalKilled Corpse = new AnimalKilled();
                            Corpse.m_Position = m_Animal.transform.position;
                            Corpse.m_Rotation = m_Animal.transform.rotation;
                            Corpse.m_PrefabName = RightName;
                            Corpse.m_GUID = m_Animal.GetComponent<ObjectGuid>().Get();
                            Corpse.m_LevelGUID = level_guid;
                            Corpse.m_CreatedTime = MinutesFromStartServer;
                            Corpse.m_Knocked = bai.m_CurrentMode == AiMode.Stunned;
                            Corpse.m_RegionGUID = m_RegionGUID;

                            if (!Corpse.m_Knocked)
                            {
                                MelonLogger.Msg("Animal been killed SpawnRegion " + m_RegionGUID);
                            }else{
                                MelonLogger.Msg("Animal been knocked SpawnRegion " + m_RegionGUID);
                            }
                            m_Banned = true;

                            if (iAmHost == true)
                            {
                                m_MarkToDestroy = true;
                                ServerSend.ANIMALCORPSE(0, Corpse, true);
                                ObjectGuidManager.UnRegisterGuid(m_Animal.GetComponent<ObjectGuid>().Get());
                                OnAnimalKilled(RightName, m_Animal.transform.position, m_Animal.transform.rotation, m_Animal.GetComponent<ObjectGuid>().Get(), level_guid, m_RegionGUID, Corpse.m_Knocked);
                            }else if (sendMyPosition == true)
                            {
                                using (Packet _packet = new Packet((int)ClientPackets.ANIMALKILLED))
                                {
                                    _packet.Write(Corpse);
                                    SendTCPData(_packet);
                                }
                                m_MarkToDestroy = true;
                                ObjectGuidManager.UnRegisterGuid(m_Animal.GetComponent<ObjectGuid>().Get());
                                SpawnAnimalCorpse(RightName, m_Animal.transform.position, m_Animal.transform.rotation, m_Animal.GetComponent<ObjectGuid>().Get(), m_RegionGUID); // So we won't wait responce.
                            }
                        }
                    }
                }
            }
        }

        public static void SwitchToAnimalController()
        {
            Dictionary<string, int> AnimalsInRegions = new Dictionary<string, int>();
            if(ActorsList.Count > 0)
            {
                foreach (var item in ActorsList)
                {
                    item.Value.m_ClientController = instance.myId;
                    string RegionGUID = item.Value.m_RegionGUID;
                    if (RegionGUID != "")
                    {
                        int val;
                        if (!AnimalsInRegions.TryGetValue(RegionGUID, out val))
                        {
                            AnimalsInRegions.Add(RegionGUID, 1);
                        }else{
                            AnimalsInRegions.Remove(RegionGUID);
                            AnimalsInRegions.Add(RegionGUID, val+1);
                        }
                    }
                }
            }
            if(BaseAiManager.m_BaseAis.Count > 0)
            {
                foreach (var item in BaseAiManager.m_BaseAis)
                {
                    if(item != null && item.gameObject != null)
                    {
                        if (item.gameObject.GetComponent<AnimalUpdates>())
                        {
                            string RegionGUID = item.gameObject.GetComponent<AnimalUpdates>().m_RegionGUID;
                            if (RegionGUID != "")
                            {
                                int val;
                                if (!AnimalsInRegions.TryGetValue(RegionGUID, out val))
                                {
                                    AnimalsInRegions.Add(RegionGUID, 1);
                                }else{
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
                        }else{
                            AnimalsInRegions.Remove(RegionGUID);
                            AnimalsInRegions.Add(RegionGUID, val + 1);
                        }
                    }
                }
            }
            // Final
            if(AnimalsInRegions.Count > 0)
            {
                foreach (var item in AnimalsInRegions)
                {
                    GameObject Region = ObjectGuidManager.Lookup(item.Key);
                    if (Region && Region.GetComponent<SpawnRegionSimple>() != null)
                    {
                        Region.GetComponent<SpawnRegionSimple>().m_Spawned = item.Value;
                    }
                }
            }
        }

        public static void RecreateAnimalToSyncable(GameObject animal, string RegionGUID, float health)
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
            }else{
                newAnimal.GetComponent<ObjectGuid>().Set(GUID);
            }
            BaseAiManager.CreateMoveAgent(bai.transform, bai, v3);


            AnimalUpdates au = newAnimal.GetComponent<AnimalUpdates>();

            if (au == null)
            {
                newAnimal.AddComponent<AnimalUpdates>();

                au = newAnimal.GetComponent<AnimalUpdates>();
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
            newAnimal.transform.position = v3;
        }

        public static Dictionary<string, AnimalActor> ActorsList = new Dictionary<string, AnimalActor>();
        public static Dictionary<string, AnimalCorpseObject> AnimalCorplesList = new Dictionary<string, AnimalCorpseObject>();

        public class AnimalActor : MonoBehaviour
        {
            public AnimalActor(IntPtr ptr) : base(ptr) { }
            public GameObject m_Animal = null;
            public int NoResponce = 5;
            public int ReTakeCoolDown = 5;

            // Timers
            public float nextActionTimeNR = 0.0f;
            public float noresponce_perioud = 1f;
            public float nextActionSync = 0.0f;
            public float actionSync_perioud = 0.3f;
            public float nextActionBloodDrop = 0.0f;
            public float blooddrop_period = 0.15f;
            public float nextActionDampingOn = 0.0f;
            public float dampingOn_perioud = 1.5f;

            public Vector3 m_ToGo = new Vector3(0, 0, 0);
            public Quaternion m_ToRotate = new Quaternion(0, 0, 0, 0);

            // Animation parameters
            public float AP_TurnAngle; //0
            public float AP_TurnSpeed; //1
            public float AP_Speed; //2
            public float AP_Wounded; //3
            public float AP_Roll; //4
            public float AP_Pitch; //5
            public float AP_TargetHeading; //6
            public float AP_TargetHeadingSmooth; //7
            public float AP_TapMeter; //8
            public int AP_AiState; //10
            public bool AP_Corpse; //14
            public bool AP_Dead; //15
            public int AP_DeadSide; //16
            public int AP_DamageBodyPart; //19
            public int AP_AttackId; //23
            public bool AP_Stunned;

            public float m_Hp = 100;
            public bool m_Bleeding = false;
            public int m_ClientController = -1;
            public bool m_Banned = false;
            public bool m_MarkToDestroy = false;
            public bool m_DampingIgnore = false;
            public string m_RegionGUID = "";
            public bool m_AddedToRegion = false;

            int m_AnimParameter_TurnAngle;
            int m_AnimParameter_TurnSpeed;
            int m_AnimParameter_Speed;
            int m_AnimParameter_Wounded;
            int m_AnimParameter_Roll;
            int m_AnimParameter_Pitch;
            int m_AnimParameter_TargetHeading;
            int m_AnimParameter_TargetHeadingSmooth;
            int m_AnimParameter_TapMeter;
            int m_AnimParameter_AiState;
            int m_AnimParameter_Corpse;
            int m_AnimParameter_Dead;
            int m_AnimParameter_DamageSide;
            int m_AnimParameter_DamageBodyPart;
            int m_AnimParameter_AttackId;
            int m_AnimParameter_Stunned;

            public Dictionary<string, int> m_AnimalAnimatorHashes = new Dictionary<string, int>();
            public bool m_AnimalAnimatorHashesReady = false;
            public Animator m_Animator;

            void Start()
            {
                nextActionTimeNR = Time.time;
                nextActionSync = Time.time;
                nextActionBloodDrop = Time.time;
                nextActionDampingOn = Time.time + dampingOn_perioud;
            }

            public void AnimInit()
            {
                m_AnimParameter_TurnAngle = Animator.StringToHash("TurnAngle");
                m_AnimParameter_TurnSpeed = Animator.StringToHash("TurnSpeed");
                m_AnimParameter_Speed = Animator.StringToHash("Speed");
                m_AnimParameter_Wounded = Animator.StringToHash("Wounded");
                m_AnimParameter_Roll = Animator.StringToHash("Roll");
                m_AnimParameter_Pitch = Animator.StringToHash("Pitch");
                m_AnimParameter_TargetHeading = Animator.StringToHash("TargetHeading");
                m_AnimParameter_TargetHeadingSmooth = Animator.StringToHash("TargetHeadingSmooth");
                m_AnimParameter_TapMeter = Animator.StringToHash("TapMeter");
                m_AnimParameter_AiState = Animator.StringToHash("AiState");
                m_AnimParameter_Corpse = Animator.StringToHash("Corpse");
                m_AnimParameter_Dead = Animator.StringToHash("Dead");
                m_AnimParameter_DamageSide = Animator.StringToHash("DamageSide");
                m_AnimParameter_DamageBodyPart = Animator.StringToHash("DamageBodyPart");
                m_AnimParameter_AttackId = Animator.StringToHash("AttackId");
                m_AnimParameter_Stunned = Animator.StringToHash("Stunned");
                m_AnimalAnimatorHashesReady = true;
            }

            void SetAnimations()
            {
                Animator AN = m_Animator;
                if (AN != null)
                {
                    if (!m_AnimalAnimatorHashesReady)
                    {
                        AnimInit();
                    }

                    AN.SetFloat(m_AnimParameter_TurnAngle, AP_TurnAngle);
                    AN.SetFloat(m_AnimParameter_TurnSpeed, AP_TurnSpeed);
                    AN.SetFloat(m_AnimParameter_Speed, AP_Speed);
                    AN.SetFloat(m_AnimParameter_Wounded, AP_Wounded);
                    AN.SetFloat(m_AnimParameter_Roll, AP_Roll);
                    AN.SetFloat(m_AnimParameter_Pitch, AP_Pitch);
                    AN.SetFloat(m_AnimParameter_TargetHeading, AP_TargetHeading);
                    AN.SetFloat(m_AnimParameter_TargetHeadingSmooth, AP_TargetHeadingSmooth);
                    AN.SetFloat(m_AnimParameter_TapMeter, AP_TapMeter);
                    AN.SetInteger(m_AnimParameter_AiState, AP_AiState);
                    AN.SetBool(m_AnimParameter_Corpse, AP_Corpse);
                    AN.SetBool(m_AnimParameter_Dead, AP_Dead);
                    AN.SetInteger(m_AnimParameter_DamageSide, AP_DeadSide);
                    AN.SetInteger(m_AnimParameter_DamageBodyPart, AP_DamageBodyPart);
                    AN.SetInteger(m_AnimParameter_AttackId, AP_AttackId);
                    AN.SetBool(m_AnimParameter_Stunned, AP_Stunned);
                }
            }

            void SetPosition()
            {
                if (m_DampingIgnore == true)
                {
                    m_Animal.transform.position = m_ToGo;
                    m_Animal.transform.rotation = m_ToRotate;
                } else {
                    m_Animal.transform.position = Vector3.Lerp(m_Animal.transform.position, m_ToGo, Time.deltaTime * DeltaAnimalsMultiplayer);
                    m_Animal.transform.rotation = Quaternion.Lerp(m_Animal.transform.rotation, m_ToRotate, Time.deltaTime * DeltaAnimalsMultiplayer);
                }
            }

            void OnDestroy()
            {
                if (m_Animal)
                {
                    if (m_Animal.GetComponent<ObjectGuid>() != null)
                    {
                        ActorsList.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                        GameAudioManager.StopAllSoundsFromGameObject(m_Animal);
                    }
                    if (m_RegionGUID != "")
                    {
                        GameObject RegionSpawnObj = ObjectGuidManager.Lookup(m_RegionGUID);

                        if (RegionSpawnObj != null)
                        {
                            RegionSpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                        }
                    }
                }
            }
            void MaybeAddToSpawnRegion()
            {
                if (!m_AddedToRegion && m_RegionGUID != "")
                {
                    m_AddedToRegion = true;
                    GameObject RegionSpawnObj = ObjectGuidManager.Lookup(m_RegionGUID);

                    if (m_Animal.GetComponent<ObjectGuid>() != null)
                    {
                        if (m_RegionGUID != "")
                        {
                            if (RegionSpawnObj != null)
                            {
                                RegionSpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                                RegionSpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Add(m_Animal.GetComponent<ObjectGuid>().Get(), m_Animal);
                            }
                        }
                    }
                }
            }

            public uint m_FeedingAudioID = 0U;
            public uint m_FleeAudioId = 0U;
            public uint m_HoldGroundAudioID = 0U;
            public uint m_IdleAudioId = 0U;
            public uint m_SleepingLoopAudioID = 0U;
            public uint m_StalkingAudioID = 0U;
            public uint m_StalkingLoopAudioID = 0U;
            public uint m_StuggleAudioId = 0U;
            public uint m_WanderAudioId = 0U;
            public uint m_HideAndSeekAudioId = 0U;
            public uint m_JoinPackAudioId = 0U;

            void ExitFeeding()
            {
                if (m_FeedingAudioID == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_FeedingAudioID, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
            }
            void ExitFlee()
            {
                if (m_FleeAudioId == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_FleeAudioId, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_FleeAudioId = 0U;
            }
            void ExitHoldGround()
            {
                if (m_HoldGroundAudioID == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_HoldGroundAudioID, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_HoldGroundAudioID = 0U;
            }
            void ExitIdle()
            {
                if (m_IdleAudioId == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_IdleAudioId, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_IdleAudioId = 0U;
            }
            void ExitInvestigateFood()
            {
                ExitFeeding();
            }
            void ExitSleep()
            {
                if (m_SleepingLoopAudioID == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_SleepingLoopAudioID, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_SleepingLoopAudioID = 0U;
            }
            void ExitStalking()
            {
                if (m_StalkingAudioID != 0U)
                {
                    AkSoundEngine.StopPlayingID(m_StalkingAudioID, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                    m_StalkingAudioID = 0U;
                }
                if (m_StalkingLoopAudioID == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_StalkingLoopAudioID, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_StalkingLoopAudioID = 0U;
            }
            void ExitStruggle()
            {
                if (m_StuggleAudioId == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_StuggleAudioId, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_StuggleAudioId = 0U;
            }
            void ExitWander()
            {
                if (m_WanderAudioId == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_WanderAudioId, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_WanderAudioId = 0U;
            }
            void ExitHideAndSeek()
            {
                if (m_HideAndSeekAudioId == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_HideAndSeekAudioId, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_HideAndSeekAudioId = 0U;
            }
            void ExitJoinPack()
            {
                if (m_JoinPackAudioId == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_JoinPackAudioId, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_JoinPackAudioId = 0U;
            }

            public AiMode m_CurrentMode = AiMode.Idle;
            public AiMode m_NextMode = AiMode.Idle;

            public string m_EnterAttackModeAudio = "";
            public string m_EnterFleeModeAudio = "";
            public string m_HoldGroundAudio = "";
            public string m_IdleAudio = "";
            public string m_SleepingAudio = "";
            public string m_EnterStalkingAudio = "";
            public string m_WanderAudio = "";
            public string m_HideAndSeekAudio = "";
            public string m_JoinPackAudio = "";
            bool m_AudioReady = false;
            void EnterAttack()
            {
                GameAudioManager.Play3DSound(m_EnterAttackModeAudio, m_Animal);
            }
            void EnterFlee()
            {
                if(m_EnterFleeModeAudio == "" || m_FleeAudioId != 0U)
                {
                    return;
                }
                m_FleeAudioId = GameAudioManager.Play3DSound(m_EnterFleeModeAudio, m_Animal);
            }
            void EnterHoldGround()
            {
                if(m_HoldGroundAudio == "")
                {
                    return;
                }
                
                m_HoldGroundAudioID = GameAudioManager.Play3DSound(m_HoldGroundAudio, m_Animal);
            }
            void EnterIdle()
            {
                if (m_IdleAudio == "")
                {
                    return;
                }
                if (m_IdleAudioId == 0U)
                {
                    m_IdleAudioId = GameAudioManager.Play3DSound(m_IdleAudio, m_Animal);
                }
            }
            void EnterSleep()
            {
                if (m_SleepingAudio == "")
                {
                    return;
                }
                m_SleepingLoopAudioID = GameAudioManager.Play3DSound(m_SleepingAudio, m_Animal);
            }
            void EnterStalking()
            {
                if (m_EnterStalkingAudio == "")
                {
                    return;
                }
                m_StalkingAudioID = GameAudioManager.Play3DSound(m_EnterStalkingAudio, m_Animal);
                m_StalkingLoopAudioID = 0U;
            }
            void EnterWander()
            {
                if (m_WanderAudioId != 0U || m_WanderAudio == "")
                    return;
                m_WanderAudioId = GameAudioManager.Play3DSound(m_WanderAudio, m_Animal);
            }
            void EnterHideAndSeek()
            {
                if (m_HideAndSeekAudioId == 0U && m_HideAndSeekAudio != "")
                {
                    m_HideAndSeekAudioId = GameAudioManager.Play3DSound(m_HideAndSeekAudio, m_Animal);
                }
            }
            void EnterJoinPack()
            {
                if(m_JoinPackAudioId == 0U && m_JoinPackAudio != "")
                {
                    m_JoinPackAudioId = GameAudioManager.Play3DSound(m_JoinPackAudio, m_Animal);
                }
            }

            public void SetAiMode()
            {
                if(m_CurrentMode == m_NextMode)
                {
                    return;
                }
                
                switch (m_CurrentMode) // Stop Audio Events from current animation
                {
                    case AiMode.Attack:
                        break;
                    case AiMode.Dead:
                        break;
                    case AiMode.Feeding:
                        ExitFeeding();
                        break;
                    case AiMode.Flee:
                        ExitFlee();
                        break;
                    case AiMode.FollowWaypoints:
                        break;
                    case AiMode.HoldGround:
                        ExitHoldGround();
                        break;
                    case AiMode.Idle:
                        ExitIdle();
                        break;
                    case AiMode.Investigate:
                        break;
                    case AiMode.InvestigateFood:
                        ExitInvestigateFood();
                        break;
                    case AiMode.InvestigateSmell:
                        break;
                    case AiMode.Sleep:
                        ExitSleep();
                        break;
                    case AiMode.Stalking:
                        ExitStalking();
                        break;
                    case AiMode.Struggle:
                        ExitStruggle();
                        break;
                    case AiMode.Wander:
                        ExitWander();
                        break;
                    case AiMode.WanderPaused:
                        break;
                    case AiMode.GoToPoint:
                        break;
                    case AiMode.InteractWithProp:
                        break;
                    case AiMode.ScriptedSequence:
                        break;
                    case AiMode.Stunned:
                        break;
                    case AiMode.ScratchingAntlers:
                        break;
                    case AiMode.PatrolPointsOfInterest:
                        break;
                    case AiMode.HideAndSeek:
                        ExitHideAndSeek();
                        break;
                    case AiMode.JoinPack:
                        ExitJoinPack();
                        break;
                    case AiMode.PassingAttack:
                        break;
                    case AiMode.Howl:
                        break;
                }
                switch (m_NextMode) // Play audio for this mode
                {
                    case AiMode.Attack:
                        EnterAttack();
                        break;
                    case AiMode.Dead:;
                        break;
                    case AiMode.Feeding:
                        break;
                    case AiMode.Flee:
                        EnterFlee();
                        break;
                    case AiMode.FollowWaypoints:
                        break;
                    case AiMode.HoldGround:
                        EnterHoldGround();
                        break;
                    case AiMode.Idle:
                        EnterIdle();
                        break;
                    case AiMode.Investigate:
                        break;
                    case AiMode.InvestigateFood:
                        break;
                    case AiMode.InvestigateSmell:
                        break;
                    case AiMode.Rooted:
                        break;
                    case AiMode.Sleep:
                        this.EnterSleep();
                        break;
                    case AiMode.Stalking:
                        this.EnterStalking();
                        break;
                    case AiMode.Struggle:
                        break;
                    case AiMode.Wander:
                        this.EnterWander();
                        break;
                    case AiMode.WanderPaused:
                        break;
                    case AiMode.GoToPoint:
                        break;
                    case AiMode.InteractWithProp:
                        break;
                    case AiMode.ScriptedSequence:
                        break;
                    case AiMode.Stunned:
                        break;
                    case AiMode.ScratchingAntlers:
                        break;
                    case AiMode.PatrolPointsOfInterest:
                        break;
                    case AiMode.HideAndSeek:
                        EnterHideAndSeek();
                        break;
                    case AiMode.JoinPack:
                        EnterJoinPack();
                        break;
                    case AiMode.PassingAttack:
                        break;
                    case AiMode.Howl:
                        break;
                }
                m_CurrentMode = m_NextMode;
            }

            void SetAudioNames()
            {
                string name = m_Animal.gameObject.name;
                if (name.Contains("Bear"))
                {
                    m_EnterAttackModeAudio = "Play_BearDetect";
                    m_EnterFleeModeAudio = "Play_BearFlee";
                    m_HoldGroundAudio = "Play_BearHoldGround";
                    m_IdleAudio = "Play_BearIdle";
                    m_SleepingAudio = "Play_BearSleeping";
                    m_EnterStalkingAudio = "Play_BearAttack";
                    m_WanderAudio = "Play_BearIdle";
                }else if (name.Contains("Wolf") && !name.Contains("gray"))
                {
                    m_EnterAttackModeAudio = "Play_WolfAttackEnter";
                    m_EnterFleeModeAudio = "PLAY_WolfWhine";
                    m_HoldGroundAudio = "Play_WolfGrowlLoop";
                    m_SleepingAudio = "Play_WolfSleeping";
                    m_EnterStalkingAudio = "Play_WolfWarn";
                }else if (name.Contains("Wolf") && name.Contains("gray"))
                {
                    m_EnterAttackModeAudio = "Play_TimberwolfAttackEnter";
                    m_EnterFleeModeAudio = "Play_TimberwolfWhine";
                    m_HoldGroundAudio = "Play_TimberwolfGrowlLoop";
                    m_IdleAudio = "Play_TimberwolfIdle";
                    m_SleepingAudio = "Play_TimberwolfSleeping";
                    m_EnterStalkingAudio = "Play_TimberwolfWarn";
                    m_HideAndSeekAudio = "Play_TimberwolfGrowlLoop";
                }
                else if (name.Contains("Rabbit"))
                {
                    m_EnterFleeModeAudio = "Play_RabbitSqueal"; 
                }
                else if (name.Contains("Moose"))
                {
                    m_EnterAttackModeAudio = "Play_MooseAttack";
                    m_HoldGroundAudio = "Play_MooseAngry";
                    m_EnterStalkingAudio = "Play_MooseAlerted";
                }
                m_AudioReady = true;
            }


            void Update()
            {
                if(m_MarkToDestroy == true)
                {
                    UnityEngine.Object.Destroy(m_Animal);
                    return;
                }
                
                if (m_Animal != null && m_Banned == false)
                {
                    MaybeAddToSpawnRegion();
                    if (AnimalsController == false || m_ClientController != instance.myId)
                    {
                        SetAnimations();
                        SetPosition();
                    }

                    if (m_ClientController == instance.myId)
                    {
                        m_Banned = true;
                        m_MarkToDestroy = true;
                        RecreateAnimalToSyncable(m_Animal, m_RegionGUID, m_Hp);
                    }
                    if(!m_AudioReady)
                    {
                        SetAudioNames();
                    }

                    if (Time.time > nextActionTimeNR)
                    {
                        nextActionTimeNR += noresponce_perioud;
                        NoResponce--;
                        if (NoResponce <= 0)
                        {
                            //MelonLogger.Msg(ConsoleColor.Yellow, "Found animal that we not need anymore " + m_Animal.GetComponent<ObjectGuid>().Get());
                            GameAudioManager.StopAllSoundsFromGameObject(m_Animal);
                            m_MarkToDestroy = true;
                        }
                    }
                }
            }
        }

        public class AnimalCorpseObject : MonoBehaviour
        {
            public AnimalCorpseObject(IntPtr ptr) : base(ptr) { }
            public GameObject m_Animal = null;
            public string m_RegionGUID = "";
            public bool m_AddedToRegion = false;
            public void Update()
            {
                if (m_AddedToRegion == false)
                {
                    if(m_RegionGUID != "")
                    {
                        if (m_Animal.GetComponent<ObjectGuid>())
                        {
                            GameObject SpawnObj = ObjectGuidManager.Lookup(m_RegionGUID);
                            if (SpawnObj)
                            {
                                if (SpawnObj.GetComponent<SpawnRegionSimple>())
                                {
                                    SpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                                    SpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Add(m_Animal.GetComponent<ObjectGuid>().Get(),m_Animal);
                                    MelonLogger.Msg(ConsoleColor.Blue, "Animal corpse added to SpawnRegion " + m_RegionGUID);
                                    m_AddedToRegion = true;
                                }
                            }
                        }
                    }
                }
            }
            void OnDestroy()
            {
                if(m_Animal.GetComponent<ObjectGuid>() != null)
                {
                    AnimalCorplesList.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                    if (m_RegionGUID != "")
                    {
                        GameObject SpawnObj = ObjectGuidManager.Lookup(m_RegionGUID);
                        if (SpawnObj)
                        {
                            if (SpawnObj.GetComponent<SpawnRegionSimple>())
                            {
                                SpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                            }
                        }
                    }
                }
            }
        }

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
            }else{
                return true;
            }
        }

        public static Dictionary<string, int> BannedSpawnRegions = new Dictionary<string, int>();

        public static bool CheckSpawnRegionBanned(string GUID)
        {
            int CanBeUnbannedIn;
            if(BannedSpawnRegions.TryGetValue(GUID, out CanBeUnbannedIn))
            {
                if(MinutesFromStartServer > CanBeUnbannedIn)
                {
                    BannedSpawnRegions.Remove(GUID);
                    return false;
                }else{
                    return true;
                }
            }
            return false;
        }
        public static void BanSpawnRegion(string GUID)
        {
            int CanBeUnbannedIn = MinutesFromStartServer + 1440;
            if (!BannedSpawnRegions.ContainsKey(GUID))
            {
                BannedSpawnRegions.Add(GUID, CanBeUnbannedIn);
                ServerSend.SPAWNREGIONBANCHECK(GUID);
            }
        }


        public class SpawnRegionSimple : MonoBehaviour
        {
            public SpawnRegionSimple(IntPtr ptr) : base(ptr) { }
            public SpawnRegion m_Region;
            public string m_GUID = "";
            public bool m_RolledToBeDisabled = false;
            public bool m_ChanceRolled = false;
            public int m_Spawned = 0;
            public bool m_Banned = false;
            public bool m_CheckedForBan = false;
            public bool m_PendingBanCheck = false;
            public Dictionary<string, GameObject> m_Animals = new Dictionary<string, GameObject>();
            public void SetBanned(bool Banned)
            {
                m_CheckedForBan = true;
                m_PendingBanCheck = false;
                m_Banned = Banned;
                if (m_Banned)
                {
                    MelonLogger.Msg(ConsoleColor.Cyan, "SpawnRegion " + m_Region.GetComponent<ObjectGuid>().Get() + " is banned");
                }
            }
            public void BanCheck()
            {
                m_PendingBanCheck = true;

                if (iAmHost)
                {
                    SetBanned(CheckSpawnRegionBanned(m_Region.GetComponent<ObjectGuid>().Get()));
                }
                if (sendMyPosition)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.SPAWNREGIONBANCHECK))
                    {
                        _packet.Write(m_Region.GetComponent<ObjectGuid>().Get());
                        SendTCPData(_packet);
                    }
                }
            }
            public void UpdateFromManager()
            {                
                if(m_ChanceRolled == false)
                {
                    float newProcent = m_Region.m_ChanceActive;
                    if(m_Region.m_ChanceActive > 85)
                    {
                        newProcent = 85;
                    }

                    //float percent = m_Region.m_ChanceActive * GameManager.GetExperienceModeManagerComponent().GetSpawnRegionChanceActiveScale();
                    int seed = GameManager.GetRandomSeed((int)m_Region.m_Center.x + (int)m_Region.m_Center.y + (int)m_Region.m_Center.z);
                    System.Random RNG = new System.Random(seed);
                    bool Active = RollChanceSeeded(newProcent, RNG);
                    if (!Active)
                    {
                        m_RolledToBeDisabled = true;
                    }
                    m_ChanceRolled = true;
                    //MelonLogger.Msg("[SpawnRegion] Procent "+ m_Region.m_ChanceActive);
                    //MelonLogger.Msg("[SpawnRegion] Scaler " + GameManager.GetExperienceModeManagerComponent().GetSpawnRegionChanceActiveScale());
                    //MelonLogger.Msg("[SpawnRegion] Roll " + percent);
                }
                if(!m_RolledToBeDisabled && !m_Region.SpawningSupppressedByExperienceMode())
                {
                    int WhoClose = AnyOneClose(m_Region.m_Radius + MinimalDistanceForSpawn, m_Region.m_Center);
                    if (WhoClose != -1 && SpawnInBlizzard(m_Region) == true)
                    {
                        if (m_Region != null && AnimalsController == true)
                        {
                            if (m_Spawned < CalculateTargetPopulation(m_Region)) // If animals less than should be
                            {
                                //MelonLogger.Msg("Region "+ m_Region.GetComponent<ObjectGuid>().Get()+" going to spawn, for "+ WhoClose);

                                if (m_CheckedForBan)
                                {
                                    if (!m_Banned)
                                    {
                                        SimulateSpawnFromRegionSpawn(m_Region.GetComponent<ObjectGuid>().Get(), m_Region); // Spawn new animals for this region
                                    }
                                }
                                else if(!m_PendingBanCheck)
                                {
                                    BanCheck();
                                }
                            }
                        }
                    }else{
                        if (m_Region != null)
                        {
                            if (m_Animals.Count > 0)
                            {
                                List<GameObject> ToDelete = new List<GameObject>();
                                foreach (var item in m_Animals)
                                {
                                    GameObject animal = item.Value;
                                    if (animal != null && animal.GetComponent<BaseAi>() != null)
                                    {
                                        BaseAi AI = animal.GetComponent<BaseAi>();
                                        // If animal is valid to unload, unloading it.
                                        if (AI.GetAiMode() != AiMode.Flee && AI.GetAiMode() != AiMode.Dead && AI.GetAiMode() != AiMode.Struggle && (AI.m_CurrentTarget == null || !AI.m_CurrentTarget.IsPlayer()))
                                        {
                                            ToDelete.Add(animal);
                                        }
                                    }
                                }
                                for (int i = 0; i < ToDelete.Count; i++)
                                {
                                    if (ToDelete[i].GetComponent<ObjectGuid>())
                                    {
                                        if (iAmHost == true)
                                        {
                                            ServerSend.ANIMALDELETE(0, ToDelete[i].GetComponent<ObjectGuid>().Get());
                                        }else if(sendMyPosition == true)
                                        {
                                            using (Packet _packet = new Packet((int)ClientPackets.ANIMALDELETE))
                                            {
                                                _packet.Write(ToDelete[i].GetComponent<ObjectGuid>().Get());
                                                SendTCPData(_packet);
                                            }
                                        }
                                    }
                                   // MelonLogger.Msg("Animal deleted from region " + m_Region.GetComponent<ObjectGuid>().Get()+" Anyone close "+WhoClose);
                                    UnityEngine.Object.Destroy(ToDelete[i]);
                                    m_Spawned--;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ExitHarvesting()
        {
            Panel_BodyHarvest PBH = InterfaceManager.m_Panel_BodyHarvest;

            if (PBH.m_CurrentHarvestAction != Panel_BodyHarvest.HarvestAction.None)
            {
                PBH.OnCancel();
            }else{
                PBH.OnBack();
            }
        }

        public static void ExitHarvestingCareful()
        {
            Panel_BodyHarvest PBH = InterfaceManager.m_Panel_BodyHarvest;
            PBH.InterruptHarvest();
            PBH.CleanUpOnExit();
            if (PBH.m_PlayBookEndAnimation)
            {
                GameManager.GetPlayerAnimationComponent().Trigger_AnimatedInteraction("Trigger_HarvestDeer_End", true, (PlayerAnimation.OnAnimationEvent)null, PlayerAnimation.AnimationLayerFlags.Hip);
            }

            PBH.m_ProgressBar_Harvest.value = 0.0f;
            PBH.ResetErrorMessage();

            GameManager.GetVpFPSCamera().m_PanViewCamera.ReattachToPlayer();
            //PBH.DisableProgressBar();
            //GameManager.GetPlayerManagerComponent().ItemInHandsDuringInteractionEnd();

            PBH.enabled = false;

            //Input.ResetInputAxes();
        }

        public static GearItem GetGearItemPrefab(string name) => Resources.Load(name).Cast<GameObject>().GetComponent<GearItem>();
        public static GameObject GetGearItemObject(string name)
        {
            if(Resources.Load(name) == null)
            {
                return null;
            }
            if(Resources.Load(name).Cast<GameObject>() != null)
            {
                return Resources.Load(name).Cast<GameObject>();
            }
            return null;
        }

        //Part of code for working with Outlines.cs by Chris Nolet
        public static List<Vector3> SmoothNormals(Mesh mesh)
        {

            // Group vertices by location
            var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

            // Copy normals to a new list
            var smoothNormals = new List<Vector3>(mesh.normals);

            // Average normals for grouped vertices
            foreach (var group in groups)
            {

                // Skip single vertices
                if (group.Count() == 1)
                {
                    continue;
                }

                // Calculate the average normal
                var smoothNormal = Vector3.zero;

                foreach (var pair in group)
                {
                    smoothNormal += mesh.normals[pair.Value];
                }

                smoothNormal.Normalize();

                // Assign smooth normal to each vertex
                foreach (var pair in group)
                {
                    smoothNormals[pair.Value] = smoothNormal;
                }
            }

            return smoothNormals;
        }

        public class Outline : MonoBehaviour
        {
            public Outline(IntPtr ptr) : base(ptr) { }
            //
            //  Outline.cs
            //  QuickOutline
            //
            //  Created by Chris Nolet on 3/30/18.
            //  Copyright © 2018 Chris Nolet. All rights reserved.
            //
            private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();
            private class ListVector3
            {
                public List<Vector3> data;
            }
            private bool precomputeOutline;
            private List<Mesh> bakeKeys = new List<Mesh>();
            private List<ListVector3> bakeValues = new List<ListVector3>();

            private Renderer[] renderers;
            public Material outlineMaskMaterial;
            public Material outlineFillMaterial;

            //On update this values need set
            //needsUpdate = true;
            public int m_RenderMode = 3;
            public Color m_OutlineColor = Color.white;
            public float m_OutlineWidth = 2f;

            //OutlineAll, 0
            //OutlineVisible, 1
            //OutlineHidden, 2
            //OutlineAndSilhouette, 3
            //SilhouetteOnly 4

            public bool needsUpdate;

            void Awake()
            {

                // Cache renderers
                renderers = GetComponentsInChildren<Renderer>();

                // Instantiate outline materials
                Material _Mask = LoadedBundle.LoadAsset<Material>("OutlineMask");
                Material _Fill = LoadedBundle.LoadAsset<Material>("OutlineFill");

                outlineMaskMaterial = Instantiate(_Mask);
                outlineFillMaterial = Instantiate(_Fill);

                outlineMaskMaterial.name = "OutlineMask (Instance)";
                outlineFillMaterial.name = "OutlineFill (Instance)";

                // Retrieve or generate smooth normals
                LoadSmoothNormals();

                // Apply material properties immediately
                needsUpdate = true;
            }

            void OnEnable()
            {
                foreach (var renderer in renderers)
                {
                    if(renderer == null)
                    {
                        return;
                    }

                    // Append outline shaders
                    var materials = renderer.sharedMaterials.ToList();

                    materials.Add(outlineMaskMaterial);
                    materials.Add(outlineFillMaterial);

                    renderer.materials = materials.ToArray();
                }
            }

            void OnValidate()
            {

                // Update material properties
                needsUpdate = true;

                // Clear cache when baking is disabled or corrupted
                if (!precomputeOutline && bakeKeys.Count != 0 || bakeKeys.Count != bakeValues.Count)
                {
                    bakeKeys.Clear();
                    bakeValues.Clear();
                }

                // Generate smooth normals when baking is enabled
                if (precomputeOutline && bakeKeys.Count == 0)
                {
                    Bake();
                }
            }

            public void Update()
            {
                if (needsUpdate)
                {
                    needsUpdate = false;

                    UpdateMaterialProperties();
                }
            }

            public void OnDisable()
            {
                foreach (var renderer in renderers)
                {
                    if (renderer == null)
                    {
                        return;
                    }
                    // Remove outline shaders
                    var materials = renderer.sharedMaterials.ToList();

                    materials.Remove(outlineMaskMaterial);
                    materials.Remove(outlineFillMaterial);

                    renderer.materials = materials.ToArray();
                }
            }

            public void OnDestroy()
            {
                // Destroy material instances
                UnityEngine.Object.Destroy(outlineMaskMaterial);
                UnityEngine.Object.Destroy(outlineFillMaterial);
            }

            void Bake()
            {

                // Generate smooth normals for each mesh
                var bakedMeshes = new HashSet<Mesh>();

                foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
                {

                    // Skip duplicates
                    if (!bakedMeshes.Add(meshFilter.sharedMesh))
                    {
                        continue;
                    }

                    // Serialize smooth normals
                    var smoothNormals = MyMod.SmoothNormals(meshFilter.sharedMesh);

                    bakeKeys.Add(meshFilter.sharedMesh);
                    bakeValues.Add(new ListVector3() { data = smoothNormals });
                }
            }

            void LoadSmoothNormals()
            {

                // Retrieve or generate smooth normals
                foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
                {

                    // Skip if smooth normals have already been adopted
                    if (!registeredMeshes.Add(meshFilter.sharedMesh))
                    {
                        continue;
                    }

                    // Retrieve or generate smooth normals
                    var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
                    var smoothNormals = (index >= 0) ? bakeValues[index].data : MyMod.SmoothNormals(meshFilter.sharedMesh);

                    Il2CppSystem.Collections.Generic.List<Vector2> Converted = new Il2CppSystem.Collections.Generic.List<Vector2>();

                    for (int i = 0; i < smoothNormals.Count; i++)
                    {
                        Vector2 writeConverted = new Vector2(0, 0);

                        writeConverted = smoothNormals[i];

                        Converted.Add(writeConverted);
                    }

                    // Store smooth normals in UV3
                    meshFilter.sharedMesh.SetUVs(3, Converted);
                }

                // Clear UV3 on skinned mesh renderers
                foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    if (registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
                    {
                        skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];
                    }
                }
            }
            void UpdateMaterialProperties()
            {

                // Apply properties according to mode
                outlineFillMaterial.SetColor("_OutlineColor", m_OutlineColor);

                //OutlineAll, 0
                //OutlineVisible, 1
                //OutlineHidden, 2
                //OutlineAndSilhouette, 3
                //SilhouetteOnly 4

                if (m_RenderMode == 0)//OutlineAll
                {
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_OutlineWidth", m_OutlineWidth);
                }
                else if (m_RenderMode == 1)//OutlineVisible
                {
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    outlineFillMaterial.SetFloat("_OutlineWidth", m_OutlineWidth);
                }
                else if (m_RenderMode == 2)//OutlineHidden
                {
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                    outlineFillMaterial.SetFloat("_OutlineWidth", m_OutlineWidth);
                }
                else if (m_RenderMode == 3)//OutlineAndSilhouette, 3
                {
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_OutlineWidth", m_OutlineWidth);
                }
                else if (m_RenderMode == 4)//SilhouetteOnly
                {
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                    outlineFillMaterial.SetFloat("_OutlineWidth", 0);
                }
            }
        }

        public class FakeFire : MonoBehaviour
        {
            public FakeFire(IntPtr ptr) : base(ptr) { }
            public GameObject m_Obj;

            void Update()
            {
                if(m_Obj != null)
                {
                    if(m_Obj.GetComponent<EffectsControllerFire>() != null)
                    {
                        //m_Obj.GetComponent<EffectsControllerFire>().lighting.GetChild(0).gameObject.GetComponent<LightFadeFire>();

                        if(m_Obj.GetComponent<EffectsControllerFire>().lighting.GetChild(0).gameObject.GetComponent<FakeFireLight>() == null)
                        {
                            m_Obj.GetComponent<EffectsControllerFire>().lighting.GetChild(0).gameObject.AddComponent<FakeFireLight>();
                        }
                    }
                }
            }
        }

        public class FakeFireLight : MonoBehaviour
        {
            public FakeFireLight(IntPtr ptr) : base(ptr) { }
        }

        public class DoNotSerializeThis : MonoBehaviour
        {
            public DoNotSerializeThis(IntPtr ptr) : base(ptr) { }
        }


        public void ConnectToServer()
        {
            MyMod.instance.myId = 0;

            udp = new UDP();

            if (ip != "")
            {
                MelonLogger.Msg("Trying connect to " + ip+":"+ instance.port);
                InitializeClientData();
                //tcp.Connect();

                if(udp == null)
                {
                    MelonLogger.Msg("udp is null");
                }

                //udp.Connect(instance.port+1, ip);
                udp.Connect(instance.port, ip);
            }
        }
        //PACKETS
        public static void InitializeClientData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.XYZ, ClientHandle.XYZ},
            { (int)ServerPackets.XYZW, ClientHandle.XYZW},
            { (int)ServerPackets.BLOCK, ClientHandle.BLOCK},
            { (int)ServerPackets.XYZDW, ClientHandle.XYZDW},
            { (int)ServerPackets.LEVELID, ClientHandle.LEVELID},
            { (int)ServerPackets.GOTITEM, ClientHandle.GOTITEM},
            { (int)ServerPackets.GAMETIME, ClientHandle.GAMETIME},
            { (int)ServerPackets.LIGHTSOURCENAME, ClientHandle.LIGHTSOURCENAME},
            { (int)ServerPackets.LIGHTSOURCE, ClientHandle.LIGHTSOURCE},
            { (int)ServerPackets.ANIMSTATE, ClientHandle.ANIMSTATE},
            { (int)ServerPackets.SLEEPHOURS, ClientHandle.SLEEPHOURS},
            { (int)ServerPackets.SYNCWEATHER, ClientHandle.SYNCWEATHER},
            { (int)ServerPackets.REVIVE, ClientHandle.REVIVE},
            { (int)ServerPackets.REVIVEDONE, ClientHandle.REVIVEDONE},
            { (int)ServerPackets.ANIMALROLE, ClientHandle.ANIMALROLE},
            { (int)ServerPackets.DARKWALKERREADY, ClientHandle.DARKWALKERREADY},
            { (int)ServerPackets.HOSTISDARKWALKER, ClientHandle.HOSTISDARKWALKER},
            { (int)ServerPackets.WARDISACTIVE, ClientHandle.WARDISACTIVE},
            { (int)ServerPackets.DWCOUNTDOWN, ClientHandle.DWCOUNTDOWN},
            { (int)ServerPackets.ANIMALSYNCTRIGG, ClientHandle.ANIMALSYNCTRIGG},
            { (int)ServerPackets.SHOOTSYNC, ClientHandle.SHOOTSYNC},
            { (int)ServerPackets.PIMPSKILL, ClientHandle.PIMPSKILL},
            { (int)ServerPackets.HARVESTINGANIMAL, ClientHandle.HARVESTINGANIMAL},
            { (int)ServerPackets.SAVEDATA, ClientHandle.SAVEDATA},
            { (int)ServerPackets.BULLETDAMAGE, ClientHandle.BULLETDAMAGE},
            { (int)ServerPackets.MULTISOUND, ClientHandle.MULTISOUND},
            { (int)ServerPackets.CONTAINEROPEN, ClientHandle.CONTAINEROPEN},
            { (int)ServerPackets.LUREPLACEMENT, ClientHandle.LUREPLACEMENT},
            { (int)ServerPackets.LUREISACTIVE, ClientHandle.LUREISACTIVE},
            { (int)ServerPackets.ALIGNANIMAL, ClientHandle.ALIGNANIMAL},
            { (int)ServerPackets.ASKFORANIMALPROXY, ClientHandle.ASKFORANIMALPROXY},
            { (int)ServerPackets.CARRYBODY, ClientHandle.CARRYBODY},
            { (int)ServerPackets.BODYWARP, ClientHandle.BODYWARP},
            { (int)ServerPackets.ANIMALDELETE, ClientHandle.ANIMALDELETE},
            { (int)ServerPackets.KEEPITALIVE, ClientHandle.KEEPITALIVE},
            { (int)ServerPackets.RQRECONNECT, ClientHandle.RQRECONNECT},
            { (int)ServerPackets.EQUIPMENT, ClientHandle.EQUIPMENT},
            { (int)ServerPackets.CHAT, ClientHandle.CHAT},
            { (int)ServerPackets.PLAYERSSTATUS, ClientHandle.PLAYERSSTATUS},
            { (int)ServerPackets.CHANGENAME, ClientHandle.CHANGENAME},
            { (int)ServerPackets.CLOTH, ClientHandle.CLOTH},
            { (int)ServerPackets.ASKSPAWNDATA, ClientHandle.ASKSPAWNDATA},
            { (int)ServerPackets.LEVELGUID, ClientHandle.LEVELGUID},
            { (int)ServerPackets.FURNBROKEN, ClientHandle.FURNBROKEN},
            { (int)ServerPackets.FURNBROKENLIST, ClientHandle.FURNBROKENLIST},
            { (int)ServerPackets.FURNBREAKINGGUID, ClientHandle.FURNBREAKINGGUID},
            { (int)ServerPackets.FURNBREAKINSTOP, ClientHandle.FURNBREAKINSTOP},
            { (int)ServerPackets.GEARPICKUP, ClientHandle.GEARPICKUP},
            { (int)ServerPackets.GEARPICKUPLIST, ClientHandle.GEARPICKUPLIST},
            { (int)ServerPackets.ROPE, ClientHandle.ROPE},
            { (int)ServerPackets.ROPELIST, ClientHandle.ROPELIST},
            { (int)ServerPackets.CONSUME, ClientHandle.CONSUME},
            { (int)ServerPackets.SERVERCFG, ClientHandle.SERVERCFG},
            { (int)ServerPackets.STOPCONSUME, ClientHandle.STOPCONSUME},
            { (int)ServerPackets.HEAVYBREATH, ClientHandle.HEAVYBREATH},
            { (int)ServerPackets.BLOODLOSTS, ClientHandle.BLOODLOSTS},
            { (int)ServerPackets.APPLYACTIONONPLAYER, ClientHandle.APPLYACTIONONPLAYER},
            { (int)ServerPackets.DONTMOVEWARNING, ClientHandle.DONTMOVEWARNING},
            { (int)ServerPackets.INFECTIONSRISK, ClientHandle.INFECTIONSRISK},
            { (int)ServerPackets.CANCLEPICKUP, ClientHandle.CANCLEPICKUP},
            { (int)ServerPackets.CONTAINERINTERACT, ClientHandle.CONTAINERINTERACT},
            { (int)ServerPackets.LOOTEDCONTAINER, ClientHandle.LOOTEDCONTAINER},
            { (int)ServerPackets.LOOTEDCONTAINERLIST, ClientHandle.LOOTEDCONTAINERLIST},
            { (int)ServerPackets.HARVESTPLANT, ClientHandle.HARVESTPLANT},
            { (int)ServerPackets.LOOTEDHARVESTABLE, ClientHandle.LOOTEDHARVESTABLE},
            { (int)ServerPackets.LOOTEDHARVESTABLEALL, ClientHandle.LOOTEDHARVESTABLEALL},
            { (int)ServerPackets.SELECTEDCHARACTER, ClientHandle.SELECTEDCHARACTER},
            { (int)ServerPackets.ADDSHELTER, ClientHandle.ADDSHELTER},
            { (int)ServerPackets.REMOVESHELTER, ClientHandle.REMOVESHELTER},
            { (int)ServerPackets.ALLSHELTERS, ClientHandle.ALLSHELTERS},
            { (int)ServerPackets.USESHELTER, ClientHandle.USESHELTER},
            { (int)ServerPackets.FIRE, ClientHandle.FIRE},
            { (int)ServerPackets.CUSTOM, ClientHandle.CUSTOM},
            { (int)ServerPackets.KICKMESSAGE, ClientHandle.KICKMESSAGE},
            { (int)ServerPackets.GOTITEMSLICE, ClientHandle.GOTITEMSLICE},
            { (int)ServerPackets.VOICECHAT, ClientHandle.VOICECHAT},
            { (int)ServerPackets.SLICEDBYTES, ClientHandle.SLICEDBYTES},
            { (int)ServerPackets.ANIMALDAMAGE, ClientHandle.ANIMALDAMAGE},
            { (int)ServerPackets.FIREFUEL, ClientHandle.FIREFUEL},
            { (int)ServerPackets.DROPITEM, ClientHandle.DROPITEM},
            { (int)ServerPackets.PICKDROPPEDGEAR, ClientHandle.PICKDROPPEDGEAR},
            { (int)ServerPackets.GETREQUESTEDITEMSLICE, ClientHandle.GETREQUESTEDITEMSLICE},
            { (int)ServerPackets.GETREQUESTEDFORPLACESLICE, ClientHandle.GETREQUESTEDFORPLACESLICE},
            { (int)ServerPackets.GOTCONTAINERSLICE, ClientHandle.GOTCONTAINERSLICE},
            { (int)ServerPackets.OPENEMPTYCONTAINER, ClientHandle.OPENEMPTYCONTAINER},
            { (int)ServerPackets.MARKSEARCHEDCONTAINERS, ClientHandle.MARKSEARCHEDCONTAINERS},
            { (int)ServerPackets.READYSENDNEXTSLICE, ClientHandle.READYSENDNEXTSLICE},
            { (int)ServerPackets.CHANGEAIM, ClientHandle.CHANGEAIM},
            { (int)ServerPackets.LOADINGSCENEDROPSDONE, ClientHandle.LOADINGSCENEDROPSDONE},
            { (int)ServerPackets.GEARNOTEXIST, ClientHandle.GEARNOTEXIST},
            { (int)ServerPackets.USEOPENABLE, ClientHandle.USEOPENABLE},
            { (int)ServerPackets.TRYDIAGNISISPLAYER, ClientHandle.TRYDIAGNISISPLAYER},
            { (int)ServerPackets.SENDMYAFFLCTIONS, ClientHandle.SENDMYAFFLCTIONS},
            { (int)ServerPackets.CUREAFFLICTION, ClientHandle.CUREAFFLICTION},
            { (int)ServerPackets.ANIMALTEST, ClientHandle.ANIMALTEST},
            { (int)ServerPackets.ANIMALCORPSE, ClientHandle.ANIMALCORPSE},
            { (int)ServerPackets.REQUESTANIMALCORPSE, ClientHandle.REQUESTANIMALCORPSE},
            { (int)ServerPackets.QUARTERANIMAL, ClientHandle.QUARTERANIMAL},
            { (int)ServerPackets.ANIMALAUDIO, ClientHandle.ANIMALAUDIO},
            { (int)ServerPackets.GOTRABBIT, ClientHandle.GOTRABBIT},
            { (int)ServerPackets.RELEASERABBIT, ClientHandle.RELEASERABBIT},
            { (int)ServerPackets.HITRABBIT, ClientHandle.HITRABBIT},
            { (int)ServerPackets.RABBITREVIVED, ClientHandle.RABBITREVIVED},
            { (int)ServerPackets.MELEESTART, ClientHandle.MELEESTART},
            { (int)ServerPackets.TRYBORROWGEAR, ClientHandle.TRYBORROWGEAR},
            { (int)ServerPackets.CHALLENGEINIT, ClientHandle.CHALLENGEINIT},
            { (int)ServerPackets.CHALLENGEUPDATE, ClientHandle.CHALLENGEUPDATE},
            { (int)ServerPackets.CHALLENGETRIGGER, ClientHandle.CHALLENGETRIGGER},
            { (int)ServerPackets.ADDDEATHCONTAINER, ClientHandle.ADDDEATHCONTAINER},
            { (int)ServerPackets.DEATHCREATEEMPTYNOW, ClientHandle.DEATHCREATEEMPTYNOW},
            { (int)ServerPackets.SPAWNREGIONBANCHECK, ClientHandle.SPAWNREGIONBANCHECK},
            { (int)ServerPackets.CAIRNS, ClientHandle.CAIRNS},
        };
            MelonLogger.Msg("Initialized packets.");
        }
        public class TCP
        {
            public TcpClient socket;

            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];

                //socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);

                IAsyncResult result = socket.BeginConnect(instance.ip, instance.port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(5000, true);
                if (socket.Connected)
                {
                    socket.EndConnect(result);
                    stream = socket.GetStream();
                    receivedData = new Packet();
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                else
                {
                    socket.Close();
                    MelonLogger.Msg("Connection failed!!!");

                    //if(TryingReconnect == true)
                    //{
                    //    TryingReconnect = false;
                    //    AttempsToReconnect = AttempsToReconnect + 1;
                    //} 
                }
            }
            /*
            private void ConnectCallback(IAsyncResult _result)
            {
                socket.EndConnect(_result);

                if (!socket.Connected)
                {
                    return;
                }

                stream = socket.GetStream();

                receivedData = new Packet();

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            */
            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    Debug.Log($"Error sending data to server via TCP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                if (socket == null || stream == null)
                {
                    if (stream == null)
                    {
                        MelonLogger.Msg("ReceiveCallback has got null stream");
                    }
                    if (socket == null)
                    {
                        MelonLogger.Msg("ReceiveCallback has got null socket");
                    }
                    return;
                }

                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        MelonLogger.Msg("[CLIENT TCP SOCKET] Disconnect got no bytes in data stream");

                        if (socket != null)
                        {
                            if (sendMyPosition == true)
                            {
                                MelonLogger.Msg("Killing data stream...");
                                stream = null;
                                MelonLogger.Msg("Wiping recevied data...");
                                receivedData = null;
                                MelonLogger.Msg("Reseting data buffer...");
                                receiveBuffer = null;
                                MelonLogger.Msg("Closing socket");
                                socket.Close();
                                socket = null;
                                MelonLogger.Msg("Disconnected from server.");
                                HUDMessage.AddMessage("DISCONNECTED FROM SERVER");
                                sendMyPosition = false;
                            }
                        }
                        return;
                    } else {
                        byte[] _data = new byte[_byteLength];
                        Array.Copy(receiveBuffer, _data, _byteLength);

                        receivedData.Reset(HandleData(_data));
                        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                    }
                }
                catch
                {
                    /*
                    if(socket != null)
                    {
                        MelonLogger.Msg("[CLIENT TCP SOCKET] Disconnect cause getting callback error");

                        if (sendMyPosition == true)
                        {
                            MelonLogger.Msg("Killing data stream...");
                            stream = null;
                            MelonLogger.Msg("Wiping recevied data...");
                            receivedData = null;
                            MelonLogger.Msg("Reseting data buffer...");
                            receiveBuffer = null;
                            MelonLogger.Msg("Closing socket");
                            socket.Close();
                            socket = null;
                            MelonLogger.Msg("Disconnected from server.");
                            HUDMessage.AddMessage("DISCONNECTED FROM SERVER");
                            sendMyPosition = false;
                        }
                    }
                    */
                    MelonLogger.Msg("[CLIENT TCP SOCKET] Disconnect cause getting callback error");
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                if (receivedData == null || _data == null)
                {
                    return false;
                }

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            if (DebugTrafficCheck == true)
                            {
                                MelonLogger.Msg(ConsoleColor.Yellow, "[DebugTrafficCheck] Got packet ID " + _packetId);
                            }
                            

                            if(packetHandlers[_packetId] != null)
                            {
                                if (DebugTrafficCheck == true)
                                {
                                    MelonLogger.Msg(ConsoleColor.Yellow, "[DebugTrafficCheck] Packet contains " + _packet.ReturnSize() + " bytes");
                                }
                                packetHandlers[_packetId](_packet);
                            }else{
                                MelonLogger.Msg("Got unregisted _packetId"+ _packetId);
                            }
                        }
                    });

                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;
            }
        }

        public class UDP
        {
            public UdpClient socket;
            public IPEndPoint endPoint;

            public UDP()
            {
                endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
            }

            /// <summary>Attempts to connect to the server via UDP.</summary>
            /// <param name="_localPort">The port number to bind the UDP socket to.</param>
            public void Connect(int _localPort, string ip)
            {
                endPoint = new IPEndPoint(IPAddress.Parse(ip), instance.port);
                socket = new UdpClient(_localPort);
                socket.Connect(endPoint);
                MelonLogger.Msg("Open socket for "+ endPoint.Address+":"+ endPoint.Port);
                //IAsyncResult result = socket.BeginReceive(ReceiveCallback, null);
                socket.BeginReceive(ReceiveCallback, null);
                //bool success = result.AsyncWaitHandle.WaitOne(5000, true);
                using (Packet _packet = new Packet())
                {
                    _packet.Write(MyMod.instance.myId);
                    SendData(_packet);
                }
            }

            /// <summary>Sends data to the client via UDP.</summary>
            /// <param name="_packet">The packet to send.</param>
            public void SendData(Packet _packet)
            {
                try
                {
                    _packet.InsertInt(instance.myId); // Insert the client's ID at the start of the packet
                    if (socket != null)
                    {
                        socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    Debug.Log($"Error sending data to server via UDP: {_ex}");
                }
            }

            /// <summary>Receives incoming UDP data.</summary>
            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    byte[] _data = socket.EndReceive(_result, ref endPoint);
                    socket.BeginReceive(ReceiveCallback, null);
                    if (_data.Length < 4)
                    {
                        endPoint = null;
                        socket.Close();

                        MyMod.Disconnect();
                        MelonLogger.Msg("Connection failed");
                        return;
                    }
                    HandleData(_data);
                }
                catch
                {
                    endPoint = null;
                    socket.Close();
                    MyMod.Disconnect();
                    MelonLogger.Msg("Connection failed");
                }
            }

            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="_data">The recieved data.</param>
            private void HandleData(byte[] _data)
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetLength = _packet.ReadInt();
                    _data = _packet.ReadBytes(_packetLength);
                }

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_data))
                    {
                        int _packetId = _packet.ReadInt();
                        if (DebugTrafficCheck == true)
                        {
                            MelonLogger.Msg(ConsoleColor.Yellow, "[DebugTrafficCheck] Got packet ID " + _packetId);
                            MelonLogger.Msg(ConsoleColor.Yellow, "[DebugTrafficCheck] Packet size " + _packet.ReturnSize()+" bytes");
                        }
                        packetHandlers[_packetId](_packet); // Call appropriate method to handle the packet
                    }
                });
            }

            /// <summary>Disconnects from the server and cleans up the UDP connection.</summary>
            private void Disconnect()
            {
                Disconnect();

                endPoint = null;
                socket = null;
            }
        }

        public static void AWait()
        {
            Thread.Sleep(5000);
        }

        private static void DoneDisconnect(IAsyncResult _result)
        {
            if (MyMod.instance.udp != null && MyMod.instance.udp.socket != null)
            {
                MyMod.instance.udp.endPoint = null;
                MyMod.instance.udp.socket.Client.Close();
                //MyMod.instance.udp.socket = null;
            }
            MelonLogger.Msg("[UDP] Disconnected from server");
        }

        public static void Disconnect()
        {
            MelonLogger.Msg("[CLIENT] Trying disconnect");
            if (iAmHost == true || sendMyPosition == true)
            {
                if(SteamServerWorks != "")
                {
                    if(SteamConnect.CanUseSteam == true)
                    {
                        MelonLogger.Msg("[Steamworks.NET] Disconnecting...");
                        SteamConnect.Main.Disconnect(SteamServerWorks);
                        sendMyPosition = false;
                        MyMod.instance.myId = 0;
                        return;
                    }
                }
                if (MyMod.instance == null)
                {
                    MelonLogger.Msg("TCP/UDP instance is dead.");
                    return;
                }
                if (MyMod.instance.udp == null)
                {
                    MelonLogger.Msg("UDP is already closed.");
                    return;
                }
                if (MyMod.instance.udp.socket == null)
                {
                    MelonLogger.Msg("Socket of udp is closed.");
                    return;
                }
                if (MyMod.instance.udp != null && MyMod.instance.udp.socket != null)
                {
                    MelonLogger.Msg("[UDP] Disconnecting...");
                    //MyMod.instance.udp.endPoint = null;

                    if(MyMod.instance.udp.socket.Client != null)
                    {
                        MyMod.instance.udp.socket.Client.BeginDisconnect(true, DoneDisconnect, null);
                    }
                }
                HUDMessage.AddMessage("DISCONNECTED FROM SERVER");
                sendMyPosition = false;
                MyMod.instance.myId = 0;
            }
        }

        public static void SendTCPData(Packet _packet)
        {
            //_packet.WriteLength();
            //MyMod.instance.tcp.SendData(_packet);
            SendUDPData(_packet);
        }

        public static void RepeatUDPData(Packet _packet)
        {
            if (SteamConnect.CanUseSteam == true && ConnectedSteamWorks == true)
            {
                _packet.InsertInt(instance.myId);
                SteamConnect.Main.SendUDPData(_packet, SteamServerWorks);
            }else{
                instance.udp.SendData(_packet);
            }
        }

        public static void SendUDPData(Packet _packet)
        {
            _packet.WriteLength();

            //MelonLogger.Msg("SteamConnect.CanUseSteam " + SteamConnect.CanUseSteam);
            //MelonLogger.Msg("ConnectedSteamWorks " + ConnectedSteamWorks);

            if (SteamConnect.CanUseSteam == true && ConnectedSteamWorks == true)
            {
                _packet.InsertInt(instance.myId);
                SteamConnect.Main.SendUDPData(_packet, SteamServerWorks);
            }else{
                MyMod.instance.udp.SendData(_packet);
            }
            //MyMod.instance.udp.SendData(_packet);
        }

        public static bool IsEquippable(GearItem gi)
        {
            if (gi.m_MatchesItem || gi.m_KeroseneLampItem || gi.m_FlareItem || gi.m_NoiseMakerItem || gi.m_TorchItem || gi.m_EmergencyStim || gi.m_FlashlightItem || gi.m_FirstPersonItem)
            {
                return true;
            }
            return false;
        }

        public static void GiveRecivedItem(GearItemDataPacket gearData)
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
            }else{
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
                if(closestMatchStackable == null)
                {
                    MelonLogger.Msg("Not stack for item "+ give_name + ", creating new item");
                    GearItem new_gear = GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory(give_name);
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

                }else{
                    closestMatchStackable.m_StackableItem.m_Units+=1;
                    MelonLogger.Msg("Found stack for " + give_name + ", now units " + closestMatchStackable.m_StackableItem.m_Units);
                    if (GameManager.GetPlayerManagerComponent().m_ItemInHands == null)
                    {
                        if (IsEquippable(closestMatchStackable) == true)
                        {
                            GameManager.GetPlayerManagerComponent().EquipItem(closestMatchStackable, false);
                        }
                    }
                }
            }else{
                string bottlename = Resources.Load(give_name).Cast<GameObject>().GetComponent<GearItem>().m_LocalizedDisplayName.Text();

                MelonLogger.Msg("Got water " + gearData.m_Water);
                if (gearData.m_Water == 0.5f)
                {
                    say = "half liter of " + bottlename;
                }else{
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

            if(animal != null)
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

        public static void SetAnimationParams(GameObject animal, AnimalAnimsSync obj)
        {
            AnimalActor au = animal.GetComponent<AnimalActor>();
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
            AnimalActor au = animal.GetComponent<AnimalActor>();
            if (!au)
            {
                return;
            }
            au.m_ToGo = xyz;
            au.m_ToRotate = xyzw;
        }

        public static void DoAnimalSync(AnimalCompactData dat, AnimalAnimsSync anim)
        {
            if (NoAnimalSync)
            {
                return;
            }
            GameObject animal = ObjectGuidManager.Lookup(dat.m_GUID);
            if (animal)
            {
                if (animal.GetComponent<AnimalActor>() != null)
                {
                    AnimalActor Actor = animal.GetComponent<AnimalActor>();
                    if(Actor.m_MarkToDestroy == false)
                    {
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
            }else{
                animal = SpawnAnimalActor(dat.m_PrefabName, dat.m_Position, dat.m_Rotation, dat.m_GUID, dat.m_RegionGUID);
                if (animal && animal.GetComponent<AnimalActor>())
                {
                    AnimalActor Actor = animal.GetComponent<AnimalActor>();
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

        public static void SetAnimalTriggers(AnimalTrigger obj)
        {
            string _guid = obj.m_Guid;
            int trigg = obj.m_Trigger;

            GameObject animal = ObjectGuidManager.Lookup(_guid);

            if(animal != null)
            {
                if (animal.GetComponent<AnimalActor>() != null)
                {
                    AnimalActor _AI = animal.GetComponent<AnimalActor>();
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
            LightComp.color = new Color(1, 0.5623099f, 0.3268814f,1);
            UnityEngine.Object.Destroy(LightFX, 0.1f);
        }

        public static void DoShootSync(ShootSync shoot, int from)
        {
            //MelonLogger.Msg("Shooting client "+ from + " on level  "+ playersData[from].m_Levelid);
            if (playersData.Count > 0 && playersData[from].m_Levelid != levelid)
            {
                return;
            }
            //MelonLogger.Msg("Shoot: ");
            //MelonLogger.Msg("X: " + shoot.m_position.x);
            //MelonLogger.Msg("Y: " + shoot.m_position.y);
            //MelonLogger.Msg("Z: " + shoot.m_position.z);
            //MelonLogger.Msg("Rotation X: " + shoot.m_rotation.x);
            //MelonLogger.Msg("Rotation Y: " + shoot.m_rotation.y);
            //MelonLogger.Msg("Rotation Z: " + shoot.m_rotation.z);
            //MelonLogger.Msg("Rotation W: " + shoot.m_rotation.w);
            if (shoot.m_projectilename == "GEAR_FlareGunAmmoSingle")
            {
                FlareGunRoundItem.SpawnAndFire(GetGearItemObject("GEAR_FlareGunAmmoSingle"), shoot.m_position, shoot.m_rotation);
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

                gameObject.AddComponent<DestoryArrowOnHit>();

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
                gameObject.AddComponent<DestoryStoneOnStop>();
                gameObject.GetComponent<DestoryStoneOnStop>().m_Obj = gameObject;
                gameObject.GetComponent<DestoryStoneOnStop>().m_RB = component2;
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
            }else if (shoot.m_projectilename == "Melee")
            {
                DoMeleeHitFX(shoot.m_position, shoot.m_camera_forward, shoot.m_rotation, players[from]);
            }else{
                //MelonLogger.Msg("Got remote shoot event " + shoot.m_projectilename);

                GameObject gameObject = null;
                GearItem itemInHands = null;

                if (shoot.m_projectilename == "PistolBullet")
                {
                    itemInHands = GetGearItemPrefab("GEAR_Rifle");
                    gameObject = UnityEngine.Object.Instantiate<GameObject>(PistolBulletPrefab, shoot.m_position, shoot.m_rotation);
                    gameObject.AddComponent<ClientProjectile>();
                    ClientProjectile ClientP = gameObject.GetComponent<ClientProjectile>();
                    ClientP.m_ClientID = from;
                    PlayMultiplayer3dAduio("PLAY_RIFLE_SHOOT_3D", from);
                    DoShootFX(shoot.m_position);
                    if (players[from] != null && players[from].GetComponent<MultiplayerPlayerAnimator>() != null)
                    {
                        string shootStrhing = "RifleShoot";
                        if(playersData[from] != null && playersData[from].m_AnimState == "Ctrl")
                        {
                            shootStrhing = "RifleShoot_Sit";
                        }
                        players[from].GetComponent<MyMod.MultiplayerPlayerAnimator>().m_PreAnimStateHands = shootStrhing;
                    }
                }
                else if (shoot.m_projectilename == "RevolverBullet")
                {
                    itemInHands = GetGearItemPrefab("GEAR_Revolver");
                    gameObject = UnityEngine.Object.Instantiate<GameObject>(RevolverBulletPrefab, shoot.m_position, shoot.m_rotation);
                    gameObject.AddComponent<ClientProjectile>();
                    ClientProjectile ClientP = gameObject.GetComponent<ClientProjectile>();
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

                if(players[from] != null)
                {
                    GameAudioManager.NotifyAiAudioEvent(players[from], players[from].transform.position, GameAudioAiEvent.Gunshot);
                }
            }
        }

        public static void DoSyncContainer(ContainerOpenSync sync)
        {
            Il2CppSystem.Collections.Generic.List<Container> Boxes = ContainerManager.m_Containers;

            for (int i = 0; i < Boxes.Count; i++)
            {
                Container box = Boxes[i];
                if (box != null && box.GetComponent<ContainersSync>() != null && box.GetComponent<ContainersSync>().m_Guid == sync.m_Guid)
                {
                    if (sync.m_State == true)
                    {
                        box.GetComponent<ContainersSync>().Open();
                    }
                    else
                    {
                        box.GetComponent<ContainersSync>().Close();
                    }
                    break;
                }
            }
        }

        private static void MainThread()
        {

        }

        private static readonly List<Action> executeOnMainThread = new List<Action>();
        private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        private static bool actionToExecuteOnMainThread = false;

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

        public static void SkipRTTime(int h)
        {
            int totaltime = OverridedHourse + h;
            PlayedHoursInOnline = PlayedHoursInOnline + h;
            int leftovers;
            if (totaltime > 24)
            {
                leftovers = totaltime - 24;
                OverridedHourse = 0 + leftovers;
            }else{
                OverridedHourse = OverridedHourse + h;
            }
            int PrevMinutesFromStartServer = MinutesFromStartServer;
            MinutesFromStartServer = MinutesFromStartServer + h*60;
            MelonLogger.Msg("Skipping "+ h+" hour(s) now should be "+ OverridedHourse);
            MelonLogger.Msg("MinutesFromStartServer "+ PrevMinutesFromStartServer + " now it "+ MinutesFromStartServer+" because "+ h * 60+" been added");
            EveryInGameMinute();
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
            RestCon.RollForRestInterruption();
            GameManager.GetRestComponent().m_WakeUpAtFullRest = false;
            //MelonLogger.Msg("Called fake sleep");
        }

        public static void SendConsume(bool IsDrink)
        {
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.CONSUME))
                {
                    _packet.Write(IsDrink);
                    SendTCPData(_packet);
                }
            }

            if (iAmHost == true)
            {
                ServerSend.CONSUME(0, IsDrink, true);
            }

            if (IsDrink)
            {
                PushActionToMyDoll("Drink");
            }else{
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
                    SendTCPData(_packet);
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
            if(fire.m_FireState != FireState.FullBurn)
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
            
            if(fire.m_Campfire != null)
            {
                Campfire campFire = fire.m_Campfire.GetComponent<Campfire>();
                if (campFire.m_State != CampfireState.Lit)
                {
                    campFire.SetState(CampfireState.Lit);
                }
            }
        }

        public static void MakeFakeCampfire(FireSourcesSync SyncData)
        {
            GameObject campfireObj = UnityEngine.Object.Instantiate<GameObject>(GameManager.GetFireManagerComponent().m_CampFirePrefab);
            campfireObj.name = GameManager.GetFireManagerComponent().m_CampFirePrefab.name;
            campfireObj.transform.position = SyncData.m_Position;
            campfireObj.transform.rotation = SyncData.m_Rotation;
            Fire cfFire = campfireObj.GetComponent<Fire>();
            MakeFakeFire(cfFire);
            Campfire campFire = campfireObj.GetComponent<Campfire>();
            if(campFire.m_State != CampfireState.Lit)
            {
                campFire.SetState(CampfireState.Lit);
            }
        }

        public static bool IsSameFire(FireSourcesSync FindData, FireSourcesSync SyncData)
        {
            if(SyncData.m_IsCampfire == true)
            {
                if (SyncData.m_Position == FindData.m_Position)
                {
                    return true;
                }
            }else{
                if (FindData.m_Guid != "")
                {
                    if (SyncData.m_Guid == FindData.m_Guid)
                    {
                        return true;
                    }
                }else{
                    if (SyncData.m_Position == FindData.m_Position)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void ApplyOtherFireSource(FireSourcesSync SyncData)
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
                            FireSourcesSync FindData = new FireSourcesSync();
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
                                if(curfire.m_FireState == FireState.Off || curfire.m_StartedByPlayer == false)
                                {
                                    //MelonLogger.Msg("Apply fire on existen object");
                                    MakeFakeFire(curfire);
                                    FoundSource = true;
                                }else{
                                    //MelonLogger.Msg("Found object, but won't apply fire on it");
                                    FoundSource = true;
                                }
                                break;
                            }else{
                                //MelonLogger.Msg("Object is not same");
                            }
                        }else{
                            //MelonLogger.Msg("[FireSourcesSync] Object in array not exist!");
                        }
                    }
                    if(FoundSource == false)
                    {
                        //MelonLogger.Msg("Not found fire object");
                        if(SyncData.m_IsCampfire == true)
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

            if(fuel != null && fuel.gameObject)
            {
                UnityEngine.Object.DestroyImmediate(fuel.gameObject);
            }
        }


        public static void AddOtherFuel(FireSourcesSync SyncData, string fuelName)
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
                            FireSourcesSync FindData = new FireSourcesSync();
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
                                        if(gearItem)
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

        public static List<FireSourcesSync> FireSources = new List<FireSourcesSync>();
        public static void MayAddFireSources(FireSourcesSync fire)
        {
            //MelonLogger.Msg("[FireSourcesSync] MayAddFireSources "+ fire.m_LevelId+" "+ fire.m_LevelGUID);
            if(fire.m_LevelId != levelid || fire.m_LevelGUID != level_guid)
            {
                return;
            }
            //MelonLogger.Msg("MayAddFireSources for SyncData " + fire.m_LevelId + " m_LevelGUID " + fire.m_LevelGUID + " m_Position" + fire.m_Position.x + " " + fire.m_Position.y + " " + fire.m_Position.z);
            //MelonLogger.Msg("FireSources Size" + FireSources.Count);
            fire.m_RemoveIn = 5;
            for (int i = 0; i < FireSources.Count; i++)
            {
                FireSourcesSync currFire = FireSources[i];
                //MelonLogger.Msg("FireSources["+i+ "] m_LevelId" + currFire.m_LevelId + " m_LevelGUID "+ currFire.m_LevelGUID + " m_Position" + currFire.m_Position.x+" "+ currFire.m_Position.y+" "+ currFire.m_Position.z);
                bool UpdateCurr = false;
                if (currFire.m_LevelId == fire.m_LevelId && currFire.m_LevelGUID == fire.m_LevelGUID)
                {
                    if(currFire.m_Guid != "")
                    {
                        if(currFire.m_Guid == fire.m_Guid)
                        {
                            UpdateCurr = true;
                        }
                    }else{
                        if(currFire.m_Position == fire.m_Position)
                        {
                            UpdateCurr = true;
                        }
                    }
                }
                if(UpdateCurr == true)
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
            if(ServerConfig.m_FireSync == 0)
            {
                return;
            }
            if (fuel != "" && ServerConfig.m_FireSync == 1)
            {
                return;
            }


            //MelonLogger.Msg("SendMyFire");
            FireSourcesSync SendData = new FireSourcesSync();
            SendData.m_Fuel = fireTosend.GetRemainingLifeTimeSeconds();
            SendData.m_Guid = "";
            if(fireTosend.gameObject.GetComponent<ObjectGuid>() != null)
            {
                SendData.m_Guid = fireTosend.gameObject.GetComponent<ObjectGuid>().Get();
            }
            SendData.m_LevelGUID = level_guid;
            SendData.m_LevelId = levelid;
            SendData.m_Position = fireTosend.gameObject.transform.position;
            SendData.m_Rotation = fireTosend.gameObject.transform.rotation;

            if(fireTosend.m_Campfire == null)
            {
                SendData.m_IsCampfire = false;
            }else{
                SendData.m_IsCampfire = true;
                SendData.m_Guid = "";
            }

            if(fuel == "")
            {
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.FIRE))
                    {
                        _packet.Write(SendData);
                        SendTCPData(_packet);
                    }
                }

                if (iAmHost == true)
                {
                    ServerSend.FIRE(0, SendData, true);
                }
            }else{
                SendData.m_FuelName = fuel;
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.FIREFUEL))
                    {
                        _packet.Write(SendData);
                        SendTCPData(_packet);
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
        public static bool NeedApplyAutoCMDs = false;
        public static bool AutoHostWhenLoaded = false;
        public static int ApplyAutoThingsAfterLoaed = 0;

        public static float MinimalDistanceForSpawn = 300;
        public static float MaximalDistanceForAnimalRender = 260;

        public static int AnyOneClose(float minimalDistance, Vector3 point) // Returns true if someone locates near with this point.
        {
            // First checking if I am myself close to this point, if so end up this very quick, without even checking other.
            float MyDis = Vector3.Distance(GameManager.GetPlayerTransform().position, point);
            if(MyDis <= minimalDistance)
            {
                return instance.myId;
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
                        if(plDis < result)
                        {
                            result = plDis;
                        }
                    }
                }
            }
            return result;
        }

        public class AnimalsSpawnsShort
        {
            public Vector3 m_Position = new Vector3(0,0,0);
            public SpawnRegion m_SPR = null;
            public string m_GUID = "";

            public AnimalsSpawnsShort(SpawnRegion spR)
            {
                m_Position = spR.m_Center;
                m_SPR = spR;
                m_GUID = spR.GetComponent<ObjectGuid>().Get();
            }
        }

        public static void ProcessSleep(int Sleepers, int SleepersNeed, int Deads, int FinalHours)
        {
            bool EveryOneIsSleeping = false;
            string SleepersText = "Players sleep " + Sleepers + "/" + SleepersNeed;
            if (WaitForSleepLable != null)
            {
                WaitForSleepLable.GetComponent<UILabel>().text = "WAITING OTHER PLAYERS TO SLEEP\n" + SleepersText;
            }
            if (Sleepers >= SleepersNeed && Deads < SleepersNeed)
            {
                EveryOneIsSleeping = true;
            }

            if (WaitForSleepLable != null && WaitForSleepLable.activeSelf == true && EveryOneIsSleeping == true)
            {
                if (WaitForSleepLable != null)
                {
                    WaitForSleepLable.SetActive(false);
                }
                if (SleepingButtons != null)
                {
                    SleepingButtons.SetActive(true);
                }
                if (m_InterfaceManager != null && InterfaceManager.m_Panel_Rest != null)
                {
                    InterfaceManager.m_Panel_Rest.OnRest();
                }
            }

            if (iAmHost == true)
            {
                if (EveryOneIsSleeping == true)
                {
                    if ((GameManager.GetPlayerManagerComponent().PlayerIsSleeping() == true || IsDead == true) && IsCycleSkiping == false)
                    {
                        IsCycleSkiping = true;
                        int Skip = FinalHours;

                        for (int i = 0; i < playersData.Count; i++)
                        {
                            if (playersData[i] != null)
                            {
                                playersData[i].m_SleepHours = 0;
                            }
                        }
                        if (Skip > 0)
                        {
                            if (IsDead == true)
                            {
                                SimpleSleepWithNoSleep(Skip);
                            }
                            SkipRTTime(Skip);
                        }
                    }
                }else{
                    IsCycleSkiping = false;
                }
            }
        }


        public static Dictionary<int, bool> LoadersList = new Dictionary<int, bool>();

        public static void AddLoadingClient(int clientID)
        {
            if (!LoadersList.ContainsKey(clientID))
            {
                LoadersList.Add(clientID, true);
                MelonLogger.Msg("Client "+ clientID+" loading scene...");
            }
        }
        public static void RemoveLoadingClient(int clientID)
        {
            LoadersList.Remove(clientID);
            MelonLogger.Msg("Client " + clientID + " finished loading scene");
        }
        public static bool ClientIsLoading(int clientID)
        {
            return LoadersList.ContainsKey(clientID);
        }

        public static List<MultiPlayerClientStatus> SleepTrackerAndTimeOutAndAnimalControllers()
        {
            List<MultiPlayerClientStatus> L = new List<MultiPlayerClientStatus>();
            using (Packet _packet = new Packet((int)ServerPackets.PLAYERSSTATUS))
            {
                int ReadCount = 0;
                int Sleepers = 0;
                int Deads = 0;
                List<int> SleepingHours = new List<int>();
                SleepingHours.Add(MyCycleSkip);
                if (Application.isBatchMode == false)
                {
                    ReadCount = ReadCount + 1;
                    MultiPlayerClientStatus me = new MyMod.MultiPlayerClientStatus();
                    me.m_ID = 0;
                    me.m_Name = MyChatName;
                    me.m_Sleep = IsSleeping;
                    if (me.m_Sleep || me.m_Dead)
                    {
                        Sleepers = Sleepers + 1;
                    }
                    if (me.m_Dead)
                    {
                        Deads = Deads + 1;
                    }
                    me.m_Dead = IsDead;
                    L.Add(me);
                }

                bool shouldBeControllerME = false;
                if (IsDead == false)
                {
                    shouldBeControllerME = ShouldbeAnimalController(MyTicksOnScene, levelid, 0);
                }else{
                    shouldBeControllerME = false;
                }

                if (shouldBeControllerME != AnimalsController)
                {
                    AnimalsController = shouldBeControllerME;
                    DisableOriginalAnimalSpawns();
                    if (AnimalsController)
                    {
                        SwitchToAnimalController();
                    }
                }
                for (int i = 1; i <= Server.MaxPlayers; i++)
                {
                    if (Server.clients[i] != null && Server.clients[i].IsBusy() == true)
                    {
                        ReadCount = ReadCount + 1;
                        MultiPlayerClientStatus other = new MultiPlayerClientStatus();
                        other.m_ID = i;
                        other.m_Name = playersData[i].m_Name;
                        if (playersData[i].m_SleepHours > 0)
                        {
                            other.m_Sleep = true;
                        }else{
                            other.m_Sleep = false;
                        }
                        if (playersData[i].m_AnimState == "Knock")
                        {
                            other.m_Dead = true;
                        }else{
                            other.m_Dead = false;
                        }

                        if(other.m_Sleep || other.m_Dead)
                        {
                            Sleepers = Sleepers + 1;
                        }
                        if (other.m_Dead)
                        {
                            Deads = Deads + 1;
                        }
                        if(iAmHost == true)
                        {
                            SleepingHours.Add(playersData[other.m_ID].m_SleepHours);
                        }
                        L.Add(other);


                        int TimeOutForClient = TimeOutSeconds;
                        if (ClientIsLoading(i))
                        {
                            TimeOutForClient = TimeOutSecondsForLoaders;
                        }

                        Server.clients[i].TimeOutTime = Server.clients[i].TimeOutTime + 1;
                        if (Server.clients[i].TimeOutTime > 15)
                        {
                            MelonLogger.Msg("Client " + i + " no responce time " + Server.clients[i].TimeOutTime);
                        }
                        if (Server.clients[i].TimeOutTime > TimeOutForClient)
                        {
                            Server.clients[i].TimeOutTime = 0;
                            MyMod.MultiplayerChatMessage DisconnectMessage = new MyMod.MultiplayerChatMessage();
                            DisconnectMessage.m_Type = 0;
                            DisconnectMessage.m_By = playersData[i].m_Name;
                            DisconnectMessage.m_Message = playersData[i].m_Name + " disconnected!";
                            SendMessageToChat(DisconnectMessage, true);
                            ResetDataForSlot(i);
                            MelonLogger.Msg("Client " + i + " processing disconnect");
                            Server.clients[i].udp.Disconnect();
                        }
                        if (playersData[i] != null)
                        {
                            if (Server.clients[i].IsBusy() == true)
                            {
                                bool shouldBeController = false;
                                if (playersData[i].m_AnimState != "Knock")
                                {
                                    shouldBeController = ShouldbeAnimalController(playersData[i].m_TicksOnScene, playersData[i].m_Levelid, i);
                                }else{
                                    shouldBeController = false;
                                }
                                ServerSend.ANIMALROLE(i, shouldBeController);
                            }
                        }
                    }
                }

                _packet.Write(ReadCount);
                for (int i = 0; i < L.Count; i++)
                {
                    _packet.Write(L[i]);
                }
                
                ServerSend.SendUDPDataToAll(_packet);
                PlayersOnServer = ReadCount;
                SleepingHours.Sort();
                int Skip = 0;
                if (SleepingHours.Count > 0)
                {
                    Skip = SleepingHours[SleepingHours.Count - 1];
                }
                ProcessSleep(Sleepers, ReadCount, Deads, Skip);
            }
            return L;
        }

        public static float LastResponceTime = 0;

        public static int GetAnimalCount()
        {
            return BaseAiManager.m_BaseAis.Count + ActorsList.Count;
        }

        public static void EverySecond()
        {
            if(DsServerIsUp == true)
            {
                SecondsWithoutSaving = SecondsWithoutSaving + 1;
                if(SecondsWithoutSaving > 300)
                {
                    SecondsWithoutSaving = 0;
                    ConsoleManager.CONSOLE_save();
                    MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Server saved! Next save 5 minutes later!");
                }
            }
            if (CurrentCustomChalleng.m_Started)
            {
                CustomChallengeUpdate();
            }
            if (iAmHost)
            {
                UpdateStunnedRabbits();

                if(CurrentCustomChalleng.m_Started)
                {
                    if(iAmHost == true)
                    {
                        if (CurrentCustomChalleng.m_Time > 0)
                        {
                            CurrentCustomChalleng.m_Time--;
                        }
                        ServerSend.CHALLENGEUPDATE(CurrentCustomChalleng);
                    }
                }
            }
            if (MyLobby != "")
            {
                SteamConnect.Main.OnRegularUpdate();
            }

            if (RegularUpdateSeconds > 0)
            {
                RegularUpdateSeconds = RegularUpdateSeconds - 1;

                if(RegularUpdateSeconds == 0)
                {
                    RegularUpdateSeconds = 7;
                    SendRegularAlignData();
                }
            }
            if(SendAfterLoadingFinished > 0)
            {
                SendAfterLoadingFinished = SendAfterLoadingFinished - 1;
                if(SendAfterLoadingFinished == 0)
                {
                    FinishedLoading();
                }
            }
            if(SecondsLeftUntilWorryAboutPacket != -1)
            {
                if(SecondsLeftUntilWorryAboutPacket > 0)
                {
                    SecondsLeftUntilWorryAboutPacket = SecondsLeftUntilWorryAboutPacket - 1;
                    if(SecondsLeftUntilWorryAboutPacket == 0)
                    {
                        RepeatLastRequest();
                    }
                }
            }
            if(TryMakeLobbyAgain > 0)
            {
                TryMakeLobbyAgain = TryMakeLobbyAgain - 1;
                if(TryMakeLobbyAgain == 0)
                {
                    SteamConnect.Main.MakeLobby();
                }
            }
            PlayersUpdateManager();
            for (int i = 0; i < FireSources.Count; i++)
            {
                if(FireSources[i] != null)
                {
                    if(FireSources[i].m_RemoveIn > 0)
                    {
                        FireSources[i].m_RemoveIn = FireSources[i].m_RemoveIn - 1;

                        if(FireSources[i].m_LevelId == levelid && FireSources[i].m_LevelGUID == level_guid)
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
                if(NeedLoadSaveAfterLoad > 0)
                {
                    NeedLoadSaveAfterLoad = NeedLoadSaveAfterLoad - 1;
                    if(NeedLoadSaveAfterLoad == 0)
                    {
                        NeedLoadSaveAfterLoad = -1;
                        ForceLoadSlotForPlaying(AutoStartSlot);
                    }
                }
                if(StartDSAfterLoad > 0)
                {
                    StartDSAfterLoad = StartDSAfterLoad - 1;
                    if(StartDSAfterLoad == 0)
                    {
                        StartDSAfterLoad = -1;
                        StartDedicatedServer();
                    }
                }
            }

            if (DsServerIsUp && Application.isBatchMode)
            {
                levelid = 0;
                level_guid = "null";
            }

            if (level_name != "Empty" && level_name != "Boot" && level_name != "MainMenu")
            {
                //MelonLogger.Msg("Trying call UpdateMyClothing");
                UpdateMyClothing();
                ApplyOpenables();

                if (Application.isBatchMode && DsServerIsUp == false)
                {
                    DsServerIsUp = true;

                    if (SetP2PToLobbyForDSAfterLoad)
                    {
                        SetP2PToLobbyForDSAfterLoad = false;
                        FinishStartingDsServer();
                    }
                    MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Server is ready! Have fun!");
                }

                if (ApplyAutoThingsAfterLoaed == 0 && (NeedApplyAutoCMDs == true || AutoHostWhenLoaded == true))
                {
                    if(NeedApplyAutoCMDs == true || AutoHostWhenLoaded == true)
                    {
                        MelonLogger.Msg(ConsoleColor.Magenta, "[Start-ups] Going to apply some parameters after load save");
                    }
                    
                    ApplyAutoThingsAfterLoaed = 3;
                }
                if (ApplyAutoThingsAfterLoaed > 0)
                {
                    ApplyAutoThingsAfterLoaed = ApplyAutoThingsAfterLoaed - 1;
                    if(ApplyAutoThingsAfterLoaed == 0)
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

                if(UpdateLootedContainers > 0)
                {
                    UpdateLootedContainers = UpdateLootedContainers - 1;
                    if(UpdateLootedContainers == 0)
                    {
                        UpdateLootedContainers = -1;
                        //MelonLogger.Msg(ConsoleColor.Blue, "Apply looted containers");
                        ApplyLootedContainers();
                    }
                }

                if (UpdatePickedGears > 0)
                {
                    UpdatePickedGears = UpdatePickedGears - 1;
                    if (UpdatePickedGears == 0)
                    {
                        UpdatePickedGears = -1;
                        //MelonLogger.Msg(ConsoleColor.Blue, "Apply picked gears");
                        DestoryPickedGears();
                    }
                }

                if(UpdatePickedPlants > 0)
                {
                    UpdatePickedPlants = UpdatePickedPlants - 1;
                    if(UpdatePickedPlants == 0)
                    {
                        UpdatePickedPlants = -1;
                        RemoveHarvastedPlants();
                    }
                }

                if(UpdateSnowshelters > 0)
                {
                    UpdateSnowshelters = UpdateSnowshelters - 1;
                    if(UpdateSnowshelters == 0)
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

                    if(UpdateRopesAndFurns == 0)
                    {
                        //MelonLogger.Msg(ConsoleColor.Blue, "Apply ropes and furns");
                        DestoryBrokenFurniture();
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
                if(UpdateEverything > 0)
                {
                    UpdateEverything = UpdateEverything - 1;
                    if(UpdateEverything == 0)
                    {
                        UpdateEverything = -1;
                        Pathes.LoadEverything();
                    }
                }
                if (RemoveAttachedObjectsAfterSecond == true)
                {
                    DestoryPickedGears();
                    RemoveAttachedObjectsAfterSecond = false;
                }
                for (int n = 0; n < RecentlyPickedGears.Count; n++)
                {
                    PickedGearSync currGear = RecentlyPickedGears[n];
                    if (currGear != null)
                    {
                        if (currGear.m_Recently > 0)
                        {
                            currGear.m_Recently = currGear.m_Recently - 1;
                        }
                        if (currGear.m_Recently <= 0)
                        {
                            RecentlyPickedGears.RemoveAt(n);
                        }
                    }
                }

                if (GameManager.m_Thirst != null)
                {
                    Thirst th = GameManager.GetThirstComponent();
                    if (th.m_LitersLeftToDrink > 0)
                    {
                        IsDrinking = true;
                    }else{
                        IsDrinking = false;
                    }
                    if (PreviousIsDrinking != IsDrinking)
                    {
                        PreviousIsDrinking = IsDrinking;
                        if (IsDrinking == true)
                        {
                            MelonLogger.Msg("Drinking");
                            SendConsume(true);
                        }else{
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
                    }else{
                        IsEating = false;
                    }
                    if (PreviousIsEating != IsEating)
                    {
                        PreviousIsEating = IsEating;
                        if (IsEating == true)
                        {
                            if(hun.m_FoodItemProvidingCalories != null)
                            {
                                MelonLogger.Msg("Eating " + hun.m_FoodItemProvidingCalories.m_GearItem.m_GearName);
                                SendConsume(hun.m_FoodItemProvidingCalories.m_IsDrink);
                            }
                        }else{
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

            if(chatInput != null && chatInput.gameObject.activeSelf == false && ChatObject != null && ChatObject.activeSelf == true)
            {
                HideChatTimer = HideChatTimer-1;
                if(HideChatTimer <= 0)
                {
                    ChatObject.SetActive(false);
                }
            }

            if (iAmHost == true)
            {
                List<MultiPlayerClientStatus> PlayersListDat = SleepTrackerAndTimeOutAndAnimalControllers();
                UpdatePlayerStatusMenu(PlayersListDat);
            }

            if (LastConnectedIp != "" || iAmHost == true)
            {
                int character = 0;

                if(GameManager.m_PlayerManager  != null)
                {
                    character = (int)GameManager.GetPlayerManagerComponent().m_VoicePersona;
                }

                if (iAmHost == true)
                {
                    ServerSend.SELECTEDCHARACTER(0, character, true);
                }

                if (sendMyPosition == true)
                {
                    if(LastResponceTime == 0)
                    {
                        LastResponceTime = Time.time;
                    }else{
                        //double Ping = Math.Round(Time.time - MyMod.LastResponceTime, 2) * 100;
                        //MelonLogger.Msg("Ping " + Ping + "ms");
                    }
                    using (Packet _packet = new Packet((int)ClientPackets.SELECTEDCHARACTER))
                    {
                        _packet.Write(character);
                        SendTCPData(_packet);
                    }
                }
            }
            if (iAmHost == true && ServerHandle.OverflowAnimalsOnConnectTimer > 0)
            {
                MaxAnimalsSyncNeed = MaxAnimalsSyncCountOnConnect;
                ServerHandle.OverflowAnimalsOnConnectTimer = ServerHandle.OverflowAnimalsOnConnectTimer - 1;
            }else{
                MaxAnimalsSyncNeed = MaxAnimalsSyncCount;
            }
        }

        private static void EveryInGameMinute()
        {
            OverridedMinutes = OverridedMinutes + 1;

            if (iAmHost == true) 
            {
                MinutesFromStartServer = MinutesFromStartServer + 1;
                MPSaveManager.SaveRecentStuff();
            }

            if (OverridedMinutes > 59)
            {
                OverridedMinutes = 0;
                OverridedHourse = OverridedHourse + 1;
                PlayedHoursInOnline = PlayedHoursInOnline + 1;
            }
            if (OverridedHourse > 23)
            {
                OverridedHourse = 0;
            }

            OveridedTime = OverridedHourse + ":" + OverridedMinutes;

            ServerHandle.gametime = OveridedTime;

            using (Packet _packet = new Packet((int)ServerPackets.GAMETIME))
            {
                ServerSend.GAMETIME(OveridedTime);
            }

            if (level_name != "Boot" && level_name != "Empty" && GameManager.m_Wind != null && GameManager.m_Wind.m_ActiveSettings != null && GameManager.m_Weather != null && GameManager.m_WeatherTransition != null)
            {
                using (Packet _packet = new Packet((int)ServerPackets.SYNCWEATHER))
                {
                    WeatherProxies weather = new WeatherProxies();
                    weather.m_WeatherProxy = GameManager.GetWeatherComponent().Serialize();
                    weather.m_WeatherTransitionProxy = GameManager.GetWeatherTransitionComponent().Serialize();
                    weather.m_WindProxy = GameManager.GetWindComponent().Serialize();

                    ServerSend.SYNCWEATHER(1, weather);
                }
            }
        }

        public static float nextActionTime = 0.0f;
        public static float period = 5f;
        public static float nextActionTimeAniamls = 0.0f;
        public static float periodAniamls = 0.3f;
        public static float nextActionTimeSecond = 0.0f;
        public static float periodSecond = 1f;

        public static bool IsShatalkerMode() // This is darlkwalker mode active.
        {
            if (IamShatalker == true || InDarkWalkerMode == true) // If you are darkwalker
            {
                return true;
            } else
            {
                if (ShatalkerModeClient == false && ServerHandle.DarkShatalkerMode == false) // If other player host/client is darkwalker
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

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
                }else{
                    if (_name.StartsWith("WILDLIFE_Wolf_grey_aurora"))
                    {
                        return "WILDLIFE_Wolf_grey_aurora";
                    }else{
                        if (_name.StartsWith("WILDLIFE_Wolf_grey"))
                        {
                            return "WILDLIFE_Wolf_grey";
                        }else{
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
                }else{
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
            else{
                return "WILDLIFE_Wolf";
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
                UnityEngine.Object.DestroyImmediate(animal.transform.parent.GetComponent<MoveAgent>());
            }
            if (animal.transform.parent != null && animal.transform.parent.GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
            {
                UnityEngine.Object.DestroyImmediate(animal.transform.parent.GetComponent<UnityEngine.AI.NavMeshAgent>());
            }
            if (animal.GetComponent<AiTarget>() != null)
            {
                UnityEngine.Object.DestroyImmediate(animal.GetComponent<AiTarget>());
            }
            if (animal.GetComponent<AiWolf>() != null)
            {
                UnityEngine.Object.DestroyImmediate(animal.GetComponent<AiWolf>());
            }
            if (animal.GetComponent<AiStag>() != null)
            {
                UnityEngine.Object.DestroyImmediate(animal.GetComponent<AiStag>());
            }
            if (animal.GetComponent<AiRabbit>() != null)
            {
                UnityEngine.Object.DestroyImmediate(animal.GetComponent<AiRabbit>());
            }
            //MelonLogger.Msg("AiRabbit yahooo");
            if (animal.GetComponent<AiMoose>() != null)
            {
                UnityEngine.Object.DestroyImmediate(animal.GetComponent<AiMoose>());
            }

            if (animal.GetComponent<AiBear>() != null)
            {
                UnityEngine.Object.DestroyImmediate(animal.GetComponent<AiBear>());
            }
            if (animal.GetComponent<CharacterController>() != null)
            {
                UnityEngine.Object.DestroyImmediate(animal.GetComponent<CharacterController>());
            }
            //MelonLogger.Msg("CharacterController ANUS SEBE CONTROLIRUI");
            if (animal.GetComponent<NodeCanvas.Framework.Blackboard>() != null)
            {
                UnityEngine.Object.DestroyImmediate(animal.GetComponent<NodeCanvas.Framework.Blackboard>());
            }
            //MelonLogger.Msg("Blackboard DA BECAUSE DA");
            if (animal.GetComponent<TLDBehaviourTreeOwner>() != null)
            {
                UnityEngine.Object.DestroyImmediate(animal.GetComponent<TLDBehaviourTreeOwner>());
            }
            if(animal.GetComponent<AnimalUpdates>() != null)
            {
                UnityEngine.Object.DestroyImmediate(animal.GetComponent<AnimalUpdates>());
            }
            if (animal.GetComponent<BaseAi>() != null)
            {
                Animator anim = animal.GetComponent<BaseAi>().m_Animator;
                UnityEngine.Object.DestroyImmediate(animal.GetComponent<BaseAi>());
                return anim;
            }
            return null;
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
        public static GameObject SpawnAnimalActor(string prefabName, Vector3 v3, Quaternion rot, string GUID, string RegionGUID)
        {
            GameObject reference = GetGearItemObject(prefabName);

            if (reference == null)
            {
                MelonLogger.Msg(ConsoleColor.Red, "Can't create animal actor with prefab name "+ prefabName);
                return null;
            }

            GameObject obj = UnityEngine.Object.Instantiate(reference, v3, rot);
            AnimalActor act = obj.AddComponent<AnimalActor>();
            act.m_Animal = obj;
            act.m_Animator = RemoveAnimalComponents(obj);
            act.m_ToGo = v3;
            act.m_ToRotate = rot;
            Light[] componentsInChildren = (Light[])obj.GetComponentsInChildren<Light>(true);
            if (componentsInChildren != null)
            {
                for (int index = 0; index < componentsInChildren.Length; ++index)
                    componentsInChildren[index].enabled = false;
            }

            if(obj.GetComponent<ObjectGuid>() == null)
            {
                obj.AddComponent<ObjectGuid>();
                obj.GetComponent<ObjectGuid>().Set(GUID);
            }else{
                obj.GetComponent<ObjectGuid>().Set(GUID);
            }

            if (!ActorsList.ContainsKey(GUID))
            {
                ActorsList.Add(GUID, act);
            }
            
            return obj;
        }

        public static bool ShouldbeAnimalController(int _Ticks, int _Level, int _From)
        {
            if (playersData.Count < MaxPlayers)
            {
                return false;
            }
            int LastFoundTicks = 0;
            int LastFoundID = -1;
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (_From != i)
                {
                    int OtherPlayerLevel = 0;
                    int OtherPlayerTicks = 0;
                    bool ValidPlayer = false;
                    
                    if (i == 0)
                    {
                        OtherPlayerLevel = levelid;
                        OtherPlayerTicks = MyTicksOnScene;

                        if(IsDead == false)
                        {
                            ValidPlayer = true;
                        }else{
                            ValidPlayer = false;
                        }
                    }else{
                        OtherPlayerLevel = playersData[i].m_Levelid;
                        OtherPlayerTicks = playersData[i].m_TicksOnScene;
                        ValidPlayer = Server.clients[i].IsBusy();
                        if (Server.clients[i].IsBusy() == true && playersData[i].m_AnimState != "Knock")
                        {
                            ValidPlayer = true;
                        }else{
                            ValidPlayer = false;
                        }
                    }

                    if (ValidPlayer == true)
                    {
                        if(_Level == OtherPlayerLevel)
                        {
                            if(LastFoundTicks == 0)
                            {
                                if (OtherPlayerTicks > _Ticks)
                                {
                                    //MelonLogger.Msg("[Animals] [Client " + _From + "] "+_Ticks+" less than [Client "+ i + "]" + OtherPlayerTicks);
                                    LastFoundTicks = OtherPlayerTicks;
                                    LastFoundID = i;
                                }
                            }else{
                                if (LastFoundTicks < OtherPlayerTicks)
                                {
                                    //MelonLogger.Msg("[Animals] [Client " + _From + "] LastFoundTicks " + LastFoundTicks + " less than [Client " + i + "]" + OtherPlayerTicks);
                                    LastFoundTicks = OtherPlayerTicks;
                                    LastFoundID = i;
                                }
                            }
                        }
                    }
                }
            }

            if(LastFoundID == -1)
            {
                //MelonLogger.Msg("[Animals] Client "+_From+" Is controller");
                return true;
            }else{
                //MelonLogger.Msg("[Animals] Client " + _From + " can't be controller cause of Client "+ LastFoundID);
                //MelonLogger.Msg("[Animals] [Client "+ _From + "] Ticks "+ _Ticks + " [Client "+ LastFoundID + "] Ticks "+ LastFoundTicks);
                return false;
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
                        }else{
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

        public static SaveSlotSync PendingSave = null;
        public static bool ShouldCreateSaveForHost = false;

        public static void SendSlotData(int _forClient)
        {
            MelonLogger.Msg("Sending savedata for "+_forClient);
            SaveSlotSync SaveData = new SaveSlotSync();
            SaveData.m_Episode = (int) SaveGameSystem.m_CurrentEpisode;
            SaveData.m_SaveSlotType = (int) SaveGameSystem.m_CurrentGameMode;
            SaveData.m_Seed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            SaveData.m_ExperienceMode = (int) ExperienceModeManager.s_CurrentModeType;
            SaveData.m_Location = (int) RegionManager.GetCurrentRegion();
            SaveData.m_FixedSpawnScene = SavedSceneForSpawn;
            SaveData.m_FixedSpawnPosition = SavedPositionForSpawn;

            if(ExperienceModeManager.s_CurrentModeType == ExperienceModeType.Custom)
            {
                SaveData.m_CustomExperienceStr = GameManager.GetExperienceModeManagerComponent().GetCurrentCustomModeString();
            }else{
                SaveData.m_CustomExperienceStr = "";
            }

            using (Packet __packet = new Packet((int)ServerPackets.SAVEDATA))
            {
                ServerSend.SAVEDATA(_forClient, SaveData);
            }
        }

        public static void SelectNameForHostSaveFile()
        {
            if(m_Panel_MainMenu != null)
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
                }else{
                    if (!ShouldCreateSaveForHost)
                    {
                        ForcedCreateSave(PendingSave);
                    }else{
                        SelectNameForHostSaveFile();
                    }
                }
            }
        }

        public static void SelectGenderForConnection()
        {
            if(m_Panel_SelectSurvivor != null)
            {
                m_Panel_SelectSurvivor.Enable(true);
                m_Panel_SelectSurvivor.m_BasicMenu.m_OnClickBackAction = null;
            }
        }

        public static void CheckHaveSaveFileToJoin(SaveSlotSync Data)
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

                if (Slot.m_UserDefinedName == Seed+"")
                {
                    MelonLogger.Msg("Found slot to load");
                    MelonLogger.Msg("Loading save file with seed: " + Seed);
                    HaveSaveFile = true;
                    SaveToLoad = Slot;
                    break;
                }
            }

            if(HaveSaveFile == true)
            {
                KillConsole(); // Unregistering cheats if server not allow cheating for you
                MelonLogger.Msg("Trying loading save slot...");
                SaveGameSlots.SetBaseNameForSave(SaveToLoad.m_SaveSlotName, SaveToLoad.m_SaveSlotName);
                MelonLogger.Msg("Save slot base name is " + SaveGameSlots.GetBaseNameForSave(SaveToLoad.m_SaveSlotName));
                MelonLogger.Msg("Save slot name " + SaveToLoad.m_SaveSlotName);
                MelonLogger.Msg("Save slot user defined name " + SaveGameSlots.GetUserDefinedSlotName(SaveToLoad.m_SaveSlotName));
                SaveGameSystem.SetCurrentSaveInfo(SaveToLoad.m_Episode, SaveToLoad.m_GameMode, SaveToLoad.m_GameId, SaveToLoad.m_SaveSlotName);
                MelonLogger.Msg("Selecting slot " + SaveGameSystem.GetCurrentSaveName());
                GameManager.LoadSaveGameSlot(SaveToLoad.m_SaveSlotName, SaveToLoad.m_SaveChangelistVersion);
            }else{
                LetChooseSpawnForClient(Data);
            }
        }

        public static void LetChooseSpawnForClient(SaveSlotSync Data)
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


        public static void ForcedCreateSave(SaveSlotSync Data, string Name = "")
        {
            Episode Ep = (Episode)Data.m_Episode;
            SaveSlotType SST = (SaveSlotType)Data.m_SaveSlotType;
            int Seed = Data.m_Seed;
            ExperienceModeType ExpType = (ExperienceModeType)Data.m_ExperienceMode;
            GameRegion Region = (GameRegion)Data.m_Location;

            MelonLogger.Msg("Creating save slot " + Seed);
            GameManager.GetExperienceModeManagerComponent().SetExperienceModeType(ExpType);
            //SaveGameSystem.SetCurrentSaveInfo(Ep, SST, SaveGameSlots.GetUnusedGameId(), null);

            KillConsole(); // Unregistering cheats if server not allow cheating for you

            SaveGameSystem.m_CurrentEpisode = Ep;
            SaveGameSystem.m_CurrentGameId = SaveGameSlots.GetUnusedGameId();
            SaveGameSystem.m_CurrentGameMode = SST;
            SaveGameSystem.m_CurrentSaveName = SaveGameSlots.BuildSlotName(SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameMode, SaveGameSystem.m_CurrentGameId);
            if(Seed != 0)
            {
                SaveGameSlots.SetSlotDisplayName(SaveGameSystem.m_CurrentSaveName, Seed + "");
            }else{
                SaveGameSlots.SetSlotDisplayName(SaveGameSystem.m_CurrentSaveName, Name);
            }
            
            GameManager.GetExperienceModeManagerComponent().SetExperienceModeType(ExpType);
            if(ExpType == ExperienceModeType.Custom)
            {
                MelonLogger.Msg("Custom Experience Mode Key is " + Data.m_CustomExperienceStr);
                bool ok = GameManager.GetExperienceModeManagerComponent().SetCurrentCustomModeString(Data.m_CustomExperienceStr);
                if (ok)
                {
                    MelonLogger.Msg("Custom Experience Mode recreated from string!");
                }else{
                    MelonLogger.Msg(ConsoleColor.Red, "Failed to recreate custom experience from string!");
                }
            }

            MelonLogger.Msg("Save slot created!");
            MelonLogger.Msg("Save slot current name " + SaveGameSystem.GetCurrentSaveName());

            GameManager.m_StartRegion = Region;
            InterloperHook = true;
            OverridedSceneForSpawn = Data.m_FixedSpawnScene;
            OverridedPositionForSpawn = Data.m_FixedSpawnPosition;

            GameManager.Instance().LaunchSandbox();
            if(Seed != 0)
            {
                GameManager.m_SceneTransitionData.m_GameRandomSeed = Seed;
            }
            
            PendingSave = null;
            ShouldCreateSaveForHost = false;
        }

        public static void PlayMultiplayer3dAduio(string sound, int from)
        {
            if(playersData.Count > 0 && playersData[from] != null && playersData[from].m_Levelid == levelid && players[from] != null && playersData[from].m_LevelGuid == level_guid)
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
                    SendTCPData(_packet);
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
            if(Golosovanie == null)
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
        public static List<MultiplayerChatMessage> ChatMessages = new List<MultiplayerChatMessage>();
        public static int MaxChatMessages = 50;
        public static string MyChatName = "Player";

        public static UnityEngine.UI.InputField chatInput = null;
        public static GameObject chatPanel = null;
        public static GameObject ChatObject = null;
        public static GameObject StatusPanel = null;
        public static GameObject StatusObject = null;
        public static UnityEngine.UI.ScrollRect chatScroller = null;
        public static int HideChatTimer = 0;

        public static void SaveNewName(string newname)
        {
            if (System.IO.File.Exists("Mods\\nickname.txt"))
            {
                System.IO.File.WriteAllText("Mods\\nickname.txt", newname);
            }
            else
            {
                using (System.IO.StreamWriter sw = System.IO.File.CreateText("Mods\\nickname.txt"))
                {
                    sw.Write(newname);
                }
            }
            if (sendMyPosition == true) // CLIENT
            {
                using (Packet _packet = new Packet((int)ClientPackets.CHANGENAME))
                {
                    _packet.Write(newname);
                    SendTCPData(_packet);
                }
            }
            if (iAmHost == true) // HOST
            {
                using (Packet _packet = new Packet((int)ServerPackets.CHANGENAME))
                {
                    ServerSend.CHANGENAME(0, newname, true);
                }
            }
        }
        public static bool ValidNickName(string name)
        {
            if(name == "" || name == " " || name == "Player" || HasNonASCIIChars(name) == true)
            {
                return false;
            }else{
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
            }else{
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
                DummyLoader.AddComponent<LoadScene>();

                Loader.m_SceneToLoad = OutDoorToLoad;
                Loader.Activate();
            }else{
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
            if(GameManager.m_PlayerObject == null)
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
                if(playersData[i] != null && playersData[i].m_Levelid == levelid && playersData[i].m_LevelGuid == level_guid && Vector3.Distance(myXYZ,playersData[i].m_Position) < 15 && i != instance.myId)
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
        public static Dictionary<ulong,GameObject> LobbyElements = new Dictionary<ulong, GameObject>();

        public class LobbyHoverNickname : MonoBehaviour
        {
            public LobbyHoverNickname(IntPtr ptr) : base(ptr) { }
            public UnityEngine.UI.Button m_Btn;
            public string m_Name = "???";

            void Update()
            {
                if (m_Btn)
                {
                    if(m_Btn.currentSelectionState == UnityEngine.UI.Selectable.SelectionState.Highlighted)
                    {
                        if(LobbyUI != null)
                        {
                            LobbyUI.transform.GetChild(3).gameObject.SetActive(true);
                            LobbyUI.transform.GetChild(3).GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = m_Name;

                            //Rect rect = LobbyUI.transform.GetChild(3).gameObject.GetComponent<RectTransform>().rect;
                            //Rect btnRect = m_Btn.GetComponent<RectTransform>().rect;
                            //Rect newRect = new Rect(btnRect.x, rect.y, rect.width, rect.height);


                            LobbyUI.transform.GetChild(3).gameObject.transform.position = new Vector3(m_Btn.gameObject.transform.position.x, LobbyUI.transform.GetChild(3).gameObject.transform.position.y, LobbyUI.transform.GetChild(3).gameObject.transform.position.z);
                        }
                    }
                }
            }
        }

        public class CookpotHelmet : MonoBehaviour
        {
            public CookpotHelmet(IntPtr ptr) : base(ptr) { }
            public GearItem m_GearItem;
            public ClothingItem m_ClothingItem;

            void Update()
            {
                if (m_GearItem && m_ClothingItem)
                {
                    if(InterfaceManager.m_Panel_Clothing != null && InterfaceManager.m_Panel_Clothing.isActiveAndEnabled)
                    {
                        m_GearItem.m_ClothingItem = null;
                    }else{
                        m_GearItem.m_ClothingItem = m_ClothingItem;
                    }
                }
            }
        }

        public class Borrowable : MonoBehaviour
        {
            public Borrowable(IntPtr ptr) : base(ptr) { }
            public string m_GearName = "";
            public int m_PlayerID = 0;
            public MultiplayerPlayer m_mP;
            public GameObject m_Obj;
            
            void Update()
            {

            }
        }


        public static void AddPersonToLobby(string name, ulong SteamID, Texture2D Avatar)
        {
            if (!LobbyElements.ContainsKey(SteamID))
            {
                GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>("MP_PlayerLobby");
                GameObject Element = GameObject.Instantiate(LoadedAssets, LobbyUI.transform.GetChild(0).GetChild(0));
                Sprite sprite = Sprite.Create(Avatar, new Rect(0, 0, 64, -64), new Vector2(0, 0));
                Element.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>().overrideSprite = sprite;
                LobbyElements.Add(SteamID, Element);
                Element.AddComponent<LobbyHoverNickname>();
                LobbyHoverNickname LHN = Element.GetComponent<LobbyHoverNickname>();
                LHN.m_Btn = Element.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Button>();
            }
        }

       public static bool LobbyContains(ulong SteamID)
       {
            return LobbyElements.ContainsKey(SteamID);
       }

        public static void RemovePersonFromLobby(ulong SteamID)
        {
            GameObject Element;
            if (LobbyElements.TryGetValue(SteamID, out Element))
            {
                LobbyElements.Remove(SteamID);
                UnityEngine.Object.Destroy(Element);
            }
        }
        public static void SetPersonNameFromLobby(ulong SteamID, string name)
        {
            GameObject Element;
            if (LobbyElements.TryGetValue(SteamID, out Element))
            {
                Element.GetComponent<LobbyHoverNickname>().m_Name = name;
            }
        }

        public static void UpdatePlayerStatusMenu(List<MultiPlayerClientStatus> MPs)
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

                        StatusTexes[i].SetActive(true);
                    }
                }
            }
        }

        public static void SendMessageToChat(MultiplayerChatMessage message, bool needSync = true)
        {
            //if(HasNonASCIIChars(message.m_Message) == true)
            //{
            //    message.m_Type = 0;
            //    message.m_By = MyChatName;
            //    message.m_Message = "Please use only english! Other languages are not supported!";
            //    needSync = false;
            //}
            if(message.m_By.Contains("Filigrani") || message.m_By.Contains("REDcat"))
            {
                if (message.m_Message == "!debug")
                {
                    if (DebugGUI == false)
                    {
                        DebugGUI = true;
                        DebugBind = true;
                    }else{
                        DebugGUI = false;
                        DebugBind = false;
                    }
                    return;
                }
            } 
            if (message.m_Message == "!fasteat")
            {
                if(iAmHost == true)
                {
                    if(ServerConfig.m_FastConsumption == false)
                    {
                        ServerConfig.m_FastConsumption = true;
                    }else{
                        ServerConfig.m_FastConsumption = false;
                    }
                    message.m_Type = 0;
                    message.m_By = MyChatName;
                    message.m_Message = "Server configuration parameter ServerConfig.m_FastConsumption now is "+ServerConfig.m_FastConsumption;
                    needSync = true;
                    ServerSend.SERVERCFGUPDATED();
                }else{
                    message.m_Type = 0;
                    message.m_By = MyChatName;
                    message.m_Message = "You not a host to change this!";
                    needSync = false;
                }
            }
            if (message.m_Message == "!dupes")
            {
                if (iAmHost == true)
                {
                    if (ServerConfig.m_DuppedSpawns == false)
                    {
                        ServerConfig.m_DuppedSpawns = true;
                    }else{
                        ServerConfig.m_DuppedSpawns = false;
                    }
                    message.m_Type = 0;
                    message.m_By = MyChatName;
                    message.m_Message = "Server configuration parameter ServerConfig.m_DuppedSpawns now is " + ServerConfig.m_DuppedSpawns;
                    needSync = true;
                    ServerSend.SERVERCFGUPDATED();
                }else{
                    message.m_Type = 0;
                    message.m_By = MyChatName;
                    message.m_Message = "You not a host to change this!";
                    needSync = false;
                }
            }
            //if (message.m_Message == "!dupesboxes")
            //{
            //    if (iAmHost == true)
            //    {
            //        if (ServerConfig.m_DuppedContainers == false)
            //        {
            //            ServerConfig.m_DuppedContainers = true;
            //        }else{
            //            ServerConfig.m_DuppedContainers = false;
            //        }
            //        message.m_Type = 0;
            //        message.m_By = MyChatName;
            //        message.m_Message = "Server configuration parameter ServerConfig.m_DuppedContainers now is " + ServerConfig.m_DuppedContainers;
            //        needSync = true;
            //        ServerSend.SERVERCFGUPDATED();
            //    }else{
            //        message.m_Type = 0;
            //        message.m_By = MyChatName;
            //        message.m_Message = "You not a host to change this!";
            //        needSync = false;
            //    }
            //}
            if (message.m_Message.StartsWith("!fire ") == true)
            {
                if (iAmHost == true)
                {
                    string text = message.m_Message;
                    int fire = Convert.ToInt32(text.Replace("!fire ", ""));
                    message.m_Type = 0;
                    message.m_By = MyChatName;

                    if(fire == 0 || fire == 1 || fire == 2)
                    {
                        ServerConfig.m_FireSync = fire;
                        message.m_Message = "Server configuration parameter ServerConfig.m_FireSync now is " + ServerConfig.m_FireSync;
                        needSync = true;
                        ServerSend.SERVERCFGUPDATED();
                    }
                    else{
                        message.m_Message = "ServerConfig.m_FireSync can't be  " + fire+". It can be only 0 or 1 or 2.";
                        needSync = false;
                    }
                }else{
                    message.m_Type = 0;
                    message.m_By = MyChatName;
                    message.m_Message = "You not a host to change this!";
                    needSync = false;
                }
            }
            if (message.m_Message.StartsWith("!name ") == true)
            {
                string text = message.m_Message;
                MyChatName = text.Replace("!name ", "");
                message.m_Type = 0;
                message.m_By = MyChatName;
                message.m_Message = "Your new name "+ MyChatName;
                needSync = false;
                SaveNewName(MyChatName);
            }
            if (message.m_Message.StartsWith("!v ") == true)
            {
                string text = message.m_Message;
                DeBugMenu.DebugVal = Convert.ToInt32(text.Replace("!v ", ""));
                MultiplayerChatMessage NewMsg = new MultiplayerChatMessage();
                message.m_Type = 0;
                message.m_By = MyChatName;
                message.m_Message = "New value is " + DeBugMenu.DebugVal;
                
                SendMessageToChat(NewMsg, false);
            }

            if (ChatMessages.Count > MaxChatMessages)
            {
                UnityEngine.Object.Destroy(ChatMessages[0].m_TextObj.gameObject);
                ChatMessages.RemoveAt(0);
            }
            GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>("MP_ChatText");
            GameObject newText = GameObject.Instantiate(LoadedAssets, chatPanel.transform);
            UnityEngine.UI.Text Comp = newText.GetComponent<UnityEngine.UI.Text>();
            message.m_TextObj = Comp;
            if(message.m_Type == 1)
            {
                Comp.text = message.m_By + ": " + message.m_Message;
            }else{
                Comp.text = message.m_Message;
            }

            if(message.m_Type == 0)
            {
                Comp.color = Color.green;
            }

            ChatMessages.Add(message);
            HideChatTimer = 5;
            ChatObject.SetActive(true);

            if(needSync == true)
            {
                if (sendMyPosition == true) // CLIENT
                {
                    using (Packet _packet = new Packet((int)ClientPackets.CHAT))
                    {
                        _packet.Write(message);
                        SendTCPData(_packet);
                    }
                }
                if (iAmHost == true) // HOST
                {
                    using (Packet _packet = new Packet((int)ServerPackets.CHAT))
                    {
                        ServerSend.CHAT(0, message, true);
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
            PlayerEquipmentData Edata = new PlayerEquipmentData();
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
                    SendTCPData(_packet);
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

        //public static void LinkEquipmentToRIG(GameObject m_Player)
        //{
        //    int hipsChild = 3;
        //    int rootChild = 8;
        //    Transform equipment = m_Player.transform.GetChild(4);
        //    Transform hip = m_Player.transform.GetChild(hipsChild);
        //    Transform root = hip.transform.GetChild(rootChild);
        //    Transform Spine1 = root.transform.GetChild(0);
        //    Transform Spine2 = Spine1.transform.GetChild(0);
        //    Transform hand_r = Spine2.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0);
        //    Transform hand_l = Spine2.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0);

        //    for (int i = 0; i < equipment.childCount; i++)
        //    {
        //        if(equipment.GetChild(i) != null)
        //        {
        //            GameObject EQ = equipment.GetChild(i).gameObject;
        //        }
        //    }
        //}

        

        public static void InitAllPlayers()
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (playersData.Count < MaxPlayers)
                {
                    playersData.Add(null);
                }
                if (playersData[i] == null)
                {
                    playersData[i] = new MultiPlayerClientData();
                }
            }

            if(MyRadioAudio == null)
            {
                GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>("MyRadio");
                MyRadioAudio = GameObject.Instantiate(LoadedAssets);

                GameObject RadioAudio = MyRadioAudio.transform.GetChild(1).gameObject;
                GameObject RadioBg = MyRadioAudio.transform.GetChild(2).gameObject;
                RadioAudio.AddComponent<MultiplayerPlayerVoiceChatPlayer>().aSource = RadioAudio.GetComponent<AudioSource>();
                RadioAudio.GetComponent<MultiplayerPlayerVoiceChatPlayer>().aSourceBgNoise = RadioBg.GetComponent<AudioSource>();
                RadioAudio.GetComponent<MultiplayerPlayerVoiceChatPlayer>().m_ID = -1;
                RadioAudio.GetComponent<MultiplayerPlayerVoiceChatPlayer>().m_RadioFilter = true;
            }

            for (int i = 0; i < MaxPlayers; i++)
            {
                if (players.Count < MaxPlayers)
                {
                    players.Add(null);
                }
                if (players[i] == null)
                {
                    GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>("multiplayerPlayer");
                    GameObject m_Player = GameObject.Instantiate(LoadedAssets);
                    m_Player.AddComponent<MultiplayerPlayerAnimator>().m_Animer = m_Player.GetComponent<Animator>();
                    m_Player.AddComponent<MultiplayerPlayerClothingManager>().m_Player = m_Player;

                    Transform Speakers = m_Player.transform.GetChild(4);
                    GameObject Generic = Speakers.GetChild(0).gameObject;
                    GameObject Voice = Speakers.GetChild(1).gameObject;
                    GameObject Radio = Speakers.GetChild(2).gameObject;
                    GameObject RadioBg = Speakers.GetChild(3).gameObject;

                    Voice.AddComponent<MultiplayerPlayerVoiceChatPlayer>().aSource = Voice.GetComponent<AudioSource>();
                    Voice.GetComponent<MultiplayerPlayerVoiceChatPlayer>().m_ID = i;

                    Radio.AddComponent<MultiplayerPlayerVoiceChatPlayer>().aSource = Radio.GetComponent<AudioSource>();
                    Radio.GetComponent<MultiplayerPlayerVoiceChatPlayer>().aSourceBgNoise = RadioBg.GetComponent<AudioSource>();
                    Radio.GetComponent<MultiplayerPlayerVoiceChatPlayer>().m_ID = i;
                    Radio.GetComponent<MultiplayerPlayerVoiceChatPlayer>().m_RadioFilter = true;
                    

                    MultiplayerPlayer mP = m_Player.AddComponent<MultiplayerPlayer>();
                    mP.m_Player = m_Player;
                    mP.m_ID = i;

                    players[i] = m_Player;

                    if (playersData.Count > 0 && playersData[i] != null)
                    {
                        m_Player.transform.position = playersData[i].m_Position;
                        m_Player.transform.rotation = playersData[i].m_Rotation;
                    }
                    ApplyDamageZones(m_Player, mP);
                    m_Player.SetActive(false);

                    mP.m_TorchIgniter = m_Player.transform.GetChild(3).GetChild(8).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(8).GetChild(1).gameObject; //Tourch Fire
                }
            }

            if(MyPlayerDoll == null)
            {
                GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>("multiplayerPlayer");
                GameObject m_Player = GameObject.Instantiate(LoadedAssets);
                m_Player.name = "MyPlayerDoll";
                MyPlayerDoll = m_Player;
                MyPlayerDoll.SetActive(false);
                MultiplayerPlayerAnimator Anim = m_Player.AddComponent<MultiplayerPlayerAnimator>();
                Anim.m_MyDoll = true;
                Anim.m_Animer = m_Player.GetComponent<Animator>();
                DollCameraDummy = m_Player.transform.GetChild(3).GetChild(8).GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).gameObject;
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
                    }else{
                        if (DefaultIsRussian)
                        {
                            Txt = "\nЕсли вы используйте hamachi/radmin или другой эмулятор локалки, и у вас проблемы с подключением, не простите о помощи нас.";
                        }else{
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
            Gear = 3,
        }
        public static ResendPacketType LastPacketForRepeat = ResendPacketType.None;
        public static int SecondsLeftUntilWorryAboutPacket = -1;
        public static void SetRepeatPacket(ResendPacketType Pak)
        {
            LastPacketForRepeat = Pak;
            SecondsLeftUntilWorryAboutPacket = TimeToWorryAboutLastRequest;
        }
        public static void DiscardRepeatPacket()
        {
            SecondsLeftUntilWorryAboutPacket = -1;
            LastPacketForRepeat = ResendPacketType.None;
        }

        public static void SimulateAnotherRequest(ResendPacketType request)
        {
            if(request == ResendPacketType.Scene)
            {
                Pathes.RequestDropsForScene();
            }else{
                MelonLogger.Msg("Can't simulate request "+ request);
                DiscardRepeatPacket();
                DoPleaseWait("Request failed", "Something wrong with #"+request+" request, please reconnect to the server!");
            }
        }

        public static void RepeatLastRequest()
        {
            RemovePleaseWait();
            if(LastPacketForRepeat != ResendPacketType.None)
            {
                SlicedJsonDataBuffer.Clear();
                SecondsLeftUntilWorryAboutPacket = TimeToWorryAboutLastRequest;
                DoPleaseWait("Something wrong", "Timed out on response from host for last request. Trying to repeat request to correct the situation, please wait...");
                SimulateAnotherRequest(LastPacketForRepeat);
            }
        }

        public static void DoPleaseWait(string title, string text)
        {
            if (m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
            {
                InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Waiting, title, "\n"+text, Panel_Confirmation.ButtonLayout.Button_0, Panel_Confirmation.Background.Transperent, null, null);
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
                InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, "DISCONNECTED", "\n"+ txt, Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null, null);
            }
            MelonLogger.Msg("Kicked by server, message from host: "+ txt);
            MyMod.LastConnectedIp = "";
            MyMod.Disconnect();
        }

        public static void CancleDismantling()
        {
            ShowShelterByOther FindData = new ShowShelterByOther();

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
                            ShowShelterByOther shelter = MyMod.playersData[i].m_Shelter;
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
                    MultiplayerPlayerVoiceChatPlayer mPVoice = MyRadioAudio.transform.GetChild(1).gameObject.GetComponent<MultiplayerPlayerVoiceChatPlayer>();
                    VoiceChatQueueElement DataForQueue = new VoiceChatQueueElement();
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
                if (players[from] != null)
                {
                    if (playersData[from].m_Levelid == levelid && playersData[from].m_LevelGuid == level_guid)
                    {
                        MultiplayerPlayerVoiceChatPlayer mPVoice;
                        if (!Radio)
                        {
                            mPVoice = players[from].transform.GetChild(4).GetChild(1).gameObject.GetComponent<MultiplayerPlayerVoiceChatPlayer>();
                        }else{
                            mPVoice = players[from].transform.GetChild(4).GetChild(2).gameObject.GetComponent<MultiplayerPlayerVoiceChatPlayer>();
                        }
                        if (mPVoice)
                        {
                            VoiceChatQueueElement DataForQueue = new VoiceChatQueueElement();
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

            if((chatInput != null && chatInput.gameObject.activeSelf == true) || (uConsole.m_Instance != null && uConsole.m_On == true))
            {
                DoingRecord = false;
            }

            if(PreviousRecord != DoingRecord)
            {
                if(DoingRecord == false)
                {
                    RecordReleseButtonHold = Time.time + 0.5f;
                    if(ViewModelRadio && ViewModelRadio.activeSelf)
                    {
                        //PlayRadioOver();
                    }
                }else{
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
                else if(Radio)
                {
                    SetA = 0;
                }
                Img.color = new Color(Img.color.r, Img.color.g, Img.color.b, SetA);
            }

            if(RadioIdicator != null)
            {
                UnityEngine.UI.Image Img = RadioIdicator.GetComponent<UnityEngine.UI.Image>();
                float SetA = 1f;
                if (!DoingRecord)
                {
                    float CurrA = Img.color.a;
                    SetA = Mathf.Lerp(CurrA, 0, 5 * Time.deltaTime);
                }
                else if(!Radio)
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
                        SendTCPData(_packet);
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
                        _packet.Write(instance.myId);
                        SendTCPData(_packet);
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
                HostAServer(PortsToHostOn);
            }
        }


        public static void DedicatedServerUpdate()
        {
            if(GameManager.m_PlayerManager != null)
            {
                GameManager.m_PlayerManager.m_God = true;
                GameManager.m_PlayerManager.m_Ghost = true;
                if(GameManager.m_PlayerVoice == true)
                {
                    GameManager.m_PlayerVoice.enabled = false;
                }
                GameManager.m_PlayerObject.transform.position = new Vector3(0, -300, 0);
                IsDead = true;
                GameAudioManager.SetRTPCValue(AK.GAME_PARAMETERS.GLOBALVOLUME, 0 * 100f, (GameObject)null);

                int onServer = 0;

                for (int i = 1; i <= Server.MaxPlayers; i++)
                {
                    if (Server.clients[i].IsBusy() == true)
                    {
                        onServer = onServer + 1;
                    }
                }
            }
        }

        public static FireSourcesSync DebugFireSource = null;

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
        public static void FakeDropItem(DroppedGearItemDataPacket GearData, bool JustLoad = false)
        {
            if(iAmHost == true && !JustLoad)
            {
                MPSaveManager.AddGearVisual(GearData.m_LevelGUID, GearData);
            }

            if(GearData.m_LevelGUID == level_guid)
            {
                FakeDropItem(GearData.m_GearID, GearData.m_Position, GearData.m_Rotation, GearData.m_Hash, GearData.m_Extra);
            }
        }


        public static void FakeDropItem(int GearID, Vector3 v3, Quaternion rot, int Hash, ExtraDataForDroppedGear extra)
        {
            string gearName = "";
            if (GearID == -1)
            {
                gearName = extra.m_GearName;
            }else{
                gearName = GetGearNameByID(GearID);
            }

            if(DroppedGearsObjs.ContainsKey(Hash) == true)
            {
                MelonLogger.Msg(ConsoleColor.Red,"Gear with hash " + Hash + " already exist!");
                return;
            }

            GameObject reference = GetGearItemObject(gearName);

            if(reference == null)
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
                            if(obj.GetComponent<MeshSwapItem>() != null)
                            {
                                obj.GetComponent<MeshSwapItem>().m_MeshObjOpened.SetActive(true);
                                obj.GetComponent<MeshSwapItem>().m_MeshObjUnopened.SetActive(false);
                                UnityEngine.Object.Destroy(obj.GetComponent<MeshSwapItem>());
                            }
                        }
                    }
                    _DisName = GI.m_LocalizedDisplayName.Text();
                    UnityEngine.Object.Destroy(obj.GetComponent<GearItem>());
                }
                if (obj.GetComponent<Bed>() != null)
                {
                    if(extra.m_Variant == 1)
                    {
                        obj.GetComponent<Bed>().SetState(BedRollState.Placed);
                        _FakeBed = true;
                    }
                    UnityEngine.Object.Destroy(obj.GetComponent<Bed>());
                }
                if(obj.GetComponent<KeroseneLampItem>() != null)
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
                        }else{
                            Lamp.m_LightIndoor.enabled = true;
                            Lamp.m_LightIndoorCore.enabled = true;
                            Lamp.m_LightOutdoor.enabled = false;
                        }

                        int minutesLeft = extra.m_GoalTime - MinutesFromStartServer;
                        if (minutesLeft <= 0)
                        {
                            obj.transform.GetChild(0).gameObject.SetActive(false);
                        }else{
                            obj.transform.GetChild(0).gameObject.SetActive(true);
                        }

                        TrackableDroppedGearsObjs.Add(Hash, obj);
                    }
                }

                //foreach (var comp in obj.GetComponents<Component>())
                //{
                //    if (!(comp is Transform) && !(comp is DroppedGearDummy))
                //    {
                //        UnityEngine.Object.Destroy(comp);
                //    }
                //}
                if (obj.GetComponent<CookingPotItem>() != null)
                {
                    UnityEngine.Object.Destroy(obj.GetComponent<CookingPotItem>());
                }
                if (obj.GetComponent<EvolveItem>() != null)
                {
                    UnityEngine.Object.Destroy(obj.GetComponent<EvolveItem>());
                }
                if (obj.GetComponent<FlareGunRoundItem>() != null)
                {
                    UnityEngine.Object.Destroy(obj.GetComponent<FlareGunRoundItem>());
                }
                if (obj.GetComponent<SnareItem>() != null)
                {
                    int stateToSent = 0;
                    if(extra.m_Variant == 4)
                    {
                        stateToSent = 1;
                    }else{
                        stateToSent = extra.m_Variant;
                    }
                    
                    obj.GetComponent<SnareItem>().SetState((SnareState)stateToSent);
                    UnityEngine.Object.Destroy(obj.GetComponent<SnareItem>());
                }
                if (obj.GetComponent<Inspect>() != null)
                {
                    UnityEngine.Object.Destroy(obj.GetComponent<Inspect>());
                }
                if (obj.GetComponent<PlaceableItem>() != null)
                {
                    UnityEngine.Object.Destroy(obj.GetComponent<PlaceableItem>());
                }
                if (obj.GetComponent<Harvest>() != null)
                {
                    UnityEngine.Object.Destroy(obj.GetComponent<Harvest>());
                }
                if (obj.GetComponent<BodyHarvest>() != null)
                {
                    UnityEngine.Object.Destroy(obj.GetComponent<BodyHarvest>());
                }
                if (obj.GetComponent<KeroseneLampItem>() != null)
                {
                    UnityEngine.Object.Destroy(obj.GetComponent<KeroseneLampItem>());
                }
                
                DroppedGearDummy DGD = obj.AddComponent<DroppedGearDummy>();
                DGD.m_SearchKey = Hash;
                DGD.m_Extra = extra;
                DGD.m_LocalizedDisplayName = _DisName;
                if (_FakeBed)
                {
                    obj.AddComponent<MyMod.FakeBed>();
                }
                obj.SetActive(true);
                DroppedGearsObjs.Add(Hash, obj);
            }
            else
            {
                MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
            }
        }

        public static void AddDroppedGear(int GearID, int Hash, string DataProxy, string Scene, ExtraDataForDroppedGear extra)
        {
            SlicedJsonDroppedGear element = new SlicedJsonDroppedGear();
            if (GearID == -1)
            {
                element.m_GearName = extra.m_GearName;
            }else{
                element.m_GearName = GetGearNameByID(GearID);
            }
            element.m_Json = DataProxy;
            element.m_Extra = extra;
            MPSaveManager.AddGearData(Scene, Hash, element);
        }

        public static void PlaceDroppedGear(GameObject obj)
        {
            string OriginalName = obj.name;
            string GearName = CloneTrimer(OriginalName).ToLower();
            int GearID = GetGearIDByName(GearName);
            string GiveGear = "";
            if(GearID == -1)
            {
                GiveGear = CloneTrimer(OriginalName);
            }


            Vector3 v3 = obj.transform.position;
            Quaternion rot = obj.transform.rotation;
            int SearchKey = 0;
            string lvlKey = level_guid;

            if (obj.GetComponent<DroppedGearDummy>() != null)
            {
                SearchKey = obj.GetComponent<DroppedGearDummy>().m_SearchKey;
            }else{
                //MelonLogger.Msg(ConsoleColor.Red, "DroppedGearDummy is not exist by somereason...");


                GearItem GI = obj.GetComponent<GearItem>();

                if (GI != null && !GI.m_BeenInPlayerInventory && !ServerConfig.m_DuppedSpawns)
                {
                    DropFakeOnLeave DFL = obj.AddComponent<DropFakeOnLeave>();
                    DFL.m_OldPossition = obj.transform.position;
                    DFL.m_OldRotation = obj.transform.rotation;
                    MelonLogger.Msg("Trying place pre-spawned gear, so dropping fake on cancle");
                }

                return;
            }

            MelonLogger.Msg("Searching for "+ GearName +" with hash "+ SearchKey +" ID "+ GearID);

            if (sendMyPosition == true)
            {
                DoPleaseWait("Downloading Gear", "Please wait..");
                using (Packet _packet = new Packet((int)ClientPackets.REQUESTPLACE))
                {
                    _packet.Write(SearchKey);
                    _packet.Write(lvlKey);
                    SetRepeatPacket(ResendPacketType.Gear);
                    SendTCPData(_packet);
                }
                return;
            }

            SlicedJsonDroppedGear DataProxy = MPSaveManager.RequestSpecificGear(SearchKey, level_guid, true);
            if(DataProxy != null)
            {
                MelonLogger.Msg("Found " + SearchKey);

                string gearName = "";

                if (GearID == -1)
                {
                    gearName = GiveGear;
                }else{
                    gearName = GetGearNameByID(GearID);
                }
                GameObject reference = GetGearItemObject(gearName);

                if (reference == null)
                {
                    MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
                    return;
                }

                GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(reference, v3, rot);

                ExtraDataForDroppedGear Extra = obj.GetComponent<DroppedGearDummy>().m_Extra;

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
                newGear.GetComponent<GearItem>().Deserialize(DataProxy.m_Json);
                newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;

                DropFakeOnLeave DFL = newGear.AddComponent<DropFakeOnLeave>();
                DFL.m_OldPossition = newGear.gameObject.transform.position;
                DFL.m_OldRotation = newGear.gameObject.transform.rotation;
                newGear.GetComponent<GearItem>().PlayPickUpClip();
                GameManager.GetPlayerManagerComponent().StartPlaceMesh(newGear, PlaceMeshFlags.None);
                DroppedGearsObjs.Remove(SearchKey);
                TrackableDroppedGearsObjs.Remove(SearchKey);
                UnityEngine.Object.DestroyImmediate(obj);
                ServerSend.PICKDROPPEDGEAR(0, SearchKey, true);
            }else{
                MelonLogger.Msg("Gear with hash " + SearchKey + " is missing!");
            }
        }

        public static void UseFakeBed(GameObject obj)
        {
            string OriginalName = obj.name;
            string GearName = CloneTrimer(OriginalName).ToLower();
            int GearID = GetGearIDByName(GearName);
            Vector3 v3 = obj.transform.position;
            Quaternion rot = obj.transform.rotation;
            int SearchKey = 0;
            string lvlKey = level_guid;

            if (obj.GetComponent<DroppedGearDummy>() != null)
            {
                SearchKey = obj.GetComponent<DroppedGearDummy>().m_SearchKey;
            }else{
                MelonLogger.Msg(ConsoleColor.Red, "DroppedGearDummy by somereason...");
                return;
            }

            MelonLogger.Msg("Searching for " + GearName + " with hash " + SearchKey + " ID " + GearID);

            string gearName;
            if (GearID != -1)
            {
                gearName = GetGearNameByID(GearID);
            }else{
                gearName = GearName;
            }
            GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject(gearName), v3, rot);
            newGear.name = CloneTrimer(newGear.name);
            newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;
            newGear.GetComponent<GearItem>().m_Bed.SetState(BedRollState.Placed);
            newGear.AddComponent<FakeBedDummy>().m_LinkedFakeObject = obj;
            GameManager.GetPlayerManagerComponent().ProcessBedInteraction(newGear.GetComponent<GearItem>().m_Bed);
        }

        public static void PickupDroppedGear(GameObject obj)
        {
            string OriginalName = obj.name;
            string GearName = CloneTrimer(OriginalName).ToLower();
            int GearID = GetGearIDByName(GearName);
            string GearToGive = "";

            if(GearID == -1)
            {
                GearToGive = CloneTrimer(OriginalName);
            }

            if(GearName == "gear_technicalbackpack" && GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_TechnicalBackpack"))
            {
                HUDMessage.AddMessage("You already have technical backpack!");
                return;
            }

            Vector3 v3 = obj.transform.position;
            Quaternion rot = obj.transform.rotation;
            int SearchKey = 0;

            if (obj.GetComponent<DroppedGearDummy>() != null)
            {
                SearchKey = obj.GetComponent<DroppedGearDummy>().m_SearchKey;
            }else{
                MelonLogger.Msg(ConsoleColor.Red, "DroppedGearDummy by somereason...");
                return;
            }

            MelonLogger.Msg("Searching for "+ GearName + " with hash " + SearchKey + " ID " + GearID);

            if (sendMyPosition == true)
            {
                DoPleaseWait("Please wait...", "Downloading gear...");
                using (Packet _packet = new Packet((int)ClientPackets.REQUESTPICKUP))
                {
                    _packet.Write(SearchKey);
                    _packet.Write(level_guid);
                    SetRepeatPacket(ResendPacketType.Gear);
                    SendTCPData(_packet);
                }
                return;
            }

            SlicedJsonDroppedGear DataProxy = MPSaveManager.RequestSpecificGear(SearchKey, level_guid, true);

            if(DataProxy != null)
            {
                MelonLogger.Msg("Found " + SearchKey);
                string gearName = "";

                if (GearID == -1)
                {
                    gearName = GearToGive;
                }else{
                    gearName = GetGearNameByID(GearID);
                }

                GameObject reference = GetGearItemObject(gearName);

                if (reference == null)
                {
                    MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
                    return;
                }

                GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(reference, v3, rot);

                if (newGear == null)
                {
                    MelonLogger.Msg(ConsoleColor.Red, "Gear prefab with name " + gearName + " not exist! Maybe you miss an modded item pack?");
                    return;
                }

                newGear.name = CloneTrimer(newGear.name);
                DroppedGearDummy DGD = obj.GetComponent<DroppedGearDummy>();
                if (newGear.GetComponent<KeroseneLampItem>() != null)
                {
                    int minutesDroped = MinutesFromStartServer - DGD.m_Extra.m_DroppedTime;
                    OverrideLampReduceFuel = minutesDroped;
                    MelonLogger.Msg(ConsoleColor.Cyan, "Lamp been dropped " + minutesDroped + " minutes");
                }

                newGear.GetComponent<GearItem>().Deserialize(DataProxy.m_Json);
                newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;

                DropFakeOnLeave DFL = newGear.AddComponent<DropFakeOnLeave>();
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
                }else{
                    GameManager.GetPlayerManagerComponent().ProcessPickupItemInteraction(newGear.GetComponent<GearItem>(), false, false);
                }
                PatchBookReadTime(newGear.GetComponent<GearItem>());

                DroppedGearsObjs.Remove(SearchKey);
                TrackableDroppedGearsObjs.Remove(SearchKey);
                UnityEngine.Object.DestroyImmediate(obj);
                ServerSend.PICKDROPPEDGEAR(0, SearchKey, true);
            }else{
                MelonLogger.Msg("Gear with hash " + SearchKey + " is missing!");
            }
        }

        public static void PickDroppedItem(int Hash, int Picker)
        {
            GameObject gearObj;
            DroppedGearsObjs.TryGetValue(Hash, out gearObj);
            if(gearObj != null)
            {
                DroppedGearsObjs.Remove(Hash);
                TrackableDroppedGearsObjs.Remove(Hash);
                UnityEngine.Object.DestroyImmediate(gearObj);
            }
            if (players[Picker] != null && players[Picker].GetComponent<MultiplayerPlayerAnimator>() != null)
            {
                players[Picker].GetComponent<MultiplayerPlayerAnimator>().Pickup();
            }
        }

        public static void SendDroppedItemToPicker(string DataProxy, int GiveItemTo, int SearchKey, int GearID, bool place, ExtraDataForDroppedGear Extra)
        {
            byte[] bytesToSlice = Encoding.UTF8.GetBytes(DataProxy);
            MelonLogger.Msg("Going to send gear "+ GearID + " to client "+ GiveItemTo +" bytes: "+ bytesToSlice.Length);

            if (bytesToSlice.Length > 500)
            {
                List<byte> BytesBuffer = new List<byte>();
                BytesBuffer.AddRange(bytesToSlice);

                while (BytesBuffer.Count >= 500)
                {
                    byte[] sliceOfBytes = BytesBuffer.GetRange(0, 499).ToArray();
                    BytesBuffer.RemoveRange(0, 499);

                    string jsonStringSlice = Encoding.UTF8.GetString(sliceOfBytes);
                    SlicedJsonData SlicedPacket = new SlicedJsonData();
                    SlicedPacket.m_GearName = level_guid;
                    SlicedPacket.m_SendTo = GearID;
                    SlicedPacket.m_Hash = SearchKey;
                    SlicedPacket.m_Str = jsonStringSlice;

                    if (BytesBuffer.Count != 0)
                    {
                        SlicedPacket.m_Last = false;
                    }else{
                        SlicedPacket.m_Last = true;
                        SlicedPacket.m_Extra = Extra;
                    }

                    if(place == false)
                    {
                        ServerSend.GETREQUESTEDITEMSLICE(GiveItemTo, SlicedPacket);
                    }else{
                        ServerSend.GETREQUESTEDFORPLACESLICE(GiveItemTo, SlicedPacket);
                    }
                }

                if (BytesBuffer.Count < 500 && BytesBuffer.Count != 0)
                {
                    byte[] LastSlice = BytesBuffer.GetRange(0, BytesBuffer.Count).ToArray();
                    BytesBuffer.RemoveRange(0, BytesBuffer.Count);

                    string jsonStringSlice = Encoding.UTF8.GetString(LastSlice);
                    SlicedJsonData SlicedPacket = new SlicedJsonData();
                    SlicedPacket.m_GearName = level_guid;
                    SlicedPacket.m_SendTo = GearID;
                    SlicedPacket.m_Hash = SearchKey;
                    SlicedPacket.m_Str = jsonStringSlice;
                    SlicedPacket.m_Last = true;
                    SlicedPacket.m_Extra = Extra;
                    if (place == false)
                    {
                        ServerSend.GETREQUESTEDITEMSLICE(GiveItemTo, SlicedPacket);
                    }else{
                        ServerSend.GETREQUESTEDFORPLACESLICE(GiveItemTo, SlicedPacket);
                    }
                }
            }else{
                SlicedJsonData SlicedPacket = new SlicedJsonData();
                SlicedPacket.m_GearName = level_guid;
                SlicedPacket.m_SendTo = GearID;
                SlicedPacket.m_Hash = SearchKey;
                SlicedPacket.m_Str = DataProxy;
                SlicedPacket.m_Last = true;
                SlicedPacket.m_Extra = Extra;
                if (place == false)
                {
                    ServerSend.GETREQUESTEDITEMSLICE(GiveItemTo, SlicedPacket);
                }else{
                    ServerSend.GETREQUESTEDFORPLACESLICE(GiveItemTo, SlicedPacket);
                }
            }
        }

        public static void ClientTryPickupItem(int Hash, int sendTo, string Scene, bool place)
        {
            GameObject gearObj;
            DroppedGearsObjs.TryGetValue(Hash, out gearObj);
            if (gearObj != null)
            {
                DroppedGearsObjs.Remove(Hash);
                TrackableDroppedGearsObjs.Remove(Hash);
                UnityEngine.Object.DestroyImmediate(gearObj);
            }
            ServerSend.PICKDROPPEDGEAR(sendTo, Hash, true);
            SlicedJsonDroppedGear DataProxy = MPSaveManager.RequestSpecificGear(Hash, Scene, true);
            if(DataProxy != null)
            {
                MelonLogger.Msg("Found gear with hash " + Hash);
                SendDroppedItemToPicker(DataProxy.m_Json, sendTo, Hash, GetGearIDByName(DataProxy.m_GearName), place, DataProxy.m_Extra);
                if (players[sendTo] != null && players[sendTo].GetComponent<MultiplayerPlayerAnimator>() != null)
                {
                    players[sendTo].GetComponent<MultiplayerPlayerAnimator>().Pickup();
                }
            }else{
                MelonLogger.Msg("Client requested gear we have not data for, so gear most likely is missing. Gear hash " + Hash);
                ServerSend.GEARNOTEXIST(sendTo, true);
            }
        }

        public static GameObject DiagnosisDummy = null;
        public static int CurePlayerID = 0;

        public class AffictionSync
        {
            public int m_Type = 0;
            public int m_Location = 0;
            public string m_Case = "";
            public bool m_ShouldBeTreated = true;
        }

        public static List<AffictionSync> BuildMyAfflictionList()
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
            List<AffictionSync> ToReturn = new List<AffictionSync>();
            for (int index = 0; index < Affs.Count; ++index)
            {
                AffictionSync AffSync = new AffictionSync();
                AffSync.m_Type = (int)Affs[index].m_AfflictionType;
                AffSync.m_Location = (int)Affs[index].m_Location;
                if(Affs[index].m_Cause == null)
                {
                    AffSync.m_Case = "Unknown";
                }else{
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
            List<AffictionSync> Affs = BuildMyAfflictionList();

            if(iAmHost == true)
            {
                ServerSend.SENDMYAFFLCTIONS(PlayerSendTo, Affs, hp, 0);
            }
            if(sendMyPosition == true)
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
                    SendTCPData(_packet);
                }
            }
        }

        public static void OtherPlayerCuredMyAffiction(AffictionSync Aff)
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
                    if(GameManager.GetSprainedAnkleComponent().m_Locations[i] == Aff.m_Location)
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
                    if (GameManager.GetSprainPainComponent().m_ActiveInstances[i].m_Location == (AfflictionBodyArea) Aff.m_Location)
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
        public static void SendCureAffliction(AffictionSync Aff)
        {
            if(DebugDiagnosis == true)
            {
                OtherPlayerCuredMyAffiction(Aff);
            }else{
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
                        SendTCPData(_packet);
                    }
                }
            }
        }

        public static void TryToBorrowThing(int PlayerID)
        {
            if (LastBorrowable && LastBorrowable.m_Obj)
            {
                LastBorrowable.m_Obj.SetActive(false);
                if (iAmHost == true)
                {
                    ServerSend.TRYBORROWGEAR(PlayerID, 0, LastBorrowable.m_GearName);
                }
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.TRYBORROWGEAR))
                    {
                        _packet.Write(PlayerID);
                        _packet.Write(LastBorrowable.m_GearName);
                        SendTCPData(_packet);
                    }
                }
            }
        }

        public static void TryToCheckPlayer(int PlayerID) //Examine
        {
            DoPleaseWait("Diagnosis player", "Please wait...");
            MelonLogger.Msg("Trying diagnosis client "+ PlayerID);
            if(iAmHost == true)
            {
                ServerSend.TRYDIAGNISISPLAYER(PlayerID, instance.myId);
            }
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.TRYDIAGNISISPLAYER))
                {
                    _packet.Write(PlayerID);
                    SendTCPData(_packet);
                }
            }
        }

        public static bool DebugDiagnosis = false;

        public static void CheckOtherPlayer(List<AffictionSync> Affs, int PlayerID, float health)
        {
            if(DiagnosisDummy != null)
            {
                UnityEngine.Object.DestroyImmediate(DiagnosisDummy);
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
                }else{
                    HasAnyBadOnes = true;
                }
                npcAff.m_Definition = definitionByType;
                npcAff.m_Location = (AfflictionBodyArea)Affs[i].m_Location;
                npcAff.m_CauseLocId = Affs[i].m_Case;

                m_NPCaff.m_Afflictions.Add(npcAff);
                if(definitionByType.m_AfflictionType == AfflictionType.BloodLoss)
                {
                    HasBloodLoss = true;
                }
            }

            m_NPCcon.m_CurrentHP = health;
            m_NPCcon.m_MaxHP = 100;
            Panel_Diagnosis Panel = InterfaceManager.m_Panel_Diagnosis;
            RemovePleaseWait();
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
                }else if(HasBloodLoss) // Has blood loss
                {
                    Speach = "PLAY_SNDVOSMMAC1820"; // You’re losing blood. Just stay awake. We’ll get there
                }else{ // Anything else
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

        public class DictionaryElementToReNew
        {
            public int m_Key;
            public SlicedJsonDroppedGear m_Val;
            public DroppedGearItemDataPacket m_Val2;
        }

        public static void ModifyDynamicGears(string Scene)
        {
            Dictionary<int, DroppedGearItemDataPacket> LoadedVisual = MPSaveManager.LoadDropVisual(Scene);
            Dictionary<int, SlicedJsonDroppedGear> LoadedData = MPSaveManager.LoadDropData(Scene);

            if(LoadedVisual == null || LoadedData == null)
            {
                return;
            }

            // Here goes kinda crappy code!!!!!
            List<DictionaryElementToReNew> Buff = new List<DictionaryElementToReNew>();
            List<Vector3> RabbitsBuff = new List<Vector3>();

            foreach (var cur in LoadedData) //Checking if we have any dynamic gears we need to modify!
            {
                int curKey = cur.Key;
                SlicedJsonDroppedGear dat = cur.Value;
                if (dat.m_GearName.Contains("gear_snare") == true && (dat.m_Extra.m_Variant == 1 || dat.m_Extra.m_Variant == 4)) // If is placed snare
                {
                    int minutesPlaced = MinutesFromStartServer - dat.m_Extra.m_DroppedTime;
                    int NeedToBePlaced = 720;
                    int minutesLeft = NeedToBePlaced - minutesPlaced + 1;
                    float ChanceToCatch = 50;
                    float ChanceToBreak = 15;
                    if (minutesLeft <= 0) // If snare ready to roll random
                    {
                        if (dat.m_Extra.m_Variant == 4 || Utils.RollChance(ChanceToCatch))
                        {
                            dat.m_Extra.m_Variant = (int)SnareState.WithRabbit; // New visual state!
                            dat.m_Extra.m_GoalTime = -1; // So it won't reload itself.

                            GearItemSaveDataProxy DummyGear = Utils.DeserializeObject<GearItemSaveDataProxy>(dat.m_Json);
                            SnareItem DummySnare = Utils.DeserializeObject<SnareItem>(DummyGear.m_SnareItemSerialized);
                            DummySnare.m_State = (SnareState)dat.m_Extra.m_Variant;
                            DummyGear.m_SnareItemSerialized = DummySnare.Serialize();
                            dat.m_Json = Utils.SerializeObject(DummyGear);

                            RabbitsBuff.Add(DummyGear.m_Position); // Add request on rabbit on snare position
                        }else{
                            if (Utils.RollChance(ChanceToBreak))
                            {
                                dat.m_Extra.m_Variant = (int)SnareState.Broken; // New visual state!
                                dat.m_Extra.m_GoalTime = -1; // So it won't reload itself.

                                GearItemSaveDataProxy DummyGear = Utils.DeserializeObject<GearItemSaveDataProxy>(dat.m_Json);
                                SnareItem DummySnare = Utils.DeserializeObject<SnareItem>(DummyGear.m_SnareItemSerialized);
                                DummySnare.m_State = (SnareState)dat.m_Extra.m_Variant;
                                DummyGear.m_SnareItemSerialized = DummySnare.Serialize();
                                dat.m_Json = Utils.SerializeObject(DummyGear);
                            }else{
                                // No new visual state, but reseting time player should to wait for.
                                dat.m_Extra.m_DroppedTime = MinutesFromStartServer;
                                dat.m_Extra.m_GoalTime = MinutesFromStartServer + NeedToBePlaced;
                            }
                        }
                        DictionaryElementToReNew newGear = new DictionaryElementToReNew();
                        newGear.m_Key = curKey;
                        newGear.m_Val = dat;

                        DroppedGearItemDataPacket Visual;
                        if(LoadedVisual.TryGetValue(curKey, out Visual))
                        {
                            Visual.m_Extra = dat.m_Extra;
                            newGear.m_Val2 = Visual;
                        }
                        Buff.Add(newGear); //Adding to buffer snare to re-add them.
                    }
                }
            }

            if (Buff.Count > 0) //If buffer contains anything, we need to remove old gears and update them with modified ones
            {
                for (int i = 0; i < Buff.Count; i++)
                {
                    int Key = Buff[i].m_Key;
                    LoadedData.Remove(Key);
                    LoadedData.Add(Key, Buff[i].m_Val);

                    if(Buff[i].m_Val2 != null)
                    {
                        LoadedVisual.Remove(Key);
                        LoadedVisual.Add(Key, Buff[i].m_Val2);
                    }
                }
            }
            if (RabbitsBuff.Count > 0) //If buffer contains anything, we need to spawn rabbits
            {
                for (int i = 0; i < RabbitsBuff.Count; i++)
                {
                    SlicedJsonDroppedGear Rabbit = new SlicedJsonDroppedGear();
                    Rabbit.m_GearName = "gear_rabbitcarcass";
                    Rabbit.m_Extra.m_DroppedTime = MinutesFromStartServer;
                    Rabbit.m_Extra.m_Dropper = "Nature";
                    string RabbitJson = "";
                    GameObject reference = GetGearItemObject("gear_rabbitcarcass");
                    int SearchKey = 0;

                    if (reference != null)
                    {
                        Vector3 v3 = RabbitsBuff[i];
                        Quaternion rot = new Quaternion(0, 0, 0, 0);

                        GameObject obj = UnityEngine.Object.Instantiate<GameObject>(reference, v3, rot);
                        GearItem gi = obj.GetComponent<GearItem>();
                        gi.SkipSpawnChanceRollInitialDecayAndAutoEvolve();
                        BodyHarvest bh = obj.GetComponent<BodyHarvest>();
                        if (bh == null)
                        {
                            bh = obj.AddComponent<BodyHarvest>();
                        }
                        obj.name = "gear_rabbitcarcass";
                        gi.m_CurrentHP = bh.GetCondition() / 100f * gi.m_MaxHP;

                        RabbitJson = obj.GetComponent<GearItem>().Serialize();

                        int GearID = GetGearIDByName("gear_rabbitcarcass");
                        int hashGearID = GearID.GetHashCode();
                        int hashV3 = v3.GetHashCode();
                        int hashRot = rot.GetHashCode();
                        int hashLevelKey = Scene.GetHashCode();
                        SearchKey = hashGearID + hashV3 + hashRot + hashLevelKey;
                        UnityEngine.Object.DestroyImmediate(obj);
                    }

                    DroppedGearItemDataPacket RabbitVisual = new DroppedGearItemDataPacket();
                    RabbitVisual.m_Extra = Rabbit.m_Extra;
                    RabbitVisual.m_GearID = GetGearIDByName("gear_rabbitcarcass");
                    RabbitVisual.m_Hash = SearchKey;
                    RabbitVisual.m_LevelGUID = Scene;
                    RabbitVisual.m_Position = RabbitsBuff[i];
                    RabbitVisual.m_Rotation = new Quaternion(0,0,0,0);
                    Rabbit.m_Json = RabbitJson;
                    LoadedData.Add(SearchKey, Rabbit);
                    LoadedVisual.Add(SearchKey, RabbitVisual);
                }
            }
            if (MPSaveManager.RecentData.ContainsKey(Scene))
            {
                MPSaveManager.RecentData.Remove(Scene);
            }
            MPSaveManager.RecentData.Add(Scene, LoadedData);
            if (MPSaveManager.RecentVisual.ContainsKey(Scene))
            {
                MPSaveManager.RecentVisual.Remove(Scene);
            }
            MPSaveManager.RecentVisual.Add(Scene, LoadedVisual);
        }

        public class BodyHarvestUnits
        {
            public float m_Meat = 0;
            public int m_Guts = 0;
        }

        public static BodyHarvestUnits GetBodyHarvestUnits(string name)
        {
            BodyHarvestUnits bh = new BodyHarvestUnits();
            if (name == "WILDLIFE_Wolf")
            {
                bh.m_Meat = UnityEngine.Random.Range(3, 6);
                bh.m_Guts = 2;
            }
            else if(name == "WILDLIFE_Wolf_grey")
            {
                bh.m_Meat = UnityEngine.Random.Range(4, 7);
                bh.m_Guts = 2;
            }
            else if (name == "WILDLIFE_Bear")
            {
                bh.m_Meat = UnityEngine.Random.Range(25, 40);
                bh.m_Guts = 10;
            }
            else if (name == "WILDLIFE_Stag")
            {
                bh.m_Meat = UnityEngine.Random.Range(8, 10);
                bh.m_Guts = 2;
            }
            else if (name == "WILDLIFE_Rabbit")
            {
                bh.m_Meat = UnityEngine.Random.Range(0.75f, 1.5f);
                bh.m_Guts = 1;
            }
            else if (name == "WILDLIFE_Moose")
            {
                bh.m_Meat = UnityEngine.Random.Range(30, 45);
                bh.m_Guts = 12;
            }
            return bh;
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

        public static void SendDropItem(GearItem gear, int nums = 0, int total = 0, bool samepose = false, int variant = 0)
        {            
            if(gear != null && gear.gameObject != null)
            {
                GameObject obj = gear.gameObject;

                if (samepose == false)
                {
                    gear.StickToGroundAtPlayerFeet(GameManager.GetPlayerTransform().position);
                }else{
                    if(obj.GetComponent<DropFakeOnLeave>() != null)
                    {
                        DropFakeOnLeave DFL = obj.GetComponent<DropFakeOnLeave>();
                        obj.transform.position = DFL.m_OldPossition;
                        obj.transform.rotation = DFL.m_OldRotation;
                    }
                }

                Vector3 v3 = gear.gameObject.transform.position;
                Quaternion rot = gear.gameObject.transform.rotation;

                string OriginalName = obj.name;
                string GearName = CloneTrimer(OriginalName).ToLower();
                int GearID = GetGearIDByName(GearName);
                string GearGiveName = "";
                if(GearID == -1)
                {
                    GearGiveName = CloneTrimer(OriginalName);
                }

                int hashGearID = GearID.GetHashCode();
                int hashV3 = v3.GetHashCode();
                int hashRot = rot.GetHashCode();
                int hashLvlGUID = level_guid.GetHashCode();

                int SearchKey = hashGearID + hashV3 + hashRot + hashLvlGUID;
                string LevelKey = level_guid;

                //MelonLogger.Msg("hashGearID " + hashGearID);
                //MelonLogger.Msg("hashV3 " + hashV3);
                //MelonLogger.Msg("hashRot " + hashRot);
                //MelonLogger.Msg("hashLvl " + hashLvl);
                //MelonLogger.Msg("hashLvlGUID " + hashLvlGUID);
                string DataProxy;
                if (nums > 0)
                {
                    if(gear.m_StackableItem != null)
                    {
                        gear.m_StackableItem.m_Units = nums;
                    }
                }

                DataProxy = gear.Serialize();

                int NeedToDry = 0;
                int MinuteToSkip = 0;

                if(gear.m_EvolveItem != null)
                {
                    if(gear.m_EvolveItem.CanEvolve() == true)
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
                    }else{
                        NeedToDry = -1;
                    }
                }

                if(gear.m_SnareItem != null)
                {
                    variant = (int)gear.m_SnareItem.m_State;

                    if(variant == 1)
                    {                        
                        if (SnareCanTrap(v3, gear.m_SnareItem.m_RabbitPrefab.name) == true)
                        {
                            NeedToDry = 720;
                            if(GameManager.GetFeatExpertTrapper().IsUnlockedAndEnabled() == true)
                            {
                                variant = 4;
                            }
                        }else{
                            NeedToDry = -1;
                        }
                    }
                }

                MelonLogger.Msg("Dropping "+ GearName + " ID "+ GearID +" Hash "+ SearchKey+" variant "+ variant);

                if (nums == 0)
                {
                    UnityEngine.Object.Destroy(obj);
                }else{
                    if (gear.m_StackableItem != null)
                    {
                        gear.m_StackableItem.m_Units = total-nums;
                        GameManager.GetInventoryComponent().AddGear(obj);
                    }
                }

                DroppedGearItemDataPacket SyncData = new DroppedGearItemDataPacket();
                SyncData.m_GearID = GearID;
                SyncData.m_Position = v3;
                SyncData.m_Rotation = rot;
                SyncData.m_LevelID = levelid;
                SyncData.m_LevelGUID = level_guid;
                SyncData.m_Hash = SearchKey;

                ExtraDataForDroppedGear Extra = new ExtraDataForDroppedGear();
                Extra.m_Dropper = MyChatName;
                Extra.m_DroppedTime = MinutesFromStartServer-MinuteToSkip;
                Extra.m_Variant = variant;
                if(GearGiveName != "")
                {
                    Extra.m_GearName = GearGiveName;
                }

                if (NeedToDry != 0 && NeedToDry != -1)
                {
                    Extra.m_GoalTime = MinutesFromStartServer + NeedToDry;
                }else{
                    Extra.m_GoalTime = NeedToDry;
                }

                if (gear.m_KeroseneLampItem != null)
                {
                    if(variant == 1)
                    {
                        float fuel = gear.m_KeroseneLampItem.m_CurrentFuelLiters;
                        float fuelPerMinute = gear.m_KeroseneLampItem.m_FuelBurnLitersPerHour/60;
                        float minuteLeft = fuel / fuelPerMinute;
                        
                        Extra.m_GoalTime = MinutesFromStartServer + (int)Math.Round(minuteLeft);
                    }else{
                        Extra.m_GoalTime = 0;
                    }
                }

                SyncData.m_Extra = Extra;

                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.DROPITEM))
                    {
                        _packet.Write(SyncData);
                        SendTCPData(_packet);
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
                            SlicedJsonData SlicedPacket = new SlicedJsonData();
                            SlicedPacket.m_GearName = LevelKey;
                            SlicedPacket.m_SendTo = GearID;
                            SlicedPacket.m_Hash = SearchKey;
                            SlicedPacket.m_Str = jsonStringSlice;

                            if (BytesBuffer.Count != 0)
                            {
                                SlicedPacket.m_Last = false;
                            }else{
                                SlicedPacket.m_Last = true;
                            }
                            using (Packet _packet = new Packet((int)ClientPackets.GOTDROPSLICE))
                            {
                                _packet.Write(SlicedPacket);
                                SendTCPData(_packet);
                            }
                        }

                        if (BytesBuffer.Count < 500 && BytesBuffer.Count != 0)
                        {
                            byte[] LastSlice = BytesBuffer.GetRange(0, BytesBuffer.Count).ToArray();
                            BytesBuffer.RemoveRange(0, BytesBuffer.Count);

                            string jsonStringSlice = Encoding.UTF8.GetString(LastSlice);
                            SlicedJsonData SlicedPacket = new SlicedJsonData();
                            SlicedPacket.m_GearName = LevelKey;
                            SlicedPacket.m_SendTo = GearID;
                            SlicedPacket.m_Hash = SearchKey;
                            SlicedPacket.m_Str = jsonStringSlice;
                            SlicedPacket.m_Last = true;
                            SlicedPacket.m_Extra = Extra;

                            //MelonLogger.Msg(ConsoleColor.Yellow, "Sending slice " + SlicedPacket.m_Hash + " DATA: " + SlicedPacket.m_Str);
                            if (sendMyPosition == true)
                            {
                                using (Packet _packet = new Packet((int)ClientPackets.GOTDROPSLICE))
                                {
                                    _packet.Write(SlicedPacket);
                                    SendTCPData(_packet);
                                }
                            }
                        }
                    }else{
                        SlicedJsonData DropPacket = new SlicedJsonData();
                        DropPacket.m_GearName = LevelKey;
                        DropPacket.m_SendTo = GearID;
                        DropPacket.m_Hash = SearchKey;
                        DropPacket.m_Str = DataProxy;
                        DropPacket.m_Last = true;
                        DropPacket.m_Extra = Extra;
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.GOTDROPSLICE))
                            {
                                _packet.Write(DropPacket);
                                SendTCPData(_packet);
                            }
                        }
                    }
                }
                if(iAmHost == true)
                {
                    AddDroppedGear(GearID, SearchKey, DataProxy, LevelKey, Extra);
                    ServerSend.DROPITEM(0, SyncData, true);
                    FakeDropItem(SyncData);
                }
            }
        }

        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        public static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
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
                if(OpCl.gameObject != null)
                {
                    Vector3 doorV3 = OpCl.gameObject.transform.position;
                    Vector3 playerV3 = GameManager.GetPlayerObject().transform.position;
                    float dis = Vector3.Distance(playerV3, doorV3);
                    if(dis > 50)
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
            if(OpenCloseManager.s_ActiveOpenClosers != null && OpenablesObjs.Count < OpenCloseManager.s_ActiveOpenClosers.Count)
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
                        if(curObj.GetComponent<ObjectGuid>() != null)
                        {
                            _Guid = curObj.GetComponent<ObjectGuid>().Get();
                            
                            if(OpenablesObjs.ContainsKey(_Guid) == false)
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
                            }else{
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

                SendTCPData(_packet);
            }
        }

        public static void ChangeOpenableThingState(string Scene, string GUID, bool state)
        {
            OpenableThings.Remove(GUID);
            OpenableThings.Add(GUID, state);
            MelonLogger.Msg(ConsoleColor.Blue, "Openable things "+ GUID + " changed state to OpenIs="+ state);

            if(iAmHost == true)
            {
                ServerSend.USEOPENABLE(0, GUID, state, true);
                MPSaveManager.ChangeOpenableThingState(Scene, GUID, state);
            }
        }

        public static List<SlicedJsonData> CarefulSlicesBuffer = new List<SlicedJsonData>();

        public static void AddCarefulSlice(SlicedJsonData slice)
        {
            CarefulSlicesBuffer.Add(slice);
        }
        public static int CarefulSlicesSent = 0;
        public static void SendNextCarefulSlice()
        {
            if(CarefulSlicesBuffer.Count > 0)
            {
                SlicedJsonData slice = CarefulSlicesBuffer[0];
                using (Packet _packet = new Packet((int)ClientPackets.GOTCONTAINERSLICE))
                {
                    _packet.Write(slice);
                    SendUDPData(_packet);
                }
                CarefulSlicesBuffer.Remove(CarefulSlicesBuffer[0]);
                CarefulSlicesSent++;
            }else{
                MelonLogger.Msg("Finished sending all "+ CarefulSlicesSent+" slices");
                CarefulSlicesSent = 0;
                Container box = InterfaceManager.m_Panel_Container.m_Container;
                if (box != null)
                {
                    if (!box.Close())
                        return;
                    if (box.m_CloseAudio.Length == 0)
                        GameAudioManager.PlayGUIButtonBack();
                }
                RemovePleaseWait();
                GameManager.GetPlayerManagerComponent().MaybeRevealPolaroidDiscoveryOnClose();
                InterfaceManager.m_Panel_Container.Enable(false);
            }
        }

        public static void SendContainerData(string DataProxy, string LevelKey, string GUID, int SendTo = 0)
        {
            byte[] bytesToSlice = Encoding.UTF8.GetBytes(DataProxy);
            int Hash = GUID.GetHashCode();
            MelonLogger.Msg("Going to sent " + bytesToSlice.Length+"bytes"); 

            int CHUNK_SIZE = 1000;
            int SlicesSent = 0;

            if (bytesToSlice.Length > CHUNK_SIZE)
            {
                List<byte> BytesBuffer = new List<byte>();
                BytesBuffer.AddRange(bytesToSlice);

                while (BytesBuffer.Count >= CHUNK_SIZE)
                {
                    byte[] sliceOfBytes = BytesBuffer.GetRange(0, CHUNK_SIZE-1).ToArray();
                    BytesBuffer.RemoveRange(0, CHUNK_SIZE-1);

                    string jsonStringSlice = Encoding.UTF8.GetString(sliceOfBytes);
                    SlicedJsonData SlicedPacket = new SlicedJsonData();
                    SlicedPacket.m_GearName = LevelKey+"|"+GUID;
                    SlicedPacket.m_SendTo = 0;
                    SlicedPacket.m_Hash = Hash;
                    SlicedPacket.m_Str = jsonStringSlice;

                    if (BytesBuffer.Count != 0)
                    {
                        SlicedPacket.m_Last = false;
                    }else{
                        SlicedPacket.m_Last = true;
                    }

                    if(SendTo == 0)
                    {
                        //using (Packet _packet = new Packet((int)ClientPackets.GOTCONTAINERSLICE))
                        //{
                        //    _packet.Write(SlicedPacket);
                        //    SendUDPData(_packet);
                        //}
                        AddCarefulSlice(SlicedPacket);
                    }
                    else{
                        ServerSend.GOTCONTAINERSLICE(SendTo, SlicedPacket);
                    }
                    SlicesSent = SlicesSent + 1;
                }

                if (BytesBuffer.Count < CHUNK_SIZE && BytesBuffer.Count != 0)
                {
                    byte[] LastSlice = BytesBuffer.GetRange(0, BytesBuffer.Count).ToArray();
                    BytesBuffer.RemoveRange(0, BytesBuffer.Count);

                    string jsonStringSlice = Encoding.UTF8.GetString(LastSlice);
                    SlicedJsonData SlicedPacket = new SlicedJsonData();
                    SlicedPacket.m_GearName = LevelKey+"|"+GUID;
                    SlicedPacket.m_SendTo = 0;
                    SlicedPacket.m_Hash = Hash;
                    SlicedPacket.m_Str = jsonStringSlice;
                    SlicedPacket.m_Last = true;

                    if (SendTo == 0)
                    {
                        //using (Packet _packet = new Packet((int)ClientPackets.GOTCONTAINERSLICE))
                        //{
                        //    _packet.Write(SlicedPacket);
                        //    SendUDPData(_packet);
                        //} 
                        AddCarefulSlice(SlicedPacket);
                    }else{
                        ServerSend.GOTCONTAINERSLICE(SendTo, SlicedPacket);
                    }
                    SlicesSent = SlicesSent + 1;
                }
            }else{
                SlicedJsonData SlicedPacket = new SlicedJsonData();
                SlicedPacket.m_GearName = LevelKey + "|" + GUID;
                SlicedPacket.m_SendTo = 0;
                SlicedPacket.m_Hash = Hash;
                SlicedPacket.m_Str = DataProxy;
                SlicedPacket.m_Last = true;

                if (SendTo == 0)
                {
                    //using (Packet _packet = new Packet((int)ClientPackets.GOTCONTAINERSLICE))
                    //{
                    //    _packet.Write(SlicedPacket);
                    //    SendUDPData(_packet);
                    //}
                    AddCarefulSlice(SlicedPacket);
                }else{
                    ServerSend.GOTCONTAINERSLICE(SendTo, SlicedPacket);
                }
                SlicesSent = SlicesSent + 1;
            }

            if(iAmHost == true)
            {
                MelonLogger.Msg("Slices sent " + SlicesSent);
            }else{
                MelonLogger.Msg("Prepared " + SlicesSent+ " slices to send");
                MelonLogger.Msg("Starting send slices");
                SendNextCarefulSlice();
            }
        }

        public static void FinishOpeningFakeContainer(string CompressedData)
        {
            MelonLogger.Msg("Finish Opening Fake Container");
            string Data = "";
            if(CompressedData != "")
            {
                Data = DecompressString(CompressedData);
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
            }else{
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
                    SendTCPData(_packet);
                }
                return;
            }

            string CompressedData = MPSaveManager.LoadContainer(level_guid, boxGUID);
            string Data = "";
            if (CompressedData != "")
            {
                Data = DecompressString(CompressedData);
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
            }else{
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
            string CompressedData = CompressString(Data);
            bool IsEmpty = false;
            if (box.gameObject.GetComponent<DeathDropContainer>() != null)
            {
                if(box.m_Items != null && box.m_Items.Count <= 2)
                {
                    for (int i = box.m_Items.Count-1; i >= 0 ; i--)
                    {
                        GearItemObject item = box.m_Items[i];
                        if (item.m_GearItem && item.m_GearItem.m_WaterSupply && item.m_GearItem.m_WaterSupply.m_VolumeInLiters == 0)
                        {
                            box.RemoveGear(item.m_GearItem);
                        }
                    }
                }

                IsEmpty = box.IsEmpty();
            }
            box.DestroyAllGear();
            MelonLogger.Msg("[CloseFakeContainer] " + boxGUID + " Is Empty "+ IsEmpty);
            int Bags = GameManager.GetInventoryComponent().GetNumGearWithName("GEAR_TechnicalBackpack");
            if(Bags > 1)
            {                
                IL2CPP.List<GearItemObject> GI = GameManager.GetInventoryComponent().m_Items;
                for (int i = 0; i < GI.Count; i++)
                {
                    if(GI[i].m_GearItemName == "GEAR_TechnicalBackpack")
                    {
                        GI[i].m_GearItem.Drop(1);
                        break;
                    }
                }
            }

            if (IsEmpty)
            {
                MelonLogger.Msg("[CloseFakeContainer] Removing container");
                RemoveDeathContainer(boxGUID, level_guid);
                
                if (sendMyPosition)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.DEATHCREATEEMPTYNOW))
                    {
                        _packet.Write(boxGUID);
                        _packet.Write(level_guid);
                        SendTCPData(_packet);
                    }
                }else{
                    MPSaveManager.RemoveContainer(level_guid, boxGUID);
                }               
            }else{
                MelonLogger.Msg("[CloseFakeContainer] Saving container");
                if (sendMyPosition == true)
                {
                    DoPleaseWait("Please wait...", "Sending container data...");
                    SendContainerData(CompressedData, level_guid, boxGUID);
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
            }
            GameManager.GetPlayerManagerComponent().MaybeRevealPolaroidDiscoveryOnClose();
            InterfaceManager.m_Panel_Container.Enable(false);
        }

        public static void InitAudio()
        {
            Camera Cam = GameManager.GetMainCamera();

            if (Cam != null)
            {
                AudioListener pListener = Cam.GetComponent<AudioListener>();

                if (pListener == null)
                {
                    pListener = Cam.gameObject.AddComponent<AudioListener>();
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
            }else{
                API.m_ClientState = API.SkyCoopClientState.NONE;
            }
            API.m_MyClientID = instance.myId;
        }

        public static void BuildSleepScreen()
        {
            if (SleepingButtons == null)
            {
                SleepingButtons = InterfaceManager.m_Panel_Rest.gameObject.transform.GetChild(3).gameObject;
                WaitForSleepLable = UnityEngine.Object.Instantiate(SleepingButtons.transform.GetChild(2).GetChild(1).gameObject, InterfaceManager.m_Panel_Rest.gameObject.transform);
                UnityEngine.Object.Destroy(WaitForSleepLable.GetComponent<UILocalize>());
                WaitForSleepLable.GetComponent<UILabel>().text = "WAITING OTHER PLAYERS TO SLEEP";
                WaitForSleepLable.SetActive(false);
            }

            if (new_button == null)
            {
                new_button = UnityEngine.Object.Instantiate(InterfaceManager.m_Panel_Rest.m_SleepButton, InterfaceManager.m_Panel_Rest.m_SleepButton.transform.parent);

                if (new_button2 == null)
                {
                    new_button.transform.position = new Vector3(0, -0.59f, 0);
                }else{
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

        public static void BuildCanvasUIs()
        {
            if (uConsole.m_Instance != null && uConsole.m_Instance.gameObject != null && uConsole.m_Instance.gameObject.transform.childCount > 0 && uConsole.m_Instance.gameObject.transform.GetChild(0) != null)
            {
                MelonLogger.Msg("[UI] Got Canvas");
                UiCanvas = uConsole.m_Instance.gameObject.transform.GetChild(0).gameObject.GetComponent<Canvas>();

                GameObject LoadedAssets0 = LoadedBundle.LoadAsset<GameObject>("UI_GasMaskOverlay");
                GasMaskOverlay = GameObject.Instantiate(LoadedAssets0, UiCanvas.transform);
                GasMaskOverlay.SetActive(false);
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
                if(EmoteWheel != null)
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
            }
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
                    }else{
                        if (chatInput.text != "")
                        {
                            MultiplayerChatMessage message = new MultiplayerChatMessage();
                            message.m_By = MyChatName;
                            message.m_Type = 1;
                            message.m_Message = chatInput.text;
                            SendMessageToChat(message);
                            chatInput.text = "";
                        }
                        chatInput.gameObject.SetActive(false);
                        if (GameManager.GetPlayerManagerComponent().GetControlMode() == PlayerControlMode.Locked)
                        {
                            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Normal);
                        }
                    }
                }
                //if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Alpha7))
                //{
                //    Il2CppSystem.Collections.Generic.List<GearItem> GearList = new Il2CppSystem.Collections.Generic.List<GearItem>();
                //    GearList = Panel_WeaponPicker.GetPrioritizedWeaponList();

                //    for (int i = 0; i < GearList.Count; i++)
                //    {
                //        if (GearList[i].m_GearName == "GEAR_Revolver")
                //        {
                //            GearList.Remove(GearList[i]);
                //        }
                //    }
                //    if (GearList.Count > 0)
                //    {
                //        InterfaceManager.m_Panel_WeaponPicker.Enable(true, GearList, 1000);
                //    }else{
                //        HUDMessage.AddMessage("YOU HAVE NOT ANY MELEE WEAPONS!");
                //    }
                //}

                if (InterfaceManager.m_Panel_PauseMenu.isActiveAndEnabled && level_name != "MainMenu")
                {
                    if (InOnline())
                    {
                        if(MyLobby == "")
                        {
                            StatusObject.SetActive(true);
                        }else{
                            LobbyUI.SetActive(true);
                        }
                    }else{
                        StatusObject.SetActive(false);
                        LobbyUI.SetActive(false);
                    }
                }else{
                    if(level_name != "MainMenu")
                    {
                        LobbyUI.SetActive(false);
                    }
                    StatusObject.SetActive(false);
                }

                if (UseGasMaskOverlay)
                {
                    if (GasMaskOverlay && m_InterfaceManager && InterfaceManager.m_Panel_HUD && InterfaceManager.m_Panel_HUD.isActiveAndEnabled)
                    {
                        GasMaskOverlay.SetActive(true);
                    }else{
                        GasMaskOverlay.SetActive(false);
                    }
                }else{
                    GasMaskOverlay.SetActive(false);
                }
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
                }else{
                    //PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().Set(false);
                    //PublicSteamServer.SetActive(false);
                    PortsObject.SetActive(true);
                    SteamLobbyType.SetActive(false);
                }
            }
        }

        public static void DebugCrap()
        {
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
            }else{
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
                        cube.GetComponent<MeshRenderer>().SetMaterial(null);
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.BLOCK))
                            {
                                _packet.Write(cube.transform.position);
                                SendTCPData(_packet);
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
            }else{
                MyHasAxe = false;
            }

            EQCheck = MyHasRifle.ToString() + MyArrows.ToString() + MyFlares.ToString() + MyBlueFlares.ToString() + MyHasRevolver.ToString() + MyHasMedkit.ToString() + MyHasAxe.ToString();
            if(PreviousEquipmentCheck != EQCheck)
            {
                PreviousEquipmentCheck = EQCheck;
                SendMyEQ();
            }

            if (MyLightSourceName != MyLastLightSourceName)
            {
                if(SkipItemEvent > 0)
                {
                    SkipItemEvent--;
                }else{
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
                            SendTCPData(_packet);
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
                        SendTCPData(_packet);
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
                }else{

                    if (ModdedHandsBook == null)
                    {
                        MyLightSourceName = "";
                    }else{
                        if (!ModdedHandsBook.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Offscreen"))
                        {
                            MyLightSourceName = "Book";
                        }else{
                            MyLightSourceName = "";
                        }
                    }
                }
            }else{
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
            GearItemDataPacket GearDataPak = new GearItemDataPacket();
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
                }else{
                    waterGave = Liters;
                }
                GearDataPak.m_Water = waterGave;
            }else{
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
                        SendTCPData(_packet);
                    }
                }
            }else{
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
                        SlicedJsonData SlicedPacket = new SlicedJsonData();
                        SlicedPacket.m_GearName = _gear.m_GearName;
                        SlicedPacket.m_SendTo = GiveItemTo;
                        SlicedPacket.m_Hash = saveProxyData.GetHashCode();
                        SlicedPacket.m_Str = jsonStringSlice;

                        if (BytesBuffer.Count != 0)
                        {
                            SlicedPacket.m_Last = false;
                        }else{
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
                                SendTCPData(_packet);
                            }
                        }
                    }

                    if (BytesBuffer.Count < 500 && BytesBuffer.Count != 0)
                    {
                        byte[] LastSlice = BytesBuffer.GetRange(0, BytesBuffer.Count).ToArray();
                        BytesBuffer.RemoveRange(0, BytesBuffer.Count);

                        string jsonStringSlice = Encoding.UTF8.GetString(LastSlice);
                        SlicedJsonData SlicedPacket = new SlicedJsonData();
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
                                SendTCPData(_packet);
                            }
                        }
                    }
                }else{
                    SlicedJsonData DropPacket = new SlicedJsonData();
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
                            SendTCPData(_packet);
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
            }else{
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

        public static void ProcessGivingItem()
        {
            if(m_InterfaceManager != null && InterfaceManager.m_Panel_Inventory != null && InterfaceManager.m_Panel_Inventory.IsEnabled())
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
                        }else{
                            LastSelectedGearName = "";
                        }
                    }else{
                        LastSelectedGearName = "";
                    }
                }
            }else{
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

                        //UnityEngine.Object.DestroyImmediate(cloneObj);
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
            }else{
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
                        SendTCPData(_packet);
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
                        SendTCPData(_packet);
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
                    }else{
                        if (GameManager.GetPlayerManagerComponent().PlayerIsCrouched() == true)
                        {
                            MyAnimState = "Ctrl";
                        }else{
                            MyAnimState = "Walk";
                        }
                    }
                }else{
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
                    SendTCPData(_packet);
                }
            }

            if(iAmHost == true)
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
                    SendTCPData(_packet);
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
                }else{
                    MyAnimState = "Fight";
                }
            }else{
                
                if(MyEmote != null)
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
                                }
                                else if (GameManager.GetPlayerManagerComponent().m_HarvestableInProgress != null)
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
                                        }else{
                                            float CamY = GameManager.GetMainCamera().transform.position.y;
                                            if (bkObj.transform.position.y > CamY)
                                            {
                                                MyAnimState = "HarvestingStanding";
                                            }else{
                                                MyAnimState = "Harvesting";
                                            }
                                        }
                                    }
                                }else{
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
                                        }else{
                                            MyAnimState = "Ctrl";
                                        }
                                    }else{
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
                                        }else{
                                            MyAnimState = "Idle";
                                        }
                                    }
                                }
                            }else{
                                MyAnimState = "Map";
                            }
                        }
                    }
                }else{
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
                        SendTCPData(_packet);
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
                        SendTCPData(_packet);
                    }
                }
                if (iAmHost == true)
                {
                    ServerSend.CHANGEAIM(0, MyIsAiming, true);
                }
            }
        }

        public static void UpdateTicksOnScenes()
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (i == 0)
                {
                    MyTicksOnScene = MyTicksOnScene + 1;
                }else{
                    if (playersData[i] != null)
                    {
                        if (Server.clients[i].IsBusy() == true && playersData[i].m_AnimState != "Knock")
                        {
                            if (playersData[i].m_Levelid != playersData[i].m_PreviousLevelId)
                            {
                                playersData[i].m_PreviousLevelId = playersData[i].m_Levelid;
                                playersData[i].m_TicksOnScene = 0;
                            }else{
                                playersData[i].m_TicksOnScene = playersData[i].m_TicksOnScene + 1;
                            }
                        }else{
                            playersData[i].m_TicksOnScene = 0;
                        }
                    }
                }
            }
        }

        public static Quaternion GetBedRotation(Bed bed)
        {
            GameObject obj = bed.gameObject;
            Quaternion rot = new Quaternion(0,0,0,0);
            if (obj)
            {
                if(obj.GetComponent<Bed>().m_Bedroll == null)
                {
                    rot = Quaternion.Inverse(obj.GetComponent<Bed>().m_BodyPlacementTransform.rotation);
                }else{
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
            }else{
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
                }else{
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
                        SendTCPData(_packet);
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
                        SendTCPData(_packet);
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
            if(c != null && c.gameObject != null && c.gameObject.GetComponent<ObjectGuid>() != null)
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
            if (PBH !=  null && PBH.isActiveAndEnabled == true)
            {
                if(PBH.m_BodyHarvest != null && PBH.m_BodyHarvest.gameObject != null && PBH.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    harvestAnimalGUID = PBH.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>().Get();
                }
            }
            // If I am breakdown something
            Panel_BreakDown PBD = InterfaceManager.m_Panel_BreakDown;
            if (PBD !=  null && PBD.isActiveAndEnabled == true && PBD.m_BreakDown != null && PBD.m_BreakDown.gameObject != null)
            {
                if(PBD.m_BreakDown.gameObject.activeSelf == true)
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
                }else{
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
                if (playersData[i] != null && i != instance.myId)
                {
                    if(boxGUID != null || harvestGUID != null || harvestAnimalGUID != null || (breakGuid != null || breakParentGuid != null))
                    {
                        if (playersData[i].m_Container != null)
                        {
                            ContainerOpenSync otherBox = playersData[i].m_Container;
                            if (otherBox.m_Guid == boxGUID && otherBox.m_LevelID == levelid && otherBox.m_LevelGUID == level_guid)
                            {
                                boxGUID = null;
                                c.CancelSearch();
                            }
                        }
                        string otherHarvestPlant = playersData[i].m_Plant;

                        if(harvestGUID != null)
                        {
                            if (otherHarvestPlant == harvestGUID)
                            {
                                HUDMessage.AddMessage(playersData[i].m_Name + " IS ALREADY COLLECTING THIS");
                                harvestGUID = null;
                                h.CancelHarvest();
                            }
                        }

                        string otherAnimlGuid = playersData[i].m_HarvestingAnimal;

                        if(harvestAnimalGUID != null)
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
                            BrokenFurnitureSync otherFurn = playersData[i].m_BrakingObject;
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
                    }else{
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
                    SendTCPData(_packet);
                }
            }
            //PlayRadioOver();
        }

        public static string GetRadioFrequency(float ID)
        {
            float Correct = 325+ID;
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
            if(CurrentCustomChalleng.m_Started && CurrentChallengeRules.m_Name == "Lost in action" && CurrentCustomChalleng.m_CurrentTask == 0 && SearchModeActive)
            {
                return true;
            }else{
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
                RadioFrequency -=0.1f;
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
                SearchModeActive = !SearchModeActive;
            }

            if (LastSearchDistance != float.PositiveInfinity && CairnsSearchActive())
            {
                float Rate = GetRadioBeepRate(LastSearchDistance);
                if(nextLEDBlink == 0.0f)
                {
                    nextLEDBlink = Time.time + Rate;
                }

                if(Time.time > nextLEDBlink)
                {
                    nextLEDBlink = Time.time + Rate;

                    if(LEDState == false)
                    {
                        DoRadioBeep();
                        LEDState = true;
                    }else{
                        LEDState = false;
                    }
                }
            }else{
                LEDState = false;
            }

            ViewModelRadioLED.SetActive(LEDState);
        }

        public static bool IsCustomHandItem(string item)
        {
            if(item == "GEAR_Hatchet" 
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
        public class MeleeDescripter
        {
            public float m_PlayerDamage = 0;
            public float m_AnimalDamage = 0;
            public bool m_BloodLoss = false;
            public bool m_Pain = false;
            public bool m_ClothingTearing = false;
            public float m_AttackSpeed = 1;
            public float m_RetakeTime = 1;
        }

        public static MeleeDescripter GetMeelePlayerInfo(string weapon)
        {
            MeleeDescripter Info = new MeleeDescripter();

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
            if(weapon == "GEAR_Shovel")
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

        public static void PerformMeleeAttack(string Weapon)
        {
            if (GameManager.GetPlayerAnimationComponent().CanTransitionToState(PlayerAnimation.State.Throwing))
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
                        SendTCPData(_packet);
                    }
                }
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
            ShootSync HitSync = new MyMod.ShootSync();
            HitSync.m_position = GameManager.GetVpFPSCamera().transform.position;
            HitSync.m_rotation = GameManager.GetVpFPSCamera().transform.rotation;
            HitSync.m_projectilename = "Melee";
            HitSync.m_skill = 0;
            HitSync.m_camera_forward = GameManager.GetVpFPSCamera().transform.forward;
            HitSync.m_camera_right = GameManager.GetVpFPSCamera().transform.right;
            HitSync.m_camera_up = GameManager.GetVpFPSCamera().transform.up;
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.SHOOTSYNC))
                {
                    _packet.Write(HitSync);
                    SendTCPData(_packet);
                }
            }
            if (iAmHost == true)
            {
                ServerSend.SHOOTSYNC(0, HitSync, true);
            }

            int layerMask = Utils.m_WeaponProjectileCollisionLayerMask | 134217728;
            RaycastHit hit;

            string Weapon = "";

            if(GameManager.GetPlayerManagerComponent().m_ItemInHands != null)
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
                PlayerBulletDamage PlayerDamage = hit.collider.gameObject.GetComponent<MyMod.PlayerBulletDamage>();
                if(PlayerDamage != null)
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
                            SendTCPData(_packet);
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
                AnimalActor ActorFromObject;
                if (!hit.collider.gameObject)
                {
                    ActorFromObject = null;
                }
                else if (hit.collider.gameObject.layer == 16)
                {
                    ActorFromObject = hit.collider.gameObject.GetComponent<AnimalActor>();
                }else{
                    if (hit.collider.gameObject.layer == 27)
                    {
                        ActorFromObject = hit.collider.gameObject.transform.GetComponentInParent<AnimalActor>();
                    }else{
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
                if(ActorFromObject != null)
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
                            SendTCPData(_packet);
                        }
                    }
                    return;
                }
            }
        }
        public static bool IgnoreEmoteKeyDownUntilReleased = false;

        public class MultiplayerEmote
        {
            public string m_Name = "Anim";
            public string m_Animation = "Idle";
            public bool m_ForceCrouch = false;
            public bool m_ForceStandup = false;
            public bool m_FollowDollCamera = false;
            public bool m_NeedFreeHands = false;
        }

        public static MultiplayerEmote GetEmoteByID(int ID)
        {
            MultiplayerEmote Emote = new MultiplayerEmote();
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
            }else{
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
            MyEmote = GetEmoteByID(EmoteID);
        }

        public static void UpdateEmoteWheel()
        {
            if (EmoteWheel)
            {
                bool IsDown = KeyboardUtilities.InputManager.GetKey(KeyCode.T);

                if((chatInput != null && chatInput.gameObject.activeSelf == true) || (uConsole.m_Instance != null && uConsole.m_On == true))
                {
                    IsDown = false;
                }


                if(IsDown && IgnoreEmoteKeyDownUntilReleased == false)
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
                    if(Selected != -1)
                    {
                        EmoteWheel.transform.GetChild(8).gameObject.SetActive(true);
                        EmoteWheel.transform.GetChild(8).GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = GetEmoteByID(Selected).m_Name;
                    }else{
                        EmoteWheel.transform.GetChild(8).gameObject.SetActive(false);
                    }
                }
                else{
                    EmoteWheel.SetActive(false);
                }

                if(IgnoreEmoteKeyDownUntilReleased && !IsDown)
                {
                    IgnoreEmoteKeyDownUntilReleased = false;
                }
            }
        }

        public static void PushActionToMyDoll(string Act)
        {
            if (MyPlayerDoll)
            {
                MultiplayerPlayerAnimator Anim = MyPlayerDoll.GetComponent<MultiplayerPlayerAnimator>();
                if (Anim)
                {
                    if(Act == "Pickup")
                    {
                        Anim.Pickup();
                    }else if(Act == "Eat" || Act == "Drink")
                    {
                        Anim.m_IsDrink = Act == "Drink";
                        Anim.Consumption();
                    }else if(Act == "Shoot")
                    {
                        if(MyLightSourceName == "GEAR_Rifle")
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
                    }else if(Act == "Melee")
                    {
                        Anim.MeleeAttack();
                    }
                }
            }
        }

        public static void UpdateDoll()
        {
            if (MyPlayerDoll)
            {
                MyPlayerDoll.SetActive(FlyMode.m_Enabled);
                if (FlyMode.m_Enabled)
                {
                    MyPlayerDoll.transform.position = GameManager.GetPlayerTransform().position;
                    MyPlayerDoll.transform.rotation = GameManager.GetPlayerTransform().rotation;
                }
            }
        }

        public override void OnUpdate()
        {
            if(KillOnUpdate == true)
            {
                //DebugCrap(); // Debug for debuging and debuging debug.
                return;
            }

            if (LobbyUI != null)
            {
                LobbyUI.transform.GetChild(3).gameObject.SetActive(false);
            }

            if (Application.runInBackground == false) { Application.runInBackground = true; } // Always running in bg, to not lost any sync packets
            UpdateMain(); // Updating sync tread
            GameLogic.Update(); // Updating sync tread for server
            UpdateAPIStates(); // Updating ID and Client/Host state for API
            InitAudio(); // Adding audio listener if needed
            if (Application.isBatchMode) // If Dedicated mode, doing specific code for it
            {
                DedicatedServerUpdate(); // Updating dedicated settings
            }else{
                if (GameManager.m_Condition)
                {
                    IsDead = GameManager.m_Condition.m_CurrentHP <= 0;
                }
            }
            
            if (UiCanvas == null)
            {
                BuildCanvasUIs(); // Creating canvas based UI
            }else{
                UpdateCanvasUis(); // Upating text and other UI
            }

            if (Time.time > nextActionTimeSecond)
            {
                nextActionTimeSecond += periodSecond;

                if (iAmHost == true)
                {
                    ServerSend.KEEPITALIVE(0, true);
                }
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.KEEPITALIVE))
                    {
                        _packet.Write(true);
                        SendTCPData(_packet);
                    }
                }

                if (KillEverySecond == false)
                {
                    EverySecond(); // Triggering this code very second
                }
            }

            if (m_InterfaceManager != null)
            {
                if(NeedConnectAfterLoad != -1 && InterfaceManager.m_Panel_Confirmation != null)
                {
                    DoWaitForConnect(true); // Showing connection message after startup connection
                }
                if(InterfaceManager.m_Panel_SnowShelterInteract != null)
                {
                    CancleDismantling(); // Cancle Break-down of snowshelter where any other player is inside
                }
                if(InterfaceManager.m_Panel_Rest != null && InterfaceManager.m_Panel_Rest.isActiveAndEnabled == true)
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

                if (CurrentCustomChalleng.m_Started)
                {
                    if (InterfaceManager.m_Panel_Actions != null && InterfaceManager.m_Panel_Actions.isActiveAndEnabled)
                    {
                        InterfaceManager.m_Panel_Actions.m_MissionObjectWithTimer.SetActive(true);
                        InterfaceManager.m_Panel_Actions.m_MissionObjectiveWithTimerLabel.gameObject.SetActive(true);
                    }
                    PatchChallange();
                }
                UpdateEmoteWheel();
            }
            if (SteamServerWorks != "" || Server.UsingSteamWorks == true)
            {
                bool HoldRadio = false;
                if(GameManager.m_PlayerManager && GameManager.m_PlayerManager.m_ItemInHands && GameManager.m_PlayerManager.m_ItemInHands.GetFPSMeshID() == (int)FPSMeshID.HandledShortwave)
                {
                    HoldRadio = true;
                }

                TrackWhenRecordOver(HoldRadio); // Processing voice chat
            }

            if (FirstBoot == false && SteamConnect.CanUseSteam == true)
            {
                SteamConnect.DoUpdate(); // Start tracking of incomming data from other players
            }
            if (Time.time > nextActionTime)
            {
                nextActionTime += period;
                if (NeedSyncTime == true)
                {
                    EveryInGameMinute(); // Trigger actions that happends every in game minute (5 seconds) and recalculate realtime cycle and resend weather.
                }
                if (iAmHost == true)
                {
                    if (IsCycleSkiping == true && GameManager.m_PlayerManager != null)
                    {
                        if (GameManager.GetPlayerManagerComponent().PlayerIsSleeping() == false || IsDead == true)
                        {
                            IsCycleSkiping = false;
                        }
                    }
                    UpdateTicksOnScenes(); // Calculate how long player on this scene
                }
                if (TrackableDroppedGearsObjs.Count > 0)
                {
                    foreach (var item in TrackableDroppedGearsObjs)
                    {
                        if (item.Value && item.Value.GetComponent<DroppedGearDummy>())
                        {
                            DroppedGearDummy DGD = item.Value.GetComponent<DroppedGearDummy>();
                            int minutesLeft = DGD.m_Extra.m_GoalTime - MinutesFromStartServer;
                            if (minutesLeft <= 0)
                            {
                                item.Value.transform.GetChild(0).gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }

            if (GameManager.m_PlayerManager != null && GameManager.GetPlayerManagerComponent().m_ItemInHands)
            {
                string InHandName = GameManager.GetPlayerManagerComponent().m_ItemInHands.m_GearName;
                if (GameManager.GetPlayerManagerComponent().m_ItemInHands.GetFPSMeshID() == (int)FPSMeshID.HandledShortwave)
                {
                    ViewModelRadio.SetActive(true);
                    if (!OriginalRadioSeaker)
                    {
                        UpdateRadio();
                    }
                }else{
                    ViewModelRadio.SetActive(false);
                    SearchModeActive = false;
                }

                if (IsCustomHandItem(InHandName))
                {
                    ViewModelDummy.SetActive(false);

                    if (InputManager.GetFirePressed(InputManager.m_CurrentContext))
                    {
                        bool ShouldAttack = true;
                        if (ShouldReEquipFaster &&(InHandName == "GEAR_Knife" || InHandName == "GEAR_KnifeImprovised" || InHandName == "GEAR_KnifeScrapMetal" || InHandName == "GEAR_JeremiahKnife"))
                        {
                            ShouldAttack = false;
                        }
                        if (ShouldAttack)
                        {
                            PerformMeleeAttack(InHandName);
                        }
                    }
                }
                ViewModelHatchet.SetActive(InHandName == "GEAR_Hatchet");
                ViewModelHatchet2.SetActive(InHandName == "GEAR_HatchetImprovised");
                ViewModelHammer.SetActive(InHandName == "GEAR_Hammer");
                ViewModelKnife.SetActive(InHandName == "GEAR_Knife");
                ViewModelJeremiahKnife.SetActive(InHandName == "GEAR_JeremiahKnife");
                ViewModelKnife2.SetActive(InHandName == "GEAR_KnifeImprovised" || InHandName == "GEAR_KnifeScrapMetal");
                ViewModelPrybar.SetActive(InHandName == "GEAR_Prybar");
                ViewModelShovel.SetActive(InHandName == "GEAR_Shovel");
                ViewModelFireAxe.SetActive(InHandName == "GEAR_FireAxe");
                if (UseBoltInsteadOfStone)
                {
                    ViewModelStone.SetActive(false);
                    ViewModelBolt.SetActive(InHandName == "GEAR_Stone");
                }
            }

            if (InOnline() == true)
            {
                if (RealTimeCycleSpeed == true)
                {
                    string todString = OveridedTime;
                    float normalizedTOD = 0.0f;
                    if (Utils.TryParseTOD(todString, out normalizedTOD))
                    {
                        GameManager.GetTimeOfDayComponent().SetNormalizedTime(normalizedTOD);
                    }
                }
                if(GameManager.m_ExperienceModeManager != null)
                {
                    GameManager.m_ExperienceModeManager.GetCurrentExperienceMode().m_DecayScale = 0;
                }
            }

            if (level_name != "Empty" && level_name != "Boot" && level_name != "MainMenu" && GameManager.m_PlayerObject != null)
            {
                Transform transf = GameManager.GetPlayerTransform();
                bool InFight = GameManager.GetPlayerStruggleComponent().InStruggle();
                if (GameManager.GetPlayerManagerComponent().m_InteractiveObjectUnderCrosshair != null)
                {
                    LastObjectUnderCrosshair = GameManager.GetPlayerManagerComponent().m_InteractiveObjectUnderCrosshair;
                }
                UpdateDoll();

                if (GameManager.m_PlayerManager)
                {
                    if(m_InterfaceManager && InOnline() == true)
                    {
                        CancleIfSomeoneDoSame(); // Stop actions if another player doing same with that object.
                    }
                }

                if(DebugGUI == true)
                {
                    DebugCrap();
                }

                UpdateMyEquipment(); // Resend my current equipment if needed.
                UpdateVisualSyncs(); // Resend some effects if needed. (Bloodloss, heavy breath).

                if (transf)
                {
                    if(previoustickpos != transf.position) // If moving
                    {
                        MyEmote = null;
                        previoustickpos = GameManager.GetPlayerTransform().position;
                        SyncMovement(InFight, transf); // Send movement position and sets runing/walking animation.
                    }else{
                        SetCurrentAnimation(InFight); // Sets animations when not moving, actions and almost everything.
                    }
                    if(previoustickrot != transf.rotation) // If rotating
                    {
                        previoustickrot = GameManager.GetPlayerTransform().rotation;
                        SyncRotation(transf); // Send rotation of player model.
                    }
                }
                SendMyAnimation(); //Send current animation if changed.
                SyncSleeping(); //Send sleeping hours and sleep position, if changed.
                if(PlayerInteractionWith != null && InputManager.GetInteractReleased(InputManager.m_CurrentContext))
                {
                    LongActionCanceled(PlayerInteractionWith);
                }
                if(MyEmote != null)
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
                    else if(MyEmote.m_ForceCrouch)
                    {
                        if (!IsCrouched)
                        {
                            GameManager.GetVpFPSPlayer().InputCrouch();
                        }
                    }
                }
            }

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.G))
            {
                FindPlayerToTrade(); // Find closest player to give item to.
                ProcessGivingItem(); // Attempt to give item to nearest player.
            }
            if (InputManager.GetPutBackPressed(InputManager.m_CurrentContext))
            {
                if (GameManager.m_PlayerManager)
                {
                    PlayerManager __PM = GameManager.m_PlayerManager;
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
                            if (GI.gameObject.GetComponent<DropFakeOnLeave>() != null)
                            {
                                MelonLogger.Msg("Refuse from picking fake gear, make it fake again");

                                SendDropItem(GI, 0, 0, true, variant);
                            }
                        }
                    }
                }
            }
        }

        public static bool NoUI = false;
        public static bool ForcedUiOn = true;
        public static bool PreReleaseUI = false;
        public static string LastConnectedIp = "";
        public static string PendingConnectionIp = "";

        public static void DoConnectToIp(string _ip)
        {
            string newPort = "";
            string newIP = "";
            if (_ip.Contains(":"))
            {
                char seperator = Convert.ToChar(":");
                
                string[] sliced = _ip.Split(seperator);
                newIP = sliced[0];
                newPort = sliced[1];
            }

            if(newIP == "" && newPort == "")
            {
                PendingConnectionIp = _ip;
                instance.ip = _ip;
                MelonLogger.Msg("Going to connect to "+instance.ip+":"+instance.port);
            }else{
                PendingConnectionIp = newIP;
                instance.ip = newIP;
                instance.port = Convert.ToInt32(newPort);
                MelonLogger.Msg("Going to connect to "+instance.ip+":"+instance.port);
            }
            instance.ConnectToServer();
            DoWaitForConnect(false);
        }


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
            ModifyDynamicGears(Scene);
            Dictionary<int, DroppedGearItemDataPacket> Visuals = MPSaveManager.LoadDropVisual(Scene);
            Dictionary<int, SlicedJsonDroppedGear> Drops = MPSaveManager.LoadDropData(Scene);


            if (Drops != null && Visuals != null)
            {
                foreach (var item in Visuals)
                {
                    FakeDropItem(item.Value, true);
                }
            }
        }

        public static void SetFixedSpawn()
        {
            if(ServerConfig.m_PlayersSpawnType != 3) // Fixed spawn only
            {
                return;
            }
            
            MelonLogger.Msg("[FixedPlaceSpawns] SetFixedSpawn()");
            if(FixedPlaceLoaded == false)
            {
                SavedSceneForSpawn = level_name;
                SavedPositionForSpawn = GameManager.GetPlayerTransform().position;
                MelonLogger.Msg("[FixedPlaceSpawns] Isn't saved yet, set current position");
            }
            MelonLogger.Msg("[FixedPlaceSpawns] Scene: "+ SavedSceneForSpawn+" position X "+ SavedPositionForSpawn.x+" Y "+ SavedPositionForSpawn.y+" Z "+ SavedPositionForSpawn.z);
        }

        public static void HostAServer(int port = 26950)
        {
            if (iAmHost != true)
            {
                isRuning = true;

                Thread mainThread = new Thread(new ThreadStart(MainThread));
                Server.Start(MaxPlayers, port);
                nextActionTime = Time.time;
                nextActionTimeAniamls = Time.time;
                iAmHost = true;
                InitAllPlayers(); // Prepare players objects based on amount of max players
                MelonLogger.Msg("Server has been runned with InGame time: " + GameManager.GetTimeOfDayComponent().GetHour() + ":" + GameManager.GetTimeOfDayComponent().GetMinutes() + " seed "+ GameManager.m_SceneTransitionData.m_GameRandomSeed);
                OverridedHourse = GameManager.GetTimeOfDayComponent().GetHour();
                OverridedMinutes = GameManager.GetTimeOfDayComponent().GetMinutes();
                OveridedTime = OverridedHourse + ":" + OverridedMinutes;
                NeedSyncTime = true;
                RealTimeCycleSpeed = true;
                DisableOriginalAnimalSpawns(true);
                SetFixedSpawn();
                KillConsole(); // Unregistering cheats if server not allow cheating for you
            }else{
                HUDMessage.AddMessage("YOU ALREADY HOSING!!!!!!");
            }
        }

        public static void DoIPConnectWindow()
        {
            if (sendMyPosition != true)
            {
                if(SteamServerWorks == "")
                {
                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input server address", "127.0.0.1", Panel_Confirmation.ButtonLayout.Button_2, "Connect", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                }
                else{
                    if(SteamConnect.CanUseSteam == true)
                    {
                        SteamConnect.Main.JoinToLobby(SteamServerWorks);
                    }
                }
               
                RealTimeCycleSpeed = true;
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
            GameManager.GetHungerComponent().m_CurrentReserveCalories = GameManager.GetHungerComponent().m_MaxReserveCalories/4;
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

        public static void ResetDataForSlot(int _from)
        {
            if(_from != 0)
            {
                if (playersData[_from] != null)
                {
                    playersData[_from] = new MultiPlayerClientData();
                }
                ServerSend.EQUIPMENT(_from, new PlayerEquipmentData(), false);
                ServerSend.LIGHTSOURCE(_from, false, false);
                ServerSend.LIGHTSOURCENAME(_from, "", false);
                ServerSend.XYZ(_from, new Vector3(0, 0, 0), false);
                ServerSend.XYZW(_from, new Quaternion(0, 0, 0, 0), false);
                ServerSend.ANIMSTATE(_from, "Idle", false);
                ServerSend.LEVELID(_from, 0, false);
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
            if(sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.XYZ))
                {
                    _packet.Write(v3);
                    SendTCPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.XYZW))
                {
                    _packet.Write(rot);
                    SendTCPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.LEVELID))
                {
                    _packet.Write(levelid);
                    SendTCPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.LEVELGUID))
                {
                    _packet.Write(level_guid);
                    SendTCPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.ANIMSTATE))
                {
                    _packet.Write(MyAnimState);
                    SendTCPData(_packet);
                }
            }
        }

        public static void SendSpawnData(bool AskResponce = false)
        {
            if (Application.isBatchMode)
            {
                return;
            }
            if(LoadingScreenIsOn == false)
            {
                return;
            }
            if(AskResponce == false)
            {
                MelonLogger.Msg("Sending my spawn data to all players");
            }else{
                MelonLogger.Msg("Sending spawn data as responce");
            }
            PlayerClothingData Cdata = new PlayerClothingData();
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
                    SendTCPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.LEVELID))
                {
                    _packet.Write(levelid);
                    SendTCPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.LEVELGUID))
                {
                    _packet.Write(level_guid);
                    SendTCPData(_packet);
                }
                SendMyEQ();
                using (Packet _packet = new Packet((int)ClientPackets.LIGHTSOURCE))
                {
                    _packet.Write(MyLightSource);
                    SendTCPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.LIGHTSOURCENAME))
                {
                    _packet.Write(MyLightSourceName);
                    SendTCPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.XYZ))
                {
                    _packet.Write(GameManager.GetPlayerTransform().position);
                    SendTCPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.XYZW))
                {
                    _packet.Write(GameManager.GetPlayerTransform().rotation);
                    SendTCPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.ANIMSTATE))
                {
                    _packet.Write(MyAnimState);
                    SendTCPData(_packet);
                }
                using (Packet _packet = new Packet((int)ClientPackets.CLOTH))
                {
                    _packet.Write(Cdata);
                    SendTCPData(_packet);
                }
                if(AskResponce == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.ASKSPAWNDATA))
                    {
                        _packet.Write(levelid);
                        SendTCPData(_packet);
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

        public static bool FirstBoot = true;

        public static void ExecuteOnMainThread(Action _action)
        {
            if (_action == null)
            {
                MelonLogger.Msg("No action to execute on main thread!");
                return;
            }

            lock (executeOnMainThread)
            {
                executeOnMainThread.Add(_action);
                actionToExecuteOnMainThread = true;
            }
        }
        /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
        public static void UpdateMain()
        {
            if (actionToExecuteOnMainThread)
            {
                executeCopiedOnMainThread.Clear();
                lock (executeOnMainThread)
                {
                    executeCopiedOnMainThread.AddRange(executeOnMainThread);
                    executeOnMainThread.Clear();
                    actionToExecuteOnMainThread = false;
                }

                for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
                {
                    executeCopiedOnMainThread[i]();
                }
            }
        }

        public static void DoSteamWorksConnect(string sid)
        {
            MyMod.PendingConnectionIp = SteamServerWorks;
            using (Packet _packet = new Packet((int)ClientPackets.CONNECTSTEAM))
            {
                _packet.Write(sid);
                SendTCPData(_packet);
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
                if(domessage == true)
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
            if(GameManager.m_PlayerManager == null || GameManager.m_PlayerManager.m_Clothing == null)
            {
                return "";
            }

            GearItem gi = GameManager.GetPlayerManagerComponent().GetClothingInSlot(s, l);
            if(gi != null)
            {
                string dummyN = gi.m_GearName;
                string finalN = "";
                if (dummyN.Contains("(Clone)")) //If it has ugly (Clone), cutting it.
                {
                    int L = dummyN.Length - 7;
                    finalN = dummyN.Remove(L, 7);
                }else{
                    finalN = dummyN;
                }
                return finalN;
            }else{
                return "";
            }
        }

        public static string GetClothToDisplay(string bottom, string mid, string top = "", string top2 = "")
        {
            //2 layers select
            if(top == "" && top2 == "")
            {
                if(mid == "")
                {
                    return bottom;
                }else{
                    return mid;
                }
            }else{
                if(top2 != "")
                {
                    return top2;
                }else if( top != "")
                {
                    return top;
                }else if (mid != "")
                {
                    return mid;
                }else if (bottom != "")
                {
                    return bottom;
                }else{
                    return ""; 
                }
            }
        }

        public static bool IsScarf(string name)
        {
            if(name.Contains("Scarf"))
            {
                return true;
            }else{
                return false;
            }
        }
        public static bool IsBalaclava(string name)
        {
            if (name.Contains("Balaclava"))
            {
                return true;
            }else{
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
            if(m_Panel_Sandbox && m_Panel_Sandbox.isActiveAndEnabled)
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
                MaxPlayers = slotsMax;
                ApplyOtherCampfires = true;

                InitAllPlayers();
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
                }else{
                    SteamConnect.Main.MakeLobby(LobbyType, MaxPlayers);               
                    MenuChange.ChangeMenuItems("Lobby");
                }

                UnityEngine.Object.Destroy(UIHostMenu);
                HostMenuHints = new List<GameObject>();
                GameManager.m_IsPaused = false;


                ServerSettingsData SaveData = new ServerSettingsData();
                SaveData.m_CFG = ServerConfig;
                SaveData.m_MaxPlayers = MaxPlayers;
                SaveData.m_P2P = ShouldUseSteam;
                SaveData.m_Accessibility = LobbyType;
                SaveData.m_Port = PortToHost;

                MPSaveManager.SaveServerCFG(SaveData);
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
            }else{
                obj.SetActive(false);
            }
        }

        public static void AddHintScript(GameObject obj)
        {
            //MelonLogger.Msg("[AddHintScript] Started...");
            if(obj == null)
            {
                MelonLogger.Msg("[AddHintScript] Got null object. Why?");
                return;
            }
            GameObject hintButn = obj.transform.GetChild(0).gameObject;
            if(hintButn == null)
            {
                MelonLogger.Msg("[AddHintScript] "+ obj.name + " have not hint button!");
                return;
            }
            GameObject hintObj = hintButn.transform.GetChild(0).gameObject;
            if(hintObj == null)
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
                MelonLogger.Msg("CantBeUsedForMP "+ CantBeUsedForMP+ " LastLoadedGenVersion "+ LastLoadedGenVersion+ " RandomGenVersion "+ BuildInfo.RandomGenVersion);
                return;
            }

            if (UiCanvas != null && UIHostMenu == null)
            {
                UIHostMenu = MakeModObject("MP_HostSettings", UiCanvas.transform);

                ServerSettingsData SavedSettings = MPSaveManager.RequestServerCFG();
                if (SavedSettings != null)
                {
                    ServerConfig = SavedSettings.m_CFG;
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
                }else{

                    IsSteamHost.GetComponent<UnityEngine.UI.Toggle>().Set(SavedSettings.m_P2P);

                    if (SavedSettings.m_P2P)
                    {
                        SteamLobbyType.SetActive(true);
                        SteamLobbyType.GetComponent<UnityEngine.UI.Dropdown>().Set(SavedSettings.m_Accessibility);
                        PortsObject.SetActive(false);
                    }else{
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

            if(GameManager.m_PlayerManager == null || GameManager.m_Inventory == null)
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
            }else{
                hatCandidat1 = Hat1;
            }

            if (IsScarf(Hat2))
            {
                m_Scarf = Hat2;
            }else{
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

            if(NeedUpdate == true)
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

                PlayerClothingData Cdata = new PlayerClothingData();
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
                        SendTCPData(_packet);
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
                }else{
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
                }else{
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
                if(m_Players.Count-1 < m_CurrentPlayer)
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
                    if(BoardGamesSessions[i].m_GUID == GUID)
                    {
                        return BoardGamesSessions[i]; 
                    }
                }
            }

            return null;
        }

        public static void StartBoardGameSessions(string GameType, string GUID)
        {
            if(GetBoardGameSession(GUID) == null)
            {
                BoardGamesSessions.Add(new BoardGameSession(GUID, GameType));
            }else{
                MelonLogger.Msg("Boardgame session with GUID "+ GUID+" already exist");
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
                if(Dist < MaxRadioSearchDistance)
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
            if(BeepRate < MinimalBlink)
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

        public class CustomChallengeTaskData
        {
            public string m_Task = "";
            public int m_Timer = 0;
            public int m_GoalVal = 1;

            public CustomChallengeTaskData(string Task, int Timer, int GoalVal)
            {
                m_Task = Task;
                m_Timer = Timer;
                m_GoalVal = GoalVal;
            }
        }

        public class CustomChallengeRules
        {
            public string m_Name = "";
            public string m_Description = "";
            public List<CustomChallengeTaskData> m_Tasks = new List<CustomChallengeTaskData>();
            public bool m_Lineal = true;
            public bool m_CompetitiveMode = false;
            public int m_ID = 0;
        }

        public class CustomChallengeData
        {
            public int m_CurrentTask = 0;
            public int m_Time = 0;
            public bool m_Started = false;
            public List<int> m_Done = new List<int>();
        }

        public static CustomChallengeRules GetCustomChallengeByID(int ID)
        {
            CustomChallengeRules c = new CustomChallengeRules();
            if(ID == 1)
            {
                c.m_Name = "Cold Allergic";
                c.m_Description = "This island is worst nightmare for you. Getting crash on frozen hell with having Cold urticaria, how ironic.";
                c.m_Tasks.Add(new CustomChallengeTaskData("Get to the Desolation Point without getting risk of Hypothermia", 5400, 1));
                c.m_Tasks.Add(new CustomChallengeTaskData("Enter any building", 600, 1));
                c.m_Lineal = true;
                c.m_CompetitiveMode = false;
                c.m_ID = 1;
            }
            else if (ID == 2)
            {
                c.m_Name = "Lost in action";
                c.m_Description = "Find cairn of poor souls who had bad luck to end up here for forever.";
                c.m_Tasks.Add(new CustomChallengeTaskData("Find cairns", 5400, 30));
                c.m_Tasks.Add(new CustomChallengeTaskData("Meet with other survivors on Camp Office", 2200, 1));
                c.m_Lineal = true;
                c.m_CompetitiveMode = false;
                c.m_ID = 2;
            }
            return c;
        }

        public static void LoadCustomChallenge(int ID, int CurrentTask)
        {
            CustomChallengeRules rules = GetCustomChallengeByID(ID);
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
            if(CurrentChallengeRules.m_Name == "Cold Allergic")
            {
                if(CurrentCustomChalleng.m_CurrentTask == 0)
                {
                    if(RegionManager.GetCurrentRegion() == GameRegion.WhalingStationRegion)
                    {
                        NextCustomTask();
                    }
                }
                else if(CurrentCustomChalleng.m_CurrentTask == 1)
                {
                    if (!GameManager.IsOutDoorsScene(level_name) && !level_name.Contains("Cave") && !level_name.Contains("Mine"))
                    {
                        CurrentCustomChalleng.m_Started = false;
                        ProcessCustomChallengeTrigger("Win");
                        SendCustomChallengeTrigger("Win");
                    }
                }
                if(level_name != "Boot" && level_name != "Empty" && level_name != "MainMenu")
                {
                    if(GameManager.GetFreezingComponent().m_CurrentFreezing >= 100)
                    {
                        CurrentCustomChalleng.m_Started = false;
                        ProcessCustomChallengeTrigger("Lose");
                        SendCustomChallengeTrigger("Lose");
                    }
                }
            }else if (CurrentChallengeRules.m_Name == "Lost in action")
            {
                if(CurrentCustomChalleng.m_CurrentTask == 1)
                {
                    if(iAmHost == true)
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
                                if (ClientIsLoading(i) == false && playersData[i].m_Levelid == 10)
                                {
                                    HowManyOnCamp++;
                                }
                            }
                        }

                        if(HowManyOnCamp == HowManyOnServer)
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
            MelonLogger.Msg("[ProcessCustomChallengeTrigger] "+ TRIGGER);
            
            if(TRIGGER == "Lose")
            {
                if(m_Panel_ChallengeComplete == null)
                {
                    m_Panel_ChallengeComplete = InterfaceManager.LoadPanel<Panel_ChallengeComplete>();
                }
                m_Panel_ChallengeComplete.ShowPanel(Panel_ChallengeComplete.Options.ShowFailStatInfo);
                m_Panel_ChallengeComplete.SetStatInfoText("Ехать ты лох конечно", "Оставшиеся время:", CurrentCustomChalleng.m_Time,"Обкакался раз:", 99);
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
            else if (TRIGGER.StartsWith("CairnID"))
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
                    SendTCPData(_packet);
                }
            }
        }

        public static CustomChallengeData CurrentCustomChalleng = new CustomChallengeData();
        public static CustomChallengeRules CurrentChallengeRules = new CustomChallengeRules();

        public static string GetFormatedTextForChallange()
        {
            TimeSpan span = TimeSpan.FromSeconds(CurrentCustomChalleng.m_Time);
            return span.ToString(@"hh\:mm\:ss");
        }
        public static int CalculateHowManyTaskDone()
        {
            int done = 0;
            for (int i = 0; i < CurrentCustomChalleng.m_Done.Count; i++)
            {
                if(CurrentCustomChalleng.m_Done[i] >= CurrentChallengeRules.m_Tasks[i].m_GoalVal)
                {
                    done++;
                }
            }
            return done;
        }

        public static void PatchChallange()
        {
            if(CurrentChallengeRules.m_ID == 0)
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
            }else{
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
                    element = element+ "[";
                    if (CurrentResult >= NeedToGoal)
                    {
                        element = element + "X] ";
                    }else{
                        element = element + " ] ";
                    }
                    element = element + TaskText;
                    Cdesc = Cdesc+element;
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

                if(CurrentChallengeRules.m_Tasks.Count > 1)
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
            if(CurrentSearchIndex < sceneCount)
            {
                string Load = scenes[CurrentSearchIndex];
                GameManager.LoadSceneWithLoadingScreen(Load);
                MelonLogger.Msg(ConsoleColor.Green, "[PoiskMujikov] Loading scene " + Load +" Left scenes "+ CurrentSearchIndex+"/"+ sceneCount);
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
    }
}
