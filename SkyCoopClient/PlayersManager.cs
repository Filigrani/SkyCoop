using Il2Cpp;
using Il2CppTLD.Gameplay;
using Il2CppTLD.Interactions;
using Il2CppTLD.Stats;
using MelonLoader;
using SkyCoopClient;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2Cpp.PlayerManager;
using UnityEngine.AddressableAssets;
using static SkyCoop.Comps;
using Il2CppRewired.Demos;

namespace SkyCoop
{
    public static class PlayersManager
    {
        public static List<Comps.NetworkPlayer> s_Players = new List<Comps.NetworkPlayer>();
        public static LocalPlayerData m_LocalPlayerData = new LocalPlayerData();
        public class LocalPlayerData
        {
            public Vector3 m_LastSentPosition = Vector3.zero;
            public Quaternion m_LastSentRotation = Quaternion.identity;
            public string m_LastSentScene = "MainMenu";

            public string m_GearName = "";
            public int m_GearVariant = 0;

            public bool m_LastSentCrouch = false;
            public Comps.NetworkPlayer.Actions m_LastSentAction = Comps.NetworkPlayer.Actions.None;
        }

        public static Comps.NetworkPlayer CreatePlayer(int PlayerID)
        {
            GameObject Reference = AssetManager.GetAssetFromBundle<GameObject>("SkyCoopPlayer");

            if(Reference)
            {
                GameObject Player = GameObject.Instantiate(Reference);
                if(Player)
                {
                    UnityEngine.Object.DontDestroyOnLoad(Player); // if scence change, this object won't be destroyed.

                    Comps.NetworkPlayer NP = Player.AddComponent<Comps.NetworkPlayer>();
                    NP.m_Animator = Player.GetComponent<Animator>();
                    NP.LoadEquipment();
                    NP.AddInteraction();
                    NP.AddAudioSource();
                    NP.CreateColiders();

                    NP.m_PlayerID = PlayerID;

                    Player.SetActive(false);

                    return NP;
                }
            }

            return null;
        }

        public static Comps.NetworkPlayer GetPlayer(int Index)
        {
            return s_Players[Index];
        }

        public static void InitilizePlayers(int PlayersCount)
        {
            m_LocalPlayerData = new LocalPlayerData();
            
            // Trying to re-use such complex objects as much as possible.
            // So in for some reason we have less or more characters already exist
            // (for example join server with less/more Max Players), we just add/remove 
            // Instead of re-creating whole list of objects.
            while (s_Players.Count != PlayersCount)
            {
                if (s_Players.Count > PlayersCount)
                {
                    int Index = s_Players.Count - 1;
                    Comps.NetworkPlayer Player = s_Players[Index];
                    if (Player != null)
                    {
                        UnityEngine.Object.Destroy(Player.gameObject);
                    }
                    s_Players.RemoveAt(Index);
                } else if (s_Players.Count < PlayersCount)
                {
                    int Index = s_Players.Count;
                    Comps.NetworkPlayer Player = CreatePlayer(Index);
                    if (Player == null)
                    {
                        Logger.Log(ConsoleColor.Red, "[PlayersManager][InitilizePlayers] Wasn't able to create player object!");
                        break; // Else, we going to go to infinite loop.
                    }
                    s_Players.Add(Player);
                }
            }
        }

