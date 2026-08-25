using Il2Cpp;
using Il2CppRewired.ComponentControls.Data;
using Il2CppTLD.UI;
using MelonLoader;
using ModSettings;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Analytics.Internal;
using UnityEngine;

namespace SkyCoopClient
{
    public class Settings : JsonModSettings
    {
        internal static Settings m_Options = new Settings();

        //[Section("Generic Settings")]

        //[Name("User Name")]
        //[Description("Nickname other players will see. Leave empty to use your name from Steam.")]
        //public string m_UserName = "";

        [Section("SC_Settings_VoiceChatSection", Localize = true)]

        [Name("SC_Settings_Microphone", Localize = true)]
        [Description("SC_Settings_MicrophoneDescription")]
        [Choice(
            "MODINTERNAL_Microphone0",
            "MODINTERNAL_Microphone1",
            "MODINTERNAL_Microphone2",
            "MODINTERNAL_Microphone3",
            "MODINTERNAL_Microphone4",
            "MODINTERNAL_Microphone5",
            "MODINTERNAL_Microphone6",
            "MODINTERNAL_Microphone7",
            "MODINTERNAL_Microphone8",
            "MODINTERNAL_Microphone9"
            , Localize = true)]
        public int m_MicrophoneDeviceNumber = 0;

        [Name("SC_Settings_PushToTalk", Localize = true)]
        [Description("SC_Settings_PushToTalkDescription")]
        public bool m_PushToTalk = false;

        [Name("SC_Settings_PushToTalkKey", Localize = true)]
        [Description("SC_Settings_PushToTalkKeyDescription")]
        public KeyCode m_VoiceButton = KeyCode.V;

        [Name("SC_Settings_ReceivedVoice", Localize = true)]
        [Description("SC_Settings_ReceivedVoiceDescription")]
        [Slider(0, 5)]
        public float m_ReceivedVoiceVolume = 1;

        [Name("SC_Settings_MicrophoneVolume", Localize = true)]
        [Description("SC_Settings_MicrophoneVolumeDescription")]
        [Slider(0, 5)]
        public float m_MicrophoneVoice = 1;

        [Name("SC_Settings_NoiseSuppression", Localize = true)]
        [Description("SC_Settings_NoiseSuppressionDescription")]
        public bool m_NoiseSuppression = true;

        [Name("SC_Settings_SpeakingIndicator", Localize = true)]
        [Description("SC_Settings_SpeakingIndicatorDescription")]
        public bool m_DisplayMicrophoneIcon = true;

        [Section("SC_Settings_GeneralSettingsSection", Localize = true)]

        [Name("SC_Settings_MeleeWeaponKey", Localize = true)]
        [Description("SC_Settings_MeleeWeaponKeyDescription")]
        public KeyCode m_MeleeButton = KeyCode.X;

        [Name("SC_Settings_MeleeOverDecoy", Localize = true)]
        [Description("SC_Settings_MeleeOverDecoyDescription")]
        public bool m_MeleeOverDecoy = true;

        [Name("SC_Settings_CookingPassTime", Localize = true)]
        [Description("SC_Settings_CookingPassTimeDescription")]
        public bool m_CookingPassTime = true;

        //[Name("Max players")]
        //[Description("How many players can connect to the server.")]
        //[Slider(2, 32)]
        //public int m_ServerSetting_MaxPlayers = 4;

        // Rest of the settings only when modsetting updated to have strings fields.

        public static void Init()
        {
            m_Options = new Settings();
            m_Options.RefreshGUI();
            m_Options.AddToModSettings("Sky Co-op: Reborn");
            ToggleSettingsMode(false);
        }

        public static void ToggleSettingsMode(bool ServerSettings = false)
        {
            m_Options.SetFieldVisible("m_MicrophoneDeviceNumber", !ServerSettings);
            m_Options.SetFieldVisible("m_PushToTalk", !ServerSettings);
            m_Options.SetFieldVisible("m_VoiceButton", !ServerSettings);
            m_Options.SetFieldVisible("m_ReceivedVoiceVolume", !ServerSettings);
            m_Options.SetFieldVisible("m_MicrophoneVoice", !ServerSettings);
            m_Options.SetFieldVisible("m_NoiseSuppression", !ServerSettings);
            m_Options.SetFieldVisible("m_DisplayMicrophoneIcon", !ServerSettings);

            //m_Options.SetFieldVisible("m_ServerSetting_MaxPlayers", ServerSettings);

            m_Options.RefreshGUI();
        }

        protected override void OnChange(FieldInfo field, object? oldValue, object? newValue)
        {
            base.OnChange(field, oldValue, newValue);

            ClientVoice.OnMicrophoneChanged(m_MicrophoneDeviceNumber);
            ClientVoice.OnNoiseSuppressionChanged();
        }

        public static void BackFromForcedMenu()
        {
            Panel_OptionsMenu Options = InterfaceManager.GetPanel<Panel_OptionsMenu>();

            if (Options)
            {
                Transform Pages = Options.transform.FindChild("Pages");
                if (Pages)
                {
                    Transform ModSettings = Pages.FindChild("ModSettings");

                    ConsoleComboBox box = ModSettings.GetChild(1).GetChild(0).GetComponent<ConsoleComboBox>();
                    Transform SubMenuDisplay = ModSettings.GetChild(0);
                    if (SubMenuDisplay)
                    {
                        SubMenuDisplay.gameObject.SetActive(true);
                    }

                    if (box)
                    {
                        box.gameObject.SetActive(true);
                    }
                }
            }
        }

        public static void ForceToShow(bool ServerSettings = false)
        {
            Panel_OptionsMenu Options = InterfaceManager.GetPanel<Panel_OptionsMenu>();


            ToggleSettingsMode(ServerSettings);

            if (Options)
            {
                Options.m_MainTab.SetActive(false);

                Transform Pages = Options.transform.FindChild("Pages");
                if (Pages)
                {
                    Transform ModSettings = Pages.FindChild("ModSettings");

                    ModSettings.gameObject.SetActive(true);

                    ConsoleComboBox box = ModSettings.GetChild(1).GetChild(0).GetComponent<ConsoleComboBox>();
                    Transform SubMenuDisplay = ModSettings.GetChild(0);
                    if (SubMenuDisplay)
                    {
                        SubMenuDisplay.gameObject.SetActive(false);
                    }

                    if (box)
                    {
                        box.m_CurrentIndex = box.items.IndexOf("Sky Co-op: Reborn");
                        box.m_SelectedItem = box.items[box.m_CurrentIndex];
                        box.Refresh();
                        if (EventDelegate.IsValid(box.onChange))
                        {
                            EventDelegate.Execute(box.onChange);
                        }
                        box.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public static class SettingsHooks
    {
        [HarmonyLib.HarmonyPatch(typeof(Localization), "Get")]
        private static class Localization_Get
        {
            private static void Postfix(string key, ref string __result)
            {
                if (!key.StartsWith("MODINTERNAL_"))
                {
                    return;
                }

                int Num = int.Parse(key.Replace("MODINTERNAL_Microphone", ""));

                __result = ClientVoice.GetMicrophoneName(Num);
            }
        }
    }
}
