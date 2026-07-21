using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoop
{
    public static class DangerCircleManager
    {
        public static Comps.DangerCircleZone s_DangerCircle;

        public static float s_NextZoneRadius = 0;
        public static Vector3 s_NextZoneCenter = Vector3.zero;
        public static Vector2 s_MapRefScale = new Vector2(0.018f, 0.018f);

        public static void RemoveDangerCircle()
        {
            if (s_DangerCircle)
            {
                UnityEngine.Object.Destroy(s_DangerCircle);
                s_DangerCircle = null;
            }
        }


        public static void HandleDangerCircleSync(DataStr.DangerCircleShrinkStateData Stage, Vector3 NextCenter, float NextRadius, Vector2 MapRefScale)
        {
            if(s_DangerCircle == null)
            {
                GameObject Obj = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromBundle<GameObject>("Zone"));
                Comps.DangerCircleZone Comp = Obj.AddComponent<Comps.DangerCircleZone>();
                s_DangerCircle = Comp;
            }

            if (s_DangerCircle)
            {
                s_DangerCircle.m_Data = Stage;
            }
            s_NextZoneCenter = NextCenter;
            s_NextZoneRadius = NextRadius;
            s_MapRefScale = MapRefScale;

            SkyCoop.Logger.Log($"HandleDangerCircleSync()");
        }
    }
}