        public static Comps.NetworkPlayer.Actions GetCurrentAction()
        {
            Panel_BreakDown bk = InterfaceManager.GetPanel<Panel_BreakDown>();
            if (bk && bk.IsBreakingDown())
            {
                return Comps.NetworkPlayer.Actions.Harvesting;
            }
            Panel_Diagnosis Diagnos = InterfaceManager.GetPanel<Panel_Diagnosis>();
            if (Diagnos && Diagnos.TreatmentInProgress())
            {
                return Comps.NetworkPlayer.Actions.Harvesting;
            }
            Panel_BodyHarvest bh = InterfaceManager.GetPanel<Panel_BodyHarvest>();
            if(bh && bh.m_BodyHarvest)
            {
                return Comps.NetworkPlayer.Actions.Harvesting;
            }
            PlayerManager PM = GameManager.GetPlayerManagerComponent();
            if (PM.ActiveInteraction != null && PM.ActiveInteraction.GetInteractiveObject().GetComponent<HarvestableInteraction>())
            {
                return Comps.NetworkPlayer.Actions.Harvesting;
            }

            PlayerControlMode CMode = PM.GetControlMode();
            if (CMode == PlayerControlMode.AimRevolver)
            {
                return Comps.NetworkPlayer.Actions.PistolAim;
            }
            if(CMode == PlayerControlMode.StartingFire)
            {
                return Comps.NetworkPlayer.Actions.Igniting;
            }
            if(CMode == PlayerControlMode.DeployRope || CMode == PlayerControlMode.TakeRope)
            {
                return Comps.NetworkPlayer.Actions.Harvesting;
            }
            if (PM.m_ItemInHands)
            {
                if(PM.m_ItemInHands.name == "GEAR_Rifle")
                {
                    if (PM.m_ItemInHands.m_GunItem.IsAiming())
                    {
                        return Comps.NetworkPlayer.Actions.RifleAim;
                    }
                }else if(PM.m_ItemInHands.name == "GEAR_FlareGun")
                {
                    if (PM.m_ItemInHands.m_GunItem.IsAiming())
                    {
                        return Comps.NetworkPlayer.Actions.PistolAim;
                    }
                }
            }

            return Comps.NetworkPlayer.Actions.None;
        }

        public static void UpdateLocalPlayer()
        {
            if(!GameManager.s_IsGameplaySuspended)
            {
                string Scene = ModMain.GetCurrentSceneName();
                if (ModMain.IsGameplayScene(Scene))
                {
                    if (GameManager.m_PlayerObject)
                    {
                        Transform T = GameManager.GetPlayerTransform();
                        if (m_LocalPlayerData.m_LastSentPosition != T.position)
                        {
                            m_LocalPlayerData.m_LastSentPosition = T.position;
                            ClientSend.SendPosition(T.position);
                        }
                        if (m_LocalPlayerData.m_LastSentRotation != T.rotation)
                        {
                            m_LocalPlayerData.m_LastSentRotation = T.rotation;
                            ClientSend.SendRotation(T.rotation);
                        }
                        PlayerManager PM = GameManager.GetPlayerManagerComponent();
                        if (PM)
                        {
                            GearItem Gi = PM.m_ItemInHands;
                            string HoldingGear = Gi ? Gi.name : "";
                            if (m_LocalPlayerData.m_GearName != HoldingGear)
                            {
                                m_LocalPlayerData.m_GearName = HoldingGear;
                                ClientSend.SendHoldingGear(HoldingGear, m_LocalPlayerData.m_GearVariant);
                            }
                            if (m_LocalPlayerData.m_LastSentCrouch != PM.PlayerIsCrouched())
                            {
                                m_LocalPlayerData.m_LastSentCrouch = PM.PlayerIsCrouched();
                                ClientSend.SendCrouch(m_LocalPlayerData.m_LastSentCrouch);
                            }
                            Comps.NetworkPlayer.Actions Action = GetCurrentAction();
                            if(m_LocalPlayerData.m_LastSentAction != Action)
                            {
                                m_LocalPlayerData.m_LastSentAction = Action;
                                ClientSend.SendAction((int)Action);
                            }
                        }
                    }
                }

                if (m_LocalPlayerData.m_LastSentScene != Scene)
                {
                    m_LocalPlayerData.m_LastSentScene = Scene;
                    ClientSend.SendScene(Scene);
                }
            }
        }

