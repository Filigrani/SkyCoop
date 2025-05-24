using Il2Cpp;
using LiteNetLib.Utils;
using LiteNetLib;
using OpenVoiceSharp;
using System.Net;
using SkyCoop;
using UnityEngine;
using Il2CppRewired;

namespace SkyCoopClient
{
    public class ClientVoice
    {
        public NetPacketProcessor m_PacketProcessor = new NetPacketProcessor();
        public EventBasedNetListener m_Listener;
        public NetManager m_Instance;
        public bool m_IsReady = false;
        public NetPeer m_HostEndPoint;

        public VoiceChatInterface VoiceInterface = new VoiceChatInterface(stereo: false, enableNoiseSuppression: false);
        public BasicMicrophoneRecorder MicrophoneRecorder = new BasicMicrophoneRecorder(stereo: false);
        public int BufferSamples = VoiceUtilities.GetSampleSize(1) / 2;
        public Dictionary<int, CircularAudioBuffer<float>> VoiceBuffer = new Dictionary<int, CircularAudioBuffer<float>>();
        public CircularAudioBuffer<float> m_AnnouncerBuffer = new CircularAudioBuffer<float>();

        public AudioSource m_AnnoncerAudioSource = null;

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
            int clientId = Reader.GetInt(); //id
            byte[] Data = new byte[Reader.GetInt()];
            Reader.GetBytes(Data, Data.Length);

            (byte[] decodedData, int decodedLength) = VoiceInterface.WhenDataReceived(Data, Data.Length);
            float[] samples = new float[decodedLength / 2];
            VoiceUtilities.Convert16BitToFloat(decodedData, samples);

            //SkyCoop.Logger.Log("ExecuteVoice Recived " + samples.Length);

            if (VoiceBuffer.ContainsKey(clientId)) 
            {
                CircularAudioBuffer<float> buffer = VoiceBuffer[clientId];
                buffer.PushChunk(samples);
                VoiceBuffer[clientId] = buffer;
            }
            else
            {
                CircularAudioBuffer<float> buffer = new CircularAudioBuffer<float>(BufferSamples, RecommendedChunkAmount.Unity);
                buffer.PushChunk(samples);
                VoiceBuffer.Add(clientId, buffer);
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

        public void StartRecording()
        {
            SkyCoop.Logger.Log(ConsoleColor.Green, "Start Voice chat");

            MicrophoneRecorder.DataAvailable += (pcmData, length) =>
            {
                if (!m_IsReady)
                    return;
                if (!VoiceInterface.IsSpeaking(pcmData))
                    return;

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

        public void Update()
        {
            m_Instance.PollEvents();

            if (m_IsReady)
            {
                if (VoiceBuffer.Count > 0)
                {
                    foreach (var clientBuffer in VoiceBuffer.ToList())
                    {
                        //SkyCoop.Logger.Log(ConsoleColor.Magenta, $"VoiceBuffer id:{clientBuffer.Key} has {clientBuffer.Value.ChunksAvailable} chunck");
                        if (clientBuffer.Value.ChunksAvailable > 0)
                        {
                            Comps.NetworkPlayer player = PlayersManager.GetPlayer(clientBuffer.Key);
                            if (!player.m_AudioSource3D.isPlaying)
                            {
                                CircularAudioBuffer<float> buffer = clientBuffer.Value;
                                AudioClip clip = AudioClip.Create("Voice", buffer.BufferLength, 1, 48000, true, false);
                                float[] voice = buffer.ReadAllBuffer();
                                voice[voice.Length - 1] = 0f;
                                clip.SetData(voice, 0);
                                player.m_AudioSource3D.PlayOneShot(clip, 3);
                                player.SetVoiceSampleForAnimation(clip);
                                VoiceBuffer[clientBuffer.Key] = buffer;
                            }
                        }
                    }
                }

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
