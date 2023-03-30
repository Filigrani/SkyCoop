using System;
using System.Collections.Generic;
using System.Text;
using SkyCoop;
using static SkyCoop.DataStr;
#if (DEDICATED)
using System.Numerics;
#else
using MelonLoader;
using UnityEngine;
#endif



namespace GameServer
{
    /// <summary>Sent from server to client.</summary>
    public enum ServerPackets
    {
        welcome = 1,
        XYZ,
        XYZW,
        BLOCK,
        XYZDW, // XYZ SYNC IN DARK WALKER MODE
        LEVELID,
        GOTITEM,
        GAMETIME,
        LIGHTSOURCE,
        LIGHTSOURCENAME,
        ANIMSTATE,
        SLEEPHOURS,
        SYNCWEATHER,
        REVIVE,
        REVIVEDONE,
        ANIMALROLE,
        ANIMALSYNC,
        DARKWALKERREADY,
        HOSTISDARKWALKER,
        WARDISACTIVE,
        REQUESTDWREADYSTATE,
        DWCOUNTDOWN,
        ANIMALSYNCTRIGG,
        SHOOTSYNC,
        PIMPSKILL,
        HARVESTINGANIMAL,
        DONEHARVASTING,
        SAVEDATA,
        BULLETDAMAGE,
        MULTISOUND,
        CONTAINEROPEN,
        LUREPLACEMENT,
        LUREISACTIVE,
        ASKFORANIMALPROXY,
        ALIGNANIMAL,
        CARRYBODY,
        BODYWARP,
        ANIMALDELETE,
        KEEPITALIVE,
        RQRECONNECT,
        EQUIPMENT,
        CHAT,
        PLAYERSSTATUS,
        CONNECTSTEAM,
        CHANGENAME,
        CLOTH,
        ASKSPAWNDATA,
        LEVELGUID,
        FURNBROKEN,
        FURNBROKENLIST,
        FURNBREAKINGGUID,
        FURNBREAKINSTOP,
        GEARPICKUP,
        GEARPICKUPLIST,
        ROPE,
        ROPELIST,
        CONSUME,
        SERVERCFG,
        STOPCONSUME,
        HEAVYBREATH,
        BLOODLOSTS,
        APPLYACTIONONPLAYER,
        DONTMOVEWARNING,
        INFECTIONSRISK,
        CANCLEPICKUP,
        CONTAINERINTERACT,
        LOOTEDCONTAINER,
        LOOTEDCONTAINERLIST,
        HARVESTPLANT,
        LOOTEDHARVESTABLE,
        LOOTEDHARVESTABLEALL,
        SELECTEDCHARACTER,
        ADDSHELTER,
        REMOVESHELTER,
        ALLSHELTERS,
        USESHELTER,
        FIRE,
        CUSTOM,
        KICKMESSAGE,
        GOTITEMSLICE,
        VOICECHAT,
        SLICEDBYTES,
        ANIMALDAMAGE,
        FIREFUEL,
        DROPITEM,
        GOTDROPSLICE,
        PICKDROPPEDGEAR,
        REQUESTPICKUP,
        GETREQUESTEDITEMSLICE,
        REQUESTDROPSFORSCENE,
        REQUESTPLACE,
        GETREQUESTEDFORPLACESLICE,
        GOTCONTAINERSLICE,
        REQUESTOPENCONTAINER,
        OPENEMPTYCONTAINER,
        MARKSEARCHEDCONTAINERS,
        READYSENDNEXTSLICE,
        CHANGEAIM,
        LOADINGSCENEDROPSDONE,
        GEARNOTEXIST,
        USEOPENABLE,
        TRYDIAGNISISPLAYER,
        SENDMYAFFLCTIONS,
        CUREAFFLICTION,
        ANIMALTEST,
        ANIMALKILLED,
        ANIMALCORPSE,
        REQUESTANIMALCORPSE,
        QUARTERANIMAL,
        ANIMALAUDIO,
        PICKUPRABBIT,
        GOTRABBIT,
        RELEASERABBIT,
        HITRABBIT,
        RABBITREVIVED,
        CHANGEDFREQUENCY,
        MELEESTART,
        TRYBORROWGEAR,
        CHALLENGEINIT,
        CHALLENGEUPDATE,
        CHALLENGETRIGGER,
        ADDDEATHCONTAINER,
        DEATHCREATEEMPTYNOW,
        SPAWNREGIONBANCHECK,
        CAIRNS,
        BENEFITINIT,
        MODSLIST,
        RCONCONNECTED,
        RCONCOMMAND,
        RCONCALLBACK,
        ADDDOORLOCK,
        DOORLOCKEDMSG,
        TRYOPENDOOR,
        ENTERDOOR,
        LOCKPICK,
        REMOVEDOORLOCK,
        VERIFYSAVE,
        SAVEHASH,
        FORCELOADING,
        RPC,
        REQUESTLOCKSMITH,
        APPLYTOOLONBLANK,
        KNOCKKNOCK,
        KNOCKENTER,
        LETENTER,
        PEEPHOLE,
        PINGSERVER,
        RESTART,
        READYSENDNEXTSLICEGEAR,
        DEDICATEDWEATHER,
        WEATHERVOLUNTEER,
        REREGISTERWEATHER,
        REMOVEKEYBYSEED,
        ADDHUDMSG,
        CHANGECONTAINERSTATE,
        FINISHEDSENDINGCONTAINER,
        TRIGGEREMOTE,
        EXPEDITIONSYNC,
        EXPEDITIONRESULT,
        PHOTOREQUEST,
        GOTPHOTOSLICE,
        READYSENDNEXTSLICEPHOTO,
        STARTEXPEDITION,
        ACCEPTEXPEDITIONINVITE,
        REQUESTEXPEDITIONINVITES,
        CREATEEXPEDITIONINVITE,
        NEWPLAYEREXPEDITION,
        NEWEXPEDITIONINVITE,
        BASE64SLICE,
        ADDROCKCACH,
        REMOVEROCKCACH,
        REMOVEROCKCACHFINISHED,
        CHARCOALDRAW,
        CHATCOMMAND,
        ADDUNIVERSALSYNCABLE,
        REMOVEUNIVERSALSYNCABLE,
        REQUESTCONTAINERSTATE,
    }

