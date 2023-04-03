﻿using System;
using UnityEngine;
using Il2Cpp;
using System.Collections.Generic;

using GameServer;

namespace SkyCoop
{
    public class DeBugMenu
    {
        public static int DebugVal = 0;
        public static uint DebugSound = 0U;
        public static string ContainerOverride = "";

        public static DecalProjectorInstance Replica;
        public static bool Inited = false;

        public class DebugFunction 
        {
            public float X = 0;
            public float Y = 0;
            public Action Fn = null;
            public DebugFunction(float _X, float _Y, Action _Fn) 
            {
                X = _X;
                Y = _Y;
                Fn = _Fn;
            }
        }
        public static Dictionary<string, List<DebugFunction>> Categories = new Dictionary<string, List<DebugFunction>>();

        public void AddOption(string Tab, Action Fn)
        {
            List<DebugFunction> Options;
            if(!Categories.TryGetValue(Tab, out Options))
            {
                Categories.Add(Tab, new List<DebugFunction>());
                Categories.TryGetValue(Tab, out Options);
            }

            float X = 20;

            if(Tab == "Main")
            {
                X = 160;
            }
            int offset = 20;

            float Y = 10 + offset * Options.Count;

            Options.Add(new DebugFunction(X, Y, Fn));
        }

        public static void InitUI()
        {
            Inited = true;
        }

