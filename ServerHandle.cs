using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SkyCoop;

namespace GameServer
{
    class ServerHandle
    {
        public static Vector3 boiVector3 = new Vector3(0,0,0);
        public static Quaternion boiQuat = new Quaternion(0,0,0,0);
        public static Vector3 LastBlockVector3 = new Vector3(0, 0, 0);
        public static bool DarkShatalkerMode = false;
        public static int clientlevelid = 0;
        public static int mylevelid = 0;
        public static string gametime = "12:0";
        public static string LastLightName = "";
        public static string LastAnimState = "Idle";
        public static bool LastLight = false;
        public static Vector3 LastFire = new Vector3(0, 0, 0);
        public static bool MyHasRifle = false;
        public static bool MyHasRevolver = false;
        public static bool MyHasAxe = false;
        public static bool LastHasRifle = false;
        public static bool LastHasRevolver = false;
        public static bool LastHasAxe = false;
        public static int MyArrows = 0;
        private static int LastArrows = 0;
        private static bool MyHasMedkit = false;
        private static bool LastHasMedkit = false;
        public static int OtherPlayerSleep = 0;
        public static bool CanSend = false;
        public static bool LastReadyState = false;
        public static bool IamShatalker = false;
        public static bool LastWardIsActive = false;
        public static bool NeedReloadDWReadtState = false;
        public static float LastCountDown = 900f;
        public static int OverflowAnimalsOnConnectTimer = 0;

        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            OverflowAnimalsOnConnectTimer = 10;
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
            CanSend = true;
            using (Packet __packet = new Packet((int)ServerPackets.LEVELID))
            {
                ServerSend.LEVELID(1, mylevelid);
            }
            using (Packet __packet = new Packet((int)ServerPackets.GAMETIME))
            {
                ServerSend.GAMETIME(1, gametime);
            }
            using (Packet __packet = new Packet((int)ServerPackets.HOSTISDARKWALKER))
            {
                ServerSend.HOSTISDARKWALKER(1, IamShatalker);
            }
            using (Packet __packet = new Packet((int)ServerPackets.HASRIFLE))
            {
                ServerSend.HASRIFLE(1, MyHasRifle);
            }
            using (Packet __packet = new Packet((int)ServerPackets.HASREVOLVER))
            {
                ServerSend.HASREVOLVER(1, MyHasRevolver);
            }
            using (Packet __packet = new Packet((int)ServerPackets.HASAXE))
            {
                ServerSend.HASAXE(1, MyHasAxe);
            }
            using (Packet __packet = new Packet((int)ServerPackets.HISARROWS))
            {
                ServerSend.HISARROWS(1, MyArrows);
            }
            MyMod.SendSlotData();
        }
        public static void XYZ(int _fromClient, Packet _packet)
        {
            Vector3 maboi;
            maboi = _packet.ReadVector3();
            boiVector3 = new Vector3(maboi.x, maboi.y + 0.03f, maboi.z);

            if (MyMod.IamShatalker == true)
            {
                bool NeedListIt = true;

                for (int i = 0; i < MyMod.SurvivorWalks.Count; i++)
                {
                    if (MyMod.SurvivorWalks[i].m_levelid == MyMod.anotherplayer_levelid)
                    {
                        NeedListIt = false;
                        MyMod.SurvivorWalks[i].m_V3 = maboi;
                        break;
                    }
                }
                if (NeedListIt == true)
                {
                    MyMod.WalkTracker ToAdd = new MyMod.WalkTracker();
                    ToAdd.m_levelid = MyMod.anotherplayer_levelid;
                    ToAdd.m_V3 = maboi;
                    MyMod.SurvivorWalks.Add(ToAdd);
                }
            }
        }
        public static void XYZDW(int _fromClient, Packet _packet)
        {
            DarkShatalkerMode = true;
            Vector3 maboi;
            maboi = _packet.ReadVector3();
            boiVector3 = maboi;         
        }
        public static void XYZW(int _fromClient, Packet _packet)
        {
            Quaternion maboi;
            maboi = _packet.ReadQuaternion();
            boiQuat = maboi;
        }
        public static void BLOCK(int _fromClient, Packet _packet)
        {
            Vector3 maboi;
            maboi = _packet.ReadVector3();

            LastBlockVector3 = maboi;
        }
        public static void LEVELID(int _fromClient, Packet _packet)
        {
            int maboi;
            maboi = _packet.ReadInt();

            clientlevelid = maboi;
        }
        public static void GOTITEM(int _fromClient, Packet _packet)
        {
            GearItem got = _packet.ReadGear();
            MyMod.GiveRecivedItem(got);
        }
        public static void GAMETIME(int _fromClient, Packet _packet)
        {
            string got = _packet.ReadString();

            uConsole.RunCommand("set_time " + got);
        }
        public static void LIGHTSOURCENAME(int _fromClient, Packet _packet)
        {
            string got = _packet.ReadString();
            LastLightName = got;
            Console.WriteLine("Other player changed source light: " + got);
        }
        public static void LIGHTSOURCE(int _fromClient, Packet _packet)
        {
            bool got = _packet.ReadBool();
            LastLight = got;
            Console.WriteLine("Other player toggle light: " + got);
        }
        public static void ANIMSTATE(int _fromClient, Packet _packet)
        {
            LastAnimState = _packet.ReadString();
            //Console.WriteLine("Other player changed animation: " + LastAnimState);
        }
        public static void HASRIFLE(int _fromClient, Packet _packet)
        {
            bool got = _packet.ReadBool();
            LastHasRifle = got;
            Console.WriteLine("Other player toggle rifle: " + got);
        }
        public static void HASREVOLVER(int _fromClient, Packet _packet)
        {
            bool got = _packet.ReadBool();
            LastHasRevolver = got;
            Console.WriteLine("Other player toggle revolver: " + got);
        }
        public static void HASAXE(int _fromClient, Packet _packet)
        {
            bool got = _packet.ReadBool();
            LastHasAxe = got;
            Console.WriteLine("Other player toggle axe: " + got);
        }
        public static void HISARROWS(int _fromClient, Packet _packet)
        {
            int got = _packet.ReadInt();
            LastArrows = got;
            Console.WriteLine("Other player changed count of arrows: " + got);
        }
        public static void HASMEDKIT(int _fromClient, Packet _packet)
        {
            bool got = _packet.ReadBool();
            LastHasMedkit = got;
            Console.WriteLine("Other player toggle medkit: " + got);
        }
        public static void SYNCWEATHER(int _fromClient, Packet _packet)
        {
            Console.WriteLine("Got useless event!");
        }
        public static void REVIVE(int _fromClient, Packet _packet)
        {
            MyMod.SimRevive();
            using (Packet __packet = new Packet((int)ServerPackets.REVIVEDONE))
            {
                ServerSend.REVIVEDONE(1, true);
            }
        }
        public static void REVIVEDONE(int _fromClient, Packet _packet)
        {
            GameManager.GetInventoryComponent().RemoveGearFromInventory("GEAR_MedicalSupplies_hangar", 1);
        }
        public static void SLEEPHOURS(int _fromClient, Packet _packet)
        {
            OtherPlayerSleep = _packet.ReadInt();
        }
        public static void DARKWALKERREADY(int _fromClient, Packet _packet)
        {
            LastReadyState = _packet.ReadBool();
            Console.WriteLine("Got new darkwalker ready state: " + LastReadyState);
        }
        public static void WARDISACTIVE(int _fromClient, Packet _packet)
        {
            LastWardIsActive = _packet.ReadBool();
        }
        public static void REQUESTDWREADYSTATE(int _fromClient, Packet _packet)
        {
            NeedReloadDWReadtState = true;
        }
        public static void DWCOUNTDOWN(int _fromClient, Packet _packet)
        {
            LastCountDown = _packet.ReadFloat();
            //Console.WriteLine("LastCountDown " + LastCountDown);
        }
        private static GearItem GetGearItemPrefab(string name) => Resources.Load(name).Cast<GameObject>().GetComponent<GearItem>();
        private static GameObject GetGearItemObject(string name) => Resources.Load(name).Cast<GameObject>();
        public static void SHOOTSYNC(int _fromClient, Packet _packet)
        {
            MyMod.ShootSync shoot = _packet.ReadShoot();
            MyMod.DoShootSync(shoot);
        }
        public static void PIMPSKILL(int _fromClient, Packet _packet)
        {
            int SkillTypeId = _packet.ReadInt();

            if (SkillTypeId == 1)
            {
                GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Rifle, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                MelonLoader.MelonLogger.Log("Got remote skill upgrade Rifle");
            }
            else if (SkillTypeId == 2)
            {
                GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Revolver, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                MelonLoader.MelonLogger.Log("Got remote skill upgrade Revolver");
            }
        }
        public static void HARVESTINGANIMAL(int _fromClient, Packet _packet)
        {
            string got = _packet.ReadString();
            MyMod.OtherHarvetingAnimal = got;
        }
        public static void DONEHARVASTING(int _fromClient, Packet _packet)
        {
            MyMod.HarvestStats got = _packet.ReadHarvest();
            MyMod.DoForcedHarvestAnimal(MyMod.OtherHarvetingAnimal, got);
        }
        public static void ANIMALSYNC(int _fromClient, Packet _packet)
        {
            MyMod.AnimalSync got = _packet.ReadAnimal();
            MyMod.DoAnimalSync(got);
        }
        public static void ANIMALSYNCTRIGG(int _fromClient, Packet _packet)
        {
            MyMod.AnimalTrigger got = _packet.ReadAnimalTrigger();
            MyMod.SetAnimalTriggers(got);
        }
        public static void BULLETDAMAGE(int _fromClient, Packet _packet)
        {
            float damage = _packet.ReadFloat();
            MyMod.DamageByBullet(damage);
        }
        public static void MULTISOUND(int _fromClient, Packet _packet)
        {
            string sound = _packet.ReadString();
            MyMod.PlayMultiplayer3dAduio(sound);
        }
        public static void CONTAINEROPEN(int _fromClient, Packet _packet)
        {
            MyMod.ContainerOpenSync box = _packet.ReadContainer();
            MyMod.DoSyncContainer(box);
        }
        public static void LUREPLACEMENT(int _fromClient, Packet _packet)
        {
            MyMod.WalkTracker sync = _packet.ReadWalkTracker();
            MyMod.LastLure = sync;
        }
        public static void LUREISACTIVE(int _fromClient, Packet _packet)
        {
            MyMod.LureIsActive = _packet.ReadBool();
        }
        public static void ALIGNANIMAL(int _fromClient, Packet _packet)
        {
            MyMod.AnimalAligner Alig = _packet.ReadAnimalAligner();
            MyMod.AlignAnimalWithProxy(Alig.m_Proxy, Alig.m_Guid);
        }
        public static void ASKFORANIMALPROXY(int _fromClient, Packet _packet)
        {
            string _guid = _packet.ReadString();
            string Proxy = "";
            for (int i = 0; i < BaseAiManager.m_BaseAis.Count; i++)
            {
                if (BaseAiManager.m_BaseAis[i] != null && BaseAiManager.m_BaseAis[i].gameObject != null)
                {
                    GameObject animal = BaseAiManager.m_BaseAis[i].gameObject;
                    if (animal.GetComponent<ObjectGuid>() != null && animal.GetComponent<ObjectGuid>().Get() == _guid)
                    {
                        Proxy = BaseAiManager.m_BaseAis[i].Serialize();
                        break;
                    }
                }
            }
            using (Packet __packet = new Packet((int)ServerPackets.ALIGNANIMAL))
            {
                MyMod.AnimalAligner Alig = new MyMod.AnimalAligner();

                Alig.m_Guid = _guid;
                Alig.m_Proxy = Proxy;

                ServerSend.ALIGNANIMAL(1, Alig);
            }
        }
        public static void CARRYBODY(int _fromClient, Packet _packet)
        {
            MyMod.IsCarringMe = _packet.ReadBool();
        }
        public static void BODYWARP(int _fromClient, Packet _packet)
        {
            string _LevelName = _packet.ReadString();

            MyMod.WarpBody(_LevelName);
        }
        public static void ANIMALDELETE(int _fromClient, Packet _packet)
        {
            string AnimalGuid = _packet.ReadString();
            MyMod.DeleteAnimal(AnimalGuid);
        }