    /// <summary>Sent from client to server.</summary>
    public enum ClientPackets
    {
        welcomeReceived = 1,
        XYZ,
        XYZW,
        BLOCK,
        XYZDW,
        LEVELID,
        GOTITEM,
        GAMETIME,
        LIGHTSOURCE,
        LIGHTSOURCENAME,
        ANIMSTATE,
        SLEEPHOURS,
        SYNCWEATHER,
        REVIVE,
        REVIVEDONE,
        ANIMALROLE,
        ANIMALSYNC,
        DARKWALKERREADY,
        HOSTISDARKWALKER,
        WARDISACTIVE,
        REQUESTDWREADYSTATE,
        DWCOUNTDOWN,
        ANIMALSYNCTRIGG,
        SHOOTSYNC,
        PIMPSKILL,
        HARVESTINGANIMAL,
        DONEHARVASTING,
        SAVEDATA,
        BULLETDAMAGE,
        MULTISOUND,
        CONTAINEROPEN,
        LUREPLACEMENT,
        LUREISACTIVE,
        ASKFORANIMALPROXY,
        ALIGNANIMAL,
        CARRYBODY,
        BODYWARP,
        ANIMALDELETE,
        KEEPITALIVE,
        RQRECONNECT,
        EQUIPMENT,
        CHAT,
        PLAYERSSTATUS,
        CONNECTSTEAM,
        CHANGENAME,
        CLOTH,
        ASKSPAWNDATA,
        LEVELGUID,
        FURNBROKEN,
        FURNBROKENLIST,
        FURNBREAKINGGUID,
        FURNBREAKINSTOP,
        GEARPICKUP,
        GEARPICKUPLIST,
        ROPE,
        ROPELIST,
        CONSUME,
        SERVERCFG,
        STOPCONSUME,
        HEAVYBREATH,
        BLOODLOSTS,
        APPLYACTIONONPLAYER,
        DONTMOVEWARNING,
        INFECTIONSRISK,
        CANCLEPICKUP,
        CONTAINERINTERACT,
        LOOTEDCONTAINER,
        LOOTEDCONTAINERLIST,
        HARVESTPLANT,
        LOOTEDHARVESTABLE,
        LOOTEDHARVESTABLEALL,
        SELECTEDCHARACTER,
        ADDSHELTER,
        REMOVESHELTER,
        ALLSHELTERS,
        USESHELTER,
        FIRE,
        CUSTOM,
        KICKMESSAGE,
        GOTITEMSLICE,
        VOICECHAT,
        SLICEDBYTES,
        ANIMALDAMAGE,
        FIREFUEL,
        DROPITEM,
        GOTDROPSLICE,
        PICKDROPPEDGEAR,
        REQUESTPICKUP,
        GETREQUESTEDITEMSLICE,
        REQUESTDROPSFORSCENE,
        REQUESTPLACE,
        GETREQUESTEDFORPLACESLICE,
        GOTCONTAINERSLICE,
        REQUESTOPENCONTAINER,
        OPENEMPTYCONTAINER,
        MARKSEARCHEDCONTAINERS,
        READYSENDNEXTSLICE,
        CHANGEAIM,
        LOADINGSCENEDROPSDONE,
        GEARNOTEXIST,
        USEOPENABLE,
        TRYDIAGNISISPLAYER,
        SENDMYAFFLCTIONS,
        CUREAFFLICTION,
        ANIMALTEST,
        ANIMALKILLED,
        ANIMALCORPSE,
        REQUESTANIMALCORPSE,
        QUARTERANIMAL,
        ANIMALAUDIO,
        PICKUPRABBIT,
        GOTRABBIT,
        RELEASERABBIT,
        HITRABBIT,
        RABBITREVIVED,
        CHANGEDFREQUENCY,
        MELEESTART,
        TRYBORROWGEAR,
        CHALLENGEINIT,
        CHALLENGEUPDATE,
        CHALLENGETRIGGER,
        ADDDEATHCONTAINER,
        DEATHCREATEEMPTYNOW,
        SPAWNREGIONBANCHECK,
        CAIRNS,
        BENEFITINIT,
        MODSLIST,
        RCONCONNECTED,
        RCONCOMMAND,
        RCONCALLBACK,
        ADDDOORLOCK,
        DOORLOCKEDMSG,
        TRYOPENDOOR,
        ENTERDOOR,
        LOCKPICK,
        REMOVEDOORLOCK,
        VERIFYSAVE,
        SAVEHASH,
        FORCELOADING,
        RPC,
        REQUESTLOCKSMITH,
        APPLYTOOLONBLANK,
        KNOCKKNOCK,
        KNOCKENTER,
        LETENTER,
        PEEPHOLE,
        PINGSERVER,
        RESTART,
        READYSENDNEXTSLICEGEAR,
        DEDICATEDWEATHER,
        WEATHERVOLUNTEER,
        REREGISTERWEATHER,
        REMOVEKEYBYSEED,
        ADDHUDMSG,
        CHANGECONTAINERSTATE,
        FINISHEDSENDINGCONTAINER,
        TRIGGEREMOTE,
        EXPEDITIONSYNC,
        EXPEDITIONRESULT,
        PHOTOREQUEST,
        GOTPHOTOSLICE,
        READYSENDNEXTSLICEPHOTO,
        STARTEXPEDITION,
        ACCEPTEXPEDITIONINVITE,
        REQUESTEXPEDITIONINVITES,
        CREATEEXPEDITIONINVITE,
        NEWPLAYEREXPEDITION,
        NEWEXPEDITIONINVITE,
        BASE64SLICE,
        ADDROCKCACH,
        REMOVEROCKCACH,
        REMOVEROCKCACHFINISHED,
        CHARCOALDRAW,
        CHATCOMMAND,
        ADDUNIVERSALSYNCABLE,
        REMOVEUNIVERSALSYNCABLE,
        REQUESTCONTAINERSTATE,
    }

    public class Packet : IDisposable
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

        public static void Log(string TXT, Shared.LoggerColor Color = Shared.LoggerColor.White)
        {
#if (!DEDICATED)
            MelonLogger.Msg(TXT);
#else
            Logger.Log(TXT);
#endif
        }

        /// <summary>Creates a new empty packet (without an ID).</summary>
        public Packet()
        {
            buffer = new List<byte>(); // Intitialize buffer
            readPos = 0; // Set readPos to 0
        }

        /// <summary>Creates a new packet with a given ID. Used for sending.</summary>
        /// <param name="_id">The packet ID.</param>
        public Packet(int _id)
        {
            buffer = new List<byte>(); // Intitialize buffer
            readPos = 0; // Set readPos to 0

            Write(_id); // Write packet id to the buffer
        }

