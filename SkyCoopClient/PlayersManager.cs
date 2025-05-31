using Il2Cpp;
using Il2CppTLD.Interactions;
using Il2CppTLD.Stats;
using SkyCoopClient;
using UnityEngine;
using static Il2Cpp.PlayerManager;
using UnityEngine.AddressableAssets;
using SkyCoopServer;
using static SkyCoop.Comps.PlayerDamageColider;
using Il2CppTLD.PDID;
using static SkyCoopServer.DataStr;
using UnityEngine.UIElements;

namespace SkyCoop
{
    public static class PlayersManager
    {
        public static List<Comps.NetworkPlayer> s_Players = new List<Comps.NetworkPlayer>();
        public static LocalPlayerData m_LocalPlayerData = new LocalPlayerData();
        public static DataStr.DamageType m_LastDamageType = DataStr.DamageType.Unknown;
        public static Comps.PlayerDamageColider.DamageZone m_LastDamageZone = Comps.PlayerDamageColider.DamageZone.Chest;
        public static GameObject s_LastTryInteractionObject = null;
        public static bool s_ForceUpdateClothing = false;
        public static bool s_Spectator = false;

        public class LocalPlayerData
        {
            public Vector3 m_LastSentPosition = Vector3.zero;
            public Quaternion m_LastSentRotation = Quaternion.identity;
            public string m_LastSentScene = "MainMenu";

            public string m_GearName = "";
            public int m_GearVariant = 0;
            public string m_HeadGear = "";
            public string m_BodyGear = "";

