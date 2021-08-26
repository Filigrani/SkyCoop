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
using MelonLoader;
using Harmony;
using UnhollowerRuntimeLib;
using UnhollowerBaseLib;
using GameServer;
using MelonLoader.TinyJSON;

namespace SkyCoop
{
    public class MyMod : MelonMod
    {
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
        public static bool ShiftPressed = false;
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
        public static bool PrviousHasRifle = false;
        public static bool MyHasRifle = false;
        public static bool HasRevolver = false;
        public static bool PrviousHasRevolver = false;
        public static bool MyHasRevolver = false;
        public static bool MyHasAxe = false;
        public static bool PreviousHasAxe = false;
        public static bool PreviousHasMedkit = false;
        public static bool MyHasMedkit = false;
        public static int CycleSkip = 0;
        public static int MyCycleSkip = 0;
        public static bool IsSleeping = false;
        public static bool PreviousSleeping = false;
        public static bool IsCycleSkiping = false;
        public static int LastSelectedWeatherSet;
        public static WeatherSet LastSelectedWeatherSet2;
        public static List<string> KnownAnimals = new List<string>();
        public static int MyArrows = 0;
        public static int PreviousArrows = 0;
        public static int MyFlares = 0;
        public static int PreviousFlares = 0;
        public static int MyBlueFlares = 0;
        public static int PreviousBlueFlares = 0;
        public static int LastRecivedArrows = 0;
        public static int StepState = 0;
        public static GameObject LastObjectUnderCrosshair = null;
        public static Vector3 V3BeforeSleep = new Vector3(0, 0, 0);
        public static bool NeedV3BeforeSleep = false;
        public static GameObject MenuStuffSpawned = null;
        public static bool AnimalsController = true;
        public static int MyTicksOnScene = 0;
        public static int previous_anotherplayer_levelid = 0;
        public static int previous_levelid = 0;
        //public static UnhollowerBaseLib.Il2CppArrayBase<BaseAi> animals = Resources.FindObjectsOfTypeAll<BaseAi>();
        public static float MaxAniamlsSyncDistance = 245f;
        public static int MaxAnimalsSyncCount = 11;
        public static int MaxAnimalsSyncCountOnConnect = 2;
        public static int MaxAnimalsSyncNeed = 0;
        public static int DeltaAnimalsMultiplayer = 4;
        public static string DebugAnimalGUID = "";
        public static string DebugAnimalGUIDLast = "";
        public static GameObject DebugLastAnimal = null;
        public static bool InDarkWalkerMode = false;
        public static float DarkWalkerSpeed = 0.12f;
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
        public static List<BrokenFurnitureSync> BrokenFurniture = new List<BrokenFurnitureSync>();
        public static List<PickedGearSync> PickedGears = new List<PickedGearSync>();
        public static List<PickedGearSync> RecentlyPickedGears = new List<PickedGearSync>();
        public static List<ClimbingRopeSync> DeployedRopes = new List<ClimbingRopeSync>();
        public static List<ContainerOpenSync> LootedContainers = new List<ContainerOpenSync>();
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
        public static int LowHealthStaggerBlockTime = 0;
        public static GearItem EmergencyStimBeforeUse = null;
        public static bool HasInfecitonRisk = false;
        public static bool PreviousHasInfectionRisk = false;
        public static ContainerOpenSync MyContainer = null;
        public static int UpdateLootedContainers = -1;
        public static int UpdatePickedGears = -1;
        public static int UpdatePickedPlants = -1;
        public static int NeedConnectAfterLoad = -1;
        public static int UpdateSnowshelters = -1;
        public static int UpdateRopesAndFurns = -1;
        public static bool SkipEverythingForConnect = false;
        public static GameObject UISteamFreindsMenuObj = null;
        public static int PlayersOnServer = 0;
        public static GameObject UIHostMenu = null;
        public static bool IsPublicServer = false;
        #endregion

        //STRUCTS
        #region STRUCTS
        public class ServerConfigData
        {
            public bool m_FastConsumption = true;
            public bool m_DuppedSpawns = false;
            public bool m_DuppedContainers = false;
            public int m_PlayersSpawnType = 0;
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

        public static class JsonNullsRemover //http://www.java2s.com/Code/CSharp/Network/RemovesJsonnullobjectsfromtheserializedstringandreturnanewstringExtentionMethod.htm
        {
            public static string JsonNullRegEx = "[\"][a-zA-Z0-9_]*[\"]:null[ ]*[,]?";
            public static string JsonNullArrayRegEx = "\\[( *null *,? *)*]";

            public static bool IsEmptyOrNull(string str)
            {
                if (str == null || str == string.Empty)
                {
                    return true;
                }
                return false;
            }
            public static string RemoveJsonNulls(string str)
            {
                if (!IsEmptyOrNull(str))
                {
                    Regex regex = new Regex(JsonNullRegEx);
                    string data = regex.Replace(str, string.Empty);
                    regex = new Regex(JsonNullArrayRegEx);
                    return regex.Replace(data, "[]");
                }
                return null;
            }
        }
        public class FireSourcesSync
        {
            public string m_Guid = "";
            public int m_LevelId = 0;
            public string m_LevelGUID = "";
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Quaternion m_Rotation = new Quaternion(0, 0, 0, 0);
            public float m_Fuel = 0;
            public int m_RemoveIn = 0;
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
            else
            {
                act.m_Action = "Look";
                act.m_DisplayText = "Look";
                act.m_CancleOnMove = false;
            }

            return act;
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

            if(_name == "")
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
            }
            else
            {
                MelonLogger.Msg("Models loaded.");

            }
        }

        public override void OnApplicationQuit()
        {
            MelonLogger.Msg("[CLIENT] Disconnect cause quit game");
            Disconnect();
        }

        public override void OnLevelWasInitialized(int level)
        {
            MelonLogger.Msg("Level initialized: " + level);
            levelid = level;
            MelonLogger.Msg("Level name: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            level_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            MyTicksOnScene = 0;

            if (iAmHost == true)
            {
                using (Packet _packet = new Packet((int)ServerPackets.LEVELID))
                {
                    ServerSend.LEVELID(0, level, true);
                }
            }
            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.LEVELID))
                {
                    _packet.Write(level);
                    SendTCPData(_packet);
                }
            }
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
            if(ServerConfig.m_DuppedSpawns == true)
            {
                return;
            }

            if(GearManager.m_Gear.Count == 0 || PickedGears.Count == 0)
            {
                return;
            }

            for (int i = 0; i < GearManager.m_Gear.Count; i++)
            {
                GearItem currentGear = GearManager.m_Gear.get_Item(i);
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
        }

