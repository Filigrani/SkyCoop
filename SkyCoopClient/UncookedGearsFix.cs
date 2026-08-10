using Il2Cpp;
using Il2CppTLD.Cooking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler;

namespace SkyCoopClient
{
    public static class UncookedGearsFix
    {
        [HarmonyLib.HarmonyPatch(typeof(InventoryGridItem), "Refresh")]
        private static class InventoryGridItem_Refresh
        {
            private static void Postfix(InventoryGridItem __instance, GearItem gi, int index)
            {
                if(gi != null)
                {
                    if (gi.name.StartsWith("GEAR_Uncooked"))
                    {
                        if (gi.m_Cookable && gi.m_Cookable.m_CookedPrefab)
                        {
                            __instance.m_GearSprite.mainTexture = Utils.GetInventoryIconTexture(gi.m_Cookable.m_CookedPrefab);
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GearItemCoverflow), "SetGearItem")]
        private static class GearItemCoverflow_SetGearItem
        {
            private static void Postfix(GearItemCoverflow __instance, GearItem gi, string gearPrefabName, bool isSmash)
            {
                if (gi != null)
                {
                    if (gi.name.StartsWith("GEAR_Uncooked"))
                    {
                        if (gi.m_Cookable && gi.m_Cookable.m_CookedPrefab)
                        {
                            __instance.m_Texture.mainTexture = Utils.GetGearCoverflowTexture(gi.m_Cookable.m_CookedPrefab);
                        }
                    }
                }
            }
        }
        public static void UncookedGearPatch(GameObject GearObj)
        {
            if (GearObj)
            {
                Collider Collider = GearObj.GetComponent<Collider>();

                if(Collider == null)
                {
                    BoxCollider Box = GearObj.AddComponent<BoxCollider>();
                    Box.center = new Vector3(-0.0001f, 0.1252f, 0.0001f);
                    Box.extents = new Vector3(0.0433f, 0.1254f, 0.0412f);
                    Box.size = new Vector3(0.0866f, 0.2507f, 0.0823f);
                }
            }
        }

        public static bool UncookedGearCompatible(GearItem UncookedGear, CookingPotItem Pot)
        {
            if(Pot == null || Pot == null)
            {
                return false;
            }
            if (UncookedGear.m_Cookable)
            {
                switch (Pot.m_GrubMeshType)
                {
                    case CookingPotItem.GrubMeshType.Pot:
                        return UncookedGear.m_Cookable.m_MeshPotStyle || UncookedGear.m_Cookable.m_MeshRawPotStyle;
                    case CookingPotItem.GrubMeshType.Can:
                        return UncookedGear.m_Cookable.m_MeshCanStyle || UncookedGear.m_Cookable.m_MeshRawCanStyle;
                    case CookingPotItem.GrubMeshType.FryingPan:
                        return UncookedGear.m_Cookable.m_MeshFryingPanStyle || UncookedGear.m_Cookable.m_MeshRawFryingPanStyle;
                    default:
                        return false;
                }
            }
            return false;
        }
    }
}
