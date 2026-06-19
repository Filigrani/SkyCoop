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

        public static void RequestPresentLoot()
        {
            Panel_Inventory Panel = InterfaceManager.GetPanel<Panel_Inventory>();
            Panel.Enable(false);

            if (ModMain.Client.m_IsReady)
            {
                GearsSync.s_PlaceModeAfterPickup = false;
                ClientSend.SendRequestPresent();
            }
        }

        public static void OpenPresentFinished()
        {
            if (s_PresentOpenGear)
            {
                GameManager.GetPlayerManagerComponent().ConsumeUnitFromInventory(s_PresentOpenGear.gameObject);
                RequestPresentLoot();
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
