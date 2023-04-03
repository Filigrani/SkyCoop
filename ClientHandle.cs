using System;
using UnityEngine;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MelonLoader;
using Harmony;
using GameServer;
using Il2CppSystem.Reflection;
using System.Diagnostics;
using Il2Cpp;
using static SkyCoop.Shared;

namespace SkyCoop
{
    public class ClientHandle
    {
        public static void Welcome(Packet _packet)
        {
            MyMod.RemovePleaseWait();
            MyMod.DiscardRepeatPacket();
            int _myId = _packet.ReadInt();
            int _MaxPlayers = _packet.ReadInt();
            List<string> ModsWhiteList = _packet.ReadStringList();

            MyMod.MaxPlayers = _MaxPlayers;
            Shared.InitAllPlayers();

            MelonLogger.Msg("Welcome to server!");
            ClientUser.myId = _myId;
            MelonLogger.Msg("Host registered me as client "+ _myId);
            MyMod.sendMyPosition = true;
            ClientUser.LastConnectedIp = ClientUser.PendingConnectionIp;
            MyMod.NoHostResponceSeconds = 0;
            MyMod.NeedTryReconnect = false;
            MyMod.TryingReconnect = false;
            WelcomeReceived(ModsWhiteList);
        }
        public static void WelcomeReceived(List<string> ModsWhiteList)
        {
            MyMod.RemovePleaseWait();
            MyMod.DiscardRepeatPacket();
            using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
            {
                _packet.Write(ClientUser.myId);
                _packet.Write(MyMod.MyChatName);
                _packet.Write(MyMod.BuildInfo.Version);
                _packet.Write(Supporters.MyID);
                _packet.Write(Supporters.ConfiguratedBenefits);
                _packet.Write(ModsValidation.GetModsHash(true, ModsWhiteList).m_Hash);

                MyMod.SendUDPData(_packet);
            }
            if (MyMod.level_name != "Empty" && MyMod.level_name != "Boot" && MyMod.level_name != "MainMenu")
            {
                MyMod.SendSpawnData();
            }
        }
        public static void XYZ(Packet _packet)
        {
            Vector3 maboi = _packet.ReadVector3();
            int from = _packet.ReadInt();

            if (MyMod.playersData.Count > 0 && MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_Position = maboi;

                if(MyMod.players[from] != null && MyMod.players[from].GetComponent<Comps.MultiplayerPlayer>() != null)
                {
                    MyMod.LongActionCancleCauseMoved(MyMod.players[from].GetComponent<Comps.MultiplayerPlayer>());
                }
            }
        }
        public static void XYZDW(Packet _packet)
        {

        }
        public static void XYZW(Packet _packet)
        {
            Quaternion maboi = _packet.ReadQuaternion();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_Rotation = maboi;
            }
        }
        public static void BLOCK(Packet _packet)
        {
            Vector3 maboi = _packet.ReadVector3();
            int from = _packet.ReadInt();

            if (MyMod.playersData.Count > 0 && MyMod.playersData[from] != null)
            {
                if (MyMod.playersData[from].m_Levelid == MyMod.levelid)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = maboi;
                }
            }
        }
        public static void LEVELID(Packet _packet)
        {
            int lel = _packet.ReadInt();
            int from = _packet.ReadInt();

            if (MyMod.playersData.Count > 0 && MyMod.playersData[from] != null)
            {
                if(MyMod.playersData[from].m_Levelid != lel)
                {
                    MyMod.playersData[from].m_Levelid = lel;
                    MelonLogger.Msg("Player " + from + " transition to level " + lel);
                }    
            }
            
        }
        public static void LEVELGUID(Packet _packet)
        {
            string lel = _packet.ReadString();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null)
            {
                if(MyMod.playersData[from].m_LevelGuid != lel)
                {
                    MyMod.playersData[from].m_LevelGuid = lel;
                    //MelonLogger.Msg("Player " + from + "  transition to level with GUID " + lel);
                }
            }
        }
        public static void GOTITEM(Packet _packet)
        {
            DataStr.GearItemDataPacket got = _packet.ReadGearData();
            //MelonLogger.Msg(ConsoleColor.Blue, "Someone gave item to" + got.m_SendedTo);
            //MelonLogger.Msg(ConsoleColor.Blue, "Got gear with name [" + got.m_GearName + "] DATA: " + got.m_DataProxy);

            if(got.m_SendedTo == ClientUser.myId)
            {
                //MelonLogger.Msg(ConsoleColor.Blue, "This is item for me");
            }
            MyMod.GiveRecivedItem(got);
        }
        public static void GAMETIME(Packet _packet)
        {
            MyMod.OveridedTime = _packet.ReadString();
            MyMod.MinutesFromStartServer = _packet.ReadInt();
            MyMod.NeedSyncTime = true;

            //MelonLogger.Msg("Time: " + OveridedTime);
        }
        public static void LIGHTSOURCENAME(Packet _packet)
        {
            string item = _packet.ReadString();
            int from = _packet.ReadInt();

            if (MyMod.playersData.Count > 0 && MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_PlayerEquipmentData.m_HoldingItem = item;
            }
        }
        public static void LIGHTSOURCE(Packet _packet)
        {
            bool itemToggle = _packet.ReadBool();
            int from = _packet.ReadInt();

            if (MyMod.playersData.Count > 0 && MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_PlayerEquipmentData.m_LightSourceOn = itemToggle;
            }
        }
        public static void MAKEFIRE(Packet _packet)
        {

        }
        public static void ANIMSTATE(Packet _packet)
        {
            string _anim = _packet.ReadString();
            int from = _packet.ReadInt();
            //MelonLogger.Msg("Other player changed animation: " + AnimState);

            if (MyMod.playersData.Count > 0 && MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_AnimState = _anim;
            }
        }
        public static void EQUIPMENT(Packet _packet)
        {
            DataStr.PlayerEquipmentData item = _packet.ReadEQ();
            int from = _packet.ReadInt();

            if (MyMod.playersData.Count > 0 && MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_PlayerEquipmentData.m_Arrows = item.m_Arrows;
                MyMod.playersData[from].m_PlayerEquipmentData.m_HasAxe = item.m_HasAxe;
                MyMod.playersData[from].m_PlayerEquipmentData.m_HasMedkit = item.m_HasMedkit;
                MyMod.playersData[from].m_PlayerEquipmentData.m_HasRevolver = item.m_HasRevolver;
                MyMod.playersData[from].m_PlayerEquipmentData.m_HasRifle = item.m_HasRifle;
                MyMod.playersData[from].m_PlayerEquipmentData.m_Flares = item.m_Flares;
                MyMod.playersData[from].m_PlayerEquipmentData.m_BlueFlares = item.m_BlueFlares;
            }
        }
        public static void SLEEPHOURS(Packet _packet)
        {
            int sleep = _packet.ReadInt();
            int from = _packet.ReadInt();

            if (MyMod.playersData.Count > 0 && MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_SleepHours = sleep;
            }
        }
        public static void SYNCWEATHER(Packet _packet)
        {
        }
        public static void REVIVE(Packet _packet)
        {
            MyMod.SimRevive();
            if (MyMod.sendMyPosition == true)
            {
                using (Packet __packet = new Packet((int)ClientPackets.REVIVEDONE))
                {
                    __packet.Write(true);
                    MyMod.SendUDPData(__packet);
                }
            }
        }
        public static void ANIMALROLE(Packet _packet)
        {
            bool New = _packet.ReadBool();
            if (MyMod.AnimalsController != New)
            {
                MyMod.AnimalsController = New;
                MyMod.DisableOriginalAnimalSpawns();

                if(MyMod.AnimalsController)
                {
                    MyMod.SwitchToAnimalController();
                }

                MelonLogger.Msg("Got new animal controller role: " + MyMod.AnimalsController);
            }
        }
        public static void ALIGNANIMAL(Packet _packet)
        {

        }
        public static void ASKFORANIMALPROXY(Packet _packet)
        {

        }
        public static void ANIMALDELETE(Packet _packet)
        {
            string AnimalGuid = _packet.ReadString();
            MyMod.DeleteAnimal(AnimalGuid);
        }

        public static void KEEPITALIVE(Packet _packet)
        {
            MyMod.LastResponceTime = Time.time;
            MyMod.NoHostResponceSeconds = 0;
        }

        public static void RQRECONNECT(Packet _packet)
        {
            MyMod.Disconnect();
            ClientUser.DoConnectToIp(ClientUser.LastConnectedIp);
        }
        public static void CHAT(Packet _packet)
        {
            DataStr.MultiplayerChatMessage message = _packet.ReadChat();
            int from = _packet.ReadInt();
            Shared.SendMessageToChat(message, false);
        }
        public static void PLAYERSSTATUS(Packet _packet)
        {
            int howmany = _packet.ReadInt();

            if (howmany > MyMod.MaxPlayers)
            {
                return;
            }

            for (int i = 0; i < MyMod.MaxPlayers; i++)
            {
                if (MyMod.playersData[i] != null)
                {
                    MyMod.playersData[i].m_Used = false;
                }
            }
            List<DataStr.MultiPlayerClientStatus> MPStatus = new List<DataStr.MultiPlayerClientStatus>();
            int Sleepers = 0;
            int Deads = 0;
            for (int i = 1; i <= howmany; i++)
            {
                DataStr.MultiPlayerClientStatus client = _packet.ReadClientStatus();
                MPStatus.Add(client);
                if (MyMod.playersData[client.m_ID] != null)
                {
                    MyMod.playersData[client.m_ID].m_Used = true;
                    MyMod.playersData[client.m_ID].m_Name = client.m_Name;
                    MyMod.playersData[client.m_ID].m_IsLoading = client.m_IsLoading;

                    if (client.m_Dead || client.m_Sleep)
                    {
                        Sleepers = Sleepers + 1;
                    }
                    if (client.m_Dead)
                    {
                        Deads = Deads + 1;
                    }

                    if (client.m_Sleep == true)
                    {
                        MyMod.playersData[client.m_ID].m_SleepHours = 1;
                    }else{
                        MyMod.playersData[client.m_ID].m_SleepHours = 0;
                    }
                    MyMod.playersData[client.m_ID].m_Dead = client.m_Dead;
                }
            }
            Shared.ProcessSleep(Sleepers, howmany, Deads, 0);
            MyMod.UpdatePlayerStatusMenu(MPStatus);
        }
        public static void ANIMALTEST(Packet _packet)
        {
            DataStr.AnimalCompactData dat = _packet.ReadAnimalCompactData();
            DataStr.AnimalAnimsSync anim = _packet.ReadAnimalAnim();
            int from = _packet.ReadInt();
            MyMod.DoAnimalSync(dat, anim);
        }

        public static void ANIMALSYNCTRIGG(Packet _packet)
        {
            if (MyMod.level_name == "Boot" || MyMod.level_name == "Empty")
            {
                return;
            }

            DataStr.AnimalTrigger obj = _packet.ReadAnimalTrigger();
            MyMod.SetAnimalTriggers(obj);
        }

        public static void REVIVEDONE(Packet _packet)
        {
            GameManager.GetInventoryComponent().RemoveGearFromInventory("GEAR_MedicalSupplies_hangar", 1);
        }

        public static void DARKWALKERREADY(Packet _packet)
        {

        }
        public static void HOSTISDARKWALKER(Packet _packet)
        {

        }

        public static void WARDISACTIVE(Packet _packet)
        {

        }
        public static void DWCOUNTDOWN(Packet _packet)
        {

        }
        public static void SHOOTSYNC(Packet _packet)
        {
            DataStr.ShootSync shoot = _packet.ReadShoot();
            int from = _packet.ReadInt();
            //MelonLogger.Msg("Client " + from + " shoot!");

            MyMod.DoShootSync(shoot, from);
        }

        public static void PIMPSKILL(Packet _packet)
        {
            int SkillTypeId = _packet.ReadInt();

            if (SkillTypeId == 1)
            {
                GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Rifle, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                MelonLogger.Msg("Got remote skill upgrade Rifle");
            }
            else if (SkillTypeId == 2)
            {
                GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Revolver, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                MelonLogger.Msg("Got remote skill upgrade Revolver");
            }
        }

        public static void HARVESTINGANIMAL(Packet _packet)
        {
            string GUID = _packet.ReadString();
            int FromWho = _packet.ReadInt();
            if (MyMod.playersData[FromWho] != null)
            {
                MyMod.playersData[FromWho].m_HarvestingAnimal = GUID;
            }
        }

        public static void BULLETDAMAGE(Packet _packet)
        {
            float damage = _packet.ReadFloat();
            int BodyPart = _packet.ReadInt();
            int from = _packet.ReadInt();
            bool Melee = _packet.ReadBool();
            string MeleeWeapon = "";
            if (Melee)
            {
                MeleeWeapon = _packet.ReadString();
            }
            MyMod.DamageByBullet(damage, from, BodyPart, Melee, MeleeWeapon);
        }
        public static void MULTISOUND(Packet _packet)
        {
            string sound = _packet.ReadString();
            int from = _packet.ReadInt();
            MyMod.PlayMultiplayer3dAduio(sound, from);
        }
        public static void CONTAINEROPEN(Packet _packet)
        {
            DataStr.ContainerOpenSync sync = _packet.ReadContainer();
            MyMod.DoSyncContainer(sync);
        }

        public static void LUREPLACEMENT(Packet _packet)
        {

        }
        public static void LUREISACTIVE(Packet _packet)
        {

        }
        public static void SAVEDATA(Packet _packet)
        {
            DataStr.SaveSlotSync SaveData = _packet.ReadSaveSlot();
            MyMod.RemoveWaitForConnect();
            MyMod.ApplyOtherCampfires = true;

            if (MyMod.level_name != "MainMenu")
            {
                return;
            }

            MyMod.PendingSave = SaveData;

            MyMod.CheckHaveSaveFileToJoin(SaveData);
        }
        public static void VERIFYSAVE(Packet _packet)
        {
            MyMod.RemoveWaitForConnect();
            MyMod.RemovePleaseWait();
            MyMod.MyUGUID = _packet.ReadString();
            bool IsValid = _packet.ReadBool();
            if (MyMod.level_name != "MainMenu")
            {
                return;
            }
            MyMod.VerifiedSave(IsValid);
        }
        public static void CARRYBODY(Packet _packet)
        {
        }
        public static void BODYWARP(Packet _packet)
        {
        }
        public static void CHANGENAME(Packet _packet)
        {
            string newName = _packet.ReadString();
            int from = _packet.ReadInt();
            MyMod.playersData[from].m_Name = newName;
        }
        public static void CLOTH(Packet _packet)
        {
            DataStr.PlayerClothingData ClotchData = _packet.ReadClothingData();
            int from = _packet.ReadInt();
            MyMod.playersData[from].m_PlayerClothingData = ClotchData;
            //MelonLogger.Msg("[Clothing] Client "+ from + " Hat " + MyMod.playersData[from].m_PlayerClothingData.m_Hat);
            //MelonLogger.Msg("[Clothing] Client " + from + " Torso "+ MyMod.playersData[from].m_PlayerClothingData.m_Top);
            //MelonLogger.Msg("[Clothing] Client " + from + " Legs "+ MyMod.playersData[from].m_PlayerClothingData.m_Bottom);
            //MelonLogger.Msg("[Clothing] Client " + from + " Feets "+ MyMod.playersData[from].m_PlayerClothingData.m_Boots);
        }
        public static void ASKSPAWNDATA(Packet _packet)
        {
            int lvl = _packet.ReadInt();
            int from = _packet.ReadInt();
            if(lvl == MyMod.levelid)
            {
                MyMod.SendSpawnData(false);
            }
        }

        

        public static void FURNBROKEN(Packet _packet)
        {
            DataStr.BrokenFurnitureSync furn = _packet.ReadFurn();

            if(furn.m_LevelGUID == MyMod.level_guid)
            {
                if (furn.m_Broken)
                {
                    if (MyMod.DelayedGearsPickup)
                    {
                        MyMod.BrokenFurnsBackup.Add(furn);

                    } else
                    {
                        MyMod.RemoveBrokenFurniture(furn.m_Guid, furn.m_ParentGuid);
                    }
                } else
                {
                    MyMod.RepairBrokenFurniture(furn.m_Guid, furn.m_ParentGuid);
                }
            }
        }
        public static void FURNBROKENLIST(Packet _packet)
        {

        }
        public static void FURNBREAKINGGUID(Packet _packet)
        {
            DataStr.BrokenFurnitureSync furn = _packet.ReadFurn();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_BrakingObject = furn;

                if (MyMod.playersData[from].m_LevelGuid == MyMod.level_guid)
                {
                    MyMod.playersData[from].m_BrakingSounds = MyMod.GetBreakDownSound(furn);
                }
            }
        }

        public static void FURNBREAKINSTOP(Packet _packet)
        {
            bool broken = _packet.ReadBool();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_BrakingObject = new DataStr.BrokenFurnitureSync();
                MyMod.playersData[from].m_BrakingSounds = "";
            }
        }



        public static void ProcessDeleyedGearsPickcup()
        {
            MyMod.DelayedGearsPickup = false;
            foreach (DataStr.PickedGearSync gear in MyMod.PickedGearsBackup)
            {
                Shared.AddPickedGear(gear.m_Spawn, gear.m_LevelID, gear.m_LevelGUID, -1, gear.m_MyInstanceID, gear.m_GearName, false);
            }
            MyMod.FoundSomethingToBreak = false;
            foreach (DataStr.BrokenFurnitureSync furn in MyMod.BrokenFurnsBackup)
            {
                MyMod.RemoveBrokenFurniture(furn.m_Guid, furn.m_ParentGuid, false);
            }
            MyMod.BakePreSpawnedGearsList();
            if (MyMod.FoundSomethingToBreak)
            {
                MyMod.RemoveAttachedGears = 2;
            } else{
                MyMod.PickedGearsBackup.Clear();
            }
            MyMod.BrokenFurnsBackup.Clear();
        }

        public static void GEARPICKUP(Packet _packet)
        {
            DataStr.PickedGearSync gear = _packet.ReadPickedGear();
            int from = _packet.ReadInt();

            if(from != -1)
            {
                if (MyMod.playersData[from] != null && MyMod.playersData[from].m_LevelGuid == MyMod.level_guid)
                {
                    if (MyMod.players[from] != null && MyMod.players[from].GetComponent<Comps.MultiplayerPlayerAnimator>() != null)
                    {
                        MyMod.players[from].GetComponent<Comps.MultiplayerPlayerAnimator>().Pickup();
                    }
                }
            }

            if (MyMod.DelayedGearsPickup)
            {
                MyMod.PickedGearsBackup.Add(gear);
            } else
            {
                Shared.AddPickedGear(gear.m_Spawn, gear.m_LevelID, gear.m_LevelGUID, from, gear.m_MyInstanceID, gear.m_GearName, false);
            }

            
        }
        public static void GEARPICKUPLIST(Packet _packet)
        {

        }
        public static void ROPE(Packet _packet)
        {
            DataStr.ClimbingRopeSync rope = _packet.ReadRope();

            Shared.AddDeployedRopes(rope.m_Position, rope.m_Deployed, rope.m_Snapped, rope.m_LevelID, rope.m_LevelGUID, false);
        }
        public static void ROPELIST(Packet _packet)
        {
            int howmany = _packet.ReadInt();

            for (int i = 1; i <= howmany; i++)
            {
                DataStr.ClimbingRopeSync rope = _packet.ReadRope();

                if (MyMod.DeployedRopes.Contains(rope) == false)
                {
                    Shared.AddDeployedRopes(rope.m_Position, rope.m_Deployed, rope.m_Snapped, rope.m_LevelID, rope.m_LevelGUID, false);
                }
            }
        }
        public static void CONSUME(Packet _packet)
        {
            bool IsDrink = _packet.ReadBool();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null && MyMod.playersData[from].m_Levelid == MyMod.levelid && MyMod.playersData[from].m_LevelGuid == MyMod.level_guid)
            {
                if (MyMod.players[from] != null && MyMod.players[from].GetComponent<Comps.MultiplayerPlayerAnimator>() != null)
                {
                    MyMod.players[from].GetComponent<Comps.MultiplayerPlayerAnimator>().m_IsDrink = IsDrink;
                    MyMod.players[from].GetComponent<Comps.MultiplayerPlayerAnimator>().Consumption();
                }
            }
        }
        public static void SERVERCFG(Packet _packet)
        {
            DataStr.ServerConfigData cfg = _packet.ReadServerCFG();
            MyMod.ServerConfig = cfg;
            MyMod.ShowCFGData();
        }
        public static void STOPCONSUME(Packet _packet)
        {
            string LastAnim = _packet.ReadString();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null)
            {
                if(MyMod.playersData[from].m_Levelid == MyMod.levelid && MyMod.playersData[from].m_LevelGuid == MyMod.level_guid)
                {
                    if (MyMod.players[from] != null && MyMod.players[from].GetComponent<Comps.MultiplayerPlayerAnimator>() != null)
                    {
                        MyMod.players[from].GetComponent<Comps.MultiplayerPlayerAnimator>().StopConsumption();
                    }
                }
                MyMod.playersData[from].m_AnimState = LastAnim;
            }
        }
        public static void HEAVYBREATH(Packet _packet)
        {
            bool IsHeavyBreath = _packet.ReadBool();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_HeavyBreath = IsHeavyBreath;
            }
        }
        public static void BLOODLOSTS(Packet _packet)
        {
            int BloodCount = _packet.ReadInt();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_BloodLosts = BloodCount;
            }
        }
        public static void APPLYACTIONONPLAYER(Packet _packet)
        {
            string ActionType = _packet.ReadString();
            int from = _packet.ReadInt();

            MyMod.OtherPlayerApplyActionOnMe(ActionType, from);
        }
        public static void DONTMOVEWARNING(Packet _packet)
        {
            bool ok = _packet.ReadBool();
            int from = _packet.ReadInt();

            if(MyMod.playersData[from] != null)
            {
                HUDMessage.AddMessage("PLEASE DON'T MOVE, "+MyMod.playersData[from].m_Name+" IS TENDING YOU");
                GameManager.GetVpFPSPlayer().Controller.Stop();
            }
        }
        public static void INFECTIONSRISK(Packet _packet)
        {
            bool InfRisk = _packet.ReadBool();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_NeedAntiseptic = InfRisk;
            }
        }
        public static void CANCLEPICKUP(Packet _packet)
        {
            DataStr.PickedGearSync gear = _packet.ReadPickedGear();
            int from = _packet.ReadInt();

            MelonLogger.Msg(ConsoleColor.Blue, "Other shokal has pickup item before me! I need to delete my picked gear Item InstanceID "+ gear.m_MyInstanceID);

            int _IID = gear.m_MyInstanceID;

            Il2CppSystem.Collections.Generic.List<Il2CppTLD.Gear.GearItemObject> invItems = GameManager.GetInventoryComponent().m_Items;
            for (int i = 0; i < invItems.Count; i++)
            {
                Il2CppTLD.Gear.GearItemObject currGear = invItems[i];
                if (currGear != null)
                {
                    if(currGear.m_GearItem.m_InstanceID == _IID)
                    {
                        HUDMessage.AddMessage("THIS ALREADY PICKED!");
                        GameAudioManager.PlayGUIError();
                        GameManager.GetInventoryComponent().DestroyGear(currGear.m_GearItem.gameObject);
                        return;
                    }
                }
            }

            for (int i = 0; i < GearManager.m_Gear.Count; i++)
            {
                GearItem currentGear = GearManager.m_Gear[i];
                if (currentGear.m_InstanceID == _IID)
                {
                    currentGear.gameObject.SetActive(false);
                    UnityEngine.Object.Destroy(currentGear.gameObject);
                    HUDMessage.AddMessage("THIS ALREADY PICKED!");
                    GameAudioManager.PlayGUIError();
                    return;
                }
            }
        }
        public static void CONTAINERINTERACT(Packet _packet)
        {
            DataStr.ContainerOpenSync box = _packet.ReadContainer();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null)
            {
                if (box.m_Guid == "NULL")
                {
                    MyMod.playersData[from].m_Container = null;
                }else{
                    MyMod.playersData[from].m_Container = box;
                }
            }
        }
        public static void LOOTEDCONTAINER(Packet _packet)
        {
            DataStr.ContainerOpenSync box = _packet.ReadContainer();
            int from = _packet.ReadInt();
            int State = _packet.ReadInt();
            Shared.AddLootedContainer(box, false, from, State);
        }
        public static void LOOTEDCONTAINERLIST(Packet _packet)
        {

        }
        public static void HARVESTPLANT(Packet _packet)
        {
            DataStr.HarvestableSyncData harveData = _packet.ReadHarvestablePlant();
            int from = _packet.ReadInt();
            if (MyMod.playersData[from] != null)
            {
                if(harveData.m_State == "Start")
                {
                    MyMod.playersData[from].m_Plant = harveData.m_Guid;
                }else{
                    MyMod.playersData[from].m_Plant = "";
                }
            }
        }
        public static void LOOTEDHARVESTABLE(Packet _packet)
        {
            string plantGUID = _packet.ReadString();
            string Scene = _packet.ReadString();
            int HarvestTime = _packet.ReadInt();
            if(Scene == MyMod.level_guid)
            {
                if(HarvestTime == -1)
                {
                    MyMod.AddHarvastedPlant(plantGUID);
                } else
                {
                    MyMod.RemoveHarvastedPlant(plantGUID);
                }
            }
        }
        public static void LOOTEDHARVESTABLEALL(Packet _packet)
        {

        }
        public static void SELECTEDCHARACTER(Packet _packet)
        {
            int character = _packet.ReadInt();
            int from = _packet.ReadInt();
            if (MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_Character = character;

                if(character == 0)
                {
                    MyMod.playersData[from].m_Female = false;
                }
                if (character == 1)
                {
                    MyMod.playersData[from].m_Female = true;
                }
            }
        }
        public static void ADDSHELTER(Packet _packet)
        {
            MelonLogger.Msg("Someone created shelter!");
            DataStr.ShowShelterByOther shelter = _packet.ReadShelter();
            int from = _packet.ReadInt();
            Shared.ShelterCreated(shelter.m_Position, shelter.m_Rotation, shelter.m_LevelID, shelter.m_LevelGUID, false);
        }
        public static void REMOVESHELTER(Packet _packet)
        {
            MelonLogger.Msg("Someone removed shelter!");
            DataStr.ShowShelterByOther shelter = _packet.ReadShelter();
            int from = _packet.ReadInt();
            Shared.ShelterRemoved(shelter.m_Position, shelter.m_LevelID, shelter.m_LevelGUID, false);
        }
        public static void ALLSHELTERS(Packet _packet)
        {
            int howmany = _packet.ReadInt();

            for (int i = 1; i <= howmany; i++)
            {
                DataStr.ShowShelterByOther shelter = _packet.ReadShelter();

                if (MyMod.ShowSheltersBuilded.Contains(shelter) == false)
                {
                    Shared.ShelterCreated(shelter.m_Position, shelter.m_Rotation, shelter.m_LevelID, shelter.m_LevelGUID, false);
                }
            }
        }
        public static void USESHELTER(Packet _packet)
        {
            DataStr.ShowShelterByOther shelter = _packet.ReadShelter();
            int from = _packet.ReadInt();
            if (MyMod.playersData[from] != null)
            {
                if(shelter.m_Position == new Vector3(0, 0, 0))
                {
                    MyMod.playersData[from].m_Shelter = null;
                }else{
                    MyMod.playersData[from].m_Shelter = shelter;
                }
            }
        }
        public static void FIRE(Packet _packet)
        {
            DataStr.FireSourcesSync FireSource = _packet.ReadFire();
            int from = _packet.ReadInt();
            MyMod.MayAddFireSources(FireSource);
        }
        public static void FIREFUEL(Packet _packet)
        {
            DataStr.FireSourcesSync FireSource = _packet.ReadFire();
            if(FireSource.m_LevelId != MyMod.levelid || FireSource.m_LevelGUID != MyMod.level_guid)
            {
                return;
            }
            int from = _packet.ReadInt();
            MyMod.AddOtherFuel(FireSource, FireSource.m_FuelName);
        }
        public static void CUSTOM(Packet _packet)
        {
            API.CustomEventCallback(_packet, -1);
        }
        public static void KICKMESSAGE(Packet _packet)
        {
            string kickMessage = _packet.ReadString();
            MyMod.DoKickMessage(kickMessage);
        }

        public static void PrintModsList(string RawString)
        {
            MelonLogger.Msg(ConsoleColor.Red,"SERVER MODS:\n"+RawString.Replace(@"M\", @"Mods\").Replace(@"A\", @"Mods\").Replace(@"P\", @"Plugins\"));
        }
        public static void MODSLIST(Packet _packet)
        {
            string CompressedString = _packet.ReadString();
            string DecompressedString = Shared.DecompressString(CompressedString);
            PrintModsList(DecompressedString);
        }
        public static void GOTITEMSLICE(Packet _packet)
        {
            DataStr.SlicedJsonData got = _packet.ReadSlicedGear();
            MyMod.AddSlicedJsonData(got);
        }
        public static void VOICECHAT(Packet _packet)
        {
            int BytesWritten = _packet.ReadInt();
            int ReadLength = _packet.ReadInt();
            byte[] CompressedData = _packet.ReadBytes(ReadLength);
            float RecordTime = _packet.ReadFloat();
            float RadioF = _packet.ReadFloat();
            RadioF = Mathf.Round(RadioF * 10.0f) * 0.1f;
            int from = _packet.ReadInt(); // Play from
            int Sender = _packet.ReadInt(); // Play whos voice

            if (MyMod.playersData[from] != null && MyMod.playersData[from].m_LevelGuid == MyMod.level_guid)
            {
                bool IsRadio = false;
                if(from != Sender)
                {
                    IsRadio = true;
                }

                if (!MyMod.DedicatedServerAppMode)
                {
                    MyMod.ProcessVoiceChatData(from, CompressedData, uint.Parse(BytesWritten.ToString()), RecordTime, IsRadio);
                }
            }
            if(MyMod.RadioFrequency == RadioF && MyMod.playersData[from].m_PlayerEquipmentData.m_HoldingItem == "GEAR_HandheldShortwave" && !MyMod.DoingRecord)
            {
                MyMod.ProcessRadioChatData(CompressedData, uint.Parse(BytesWritten.ToString()), RecordTime);
                MyMod.SendMyRadioAudio(CompressedData, BytesWritten, RecordTime, Sender);
            }
        }
        public static void SLICEDBYTES(Packet _packet)
        {

        }
        public static void ANIMALDAMAGE(Packet _packet)
        {
            string guid = _packet.ReadString();
            float damage = _packet.ReadFloat();
            MyMod.DoAnimalDamage(guid, damage);
        }
        public static void DROPITEM(Packet _packet)
        {
            DataStr.DroppedGearItemDataPacket GearData = _packet.ReadDroppedGearData();
            Shared.FakeDropItem(GearData);
        }
        public static void PICKDROPPEDGEAR(Packet _packet)
        {
            int ItemHash = _packet.ReadInt();
            int from = _packet.ReadInt();
            MyMod.PickDroppedItem(ItemHash, from);
        }
        public static void GETREQUESTEDITEMSLICE(Packet _packet)
        {
            DataStr.SlicedJsonData got = _packet.ReadSlicedGear();
            MyMod.AddSlicedJsonDataForPicker(got, false);
        }
        public static void GETREQUESTEDFORPLACESLICE(Packet _packet)
        {
            DataStr.SlicedJsonData got = _packet.ReadSlicedGear();
            MyMod.AddSlicedJsonDataForPicker(got, true);
        }
        public static void GOTCONTAINERSLICE(Packet _packet)
        {
            DataStr.SlicedJsonData got = _packet.ReadSlicedGear();
            Shared.AddSlicedJsonDataForContainer(got);
        }
        public static void OPENEMPTYCONTAINER(Packet _packet)
        {
            MelonLogger.Msg("Host sent that this container is empty!");
            MyMod.DiscardRepeatPacket();
            MyMod.FinishOpeningFakeContainer("");
        }
        public static void MARKSEARCHEDCONTAINERS(Packet _packet)
        {
            string _GUID = _packet.ReadString();
            GameObject box = Il2CppTLD.PDID.PdidTable.GetGameObject(_GUID);
            if (box != null)
            {
                GameObject reference = MyMod.GetGearItemObject("GEAR_SoftWood");
                GameObject newGear = UnityEngine.Object.Instantiate<GameObject>(reference, box.transform.position, box.transform.rotation);
                box.GetComponent<Container>().AddGear(newGear.GetComponent<GearItem>());
            }
        }
        public static void READYSENDNEXTSLICE(Packet _packet)
        {
            MyMod.SendNextCarefulSlice();
        }
        public static void READYSENDNEXTSLICEGEAR(Packet _packet)
        {
            MyMod.SendNextGearCarefulSlice();
        }
        public static void READYSENDNEXTSLICEPHOTO(Packet _packet)
        {
            MyMod.SendNextPhotoCarefulSlice();
        }
        
        public static void CHANGEAIM(Packet _packet)
        {
            bool IsAiming = _packet.ReadBool();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_Aiming = IsAiming;
            }
        }
        public static void LOADINGSCENEDROPSDONE(Packet _packet)
        {
            MelonLogger.Msg("Host sent scene loading done!");
            MyMod.DiscardRepeatPacket();
            MyMod.RemovePleaseWait();
            if (MyMod.DelayedGearsPickup)
            {
                ProcessDeleyedGearsPickcup();
            }
        }
        public static void GEARNOTEXIST(Packet _packet)
        {
            MelonLogger.Msg("Gear I requested is not exist!");
            MyMod.DiscardRepeatPacket();
            MyMod.RemovePleaseWait();
        }
        public static void DOORLOCKEDMSG(Packet _packet)
        {
            MyMod.DiscardRepeatPacket();
            MyMod.RemovePleaseWait();
            string Message = _packet.ReadString();
            HUDMessage.AddMessage(Message);
            if(Message == "Incorrect key!")
            {
                GameAudioManager.PlaySound("Play_SndMechDoorWoodLocked01", GameManager.GetGameAudioManagerComponent().gameObject);
            }
        }
        public static void USEOPENABLE(Packet _packet)
        {
            string _GUID = _packet.ReadString();
            bool state = _packet.ReadBool();
            Shared.ChangeOpenableThingState(MyMod.level_guid, _GUID, state);
        }
        public static void TRYDIAGNISISPLAYER(Packet _packet)
        {
            MelonLogger.Msg(ConsoleColor.Green, "TRYDIAGNISISPLAYER");
            int from = _packet.ReadInt();
            Condition Con = GameManager.GetConditionComponent();
            Hunger Hun = GameManager.GetHungerComponent();
            Thirst Thi = GameManager.GetThirstComponent();
            MyMod.SendMyAffictions(from, Con.m_CurrentHP, Con.m_MaxHP, Thi.m_CurrentThirst, Hun.m_CurrentReserveCalories, Hun.m_MaxReserveCalories);
            MelonLogger.Msg(ConsoleColor.Green, "SendMyAffictions("+ from+","+" "+GameManager.GetConditionComponent().m_CurrentHP+");");
        }
        public static void SENDMYAFFLCTIONS(Packet _packet)
        {
            int Who = _packet.ReadInt();
            int Count = _packet.ReadInt();
            float hp = _packet.ReadFloat();
            float hpmax = _packet.ReadFloat();
            float thirst = _packet.ReadFloat();
            float hunger = _packet.ReadFloat();
            float hungermax = _packet.ReadFloat();
            List<DataStr.AffictionSync> Affs = new List<DataStr.AffictionSync>();

            for (int index = 0; index < Count; ++index)
            {
                DataStr.AffictionSync newElement = _packet.ReadAffiction();
                Affs.Add(newElement);
            }
            MyMod.CheckOtherPlayer(Affs, Who, hp, hpmax, thirst, hunger, hungermax);
        }
        public static void CUREAFFLICTION(Packet _packet)
        {
            DataStr.AffictionSync toCure = _packet.ReadAffiction();
            MyMod.OtherPlayerCuredMyAffiction(toCure);
        }
        public static void ANIMALCORPSE(Packet _packet)
        {
            DataStr.AnimalKilled Data = _packet.ReadAnimalCorpse();
            MyMod.ProcessAnimalCorpseSync(Data);
        }
        public static void GOTRABBIT(Packet _packet)
        {
            int result = _packet.ReadInt();
            MyMod.FinallyPickupRabbit(result);
        }
        public static void RELEASERABBIT(Packet _packet)
        {
            int from = _packet.ReadInt();
            MyMod.OnReleaseRabbit(from);
        }
        public static void HITRABBIT(Packet _packet)
        {
            string GUID = _packet.ReadString();
            MyMod.OnHitRabbit(GUID);
        }
        public static void RABBITREVIVED(Packet _packet)
        {
            Vector3 v3 = _packet.ReadVector3();
            if(MyMod.AnimalsController == true)
            {
                MyMod.OnRabbitRevived(v3);
            }
        }
        public static void REQUESTANIMALCORPSE(Packet _packet)
        {
            float Meat = _packet.ReadFloat();
            int Guts = _packet.ReadInt();
            int Hide = _packet.ReadInt();
            if (Meat != -1)
            {
                if (MyMod.GoingToHarvest)
                {
                    MyMod.GoingToHarvest.m_MeatAvailableKG = Meat;
                    MyMod.GoingToHarvest.m_GutAvailableUnits = Guts;
                    MyMod.GoingToHarvest.m_HideAvailableUnits = Hide;
                    MyMod.OpenBodyHarvest(MyMod.GoingToHarvest);
                }
            }else{
                MyMod.RemovePleaseWait();
                HUDMessage.AddMessage("Nothing to harvest");
            }
        }
        public static void QUARTERANIMAL(Packet _packet)
        {
            string GUID = _packet.ReadString();
            MelonLogger.Msg("QUARTERANIMAL "+ GUID);
            MyMod.SpawnQuartedMess(GUID);
        }
        public static void ANIMALAUDIO(Packet _packet)
        {
            bool InInt = _packet.ReadBool();
            string GUID = _packet.ReadString();
            if (InInt)
            {
                int soundID = _packet.ReadInt();
                int From = _packet.ReadInt();
                if (MyMod.playersData[From] != null)
                {
                    if (MyMod.level_guid == MyMod.playersData[From].m_LevelGuid)
                    {
                        Pathes.Play3dAudioOnAnimal(GUID, soundID);
                    }
                }
            }else{
                string soundID = _packet.ReadString();
                int From = _packet.ReadInt();
                if (MyMod.playersData[From] != null)
                {
                    if (MyMod.level_guid == MyMod.playersData[From].m_LevelGuid)
                    {
                        Pathes.Play3dAudioOnAnimal(GUID, soundID);
                    }
                }
            }
        }
        public static void MELEESTART(Packet _packet)
        {
            int from = _packet.ReadInt();
            if (MyMod.players[from] != null && MyMod.players[from].GetComponent<Comps.MultiplayerPlayerAnimator>() != null)
            {
                MyMod.players[from].GetComponent<Comps.MultiplayerPlayerAnimator>().MeleeAttack();
            }
        }

        public static void TRYBORROWGEAR(Packet _packet)
        {
            string GearName = _packet.ReadString();
            int For = _packet.ReadInt();
            MyMod.GiveBorrowedItem(GearName, For);
        }

        public static void CHALLENGEINIT(Packet _packet)
        {
            int ID = _packet.ReadInt();
            int CurrentTask = _packet.ReadInt();
            MyMod.LoadCustomChallenge(ID, CurrentTask);
        }
        public static void CHALLENGEUPDATE(Packet _packet)
        {
            MyMod.CurrentCustomChalleng = _packet.ReadChallengeData();
        }
        public static void CHALLENGETRIGGER(Packet _packet)
        {
            string TRIGGER = _packet.ReadString();
            MyMod.ProcessCustomChallengeTrigger(TRIGGER);
        }
        public static void ADDDEATHCONTAINER(Packet _packet)
        {
            DataStr.DeathContainerData Con = _packet.ReadDeathContainer();
            if(MyMod.level_guid == Con.m_LevelKey)
            {
                MyMod.MakeDeathCreate(Con);
            }
        }
        public static void DEATHCREATEEMPTYNOW(Packet _packet)
        {
            string GUID = _packet.ReadString();
            string Scene = _packet.ReadString();
            MyMod.RemoveDeathContainer(GUID, Scene);
        }
        public static void SPAWNREGIONBANCHECK(Packet _packet)
        {
            string GUID = _packet.ReadString();
            bool Result = _packet.ReadBool();
            GameObject obj = Il2CppTLD.PDID.PdidTable.GetGameObject(GUID);
            if (obj != null && obj.GetComponent<Comps.SpawnRegionSimple>())
            {
                obj.GetComponent<Comps.SpawnRegionSimple>().SetBanned(Result);
            }
        }
        public static void CAIRNS(Packet _packet)
        {
            int HowMuch = _packet.ReadInt();
            for (int i = 0; i < HowMuch; i++)
            {
                int Cairn = _packet.ReadInt();
                MyMod.AddFoundCairn(Cairn);
            }

            MyMod.CreateCairnsSearchList();
        }
        public static void BENEFITINIT(Packet _packet)
        {
            int From = _packet.ReadInt();
            Supporters.SupporterBenefits B = _packet.ReadSupporterBenefits();
            if (MyMod.playersData[From] != null)
            {
                MyMod.playersData[From].m_SupporterBenefits = B;
                for (int i = 0; i < B.m_Flairs.Count; i++)
                {
                    Log("[BENEFITINIT] From Client "+From + " Slot" + i + " FlairID " + B.m_Flairs[i]);
                }
            }
        }
        public static void ADDDOORLOCK(Packet _packet)
        {
            string DoorGUID = _packet.ReadString();
            MyMod.AddLocksToDoorsByGUID(DoorGUID);
        }
        public static void ENTERDOOR(Packet _packet)
        {
            MyMod.DiscardRepeatPacket();
            MyMod.RemovePleaseWait();
            string DoorGUID = _packet.ReadString();
            MyMod.EnterDoorsByGUID(DoorGUID);
        }
        public static void REMOVEDOORLOCK(Packet _packet)
        {
            MyMod.DiscardRepeatPacket();
            MyMod.RemovePleaseWait();
            string DoorGUID = _packet.ReadString();
            MyMod.RemoveLocksFromDoorsByGUID(DoorGUID);
        }
        public static void LOCKPICK(Packet _packet)
        {
            bool Swear = _packet.ReadBool();
            MyMod.SwearOnLockpickingDone = Swear;
        }
        public static void RPC(Packet _packet)
        {
            string RPCDATA = _packet.ReadString();
            uConsole.RunCommand(RPCDATA);
        }
        public static void REQUESTLOCKSMITH(Packet _packet)
        {
            MyMod.RemovePleaseWait();
            int State = _packet.ReadInt();
            if (State != -1)
            {
                if (MyMod.PendingLocksmithObject)
                {
                    string Name = MyMod.PendingLocksmithObject.GetComponent<Comps.DroppedGearDummy>().m_Extra.m_GearName;
                    DataStr.PriorityActionForOtherPlayer act = MyMod.GetCustomAction("Locksmith" + MyMod.PendingLocksmithAction);
                    MyMod.DoLongAction(MyMod.PendingLocksmithObject, act.m_ProcessText, act.m_Action);
                }
            }else{
                HUDMessage.AddMessage("Someone already work on this!");
                GameAudioManager.PlayGUIError();
            }
        }
        public static void KNOCKKNOCK(Packet _packet)
        {
            HUDMessage.AddMessage("Somebody's knocking on the door");
        }
        public static void KNOCKENTER(Packet _packet)
        {
            string Scene = _packet.ReadString();
            MyMod.EnterDoorsByScene(Scene);
        }
        public static void PEEPHOLE(Packet _packet)
        {
            MyMod.RemovePleaseWait();

            if(MyMod.TempDoor != null)
            {
                MyMod.ShowKnockersPicker(MyMod.TempDoor, _packet.ReadIntList());
            }
        }
        public static void RESTART(Packet _packet)
        {
            MyMod.RemovePleaseWait();
            MyMod.SetRepeatPacket(MyMod.ResendPacketType.ServerRestart);
            MyMod.RepeatLastRequest();
        }
        
        public static void DoWeatherSync(float StartAtFrac, int WeatherSeed, float Duration, WeatherStage ST,int Indx, List<float> Durations, List<float> Transitions, int TOD, float High, float Low, int PreviousStage)
        {
            if (MyMod.level_name != "Boot" && MyMod.level_name != "Empty" && GameManager.m_Wind != null && GameManager.m_Wind.m_ActiveSettings != null && GameManager.m_Weather != null && GameManager.m_WeatherTransition != null && GameManager.GetUniStorm() != null && MyMod.m_InterfaceManager != null && InterfaceManager.m_Panel_Loading != null && InterfaceManager.m_Panel_Loading.IsLoading() == false)
            {
                WeatherStage PreviousStageType = (WeatherStage)PreviousStage;
                System.Random RNG = new System.Random(WeatherSeed);
                Weather Weather = GameManager.GetWeatherComponent();
                WeatherSet Set = GameManager.GetWeatherTransitionComponent().m_CurrentWeatherSet;
                if (Set == null)
                {
                    return;
                }
                int SetIndex = 0;
                for (int i = 0; i < Weather.m_WeatherSetsForScene.Count; i++)
                {
                    if (Weather.m_WeatherSetsForScene[i].gameObject.name == Set.gameObject.name)
                    {
                        SetIndex = i;
                        break;
                    }
                }
                if (SetIndex == Indx)
                {
                    Set.m_WeatherStages[0].m_PreviousType = PreviousStageType;
                    Set.Activate(StartAtFrac, PreviousStageType);
                    Set.m_WeatherStages[0].m_PreviousType = PreviousStageType;
                    WeatherTransition.m_WeatherTransitionTimeScalar = 0;
                    Weather.m_TemperatureCountForTimeOfDay = TOD;
                    Weather.m_TempHigh = High;
                    Weather.m_TempLow = Low;
                    return;
                }

                if (Weather == null)
                {
                    return;
                }
                WeatherSet ws;

                if (Weather.m_WeatherSetsForScene.Count == 0)
                {
                    for (int index = 0; index < Weather.m_DefaultWeatherSets.Length; ++index)
                    {
                        if (Weather.m_DefaultWeatherSets[index] && Weather.m_DefaultWeatherSets[index].gameObject)
                        {
                            Weather.m_WeatherSetsForScene.Add(Weather.GetInstancedWeatherSet(Weather.m_DefaultWeatherSets[index].gameObject));
                        }
                    }
                }
                WeatherSet weatherSet1 = null;


                if (Weather.m_WeatherSetsForScene.Count - 1 <= Indx)
                {
                    weatherSet1 = Weather.m_WeatherSetsForScene[Indx];
                } else
                {
                    int num1 = 0;
                    for (int index = 0; index < Weather.m_WeatherSetsForScene.Count; ++index)
                    {
                        WeatherSet weatherSet2 = Weather.m_WeatherSetsForScene[index];
                        if (weatherSet2.m_CharacterizingType == ST)
                            num1 += weatherSet2.m_SameTypeSelectionWeight;
                    }
                    int num2 = RNG.Next(0, num1);
                    for (int index = Weather.m_WeatherSetsForScene.Count - 1; index >= 0; --index)
                    {
                        WeatherSet weatherSet3 = Weather.m_WeatherSetsForScene[index];
                        if (weatherSet3.m_CharacterizingType == ST)
                        {
                            num1 -= weatherSet3.m_SameTypeSelectionWeight;
                            if (num2 >= num1)
                            {
                                weatherSet1 = weatherSet3;
                                break;
                            }
                        }
                    }
                }

                if (weatherSet1 != null && !weatherSet1.m_IsDefaultSet)
                {
                    weatherSet1 = Weather.GetInstancedWeatherSet(weatherSet1.gameObject);
                }

                ws = weatherSet1;

                if (ws == null)
                {
                    return;
                }


                ws.m_CurrentSetDuration = Duration;
                for (int index = 0; index < ws.m_WeatherStages.Length; ++index)
                {
                    WeatherSetStage weatherStage = ws.m_WeatherStages[index];
                    if (Durations.Count - 1 >= index)
                    {
                        weatherStage.m_CurrentDuration = Durations[index];
                        weatherStage.m_CurrentTransitionTime = Transitions[index];
                    }
                }
                WeatherTransition.m_WeatherTransitionTimeScalar = 0;
                GameManager.GetWeatherTransitionComponent().ActivateWeatherSet(ws, StartAtFrac, PreviousStageType);
                Set.m_WeatherStages[0].m_PreviousType = PreviousStageType;
                Weather.m_TemperatureCountForTimeOfDay = TOD;
                Weather.m_TempHigh = High;
                Weather.m_TempLow = Low;
                //MelonLogger.Msg(ConsoleColor.Blue, "WeatherSet updated!");
            } else
            {
                MelonLogger.Msg("Can't apply WeatherSync, because loading, skipping");
            }
        }
        public static void DEDICATEDWEATHER(Packet _packet)
        {
            WeatherStage WeatherType = (WeatherStage)_packet.ReadInt();
            int Indx = _packet.ReadInt();
            float StartAtFrac = _packet.ReadFloat();
            int WeatherSeed = _packet.ReadInt();
            float Duration = _packet.ReadFloat();
            List<float> Durations = _packet.ReadFloatList();
            List<float> Transitions = _packet.ReadFloatList();
            int TOD = _packet.ReadInt();
            float High = _packet.ReadFloat();
            float Low = _packet.ReadFloat();
            int PreviousStage = _packet.ReadInt();
            DoWeatherSync(StartAtFrac, WeatherSeed, Duration, WeatherType, Indx, Durations, Transitions, TOD, High, Low, PreviousStage);
        }
        public static void WEATHERVOLUNTEER(Packet _packet)
        {
            int AskRegion = _packet.ReadInt();
            if (MyMod.level_name != "Boot" && MyMod.level_name != "Empty" && GameManager.m_Wind != null && GameManager.m_Wind.m_ActiveSettings != null && GameManager.m_Weather != null && GameManager.m_WeatherTransition != null && GameManager.GetUniStorm() != null)
            {
                if((int)GameManager.GetUniStorm().m_CurrentRegion == AskRegion)
                {
                    using (Packet __packet = new Packet((int)ClientPackets.WEATHERVOLUNTEER))
                    {
                        __packet.Write(AskRegion);
                        MyMod.SendUDPData(__packet);
                    }
                }
            }
        }
        public static void REREGISTERWEATHER(Packet _packet)
        {
            int AskRegion = _packet.ReadInt();
            if (MyMod.level_name != "Boot" && MyMod.level_name != "Empty" && GameManager.m_Wind != null && GameManager.m_Wind.m_ActiveSettings != null && GameManager.m_Weather != null && GameManager.m_WeatherTransition != null && GameManager.GetUniStorm() != null)
            {
                if ((int)GameManager.GetUniStorm().m_CurrentRegion == AskRegion)
                {
                    Pathes.SendWeatherVolunteerData();
                }
            }
        }
        public static void REMOVEKEYBYSEED(Packet _packet)
        {
            string Key = _packet.ReadString();
            MyMod.RemoveKey(Key);
        }
        public static void ADDHUDMSG(Packet _packet)
        {
            string Message = _packet.ReadString();
            HUDMessage.AddMessage(Message);
        }
        public static void CHANGECONTAINERSTATE(Packet _packet)
        {
            string GUID = _packet.ReadString();
            int State = _packet.ReadInt();
            MelonLogger.Msg("Signal to container " + GUID + " state changed to " + State);
            MyMod.RemoveLootFromContainer(GUID, State);
        }
        public static void FINISHEDSENDINGCONTAINER(Packet _packet)
        {
            bool Error = _packet.ReadBool();
            MelonLogger.Msg("FINISHEDSENDINGCONTAINER Error " + Error);
            if (!Error)
            {
                Container box = InterfaceManager.m_Panel_Container.m_Container;
                if (box != null)
                {
                    if (!box.Close())
                        return;
                    if (box.m_CloseAudio.Length == 0)
                        GameAudioManager.PlayGUIButtonBack();
                }
                MyMod.RemovePleaseWait();
                GameManager.GetPlayerManagerComponent().MaybeRevealPolaroidDiscoveryOnClose();
                InterfaceManager.m_Panel_Container.Enable(false);
                Shared.ContainerDecompressedDataBackup = "";
            } else
            {
                Container box = InterfaceManager.m_Panel_Container.m_Container;
                string GUID = "";
                if (!string.IsNullOrEmpty(Shared.ContainerDecompressedDataBackup) && box != null)
                {
                    if (box.GetComponent<ObjectGuid>())
                    {
                        GUID = box.GetComponent<ObjectGuid>().Get();
                    }
                    MyMod.RemovePleaseWait();
                    MyMod.DoPleaseWait("Host received invalid data", "Trying send data again...");
                    Shared.SendContainerData(Shared.CompressString(Shared.ContainerDecompressedDataBackup), MyMod.level_guid, GUID, Shared.ContainerDecompressedDataBackup);
                }
            }
        }
        public static void TRIGGEREMOTE(Packet _packet)
        {
            int from = _packet.ReadInt();
            int EmoteID = _packet.ReadInt();

            if (MyMod.playersData[from] != null && MyMod.players[from] != null)
            {
                if(MyMod.playersData[from].m_LevelGuid == MyMod.level_guid)
                {
                    Comps.MultiplayerPlayerAnimator Anim = MyMod.players[from].GetComponent<Comps.MultiplayerPlayerAnimator>();
                    if(Anim != null)
                    {
                        DataStr.MultiplayerEmote Emote = MyMod.GetEmoteByID(EmoteID);

                        if (Emote.m_LeftHandEmote)
                        {
                            Anim.DoLeftHandEmote(Emote.m_Animation);
                        }
                    }
                }
            }
        }
        public static void EXPEDITIONSYNC(Packet _packet)
        {
            string ExpeditionName = _packet.ReadString();
            string Text = _packet.ReadString();
            int TimeLeft = _packet.ReadInt();

            if(string.IsNullOrEmpty(ExpeditionName) && string.IsNullOrEmpty(Text) && TimeLeft == 0)
            {
                MyMod.OnExpedition = false;
            } else
            {
                MyMod.OnExpedition = true;
                MyMod.ExpeditionLastName = ExpeditionName;
                MyMod.ExpeditionLastTaskText = Text;
                MyMod.ExpeditionLastTime = TimeLeft;
            }
        }
        public static void EXPEDITIONRESULT(Packet _packet)
        {
            int State = _packet.ReadInt();
            MyMod.DoExpeditionState(State);
        }
        public static void REQUESTEXPEDITIONINVITES(Packet _packet)
        {
            MyMod.RemovePleaseWait();
            MyMod.DiscardRepeatPacket();
            MyMod.ShowInvitesPicker(null, _packet.ReadInvitesList());
        }
        public static void NEWPLAYEREXPEDITION(Packet _packet)
        {
            string PlayerName = _packet.ReadString();
            MyMod.NewPlayerInExpedition(PlayerName);
        }
        public static void NEWEXPEDITIONINVITE(Packet _packet)
        {
            string PlayerName = _packet.ReadString();
            MyMod.NewExpeditionInvite(PlayerName);
        }
        public static void BASE64SLICE(Packet _packet)
        {
            DataStr.SlicedBase64Data Slice = _packet.ReadSlicedBase64Data();
            AddBase64Slice(Slice);
        }
        public static void ADDROCKCACH(Packet _packet)
        {
            DataStr.FakeRockCacheVisualData Data = _packet.ReadFakeRockCache();
            MyMod.AddRockCache(Data);
        }
        public static void REMOVEROCKCACH(Packet _packet)
        {
            DataStr.FakeRockCacheVisualData Data = _packet.ReadFakeRockCache();
            int State = _packet.ReadInt();
            if(State == -1)
            {
                MyMod.RemovePleaseWait();
                MyMod.PendingRockCahceRemove = null;
                using (Packet __packet = new Packet((int)ClientPackets.FURNBREAKINSTOP))
                {
                    __packet.Write(true);
                    MyMod.SendUDPData(__packet);
                }
            } else if(State == 0)
            {
                MyMod.RemovePleaseWait();
                MyMod.ContinueRemovingRockCache();
            }else if(State == 1)
            {
                MyMod.RemoveRockCache(Data);
            }
        }
        public static void ADDUNIVERSALSYNCABLE(Packet _packet)
        {
            DataStr.UniversalSyncableObject Obj = _packet.ReadUniversalSyncable();
            MyMod.SpawnUniversalSyncableObject(Obj);
        }
        public static void REMOVEUNIVERSALSYNCABLE(Packet _packet)
        {
            string GUID = _packet.ReadString();
            MyMod.RemoveObjectByGUID(GUID);
        }
    }
}
