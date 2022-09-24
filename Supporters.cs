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
    public class Supporters
    {
        public static string LoadedJSON = "";
        public static Dictionary<string, List<string>> SupportersData = new Dictionary<string, List<string>>();
        public static List<string> EveryOneAvailableFlairs = new List<string>();
        public static bool DeBug = false;
        public static bool FlairsIDsReady = false;
        public static List<string> FlairsIDs = new List<string>();

        public static string MyID = "";
        public static SupporterBenefits AvailableBenefits = new SupporterBenefits();
        public static SupporterBenefits ConfiguratedBenefits = new SupporterBenefits();
        public static int FlairSpots = 4;

        public static void FlairsIDsInit()
        {
            FlairsIDs.Add("Supp"); // 0
            FlairsIDs.Add("Gift"); // 1
            FlairsIDs.Add("DrkW"); // 2
            FlairsIDs.Add("Crtg"); // 3
            FlairsIDs.Add("Bugy"); // 4
            FlairsIDs.Add("Ban"); // 5
            FlairsIDs.Add("Bee"); // 6
            FlairsIDs.Add("Pin"); // 7
            FlairsIDs.Add("Crab"); // 8
            FlairsIDs.Add("Ghst"); // 9
            FlairsIDs.Add("Mlon"); // 10
            FlairsIDs.Add("Lkot"); // 11
            FlairsIDs.Add("WiRo"); // 12
            FlairsIDs.Add("Bear"); // 13
            FlairsIDs.Add("Kai"); // 14

            FlairsIDsReady = true;
        }

        public static void ApplyFlairsForModel(int PlayerIndex, List<int> FlairsConfig)
        {
            if(PlayerIndex >= 0 && PlayerIndex < MyMod.players.Count && MyMod.players[PlayerIndex])
            {
                ApplyFlairsForModel(MyMod.players[PlayerIndex], FlairsConfig, "Client "+PlayerIndex);
            }else{
                DebugLog("Can't apply flairs, there no player object with index " + PlayerIndex);
            }
        }

        public static void ApplyFlairsForModel(GameObject obj, List<int> FlairsConfig, string DebugName = "unknown")
        {
            if (obj == null)
            {
                DebugLog("Can't apply flairs, object " +DebugName+ " is null");

                return;
            }

            // hips/root/Spine1/Flairs
            Transform FlairsRoot = obj.transform.GetChild(3).GetChild(8).GetChild(0).GetChild(7);
            for (int Spot = 0; Spot < FlairSpots; Spot++)
            {
                // hips/root/Spine1/Flairs/SpotX
                Transform FlairSpot = FlairsRoot.GetChild(Spot);
                if (FlairSpot)
                {
                    for (int FlairInx = 0; FlairInx < FlairSpot.childCount; FlairInx++)
                    {
                        // hips/root/Spine1/Flairs/SpotX/Flair
                        GameObject Flair = FlairSpot.GetChild(FlairInx).gameObject;
                        if(Spot > FlairsConfig.Count-1 || FlairsConfig[Spot] != FlairInx)
                        {
                            Flair.SetActive(false);
                        }else {
                            Flair.SetActive(true);
                            int DisplaySpot = FlairInx + 1;
                            DebugLog(DebugName + " equipped "+GetFlairNameByID(FlairInx)+" on spot "+ DisplaySpot);
                        }
                    }
                }
            }
        }

        public static int GetFlairIDByName(string name)
        {
            for (int i = 0; i < FlairsIDs.Count; i++)
            {
                if(FlairsIDs[i] == name)
                {
                    return i;
                }
            }
            return -1;
        }
        public static string GetFlairNameByID(int ID)
        {
            if (FlairsIDs[ID] != null)
            {
                return FlairsIDs[ID];
            }else{
                return "";
            }
        }
        public static bool IsFlairForEveryone(int ID)
        {
            string FlairName = GetFlairNameByID(ID);
            return IsFlairForEveryone(FlairName);
        }
        public static bool IsFlairForEveryone(string Name)
        {
            return EveryOneAvailableFlairs.Contains(Name);
        }
        // This method compares if configuration has benefits that not owned by user, this can happen
        // if user has old or modified saves where listed benefits that can't be used anymore.
        public static SupporterBenefits VerifyBenefitsWithConfig(string Owner, SupporterBenefits Desired)
        {
            SupporterBenefits Original = GetPlayerBenefits(Owner);
            SupporterBenefits Configurated = new SupporterBenefits();
            for (int i = 0; i < FlairSpots; i++)
            {
                Configurated.m_Flairs.Add(-1);
            }

            if (Original.m_Knife && Desired.m_Knife)
            {
                Configurated.m_Knife = true;
            }
            if (Original.m_BrightNick && Desired.m_BrightNick)
            {
                Configurated.m_BrightNick = true;
            }
            for (int i = 0; i < Desired.m_Flairs.Count; i++)
            {
                if(i >= FlairSpots)
                {
                    break;
                }
                int Flair = Desired.m_Flairs[i];
                if(Flair != -1)
                {
                    if (IsFlairForEveryone(Flair) || Original.m_Flairs.Contains(Flair))
                    {
                        Configurated.m_Flairs[i] = Flair;
                    }
                }
            }

            return Configurated;
        }

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
            public List<string> Every = new List<string>();
        }
        public static void Log(string LOG)
        {
            MelonLoader.MelonLogger.Msg(ConsoleColor.Blue, "[Supporters] " + LOG);
        }
        public static void DebugLog(string LOG)
        {
            if (DeBug)
            {
                MelonLoader.MelonLogger.Msg(ConsoleColor.Blue, "[Supporters] " + LOG);
            }
        }
        public static void Error(string LOG)
        {
            MelonLoader.MelonLogger.Msg(ConsoleColor.Red, "[Supporters] " + LOG);
        }

        public static bool IsLoaded()
        {
            return LoadedJSON != "";
        }

        public static int GetFlairsUpdateData()
        {
            if (!MPSaveManager.IsFileExist("FlairsUpdateData"))
            {
                return -1;
            }else{
                int PreviousAmount = int.Parse(MPSaveManager.LoadData("FlairsUpdateData"));
                return PreviousAmount;
            }
        }
        public static void SetFlairsUpdateData()
        {
            if (IsLoaded())
            {
                int Amount = AvailableBenefits.m_Flairs.Count;
                MPSaveManager.SaveData("FlairsUpdateData", Amount.ToString());
            }
        }

        public static bool AnythingNew()
        {
            if (IsLoaded())
            {
                int Previous = GetFlairsUpdateData();
                if (Previous != -1 && Previous < AvailableBenefits.m_Flairs.Count)
                {
                    return true;
                }else{
                    return false;
                }
            }else{
                return false;
            }
        }


        public static List<string> GetPlayerPerksList(string ID)
        {
            List<string> Perks = new List<string>();
            if (IsLoaded())
            {
                if (SupportersData.ContainsKey(ID))
                {
                    SupportersData.TryGetValue(ID, out Perks);
                }
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

            for (int i = 0; i < Perks.Count; i++)
            {
                int FlairID = GetFlairIDByName(Perks[i]);
                if (FlairID != -1)
                {
                    Data.m_Flairs.Add(FlairID);
                }
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
            foreach (string item in EveryOneAvailableFlairs)
            {
                Log("Everyone Available: " + item);
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
                DebugLog(LoadedJSON);
                JsonStruct LoadedData = JSON.Load(LoadedJSON).Make<JsonStruct>();
                SupportersData = new Dictionary<string, List<string>>();
                EveryOneAvailableFlairs = new List<string>();
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
                    EveryOneAvailableFlairs = LoadedData.Every;
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
            if (!FlairsIDsReady)
            {
                FlairsIDsInit();
            }
            if (!Force && !IsLoaded())
            {
                Log("Trying to get supporters list...");
                WebClient web = new WebClient();
                string url = "https://raw.githubusercontent.com/Filigrani/SkyCoop/main/Supporters.json";
                Uri uri = new Uri(url);
                web.DownloadStringCompleted += new DownloadStringCompletedEventHandler(GotJsonCallback);
                web.DownloadStringAsync(uri);
            }
        }

        public static void LoadConfiguredBenfits()
        {
            string SavedConfig = MPSaveManager.LoadData("ConfiguratedBenefits");
            if(SavedConfig == "")
            {
                ConfiguratedBenefits = VerifyBenefitsWithConfig(MyID, AvailableBenefits);
            }else{
                SupporterBenefits Desired = JSON.Load(SavedConfig).Make<SupporterBenefits>();
                ConfiguratedBenefits = VerifyBenefitsWithConfig(MyID, Desired);
            }

            if (DeBug)
            {
                Log("ConfiguratedBenefits.m_Knife " + ConfiguratedBenefits.m_Knife);
                Log("ConfiguratedBenefits.m_BrightNick " + ConfiguratedBenefits.m_BrightNick);
                Log("ConfiguratedBenefits.m_Flairs:");
                for (int i = 0; i < ConfiguratedBenefits.m_Flairs.Count; i++)
                {
                    Log("["+i+"] "+ ConfiguratedBenefits.m_Flairs[i]);
                }
                Log("AvailableBenefits.m_Knife " + AvailableBenefits.m_Knife);
                Log("AvailableBenefits.m_BrightNick " + AvailableBenefits.m_BrightNick);
                Log("AvailableBenefits.m_Flairs:");
                for (int i = 0; i < AvailableBenefits.m_Flairs.Count; i++)
                {
                    Log("[" + i + "] " + AvailableBenefits.m_Flairs[i]);
                }
            }
        }
        public static void EquipFlair(int Slot, int FlairID)
        {
            Log("Equip Flair " + FlairID + " To Slot " + Slot);
            Log("Flaist config " + ConfiguratedBenefits.m_Flairs.Count);
            ConfiguratedBenefits.m_Flairs[Slot-1] = FlairID;
            MPSaveManager.SaveData("ConfiguratedBenefits", JSON.Dump(ConfiguratedBenefits));
            ApplyFlairsForModel(MyMod.MyPlayerDoll, ConfiguratedBenefits.m_Flairs, "I am");
        }
        public static void CheckForNewFlairs()
        {
            int Amount = GetFlairsUpdateData();
            if(Amount == AvailableBenefits.m_Flairs.Count)
            {
                Log("No any new flairs from previous visit");
            }else if(Amount == -1)
            {
                Log("First visit, saving flairs count");
                SetFlairsUpdateData();
            }else if(Amount < AvailableBenefits.m_Flairs.Count)
            {
                int New = AvailableBenefits.m_Flairs.Count - Amount;
                Log("You got "+ New + " new flairs!");
                MyMod.NotificationString = New.ToString();
            }else if(Amount > AvailableBenefits.m_Flairs.Count)
            {
                SetFlairsUpdateData();
            }
        }

        public static void GetMyPerks()
        {
            if (IsLoaded() && MyID != "")
            {
                AvailableBenefits = GetPlayerBenefits(MyID);
                LoadConfiguredBenfits();
                if (MyMod.MyPlayerDoll)
                {
                    ApplyFlairsForModel(MyMod.MyPlayerDoll, ConfiguratedBenefits.m_Flairs, "I am");
                }
                Log("My Supporter Bonuses loaded!");
                CheckForNewFlairs();
            }else{
                Error("Can't loade supporter bonuses because supporters list isn't loaded!");
            }
        }
        public static void SetID(string ID)
        {
            MyID = ID;
            GetMyPerks();
        }
    }
}