        public int ReturnSize()
        {
            return buffer.Count;
        }
        public int ReturnUnusedBytesCount()
        {
            int Count = buffer.Count - readPos;
            return Count;
        }

        /// <summary>Creates a packet from which data can be read. Used for receiving.</summary>
        /// <param name="_data">The bytes to add to the packet.</param>
        public Packet(byte[] _data)
        {
            buffer = new List<byte>(); // Intitialize buffer
            readPos = 0; // Set readPos to 0

            SetBytes(_data);
        }

#region Functions
        /// <summary>Sets the packet's content and prepares it to be read.</summary>
        /// <param name="_data">The bytes to add to the packet.</param>
        public void SetBytes(byte[] _data)
        {
            Write(_data);
            readableBuffer = buffer.ToArray();
        }

        /// <summary>Inserts the length of the packet's content at the start of the buffer.</summary>
        public void WriteLength()
        {
            buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count)); // Insert the byte length of the packet at the very beginning
        }

        /// <summary>Inserts the given int at the start of the buffer.</summary>
        /// <param name="_value">The int to insert.</param>
        public void InsertInt(int _value)
        {
            buffer.InsertRange(0, BitConverter.GetBytes(_value)); // Insert the int at the start of the buffer
        }
        public void InsertString(string _value)
        {
            buffer.InsertRange(0, Encoding.UTF8.GetBytes(_value));
            buffer.InsertRange(0, BitConverter.GetBytes(_value.Length));
        }
        public void InsertClientInfo(int ID, string GUID)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(ID)); // ID
            bytes.AddRange(BitConverter.GetBytes(GUID.Length)); // GUID Length
            bytes.AddRange(Encoding.UTF8.GetBytes(GUID)); // GUID

            buffer.InsertRange(0, bytes);
        }

        /// <summary>Gets the packet's content in array form.</summary>
        public byte[] ToArray()
        {
            readableBuffer = buffer.ToArray();
            return readableBuffer;
        }

        /// <summary>Gets the length of the packet's content.</summary>
        public int Length()
        {
            return buffer.Count; // Return the length of buffer
        }

        /// <summary>Gets the length of the unread data contained in the packet.</summary>
        public int UnreadLength()
        {
            return Length() - readPos; // Return the remaining length (unread)
        }

        /// <summary>Resets the packet instance to allow it to be reused.</summary>
        /// <param name="_shouldReset">Whether or not to reset the packet.</param>
        public void Reset(bool _shouldReset = true)
        {
            if (_shouldReset)
            {
                buffer.Clear(); // Clear buffer
                readableBuffer = null;
                readPos = 0; // Reset readPos
            }
            else
            {
                readPos -= 4; // "Unread" the last read int
            }
        }
#endregion

