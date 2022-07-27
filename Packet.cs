using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using MelonLoader;
using SkyCoop;

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
        SLEEPPOSE,
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
        SLEEPPOSE,
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
    }

    public class Packet : IDisposable
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

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
            Write(_value.x);
            Write(_value.y);
            Write(_value.z);
        }
        /// <summary>Adds a Quaternion to the packet.</summary>
        /// <param name="_value">The Quaternion to add.</param>
        public void Write(Quaternion _value)
        {
            Write(_value.x);
            Write(_value.y);
            Write(_value.z);
            Write(_value.w);
        }
        public void Write(MyMod.WeatherProxies weather)
        {
            Write(weather.m_WeatherProxy);
            Write(weather.m_WeatherTransitionProxy);
            Write(weather.m_WindProxy);
        }
        public void Write(StackableItem stackable)
        {
            Write(stackable.m_Units);
        }
        public void Write(FoodItem food)
        {
            Write(food.m_CaloriesTotal);
            Write(food.m_CaloriesRemaining);
            Write(food.m_HeatPercent);
            Write(food.m_Packaged);
            Write(food.m_Opened);
        }
        public void Write(WaterSupply water)
        {
            float save = water.m_VolumeInLiters;

            if (save > 0.5f)
            {
                water.m_VolumeInLiters = 0.5f;
                Write(water.m_VolumeInLiters);
                water.m_VolumeInLiters = save;
            }else
            {
                Write(water.m_VolumeInLiters);
            }
        }
        public void Write(EvolveItem evo)
        {
            Write(evo.m_TimeSpentEvolvingGameHours);
        }

        public void Write(GearItem gear)
        {
            Write(gear.Serialize());
        }

        public void Write(MyMod.GearItemDataPacket gear)
        {
            Write(gear.m_GearName);
            Write(gear.m_DataProxy);
            Write(gear.m_Water);
            Write(gear.m_SendedTo);
        }
        public void Write(MyMod.DroppedGearItemDataPacket gear)
        {
            Write(gear.m_GearID);
            Write(gear.m_Position);
            Write(gear.m_Rotation);
            Write(gear.m_LevelID);
            Write(gear.m_LevelGUID);
            Write(gear.m_Hash);
            Write(gear.m_Extra);
        }
        public void Write(MyMod.ShowShelterByOther shelter)
        {
            Write(shelter.m_Position);
            Write(shelter.m_Rotation);
            Write(shelter.m_LevelID);
            Write(shelter.m_LevelGUID);
        }

        //public void Write(GearItem gear)
        //{
        //    Write(gear.m_GearName);
        //    Write(gear.m_CurrentHP);
        //    Write(gear.m_WeightKG);
        //    if (gear.m_FoodItem != null)
        //    {
        //        Write(gear.m_FoodItem);
        //    }else{
        //        Write(new FoodItem());
        //    }
        //    if (gear.m_EvolveItem != null)
        //    {
        //        Write(gear.m_EvolveItem);
        //    }else{
        //        Write(new EvolveItem());
        //    }
        //    Write(gear.m_GearBreakConditionThreshold);
        //    if (gear.m_WaterSupply != null)
        //    {
        //        Write(gear.m_WaterSupply);
        //    }
        //    else
        //    {
        //        Write(new WaterSupply());
        //    }
        //    //Write(gear.m_StackableItem);
        //}
        public void Write(ObjectGuid _guid, GameObject obj)
        {
            if(_guid == null)
            {
                string set_guid = ObjectGuidManager.GenerateNewGuidString();
                Utils.SetGuidForGameObject(obj.gameObject, set_guid);
                _guid = obj.gameObject.GetComponent<ObjectGuid>();
            }
            Write(_guid.m_Guid);
        }

        public void Write(GameObject obj)
        {
            Write(obj.transform.position);
            Write(obj.transform.rotation);
            Write(obj.gameObject.GetComponent<ObjectGuid>(), obj);
            Write(obj.gameObject.name);
        }
        public void Write(MyMod.AnimalSync obj)
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

        public void Write(MyMod.AnimalAnimsSync obj)
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

        public void Write(MyMod.AnimalTrigger obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_Trigger);
        }

        public void Write(MyMod.ShootSync obj)
        {
            Write(obj.m_position);
            Write(obj.m_rotation);
            Write(obj.m_projectilename);
            Write(obj.m_skill);
            Write(obj.m_camera_forward);
            Write(obj.m_camera_right);
            Write(obj.m_camera_up);
        }

        public void Write(MyMod.HarvestStats obj)
        {
            Write(obj.m_Meat);
            Write(obj.m_Guts);
            Write(obj.m_Hide);
            Write(obj.m_Guid);
        }

        public void Write(MyMod.SaveSlotSync obj)
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

        public void Write(MyMod.ContainerOpenSync obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_State);
            Write(obj.m_LevelID);
            Write(obj.m_LevelGUID);
            Write(obj.m_Inspected);
        }

        public void Write(MyMod.WalkTracker obj)
        {
            Write(obj.m_levelid);
            Write(obj.m_V3);
        }
        public void Write(MyMod.AnimalAligner obj)
        {
            Write(obj.m_Proxy);
            Write(obj.m_Guid);
        }
        public void Write(MyMod.PlayerEquipmentData obj)
        {
            Write(obj.m_Arrows);
            Write(obj.m_HasAxe);
            Write(obj.m_HasMedkit);
            Write(obj.m_HasRevolver);
            Write(obj.m_HasRifle);
            Write(obj.m_Flares);
            Write(obj.m_BlueFlares);
        }
        public void Write(MyMod.MultiplayerChatMessage obj)
        {
            //Write(obj.m_By);
            //Write(obj.m_Message);
            WriteUnicodeString(obj.m_By);
            WriteUnicodeString(obj.m_Message);
            Write(obj.m_Type);
        }
        public void Write(MyMod.MultiPlayerClientStatus obj)
        {
            Write(obj.m_ID);
            Write(obj.m_Name);
            Write(obj.m_Sleep);
            Write(obj.m_Dead);
        }
        public void Write(MyMod.PlayerClothingData obj)
        {
            Write(obj.m_Hat);
            Write(obj.m_Top);
            Write(obj.m_Bottom);
            Write(obj.m_Boots);
            Write(obj.m_Scarf);
            Write(obj.m_Balaclava);
        }
        public void Write(MyMod.BrokenFurnitureSync obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_ParentGuid);
            Write(obj.m_LevelID);
            Write(obj.m_LevelGUID);
        }
        public void Write(MyMod.PickedGearSync obj)
        {
            Write(obj.m_Spawn);
            Write(obj.m_LevelID);
            Write(obj.m_LevelGUID);
            Write(obj.m_MyInstanceID);
        }
        public void Write(MyMod.ClimbingRopeSync obj)
        {
            Write(obj.m_Position);
            Write(obj.m_LevelID);
            Write(obj.m_LevelGUID);
            Write(obj.m_Deployed);
            Write(obj.m_Snapped);
        }
        public void Write(MyMod.ServerConfigData obj)
        {
            Write(obj.m_FastConsumption);
            Write(obj.m_DuppedSpawns);
            Write(obj.m_DuppedContainers);
            Write(obj.m_PlayersSpawnType);
            Write(obj.m_FireSync);
            Write(obj.m_CheatsMode);
        }
        public void Write(MyMod.HarvestableSyncData obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_State);
        }
        public void Write(MyMod.FireSourcesSync obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_LevelGUID);
            Write(obj.m_LevelId);
            Write(obj.m_Position);
            Write(obj.m_Rotation);
            Write(obj.m_IsCampfire);
            Write(obj.m_FuelName);
        }
        public void Write(MyMod.SlicedJsonData obj)
        {
            Write(obj.m_GearName);
            Write(obj.m_Hash);
            Write(obj.m_Str);
            Write(obj.m_SendTo);
            Write(obj.m_Last);
            Write(obj.m_Extra);
        }
        public void Write(MyMod.SlicedBytesData obj)
        {
            Write(obj.m_Hash);
            Write(obj.m_Action);
            Write(obj.m_SendTo);
            Write(obj.m_Last);
            Write(obj.m_Length);
            Write(obj.m_ExtraInt);
            Write(obj.m_Data);
        }
        public void Write(MyMod.ExtraDataForDroppedGear obj)
        {
            Write(obj.m_DroppedTime);
            Write(obj.m_GoalTime);
            Write(obj.m_Dropper);
            Write(obj.m_Variant);
            Write(obj.m_GearName);
        }
        public void Write(MyMod.AffictionSync obj)
        {
            Write(obj.m_Type);
            Write(obj.m_Location);
            WriteUnicodeString(obj.m_Case);
            Write(obj.m_ShouldBeTreated);
        }

        public void Write(MyMod.AnimalCompactData obj)
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
        }
        public void Write(MyMod.AnimalKilled obj)
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

        public void Write(DecalProjectorInstance obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_DecalName);
            Write((int)obj.m_ProjectileType);
            Write(obj.m_Indoors);
            Write(obj.m_Pos);
            Write(obj.m_Normal);
            Write(obj.m_Yaw);
            Write(obj.m_Alpha);
            Write(obj.m_ColorTint.r);
            Write(obj.m_ColorTint.g);
            Write(obj.m_ColorTint.b);
            Write(obj.m_RevealAmount);
            Write(obj.m_RevealedOnMap);
        }

        public void Write(List<int> LIST)
        {
            Write(LIST.Count);
            for (int i = 0; i < LIST.Count; i++)
            {
                Write(LIST[i]);
            }
        }

        public void Write(MyMod.CustomChallengeData DAT)
        {
            Write(DAT.m_CurrentTask);
            Write(DAT.m_Time);
            Write(DAT.m_Started);
            Write(DAT.m_Done);
        }
        public MyMod.CustomChallengeData ReadChallengeData()
        {
            MyMod.CustomChallengeData obj = new MyMod.CustomChallengeData();
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

        public DecalProjectorInstance ReadSprayDecal()
        {
            DecalProjectorInstance obj = new DecalProjectorInstance();
            obj.m_Guid = ReadString();
            obj.m_DecalName = ReadString();
            obj.m_ProjectileType = (ProjectileType)ReadInt();
            obj.m_Indoors = ReadBool();
            obj.m_Pos = ReadVector3();
            obj.m_Normal = ReadVector3();
            obj.m_Yaw = ReadFloat();
            obj.m_Alpha = ReadFloat();
            obj.m_ColorTint = new Color(ReadFloat(), ReadFloat(), ReadFloat());
            obj.m_RevealAmount = ReadFloat();
            obj.m_RevealedOnMap = ReadBool();

            return obj;
        }

        public void Write(MyMod.DeathContainerData obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_LevelKey);
            Write(obj.m_Position);
            Write(obj.m_Rotation);
            Write(obj.m_Owner);
            Write(obj.m_ContainerPrefab);
        }
        public MyMod.DeathContainerData ReadDeathContainer()
        {
            MyMod.DeathContainerData obj = new MyMod.DeathContainerData();
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
                    Console.WriteLine("Could not read value of type 'int'! readableBuffer is null!!!");
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
                Console.WriteLine("Could not read value of type 'int'!");
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
        public MyMod.WeatherProxies ReadWeather(bool _moveReadPos = true)
        {
            MyMod.WeatherProxies weather = new MyMod.WeatherProxies();

            weather.m_WeatherProxy = ReadString(_moveReadPos);
            weather.m_WeatherTransitionProxy = ReadString(_moveReadPos);
            weather.m_WindProxy = ReadString(_moveReadPos);

            return weather;
        }

        public StackableItem ReadStackable(bool _moveReadPos = true)
        {
            StackableItem stack = new StackableItem();
            stack.m_Units = ReadInt(_moveReadPos);

            return stack;
        }
        public FoodItem ReadFoodItem(bool _moveReadPos = true)
        {
            FoodItem food = new FoodItem();
            food.m_CaloriesTotal = ReadFloat();
            food.m_CaloriesRemaining = ReadFloat();
            food.m_HeatPercent = ReadFloat();
            food.m_Packaged = ReadBool();
            food.m_Opened = ReadBool();

            return food;
        }
        public WaterSupply ReadWater(bool _moveReadPos = true)
        {
            WaterSupply water = new WaterSupply();
            water.m_VolumeInLiters = ReadFloat();

            return water;
        }
        public EvolveItem ReadEvoItem(bool _moveReadPos = true)
        {
            EvolveItem evo = new EvolveItem();
            evo.m_TimeSpentEvolvingGameHours = ReadFloat();

            return evo;
        }

        public GearItem ReadGearSimple (bool _moveReadPos = true)
        {
            GearItem gear = new GearItem();

            gear.m_GearName = ReadString();
            gear.m_CurrentHP = ReadFloat();
            gear.m_WeightKG = ReadFloat();
            gear.m_FoodItem = ReadFoodItem();
            gear.m_EvolveItem = ReadEvoItem();
            gear.m_GearBreakConditionThreshold = ReadFloat();
            gear.m_WaterSupply = ReadWater();
            //gear.m_StackableItem = ReadStackable(_moveReadPos);

            return gear;
        }

        public ObjectGuid ReadObjectGuid(bool _moveReadPos = true)
        {
            ObjectGuid objguid = new ObjectGuid();

            objguid.m_Guid = ReadString();

            return objguid;
        }

        public GameObject ReadObject(bool _moveReadPos = true)
        {
            GameObject obj = new GameObject();

            obj.transform.position = ReadVector3();
            obj.transform.rotation = ReadQuaternion();
            obj.AddComponent<ObjectGuid>();
            ObjectGuid objguid = new ObjectGuid();
            objguid = ReadObjectGuid();
            obj.GetComponent<ObjectGuid>().m_Guid = objguid.m_Guid;
            obj.gameObject.name = ReadString();

            return obj;
        }

        public MyMod.AnimalSync ReadAnimal(bool _moveReadPos = true)
        {
            MyMod.AnimalSync obj = new MyMod.AnimalSync();
            obj.m_position = ReadVector3();
            obj.m_rotation = ReadQuaternion();
            obj.m_guid = ReadString();
            obj.m_name = ReadString();
            obj.m_Hp = ReadFloat();
            obj.m_Bleeding = ReadBool();

            obj.m_Controller = ReadInt();
            obj.m_ProxySave = ReadString();
            obj.m_LevelD = ReadInt();
            obj.m_SpawnRegionGUID = ReadString();

            return obj;
        }

        public MyMod.AnimalAnimsSync ReadAnimalAnim(bool _moveReadPos = true)
        {
            MyMod.AnimalAnimsSync obj = new MyMod.AnimalAnimsSync();

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

        public MyMod.AnimalTrigger ReadAnimalTrigger(bool _moveReadPos = true)
        {
            MyMod.AnimalTrigger obj = new MyMod.AnimalTrigger();
            obj.m_Guid = ReadString();
            obj.m_Trigger = ReadInt();
            return obj;
        }

        public MyMod.ShootSync ReadShoot(bool _moveReadPos = true)
        {
            MyMod.ShootSync obj = new MyMod.ShootSync();
            obj.m_position = ReadVector3();
            obj.m_rotation = ReadQuaternion();
            obj.m_projectilename = ReadString();
            obj.m_skill = ReadFloat();
            obj.m_camera_forward = ReadVector3();
            obj.m_camera_right = ReadVector3();
            obj.m_camera_up = ReadVector3();
            return obj;
        }
        public MyMod.HarvestStats ReadHarvest(bool _moveReadPos = true)
        {
            MyMod.HarvestStats obj = new MyMod.HarvestStats();
            obj.m_Meat = ReadFloat();
            obj.m_Guts = ReadInt();
            obj.m_Hide = ReadInt();
            obj.m_Guid = ReadString();
            return obj;
        }

        public MyMod.SaveSlotSync ReadSaveSlot(bool _moveReadPos = true)
        {
            MyMod.SaveSlotSync obj = new MyMod.SaveSlotSync();

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

        public MyMod.ContainerOpenSync ReadContainer(bool _moveReadPos = true)
        {
            MyMod.ContainerOpenSync obj = new MyMod.ContainerOpenSync();

            obj.m_Guid = ReadString();
            obj.m_State = ReadBool();
            obj.m_LevelID = ReadInt();
            obj.m_LevelGUID = ReadString();
            obj.m_Inspected = ReadBool();
            return obj;
        }

        public MyMod.WalkTracker ReadWalkTracker(bool _moveReadPos = true)
        {
            MyMod.WalkTracker obj = new MyMod.WalkTracker();

            obj.m_levelid = ReadInt();
            obj.m_V3 = ReadVector3();
            return obj;
        }

        public MyMod.AnimalAligner ReadAnimalAligner(bool _moveReadPos = true)
        {
            MyMod.AnimalAligner obj = new MyMod.AnimalAligner();
            obj.m_Proxy = ReadString();
            obj.m_Guid = ReadString();
            return obj;
        }

        public MyMod.PlayerEquipmentData ReadEQ(bool _moveReadPos = true)
        {
            MyMod.PlayerEquipmentData obj = new MyMod.PlayerEquipmentData();
            obj.m_Arrows = ReadInt();
            obj.m_HasAxe = ReadBool();
            obj.m_HasMedkit = ReadBool();
            obj.m_HasRevolver = ReadBool();
            obj.m_HasRifle = ReadBool();
            obj.m_Flares = ReadInt();
            obj.m_BlueFlares = ReadInt();

            return obj;
        }
        public MyMod.MultiplayerChatMessage ReadChat()
        {
            MyMod.MultiplayerChatMessage obj = new MyMod.MultiplayerChatMessage();
            obj.m_By = ReadUnicodeString();
            obj.m_Message = ReadUnicodeString();
            obj.m_Type = ReadInt();

            return obj;
        }

        public MyMod.MultiPlayerClientStatus ReadClientStatus()
        {
            MyMod.MultiPlayerClientStatus obj = new MyMod.MultiPlayerClientStatus();
            obj.m_ID = ReadInt();
            obj.m_Name = ReadString();
            obj.m_Sleep = ReadBool();
            obj.m_Dead = ReadBool();

            return obj;
        }
        public MyMod.PlayerClothingData ReadClothingData()
        {
            MyMod.PlayerClothingData obj = new MyMod.PlayerClothingData();
            obj.m_Hat = ReadString();
            obj.m_Top = ReadString();
            obj.m_Bottom = ReadString();
            obj.m_Boots = ReadString();
            obj.m_Scarf = ReadString();
            obj.m_Balaclava = ReadString();

            return obj;
        }

        public MyMod.BrokenFurnitureSync ReadFurn()
        {
            MyMod.BrokenFurnitureSync obj = new MyMod.BrokenFurnitureSync();
            obj.m_Guid = ReadString();
            obj.m_ParentGuid = ReadString();
            obj.m_LevelID = ReadInt();
            obj.m_LevelGUID = ReadString();

            return obj;
        }

        public MyMod.PickedGearSync ReadPickedGear()
        {
            MyMod.PickedGearSync obj = new MyMod.PickedGearSync();
            obj.m_Spawn = ReadVector3();
            obj.m_LevelID = ReadInt();
            obj.m_LevelGUID = ReadString();
            obj.m_MyInstanceID = ReadInt();

            return obj;
        }

        public MyMod.ClimbingRopeSync ReadRope()
        {
            MyMod.ClimbingRopeSync obj = new MyMod.ClimbingRopeSync();
            obj.m_Position = ReadVector3();
            obj.m_LevelID = ReadInt();
            obj.m_LevelGUID = ReadString();
            obj.m_Deployed = ReadBool();
            obj.m_Snapped = ReadBool();

            return obj;
        }

        public MyMod.ServerConfigData ReadServerCFG()
        {
            MyMod.ServerConfigData obj = new MyMod.ServerConfigData();
            obj.m_FastConsumption = ReadBool();
            obj.m_DuppedSpawns = ReadBool();
            obj.m_DuppedContainers = ReadBool();
            obj.m_PlayersSpawnType = ReadInt();
            obj.m_FireSync = ReadInt();
            obj.m_CheatsMode = ReadInt();

            return obj;
        }
        public MyMod.HarvestableSyncData ReadHarvestablePlant()
        {
            MyMod.HarvestableSyncData obj = new MyMod.HarvestableSyncData();
            obj.m_Guid = ReadString();
            obj.m_State = ReadString();

            return obj;
        }

        public MyMod.GearItemDataPacket ReadGearData()
        {
            MyMod.GearItemDataPacket obj = new MyMod.GearItemDataPacket();
            obj.m_GearName = ReadString();
            obj.m_DataProxy = ReadString();
            obj.m_Water = ReadFloat();
            obj.m_SendedTo = ReadInt();
            
            return obj;
        }
        public MyMod.DroppedGearItemDataPacket ReadDroppedGearData()
        {
            MyMod.DroppedGearItemDataPacket obj = new MyMod.DroppedGearItemDataPacket();
            obj.m_GearID = ReadInt();
            obj.m_Position = ReadVector3();
            obj.m_Rotation = ReadQuaternion();
            obj.m_LevelID = ReadInt();
            obj.m_LevelGUID = ReadString();
            obj.m_Hash = ReadInt();
            obj.m_Extra = ReadExtraDataForDropperGear();

            return obj;
        }
        public MyMod.ShowShelterByOther ReadShelter()
        {
            MyMod.ShowShelterByOther obj = new MyMod.ShowShelterByOther();
            obj.m_Position = ReadVector3();
            obj.m_Rotation = ReadQuaternion();
            obj.m_LevelID = ReadInt();
            obj.m_LevelGUID = ReadString();

            return obj;
        }
        public MyMod.FireSourcesSync ReadFire()
        {
            MyMod.FireSourcesSync obj = new MyMod.FireSourcesSync();
            obj.m_Guid = ReadString();
            obj.m_LevelGUID = ReadString();
            obj.m_LevelId = ReadInt();
            obj.m_Position = ReadVector3();
            obj.m_Rotation = ReadQuaternion();
            obj.m_IsCampfire = ReadBool();
            obj.m_FuelName = ReadString();

            return obj;
        }

        public MyMod.SlicedJsonData ReadSlicedGear()
        {
            MyMod.SlicedJsonData obj = new MyMod.SlicedJsonData();
            obj.m_GearName = ReadString();
            obj.m_Hash = ReadInt();
            obj.m_Str = ReadString();
            obj.m_SendTo = ReadInt();
            obj.m_Last = ReadBool();
            obj.m_Extra = ReadExtraDataForDropperGear();
            return obj;
        }
        public MyMod.SlicedBytesData ReadSlicedBytes()
        {
            MyMod.SlicedBytesData obj = new MyMod.SlicedBytesData();
            obj.m_Hash = ReadInt();
            obj.m_Action = ReadString();
            obj.m_SendTo = ReadInt();
            obj.m_Last = ReadBool();
            obj.m_Length = ReadInt();
            obj.m_ExtraInt = ReadInt();
            obj.m_Data = ReadBytes(obj.m_Length);
            return obj;
        }

        public MyMod.ExtraDataForDroppedGear ReadExtraDataForDropperGear()
        {
            MyMod.ExtraDataForDroppedGear obj = new MyMod.ExtraDataForDroppedGear();
            obj.m_DroppedTime = ReadInt();
            obj.m_GoalTime = ReadInt();
            obj.m_Dropper = ReadString();
            obj.m_Variant = ReadInt();
            obj.m_GearName = ReadString();
            return obj;
        }
        public MyMod.AffictionSync ReadAffiction()
        {
            MyMod.AffictionSync obj = new MyMod.AffictionSync();
            obj.m_Type = ReadInt();
            obj.m_Location = ReadInt();
            obj.m_Case = ReadUnicodeString();
            obj.m_ShouldBeTreated = ReadBool();
            return obj;
        }

        public MyMod.AnimalCompactData ReadAnimalCompactData()
        {
            MyMod.AnimalCompactData obj = new MyMod.AnimalCompactData();
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

            return obj;
        }

        public MyMod.AnimalKilled ReadAnimalCorpse()
        {
            MyMod.AnimalKilled obj = new MyMod.AnimalKilled();
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
