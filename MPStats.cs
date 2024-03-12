using GameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SkyCoop.MPSaveManager;
#if (DEDICATED)
using System.Numerics;
using TinyJSON;
#else
using MelonLoader.TinyJSON;
using MelonLoader;
using UnityEngine;
#endif

namespace SkyCoop
{
    public class MPStats
    {
        public static bool Started = false;
        public static Day TodayStats = new Day();
        public static Day AllTimeStats = new Day();
        public static int DataDay = 0;
        public static Dictionary<string, PlayerStatistic> RecentPlayersGlobalStatistic = new Dictionary<string, PlayerStatistic>();
        public static int SaveRecentTimer = 0;
        public static int SaveRecentPerioud = 300;
        public static void Log(string TXT, Shared.LoggerColor Color = Shared.LoggerColor.White)
        {
#if (!DEDICATED)
            MelonLogger.Msg(MyMod.ConvertLoggerColor(Color), TXT);
#else
            Logger.Log(TXT, Color);
#endif
        }

        public static void Start()
        {
            Started = true;
            AllTimeStats = LoadDayStats("AllTime");
            StartDay();
        }
        
        
        public static void StartDay(bool SendWebhook = false)
        {
            string StatsString = TodayStats.GetString(false, true, true);
            DateTime DT = System.DateTime.Now;
            DataDay = DT.Day;
            string FileName = DT.Day + "_" + DT.Month + "_" + DT.Year;
            TodayStats = LoadDayStats(FileName);
            Log("[MPStats] Starting statistic for "+ DT.Day + "." + DT.Month + "." + DT.Year, Shared.LoggerColor.Blue);

            if (SendWebhook)
            {
#if (DEDICATED)
                DiscordManager.TodayStats(StatsString);
#endif
            }
        }


        public static void SaveRecentStuff()
        {
            int SaveSeed = GetSeed();
            ValidateRootExits();
            CreateFolderIfNotExist(GetPathForName("Statistic", GetSeed()));
            CreateFolderIfNotExist(GetPathForName("Statistic" + GetSeparator()+"Global", GetSeed()));

            foreach (var item in RecentPlayersGlobalStatistic)
            {
                if (!string.IsNullOrEmpty(item.Value.MAC))
                {
                    SaveData(item.Value.MAC, JSON.Dump(item.Value), SaveSeed, "", "Statistic" + GetSeparator() + "Global" + GetSeparator() + item.Value.MAC);
                }
            }
            RecentPlayersGlobalStatistic.Clear();
            SaveData("AllTime", JSON.Dump(AllTimeStats), SaveSeed, "", "Statistic" + GetSeparator() + "AllTime");
            DateTime DT = System.DateTime.Now;
            if (DT.Day == DataDay)
            {
                string DayFileName = DT.Day + "_" + DT.Month + "_" + DT.Year;
                SaveData(DayFileName, JSON.Dump(TodayStats), SaveSeed, "", "Statistic" + GetSeparator() + DayFileName);
            }
        }
        public static PlayerStatistic LoadPlayerGlobalStats(string MAC, string Name)
        {
            CreateFolderIfNotExist(GetPathForName("Statistic", GetSeed()));
            CreateFolderIfNotExist(GetPathForName("Statistic" + GetSeparator() + "Global", GetSeed()));
            string Data = LoadData("Statistic" + GetSeparator() + "Global" + GetSeparator() + MAC, GetSeed());

            if (string.IsNullOrEmpty(Data))
            {
                return new PlayerStatistic(MAC, Name);
            } else
            {
                return JSON.Load(Data).Make<PlayerStatistic>();
            }
        }
        public static Day LoadDayStats(string TodayFileName)
        {
            CreateFolderIfNotExist(GetPathForName("Statistic", GetSeed()));
            string Data = LoadData("Statistic" + GetSeparator() + TodayFileName, GetSeed());

            if (string.IsNullOrEmpty(Data))
            {
                return new Day();
            } else
            {
                return JSON.Load(Data).Make<Day>();
            }
        }

