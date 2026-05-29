using Il2Cpp;
using SkyCoop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkyCoopClient
{
    public static class DebugGUI
    {
        public static State s_State = State.SelectTool;
        public static bool s_Open = false;
        public static Vector2 s_ScrollPosition = Vector2.zero;

        public enum State
        {
            SelectTool,
            SpawnPoints,
            RadialSpawners,
            Props,
        }

        public static void Toggle()
        {
            s_Open = !s_Open;
        }

        public static void SetState(State State)
        {
            s_State = State;
            s_ScrollPosition = Vector2.zero;
        }

        public static void OnSetTable()
        {
            string text = InterfaceManager.GetPanel<Panel_Confirmation>().m_CurrentGroup.m_InputField.GetText();
            RadialLootSpawnersEditor.s_SpawnerLootTable = text;
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
                    float _listItemHeight = 25;
                    float _listItemWidth = 120;

                    int RootElementsCount = 3;

                    GUI.Box(new Rect(10, 10, _listItemWidth, 90), "Debug tools");

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
                                if(GUI.Button(new Rect(10, yPos, 120, _listItemHeight - 2), "SpawnPoints"))
                                {
                                    SetState(State.SpawnPoints);
                                    SpawnPointEditor.UpdateVizualization();
                                }
                                break;
                            case 1:
                                if (GUI.Button(new Rect(10, yPos, 120, _listItemHeight - 2), "RadialSpawners"))
                                {
                                    SetState(State.RadialSpawners);
                                    RadialLootSpawnersEditor.UpdateVizualization();
                                }
                                break;
                            case 2:
                                if (GUI.Button(new Rect(10, yPos, 120, _listItemHeight - 2), "Props"))
                                {
                                    SetState(State.Props);
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

                    if (GUI.Button(new Rect(20, 40, 80, 25), "Load"))
                    {
                        SpawnPointEditor.LoadCurrentSceneFile();
                    }
                    if (GUI.Button(new Rect(110, 40, 80, 25), "Save"))
                    {
                        SpawnPointEditor.Save();
                    }
                    if (GUI.Button(new Rect(200, 40, 80, 25), "Back"))
                    {
                        SpawnPointEditor.DeleteVizualization();
                        SetState(State.SelectTool);
                    }
                    float scrollViewHeight = 200;
                    float listItemHeight = 25;
                    float listItemWidth = 260;

                    float contentHeight = SpawnPointEditor.m_Vectors.Count * listItemHeight;

                    s_ScrollPosition = GUI.BeginScrollView(
                        new Rect(20, 75, listItemWidth + 20, scrollViewHeight),
                        s_ScrollPosition,
                        new Rect(0, 0, listItemWidth, contentHeight)
                    );

                    for (int i = 0; i < SpawnPointEditor.m_Vectors.Count; i++)
                    {
                        float yPos = i * listItemHeight;

                        GUI.Label(new Rect(5, yPos, 100, listItemHeight),
                            $"{SpawnPointEditor.m_Vectors[i]}");

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

                    float createButtonY = 75 + scrollViewHeight + 10;
                    if (GUI.Button(new Rect(20, createButtonY, 100, 30), "Create"))
                    {
                        SpawnPointEditor.AddSpawnPoint();
                    }

                    break;
                case State.RadialSpawners:

                    GUI.Box(new Rect(10, 10, 300, 40 * 12), "RadialSpawners");

                    if (GUI.Button(new Rect(20, 40, 80, 25), "Load"))
                    {
                        RadialLootSpawnersEditor.LoadCurrentSceneFile();
                    }
                    if (GUI.Button(new Rect(110, 40, 80, 25), "Save"))
                    {
                        RadialLootSpawnersEditor.Save();
                    }
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
                            $"{RadialLootSpawnersEditor.s_Spawners[i].center.ToVector()}");

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
                        InterfaceManager.GetPanel<Panel_Confirmation>().AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input name of any Loot Table from\n Mods\\SkyModData\\LootTables", "", Panel_Confirmation.ButtonLayout.Button_2, "GAMEPLAY_Sumbit", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, new Action(OnSetTable), null);
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
                default:
                    break;
            }
        }
    }
}
