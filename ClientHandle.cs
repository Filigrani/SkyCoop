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
using UnhollowerRuntimeLib;
using UnhollowerBaseLib;
using GameServer;

namespace SkyCoop
{
    public class ClientHandle
    {
        public static void Welcome(Packet _packet)
        {
            int _myId = _packet.ReadInt();
            int _MaxPlayers = _packet.ReadInt();

            MyMod.MaxPlayers = _MaxPlayers;
            MyMod.InitAllPlayers();

            MelonLogger.Msg("Welcome to server!");
            MyMod.instance.myId = _myId;
            MelonLogger.Msg("Host registered me as client "+ _myId);
            MyMod.sendMyPosition = true;
            MyMod.LastConnectedIp = MyMod.PendingConnectionIp;
            MyMod.NoHostResponceSeconds = 0;
            MyMod.NeedTryReconnect = false;
            MyMod.TryingReconnect = false;
            WelcomeReceived();
        }
        public static void WelcomeReceived()
        {
            using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
            {
                _packet.Write(MyMod.instance.myId);
                _packet.Write(MyMod.MyChatName);
                _packet.Write(MyMod.BuildInfo.Version);

                MyMod.SendTCPData(_packet);
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

                if(MyMod.players[from] != null && MyMod.players[from].GetComponent<MyMod.MultiplayerPlayer>() != null)
                {
                    MyMod.LongActionCancleCauseMoved(MyMod.players[from].GetComponent<MyMod.MultiplayerPlayer>());
                }
            }
        }
        public static void XYZDW(Packet _packet)
        {
            MyMod.ShatalkerModeClient = true;
            Vector3 maboi;
            maboi = _packet.ReadVector3();

            if (MyMod.ShatalkerObject != null)
            {
                MyMod.ShatalkerObject.m_WorldPosition = maboi;
                MyMod.LastRecivedShatalkerVector = maboi;
            }
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
                MyMod.playersData[from].m_Levelid = lel;
            }
            MelonLogger.Msg("Player " + from + "  transition to level " + lel);
        }
        public static void LEVELGUID(Packet _packet)
        {
            string lel = _packet.ReadString();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_LevelGuid = lel;
            }
            MelonLogger.Msg("Player " + from + "  transition to level with GUID " + lel);
        }
        public static void GOTITEM(Packet _packet)
        {
            MyMod.GearItemDataPacket got = _packet.ReadGearData();
            //MelonLogger.Msg(ConsoleColor.Blue, "Someone gave item to" + got.m_SendedTo);
            //MelonLogger.Msg(ConsoleColor.Blue, "Got gear with name [" + got.m_GearName + "] DATA: " + got.m_DataProxy);

            if(got.m_SendedTo == MyMod.instance.myId)
            {
                //MelonLogger.Msg(ConsoleColor.Blue, "This is item for me");
            }
            MyMod.GiveRecivedItem(got);
        }
        public static void GAMETIME(Packet _packet)
        {
            MyMod.OveridedTime = _packet.ReadString();
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
            MyMod.PlayerEquipmentData item = _packet.ReadEQ();
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
            MyMod.WeatherProxies weather = _packet.ReadWeather();
            if (MyMod.level_name != "Boot" && MyMod.level_name != "Empty" && GameManager.m_Wind != null && GameManager.m_Wind.m_ActiveSettings != null && GameManager.m_Weather != null && GameManager.m_WeatherTransition != null)
            {
                //Weather wa = GameManager.GetWeatherComponent();
                //WeatherSaveDataProxy weatherSaveDataProxy = Utils.DeserializeObject<WeatherSaveDataProxy>(weather.m_WeatherProxy);
                //wa.m_PrevBodyTemp = weatherSaveDataProxy.m_PrevBodyTempProxy;
                //wa.SetTempHigh(weatherSaveDataProxy.m_TempHighProxy);
                //wa.SetTempLow(weatherSaveDataProxy.m_TempLowProxy);
                //if (weatherSaveDataProxy.m_UseMinAirTemperature)
                //    wa.m_MinAirTemperature = weatherSaveDataProxy.m_MinAirTemperature;
                //wa.m_LastTimeOfDay = weatherSaveDataProxy.m_LastTimeOfDay;
                //wa.m_BaseTemperatureAccumulatorForTimeOfDay = weatherSaveDataProxy.m_BaseTemperatureAccumulatorForTimeOfDay;
                //wa.m_WindchillAccumulatorForTimeOfDay = weatherSaveDataProxy.m_WindchillAccumulatorForTimeOfDay;
                //wa.m_TemperatureCountForTimeOfDay = weatherSaveDataProxy.m_TemperatureCountForTimeOfDay;
                //GameManager.GetUniStorm().SetWeatherStage(weatherSaveDataProxy.m_WeatherStageProxy, 0.0f);
                //GameManager.GetUniStorm().SetElapsedHours(weatherSaveDataProxy.m_UniStormElapsedHoursProxy);

                GameManager.GetWeatherComponent().Deserialize(weather.m_WeatherProxy);
                GameManager.GetWeatherTransitionComponent().Deserialize(weather.m_WeatherTransitionProxy);
                GameManager.GetWindComponent().Deserialize(weather.m_WindProxy);
                //MelonLogger.Msg("Amogus slomal jopu Tod'oo");
            }
        }
        public static void REVIVE(Packet _packet)
        {
            MyMod.SimRevive();
            if (MyMod.sendMyPosition == true)
            {
                using (Packet __packet = new Packet((int)ClientPackets.REVIVEDONE))
                {
                    __packet.Write(true);
                    MyMod.SendTCPData(__packet);
                }
            }
        }
        public static void ANIMALROLE(Packet _packet)
        {
            MyMod.AnimalsController = _packet.ReadBool();
            //MelonLogger.Msg("Got new animal controller role: " + MyMod.AnimalsController);
            MyMod.AllowSpawnAnimals(MyMod.AnimalsController);
        }
        public static void ALIGNANIMAL(Packet _packet)
        {
            MyMod.AnimalAligner Alig = _packet.ReadAnimalAligner();
            MyMod.AlignAnimalWithProxy(Alig.m_Proxy, Alig.m_Guid);
        }
        public static void ASKFORANIMALPROXY(Packet _packet)
        {
            //string _guid = _packet.ReadString();
            //string Proxy = "";
            //for (int i = 0; i < BaseAiManager.m_BaseAis.Count; i++)
            //{
            //    if (BaseAiManager.m_BaseAis[i] != null && BaseAiManager.m_BaseAis[i].gameObject != null)
            //    {
            //        GameObject animal = BaseAiManager.m_BaseAis[i].gameObject;
            //        if (animal.GetComponent<ObjectGuid>() != null && animal.GetComponent<ObjectGuid>().Get() == _guid)
            //        {
            //            Proxy = BaseAiManager.m_BaseAis[i].Serialize();
            //            MelonLogger.Msg("ASKFORANIMALPROXY " + Proxy);
            //            break;
            //        }
            //    }
            //}
            //using (Packet __packet = new Packet((int)ClientPackets.ALIGNANIMAL))
            //{
            //    MyMod.AnimalAligner Alig = new MyMod.AnimalAligner();

            //    Alig.m_Guid = _guid;
            //    Alig.m_Proxy = Proxy;
            //    __packet.Write(Alig);
            //    MyMod.SendTCPData(__packet);
            //}
        }
        public static void ANIMALDELETE(Packet _packet)
        {
            string AnimalGuid = _packet.ReadString();
            MyMod.DeleteAnimal(AnimalGuid);
        }

        public static void KEEPITALIVE(Packet _packet)
        {
            MyMod.NoHostResponceSeconds = 0;
        }

        public static void RQRECONNECT(Packet _packet)
        {
            MyMod.Disconnect();
            MyMod.DoConnectToIp(MyMod.LastConnectedIp);
        }
        public static void CHAT(Packet _packet)
        {
            MyMod.MultiplayerChatMessage message = _packet.ReadChat();
            int from = _packet.ReadInt();
            MyMod.SendMessageToChat(message, false);
        }
        public static void PLAYERSSTATUS(Packet _packet)
        {
            int howmany = _packet.ReadInt();

            if(howmany > MyMod.MaxPlayers)
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
            List<MyMod.MultiPlayerClientStatus> MPStatus = new List<MyMod.MultiPlayerClientStatus>();

            for (int i = 1; i <= howmany; i++)
            {
                MyMod.MultiPlayerClientStatus client = _packet.ReadClientStatus();
                MPStatus.Add(client);
                if (MyMod.playersData[client.m_ID] != null)
                {
                    MyMod.playersData[client.m_ID].m_Used = true;
                    MyMod.playersData[client.m_ID].m_Name = client.m_Name;
                    if (client.m_Sleep == true)
                    {
                        MyMod.playersData[client.m_ID].m_SleepHours = 1;
                    }else{
                        MyMod.playersData[client.m_ID].m_SleepHours = 0;
                    }
                    MyMod.playersData[client.m_ID].m_Dead = client.m_Dead;
                }
            }
            MyMod.SleepTracker(MPStatus);
            MyMod.UpdatePlayerStatusMenu(MPStatus);
        }
        public static void ANIMALSYNC(Packet _packet)
        {
            if (MyMod.level_name == "Boot" || MyMod.level_name == "Empty")
            {
                return;
            }

            MyMod.AnimalSync obj = _packet.ReadAnimal();
            MyMod.DoAnimalSync(obj);
        }

        public static void ANIMALSYNCTRIGG(Packet _packet)
        {
            if (MyMod.level_name == "Boot" || MyMod.level_name == "Empty")
            {
                return;
            }

            MyMod.AnimalTrigger obj = _packet.ReadAnimalTrigger();
            MyMod.SetAnimalTriggers(obj);
        }

        public static void REVIVEDONE(Packet _packet)
        {
            GameManager.GetInventoryComponent().RemoveGearFromInventory("GEAR_MedicalSupplies_hangar", 1);
        }

        public static void DARKWALKERREADY(Packet _packet)
        {
            MyMod.DarkWalkerIsReady = _packet.ReadBool();
            MelonLogger.Msg("Got new darkwalker ready state: " + MyMod.DarkWalkerIsReady);

            if (MyMod.DarkWalkerIsReady == true)
            {
                ServerHandle.LastCountDown = 0;
                MyMod.OverridenStartCountDown = 0;
            }
        }
        public static void HOSTISDARKWALKER(Packet _packet)
        {
            if (MyMod.InDarkWalkerMode == true)
            {
                MyMod.ShatalkerModeClient = _packet.ReadBool();
                MelonLogger.Msg("Host is darkwalker: " + MyMod.ShatalkerModeClient);

                MyMod.RealTimeCycleSpeed = false;

                if (MyMod.ShatalkerModeClient == false)
                {
                    MyMod.IamShatalker = true;
                    uConsole.RunCommandSilent("Ghost");
                    uConsole.RunCommandSilent("God");

                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet __packet = new Packet((int)ClientPackets.REQUESTDWREADYSTATE))
                        {
                            __packet.Write(true);
                            MyMod.SendTCPData(__packet);
                        }
                    }
                }
                else
                {
                    if (MyMod.ShatalkerObject.GetStartMovementDelayTime() < 2)
                    {
                        using (Packet __packet = new Packet((int)ClientPackets.DARKWALKERREADY))
                        {
                            __packet.Write(MyMod.DarkWalkerIsReady);
                            MyMod.SendTCPData(__packet);
                        }
                    }
                }
            }
        }

        public static void WARDISACTIVE(Packet _packet)
        {
            MyMod.WardIsActive = _packet.ReadBool();
        }
        public static void DWCOUNTDOWN(Packet _packet)
        {
            float got = _packet.ReadFloat();

            if (MyMod.OverridenStartCountDown < 5 && got > MyMod.OverridenStartCountDown)
            {
                return;
            }
            MyMod.OverridenStartCountDown = got;
            ///Console.WriteLine("LastCountDown " + OverridenStartCountDown);
        }
        public static void SHOOTSYNC(Packet _packet)
        {
            MyMod.ShootSync shoot = _packet.ReadShoot();
            int from = _packet.ReadInt();
            MelonLogger.Msg("Client " + from + " shoot!");

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
            MyMod.OtherHarvetingAnimal = _packet.ReadString();
        }
        public static void DONEHARVASTING(Packet _packet)
        {
            MyMod.HarvestStats Harvey = _packet.ReadHarvest();
            MyMod.DoForcedHarvestAnimal(Harvey.m_Guid, Harvey);
        }

        public static void BULLETDAMAGE(Packet _packet)
        {
            float damage = _packet.ReadFloat();
            MyMod.DamageByBullet(damage);
        }
        public static void MULTISOUND(Packet _packet)
        {
            string sound = _packet.ReadString();
            int from = _packet.ReadInt();
            MyMod.PlayMultiplayer3dAduio(sound, from);
        }
        public static void CONTAINEROPEN(Packet _packet)
        {
            MyMod.ContainerOpenSync sync = _packet.ReadContainer();
            MyMod.DoSyncContainer(sync);
        }

        public static void LUREPLACEMENT(Packet _packet)
        {
            MyMod.WalkTracker sync = _packet.ReadWalkTracker();
            MyMod.LastLure = sync;
        }
        public static void LUREISACTIVE(Packet _packet)
        {
            MyMod.LureIsActive = _packet.ReadBool();
        }
        public static void SAVEDATA(Packet _packet)
        {
            MyMod.SaveSlotSync SaveData = _packet.ReadSaveSlot();
            MyMod.RemoveWaitForConnect();

            if (InterfaceManager.IsMainMenuEnabled() == false)
            {
                return;
            }

            MyMod.PendingSave = SaveData;

            MyMod.CheckHaveSaveFileToJoin(SaveData);
        }
        public static void CARRYBODY(Packet _packet)
        {
            MyMod.IsCarringMe = _packet.ReadBool();
        }
        public static void BODYWARP(Packet _packet)
        {
            string DoorId = _packet.ReadString();

            MelonLogger.Msg("Got remote door enter request " + DoorId);

            MyMod.WarpBody(DoorId);
        }
        public static void CHANGENAME(Packet _packet)
        {
            string newName = _packet.ReadString();
            int from = _packet.ReadInt();
            MyMod.playersData[from].m_Name = newName;
        }
        public static void CLOTH(Packet _packet)
        {
            MyMod.PlayerClothingData ClotchData = _packet.ReadClothingData();
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
            MyMod.BrokenFurnitureSync furn = _packet.ReadFurn();

            MyMod.OnFurnitureDestroyed(furn.m_Guid, furn.m_ParentGuid, furn.m_LevelID, furn.m_LevelGUID, false);
        }
        public static void FURNBROKENLIST(Packet _packet)
        {
            int howmany = _packet.ReadInt();

            for (int i = 1; i <= howmany; i++)
            {
                MyMod.BrokenFurnitureSync furn = _packet.ReadFurn();

                if(MyMod.BrokenFurniture.Contains(furn) == false)
                {
                    MyMod.OnFurnitureDestroyed(furn.m_Guid, furn.m_ParentGuid, furn.m_LevelID, furn.m_LevelGUID, false);
                }
            }
        }
        public static void FURNBREAKINGGUID(Packet _packet)
        {
            MyMod.BrokenFurnitureSync furn = _packet.ReadFurn();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_BrakingObject = furn;

                if (MyMod.playersData[from].m_Levelid == MyMod.levelid && MyMod.playersData[from].m_LevelGuid == MyMod.level_guid)
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
                MyMod.playersData[from].m_BrakingObject = new MyMod.BrokenFurnitureSync();
                MyMod.playersData[from].m_BrakingSounds = "";
            }
        }
        public static void GEARPICKUP(Packet _packet)
        {
            MyMod.PickedGearSync gear = _packet.ReadPickedGear();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null && MyMod.playersData[from].m_Levelid == MyMod.levelid && MyMod.playersData[from].m_LevelGuid == MyMod.level_guid)
            {
                if(MyMod.players[from] != null && MyMod.players[from].GetComponent<MyMod.MultiplayerPlayerAnimator>() != null)
                {
                    MyMod.players[from].GetComponent<MyMod.MultiplayerPlayerAnimator>().Pickup();
                }
            }

            MyMod.AddPickedGear(gear.m_Spawn, gear.m_LevelID, gear.m_LevelGUID, from, gear.m_MyInstanceID, false);
        }
        public static void GEARPICKUPLIST(Packet _packet)
        {
            int howmany = _packet.ReadInt();

            for (int i = 1; i <= howmany; i++)
            {
                MyMod.PickedGearSync gear = _packet.ReadPickedGear();

                if (MyMod.PickedGears.Contains(gear) == false)
                {
                    MyMod.AddPickedGear(gear.m_Spawn, gear.m_LevelID, gear.m_LevelGUID, 0, gear.m_MyInstanceID, false);
                }
            }
        }
        public static void ROPE(Packet _packet)
        {
            MyMod.ClimbingRopeSync rope = _packet.ReadRope();

            MyMod.AddDeployedRopes(rope.m_Position, rope.m_Deployed, rope.m_Snapped, rope.m_LevelID, rope.m_LevelGUID, false);
        }
        public static void ROPELIST(Packet _packet)
        {
            int howmany = _packet.ReadInt();

            for (int i = 1; i <= howmany; i++)
            {
                MyMod.ClimbingRopeSync rope = _packet.ReadRope();

                if (MyMod.DeployedRopes.Contains(rope) == false)
                {
                    MyMod.AddDeployedRopes(rope.m_Position, rope.m_Deployed, rope.m_Snapped, rope.m_LevelID, rope.m_LevelGUID, false);
                }
            }
        }
        public static void CONSUME(Packet _packet)
        {
            bool IsDrink = _packet.ReadBool();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null && MyMod.playersData[from].m_Levelid == MyMod.levelid && MyMod.playersData[from].m_LevelGuid == MyMod.level_guid)
            {
                if (MyMod.players[from] != null && MyMod.players[from].GetComponent<MyMod.MultiplayerPlayerAnimator>() != null)
                {
                    MyMod.players[from].GetComponent<MyMod.MultiplayerPlayerAnimator>().m_IsDrink = IsDrink;
                    MyMod.players[from].GetComponent<MyMod.MultiplayerPlayerAnimator>().Consumption();
                }
            }
        }
        public static void SERVERCFG(Packet _packet)
        {
            MyMod.ServerConfigData cfg = _packet.ReadServerCFG();
            MyMod.ServerConfig = cfg;
            MelonLogger.Msg(ConsoleColor.Green, "[Server Config Data] Data updated");
            MelonLogger.Msg(ConsoleColor.Blue, "m_FastConsumption: " + MyMod.ServerConfig.m_FastConsumption);
            MelonLogger.Msg(ConsoleColor.Blue, "m_DuppedSpawns: " + MyMod.ServerConfig.m_DuppedSpawns);
            MelonLogger.Msg(ConsoleColor.Blue, "m_DuppedContainers: " + MyMod.ServerConfig.m_DuppedContainers);
            MelonLogger.Msg(ConsoleColor.Blue, "m_PlayersSpawnType: " + MyMod.ServerConfig.m_PlayersSpawnType);
        }
        public static void STOPCONSUME(Packet _packet)
        {
            string LastAnim = _packet.ReadString();
            int from = _packet.ReadInt();

            if (MyMod.playersData[from] != null)
            {
                if(MyMod.playersData[from].m_Levelid == MyMod.levelid && MyMod.playersData[from].m_LevelGuid == MyMod.level_guid)
                {
                    if (MyMod.players[from] != null && MyMod.players[from].GetComponent<MyMod.MultiplayerPlayerAnimator>() != null)
                    {
                        MyMod.players[from].GetComponent<MyMod.MultiplayerPlayerAnimator>().StopConsumption();
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
                MyMod.LowHealthStaggerBlockTime = 5;
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
            MyMod.PickedGearSync gear = _packet.ReadPickedGear();
            int from = _packet.ReadInt();

            MelonLogger.Msg(ConsoleColor.Blue, "Other shokal has pickup item before me! I need to delete my picked gear Item InstanceID "+ gear.m_MyInstanceID);

            int _IID = gear.m_MyInstanceID;

            Il2CppSystem.Collections.Generic.List<GearItemObject> invItems = GameManager.GetInventoryComponent().m_Items;
            for (int i = 0; i < invItems.Count; i++)
            {
                GearItemObject currGear = invItems.get_Item(i);
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
                GearItem currentGear = GearManager.m_Gear.get_Item(i);
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
            MyMod.ContainerOpenSync box = _packet.ReadContainer();
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
            MyMod.ContainerOpenSync box = _packet.ReadContainer();
            int from = _packet.ReadInt();
            MyMod.AddLootedContainer(box, false, from);
        }
        public static void LOOTEDCONTAINERLIST(Packet _packet)
        {
            int howmany = _packet.ReadInt();

            for (int i = 1; i <= howmany; i++)
            {
                MyMod.ContainerOpenSync box = _packet.ReadContainer();

                if (MyMod.LootedContainers.Contains(box) == false)
                {
                    MyMod.AddLootedContainer(box, false);
                }
            }
        }
        public static void HARVESTPLANT(Packet _packet)
        {
            MyMod.HarvestableSyncData harveData = _packet.ReadHarvestablePlant();
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
            int from = _packet.ReadInt();
            MyMod.AddHarvastedPlant(plantGUID, from);
        }
        public static void LOOTEDHARVESTABLEALL(Packet _packet)
        {
            int howmany = _packet.ReadInt();

            for (int i = 1; i <= howmany; i++)
            {
                string plantGUID = _packet.ReadString();

                if (MyMod.HarvestedPlants.Contains(plantGUID) == false)
                {
                    MyMod.AddHarvastedPlant(plantGUID, 0);
                }
            }
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
            MyMod.ShowShelterByOther shelter = _packet.ReadShelter();
            int from = _packet.ReadInt();
            MyMod.ShelterCreated(shelter.m_Position, shelter.m_Rotation, shelter.m_LevelID, shelter.m_LevelGUID, false);
        }
        public static void REMOVESHELTER(Packet _packet)
        {
            MelonLogger.Msg("Someone removed shelter!");
            MyMod.ShowShelterByOther shelter = _packet.ReadShelter();
            int from = _packet.ReadInt();
            MyMod.ShelterRemoved(shelter.m_Position, shelter.m_LevelID, shelter.m_LevelGUID, false);
        }
        public static void ALLSHELTERS(Packet _packet)
        {
            int howmany = _packet.ReadInt();

            for (int i = 1; i <= howmany; i++)
            {
                MyMod.ShowShelterByOther shelter = _packet.ReadShelter();

                if (MyMod.ShowSheltersBuilded.Contains(shelter) == false)
                {
                    MyMod.ShelterCreated(shelter.m_Position, shelter.m_Rotation, shelter.m_LevelID, shelter.m_LevelGUID, false);
                }
            }
        }
        public static void USESHELTER(Packet _packet)
        {
            MyMod.ShowShelterByOther shelter = _packet.ReadShelter();
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
            MyMod.FireSourcesSync FireSource = _packet.ReadFire();
            int from = _packet.ReadInt();
            MyMod.MayAddFireSources(FireSource);
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
        public static void GOTITEMSLICE(Packet _packet)
        {
            MyMod.SlicedJsonData got = _packet.ReadSlicedGear();
            MyMod.AddSlicedJsonData(got);
        }
        public static void VOICECHAT(Packet _packet)
        {
            int readLength = _packet.ReadInt();
            int samples = _packet.ReadInt();
            byte[] CompressedData = _packet.ReadBytes(readLength);
            int from = _packet.ReadInt();
            MyMod.ProcessVoiceChatData(from, CompressedData, samples);
        }
        public static void SLICEDBYTES(Packet _packet)
        {
            MyMod.SlicedBytesData got = _packet.ReadSlicedBytes();
            int from = _packet.ReadInt();
            MyMod.AddSlicedBytesData(got, from);
        }
        public static void SLEEPPOSE(Packet _packet)
        {
            Vector3 Position = _packet.ReadVector3();
            Quaternion Rotation = _packet.ReadQuaternion();
            int from = _packet.ReadInt();
            if (MyMod.playersData[from] != null)
            {
                MyMod.playersData[from].m_SleepV3 = Position;
                MyMod.playersData[from].m_SleepQuat = Rotation;
            }
        }
    }
}
