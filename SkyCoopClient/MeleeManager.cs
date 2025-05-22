using AssetsTools.NET.Extra;
using Il2Cpp;
using Il2CppTLD.Gameplay;
using SkyCoop;
using UnityEngine;

namespace SkyCoopClient
{
    public class MeleeManager
    {
        public static float s_MeleeCooldown = 0;
        public static List<GameObject> s_Meshes = new List<GameObject>();
        public static Dictionary<string, GameObject> s_MeshsByGearName = new Dictionary<string, GameObject>();
        const string HandPropSearchPath = "/CHARACTER_FPSPlayer/NEW_FPHand_Rig/GAME_DATA/Origin/HipJoint/Chest_Joint/Camera_Weapon_Offset/Shoulder_Joint/Shoulder_Joint_Offset/Right_Shoulder_Joint_Offset/RightClavJoint/RightShoulderJoint/RightElbowJoint/RightWristJoint/RightPalm/right_prop_point/";

        public static FPSItem s_DummyFPSItem = null;


        public static List<string> s_MeleeWeapons = new List<string>()
        {
            "GEAR_Hatchet",
            "GEAR_HatchetImprovised",
            "GEAR_Knife",
            "GEAR_KnifeImprovised",
            "GEAR_JeremiahKnife",
            "GEAR_KnifeScrapMetal",
            "GEAR_Hammer",
            "GEAR_Prybar",
        };

        public static bool IsMeleeWeapon(string GearName)
        {
            return s_MeleeWeapons.Contains(GearName);
        }

        public static void OnMeleeEquipped(string WeaponName)
        {
            //SkyCoop.Logger.Log("OnWeaponEquipped " + WeaponName);
            GameObject ViewModel;
            if (s_MeshsByGearName.TryGetValue(WeaponName, out ViewModel))
            {
                ViewModel.SetActive(true);
            }
        }
        public static void OnMeleeUnEquipped()
        {
            //SkyCoop.Logger.Log("OnMeleeUnEquipped");
            foreach (GameObject Mesh in s_Meshes)
            {
                Mesh.SetActive(false);
            }
        }

        public static void AddViewModel(GameObject Mesh, string GearName)
        {
            s_Meshes.Add(Mesh);
            s_MeshsByGearName.Add(GearName, Mesh);
            //SkyCoop.Logger.Log("AddViewModel " + GearName+" "+Mesh.name);
        }

        public static GameObject AssignExistingViewModel(Transform Parnet, string MeshName, string GearName)
        {
            //SkyCoop.Logger.Log("AssignExistingViewModel "+MeshName);
            if (Parnet)
            {
                Transform T = Parnet.FindChild(MeshName);
                if (T)
                {
                    AddViewModel(T.gameObject, GearName);
                    return T.gameObject;
                }
            }
            return null;
        }

        public static GameObject AssignBogusViewModel(Transform Parnet, string PrefabName, string GearName, Vector3 Position, Vector3 Rotation)
        {
            //SkyCoop.Logger.Log("AssignExistingViewModel "+MeshName);
            if (Parnet)
            {
                GameObject Bogus = AssetManager.CreateBogusGear(PrefabName, Parnet);
                if (Bogus)
                {
                    AddViewModel(Bogus, GearName);
                    Bogus.transform.localPosition = Position;
                    Bogus.transform.SetLocalEulerAngles(Rotation, RotationOrder.OrderXYZ);
                }
                return Bogus;
            }
            return null;
        }

        public static void ReintilizeViewModels()
        {
            //SkyCoop.Logger.Log("ReintilizeViewModels");
            s_MeshsByGearName.Clear();
            s_Meshes.Clear();
            GameObject ParnetObject = GameObject.Find(HandPropSearchPath);
            if (ParnetObject)
            {
                AssignExistingViewModel(ParnetObject.transform, "mesh_hatchet", "GEAR_Hatchet");
                AssignExistingViewModel(ParnetObject.transform, "mesh_Hatchet_improvised", "GEAR_HatchetImprovised");
                GameObject KnifeMesh = AssignExistingViewModel(ParnetObject.transform, "mesh_knife", "GEAR_Knife");
                AssignExistingViewModel(ParnetObject.transform, "mesh_knife_improvised", "GEAR_KnifeImprovised");
                AssignExistingViewModel(ParnetObject.transform, "mesh_prybar", "GEAR_Prybar");
                AssignExistingViewModel(ParnetObject.transform, "mesh_hammer", "GEAR_Hammer");
                AssignExistingViewModel(ParnetObject.transform, "FPH_Brand(Clone)", "GEAR_Brand");

                if(AssignExistingViewModel(ParnetObject.transform, "mesh_scrapknife", "GEAR_KnifeScrapMetal") == null)
                {
                    GameObject KnifeScrap = AssignBogusViewModel(ParnetObject.transform, "GEAR_KnifeScrapMetal", "GEAR_KnifeScrapMetal", new Vector3(0, 0.05f, 0), new Vector3(90, -30, 0));
                    KnifeScrap.name = "mesh_scrapknife";
                    KnifeScrap.SetActive(false);
                }
                if(AssignExistingViewModel(ParnetObject.transform, "mesh_jeremiahknife", "GEAR_JeremiahKnife") == null)
                {
                    if (KnifeMesh)
                    {
                        GameObject KnifeMeshClone = UnityEngine.Object.Instantiate<GameObject>(KnifeMesh, KnifeMesh.transform.parent);
                        if (KnifeMeshClone)
                        {
                            KnifeMeshClone.transform.position = KnifeMesh.transform.position;
                            KnifeMeshClone.transform.rotation = KnifeMesh.transform.rotation;
                            KnifeMeshClone.name = "mesh_jeremiahknife";

                            GameObject Bogus = AssetManager.CreateBogusGear("GEAR_JeremiahKnife");
                            if (Bogus)
                            {
                                KnifeMeshClone.GetComponent<MeshRenderer>().material = Bogus.transform.GetChild(0).GetComponent<MeshRenderer>().material;
                            }
                            UnityEngine.Object.Destroy(Bogus); // This one was needed, just to grab texture.
                            AddViewModel(KnifeMeshClone, "GEAR_JeremiahKnife");
                        }
                    }
                }
                FindDummy();
            }
        }

