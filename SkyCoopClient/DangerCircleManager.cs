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
        
        public static void HandleDangerCircleSync(Vector3 Center, float Radius)
        {
            if(s_DangerCircle == null)
            {
                GameObject Obj = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromBundle<GameObject>("Zone"));
                Comps.DangerCircleZone Comp = Obj.AddComponent<Comps.DangerCircleZone>();
                s_DangerCircle = Comp;

            }

            if (s_DangerCircle)
            {
                s_DangerCircle.m_TargetScale = Radius;
                s_DangerCircle.m_Center = Center;
            }
        }
    }
}