        public class PlayTime
        {
            public int Seconds = 0;
            public int Minutes = 0;
            public int Hours = 0;
            public int Days = 0;
            
            public void AddSecond()
            {
                Seconds++;
                if (Seconds >= 60){Seconds = 0;AddMinute();}
            }
            public void AddMinute()
            {
                Minutes++;
                if (Minutes >= 60){ Minutes = 0; AddHour(); }
            }
            public void AddHour()
            {
                Hours++;
                if (Hours >= 24){Hours = 0; AddDay(); }
            }
            public void AddDay()
            {
                Days++;
            }
            public string GetString()
            {
                string Info = "";
                if(Seconds > 1)
                {
                    Info = Seconds+" Seconds";
                } else
                {
                    Info = "1 Second";
                }
                if(Minutes > 0)
                {
                    if(Minutes == 1)
                    {
                        Info = "1 Minute "+Info;
                    } else
                    {
                        Info = Minutes+" Minutes " + Info;
                    }
                }
                if (Hours > 0)
                {
                    if (Hours == 1)
                    {
                        Info = "1 Hour " + Info;
                    } else
                    {
                        Info = Hours + " Hours " + Info;
                    }
                }
                if (Days > 0)
                {
                    if (Days == 1)
                    {
                        Info = "1 Day " + Info;
                    } else
                    {
                        Info = Days + " Days " + Info;
                    }
                }
                return Info;
            }
        }
        public class ResourcesStatistic
        {
            public int GearsPicked = 0;
            public int ContainersLooted = 0;
            public int AnimalsKilled = 0;
            public int PlantsHarvested = 0;
        }

        public static string GetRegionName(int Region)
        {
            return ExpeditionBuilder.GetRegionString(Region);
        }

        public class PlayerStatistic
        {
            public string Name = "";
            public string MAC = "";
            public int Visits = 1;
            public int Deaths = 0;
            public int ExpeditionsCompleted = 0;
            public Dictionary<string, bool> CompletedExpeditions = new Dictionary<string, bool>();
            public int CrashSitesFound = 0;
            public PlayTime TotalPlayTime = new PlayTime();
            public Dictionary<int, PlayTime> RegionsHistory = new Dictionary<int, PlayTime>();
            public ResourcesStatistic Looted = new ResourcesStatistic();
            public PlayerStatistic(string mac = "", string name = "")
            {
                MAC = mac;
                Name = name;
            }
            public PlayerStatistic()
            {

            }

            public string GetString(bool HideMAC = true, bool HideRegionPlayTime = true)
            {
                string Info = "Name "+Name;
                if (!HideMAC)
                {
                    Info += "\nMAC " + MAC;
                }
                Info += "\nTotal Play Time "+TotalPlayTime.GetString()+ 
                    "\nServer Visits " + Visits + 
                    "\nDeaths " + Deaths +
                    "\nPicked Gears " + Looted.GearsPicked + 
                    "\nLooted Containers " + Looted.ContainersLooted + 
                    "\nPlants Harvested " + Looted.PlantsHarvested + 
                    "\nAnimals Killed " + Looted.AnimalsKilled +
                    "\nExpeditions Completed " + ExpeditionsCompleted +
                    "\nCrash Sites Found "+ CrashSitesFound;

                if (!HideRegionPlayTime && RegionsHistory.Count > 0)
                {
                    Info += "\nRegions Play Time:";
                    foreach (var item in RegionsHistory)
                    {
                        Info += "\n" + GetRegionName(item.Key) + ": " + item.Value.GetString();
                    }
                }
                return Info;
            }
        }

        public class ExpeditionsProgressData
        {
            public Dictionary<int, int> ExpeditionsProgress = new Dictionary<int, int>();
            public int TotalProgress = 0;
        }

