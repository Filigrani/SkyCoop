using Harmony;
using Il2Cpp;
using Il2Cpp;
using Il2CppTLD.AddressableAssets;
using SkyCoop;
using UnityEngine;
using static Il2Cpp.Utils;

namespace SkyCoopClient
{
    public class MaterialsContainer
    {
        public static Dictionary<string, Material> s_Materials = new Dictionary<string, Material>();
        public static DecorationItemVerificationList s_Decos;
        public static bool s_Intilized = false;

        public static Material GetMaterial(string MaterialName)
        {
            if (s_Materials.ContainsKey(MaterialName))
            {
                return UnityEngine.Object.Instantiate(s_Materials[MaterialName]);
            }
            return null;
        }

        public static void AddMaterial(Material Material)
        {
            SkyCoop.Logger.Log(ConsoleColor.Blue, $"[MaterialContainer] {Material.name} added");
            s_Materials.Remove(Material.name);
            s_Materials.Add(Material.name, Material);
        }

        public static void PrintAllMaterials()
        {
            foreach (string Key in s_Materials.Keys)
            {
                SkyCoop.Logger.Log(ConsoleColor.Cyan, $"[MaterialContainer] PrintAllMaterials Key {Key} material {s_Materials[Key].name}");
            }
        }

        public static GameObject GetDecoByIndex(int Index)
        {
            GameObject Obj = AssetHelper.SafeInstantiateAssetAsync(s_Decos.m_DecorationPrefabs[Index].AssetGUID, null, true).WaitForCompletion();
            if (Obj)
            {
                SkyCoop.Logger.Log(ConsoleColor.Blue, $"[MaterialContainer] Deco {Index} Found");
                return Obj;
            }
            else
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, $"[MaterialContainer] Deco {Index} NOT EXIST!");
            }
            return null;
        }

        public static void ApplyInGameMaterials(GameObject Obj)
        {
            foreach (Renderer Rend in Obj.GetComponentsInChildren<Renderer>())
            {
                Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Material> NewMatsArr = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Material>(Rend.materials.Length);
                for (int i = 0; i < NewMatsArr.Length; i++)
                {
                    Material MaterialInSlot = Rend.materials[i];
                    if (MaterialInSlot != null && MaterialInSlot.name != "Default-Material (Instance)")
                    {
                        Material InGameMaterial = GetMaterial(MaterialInSlot.name);
                        if (InGameMaterial != null)
                        {
                            SkyCoop.Logger.Log(ConsoleColor.Green, $"[MaterialContainer] ApplyInGameMaterials Renderer object {Rend.gameObject.name} MaterialInSlot {MaterialInSlot.name} replaced with in-game material.");
                            InGameMaterial.color = MaterialInSlot.color;
                            NewMatsArr[i] = InGameMaterial;
                        }
                        else
                        {
                            SkyCoop.Logger.Log(ConsoleColor.Red, $"[MaterialContainer] ApplyInGameMaterials Renderer object {Rend.gameObject.name} MaterialInSlot {MaterialInSlot.name} wasn't able to replace material!");
                            NewMatsArr[i] = MaterialInSlot;
                        }
                    }
                }
                Rend.SetMaterialArray(NewMatsArr);
            }
        }

        public static void NotGarbageCollect(GameObject Obj)
        {
            Obj.SetActive(false);
            SceneManager.DontDestroyOnLoad(Obj);
        }

        public static void PreloadMaterials()
        {
            s_Decos = DecorationItemVerificationList.Load();
            GameObject Obj = GetDecoByIndex(101);
            // GLB_WallWoodNatural_B | GLB_WoodWallNatural_B04_Blend
            AddMaterial(Obj.transform.GetChild(0).GetComponent<Renderer>().material);
            // TRN_Snow_Ground_A_Low_Noise | TRN_SnowA_01
            AddMaterial(Obj.transform.GetChild(3).GetChild(0).GetComponent<Renderer>().material);
            NotGarbageCollect(Obj);

            Obj = GetDecoByIndex(32);
            // GLB_WallWoodNatural_A_Flat | GLB_WoodWallnatural_A01_Flat
            AddMaterial(Obj.GetComponent<Renderer>().materials[1]);
            NotGarbageCollect(Obj);

            Obj = GetDecoByIndex(807);
            // GLB_Green_C01 | GLB_Green_E02
            AddMaterial(Obj.transform.GetChild(0).GetComponent<Renderer>().materials[0]);
            NotGarbageCollect(Obj);

            Obj = GetDecoByIndex(772);
            // OBJ_Bones_A | OBJ_BonesA_Mat
            AddMaterial(Obj.transform.GetChild(0).GetComponent<Renderer>().materials[1]);
            NotGarbageCollect(Obj);

            Obj = GetDecoByIndex(156);
            // OBJ_WoodCrates_A | OBJ_WoodCrates_A
            AddMaterial(Obj.transform.GetChild(0).GetComponent<Renderer>().materials[0]);
            NotGarbageCollect(Obj);

            Obj = GetDecoByIndex(412);
            // GLB_MetalCorrugated_I | GLB_MetalCorrugated_I01_Blend
            AddMaterial(Obj.transform.GetChild(0).GetComponent<Renderer>().materials[0]);
            NotGarbageCollect(Obj);

            Obj = GetDecoByIndex(769);
            // GLB_WallWoodWhitewash_A_Flat | GLB_WoodWallRed_D03_Flat_Blend
            AddMaterial(Obj.transform.GetChild(0).GetComponent<Renderer>().materials[0]);
            NotGarbageCollect(Obj);

            //AssetManager.GetAssetFromGame<GameObject>("GEAR_CookingPot");
        }
    }
}
