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
        public static GameObject s_DangerCircleObject;
        public static Vector3 s_LastCenter = Vector3.zero;
        public static Vector3 s_LastScale = Vector3.zero;
        public const float c_Smoother = 7f;
        
        public static void HandleDangerCircleSync(Vector3 Center, float Radius)
        {
            if(s_DangerCircleObject == null)
            {
                s_DangerCircleObject = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromBundle<GameObject>("Zone"));
            }

            if (s_DangerCircleObject)
            {
                s_LastScale = new Vector3 (Radius, Radius, 4300);
                s_LastCenter = Center;

                s_DangerCircleObject.transform.localScale = s_LastScale;
                s_DangerCircleObject.transform.position = s_LastCenter;
            }
        }

        public static void Update()
        {
            if (s_DangerCircleObject)
            {
                s_DangerCircleObject.transform.localScale = Vector3.Lerp(s_DangerCircleObject.transform.localScale, s_LastScale, c_Smoother * Time.deltaTime);
                s_DangerCircleObject.transform.position = Vector3.Lerp(s_DangerCircleObject.transform.position, s_LastCenter, c_Smoother * Time.deltaTime);
            }
        }
    }
}
