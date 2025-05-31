using Il2Cpp;
using Il2CppTLD.Gameplay;
using Il2CppTLD.Gear;
using Il2CppTLD.Scenes;
using Il2CppTLD.UI;
using SkyCoop;
using SkyCoopServer;
using UnityEngine;

namespace SkyCoopClient
{
    public class Minimalizer
    {
        public static string s_SceneSpawnOverride = "";
        
        [HarmonyLib.HarmonyPatch(typeof(MiniTopNav), "Update")]
        private static class MiniTopNav_Update
        {
            private static void Postfix(MiniTopNav __instance)
            {
                for (int i = __instance.m_ActiveElements.Count - 1; i >= 0; i--)
                {
                    MiniTopNavButton butt = __instance.m_ActiveElements[i];
                    if (butt && (butt.name != "SpriteFirstAid" && butt.name != "SpriteInventory" && butt.name != "SpriteInventory"))
                    {
                        __instance.m_ActiveElements.RemoveAt(i);
                    }
                }
                for (int i = __instance.m_NavElements.Count - 1; i >= 0; i--)
                {
                    MiniTopNavButton butt = __instance.m_NavElements[i];
                    if (butt && (butt.name != "SpriteFirstAid" && butt.name != "SpriteInventory" && butt.name != "SpriteInventory"))
                    {
                        butt.SetEnabled(false);
                        butt.gameObject.SetActive(false);
                    }
                }
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(Panel_Clothing), "Enable")]
        //private static class Panel_Clothing_Enable
        //{
        //    private static bool Prefix(Panel_Clothing __instance)
        //    {
        //        return false;
        //    }
        //}
        [HarmonyLib.HarmonyPatch(typeof(Panel_Map), "Enable", new System.Type[] { typeof(bool)})]
        private static class Panel_Map_Enable
        {
            private static bool Prefix(Panel_Map __instance)
            {
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Log), "Enable")]
        private static class Panel_Log_Enable
        {
            private static bool Prefix(Panel_Log __instance)
            {
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_RecipeBook), "Enable")]
        private static class Panel_RecipeBook_Enable
        {
            private static bool Prefix(Panel_RecipeBook __instance)
            {
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Crafting), "Enable", new System.Type[] { typeof(bool) })]
        private static class Panel_Crafting_Enable
        {
            private static bool Prefix(Panel_Crafting __instance)
            {
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Crafting), "Enable", new System.Type[] { typeof(bool), typeof(bool) })]
        private static class Panel_Crafting_Enable2
        {
            private static bool Prefix(Panel_Crafting __instance)
            {
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_ActionsRadial), "Enable", new System.Type[] { typeof(bool) })]
        private static class Panel_ActionsRadial_Update
        {
            private static void Postfix(Panel_ActionsRadial __instance)
            {
                Panel_ActionsRadial.RadialInfo Empty = new Panel_ActionsRadial.RadialInfo();
                Empty.m_RadialElement = Panel_ActionsRadial.RadialType.Empty;
                Empty.m_SpriteName = "";
                for (int i = __instance.m_PrimaryRadial.Count - 1; i >= 0; i--)
                {
                    Panel_ActionsRadial.RadialInfo Info = __instance.m_PrimaryRadial[i];
                    if (Info.m_RadialElement != Panel_ActionsRadial.RadialType.Weapons 
                        && Info.m_RadialElement != Panel_ActionsRadial.RadialType.FirstAid 
                        && Info.m_RadialElement != Panel_ActionsRadial.RadialType.Status 
                        && Info.m_RadialElement != Panel_ActionsRadial.RadialType.Food 
                        && Info.m_RadialElement != Panel_ActionsRadial.RadialType.Clothing
                        && Info.m_RadialElement != Panel_ActionsRadial.RadialType.Tools
                        && Info.m_RadialElement != Panel_ActionsRadial.RadialType.LightSources
                        && Info.m_RadialElement != Panel_ActionsRadial.RadialType.Drink
                        && Info.m_RadialElement != Panel_ActionsRadial.RadialType.Inventory)
                    {
                        if (Info.m_RadialElement == Panel_ActionsRadial.RadialType.Decoy)
                        {
                            Info.m_RadialElement = Panel_ActionsRadial.RadialType.Tools;
                            Info.m_GreyOutSpriteName = "ico_Radial_tools";
                            Info.m_SpriteName = "ico_Radial_tools";
                            Info.m_SpriteNameHover = "ico_Radial_tools";
                        }
                        else if(Info.m_RadialElement == Panel_ActionsRadial.RadialType.Navigation)
                        {
                            Info.m_RadialElement = Panel_ActionsRadial.RadialType.Clothing;
                            Info.m_GreyOutSpriteName = "ico_inv_clothing";
                            Info.m_SpriteName = "ico_inv_clothing";
                            Info.m_SpriteNameHover = "ico_inv_clothing";
                        }
                        else if (Info.m_RadialElement == Panel_ActionsRadial.RadialType.PlaceItem)
                        {
                            Info.m_RadialElement = Panel_ActionsRadial.RadialType.Inventory;
                            Info.m_GreyOutSpriteName = "ico_Radial_pack";
                            Info.m_SpriteName = "ico_Radial_pack";
                            Info.m_SpriteNameHover = "ico_Radial_pack";
                        }
                        else
                        {
                            Info.m_RadialElement = Panel_ActionsRadial.RadialType.Empty;
                            Info.m_SpriteName = "";
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
                __instance.m_Queue.Add(new Action(__instance.StartClothingUI));
                __instance.ShowGearRadial(GetClothingItemsInInventory(), new Action(ShowNoClothing));
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_FirstAid), "Enable")]
        private static class Panel_FirstAid_Start
        {
            private static void Postfix(Panel_FirstAid __instance)
            {
                __instance.transform.GetChild(2).gameObject.SetActive(false);
                __instance.transform.GetChild(5).GetChild(11).gameObject.SetActive(false);

            }
        }
        [HarmonyLib.HarmonyPatch(typeof(TimeWidget), "Start")]
        private static class TimeWidget_Start
        {
            private static void Postfix(TimeWidget __instance)
            {
                //UnityEngine.Object.Destroy(__instance.gameObject);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BreakDown), "Awake")]
        private static class BreakDown_Start
        {
            private static void Postfix(BreakDown __instance)
            {
                __instance.enabled = false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(HarvestableInteraction), "Awake")]
        private static class HarvestableInteraction_Start
        {
            private static void Postfix(HarvestableInteraction __instance)
            {
                __instance.enabled = false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(IceFishingHole), "Awake")]
        private static class IceFishingHole_Start
        {
            private static void Postfix(IceFishingHole __instance)
            {
                __instance.enabled = false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GenericStatusBarSpawner), "AssignValuesToSpawnedObject")]
        private static class GenericStatusBarSpawner_Start
        {
            private static void Postfix(GenericStatusBarSpawner __instance)
            {
                if (__instance.m_StatusBarType != StatusBar.StatusBarType.Condition)
                {
                    if (__instance.m_SpawnedObject)
                    {
                        __instance.m_SpawnedObject.SetActive(false);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GameManager), "Update")]
        private static class GameManager_Update
        {
            private static void Postfix(GameManager __instance)
            {
                if (GameManager.m_TimeOfDay)
                {
                    //GameManager.m_TimeOfDay.enabled = false;
                    GameManager.m_TimeOfDay.m_StartTimeHour = 12;
                    GameManager.m_TimeOfDay.m_StartTimeMinutes = 0;
                    //GameManager.m_TimeOfDay.SetTODLocked(true);
                    GameManager.m_TimeOfDay.SetNormalizedTime(0.5f);
                }
                if (GameManager.m_Weather)
                {
                    GameManager.m_Weather.enabled = false;
                }
                if (GameManager.m_WeatherTransition)
                {
                    //GameManager.m_WeatherTransition.enabled = false;
                    GameManager.m_WeatherTransition.m_DefaultStartWeather = WeatherStage.Clear;
                    if (GameManager.m_WeatherTransition.m_CurrentWeatherSet)
                    {
                        GameManager.m_WeatherTransition.m_CurrentWeatherSet.SetDirty();
                    }
                    GameManager.m_WeatherTransition.ActivateDefaultWeatherSet();
                    WeatherTransition.m_WeatherTransitionTimeScalar = 1;
                }
                if (GameManager.m_Wind)
                {
                    GameManager.m_Wind.enabled = false;
                    GameManager.m_Wind.m_CurrentAngleDeg = 0;
                    GameManager.m_Wind.m_CurrentAngleDeg_Base = 0;
                    GameManager.m_Wind.m_CurrentMPH = 0;
                    GameManager.m_Wind.m_CurrentMPH_Base = 0;
                    GameManager.m_Wind.m_CurrentDirection = Vector3.zero;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "SpawningSupppressedByExperienceMode")]
        private static class SpawnRegion_SpawningSupppressedByExperienceMode
        {
            private static void Postfix(SpawnRegion __instance, bool __result)
            {
                __result = true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "Spawn")]
        private static class SpawnRegion_Spawn
        {
            private static bool Prefix(SpawnRegion __instance)
            {
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PrefabSpawn), "SpawnObject")]
        private static class PrefabSpawn_SpawnObject
        {
            private static bool Prefix(PrefabSpawn __instance)
            {
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(RadialObjectSpawner), "SpawnAtPosition")]
        private static class RadialObjectSpawner_SpawnAtPosition
        {
            private static bool Prefix(RadialObjectSpawner __instance)
            {
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(RadialSpawnManager), "DeserializeAll")]
        private static class RadialSpawnManager_DeserializeAll
        {
            private static void Prefix(RadialSpawnManager __instance)
            {
                RadialSpawnManager.m_RadialSpawnObjects.Clear();
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GameManager), "AllScenesLoaded")]
        private static class GameManager_AllScenesLoaded
        {
            private static void Postfix(GameManager __instance)
            {
                SkyCoop.Logger.Log(ConsoleColor.Cyan, "Scenes loaded");

                for (int i = GearManager.m_Gear.Count - 1; i >= 0; i--)
                {
                    GearItem item = GearManager.m_Gear[i];
                    if (!item.m_HasBeenOwnedByPlayer && !item.m_BeenInPlayerInventory)
                    {
                        GearManager.DestroyGearObject(item);
                    }
                }
                ClientSend.SendNewScene(ModMain.GetCurrentSceneName());
            }
        }


        [HarmonyLib.HarmonyPatch(typeof(Fatigue), "Update")]
        private static class Fatigue_Update
        {
            private static void Postfix(Fatigue __instance)
            {
                __instance.m_CurrentFatigue = 0;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Hunger), "Update")]
        private static class Hunger_Update
        {
            private static void Postfix(Hunger __instance)
            {
                __instance.m_CurrentReserveCalories = __instance.m_MaxReserveCalories*0.9f;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Thirst), "Update")]
        private static class Thirst_Update
        {
            private static void Postfix(Thirst __instance)
            {
                __instance.m_CurrentThirst = 15;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(EmergencyStim), "ApplyEmergencyStimExitEffects")]
        private static class EmergencyStim_ApplyEmergencyStimExitEffects
        {
            private static void Postfix(EmergencyStim __instance)
            {
                GameManager.GetDiminishedState().Apply(1, AfflictionOptions.None);
                GameManager.GetSprainPainComponent().ApplyAffliction(AfflictionBodyArea.LegLeft, "Emergency Stimulator");
                GameManager.GetSprainPainComponent().ApplyAffliction(AfflictionBodyArea.LegRight, "Emergency Stimulator");
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Condition), "PlayerDeath")]
        private static class Condition_PlayerDeath
        {
            private static bool Prefix(Condition __instance)
            {
                DataStr.DamageType DamageType = DataStr.DamageType.Unknown;

                SkyCoop.Logger.Log("PlayerDeath Cause " + __instance.m_CauseOfDeath);

                DamageType = PlayersManager.m_LastDamageType;
                SkyCoop.Logger.Log("PlayerDeath DamageType " + DamageType);


                if(ModMain.Client != null && ModMain.Client.m_Rules.m_PlayerCanBeKnocked)
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
                if (GameManager.GetBrokenBody().HasAffliction)
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
                if (GameManager.GetBrokenBody().HasAffliction)
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
        [HarmonyLib.HarmonyPatch(typeof(LoadingZone), "Awake")]
        private static class LoadingZone_Awake
        {
            private static void Postfix(LoadingZone __instance)
            {
                __instance.enabled = false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Hypothermia), "HypothermiaStart")]
        private static class Hypothermia_HypothermiaStart
        {
            private static bool Prefix(Hypothermia __instance)
            {
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Hypothermia), "Start")]
        private static class Hypothermia_Start
        {
            private static void Postfix(Hypothermia __instance)
            {
                __instance.m_SuppressHypothermia = true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(StartGear), "AddAllToInventory")]
        private static class StartGear_AddAllToInventory
        {
            private static bool Prefix(StartGear __instance)
            {
                List<DataStr.StartingGearData> StartingGear = new List<DataStr.StartingGearData>();

                if (ModMain.Client != null)
                {
                    StartingGear = ModMain.Client.m_Rules.m_StartingItems;
                }

                if(StartingGear.Count == 0)
                {
                    return true;
                }

                GameManager.GetInventoryComponent().DestroyAllGear();

                foreach (DataStr.StartingGearData Gear in StartingGear)
                {
                    string GearName = Gear.Get();

                    if (!string.IsNullOrEmpty(GearName))
                    {
                        GearItem GearItem = PlayersManager.GiveItemToPlayer(GearName, Gear.Units);
                        if (GearItem)
                        {
                            if (GearItem.m_GunItem)
                            {
                                GearItem.m_GunItem.FillClipAtCondition(100);
                            }
                            if (GearItem.m_ClothingItem)
                            {
                                GearItem.m_ClothingItem.PutOn();
                                GearItem.m_NarrativeCollectibleItem = GearItem.gameObject.AddComponent<NarrativeCollectibleItem>();
                            }
                        }
                    }
                }
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_LifeAfterDeath), "Enable")]
        private static class Panel_LifeAfterDeath_Enable
        {
            private static void Postfix(Panel_LifeAfterDeath __instance)
            {
                __instance.m_CampfireGrid.gameObject.SetActive(false);
                UILocalize RespawnButton = __instance.m_CheatDeathButtonWidget.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<UILocalize>();
                
                if(ModMain.Client != null && ModMain.Client.m_Rules.m_Respawns)
                {
                    RespawnButton.key = "Respawn";
                }
                else
                {
                    RespawnButton.key = "Spectate";
                }
                
                RespawnButton.OnLocalize();

                UILocalize QuitButton = __instance.m_CheatDeathButtonWidget.transform.parent.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<UILocalize>();
                QuitButton.key = "Rage Quit!";
                QuitButton.OnLocalize();
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_LifeAfterDeath), "HandleOnLeftButtonPressed")]
        private static class Panel_LifeAfterDeath_HandleOnLeftButtonPressed
        {
            private static bool Prefix(Panel_LifeAfterDeath __instance)
            {
                GameManager.GetConditionComponent().ResetAudio();
                ClientSend.SendRespawnRequest();
                MenuHook.RemovePleaseWait();
                MenuHook.DoPleaseWait("Да блин, жди что ли...", "Грузим шпингалеты...");
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Condition), "PlayDeathMusic")]
        private static class Condition_PlayDeathMusic
        {
            private static bool Prefix(Condition __instance)
            {
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_LifeAfterDeath), "HandleOnRightButtonPressed")]
        private static class Panel_LifeAfterDeath_HandleOnRightButtonPressed
        {
            private static bool Prefix(Panel_LifeAfterDeath __instance)
            {
                if (ModMain.Client != null)
                {
                    ModMain.Client.m_Instance.Stop();
                    Application.Quit();
                }
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SafehouseManager), "MaybeToggleCustomizing")]
        private static class SafehouseManager_Enable
        {
            private static bool Prefix(SafehouseManager __instance)
            {
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SafehouseManager), "InCustomizableSafehouse")]
        private static class SafehouseManager_InCustomizableSafehouse
        {
            private static void Postfix(SafehouseManager __instance, ref bool __result)
            {
                __result = false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GameManager), "LoadSceneWithLoadingScreen", new System.Type[] { typeof(string) })]
        private static class GameManager_LoadSceneWithLoadingScreen
        {
            private static bool Prefix(GameManager __instance, string sceneName)
            {
                SkyCoop.Logger.Log("LoadSceneWithLoadingScreen");
                if (string.IsNullOrEmpty(s_SceneSpawnOverride))
                {
                    return true;
                }
                SkyCoop.Logger.Log("s_SceneSpawnOverride "+ s_SceneSpawnOverride);
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
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "EatingComplete_Internal")]
        private static class PlayerManager_EatingComplete_Internal
        {
            private static void Postfix(PlayerManager __instance, bool success, bool playerCancel, float progress)
            {
                if(success && !playerCancel && __instance.m_FoodItemEaten)
                {
                    float Cal = __instance.m_FoodItemEaten.m_FoodItem.m_CaloriesTotal;

                    float Health = 60 * Cal / 1500f;

                    GameManager.GetConditionComponent().AddHealth(Health, DamageSource.FirstAid);

                    PlayerDamageEvent.SpawnAfflictionEvent($"+{Math.Round(Health).ToString()} {Localization.Get("GAMEPLAY_PlayerHealthPercent")}", "GAMEPLAY_Food", "ico_status_hunger1", Color.cyan);
                    if (__instance.m_FoodItemEaten.m_StackableItem)
                    {
                        if(__instance.m_FoodItemEaten.m_StackableItem.m_Units == 1)
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
                __instance.enabled = false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(WaterSource), "Awake")]
        private static class WaterSource_Awake
        {
            private static void Postfix(WaterSource __instance)
            {
                __instance.enabled = false;
            }
        }
    }
}