            public bool m_LastSentCrouch = false;
            public bool m_LastSentInVehicle = false;
            public Comps.NetworkPlayer.Actions m_LastSentAction = Comps.NetworkPlayer.Actions.None;
            public DataStr.ClothingData m_ClothingData = new DataStr.ClothingData();
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
                    NP.CreateColiders();
                    NP.LoadEquipment();
                    NP.AddInteraction();
                    NP.AddAudioSource();
                    NP.UpdateClothing();

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
        public static string GetPlayerName(int Index)
        {
            Comps.NetworkPlayer Player = s_Players[Index];
            if (Player)
            {
                return Player.m_PlayerName;
            }
            return "";
        }
        public static void SetPlayerName(int Index, string Name)
        {
            Comps.NetworkPlayer Player = s_Players[Index];
            if (Player)
            {
                Player.m_PlayerName = Name;
                Player.UpdateName();
            }
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
            if (GameManager.GetConditionComponent().IsConsideredDead())
            {
                return Comps.NetworkPlayer.Actions.Death;
            }
            if (GameManager.GetBrokenBody().HasAffliction)
            {
                return Comps.NetworkPlayer.Actions.Knocked;
            }
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

        public static string GetClothForSlot(ClothingRegion s, ClothingLayer l)
        {
            if(GameManager.m_PlayerManager)
            {
                GearItem gi = GameManager.GetPlayerManagerComponent().GetClothingInSlot(s, l);
                if (gi != null)
                {
                    string dummyN = gi.name;
                    string finalN = "";
                    if (dummyN.Contains("(Clone)")) //If it has ugly (Clone), cutting it.
                    {
                        int L = dummyN.Length - 7;
                        finalN = dummyN.Remove(L, 7);
                    }
                    else
                    {
                        finalN = dummyN;
                    }
                    return finalN;
                }
            }
            return "";
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
                            if (m_LocalPlayerData.m_LastSentInVehicle != GameManager.GetPlayerInVehicle().m_InVehicle)
                            {
                                m_LocalPlayerData.m_LastSentInVehicle = GameManager.GetPlayerInVehicle().m_InVehicle;
                                ClientSend.SendInVehicle(m_LocalPlayerData.m_LastSentInVehicle);
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
                DataStr.ClothingData NewClothingData = ClothingCombininizer.GetClothing();
                if (!m_LocalPlayerData.m_ClothingData.Equals(NewClothingData) || s_ForceUpdateClothing)
                {
                    m_LocalPlayerData.m_ClothingData = NewClothingData;
                    ClientSend.SendClothing(NewClothingData);
                    SkyCoop.Logger.Log(ConsoleColor.Green, "m_LocalPlayerData SendClothing");
                    s_ForceUpdateClothing = false;
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

                GameManager.GetPlayerVoiceComponent().Play(Spr.m_SprainedAnkleAudioEvent, Il2CppVoice.Priority.Critical);
                GameAudioManager.PlaySound(Spr.m_SprainedAnkleSFXEvent, Spr.gameObject);
                Spr.DoStumbleEffects();

                for (int i = 0; i < Spr.m_Locations.Count; i++)
                {
                    if (Spr.m_Locations[i] == (int)location)
                    {
                        return;
                    }
                }

                Spr.m_CausesLocIDs.Add(causeID);
                Spr.m_Locations.Add((int)location);
                Spr.m_ElapsedHoursList.Add(0.0f);
                Spr.m_DurationHoursList.Add(UnityEngine.Random.Range(Spr.m_DurationHoursMin, Spr.m_DurationHoursMax));
                Spr.m_ElapsedRestList.Add(0.0f);
                Spr.m_SecondsSinceLastPainAudio = 0.0f;
                PlayerDamageEvent.SpawnDamageEvent(Spr.m_LocalizedDisplayName.m_LocalizationID, "GAMEPLAY_Affliction", "ico_injury_sprainedAnkle", InterfaceManager.m_FirstAidRedColor, true, 5, 2);
                GameManager.GetLogComponent().AddAffliction(AfflictionType.SprainedAnkle, causeID);
                StatsManager.IncrementValue(StatID.Sprains_Ankle);
            }
            else
            {
                SprainedWrist Spr = GameManager.GetSprainedWristComponent();
                GameManager.GetPlayerVoiceComponent().Play(Spr.m_SprainedWristVO, Il2CppVoice.Priority.Critical);
                GameAudioManager.PlaySound(Spr.m_SprainedWristSFX, Spr.gameObject);
                Spr.DoStumbleEffects();

                for (int i = 0; i < Spr.m_Locations.Count; i++)
                {
                    if (Spr.m_Locations[i] == (int)location)
                    {
                        return;
                    }
                }

                GearItem Item = GameManager.GetPlayerManagerComponent().m_ItemInHands;
                if (Item)
                {
                    GameManager.GetPlayerManagerComponent().UnequipItemInHands();
                }

                Spr.m_CausesLocIDs.Add(causeID);
                Spr.m_Locations.Add((int)location);
                Spr.m_ElapsedHoursList.Add(0.0f);
                Spr.m_DurationHoursList.Add(UnityEngine.Random.Range(Spr.m_DurationHoursMin, Spr.m_DurationHoursMax));
                Spr.m_ElapsedRestList.Add(0.0f);
                PlayerDamageEvent.SpawnDamageEvent(Spr.m_LocalizedDisplayName.m_LocalizationID, "GAMEPLAY_Affliction", "ico_injury_sprainedWrist", InterfaceManager.m_FirstAidRedColor, true, 5, 2);
                GameManager.GetLogComponent().AddAffliction(AfflictionType.SprainedWrist, causeID);
                StatsManager.IncrementValue(StatID.Sprains_Wrist);
            }
            GameManager.GetSprainPainComponent().ApplyAffliction(location, causeID, AfflictionOptions.PlayFX);
        }

        public static void OtherPlayerDamageMe(float damage, int from, Comps.PlayerDamageColider.DamageZone bodypart, string Weapon)
        {
            if (damage <= 0)
            {
                return;
            }

            WeaponsManager.WeaponDescripter DmgInfo = WeaponsManager.GetDescriptor(Weapon);

            string DamageCase = "Player";
            string Extra = " shoot you";
            if (DmgInfo.m_IsMelee)
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
                if (GetClothForSlot(ClothingRegion.Head, ClothingLayer.Mid) == "GEAR_CookingPot")
                {
                    damage = (20 * damage) / 100;
                    HasHelemet = true;
                }
            }

            m_LastDamageType = DmgInfo.m_DamageType;
            m_LastDamageZone = bodypart;
            SkyCoop.Logger.Log("OtherPlayerDamageMe Damage " + damage + " from " + from + " to the " + bodypart.ToString()+" using "+Weapon+" dealing damage type "+ DmgInfo.m_DamageType);
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
                        if (!GameManager.GetHeadacheComponent().HasHeadache())
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
                if (!DmgInfo.m_IsMelee && ribBroke <= 5)
                {
                    GameManager.GetBrokenRibComponent().BrokenRibStart(DamageCase, true, false, true, false);
                }
                if (HasHelemet)
                {
                    if (GameManager.GetHeadacheComponent().HasHeadache())
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
            }

            if (DmgInfo.m_Burn)
            {
                GameManager.GetBurnsComponent().BurnsStart(DamageCase, true, true, AfflictionOptions.PlayFX);
            }

            if(Weapon == "GEAR_Bow" || Weapon == "GEAR_Arrow" || Weapon == "GEAR_ArrowHardened")
            {
                AddArrowAffiction(BodyArea, DamageCase, Weapon);
            }

            if(Weapon == "ZONE")
            {
                CanvasUI.DoZoneDamageOverlay();
                SprainedAnkle Spr = GameManager.GetSprainedAnkleComponent();
                GameManager.GetPlayerVoiceComponent().Play(Spr.m_SprainedAnkleAudioEvent, Il2CppVoice.Priority.Critical);
                GameAudioManager.PlaySound(Spr.m_SprainedAnkleSFXEvent, Spr.gameObject);
                GameManager.GetPlayerAnimationComponent().Trigger_WolfPassBite();
            }

            GameManager.GetPlayerVoiceComponent().Play("PLAY_PLAYERDAMAGE", Il2CppVoice.Priority.Critical, PlayerVoice.Options.None);

            Transform V3 = GameManager.GetPlayerTransform();
            GameObject Player = GameManager.GetPlayerObject();

            GameAudioManager.SetMaterialSwitch("Flesh", Player);
            int num = (int)AkSoundEngine.PostEvent(Il2CppAK.EVENTS.PLAY_BULLETIMPACTS, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
            GameAudioManager.SetAudioSourceTransform(Player, V3);
        }

        public static void AddArrowAffiction(AfflictionBodyArea Area, string Case, string ArrowName)
        {
            string CaseHack = "_ARROW_";
            string GameplayString = "GAMEPLAY_ARROW";
            string Ico = "ico_ammo_arrow";

            if (ArrowName == "GEAR_ArrowHardened")
            {
                CaseHack = "_ARROW2_";
                GameplayString = "GAMEPLAY_ArrowHardened";
                Ico = "ico_ammo_arrowHardened";
            }

            BloodLoss bloodLossComponent = GameManager.GetBloodLossComponent();
            bloodLossComponent.m_Locations.Add((int)Area);
            bloodLossComponent.m_CausesLocIDs.Add(CaseHack);
            bloodLossComponent.m_ElapsedHoursList.Add(0.0f);
            bloodLossComponent.m_DurationHoursList.Add(UnityEngine.Random.Range(bloodLossComponent.m_DurationHoursMin, bloodLossComponent.m_DurationHoursMax));

            PlayerDamageEvent.SpawnDamageEvent(GameplayString, "GAMEPLAY_Affliction", Ico, InterfaceManager.m_FirstAidRedColor, true, 5f, 3f);

            GameManager.GetLogComponent().AddAffliction(AfflictionType.BloodLoss, CaseHack);
            GameManager.GetPlayerVoiceComponent().Play(bloodLossComponent.m_SoundToPlayBelowThreshold, Il2CppVoice.Priority.Critical);
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

        [HarmonyLib.HarmonyPatch(typeof(ConsoleManager), "CONSOLE_god")]
        private static class ConsoleManager_CONSOLE_god
        {
            private static void Postfix()
            {
                GameManager.GetPlayerMovementComponent().SetForceCrouch(false);
                ClientSend.SendEraceAllInjectedItems();
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(EmergencyStim), "ApplyEmergencyStim")]
        private static class EmergencyStim_ApplyEmergencyStim
        {
            private static void Postfix(EmergencyStim __instance)
            {
                if (GameManager.GetBrokenBody().HasAffliction)
                {
                    RevivedViaEmergencyStim();
                }
            }
        }

        public static GearItem GiveItemToPlayer(string GearName, int units = 1)
        {
            GameObject reference = AssetManager.GetAssetFromGame<GameObject>(GearName);
            if (reference)
            {
                GearItem gearItem = reference.GetComponent<GearItem>();
                if (gearItem)
                {
                    GearItem given = GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory(gearItem, units, 1f, InventoryInstantiateFlags.None);
                    if (given)
                    {
                        return given;
                    }
                    else
                    {
                        GameObject GearObj = UnityEngine.Object.Instantiate<GameObject>(reference);
                        if (GearObj)
                        {
                            given = GearObj.GetComponent<GearItem>();
                            if (given)
                            {
                                GameManager.GetPlayerManagerComponent().ProcessPickupItemInteraction(given, false, false, true);
                            }
                            else
                            {
                                SkyCoop.Logger.Log(ConsoleColor.Red, $"GiveItemToPlayer can't give {GearName}, it has no GearItem component. [Instantiated variant]");
                                UnityEngine.Object.Destroy(GearObj);
                            }
                        }
                        else
                        {
                            SkyCoop.Logger.Log(ConsoleColor.Red, $"GiveItemToPlayer can't give {GearName}, wasn't able to instantiate in inventory.");
                        }
                    }
                }
                else
                {
                    SkyCoop.Logger.Log(ConsoleColor.Red, $"GiveItemToPlayer can't give {GearName}, it has no GearItem component.");
                }
            }
            else
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"GiveItemToPlayer can't give {GearName} it not exist.");
            }
            return null;
        }

        public static void FullyCure()
        {
            GameManager.GetEmergencyStimComponent().ResetEmergencyStim();
            GameManager.GetDiminishedState().Cure();
            GameManager.GetPlayerMovementComponent().AddSprintStamina(GameManager.GetPlayerMovementComponent().DefaultMaxStamina);
            PlayersManager.m_LastDamageType = DataStr.DamageType.Unknown;
            PlayersManager.m_LastDamageZone = Comps.PlayerDamageColider.DamageZone.Chest;
            GameManager.GetBrokenBody().Cure();
            GameManager.GetConditionComponent().m_CurrentHP = GameManager.GetConditionComponent().GetAdjustedMaxHP();
            ConsoleManager.CONSOLE_afflictions_cure();
            GameManager.GetSprainedAnkleComponent().Cure();
            GameManager.GetSprainedWristComponent().Cure();
            GameManager.GetHeadacheComponent().Cure();

            GameManager.GetBloodLossComponent().Cure();
        }

        public static void RespawnAsSpecator()
        {
            GameManager.GetConditionComponent().m_NeverDie = false;
            s_Spectator = true;
            GameManager.GetPlayerMovementComponent().SetForceCrouch(false);
            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);

            ExitVehicleForced();
            FullyCure();

            MenuHook.RemovePleaseWait();
            InterfaceManager.TrySetPanelEnabled<Panel_LifeAfterDeath>(false);
        }

