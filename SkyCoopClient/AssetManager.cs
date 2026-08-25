using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppSystem.Linq;
using Il2CppTLD.AddressableAssets;
using Il2CppTLD.Scenes;
using SkyCoopClient;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using static SkyCoop.Comps;
using Il2CppCollection = Il2CppSystem.Collections.Generic;

namespace SkyCoop
{
    internal class AssetManager
    {
        public static string s_MainBundlePath = "Mods/skycoop";
        public static AssetBundle s_MainBundle = null;
        public static GameObject s_PistolBulletPrefab = null;
        public static GameObject s_RevolverBulletPrefab = null;

        public static AssetBundle LoadAssetBundle(string name)
        {
            using (FileStream? stream = new FileStream(name, FileMode.Open))
            {
                MemoryStream? memory = new((int)stream.Length);
                stream!.CopyTo(memory);

                Il2CppSystem.IO.MemoryStream memoryStream = new(memory.ToArray());
                AssetBundle bundle = AssetBundle.LoadFromStream(memoryStream);

                return bundle;
            }
        }

        public static void PreloadMainBundle()
        {
            if(s_MainBundle == null)
            {
                //s_MainBundle = AssetBundle.LoadFromFile(s_MainBundlePath);
                s_MainBundle = LoadAssetBundle(s_MainBundlePath);

                if (s_MainBundle == null)
                {
                    Logger.Log(ConsoleColor.Red,"Have problems with loading main asset bundle!");
                } else
                {
                    Logger.Log(ConsoleColor.Blue, "Main Asset Bundle is loaded.");
                }
            }
            //DumpAddressablesContent();
            //DumpPrefabsList();
            //DumpScenes();
        }

        public static T GetAssetFromGame<T>(string AssetName) where T : UnityEngine.Object
        {
            T Asset = Addressables.LoadAssetAsync<T>(AssetName).WaitForCompletion();
            if (Asset == null)
            {
                //Logger.Log(System.ConsoleColor.Yellow, "Can't load "+AssetName+" from game assets!");
                //Logger.Log(System.ConsoleColor.DarkMagenta, "Fine...lets try old way");
                Asset = GetAssetFromResources_OLD<T>(AssetName);
                if(Asset == null)
                {
                    Logger.Log(System.ConsoleColor.Yellow, "Can't load " + AssetName + " from game assets!");
                }
            }
            return Asset;
        }

        public static void BogusIt(GameObject Obj)
        {
            foreach (Component Com in Obj.GetComponents<Component>())
            {
                string ComName = Com.GetIl2CppType().Name;
                if (ComName != Il2CppType.Of<BoxCollider>().Name
                    && ComName != Il2CppType.Of<SphereCollider>().Name
                    && ComName != Il2CppType.Of<CapsuleCollider>().Name
                    && ComName != Il2CppType.Of<MeshCollider>().Name
                    && ComName != "PhysicMaterial"
                    && ComName != Il2CppType.Of<MeshFilter>().Name
                    && ComName != Il2CppType.Of<LODGroup>().Name
                    && ComName != Il2CppType.Of<Transform>().Name
                    && ComName != Il2CppType.Of<Rigidbody>().Name
                    && ComName != Il2CppType.Of<MeshRenderer>().Name
                    && ComName != Il2CppType.Of<SkinnedMeshRenderer>().Name
                    && ComName != Il2CppType.Of<AudioSource>().Name)
                {
                    UnityEngine.Object.Destroy(Com);
                }
            }
        }