        public static void Render()
        {
            if (MyMod.DebugGUI == false)
            {
                return;
            }

            if (!Inited)
            {
                InitUI();
            }

            if (MyMod.AdvancedDebugMode != "")
            {
                int animalcount = BaseAiManager.m_BaseAis.Count;
                int animalActors = MyMod.ActorsList.Count;

                if (MyMod.AdvancedDebugMode == "AnimalsGUID" || MyMod.AdvancedDebugMode == "AnimalsAllStats")
                {
                    int offset = 20;

                    GUI.Label(new Rect(700, 10, 500, 100), "Animals: " + animalcount + " Actors: "+ animalActors + " Controller " + MyMod.AnimalsController);
                    GUI.Label(new Rect(700, 10 + offset * 1, 500, 100), "GUID: " + MyMod.DebugAnimalGUID);
                    GUI.Label(new Rect(700, 10 + offset * 2, 500, 100), "Last GUID: " + MyMod.DebugAnimalGUIDLast);

                    bool stillExists = true;

                    if (MyMod.DebugLastAnimal == null)
                    {
                        stillExists = false;
                    }
                    else
                    {
                        stillExists = true;
                    }
                    GUI.Label(new Rect(700, 10 + offset * 3, 500, 100), "Still exists: " + stillExists);

                    if (stillExists == true && MyMod.DebugLastAnimal != null && MyMod.AdvancedDebugMode == "AnimalsAllStats")
                    {
                        Comps.AnimalUpdates _AU = MyMod.DebugLastAnimal.GetComponent<Comps.AnimalUpdates>();
                        BaseAi _AI = MyMod.DebugLastAnimal.GetComponent<BaseAi>();

                        string spawnRegionString = "";

                        if(_AI.m_SpawnRegionParent != null)
                        {
                            string spGUID = "NONE";
                            if(_AI.m_SpawnRegionParent.GetComponent<ObjectGuid>() != null)
                            {
                                spGUID = _AI.m_SpawnRegionParent.GetComponent<ObjectGuid>().Get();
                            }
                            bool spActive = _AI.m_SpawnRegionParent.gameObject.activeSelf;
                            spawnRegionString = spActive + " (" + spGUID + ") "+ _AI.m_SpawnRegionParent.gameObject.name;
                        }else{
                            spawnRegionString = "Null";
                        }

                        if (_AU != null)
                        {
                            GUI.Label(new Rect(700, 10 + offset * 5, 500, 100), "m_Banned " + _AU.m_Banned);
                            GUI.Label(new Rect(700, 10 + offset * 6, 500, 100), "m_DampingIgnore " + _AU.m_DampingIgnore);
                            GUI.Label(new Rect(700, 10 + offset * 7, 500, 100), "NoResponce " + _AU.NoResponce);
                            GUI.Label(new Rect(700, 10 + offset * 8, 500, 100), "Object active " + MyMod.DebugLastAnimal.activeSelf + " Rendering "+ _AI.m_RenderersEnabled);
                            GUI.Label(new Rect(700, 10 + offset * 9, 500, 100), "RetakeCooldown " + _AU.ReTakeCoolDown);
                            GUI.Label(new Rect(700, 10 + offset * 10, 500, 100), "LastFoundPlayer " + _AU.LastFoundPlayer);
                            GUI.Label(new Rect(700, 10 + offset * 11, 500, 100), "Ai Mode " + _AI.GetAiMode());
                            GUI.Label(new Rect(700, 10 + offset * 12, 500, 100), "HP " + _AI.m_CurrentHP);
                            GUI.Label(new Rect(700, 10 + offset * 13, 500, 100), "m_HasEnteredStruggleOnLastAttack " + _AI.m_HasEnteredStruggleOnLastAttack);
                            GUI.Label(new Rect(700, 10 + offset * 14, 500, 100), "SpawnRegion " + spawnRegionString);

                            if (GameManager.m_PlayerObject != null)
                            {
                                GUI.Label(new Rect(700, 10 + offset * 18, 500, 100), "My Distance " + Vector3.Distance(MyMod.DebugLastAnimal.transform.position, GameManager.GetPlayerObject().transform.position));
                                for (int i = 0; i < MyMod.playersData.Count; i++)
                                {
                                    if(MyMod.playersData[i] != null)
                                    {
                                        int mult = 19 + i;
                                        GUI.Label(new Rect(700, 10 + offset * mult, 500, 100), "Player[" + i + "] Distance " + Vector3.Distance(MyMod.DebugLastAnimal.transform.position, MyMod.playersData[i].m_Position));
                                    }
                                }
                            }
                        }
                        else
                        {
                            GUI.Label(new Rect(700, 10 + offset * 39, 500, 100), "No AnimalUpdates!");
                        }
                    }
                    if(MyMod.iAmHost == true)
                    {
                        for (int i = 0; i < MyMod.MaxPlayers; i++)
                        {
                            int mult = 40 + i;

                            if(MyMod.playersData[i].m_Levelid != 0)
                            {
                                GUI.Label(new Rect(700, 10 + offset * mult, 500, 100), MyMod.playersData[i].m_Name + " Ticks: " + MyMod.playersData[i].m_TicksOnScene + " Scene " + MyMod.playersData[i].m_Levelid);
                            }
                        }
                    }
                }
            }


            GUI.Box(new Rect(10, 10, 100, 160), "Debug menu");

            if (GUI.Button(new Rect(20, 40, 80, 20), "Cube"))
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Transform target = GameManager.GetPlayerTransform();
                cube.transform.position = new Vector3(target.position.x, target.position.y, target.position.z);

                if (MyMod.sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.BLOCK))
                    {
                        _packet.Write(cube.transform.position);
                        MyMod.SendUDPData(_packet);
                    }
                }
                if (MyMod.iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.BLOCK))
                    {
                        ServerSend.BLOCK(0, cube.transform.position, true);
                    }
                }
            }
            if (GUI.Button(new Rect(20, 70, 80, 20), "Hour skip"))
            {
                Shared.SkipRTTime(1);
            }
            if (GUI.Button(new Rect(20, 100, 80, 20), "More"))
            {
                if (MyMod.UIDebugType != "")
                {
                    MyMod.UIDebugType = "";
                }
                else
                {
                    MyMod.UIDebugType = "Open";
                }
            }
            if (GUI.Button(new Rect(20, 130, 80, 20), "Advanced"))
            {
                if (MyMod.UIDebugType != "")
                {
                    MyMod.UIDebugType = "";
                }else{
                    MyMod.UIDebugType = "Advanced";
                }
            }

            if (MyMod.UIDebugType == "Advanced")
            {
                GUI.Box(new Rect(150, 10, 140, 200), "Crazy Debug Tools");
                if (GUI.Button(new Rect(160, 40, 120, 20), "UpdateBan="+MyMod.KillOnUpdate))
                {
                    MyMod.KillOnUpdate = !MyMod.KillOnUpdate;
                }
                if (GUI.Button(new Rect(160, 70, 120, 20), "Trace="+MyMod.CrazyPatchesLogger))
                {
                    MyMod.CrazyPatchesLogger = !MyMod.CrazyPatchesLogger;
                }
                if (GUI.Button(new Rect(160, 100, 120, 20), "Binds="+MyMod.DebugBind))
                {
                    MyMod.DebugBind = !MyMod.DebugBind;
                }
                if (GUI.Button(new Rect(160, 130, 120, 20), "Nuke patches"))
                {
                    HarmonyLib.Harmony.UnpatchAll();
                }
                if (GUI.Button(new Rect(160, 160, 120, 20), "SecondBan="+MyMod.KillEverySecond))
                {
                    MyMod.KillEverySecond = !MyMod.KillEverySecond;
                }
                if (GUI.Button(new Rect(160, 190, 120, 20), "MinuteBan=" + MyMod.KillEveryInGameMinute))
                {
                    MyMod.KillEveryInGameMinute = !MyMod.KillEveryInGameMinute;
                }
                if (GUI.Button(new Rect(160, 220, 120, 20), "Flag1=" + MyMod.Flag1))
                {
                    MyMod.Flag1 = !MyMod.Flag1;
                }
                if (GUI.Button(new Rect(160, 250, 120, 20), "Flag2=" + MyMod.Flag2))
                {
                    MyMod.Flag2 = !MyMod.Flag2;
                }
                if (GUI.Button(new Rect(160, 280, 120, 20), "Flag3=" + MyMod.Flag3))
                {
                    MyMod.Flag3 = !MyMod.Flag3;
                }
                if (GUI.Button(new Rect(160, 310, 120, 20), "Flag4=" + MyMod.Flag4))
                {
                    MyMod.Flag4 = !MyMod.Flag4;
                }
                if (GUI.Button(new Rect(160, 340, 120, 20), "Flag5=" + MyMod.Flag5))
                {
                    MyMod.Flag5 = !MyMod.Flag5;
                }
                if (GUI.Button(new Rect(160, 370, 120, 20), "Flag6=" + MyMod.Flag6))
                {
                    MyMod.Flag6 = !MyMod.Flag6;
                }
                if (GUI.Button(new Rect(160, 400, 120, 20), "Flag7=" + MyMod.Flag7))
                {
                    MyMod.Flag7 = !MyMod.Flag7;
                }
                if (GUI.Button(new Rect(160, 430, 120, 20), "Flag8=" + MyMod.Flag8))
                {
                    MyMod.Flag8 = !MyMod.Flag8;
                }
                if (GUI.Button(new Rect(160, 460, 120, 20), "Flag9=" + MyMod.Flag9))
                {
                    MyMod.Flag9 = !MyMod.Flag9;
                }
                if (GUI.Button(new Rect(160, 490, 120, 20), "Flag10=" + MyMod.Flag10))
                {
                    MyMod.Flag10 = !MyMod.Flag10;
                }
                if (GUI.Button(new Rect(160, 520, 120, 20), "Flag11=" + MyMod.Flag11))
                {
                    MyMod.Flag11 = !MyMod.Flag11;
                }
                if (GUI.Button(new Rect(160, 550, 120, 20), "Flag12=" + MyMod.Flag12))
                {
                    MyMod.Flag12 = !MyMod.Flag12;
                }
                if (GUI.Button(new Rect(160, 580, 120, 20), "Flag13=" + MyMod.Flag13))
                {
                    MyMod.Flag13 = !MyMod.Flag13;
                }
            }

            if (MyMod.UIDebugType == "Open")
            {
                GUI.Box(new Rect(150, 10, 100, 200), "Debug Tools");
                if (GUI.Button(new Rect(160, 40, 80, 20), "Player"))
                {
                    MyMod.UIDebugType = "PlayerDebug";
                }
                if (GUI.Button(new Rect(160, 70, 80, 20), "Teleport"))
                {
                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "INPUT ID OF PLAYER TELEPORT TO", "", Panel_Confirmation.ButtonLayout.Button_2, "TELEPORT", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                }
                if (GUI.Button(new Rect(160, 100, 80, 20), "Animals"))
                {
                    MyMod.UIDebugType = "AnimalsDebug";
                }
                if (GUI.Button(new Rect(160, 130, 80, 20), "Photo"))
                {
                    //string LoadSceneSTR = "BlackrockPrisonZone";
                    //GameManager.GetPlayerManagerComponent().m_SceneTransitionStarted = true;
                    //string str = (string)null;
                    //UnityEngine.SceneManagement.Scene activeScene;
                    //activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                    //str = GameManager.StripOptFromSceneName(activeScene.name);
                    //GameManager.m_SceneTransitionData.m_ForceSceneOnNextNavMapLoad = (string)null;
                    //GameManager.m_SceneTransitionData.m_ForceNextSceneLoadTriggerScene = str;
                    //GameManager.m_SceneTransitionData.m_SceneLocationLocIDToShow = "Cheating!";
                    //GameManager.m_SceneTransitionData.m_Location = (string)null;
                    //if (RegionManager.GetRegionFromString(activeScene.name, out GameRegion _))
                    //{
                    //    GameManager.m_SceneTransitionData.m_PosBeforeInteriorLoad = GameManager.GetPlayerTransform().position;
                    //}
                    //GameManager.m_SceneTransitionData.m_SceneSaveFilenameNextLoad = LoadSceneSTR;
                    //GameManager.LoadScene(LoadSceneSTR, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent);

                    //MyMod.DebugDiagnosis = true;

                    //Condition Con = GameManager.GetConditionComponent();
                    //Hunger Hun = GameManager.GetHungerComponent();
                    //Thirst Thi = GameManager.GetThirstComponent();

                    //MyMod.CheckOtherPlayer(MyMod.BuildMyAfflictionList(), 0, Con.m_CurrentHP, Con.m_MaxHP,Thi.m_CurrentThirst, Hun.m_CurrentReserveCalories, Hun.m_MaxReserveCalories);
                    //MyMod.MakeFakeFire(FireManager.m_Fires[0]);
                    //InterfaceManager.m_Panel_ActionsRadial.ShowToolsRadial();
                    //MyMod.AddSpray(Replica);
                    //MyMod.DropAll();
                    //MyMod.OriginalRadioSeaker = true;

                    //if (MyMod.ViewModelRadio)
                    //{
                    //    FirstPersonHandheldShortwave FPHS = MyMod.ViewModelRadio.GetComponent<FirstPersonHandheldShortwave>();
                    //    if (FPHS == null)
                    //    {
                    //        FPHS = MyMod.ViewModelRadio.AddComponent<FirstPersonHandheldShortwave>();
                    //        FPHS.m_NeedleTransform = MyMod.ViewModelRadioNeedle.transform;
                    //        GameObject obj = new GameObject("LIGHT");
                    //        obj.transform.SetParent(MyMod.ViewModelRadio.transform);
                    //        Light L = obj.AddComponent<Light>();
                    //        L.color = Color.green;
                    //        FPHS.m_Light = L;
                    //        FPHS.m_FarRangeBlinkRate = 5;
                    //        FPHS.m_MidRangeBlinkRate = 2;
                    //        FPHS.m_FarRangeBlinkRate = 1;
                    //        FPHS.m_MinAngle = -25f;
                    //        FPHS.m_MaxAngle = 25f;
                    //        FPHS.Awake();
                    //    }else{
                    //        GameObject reference = MyMod.GetGearItemObject("CONTAINER_ForestryCrate");
                    //        GameObject Stash = UnityEngine.Object.Instantiate<GameObject>(reference, GameManager.GetPlayerTransform().position, GameManager.GetPlayerTransform().rotation);
                    //        Stash.AddComponent<TrackableCache>();
                    //        GameObject reference2 = MyMod.GetGearItemObject("GEAR_RawCohoSalmon");
                    //        Stash.GetComponent<Container>().InstantiateGearPrefabInContainer(reference2);
                    //    }
                    //}
                    //MyMod.PoiskMujikov();
                    //if (MyMod.iAmHost)
                    //{
                    //    MyMod.LoadCustomChallenge(2, 1);
                    //    MyMod.StartCustomChallenge();
                    //    MyMod.CreateCairnsSearchList();
                    //    ServerSend.CAIRNS();
                    //}
                    //GameManager.GetPlayerAnimationComponent().Trigger_Breakdown_Intro(StruggleBonus.StruggleWeaponType.Hammer);
                    //string quote = "\"";
                    //string Copy = "AddSafeScene("+ quote + MyMod.level_guid + quote+ ");";
                    //GUIUtility.systemCopyBuffer = Copy;

                    //MelonLogger.Msg("Save slot current name " + SaveGameSystem.GetCurrentSaveName());
                    //MelonLogger.Msg("Save slot userdefined name " + SaveGameSlots.GetUserDefinedSlotName(SaveGameSystem.GetCurrentSaveName()));

                    //if (string.IsNullOrEmpty(ContainerOverride))
                    //{
                    //    ContainerOverride = "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";
                    //} else
                    //{
                    //    ContainerOverride = "";
                    //}
                    //ExpeditionManager.StartNewExpedition(MPSaveManager.GetSubNetworkGUID(), (int)GameManager.GetUniStorm().m_CurrentRegion);
                    MyMod.LogScreenshotData();
                }
                if (GUI.Button(new Rect(160, 160, 80, 20), "CrashSite"))
                {
                    if (MyMod.iAmHost)
                    {
                        ExpeditionManager.StartCrashSite();
                    }
                }
                if (GUI.Button(new Rect(160, 190, 80, 20), "Show"))
                {
                    SafeZoneManager.DebugRenderZones();
                }
            }

            if (MyMod.UIDebugType == "PlayerDebug")
            {

                GUI.Box(new Rect(150, 10, 100, 200), "Player Debug");

                if (GUI.Button(new Rect(160, 40, 80, 20), "Items"))
                {
                    MyMod.UIDebugType = "PlayerDebug_Items";
                }
                if (GUI.Button(new Rect(160, 70, 80, 20), "Anims"))
                {
                    MyMod.UIDebugType = "PlayerDebug_Anims";
                }
                if (GUI.Button(new Rect(160, 100, 80, 20), "Set"))
                {
                    MyMod.playersData[0].m_Levelid = MyMod.levelid;
                    MyMod.playersData[0].m_LevelGuid = MyMod.level_guid;
                    MyMod.playersData[0].m_Position = GameManager.GetPlayerTransform().position;
                    MyMod.playersData[0].m_Rotation = GameManager.GetPlayerTransform().rotation;
                }
                if (GUI.Button(new Rect(160, 130, 80, 20), "Mimic"))
                {
                    if (MyMod.playersData[0].m_Mimic == false)
                    {
                        MyMod.playersData[0].m_Mimic = true;
                    }else{
                        MyMod.playersData[0].m_Mimic = false;
                    }

                }
                if (GUI.Button(new Rect(160, 160, 80, 20), "Clothes"))
                {
                    MyMod.UIDebugType = "Clothes";
                    MyMod.playersData[0].m_PlayerClothingData.m_Boots = "GEAR_CombatBoots";
                }
                if (GUI.Button(new Rect(160, 190, 80, 20), "Fake"))
                {
                    if(MyMod.iAmHost == true)
                    {
                        MyMod.playersData[1].m_Levelid = MyMod.levelid;
                        MyMod.playersData[1].m_Position = GameManager.GetPlayerTransform().position;
                        MyMod.playersData[1].m_Rotation = GameManager.GetPlayerTransform().rotation;
                        MyMod.playersData[1].m_Position = GameManager.GetPlayerTransform().position;
                        MyMod.playersData[1].m_Rotation = GameManager.GetPlayerTransform().rotation;
                        Server.clients[1].udp.sid = "11111111111111111";
                        Server.clients[1].TimeOutTime = 0;
                    }
                }
                if (GUI.Button(new Rect(160, 220, 80, 20), "Back"))
                {
                    MyMod.UIDebugType = "Open";
                }
            }

            if (MyMod.UIDebugType == "PlayerDebug_Items")
            {
                GUI.Box(new Rect(150, 10, 100, 250), "Debug Items");
                if (MyMod.ItemsForDebug.Count == 0)
                {
                    MyMod.ItemsForDebug.Add("GEAR_WoodMatches");
                    MyMod.ItemsForDebug.Add("GEAR_FlareA");
                    MyMod.ItemsForDebug.Add("GEAR_BlueFlare");
                    MyMod.ItemsForDebug.Add("GEAR_Torch");
                    MyMod.ItemsForDebug.Add("GEAR_Rifle");
                    MyMod.ItemsForDebug.Add("GEAR_Revolver");
                    MyMod.ItemsForDebug.Add("GEAR_SprayPaintCanGlyphA");
                    MyMod.ItemsForDebug.Add("GEAR_Stone");
                    MyMod.ItemsForDebug.Add("GEAR_KeroseneLamp");
                    MyMod.ItemsForDebug.Add("GEAR_FlareGun");
                    MyMod.ItemsForDebug.Add("Book");
                    MyMod.ItemsForDebug.Add("");
                }
                if (GUI.Button(new Rect(160, 40, 80, 20), "Next Item"))
                {
                    MyMod.ItemForDebug = MyMod.ItemForDebug + 1;
                    if (MyMod.ItemForDebug > MyMod.ItemsForDebug.Count)
                    {
                        MyMod.ItemForDebug = 0;
                    }

                    MyMod.playersData[0].m_PlayerEquipmentData.m_HoldingItem = MyMod.ItemsForDebug[MyMod.ItemForDebug];
                }
                if (GUI.Button(new Rect(160, 70, 80, 20), "Light"))
                {
                    if (MyMod.playersData[0].m_PlayerEquipmentData.m_LightSourceOn == false)
                    {
                        MyMod.playersData[0].m_PlayerEquipmentData.m_LightSourceOn = true;
                    }
                    else
                    {
                        MyMod.playersData[0].m_PlayerEquipmentData.m_LightSourceOn = false;
                    }
                }
                if (GUI.Button(new Rect(160, 100, 80, 20), "Has Rifle"))
                {
                    if (MyMod.playersData[0].m_PlayerEquipmentData.m_HasRifle == false)
                    {
                        MyMod.playersData[0].m_PlayerEquipmentData.m_HasRifle = true;
                    }
                    else
                    {
                        MyMod.playersData[0].m_PlayerEquipmentData.m_HasRifle = false;
                    }
                }
                if (GUI.Button(new Rect(160, 130, 80, 20), "Has Rev."))
                {
                    if (MyMod.playersData[0].m_PlayerEquipmentData.m_HasRevolver == false)
                    {
                        MyMod.playersData[0].m_PlayerEquipmentData.m_HasRevolver = true;
                    }
                    else
                    {
                        MyMod.playersData[0].m_PlayerEquipmentData.m_HasRevolver = false;
                    }
                }
                if (GUI.Button(new Rect(160, 160, 80, 20), "Has Axe"))
                {
                    if (MyMod.playersData[0].m_PlayerEquipmentData.m_HasAxe == false)
                    {
                        MyMod.playersData[0].m_PlayerEquipmentData.m_HasAxe = true;
                    }
                    else
                    {
                        MyMod.playersData[0].m_PlayerEquipmentData.m_HasAxe = false;
                    }
                }
                if (GUI.Button(new Rect(160, 190, 80, 20), "Has Medkit"))
                {
                    if (MyMod.playersData[0].m_PlayerEquipmentData.m_HasMedkit == false)
                    {
                        MyMod.playersData[0].m_PlayerEquipmentData.m_HasMedkit = true;
                    }
                    else
                    {
                        MyMod.playersData[0].m_PlayerEquipmentData.m_HasMedkit = false;
                    }
                }
                if (GUI.Button(new Rect(160, 220, 80, 20), "Back"))
                {
                    MyMod.UIDebugType = "PlayerDebug";
                }
            }
            if (MyMod.UIDebugType == "PlayerDebug_Anims")
            {
                GUI.Box(new Rect(150, 10, 100, 230), "Debug Anims");
                if (GUI.Button(new Rect(160, 40, 80, 20), "Idle"))
                {
                    MyMod.playersData[0].m_AnimState = "Idle";
                }
                if (GUI.Button(new Rect(160, 70, 80, 20), "Walk"))
                {
                    MyMod.playersData[0].m_AnimState = "Walk";
                }
                if (GUI.Button(new Rect(160, 100, 80, 20), "Run"))
                {
                    MyMod.playersData[0].m_AnimState = "Run";
                }
                if (GUI.Button(new Rect(160, 130, 80, 20), "Sit"))
                {
                    if (MyMod.playersData[0].m_AnimState != "Ctrl")
                    {
                        MyMod.playersData[0].m_AnimState = "Ctrl";
                    }
                    else
                    {
                        MyMod.playersData[0].m_AnimState = "Idle";
                    }
                }
                if (GUI.Button(new Rect(160, 160, 80, 20), "Flex"))
                {
                    MyMod.playersData[0].m_AnimState = "Cringe2";
                }
                if (GUI.Button(new Rect(160, 190, 80, 20), "Aim"))
                {
                    if (MyMod.playersData[0].m_PlayerEquipmentData.m_HoldingItem == "GEAR_Revolver")
                    {
                        if (MyMod.playersData[0].m_AnimState != "Ctrl")
                        {
                            MyMod.playersData[0].m_AnimState = "HoldGun";
                        }
                        else
                        {
                            MyMod.playersData[0].m_AnimState = "HoldGun_Sit";
                        }
                    }
                }

                if (GUI.Button(new Rect(160, 220, 80, 20), "Back"))
                {
                    MyMod.UIDebugType = "PlayerDebug";
                }
            }
            if (MyMod.UIDebugType == "AnimalsDebug")
            {
                GUI.Box(new Rect(150, 10, 100, 270), "Debug Animals");
                if (GUI.Button(new Rect(160, 40, 80, 20), ThreadManager.executeOnMainThread.Count + "-" + ThreadManager.executeCopiedOnMainThread.Count))
                {

                }
                if (GUI.Button(new Rect(160, 70, 80, 20), "GUID Stats"))
                {
                    if (MyMod.AdvancedDebugMode == "AnimalsGUID")
                    {
                        MyMod.AdvancedDebugMode = "";
                    }
                    else
                    {
                        MyMod.AdvancedDebugMode = "AnimalsGUID";
                    }
                }
                if (GUI.Button(new Rect(160, 100, 80, 20), "All Stats"))
                {
                    if (MyMod.AdvancedDebugMode == "AnimalsAllStats")
                    {
                        MyMod.AdvancedDebugMode = "";
                    }
                    else
                    {
                        MyMod.AdvancedDebugMode = "AnimalsAllStats";
                    }
                }
                if (GUI.Button(new Rect(160, 130, 80, 20), "TP to GUID"))
                {
                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "INPUT GUID TO TELEPORT TO", "", Panel_Confirmation.ButtonLayout.Button_2, "TELEPORT", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                }
                if (GUI.Button(new Rect(160, 160, 80, 20), "Track GUID"))
                {
                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "INPUT GUID TO TRACK", "", Panel_Confirmation.ButtonLayout.Button_2, "TELEPORT", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                }
                if (GUI.Button(new Rect(160, 220, 80, 20), "Remove all"))
                {
                    for (int index = 0; index < BaseAiManager.m_BaseAis.Count; ++index)
                    {
                        GameObject animal = BaseAiManager.m_BaseAis[index].gameObject;
                        if (animal != null)
                        {
                            ObjectGuidManager.UnRegisterGuid(animal.GetComponent<ObjectGuid>().Get());
                            UnityEngine.Object.Destroy(animal);
                        }
                    }
                }
                if (GUI.Button(new Rect(160, 250, 80, 20), "Back"))
                {
                    MyMod.UIDebugType = "Open";
                }
            }
            if(MyMod.UIDebugType == "Clothes")
            {
                GUI.Box(new Rect(150, 10, 100, 270), "Debug Clotches");
                if (GUI.Button(new Rect(160, 40, 80, 20), "Next Top"))
                {
                    int topsCount = MyMod.players[0].transform.GetChild(0).GetChild(1).childCount;
                    Comps.MultiplayerPlayerClothingManager mPMC = MyMod.players[0].GetComponent<Comps.MultiplayerPlayerClothingManager>();
                    mPMC.m_Debug = true;
                    mPMC.m_DebugT = mPMC.m_DebugT + 1;
                    if (mPMC.m_DebugT >= topsCount)
                    {
                        mPMC.m_DebugT = 0;
                    }
                    mPMC.UpdateClothing();
                }
                if (GUI.Button(new Rect(160, 70, 80, 20), "Next Bottom"))
                {
                    int pantsCount = MyMod.players[0].transform.GetChild(0).GetChild(2).childCount;
                    Comps.MultiplayerPlayerClothingManager mPMC = MyMod.players[0].GetComponent<Comps.MultiplayerPlayerClothingManager>();
                    mPMC.m_Debug = true;
                    mPMC.m_DebugB = mPMC.m_DebugB + 1;
                    if (mPMC.m_DebugB >= pantsCount)
                    {
                        mPMC.m_DebugB = 0;
                    }
                    mPMC.UpdateClothing();
                }
                if (GUI.Button(new Rect(160, 100, 80, 20), "Back"))
                {
                    MyMod.UIDebugType = "PlayerDebug";
                }
            }
        }
    }
}