        public static void ExitVehicleForced()
        {
            if (GameManager.GetPlayerInVehicle().m_VehicleDoorUsed)
            {
                GameManager.GetPlayerInVehicle().m_VehicleDoorUsed = null;
                GameManager.GetPlayerInVehicle().ExitVehicleAfterFadeOut();
                ClientSend.SendVehicleSeat("");
            }
        }

        public static void RespawnOnPoint(Vector3 Position, Quaternion Rotation, bool RespawnAnim)
        {
            GameManager.GetConditionComponent().m_NeverDie = false;
            s_Spectator = false;

            GameManager.GetPlayerManagerComponent().TeleportPlayer(Position, Rotation);
            ExitVehicleForced();

            GameManager.GetPlayerMovementComponent().SetForceCrouch(false);
            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Normal);
            if (RespawnAnim)
            {
                FullyCure();
                GameManager.GetLifeAfterDeathManager().PlayRespawnTimeline();
            }
            GameManager.GetInventoryComponent().DestroyAllGear();
            GameManager.GetPlayerManagerComponent().m_StartGear.AddAllToInventory();

            ClientSend.SendRevived(-2);

            MenuHook.RemovePleaseWait();
            InterfaceManager.TrySetPanelEnabled<Panel_LifeAfterDeath>(false);
        }

