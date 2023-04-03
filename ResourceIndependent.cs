using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Globalization;

namespace SkyCoop
{
    public class ResourceIndependent
    {
        public static Dictionary<string, string> Templates = new Dictionary<string, string>();
        public static bool IsInit = false;

        public static void Log(string TXT, Shared.LoggerColor Color = Shared.LoggerColor.White)
        {
#if (DEDICATED)
            Logger.Log(TXT, Color);
#endif
        }

        public static void Init()
        {
            if (IsInit)
            {
                return;
            }
            string BasicBlank = @"{""m_Position"":[],""m_Rotation"":[],""m_InstanceIDProxy"":-1,""m_CurrentHPProxy"":100,""m_NormalizedCondition"":1,""m_RolledSpawnChanceProxy"":true,""m_WeightKG"":0.1,""m_GearItemSaveVersion"":4,""m_InspectSerialized"":""{}""}";
            string Key = @"{""m_Position"":[],""m_Rotation"":[],""m_InstanceIDProxy"":-1,""m_CurrentHPProxy"":100,""m_NormalizedCondition"":1,""m_RolledSpawnChanceProxy"":true,""m_ObjectGuidSerialized"":""#KEYNAME_#KEYSEED"",""m_WeightKG"":0.1,""m_GearItemSaveVersion"":4,""m_InspectSerialized"":""{}""}";
            AddTemplate("gear_sclockpick", BasicBlank);
            AddTemplate("gear_scmetalblanksmall", BasicBlank);
            AddTemplate("gear_scdoorkeytemp", BasicBlank);
            AddTemplate("gear_scdoorkeyleadtemp", BasicBlank);
            AddTemplate("gear_scdoorkey", Key);
            AddTemplate("gear_scdoorkeylead", Key);
            AddTemplate("gear_rabbitcarcass", @"{""m_Position"":[],""m_Rotation"":[],""m_InstanceIDProxy"":-1,""m_CurrentHPProxy"":9.693987,""m_NormalizedCondition"":0.9693987,""m_RolledSpawnChanceProxy"":true,""m_WeightKG"":3,""m_BodyHarvestSerialized"":""{\""m_MeatAvailableKG\"":#KGVAL,\""m_HideAvailableUnits\"":1,\""m_GutAvailableUnits\"":1,\""m_Condition\"":96.93987,\""m_HoursPlayed\"":5.815399,\""m_QuarterBagWasteMultiplier\"":1,\""m_DamageSide\"":\""DamageSideLeft\""}"",""m_GearItemSaveVersion"":4,""m_InspectSerialized"":""{}""}");
            AddTemplate("gear_snare_3", @"{""m_HoursPlayed"":0,""m_Position"":[],""m_Rotation"":[],""m_InstanceIDProxy"":-1,""m_CurrentHPProxy"":100,""m_NormalizedCondition"":1,""m_BeenInPlayerInventoryProxy"":true,""m_BeenInspectedProxy"":true,""m_HasBeenOwnedByPlayer"":true,""m_SnareItemSerialized"":""{\""m_HoursPlayed\"":0,\""m_HoursAtLastRoll\"":0,\""m_State\"":\""WithRabbit\""}"",""m_WeightKG"":0.3,""m_GearItemSaveVersion"":4,""m_InspectSerialized"":""{}""}");
            AddTemplate("gear_snare_2", @"{""m_HoursPlayed"":0,""m_Position"":[],""m_Rotation"":[],""m_InstanceIDProxy"":-1,""m_CurrentHPProxy"":100,""m_NormalizedCondition"":1,""m_BeenInPlayerInventoryProxy"":true,""m_BeenInspectedProxy"":true,""m_HasBeenOwnedByPlayer"":true,""m_SnareItemSerialized"":""{\""m_HoursPlayed\"":0,\""m_HoursAtLastRoll\"":0,\""m_State\"":\""Broken\""}"",""m_WeightKG"":0.3,""m_GearItemSaveVersion"":4,""m_InspectSerialized"":""{}""}");
            IsInit = true;
        }


        public static void AddTemplate(string GearName, string JsonTemplate)
        {
            if(!Templates.ContainsKey(GearName))
            {
                Templates.Add(GearName, JsonTemplate);
            } 
        }

        public static string OverrideTransform(string JSON, Vector3 pos, Quaternion rot)
        {
            string Pos = "[" +pos.X.ToString(CultureInfo.InvariantCulture) + "," + pos.Y.ToString(CultureInfo.InvariantCulture) + "," + pos.Z.ToString(CultureInfo.InvariantCulture) + "]";
            string Rot = "[" + rot.X.ToString(CultureInfo.InvariantCulture) + "," + rot.Y.ToString(CultureInfo.InvariantCulture) + "," + rot.Z.ToString(CultureInfo.InvariantCulture) + "," + rot.W.ToString(CultureInfo.InvariantCulture) + "]";

            JSON = JSON.Replace(@"""m_Position"":[]", @"""m_Position"":" + Pos);
            JSON = JSON.Replace(@"""m_Rotation"":[]", @"""m_Rotation"":" + Rot);

            return JSON;
        }
        public static string OverrideKey(string JSON, string KeyName, string KeySeed)
        {
            string GUID = KeyName + "_" + KeySeed;
            JSON = JSON.Replace("#KEYNAME_#KEYSEED", GUID);

            return JSON;
        }

        public static string GetRabbit(Vector3 pos, Quaternion rot)
        {
            string JSON;
            if (Templates.TryGetValue("gear_rabbitcarcass", out JSON))
            {
                JSON = OverrideTransform(JSON, pos, rot);

                JSON = JSON.Replace("#KGVAL", Shared.GetBodyHarvestUnits("WILDLIFE_Rabbit").m_Meat.ToString(CultureInfo.InvariantCulture));
                Log("gear_rabbitcarcass JSON:");
                Log(JSON);

                return JSON;
            } else
            {
                return "";
            }
        }
        public static string GetSnare(Vector3 pos, Quaternion rot, int State)
        {
            string JSON;
            if (Templates.TryGetValue("gear_snare_"+ State, out JSON))
            {
                JSON = OverrideTransform(JSON, pos, rot);
                Log("gear_snare JSON:");
                Log(JSON);

                return JSON;
            } else
            {
                return "";
            }
        }

        public static string GetLocksmithGear(string GearName, Vector3 pos, Quaternion rot, string KeyName = "", string KeySeed = "")
        {
            GearName = GearName.ToLower();
            string JSON;
            if(Templates.TryGetValue(GearName, out JSON))
            {
                JSON = OverrideTransform(JSON, pos, rot);
                if(!string.IsNullOrEmpty(KeyName) && !string.IsNullOrEmpty(KeySeed))
                {
                    JSON = OverrideKey(JSON, KeyName, KeySeed);
                }
                Log(GearName + " JSON:");
                Log(JSON);

                return JSON;
            } else{
                return "";
            }
        }
    }
}
