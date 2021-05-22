using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SkyCoop;

namespace GameServer
{
    class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }

        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void XYZ(int _toClient, Vector3 _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.XYZ))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void XYZDW(int _toClient, Vector3 _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.XYZDW))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void XYZW(int _toClient, Quaternion _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.XYZW))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void BLOCK(int _toClient, Vector3 _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.BLOCK))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void LEVELID(int _toClient, int _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LEVELID))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        //public static void GOTITEM(int _toClient, string _msg)
        public static void GOTITEM(int _toClient, GearItem _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.GOTITEM))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void GAMETIME(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.GAMETIME))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void LIGHTSOURCENAME(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LIGHTSOURCENAME))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void LIGHTSOURCE(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LIGHTSOURCE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void ANIMSTATE(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMSTATE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void HASRIFLE(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.HASRIFLE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void HASREVOLVER(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.HASREVOLVER))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void SLEEPHOURS(int _toClient, int _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SLEEPHOURS))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void SYNCWEATHER(int _toClient, MyMod.WeatherProxies _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SYNCWEATHER))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void REVIVE(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.REVIVE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void REVIVEDONE(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.REVIVEDONE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void HASAXE(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.HASAXE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void HISARROWS(int _toClient, int _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.HISARROWS))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void HASMEDKIT(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.HASMEDKIT))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void ANIMALROLE(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMALROLE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void ANIMALSYNC(int _toClient, MyMod.AnimalSync _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMALSYNC))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void DARKWALKERREADY(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.DARKWALKERREADY))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void HOSTISDARKWALKER(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.HOSTISDARKWALKER))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void WARDISACTIVE(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.WARDISACTIVE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void DWCOUNTDOWN(int _toClient, float _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.DWCOUNTDOWN))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void ANIMALSYNCTRIGG(int _toClient, MyMod.AnimalTrigger _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMALSYNCTRIGG))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void SHOOTSYNC(int _toClient, MyMod.ShootSync _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SHOOTSYNC))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void PIMPSKILL(int _toClient, int _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.PIMPSKILL))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void HARVESTINGANIMAL(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.HARVESTINGANIMAL))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void DONEHARVASTING(int _toClient, MyMod.HarvestStats _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.DONEHARVASTING))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void SAVEDATA(int _toClient, MyMod.SaveSlotSync _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SAVEDATA))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void BULLETDAMAGE(int _toClient, float _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.BULLETDAMAGE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void MULTISOUND(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.MULTISOUND))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void CONTAINEROPEN(int _toClient, MyMod.ContainerOpenSync _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CONTAINEROPEN))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void LUREPLACEMENT(int _toClient, MyMod.WalkTracker _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LUREPLACEMENT))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void LUREISACTIVE(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LUREISACTIVE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void ALIGNANIMAL(int _toClient, MyMod.AnimalAligner _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ALIGNANIMAL))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void ASKFORANIMALPROXY(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ASKFORANIMALPROXY))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void CARRYBODY(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CARRYBODY))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void BODYWARP(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.BODYWARP))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void ANIMALDELETE(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMALDELETE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
    }
}
