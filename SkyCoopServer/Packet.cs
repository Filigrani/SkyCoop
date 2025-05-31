using System.Numerics;
using LiteNetLib.Utils;

namespace SkyCoopServer
{
    public static class Packet
    {
        public enum Type
        {
            Welcome = 0,
            CFG,
            ClientPosition,
            ClientRotation,
            ClientScene,
            ClientHoldigGear,
            ClientCrouch,
            ClientAction,
            ClientFire,
            ClientDamageOtherClient,
            ClientProjectile,
            ClientDied,
            ClientRevived,
            KillFeedMessage,
            ClientProjectileThrow,
            ClientName,
            ClientRequestRespawn,
            ClientInjectedItem,
            ClientRemoveInjectedItem,
            ClientEraceAllInjectedItems,
            ClientGettingDamage,
            ClientSendGear,
            ClientPickUpGear,
            ClientRemoveGear,
            ClientLoadedScene,
            ClientOpenableInteraction,
            ClientClothing,
            ClientZoneUpdated,
            ClientGameModeTimer,
            ClientHUDSideBar,
            ClientHUDSideBarUpdate,
            ClientFreeze,
            ServerConfigUpdated,
            ServerChangesMap,
            ServerLeaders,
            ClientTryInteract,
            ClientVehicleSeat,
            ClientInVehicle,
            ClientStatusMessage,
            ClientDeathPackAdded,
            ClientDeathPackRemoved,
            ClientContainerOpen,
            ClientUpdateContainerData,
            ServerContainerDataArrived,
            ClientFinishInteract,
            ClientSetInteraction,
            ClientContainerStateUpdated,
            ClientHUDTimerPrefix,
            ClientRespawnAsSpectator,
        }

        public static void Put(this NetDataWriter Writer, Vector3 v3)
        {
            Writer.Put(v3.X);
            Writer.Put(v3.Y);
            Writer.Put(v3.Z);
        }