        public class Day
        {
            public int Visits = 0;
            public int UniqueVisits = 0;
            public int ExpeditionsCompleted = 0;
            public int CrashSitesFound = 0;
            public Dictionary<string, int> VisitsHistory = new Dictionary<string, int>();
            public PlayTime OnlineTime = new PlayTime();
            public PlayTime EmptyTime = new PlayTime();
            public Dictionary<string, Dictionary<string, int>> ActivitySnapshots = new Dictionary<string, Dictionary<string, int>>();
            public int Deaths = 0;
            public ResourcesStatistic Looted = new ResourcesStatistic();
            public Dictionary<string, PlayerStatistic> Players = new Dictionary<string, PlayerStatistic>();
            public Dictionary<int, PlayTime> RegionsHistory = new Dictionary<int, PlayTime>();

            public string GetString(bool IncludePlayersStats = false, bool HideMAC = true, bool HideRegionPlayTime = true)
            {
                string Info = "Visits " + Visits+ 
                    "\nUnique Visits "+ UniqueVisits+
                    "\nServer Works " + OnlineTime.GetString() +
                    "\nEmpty Time " + EmptyTime.GetString() +
                    "\nPlayers Died " + Deaths +
                    "\nPicked Gears " + Looted.GearsPicked+
                    "\nLooted Containers "+ Looted.ContainersLooted +
                    "\nPlants Harvested " +Looted.PlantsHarvested+
                    "\nAnimals Killed "+Looted.AnimalsKilled +
                    "\nExpeditions Completed " + ExpeditionsCompleted +
                    "\nCrash Sites Found "+ CrashSitesFound;

                if (!HideRegionPlayTime && RegionsHistory.Count > 0)
                {
                    Info += "\nRegions Play Time:";

                    foreach (var item in RegionsHistory)
                    {
                        Info += "\n" + GetRegionName(item.Key) + ": " + item.Value.GetString();
                    }
                }

                if (IncludePlayersStats && Players.Count > 0)
                {
                    Info += "\nToday Statistic Of Players:";
                    foreach (var item in Players)
                    {
                        Info += "\n"+item.Value.GetString(HideMAC, HideRegionPlayTime);
                    }
                }
                return Info;
            }
        }

        public static void AddPlayer(string mac, string name = "")
        {
            if (!TodayStats.Players.ContainsKey(mac))
            {
                TodayStats.Players.Add(mac, new PlayerStatistic(mac, name));
            } else
            {
                TodayStats.Players[mac].Visits++;
            }
            if (!TodayStats.VisitsHistory.ContainsKey(mac))
            {
                TodayStats.VisitsHistory.Add(mac, 1);
                TodayStats.UniqueVisits++;
            } else{
                TodayStats.VisitsHistory[mac] = TodayStats.VisitsHistory[mac]++;
            }
            if (!AllTimeStats.VisitsHistory.ContainsKey(mac))
            {
                AllTimeStats.VisitsHistory.Add(mac, 1);
                AllTimeStats.UniqueVisits++;
            } else
            {
                AllTimeStats.VisitsHistory[mac] = AllTimeStats.VisitsHistory[mac]++;
            }
            TodayStats.Visits++;
            AllTimeStats.Visits++;
            GetPlayerGlobalStats(mac, name);
            if (RecentPlayersGlobalStatistic.ContainsKey(mac))
            {
                RecentPlayersGlobalStatistic[mac].Visits++;
                RecentPlayersGlobalStatistic[mac].Name = name;
            }
        }

        public static PlayerStatistic GetPlayerGlobalStats(string MAC, string Name = "")
        {
            PlayerStatistic Stat;
            if (RecentPlayersGlobalStatistic.TryGetValue(MAC, out Stat))
            {
                return Stat;
            }
            Stat = LoadPlayerGlobalStats(MAC, Name);
            RecentPlayersGlobalStatistic.Add(MAC, Stat);
            return Stat;
        }

