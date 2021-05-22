using System;
using UnityEngine;
using System.Reflection;
using System.Xml.XPath;
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

using GameServer;

namespace SkyCoop
{
    public class MyMod : MelonMod
    {

        public static bool IsZero(float a)
        {
            return Mathf.Abs(a) <= 0.0001f;
        }

        public static MyMod instance;
        public static int dataBufferSize = 4096;

        public string ip = "127.0.0.1";
        public int port = 26950;
        public int myId = 0;
        public TCP tcp;

        private delegate void PacketHandler(Packet _packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        [HarmonyPatch(typeof(GearItem), "Drop")]
        public class GearItemDrop
        {
            public static void Prefix(GearItem __instance)
            {
                MelonLogger.Log("Item dropped " + __instance.m_GearName);
            }
        }
        [HarmonyPatch(typeof(GameManager), "Awake")]
        public class GameManager_Awake
        {

            [HarmonyPatch(typeof(Panel_Inventory), "Update")]
            private static class Panel_Inventory_Open
            {
                internal static void Postfix(Panel_Inventory __instance)
                {
                    if (__instance == null)
                    {
                        return;
                    }

                    int seelecteditem = __instance.m_SelectedItemIndex;
                    seelecteditem = seelecteditem + __instance.m_FirstItemDisplayedIndex;
                    Il2CppSystem.Collections.Generic.List<GearItem> itemlist = __instance.m_FilteredInventoryList;

                    if (itemlist.ToArray().Length > 0) // If list in tab isn't empty.
                    {
                        GearItem gear = itemlist.ToArray().ElementAt(seelecteditem);
                        if (gear == null)
                        {
                            return;
                        }

                        string itemname = gear.m_GearName;

                        if (itemname.Contains("(Clone)"))
                        {
                            int L = itemname.Length - 7;
                            LastSelectedGearName = itemname.Remove(L, 7);
                        }
                        else
                        {
                            LastSelectedGearName = itemname;
                        }

                        if (LastSelectedGearName != "gg")
                        {
                            LastSelectedGear = gear;
                        }

                        if (NeedRefreshInv == true)
                        {
                            NeedRefreshInv = false;
                            __instance.m_IsDirty = true;
                            __instance.Update();
                        }
                    }
                    else
                    {
                        LastSelectedGearName = ""; // Nothing to drop.
                    }
                }
            }
            [HarmonyPatch(typeof(Panel_Rest), "Update")]
            private static class Panel_Rest_Update
            {
                internal static void Postfix(Panel_Rest __instance)
                {
                    //__instance.m_SleepButton.SetActive(false);

                    MyCycleSkip = __instance.m_SleepHours;
                }
            }
            [HarmonyPatch(typeof(Panel_Rest), "OnCancel")]
            private static class Panel_Rest_Close
            {
                internal static void Postfix(Panel_Rest __instance)
                {
                    MelonLogger.Log("Sleeping menu close.");
                }
            }
            [HarmonyPatch(typeof(Panel_Rest), "OnPickUp")]
            private static class Panel_Rest_Close2
            {
                internal static void Postfix(Panel_Rest __instance)
                {
                    MelonLogger.Log("Sleeping menu close.");
                }
            }
            [HarmonyPatch(typeof(Panel_Rest), "OnRest")]
            private static class Panel_Rest_Close3
            {
                internal static void Postfix(Panel_Rest __instance)
                {
                    MelonLogger.Log("Sleeping menu close.");
                }
            }
            [HarmonyPatch(typeof(Panel_Rest), "OnPassTime")]
            private static class Panel_Rest_Close4
            {
                internal static void Postfix(Panel_Rest __instance)
                {
                    MelonLogger.Log("Sleeping menu close.");
                }
            }
            [HarmonyPatch(typeof(Panel_Confirmation), "OnConfirm")]
            private static class Panel_Confirmation_Ok
            {
                internal static void Postfix(Panel_Confirmation __instance)
                {
                    if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INPUT SERVER ADDRESS")
                    {
                        string text = __instance.m_CurrentGroup.m_InputField.GetText();
                        DoConnectToIp(text);
                    }
                    if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INPUT GUID TO TELEPORT TO")
                    {
                        string text = __instance.m_CurrentGroup.m_InputField.GetText();
                        bool found = false;
                        for (int index = 0; index < BaseAiManager.m_BaseAis.Count; ++index)
                        {
                            GameObject animal = BaseAiManager.m_BaseAis[index].gameObject;

                            if (animal != null && animal.GetComponent<ObjectGuid>() != null)
                            {
                                if(animal.GetComponent<ObjectGuid>().Get() == text)
                                {
                                    found = true;
                                    GameManager.GetPlayerManagerComponent().TeleportPlayer(animal.transform.position, animal.transform.rotation);
                                    break;
                                }
                            }
                        }

                        if(found == false)
                        {
                            HUDMessage.AddMessage("Animal with GUID "+ text+" not exist!");
                        }
                    }
                    if (__instance.m_CurrentGroup != null && __instance.m_CurrentGroup.m_MessageLabel_InputFieldTitle.text == "INPUT GUID TO TRACK")
                    {
                        string text = __instance.m_CurrentGroup.m_InputField.GetText();
                        bool found = false;
                        for (int index = 0; index < BaseAiManager.m_BaseAis.Count; ++index)
                        {
                            GameObject animal = BaseAiManager.m_BaseAis[index].gameObject;

                            if (animal != null && animal.GetComponent<ObjectGuid>() != null)
                            {
                                if (animal.GetComponent<ObjectGuid>().Get() == text)
                                {
                                    found = true;
                                    DebugAnimalGUID = text;
                                    DebugAnimalGUIDLast = text;
                                    DebugLastAnimal = animal;
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
            [HarmonyPatch(typeof(Weather), "ChooseWeatherSetOfType")]
            private static class Weather_ChooseWeatherSetOfType
            {
                internal static void Postfix(Weather __instance, WeatherStage reqType, WeatherSet __result)
                {
                    string l = level_name;

                    if (__result == null || reqType == null || l == "Empty" || l == "Boot" || l == "MainMenu")
                    {
                        return;
                    }
                    Weather wea = GameManager.GetWeatherComponent();
                    for (int index = 0; index < wea.m_WeatherSetsForScene.Count; ++index)
                    {
                        WeatherSet ForweatherSet = wea.m_WeatherSetsForScene[index];
                        if (ForweatherSet == __result)
                        {
                            MelonLogger.Log("New Weather Stage with set [ID " + index + "]" + __result.name);
                            LastSelectedWeatherSet = index;
                            LastSelectedWeatherSet2 = __result;
                            break;
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(FirstPersonLightSource), "TurnOnEffects")]
        internal class FirstPersonLightSource_Start
        {

            public static void Prefix(FirstPersonLightSource __instance)
            {
                //MyLightSourceName = __instance.gameObject.name;
                MyLightSource = true;
            }
        }
        [HarmonyPatch(typeof(FirstPersonLightSource), "TurnOffEffects")]
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
                MyLightSource = false;
            }
        }

        [HarmonyPatch(typeof(PlayerManager), "OnCompletedDecalPlaceDown")]
        internal class PlayerManager_Start
        {

            public static void Prefix(PlayerManager __instance)
            {
                MelonLogger.Log("Placed Decal "+__instance.m_DecalToPlace.m_DecalName);
                MelonLogger.Log("X " + __instance.m_DecalToPlace.m_Pos.x + " Y "+ __instance.m_DecalToPlace.m_Pos.y + " Z " + __instance.m_DecalToPlace.m_Pos.z);
                if(InDarkWalkerMode == true && IamShatalker == false && __instance.m_DecalToPlace.m_DecalName == "NowhereToHide_Lure")
                {
                    WalkTracker WT = new WalkTracker();
                    WT.m_levelid = levelid;
                    WT.m_V3 = __instance.m_DecalToPlace.m_Pos;
                    if (sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.LUREPLACEMENT))
                        {
                            _packet.Write(WT);
                            SendTCPData(_packet);
                        }
                    }
                    if (iAmHost == true)
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.LUREPLACEMENT))
                        {
                            ServerSend.LUREPLACEMENT(1, WT);
                        }
                    }
                }
            }
        }

        public static Vector3 LastShootV3 = new Vector3(0, 0, 0);
        public static Quaternion LastShootQ = new Quaternion(0, 0, 0, 0);
        public static ShootSync PendingShoot = null;

        public class DestoryArrowOnHit : MonoBehaviour
        {
            public DestoryArrowOnHit(IntPtr ptr) : base(ptr) { }

            void Update()
            {

            }
        }

        public class DestoryStoneOnStop : MonoBehaviour
        {
            public DestoryStoneOnStop(IntPtr ptr) : base(ptr) { }
            public GameObject m_Obj = null;
            public Rigidbody m_RB = null;

            void Update()
            {
                if(m_Obj != null && m_RB != null)
                {
                    if(m_RB.isKinematic == true)
                    {
                        UnityEngine.Object.Destroy(m_Obj);
                    }
                }
            }
        }

        public class MultiplayerPlayer : MonoBehaviour
        {
            public MultiplayerPlayer(IntPtr ptr) : base(ptr) { }

            void Update()
            {

            }
        }

        public class ClientProjectile : MonoBehaviour
        {
            public ClientProjectile(IntPtr ptr) : base(ptr) { }


            void Update()
            {

            }
        }

        public class UiButtonPressHook : MonoBehaviour
        {
            public UiButtonPressHook(IntPtr ptr) : base(ptr) { }
            public int m_CustomId = 0;

            void Update()
            {

            }
        }

        public class ContainerOpenSync : MelonMod
        {
            public string m_Guid = "";
            public bool m_State = false;
        }

        [HarmonyPatch(typeof(ObjectAnim), "Play")]
        internal class ObjectAnim_Hack
        {
            public static void Postfix(ObjectAnim __instance, string name)
            {

                //MelonLogger.Log("ObjectAnim last played anim " + name);
                if(__instance.gameObject != null && __instance.gameObject.GetComponent<ContainersSync>() != null)
                {
                    if(__instance.gameObject.GetComponent<ContainersSync>().m_LastAnim != name)
                    {
                        __instance.gameObject.GetComponent<ContainersSync>().m_LastAnim = name;
                        __instance.gameObject.GetComponent<ContainersSync>().CallSync();
                    }
                }
            }
        }

        public class ContainersSync : MonoBehaviour
        {
            public ContainersSync(IntPtr ptr) : base(ptr) { }
            public GameObject m_Obj = null;
            public Container m_Cont = null;
            public string m_Guid = "";
            public string m_LastAnim = "";


            void Update()
            {
                if(m_Obj != null)
                {
                    if(m_Cont == null && m_Obj.GetComponent<Container>() != null)
                    {
                        m_Cont = m_Obj.GetComponent<Container>();
                    }
                    if (m_Guid == "" && m_Obj.GetComponent<ObjectGuid>() != null)
                    {
                        m_Guid = m_Obj.GetComponent<ObjectGuid>().Get();
                    }

                    if(m_Guid != "" && m_Cont != null)
                    {
                        if(m_Cont.m_IsCorpse == true)
                        {
                            UnityEngine.Object.Destroy(m_Obj.GetComponent<ContainersSync>());
                        }
                    }
                }
            }

            public void CallSync()
            {
                if (levelid == anotherplayer_levelid)
                {
                    ContainerOpenSync sync = new ContainerOpenSync();
                    sync.m_Guid = m_Guid;

                    if(m_LastAnim == "close")
                    {
                        sync.m_State = false;
                    }else{
                        sync.m_State = true;
                    }

                    if (iAmHost == true)
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.CONTAINEROPEN))
                        {
                            ServerSend.CONTAINEROPEN(1, sync);
                        }
                    }
                    if (sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.CONTAINEROPEN))
                        {
                            _packet.Write(sync);
                            SendTCPData(_packet);
                        }
                    }
                }
            }

