using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MelonLoader.TinyJSON;
using System.Net;

namespace SkyCoop
{
    class Supporters
    {
        public static string LoadedJSON = "";
        public static Dictionary<string, List<string>> SupportersData = new Dictionary<string, List<string>>();
        public static bool DeBug = false;

        public class SupporterBenefits
        {
            public bool m_Knife = false;
            public bool m_BrightNick = false;
            public List<int> m_Flairs = new List<int>();
        }

        public class Supporter
        {
            public string E = "";
            public string S = "";
            public string N = "";
            public List<string> P = new List<string>();
        }
        public class JsonStruct
        {
            public List<Supporter> Supps = new List<Supporter>();
        }
        public static void Log(string LOG)
        {
            MelonLoader.MelonLogger.Msg(ConsoleColor.Blue, "[Supporters] " + LOG);
        }

        public static bool IsLoaded()
        {
            return LoadedJSON != "";
        }
        public static List<string> GetPlayerPerksList(string ID)
        {
            List<string> Perks = new List<string>();
            if (IsLoaded())
            {
                SupportersData.TryGetValue(ID, out Perks);
            }
            return Perks;
        }
        public static SupporterBenefits GetPlayerBenefits(string ID)
        {
            return ConvertToBenefits(GetPlayerPerksList(ID));
        }

        public static SupporterBenefits ConvertToBenefits(List<string> Perks)
        {
            SupporterBenefits Data = new SupporterBenefits();

            if (Perks.Contains("K"))
            {
                Data.m_Knife = true;
            }
            if (Perks.Contains("B"))
            {
                Data.m_BrightNick = true;
            }

            return Data;
        }

        public static void PrintAll()
        {
            Log("Going to print everything...");
            foreach (var item in SupportersData)
            {
                string Perks = "";
                for (int i = 0; i < item.Value.Count; i++)
                {
                    if(Perks == "")
                    {
                        Perks = item.Value[i];
                    }else{
                        Perks = Perks + " " + item.Value[i];
                    }
                }
                Log("Key: "+ item.Key+" Perks: "+Perks);
            }
        }

        public static void GotJsonCallback(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }
            if (e.Result != "")
            {
                Log("Json data has been successfully received!");
                LoadedJSON = e.Result;
                if (DeBug)
                {
                    Log(LoadedJSON);
                }
                JsonStruct LoadedData = JSON.Load(LoadedJSON).Make<JsonStruct>();
                SupportersData = new Dictionary<string, List<string>>();
                if (LoadedData != null)
                {
                    foreach (Supporter item in LoadedData.Supps)
                    {
                        if (!string.IsNullOrEmpty(item.S))
                        {
                            if (!SupportersData.ContainsKey(item.S))
                            {
                                SupportersData.Add(item.S, item.P);
                            }
                        }
                        if (!string.IsNullOrEmpty(item.E))
                        {
                            if (!SupportersData.ContainsKey(item.E))
                            {
                                SupportersData.Add(item.E, item.P);
                            }
                        }
                    }
                }
                Log("Parsing to dictionary complete!");
                if (DeBug)
                {
                    PrintAll();
                }
            }
        }

        public static void GetSupportersList(bool Force = false)
        {
            if(!Force && !IsLoaded())
            {
                Log("Trying to get supporters list...");
                WebClient web = new WebClient();
                string url = "https://raw.githubusercontent.com/Filigrani/SkyCoop/main/Supporters.json";
                Uri uri = new Uri(url);
                web.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GotJsonCallback);
                web.DownloadStringAsync(uri);
            }
        }
    }
}
