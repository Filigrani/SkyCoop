using Il2Cpp;
using LiteNetLib.Utils;
using LiteNetLib;
using OpenVoiceSharp;
using System.Net;
using SkyCoop;
using UnityEngine;
using SkyCoopServer;

namespace SkyCoopClient
{
    public class ClientVoice
    {
        public NetPacketProcessor m_PacketProcessor = new NetPacketProcessor();
        public EventBasedNetListener m_Listener;
        public NetManager m_Instance;
        public bool m_IsReady = false;
        public NetPeer m_HostEndPoint;

        public VoiceChatInterface VoiceInterface = new VoiceChatInterface(stereo: false, enableNoiseSuppression: Settings.m_Options.m_NoiseSuppression);
        public BasicMicrophoneRecorder MicrophoneRecorder = new BasicMicrophoneRecorder(stereo: false);
        public int BufferSamples = VoiceUtilities.GetSampleSize(1) / 2;
        public Dictionary<int, CircularAudioBuffer<float>> VoiceBuffer3D = new Dictionary<int, CircularAudioBuffer<float>>();
        public Dictionary<int, CircularAudioBuffer<float>> VoiceBuffer2D = new Dictionary<int, CircularAudioBuffer<float>>();
        public Dictionary<int, CircularAudioBuffer<float>> VoiceBufferRadio = new Dictionary<int, CircularAudioBuffer<float>>();
        public CircularAudioBuffer<float> m_AnnouncerBuffer = new CircularAudioBuffer<float>();

        public AudioSource m_AnnoncerAudioSource = null;

        public bool m_IsSpeakingFlag = false;

        private static bool s_LastPushToTalkState = false;
        private static float s_PushTotalReleaseTime = 0;

        public static void OnNoiseSuppressionChanged()
        {
            if (ModMain.ClientVoice != null)
            {
                ModMain.ClientVoice.VoiceInterface.EnableNoiseSuppression = Settings.m_Options.m_NoiseSuppression;
            }
        }

        public ClientVoice()
        {
            m_Listener = new EventBasedNetListener();
            m_Instance = new NetManager(m_Listener);

            m_AnnouncerBuffer = new CircularAudioBuffer<float>(BufferSamples, RecommendedChunkAmount.Unity);

            CreateAnnoncerAudioSource();
            StartRecording();


            m_Listener.NetworkErrorEvent += (fromPeer, error) =>
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, "Voice Connection failed: " + error);
                m_IsReady = false;
                m_Instance.Stop();
            };
            m_Listener.NetworkLatencyUpdateEvent += (peer, ping) =>
            {
                //SkyCoop.Logger.Log(ConsoleColor.Cyan, "Ping to voice server: " + ping);
            };

