using System.Numerics;
using LiteNetLib.Utils;

namespace SkyCoopServer
{
    public static class Packet
    {
        public enum Type
        {
            Welcome = 0,
            CFG,
            ClientPosition,
            ClientRotation,
            ClientScene,
            ClientHoldigGear,
            ClientCrouch,
            ClientAction,
            ClientFire,
            ClientDamageOtherClient,
            ClientProjectile,
            ClientDied,
            ClientRevived,
            KillFeedMessage,
            ClientProjectileThrow,
            ClientName,
            ClientRequestRespawn,
        }

        public static void Write(this NetDataWriter Writer, string Message)
        {
            Writer.Put(Message.Length);
            Writer.Put(Message);
        }
        public static string ReadString(this NetDataReader Reader)
        {
            return Reader.GetString(Reader.GetInt());
        }

        public static void Write(this NetDataWriter Writer, Vector3 v3)
        {
            Writer.Put(v3.X);
            Writer.Put(v3.Y);
            Writer.Put(v3.Z);
        }

        public static Vector3 ReadVector3(this NetDataReader Reader)
        {
            Vector3 v3 = new Vector3(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
            return v3;
        }

        public static void Write(this NetDataWriter Writer, Quaternion quat)
        {
            Writer.Put(quat.X);
            Writer.Put(quat.Y);
            Writer.Put(quat.Z);
            Writer.Put(quat.W);
        }

        public static void Write(this NetDataWriter Writer, DataStr.KillFeedMessage Message)
        {
            Writer.Put(Message.m_Killer);
            Writer.Put(Message.m_Victim);
            Writer.Put(Message.m_Assist);
            Writer.Put((int)Message.m_DeathReason);
            Writer.Put(Message.m_Flags.Count);
            foreach (DataStr.KillFeedFlag Flag in Message.m_Flags)
            {
                Writer.Put((int)Flag);
            }
        }

        public static DataStr.KillFeedMessage ReadKillFeedMessage(this NetDataReader Reader)
        {
            DataStr.KillFeedMessage Message = new DataStr.KillFeedMessage();
            Message.m_Killer = Reader.GetInt();
            Message.m_Victim = Reader.GetInt();
            Message.m_Assist = Reader.GetInt();
            Message.m_DeathReason = (DataStr.DamageType)Reader.GetInt();
            int Flags = Reader.GetInt();
            if(Flags > 0)
            {
                for (int i = 0; i < Flags; i++)
                {
                    Message.m_Flags.Add((DataStr.KillFeedFlag)Reader.GetInt());
                }
            }
            return Message;
        }

        public static Quaternion ReadQuaternion(this NetDataReader Reader)
        {
            Quaternion quat = new Quaternion(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
            return quat;
        }

        public static Quaternion ReadVoice(this NetDataReader Reader)
        {
            Quaternion quat = new Quaternion(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
            return quat;
        }
    }
}
