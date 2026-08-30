using Il2Cpp;
using Il2CppRewired;
using Il2CppTLD.Gameplay;
using Il2CppTLD.Gear;
using Il2CppTLD.ModularElectrolizer;
using Il2CppTLD.PDID;
using Il2CppTLD.Scenes;
using Il2CppTLD.UI;
using SkyCoop;
using SkyCoopServer;
using UnityEngine;
using static SkyCoop.Comps;

namespace SkyCoopClient
{
    public class Minimalizer
    {
        public static string s_SceneSpawnOverride = "";
        public static bool s_LoadingFlag = false;
        public static int s_FramesDelayBeforeSendNewScene = 0;

        public static void OnStartedLoading()
        {
            s_LoadingFlag = true;
            SkyCoop.Logger.Log(ConsoleColor.DarkMagenta, "Start loading scenes...");
            if (ModMain.IsMultiplayer())
            {
                ClientSend.SendNewScene("Empty");
            }
        }


        [HarmonyLib.HarmonyPatch(typeof(Panel_Loading), "Update")]
        private static class Panel_Loading_Update
        {
            private static void Postfix(Panel_Loading __instance)
            {
                bool IsLoading = !__instance.HasFinishedLoading();

                if (IsLoading && !s_LoadingFlag)
                {
                    OnStartedLoading();
                }
                else if (s_LoadingFlag && __instance.HasFinishedLoading() && __instance.HasFinishedHolding())
                {
                    s_LoadingFlag = false;
                    OnFinishedLoading();
                }
            }
        }

