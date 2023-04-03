using System;
using System.Collections.Generic;
#if (DEDICATED)
using System.Numerics;
#else
using MelonLoader;
using UnityEngine;
using Il2Cpp;
#endif

namespace SkyCoop
{
    public class DataStr
    {
        public class ServerConfigData
        {
            public bool m_FastConsumption = true;
            public bool m_DuppedSpawns = false;
            public bool m_DuppedContainers = false;
            public int m_PlayersSpawnType = 0;
            public int m_FireSync = 2;
            public int m_CheatsMode = 2;
            public bool m_CheckModsValidation = false;
            public bool m_SaveScamProtection = false;
            public bool m_PVP = true;
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
            public long m_CheckHash = 0;
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
            public string m_PhotoGUID = "";
            public string m_ExpeditionNote = "";
        }
        public class AnimalArrow
        {
            public float m_Condition = 100;
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Vector3 m_Angle = new Vector3(0, 0, 0);
            public float m_Depth = 0;
            public string m_LocaName = "";
        }

        #if (!DEDICATED)


        public class AnimalsSpawnsShort
        {
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public SpawnRegion m_SPR = null;
            public string m_GUID = "";

            public AnimalsSpawnsShort(SpawnRegion spR)
            {
                m_Position = spR.m_Center;
                m_SPR = spR;
                m_GUID = spR.GetComponent<ObjectGuid>().Get();
            }
        }

        #endif
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
            public int m_ArrowsCount = 0;
            public List<AnimalArrow> m_Arrows = new List<AnimalArrow>();
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
        public class AnimalAligner 
        {
            public string m_Proxy = "";
            public string m_Guid = "";
        }
        public class AnimalTrigger
        {
            public string m_Guid = "";
            public int m_Trigger = 0;
        }
        public class PlayerEquipmentData 
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
        public class PlayerClothingData
        {
            public string m_Hat = "";
            public string m_Top = "";
            public string m_Bottom = "";
            public string m_Boots = "";
            public string m_Scarf = "";
            public string m_Balaclava = "";
        }
        public class MultiPlayerClientData
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
            public Supporters.SupporterBenefits m_SupporterBenefits = new Supporters.SupporterBenefits();
            public bool m_IsLoading = false;
            public int m_LastWeatherRegion = (int) Shared.GameRegion.RandomRegion;
            public int m_LastRegion = (int)Shared.GameRegion.RandomRegion;

            public bool m_IsSafe = false;
            public string m_SteamOrEGSID = "";
        }
        public class MultiPlayerClientStatus //: MelonMod
        {
            public int m_ID = 0;
            public string m_Name = "";
            public bool m_Sleep = false;
            public bool m_Dead = false;
            public bool m_IsLoading = false;
        }
        public class PriorityActionForOtherPlayer
        {
            public string m_Action = "";
            public string m_DisplayText = "";
            public string m_ProcessText = "";
            public bool m_CancleOnMove = false;
            public float m_ActionDuration = 3;
            public bool m_Hold = true;
            public bool m_EscCancle = false;
        }
        public class AffictionSync
        {
            public int m_Type = 0;
            public int m_Location = 0;
            public string m_Case = "";
            public bool m_ShouldBeTreated = true;
        }

        public class DeathContainerData
        {
            public string m_Guid = "";
            public string m_LevelKey = "";
            public string m_Owner = "";
            public string m_ContainerPrefab = "CONTAINER_BackPack";
            public Vector3 m_Position = new Vector3(0,0,0);
            public Quaternion m_Rotation = new Quaternion(0,0,0,0);
            public bool Equals(DeathContainerData other)
            {
                if (other == null)
                    return false;

                if (m_Guid == other.m_Guid)
                {
                    return true;
                }
                else
                {
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
            public bool m_lookat = false;
            public string m_sceneguid = "";
        }
        public class HarvestStats //: MelonMod
        {
            public float m_Meat = 0;
            public int m_Guts = 0;
            public int m_Hide = 0;
            public string m_Guid = "";
        }
        public class WeatherProxies 
        {
            public string m_WeatherProxy = "";
            public string m_WeatherTransitionProxy = "";
            public string m_WindProxy = "";
        }
        public class SaveSlotSync
        {
            public int m_Episode = 0;
            public int m_SaveSlotType = 3;
            public int m_Seed = 0;
            public int m_ExperienceMode = 2;
            public int m_Location = 0;
            public string m_FixedSpawnScene = "";
            public Vector3 m_FixedSpawnPosition = new Vector3(0,0,0);
            public string m_CustomExperienceStr = "";
        }
        public class MultiplayerChatMessage
        {
            public int m_Type = 0;
            public string m_Message = "";
            public string m_By = "";
            public bool m_Global = true;
            public bool m_Private = false;
#if (!DEDICATED)
            public UnityEngine.UI.Text m_TextObj = null;
#endif
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
            public bool m_Broken = true;

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
                }
                else
                {
                    return false;
                }
            }
        }
        public class PickedGearSync : IEquatable<PickedGearSync>
        {
            public Vector3 m_Spawn = new Vector3(0, 0, 0);
            public int m_LevelID = 0;
            public string m_LevelGUID = "";
            public int m_PickerID = 0;
            public int m_Recently = 0;
            public int m_MyInstanceID = 0;
            public string m_GearName = "";