        public static GameObject CreateLocalizedBogusGear(string GearName, out string LocalizedName, float Volume = 0, float ConditionNormalized = 1, int Style = 0, Transform parent = null)
        {
            LocalizedName = "Invalid";
            GameObject Prefab = GetAssetFromGame<GameObject>(GearName);
            if (Prefab)
            {
                GameObject GearObject = UnityEngine.Object.Instantiate(Prefab, parent);
                if (GearObject)
                {
                    GearObject.name = GearName;
                    
                    GearItem gi = GearObject.GetComponent<GearItem>();
                    if (gi)
                    {
                        LocalizedName = gi.DisplayName;
                    }
                    else
                    {
                        return null;
                    }

                    if (gi.m_CookingPotItem)
                    {
                        Comps.GearCookingVisual Cooking = GearObject.AddComponent<GearCookingVisual>();
                        if (Cooking)
                        {
                            Cooking.Override(gi.m_CookingPotItem);
                        }
                    }
                    if (gi.m_Cookable)
                    {
                        Comps.GearCookingVisual Cooking = GearObject.AddComponent<GearCookingVisual>();
                        if (Cooking)
                        {
                            Cooking.Override(gi.m_Cookable, Volume);
                        }
                    }

                    gi.SetNormalizedHP(ConditionNormalized);

                    MeshSwapItem Swap = GearObject.GetComponent<MeshSwapItem>();

                    if (Swap)
                    {
                        if (gi.m_FoodItem)
                        {
                            if (Swap.m_MeshObjUnopened)
                            {
                                Swap.m_MeshObjUnopened.SetActive(Style == 0);
                            }
                            if (Swap.m_MeshObjOpened)
                            {
                                Swap.m_MeshObjOpened.SetActive(Style == 1);
                            }
                        }
                    }
                    if (gi.m_Bed)
                    {
                        gi.m_Bed.SetState(Style == 0 ? BedRollState.Rolled : BedRollState.Placed);
                    }

                    if (gi.m_FlareItem)
                    {
                        switch (Style)
                        {
                            case 0:
                                gi.m_FlareItem.SetState(FlareState.Fresh);
                                break;
                            case 1:
                                gi.m_FlareItem.SetState(FlareState.Burning);
                                break;
                            case 2:
                                gi.m_FlareItem.SetState(FlareState.BurnedOut);
                                break;
                            default:
                                break;
                        }
                        if (gi.m_FlareItem.m_FXGameObject)
                        {
                            gi.m_FlareItem.m_FXGameObject.SetActive(Style == 1);
                        }
                    }
                    if (gi.m_TorchItem)
                    {
                        switch (Style)
                        {
                            case 0:
                                gi.m_TorchItem.SetState(TorchState.Fresh);
                                break;
                            case 1:
                                gi.m_TorchItem.SetState(TorchState.Burning);
                                break;
                            case 2:
                                gi.m_TorchItem.SetState(TorchState.BurnedOut);
                                break;
                            default:
                                break;
                        }
                        if (gi.m_TorchItem.m_FXGameObject)
                        {
                            gi.m_TorchItem.m_FXGameObject.SetActive(Style == 1);
                        }
                        Transform Gradient = gi.transform.FindChild("RadialGradient");
                        if (Gradient)
                        {
                            Gradient.gameObject.SetActive(Style == 1);
                        }
                    }

                    Collider Collider = gi.gameObject.GetComponent<Collider>();

                    if(Collider == null)
                    {
                        UncookedGearsFix.UncookedGearPatch(GearObject);
                    }


                    foreach (Component Com in GearObject.GetComponents<Component>())
                    {
                        string ComName = Com.GetIl2CppType().Name;
                        if (ComName != Il2CppType.Of<BoxCollider>().Name
                            && ComName != Il2CppType.Of<SphereCollider>().Name
                            && ComName != Il2CppType.Of<CapsuleCollider>().Name
                            && ComName != Il2CppType.Of<MeshCollider>().Name
                            && ComName != "PhysicMaterial"
                            && ComName != Il2CppType.Of<MeshFilter>().Name
                            && ComName != Il2CppType.Of<LODGroup>().Name
                            && ComName != Il2CppType.Of<Transform>().Name
                            && ComName != Il2CppType.Of<Rigidbody>().Name
                            && ComName != Il2CppType.Of<MeshRenderer>().Name
                            && ComName != Il2CppType.Of<SkinnedMeshRenderer>().Name
                            && ComName != Il2CppType.Of<AudioSource>().Name
                            && ComName != Il2CppType.Of<GearCookingVisual>().Name
                            && ComName != Il2CppType.Of<Bed>().Name
                            && ComName != Il2CppType.Of<FlareItem>().Name
                            && ComName != Il2CppType.Of<TorchItem>().Name)
                        {
                            UnityEngine.Object.Destroy(Com);
                        }
                    }
                    return GearObject;
                }
                else
                {
                    Logger.Log(ConsoleColor.Red, "Can't instantiate " + Prefab.name);
                }
            }
            return null;
        }

