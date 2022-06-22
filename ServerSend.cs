using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using SkyCoop;

namespace GameServer
{
    class ServerSend
    {
        public static void SendTCPData(int _toClient, Packet _packet)
        {
            SendUDPData(_toClient, _packet);
        }
        public static void SendUDPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].udp.SendData(_packet);
        }

        public static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
        public static void SendTCPDataToAll(int _exceptClient, Packet _packet)
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

        public static void SendUDPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (Server.clients[i].IsBusy() == true)
                {
                    Server.clients[i].udp.SendData(_packet);
                }
            }
        }
        public static void SendUDPDataToAll(Packet _packet, string level_guid)
        {
            _packet.WriteLength();
            for (int i = 1; i < MyMod.playersData.Count; i++)
            {
                if (MyMod.playersData[i] != null)
                {
                    if (MyMod.playersData[i].m_LevelGuid == level_guid)
                    {
                        Server.clients[i].udp.SendData(_packet);
                    }
                }
            }
        }
        public static void SendUDPDataToAllButNotSender(Packet _packet, int SenderId)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != SenderId && Server.clients[i].IsBusy() == true)
                {
                    Server.clients[i].udp.SendData(_packet);
                }
            }
        }
        public static void SendUDPDataToAllButNotSender(Packet _packet, int SenderId, string level_guid)
        {
            _packet.WriteLength();
            for (int i = 1; i < MyMod.playersData.Count; i++)
            {
                if (i != SenderId && MyMod.playersData[i] != null)
                {
                    if (MyMod.playersData[i].m_LevelGuid == level_guid)
                    {
                        Server.clients[i].udp.SendData(_packet);
                    }
                }
            }
        }
        public static void SendUDPDataToAllButNotSender(Packet _packet, int SenderId, string level_guid, float RadioF)
        {
            _packet.WriteLength();
            for (int i = 1; i < MyMod.playersData.Count; i++)
            {
                if (i != SenderId && MyMod.playersData[i] != null)
                {                    
                    if (MyMod.playersData[i].m_LevelGuid == level_guid || MyMod.playersData[i].m_RadioFrequency == RadioF)
                    {
                        Server.clients[i].udp.SendData(_packet);
                    }
                }
            }
        }
        /// <summary>Sends a packet to all clients except one via UDP.</summary>
        /// <param name="_exceptClient">The client to NOT send the data to.</param>
        /// <param name="_packet">The packet to send.</param>
        public static void SendUDPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].udp.SendData(_packet);
                }
            }
        }

        public static void Welcome(int _toClient, int maxPlayers)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_toClient);
                _packet.Write(maxPlayers);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void XYZ(int _From, Vector3 _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.XYZ))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }else{

                    if(OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }else{
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void XYZDW(int _toClient, Vector3 _msg)
        {
            //using (Packet _packet = new Packet((int)ServerPackets.XYZDW))
            //{
            //    _packet.Write(_msg);
            //    _packet.Write(5);
            //    SendUDPDataToAll(_packet);
            //}
        }
        public static void XYZW(int _From, Quaternion _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.XYZW))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }else{

                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }else{
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void BLOCK(int _From, Vector3 _msg, bool toEveryOne)
        {
            using (Packet _packet = new Packet((int)ServerPackets.BLOCK))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }else{
                    _packet.Write(_msg);
                    _packet.Write(_From);
                    SendUDPDataToAllButNotSender(_packet, _From);
                }
            }
        }
        public static void LEVELID(int _From, int _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LEVELID))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }else{
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }else{
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void LEVELGUID(int _From, string _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LEVELGUID))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void GOTITEM(int _toClient, MyMod.GearItemDataPacket _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.GOTITEM))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void GOTITEMSLICE(int _toClient, MyMod.SlicedJsonData _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.GOTITEMSLICE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void GOTCONTAINERSLICE(int _toClient, MyMod.SlicedJsonData _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.GOTCONTAINERSLICE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                int pSize = _packet.Length();

                if (pSize > 1500)
                {
                    MelonLoader.MelonLogger.Msg("Packet reached limiet! Size of packet is " + pSize + "bytes");
                }else{
                    //MelonLoader.MelonLogger.Msg("Packet sent "+ pSize + "bytes");
                }

                //MelonLoader.MelonLogger.Msg("Limit is 1500bytes, slice split size is 1000bytes, final packet size is " + _packet.Length()+"bytes");
                SendTCPData(_toClient, _packet);
            }
        }
        public static void OPENEMPTYCONTAINER(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.OPENEMPTYCONTAINER))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void GETREQUESTEDITEMSLICE(int _toClient, MyMod.SlicedJsonData _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.GETREQUESTEDITEMSLICE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void GETREQUESTEDFORPLACESLICE(int _toClient, MyMod.SlicedJsonData _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.GETREQUESTEDFORPLACESLICE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void MARKSEARCHEDCONTAINERS(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.MARKSEARCHEDCONTAINERS))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void GAMETIME(string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.GAMETIME))
            {
                _packet.Write(_msg);
                _packet.Write(MyMod.MinutesFromStartServer);
                _packet.Write(0);

                SendUDPDataToAll(_packet);
            }
        }
        public static void LIGHTSOURCENAME(int _From, string _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LIGHTSOURCENAME))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }else{
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }else{
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void LIGHTSOURCE(int _From, bool _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LIGHTSOURCE))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }else{
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }else{
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void ANIMSTATE(int _from, string _msg, bool toEveryOne)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMSTATE))
            {
                if(toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);

                    SendUDPDataToAll(_packet);
                }else{
                    _packet.Write(_msg);
                    _packet.Write(_from);
                    SendUDPDataToAllButNotSender(_packet, _from);
                }
            }
        }
        public static void SYNCWEATHER(int _toClient, MyMod.WeatherProxies _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SYNCWEATHER))
            {
                _packet.Write(_msg);
                _packet.Write(0);

                SendUDPDataToAll(_packet);
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
        public static void ANIMALROLE(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMALROLE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        //public static void SendOnlyToClosePlayers(Packet _packet, int _From, int LevelID, Vector3 V3)
        //{
        //    _packet.WriteLength();
        //    for (int i = 1; i <= Server.MaxPlayers; i++)
        //    {
        //        float maxDis = MyMod.MaxAniamlsSyncDistance;
        //        if (i != _From && Server.clients[i].IsBusy() == true && MyMod.players[i] != null && MyMod.playersData[i] != null && MyMod.playersData[i].m_Levelid == LevelID && Vector3.Distance(MyMod.playersData[i].m_Position, V3) < maxDis)
        //        {
        //            Server.clients[i].udp.SendData(_packet);
        //        }
        //    }
        //}

        //public static void ANIMALSYNC(int _From, MyMod.AnimalSync _msg, bool toEveryOne, int LevelID, Vector3 AnimalV3)
        //{
        //    using (Packet _packet = new Packet((int)ServerPackets.ANIMALSYNC))
        //    {
        //        _packet.Write(_msg);
        //        _packet.Write(_From);
        //        SendOnlyToClosePlayers(_packet, _From, LevelID, AnimalV3);
        //    }
        //}
        public static void ANIMALTEST(int _From, MyMod.AnimalCompactData _msg, MyMod.AnimalAnimsSync anim, int OnlyFor)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMALTEST))
            {
                _packet.Write(_msg);
                _packet.Write(anim);
                _packet.Write(_From);
                SendUDPData(OnlyFor, _packet);
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
        public static void ANIMALSYNCTRIGG(int _From, MyMod.AnimalTrigger _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMALSYNCTRIGG))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void SHOOTSYNC(int _From, MyMod.ShootSync _msg, bool toEveryOne)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SHOOTSYNC))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    _packet.Write(_msg);
                    _packet.Write(_From);
                    SendUDPDataToAllButNotSender(_packet, _From);
                }
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
        public static void HARVESTINGANIMAL(int _From, string _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.HARVESTINGANIMAL))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }else{
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }else{
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
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
        public static void BULLETDAMAGE(int _toClient, float _msg,int bodypart, int _From, bool Melee = false, string MeleeWeapon = "")
        {
            using (Packet _packet = new Packet((int)ServerPackets.BULLETDAMAGE))
            {
                _packet.Write(_msg);
                _packet.Write(bodypart);
                _packet.Write(_From);
                _packet.Write(Melee);

                if(Melee && MeleeWeapon != "")
                {
                    _packet.Write(MeleeWeapon);
                }

                SendTCPData(_toClient, _packet);
            }
        }
        public static void MULTISOUND(int _From, string _msg, bool toEveryOne)
        {
            using (Packet _packet = new Packet((int)ServerPackets.MULTISOUND))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    _packet.Write(_msg);
                    _packet.Write(_From);
                    SendUDPDataToAllButNotSender(_packet, _From);
                }
            }
        }
        public static void CONTAINEROPEN(int _From, MyMod.ContainerOpenSync _msg, bool toEveryOne)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CONTAINEROPEN))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    _packet.Write(_msg);
                    _packet.Write(_From);
                    SendUDPDataToAllButNotSender(_packet, _From);
                }
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
        public static void ANIMALDELETE(int from, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMALDELETE))
            {
                _packet.Write(_msg);
                _packet.Write(from);

                SendUDPDataToAllButNotSender(_packet, from);
            }
        }
        public static void KEEPITALIVE(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.KEEPITALIVE))
            {
                _packet.Write(_msg);
                _packet.Write(0);

                SendUDPDataToAll(_packet);
            }
        }
        public static void RQRECONNECT(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.RQRECONNECT))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void EQUIPMENT(int _From, MyMod.PlayerEquipmentData _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.EQUIPMENT))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }else{
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }else{
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void CHAT(int _From, MyMod.MultiplayerChatMessage _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CHAT))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void CHANGENAME(int _From, string _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CHANGENAME))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void CLOTH(int _From, MyMod.PlayerClothingData _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CLOTH))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void ASKSPAWNDATA(int _From, int _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ASKSPAWNDATA))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void FURNBROKEN(int _From, MyMod.BrokenFurnitureSync _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.FURNBROKEN))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void FURNBROKENLIST(int OnlyFor)
        {
            using (Packet _packet = new Packet((int)ServerPackets.FURNBROKENLIST))
            {
                int ReadCount = MyMod.BrokenFurniture.Count;

                if(ReadCount == 0)
                {
                    return;
                }

                _packet.Write(ReadCount);

                for (int i = 0; i < ReadCount; i++)
                {
                    _packet.Write(MyMod.BrokenFurniture[i]);
                }
                SendUDPData(OnlyFor, _packet);
            }
        }
        public static void FURNBREAKINGGUID(int _From, MyMod.BrokenFurnitureSync _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.FURNBREAKINGGUID))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void FURNBREAKINSTOP(int _From, bool _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.FURNBREAKINSTOP))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }

        public static void GEARPICKUP(int _From, MyMod.PickedGearSync _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.GEARPICKUP))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void GEARPICKUPLIST(int OnlyFor)
        {
            using (Packet _packet = new Packet((int)ServerPackets.GEARPICKUPLIST))
            {
                int ReadCount = MyMod.PickedGears.Count;

                if (ReadCount == 0)
                {
                    return;
                }

                _packet.Write(ReadCount);

                for (int i = 0; i < ReadCount; i++)
                {
                    _packet.Write(MyMod.PickedGears[i]);
                }
                SendUDPData(OnlyFor, _packet);
            }
        }

        public static void ROPE(int _From, MyMod.ClimbingRopeSync _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ROPE))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void ROPELIST(int OnlyFor)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ROPELIST))
            {
                int ReadCount = MyMod.DeployedRopes.Count;

                if (ReadCount == 0)
                {
                    return;
                }

                _packet.Write(ReadCount);

                for (int i = 0; i < ReadCount; i++)
                {
                    _packet.Write(MyMod.DeployedRopes[i]);
                }
                SendUDPData(OnlyFor, _packet);
            }
        }
        public static void CONSUME(int _From, bool _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CONSUME))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void SERVERCFG(int OnlyFor)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SERVERCFG))
            {
                _packet.Write(MyMod.ServerConfig);
                SendUDPData(OnlyFor, _packet);
            }
        }
        public static void SERVERCFGUPDATED()
        {
            using (Packet _packet = new Packet((int)ServerPackets.SERVERCFG))
            {
                _packet.Write(MyMod.ServerConfig);
                SendUDPDataToAll(_packet);
            }
        }
        public static void STOPCONSUME(int _From, string _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.STOPCONSUME))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void HEAVYBREATH(int _From, bool _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.HEAVYBREATH))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }

        public static void BLOODLOSTS(int _From, int _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.BLOODLOSTS))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void APPLYACTIONONPLAYER(int _From, string _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.APPLYACTIONONPLAYER))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void DONTMOVEWARNING(int _From, bool _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.DONTMOVEWARNING))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void INFECTIONSRISK(int _From, bool _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.DONTMOVEWARNING))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void CANCLEPICKUP(int _From, MyMod.PickedGearSync _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CANCLEPICKUP))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void CONTAINERINTERACT(int _From, MyMod.ContainerOpenSync _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CONTAINERINTERACT))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void LOOTEDCONTAINER(int _From, MyMod.ContainerOpenSync _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LOOTEDCONTAINER))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void LOOTEDCONTAINERLIST(int OnlyFor)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LOOTEDCONTAINERLIST))
            {
                int ReadCount = MyMod.LootedContainers.Count;

                if (ReadCount == 0)
                {
                    return;
                }

                _packet.Write(ReadCount);

                for (int i = 0; i < ReadCount; i++)
                {
                    _packet.Write(MyMod.LootedContainers[i]);
                }
                SendUDPData(OnlyFor, _packet);
            }
        }
        public static void HARVESTPLANT(int _From, MyMod.HarvestableSyncData _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.HARVESTPLANT))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void LOOTEDHARVESTABLE(int _From, string _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LOOTEDHARVESTABLE))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void LOOTEDHARVESTABLEALL(int OnlyFor)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LOOTEDHARVESTABLEALL))
            {
                int ReadCount = MyMod.HarvestedPlants.Count;

                if (ReadCount == 0)
                {
                    return;
                }

                _packet.Write(ReadCount);

                for (int i = 0; i < ReadCount; i++)
                {
                    _packet.Write(MyMod.HarvestedPlants[i]);
                }
                SendUDPData(OnlyFor, _packet);
            }
        }
        public static void SELECTEDCHARACTER(int _From, int _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SELECTEDCHARACTER))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void ADDSHELTER(int _From, MyMod.ShowShelterByOther _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ADDSHELTER))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void REMOVESHELTER(int _From, MyMod.ShowShelterByOther _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.REMOVESHELTER))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void ALLSHELTERS(int OnlyFor)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ALLSHELTERS))
            {
                int ReadCount = MyMod.ShowSheltersBuilded.Count;

                if (ReadCount == 0)
                {
                    return;
                }

                _packet.Write(ReadCount);

                for (int i = 0; i < ReadCount; i++)
                {
                    _packet.Write(MyMod.ShowSheltersBuilded[i]);
                }
                SendUDPData(OnlyFor, _packet);
            }
        }
        public static void USESHELTER(int _From, MyMod.ShowShelterByOther _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.USESHELTER))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void FIRE(int _From, MyMod.FireSourcesSync _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.FIRE))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }else{
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }else{
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void FIREFUEL(int _From, MyMod.FireSourcesSync _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.FIREFUEL))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }

        public static void DROPITEM(int _From, MyMod.DroppedGearItemDataPacket _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.DROPITEM))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }

        public static void PICKDROPPEDGEAR(int _From, int _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.PICKDROPPEDGEAR))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(_From);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }

        public static void SLEEPPOSE(int _From, Vector3 xyzui, Quaternion rotation, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SLEEPPOSE))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(xyzui);
                    _packet.Write(rotation);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(xyzui);
                        _packet.Write(rotation);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(xyzui);
                        _packet.Write(rotation);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }

        public static void CUSTOM(int _From, byte _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CUSTOM))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void KICKMESSAGE(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.KICKMESSAGE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void VOICECHAT(int _From, byte[] _msg, int ReadLength, float RecordTime, string LevelGUID, float RadioF, int Sender)
        {
            using (Packet _packet = new Packet((int)ServerPackets.VOICECHAT))
            {
                _packet.Write(ReadLength);
                _packet.Write(_msg.Length);
                _packet.Write(_msg);
                _packet.Write(RecordTime);
                _packet.Write(RadioF);
                _packet.Write(_From);
                _packet.Write(Sender);
                SendUDPDataToAllButNotSender(_packet, _From, LevelGUID, RadioF);
            }
        }

        public static void ANIMALDAMAGE(int _From, string guid, float damage)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMALDAMAGE))
            {
                _packet.Write(guid);
                _packet.Write(damage);
                SendUDPDataToAllButNotSender(_packet, _From);
            }
        }

        public static void SLICEDBYTES(int _From, MyMod.SlicedBytesData _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SLICEDBYTES))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }

        public static void READYSENDNEXTSLICE(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.READYSENDNEXTSLICE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void LOADINGSCENEDROPSDONE(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.LOADINGSCENEDROPSDONE))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void USEOPENABLE(int _toClient, string _GUID, bool state, bool toAll = false)
        {
            using (Packet _packet = new Packet((int)ServerPackets.USEOPENABLE))
            {
                _packet.Write(_GUID);
                _packet.Write(state);
                _packet.Write(_toClient);

                if (toAll == false)
                {
                    SendTCPData(_toClient, _packet);
                }else{
                    SendUDPDataToAll(_packet);
                }
            }
        }
        public static void GEARNOTEXIST(int _toClient, bool _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.GEARNOTEXIST))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void TRYDIAGNISISPLAYER(int _toClient, int _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.TRYDIAGNISISPLAYER))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void TRYBORROWGEAR(int _toClient, int _fromClient, string GearName)
        {
            using (Packet _packet = new Packet((int)ServerPackets.TRYBORROWGEAR))
            {
                _packet.Write(GearName);
                _packet.Write(_fromClient);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void CUREAFFLICTION(int _toClient, MyMod.AffictionSync _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CUREAFFLICTION))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void SENDMYAFFLCTIONS(int _toClient, List<MyMod.AffictionSync> _msg, float hp, int fromWho)
        {
            using (Packet _packet = new Packet((int)ServerPackets.SENDMYAFFLCTIONS))
            {
                _packet.Write(fromWho);
                _packet.Write(_msg.Count);
                _packet.Write(hp);
                for (int index = 0; index < _msg.Count; ++index)
                {
                    _packet.Write(_msg[index]);
                }
                SendTCPData(_toClient, _packet);
            }
        }
        public static void CHANGEAIM(int _From, bool _msg, bool toEveryOne, int OnlyFor = -1)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CHANGEAIM))
            {
                if (toEveryOne == true)
                {
                    _packet.Write(_msg);
                    _packet.Write(0);
                    SendUDPDataToAll(_packet);
                }
                else
                {
                    if (OnlyFor == -1)
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPDataToAllButNotSender(_packet, _From);
                    }
                    else
                    {
                        _packet.Write(_msg);
                        _packet.Write(_From);
                        SendUDPData(OnlyFor, _packet);
                    }
                }
            }
        }
        public static void ANIMALCORPSE(int _toClient, MyMod.AnimalKilled data, bool toAll = false)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMALCORPSE))
            {
                _packet.Write(data);
                _packet.Write(_toClient);

                if (toAll == false)
                {
                    SendTCPData(_toClient, _packet);
                }else{
                    SendUDPDataToAll(_packet);
                }
            }
        }
        public static void REQUESTANIMALCORPSE(int _toClient, float Meat, int Guts, int Hide)
        {
            using (Packet _packet = new Packet((int)ServerPackets.REQUESTANIMALCORPSE))
            {
                _packet.Write(Meat);
                _packet.Write(Guts);
                _packet.Write(Hide);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void GOTRABBIT(int _toClient, int result)
        {
            using (Packet _packet = new Packet((int)ServerPackets.GOTRABBIT))
            {
                _packet.Write(result);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        public static void QUARTERANIMAL(int _toClient, string data, bool toAll = false)
        {
            using (Packet _packet = new Packet((int)ServerPackets.QUARTERANIMAL))
            {
                _packet.Write(data);
                _packet.Write(_toClient);

                if (toAll == false)
                {
                    SendTCPData(_toClient, _packet);
                }else{
                    SendUDPDataToAll(_packet);
                }
            }
        }
        public static void ANIMALAUDIO(int _from, string soundID, string GUID, string LEVELGUID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMALAUDIO))
            {
                _packet.Write(false);
                _packet.Write(GUID);
                _packet.Write(soundID);
                _packet.Write(_from);

                SendUDPDataToAllButNotSender(_packet, _from, LEVELGUID);
            }
        }
        public static void ANIMALAUDIO(int _from, int soundID, string GUID, string LEVELGUID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ANIMALAUDIO))
            {
                _packet.Write(true);
                _packet.Write(GUID);
                _packet.Write(soundID);
                _packet.Write(_from);

                SendUDPDataToAllButNotSender(_packet, _from, LEVELGUID);
            }
        }
        public static void RELEASERABBIT(int _from, string LEVELGUID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.RELEASERABBIT))
            {
                _packet.Write(_from);

                SendUDPDataToAllButNotSender(_packet, _from, LEVELGUID);
            }
        }
        public static void HITRABBIT(int For, string GUID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.HITRABBIT))
            {
                _packet.Write(GUID);

                SendUDPData(For, _packet);
            }
        }
        public static void RABBITREVIVED(int _from, Vector3 v3, string LEVELGUID)
        {
            using (Packet _packet = new Packet((int)ServerPackets.RABBITREVIVED))
            {
                _packet.Write(v3);

                SendUDPDataToAllButNotSender(_packet, _from, LEVELGUID);
            }
        }
        public static void MELEESTART(int _from)
        {
            using (Packet _packet = new Packet((int)ServerPackets.MELEESTART))
            {
                _packet.Write(_from);

                SendUDPDataToAllButNotSender(_packet, _from);
            }
        }
        public static void CHALLENGEINIT(int ID, int StartFrom)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CHALLENGEINIT))
            {
                _packet.Write(ID);
                _packet.Write(StartFrom);

                SendUDPDataToAll(_packet);
            }
        }
        public static void CHALLENGEUPDATE(MyMod.CustomChallengeData DAT)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CHALLENGEUPDATE))
            {
                _packet.Write(DAT);

                SendUDPDataToAll(_packet);
            }
        }
        public static void CHALLENGETRIGGER(string TRIGGER)
        {
            using (Packet _packet = new Packet((int)ServerPackets.CHALLENGETRIGGER))
            {
                _packet.Write(TRIGGER);

                SendUDPDataToAll(_packet);
            }
        }
    }
}