#region Write Data
        /// <summary>Adds a byte to the packet.</summary>
        /// <param name="_value">The byte to add.</param>
        public void Write(byte _value)
        {
            buffer.Add(_value);
        }
        /// <summary>Adds an array of bytes to the packet.</summary>
        /// <param name="_value">The byte array to add.</param>
        public void Write(byte[] _value)
        {
            buffer.AddRange(_value);
        }
        /// <summary>Adds a short to the packet.</summary>
        /// <param name="_value">The short to add.</param>
        public void Write(short _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adds an int to the packet.</summary>
        /// <param name="_value">The int to add.</param>
        public void Write(int _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adds a long to the packet.</summary>
        /// <param name="_value">The long to add.</param>
        public void Write(long _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adds a float to the packet.</summary>
        /// <param name="_value">The float to add.</param>
        public void Write(float _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adds a bool to the packet.</summary>
        /// <param name="_value">The bool to add.</param>
        public void Write(bool _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adds a string to the packet.</summary>
        /// <param name="_value">The string to add.</param>
        public void Write(string _value)
        {
            Write(_value.Length);
            buffer.AddRange(Encoding.UTF8.GetBytes(_value));
        }
        public void WriteUnicodeString(string _value)
        {
            Write(Encoding.Unicode.GetBytes(_value).Length);
            buffer.AddRange(Encoding.Unicode.GetBytes(_value));
        }
        
        public void Write(Vector3 _value)
        {
#if (!DEDICATED)
            Write(_value.x);
            Write(_value.y);
            Write(_value.z);
#else
            Write(_value.X);
            Write(_value.Y);
            Write(_value.Z);
#endif
        }
        /// <summary>Adds a Quaternion to the packet.</summary>
        /// <param name="_value">The Quaternion to add.</param>
        public void Write(Quaternion _value)
        {
#if (!DEDICATED)
            Write(_value.x);
            Write(_value.y);
            Write(_value.z);
            Write(_value.w);
#else
            Write(_value.X);
            Write(_value.Y);
            Write(_value.Z);
            Write(_value.W);
#endif
        }
        public void Write(DataStr.WeatherProxies weather)
        {
            Write(weather.m_WeatherProxy);
            Write(weather.m_WeatherTransitionProxy);
            Write(weather.m_WindProxy);
        }

        public void Write(DataStr.GearItemDataPacket gear)
        {
            Write(gear.m_GearName);
            Write(gear.m_DataProxy);
            Write(gear.m_Water);
            Write(gear.m_SendedTo);
        }
        public void Write(DataStr.DroppedGearItemDataPacket gear)
        {
            Write(gear.m_GearID);
            Write(gear.m_Position);
            Write(gear.m_Rotation);
            Write(gear.m_LevelID);
            Write(gear.m_LevelGUID);
            Write(gear.m_Hash);
            Write(gear.m_Extra);
        }
        public void Write(DataStr.ShowShelterByOther shelter)
        {
            Write(shelter.m_Position);
            Write(shelter.m_Rotation);
            Write(shelter.m_LevelID);
            Write(shelter.m_LevelGUID);
        }
        public void Write(DataStr.AnimalSync obj)
        {
            Write(obj.m_position);
            Write(obj.m_rotation);
            Write(obj.m_guid);
            Write(obj.m_name);
            Write(obj.m_Hp);
            Write(obj.m_Bleeding);

            Write(obj.m_Controller);
            Write(obj.m_ProxySave);
            Write(obj.m_LevelD);
            Write(obj.m_SpawnRegionGUID);
        }

        public void Write(DataStr.AnimalAnimsSync obj)
        {
            Write(obj.AP_TurnAngle);
            Write(obj.AP_TurnSpeed);
            Write(obj.AP_Speed);
            Write(obj.AP_Wounded);
            Write(obj.AP_Roll);
            Write(obj.AP_Pitch);
            Write(obj.AP_TargetHeading);
            Write(obj.AP_TargetHeadingSmooth);
            Write(obj.AP_TapMeter);
            Write(obj.AP_AiState);
            Write(obj.AP_Corpse);
            Write(obj.AP_Dead);
            Write(obj.AP_DeadSide);
            Write(obj.AP_DamageBodyPart);
            Write(obj.AP_AttackId);
            Write(obj.AP_Stunned);
        }

        public void Write(DataStr.AnimalTrigger obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_Trigger);
        }

        public void Write(DataStr.ShootSync obj)
        {
            Write(obj.m_position);
            Write(obj.m_rotation);
            Write(obj.m_projectilename);
            Write(obj.m_skill);
            Write(obj.m_camera_forward);
            Write(obj.m_camera_right);
            Write(obj.m_camera_up);
            Write(obj.m_lookat);
            Write(obj.m_sceneguid);
        }

        public void Write(DataStr.HarvestStats obj)
        {
            Write(obj.m_Meat);
            Write(obj.m_Guts);
            Write(obj.m_Hide);
            Write(obj.m_Guid);
        }

        public void Write(DataStr.SaveSlotSync obj)
        {
            Write(obj.m_Episode);
            Write(obj.m_SaveSlotType);
            Write(obj.m_Seed);
            Write(obj.m_ExperienceMode);
            Write(obj.m_Location);
            Write(obj.m_FixedSpawnScene);
            Write(obj.m_FixedSpawnPosition);
            Write(obj.m_CustomExperienceStr);
        }

        public void Write(DataStr.ContainerOpenSync obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_State);
            Write(obj.m_LevelID);
            Write(obj.m_LevelGUID);
            Write(obj.m_Inspected);
        }
        public void Write(DataStr.AnimalAligner obj)
        {
            Write(obj.m_Proxy);
            Write(obj.m_Guid);
        }
        public void Write(DataStr.PlayerEquipmentData obj)
        {
            Write(obj.m_Arrows);
            Write(obj.m_HasAxe);
            Write(obj.m_HasMedkit);
            Write(obj.m_HasRevolver);
            Write(obj.m_HasRifle);
            Write(obj.m_Flares);
            Write(obj.m_BlueFlares);
        }
        public void Write(DataStr.MultiplayerChatMessage obj)
        {
            WriteUnicodeString(obj.m_By);
            WriteUnicodeString(obj.m_Message);
            Write(obj.m_Type);
            Write(obj.m_Global);
            Write(obj.m_Private);
        }
        public void Write(DataStr.MultiPlayerClientStatus obj)
        {
            Write(obj.m_ID);
            Write(obj.m_Name);
            Write(obj.m_Sleep);
            Write(obj.m_Dead);
            Write(obj.m_IsLoading);
        }
        public void Write(DataStr.PlayerClothingData obj)
        {
            Write(obj.m_Hat);
            Write(obj.m_Top);
            Write(obj.m_Bottom);
            Write(obj.m_Boots);
            Write(obj.m_Scarf);
            Write(obj.m_Balaclava);
        }
        public void Write(DataStr.BrokenFurnitureSync obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_ParentGuid);
            Write(obj.m_LevelID);
            Write(obj.m_LevelGUID);
            Write(obj.m_Broken);
        }
        public void Write(DataStr.PickedGearSync obj)
        {
            Write(obj.m_Spawn);
            Write(obj.m_LevelID);
            Write(obj.m_LevelGUID);
            Write(obj.m_MyInstanceID);
            Write(obj.m_GearName);
        }
        public void Write(DataStr.ClimbingRopeSync obj)
        {
            Write(obj.m_Position);
            Write(obj.m_LevelID);
            Write(obj.m_LevelGUID);
            Write(obj.m_Deployed);
            Write(obj.m_Snapped);
        }
        public void Write(DataStr.ServerConfigData obj)
        {
            Write(obj.m_FastConsumption);
            Write(obj.m_DuppedSpawns);
            Write(obj.m_DuppedContainers);
            Write(obj.m_PlayersSpawnType);
            Write(obj.m_FireSync);
            Write(obj.m_CheatsMode);
            Write(obj.m_CheckModsValidation);
            Write(obj.m_SaveScamProtection);
            Write(obj.m_PVP);
        }
        public void Write(DataStr.HarvestableSyncData obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_State);
        }
        public void Write(DataStr.FireSourcesSync obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_LevelGUID);
            Write(obj.m_LevelId);
            Write(obj.m_Position);
            Write(obj.m_Rotation);
            Write(obj.m_IsCampfire);
            Write(obj.m_FuelName);
        }
        public void Write(DataStr.SlicedJsonData obj)
        {
            Write(obj.m_GearName);
            Write(obj.m_Hash);
            Write(obj.m_Str);
            Write(obj.m_SendTo);
            Write(obj.m_Last);
            Write(obj.m_Extra);
            Write(obj.m_CheckHash);
        }
        public void Write(DataStr.SlicedBytesData obj)
        {
            Write(obj.m_Hash);
            Write(obj.m_Action);
            Write(obj.m_SendTo);
            Write(obj.m_Last);
            Write(obj.m_Length);
            Write(obj.m_ExtraInt);
            Write(obj.m_Data);
        }
        public void Write(DataStr.ExtraDataForDroppedGear obj)
        {
            Write(obj.m_DroppedTime);
            Write(obj.m_GoalTime);
            Write(obj.m_Dropper);
            Write(obj.m_Variant);
            Write(obj.m_GearName);
            Write(obj.m_PhotoGUID);
            WriteUnicodeString(obj.m_ExpeditionNote);
        }
        public void Write(DataStr.AffictionSync obj)
        {
            Write(obj.m_Type);
            Write(obj.m_Location);
            WriteUnicodeString(obj.m_Case);
            Write(obj.m_ShouldBeTreated);
        }

        public void Write(DataStr.AnimalArrow Arrow)
        {
            Write(Arrow.m_Condition);
            Write(Arrow.m_Position);
            Write(Arrow.m_Angle);
            Write(Arrow.m_Depth);
            Write(Arrow.m_LocaName);
        }
        public void Write(List<DataStr.AnimalArrow> Arrows, int Count)
        {
            Write(Count);
            for (int i = 0; i < Arrows.Count; i++)
            {
                Write(Arrows[i]);
            }
        }

        public void Write(DataStr.AnimalCompactData obj)
        {
            Write(obj.m_PrefabName);
            Write(obj.m_Position);
            Write(obj.m_Rotation);
            Write(obj.m_GUID);
            Write(obj.m_LastSeen);
            Write(obj.m_Health);
            Write(obj.m_Bleeding);
            Write(obj.m_TimeOfBleeding);
            Write(obj.m_RegionGUID);
            Write(obj.m_LastController);
            Write(obj.m_LastAiMode);
            Write(obj.m_Arrows, obj.m_ArrowsCount);
        }
        public void Write(DataStr.AnimalKilled obj)
        {
            Write(obj.m_Position);
            Write(obj.m_Rotation);
            Write(obj.m_PrefabName);
            Write(obj.m_GUID);
            Write(obj.m_LevelGUID);
            Write(obj.m_CreatedTime);

            Write(obj.m_Meat);
            Write(obj.m_Guts);
            Write(obj.m_Hide);

            Write(obj.m_Knocked);

            Write(obj.m_RegionGUID);
        }

        public void Write(List<int> LIST)
        {
            Write(LIST.Count);
            for (int i = 0; i < LIST.Count; i++)
            {
                Write(LIST[i]);
            }
        }
        public void Write(List<float> LIST)
        {
            Write(LIST.Count);
            for (int i = 0; i < LIST.Count; i++)
            {
                Write(LIST[i]);
            }
        }
        public void Write(List<string> LIST)
        {
            Write(LIST.Count);
            for (int i = 0; i < LIST.Count; i++)
            {
                Write(LIST[i]);
            }
        }
        public void Write(List<ExpeditionManager.ExpeditionInvite> LIST)
        {
            Write(LIST.Count);
            for (int i = 0; i < LIST.Count; i++)
            {
                Write(LIST[i]);
            }
        }

        public void Write(DataStr.CustomChallengeData DAT)
        {
            Write(DAT.m_CurrentTask);
            Write(DAT.m_Time);
            Write(DAT.m_Started);
            Write(DAT.m_Done);
        }
        public DataStr.CustomChallengeData ReadChallengeData()
        {
            DataStr.CustomChallengeData obj = new DataStr.CustomChallengeData();
            obj.m_CurrentTask = ReadInt();
            obj.m_Time = ReadInt();
            obj.m_Started = ReadBool();
            obj.m_Done = ReadIntList();
            return obj;
        }

        public List<int> ReadIntList()
        {
            List<int> LIST = new List<int>();
            int Count = ReadInt();
            for (int i = 0; i < Count; i++)
            {
                LIST.Add(ReadInt());
            }
            return LIST;
        }
        public List<float> ReadFloatList()
        {
            List<float> LIST = new List<float>();
            int Count = ReadInt();
            for (int i = 0; i < Count; i++)
            {
                LIST.Add(ReadFloat());
            }
            return LIST;
        }
        public List<string> ReadStringList()
        {
            List<string> LIST = new List<string>();
            int Count = ReadInt();
            for (int i = 0; i < Count; i++)
            {
                LIST.Add(ReadString());
            }
            return LIST;
        }
        public List<ExpeditionManager.ExpeditionInvite> ReadInvitesList()
        {
            List<ExpeditionManager.ExpeditionInvite> LIST = new List<ExpeditionManager.ExpeditionInvite>();
            int Count = ReadInt();
            for (int i = 0; i < Count; i++)
            {
                LIST.Add(ReadExpeditionInvite());
            }
            return LIST;
        }

        public void Write(DataStr.DeathContainerData obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_LevelKey);
            Write(obj.m_Position);
            Write(obj.m_Rotation);
            Write(obj.m_Owner);
            Write(obj.m_ContainerPrefab);
        }
        public DataStr.DeathContainerData ReadDeathContainer()
        {
            DataStr.DeathContainerData obj = new DataStr.DeathContainerData();
            obj.m_Guid = ReadString();
            obj.m_LevelKey = ReadString();
            obj.m_Position = ReadVector3();
            obj.m_Rotation = ReadQuaternion();
            obj.m_Owner = ReadString();
            obj.m_ContainerPrefab = ReadString();
            return obj;
        }


