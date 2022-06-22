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
using SkyCoop;

using GameServer;

namespace SkyCoop
{
    public class DeBugMenu
    {
        public static int DebugVal = 0;
        public static uint DebugSound = 0U;

        public static DecalProjectorInstance Replica;

        public static void Render()
        {
            if (MyMod.DebugGUI == false)
            {
                return;
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
                        MyMod.AnimalUpdates _AU = MyMod.DebugLastAnimal.GetComponent<MyMod.AnimalUpdates>();
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
                        MyMod.SendTCPData(_packet);
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
                MyMod.SkipRTTime(1);
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
                if (GUI.Button(new Rect(160, 160, 120, 20), "EverySecond="+MyMod.KillEverySecond))
                {
                    MyMod.KillEverySecond = !MyMod.KillEverySecond;
                }
                if (GUI.Button(new Rect(160, 190, 120, 20), "Close"))
                {
                    MyMod.UIDebugType = "";
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
                if (GUI.Button(new Rect(160, 130, 80, 20), "Test"))
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
                    //MyMod.CheckOtherPlayer(MyMod.BuildMyAfflictionList(), 0, GameManager.GetConditionComponent().m_CurrentHP);
                    //MyMod.MakeFakeFire(FireManager.m_Fires[0]);
                    //InterfaceManager.m_Panel_ActionsRadial.ShowToolsRadial();

                    MyMod.LoadCustomChallenge(1, 0);
                    MyMod.StartCustomChallenge();
                    //MyMod.AddSpray(Replica);

                    //MyMod.PoiskMujikov();
                }
                if (GUI.Button(new Rect(160, 160, 80, 20), "Close"))
                {
                    MyMod.UIDebugType = "";
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
                if (GUI.Button(new Rect(160, 160, 80, 20), "Knock"))
                {
                    MyMod.playersData[0].m_AnimState = "Knock";
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
                if (GUI.Button(new Rect(160, 40, 80, 20), "UNUSED"))
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
                if (GUI.Button(new Rect(160, 190, 80, 20), "Glow"))
                {
                    if (MyMod.DebugLastAnimal != null)
                    {
                        MyMod.Outline OL = MyMod.DebugLastAnimal.GetComponent<MyMod.Outline>();

                        if (OL == null)
                        {
                            MyMod.DebugLastAnimal.AddComponent<MyMod.Outline>();
                            OL = MyMod.DebugLastAnimal.GetComponent<MyMod.Outline>();
                            OL.m_OutlineColor = Color.green;
                            OL.needsUpdate = true;
                            MelonLogger.Msg("Added outlines");
                        }
                        else
                        {
                            if (OL.m_OutlineColor == Color.green)
                            {
                                OL.m_OutlineColor = Color.clear;
                                OL.needsUpdate = true;
                            }
                            else
                            {
                                OL.m_OutlineColor = Color.green;
                                OL.needsUpdate = true;
                            }
                        }
                    }
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
                    MyMod.MultiplayerPlayerClothingManager mPMC = MyMod.players[0].GetComponent<MyMod.MultiplayerPlayerClothingManager>();
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
                    MyMod.MultiplayerPlayerClothingManager mPMC = MyMod.players[0].GetComponent<MyMod.MultiplayerPlayerClothingManager>();
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
