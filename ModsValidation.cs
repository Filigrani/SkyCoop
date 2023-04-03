using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MelonLoader;
using System.Security.Cryptography;
using MelonLoader.TinyJSON;
using MelonLoader.Lemons.Cryptography;
using Il2Cpp;

namespace SkyCoop
{
    public class ModsValidation
    {
        public static ModValidationData LastRequested = null;
        public static List<string> ServerSideOnlyFiles = new List<string>();
        public static List<string> WhitelistFiles = new List<string>();

        public static string SHA256CheckSum(string filePath)
        {
            if (!File.Exists(filePath))
                return "null";

            byte[] byteHash;
            using (SHA256 SHA256 = SHA256Managed.Create())
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    byteHash = SHA256.ComputeHash(fileStream);
                }
            }
            string finalHash = string.Empty;
            foreach (byte b in byteHash)
                finalHash += b.ToString("x2");

            return finalHash;
        }

        public class ModHashPair
        {
            public string m_Name = "";
            public string m_Hash = "";
            public ModHashPair(string n, string h)
            {
                m_Name = n;
                m_Hash = h;
            }
        }

        public class ModValidationData 
        {
            public long m_Hash = 0;
            public List<ModHashPair> m_Files = new List<ModHashPair>();
            public string m_FullString = "";
            public string m_FullStringBase64 = "";
            public List<string> m_WhiteList = new List<string>();
        }

        public static bool ServerSideOnly(string Name)
        {
            return ServerSideOnlyFiles.Contains(Name) || WhitelistFiles.Contains(Name);
        }

        public static ModValidationData GetModsHash(bool Force = false, List<string> WhitelistedHashes = default)
        {
            ModValidationData Valid = new ModValidationData();
            if(WhitelistedHashes == null)
            {
                WhitelistedHashes = new List<string>();
            }
            if (!Force && LastRequested != null)
            {
                return LastRequested;
            }

            if (MyMod.DedicatedServerAppMode)
            {
                if (File.Exists(@"Mods\serversideonly.json"))
                {
                    MelonLogger.Msg(ConsoleColor.Yellow, "[ModsValidation][Info] Found Server Side Files List!");
                    string FilterJson = System.IO.File.ReadAllText("Mods\\serversideonly.json");
                    ServerSideOnlyFiles = JSON.Load(FilterJson).Make<List<string>>();
                }
                if (File.Exists("modswhitelist.json"))
                {
                    MelonLogger.Msg(ConsoleColor.Yellow, "[ModsValidation][Info] Found Mods White List!");
                    string FilterJson = System.IO.File.ReadAllText("modswhitelist.json");
                    WhitelistFiles = JSON.Load(FilterJson).Make<List<string>>();
                }
            }

            foreach (MelonMod Mod in MelonHandler.Mods)
            {
                string Hash = MelonHandler.GetMelonHash(Mod);
                string FileName = Mod.Assembly.GetName().Name + ".dll";
                if (!ServerSideOnly(FileName) || WhitelistedHashes.Contains(Hash))
                {
                    Valid.m_Files.Add(new ModHashPair(@"Mods\" + FileName, Hash));
                }else{
                    MelonLogger.Msg(ConsoleColor.Yellow, "[ModsValidation][Info] Ignore " + FileName);
                }

                if (WhitelistFiles.Contains(FileName))
                {
                    Valid.m_WhiteList.Add(Hash);
                }
            }
            DirectoryInfo d = new DirectoryInfo("Mods");
            FileInfo[] Files = d.GetFiles("*.modcomponent");

            foreach (FileInfo file in Files)
            {
                string Hash = SHA256CheckSum("Mods\\" + file.Name);
                string FileName = file.Name;
                if (!ServerSideOnly(FileName) && !WhitelistedHashes.Contains(Hash))
                {
                    Valid.m_Files.Add(new ModHashPair(@"Mods\" + FileName, Hash));
                }else{
                    MelonLogger.Msg(ConsoleColor.Yellow, "[ModsValidation][Info] Ignore " + FileName);
                }

                if (WhitelistFiles.Contains(FileName))
                {
                    Valid.m_WhiteList.Add(Hash);
                }
            }
            foreach (MelonPlugin Plugin in MelonHandler.Plugins)
            {
                string Hash = MelonHandler.GetMelonHash(Plugin);
                string FileName = Plugin.Assembly.GetName().Name + ".dll";
                if (!ServerSideOnly(FileName) && !WhitelistedHashes.Contains(Hash))
                {
                    Valid.m_Files.Add(new ModHashPair(@"Plugins\" + FileName, Hash));
                }else{
                    MelonLogger.Msg(ConsoleColor.Yellow, "[ModsValidation][Info] Ignore " + FileName);
                }

                if (WhitelistFiles.Contains(FileName))
                {
                    Valid.m_WhiteList.Add(Hash);
                }
            }
            string MainHash = "";
            string FullString = "";
            Valid.m_Files.Sort(delegate (ModHashPair x, ModHashPair y) {
                return x.m_Name.CompareTo(y.m_Name);
            });
            foreach (ModHashPair Mod in Valid.m_Files)
            {
                if (string.IsNullOrEmpty(MainHash))
                {
                    MainHash = Mod.m_Hash;
                    FullString = Mod.m_Name;
                }else{
                    MainHash = MainHash + Mod.m_Hash;
                    FullString = FullString + "\n" + Mod.m_Name;
                }

                MelonLogger.Msg(ConsoleColor.Green,"[ModsValidation][Info] " +Mod.m_Name+" Hash: "+Mod.m_Hash);
            }

            Valid.m_Hash = Shared.GetDeterministicId(MainHash);
            Valid.m_FullString = FullString;
            Valid.m_FullStringBase64 = Shared.CompressString(FullString);
            MelonLogger.Msg(ConsoleColor.Blue,"[ModsValidation][Info] Main Hash: " + Valid.m_Hash);
            MelonLogger.Msg(ConsoleColor.Magenta, "[ModsValidation][Info] Stock: " + Encoding.UTF8.GetBytes(Valid.m_FullString).Length);
            MelonLogger.Msg(ConsoleColor.Magenta, "[ModsValidation][Info] Compressed: " + Shared.CompressString(Valid.m_FullStringBase64).Length);
            LastRequested = Valid;
            return Valid;
        }
    }
}
