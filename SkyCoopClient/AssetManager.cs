using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppSystem.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SkyCoop
{
    internal class AssetManager
    {
        public static string s_MainBundlePath = "Mods\\skycoop";
        public static AssetBundle s_MainBundle = null;
        public static GameObject s_PistolBulletPrefab = null;
        public static GameObject s_RevolverBulletPrefab = null;

        public static void PreloadMainBundle()
        {
            if(s_MainBundle == null)
            {
                s_MainBundle = AssetBundle.LoadFromFile(s_MainBundlePath);
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
        }

        public static T GetAssetFromGame<T>(string AssetName) where T : UnityEngine.Object
        {
            T Asset = Addressables.LoadAssetAsync<T>(AssetName).WaitForCompletion();
            if (Asset == null)
            {
                Logger.Log(System.ConsoleColor.Red, "Can't load "+AssetName+" from game assets!");
                Logger.Log(System.ConsoleColor.DarkMagenta, "Fine...lets try old way");
                Asset = GetAssetFromResources_OLD<T>(AssetName);
                if(Asset == null)
                {
                    Logger.Log(System.ConsoleColor.DarkMagenta, "Na, bogus.");
                }
            }
            return Asset;
        }

        public static GameObject CreateBogusGear(string GearName)
        {
            GameObject Prefab = GetAssetFromGame<GameObject>(GearName);
            if (Prefab)
            {
                GameObject GearObject = UnityEngine.Object.Instantiate(Prefab);
                if (GearObject)
                {
                    foreach (Component Com in GearObject.GetComponents<Component>())
                    {
                        string ComName = Com.GetIl2CppType().Name;
                        if (ComName != Il2CppType.Of<BoxCollider>().Name
                            && ComName != Il2CppType.Of<SphereCollider>().Name
                            && ComName != Il2CppType.Of<CapsuleCollider>().Name
                            && ComName != Il2CppType.Of<MeshCollider>().Name
                            && ComName != Il2CppType.Of<PhysicMaterial>().Name
                            && ComName != Il2CppType.Of<MeshFilter>().Name
                            && ComName != Il2CppType.Of<LODGroup>().Name
                            && ComName != Il2CppType.Of<Transform>().Name
                            && ComName != Il2CppType.Of<Rigidbody>().Name
                            && ComName != Il2CppType.Of<MeshRenderer>().Name
                            && ComName != Il2CppType.Of<SkinnedMeshRenderer>().Name)
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
            foreach (var item in Resources.LoadAll(""))
            {
                Logger.Log(ConsoleColor.Magenta, "[Resources] " + item.name);
            }
        }

        public static void RegisterIlegalGearsCommand()
        {
            uConsole.RegisterCommand("give", new Action(GiveIlegalGear));
        }

        public static void GiveIlegalGear()
        {
            GameObject reference = GetAssetFromGame<GameObject>(uConsole.GetString());
            if (reference)
            {
                GameObject GearObject = UnityEngine.Object.Instantiate(reference);
                GearItem item = GearObject.GetComponent<GearItem>();
                if(item != null)
                {
                    item.CompleteSpawnFromCONSOLE();
                    GameManager.GetInventoryComponent().AddGear(item);
                }

            }
        }
    }
}