        public static void AddPickedGear(string MAC)
        {
            PlayerStatistic Stat;
            if (TodayStats.Players.TryGetValue(MAC, out Stat))
            {
                Stat.Looted.GearsPicked++;
                TodayStats.Looted.GearsPicked++;
                TodayStats.Players[MAC] = Stat;
            }
            GetPlayerGlobalStats(MAC);
            if (RecentPlayersGlobalStatistic.ContainsKey(MAC))
            {
                RecentPlayersGlobalStatistic[MAC].Looted.GearsPicked++;
            }
            AllTimeStats.Looted.GearsPicked++;
        }
        public static void AddLootedContainer(string MAC)
        {
            PlayerStatistic Stat;
            if (TodayStats.Players.TryGetValue(MAC, out Stat))
            {
                Stat.Looted.ContainersLooted++;
                TodayStats.Looted.ContainersLooted++;
                TodayStats.Players[MAC] = Stat;
            }
            GetPlayerGlobalStats(MAC);
            if (RecentPlayersGlobalStatistic.ContainsKey(MAC))
            {
                RecentPlayersGlobalStatistic[MAC].Looted.ContainersLooted++;
            }
            AllTimeStats.Looted.ContainersLooted++;
        }
        public static void AddPlantHarvested(string MAC)
        {
            PlayerStatistic Stat;
            if (TodayStats.Players.TryGetValue(MAC, out Stat))
            {
                Stat.Looted.PlantsHarvested++;
                TodayStats.Looted.PlantsHarvested++;
                TodayStats.Players[MAC] = Stat;
            }
            GetPlayerGlobalStats(MAC);
            if (RecentPlayersGlobalStatistic.ContainsKey(MAC))
            {
                RecentPlayersGlobalStatistic[MAC].Looted.PlantsHarvested++;
            }
            AllTimeStats.Looted.PlantsHarvested++;
        }
        public static void AddDeath(string MAC)
        {
            PlayerStatistic Stat;
            if (TodayStats.Players.TryGetValue(MAC, out Stat))
            {
                Stat.Deaths++;
                TodayStats.Players[MAC] = Stat;
            }
            TodayStats.Deaths++;
            GetPlayerGlobalStats(MAC);
            if (RecentPlayersGlobalStatistic.ContainsKey(MAC))
            {
                RecentPlayersGlobalStatistic[MAC].Deaths++;
            }
            AllTimeStats.Deaths++;
        }

        public static ExpeditionsProgressData GetExpeditionsProgress(string MAC)
        {
            PlayerStatistic Stat = GetPlayerGlobalStats(MAC);
            ExpeditionsProgressData Data = new ExpeditionsProgressData();
            if (Stat != null)
            {
                int TotalExpeditions = 0;
                int CompletedExpeditions = 0;
                for (int RegionIndex = -Shared.GameRegionNegativeOffset; RegionIndex <= Shared.GameRegionPositiveOffset; RegionIndex++)
                {
                    string RegionName = ExpeditionBuilder.GetRegionString(RegionIndex);
                    int TotalExpeditionsOnRegion = 0;
                    int CompletedExpeditionsOnRegion = 0;
                    int RegionProgress = 0;

                    List<ExpeditionBuilder.ExpeditionTaskTemplate> Expeditions;
                    if (ExpeditionBuilder.m_ExpeditionTasks.TryGetValue(RegionIndex, out Expeditions))
                    {
                        foreach (ExpeditionBuilder.ExpeditionTaskTemplate Expedition in Expeditions)
                        {
                            if (Expedition.m_CanBeTaken)
                            {
                                bool Completed = Stat.CompletedExpeditions.ContainsKey(Expedition.m_Alias);
                                TotalExpeditions++;
                                TotalExpeditionsOnRegion++;
                                if (Completed)
                                {
                                    CompletedExpeditionsOnRegion++;
                                    CompletedExpeditions++;
                                }
                            }
                        }
                        RegionProgress = (int)((double)CompletedExpeditionsOnRegion / TotalExpeditionsOnRegion * 100);
                        Data.ExpeditionsProgress.Add(RegionIndex, RegionProgress);
                    } else
                    {
                        continue;
                    }
                }
                Data.TotalProgress = (int)((double)CompletedExpeditions / TotalExpeditions * 100);
            }
            return Data;
        }