        public static void AddLocalizedSprain(AfflictionBodyArea location, string causeID)
        {
            bool Legs = false;
            if (location == AfflictionBodyArea.LegLeft)
            {
                location = AfflictionBodyArea.FootLeft;
                Legs = true;
            }
            else if (location == AfflictionBodyArea.LegRight)
            {
                location = AfflictionBodyArea.FootRight;
                Legs = true;
            }
            else if (location == AfflictionBodyArea.ArmLeft)
            {
                location = AfflictionBodyArea.HandLeft;
            }
            else if (location == AfflictionBodyArea.ArmRight)
            {
                location = AfflictionBodyArea.HandRight;
            }

            if (Legs)
            {
                SprainedAnkle Spr = GameManager.GetSprainedAnkleComponent();

                Spr.m_CausesLocIDs.Add(causeID);
                Spr.m_Locations.Add((int)location);
                Spr.m_ElapsedHoursList.Add(0.0f);
                Spr.m_DurationHoursList.Add(UnityEngine.Random.Range(Spr.m_DurationHoursMin, Spr.m_DurationHoursMax));
                Spr.m_ElapsedRestList.Add(0.0f);
                Spr.m_SecondsSinceLastPainAudio = 0.0f;
                Spr.DoStumbleEffects();
                PlayerDamageEvent.SpawnDamageEvent(Spr.m_LocalizedDisplayName.m_LocalizationID, "GAMEPLAY_Affliction", "ico_injury_sprainedAnkle", InterfaceManager.m_FirstAidRedColor, true, 5, 2);
                GameManager.GetPlayerVoiceComponent().Play(Spr.m_SprainedAnkleAudioEvent, Il2CppVoice.Priority.Critical);
                GameAudioManager.PlaySound(Spr.m_SprainedAnkleSFXEvent, Spr.gameObject);

                GameManager.GetLogComponent().AddAffliction(AfflictionType.SprainedAnkle, causeID);
                StatsManager.IncrementValue(StatID.Sprains_Ankle);
            }
            else
            {
                SprainedWrist Spr = GameManager.GetSprainedWristComponent();

                Spr.m_CausesLocIDs.Add(causeID);
                Spr.m_Locations.Add((int)location);
                Spr.m_ElapsedHoursList.Add(0.0f);
                Spr.m_DurationHoursList.Add(UnityEngine.Random.Range(Spr.m_DurationHoursMin, Spr.m_DurationHoursMax));
                Spr.m_ElapsedRestList.Add(0.0f);
                Spr.DoStumbleEffects();
                PlayerDamageEvent.SpawnDamageEvent(Spr.m_LocalizedDisplayName.m_LocalizationID, "GAMEPLAY_Affliction", "ico_injury_sprainedWrist", InterfaceManager.m_FirstAidRedColor, true, 5, 2);
                GameManager.GetPlayerVoiceComponent().Play(Spr.m_SprainedWristVO, Il2CppVoice.Priority.Critical);
                GameAudioManager.PlaySound(Spr.m_SprainedWristSFX, Spr.gameObject);
                GameManager.GetLogComponent().AddAffliction(AfflictionType.SprainedWrist, causeID);
                StatsManager.IncrementValue(StatID.Sprains_Wrist);
            }
            GameManager.GetSprainPainComponent().ApplyAffliction(location, causeID, AfflictionOptions.PlayFX);
        }