            public bool Equals(PickedGearSync other)
            {
                if (other == null)
                    return false;

                if (this.m_LevelGUID == other.m_LevelGUID && this.m_Spawn == other.m_Spawn)
                {
                    return true;
                }
                else
                {
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
                }
                else
                {
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
                }
                else
                {
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
        public class DictionaryElementToReNew
        {
            public int m_Key;
            public SlicedJsonDroppedGear m_Val;
            public DroppedGearItemDataPacket m_Val2;
        }
        public class ElementToModfy
        {
            public string m_GUID;
            public int m_Time;
        }
        public class BodyHarvestUnits
        {
            public float m_Meat = 0;
            public int m_Guts = 0;
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
        public class MultiplayerEmote
        {
            public string m_Name = "Anim";
            public string m_Animation = "Idle";
            public bool m_ForceCrouch = false;
            public bool m_ForceStandup = false;
            public bool m_FollowDollCamera = false;
            public bool m_NeedFreeHands = false;
            public bool m_LeftHandEmote = false;
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
        public class DoorKey
        {
            public string m_Seed = "";
            public string m_Name = "";
            public bool m_Lead = false;

            public DoorKey(string Seed, string Name, bool Lead = false)
            {
                m_Seed = Seed;
                m_Name = Name;
                m_Lead = Lead;
            }
        }
        public class VoiceChatQueueElement
        {
            public byte[] m_VoiceData;
            public float m_Length = 0;
        }
        public class DedicatedServerData
        {
            public string SaveSlot = "UnspecifiedSave";
            public bool ItemDupes = false;
            public bool ContainersDupes = false;
            public int SpawnStyle = 0;
            public int MaxPlayers = 2;
            public bool UsingSteam = false;
            public int Ports = 26950;
            public string[] WhiteList;
            public string ServerName = "";
            public int Cheats = 2;
            public int SteamServerAccessibility = 2;
            public string RCON = "12345";
            public int DropUnloadPeriod = 5;
            public bool SaveScamProtection = false;
            public bool ModValidationCheck = false;
            public int ExperienceMode = 0;
            public int StartRegion = 0;
            public int Seed = 0;
            public bool PVP = false;
            public int SavingPeriod = 60;
            public int RestartPerioud = 10800;
        }
        public enum TimeOfDayStatus
        {
            NightEndToDawn,
            DawnToMorning,
            MorningToMidday,
            MiddayToAfternoon,
            AfternoonToDusk,
            DuskToNightStart,
            NightStartToNightEnd,
        }
        public class WeatherVolunteerData
        {
            public int WeatherType = 0;
            public float WeatherDuration = 1f;
            public int CurrentRegion = 0;
            public List<float> StageDuration = new List<float>();
            public List<float> StageTransition = new List<float>();
            public int SetIndex = 0;
            public float HighMin = 0f;
            public float HighMax = 0f;
            public  float LowMin = 0f;
            public float LowMax = 0f;
            public int CoolingHours = 0;
            public int WarmingHours = 0;
            public int PreviousStage = 10;
        }

        public enum SlicedBase64Purpose
        {
            Photo,
        }


        public class SlicedBase64Data
        {
            public string m_Slice = "";
            public int m_Slices = 0;
            public int m_SliceNum = 0;
            public long m_CheckSum = 0;
            public string m_GUID = "";
            public int m_Purpose = 0;
        }
        public class FakeRockCacheVisualData
        {
            public string m_Owner = "";
            public string m_LevelGUID = "";
            public string m_GUID = "";
            public Vector3 m_Position = new Vector3(0,0,0);
            public Quaternion m_Rotation = new Quaternion(0, 0, 0, 0);
        }

        public class UniversalSyncableObject
        {
            public string m_Prefab = "";
            public string m_Scene = "";
            public string m_GUID = "";
            public Vector3 m_Position = new Vector3(0,0,0);
            public Quaternion m_Rotation = new Quaternion(0,0,0,0);
            public int m_CreationTime = 0;
            public int m_RemoveTime = 0;
            public string m_ExpeditionBelong = "";
        }
        public class UniversalSyncableObjectSpawner
        {
            public string m_Prefab = "";
            public string m_GUID = "";
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Quaternion m_Rotation = new Quaternion(0, 0, 0, 0);
            public string m_Content = "";
        }

        public class Vector2Int
        {
            public int X = 0;
            public int Y = 0;
            public Vector2Int(int x, int y)
            {
                X = x; 
                Y = y;
            }
        }
        public class WebhookSettings
        {
            public string Name = "";
            public string URL = "";
        }

        public class ExpeditionInteractiveData
        {
            public Vector3 m_Position = new Vector3(0,0,0);
            public Quaternion m_Rotation = new Quaternion(0,0,0,0);
            public Vector3 m_Scale = new Vector3(1,1,1);
            public string m_ObjectText = "Object";
            public string m_InteractText = "Interacting...";
            public float m_InteractTime = 1f;
            public string m_Tool = "";
            public string m_Material = "";
            public int m_MaterialCount = 1;
            public string m_GUID = "";
        }
    }
}