        public static void RevivedViaEmergencyStim()
        {
            PlayersManager.m_LastDamageType = DataStr.DamageType.Unknown;
            PlayersManager.m_LastDamageZone = Comps.PlayerDamageColider.DamageZone.Chest;
            GameManager.GetBrokenBody().Cure();
            GameManager.GetPlayerMovementComponent().SetForceCrouch(false);
            GameManager.GetDiminishedState().Cure();
            if (ModMain.Client != null && ModMain.Client.m_MyEndPoint != null)
            {
                ClientSend.SendRevived(ModMain.Client.GetMyId());
            }
        }

        public static void ToKnockedState(DataStr.DamageType DeathCase = DataStr.DamageType.Unknown, Comps.PlayerDamageColider.DamageZone DamageZone = Comps.PlayerDamageColider.DamageZone.Chest)
        {
            PlayersManager.m_LastDamageType = DataStr.DamageType.Unknown;
            PlayersManager.m_LastDamageZone = Comps.PlayerDamageColider.DamageZone.Chest;
            GameManager.GetConditionComponent().m_CurrentHP = 30;
            //GameManager.GetBloodLossComponent().Cure();
            GameManager.GetBloodLossComponent().BloodLossStartOverrideArea(AfflictionBodyArea.Chest, "Knocked down", true, AfflictionOptions.PlayFX);
            GameManager.GetBrokenBody().ApplyBrokenBody(AfflictionOptions.None);
            GameManager.GetDiminishedState().Apply(2, AfflictionOptions.None);
            GameManager.GetPlayerMovementComponent().SetForceCrouch(true);

            ClientSend.SendDeath(DeathCase, true, DamageZone == Comps.PlayerDamageColider.DamageZone.Head);
            ClientSend.SendRevived(-1);
        }

