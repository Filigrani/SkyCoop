using Il2Cpp;
using LiteNetLib.Utils;
using LiteNetLib;
using OpenVoiceSharp;
using System.Net;
using SkyCoop;
using UnityEngine;

namespace SkyCoopClient
{
    public class ClientVoice
    {
        public ClientVoice()
        {
            m_Listener = new EventBasedNetListener();
            m_Instance = new NetManager(m_Listener);
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
                m_IsReady = true;
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
                ExecuteVoice(fromPeer, dataReader);

                dataReader.Recycle();
            };
        }


        public NetPacketProcessor m_PacketProcessor = new NetPacketProcessor();
        public EventBasedNetListener m_Listener;
        public NetManager m_Instance;
        public bool m_IsReady = false;
        public NetPeer m_HostEndPoint;


        public VoiceChatInterface VoiceInterface = new VoiceChatInterface(stereo:true, enableNoiseSuppression:false);
        public BasicMicrophoneRecorder MicrophoneRecorder = new BasicMicrophoneRecorder(stereo:true);
        public int BufferSamples = VoiceUtilities.GetSampleSize(2) / 2;
        public CircularAudioBuffer<float> VoiceBuffer;
        public AudioSource VoiceSource;

        public void Connect(IPAddress Ip, int Port)
        {
            SkyCoop.Logger.Log(ConsoleColor.Cyan, "Going to connect to voice chat: " + Ip.ToString()+":"+ Port);
            m_Instance.Start();
            m_Instance.Connect(Ip.ToString(), Port, "voice");
            DefineGlobalAudio();
        }

        public void DefineGlobalAudio()
        {
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
        }

        public void ExecuteVoice(NetPeer Peer, NetDataReader Reader)
        {
            byte[] Data = new byte[Reader.GetInt()];

            SkyCoop.Logger.Log("Got voice sample " + Data.Length);
            Reader.GetBytes(Data, Data.Length);
            //foreach (byte b in Data)
            //{
            //    SkyCoop.Logger.Log("byte " + b);
            //}
            PlayVoice(Data);
        }

        public void PlayVoice(byte[] Data)
        {
            SkyCoop.Logger.Log("Going to decode");
            (byte[] decodedData, int decodedLength) = VoiceInterface.WhenDataReceived(Data, Data.Length);
            float[] samples = new float[decodedLength / 2];
            VoiceUtilities.Convert16BitToFloat(decodedData, samples);

            //foreach (byte b in samples)
            //{
            //    SkyCoop.Logger.Log("byte " + b);
            //}
            VoiceBuffer.PushChunk(samples);
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
                //if (!VoiceConnected)
                //    return;
                if (!VoiceInterface.IsSpeaking(pcmData))
                    return;

                (byte[] encodedData, int encodedLength) = VoiceInterface.SubmitAudioData(pcmData, length);


                SkyCoop.Logger.Log(ConsoleColor.Cyan, "encodedData " + encodedLength + " " + encodedData.Length);

                NetDataWriter writer = new NetDataWriter();
                writer.Put(encodedLength);
                writer.Put(encodedData);
                SendToHost(writer);
            };
            VoiceBuffer = new CircularAudioBuffer<float>(BufferSamples, RecommendedChunkAmount.Unity);

            MicrophoneRecorder.StartRecording();
        }

        public void Update()
        {
            if (VoiceSource && VoiceBuffer.BufferFull)
            {
                VoiceSource.clip.SetData(VoiceBuffer.ReadAllBuffer(), 0);
                VoiceSource.Play();
            }
        }
    }
}
