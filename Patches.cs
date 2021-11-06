using System;
using UnityEngine;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MelonLoader;
using Harmony;
using UnhollowerRuntimeLib;
using UnhollowerBaseLib;
using SkyCoop;
using MelonLoader.TinyJSON;

using GameServer;

namespace SkyCoop
{
    public class Pathes : MelonMod
    {
        public static void SendTCPData(Packet _packet)
        {
            MyMod.SendUDPData(_packet);
        }
        [HarmonyLib.HarmonyPatch(typeof(GearItem), "Drop")]
        public class GearItemDrop
        {
            public static void Prefix(GearItem __instance)
            {
                MelonLogger.Msg("Item dropped " + __instance.m_GearName);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GameManager), "Awake")]
        public class GameManager_Awake
        {

            [HarmonyLib.HarmonyPatch(typeof(Panel_Inventory), "Update")]
            private static class Panel_Inventory_Open
            {
                internal static void Postfix(Panel_Inventory __instance)
                {
                    if (__instance == null)
                    {
                        return;
                    }

                    if(__instance.m_SelectedItemIndex != -1)
                    {
                        int seelecteditem = __instance.m_SelectedItemIndex;
                        seelecteditem = seelecteditem + __instance.m_FirstItemDisplayedIndex;
                        Il2CppSystem.Collections.Generic.List<GearItem> itemlist = __instance.m_FilteredInventoryList;

                        if (__instance.m_FilteredInventoryList.Count > 0)
                        {
                            GearItem gear = __instance.m_FilteredInventoryList.get_Item(seelecteditem);

                            if (gear != null)
                            {
                                string itemname = gear.m_GearName;

                                if (itemname.Contains("(Clone)"))
                                {
                                    int L = itemname.Length - 7;
                                    MyMod.LastSelectedGearName = itemname.Remove(L, 7);
                                }
                                else
                                {
                                    MyMod.LastSelectedGearName = itemname;
                                }

                                if (MyMod.LastSelectedGearName != "gg")
                                {
                                    MyMod.LastSelectedGear = gear;
                                }

                                if (MyMod.NeedRefreshInv == true)
                                {
                                    MyMod.NeedRefreshInv = false;
                                    __instance.m_IsDirty = true;
                                    __instance.Update();
                                }
                            }else{
                                MyMod.LastSelectedGearName = "";
                            }
                        }else{
                            MyMod.LastSelectedGearName = "";
                        }
                    }
                }
            }
            [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "Update")]
            private static class Panel_Rest_Update
            {
                internal static void Postfix(Panel_Rest __instance)
                {
                    //__instance.m_SleepButton.SetActive(false);

                    MyMod.MyCycleSkip = __instance.m_SleepHours;
                }
            }
            [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "OnCancel")]
            private static class Panel_Rest_Close
            {
                internal static void Postfix(Panel_Rest __instance)
                {
                    MelonLogger.Msg("Sleeping menu close.");
                    if (MyMod.SleepingButtons != null)
                    {
                        MyMod.SleepingButtons.SetActive(true);
                    }
                    if (MyMod.WaitForSleepLable != null)
                    {
                        MyMod.WaitForSleepLable.SetActive(false);
                    }
                }
            }
            [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "OnPickUp")]
            private static class Panel_Rest_Close2
            {
                internal static void Postfix(Panel_Rest __instance)
                {
                    MelonLogger.Msg("Sleeping menu close.");
                }
            }
            [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "OnRest")]
            private static class Panel_Rest_Close3
            {
                internal static void Postfix(Panel_Rest __instance)
                {
                    MelonLogger.Msg("Sleeping menu close.");
                }
            }
            [HarmonyLib.HarmonyPatch(typeof(Panel_Rest), "OnPassTime")]
            private static class Panel_Rest_Close4
            {
                internal static void Postfix(Panel_Rest __instance)
                {
                    MelonLogger.Msg("Sleeping menu close.");
                }
            }
            [HarmonyLib.HarmonyPatch(typeof(Panel_Confirmation), "OnConfirm")]
            private static class Panel_Confirmation_Ok
            {
                internal static void Postfix(Panel_Confirmation __instance)
                {
                    if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INPUT SERVER ADDRESS")
                    {
                        string text = __instance.m_CurrentGroup.m_InputField.GetText();
                        MyMod.DoConnectToIp(text);
                    }
                    if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "LOCAL OR STEAM?")
                    {
                        MyMod.HostAServer();
                    }
                    if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INPUT GUID TO TELEPORT TO")
                    {
                        string text = __instance.m_CurrentGroup.m_InputField.GetText();
                        bool found = false;
                        for (int index = 0; index < BaseAiManager.m_BaseAis.Count; ++index)
                        {
                            GameObject animal = BaseAiManager.m_BaseAis.get_Item(index).gameObject;

                            if (animal != null && animal.GetComponent<ObjectGuid>() != null)
                            {
                                if (animal.GetComponent<ObjectGuid>().Get() == text)
                                {
                                    found = true;
                                    GameManager.GetPlayerManagerComponent().TeleportPlayer(animal.transform.position, animal.transform.rotation);
                                    break;
                                }
                            }
                        }

                        if (found == false)
                        {
                            HUDMessage.AddMessage("Animal with GUID " + text + " not exist!");
                        }
                    }
                    if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INPUT ID OF PLAYER TELEPORT TO")
                    {
                        string text = __instance.m_CurrentGroup.m_InputField.GetText();

                        int ID = int.Parse(text);

                        if(ID > MyMod.MaxPlayers-1 || ID < 0)
                        {
                            HUDMessage.AddMessage("Invalid ID of player!");
                        }else{
                            if(MyMod.playersData[ID].m_Levelid != MyMod.levelid)
                            {
                                HUDMessage.AddMessage("You and player on different scenes!");
                            }else{
                                GameManager.GetPlayerManagerComponent().TeleportPlayer(MyMod.playersData[ID].m_Position, MyMod.playersData[ID].m_Rotation);
                                HUDMessage.AddMessage("You has been teleported to "+ MyMod.playersData[ID].m_Name);
                            }
                        }
                    }
                    if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INPUT GUID TO TRACK")
                    {
                        string text = __instance.m_CurrentGroup.m_InputField.GetText();
                        bool found = false;
                        for (int index = 0; index < BaseAiManager.m_BaseAis.Count; ++index)
                        {
                            GameObject animal = BaseAiManager.m_BaseAis.get_Item(index).gameObject;

                            if (animal != null && animal.GetComponent<ObjectGuid>() != null)
                            {
                                if (animal.GetComponent<ObjectGuid>().Get() == text)
                                {
                                    found = true;
                                    MyMod.DebugAnimalGUID = text;
                                    MyMod.DebugAnimalGUIDLast = text;
                                    MyMod.DebugLastAnimal = animal;
                                    HUDMessage.AddMessage("Found animal starting tracking");
                                    break;
                                }
                            }
                        }

                        if (found == false)
                        {
                            HUDMessage.AddMessage("Animal with GUID " + text + " not exist!");
                        }
                    }
                }
            }
            [HarmonyLib.HarmonyPatch(typeof(Weather), "ChooseWeatherSetOfType")]
            private static class Weather_ChooseWeatherSetOfType
            {
                internal static void Postfix(Weather __instance, WeatherStage reqType, WeatherSet __result)
                {
                    string l = MyMod.level_name;

                    if (__result == null  || l == "Empty" || l == "Boot" || l == "MainMenu")
                    {
                        return;
                    }
                    Weather wea = GameManager.GetWeatherComponent();
                    for (int index = 0; index < wea.m_WeatherSetsForScene.Count; ++index)
                    {
                        WeatherSet ForweatherSet = wea.m_WeatherSetsForScene.get_Item(index);
                        if (ForweatherSet == __result)
                        {
                            MelonLogger.Msg("New Weather Stage with set [ID " + index + "]" + __result.name);
                            MyMod.LastSelectedWeatherSet = index;
                            MyMod.LastSelectedWeatherSet2 = __result;
                            break;
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(FirstPersonLightSource), "TurnOnEffects")]
        internal class FirstPersonLightSource_Start
        {

            public static void Prefix(FirstPersonLightSource __instance)
            {
                //MyLightSourceName = __instance.gameObject.name;
                MyMod.MyLightSource = true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(FirstPersonLightSource), "TurnOffEffects")]
        internal class FirstPersonLightSource_End
        {

            public static void Prefix(FirstPersonLightSource __instance)
            {
                //FPH_Match Range - 5 
                //FPH_Torch
                //FPH_KerosceneLamp
                //FPH_Flare
                //FPH_BlueFlare
                //MyLightSourceName = __instance.gameObject.name;
                MyMod.MyLightSource = false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "OnCompletedDecalPlaceDown")]
        internal class PlayerManager_Start
        {

            public static void Prefix(PlayerManager __instance)
            {
                MelonLogger.Msg("Placed Decal " + __instance.m_DecalToPlace.m_DecalName);
                MelonLogger.Msg("X " + __instance.m_DecalToPlace.m_Pos.x + " Y " + __instance.m_DecalToPlace.m_Pos.y + " Z " + __instance.m_DecalToPlace.m_Pos.z);
                if (MyMod.InDarkWalkerMode == true && MyMod.IamShatalker == false && __instance.m_DecalToPlace.m_DecalName == "NowhereToHide_Lure")
                {
                    MyMod.WalkTracker WT = new MyMod.WalkTracker();
                    WT.m_levelid = MyMod.levelid;
                    WT.m_V3 = __instance.m_DecalToPlace.m_Pos;
                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.LUREPLACEMENT))
                        {
                            _packet.Write(WT);
                            SendTCPData(_packet);
                        }
                    }
                    if (MyMod.iAmHost == true)
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.LUREPLACEMENT))
                        {
                            ServerSend.LUREPLACEMENT(1, WT);
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(ObjectAnim), "Play")]
        internal class ObjectAnim_Hack
        {
            public static void Postfix(ObjectAnim __instance, string name)
            {

                //MelonLogger.Msg("ObjectAnim last played anim " + name);
                if (__instance.gameObject != null && __instance.gameObject.GetComponent<MyMod.ContainersSync>() != null)
                {
                    if (__instance.gameObject.GetComponent<MyMod.ContainersSync>().m_LastAnim != name)
                    {
                        __instance.gameObject.GetComponent<MyMod.ContainersSync>().m_LastAnim = name;
                        __instance.gameObject.GetComponent<MyMod.ContainersSync>().CallSync();
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BowItem), "ReleaseFire")]
        internal class BowItem_Shoot
        {
            public static void Prefix(BowItem __instance)
            {
                if (!(bool)(UnityEngine.Object)__instance.m_GearArrow)
                    return;

                if (__instance.m_BowState == BowState.Aim)
                {
                    Transform transform = GameManager.GetPlayerAnimationComponent().m_ArrowFirePropPoint.transform;
                    Transform playerTransform = GameManager.GetPlayerTransform();

                    MelonLogger.Msg("[BowItem] Arrow Fire! " + __instance.m_GearArrow.gameObject.name);
                    MyMod.ShootSync shoot = new MyMod.ShootSync();
                    shoot.m_projectilename = "GEAR_Arrow";
                    shoot.m_position = playerTransform.TransformPoint(transform.position);
                    shoot.m_rotation = playerTransform.rotation * transform.rotation;

                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.SHOOTSYNC))
                        {
                            _packet.Write(shoot);
                            SendTCPData(_packet);
                        }
                    }
                    if (MyMod.iAmHost == true)
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.SHOOTSYNC))
                        {
                            ServerSend.SHOOTSYNC(0, shoot, true);
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(ArrowItem), "HandleCollisionWithObject")]
        internal class ArrowItem_Shoot
        {
            public static void Postfix(ArrowItem __instance)
            {
                if (__instance.gameObject.GetComponent<MyMod.DestoryArrowOnHit>() != null)
                {
                    UnityEngine.Object.Destroy(__instance.gameObject);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(vp_Bullet), "Start")]
        internal class vp_Bullet_Start
        {
            public static bool Prefix(vp_Bullet __instance)
            {
                if (__instance != null && __instance.gameObject != null)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(MyMod.GetGearItemObject("GEAR_Arrow"), new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                    GearItem componentArrow = gameObject.GetComponent<GearItem>();
                    componentArrow.m_ArrowItem.Fire();
                    UnityEngine.Object.Destroy(gameObject);

                    bool NeedPipSkills = false;
                    bool MyBullet = true;

                    if (__instance.gameObject.GetComponent<MyMod.ClientProjectile>() != null)
                    {
                        MyBullet = false;
                    }

                    float RandomRotate = 0;
                    float RandomForce = 0;
                    Vector3 RandomTorque = new Vector3(0, 0, 0);

                    float maxAngleDegrees = 0.0f;
                    if (__instance.m_GunType == GunType.Rifle)
                    {
                        double num = (double)StatsManager.IncrementValue(StatID.RifleShot);
                        maxAngleDegrees = GameManager.GetSkillRifle().GetAimAssistAngleDegrees();
                    }
                    else if (__instance.m_GunType == GunType.Revolver)
                    {
                        double num = (double)StatsManager.IncrementValue(StatID.RevolverShot);
                        maxAngleDegrees = GameManager.GetSkillRevolver().GetAimAssistAngleDegrees();
                    }
                    Vector3 position = __instance.transform.position;
                    Vector3 p2 = position + __instance.transform.forward * 100f;
                    int layerMask = Utils.m_WeaponProjectileCollisionLayerMask | 134217728;
                    RaycastHit hit;

                    if (AiUtils.RaycastWithAimAssist(__instance.transform.position, __instance.transform.forward, out hit, __instance.Range, __instance.MinDistanceForAimAssist, __instance.Accuracy, maxAngleDegrees, layerMask))
                    {
                        p2 = hit.point;
                        Vector3 localScale = __instance.transform.localScale;
                        __instance.transform.parent = hit.transform;
                        __instance.transform.localPosition = hit.transform.InverseTransformPoint(hit.point);
                        __instance.transform.rotation = Quaternion.LookRotation(hit.normal);
                        if (hit.transform.lossyScale == Vector3.one)
                        {
                            RandomRotate = (float)UnityEngine.Random.Range(0, 360);
                            __instance.transform.Rotate(Vector3.forward, RandomRotate, Space.Self);
                        }
                        else
                        {
                            __instance.transform.parent = (Transform)null;
                            __instance.transform.localScale = localScale;
                            __instance.transform.parent = hit.transform;
                        }
                        if ((bool)(UnityEngine.Object)hit.collider)
                            __instance.SpawnImpactEffects(hit);
                        BaseAi baseAiFromObject = AiUtils.GetBaseAiFromObject(hit.collider.gameObject);
                        MyMod.PlayerBulletDamage PlayerDamage = hit.collider.gameObject.GetComponent<MyMod.PlayerBulletDamage>();

                        if (baseAiFromObject != null)
                        {
                            float num1 = Vector3.Distance(GameManager.GetPlayerTransform().position, hit.collider.transform.position);
                            LocalizedDamage component = hit.collider.GetComponent<LocalizedDamage>();
                            if (__instance.m_GunType == GunType.Rifle)
                            {
                                double num2 = (double)StatsManager.IncrementValue(StatID.SuccessfulHits_Rifle);
                            }
                            else if (__instance.m_GunType == GunType.Revolver)
                            {
                                double num3 = (double)StatsManager.IncrementValue(StatID.SuccessfulHits_Revolver);
                            }
                            BodyDamage.Weapon bodyDamageWeapon = GunTypeMethods.ToBodyDamageWeapon(__instance.m_GunType);
                            float bleedOutMinutes = component.GetBleedOutMinutes(bodyDamageWeapon);
                            float num4 = __instance.Damage * component.GetDamageScale(bodyDamageWeapon);
                            if ((double)num1 < (double)__instance.Accuracy)
                            {
                                if (!baseAiFromObject.m_IgnoreCriticalHits && component.RollChanceToKill(bodyDamageWeapon))
                                    num4 = float.PositiveInfinity;
                            }
                            else
                            {
                                float num5 = (num1 - __instance.Accuracy) * __instance.DamageFalloffPerMeterBeyondEffectiveRange;
                                num4 = Mathf.Max(__instance.MinimumDamageFalloffBeyondEffectiveRange, num4 - num5);
                            }
                            if (!Utils.IsZero(num4) || baseAiFromObject.ForceApplyDamage())
                            {
                                if (baseAiFromObject.GetAiMode() != AiMode.Dead)
                                {
                                    MyMod.AnimalUpdates au = baseAiFromObject.gameObject.GetComponent<MyMod.AnimalUpdates>();

                                    bool underMyControl = false;
                                    bool clientControl = false;

                                    if (MyBullet == true)
                                    {
                                        if (MyMod.AnimalsController == true || au.m_ClientController == MyMod.instance.myId)
                                        {
                                            NeedPipSkills = true;
                                        }
                                        else
                                        {
                                            NeedPipSkills = false;
                                        }
                                    }
                                    else
                                    {
                                        NeedPipSkills = false;
                                    }


                                    if (NeedPipSkills == true)
                                    {
                                        if (__instance.m_GunType == GunType.Rifle)
                                        {
                                            GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Rifle, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                                            MelonLogger.Msg("Got skill upgrade from your own shoot Rifle");
                                        }
                                        else if (__instance.m_GunType == GunType.Revolver)
                                        {
                                            GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Revolver, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                                            MelonLogger.Msg("Got skill upgrade from your own shoot Revolver");
                                        }
                                    }
                                    else
                                    {
                                        if (MyBullet == false && ((MyMod.AnimalsController == true && clientControl == false) || (MyMod.AnimalsController == false && underMyControl == true)))
                                        {
                                            MelonLogger.Msg("Remote shoot hit the animal, sending responce to client.");
                                            int SkillTypeId = 0;

                                            if (__instance.m_GunType == GunType.Rifle)
                                            {
                                                SkillTypeId = 1;
                                            }
                                            else if (__instance.m_GunType == GunType.Revolver)
                                            {
                                                SkillTypeId = 2;
                                            }

                                            if (MyMod.sendMyPosition == true)
                                            {
                                                using (Packet _packet = new Packet((int)ClientPackets.PIMPSKILL))
                                                {
                                                    _packet.Write(SkillTypeId);
                                                    SendTCPData(_packet);
                                                }
                                            }
                                            if (MyMod.iAmHost == true)
                                            {
                                                using (Packet _packet = new Packet((int)ServerPackets.PIMPSKILL))
                                                {
                                                    ServerSend.PIMPSKILL(1, SkillTypeId);
                                                }
                                            }
                                        }
                                    }
                                }
                                baseAiFromObject.SetupDamageForAnim(hit.collider.transform.position, GameManager.GetPlayerTransform().position, component);
                                baseAiFromObject.ApplyDamage(num4, bleedOutMinutes, DamageSource.Player, hit.collider.name);
                            }
                        }
                        else if (PlayerDamage != null && MyBullet == true)
                        {
                            MelonLogger.Msg("You damaged other player on " + PlayerDamage.m_Damage);
                            if (MyMod.sendMyPosition == true)
                            {
                                using (Packet _packet = new Packet((int)ClientPackets.BULLETDAMAGE))
                                {
                                    _packet.Write((float)PlayerDamage.m_Damage);
                                    _packet.Write(PlayerDamage.m_ClientId);

                                    SendTCPData(_packet);
                                }
                            }
                            if (MyMod.iAmHost == true)
                            {
                                using (Packet _packet = new Packet((int)ServerPackets.BULLETDAMAGE))
                                {
                                    ServerSend.BULLETDAMAGE(PlayerDamage.m_ClientId, (float)PlayerDamage.m_Damage);
                                }
                            }
                        }
                        else
                        {
                            GearItem gearItemFromObject = Utils.GetGearItemFromObject(hit.collider.gameObject);
                            if ((bool)(UnityEngine.Object)gearItemFromObject)
                            {
                                RandomForce = UnityEngine.Random.Range(0.0f, __instance.m_GearImpactUpwardForce);
                                RandomTorque = new Vector3(UnityEngine.Random.Range(-__instance.m_GearImpactTorqueForce, __instance.m_GearImpactTorqueForce), UnityEngine.Random.Range(-__instance.m_GearImpactTorqueForce, __instance.m_GearImpactTorqueForce), UnityEngine.Random.Range(-__instance.m_GearImpactTorqueForce, __instance.m_GearImpactTorqueForce));
                                Vector3 force = -hit.normal * __instance.m_GearImpactForce + Vector3.up * RandomForce;
                                gearItemFromObject.ApplyForce(force, RandomTorque);
                            }
                        }
                        Renderer component1 = __instance.gameObject.GetComponent<Renderer>();
                        if ((UnityEngine.Object)component1 != (UnityEngine.Object)null && component1.enabled)
                            vp_DecalManager.Add(__instance.gameObject);
                        else
                            UnityEngine.Object.Destroy((UnityEngine.Object)__instance.gameObject);
                    }
                    else
                    {
                        UnityEngine.Object.Destroy((UnityEngine.Object)__instance.gameObject);
                        Utils.DebugBulletHit(position, p2);
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(vp_FPSShooter), "Start")]
        internal class vp_FPSShooter_Start
        {
            public static void Postfix(vp_FPSShooter __instance)
            {
                if (__instance != null && __instance.gameObject != null && __instance.ProjectilePrefab != null)
                {
                    if (__instance.gameObject.name == "Rifle" && __instance.ProjectilePrefab.name == "PistolBullet")
                    {
                        MyMod.PistolBulletPrefab = __instance.ProjectilePrefab;
                    }
                    if (__instance.gameObject.name == "Revolver" && __instance.ProjectilePrefab.name == "RevolverBullet")
                    {
                        MyMod.RevolverBulletPrefab = __instance.ProjectilePrefab;
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(vp_FPSShooter), "Fire")]
        internal class vp_FPSShooter_End
        {

            public static void Prefix(vp_FPSShooter __instance)
            {
                if ((UnityEngine.Object)__instance.m_Weapon == (UnityEngine.Object)null || (double)Time.time < (double)__instance.m_NextAllowedFireTime || (__instance.m_Weapon.ReloadInProgress() || !GameManager.GetPlayerAnimationComponent().IsAllowedToFire(__instance.m_Weapon.m_GunItem.m_AllowHipFire)) || GameManager.GetPlayerAnimationComponent().IsReloading())
                {
                    //MelonLogger.Msg("[vp_FPSShooter] Can't shoot now!");
                    return;
                }
                if (__instance.m_Weapon.GetAmmoCount() < 1)
                {
                    //MelonLogger.Msg("[vp_FPSShooter] Dry fire!");
                    MyMod.SendMultiplayerAudio("PLAY_RIFLE_DRY_3D");
                    return;
                }
                else
                {
                    if (__instance.m_Weapon.m_GunItem.m_IsJammed)
                    {
                        //MelonLogger.Msg("[vp_FPSShooter] Jammed!");
                        MyMod.SendMultiplayerAudio("PLAY_RIFLE_DRY_3D");
                        return;
                    }
                }
                Vector3 vector3 = __instance.m_Camera.transform.position;
                Quaternion quaternion = __instance.m_Camera.transform.rotation;

                for (int index = 0; index < __instance.ProjectileCount; ++index)
                {
                    if ((UnityEngine.Object)__instance.ProjectilePrefab != (UnityEngine.Object)null)
                    {
                        if (__instance.ProjectileCustomPrefab)
                        {
                            MelonLogger.Msg("[vp_FPSShooter] Flaregun projectile spawn! " + __instance.ProjectilePrefab.name);
                        }
                        else
                        {
                            MelonLogger.Msg("[vp_FPSShooter] Bullet projectile spawn " + __instance.ProjectilePrefab.name);
                        }

                        MyMod.ShootSync shoot = new MyMod.ShootSync();

                        shoot.m_projectilename = __instance.ProjectilePrefab.name;
                        shoot.m_position = vector3;
                        shoot.m_rotation = quaternion;

                        if (__instance.ProjectilePrefab.name == "PistolBullet")
                        {
                            shoot.m_skill = GameManager.GetSkillRifle().GetEffectiveRange();
                        }

                        if (MyMod.sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.SHOOTSYNC))
                            {
                                _packet.Write(shoot);
                                SendTCPData(_packet);
                            }
                        }
                        if (MyMod.iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.SHOOTSYNC))
                            {
                                ServerSend.SHOOTSYNC(0, shoot, true);
                            }
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GearItem), "ManualUpdate")]
        private static class OverrideMedkit
        {
            internal static void Postfix(GearItem __instance)
            {
                if (__instance.name == "GEAR_MedicalSupplies_hangar")
                {
                    __instance.m_WeightKG = 0.5f;
                    FirstPersonItem fp = __instance.gameObject.AddComponent<FirstPersonItem>();
                    fp.m_FPSWeapon = GameManager.GetVpFPSCamera().GetWeaponFromID(5);
                    fp.m_FPSMeshID = 5;
                    __instance.m_EmergencyStim = new EmergencyStimItem();
                }
            }
        }

        public static bool DuppableGearItem(string GearName)
        {
            if (GearName.Contains("TechnicalBackpack") == true 
                || GearName.Contains("Crampons") == true
                || GearName.Contains("CanneryCodeNote") == true
                || GearName.Contains("MountainTownFarmKey") == true)
            {
                MelonLogger.Msg(ConsoleColor.Blue, "Item " + GearName + " is can be picked by other player");
                return true;
            }else{
                return false;
            }
        }
        public static bool GarbadgeFilter(string GearName)
        {
            if (GearName == "GEAR_CrowFeather" 
                || GearName == "GEAR_Stone" 
                || GearName == "GEAR_Stick")
            {
                return true;
            }
            return false;
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ProcessInspectablePickupItem")]
        private static class Inventory_Pickup
        {
            internal static void Prefix(PlayerManager __instance, GearItem pickupItem)
            {
                if (pickupItem.m_BeenInPlayerInventory == false)
                {
                    MelonLogger.Msg("Pickedup " + pickupItem.m_GearName);
                    pickupItem.m_BeenInPlayerInventory = true;
                    if(DuppableGearItem(pickupItem.m_GearName) == true)
                    {
                        return;
                    }
                    if (GarbadgeFilter(pickupItem.m_GearName))
                    {
                        return;
                    }
                    MyMod.AddPickedGear(pickupItem.gameObject.transform.position, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent,MyMod.instance.myId, pickupItem.m_InstanceID, true);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ProcessPickupWithNoInspectScreen")]
        private static class Inventory_Pickup2
        {
            internal static void Prefix(PlayerManager __instance, GearItem pickupItem)
            {
                if (pickupItem.m_BeenInPlayerInventory == false)
                {
                    MelonLogger.Msg("Pickedup " + pickupItem.m_GearName);
                    pickupItem.m_BeenInPlayerInventory = true;
                    if (DuppableGearItem(pickupItem.m_GearName) == true)
                    {
                        return;
                    }
                    if (GarbadgeFilter(pickupItem.m_GearName))
                    {
                        return;
                    }
                    MyMod.AddPickedGear(pickupItem.gameObject.transform.position, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent, MyMod.instance.myId, pickupItem.m_InstanceID, true);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InitializeObjectToPlace")]
        private static class Inventory_Pickup3
        {
            public static void Prefix(PlayerManager __instance, GameObject go)
            {
                if (go != null && go.GetComponent<GearItem>() != null)
                {
                    GearItem pickupItem = go.GetComponent<GearItem>();
                    if (pickupItem.m_BeenInPlayerInventory == false)
                    {
                        MelonLogger.Msg("Pickedup " + pickupItem.m_GearName);
                        pickupItem.m_BeenInPlayerInventory = true;
                        if (DuppableGearItem(pickupItem.m_GearName) == true)
                        {
                            return;
                        }
                        if (GarbadgeFilter(pickupItem.m_GearName))
                        {
                            return;
                        }
                        MyMod.AddPickedGear(pickupItem.gameObject.transform.position, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent, MyMod.instance.myId, pickupItem.m_InstanceID, true);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BlueprintDisplayItem), "Setup")]
        private static class FixRecipeIcons
        {
            internal static void Postfix(BlueprintDisplayItem __instance, BlueprintItem bpi)
            {
                if (bpi?.m_CraftedResult?.name == "GEAR_MedicalSupplies_hangar")
                {
                    Texture2D medkitTexture = Utils.GetCachedTexture("ico_CraftItem__MedicalSupplies_hangar");
                    if (!medkitTexture)
                    {
                        medkitTexture = MyMod.LoadedBundle.LoadAsset("ico_CraftItem__MedicalSupplies_hangar").Cast<Texture2D>();
                        Utils.CacheTexture("ico_CraftItem__MedicalSupplies_hangar", medkitTexture);
                    }
                    __instance.m_Icon.mTexture = medkitTexture;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_Crafting), "ItemPassesFilter")]
        private static class ShowRecipesInMedsTab
        {
            internal static void Postfix(Panel_Crafting __instance, ref bool __result, BlueprintItem bpi)
            {
                if (bpi?.m_CraftedResult?.name == "GEAR_MedicalSupplies_hangar" && __instance.m_CurrentCategory == Panel_Crafting.Category.FirstAid)
                {
                    __result = true;
                }
                if (bpi?.m_CraftedResult?.name == "GEAR_MedicalSupplies_hangar" && __instance.m_CurrentCategory != Panel_Crafting.Category.FirstAid)
                {
                    __result = false;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Container), "Start")]
        private static class Container_Hack
        {
            internal static void Postfix(Container __instance)
            {
                if (__instance != null && __instance.gameObject != null)
                {
                    __instance.gameObject.AddComponent<MyMod.ContainersSync>();
                    __instance.gameObject.GetComponent<MyMod.ContainersSync>().m_Obj = __instance.gameObject;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "OnSelectFeatsBack")]
        internal class Panel_MainMenu_FeatBack
        {
            public static bool Prefix(Panel_MainMenu __instance)
            {
                if (MyMod.PendingSave != null)
                {
                    MyMod.SelectGenderForConnection();
                    return false;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "OnSelectFeatsContinue")]
        internal class Panel_MainMenu_FeatContinue
        {
            public static bool Prefix(Panel_MainMenu __instance)
            {
                if (MyMod.PendingSave != null)
                {
                    GameAudioManager.PlayGuiConfirm();
                    FeatEnabledTracker.m_FeatsEnabledThisSandbox = new Il2CppSystem.Collections.Generic.List<FeatType>();
                    for (int index1 = 0; index1 < __instance.m_SelectedFeats.Count; ++index1)
                    {
                        for (int index2 = 0; index2 < GameManager.GetFeatsManager().GetNumFeats(); ++index2)
                        {
                            Feat featFromIndex = GameManager.GetFeatsManager().GetFeatFromIndex(index2);
                            if (__instance.m_SelectedFeats.get_Item(index1) == featFromIndex.m_LocalizedDisplayName.m_LocalizationID)
                            {
                                FeatEnabledTracker.m_FeatsEnabledThisSandbox.Add(featFromIndex.m_FeatType);
                            }
                        }
                    }
                    MyMod.ForcedCreateSave(MyMod.PendingSave);
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectRegion_Map), "OnSelectRegionContinue")]
        internal class Panel_SelectRegion_Map_Done
        {
            public static bool Prefix(Panel_SelectRegion_Map __instance)
            {
                if (MyMod.PendingSave != null)
                {
                    return false;
                }
                return true;
            }
            public static void Postfix(Panel_SelectRegion_Map __instance)
            {
                if (MyMod.PendingSave != null)
                {
                    InterfaceManager.m_Panel_OptionsMenu.m_State.m_StartRegion = __instance.m_SelectedItem.m_Region;
                    MyMod.PendingSave.m_Location = (int)__instance.m_SelectedItem.m_Region;
                    __instance.Enable(false);
                    MyMod.SelectGenderForConnection();
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectRegion), "OnSelectRegionContinue")]
        internal class Panel_SelectRegion_Done
        {
            public static bool Prefix(Panel_SelectRegion __instance)
            {
                if (MyMod.PendingSave != null)
                {
                    __instance.Enable(false);
                    MyMod.SelectGenderForConnection();
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectRegion_Map), "OnClickBack")]
        internal class Panel_SelectRegion_Map_OnClickBack
        {
            public static bool Prefix(Panel_SelectRegion_Map __instance)
            {
                if (MyMod.PendingSave != null)
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectRegion), "OnClickBack")]
        internal class Panel_SelectRegion_OnClickBack
        {
            public static bool Prefix(Panel_SelectRegion __instance)
            {
                if (MyMod.PendingSave != null)
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectSurvivor), "OnSelectSurvivorMale")]
        internal class Panel_SelectSurvivor_Select1
        {
            public static bool Prefix(Panel_SelectSurvivor __instance)
            {
                if (MyMod.PendingSave != null)
                {
                    InterfaceManager.m_Panel_OptionsMenu.m_State.m_VoicePersona = VoicePersona.Male;
                    __instance.Enable(false);
                    MyMod.SelectBagesForConnection();

                    return false;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectSurvivor), "OnSelectSurvivorFemale")]
        internal class Panel_SelectSurvivor_Select2
        {
            public static bool Prefix(Panel_SelectSurvivor __instance)
            {
                if (MyMod.PendingSave != null)
                {
                    InterfaceManager.m_Panel_OptionsMenu.m_State.m_VoicePersona = VoicePersona.Female;
                    __instance.Enable(false);
                    MyMod.SelectBagesForConnection();
                    
                    return false;
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BaseAi), "AnimSetTrigger")]
        private static class GetID
        {
            internal static void Postfix(BaseAi __instance, int id)
            {
                if (__instance != null && __instance.gameObject != null && __instance.gameObject.GetComponent<ObjectGuid>() != null && __instance.gameObject.activeSelf == true)
                {
                    if (MyMod.iAmHost == true && MyMod.AnimalsController == true)
                    {
                        GameObject animal = __instance.gameObject;
                        //MelonLogger.Msg("Animal with GUID " + animal.GetComponent<ObjectGuid>().Get() + " used trigger with hash name " + id);

                        MyMod.AnimalTrigger trigg = new MyMod.AnimalTrigger();

                        trigg.m_Guid = animal.GetComponent<ObjectGuid>().Get();
                        trigg.m_Trigger = id;

                        if (MyMod.iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.ANIMALSYNCTRIGG))
                            {
                                ServerSend.ANIMALSYNCTRIGG(0, trigg, true);
                            }
                        }

                        if (MyMod.sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.ANIMALSYNCTRIGG))
                            {
                                _packet.Write(trigg);
                                SendTCPData(_packet);
                            }
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BaseAi), "Update")]
        private static class AI_Hack
        {
            internal static void Postfix(BaseAi __instance)
            {
                if (__instance != null && __instance.gameObject != null && __instance.gameObject.GetComponent<ObjectGuid>() != null && __instance.gameObject.activeSelf == true)
                {
                    GameObject animal = __instance.gameObject;
                    string _guid = "";

                    if (MyMod.NoRabbits == true)
                    {
                        if (animal.name.Contains("Rabbit") && animal.activeSelf == true)
                        {
                            animal.SetActive(false);
                        }
                    }

                    if (animal != null)
                    {
                        MyMod.AnimalUpdates au = animal.GetComponent<MyMod.AnimalUpdates>();
                        if (animal.GetComponent<ObjectGuid>() != null)
                        {
                            _guid = animal.GetComponent<ObjectGuid>().Get();
                        }
                        if (au == null)
                        {
                            //MelonLogger.Msg("Added AnimalUpdates");
                            animal.AddComponent<MyMod.AnimalUpdates>();
                            au = animal.GetComponent<MyMod.AnimalUpdates>();
                            au.m_Animal = __instance.gameObject;
                            //animal.name = animal.name + "_MULTIPLAYER_" + _guid;
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_BodyHarvest), "Update")]
        private static class Panel_BodyHarvest_KickOutFromShokal
        {
            internal static void Postfix(Panel_BodyHarvest __instance)
            {
                if (__instance != null && __instance.m_BodyHarvest != null && __instance.m_BodyHarvest.gameObject != null && __instance.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    GameObject shokal = __instance.m_BodyHarvest.gameObject;

                    for (int i = 0; i < MyMod.playersData.Count; i++)
                    {
                        if (MyMod.playersData[i] != null)
                        {
                            string otherAnimlGuid = MyMod.playersData[i].m_HarvestingAnimal;
                            if (otherAnimlGuid == shokal.GetComponent<ObjectGuid>().Get())
                            {
                                MyMod.ExitHarvesting();
                                break;
                            }
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ProcessBodyHarvestInteraction")]
        private static class BodyHarvest_OutOfSomeoneAlready
        {
            internal static bool Prefix(BodyHarvest bh, bool playBookEndAnim)
            {
                GameObject shokal = bh.gameObject;

                for (int i = 0; i < MyMod.playersData.Count; i++)
                {
                    if (MyMod.playersData[i] != null)
                    {
                        string otherAnimlGuid = MyMod.playersData[i].m_HarvestingAnimal;
                        if (otherAnimlGuid == shokal.GetComponent<ObjectGuid>().Get())
                        {
                            HUDMessage.AddMessage(MyMod.playersData[i].m_Name + " IS BUSY WITH THIS ALREADY");
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BodyHarvest), "UpdateBodyHarvest")]
        private static class BodyHarvest_UpdateBodyHarvest
        {
            internal static bool Prefix(BodyHarvest __instance, float todHours)
            {
                if (__instance != null && __instance.gameObject != null)
                {
                    float _todHours = Time.deltaTime * (24f / GameManager.GetTimeOfDayComponent().GetDayLengthSecondsUnscaled());
                    __instance.MaybeDecay(_todHours);
                    __instance.MaybeFreeze(_todHours);
                    if (!__instance.ConditionReachedZero() && !__instance.NoMoreResources())
                        return false;
                    __instance.DestroyIfFarAway();
                }
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_BodyHarvest), "HarvestSuccessful")]
        private static class Panel_BodyHarvest_Hack
        {
            internal static void Prefix(Panel_BodyHarvest __instance)
            {
                if (__instance != null && __instance.m_BodyHarvest != null && __instance.m_BodyHarvest.gameObject != null && __instance.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    MelonLogger.Msg("Harvested meant " + __instance.m_MenuItem_Meat.m_HarvestAmount);
                    MelonLogger.Msg("Harvested guts " + __instance.m_MenuItem_Gut.m_HarvestAmount);
                    MelonLogger.Msg("Harvested hide " + __instance.m_MenuItem_Hide.m_HarvestAmount);

                    MyMod.HarvestStats Harvey = new MyMod.HarvestStats();
                    Harvey.m_Meat = __instance.m_MenuItem_Meat.m_HarvestAmount;
                    Harvey.m_Guts = (int)__instance.m_MenuItem_Gut.m_HarvestAmount;
                    Harvey.m_Hide = (int)__instance.m_MenuItem_Hide.m_HarvestAmount;
                    Harvey.m_Guid = __instance.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>().Get();

                    if (MyMod.AnimalsController == false)
                    {
                        if (MyMod.sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.DONEHARVASTING))
                            {
                                _packet.Write(Harvey);
                                SendTCPData(_packet);
                            }
                        }
                        if (MyMod.iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.DONEHARVASTING))
                            {
                                ServerSend.DONEHARVASTING(0, Harvey, true);
                            }
                        }
                    }
                }
            }
        }

        public static void SendDamageAnimal(MyMod.AnimalUpdates au, float damage)
        {
            if (MyMod.iAmHost == true)
            {
                ServerSend.ANIMALDAMAGE(0, au.gameObject.GetComponent<ObjectGuid>().Get(), damage, au.m_ClientController);
            }
            if(MyMod.sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.ANIMALDAMAGE))
                {
                    _packet.Write(au.gameObject.GetComponent<ObjectGuid>().Get());
                    _packet.Write(damage);
                    _packet.Write(au.m_ClientController);
                    SendTCPData(_packet);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BaseAi), "ApplyDamage", new System.Type[] { typeof(float), typeof(float), typeof(DamageSource), typeof(string) })]
        private static class AI_Hack_Damage
        {
            internal static bool Prefix(BaseAi __instance, float damage)
            {
                MyMod.AnimalUpdates au = __instance.gameObject.GetComponent<MyMod.AnimalUpdates>();

                if ((MyMod.AnimalsController == true && au.m_ClientControlled == false) || au.m_ClientController == MyMod.instance.myId)
                {
                    return true;
                }else{
                    SendDamageAnimal(au, damage);
                    return false;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(ArrowItem), "InflictDamage", new System.Type[] { typeof(GameObject), typeof(float), typeof(bool), typeof(string), typeof(Vector3) })]
        private static class ArrowItem_DamageFix
        {
            internal static bool Prefix()
            {
                return false;
            }
            internal static void Postfix(ArrowItem __instance, GameObject victim, float damageScalar, bool stickIn, string collider, Vector3 collisionPoint, BaseAi __result)
            {
                BaseAi baseAi = (BaseAi)null;
                if (victim.layer == 16)
                    baseAi = victim.GetComponent<BaseAi>();
                else if (victim.layer == 27)
                    baseAi = victim.transform.GetComponentInParent<BaseAi>();
                if ((UnityEngine.Object)baseAi == (UnityEngine.Object)null)
                {
                    __result = null;
                    return;
                }

                LocalizedDamage component = victim.GetComponent<LocalizedDamage>();
                double num = (double)StatsManager.IncrementValue(StatID.SuccessfulHits_Bow);
                float bleedOutMinutes = component.GetBleedOutMinutes(BodyDamage.Weapon.Arrow);
                float damage = __instance.m_VictimDamage * damageScalar * component.GetDamageScale(BodyDamage.Weapon.Arrow);
                if (!baseAi.m_IgnoreCriticalHits && component.RollChanceToKill(BodyDamage.Weapon.Arrow))
                {
                    damage = float.PositiveInfinity;
                }
                    
                if (baseAi.GetAiMode() != AiMode.Dead)
                {
                    if (__instance.gameObject != null && __instance.gameObject.GetComponent<MyMod.DestoryArrowOnHit>() == null)
                    {
                        MelonLogger.Msg("I am hit target with bow");
                        GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Archery, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                    }else{
                        MelonLogger.Msg("Other player hit targer with bow");
                    }
                }
                baseAi.SetupDamageForAnim(collisionPoint, GameManager.GetPlayerTransform().position, component);
                baseAi.ApplyDamage(damage, !stickIn ? 0.0f : bleedOutMinutes, DamageSource.Player, collider);
                __result = baseAi;
                return;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(CarryableBody), "Update")]
        public class CarryableBody_Vzlom_Djopi
        {
            public static void Prefix(CarryableBody __instance)
            {
                if (__instance.gameObject != null && __instance.gameObject.GetComponent<ObjectGuid>() && __instance.gameObject.GetComponent<ObjectGuid>().Get() != MyMod.PlayerBodyGUI)
                {
                    UnityEngine.Object.Destroy(__instance.gameObject);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(LoadScene), "Activate")]
        public class LoadScene_Enter
        {
            public static void Prefix(LoadScene __instance)
            {
                if (__instance.gameObject != null && __instance.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    string DoorGUID = __instance.gameObject.GetComponent<ObjectGuid>().Get();

                    MelonLogger.Msg("Entering door " + DoorGUID);

                    if (MyMod.CarryingPlayer == true)
                    {
                        if (MyMod.sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.BODYWARP))
                            {
                                _packet.Write(DoorGUID);
                                SendTCPData(_packet);
                            }
                        }

                        if (MyMod.iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.BODYWARP))
                            {
                                ServerSend.BODYWARP(1, DoorGUID);
                            }
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(LoadScene), "LoadLevelWhenFadeOutComplete")]
        public class LoadScene_Load
        {
            public static bool Prefix(LoadScene __instance)
            {
                MyMod.NotNeedToPauseUntilLoaded = true;
                //if (GameManager.GetPlayerManagerComponent().PlayerIsDead() && IsCarringMe == false )
                //    return false;
                string str = (string)null;
                if (__instance.m_SceneCanBeInstanced)
                    str = GameManager.StripOptFromSceneName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                string sceneName = __instance.m_SceneToLoad;
                if (GameManager.m_SceneTransitionData.m_ForceNextSceneLoadTriggerScene != null)
                    sceneName = GameManager.m_SceneTransitionData.m_ForceNextSceneLoadTriggerScene;
                GameManager.m_SceneTransitionData.m_SpawnPointName = __instance.m_ExitPointName;
                GameManager.m_SceneTransitionData.m_SpawnPointAudio = __instance.m_SoundDuringFadeIn;
                GameManager.m_SceneTransitionData.m_ForceSceneOnNextNavMapLoad = (string)null;
                GameManager.m_SceneTransitionData.m_ForceNextSceneLoadTriggerScene = str;
                GameManager.m_SceneTransitionData.m_SceneLocationLocIDToShow = __instance.m_SceneLocationLocIDToShow;
                GameManager.m_SceneTransitionData.m_Location = (string)null;
                GameRegion UselessDummy = new GameRegion();
                if (RegionManager.GetRegionFromString(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, out UselessDummy))
                    GameManager.m_SceneTransitionData.m_PosBeforeInteriorLoad = __instance.gameObject.transform.position;
                if (__instance.m_SceneCanBeInstanced)
                {
                    GameManager.m_SceneTransitionData.m_PosBeforeInteriorLoad = __instance.gameObject.transform.position;
                    GameManager.m_SceneTransitionData.m_SceneSaveFilenameNextLoad = sceneName + "_" + __instance.m_GUID;
                }
                else
                    GameManager.m_SceneTransitionData.m_SceneSaveFilenameNextLoad = sceneName;
                GameManager.LoadScene(sceneName, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent);
                return false;
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(Condition), "PlayerDeath")]
        //public class Condition_Test
        //{
        //    public static bool Prefix(Condition __instance)
        //    {
        //        MelonLogger.Msg("[Condition] PlayerDeath");
        //        if (MyMod.InOnline() == true)
        //        {
        //            //InterfaceManager.m_Panel_Log.Enable(false);
        //            InterfaceManager.m_Panel_HUD.m_Sprite_SystemFadeOverlay.enabled = false;
        //            __instance.PlayPlayerDeathAudio();
        //            GameManager.GetPlayerManagerComponent().UnequipImmediate(false);
        //            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Dead);
        //            if (GameManager.m_PlayerStruggle != null && GameManager.m_PlayerStruggle.m_PartnerBaseAi != null)
        //            {
        //                //GameManager.m_PlayerStruggle.m_PlayerDied = false;
        //                if (GameManager.m_PlayerStruggle.m_PartnerBaseAi.gameObject.name.Contains("WILDLIFE_Bear") != true && GameManager.m_PlayerStruggle.m_PartnerBaseAi.gameObject.name.Contains("WILDLIFE_Moose") != true)
        //                {
        //                    MelonLogger.Msg("[Condition] DoFakeGetup");
        //                    GameManager.GetPlayerStruggleComponent().MakePartnerFlee();
        //                    MyMod.DoFakeGetup = true;
        //                }
        //            }
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(PlayerStruggle), "GetUpAnimationComplete")]
        //public class PlayerStruggle_Over
        //{
        //    public static void Prefix(PlayerStruggle __instance)
        //    {
        //        MelonLogger.Msg("[PlayerStruggle] Getup done");
        //        MyMod.DoFakeGetup = false;
        //        if (MyMod.LastStruggleAnimalName.Contains("WILDLIFE_Bear") == true)
        //        {
        //            MelonLogger.Msg("[PlayerStruggle] Struggle with bear complete, doing late damage");
        //            GameManager.GetConditionComponent().m_NeverDie = false;
        //            MyMod.NeedDoBearDamage = true;
        //            __instance.ApplyBearDamageAfterStruggleEnds();
        //        }
        //        if (MyMod.LastStruggleAnimalName.Contains("WILDLIFE_Moose") == true)
        //        {
        //            MelonLogger.Msg("[PlayerStruggle] Struggle with moose complete, doing late damage");
        //            GameManager.GetConditionComponent().m_NeverDie = false;
        //            MyMod.NeedDoMooseDamage = true;
        //            __instance.ApplyMooseDamageAfterStruggleEnds();
        //        }
        //    }
        //}

        [HarmonyLib.HarmonyPatch(typeof(WildlifeItem), "ProcessInteraction")]
        public class WildlifeItem_Pickup
        {
            public static bool Prefix(WildlifeItem __instance)
            {
                MelonLogger.Msg("[WildlifeItem] ProcessInteraction");
                if (!GameManager.GetPlayerAnimationComponent().CanInteract())
                {
                    return false;
                }

                if (__instance.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    MelonLogger.Msg("[WildlifeItem] Pickedup " + __instance.gameObject.name + " " + __instance.gameObject.GetComponent<ObjectGuid>().Get());
                    if (MyMod.iAmHost == true)
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.ANIMALDELETE))
                        {
                            ServerSend.ANIMALDELETE(1, __instance.gameObject.GetComponent<ObjectGuid>().Get());
                        }
                    }

                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.ANIMALDELETE))
                        {
                            _packet.Write(__instance.gameObject.GetComponent<ObjectGuid>().Get());
                            SendTCPData(_packet);
                        }
                    }
                }

                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ReleaseThrownObject")]
        public class PlayerManager_Throw
        {
            public static void Prefix(PlayerManager __instance)
            {
                if (__instance.m_ThrownItem != null)
                {
                    MelonLogger.Msg("[PlayerManager][PreFix] ReleaseThrownObject " + __instance.m_ThrownItem.m_GearName);
                    MyMod.SaveThrowingItem = __instance.m_ThrownItem;
                }
                else
                {
                    MelonLogger.Msg("[PlayerManager][PreFix] Trying throw NULL somehow, wot?");
                }
            }
            public static void Postfix(PlayerManager __instance)
            {
                if (MyMod.SaveThrowingItem != null)
                {
                    MelonLogger.Msg("[PlayerManager][Postfix] ReleaseThrownObject SaveThrowingItem " + MyMod.SaveThrowingItem.name);

                    if (MyMod.SaveThrowingItem.name.StartsWith("GEAR_Stone"))
                    {
                        Vector3 V3 = MyMod.SaveThrowingItem.transform.position;
                        Quaternion Qu = MyMod.SaveThrowingItem.transform.rotation;

                        MelonLogger.Msg("[PlayerManager][Postfix] Throwing stone " + V3.x + " y " + V3.y + " z " + V3.z);

                        MyMod.ShootSync stone = new MyMod.ShootSync();
                        stone.m_position = V3;
                        stone.m_rotation = Qu;
                        stone.m_projectilename = "GEAR_Stone";
                        stone.m_skill = 0;
                        stone.m_camera_forward = GameManager.GetVpFPSCamera().transform.forward;
                        stone.m_camera_right = GameManager.GetVpFPSCamera().transform.right;
                        stone.m_camera_up = GameManager.GetVpFPSCamera().transform.up;
                        if (MyMod.sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.SHOOTSYNC))
                            {
                                _packet.Write(stone);
                                SendTCPData(_packet);
                            }
                        }
                        if (MyMod.iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.SHOOTSYNC))
                            {
                                ServerSend.SHOOTSYNC(0, stone, true);
                            }
                        }
                    }
                }
                else
                {
                    MelonLogger.Msg("[PlayerManager][PostFix] SaveThrowingItem is NULL");
                }
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(Condition), "ForceStartEarRinging")]
        //public class Condition_EarRining
        //{
        //    public static bool Prefix(Condition __instance)
        //    {
        //        MelonLogger.Msg("[Condition] ForceStartEarRinging");
        //        if (MyMod.InOnline() == true)
        //        {
        //            MelonLogger.Msg("Killing annoying ear ring effect after fight with moose in multiplayer, cause it getting bugged when baseAI controller is off.");
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(PlayerStruggle), "ApplyBearDamageAfterStruggleEnds")]
        //public class PlayerStruggle_BearDamage
        //{
        //    public static bool Prefix(PlayerStruggle __instance)
        //    {
        //        if (MyMod.InOnline() == true)
        //        {
        //            if (MyMod.NeedDoBearDamage == true)
        //            {
        //                MyMod.NeedDoBearDamage = false;
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(PlayerStruggle), "ApplyMooseDamageAfterStruggleEnds")]
        //public class PlayerStruggle_MooseDamage
        //{
        //    public static bool Prefix(PlayerStruggle __instance)
        //    {
        //        if (MyMod.InOnline() == true)
        //        {
        //            if (MyMod.NeedDoMooseDamage == true)
        //            {
        //                MyMod.NeedDoMooseDamage = false;
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    }
        //}

        //[HarmonyLib.HarmonyPatch(typeof(PlayerStruggle), "Begin", new System.Type[] { typeof(GameObject) })]
        //public class PlayerStruggle_Begin
        //{
        //    public static void Prefix(PlayerStruggle __instance, GameObject partner)
        //    {
        //        MyMod.LastStruggleAnimalName = partner.name;
        //        MelonLogger.Msg("[PlayerStruggle] Begin struggle with " + MyMod.LastStruggleAnimalName);
        //        ////WILDLIFE_Moose
        //        if (MyMod.InOnline() == true)
        //        {
        //            if (MyMod.LastStruggleAnimalName.Contains("WILDLIFE_Bear") == true || MyMod.LastStruggleAnimalName.Contains("WILDLIFE_Moose") == true)
        //            {
        //                string tauntname = "";
        //                string punchline = "";
        //                if (MyMod.LastStruggleAnimalName.Contains("WILDLIFE_Bear") == true)
        //                {
        //                    tauntname = "bear";
        //                    punchline = "wide parody of a wolf";
        //                }
        //                else
        //                {
        //                    tauntname = "moose";
        //                    punchline = "step dancer";
        //                }
        //                MelonLogger.Msg("[PlayerStruggle] This is " + tauntname + "....fuck no, I so tired of fixing bugs with him, I just set NeverDie, cause I am so no care about this " + punchline);
        //                GameManager.GetConditionComponent().m_NeverDie = true;
        //            }
        //        }
        //    }
        //}
        [HarmonyLib.HarmonyPatch(typeof(UIButton), "OnClick")]
        public class UIButton_OnClick
        {
            public static void Postfix(UIButton __instance)
            {
                if (__instance.gameObject != null && __instance.gameObject.transform.parent != null && __instance.gameObject.transform.parent != null && __instance.hoverSprite == "genericButton_over 1")
                {
                    if(__instance.gameObject.transform.parent.name == "WaitForEveryoneButton")
                    {
                        if (MyMod.CanSleep(false) == false)
                        {
                            MyMod.CanSleep(true);
                        }else{
                            if (MyMod.SleepingButtons != null)
                            {
                                MyMod.SleepingButtons.SetActive(false);
                            }
                            if (MyMod.WaitForSleepLable != null)
                            {
                                MyMod.WaitForSleepLable.SetActive(true);
                            }
                        }
                    }else if (__instance.gameObject.transform.parent.name == "LieDownButton")
                    {
                        if(MyMod.AtBed == false)
                        {
                            MyMod.OutOfBedPosition = GameManager.GetPlayerTransform().position;
                            if (InterfaceManager.m_Panel_Rest.m_Bed)
                            {
                                GameManager.GetPlayerManagerComponent().TeleportPlayer(InterfaceManager.m_Panel_Rest.m_Bed.m_BodyPlacementTransform.position, InterfaceManager.m_Panel_Rest.m_Bed.m_BodyPlacementTransform.rotation);
                                MyMod.AtBed = true;
                            }
                        }else{
                            GameManager.GetPlayerManagerComponent().TeleportPlayer(MyMod.OutOfBedPosition, GameManager.GetPlayerTransform().rotation);
                            MyMod.AtBed = false;
                        }
                        InterfaceManager.m_Panel_Rest.OnCancel();
                    }
                }
            }
        }

        public static bool NeedSkipCauseConnect()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            for (int i = 0; i < arguments.Length; i++)
            {
                //MelonLogger.Msg("Argument ["+i+"] Parameter ["+arguments[i]+"]");

                if (arguments[i] == "join" || arguments[i] == "-join")
                {
                    if (i+1 <= arguments.Length-1)
                    {
                        MelonLogger.Msg("[STEAMWORKS.NET] Skip everything to connect to: " + arguments[i+1]);
                        return true;
                    }
                }
            }
            return false;
        }

        [HarmonyLib.HarmonyPatch(typeof(GameManager), "LoadScene", new Type[] { typeof(string), typeof(string) })]
        public class GameManager_LoadSceneOverLoad1
        {
            public static void Prefix()
            {                
                MelonLogger.Msg(ConsoleColor.Yellow,"Loading other scene...");
                MyMod.ApplyOtherCampfires = false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GameManager), "LoadScene", new Type[] { typeof(string)})]
        public class GameManager_LoadSceneOverLoad2
        {
            public static void Prefix()
            {
                MelonLogger.Msg(ConsoleColor.Yellow, "Loading other scene...");
                MyMod.ApplyOtherCampfires = false;
            }
        }

        public static bool OldPostLoadActionType = false;

        [HarmonyLib.HarmonyPatch(typeof(SceneManager), "OnSceneLoaded")]
        public class SceneManager_Load
        {
            public static void Postfix()
            {
                MelonLogger.Msg(ConsoleColor.Yellow, "[SceneManager] OnSceneLoaded");
                if (MyMod.level_name == "Boot")
                {
                    if (uConsole.m_Instance == null)
                    {
                        MelonLogger.Msg("No uConsole present, creating one.");
                        UnityEngine.Object.Instantiate(Resources.Load("uConsole"));
                    }

                    string v_type = "";
                    if (GameManager.GetVersionString().Contains("S"))
                    {
                        v_type = "Steam";
                    }
                    else if (GameManager.GetVersionString().Contains("E"))
                    {
                        v_type = "EGS";
                        MyMod.LoadChatName();
                    }
                    else
                    {
                        v_type = "Unknown";
                        MyMod.LoadChatName();
                    }
                    if (v_type != "Unknown")
                    {
                        MelonLogger.Msg(v_type + " version");
                    }
                    else
                    {
                        MelonLogger.Msg("Unknown build of game, not using SteamWorks.NET");
                    }
                    if (v_type == "Steam")
                    {
                        MelonLogger.Msg("[SteamWorks.NET] Loading...");
                        bool ok = SteamConnect.StartSteam();

                        if(ok == false)
                        {
                            return;
                        }

                        string[] arguments = Environment.GetCommandLineArgs();
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            //MelonLogger.Msg("Argument ["+i+"] Parameter ["+arguments[i]+"]");

                            if(arguments[i] == "join" || arguments[i] == "-join")
                            {
                                if(i+1 <= arguments.Length-1)
                                {
                                    MyMod.SteamServerWorks = arguments[i + 1];
                                    MyMod.ConnectedSteamWorks = true;
                                    MyMod.NeedConnectAfterLoad = 3;
                                    break;
                                }
                            }
                        }


                        //if (arguments.Count() > 2)
                        //{
                        //    int PreLast = arguments.Count() - 2;
                        //    int Last = arguments.Count() - 1;
                        //    if(arguments[PreLast] == "join")
                        //    {
                        //        MelonLogger.Msg("[STEAMWORKS.NET] Connect from startup to: "+ arguments[Last]);
                        //        MyMod.ConnectedSteamWorks = true;
                        //        MyMod.SteamServerWorks = arguments[Last];
                        //        MyMod.NeedConnectAfterLoad = 3;
                        //    }
                        //}
                    }

                    if (Application.isBatchMode)
                    {
                        MyMod.StartDSAfterLoad = 3;
                    }
                    
                    MyMod.FirstBoot = false;
                }
                if (MyMod.level_name != "Empty" && MyMod.level_name != "Boot" && MyMod.level_name != "MainMenu")
                {
                    MelonLogger.Msg("Loading scene finished " + MyMod.level_name + " health is: " + GameManager.GetConditionComponent().m_CurrentHP);
                    if (MyMod.KillAfterLoad == true && GameManager.GetConditionComponent().m_CurrentHP > 0)
                    {
                        MelonLogger.Msg("Should dead but has " + GameManager.GetConditionComponent().m_CurrentHP + " health ");
                        MyMod.IsDead = true;
                        GameManager.GetConditionComponent().m_CurrentHP = 0.0f;
                        MyMod.SetRevivedStats(false);
                        MelonLogger.Msg("Has set it to zero, now health is " + GameManager.GetConditionComponent().m_CurrentHP + " health ");
                    }

                    MyMod.NotNeedToPauseUntilLoaded = false;

                    if (OldPostLoadActionType == true)
                    {
                        MyMod.SendSpawnData();
                        MelonLogger.Msg(ConsoleColor.Blue, "Gonna UpdateRopesAndFurnsAndCampfires in 2 seconds!");
                        MyMod.UpdateRopesAndFurns = 2;
                        MelonLogger.Msg(ConsoleColor.Blue, "Gonna UpdateLootedContainers in 3 seconds!");
                        MyMod.UpdateLootedContainers = 3;
                        MelonLogger.Msg(ConsoleColor.Blue, "Gonna UpdatePickedPlants in 3 seconds!");
                        MyMod.UpdatePickedPlants = 3;
                        MelonLogger.Msg(ConsoleColor.Blue, "Gonna UpdateSnowshelters in 3 seconds!");
                        MyMod.UpdateSnowshelters = 3;
                        MelonLogger.Msg(ConsoleColor.Blue, "Gonna UpdatePickedGears in 3 seconds!");
                        MyMod.UpdatePickedGears = 3;
                        MelonLogger.Msg(ConsoleColor.Blue, "Gonna UpdateCampfires in 4 seconds!");
                        MyMod.UpdateCampfires = 4;
                    }
                }
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(BloodLoss), "ApplyBandageToLocation")]
        //private static class BloodLoss_Stop
        //{
        //    internal static void Postfix(BloodLoss __instance, AfflictionBodyArea area)
        //    {
        //        MyMod.UsedBandage(area);
        //    }
        //}
        [HarmonyLib.HarmonyPatch(typeof(BreakDown), "DoBreakDown")]
        private static class DoBreakDown_Sync
        {
            internal static void Postfix(BreakDown __instance)
            {
                string breakGuid = "";
                string breakParentGuid = "";
                if(__instance.gameObject != null)
                {
                    ObjectGuid BreakGuidComp = __instance.gameObject.GetComponent<ObjectGuid>();
                    if(BreakGuidComp != null)
                    {
                        breakGuid = BreakGuidComp.Get();
                    }
                    if(__instance.gameObject.transform.parent != null)
                    {
                        ObjectGuid BreakGuidParentComp = __instance.gameObject.transform.parent.GetComponent<ObjectGuid>();
                        if(BreakGuidParentComp != null)
                        {
                            breakParentGuid = BreakGuidParentComp.Get();
                        }
                    }
                }

                if(breakGuid == "" && breakParentGuid == "")
                {
                    MelonLogger.Msg("Deny listing that furn, cause have not any GUIDs, so it should be duppable");
                    return;
                }

                MyMod.OnFurnitureDestroyed(breakGuid, breakParentGuid, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent, true);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(LoadScene), "Start")]
        private static class LoadScene_SeededGUIDHook
        {
            internal static bool Prefix(LoadScene __instance)
            {
                //MelonLogger.Msg("[LoadScene] SceneCanBeInstanced " + __instance.m_SceneCanBeInstanced + " ExitPoint " + __instance.m_ExitPointName + " SceneToLoad " + __instance.m_SceneToLoad);
                if (!__instance.m_Active || __instance.m_StartHasBeenCalled)
                    return false;
                __instance.m_StartHasBeenCalled = true;
                __instance.m_FadeOutStarted = false;
                if (__instance.m_LoadSceneParent != null)
                {
                    __instance.m_LoadSceneParent.Start();
                    __instance.m_GUID = __instance.m_LoadSceneParent.m_GUID;
                }
                else
                {
                    //MelonLogger.Msg("[LoadScene] Have not LoadSceneParent generating custom seeded GUID");
                    __instance.m_GUID = MyMod.GenerateSeededGUID(GameManager.m_SceneTransitionData.m_GameRandomSeed, __instance.gameObject.transform.position);

                    //MelonLogger.Msg("[LoadScene] Got new m_GUID " + __instance.m_GUID);
                    //__instance.gameObject.name = "InteriorLoadTrigger" + __instance.m_GUID;
                }
                if (__instance.m_TransitionOnContact)
                {
                    __instance.GetComponent<Collider>().isTrigger = true;
                    vp_Layer.Set(__instance.gameObject, 21);
                }
                else if (__instance.gameObject.layer != 21)
                {
                    vp_Layer.Set(__instance.gameObject, 19);
                }
                if (__instance.m_LoadSceneParent != null)
                {
                    __instance.m_LoadSceneParent.Register(__instance);
                }
                __instance.m_Lock = __instance.gameObject.GetComponent<Lock>();
                if (!__instance.m_Lock)
                    return false;
                __instance.m_Lock.RollLockedState();
                return false;
            }
        }

        //[HarmonyLib.HarmonyPatch(typeof(LoadScene), "Start")]
        //private static class LoadScene_SeededGUIDHook
        //{
        //    internal static void Postfix(LoadScene __instance)
        //    {
        //        MelonLogger.Msg("[LoadScene] SceneCanBeInstanced " + __instance.m_SceneCanBeInstanced + " ExitPoint " + __instance.m_ExitPointName + " SceneToLoad " + __instance.m_SceneToLoad + " m_GUID "+__instance.m_GUID);
        //    }
        //}

        [HarmonyLib.HarmonyPatch(typeof(LoadSceneParent), "Start")]
        private static class LoadSceneParent_SeededGUIDHook
        {
            internal static bool Prefix(LoadSceneParent __instance)
            {
                //MelonLogger.Msg("[LoadSceneParent] Start");
                if (__instance.m_StartHasBeenCalled)
                    return false;
                __instance.m_StartHasBeenCalled = true;
                //MelonLogger.Msg("[LoadSceneParent] Generating custom seeded GUID");
                //__instance.m_GUID = Utils.GetGuid();
                __instance.m_GUID = MyMod.GenerateSeededGUID(GameManager.m_SceneTransitionData.m_GameRandomSeed, __instance.gameObject.transform.position);
                //MelonLogger.Msg("[LoadSceneParent] Got new m_GUID " + __instance.m_GUID);
                return false;
            }
        }

        public static void SaveBrokenFurtiture(SaveSlotType gameMode, string name)
        {
            //MelonLogger.Msg("[Saving][BrokenFurnitureSync] Saving...");
            MyMod.BrokenFurnitureSync[] saveProxy = MyMod.BrokenFurniture.ToArray();
            string data = JSON.Dump(saveProxy);
            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_furns", data);
            if (ok == true)
            {
                //MelonLogger.Msg("[Saving][BrokenFurnitureSync] Successfully!");
            }else{
                //MelonLogger.Msg("[Saving][BrokenFurnitureSync] Fail!");
            }
        }
        public static void SavePickedGears(SaveSlotType gameMode, string name)
        {
            //MelonLogger.Msg("[Saving][PickedGearSync] Saving...");
            MyMod.PickedGearSync[] saveProxy = MyMod.PickedGears.ToArray();
            string data = JSON.Dump(saveProxy);
            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_pickedgears", data);
            if (ok == true)
            {
                //MelonLogger.Msg("[Saving][PickedGearSync] Successfully!");
            }else{
                //MelonLogger.Msg("[Saving][PickedGearSync] Fail!");
            }
        }

        public static void SaveDeployedRopes(SaveSlotType gameMode, string name)
        {
            //MelonLogger.Msg("[Saving][ClimbingRopeSync] Saving...");
            MyMod.ClimbingRopeSync[] saveProxy = MyMod.DeployedRopes.ToArray();
            string data = JSON.Dump(saveProxy);
            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_ropes", data);
            if (ok == true)
            {
                //MelonLogger.Msg("[Saving][ClimbingRopeSync] Successfully!");
            }else{
                //MelonLogger.Msg("[Saving][ClimbingRopeSync] Fail!");
            }
        }

        public static void SaveLootedBoxes(SaveSlotType gameMode, string name)
        {
            //MelonLogger.Msg("[Saving][LootedContainers] Saving...");
            MyMod.ContainerOpenSync[] saveProxy = MyMod.LootedContainers.ToArray();
            string data = JSON.Dump(saveProxy);
            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_containers", data);
            if (ok == true)
            {
                //MelonLogger.Msg("[Saving][LootedContainers] Successfully!");
            }else{
                //MelonLogger.Msg("[Saving][LootedContainers] Fail!");
            }
        }

        public static void SaveServerConfig(SaveSlotType gameMode, string name)
        {
            //MelonLogger.Msg("[Saving][ServerConfig] Saving...");
            if (MyMod.sendMyPosition == true)
            {
                //MelonLogger.Msg("[Saving][ServerConfig] You on server being client, not need save this.");
                return;
            }
            string data = JSON.Dump(MyMod.ServerConfig);
            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_cfg", data);
            if (ok == true)
            {
                //MelonLogger.Msg("[Saving][ServerConfig] Successfully!");
            }else{
                //MelonLogger.Msg("[Saving][ServerConfig] Fail!");
            }
        }

        public static void SavePlants(SaveSlotType gameMode, string name)
        {
            //MelonLogger.Msg("[Saving][HarvestableSyncData] Saving...");
            string[] saveProxy = MyMod.HarvestedPlants.ToArray();
            string data = JSON.Dump(saveProxy);
            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_plants", data);
            if (ok == true)
            {
                //MelonLogger.Msg("[Saving][HarvestableSyncData] Successfully!");
            }else{
                //MelonLogger.Msg("[Saving][HarvestableSyncData] Fail!");
            }
        }

        public static void SaveSnowShelters(SaveSlotType gameMode, string name)
        {
            //MelonLogger.Msg("[Saving][SnowShelters] Saving...");
            MyMod.ShowShelterByOther[] saveProxy = MyMod.ShowSheltersBuilded.ToArray();
            string data = JSON.Dump(saveProxy);
            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_shelters", data);
            if (ok == true)
            {
                //MelonLogger.Msg("[Saving][SnowShelters] Successfully!");
            }else{
                //MelonLogger.Msg("[Saving][SnowShelters] Fail!");
            }
        }
        public static void SaveMPDeath(SaveSlotType gameMode, string name)
        {
            //MelonLogger.Msg("[Saving][MultiplayerDeath] Saving...");
            bool[] saveProxy = { MyMod.IsDead };
            string data = JSON.Dump(saveProxy);

            bool ok = SaveGameSlots.SaveDataToSlot(gameMode, SaveGameSystem.m_CurrentEpisode, SaveGameSystem.m_CurrentGameId, name, "skycoop_death", data);
            if (ok == true)
            {
                //MelonLogger.Msg("[Saving][MultiplayerDeath] Successfully!");
            }else{
                //MelonLogger.Msg("[Saving][MultiplayerDeath] Fail!");
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SaveGameSystem), "SaveGlobalData")]
        public static class SaveGameSystemPatch_SaveSceneData
        {
            public static void Postfix(SaveSlotType gameMode, string name)
            {
                SaveServerConfig(gameMode, name);
                SaveBrokenFurtiture(gameMode, name);
                SavePickedGears(gameMode, name);
                SaveDeployedRopes(gameMode, name);
                SaveLootedBoxes(gameMode, name);
                SavePlants(gameMode, name);
                SaveSnowShelters(gameMode, name);
                SaveMPDeath(gameMode, name);
            }
        }

        public static void LoadBrokenFurtiture(string name)
        {
            //MelonLogger.Msg("[Saving][BrokenFurnitureSync] Loading...");
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_furns");
            if (data != null)
            {
                MyMod.BrokenFurnitureSync[] saveProxy = JSON.Load(data).Make<MyMod.BrokenFurnitureSync[]>();
                List<MyMod.BrokenFurnitureSync> loadedData = saveProxy.ToList<MyMod.BrokenFurnitureSync>();

                for (int i = 0; i < loadedData.Count; i++)
                {
                    MyMod.BrokenFurnitureSync ToAdd = loadedData[i];
                    if (MyMod.BrokenFurniture.Contains(ToAdd) == false)
                    {
                        MyMod.BrokenFurniture.Add(ToAdd);
                    }
                }
                //MelonLogger.Msg("[Saving][BrokenFurnitureSync] Loaded Entries: " + loadedData.Count);
                //MelonLogger.Msg("[Saving][BrokenFurnitureSync] Total Entries: " + MyMod.BrokenFurniture.Count);
            }else{
                //MelonLogger.Msg("[Saving][BrokenFurnitureSync] No saves found!");
            }
        }

        public static void LoadPickedGears(string name)
        {
            //MelonLogger.Msg("[Saving][PickedGearSync] Loading...");
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_pickedgears");
            if (data != null)
            {
                MyMod.PickedGearSync[] saveProxy = JSON.Load(data).Make<MyMod.PickedGearSync[]>();
                List<MyMod.PickedGearSync> loadedData = saveProxy.ToList<MyMod.PickedGearSync>();

                for (int i = 0; i < loadedData.Count; i++)
                {
                    MyMod.PickedGearSync ToAdd = loadedData[i];
                    if (MyMod.PickedGears.Contains(ToAdd) == false)
                    {
                        MyMod.PickedGears.Add(ToAdd);
                    }
                }
                //MelonLogger.Msg("[Saving][PickedGearSync] Loaded Entries: " + loadedData.Count);
                //MelonLogger.Msg("[Saving][PickedGearSync] Total Entries: " + MyMod.PickedGears.Count);
            }else{
                //MelonLogger.Msg("[Saving][PickedGearSync] No saves found!");
            }
        }

        public static void LoadDeployedRopes(string name)
        {
            //MelonLogger.Msg("[Saving][ClimbingRopeSync] Loading...");
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_ropes");
            if (data != null)
            {
                MyMod.ClimbingRopeSync[] saveProxy = JSON.Load(data).Make<MyMod.ClimbingRopeSync[]>();
                List<MyMod.ClimbingRopeSync> loadedData = saveProxy.ToList<MyMod.ClimbingRopeSync>();

                for (int i = 0; i < loadedData.Count; i++)
                {
                    MyMod.ClimbingRopeSync ToAdd = loadedData[i];
                    if (MyMod.DeployedRopes.Contains(ToAdd) == false)
                    {
                        MyMod.DeployedRopes.Add(ToAdd);
                    }
                }
                //MelonLogger.Msg("[Saving][ClimbingRopeSync] Loaded Entries: " + loadedData.Count);
                //MelonLogger.Msg("[Saving][ClimbingRopeSync] Total Entries: " + MyMod.DeployedRopes.Count);
            }else{
                //MelonLogger.Msg("[Saving][ClimbingRopeSync] No saves found!");
            }
        }

        public static void LoadLootedBoxes(string name)
        {
            //MelonLogger.Msg("[Saving][LootedContainers] Loading...");
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_containers");
            if (data != null)
            {
                MyMod.ContainerOpenSync[] saveProxy = JSON.Load(data).Make<MyMod.ContainerOpenSync[]>();
                List<MyMod.ContainerOpenSync> loadedData = saveProxy.ToList<MyMod.ContainerOpenSync>();

                for (int i = 0; i < loadedData.Count; i++)
                {
                    MyMod.ContainerOpenSync ToAdd = loadedData[i];
                    if (MyMod.LootedContainers.Contains(ToAdd) == false)
                    {
                        MyMod.LootedContainers.Add(ToAdd);
                    }
                }
                //MelonLogger.Msg("[Saving][LootedContainers] Loaded Entries: " + loadedData.Count);
                //MelonLogger.Msg("[Saving][LootedContainers] Total Entries: " + MyMod.LootedContainers.Count);
            }else{
                //MelonLogger.Msg("[Saving][LootedContainers] No saves found!");
            }
        }

        public static void LoadPlants(string name)
        {
            //MelonLogger.Msg("[Saving][HarvestableSyncData] Loading...");
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_plants");
            if (data != null)
            {
                string[] saveProxy = JSON.Load(data).Make<string[]>();
                List<string> loadedData = saveProxy.ToList<string>();

                for (int i = 0; i < loadedData.Count; i++)
                {
                    string ToAdd = loadedData[i];
                    if (MyMod.HarvestedPlants.Contains(ToAdd) == false)
                    {
                        MyMod.HarvestedPlants.Add(ToAdd);
                    }
                }
                //MelonLogger.Msg("[Saving][HarvestableSyncData] Loaded Entries: " + loadedData.Count);
                //MelonLogger.Msg("[Saving][HarvestableSyncData] Total Entries: " + MyMod.HarvestedPlants.Count);
            }else{
                //MelonLogger.Msg("[Saving][HarvestableSyncData] No saves found!");
            }
        }

        public static void LoadSnowShelters(string name)
        {
            //MelonLogger.Msg("[Saving][SnowShelters] Loading...");
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_shelters");
            if (data != null)
            {
                MyMod.ShowShelterByOther[] saveProxy = JSON.Load(data).Make<MyMod.ShowShelterByOther[]>();
                List<MyMod.ShowShelterByOther> loadedData = saveProxy.ToList<MyMod.ShowShelterByOther>();

                for (int i = 0; i < loadedData.Count; i++)
                {
                    MyMod.ShowShelterByOther ToAdd = loadedData[i];
                    if (MyMod.ShowSheltersBuilded.Contains(ToAdd) == false)
                    {
                        MyMod.ShowSheltersBuilded.Add(ToAdd);
                    }
                }
                //MelonLogger.Msg("[Saving][SnowShelters] Loaded Entries: " + loadedData.Count);
                //MelonLogger.Msg("[Saving][SnowShelters] Total Entries: " + MyMod.ShowSheltersBuilded.Count);
            }else{
                //MelonLogger.Msg("[Saving][SnowShelters] No saves found!");
            }
        }

        public static void LoadServerConfig(string name)
        {
            //MelonLogger.Msg("[Saving][ServerConfig] Loading...");

            if(MyMod.sendMyPosition == true)
            {
                //MelonLogger.Msg("[Saving][ServerConfig] You on server being client, not need load this.");
                return;
            }

            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_cfg");
            if (data != null)
            {
                MyMod.ServerConfigData saveProxy = JSON.Load(data).Make<MyMod.ServerConfigData>();
                MyMod.ServerConfig = saveProxy;
                //MelonLogger.Msg("[Saving][ServerConfig] m_FastConsumption: " + saveProxy.m_FastConsumption);
                //MelonLogger.Msg("[Saving][ServerConfig] m_DuppedSpawns: " + saveProxy.m_DuppedSpawns);
            }else{
                //MelonLogger.Msg("[Saving][ServerConfig] No saves found!");
            }
        }
        public static void LoadMPDeath(string name)
        {
            //MelonLogger.Msg("[Saving][MultiplayerDeath] Loading...");
            string data = SaveGameSlots.LoadDataFromSlot(name, "skycoop_death");
            if (data != null)
            {
                bool[] saveProxy = JSON.Load(data).Make<bool[]>();
                //MelonLogger.Msg("[Saving][MultiplayerDeath] IsDead: " + saveProxy[0]);
                MyMod.IsDead = saveProxy[0];
            }else{
                //MelonLogger.Msg("[Saving][MultiplayerDeath] No saves found!");
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SaveGameSystem), "RestoreGlobalData")]
        public static class SaveGameSystemPatch_RestoreGlobalData
        {
            public static void Postfix(string name)
            {
                MelonLogger.Msg(ConsoleColor.Yellow,"[Saving] Loading "+name+"...");
                LoadServerConfig(name);
                LoadBrokenFurtiture(name);
                LoadPickedGears(name);
                LoadDeployedRopes(name);
                LoadLootedBoxes(name);
                LoadPlants(name);
                LoadSnowShelters(name);
                LoadMPDeath(name);
            }
        }
        public static void FlushAllSavable()
        {
            MelonLogger.Msg("[Saving] Wipe all savables cause of quit");
            MyMod.ServerConfig = new MyMod.ServerConfigData();
            MyMod.BrokenFurniture = new List<MyMod.BrokenFurnitureSync>();
            MyMod.PickedGears = new List<MyMod.PickedGearSync>();
            MyMod.DeployedRopes = new List<MyMod.ClimbingRopeSync>();
            if(MyMod.sendMyPosition == true)
            {
                MelonLogger.Msg("[CLIENT] Disconnect cause quit game");
                MyMod.Disconnect();
            }
        }



        [HarmonyLib.HarmonyPatch(typeof(GameManager), "LoadMainMenu")]
        public static class GameManager_BackToMenu
        {
            public static void Prefix()
            {
                FlushAllSavable();
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_BreakDown), "OnBreakDown")]
        public static class Panel_BreakDown_AudioStart
        {
            public static void Postfix(Panel_BreakDown __instance)
            {
                if(__instance.m_BreakDown != null)
                {
                    GameObject gameObject = __instance.m_BreakDown.gameObject;
                    string breakGuid = "";
                    string breakParentGuid = "";
                    if (gameObject != null)
                    {
                        ObjectGuid BreakGuidComp = gameObject.GetComponent<ObjectGuid>();
                        if (BreakGuidComp != null)
                        {
                            breakGuid = BreakGuidComp.Get();
                        }
                        if (gameObject.transform.parent != null)
                        {
                            ObjectGuid BreakGuidParentComp = gameObject.transform.parent.GetComponent<ObjectGuid>();
                            if (BreakGuidParentComp != null)
                            {
                                breakParentGuid = BreakGuidParentComp.Get();
                            }
                        }
                    }

                    MyMod.BrokenFurnitureSync furn = new MyMod.BrokenFurnitureSync();
                    furn.m_Guid = breakGuid;
                    furn.m_ParentGuid = breakParentGuid;
                    furn.m_LevelID = MyMod.levelid;
                    furn.m_LevelGUID = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;

                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.FURNBREAKINGGUID))
                        {
                            _packet.Write(furn);
                            SendTCPData(_packet);
                        }
                    }

                    if (MyMod.iAmHost == true)
                    {
                        ServerSend.FURNBREAKINGGUID(0, furn, true);
                    }

                    MelonLogger.Msg("Starting braking object " + furn.m_Guid + " audio");
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_BreakDown), "StopAudioAndRumbleEffects")]
        public static class Panel_BreakDown_AudioStop
        {
            public static void Postfix(Panel_BreakDown __instance)
            {
                if (MyMod.sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.FURNBREAKINSTOP))
                    {
                        _packet.Write(true);
                        SendTCPData(_packet);
                    }
                }

                if (MyMod.iAmHost == true)
                {
                    ServerSend.FURNBREAKINSTOP(0, true, true);
                }

                MelonLogger.Msg("Stop braking object");
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_BreakDown), "Update")]
        public static class Panel_BreakDown_KickOutFromMenu
        {
            public static void Postfix(Panel_BreakDown __instance)
            {            
                if(__instance.m_BreakDown != null)
                {
                    GameObject gameObject = __instance.m_BreakDown.gameObject;
                    string breakGuid = "";
                    string breakParentGuid = "";
                    if (gameObject != null)
                    {
                        ObjectGuid BreakGuidComp = gameObject.GetComponent<ObjectGuid>();
                        if (BreakGuidComp != null)
                        {
                            breakGuid = BreakGuidComp.Get();
                        }
                        if (gameObject.transform.parent != null)
                        {
                            ObjectGuid BreakGuidParentComp = gameObject.transform.parent.GetComponent<ObjectGuid>();
                            if (BreakGuidParentComp != null)
                            {
                                breakParentGuid = BreakGuidParentComp.Get();
                            }
                        }
                    }

                    MyMod.BrokenFurnitureSync furn = new MyMod.BrokenFurnitureSync();
                    furn.m_Guid = breakGuid;
                    furn.m_ParentGuid = breakParentGuid;
                    furn.m_LevelID = MyMod.levelid;
                    furn.m_LevelGUID = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;

                    if (__instance.m_BreakDown.gameObject.activeSelf == false)
                    {
                        //HUDMessage.AddMessage("THIS ALREADY BROKEN");
                        //GameAudioManager.PlayGUIError();
                        if (__instance.m_IsBreakingDown == true)
                        {
                            __instance.m_IsBreakingDown = false;
                            __instance.StopAudioAndRumbleEffects();

                        }
                        __instance.OnCancel();
                    }else{
                        for (int i = 0; i < MyMod.playersData.Count; i++)
                        {
                            if (MyMod.playersData[i] != null)
                            {
                                MyMod.BrokenFurnitureSync otherFurn = MyMod.playersData[i].m_BrakingObject;
                                if (otherFurn.m_Guid == furn.m_Guid && otherFurn.m_ParentGuid == furn.m_ParentGuid && otherFurn.m_LevelID == MyMod.levelid && otherFurn.m_LevelGUID == MyMod.level_guid)
                                {
                                    HUDMessage.AddMessage(MyMod.playersData[i].m_Name + " IS BREAKING THIS");
                                    GameAudioManager.PlayGUIError();
                                    if (__instance.m_IsBreakingDown == true)
                                    {
                                        __instance.m_IsBreakingDown = false;
                                        __instance.StopAudioAndRumbleEffects();

                                    }
                                    __instance.OnCancel();
                                }
                            }
                        }
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(BreakDown), "ProcessInteraction")]
        public static class BreakDown_ProcessInteraction
        {
            public static bool Prefix(BreakDown __instance)
            {
                GameObject gameObject = __instance.gameObject;
                string breakGuid = "";
                string breakParentGuid = "";
                if (gameObject != null)
                {
                    ObjectGuid BreakGuidComp = gameObject.GetComponent<ObjectGuid>();
                    if (BreakGuidComp != null)
                    {
                        breakGuid = BreakGuidComp.Get();
                    }
                    if (gameObject.transform.parent != null)
                    {
                        ObjectGuid BreakGuidParentComp = gameObject.transform.parent.GetComponent<ObjectGuid>();
                        if (BreakGuidParentComp != null)
                        {
                            breakParentGuid = BreakGuidParentComp.Get();
                        }
                    }
                }

                MyMod.BrokenFurnitureSync furn = new MyMod.BrokenFurnitureSync();
                furn.m_Guid = breakGuid;
                furn.m_ParentGuid = breakParentGuid;
                furn.m_LevelID = MyMod.levelid;
                furn.m_LevelGUID = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;

                for (int i = 0; i < MyMod.playersData.Count; i++)
                {
                    if (MyMod.playersData[i] != null)
                    {
                        MyMod.BrokenFurnitureSync otherFurn = MyMod.playersData[i].m_BrakingObject;
                        if (otherFurn.m_Guid == furn.m_Guid && otherFurn.m_ParentGuid == furn.m_ParentGuid && otherFurn.m_LevelID == MyMod.levelid && otherFurn.m_LevelGUID == MyMod.level_guid)
                        {
                            HUDMessage.AddMessage(MyMod.playersData[i].m_Name + " IS BREAKING THIS");
                            GameAudioManager.PlayGUIError();
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "ProcessContainerInteraction")]
        public static class PlayerManager_ProcessContainerInteraction
        {
            public static bool Prefix(PlayerManager __instance, Container c)
            {
                string boxGUID = "";

                if (c != null && c.gameObject != null && c.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    boxGUID = c.gameObject.GetComponent<ObjectGuid>().Get();
                }

                MyMod.ContainerOpenSync box = new MyMod.ContainerOpenSync();
                box.m_Guid = boxGUID;
                box.m_LevelID = MyMod.levelid;
                box.m_LevelGUID = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;

                for (int i = 0; i < MyMod.playersData.Count; i++)
                {
                    if (MyMod.playersData[i] != null)
                    {
                        if (MyMod.playersData[i].m_Container != null)
                        {
                            MyMod.ContainerOpenSync otherBox = MyMod.playersData[i].m_Container;
                            if (otherBox.m_Guid == box.m_Guid && otherBox.m_LevelID == MyMod.levelid && otherBox.m_LevelGUID == MyMod.level_guid)
                            {
                                HUDMessage.AddMessage(MyMod.playersData[i].m_Name + " IS USING THIS");
                                GameAudioManager.PlayGUIError();
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "Update")]
        public static class PlayerManager_Update
        {
            public static void Postfix(PlayerManager __instance)
            {
                if(__instance.m_ContainerBeingSearched != null)
                {
                    Container c = __instance.m_ContainerBeingSearched;
                    string boxGUID = "";

                    if (c != null && c.gameObject != null && c.gameObject.GetComponent<ObjectGuid>() != null)
                    {
                        boxGUID = c.gameObject.GetComponent<ObjectGuid>().Get();
                    }

                    MyMod.ContainerOpenSync box = new MyMod.ContainerOpenSync();
                    box.m_Guid = boxGUID;
                    box.m_LevelID = MyMod.levelid;
                    box.m_LevelGUID = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;

                    for (int i = 0; i < MyMod.playersData.Count; i++)
                    {
                        if (MyMod.playersData[i] != null)
                        {
                            if (MyMod.playersData[i].m_Container != null)
                            {
                                MyMod.ContainerOpenSync otherBox = MyMod.playersData[i].m_Container;
                                if (otherBox.m_Guid == box.m_Guid && otherBox.m_LevelID == MyMod.levelid && otherBox.m_LevelGUID == MyMod.level_guid)
                                {
                                    __instance.m_ContainerBeingSearched.CancelSearch();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(RopeAnchorPoint), "Start")]
        public static class RopeAnchorPoint_Hook
        {
            public static void Postfix(RopeAnchorPoint __instance)
            {
                if (MyMod.IsDead == true || GameManager.GetPlayerStruggleComponent().InStruggle() == true)
                {
                    return;
                }
                if (__instance.m_StartHasBeenCalled)
                    return;
                MelonLogger.Msg("Rope Start");
                MyMod.AddDeployedRopes(__instance.gameObject.transform.position, __instance.m_RopeDeployed, __instance.m_RopeSnapped, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent, true);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(RopeAnchorPoint), "SnapRope")]
        public static class RopeAnchorPoint_Snapped
        {
            public static void Postfix(RopeAnchorPoint __instance)
            {
                if (MyMod.IsDead == true || GameManager.GetPlayerStruggleComponent().InStruggle() == true)
                {
                    return;
                }
                MelonLogger.Msg("Rope Snapped");
                MyMod.AddDeployedRopes(__instance.gameObject.transform.position, __instance.m_RopeDeployed, __instance.m_RopeSnapped, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent, true);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(RopeAnchorPoint), "ActionFinished")]
        public static class RopeAnchorPoint_TakeOrDeploy
        {
            public static void Prefix(RopeAnchorPoint __instance)
            {
                if (MyMod.IsDead == true || GameManager.GetPlayerStruggleComponent().InStruggle() == true)
                {
                    return;
                }
                if(__instance.m_ActionState == RopeAnchorActionState.None)
                {
                    return;
                }
                if (GameManager.m_PlayerManager != null)
                {
                    PlayerControlMode Plc = GameManager.GetPlayerManagerComponent().GetControlMode();
                    if(Plc == PlayerControlMode.DeployRope || Plc == PlayerControlMode.TakeRope)
                    {
                        bool deployed = false;
                        if (Plc == PlayerControlMode.DeployRope)
                        {
                            MelonLogger.Msg("Rope Deployed");
                            deployed = true;
                        }else{
                            MelonLogger.Msg("Rope Taken");
                            deployed = false;
                        }
                        MyMod.AddDeployedRopes(__instance.gameObject.transform.position, deployed, __instance.m_RopeSnapped, MyMod.levelid, GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent, true);
                    }
                }
            }
        }

        //MAKING RANDOM GEARS SEEDED!


        [HarmonyLib.HarmonyPatch(typeof(GameManager), "RollSpawnChance", new System.Type[] { typeof(GameObject), typeof(float) })]
        public static class GameManager_SeededRandom
        {
            public static bool Prefix(GameObject go, float spawnChance)
            {
                return false;
            }
            public static void Postfix(GameObject go, float spawnChance, ref bool __result)
            {
                int _x = (int)go.transform.position.x;
                int _y = (int)go.transform.position.y;
                int _z = (int)go.transform.position.z;

                int seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + _x + _y + _z;
                spawnChance = Mathf.Clamp(spawnChance, 0.0f, 100f);

                System.Random RNG = new System.Random(seed);

                int num = MyMod.RollChanceSeeded(spawnChance, RNG) ? 1 : 0;
                if (num == 0)
                    go.SetActive(false);
                __result = num != 0;
                //MelonLogger.Msg("[RollSpawnChance Seeded] Gear " + go.name + " Chance " + spawnChance + "% " + " Success " + __result + " Position: X " + _x + " Y " + _y + " Z " + _z);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(GameManager), "GetRandomSeed")]
        public static class GameManager_GetRandomSeed
        {
            public static bool Prefix(int seed)
            {
                return false;
            }
            public static void Postfix(int seed, ref int __result)
            {
                __result = seed ^ GameManager.m_SceneTransitionData.m_GameRandomSeed;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(RandomSpawnObject), "ActivateRandomObject")]
        public static class RandomSpawnObject_SeededFix
        {
            public static bool Prefix(RandomSpawnObject __instance)
            {
                return false;
            }
            public static void Postfix(RandomSpawnObject __instance)
            {
                //MelonLogger.Msg("[RandomSpawnObject] ActivateRandomObject started for "+__instance.gameObject.name+" Position X " + __instance.gameObject.transform.position.x + "Y " + __instance.gameObject.transform.position.y + " Z " + __instance.gameObject.transform.position.z);
                List<GameObject> gameObjectList = new List<GameObject>((IEnumerable<GameObject>)__instance.m_ObjectList);
                List<int> intList = new List<int>((IEnumerable<int>)__instance.m_Weights);
                float hoursPlayedNotPaused = GameManager.GetTimeOfDayComponent().GetHoursPlayedNotPaused();
                UnityEngine.Random.State state = UnityEngine.Random.state;

                UnityEngine.Random.InitState(GameManager.GetRandomSeed(__instance.transform.position.GetHashCode()));
                int enableCurrentXpMode = __instance.GetNumObjectsToEnableCurrentXPMode();
                for (int index1 = 0; index1 < enableCurrentXpMode; ++index1)
                {
                    int num1 = 0;
                    foreach (int num2 in intList)
                    {
                        num1 += num2;
                    }
                    if (num1 == 0)
                    {
                        UnityEngine.Random.state = state;
                        return;
                    }
                    List<MapDetail> mapDetailList = new List<MapDetail>();
                    using (List<GameObject>.Enumerator enumerator = gameObjectList.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            GameObject current = enumerator.Current;
                            if (current)
                            {
                                MapDetail[] componentsInChildren = (MapDetail[])current.GetComponentsInChildren<MapDetail>();
                                mapDetailList.AddRange((IEnumerable<MapDetail>)componentsInChildren);
                            }
                        }
                    }
                    int num3 = UnityEngine.Random.Range(0, num1);
                    int num4 = 0;
                    int index2 = -1;
                    for (int index3 = 0; index3 < intList.Count; ++index3)
                    {
                        int num2 = num4 + intList[index3];
                        if (num3 >= num4 && num3 < num2)
                        {
                            if (gameObjectList[index3])
                            {
                                //MelonLogger.Msg("[RandomSpawnObject] Spawns " + gameObjectList[index3].name+" on "+ gameObjectList[index3].transform.position.x+" " + gameObjectList[index3].transform.position.y + " " + gameObjectList[index3].transform.position.z);
                                gameObjectList[index3].SetActive(true);
                                foreach (MapDetail componentsInChild in (MapDetail[])gameObjectList[index3].GetComponentsInChildren<MapDetail>())
                                    mapDetailList.Remove(componentsInChild);
                                __instance.RecheckDisableObjectForXPMode(gameObjectList[index3]);
                            }
                            index2 = index3;
                            break;
                        }
                        num4 = num2;
                    }
                    if (index2 >= 0)
                    {
                        intList.RemoveAt(index2);
                        gameObjectList.RemoveAt(index2);
                    }
                    else
                    {
                        //Debug.LogError((object)("RandomSpawnObject did not activate an object:" + ((Object)((Component)this).get_gameObject()).get_name()));
                    }
                }
                UnityEngine.Random.state = state;
            }
        }

        //public static bool GonnaDoRangeForPrefabSpawn = false;
        //public static bool GonnaDoRollChanceForPrefabSpawn = false;
        //public static int PrefabSpawnerRangeCallCounter = 0;
        //public static int PrefabSpawnerGuidHash = 0;

        //[HarmonyLib.HarmonyPatch(typeof(PrefabSpawn), "SpawnObjects")]
        //private static class SpawnObjectCrap
        //{
        //    private static void Prefix(PrefabSpawn __instance)
        //    {
        //        GonnaDoRangeForPrefabSpawn = true;
        //        GonnaDoRollChanceForPrefabSpawn = true;
        //        if (__instance.gameObject.GetComponent<ObjectGuid>() != null && __instance.gameObject.GetComponent<ObjectGuid>().Get() != "")
        //        {
        //            PrefabSpawnerGuidHash = __instance.gameObject.GetComponent<ObjectGuid>().Get().GetHashCode();
        //        }
        //        else
        //        {
        //            PrefabSpawnerGuidHash = __instance.gameObject.transform.position.GetHashCode();
        //        }
        //        MelonLogger.Msg(ConsoleColor.Blue, "PrefabSpawn Prefix() of " + PrefabSpawnerGuidHash);
        //    }
        //    private static void Postfix(PrefabSpawn __instance)
        //    {
        //        MelonLogger.Msg(ConsoleColor.Blue, "PrefabSpawn Postfix() of " + PrefabSpawnerGuidHash);
        //        GonnaDoRangeForPrefabSpawn = false;
        //        GonnaDoRollChanceForPrefabSpawn = false;
        //        PrefabSpawnerRangeCallCounter = 0;
        //        PrefabSpawnerGuidHash = 0;
        //    }
        //}

        [HarmonyLib.HarmonyPatch(typeof(PrefabSpawn), "SpawnObjects")]
        private static class SpawnObjectCrap
        {
            private static bool Prefix(PrefabSpawn __instance)
            {
                return false;
            }
            private static void Postfix(PrefabSpawn __instance, ref GameObject __result)
            {
                if (__instance.m_PrefabList == null)
                {
                    __result = (GameObject)null;
                    return;
                }

                int CallCounter = 0;
                int GuidHash = 0;

                if (__instance.gameObject.GetComponent<ObjectGuid>() != null && __instance.gameObject.GetComponent<ObjectGuid>().Get() != "")
                {
                    GuidHash = __instance.gameObject.GetComponent<ObjectGuid>().Get().GetHashCode();
                }else{
                    GuidHash = __instance.gameObject.transform.position.GetHashCode();
                }

                int num1 = 0;
                for (int index = 0; index < __instance.m_PrefabList.Length; ++index)
                {
                    num1 = Mathf.Max(num1, __instance.m_PrefabList[index].m_SetId);
                }
                    
                List<List<PrefabSpawn.Element>> elementListList = new List<List<PrefabSpawn.Element>>();
                for (int index = 0; index <= num1; ++index)
                    elementListList.Add(new List<PrefabSpawn.Element>());
                for (int index = 0; index < __instance.m_PrefabList.Length; ++index)
                {
                    PrefabSpawn.Element prefab = __instance.m_PrefabList[index];
                    elementListList[prefab.m_SetId].Add(prefab);
                }
                int num2 = 0;
                List<int> intList = new List<int>();
                for (int index1 = 0; index1 < elementListList.Count; ++index1)
                {
                    intList.Add(0);
                    int num3 = 0;
                    for (int index2 = 0; index2 < elementListList[index1].Count; ++index2)
                        num3 += elementListList[index1][index2].m_SpawnWeight;
                    intList[index1] = num3;
                    num2 += num3;
                }

                CallCounter = CallCounter + 1;
                int seedminMax = GameManager.m_SceneTransitionData.m_GameRandomSeed + GuidHash + CallCounter;
                System.Random RNGminMax = new System.Random(seedminMax);
                int num4 = RNGminMax.Next(__instance.m_NumToSpawnMin, __instance.m_NumToSpawnMax + 1);
                int num5 = Mathf.Min(num1 == 0 ? __instance.m_PrefabList.Length : elementListList.Count, num4);
                int num6 = num5;
                if (__instance.m_NumToSpawnMin != 1 || __instance.m_NumToSpawnMax != 1)
                    num6 = Mathf.Max(0, num6 - GameManager.GetExperienceModeManagerComponent().GetReduceMaxItemsInContainer());
                float percent = (float)__instance.m_ChanceOfNoSpawn;
                if (num6 == 0 && num5 != 0)
                {
                    num6 = num5;
                    percent = Mathf.Min(75f, percent);
                }
                else if ((double)percent > 0.0)
                {
                    percent = (float)(100.0 - (100.0 - (double)percent) * (double)GameManager.GetExperienceModeManagerComponent().GetGearSpawnChanceScale());
                }

                int seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + GuidHash;
                System.Random RNG = new System.Random(seed);

                if (MyMod.RollChanceSeeded(percent, RNG))
                {
                    __result = (GameObject)null;
                    return;
                }
                List<GameObject> gameObjectList = new List<GameObject>();
                for (; num6 > 0; --num6)
                {
                    CallCounter = CallCounter + 1;
                    int seedNum3 = GameManager.m_SceneTransitionData.m_GameRandomSeed + GuidHash + CallCounter;
                    System.Random RNGnum3 = new System.Random(seedNum3);                              
                    int num3 = RNGnum3.Next(0, num2);
                    int num7 = 0;
                    for (int index1 = 0; index1 < elementListList.Count; ++index1)
                    {
                        int num8 = num7 + intList[index1];
                        if (num3 < num8)
                        {
                            int num9 = num7;
                            for (int index2 = 0; index2 < elementListList[index1].Count; ++index2)
                            {
                                PrefabSpawn.Element spawnElement = elementListList[index1][index2];
                                int num10 = num9 + spawnElement.m_SpawnWeight;
                                if (num3 < num10)
                                {
                                    GameObject gameObject = __instance.SpawnObject(spawnElement);
                                    if (gameObject != null)
                                    {
                                        gameObjectList.Add(gameObject);
                                    }
                                    if (elementListList.Count == 1)
                                    {
                                        num2 -= spawnElement.m_SpawnWeight;
                                        elementListList[index1].RemoveAt(index2);
                                        break;
                                    }
                                    break;
                                }
                                num9 = num10;
                            }
                            if (elementListList.Count > 1)
                            {
                                elementListList.RemoveAt(index1);
                                num2 -= intList[index1];
                                intList.RemoveAt(index1);
                                break;
                            }
                            break;
                        }
                        num7 = num8;
                    }
                }
                __result = (GameObject)null;
                return;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PrefabSpawn), "SpawnObject")] //SpawnObject and SpawnObjects is different methods, S you see? Nice names (no).
        private static class SpawnObjectCrapLogs
        {
            private static void Postfix(PrefabSpawn __instance, PrefabSpawn.Element spawnElement, ref GameObject __result)
            {
                if(__result != null)
                {
                    //MelonLogger.Msg("[PrefabSpawn Seeded] Spawned " + __result.gameObject.name+" X "+__result.gameObject.transform.position.x + " Y " + __result.gameObject.transform.position.y + " Z " + __result.gameObject.transform.position.z);
                }
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(Utils), "RollChance")]
        //private static class SpawnObjectRollChance
        //{
        //    private static void Postfix(float percent, ref bool __result)
        //    {
        //        if (GonnaDoRollChanceForPrefabSpawn)
        //        {
        //            int seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + PrefabSpawnerGuidHash;
        //            System.Random RNG = new System.Random(seed);
        //            GonnaDoRollChanceForPrefabSpawn = false;
        //            __result = MyMod.RollChanceSeeded(percent, RNG);
        //            MelonLogger.Msg(ConsoleColor.Blue, "PrefabSpawn " + PrefabSpawnerGuidHash + " Got RollChance result " + __result);
        //        }
        //    }
        //}

        //[HarmonyLib.HarmonyPatch(typeof(UnityEngine.Random), "RandomRangeInt", new Type[] { typeof(int), typeof(int) })]
        //private static class StateChanger
        //{
        //    private static UnityEngine.Random.State preHookState = new UnityEngine.Random.State();
        //    private static void Prefix()
        //    {
        //        MelonLogger.Msg(ConsoleColor.Yellow, "UnityEngine.Random.RandomRangeInt GonnaDoRangeForPrefabSpawn " + GonnaDoRangeForPrefabSpawn);
        //        preHookState = UnityEngine.Random.state;
        //        if (GonnaDoRangeForPrefabSpawn)
        //        {
        //            PrefabSpawnerRangeCallCounter = PrefabSpawnerRangeCallCounter + 1;
        //            int _seed = PrefabSpawnerGuidHash + PrefabSpawnerRangeCallCounter;
        //            UnityEngine.Random.InitState(GameManager.GetRandomSeed(_seed));
        //            MelonLogger.Msg(ConsoleColor.Blue, "PrefabSpawn " + PrefabSpawnerGuidHash + " Used Random.Range with seed " + GameManager.GetRandomSeed(_seed)+ " call counts "+ PrefabSpawnerRangeCallCounter);
        //        }
        //    }
        //    private static void Postfix()
        //    {
        //        if (GonnaDoRangeForPrefabSpawn)
        //        {
        //            UnityEngine.Random.state = preHookState;
        //        }
        //    }
        //}

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "DrinkFromWaterSupply")] //Part of code from TLD_RelativeConsumptionTime
        internal class PlayerManager_DrinkFromWaterSupply_Patch
        {
            internal static float restoreTimeToDrink; //Thank you, Remodor, now I know I can have varible right in patch class!
            internal static void Prefix(PlayerManager __instance, WaterSupply ws, float volumeAvailable)
            {
                restoreTimeToDrink = ws.m_TimeToDrinkSeconds;
                if(MyMod.ServerConfig.m_FastConsumption == false)
                {
                    ws.m_TimeToDrinkSeconds = 10;
                }
            }
            internal static void Postfix(PlayerManager __instance, WaterSupply ws)
            {
                ws.m_TimeToDrinkSeconds = restoreTimeToDrink;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "UseFoodInventoryItem")]//Part of code from TLD_RelativeConsumptionTime
        internal class PlayerManager_UseFoodInventoryItem_Patch
        {
            internal static float restoreTimeToEat; 
            internal static void Prefix(PlayerManager __instance, GearItem gi)
            {
                restoreTimeToEat = gi.m_FoodItem.m_TimeToEatSeconds;
                if (MyMod.ServerConfig.m_FastConsumption == false)
                {
                    gi.m_FoodItem.m_TimeToEatSeconds = 11;
                }
            }
            internal static void Postfix(PlayerManager __instance, GearItem gi)
            {
                gi.m_FoodItem.m_TimeToEatSeconds = restoreTimeToEat;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "GetInteractiveObjectDisplayText")]
        internal class PlayerManager_GetInteractiveObjectDisplayText
        {
            internal static void Postfix(PlayerManager __instance, GameObject interactiveObject, ref string __result)
            {
                if(interactiveObject != null)
                {
                    if(interactiveObject.GetComponent<MyMod.MultiplayerPlayer>() != null)
                    {
                        MyMod.MultiplayerPlayer mP = interactiveObject.GetComponent<MyMod.MultiplayerPlayer>();
                        int m_LevelId = MyMod.playersData[mP.m_ID].m_Levelid;
                        string m_LevelGUID = MyMod.playersData[mP.m_ID].m_LevelGuid;
                        if (MyMod.levelid != m_LevelId && MyMod.level_guid != m_LevelGUID)
                        {
                            return;
                        }
                        string actString = GetPriorityActionForPlayer(mP.m_ID, mP).m_DisplayText;
                        if(actString == "Look")
                        {
                            if (MyMod.playersData[mP.m_ID] != null && MyMod.playersData[mP.m_ID].m_Name != "")
                            {
                                actString = MyMod.playersData[mP.m_ID].m_Name;
                            }else{
                                actString = "Player";
                            }
                        }
                        __result = actString;
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "GetInteractiveObjectUnderCrosshairs")]
        internal class PlayerManager_GetInteractiveObjectUnderCrosshairs
        {
            internal static void Postfix(PlayerManager __instance, float maxRange, ref GameObject __result)
            {
                int layerMask = vp_Layer.Default;
                RaycastHit hit;

                if (Physics.Raycast(GameManager.GetMainCamera().transform.position, GameManager.GetMainCamera().transform.forward, out hit, maxRange))
                {
                    if(hit.collider.gameObject != null)
                    {
                        
                        GameObject hitObj = hit.collider.transform.gameObject;
                        //MelonLogger.Msg("Found something "+ hitObj.name);
                        if(hitObj.GetComponent<MyMod.PlayerBulletDamage>() != null)
                        {
                            if(hitObj.GetComponent<MyMod.PlayerBulletDamage>().m_Player != null)
                            {
                                int m_ID = hitObj.GetComponent<MyMod.PlayerBulletDamage>().m_ClientId;

                                if (MyMod.playersData[m_ID] != null)
                                {
                                    int m_LevelId = MyMod.playersData[m_ID].m_Levelid;
                                    string m_LevelGUID = MyMod.playersData[m_ID].m_LevelGuid;
                                    if (MyMod.levelid == m_LevelId && MyMod.level_guid == m_LevelGUID)
                                    {
                                        __result = hitObj.GetComponent<MyMod.PlayerBulletDamage>().m_Player;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "IsClickHoldActive")]
        internal class PlayerManager_IsClickHoldActive
        {
            internal static void Postfix(ref bool __result)
            {
                if (__result == false)
                {
                    if (MyMod.PlayerInteractionWith != null)
                    {
                        __result = true;
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "IsCancelableActionInProgress")]
        internal class PlayerManager_IsCancelableActionInProgress
        {
            internal static void Postfix(ref bool __result)
            {
                if(__result == false)
                {
                    if(MyMod.PlayerInteractionWith != null)
                    {
                        __result = true;
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(InputManager), "MaybeCancelClickHold")]
        internal class InputManager_MaybeCancelClickHold
        {
            internal static void Postfix(ref bool __result)
            {
                if (__result == false)
                {
                    if (MyMod.PlayerInteractionWith != null)
                    {
                        MyMod.LongActionCanceled(MyMod.PlayerInteractionWith);
                        __result = true;
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(InputManager), "IsClickHoldActive")]
        internal class InputManager_IsClickHoldActive
        {
            internal static void Postfix(ref bool __result)
            {
                if (__result == false)
                {
                    if (MyMod.PlayerInteractionWith != null)
                    {
                        __result = true;
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(PlayerStunned), "Update")]
        internal class PlayerStunned_UpdateLowHealthStagger
        {
            internal static bool Prefix()
            {
                if(MyMod.LowHealthStaggerBlockTime > 0)
                {
                    return false;
                }else{
                    return true;
                }
            }
        }

        public static MyMod.PriorityActionForOtherPlayer GetPriorityActionForPlayer(int m_ID, MyMod.MultiplayerPlayer mP)
        {
            MyMod.MultiPlayerClientData pData = MyMod.playersData[m_ID];
            MyMod.PriorityActionForOtherPlayer act = new MyMod.PriorityActionForOtherPlayer();

            if (pData != null && MyMod.playersData[mP.m_ID].m_AnimState == "Knock")
            {
                act = MyMod.GetActionForOtherPlayer("Revive");
            }
            else if (GameManager.m_PlayerManager != null && GameManager.m_PlayerManager.m_ItemInHands!= null && GameManager.m_PlayerManager.m_ItemInHands.m_EmergencyStim != null)
            {
                act = MyMod.GetActionForOtherPlayer("Stim");
            }
            else if (mP.m_BloodLosts > 0)
            {
                act = MyMod.GetActionForOtherPlayer("Bandage");
            }
            else if (mP.m_NeedAntiseptic == true)
            {
                act = MyMod.GetActionForOtherPlayer("Sterilize");
            }
            else
            {
                act = MyMod.GetActionForOtherPlayer("Look");
            }
            return act;
        }

        [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "InteractiveObjectsProcessInteraction")]
        internal class PlayerManager_InteractiveObjectsProcessInteraction
        {
            internal static void Postfix(PlayerManager __instance, ref bool __result)
            {
                if(__instance.m_InteractiveObjectUnderCrosshair != null)
                {
                    GameObject obj = __instance.m_InteractiveObjectUnderCrosshair;
                    if(obj.GetComponent<MyMod.MultiplayerPlayer>() != null)
                    {
                        MyMod.MultiplayerPlayer mP = obj.GetComponent<MyMod.MultiplayerPlayer>();

                        string PAction = GetPriorityActionForPlayer(mP.m_ID, mP).m_Action;
                        string ProcessText = GetPriorityActionForPlayer(mP.m_ID, mP).m_ProcessText;

                        if (PAction == "Bandage")
                        {
                            int bandages = GameManager.GetInventoryComponent().NumGearInInventory("GEAR_HeavyBandage");

                            if (bandages > 0)
                            {
                                MyMod.DoLongAction(mP, ProcessText, PAction);
                                __result = true;
                            }else{
                                HUDMessage.AddMessage("YOU HAVE NOT ANY BANDAGES");
                                GameAudioManager.PlayGUIError();
                                __result = false;
                            }
                        }
                        else if (PAction == "Sterilize")
                        {
                            bool HaveAntiseptic = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_BottleHydrogenPeroxide");

                            if (HaveAntiseptic)
                            {
                                MyMod.DoLongAction(mP, ProcessText, PAction);
                                __result = true;
                            }else{
                                HUDMessage.AddMessage("YOU HAVE NOT HYDROGEN PEROXIDE");
                                GameAudioManager.PlayGUIError();
                                __result = false;
                            }
                        }
                        else if(PAction == "Stim")
                        {
                            if(GameManager.m_PlayerManager != null && GameManager.m_PlayerManager.m_ItemInHands != null && GameManager.m_PlayerManager.m_ItemInHands.m_EmergencyStim != null)
                            {
                                MyMod.EmergencyStimBeforeUse = GameManager.m_PlayerManager.m_ItemInHands;
                                GameManager.GetPlayerManagerComponent().UseInventoryItem(GameManager.m_PlayerManager.m_ItemInHands);// Unequip GEAR_EmergencyStim
                                MyMod.DoLongAction(mP, ProcessText, PAction);
                                __result = true;
                            }
                        }
                        else if (PAction == "Revive")
                        {
                            bool HaveMedkit = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_MedicalSupplies_hangar");

                            if (HaveMedkit)
                            {
                                MyMod.DoLongAction(mP, ProcessText, PAction);
                                __result = true;
                            }
                            else
                            {
                                HUDMessage.AddMessage("YOU HAVE NOT MEDKIT");
                                GameAudioManager.PlayGUIError();
                                __result = false;
                            }
                        }
                    }
                } 
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Container), "Close")]
        public static class Container_UsingSyncClose
        {
            public static void Postfix(Container __instance, ref bool __result)
            {
                if(__result == true)
                {
                    if (MyMod.MyContainer != null)
                    {
                        MyMod.MyContainer = null;
                        //MelonLogger.Msg("Stop interacting wtih container");
                        MyMod.ContainerOpenSync pendingContainer = new MyMod.ContainerOpenSync();
                        pendingContainer.m_Guid = "NULL";
                        if (MyMod.sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.CONTAINERINTERACT))
                            {
                                _packet.Write(pendingContainer);
                                SendTCPData(_packet);
                            }
                        }
                        if (MyMod.iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.CONTAINERINTERACT))
                            {
                                ServerSend.CONTAINERINTERACT(0, pendingContainer, true);
                            }
                        }
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Container), "Open")]
        public static class Container_UsingSyncOpen
        {
            public static void Postfix(Container __instance, ref bool __result)
            {
                if (__result == true)
                {
                    MyMod.ContainerOpenSync pendingContainer = new MyMod.ContainerOpenSync();
                    pendingContainer.m_LevelID = MyMod.levelid;
                    pendingContainer.m_LevelGUID = MyMod.level_guid;
                    pendingContainer.m_Inspected = __instance.m_Inspected;

                    if (__instance.gameObject != null)
                    {
                        GameObject contObj = __instance.gameObject;
                        if (contObj.GetComponent<ObjectGuid>() != null)
                        {
                            pendingContainer.m_Guid = contObj.GetComponent<ObjectGuid>().Get();
                        }

                        for (int index = 0; index < __instance.m_Items.Count; ++index)
                        {
                            GearItem gearItem = __instance.m_Items.get_Item(index);
                            if (gearItem != null)
                            {
                                if(gearItem.m_GearName.Contains("MountainTownFarmKey") == true)
                                {
                                    return;
                                }
                            }
                        }
                    }
                    if (MyMod.MyContainer == null || MyMod.MyContainer.Equals(pendingContainer) == false)
                    {
                        MyMod.MyContainer = pendingContainer;
                        //MelonLogger.Msg("Interacting wtih container");
                        if (MyMod.sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.CONTAINERINTERACT))
                            {
                                _packet.Write(MyMod.MyContainer);
                                SendTCPData(_packet);
                            }
                        }
                        if (MyMod.iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.CONTAINERINTERACT))
                            {
                                ServerSend.CONTAINERINTERACT(0, MyMod.MyContainer, true);
                            }
                        }

                        MyMod.AddLootedContainer(MyMod.MyContainer, true, MyMod.instance.myId);
                    }
                }
            }
        }

        //[HarmonyLib.HarmonyPatch(typeof(ContainerManager), "Deserialize", new System.Type[] { typeof(string), typeof(Il2CppSystem.Collections.Generic.List<GearItem>) })]
        //public static class ContainerManager_Deserialize
        //{
        //    public static void Postfix(ContainerManager __instance)
        //    {

        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(GearManager), "Deserialize", new System.Type[] { typeof(string), typeof(Il2CppSystem.Collections.Generic.List<GearItem>) })]
        //public static class GearManager_Deserialize
        //{
        //    public static void Postfix()
        //    {

        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(HarvestableManager), "DeserializeAll")]
        //public static class HarvestableManager_Deserialize
        //{
        //    public static void Postfix()
        //    {

        //    }
        //}

        //[HarmonyLib.HarmonyPatch(typeof(SnowShelterManager), "DeserializeAll")]
        //public static class SnowShelterManager_Deserialize
        //{
        //    public static void Postfix()
        //    {
                
        //    }
        //}

        public static void SendHarvestPlantState(string state, Harvestable plant)
        {
            MyMod.HarvestableSyncData harvData = new MyMod.HarvestableSyncData();
            harvData.m_State = state;
            if (plant.gameObject != null)
            {
                GameObject plantObj = plant.gameObject;
                if (plantObj.GetComponent<ObjectGuid>() != null)
                {
                    harvData.m_Guid = plantObj.GetComponent<ObjectGuid>().Get();
                }
            }
            if (MyMod.sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.HARVESTPLANT))
                {
                    _packet.Write(harvData);
                    SendTCPData(_packet);
                }
            }
            if (MyMod.iAmHost == true || MyMod.InOnline() == false)
            {
                using (Packet _packet = new Packet((int)ServerPackets.HARVESTPLANT))
                {
                    ServerSend.HARVESTPLANT(0, harvData, true);
                }

                if(state == "Done")
                {
                    MyMod.AddHarvastedPlant(harvData.m_Guid, 0);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Harvestable), "DoHarvest")]
        public static class Harvestable_DoHarvest
        {
            public static void Postfix(Harvestable __instance)
            {
                SendHarvestPlantState("Start", __instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Harvestable), "CancelHarvest")]
        public static class Harvestable_CancelHarvest
        {
            public static void Postfix(Harvestable __instance)
            {
                SendHarvestPlantState("Cancel", __instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Harvestable), "CompletedHarvest")]
        public static class Harvestable_CompletedHarvest
        {
            public static void Postfix(Harvestable __instance, bool success)
            {
                if(success == true)
                {
                    SendHarvestPlantState("Done", __instance);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Harvestable), "ProcessInteraction")]
        public static class Harvestable_ProcessInteraction
        {
            public static bool Prefix(Harvestable __instance)
            {
                string ObjGUID = "";
                
                
                if (__instance.gameObject != null)
                {
                    GameObject contObj = __instance.gameObject;
                    if (contObj.GetComponent<ObjectGuid>() != null)
                    {
                        ObjGUID = contObj.GetComponent<ObjectGuid>().Get();
                    }

                    for (int i = 0; i < MyMod.playersData.Count; i++)
                    {
                        if (MyMod.playersData[i] != null)
                        {
                            string otherGuid = MyMod.playersData[i].m_HarvestingAnimal;
                            if (otherGuid == ObjGUID)
                            {
                                HUDMessage.AddMessage(MyMod.playersData[i].m_Name + " IS ALREADY COLLECTING THIS");
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Harvestable), "UpdateHarvesting")]
        public static class Harvestable_UpdateHarvesting
        {
            public static bool Prefix(Harvestable __instance)
            {
                if(GameManager.GetPlayerManagerComponent().m_HarvestableInProgress == __instance)
                {
                    string ObjGUID = "";
                    if (__instance.gameObject != null)
                    {
                        GameObject contObj = __instance.gameObject;
                        if (contObj.GetComponent<ObjectGuid>() != null)
                        {
                            ObjGUID = contObj.GetComponent<ObjectGuid>().Get();
                        }

                        for (int i = 0; i < MyMod.playersData.Count; i++)
                        {
                            if (MyMod.playersData[i] != null)
                            {
                                string otherGuid = MyMod.playersData[i].m_Plant;
                                if (otherGuid == ObjGUID)
                                {
                                    HUDMessage.AddMessage(MyMod.playersData[i].m_Name + " IS ALREADY COLLECTING THIS");
                                    return false;
                                }
                            }
                        }
                    }
                }

                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BootUpdate), "Start")]
        public class BootUpdate_Start
        {
            public static void Postfix(BootUpdate __instance)
            {
                if(NeedSkipCauseConnect() == false && Application.isBatchMode == false)
                {
                    return;
                }
                for (int i = 1; i <= 3; i++)
                {
                    __instance.gameObject.transform.Find($"Label_Disclaimer_{i}")?.gameObject.SetActive(false);
                }
                __instance.LoadMainMenu();
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(BootUpdate), "Update")]
        public class BootUpdate_Update
        {
            public static void Postfix(BootUpdate __instance)
            {
                if (NeedSkipCauseConnect() == false && Application.isBatchMode == false)
                {
                    return;
                }
                __instance.m_Label_Continue.gameObject.SetActive(false);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "Enable")]
        public class SkipIntroReduxSkipIntro
        {
            public static void Prefix(Panel_MainMenu __instance)
            {
                if (NeedSkipCauseConnect() == false && Application.isBatchMode == false)
                {
                    return;
                }
                MoviePlayer.m_HasIntroPlayedForMainMenu = true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "Enable")]
        internal class Panel_MainMenu_Enable
        {
            private static void Postfix(Panel_MainMenu __instance)
            {
                if (NeedSkipCauseConnect() == false && Application.isBatchMode == false)
                {
                    return;
                }
                __instance?.m_HinterlandMailingListWidget?.gameObject?.SetActive(false);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(TextInputField), "Start")]
        internal class TextInputField_MoreLetters
        {
            private static void Postfix(TextInputField __instance)
            {
                if(__instance.m_Input.characterLimit < 100)
                {
                    __instance.m_MaxLength = 100;
                    __instance.m_Input.characterLimit = (int)__instance.m_MaxLength;
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SnowShelter), "Start")]
        internal class SnowShelter_Serialize
        {
            private static bool Prefix(SnowShelter __instance)
            {
                if (__instance.m_StartHasBeenCalled)
                {
                    return false;
                }
                __instance.m_StartHasBeenCalled = true;
                if (__instance.gameObject != null && __instance.gameObject.GetComponent<MyMod.DoNotSerializeThis>() == null)
                {
                    SnowShelterManager.Add(__instance);
                    //MelonLogger.Msg("Registered an snowshelter, cause have not DoNotSerializeThis component.");
                }else{
                    //MelonLogger.Msg("Skip snowshelter, cause have DoNotSerializeThis component.");
                }
                return false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_SnowShelterBuild), "BuildFinished")]
        public static class Panel_SnowShelterBuild_BuildFinished
        {
            public static void Prefix(Panel_SnowShelterBuild __instance)
            {
                if(__instance.m_SnowShelter != null)
                {
                    MelonLogger.Msg("Shelter builded!");
                    GameObject shelter = __instance.m_SnowShelter.gameObject;
                    MyMod.ShelterCreated(shelter.transform.position, shelter.transform.rotation, MyMod.levelid, MyMod.level_guid, true);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SnowShelter), "DismantleFinished")]
        public static class SnowShelter_DismantleFinished
        {
            public static void Postfix(SnowShelter __instance)
            {
                MelonLogger.Msg("Shelter removed!");
                GameObject shelter = __instance.gameObject;
                MyMod.ShelterRemoved(shelter.transform.position, MyMod.levelid, MyMod.level_guid, true);
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_SnowShelterInteract), "OnUse")]
        public static class Panel_SnowShelterInteract_OnUse
        {
            public static void Postfix(Panel_SnowShelterInteract __instance)
            {
                if (__instance.m_SnowShelter != null)
                {
                    MelonLogger.Msg("Entering shelter");

                    MyMod.ShowShelterByOther shelter = new MyMod.ShowShelterByOther();
                    shelter.m_Position = __instance.m_SnowShelter.gameObject.transform.position;
                    shelter.m_Rotation = __instance.m_SnowShelter.gameObject.transform.rotation;
                    shelter.m_LevelID = MyMod.levelid;
                    shelter.m_LevelGUID = MyMod.level_guid;
                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.USESHELTER))
                        {
                            _packet.Write(shelter);
                            SendTCPData(_packet);
                        }
                    }
                    if (MyMod.iAmHost == true)
                    {
                        ServerSend.USESHELTER(0, shelter, true);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SnowShelterManager), "EnterShelter")]
        public static class SnowShelterManager_EnterShelter
        {
            public static void Postfix(SnowShelterManager __instance, SnowShelter ss)
            {
                if (ss != null)
                {
                    MelonLogger.Msg("Entering shelter");

                    MyMod.ShowShelterByOther shelter = new MyMod.ShowShelterByOther();
                    shelter.m_Position = ss.gameObject.transform.position;
                    shelter.m_Rotation = ss.gameObject.transform.rotation;
                    shelter.m_LevelID = MyMod.levelid;
                    shelter.m_LevelGUID = MyMod.level_guid;
                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.USESHELTER))
                        {
                            _packet.Write(shelter);
                            SendTCPData(_packet);
                        }
                    }
                    if (MyMod.iAmHost == true)
                    {
                        ServerSend.USESHELTER(0, shelter, true);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SnowShelterManager), "ExitShelter")]
        public static class SnowShelterManager_ExitShelter
        {
            public static void Postfix(SnowShelterManager __instance, SnowShelter ss)
            {
                if (ss != null)
                {
                    MelonLogger.Msg("Exiting shelter");
                    MyMod.ShowShelterByOther shelter = new MyMod.ShowShelterByOther();
                    if (MyMod.sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.USESHELTER))
                        {
                            _packet.Write(shelter);
                            SendTCPData(_packet);
                        }
                    }
                    if (MyMod.iAmHost == true)
                    {
                        ServerSend.USESHELTER(0, shelter, true);
                    }
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_SnowShelterInteract), "OnDismantle")]
        public static class Panel_SnowShelterInteract_OnDismantle
        {
            public static bool Prefix(Panel_SnowShelterInteract __instance)
            {
                MyMod.ShowShelterByOther FindData = new MyMod.ShowShelterByOther();
                if (__instance.m_SnowShelter != null && __instance.m_SnowShelter.gameObject != null)
                {
                    FindData.m_Position = __instance.m_SnowShelter.gameObject.transform.position;
                    FindData.m_Rotation = __instance.m_SnowShelter.gameObject.transform.rotation;
                    FindData.m_LevelID = MyMod.levelid;
                    FindData.m_LevelGUID = MyMod.level_guid;
                }
                for (int i = 0; i < MyMod.playersData.Count; i++)
                {
                    if (MyMod.playersData[i] != null)
                    {
                        MyMod.ShowShelterByOther shelter = MyMod.playersData[i].m_Shelter;
                        if (FindData == shelter)
                        {
                            HUDMessage.AddMessage(MyMod.playersData[i].m_Name + " INSIDE, CAN'T DISMANTLE THIS!");
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Fire), "AddFuel")]
        public static class Fire_AddFuel
        {
            public static void Postfix(Fire __instance, GearItem fuel)
            {
                if(__instance.gameObject != null && fuel != null)
                {
                    MelonLogger.Msg("[Fire][AddFuel] " + __instance.gameObject.name+" fuel is "+fuel.m_GearName);
                }
                else if(__instance.gameObject != null && fuel == null)
                {
                    MelonLogger.Msg("[Fire][AddFuel] " + __instance.gameObject.name + " fuel is null");
                }
                else if (__instance.gameObject == null)
                {
                    MelonLogger.Msg("[Fire][AddFuel] firesource is null");
                }

                if(__instance.m_StartedByPlayer == false)
                {
                    MelonLogger.Msg("Assign firesource of other player to myself");
                    __instance.m_StartedByPlayer = true;
                    MyMod.SendMyFire(__instance);
                }
                if (__instance.m_StartedByPlayer == true)
                {
                    MelonLogger.Msg("Added fuel to fire " + fuel.m_GearName);
                    MyMod.SendMyFire(__instance, fuel.m_GearName);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(PlacePoints), "ShouldShow")]
        public static class PlacePoints_ShouldShow
        {
            public static void Postfix(PlacePoints __instance, bool __result)
            {
                if(__instance.m_AttachedFire != null)
                {
                    if(__instance.m_AttachedFire.IsBurning() && __instance.m_AttachedFire.m_StartedByPlayer == false)
                    {
                        __result = false;
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(CookingSlot), "CanBeInteractedWith")]
        public static class CookingSlot_CanBeInteractedWith
        {
            public static bool Prefix(CookingSlot __instance)
            {
                if (__instance.m_GearPlacePoint != null
                    && __instance.m_GearPlacePoint.m_FireToAttach != null
                    && __instance.m_GearPlacePoint.m_FireToAttach.IsBurning()
                    && __instance.m_GearPlacePoint.m_FireToAttach.m_StartedByPlayer == false)
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(CookingPotItem), "CanOpenCookingInterface")]
        public static class CookingPotItem_CanOpenCookingInterface
        {
            public static bool Prefix(CookingPotItem __instance)
            {
                if (__instance.m_FireBeingUsed != null
                    && __instance.m_FireBeingUsed.IsBurning()
                    && __instance.m_FireBeingUsed.m_StartedByPlayer == false)
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(CookingPotItem), "AttachedFireIsBurning")]
        public static class CookingPotItem_AttachedFireIsBurning
        {
            public static bool Prefix(CookingPotItem __instance)
            {
                if (__instance.m_FireBeingUsed != null
                    && __instance.m_FireBeingUsed.IsBurning()
                    && __instance.m_FireBeingUsed.m_StartedByPlayer == false)
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(WoodStove), "GetHoverText")]
        public static class WoodStove_GetHoverText
        {
            public static void Postfix(WoodStove __instance, ref string __result)
            {
                if (__instance.m_Fire != null
                    && __instance.m_Fire.IsBurning()
                    && __instance.m_Fire.m_StartedByPlayer == false)
                {
                    __result = __instance.m_LocalizedDisplayName.Text() + "\nADD YOUR FUEL\nTO ABLE COOK AND WARM";
                    //MelonLogger.Msg(__result);
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Campfire), "GetHoverText")]
        public static class Campfire_GetHoverText
        {
            public static void Postfix(Campfire __instance, ref string __result)
            {
                if (__instance.m_Fire != null
                    && __instance.m_Fire.IsBurning()
                    && __instance.m_Fire.m_StartedByPlayer == false)
                {
                    __result = __instance.m_DisplayName + "\nADD YOUR FUEL\nTO ABLE COOK AND WARM";
                    //MelonLogger.Msg(__result);
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(InterfaceManager), "Awake")]
        public static class UI_InterfaceManager
        {
            public static void Prefix(InterfaceManager __instance)
            {
                if (__instance != null)
                {
                    MyMod.m_InterfaceManager = __instance;
                    MelonLogger.Msg("[Legacy UIs] Captured link on InterfaceManager!");
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectSurvivor), "Initialize")]
        public static class UI_Panel_SelectSurvivor
        {
            public static void Prefix(Panel_SelectSurvivor __instance)
            {
                if (__instance != null)
                {
                    MyMod.m_Panel_SelectSurvivor = __instance;
                    MelonLogger.Msg("[Legacy UIs] Captured link on Panel_SelectSurvivor!");
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_MainMenu), "Initialize")]
        public static class UI_Panel_MainMenu
        {
            public static void Prefix(Panel_MainMenu __instance)
            {
                if (__instance != null)
                {
                    MyMod.m_Panel_MainMenu = __instance;
                    MelonLogger.Msg("[Legacy UIs] Captured link on Panel_MainMenu!");
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectRegion), "Initialize")]
        public static class UI_Panel_SelectRegion
        {
            public static void Prefix(Panel_SelectRegion __instance)
            {
                if (__instance != null)
                {
                    MyMod.m_Panel_SelectRegion = __instance;
                    MelonLogger.Msg("[Legacy UIs] Captured link on Panel_SelectRegion!");
                }
            }
        }

        //[HarmonyLib.HarmonyPatch(typeof(LootTable), "GetRandomGearPrefab")]
        //public static class LootTable_SeededRandom
        //{
        //    public static bool Prefix()
        //    {
        //        return false;
        //    }

        //    public static List<LootTableItem> m_FilteredLootTableItems = new List<LootTableItem>();

        //    public static void Postfix(LootTable __instance, ref GameObject __result)
        //    {
        //        //MelonLogger.Msg("Proessing loottable " + __instance.gameObject.name);
        //        //MelonLogger.Msg("Prefabs in loottable: " + __instance.m_Prefabs.Count);
        //        int index1 = 0;
        //        for (int index2 = 0; index2 < __instance.m_Prefabs.Count; ++index2)
        //        {
        //            if (index1 >= 256)
        //            {
        //                break;
        //            }
        //            if (!__instance.DisableForXPMode(__instance.m_Prefabs[index2]))
        //            {
        //                GameObject pref = __instance.m_Prefabs[index2];
        //                int weig = __instance.m_Weights[index2];
        //                //MelonLogger.Msg("__instance: " + pref + " Chance " + weig + "%");

        //                if(m_FilteredLootTableItems.Count <= index1) 
        //                {
        //                    m_FilteredLootTableItems.Add(new LootTableItem());
        //                }


        //                m_FilteredLootTableItems[index1].m_Weight = weig;
        //                m_FilteredLootTableItems[index1].m_Prefab = pref;
        //                //MelonLogger.Msg("FilteredLootTableItems: " + m_FilteredLootTableItems[index1].m_Prefab.name + " Chance " + m_FilteredLootTableItems[index1].m_Weight + "%");
        //                ++index1;
        //            }
        //        }
        //        //MelonLogger.Msg("Found " + index1 + " for this experience mode");
        //        GameObject go = __instance.gameObject;
        //        int _x = (int)go.transform.position.x;
        //        int _y = (int)go.transform.position.y;
        //        int _z = (int)go.transform.position.z;
        //        int seed = GameManager.m_SceneTransitionData.m_GameRandomSeed + _x + _y + _z;
        //        MelonLogger.Msg("Input values for Random: X " + _x + " Y " + _y + " Z " + _z + " GameSeed " + GameManager.m_SceneTransitionData.m_GameRandomSeed + " Final seed " + seed + " for table " + __instance.gameObject.name);
        //        System.Random RNG = new System.Random(seed);

        //        int num1 = 0;
        //        for (int index2 = 0; index2 < index1; ++index2)
        //        {
        //            if (m_FilteredLootTableItems[index2] == null)
        //            {
        //                //MelonLogger.Msg("LootTable.m_FilteredLootTableItems[" + index2 + "] is NULL");
        //            }
        //            else
        //            {
        //                if (m_FilteredLootTableItems[index2].m_Prefab != null)
        //                {
        //                    num1 += m_FilteredLootTableItems[index2].m_Weight;
        //                }
        //            }
        //        }

        //        int num2 = RNG.Next(0, num1);

        //        MelonLogger.Msg("Got random int " + num2 + " from 0/" + num1 + " for " + __instance.gameObject.name);
        //        int num3 = 0;
        //        int index3 = 0;
        //        for (int index2 = 0; index2 < index1; ++index2)
        //        {
        //            int num4 = num3 + m_FilteredLootTableItems[index2].m_Weight;
        //            if (num2 >= num3 && num2 < num4)
        //            {
        //                if (m_FilteredLootTableItems[index3].m_Prefab == null)
        //                {
        //                    //MelonLogger.Msg("Null prefab found at loot table index: " + index3 + " for loot table: " + __instance.gameObject.name);
        //                    __result = (GameObject)null;
        //                    return;
        //                }

        //                if (m_FilteredLootTableItems[index3].m_Prefab.GetComponent<GearItem>() == null)
        //                {
        //                    //MelonLogger.Msg("Gear Null found  at loot table index " + index3 + " for loot table: " + __instance.gameObject.name);
        //                    __result = (GameObject)null;
        //                    return;
        //                }
        //                else
        //                {
        //                    MelonLogger.Msg("Picked result " + m_FilteredLootTableItems[index3].m_Prefab.name + " for loot table: " + __instance.gameObject.name);
        //                    __result = m_FilteredLootTableItems[index3].m_Prefab;
        //                    return;
        //                }

        //                //__result = LootTable.m_FilteredLootTableItems[index3].m_Prefab.GetComponent<GearItem>() == null ? (GameObject)null : LootTable.m_FilteredLootTableItems[index3].m_Prefab;
        //                //return;
        //            }
        //            num3 = num4;
        //            ++index3;
        //        }
        //        MelonLogger.Msg("Found nothing for loot table: " + __instance.gameObject.name);
        //        __result = (GameObject)null;
        //        return;
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(SaveGameSystem), "LoadSceneData")]
        //private static class LoadSceneData_LevelGUID
        //{
        //    internal static void Prefix(SaveGameSystem _instance, string name, string sceneSaveName)
        //    {
        //        MelonLogger.Msg("Loading scene GUID "+ sceneSaveName);
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(LightFadeFire), "LateUpdate")]
        //private static class LateUpdate_Hook
        //{
        //    internal static bool Prefix(LightFadeFire __instance)
        //    {
        //        if (GameManager.m_IsPaused)
        //            return false;
        //        if (__instance.m_Light)
        //        {
        //            __instance.m_Light = __instance.gameObject.GetComponent<Light>();
        //            __instance.originalBrightness = __instance.m_Light.intensity;
        //            if (__instance.startOff)
        //            {
        //                __instance.m_Light.intensity = 0.0f;
        //                __instance.m_Light.enabled = false;
        //            }
        //        }else{
        //            return false;
        //        }
        //        __instance.fireTime = 10000;
        //        if (__instance.fireTime > __instance.maxTimeSec)
        //        {
        //            __instance.m_Light.intensity = __instance.originalBrightness * 1;
        //            if (__instance.autoSwitchAfterIngnition)
        //            {
        //                __instance.autoSwitchAfterIngnition = false;
        //                __instance.SetNewMaxTime(__instance.fullOnMinutes * 60f);
        //            }
        //        }
        //        else if (__instance.fireTime > __instance.minTimeSec && __instance.fireTime <= __instance.maxTimeSec)
        //        {
        //            __instance.fadeTimeStep = __instance.fireTime * __instance.fadeTimeDifference;
        //            __instance.m_Light.intensity = Mathf.Lerp(0.0f, __instance.originalBrightness, __instance.fadeTimeStep) * __instance.sourceFire.m_LightIntensityMultiplier;
        //            if (__instance.useFireIgnitionFirst)
        //            {
        //                __instance.useFireIgnitionFirst = false;
        //                __instance.SetNewMaxTime(100000);
        //            }
        //        }
        //        else if (__instance.fireTime <= __instance.minTimeSec)
        //            __instance.m_Light.intensity = 0.0f;
        //        if (Utils.IsZero(__instance.m_Light.intensity, 0.0001f))
        //            __instance.m_Light.enabled = false;
        //        else
        //            __instance.m_Light.enabled = true;
        //        return false;
        //    }
        //}
        [HarmonyLib.HarmonyPatch(typeof(Condition), "KillPlayer")]
        public class Condition_Test
        {
            public static bool Prefix(Condition __instance)
            {
                MelonLogger.Msg("[Condition] PlayerDeath");
                if (MyMod.InOnline() == true)
                {
                    __instance.ForceKill();
                    return false;
                }else{
                    return true;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Rest), "EndSleeping")]
        public class Rest_StopGod
        {
            public static void Postfix(Rest __instance)
            {
                MelonLogger.Msg("[Rest] EndSleeping");
                if(MyMod.IsDead == true)
                {
                    GameManager.GetPlayerManagerComponent().m_God = false;
                }
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(GameManager), "SetAudioModeForLoadedScene")]
        //internal static class GameManager_SetAudioModeForLoadedScene
        //{
        //    private static void Prefix()
        //    {
        //        MelonLogger.Msg("Prepare Scene");
        //    }
        //}

        public static void LoadEverything()
        {
            MelonLogger.Msg(ConsoleColor.Yellow, "Loading everything...");
            MyMod.SendSpawnData();
            MyMod.DestoryBrokenFurniture();
            MyMod.UpdateDeployedRopes();
            MyMod.ApplyLootedContainers();
            MyMod.RemoveHarvastedPlants();
            MyMod.LoadAllSnowSheltersByOther();
            MyMod.DestoryPickedGears();
            MyMod.ApplyOtherCampfires = true;
            MelonLogger.Msg(ConsoleColor.Yellow, "Loading done!");
        }


        [HarmonyLib.HarmonyPatch(typeof(Panel_Loading), "Enable")]
        internal static class Panel_Loading_Toggle
        {
            private static void Postfix(Panel_Loading __instance, bool enable)
            {
                if(enable == false)
                {
                    MelonLogger.Msg(ConsoleColor.Yellow, "[Scene Load] Everything finished loading!");
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SaveGameSystem), "LoadSceneData")]
        public static class SaveGameSystemPatch_LoadSceneData
        {
            public static void Postfix(string name)
            {
                MelonLogger.Msg(ConsoleColor.Yellow, "[Scene Load] Saved data for scene is loaded!");
                MyMod.UpdateEverything = 2;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(InputManager), "PauseGame")]
        public static class InputManager_DuckYOuPause
        {
            public static void Postfix()
            {
                if (MyMod.InOnline() == true)
                {
                    GameManager.m_IsPaused = false;
                }
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(BaseAiManager), "Serialize")]
        //public static class BaseAiManager_Serialize
        //{
        //    public static void Prefix()
        //    {
        //        MelonLogger.Msg("[BaseAiManager][Serialize] Started");
        //        BaseAiSaveList baseAiSaveList = new BaseAiSaveList();
        //        baseAiSaveList.m_SerializedBaseAI.Clear();
        //        for (int i = 0; i < BaseAiManager.m_BaseAis.Count; i++)
        //        {
        //            MelonLogger.Msg("[BaseAiManager][Serialize] Index "+ i);
        //            BaseAi baseAi = BaseAiManager.m_BaseAis.get_Item(i);
        //            if (baseAi && baseAi.GetAiMode() != AiMode.Dead && !(baseAi.GetSpawnRegionParent() != null) && (!baseAi.m_AiBear || ExperienceModeManager.GetCurrentExperienceModeType() != ExperienceModeType.ChallengeHunted) && (!baseAi.GetComponent<DisableObjectForXPMode>() || !baseAi.GetComponent<DisableObjectForXPMode>().ShouldDisableForCurrentMode()) && (!baseAi.GetComponent<EnableObjectForXPMode>() || !baseAi.GetComponent<EnableObjectForXPMode>().ShouldDisableForCurrentMode()) && (baseAi.gameObject.activeSelf || !baseAi.GetComponent<MissionObjectIdentifier>() || !baseAi.GetComponent<MissionObjectIdentifier>().m_BaseAiOnlySerializeIfActive))
        //            {
        //                BaseAiData baseAiData = baseAi.GetDeferredDeserializeData();
        //                MelonLogger.Msg("[BaseAiManager][Serialize] Index " + i);
        //                if (baseAiData == null)
        //                {
        //                    MelonLogger.Msg("[BaseAiManager][Serialize] Null! Making new one");
        //                    baseAiData = new BaseAiData();
        //                    baseAiData.m_Position = baseAi.transform.position;
        //                    baseAiData.m_Rotation = baseAi.transform.rotation;
        //                    baseAiData.m_PrefabName = baseAi.name;
        //                    baseAiData.m_BaseAiSerialized = baseAi.Serialize();
        //                    baseAiData.m_Guid = Utils.GetGuidFromGameObject(baseAi.gameObject);
        //                }else{
        //                    MelonLogger.Msg("[BaseAiManager][Serialize] Not null");
        //                }
        //                baseAiSaveList.m_SerializedBaseAI.Add(baseAiData);
        //            }else{
        //                if(baseAi == null)
        //                {
        //                    MelonLogger.Msg("[BaseAiManager][Serialize] Error 1");
        //                }
        //                else if (baseAi.GetAiMode() == AiMode.Dead)
        //                {
        //                    MelonLogger.Msg("[BaseAiManager][Serialize] Error 2");
        //                }
        //                else if (baseAi.GetSpawnRegionParent() != null)
        //                {
        //                    MelonLogger.Msg("[BaseAiManager][Serialize] Error 3");
        //                }
        //                else if((baseAi.m_AiBear || ExperienceModeManager.GetCurrentExperienceModeType() == ExperienceModeType.ChallengeHunted))
        //                {
        //                    MelonLogger.Msg("[BaseAiManager][Serialize] Error 4");
        //                }
        //                else if((baseAi.GetComponent<DisableObjectForXPMode>() || baseAi.GetComponent<DisableObjectForXPMode>().ShouldDisableForCurrentMode()))
        //                {
        //                    MelonLogger.Msg("[BaseAiManager][Serialize] Error 5");
        //                }
        //                else if((baseAi.GetComponent<EnableObjectForXPMode>() || baseAi.GetComponent<EnableObjectForXPMode>().ShouldDisableForCurrentMode()))
        //                {
        //                    MelonLogger.Msg("[BaseAiManager][Serialize] Error 6");
        //                }
        //                else if ((baseAi.gameObject.activeSelf == false || !baseAi.GetComponent<MissionObjectIdentifier>() || baseAi.GetComponent<MissionObjectIdentifier>().m_BaseAiOnlySerializeIfActive))
        //                {
        //                    MelonLogger.Msg("[BaseAiManager][Serialize] Error 7");
        //                }
        //            }
        //        }
        //        MelonLogger.Msg("[BaseAiManager][Serialize] Final result " + Utils.SerializeObject(baseAiSaveList));
        //    }
        //}
        //[HarmonyLib.HarmonyPatch(typeof(BaseAi), "Serialize")]
        //public static class BaseAiManager_Serialize
        //{
        //    public static void Postfix(string __result)
        //    {
        //        MelonLogger.Msg("[BaseAi] Serialize result " + __result);
        //    }
        //}
        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "SpawnRegionCloseEnoughForSpawning")]
        public static class SpawnRegion_SpawnRegionCloseEnoughForSpawning
        {
            public static void Postfix(SpawnRegion __instance, bool __result)
            {
                float maxDis = __instance.m_Radius + GameManager.GetSpawnRegionManager().m_SpawnRegionDisableDistance;
                bool AllPlayersAllowsIt = MyMod.CheckDistanceAgnistEveryPlayer(__instance.m_Center, maxDis, true);

                if (AllPlayersAllowsIt == true)
                {
                    __result = true;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "SpawnPositionOnScreenTooClose")]
        public static class SpawnRegion_SpawnPositionOnScreenTooClose
        {
            public static void Postfix(SpawnRegion __instance, Vector3 spawnPos, bool __result)
            {
                bool AllPlayersAllowsIt = MyMod.CheckDistanceAgnistEveryPlayer(spawnPos, GameManager.GetSpawnRegionManager().m_AllowSpawnOnscreenDistance, true);
                if (AllPlayersAllowsIt == true)
                {
                    __result = true;
                }
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(SpawnRegion), "RemoveActiveSpawns")]
        public static class SpawnRegion_RemoveActiveSpawns
        {
            public static bool Prefix(SpawnRegion __instance, int numToDeActivate, WildlifeMode wildlifeMode, bool isAdjustingOtherWildlifeMode)
            {
                for (int index = 0; index < __instance.m_Spawns.Count && numToDeActivate != 0; ++index)
                {
                    if (__instance.m_Spawns.get_Item(index) && __instance.m_Spawns.get_Item(index).gameObject.activeSelf)
                    {
                        GameManager.GetPackManager().UnregisterPackAnimal(__instance.m_Spawns.get_Item(index).m_PackAnimal, false);
                        if ((isAdjustingOtherWildlifeMode && __instance.HasSameWildlifeMode(__instance.m_Spawns.get_Item(index), wildlifeMode) || !__instance.HasSameWildlifeMode(__instance.m_Spawns.get_Item(index), wildlifeMode)) && (__instance.m_Spawns.get_Item(index).GetAiMode() != AiMode.Flee && __instance.m_Spawns.get_Item(index).GetAiMode() != AiMode.Dead))
                        {
                            __instance.m_Spawns.get_Item(index).SetAiMode(AiMode.Flee);
                        }
                        Vector3 pos = __instance.m_Spawns.get_Item(index).m_CachedTransform.position;

                        bool AllPlayersAllowsIt = false;

                        //150 because GameManager.GetSpawnRegionManager().m_DisallowDespawnBelowDistance
                        if (MyMod.CheckDistanceAgnistEveryPlayer(pos, 150, false))
                        {
                            AllPlayersAllowsIt = true;
                        }else{
                            //MelonLogger.Msg("Want to unload spawn area but can't cause any other player there");
                        }
                        if(MyMod.InOnline() == false)
                        {
                            AllPlayersAllowsIt = false;
                        }

                        if (AllPlayersAllowsIt == true && (Utils.DistanceToMainCamera(pos) >= GameManager.GetSpawnRegionManager().m_DisallowDespawnBelowDistance && (!Utils.PositionIsOnscreen(pos) || (double)Utils.DistanceToMainCamera(pos) >= (double)GameManager.GetSpawnRegionManager().m_AllowDespawnOnscreenDistance) && !Utils.PositionIsInLOSOfPlayer(pos)))
                        {
                            if (isAdjustingOtherWildlifeMode && __instance.HasSameWildlifeMode(__instance.m_Spawns.get_Item(index), wildlifeMode) || !__instance.HasSameWildlifeMode(__instance.m_Spawns.get_Item(index), wildlifeMode))
                            {
                                __instance.m_Spawns.get_Item(index).Despawn();
                                --numToDeActivate;
                            }
                            else if ((!__instance.m_Spawns.get_Item(index).m_CurrentTarget || !__instance.m_Spawns.get_Item(index).m_CurrentTarget.IsPlayer()) && __instance.m_Spawns.get_Item(index).GetAiMode() != AiMode.Feeding && __instance.m_Spawns.get_Item(index).GetAiMode() != AiMode.Sleep && !__instance.m_Spawns.get_Item(index).IsBleedingOut() && (!__instance.m_Spawns.get_Item(index).m_AiWolf || __instance.m_Spawns.get_Item(index).GetAiMode() != AiMode.WanderPaused))
                            {
                                __instance.m_Spawns.get_Item(index).Despawn();
                                numToDeActivate--;
                            }
                        }
                    }
                }
                return false;
            }
        }
        [HarmonyLib.HarmonyPatch(typeof(Panel_SelectExperience), "OnExperienceClicked")]
        public static class Panel_SelectExperience_OnExperienceClicked
        {
            public static bool Prefix(Panel_SelectExperience __instance)
            {
                Panel_SelectExperience.XPModeMenuItem selectedMenuItem = __instance.GetSelectedMenuItem();
                if (selectedMenuItem == null)
                    return false;
                ExperienceModeType _type = selectedMenuItem.m_Type;
                if(_type == ExperienceModeType.Custom)
                {
                    MyMod.NoCustomExp();
                    return false;
                }
                return true;
            }
        }
        //[HarmonyLib.HarmonyPatch(typeof(GearItem), "Deserialize")]
        //public class GearItem_LoadDebug
        //{
        //    public static bool Prefix(GearItem __instance, string text, bool applyPositioningFix = true)
        //    {
        //        MelonLogger.Msg(ConsoleColor.Blue, "[GearItem][Deserialize] Starting...");
        //        if (text == null)
        //            return false;
        //        GearItemSaveDataProxy itemSaveDataProxy = Utils.DeserializeObject<GearItemSaveDataProxy>(text);
        //        MelonLogger.Msg(ConsoleColor.Green, "[GearItem][Deserialize] itemSaveDataProxy.m_NormalizedCondition " + itemSaveDataProxy.m_NormalizedCondition);
        //        MelonLogger.Msg(ConsoleColor.Green, "[GearItem][Deserialize] itemSaveDataProxy.m_CurrentHPProxy " + itemSaveDataProxy.m_CurrentHPProxy);
        //        MelonLogger.Msg(ConsoleColor.Green, "[GearItem][Deserialize] itemSaveDataProxy.m_MaxHP" + __instance.m_MaxHP);
        //        bool flag1 = false;
        //        if (float.IsNaN((float)itemSaveDataProxy.m_Position.x) || float.IsNaN((float)itemSaveDataProxy.m_Position.y) || float.IsNaN((float)itemSaveDataProxy.m_Position.z))
        //        {
        //            itemSaveDataProxy.m_Position = GameManager.GetPlayerTransform().position;
        //            flag1 = true;
        //        }
        //        if (float.IsNaN((float)itemSaveDataProxy.m_Rotation.x) || float.IsNaN((float)itemSaveDataProxy.m_Rotation.y) || (float.IsNaN((float)itemSaveDataProxy.m_Rotation.z) || float.IsNaN((float)itemSaveDataProxy.m_Rotation.w)))
        //            itemSaveDataProxy.m_Rotation = Quaternion.identity;
        //        __instance.transform.position = itemSaveDataProxy.m_Position;
        //        __instance.transform.rotation = itemSaveDataProxy.m_Rotation;
        //        Physics.SyncTransforms();
        //        __instance.m_InstanceID = itemSaveDataProxy.m_InstanceIDProxy;
        //        __instance.m_CurrentHP = itemSaveDataProxy.m_CurrentHPProxy;
        //        __instance.m_CurrentHP = Mathf.Clamp(__instance.m_CurrentHP, 0.0f, __instance.m_MaxHP);
        //        MelonLogger.Msg(ConsoleColor.Yellow, "[GearItem][Deserialize] __instance.m_CurrentHP (after clamp) " + __instance.m_CurrentHP);
        //        if (!Utils.IsZero(itemSaveDataProxy.m_NormalizedCondition, 0.0001f))
        //            __instance.m_CurrentHP = itemSaveDataProxy.m_NormalizedCondition * __instance.m_MaxHP;
        //            MelonLogger.Msg(ConsoleColor.DarkYellow, "[GearItem][Deserialize] __instance.m_CurrentHP (After normalizing) " + __instance.m_CurrentHP);
        //        __instance.m_BeenInPlayerInventory = itemSaveDataProxy.m_BeenInPlayerInventoryProxy;
        //        __instance.m_BeenInContainer = itemSaveDataProxy.m_BeenInContainerProxy;
        //        __instance.m_BeenInspected = itemSaveDataProxy.m_BeenInspectedProxy;
        //        __instance.m_ItemLooted = itemSaveDataProxy.m_ItemLootedProxy;
        //        __instance.m_HasBeenOwnedByPlayer = itemSaveDataProxy.m_HasBeenOwnedByPlayer;
        //        __instance.m_RolledSpawnChance = itemSaveDataProxy.m_RolledSpawnChanceProxy;
        //        __instance.m_WornOut = itemSaveDataProxy.m_WornOut;
        //        __instance.m_HarvestedByPlayer = itemSaveDataProxy.m_HarvestedByPlayer;
        //        __instance.m_IsInSatchel = itemSaveDataProxy.m_IsInSatchel;
        //        __instance.m_SatchelIndex = itemSaveDataProxy.m_SatchelIndex;
        //        __instance.m_LockedInContainer = itemSaveDataProxy.m_LockedInContainer;
        //        __instance.m_NonInteractive = itemSaveDataProxy.m_NonInteractive;
        //        __instance.DeserializeRigidBody(itemSaveDataProxy.m_RigidBodySerialized);
        //        if (__instance.m_StackableItem && itemSaveDataProxy.m_StackableItemSerialized != null)
        //            __instance.m_StackableItem.Deserialize(itemSaveDataProxy.m_StackableItemSerialized);
        //        if (__instance.m_FoodItem && itemSaveDataProxy.m_FoodItemSerialized != null)
        //        {
        //            __instance.m_FoodItem.Deserialize(itemSaveDataProxy.m_FoodItemSerialized);
        //            if (__instance.m_FoodItem.WasHarvested())
        //                __instance.m_HarvestedByPlayer = true;
        //        }
        //        if (__instance.m_LiquidItem && itemSaveDataProxy.m_LiquidItemSerialized != null)
        //            __instance.m_LiquidItem.Deserialize(itemSaveDataProxy.m_LiquidItemSerialized);
        //        if (__instance.m_FlareItem && itemSaveDataProxy.m_FlareItemSerialized != null)
        //            __instance.m_FlareItem.Deserialize(itemSaveDataProxy.m_FlareItemSerialized);
        //        if (__instance.m_FlashlightItem && itemSaveDataProxy.m_FlashlightItemSerialized != null)
        //            __instance.m_FlashlightItem.Deserialize(itemSaveDataProxy.m_FlashlightItemSerialized);
        //        if (__instance.m_KeroseneLampItem && itemSaveDataProxy.m_KeroseneLampItemSerialized != null)
        //            __instance.m_KeroseneLampItem.Deserialize(itemSaveDataProxy.m_KeroseneLampItemSerialized);
        //        if (__instance.m_ClothingItem && itemSaveDataProxy.m_ClothingItemSerialized != null)
        //            __instance.m_ClothingItem.Deserialize(itemSaveDataProxy.m_ClothingItemSerialized);
        //        if (__instance.m_GunItem && itemSaveDataProxy.m_WeaponItemSerialized != null)
        //            __instance.m_GunItem.Deserialize(itemSaveDataProxy.m_WeaponItemSerialized);
        //        if (__instance.m_WaterSupply && itemSaveDataProxy.m_WaterSupplySerialized != null)
        //            __instance.m_WaterSupply.Deserialize(itemSaveDataProxy.m_WaterSupplySerialized);
        //        if (__instance.m_Bed && itemSaveDataProxy.m_BedSerialized != null)
        //            __instance.m_Bed.Deserialize(itemSaveDataProxy.m_BedSerialized);
        //        if (__instance.m_SmashableItem && itemSaveDataProxy.m_SmashableItemSerialized != null)
        //            __instance.m_SmashableItem.Deserialize(itemSaveDataProxy.m_SmashableItemSerialized);
        //        if (__instance.m_MatchesItem && itemSaveDataProxy.m_MatchesItemSerialized != null)
        //            __instance.m_MatchesItem.Deserialize(itemSaveDataProxy.m_MatchesItemSerialized);
        //        if (__instance.m_SnareItem && itemSaveDataProxy.m_SnareItemSerialized != null)
        //            __instance.m_SnareItem.Deserialize(itemSaveDataProxy.m_SnareItemSerialized);
        //        if (itemSaveDataProxy.m_InProgressItemSerialized != null)
        //        {
        //            if (__instance.m_InProgressCraftItem == null)
        //                __instance.m_InProgressCraftItem = __instance.gameObject.AddComponent<InProgressCraftItem>();
        //            __instance.m_InProgressCraftItem.Deserialize(itemSaveDataProxy.m_InProgressItemSerialized);
        //        }
        //        if (__instance.m_TorchItem && itemSaveDataProxy.m_TorchItemSerialized != null)
        //            __instance.m_TorchItem.Deserialize(itemSaveDataProxy.m_TorchItemSerialized);
        //        if (__instance.m_EvolveItem && itemSaveDataProxy.m_EvolveItemSerialized != null)
        //            __instance.m_EvolveItem.Deserialize(itemSaveDataProxy.m_EvolveItemSerialized);
        //        if (__instance.m_BodyHarvest && itemSaveDataProxy.m_BodyHarvestSerialized != null)
        //            __instance.m_BodyHarvest.Deserialize(itemSaveDataProxy.m_BodyHarvestSerialized, true);
        //        if (__instance.m_CookingPotItem && itemSaveDataProxy.m_CookingPotItemSerialized != null)
        //            __instance.m_CookingPotItem.Deserialize(itemSaveDataProxy.m_CookingPotItemSerialized);
        //        if (itemSaveDataProxy.m_ObjectGuidSerialized != null)
        //        {
        //            if (__instance.m_ObjectGuid == null)
        //                __instance.m_ObjectGuid = __instance.GetComponent<ObjectGuid>();
        //            if (__instance.m_ObjectGuid == null)
        //                __instance.m_ObjectGuid = __instance.gameObject.AddComponent<ObjectGuid>();
        //            ObjectGuidManager.UnRegisterGuid(itemSaveDataProxy.m_ObjectGuidSerialized);
        //            __instance.m_ObjectGuid.Set(itemSaveDataProxy.m_ObjectGuidSerialized);
        //        }
        //        if (itemSaveDataProxy.m_MissionObjectSerialized != null)
        //        {
        //            __instance.AssumeMissionObjectResponsibility(itemSaveDataProxy.m_MissionObjectSerialized);
        //            __instance.MaybeDestroyGearItemAfterMission();
        //        }
        //        if (__instance.m_ResearchItem && itemSaveDataProxy.m_ResearchItemSerialized != null)
        //            __instance.m_ResearchItem.Deserialize(itemSaveDataProxy.m_ResearchItemSerialized);
        //        if (__instance.m_StoneItem)
        //            __instance.m_StoneItem.SetThrown(itemSaveDataProxy.m_StoneItemThrown);
        //        if (itemSaveDataProxy.m_OwnershipOverrideSerialized != null)
        //        {
        //            __instance.m_OwnershipOverrideItem = __instance.gameObject.AddComponent<OwnershipOverride>();
        //            __instance.m_OwnershipOverrideItem.Deserialize(itemSaveDataProxy.m_OwnershipOverrideSerialized);
        //        }
        //        if (__instance.m_FoodWeight && (double)itemSaveDataProxy.m_WeightKG > 0.0)
        //        {
        //            __instance.m_WeightKG = itemSaveDataProxy.m_WeightKG;
        //            if (__instance.m_FoodItem)
        //                __instance.m_FoodItem.m_CaloriesTotal = __instance.m_WeightKG * __instance.m_FoodWeight.m_CaloriesPerKG;
        //        }
        //        if (__instance.m_Inspect && itemSaveDataProxy.m_InspectSerialized != null)
        //            __instance.m_Inspect.Deserialize(itemSaveDataProxy.m_InspectSerialized);
        //        if (__instance.m_PowderItem && itemSaveDataProxy.m_PowderItemSerialized != null)
        //            __instance.m_PowderItem.Deserialize(itemSaveDataProxy.m_PowderItemSerialized);
        //        if (__instance.m_Millable && itemSaveDataProxy.m_MillableSerialized != null)
        //            __instance.m_Millable.Deserialize(itemSaveDataProxy.m_MillableSerialized);
        //        if (__instance.m_SprayPaintCan && itemSaveDataProxy.m_SprayPaintCanSerialized != null)
        //            __instance.m_SprayPaintCan.Deserialize(itemSaveDataProxy.m_SprayPaintCanSerialized);
        //        MelonLogger.Msg(ConsoleColor.Yellow, "[GearItem][Deserialize] __instance.m_CurrentHP (Before TOD & DecayOverTODHours) " + __instance.m_CurrentHP);
        //        __instance.InitializeLastUpdatedTodHours();
        //        MelonLogger.Msg(ConsoleColor.DarkYellow, "[GearItem][Deserialize] __instance.m_CurrentHP (After TOD Init) " + __instance.m_CurrentHP);
        //        MelonLogger.Msg(ConsoleColor.DarkYellow, "[GearItem][Deserialize] __instance.m_DecayScalar " + __instance.m_DecayScalar);
        //        __instance.DecayOverTODHours(__instance.m_LastUpdatedTODHours - itemSaveDataProxy.m_HoursPlayed, __instance.m_DecayScalar);
        //        MelonLogger.Msg(ConsoleColor.DarkYellow, "[GearItem][Deserialize] __instance.m_CurrentHP (After DecayOverTODHours) " + __instance.m_CurrentHP);
        //        __instance.m_InitialDecayApplied = true;
        //        if (__instance.IsUndegradableAccelerant())
        //            __instance.m_CurrentHP = __instance.m_MaxHP;
        //        if (__instance.m_StackableItem == null && itemSaveDataProxy.m_StackableItemSerialized != null)
        //        {
        //            int unitsProxy = Utils.DeserializeObject<StackableItemSaveDataProxy>(itemSaveDataProxy.m_StackableItemSerialized).m_UnitsProxy;
        //            for (int index = 1; index < unitsProxy; ++index)
        //            {
        //                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.gameObject);
        //                DisableObjectForXPMode.RemoveDisabler(gameObject);
        //                gameObject.name = __instance.gameObject.name;
        //                if (__instance.m_InPlayerInventory)
        //                {
        //                    GameManager.GetInventoryComponent().AddGear(gameObject);
        //                    flag1 = false;
        //                }
        //                else if (__instance.IsInsideContainer())
        //                {
        //                    __instance.AddGearToContainer(__instance, (GearItem)gameObject.GetComponent<GearItem>());
        //                    flag1 = false;
        //                }
        //                else
        //                {
        //                    gameObject.transform.position = __instance.gameObject.transform.position;
        //                    gameObject.transform.rotation = __instance.gameObject.transform.rotation;
        //                }
        //            }
        //        }
        //        if (itemSaveDataProxy.m_GearItemSaveVersion < 1 && (__instance.name == "GEAR_Cloth" && __instance.m_StackableItem == null))
        //        {
        //            StackableItem stackableItem = __instance.m_StackableItem;
        //            stackableItem.m_Units = stackableItem.m_Units;
        //        }
        //        __instance.m_PlacePointGuid = itemSaveDataProxy.m_PlacePointGuidSerialized;
        //        __instance.m_PlacePointName = itemSaveDataProxy.m_PlacePointNameSerialized;
        //        __instance.m_HasBeenEquippedAndUsed = itemSaveDataProxy.m_HasBeenEquippedAndUsed;
        //        bool flag2 = __instance.MaybePlaceGear();
        //        if (flag1)
        //        {
        //            InaccessibleGearContainer.AddGearToNearestContainer(__instance);
        //        }
        //        else
        //        {
        //            applyPositioningFix &= __instance.gameObject.name != "GEAR_Tinder";
        //            if (applyPositioningFix && itemSaveDataProxy.m_GearItemSaveVersion < 4 && !flag2 && (__instance.m_BeenInPlayerInventory || GameManager.GetWeatherComponent().IsIndoorEnvironment()))
        //                __instance.MaybeAddToLostAndFound();
        //        }
        //        if (__instance.m_StartHasBeenCalled)
        //            return false;
        //        __instance.ManualStart();

        //        return false;
        //    }
        //}
    }
}