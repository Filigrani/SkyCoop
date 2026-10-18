using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace SkyCoop
{
    // Hand-tuning of held items on other players. Mods/SkyCoopGearOffsets.txt, re-read while the game runs:
    //   GEAR_Name = x y z rx ry rz scale      model offset (bundle models only): metres along the hand axes, degrees around the model axes, scale multiplier
    //   HANDR_GEAR_Name = x y z rx ry rz      right hand bone offset while this item is held (metres in the parent axes, degrees)
    //   HANDL_GEAR_Name = x y z rx ry rz      same for the left hand
    // The "Gear Fit" panel of the debug menu edits the same values and saves them.
    public static class GearFit
    {
        public static readonly string[] BundleItems = {
            "GEAR_Rifle_Barbs", "GEAR_Rifle_Curators", "GEAR_Rifle_Trader", "GEAR_Rifle_Vaughns", "GEAR_RevolverFancy", "GEAR_RevolverGreen", "GEAR_RevolverStubNosed",
            "GEAR_Bow_Bushcraft", "GEAR_Bow_Manufactured", "GEAR_Bow_Woodwrights", "GEAR_KeroseneLamp_Spelunkers", "GEAR_Knife", "GEAR_KnifeImprovised", "GEAR_SurvivalKnife",
            "GEAR_CougarClawKnife", "GEAR_Hatchet", "GEAR_HatchetImprovised", "GEAR_Hammer", "GEAR_Prybar", "GEAR_Hacksaw", "GEAR_Flashlight", "GEAR_Flashlight_LongLasting",
            "GEAR_HandheldShortwave", "GEAR_Camera" };
        // Items the player model already has (only the hands can be tuned for these).
        public static readonly string[] BaseItems = { "GEAR_Rifle", "GEAR_Revolver", "GEAR_Bow", "GEAR_KeroseneLamp", "GEAR_Torch", "GEAR_FlareGun", "GEAR_FlareA", "GEAR_BlueFlare", "GEAR_Stone", "GEAR_NoiseMaker", "GEAR_WoodMatches" };
        public static readonly List<string> AllItems = new List<string>();

        static readonly Dictionary<string, float[]> s_Values = new Dictionary<string, float[]>();
        static System.DateTime s_Stamp = System.DateTime.MinValue;
        static bool s_Loaded = false;
        static float s_NextPoll = 0f;
        public static int Version = 0;

        static string FilePath { get { return System.IO.Path.Combine(MelonLoader.Utils.MelonEnvironment.ModsDirectory, "SkyCoopGearOffsets.txt"); } }

        static GearFit()
        {
            AllItems.AddRange(BundleItems);
            AllItems.AddRange(BaseItems);
        }

        public static bool IsBundleItem(string gear) { return System.Array.IndexOf(BundleItems, gear) >= 0; }

        // Every rifle (Barbs, Curators, Trader, Vaughns...) is held like the base rifle: they share its hand bone tuning.
        static string Canon(string key)
        {
            int i = key.IndexOf("GEAR_Rifle_");
            return key.StartsWith("HAND") && i > 0 ? key.Substring(0, i) + "GEAR_Rifle" : key;
        }

        public static float[] Get(string key)
        {
            key = Canon(key);
            if (!s_Loaded) Load();
            float[] v;
            if (!s_Values.TryGetValue(key, out v))
            {
                v = key.StartsWith("HAND") ? new float[6] : new float[] { 0, 0, 0, 0, 0, 0, 1 };
                s_Values[key] = v;
            }
            return v;
        }

        // Called often: re-reads the file when it changed on disk.
        public static void Poll()
        {
            if (Time.time < s_NextPoll) return;
            s_NextPoll = Time.time + 1f;
            try
            {
                if (File.Exists(FilePath) && File.GetLastWriteTimeUtc(FilePath) != s_Stamp) { Load(); Version++; }
            }
            catch (System.Exception e) { MelonLoader.MelonLogger.Warning("[GearFit] " + e.Message); }
        }

        static void Load()
        {
            s_Loaded = true;
            try
            {
                if (!File.Exists(FilePath)) { Save(); return; }
                s_Stamp = File.GetLastWriteTimeUtc(FilePath);
                foreach (string line in File.ReadAllLines(FilePath))
                {
                    string[] kv = line.Split('=');
                    if (kv.Length != 2 || line.TrimStart().StartsWith("#")) continue;
                    string key = Canon(kv[0].Trim());
                    if (kv[0].Trim() != key) continue; // rifle variants follow the base rifle's hand values
                    string[] v = kv[1].Split(new[] { ' ', '\t', ';' }, System.StringSplitOptions.RemoveEmptyEntries);
                    int need = key.StartsWith("HAND") ? 6 : 7;
                    var f = new float[need];
                    bool ok = v.Length == need;
                    for (int i = 0; ok && i < need; i++) ok = float.TryParse(v[i].Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out f[i]);
                    if (ok) s_Values[key] = f; else MelonLoader.MelonLogger.Warning("[GearFit] Bad line: " + line);
                }
            }
            catch (System.Exception e) { MelonLoader.MelonLogger.Warning("[GearFit] " + e.Message); }
        }

        static string Fmt(float[] v)
        {
            var parts = new string[v.Length];
            for (int i = 0; i < v.Length; i++) parts[i] = v[i].ToString("0.####", CultureInfo.InvariantCulture);
            return string.Join(" ", parts);
        }

        public static void Save()
        {
            try
            {
                var sb = new System.Text.StringBuilder();
                sb.Append("# GEAR_Name = x y z rx ry rz scale   model offset: metres along the hand axes, degrees around the model axes, scale multiplier\n");
                sb.Append("# HANDR_GEAR_Name / HANDL_GEAR_Name = x y z rx ry rz   hand bone offset while the item is held: metres, degrees\n");
                foreach (string g in AllItems)
                {
                    if (IsBundleItem(g)) sb.Append(g + " = " + Fmt(Get(g)) + "\n");
                    sb.Append("HANDR_" + g + " = " + Fmt(Get("HANDR_" + g)) + "\n");
                    sb.Append("HANDL_" + g + " = " + Fmt(Get("HANDL_" + g)) + "\n");
                }
                File.WriteAllText(FilePath, sb.ToString());
                s_Stamp = File.GetLastWriteTimeUtc(FilePath);
            }
            catch (System.Exception e) { MelonLoader.MelonLogger.Warning("[GearFit] " + e.Message); }
        }

        public static void ApplyModel(Transform model, string gear, Transform hand)
        {
            if (model == null) return;
            float[] o = Get(gear);
            float m = 1f / hand.lossyScale.x; // metres -> hand bone units
            model.localPosition += new Vector3(o[0], o[1], o[2]) * m;
            model.localRotation = model.localRotation * Quaternion.Euler(o[3], o[4], o[5]);
            model.localScale *= o[6];
        }

        // After the animator: nudge the hand bones. The arm skin stretches with them, so keep it small.
        public static void ApplyHand(Transform bone, string key)
        {
            float[] o = Get(key);
            if (o[0] == 0 && o[1] == 0 && o[2] == 0 && o[3] == 0 && o[4] == 0 && o[5] == 0) return;
            float m = 1f / bone.parent.lossyScale.x;
            bone.localPosition += new Vector3(o[0], o[1], o[2]) * m;
            bone.localRotation = bone.localRotation * Quaternion.Euler(o[3], o[4], o[5]);
        }

        // ---- debug menu panel ----
        static int s_Item = 0;
        static bool s_Fine = false;

        static void Row(float y, string label, float[] v, int i, float step)
        {
            GUI.Label(new Rect(270, y, 90, 20), label);
            GUI.Label(new Rect(360, y, 60, 20), v[i].ToString("0.###", CultureInfo.InvariantCulture));
            if (GUI.Button(new Rect(420, y, 28, 20), "-")) { v[i] -= step; Version++; }
            if (GUI.Button(new Rect(450, y, 28, 20), "+")) { v[i] += step; Version++; }
            if (GUI.Button(new Rect(480, y, 28, 20), "0")) { v[i] = (i == 6) ? 1f : 0f; Version++; }
        }

        public static void DrawPanel()
        {
            string gear = AllItems[s_Item];
            bool bundle = IsBundleItem(gear);
            GUI.Box(new Rect(260, 10, 260, 640), "Gear Fit");
            if (GUI.Button(new Rect(270, 35, 28, 20), "<")) { s_Item = (s_Item + AllItems.Count - 1) % AllItems.Count; Version++; }
            GUI.Label(new Rect(300, 35, 180, 20), gear);
            if (GUI.Button(new Rect(482, 35, 28, 20), ">")) { s_Item = (s_Item + 1) % AllItems.Count; Version++; }
            if (GUI.Button(new Rect(270, 60, 110, 20), s_Fine ? "Step: fine" : "Step: coarse")) s_Fine = !s_Fine;
            if (GUI.Button(new Rect(390, 60, 120, 20), "Save to file")) Save();
            float pos = s_Fine ? 0.002f : 0.01f, rot = s_Fine ? 1f : 5f, scl = s_Fine ? 0.02f : 0.1f;
            string[] names = { "x (m)", "y (m)", "z (m)", "rot x", "rot y", "rot z", "scale" };
            float y = 90;
            if (bundle)
            {
                GUI.Label(new Rect(270, y, 240, 20), "Model in hand");
                y += 22;
                float[] v = Get(gear);
                for (int i = 0; i < 7; i++, y += 22) Row(y, names[i], v, i, i < 3 ? pos : i < 6 ? rot : scl);
                y += 6;
            }
            foreach (string side in new[] { "HANDR_", "HANDL_" })
            {
                GUI.Label(new Rect(270, y, 240, 20), side == "HANDR_" ? "Right hand" : "Left hand");
                y += 22;
                float[] v = Get(side + gear);
                for (int i = 0; i < 6; i++, y += 22) Row(y, names[i], v, i, i < 3 ? pos : rot);
                y += 6;
            }
            GUI.Label(new Rect(270, y, 240, 40), "Hold the item on another player;\nchanges apply live.");
        }
    }
}