#endregion

#region Read Data
        /// <summary>Reads a byte from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public byte ReadByte(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                byte _value = readableBuffer[readPos]; // Get the byte at readPos' position
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 1; // Increase readPos by 1
                }
                return _value; // Return the byte
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        /// <summary>Reads an array of bytes from the packet.</summary>
        /// <param name="_length">The length of the byte array.</param>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public byte[] ReadBytes(int _length, bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                byte[] _value = buffer.GetRange(readPos, _length).ToArray(); // Get the bytes at readPos' position with a range of _length
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += _length; // Increase readPos by _length
                }
                return _value; // Return the bytes
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        /// <summary>Reads a short from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public short ReadShort(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                short _value = BitConverter.ToInt16(readableBuffer, readPos); // Convert the bytes to a short
                if (_moveReadPos)
                {
                    // If _moveReadPos is true and there are unread bytes
                    readPos += 2; // Increase readPos by 2
                }
                return _value; // Return the short
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }

        /// <summary>Reads an int from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public int ReadInt(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes

                if(readableBuffer == null)
                {
                    Log("Could not read value of type 'int'! readableBuffer is null!!!");
                    return 0;
                }

                int _value = BitConverter.ToInt32(readableBuffer, readPos); // Convert the bytes to an int
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 4; // Increase readPos by 4
                }
                return _value; // Return the int
            }
            else
            {
                //throw new Exception("Could not read value of type 'int'!");
                Log("Could not read value of type 'int'!");
                return 0;
            }
        }

        /// <summary>Reads a long from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public long ReadLong(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                long _value = BitConverter.ToInt64(readableBuffer, readPos); // Convert the bytes to a long
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 8; // Increase readPos by 8
                }
                return _value; // Return the long
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        /// <summary>Reads a float from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public float ReadFloat(bool _moveReadPos = true)
        {

            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                float _value = BitConverter.ToSingle(readableBuffer, readPos); // Convert the bytes to a float
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 4; // Increase readPos by 4
                }
                return _value; // Return the float
            }
            else
            {
                //throw new Exception("Could not read value of type 'float'!");
                return -0.777f;
            }
        }

        /// <summary>Reads a bool from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public bool ReadBool(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                bool _value = BitConverter.ToBoolean(readableBuffer, readPos); // Convert the bytes to a bool
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 1; // Increase readPos by 1
                }
                return _value; // Return the bool
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        /// <summary>Reads a string from the packet.</summary>
        /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
        public string ReadString(bool _moveReadPos = true)
        {
            try
            {
                int _length = ReadInt(); // Get the length of the string
                //string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length); // Convert the bytes to a string
                string _value = Encoding.UTF8.GetString(readableBuffer, readPos, _length); // Convert the bytes to a string
                //_value = _value.TrimStart();
                if (_moveReadPos && _value.Length > 0)
                {
                    // If _moveReadPos is true string is not empty
                    readPos += _length; // Increase readPos by the length of the string
                }
                return _value; // Return the string
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }
        public string ReadUnicodeString()
        {
            try
            {
                int _length = ReadInt();
                string _value = Encoding.Unicode.GetString(readableBuffer, readPos, _length);
                readPos += _length;
                return _value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }
        public Vector3 ReadVector3(bool _moveReadPos = true)
        {
            return new Vector3(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
        }
        public Quaternion ReadQuaternion(bool _moveReadPos = true)
        {
            return new Quaternion(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
        }
        public DataStr.WeatherProxies ReadWeather(bool _moveReadPos = true)
        {
            DataStr.WeatherProxies weather = new DataStr.WeatherProxies();

            weather.m_WeatherProxy = ReadString(_moveReadPos);
            weather.m_WeatherTransitionProxy = ReadString(_moveReadPos);
            weather.m_WindProxy = ReadString(_moveReadPos);

            return weather;
        }

        public DataStr.AnimalAnimsSync ReadAnimalAnim(bool _moveReadPos = true)
        {
            DataStr.AnimalAnimsSync obj = new DataStr.AnimalAnimsSync();

            obj.AP_TurnAngle = ReadFloat();
            obj.AP_TurnSpeed = ReadFloat();
            obj.AP_Speed = ReadFloat();
            obj.AP_Wounded = ReadFloat();
            obj.AP_Roll = ReadFloat();
            obj.AP_Pitch = ReadFloat();
            obj.AP_TargetHeading = ReadFloat();
            obj.AP_TargetHeadingSmooth = ReadFloat();
            obj.AP_TapMeter = ReadFloat();
            obj.AP_AiState = ReadInt();
            obj.AP_Corpse = ReadBool();
            obj.AP_Dead = ReadBool();
            obj.AP_DeadSide = ReadInt();
            obj.AP_DamageBodyPart = ReadInt();
            obj.AP_AttackId = ReadInt();
            obj.AP_Stunned = ReadBool();

            return obj;
        }

        public DataStr.AnimalTrigger ReadAnimalTrigger(bool _moveReadPos = true)
        {
            DataStr.AnimalTrigger obj = new DataStr.AnimalTrigger();
            obj.m_Guid = ReadString();
            obj.m_Trigger = ReadInt();
            return obj;
        }

        public DataStr.ShootSync ReadShoot(bool _moveReadPos = true)
        {
            DataStr.ShootSync obj = new DataStr.ShootSync();
            obj.m_position = ReadVector3();
            obj.m_rotation = ReadQuaternion();
            obj.m_projectilename = ReadString();
            obj.m_skill = ReadFloat();
            obj.m_camera_forward = ReadVector3();
            obj.m_camera_right = ReadVector3();
            obj.m_camera_up = ReadVector3();
            obj.m_lookat = ReadBool();
            obj.m_sceneguid = ReadString();
            return obj;
        }
        public DataStr.HarvestStats ReadHarvest(bool _moveReadPos = true)
        {
            DataStr.HarvestStats obj = new DataStr.HarvestStats();
            obj.m_Meat = ReadFloat();
            obj.m_Guts = ReadInt();
            obj.m_Hide = ReadInt();
            obj.m_Guid = ReadString();
            return obj;
        }

        public DataStr.SaveSlotSync ReadSaveSlot(bool _moveReadPos = true)
        {
            DataStr.SaveSlotSync obj = new DataStr.SaveSlotSync();

            obj.m_Episode = ReadInt();
            obj.m_SaveSlotType = ReadInt();
            obj.m_Seed = ReadInt();
            obj.m_ExperienceMode = ReadInt();
            obj.m_Location = ReadInt();
            obj.m_FixedSpawnScene = ReadString();
            obj.m_FixedSpawnPosition = ReadVector3();
            obj.m_CustomExperienceStr = ReadString();
            return obj;
        }

        public DataStr.ContainerOpenSync ReadContainer(bool _moveReadPos = true)
        {
            DataStr.ContainerOpenSync obj = new DataStr.ContainerOpenSync();

            obj.m_Guid = ReadString();
            obj.m_State = ReadBool();
            obj.m_LevelID = ReadInt();
            obj.m_LevelGUID = ReadString();
            obj.m_Inspected = ReadBool();
            return obj;
        }

        public DataStr.AnimalAligner ReadAnimalAligner(bool _moveReadPos = true)
        {
            DataStr.AnimalAligner obj = new DataStr.AnimalAligner();
            obj.m_Proxy = ReadString();
            obj.m_Guid = ReadString();
            return obj;
        }

        public DataStr.PlayerEquipmentData ReadEQ(bool _moveReadPos = true)
        {
            DataStr.PlayerEquipmentData obj = new DataStr.PlayerEquipmentData();
            obj.m_Arrows = ReadInt();
            obj.m_HasAxe = ReadBool();
            obj.m_HasMedkit = ReadBool();
            obj.m_HasRevolver = ReadBool();
            obj.m_HasRifle = ReadBool();
            obj.m_Flares = ReadInt();
            obj.m_BlueFlares = ReadInt();

            return obj;
        }
        public DataStr.MultiplayerChatMessage ReadChat()
        {
            DataStr.MultiplayerChatMessage obj = new DataStr.MultiplayerChatMessage();
            obj.m_By = ReadUnicodeString();
            obj.m_Message = ReadUnicodeString();
            obj.m_Type = ReadInt();
            obj.m_Global = ReadBool();
            obj.m_Private = ReadBool();

            return obj;
        }

        public DataStr.MultiPlayerClientStatus ReadClientStatus()
        {
            DataStr.MultiPlayerClientStatus obj = new DataStr.MultiPlayerClientStatus();
            obj.m_ID = ReadInt();
            obj.m_Name = ReadString();
            obj.m_Sleep = ReadBool();
            obj.m_Dead = ReadBool();
            obj.m_IsLoading = ReadBool();

            return obj;
        }
        public DataStr.PlayerClothingData ReadClothingData()
        {
            DataStr.PlayerClothingData obj = new DataStr.PlayerClothingData();
            obj.m_Hat = ReadString();
            obj.m_Top = ReadString();
            obj.m_Bottom = ReadString();
            obj.m_Boots = ReadString();
            obj.m_Scarf = ReadString();
            obj.m_Balaclava = ReadString();

            return obj;
        }

        public DataStr.BrokenFurnitureSync ReadFurn()
        {
            DataStr.BrokenFurnitureSync obj = new DataStr.BrokenFurnitureSync();
            obj.m_Guid = ReadString();
            obj.m_ParentGuid = ReadString();
            obj.m_LevelID = ReadInt();
            obj.m_LevelGUID = ReadString();
            obj.m_Broken = ReadBool();

            return obj;
        }

        public DataStr.PickedGearSync ReadPickedGear()
        {
            DataStr.PickedGearSync obj = new DataStr.PickedGearSync();
            obj.m_Spawn = ReadVector3();
            obj.m_LevelID = ReadInt();
            obj.m_LevelGUID = ReadString();
            obj.m_MyInstanceID = ReadInt();
            obj.m_GearName = ReadString();

            return obj;
        }

        public DataStr.ClimbingRopeSync ReadRope()
        {
            DataStr.ClimbingRopeSync obj = new DataStr.ClimbingRopeSync();
            obj.m_Position = ReadVector3();
            obj.m_LevelID = ReadInt();
            obj.m_LevelGUID = ReadString();
            obj.m_Deployed = ReadBool();
            obj.m_Snapped = ReadBool();

            return obj;
        }

        public DataStr.ServerConfigData ReadServerCFG()
        {
            DataStr.ServerConfigData obj = new DataStr.ServerConfigData();
            obj.m_FastConsumption = ReadBool();
            obj.m_DuppedSpawns = ReadBool();
            obj.m_DuppedContainers = ReadBool();
            obj.m_PlayersSpawnType = ReadInt();
            obj.m_FireSync = ReadInt();
            obj.m_CheatsMode = ReadInt();
            obj.m_CheckModsValidation = ReadBool();
            obj.m_SaveScamProtection = ReadBool();
            obj.m_PVP = ReadBool();

            return obj;
        }
        public DataStr.HarvestableSyncData ReadHarvestablePlant()
        {
            DataStr.HarvestableSyncData obj = new DataStr.HarvestableSyncData();
            obj.m_Guid = ReadString();
            obj.m_State = ReadString();

            return obj;
        }

        public DataStr.GearItemDataPacket ReadGearData()
        {
            DataStr.GearItemDataPacket obj = new DataStr.GearItemDataPacket();
            obj.m_GearName = ReadString();
            obj.m_DataProxy = ReadString();
            obj.m_Water = ReadFloat();
            obj.m_SendedTo = ReadInt();
            
            return obj;
        }
        public DataStr.DroppedGearItemDataPacket ReadDroppedGearData()
        {
            DataStr.DroppedGearItemDataPacket obj = new DataStr.DroppedGearItemDataPacket();
            obj.m_GearID = ReadInt();
            obj.m_Position = ReadVector3();
            obj.m_Rotation = ReadQuaternion();
            obj.m_LevelID = ReadInt();
            obj.m_LevelGUID = ReadString();
            obj.m_Hash = ReadInt();
            obj.m_Extra = ReadExtraDataForDropperGear();

            return obj;
        }
        public DataStr.ShowShelterByOther ReadShelter()
        {
            DataStr.ShowShelterByOther obj = new DataStr.ShowShelterByOther();
            obj.m_Position = ReadVector3();
            obj.m_Rotation = ReadQuaternion();
            obj.m_LevelID = ReadInt();
            obj.m_LevelGUID = ReadString();

            return obj;
        }
        public DataStr.FireSourcesSync ReadFire()
        {
            DataStr.FireSourcesSync obj = new DataStr.FireSourcesSync();
            obj.m_Guid = ReadString();
            obj.m_LevelGUID = ReadString();
            obj.m_LevelId = ReadInt();
            obj.m_Position = ReadVector3();
            obj.m_Rotation = ReadQuaternion();
            obj.m_IsCampfire = ReadBool();
            obj.m_FuelName = ReadString();

            return obj;
        }

        public DataStr.SlicedJsonData ReadSlicedGear()
        {
            DataStr.SlicedJsonData obj = new DataStr.SlicedJsonData();
            obj.m_GearName = ReadString();
            obj.m_Hash = ReadInt();
            obj.m_Str = ReadString();
            obj.m_SendTo = ReadInt();
            obj.m_Last = ReadBool();
            obj.m_Extra = ReadExtraDataForDropperGear();
            obj.m_CheckHash = ReadLong();
            return obj;
        }
        public DataStr.SlicedBytesData ReadSlicedBytes()
        {
            DataStr.SlicedBytesData obj = new DataStr.SlicedBytesData();
            obj.m_Hash = ReadInt();
            obj.m_Action = ReadString();
            obj.m_SendTo = ReadInt();
            obj.m_Last = ReadBool();
            obj.m_Length = ReadInt();
            obj.m_ExtraInt = ReadInt();
            obj.m_Data = ReadBytes(obj.m_Length);
            return obj;
        }

        public DataStr.ExtraDataForDroppedGear ReadExtraDataForDropperGear()
        {
            DataStr.ExtraDataForDroppedGear obj = new DataStr.ExtraDataForDroppedGear();
            obj.m_DroppedTime = ReadInt();
            obj.m_GoalTime = ReadInt();
            obj.m_Dropper = ReadString();
            obj.m_Variant = ReadInt();
            obj.m_GearName = ReadString();
            obj.m_PhotoGUID = ReadString();
            obj.m_ExpeditionNote = ReadUnicodeString();

            return obj;
        }
        public DataStr.AffictionSync ReadAffiction()
        {
            DataStr.AffictionSync obj = new DataStr.AffictionSync();
            obj.m_Type = ReadInt();
            obj.m_Location = ReadInt();
            obj.m_Case = ReadUnicodeString();
            obj.m_ShouldBeTreated = ReadBool();
            return obj;
        }


        public class AnimalArrow
        {
            public float m_Condition = 100;
            public Vector3 m_Position = new Vector3(0, 0, 0);
            public Vector3 m_Angle = new Vector3(0, 0, 0);
            public float m_Depth = 0;
            public string m_LocaName = "";
        }
        public DataStr.AnimalArrow ReadArrow()
        {
            DataStr.AnimalArrow Arrow = new DataStr.AnimalArrow();

            Arrow.m_Condition = ReadFloat();
            Arrow.m_Position = ReadVector3();
            Arrow.m_Angle = ReadVector3();
            Arrow.m_Depth = ReadFloat();
            Arrow.m_LocaName = ReadString();
            return Arrow;
        }

        public DataStr.AnimalCompactData ReadAnimalCompactData()
        {
            DataStr.AnimalCompactData obj = new DataStr.AnimalCompactData();
            obj.m_PrefabName = ReadString();
            obj.m_Position = ReadVector3();
            obj.m_Rotation = ReadQuaternion();
            obj.m_GUID = ReadString();
            obj.m_LastSeen = ReadInt();
            obj.m_Health = ReadFloat();
            obj.m_Bleeding = ReadBool();
            obj.m_TimeOfBleeding = ReadInt();
            obj.m_RegionGUID = ReadString();
            obj.m_LastController = ReadInt();
            obj.m_LastAiMode = ReadInt();
            obj.m_ArrowsCount = ReadInt();

            for (int i = 0; i < obj.m_ArrowsCount; i++)
            {
                obj.m_Arrows.Add(ReadArrow());
            }

            return obj;
        }

        public DataStr.AnimalKilled ReadAnimalCorpse()
        {
            DataStr.AnimalKilled obj = new DataStr.AnimalKilled();
            obj.m_Position = ReadVector3();
            obj.m_Rotation = ReadQuaternion();
            obj.m_PrefabName = ReadString();
            obj.m_GUID = ReadString();
            obj.m_LevelGUID = ReadString();
            obj.m_CreatedTime = ReadInt();

            obj.m_Meat = ReadFloat();
            obj.m_Guts = ReadInt();
            obj.m_Hide = ReadInt();

            obj.m_Knocked = ReadBool();

            obj.m_RegionGUID = ReadString();
            return obj;
        }

#endregion

        public void Write(Supporters.SupporterBenefits obj)
        {
            Write(obj.m_Knife);
            Write(obj.m_BrightNick);
            Write(obj.m_Flairs);
            
        }
        public Supporters.SupporterBenefits ReadSupporterBenefits()
        {
            Supporters.SupporterBenefits obj = new Supporters.SupporterBenefits();
            obj.m_Knife = ReadBool();
            obj.m_BrightNick = ReadBool();
            obj.m_Flairs = ReadIntList();


            return obj;
        }

        public void Write(WeatherVolunteerData obj)
        {
            Write(obj.WeatherType);
            Write(obj.WeatherDuration);
            Write(obj.CurrentRegion);
            Write(obj.StageDuration);
            Write(obj.StageTransition);
            Write(obj.SetIndex);
            Write(obj.HighMin);
            Write(obj.HighMax);
            Write(obj.LowMin);
            Write(obj.LowMax);
            Write(obj.CoolingHours);
            Write(obj.WarmingHours);
            Write(obj.PreviousStage);
        }
        public WeatherVolunteerData ReadWeatherVolunteerData()
        {
            WeatherVolunteerData obj = new WeatherVolunteerData();
            obj.WeatherType = ReadInt();
            obj.WeatherDuration = ReadFloat();
            obj.CurrentRegion = ReadInt();
            obj.StageDuration = ReadFloatList();
            obj.StageTransition = ReadFloatList();
            obj.SetIndex = ReadInt();
            obj.HighMin = ReadFloat();
            obj.HighMax = ReadFloat();
            obj.LowMin = ReadFloat();
            obj.LowMax = ReadFloat();
            obj.CoolingHours = ReadInt();
            obj.WarmingHours = ReadInt();
            obj.PreviousStage = ReadInt();

            return obj;
        }

        public void Write(ExpeditionManager.ExpeditionInvite obj)
        {
            Write(obj.m_InviterMAC);
            Write(obj.m_InviterName);
            Write(obj.m_PersonToInviteMAC);
        }

        public ExpeditionManager.ExpeditionInvite ReadExpeditionInvite()
        {
            ExpeditionManager.ExpeditionInvite obj = new ExpeditionManager.ExpeditionInvite();
            obj.m_InviterMAC = ReadString();
            obj.m_InviterName = ReadString();
            obj.m_PersonToInviteMAC = ReadString();
            return obj;
        }

        public void Write(SlicedBase64Data obj)
        {
            Write(obj.m_Slice);
            Write(obj.m_Slices);
            Write(obj.m_SliceNum);
            Write(obj.m_CheckSum);
            Write(obj.m_GUID);
            Write(obj.m_Purpose);
        }

        public SlicedBase64Data ReadSlicedBase64Data()
        {
            SlicedBase64Data obj = new SlicedBase64Data();
            obj.m_Slice = ReadString();
            obj.m_Slices = ReadInt();
            obj.m_SliceNum = ReadInt();
            obj.m_CheckSum = ReadLong();
            obj.m_GUID = ReadString();
            obj.m_Purpose = ReadInt();
            return obj;
        }

        public void Write(FakeRockCacheVisualData obj)
        {
            Write(obj.m_GUID);
            Write(obj.m_LevelGUID);
            Write(obj.m_Owner);
            Write(obj.m_Position);
            Write(obj.m_Rotation);
        }

        public FakeRockCacheVisualData ReadFakeRockCache()
        {
            FakeRockCacheVisualData obj = new FakeRockCacheVisualData();
            obj.m_GUID = ReadString();
            obj.m_LevelGUID = ReadString();
            obj.m_Owner = ReadString();
            obj.m_Position = ReadVector3();
            obj.m_Rotation = ReadQuaternion();
            return obj;
        }

        public void Write(UniversalSyncableObject obj)
        {
            Write(obj.m_Prefab);
            Write(obj.m_Scene);
            Write(obj.m_GUID);
            Write(obj.m_Position);
            Write(obj.m_Rotation);
            Write(obj.m_CreationTime);
            Write(obj.m_RemoveTime);
            Write(obj.m_ExpeditionBelong);
        }

        public UniversalSyncableObject ReadUniversalSyncable()
        {
            UniversalSyncableObject obj = new UniversalSyncableObject();
            obj.m_Prefab = ReadString();
            obj.m_Scene = ReadString();
            obj.m_GUID = ReadString();
            obj.m_Position = ReadVector3();
            obj.m_Rotation = ReadQuaternion();
            obj.m_CreationTime = ReadInt();
            obj.m_RemoveTime = ReadInt();
            obj.m_ExpeditionBelong = ReadString();
            return obj;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool _disposing)
        {
            if (!disposed)
            {
                if (_disposing)
                {
                    buffer = null;
                    readableBuffer = null;
                    readPos = 0;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
