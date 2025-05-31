using Il2Cpp;
using Il2CppEasyRoads3Dv3;
using Il2CppTMPro;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SkyCoopClient
{
    public class CanvasUI
    {
        public static GameObject m_UIPanel;
        public static Transform m_KillFeedTransform;
        public static GameObject m_SpawnPointEditor;
        public static Transform m_SpawnPointEditorScrollParnet;
        public static GameObject m_PropsEditor;
        public static Transform m_PropsEditorScrollParnet;

        public static GameObject s_KillfeedRegularClone;
        public static GameObject s_KillfeedKillOrAssistClone;
        public static GameObject s_KillfeedDeadClone;

        public static Animator s_ZoneDamageOverlay;
        public static GameObject s_DarkwalkerHUDClone;

        public static TMP_InputField s_PropsEditorPrefabName;
        public static Toggle s_PropsEditorIsFromBundle;

        private static Transform s_Parent;


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
        }

        public static void DoZoneDamageOverlay()
        {
            if (s_ZoneDamageOverlay)
            {
                s_ZoneDamageOverlay.SetTrigger("Damage");
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
                    m_SpawnPointEditor = m_UIPanel.transform.GetChild(1).gameObject;

                    m_PropsEditor = m_UIPanel.transform.GetChild(3).gameObject;

                    Action act = new Action(() => SpawnPointEditor.Save());
                    Action act2 = new Action(() => SpawnPointEditor.LoadCurrentSceneFile());
                    m_UIPanel.transform.GetChild(1).GetChild(2).GetComponent<Button>().onClick.AddListener(act);
                    m_UIPanel.transform.GetChild(1).GetChild(3).GetComponent<Button>().onClick.AddListener(act2);
                    m_SpawnPointEditorScrollParnet = m_UIPanel.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0);
                    m_PropsEditorScrollParnet = m_UIPanel.transform.GetChild(3).GetChild(1).GetChild(0).GetChild(0);

                    s_ZoneDamageOverlay = m_UIPanel.transform.GetChild(2).GetComponent<Animator>();

                    Action act3 = new Action(() => PropsSpawnsEditor.Save());
                    Action act4 = new Action(() => PropsSpawnsEditor.LoadCurrentSceneFile());

                    m_UIPanel.transform.GetChild(3).GetChild(2).GetComponent<Button>().onClick.AddListener(act3);
                    m_UIPanel.transform.GetChild(3).GetChild(3).GetComponent<Button>().onClick.AddListener(act4);

                    s_PropsEditorPrefabName = m_UIPanel.transform.GetChild(3).GetChild(4).GetComponent<TMP_InputField>();
                    s_PropsEditorIsFromBundle = m_UIPanel.transform.GetChild(3).GetChild(5).GetComponent<Toggle>();

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
            AddTextMessage($"{GetPlayerName(PlayerID)} join");
        }
        public static void AddLeaveMessage(int PlayerID)
        {
            AddTextMessage($"{GetPlayerName(PlayerID)} leave");
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
    }
}