        public static void Death(DataStr.DamageType DeathCase = DataStr.DamageType.Unknown, Comps.PlayerDamageColider.DamageZone DamageZone = Comps.PlayerDamageColider.DamageZone.Chest)
        {
            ClientSend.SendDeath(DeathCase, false, DamageZone == Comps.PlayerDamageColider.DamageZone.Head);
        }

        public static void EmptyFn()
        {

        }

        public static void HandleTryInteract(bool Result)
        {
            if (s_LastTryInteractionObject)
            {
                if (Result)
                {
                    VehicleDoor Door = s_LastTryInteractionObject.GetComponent<VehicleDoor>();
                    if (Door)
                    {
                        if (GameManager.GetPlayerInVehicle().IsInside())
                        {
                            GameManager.GetPlayerInVehicle().ExitVehicle(Door);
                        }
                        else
                        {
                            GameManager.GetPlayerInVehicle().EnterVehicle(Door);
                        }
                    }
                    Container Container = s_LastTryInteractionObject.GetComponent<Container>();
                    if (Container)
                    {
                        
                        MenuHook.DoPleaseWait("Please wait...", "Downloading container data...");
                        ClientSend.RequestContainerContent(Container.GetComponent<ObjectGuid>().Get());
                    }
                }
                else
                {
                    HUDMessage.AddMessage("Failed, interaction blocked by other player!", true, true);
                    Container Container = s_LastTryInteractionObject.GetComponent<Container>();
                    if (Container && !Container.m_PendingClose)
                    {
                        Container.BeginContainerClose();
                    }
                }
            }
            else
            {
                HUDMessage.AddMessage("Failed, interaction object no longer exist!", true, true);
            }
        }

