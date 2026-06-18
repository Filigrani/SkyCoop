using Il2Cpp;
using Il2CppAK;
using Il2CppVoice;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static Il2CppSystem.Linq.Expressions.Interpreter.InitializeLocalInstruction;

namespace SkyCoopClient
{
    public static class PresentsGear
    {
        public static GearItem s_PresentOpenGear = null;
        public enum PresentRarity
        {
            Shit,
            Low,
            Medium,
            High,
            Epic,
            Legend,
        }

        public static List<string> ShitTier = new List<string>()
        {
            "GEAR_Stick",
            "GEAR_Tinder",
            "GEAR_Stone",
            "GEAR_Cloth",
            "GEAR_NewsprintRoll",
            "GEAR_PaperStack",
            "GEAR_ReclaimedWoodB",
            "GEAR_LeatherShoes",
            "GEAR_BasicGloves",
            "GEAR_TeeShirt",
            "GEAR_BaseballCap",
            "GEAR_RecycledCan",
            "GEAR_SkiBoots",
        };
        public static List<string> LowTier = new List<string>()
        {
            "GEAR_CarBattery",
            "GEAR_DustingSulfur",
            "GEAR_ScrapLead",
            "GEAR_ScrapMetal",
            "GEAR_StumpRemover",
            "GEAR_Rope",
            "GEAR_Snare",
            "GEAR_Water500ml",
            "GEAR_DogFood",
            "GEAR_EnergyBar",
            "GEAR_KetchupChips",
            "GEAR_CottonHoodie",
            "GEAR_Leather",
            "GEAR_CottonSocks",
            "GEAR_MackinawJacket",
            "GEAR_BasicWinterCoat",
            "GEAR_CanOpener",
            "GEAR_SimpleTools",
            "GEAR_HeavyBandage",
            "GEAR_BottlePainKillers",
            "GEAR_BottleHydrogenPeroxide",
            "GEAR_BottleAntibiotics",
            "GEAR_RoseHipTea",
            "GEAR_ReishiTea",
            "GEAR_WaterPurificationTablets",
            "GEAR_BasicWoolHat",
            "GEAR_FleeceMittens",
            "GEAR_CoffeeCup",
            "GEAR_Jeans",
            "GEAR_BasicWoolScarf",
            "GEAR_LongUnderwear",
        };
        public static List<string> MediumTier = new List<string>()
        {
            "GEAR_GreenTeaPackage",
            "GEAR_PeanutButter",
            "GEAR_Peaches",
            "GEAR_Soda",
            "GEAR_SodaEnergy",
            "GEAR_SodaGrape",
            "GEAR_SodaOrange",
            "GEAR_TomatoSoupCan",
            "GEAR_SewingKit",
            "GEAR_LampFuel",
            "GEAR_JerryCanRusty",
            "GEAR_CoffeeTin",
            "GEAR_BirchbarkPrepared",
            "GEAR_Firestriker",
            "GEAR_Mittens",
            "GEAR_WorkGloves",
            "GEAR_DownSkiJacket",
            "GEAR_WoolSocks",
            "GEAR_HighQualityTools",
            "GEAR_CookingPot",
            "GEAR_Knife",
            "GEAR_FlareA",
            "GEAR_BlueFlare",
            "GEAR_BirchbarkTea",
            "GEAR_Toque",
            "GEAR_CargoPants",
            "GEAR_InsulatedPants",
            "GEAR_WoolWrap",
            "GEAR_FishermanSweater",
        };
        public static List<string> HighTier = new List<string>()
        {
            "GEAR_QualityWinterCoat",
            "GEAR_PremiumWinterCoat",
            "GEAR_MilitaryParka",
            "GEAR_HeavyParka",
            "GEAR_Arrow",
            "GEAR_RifleAmmoSingle",
            "GEAR_RevolverAmmoSingle",
            "GEAR_GunpowderCan",
            "GEAR_KeroseneLampB",
            "GEAR_Hacksaw",
            "GEAR_Hatchet",
            "GEAR_HomeMadeSoup",
            "GEAR_ClimbingSocks",
            "GEAR_Gauntlets",
            "GEAR_MRE",
            "GEAR_CombatPants",
            "GEAR_Balaclava",
        };
        public static List<string> EpicTier = new List<string>()
        {
            "GEAR_RevolverAmmoBox",
            "GEAR_RifleAmmoBox",
            "GEAR_FlareGunAmmoSingle",
            "GEAR_EmergencyStim",
            "GEAR_RabbitSkinMittens",
            "GEAR_RabbitskinHat",
            "GEAR_BearHide",
            "GEAR_MooseHide",
            "GEAR_SCHeatPackB",
        };
        public static List<string> LegendTier = new List<string>()
        {
            "GEAR_Bow",
            "GEAR_Revolver",
            "GEAR_Rifle",
            "GEAR_BearSkinCoat",
            "GEAR_MooseHideCloak",
            "GEAR_WolfSkinCape",
            "GEAR_EarMuffs",
            "GEAR_MooseHideBag",
            "GEAR_LongUnderwearWool",
        };