        public static void AddExpedition(string MAC, string Alias)
        {
            PlayerStatistic Stat;
            if (TodayStats.Players.TryGetValue(MAC, out Stat))
            {
                Stat.ExpeditionsCompleted++;
                if (!Stat.CompletedExpeditions.ContainsKey(Alias))
                {
                    Stat.CompletedExpeditions.Add(Alias, true);
                }
                TodayStats.Players[MAC] = Stat;
            }
            TodayStats.ExpeditionsCompleted++;
            GetPlayerGlobalStats(MAC);
            if (RecentPlayersGlobalStatistic.ContainsKey(MAC))
            {
                RecentPlayersGlobalStatistic[MAC].ExpeditionsCompleted++;

                if (!RecentPlayersGlobalStatistic[MAC].CompletedExpeditions.ContainsKey(Alias))
                {
                    RecentPlayersGlobalStatistic[MAC].CompletedExpeditions.Add(Alias, true);
                }
            }
            AllTimeStats.ExpeditionsCompleted++;
        }
        public static void AddCrashSite(string MAC)
        {
            PlayerStatistic Stat;
            if (TodayStats.Players.TryGetValue(MAC, out Stat))
            {
                Stat.ExpeditionsCompleted++;
                TodayStats.Players[MAC] = Stat;
            }
            TodayStats.CrashSitesFound++;
            GetPlayerGlobalStats(MAC);
            if (RecentPlayersGlobalStatistic.ContainsKey(MAC))
            {
                RecentPlayersGlobalStatistic[MAC].CrashSitesFound++;
            }
            AllTimeStats.CrashSitesFound++;
        }
        public static void AddRegionTime(string MAC, int Region)
        {
            if(Region == (int) Shared.GameRegion.RandomRegion)
            {
                return;
            }
            
            PlayerStatistic Stat;
            if (TodayStats.Players.TryGetValue(MAC, out Stat))
            {
                PlayTime RegionTime;
                if (Stat.RegionsHistory.TryGetValue(Region, out RegionTime))
                {
                    RegionTime.AddSecond();
                    Stat.RegionsHistory[Region] = RegionTime;
                } else
                {
                    Stat.RegionsHistory.Add(Region, new PlayTime());
                }
                Stat.TotalPlayTime.AddSecond();

                TodayStats.Players[MAC] = Stat;
            }
            PlayTime TodayRegionTime;
            if (TodayStats.RegionsHistory.TryGetValue(Region, out TodayRegionTime))
            {
                TodayRegionTime.AddSecond();
                TodayStats.RegionsHistory[Region] = TodayRegionTime;
            } else
            {
                TodayStats.RegionsHistory.Add(Region, new PlayTime());
            }
            GetPlayerGlobalStats(MAC);
            if (RecentPlayersGlobalStatistic.ContainsKey(MAC))
            {
                PlayerStatistic GlobalStat = RecentPlayersGlobalStatistic[MAC];
                PlayTime GlobalRegionTime;
                if (GlobalStat.RegionsHistory.TryGetValue(Region, out GlobalRegionTime))
                {
                    GlobalRegionTime.AddSecond();
                    GlobalStat.RegionsHistory[Region] = GlobalRegionTime;
                } else
                {
                    GlobalStat.RegionsHistory.Add(Region, new PlayTime());
                }
                GlobalStat.TotalPlayTime.AddSecond();
                RecentPlayersGlobalStatistic[MAC] = GlobalStat;
            }
            PlayTime AllTimeRegionTime;
            if (AllTimeStats.RegionsHistory.TryGetValue(Region, out AllTimeRegionTime))
            {
                AllTimeRegionTime.AddSecond();
                AllTimeStats.RegionsHistory[Region] = AllTimeRegionTime;
            } else
            {
                AllTimeStats.RegionsHistory.Add(Region, new PlayTime());
            }
        }
        public static void SetName(string MAC, string Name)
        {
            PlayerStatistic Stat;
            if (TodayStats.Players.TryGetValue(MAC, out Stat))
            {
                Stat.Name = Name;
                TodayStats.Players[MAC] = Stat;
            }
            GetPlayerGlobalStats(MAC);
            if (RecentPlayersGlobalStatistic.ContainsKey(MAC))
            {
                RecentPlayersGlobalStatistic[MAC].Name = Name;
            }
        }

