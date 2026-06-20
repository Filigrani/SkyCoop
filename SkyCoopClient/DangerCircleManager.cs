using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

namespace SkyCoop
{
    public static class DangerCircleManager
    {
        public static Comps.DangerCircleZone s_DangerCircle;
        public static float s_NextZoneRadius = 0;
        public static Vector3 s_NextZoneCenter = Vector3.zero;

        public static void RemoveDangerCircle()
        {
            if (s_DangerCircle)
            {
                UnityEngine.Object.Destroy(s_DangerCircle);
                s_DangerCircle = null;
            }
        }


        public static void HandleDangerCircleSync(Vector3 Center, float Radius, Vector3 NextCenter, float NextRadius)
        {
            if(s_DangerCircle == null)
            {
                GameObject Obj = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromBundle<GameObject>("Zone"));
                Comps.DangerCircleZone Comp = Obj.AddComponent<Comps.DangerCircleZone>();
                s_DangerCircle = Comp;

            }

            if (s_DangerCircle)
            {
                s_DangerCircle.m_TargetRadius = Radius;
                s_DangerCircle.m_Center = Center;
            }
            s_NextZoneCenter = NextCenter;
            s_NextZoneRadius = NextRadius;
        }
    }
}