        public static void DeactivateAllSpectatingTargets()
        {
            foreach (Comps.NetworkPlayer Player in s_Players)
            {
                if (Player && Player.m_CameraAttention)
                {
                    Player.m_CameraAttention.enabled = false; 
                }
            }
        }

        public static bool SetFirstValidSpectatingTarget(int StartIndex = 0, bool Reverse = false)
        {
            if (!Reverse)
            {
                for (int i = StartIndex; i < s_Players.Count; i++)
                {
                    Comps.NetworkPlayer Player = s_Players[i];
                    if (Player && Player.m_CameraAttention)
                    {
                        if (!Player.m_CameraAttention.enabled && Player.gameObject.activeSelf && Player.m_Action != Comps.NetworkPlayer.Actions.Death)
                        {
                            DeactivateAllSpectatingTargets();
                            HUDMessage.AddMessage($"YOU SPECTATING {Player.m_PlayerName}");
                            GameAudioManager.PlayGuiConfirm();
                            Player.m_CameraAttention.enabled = true;
                            return true;
                        }
                    }
                }
            }
            else
            {
                for (int i = StartIndex; i >= 0; i--)
                {
                    Comps.NetworkPlayer Player = s_Players[i];
                    if (Player && Player.m_CameraAttention)
                    {
                        if (!Player.m_CameraAttention.enabled && Player.gameObject.activeSelf && Player.m_Action != Comps.NetworkPlayer.Actions.Death)
                        {
                            DeactivateAllSpectatingTargets();
                            HUDMessage.AddMessage($"YOU SPECTATING {Player.m_PlayerName}");
                            GameAudioManager.PlayGuiConfirm();
                            Player.m_CameraAttention.enabled = true;
                            return true;
                        }
                    }
                }
            }
            

            return false;
        }