        public static Dictionary<string, int> GetGraphDummy()
        {
            Dictionary<string, int> ActivitySnapshots = new Dictionary<string, int>();

            int Hour = 11;
            int Minute = 0;
            string TimeKey = Hour + ":" + Minute;
            System.Random RNG = new System.Random();

            while (Hour != 0)
            {
                if(Minute == 0)
                {
                    Minute = 30;
                } else
                {
                    Minute = 0;
                    Hour++;
                }
                if(Hour > 24)
                {
                    Hour = 0;
                }
                TimeKey = Hour + ":" + Minute;
                int Players = RNG.Next(0, Server.MaxPlayers);
                //Log("Add Dummy " + TimeKey+" Players "+ Players);
                if (!ActivitySnapshots.ContainsKey(TimeKey))
                {
                    
                    ActivitySnapshots.Add(TimeKey, Players);
                } 
            }
            return ActivitySnapshots;
        }


        public static void EverySecond()
        {
            if (!Started)
            {
                return;
            }
            if (DataDay != 0)
            {
                System.DateTime DT = System.DateTime.Now;
                if (DataDay != DT.Day)
                {
                    SaveRecentStuff();
                    StartDay(true);
                } else
                {
                    if(DT.Minute == 0 || DT.Minute == 30)
                    {
                        string TimeKey = DT.Hour + ":" + DT.Minute;
                        string DayKey = DT.Day + "." + DT.Month+ "." + DT.Year;
                        bool NeedSave = false;

                        if(TodayStats.ActivitySnapshots.ContainsKey(DayKey))
                        {
                            if (!TodayStats.ActivitySnapshots[DayKey].ContainsKey(TimeKey))
                            {
                                TodayStats.ActivitySnapshots[DayKey].Add(TimeKey, MyMod.PlayersOnServer);
                                NeedSave = true;
                            }
                        } else
                        {
                            TodayStats.ActivitySnapshots.Add(DayKey, new Dictionary<string, int>());
                            TodayStats.ActivitySnapshots[DayKey].Add(TimeKey, MyMod.PlayersOnServer);
                            NeedSave = true;
                        }
                        if (AllTimeStats.ActivitySnapshots.ContainsKey(DayKey))
                        {
                            if (!AllTimeStats.ActivitySnapshots[DayKey].ContainsKey(TimeKey))
                            {
                                AllTimeStats.ActivitySnapshots[DayKey].Add(TimeKey, MyMod.PlayersOnServer);
                                NeedSave = true;
                            }
                        } else
                        {
                            AllTimeStats.ActivitySnapshots.Add(DayKey, new Dictionary<string, int>());
                            AllTimeStats.ActivitySnapshots[DayKey].Add(TimeKey, MyMod.PlayersOnServer);
                            NeedSave = true;
                        }

                        if (NeedSave)
                        {
                            SaveRecentStuff();
                        }
                    }
                }
            }
            foreach (var c in Server.clients)
            {
                Client client = c.Value;
                if (client.IsBusy() && !client.RCON && !string.IsNullOrEmpty(client.SubNetworkGUID) && !Shared.ClientIsLoading(client.id))
                {
                    DataStr.MultiPlayerClientData Data;
                    if (MyMod.playersData[client.id] != null)
                    {
                        Data = MyMod.playersData[client.id];
                        AddRegionTime(client.SubNetworkGUID, Data.m_LastRegion);
                    }
                }
            }
#if (!DEDICATED)
            AddRegionTime(Server.GetMACByID(0), (int) MyMod.GetCurrentRegion());
#endif
            if (MyMod.PlayersOnServer == 0)
            {
                TodayStats.EmptyTime.AddSecond();
                AllTimeStats.EmptyTime.AddSecond();
            }
            TodayStats.OnlineTime.AddSecond();
            AllTimeStats.OnlineTime.AddSecond();
            if (SaveRecentTimer < SaveRecentPerioud)
            {
                SaveRecentTimer++;

                if(SaveRecentTimer >= SaveRecentPerioud)
                {
                    SaveRecentTimer = 0;
                    SaveRecentStuff();
                }
            }
        }
    }
}