        public static Vector3 GetVector3(this NetDataReader Reader)
        {
            Vector3 v3 = new Vector3(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
            return v3;
        }

        public static void Put(this NetDataWriter Writer, Quaternion quat)
        {
            Writer.Put(quat.X);
            Writer.Put(quat.Y);
            Writer.Put(quat.Z);
            Writer.Put(quat.W);
        }

        public static void Put(this NetDataWriter Writer, DataStr.KillFeedMessage Message)
        {
            Writer.Put(Message.m_Killer);
            Writer.Put(Message.m_Victim);
            Writer.Put(Message.m_Assist);
            Writer.Put((int)Message.m_DeathReason);
            Writer.Put(Message.m_Flags.Count);
            foreach (DataStr.KillFeedFlag Flag in Message.m_Flags)
            {
                Writer.Put((int)Flag);
            }
        }

        public static DataStr.KillFeedMessage GetKillFeedMessage(this NetDataReader Reader)
        {
            DataStr.KillFeedMessage Message = new DataStr.KillFeedMessage();
            Message.m_Killer = Reader.GetInt();
            Message.m_Victim = Reader.GetInt();
            Message.m_Assist = Reader.GetInt();
            Message.m_DeathReason = (DataStr.DamageType)Reader.GetInt();
            int Flags = Reader.GetInt();
            if(Flags > 0)
            {
                for (int i = 0; i < Flags; i++)
                {
                    Message.m_Flags.Add((DataStr.KillFeedFlag)Reader.GetInt());
                }
            }
            return Message;
        }

        public static Quaternion GetQuaternion(this NetDataReader Reader)
        {
            Quaternion quat = new Quaternion(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
            return quat;
        }

        public static void Put(this NetDataWriter Writer, DataStr.GearDataVisual Visual)
        {
            Writer.Put(Visual.m_GearName);
            Writer.Put(Visual.m_Position);
            Writer.Put(Visual.m_Rotation);
            Writer.Put(Visual.m_GUID);
        }

        public static DataStr.GearDataVisual GetGearVisual(this NetDataReader Reader)
        {
            DataStr.GearDataVisual Visual = new DataStr.GearDataVisual();

            Visual.m_GearName = Reader.GetString();
            Visual.m_Position = Reader.GetVector3();
            Visual.m_Rotation = Reader.GetQuaternion();
            Visual.m_GUID = Reader.GetString();

            return Visual;
        }

        public static void Put(this NetDataWriter Writer, DataStr.ServerConfig CFG)
        {
            Writer.Put(CFG.m_MaxPlayers);
            Writer.Put(CFG.m_StartingRegion);
            Writer.Put(CFG.m_Seed);
            Writer.Put(CFG.m_VoicePort);
            Writer.Put(CFG.m_ExperienceMode);
            Writer.Put(CFG.m_SceneToSpawn);
            Writer.Put(CFG.m_GameMode);
        }

        public static DataStr.ServerConfig GetConfig(this NetDataReader Reader)
        {
            DataStr.ServerConfig CFG = new DataStr.ServerConfig();

            CFG.m_MaxPlayers = Reader.GetInt();
            CFG.m_StartingRegion = Reader.GetString();
            CFG.m_Seed = Reader.GetInt();
            CFG.m_VoicePort = Reader.GetInt();
            CFG.m_ExperienceMode = Reader.GetString();
            CFG.m_SceneToSpawn = Reader.GetString();
            CFG.m_GameMode = Reader.GetString();

            return CFG;
        }

        public static void Put(this NetDataWriter Writer, DataStr.GameRules Rules)
        {
            Writer.Put(Rules.m_PlayerCanBeKnocked);
            Writer.Put(Rules.m_PVP);
            Writer.Put(Rules.m_StartingItems);
            Writer.Put(Rules.m_HUDMode);
            Writer.Put(Rules.m_DeathPacks);
            Writer.Put(Rules.m_Respawns);
        }

        public static DataStr.GameRules GetRules(this NetDataReader Reader)
        {
            DataStr.GameRules Rules = new DataStr.GameRules();

            Rules.m_PlayerCanBeKnocked = Reader.GetBool();
            Rules.m_PVP = Reader.GetBool();
            Rules.m_StartingItems = Reader.GetStartingGearList();
            Rules.m_HUDMode = Reader.GetString();
            Rules.m_DeathPacks = Reader.GetBool();
            Rules.m_Respawns = Reader.GetBool();

            return Rules;
        }

        public static void Put(this NetDataWriter Writer, DataStr.StartingGearData GearData)
        {
            Writer.Put(GearData.Variants.Count);

            foreach (string GearVariant in GearData.Variants)
            {
                Writer.Put(GearVariant);
            }
            Writer.Put(GearData.Units);
        }

        public static DataStr.StartingGearData GetStartingGear(this NetDataReader Reader)
        {
            DataStr.StartingGearData GearData = new DataStr.StartingGearData();
            int Count = Reader.GetInt();
            GearData.Variants = new List<string>();
            for (int i = 0; i < Count; i++)
            {
                GearData.Variants.Add(Reader.GetString());
            }
            GearData.Units = Reader.GetInt();
            return GearData;
        }

        public static void Put(this NetDataWriter Writer, List<DataStr.StartingGearData> GearDataList)
        {
            Writer.Put(GearDataList.Count);

            foreach (DataStr.StartingGearData GearData in GearDataList)
            {
                Writer.Put(GearData);
            }
        }

        public static List<DataStr.StartingGearData> GetStartingGearList(this NetDataReader Reader)
        {
            List<DataStr.StartingGearData> GearDataList = new List<DataStr.StartingGearData> ();
            int Count = Reader.GetInt();
            for (int i = 0; i < Count; i++)
            {
                GearDataList.Add(Reader.GetStartingGear());
            }
            return GearDataList;
        }

        public static void Put(this NetDataWriter Writer, DataStr.ClothingData Data)
        {
            Writer.Put(Data.m_Hat1);
            Writer.Put(Data.m_Hat2);

            Writer.Put(Data.m_Body);
            Writer.Put(Data.m_Gloves);

            Writer.Put(Data.m_Pants);
            Writer.Put(Data.m_Boots);

            Writer.Put(Data.m_Accs1);
            Writer.Put(Data.m_Accs2);

            Writer.Put(Data.m_Hat1Damage);
            Writer.Put(Data.m_Hat2Damage);

            Writer.Put(Data.m_BodyDamage);
            Writer.Put(Data.m_GlovesDamage);

            Writer.Put(Data.m_PantsDamage);
            Writer.Put(Data.m_BootsDamage);

            Writer.Put(Data.m_TechPack);
        }

        public static DataStr.ClothingData GetClothingData(this NetDataReader Reader)
        {
            DataStr.ClothingData Data = new DataStr.ClothingData();

            Data.m_Hat1 = Reader.GetString();
            Data.m_Hat2 = Reader.GetString();

            Data.m_Body = Reader.GetString();
            Data.m_Gloves = Reader.GetString();

            Data.m_Pants = Reader.GetString();
            Data.m_Boots = Reader.GetString();

            Data.m_Accs1 = Reader.GetString();
            Data.m_Accs2 = Reader.GetString();

            Data.m_Hat1Damage = Reader.GetFloat();
            Data.m_Hat2Damage = Reader.GetFloat();

            Data.m_BodyDamage = Reader.GetFloat();
            Data.m_GlovesDamage = Reader.GetFloat();

            Data.m_PantsDamage = Reader.GetFloat();
            Data.m_BootsDamage = Reader.GetFloat();

            Data.m_TechPack = Reader.GetBool();

            return Data;
        }

        public static void Put(this NetDataWriter Writer, DataStr.DeathPack Data)
        {
            Writer.Put(Data.m_Prefab);
            Writer.Put(Data.m_GUID);
            Writer.Put(Data.m_Owner);

            Writer.Put(Data.m_Position);
            Writer.Put(Data.m_Rotation);
        }

        public static DataStr.DeathPack GetDeathPack(this NetDataReader Reader)
        {
            DataStr.DeathPack Data = new DataStr.DeathPack();

            Data.m_Prefab = Reader.GetString();
            Data.m_GUID = Reader.GetString();
            Data.m_Owner = Reader.GetString();

            Data.m_Position = Reader.GetVector3();
            Data.m_Rotation = Reader.GetQuaternion();

            return Data;
        }
    }
}