        public static int GetSpectatorIndex()
        {
            for (int i = 0; i < s_Players.Count; i++)
            {
                Comps.NetworkPlayer Player = s_Players[i];
                if (Player && Player.m_CameraAttention)
                {
                    if (Player.m_CameraAttention.enabled)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }

        public static void NextSpectatingTarget()
        {
            int CurrentSpectatingTarget = GetSpectatorIndex();

            bool Result = SetFirstValidSpectatingTarget(CurrentSpectatingTarget);
            if (!Result)
            {
                bool Result2 = SetFirstValidSpectatingTarget();
                if (!Result2)
                {
                    HUDMessage.AddMessage($"NO PLAYERS TO SPECTATE", true, true);
                    GameAudioManager.PlayGUIError();
                }
            }
        }

        public static void PreviousSpectatingTarget()
        {
            int CurrentSpectatingTarget = GetSpectatorIndex();

            bool Result = SetFirstValidSpectatingTarget(CurrentSpectatingTarget, true);
            if (!Result)
            {
                bool Result2 = SetFirstValidSpectatingTarget(s_Players.Count-1, true);
                if (!Result2)
                {
                    HUDMessage.AddMessage($"NO PLAYERS TO SPECTATE", true, true);
                    GameAudioManager.PlayGUIError();
                }
            }
        }

        public static void SpectatorControls()
        {
            if (s_Spectator)
            {
                if (InputManager.GetFirePressed(InputManager.m_CurrentContext))
                {
                    NextSpectatingTarget();
                }else if (InputManager.GetAltFirePressed(InputManager.m_CurrentContext))
                {
                    PreviousSpectatingTarget();
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(AfflictionButton), "SetCauseAndEffect")]
        private static class AfflictionButton_SetCauseAndEffect
        {
            private static void Postfix(AfflictionButton __instance, string causeStr, AfflictionType affType, AfflictionBodyArea location, int index, string effectName, string spriteName)
            {
                if (causeStr == "_ARROW_" || causeStr == "_ARROW2_")
                {
                    __instance.m_SpriteEffect.spriteName = "ico_ammo_arrow";
                    if(causeStr == "_ARROW_")
                    {
                        __instance.m_LabelEffect.text = Localization.Get("GAMEPLAY_Arrow");
                    }
                    else
                    {
                        __instance.m_LabelEffect.text = Localization.Get("GAMEPLAY_ArrowHardened");
                    }
                    __instance.m_LabelCause.text = "Player shot you";
                }

            }
        }

        [HarmonyLib.HarmonyPatch(typeof(AfflictionCoverflow), "SetAffliction")]
        private static class AfflictionCoverflow_SetAffliction
        {
            private static void Postfix(AfflictionCoverflow __instance, Affliction affliction)
            {
                if (affliction.m_Cause == "_ARROW_")
                {
                    __instance.m_SpriteEffect.spriteName = "ico_ammo_arrow";
                }
                else if (affliction.m_Cause == "_ARROW2_")
                {
                    __instance.m_SpriteEffect.spriteName = "ico_ammo_arrowHardened";
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Affliction), "UpdateSelectedAffliction", new Type[] { typeof(Affliction) })]
        private static class Panel_Affliction_UpdateSelectedAffliction
        {
            private static void Postfix(Panel_Affliction __instance, Affliction affliction)
            {
                if (affliction.m_Cause == "_ARROW_" || affliction.m_Cause == "_ARROW2_")
                {
                    if (affliction.m_Cause == "_ARROW_")
                    {
                        __instance.m_Label.text = Localization.Get("GAMEPLAY_Arrow");
                    }
                    else
                    {
                        __instance.m_Label.text = Localization.Get("GAMEPLAY_ArrowHardened");
                    }
                    __instance.m_LabelCause.text = "Player shot you";
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BloodLoss), "BloodLossEnd")]
        private static class BloodLoss_BloodLossEnd
        {
            private static bool Prefix(BloodLoss __instance, int index)
            {
                if (index < 0 || index >= __instance.m_CausesLocIDs.Count)
                    return true;


                string Cause = __instance.m_CausesLocIDs[index];
                int Location = __instance.m_Locations[index];

                if(Cause != "_ARROW_" && Cause != "_ARROW2_")
                {
                    return true;
                }

                string GearName = "GEAR_Arrow";
                string GamePlayString = "GAMEPLAY_Arrow";
                string Ico = "ico_ammo_arrow";

                if(Cause == "_ARROW2_")
                {
                    GearName = "GEAR_ArrowHardened";
                    GamePlayString = "GAMEPLAY_ArrowHardened";
                    Ico = "ico_ammo_arrowHardened";
                }

                __instance.m_CausesLocIDs.RemoveAt(index);
                __instance.m_Locations.RemoveAt(index);
                __instance.m_ElapsedHoursList.RemoveAt(index);
                __instance.m_DurationHoursList.RemoveAt(index);

                PlayerDamageEvent.SpawnAfflictionEvent(GamePlayString, "GAMEPLAY_Healed", Ico, InterfaceManager.m_FirstAidBuffColor);


                Panel_FirstAid panelFirstAid;
                if (InterfaceManager.TryGetPanel<Panel_FirstAid>(out panelFirstAid))
                    panelFirstAid.UpdateDueToAfflictionHealed();
                GameObject Reference = Addressables.LoadAssetAsync<GameObject>(GearName).WaitForCompletion();
                if (Reference)
                {
                    GameObject GearObject = GameObject.Instantiate<GameObject>(Reference, GameManager.GetPlayerTransform().position, GameManager.GetPlayerTransform().rotation);
                    GameManager.GetPlayerAnimationComponent().Trigger_WolfPassBite();
                    GameManager.GetPlayerVoiceComponent().Play(GameManager.GetFallDamageComponent().m_HeavyDamage, Il2CppVoice.Priority.Critical, PlayerVoice.Options.None, null);
                    GearObject.SetActive(true);
                    GearItem component = GearObject.GetComponent<GearItem>();
                    component.CompleteSpawnFromCONSOLE();
                    GameManager.GetPlayerManagerComponent().EnterInspectGearMode(component);
                    AfflictionBodyArea BodyArea = (AfflictionBodyArea)Location;
                    Comps.PlayerDamageColider.DamageZone DamageZone = DamageZone.Head;

                    switch (BodyArea)
                    {
                        case AfflictionBodyArea.Head:
                            DamageZone = DamageZone.Head;
                            break;
                        case AfflictionBodyArea.Chest:
                            DamageZone = DamageZone.Chest;
                            break;
                        case AfflictionBodyArea.ArmRight:
                            DamageZone = DamageZone.RightArm;
                            break;
                        case AfflictionBodyArea.ArmLeft:
                            DamageZone = DamageZone.LeftArm;
                            break;
                        case AfflictionBodyArea.LegRight:
                            DamageZone = DamageZone.RightLeg;
                            break;
                        case AfflictionBodyArea.LegLeft:
                            DamageZone = DamageZone.LeftLeg;
                            break;
                    }

                    if(ModMain.Client != null && ModMain.Client.m_MyEndPoint != null)
                    {
                        ClientSend.SendRemoveInjectedItem(ModMain.Client.GetMyId(), GearName, DamageZone);
                    }
                }
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(VehicleDoor), "Awake")]
        private static class VehicleDoor_Awake
        {
            private static void Postfix(VehicleDoor __instance)
            {
                ObjectGuid ObjGUID = __instance.GetComponent<ObjectGuid>();
                if (ObjGUID == null)
                {
                    ObjGUID = __instance.gameObject.AddComponent<ObjectGuid>();
                }
                PdidTable.RuntimeRegister(ObjGUID, ModMain.GenerateSeededGUID(0, __instance.m_ExitPoint.transform.position));
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(TimedHoldInteraction), "PerformHold")]
        private static class TimedHoldInteraction_PerformHold
        {
            private static bool Prefix(TimedHoldInteraction __instance)
            {
                VehicleDoor door = __instance.GetComponent<VehicleDoor>();
                if (door)
                {
                    string DoorGUID = "";
                    ObjectGuid ObjGUID = __instance.GetComponent<ObjectGuid>();
                    if (ObjGUID)
                    {
                        DoorGUID = ObjGUID.Get();
                    }
                    s_LastTryInteractionObject = __instance.gameObject;
                    SkyCoop.Logger.Log($"VehicleDoor PerformHold {DoorGUID}");

                    ClientSend.SendTryInteract(DoorGUID);
                    return false;
                }
                Container container = __instance.GetComponent<Container>();
                if (container && container.m_Inspected)
                {
                    string ContainerGUID = "";
                    ObjectGuid ObjGUID = __instance.GetComponent<ObjectGuid>();
                    if (ObjGUID)
                    {
                        ContainerGUID = ObjGUID.Get();
                    }
                    s_LastTryInteractionObject = __instance.gameObject;
                    SkyCoop.Logger.Log($"Container PerformHold {ContainerGUID}");

                    ClientSend.SendTryInteract(ContainerGUID);
                    return false;
                }
                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerInVehicle), "EnterVehicle")]
        private static class PlayerInVehicle_EnterVehicle
        {
            private static void Postfix(PlayerInVehicle __instance, VehicleDoor vd)
            {
                string DoorGUID = "";
                ObjectGuid ObjGUID = vd.GetComponent<ObjectGuid>();
                if (ObjGUID)
                {
                    DoorGUID = ObjGUID.Get();
                }
                SkyCoop.Logger.Log($"VehicleDoor EnterVehicle {DoorGUID}");
                ClientSend.SendVehicleSeat(DoorGUID);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerInVehicle), "ExitVehicle")]
        private static class PlayerInVehicle_ExitVehicle
        {
            private static void Postfix(PlayerInVehicle __instance, VehicleDoor vd)
            {
                string DoorGUID = "";
                ObjectGuid ObjGUID = vd.GetComponent<ObjectGuid>();
                if (ObjGUID)
                {
                    DoorGUID = ObjGUID.Get();
                }
                SkyCoop.Logger.Log($"VehicleDoor ExitVehicle {DoorGUID}");
                ClientSend.SendVehicleSeat("");
            }
        }
    }
}