            public void Open()
            {
                if (m_Cont != null && m_Cont.m_ObjectAnims != null && m_LastAnim != "open")
                {
                    if (m_Cont.m_PendingClose)
                        return;
                    for (int index = 0; index < m_Cont.m_ObjectAnims.Length; ++index)
                    {
                        ObjectAnim objectAnim = m_Cont.m_ObjectAnims[index];
                        if ((bool)(UnityEngine.Object)objectAnim && !objectAnim.Play("open"))
                            return;
                    }
                    m_Cont.PlayContainerOpenSound();
                }
            }
            public void Close()
            {
                if (m_Cont != null && m_Cont.m_ObjectAnims != null && m_LastAnim != "close")
                {
                    for (int index = 0; index < m_Cont.m_ObjectAnims.Length; ++index)
                    {
                        ObjectAnim objectAnim = m_Cont.m_ObjectAnims[index];
                        if ((bool)(UnityEngine.Object)objectAnim && !objectAnim.Play("close"))
                        {
                            m_Cont.m_PendingClose = true;
                            return;
                        }
                    }
                    m_Cont.PlayContainerCloseSound();
                    m_Cont.m_PendingClose = false;
                    return;
                }
            }
        }

        public void ApplyDamageZones(GameObject p)
        {
            if (p != null)
            {
                Transform root = p.transform.GetChild(6).GetChild(8);
                GameObject chest = root.GetChild(0).GetChild(0).GetChild(0).gameObject;
                GameObject arm_r1 = chest.transform.GetChild(0).GetChild(0).gameObject;
                GameObject arm_r2 = chest.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
                GameObject arm_l1 = chest.transform.GetChild(1).GetChild(0).gameObject;
                GameObject arm_l2 = chest.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
                GameObject head = chest.transform.GetChild(2).GetChild(0).gameObject;
                GameObject leg_r = root.GetChild(2).gameObject;
                GameObject leg_l = root.GetChild(1).gameObject;

                chest.AddComponent<PlayerBulletDamage>();
                arm_r1.AddComponent<PlayerBulletDamage>();
                arm_r2.AddComponent<PlayerBulletDamage>();
                arm_l1.AddComponent<PlayerBulletDamage>();
                arm_l2.AddComponent<PlayerBulletDamage>();
                head.AddComponent<PlayerBulletDamage>();
                leg_r.AddComponent<PlayerBulletDamage>();
                leg_l.AddComponent<PlayerBulletDamage>();

                chest.GetComponent<PlayerBulletDamage>().SetLocaZone(chest);
                arm_r1.GetComponent<PlayerBulletDamage>().SetLocaZone(arm_r1);
                arm_r2.GetComponent<PlayerBulletDamage>().SetLocaZone(arm_r2);
                arm_l1.GetComponent<PlayerBulletDamage>().SetLocaZone(arm_l1);
                arm_l2.GetComponent<PlayerBulletDamage>().SetLocaZone(arm_l2);
                head.GetComponent<PlayerBulletDamage>().SetLocaZone(head);
                leg_r.GetComponent<PlayerBulletDamage>().SetLocaZone(leg_r);
                leg_l.GetComponent<PlayerBulletDamage>().SetLocaZone(leg_l);
            }
        }

        public static void DamageByBullet(float damage)
        {
            GameManager.GetConditionComponent().AddHealth(-damage, DamageSource.Player);
            GameManager.GetBloodLossComponent().BloodLossStart("OtherPlayer", true, AfflictionOptions.PlayFX);
            GameManager.GetPlayerManagerComponent().ApplyDamageToWornClothing(damage);
            GameManager.GetPlayerVoiceComponent().Play("PLAY_PLAYERDAMAGE", Voice.Priority.Critical, PlayerVoice.Options.None);

            Transform V3 = GameManager.GetPlayerTransform();
            GameObject Player = GameManager.GetPlayerObject();

            GameAudioManager.SetMaterialSwitch("Flesh", Player);
            int num = (int)AkSoundEngine.PostEvent(AK.EVENTS.PLAY_BULLETIMPACTS, GameAudioManager.GetSoundEmitterFromGameObject(GameManager.GetPlayerObject()));
            GameAudioManager.SetAudioSourceTransform(Player, V3);
        }

        public static void DoColisionForArrows()
        {
            if(anotherbutt == null)
            {
                return;
            }
            Il2CppSystem.Collections.Generic.List<ArrowItem> ArrowsList = ArrowManager.m_ArrowItems;

            bool IsNeedToBeTrigger = true;

            for (int index = 0; index < ArrowsList.Count; ++index)
            {
                if (ArrowsList[index] != null && ArrowsList[index].InFlight(false))
                {
                    IsNeedToBeTrigger = false;
                    break;
                }
            }
            for (int index = 0; index < PlayerColiders.Count; ++index)
            {
                if (PlayerColiders[index] != null && PlayerColiders[index].GetComponent<BoxCollider>() != null)
                {
                    PlayerColiders[index].GetComponent<BoxCollider>().isTrigger = IsNeedToBeTrigger;
                }
            }
        }

        public class PlayerBulletDamage : MonoBehaviour
        {
            public PlayerBulletDamage(IntPtr ptr) : base(ptr) { }
            public int m_Damage = 0;
            public GameObject m_Obj = null;

            public void OnCollisionEnter(Collision col)
            {
                if (col.gameObject.GetComponent<ArrowItem>() != null)
                {
                    ArrowItem ARR = col.gameObject.GetComponent<ArrowItem>();
                    if (col.gameObject.GetComponent<DestoryArrowOnHit>() == null)
                    {
                        ARR.m_ArrowMesh.GetComponent<BoxCollider>().enabled = false;
                        MelonLogger.Log("Arrow colided other player, and dealing damage " + m_Damage);
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.BULLETDAMAGE))
                            {
                                _packet.Write((float)m_Damage);
                                SendTCPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.BULLETDAMAGE))
                            {
                                ServerSend.BULLETDAMAGE(1, (float)m_Damage);
                            }
                        }
                    }
                }
            }

            public void SetLocaZone(GameObject t)
            {
                m_Obj = t;
                m_Obj.tag = "Flesh";

                if (m_Obj.name == "Chest")
                {
                    m_Damage = 50;
                } else if (m_Obj.name.StartsWith("arm"))
                {
                    m_Damage = 30;
                } else if (m_Obj.name.StartsWith("Head"))
                {
                    m_Damage = 70;
                } else if (m_Obj.name.StartsWith("Thigh"))
                {
                    m_Damage = 20;
                }
                PlayerColiders.Add(m_Obj);
                //MelonLogger.Log(m_Obj.name + " = "+ m_Damage);
            }
        }

        public class ShootSync : MelonMod
        {
            public Vector3 m_position = new Vector3(0, 0, 0);
            public Quaternion m_rotation = new Quaternion(0, 0, 0, 0);
            public string m_projectilename = "";
            public float m_skill = 0;
            public Vector3 m_camera_forward = new Vector3(0, 0, 0);
            public Vector3 m_camera_right = new Vector3(0, 0, 0);
            public Vector3 m_camera_up = new Vector3(0, 0, 0);
        }

        [HarmonyPatch(typeof(BowItem), "ReleaseFire")]
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

                    LastShootV3 = playerTransform.TransformPoint(transform.position);
                    LastShootQ = playerTransform.rotation * transform.rotation;

                    MelonLogger.Log("[BowItem] Arrow Fire! " + __instance.m_GearArrow.gameObject.name);
                    if (levelid == anotherplayer_levelid)
                    {
                        ShootSync shoot = new ShootSync();
                        shoot.m_projectilename = "GEAR_Arrow";
                        shoot.m_position = playerTransform.TransformPoint(transform.position);
                        shoot.m_rotation = playerTransform.rotation * transform.rotation;

                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.SHOOTSYNC))
                            {
                                _packet.Write(shoot);
                                SendTCPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.SHOOTSYNC))
                            {
                                ServerSend.SHOOTSYNC(1, shoot);
                            }
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(ArrowItem), "HandleCollisionWithObject")]
        internal class ArrowItem_Shoot
        {
            public static void Postfix(ArrowItem __instance)
            {
                if (__instance.gameObject.GetComponent<DestoryArrowOnHit>() != null)
                {
                    UnityEngine.Object.Destroy(__instance.gameObject);
                }
            }
        }

        public static GameObject PistolBulletPrefab = null;
        public static GameObject RevolverBulletPrefab = null;

        [HarmonyPatch(typeof(vp_Bullet), "Start")]
        internal class vp_Bullet_Start
        {
            public static bool Prefix(vp_Bullet __instance)
            {
                if (__instance != null && __instance.gameObject != null)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("GEAR_Arrow"), new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                    GearItem componentArrow = gameObject.GetComponent<GearItem>();
                    componentArrow.m_ArrowItem.Fire();
                    UnityEngine.Object.Destroy(gameObject);

                    bool NeedPipSkills = false;
                    bool MyBullet = true;

                    if (__instance.gameObject.GetComponent<ClientProjectile>() != null)
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
                        PlayerBulletDamage PlayerDamage = hit.collider.gameObject.GetComponent<PlayerBulletDamage>();

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
                                    AnimalUpdates au = baseAiFromObject.gameObject.GetComponent<AnimalUpdates>();

                                    bool underMyControl = false;
                                    bool clientControl = false;

                                    if (au != null)
                                    {
                                        underMyControl = au.m_UnderMyControl;
                                        clientControl = au.m_ClientControlled;
                                    }

                                    if (MyBullet == true)
                                    {
                                        if ((AnimalsController == true && clientControl == false) || (AnimalsController == false && underMyControl == true))
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
                                            MelonLogger.Log("Got skill upgrade from your own shoot Rifle");
                                        }
                                        else if (__instance.m_GunType == GunType.Revolver)
                                        {
                                            GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Revolver, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                                            MelonLogger.Log("Got skill upgrade from your own shoot Revolver");
                                        }
                                    }
                                    else
                                    {
                                        if (MyBullet == false && ((AnimalsController == true && clientControl == false) || (AnimalsController == false && underMyControl == true)))
                                        {
                                            MelonLogger.Log("Remote shoot hit the animal, sending responce to client.");
                                            int SkillTypeId = 0;

                                            if (__instance.m_GunType == GunType.Rifle)
                                            {
                                                SkillTypeId = 1;
                                            }
                                            else if (__instance.m_GunType == GunType.Revolver)
                                            {
                                                SkillTypeId = 2;
                                            }

                                            if (sendMyPosition == true)
                                            {
                                                using (Packet _packet = new Packet((int)ClientPackets.PIMPSKILL))
                                                {
                                                    _packet.Write(SkillTypeId);
                                                    SendTCPData(_packet);
                                                }
                                            }
                                            if (iAmHost == true)
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
                        else if(PlayerDamage != null && MyBullet == true)
                        {
                            MelonLogger.Log("You damaged other player on " + PlayerDamage.m_Damage);
                            if (sendMyPosition == true)
                            {
                                using (Packet _packet = new Packet((int)ClientPackets.BULLETDAMAGE))
                                {
                                    _packet.Write((float)PlayerDamage.m_Damage);
                                    SendTCPData(_packet);
                                }
                            }
                            if (iAmHost == true)
                            {
                                using (Packet _packet = new Packet((int)ServerPackets.BULLETDAMAGE))
                                {
                                    ServerSend.BULLETDAMAGE(1, (float)PlayerDamage.m_Damage);
                                }
                            }
                        }
                        else{
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
                    else {
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

        [HarmonyPatch(typeof(vp_FPSShooter), "Start")]
        internal class vp_FPSShooter_Start
        {
            public static void Postfix(vp_FPSShooter __instance)
            {
                if (__instance != null && __instance.gameObject != null && __instance.ProjectilePrefab != null)
                {
                    if (__instance.gameObject.name == "Rifle" && __instance.ProjectilePrefab.name == "PistolBullet")
                    {
                        PistolBulletPrefab = __instance.ProjectilePrefab;
                    }
                    if (__instance.gameObject.name == "Revolver" && __instance.ProjectilePrefab.name == "RevolverBullet")
                    {
                        RevolverBulletPrefab = __instance.ProjectilePrefab;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(vp_FPSShooter), "Fire")]
        internal class vp_FPSShooter_End
        {

            public static void Prefix(vp_FPSShooter __instance)
            {
                if ((UnityEngine.Object)__instance.m_Weapon == (UnityEngine.Object)null || (double)Time.time < (double)__instance.m_NextAllowedFireTime || (__instance.m_Weapon.ReloadInProgress() || !GameManager.GetPlayerAnimationComponent().IsAllowedToFire(__instance.m_Weapon.m_GunItem.m_AllowHipFire)) || GameManager.GetPlayerAnimationComponent().IsReloading())
                {
                    //MelonLogger.Log("[vp_FPSShooter] Can't shoot now!");
                    return;
                }
                if (__instance.m_Weapon.GetAmmoCount() < 1)
                {
                    //MelonLogger.Log("[vp_FPSShooter] Dry fire!");
                    SendMultiplayerAudio("PLAY_RIFLE_DRY_3D");
                    return;
                }
                else {
                    if (__instance.m_Weapon.m_GunItem.m_IsJammed)
                    {
                        //MelonLogger.Log("[vp_FPSShooter] Jammed!");
                        SendMultiplayerAudio("PLAY_RIFLE_DRY_3D");
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
                            MelonLogger.Log("[vp_FPSShooter] Flaregun projectile spawn! " + __instance.ProjectilePrefab.name);
                        }
                        else
                        {
                            MelonLogger.Log("[vp_FPSShooter] Bullet projectile spawn " + __instance.ProjectilePrefab.name);
                        }

                        LastShootV3 = vector3;
                        LastShootQ = quaternion;

                        if (levelid == anotherplayer_levelid)
                        {
                            ShootSync shoot = new ShootSync();

                            shoot.m_projectilename = __instance.ProjectilePrefab.name;
                            shoot.m_position = vector3;
                            shoot.m_rotation = quaternion;

                            if (__instance.ProjectilePrefab.name == "PistolBullet")
                            {
                                shoot.m_skill = GameManager.GetSkillRifle().GetEffectiveRange();
                            }

                            if (sendMyPosition == true)
                            {
                                using (Packet _packet = new Packet((int)ClientPackets.SHOOTSYNC))
                                {
                                    _packet.Write(shoot);
                                    SendTCPData(_packet);
                                }
                            }
                            if (iAmHost == true)
                            {
                                using (Packet _packet = new Packet((int)ServerPackets.SHOOTSYNC))
                                {
                                    ServerSend.SHOOTSYNC(1, shoot);
                                }
                            }
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(GearItem), "ManualUpdate")]
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
        /*
        [HarmonyPatch(typeof(GameManager), "Awake")]
        private static class AddMedkitCraft
        {
            internal static void Postfix()
            {
                BlueprintItem blueprint = GameManager.GetBlueprints().AddComponent<BlueprintItem>();

                // Inputs
                blueprint.m_RequiredGear = new Il2CppReferenceArray<GearItem>(1) {
                    [0] = GetGearItemPrefab("GEAR_HeavyBandage"),
                    [1] = GetGearItemPrefab("GEAR_BottleHydrogenPeroxide"),
                    [2] = GetGearItemPrefab("GEAR_BottlePainKillers")
                };
                //blueprint.m_RequiredGearUnits = new Il2CppStructArray<int>(3) {
                //    [0] = 2,
                //    [1] = 1,
                //    [2] = 2
                //};
                blueprint.m_KeroseneLitersRequired = 0f;
                blueprint.m_GunpowderKGRequired = 0f;
                //blueprint.m_RequiredTool = null;
                //blueprint.m_OptionalTools = new Il2CppReferenceArray<ToolsItem>(1) { [0] = GetToolsItemPrefab(FISHING_TACKLE_NAME) };

                // Outputs
                blueprint.m_CraftedResult = GetGearItemPrefab("GEAR_MedicalSupplies_hangar");
                blueprint.m_CraftedResultCount = 1;

                // Process
                blueprint.m_Locked = false;
                blueprint.m_AppearsInStoryOnly = false;
                blueprint.m_RequiresLight = false;
                blueprint.m_RequiresLitFire = false;
                blueprint.m_RequiredCraftingLocation = CraftingLocation.Workbench;
                blueprint.m_DurationMinutes = 20;
                blueprint.m_CraftingAudio = "PLAY_CraftingCloth";
                blueprint.m_AppliedSkill = SkillType.None;
                blueprint.m_ImprovedSkill = SkillType.None;
            }

            private static GearItem GetGearItemPrefab(string name) => Resources.Load(name).Cast<GameObject>().GetComponent<GearItem>();
            private static ToolsItem GetToolsItemPrefab(string name) => Resources.Load(name).Cast<GameObject>().GetComponent<ToolsItem>();
        }
        [HarmonyPatch(typeof(Panel_Crafting), "ItemPassesFilter")]
        private static class ShowRecipesInMedsTab
        {
            internal static void Postfix(Panel_Crafting __instance, ref bool __result, BlueprintItem bpi)
            {
                if (bpi?.m_CraftedResult?.name == "GEAR_MedicalSupplies_hangar" && __instance.m_CurrentCategory == Panel_Crafting.Category.FirstAid)
                {
                    __result = true;
                }
            }
        }
        */
        [HarmonyPatch(typeof(BlueprintDisplayItem), "Setup")]
        private static class FixRecipeIcons
        {
            internal static void Postfix(BlueprintDisplayItem __instance, BlueprintItem bpi)
            {
                if (bpi?.m_CraftedResult?.name == "GEAR_MedicalSupplies_hangar")
                {
                    Texture2D medkitTexture = Utils.GetCachedTexture("ico_CraftItem__MedicalSupplies_hangar");
                    if (!medkitTexture)
                    {
                        medkitTexture = LoadedBundle.LoadAsset("ico_CraftItem__MedicalSupplies_hangar").Cast<Texture2D>();
                        Utils.CacheTexture("ico_CraftItem__MedicalSupplies_hangar", medkitTexture);
                    }
                    __instance.m_Icon.mTexture = medkitTexture;
                }
            }
        }
        [HarmonyPatch(typeof(Panel_Crafting), "ItemPassesFilter")]
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

        [HarmonyPatch(typeof(Container), "Start")]
        private static class Container_Hack
        {
            internal static void Postfix(Container __instance)
            {
                if (__instance != null && __instance.gameObject != null)
                {
                    __instance.gameObject.AddComponent<ContainersSync>();
                    __instance.gameObject.GetComponent<ContainersSync>().m_Obj = __instance.gameObject;
                }
            }
        }

        //public static void RecallculateAnimals()
        //{
        //    List<GameObject> AnimalsList = new List<GameObject>;

        //    Il2CppSystem.Collections.Generic.List<SpawnRegion> Regions = GameManager.GetSpawnRegionManager().m_SpawnRegions;
        //    for (int i = 0; i < Regions.Count; i++)
        //    {
        //        if (Regions[i] != null && Regions[i].enabled == true)
        //        {
        //            SpawnRegion region = Regions[i];
        //            Il2CppSystem.Collections.Generic.List<BaseAi> animals = region.m_Spawns;
        //            for (int i2 = 0; i2 < animals.Count; i2++)
        //            {
        //                if(animals[i2] != null && animals[i2].gameObject != null && animals[i2].gameObject.activeSelf == true)
        //                {
        //                    AnimalsList.Add(animals[i2].gameObject);
        //                }
        //            }
        //        }
        //    }
        //}

        public static void AllowSpawnAnimals(bool spawn)
        {
            Il2CppSystem.Collections.Generic.List<SpawnRegion> Regions = GameManager.GetSpawnRegionManager().m_SpawnRegions;

            for (int i = 0; i < Regions.Count; i++)
            {
                if (Regions[i] != null)
                {
                    SpawnRegion region = Regions[i];
                    region.SetActive(spawn);
                }
            }

            if (spawn == true)
            {
                MelonLogger.Log("[SpawnAnimals] " + Regions.Count + " Regions has been activated");
            }
            else {
                MelonLogger.Log("[SpawnAnimals] " + Regions.Count + " Regions has been deactivated");
            }
        }

        public static int IntToTrigger(int i, BaseAi _AI)
        {
            if (i == 1) { return _AI.m_AnimParameter_Attack_Trigger; }
            else if (i == 2) { return _AI.m_AnimParameter_DamageImpact_Trigger; }
            else if (i == 3) { return _AI.m_AnimParameter_Flinch_Trigger; }
            else if (i == 4) { return _AI.m_AnimParameter_Howl_Trigger; }
            else if (i == 5) { return _AI.m_AnimParameter_MooseStomp_Trigger; }
            else if (i == 6) { return _AI.m_AnimParameter_RandomId_Trigger; }
            else if (i == 7) { return _AI.m_AnimParameter_Roar_Trigger; }
            else if (i == 8) { return _AI.m_AnimParameter_Stunned_Trigger; }
            else if (i == 9) { return _AI.m_AnimParameter_Trigger_PassingAttack; }
            else if (i == 10) { return _AI.m_AnimParameter_Trigger_PassingAttackNpc; }
            else if (i == 11) { return _AI.m_AnimParameter_Trigger_Spear_Exit_Fail; }
            else if (i == 12) { return _AI.m_AnimParameter_Trigger_Spear_Exit_Success; }
            else if (i == 13) { return _AI.m_AnimParameter_Trigger_Spear_Exit_Success_Death; }
            else if (i == 14) { return _AI.m_AnimParameter_Trigger_Spear_Struggle_Entry; }
            else if (i == 15) { return _AI.m_AnimParameter_Trigger_Timberwolf_Attack; }
            else { return 0; }
        }

        public class AnimalTrigger : MonoBehaviour
        {
            public string m_Guid = "";
            public int m_Trigger = 0;
        }

        class AnimalUpdates : MonoBehaviour
        {
            public AnimalUpdates(IntPtr ptr) : base(ptr) { }
            public GameObject m_Animal = null;
            public int NoResponce = 5;
            public int ReTakeCoolDown = 5;
            public float nextActionTimeNR = 0.0f;
            public float noresponce_perioud = 1f;
            public float nextActionSync = 0.0f;
            public float actionSync_perioud = 0.3f;
            public float nextActionBloodDrop = 0.0f;
            public float blooddrop_period = 0.15f;
            public float nextActionDampingOn = 0.0f;
            public float dampingOn_perioud = 1.5f;
            public Vector3 m_ToGo = new Vector3(0, 0, 0);
            public Quaternion m_ToRotate = new Quaternion(0, 0, 0, 0);
            public int m_Anim = 0;
            public int m_AnimNext = 0;
            public GameObject m_TestModel = null;
            public bool m_RemoteSpawned = false;
            public string m_RightName = "";
            public bool m_DebugAnimal = false;
            public bool m_CanSync = false;
            public bool m_InActive = false;
            // Animation parameters
            public float AP_TurnAngle; //0
            public float AP_TurnSpeed; //1
            public float AP_Speed; //2
            public float AP_Wounded; //3
            public float AP_Roll; //4
            public float AP_Pitch; //5
            public float AP_TargetHeading; //6
            public float AP_TargetHeadingSmooth; //7
            public float AP_TapMeter; //8
            //AE_Trigger_Branchpoint; //9
            public int AP_AiState; //10
            //StruggleStart //11
            //StruggleEnd //12
            //DamageImpact //13
            public bool AP_Corpse; //14
            public bool AP_Dead; //15
            public int AP_DeadSide; //16
            //AE_IsInStruggle //17
            //public int AP_AE_NavigationState; //18
            public int AP_DamageBodyPart; //19
            //public int AP_AE_CorpseID; //20
            //ScriptedSequence_Hostile //21
            //ScriptedSequence_Feeding //22
            public int AP_AttackId; //23
            //Attack_Trigger //24
            public bool m_WritenMeshes = false;
            public SkinnedMeshRenderer m_Mesh1 = null;
            public SkinnedMeshRenderer m_Mesh2 = null;
            public SkinnedMeshRenderer m_Mesh3 = null;
            public float m_Hp = 100;
            public bool m_Bleeding = false;
            public bool m_UnderMyControl = false;
            public bool m_ClientControlled = false;
            public bool m_Banned = false;
            public bool m_DampingIgnore = true;
            public string m_PendingProxy = "";
            public bool m_WaitForAligment = false;

            void Start()
            {
                nextActionTimeNR = Time.time;
                nextActionSync = Time.time;
                nextActionBloodDrop = Time.time;
                nextActionDampingOn = Time.time+dampingOn_perioud;
            }
            public void CallSync()
            {
                if (m_Animal != null && m_Banned == false)
                {
                    if ((AnimalsController == true || m_UnderMyControl == true) && levelid == anotherplayer_levelid && m_WaitForAligment == false)
                    {
                        if(m_Animal.GetComponent<WildlifeItem>() != null && m_Animal.GetComponent<WildlifeItem>().m_PickedUp == true)
                        {
                            return;
                        }

                        AnimalSync sync = new AnimalSync();
                        sync.m_position = m_Animal.transform.position;
                        sync.m_rotation = m_Animal.transform.rotation;
                        sync.m_guid = m_Animal.GetComponent<ObjectGuid>().Get();

                        if (m_Animal.GetComponent<BaseAi>() != null && m_Animal.GetComponent<BaseAi>().m_Animator != null)
                        {

                            BaseAi _AI = m_Animal.GetComponent<BaseAi>();

                            Animator AN = _AI.m_Animator;

                            sync.AP_TurnAngle = AN.GetFloat(_AI.m_AnimParameter_TurnAngle);
                            sync.AP_TurnSpeed = AN.GetFloat(_AI.m_AnimParameter_TurnSpeed);
                            sync.AP_Speed = AN.GetFloat(_AI.m_AnimParameter_Speed);
                            sync.AP_Wounded = AN.GetFloat(_AI.m_AnimParameter_Wounded);
                            sync.AP_Roll = AN.GetFloat(_AI.m_AnimParameter_Roll);
                            sync.AP_Pitch = AN.GetFloat(_AI.m_AnimParameter_Pitch);
                            sync.AP_TargetHeading = AN.GetFloat(_AI.m_AnimParameter_TargetHeading);
                            sync.AP_TargetHeadingSmooth = AN.GetFloat(_AI.m_AnimParameter_TargetHeadingSmooth);
                            sync.AP_TapMeter = AN.GetFloat(_AI.m_AnimParameter_TapMeter);
                            sync.AP_AiState = AN.GetInteger(_AI.m_AnimParameter_AiState);
                            sync.AP_Corpse = AN.GetBool(_AI.m_AnimParameter_Corpse);
                            sync.AP_Dead = AN.GetBool(_AI.m_AnimParameter_Dead);
                            sync.AP_DeadSide = AN.GetInteger(_AI.m_AnimParameter_DamageSide);
                            sync.AP_DamageBodyPart = AN.GetInteger(_AI.m_AnimParameter_DamageBodyPart);
                            sync.AP_AttackId = AN.GetInteger(_AI.m_AnimParameter_AttackId);

                            bool ItWas = m_ClientControlled;
                            bool CantRetake = false;
                            float among = Vector3.Distance(GameManager.GetPlayerTransform().position, anotherbutt.transform.position);
                            int ReTakeChill = 5;

                            if(m_Animal.GetComponent<AnimalUpdates>() != null)
                            {
                                ReTakeChill = m_Animal.GetComponent<AnimalUpdates>().ReTakeCoolDown;
                            }

                            if(_AI.GetAiMode() == AiMode.Dead || _AI.GetAiMode() == AiMode.Struggle || _AI.m_CurrentHP <= 0 || _AI.m_HasEnteredStruggleOnLastAttack == true || AnimState == "Knock" || IsDead == true || among < 15 || ReTakeChill > 0)
                            {
                                CantRetake = true;
                            }

                            if (AnimalsController == true && CantRetake == false)
                            {
                                float iam = Vector3.Distance(GameManager.GetPlayerTransform().position, m_Animal.transform.position);
                                float other = Vector3.Distance(anotherbutt.transform.position, m_Animal.transform.position);

                                if (iam < other)
                                {
                                    sync.m_UnderYourControl = false;
                                }
                                else{
                                    sync.m_UnderYourControl = true;
                                }
                                m_ClientControlled = sync.m_UnderYourControl;
                                m_UnderMyControl = false;
                            }else{
                                sync.m_UnderYourControl = false;
                                m_ClientControlled = false;
                            }

                            if(m_ClientControlled != ItWas)
                            {
                                if(m_ClientControlled == true)
                                {
                                    sync.m_ProxySave = _AI.Serialize();
                                }else{
                                    //if(m_WaitForAligment != true)
                                    //{
                                    //    if (iAmHost == true)
                                    //    {
                                    //        using (Packet _packet = new Packet((int)ServerPackets.ASKFORANIMALPROXY))
                                    //        {
                                    //            ServerSend.ASKFORANIMALPROXY(1, sync.m_guid);
                                    //        }
                                    //    }

                                    //    if (sendMyPosition == true)
                                    //    {
                                    //        using (Packet _packet = new Packet((int)ClientPackets.ASKFORANIMALPROXY))
                                    //        {
                                    //            _packet.Write(sync.m_guid);
                                    //            SendTCPData(_packet);
                                    //        }
                                    //    }
                                    //    m_WaitForAligment = true;
                                    //    return;
                                    //}
                                }
                            }

                            if (_AI.GetAiMode() == AiMode.Dead)
                            {
                                sync.m_Hp = 0;
                                sync.m_Bleeding = false;
                            } else {
                                sync.m_Hp = _AI.m_CurrentHP;
                                sync.m_Bleeding = _AI.m_BleedingOut;
                            }

                            if(m_Animal.GetComponent<BodyHarvest>() != null)
                            {
                                BodyHarvest BH = m_Animal.GetComponent<BodyHarvest>();
                                sync.m_Meat = BH.m_MeatAvailableKG;
                                sync.m_Guts = BH.m_GutAvailableUnits;
                                sync.m_Hide = BH.m_HideAvailableUnits;
                                sync.m_Frozen = BH.m_PercentFrozen;
                            }
                        }
                        sync.m_name = GetAnimalPrefabName(m_Animal.name);

                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.ANIMALSYNC))
                            {
                                ServerSend.ANIMALSYNC(1, sync);
                            }
                        }

                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.ANIMALSYNC))
                            {
                                _packet.Write(sync);
                                SendTCPData(_packet);
                            }
                        }
                    }
                }
            }

            //void SetTrigger(int trigger)
            //{
            //    BaseAi _AI = m_Animal.GetComponent<BaseAi>();

            //    if (_AI != null)
            //    {
            //        Animator AN = _AI.m_Animator;
            //        AN.SetTrigger(IntToTrigger(trigger, _AI));
            //    }
            //}

            void Update()
            {
                if (m_Animal != null)
                {
                    if (AnimalsController == false)
                    {
                        if(m_UnderMyControl == false)
                        {
                            if (m_InActive == false)
                            {
                                m_InActive = true;
                                MakeAnimalActive(m_Animal, false);
                            }
                        }else{
                            if (m_InActive == true)
                            {
                                MelonLogger.Log("Re-creating animal under my control.");
                                m_InActive = false;
                                ReCreateAnimal(m_Animal, m_PendingProxy);
                                m_PendingProxy = "";
                            }
                        }
                    }else{
                        if(m_ClientControlled == true)
                        {
                            if (m_InActive == false)
                            {

                                MelonLogger.Log("Making controlled by client animal inactive");
                                m_InActive = true;
                                MakeAnimalActive(m_Animal, false);
                            }
                        }else{
                            if (m_InActive == true)
                            {
                                m_InActive = false;
                                ReCreateAnimal(m_Animal, m_PendingProxy);
                                m_PendingProxy = "";
                            }
                        }
                    }

                    if ((AnimalsController == false || m_ClientControlled == true))
                    {
                        if (m_WritenMeshes == false)
                        {
                            m_WritenMeshes = true;
                            WriteDownMesh(m_Animal);
                        }

                        if (m_Mesh1 != null) { if (m_Mesh1.isVisible == false) { m_Mesh1.enabled = true; m_Mesh1.forceRenderingOff = false; } }
                        if (m_Mesh2 != null) { if (m_Mesh2.isVisible == false) { m_Mesh2.enabled = true; m_Mesh2.forceRenderingOff = false; } }
                        if (m_Mesh3 != null) { if (m_Mesh3.isVisible == false) { m_Mesh3.enabled = true; m_Mesh3.forceRenderingOff = false; } }

                        BaseAi _AI = m_Animal.GetComponent<BaseAi>();

                        if (m_UnderMyControl == false)
                        {
                            if(m_DampingIgnore == true)
                            {
                                m_Animal.transform.position = m_ToGo;
                                m_Animal.transform.rotation = m_ToRotate;
                            }else{
                                m_Animal.transform.position = Vector3.Lerp(m_Animal.transform.position, m_ToGo, Time.deltaTime * DeltaAnimalsMultiplayer);
                                m_Animal.transform.rotation = Quaternion.Lerp(m_Animal.transform.rotation, m_ToRotate, Time.deltaTime * DeltaAnimalsMultiplayer);
                            }

                            if (_AI != null)
                            {
                                Animator AN = _AI.m_Animator;

                                AN.SetFloat(_AI.m_AnimParameter_TurnAngle, AP_TurnAngle);
                                AN.SetFloat(_AI.m_AnimParameter_TurnSpeed, AP_TurnSpeed);
                                AN.SetFloat(_AI.m_AnimParameter_Speed, AP_Speed);
                                AN.SetFloat(_AI.m_AnimParameter_Wounded, AP_Wounded);
                                AN.SetFloat(_AI.m_AnimParameter_Roll, AP_Roll);
                                AN.SetFloat(_AI.m_AnimParameter_Pitch, AP_Pitch);
                                AN.SetFloat(_AI.m_AnimParameter_TargetHeading, AP_TargetHeading);
                                AN.SetFloat(_AI.m_AnimParameter_TargetHeadingSmooth, AP_TargetHeadingSmooth);
                                AN.SetFloat(_AI.m_AnimParameter_TapMeter, AP_TapMeter);
                                AN.SetInteger(_AI.m_AnimParameter_AiState, AP_AiState);
                                AN.SetBool(_AI.m_AnimParameter_Corpse, AP_Corpse);
                                AN.SetBool(_AI.m_AnimParameter_Dead, AP_Dead);
                                AN.SetInteger(_AI.m_AnimParameter_DamageSide, AP_DeadSide);
                                AN.SetInteger(_AI.m_AnimParameter_DamageBodyPart, AP_DamageBodyPart);
                                AN.SetInteger(_AI.m_AnimParameter_AttackId, AP_AttackId);
                            }
                        }

                        _AI.m_CurrentHP = m_Hp;

                        if (m_Hp <= 0)
                        {
                            _AI.SetAiMode(AiMode.Dead);
                        }
                    }

                    if (m_Animal.GetComponent<BodyHarvest>() != null)
                    {
                        if (m_Animal.GetComponent<BaseAi>().GetAiMode() == AiMode.Dead || m_Animal.GetComponent<BaseAi>().m_CurrentHP == 0)
                        {
                            BodyHarvest BH = m_Animal.GetComponent<BodyHarvest>();

                            if (BH.enabled == true)
                            {
                                if (m_Animal.GetComponent<ObjectGuid>().Get() == OtherHarvetingAnimal)
                                {
                                    BH.enabled = false;
                                }
                            }
                            else
                            {
                                if (m_Animal.GetComponent<ObjectGuid>().Get() != OtherHarvetingAnimal)
                                {
                                    BH.enabled = true;
                                }
                            }
                        }
                    }

                    bool animalListed = false;
                    bool animalListedGLOBAL = false;
                    int animalListedIndex = 0;

                    float dis = MaxAniamlsSyncDistance;
                    if (anotherbutt != null)
                    {
                        dis = Vector3.Distance(anotherbutt.transform.position, m_Animal.transform.position);
                    }
                    for (int i = 0; i < AllAnimalsNew.Count; i++)
                    {
                        if (AllAnimalsNew.ElementAt(i) == m_Animal)
                        {
                            animalListedIndex = i;
                            animalListed = true;
                            break;
                        }
                    }

                    for (int i = 0; i < AllAnimals.Count; i++)
                    {
                        if (AllAnimals.ElementAt(i) == m_Animal)
                        {
                            animalListedGLOBAL = true;
                            break;
                        }
                    }

                    if (AnimalsController == true || m_UnderMyControl == true)
                    {
                        if (animalListed == false)
                        {
                            if (dis < MaxAniamlsSyncDistance && AllAnimalsNew.Count < MaxAnimalsSyncNeed)
                            {
                                AllAnimalsNew.Add(m_Animal);
                                m_CanSync = true;
                            } else {
                                m_CanSync = false;
                            }
                        } else {
                            if (dis >= MaxAniamlsSyncDistance)
                            {
                                AllAnimalsNew.RemoveAt(animalListedIndex);
                                m_CanSync = false;
                            } else {
                                m_CanSync = true;
                            }
                        }
                    }else{
                        m_CanSync = false;
                    }

                    if (animalListedGLOBAL == false)
                    {
                        AllAnimals.Add(m_Animal);
                    }
                    if (Time.time > nextActionTimeNR)
                    {
                        nextActionTimeNR += noresponce_perioud;

                        if(AnimalsController == true || m_UnderMyControl == true)
                        {
                            if(ReTakeCoolDown > 0)
                            {
                                ReTakeCoolDown = ReTakeCoolDown-1;
                            }
                        }
                        if (AnimalsController == false && m_UnderMyControl == false)
                        {
                            NoResponce = NoResponce - 1;
                            if (NoResponce <= 0)
                            {
                                MelonLogger.Log("Found animal that we not need anymore " + m_Animal.GetComponent<ObjectGuid>().Get());

                                if (m_Animal.GetComponent<ObjectGuid>() != null && m_Animal.GetComponent<ObjectGuid>().Get() == HarvestingAnimal)
                                {
                                    ExitHarvesting();
                                    if(m_Animal.GetComponent<BodyHarvest>() != null)
                                    {
                                        m_Animal.GetComponent<BodyHarvest>().enabled = false;
                                    }
                                }
                                UnityEngine.Object.Destroy(m_Animal);
                            }
                        }
                    }
                    if(Time.time > nextActionDampingOn)
                    {
                        if(m_DampingIgnore == true)
                        {
                            m_DampingIgnore = false;
                        }
                    }
                    if (Time.time > nextActionSync)
                    {
                        nextActionSync += actionSync_perioud;
                        if (levelid > 3 && (AnimalsController == true || m_UnderMyControl == true) && IsShatalkerMode() == false && m_CanSync == true)
                        {
                            CallSync();
                        }
                    }
                    if (Time.time > nextActionBloodDrop)
                    {
                        nextActionBloodDrop += blooddrop_period;

                        if (levelid > 3 && AnimalsController == false && m_Bleeding == true)
                        {
                            BaseAi _AI = m_Animal.GetComponent<BaseAi>();
                            if (_AI.m_BloodTrail != null)
                            {
                                BloodTrail _Blood = _AI.m_BloodTrail;

                                Vector3 pos = m_Animal.transform.position;
                                ++pos.y;
                                Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                                insideUnitCircle.Normalize();
                                Vector2 vector2 = insideUnitCircle * UnityEngine.Random.Range(0.0f, 0.75f);
                                pos.x += vector2.x;
                                pos.z += vector2.y;
                                pos -= m_Animal.transform.forward * 0.5f;
                                RaycastHit hitInfo;
                                if (!Physics.Raycast(pos, Vector3.down, out hitInfo, float.PositiveInfinity, Utils.m_PhysicalCollisionLayerMask) || (UnityEngine.Object)hitInfo.collider == (UnityEngine.Object)null)
                                    return;
                                Vector3 scale = _Blood.m_DecalProjectorScale * UnityEngine.Random.Range(0.5f, 2f);
                                int uvRectangleIndex = _Blood.m_UvRectangleIndexBloodSmall;
                                if (Utils.RollChance(50f))
                                    uvRectangleIndex = _Blood.m_UvRectangleIndexBloodLarge;
                                GameManager.GetDynamicDecalsManager().CreateDecal(hitInfo.point, _Blood.transform.rotation.eulerAngles.y, hitInfo.normal, uvRectangleIndex, scale, _Blood.GetDecalProjectorType(), GameManager.GetWeatherComponent().IsIndoorEnvironment());
                            }
                        }
                    }
                }
            }
        }

        public static AiMode IntToAiMode(int i)
        {
            if (i == 0) { return AiMode.None; }
            if (i == 1) { return AiMode.Attack; }
            if (i == 2) { return AiMode.Dead; }
            if (i == 3) { return AiMode.Feeding; }
            if (i == 4) { return AiMode.Flee; }
            if (i == 5) { return AiMode.FollowWaypoints; }
            if (i == 6) { return AiMode.HoldGround; }
            if (i == 7) { return AiMode.Idle; }
            if (i == 8) { return AiMode.Investigate; }
            if (i == 9) { return AiMode.InvestigateFood; }
            if (i == 10) { return AiMode.InvestigateSmell; }
            if (i == 11) { return AiMode.Rooted; }
            if (i == 12) { return AiMode.Sleep; }
            if (i == 13) { return AiMode.Stalking; }
            if (i == 14) { return AiMode.Struggle; }
            if (i == 15) { return AiMode.Wander; }
            if (i == 16) { return AiMode.WanderPaused; }
            if (i == 17) { return AiMode.GoToPoint; }
            if (i == 18) { return AiMode.InteractWithProp; }
            if (i == 19) { return AiMode.ScriptedSequence; }
            if (i == 20) { return AiMode.Stunned; }
            if (i == 21) { return AiMode.ScratchingAntlers; }
            if (i == 22) { return AiMode.PatrolPointsOfInterest; }
            if (i == 23) { return AiMode.HideAndSeek; }
            if (i == 24) { return AiMode.JoinPack; }
            if (i == 25) { return AiMode.PassingAttack; }
            if (i == 26) { return AiMode.Howl; }
            if (i == 27) { return AiMode.Disabled; }

            return AiMode.None;
        }

        public static int AiModeToInt(AiMode i)
        {
            if (i == AiMode.None) { return 0; }
            if (i == AiMode.Attack) { return 1; }
            if (i == AiMode.Dead) { return 2; }
            if (i == AiMode.Feeding) { return 3; }
            if (i == AiMode.Flee) { return 4; }
            if (i == AiMode.FollowWaypoints) { return 5; }
            if (i == AiMode.HoldGround) { return 6; }
            if (i == AiMode.Idle) { return 7; }
            if (i == AiMode.Investigate) { return 8; }
            if (i == AiMode.InvestigateFood) { return 9; }
            if (i == AiMode.InvestigateSmell) { return 10; }
            if (i == AiMode.Rooted) { return 11; }
            if (i == AiMode.Sleep) { return 12; }
            if (i == AiMode.Stalking) { return 13; }
            if (i == AiMode.Struggle) { return 14; }
            if (i == AiMode.Wander) { return 15; }
            if (i == AiMode.WanderPaused) { return 16; }
            if (i == AiMode.GoToPoint) { return 17; }
            if (i == AiMode.InteractWithProp) { return 18; }
            if (i == AiMode.ScriptedSequence) { return 19; }
            if (i == AiMode.Stunned) { return 20; }
            if (i == AiMode.ScratchingAntlers) { return 21; }
            if (i == AiMode.PatrolPointsOfInterest) { return 22; }
            if (i == AiMode.HideAndSeek) { return 23; }
            if (i == AiMode.JoinPack) { return 24; }
            if (i == AiMode.PassingAttack) { return 25; }
            if (i == AiMode.Howl) { return 26; }
            if (i == AiMode.Disabled) { return 27; }

            return 0;
        }

        public class HarvestStats : MelonMod
        {
            public float m_Meat = 0;
            public int m_Guts = 0;
            public int m_Hide = 0;
        }

        public class AnimalSync : MelonMod
        {
            public Vector3 m_position = new Vector3(0, 0, 0);
            public Quaternion m_rotation = new Quaternion(0, 0, 0, 0);
            public string m_guid = "";
            public string m_name = "";
            public float m_Hp = 100;
            public bool m_Bleeding = false;

            public float AP_TurnAngle; //0
            public float AP_TurnSpeed; //1
            public float AP_Speed; //2
            public float AP_Wounded; //3
            public float AP_Roll; //4
            public float AP_Pitch; //5
            public float AP_TargetHeading; //6
            public float AP_TargetHeadingSmooth; //7
            public float AP_TapMeter; //8
            //AE_Trigger_Branchpoint; //9
            public int AP_AiState; //10
            //StruggleStart //11
            //StruggleEnd //12
            //DamageImpact //13
            public bool AP_Corpse; //14
            public bool AP_Dead; //15
            public int AP_DeadSide; //16
            //AE_IsInStruggle //17
            //public int AP_AE_NavigationState; //18
            public int AP_DamageBodyPart; //19
            //public int AP_AE_CorpseID; //20
            //ScriptedSequence_Hostile //21
            //ScriptedSequence_Feeding //22
            public int AP_AttackId; //23
            //Attack_Trigger //24

            public float m_Meat = 0;
            public int m_Guts = 0;
            public int m_Hide = 0;
            public float m_Frozen = 0;
            public bool m_UnderYourControl = false;
            public string m_ProxySave = "";
        }

        [HarmonyPatch(typeof(BaseAi), "AnimSetTrigger")]
        private static class GetID
        {
            internal static void Postfix(BaseAi __instance, int id)
            {
                if (__instance != null && __instance.gameObject != null && __instance.gameObject.GetComponent<ObjectGuid>() != null && __instance.gameObject.activeSelf == true)
                {
                    if (iAmHost == true && AnimalsController == true && levelid == anotherplayer_levelid)
                    {
                        GameObject animal = __instance.gameObject;
                        MelonLogger.Log("Animal with GUID " + animal.GetComponent<ObjectGuid>().Get() + " used trigger with hash name " + id);

                        AnimalTrigger trigg = new AnimalTrigger();

                        trigg.m_Guid = animal.GetComponent<ObjectGuid>().Get();
                        trigg.m_Trigger = id;

                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.ANIMALSYNCTRIGG))
                            {
                                ServerSend.ANIMALSYNCTRIGG(1, trigg);
                            }
                        }

                        if (sendMyPosition == true)
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

        [HarmonyPatch(typeof(BaseAi), "Update")]
        private static class AI_Hack
        {
            internal static void Postfix(BaseAi __instance)
            {
                if (__instance != null && __instance.gameObject != null && __instance.gameObject.GetComponent<ObjectGuid>() != null && __instance.gameObject.activeSelf == true)
                {
                    GameObject animal = __instance.gameObject;
                    string _guid = "";

                    if(NoRabbits == true)
                    {
                        if (animal.name.Contains("Rabbit") && animal.activeSelf == true)
                        {
                            animal.SetActive(false);
                        }
                    }

                    if (animal != null)
                    {
                        AnimalUpdates au = animal.GetComponent<AnimalUpdates>();
                        if (animal.GetComponent<ObjectGuid>() != null)
                        {
                            _guid = animal.GetComponent<ObjectGuid>().Get();
                        }
                        if (au == null)
                        {
                            //MelonLogger.Log("Added AnimalUpdates");
                            animal.AddComponent<AnimalUpdates>();
                            au = animal.GetComponent<AnimalUpdates>();
                            au.m_Animal = __instance.gameObject;
                            //animal.name = animal.name + "_MULTIPLAYER_" + _guid;
                        }
                    }
                }
            }
        }

        public static void ExitHarvesting()
        {
            Panel_BodyHarvest PBH = InterfaceManager.m_Panel_BodyHarvest;

            if (PBH.m_CurrentHarvestAction != Panel_BodyHarvest.HarvestAction.None)
            {
                PBH.OnCancel();
            }else{
                PBH.OnBack();
            }
        }

        public static void ExitHarvestingCareful()
        {
            Panel_BodyHarvest PBH = InterfaceManager.m_Panel_BodyHarvest;
            PBH.InterruptHarvest();
            PBH.CleanUpOnExit();
            if (PBH.m_PlayBookEndAnimation)
            {
                GameManager.GetPlayerAnimationComponent().Trigger_AnimatedInteraction("Trigger_HarvestDeer_End", true, (PlayerAnimation.OnAnimationEvent)null, PlayerAnimation.AnimationLayerFlags.Hip);
            }

            PBH.m_ProgressBar_Harvest.value = 0.0f;
            PBH.ResetErrorMessage();

            GameManager.GetVpFPSCamera().m_PanViewCamera.ReattachToPlayer();
            //PBH.DisableProgressBar();
            //GameManager.GetPlayerManagerComponent().ItemInHandsDuringInteractionEnd();

            PBH.enabled = false;

            //Input.ResetInputAxes();
        }


        [HarmonyPatch(typeof(Panel_BodyHarvest), "Update")]
        private static class Panel_BodyHarvest_Hack_Update
        {
            internal static void Postfix(Panel_BodyHarvest __instance)
            {
                if (__instance != null && __instance.m_BodyHarvest != null && __instance.m_BodyHarvest.gameObject != null && __instance.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    GameObject shokal = __instance.m_BodyHarvest.gameObject;

                    if (shokal.GetComponent<ObjectGuid>().Get() == OtherHarvetingAnimal)
                    {
                        ExitHarvesting();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(BodyHarvest), "UpdateBodyHarvest")]
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

        [HarmonyPatch(typeof(Panel_BodyHarvest), "HarvestSuccessful")]
        private static class Panel_BodyHarvest_Hack
        {
            internal static void Prefix(Panel_BodyHarvest __instance)
            {
                if (__instance != null && __instance.m_BodyHarvest != null && __instance.m_BodyHarvest.gameObject != null && __instance.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    MelonLogger.Log("Harvested meant "+ __instance.m_MenuItem_Meat.m_HarvestAmount);
                    MelonLogger.Log("Harvested guts " + __instance.m_MenuItem_Gut.m_HarvestAmount);
                    MelonLogger.Log("Harvested hide " + __instance.m_MenuItem_Hide.m_HarvestAmount);

                    HarvestStats Harvey = new HarvestStats();
                    Harvey.m_Meat = __instance.m_MenuItem_Meat.m_HarvestAmount;
                    Harvey.m_Guts = (int)__instance.m_MenuItem_Gut.m_HarvestAmount;
                    Harvey.m_Hide = (int)__instance.m_MenuItem_Hide.m_HarvestAmount;

                    if(AnimalsController == false)
                    {
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.DONEHARVASTING))
                            {
                                _packet.Write(Harvey);
                                SendTCPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.DONEHARVASTING))
                            {
                                ServerSend.DONEHARVASTING(1, Harvey);
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(BaseAi), "ApplyDamage", new System.Type[] { typeof(float), typeof(float), typeof(DamageSource), typeof(string) })]
        private static class AI_Hack_Damage
        {
            internal static bool Prefix(BaseAi __instance)
            {
                AnimalUpdates au = __instance.gameObject.GetComponent<AnimalUpdates>();

                bool underMyControl = false;
                bool clientControl = false;

                if(au != null)
                {
                    underMyControl = au.m_UnderMyControl;
                    clientControl = au.m_ClientControlled;
                }

                if ((AnimalsController == true && clientControl == false) || (AnimalsController == false && underMyControl == true))
                {
                    return true;
                }else{
                    return false;
                }
            }
        }

        [HarmonyPatch(typeof(ArrowItem), "InflictDamage", new System.Type[] { typeof(GameObject), typeof(float), typeof(bool), typeof(string), typeof(Vector3) })]
        private static class ArrowItem_DamageFix
        {
            internal static bool Prefix(ArrowItem __instance)
            {
                return false;
            }
            internal static BaseAi Prefix(ArrowItem __instance, GameObject victim, float damageScalar, bool stickIn, string collider, Vector3 collisionPoint)
            {          
                BaseAi baseAi = (BaseAi)null;
                if (victim.layer == 16)
                    baseAi = victim.GetComponent<BaseAi>();
                else if (victim.layer == 27)
                    baseAi = victim.transform.GetComponentInParent<BaseAi>();
                if ((UnityEngine.Object)baseAi == (UnityEngine.Object)null)
                    return (BaseAi)null;
                LocalizedDamage component = victim.GetComponent<LocalizedDamage>();
                double num = (double)StatsManager.IncrementValue(StatID.SuccessfulHits_Bow);
                float bleedOutMinutes = component.GetBleedOutMinutes(BodyDamage.Weapon.Arrow);
                float damage = __instance.m_VictimDamage * damageScalar * component.GetDamageScale(BodyDamage.Weapon.Arrow);
                if (!baseAi.m_IgnoreCriticalHits && component.RollChanceToKill(BodyDamage.Weapon.Arrow))
                    damage = float.PositiveInfinity;
                if (baseAi.GetAiMode() != AiMode.Dead)

                    if (__instance.gameObject != null && __instance.gameObject.GetComponent<DestoryArrowOnHit>() == null)
                    {
                        MelonLogger.Log("I am hit target with bow");
                        GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Archery, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                    }
                    else
                    {
                        MelonLogger.Log("Other player hit targer with bow");
                    }
                baseAi.SetupDamageForAnim(collisionPoint, GameManager.GetPlayerTransform().position, component);
                baseAi.ApplyDamage(damage, !stickIn ? 0.0f : bleedOutMinutes, DamageSource.Player, collider);
                return baseAi;
            }
        }

        private static AssetBundle LoadedBundle = null;

        private static GearItem GetGearItemPrefab(string name) => Resources.Load(name).Cast<GameObject>().GetComponent<GearItem>();
        private static GameObject GetGearItemObject(string name) => Resources.Load(name).Cast<GameObject>();

        //Part of code for working with Outlines.cs by Chris Nolet
        public static List<Vector3> SmoothNormals(Mesh mesh)
        {

            // Group vertices by location
            var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

            // Copy normals to a new list
            var smoothNormals = new List<Vector3>(mesh.normals);

            // Average normals for grouped vertices
            foreach (var group in groups)
            {

                // Skip single vertices
                if (group.Count() == 1)
                {
                    continue;
                }

                // Calculate the average normal
                var smoothNormal = Vector3.zero;

                foreach (var pair in group)
                {
                    smoothNormal += mesh.normals[pair.Value];
                }

                smoothNormal.Normalize();

                // Assign smooth normal to each vertex
                foreach (var pair in group)
                {
                    smoothNormals[pair.Value] = smoothNormal;
                }
            }

            return smoothNormals;
        }

        public class Outline : MonoBehaviour
        {
            public Outline(IntPtr ptr) : base(ptr) { }
            //
            //  Outline.cs
            //  QuickOutline
            //
            //  Created by Chris Nolet on 3/30/18.
            //  Copyright © 2018 Chris Nolet. All rights reserved.
            //
            private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();
            private class ListVector3
            {
                public List<Vector3> data;
            }
            private bool precomputeOutline;
            private List<Mesh> bakeKeys = new List<Mesh>();
            private List<ListVector3> bakeValues = new List<ListVector3>();

            private Renderer[] renderers;
            public Material outlineMaskMaterial;
            public Material outlineFillMaterial;

            //On update this values need set
            //needsUpdate = true;
            public int m_RenderMode = 3;
            public Color m_OutlineColor = Color.white;
            public float m_OutlineWidth = 2f;

            //OutlineAll, 0
            //OutlineVisible, 1
            //OutlineHidden, 2
            //OutlineAndSilhouette, 3
            //SilhouetteOnly 4

            public bool needsUpdate;

            void Awake()
            {

                // Cache renderers
                renderers = GetComponentsInChildren<Renderer>();

                // Instantiate outline materials
                Material _Mask = LoadedBundle.LoadAsset<Material>("OutlineMask");
                Material _Fill = LoadedBundle.LoadAsset<Material>("OutlineFill");

                outlineMaskMaterial = Instantiate(_Mask);
                outlineFillMaterial = Instantiate(_Fill);

                outlineMaskMaterial.name = "OutlineMask (Instance)";
                outlineFillMaterial.name = "OutlineFill (Instance)";

                // Retrieve or generate smooth normals
                LoadSmoothNormals();

                // Apply material properties immediately
                needsUpdate = true;
            }

            void OnEnable()
            {
                foreach (var renderer in renderers)
                {
                    if(renderer == null)
                    {
                        return;
                    }

                    // Append outline shaders
                    var materials = renderer.sharedMaterials.ToList();

                    materials.Add(outlineMaskMaterial);
                    materials.Add(outlineFillMaterial);

                    renderer.materials = materials.ToArray();
                }
            }

            void OnValidate()
            {

                // Update material properties
                needsUpdate = true;

                // Clear cache when baking is disabled or corrupted
                if (!precomputeOutline && bakeKeys.Count != 0 || bakeKeys.Count != bakeValues.Count)
                {
                    bakeKeys.Clear();
                    bakeValues.Clear();
                }

                // Generate smooth normals when baking is enabled
                if (precomputeOutline && bakeKeys.Count == 0)
                {
                    Bake();
                }
            }

            public void Update()
            {
                if (needsUpdate)
                {
                    needsUpdate = false;

                    UpdateMaterialProperties();
                }
            }

            public void OnDisable()
            {
                foreach (var renderer in renderers)
                {
                    if (renderer == null)
                    {
                        return;
                    }
                    // Remove outline shaders
                    var materials = renderer.sharedMaterials.ToList();

                    materials.Remove(outlineMaskMaterial);
                    materials.Remove(outlineFillMaterial);

                    renderer.materials = materials.ToArray();
                }
            }

            public void OnDestroy()
            {
                // Destroy material instances
                UnityEngine.Object.Destroy(outlineMaskMaterial);
                UnityEngine.Object.Destroy(outlineFillMaterial);
            }

            void Bake()
            {

                // Generate smooth normals for each mesh
                var bakedMeshes = new HashSet<Mesh>();

                foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
                {

                    // Skip duplicates
                    if (!bakedMeshes.Add(meshFilter.sharedMesh))
                    {
                        continue;
                    }

                    // Serialize smooth normals
                    var smoothNormals = MyMod.SmoothNormals(meshFilter.sharedMesh);

                    bakeKeys.Add(meshFilter.sharedMesh);
                    bakeValues.Add(new ListVector3() { data = smoothNormals });
                }
            }

            void LoadSmoothNormals()
            {

                // Retrieve or generate smooth normals
                foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
                {

                    // Skip if smooth normals have already been adopted
                    if (!registeredMeshes.Add(meshFilter.sharedMesh))
                    {
                        continue;
                    }

                    // Retrieve or generate smooth normals
                    var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
                    var smoothNormals = (index >= 0) ? bakeValues[index].data : MyMod.SmoothNormals(meshFilter.sharedMesh);

                    Il2CppSystem.Collections.Generic.List<Vector2> Converted = new Il2CppSystem.Collections.Generic.List<Vector2>();

                    for (int i = 0; i < smoothNormals.Count; i++)
                    {
                        Vector2 writeConverted = new Vector2(0, 0);

                        writeConverted = smoothNormals[i];

                        Converted.Add(writeConverted);
                    }

                    // Store smooth normals in UV3
                    meshFilter.sharedMesh.SetUVs(3, Converted);
                }

                // Clear UV3 on skinned mesh renderers
                foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    if (registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
                    {
                        skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];
                    }
                }
            }
            void UpdateMaterialProperties()
            {

                // Apply properties according to mode
                outlineFillMaterial.SetColor("_OutlineColor", m_OutlineColor);

                //OutlineAll, 0
                //OutlineVisible, 1
                //OutlineHidden, 2
                //OutlineAndSilhouette, 3
                //SilhouetteOnly 4

                if (m_RenderMode == 0)//OutlineAll
                {
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_OutlineWidth", m_OutlineWidth);
                }
                else if (m_RenderMode == 1)//OutlineVisible
                {
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    outlineFillMaterial.SetFloat("_OutlineWidth", m_OutlineWidth);
                }
                else if (m_RenderMode == 2)//OutlineHidden
                {
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                    outlineFillMaterial.SetFloat("_OutlineWidth", m_OutlineWidth);
                }
                else if (m_RenderMode == 3)//OutlineAndSilhouette, 3
                {
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                    outlineFillMaterial.SetFloat("_OutlineWidth", m_OutlineWidth);
                }
                else if (m_RenderMode == 4)//SilhouetteOnly
                {
                    outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                    outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                    outlineFillMaterial.SetFloat("_OutlineWidth", 0);
                }
            }
        }

        public override void OnApplicationStart()
        {
            Debug.Log($"[{InfoAttribute.Name}] Version {InfoAttribute.Version} loaded!");

            ClassInjector.RegisterTypeInIl2Cpp<AnimalUpdates>();
            ClassInjector.RegisterTypeInIl2Cpp<DestoryArrowOnHit>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerBulletDamage>();
            ClassInjector.RegisterTypeInIl2Cpp<ClientProjectile>();
            ClassInjector.RegisterTypeInIl2Cpp<MultiplayerPlayer>();
            ClassInjector.RegisterTypeInIl2Cpp<ContainersSync>();
            ClassInjector.RegisterTypeInIl2Cpp<Outline>();
            ClassInjector.RegisterTypeInIl2Cpp<UiButtonPressHook>();
            ClassInjector.RegisterTypeInIl2Cpp<DestoryStoneOnStop>();

            if (instance == null)
            {
                instance = this;
                tcp = new TCP();
            }
            else if (instance != this)
            {

            }
            LoadedBundle = AssetBundle.LoadFromFile("Mods\\multiplayerstuff.unity3d");

            if (LoadedBundle == null)
            {
                MelonLogger.Log("Have problems with loading multiplayerstuff.unity3d!!");
            }
            else
            {
                MelonLogger.Log("Models loaded.");

            }
        }

        public override void OnApplicationQuit()
        {
            MelonLogger.Log("[CLIENT] Disconnect cause quit game");
            Disconnect();
        }

        private void Start()
        {
            tcp = new TCP();
        }

        public void ConnectToServer()
        {
            MelonLogger.Log("Trying connect to " + ip);
            InitializeClientData();
            tcp.Connect();
        }
        private void InitializeClientData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, MyMod.Welcome },
            { (int)ServerPackets.XYZ, MyMod.XYZ},
            { (int)ServerPackets.XYZW, MyMod.XYZW},
            { (int)ServerPackets.BLOCK, MyMod.BLOCK},
            { (int)ServerPackets.XYZDW, MyMod.XYZDW},
            { (int)ServerPackets.LEVELID, MyMod.LEVELID},
            { (int)ServerPackets.GOTITEM, MyMod.GOTITEM},
            { (int)ServerPackets.GAMETIME, MyMod.GAMETIME},
            { (int)ServerPackets.LIGHTSOURCENAME, MyMod.LIGHTSOURCENAME},
            { (int)ServerPackets.LIGHTSOURCE, MyMod.LIGHTSOURCE},
            //{ (int)ServerPackets.MAKEFIRE, MyMod.MAKEFIRE},
            { (int)ServerPackets.ANIMSTATE, MyMod.ANIMSTATE},
            { (int)ServerPackets.HASRIFLE, MyMod.HASRIFLE},
            { (int)ServerPackets.HASREVOLVER, MyMod.HASREVOLVER},
            { (int)ServerPackets.SLEEPHOURS, MyMod.SLEEPHOURS},
            { (int)ServerPackets.SYNCWEATHER, MyMod.SYNCWEATHER},
            { (int)ServerPackets.REVIVE, MyMod.REVIVE},
            { (int)ServerPackets.REVIVEDONE, MyMod.REVIVEDONE},
            { (int)ServerPackets.HASAXE, MyMod.HASAXE},
            { (int)ServerPackets.HISARROWS, MyMod.HISARROWS},
            { (int)ServerPackets.HASMEDKIT, MyMod.HASMEDKIT},
            { (int)ServerPackets.ANIMALROLE, MyMod.ANIMALROLE},
            { (int)ServerPackets.ANIMALSYNC, MyMod.ANIMALSYNC},
            { (int)ServerPackets.DARKWALKERREADY, MyMod.DARKWALKERREADY},
            { (int)ServerPackets.HOSTISDARKWALKER, MyMod.HOSTISDARKWALKER},
            { (int)ServerPackets.WARDISACTIVE, MyMod.WARDISACTIVE},
            { (int)ServerPackets.DWCOUNTDOWN, MyMod.DWCOUNTDOWN},
            { (int)ServerPackets.ANIMALSYNCTRIGG, MyMod.ANIMALSYNCTRIGG},
            { (int)ServerPackets.SHOOTSYNC, MyMod.SHOOTSYNC},
            { (int)ServerPackets.PIMPSKILL, MyMod.PIMPSKILL},
            { (int)ServerPackets.HARVESTINGANIMAL, MyMod.HARVESTINGANIMAL},
            { (int)ServerPackets.DONEHARVASTING, MyMod.DONEHARVASTING},
            { (int)ServerPackets.SAVEDATA, MyMod.SAVEDATA},
            { (int)ServerPackets.BULLETDAMAGE, MyMod.BULLETDAMAGE},
            { (int)ServerPackets.MULTISOUND, MyMod.MULTISOUND},
            { (int)ServerPackets.CONTAINEROPEN, MyMod.CONTAINEROPEN},
            { (int)ServerPackets.LUREPLACEMENT, MyMod.LUREPLACEMENT},
            { (int)ServerPackets.LUREISACTIVE, MyMod.LUREISACTIVE},
            { (int)ServerPackets.ALIGNANIMAL, MyMod.ALIGNANIMAL},
            { (int)ServerPackets.ASKFORANIMALPROXY, MyMod.ASKFORANIMALPROXY},
            { (int)ServerPackets.CARRYBODY, MyMod.CARRYBODY},
            { (int)ServerPackets.BODYWARP, MyMod.BODYWARP},
            { (int)ServerPackets.ANIMALDELETE, MyMod.ANIMALDELETE},
        };
            MelonLogger.Log("Initialized packets.");
        }
        public class TCP
        {
            public TcpClient socket;

            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];

                //socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);

                IAsyncResult result = socket.BeginConnect(instance.ip, instance.port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(5000, true);
                if (socket.Connected)
                {
                    socket.EndConnect(result);
                    stream = socket.GetStream();
                    receivedData = new Packet();
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                else
                {
                    socket.Close();
                    MelonLogger.Log("Connection failed!!!");
                }
            }
            /*
            private void ConnectCallback(IAsyncResult _result)
            {
                socket.EndConnect(_result);

                if (!socket.Connected)
                {
                    return;
                }

                stream = socket.GetStream();

                receivedData = new Packet();

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            */
            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    Debug.Log($"Error sending data to server via TCP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                if (socket == null || stream == null)
                {
                    if (stream == null)
                    {
                        MelonLogger.Log("ReceiveCallback has got null stream");
                    }
                    if (socket == null)
                    {
                        MelonLogger.Log("ReceiveCallback has got null socket");
                    }
                    return;
                }

                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        MelonLogger.Log("[CLIENT TCP SOCKET] Disconnect got no bytes in data stream");

                        if (socket != null)
                        {
                            if (sendMyPosition == true)
                            {
                                MelonLogger.Log("Killing data stream...");
                                stream = null;
                                MelonLogger.Log("Wiping recevied data...");
                                receivedData = null;
                                MelonLogger.Log("Reseting data buffer...");
                                receiveBuffer = null;
                                MelonLogger.Log("Closing socket");
                                socket.Close();
                                socket = null;
                                MelonLogger.Log("Disconnected from server.");
                                HUDMessage.AddMessage("DISCONNECTED FROM SERVER");
                                sendMyPosition = false;
                            }
                        }
                        return;
                    } else {
                        byte[] _data = new byte[_byteLength];
                        Array.Copy(receiveBuffer, _data, _byteLength);

                        receivedData.Reset(HandleData(_data));
                        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                    }
                }
                catch
                {
                    /*
                    if(socket != null)
                    {
                        MelonLogger.Log("[CLIENT TCP SOCKET] Disconnect cause getting callback error");

                        if (sendMyPosition == true)
                        {
                            MelonLogger.Log("Killing data stream...");
                            stream = null;
                            MelonLogger.Log("Wiping recevied data...");
                            receivedData = null;
                            MelonLogger.Log("Reseting data buffer...");
                            receiveBuffer = null;
                            MelonLogger.Log("Closing socket");
                            socket.Close();
                            socket = null;
                            MelonLogger.Log("Disconnected from server.");
                            HUDMessage.AddMessage("DISCONNECTED FROM SERVER");
                            sendMyPosition = false;
                        }
                    }
                    */
                    MelonLogger.Log("[CLIENT TCP SOCKET] Disconnect cause getting callback error");
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                if (receivedData == null || _data == null)
                {
                    return false;
                }

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();

                            if(packetHandlers[_packetId] != null)
                            {
                                packetHandlers[_packetId](_packet);
                            }else{
                                MelonLogger.Log("Got unregisted _packetId"+ _packetId);
                            }
                        }
                    });

                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;
            }
        }

        public static void AWait()
        {
            Thread.Sleep(5000);
        }


        public static async void SlowDisconnect()
        {
            MelonLogger.Log("[CLIENT] Slow disconnect...");
            await Task.Run(() => AWait());
            MelonLogger.Log("[CLIENT] Now doing disconnect..");
            Disconnect();
        }

        public static void Disconnect()
        {
            MelonLogger.Log("[CLIENT] Trying disconnect");
            if (iAmHost == true || sendMyPosition == true)
            {

                if (MyMod.instance == null)
                {
                    MelonLogger.Log("TCP instance is dead.");
                    return;
                }

                if (MyMod.instance.tcp == null)
                {
                    MelonLogger.Log("TCP protocol is closed.");
                    return;
                }
                if (MyMod.instance.tcp.socket == null)
                {
                    MelonLogger.Log("Socket of tcp protocl is closed.");
                    return;
                }
                if (MyMod.instance.tcp != null && MyMod.instance.tcp.socket != null)
                {
                    MyMod.instance.tcp.socket.Close();
                    MyMod.instance.tcp.socket = null;
                }
                Debug.Log("Disconnected from server.");
                HUDMessage.AddMessage("DISCONNECTED FROM SERVER");
                sendMyPosition = false;
            }
        }

        private static void SendTCPData(Packet _packet)
        {
            _packet.WriteLength();
            MyMod.instance.tcp.SendData(_packet);
        }

        #region Packets
        public static void WelcomeReceived()
        {
            using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
            {
                _packet.Write(MyMod.instance.myId);
                _packet.Write("Name");

                SendTCPData(_packet);
            }
            using (Packet _packet = new Packet((int)ClientPackets.LEVELID))
            {
                _packet.Write(levelid);
                SendTCPData(_packet);
            }
        }
        #endregion

        private static bool isRuning = false;
        private static bool sendMyPosition = false;
        private static bool iAmHost = false;
        public static bool IamShatalker = false;
        public static GameObject anotherbutt = null;
        public static GameObject playerbody = null;
        public static Campfire FlexFire = null;
        public static Animator MenuErjan1 = null;
        public static Animator MenuErjan2 = null;
        public static bool CarryingPlayer = false;
        public static bool IsCarringMe = false;
        public static bool IsDead = false;
        public static bool KillAfterLoad = false;
        public static bool DoFakeGetup = false;
        public static string PlayerBodyGUI = "";
        private static Animator animbutt;
        private static InvisibleEntityManager ShatalkerObject = null;
        private static HUDNowhereToHide DarkWalkerHUD = null;
        private static GameObject WardWidget = null;
        private static GameObject LureWidget = null;
        private static GameObject DistanceWidget = null;
        private static UILabel DistanceLable = null;
        public static bool WardIsActive = false;
        public static bool LureIsActive = false;
        private static bool PreviousDarkWalkerReady = false;
        private static bool DarkWalkerIsReady = false;
        private static bool ShatalkerModeClient = false;
        private static int levelid = 0;
        public static int anotherplayer_levelid = 0;
        private static string level_name = "";
        private static Vector3 PreviousBlock = new Vector3(0, 0, 0);
        public static Vector3 LastRecivedShatalkerVector = new Vector3(0, 0, 0);
        public static Vector3 LastRecivedOtherPlayerVector = new Vector3(0, 0, 0);
        public static Quaternion LastRecivedOtherPlayerQuatration = new Quaternion(0, 0, 0, 0);
        private static bool ShiftPressed = false;
        private static bool NeedSyncTime = false;
        private static bool LightSource = false;
        private static bool PreviousLightSource = false;
        private static string LightSourceName = "";
        private static string PreviousLightSourceName = "";
        private static bool MyLightSource = false;
        private static string MyLightSourceName = "";
        private static bool MyLastLightSource = false;
        private static string MyLastLightSourceName = "";
        //private static Vector3 FireToIgnite = new Vector3(0, 0, 0);
        private static string LastSelectedGearName = "";
        private static GearItem LastSelectedGear = null;
        private static bool NeedRefreshInv = false;
        private static string AnimState = "Idle";
        private static string AnimStateHands = "No";
        private static string PreAnimStateHands = "";
        private static string AnimStateFingers = "No";
        private static string MyAnimState = "Idle";
        private static string MyPreviousAnimState = "Idle";
        private static bool HasRifle = false;
        private static bool PrviousHasRifle = false;
        private static bool MyHasRifle = false;
        private static bool LastRecivedHasRifle = false;
        private static bool HasRevolver = false;
        private static bool PrviousHasRevolver = false;
        private static bool MyHasRevolver = false;
        private static bool LastRecivedHasRevolver = false;
        private static bool MyHasAxe = false;
        private static bool PreviousHasAxe = false;
        private static bool HasAxe = false;
        private static bool LastRecivedHasAxe = false;
        private static bool HasMedkit = false;
        private static bool PreviousHasMedkit = false;
        private static bool MyHasMedkit = false;
        private static bool LastRecivedHasMedkit = false;
        private static int CycleSkip = 0;
        private static int MyCycleSkip = 0;
        private static bool PreviousSleeping = false;
        private static bool IsCycleSkiping = false;
        private static int LastSelectedWeatherSet;
        private static WeatherSet LastSelectedWeatherSet2;
        private static List<string> KnownAnimals = new List<string>();
        private static int MyArrows = 0;
        private static int PreviousArrows = 0;
        private static int HisArrows = 0;
        private static int LastRecivedArrows = 0;
        private static int StepState = 0;
        private static GameObject LastObjectUnderCrosshair = null;
        private static Vector3 V3BeforeSleep = new Vector3(0, 0, 0);
        private static bool NeedV3BeforeSleep = false;
        private static GameObject MenuStuffSpawned = null;
        private static bool AnimalsController = true;
        private static int TicksOnScene = 0;
        private static int MyTicksOnScene = 0;
        private static int previous_anotherplayer_levelid = 0;
        private static int previous_levelid = 0;
        //private static UnhollowerBaseLib.Il2CppArrayBase<BaseAi> animals = Resources.FindObjectsOfTypeAll<BaseAi>();
        private static List<GameObject> AllAnimalsNew = new List<GameObject>();
        private static List<GameObject> AllAnimals = new List<GameObject>();
        public static float MaxAniamlsSyncDistance = 245f;
        public static int MaxAnimalsSyncCount = 11;
        public static int MaxAnimalsSyncCountOnConnect = 2;
        public static int MaxAnimalsSyncNeed = 0;
        public static int DeltaAnimalsMultiplayer = 4;
        public static string DebugAnimalGUID = "";
        public static string DebugAnimalGUIDLast = "";
        public static GameObject DebugLastAnimal = null;
        public static bool InDarkWalkerMode = false;
        public static float DarkWalkerSpeed = 0.12f;
        public static bool RealTimeCycleSpeed = true;
        public static string HarvestingAnimal = "";
        public static string PreviousHarvestingAnimal = "";
        public static string OtherHarvetingAnimal = "";
        public static List<WalkTracker> SurvivorWalks = new List<WalkTracker>();
        public static WalkTracker LastLure = new WalkTracker();
        public static bool ALWAYS_FUCKING_CURSOR_ON = false;
        public static List<GameObject> PlayerColiders = new List<GameObject>();
        public static bool NoRabbits = true;

        public static void Welcome(Packet _packet)
        {
            string _msg = _packet.ReadString();
            int _myId = _packet.ReadInt();

            MelonLogger.Log($"Message from server: {_msg}");
            MyMod.instance.myId = _myId;
            MyMod.WelcomeReceived();
            sendMyPosition = true;
        }
        public static void XYZ(Packet _packet)
        {
            Vector3 maboi;
            maboi = _packet.ReadVector3();

            if (anotherbutt != null && levelid == anotherplayer_levelid)
            {
                //anotherbutt.transform.position = new Vector3(maboi.x, maboi.y + 0.03f, maboi.z);

                LastRecivedOtherPlayerVector = new Vector3(maboi.x, maboi.y + 0.03f, maboi.z);
            }

            if(IamShatalker == true)
            {
                bool NeedListIt = true;

                for (int i = 0; i < SurvivorWalks.Count; i++)
                {
                    if(SurvivorWalks[i].m_levelid == anotherplayer_levelid)
                    {
                        NeedListIt = false;
                        SurvivorWalks[i].m_V3 = maboi;
                        break;
                    }
                }
                if(NeedListIt == true)
                {
                    WalkTracker ToAdd = new WalkTracker();
                    ToAdd.m_levelid = anotherplayer_levelid;
                    ToAdd.m_V3 = maboi;
                    SurvivorWalks.Add(ToAdd);
                }
            }
        }
        public class WalkTracker : MelonMod
        {
            public int m_levelid = 0;
            public Vector3 m_V3 = new Vector3(0, 0, 0);
        }

        public static void XYZDW(Packet _packet)
        {
            ShatalkerModeClient = true;
            Vector3 maboi;
            maboi = _packet.ReadVector3();

            if (ShatalkerObject != null && levelid == anotherplayer_levelid)
            {
                ShatalkerObject.m_WorldPosition = maboi;
                LastRecivedShatalkerVector = maboi;
            }
        }
        public static void XYZW(Packet _packet)
        {
            Quaternion maboi;
            maboi = _packet.ReadQuaternion();

            if (anotherbutt != null && levelid == anotherplayer_levelid)
            {
                //anotherbutt.transform.rotation = new Quaternion(maboi.x, maboi.y, maboi.z, maboi.w);
                LastRecivedOtherPlayerQuatration = new Quaternion(maboi.x, maboi.y, maboi.z, maboi.w);
            }
        }
        public static void BLOCK(Packet _packet)
        {
            Vector3 maboi;
            maboi = _packet.ReadVector3();

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = maboi;
        }
        public static void LEVELID(Packet _packet)
        {
            anotherplayer_levelid = _packet.ReadInt();
            MelonLogger.Log("Other player transition to level " + anotherplayer_levelid);
        }

        public static void GiveRecivedItem(GearItem got)
        {
            string dummy_name = got.m_GearName;
            string give_name = "";

            string say = "";
            bool watermode = false;
            LiquidQuality water_q = LiquidQuality.Potable;

            if (dummy_name.Contains("(Clone)")) //If it has ugly (Clone), cutting it.
            {
                int L = dummy_name.Length - 7;
                give_name = dummy_name.Remove(L, 7);
            }
            else
            {
                give_name = dummy_name;
            }

            if (give_name == "GEAR_WaterSupplyPotable")
            {
                watermode = true;
            }
            if (give_name == "GEAR_WaterSupplyNotPotable")
            {
                watermode = true;
                water_q = LiquidQuality.NonPotable;
            }

            if (watermode == false) // If this is water we not give new item, but just add to supply.
            {
                //Creating new item to load all static parameters of item as base, to not ask host about constant values of item.
                GearItem new_gear = GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory(give_name, 1);
                //Setting dynamic values of item that we got from host
                new_gear.m_CurrentHP = got.m_CurrentHP;
                new_gear.m_WeightKG = got.m_WeightKG;
                new_gear.m_GearBreakConditionThreshold = got.m_GearBreakConditionThreshold;

                if (new_gear.m_FoodItem != null) // If it food, we need load FoodItem component too.
                {
                    new_gear.m_FoodItem.m_CaloriesTotal = got.m_FoodItem.m_CaloriesTotal;
                    new_gear.m_FoodItem.m_CaloriesRemaining = got.m_FoodItem.m_CaloriesRemaining;
                    new_gear.m_FoodItem.m_HeatPercent = got.m_FoodItem.m_HeatPercent;
                    new_gear.m_FoodItem.m_Packaged = got.m_FoodItem.m_Packaged;
                    new_gear.m_FoodItem.m_Opened = got.m_FoodItem.m_Opened;
                }
                if (new_gear.m_EvolveItem != null) // If is evolve item(So stupid name) then we need load item of it was drying.
                {
                    new_gear.m_EvolveItem.m_TimeSpentEvolvingGameHours = got.m_EvolveItem.m_TimeSpentEvolvingGameHours;
                }
                say = new_gear.m_LocalizedDisplayName.Text();
            }
            else
            {
                string bottlename = Resources.Load(give_name).Cast<GameObject>().GetComponent<GearItem>().m_LocalizedDisplayName.Text();

                MelonLogger.Log("Got water " + got.m_WaterSupply.m_VolumeInLiters);
                if (got.m_WaterSupply.m_VolumeInLiters == 0.5f)
                {
                    say = "half liter of " + bottlename;
                }
                else
                {
                    say = got.m_WaterSupply.m_VolumeInLiters + " of " + bottlename;
                }
                GameManager.GetInventoryComponent().AddToWaterSupply(got.m_WaterSupply.m_VolumeInLiters, water_q);
            }

            HUDMessage.AddMessage("Other player gave you " + say + ".");

            MelonLogger.Log("Other player gave you item " + give_name);
        }


        public static void GOTITEM(Packet _packet)
        {
            GearItem got = _packet.ReadGear();
            GiveRecivedItem(got);
        }
        public static void GAMETIME(Packet _packet)
        {
            OveridedTime = _packet.ReadString();
            //MelonLogger.Log("Time: " + OveridedTime);
        }
        public static void LIGHTSOURCENAME(Packet _packet)
        {
            LightSourceName = _packet.ReadString();
            MelonLogger.Log("Other player changed source light: " + LightSourceName);
            UpdateHeldItem(LightSourceName);
        }
        public static void LIGHTSOURCE(Packet _packet)
        {
            LightSource = _packet.ReadBool();
            MelonLogger.Log("Other player toggle light: " + LightSource);
            UpdateHeldItem("nodis");
        }
        public static void MAKEFIRE(Packet _packet)
        {
            //FireToIgnite = _packet.ReadVector3();
            //MelonLogger.Log("Other player ignite fire");
        }
        public static void ANIMSTATE(Packet _packet)
        {
            AnimState = _packet.ReadString();
            //MelonLogger.Log("Other player changed animation: " + AnimState);
        }
        public static void HASRIFLE(Packet _packet)
        {
            HasRifle = _packet.ReadBool();
            MelonLogger.Log("Other player toggle rifle: " + HasRifle);
            UpdateHeldItem("nodis");
        }
        public static void HASREVOLVER(Packet _packet)
        {
            HasRevolver = _packet.ReadBool();
            MelonLogger.Log("Other player toggle rifle: " + HasRevolver);
            UpdateHeldItem("nodis");
        }
        public static void HASAXE(Packet _packet)
        {
            HasAxe = _packet.ReadBool();
            MelonLogger.Log("Other player toggle axe: " + HasAxe);
            UpdateHeldItem("nodis");
        }
        public static void HASMEDKIT(Packet _packet)
        {
            HasMedkit = _packet.ReadBool();
            MelonLogger.Log("Other player toggle medkit: " + HasMedkit);
        }
        public static void HISARROWS(Packet _packet)
        {
            HisArrows = _packet.ReadInt();
            MelonLogger.Log("Other player changed count of arrows: " + HisArrows);
            UpdateHeldItem("nodis");
        }
        public static void SLEEPHOURS(Packet _packet)
        {
            CycleSkip = _packet.ReadInt();

            if (CycleSkip != 0)
            {
                MelonLogger.Log("Other player is sleeping");
            }
            else
            {
                MelonLogger.Log("Other player is wakeup");
            }
        }
        public class WeatherProxies : MelonMod
        {
            public string m_WeatherProxy = "";
            public string m_WeatherTransitionProxy = "";
            public string m_WindProxy = "";
        }

        public static void SYNCWEATHER(Packet _packet)
        {
            WeatherProxies weather = _packet.ReadWeather();
            if (level_name != "Boot" && level_name != "Empty" && GameManager.m_Wind != null && GameManager.m_Wind.m_ActiveSettings != null && GameManager.m_Weather != null && GameManager.m_WeatherTransition != null)
            {
                GameManager.GetWeatherComponent().Deserialize(weather.m_WeatherProxy);
                GameManager.GetWeatherTransitionComponent().Deserialize(weather.m_WeatherTransitionProxy);
                GameManager.GetWindComponent().Deserialize(weather.m_WindProxy);
            }
        }

        public static void SimRevive()
        {
            KillAfterLoad = false;
            SetRevivedStats();
            GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Normal);
            Condition con = GameManager.GetConditionComponent();
        }

        public static void REVIVE(Packet _packet)
        {
            SimRevive();
            if (sendMyPosition == true)
            {
                using (Packet __packet = new Packet((int)ClientPackets.REVIVEDONE))
                {
                    __packet.Write(true);
                    SendTCPData(__packet);
                }
            }
        }
        public static void ANIMALROLE(Packet _packet)
        {
            AnimalsController = _packet.ReadBool();
            MelonLogger.Log("Got new animal controller role: " + AnimalsController);
            AllowSpawnAnimals(AnimalsController);
        }
        public static void ALIGNANIMAL(Packet _packet)
        {
            AnimalAligner Alig = _packet.ReadAnimalAligner();
            AlignAnimalWithProxy(Alig.m_Proxy, Alig.m_Guid);
        }
        public static void ASKFORANIMALPROXY(Packet _packet)
        {
            string _guid = _packet.ReadString();
            string Proxy = "";
            for (int i = 0; i < BaseAiManager.m_BaseAis.Count; i++)
            {
                if (BaseAiManager.m_BaseAis[i] != null && BaseAiManager.m_BaseAis[i].gameObject != null)
                {
                    GameObject animal = BaseAiManager.m_BaseAis[i].gameObject;
                    if (animal.GetComponent<ObjectGuid>() != null && animal.GetComponent<ObjectGuid>().Get() == _guid)
                    {
                        Proxy = BaseAiManager.m_BaseAis[i].Serialize();
                        MelonLogger.Log("ASKFORANIMALPROXY " + Proxy);
                        break;
                    }
                }
            }
            using (Packet __packet = new Packet((int)ClientPackets.ALIGNANIMAL))
            {
                AnimalAligner Alig = new AnimalAligner();

                Alig.m_Guid = _guid;
                Alig.m_Proxy = Proxy;
                __packet.Write(Alig);
                SendTCPData(__packet);
            }
        }
        public class AnimalAligner : MelonMod
        {
            public string m_Proxy = "";
            public string m_Guid = "";
        }
        public static void AlignAnimalWithProxy(string Proxy, string _guid)
        {
            for (int i = 0; i < BaseAiManager.m_BaseAis.Count; i++)
            {
                if (BaseAiManager.m_BaseAis[i] != null && BaseAiManager.m_BaseAis[i].gameObject != null)
                {
                    GameObject animal = BaseAiManager.m_BaseAis[i].gameObject;
                    if(animal.GetComponent<ObjectGuid>() != null && animal.GetComponent<ObjectGuid>().Get() == _guid)
                    {
                        MelonLogger.Log("AlignAnimalWithProxy Deserialize " + Proxy);
                        BaseAiManager.m_BaseAis[i].Deserialize(Proxy);

                        AnimalUpdates au = animal.GetComponent<AnimalUpdates>();

                        if(au != null)
                        {
                            au.m_WaitForAligment = false;
                        }
                        break;
                    }
                }
            }
        }

        public static void CARRYBODY(Packet _packet)
        {
            IsCarringMe = _packet.ReadBool();
        }
        public static void BODYWARP(Packet _packet)
        {
            string DoorId = _packet.ReadString();

            MelonLogger.Log("Got remote door enter request "+ DoorId);

            WarpBody(DoorId);
        }

        public static void DeleteAnimal(string _guid)
        {
            MelonLogger.Log("Got signal to delete " + _guid + " animal");
            for (int i = 0; i < BaseAiManager.m_BaseAis.Count; i++)
            {
                if (BaseAiManager.m_BaseAis[i] != null && BaseAiManager.m_BaseAis[i].gameObject != null)
                {
                    GameObject animal = BaseAiManager.m_BaseAis[i].gameObject;
                    if (animal.GetComponent<ObjectGuid>() != null)
                    {
                        if (_guid == animal.GetComponent<ObjectGuid>().Get())
                        {
                            MelonLogger.Log("Found and deleted" + animal.GetComponent<ObjectGuid>().Get() + " animal");
                            UnityEngine.Object.Destroy(animal);
                            break;
                        }
                    }
                }
            }
        }


        public static void ANIMALDELETE(Packet _packet)
        {
            string AnimalGuid = _packet.ReadString();
            DeleteAnimal(AnimalGuid);
        }

        public static void DoAnimalSync(AnimalSync obj)
        {
            string _guid = obj.m_guid;
            string prefabName = obj.m_name;
            bool AnimalExists = false;
            bool ShouldRecreate = obj.m_UnderYourControl;

            if (obj.m_name == null)
            {
                MelonLogger.Log("Are you pidor? Animal name is NULL!!!!!!!!!!!!!!!!!");
                return;
            }
            if(NoRabbits == true && obj.m_name.Contains("Rabbit"))
            {
                MelonLogger.Log("Got sync of rabbit, refuse to use this.");
                return;
            }

            //MelonLogger.Log(_guid + " Got animal " + obj.m_name);

            for (int i = 0; i < AllAnimals.Count; i++)
            {
                if (AllAnimals.ElementAt(i) != null && AllAnimals.ElementAt(i).gameObject != null && AllAnimals.ElementAt(i).gameObject.GetComponent<ObjectGuid>() != null)
                {
                    GameObject animal = AllAnimals.ElementAt(i).gameObject;

                    string guid_ = animal.GetComponent<ObjectGuid>().Get();
                    if (guid_ == _guid)
                    {
                        //MelonLogger.Log(_guid + "---------------------> Updating animal");

                        BaseAi _AI = animal.GetComponent<BaseAi>();
                        AnimalUpdates au = animal.GetComponent<AnimalUpdates>();
                        float among = Vector3.Distance(GameManager.GetPlayerTransform().position, anotherbutt.transform.position);

                        //if (AnimalsController == true)
                        //{
                        //    if (_AI.GetAiMode() == AiMode.Dead || _AI.GetAiMode() == AiMode.Struggle || _AI.m_CurrentHP <= 0 || _AI.m_HasEnteredStruggleOnLastAttack == true || AnimState == "Knock" || IsDead == true || among < 15 || au.ReTakeCoolDown > 0)
                        //    {

                        //    }else{
                        //        MelonLogger.Log("Stop from beaing updated from client");
                        //        au.m_ClientControlled = false;
                        //        au.CallSync();
                        //        return;
                        //    }
                        //}

                        if (animal.GetComponent<AnimalUpdates>() != null)
                        {
                            if(au.m_Banned == true)
                            {
                                return;
                            }
                            if (obj.m_ProxySave != "")
                            {
                                au.m_PendingProxy = obj.m_ProxySave;
                            }
                            if (AnimalsController == false)
                            {
                                au.m_UnderMyControl = obj.m_UnderYourControl;
                                if(au.m_UnderMyControl == true)
                                {                      
                                    //MelonLogger.Log("Got sync of controlled by me animal.");
                                }
                            }else{
                                au.m_UnderMyControl = false;
                                au.m_ClientControlled = true;
                            }

                            if (au.m_UnderMyControl == false)
                            {
                                au.m_ToGo = obj.m_position;
                                au.m_ToRotate = obj.m_rotation;

                                au.AP_TurnAngle = obj.AP_TurnAngle;
                                au.AP_TurnSpeed = obj.AP_TurnSpeed;
                                au.AP_Speed = obj.AP_Speed;
                                au.AP_Wounded = obj.AP_Wounded;
                                au.AP_Roll = obj.AP_Roll;
                                au.AP_Pitch = obj.AP_Pitch;
                                au.AP_TargetHeading = obj.AP_TargetHeading;
                                au.AP_TargetHeadingSmooth = obj.AP_TargetHeadingSmooth;
                                au.AP_TapMeter = obj.AP_TapMeter;
                                au.AP_AiState = obj.AP_AiState;
                                au.AP_Corpse = obj.AP_Corpse;
                                au.AP_Dead = obj.AP_Dead;
                                au.AP_DeadSide = obj.AP_DeadSide;
                                au.AP_DamageBodyPart = obj.AP_DamageBodyPart;
                                au.AP_AttackId = obj.AP_AttackId;

                                au.m_Hp = obj.m_Hp;
                                au.m_Bleeding = obj.m_Bleeding;
                            }

                            au.NoResponce = 5;
                            BodyHarvest BH = animal.GetComponent<BodyHarvest>();
                            BH.m_MeatAvailableKG = obj.m_Meat;
                            BH.m_GutAvailableUnits = obj.m_Guts;
                            BH.m_HideAvailableUnits = obj.m_Hide;
                            BH.m_PercentFrozen = obj.m_Frozen;

                            //if (obj.m_anim != 0)
                            //{
                            //    animal.GetComponent<AnimalUpdates>().m_Anim = obj.m_anim;
                            //}
                            //animal.GetComponent<AnimalUpdates>().m_AnimNext = obj.m_animnext;
                        }
                        AnimalExists = true;
                        break;
                    }
                }
            }
            if (AnimalExists == false)
            {
                SpawnAnimal(prefabName, obj.m_position, _guid, obj.m_UnderYourControl, obj.m_ProxySave);
            }
        }

        public static void SetAnimalTriggers(AnimalTrigger obj)
        {
            string _guid = obj.m_Guid;
            int trigg = obj.m_Trigger;

            for (int i = 0; i < AllAnimals.Count; i++)
            {
                if (AllAnimals.ElementAt(i) != null && AllAnimals.ElementAt(i).gameObject != null && AllAnimals.ElementAt(i).gameObject.GetComponent<ObjectGuid>() != null)
                {
                    GameObject animal = AllAnimals.ElementAt(i).gameObject;

                    string guid_ = animal.GetComponent<ObjectGuid>().Get();
                    if (guid_ == _guid)
                    {
                        if (animal.GetComponent<BaseAi>() != null)
                        {
                            BaseAi _AI = animal.GetComponent<BaseAi>();
                            Animator AN = _AI.m_Animator;

                            if(trigg == _AI.m_AnimParameter_StruggleEnd)
                            {
                                MelonLogger.Log("Going to end struggle, so reseting StruggleStart");
                                AN.ResetTrigger(_AI.m_AnimParameter_StruggleStart);
                            }else if (trigg == _AI.m_AnimParameter_StruggleStart)
                            {
                                MelonLogger.Log("Going to start struggle, so reseting StruggleEnd");
                                AN.ResetTrigger(_AI.m_AnimParameter_StruggleEnd);
                            }
                            _AI.AnimSetTrigger(trigg);
                            MelonLogger.Log("Set trigger for animal " + _guid + " trigger hash " + trigg);
                        }
                        break;
                    }
                }
            }
        }

        public static void ANIMALSYNC(Packet _packet)
        {
            if (level_name == "Boot" || level_name == "Empty")
            {
                return;
            }

            AnimalSync obj = _packet.ReadAnimal();
            DoAnimalSync(obj);
        }

        public static void ANIMALSYNCTRIGG(Packet _packet)
        {
            if (level_name == "Boot" || level_name == "Empty")
            {
                return;
            }

            AnimalTrigger obj = _packet.ReadAnimalTrigger();
            SetAnimalTriggers(obj);
        }

        public static void REVIVEDONE(Packet _packet)
        {
            GameManager.GetInventoryComponent().RemoveGearFromInventory("GEAR_MedicalSupplies_hangar", 1);
        }

        public static void DARKWALKERREADY(Packet _packet)
        {
            DarkWalkerIsReady = _packet.ReadBool();
            MelonLogger.Log("Got new darkwalker ready state: " + DarkWalkerIsReady);

            if (DarkWalkerIsReady == true)
            {
                ServerHandle.LastCountDown = 0;
                OverridenStartCountDown = 0;
            }
        }

        public static void HOSTISDARKWALKER(Packet _packet)
        {
            if (InDarkWalkerMode == true)
            {
                ShatalkerModeClient = _packet.ReadBool();
                MelonLogger.Log("Host is darkwalker: " + ShatalkerModeClient);

                RealTimeCycleSpeed = false;

                if (ShatalkerModeClient == false)
                {
                    IamShatalker = true;
                    uConsole.RunCommandSilent("Ghost");
                    uConsole.RunCommandSilent("God");

                    if (sendMyPosition == true)
                    {
                        using (Packet __packet = new Packet((int)ClientPackets.REQUESTDWREADYSTATE))
                        {
                            __packet.Write(true);
                            SendTCPData(__packet);
                        }
                    }
                } else
                {
                    if (ShatalkerObject.GetStartMovementDelayTime() < 2)
                    {
                        using (Packet __packet = new Packet((int)ClientPackets.DARKWALKERREADY))
                        {
                            __packet.Write(DarkWalkerIsReady);
                            SendTCPData(__packet);
                        }
                    }
                }
            }
        }

        public static void WARDISACTIVE(Packet _packet)
        {
            WardIsActive = _packet.ReadBool();
        }
        public static void DWCOUNTDOWN(Packet _packet)
        {
            float got = _packet.ReadFloat();

            if (OverridenStartCountDown < 5 && got > OverridenStartCountDown)
            {
                return;
            }
            OverridenStartCountDown = got;
            ///Console.WriteLine("LastCountDown " + OverridenStartCountDown);
        }

        public static void DoShootFX(Vector3 pos)
        {
            GameObject LightFX = new GameObject();
            LightFX.transform.position = pos;
            Light LightComp = LightFX.AddComponent<Light>();
            LightComp.type = LightType.Point;
            LightComp.range = 5;
            LightComp.intensity = 5;
            LightComp.color = new Color(1, 0.5623099f, 0.3268814f,1);
            UnityEngine.Object.Destroy(LightFX, 0.1f);
        }

        public static void DoShootSync(ShootSync shoot)
        {
            MelonLogger.Log("Shoot: ");
            MelonLogger.Log("X: " + shoot.m_position.x);
            MelonLogger.Log("Y: " + shoot.m_position.y);
            MelonLogger.Log("Z: " + shoot.m_position.z);
            MelonLogger.Log("Rotation X: " + shoot.m_rotation.x);
            MelonLogger.Log("Rotation Y: " + shoot.m_rotation.y);
            MelonLogger.Log("Rotation Z: " + shoot.m_rotation.z);
            MelonLogger.Log("Rotation W: " + shoot.m_rotation.w);

            if (shoot.m_projectilename == "GEAR_FlareGunAmmoSingle")
            {
                FlareGunRoundItem.SpawnAndFire(GetGearItemObject("GEAR_FlareGunAmmoSingle"), shoot.m_position, shoot.m_rotation);
            }
            else if (shoot.m_projectilename == "GEAR_Arrow")
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("GEAR_Arrow"), shoot.m_position, shoot.m_rotation);
                GearItem component = gameObject.GetComponent<GearItem>();
                gameObject.name = "GEAR_Arrow";
                gameObject.transform.parent = (Transform)null;
                component.m_InPlayerInventory = false;
                component.m_StackableItem.m_Units = 1;
                component.m_CurrentHP = 100;
                component.m_ArrowItem.SetPlacementHelperEnabled(true);

                gameObject.AddComponent<DestoryArrowOnHit>();

                Utils.ChangeLayersForGearItem(gameObject, 17);
                component.m_ArrowItem.Fire();
            }
            else if (shoot.m_projectilename == "GEAR_Stone")
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("GEAR_Stone"), shoot.m_position, shoot.m_rotation);
                float throwForce = GameManager.m_PlayerManager.m_ThrowForce;
                float num = GameManager.m_PlayerManager.m_ThrowTorque;
                GearItem component = gameObject.GetComponent<GearItem>();
                component.m_StoneItem.PrepareForThrow();
                component.m_StoneItem.SetThrown(true);
                throwForce = component.m_StoneItem.m_ThrowForce;
                num = component.m_StoneItem.m_ThrowForce;

                Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
                component2.isKinematic = false;
                component2.velocity = shoot.m_camera_forward * throwForce;

                Vector3 vector3 = shoot.m_camera_right + UnityEngine.Random.Range(-0.2f, 0.2f) * shoot.m_camera_up;
                vector3.Normalize();
                component2.angularVelocity = vector3 * num;
                component2.angularDrag = 0.0f;
                component2.drag = 0.0f;
                gameObject.AddComponent<DestoryStoneOnStop>();
                gameObject.GetComponent<DestoryStoneOnStop>().m_Obj = gameObject;
                gameObject.GetComponent<DestoryStoneOnStop>().m_RB = component2;
            }
            else
            {
                MelonLogger.Log("Got remote shoot event " + shoot.m_projectilename);

                GameObject gameObject = null;
                GearItem itemInHands = null;

                if (shoot.m_projectilename == "PistolBullet")
                {
                    itemInHands = GetGearItemPrefab("GEAR_Rifle");
                    gameObject = UnityEngine.Object.Instantiate<GameObject>(PistolBulletPrefab, shoot.m_position, shoot.m_rotation);
                    gameObject.AddComponent<ClientProjectile>();
                    ClientProjectile ClientP = gameObject.GetComponent<ClientProjectile>();
                    PlayMultiplayer3dAduio("PLAY_RIFLE_SHOOT_3D");
                    DoShootFX(shoot.m_position);
                }
                else if (shoot.m_projectilename == "RevolverBullet")
                {
                    itemInHands = GetGearItemPrefab("GEAR_Revolver");
                    gameObject = UnityEngine.Object.Instantiate<GameObject>(RevolverBulletPrefab, shoot.m_position, shoot.m_rotation);
                    gameObject.AddComponent<ClientProjectile>();
                    ClientProjectile ClientP = gameObject.GetComponent<ClientProjectile>();
                    PlayMultiplayer3dAduio("PLAY_REVOLVER_SHOOT_3D");
                    DoShootFX(shoot.m_position);
                }

                if (itemInHands == null || gameObject == null || anotherbutt == null)
                {
                    return;
                }

                gameObject.hideFlags = HideFlags.HideInHierarchy;
                vp_Bullet component1 = gameObject.GetComponent<vp_Bullet>();

                if ((bool)(UnityEngine.Object)itemInHands)
                {
                    GunItem component2 = itemInHands.GetComponent<GunItem>();
                    if ((bool)(UnityEngine.Object)component2)
                    {
                        component1.m_GunType = component2.m_GunType;
                        component1.MinDistanceForAimAssist = component2.m_MinDistanceForAimAssist;
                        component1.Damage = component2.m_DamageHP;
                        component1.Accuracy = component1.m_GunType != GunType.Rifle ? component2.m_AccuracyRange : shoot.m_skill;
                        component1.m_ImpactAudio = component2.m_ImpactAudio;
                    }
                }
                //GameObject emitterFromGameObject = GameAudioManager.GetSoundEmitterFromGameObject(gameObject);
                //AkSoundEngine.PostEvent(AK.EVENTS.PLAY_RIFLEFIRE, emitterFromGameObject);
                //GameAudioManager.SetAudioSourceTransform(emitterFromGameObject, emitterFromGameObject.transform);
                GameAudioManager.NotifyAiAudioEvent(anotherbutt, anotherbutt.transform.position, GameAudioAiEvent.Gunshot);
            }
        }

        public static void SHOOTSYNC(Packet _packet)
        {
            ShootSync shoot = _packet.ReadShoot();
            DoShootSync(shoot);
        }

        public static void PIMPSKILL(Packet _packet)
        {
            int SkillTypeId = _packet.ReadInt();

            if (SkillTypeId == 1)
            {
                GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Rifle, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                MelonLogger.Log("Got remote skill upgrade Rifle");
            }
            else if (SkillTypeId == 2)
            {
                GameManager.GetSkillsManager().IncrementPointsAndNotify(SkillType.Revolver, 1, SkillsManager.PointAssignmentMode.AssignOnlyInSandbox);
                MelonLogger.Log("Got remote skill upgrade Revolver");
            }
        }

        public static void HARVESTINGANIMAL(Packet _packet)
        {
            OtherHarvetingAnimal = _packet.ReadString();
        }

        public static void DoForcedHarvestAnimal(string _guid, HarvestStats Harvey)
        {
            for (int i = 0; i < BaseAiManager.m_BaseAis.Count; i++)
            {
                if (BaseAiManager.m_BaseAis[i] != null && BaseAiManager.m_BaseAis[i].gameObject != null)
                {
                    GameObject animal = BaseAiManager.m_BaseAis[i].gameObject;
                    if (animal.GetComponent<ObjectGuid>() != null)
                    {
                        if (_guid == animal.GetComponent<ObjectGuid>().Get())
                        {
                            MelonLogger.Log("Remote harvesting of " + animal.GetComponent<ObjectGuid>().Get() + " animal");
                            if (animal.GetComponent<BodyHarvest>() != null)
                            {
                                BodyHarvest BH = animal.GetComponent<BodyHarvest>();
                                BH.m_MeatAvailableKG = BH.m_MeatAvailableKG - Harvey.m_Meat;
                                BH.m_GutAvailableUnits = BH.m_GutAvailableUnits - Harvey.m_Guts;
                                BH.m_HideAvailableUnits = BH.m_HideAvailableUnits - Harvey.m_Hide;
                            }
                            if(animal.GetComponent<AnimalUpdates>() != null)
                            {
                                AnimalUpdates AU = animal.GetComponent<AnimalUpdates>();
                                AU.CallSync();
                            }
                            break;
                        }
                    }
                }
            }
        }

        public static void DONEHARVASTING(Packet _packet)
        {
            HarvestStats Harvey = _packet.ReadHarvest();
            DoForcedHarvestAnimal(OtherHarvetingAnimal, Harvey);
        }

        public static void BULLETDAMAGE(Packet _packet)
        {
            float damage = _packet.ReadFloat();
            DamageByBullet(damage);
        }
        public static void MULTISOUND(Packet _packet)
        {
            string sound = _packet.ReadString();
            PlayMultiplayer3dAduio(sound);
        }

        public static void DoSyncContainer(ContainerOpenSync sync)
        {
            Il2CppSystem.Collections.Generic.List<Container> Boxes = ContainerManager.m_Containers;

            for (int i = 0; i < Boxes.Count; i++)
            {
                if (Boxes[i] != null && Boxes[i].GetComponent<ContainersSync>() != null && Boxes[i].GetComponent<ContainersSync>().m_Guid == sync.m_Guid)
                {
                    if (sync.m_State == true)
                    {
                        Boxes[i].GetComponent<ContainersSync>().Open();
                    }
                    else
                    {
                        Boxes[i].GetComponent<ContainersSync>().Close();
                    }
                    break;
                }
            }
        }


        public static void CONTAINEROPEN(Packet _packet)
        {
            ContainerOpenSync sync = _packet.ReadContainer();
            DoSyncContainer(sync);
        }

        public static void LUREPLACEMENT(Packet _packet)
        {
            WalkTracker sync = _packet.ReadWalkTracker();
            LastLure = sync;
        }
        public static void LUREISACTIVE(Packet _packet)
        {
            LureIsActive = _packet.ReadBool();
        }

        private static void MainThread()
        {

        }

        private static readonly List<Action> executeOnMainThread = new List<Action>();
        private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        private static bool actionToExecuteOnMainThread = false;

        Vector3 previoustickpos;
        Quaternion previoustickrot;

        Vector3 SnappedPosition(Vector3 pointToSnap, Vector3 blockCenterPosition)
        {
            Vector3 relativePosition = pointToSnap - blockCenterPosition;

            if (relativePosition.x == 0.5f) return blockCenterPosition + Vector3.right;
            else if (relativePosition.x == -0.5f) return blockCenterPosition + Vector3.left;
            else if (relativePosition.y == 0.5f) return blockCenterPosition + Vector3.up;
            else if (relativePosition.y == -0.5f) return blockCenterPosition + Vector3.down;
            else if (relativePosition.z == 0.5f) return blockCenterPosition + Vector3.forward;
            else if (relativePosition.z == -0.5f) return blockCenterPosition + Vector3.back;
            else return Vector3.zero;  // error (should not occur)
        }

        //GameManager.GetHungerComponent().RemoveReserveCalories(15f); -15 calories

        private static int OverridedHourse = 12;
        private static int OverridedMinutes = 0;
        private static string OveridedTime = "12:0";

        private void MaybeLeaveFootPrint(Vector3 footPos, bool bothFeet = false, float forceDeepFrac = 0.0f, bool leftfoot = false)
        {

            bool m_IgnoreNextFootStep = false;
            bool m_LeftFootStep = leftfoot;
            float m_FootPrintSideOffset = 0;

            if (GameManager.GetWeatherComponent().IsIndoorEnvironment())
                return;
            if (m_IgnoreNextFootStep && !bothFeet)
            {
                m_IgnoreNextFootStep = false;
            }
            else
            {
                ++footPos.y;
                Transform playerTransform = anotherbutt.transform;
                Transform vpfpsplayer = anotherbutt.transform;
                int num = !bothFeet ? 1 : 2;
                bool flip = m_LeftFootStep;
                Vector3 vector3 = playerTransform.right * (!m_LeftFootStep ? m_FootPrintSideOffset : -m_FootPrintSideOffset);

                for (int index = 0; index < num; ++index)
                {
                    Vector3 offset = playerTransform.forward * 0;
                    Vector3 heelPos = footPos - offset + vector3;
                    if (index > 0)
                        heelPos -= offset * (UnityEngine.Random.value * 0.25f);
                    Vector3 point;
                    Vector3 normal;
                    if (GameManager.GetFootstepTrailManager().IsFootprintPositionValid(heelPos, offset, 0, out point, out normal))
                    {
                        if (SnowPatchManager.m_Active)
                            GameManager.GetFootstepTrailManager().AddPlayerFootstep(vpfpsplayer.position, point, normal, playerTransform.rotation.eulerAngles.y, flip, forceDeepFrac);

                        vector3 = -vector3;
                        flip = !flip;
                    }
                }
                m_IgnoreNextFootStep = bothFeet;
            }
        }

        public void DoWeatherOverride()
        {
            Weather weatherComponent = GameManager.GetWeatherComponent();
            if (!InteriorTemperatureTrigger.m_PlayerInside)
                weatherComponent.m_IndoorTemperatureCelsius = 0; //this.m_IndoorTemperatureCelsius;
            weatherComponent.m_HighTempMinCelsius = 0; //this.m_HighTempMinCelsius;
            weatherComponent.m_HighTempMaxCelsius = 0; //this.m_HighTempMaxCelsius;
            weatherComponent.m_LowTempMinCelsius = 0; //this.m_LowTempMinCelsius;
            weatherComponent.m_LowTempMaxCelsius = 0; //this.m_LowTempMaxCelsius;
            weatherComponent.m_BlizzardDegreesDrop = 0; //this.m_BlizzardDegreesDrop;
            weatherComponent.m_BlizzardDegreesChangePerSecond = 0; //this.m_BlizzardDegreesChangePerSecond;
            weatherComponent.m_HourWarmingBegins = 0; //this.m_HourWarmingBegins;
            weatherComponent.m_HourCoolingBegins = 0; //this.m_HourCoolingBegins;
                                                      //if (this.m_AuroraEarlyWindowProbability != 0)
            weatherComponent.m_AuroraEarlyWindowProbability = 0; //this.m_AuroraEarlyWindowProbability;
            //if (this.m_AuroraLateWindowProbability != 0)
            weatherComponent.m_AuroraLateWindowProbability = 0; //this.m_AuroraLateWindowProbability;
            weatherComponent.m_DegreesPerSecondChangeLow = 0; //this.m_DegreesPerSecondChangeLow;
            weatherComponent.m_DegreesPerSecondChangeMedium = 0; //this.m_DegreesPerSecondChangeMedium;
            weatherComponent.m_DegreesPerSecondChangeHigh = 0; //this.m_DegreesPerSecondChangeHigh;
            weatherComponent.m_TimeToDisplayTempWhenChanged = 0; //this.m_TimeToDisplayTempWhenChanged;
            weatherComponent.m_MinWindSpeedForBlowingSnow = 0; //this.m_MinWindSpeedForBlowingSnow;
            weatherComponent.m_BlowingSnowTransitionSeconds = 0; //this.m_BlowingSnowTransitionSeconds;
            weatherComponent.m_SkyboxHorizonAdjust = new Vector4(); //this.m_SkyboxHorizonAdjust;
            //weatherComponent.RegisterSceneWeatherSets(new WeatherSet); //this.m_WeatherSetOverrides);
            GameManager.GetWindComponent().ApplySceneOverrides(null);
            UniStormWeatherSystem.m_MinimumFogDensityScale = 0; //this.m_MinimumFogDensityScale;
        }


        private static void UpdateHeldItem(string itemname)
        {
            if (itemname == null || anotherbutt == null)
            {
                MelonLogger.Log("Got Null");
                return;
            }
            if (level_name == "Boot" || level_name == "Empty")
            {
                MelonLogger.Log("Attempt of loading itemset when in wrong location");
                return;
            }

            MelonLogger.Log("Updating helditem with " + itemname);
            //6 - hips
            //6,8 - root
            //6,8,0 - spine1
            //6,8,0,0 - spine2
            //6,8,0,0,0 - chest
            //6,8,0,1 - back rifle
            //6,8,2,1 - back revolver
            //6,8,0,2 - Quiver
            //6,8,0,3 - Axe
            //9 - sleepingbag

            //6,8,0,0,0,1 - chest_r
            //6,8,0,0,0,1,0 - arms_r
            //6,8,0,0,0,1,0,0 - arms_r2
            //6,8,0,0,0,1,0,0,0 - hand_r

            //6,8,0,0,0,0 - chest_l
            //6,8,0,0,0,0,0 - arms_l
            //6,8,0,0,0,0,0,0 - arms_l2
            //6,8,0,0,0,0,0,0,0 - hand_l

            //6,8,1,0,0,0 - left_foot
            //6,8,2,0,0,0 - right foot

            GameObject hand_r = anotherbutt.transform.GetChild(6).GetChild(8).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;
            GameObject hand_l = anotherbutt.transform.GetChild(6).GetChild(8).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
            GameObject rifle = anotherbutt.transform.GetChild(6).GetChild(8).GetChild(0).GetChild(1).gameObject;
            GameObject revolver = anotherbutt.transform.GetChild(6).GetChild(8).GetChild(2).GetChild(1).gameObject;
            GameObject quiver = anotherbutt.transform.GetChild(6).GetChild(8).GetChild(0).GetChild(2).gameObject;
            GameObject axe = anotherbutt.transform.GetChild(6).GetChild(8).GetChild(0).GetChild(3).gameObject;
            GameObject medkit = anotherbutt.transform.GetChild(6).GetChild(8).GetChild(0).GetChild(4).gameObject;

            if (HasRifle == true && LightSourceName != "GEAR_Rifle")
            {
                rifle.SetActive(true);
            }
            else
            {
                rifle.SetActive(false);
            }
            if (HasRevolver == true && LightSourceName != "GEAR_Revolver")
            {
                revolver.SetActive(true);
            }
            else
            {
                revolver.SetActive(false);
            }

            axe.SetActive(HasAxe);
            medkit.SetActive(HasMedkit);

            if (HisArrows > 0)
            {
                quiver.SetActive(true);
                quiver.transform.GetChild(0).gameObject.SetActive(false);
                quiver.transform.GetChild(1).gameObject.SetActive(false);
                quiver.transform.GetChild(2).gameObject.SetActive(false);
                quiver.transform.GetChild(3).gameObject.SetActive(false);
                quiver.transform.GetChild(4).gameObject.SetActive(false);
                for (int i = 0; i < 4; i++)
                {
                    quiver.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
            else
            {
                quiver.SetActive(false);
            }

            if (itemname != "nodis")
            {
                //Flare
                hand_r.transform.GetChild(0).gameObject.SetActive(false);
                //Matches
                hand_r.transform.GetChild(1).gameObject.SetActive(false); hand_l.transform.GetChild(0).gameObject.SetActive(false);
                //Flare blue
                hand_r.transform.GetChild(2).gameObject.SetActive(false);
                //Rifle
                hand_r.transform.GetChild(3).gameObject.SetActive(false);
                //Revolver
                hand_r.transform.GetChild(4).gameObject.SetActive(false);
                //SprayCan
                hand_r.transform.GetChild(5).gameObject.SetActive(false);
                //Stone 
                hand_r.transform.GetChild(6).gameObject.SetActive(false);
                // Lamp
                hand_r.transform.GetChild(7).gameObject.SetActive(false);
                //Tourch
                hand_r.transform.GetChild(8).gameObject.SetActive(false);
                //Flaregun
                hand_r.transform.GetChild(9).gameObject.SetActive(false);
                //Map
                hand_l.transform.GetChild(2).gameObject.SetActive(false);
                //Bow
                hand_l.transform.GetChild(3).gameObject.SetActive(false);
            }
            if (itemname.StartsWith("GEAR_Flare") != itemname.Contains("Gun")) // By some reason, it called GEAR_FlareA, i not know exist FlareB but better check it.
            {
                hand_r.transform.GetChild(0).gameObject.SetActive(true);
            }
            if (itemname == "GEAR_WoodMatches" || itemname == "GEAR_PackMatches")
            {
                hand_r.transform.GetChild(1).gameObject.SetActive(true); hand_l.transform.GetChild(0).gameObject.SetActive(true);
            }
            if (itemname == "GEAR_BlueFlare")
            {
                hand_r.transform.GetChild(2).gameObject.SetActive(true);
            }
            if (itemname == "GEAR_Rifle")
            {
                hand_r.transform.GetChild(3).gameObject.SetActive(true);
            }
            if (itemname == "GEAR_Revolver")
            {
                hand_r.transform.GetChild(4).gameObject.SetActive(true);
            }
            if (itemname.StartsWith("GEAR_SprayPaintCanGlyph")) // By some reason, it called GEAR_SprayPaintCanGlyphA, i not know exist GEAR_SprayPaintCanGlyphB but better check it.
            {
                hand_r.transform.GetChild(5).gameObject.SetActive(true);
            }
            if (itemname == "GEAR_Stone")
            {
                hand_r.transform.GetChild(6).gameObject.SetActive(true);
            }
            if (itemname.StartsWith("GEAR_KeroseneLamp"))
            {
                hand_r.transform.GetChild(7).gameObject.SetActive(true);
            }
            if (itemname == "GEAR_Torch")
            {
                hand_r.transform.GetChild(8).gameObject.SetActive(true);
            }
            if (itemname == "GEAR_Bow")
            {
                hand_l.transform.GetChild(3).gameObject.SetActive(true);
            }
            if (itemname == "Map")
            {
                hand_l.transform.GetChild(2).gameObject.SetActive(true);
            }
            if (itemname == "GEAR_FlareGun")
            {
                hand_r.transform.GetChild(9).gameObject.SetActive(true);
            }

            //Additionals
            if (LightSource == true)
            {
                hand_r.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); //Flare cap
                hand_r.transform.GetChild(0).GetChild(2).gameObject.SetActive(true); //Flare fx
                hand_r.transform.GetChild(1).GetChild(0).gameObject.SetActive(false); // Matche cap normal
                hand_r.transform.GetChild(1).GetChild(1).gameObject.SetActive(true); // Matche cap burt
                hand_r.transform.GetChild(1).GetChild(3).gameObject.SetActive(true); // Matche fire
                hand_r.transform.GetChild(2).GetChild(1).gameObject.SetActive(false); //Blue Flare cap
                hand_r.transform.GetChild(2).GetChild(2).gameObject.SetActive(true); //Blue Flare fx
                hand_r.transform.GetChild(7).GetChild(0).gameObject.SetActive(true); //Lamp Fire
                hand_r.transform.GetChild(8).GetChild(1).gameObject.SetActive(true); //Tourch Fire
            }
            else
            {
                hand_r.transform.GetChild(0).GetChild(1).gameObject.SetActive(true); //Flare cap
                hand_r.transform.GetChild(0).GetChild(2).gameObject.SetActive(false); //Flare fx
                hand_r.transform.GetChild(1).GetChild(0).gameObject.SetActive(true); // Matche cap normal
                hand_r.transform.GetChild(1).GetChild(1).gameObject.SetActive(false); // Matche cap burt
                hand_r.transform.GetChild(1).GetChild(3).gameObject.SetActive(false); // Matche fire
                hand_r.transform.GetChild(2).GetChild(1).gameObject.SetActive(true); //Blue Flare cap
                hand_r.transform.GetChild(2).GetChild(2).gameObject.SetActive(false); //Blue Flare fx
                hand_r.transform.GetChild(7).GetChild(0).gameObject.SetActive(false); //Lamp Fire
                hand_r.transform.GetChild(8).GetChild(1).gameObject.SetActive(false); //Tourch Fire
            }

            //Lights
            hand_r.transform.GetChild(0).GetChild(3).gameObject.SetActive(LightSource); //Flare Red Light
            hand_r.transform.GetChild(1).GetChild(4).gameObject.SetActive(LightSource); //Matche Light
            hand_r.transform.GetChild(2).GetChild(3).gameObject.SetActive(LightSource); //Flare Blue Light
            hand_r.transform.GetChild(7).GetChild(3).gameObject.SetActive(LightSource); //Lamp Light
            hand_r.transform.GetChild(8).GetChild(0).gameObject.SetActive(LightSource); //Tourch Light
        }

        public static float OverridenStartCountDown = 900f;

        private static void EverySecond()
        {
            if(iAmHost == true && ServerHandle.OverflowAnimalsOnConnectTimer > 0)
            {
                MaxAnimalsSyncNeed = MaxAnimalsSyncCountOnConnect;
                ServerHandle.OverflowAnimalsOnConnectTimer = ServerHandle.OverflowAnimalsOnConnectTimer - 1;
            }else
            {
                MaxAnimalsSyncNeed = MaxAnimalsSyncCount;
            }

            if (sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.CARRYBODY))
                {
                    _packet.Write(CarryingPlayer);
                    SendTCPData(_packet);
                }
            }
            if (iAmHost == true)
            {
                using (Packet _packet = new Packet((int)ServerPackets.CARRYBODY))
                {
                    ServerSend.CARRYBODY(1, CarryingPlayer);
                }
            }

            if (IamShatalker == true && anotherbutt != null)
            {
                Outline OL = anotherbutt.GetComponent<Outline>();
                if (OL == null)
                {
                    anotherbutt.AddComponent<Outline>();
                    OL = anotherbutt.GetComponent<Outline>();
                    OL.m_OutlineColor = Color.green;
                    OL.needsUpdate = true;
                }

                if (LureIsActive == true)
                {
                    if (OL != null)
                    {
                        if(OL.m_OutlineColor != Color.clear)
                        {
                            OL.m_OutlineColor = Color.clear;
                            OL.needsUpdate = true;
                        }
                    }
                    anotherbutt.SetActive(false);
                }else{
                    if (OL != null)
                    {
                        if (OL.m_OutlineColor != Color.green)
                        {
                            OL.m_OutlineColor = Color.green;
                            OL.needsUpdate = true;
                        }
                    }
                    anotherbutt.SetActive(true);
                }
            }

            if (IsShatalkerMode() == true)
            {
                if (IamShatalker == false && ShatalkerObject != null)
                {
                    if (ShatalkerObject.GetStartMovementDelayTime() > 1)
                    {
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.DWCOUNTDOWN))
                            {
                                _packet.Write(ShatalkerObject.GetStartMovementDelayTime());
                                SendTCPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.DWCOUNTDOWN))
                            {
                                ServerSend.DWCOUNTDOWN(1, ShatalkerObject.GetStartMovementDelayTime());
                            }
                        }
                    }

                    if (WardWidget != null)
                    {
                        WardIsActive = WardWidget.activeSelf;
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.WARDISACTIVE))
                            {
                                _packet.Write(WardIsActive);
                                SendTCPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.WARDISACTIVE))
                            {
                                ServerSend.WARDISACTIVE(1, WardIsActive);
                            }
                        }
                    }
                    if (LureWidget != null)
                    {
                        LureIsActive = LureWidget.activeSelf;
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.LUREISACTIVE))
                            {
                                _packet.Write(LureIsActive);
                                SendTCPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.LUREISACTIVE))
                            {
                                ServerSend.LUREISACTIVE(1, LureIsActive);
                            }
                        }
                    }
                }
            }
        }

        private static void EveryInGameMinute()
        {
            OverridedMinutes = OverridedMinutes + 1;
            if (OverridedMinutes > 59)
            {
                OverridedMinutes = 0;
                OverridedHourse = OverridedHourse + 1;
            }
            if (OverridedHourse > 23)
            {
                OverridedHourse = 0;
            }

            OveridedTime = OverridedHourse + ":" + OverridedMinutes;

            //MelonLogger.Log("Seed "+ GameManager.m_SceneTransitionData.m_GameRandomSeed);

            ServerHandle.SetGameTime(OveridedTime);

            using (Packet _packet = new Packet((int)ServerPackets.GAMETIME))
            {
                ServerSend.GAMETIME(1, OveridedTime);
            }

            if (level_name != "Boot" && level_name != "Empty" && GameManager.m_Wind != null && GameManager.m_Wind.m_ActiveSettings != null && GameManager.m_Weather != null && GameManager.m_WeatherTransition != null)
            {
                using (Packet _packet = new Packet((int)ServerPackets.SYNCWEATHER))
                {
                    WeatherProxies weather = new WeatherProxies();
                    weather.m_WeatherProxy = GameManager.GetWeatherComponent().Serialize();
                    weather.m_WeatherTransitionProxy = GameManager.GetWeatherTransitionComponent().Serialize();
                    weather.m_WindProxy = GameManager.GetWindComponent().Serialize();

                    ServerSend.SYNCWEATHER(1, weather);
                }
            }
            else{
                //MelonLogger.Log("Can't send wind sync.");
            }
        }

        public static float nextActionTime = 0.0f;
        public float period = 5f;
        public static float nextActionTimeAniamls = 0.0f;
        public float periodAniamls = 0.3f;
        public float nextActionTimeSecond = 0.0f;
        public float periodSecond = 1f;

        public bool DeathTrigger = false;

        public static bool IsShatalkerMode() // This is darlkwalker mode active.
        {
            if (IamShatalker == true || InDarkWalkerMode == true) // If you are darkwalker
            {
                return true;
            } else
            {
                if (ShatalkerModeClient == false && ServerHandle.ReturnShatalkerMode() == false) // If other player host/client is darkwalker
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool InOnline() // If you are host or client
        {
            if (iAmHost == true || sendMyPosition == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //WILDLIFE_Wolf
        //WILDLIFE_Wolf_Aurora
        //WILDLIFE_Bear
        //WILDLIFE_Bear_Aurora
        //WILDLIFE_Stag
        //WILDLIFE_Rabbit
        //WILDLIFE_Moose

        public static void WriteDownMesh(GameObject animal)
        {

            SkinnedMeshRenderer mesh1 = null;
            SkinnedMeshRenderer mesh2 = null;
            SkinnedMeshRenderer mesh3 = null;

            if (animal.name.StartsWith("WILDLIFE_Wolf"))
            {
                //7 Rig, Meshs 12,13
                mesh1 = animal.transform.GetChild(7).GetChild(12).gameObject.GetComponent<SkinnedMeshRenderer>();
                mesh2 = animal.transform.GetChild(7).GetChild(13).gameObject.GetComponent<SkinnedMeshRenderer>();
            }
            else if (animal.name.StartsWith("WILDLIFE_Stag"))
            {
                //23 Mesh
                mesh1 = animal.transform.GetChild(23).gameObject.GetComponent<SkinnedMeshRenderer>();
            }

            else if (animal.name.StartsWith("WILDLIFE_Rabbit"))
            {
                // 6,7 Meshs
                mesh1 = animal.transform.GetChild(6).gameObject.GetComponent<SkinnedMeshRenderer>();
                mesh2 = animal.transform.GetChild(7).gameObject.GetComponent<SkinnedMeshRenderer>();
            }
            else if (animal.name.StartsWith("WILDLIFE_Moose"))
            {
                // 24,25 Meshs
                mesh1 = animal.transform.GetChild(24).gameObject.GetComponent<SkinnedMeshRenderer>();
                mesh2 = animal.transform.GetChild(25).gameObject.GetComponent<SkinnedMeshRenderer>();
            }
            else if (animal.name.StartsWith("WILDLIFE_Bear"))
            {
                // 10,11,12 Meshs
                mesh1 = animal.transform.GetChild(10).gameObject.GetComponent<SkinnedMeshRenderer>();
                mesh2 = animal.transform.GetChild(11).gameObject.GetComponent<SkinnedMeshRenderer>();
                mesh3 = animal.transform.GetChild(12).gameObject.GetComponent<SkinnedMeshRenderer>();
            }

            AnimalUpdates au = animal.GetComponent<AnimalUpdates>();

            if (mesh1 != null){if(au != null){au.m_Mesh1 = mesh1;}}
            if (mesh2 != null){if(au != null){au.m_Mesh2 = mesh2;}}
            if (mesh3 != null){if(au != null){au.m_Mesh3 = mesh3;}}
        }

        public static string GetAnimalPrefabName(string _name)
        {
            if (_name.StartsWith("WILDLIFE_Wolf"))
            {
                if (_name.StartsWith("WILDLIFE_Wolf_Aurora"))
                {
                    return "WILDLIFE_Wolf_Aurora";
                }else{
                    if (_name.StartsWith("WILDLIFE_Wolf_grey_aurora"))
                    {
                        return "WILDLIFE_Wolf_grey_aurora";
                    }else{
                        if (_name.StartsWith("WILDLIFE_Wolf_grey"))
                        {
                            return "WILDLIFE_Wolf_grey";
                        }else{
                            return "WILDLIFE_Wolf";
                        }
                    }
                }
            }

            if (_name.StartsWith("WILDLIFE_Bear"))
            {
                if (_name.StartsWith("WILDLIFE_Bear_Aurora"))
                {
                    return "WILDLIFE_Bear_Aurora";
                }
                else
                {
                    return "WILDLIFE_Bear";
                }
            }

            if (_name.StartsWith("WILDLIFE_Stag"))
            {
                return "WILDLIFE_Stag";
            }
            if (_name.StartsWith("WILDLIFE_Rabbit"))
            {
                return "WILDLIFE_Rabbit";
            }
            if (_name.StartsWith("WILDLIFE_Moose"))
            {
                return "WILDLIFE_Moose";
            }
            else{
                return "WILDLIFE_Wolf";
            }
        }

        public static void ReCreateAnimal(GameObject animal, string proxy = "")
        {
            if (animal == null)
            {
                return;
            }
            animal.SetActive(false);
            Vector3 pos = animal.transform.position;
            Quaternion rotation = animal.transform.rotation;
            string _GUID = animal.GetComponent<ObjectGuid>().Get();
            string prefab = GetAnimalPrefabName(animal.name);
            MelonLogger.Log("Trying re-create animal " + prefab + " " + _GUID);
            AnimalUpdates AU = animal.GetComponent<AnimalUpdates>();

            if(animal.GetComponent<BaseAi>().m_CurrentHP == 0 || AU.m_Hp == 0 && AU.AP_AiState == 2 || animal.GetComponent<Harvestable>() != null && (animal.GetComponent<Harvestable>().enabled == true))
            {
                MelonLogger.Log("Recreation canceled");
                return;
            }

            if(AU != null)
            {
                AU.m_Banned = true;
            }

            string JsonProx = proxy;

            if(JsonProx == "")
            {
                JsonProx = animal.GetComponent<BaseAi>().Serialize();
            }
            UnityEngine.Object.Destroy(animal);
            SpawnAnimal(prefab, pos, _GUID, true, JsonProx);
            return;
        }
        public static void ReCreateAnimal_test_v2(GameObject animal, string proxy = "")
        {
            if (animal == null)
            {
                return;
            }
            MakeAnimalActive(animal, true);
            return;
        }

        public static void MakeAnimalActive(GameObject animal, bool active)
        {
            //MelonLogger.Log("Nachinayem kuhat");
            if (animal == null)
            {
                return;
            }

            //MelonLogger.Log("Animal narmalna");
            if (animal.transform.parent != null && animal.transform.parent.GetComponent<MoveAgent>() != null)
            {
                //UnityEngine.Component.Destroy(animal.transform.parent.GetComponent<MoveAgent>());
                if(active == true)
                {
                    BaseAiManager.CreateMoveAgent(animal.transform, animal.GetComponent<BaseAi>(), animal.transform.position);
                }
                animal.transform.parent.GetComponent<MoveAgent>().enabled = active;
                if (active == true)
                {
                    BaseAiManager.CreateMoveAgent(animal.transform, animal.GetComponent<BaseAi>(), animal.transform.position);
                }
                //MelonLogger.Log("[MoveAgent]-> off");
            }
            if (animal.transform.parent != null && animal.transform.parent.GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
            {
                animal.transform.parent.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = active;
            }
            //MelonLogger.Log("MoveAgent nevonayet");
            if (animal.GetComponent<AiTarget>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<AiTarget>());
                animal.GetComponent<AiTarget>().enabled = active;
                //MelonLogger.Log("[AiTarget]-> off");
            }
            //MelonLogger.Log("AiTarget vkusna");
            if (animal.GetComponent<AiWolf>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<AiWolf>());
                animal.GetComponent<AiWolf>().enabled = active;
                //MelonLogger.Log("[AiWolf]-> off");
            }
            //MelonLogger.Log("AiWolf ogon");
            if (animal.GetComponent<AiStag>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<AiStag>());
                animal.GetComponent<AiStag>().enabled = active;
                //MelonLogger.Log("[AiStag]-> off");
            }
            //MelonLogger.Log("AiStag alright");
            if (animal.GetComponent<AiRabbit>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<AiRabbit>());
                animal.GetComponent<AiRabbit>().enabled = active;
                //MelonLogger.Log("[AiRabbit]-> off");
            }
            //MelonLogger.Log("AiRabbit yahooo");
            if (animal.GetComponent<AiMoose>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<AiMoose>());
                animal.GetComponent<AiMoose>().enabled = active;
                //MelonLogger.Log("[AiMoose]-> off");
            }
            //MelonLogger.Log("AiMoose DA");
            if (animal.GetComponent<AiBear>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<AiBear>());
                animal.GetComponent<AiBear>().enabled = active;
                //MelonLogger.Log("[AiBear]-> off");
            }
            //MelonLogger.Log("AiBear PIVO");
            if (animal.GetComponent<CharacterController>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<CharacterController>());
                animal.GetComponent<CharacterController>().enabled = active;
                //MelonLogger.Log("[CharacterController]-> off");
            }
            //MelonLogger.Log("CharacterController ANUS SEBE CONTROLIRUI");
            if (animal.GetComponent<NodeCanvas.Framework.Blackboard>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<NodeCanvas.Framework.Blackboard>());
                animal.GetComponent<NodeCanvas.Framework.Blackboard>().enabled = active;
                //MelonLogger.Log("[NodeCanvas.Framework.Blackboard]-> off");
            }
            //MelonLogger.Log("Blackboard DA BECAUSE DA");
            if (animal.GetComponent<TLDBehaviourTreeOwner>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<TLDBehaviourTreeOwner>());
                animal.GetComponent<TLDBehaviourTreeOwner>().enabled = active;
                //MelonLogger.Log("TLDBehaviourTreeOwner]-> off");
            }
            //MelonLogger.Log("TLDBehaviourTreeOwner ZDRAVSVUI DEREVO");
            if (animal.GetComponent<BaseAi>() != null)
            {
                //UnityEngine.Component.Destroy(animal.GetComponent<BaseAi>());
                animal.GetComponent<BaseAi>().enabled = active;

                //if(active == true)
                //{
                //    BaseAi _AI = animal.GetComponent<BaseAi>();
                //    Il2CppSystem.Collections.Generic.List<SpawnRegion> Regions = GameManager.GetSpawnRegionManager().m_SpawnRegions;

                //    float RegionDistence = 0;
                //    SpawnRegion NearestSpawnRegion = null;
                //    MelonLogger.Log("Trying get nearest spawn region to get waypoints from...");

                //    for (int i = 0; i < Regions.Count; i++)
                //    {
                //        if (Regions[i] != null)
                //        {
                //            float CurrentDistance = Vector3.Distance(animal.transform.position, Regions[i].transform.position);

                //            if (RegionDistence < CurrentDistance)
                //            {
                //                RegionDistence = CurrentDistance;
                //                NearestSpawnRegion = Regions[i];
                //            }
                //        }
                //    }

                //    if(NearestSpawnRegion != null)
                //    {
                //        MelonLogger.Log("Found nearest spawn region!");
                //        _AI.SetWaypoints(NearestSpawnRegion.GetWaypointCircuit());
                //    }
                //}

                //animal.GetComponent<BaseAi>().SetWaypoints();
            }
            //MelonLogger.Log("BaseAi NE SUS");
            if(animal.GetComponent<AnimalUpdates>() != null)
            {
                AnimalUpdates au = animal.GetComponent<AnimalUpdates>();
                au.nextActionDampingOn = Time.time+au.dampingOn_perioud;
                au.m_DampingIgnore = true;
            }
        }

        public static void SpawnAnimal(string prefabName, Vector3 v3spawn, string _guid, bool recreateion = false, string prox = "")
        {
            for (int index = 0; index < BaseAiManager.m_BaseAis.Count; ++index)
            {
                GameObject checkanimal = BaseAiManager.m_BaseAis[index].gameObject;
                if (checkanimal != null && checkanimal.GetComponent<ObjectGuid>() != null)
                {
                    if(checkanimal.GetComponent<ObjectGuid>().Get() == _guid && checkanimal.GetComponent<AnimalUpdates>() && checkanimal.GetComponent<AnimalUpdates>().m_Banned == false)
                    {
                        MelonLogger.Log("Can't create animal, " + _guid + " cause such one exist. ");
                        return;
                    }
                }
            }

            //UnityEngine.Object original = Resources.Load(prefabName);
            //GameObject animal = UnityEngine.Object.Instantiate(original) as GameObject;

            GameObject animal = UnityEngine.Object.Instantiate<GameObject>(Resources.Load(prefabName).Cast<GameObject>());
            animal.name = prefabName;
            animal.transform.position = v3spawn;

            //MoveAgent AG = animal.transform.parent.GetComponent<MoveAgent>();
            //AG.m_Target

            BaseAi _AI = animal.GetComponent<BaseAi>();
            _AI.m_SpawnPos = v3spawn;

            //_AI.m_SpawnRegionParent.InstantiateSpawn

            if (animal.GetComponent<ObjectGuid>() != null)
            {
                animal.GetComponent<ObjectGuid>().m_Guid = _guid;
            }
            else
            {
                animal.AddComponent<ObjectGuid>();
                animal.GetComponent<ObjectGuid>().m_Guid = _guid;
            }

            AnimalUpdates au = animal.GetComponent<AnimalUpdates>();

            if (au == null)
            {
                animal.AddComponent<AnimalUpdates>();

                au = animal.GetComponent<AnimalUpdates>();
            }
            au.m_ToGo = v3spawn;

            if (recreateion == true)
            {
                if(prox != "")
                {
                    //MelonLogger.Log("Spawn Recreation deserialize " + prox);
                    _AI.Deserialize(prox);
                }

                if (AnimalsController == false)
                {
                    au.m_UnderMyControl = true;
                    au.m_InActive = false;
                }
                else
                {
                    au.m_ClientControlled = false;
                    au.m_InActive = false;
                }
            }
            au.m_Animal = animal;
            au.m_RemoteSpawned = true;
            animal.transform.position = v3spawn;
            au.m_RemoteSpawned = true;
            au.m_RightName = prefabName;

            BaseAiManager.CreateMoveAgent(animal.transform, _AI, v3spawn);

            MelonLogger.Log(animal.GetComponent<ObjectGuid>().Get() + " Created " + prefabName);
        }

        public static void SetAnimalControllerRole()
        {
            if (IsShatalkerMode() == true)
            {
                return;
            }

            bool controller = false;
            bool mycontroller = false;

            if (levelid != anotherplayer_levelid)
            {
                controller = true;
                mycontroller = true;
            }else {
                if (levelid == anotherplayer_levelid)
                {
                    if (MyTicksOnScene > TicksOnScene)
                    {
                        controller = false;
                        mycontroller = true;
                    } else {
                        if (MyTicksOnScene < TicksOnScene)
                        {
                            controller = true;
                            mycontroller = false;
                        } else {
                            if (MyTicksOnScene == TicksOnScene)  //Wot? This almsot not possible...but need to check this anyway.
                            {
                                controller = false;
                                mycontroller = true;
                            }
                        }
                    }

                    if (mycontroller == true && IsDead == true)
                    {
                        mycontroller = false;
                        controller = true;
                    }

                    if (controller == true && AnimState == "Knock")
                    {
                        mycontroller = true;
                        controller = false;
                    }
                }
            }

            AnimalsController = mycontroller;

            AllowSpawnAnimals(AnimalsController);


            using (Packet _packet = new Packet((int)ServerPackets.ANIMALROLE))
            {
                ServerSend.ANIMALROLE(1, controller);
            }
        }

        public static GameObject WaitingRoom = null;
        public static Vector3 ReturnFromWaitngRoomV3 = new Vector3(0, 0, 0);
        public static List<GameObject> WaitingRoomGears = new List<GameObject>();
        public static bool InWaitingRoom = false;

        public static void SendToWaitngRoom()
        {
            Vector3 pV3 = GameManager.GetPlayerTransform().position;
            Vector3 spawn_V3 = new Vector3(pV3.x, pV3.y + 1000, pV3.z);
            GameObject room = null;

            if (WaitingRoom == null)
            {
                GameObject RoomPrefab = LoadedBundle.LoadAsset<GameObject>("WaitRoom");
                room = GameObject.Instantiate(RoomPrefab);
                room.transform.position = spawn_V3;
                WaitingRoom = room;
                ReturnFromWaitngRoomV3 = GameManager.GetPlayerTransform().position;
                GameManager.GetPlayerManagerComponent().TeleportPlayer(room.transform.GetChild(3).position, GameManager.GetMainCamera().transform.rotation);
            } else
            {
                room = WaitingRoom;
                if (ReturnFromWaitngRoomV3 != new Vector3(0, 0, 0))
                {
                    ReturnFromWaitngRoomV3 = GameManager.GetPlayerTransform().position;
                }
                GameManager.GetPlayerManagerComponent().TeleportPlayer(room.transform.GetChild(3).position, GameManager.GetMainCamera().transform.rotation);
            }

            InWaitingRoom = true;


            for (int i = 0; i < WaitingRoomGears.Count; i++)
            {
                if (WaitingRoomGears[i] != null)
                {
                    if (GameManager.GetPlayerManagerComponent().m_ItemInHands != null && GameManager.GetPlayerManagerComponent().m_ItemInHands == WaitingRoomGears[i].GetComponent<GearItem>())
                    {
                        MelonLogger.Log("Waiting room item was in hands, trying unequip");
                        GameManager.GetPlayerManagerComponent().UseInventoryItem(WaitingRoomGears[i].gameObject.GetComponent<GearItem>());
                    }
                    UnityEngine.Object.Destroy(WaitingRoomGears[i]);
                }
            }

            int cans = 51;

            for (int i = 0; i < cans; i++)
            {
                Vector3 can_spawnV3 = room.transform.GetChild(2).GetChild(i).position;

                GameObject gear = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("GEAR_RecycledCan"));
                gear.transform.position = can_spawnV3;
                WaitingRoomGears.Add(gear);
            }

            int ammoboxes = 5;

            for (int i = 0; i < ammoboxes; i++)
            {
                Vector3 can_spawnV3 = room.transform.GetChild(5).GetChild(i).position;

                GameObject gear = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("GEAR_RifleAmmoBox"));
                gear.transform.position = can_spawnV3;
                WaitingRoomGears.Add(gear);
            }

            GameObject rifle = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("GEAR_Rifle"));
            rifle.transform.position = room.transform.GetChild(4).position;
            rifle.GetComponent<GearItem>().m_CurrentHP = rifle.GetComponent<GearItem>().m_MaxHP;
            rifle.GetComponent<GearItem>().m_GunItem.m_FiringRateSeconds = 0.1f;
            rifle.GetComponent<GearItem>().m_GunItem.m_ClipSize = 100;
            rifle.GetComponent<GearItem>().m_GunItem.m_RoundsInClip = 100;
            //rifle.GetComponent<GearItem>().m_GunItem.m_AllowHipFire = true;
            //rifle.GetComponent<GearItem>().m_GunItem.m_MultiplierReload = 1;
            //rifle.GetComponent<GearItem>().m_GunItem.m_ReloadCoolDownSeconds = 0.1f;
            //rifle.GetComponent<GearItem>().m_GunItem.m_YawRecoilMax = 0.1f;
            //rifle.GetComponent<GearItem>().m_GunItem.m_YawRecoilMin = 0.1f;
            //rifle.GetComponent<GearItem>().m_GunItem.m_PitchRecoilMax = 0.1f;
            //rifle.GetComponent<GearItem>().m_GunItem.m_PitchRecoilMin = 0.1f;
            WaitingRoomGears.Add(rifle);
        }

        public static void SendToAnimalRoom()
        {
            Vector3 pV3 = GameManager.GetPlayerTransform().position;
            Vector3 spawn_V3 = new Vector3(pV3.x, pV3.y + 1000, pV3.z);
            

            GameObject RoomPrefab = LoadedBundle.LoadAsset<GameObject>("WaitRoom");
            GameObject room = GameObject.Instantiate(RoomPrefab); 
            room.transform.position = spawn_V3;
            GameManager.GetPlayerManagerComponent().TeleportPlayer(room.transform.GetChild(3).position, GameManager.GetMainCamera().transform.rotation);

            Vector3 v3AnimalSpawn = room.transform.GetChild(4).position;
            GameObject animal = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("WILDLIFE_WOLF").Cast<GameObject>());
            MakeAnimalActive(animal, false);
            animal.transform.position = v3AnimalSpawn;
            animal.AddComponent<AnimalUpdates>();
            AnimalUpdates au = animal.GetComponent<AnimalUpdates>();
            au.m_Animal = animal;
            animal.name = animal.name + "_DEBUG";
            au.m_ToGo = v3AnimalSpawn;
            animal.transform.position = v3AnimalSpawn;
            au.m_RemoteSpawned = true;
            au.m_DebugAnimal = true;
            if (animal.GetComponent<ObjectGuid>() == null)
            {
                animal.AddComponent<ObjectGuid>();
                animal.GetComponent<ObjectGuid>().m_Guid = ObjectGuidManager.GenerateNewGuidString();
            }
            MelonLogger.Log("Created DEBUG ANIMAL");
        }

        public static void DestoryWaitingRoom()
        {
            for (int i = 0; i < WaitingRoomGears.Count; i++)
            {
                if (WaitingRoomGears[i] != null)
                {
                    if (GameManager.GetPlayerManagerComponent().m_ItemInHands != null && GameManager.GetPlayerManagerComponent().m_ItemInHands == WaitingRoomGears[i].GetComponent<GearItem>())
                    {
                        MelonLogger.Log("Waiting room item was in hands, trying unequip");
                        GameManager.GetPlayerManagerComponent().UseInventoryItem(WaitingRoomGears[i].gameObject.GetComponent<GearItem>());
                    }
                    UnityEngine.Object.Destroy(WaitingRoomGears[i]);
                }
            }
            if (WaitingRoom != null)
            {
                UnityEngine.Object.Destroy(WaitingRoom);
            }
        }

        public static void ReturnFromWaitingRoom()
        {
            if (ReturnFromWaitngRoomV3 != new Vector3(0, 0, 0))
            {
                DestoryWaitingRoom();
                GameManager.GetPlayerManagerComponent().TeleportPlayer(ReturnFromWaitngRoomV3, GameManager.GetMainCamera().transform.rotation);
                InWaitingRoom = false;
                ReturnFromWaitngRoomV3 = new Vector3(0, 0, 0);
            }
        }

        public string mWeatherProxy = "";
        public string mWeatherTransitionProxy = "";
        public string mWindProxy = "";

        [HarmonyPatch(typeof(GameManager), "Start")]
        internal class GameManager_Seed
        {
            public static void Postfix(GameManager __instance)
            {
                //GameManager.m_SceneTransitionData.m_GameRandomSeed = 48874887;
            }
        }

        public class SaveSlotSync : MelonMod
        {
            public int m_Episode;
            public int m_SaveSlotType;
            public int m_Seed;
            public int m_ExperienceMode;
            public int m_Location;
        }

        public static SaveSlotSync PendingSave = null;
        public static bool OverrideMenusForConnection = false;

        public static void SAVEDATA(Packet _packet)
        {
            SaveSlotSync SaveData = _packet.ReadSaveSlot();

            if (InterfaceManager.IsMainMenuActive() == false)
            {
                return;
            }

            PendingSave = SaveData;

            CheckHaveSaveFileToJoin(SaveData);

            //ForcedCreateSave(SaveData);
        }

        public static void SendSlotData()
        {
            SaveSlotSync SaveData = new SaveSlotSync();
            SaveData.m_Episode = (int) SaveGameSystem.m_CurrentEpisode;
            SaveData.m_SaveSlotType = (int) SaveGameSystem.m_CurrentGameMode;
            SaveData.m_Seed = GameManager.m_SceneTransitionData.m_GameRandomSeed;
            SaveData.m_ExperienceMode = (int) ExperienceModeManager.s_CurrentModeType;
            SaveData.m_Location = (int) RegionManager.GetCurrentRegion();

            using (Packet __packet = new Packet((int)ServerPackets.SAVEDATA))
            {
                ServerSend.SAVEDATA(1, SaveData);
            }
        }

        public static void SelectBagesForConnection()
        {
            GameAudioManager.PlayGuiConfirm();

            if(PendingSave.m_SaveSlotType == (int)SaveSlotType.SANDBOX && InterfaceManager.m_Panel_MainMenu.GetNumUnlockedFeats() > 0)
            {
                InterfaceManager.m_Panel_MainMenu.SelectWindow(InterfaceManager.m_Panel_MainMenu.m_SelectFeatWindow);
            }else{
                ForcedCreateSave(PendingSave);
            }
        }

        public static void SelectGenderForConnection()
        {
            InterfaceManager.m_Panel_SelectSurvivor.Enable(true);

            InterfaceManager.m_Panel_SelectSurvivor.m_BasicMenu.m_OnClickBackAction = null;
        }

        
        [HarmonyPatch(typeof(Panel_MainMenu), "OnSelectFeatsBack")]
        internal class Panel_MainMenu_FeatBack
        {
            public static bool Prefix(Panel_MainMenu __instance)
            {
                if (OverrideMenusForConnection == true)
                {
                    SelectGenderForConnection();
                    return false;
                }

                return true;
            }
        }
        [HarmonyPatch(typeof(Panel_MainMenu), "OnSelectFeatsContinue")]
        internal class Panel_MainMenu_FeatContinue
        {
            public static bool Prefix(Panel_MainMenu __instance)
            {
                if (OverrideMenusForConnection == true)
                {
                    GameAudioManager.PlayGuiConfirm();
                    FeatEnabledTracker.m_FeatsEnabledThisSandbox = new Il2CppSystem.Collections.Generic.List<FeatType>();
                    for (int index1 = 0; index1 < __instance.m_SelectedFeats.Count; ++index1)
                    {
                        for (int index2 = 0; index2 < GameManager.GetFeatsManager().GetNumFeats(); ++index2)
                        {
                            Feat featFromIndex = GameManager.GetFeatsManager().GetFeatFromIndex(index2);
                            if (__instance.m_SelectedFeats[index1] == featFromIndex.m_LocalizedDisplayName.m_LocalizationID)
                            {
                                FeatEnabledTracker.m_FeatsEnabledThisSandbox.Add(featFromIndex.m_FeatType);
                            }
                        }
                    }
                    ForcedCreateSave(PendingSave);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Panel_SelectSurvivor), "OnSelectSurvivorMale")]
        internal class Panel_SelectSurvivor_Select1
        {
            public static bool Prefix(Panel_SelectSurvivor __instance)
            {
                if(OverrideMenusForConnection == true)
                {
                    InterfaceManager.m_Panel_OptionsMenu.m_State.m_VoicePersona = VoicePersona.Male;
                    __instance.Enable(false);
                    InterfaceManager.m_Panel_MainMenu.SendMessage("Update");
                    SelectBagesForConnection();

                    return false;
                }

                return true;
            }
        }
        [HarmonyPatch(typeof(Panel_SelectSurvivor), "OnSelectSurvivorFemale")]
        internal class Panel_SelectSurvivor_Select2
        {
            public static bool Prefix(Panel_SelectSurvivor __instance)
            {
                if (OverrideMenusForConnection == true)
                {
                    InterfaceManager.m_Panel_OptionsMenu.m_State.m_VoicePersona = VoicePersona.Female;
                    __instance.Enable(false);
                    InterfaceManager.m_Panel_MainMenu.SendMessage("Update");
                    SelectBagesForConnection();

                    return false;
                }

                return true;
            }
        }

        public static void CheckHaveSaveFileToJoin(SaveSlotSync Data)
        {
            bool HaveSaveFile = false;
            Episode Ep = (Episode)Data.m_Episode;
            SaveSlotType SST = (SaveSlotType)Data.m_SaveSlotType;
            int Seed = Data.m_Seed;
            string SaveToLoad = "";

            Il2CppSystem.Collections.Generic.List<SaveSlotInfo> Slots = SaveGameSystem.GetSortedSaveSlots(Ep, SST);

            for (int i = 0; i < Slots.Count; i++)
            {
                SaveSlotInfo Slot = Slots[i];

                if (Slot.m_UserDefinedName == "MULTIPLAYER_" + Seed)
                {
                    MelonLogger.Log("Found slot to load");
                    HaveSaveFile = true;
                    SaveToLoad = Slot.m_SaveSlotName;
                    break;
                }
            }

            if(HaveSaveFile == true)
            {
                MelonLogger.Log("Trying loading save slot...");
                GameManager.LoadSaveGameSlot(SaveToLoad, 0);
            }else{
                OverrideMenusForConnection = true;
                SelectGenderForConnection();
            }
        }


        public static void ForcedCreateSave(SaveSlotSync Data)
        {
            Episode Ep = (Episode)Data.m_Episode;
            SaveSlotType SST = (SaveSlotType)Data.m_SaveSlotType;
            int Seed = Data.m_Seed;
            ExperienceModeType ExpType = (ExperienceModeType)Data.m_ExperienceMode;
            GameRegion Region = (GameRegion)Data.m_Location;

            MelonLogger.Log("Creating save slot " + "MULTIPLAYER_" + Seed);
            SaveGameSystem.SetCurrentSaveInfo(Episode.One, SaveSlotType.SANDBOX, SaveGameSlots.GetUnusedGameId(), (string)null);

            SaveGameSlots.SetSlotDisplayName(SaveGameSystem.GetCurrentSaveName(), "MULTIPLAYER_" + Seed);

            InterfaceManager.m_Panel_OptionsMenu.m_State.m_StartRegion = Region;

            GameManager.GetExperienceModeManagerComponent().SetExperienceModeType(ExpType);

            InterfaceManager.m_Panel_MainMenu.OnSandboxFinal();
            GameManager.m_SceneTransitionData.m_GameRandomSeed = Seed;
        }

        public static void PlayMultiplayer3dAduio(string sound)
        {
            if(anotherbutt && levelid == anotherplayer_levelid)
            {
                GameAudioManager.Play3DSound(sound, anotherbutt);
            }
        }
        public static void SendMultiplayerAudio(string sound)
        {
            if (levelid == anotherplayer_levelid)
            {
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.MULTISOUND))
                    {
                        _packet.Write(sound);
                        SendTCPData(_packet);
                    }
                }
                if (iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.MULTISOUND))
                    {
                        ServerSend.MULTISOUND(1, sound);
                    }
                }
            }
        }

        public static bool NotNeedToPauseUntilLoaded = false;


        public override void OnUpdate()
        {
            UpdateMain();
            GameLogic.Update();

            if(ALWAYS_FUCKING_CURSOR_ON == true)
            {
                InputManager.m_CursorState = InputManager.CursorState.Show;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            DoColisionForArrows();

            if(InOnline() == true)
            {
                if (GameManager.m_InterfaceManager != null && InterfaceManager.m_Panel_PauseMenu != null && InterfaceManager.m_Panel_PauseMenu != null && InterfaceManager.m_Panel_PauseMenu.isActiveAndEnabled == true && SceneManager.IsLoading() == false && NotNeedToPauseUntilLoaded == false)
                {
                    GameManager.m_IsPaused = false;
                }

                if (GameManager.m_Condition != null && GameManager.m_PlayerManager != null && GameManager.GetPlayerManagerComponent().PlayerIsDead())
                {
                    GameManager.m_Condition.DisableLowHealthEffects();
                }
                if(DoFakeGetup == true)
                {
                    BashGetupDelayCamera();
                }
            }

            if (Time.time > nextActionTimeSecond)
            {
                nextActionTimeSecond += periodSecond;
                MyMod.EverySecond();
            }

            if (InterfaceManager.m_Panel_BodyHarvest != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject != null && InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>() != null)
            {
                HarvestingAnimal = InterfaceManager.m_Panel_BodyHarvest.m_BodyHarvest.gameObject.GetComponent<ObjectGuid>().Get();
            }else{
                HarvestingAnimal = "";
            }

            if (GameManager.m_BodyCarry != null && GameManager.GetBodyCarryComponent().m_Body != null)
            {
                GameObject HeldBody = GameManager.GetBodyCarryComponent().m_Body.gameObject;
                if (HeldBody.GetComponent<ObjectGuid>() != null && HeldBody.GetComponent<ObjectGuid>().Get() == PlayerBodyGUI)
                //if (HeldBody.GetComponent<ObjectGuid>() != null)
                {
                    CarryingPlayer = true;
                }else{
                    CarryingPlayer = false;
                }
            }else{
                CarryingPlayer = false;
            }

            if(CarryingPlayer == true && AnimState == "Knock")
            {
                if(playerbody != null && anotherbutt != null)
                {
                    anotherplayer_levelid = levelid;
                    LastRecivedOtherPlayerVector = playerbody.transform.position;
                    anotherbutt.transform.position = playerbody.transform.position;
                }
            }

            if (PreviousHarvestingAnimal != HarvestingAnimal)
            {
                PreviousHarvestingAnimal = HarvestingAnimal;
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.HARVESTINGANIMAL))
                    {
                        _packet.Write(HarvestingAnimal);
                        SendTCPData(_packet);
                    }
                }
                if (iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.HARVESTINGANIMAL))
                    {
                        ServerSend.HARVESTINGANIMAL(1, HarvestingAnimal);
                    }
                }
            }

            //if (Time.time > nextActionTimeAniamls)
            //{
            //    nextActionTimeAniamls += periodAniamls;
            //    if (levelid > 3 && AnimalsController == true)
            //    {
            //        for (int i = 0; i < AllAnimalsNew.Count; i++)
            //        {
            //            if (AllAnimalsNew.ElementAt<GameObject>(i) != null)
            //            {
            //                GameObject animal = AllAnimalsNew.ElementAt<GameObject>(i);
            //                if (animal.GetComponent<AnimalUpdates>() != null && IsShatalkerMode() == false)
            //                {
            //                    animal.GetComponent<AnimalUpdates>().CallSync(false);
            //                }
            //            }
            //        }
            //    }
            //}

            if (IamShatalker == true && IsShatalkerMode() == true)
            {
                GameManager.m_IceCrackingManager.enabled = false;
                if (ShatalkerObject != null)
                {
                    ShatalkerObject.SetStartMovementDelayTime(OverridenStartCountDown);
                }

                if (anotherbutt != null)
                {
                    if (DarkWalkerIsReady == false)
                    {
                        if (InWaitingRoom == false)
                        {
                            InWaitingRoom = true;
                            SendToWaitngRoom();
                        }
                    } else {
                        if (InWaitingRoom == true)
                        {
                            InWaitingRoom = false;
                            ReturnFromWaitingRoom();
                        }

                        float dist = Vector3.Distance(GameManager.GetPlayerTransform().position, anotherbutt.transform.position);
                        if (dist < 200 && WardIsActive == true)
                        {
                            if (GameManager.GetPlayerManagerComponent().m_ControlMode != PlayerControlMode.Locked)
                            {
                                HUDMessage.AddMessage("You got too close to the survivor!");
                                GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Locked);
                            }
                        } else {
                            if (GameManager.GetPlayerManagerComponent().m_ControlMode == PlayerControlMode.Locked)
                            {
                                GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Normal);
                            }
                        }
                    }
                }
            }

            if (iAmHost)
            {
                bool need_update = false;

                if (levelid != previous_levelid)
                {
                    previous_levelid = levelid;
                    MyTicksOnScene = 0;
                    need_update = true;
                }
                else {
                    MyTicksOnScene = MyTicksOnScene + 1;
                }

                if (anotherplayer_levelid != previous_anotherplayer_levelid)
                {
                    previous_anotherplayer_levelid = anotherplayer_levelid;
                    TicksOnScene = 0;
                    need_update = true;
                }
                else {
                    TicksOnScene = TicksOnScene + 1;
                }

                if (need_update == true)
                {
                    SetAnimalControllerRole();
                }
            }

            if (level_name == "MainMenu")
            {
                Vector3 v3 = GameManager.GetMainCamera().transform.position;
                GameManager.GetMainCamera().transform.position = new Vector3(80.27f, 2.9f, 47.47f);

                ///MelonLogger.Log("Camera X "+ v3.x  + " Y "+ v3.y + " Z "+ v3.z);

                if (MenuStuffSpawned == null)
                {
                    GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>("multi_test");
                    GameObject pl1 = GameObject.Instantiate(LoadedAssets);
                    GameObject pl2 = GameObject.Instantiate(LoadedAssets);
                    Animator pl1_anim = pl1.GetComponentInChildren<Animator>();
                    Animator pl2_anim = pl2.GetComponentInChildren<Animator>();
                    pl1_anim.Play("Menu1", 0);
                    pl2_anim.Play("Menu2", 0);
                    pl1.transform.position = new Vector3(77, 2.3f, 47);
                    pl2.transform.position = new Vector3(75, 2.3f, 49);
                    pl2.transform.rotation = new Quaternion(0, 0.773023f, 0, 0.634378f);

                    GameObject campfire = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("INTERACTIVE_CampFire").Cast<GameObject>());
                    campfire.transform.position = new Vector3(77, 2.3f, 48);
                    Campfire cf = campfire.GetComponent<Campfire>();
                    FlexFire = cf;
                    MenuErjan1 = pl1_anim;
                    MenuErjan2 = pl2_anim;
                    if (cf != null)
                    {
                        cf.m_Fire.FireStateSet(FireState.FullBurn);
                        cf.m_Fire.ForceBurnTimeInMinutes(10);
                    }

                    MenuStuffSpawned = pl1;
                }
            }

            if (levelid > 3)
            {

                if (IamShatalker == true)
                {
                    GameManager.GetVpFPSPlayer().Controller.MotorVelocityMax = DarkWalkerSpeed;
                    GameManager.GetVpFPSPlayer().Controller.MotorVelocityMin = DarkWalkerSpeed;
                }

                if (GameManager.GetPlayerManagerComponent().m_InteractiveObjectUnderCrosshair != null)
                {
                    LastObjectUnderCrosshair = GameManager.GetPlayerManagerComponent().m_InteractiveObjectUnderCrosshair;
                }

                int layerMask = 1 << 16;

                RaycastHit hit;

                Transform transform = GameManager.GetMainCamera().transform;


                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 50, layerMask))
                {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

                    if (hit.collider.gameObject.GetComponent<ObjectGuid>())
                    {
                        DebugAnimalGUID = hit.collider.gameObject.GetComponent<ObjectGuid>().Get();
                        DebugAnimalGUIDLast = DebugAnimalGUID;
                        DebugLastAnimal = hit.collider.gameObject;
                    }
                    else {
                        DebugAnimalGUID = "Have not objectGuid";
                        DebugAnimalGUIDLast = DebugAnimalGUID;
                        DebugLastAnimal = hit.collider.gameObject;
                    }
                }
                else {
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                    DebugAnimalGUID = "";
                }

                for (int i = 0; i < AllAnimalsNew.Count; i++)
                {
                    if (AllAnimalsNew.ElementAt<GameObject>(i) == null)
                    {
                        AllAnimalsNew.RemoveAt(i);
                        MelonLogger.Log("[LOCAL] Wiping out missing animals.");
                    }
                }
                for (int i = 0; i < AllAnimals.Count; i++)
                {
                    if (AllAnimals.ElementAt<GameObject>(i) == null)
                    {
                        AllAnimals.RemoveAt(i);
                        MelonLogger.Log("[GLOBAL] Wiping out missing animals.");
                    }
                }

                if (InOnline() == true && IsShatalkerMode() == false)
                {
                    if (Utils.IsZero(GameManager.GetConditionComponent().m_CurrentHP)) // Make fake death.
                    {
                        IsDead = true;

                        if (IsCarringMe == true && levelid == anotherplayer_levelid && anotherbutt != null && GameManager.m_PlayerManager != null)
                        {
                            GameManager.GetPlayerManagerComponent().TeleportPlayer(anotherbutt.transform.position, anotherbutt.transform.rotation);
                        }
                    }else{
                        IsDead = false;
                    }
                }

                MyHasRifle = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_Rifle");

                if (PrviousHasRifle != MyHasRifle)
                {
                    PrviousHasRifle = MyHasRifle;
                    if (sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.HASRIFLE))
                        {
                            _packet.Write(MyHasRifle);
                            SendTCPData(_packet);
                        }
                    }
                    if (iAmHost == true)
                    {
                        ServerHandle.MyHasRifle = MyHasRifle;

                        using (Packet _packet = new Packet((int)ServerPackets.HASRIFLE))
                        {
                            ServerSend.HASRIFLE(1, MyHasRifle);
                        }
                    }
                }

                MyHasRevolver = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_Revolver");

                bool axe = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_Hatchet"); // Axe
                bool f_axe = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_FireAxe"); // Fire axe
                bool h_axe = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_HatchetImprovised"); // Handmade axe

                if (axe == true || f_axe == true || h_axe) // If have any axe
                {
                    MyHasAxe = true;
                } else
                {
                    MyHasAxe = false;
                }

                MyHasMedkit = GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_MedicalSupplies_hangar");

                if (PreviousHasMedkit != MyHasMedkit)
                {
                    PreviousHasMedkit = MyHasMedkit;
                    if (sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.HASMEDKIT))
                        {
                            _packet.Write(MyHasMedkit);
                            SendTCPData(_packet);
                        }
                    }
                    if (iAmHost == true)
                    {
                        ServerHandle.MyHasAxe = MyHasMedkit;

                        using (Packet _packet = new Packet((int)ServerPackets.HASMEDKIT))
                        {
                            ServerSend.HASMEDKIT(1, MyHasMedkit);
                        }
                    }
                }


                MyArrows = GameManager.GetInventoryComponent().NumGearInInventory("GEAR_Arrow");

                if (PrviousHasRevolver != MyHasRevolver)
                {
                    PrviousHasRevolver = MyHasRevolver;
                    if (sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.HASREVOLVER))
                        {
                            _packet.Write(MyHasRevolver);
                            SendTCPData(_packet);
                        }
                    }
                    if (iAmHost == true)
                    {
                        ServerHandle.MyHasRifle = MyHasRifle;

                        using (Packet _packet = new Packet((int)ServerPackets.HASREVOLVER))
                        {
                            ServerSend.HASREVOLVER(1, MyHasRevolver);
                        }
                    }
                }
                if (PreviousHasAxe != MyHasAxe)
                {
                    PreviousHasAxe = MyHasAxe;
                    if (sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.HASAXE))
                        {
                            _packet.Write(MyHasAxe);
                            SendTCPData(_packet);
                        }
                    }
                    if (iAmHost == true)
                    {
                        ServerHandle.MyHasAxe = MyHasAxe;

                        using (Packet _packet = new Packet((int)ServerPackets.HASAXE))
                        {
                            ServerSend.HASAXE(1, MyHasAxe);
                        }
                    }
                }
                if (PreviousArrows != MyArrows)
                {
                    PreviousArrows = MyArrows;
                    if (sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.HISARROWS))
                        {
                            _packet.Write(MyArrows);
                            SendTCPData(_packet);
                        }
                    }
                    if (iAmHost == true)
                    {
                        ServerHandle.MyArrows = MyArrows;

                        using (Packet _packet = new Packet((int)ServerPackets.HISARROWS))
                        {
                            ServerSend.HISARROWS(1, MyArrows);
                        }
                    }
                }

                if (InterfaceManager.m_Panel_Map.IsEnabled() == false)
                {
                    if (GameManager.GetPlayerManagerComponent().m_ItemInHands != null)
                    {
                        MyLightSourceName = GameManager.GetPlayerManagerComponent().m_ItemInHands.name;
                    } else
                    {
                        MyLightSourceName = "";
                    }
                } else
                {
                    MyLightSourceName = "Map";
                }

                if (PreviousSleeping != GameManager.GetPlayerManagerComponent().PlayerIsSleeping())
                {
                    PreviousSleeping = GameManager.GetPlayerManagerComponent().PlayerIsSleeping();
                    if (GameManager.GetPlayerManagerComponent().PlayerIsSleeping() == true)
                    {
                        MelonLogger.Log("Going sleep");
                        MelonLogger.Log("Skiping Cycle time " + MyCycleSkip);
                        GameObject bed = LastObjectUnderCrosshair;
                        if (bed != null && bed.GetComponent<Bed>() != null)
                        {
                            //V3BeforeSleep = GameManager.GetPlayerTransform().position;
                            //NeedV3BeforeSleep = true;
                            //GameManager.GetPlayerTransform().position = bed.gameObject.transform.position;
                            //GameManager.GetPlayerTransform().rotation = bed.gameObject.transform.rotation;
                        } else {
                            NeedV3BeforeSleep = false;
                        }
                    } else {
                        MyCycleSkip = 0;
                        MelonLogger.Log("Has wakeup");
                        if (NeedV3BeforeSleep == true)
                        {
                            GameManager.GetPlayerTransform().position = V3BeforeSleep;
                        }
                    }

                    if (sendMyPosition == true)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.SLEEPHOURS))
                        {
                            _packet.Write(MyCycleSkip);
                            SendTCPData(_packet);
                        }
                    }
                    if (iAmHost == true)
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.SLEEPHOURS))
                        {
                            ServerSend.SLEEPHOURS(1, MyCycleSkip);
                        }
                    }
                }

                if (iAmHost == true && GameManager.GetPlayerManagerComponent().PlayerIsSleeping() == true && MyCycleSkip > 0 && (CycleSkip > 0 || AnimState == "Knock") && IsCycleSkiping == false)
                {
                    IsCycleSkiping = true;
                    int MySkip = MyCycleSkip;
                    int OtherSkip = CycleSkip;
                    int Skip = 0;
                    MelonLogger.Log("I am selected to sleep " + MySkip);
                    MelonLogger.Log("Other player selected to sleep " + OtherSkip);

                    if (MySkip != 0 && (OtherSkip != 0 || AnimState == "Knock"))
                    {
                        if (MySkip > OtherSkip)
                        {
                            MelonLogger.Log(MySkip + "(My) is higher than " + OtherSkip + " so skiping " + MySkip + "(My) hours.");
                            Skip = MySkip;
                        }
                        else if (MySkip < OtherSkip)
                        {
                            MelonLogger.Log(OtherSkip + " is higher than " + MySkip + "(My) so skiping " + OtherSkip + " hours.");
                            Skip = OtherSkip;
                        }
                        else if (MySkip == OtherSkip)
                        {
                            MelonLogger.Log(OtherSkip + " and " + MySkip + "(My) is equal, skiping " + MySkip + " hours.");
                            Skip = MySkip;
                        }
                        int totaltime = OverridedHourse + Skip;
                        int leftovers = 0;
                        if (totaltime > 24)
                        {
                            leftovers = totaltime - 24;
                            OverridedHourse = 0 + leftovers;
                        } else
                        {
                            OverridedHourse = OverridedHourse + Skip;
                        }
                        MyMod.EveryInGameMinute();
                        CycleSkip = 0;
                        MyCycleSkip = 0;
                        IsCycleSkiping = false;
                    }
                    else
                    {
                        MelonLogger.Log("One of value is zero!!! What the duck? How can this be?");
                        MelonLogger.Log(MySkip + "(My) " + OtherSkip);
                    }
                }
                else if (IsCycleSkiping == true)
                {
                    IsCycleSkiping = false;
                }
            }

            if (iAmHost == true && NeedSyncTime == true)
            {
                if (Time.time > nextActionTime)
                {
                    nextActionTime += period;
                    MyMod.EveryInGameMinute();
                }
            }

            if (MyPreviousAnimState != MyAnimState)
            {
                MyPreviousAnimState = MyAnimState;
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.ANIMSTATE))
                    {
                        _packet.Write(MyAnimState);
                        SendTCPData(_packet);
                    }
                }
                if (iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.ANIMSTATE))
                    {
                        ServerSend.ANIMSTATE(1, MyAnimState);
                    }
                }
            }

            if (MyLightSourceName != MyLastLightSourceName)
            {
                MyLastLightSourceName = MyLightSourceName;
                MelonLogger.Log("Holding item: " + MyLastLightSourceName);
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.LIGHTSOURCENAME))
                    {
                        _packet.Write(MyLightSourceName);
                        SendTCPData(_packet);
                    }
                }
                if (iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.LIGHTSOURCENAME))
                    {
                        ServerSend.LIGHTSOURCENAME(1, MyLightSourceName);
                    }
                }
            }
            if (MyLightSource != MyLastLightSource)
            {
                MelonLogger.Log("Lightchanged " + MyLightSource);
                MyLastLightSource = MyLightSource;
                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.LIGHTSOURCE))
                    {
                        _packet.Write(MyLightSource);
                        SendTCPData(_packet);
                    }
                }
                if (iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.LIGHTSOURCE))
                    {
                        ServerSend.LIGHTSOURCE(1, MyLightSource);
                    }
                }
            }

            if (RealTimeCycleSpeed == true && InOnline() == true)
            {
                uConsole.RunCommandSilent("set_time " + OveridedTime);
            }

            if (IamShatalker == true || ((ShatalkerModeClient == true || ServerHandle.ReturnShatalkerMode() == true) && levelid != anotherplayer_levelid))
            {
                Vector3 shatalkerV3 = new Vector3(-1000, -1000, -1000);
                if (ShatalkerObject != null)
                {
                    ShatalkerObject.m_WorldPosition = shatalkerV3;
                }
            }
            if (sendMyPosition == true && ShatalkerModeClient == true && levelid == anotherplayer_levelid)
            {
                if (ShatalkerObject != null)
                {
                    ShatalkerObject.m_WorldPosition = LastRecivedShatalkerVector;
                }
            }

            if(DebugGUI == true)
            {
                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.B))
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(GameManager.GetMainCamera().transform.position, GameManager.GetMainCamera().transform.forward, out hitInfo, 50))
                    {
                        Vector3 instantiatePosition = SnappedPosition(hitInfo.point, hitInfo.collider.transform.position);

                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = instantiatePosition;

                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.BLOCK))
                            {
                                _packet.Write(cube.transform.position);
                                SendTCPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.BLOCK))
                            {
                                ServerSend.BLOCK(1, cube.transform.position);
                            }
                        }
                    }
                }
                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.K))
                {
                    SimRevive();
                }

                if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.L))
                {
                    if (ALWAYS_FUCKING_CURSOR_ON == false)
                    {
                        ALWAYS_FUCKING_CURSOR_ON = true;
                    }else{
                        ALWAYS_FUCKING_CURSOR_ON = false;
                    }
                }
            }

            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.G))
            {
                float dist = 0;
                if (anotherbutt == null)
                {
                    MelonLogger.Log("Other player not exist.");
                }
                else
                {
                    dist = Vector3.Distance(GameManager.GetPlayerTransform().position, anotherbutt.transform.position);
                }

                //if (LastSelectedGearName != "" && LastSelectedGearGUID != "" && anotherbutt != null)
                if (LastSelectedGearName != "" && LastSelectedGear != null && InterfaceManager.m_Panel_Inventory.IsEnabled() == true)
                {
                    if (levelid == anotherplayer_levelid && dist < 100)
                    {
                        MelonLogger.Log("You gave " + LastSelectedGearName + " to other player");
                        Il2CppSystem.Collections.Generic.List<GearItemObject> items = GameManager.GetInventoryComponent().m_Items;
                        GearItem _gear = null;

                        for (int i = 0; i < items.Count; i++)
                        {
                            GearItem gear_ = items[i].m_GearItem;

                            if (gear_ == LastSelectedGear)
                            {
                                _gear = items[i].m_GearItem;
                                break;
                            }
                        }

                        if (_gear != null)
                        {

                            if (iAmHost == true)
                            {
                                using (Packet _packet = new Packet((int)ServerPackets.GOTITEM))
                                {
                                    ServerSend.GOTITEM(1, _gear);
                                }
                            }
                            if (sendMyPosition == true)
                            {
                                using (Packet _packet = new Packet((int)ClientPackets.GOTITEM))
                                {
                                    _packet.Write(_gear);
                                    SendTCPData(_packet);
                                }
                            }

                            MelonLogger.Log("You gave " + LastSelectedGearName + " to other player");

                            if (_gear.GetComponent<GunItem>() != null)
                            {
                                InterfaceManager.m_Panel_Inventory.OnRefuel();
                                HUDMessage.AddMessage("You gave " + _gear.m_LocalizedDisplayName.Text() + " to other player, bullets back to inventory.");
                                GameManager.GetInventoryComponent().RemoveUnits(_gear, 1);
                            }
                            else
                            {
                                if (LastSelectedGearName == "GEAR_WaterSupplyPotable" || LastSelectedGearName == "GEAR_WaterSupplyNotPotable")
                                {
                                    WaterSupply bottle = null;
                                    string say = "half liter of " + _gear.m_LocalizedDisplayName.Text();

                                    if (LastSelectedGearName == "GEAR_WaterSupplyPotable")
                                    {
                                        bottle = GameManager.GetInventoryComponent().m_WaterSupplyPotable.m_WaterSupply;
                                    }
                                    if (LastSelectedGearName == "GEAR_WaterSupplyNotPotable")
                                    {
                                        bottle = GameManager.GetInventoryComponent().m_WaterSupplyNotPotable.m_WaterSupply;
                                    }

                                    float Liters = bottle.m_VolumeInLiters;

                                    if (Liters >= 0.5f)
                                    {
                                        bottle.m_VolumeInLiters = Liters - 0.5f;
                                    } else
                                    {
                                        bottle.m_VolumeInLiters = 0f;
                                        say = Liters + " of " + _gear.m_LocalizedDisplayName.Text();
                                    }
                                    HUDMessage.AddMessage("You gave " + say + " to other player.");
                                }
                                else
                                {
                                    HUDMessage.AddMessage("You gave " + _gear.m_LocalizedDisplayName.Text() + " to other player");
                                    GameManager.GetInventoryComponent().RemoveUnits(_gear, 1);
                                }
                            }
                            NeedRefreshInv = true; // Reloading items list.
                        }
                        else
                        {
                            HUDMessage.AddMessage("You try give item that not exist anymore.");
                        }
                    }
                    else if (levelid != anotherplayer_levelid)
                    {
                        HUDMessage.AddMessage("Your friend aren't in same house/location.");
                    }
                    else if (levelid == anotherplayer_levelid && dist > 100)
                    {
                        HUDMessage.AddMessage("You too far to give an item.");
                    }
                }
            }
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, KeyCode.N))
            {
                float dist = 3;
                if (anotherbutt != null)
                {
                    dist = Vector3.Distance(GameManager.GetPlayerTransform().position, anotherbutt.transform.position);
                }

                if (dist < 1 && AnimState == "Knock" && GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_MedicalSupplies_hangar") == true)
                {
                    if (sendMyPosition == true) // CLIENT
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.REVIVE))
                        {
                            _packet.Write(true);
                            SendTCPData(_packet);
                        }
                    }
                    if (iAmHost == true) // HOST
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.REVIVE))
                        {
                            ServerSend.REVIVE(1, true);
                        }
                    }
                }
            }

            ShiftPressed = InputManager.GetSprintDown(InputManager.m_CurrentContext);

            Transform target = null;

            if (levelid > 3) // Trying get player's position only if not at menu.
            {
                target = GameManager.GetPlayerTransform();
                //SYNC MOVEMENT
                bool InFight = GameManager.GetPlayerStruggleComponent().InStruggle();
                if (target != null && previoustickpos != GameManager.GetPlayerTransform().position) // If position changed.
                {

                    previoustickpos = GameManager.GetPlayerTransform().position;

                    if(IsDead == false && KillAfterLoad == false && InFight == false)
                    {
                        if (GameManager.GetPlayerManagerComponent().PlayerCanSprint() == true && GameManager.GetPlayerManagerComponent().PlayerIsSprinting())
                        {
                            MyAnimState = "Run";
                        }
                        else
                        {
                            if (GameManager.GetPlayerManagerComponent().PlayerIsCrouched() == true)
                            {
                                MyAnimState = "Ctrl";
                            }
                            else
                            {
                                MyAnimState = "Walk";
                            }
                        }
                    }

                    if (sendMyPosition == true) // CLIENT
                    {
                        if (IamShatalker == false)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.XYZ))
                            {
                                _packet.Write(target.position);
                                SendTCPData(_packet);
                            }
                        }
                        if (IamShatalker == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.XYZDW))
                            {
                                _packet.Write(target.position);
                                SendTCPData(_packet);
                            }
                        }
                    }
                    if (iAmHost == true) // HOST
                    {
                        if (IamShatalker == false)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.XYZ))
                            {
                                ServerSend.XYZ(1, target.position);
                            }
                        }
                        if (IamShatalker == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.XYZDW))
                            {
                                ServerSend.XYZDW(1, target.position);
                            }
                        }
                    }
                }
                else
                {
                    if (IsDead == true || KillAfterLoad == true || InFight == true)
                    {
                        if(IsDead == true || KillAfterLoad == true)
                        {
                            MyAnimState = "Knock";
                        }else{
                            MyAnimState = "Fight";
                        }
                    }else{
                        if (GameManager.GetPlayerManagerComponent().PlayerIsSleeping() == true)
                        {
                            MyAnimState = "Sleep";
                        }
                        else
                        {
                            if (GameManager.GetPlayerInVehicle().m_InVehicle == true)
                            {
                                MyAnimState = "Sit";
                            }
                            else
                            {
                                if (InterfaceManager.m_Panel_Map.IsEnabled() == false)
                                {
                                    if (GameManager.GetPlayerManagerComponent().PlayerIsCrouched() == true)
                                    {
                                        MyAnimState = "Ctrl";
                                    }
                                    else
                                    {
                                        MyAnimState = "Idle";
                                    }
                                } else
                                {
                                    MyAnimState = "Map";
                                }
                            }
                        }
                    }
                }
            }
            /*
            //Get movement speed.
            if(MyAnimState == "Run" || MyAnimState == "Walk")
            {
                Vector3 pastV3 = GameManager.GetPlayerTransform().position;

                float distance = Vector3.Distance(pastV3, GameManager.GetPlayerTransform().position); // distance player has run.
                float dif = 1 / 30;
                float speed = distance/ dif;
                MelonLogger.Log("Speed: "+ speed);
            }
            */
            //SYNC ROTATION
            if (target != null && previoustickrot != GameManager.GetPlayerTransform().rotation) // If rotation changed.
            {
                previoustickrot = GameManager.GetPlayerTransform().rotation;
                if (sendMyPosition == true) // CLIENT
                {
                    using (Packet _packet = new Packet((int)ClientPackets.XYZW))
                    {
                        _packet.Write(target.rotation);
                        SendTCPData(_packet);
                    }
                }
                if (iAmHost == true) // HOST
                {
                    using (Packet _packet = new Packet((int)ServerPackets.XYZW))
                    {
                        ServerSend.XYZW(1, target.rotation);
                    }
                }
            }

            if (iAmHost == true) // SYNC OTHER PLAYER
            {
                if (ServerHandle.ReturnShatalkerMode() == false && levelid == anotherplayer_levelid)
                {
                    if (anotherbutt != null)
                    {
                        Vector3 maboi = ServerHandle.ReturnLastBoi();

                        if (LastRecivedOtherPlayerVector != maboi)
                        {
                            LastRecivedOtherPlayerVector = maboi;
                        }
                        LastRecivedOtherPlayerQuatration = ServerHandle.ReturnLastBoiQuat();
                        //anotherbutt.transform.position = new Vector3(maboi.x, maboi.y + 0.03f, maboi.z);
                    }
                }
                if (ServerHandle.ReturnShatalkerMode() == true && levelid == anotherplayer_levelid)
                {
                    Vector3 maboi = ServerHandle.ReturnLastBoi();

                    if (ShatalkerObject != null)
                    {
                        ShatalkerObject.m_WorldPosition = maboi;
                    }
                }
                if (PreviousBlock != ServerHandle.ReutrnLastBlock())
                {
                    Vector3 maboi = ServerHandle.ReutrnLastBlock();
                    PreviousBlock = maboi;
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = maboi;
                }

                LightSourceName = ServerHandle.ReturnLastLight();

                if (PreviousLightSourceName != LightSourceName)
                {
                    PreviousLightSourceName = LightSourceName;
                    UpdateHeldItem(LightSourceName);
                }

                LightSource = ServerHandle.ReturnLastLightState();
                //FireToIgnite = ServerHandle.ReturnLastFire();
                AnimState = ServerHandle.ReturnLastAnimState();
                HasRifle = ServerHandle.ReturnLastHasRifle();
                HasRevolver = ServerHandle.ReturnLastHasRevolver();
                HasAxe = ServerHandle.ReturnLastHasAxe();
                HisArrows = ServerHandle.ReturnLastArrow();
                HasMedkit = ServerHandle.ReturnLastHasMedkit();
                CycleSkip = ServerHandle.ReturnLastSleeping();
                WardIsActive = ServerHandle.ReturnLastWardIsActive();
                OverridenStartCountDown = ServerHandle.ReturnLastCountDown();
                if (OverridenStartCountDown < 2)
                {
                    OverridenStartCountDown = 0;
                    ShatalkerObject.SetStartMovementDelayTime(OverridenStartCountDown);
                }

                if (ServerHandle.NeedReloadDWReadtState == true)
                {
                    ServerHandle.NeedReloadDWReadtState = false;
                    if (iAmHost == true) // HOST
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.DARKWALKERREADY))
                        {
                            ServerSend.DARKWALKERREADY(1, DarkWalkerIsReady);
                        }
                    }
                }

                if (LastRecivedHasRifle != HasRifle)
                {
                    LastRecivedHasRifle = HasRifle;
                    UpdateHeldItem("nodis");
                }
                if (LastRecivedHasRevolver != HasRevolver)
                {
                    LastRecivedHasRevolver = HasRevolver;
                    UpdateHeldItem("nodis");
                }
                if (LastRecivedHasAxe != HasAxe)
                {
                    LastRecivedHasAxe = HasAxe;
                    UpdateHeldItem("nodis");
                }
                if (LastRecivedArrows != HisArrows)
                {
                    LastRecivedArrows = HisArrows;
                    UpdateHeldItem("nodis");
                }
                if (LastRecivedHasMedkit != HasMedkit)
                {
                    LastRecivedHasMedkit = HasMedkit;
                    UpdateHeldItem("nodis");
                }
                if (PreviousLightSource != LightSource)
                {
                    PreviousLightSource = LightSource;
                    UpdateHeldItem("nodis");
                }

                if (anotherplayer_levelid != ServerHandle.ReturnLastLevelID())
                {
                    anotherplayer_levelid = ServerHandle.ReturnLastLevelID();
                    MelonLogger.Log("Other player transition to level " + anotherplayer_levelid);
                }

                if (IamShatalker == true)
                {
                    DarkWalkerIsReady = ServerHandle.ReturnLastReadyState();
                    if (DarkWalkerIsReady == true)
                    {
                        ServerHandle.LastCountDown = 0;
                        OverridenStartCountDown = 0;
                    }
                }
            }
            if (anotherbutt == null && ShatalkerModeClient == false && ServerHandle.ReturnShatalkerMode() == false && levelid > 3) // Create multiplayer player object if it not exist.
            {
                //anotherbutt = GameObject.CreatePrimitive(PrimitiveType.Cube);
                GameObject LoadedAssets = LoadedBundle.LoadAsset<GameObject>("multi_test");
                anotherbutt = GameObject.Instantiate(LoadedAssets);
                anotherbutt.AddComponent<MultiplayerPlayer>();
                //anotherbutt.AddComponent<AiStag>();
                //anotherbutt.AddComponent<BaseAi>();
                //anotherbutt.AddComponent<BoxCollider>();
                //ObjectGuid _ObjectGuid = anotherbutt.AddComponent<ObjectGuid>();
                //_ObjectGuid.Set("OTHER_PLAYER");

                //NPC _NPC = anotherbutt.AddComponent<NPC>();
                //NPCCondition _NPCCondition = anotherbutt.AddComponent<NPCCondition>();
                //NPCFreezing _NPCFreezing = anotherbutt.AddComponent<NPCFreezing>();
                //NPCAfflictions _NPCAfflictions = anotherbutt.AddComponent<NPCAfflictions>();
                //AiTarget _AiTarget = anotherbutt.AddComponent<AiTarget>();
                //NPCThirst _NPCThirst = anotherbutt.AddComponent<NPCThirst>();
                //InteractionOverride _InteractionOverride = anotherbutt.AddComponent<InteractionOverride>();
                //Placeable _Placeable = anotherbutt.AddComponent<Placeable>();

                //_InteractionOverride.m_Active = false;
                //_InteractionOverride.m_Guid = _ObjectGuid.m_Guid;
                //_AiTarget.m_Npc = _NPC;
                //_Placeable.m_Guid = _ObjectGuid.m_Guid;
                //_Placeable.m_InteractionOverride = _InteractionOverride;
                //_NPC.m_Guid = _ObjectGuid.m_Guid;
                //_NPC.m_Freezing = _NPCFreezing;
                //_NPC.m_Thirst = _NPCThirst;

                //anotherbutt.AddComponent<CarryableBody>();
                //CarryableBody Body = anotherbutt.GetComponent<CarryableBody>();
                //Body.m_NPC = _NPC;
                //Body.m_FPHGearItemPrefab = GetGearItemObject("Gear_Body_Dummy");
                //Body.m_PickupIntensity = CarryableBody.PickupIntensity.Low;
                //Body.m_Placeable = _Placeable;

                //BaseAi AI = anotherbutt.GetComponent<BaseAi>();

                PlayerColiders = new List<GameObject>();
                ApplyDamageZones(anotherbutt);

                anotherbutt.transform.position = new Vector3(0, 0 + 0.03f, 0);

                animbutt = anotherbutt.GetComponentInChildren<Animator>();
                UpdateHeldItem(LightSourceName);

                //GEAR_MedicalSupplies_hangar
                //GEAR_BottlePainKillers


            }
            if (anotherbutt != null)
            {
                //Renderer rend = anotherbutt.GetComponent<Renderer>();

                //GameManager.GetFootstepTrailManager().m_ActivePlayerTrail.

                //bool walking = GameManager.GetPlayerManagerComponent().PlayerIsWalking();

                if (AnimState == "Knock")
                {
                    if(playerbody == null && levelid == anotherplayer_levelid && CarryingPlayer == false)
                    {
                        if(PlayerBodyGUI == "" || ObjectGuidManager.Lookup(PlayerBodyGUI) == null)
                        {
                            playerbody = UnityEngine.Object.Instantiate<GameObject>(GetGearItemObject("Survivor_NPC_Female_001"));
                            PlayerBodyGUI = playerbody.GetComponent<ObjectGuid>().Get();
                            playerbody.GetComponent<NPC>().m_DisplayName.m_LocalizationID = "Body";
                            playerbody.transform.position = LastRecivedOtherPlayerVector;
                            playerbody.GetComponent<NPCCondition>().m_CurrentHP = 1;
                            playerbody.GetComponent<NPCCondition>().enabled = false;
                            playerbody.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                            MelonLogger.Log("Played knocked, spawning body " + PlayerBodyGUI);
                        }else{
                            MelonLogger.Log("Not need spawn body, cause we have already one " + PlayerBodyGUI);
                        }
                    }
                }
                else{
                    if (playerbody != null)
                    {
                        PlayerBodyGUI = "";
                        MelonLogger.Log("Played not knocked, removing body");
                        UnityEngine.Object.Destroy(playerbody);
                    }
                }

                if (PlayerBodyGUI != "")
                {
                    GameObject findbody = ObjectGuidManager.Lookup(PlayerBodyGUI);
                    if (findbody != null)
                    {
                        findbody.GetComponent<NPC>().m_DisplayName.m_LocalizationID = "Body";
                        findbody.transform.position = LastRecivedOtherPlayerVector;
                        findbody.GetComponent<NPCCondition>().m_CurrentHP = 1;
                        findbody.GetComponent<NPCCondition>().enabled = false;
                        findbody.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                        if(playerbody == null)
                        {
                            playerbody = findbody;
                        }
                    }
                }

                if (animbutt != null)
                {

                    int currentTagHash = animbutt.GetCurrentAnimatorStateInfo(0).tagHash; // This what tag is now
                    int neededTagHash = Animator.StringToHash(AnimState); // This is what tag we need.
                    animbutt.cullingMode = AnimatorCullingMode.AlwaysAnimate;

                    if(AnimState == "Fight")
                    {
                        neededTagHash = Animator.StringToHash("Knock");
                    }


                    // MAIN LAYER
                    if (currentTagHash != neededTagHash)
                    {
                        if (AnimState == "Walk")
                        {
                            animbutt.Play("BasicMotions@Walk01", 0);
                        }
                        if (AnimState == "Idle")
                        {
                            string current_anim = animbutt.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                            //MelonLogger.Log("Ctrl current animation " + current_anim);

                            if (current_anim == "Male Crouch Pose" && current_anim != "Sit_To_Idle")
                            {
                                animbutt.Play("Sit_To_Idle", 0);
                            }
                            else
                            {
                                animbutt.Play("BasicMotions@Idle02", 0);
                            }
                        }
                        if (AnimState == "Run")
                        {
                            animbutt.Play("BasicMotions@Run01", 0);
                        }
                        if (AnimState == "Ctrl")
                        {
                            string current_anim = animbutt.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                            //MelonLogger.Log("Ctrl current animation " + current_anim);

                            if (current_anim != "Male Crouch Pose" && current_anim != "Idle_To_Sit")
                            {
                                animbutt.Play("Idle_To_Sit", 0);
                            }
                            else
                            {
                                animbutt.Play("Male Crouch Pose", 0);
                            }
                        }
                        if (AnimState == "Sit")
                        {
                            animbutt.Play("Sitting Idle", 0);
                        }
                        if (AnimState == "Flex")
                        {
                            animbutt.Play("Samba Dancing", 0);
                        }
                        if (AnimState == "Knock" || AnimState == "Fight")
                        {
                            animbutt.Play("Writhing In Pain", 0);
                        }
                        if (AnimState == "Map")
                        {
                            animbutt.Play("Map", 0);
                        }
                        if (AnimState == "Sleep")
                        {
                            anotherbutt.transform.GetChild(9).gameObject.SetActive(true);
                            animbutt.Play("Sleeping Idle", 0);
                        }
                        if (AnimState != "Sleep")
                        {
                            anotherbutt.transform.GetChild(9).gameObject.SetActive(false);
                        }

                        //MelonLogger.Log("Animation changed now is " + AnimState);
                    }

                    // HANDS LAYER

                    if (LightSourceName == "GEAR_Rifle")
                    {
                        if (AnimState != "Ctrl")
                        {
                            if (AnimStateHands != "Rifle" && AnimStateHands != "Rifle_Sit")
                            {
                                PreAnimStateHands = "Pick";
                            }
                            AnimStateHands = "Rifle";
                        }
                        else
                        {
                            if (AnimStateHands != "Rifle" && AnimStateHands != "Rifle_Sit")
                            {
                                PreAnimStateHands = "Pick_Sit";
                            }
                            AnimStateHands = "Rifle_Sit";
                        }
                    }
                    else if (LightSourceName.StartsWith("GEAR_KeroseneLamp"))
                    {
                        PreAnimStateHands = "";
                        AnimStateHands = "HoldLantern";
                    }
                    else if(LightSourceName == "GEAR_Bow")
                    {
                        AnimStateHands = "Bow";
                    }
                    else
                    {
                        PreAnimStateHands = "";
                        AnimStateHands = "No";
                    }

                    int handsTagHash = animbutt.GetCurrentAnimatorStateInfo(1).tagHash; // This what tag is now
                    int handsNeededTagHash = Animator.StringToHash(AnimStateHands); // This is what tag we need.

                    if (handsTagHash != handsNeededTagHash)
                    {
                        if (AnimStateHands == "Rifle")
                        {
                            if (PreAnimStateHands == "")
                            {
                                animbutt.Play("Rifle", 1);
                            }
                            else
                            {
                                animbutt.Play("Pick", 1);
                                PreAnimStateHands = "";
                            }
                        }
                        if (AnimStateHands == "Rifle_Sit")
                        {
                            if (PreAnimStateHands == "")
                            {
                                animbutt.Play("Rifle_Sit", 1);
                            }
                            else
                            {
                                animbutt.Play("Pick_Sit", 1);
                                PreAnimStateHands = "";
                            }
                        }
                        if (AnimStateHands == "HoldLantern")
                        {
                            animbutt.Play("HoldLantern", 1);
                        }
                        if (AnimStateHands == "Bow")
                        {
                            animbutt.Play("Bow", 1);
                        }
                        if (AnimStateHands == "No")
                        {
                            animbutt.Play("No", 1);
                        }
                    }
                    // FINGERS LAYER
                    if (LightSourceName == "GEAR_Revolver" || LightSourceName == "GEAR_FlareGun")
                    {
                        AnimStateFingers = "HoldRevolver";
                    }
                    else if (LightSourceName == "GEAR_Stone" || LightSourceName == "GEAR_FlareA" || LightSourceName == "GEAR_BlueFlare" || LightSourceName == "GEAR_Torch")
                    {
                        AnimStateFingers = "Hold";
                    }
                    else if (LightSourceName.StartsWith("GEAR_SprayPaint"))
                    {
                        AnimStateFingers = "HoldCan";
                    }
                    else if (LightSourceName == "GEAR_PackMatches" || LightSourceName == "GEAR_WoodMatches")
                    {
                        AnimStateFingers = "HoldMatch";
                    }
                    else
                    {
                        AnimStateFingers = "No";
                    }

                    int fingersTagHash = animbutt.GetCurrentAnimatorStateInfo(1).tagHash; // This what tag is now
                    int fingersNeededTagHash = Animator.StringToHash(AnimStateFingers); // This is what tag we need.

                    if (fingersTagHash != fingersNeededTagHash)
                    {
                        animbutt.Play(AnimStateFingers, 2);
                    }

                    if (AnimState == "Walk" || AnimState == "Run")
                    {
                        GameObject foot_l = anotherbutt.transform.GetChild(6).GetChild(8).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;
                        GameObject foot_r = anotherbutt.transform.GetChild(6).GetChild(8).GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject;

                        float main_y = anotherbutt.transform.position.y; // Y of player object.
                        float leg_y_r = foot_r.transform.position.y;
                        float leg_y_l = foot_l.transform.position.y;
                        float fixed_y_r = main_y - leg_y_r;
                        float fixed_y_l = main_y - leg_y_l;

                        double max = -0.05;
                        double min = -0.057;

                        if (fixed_y_r >= min && fixed_y_r <= max && StepState != 1)
                        {
                            StepState = 1;
                            MaybeLeaveFootPrint(foot_r.transform.position, false, 0.0f, false);
                            string ground_Tag = Utils.GetMaterialTagForObjectAtPosition(anotherbutt, foot_r.transform.position);
                            GameAudioManager.Play3DSound(AK.EVENTS.PLAY_FOOTSTEPSWOLFWALK, anotherbutt);
                        }
                        if (fixed_y_l >= min && fixed_y_l <= max && StepState != 2)
                        {
                            StepState = 2;
                            MaybeLeaveFootPrint(foot_l.transform.position, false, 0.0f, true);
                            string ground_Tag = Utils.GetMaterialTagForObjectAtPosition(anotherbutt, foot_l.transform.position);
                            GameAudioManager.Play3DSound(AK.EVENTS.PLAY_FOOTSTEPSWOLFWALK, anotherbutt);
                        }
                    }
                    else
                    {
                        StepState = 0;
                    }

                    if (levelid != anotherplayer_levelid || CarryingPlayer == true || IsCarringMe == true)
                    {

                        if (anotherbutt.transform.GetChild(0).gameObject.activeSelf == true)
                        {
                            int bodyparts = 9;

                            for (int i = 0; i < bodyparts; i++)
                            {
                                anotherbutt.transform.GetChild(i).gameObject.SetActive(false);
                            }
                        }
                    }
                    if (levelid == anotherplayer_levelid && CarryingPlayer == false)
                    {
                        if(IsCarringMe == false)
                        {
                            if (anotherbutt.transform.GetChild(0).gameObject.activeSelf == false)
                            {
                                int bodyparts = 9;

                                for (int i = 0; i < bodyparts; i++)
                                {
                                    anotherbutt.transform.GetChild(i).gameObject.SetActive(true);
                                }
                            }
                        }

                        Vector3 togo = Vector3.Lerp(anotherbutt.transform.position, LastRecivedOtherPlayerVector, Time.deltaTime * 20);
                        Quaternion toRot = Quaternion.Lerp(anotherbutt.transform.rotation, LastRecivedOtherPlayerQuatration, Time.deltaTime * 20);
                        anotherbutt.transform.position = togo;
                        anotherbutt.transform.rotation = toRot;
                    }
                }
                if (IsShatalkerMode() == true)
                {
                    if (ShatalkerObject == null)
                    {
                        ShatalkerObject = Resources.FindObjectsOfTypeAll<InvisibleEntityManager>().First();
                        MelonLogger.Log("Darkwalker object has been restored");
                    }
                    if (DarkWalkerHUD == null)
                    {
                        DarkWalkerHUD = Resources.FindObjectsOfTypeAll<HUDNowhereToHide>().First();
                        MelonLogger.Log("Got DarkWalker HUD");
                    }
                    else
                    {
                        if (IamShatalker == false)
                        {
                            if (WardWidget == null)
                            {
                                WardWidget = DarkWalkerHUD.transform.GetChild(3).gameObject;
                                MelonLogger.Log("Got WardWidget object");
                            }
                            if(LureWidget == null)
                            {
                                LureWidget = DarkWalkerHUD.transform.GetChild(2).gameObject;
                            }

                            if (ShatalkerObject.GetStartMovementDelayTime() < 2)
                            {
                                DarkWalkerIsReady = true;
                            }
                            else
                            {
                                DarkWalkerIsReady = false;
                            }
                            if (DarkWalkerIsReady != PreviousDarkWalkerReady)
                            {
                                PreviousDarkWalkerReady = DarkWalkerIsReady;
                                MelonLogger.Log("Sending to Darkwalker my ready state");
                                if (sendMyPosition == true) // CLIENT
                                {
                                    using (Packet _packet = new Packet((int)ClientPackets.DARKWALKERREADY))
                                    {
                                        _packet.Write(DarkWalkerIsReady);
                                        SendTCPData(_packet);
                                    }
                                }
                                if (iAmHost == true) // HOST
                                {
                                    using (Packet _packet = new Packet((int)ServerPackets.DARKWALKERREADY))
                                    {
                                        ServerSend.DARKWALKERREADY(1, DarkWalkerIsReady);
                                    }
                                }
                            }
                        }
                        if (IamShatalker == true)
                        {
                            if (DistanceWidget == null || DistanceLable == null)
                            {
                                DistanceWidget = DarkWalkerHUD.transform.GetChild(1).gameObject;

                                UnityEngine.Component.Destroy(DarkWalkerHUD.transform.GetChild(1).GetChild(0).gameObject.GetComponent<UILocalize>());
                                DarkWalkerHUD.transform.GetChild(1).GetChild(0).gameObject.GetComponent<UILabel>().text = "SURVIVOR DISTANCE";
                                DistanceLable = DarkWalkerHUD.transform.GetChild(1).GetChild(1).gameObject.GetComponent<UILabel>();
                                MelonLogger.Log("Got DistanceWidget object");
                            } else
                            {
                                if (DarkWalkerIsReady == true)
                                {
                                    DistanceWidget.SetActive(true);
                                    DarkWalkerHUD.transform.GetChild(0).gameObject.SetActive(false);
                                    DarkWalkerHUD.enabled = false;
                                    DarkWalkerHUD.transform.GetChild(0).gameObject.SetActive(false);
                                }
                                if (anotherbutt != null)
                                {
                                    float dist = 0;
                                    bool HavePossition = false;

                                    if (levelid == anotherplayer_levelid)
                                    {
                                        dist = Vector3.Distance(GameManager.GetPlayerTransform().position, anotherbutt.transform.position);
                                        HavePossition = true;
                                    }else{
                                        for (int i = 0; i < SurvivorWalks.Count; i++)
                                        {
                                            if (SurvivorWalks[i].m_levelid == levelid)
                                            {
                                                HavePossition = true;
                                                dist = Vector3.Distance(GameManager.GetPlayerTransform().position, SurvivorWalks[i].m_V3);
                                                break;
                                            }
                                        }
                                    }

                                    if(HavePossition == true)
                                    {
                                        if(LureIsActive && LastLure.m_levelid == levelid)
                                        {
                                            dist = Vector3.Distance(GameManager.GetPlayerTransform().position, LastLure.m_V3);
                                        }
                                        DistanceLable.text = Convert.ToInt32(dist) + " METERS";
                                    }else{
                                        DistanceLable.text = "SURVIVOR NEVER WAS HERE";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if(MenuErjan1 != null && MenuErjan2 != null && FlexFire != null)
            {
                if(FlexFire.m_Fire.GetRemainingLifeTimeSeconds() <= 0)
                {
                    int currentTagHash = MenuErjan1.GetCurrentAnimatorStateInfo(0).tagHash;
                    int neededTagHash = Animator.StringToHash("Samba Dancing");

                    if(currentTagHash != neededTagHash)
                    {
                        MenuErjan1.Play("Samba Dancing",0);
                        MenuErjan2.Play("Samba Dancing",0);
                    }
                }
            }
        }

        public static bool NoUI = false;
        public static bool ForcedUiOn = true;
        public static bool PreReleaseUI = false;

        public static void DoConnectToIp(string _ip)
        {
            instance.ip = _ip;
            instance.ConnectToServer();
        }


        public static bool AtHostMenu = false;

        public static void HostAServer()
        {
            if (iAmHost != true)
            {
                isRuning = true;

                Thread mainThread = new Thread(new ThreadStart(MainThread));
                ServerHandle.SetMyLevelId(levelid);

                Server.Start(1, 26950);
                nextActionTime = Time.time;
                nextActionTimeAniamls = Time.time;
                iAmHost = true;
                MelonLogger.Log("Server has been runned with InGame time: " + GameManager.GetTimeOfDayComponent().GetHour() + ":" + GameManager.GetTimeOfDayComponent().GetMinutes());
                OverridedHourse = GameManager.GetTimeOfDayComponent().GetHour();
                OverridedMinutes = GameManager.GetTimeOfDayComponent().GetMinutes();
                OveridedTime = OverridedHourse + ":" + OverridedMinutes;
                ServerHandle.SetGameTime(OveridedTime);
                NeedSyncTime = true;
                RealTimeCycleSpeed = true;
            }
            else
            {
                HUDMessage.AddMessage("YOU ALREADY HOSING!!!!!!");
            }
        }

        public static void DoIPConnectWindow()
        {
            if (sendMyPosition != true)
            {
                InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input server address", "127.0.0.1", Panel_Confirmation.ButtonLayout.Button_2, "Connect", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                RealTimeCycleSpeed = true;
            }
            else
            {
                HUDMessage.AddMessage("YOU ALREADY CONNECTED!!!!");
            }
        } 

        public void PreReleaseGUI()
        {
            if (IamShatalker == true)
            {
                if (anotherbutt != null)
                {
                    if (WardIsActive == true)
                    {
                        //MelonLogger.Log("Ward is active!");
                        GUI.Label(new Rect(700, 30, 500, 100), "Ward is active!");
                    }
                }
            }

            if (level_name != "Empty" && level_name != "Boot" && level_name != "MainMenu" && GameManager.m_InterfaceManager != null && InterfaceManager.m_Panel_PauseMenu != null && InterfaceManager.m_Panel_PauseMenu.IsEnabled() == true)
            {
                GUI.Box(new Rect(10, 10, 100, 200), "Multiplayer");

                if (InOnline() == false)
                {
                    if (AtHostMenu == false)
                    {
                        if (GUI.Button(new Rect(20, 40, 80, 20), "Host"))
                        {
                            if (iAmHost != true)
                            {
                                AtHostMenu = true;
                            }
                            else
                            {
                                HUDMessage.AddMessage("YOU ALREADY HOSING!!!!!!");
                            }
                        }
                        if (GUI.Button(new Rect(20, 70, 80, 20), "Connect"))
                        {
                            if (sendMyPosition != true)
                            {
                                InDarkWalkerMode = true;
                                RealTimeCycleSpeed = false;
                                InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "Input server address", "127.0.0.1", Panel_Confirmation.ButtonLayout.Button_2, "Connect", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                            }
                            else
                            {
                                HUDMessage.AddMessage("YOU ALREADY CONNECTED!!!!");
                            }
                        }
                    }
                    else
                    {
                        GUI.Box(new Rect(10, 10, 100, 200), "Host as");
                        if (GUI.Button(new Rect(20, 40, 80, 20), "Darkwalker"))
                        {
                            IamShatalker = true;
                            uConsole.RunCommandSilent("Ghost");
                            uConsole.RunCommandSilent("God");
                            ///Host
                            isRuning = true;

                            Thread mainThread = new Thread(new ThreadStart(MainThread));
                            ServerHandle.SetMyLevelId(levelid);
                            ServerHandle.IamShatalker = true;

                            Server.Start(1, 26950);
                            nextActionTime = Time.time;
                            nextActionTimeAniamls = Time.time;
                            iAmHost = true;
                            MelonLogger.Log("Server has been runned with InGame time: " + GameManager.GetTimeOfDayComponent().GetHour() + ":" + GameManager.GetTimeOfDayComponent().GetMinutes());
                            OverridedHourse = GameManager.GetTimeOfDayComponent().GetHour();
                            OverridedMinutes = GameManager.GetTimeOfDayComponent().GetMinutes();
                            OveridedTime = OverridedHourse + ":" + OverridedMinutes;
                            RealTimeCycleSpeed = false;
                            NeedSyncTime = false;
                            InDarkWalkerMode = true;
                            AtHostMenu = false;
                        }
                        if (GUI.Button(new Rect(20, 70, 80, 20), "Survivor"))
                        {
                            ///Host
                            isRuning = true;

                            Thread mainThread = new Thread(new ThreadStart(MainThread));
                            ServerHandle.SetMyLevelId(levelid);
                            ServerHandle.IamShatalker = false;

                            Server.Start(1, 26950);
                            nextActionTime = Time.time;
                            nextActionTimeAniamls = Time.time;
                            iAmHost = true;
                            MelonLogger.Log("Server has been runned with InGame time: " + GameManager.GetTimeOfDayComponent().GetHour() + ":" + GameManager.GetTimeOfDayComponent().GetMinutes());
                            OverridedHourse = GameManager.GetTimeOfDayComponent().GetHour();
                            OverridedMinutes = GameManager.GetTimeOfDayComponent().GetMinutes();
                            OveridedTime = OverridedHourse + ":" + OverridedMinutes;
                            RealTimeCycleSpeed = false;
                            NeedSyncTime = false;
                            InDarkWalkerMode = true;
                            AtHostMenu = false;
                        }
                        if (GUI.Button(new Rect(20, 120, 80, 20), "Back"))
                        {
                            AtHostMenu = false;
                        }
                    }
                } else
                {
                    if (GUI.Button(new Rect(20, 40, 80, 20), "Disconnect"))
                    {
                        MelonLogger.Log("Disconnect case pressed disconnect button");
                        Disconnect();
                    }
                    if (IamShatalker == false && ShatalkerObject != null && ShatalkerObject.GetStartMovementDelayTime() > 2)
                    {
                        if (GUI.Button(new Rect(20, 70, 80, 20), "Skip waiting"))
                        {
                            ShatalkerObject.SetStartMovementDelayTime(0);
                        }
                    }
                }
            }
        }

        public static bool AtDebug = false;
        public static string UIDebugType = "";
        public static List<string> ItemsForDebug = new List<string>();
        public static int ItemForDebug = -1;
        public static string AdvancedDebugMode = "";
        public static bool DebugGUI = true;

        public override void OnGUI()
        {
            if(NoUI == true && ForcedUiOn == false)
            {
                return;
            }          

            if (anotherbutt != null)
            {
                float dist = Vector3.Distance(GameManager.GetPlayerTransform().position, anotherbutt.transform.position);

                if(dist < 1 && AnimState == "Knock" && CarryingPlayer == false)
                {
                    string name;
                    if(GameManager.GetInventoryComponent().HasNonRuinedItem("GEAR_MedicalSupplies_hangar") == true)
                    {
                        name = "Press N to revive\n(Requires a First Aid Kit)";
                    }else
                    {
                        name = "You need first aid kit to revive";
                    }

                    GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 25, 500, 100), name);
                }
            }

            if(DebugGUI == false)
            {
                return;
            }

            if (AdvancedDebugMode != "")
            {
                int animalcount = 0;

                if (AnimalsController == true)
                {
                    animalcount = AllAnimalsNew.Count();
                }
                else
                {
                    animalcount = AllAnimals.Count();
                }

                if (AdvancedDebugMode == "AnimalsGUID" || AdvancedDebugMode == "AnimalsAllStats")
                {
                    int offset = 20;

                    GUI.Label(new Rect(700, 10, 500, 100), "Animals around: " + animalcount + " Animal controller " + AnimalsController);
                    GUI.Label(new Rect(700, 10 + offset * 1, 500, 100), "GUID: " + DebugAnimalGUID);
                    GUI.Label(new Rect(700, 10 + offset * 2, 500, 100), "Last GUID: " + DebugAnimalGUIDLast);

                    bool stillExists = true;

                    if (DebugLastAnimal == null)
                    {
                        stillExists = false;
                    }
                    else
                    {
                        stillExists = true;
                    }
                    GUI.Label(new Rect(700, 10 + offset * 3, 500, 100), "Still exists: " + stillExists);

                    if (stillExists == true && DebugLastAnimal != null && AdvancedDebugMode == "AnimalsAllStats")
                    {
                        AnimalUpdates _AU = DebugLastAnimal.GetComponent<AnimalUpdates>();

                        if (_AU != null)
                        {
                            GUI.Label(new Rect(700, 10 + offset * 4, 500, 100), "m_UnderMyControl " + _AU.m_UnderMyControl);
                            GUI.Label(new Rect(700, 10 + offset * 5, 500, 100), "m_ClientControlled " + _AU.m_ClientControlled);
                            GUI.Label(new Rect(700, 10 + offset * 6, 500, 100), "m_InActive " + _AU.m_InActive);
                            GUI.Label(new Rect(700, 10 + offset * 7, 500, 100), "m_Banned " + _AU.m_Banned);
                            GUI.Label(new Rect(700, 10 + offset * 8, 500, 100), "m_DampingIgnore " + _AU.m_DampingIgnore);
                            GUI.Label(new Rect(700, 10 + offset * 9, 500, 100), "NoResponce " + _AU.NoResponce);
                            GUI.Label(new Rect(700, 10 + offset * 10, 500, 100), "ActiveSelf " + DebugLastAnimal.activeSelf);
                        }
                        else
                        {
                            GUI.Label(new Rect(700, 10 + offset * 4, 500, 100), "No AnimalUpdates!");
                        }
                    }
                }
            }


            GUI.Box(new Rect(10, 10, 100, 250), "Multiplayer");

            if (GUI.Button(new Rect(20, 40, 80, 20), "Host"))
            {
                HostAServer();
            }
            if (GUI.Button(new Rect(20, 70, 80, 20), "Connect"))
            {
                DoIPConnectWindow();
            }
            if (GUI.Button(new Rect(20, 100, 80, 20), "Disconnect"))
            {
                MelonLogger.Log("Disconnect case pressed disconnect button");
                Disconnect();
            }
            if (GUI.Button(new Rect(20, 130, 80, 20), "Cube"))
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);            
                Transform target = GameManager.GetPlayerTransform();
                cube.transform.position = new Vector3(target.position.x, target.position.y, target.position.z);

                if (sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.BLOCK))
                    {
                        _packet.Write(cube.transform.position);
                        SendTCPData(_packet);
                    }
                }
                if (iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.BLOCK))
                    {
                        ServerSend.BLOCK(1, cube.transform.position);
                    }
                }
            }
            if (GUI.Button(new Rect(20, 160, 80, 20), "Hour skip"))
            {
                OverridedHourse = OverridedHourse + 1;
                MyMod.EveryInGameMinute();
            }
            if (GUI.Button(new Rect(20, 190, 80, 20), "HideUI"))
            {
                ForcedUiOn = false;
                NoUI = true;
            }
            if (GUI.Button(new Rect(20, 220, 80, 20), "Debug"))
            {
                if(UIDebugType != "")
                {
                    UIDebugType = "";
                }else
                {
                    UIDebugType = "Open";
                }
            }

            if(UIDebugType == "Open")
            {
                GUI.Box(new Rect(150, 10, 100, 200), "Debug Tools");
                if (GUI.Button(new Rect(160, 40, 80, 20), "Player"))
                {
                    UIDebugType = "PlayerDebug";
                }
                if (GUI.Button(new Rect(160, 70, 80, 20), "Cans Room"))
                {
                    SendToWaitngRoom();
                }
                if (GUI.Button(new Rect(160, 100, 80, 20), "Animals"))
                {
                    UIDebugType = "AnimalsDebug";
                }
                if (GUI.Button(new Rect(160, 130, 80, 20), "Test"))
                {

                }
                if (GUI.Button(new Rect(160, 160, 80, 20), "Close"))
                {
                    UIDebugType = "";
                }
            }

            if (UIDebugType == "PlayerDebug")
            {

                GUI.Box(new Rect(150, 10, 100, 200), "Player Debug");

                if (GUI.Button(new Rect(160, 40, 80, 20), "Items"))
                {
                    UIDebugType = "PlayerDebug_Items";
                }

                if (GUI.Button(new Rect(160, 70, 80, 20), "Anims"))
                {
                    UIDebugType = "PlayerDebug_Anims";
                }
                if (GUI.Button(new Rect(160, 100, 80, 20), "Set"))
                {
                    anotherplayer_levelid = levelid;
                    LastRecivedOtherPlayerVector = GameManager.GetPlayerTransform().position;
                    anotherbutt.transform.position = GameManager.GetPlayerTransform().position;
                    anotherbutt.transform.rotation = GameManager.GetPlayerTransform().rotation;
                    int bodyparts = 9;

                    for (int i = 0; i < bodyparts; i++)
                    {
                        anotherbutt.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
                if (GUI.Button(new Rect(160, 130, 80, 20), "Outlines"))
                {
                    Outline OL = anotherbutt.GetComponent<Outline>();

                    if (OL == null)
                    {
                        anotherbutt.AddComponent<Outline>();
                        OL = anotherbutt.GetComponent<Outline>();
                        OL.m_OutlineColor = Color.green;
                        OL.needsUpdate = true;
                        MelonLogger.Log("Added outlines");
                    }else{
                        OL.m_OutlineColor = Color.clear;
                        OL.needsUpdate = true;
                    }
                }
                if (GUI.Button(new Rect(160, 160, 80, 20), "Back"))
                {
                    UIDebugType = "Open";
                }
            }

            if(UIDebugType == "PlayerDebug_Items")
            {
                GUI.Box(new Rect(150, 10, 100, 250), "Debug Items");
                if(ItemsForDebug.Count == 0)
                {
                    ItemsForDebug.Add("GEAR_WoodMatches");
                    ItemsForDebug.Add("GEAR_FlareA");
                    ItemsForDebug.Add("GEAR_BlueFlare");
                    ItemsForDebug.Add("GEAR_Torch");
                    ItemsForDebug.Add("GEAR_Rifle");
                    ItemsForDebug.Add("GEAR_Revolver");
                    ItemsForDebug.Add("GEAR_SprayPaintCanGlyphA");
                    ItemsForDebug.Add("GEAR_Stone");
                    ItemsForDebug.Add("GEAR_KeroseneLamp");
                    ItemsForDebug.Add("GEAR_FlareGun");
                    ItemsForDebug.Add("");
                }
                if (GUI.Button(new Rect(160, 40, 80, 20), "Next Item"))
                {
                    ItemForDebug = ItemForDebug+1;
                    if(ItemForDebug > ItemsForDebug.Count)
                    {
                        ItemForDebug = 0;
                    }
                    LightSourceName = ItemsForDebug[ItemForDebug];
                    UpdateHeldItem(LightSourceName);
                }
                if (GUI.Button(new Rect(160, 70, 80, 20), "Light"))
                {
                    if (LightSource == false)
                    {
                        LightSource = true;
                    }
                    else
                    {
                        LightSource = false;
                    }
                    UpdateHeldItem("nodis");
                }
                if (GUI.Button(new Rect(160, 100, 80, 20), "Has Rifle"))
                {
                    if (HasRifle == false)
                    {
                        HasRifle = true;
                    }
                    else
                    {
                        HasRifle = false;
                    }
                    UpdateHeldItem("nodis");
                }
                if (GUI.Button(new Rect(160, 130, 80, 20), "Has Rev."))
                {
                    if (HasRevolver == false)
                    {
                        HasRevolver = true;
                    }
                    else
                    {
                        HasRevolver = false;
                    }
                    UpdateHeldItem("nodis");
                }
                if (GUI.Button(new Rect(160, 160, 80, 20), "Has Axe"))
                {
                    if (HasAxe == false)
                    {
                        HasAxe = true;
                    }
                    else
                    {
                        HasAxe = false;
                    }
                    UpdateHeldItem("nodis");
                }
                if (GUI.Button(new Rect(160, 190, 80, 20), "Has Medkit"))
                {
                    if (HasMedkit == false)
                    {
                        HasMedkit = true;
                    }
                    else
                    {
                        HasMedkit = false;
                    }
                    UpdateHeldItem("nodis");
                }
                if (GUI.Button(new Rect(160, 220, 80, 20), "Back"))
                {
                    UIDebugType = "PlayerDebug";
                }
            }
            if(UIDebugType == "PlayerDebug_Anims")
            {
                GUI.Box(new Rect(150, 10, 100, 230), "Debug Anims");
                if (GUI.Button(new Rect(160, 40, 80, 20), "Idle"))
                {
                    AnimState = "Idle";
                }
                if (GUI.Button(new Rect(160, 70, 80, 20), "Walk"))
                {
                    AnimState = "Walk";
                }
                if (GUI.Button(new Rect(160, 100, 80, 20), "Run"))
                {
                    AnimState = "Run";
                }
                if (GUI.Button(new Rect(160, 130, 80, 20), "Sit"))
                {
                    if (AnimState != "Ctrl")
                    {
                        AnimState = "Ctrl";
                    }
                    else
                    {
                        AnimState = "Idle";
                    }
                }
                if (GUI.Button(new Rect(160, 160, 80, 20), "Knock"))
                {
                    AnimState = "Knock";
                }
                if (GUI.Button(new Rect(160, 190, 80, 20), "Back"))
                {
                    UIDebugType = "PlayerDebug";
                }
            }
            if(UIDebugType == "AnimalsDebug")
            {
                GUI.Box(new Rect(150, 10, 100, 270), "Debug Animals");
                if (GUI.Button(new Rect(160, 40, 80, 20), "Room"))
                {
                    SendToAnimalRoom();
                }
                if (GUI.Button(new Rect(160, 70, 80, 20), "GUID Stats"))
                {
                    if (AdvancedDebugMode == "AnimalsGUID")
                    {
                        AdvancedDebugMode = "";
                    }else{
                        AdvancedDebugMode = "AnimalsGUID";
                    }
                }
                if (GUI.Button(new Rect(160, 100, 80, 20), "All Stats"))
                {
                    if (AdvancedDebugMode == "AnimalsAllStats")
                    {
                        AdvancedDebugMode = "";
                    }
                    else
                    {
                        AdvancedDebugMode = "AnimalsAllStats";
                    }
                }
                if (GUI.Button(new Rect(160, 130, 80, 20), "TP to GUID"))
                {
                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "INPUT GUID TO TELEPORT TO", "", Panel_Confirmation.ButtonLayout.Button_2, "TELEPORT", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                }
                if (GUI.Button(new Rect(160, 160, 80, 20), "Track GUID"))
                {
                    InterfaceManager.m_Panel_Confirmation.AddConfirmation(Panel_Confirmation.ConfirmationType.Rename, "INPUT GUID TO TRACK", "", Panel_Confirmation.ButtonLayout.Button_2, "TELEPORT", "GAMEPLAY_Cancel", Panel_Confirmation.Background.Transperent, null, null);
                }
                if (GUI.Button(new Rect(160, 190, 80, 20), "Glow"))
                {
                    if(DebugLastAnimal != null)
                    {
                        Outline OL = DebugLastAnimal.GetComponent<Outline>();

                        if (OL == null)
                        {
                            DebugLastAnimal.AddComponent<Outline>();
                            OL = DebugLastAnimal.GetComponent<Outline>();
                            OL.m_OutlineColor = Color.green;
                            OL.needsUpdate = true;
                            MelonLogger.Log("Added outlines");
                        }
                        else
                        {
                            if(OL.m_OutlineColor == Color.green)
                            {
                                OL.m_OutlineColor = Color.clear;
                                OL.needsUpdate = true;
                            }else{
                                OL.m_OutlineColor = Color.green;
                                OL.needsUpdate = true;
                            }
                        }
                    }
                }
                if (GUI.Button(new Rect(160, 220, 80, 20), "Remove all"))
                {
                    for (int index = 0; index < BaseAiManager.m_BaseAis.Count; ++index)
                    {
                        GameObject animal = BaseAiManager.m_BaseAis[index].gameObject;
                        if(animal != null)
                        {
                            UnityEngine.Object.Destroy(animal);
                        }
                    }
                }
                if (GUI.Button(new Rect(160, 250, 80, 20), "Back"))
                {
                    UIDebugType = "Open";
                }
            }
        }

        public static void SetRevivedStats(bool health = true)
        {
            if(health == true)
            {
                GameManager.GetConditionComponent().m_CurrentHP = 25;
            }
            GameManager.GetFatigueComponent().m_CurrentFatigue = 25;
            GameManager.GetThirstComponent().m_CurrentThirst = 25;
            GameManager.GetHungerComponent().m_CurrentReserveCalories = GameManager.GetHungerComponent().m_MaxReserveCalories/4;
        }

        public static void WarpBody(string _GUID)
        {
            MelonLogger.Log("Trying warping with door " + _GUID);
            GameObject Door = ObjectGuidManager.Lookup(_GUID);
            if (Door != null)
            {
                MelonLogger.Log("Simulating entering to door "+ _GUID);

                if(GameManager.m_PlayerManager.PlayerIsDead() == true)
                {
                    KillAfterLoad = true;
                }
                Door.GetComponent<LoadScene>().Activate();
            }
            else{
                MelonLogger.Log("Not found door" + _GUID);
            }
        }

        [HarmonyPatch(typeof(CarryableBody), "Update")]
        public class CarryableBody_Vzlom_Djopi
        {
            public static void Prefix(CarryableBody __instance)
            {
                if(__instance.gameObject != null && __instance.gameObject.GetComponent<ObjectGuid>() && __instance.gameObject.GetComponent<ObjectGuid>().Get() != PlayerBodyGUI)
                {
                    UnityEngine.Object.Destroy(__instance.gameObject);
                }
            }
        }

        [HarmonyPatch(typeof(LoadScene), "Activate")]
        public class LoadScene_Enter
        {
            public static void Prefix(LoadScene __instance)
            {
                if(__instance.gameObject != null && __instance.gameObject.GetComponent<ObjectGuid>() != null)
                {
                    string DoorGUID = __instance.gameObject.GetComponent<ObjectGuid>().Get();

                    MelonLogger.Log("Entering door " + DoorGUID);

                    if (CarryingPlayer == true)
                    {
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.BODYWARP))
                            {
                                _packet.Write(DoorGUID);
                                SendTCPData(_packet);
                            }
                        }

                        if (iAmHost == true)
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

        [HarmonyPatch(typeof(LoadScene), "LoadLevelWhenFadeOutComplete")]
        public class LoadScene_Load
        {
            public static bool Prefix(LoadScene __instance)
            {
                NotNeedToPauseUntilLoaded = true;
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

        [HarmonyPatch(typeof(SceneManager), "OnSceneLoaded")]
        public class SceneManager_Load
        {
            public static void Postfix(SceneManager __instance)
            {
                if (level_name != "Empty" && level_name != "Boot" && level_name != "MainMenu")
                {
                    MelonLogger.Log("Loading scene finished "+level_name+" health is: "+GameManager.GetConditionComponent().m_CurrentHP);
                    if(KillAfterLoad == true && GameManager.GetConditionComponent().m_CurrentHP > 0)
                    {
                        MelonLogger.Log("Should dead but has " + GameManager.GetConditionComponent().m_CurrentHP+" health ");
                        IsDead = true;
                        GameManager.GetConditionComponent().m_CurrentHP = 0.0f;
                        SetRevivedStats(false);
                        MelonLogger.Log("Has set it to zero, now health is " + GameManager.GetConditionComponent().m_CurrentHP + " health ");
                    }

                    NotNeedToPauseUntilLoaded = false;
                }
            }
        }

        public override void OnLevelWasInitialized(int level)
        {
            MelonLogger.Log("Level initialized: " + level);
            levelid = level;         
            MelonLogger.Log("Level name: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            level_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            if (iAmHost == true)
            {
                ServerHandle.SetMyLevelId(level);
                using (Packet _packet = new Packet((int)ServerPackets.LEVELID))
                {
                    ServerSend.LEVELID(1, level);
                }            
            }
            if(sendMyPosition == true)
            {
                using (Packet _packet = new Packet((int)ClientPackets.LEVELID))
                {
                    _packet.Write(level);
                    SendTCPData(_packet);
                }
            }
            if (ShatalkerModeClient == true || ServerHandle.ReturnShatalkerMode() == true || IamShatalker == true)
            {
                ShatalkerObject = Resources.FindObjectsOfTypeAll<InvisibleEntityManager>().First();
                MelonLogger.Log("Shatalker was null");
            }
            if(IamShatalker == true)
            {
                uConsole.RunCommandSilent("Ghost");
                uConsole.RunCommandSilent("God");
            }
        }

        /// <summary>Sets an action to be executed on the main thread.</summary>
        /// <param name="_action">The action to be executed on the main thread.</param>
        public static void ExecuteOnMainThread(Action _action)
        {
            if (_action == null)
            {
                MelonLogger.Log("No action to execute on main thread!");
                return;
            }

            lock (executeOnMainThread)
            {
                executeOnMainThread.Add(_action);
                actionToExecuteOnMainThread = true;
            }
        }
        /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
        public static void UpdateMain()
        {
            if (actionToExecuteOnMainThread)
            {
                executeCopiedOnMainThread.Clear();
                lock (executeOnMainThread)
                {
                    executeCopiedOnMainThread.AddRange(executeOnMainThread);
                    executeOnMainThread.Clear();
                    actionToExecuteOnMainThread = false;
                }

                for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
                {
                    executeCopiedOnMainThread[i]();
                }
            }
        }


        public static void BashGetupDelayCamera()
        {
            PlayerStruggle Strug = GameManager.GetPlayerStruggleComponent();
            Strug.BeginReturnToStart(Strug.m_PartnerBaseAi);
            Utils.SetCameraFOVSafe(GameManager.GetMainCamera(), Strug.m_StartCameraFOV);
            GameManager.GetMainCamera().transform.position = Strug.GetOverrideGameCameraPosition();
            GameManager.GetMainCamera().transform.rotation = Strug.m_OverrideGameCamera.transform.rotation;
        }
        public static void BashGetupDelayCamera_old()
        {
            PlayerStruggle Strug = GameManager.GetPlayerStruggleComponent();
            Strug.BashGetupDelayCamera();
        }

        [HarmonyPatch(typeof(Condition), "PlayerDeath")]
        public class Condition_Test
        {
            public static bool Prefix(Condition __instance)
            {
                MelonLogger.Log("[Condition] PlayerDeath");
                if (InOnline() == true)
                {
                    //InterfaceManager.m_Panel_Log.Enable(false);
                    InterfaceManager.m_Panel_HUD.m_Sprite_SystemFadeOverlay.enabled = false;
                    __instance.PlayPlayerDeathAudio();
                    GameManager.GetPlayerManagerComponent().UnequipImmediate(false);
                    GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Dead);
                    if(GameManager.m_PlayerStruggle != null && GameManager.m_PlayerStruggle.m_PartnerBaseAi != null)
                    {
                        GameManager.m_PlayerStruggle.m_PlayerDied = false;
                        if (GameManager.m_PlayerStruggle.m_PartnerBaseAi.gameObject.name.Contains("WILDLIFE_Bear") != true && GameManager.m_PlayerStruggle.m_PartnerBaseAi.gameObject.name.Contains("WILDLIFE_Moose") != true)
                        {
                            MelonLogger.Log("[Condition] DoFakeGetup");
                            GameManager.GetPlayerStruggleComponent().MakePartnerFlee();
                            DoFakeGetup = true;
                        }
                    }
                    return false;
                }else
                {
                    return true;
                }
            }
        }
        public static bool NeedDoBearDamage = false;
        public static bool NeedDoMooseDamage = false;
        [HarmonyPatch(typeof(PlayerStruggle), "GetUpAnimationComplete")]
        public class PlayerStruggle_Over
        {
            public static void Prefix(PlayerStruggle __instance)
            {
                MelonLogger.Log("[PlayerStruggle] Getup done");
                DoFakeGetup = false;
                if (LastStruggleAnimalName.Contains("WILDLIFE_Bear") == true)
                {
                    MelonLogger.Log("[PlayerStruggle] Struggle with bear complete, doing late damage");
                    GameManager.GetConditionComponent().m_NeverDie = false;
                    NeedDoBearDamage = true;
                    __instance.ApplyBearDamageAfterStruggleEnds();
                }
                if (LastStruggleAnimalName.Contains("WILDLIFE_Moose") == true)
                {
                    MelonLogger.Log("[PlayerStruggle] Struggle with moose complete, doing late damage");
                    GameManager.GetConditionComponent().m_NeverDie = false;
                    NeedDoMooseDamage = true;
                    __instance.ApplyMooseDamageAfterStruggleEnds();
                }
            }
        }

        [HarmonyPatch(typeof(WildlifeItem), "ProcessInteraction")]
        public class WildlifeItem_Pickup
        {
            public static bool Prefix(WildlifeItem __instance)
            {
                MelonLogger.Log("[WildlifeItem] ProcessInteraction");
                if (!GameManager.GetPlayerAnimationComponent().CanInteract())
                {
                    return false;
                }

                if(__instance.gameObject.GetComponent<ObjectGuid>() != null )
                {
                    MelonLogger.Log("[WildlifeItem] Pickedup " + __instance.gameObject.name + " "+ __instance.gameObject.GetComponent<ObjectGuid>().Get());
                    if (iAmHost == true)
                    {
                        using (Packet _packet = new Packet((int)ServerPackets.ANIMALDELETE))
                        {
                            ServerSend.ANIMALDELETE(1, __instance.gameObject.GetComponent<ObjectGuid>().Get());
                        }
                    }

                    if (sendMyPosition == true)
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

        public static GearItem SaveThrowingItem = null;

        [HarmonyPatch(typeof(PlayerManager), "ReleaseThrownObject")]
        public class PlayerManager_Throw
        {
            public static void Prefix(PlayerManager __instance)
            {
                if(__instance.m_ThrownItem != null)
                {
                    MelonLogger.Log("[PlayerManager][PreFix] ReleaseThrownObject " + __instance.m_ThrownItem.m_GearName);
                    SaveThrowingItem = __instance.m_ThrownItem;
                }else{
                    MelonLogger.Log("[PlayerManager][PreFix] Trying throw NULL somehow, wot?");
                }
            }
            public static void Postfix(PlayerManager __instance)
            {
                if (SaveThrowingItem != null)
                {
                    MelonLogger.Log("[PlayerManager][Postfix] ReleaseThrownObject SaveThrowingItem " + SaveThrowingItem.name);

                    if(SaveThrowingItem.name.StartsWith("GEAR_Stone"))
                    {
                        Vector3 V3 = SaveThrowingItem.transform.position;
                        Quaternion Qu = SaveThrowingItem.transform.rotation;

                        MelonLogger.Log("[PlayerManager][Postfix] Throwing stone " + V3.x + " y " + V3.y + " z " + V3.z);

                        ShootSync stone = new ShootSync();
                        stone.m_position = V3;
                        stone.m_rotation = Qu;
                        stone.m_projectilename = "GEAR_Stone";
                        stone.m_skill = 0;
                        stone.m_camera_forward = GameManager.GetVpFPSCamera().transform.forward;
                        stone.m_camera_right = GameManager.GetVpFPSCamera().transform.right;
                        stone.m_camera_up = GameManager.GetVpFPSCamera().transform.up;
                        if (sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.SHOOTSYNC))
                            {
                                _packet.Write(stone);
                                SendTCPData(_packet);
                            }
                        }
                        if (iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.SHOOTSYNC))
                            {
                                ServerSend.SHOOTSYNC(1, stone);
                            }
                        }
                    }
                }
                else
                {
                    MelonLogger.Log("[PlayerManager][PostFix] SaveThrowingItem is NULL");
                }
            }
        }
        //this.GetUpAnimationComplete();

        //[HarmonyPatch(typeof(PlayerStruggle), "OnPlayerDeath")]
        //public class PlayerStruggle_Test
        //{
        //    public static bool Prefix(PlayerStruggle __instance)
        //    {
        //        MelonLogger.Log("[PlayerStruggle] OnPlayerDeath");
        //        if (__instance.m_PartnerBaseAi != null && __instance.m_PartnerBaseAi.m_AiBear != null)
        //        {
        //            if (InOnline() == true)
        //            {
        //                MelonLogger.Log("[PlayerStruggle] Ignore death with bear in online.");
        //                GameManager.GetConditionComponent().m_CurrentHP = 1;
        //                GameManager.GetPlayerManagerComponent().SetControlMode(PlayerControlMode.Normal);
        //                return false;
        //            }else{
        //                return true;
        //            }
        //        }else{
        //            return true;
        //        }
        //    }
        //}
        [HarmonyPatch(typeof(Condition), "ForceStartEarRinging")]
        public class Condition_EarRining
        {
            public static bool Prefix(Condition __instance)
            {
                MelonLogger.Log("[Condition] ForceStartEarRinging");
                if(InOnline() == true)
                {
                    MelonLogger.Log("Killing annoying ear ring effect after fight with moose in multiplayer, cause it getting bugged when baseAI controller is off.");
                    return false;
                }else{
                    return true;
                }
            }
        }
        [HarmonyPatch(typeof(PlayerStruggle), "ApplyBearDamageAfterStruggleEnds")]
        public class PlayerStruggle_BearDamage
        {
            public static bool Prefix(PlayerStruggle __instance)
            {
                if(InOnline() == true)
                {
                    if(NeedDoBearDamage == true)
                    {
                        NeedDoBearDamage = false;
                        return true;
                    }else{
                        return false;
                    }
                }else{
                    return true;
                }
            }
        }
        [HarmonyPatch(typeof(PlayerStruggle), "ApplyMooseDamageAfterStruggleEnds")]
        public class PlayerStruggle_MooseDamage
        {
            public static bool Prefix(PlayerStruggle __instance)
            {
                if (InOnline() == true)
                {
                    if (NeedDoMooseDamage == true)
                    {
                        NeedDoMooseDamage = false;
                        return true;
                    }else{
                        return false;
                    }
                }else{
                    return true;
                }
            }
        }

        public static string LastStruggleAnimalName = "";
        [HarmonyPatch(typeof(PlayerStruggle), "Begin", new System.Type[] { typeof(GameObject) })]
        public class PlayerStruggle_Begin
        {
            public static void Prefix(PlayerStruggle __instance, GameObject partner)
            {
                LastStruggleAnimalName = partner.name;
                MelonLogger.Log("[PlayerStruggle] Begin struggle with " + LastStruggleAnimalName);
                ////WILDLIFE_Moose
                if(InOnline() == true)
                {
                    if(LastStruggleAnimalName.Contains("WILDLIFE_Bear") == true || LastStruggleAnimalName.Contains("WILDLIFE_Moose") == true)
                    {
                        string tauntname = "";
                        string punchline = "";
                        if (LastStruggleAnimalName.Contains("WILDLIFE_Bear") == true)
                        {
                            tauntname = "bear";
                            punchline = "wide parody of a wolf";
                        }else{
                            tauntname = "moose";
                            punchline = "step dancer";
                        }
                        MelonLogger.Log("[PlayerStruggle] This is "+ tauntname + "....fuck no, I so tired of fixing bugs with him, I just set NeverDie, cause I am so no care about this "+ punchline);
                        GameManager.GetConditionComponent().m_NeverDie = true;
                    }
                }
            }
        }
    }
}