        public static void UpdateDeployedRopes()
        {
            for (int i = 0; i < RopeAnchorPoint.m_RopeAnchorPoints.Count; i++)
            {
                RopeAnchorPoint rope = RopeAnchorPoint.m_RopeAnchorPoints.get_Item(i);
                ClimbingRopeSync FindData = new ClimbingRopeSync();
                FindData.m_LevelID = levelid;
                FindData.m_LevelGUID = level_guid;
                FindData.m_Position = rope.gameObject.transform.position;
                if (DeployedRopes.Contains(FindData) == true)
                {
                    if (rope.gameObject.transform.position == FindData.m_Position)
                    {
                        MelonLogger.Msg("Processing synced rope " + FindData.m_Position.x + " Y " + FindData.m_Position.y + " Z " + FindData.m_Position.z);
                        if (FindData.m_Deployed == true)
                        {
                            rope.SetRopeActiveState(FindData.m_Deployed, false);
                            rope.SetRopeActiveState(true, true);
                            rope.m_RopeDeployed = true;
                            if (FindData.m_Snapped == true)
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
                        }else{
                            rope.m_RopeDeployed = false;
                            rope.SetRopeActiveState(false, true);
                            if (rope.m_RopeSnapped == true && FindData.m_Snapped == false)
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
            if (HarvestableManager.m_Harvestables.Count == 0 || HarvestedPlants.Count == 0)
            {
                return;
            }

            for (int i = 0; i < HarvestableManager.m_Harvestables.Count; i++)
            {
                Harvestable currentPlant = HarvestableManager.m_Harvestables.get_Item(i);
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
                    GearItemObject currGear = invItems.get_Item(i);
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
                if (ContainerManager.m_Containers.get_Item(i) != null)
                {
                    Container currentBox = ContainerManager.m_Containers.get_Item(i);

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
                GameObject shelter = SnowShelterManager.m_SnowShelters.get_Item(i).gameObject;
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
                    if (currentTagHash != neededTagHash)
                    {
                        if (m_AnimState == "Walk")
                        {
                            m_Animer.Play("Walk", 0);
                        }
                        if (m_AnimState == "Idle")
                        {
                            string current_anim = m_Animer.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                            //MelonLogger.Msg("Ctrl current animation " + current_anim);
                            if (currentTagHash == Animator.StringToHash("Harvesting"))
                            {
                                m_Animer.Play("StopHarvesting", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("HarvestingStanding"))
                            {
                                m_Animer.Play("StopHarvestingStanding", 0);
                            }
                            else if (current_anim == "Male Crouch Pose" && current_anim != "Sit_To_Idle")
                            {
                                m_Animer.Play("Sit_To_Idle", 0);
                            }else{
                                m_Animer.Play("Idle", 0);
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
                            m_Player.transform.GetChild(4).gameObject.SetActive(true);
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
                    }

                    // HANDS LAYER

                    if(m_AnimStateHands == "UnAimGunDone")
                    {
                        m_AnimStateHands = "No";
                    }

                    if(m_AnimState == "Drinking" || m_AnimState == "Eating" || m_AnimState == "Harvesting" || m_AnimState == "HarvestingStanding")
                    {
                        m_PreAnimStateHands = "";
                        m_AnimStateHands = "No";
                    }else{
                        if (HoldingItem == "GEAR_Rifle")
                        {
                            if (m_AnimState != "Ctrl")
                            {
                                if (m_AnimStateHands != "Rifle" && m_AnimStateHands != "Rifle_Sit")
                                {
                                    m_PreAnimStateHands = "Pick";
                                }
                                m_AnimStateHands = "Rifle";
                            }else{
                                if (m_AnimStateHands != "Rifle" && m_AnimStateHands != "Rifle_Sit")
                                {
                                    m_PreAnimStateHands = "Pick_Sit";
                                }
                                m_AnimStateHands = "Rifle_Sit";
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
                            m_Animer.Play(m_AnimStateHands, 1);
                        }else{
                            m_Animer.Play(m_PreAnimStateHands, 1);
                            m_PreAnimStateHands = "";
                        }
                    }
                    // FINGERS LAYER

                    if(m_AnimState == "Drinking" || m_AnimState == "Eating" || m_AnimState == "Harvesting" || m_AnimState == "HarvestingStanding")
                    {
                        m_AnimStateFingers = "No";
                    }else{
                        if (HoldingItem == "GEAR_Revolver" || HoldingItem == "GEAR_FlareGun")
                        {
                            m_AnimStateFingers = "HoldRevolver";
                        }
                        else if (HoldingItem == "GEAR_Stone" || HoldingItem == "GEAR_FlareA" || HoldingItem == "GEAR_BlueFlare" || HoldingItem == "GEAR_Torch")
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
                MelonLogger.Msg("Player "+m_ID +" Picked up something");

                if (playersData[m_ID] == null || playersData[m_ID].m_Levelid != levelid || playersData[m_ID].m_LevelGuid != level_guid)
                {
                    return;
                }
                MelonLogger.Msg("You on same scene with  " + m_ID + " to play pickup");

                string m_AnimState = playersData[m_ID].m_AnimState;
                int armTagHash = m_Animer.GetCurrentAnimatorStateInfo(3).tagHash;
                int armNeededTagHash = Animator.StringToHash("Pickup");
                MelonLogger.Msg("Current state of  " + m_ID + " is "+ m_AnimState);
                MelonLogger.Msg("ArmTagHash  " + armTagHash + " TagHash we need " + armNeededTagHash);
                if (armTagHash != armNeededTagHash)
                {
                    if (m_AnimState != "Ctrl")
                    {
                        m_Animer.Play("DoPickup", 3);
                        MelonLogger.Msg("Playing DoPickup");
                    }else{
                        m_Animer.Play("DoSitPickup", 3);
                        MelonLogger.Msg("Playing DoSitPickup");
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
                    if ((Cdata.m_Hat != m_Hat) || (Cdata.m_Top != m_Top) || (Cdata.m_Bottom != m_Bottom) || (Cdata.m_Boots != m_Boots) || (Cdata.m_Scarf != m_Scarf) || firstUpdate == true)
                    {
                        m_Hat = Cdata.m_Hat;
                        m_Top = Cdata.m_Top;
                        m_Bottom = Cdata.m_Bottom;
                        m_Boots = Cdata.m_Boots;
                        m_Scarf = Cdata.m_Scarf;

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

                    if(HeadVariant == 0)
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
                        if(Pants == 12)
                        {
                            nakedBodyAtBottom = true;
                        }
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
                        }else{
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
                //Map
                hand_l.transform.GetChild(2).gameObject.SetActive(false);
                //Bow
                hand_l.transform.GetChild(3).gameObject.SetActive(false);
                //Bottle
                hand_l.transform.GetChild(4).gameObject.SetActive(false);
                //Food can
                hand_l.transform.GetChild(5).gameObject.SetActive(false);

                if (m_HoldingFood == "")
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
                    }
                    if (m_HoldingItem == "GEAR_FlareGun")
                    {
                        hand_r.transform.GetChild(9).gameObject.SetActive(true);
                    }
                }else{
                    if(m_HoldingFood == "Water")
                    {
                        hand_l.transform.GetChild(4).gameObject.SetActive(true);
                    }else{
                        hand_l.transform.GetChild(5).gameObject.SetActive(true);
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

                        MakenzyHead = body.transform.GetChild(1).GetChild(0).gameObject;
                        AstridHead = body.transform.GetChild(1).GetChild(1).gameObject;
                    }

                    MultiPlayerClientData pD = playersData[m_ID];

                    BloodLostUpdate();
                    UpdateOtherAffictions();
                    UpdateHead();

                    if (m_IsBeingInteractedWith == true)
                    {
                        m_InteractTimer -= Time.deltaTime;

                        if(m_InteractTimer <= 0.0)
                        {
                            DoneAction();
                        }
                    }

                    int m_LevelId = pD.m_Levelid;
                    string m_LevelGUID = pD.m_LevelGuid;

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

                    if (levelid != m_LevelId || level_guid != m_LevelGUID)
                    {
                        clothing.SetActive(false);
                        body.SetActive(false);
                        hip.SetActive(false);
                        m_Player.transform.GetChild(3).gameObject.SetActive(false);
                    }
                    if (levelid == m_LevelId && level_guid == m_LevelGUID)
                    {
                        clothing.SetActive(true);
                        body.SetActive(true);
                        hip.SetActive(true);
                        m_Player.transform.GetChild(3).gameObject.SetActive(true);
                        Vector3 m_XYZ = pD.m_Position;

                        if(pD.m_Female == false)
                        {
                            m_XYZ = pD.m_Position;
                            m_Player.transform.localScale = new Vector3(1, 1, 1);
                        }else{
                            m_XYZ = new Vector3(pD.m_Position.x, pD.m_Position.y-0.194f, pD.m_Position.z);
                            m_Player.transform.localScale = new Vector3(0.867f, 1, 0.867f);
                        }
                        Quaternion m_XYZW = pD.m_Rotation;
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
                            if(m_HeavyBreathSoundReference == 0U)
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
                }
                else
                {
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
                    m_Cont.PlayContainerOpenSound();
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
                    m_Cont.PlayContainerCloseSound();
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

        public static void DamageByBullet(float damage)
        {
            GameManager.GetConditionComponent().AddHealth(-damage, DamageSource.Player);
            GameManager.GetBloodLossComponent().BloodLossStart("OtherPlayer", true, AfflictionOptions.PlayFX);
            GameManager.GetPlayerManagerComponent().ApplyDamageToWornClothing(damage);
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
                if (ArrowsList.get_Item(index) != null && ArrowsList.get_Item(index).InFlight(false))
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
                                ServerSend.BULLETDAMAGE(m_ClientId, (float)m_Damage);
                            }
                        }
                    }
                }
            }

            public void SetLocaZone(GameObject t, MultiplayerPlayer pl)
            {
                m_Obj = t;
                m_Obj.tag = "Flesh";
                m_Obj.layer = vp_Layer.Default;

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

        public static void AllowSpawnAnimals(bool spawn)
        {
            //Il2CppSystem.Collections.Generic.List<SpawnRegion> Regions = GameManager.GetSpawnRegionManager().m_SpawnRegions;

            //for (int i = 0; i < Regions.Count; i++)
            //{
            //    if (Regions[i] != null)
            //    {
            //        SpawnRegion region = Regions[i];
            //        region.SetActive(spawn);
            //    }
            //}

            if (spawn == true)
            {
                //MelonLogger.Msg("[SpawnAnimals] " + Regions.Count + " Regions has been activated");
            }
            else {
                //MelonLogger.Msg("[SpawnAnimals] " + Regions.Count + " Regions has been deactivated");
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
            float LastFoundDistance = 0;
            int LastFoundID = 0;
            bool FoundNewController = false;

            if (_AI.GetAiMode() == AiMode.Dead || _AI.GetAiMode() == AiMode.Struggle || _AI.m_CurrentHP <= 0 || _AI.m_HasEnteredStruggleOnLastAttack == true || ReTakeChill > 0)
            {
                return CurrentController;
            }

            for (int i = 0; i < MaxPlayers; i++)
            {
                Vector3 otherPlayerV3 = new Vector3(0, 0, 0);
                int otherPlayerLevel = 0;

                if(iAmHost && i == 0)
                {
                    otherPlayerV3 = GameManager.GetPlayerTransform().position;
                    otherPlayerLevel = levelid;
                }else{
                    otherPlayerV3 = playersData[i].m_Position;
                    otherPlayerLevel = playersData[i].m_Levelid;
                }
                
                if (playersData[i] != null)
                {
                    if (playersData[i].m_AnimState != "Knock" && otherPlayerLevel == LevelID_Controller && Vector3.Distance(V3_Controller, otherPlayerV3) < MinimalDistance)
                    {
                        if(LastFoundDistance == 0)
                        {
                            if(Vector3.Distance(V3, otherPlayerV3) < Vector3.Distance(V3, V3_Controller))
                            {
                                LastFoundDistance = Vector3.Distance(V3, otherPlayerV3);
                                LastFoundID = i;
                                FoundNewController = true;
                            }
                        }
                        else
                        {
                            if (Vector3.Distance(V3, otherPlayerV3) < LastFoundDistance)
                            {
                                LastFoundDistance = Vector3.Distance(V3, otherPlayerV3);
                                LastFoundID = i;
                                FoundNewController = true;
                            }
                        }
                    }
                }
            }

            if(FoundNewController == true)
            {
                return LastFoundID;
            }
            else
            {
                return CurrentController;
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
                                }else{
                                    NewController = GetClosestPlayerToAnimal(m_Animal, m_Animal.GetComponent<AnimalUpdates>().ReTakeCoolDown, m_ClientController, playersData[m_ClientController].m_Position, playersData[m_ClientController].m_Levelid);
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

                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.ANIMALSYNC))
                            {
                                ServerSend.ANIMALSYNC(0, sync, true, levelid, m_Animal.transform.position);
                            }
                        }

                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.ANIMALSYNC))
                            {
                                _packet.Write(sync);
                                SendTCPData(_packet);
                            }
                        }
                    }
                }
            }

            //void SetTrigger(int trigger)
            //{
            //    BaseAi _AI = m_Animal.GetComponent<BaseAi>();

            //    if (_AI != null)
            //    {
            //        Animator AN = _AI.m_Animator;
            //        AN.SetTrigger(IntToTrigger(trigger, _AI));
            //    }
            //}

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
                                if(m_Animal.GetComponent<BaseAi>().m_SpawnRegionParent != null && m_Animal.GetComponent<BaseAi>().m_SpawnRegionParent.gameObject.GetComponent<ObjectGuid>() != null)
                                {
                                    regionGUID = m_Animal.GetComponent<BaseAi>().m_SpawnRegionParent.gameObject.GetComponent<ObjectGuid>().Get();
                                }
                                
                                MakeAnimalActive(m_Animal, false, regionGUID);
                            }
                        }else{
                            if (m_InActive == true)
                            {
                                //MelonLogger.Msg("Re-creating animal under my control.");
                                m_InActive = false;
                                ReCreateAnimal(m_Animal, m_PendingProxy);
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
                                if (m_Animal.GetComponent<BaseAi>().m_SpawnRegionParent != null && m_Animal.GetComponent<BaseAi>().m_SpawnRegionParent.gameObject.GetComponent<ObjectGuid>() != null)
                                {
                                    regionGUID = m_Animal.GetComponent<BaseAi>().m_SpawnRegionParent.gameObject.GetComponent<ObjectGuid>().Get();
                                }
                                MakeAnimalActive(m_Animal, false, regionGUID);
                            }
                        }else{
                            if (m_InActive == true)
                            {
                                m_InActive = false;
                                ReCreateAnimal(m_Animal, m_PendingProxy);
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

                    float dis = MaxAniamlsSyncDistance;
                    if (players[0] != null)
                    {
                        dis = Vector3.Distance(players[0].transform.position, m_Animal.transform.position);
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

        public static AiMode IntToAiMode(int i)
        {
            if (i == 0) { return AiMode.None; }
            if (i == 1) { return AiMode.Attack; }
            if (i == 2) { return AiMode.Dead; }
            if (i == 3) { return AiMode.Feeding; }
            if (i == 4) { return AiMode.Flee; }
            if (i == 5) { return AiMode.FollowWaypoints; }
            if (i == 6) { return AiMode.HoldGround; }
            if (i == 7) { return AiMode.Idle; }
            if (i == 8) { return AiMode.Investigate; }
            if (i == 9) { return AiMode.InvestigateFood; }
            if (i == 10) { return AiMode.InvestigateSmell; }
            if (i == 11) { return AiMode.Rooted; }
            if (i == 12) { return AiMode.Sleep; }
            if (i == 13) { return AiMode.Stalking; }
            if (i == 14) { return AiMode.Struggle; }
            if (i == 15) { return AiMode.Wander; }
            if (i == 16) { return AiMode.WanderPaused; }
            if (i == 17) { return AiMode.GoToPoint; }
            if (i == 18) { return AiMode.InteractWithProp; }
            if (i == 19) { return AiMode.ScriptedSequence; }
            if (i == 20) { return AiMode.Stunned; }
            if (i == 21) { return AiMode.ScratchingAntlers; }
            if (i == 22) { return AiMode.PatrolPointsOfInterest; }
            if (i == 23) { return AiMode.HideAndSeek; }
            if (i == 24) { return AiMode.JoinPack; }
            if (i == 25) { return AiMode.PassingAttack; }
            if (i == 26) { return AiMode.Howl; }
            if (i == 27) { return AiMode.Disabled; }

            return AiMode.None;
        }

        public static int AiModeToInt(AiMode i)
        {
            if (i == AiMode.None) { return 0; }
            if (i == AiMode.Attack) { return 1; }
            if (i == AiMode.Dead) { return 2; }
            if (i == AiMode.Feeding) { return 3; }
            if (i == AiMode.Flee) { return 4; }
            if (i == AiMode.FollowWaypoints) { return 5; }
            if (i == AiMode.HoldGround) { return 6; }
            if (i == AiMode.Idle) { return 7; }
            if (i == AiMode.Investigate) { return 8; }
            if (i == AiMode.InvestigateFood) { return 9; }
            if (i == AiMode.InvestigateSmell) { return 10; }
            if (i == AiMode.Rooted) { return 11; }
            if (i == AiMode.Sleep) { return 12; }
            if (i == AiMode.Stalking) { return 13; }
            if (i == AiMode.Struggle) { return 14; }
            if (i == AiMode.Wander) { return 15; }
            if (i == AiMode.WanderPaused) { return 16; }
            if (i == AiMode.GoToPoint) { return 17; }
            if (i == AiMode.InteractWithProp) { return 18; }
            if (i == AiMode.ScriptedSequence) { return 19; }
            if (i == AiMode.Stunned) { return 20; }
            if (i == AiMode.ScratchingAntlers) { return 21; }
            if (i == AiMode.PatrolPointsOfInterest) { return 22; }
            if (i == AiMode.HideAndSeek) { return 23; }
            if (i == AiMode.JoinPack) { return 24; }
            if (i == AiMode.PassingAttack) { return 25; }
            if (i == AiMode.Howl) { return 26; }
            if (i == AiMode.Disabled) { return 27; }

            return 0;
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
        public static GameObject GetGearItemObject(string name) => Resources.Load(name).Cast<GameObject>();

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
                MelonLogger.Msg("Trying connect to " + ip);
                InitializeClientData();
                //tcp.Connect();

                if(udp == null)
                {
                    MelonLogger.Msg("udp is null");
                }

                udp.Connect(instance.port+1, ip);
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
            //{ (int)ServerPackets.MAKEFIRE, ClientHandle.MAKEFIRE},
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

                            if(packetHandlers[_packetId] != null)
                            {
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
                        //MelonLogger.Msg(ConsoleColor.Cyan, "Handle Packet with ID "+ _packetId);
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
                    MyMod.instance.udp.socket.Client.BeginDisconnect(true, DoneDisconnect, null);
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

        //public static void GiveRecivedItem(GearItem got)
        //{
        //    string dummy_name = got.m_GearName;
        //    string give_name = "";

        //    string say = "";
        //    bool watermode = false;
        //    LiquidQuality water_q = LiquidQuality.Potable;

        //    if (dummy_name.Contains("(Clone)")) //If it has ugly (Clone), cutting it.
        //    {
        //        int L = dummy_name.Length - 7;
        //        give_name = dummy_name.Remove(L, 7);
        //    }
        //    else
        //    {
        //        give_name = dummy_name;
        //    }

        //    if (give_name == "GEAR_WaterSupplyPotable")
        //    {
        //        watermode = true;
        //    }
        //    if (give_name == "GEAR_WaterSupplyNotPotable")
        //    {
        //        watermode = true;
        //        water_q = LiquidQuality.NonPotable;
        //    }

        //    if (watermode == false) // If this is water we not give new item, but just add to supply.
        //    {
        //        //Creating new item to load all static parameters of item as base, to not ask host about constant values of item.
        //        GearItem new_gear = GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory(give_name, 1);
        //        //Setting dynamic values of item that we got from host
        //        new_gear.m_CurrentHP = got.m_CurrentHP;
        //        new_gear.m_WeightKG = got.m_WeightKG;
        //        new_gear.m_GearBreakConditionThreshold = got.m_GearBreakConditionThreshold;

        //        if (new_gear.m_FoodItem != null) // If it food, we need load FoodItem component too.
        //        {
        //            new_gear.m_FoodItem.m_CaloriesTotal = got.m_FoodItem.m_CaloriesTotal;
        //            new_gear.m_FoodItem.m_CaloriesRemaining = got.m_FoodItem.m_CaloriesRemaining;
        //            new_gear.m_FoodItem.m_HeatPercent = got.m_FoodItem.m_HeatPercent;
        //            new_gear.m_FoodItem.m_Packaged = got.m_FoodItem.m_Packaged;
        //            new_gear.m_FoodItem.m_Opened = got.m_FoodItem.m_Opened;
        //        }
        //        if (new_gear.m_EvolveItem != null) // If is evolve item(So stupid name) then we need load item of it was drying.
        //        {
        //            new_gear.m_EvolveItem.m_TimeSpentEvolvingGameHours = got.m_EvolveItem.m_TimeSpentEvolvingGameHours;
        //        }
        //        say = new_gear.m_LocalizedDisplayName.Text();
        //    }
        //    else
        //    {
        //        string bottlename = Resources.Load(give_name).Cast<GameObject>().GetComponent<GearItem>().m_LocalizedDisplayName.Text();

        //        MelonLogger.Msg("Got water " + got.m_WaterSupply.m_VolumeInLiters);
        //        if (got.m_WaterSupply.m_VolumeInLiters == 0.5f)
        //        {
        //            say = "half liter of " + bottlename;
        //        }
        //        else
        //        {
        //            say = got.m_WaterSupply.m_VolumeInLiters + " of " + bottlename;
        //        }
        //        GameManager.GetInventoryComponent().AddToWaterSupply(got.m_WaterSupply.m_VolumeInLiters, water_q);
        //    }

        //    HUDMessage.AddMessage("Other player gave you " + say + ".");

        //    MelonLogger.Msg("Other player gave you item " + give_name);
        //}

        public static void GiveRecivedItem(GearItemDataPacket gearData)
        {
            MelonLogger.Msg(ConsoleColor.Blue, "Got gear with name ["+ gearData.m_GearName + "] DATA: "+ gearData.m_DataProxy);

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
                GearItem new_gear = GameManager.GetPlayerManagerComponent().InstatiateItemAtLocation(give_name, 1, new Vector3(0,0,0), false);
                new_gear.Deserialize(gearData.m_DataProxy);
                GameManager.GetPlayerManagerComponent().AddItemToPlayerInventory(new_gear);
                //GameManager.GetInventoryComponent().AddGear(new_gear.gameObject);
                say = new_gear.m_LocalizedDisplayName.Text();
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
            KillAfterLoad = false;
            SetRevivedStats();
            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Normal);
            Condition con = GameManager.GetConditionComponent();
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
            }
            else
            {
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
                SpawnAnimal(prefabName, obj.m_position, _guid, obj.m_SpawnRegionGUID, ShouldRecreate, obj.m_ProxySave);
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
                        component1.Damage = component2.m_DamageHP;
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
                Container box = Boxes.get_Item(i);
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

        Vector3 SnappedPosition(Vector3 pointToSnap, Vector3 blockCenterPosition)
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
            MelonLogger.Msg("Skipping "+ h+" hour(s) now should be "+ OverridedHourse);
            MyMod.EveryInGameMinute();
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
                if (GameManager.m_InterfaceManager != null && InterfaceManager.m_Panel_Rest != null)
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

        public static List<FireSourcesSync> FireSources = new List<FireSourcesSync>();
        public static void MayAddFireSources(FireSourcesSync fire)
        {
            fire.m_RemoveIn = 5;
            for (int i = 0; i < FireSources.Count; i++)
            {
                FireSourcesSync currFire = FireSources[i];
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

        private static void EverySecond()
        {
            for (int i = 0; i < FireSources.Count; i++)
            {
                if(FireSources[i] != null)
                {
                    if(FireSources[i].m_RemoveIn > 0)
                    {
                        FireSources[i].m_RemoveIn = FireSources[i].m_RemoveIn - 1;
                    }else{
                        FireSources.RemoveAt(i);
                    }
                }
            }

            for (int i = 0; i < FireManager.m_Fires.Count; i++)
            {
                Fire fireCur = FireManager.m_Fires.get_Item(i);
                //MelonLogger.Msg("[FireManager] "+i+". "+ fireCur.gameObject.name+" Burning "+ fireCur.IsBurning());
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
            }

            if (level_name != "Empty" && level_name != "Boot" && level_name != "MainMenu")
            {
                //MelonLogger.Msg("Trying call UpdateMyClothing");
                UpdateMyClothing();

                if(LowHealthStaggerBlockTime > 0)
                {
                    LowHealthStaggerBlockTime = LowHealthStaggerBlockTime - 1;
                }

                if(UpdateLootedContainers > 0)
                {
                    UpdateLootedContainers = UpdateLootedContainers - 1;
                    if(UpdateLootedContainers == 0)
                    {
                        UpdateLootedContainers = -1;
                        MelonLogger.Msg(ConsoleColor.Blue, "Apply looted containers");
                        ApplyLootedContainers();
                    }
                }

                if (UpdatePickedGears > 0)
                {
                    UpdatePickedGears = UpdatePickedGears - 1;
                    if (UpdatePickedGears == 0)
                    {
                        UpdatePickedGears = -1;
                        MelonLogger.Msg(ConsoleColor.Blue, "Apply picked gears");
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
                        MelonLogger.Msg(ConsoleColor.Blue, "Apply all showshelters");
                        LoadAllSnowSheltersByOther();
                    }
                }

                if (UpdateRopesAndFurns > 0)
                {
                    UpdateRopesAndFurns = UpdateRopesAndFurns - 1;

                    if(UpdateRopesAndFurns == 0)
                    {
                        MelonLogger.Msg(ConsoleColor.Blue, "Apply ropes and furns");
                        DestoryBrokenFurniture();
                        UpdateDeployedRopes();
                    }
                }
                if (RemoveAttachedObjectsAfterSecond == true)
                {
                    DestoryPickedGears();
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
                        if(Server.clients[i].TimeOutTime > 10)
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
            //if (LastConnectedIp != "" || iAmHost == true)
            //{
            //    if(NeedTryReconnect == false)
            //    {
            //        if (iAmHost == true)
            //        {
            //            using (Packet _packet = new Packet((int)ServerPackets.KEEPITALIVE))
            //            {
            //                ServerSend.KEEPITALIVE(0, true);
            //            }
            //        }

            //        if (sendMyPosition == true)
            //        {
            //            using (Packet _packet = new Packet((int)ClientPackets.KEEPITALIVE))
            //            {
            //                _packet.Write(true);
            //                SendTCPData(_packet);
            //            }
            //        }

            //        if (NoHostResponceSeconds > 10)
            //        {
            //            NeedTryReconnect = true;
            //        }
            //    }else{
            //        if (iAmHost == false)
            //        {
            //            if (TryingReconnect == false)
            //            {
            //                TryingReconnect = true;
            //                ProcessingReconnect();
            //            }
            //        }
            //        if (iAmHost == true && SendRQEvent == false)
            //        {
            //            using (Packet _packet = new Packet((int)ServerPackets.RQRECONNECT))
            //            {
            //                ServerSend.RQRECONNECT(1, true);
            //            }

            //            SendRQEvent = true;
            //        }
            //    }

            //    //NoHostResponceSeconds = NoHostResponceSeconds + 1;
            //}

            if (LastConnectedIp != "" || iAmHost == true)
            {
                if (iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.KEEPITALIVE))
                    {
                        ServerSend.KEEPITALIVE(0, true);
                    }
                }

                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.KEEPITALIVE))
                    {
                        _packet.Write(true);
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

            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.CARRYBODY))
                {
                    _packet.Write(CarryingPlayer);
                    SendTCPData(_packet);
                }
            }
            if (iAmHost == true)
            {
                using (Packet _packet = new Packet((int)ServerPackets.CARRYBODY))
                {
                    ServerSend.CARRYBODY(1, CarryingPlayer);
                }
            }

            //if (IamShatalker == true && anotherbutt != null)
            //{
            //    Outline OL = anotherbutt.GetComponent<Outline>();
            //    if (OL == null)
            //    {
            //        anotherbutt.AddComponent<Outline>();
            //        OL = anotherbutt.GetComponent<Outline>();
            //        OL.m_OutlineColor = Color.green;
            //        OL.needsUpdate = true;
            //    }

            //    if (LureIsActive == true)
            //    {
            //        if (OL != null)
            //        {
            //            if(OL.m_OutlineColor != Color.clear)
            //            {
            //                OL.m_OutlineColor = Color.clear;
            //                OL.needsUpdate = true;
            //            }
            //        }
            //        anotherbutt.SetActive(false);
            //    }else{
            //        if (OL != null)
            //        {
            //            if (OL.m_OutlineColor != Color.green)
            //            {
            //                OL.m_OutlineColor = Color.green;
            //                OL.needsUpdate = true;
            //            }
            //        }
            //        anotherbutt.SetActive(true);
            //    }
            //}

            if (IsShatalkerMode() == true)
            {
                if (IamShatalker == false && ShatalkerObject != null)
                {
                    if (ShatalkerObject.GetStartMovementDelayTime() > 1)
                    {
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.DWCOUNTDOWN))
                            {
                                _packet.Write(ShatalkerObject.GetStartMovementDelayTime());
                                SendTCPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.DWCOUNTDOWN))
                            {
                                ServerSend.DWCOUNTDOWN(1, ShatalkerObject.GetStartMovementDelayTime());
                            }
                        }
                    }

                    if (WardWidget != null)
                    {
                        WardIsActive = WardWidget.activeSelf;
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.WARDISACTIVE))
                            {
                                _packet.Write(WardIsActive);
                                SendTCPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.WARDISACTIVE))
                            {
                                ServerSend.WARDISACTIVE(1, WardIsActive);
                            }
                        }
                    }
                    if (LureWidget != null)
                    {
                        LureIsActive = LureWidget.activeSelf;
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.LUREISACTIVE))
                            {
                                _packet.Write(LureIsActive);
                                SendTCPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.LUREISACTIVE))
                            {
                                ServerSend.LUREISACTIVE(1, LureIsActive);
                            }
                        }
                    }
                }
            }
        }

        private static void EveryInGameMinute()
        {
            OverridedMinutes = OverridedMinutes + 1;
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

            //MelonLogger.Msg("Seed "+ GameManager.m_SceneTransitionData.m_GameRandomSeed);

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
            else{
                //MelonLogger.Msg("Can't send wind sync.");
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

            SkinnedMeshRenderer mesh1 = null;
            SkinnedMeshRenderer mesh2 = null;
            SkinnedMeshRenderer mesh3 = null;

            if (animal.name.StartsWith("WILDLIFE_Wolf"))
            {
                //7 Rig, Meshs 12,13
                mesh1 = animal.transform.GetChild(7).GetChild(12).gameObject.GetComponent<SkinnedMeshRenderer>();
                mesh2 = animal.transform.GetChild(7).GetChild(13).gameObject.GetComponent<SkinnedMeshRenderer>();
            }
            else if (animal.name.StartsWith("WILDLIFE_Stag"))
            {
                //23 Mesh
                mesh1 = animal.transform.GetChild(23).gameObject.GetComponent<SkinnedMeshRenderer>();
            }

            else if (animal.name.StartsWith("WILDLIFE_Rabbit"))
            {
                // 6,7 Meshs
                mesh1 = animal.transform.GetChild(6).gameObject.GetComponent<SkinnedMeshRenderer>();
                mesh2 = animal.transform.GetChild(7).gameObject.GetComponent<SkinnedMeshRenderer>();
            }
            else if (animal.name.StartsWith("WILDLIFE_Moose"))
            {
                // 24,25 Meshs
                mesh1 = animal.transform.GetChild(24).gameObject.GetComponent<SkinnedMeshRenderer>();
                mesh2 = animal.transform.GetChild(25).gameObject.GetComponent<SkinnedMeshRenderer>();
            }
            else if (animal.name.StartsWith("WILDLIFE_Bear"))
            {
                // 10,11,12 Meshs
                mesh1 = animal.transform.GetChild(10).gameObject.GetComponent<SkinnedMeshRenderer>();
                mesh2 = animal.transform.GetChild(11).gameObject.GetComponent<SkinnedMeshRenderer>();
                mesh3 = animal.transform.GetChild(12).gameObject.GetComponent<SkinnedMeshRenderer>();
            }

            AnimalUpdates au = animal.GetComponent<AnimalUpdates>();

            if (mesh1 != null){if(au != null){au.m_Mesh1 = mesh1;}}
            if (mesh2 != null){if(au != null){au.m_Mesh2 = mesh2;}}
            if (mesh3 != null){if(au != null){au.m_Mesh3 = mesh3;}}
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
                }
                else
                {
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
            SpawnAnimal(prefab, pos, _GUID, regionGUID, true, JsonProx);
            return;
        }
        public static void ReCreateAnimal_test_v2(GameObject animal, string proxy = "")
        {
            if (animal == null)
            {
                return;
            }
            string regionGUID = "";
            if (animal.GetComponent<BaseAi>().m_SpawnRegionParent != null && animal.GetComponent<BaseAi>().m_SpawnRegionParent.gameObject.GetComponent<ObjectGuid>() != null)
            {
                regionGUID = animal.GetComponent<BaseAi>().m_SpawnRegionParent.gameObject.GetComponent<ObjectGuid>().Get();
            }
            MakeAnimalActive(animal, true, regionGUID);
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
                    BaseAiManager.CreateMoveAgent(animal.transform, animal.GetComponent<BaseAi>(), animal.transform.position);
                }
                animal.transform.parent.GetComponent<MoveAgent>().enabled = active;
                if (active == true)
                {
                    BaseAiManager.CreateMoveAgent(animal.transform, animal.GetComponent<BaseAi>(), animal.transform.position);
                }
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
                //UnityEngine.Component.Destroy(animal.GetComponent<BaseAi>());
                animal.GetComponent<BaseAi>().enabled = active;

                if (active == true)
                {
                    BaseAi _AI = animal.GetComponent<BaseAi>();

                    GameObject RegionSpawnObj = ObjectGuidManager.Lookup(sRegionGUID);

                    if(RegionSpawnObj != null)
                    {
                        SpawnRegion sp = RegionSpawnObj.GetComponent<SpawnRegion>();
                        _AI.SetSpawnRegionParent(sp);
                        sp.m_Spawns.Add(animal.GetComponent<BaseAi>());
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

        public static void SpawnAnimal(string prefabName, Vector3 v3spawn, string _guid, string sRegionGUID, bool recreateion = false, string prox = "")
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
            }
            else
            {
                animal.AddComponent<ObjectGuid>();
                animal.GetComponent<ObjectGuid>().m_Guid = _guid;
                ObjectGuidManager.RegisterGuid(_guid, animal);
            }

            GameObject RegionSpawnObj = ObjectGuidManager.Lookup(sRegionGUID);

            if (RegionSpawnObj != null)
            {
                SpawnRegion sp = RegionSpawnObj.GetComponent<SpawnRegion>();
                _AI.SetSpawnRegionParent(sp);
                sp.m_Spawns.Add(animal.GetComponent<BaseAi>());
            }

            AnimalUpdates au = animal.GetComponent<AnimalUpdates>();

            if (au == null)
            {
                animal.AddComponent<AnimalUpdates>();

                au = animal.GetComponent<AnimalUpdates>();
            }
            au.m_ToGo = v3spawn;

            if (recreateion == true)
            {
                if(prox != "")
                {
                    //MelonLogger.Msg("Spawn Recreation deserialize " + prox);
                    _AI.Deserialize(prox);
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

            BaseAiManager.CreateMoveAgent(animal.transform, _AI, v3spawn);

            //MelonLogger.Msg(animal.GetComponent<ObjectGuid>().Get() + " Created " + prefabName);
        }

        public static bool ShouldbeAnimalController(int _Ticks, int _Level, int _From)
        {
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

        public static void SetAnimalControllers()
        {
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
                    }
                    else
                    {
                        if (Server.clients[i].IsBusy() == true)
                        {
                            bool shouldBeController = false;
                            if(playersData[i].m_AnimState != "Knock")
                            {
                                shouldBeController = ShouldbeAnimalController(playersData[i].m_TicksOnScene, playersData[i].m_Levelid, i);
                            }else{
                                shouldBeController = false;
                            }
                            using (Packet _packet = new Packet((int)ServerPackets.ANIMALROLE))
                            {
                                ServerSend.ANIMALROLE(i, shouldBeController);
                            }
                        }
                    }
                }
            }
        }

        //public static void SetAnimalControllerRole()
        //{
        //    if (IsShatalkerMode() == true)
        //    {
        //        return;
        //    }

        //    bool controller = false;
        //    bool mycontroller = false;

        //    if (levelid != anotherplayer_levelid)
        //    {
        //        controller = true;
        //        mycontroller = true;
        //    }else {
        //        if (levelid == anotherplayer_levelid)
        //        {
        //            if (MyTicksOnScene > TicksOnScene)
        //            {
        //                controller = false;
        //                mycontroller = true;
        //            } else {
        //                if (MyTicksOnScene < TicksOnScene)
        //                {
        //                    controller = true;
        //                    mycontroller = false;
        //                } else {
        //                    if (MyTicksOnScene == TicksOnScene)  //Wot? This almsot not possible...but need to check this anyway.
        //                    {
        //                        controller = false;
        //                        mycontroller = true;
        //                    }
        //                }
        //            }

        //            if (mycontroller == true && IsDead == true)
        //            {
        //                mycontroller = false;
        //                controller = true;
        //            }

        //            if (controller == true)
        //            {
        //                mycontroller = true;
        //                controller = false;
        //            }
        //        }
        //    }

        //    AnimalsController = mycontroller;

        //    AllowSpawnAnimals(AnimalsController);


        //    using (Packet _packet = new Packet((int)ServerPackets.ANIMALROLE))
        //    {
        //        ServerSend.ANIMALROLE(1, controller);
        //    }
        //}

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
        public static bool OverrideMenusForConnection = false;

        public static void SendSlotData(int _forClient)
        {
            MelonLogger.Msg("Sending savedata for "+_forClient);
            SaveSlotSync SaveData = new SaveSlotSync();
            SaveData.m_Episode = (int) SaveGameSystem.m_CurrentEpisode;
            SaveData.m_SaveSlotType = (int) SaveGameSystem.m_CurrentGameMode;
            SaveData.m_Seed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            SaveData.m_ExperienceMode = (int) ExperienceModeManager.s_CurrentModeType;
            SaveData.m_Location = (int) RegionManager.GetCurrentRegion();

            using (Packet __packet = new Packet((int)ServerPackets.SAVEDATA))
            {
                ServerSend.SAVEDATA(_forClient, SaveData);
            }
        }

        public static void SelectBagesForConnection()
        {
            GameAudioManager.PlayGuiConfirm();

            if(PendingSave.m_SaveSlotType == (int)SaveSlotType.SANDBOX && InterfaceManager.m_Panel_MainMenu.GetNumUnlockedFeats() > 0)
            {
                InterfaceManager.m_Panel_MainMenu.SelectWindow(InterfaceManager.m_Panel_MainMenu.m_SelectFeatWindow);
            }
        }

        public static void SelectGenderForConnection()
        {
            InterfaceManager.m_Panel_SelectSurvivor.Enable(true);

            InterfaceManager.m_Panel_SelectSurvivor.m_BasicMenu.m_OnClickBackAction = null;
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
                SaveSlotInfo Slot = Slots.get_Item(i);

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
                OverrideMenusForConnection = true;
                LetChooseSpawnForClient(PendingSave);
            }
        }

        public static void LetChooseSpawnForClient(SaveSlotSync Data)
        {
            
            if(ServerConfig.m_PlayersSpawnType == 0) // Same as host.
            {
                SelectGenderForConnection();
            }
            else if (ServerConfig.m_PlayersSpawnType == 1) // Can select
            {
                if(Data.m_ExperienceMode == (int)ExperienceModeType.Interloper)
                {
                    Data.m_Location = (int)GameRegion.RandomRegion;
                    SelectGenderForConnection();
                }else{
                    InterfaceManager.m_Panel_SelectRegion.Enable(true);
                }
            }
            else if (ServerConfig.m_PlayersSpawnType == 2) // Random
            {
                Data.m_Location = (int)GameRegion.RandomRegion;
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

            MelonLogger.Msg("Save slot created!");
            MelonLogger.Msg("Save slot current name " + SaveGameSystem.GetCurrentSaveName());

            InterfaceManager.m_Panel_OptionsMenu.m_State.m_StartRegion = Region;
            InterfaceManager.m_Panel_MainMenu.OnSandboxFinal();
            GameManager.m_SceneTransitionData.m_GameRandomSeed = Seed;
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
            if(name == "" || name == " " || name == "Player")
            {
                return false;
            }else{
                return true;
            }
        }

        public static void FindPlayerToTrade()
        {
            Vector3 myXYZ = GameManager.GetPlayerTransform().position;
            float LastDistance = 0;
            int LastPlayerFoundID = -1;
            for (int i = 0; i < playersData.Count; i++)
            {
                if(playersData[i] != null && playersData[i].m_Levelid == levelid && Vector3.Distance(myXYZ,playersData[i].m_Position) < 15 && i != instance.myId)
                {
                    float checkDis = Vector3.Distance(myXYZ, playersData[i].m_Position);

                    if(LastDistance == 0)
                    {
                        LastPlayerFoundID = i;
                        LastDistance = checkDis;
                    }else{
                        if (checkDis < LastDistance)
                        {
                            LastPlayerFoundID = i;
                            LastDistance = checkDis;
                        }
                    }
                }
            }
            GiveItemTo = LastPlayerFoundID;

            if(GiveItemLableObj != null)
            {
                if(GiveItemTo != -1)
                {
                    //GiveItemLable.text = "PRESS G TO GIVE ITEM TO " + playersData[LastPlayerFoundID].m_Name.ToUpper();
                    GiveItemLable.text = "ID " + LastPlayerFoundID;
                }else{
                    GiveItemLable.text = "";
                }
            }else{
                //MelonLogger.Msg("GiveItemLableObj is null");
            }
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

                    m_Player.AddComponent<MultiplayerPlayerAnimator>().m_Animer = m_Player.GetComponent<Animator>();
                    m_Player.AddComponent<MultiplayerPlayerClothingManager>().m_Player = m_Player;

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
                }
            }
        }

        public static bool HasWaitForConnect = false;

        public static void DoWaitForConnect()
        {
            if(HasWaitForConnect == false)
            {
                if(GameManager.m_InterfaceManager != null && GameManager.m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
                {
                    if(InterfaceManager.m_Panel_MainMenu != null && InterfaceManager.m_Panel_MainMenu.m_Sprite_FadeOverlay != null)
                    {
                        InterfaceManager.m_Panel_MainMenu.m_Sprite_FadeOverlay.gameObject.SetActive(false);
                    }
                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Waiting, "Connecting...", "\nPlease wait, if you see this message for too long, you have connection problems. Or host has game minimized.", Panel_Confirmation.ButtonLayout.Button_0, Panel_Confirmation.Background.Transperent, null, null);
                    HasWaitForConnect = true;
                }
            }
        }
        public static void RemoveWaitForConnect()
        {
            HasWaitForConnect = false;
            if (GameManager.m_InterfaceManager != null && GameManager.m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null)
            {
                InterfaceManager.m_Panel_Confirmation.OnCancel();
            }
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

        public override void OnUpdate()
        {
            UpdateMain();
            GameLogic.Update();

            if (GameManager.m_SceneTransitionData != null)
            {
                level_guid = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;
                if (level_guid != previous_level_guid)
                {
                    previous_level_guid = level_guid;
                    MelonLogger.Msg("Scene GUID " + GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent);

                    if (sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.LEVELGUID))
                        {
                            _packet.Write(level_guid);
                            SendTCPData(_packet);
                        }
                    }

                    if (iAmHost == true)
                    {
                        ServerSend.LEVELGUID(0, level_guid, true);
                    }
                }
            }

            if (GameManager.m_InterfaceManager != null && GameManager.m_InterfaceManager != null && InterfaceManager.m_Panel_Confirmation != null && NeedConnectAfterLoad != -1)
            {
                DoWaitForConnect();
            }

            if(GameManager.m_InterfaceManager != null && InterfaceManager.m_Panel_SnowShelterInteract != null)
            {
                CancleDismantling();
            }

            if(GameManager.m_InterfaceManager != null && InterfaceManager.m_Panel_Rest != null && InterfaceManager.m_Panel_Rest.isActiveAndEnabled == true)
            {
                //InterfaceManager.m_Panel_Rest.OnRest();
                //GameObject new_button = new GameObject();
                //UnityEngine.Object.Instantiate(new_button, InterfaceManager.m_Panel_Rest.m_SleepButton.transform.parent);

                if(SleepingButtons == null)
                {
                    SleepingButtons = InterfaceManager.m_Panel_Rest.gameObject.transform.GetChild(3).gameObject;
                    WaitForSleepLable = UnityEngine.Object.Instantiate(SleepingButtons.transform.GetChild(2).GetChild(1).gameObject, InterfaceManager.m_Panel_Rest.gameObject.transform);
                    UnityEngine.Object.Destroy(WaitForSleepLable.GetComponent<UILocalize>());
                    WaitForSleepLable.GetComponent<UILabel>().text = "WAITING OTHER PLAYERS TO SLEEP";
                    WaitForSleepLable.SetActive(false);
                }

                if(new_button == null)
                {
                    new_button = UnityEngine.Object.Instantiate(InterfaceManager.m_Panel_Rest.m_SleepButton, InterfaceManager.m_Panel_Rest.m_SleepButton.transform.parent);
                    new_button.transform.position = new Vector3(0, -0.59f, 0);
                    new_button.name = "WaitForEveryoneButton";
                    GameObject Labl = new_button.transform.GetChild(0).GetChild(0).gameObject;
                    UnityEngine.Object.Destroy(Labl.GetComponent<UILocalize>());
                    Labl.GetComponent<UILabel>().text = "WAIT FOR EVERYONE";
                    new_button.GetComponent<GenericButtonMouseSpawner>().onClick = null;
                    new_button.transform.GetChild(0).gameObject.GetComponent<UIButton>().onClick = null;
                }
            }
            if (GameManager.m_InterfaceManager != null && InterfaceManager.m_Panel_Inventory != null && InterfaceManager.m_Panel_Inventory.isActiveAndEnabled == true)
            {
                if(GiveItemLableObj == null)
                {
                    GameObject CloneTextDummy = InterfaceManager.m_Panel_Inventory.gameObject.transform.GetChild(3).GetChild(9).gameObject;
                    GiveItemLableObj = UnityEngine.Object.Instantiate(CloneTextDummy, InterfaceManager.m_Panel_Inventory.gameObject.transform.GetChild(3));
                    GiveItemLableObj.transform.position = new Vector3(-453, 300, 0);
                    GiveItemLable = GiveItemLableObj.GetComponent<UILabel>();
                    GiveItemLable.text = "SAS";
                }
            }

            if (iAmHost == true && IsCycleSkiping == true && GameManager.m_PlayerManager != null)
            {                
                if(GameManager.GetPlayerManagerComponent().PlayerIsSleeping() == false || IsDead == true)
                {
                    IsCycleSkiping = false;
                }
            }

            if(WaitForSleepLable != null && WaitForSleepLable.activeSelf == true && CanSleep(false) == false)
            {
                CanSleep(true);
                WaitForSleepLable.SetActive(false);
                if(SleepingButtons != null)
                {
                    SleepingButtons.SetActive(true);
                }
            }


            if (FirstBoot == false && SteamConnect.CanUseSteam == true)
            {
                SteamConnect.DoUpdate();
            }

            InitAllPlayers();

            if(iAmHost == true)
            {
                for (int i = 0; i < MaxPlayers; i++)
                {
                    if(i == 0)
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
            if (InOnline() == true)
            {
                FindPlayerToTrade();
                DoColisionForArrows();
            }
            if (ALWAYS_FUCKING_CURSOR_ON == true)
            {
                InputManager.m_CursorState = InputManager.CursorState.Show;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if (UiCanvas == null)
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
                }
            }else{
                if(GameManager.m_InterfaceManager != null && GameManager.m_InterfaceManager && uConsole.m_Instance != null && uConsole.m_On == false && Cursor.visible == false)
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
                }
                if(UIHostMenu != null)
                {
                    GameObject IsSteamHost = UIHostMenu.transform.GetChild(4).gameObject;
                    GameObject PublicSteamServer = UIHostMenu.transform.GetChild(5).gameObject;
                    if(IsSteamHost.GetComponent<UnityEngine.UI.Toggle>().isOn == true)
                    {
                        if(PublicSteamServer.activeSelf == false)
                        {
                            PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().Set(false);
                        }
                        PublicSteamServer.SetActive(true);
                    }else{
                        PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().Set(false);
                        PublicSteamServer.SetActive(false);
                    }
                }
            }

            //if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.P))
            //{
            //    Fire dummyFire = FireManager.m_Fires.get_Item(0);
            //    dummyFire.FireStateSet(FireState.FullBurn);
            //    dummyFire.m_HeatSource.TurnOn();
            //    dummyFire.m_FX.TriggerStage(FireState.FullBurn, true, true);
            //    dummyFire.m_FuelHeatIncrease = dummyFire.m_HeatSource.m_MaxTempIncrease;
            //    dummyFire.m_ElapsedOnTODSeconds = 0.0f;
            //    dummyFire.m_ElapsedOnTODSecondsUnmodified = 0.0f;
            //    dummyFire.ForceBurnTimeInMinutes(5);
            //    dummyFire.PlayFireLoop(100f);
            //    //EffectsControllerFire ecf = dummyFire.gameObject.GetComponent<EffectsControllerFire>();
            //    //dummyFire.FireStateSet(FireState.FullBurn);
            //    //dummyFire.ForceBurnTimeInMinutes(5);
            //    //dummyFire.m_FullBurnTriggered = true;
            //    //dummyFire.m_IsPerpetual = true;
            //    //ecf.Initialize();
            //}

            if (InOnline() == true)
            {
                //if (GameManager.m_InterfaceManager != null && InterfaceManager.m_Panel_PauseMenu != null && InterfaceManager.m_Panel_PauseMenu != null && InterfaceManager.m_Panel_PauseMenu.isActiveAndEnabled == true && SceneManager.IsLoading() == false && NotNeedToPauseUntilLoaded == false)
                //{
                //    GameManager.m_IsPaused = false;
                //}

                if (GameManager.m_Condition != null && GameManager.m_PlayerManager != null && GameManager.GetPlayerManagerComponent().PlayerIsDead())
                {
                    GameManager.m_Condition.DisableLowHealthEffects();
                }
                if(DoFakeGetup == true)
                {
                    BashGetupDelayCamera();
                }
            }

            if (Time.time > nextActionTimeSecond)
            {
                nextActionTimeSecond += periodSecond;
                MyMod.EverySecond();
            }

            if (InterfaceManager.m_Panel_BodyHarvest != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>() != null)
            {
                if(InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject.GetComponent<BaseAi>() != null)
                {
                    HarvestingAnimal = InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>().Get();
                }else{
                    HarvestingAnimal = "";
                }
            }else{
                HarvestingAnimal = "";
            }

            //if (GameManager.m_BodyCarry != null && GameManager.GetBodyCarryComponent().m_Body != null)
            //{
            //    GameObject HeldBody = GameManager.GetBodyCarryComponent().m_Body.gameObject;
            //    if (HeldBody.GetComponent<ObjectGuid>() != null && HeldBody.GetComponent<ObjectGuid>().Get() == PlayerBodyGUI)
            //    //if (HeldBody.GetComponent<ObjectGuid>() != null)
            //    {
            //        CarryingPlayer = true;
            //    }else{
            //        CarryingPlayer = false;
            //    }
            //}else{
            //    CarryingPlayer = false;
            //}

            //if(CarryingPlayer == true)
            //{
            //    if(playerbody != null && anotherbutt != null)
            //    {
            //        anotherplayer_levelid = levelid;
            //        LastRecivedOtherPlayerVector = playerbody.transform.position;
            //        anotherbutt.transform.position = playerbody.transform.position;
            //    }
            //}

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

            //if (Time.time > nextActionTimeAniamls)
            //{
            //    nextActionTimeAniamls += periodAniamls;
            //    if (levelid > 3 && AnimalsController == true)
            //    {
            //        for (int i = 0; i < AllAnimalsNew.Count; i++)
            //        {
            //            if (AllAnimalsNew.ElementAt<GameObject>(i) != null)
            //            {
            //                GameObject animal = AllAnimalsNew.ElementAt<GameObject>(i);
            //                if (animal.GetComponent<AnimalUpdates>() != null && IsShatalkerMode() == false)
            //                {
            //                    animal.GetComponent<AnimalUpdates>().CallSync(false);
            //                }
            //            }
            //        }
            //    }
            //}

            //if (IamShatalker == true && IsShatalkerMode() == true)
            //{
            //    GameManager.m_IceCrackingManager.enabled = false;
            //    if (ShatalkerObject != null)
            //    {
            //        ShatalkerObject.SetStartMovementDelayTime(OverridenStartCountDown);
            //    }

            //    if (anotherbutt != null)
            //    {
            //        if (DarkWalkerIsReady == false)
            //        {
            //            if (InWaitingRoom == false)
            //            {
            //                InWaitingRoom = true;
            //                SendToWaitngRoom();
            //            }
            //        } else {
            //            if (InWaitingRoom == true)
            //            {
            //                InWaitingRoom = false;
            //                ReturnFromWaitingRoom();
            //            }

            //            float dist = Vector3.Distance(GameManager.GetPlayerTransform().position, anotherbutt.transform.position);
            //            if (dist < 200 && WardIsActive == true)
            //            {
            //                if (GameManager.GetPlayerManagerComponent().m_ControlMode != PlayerControlMode.Locked)
            //                {
            //                    HUDMessage.AddMessage("You got too close to the survivor!");
            //                    GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);
            //                }
            //            } else {
            //                if (GameManager.GetPlayerManagerComponent().m_ControlMode == PlayerControlMode.Locked)
            //                {
            //                    GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Normal);
            //                }
            //            }
            //        }
            //    }
            //}

            if (level_name == "MainMenu")
            {
                Vector3 v3 = GameManager.GetMainCamera().transform.position;
                GameManager.GetMainCamera().transform.position = new Vector3(80.27f, 2.9f, 47.47f);

                ///MelonLogger.Msg("Camera X "+ v3.x  + " Y "+ v3.y + " Z "+ v3.z);

                if (MenuStuffSpawned == null)
                {
                    GameObject pl1 = MakeModObject("MenuPlayer1");
                    GameObject pl2 = MakeModObject("MenuPlayer2");
                    Animator pl1_anim = pl1.GetComponentInChildren<Animator>();
                    Animator pl2_anim = pl2.GetComponentInChildren<Animator>();
                    pl1_anim.Play("Menu1", 0);
                    pl2_anim.Play("Menu2", 0);
                    pl1.transform.position = new Vector3(77, 2.3f, 47);
                    pl2.transform.position = new Vector3(75, 2.3f, 49);
                    pl2.transform.rotation = new Quaternion(0, 0.773023f, 0, 0.634378f);
                    GameObject campfire = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("INTERACTIVE_CampFire").Cast<GameObject>());
                    campfire.transform.position = new Vector3(77, 2.3f, 48);
                    Campfire cf = campfire.GetComponent<Campfire>();

                    FlexFire = cf;
                    MenuErjan1 = pl1_anim;
                    MenuErjan2 = pl2_anim;
                    if (cf != null)
                    {
                        cf.m_Fire.FireStateSet(FireState.FullBurn);
                        cf.m_Fire.ForceBurnTimeInMinutes(50);
                    }
                    EffectsControllerFire ecf = campfire.GetComponent<EffectsControllerFire>();

                    ecf.Initialize();
                    ecf.TriggerStage(FireState.FullBurn, true);

                    MenuStuffSpawned = pl1;
                }
            }

            if (levelid > 3)
            {

                if (IamShatalker == true)
                {
                    GameManager.GetVpFPSPlayer().Controller.MotorVelocityMax = DarkWalkerSpeed;
                    GameManager.GetVpFPSPlayer().Controller.MotorVelocityMin = DarkWalkerSpeed;
                }

                if (GameManager.GetPlayerManagerComponent().m_InteractiveObjectUnderCrosshair != null)
                {
                    LastObjectUnderCrosshair = GameManager.GetPlayerManagerComponent().m_InteractiveObjectUnderCrosshair;
                }

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
                    }
                    else {
                        DebugAnimalGUID = "Have not objectGuid";
                        DebugAnimalGUIDLast = DebugAnimalGUID;
                        DebugLastAnimal = hit.collider.gameObject;
                    }
                }
                else {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                    DebugAnimalGUID = "";
                }

                if (InOnline() == true && IsShatalkerMode() == false)
                {
                    if (Utils.IsZero(GameManager.GetConditionComponent().m_CurrentHP)) // Make fake death.
                    {
                        IsDead = true;

                        //if (IsCarringMe == true && levelid == anotherplayer_levelid && anotherbutt != null && GameManager.m_PlayerManager != null)
                        //{
                        //    GameManager.GetPlayerManagerComponent().TeleportPlayer(anotherbutt.transform.position, anotherbutt.transform.rotation);
                        //}
                    }else{
                        IsDead = false;
                    }
                }

                MyHasRifle = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_Rifle");

                if (PrviousHasRifle != MyHasRifle)
                {
                    PrviousHasRifle = MyHasRifle;
                    SendMyEQ();
                }

                MyHasRevolver = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_Revolver");

                bool axe = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_Hatchet"); // Axe
                bool f_axe = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_FireAxe"); // Fire axe
                bool h_axe = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_HatchetImprovised"); // Handmade axe

                if (axe == true || f_axe == true || h_axe) // If have any axe
                {
                    MyHasAxe = true;
                }else{
                    MyHasAxe = false;
                }

                MyHasMedkit = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_MedicalSupplies_hangar");

                if (PreviousHasMedkit != MyHasMedkit)
                {
                    PreviousHasMedkit = MyHasMedkit;
                    SendMyEQ();
                }


                MyArrows = GameManager.GetInventoryComponent().NumGearInInventory("GEAR_Arrow");
                MyFlares = GameManager.GetInventoryComponent().NumGearInInventory("GEAR_FlareA");
                MyBlueFlares = GameManager.GetInventoryComponent().NumGearInInventory("GEAR_FlareBlue");

                if (PrviousHasRevolver != MyHasRevolver)
                {
                    PrviousHasRevolver = MyHasRevolver;
                    SendMyEQ();
                }
                if (PreviousHasAxe != MyHasAxe)
                {
                    PreviousHasAxe = MyHasAxe;
                    SendMyEQ();
                }
                if (PreviousArrows != MyArrows)
                {
                    PreviousArrows = MyArrows;
                    SendMyEQ();
                }
                if(PreviousFlares != MyFlares)
                {
                    PreviousFlares = MyFlares;
                    SendMyEQ();
                }
                if(PreviousBlueFlares != MyBlueFlares)
                {
                    PreviousBlueFlares = MyBlueFlares;
                    SendMyEQ();
                }

                if (InterfaceManager.m_Panel_Map.IsEnabled() == false)
                {
                    if (GameManager.GetPlayerManagerComponent().m_ItemInHands != null)
                    {
                        MyLightSourceName = GameManager.GetPlayerManagerComponent().m_ItemInHands.name;
                    } else
                    {
                        MyLightSourceName = "";
                    }
                } else
                {
                    MyLightSourceName = "Map";
                }

                if(WaitForSleepLable != null && WaitForSleepLable.activeSelf == true)
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
                            //V3BeforeSleep = GameManager.GetPlayerTransform().position;
                            //NeedV3BeforeSleep = true;
                            //GameManager.GetPlayerTransform().position = bed.gameObject.transform.position;
                            //GameManager.GetPlayerTransform().rotation = bed.gameObject.transform.rotation;
                        } else {
                            NeedV3BeforeSleep = false;
                        }
                    } else {
                        MyCycleSkip = 0;
                        MelonLogger.Msg("Has wakeup or cancle sleep");
                        //if (NeedV3BeforeSleep == true)
                        //{
                        //    GameManager.GetPlayerTransform().position = V3BeforeSleep;
                        //}
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

            if (iAmHost == true && NeedSyncTime == true)
            {
                if (Time.time > nextActionTime)
                {
                    nextActionTime += period;
                    MyMod.EveryInGameMinute();
                }
            }

            if (MyPreviousAnimState != MyAnimState)
            {
                MyPreviousAnimState = MyAnimState;
                if(playersData[0].m_Mimic == true && players[0] != null)
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
            if (MyLightSourceName != MyLastLightSourceName)
            {
                MyLastLightSourceName = MyLightSourceName;
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

            if (RealTimeCycleSpeed == true && InOnline() == true)
            {
                uConsole.RunCommandSilent("set_time " + OveridedTime);
            }

            //if (IamShatalker == true || ((ShatalkerModeClient == true || ServerHandle.DarkShatalkerMode == true) && levelid != anotherplayer_levelid))
            //{
            //    Vector3 shatalkerV3 = new Vector3(-1000, -1000, -1000);
            //    if (ShatalkerObject != null)
            //    {
            //        ShatalkerObject.m_WorldPosition = shatalkerV3;
            //    }
            //}
            //if (sendMyPosition == true && ShatalkerModeClient == true && levelid == anotherplayer_levelid)
            //{
            //    if (ShatalkerObject != null)
            //    {
            //        ShatalkerObject.m_WorldPosition = LastRecivedShatalkerVector;
            //    }
            //}

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

                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.L))
                {
                    if (ALWAYS_FUCKING_CURSOR_ON == false)
                    {
                        ALWAYS_FUCKING_CURSOR_ON = true;
                    }else{
                        ALWAYS_FUCKING_CURSOR_ON = false;
                    }
                }
                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.LeftAlt))
                {
                    GameManager.GetVpFPSPlayer().Controller.Jump();
                }
            }

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.G))
            {
                if (LastSelectedGearName != "" && LastSelectedGear != null && InterfaceManager.m_Panel_Inventory.IsEnabled() == true && GiveItemTo != -1)
                {
                    Il2CppSystem.Collections.Generic.List<GearItemObject> items = GameManager.GetInventoryComponent().m_Items;
                    GearItem _gear = null;
                    string saveProxyData = "";

                    for (int i = 0; i < items.Count; i++)
                    {
                        GearItem gear_ = items.get_Item(i).m_GearItem;

                        if (gear_ == LastSelectedGear)
                        {
                            _gear = items.get_Item(i).m_GearItem;
                            //saveProxyData = _gear.Serialize();
                            GameObject cloneObj = UnityEngine.Object.Instantiate(_gear.gameObject);
                            GearItem cloneGear = cloneObj.GetComponent<GearItem>();
                            if(cloneGear.m_StackableItem != null)
                            {
                                cloneGear.m_StackableItem.m_Units = 1;
                            }
                            cloneGear.m_InPlayerInventory = false;
                            saveProxyData = JsonNullsRemover.RemoveJsonNulls(cloneGear.Serialize());
                            UnityEngine.Object.DestroyImmediate(cloneObj);
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

                        if(_gear.m_ClothingItem != null && _gear.m_ClothingItem.IsWearing())
                        {
                            _gear.m_ClothingItem.TakeOff();
                        }
                        if (GameManager.GetPlayerManagerComponent().m_ItemInHands == _gear)
                        {
                            GameManager.GetPlayerManagerComponent().UnequipItemInHandsSkipAnimation();
                        }

                        WaterSupply bottle = null;

                        if(LastSelectedGearName == "GEAR_WaterSupplyPotable" || LastSelectedGearName == "GEAR_WaterSupplyNotPotable")
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

                                if(_packet.Length() >= 1500 && ConnectedSteamWorks == false)
                                {
                                    MelonLogger.Msg(ConsoleColor.Red, "Can't transfer item to other player " + GearDataPak.m_GearName + " json is too large, this is not fits to MTU limit. Used bytes: "+ _packet.Length() + "/1500");
                                    MelonLogger.Msg(ConsoleColor.Red, "Please make screenshot of this error message and send it to #discussions on Sky Co-op Discord server with ping @Filigrani");
                                    MelonLogger.Msg(ConsoleColor.Blue, "Data proxy JSON string: "+ GearDataPak.m_DataProxy);
                                    return;
                                }
                                SendTCPData(_packet);
                            }
                        }
                        if (waterMode == true)
                        {
                            string say = "half liter of " + _gear.m_LocalizedDisplayName.Text();

                            if(bottle.m_VolumeInLiters == waterGave)
                            {
                                bottle.m_VolumeInLiters = 0;
                            }else{
                                bottle.m_VolumeInLiters = bottle.m_VolumeInLiters - waterGave;
                                say = waterGave+" liter of " + _gear.m_LocalizedDisplayName.Text();
                            }
                            HUDMessage.AddMessage("You gave " + say + " to " + playersData[GiveItemTo].m_Name);
                        }else{
                            HUDMessage.AddMessage("You gave " + _gear.m_LocalizedDisplayName.Text() + " to " + playersData[GiveItemTo].m_Name);
                            GameManager.GetInventoryComponent().RemoveUnits(_gear, 1);
                        }
                        MelonLogger.Msg("You gave " + LastSelectedGearName + " to " + playersData[GiveItemTo].m_Name);
                        NeedRefreshInv = true; // Reloading items list.
                    }
                }
            }

            ShiftPressed = InputManager.GetSprintDown(InputManager.m_CurrentContext);

            Transform target = null;

            if (levelid > 3) // Trying get player's position only if not at menu.
            {
                if(GameManager.GetFatigueComponent().GetHeavyBreathingState() == HeavyBreathingState.Heavy)
                {
                    NowHeavyBreath = true;
                }else{
                    NowHeavyBreath = false;
                }

                //if(GameManager.m_PlayerManager != null && GameManager.m_PlayerManager.m_ContainerBeingSearched != null)
                //{
                //    ContainerOpenSync pendingContainer = new ContainerOpenSync();
                //    pendingContainer.m_LevelID = levelid;
                //    pendingContainer.m_LevelGUID = level_guid;

                //    if(GameManager.m_PlayerManager.m_ContainerBeingSearched.gameObject != null)
                //    {
                //        GameObject contObj = GameManager.m_PlayerManager.m_ContainerBeingSearched.gameObject;
                //        if(contObj.GetComponent<ObjectGuid>() != null)
                //        {
                //            pendingContainer.m_Guid = contObj.GetComponent<ObjectGuid>().Get();
                //        }
                //    }

                //    if(MyContainer == null || MyContainer.Equals(pendingContainer) == false)
                //    {
                //        MyContainer = pendingContainer;
                //        MelonLogger.Msg("Interacting wtih container");
                //    }
                //}else{
                //    if(MyContainer != null)
                //    {
                //        MyContainer = null;
                //        MelonLogger.Msg("Stop interacting wtih container");
                //    }
                //}

                BloodLosts = GameManager.GetBloodLossComponent().GetAfflictionsCount();

                if(PreviousBloodLosts != BloodLosts)
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

                HasInfecitonRisk = GameManager.GetInfectionRiskComponent().HasInfectionRisk();

                if(PreviousHasInfectionRisk != HasInfecitonRisk)
                {
                    PreviousHasInfectionRisk = HasInfecitonRisk;
                    if (sendMyPosition == true) // CLIENT
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.INFECTIONSRISK))
                        {
                            _packet.Write(HasInfecitonRisk);
                            SendTCPData(_packet);
                        }
                    }
                    if (iAmHost == true) // HOST
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.INFECTIONSRISK))
                        {
                            ServerSend.INFECTIONSRISK(0, HasInfecitonRisk, true);
                        }
                    }
                }

                if(PreviousNowHeavyBreath != NowHeavyBreath)
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

                target = GameManager.GetPlayerTransform();
                //SYNC MOVEMENT
                bool InFight = GameManager.GetPlayerStruggleComponent().InStruggle();

                if (target != null && previoustickpos != target.position) // If position changed.
                {

                    previoustickpos = GameManager.GetPlayerTransform().position;

                    if(IsDead == false && KillAfterLoad == false && InFight == false)
                    {
                        if(GameManager.GetPlayerManagerComponent().PlayerIsClimbing() == false)
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
                        if (IamShatalker == false)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.XYZ))
                            {
                                _packet.Write(target.position);
                                SendTCPData(_packet);
                            }
                        }
                        if (IamShatalker == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.XYZDW))
                            {
                                _packet.Write(target.position);
                                SendTCPData(_packet);
                            }
                        }
                    }
                    if (iAmHost == true) // HOST
                    {
                        if (IamShatalker == false)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.XYZ))
                            {
                                ServerSend.XYZ(0, target.position, true);
                            }
                        }
                        if (IamShatalker == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.XYZDW))
                            {
                                ServerSend.XYZDW(1, target.position);
                            }
                        }
                    }
                }
                else
                {
                    if (IsDead == true || KillAfterLoad == true || InFight == true)
                    {
                        if(IsDead == true || KillAfterLoad == true)
                        {
                            MyAnimState = "Knock";
                        }else{
                            MyAnimState = "Fight";
                        }
                    }else{
                        
                        if(GameManager.GetPlayerManagerComponent().PlayerIsClimbing() == false)
                        {
                            if (GameManager.GetPlayerManagerComponent().PlayerIsSleeping() == true)
                            {
                                MyAnimState = "Sleep";
                            }else{
                                if (GameManager.GetPlayerInVehicle().m_InVehicle == true)
                                {
                                    MyAnimState = "Sit";
                                }else{
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
                                                } else {
                                                    MyAnimState = "Harvesting";
                                                }
                                            } else {
                                                MyAnimState = "Harvesting";
                                            }
                                        }else if(PlayerInteractionWith != null)  //GameManager.GetPlayerManagerComponent().m_HarvestableInProgress 
                                        { 
                                            if (GameManager.GetPlayerManagerComponent().PlayerIsCrouched() == true)
                                            {
                                                MyAnimState = "Harvesting";
                                            }else{
                                                MyAnimState = "HarvestingStanding";
                                            }
                                        }else if (InterfaceManager.m_Panel_BodyHarvest != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject != null)
                                        {
                                            MyAnimState = "Harvesting";
                                        }else if ((InterfaceManager.m_Panel_SnowShelterBuild != null && InterfaceManager.m_Panel_SnowShelterBuild.m_IsBuilding == true) || (InterfaceManager.m_Panel_SnowShelterInteract != null && InterfaceManager.m_Panel_SnowShelterInteract.m_IsDismantling == true))
                                        {
                                            MyAnimState = "Harvesting";
                                        }else if (GameManager.GetPlayerManagerComponent().m_HarvestableInProgress != null)
                                        {
                                            if (GameManager.GetPlayerManagerComponent().PlayerIsCrouched() == true)
                                            {
                                                MyAnimState = "Harvesting";
                                            }else{
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
                                                }else if (Plc == PlayerControlMode.DeployRope || Plc == PlayerControlMode.TakeRope)
                                                {
                                                    MyAnimState = "Harvesting";
                                                }else{
                                                    MyAnimState = "Ctrl";
                                                }
                                            }else{
                                                if (Plc == PlayerControlMode.AimRevolver)
                                                {
                                                    MyAnimState = "HoldGun";
                                                }else if (Plc == PlayerControlMode.DeployRope || Plc == PlayerControlMode.TakeRope)
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
            }

            //SYNC ROTATION
            if (target != null && previoustickrot != GameManager.GetPlayerTransform().rotation) // If rotation changed.
            {
                previoustickrot = GameManager.GetPlayerTransform().rotation;
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

            //if (iAmHost == true) // SYNC OTHER PLAYER
            //{
            //    WardIsActive = false;
            //    OverridenStartCountDown = 0;
            //    if (OverridenStartCountDown < 2)
            //    {
            //        OverridenStartCountDown = 0;
            //        ShatalkerObject.SetStartMovementDelayTime(OverridenStartCountDown);
            //    }

            //    if (ServerHandle.NeedReloadDWReadtState == true)
            //    {
            //        ServerHandle.NeedReloadDWReadtState = false;
            //        if (iAmHost == true) // HOST
            //        {
            //            using (Packet _packet = new Packet((int)ServerPackets.DARKWALKERREADY))
            //            {
            //                ServerSend.DARKWALKERREADY(1, DarkWalkerIsReady);
            //            }
            //        }
            //    }

            //    if (IamShatalker == true)
            //    {
            //        DarkWalkerIsReady = false;
            //        if (DarkWalkerIsReady == true)
            //        {
            //            ServerHandle.LastCountDown = 0;
            //            OverridenStartCountDown = 0;
            //        }
            //    }
            //}
            //if (anotherbutt != null)
            //{

            //    if (PlayerBodyGUI != "")
            //    {
            //        GameObject findbody = ObjectGuidManager.Lookup(PlayerBodyGUI);
            //        if (findbody != null)
            //        {
            //            findbody.GetComponent<NPC>().m_DisplayName.m_LocalizationID = "Body";
            //            findbody.transform.position = LastRecivedOtherPlayerVector;
            //            findbody.GetComponent<NPCCondition>().m_CurrentHP = 1;
            //            findbody.GetComponent<NPCCondition>().enabled = false;
            //            findbody.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            //            if(playerbody == null)
            //            {
            //                playerbody = findbody;
            //            }
            //        }
            //    }
            //    if (IsShatalkerMode() == true)
            //    {
            //        if (ShatalkerObject == null)
            //        {
            //            ShatalkerObject = Resources.FindObjectsOfTypeAll<InvisibleEntityManager>().First();
            //            MelonLogger.Msg("Darkwalker object has been restored");
            //        }
            //        if (DarkWalkerHUD == null)
            //        {
            //            DarkWalkerHUD = Resources.FindObjectsOfTypeAll<HUDNowhereToHide>().First();
            //            MelonLogger.Msg("Got DarkWalker HUD");
            //        }
            //        else
            //        {
            //            if (IamShatalker == false)
            //            {
            //                if (WardWidget == null)
            //                {
            //                    WardWidget = DarkWalkerHUD.transform.GetChild(3).gameObject;
            //                    MelonLogger.Msg("Got WardWidget object");
            //                }
            //                if(LureWidget == null)
            //                {
            //                    LureWidget = DarkWalkerHUD.transform.GetChild(2).gameObject;
            //                }

            //                if (ShatalkerObject.GetStartMovementDelayTime() < 2)
            //                {
            //                    DarkWalkerIsReady = true;
            //                }
            //                else
            //                {
            //                    DarkWalkerIsReady = false;
            //                }
            //                if (DarkWalkerIsReady != PreviousDarkWalkerReady)
            //                {
            //                    PreviousDarkWalkerReady = DarkWalkerIsReady;
            //                    MelonLogger.Msg("Sending to Darkwalker my ready state");
            //                    if (sendMyPosition == true) // CLIENT
            //                    {
            //                        using (Packet _packet = new Packet((int)ClientPackets.DARKWALKERREADY))
            //                        {
            //                            _packet.Write(DarkWalkerIsReady);
            //                            SendTCPData(_packet);
            //                        }
            //                    }
            //                    if (iAmHost == true) // HOST
            //                    {
            //                        using (Packet _packet = new Packet((int)ServerPackets.DARKWALKERREADY))
            //                        {
            //                            ServerSend.DARKWALKERREADY(1, DarkWalkerIsReady);
            //                        }
            //                    }
            //                }
            //            }
            //            if (IamShatalker == true)
            //            {
            //                if (DistanceWidget == null || DistanceLable == null)
            //                {
            //                    DistanceWidget = DarkWalkerHUD.transform.GetChild(1).gameObject;

            //                    UnityEngine.Component.Destroy(DarkWalkerHUD.transform.GetChild(1).GetChild(0).gameObject.GetComponent<UILocalize>());
            //                    DarkWalkerHUD.transform.GetChild(1).GetChild(0).gameObject.GetComponent<UILabel>().text = "SURVIVOR DISTANCE";
            //                    DistanceLable = DarkWalkerHUD.transform.GetChild(1).GetChild(1).gameObject.GetComponent<UILabel>();
            //                    MelonLogger.Msg("Got DistanceWidget object");
            //                } else
            //                {
            //                    if (DarkWalkerIsReady == true)
            //                    {
            //                        DistanceWidget.SetActive(true);
            //                        DarkWalkerHUD.transform.GetChild(0).gameObject.SetActive(false);
            //                        DarkWalkerHUD.enabled = false;
            //                        DarkWalkerHUD.transform.GetChild(0).gameObject.SetActive(false);
            //                    }
            //                    if (anotherbutt != null)
            //                    {
            //                        float dist = 0;
            //                        bool HavePossition = false;

            //                        if (levelid == anotherplayer_levelid)
            //                        {
            //                            dist = Vector3.Distance(GameManager.GetPlayerTransform().position, anotherbutt.transform.position);
            //                            HavePossition = true;
            //                        }else{
            //                            for (int i = 0; i < SurvivorWalks.Count; i++)
            //                            {
            //                                if (SurvivorWalks[i].m_levelid == levelid)
            //                                {
            //                                    HavePossition = true;
            //                                    dist = Vector3.Distance(GameManager.GetPlayerTransform().position, SurvivorWalks[i].m_V3);
            //                                    break;
            //                                }
            //                            }
            //                        }

            //                        if(HavePossition == true)
            //                        {
            //                            if(LureIsActive && LastLure.m_levelid == levelid)
            //                            {
            //                                dist = Vector3.Distance(GameManager.GetPlayerTransform().position, LastLure.m_V3);
            //                            }
            //                            DistanceLable.text = Convert.ToInt32(dist) + " METERS";
            //                        }else{
            //                            DistanceLable.text = "SURVIVOR NEVER WAS HERE";
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            if(MenuErjan1 != null && MenuErjan2 != null && FlexFire != null)
            {
                if(FlexFire.m_Fire.GetRemainingLifeTimeSeconds() <= 0)
                {
                    int currentTagHash = MenuErjan1.GetCurrentAnimatorStateInfo(0).tagHash;
                    int neededTagHash = Animator.StringToHash("Samba Dancing");

                    if(currentTagHash != neededTagHash)
                    {
                        MenuErjan1.Play("Samba Dancing",0);
                        MenuErjan2.Play("Samba Dancing",0);
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
            PendingConnectionIp = _ip;
            instance.ip = _ip;
            instance.ConnectToServer();
            DoWaitForConnect();
        }


        public static bool AtHostMenu = false;

        public static void HostAServer()
        {
            if (iAmHost != true)
            {
                isRuning = true;

                Thread mainThread = new Thread(new ThreadStart(MainThread));
                Server.Start(MaxPlayers, 26950);
                nextActionTime = Time.time;
                nextActionTimeAniamls = Time.time;
                iAmHost = true;
                MelonLogger.Msg("Server has been runned with InGame time: " + GameManager.GetTimeOfDayComponent().GetHour() + ":" + GameManager.GetTimeOfDayComponent().GetMinutes() + " seed "+ GameManager.m_SceneTransitionData.m_GameRandomSeed);
                OverridedHourse = GameManager.GetTimeOfDayComponent().GetHour();
                OverridedMinutes = GameManager.GetTimeOfDayComponent().GetMinutes();
                OveridedTime = OverridedHourse + ":" + OverridedMinutes;
                NeedSyncTime = true;
                RealTimeCycleSpeed = true;
            }
            else
            {
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

            if (level_name != "Empty" && level_name != "Boot" && level_name != "MainMenu" && GameManager.m_InterfaceManager != null && InterfaceManager.m_Panel_PauseMenu != null && InterfaceManager.m_Panel_PauseMenu.IsEnabled() == true)
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
            if(health == true)
            {
                GameManager.GetConditionComponent().m_CurrentHP = 25;
            }
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
            if(playersData[_from] != null)
            {
                playersData[_from] = new MultiPlayerClientData();
            }
            ServerSend.EQUIPMENT(_from, playersData[_from].m_PlayerEquipmentData, false);
            ServerSend.LIGHTSOURCE(_from, false, false);
            ServerSend.LIGHTSOURCENAME(_from, playersData[_from].m_PlayerEquipmentData.m_HoldingItem, false);
            ServerSend.XYZ(_from, new Vector3(0,0,0), false);
            ServerSend.XYZW(_from, new Quaternion(0,0,0,0), false);
            ServerSend.ANIMSTATE(_from, "Idle", false);
            ServerSend.LEVELID(_from, 0, false);
        }

        public static void SendSpawnData(bool AskResponce = false)
        {
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
            if(name.Contains("Scarf") || name.Contains("GEAR_Balaclava"))
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

                GameObject PublicSteamServer = UIHostMenu.transform.GetChild(5).gameObject;
                bool IsPub = PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().isOn;

                IsPublicServer = IsPub;
                ServerConfig.m_DuppedSpawns = DupesIsChecked;
                ServerConfig.m_DuppedContainers = BoxDupesIsChecked;
                ServerConfig.m_PlayersSpawnType = spawnStyle;
                MaxPlayers = slotsMax;

                if (ShouldUseSteam == false)
                {
                    HostAServer();
                }else{
                    Server.StartSteam(MaxPlayers);
                }

                UnityEngine.Object.Destroy(UIHostMenu);
            }
        }

        public static void HostMenu()
        {
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

                GameObject PublicSteamServer = UIHostMenu.transform.GetChild(5).gameObject;

                if (SteamConnect.CanUseSteam == false)
                {
                    GameObject IsSteamHost = UIHostMenu.transform.GetChild(4).gameObject;
                    IsSteamHost.GetComponent<UnityEngine.UI.Toggle>().Set(false);
                    IsSteamHost.SetActive(false);

                    
                    PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().Set(false);
                    PublicSteamServer.SetActive(false);
                }
                PublicSteamServer.GetComponent<UnityEngine.UI.Toggle>().Set(false);
            }
        }


        public static void UpdateMyClothing()
        {
            string m_Hat = "";
            string m_Top = "";
            string m_Bottom = "";
            string m_Boots = "";
            string m_Scarf = "";
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

            if (IsScarf(Hat1))
            {
                m_Scarf = Hat1;
                m_Hat = Hat2;
            }
            else if (IsScarf(Hat2))
            {
                m_Scarf = Hat2;
                m_Hat = Hat1;
            }
            else{
                m_Hat = GetClothToDisplay(Hat1, Hat2);
            }

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
    }
}
