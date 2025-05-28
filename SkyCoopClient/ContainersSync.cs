using Il2Cpp;
using Il2CppNodeCanvas.Tasks.Actions;
using Il2CppTLD.Interactions;
using Il2CppTLD.PDID;
using MelonLoader;
using MonoMod.RuntimeDetour;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace SkyCoopClient
{
    public class ContainersSync
    {
        public static bool s_SendRemoveOnClosed = false;
        public static void HandleContainerOpen(string CompressedJSON)
        {
            Container Container = null;
            if (PlayersManager.s_LastTryInteractionObject)
            {
                string GUID = "";
                ObjectGuid GUIDOBJ = PlayersManager.s_LastTryInteractionObject.GetComponent<ObjectGuid>();
                if (GUIDOBJ)
                {
                    GUID = GUIDOBJ.Get();
                }
                ClientSend.SendInteractionGUID(GUID);
                Container = PlayersManager.s_LastTryInteractionObject.GetComponent<Container>();

                if (Container)
                {
                    Panel_Container Panel = null;
                    if (InterfaceManager.TryGetPanel<Panel_Container>(out Panel))
                    {
                        Container.DestroyAllGear();
                        if (!string.IsNullOrEmpty(CompressedJSON))
                        {
                            if (DataStr.IsBase64String(CompressedJSON))
                            {
                                string JSON = DataStr.DecompressString(CompressedJSON);
                                Container.Deserialize(JSON, new Il2CppSystem.Collections.Generic.List<GearItem>());
                            }
                            else
                            {
                                SkyCoop.Logger.Log(ConsoleColor.Red, "HandleContainerOpen got Non base64 string!");
                            }
                        }
                        Panel.SetContainer(Container, Container.m_LocalizedDisplayName.Text());
                        Panel.Enable(true);
                        Container.m_Inspected = true;
                        Container.m_StartInspected = true;
                        Container.m_GearToInstantiate.Clear();
                        Container.m_NotPopulated = false;
                    }
                }
            }
        }

        public static void HandleContainerClose(Container Container)
        {
            if (Container)
            {
                string GUID = "";
                ObjectGuid GUIDOBJ = Container.GetComponent<ObjectGuid>();
                if (GUIDOBJ)
                {
                    GUID = GUIDOBJ.Get();
                }

                string JSON = Container.Serialize();

                bool IsDeathContainer = Container.GetComponent<Comps.DeathPackComp>();

                s_SendRemoveOnClosed = Container.IsEmpty() && IsDeathContainer;

                if (!IsDeathContainer)
                {
                    if (Container.IsEmpty())
                    {
                        ClientSend.SendContainerState(GUID, 2);
                    }
                    else
                    {
                        ClientSend.SendContainerState(GUID, 1);
                    }
                }

                Container.DestroyAllGear();

                MenuHook.DoPleaseWait("Please wait...", "Sending container data...");

                ClientSend.SendContainerData(GUID, DataStr.CompressString(JSON));

                Container.m_Inspected = true;
                Container.m_StartInspected = true;
            }
        }

        public static void HandleClosePanel()
        {
            Panel_Container Panel = null;

            if(InterfaceManager.TryGetPanel<Panel_Container>(out Panel))
            {
                if (Panel.m_Container)
                {
                    Panel.m_Container.BeginContainerClose();
                    ClientSend.SendFinishInteract();
                    string GUID = "";
                    ObjectGuid GUIDOBJ = PlayersManager.s_LastTryInteractionObject.GetComponent<ObjectGuid>();
                    if (GUIDOBJ)
                    {
                        GUID = GUIDOBJ.Get();
                    }
                    if (s_SendRemoveOnClosed)
                    {
                        ClientSend.SendRemoveDeathPack(GUID);
                        s_SendRemoveOnClosed = false;
                    }
                }
                Panel.Enable(false);
            }
        }

        public static void HandleStateUpdated(string GUID, int State)
        {
            GameObject Box = PdidTable.GetGameObject(GUID);
            if (Box)
            {
                Comps.ContainerDescriptorHook Hook = Box.GetComponent<Comps.ContainerDescriptorHook>();
                if (Hook)
                {
                    Hook.m_HookState = (Comps.ContainerDescriptorHook.ContainerState)State;
                    Container Container = Box.GetComponent<Container>();
                    switch (Hook.m_HookState)
                    {
                        case Comps.ContainerDescriptorHook.ContainerState.Inspected:
                        case Comps.ContainerDescriptorHook.ContainerState.Empty:
                            Container.MarkAsInspected();
                            Container.DestroyAllGear();
                            Container.m_GearToInstantiate.Clear();
                            if(Container.m_Lock != null)
                            {
                                UnityEngine.Object.Destroy(Container.m_Lock);
                            }
                            break;
                    }
                }
            }
            else
            {
                //SkyCoop.Logger.Log(ConsoleColor.Red, $"HandleStateUpdated {GUID} not found!");
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_Container), "OnDone")] // Once
        internal static class Panel_Container_Close
        {
            private static bool Prefix(Panel_Container __instance)
            {
                Container box = __instance.m_Container;
                if (__instance.ShouldEnterSectionNav() && __instance.m_Container != null && !__instance.m_Container.m_FilterLocked)
                {
                    __instance.EnterSectionNav();
                }
                else
                {
                    if (__instance.m_DraggedItem != null)
                    {
                        __instance.m_DraggedItem.TryForceDrop(false);
                        __instance.FinishDragDrop();
                    }
                    HandleContainerClose(box);
                }
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(ContainerInteraction), "GetHoverText")]
        private static class ContainerInteraction_GetHoverText
        {
            private static void Postfix(ContainerInteraction __instance, ref string __result)
            {
                Comps.DeathPackComp DeathBox = __instance.gameObject.GetComponent<Comps.DeathPackComp>();
                Comps.ContainerDescriptorHook Hook = __instance.gameObject.GetComponent<Comps.ContainerDescriptorHook>();
                if (DeathBox)
                {
                    if(Localization.Language.ToLower() == "english")
                    {
                        __result = $"[FF0000]{DeathBox.m_OwnerName}'s {Localization.Get("GAMEPLAY_BACKPACK")}[-]";
                    }
                    else
                    {
                        __result = $"[FF0000]{Localization.Get("GAMEPLAY_BACKPACK")} {DeathBox.m_OwnerName}[-]";
                    }
                }
                else if(Hook)
                {
                    if(Hook.m_HookState == Comps.ContainerDescriptorHook.ContainerState.Inspected)
                    {
                        __result = $"{Localization.Get(__instance.m_Container.m_LocalizedDisplayName.m_LocalizationID + "ContainerOpen")}\n{Localization.Get("GAMEPLAY_SearchedPostfix")}";
                    }else if(Hook.m_HookState == Comps.ContainerDescriptorHook.ContainerState.Empty)
                    {
                        __result = $"{Localization.Get(__instance.m_Container.m_LocalizedDisplayName.m_LocalizationID + "ContainerOpen")}\n{Localization.Get("GAMEPLAY_EmptyPostfix")}";
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Container), "Awake")]
        private static class Container_Awake
        {
            private static void Postfix(Container __instance)
            {
                Comps.ContainerDescriptorHook Hook = __instance.gameObject.GetComponent<Comps.ContainerDescriptorHook>();
                if (Hook == null)
                {
                    Hook = __instance.gameObject.AddComponent<Comps.ContainerDescriptorHook>();
                }
            }
        }
    }
}
