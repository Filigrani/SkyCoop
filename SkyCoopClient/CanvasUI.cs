using Il2Cpp;
using Il2CppTMPro;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoopClient
{
    public class CanvasUI
    {
        public static GameObject m_UIPanel;
        public static Transform m_KillFeedTransform;

        private static Transform s_Parent;

        [HarmonyLib.HarmonyPatch(typeof(uConsole), "Start")]
        private static class uConsole_Start
        {
            private static void Postfix(uConsole __instance)
            {
                CreateUI(__instance.transform.GetChild(0));
            }
        }
        public static void CreateUI(Transform Parent)
        {
            s_Parent = Parent;
            GameObject UIReference = AssetManager.GetAssetFromBundle<GameObject>("SkyCoopUI");
            if(UIReference != null)
            {
                GameObject UIPanel = GameObject.Instantiate(UIReference, s_Parent);
                if (UIPanel)
                {
                    m_UIPanel = UIPanel;
                    m_KillFeedTransform = m_UIPanel.transform.GetChild(0);
                    SkyCoop.Logger.Log(ConsoleColor.Cyan, "Canvas UI created!");
                }
            }
            else
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, "Can't create UI!");
            }
        }

        public static string GetFontIcon(DataStr.DamageType DamageType)
        {
            return $"<sprite name=\"{DamageType.ToString()}\">";
        }
        public static string GetFontIcon(string IconName)
        {
            return $"<sprite name=\"{IconName}\">";
        }

        public static string GetPlayerName(int PlayerID)
        {
            return PlayersManager.GetPlayerName(PlayerID);
        }

        public static void AddKillFeedMessage(DataStr.KillFeedMessage Message)
        {
            string FinalString = "";

            if (Message.m_Flags.Contains(DataStr.KillFeedFlag.HelpedToDie) && !Message.m_Flags.Contains(DataStr.KillFeedFlag.Knocked))
            {
                FinalString = GetPlayerName(Message.m_Killer) + " helped " + GetPlayerName(Message.m_Victim) + " to finish himself.";
            }
            else
            {
                string ExtraPart = "";

                if (Message.m_Flags.Contains(DataStr.KillFeedFlag.Knocked))
                {
                    ExtraPart = " " + GetFontIcon("Knocked")+ " ";
                }

                if(Message.m_Killer != Message.m_Victim)
                {
                    if(Message.m_Assist == -1)
                    {
                        FinalString = GetPlayerName(Message.m_Killer) + " " + GetFontIcon(Message.m_DeathReason) + ExtraPart + " " + GetPlayerName(Message.m_Victim);
                    }
                    else
                    {
                        FinalString = GetPlayerName(Message.m_Killer) + " + " + GetPlayerName(Message.m_Killer) + " " + GetFontIcon(Message.m_DeathReason) + ExtraPart + " " + GetPlayerName(Message.m_Victim);
                    }
                }
                else
                {
                    FinalString = GetPlayerName(Message.m_Killer) + " " + GetFontIcon(Message.m_DeathReason) + ExtraPart + " " + GetPlayerName(Message.m_Victim);
                }
            }

            AddKillFeedMessage(FinalString);
        }

        public static void AddKillFeedMessage(string Text)
        {
            if (m_KillFeedTransform)
            {
                GameObject Reference = AssetManager.GetAssetFromBundle<GameObject>("KillFeedElement");
                if (Reference != null)
                {
                    GameObject Element = GameObject.Instantiate(Reference, m_KillFeedTransform);
                    if (Element)
                    {
                        Element.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(Text);
                    }
                }
                else
                {
                    SkyCoop.Logger.Log(ConsoleColor.Red, "Can't load KillFeedElement!");
                }
            }
            else
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, "m_KillFeedTransform is null!");
            }
        }
    }
}