        public static void OtherPlayerDamageMe(float damage, int from, Comps.PlayerDamageColider.DamageZone bodypart, bool Meele, string MeleeWeapon)
        {
            if (damage <= 0)
            {
                return;
            }

            SkyCoop.Logger.Log("OtherPlayerDamageMe Damage "+ damage+" from "+from+" to the "+ bodypart.ToString());

            MeleeManager.MeleeDescripter DmgInfo;

            if (Meele)
            {
                DmgInfo = MeleeManager.GetMeelePlayerInfo(MeleeWeapon);
            }
            else
            {
                DmgInfo = new MeleeManager.MeleeDescripter();
                DmgInfo.m_PlayerDamage = damage;
                DmgInfo.m_AnimalDamage = 0;
                DmgInfo.m_BloodLoss = true;
                DmgInfo.m_ClothingTearing = true;
                DmgInfo.m_Pain = false;
            }

            string DamageCase = "Player";
            string Extra = " shoot you";
            if (Meele)
            {
                Extra = " hit you";
            }
            DamageCase = DamageCase + Extra;

            bool HasArmor = false;
            bool HasHelemet = false;

            if (bodypart == Comps.PlayerDamageColider.DamageZone.Chest) // If Chest
            {
                HasArmor = GameManager.GetDamageProtection().HasBallisticVest();
                if (HasArmor)
                {
                    damage = damage / 10;
                }
            }

            if (bodypart == Comps.PlayerDamageColider.DamageZone.Head) // If Head
            {
                //if (GetClothForSlot(ClothingRegion.Head, ClothingLayer.Mid) == "GEAR_CookingPot")
                //{
                //    damage = (20 * damage) / 100;
                //    HasHelemet = true;
                //}
            }



            GameManager.GetConditionComponent().AddHealth(-damage, DamageSource.BulletWound);
            AfflictionBodyArea BodyArea = AfflictionBodyArea.Head;

            switch (bodypart)
            {
                case Comps.PlayerDamageColider.DamageZone.Head:
                    BodyArea = AfflictionBodyArea.Head;
                    break;
                case Comps.PlayerDamageColider.DamageZone.Chest:
                    BodyArea = AfflictionBodyArea.Chest;
                    break;
                case Comps.PlayerDamageColider.DamageZone.RightArm:
                    BodyArea = AfflictionBodyArea.ArmRight;
                    break;
                case Comps.PlayerDamageColider.DamageZone.LeftArm:
                    BodyArea = AfflictionBodyArea.ArmLeft;
                    break;
                case Comps.PlayerDamageColider.DamageZone.RightLeg:
                    BodyArea = AfflictionBodyArea.LegRight;
                    break;
                case Comps.PlayerDamageColider.DamageZone.LeftLeg:
                    BodyArea = AfflictionBodyArea.LegLeft;
                    break;
            }

            if (!HasArmor && !HasHelemet)
            {
                if (DmgInfo.m_BloodLoss)
                {
                    GameManager.GetBloodLossComponent().BloodLossStartOverrideArea(BodyArea, DamageCase, true, AfflictionOptions.PlayFX);
                }
                if (DmgInfo.m_Pain)
                {
                    if (BodyArea == AfflictionBodyArea.Head)
                    {
                        HeadacheData Stock = GameManager.GetHeadacheComponent().m_LegacyHeadacheData;
                        HeadacheData headacheData = new HeadacheData();
                        headacheData.m_Cause = HeadacheCause.None;
                        LocalizedString Case = new LocalizedString();
                        Case.m_LocalizationID = DamageCase;
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
                    else if (BodyArea == AfflictionBodyArea.ArmRight
                        || BodyArea == AfflictionBodyArea.ArmLeft
                        || BodyArea == AfflictionBodyArea.LegLeft
                        || BodyArea == AfflictionBodyArea.LegRight)
                    {
                        AddLocalizedSprain(BodyArea, DamageCase);
                    }
                }

                ClothingRegion Region = ClothingRegion.Chest;

                switch (BodyArea)
                {
                    case AfflictionBodyArea.Head:
                        Region = ClothingRegion.Head;
                        break;
                    case AfflictionBodyArea.ArmLeft:
                        Region = ClothingRegion.Hands;
                        break;
                    case AfflictionBodyArea.ArmRight:
                        Region = ClothingRegion.Hands;
                        break;
                    case AfflictionBodyArea.Chest:
                        Region = ClothingRegion.Chest;
                        break;
                    case AfflictionBodyArea.LegLeft:
                        Region = ClothingRegion.Legs;
                        break;
                    case AfflictionBodyArea.LegRight:
                        Region = ClothingRegion.Legs;
                        break;
                }
                if (DmgInfo.m_ClothingTearing)
                {
                    var RNG = new System.Random(); int clothingRNG = RNG.Next(20, 40);
                    GameManager.GetPlayerManagerComponent().ApplyDamageToWornClothingRegion(Region, clothingRNG);
                }
            }
            else
            {
                var RNG = new System.Random(); int ribBroke = RNG.Next(0, 100);
                if (!Meele && ribBroke <= 5)
                {
                    GameManager.GetBrokenRibComponent().BrokenRibStart(DamageCase, true, false, true, false);
                }
                if (HasHelemet)
                {
                    HeadacheData Stock = GameManager.GetHeadacheComponent().m_LegacyHeadacheData;
                    HeadacheData headacheData = new HeadacheData();
                    headacheData.m_Cause = HeadacheCause.None;
                    LocalizedString Case = new LocalizedString();
                    Case.m_LocalizationID = DamageCase;
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

            GameManager.GetPlayerVoiceComponent().Play("PLAY_PLAYERDAMAGE", Il2CppVoice.Priority.Critical, PlayerVoice.Options.None);

            Transform V3 = GameManager.GetPlayerTransform();
            GameObject Player = GameManager.GetPlayerObject();

            GameAudioManager.SetMaterialSwitch("Flesh", Player);
            int num = (int)AkSoundEngine.PostEvent(Il2CppAK.EVENTS.PLAY_BULLETIMPACTS, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
            GameAudioManager.SetAudioSourceTransform(Player, V3);
        }

        public static float GetDamageValueForPlayer(float Damage, GunType Weapon)
        {
            if(Weapon == GunType.Rifle)
            {
                return 50;
            }
            if(Weapon == GunType.Revolver)
            {
                return 30;
            }
            return Damage;
        }

        [HarmonyLib.HarmonyPatch(typeof(vp_FPSCamera), "Awake")]
        private static class vp_FPSCamera_Awake
        {
            private static void Postfix(vp_FPSCamera __instance)
            {
                if (__instance.GetComponent<AudioListener>() == null)
                {
                    __instance.gameObject.AddComponent<AudioListener>();
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(vp_Bullet), "Start")]
        private static class vp_Bullet_Start
        {
            private static void Prefix(vp_Bullet __instance)
            {
                Comps.OtherPlayerBullet OPB = __instance.gameObject.GetComponent<Comps.OtherPlayerBullet>();
                if (OPB == null)
                {
                    string ProjectileName = "";
                    if(__instance.m_GunType == GunType.Rifle)
                    {
                        ProjectileName = "Rifle";
                    }else if(__instance.m_GunType == GunType.Revolver)
                    {
                        ProjectileName = "Revolver";
                    }
                    ClientSend.SendProjectile(__instance.transform.position, __instance.transform.rotation, ProjectileName);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(vp_FPSShooter), "Start")] // Once
        internal class vp_FPSShooter_Start
        {
            public static void Postfix(vp_FPSShooter __instance)
            {
                if (__instance != null && __instance.gameObject != null && __instance.ProjectilePrefab != null)
                {
                    if (__instance.gameObject.name == "Rifle" && __instance.ProjectilePrefab.name == "PistolBullet")
                    {
                        AssetManager.s_PistolBulletPrefab = __instance.ProjectilePrefab;
                    }
                    if (__instance.gameObject.name == "Revolver" && __instance.ProjectilePrefab.name == "RevolverBullet")
                    {
                        AssetManager.s_RevolverBulletPrefab = __instance.ProjectilePrefab;
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(vp_FPSShooter), "Fire")] // Once
        internal class vp_FPSShooter_Fire
        {
            public static void Prefix(vp_FPSShooter __instance)
            {
                if (__instance.m_Weapon.GetAmmoCount() < 1)
                {
                    ClientSend.SendProjectile(GameManager.GetPlayerTransform().position, GameManager.GetPlayerTransform().rotation, "DryFire");
                    return;
                }
                else
                {
                    if (__instance.m_Weapon.m_GunItem.m_IsJammed)
                    {
                        ClientSend.SendProjectile(GameManager.GetPlayerTransform().position, GameManager.GetPlayerTransform().rotation, "DryFire");
                        return;
                    }
                }
                if (__instance.ProjectilePrefab.name == "GEAR_FlareGunAmmoSingle")
                {
                    ClientSend.SendProjectile(__instance.m_Camera.transform.position, __instance.m_Camera.transform.rotation, "GEAR_FlareGunAmmoSingle");
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(vp_Bullet), "SpawnImpactEffects")]
        private static class vp_Bullet_SpawnImpactEffects
        {
            private static void Postfix(vp_Bullet __instance, RaycastHit hit)
            {
                Comps.OtherPlayerBullet OPB = __instance.gameObject.GetComponent<Comps.OtherPlayerBullet>();
                if (OPB == null)
                {
                    Comps.PlayerDamageColider PlayerColider = hit.collider.gameObject.gameObject.GetComponent<Comps.PlayerDamageColider>();
                    if (PlayerColider)
                    {
                        SkyCoop.Logger.Log("Bullet hits Player " + PlayerColider.m_Player.m_PlayerID + "  to the " + PlayerColider.m_DamageZone.ToString());


                        ClientSend.SendDamageToPlayer(GetDamageValueForPlayer(__instance.Damage, __instance.m_GunType) * PlayerColider.m_DamageScaler, PlayerColider.m_Player.m_PlayerID, PlayerColider.m_DamageZone, false);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(ConsoleManager), "CONSOLE_god")]
        private static class ConsoleManager_CONSOLE_god
        {
            private static void Postfix()
            {
                GameManager.GetPlayerMovementComponent().SetForceCrouch(false);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(EmergencyStim), "ApplyEmergencyStim")]
        private static class EmergencyStim_ApplyEmergencyStim
        {
            private static void Postfix(EmergencyStim __instance)
            {
                if (GameManager.GetBrokenBody().HasAffliction)
                {
                    RespawnMe(true, true);
                }
            }
        }

        public static GearItem GiveItemToPlayer(string GearName, int units = 1)
        {
            GearItem gearItem = Addressables.LoadAssetAsync<GameObject>(GearName).WaitForCompletion().GetComponent<GearItem>();
            GearItem given = GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory(gearItem, units, 1f, InventoryInstantiateFlags.None);
            return given;
        }

        public static void RespawnMe(bool FromKnockedDownState = false, bool EmergencyStim = false)
        {
            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Normal);
            GameManager.GetBrokenBody().Cure();
            GameManager.GetPlayerMovementComponent().SetForceCrouch(false);

            if (!EmergencyStim)
            {
                GameManager.GetLifeAfterDeathManager().PlayRespawnTimeline();
            }
            else
            {
                GameManager.GetDiminishedState().Cure();
            }
            if (!FromKnockedDownState)
            {
                GameManager.GetConditionComponent().m_CurrentHP = GameManager.GetConditionComponent().GetAdjustedMaxHP();
                ConsoleManager.CONSOLE_afflictions_cure();
                GameManager.GetInventoryComponent().DestroyAllGear();
                GameManager.GetPlayerManagerComponent().m_StartGear.AddAllToInventory();
                GameObject SP = PlayerManager.PickRandomSpawnPoint();
                GameManager.GetPlayerManagerComponent().TeleportPlayer(SP.transform.position, SP.transform.rotation);
            }
            else
            {
                GameManager.GetConditionComponent().m_CurrentHP = 30;
            }
        }

        public static void HandleProjectileSync(Vector3 Position, Quaternion Rotation, string ProjectileName)
        {
            SkyCoop.Logger.Log("HandleProjectileSync "+ ProjectileName);
            if(ProjectileName == "Rifle" || ProjectileName == "Revolver")
            {
                GameObject LightFX = new GameObject();
                LightFX.transform.position = Position;
                Light LightComp = LightFX.AddComponent<Light>();
                LightComp.type = LightType.Point;
                LightComp.range = 5;
                LightComp.intensity = 5;
                LightComp.color = new Color(1, 0.5623099f, 0.3268814f, 1);
                UnityEngine.Object.Destroy(LightFX, 0.1f);
            }
            GameObject SoundObj = null;
            GameObject Bullet = null;
            if (ProjectileName == "Rifle")
            {
                SoundObj = AssetManager.GetAssetFromBundle<GameObject>("3DRifleSound");
                if (AssetManager.s_PistolBulletPrefab)
                {
                    Bullet = UnityEngine.Object.Instantiate<GameObject>(AssetManager.s_PistolBulletPrefab, Position, Rotation);
                }
                else
                {
                    SkyCoop.Logger.Log(ConsoleColor.Red, "s_PistolBulletPrefab null!");
                }
            }
            else if (ProjectileName == "Revolver")
            {
                SoundObj = AssetManager.GetAssetFromBundle<GameObject>("3DRevolverSound");
                if (AssetManager.s_RevolverBulletPrefab)
                {
                    Bullet = UnityEngine.Object.Instantiate<GameObject>(AssetManager.s_RevolverBulletPrefab, Position, Rotation);
                }
                else
                {
                    SkyCoop.Logger.Log(ConsoleColor.Red, "s_RevolverBulletPrefab null!");
                }
            }
            else if (ProjectileName == "DryFire")
            {
                SoundObj = AssetManager.GetAssetFromBundle<GameObject>("3DDryFireSound");
            }
            else if(ProjectileName == "GEAR_FlareGunAmmoSingle")
            {
                FlareGunRoundItem.SpawnAndFire(AssetManager.GetAssetFromGame<GameObject>("GEAR_FlareGunAmmoSingle"), Position, Rotation);
            }
            if (Bullet)
            {
                Bullet.AddComponent<Comps.OtherPlayerBullet>();
            }
            if (SoundObj)
            {
                UnityEngine.Object.Instantiate<GameObject>(SoundObj, Position, Rotation);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BaseInteraction), "InitializeInteraction")]
        private static class BaseInteraction_InitializeInteraction
        {
            private static void Postfix(BaseInteraction __instance)
            {

            }
        }
    }
}
