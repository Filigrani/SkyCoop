using LiteNetLib.Utils;
using UnityEngine;
using System.Numerics;
using Il2Cpp;

namespace SkyCoop
{
    public static class Extensions
    {
        public static void Write(this NetDataWriter Writer, UnityEngine.Quaternion quat)
        {
            Writer.Put(quat.x);
            Writer.Put(quat.y);
            Writer.Put(quat.z);
            Writer.Put(quat.w);
        }

        public static void Write(this NetDataWriter Writer, UnityEngine.Vector3 v3)
        {
            Writer.Put(v3.x);
            Writer.Put(v3.y);
            Writer.Put(v3.z);
        }

        public static UnityEngine.Vector3 ReadVector3Unity(this NetDataReader Reader)
        {
            UnityEngine.Vector3 v3 = new UnityEngine.Vector3(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
            return v3;
        }

        public static UnityEngine.Quaternion ReadQuaternionUnity(this NetDataReader Reader)
        {
            UnityEngine.Quaternion quat = new UnityEngine.Quaternion(Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat(), Reader.GetFloat());
            return quat;
        }

        public static UnityEngine.Vector3 ConvertToUnity(this System.Numerics.Vector3 V3)
        {
            UnityEngine.Vector3 Vector = new UnityEngine.Vector3(V3.X, V3.Y, V3.Z);
            return Vector;
        }
        public static UnityEngine.Quaternion ConvertToUnity(this System.Numerics.Quaternion Quat)
        {
            UnityEngine.Quaternion Quaternion = new UnityEngine.Quaternion(Quat.X, Quat.Y, Quat.Z, Quat.W);
            return Quaternion;
        }
    }
}
