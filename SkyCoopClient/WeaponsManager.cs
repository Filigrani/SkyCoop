using Il2Cpp;
using Il2CppTLD.Gameplay;
using MelonLoader;
using SkyCoop;
using SkyCoopServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MelonLoader.Modules.MelonModule;
using static SkyCoop.Comps;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace SkyCoopClient
{
    public static class WeaponsManager
    {
        public static Dictionary<string, WeaponDescripter> s_WeaponDescriptors = new Dictionary<string, WeaponDescripter>();

        public static void InitDescriptors()
        {
            s_WeaponDescriptors.Clear();

            WeaponDescripter Rifle = new WeaponDescripter();
            Rifle.m_PlayerDamage = 50;
            Rifle.m_BloodLoss = true;
            Rifle.m_ClothingTearing = true;
            Rifle.m_DamageType = DataStr.DamageType.Rifle;
            AddDescriptor("GEAR_Rifle", Rifle);

            WeaponDescripter Revolver = new WeaponDescripter();
            Revolver.m_PlayerDamage = 30;
            Revolver.m_BloodLoss = true;
            Revolver.m_ClothingTearing = true;
            Revolver.m_DamageType = DataStr.DamageType.Revolver;
            AddDescriptor("GEAR_Revolver", Revolver);

            WeaponDescripter FlareGun = new WeaponDescripter();
            FlareGun.m_PlayerDamage = 20;
            FlareGun.m_Burn = true;
            FlareGun.m_Pain = true;
            FlareGun.m_DamageType = DataStr.DamageType.FlareGun;
            AddDescriptor("GEAR_FlareGun", FlareGun);

            WeaponDescripter SimpleArrow = new WeaponDescripter();
            SimpleArrow.m_PlayerDamage = 25;
            SimpleArrow.m_ClothingTearing = true;
            SimpleArrow.m_DamageType = DataStr.DamageType.Bow;
            AddDescriptor("GEAR_Arrow", SimpleArrow);
            // Just in case, because damage per arrow type must be used instead.
            AddDescriptor("GEAR_Bow", SimpleArrow);

            WeaponDescripter FireheadedArrow = new WeaponDescripter();
            FireheadedArrow.m_PlayerDamage = 20;
            FireheadedArrow.m_ClothingTearing = true;
            FireheadedArrow.m_DamageType = DataStr.DamageType.Bow;
            AddDescriptor("GEAR_ArrowHardened", SimpleArrow);

            WeaponDescripter Hatchet = new WeaponDescripter();
            Hatchet.m_IsMelee = true;
            Hatchet.m_PlayerDamage = 20;
            Hatchet.m_AnimalDamage = 45;
            Hatchet.m_BloodLoss = true;
            Hatchet.m_ClothingTearing = true;
            Hatchet.m_AttackSpeed = 1.1f;
            Hatchet.m_DamageType = DataStr.DamageType.Hatchet;
            AddDescriptor("GEAR_Hatchet", Hatchet);
            AddDescriptor("GEAR_HatchetImprovised", Hatchet);

            WeaponDescripter Hammer = new WeaponDescripter();
            Hammer.m_IsMelee = true;
            Hammer.m_PlayerDamage = 25;
            Hammer.m_AnimalDamage = 55;
            Hammer.m_Pain = true;
            Hammer.m_ClothingTearing = false;
            Hammer.m_AttackSpeed = 1f;
            Hammer.m_DamageType = DataStr.DamageType.Hammer;
            AddDescriptor("GEAR_Hammer", Hammer);

            WeaponDescripter Prybar = new WeaponDescripter();
            Prybar.m_IsMelee = true;
            Prybar.m_PlayerDamage = 15;
            Prybar.m_AnimalDamage = 40;
            Prybar.m_Pain = true;
            Prybar.m_ClothingTearing = false;
            Prybar.m_AttackSpeed = 1.2f;
            Prybar.m_DamageType = DataStr.DamageType.Prybar;
            AddDescriptor("GEAR_Prybar", Prybar);

            WeaponDescripter Knife = new WeaponDescripter();
            Knife.m_IsMelee = true;
            Knife.m_PlayerDamage = 17;
            Knife.m_AnimalDamage = 57;
            Knife.m_BloodLoss = true;
            Knife.m_ClothingTearing = true;
            Knife.m_AttackSpeed = 10f;
            Knife.m_DamageType = DataStr.DamageType.Knife;
            AddDescriptor("GEAR_Knife", Knife);
            AddDescriptor("GEAR_KnifeImprovised", Knife);
            AddDescriptor("GEAR_JeremiahKnife", Knife);
            AddDescriptor("GEAR_KnifeScrapMetal", Knife);

            WeaponDescripter Stone = new WeaponDescripter();
            Stone.m_PlayerDamage = 5;
            Stone.m_Pain = true;
            Stone.m_DamageType = DataStr.DamageType.Stone;
            AddDescriptor("GEAR_Stone", Stone);

            WeaponDescripter NoiseMaker = new WeaponDescripter();
            NoiseMaker.m_PlayerDamage = 10;
            NoiseMaker.m_Pain = true;
            NoiseMaker.m_DamageType = DataStr.DamageType.NoiseMaker;
            AddDescriptor("GEAR_NoiseMaker", NoiseMaker);
        }

        public static void AddDescriptor(string WeaponName, WeaponDescripter Descriptor)
        {
            if(!s_WeaponDescriptors.ContainsKey(WeaponName))
            {
                s_WeaponDescriptors.Add(WeaponName, Descriptor);
            }
            else
            {
                SkyCoop.Logger.Log(ConsoleColor.Red, "AddDescriptor Can't add "+ WeaponName+" because it's already registered!");
            }
        }

        public static WeaponDescripter GetDescriptor(string WeaponName)
        {
            WeaponDescripter Descriptor;
            if(s_WeaponDescriptors.TryGetValue(WeaponName, out Descriptor))
            {
                return Descriptor;
            }
            SkyCoop.Logger.Log(ConsoleColor.Yellow, "GetDescriptor Can't add " + WeaponName + " returning failsafe descriptor");
            Descriptor = new WeaponDescripter();
            Descriptor.m_PlayerDamage = 25;
            Descriptor.m_AnimalDamage = 25;
            return Descriptor;
        }

        public class WeaponDescripter
        {
            public float m_PlayerDamage = 0;
            public float m_AnimalDamage = 0;
            public bool m_BloodLoss = false;
            public bool m_Pain = false;
            public bool m_ClothingTearing = false;
            public bool m_Burn = false;
            public float m_AttackSpeed = 1;
            public DataStr.DamageType m_DamageType = DataStr.DamageType.Unknown;
            public bool m_IsMelee = false;
        }

        public static string GetGearNameByGunType(GunType Weapon)
        {
            switch (Weapon)
            {
                case GunType.Revolver:
                    return "GEAR_Revolver";
                case GunType.Rifle:
                    return "GEAR_Rifle";
                case GunType.FlareGun:
                    return "GEAR_FlareGun";
                default:
                    return "";
            }
        }

        public static DataStr.DamageType GetDamageType(GunType Weapon)
        {
            switch (Weapon)
            {
                case GunType.Revolver:
                    return DataStr.DamageType.Revolver;
                case GunType.Rifle:
                    return DataStr.DamageType.Rifle;
                case GunType.FlareGun:
                    return DataStr.DamageType.FlareGun;
                default:
                    return DataStr.DamageType.Unknown;
            }
        }
        public static DataStr.DamageType GetDamageType(FPSMeshID Weapon)
        {
            switch (Weapon)
            {
                case FPSMeshID.Rifle:
                    return DataStr.DamageType.Rifle;
                case FPSMeshID.PryBar:
                    return DataStr.DamageType.Prybar;
                case FPSMeshID.HuntingKnife:
                    return DataStr.DamageType.Knife;
                case FPSMeshID.Hatchet:
                    return DataStr.DamageType.Hatchet;
                case FPSMeshID.Bow:
                    return DataStr.DamageType.Bow;
                case FPSMeshID.FlareGun:
                    return DataStr.DamageType.FlareGun;
                case FPSMeshID.Brand:
                    return DataStr.DamageType.Knife;
                case FPSMeshID.Stone:
                    return DataStr.DamageType.Stone;
                case FPSMeshID.Revolver:
                    return DataStr.DamageType.Revolver;
                case FPSMeshID.NoiseMaker:
                    return DataStr.DamageType.NoiseMaker;
                default:
                    return DataStr.DamageType.Unknown;
            }
        }

        public static void HandleProjectileSync(int ShooterID, Vector3 Position, Quaternion Rotation, string ProjectileName, float ExtraFloat)
        {
            HandleProjectileSync(ShooterID, Position, Rotation, ProjectileName, Vector3.zero, Vector3.zero, ExtraFloat);
        }

        public static void HandleProjectileSync(int ShooterID, Vector3 Position, Quaternion Rotation, string ProjectileName, Vector3 Velocity, Vector3 AngularVelocity, float Fuse)
        {
            //SkyCoop.Logger.Log("HandleProjectileSync " + ProjectileName);
            //SkyCoop.Logger.Log("HandleProjectileSync Body.velocity " + Velocity.ToString());
            //SkyCoop.Logger.Log("HandleProjectileSync Body.angularVelocity " + AngularVelocity.ToString());
            if (ProjectileName == "Rifle" || ProjectileName == "Revolver")
            {
                GameObject LightFX = new GameObject();
                LightFX.transform.position = Position;
                Light LightComp = LightFX.AddComponent<Light>();
                LightComp.type = LightType.Point;
                LightComp.range = 5;
                LightComp.intensity = 5;
                LightComp.color = new Color(1, 0.5623099f, 0.3268814f, 1);
                UnityEngine.Object.Destroy(LightFX, 0.1f);
            }
            GameObject SoundObj = null;
            GameObject Bullet = null;
            if (ProjectileName == "Rifle")
            {
                SoundObj = AssetManager.GetAssetFromBundle<GameObject>("3DRifleSound");
                if (AssetManager.s_PistolBulletPrefab)
                {
                    Bullet = UnityEngine.Object.Instantiate<GameObject>(AssetManager.s_PistolBulletPrefab, Position, Rotation);
                }
                else
                {
                    SkyCoop.Logger.Log(ConsoleColor.Red, "s_PistolBulletPrefab null!");
                }
            }
            else if (ProjectileName == "Revolver")
            {
                SoundObj = AssetManager.GetAssetFromBundle<GameObject>("3DRevolverSound");
                if (AssetManager.s_RevolverBulletPrefab)
                {
                    Bullet = UnityEngine.Object.Instantiate<GameObject>(AssetManager.s_RevolverBulletPrefab, Position, Rotation);
                }
                else
                {
                    SkyCoop.Logger.Log(ConsoleColor.Red, "s_RevolverBulletPrefab null!");
                }
            }
            else if (ProjectileName == "DryFire")
            {
                SoundObj = AssetManager.GetAssetFromBundle<GameObject>("3DDryFireSound");
            }
            else if (ProjectileName == "GEAR_FlareGunAmmoSingle")
            {
                GameObject FlareShot = FlareGunRoundItem.SpawnAndFire(AssetManager.GetAssetFromGame<GameObject>("GEAR_FlareGunAmmoSingle"), Position, Rotation);
                NetworkPlayer Player = PlayersManager.GetPlayer(ShooterID);
                if (Player)
                {
                    Player.SetIgnorePhysicsForObject(FlareShot);
                    GameObject localplayerColider = new GameObject();
                    localplayerColider.name = "LocalPlayerColider";
                    BoxCollider Colider = localplayerColider.AddComponent<BoxCollider>();
                    Colider.center = new Vector3(0, 0.028f, 0f);
                    Colider.size = new Vector3(0.45f, 0.45f, 0.11f);
                    Colider.extents = new Vector3(0.225f, 0.225f, 0.55f);
                    localplayerColider.transform.SetParent(FlareShot.transform);
                    localplayerColider.layer = vp_Layer.CharacterControllerCollideOnly;
                }
            }else if(ProjectileName == "GEAR_NoiseMaker")
            {
                GameObject Noise = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromGame<GameObject>("GEAR_NoiseMaker"), Position, Rotation);
                NetworkPlayer Player = PlayersManager.GetPlayer(ShooterID);
                if (Player)
                {
                    Player.SetIgnorePhysicsForObject(Noise);
                    //GameObject localplayerColider = new GameObject();
                    //localplayerColider.name = "LocalPlayerColider";
                    //BoxCollider Colider = localplayerColider.AddComponent<BoxCollider>();
                    //Colider.center = new Vector3(0, 0.028f, 0f);
                    //Colider.size = new Vector3(0.45f, 0.45f, 0.11f);
                    //Colider.extents = new Vector3(0.225f, 0.225f, 0.55f);
                    //localplayerColider.transform.SetParent(Noise.transform);
                    //localplayerColider.layer = vp_Layer.CharacterControllerCollideOnly;

                    if(Fuse > 0)
                    {
                        Player.DoThrow();
                    }
                }
                float throwForce = GameManager.m_PlayerManager.m_ThrowForce;
                float num = GameManager.m_PlayerManager.m_ThrowTorque;
                GearItem component = Noise.GetComponent<GearItem>();
                if (component)
                {
                    NoiseMakerItem component1 = component.m_NoiseMakerItem;
                    Noise.AddComponent<Comps.OtherPlayerBullet>();
                    Noise.AddComponent<Comps.NoiseMakerKillFeedHandle>().m_ThrowerID = ShooterID;

                    if (component1)
                    {
                        component1.m_CanThrow = false;
                        component1.Ignite();
                        component1.m_PlayerDamageInflictionInRadius = 15;
                        component1.m_PlayerDamageRadius = component1.m_AIDamageRadius;
                        component1.m_GearItem.SetNormalizedHP(Fuse);
                        component1.m_PlayerDamageInflictionInRadius = 15;
                        component1.m_PlayerDamageRadius = component1.m_AIDamageRadius;
                        if (ProjectileName == "GEAR_NoiseMaker")
                        {
                            component1.PrepareForThrow();
                            component.m_NoiseMakerItem.m_Thrown = true;
                            Rigidbody rigidbody = component.GetComponent<Rigidbody>();
                            if (rigidbody == null)
                            {
                                return;
                            }
                            Utils.SetIsKinematic(rigidbody, false);
                            rigidbody.velocity = Velocity;
                            rigidbody.angularVelocity = AngularVelocity;
                            rigidbody.angularDrag = 0.0f;
                            rigidbody.drag = 0.0f;
                        }
                    }
                }
            }else if(ProjectileName == "GEAR_Stone")
            {
                GameObject Stone = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromGame<GameObject>("GEAR_Stone"), Position, Rotation);
                NetworkPlayer Player = PlayersManager.GetPlayer(ShooterID);
                if (Player)
                {
                    Player.SetIgnorePhysicsForObject(Stone);
                    //GameObject localplayerColider = new GameObject();
                    //localplayerColider.name = "LocalPlayerColider";
                    //BoxCollider Colider = localplayerColider.AddComponent<BoxCollider>();
                    //Colider.center = new Vector3(0, 0.028f, 0f);
                    //Colider.size = new Vector3(0.45f, 0.45f, 0.11f);
                    //Colider.extents = new Vector3(0.225f, 0.225f, 0.55f);
                    //localplayerColider.transform.SetParent(Stone.transform);
                    //localplayerColider.layer = vp_Layer.CharacterControllerCollideOnly;

                    Player.DoThrow();
                }
                GearItem component = Stone.GetComponent<GearItem>();
                Stone.AddComponent<Comps.OtherPlayerBullet>();
                component.m_StoneItem.PrepareForThrow();
                component.m_StoneItem.SetThrown(true);

                Rigidbody rigidbody = Stone.GetComponent<Rigidbody>();
                Utils.SetIsKinematic(rigidbody, false);
                rigidbody.velocity = Velocity;
                rigidbody.angularVelocity = AngularVelocity;
                rigidbody.angularDrag = 0.0f;
                rigidbody.drag = 0.0f;
            }else if(ProjectileName == "Melee")
            {
                if (AssetManager.s_PistolBulletPrefab)
                {
                    Bullet = UnityEngine.Object.Instantiate<GameObject>(AssetManager.s_PistolBulletPrefab, Position, Rotation);
                    if (Bullet)
                    {
                        vp_Bullet Comp = Bullet.GetComponent<vp_Bullet>();
                        if (Comp)
                        {
                            Comp.m_GunType = GunType.Camera;
                            Comp.Range = 2.5f;
                            Comp.m_ImpactAudio = "Play_StoneImpacts";
                        }
                    }
                    NetworkPlayer Player = PlayersManager.GetPlayer(ShooterID);
                    if (Player)
                    {
                        Player.DoHit();
                    }
                }
                else
                {
                    SkyCoop.Logger.Log(ConsoleColor.Red, "s_PistolBulletPrefab is null!");
                }
            } else if(ProjectileName == "GEAR_Arrow" || ProjectileName == "GEAR_ArrowHardened")
            {
                Bullet = UnityEngine.Object.Instantiate<GameObject>(AssetManager.GetAssetFromGame<GameObject>(ProjectileName), Position, Rotation);
                NetworkPlayer Player = PlayersManager.GetPlayer(ShooterID);
                if (Player)
                {
                    Player.SetIgnorePhysicsForObject(Bullet);
                }
                GearItem component = Bullet.GetComponent<GearItem>();
                component.SetNormalizedHP(Fuse);
                Bullet.name = ProjectileName;
                Bullet.transform.parent = (Transform)null;
                component.m_InPlayerInventory = false;
                component.m_StackableItem.m_Units = 1;
                component.m_CurrentHP = 100;
                component.m_ArrowItem.SetPlacementHelperEnabled(true);
                component.m_ArrowItem.Fire(component.m_ArrowItem.m_BowDamageMultiplier);
            }
            if (Bullet)
            {
                if (Bullet.GetComponent<Comps.OtherPlayerBullet>() == null)
                {
                    Bullet.AddComponent<Comps.OtherPlayerBullet>();
                }
            }
            if (SoundObj)
            {
                if (ModMain.s_AppFocus)
                {
                    GameObject SoundEmitter = UnityEngine.Object.Instantiate<GameObject>(SoundObj, Position, Rotation);
                    if (SoundEmitter)
                    {
                        SoundEmitter.GetComponent<AudioSource>().Play();
                        UnityEngine.Object.Destroy(SoundEmitter, 5);
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(vp_Bullet), "Start")]
        private static class vp_Bullet_Start
        {
            private static void Prefix(vp_Bullet __instance)
            {
                Comps.OtherPlayerBullet OPB = __instance.gameObject.GetComponent<Comps.OtherPlayerBullet>();
                if (OPB == null)
                {
                    string ProjectileName = "";
                    if (__instance.m_GunType == GunType.Rifle)
                    {
                        ProjectileName = "Rifle";
                    }
                    else if (__instance.m_GunType == GunType.Revolver)
                    {
                        ProjectileName = "Revolver";
                    }
                    ClientSend.SendProjectile(__instance.transform.position, __instance.transform.rotation, ProjectileName);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(vp_Bullet), "SpawnImpactEffects")]
        private static class vp_Bullet_SpawnImpactEffects
        {
            private static void Postfix(vp_Bullet __instance, RaycastHit hit)
            {
                Comps.OtherPlayerBullet OPB = __instance.gameObject.GetComponent<Comps.OtherPlayerBullet>();
                if (OPB == null)
                {
                    Comps.PlayerDamageColider PlayerColider = hit.collider.gameObject.gameObject.GetComponent<Comps.PlayerDamageColider>();
                    if (PlayerColider)
                    {
                        string WeaponName = "Unknown";
                        Comps.MeleeBulletHandler Melee = __instance.GetComponent<Comps.MeleeBulletHandler>();
                        if (Melee)
                        {
                            WeaponName = Melee.m_GearName;
                            SkyCoop.Logger.Log("Melee hits Player " + PlayerColider.m_Player.m_PlayerID + "  to the " + PlayerColider.m_DamageZone.ToString());
                        }
                        else
                        {
                            WeaponName = GetGearNameByGunType(__instance.m_GunType);
                            SkyCoop.Logger.Log("Bullet hits Player " + PlayerColider.m_Player.m_PlayerID + "  to the " + PlayerColider.m_DamageZone.ToString());
                        }
                        WeaponDescripter weaponDescripter = GetDescriptor(WeaponName);

                        ClientSend.SendDamageToPlayer(weaponDescripter.m_PlayerDamage * PlayerColider.m_DamageScaler, PlayerColider.m_Player.m_PlayerID, PlayerColider.m_DamageZone, WeaponName, weaponDescripter.m_DamageType);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(vp_FPSShooter), "Awake")] // Once
        internal class vp_FPSShooter_Start
        {
            public static void Postfix(vp_FPSShooter __instance)
            {
                if (__instance != null && __instance.gameObject != null && __instance.ProjectilePrefab != null)
                {
                    if (__instance.ProjectilePrefab.name == "PistolBullet")
                    {
                        AssetManager.s_PistolBulletPrefab = __instance.ProjectilePrefab;
                    }
                    if (__instance.ProjectilePrefab.name == "RevolverBullet")
                    {
                        AssetManager.s_RevolverBulletPrefab = __instance.ProjectilePrefab;
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(vp_FPSShooter), "Fire")] // Once
        internal class vp_FPSShooter_Fire
        {
            public static void Prefix(vp_FPSShooter __instance)
            {
                if (__instance.m_Weapon.GetAmmoCount() < 1)
                {
                    ClientSend.SendProjectile(GameManager.GetPlayerTransform().position, GameManager.GetPlayerTransform().rotation, "DryFire");
                    return;
                }
                else
                {
                    if (__instance.m_Weapon.m_GunItem.m_IsJammed)
                    {
                        ClientSend.SendProjectile(GameManager.GetPlayerTransform().position, GameManager.GetPlayerTransform().rotation, "DryFire");
                        return;
                    }
                }
                if (__instance.ProjectilePrefab.name == "GEAR_FlareGunAmmoSingle")
                {
                    ClientSend.SendProjectile(__instance.m_Camera.transform.position, __instance.m_Camera.transform.rotation, "GEAR_FlareGunAmmoSingle");
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BowItem), "ShootArrow")]
        internal class BowItem_ShootArrow
        {
            public static void Prefix(BowItem __instance)
            {
                if (__instance.m_GearArrow)
                {
                    Transform transform = GameManager.GetPlayerAnimationComponent().m_ArrowFirePropPoint.transform;
                    Transform playerTransform = GameManager.GetPlayerTransform();
                    Vector3 Position = playerTransform.TransformPoint(transform.position);
                    Quaternion Rotation = playerTransform.rotation * transform.rotation;
                    ClientSend.SendProjectile(Position, Rotation, __instance.m_GearArrow.name, __instance.m_GearArrow.GetNormalizedCondition());
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GunItem), "Fired")]
        internal class GunItem_Fired
        {
            public static void Postfix(GunItem __instance)
            {
                if (__instance.m_GunType == GunType.FlareGun)
                {
                    Transform T = GameManager.GetVpFPSCamera().CurrentShooter.m_Camera.transform;
                    ClientSend.SendProjectile(T.position, T.rotation, "GEAR_FlareGunAmmoSingle");
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ReleaseThrownObject")] // Once
        public class PlayerManager_Throw
        {
            private static GearItem m_Throw = null;
            public static void Prefix(PlayerManager __instance)
            {
                if (__instance.m_ThrownItem != null)
                {
                    m_Throw = __instance.m_ThrownItem;
                }
            }
            public static void Postfix(PlayerManager __instance)
            {
                if (m_Throw != null)
                {
                    if (m_Throw.m_NoiseMakerItem)
                    {
                        m_Throw.m_NoiseMakerItem.m_PlayerDamageInflictionInRadius = 15;
                        m_Throw.m_NoiseMakerItem.m_PlayerDamageRadius = m_Throw.m_NoiseMakerItem.m_AIDamageRadius;

                        if(ModMain.Client != null && ModMain.Client.m_MyEndPoint != null)
                        {
                            m_Throw.gameObject.AddComponent<Comps.NoiseMakerKillFeedHandle>().m_ThrowerID = ModMain.Client.GetMyId();
                        }
                        
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ReleaseFromHands")] // Once
        public class PlayerManager_ReleaseFromHands
        {
            public static void Postfix(PlayerManager __instance, GameObject go, GameObject __result)
            {
                if (__result != null)
                {
                    GearItem Gi = __result.GetComponent<GearItem>();
                    if (Gi)
                    {
                        if (Gi.m_StoneItem)
                        {
                            Comps.StoneThrowHook Hook = __result.AddComponent<Comps.StoneThrowHook>();
                            Hook.m_StoneItem = Gi.m_StoneItem;
                        }else if (Gi.m_NoiseMakerItem)
                        {
                            Comps.NoiseMakerThrowHook Hook = __result.AddComponent<Comps.NoiseMakerThrowHook>();
                            Hook.m_NoiseMaker = Gi.m_NoiseMakerItem;
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(NoiseMakerItem), "ProcessItemInHandDetonated")] // Once
        public class NoiseMakerItem_ProcessItemInHandDetonated
        {
            public static void Postfix(NoiseMakerItem __instance)
            {
                Transform T = GameManager.GetPlayerTransform();

                ClientSend.SendProjectileThrow(T.position, T.rotation, "GEAR_NoiseMaker", Vector3.zero, Vector3.zero, 0);
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(NoiseMakerItem), "HandleOnThrown")] // Once
        //public class NoiseMakerItem_HandleOnThrown
        //{
        //    public static void Postfix(NoiseMakerItem __instance)
        //    {
        //        Rigidbody Body = __instance.GetComponent<Rigidbody>();
        //        if (Body)
        //        {
        //            ClientSend.SendProjectileThrow(__instance.transform.position, __instance.transform.rotation, "GEAR_NoiseMaker", Body.velocity, Body.angularVelocity, __instance.m_GearItem.GetNormalizedCondition());
        //        }
        //    }
        //}
        [HarmonyLib.HarmonyPatch(typeof(NoiseMakerItem), "PerformDamageToAllInRange")] // Once
        public class NoiseMakerItem_PerformDamageToAllInRange
        {
            public static bool Prefix(NoiseMakerItem __instance)
            {
                foreach (BaseAi baseAi in AiUtils.GetAisWithinRange(__instance.transform.position, __instance.m_AIDamageRadius))
                {
                    baseAi.ApplyDamage(__instance.m_AIDamageInflictionInRadius, DamageSource.NoiseMaker, "noisemaker");
                }
                if ((GameManager.GetPlayerTransform().position - __instance.transform.position).sqrMagnitude <= __instance.m_PlayerDamageRadius * __instance.m_PlayerDamageRadius)
                {
                    Comps.NoiseMakerKillFeedHandle handle = __instance.GetComponent<Comps.NoiseMakerKillFeedHandle>();
                    
                    if (ModMain.Client != null && ModMain.Client.m_MyEndPoint != null && handle)
                    {
                        ClientSend.SendDamageToPlayer(__instance.m_PlayerDamageInflictionInRadius, ModMain.Client.GetMyId(), PlayerDamageColider.DamageZone.Chest, "GEAR_NoiseMaker", DataStr.DamageType.NoiseMaker, handle.m_ThrowerID);
                    }
                }
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(ArrowItem), "Fire")] // Once
        public class ArrowItem_Fire
        {
            public static void Prefix(ArrowItem __instance)
            {
                __instance.gameObject.AddComponent<Comps.ArrowHook>();
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(ArrowItem), "Break")] // Once
        public class ArrowItem_Break
        {
            public static bool Prefix(ArrowItem __instance)
            {
                Comps.ArrowHook Hook = __instance.gameObject.AddComponent<Comps.ArrowHook>();
                bool Other = __instance.gameObject.GetComponent<Comps.OtherPlayerBullet>() != null;
                if (Hook)
                {
                    Hook.m_Broken = true;
                }
                GameObject BrokenArrow = UnityEngine.Object.Instantiate<GameObject>(__instance.m_BrokenArrow, __instance.transform.position, __instance.transform.rotation);
                if (BrokenArrow)
                {
                    BrokenArrow.name = __instance.m_BrokenArrow.name;
                    Rigidbody component = BrokenArrow.GetComponent<Rigidbody>();
                    component.AddForce(__instance.m_Rigidbody.velocity, ForceMode.VelocityChange);
                    component.mass = __instance.m_Rigidbody.mass;
                    component.drag = __instance.m_Rigidbody.drag;
                    component.angularDrag = __instance.m_Rigidbody.angularDrag;
                    BrokenArrow.GetComponent<GearItem>().m_CurrentHP = 0.0025f;

                    if (Other)
                    {
                        BrokenArrow.AddComponent<Comps.OtherPlayerBullet>();
                    }

                    BrokenArrow.AddComponent<Comps.ArrowHook>();

                    Utils.SetIsKinematic(component, false);
                    GameAudioManager.Play3DSound("Play_ArrowBreak", BrokenArrow);
                }
                UnityEngine.Object.Destroy(__instance.gameObject);

                return false;
            }
        }
    }
}
