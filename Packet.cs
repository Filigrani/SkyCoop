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
        //MAKEFIRE,
        ANIMSTATE,
        HASRIFLE,
        HASREVOLVER,
        SLEEPHOURS,
        SYNCWEATHER,
        REVIVE,
        REVIVEDONE,
        HASAXE,
        HISARROWS,
        HASMEDKIT,
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
        //MAKEFIRE,
        ANIMSTATE,
        HASRIFLE,
        HASREVOLVER,
        SLEEPHOURS,
        SYNCWEATHER,
        REVIVE,
        REVIVEDONE,
        HASAXE,
        HISARROWS,
        HASMEDKIT,
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
            Write(_value.Length); // Add the length of the string to the packet
            buffer.AddRange(Encoding.ASCII.GetBytes(_value)); // Add the string itself
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
            Write(gear.m_GearName);
            Write(gear.m_CurrentHP);
            Write(gear.m_WeightKG);
            if (gear.m_FoodItem != null)
            {
                Write(gear.m_FoodItem);
            }else{
                Write(new FoodItem());
            }
            if (gear.m_EvolveItem != null)
            {
                Write(gear.m_EvolveItem);
            }else{
                Write(new EvolveItem());
            }
            Write(gear.m_GearBreakConditionThreshold);
            if (gear.m_WaterSupply != null)
            {
                Write(gear.m_WaterSupply);
            }
            else
            {
                Write(new WaterSupply());
            }
            //Write(gear.m_StackableItem);
        }
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
            Write(obj.m_Meat);
            Write(obj.m_Guts);
            Write(obj.m_Hide);
            Write(obj.m_Frozen);

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

            Write(obj.m_UnderYourControl);
            Write(obj.m_ProxySave);
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
        }

        public void Write(MyMod.SaveSlotSync obj)
        {
            Write(obj.m_Episode);
            Write(obj.m_SaveSlotType);
            Write(obj.m_Seed);
            Write(obj.m_ExperienceMode);
            Write(obj.m_Location);
        }

        public void Write(MyMod.ContainerOpenSync obj)
        {
            Write(obj.m_Guid);
            Write(obj.m_State);
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
                throw new Exception("Could not read value of type 'int'!");
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
                string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length); // Convert the bytes to a string
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

        public GearItem ReadGear(bool _moveReadPos = true)
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
            obj.m_Meat = ReadFloat();
            obj.m_Guts = ReadInt();
            obj.m_Hide = ReadInt();
            obj.m_Frozen = ReadFloat();

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
            obj.m_UnderYourControl = ReadBool();
            obj.m_ProxySave = ReadString();

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
            return obj;
        }

        public MyMod.ContainerOpenSync ReadContainer(bool _moveReadPos = true)
        {
            MyMod.ContainerOpenSync obj = new MyMod.ContainerOpenSync();

            obj.m_Guid = ReadString();
            obj.m_State = ReadBool();
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