            m_Listener.PeerDisconnectedEvent += (peer, message) =>
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, "Disconnected: " + message.Reason);
                SkyCoop.Logger.Log(ConsoleColor.Red, message.AdditionalData);

                if (peer.RemoteId == 0)
                {
                    string Message = "Unknown reason";

                    if (message.Reason == DisconnectReason.RemoteConnectionClose)
                    {
                        //TODO: Print Host message
                    }
                    else
                    {
                        switch (message.Reason)
                        {
                            case DisconnectReason.ConnectionFailed:
                                Message = "Wasn't able to connect to the server.";
                                break;
                            case DisconnectReason.Timeout:
                                Message = "Disconnected doe timeout.";
                                break;
                            case DisconnectReason.HostUnreachable:
                                Message = "Server is unreachable.";
                                break;
                            case DisconnectReason.NetworkUnreachable:
                                Message = "Network is unreachable.";
                                break;
                            case DisconnectReason.RemoteConnectionClose:
                                break;
                            case DisconnectReason.DisconnectPeerCalled:
                                Message = "Disconnected by my request.";
                                break;
                            case DisconnectReason.ConnectionRejected:
                                break;
                            case DisconnectReason.InvalidProtocol:
                                Message = "Invalid connection protocol.";
                                break;
                            case DisconnectReason.UnknownHost:
                                Message = "Unknown host.";
                                break;
                            case DisconnectReason.Reconnect:
                                Message = "Reconnect";
                                break;
                            case DisconnectReason.PeerToPeerConnection:
                                Message = "Peer to Peer Connection";
                                break;
                            case DisconnectReason.PeerNotFound:
                                Message = "Peer not found.";
                                break;
                            default:
                                break;
                        }
                    }
                    m_IsReady = false;
                    m_Instance.Stop();
                }
            };

            m_Listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                m_HostEndPoint = fromPeer;
                switch(dataReader.GetInt())
                {
                    case 0:
                        ExecuteVoice(fromPeer, dataReader);
                        break;
                    case 1:
                        Welcome(dataReader);
                        break;
                    case 2:
                        ExecuteVoice(fromPeer, dataReader);
                        break;
                }
                

                dataReader.Recycle();
            };

            Task.Run(() =>
            {
                while (ModMain.Client.m_IsReady) 
                {
                    Update();
                }
            });
        }

        public void Connect(IPAddress Ip, int Port)
        {
            SkyCoop.Logger.Log(ConsoleColor.Cyan, "Going to connect to voice chat: " + Ip.ToString()+":"+ Port);
            m_Instance.Start();
            m_Instance.Connect(Ip.ToString(), Port, "voice");
        }

        public void ExecuteVoice(NetPeer Peer, NetDataReader Reader)
        {
            if(Settings.m_Options.m_ReceivedVoiceVolume == 0)
            {
                return;
            }
            
            int clientId = Reader.GetInt(); //id
            DataStr.PlayerHearing Hearing = (DataStr.PlayerHearing)Reader.GetInt();
            float VolumeScaler = Reader.GetFloat();
            byte[] Data = new byte[Reader.GetInt()];
            Reader.GetBytes(Data, Data.Length);

            (byte[] decodedData, int decodedLength) = VoiceInterface.WhenDataReceived(Data, Data.Length);
            float[] samples = new float[decodedLength / 2];
            VoiceUtilities.Convert16BitToFloat(decodedData, samples);

            //SkyCoop.Logger.Log("ExecuteVoice Recived " + samples.Length);

            switch (Hearing)
            {
                case DataStr.PlayerHearing.Proximity:
                    if (VoiceBuffer3D.ContainsKey(clientId))
                    {
                        CircularAudioBuffer<float> buffer = VoiceBuffer3D[clientId];
                        buffer.PushChunk(samples);
                        VoiceBuffer3D[clientId] = buffer;
                    }
                    else
                    {
                        CircularAudioBuffer<float> buffer = new CircularAudioBuffer<float>(BufferSamples, RecommendedChunkAmount.Unity);
                        buffer.PushChunk(samples);
                        VoiceBuffer3D.Add(clientId, buffer);
                    }
                    break;
                case DataStr.PlayerHearing.Global:
                    if (VoiceBuffer2D.ContainsKey(clientId))
                    {
                        CircularAudioBuffer<float> buffer = VoiceBuffer2D[clientId];
                        buffer.PushChunk(samples);
                        VoiceBuffer2D[clientId] = buffer;
                    }
                    else
                    {
                        CircularAudioBuffer<float> buffer = new CircularAudioBuffer<float>(BufferSamples, RecommendedChunkAmount.Unity);
                        buffer.PushChunk(samples);
                        VoiceBuffer2D.Add(clientId, buffer);
                    }
                    break;
                case DataStr.PlayerHearing.Radio:
                    if (VoiceBufferRadio.ContainsKey(clientId))
                    {
                        CircularAudioBuffer<float> buffer = VoiceBufferRadio[clientId];
                        buffer.PushChunk(samples);
                        VoiceBufferRadio[clientId] = buffer;
                    }
                    else
                    {
                        CircularAudioBuffer<float> buffer = new CircularAudioBuffer<float>(BufferSamples, RecommendedChunkAmount.Unity);
                        buffer.PushChunk(samples);
                        VoiceBufferRadio.Add(clientId, buffer);
                    }
                    break;
            }
        }

        public void ExecuteAnnoncerAudio(NetDataReader Reader)
        {
            byte[] Data = new byte[Reader.GetInt()];
            Reader.GetBytes(Data, Data.Length);

            (byte[] decodedData, int decodedLength) = VoiceInterface.WhenDataReceived(Data, Data.Length);
            float[] samples = new float[decodedLength / 2];
            VoiceUtilities.Convert16BitToFloat(decodedData, samples);
            m_AnnouncerBuffer.PushChunk(samples);
        }

        public void CreateAnnoncerAudioSource()
        {
            if(m_AnnoncerAudioSource == null)
            {
                GameObject Annon = new GameObject();
                m_AnnoncerAudioSource = Annon.AddComponent<AudioSource>();
                SceneManager.DontDestroyOnLoad(Annon);
            }
        }

        public void Welcome(NetDataReader dataReader)
        {
            SkyCoop.Logger.Log(ConsoleColor.Cyan, dataReader.GetString());
            m_IsReady = true;
        }

        public void SendToHost(NetDataWriter writer)
        {
            if (m_Instance != null)
            {
                m_HostEndPoint.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }

        public static bool IsSpeaking()
        {
            if(ModMain.ClientVoice == null)
            {
                return false;
            }
            return ModMain.ClientVoice.m_IsSpeakingFlag;
        }

        public static bool PushToTalkisHeldRaw()
        {
            return Input.GetKey(Settings.m_Options.m_VoiceButton);
        }

        public static bool PushToTalkIsHeld()
        {
            if (Input.GetKey(Settings.m_Options.m_VoiceButton))
            {
                s_LastPushToTalkState = true;
            }
            else
            {
                if (s_LastPushToTalkState)
                {
                    s_PushTotalReleaseTime = Time.time + 0.5f;
                }
                s_LastPushToTalkState = false;
            }

            if (s_LastPushToTalkState)
            {
                return true;
            }
            else
            {
                return s_PushTotalReleaseTime > Time.time;
            }
        }

        public void StartRecording()
        {
            SkyCoop.Logger.Log(ConsoleColor.Green, "Start Voice chat");

            MicrophoneRecorder.DataAvailable += (pcmData, length) =>
            {
                m_IsSpeakingFlag = false;
                if (!m_IsReady)
                {
                    return;
                }
                if (!VoiceInterface.IsSpeaking(pcmData))
                {
                    return;
                }      
                if(Settings.m_Options.m_PushToTalk && !PushToTalkIsHeld())
                {
                    return;
                }
                if(Settings.m_Options.m_MicrophoneVoice == 0)
                {
                    return;
                }
                m_IsSpeakingFlag = true;

                (byte[] encodedData, int encodedLength) = VoiceInterface.SubmitAudioData(pcmData, length);

                NetDataWriter writer = new NetDataWriter();
                writer.Put(0);
                writer.Put(ModMain.Client.m_MyEndPoint.RemoteId);
                writer.Put(encodedLength);
                writer.Put(encodedData);
                SendToHost(writer);
            };
            MicrophoneRecorder.StartRecording();
        }
        public void HandleVoiceBuffer(Dictionary<int, CircularAudioBuffer<float>> Buffer, DataStr.PlayerHearing AudioSourceType)
        {
            if (Buffer.Count > 0)
            {
                foreach (var clientBuffer in Buffer.ToArray())
                {
                    //SkyCoop.Logger.Log(ConsoleColor.Magenta, $"VoiceBuffer id:{clientBuffer.Key} has {clientBuffer.Value.ChunksAvailable} chunck");
                    if (clientBuffer.Value.ChunksAvailable > 0)
                    {
                        Comps.NetworkPlayer player = PlayersManager.GetPlayer(clientBuffer.Key);

                        AudioSource audioSource = null;

                        switch (AudioSourceType)
                        {
                            case DataStr.PlayerHearing.Proximity:
                                audioSource = player.m_AudioSource3D;
                                break;
                            case DataStr.PlayerHearing.Global:
                                audioSource = player.m_AudioSource2D;
                                break;
                            case DataStr.PlayerHearing.Radio:
                                audioSource = player.m_AudioSourceRadio;
                                break;
                        }
                        if (audioSource && !audioSource.isPlaying)
                        {
                            CircularAudioBuffer<float> buffer = clientBuffer.Value;
                            AudioClip clip = AudioClip.Create("Voice", buffer.BufferLength, 1, 48000, true, false);
                            float[] voice = buffer.ReadAllBuffer();
                            voice[voice.Length - 1] = 0f;
                            clip.SetData(voice, 0);
                            audioSource.PlayOneShot(clip, Settings.m_Options.m_ReceivedVoiceVolume);
                            player.SetVoiceSampleForAnimation(clip);
                            Buffer[clientBuffer.Key] = buffer;
                        }
                    }
                }
            }
        }

        public void Update()
        {
            m_Instance.PollEvents();

            if (m_IsReady)
            {

                HandleVoiceBuffer(VoiceBuffer3D, DataStr.PlayerHearing.Proximity);
                HandleVoiceBuffer(VoiceBuffer2D, DataStr.PlayerHearing.Global);
                HandleVoiceBuffer(VoiceBufferRadio, DataStr.PlayerHearing.Radio);

                if (m_AnnouncerBuffer.ChunksAvailable > 0)
                {
                    if (!m_AnnoncerAudioSource.isPlaying)
                    {
                        AudioClip clip = AudioClip.Create("Announce", m_AnnouncerBuffer.BufferLength, 1, 48000, true, false);
                        float[] voice = m_AnnouncerBuffer.ReadAllBuffer();
                        voice[voice.Length - 1] = 0f;
                        clip.SetData(voice, 0);
                        m_AnnoncerAudioSource.PlayOneShot(clip);
                    }
                }
            }
        }
    }
}