        public static void SpawnAndTakeGiftGear()
        {
            Panel_Inventory Panel = InterfaceManager.GetPanel<Panel_Inventory>();



            Panel.Enable(false);
            PresentRarity Rarity = PresentRarity.Shit;
            System.Random random = new System.Random();
            string GearName = "GEAR_Stone";

            int Rolled = random.Next(0, 100);
            if (Rolled <= 5)
            {
                Rarity = PresentRarity.Legend;
                GearName = LegendTier[random.Next(0, LegendTier.Count)];
            }
            else if (Rolled <= 13)
            {
                Rarity = PresentRarity.Epic;
                GearName = EpicTier[random.Next(0, EpicTier.Count)];
            }
            else if (Rolled <= 16)
            {
                Rarity = PresentRarity.High;
                GearName = HighTier[random.Next(0, HighTier.Count)];
            }
            else if (Rolled <= 20)
            {
                Rarity = PresentRarity.Medium;
                GearName = MediumTier[random.Next(0, MediumTier.Count)];
            }
            else if (Rolled <= 22)
            {
                Rarity = PresentRarity.Low;
                GearName = LowTier[random.Next(0, LowTier.Count)];
            }
            else
            {
                Rarity = PresentRarity.Shit;
                GearName = ShitTier[random.Next(0, ShitTier.Count)];
            }

           

            GameObject reference = AssetManager.GetAssetFromGame<GameObject>(GearName);
            if (reference)
            {
                GearItem itemReference = reference.GetComponent<GearItem>();
                if (itemReference)
                {
                    GearsSync.s_NoSyncFlag = true;
                    GearItem Item = GameManager.GetPlayerManagerComponent().InstantiateItemAtPlayersFeet(itemReference, 1);
                    GearsSync.s_NoSyncFlag = false;
                    if (Item)
                    {
                        GameManager.GetPlayerManagerComponent().EnterInspectGearMode(Item);
                    }
                }
            }


            if (Rarity == PresentRarity.Shit)
            {
                AkSoundEngine.SetSwitch(SWITCHES.URGENCY.GROUP, SWITCHES.URGENCY.SWITCH.HIGH, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
                GameManager.GetPlayerVoiceComponent().Play("PLAY_FIREFAIL", Il2CppVoice.Priority.Critical);
            }
            else if (Rarity == PresentRarity.Low)
            {
                AkSoundEngine.SetSwitch(SWITCHES.URGENCY.GROUP, SWITCHES.URGENCY.SWITCH.LOW, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
                GameManager.GetPlayerVoiceComponent().Play("PLAY_VOINSPECTOBJECT", Il2CppVoice.Priority.Critical);
            }
            else if (Rarity == PresentRarity.Medium)
            {
                AkSoundEngine.SetSwitch(SWITCHES.URGENCY.GROUP, SWITCHES.URGENCY.SWITCH.MED, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
                GameManager.GetPlayerVoiceComponent().Play("PLAY_VOINSPECTOBJECT", Il2CppVoice.Priority.Critical);
            }
            else if (Rarity == PresentRarity.High)
            {
                AkSoundEngine.SetSwitch(SWITCHES.URGENCY.GROUP, SWITCHES.URGENCY.SWITCH.MED, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
                GameManager.GetPlayerVoiceComponent().Play("PLAY_VOINSPECTOBJECT", Il2CppVoice.Priority.Critical);
            }
            else if (Rarity == PresentRarity.Epic)
            {
                AkSoundEngine.SetSwitch(SWITCHES.URGENCY.GROUP, SWITCHES.URGENCY.SWITCH.HIGH, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
                GameManager.GetPlayerVoiceComponent().Play("PLAY_VOINSPECTOBJECT", Il2CppVoice.Priority.Critical);
            }
            else if (Rarity == PresentRarity.Legend)
            {
                GameManager.GetPlayerVoiceComponent().Play("PLAY_ENTITYDEATHVO", Il2CppVoice.Priority.Critical);
            }
        }

        public static void OpenPresentFinished()
        {
            if (s_PresentOpenGear)
            {
                GameManager.GetPlayerManagerComponent().ConsumeUnitFromInventory(s_PresentOpenGear.gameObject);
                SpawnAndTakeGiftGear();
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "UseInventoryItem", new System.Type[] { typeof(GearItem), typeof(bool)})]
        private static class PlayerManager_UseInventoryItem
        {
            private static void Postfix(PlayerManager __instance, GearItem gi)
            {
                if (gi)
                {
                    SkyCoop.Logger.Log(ConsoleColor.Magenta, $"[UseInventoryItem] {gi.name}");
                    if (gi.name == "GEAR_SCPresent")
                    {
                        Panel_GenericProgressBar PanelBar = InterfaceManager.GetPanel<Panel_GenericProgressBar>();
                        if (PanelBar)
                        {
                            s_PresentOpenGear = gi;
                            PanelBar.Launch(Localization.Get("GAMEPLAY_OpeningProgress"), 3f, 0.0f, 0.0f, "Play_HarvestingCardboard", null, true, true, null);
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_GenericProgressBar), "ProgressBarEnded")]
        internal static class Panel_GenericProgressBar_ProgressBarEnded
        {
            private static void Postfix(Panel_GenericProgressBar __instance, bool success, bool playerCancel)
            {
                if (s_PresentOpenGear && success)
                {
                    OpenPresentFinished();
                }
            }
        }
    }
}