        public static Vector3 ReturnLastBoi()
        {
            return boiVector3;
        }
        public static Quaternion ReturnLastBoiQuat()
        {
            return boiQuat;
        }
        public static Vector3 ReutrnLastBlock()
        {
            return LastBlockVector3;
        }
        public static bool ReturnShatalkerMode()
        {
            return DarkShatalkerMode;
        }
        public static int ReturnLastLevelID()
        {
            return clientlevelid;
        }
        public static void SetMyLevelId(int level)
        {
            mylevelid = level;
        }
        public static void SetGameTime(string gtime)
        {
            gametime = gtime;
        }
        public static string ReturnLastLight()
        {
            return LastLightName;
        }
        public static bool ReturnLastLightState()
        {
            return LastLight;
        }
        public static Vector3 ReturnLastFire()
        {
            return LastFire;
        }
        public static string ReturnLastAnimState()
        {
            return LastAnimState;
        }
        public static bool ReturnLastHasRifle()
        {
            return LastHasRifle;
        }
        public static bool ReturnLastHasRevolver()
        {
            return LastHasRevolver;
        }
        public static bool ReturnLastHasAxe()
        {
            return LastHasAxe;
        }
        public static int ReturnLastArrow()
        {
            return LastArrows;
        }
        public static bool ReturnLastHasMedkit()
        {
            return LastHasMedkit;
        }
        public static int ReturnLastSleeping()
        {
            return OtherPlayerSleep;
        }
        public static bool ReturnLastReadyState()
        {
            return LastReadyState;
        }
        public static bool ReturnLastWardIsActive()
        {
            return LastWardIsActive;
        }
        public static bool ReturnNeedReloadDWReadtState()
        {
            return NeedReloadDWReadtState;
        }
        public static float ReturnLastCountDown()
        {
            return LastCountDown;
        }
    }
}