        public static void OnFinishedLoading()
        {
            SkyCoop.Logger.Log(ConsoleColor.DarkMagenta, "Scenes loaded");
            GearSpawnsRipper.SceneLoaded();

            if (ModMain.IsMultiplayer())
            {
                for (int i = GearManager.m_Gear.Count - 1; i >= 0; i--)
                {
                    GearItem item = GearManager.m_Gear[i];
                    if (!item.m_HasBeenOwnedByPlayer && !item.m_BeenInPlayerInventory)
                    {
                        GearManager.DestroyGearObject(item);
                    }
                }

                foreach (Camera Cam in Camera.allCameras)
                {
                    if (Cam.name != "FPSCamera")
                    {
                        AudioListener AudioListner = Cam.gameObject.GetComponent<AudioListener>();
                        if (AudioListner)
                        {
                            string Log = AudioListner.gameObject.name;

                            Transform Parent = AudioListner.gameObject.transform.parent;
                            while (Parent != null)
                            {
                                Log = $"{Parent.name}/{Log}";
                                Parent = Parent.parent;
                            }
                            SkyCoop.Logger.Log(ConsoleColor.Green, $"Found random ass AudioListener imposter: Scene Name {ModMain.GetCurrentSceneName()} location {Log}");

                            UnityEngine.Object.Destroy(AudioListner);
                        }
                    }
                }
                s_FramesDelayBeforeSendNewScene = 2;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "SpawningSupppressedByExperienceMode")]
        private static class SpawnRegion_SpawningSupppressedByExperienceMode
        {
            private static void Postfix(SpawnRegion __instance, bool __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                __result = true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "Spawn")]
        private static class SpawnRegion_Spawn
        {
            private static bool Prefix(SpawnRegion __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PrefabSpawn), "SpawnObject")]
        private static class PrefabSpawn_SpawnObject
        {
            private static bool Prefix(PrefabSpawn __instance)
            {
                if (GearSpawnsRipper.s_Active){ return false; }
                
                if (!ModMain.IsMultiplayer()) { return true; }

                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(RandomSpawnObject), "ActivateRandomObject")]
        private static class RandomSpawnObjectActivateRandomObject
        {
            private static bool Prefix(RandomSpawnObject __instance)
            {
                if (GearSpawnsRipper.s_Active) { return false; }

                if (!ModMain.IsMultiplayer()) { return true; }
                string GUID = "";
                ObjectGuid ObjGUID = __instance.gameObject.GetComponent<ObjectGuid>();
                if (ObjGUID)
                {
                    GUID = ObjGUID.Get();
                }
                List<GameObject> list = new List<GameObject>(__instance.m_ObjectList);
                List<int> list2 = new List<int>(__instance.m_Weights);
                UnityEngine.Random.State state = UnityEngine.Random.state;
                int GUIDBasedSeed = ModMain.GetDeterministicSeed(GUID);
                int Seed = GameManager.GetRandomSeed(GUIDBasedSeed);
                UnityEngine.Random.InitState(Seed);

                //string SpawnerName = __instance.gameObject.name;

                //if (__instance.gameObject.transform.parent)
                //{
                //    SpawnerName = __instance.gameObject.transform.parent.gameObject.name;
                //}

                //SkyCoop.Logger.Log(ConsoleColor.Green, $"[RandomSpawnObject] {SpawnerName} GUIDBasedSeed {GUIDBasedSeed} Final seed {Seed}");
                int num = __instance.GetNumObjectsToEnableCurrentXPMode();
                if (num > list2.Count)
                {
                    num = list2.Count;
                }
                for (int i = 0; i < num; i++)
                {
                    int num2 = 0;
                    foreach (int num3 in list2)
                    {
                        num2 += num3;
                    }
                    if (num2 == 0)
                    {
                        UnityEngine.Random.state = state;
                        return false;
                    }
                    List<MapDetail> list3 = new List<MapDetail>();
                    foreach (GameObject gameObject in list)
                    {
                        if (gameObject)
                        {
                            MapDetail[] componentsInChildren = gameObject.GetComponentsInChildren<MapDetail>();
                            list3.AddRange(componentsInChildren);
                        }
                    }
                    int num4 = UnityEngine.Random.Range(0, num2);
                    int num5 = 0;
                    int num6 = -1;
                    for (int j = 0; j < list2.Count; j++)
                    {
                        int num7 = num5 + list2[j];
                        if (num4 >= num5 && num4 < num7)
                        {
                            if (list[j] && list[j].GetComponent<GearItem>() == null)
                            {
                                list[j].SetActive(true);
                                foreach (MapDetail item in list[j].GetComponentsInChildren<MapDetail>())
                                {
                                    list3.Remove(item);
                                }
                            }
                            num6 = j;
                            break;
                        }
                        num5 = num7;
                    }
                    if (num6 >= 0)
                    {
                        list2.RemoveAt(num6);
                        list.RemoveAt(num6);
                    }
                }


                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GearItem), "RollSpawnChance")]
        private static class GearItem_RollSpawnChance
        {
            private static bool Prefix(GearItem __instance)
            {
                if (GearSpawnsRipper.s_Active) { return false; }
                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(RadialObjectSpawner), "SpawnObjectAttempt")]
        public static class RadialObjectSpawner_SpawnObjectAttempt
        {
            public static bool s_ByPass = false;
            
            private static bool Prefix(RadialObjectSpawner __instance)
            {
                if (s_ByPass) { return true; }

                if (GearSpawnsRipper.s_Active) { return false; }

                if (!ModMain.IsMultiplayer()) { return true; }

                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(RadialSpawnManager), "DeserializeAll")]
        private static class RadialSpawnManager_DeserializeAll
        {
            private static void Prefix(RadialSpawnManager __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                RadialSpawnManager.m_RadialSpawnObjects.Clear();
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SpawnGearVariant), "Awake")]
        private static class SpawnGearVariant_
        {
            private static bool Prefix(SpawnGearVariant __instance)
            {
                if (GearSpawnsRipper.s_Active) { return false; }

                if (!ModMain.IsMultiplayer()) { return false; }

                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GameManager), "LoadSceneWithLoadingScreen", new System.Type[] { typeof(string) })]
        private static class GameManager_LoadSceneWithLoadingScreen
        {
            private static bool Prefix(GameManager __instance, string sceneName)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                SkyCoop.Logger.Log("LoadSceneWithLoadingScreen");
                if (string.IsNullOrEmpty(s_SceneSpawnOverride))
                {
                    return true;
                }
                SkyCoop.Logger.Log("s_SceneSpawnOverride " + s_SceneSpawnOverride);
                InterfaceManager.CloseOverlaysDueToSceneLoad();
                SaveGameSystem.ResetForSceneLoad();
                if (GameManager.IsMainMenuActive() || GameManager.IsActiveScene("Empty"))
                {
                    GameManager.LoadSceneAsynchronously(s_SceneSpawnOverride);
                    s_SceneSpawnOverride = "";
                    GameManager.SetPhysicsAutoSimulationEnabled(false);
                    return false;
                }
                EmptyScene.s_SceneLoadFromEmpty = s_SceneSpawnOverride;
                s_SceneSpawnOverride = "";
                GameManager.ResetLists();
                SceneManager.LoadScene("Empty", 0);
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(MiniTopNav), "Update")]
        private static class MiniTopNav_Update
        {
            private static void Postfix(MiniTopNav __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                bool Clothing = ModMain.Client != null && ModMain.Client.m_Rules.m_Clothing;
                bool CanStartFire = ModMain.Client != null && ModMain.Client.m_Rules.m_CanStartFire;
                bool CanCraft = ModMain.Client != null && ModMain.Client.m_IsReady && ModMain.Client.m_Rules.m_CanCraft;
                bool CanUseMap = ModMain.Client != null && ModMain.Client.m_IsReady && ModMain.Client.m_Rules.m_CanUseMap;

                for (int i = __instance.m_ActiveElements.Count - 1; i >= 0; i--)
                {
                    MiniTopNavButton butt = __instance.m_ActiveElements[i];
                    if (butt.name == "SpriteClothing" && Clothing)
                    {
                        continue;
                    }
                    if (butt.name == "SpriteRecipeBook" && CanStartFire)
                    {
                        continue;
                    }
                    if (butt.name == "SpriteCrafting" && CanCraft)
                    {
                        continue;
                    }
                    if (butt.name == "SpriteMap" && CanUseMap)
                    {
                        continue;
                    }
                    if (butt && (butt.name != "SpriteFirstAid" && butt.name != "SpriteInventory" && butt.name != "SpriteJournal"))
                    {
                        __instance.m_ActiveElements.RemoveAt(i);
                    }
                }
                for (int i = __instance.m_NavElements.Count - 1; i >= 0; i--)
                {
                    MiniTopNavButton butt = __instance.m_NavElements[i];
                    if (butt.name == "SpriteClothing" && Clothing)
                    {
                        continue;
                    }
                    if (butt.name == "SpriteRecipeBook" && CanStartFire)
                    {
                        continue;
                    }
                    if (butt.name == "SpriteCrafting" && CanCraft)
                    {
                        continue;
                    }
                    if (butt.name == "SpriteMap" && CanUseMap)
                    {
                        continue;
                    }
                    if (butt && (butt.name != "SpriteFirstAid" && butt.name != "SpriteInventory" && butt.name != "SpriteJournal"))
                    {
                        butt.SetEnabled(false);
                        butt.gameObject.SetActive(false);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Clothing), "Enable")]
        private static class Panel_Clothing_Enable
        {
            private static bool Prefix(Panel_Clothing __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return ModMain.Client != null && ModMain.Client.m_Rules.m_Clothing;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Map), "ToggleWorldMap")]
        private static class Panel_Map_ToggleWorldMap
        {
            private static void Postfix(Panel_Map __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                foreach (GameObject Obj in GearsSync.s_SpawnersMarkersObjects)
                {
                    if (Obj)
                    {
                        UISprite Sprite = Obj.GetComponent<UISprite>();
                        if (Sprite)
                        {
                            Sprite.enabled = !__instance.IsWorldMapActive();
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Map), "ResetToNormal")]
        private static class Panel_Map_ResetToNormal
        {
            private static void Postfix(Panel_Map __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                foreach (GameObject Obj in GearsSync.s_SpawnersMarkersObjects)
                {
                    if (Obj)
                    {
                        UISprite Sprite = Obj.GetComponent<UISprite>();
                        if (Sprite)
                        {
                            Sprite.enabled = __instance.m_RegionSelectedIndex == __instance.GetIndexOfCurrentScene();
                        }
                    }
                }
                foreach (GameObject Obj in SpawnPointEditor.m_MapMarkers)
                {
                    if (Obj)
                    {
                        UISprite Sprite = Obj.GetComponent<UISprite>();
                        if (Sprite)
                        {
                            Sprite.enabled = __instance.m_RegionSelectedIndex == __instance.GetIndexOfCurrentScene();
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Map), "Enable", new System.Type[] { typeof(bool) })]
        private static class Panel_Map_Enable
        {
            private static bool Prefix(Panel_Map __instance, bool enable)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (enable)
                {
                    bool CanUseMap = ModMain.s_MapEditor || ModMain.Client.m_Rules.m_CanUseMap;

                    if (CanUseMap)
                    {
                        __instance.UnlockMapCurrentScene();
                        __instance.RevealFogForScene(__instance.GetMapNameOfCurrentScene());
                        Panel_Map.s_ForceShowPlayerIcon = !PlayersManager.s_Spectator;
                    }
                    else
                    {
                        Panel_Map.s_ForceShowPlayerIcon = false;
                    }
                    return CanUseMap;
                }
                else
                {
                    return true;
                }
            }

            private static void Postfix(Panel_Map __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                bool CanUseMap = ModMain.s_MapEditor || ModMain.Client.m_Rules.m_CanUseMap;

                if (CanUseMap)
                {
                    if (__instance.m_MapElementsTransform.FindChild($"PlayerIcon_0") == null)
                    {
                        foreach (NetworkPlayer Player in PlayersManager.s_Players)
                        {
                            if (Player)
                            {
                                GameObject OtherPlayerArrow = GameObject.Instantiate(__instance.m_PlayerIcon.gameObject, __instance.m_PlayerIcon.parent);
                                if (OtherPlayerArrow)
                                {
                                    OtherPlayerArrow.name = $"PlayerIcon_{Player.m_PlayerID}";
                                    OtherPlayerArrow.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                                    OtherPlayerArrow.transform.SetSiblingIndex(0);
                                    Comps.TeammateMapIcon TMI = OtherPlayerArrow.AddComponent<Comps.TeammateMapIcon>();
                                    if (TMI)
                                    {
                                        TMI.m_IndexHandler = Player.m_PlayerID;
                                        TMI.m_Sprite = OtherPlayerArrow.GetComponent<UISprite>();
                                        if (TMI.m_Sprite)
                                        {
                                            TMI.m_Sprite.color = Color.green;
                                        }
                                        TMI.m_Panel = __instance;
                                    }
                                }
                            }
                        }
                        __instance.m_PlayerIcon.SetSiblingIndex(0);
                    }
                    if (__instance.m_MapElementsTransform.FindChild($"ZoneIcon") == null)
                    {
                        GameObject ZoneIconObject = GameObject.Instantiate(__instance.m_PlayerIcon.gameObject, __instance.m_PlayerIcon.parent);
                        if (ZoneIconObject)
                        {
                            ZoneIconObject.SetActive(true);
                            ZoneIconObject.name = "ZoneIcon";
                            ZoneIconObject.transform.SetSiblingIndex(0);
                            Comps.ZoneMapIcon ZMI = ZoneIconObject.AddComponent<Comps.ZoneMapIcon>();
                            if (ZMI)
                            {
                                ZMI.m_Sprite = ZoneIconObject.GetComponent<UISprite>();
                                ZMI.m_Sprite.atlas = GameModeHUD.s_BaseAtlas;
                                ZMI.m_Sprite.spriteName = "outerGlow_circle";
                                if (ZMI.m_Sprite)
                                {
                                    ZMI.m_Sprite.color = Color.blue;
                                }
                                ZMI.m_Panel = __instance;
                            }
                        }
                        GameObject NextZoneIconObject = GameObject.Instantiate(__instance.m_PlayerIcon.gameObject, __instance.m_PlayerIcon.parent);
                        if (NextZoneIconObject)
                        {
                            NextZoneIconObject.SetActive(true);
                            NextZoneIconObject.name = "NextZoneIcon";
                            NextZoneIconObject.transform.SetSiblingIndex(0);
                            Comps.ZoneMapIcon ZMI = NextZoneIconObject.AddComponent<Comps.ZoneMapIcon>();
                            if (ZMI)
                            {
                                ZMI.m_Sprite = NextZoneIconObject.GetComponent<UISprite>();
                                ZMI.m_Sprite.atlas = GameModeHUD.s_BaseAtlas;
                                ZMI.m_Sprite.spriteName = "outerGlow_circle";
                                ZMI.m_IsNextZone = true;
                                if (ZMI.m_Sprite)
                                {
                                    ZMI.m_Sprite.color = new Color(1, 1, 1, 0.6f);
                                }
                                ZMI.m_Panel = __instance;
                            }
                        }
                    }
                    if (__instance.m_MapElementsTransform.FindChild($"GearSpawnMarker") == null)
                    {
                        foreach (Vector3 Position in GearsSync.s_SpawnersMarkers)
                        {
                            GameObject GearSpawner = GameObject.Instantiate(__instance.m_PlayerIcon.gameObject, __instance.m_PlayerIcon.parent);
                            if (GearSpawner)
                            {
                                GearSpawner.transform.rotation = Quaternion.identity;
                                GearSpawner.SetActive(true);
                                GearSpawner.name = "GearSpawnMarker";
                                GearSpawner.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                                GearSpawner.transform.localPosition = __instance.WorldPositionToMapPosition(__instance.m_UnlockedRegionNames[__instance.m_RegionSelectedIndex], Position);

                                UISprite Sprite = GearSpawner.GetComponent<UISprite>();
                                Sprite.atlas = GameModeHUD.s_BaseAtlas;
                                Sprite.spriteName = "icoMap_Generic";
                                Sprite.color = Color.magenta;
                                GearsSync.s_SpawnersMarkersObjects.Add(GearSpawner);
                            }
                        }
                    }

                    if (ModMain.s_MapEditor)
                    {
                        for (int i = SpawnPointEditor.m_MapMarkers.Count - 1; i >= 0; i--)
                        {
                            UnityEngine.Object.Destroy(SpawnPointEditor.m_MapMarkers[i]);
                        }
                        SpawnPointEditor.m_MapMarkers.Clear();
                        foreach (DataStr.V3Quat V3Q in SpawnPointEditor.m_Points)
                        {
                            GameObject PlayerSpawner = GameObject.Instantiate(__instance.m_PlayerIcon.gameObject, __instance.m_PlayerIcon.parent);
                            if (PlayerSpawner)
                            {
                                PlayerSpawner.transform.rotation = Quaternion.identity;
                                PlayerSpawner.SetActive(true);
                                PlayerSpawner.name = "PlayerSpawner";
                                PlayerSpawner.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                                PlayerSpawner.transform.localPosition = __instance.WorldPositionToMapPosition(__instance.m_UnlockedRegionNames[__instance.m_RegionSelectedIndex], V3Q.m_Position.ConvertToUnity());

                                UISprite Sprite = PlayerSpawner.GetComponent<UISprite>();
                                Sprite.atlas = GameModeHUD.s_BaseAtlas;
                                Sprite.spriteName = "ico_knowledge_people";
                                Sprite.color = Color.green;
                                SpawnPointEditor.m_MapMarkers.Add(PlayerSpawner);
                            }
                        }
                    }

                    string mapNameOfCurrentScene = __instance.GetMapNameOfCurrentScene();
                    if (!string.IsNullOrEmpty(mapNameOfCurrentScene))
                    {
                        if (__instance.m_MapElementData.ContainsKey(mapNameOfCurrentScene))
                        {
                            Il2CppSystem.Collections.Generic.List<MapElementSaveData> list = __instance.m_MapElementData[mapNameOfCurrentScene];
                            for (int i = 0; i < list.Count; i++)
                            {
                                MapElementSaveData Element = list[i];

                                if (Element.m_IsArea || Element.m_BigSprite)
                                {
                                    Element.m_NameIsKnown = true;
                                }
                            }
                            return;
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Map), "ShouldCenterOnPlayer")]
        private static class Panel_Map_ShouldCenterOnPlayer
        {
            private static void Postfix(Panel_Map __instance, ref bool __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }


                bool CanUseMap = ModMain.Client != null && ModMain.Client.m_Rules.m_CanUseMap;

                __result = CanUseMap;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Il2Cpp.InputManager), "ExecuteOpenMapAction")]
        private static class InputManager_ExecuteOpenMapAction
        {
            private static bool Prefix()
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (PlayersManager.s_Spectator)
                {
                    InterfaceManager.TrySetPanelEnabled<Panel_Map>(!InterfaceManager.IsPanelEnabled<Panel_Map>());
                    return false;
                }
                return true;
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(Panel_Log), "Enable")]
        //private static class Panel_Log_Enable
        //{
        //    private static void Postfix(Panel_Log __instance)
        //    {
        //        if (!ModMain.IsMultiplayer()) { return; }
        //        __instance.EnterState(PanelLogState.WhatIKnow);

        //        if (__instance.m_SectionNav)
        //        {
        //            __instance.m_SectionNav.SetActive(false);
        //        }
        //        if (__instance.m_LogSectionObject)
        //        {
        //            __instance.m_LogSectionObject.SetActive(false);
        //        }
        //        if (__instance.m_WhatIKnowSectionObject)
        //        {
        //            __instance.m_WhatIKnowSectionObject.SetActive(true);
        //        }
        //        if (__instance.m_SelectScreenOnly)
        //        {
        //            Transform T = __instance.m_SelectScreenOnly.transform.GetChild(0);
        //            if (T)
        //            {
        //                UILocalize Loca = T.GetComponent<UILocalize>();
        //                Loca.key = "GAMEPLAY_PEOPLE";
        //                Loca.OnLocalize();
        //            }
        //        }
        //        //if (__instance.m_WhatIKnowScrollList)
        //        //{
        //        //    GameObject CloneVictim = __instance.m_WhatIKnowScrollList.m_ScrollObjects[0];
        //        //    for (int i = __instance.m_WhatIKnowScrollList.transform.childCount-1; i > 0 ; i--)
        //        //    {
        //        //        GameObject Obj = __instance.m_WhatIKnowScrollList.transform.GetChild(i).gameObject;
        //        //        UnityEngine.Object.Destroy(Obj);
        //        //    }
        //        //    __instance.m_WhatIKnowScrollList.m_ScrollObjects.Clear();

        //        //    // Scroll test
        //        //    for (int i = 0; i < 12; i++)
        //        //    {
        //        //        GameObject NewElement = GameObject.Instantiate(CloneVictim, __instance.m_WhatIKnowScrollList.transform);
        //        //        __instance.m_WhatIKnowScrollList.m_ScrollObjects.Add(NewElement);
        //        //    }
        //        //    __instance.m_WhatIKnowScrollList.RefreshPositioning();
        //        //    __instance.m_WhatIKnowScrollList.RefreshVisibility();
        //        //    //foreach (NetworkPlayer Player in PlayersManager.s_Players)
        //        //    //{
        //        //    //    if (Player)
        //        //    //    {
        //        //    //        GameObject NewElement = GameObject.Instantiate(CloneVictim, __instance.m_WhatIKnowScrollList.transform);
        //        //    //        __instance.m_WhatIKnowScrollList.m_ScrollObjects.Add(NewElement);
        //        //    //    }
        //        //    //}
        //        //    UnityEngine.Object.Destroy(CloneVictim);
        //        //}
        //    }
        //}

        [HarmonyLib.HarmonyPatch(typeof(Panel_ActionsRadial), "Enable", new System.Type[] { typeof(bool) })]
        private static class Panel_ActionsRadial_Update
        {
            private static Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray s_VanilaNavigation = null;
            private static Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray s_VanilaPlace = null;

            private static void Postfix(Panel_ActionsRadial __instance)
            {
                if(s_VanilaNavigation == null)
                {
                    s_VanilaNavigation = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray(__instance.m_NavigationRadialOrder);
                }

                if (s_VanilaPlace == null)
                {
                    s_VanilaPlace = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray(__instance.m_PlaceItemRadialOrder);
                }

                if (!ModMain.IsMultiplayer())
                {
                    __instance.m_NavigationRadialOrder = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray(s_VanilaNavigation);
                    __instance.m_PlaceItemRadialOrder = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray(s_VanilaPlace);
                }
                else
                {
                    List<string> Navigation = new List<string>() 
                    {
                        "GEAR_Charcoal",
                        "", // SprayPaint
                        "", // RockCache
                        "Map",
                        "", // GEAR_HandheldShortwave
                        "", // GEAR_Camera
                    };
                    
                    __instance.m_NavigationRadialOrder = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray(Navigation.ToArray());

                    string BedRoll = ModMain.Client != null && ModMain.Client.m_IsReady && ModMain.Client.m_Rules.m_CanUseBeds ? "GEAR_BedRoll" : "";
                    string Campfire = ModMain.Client != null && ModMain.Client.m_IsReady && ModMain.Client.m_Rules.m_CanStartFire ? "Fire" : "";
                    string CookingPot = ModMain.Client != null && ModMain.Client.m_IsReady && ModMain.Client.m_Rules.m_CanStartFire ? "GEAR_CookingPot" : "";
                    string RecycleCan = ModMain.Client != null && ModMain.Client.m_IsReady && ModMain.Client.m_Rules.m_CanStartFire ? "GEAR_RecycledCan" : "";

                    List<string> PlaceItems = new List<string>() 
                    {
                        BedRoll,
                        "", // SnowShelter
                        Campfire,
                        CookingPot,
                        "PassTime",
                        "", // IceFishingHole
                        "GEAR_Snare",
                        RecycleCan,
                    };

                    __instance.m_PlaceItemRadialOrder = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray(PlaceItems.ToArray());
                }

                for (int i = __instance.m_PrimaryRadial.Count - 1; i >= 0; i--)
                {
                    Panel_ActionsRadial.RadialInfo Info = __instance.m_PrimaryRadial[i];
                    if (Info.m_RadialElement == Panel_ActionsRadial.RadialType.Decoy)
                    {
                        if (Settings.m_Options.m_MeleeOverDecoy)
                        {
                            Info.m_RadialElement = Panel_ActionsRadial.RadialType.Tools;
                            Info.m_GreyOutSpriteName = "ico_Radial_tools";
                            Info.m_SpriteName = "ico_Radial_tools";
                            Info.m_SpriteNameHover = "ico_Radial_tools";
                        }
                    }
                    else if (Info.m_RadialElement == Panel_ActionsRadial.RadialType.Tools)
                    {
                        if (!Settings.m_Options.m_MeleeOverDecoy)
                        {
                            Info.m_RadialElement = Panel_ActionsRadial.RadialType.Decoy;
                            Info.m_GreyOutSpriteName = "ico_Radial_decoy2";
                            Info.m_SpriteName = "ico_Radial_decoy";
                            Info.m_SpriteNameHover = "ico_Radial_decoy";
                        }
                    }
                    else if (Info.m_RadialElement == Panel_ActionsRadial.RadialType.Navigation)
                    {
                        if (ModMain.IsMultiplayer() && (ModMain.Client != null && ModMain.Client.m_IsReady && !ModMain.Client.m_Rules.m_CanUseMap))
                        {
                            Info.m_RadialElement = Panel_ActionsRadial.RadialType.Clothing;
                            Info.m_GreyOutSpriteName = "ico_inv_clothing";
                            Info.m_SpriteName = "ico_inv_clothing";
                            Info.m_SpriteNameHover = "ico_inv_clothing";
                        }
                    }
                    else if (Info.m_RadialElement == Panel_ActionsRadial.RadialType.Clothing)
                    {
                        if (!ModMain.IsMultiplayer() || (ModMain.Client != null && ModMain.Client.m_IsReady && ModMain.Client.m_Rules.m_CanUseMap))
                        {
                            Info.m_RadialElement = Panel_ActionsRadial.RadialType.Navigation;
                            Info.m_GreyOutSpriteName = "ico_map2";
                            Info.m_SpriteName = "ico_map";
                            Info.m_SpriteNameHover = "ico_map";
                        }
                    }
                    else if (Info.m_RadialElement == Panel_ActionsRadial.RadialType.PlaceItem)
                    {
                        if (ModMain.IsMultiplayer() && (ModMain.Client != null && ModMain.Client.m_IsReady && (!ModMain.Client.m_Rules.m_CanStartFire && !ModMain.Client.m_Rules.m_CanUseBeds)))
                        {
                            Info.m_RadialElement = Panel_ActionsRadial.RadialType.Inventory;
                            Info.m_GreyOutSpriteName = "ico_Radial_pack";
                            Info.m_SpriteName = "ico_Radial_pack";
                            Info.m_SpriteNameHover = "ico_Radial_pack";
                        }
                    }
                    else if (Info.m_RadialElement == Panel_ActionsRadial.RadialType.Inventory)
                    {
                        if(!ModMain.IsMultiplayer() || (ModMain.Client != null && ModMain.Client.m_IsReady && (ModMain.Client.m_Rules.m_CanStartFire || ModMain.Client.m_Rules.m_CanUseBeds)))
                        {
                            Info.m_RadialElement = Panel_ActionsRadial.RadialType.PlaceItem;
                            Info.m_GreyOutSpriteName = "ico_Radial_campCraft2";
                            Info.m_SpriteName = "ico_Radial_campCraft";
                            Info.m_SpriteNameHover = "ico_Radial_campCraft";
                        }
                    }
                }
                __instance.m_ToolsRadialOrder = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray(MeleeManager.s_MeleeWeapons.ToArray());
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(Panel_ActionsRadial), "GetDelegateForRadial")]
        //private static class Panel_ActionsRadial_GetDelegateForRadial
        //{
        //    private static void Postfix(Panel_ActionsRadial __instance, Panel_ActionsRadial.RadialType radialType, Il2CppSystem.Action __result)
        //    {
        //        if(radialType == Panel_ActionsRadial.RadialType.Clothing)
        //        {
        //            __result = new System.Action(OpenClothing);
        //        }
        //    }
        //}

        private static Il2CppSystem.Collections.Generic.List<GearItem> GetClothingItemsInInventory()
        {
            Il2CppSystem.Collections.Generic.List<GearItem> Gears = new Il2CppSystem.Collections.Generic.List<GearItem>();
            for (int i = 0; i < GameManager.GetInventoryComponent().m_Items.Count; i++)
            {
                GearItem gearItem = GameManager.GetInventoryComponent().m_Items[i];
                if (gearItem && gearItem.m_ClothingItem && gearItem.m_NarrativeCollectibleItem == null)
                {
                    Gears.Add(gearItem);
                }
            }
            return Gears;
        }

        public static void ShowNoClothing()
        {
            HUDMessage.AddMessage(Localization.Get("GAMEPLAY_None"), false, false);
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_ActionsRadial), "StartClothingUI")]
        private static class Panel_ActionsRadial_StartClothingUI
        {
            private static void Postfix(Panel_ActionsRadial __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                __instance.m_Queue.Add(new Action(__instance.StartClothingUI));
                __instance.ShowGearRadial(GetClothingItemsInInventory(), new Action(ShowNoClothing));
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_FirstAid), "Enable")]
        private static class Panel_FirstAid_Start
        {
            private static void Postfix(Panel_FirstAid __instance)
            {
                bool ShouldShowStats = true;
                
                if (ModMain.IsMultiplayer())
                {
                    ShouldShowStats = ModMain.Client != null && ModMain.Client.m_IsReady && ModMain.Client.m_Rules.m_Cold;
                }

                __instance.transform.GetChild(2).gameObject.SetActive(ShouldShowStats);
                __instance.transform.GetChild(5).GetChild(11).gameObject.SetActive(ShouldShowStats);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(IceFishingHole), "Awake")]
        private static class IceFishingHole_Start
        {
            private static void Postfix(IceFishingHole __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                __instance.enabled = false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GenericStatusBarSpawner), "AssignValuesToSpawnedObject")]
        private static class GenericStatusBarSpawner_AssignValuesToSpawnedObject
        {
            private static void Postfix(GenericStatusBarSpawner __instance)
            {
                GenericStatusBarSpawnerHook Hook = __instance.gameObject.GetComponent<GenericStatusBarSpawnerHook>();
                if (Hook == null)
                {
                    Hook = __instance.gameObject.AddComponent<GenericStatusBarSpawnerHook>();
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Hunger), "Update")]
        private static class Hunger_Update
        {
            private static void Postfix(Hunger __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if(ModMain.Client != null && ModMain.Client.m_IsReady)
                {
                    if (!ModMain.Client.m_Rules.m_Hunger)
                    {
                        __instance.m_CurrentReserveCalories = __instance.m_MaxReserveCalories * 0.9f;
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Thirst), "Update")]
        private static class Thirst_Update
        {
            private static void Postfix(Thirst __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (ModMain.Client != null && ModMain.Client.m_IsReady)
                {
                    if (!ModMain.Client.m_Rules.m_Thirst)
                    {
                        __instance.m_CurrentThirst = 15;
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(EmergencyStim), "ApplyEmergencyStimExitEffects")]
        private static class EmergencyStim_ApplyEmergencyStimExitEffects
        {
            private static void Postfix(EmergencyStim __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                //GameManager.GetDiminishedState().Apply(1, AfflictionOptions.None);
                GameManager.GetSprainPainComponent().ApplyAffliction(AfflictionBodyArea.LegLeft, "GAMEPLAY_EmergencyStim");
                GameManager.GetSprainPainComponent().ApplyAffliction(AfflictionBodyArea.LegRight, "GAMEPLAY_EmergencyStim");

                HeadacheData Stock = GameManager.GetHeadacheComponent().m_LegacyHeadacheData;
                HeadacheData headacheData = new HeadacheData();
                headacheData.m_Cause = HeadacheCause.None;
                LocalizedString Case = new LocalizedString();
                Case.m_LocalizationID = "GAMEPLAY_EmergencyStim";
                headacheData.m_CausedByLocalizedId = Case;
                headacheData.m_TreatmentRequiredDescription = Stock.m_TreatmentRequiredDescription;
                headacheData.m_HoursRequiredOutdoorToGetAffliction = Stock.m_HoursRequiredOutdoorToGetAffliction;
                headacheData.m_HoursRequiredIndoorToExitAffliction = Stock.m_HoursRequiredIndoorToExitAffliction;
                headacheData.m_HealedAfflictionLocalizedId = Stock.m_HealedAfflictionLocalizedId;
                headacheData.m_HeadacheStartAudio = Stock.m_HeadacheStartAudio;
                headacheData.m_HeadachePulseFrequencyStart = Stock.m_HeadachePulseFrequencyStart;
                headacheData.m_HeadachePulseFrequencyEnd = Stock.m_HeadachePulseFrequencyEnd;
                headacheData.m_HeadachePulseEvent = Stock.m_HeadachePulseEvent;
                headacheData.m_HeadacheDurationHours = Stock.m_HeadacheDurationHours;
                headacheData.m_HeadacheDescription = Stock.m_HeadacheDescription;
                headacheData.m_HeadacheAfflictionIcoName = Stock.m_HeadacheAfflictionIcoName;
                headacheData.m_HeadacheLocalizedId = Stock.m_HeadacheLocalizedId;
                GameManager.GetHeadacheComponent().ApplyHeadache(headacheData);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Condition), "PlayerDeath")]
        private static class Condition_PlayerDeath
        {
            private static bool Prefix(Condition __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                DataStr.DamageType DamageType = DataStr.DamageType.Unknown;

                SkyCoop.Logger.Log("PlayerDeath Cause " + __instance.m_CauseOfDeath);

                DamageType = PlayersManager.m_LastDamageType;
                SkyCoop.Logger.Log("PlayerDeath DamageType " + DamageType);

                SleepHook.OnCancle();


                if (ModMain.Client != null && ModMain.Client.m_Rules.m_PlayerCanBeKnocked)
                {
                    if (GameManager.GetBrokenBody().HasAffliction)
                    {
                        PlayersManager.Death(DamageType, PlayersManager.m_LastDamageZone);
                        DeathPacksManager.CreateMyDeathPack();
                        return true;
                    }
                    PlayersManager.ToKnockedState(DamageType, PlayersManager.m_LastDamageZone);
                    //PlayersManager.m_LastDamageType = DataStr.DamageType.Unknown;

                    return false;
                }
                else
                {
                    PlayersManager.Death(DamageType, PlayersManager.m_LastDamageZone);
                    DeathPacksManager.CreateMyDeathPack();
                }
                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "UseFirstAidItem")]
        private static class PlayerManager_UseFirstAidItem
        {
            private static bool Prefix(PlayerManager __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (GameManager.GetBrokenBody().HasAffliction && ModMain.Client != null && ModMain.Client.m_Rules.m_PlayerCanBeKnocked)
                {
                    HUDMessage.AddMessage("You can't do this while knocked down", true, true);
                    GameAudioManager.PlayGUIError();
                    return false;
                }
                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "CanUseFoodInventoryItem")]
        private static class PlayerManager_CanUseFoodInventoryItem
        {
            private static bool Prefix(PlayerManager __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (GameManager.GetBrokenBody().HasAffliction && ModMain.Client != null && ModMain.Client.m_Rules.m_PlayerCanBeKnocked)
                {
                    HUDMessage.AddMessage("You can't do this while knocked down", true, true);
                    GameAudioManager.PlayGUIError();
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(LoadScene), "Awake")]
        private static class LoadScene_Awake
        {
            private static void Postfix(LoadScene __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if(ModMain.Client != null && ModMain.Client.m_IsReady && !ModMain.Client.m_Rules.m_CanUseTransitions)
                {
                    __instance.enabled = false;
                    __instance.m_Active = false;
                    Collider COL = __instance.GetComponent<Collider>();
                    if (COL)
                    {
                        COL.isTrigger = false;
                        COL.gameObject.layer = vp_Layer.TerrainObject;
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(LoadingZone), "Awake")]
        private static class LoadingZone_Awake
        {
            private static void Postfix(LoadingZone __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (ModMain.Client != null && ModMain.Client.m_IsReady && !ModMain.Client.m_Rules.m_CanUseTransitions)
                {
                    __instance.enabled = false;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Hypothermia), "HypothermiaStart")]
        private static class Hypothermia_HypothermiaStart
        {
            private static bool Prefix(Hypothermia __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (ModMain.Client != null && ModMain.Client.m_IsReady)
                {
                    if (!ModMain.Client.m_Rules.m_Cold)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Hypothermia), "Start")]
        private static class Hypothermia_Start
        {
            private static void Postfix(Hypothermia __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (ModMain.Client != null && ModMain.Client.m_IsReady)
                {
                    if (!ModMain.Client.m_Rules.m_Cold)
                    {
                        __instance.m_SuppressHypothermia = true;
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(StartGear), "AddAllToInventory")]
        private static class StartGear_AddAllToInventory
        {
            private static bool Prefix(StartGear __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }


                return PlayersManager.GiveoutStartingGear(PlayersManager.m_LocalPlayerData.m_Tier);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_LifeAfterDeath), "Enable")]
        private static class Panel_LifeAfterDeath_Enable
        {
            private static void Postfix(Panel_LifeAfterDeath __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                UILocalize RespawnButton = __instance.m_CheatDeathButtonWidget.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<UILocalize>();

                if (ModMain.Client != null && ModMain.Client.m_Rules.m_Respawns == 2)
                {
                    RespawnButton.key = "Respawn";
                    __instance.m_CampfireGrid.gameObject.SetActive(false);
                }
                else if(ModMain.Client != null && ModMain.Client.m_Rules.m_Respawns == 0)
                {
                    RespawnButton.key = "Spectate";
                    __instance.m_CampfireGrid.gameObject.SetActive(false);
                }
                else
                {
                    RespawnButton.key = "GAMEPLAY_CheatDeath";
                    __instance.m_CampfireGrid.gameObject.SetActive(true);
                }
                RespawnButton.OnLocalize();
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_LifeAfterDeath), "HandleOnLeftButtonPressed")]
        private static class Panel_LifeAfterDeath_HandleOnLeftButtonPressed
        {
            private static bool Prefix(Panel_LifeAfterDeath __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (__instance.m_CurrentStage == Panel_LifeAfterDeath.LifeAfterDeathStage.Revive)
                {
                    if (ModMain.Client != null && ModMain.Client.m_Rules.m_Respawns == 1)
                    {
                        return true;
                    }

                    GameManager.GetConditionComponent().ResetAudio();
                    ClientSend.SendRespawnRequest();
                    MenuHook.RemovePleaseWait();
                    MenuHook.DoPleaseWait("Взламываем твой камютэр, жди...", "Грузим шпингалеты...");
                    return false;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Condition), "PlayDeathMusic")]
        private static class Condition_PlayDeathMusic
        {
            private static bool Prefix(Condition __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_LifeAfterDeath), "HandleOnRightButtonPressed")]
        private static class Panel_LifeAfterDeath_HandleOnRightButtonPressed
        {
            private static bool Prefix(Panel_LifeAfterDeath __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (__instance.m_CurrentStage == Panel_LifeAfterDeath.LifeAfterDeathStage.Revive)
                {
                    MenuHook.OnDisconnectConfirmed();
                    return true;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SafehouseManager), "MaybeToggleCustomizing")]
        private static class SafehouseManager_Enable
        {
            private static bool Prefix(SafehouseManager __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SafehouseManager), "InCustomizableSafehouse")]
        private static class SafehouseManager_InCustomizableSafehouse
        {
            private static void Postfix(SafehouseManager __instance, ref bool __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                __result = false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "EatingComplete_Internal")]
        private static class PlayerManager_EatingComplete_Internal
        {
            private static void Postfix(PlayerManager __instance, bool success, bool playerCancel, float progress)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (ModMain.Client != null && ModMain.Client.m_IsReady)
                {
                    if (ModMain.Client.m_Rules.m_Hunger || ModMain.Client.m_Rules.m_Thirst)
                    {
                        return;
                    }
                }

                if (success && !playerCancel && __instance.m_FoodItemEaten)
                {
                    float Cal = __instance.m_FoodItemEaten.m_FoodItem.m_CaloriesTotal;

                    float Health = 60 * Cal / 1500f;

                    GameManager.GetConditionComponent().AddHealth(Health, DamageSource.FirstAid);

                    PlayerDamageEvent.SpawnAfflictionEvent($"+{Math.Round(Health).ToString()} {Localization.Get("GAMEPLAY_PlayerHealthPercent")}", "GAMEPLAY_Food", "ico_status_hunger1", Color.cyan);
                    if (__instance.m_FoodItemEaten.m_StackableItem)
                    {
                        if (__instance.m_FoodItemEaten.m_StackableItem.m_Units == 1)
                        {
                            UnityEngine.Object.Destroy(__instance.m_FoodItemEaten.gameObject);
                        }
                        else
                        {
                            __instance.m_FoodItemEaten.m_StackableItem.m_Units--;
                        }
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(__instance.m_FoodItemEaten.gameObject);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BodyHarvest), "Awake")]
        private static class BodyHarvest_Awake
        {
            private static void Postfix(BodyHarvest __instance)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                __instance.enabled = false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(LifeAfterDeathManager), "DeserializeRecoveryCampsite")]
        private static class LifeAfterDeathManager_DeserializeRecoveryCampsite
        {
            private static bool Prefix(LifeAfterDeathManager __instance, string currentScene)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(LifeAfterDeathManager), "PerformRespawn")]
        private static class LifeAfterDeathManager_PerformRespawn
        {
            private static void Prefix(LifeAfterDeathManager __instance, LifeAfterDeathSpawnPointType spawnPointType, bool permanentAffliction)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                ClientSend.SendRevived(-2);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GameManager), "GetRandomSeed")]
        private static class GameManagerr_GetRandomSeed
        {
            private static void Postfix(GameManager __instance, int seed, ref int __result)
            {
                if (!ModMain.IsMultiplayer()) { return; }

                if (!GameManager.IsOutDoorsScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name))
                {
                    if (GameManager.m_SceneTransitionData != null)
                    {
                        __result = (GameManager.m_SceneTransitionData.m_GameRandomSeed + ModMain.GetDeterministicSeed(GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent)) ^ seed;
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(LoadScene), "Start")]
        private static class LoadScene_Start
        {
            private static bool Prefix(LoadScene __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (!__instance.m_Active || __instance.m_StartHasBeenCalled)
                    return false;
                __instance.m_StartHasBeenCalled = true;
                __instance.m_FadeOutStarted = false;
                if (__instance.m_LoadSceneParent != null)
                {
                    __instance.m_LoadSceneParent.Start();
                    __instance.m_GUID = __instance.m_LoadSceneParent.m_GUID;
                }
                else
                {
                    __instance.m_GUID = ModMain.GenerateSeededGUID(GameManager.m_SceneTransitionData.m_GameRandomSeed, __instance.gameObject.transform.position);
                }
                if (__instance.m_TransitionOnContact)
                {
                    __instance.GetComponent<Collider>().isTrigger = true;
                    vp_Layer.Set(__instance.gameObject, 21);
                }
                else if (__instance.gameObject.layer != 21)
                {
                    vp_Layer.Set(__instance.gameObject, 19);
                }
                if (__instance.m_LoadSceneParent != null)
                {
                    __instance.m_LoadSceneParent.Register(__instance);
                }
                __instance.Lock = __instance.gameObject.GetComponent<Lock>();
                if (!__instance.Lock)
                    return false;
                __instance.Lock.RollLockedState();

                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(LoadSceneParent), "Start")]
        private static class LoadSceneParent_Start
        {
            private static bool Prefix(LoadSceneParent __instance)
            {
                if (!ModMain.IsMultiplayer()) { return true; }

                if (__instance.m_StartHasBeenCalled)
                    return false;
                __instance.m_StartHasBeenCalled = true;

                __instance.m_GUID = ModMain.GenerateSeededGUID(GameManager.m_SceneTransitionData.m_GameRandomSeed, __instance.gameObject.transform.position);
                return false;
            }
        }
    }
}
