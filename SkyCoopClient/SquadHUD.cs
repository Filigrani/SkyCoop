using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SkyCoop.Comps;

namespace SkyCoopClient
{
    public static class SquadHUD
    {
        public static List<SquadMember> s_SquadMembers = new List<SquadMember>();

        public class SquadMember
        {
            public int m_ID = 0;
            public float m_Health = 100;
            public bool m_HasDebuffs = false;
            public bool m_KnockedDown = false;

            public SquadMember(int PlayerID, float Health, bool Debuffs, bool Knocked)
            {
                m_Health = Health;
                m_ID = PlayerID;
                m_HasDebuffs = Debuffs;
                m_KnockedDown = Knocked;
            }
        }

        public static SquadMember GetMember(int PlayerID)
        {
            for (int i = 0; i < s_SquadMembers.Count; i++)
            {
                if (s_SquadMembers[i].m_ID == PlayerID)
                {
                    return s_SquadMembers[i];
                }
            }
            return null;
        }

        public static void UpdateMember(int PlayerID, float Health, bool Debuffs, bool KnockedDown)
        {
            SquadMember member = GetMember(PlayerID);
            if (member != null)
            {
                member.m_Health = Health;
                member.m_HasDebuffs = Debuffs;
                member.m_KnockedDown = KnockedDown;
            }
            else
            {
                AddMember(PlayerID, Health, Debuffs, KnockedDown);
            }
        }

        public static void AddMember(int PlayerID, float Health, bool Debuffs, bool KnockedDown)
        {
            s_SquadMembers.Add(new SquadMember(PlayerID, Health, Debuffs, KnockedDown));
        }

        public static void RemoveMember(int PlayerID)
        {
            for(int i = s_SquadMembers.Count; i >= 0; i--)
            {
                if (s_SquadMembers[i].m_ID == PlayerID)
                {
                    s_SquadMembers.RemoveAt(i);
                }
            }
        }


        [HarmonyLib.HarmonyPatch(typeof(StatusBar), "GetFillValuesCondition")]
        private static class StatusBar_GetFillValues
        {
            private static void Postfix(StatusBar __instance, ref StatusBar.FillValues __result)
            {
                if (__instance.transform.parent)
                {
                    TeammateBar TB = __instance.transform.parent.GetComponent<TeammateBar>();
                    if (TB)
                    {
                        __result.m_Fill = TB.m_Health / 100;
                        __result.m_NormalizedValue = TB.m_Health / 100;
                    }
                }
            }
        }
    }
}
