using Il2Cpp;
using OpenVoiceSharp;
using UnityEngine;

namespace SkyCoopClient
{
    public class ClientVoice
    {
        public int VoicePort;
        public bool VoiceConnected = false;
        public VoiceChatInterface VoiceInterface = new VoiceChatInterface(stereo:true, enableNoiseSuppression:false);
        public BasicMicrophoneRecorder MicrophoneRecorder = new BasicMicrophoneRecorder(stereo:true);
        public int BufferSamples = VoiceUtilities.GetSampleSize(2) / 2;
        public CircularAudioBuffer<float> VoiceBuffer;
        public AudioSource VoiceSource;


        public void Start()
        {
            SkyCoop.Logger.Log(ConsoleColor.Green, "Start Voice chat");
            MicrophoneRecorder.DataAvailable += (pcmData, length) =>
            {
                //if (!VoiceConnected)
                //    return;
                if (!VoiceInterface.IsSpeaking(pcmData))
                    return;

                (byte[] encodedData, int encodedLength) = VoiceInterface.SubmitAudioData(pcmData, length);

                //send voice to client
                SkyCoop.Logger.Log($"Voice length: {encodedLength}");

                (byte[] decodedData, int decodedLength) = VoiceInterface.WhenDataReceived(encodedData, encodedLength);
                float[] samples = new float[decodedLength / 2];
                VoiceUtilities.Convert16BitToFloat(decodedData, samples);
                VoiceBuffer.PushChunk(samples);
            };
            VoiceBuffer = new CircularAudioBuffer<float>(BufferSamples, RecommendedChunkAmount.Unity);

            GameObject obj = new GameObject();
            obj.name = "testvoice";
            SceneManager.DontDestroyOnLoad(obj);
            VoiceSource = obj.AddComponent<AudioSource>();
            AudioClip clip = AudioClip.Create(
                "Voice",
                VoiceBuffer.BufferLength,
                2,
                48000,
                false);
            VoiceSource.clip = clip;

            MicrophoneRecorder.StartRecording();
        }

        public void Update()
        {
            if (VoiceBuffer.BufferFull)
            {
                VoiceSource.clip.SetData(VoiceBuffer.ReadAllBuffer(), 0);
                VoiceSource.Play();
            }
            
        }
    }
}
