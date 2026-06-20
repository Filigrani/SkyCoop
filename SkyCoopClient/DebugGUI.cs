using AsmResolver.PE.File;
using Il2Cpp;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine;
using static SkyCoopServer.DataStr;

namespace SkyCoopClient
{
    public static class DebugGUI
    {
        public static State s_State = State.Maps;
        public static bool s_Open = false;
        public static Vector2 s_ScrollPosition = Vector2.zero;
        public static List<string> s_Maps = new List<string>();
        public static string s_CurrentLoadedMapPath = string.Empty;
        public static MapData s_CurrentMapData = null;

        public enum State
        {
            SelectTool,
            SpawnPoints,
            RadialSpawners,
            Props,
            Maps,
            Victory,
            Zone,
        }

        public static void Toggle()
        {
            s_Open = !s_Open;

            if(s_Open && s_State == State.Maps)
            {
                RefreshMaps();
            }
        }

        public static void SetState(State State)
        {
            s_State = State;
            s_ScrollPosition = Vector2.zero;
        }

        public static void OnSetTable()
        {
            MenuHook.RemovePleaseWait();
            string text = InterfaceManager.GetPanel<Panel_Confirmation>().m_CurrentGroup.m_InputField.GetText();
            RadialLootSpawnersEditor.s_SpawnerLootTable = text;
        }

