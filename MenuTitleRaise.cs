using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace SkyCoop
{
    // In the multiplayer menus the title ("MULTIPLAYER") is drawn over the game logo ("THE LONG DARK").
    // Move it above the logo while a mod menu is shown; vanilla titles stay where they are.
    [HarmonyPatch]
    internal static class BasicMenu_UpdateTitle_Raise
    {
        const float RaiseBy = 35f;
        static readonly Dictionary<int, Vector3> Original = new Dictionary<int, Vector3>();

        private static IEnumerable<MethodBase> TargetMethods() =>
            typeof(BasicMenu).GetMethods().Where(m => m.Name == "UpdateTitle" && m.GetParameters().Length > 0 && m.GetParameters()[0].ParameterType == typeof(string)).Cast<MethodBase>();

        private static void Postfix(BasicMenu __instance, string __0)
        {
            try
            {
                if (MyMod.m_Panel_Sandbox == null || __instance.Pointer != MyMod.m_Panel_Sandbox.m_BasicMenu.Pointer || string.IsNullOrEmpty(__0))
                {
                    return;
                }
                foreach (UILabel L in MyMod.m_Panel_Sandbox.GetComponentsInChildren<UILabel>(true))
                {
                    if (L.text != __0 && L.mText != __0)
                    {
                        continue;
                    }
                    bool IsButton = false;
                    for (Transform T = L.transform.parent; T != null; T = T.parent)
                    {
                        if (T.name == "Grid") { IsButton = true; break; }
                    }
                    if (IsButton)
                    {
                        continue;
                    }
                    int Id = L.gameObject.GetInstanceID();
                    if (!Original.TryGetValue(Id, out Vector3 Orig))
                    {
                        Orig = L.transform.localPosition;
                        Original[Id] = Orig;
                    }
                    bool Mod = MenuChange.MenuMode != "Original" || __0 == Loc.T("MULTIPLAYER") || __0 == Loc.T("LOBBY");
                    L.transform.localPosition = Mod ? Orig + new Vector3(0f, RaiseBy, 0f) : Orig;
                }
            }
            catch (System.Exception) { }
        }
    }
}
