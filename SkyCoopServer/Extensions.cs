using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SkyCoopServer
{
    public static class Extensions
    {
        
        // System.Random экстеншены что бы они работали на подоби UnityEngine.Random.Range
        public static int Range(this Random random, int min, int max)
        {
            return random.Next(min, max);
        }
        public static double Range(this Random random, double min, double max)
        {
            return min + random.NextDouble() * (max - min);
        }
        public static float Range(this Random random, float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        public static Quaternion Euler(Vector3 euler)
        {
            return Euler(euler.X, euler.Y, euler.Z);
        }

        public static Quaternion Euler(float x, float y, float z)
        {
            float degToRad = MathF.PI / 180f;
            x *= degToRad;
            y *= degToRad;
            z *= degToRad;

            float halfX = x * 0.5f;
            float halfY = y * 0.5f;
            float halfZ = z * 0.5f;

            float sinX = MathF.Sin(halfX);
            float cosX = MathF.Cos(halfX);
            float sinY = MathF.Sin(halfY);
            float cosY = MathF.Cos(halfY);
            float sinZ = MathF.Sin(halfZ);
            float cosZ = MathF.Cos(halfZ);

            float w = cosX * cosY * cosZ + sinX * sinY * sinZ;
            float qx = sinX * cosY * cosZ - cosX * sinY * sinZ;
            float qy = cosX * sinY * cosZ + sinX * cosY * sinZ;
            float qz = cosX * cosY * sinZ - sinX * sinY * cosZ;

            return new Quaternion(qx, qy, qz, w);
        }
    }
}