        public static void FindDummy()
        {
            if(s_DummyFPSItem == null)
            {
                foreach (FPSItem Item in GameManager.GetVpFPSCamera().m_Weapons.Keys)
                {
                    if(Item.name == "FPSItem_Brand")
                    {
                        s_DummyFPSItem = Item;
                        return;
                    }
                }
            }
        }

        public static void MeeleWeaponPatch(GearItem __instance)
        {
            if (IsMeleeWeapon(__instance.name) && __instance.gameObject.GetComponent<FirstPersonItem>() == null)
            {
                FirstPersonItem FPI = __instance.gameObject.AddComponent<FirstPersonItem>();

                FPI.m_FirstPersonObjectName = "Revolver";

                GameObject reference = AssetManager.GetAssetFromGame<GameObject>("GEAR_Stone");
                if (reference != null && reference.GetComponent<FirstPersonItem>() != null)
                {
                    FirstPersonItem rFPI = reference.GetComponent<FirstPersonItem>();
                    FPI.m_PlayerStateTransitions = rFPI.m_PlayerStateTransitions;

                    FPI.m_ItemData = s_DummyFPSItem;
                    FPI.m_UnwieldAudioEvent = rFPI.m_UnwieldAudioEvent;
                    FPI.m_WieldAudioEvent = rFPI.m_WieldAudioEvent;
                }
                __instance.m_FirstPersonItem = FPI;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(ItemDescriptionPage), "GetEquipButtonLocalizationId")] // Once
        private static class ItemDescriptionPage_GetEquipButtonLocalizationId
        {
            private static void Postfix(ItemDescriptionPage __instance, GearItem gi, ref string __result)
            {
                if (gi != null)
                {
                    if (IsMeleeWeapon(gi.name))
                    {
                        __result = "GAMEPLAY_Use";
                    }else if(gi.name == "GEAR_CookingPot" || gi.name == "GEAR_BallisticVest")
                    {
                        __result = "GAMEPLAY_Wear";
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(vp_FPSCamera), "SetWeapon")]
        private static class ItemDescriptionPage_SetWeapon
        {
            private static void Postfix(vp_FPSCamera __instance)
            {
                if (__instance.m_CurrentWeapon == null)
                {
                    OnMeleeUnEquipped();
                }else if(__instance.m_CurrentWeapon.m_FPSMeshId == FPSMeshID.Brand)
                {
                    OnMeleeUnEquipped();
                    OnMeleeEquipped(GameManager.m_PlayerManager.m_ItemInHands.name);
                }
            }
        }

        public static void TryToAttack()
        {
            if(GameManager.m_PlayerManager && GameManager.m_PlayerManager.m_ItemInHands)
            {
                if (IsMeleeWeapon(GameManager.m_PlayerManager.m_ItemInHands.name))
                {
                    GameManager.m_NewPlayerAnimation.Trigger_Generic_Throw(new System.Action(DoMeleeHit), new System.Action(MeleeUnstove));
                }
            }
        }

        public static void DoMeleeHit()
        {
            string MeleeName = "GEAR_Knife";


            if (GameManager.m_PlayerManager.m_ItemInHands)
            {
                MeleeName = GameManager.m_PlayerManager.m_ItemInHands.name;
            }
            
            
            Vector3 Position = GameManager.GetVpFPSCamera().transform.position;
            Quaternion Rotation = GameManager.GetVpFPSCamera().transform.rotation;

            if (AssetManager.s_PistolBulletPrefab)
            {
                GameObject Bullet = UnityEngine.Object.Instantiate<GameObject>(AssetManager.s_PistolBulletPrefab, Position, Rotation);
                if (Bullet)
                {
                    vp_Bullet Comp = Bullet.GetComponent<vp_Bullet>();
                    if (Comp)
                    {
                        Comp.m_GunType = GunType.Camera;
                        Comp.Range = 2.5f;
                        Comp.m_ImpactAudio = "Play_StoneImpacts";
                        Bullet.gameObject.AddComponent<Comps.MeleeBulletHandler>().m_GearName = MeleeName;
                    }
                    ClientSend.SendProjectile(Position, Rotation, "Melee");
                }
            }
            else
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, "s_PistolBulletPrefab is null!");
            }
        }

        public static void MeleeUnstove()
        {
            
            if (GameManager.m_PlayerManager.m_ItemInHands && IsMeleeWeapon(GameManager.m_PlayerManager.m_ItemInHands.name))
            {
                GearItem gi = GameManager.m_PlayerManager.m_ItemInHands;
                GameManager.m_PlayerManager.UnequipItemInHandsSkipAnimation();
                GameManager.GetPlayerManagerComponent().UseInventoryItem(gi);
            }
        }
    }
}
