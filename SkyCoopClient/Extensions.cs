using LiteNetLib.Utils;
using UnityEngine;

namespace SkyCoop
{
    public static class Extensions
    {
        public static void Write(this NetDataWriter Writer, Quaternion quat)
        {
            Writer.Put(quat.x);
            Writer.Put(quat.y);
            Writer.Put(quat.z);
            Writer.Put(quat.w);
        }

        public static void Write(this NetDataWriter Writer, Vector3 v3)
        {
            Writer.Put(v3.x);
            Writer.Put(v3.y);
            Writer.Put(v3.z);
        }

        public static Vector3 ReadVector3Unity(this NetDataReader Reader)
        {
            Vector3 v3 = new Vector3(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
            return v3;
        }

        public static Quaternion ReadQuaternionUnity(this NetDataReader Reader)
        {
            Quaternion quat = new Quaternion(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
            return quat;
        }
    }
}
