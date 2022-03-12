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
            public const string Version = "0.8.4 pre";
            public const string DownloadLink = null;
            public const int RandomGenVersion = 2;
        }
        public static int LastLoadedGenVersion = 0;
        public static bool CantBeUsedForMP = false;

        //VARS
        #region VARS
        public static bool isRuning = false;
        public static bool sendMyPosition = false;
        public static bool iAmHost = false;
        public static bool IamShatalker = false;
        public static GameObject playerbody = null;
        public static List<GameObject> players = new List<GameObject>();
        public static List<MultiPlayerClientData> playersData = new List<MultiPlayerClientData>();
        public static Campfire FlexFire = null;
        public static Animator MenuErjan1 = null;
        public static Animator MenuErjan2 = null;
        public static bool CarryingPlayer = false;
        public static bool IsCarringMe = false;
        public static bool IsDead = false;
        public static bool KillAfterLoad = false;
        public static bool DoFakeGetup = false;
        public static string PlayerBodyGUI = "";
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
        public static string OtherHarvetingAnimal = "";
        public static List<WalkTracker> SurvivorWalks = new List<WalkTracker>();
        public static WalkTracker LastLure = new WalkTracker();
        public static bool ALWAYS_FUCKING_CURSOR_ON = false;
        public static bool NoRabbits = true;
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
        public static int RegularUpdateSeconds = 7;
        public static int MinutesFromStartServer = 0;
        public static bool SkipEverythingForConnect = false;
        public static GameObject UISteamFreindsMenuObj = null;
        public static int PlayersOnServer = 0;
        public static GameObject UIHostMenu = null;
        public static GameObject MicrophoneIdicator = null;
        public static bool IsPublicServer = false;
        public static string CustomServerName = "";
        public static bool ApplyOtherCampfires = false;
        public static bool HadEverPingedMaster = false;
        public static Dictionary<int, string> SlicedJsonDataBuffer = new Dictionary<int, string>();
        public static Dictionary<int, List<byte>> SlicedBytesDataBuffer = new Dictionary<int, List<byte>>();
        public static bool DebugTrafficCheck = false;
        public static Dictionary<string, Dictionary<int, SlicedJsonDroppedGear>> DroppedGears = new Dictionary<string, Dictionary<int, SlicedJsonDroppedGear>>();
        public static Dictionary<string, Dictionary<string, bool>> OpenableThings = new Dictionary<string, Dictionary<string, bool>>();
        public static Dictionary<int, GameObject> DroppedGearsObjs = new Dictionary<int, GameObject>();
        public static Dictionary<string, Dictionary<string, AnimalCompactData>> AnimalStorage = new Dictionary<string, Dictionary<string, AnimalCompactData>>();
        //Voice chat
        public static int VoiceChatFrequencyHz = 19000;
        public static int MyMicrophoneID = 0;
        public static bool DoingRecord = false;
        public static GameObject VoiceTestDummy = null;
        //Other mods support
        public static GameObject ModdedHandsBook = null;
        #endregion

        public static Vector3 LastSleepV3 = new Vector3(0, 0, 0);
        public static Quaternion LastSleepQuat = new Quaternion(0, 0, 0, 0);
        public static bool AtBed = false;
        public static Vector3 OutOfBedPosition = new Vector3(0, 0, 0);

        public static Dictionary<string, int> GearIDList = new Dictionary<string, int>();
        //public static bool AntiCheat = false;
        public static bool InterloperHook = false;
        public static string OverridedSceneForSpawn = "";
        public static Vector3 OverridedPositionForSpawn = Vector3.zero;
        public static string SavedSceneForSpawn = "";
        public static Vector3 SavedPositionForSpawn = new Vector3(0, 0, 0);
        public static bool FixedPlaceLoaded = false;

        public static string AutoStartSlot = "";
        public static List<string> AutoCMDs = new List<string>();
        public static bool CrazyPatchesLogger = false;
        public static bool KillOnUpdate = false;

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

                    if(jData.m_SendTo != -1)
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
                    newGear.GetComponent<GearItem>().Deserialize(finalJsonData);
                    newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;

                    DropFakeOnLeave DFL = newGear.AddComponent<DropFakeOnLeave>();
                    DFL.m_OldPossition = newGear.gameObject.transform.position;
                    DFL.m_OldRotation = newGear.gameObject.transform.rotation;

                    DroppedGearDummy DGD = newGear.gameObject.GetComponent<DroppedGearDummy>();
                    if (DGD != null)
                    {
                        if (DGD.m_Extra.m_GoalTime != 0 && DGD.m_Extra.m_GoalTime != -1)
                        {
                            GearItem gear = newGear.GetComponent<GearItem>();
                            if (gear.m_EvolveItem != null)
                            {
                                int days = Convert.ToInt32(gear.m_EvolveItem.m_TimeToEvolveGameDays);
                                int hours = days * 24;
                                int minutes = hours * 60;

                                int minutesOnDry = MyMod.MinutesFromStartServer - DGD.m_Extra.m_DroppedTime;

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
                    string LevelKey = OriginalData.Split(Convert.ToChar("|"))[0];
                    string GUID = OriginalData.Split(Convert.ToChar("|"))[1];

                    MelonLogger.Msg(ConsoleColor.Green, "Finished loading container data for " + jData.m_Hash);

                    if(iAmHost == true)
                    {
                        SaveFakeContainer(GUID, LevelKey, finalJsonData, false);
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
                    GotLargeDataArray(FinalBuffer.ToArray(), bytesData.m_Action, from, bytesData.m_ExtraInt);
                }
            }
        }

        public static void GotLargeDataArray(byte[] array, string action, int from, int ExtraInt)
        {
            if(action == "VOICE")
            {
                ProcessVoiceChatData(from, array, ExtraInt);
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
            public Vector3 m_SleepV3 = new Vector3(0, 0, 0);
            public Quaternion m_SleepQuat = new Quaternion(0, 0, 0, 0);
            public bool m_Aiming = false;
        }
        public class MultiPlayerClientStatus //: MelonMod
        {
            public int m_ID = 0;
            public string m_Name = "";
            public bool m_Sleep = false;
            public bool m_Dead = false;
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

            public float m_Meat = 0;
            public int m_Guts = 0;
            public int m_Hide = 0;
            public float m_Frozen = 0;
            public int m_Controller = 0;
            public string m_ProxySave = "";
            public int m_LevelD = 0;
            public string m_SpawnRegionGUID = "";
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
            else if (ActName == "Bandage")
            {
                act.m_Action = "Bandage";
                act.m_DisplayText = "Bandage wound";
                act.m_ProcessText = "Bandaging wound...";
                act.m_CancleOnMove = true;
                act.m_ActionDuration = 3;
            }
            else if (ActName == "Sterilize")
            {
                act.m_Action = "Sterilize";
                act.m_DisplayText = "Sterilize wound";
                act.m_ProcessText = "Sterilizing wound...";
                act.m_CancleOnMove = true;
                act.m_ActionDuration = 3;
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
            }

            return act;
        }
        public class VoiceChatQueueElement
        {
            public byte[] m_VoiceData;
            public int m_Samples = 0;
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
            }
            else
            {
                MelonLogger.Msg(ConsoleColor.Magenta, "[-slot] Save found! Loading...");
                SaveGameSlots.SetBaseNameForSave(SaveToLoad.m_SaveSlotName, SaveToLoad.m_SaveSlotName);
                SaveGameSystem.SetCurrentSaveInfo(SaveToLoad.m_Episode, SaveToLoad.m_GameMode, SaveToLoad.m_GameId, SaveToLoad.m_SaveSlotName);
                GameManager.LoadSaveGameSlot(SaveToLoad.m_SaveSlotName, SaveToLoad.m_SaveChangelistVersion);
            }
        }

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
                MelonLogger.Msg(ConsoleColor.Blue, ServStr + " P2P: " + ServerData.UsingSteam);
                MelonLogger.Msg(ConsoleColor.Blue, ServStr + " Ports: " + ServerData.Ports);
                MelonLogger.Msg(ConsoleColor.Blue, ServStr + " Cheats: " + ServerData.Cheats);
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
                    IsPublicServer = false;
                    CustomServerName = ServerData.ServerName;
                    if(ds == true)
                    {
                        Server.StartSteam(MaxPlayers, ServerData.WhiteList);
                    }else{
                        Server.StartSteam(MaxPlayers);
                    }
                }

                if(ds == true)
                {
                    ForceLoadSlotForDs(ServerData.SaveSlot);
                }
                //SteamConnect.Main.InviteFriendBySid(new Steamworks.CSteamID(76561198867520214));
            }else{
                MelonLogger.Msg(ConsoleColor.Red, ServStr + " Can't find server.json!");
            }
        }

        public override void OnApplicationStart()
        {
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
            ClassInjector.RegisterTypeInIl2Cpp<CarTest>();
            ClassInjector.RegisterTypeInIl2Cpp<FakeFire>();
            ClassInjector.RegisterTypeInIl2Cpp<FakeFireLight>();
            ClassInjector.RegisterTypeInIl2Cpp<DoNotSerializeThis>();
            ClassInjector.RegisterTypeInIl2Cpp<MultiplayerPlayerVoiceChatPlayer>();
            ClassInjector.RegisterTypeInIl2Cpp<DroppedGearDummy>();
            ClassInjector.RegisterTypeInIl2Cpp<IgnoreDropOverride>();
            ClassInjector.RegisterTypeInIl2Cpp<DropFakeOnLeave>();
            ClassInjector.RegisterTypeInIl2Cpp<FakeBed>();
            ClassInjector.RegisterTypeInIl2Cpp<FakeBedDummy>();
            ClassInjector.RegisterTypeInIl2Cpp<AnimalBornMarker>();
            ClassInjector.RegisterTypeInIl2Cpp<SpawnRegionAnimalsList>();

            if (instance == null)
            {
                instance = this;
                tcp = new TCP();
                udp = new UDP();
            }
            else if (instance != this)
            {

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

        public override void OnLevelWasInitialized(int level)
        {
            OpenablesObjs.Clear();
            MelonLogger.Msg("Level initialized: " + level);
            levelid = level;
            MelonLogger.Msg("Level name: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            level_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            MyTicksOnScene = 0;
            if (ShatalkerModeClient == true || ServerHandle.DarkShatalkerMode == true || IamShatalker == true)
            {
                ShatalkerObject = Resources.FindObjectsOfTypeAll<InvisibleEntityManager>().First();
                MelonLogger.Msg("Shatalker was null");
            }
            if (IamShatalker == true)
            {
                uConsole.RunCommandSilent("Ghost");
                uConsole.RunCommandSilent("God");
            }
            CheckHaveBookMod();
            DisableOriginalAnimalSpawns(false);
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
            public List<VoiceChatQueueElement> VoiceQueue = new List<VoiceChatQueueElement>();

            void Update()
            {
                if (aSource != null)
                {
                    //VolumeSetup();
                    if(aSource.isPlaying == false)
                    {
                        if (VoiceQueue.Count > 0)
                        {
                            VoiceChatQueueElement Voice = VoiceQueue[0];
                            if (Voice != null)
                            {
                                PlayVoiceFromPlayerObject(aSource.gameObject, Voice.m_VoiceData, Voice.m_Samples);
                            }
                            VoiceQueue.RemoveAt(0);
                        }
                    }
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

            void Update()
            {
                if (m_Animer != null)
                {
                    GameObject m_Player = m_Animer.gameObject;
                    int m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                    string HoldingItem = m_Player.GetComponent<MultiplayerPlayer>().m_HoldingItem;

                    if(playersData[m_ID] != null && playersData[m_ID].m_Levelid != levelid)
                    {
                        return;
                    }

                    string m_AnimState = playersData[m_ID].m_AnimState;

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
                            if(playersData[m_ID].m_SleepV3 != new Vector3(0, 0, 0))
                            {
                                m_Player.transform.GetChild(4).gameObject.SetActive(false);
                            }else{
                                m_Player.transform.GetChild(4).gameObject.SetActive(true);
                            }
                            m_Animer.Play("Sleep", 0);
                        }
                        if (m_AnimState != "Sleep")
                        {
                            m_Player.transform.GetChild(4).gameObject.SetActive(false);
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
                                if(playersData[m_ID].m_Aiming == true)
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
                                if (playersData[m_ID].m_Aiming == true)
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

                    if (m_AnimState == "Walk" || m_AnimState == "Run")
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
                    }
                    else
                    {
                        StepState = 0;
                    }
                }
            }
            public void Pickup()
            {
                GameObject m_Player = m_Animer.gameObject;
                int m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                //MelonLogger.Msg("Player "+m_ID +" Picked up something");

                if (playersData[m_ID] == null || playersData[m_ID].m_Levelid != levelid || playersData[m_ID].m_LevelGuid != level_guid)
                {
                    return;
                }
                //MelonLogger.Msg("You on same scene with  " + m_ID + " to play pickup");

                string m_AnimState = playersData[m_ID].m_AnimState;
                int armTagHash = m_Animer.GetCurrentAnimatorStateInfo(3).tagHash;
                int armNeededTagHash = Animator.StringToHash("Pickup");
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
            public void Consumption()
            {
                GameObject m_Player = m_Animer.gameObject;
                int m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;

                if (playersData[m_ID] == null || playersData[m_ID].m_Levelid != levelid || playersData[m_ID].m_LevelGuid != level_guid)
                {
                    return;
                }

                if(m_IsDrink == true)
                {
                    playersData[m_ID].m_AnimState = "Drinking";
                    m_Player.GetComponent<MultiplayerPlayer>().m_HoldingFood = "Water";
                }else{
                    playersData[m_ID].m_AnimState = "Eating";
                    m_Player.GetComponent<MultiplayerPlayer>().m_HoldingFood = "Food";
                }
                m_Player.GetComponent<MultiplayerPlayer>().UpdateHeldItem();
            }
            public void StopConsumption()
            {
                GameObject m_Player = m_Animer.gameObject;
                int m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;

                if (playersData[m_ID] == null || playersData[m_ID].m_Levelid != levelid || playersData[m_ID].m_LevelGuid != level_guid)
                {
                    return;
                }

                if(m_Player.GetComponent<MultiplayerPlayer>().m_HoldingFood != "")
                {
                    m_Player.GetComponent<MultiplayerPlayer>().m_HoldingFood = "";
                    m_Player.GetComponent<MultiplayerPlayer>().UpdateHeldItem();
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
            if(plObj != null)
            {
                plObj.SetActive(enable);
                //if(plObj.GetComponent<MultiplayerPlayer>() != null)
                //{
                //    plObj.GetComponent<MultiplayerPlayer>().enabled = enable;
                //}
                //if (plObj.GetComponent<MultiplayerPlayerAnimator>() != null)
                //{
                //    plObj.GetComponent<MultiplayerPlayerAnimator>().enabled = enable;
                //}
                //if (plObj.GetComponent<MultiplayerPlayerClothingManager>() != null)
                //{
                //    plObj.GetComponent<MultiplayerPlayerClothingManager>().enabled = enable;
                //}
                //if (plObj.GetComponent<MultiplayerPlayerVoiceChatPlayer>() != null)
                //{
                //    plObj.GetComponent<MultiplayerPlayerVoiceChatPlayer>().enabled = enable;
                //}
                //if (plObj.GetComponent<AudioSource>() != null)
                //{
                //    plObj.GetComponent<AudioSource>().enabled = enable;
                //}
                //if (plObj.GetComponent<Animator>() != null)
                //{
                //    plObj.GetComponent<Animator>().enabled = enable;
                //}
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
                        bookOpen = hand_l.transform.GetChild(6).gameObject;
                        bookClosed = hand_l.transform.GetChild(7).gameObject;

                        MakenzyHead = body.transform.GetChild(1).GetChild(0).gameObject;
                        AstridHead = body.transform.GetChild(1).GetChild(1).gameObject;
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

                    if(pD.m_AnimState == "Sleep")
                    {
                        if(pD.m_SleepV3 != new Vector3(0, 0, 0))
                        {
                            m_XYZ = pD.m_SleepV3;
                            m_XYZW = pD.m_SleepQuat;
                        }
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


            void Update()
            {

            }
        }
        public class UiButtonPressHook : MonoBehaviour
        {
            public UiButtonPressHook(IntPtr ptr) : base(ptr) { }
            public int m_CustomId = 0;

            void Update()
            {

            }
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
                GameObject leg_r = root.GetChild(2).gameObject;
                GameObject leg_l = root.GetChild(1).gameObject;

                chest.AddComponent<PlayerBulletDamage>();
                arm_r1.AddComponent<PlayerBulletDamage>();
                arm_r2.AddComponent<PlayerBulletDamage>();
                arm_l1.AddComponent<PlayerBulletDamage>();
                arm_l2.AddComponent<PlayerBulletDamage>();
                head.AddComponent<PlayerBulletDamage>();
                leg_r.AddComponent<PlayerBulletDamage>();
                leg_l.AddComponent<PlayerBulletDamage>();

                chest.GetComponent<PlayerBulletDamage>().SetLocaZone(chest, mP);
                arm_r1.GetComponent<PlayerBulletDamage>().SetLocaZone(arm_r1, mP);
                arm_r2.GetComponent<PlayerBulletDamage>().SetLocaZone(arm_r2, mP);
                arm_l1.GetComponent<PlayerBulletDamage>().SetLocaZone(arm_l1, mP);
                arm_l2.GetComponent<PlayerBulletDamage>().SetLocaZone(arm_l2, mP);
                head.GetComponent<PlayerBulletDamage>().SetLocaZone(head, mP);
                leg_r.GetComponent<PlayerBulletDamage>().SetLocaZone(leg_r, mP);
                leg_l.GetComponent<PlayerBulletDamage>().SetLocaZone(leg_l, mP);
            }
        }

        public static void DamageByBullet(float damage, int from)
        {
            if(damage <= 0)
            {
                return;
            }
            
            string DamageCase = "Other player";
            if(playersData[from] != null)
            {
                DamageCase = playersData[from].m_Name + " shoot you";
            }

            bool HasArmor = false;

            if(damage == 50) // Chest damage locaZone
            {
                HasArmor = GameManager.GetDamageProtection().HasBallisticVest();
                if (HasArmor)
                {
                    damage = damage / 10;
                }
            }

            GameManager.GetConditionComponent().AddHealth(-damage, DamageSource.BulletWound);

            if (!HasArmor)
            {
                GameManager.GetBloodLossComponent().BloodLossStart(DamageCase, true, AfflictionOptions.PlayFX);
                var RNG = new System.Random(); int clothingRNG = RNG.Next(20, 40);
                GameManager.GetPlayerManagerComponent().ApplyDamageToWornClothing(clothingRNG);
            }else{
                var RNG = new System.Random(); int ribBroke = RNG.Next(0, 100);
                if(ribBroke <= 5)
                {
                    GameManager.GetBrokenRibComponent().BrokenRibStart(DamageCase, true, false, true, false);
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
                                _packet.Write(m_ClientId);
                                SendTCPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.BULLETDAMAGE))
                            {
                                ServerSend.BULLETDAMAGE(m_ClientId, (float)m_Damage, 0);
                            }
                        }
                    }
                }
            }

            public void SetLocaZone(GameObject t, MultiplayerPlayer pl)
            {
                m_Obj = t;
                m_Obj.tag = "Flesh";
                m_Obj.layer = vp_Layer.Container;

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

        public static void DisableOriginalAnimalSpawns(bool OnHost)
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
                    region.gameObject.AddComponent<SpawnRegionAnimalsList>();
                }
            }
            MelonLogger.Msg("[SpawnAnimals] " + Regions.Count + " Regions has been deactivated");
            if (OnHost == true)
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

        public static void TestSaveAnimals(string LevelKey)
        {
            for (int index = 0; index < BaseAiManager.m_BaseAis.Count; ++index)
            {
                if (BaseAiManager.m_BaseAis[index] != null)
                {
                    AnimalCompactData Dat = GetCompactDataForAnimal(BaseAiManager.m_BaseAis[index]);
                    if(Dat != null)
                    {
                        RegisterAnimal(LevelKey, Dat);
                    }
                }
            }
        }

        public static void TestLoadAnimals(string LevelKey)
        {
            MelonLogger.Msg(ConsoleColor.Green, "[AnimalStorage] Trying to load data for " + LevelKey);
            Dictionary<string, AnimalCompactData> SceneStorage;
            if (AnimalStorage.TryGetValue(LevelKey, out SceneStorage) == true)
            {
                MelonLogger.Msg(ConsoleColor.Green, "[AnimalStorage] Save found, loading each animal...");
                int index = 0;
                foreach (var cur in SceneStorage)
                {
                    index++;
                    SpawnFromCompactData(cur.Value);
                }
            }else{
                MelonLogger.Msg(ConsoleColor.Red, "[AnimalStorage] Save Not found, so refuse to load!");
            }
        }

        public static void SpawnFromCompactData(AnimalCompactData dat)
        {
            MelonLogger.Msg(ConsoleColor.Green, "[AnimalStorage] Trying to load " + dat.m_GUID + " from save!");
            GameObject animal = UnityEngine.Object.Instantiate<GameObject>(Resources.Load(dat.m_PrefabName).Cast<GameObject>());
            animal.name = dat.m_PrefabName;
            animal.transform.position = dat.m_Position;
            animal.transform.rotation = dat.m_Rotation;
            BaseAi AI = animal.GetComponent<BaseAi>();
            AI.m_CurrentHP = dat.m_Health;
            if(AI.CreateMoveAgent(null) == true && !AI.GetMoveAgent().Warp(dat.m_Position, 1f, true, -1))
            {
                MelonLogger.Msg(ConsoleColor.Red, "[AnimalStorage] Can't load animal " + dat.m_GUID + " because it has invalid MoveAgent!");
            }
            if (animal.GetComponent<ObjectGuid>() == null)
            {
                animal.AddComponent<ObjectGuid>();
            }
            animal.GetComponent<ObjectGuid>().Set(dat.m_GUID);
            ObjectGuidManager.RegisterGuid(dat.m_GUID, animal);
            AnimalBornMarker ABM = animal.AddComponent<AnimalBornMarker>();
            ABM.m_Animal = animal;
            ABM.m_RegionGUID = dat.m_RegionGUID;
            GameObject spRobj = ObjectGuidManager.Lookup(dat.m_RegionGUID);
            if(spRobj != null)
            {
                SpawnRegion spR = spRobj.GetComponent<SpawnRegion>();
                if(spRobj.GetComponent<SpawnRegionAnimalsList>() != null)
                {
                    spRobj.GetComponent<SpawnRegionAnimalsList>().m_Animals.Add(dat.m_GUID);
                }
            }
        }
        public static void SimulateSpawnFromRegionSpawn(string GUID)
        {
            GameObject spRobj = ObjectGuidManager.Lookup(GUID);
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
                    AnimalBornMarker ABM = animal.AddComponent<AnimalBornMarker>();
                    ABM.m_Animal = animal;
                    ABM.m_RegionGUID = spRobj.GetComponent<ObjectGuid>().Get();
                    if(spRobj.GetComponent<SpawnRegionAnimalsList>() != null)
                    {
                        spRobj.GetComponent<SpawnRegionAnimalsList>().m_Animals.Add(animalGUID);
                    }
                    ObjectGuidManager.RegisterGuid(animalGUID, spRobj);
                }
            }
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
                return Dat;
            }
            return null;
        }

        public static void RegisterAnimal(string LevelKey, AnimalCompactData Dat)
        {
            Dictionary<string, AnimalCompactData> SceneStorage;
            if(AnimalStorage.TryGetValue(LevelKey, out SceneStorage) == false)
            {
                AnimalStorage.Add(LevelKey, new Dictionary<string, AnimalCompactData>());
                if (AnimalStorage.TryGetValue(LevelKey, out SceneStorage) == false)
                {
                    MelonLogger.Msg(ConsoleColor.Red, "Idk why, but can't create dictionary for " + LevelKey);
                }
            }
            if(SceneStorage != null)
            {
                AnimalCompactData AnimalDat;
                if (SceneStorage.TryGetValue(Dat.m_GUID, out AnimalDat) == true)
                {
                    SceneStorage.Remove(Dat.m_GUID);
                }
                SceneStorage.Add(Dat.m_GUID, Dat);
                MelonLogger.Msg(ConsoleColor.Green, "[AnimalStorage] Animal "+ Dat.m_GUID+" has been saved for "+ LevelKey);
            }
        }

        public static void UnRegisterAnimal(string LevelKey, string GUID)
        {
            Dictionary<string, AnimalCompactData> SceneStorage;
            if (AnimalStorage.TryGetValue(LevelKey, out SceneStorage) == false)
            {
                return;
            }
            if (SceneStorage != null)
            {
                AnimalCompactData AnimalDat;
                if (SceneStorage.TryGetValue(GUID, out AnimalDat) == true)
                {
                    SceneStorage.Remove(GUID);
                }
            }
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
                                    MelonLogger.Msg("Pick nearest player " + LastFoundID + " " + otherPlayerV3.x + " " + otherPlayerV3.y + " " + otherPlayerV3.z);
                                }
                            }
                        }
                    }
                }
            }

            return LastFoundID;
        }

        public static void SendAnimalForValidPlayers(AnimalSync data, Vector3 v3)
        {
            for (int i = 0; i < playersData.Count; i++)
            {
                if(playersData[i] != null)
                {
                    if(i != instance.myId)
                    {
                        float dis = Vector3.Distance(v3, playersData[i].m_Position);
                        if (playersData[i].m_Levelid == levelid && playersData[i].m_LevelGuid == level_guid && dis < GameManager.GetSpawnRegionManager().m_DisallowDespawnBelowDistance)
                        {
                            if (iAmHost == true)
                            {
                                ServerSend.ANIMALSYNC(0, data, i);
                            }
                            if (sendMyPosition == true)
                            {
                                using (Packet _packet = new Packet((int)ClientPackets.ANIMALSYNC))
                                {
                                    _packet.Write(data);
                                    _packet.Write(i);
                                    SendTCPData(_packet);
                                }
                            }
                        }
                    }
                }
            }
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
            public int m_Anim = 0;
            public int m_AnimNext = 0;
            public GameObject m_TestModel = null;
            public bool m_RemoteSpawned = false;
            public string m_RightName = "";
            public bool m_DebugAnimal = false;
            public bool m_CanSync = false;
            public bool m_InActive = false;
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
            public bool m_WritenMeshes = false;
            public SkinnedMeshRenderer m_Mesh1 = null;
            public SkinnedMeshRenderer m_Mesh2 = null;
            public SkinnedMeshRenderer m_Mesh3 = null;
            public float m_Hp = 100;
            public bool m_Bleeding = false;
            public bool m_ClientControlled = false;
            public int m_ClientController = 0;
            public bool m_Banned = false;
            public bool m_DampingIgnore = true;
            public string m_PendingProxy = "";
            public bool m_WaitForAligment = false;
            public int LastFoundPlayer = -1;

            void Start()
            {
                nextActionTimeNR = Time.time;
                nextActionSync = Time.time;
                nextActionBloodDrop = Time.time;
                nextActionDampingOn = Time.time+dampingOn_perioud;
            }
            public void CallSync()
            {
                if (m_Animal != null && m_Banned == false)
                {
                    if ((AnimalsController == true || (m_ClientController == instance.myId)) && m_WaitForAligment == false)
                    {
                        if(m_Animal.GetComponent<WildlifeItem>() != null && m_Animal.GetComponent<WildlifeItem>().m_PickedUp == true)
                        {
                            return;
                        }

                        AnimalSync sync = new AnimalSync();
                        sync.m_position = m_Animal.transform.position;
                        sync.m_rotation = m_Animal.transform.rotation;
                        sync.m_guid = m_Animal.GetComponent<ObjectGuid>().Get();
                        sync.m_LevelD = levelid;

                        if (m_Animal.GetComponent<BaseAi>() != null && m_Animal.GetComponent<BaseAi>().m_Animator != null)
                        {

                            BaseAi _AI = m_Animal.GetComponent<BaseAi>();

                            Animator AN = _AI.m_Animator;

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
                            

                            if(_AI.m_SpawnRegionParent != null && _AI.m_SpawnRegionParent.gameObject.GetComponent<ObjectGuid>() != null)
                            {
                                sync.m_SpawnRegionGUID = _AI.m_SpawnRegionParent.gameObject.GetComponent<ObjectGuid>().Get();
                            }else{
                                sync.m_SpawnRegionGUID = "";
                            }

                            bool ItWas = m_ClientControlled;
                            int NewController = instance.myId;

                            if(AnimalsController == true)
                            {
                                if(m_ClientControlled == false)
                                {
                                    NewController = GetClosestPlayerToAnimal(m_Animal, m_Animal.GetComponent<AnimalUpdates>().ReTakeCoolDown, instance.myId, GameManager.GetPlayerTransform().position, levelid);
                                    LastFoundPlayer = NewController;
                                }else{
                                    NewController = GetClosestPlayerToAnimal(m_Animal, m_Animal.GetComponent<AnimalUpdates>().ReTakeCoolDown, m_ClientController, playersData[m_ClientController].m_Position, playersData[m_ClientController].m_Levelid);
                                    LastFoundPlayer = NewController;
                                }
                                if (NewController != instance.myId)
                                {
                                    m_ClientController = NewController;
                                    m_ClientControlled = true;
                                    sync.m_Controller = NewController;
                                }
                                else
                                {
                                    m_ClientController = instance.myId;
                                    m_ClientControlled = false;
                                    sync.m_Controller = instance.myId;
                                }
                            }else{
                                m_ClientController = instance.myId;
                                m_ClientControlled = true;
                                sync.m_Controller = instance.myId;
                            }

                            if(m_ClientControlled != ItWas)
                            {
                                if(m_ClientControlled == true)
                                {
                                    sync.m_ProxySave = _AI.Serialize();
                                }else{
                                    //if(m_WaitForAligment != true)
                                    //{
                                    //    if (iAmHost == true)
                                    //    {
                                    //        using (Packet _packet = new Packet((int)ServerPackets.ASKFORANIMALPROXY))
                                    //        {
                                    //            ServerSend.ASKFORANIMALPROXY(1, sync.m_guid);
                                    //        }
                                    //    }

                                    //    if (sendMyPosition == true)
                                    //    {
                                    //        using (Packet _packet = new Packet((int)ClientPackets.ASKFORANIMALPROXY))
                                    //        {
                                    //            _packet.Write(sync.m_guid);
                                    //            SendTCPData(_packet);
                                    //        }
                                    //    }
                                    //    m_WaitForAligment = true;
                                    //    return;
                                    //}
                                }
                            }

                            if (_AI.GetAiMode() == AiMode.Dead)
                            {
                                sync.m_Hp = 0;
                                sync.m_Bleeding = false;
                            } else {
                                sync.m_Hp = _AI.m_CurrentHP;
                                sync.m_Bleeding = _AI.m_BleedingOut;
                            }

                            if(m_Animal.GetComponent<BodyHarvest>() != null)
                            {
                                BodyHarvest BH = m_Animal.GetComponent<BodyHarvest>();
                                sync.m_Meat = BH.m_MeatAvailableKG;
                                sync.m_Guts = BH.m_GutAvailableUnits;
                                sync.m_Hide = BH.m_HideAvailableUnits;
                                sync.m_Frozen = BH.m_PercentFrozen;
                            }
                        }
                        sync.m_name = GetAnimalPrefabName(m_Animal.name);
                        SendAnimalForValidPlayers(sync, m_Animal.transform.position);
                    }
                }
            }

            void Update()
            {                
                if (m_Animal != null)
                {
                    if (AnimalsController == false)
                    {
                        if(m_ClientController != instance.myId)
                        {
                            if (m_InActive == false)
                            {
                                m_InActive = true;
                                string regionGUID = "";
                                if (m_Animal.GetComponent<AnimalBornMarker>() != null)
                                {
                                    regionGUID = m_Animal.GetComponent<AnimalBornMarker>().m_RegionGUID;
                                }
                                
                                MakeAnimalActive(m_Animal, false, regionGUID);
                            }
                        }else{
                            if (m_InActive == true)
                            {
                                //MelonLogger.Msg("Re-creating animal under my control.");
                                m_InActive = false;
                                string regionGUID = "";
                                if (m_Animal.GetComponent<AnimalBornMarker>() != null)
                                {
                                    regionGUID = m_Animal.GetComponent<AnimalBornMarker>().m_RegionGUID;
                                }
                                MakeAnimalActive(m_Animal, true, regionGUID);
                                //ReCreateAnimal(m_Animal, m_PendingProxy);
                                m_PendingProxy = "";
                            }
                        }
                    }else{
                        if(m_ClientControlled == true)
                        {
                            if (m_InActive == false)
                            {

                                //MelonLogger.Msg("Making controlled by client animal inactive");
                                m_InActive = true;
                                string regionGUID = "";
                                if (m_Animal.GetComponent<AnimalBornMarker>() != null)
                                {
                                    regionGUID = m_Animal.GetComponent<AnimalBornMarker>().m_RegionGUID;
                                }
                                MakeAnimalActive(m_Animal, false, regionGUID);
                            }
                        }else{
                            if (m_InActive == true)
                            {
                                m_InActive = false;
                                string regionGUID = "";
                                if (m_Animal.GetComponent<AnimalBornMarker>() != null)
                                {
                                    regionGUID = m_Animal.GetComponent<AnimalBornMarker>().m_RegionGUID;
                                }
                                MakeAnimalActive(m_Animal, true, regionGUID);
                                //ReCreateAnimal(m_Animal, m_PendingProxy);
                                m_PendingProxy = "";
                            }
                        }
                    }

                    if ((AnimalsController == false || m_ClientControlled == true))
                    {
                        if (m_WritenMeshes == false)
                        {
                            m_WritenMeshes = true;
                            WriteDownMesh(m_Animal);
                        }

                        if (m_Mesh1 != null) { if (m_Mesh1.isVisible == false) { m_Mesh1.enabled = true; m_Mesh1.forceRenderingOff = false; } }
                        if (m_Mesh2 != null) { if (m_Mesh2.isVisible == false) { m_Mesh2.enabled = true; m_Mesh2.forceRenderingOff = false; } }
                        if (m_Mesh3 != null) { if (m_Mesh3.isVisible == false) { m_Mesh3.enabled = true; m_Mesh3.forceRenderingOff = false; } }

                        BaseAi _AI = m_Animal.GetComponent<BaseAi>();

                        if (m_ClientController != instance.myId)
                        {
                            if(m_DampingIgnore == true)
                            {
                                m_Animal.transform.position = m_ToGo;
                                m_Animal.transform.rotation = m_ToRotate;
                            }else{
                                m_Animal.transform.position = Vector3.Lerp(m_Animal.transform.position, m_ToGo, Time.deltaTime * DeltaAnimalsMultiplayer);
                                m_Animal.transform.rotation = Quaternion.Lerp(m_Animal.transform.rotation, m_ToRotate, Time.deltaTime * DeltaAnimalsMultiplayer);
                            }

                            if (_AI != null)
                            {
                                Animator AN = _AI.m_Animator;

                                AN.SetFloat(_AI.m_AnimParameter_TurnAngle, AP_TurnAngle);
                                AN.SetFloat(_AI.m_AnimParameter_TurnSpeed, AP_TurnSpeed);
                                AN.SetFloat(_AI.m_AnimParameter_Speed, AP_Speed);
                                AN.SetFloat(_AI.m_AnimParameter_Wounded, AP_Wounded);
                                AN.SetFloat(_AI.m_AnimParameter_Roll, AP_Roll);
                                AN.SetFloat(_AI.m_AnimParameter_Pitch, AP_Pitch);
                                AN.SetFloat(_AI.m_AnimParameter_TargetHeading, AP_TargetHeading);
                                AN.SetFloat(_AI.m_AnimParameter_TargetHeadingSmooth, AP_TargetHeadingSmooth);
                                AN.SetFloat(_AI.m_AnimParameter_TapMeter, AP_TapMeter);
                                AN.SetInteger(_AI.m_AnimParameter_AiState, AP_AiState);
                                AN.SetBool(_AI.m_AnimParameter_Corpse, AP_Corpse);
                                AN.SetBool(_AI.m_AnimParameter_Dead, AP_Dead);
                                AN.SetInteger(_AI.m_AnimParameter_DamageSide, AP_DeadSide);
                                AN.SetInteger(_AI.m_AnimParameter_DamageBodyPart, AP_DamageBodyPart);
                                AN.SetInteger(_AI.m_AnimParameter_AttackId, AP_AttackId);
                            }
                        }

                        _AI.m_CurrentHP = m_Hp;

                        if (m_Hp <= 0)
                        {
                            _AI.SetAiMode(AiMode.Dead);
                        }
                    }

                    if (m_Animal.GetComponent<BodyHarvest>() != null)
                    {
                        if (m_Animal.GetComponent<BaseAi>().GetAiMode() == AiMode.Dead || m_Animal.GetComponent<BaseAi>().m_CurrentHP == 0)
                        {
                            BodyHarvest BH = m_Animal.GetComponent<BodyHarvest>();

                            if (BH.enabled == true)
                            {
                                if (m_Animal.GetComponent<ObjectGuid>().Get() == OtherHarvetingAnimal)
                                {
                                    BH.enabled = false;
                                }
                            }
                            else
                            {
                                if (m_Animal.GetComponent<ObjectGuid>().Get() != OtherHarvetingAnimal)
                                {
                                    BH.enabled = true;
                                }
                            }
                        }
                    }

                    if (AnimalsController == true || m_ClientController == instance.myId)
                    {
                        m_CanSync = true;
                    }else{
                        m_CanSync = false;
                    }
                    if (Time.time > nextActionTimeNR)
                    {
                        nextActionTimeNR += noresponce_perioud;

                        if(AnimalsController == true || m_ClientController == instance.myId)
                        {
                            if(ReTakeCoolDown > 0)
                            {
                                ReTakeCoolDown = ReTakeCoolDown-1;
                            }
                        }
                        if (AnimalsController == false && m_ClientController != instance.myId)
                        {
                            NoResponce = NoResponce - 1;
                            if (NoResponce <= 0)
                            {
                                MelonLogger.Msg("Found animal that we not need anymore " + m_Animal.GetComponent<ObjectGuid>().Get());

                                if (m_Animal.GetComponent<ObjectGuid>() != null && m_Animal.GetComponent<ObjectGuid>().Get() == HarvestingAnimal)
                                {
                                    ExitHarvesting();
                                    if(m_Animal.GetComponent<BodyHarvest>() != null)
                                    {
                                        m_Animal.GetComponent<BodyHarvest>().enabled = false;
                                    }
                                }
                                ObjectGuidManager.UnRegisterGuid(m_Animal.GetComponent<ObjectGuid>().Get());
                                UnityEngine.Object.Destroy(m_Animal);
                            }
                        }
                    }
                    if(Time.time > nextActionDampingOn)
                    {
                        if(m_DampingIgnore == true)
                        {
                            m_DampingIgnore = false;
                        }
                    }
                    if (Time.time > nextActionSync)
                    {
                        nextActionSync += actionSync_perioud;
                        if (levelid > 3 && (AnimalsController == true || m_ClientController == instance.myId) && IsShatalkerMode() == false && m_CanSync == true)
                        {
                            CallSync();
                        }
                    }
                    if (Time.time > nextActionBloodDrop)
                    {
                        nextActionBloodDrop += blooddrop_period;

                        if (levelid > 3 && AnimalsController == false && m_Bleeding == true)
                        {
                            BaseAi _AI = m_Animal.GetComponent<BaseAi>();
                            if (_AI.m_BloodTrail != null)
                            {
                                BloodTrail _Blood = _AI.m_BloodTrail;

                                Vector3 pos = m_Animal.transform.position;
                                ++pos.y;
                                Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                                insideUnitCircle.Normalize();
                                Vector2 vector2 = insideUnitCircle * UnityEngine.Random.Range(0.0f, 0.75f);
                                pos.x += vector2.x;
                                pos.z += vector2.y;
                                pos -= m_Animal.transform.forward * 0.5f;
                                RaycastHit hitInfo;
                                if (!Physics.Raycast(pos, Vector3.down, out hitInfo, float.PositiveInfinity, Utils.m_PhysicalCollisionLayerMask) || (UnityEngine.Object)hitInfo.collider == (UnityEngine.Object)null)
                                    return;
                                Vector3 scale = _Blood.m_DecalProjectorScale * UnityEngine.Random.Range(0.5f, 2f);
                                int uvRectangleIndex = _Blood.m_UvRectangleIndexBloodSmall;
                                if (Utils.RollChance(50f))
                                    uvRectangleIndex = _Blood.m_UvRectangleIndexBloodLarge;
                                GameManager.GetDynamicDecalsManager().CreateDecal(hitInfo.point, _Blood.transform.rotation.eulerAngles.y, hitInfo.normal, uvRectangleIndex, scale, _Blood.GetDecalProjectorType(), GameManager.GetWeatherComponent().IsIndoorEnvironment());
                            }
                        }
                    }
                }
            }
        }

        public class AnimalBornMarker : MonoBehaviour
        {
            public AnimalBornMarker(IntPtr ptr) : base(ptr) { }
            public GameObject m_Animal = null;
            public string m_RegionGUID = "";
        }
        public class SpawnRegionAnimalsList : MonoBehaviour
        {
            public SpawnRegionAnimalsList(IntPtr ptr) : base(ptr) { }
            public List<string> m_Animals = new List<string>();
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
            { (int)ServerPackets.ANIMALSYNC, ClientHandle.ANIMALSYNC},
            { (int)ServerPackets.DARKWALKERREADY, ClientHandle.DARKWALKERREADY},
            { (int)ServerPackets.HOSTISDARKWALKER, ClientHandle.HOSTISDARKWALKER},
            { (int)ServerPackets.WARDISACTIVE, ClientHandle.WARDISACTIVE},
            { (int)ServerPackets.DWCOUNTDOWN, ClientHandle.DWCOUNTDOWN},
            { (int)ServerPackets.ANIMALSYNCTRIGG, ClientHandle.ANIMALSYNCTRIGG},
            { (int)ServerPackets.SHOOTSYNC, ClientHandle.SHOOTSYNC},
            { (int)ServerPackets.PIMPSKILL, ClientHandle.PIMPSKILL},
            { (int)ServerPackets.HARVESTINGANIMAL, ClientHandle.HARVESTINGANIMAL},
            { (int)ServerPackets.DONEHARVASTING, ClientHandle.DONEHARVASTING},
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
            { (int)ServerPackets.SLEEPPOSE, ClientHandle.SLEEPPOSE},
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

        public static void AlignAnimalWithProxy(string Proxy, string _guid)
        {
            GameObject animal = ObjectGuidManager.Lookup(_guid);

            if(animal != null)
            {
                animal.GetComponent<BaseAi>().Deserialize(Proxy);

                AnimalUpdates au = animal.GetComponent<AnimalUpdates>();

                if (au != null)
                {
                    au.m_WaitForAligment = false;
                }
            }
        }

        public static void DeleteAnimal(string _guid)
        {
            //MelonLogger.Msg("Got signal to delete " + _guid + " animal");

            GameObject animal = ObjectGuidManager.Lookup(_guid);

            if(animal != null)
            {
                UnityEngine.Object.Destroy(animal);
                ObjectGuidManager.UnRegisterGuid(_guid);
            }
        }

        public static void DoAnimalDamage(string guid, float damage)
        {
            GameObject animal = ObjectGuidManager.Lookup(guid);
            if (animal)
            {
                MelonLogger.Msg("Other player damage animal "+ guid+" on "+ damage);
                AnimalUpdates au = animal.GetComponent<AnimalUpdates>();

                if (animal.GetComponent<AnimalUpdates>() != null)
                {
                    if(AnimalsController == true || au.m_ClientController == instance.myId)
                    {
                        animal.GetComponent<BaseAi>().ApplyDamage(damage, DamageSource.Player, "");
                        MelonLogger.Msg("After apply damage current HP Is " + animal.GetComponent<BaseAi>().m_CurrentHP);
                    }
                }
            }
        }


        public static void DoAnimalSync(AnimalSync obj)
        {
            if(obj.m_LevelD != levelid)
            {
                return;
            }
            string _guid = obj.m_guid;
            string prefabName = obj.m_name;
            bool AnimalExists = false;
            bool ShouldRecreate = false;

            if(obj.m_Controller == instance.myId)
            {
                ShouldRecreate = true;
            }else{
                ShouldRecreate = false;
            }

            if(NoRabbits == true && obj.m_name.Contains("Rabbit"))
            {
               // MelonLogger.Msg("Got sync of rabbit, refuse to use this.");
                return;
            }

            //MelonLogger.Msg(_guid + " Got animal " + obj.m_name);

            GameObject animal = ObjectGuidManager.Lookup(_guid);

            if (animal != null)
            {
                AnimalExists = true;
                BaseAi _AI = animal.GetComponent<BaseAi>();
                AnimalUpdates au = animal.GetComponent<AnimalUpdates>();

                if (animal.GetComponent<AnimalUpdates>() != null)
                {
                    if (au.m_Banned == true)
                    {
                        return;
                    }
                    if (obj.m_ProxySave != "")
                    {
                        au.m_PendingProxy = obj.m_ProxySave;
                    }
                    au.m_ClientController = obj.m_Controller;

                    if (obj.m_Controller != instance.myId)
                    {
                        au.m_ToGo = obj.m_position;
                        au.m_ToRotate = obj.m_rotation;

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

                        _AI.m_CurrentHP = obj.m_Hp;
                        au.m_Hp = obj.m_Hp;
                        au.m_Bleeding = obj.m_Bleeding;
                    }

                    au.NoResponce = 5;
                    BodyHarvest BH = animal.GetComponent<BodyHarvest>();
                    BH.m_MeatAvailableKG = obj.m_Meat;
                    BH.m_GutAvailableUnits = obj.m_Guts;
                    BH.m_HideAvailableUnits = obj.m_Hide;
                    BH.m_PercentFrozen = obj.m_Frozen;
                }
            }else{
                AnimalExists = false;
            }

            if (AnimalExists == false)
            {
                SpawnAnimal(prefabName, obj.m_position, _guid, obj.m_SpawnRegionGUID, ShouldRecreate, obj.m_ProxySave, obj.m_Hp);
            }
        }

        public static void SetAnimalTriggers(AnimalTrigger obj)
        {
            string _guid = obj.m_Guid;
            int trigg = obj.m_Trigger;

            GameObject animal = ObjectGuidManager.Lookup(_guid);

            if(animal != null)
            {
                if (animal.GetComponent<BaseAi>() != null)
                {
                    BaseAi _AI = animal.GetComponent<BaseAi>();
                    Animator AN = _AI.m_Animator;

                    if (trigg == _AI.m_AnimParameter_StruggleEnd)
                    {
                        MelonLogger.Msg("Going to end struggle, so reseting StruggleStart");
                        AN.ResetTrigger(_AI.m_AnimParameter_StruggleStart);
                    }
                    else if (trigg == _AI.m_AnimParameter_StruggleStart)
                    {
                        MelonLogger.Msg("Going to start struggle, so reseting StruggleEnd");
                        AN.ResetTrigger(_AI.m_AnimParameter_StruggleEnd);
                    }
                    _AI.AnimSetTrigger(trigg);
                    MelonLogger.Msg("Set trigger for animal " + _guid + " trigger hash " + trigg);
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
            MelonLogger.Msg("Shooting client "+ from + " on level  "+ playersData[from].m_Levelid);
            if (playersData.Count > 0 && playersData[from].m_Levelid != levelid)
            {
                return;
            }
            MelonLogger.Msg("Shoot: ");
            MelonLogger.Msg("X: " + shoot.m_position.x);
            MelonLogger.Msg("Y: " + shoot.m_position.y);
            MelonLogger.Msg("Z: " + shoot.m_position.z);
            MelonLogger.Msg("Rotation X: " + shoot.m_rotation.x);
            MelonLogger.Msg("Rotation Y: " + shoot.m_rotation.y);
            MelonLogger.Msg("Rotation Z: " + shoot.m_rotation.z);
            MelonLogger.Msg("Rotation W: " + shoot.m_rotation.w);

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
                component.m_NoiseMakerItem.m_CanThrow = false;
                NoiseMakerItem component1 = component.m_NoiseMakerItem;
                component1.Ignite();

                if(shoot.m_skill == 0)
                {
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
                }else{
                    component1.PerformDetonation((ContactPoint[])null);
                }
            }
            else
            {
                MelonLogger.Msg("Got remote shoot event " + shoot.m_projectilename);

                GameObject gameObject = null;
                GearItem itemInHands = null;

                if (shoot.m_projectilename == "PistolBullet")
                {
                    itemInHands = GetGearItemPrefab("GEAR_Rifle");
                    gameObject = UnityEngine.Object.Instantiate<GameObject>(PistolBulletPrefab, shoot.m_position, shoot.m_rotation);
                    gameObject.AddComponent<ClientProjectile>();
                    ClientProjectile ClientP = gameObject.GetComponent<ClientProjectile>();
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

        public static void DoForcedHarvestAnimal(string _guid, HarvestStats Harvey)
        {
            GameObject animal = ObjectGuidManager.Lookup(_guid);

            if(animal != null)
            {
                if (animal.GetComponent<ObjectGuid>() != null)
                {
                    if (_guid == animal.GetComponent<ObjectGuid>().Get())
                    {
                        //MelonLogger.Msg("Remote harvesting of " + animal.GetComponent<ObjectGuid>().Get() + " animal");
                        if (animal.GetComponent<BodyHarvest>() != null)
                        {
                            BodyHarvest BH = animal.GetComponent<BodyHarvest>();
                            BH.m_MeatAvailableKG = BH.m_MeatAvailableKG - Harvey.m_Meat;
                            BH.m_GutAvailableUnits = BH.m_GutAvailableUnits - Harvey.m_Guts;
                            BH.m_HideAvailableUnits = BH.m_HideAvailableUnits - Harvey.m_Hide;
                        }
                        if (animal.GetComponent<AnimalUpdates>() != null)
                        {
                            AnimalUpdates AU = animal.GetComponent<AnimalUpdates>();
                            AU.CallSync();
                        }
                    }
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

        Vector3 previoustickpos;
        Quaternion previoustickrot;

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

        public void DoWeatherOverride()
        {
            Weather weatherComponent = GameManager.GetWeatherComponent();
            if (!InteriorTemperatureTrigger.m_PlayerInside)
                weatherComponent.m_IndoorTemperatureCelsius = 0; //this.m_IndoorTemperatureCelsius;
            weatherComponent.m_HighTempMinCelsius = 0; //this.m_HighTempMinCelsius;
            weatherComponent.m_HighTempMaxCelsius = 0; //this.m_HighTempMaxCelsius;
            weatherComponent.m_LowTempMinCelsius = 0; //this.m_LowTempMinCelsius;
            weatherComponent.m_LowTempMaxCelsius = 0; //this.m_LowTempMaxCelsius;
            weatherComponent.m_BlizzardDegreesDrop = 0; //this.m_BlizzardDegreesDrop;
            weatherComponent.m_BlizzardDegreesChangePerSecond = 0; //this.m_BlizzardDegreesChangePerSecond;
            weatherComponent.m_HourWarmingBegins = 0; //this.m_HourWarmingBegins;
            weatherComponent.m_HourCoolingBegins = 0; //this.m_HourCoolingBegins;
                                                      //if (this.m_AuroraEarlyWindowProbability != 0)
            weatherComponent.m_AuroraEarlyWindowProbability = 0; //this.m_AuroraEarlyWindowProbability;
            //if (this.m_AuroraLateWindowProbability != 0)
            weatherComponent.m_AuroraLateWindowProbability = 0; //this.m_AuroraLateWindowProbability;
            weatherComponent.m_DegreesPerSecondChangeLow = 0; //this.m_DegreesPerSecondChangeLow;
            weatherComponent.m_DegreesPerSecondChangeMedium = 0; //this.m_DegreesPerSecondChangeMedium;
            weatherComponent.m_DegreesPerSecondChangeHigh = 0; //this.m_DegreesPerSecondChangeHigh;
            weatherComponent.m_TimeToDisplayTempWhenChanged = 0; //this.m_TimeToDisplayTempWhenChanged;
            weatherComponent.m_MinWindSpeedForBlowingSnow = 0; //this.m_MinWindSpeedForBlowingSnow;
            weatherComponent.m_BlowingSnowTransitionSeconds = 0; //this.m_BlowingSnowTransitionSeconds;
            weatherComponent.m_SkyboxHorizonAdjust = new Vector4(); //this.m_SkyboxHorizonAdjust;
            //weatherComponent.RegisterSceneWeatherSets(new WeatherSet); //this.m_WeatherSetOverrides);
            GameManager.GetWindComponent().ApplySceneOverrides(null);
            UniStormWeatherSystem.m_MinimumFogDensityScale = 0; //this.m_MinimumFogDensityScale;
        }

        public static void ProcessingReconnect()
        {
            if(LastConnectedIp != "")
            {
                if(sendMyPosition == true)
                {
                    Disconnect();
                }
                DoConnectToIp(LastConnectedIp);
            }
        }

        public static void SkipRTTime(int h)
        {
            int totaltime = OverridedHourse + h;
            PlayedHoursInOnline = PlayedHoursInOnline + h;
            int leftovers = 0;
            if (totaltime > 24)
            {
                leftovers = totaltime - 24;
                OverridedHourse = 0 + leftovers;
            }else{
                OverridedHourse = OverridedHourse + h;
            }
            MinutesFromStartServer = MinutesFromStartServer + h*60;
            MelonLogger.Msg("Skipping "+ h+" hour(s) now should be "+ OverridedHourse);
            MyMod.EveryInGameMinute();
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

        public static void SleepTracker(List<MyMod.MultiPlayerClientStatus> _status)
        {
            int SleepersNeed = _status.Count;
            int Sleepers = 0;
            int Deads = 0;
            bool EveryOneIsSleeping = false;
            List<int> SleepingHours = new List<int>();

            for (int i = 0; i < _status.Count; i++)
            {
                if (_status[i] != null)
                {
                    MultiPlayerClientStatus mPs = _status[i];
                    //MelonLoader.MelonLogger.Msg("Player ID " + mPs.m_ID + " NAME " + mPs.m_Name + " SLEEP " + mPs.m_Sleep + " DEAD " + mPs.m_Dead);
                    if (mPs.m_Sleep == true || mPs.m_Dead == true)
                    {
                        Sleepers = Sleepers + 1;
                    }
                    if(mPs.m_Dead == true)
                    {
                        Deads = Deads + 1;
                    }

                    if(iAmHost == true)
                    {
                        SleepingHours.Add(playersData[mPs.m_ID].m_SleepHours);
                    }
                }
            }
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

            if(iAmHost == true)
            {
                if (EveryOneIsSleeping == true)
                {
                    if ((GameManager.GetPlayerManagerComponent().PlayerIsSleeping() == true || IsDead == true) && IsCycleSkiping == false)
                    {
                        IsCycleSkiping = true;
                        SleepingHours.Sort();
                        int Skip = SleepingHours[SleepingHours.Count - 1];

                        for (int i = 0; i < playersData.Count; i++)
                        {
                            if (playersData[i] != null)
                            {
                                playersData[i].m_SleepHours = 0;
                            }
                        }
                        if (Skip > 0)
                        {
                            if(IsDead == true)
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

        public static float MinimalDistanceForSpawn = 200;
        public static float MaximalDistanceForAnimalRender = 260;

        public static bool AnyOneClose(float minimalDistance, Vector3 point) // Returns true if someone locates near with this point.
        {
            // First checking if I am myself close to this point, if so end up this very quick, without even checking other.
            float MyDis = Vector3.Distance(GameManager.GetPlayerTransform().position, point);
            if(MyDis <= minimalDistance)
            {
                return true;
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
                            return true;
                        }
                    }
                }
            }
            return false;
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

        private static void EverySecond()
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

            if (GameManager.m_SpawnRegionManager != null && AnimalsController == true)
            {
                Il2CppSystem.Collections.Generic.List<SpawnRegion> Regions = GameManager.GetSpawnRegionManager().m_SpawnRegions;
                for (int i = 0; i < Regions.Count; i++) // Checking each spawn region
                {
                    SpawnRegion spR = Regions[i];
                    GameObject spRobj = Regions[i].gameObject;
                    Vector3 PlayerV3 = GameManager.GetPlayerTransform().position;
                    //if (Vector3.Distance(spR.m_Center, PlayerV3) <= spR.m_Radius + MinimalDistanceForSpawn) // If this region near.

                    if (AnyOneClose(spR.m_Radius + MinimalDistanceForSpawn, spR.m_Center)) // If anyone close to this region.
                    {
                        if (spRobj != null && spRobj.GetComponent<SpawnRegionAnimalsList>() != null)
                        {
                            if (spRobj.GetComponent<SpawnRegionAnimalsList>().m_Animals.Count < spR.CalculateTargetPopulation()) // If animals less than should be
                            {
                                SimulateSpawnFromRegionSpawn(spR.GetComponent<ObjectGuid>().Get()); // Spawn new animals for this region
                            }
                            List<string> AnimalsGuids = spRobj.GetComponent<SpawnRegionAnimalsList>().m_Animals; // Checking already spawned animals.
                            for (int i2 = 0; i2 < AnimalsGuids.Count; i2++)
                            {
                                GameObject animal = ObjectGuidManager.Lookup(AnimalsGuids[i2]);

                                if (animal != null)
                                {
                                    BaseAi AI = animal.GetComponent<BaseAi>();
                                    if(GetClosestDistanceFromEveryone(animal.transform.position) > MaximalDistanceForAnimalRender) // If animal far away from everyone disable it.
                                    {
                                        if(animal.activeSelf == true)
                                        {
                                            animal.SetActive(false);
                                        }
                                    }else{ // If animal close, but has been disabled, re-enable it.
                                        if (animal.activeSelf == false)
                                        {
                                            animal.SetActive(true);
                                        }
                                    }
                                }
                            }
                        }
                    }else{ // If spawn region is far away from everyone.
                        if (spRobj != null && spRobj.GetComponent<SpawnRegionAnimalsList>() != null)
                        {
                            List<string> AnimalsGuids = spRobj.GetComponent<SpawnRegionAnimalsList>().m_Animals;
                            if (AnimalsGuids.Count > 0)
                            {
                                for (int i2 = 0; i2 < AnimalsGuids.Count; i2++) // Checking spawned animals.
                                {
                                    GameObject animal = ObjectGuidManager.Lookup(AnimalsGuids[i2]);
                                    if(animal != null)
                                    {
                                        BaseAi AI = animal.GetComponent<BaseAi>();
                                        // If animal is valid to unload, unloading it.
                                        if (AI.GetAiMode() != AiMode.Dead && AI.GetAiMode() != AiMode.Struggle && (AI.m_CurrentTarget == null || AI.m_CurrentTarget.IsPlayer() == false))
                                        {
                                            UnRegisterAnimal(level_name, AnimalsGuids[i2]);
                                            UnityEngine.Object.DestroyImmediate(animal);
                                        }
                                    }
                                }
                                spRobj.GetComponent<SpawnRegionAnimalsList>().m_Animals = new List<string>();
                            }
                        }
                    }
                }
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
            if (iAmHost && SteamConnect.CanUseSteam && Server.UsingSteamWorks && IsPublicServer)
            {
                SteamConnect.Main.PingMasterServer();
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
                            SteamConnect.Main.ConnectToHost(MyMod.SteamServerWorks);
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

            if (level_name != "Empty" && level_name != "Boot" && level_name != "MainMenu")
            {
                //MelonLogger.Msg("Trying call UpdateMyClothing");
                UpdateMyClothing();
                ApplyOpenables();

                if (Application.isBatchMode && DsServerIsUp == false)
                {
                    DsServerIsUp = true;
                    MelonLogger.Msg(ConsoleColor.Magenta, "[Dedicated server] Server is ready! Have fun!");
                }

                if (ApplyAutoThingsAfterLoaed == 0 && (NeedApplyAutoCMDs == true || AutoHostWhenLoaded == true))
                {
                    MelonLogger.Msg(ConsoleColor.Magenta, "[Start-ups] Going to apply some parameters after load save");
                    ApplyAutoThingsAfterLoaed = 3;
                }

                if(ApplyAutoThingsAfterLoaed > 0)
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
                List<MyMod.MultiPlayerClientStatus> MPStatus = ServerSend.PLAYERSSTATUS();
                PlayersOnServer = MPStatus.Count;
                SleepTracker(MPStatus);
                UpdatePlayerStatusMenu(MPStatus);
                SetAnimalControllers();

                for (int i = 1; i <= MaxPlayers; i++)
                {
                    //MelonLogger.Msg("Client " + i + " no responce time " + Server.clients[i].TimeOutTime+ " Busy "+ Server.clients[i].IsBusy());
                    if (Server.clients[i].IsBusy() == true)
                    {
                        //if(Server.UsingSteamWorks == false)
                        //{
                        Server.clients[i].TimeOutTime = Server.clients[i].TimeOutTime + 1;
                        //}
                        if (Server.clients[i].TimeOutTime > 10)
                        {
                            MelonLogger.Msg("Client " + i + " no responce time " + Server.clients[i].TimeOutTime);
                        }
                        if (Server.clients[i].TimeOutTime > 30)
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
                    }
                }
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
                    using (Packet _packet = new Packet((int)ServerPackets.KEEPITALIVE))
                    {
                        ServerSend.KEEPITALIVE(0, true);
                    }
                    ServerSend.SELECTEDCHARACTER(0, character, true);
                }

                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.KEEPITALIVE))
                    {
                        _packet.Write(true);
                        SendTCPData(_packet);
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
            }
            if (InOnline() == false || iAmHost == true)
            {
                ManageDropsLoads();
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
        public float period = 5f;
        public static float nextActionTimeAniamls = 0.0f;
        public float periodAniamls = 0.3f;
        public float nextActionTimeSecond = 0.0f;
        public float periodSecond = 1f;

        public bool DeathTrigger = false;

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

            //SkinnedMeshRenderer mesh1 = null;
            //SkinnedMeshRenderer mesh2 = null;
            //SkinnedMeshRenderer mesh3 = null;

            //if (animal.name.StartsWith("WILDLIFE_Wolf"))
            //{
            //    //7 Rig, Meshs 12,13
            //    mesh1 = animal.transform.GetChild(7).GetChild(12).gameObject.GetComponent<SkinnedMeshRenderer>();
            //    mesh2 = animal.transform.GetChild(7).GetChild(13).gameObject.GetComponent<SkinnedMeshRenderer>();
            //}
            //else if (animal.name.StartsWith("WILDLIFE_Stag"))
            //{
            //    //23 Mesh
            //    mesh1 = animal.transform.GetChild(23).gameObject.GetComponent<SkinnedMeshRenderer>();
            //}

            //else if (animal.name.StartsWith("WILDLIFE_Rabbit"))
            //{
            //    // 6,7 Meshs
            //    mesh1 = animal.transform.GetChild(6).gameObject.GetComponent<SkinnedMeshRenderer>();
            //    mesh2 = animal.transform.GetChild(7).gameObject.GetComponent<SkinnedMeshRenderer>();
            //}
            //else if (animal.name.StartsWith("WILDLIFE_Moose"))
            //{
            //    // 24,25 Meshs
            //    mesh1 = animal.transform.GetChild(24).gameObject.GetComponent<SkinnedMeshRenderer>();
            //    mesh2 = animal.transform.GetChild(25).gameObject.GetComponent<SkinnedMeshRenderer>();
            //}
            //else if (animal.name.StartsWith("WILDLIFE_Bear"))
            //{
            //    // 10,11,12 Meshs
            //    mesh1 = animal.transform.GetChild(10).gameObject.GetComponent<SkinnedMeshRenderer>();
            //    mesh2 = animal.transform.GetChild(11).gameObject.GetComponent<SkinnedMeshRenderer>();
            //    mesh3 = animal.transform.GetChild(12).gameObject.GetComponent<SkinnedMeshRenderer>();
            //}

            //AnimalUpdates au = animal.GetComponent<AnimalUpdates>();

            //if (mesh1 != null){if(au != null){au.m_Mesh1 = mesh1;}}
            //if (mesh2 != null){if(au != null){au.m_Mesh2 = mesh2;}}
            //if (mesh3 != null){if(au != null){au.m_Mesh3 = mesh3;}}
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

        public static void ReCreateAnimal(GameObject animal, string proxy = "")
        {
            if (animal == null)
            {
                return;
            }
            animal.SetActive(false);
            Vector3 pos = animal.transform.position;
            Quaternion rotation = animal.transform.rotation;
            string _GUID = animal.GetComponent<ObjectGuid>().Get();
            string prefab = GetAnimalPrefabName(animal.name);
            //MelonLogger.Msg("Trying re-create animal " + prefab + " " + _GUID);
            AnimalUpdates AU = animal.GetComponent<AnimalUpdates>();
            float hp = animal.GetComponent<BaseAi>().m_CurrentHP;
            AiMode LastAiState = (AiMode)AU.AP_AiState;

            if(animal.GetComponent<BaseAi>().m_CurrentHP == 0 || AU.m_Hp == 0 && AU.AP_AiState == 2 || animal.GetComponent<Harvestable>() != null && (animal.GetComponent<Harvestable>().enabled == true))
            {
                MelonLogger.Msg("Recreation canceled");
                return;
            }

            if(AU != null)
            {
                AU.m_Banned = true;
            }

            string JsonProx = proxy;

            if(JsonProx == "")
            {
                JsonProx = animal.GetComponent<BaseAi>().Serialize();
            }
            string regionGUID = "";
            if (animal.GetComponent<BaseAi>().m_SpawnRegionParent != null && animal.GetComponent<BaseAi>().m_SpawnRegionParent.gameObject.GetComponent<ObjectGuid>() != null)
            {
                regionGUID = animal.GetComponent<BaseAi>().m_SpawnRegionParent.gameObject.GetComponent<ObjectGuid>().Get();
            }
            UnityEngine.Object.Destroy(animal);
            ObjectGuidManager.UnRegisterGuid(_GUID);
            MelonLogger.Msg("Recreating animal with state "+ LastAiState);
            SpawnAnimal(prefab, pos, _GUID, regionGUID, true, JsonProx, hp, LastAiState);
            return;
        }

        public static void MakeAnimalActive(GameObject animal, bool active, string sRegionGUID)
        {
            //MelonLogger.Msg("Nachinayem kuhat");
            if (animal == null)
            {
                return;
            }

            //MelonLogger.Msg("Animal narmalna");
            if (animal.transform.parent != null && animal.transform.parent.GetComponent<MoveAgent>() != null)
            {
                //UnityEngine.Component.Destroy(animal.transform.parent.GetComponent<MoveAgent>());
                if(active == true)
                {
                    if (animal.GetComponent<BaseAi>().CreateMoveAgent(null) == true && !animal.GetComponent<BaseAi>().GetMoveAgent().Warp(animal.transform.position, 1f, true, -1))
                    {
                        MelonLogger.Msg(ConsoleColor.Red, "[Reactivator] Problems with spawn animal because it has invalid MoveAgent!");
                    }
                }
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

                if (active == true)
                {
                    BaseAi _AI = animal.GetComponent<BaseAi>();

                    GameObject RegionSpawnObj = ObjectGuidManager.Lookup(sRegionGUID);

                    if(RegionSpawnObj != null)
                    {
                        SpawnRegion sp = RegionSpawnObj.GetComponent<SpawnRegion>();
                        if(sp.GetComponent<SpawnRegionAnimalsList>() != null)
                        {
                            SpawnRegionAnimalsList SRAL = sp.GetComponent<SpawnRegionAnimalsList>();
                            if (SRAL.m_Animals.Contains(animal.GetComponent<ObjectGuid>().Get()) == false)
                            {
                                SRAL.m_Animals.Add(animal.GetComponent<ObjectGuid>().Get());
                            }
                        }
                    }
                }
            }
            //MelonLogger.Msg("BaseAi NE SUS");
            if(animal.GetComponent<AnimalUpdates>() != null)
            {
                AnimalUpdates au = animal.GetComponent<AnimalUpdates>();
                au.nextActionDampingOn = Time.time+au.dampingOn_perioud;
                au.m_DampingIgnore = true;
            }
        }

        public static void SpawnAnimal(string prefabName, Vector3 v3spawn, string _guid, string sRegionGUID, bool recreateion = false, string prox = "", float health = -1, AiMode state = AiMode.Idle)
        {
            GameObject checkanimal = ObjectGuidManager.Lookup(_guid);
            if(checkanimal != null)
            {
                MelonLogger.Msg("Animal " + _guid + " had two instances, destroy old one ");
                UnityEngine.Object.Destroy(checkanimal);
                ObjectGuidManager.UnRegisterGuid(_guid);
            }

            //UnityEngine.Object original = Resources.Load(prefabName);
            //GameObject animal = UnityEngine.Object.Instantiate(original) as GameObject;

            GameObject animal = UnityEngine.Object.Instantiate<GameObject>(Resources.Load(prefabName).Cast<GameObject>());
            animal.name = prefabName;
            animal.transform.position = v3spawn;

            //MoveAgent AG = animal.transform.parent.GetComponent<MoveAgent>();
            //AG.m_Target

            BaseAi _AI = animal.GetComponent<BaseAi>();
            _AI.m_SpawnPos = v3spawn;

            //_AI.m_SpawnRegionParent.InstantiateSpawn

            if (animal.GetComponent<ObjectGuid>() != null)
            {
                animal.GetComponent<ObjectGuid>().m_Guid = _guid;
                ObjectGuidManager.RegisterGuid(_guid, animal);
            }else{
                animal.AddComponent<ObjectGuid>();
                animal.GetComponent<ObjectGuid>().m_Guid = _guid;
                ObjectGuidManager.RegisterGuid(_guid, animal);
            }

            GameObject RegionSpawnObj = ObjectGuidManager.Lookup(sRegionGUID);

            if (RegionSpawnObj != null && RegionSpawnObj.GetComponent<SpawnRegionAnimalsList>() != null)
            {
                SpawnRegionAnimalsList SML = RegionSpawnObj.GetComponent<SpawnRegionAnimalsList>();
                if(SML.m_Animals.Contains(_guid) == false)
                {
                    SML.m_Animals.Add(_guid);
                }
                
                if (health != -1)
                {
                    _AI.m_CurrentHP = health;
                }
            }

            AnimalUpdates au = animal.GetComponent<AnimalUpdates>();

            if (au == null)
            {
                animal.AddComponent<AnimalUpdates>();

                au = animal.GetComponent<AnimalUpdates>();
            }
            au.m_ToGo = v3spawn;
            if (health != -1)
            {
                au.m_Hp = health;
                _AI.m_CurrentHP = health;
            }

            if (recreateion == true)
            {
                if(prox != "")
                {
                    //MelonLogger.Msg("Spawn Recreation deserialize " + prox);
                    _AI.Deserialize(prox);
                    if (health != -1)
                    {
                        _AI.m_CurrentHP = health;
                    }
                }

                if (AnimalsController == false)
                {
                    au.m_ClientController = instance.myId;
                    au.m_InActive = false;
                }
                else
                {
                    au.m_ClientControlled = false;
                    au.m_InActive = false;
                }
            }
            au.m_Animal = animal;
            au.m_RemoteSpawned = true;
            animal.transform.position = v3spawn;
            au.m_RemoteSpawned = true;
            au.m_RightName = prefabName;

            //BaseAiManager.CreateMoveAgent(animal.transform, _AI, v3spawn);
            if (_AI.CreateMoveAgent(null) == true && !_AI.GetMoveAgent().Warp(v3spawn, 1f, true, -1))
            {
                MelonLogger.Msg(ConsoleColor.Red, "[Network AnimalSpawn] Problems with spawn animal " + _guid + " because it has invalid MoveAgent!");
            }

            //MelonLogger.Msg(animal.GetComponent<ObjectGuid>().Get() + " Created " + prefabName);
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

        //public static void SetAnimalsSpawns()
        //{
        //    if(GameManager.m_SpawnRegionManager != null)
        //    {
        //        if(InOnline() == false)
        //        {
        //            GameManager.GetSpawnRegionManager().m_SpawnRegionDisableDistance = 200;
        //            GameManager.GetSpawnRegionManager().m_DisallowDespawnBelowDistance = 150;
        //            GameManager.GetSpawnRegionManager().m_AllowDespawnOnscreenDistance = 250;
        //            GameManager.GetSpawnRegionManager().m_AllowSpawnOnscreenDistance = 175;
        //        }else{
        //            GameManager.GetSpawnRegionManager().m_SpawnRegionDisableDistance = float.PositiveInfinity;
        //            GameManager.GetSpawnRegionManager().m_DisallowDespawnBelowDistance = float.PositiveInfinity;
        //            GameManager.GetSpawnRegionManager().m_AllowDespawnOnscreenDistance = float.PositiveInfinity;
        //            GameManager.GetSpawnRegionManager().m_AllowSpawnOnscreenDistance = float.NegativeInfinity;
        //        }
        //        //MelonLogger.Msg("m_SpawnRegionDisableDistance " + GameManager.GetSpawnRegionManager().m_SpawnRegionDisableDistance); //200
        //        //MelonLogger.Msg("m_DisallowDespawnBelowDistance " + GameManager.GetSpawnRegionManager().m_DisallowDespawnBelowDistance); //150
        //        //MelonLogger.Msg("m_AllowDespawnOnscreenDistance " + GameManager.GetSpawnRegionManager().m_AllowDespawnOnscreenDistance); //250
        //        //MelonLogger.Msg("m_AllowSpawnOnscreenDistance " + GameManager.GetSpawnRegionManager().m_AllowSpawnOnscreenDistance); //175
        //    }
        //}

        public static void SetAnimalControllers()
        {
            if(MaxPlayers > playersData.Count)
            {
                return;
            }
            
            for (int i = 0; i < MaxPlayers; i++)
            {
                if (playersData[i] != null)
                {
                    if(i == 0)
                    {
                        bool shouldBeController = false;
                        if (IsDead == false)
                        {
                            shouldBeController = ShouldbeAnimalController(MyTicksOnScene, levelid, 0);
                        }else{
                            shouldBeController = false;
                        }
                        AnimalsController = shouldBeController;
                    }else{
                        if (Server.clients[i].IsBusy() == true)
                        {
                            bool shouldBeController = false;
                            if(playersData[i].m_AnimState != "Knock")
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
        }

        public static GameObject WaitingRoom = null;
        public static Vector3 ReturnFromWaitngRoomV3 = new Vector3(0, 0, 0);
        public static List<GameObject> WaitingRoomGears = new List<GameObject>();
        public static bool InWaitingRoom = false;

        public static void SendToWaitngRoom()
        {
            Vector3 pV3 = GameManager.GetPlayerTransform().position;
            Vector3 spawn_V3 = new Vector3(pV3.x, pV3.y + 1000, pV3.z);
            GameObject room = null;

            if (WaitingRoom == null)
            {
                GameObject RoomPrefab = LoadedBundle.LoadAsset<GameObject>("WaitRoom");
                room = GameObject.Instantiate(RoomPrefab);
                room.transform.position = spawn_V3;
                WaitingRoom = room;
                ReturnFromWaitngRoomV3 = GameManager.GetPlayerTransform().position;
                GameManager.GetPlayerManagerComponent().TeleportPlayer(room.transform.GetChild(3).position, GameManager.GetMainCamera().transform.rotation);
            } else
            {
                room = WaitingRoom;
                if (ReturnFromWaitngRoomV3 != new Vector3(0, 0, 0))
                {
                    ReturnFromWaitngRoomV3 = GameManager.GetPlayerTransform().position;
                }
                GameManager.GetPlayerManagerComponent().TeleportPlayer(room.transform.GetChild(3).position, GameManager.GetMainCamera().transform.rotation);
            }

            InWaitingRoom = true;


            for (int i = 0; i < WaitingRoomGears.Count; i++)
            {
                if (WaitingRoomGears[i] != null)
                {
                    if (GameManager.GetPlayerManagerComponent().m_ItemInHands != null && GameManager.GetPlayerManagerComponent().m_ItemInHands == WaitingRoomGears[i].GetComponent<GearItem>())
                    {
                        MelonLogger.Msg("Waiting room item was in hands, trying unequip");
                        GameManager.GetPlayerManagerComponent().UseInventoryItem(WaitingRoomGears[i].gameObject.GetComponent<GearItem>());
                    }
                    UnityEngine.Object.Destroy(WaitingRoomGears[i]);
                }
            }

            int cans = 51;

            for (int i = 0; i < cans; i++)
            {
                Vector3 can_spawnV3 = room.transform.GetChild(2).GetChild(i).position;

                GameObject gear = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("GEAR_RecycledCan"));
                gear.transform.position = can_spawnV3;
                WaitingRoomGears.Add(gear);
            }

            int ammoboxes = 5;

            for (int i = 0; i < ammoboxes; i++)
            {
                Vector3 can_spawnV3 = room.transform.GetChild(5).GetChild(i).position;

                GameObject gear = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("GEAR_RifleAmmoBox"));
                gear.transform.position = can_spawnV3;
                WaitingRoomGears.Add(gear);
            }

            GameObject rifle = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("GEAR_Rifle"));
            rifle.transform.position = room.transform.GetChild(4).position;
            rifle.GetComponent<GearItem>().m_CurrentHP = rifle.GetComponent<GearItem>().m_MaxHP;
            rifle.GetComponent<GearItem>().m_GunItem.m_FiringRateSeconds = 0.1f;
            rifle.GetComponent<GearItem>().m_GunItem.m_ClipSize = 100;
            rifle.GetComponent<GearItem>().m_GunItem.m_RoundsInClip = 100;
            //rifle.GetComponent<GearItem>().m_GunItem.m_AllowHipFire = true;
            //rifle.GetComponent<GearItem>().m_GunItem.m_MultiplierReload = 1;
            //rifle.GetComponent<GearItem>().m_GunItem.m_ReloadCoolDownSeconds = 0.1f;
            //rifle.GetComponent<GearItem>().m_GunItem.m_YawRecoilMax = 0.1f;
            //rifle.GetComponent<GearItem>().m_GunItem.m_YawRecoilMin = 0.1f;
            //rifle.GetComponent<GearItem>().m_GunItem.m_PitchRecoilMax = 0.1f;
            //rifle.GetComponent<GearItem>().m_GunItem.m_PitchRecoilMin = 0.1f;
            WaitingRoomGears.Add(rifle);
        }

        public static void SendToAnimalRoom()
        {
            Vector3 pV3 = GameManager.GetPlayerTransform().position;
            Vector3 spawn_V3 = new Vector3(pV3.x, pV3.y + 1000, pV3.z);
            

            GameObject RoomPrefab = LoadedBundle.LoadAsset<GameObject>("WaitRoom");
            GameObject room = GameObject.Instantiate(RoomPrefab); 
            room.transform.position = spawn_V3;
            GameManager.GetPlayerManagerComponent().TeleportPlayer(room.transform.GetChild(3).position, GameManager.GetMainCamera().transform.rotation);

            Vector3 v3AnimalSpawn = room.transform.GetChild(4).position;
            GameObject animal = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("WILDLIFE_WOLF").Cast<GameObject>());
            MakeAnimalActive(animal, false, "");
            animal.transform.position = v3AnimalSpawn;
            animal.AddComponent<AnimalUpdates>();
            AnimalUpdates au = animal.GetComponent<AnimalUpdates>();
            au.m_Animal = animal;
            animal.name = animal.name + "_DEBUG";
            au.m_ToGo = v3AnimalSpawn;
            animal.transform.position = v3AnimalSpawn;
            au.m_RemoteSpawned = true;
            au.m_DebugAnimal = true;
            if (animal.GetComponent<ObjectGuid>() == null)
            {
                animal.AddComponent<ObjectGuid>();
                animal.GetComponent<ObjectGuid>().m_Guid = ObjectGuidManager.GenerateNewGuidString();
            }
            MelonLogger.Msg("Created DEBUG ANIMAL");
        }

        public static void DestoryWaitingRoom()
        {
            for (int i = 0; i < WaitingRoomGears.Count; i++)
            {
                if (WaitingRoomGears[i] != null)
                {
                    if (GameManager.GetPlayerManagerComponent().m_ItemInHands != null && GameManager.GetPlayerManagerComponent().m_ItemInHands == WaitingRoomGears[i].GetComponent<GearItem>())
                    {
                        MelonLogger.Msg("Waiting room item was in hands, trying unequip");
                        GameManager.GetPlayerManagerComponent().UseInventoryItem(WaitingRoomGears[i].gameObject.GetComponent<GearItem>());
                    }
                    UnityEngine.Object.Destroy(WaitingRoomGears[i]);
                }
            }
            if (WaitingRoom != null)
            {
                UnityEngine.Object.Destroy(WaitingRoom);
            }
        }

        public static void ReturnFromWaitingRoom()
        {
            if (ReturnFromWaitngRoomV3 != new Vector3(0, 0, 0))
            {
                DestoryWaitingRoom();
                GameManager.GetPlayerManagerComponent().TeleportPlayer(ReturnFromWaitngRoomV3, GameManager.GetMainCamera().transform.rotation);
                InWaitingRoom = false;
                ReturnFromWaitngRoomV3 = new Vector3(0, 0, 0);
            }
        }

        public string mWeatherProxy = "";
        public string mWeatherTransitionProxy = "";
        public string mWindProxy = "";



        public static SaveSlotSync PendingSave = null;

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

        public static void SelectBagesForConnection()
        {
            GameAudioManager.PlayGuiConfirm();
            if (m_Panel_MainMenu != null)
            {
                if (PendingSave.m_SaveSlotType == (int)SaveSlotType.SANDBOX && m_Panel_MainMenu.GetNumUnlockedFeats() > 0)
                {
                    m_Panel_MainMenu.SelectWindow(m_Panel_MainMenu.m_SelectFeatWindow);
                }else{
                    ForcedCreateSave(PendingSave);
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
            if (ServerConfig.m_PlayersSpawnType == 0) // Same as host.
            {
                SelectGenderForConnection();
            }
            else if (ServerConfig.m_PlayersSpawnType == 1) // Can select
            {
                //if(Data.m_ExperienceMode == (int)ExperienceModeType.Interloper)
                //{
                //    Data.m_Location = (int)GameRegion.RandomRegion;
                //    SelectGenderForConnection();
                //}else{
                    InterfaceManager.TrySetPanelEnabled<Panel_SelectRegion_Map>(true);
                //}
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


        public static void ForcedCreateSave(SaveSlotSync Data)
        {
            Episode Ep = (Episode)Data.m_Episode;
            SaveSlotType SST = (SaveSlotType)Data.m_SaveSlotType;
            int Seed = Data.m_Seed;
            ExperienceModeType ExpType = (ExperienceModeType)Data.m_ExperienceMode;
            GameRegion Region = (GameRegion)Data.m_Location;

            MelonLogger.Msg("Creating save slot " + Seed);
            GameManager.GetExperienceModeManagerComponent().SetExperienceModeType(ExpType);
            //SaveGameSystem.SetCurrentSaveInfo(Ep, SST, SaveGameSlots.GetUnusedGameId(), null);

            SaveGameSystem.m_CurrentEpisode = Ep;
            SaveGameSystem.m_CurrentGameId = SaveGameSlots.GetUnusedGameId();
            SaveGameSystem.m_CurrentGameMode = SST;
            SaveGameSystem.m_CurrentSaveName = SaveGameSlots.BuildSlotName(SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameMode, SaveGameSystem.m_CurrentGameId);
            
            SaveGameSlots.SetSlotDisplayName(SaveGameSystem.m_CurrentSaveName, Seed + "");
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

            //InterfaceManager.m_Panel_OptionsMenu.m_State.m_StartRegion = Region;
            GameManager.Instance().LaunchSandbox();
            GameManager.m_SceneTransitionData.m_GameRandomSeed = Seed;
            MyMod.PendingSave = null;
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
            if (message.m_Message == "!dupesboxes")
            {
                if (iAmHost == true)
                {
                    if (ServerConfig.m_DuppedContainers == false)
                    {
                        ServerConfig.m_DuppedContainers = true;
                    }else{
                        ServerConfig.m_DuppedContainers = false;
                    }
                    message.m_Type = 0;
                    message.m_By = MyChatName;
                    message.m_Message = "Server configuration parameter ServerConfig.m_DuppedContainers now is " + ServerConfig.m_DuppedContainers;
                    needSync = true;
                    ServerSend.SERVERCFGUPDATED();
                }else{
                    message.m_Type = 0;
                    message.m_By = MyChatName;
                    message.m_Message = "You not a host to change this!";
                    needSync = false;
                }
            }
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
            if (message.m_Message == "!invite" && SteamConnect.CanUseSteam == true && iAmHost == true && Server.UsingSteamWorks == true)
            {
                message.m_Type = 0;
                message.m_Message = SteamConnect.Main.GetFriends();
            }
            if (message.m_Message.StartsWith("!invite ") && SteamConnect.CanUseSteam == true && iAmHost == true && Server.UsingSteamWorks == true)
            {
                string text = message.m_Message;
                int friendNum = int.Parse(text.Replace("!invite ", ""));
                message.m_Type = 0;
                message.m_By = MyChatName;
                message.m_Message = SteamConnect.Main.InviteFriendByIndex(friendNum); ;
                needSync = false;
                
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
            if (message.m_Message.StartsWith("!hz ") == true)
            {
                string text = message.m_Message;
                VoiceChatFrequencyHz = Convert.ToInt32(text.Replace("!hz ", ""));
                message.m_Type = 0;
                message.m_By = MyChatName;
                message.m_Message = "Your new name " + MyChatName;
                needSync = true;
                MultiplayerChatMessage NewMsg = new MultiplayerChatMessage();
                message.m_Type = 0;
                message.m_By = MyChatName;
                message.m_Message = "New voice chat frequency is " + VoiceChatFrequencyHz + "hz now!";
                SendMessageToChat(NewMsg, false);
            }
            if (message.m_Message.StartsWith("!mic ") == true)
            {
                string text = message.m_Message;
                int wannaDatId = Convert.ToInt32(text.Replace("!mic ", ""));
                
                message.m_Type = 0;
                message.m_By = MyChatName;
                if(wannaDatId >= 0 && wannaDatId < Microphone.devices.Count)
                {
                    MyMicrophoneID = wannaDatId;
                    message.m_Message = "Using microphone " + Microphone.devices[wannaDatId];
                }else{
                    message.m_Message = "Using microphone with ID "+ wannaDatId + " not exist!";
                }
                needSync = false;
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

        public static void SetupMenuPlayer(GameObject m_Player, int Hat, int Top, int Pants, int Boots, string ArmsType)
        {
            GameObject clothing = m_Player.transform.GetChild(0).gameObject;
            GameObject arms_middle = m_Player.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
            GameObject arms_short = m_Player.transform.GetChild(1).GetChild(0).GetChild(1).gameObject;
            GameObject arms_tiny = m_Player.transform.GetChild(1).GetChild(0).GetChild(2).gameObject;

            int hatsCount = clothing.transform.GetChild(0).childCount;
            int topsCount = clothing.transform.GetChild(1).childCount;
            int pantsCount = clothing.transform.GetChild(2).childCount;
            int bootsCount = clothing.transform.GetChild(3).childCount;
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

            clothing.transform.GetChild(0).transform.GetChild(Hat).gameObject.SetActive(true);
            clothing.transform.GetChild(1).transform.GetChild(Top).gameObject.SetActive(true);
            clothing.transform.GetChild(2).transform.GetChild(Pants).gameObject.SetActive(true);
            clothing.transform.GetChild(3).transform.GetChild(Boots).gameObject.SetActive(true);
            arms_tiny.SetActive(false);
            arms_short.SetActive(false);
            arms_middle.SetActive(false);
            if (ArmsType == "Tiny")
            {
                arms_tiny.SetActive(true);
            }else if (ArmsType == "Short"){
                arms_short.SetActive(true);
            }else{
                arms_middle.SetActive(true);
            }
        }

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
                    m_Player.AddComponent<AudioSource>();
                    m_Player.AddComponent<MultiplayerPlayerAnimator>().m_Animer = m_Player.GetComponent<Animator>();
                    m_Player.AddComponent<MultiplayerPlayerClothingManager>().m_Player = m_Player;
                    m_Player.AddComponent<MultiplayerPlayerVoiceChatPlayer>().aSource = m_Player.GetComponent<AudioSource>();
                    m_Player.GetComponent<AudioSource>().loop = false;
                    m_Player.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Logarithmic;
                    m_Player.GetComponent<AudioSource>().dopplerLevel = 0;
                    m_Player.GetComponent<AudioSource>().spatialBlend = 1;
                    m_Player.GetComponent<AudioSource>().minDistance = 13f;

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
        }

        public static bool HasWaitForConnect = false;


        //Legacy UI
        public static InterfaceManager m_InterfaceManager;
        public static Panel_SelectSurvivor m_Panel_SelectSurvivor;
        public static Panel_MainMenu m_Panel_MainMenu;
        public static Panel_SelectRegion m_Panel_SelectRegion;

        public static void NoCustomExp()
        {
            if (m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
            {
                InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, "You can't use custom experience mode in online! This case to major desync!", Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null);
            }
        }

        public static void DoWaitForConnect()
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
                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Waiting, "Connecting...", "\nPlease wait, if you see this message for too long, you have connection problems. If you using local emulators this total sure your connection problem.", Panel_Confirmation.ButtonLayout.Button_0, Panel_Confirmation.Background.Transperent, null, null);
                    HasWaitForConnect = true;
                }
            }
        }
        public static Packet LastPacketForRepeat = null;
        public static int SecondsLeftUntilWorryAboutPacket = -1;
        public static void SetRepeatPacket(Packet Pak)
        {
            LastPacketForRepeat = Pak;
            SecondsLeftUntilWorryAboutPacket = 20;
        }
        public static void DiscardRepeatPacket()
        {
            SecondsLeftUntilWorryAboutPacket = -1;
            LastPacketForRepeat = null;
        }

        public static void RepeatLastRequest()
        {
            RemovePleaseWait();
            if(LastPacketForRepeat != null)
            {
                SlicedJsonDataBuffer.Clear();
                SecondsLeftUntilWorryAboutPacket = 20;
                DoPleaseWait("Something wrong", "Timed out on response from host for last request. Trying to repeat request to correct the situation, please wait...");
                SendUDPData(LastPacketForRepeat);
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

        public static void PlayVoiceFromPlayerObject(GameObject m_Player, byte[] voiceData, int samples)
        {
            Il2CppStructArray<float> clipData = new float[samples];
            int readPos = 0;
            for (int i = 0; i < clipData.Length; i++)
            {
                float _value = BitConverter.ToSingle(voiceData, readPos);
                clipData[i] = _value;
                readPos += 4;
            }
            AudioSource audioSource = m_Player.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.clip = AudioClip.Create("Voice", VoiceChatFrequencyHz, 1, VoiceChatFrequencyHz, false);
                audioSource.clip.SetData(clipData, 0);
                audioSource.Play();

                //AudioClip NewClip = AudioClip.Create("Voice", VoiceChatFrequencyHz, 1, VoiceChatFrequencyHz, false);
                //NewClip.SetData(clipData, 0);
                //audioSource.PlayOneShot(NewClip);
            }
        }

        public static void ProcessVoiceChatData(int from, byte[] CompressedData, int samples)
        {
            //MelonLogger.Msg("Going to decompress...");
            byte[] decompressedData = MyMod.Decompress(CompressedData);
            //MelonLogger.Msg("Decompressed data " + decompressedData.Length + " bytes");
            if (MyMod.players[from] != null)
            {
                if (MyMod.playersData[from].m_Levelid == MyMod.levelid && MyMod.playersData[from].m_LevelGuid == MyMod.level_guid)
                {
                    if (MyMod.players[from].GetComponent<MyMod.MultiplayerPlayerVoiceChatPlayer>() != null)
                    {
                        MyMod.MultiplayerPlayerVoiceChatPlayer mPVoice = MyMod.players[from].GetComponent<MyMod.MultiplayerPlayerVoiceChatPlayer>();
                        MyMod.VoiceChatQueueElement DataForQueue = new MyMod.VoiceChatQueueElement();
                        DataForQueue.m_VoiceData = decompressedData;
                        DataForQueue.m_Samples = samples;
                        mPVoice.VoiceQueue.Add(DataForQueue);
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

        public static void TrackWhenRecordOver()
        {
            if (GameManager.m_PlayerObject == null)
            {
                return;
            }

            AudioSource audioSource = GameManager.GetPlayerObject().GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = GameManager.GetPlayerObject().AddComponent<AudioSource>();
            }

            DoingRecord = KeyboardUtilities.InputManager.GetKey(KeyCode.V);

            if (audioSource != null)
            {
                if(Microphone.IsRecording(MyMicrophoneID) == false)
                {
                    if(DoingRecord == true)
                    {
                        if (audioSource.clip != null)
                        {
                            TrackVoiceToSend();
                            audioSource.clip = null;
                        }
                        audioSource.loop = false;
                        audioSource.clip = Microphone.StartRecord(MyMicrophoneID, false, 1, VoiceChatFrequencyHz);
                    }
                }else{
                    if(DoingRecord == false)
                    {
                        Microphone.EndRecord(MyMicrophoneID);
                    }
                }
            }

            if(MicrophoneIdicator != null)
            {
                MicrophoneIdicator.SetActive(DoingRecord);
            }
        }

        public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool _3D, bool stream)
        {
            int i;

            for (i = 0; i < samples.Count; i++)
            {
                if (Mathf.Abs(samples[i]) > min)
                {
                    break;
                }
            }
            //MelonLogger.Msg("First samples.RemoveRange " + i);

            if(i > 0)
            {
                samples.RemoveRange(0, i);
            }
            for (i = samples.Count - 1; i > 0; i--)
            {
                if (Mathf.Abs(samples[i]) > min)
                {
                    break;
                }
            }
            int debugInt = samples.Count - i;


            //MelonLogger.Msg("Second samples.RemoveRange " + i + " samples.Count " + samples.Count + " samples.Count-i " + debugInt);
            if (i > 0 && debugInt > 0)
            {
                samples.RemoveRange(i, samples.Count - i);
            }

            if (samples.Count > 0)
            {
                var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, _3D, stream);

                clip.SetData(samples.ToArray(), 0);

                return clip;
            }else{
                return null;
            }
        }

        public static void TrackVoiceToSend()
        {
            if (GameManager.m_PlayerObject == null)
            {
                return;
            }

            AudioSource audioSource = GameManager.GetPlayerObject().GetComponent<AudioSource>();
            if (audioSource != null && audioSource.clip != null)
            {
                Il2CppStructArray<float> clipData = new float[audioSource.clip.samples];
                //MelonLogger.Msg("clipData samples " + audioSource.clip.samples);
                audioSource.clip.GetData(clipData, 0);
                Il2CppStructArray<float> CleanedclipData = new float[audioSource.clip.samples];
                AudioClip CleanedClip = TrimSilence(clipData.ToList(), 0.017f, audioSource.clip.channels, VoiceChatFrequencyHz, false, false);
                if(CleanedClip == null)
                {
                    //MelonLogger.Msg("Voice file has been empty after cleanup");
                    return;
                }

                CleanedClip.GetData(CleanedclipData, 0);
                //MelonLogger.Msg("Cleanned clipData samples " + CleanedClip.samples);

                List<byte> BytesBuffer = new List<byte>();
                for (int i = 0; i < CleanedclipData.Length; i++)
                {
                    BytesBuffer.AddRange(BitConverter.GetBytes(CleanedclipData[i]));
                }
                //MelonLogger.Msg("BytesBuffer contains " + BytesBuffer.Count + " bytes");

                byte[] compressedArray = Compress(BytesBuffer.ToArray());

                //MelonLogger.Msg("CompressedArray contains " + compressedArray.Length + " bytes");
                if (iAmHost == true)
                {
                    if (Server.UsingSteamWorks == true)
                    {
                        ServerSend.VOICECHAT(0, compressedArray, audioSource.clip.samples, true);
                    }else{
                        //SendLargeArrayToAll(compressedArray, "VOICE", audioSource.clip.samples);
                    }
                }
                if(sendMyPosition == true)
                {
                    if(ConnectedSteamWorks == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.VOICECHAT))
                        {
                            _packet.Write(compressedArray.Length);
                            _packet.Write(audioSource.clip.samples);
                            _packet.Write(compressedArray);
                            SendTCPData(_packet);
                        }
                    }else{
                        //SendLargeArray(compressedArray, 0, "VOICE", audioSource.clip.samples);
                    }
                }

                bool Debug = false;

                if(Debug == true)
                {
                    if (VoiceTestDummy == null)
                    {
                        VoiceTestDummy = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        VoiceTestDummy.transform.position = GameManager.GetPlayerObject().transform.position;
                        VoiceTestDummy.AddComponent<AudioSource>();
                        VoiceTestDummy.GetComponent<AudioSource>().loop = false;
                        VoiceTestDummy.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Logarithmic;
                        VoiceTestDummy.GetComponent<AudioSource>().dopplerLevel = 0;
                        VoiceTestDummy.GetComponent<AudioSource>().spatialBlend = 1;
                        VoiceTestDummy.GetComponent<AudioSource>().minDistance = 13f;
                    }
                    if (VoiceTestDummy.GetComponent<AudioSource>() != null)
                    {
                        byte[] decompressedData = MyMod.Decompress(compressedArray);
                        MelonLogger.Msg("Decompressed bytes array contains " + decompressedData.Length + " elements!");
                        PlayVoiceFromPlayerObject(VoiceTestDummy, decompressedData, audioSource.clip.samples);
                    }
                }
            }
        }

        public static void FakeDeath()
        {
            if(GameManager.m_PlayerManager == null || GameManager.m_Condition == null)
            {
                return;
            }
            
            Condition con = GameManager.GetConditionComponent();

            if(InOnline() == true)
            {
                con.m_NeverDie = true;
                if (Utils.IsZero(con.m_CurrentHP, 0.0001f))
                {
                    if (IsDead == false)
                    {
                        con.MaybePlayPlayerInjuredVoiceOver();
                        IsDead = true;
                        FakeDeathScreen();
                        ConsoleManager.CONSOLE_save();
                    }
                    IsDead = true;
                }
                if(IsDead == true)
                {
                    GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);
                    GameManager.GetPlayerManagerComponent().m_Ghost = true;
                }
            }else{
                if(con.m_NeverDie == true)
                {
                    con.m_NeverDie = false;
                }
            }
        }

        public static void FinishedLoading()
        {
            //if (iAmHost == true)
            //{
            //    ServerSend.LEVELID(0, levelid, true);
            //    ServerSend.LEVELGUID(0, level_guid, true);
            //}
            //if (sendMyPosition == true)
            //{
            //    using (Packet _packet = new Packet((int)ClientPackets.LEVELID))
            //    {
            //        _packet.Write(levelid);
            //        SendTCPData(_packet);
            //    }
            //    using (Packet _packet = new Packet((int)ClientPackets.LEVELGUID))
            //    {
            //        _packet.Write(level_guid);
            //        SendTCPData(_packet);
            //    }
            //}
            RegularUpdateSeconds = 2;
            MelonLogger.Msg("Sending my level id");
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

        public static void AddDroppedGear(int GearID, int SearchKey, string DataProxy, string lvlKey, ExtraDataForDroppedGear extra)
        {        
            if (DroppedGears.ContainsKey(lvlKey) == false)
            {
                LoadDropsForScene(lvlKey);
            }

            Dictionary<int, SlicedJsonDroppedGear> LevelDrops;

            if (DroppedGears.TryGetValue(lvlKey, out LevelDrops) == false)
            {
                DroppedGears.Add(lvlKey, new Dictionary<int, SlicedJsonDroppedGear>());
                if (DroppedGears.TryGetValue(lvlKey, out LevelDrops) == false)
                {
                    MelonLogger.Msg(ConsoleColor.Red, "Idk why, but can't create dictionary for "+ lvlKey);
                }
            }
            SlicedJsonDroppedGear exist;
            if (LevelDrops.TryGetValue(SearchKey, out exist) == false)
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
                LevelDrops.Add(SearchKey, element);
            }
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
            string lvlKey = levelid+level_guid;

            if (obj.GetComponent<DroppedGearDummy>() != null)
            {
                SearchKey = obj.GetComponent<DroppedGearDummy>().m_SearchKey;
            }else{
                MelonLogger.Msg(ConsoleColor.Red, "DroppedGearDummy is not exist by somereason...");
                return;
            }

            MelonLogger.Msg("Searching for "+ GearName +" with hash "+ SearchKey +" ID "+ GearID);

            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.REQUESTPLACE))
                {
                    _packet.Write(SearchKey);
                    _packet.Write(lvlKey);
                    SetRepeatPacket(_packet);
                    SendTCPData(_packet);
                }
                return;
            }

            Dictionary<int, SlicedJsonDroppedGear> LevelDrops;
            if (DroppedGears.TryGetValue(lvlKey, out LevelDrops) == true)
            {
                SlicedJsonDroppedGear DataProxy;
                if (LevelDrops.TryGetValue(SearchKey, out DataProxy) == true)
                {
                    MelonLogger.Msg("Found " + SearchKey);
                    LevelDrops.Remove(SearchKey);
                    string gearName = "";

                    if(GearID == -1)
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

                    if(newGear == null)
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
                    UnityEngine.Object.DestroyImmediate(obj);
                    ServerSend.PICKDROPPEDGEAR(0, SearchKey, true);
                }else{
                    MelonLogger.Msg("Gear with hash " + SearchKey + " is missing!");
                }
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
            string lvlKey = levelid + level_guid;

            if (obj.GetComponent<DroppedGearDummy>() != null)
            {
                SearchKey = obj.GetComponent<DroppedGearDummy>().m_SearchKey;
            }else{
                MelonLogger.Msg(ConsoleColor.Red, "DroppedGearDummy by somereason...");
                return;
            }

            MelonLogger.Msg("Searching for " + GearName + " with hash " + SearchKey + " ID " + GearID);

            if (sendMyPosition == true)
            {
                string gearName = GetGearNameByID(GearID);
                GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject(gearName), v3, rot);
                newGear.name = CloneTrimer(newGear.name);
                newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;
                newGear.GetComponent<GearItem>().m_Bed.SetState(BedRollState.Placed);
                newGear.AddComponent<FakeBedDummy>().m_LinkedFakeObject = obj;
                GameManager.GetPlayerManagerComponent().ProcessBedInteraction(newGear.GetComponent<GearItem>().m_Bed);
                return;
            }

            Dictionary<int, SlicedJsonDroppedGear> LevelDrops;
            if (DroppedGears.TryGetValue(lvlKey, out LevelDrops) == true)
            {
                SlicedJsonDroppedGear DataProxy;
                if (LevelDrops.TryGetValue(SearchKey, out DataProxy) == true)
                {
                    MelonLogger.Msg("Found " + SearchKey);
                    string gearName = GetGearNameByID(GearID);
                    GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject(gearName), v3, rot);
                    newGear.name = CloneTrimer(newGear.name);
                    newGear.GetComponent<GearItem>().m_BeenInPlayerInventory = true;
                    newGear.GetComponent<GearItem>().m_Bed.SetState(BedRollState.Placed);
                    newGear.AddComponent<FakeBedDummy>().m_LinkedFakeObject = obj;
                    GameManager.GetPlayerManagerComponent().ProcessBedInteraction(newGear.GetComponent<GearItem>().m_Bed);
                }else{
                    MelonLogger.Msg("Gear with hash " + SearchKey + " is missing!");
                }
            }
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

            Vector3 v3 = obj.transform.position;
            Quaternion rot = obj.transform.rotation;
            int SearchKey = 0;
            string lvlKey = levelid+level_guid;

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
                    _packet.Write(lvlKey);
                    SetRepeatPacket(_packet);
                    SendTCPData(_packet);
                }
                return;
            }

            Dictionary<int, SlicedJsonDroppedGear> LevelDrops;
            if (DroppedGears.TryGetValue(lvlKey, out LevelDrops) == true)
            {
                SlicedJsonDroppedGear DataProxy;
                if (LevelDrops.TryGetValue(SearchKey, out DataProxy) == true)
                {
                    MelonLogger.Msg("Found " + SearchKey);
                    LevelDrops.Remove(SearchKey);
                    string gearName = "";

                    if(GearID == -1)
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

                    if(newGear == null)
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
                    DroppedGearDummy DGD = obj.GetComponent<DroppedGearDummy>();
                    if(DGD != null)
                    {
                        if(DGD.m_Extra.m_GoalTime != 0 && DGD.m_Extra.m_GoalTime != -1)
                        {
                            GearItem gear = newGear.GetComponent<GearItem>();
                            if(gear.m_EvolveItem != null)
                            {
                                int days = Convert.ToInt32(gear.m_EvolveItem.m_TimeToEvolveGameDays);
                                int hours = days * 24;
                                int minutes = hours * 60;

                                int minutesOnDry = MyMod.MinutesFromStartServer - DGD.m_Extra.m_DroppedTime;

                                gear.m_EvolveItem.m_TimeSpentEvolvingGameHours = (float)minutesOnDry / 60;
                                MelonLogger.Msg(ConsoleColor.Blue, "Saving minutesOnDry " + minutesOnDry);
                                MelonLogger.Msg(ConsoleColor.Blue, "m_TimeSpentEvolvingGameHours " + gear.m_EvolveItem.m_TimeSpentEvolvingGameHours);
                            }
                        }
                    }

                    bool SkipPickup = false;
                    if(newGear.GetComponent<GearItem>().m_Bed != null)
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
                    
                    DroppedGearsObjs.Remove(SearchKey);
                    UnityEngine.Object.DestroyImmediate(obj);
                    ServerSend.PICKDROPPEDGEAR(0, SearchKey, true);
                }else{
                    MelonLogger.Msg("Gear with hash " + SearchKey + " is missing!");
                }
            }
        }

        public static void PickDroppedItem(int Hash, int Picker)
        {
            GameObject gearObj;
            DroppedGearsObjs.TryGetValue(Hash, out gearObj);
            if(gearObj != null)
            {
                DroppedGearsObjs.Remove(Hash);
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
                    SlicedPacket.m_GearName = levelid+level_guid;
                    SlicedPacket.m_SendTo = GearID;
                    SlicedPacket.m_Hash = SearchKey;
                    SlicedPacket.m_Str = jsonStringSlice;

                    if (BytesBuffer.Count != 0)
                    {
                        SlicedPacket.m_Last = false;
                    }else{
                        SlicedPacket.m_Last = true;
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
                    SlicedPacket.m_GearName = levelid+level_guid;
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
            }
        }

        public static void ClientTryPickupItem(int Hash, int sendTo, string lvlKey, bool place)
        {
            GameObject gearObj;
            DroppedGearsObjs.TryGetValue(Hash, out gearObj);
            if (gearObj != null)
            {
                DroppedGearsObjs.Remove(Hash);
                UnityEngine.Object.DestroyImmediate(gearObj);
            }
            ServerSend.PICKDROPPEDGEAR(sendTo, Hash, true);

            Dictionary<int, SlicedJsonDroppedGear> LevelDrops;
            if (DroppedGears.TryGetValue(lvlKey, out LevelDrops) == true)
            {
                MelonLogger.Msg("Found scene, looking for gear...");
                SlicedJsonDroppedGear DataProxy;
                if (LevelDrops.TryGetValue(Hash, out DataProxy) == true)
                {
                    MelonLogger.Msg("Found gear with hash "+ Hash);
                    LevelDrops.Remove(Hash);
                    SendDroppedItemToPicker(DataProxy.m_Json, sendTo, Hash, GetGearIDByName(DataProxy.m_GearName), place, DataProxy.m_Extra);
                }else{
                    MelonLogger.Msg("Client requested gear we have not data for, so gear most likely is missing. Gear hash "+ Hash);
                    ServerSend.GEARNOTEXIST(sendTo, true);
                }
                if (players[sendTo] != null && players[sendTo].GetComponent<MultiplayerPlayerAnimator>() != null)
                {
                    players[sendTo].GetComponent<MultiplayerPlayerAnimator>().Pickup();
                }
            }else{
                MelonLogger.Msg(ConsoleColor.Red, "Requested gear locates on unsaved scene! Scene key"+ lvlKey);
                ServerSend.GEARNOTEXIST(sendTo, true);
            }
        }

        public static void MarkSearchedContainers(string LevelKey, int SendTo = 0)
        {
            string seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + "";
            string dir = @"Mods\Unloads\" + seed + @"\" + LevelKey + @"\Containers" ;

            bool exists = System.IO.Directory.Exists(dir);

            if (!exists)
            {
                return;
            }else{
                string[] fileEntries = Directory.GetFiles(dir);
                foreach (string fileName in fileEntries)
                {
                    string step1 = fileName;
                    string[] sArray = fileName.Split(Convert.ToChar(@"\"));
                    string step2 = sArray[sArray.Length-1];
                    int L = step2.Length - 5;
                    string step3 = step2.Remove(L, 5);

                    MelonLogger.Msg("Container GUID should be searched "+ step3);

                    if(SendTo == 0)
                    {
                        GameObject box = ObjectGuidManager.Lookup(step3);
                        if (box != null)
                        {
                            GameObject reference = GetGearItemObject("GEAR_SoftWood");
                            GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(reference, box.transform.position, box.transform.rotation);
                            box.GetComponent<Container>().AddGear(newGear.GetComponent<GearItem>());
                        }
                    }else{
                        ServerSend.MARKSEARCHEDCONTAINERS(SendTo, step3);
                    }
                }
            }
        }

        //public static void SetBarFill(NPCStatusBar status, float fill)
        //{
        //    status.m_FillSprite.fillAmount = Mathf.Lerp(status.m_FillSpriteOffset, 1f - status.m_FillSpriteOffset, fill);
        //    if (status.m_ReverseFillSprite)
        //        status.m_ReverseFillSprite.fillAmount = Mathf.Lerp(status.m_FillSpriteOffset, 1f - status.m_FillSpriteOffset, status.GetReverseFillValue());
        //    if (status.m_BuffFillSprite)
        //        status.m_BuffFillSprite.fillAmount = status.GetBuffFillValue();
        //    if (status.m_BuffObject)
        //        Utils.SetActive(status.m_BuffObject, status.IsBuffActive());
        //    if (status.m_DebuffObject)
        //        Utils.SetActive(status.m_DebuffObject, status.IsDebuffActive());
        //    if (status.m_BuffFillObject)
        //        Utils.SetActive(status.m_BuffFillObject, status.ShouldShowBuffedFillSprite());
        //    if (status.m_NoBuffFillObject)
        //        Utils.SetActive(status.m_NoBuffFillObject, !status.ShouldShowBuffedFillSprite());
        //    status.SetSpriteColors(fill);
        //    status.UpdateBacksplash(fill);
        //}

        //public static void CheckOtherPlayer()
        //{
        //    float health = 10;
        //    //float thirst = 30;
        //    //float hunger = 30;
        //    //float fatigue = 30;
        //    //float freezy = 90;

        //    //float fillValueH = 1 - hunger / 100f;
        //    //float fillValueF = 1 - fatigue / 100f;

        //    GameObject DummyNpc = new GameObject();
        //    NPC m_NPC = DummyNpc.AddComponent<NPC>();
        //    NPCAfflictions m_NPCaff = DummyNpc.AddComponent<NPCAfflictions>();
        //    NPCCondition m_NPCcon = DummyNpc.AddComponent<NPCCondition>();
        //    CarryableBody m_BodyCarry = DummyNpc.AddComponent<CarryableBody>();
        //    NPCThirst m_Thirst = DummyNpc.AddComponent<NPCThirst>();
        //    NPCVoice m_Voice = DummyNpc.AddComponent<NPCVoice>();
        //    NPCFreezing m_Cold = DummyNpc.AddComponent<NPCFreezing>();
        //    m_NPC.m_AfflictionsComponent = m_NPCaff;
        //    m_NPC.m_Condition = m_NPCcon;
        //    m_NPC.m_Body = m_BodyCarry;
        //    m_NPC.m_Thirst = m_Thirst;
        //    m_NPC.m_Voice = m_Voice;
        //    m_NPC.m_Freezing = m_Cold;
        //    m_NPCaff.m_NPC = m_NPC;
        //    m_Thirst.m_NPC = m_NPC;
        //    m_Thirst.m_AlwaysHydrated = false;
        //    m_Cold.m_NPC = m_NPC;

        //    m_NPCaff.AddAffliction(AfflictionType.CabinFever, "unknown", AfflictionBodyArea.Head);
        //    m_NPCaff.AddAffliction(AfflictionType.BloodLoss, "unknown", AfflictionBodyArea.LegRight);
        //    m_NPCaff.AddAffliction(AfflictionType.InfectionRisk, "unknown", AfflictionBodyArea.ArmLeft);
        //    m_NPCaff.AddAffliction(AfflictionType.WellFed, "unknown", AfflictionBodyArea.Stomach);
        //    m_NPCaff.AddAffliction(AfflictionType.FrostbiteRisk, "unknown", AfflictionBodyArea.ArmRight);

        //    m_NPCcon.m_CurrentHP = health;
        //    m_NPCcon.m_MaxHP = 100;
        //    //m_Cold.m_CurrentFreezing = freezy;
        //    //m_Thirst.m_CurrentThirstPercentage = thirst;
        //    Panel_Diagnosis Panel = InterfaceManager.m_Panel_Diagnosis;

        //    GameObject thirstBar = Panel.gameObject.transform.GetChild(2).gameObject;
        //    GameObject hungerBar = UnityEngine.Object.Instantiate(thirstBar, thirstBar.transform.parent);
        //    GameObject fatigueBar = UnityEngine.Object.Instantiate(thirstBar, thirstBar.transform.parent);

        //    UnityEngine.Object.DestroyImmediate(hungerBar.transform.GetChild(0).GetComponent<GenericStatusBarSpawner>());
        //    UnityEngine.Object.DestroyImmediate(fatigueBar.transform.GetChild(0).GetComponent<GenericStatusBarSpawner>());

        //    //[x1] -1.1167f
        //    //[x2] -0.988f
        //    //[x3] -0.865f
        //    float y = thirstBar.transform.position.y;
        //    float z = thirstBar.transform.position.z;

        //    fatigueBar.transform.position = new Vector3(-1.1167f, y, z);
        //    thirstBar.transform.position = new Vector3(-0.988f, y, z);
        //    hungerBar.transform.position = new Vector3(-0.865f, y, z);

        //    //fatigueBar.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(6).gameObject.GetComponent<UISprite>().mSpriteName = "ico_status_fatigue1";
        //    //NPCStatusBar fatigueStatus = fatigueBar.transform.GetChild(0).GetChild(0).GetComponent<NPCStatusBar>();
        //    //NPCStatusBarOverrider fatigueOverrider = fatigueStatus.gameObject.AddComponent<NPCStatusBarOverrider>();
        //    //fatigueOverrider.m_Value = fillValueF;

        //    //hungerBar.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(6).gameObject.GetComponent<UISprite>().mSpriteName = "ico_status_hunger1";
        //    //NPCStatusBar hungerStatus = hungerBar.transform.GetChild(0).GetChild(0).GetComponent<NPCStatusBar>();
        //    //NPCStatusBarOverrider hungerOverrider = hungerStatus.gameObject.AddComponent<NPCStatusBarOverrider>();
        //    //hungerOverrider.m_Value = fillValueH;

        //    InterfaceManager.m_Panel_Diagnosis.Enable(true, m_NPC);
        //}

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

            for (int i = 0; i < Affs.Count; i++)
            {
                //m_NPCaff.AddAffliction((AfflictionType)Affs[i].m_Type, Affs[i].m_Case, (AfflictionBodyArea)Affs[i].m_Location);

                AfflictionDefinition definitionByType = GameManager.GetAfflictionDefinitionTable().GetAfflictionDefinitionByType((AfflictionType)Affs[i].m_Type);

                NPCAffliction npcAff = new NPCAffliction();
                if (Affs[i].m_ShouldBeTreated == false)
                {
                    npcAff.m_RestHours = 1;
                }
                npcAff.m_Definition = definitionByType;
                npcAff.m_Location = (AfflictionBodyArea)Affs[i].m_Location;
                npcAff.m_CauseLocId = Affs[i].m_Case;

                m_NPCaff.m_Afflictions.Add(npcAff);
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
        }

        public class DictionaryElementToReNew
        {
            public int m_Key;
            public SlicedJsonDroppedGear m_Val;
        }

        public static bool LoadDropsForScene(string LevelKey)
        {
            //MelonLogger.Msg("[DroppedGearsUnloader] Going to load drops for " + LevelKey);
            string seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + "";
            string dir = @"Mods\Unloads\" + seed + @"\" + LevelKey + @"\" + "drops.json";

            MelonLogger.Msg(ConsoleColor.Blue, "[DroppedGearsUnloader] Going to load drops by path: "+ dir);

            bool exists = System.IO.File.Exists(dir);
            string data = "";

            if (!exists)
            {
                MelonLogger.Msg(ConsoleColor.Yellow, "[DroppedGearsUnloader] Can't find drops by path: " + dir);
                return false;
            }else{
                try
                {
                    using (var sr = new StreamReader(dir))
                    {
                        data = sr.ReadToEnd();
                    }
                }
                catch (IOException e)
                {
                    MelonLogger.Msg(ConsoleColor.Red, "[DroppedGearsUnloader] The file could not be read:" + e.Message);
                    return false;
                }

                //MelonLogger.Msg(ConsoleColor.Blue, "[DroppedGearsUnloader] Loading done file conent is: ");
                //MelonLogger.Msg(ConsoleColor.Blue, data);
                Dictionary<int, SlicedJsonDroppedGear> LoadedData = JSON.Load(data).Make<Dictionary<int, SlicedJsonDroppedGear>>();


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
                        int minutesLeft = NeedToBePlaced-minutesPlaced + 1;
                        float ChanceToCatch = 50;
                        float ChanceToBreak = 15;
                        //MelonLogger.Msg(ConsoleColor.Yellow, "Found placed snare checking for update...");
                        //MelonLogger.Msg(ConsoleColor.Blue, "Time left to roll chance "+ minutesLeft);
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

                                RabbitsBuff.Add(DummyGear.m_Position);
                                //MelonLogger.Msg(ConsoleColor.Green, "We got rabbit! ");
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

                                    //MelonLogger.Msg(ConsoleColor.Green, "Snare broken!");
                                }else{
                                    // No new visual state, but reseting time player should to wait for.
                                    dat.m_Extra.m_DroppedTime = MinutesFromStartServer;
                                    dat.m_Extra.m_GoalTime = MinutesFromStartServer + NeedToBePlaced;

                                    //MelonLogger.Msg(ConsoleColor.Green, "Nothing catched, reseting timer! Settings time to "+ dat.m_Extra.m_GoalTime+" current time is "+ MinutesFromStartServer);
                                }
                            }
                            DictionaryElementToReNew newGear = new DictionaryElementToReNew();
                            newGear.m_Key = curKey;
                            newGear.m_Val = dat;
                            Buff.Add(newGear); //Adding to buffer gears that has been modified
                        }
                    }
                }

                if(Buff.Count > 0) //If buffer contains anything, we need to remove old gears and update them with modified ones
                {
                    for (int i = 0; i < Buff.Count; i++)
                    {
                        LoadedData.Remove(Buff[i].m_Key);
                        LoadedData.Add(Buff[i].m_Key, Buff[i].m_Val);
                    }
                }
                if (RabbitsBuff.Count > 0) //If buffer contains anything, we need to spawn rabbits
                {
                    for (int i = 0; i < RabbitsBuff.Count; i++)
                    {
                        //MelonLogger.Msg(ConsoleColor.Green, "Should spawn rabbit on x "+ RabbitsBuff[i].x+" y "+ RabbitsBuff[i].y+" z "+ RabbitsBuff[i].z);
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
                            int hashLevelKey = LevelKey.GetHashCode();
                            SearchKey = hashGearID + hashV3 + hashRot + hashLevelKey;
                            UnityEngine.Object.DestroyImmediate(obj);
                        }
                        
                        Rabbit.m_Json = RabbitJson;
                        LoadedData.Add(SearchKey, Rabbit);
                    }
                }

                DroppedGears.Add(LevelKey, LoadedData);
                MelonLogger.Msg(ConsoleColor.Green, "[DroppedGearsUnloader] File is found and loaded!");
                return true;
            }
        }

        public static void SaveAllLoadedDrops()
        {
            if (iAmHost == false && InOnline() == true)
            {
                return;
            }

            foreach (var cur in DroppedGears)
            {
                string currentKey = cur.Key;
                string data = JSON.Dump(cur.Value);

                string seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + "";
                string dir = @"Mods\Unloads\" + seed + @"\" + currentKey + @"\" + "drops.json";

                CreateFolderIfNotExist(@"Mods\Unloads");
                CreateFolderIfNotExist(@"Mods\Unloads\" + seed);
                CreateFolderIfNotExist(@"Mods\Unloads\" + seed + @"\" + currentKey);

                using (FileStream fs = File.Create(dir))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(data);
                    fs.Write(info, 0, info.Length);
                }
            }
        }

        public static void SaveAllLoadedOpenables()
        {
            if(iAmHost == false && InOnline() == true)
            {
                return;
            }
            
            
            foreach (var cur in OpenableThings)
            {
                string currentKey = cur.Key;
                string data = JSON.Dump(cur.Value);

                string seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + "";
                string dir = @"Mods\Unloads\" + seed + @"\" + currentKey + @"\" + "openablethings.json";

                CreateFolderIfNotExist(@"Mods\Unloads");
                CreateFolderIfNotExist(@"Mods\Unloads\" + seed);
                CreateFolderIfNotExist(@"Mods\Unloads\" + seed + @"\" + currentKey);

                using (FileStream fs = File.Create(dir))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(data);
                    fs.Write(info, 0, info.Length);
                }
            }
        }
        public static bool FsBusy = false;

        public static void UnloadDropsForScene(string LevelKey)
        {
            if(FsBusy == true)
            {
                MelonLogger.Msg("[DroppedGearsUnloader] Flie system busy can't unload right now... ");
                return;
            }
            string data = "";
            MelonLogger.Msg("[DroppedGearsUnloader] Going to unload " + LevelKey);
            Dictionary<int, SlicedJsonDroppedGear> LevelDrops;
            if (DroppedGears.TryGetValue(LevelKey, out LevelDrops) == true)
            {
                data = JSON.Dump(LevelDrops);
            }else{
                MelonLogger.Msg(ConsoleColor.Red, "[DroppedGearsUnloader] Can't get dictionary with key " + LevelKey);
                return;
            }

            DroppedGears.Remove(LevelKey);

            string seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + "";
            string dir = @"Mods\Unloads\" + seed + @"\" + LevelKey + @"\" + "drops.json";

            CreateFolderIfNotExist(@"Mods\Unloads");
            CreateFolderIfNotExist(@"Mods\Unloads\" + seed);
            CreateFolderIfNotExist(@"Mods\Unloads\" + seed + @"\" + LevelKey);
            FsBusy = true;
            using (FileStream fs = File.Create(dir))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(data);
                fs.Write(info, 0, info.Length);
            }
            FsBusy = false;
            MelonLogger.Msg("[DroppedGearsUnloader] Unloading finished!");
        }

        public static void UnloadOpenableThingsForScene(string LevelKey)
        {
            if (FsBusy == true)
            {
                MelonLogger.Msg("[OpenableThingsUnloader] Flie system busy can't unload right now... ");
                return;
            }
            string data = "";

            //MelonLogger.Msg("[DroppedGearsUnloader] Going to unload " + LevelKey);
            Dictionary<string, bool> LevelOpenables;
            if (OpenableThings.TryGetValue(LevelKey, out LevelOpenables) == true)
            {
                data = JSON.Dump(LevelOpenables);
            }else{
                return;
            }

            OpenableThings.Remove(LevelKey);

            string seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + "";
            string dir = @"Mods\Unloads\" + seed + @"\" + LevelKey + @"\openablethings.json";

            CreateFolderIfNotExist(@"Mods\Unloads");
            CreateFolderIfNotExist(@"Mods\Unloads\" + seed);
            CreateFolderIfNotExist(@"Mods\Unloads\" + seed + @"\" + LevelKey);
            FsBusy = true;
            using (FileStream fs = File.Create(dir))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(data);
                fs.Write(info, 0, info.Length);
            }
            FsBusy = false;
        }

        public static void ManageDropsLoads()
        {
            bool Dedicated = Application.isBatchMode;

            foreach (var cur in DroppedGears)
            {
                string currentKey = cur.Key;
                bool found = false;
                if(Dedicated == false) //If not dedicated server, then host can load drops too.
                {
                    string MyKey = levelid+level_guid;

                    if(currentKey == MyKey)
                    {
                        found = true;
                    }
                }

                if(found == false) //If host not on that scene, checking every player.
                {
                    for (int i = 0; i < playersData.Count; i++)
                    {
                        if (playersData[i] != null)
                        {
                            string LevelKey = playersData[i].m_Levelid + playersData[i].m_LevelGuid;
                            if (LevelKey == currentKey)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }

                if (found == false) //If no one on this scene unloading this.
                {
                    UnloadDropsForScene(currentKey);
                    return;
                }
            }
            foreach (var cur in OpenableThings)
            {
                string currentKey = cur.Key;
                bool found = false;
                if (Dedicated == false) //If not dedicated server, then host can load drops too.
                {
                    string MyKey = levelid + level_guid;

                    if (currentKey == MyKey)
                    {
                        found = true;
                    }
                }

                if (found == false) //If host not on that scene, checking every player.
                {
                    for (int i = 0; i < playersData.Count; i++)
                    {
                        if (playersData[i] != null)
                        {
                            string LevelKey = playersData[i].m_Levelid + playersData[i].m_LevelGuid;
                            if (LevelKey == currentKey)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }

                if (found == false) //If no one on this scene unloading this.
                {
                    UnloadOpenableThingsForScene(currentKey);
                    return;
                }
            }
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
                int hashLvl = levelid.GetHashCode();
                int hashLvlGUID = level_guid.GetHashCode();

                int SearchKey = hashGearID + hashV3 + hashRot + hashLvl + hashLvlGUID;
                string LevelKey = levelid+level_guid;

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
                    }
                }
                if(iAmHost == true)
                {
                    AddDroppedGear(GearID, SearchKey, DataProxy, LevelKey, Extra);
                    ServerSend.DROPITEM(0, SyncData, true);
                    FakeDropItem(GearID, v3, rot, SearchKey, Extra);
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
            bool exists = System.IO.Directory.Exists(path);
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }

        public static void SaveFakeContainer(string GUID, string LevelKey, string data, bool needCompress = true)
        {
            string seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + "";
            string dir = @"Mods\Unloads\" + seed + @"\" + LevelKey + @"\Containers\" + GUID + ".json";

            CreateFolderIfNotExist(@"Mods\Unloads");
            CreateFolderIfNotExist(@"Mods\Unloads\" + seed);
            CreateFolderIfNotExist(@"Mods\Unloads\" + seed + @"\" + LevelKey);
            CreateFolderIfNotExist(@"Mods\Unloads\" + seed + @"\" + LevelKey + @"\Containers");

            using (FileStream fs = File.Create(dir))
            {
                string compressed = "";
                if (needCompress == true)
                {
                    compressed = CompressString(data);
                }else{
                    compressed = data;
                }
                 
                byte[] info = new UTF8Encoding(true).GetBytes(compressed);
                fs.Write(info, 0, info.Length);
            }
        }
        public static string LoadFakeContainer(string GUID, string LevelKey)
        {
            MelonLogger.Msg("[DroppedGearsUnloader] Going to load container " + GUID);
            string seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + "";
            string dir = @"Mods\Unloads\" + seed + @"\" + LevelKey + @"\Containers\" + GUID + ".json";

            bool exists = System.IO.File.Exists(dir);

            if (!exists)
            {
                MelonLogger.Msg("[DroppedGearsUnloader] Saves not found");
                return "";
            }else{
                try
                {
                    using (var sr = new StreamReader(dir))
                    {
                        MelonLogger.Msg("[DroppedGearsUnloader] Saves found");
                        return sr.ReadToEnd();
                    }
                }
                catch (IOException e)
                {
                    MelonLogger.Msg(ConsoleColor.Red, "[DroppedGearsUnloader] The file could not be read:" + e.Message);
                    return "";
                }
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
            Dictionary<string, bool> LevelOpenables;
            string LevelKey = levelid + level_guid;
            if (OpenableThings.TryGetValue(LevelKey, out LevelOpenables) == true)
            {
                foreach (var cur in LevelOpenables)
                {
                    GameObject Openable;
                    if(OpenablesObjs.TryGetValue(cur.Key, out Openable) == true)
                    {
                        if(Openable != null)
                        {
                            OpenClose OpCl = Openable.GetComponent<OpenClose>();
                            if (OpCl.IsOpen() != cur.Value)
                            {
                                if(cur.Value == false)
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
        }

        public static void SendOpenableThing(string LevelKey, string GUID, bool state)
        {
            using (Packet _packet = new Packet((int)ClientPackets.USEOPENABLE))
            {
                _packet.Write(LevelKey);
                _packet.Write(GUID);
                _packet.Write(state);

                SendTCPData(_packet);
            }
        }

        public static void ChangeOpenableThingState(string LevelKey, string GUID, bool state)
        {
            if(InOnline() == false)
            {
                return;
            }
            
            Dictionary<string, bool> LevelOpenables;
            if (OpenableThings.TryGetValue(LevelKey, out LevelOpenables) == false)
            {
                OpenableThings.Add(LevelKey, new Dictionary<string, bool>());
                if (OpenableThings.TryGetValue(LevelKey, out LevelOpenables) == false)
                {
                    MelonLogger.Msg(ConsoleColor.Red, "Can't create dictionary for openables by some reason...");
                }
            }

            if(LevelOpenables.ContainsKey(GUID) == true)
            {
                LevelOpenables.Remove(GUID);
            }
            LevelOpenables.Add(GUID, state);
            MelonLogger.Msg(ConsoleColor.Blue, "Openable things "+ GUID + " changed state to OpenIs="+ state);

            if(iAmHost == true)
            {
                ServerSend.USEOPENABLE(0, GUID, state, true);
            }
        }

        public static bool LoadOpenables(string LevelKey)
        {
            string seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + "";
            string dir = @"Mods\Unloads\" + seed + @"\" + LevelKey + @"\openablethings.json";

            bool exists = System.IO.File.Exists(dir);

            if (!exists)
            {
                MelonLogger.Msg("[OpenableThingsUnloader] Saves not found for "+ LevelKey);
                return false;
            }else{
                try
                {
                    using (var sr = new StreamReader(dir))
                    {
                        MelonLogger.Msg("[OpenableThingsUnloader] Saves found "+ LevelKey);
                        string data = sr.ReadToEnd();
                        Dictionary<string, bool> LoadedData = JSON.Load(data).Make<Dictionary<string, bool>>();
                        OpenableThings.Add(LevelKey, LoadedData);
                        return true;
                    }
                }
                catch (IOException e)
                {
                    MelonLogger.Msg(ConsoleColor.Red, "[OpenableThingsUnloader] The file could not be read:" + e.Message);
                    return false;
                }
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
                CarefulSlicesSent = CarefulSlicesSent + 1;
            }else{
                MelonLogger.Msg("Finished sending all "+ CarefulSlicesSent+" slices");
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

            //if((iAmHost == true && Server.UsingSteamWorks == true) || (sendMyPosition == true && ConnectedSteamWorks == true))
            //{
            //    CHUNK_SIZE = 1000;
            //}else{
            //    CHUNK_SIZE = 500;
            //}

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

            if (box.m_RestoreControlMode != null)
            {
                GameManager.GetPlayerManagerComponent().SetControlMode(box.m_RestoreControlMode);
            }
            GameManager.GetPlayerManagerComponent().m_ContainerBeingSearched = null;
            InterfaceManager.m_Panel_HUD.CancelItemProgressBar();
            box.m_SearchInProgress = false;
            box.m_OpenInProgress = false;
            box.m_Inspected = true;
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
            GoingToOpenContinaer = null;
        }

        public static Container GoingToOpenContinaer = null;

        public static void OpenFakeContainer(Container box)
        {
            string levelKey = levelid + level_guid;
            string boxGUID = box.GetComponent<ObjectGuid>().Get();
            if (sendMyPosition == true)
            {
                GoingToOpenContinaer = box;
                DoPleaseWait("Please wait...", "Downloading container data...");
                using (Packet _packet = new Packet((int)ClientPackets.REQUESTOPENCONTAINER))
                {
                    _packet.Write(levelKey);
                    _packet.Write(boxGUID);
                    SetRepeatPacket(_packet);
                    SendTCPData(_packet);
                }
                return;
            }

            string CompressedData = LoadFakeContainer(boxGUID, levelKey);
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

            if (Data == "")
            {
                box.DestroyAllGear();
            }else{
                Il2CppSystem.Collections.Generic.List<GearItem> loadedlist = new Il2CppSystem.Collections.Generic.List<GearItem>();
                box.Deserialize(Data, loadedlist);
            }
            //box.m_CapacityKG = 1000f;
            InterfaceManager.m_Panel_Container.SetContainer(box, box.m_LocalizedDisplayName.Text());
            InterfaceManager.m_Panel_Container.Enable(true);
        }

        public static void CloseFakeContainer(Container box)
        {
            string Data = box.Serialize();
            string CompressedData = CompressString(Data);
            string levelKey = levelid + level_guid;
            string boxGUID = box.GetComponent<ObjectGuid>().Get();
            box.DestroyAllGear();
            GameObject reference = GetGearItemObject("GEAR_SoftWood");
            GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(reference, box.transform.position, box.transform.rotation);
            box.GetComponent<Container>().AddGear(newGear.GetComponent<GearItem>());
            if (sendMyPosition == true)
            {
                DoPleaseWait("Please wait...", "Sending container data...");
                SendContainerData(CompressedData, levelKey, boxGUID);
                return;
            }
            if (box != null)
            {
                if (!box.Close())
                    return;
                if (box.m_CloseAudio.Length == 0)
                    GameAudioManager.PlayGUIButtonBack();
            }
            GameManager.GetPlayerManagerComponent().MaybeRevealPolaroidDiscoveryOnClose();
            InterfaceManager.m_Panel_Container.Enable(false);
            SaveFakeContainer(boxGUID, levelKey, CompressedData, false);
        }

        public static GameObject PingElement = null;


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
            //if (GameManager.m_vpFPSCamera != null)
            //{
            //    if (GameManager.GetVpFPSCamera().gameObject.GetComponent<AudioListener>() == null)
            //    {
            //        GameManager.GetVpFPSCamera().gameObject.AddComponent<AudioListener>();
            //    }
            //}
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
            }
            else
            {
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
                    MicrophoneIdicator.SetActive(false);
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
                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Tab))
                {
                    if (StatusObject.activeSelf == false)
                    {
                        StatusObject.SetActive(true);
                    }else{
                        StatusObject.SetActive(false);
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
            }
            if (UIHostMenu != null)
            {
                GameObject IsSteamHost = UIHostMenu.transform.GetChild(4).gameObject;
                //GameObject PublicSteamServer = UIHostMenu.transform.GetChild(5).gameObject;
                GameObject PortsObject = UIHostMenu.transform.GetChild(8).gameObject;
                if (IsSteamHost.GetComponent<UnityEngine.UI.Toggle>().isOn == true)
                {
                    //if(PublicSteamServer.activeSelf == false)
                    //{
                    //    PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().Set(false);
                    //}
                    //PublicSteamServer.SetActive(true);
                    PortsObject.SetActive(false);
                }else{
                    //PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().Set(false);
                    //PublicSteamServer.SetActive(false);
                    PortsObject.SetActive(true);
                }
            }
        }

        public static void DebugCrap()
        {
            int layerMask = 1 << 16;
            RaycastHit hit;
            Transform transform = GameManager.GetMainCamera().transform;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 50, layerMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

                if (hit.collider.gameObject.GetComponent<ObjectGuid>())
                {
                    DebugAnimalGUID = hit.collider.gameObject.GetComponent<ObjectGuid>().Get();
                    DebugAnimalGUIDLast = DebugAnimalGUID;
                    DebugLastAnimal = hit.collider.gameObject;
                }else{
                    DebugAnimalGUID = "Have not objectGuid";
                    DebugAnimalGUIDLast = DebugAnimalGUID;
                    DebugLastAnimal = hit.collider.gameObject;
                }
            }else{
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
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
                    GearItemDataPacket GearDataPak = new GearItemDataPacket();
                    GearDataPak.m_GearName = _gear.m_GearName;
                    GearDataPak.m_SendedTo = GiveItemTo;
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
                        }
                    }
                    if (waterMode == true)
                    {
                        string say = "half liter of " + _gear.m_LocalizedDisplayName.Text();

                        if (bottle.m_VolumeInLiters == waterGave)
                        {
                            bottle.m_VolumeInLiters = 0;
                        }else{
                            bottle.m_VolumeInLiters = bottle.m_VolumeInLiters - waterGave;
                            say = waterGave + " liter of " + _gear.m_LocalizedDisplayName.Text();
                        }
                        HUDMessage.AddMessage("You gave " + say + " to " + playersData[GiveItemTo].m_Name);
                    }else{
                        HUDMessage.AddMessage("You gave " + _gear.m_LocalizedDisplayName.Text() + " to " + playersData[GiveItemTo].m_Name);
                        GameManager.GetInventoryComponent().RemoveUnits(_gear, 1);
                    }
                    MelonLogger.Msg("You gave " + LastSelectedGearName + " to " + playersData[GiveItemTo].m_Name);
                    InterfaceManager.m_Panel_Inventory.m_IsDirty = true;
                    InterfaceManager.m_Panel_Inventory.Update();
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
            if (IsDead == false && KillAfterLoad == false && InFight == false)
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

            if (sendMyPosition == true) // CLIENT
            {
                using (Packet _packet = new Packet((int)ClientPackets.XYZ))
                {
                    _packet.Write(target.position);
                    SendTCPData(_packet);
                }
            }

            if(iAmHost == true)
            {
                ServerSend.XYZ(0, GameManager.GetPlayerTransform().position, true);
            }
        }

        public static void SyncRotation(Transform target)
        {
            if (sendMyPosition == true) // CLIENT
            {
                using (Packet _packet = new Packet((int)ClientPackets.XYZW))
                {
                    _packet.Write(target.rotation);
                    SendTCPData(_packet);
                }
            }
            if (iAmHost == true) // HOST
            {
                using (Packet _packet = new Packet((int)ServerPackets.XYZW))
                {
                    ServerSend.XYZW(0, target.rotation, true);
                }
            }
        }

        public static void SetCurrentAnimation(bool InFight)
        {
            if (IsDead == true || KillAfterLoad == true || InFight == true)
            {
                if (IsDead == true || KillAfterLoad == true)
                {
                    MyAnimState = "Knock";
                }
                else
                {
                    MyAnimState = "Fight";
                }
            }
            else
            {
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
                                    }//InterfaceManager.m_Panel_BodyHarvest != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject != null
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
                            }
                            else
                            {
                                playersData[i].m_TicksOnScene = playersData[i].m_TicksOnScene + 1;
                            }
                        }
                        else
                        {
                            playersData[i].m_TicksOnScene = 0;
                        }
                    }
                }
            }
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
                if (GameManager.GetPlayerManagerComponent().PlayerIsSleeping() == true || (WaitForSleepLable != null && WaitForSleepLable.activeSelf == true))
                {
                    MelonLogger.Msg("Going to sleep");
                    MelonLogger.Msg("Skiping Cycle time " + MyCycleSkip);
                    GameObject bed = LastObjectUnderCrosshair;
                    if (bed != null && bed.GetComponent<Bed>() != null)
                    {
                        if (bed.GetComponent<Bed>().m_BodyPlacementTransform == true)
                        {
                            Vector3 SleepV3 = bed.GetComponent<Bed>().m_BodyPlacementTransform.position;
                            Quaternion SleepQuat = bed.GetComponent<Bed>().m_BodyPlacementTransform.rotation;

                            if (iAmHost == true)
                            {
                                ServerSend.SLEEPPOSE(0, SleepV3, SleepQuat, true);
                            }
                            if (sendMyPosition == true)
                            {
                                using (Packet _packet = new Packet((int)ClientPackets.SLEEPPOSE))
                                {
                                    _packet.Write(SleepV3);
                                    _packet.Write(SleepQuat);
                                    SendTCPData(_packet);
                                }
                            }
                        }
                        else
                        {
                            MelonLogger.Msg(ConsoleColor.DarkRed, "Bed has not sleep pose transform! AAAAAAAAAAAAAA");
                        }
                    }
                }else{
                    MyCycleSkip = 0;
                    if (iAmHost == true)
                    {
                        ServerSend.SLEEPPOSE(0, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), true);
                    }
                    if (sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.SLEEPHOURS))
                        {
                            _packet.Write(new Vector3(0, 0, 0));
                            _packet.Write(new Quaternion(0, 0, 0, 0));
                            SendTCPData(_packet);
                        }
                    }
                    MelonLogger.Msg("Has wakeup or cancle sleep");
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
                        ServerSend.HARVESTINGANIMAL(1, HarvestingAnimal);
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
                if (playersData[i] != null)
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
                        if (otherHarvestPlant == harvestGUID)
                        {
                            HUDMessage.AddMessage(playersData[i].m_Name + " IS ALREADY COLLECTING THIS");
                            harvestGUID = null;
                            h.CancelHarvest();
                        }
                        string otherAnimlGuid = MyMod.playersData[i].m_HarvestingAnimal;
                        if (otherAnimlGuid == harvestAnimalGUID)
                        {
                            harvestAnimalGUID = null;
                            ExitHarvesting();
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

        

        public override void OnUpdate()
        {
            if(KillOnUpdate == true)
            {
                DebugCrap();
                return;
            }
            
            if (Application.runInBackground == false) { Application.runInBackground = true; } // Always running in bg, to not lost any sync packets
            FakeDeath(); // Limiting movement and ignore by animals
            UpdateMain(); // Updating sync tread
            GameLogic.Update(); // Updating sync tread for server
            UpdateAPIStates(); // Updating ID and Client/Host state for API
            InitAudio(); // Adding audio listener if needed
            KillConsole(); // Unregistering cheats if server not allow cheating for you
            UpdateSceneGUID(); // Scene GUID that used for detection of dublicated interiours 

            if (GearIDList.Count == 0 && level_name == "MainMenu") // Doing it only once
            {
                InitGearsIDS(); // Compilating List of Gear names by ID, to reference on it to shorten names in sync packets
            }
            if (Application.isBatchMode) // If Dedicated mode, doing specific code for it
            {
                DedicatedServerUpdate(); // Updating dedicated settings
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
                EverySecond(); // Triggering this code very second
            }

            if (m_InterfaceManager != null)
            {
                if(InterfaceManager.m_Panel_Confirmation != null && NeedConnectAfterLoad != -1)
                {
                    DoWaitForConnect(); // Showing connection message after startup connection
                }
                if(InterfaceManager.m_Panel_SnowShelterInteract != null)
                {
                    CancleDismantling(); // Cancle Break-down of snowshelter where any other player is 
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
                    if (InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject.GetComponent<BaseAi>() != null)
                    {
                        HarvestingAnimal = InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>().Get();
                    }else{
                        HarvestingAnimal = "";
                    }
                }else{
                    HarvestingAnimal = "";
                }
                SendHarvestingAnimal(); // Send harvesting animal if it changed 
            }
            if (SteamServerWorks != "" || Server.UsingSteamWorks == true)
            {
                TrackWhenRecordOver(); // Processing voice chat
            }

            if (FirstBoot == false && SteamConnect.CanUseSteam == true)
            {
                SteamConnect.DoUpdate(); // Start tracking of incomming data from other players
            }

            InitAllPlayers(); // Prepare players objects based on amount of max players

            if(iAmHost == true)
            {
                if(IsCycleSkiping == true && GameManager.m_PlayerManager != null)
                {
                    if (GameManager.GetPlayerManagerComponent().PlayerIsSleeping() == false || IsDead == true)
                    {
                        IsCycleSkiping = false;
                    }
                }
                UpdateTicksOnScenes(); // Calculate how long player on this scene
                if (NeedSyncTime == true)
                {
                    if (Time.time > nextActionTime)
                    {
                        nextActionTime += period;
                        EveryInGameMinute(); // Trigger actions that happends every in game minute (5 seconds) and recalculate realtime cycle and resend weather.
                    }
                }
            }
            if (InOnline() == true)
            {
                //DoColisionForArrows();
                if (GameManager.m_Condition != null && GameManager.m_PlayerManager != null && GameManager.GetPlayerManagerComponent().PlayerIsDead())
                {
                    GameManager.m_Condition.DisableLowHealthEffects();
                }
                if (DoFakeGetup == true)
                {
                    BashGetupDelayCamera(); // I not even remember, but this needed.
                }
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
            }

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.G))
            {
                FindPlayerToTrade(); // Find closest player to give item to.
                ProcessGivingItem(); // Attempt to give item to nearest player.
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
            DoWaitForConnect();
        }


        public static bool AtHostMenu = false;

        public static void LoadAllDropsForScene()
        {
            string lvlKey = MyMod.levelid+MyMod.level_guid;
            MelonLogger.Msg(ConsoleColor.Blue, "Trying to load drops for scene " + lvlKey);
            if (MyMod.DroppedGears.ContainsKey(lvlKey) == false)
            {
                MelonLogger.Msg(ConsoleColor.Blue, "");
                bool FoundSaves = LoadDropsForScene(lvlKey);
                if(FoundSaves == false)
                {
                    MelonLogger.Msg(ConsoleColor.Yellow, "Drops drops for scene " + lvlKey + " not found");
                    return;
                }
            }else{
                MelonLogger.Msg(ConsoleColor.Yellow, "Drops for scene " + lvlKey+" already loaded");
            }

            Dictionary<int, MyMod.SlicedJsonDroppedGear> LevelDrops;

            if (MyMod.DroppedGears.TryGetValue(lvlKey, out LevelDrops) == true)
            {
                int index = 0;
                foreach (var cur in LevelDrops)
                {
                    index++;
                    int currentKey = cur.Key;
                    MyMod.SlicedJsonDroppedGear currentValue = cur.Value;

                    if(DroppedGearsObjs.ContainsKey(cur.Key) == false)
                    {
                        GearItemSaveDataProxy DummyGear = Utils.DeserializeObject<GearItemSaveDataProxy>(currentValue.m_Json);
                        MyMod.FakeDropItem(GetGearIDByName(currentValue.m_GearName), DummyGear.m_Position, DummyGear.m_Rotation, cur.Key, currentValue.m_Extra);
                    }else{
                        MelonLogger.Msg(ConsoleColor.Yellow, "Gear object "+ cur.Key + " already loaded not need to load this");
                    }
                }
            }
        }

        public static void LoadAllOpenableThingsForScene()
        {
            string lvlKey = MyMod.levelid+MyMod.level_guid;

            if (OpenableThings.ContainsKey(lvlKey) == false)
            {
                bool Found = LoadOpenables(lvlKey);
                if (Found == false)
                {
                    MelonLogger.Msg("[OpenableThingsUnloader] No openables saves found for " + lvlKey);
                    return;
                }
            }
            else
            {
                MelonLogger.Msg("[OpenableThingsUnloader] Dictionary for " + lvlKey+" already loaded");
            }
        }

        public static void SetFixedSpawn()
        {
            if(FixedPlaceLoaded == false)
            {
                SavedSceneForSpawn = level_name;
                SavedPositionForSpawn = GameManager.GetPlayerTransform().position;
            }
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
                MelonLogger.Msg("Server has been runned with InGame time: " + GameManager.GetTimeOfDayComponent().GetHour() + ":" + GameManager.GetTimeOfDayComponent().GetMinutes() + " seed "+ GameManager.m_SceneTransitionData.m_GameRandomSeed);
                OverridedHourse = GameManager.GetTimeOfDayComponent().GetHour();
                OverridedMinutes = GameManager.GetTimeOfDayComponent().GetMinutes();
                OveridedTime = OverridedHourse + ":" + OverridedMinutes;
                NeedSyncTime = true;
                RealTimeCycleSpeed = true;
                //LoadAllDropsForScene();
                //LoadAllOpenableThingsForScene();
                //MarkSearchedContainers(levelid+level_guid);
                DisableOriginalAnimalSpawns(true);
                SetFixedSpawn();
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
                        SteamConnect.Main.ConnectToHost(SteamServerWorks);
                    }
                }
               
                RealTimeCycleSpeed = true;
            }
            else
            {
                HUDMessage.AddMessage("YOU ALREADY CONNECTED!!!!");
            }
        } 

        public void PreReleaseGUI()
        {
            //if (IamShatalker == true)
            //{
            //    if (anotherbutt != null)
            //    {
            //        if (WardIsActive == true)
            //        {
            //            //MelonLogger.Msg("Ward is active!");
            //            GUI.Label(new Rect(700, 30, 500, 100), "Ward is active!");
            //        }
            //    }
            //}

            if (level_name != "Empty" && level_name != "Boot" && level_name != "MainMenu" && m_InterfaceManager != null && InterfaceManager.m_Panel_PauseMenu != null && InterfaceManager.m_Panel_PauseMenu.IsEnabled() == true)
            {
                GUI.Box(new Rect(10, 10, 100, 200), "Multiplayer");

                if (InOnline() == false)
                {
                    if (AtHostMenu == false)
                    {
                        if (GUI.Button(new Rect(20, 40, 80, 20), "Host"))
                        {
                            if (iAmHost != true)
                            {
                                AtHostMenu = true;
                            }
                            else
                            {
                                HUDMessage.AddMessage("YOU ALREADY HOSING!!!!!!");
                            }
                        }
                        if (GUI.Button(new Rect(20, 70, 80, 20), "Connect"))
                        {
                            if (sendMyPosition != true)
                            {
                                InDarkWalkerMode = true;
                                RealTimeCycleSpeed = false;
                                InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input server address", "127.0.0.1", Panel_Confirmation.ButtonLayout.Button_2, "Connect", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                            }
                            else
                            {
                                HUDMessage.AddMessage("YOU ALREADY CONNECTED!!!!");
                            }
                        }
                    }
                    else
                    {
                        GUI.Box(new Rect(10, 10, 100, 200), "Host as");
                        if (GUI.Button(new Rect(20, 40, 80, 20), "Darkwalker"))
                        {
                            IamShatalker = true;
                            uConsole.RunCommandSilent("Ghost");
                            uConsole.RunCommandSilent("God");
                            ///Host
                            isRuning = true;

                            Thread mainThread = new Thread(new ThreadStart(MainThread));
                            ServerHandle.IamShatalker = true;

                            Server.Start(1, 26950);
                            nextActionTime = Time.time;
                            nextActionTimeAniamls = Time.time;
                            iAmHost = true;
                            MelonLogger.Msg("Server has been runned with InGame time: " + GameManager.GetTimeOfDayComponent().GetHour() + ":" + GameManager.GetTimeOfDayComponent().GetMinutes());
                            OverridedHourse = GameManager.GetTimeOfDayComponent().GetHour();
                            OverridedMinutes = GameManager.GetTimeOfDayComponent().GetMinutes();
                            OveridedTime = OverridedHourse + ":" + OverridedMinutes;
                            RealTimeCycleSpeed = false;
                            NeedSyncTime = false;
                            InDarkWalkerMode = true;
                            AtHostMenu = false;
                        }
                        if (GUI.Button(new Rect(20, 70, 80, 20), "Survivor"))
                        {
                            ///Host
                            isRuning = true;

                            Thread mainThread = new Thread(new ThreadStart(MainThread));
                            ServerHandle.IamShatalker = false;

                            Server.Start(1, 26950);
                            nextActionTime = Time.time;
                            nextActionTimeAniamls = Time.time;
                            iAmHost = true;
                            MelonLogger.Msg("Server has been runned with InGame time: " + GameManager.GetTimeOfDayComponent().GetHour() + ":" + GameManager.GetTimeOfDayComponent().GetMinutes());
                            OverridedHourse = GameManager.GetTimeOfDayComponent().GetHour();
                            OverridedMinutes = GameManager.GetTimeOfDayComponent().GetMinutes();
                            OveridedTime = OverridedHourse + ":" + OverridedMinutes;
                            RealTimeCycleSpeed = false;
                            NeedSyncTime = false;
                            InDarkWalkerMode = true;
                            AtHostMenu = false;
                        }
                        if (GUI.Button(new Rect(20, 120, 80, 20), "Back"))
                        {
                            AtHostMenu = false;
                        }
                    }
                } else
                {
                    if (GUI.Button(new Rect(20, 40, 80, 20), "Disconnect"))
                    {
                        MelonLogger.Msg("Disconnect case pressed disconnect button");
                        LastConnectedIp = "";
                        Disconnect();
                    }
                    if (IamShatalker == false && ShatalkerObject != null && ShatalkerObject.GetStartMovementDelayTime() > 2)
                    {
                        if (GUI.Button(new Rect(20, 70, 80, 20), "Skip waiting"))
                        {
                            ShatalkerObject.SetStartMovementDelayTime(0);
                        }
                    }
                }
            }
        }

        public static bool AtDebug = false;
        public static string UIDebugType = "";
        public static List<string> ItemsForDebug = new List<string>();
        public static int ItemForDebug = -1;
        public static string AdvancedDebugMode = "";
        public static bool DebugGUI = false;
        public static bool DebugBind = false;

        public class CarTest : MonoBehaviour
        {
            public CarTest(IntPtr ptr) : base(ptr) { }

            public float MotorForce, SteerForce, BreakForce, friction;
            public UnityEngine.WheelCollider Wheel_L;
            public UnityEngine.WheelCollider Wheel_R;
            public UnityEngine.WheelCollider Wheel_BR;
            public UnityEngine.WheelCollider Wheel_BL;
            public GameObject car;

            public GameObject Wheel_R_obj;
            public GameObject Wheel_L_obj;
            public GameObject Wheel_BR_obj;
            public GameObject Wheel_BL_obj;

            void Update()
            {
                if(car == null)
                {
                    return;
                }
                
                float v = 0;
                if(InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.W) == true)
                {
                    v = 1 * MotorForce;
                }else{
                    v = 0;
                }
                Wheel_L_obj.transform.Rotate(Wheel_L.rpm / 60 * 360 * Time.deltaTime, 0, 0);
                Wheel_R_obj.transform.Rotate(Wheel_R.rpm / 60 * 360 * Time.deltaTime, 0, 0);
                Wheel_BR_obj.transform.Rotate(Wheel_L.rpm / 60 * 360 * Time.deltaTime, 0, 0);
                Wheel_BL_obj.transform.Rotate(Wheel_R.rpm / 60 * 360 * Time.deltaTime, 0, 0);


                Wheel_BR.motorTorque = v;
                Wheel_BL.motorTorque = v;

                int steering = 0;

                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.A))
                {
                    steering = -1;
                }
                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.D))
                {
                    steering = 1;
                }
                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.D) == false && InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.A) == false)
                {
                    steering = 0;
                }
                car.transform.Rotate(Vector3.up * SteerForce * Time.deltaTime * steering, Space.World);

                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.Space))
                {
                    Wheel_BR.brakeTorque = BreakForce;
                    Wheel_BL.brakeTorque = BreakForce;
                }else{
                    Wheel_BR.brakeTorque = 0;
                    Wheel_BL.brakeTorque = 0;
                }

                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.W))
                {
                    Wheel_BR.brakeTorque = 0;
                    Wheel_BL.brakeTorque = 0;
                }else{
                    if (Wheel_BR.brakeTorque <= BreakForce && Wheel_BL.brakeTorque <= BreakForce)
                    {
                        Wheel_BR.brakeTorque += friction * Time.deltaTime * BreakForce;
                        Wheel_BL.brakeTorque += friction * Time.deltaTime * BreakForce;
                    }
                    else
                    {
                        Wheel_BR.brakeTorque = BreakForce;
                        Wheel_BL.brakeTorque = BreakForce;
                    }
                }
            }
        }  

        public override void OnGUI()
        {
            DeBugMenu.Render();


            if (NoUI == true && ForcedUiOn == false)
            {
                return;
            }

            //if (anotherbutt != null)
            //{
            //    float dist = Vector3.Distance(GameManager.GetPlayerTransform().position, anotherbutt.transform.position);

            //    if(dist < 1 && AnimState == "Knock" && CarryingPlayer == false)
            //    {
            //        string name;
            //        if(GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_MedicalSupplies_hangar") == true)
            //        {
            //            name = "Press N to revive\n(Requires a First Aid Kit)";
            //        }else
            //        {
            //            name = "You need first aid kit to revive";
            //        }

            //        GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 25, 500, 100), name);
            //    }
            //}

            if(NeedTryReconnect == true)
            {
                GUI.Label(new Rect(700, 30, 500, 100), "Connection Lost,\n Trying reconnect... Attempts: " + AttempsToReconnect);
            }
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

        public static GameObject GetExitPointByName(string exitName, Vector3 posBeforeInteriorLoad)
        {
            GameObject gameObject = (GameObject)null;
            GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("InteriorExitPoint");
            if (gameObjectsWithTag.Length == 0)
            {
                gameObject = GameObject.Find(exitName);
            }
            else
            {
                float num1 = float.PositiveInfinity;
                for (int index = 0; index < gameObjectsWithTag.Length; ++index)
                {
                    if (gameObjectsWithTag[index].name != exitName)
                    {
                        float num2 = Vector3.Distance(gameObjectsWithTag[index].transform.position, posBeforeInteriorLoad);
                        if ((double)num2 < (double)num1)
                        {
                            num1 = num2;
                            gameObject = gameObjectsWithTag[index];
                        }
                    }
                }
                if (gameObject == null)
                {
                    gameObject = GameObject.Find(exitName);
                }
            }
            return gameObject;
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

        //public static string GenerateSeededGUID(int gameSeed, Matrix4x4 matrixRef)
        //{

        //    float matResult = matrixRef.transpose.m00;
        //    matResult = matResult + matrixRef.transpose.m01;
        //    matResult = matResult + matrixRef.transpose.m02;
        //    matResult = matResult + matrixRef.transpose.m03;
        //    matResult = matResult + matrixRef.transpose.m10;
        //    matResult = matResult + matrixRef.transpose.m11;
        //    matResult = matResult + matrixRef.transpose.m12;
        //    matResult = matResult + matrixRef.transpose.m13;
        //    matResult = matResult + matrixRef.transpose.m20;
        //    matResult = matResult + matrixRef.transpose.m21;
        //    matResult = matResult + matrixRef.transpose.m22;
        //    matResult = matResult + matrixRef.transpose.m23;
        //    matResult = matResult + matrixRef.transpose.m30;
        //    matResult = matResult + matrixRef.transpose.m31;
        //    matResult = matResult + matrixRef.transpose.m32;
        //    matResult = matResult + matrixRef.transpose.m33;
        //    int MatrixInt = (int)matResult;

        //    int newSeed = gameSeed + MatrixInt;
        //    MelonLogger.Msg("[Seeded GUIDs] Input Seed " + gameSeed + " Matrix Seed " + MatrixInt + " Combined seed "+ newSeed);
        //    string _chars = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //    System.Random newRNG = new System.Random(newSeed);
        //    string newGUID = "";
        //    for (int i = 1; i < 36; i++)
        //    {
        //        if (i == 9 || i == 14 || i == 19 || i == 24)
        //        {
        //            newGUID = newGUID + "-";
        //        }
        //        int charIndex = newRNG.Next(0, _chars.Length);
        //        newGUID = newGUID + _chars[charIndex];
        //    }
        //    //MelonLogger.Msg("[Seeded GUIDs] Final GUID "+ newGUID);
        //    return newGUID;
        //}

        public static void WarpBody(string _GUID)
        {
            MelonLogger.Msg("Trying warping with door " + _GUID);
            GameObject Door = ObjectGuidManager.Lookup(_GUID);
            if (Door != null)
            {
                MelonLogger.Msg("Simulating entering to door "+ _GUID);

                if(GameManager.m_PlayerManager.PlayerIsDead() == true)
                {
                    KillAfterLoad = true;
                }
                Door.GetComponent<LoadScene>().Activate();
            }
            else{
                MelonLogger.Msg("Not found door" + _GUID);
            }
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
            
            if (iAmHost == true)
            {
                ServerSend.XYZ(0, GameManager.GetPlayerTransform().position, true);
                ServerSend.XYZW(0, GameManager.GetPlayerTransform().rotation, true);
                ServerSend.LEVELID(0, levelid, true);
                ServerSend.LEVELGUID(0, level_guid, true);
                ServerSend.ANIMSTATE(0, MyAnimState, true);
            }
            if(sendMyPosition == true)
            {
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
        public static void BashGetupDelayCamera()
        {
            PlayerStruggle Strug = GameManager.GetPlayerStruggleComponent();
            Strug.BeginReturnToStart(Strug.m_PartnerBaseAi);
            Utils.SetCameraFOVSafe(GameManager.GetMainCamera(), Strug.m_StartCameraFOV);
            GameManager.GetMainCamera().transform.position = Strug.GetOverrideGameCameraPosition();
            GameManager.GetMainCamera().transform.rotation = Strug.m_OverrideGameCamera.transform.rotation;
        }
        public static void BashGetupDelayCamera_old()
        {
            PlayerStruggle Strug = GameManager.GetPlayerStruggleComponent();
            Strug.BashGetupDelayCamera();
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

        public static void UsedBandage(AfflictionBodyArea bodyArea)
        {
            MelonLogger.Msg("Bandaged " + bodyArea.ToString());
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

                IsPublicServer = false;
                ServerConfig.m_DuppedSpawns = DupesIsChecked;
                ServerConfig.m_DuppedContainers = BoxDupesIsChecked;
                ServerConfig.m_PlayersSpawnType = spawnStyle;
                ServerConfig.m_FireSync = FireSyncMode;
                ServerConfig.m_CheatsMode = CheatsMode;
                MaxPlayers = slotsMax;
                ApplyOtherCampfires = true;

                if (ShouldUseSteam == false)
                {
                    HostAServer(PortToHost);
                }else{
                    Server.StartSteam(MaxPlayers);
                }

                UnityEngine.Object.Destroy(UIHostMenu);
                HostMenuHints = new List<GameObject>();
                GameManager.m_IsPaused = false;
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

        public static void HostMenu()
        {
            //if (CantBeUsedForMP == true || LastLoadedGenVersion != BuildInfo.RandomGenVersion || SaveGameSystem.m_CurrentGameMode == SaveSlotType.STORY)
            if (CantBeUsedForMP == true || LastLoadedGenVersion != BuildInfo.RandomGenVersion)
            {
                if (m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
                {
                    string textToShow = "";

                    if (LastLoadedGenVersion == 0)
                    {
                        textToShow = "You can't use this save file for hosting multiplayer! Because this save file has been created before mod has been installed, or on old version of the mod that isn't compatible with current one!";
                    }else{
                        textToShow = "You can't use this save file for hosting multiplayer! Because this save file has been created on old version of the mod that isn't compatible with current one! Save file Generation version " + LastLoadedGenVersion + ". Current one mod use now " + BuildInfo.RandomGenVersion + "!";
                    }

                    //if (SaveGameSystem.m_CurrentGameMode == SaveSlotType.STORY)
                    //{
                    //    textToShow = "Story mode never has been planned to be synced. Mod works only in SANDBOX game mode. I not know why you even try to host story mode, we never announced it will ever work! Please play regular sandbox!";
                    //}

                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.ErrorMessage, textToShow, Panel_Confirmation.ButtonLayout.Button_1, Panel_Confirmation.Background.Transperent, null);
                }
                MelonLogger.Msg("CantBeUsedForMP "+ CantBeUsedForMP+ " LastLoadedGenVersion "+ LastLoadedGenVersion+ " RandomGenVersion "+ BuildInfo.RandomGenVersion);
                return;
            }


            //if (GameManager.GetExperienceModeManagerComponent().GetCurrentExperienceMode().m_ModeType == ExperienceModeType.Custom)
            //{
            //    NoCustomExp();
            //    return;
            //}

            if (UiCanvas != null && UIHostMenu == null)
            {
                UIHostMenu = MakeModObject("MP_HostSettings", UiCanvas.transform);

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


                GameObject P2PtxBox = UIHostMenu.transform.GetChild(4).gameObject;
                //GameObject PublicSteamServer = UIHostMenu.transform.GetChild(5).gameObject;
                GameObject PortsObject = UIHostMenu.transform.GetChild(8).gameObject;
                GameObject FireSyncObj = UIHostMenu.transform.GetChild(9).gameObject;
                GameObject CheatModeListObj = UIHostMenu.transform.GetChild(10).gameObject;
                FireSyncObj.GetComponent<UnityEngine.UI.Dropdown>().Set(ServerConfig.m_FireSync);
                CheatModeListObj.GetComponent<UnityEngine.UI.Dropdown>().Set(ServerConfig.m_CheatsMode);

                if (SteamConnect.CanUseSteam == false)
                {
                    GameObject IsSteamHost = UIHostMenu.transform.GetChild(4).gameObject;
                    IsSteamHost.GetComponent<UnityEngine.UI.Toggle>().Set(false);
                    IsSteamHost.SetActive(false);

                    //PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().Set(false);
                    //PublicSteamServer.SetActive(false);
                    PortsObject.SetActive(false);
                }else{
                    PortsObject.SetActive(true);
                }
                //PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().Set(false);

                AddHintScript(dupesCheckbox);
                AddHintScript(dupesBoxesCheckbox);
                AddHintScript(SpawnStyleList);
                AddHintScript(FireSyncObj);
                AddHintScript(P2PtxBox);
                //AddHintScript(PublicSteamServer);
                AddHintScript(PortsObject);
                AddHintScript(CheatModeListObj);
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

        public static void FakeDeathScreen()
        {
            //6 background
            //3,1
            if(m_InterfaceManager != null) 
            {
                if(InterfaceManager.m_Panel_Log.m_Button_Back != null)
                {
                    InterfaceManager.m_Panel_Log.m_Button_Back.GetComponent<GenericButtonMouseSpawner>().SetText("GIVE UP");
                    InterfaceManager.m_Panel_Log.m_Button_Back.GetComponentInChildren<UILabel>().text = "GIVE UP";
                }
                InterfaceManager.m_Panel_Log.Enable(true);
                InterfaceManager.m_Panel_Log.m_MainLogObject.SetActive(false);
                InterfaceManager.m_Panel_Log.DisableAllMouseButtons();
                InterfaceManager.m_Panel_Log.m_ExamineObject.SetActive(false);
                InterfaceManager.m_Panel_Log.gameObject.transform.GetChild(12).gameObject.SetActive(false);
                InterfaceManager.m_Panel_Log.gameObject.transform.GetChild(13).gameObject.SetActive(false);
                //string causeOfDeathString = GameManager.GetConditionComponent().GetCauseOfDeathString();
                //if (!string.IsNullOrEmpty(GameManager.m_OverridenCauseOfDeath))
                //{
                //    causeOfDeathString = Localization.Get(GameManager.m_OverridenCauseOfDeath);
                //}
                //InterfaceManager.m_Panel_Log.m_Label_CauseOfDeath.text = causeOfDeathString;
                //InterfaceManager.m_Panel_Log.m_Label_CauseOfDeath.gameObject.SetActive(true);
                UILabel UILab = InterfaceManager.m_Panel_Log.gameObject.transform.GetChild(3).GetChild(1).gameObject.GetComponent<UILabel>();
                string text = "YOU ARE KNOCKED\nMaybe someone will rescue you...";
                UILab.mText = text;
                UILab.text = text;
                UILab.ProcessText();
            }
        }
    }
}
