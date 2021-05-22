using System;
using System.Reflection;
using Harmony;
using UnityEngine;
using IL2CPP = Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

using GameServer;

namespace SkyCoop
{
    public class MenuChange
    {
        public static void MoveUpMainMenuWordmark(int numOfMainMenuItems)
        {
            GameObject.Find("Panel_MainMenu/MainPanel/Main/TLD_wordmark").transform.localPosition += new Vector3(0, (numOfMainMenuItems - 6) * 30, 0);
        }

        public static void AddButton(Panel_MainMenu __instance, string name, int order, int plus)
        {
            Panel_MainMenu.MainMenuItem mainMenuItem = new Panel_MainMenu.MainMenuItem();
            mainMenuItem.m_LabelLocalizationId = name;
            mainMenuItem.m_Type = Panel_MainMenu.MainMenuItem.MainMenuItemType.Story + plus;
            __instance.m_MenuItems.Insert(order, mainMenuItem);

            string id = __instance.m_MenuItems[order].m_Type.ToString();
            int type = (int)__instance.m_MenuItems[order].m_Type;

            __instance.m_BasicMenu.AddItem(id, type, order, name, "", "", new Action(__instance.OnSandbox), Color.gray, Color.white);
        }

        public static void FixUpButtonStrings(Panel_PauseMenu __instance)
        {
            if (__instance.gameObject != null)
            {
                if (__instance.gameObject.transform.GetChild(1) != null && __instance.gameObject.transform.GetChild(1).GetChild(0) != null)
                {
                    Transform Menu = __instance.gameObject.transform.GetChild(1).GetChild(0);
                    if (Menu.transform.GetChild(2) != null && Menu.transform.GetChild(2).GetChild(0) != null)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            GameObject Button = Menu.transform.GetChild(2).GetChild(i).gameObject;
                            GameObject Lable = Button.transform.GetChild(0).gameObject;

                            string text = "";

                            if(i == 0)
                            {
                                text = "HOST";
                            }else if(i == 1)
                            {
                                text = "MANUAL CONNECT";
                            }else if (i == 2)
                            {
                                text = "DISCONNECT";
                            }

                            if (Lable.GetComponent<UILabel>() != null)
                            {
                                UILabel UiLab = Lable.GetComponent<UILabel>();
                                UiLab.mText = text;
                                UiLab.text = text;
                                UiLab.ProcessText();
                            }
                            if (Button.GetComponent<UIButton>() != null)
                            {
                                if(Button.GetComponent<MyMod.UiButtonPressHook>() == null)
                                {
                                    Button.AddComponent<MyMod.UiButtonPressHook>();
                                    Button.GetComponent<MyMod.UiButtonPressHook>().m_CustomId = i;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void AddButtonPause(Panel_PauseMenu __instance, string name, int order)
        {
            Panel_PauseMenu.PauseMenuItemType mainMenuItem = new Panel_PauseMenu.PauseMenuItemType();

            __instance.m_MenuItems.Insert(order, mainMenuItem);
            __instance.m_BasicMenu.AddItem("", 0, order, name, "", "", new Action(__instance.Update), Color.gray, Color.white);
        }

        [HarmonyPatch(typeof(Panel_MainMenu), "Start", null)]
        public class Panel_MainMenu_Start
        {
            public static void Postfix(Panel_MainMenu __instance)
            {
                if (!InterfaceManager.IsMainMenuActive()) return;

                AddButton(__instance, "Donaters", 1, 10);
                AddButton(__instance, "Connect", 5, 9);

                MoveUpMainMenuWordmark(Convert.ToInt16(__instance.m_BasicMenu.m_MenuItems.Count.ToString()));
            }
        }
        [HarmonyPatch(typeof(Panel_PauseMenu), "Start", null)]
        public class Panel_PauseMenu_Start
        {
            public static void Postfix(Panel_PauseMenu __instance)
            {
                AddButtonPause(__instance, "HOST A SERVER", 0);
                AddButtonPause(__instance, "MANUAL CONNECT", 1);
                AddButtonPause(__instance, "DISCONNECT", 2);

                if(__instance.gameObject != null && __instance.gameObject.transform.GetChild(4))
                {
                    GameObject Icons = __instance.gameObject.transform.GetChild(4).gameObject;
                    Icons.transform.position = new Vector3(Icons.transform.position.x, 0.2f, Icons.transform.position.z);
                }
            }
        }
        [HarmonyPatch(typeof(Panel_PauseMenu), "Update", null)]
        public class Panel_PauseMenu_Update
        {
            public static void Postfix(Panel_PauseMenu __instance)
            {
                FixUpButtonStrings(__instance);
            }
        }

        [HarmonyPatch(typeof(BasicMenu), "InternalClickAction", null)]
        internal class BasicMenu_InternalClickAction_Pre
        {
            private static void Prefix(int index, BasicMenu __instance)
            {
                IL2CPP.List<BasicMenu.BasicMenuItemModel> list = __instance.m_ItemModelList;

                if (list[index].m_LabelText == "Donaters")
                {
                    Application.OpenURL("https://filigrani.github.io/SkyCoop/");
                }
                if (list[index].m_LabelText == "Connect")
                {
                    //For Debug Alone
                    //MyMod.SaveSlotSync SaveData = new MyMod.SaveSlotSync();
                    //SaveData.m_SaveSlotType = (int)SaveSlotType.SANDBOX;
                    //MyMod.PendingSave = SaveData;
                    //MyMod.CheckHaveSaveFileToJoin(SaveData);

                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input server address", "127.0.0.1", Panel_Confirmation.ButtonLayout.Button_2, "Connect", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                }
            }
        }
        [HarmonyPatch(typeof(UIButton), "OnClick")]
        internal class UIButton_Press
        {
            private static void Prefix(UIButton __instance)
            {
                if(__instance.gameObject != null && __instance.gameObject.GetComponent<MyMod.UiButtonPressHook>() != null)
                {
                    int CustomId = __instance.gameObject.GetComponent<MyMod.UiButtonPressHook>().m_CustomId;
                    if(CustomId == 0)
                    {
                        MyMod.HostAServer();
                    }else if (CustomId == 1)
                    {
                        MyMod.DoIPConnectWindow();
                    }else if (CustomId == 2)
                    {
                        MelonLogger.Log("Disconnect case pressed disconnect button");
                        MyMod.Disconnect();
                    }
                }
            }
        }
    }
}