        public static GameObject CreateBogusGear(string GearName, Transform parent = null)
        {
            GameObject Prefab = GetAssetFromGame<GameObject>(GearName);
            if (Prefab)
            {
                GameObject GearObject = UnityEngine.Object.Instantiate(Prefab, parent);
                if (GearObject)
                {
                    GearObject.name = GearName;
                    foreach (Component Com in GearObject.GetComponents<Component>())
                    {
                        string ComName = Com.GetIl2CppType().Name;
                        if (ComName != Il2CppType.Of<BoxCollider>().Name
                            && ComName != Il2CppType.Of<SphereCollider>().Name
                            && ComName != Il2CppType.Of<CapsuleCollider>().Name
                            && ComName != Il2CppType.Of<MeshCollider>().Name
                            && ComName != "PhysicMaterial"
                            && ComName != Il2CppType.Of<MeshFilter>().Name
                            && ComName != Il2CppType.Of<LODGroup>().Name
                            && ComName != Il2CppType.Of<Transform>().Name
                            && ComName != Il2CppType.Of<Rigidbody>().Name
                            && ComName != Il2CppType.Of<MeshRenderer>().Name
                            && ComName != Il2CppType.Of<SkinnedMeshRenderer>().Name
                            && ComName != Il2CppType.Of<FlareItem>().Name
                            && ComName != Il2CppType.Of<TorchItem>().Name)
                        {
                            UnityEngine.Object.Destroy(Com);
                        }
                    }
                    return GearObject;
                } else
                {
                    Logger.Log(ConsoleColor.Red, "Can't instantiate " + Prefab.name);
                }
            }

            return null;
        }

        public static T GetAssetFromResources_OLD<T>(string AssetName) where T : UnityEngine.Object
        {
            UnityEngine.Object Asset = Resources.Load(AssetName);
            if (Asset)
            {
                return Resources.Load(AssetName).Cast<T>();
            }
            return null;
        }

        // This using casting, because we can use it to load textures, audio clips and etc, not just prefabs.
        public static T GetAssetFromBundle<T>(string AssetName) where T : UnityEngine.Object
        {
            if (s_MainBundle == null)
            {
                Logger.Log(ConsoleColor.Red, "Can't load "+AssetName+" because bundle is missing!");
                return null;
            }
            return s_MainBundle.LoadAsset<T>(AssetName);
        }

        public static T GetAssetFromAddressables<T>(string AssetPath) where T : UnityEngine.Object
        {
            var Asset = AssetHelper.SafeLoadAssetAsync<T>(AssetPath);

            if (Asset == null)
            {
                Logger.Log(ConsoleColor.Red, "GetAssetFromAddressables() Can't load asset " + AssetPath);
                return null;
            }
            return Asset.WaitForCompletion();
        }

        public static void DumpAddressablesContent()
        {
            foreach (var item in Addressables.ResourceLocators.ToList())
            {
                foreach (var key in item.Keys.ToList())
                {
                    Logger.Log(ConsoleColor.Magenta, "[Addressables][LocatorId=" + item.LocatorId + "] " + key.ToString());
                }
            }
        }
        public static void DumpPrefabsList()
        {
            foreach (var item in Resources.LoadAll("", null))
            {
                Logger.Log(ConsoleColor.Magenta, "[Resources] " + item.name);
            }
        }

        public static void DumpScenes()
        {
            Il2CppCollection.List<IResourceLocation> scenes = AssetHelper.FindAllAssetsLocations<SceneSet>().Cast<Il2CppCollection.List<IResourceLocation>>();
            Il2CppCollection.List<string> sceneParamaters = new Il2CppCollection.List<string>();

            foreach (IResourceLocation sceneResource in scenes)
            {
                Logger.Log(ConsoleColor.Magenta, $"{sceneResource.PrimaryKey}");
            }
            sceneParamaters.Sort();
        }

        public static void DumpLocalizationKeysList()
        {
            foreach (StringTableData Data in Localization.s_CurrentLanguageStringTable.m_DataFiles)
            {
                foreach (StringTableData.Entry Entry in Data.m_Entries)
                {
                    Logger.Log(ConsoleColor.Magenta, $"{Data.name} Key {Entry.m_Key}");
                }
            }
        }

        public static GearItem GetGearPrefab(string GearName)
        {
            GameObject reference = GetAssetFromGame<GameObject>(GearName);
            if (reference)
            {
                GearItem GearItemPrefab = reference.GetComponent<GearItem>();

                if (GearItemPrefab)
                {
                    return GearItemPrefab;
                }
            }
            return null;
        }
    }
}
