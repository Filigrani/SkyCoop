using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Il2CppRewired.ComponentControls.Data;
using ModSettings;
using MelonLoader;
using UnityEngine;
using System.Reflection;

namespace SkyCoopClient
{
    public class Settings : JsonModSettings
    {
        internal static Settings m_Options = new Settings();
        [Section("Voice Chat")]

        [Name("Push To Talk")]
        [Description("If enabled, your voice will be sent only when defined button is held.")]
        public bool m_PushToTalk = false;

        [Name("Push To Talk Button")]
        [Description("Configure key, that will be used to send your voice when it's held.\n(only used if Push to talk is enabled!)")]
        public KeyCode m_VoiceButton = KeyCode.V;

        [Name("Received Volume")]
        [Description("Volume of recived voice.")]
        [Slider(0, 5)]
        public float m_ReceivedVoiceVolume = 1;

        [Name("Microphone Volume")]
        [Description("Volume of your voice that going to be sent.")]
        [Slider(0, 5)]
        public float m_MicrophoneVoice = 1;

        [Name("Use Noise Suppression")]
        [Description("If enabled, your voice will be filtered from noise.")]
        public bool m_NoiseSuppression = true;

        [Name("Speaking Indicator")]
        [Description("Show icon when you sending voice.")]
        public bool m_DisplayMicrophoneIcon = true;

        public static void Init()
        {
            m_Options = new Settings();
            m_Options.RefreshGUI();
            m_Options.AddToModSettings("Sky Co-op: Reborn");
        }

        protected override void OnChange(FieldInfo field, object? oldValue, object? newValue)
        {
            base.OnChange(field, oldValue, newValue);
            ClientVoice.OnNoiseSuppressionChanged();
        }
    }
}