        public static bool SaveMapToFile(string Path, DataStr.MapData Data)
        {
            JsonSerializerOptions Options = new JsonSerializerOptions();
            Options.WriteIndented = true;
            string JSON = JsonSerializer.Serialize<MapData>(Data, Options);
            try
            {
                File.WriteAllText(Path, JSON);
                return true;
            }
            catch (Exception e)
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"Cant save file because has error: {e.Message}");
            }
            return false;
        }

        public static void LoadMap(string FileName)
        {
            DataStr.MapData Data = FilesManager.GetMapData(FileName);

            if (Data != null)
            {
                s_CurrentLoadedMapPath = $"{FilesManager.s_DataDirectory}/{FilesManager.s_MapsDirectory}/{FileName}";
                LoadMap(Data);
            }
        }

        public static void LoadMap(DataStr.MapData Data)
        {
            s_CurrentMapData = Data;
            if (!string.IsNullOrEmpty(Data.Scene))
            {
                ModMain.ChangeMap(Data.Scene);
            }
            SpawnPointEditor.Load(s_CurrentMapData.SpawnPoints);
            RadialLootSpawnersEditor.Load(s_CurrentMapData.RadialLootSpawners);
            PropsSpawnsEditor.Load(s_CurrentMapData.Props);
            VictoryPointEditor.Load(s_CurrentMapData.VictoryPoint);
            ZoneEditor.Load(s_CurrentMapData.ZoneConfig);

            // Всегда отображем зону, что бы видить границы для раставления спавнов и т.п.
            ZoneEditor.UpdateVizualization();
        }

        public static void SaveMap()
        {
            if (s_CurrentMapData != null && !string.IsNullOrEmpty(s_CurrentLoadedMapPath))
            {
                s_CurrentMapData.SpawnPoints = SpawnPointEditor.Save();
                s_CurrentMapData.RadialLootSpawners = RadialLootSpawnersEditor.Save();
                s_CurrentMapData.Props = PropsSpawnsEditor.Save();
                s_CurrentMapData.VictoryPoint = VictoryPointEditor.Save();
                s_CurrentMapData.ZoneConfig = ZoneEditor.Save();
                SaveMapToFile(s_CurrentLoadedMapPath, s_CurrentMapData);
            }
        }

        public static bool CreateNewMap(string MapName)
        {
            string Path = $"{FilesManager.s_DataDirectory}/{FilesManager.s_MapsDirectory}/{MapName}";

            if (File.Exists(Path))
            {
                return false;
            }
            return true;
        }

        public static void OnNewMap()
        {
            string text = InterfaceManager.GetPanel<Panel_Confirmation>().m_CurrentGroup.m_InputField.GetText();
            MenuHook.RemovePleaseWait();
            if (CreateNewMap(text))
            {
                s_CurrentMapData = new MapData();
                s_CurrentLoadedMapPath = $"{FilesManager.s_DataDirectory}/{FilesManager.s_MapsDirectory}/{text}";
                LoadMap(s_CurrentMapData);
                InterfaceManager.GetPanel<Panel_Confirmation>().AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input scene for map", ModMain.GetCurrentSceneName(), Panel_Confirmation.ButtonLayout.Button_2, "GAMEPLAY_Confirm", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, new Action(OnSetScene), null);
                SetState(State.SelectTool);
            }
            else
            {
                MenuHook.DoOKMessage("Error", "Map with such name already exist!");
            }
        }

        public static void OnSetScene()
        {
            string text = InterfaceManager.GetPanel<Panel_Confirmation>().m_CurrentGroup.m_InputField.GetText();
            MenuHook.RemovePleaseWait();
            if(s_CurrentMapData != null)
            {
                s_CurrentMapData.Scene = text;
                ModMain.ChangeMap(text);
            }
        }

        public static void RefreshMaps()
        {
            s_Maps.Clear();

            s_Maps = new List<string>(FilesManager.GetMapsList());
        }

        public static string Truncate(Vector3 v3)
        {
            return $"{(int)v3.x},{(int)v3.y},{(int)v3.z}";
        }
        public static string Truncate(DataStr.Vector3JSON v3)
        {
            return Truncate(v3.GetVector3Unity());
        }
        public static string Truncate(DataStr.V3Quat v3)
        {
            return Truncate(v3.m_Position.ConvertToUnity());
        }
        public static string Truncate(DataStr.V3QuatJSON v3)
        {
            return Truncate(v3.position.GetVector3Unity());
        }

        public static void Render()
        {
            if (!s_Open)
            {
                return;
            }
            
            switch (s_State)
            {
                case State.SelectTool:
                    float _scrollViewHeight = 200;
                    float _listItemHeight = 35;
                    float _listItemWidth = 135;

                    int RootElementsCount = 6;

                    GUI.Box(new Rect(10, 10, _listItemWidth+30, 230), "");

                    s_ScrollPosition = GUI.BeginScrollView(
                        new Rect(10, 30, _listItemWidth + 20, _scrollViewHeight),
                        s_ScrollPosition,
                        new Rect(0, 0, _listItemWidth, RootElementsCount * _listItemHeight)
                    );

                    for (int i = 0; i <= RootElementsCount; i++)
                    {
                        float yPos = i * _listItemHeight;

                        switch (i)
                        {
                            case 0:
                                if (GUI.Button(new Rect(10, yPos, 120, _listItemHeight - 2), "File"))
                                {
                                    SetState(State.Maps);
                                    RefreshMaps();
                                }
                                break;
                            case 1:
                                if(GUI.Button(new Rect(10, yPos, 120, _listItemHeight - 2), "SpawnPoints"))
                                {
                                    SetState(State.SpawnPoints);
                                    SpawnPointEditor.UpdateVizualization();
                                }
                                break;
                            case 2:
                                if (GUI.Button(new Rect(10, yPos, 120, _listItemHeight - 2), "RadialSpawners"))
                                {
                                    SetState(State.RadialSpawners);
                                    RadialLootSpawnersEditor.UpdateVizualization();
                                }
                                break;
                            case 3:
                                if (GUI.Button(new Rect(10, yPos, 120, _listItemHeight - 2), "Props"))
                                {
                                    SetState(State.Props);
                                }
                                break;
                            case 4:
                                if (GUI.Button(new Rect(10, yPos, 120, _listItemHeight - 2), "Victory Point"))
                                {
                                    SetState(State.Victory);
                                    VictoryPointEditor.UpdateVizualization();
                                }
                                break;
                            case 5:
                                if (GUI.Button(new Rect(10, yPos, 120, _listItemHeight - 2), "Zone Config"))
                                {
                                    SetState(State.Zone);
                                    ZoneEditor.UpdateVizualization();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    GUI.EndScrollView();
                    break;
                case State.SpawnPoints:
                    GUI.Box(new Rect(10, 10, 300, 40 * 9), "SpawnPoints");

                    if (GUI.Button(new Rect(20, 40, 80, 25), "Add New"))
                    {
                        SpawnPointEditor.AddSpawnPoint();
                    }
                    if (GUI.Button(new Rect(200, 40, 80, 25), "Back"))
                    {
                        SpawnPointEditor.DeleteVizualization();
                        SetState(State.SelectTool);
                    }
                    float scrollViewHeight = 200;
                    float listItemHeight = 25;
                    float listItemWidth = 260;

                    float contentHeight = SpawnPointEditor.m_Points.Count * listItemHeight;

                    s_ScrollPosition = GUI.BeginScrollView(
                        new Rect(20, 75, listItemWidth + 20, scrollViewHeight),
                        s_ScrollPosition,
                        new Rect(0, 0, listItemWidth, contentHeight)
                    );

                    for (int i = 0; i < SpawnPointEditor.m_Points.Count; i++)
                    {
                        float yPos = i * listItemHeight;

                        GUI.Label(new Rect(5, yPos, 100, listItemHeight),
                            $"{Truncate(SpawnPointEditor.m_Points[i])}");

                        if (GUI.Button(new Rect(110, yPos, 60, listItemHeight - 2), "GoTo"))
                        {
                            SpawnPointEditor.Teleport(i);
                        }

                        if (GUI.Button(new Rect(175, yPos, 60, listItemHeight - 2), "Delete"))
                        {
                            SpawnPointEditor.Delete(i);
                            break;
                        }
                    }

                    // End scroll view
                    GUI.EndScrollView();

                    break;
                case State.RadialSpawners:

                    GUI.Box(new Rect(10, 10, 300, 40 * 12), "RadialSpawners");

                    if (GUI.Button(new Rect(200, 40, 80, 25), "Back"))
                    {
                        RadialLootSpawnersEditor.DeleteVizualization();
                        SetState(State.SelectTool);
                    }
                    float __scrollViewHeight = 200;
                    float __listItemHeight = 25;
                    float __listItemWidth = 260;

                    float __contentHeight = RadialLootSpawnersEditor.s_Spawners.Count * __listItemHeight;

                    s_ScrollPosition = GUI.BeginScrollView(
                        new Rect(20, 75, __listItemWidth + 20, __scrollViewHeight),
                        s_ScrollPosition,
                        new Rect(0, 0, __listItemWidth, __contentHeight)
                    );

                    for (int i = 0; i < RadialLootSpawnersEditor.s_Spawners.Count; i++)
                    {
                        float yPos = i * __listItemHeight;

                        GUI.Label(new Rect(5, yPos, 100, __listItemHeight),
                            $"{Truncate(RadialLootSpawnersEditor.s_Spawners[i].center)}");

                        if (GUI.Button(new Rect(110, yPos, 60, __listItemHeight - 2), "GoTo"))
                        {
                            RadialLootSpawnersEditor.Teleport(i);
                        }

                        if (GUI.Button(new Rect(175, yPos, 60, __listItemHeight - 2), "Delete"))
                        {
                            RadialLootSpawnersEditor.Delete(i);
                            break;
                        }
                    }

                    // End scroll view
                    GUI.EndScrollView();

                    float __createButtonY = 75 + __scrollViewHeight + 10;
                    if (GUI.Button(new Rect(20, __createButtonY, 100, 30), "Create"))
                    {
                        RadialLootSpawnersEditor.CreateSpawner();
                    }

                    GUI.Label(new Rect(20, __createButtonY + 40, 60, 20), "Range:");
                    RadialLootSpawnersEditor.s_SpawnerRange = GUI.HorizontalSlider(
                        new Rect(85, __createButtonY + 45, 150, 20),
                        RadialLootSpawnersEditor.s_SpawnerRange,
                        1f,
                        5f
                    );
                    GUI.Label(new Rect(240, __createButtonY + 40, 30, 20),
                        RadialLootSpawnersEditor.s_SpawnerRange.ToString("F1"));

                    GUI.Label(new Rect(20, __createButtonY + 70, 100, 20), "Peak:");
                    RadialLootSpawnersEditor.s_UpwardRaycastLength = GUI.HorizontalSlider(
                        new Rect(85, __createButtonY + 75, 150, 20),
                        RadialLootSpawnersEditor.s_UpwardRaycastLength,
                        1f,
                        5f
                    );
                    GUI.Label(new Rect(240, __createButtonY + 70, 30, 20),
                        RadialLootSpawnersEditor.s_UpwardRaycastLength.ToString("F1"));

                    GUI.Label(new Rect(20, __createButtonY + 100, 300, 20), $"Current Table: {RadialLootSpawnersEditor.s_SpawnerLootTable}");
                    if (GUI.Button(new Rect(20, __createButtonY + 120, 100, 30), "Set Table"))
                    {
                        InterfaceManager.GetPanel<Panel_Confirmation>().AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input name of any Loot Table from\n Mods/SkyModData/LootTables", "", Panel_Confirmation.ButtonLayout.Button_2, "GAMEPLAY_Sumbit", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, new Action(OnSetTable), null);
                    }
                    break;

                case State.Props:
                    GUI.Box(new Rect(10, 10, 300, 40 * 12), "Props");

                    GUI.Label(new Rect(20, 40, 80, 25), "Work In-Progress");

                    if (GUI.Button(new Rect(200, 40, 80, 25), "Back"))
                    {
                        SetState(State.SelectTool);
                    }
                    break;
                case State.Maps:
                    GUI.Box(new Rect(10, 10, 300, 40 * 9), "File");

                    if (GUI.Button(new Rect(20, 40, 80, 25), "Refresh"))
                    {
                        RefreshMaps();
                    }


                    if (!string.IsNullOrEmpty(s_CurrentLoadedMapPath))
                    {
                        if (GUI.Button(new Rect(110, 40, 80, 25), "Save"))
                        {
                            SaveMap();
                        }
                        if (GUI.Button(new Rect(200, 40, 80, 25), "Back"))
                        {
                            SetState(State.SelectTool);
                        }
                    }

                    float scrollViewHeight__ = 200;
                    float listItemHeight__ = 25;
                    float listItemWidth__ = 260;

                    float contentHeight__ = SpawnPointEditor.m_Points.Count * listItemHeight__;

                    s_ScrollPosition = GUI.BeginScrollView(
                        new Rect(20, 75, listItemWidth__ + 20, scrollViewHeight__),
                        s_ScrollPosition,
                        new Rect(0, 0, listItemWidth__, contentHeight__)
                    );

                    for (int i = 0; i < s_Maps.Count; i++)
                    {
                        float yPos = i * listItemHeight__;

                        GUI.Label(new Rect(5, yPos, 200, listItemHeight__),
                            $"{s_Maps[i]}");


                        if (GUI.Button(new Rect(175, yPos, 60, listItemHeight__ - 2), "Load"))
                        {
                            LoadMap(s_Maps[i]);
                            break;
                        }
                    }

                    // End scroll view
                    GUI.EndScrollView();

                    float createButtonY_ = 75 + scrollViewHeight__ + 10;
                    if (GUI.Button(new Rect(20, createButtonY_, 100, 30), "New Map"))
                    {
                        InterfaceManager.GetPanel<Panel_Confirmation>().AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input name for map", "", Panel_Confirmation.ButtonLayout.Button_2, "GAMEPLAY_Confirm", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, new Action(OnNewMap), null);
                    }

                    break;
                case State.Victory:
                    GUI.Box(new Rect(10, 10, 300, 130), "Victory Point");

                    if (GUI.Button(new Rect(20, 40, 80, 25), "Set"))
                    {
                        VictoryPointEditor.SetPoint();
                    }
                    if (GUI.Button(new Rect(110, 40, 80, 25), "Remove"))
                    {
                        VictoryPointEditor.m_Point = null;
                        VictoryPointEditor.DeleteVizualization();
                    }
                    if (GUI.Button(new Rect(200, 40, 80, 25), "Back"))
                    {
                        VictoryPointEditor.DeleteVizualization();
                        SetState(State.SelectTool);
                    }

                    if (VictoryPointEditor.m_Point != null)
                    {
                        GUI.Label(new Rect(50, 90, 100, 25), $"{Truncate(VictoryPointEditor.m_Point)}");
                    }
                    break;
                case State.Zone:

                    GUI.Box(new Rect(10, 10, 300, 40 * 12), "Zone Config");

                    if (GUI.Button(new Rect(20, 40, 80, 25), "Set"))
                    {
                        ZoneEditor.SetZone();
                    }
                    if (GUI.Button(new Rect(110, 40, 80, 25), "Remove"))
                    {
                        ZoneEditor.m_Config = null;
                        ZoneEditor.UpdateVizualization();
                    }
                    if (GUI.Button(new Rect(200, 40, 80, 25), "Back"))
                    {
                        SetState(State.SelectTool);
                    }
                    float ___scrollViewHeight = 250;
                    float ___listItemHeight = 50;
                    float ___listItemWidth = 780;

                    int Count = 0;

                    if(ZoneEditor.m_Config != null)
                    {
                        if(ZoneEditor.m_Config.Stages != null)
                        {
                            Count = ZoneEditor.m_Config.Stages.Count;
                        }
                    }

                    float ___contentHeight = Count * ___listItemHeight;

                    s_ScrollPosition = GUI.BeginScrollView(
                        new Rect(20, 75, ___listItemWidth + 20, ___scrollViewHeight),
                        s_ScrollPosition,
                        new Rect(0, 0, ___listItemWidth, ___contentHeight)
                    );

                    if(Count > 0)
                    {
                        for (int i = 0; i < Count; i++)
                        {
                            float yPos = i * ___listItemHeight;

                            GUI.Label(new Rect(5, yPos, ___listItemWidth, ___listItemHeight),
                                $"Shrink {ZoneEditor.m_Config.Stages[i].ShrinkTime}, Time {ZoneEditor.m_Config.Stages[i].StageTime}, Damage {ZoneEditor.m_Config.Stages[i].DamagePerSecond}");
                        }
                    }



                    // End scroll view
                    GUI.EndScrollView();

                    if (ZoneEditor.m_Config != null)
                    {
                        float SliderY = 85 + ___scrollViewHeight;

                        float PreviousRadius = ZoneEditor.m_Config.Stages[0].Radius;

                        GUI.Label(new Rect(20, SliderY, 60, 20), "Radius:");
                        ZoneEditor.m_Config.Stages[0].Radius = GUI.HorizontalSlider(
                            new Rect(85, SliderY + 5, 150, 20),
                            ZoneEditor.m_Config.Stages[0].Radius,
                            0,
                            10000
                        );
                        GUI.Label(new Rect(240, SliderY, 30, 20),
                            ZoneEditor.m_Config.Stages[0].Radius.ToString("F1"));

                        if(PreviousRadius != ZoneEditor.m_Config.Stages[0].Radius)
                        {
                            ZoneEditor.UpdateVizualization();
                        }

                        float ___createButtonY = 125 + ___scrollViewHeight;
                        if (GUI.Button(new Rect(20, ___createButtonY, 100, 30), "Add Stage"))
                        {

                        }
                    }


                    break;
                default:
                    break;
            }
        }
    }
}
