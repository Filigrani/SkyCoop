using Il2Cpp;
using Il2CppEasyRoads3Dv3;
using Il2CppTMPro;
using MelonLoader;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace SkyCoopClient
{
    public class CanvasUI
    {
        public static GameObject m_UIPanel;
        public static Transform m_KillFeedTransform;
        public static Transform m_ChatContentTransform;
        public static Scrollbar m_ChatScrollBar;
        public static ScrollRect m_ChatScrollView;

        public static GameObject s_KillfeedRegularClone;
        public static GameObject s_KillfeedKillOrAssistClone;
        public static GameObject s_KillfeedDeadClone;
        public static GameObject s_ChatMessageClone;
        public static TMP_InputField s_ChatInputField;

        public static Animator s_ZoneDamageOverlay;
        public static GameObject s_DarkwalkerHUDClone;

        public static CanvasGroup s_SpeakingIndicator;

        private static Transform s_Parent;

        public static bool m_ChatIsOpen = false;


        public static bool s_TextChatKeyHeldPreviousFrame = false;

        [HarmonyLib.HarmonyPatch(typeof(uConsole), "Start")]
        private static class uConsole_Start
        {
            private static void Postfix(uConsole __instance)
            {
                CreateUI(__instance.transform.GetChild(0));
            }
        }

        public static void LoadKillFeedPrefabs()
        {
            GameObject Regular = AssetManager.GetAssetFromBundle<GameObject>("KillFeedElement");
            if (Regular)
            {
                s_KillfeedRegularClone = GameObject.Instantiate(Regular);
                SceneManager.DontDestroyOnLoad(s_KillfeedRegularClone);
                SkyCoop.Logger.Log(ConsoleColor.Cyan, "KillFeedElement loaded!");
            }
            else
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, "Can't load KillFeedElement!");
            }
            GameObject KillOrAssist = AssetManager.GetAssetFromBundle<GameObject>("KillFeedElementKill");
            if (KillOrAssist)
            {
                s_KillfeedKillOrAssistClone = GameObject.Instantiate(KillOrAssist);
                SceneManager.DontDestroyOnLoad(s_KillfeedKillOrAssistClone);
                SkyCoop.Logger.Log(ConsoleColor.Cyan, "KillFeedElementKill loaded!");
            }
            else
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, "Can't load KillFeedElementKill!");
            }
            GameObject Dead = AssetManager.GetAssetFromBundle<GameObject>("KillFeedElementDead");
            if (Dead)
            {
                s_KillfeedDeadClone = GameObject.Instantiate(Dead);
                SceneManager.DontDestroyOnLoad(s_KillfeedDeadClone);
                SkyCoop.Logger.Log(ConsoleColor.Cyan, "KillFeedElementDead loaded!");
            }
            else
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, "Can't load KillFeedElementDead!");
            }
            GameObject ChatMessage = AssetManager.GetAssetFromBundle<GameObject>("ChatMessagePrefab");
            if (ChatMessage)
            {
                s_ChatMessageClone = GameObject.Instantiate(ChatMessage);
                SceneManager.DontDestroyOnLoad(s_ChatMessageClone);
                SkyCoop.Logger.Log(ConsoleColor.Cyan, "ChatMessagePrefab loaded!");
            }
            else
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, "Can't load ChatMessagePrefab!");
            }
        }

        public static void DoZoneDamageOverlay()
        {
            if (s_ZoneDamageOverlay)
            {
                s_ZoneDamageOverlay.SetTrigger("Damage");
            }
        }

        public static void Update()
        {
            if (ModMain.IsGameplayScene())
            {
                bool DisplayIcon = Settings.m_Options.m_DisplayMicrophoneIcon && ModMain.ClientVoice != null && ModMain.ClientVoice.m_IsReady && ((Settings.m_Options.m_PushToTalk && ClientVoice.PushToTalkisHeldRaw()) || (!Settings.m_Options.m_PushToTalk && ClientVoice.IsSpeaking()));
                if (s_SpeakingIndicator)
                {
                    s_SpeakingIndicator.alpha = Mathf.Lerp(s_SpeakingIndicator.alpha, DisplayIcon ? 1 : 0, Time.deltaTime * 8);
                }

                if(GameManager.s_IsGameplaySuspended)

                if (uConsole.m_Instance && !uConsole.m_On)
                {
                    bool PressedThisFrame = Input.GetKey(KeyCode.Return);
                    if (s_TextChatKeyHeldPreviousFrame != PressedThisFrame)
                    {
                        if (PressedThisFrame)
                        {
                            ToggleChat();
                        }
                    }
                    s_TextChatKeyHeldPreviousFrame = PressedThisFrame;
                }
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

                    s_ZoneDamageOverlay = m_UIPanel.transform.GetChild(1).GetComponent<Animator>();

                    s_SpeakingIndicator = m_UIPanel.transform.GetChild(2).GetComponent<CanvasGroup>();
                    s_SpeakingIndicator.alpha = 0;

                    m_ChatContentTransform = m_UIPanel.transform.GetChild(3).GetChild(0).GetChild(0);
                    s_ChatInputField = m_UIPanel.transform.GetChild(3).GetChild(2).GetComponent<TMP_InputField>();
                    s_ChatInputField.gameObject.SetActive(false);

                    m_ChatScrollView = m_UIPanel.transform.GetChild(3).GetComponent<ScrollRect>();
                    m_ChatScrollBar = m_ChatScrollView.verticalScrollbar;

                    SkyCoop.Logger.Log(ConsoleColor.Cyan, "Canvas UI created!");
                }
            }
            else
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, "Can't create UI!");
            }
            LoadKillFeedPrefabs();
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

        public enum KillFeedType
        {
            Regular,
            Death,
            KillOrAssist,
        }

        public static void PlayCringe(string PrefabName)
        {
            GameObject SoundPlayerPrefab = AssetManager.GetAssetFromBundle<GameObject>(PrefabName);
            if (SoundPlayerPrefab)
            {
                GameObject SoundPlayer = GameObject.Instantiate(SoundPlayerPrefab);
                SoundPlayer.GetComponent<AudioSource>().Play();
                UnityEngine.Object.Destroy(SoundPlayer, 5);
            }
        }

        public static void AddTextMessage(string Text)
        {
            AddKillFeedMessage(Text, KillFeedType.Regular);
        }

        public static void AddJoinMessage(int PlayerID)
        {
            AddTextMessage($"{GetPlayerName(PlayerID)} {Localization.Get("GAMEPLAY_PlayerJoin")}");
        }
        public static void AddLeaveMessage(int PlayerID)
        {
            AddTextMessage($"{GetPlayerName(PlayerID)} {Localization.Get("GAMEPLAY_PlayerLeft")}");
        }

        public static void AddKillFeedMessage(DataStr.KillFeedMessage Message)
        {
            string FinalString = "";
            KillFeedType Type = KillFeedType.Regular;

            if (Message.m_Flags.Contains(DataStr.KillFeedFlag.HelpedToDie) && !Message.m_Flags.Contains(DataStr.KillFeedFlag.Knocked))
            {
                FinalString = GetPlayerName(Message.m_Killer) + " helped " + GetPlayerName(Message.m_Victim) + " to finish himself.";
            }
            else
            {
                string ExtraPart = "";

                if (Message.m_Flags.Contains(DataStr.KillFeedFlag.Knocked))
                {
                    ExtraPart = ExtraPart + " " + GetFontIcon("Knocked")+ " ";
                }
                if (Message.m_Flags.Contains(DataStr.KillFeedFlag.HeadShot))
                {
                    ExtraPart = ExtraPart + " " + GetFontIcon("HeadShot") + " ";
                    PlayCringe("KillFeedHeadShot");
                }
                if (Message.m_Assist == -1)
                {
                    FinalString = GetPlayerName(Message.m_Killer) + " " + GetFontIcon(Message.m_DeathReason) + ExtraPart + " " + GetPlayerName(Message.m_Victim);
                }
                else
                {
                    FinalString = GetPlayerName(Message.m_Killer) + " + " + GetPlayerName(Message.m_Assist) + " " + GetFontIcon(Message.m_DeathReason) + ExtraPart + " " + GetPlayerName(Message.m_Victim);
                }
            }

            if(Message.m_DeathReason == DataStr.DamageType.Hammer || Message.m_DeathReason == DataStr.DamageType.Knife || Message.m_DeathReason == DataStr.DamageType.Prybar || Message.m_DeathReason == DataStr.DamageType.Hatchet)
            {
                PlayCringe("KillFeedMelee");
            }

            if(ModMain.Client != null && ModMain.Client.m_MyEndPoint != null)
            {
                int MyID = ModMain.Client.m_MyEndPoint.RemoteId;
                if(MyID == Message.m_Victim)
                {
                    Type = KillFeedType.Death;
                }else if(MyID == Message.m_Killer || MyID == Message.m_Assist)
                {
                    Type = KillFeedType.KillOrAssist;
                }
            }

            AddKillFeedMessage(FinalString, Type);
        }

        public static void AddKillFeedMessage(string Text, KillFeedType Type)
        {
            if (m_KillFeedTransform)
            {
                GameObject Prefab = null;

                switch (Type)
                {
                    case KillFeedType.Regular:
                        Prefab = s_KillfeedRegularClone;
                        break;
                    case KillFeedType.Death:
                        Prefab = s_KillfeedDeadClone;
                        break;
                    case KillFeedType.KillOrAssist:
                        Prefab = s_KillfeedKillOrAssistClone;
                        break;
                    default:
                        break;
                }

                if (Prefab)
                {
                    GameObject Element = GameObject.Instantiate(Prefab, m_KillFeedTransform);
                    if (Element)
                    {
                        Element.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(Text);
                        Element.transform.GetChild(0).GetComponent<VerticalLayoutGroup>().enabled = false;
                        Element.transform.GetChild(0).GetComponent<VerticalLayoutGroup>().enabled = true;
                        Element.transform.GetChild(0).GetComponent<ContentSizeFitter>().enabled = false;
                        Element.transform.GetChild(0).GetComponent<ContentSizeFitter>().enabled = true;
                        Canvas.ForceUpdateCanvases();
                        UnityEngine.Object.Destroy(Element, 5.5f);
                    }
                    else
                    {
                        SkyCoop.Logger.Log(ConsoleColor.Red, "Instantiated element is null!");
                    }
                }
                else
                {
                    SkyCoop.Logger.Log(ConsoleColor.Red, "Prefab for KillFeed type " + Type.ToString() + " is null!");
                }
            }
            else
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, "m_KillFeedTransform is null!");
            }
        }

        public static void ToggleChat()
        {
            if (m_ChatIsOpen)
            {
                if (s_ChatInputField)
                {
                    if (!string.IsNullOrWhiteSpace(s_ChatInputField.text))
                    {
                        ClientSend.SendChatMessage(s_ChatInputField.text);
                    }
                    s_ChatInputField.text = "";
                }
            }
            m_ChatIsOpen = !m_ChatIsOpen;

            if (m_ChatScrollView)
            {
                if (m_ChatIsOpen)
                {
                    m_ChatScrollView.verticalScrollbar = m_ChatScrollBar;
                }
                else
                {
                    m_ChatScrollView.verticalScrollbar = null;
                }

                if (m_ChatScrollBar)
                {
                    m_ChatScrollBar.gameObject.SetActive(m_ChatIsOpen);
                }
            }

            if (s_ChatInputField)
            {
                s_ChatInputField.gameObject.SetActive(m_ChatIsOpen);

                if (m_ChatIsOpen)
                {
                    EventSystem.current.SetSelectedGameObject(s_ChatInputField.gameObject);
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
        }

        public static void HandleChatMessage(string Text, int From)
        {
            string Prefix = "";

            if(From != -1)
            {
                string Name = GetPlayerName(From);

                if (!string.IsNullOrEmpty(Name))
                {
                    Prefix = $"{Name}: ";
                }
            }

            AddChatMessage($"{Prefix}{Text}");
        }

        public static void AddChatMessage(string Text)
        {
            if (m_ChatContentTransform)
            {
                GameObject Element = GameObject.Instantiate(s_ChatMessageClone, m_ChatContentTransform);
                if (Element)
                {
                    Element.GetComponent<TextMeshProUGUI>().SetText(Text);
                    Element.GetComponent<ContentSizeFitter>().enabled = false;
                    Element.GetComponent<ContentSizeFitter>().enabled = true;

                    Comps.ChatMessage Comp = Element.AddComponent<Comps.ChatMessage>();
                    if (Comp)
                    {
                        Comp.m_VisibleTimer = 10;
                        Comp.m_Group = Comp.gameObject.GetComponent<CanvasGroup>();
                    }

                    Canvas.ForceUpdateCanvases();
                }
                else
                {
                    SkyCoop.Logger.Log(ConsoleColor.Red, "Instantiated chat message, prefab is null!");
                }

                if(m_ChatContentTransform.childCount > 32)
                {
                    UnityEngine.Object.Destroy(m_ChatContentTransform.GetChild(0).gameObject);
                }

                if (m_ChatScrollView)
                {
                    m_ChatScrollView.SetNormalizedPosition(0, 1);
                }
            }
        }
    }
}
