
using UnityEngine;
using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using SkyCoopServer;
using Il2CppTLD.Interactions;
using SkyCoopClient;
using static SkyCoopServer.DataStr;
using Harmony;
using Il2CppRewired;
using Il2CppTMPro;
using UnityEngine.UIElements;
using Il2CppNodeCanvas.BehaviourTrees;
using Il2CppNodeCanvas.StateMachines;

namespace SkyCoop
{
    public class Comps
    {
        public static void RegisterComponents()
        {
            ClassInjector.RegisterTypeInIl2Cpp<NetworkPlayer>();
            ClassInjector.RegisterTypeInIl2Cpp<OtherPlayerGear>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerDamageColider>();
            ClassInjector.RegisterTypeInIl2Cpp<OtherPlayerBullet>();
            ClassInjector.RegisterTypeInIl2Cpp<StoneThrowHook>();
            ClassInjector.RegisterTypeInIl2Cpp<NoiseMakerThrowHook>();
            ClassInjector.RegisterTypeInIl2Cpp<NoiseMakerKillFeedHandle>();
            ClassInjector.RegisterTypeInIl2Cpp<MeleeBulletHandler>();
            ClassInjector.RegisterTypeInIl2Cpp<ArrowHook>();
            ClassInjector.RegisterTypeInIl2Cpp<DroppedGearVisual>();
            ClassInjector.RegisterTypeInIl2Cpp<BulletWallBangHook>();
            ClassInjector.RegisterTypeInIl2Cpp<CameraAttention>();
            ClassInjector.RegisterTypeInIl2Cpp<DeathPackComp>();
            ClassInjector.RegisterTypeInIl2Cpp<ContainerDescriptorHook>();
            ClassInjector.RegisterTypeInIl2Cpp<NetworkPlayerDummy>();
            ClassInjector.RegisterTypeInIl2Cpp<DangerCircleZone>();
            ClassInjector.RegisterTypeInIl2Cpp<ForcedFire>();
            ClassInjector.RegisterTypeInIl2Cpp<CardGameProp>();
            ClassInjector.RegisterTypeInIl2Cpp<TexasHoldEmProp>();
            ClassInjector.RegisterTypeInIl2Cpp<TexasHoldEmPlayer>();
            ClassInjector.RegisterTypeInIl2Cpp<TexasHoldEmJoin>();
            ClassInjector.RegisterTypeInIl2Cpp<TexasHoldEmPlay>();
            ClassInjector.RegisterTypeInIl2Cpp<TalkingFish>();
            ClassInjector.RegisterTypeInIl2Cpp<PropsEditorVisuzlier>();
            ClassInjector.RegisterTypeInIl2Cpp<GenericStatusBarSpawnerHook>();
            ClassInjector.RegisterTypeInIl2Cpp<TeammateBar>();
        }

        public class UiButtonPressHook : MonoBehaviour
        {
            public UiButtonPressHook(IntPtr ptr) : base(ptr) { }
            public int m_CustomId = 0;
            public string m_PanelHandle = "";
        }
        public class UiButtonKeyboardPressSkip : MonoBehaviour
        {
            public UiButtonKeyboardPressSkip(IntPtr ptr) : base(ptr) { }
            public Il2CppSystem.Collections.Generic.List<Il2Cpp.EventDelegate> m_Click;
            public Il2CppSystem.Collections.Generic.List<Il2Cpp.EventDelegate> m_DoubleClick;
            public Il2CppSystem.Collections.Generic.List<Il2Cpp.EventDelegate> m_DoubleDoubleClick;
        }

        public class UiButtonSettingHook : MonoBehaviour
        {
            public UiButtonSettingHook(IntPtr ptr) : base(ptr) { }
            public GameObject m_Background = null;
        }

        public class OtherPlayerGear : MonoBehaviour
        {
            public OtherPlayerGear(IntPtr ptr) : base(ptr) { }
            public string m_GearName = "";
            public NetworkPlayer.GearHandPose m_HandPose = 0; 
        }
        public class OtherPlayerBullet : MonoBehaviour
        {
            public OtherPlayerBullet(IntPtr ptr) : base(ptr) { }
        }
        public class NoiseMakerKillFeedHandle : MonoBehaviour
        {
            public NoiseMakerKillFeedHandle(IntPtr ptr) : base(ptr) { }
            public int m_ThrowerID = -1;
        }

        public class MeleeBulletHandler : MonoBehaviour
        {
            public MeleeBulletHandler(IntPtr ptr) : base(ptr) { }
            public string m_GearName = "";
        }
        public class ArrowHook : MonoBehaviour
        {
            public ArrowHook(IntPtr ptr) : base(ptr) { }
            public bool m_InflictedDamageOnce = false;
            public bool m_Broken = false;

            void Update()
            {
                Comps.OtherPlayerBullet Other = GetComponent<Comps.OtherPlayerBullet>();
                Rigidbody Body = GetComponent<Rigidbody>();
                if (Body && Body.isKinematic)
                {
                    if (Other)
                    {
                        UnityEngine.Object.Destroy(gameObject);
                    }
                    else
                    {
                        if (!m_Broken)
                        {
                            GearsSync.SendDropItem(gameObject.GetComponent<GearItem>(), 0, 0, true);
                        }
                        UnityEngine.Object.Destroy(gameObject);
                    }
                }
            }
        }

        public class DroppedGearVisual : MonoBehaviour
        {
            public DroppedGearVisual(IntPtr ptr) : base(ptr) { }
            public string m_GUID = "";
            public string m_LocalizedName = "GearItem";

            void Start()
            {
                LocalizedString Str = new LocalizedString();
                Str.m_LocalizationID = m_LocalizedName;
                SimpleInteraction SI = gameObject.AddComponent<SimpleInteraction>();
                SI.m_DefaultHoverText = Str;
                SI.HoverText = m_LocalizedName;
                SI.m_CanInteract = true;
            }
        }

        public class StoneThrowHook : MonoBehaviour
        {
            public StoneThrowHook(IntPtr ptr) : base(ptr) { }
            public StoneItem m_StoneItem;
            public bool m_CanDamage = true;
            public bool m_SendThrown = false;
            void Update()
            {
                if(m_StoneItem == null)
                {
                    m_StoneItem = GetComponent<StoneItem>();
                }

                if (m_StoneItem)
                {
                    if (m_StoneItem.m_Thrown && !m_SendThrown)
                    {
                        m_SendThrown = true;
                        Rigidbody Body = GetComponent<Rigidbody>();
                        ClientSend.SendProjectileThrow(m_StoneItem.transform.position, m_StoneItem.transform.rotation, "GEAR_Stone", Body.velocity, Body.angularVelocity, 0);
                    }
                    if (m_StoneItem.m_RigidBody && m_StoneItem.m_RigidBody.isKinematic)
                    {
                        if(m_SendThrown && GetComponent<OtherPlayerBullet>() == null)
                        {
                            GearsSync.SendDropItem(GetComponent<GearItem>(), 0, 0, true);
                        }
                        UnityEngine.Object.Destroy(gameObject);
                    }
                }
            }
        }
        public class NoiseMakerThrowHook : MonoBehaviour
        {
            public NoiseMakerThrowHook(IntPtr ptr) : base(ptr) { }
            public NoiseMakerItem m_NoiseMaker;
            void Update()
            {
                if (m_NoiseMaker == null)
                {
                    m_NoiseMaker = GetComponent<NoiseMakerItem>();
                }
                if (m_NoiseMaker && m_NoiseMaker.m_Thrown)
                {
                    Rigidbody Body = GetComponent<Rigidbody>();
                    ClientSend.SendProjectileThrow(m_NoiseMaker.transform.position, m_NoiseMaker.transform.rotation, "GEAR_NoiseMaker", Body.velocity, Body.angularVelocity, m_NoiseMaker.m_GearItem.GetNormalizedCondition());
                    UnityEngine.Object.Destroy(this);
                }
            }
        }

        public class BulletWallBangHook : MonoBehaviour
        {
            public BulletWallBangHook(IntPtr ptr) : base(ptr) { }
            public int m_WallBangs = 3;
            public Quaternion m_ShootRotation = Quaternion.identity;
            public Vector3 m_ShootPosition = Vector3.zero;
            public Vector3 m_ShootDirection = Vector3.zero;
        }

        public class PlayerDamageColider : MonoBehaviour
        {
            public PlayerDamageColider(IntPtr ptr) : base(ptr) { }
            public NetworkPlayer m_Player = null;
            public float m_DamageScaler = 1;
            public DamageZone m_DamageZone = PlayerDamageColider.DamageZone.Head;
            public int m_ColiderIndex = 0;
            public List<GameObject> m_InjectedItems = new List<GameObject>();

            public enum DamageZone
            {
                Head = 0,
                Chest = 1,
                RightArm = 2,
                LeftArm = 3,
                RightLeg = 4,
                LeftLeg = 5,
            }

            public void Start()
            {
                //AkSoundEngine.SetSwitch(SWITCHES.MATERIALTAG.GROUP, SWITCHES.MATERIALTAG.SWITCH.FLESH, GameAudioManager.GetSoundEmitterFromGameObject(m_Player.gameObject));

                string ObjName = gameObject.name;

                if (ObjName != "GEAR_CookingPot")
                {
                    gameObject.tag = "Flesh";
                }
                if (ObjName.StartsWith("Spine"))
                {
                    m_DamageScaler = 1;
                    m_DamageZone = DamageZone.Chest;
                }
                else if (ObjName.StartsWith("arms") || ObjName.StartsWith("hand"))
                {
                    m_DamageScaler = 0.8f;
                    if (ObjName.Contains("_l_"))
                    {
                        m_DamageZone = DamageZone.LeftArm;
                    }
                    else
                    {
                        m_DamageZone = DamageZone.RightArm;
                    }
                }
                else if (ObjName == "Head" || ObjName == "GEAR_CookingPot")
                {
                    m_DamageScaler = 1.5f;
                    m_DamageZone = DamageZone.Head;
                }
                else if (ObjName.StartsWith("Thigh") || ObjName.StartsWith("Shin") || ObjName.StartsWith("Foot"))
                {
                    m_DamageScaler = 0.5f;
                    if (ObjName.Contains(".R"))
                    {
                        m_DamageZone = DamageZone.LeftLeg;
                    }
                    else
                    {
                        m_DamageZone = DamageZone.RightLeg;
                    }
                }
            }
            public void OnCollisionEnter(Collision col)
            {
                if (col.gameObject.GetComponent<ArrowItem>() != null)
                {
                    if (col.gameObject.GetComponent<Comps.OtherPlayerBullet>() == null)
                    {
                        Comps.ArrowHook Hook = col.gameObject.GetComponent<Comps.ArrowHook>();

                        if(Hook && !Hook.m_InflictedDamageOnce)
                        {
                            ArrowItem ARR = col.gameObject.GetComponent<ArrowItem>();
                            ARR.m_ArrowMesh.GetComponent<BoxCollider>().enabled = false;
                            SkyCoop.Logger.Log("Arrow colided other player, and dealing damage");
                            WeaponsManager.WeaponDescripter Descriptor = WeaponsManager.GetDescriptor(col.gameObject.name);
                            ClientSend.SendDamageToPlayer(Descriptor.m_PlayerDamage, m_Player.m_PlayerID, m_DamageZone, col.gameObject.name, Descriptor.m_DamageType);
                            ClientSend.SendInjectedItem(m_Player.m_PlayerID, col.gameObject.name, m_ColiderIndex, m_DamageZone, col.gameObject.transform.localPosition, col.gameObject.transform.localRotation);
                            Hook.m_InflictedDamageOnce = true;
                        }
                    }
                    UnityEngine.Object.Destroy(col.gameObject);
                }
                if (col.gameObject.GetComponent<FlareGunRoundItem>() != null)
                {
                    col.gameObject.layer = vp_Layer.Trigger;
                    WeaponsManager.WeaponDescripter Descriptor = WeaponsManager.GetDescriptor("GEAR_FlareGun");
                    ClientSend.SendDamageToPlayer(Descriptor.m_PlayerDamage * m_DamageScaler, m_Player.m_PlayerID, m_DamageZone, "GEAR_FlareGun", Descriptor.m_DamageType);
                    col.transform.SetParent(null);
                }
                if (col.gameObject.GetComponent<NoiseMakerItem>() != null && col.gameObject.GetComponent<Comps.OtherPlayerBullet>() == null)
                {
                    WeaponsManager.WeaponDescripter Descriptor = WeaponsManager.GetDescriptor("GEAR_NoiseMaker");
                    ClientSend.SendDamageToPlayer(Descriptor.m_PlayerDamage * m_DamageScaler, m_Player.m_PlayerID, m_DamageZone, "GEAR_NoiseMaker", Descriptor.m_DamageType);
                }
                if (col.gameObject.GetComponent<StoneItem>() != null && col.gameObject.GetComponent<Comps.OtherPlayerBullet>() == null)
                {
                    Comps.StoneThrowHook StoneHook = col.gameObject.GetComponent<Comps.StoneThrowHook>();
                    if (StoneHook.m_CanDamage)
                    {
                        WeaponsManager.WeaponDescripter Descriptor = WeaponsManager.GetDescriptor("GEAR_Stone");
                        ClientSend.SendDamageToPlayer(Descriptor.m_PlayerDamage * m_DamageScaler, m_Player.m_PlayerID, m_DamageZone, "GEAR_Stone", Descriptor.m_DamageType);
                        StoneHook.m_CanDamage = false;
                    }
                }
            }

            public void InjectItem(string GearName, Vector3 Position, Quaternion Rotation)
            {
                GameObject Reference = AssetManager.CreateBogusGear(GearName);
                if (Reference)
                {
                    GameObject Item = Instantiate<GameObject>(Reference, transform);
                    if (Item)
                    {
                        Item.transform.SetLocalPositionAndRotation(Position, Rotation);
                        Item.layer = vp_Layer.Decoration;
                    }
                    Item.name = GearName;
                    m_InjectedItems.Add(Item);
                }
            }

            public bool RemoveInjectedItem(string GearName)
            {
                if(m_InjectedItems.Count == 0)
                {
                    return false;
                }

                for (int i = 0; i < m_InjectedItems.Count; i++)
                {
                    GameObject Item = m_InjectedItems[i];
                    if(Item.name == GearName)
                    {
                        m_InjectedItems.RemoveAt(0);
                        Destroy(Item);
                        return true;
                    }
                }
                return false;
            }
        }

        public class NetworkPlayer : MonoBehaviour
        {
            public NetworkPlayer(IntPtr ptr) : base(ptr) { }
            public int m_PlayerID = 0;
            public string m_PlayerName = "";
            public Vector3 m_Position = Vector3.zero;
            public Quaternion m_Rotation = Quaternion.identity;
            public float m_SecondsBeforeHide = 5f;
            public Animator m_Animator = null;
            public Vector3 m_LastPosition = Vector3.zero;
            public float m_MinimalSpeed = 0.1f;
            public float m_Smoother = 0.1f;
            public GearHandPose m_GearHandPose = GearHandPose.None;
            public Actions m_Action = Actions.None;
            public AudioSource m_AudioSource3D;
            public AudioSource m_AudioSource2D;
            public AudioSource m_AudioSourceRadio;
            public AudioSource m_AudioSourceRadioBG;
            public List<Collider> m_PlayerColiders = new List<Collider>();
            public GameObject m_Helmet = null;
            public GameObject m_Satchel = null;
            public GameObject m_TechnicalBackpack = null;
            public Transform m_BottomLip = null;
            public float m_MouthMinY = 0.03f;
            public float m_MouthMaxY = 0.053f;
            public float m_MouthLerpScaler = 50;
            public float m_MouthLerpSmoother = 0;
            public AudioClip m_LastVoiceSample = null;
            public AudioClip m_LastRadioSample = null;
            public float m_SampleVoiceSeek = 0;
            public float m_SampleRadioSeek = 0;
            public int m_SampleVoiceWindow = 64;
            public Vector3 m_InVehicleOffset = new Vector3(0, 0.21f, 0.21f);

            public GameObject m_HairMesh = null;
            public GameObject m_BeardMesh = null;
            public GameObject m_EyebrowsMesh = null;

            public AudioSource m_TalkingFishAudioSource;

            public CameraAttention m_CameraAttention;

            public enum GearHandPose
            {
                None = 0,
                Pistol = 1,
                Rifle = 2,
                Lantern = 3,
                GenericHold = 4,
                Matches = 5,
                Bow = 6,
            }

            public enum Actions
            {
                None = 0,
                Harvesting = 1,
                PistolAim = 2,
                RifleAim = 3,
                Igniting = 4,
                Death = 5,
                Knocked = 6,
            }

            public DataStr.PlayerVisualData m_VisualData = new DataStr.PlayerVisualData();

            public List<OtherPlayerGear> m_VisualGears = new List<OtherPlayerGear>();
            public List<GameObject> m_ClothingMeshes = new List<GameObject>();


            float s_DeltaMultiplayer = 20;
            float s_InterpolationSkipDistance = 3f;
            float s_InActiveCooldown = 5f;

            public void SetTransform(Vector3 position, Quaternion rotation)
            {
                m_Position = position;
                m_Rotation = rotation;
                KeepVisible();
            }

            public void SetPosition(Vector3 position)
            {
                m_Position = position;
                KeepVisible();
            }

            public void SetRotation(Quaternion rotation)
            {
                m_Rotation = rotation;
                KeepVisible();
            }

            public void KeepVisible()
            {
                m_SecondsBeforeHide = s_InActiveCooldown;
                gameObject.SetActive(true);
            }

            public void SetAcation(int Action)
            {
                m_Action = (Actions)Action;
                if (m_Animator)
                {
                    m_Animator.SetInteger("Action", Action);
                }
            }

            public void DoFire()
            {
                if (m_Animator)
                {
                    m_Animator.SetTrigger("Shoot");
                }
            }

            public void DoHit()
            {
                if (m_Animator)
                {
                    m_Animator.SetTrigger("Hit");
                }
            }

            public void DoThrow()
            {
                if (m_Animator)
                {
                    m_Animator.SetTrigger("Throw");
                }
            }

            public void DoGetDamage()
            {
                if (m_Animator)
                {
                    m_Animator.SetTrigger("Damaged");
                }
            }

            public void DoFishTalk()
            {
                if (m_TalkingFishAudioSource)
                {
                    m_TalkingFishAudioSource.Play();
                }
            }

            public void SetGear(string GearName, int GearVariant)
            {
                m_VisualData.m_GearInHands = GearName;
                m_VisualData.m_GearVariant = GearVariant;

                if (string.IsNullOrEmpty(GearName))
                {
                    m_GearHandPose = GearHandPose.None;
                    if (m_Animator)
                    {
                        m_Animator.SetInteger("Gear", (int)m_GearHandPose);
                    }
                }

                foreach (OtherPlayerGear Gear in m_VisualGears)
                {
                    if(Gear.m_GearName == GearName)
                    {
                        m_GearHandPose = Gear.m_HandPose;
                        if (m_Animator)
                        {
                            m_Animator.SetInteger("Gear", (int)m_GearHandPose);
                        }
                    }
                    
                    Gear.gameObject.SetActive(Gear.m_GearName == GearName);
                }
            }

            public void SetCrouching(bool IsCrouching)
            {
                m_VisualData.m_Crouch = IsCrouching;
            }

            public void SetInVehicle(bool InVehicle)
            {
                m_VisualData.m_InVehicle = InVehicle;
            }

            public bool OneOfHatsIsThis(string RequiredHat)
            {
                return m_VisualData.m_ClothingData.m_Hat1 == RequiredHat || m_VisualData.m_ClothingData.m_Hat2 == RequiredHat;
            }

            public bool CanShowHairs()
            {
                return m_VisualData.m_ClothingData.m_Hat1 == "" && m_VisualData.m_ClothingData.m_Hat1 == "";
            }

            public bool CanShowBeard()
            {
                if(OneOfHatsIsThis("GEAR_Balaclava") ||
                    OneOfHatsIsThis("GEAR_WoolWrap") ||
                    OneOfHatsIsThis("GEAR_WoolWrapCap"))
                {
                    return false;
                }
                return true;
            }

            public bool CanShowEyebrows()
            {
                if (OneOfHatsIsThis("GEAR_Balaclava"))
                {
                    return false;
                }
                return true;
            }

            public void UpdateClothing()
            {
                if (m_Helmet)
                {
                    m_Helmet.SetActive(OneOfHatsIsThis("GEAR_CookingPot"));
                }
                if (m_Satchel)
                {
                    m_Satchel.SetActive(m_VisualData.m_ClothingData.HasThis("GEAR_MooseHideBag"));
                }
                if (m_TechnicalBackpack)
                {
                    m_TechnicalBackpack.SetActive(m_VisualData.m_ClothingData.m_TechPack);
                }
                if (m_HairMesh)
                {
                    m_HairMesh.SetActive(CanShowHairs());
                }
                if (m_BeardMesh)
                {
                    m_BeardMesh.SetActive(CanShowBeard());
                }
                if (m_EyebrowsMesh)
                {
                    m_EyebrowsMesh.SetActive(CanShowEyebrows());
                }
                ClothingData Data = m_VisualData.m_ClothingData;
                foreach (GameObject Mesh in m_ClothingMeshes)
                {
                    bool HasIt = Data.HasThis(Mesh.name);
                    Mesh.SetActive(HasIt);
                    if (HasIt)
                    {
                        float DamageFloat = 0;
                        if(Mesh.name == Data.m_Hat1)
                        {
                            DamageFloat = Data.m_Hat1Damage;
                        }else if(Mesh.name == Data.m_Hat2)
                        {
                            DamageFloat = Data.m_Hat2Damage;
                        }
                        else if (Mesh.name == Data.m_Body)
                        {
                            DamageFloat = Data.m_BodyDamage;
                        }
                        else if (Mesh.name == Data.m_Gloves)
                        {
                            DamageFloat = Data.m_GlovesDamage;
                        }
                        else if (Mesh.name == Data.m_Pants)
                        {
                            DamageFloat = Data.m_PantsDamage;
                        }
                        else if (Mesh.name == Data.m_Boots)
                        {
                            DamageFloat = Data.m_BootsDamage;
                        }
                        Mesh.GetComponent<Renderer>().material.SetFloat("_blend_amt", DamageFloat);
                    }
                }
            }

            public void SetClothing(DataStr.ClothingData Data)
            {
                m_VisualData.m_ClothingData = Data;
                UpdateClothing();
            }

            public static Transform GetBone(Animator Animator, HumanBodyBones Bone)
            {
                if (Animator.isHuman)
                {
                    Transform T = Animator.GetBoneTransform(Bone);
                    if (T == null)
                    {
                        Logger.Log(System.ConsoleColor.Red, Animator.gameObject.name + " does not have " + Bone.ToString());
                    }

                    return T;
                } else
                {
                    Logger.Log(System.ConsoleColor.Red, "Can't get bone of " + Animator.gameObject.name + ", because this object is not Humanoid type!");
                    return null;
                }
            }

            public void AddInjectedItem(string GearName, int ObjectID, Vector3 Positon, Quaternion Rotation)
            {
                Collider Col = m_PlayerColiders[ObjectID];
                if (Col)
                {
                    Col.GetComponent<Comps.PlayerDamageColider>().InjectItem(GearName, Positon, Rotation);
                }
            }

            public void RemoveInjectedItem(string GearName, Comps.PlayerDamageColider.DamageZone DamageZone)
            {
                foreach (Collider col in m_PlayerColiders)
                {
                    if (col)
                    {
                        Comps.PlayerDamageColider Comp = col.GetComponent<Comps.PlayerDamageColider>();

                        if(Comp.m_DamageZone == DamageZone)
                        {
                            if(Comp.RemoveInjectedItem(GearName) == true)
                            {
                                return;
                            }
                        }
                    }
                }
            }

            public void AddSpectatorTarget()
            {
                m_CameraAttention = gameObject.AddComponent<Comps.CameraAttention>();
                m_CameraAttention.enabled = false;
                m_CameraAttention.m_OffsetTranform = transform.FindChild("SpectaterView");
            }

            public void AddInteraction()
            {
                LocalizedString Str = new LocalizedString();
                Str.m_LocalizationID = "Типок";
                SimpleInteraction SI = gameObject.AddComponent<SimpleInteraction>();
                SI.m_DefaultHoverText = Str;
                SI.HoverText = "Типок";
                SI.m_CanInteract = true;
                InteractionEventEntry Event = new InteractionEventEntry();
                Event.m_EventType = InteractionEventType.PerformInteraction;
                SI.m_EventEntries.Add(Event);
            }

            public void UpdateName()
            {
                SimpleInteraction SI = gameObject.GetComponent<SimpleInteraction>();
                if (SI)
                {
                    SI.HoverText = m_PlayerName;
                    SI.m_DefaultHoverText.m_LocalizationID = m_PlayerName;
                }
            }

            public void LoadEquipment()
            {
                AddPlaceholderHoldingGear(this, "GEAR_Rifle", new Vector3(-0.23f, 0.32f, -0.047f), new Vector3(75, 90, 0), GearHandPose.Rifle);
                AddPlaceholderHoldingGear(this, "GEAR_Revolver", new Vector3(0, 0.15f, -0.06f), new Vector3(90, 0, 0), GearHandPose.Pistol);
                AddPlaceholderHoldingGear(this, "GEAR_Bow");
                AddPlaceholderHoldingGear(this, "GEAR_FlareGun", new Vector3(0.05f, 0.14f, -0.07f), new Vector3(90, 0, 0), GearHandPose.Pistol);

                AddPlaceholderHoldingGear(this, "GEAR_Stone", new Vector3(0, 0.095f, -0.053f), new Vector3(0, 0, 0));
                AddPlaceholderHoldingGear(this, "GEAR_NoiseMaker", new Vector3(0.03f, 0.08f, -0.05f), new Vector3(-30, 0, 0));

                AddPlaceholderHoldingGear(this, "GEAR_SprayPaintCanGlyphA");

                AddPlaceholderHoldingGear(this, "GEAR_WoodMatches");
                AddPlaceholderHoldingGear(this, "GEAR_PackMatches");

                AddPlaceholderHoldingGear(this, "GEAR_KeroseneLampB", GearHandPose.Lantern);
                AddPlaceholderHoldingGear(this, "GEAR_BlueFlare");
                AddPlaceholderHoldingGear(this, "GEAR_FlareA");
                AddPlaceholderHoldingGear(this, "GEAR_Torch");

                AddPlaceholderHoldingGear(this, "GEAR_EmergencyStim", new Vector3(0.01f, 0.07f, -0.047f), new Vector3(0, 6, 0), GearHandPose.GenericHold);

                AddPlaceholderHoldingGear(this, "GEAR_Hatchet", new Vector3(0.1f, 0.135f, -0.05f), new Vector3(90, 180, 180));
                AddPlaceholderHoldingGear(this, "GEAR_HatchetImprovised", new Vector3(0.05f, 0.09f, -0.05f), new Vector3(90, 180, 180), GearHandPose.GenericHold);
                AddPlaceholderHoldingGear(this, "GEAR_Knife", new Vector3(0.09f, 0.11f, -0.061f), new Vector3(75, 0, 0), GearHandPose.GenericHold);
                AddPlaceholderHoldingGear(this, "GEAR_KnifeImprovised", new Vector3(0.09f, 0.11f, -0.061f), new Vector3(75, 0, 0));
                AddPlaceholderHoldingGear(this, "GEAR_JeremiahKnife", new Vector3(0.09f, 0.11f, -0.061f), new Vector3(75, 0, 0));
                AddPlaceholderHoldingGear(this, "GEAR_KnifeScrapMetal", new Vector3(0.08f, 0.11f, -0.051f), new Vector3(0, 270, 300));
                AddPlaceholderHoldingGear(this, "GEAR_Hammer", new Vector3(0.09f, 0.11f, -0.1f), new Vector3(80, 0, 0));
                AddPlaceholderHoldingGear(this, "GEAR_Prybar", new Vector3(0.09f, 0.1f, -0.02f), new Vector3(350, 0, 0));

                GameObject FishKnife = AddPlaceholderHoldingGearFromBundle(this, "TalkingFish", "GEAR_FishKnife", new Vector3(0.09f, 0.07f, -0.085f), new Vector3(75, 0, 0), GearHandPose.GenericHold);

                if (FishKnife)
                {
                    Comps.TalkingFish Comp = FishKnife.AddComponent<Comps.TalkingFish>();
                    Comp.m_AudioSource = FishKnife.GetComponent<AudioSource>();
                    Comp.SetupMoth();
                    m_TalkingFishAudioSource = Comp.m_AudioSource;
                    GearsSync.ApplyTextureDoner(FishKnife, "GEAR_FishKnife");
                }


                m_Helmet = AddCookpot(new Vector3(0f, 0.245f, 0f), new Vector3(0, 180, 180), 1.03f);
                m_Satchel = AddSatchel(new Vector3(0.23f, 0.23f, -0.42f), new Vector3(90, 0, -50), 1f);
                m_TechnicalBackpack = AddTechPack(new Vector3(0, -0.44f, -0.19f), new Vector3(0, 0, 0), 1f);

                AddColider(m_Helmet);

                m_HairMesh = transform.FindChild("Hair_mesh").gameObject;
                m_BeardMesh = transform.FindChild("Beard_mesh").gameObject;
                m_EyebrowsMesh = transform.FindChild("Eyebrows_mesh").gameObject;


                // Hats
                AddClothingMesh("GEAR_Balaclava"); // No UV.
                AddClothingMesh("GEAR_BaseballCap");
                AddClothingMesh("GEAR_BasicWoolHat"); // No UV.
                AddClothingMesh("GEAR_Toque");
                AddClothingMesh("GEAR_ImprovisedHat"); // No UV.
                AddClothingMesh("GEAR_CottonScarf"); // No UV.
                AddClothingMesh("GEAR_WoolWrap"); // No UV.
                AddClothingMesh("GEAR_WoolWrapCap"); // No UV.
                AddClothingMesh("GEAR_RabbitskinHat"); // No UV.

                //Torso
                AddClothingMesh("GEAR_CottonHoodie");
                AddClothingMesh("GEAR_BasicWinterCoat");
                AddClothingMesh("GEAR_HeavyWoolSweater");
                AddClothingMesh("GEAR_WoolSweater");
                AddClothingMesh("GEAR_CottonShirt");
                AddClothingMesh("GEAR_CowichanSweater");
                AddClothingMesh("GEAR_FishermanSweater");
                AddClothingMesh("GEAR_WoolSweater");
                AddClothingMesh("GEAR_SweaterChristmasA");

                //Pants
                AddClothingMesh("GEAR_CargoPants");
                AddClothingMesh("GEAR_CombatPants");
                AddClothingMesh("GEAR_DeerSkinPants");
                AddClothingMesh("GEAR_Jeans");
                AddClothingMesh("GEAR_InsulatedPants");
                AddClothingMesh("GEAR_WorkPants");
                AddClothingMesh("GEAR_LongUnderwear"); // No UV.
                AddClothingMesh("GEAR_LongUnderwearWool"); // No UV.

                //Socks
                AddClothingMesh("GEAR_CottonSocks");
                AddClothingMesh("GEAR_ClimbingSocks");
                AddClothingMesh("GEAR_WoolSocks");

                //Boots
                AddClothingMesh("GEAR_BasicShoes");


                // Gloves
                AddClothingMesh("GEAR_BasicGloves"); // No UV.


                foreach (SkinnedMeshRenderer Mesh in GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    Mesh.gameObject.layer = vp_Layer.Gear;
                }


                //ModMain.AddPlaceholderHoldingGear(this, "DarkWalker_Death", false);
                //ModMain.AddPlaceholderHoldingGear(this, "GEAR_Shovel", false);
                //ModMain.AddPlaceholderHoldingGear(this, "GEAR_ClothSheet", false);
                //ModMain.AddPlaceholderHoldingGear(this, "GEAR_FireAxe", false);
                //ModMain.AddPlaceholderHoldingGear(this, "CORPSE_Human_Frozen4", false);
            }

            public void SetIgnorePhysicsForObject(GameObject obj)
            {
                foreach (Collider col in m_PlayerColiders)
                {
                    foreach (Collider col2 in obj.GetComponentsInChildren<Collider>())
                    {
                        UnityEngine.Physics.IgnoreCollision(col2, col, true);
                    }
                }
            }

            public void AddColider(GameObject Obj)
            {
                PlayerDamageColider Col = Obj.AddComponent<PlayerDamageColider>();
                Col.m_Player = this;
                Col.m_ColiderIndex = m_PlayerColiders.Count;
                m_PlayerColiders.Add(Obj.GetComponent<Collider>());
            }

            public void CreateColiders()
            {
                CapsuleCollider[] Coliders = gameObject.GetComponentsInChildren<CapsuleCollider>();

                for (int i = 0; i < Coliders.Length; i++)
                {
                    //PlayerDamageColider Col = Coliders[i].gameObject.AddComponent<PlayerDamageColider>();
                    //Col.m_Player = this;
                    //Col.m_ColiderIndex = i;
                    AddColider(Coliders[i].gameObject);
                }
                GameAudioManager.SetMaterialSwitch("Flesh", gameObject);
                //m_PlayerColiders.AddRange(Coliders);
            }

            public void AddAudioSource()
            {
                m_AudioSource3D = gameObject.transform.FindChild("Voice3D").GetComponent<AudioSource>();
                m_AudioSource2D = gameObject.transform.FindChild("Voice2D").GetComponent<AudioSource>();
                m_AudioSourceRadio = gameObject.transform.FindChild("VoiceRadio").GetComponent<AudioSource>();
                m_AudioSourceRadioBG = gameObject.transform.FindChild("VoiceRadioBG").GetComponent<AudioSource>();
                m_BottomLip = m_Animator.GetBoneTransform(HumanBodyBones.Head).FindChild("Lip_Bottom");
            }

            public static void AddPlaceholderHoldingGear(Comps.NetworkPlayer Player, string GearName, GearHandPose HandPose = GearHandPose.None, bool Bogus = true)
            {
                AddPlaceholderHoldingGear(Player, GearName, Vector3.zero, Vector3.zero, HandPose, Bogus);
            }

            public static void AddPlaceholderHoldingGear(Comps.NetworkPlayer Player, string GearName, Vector3 LocalPosition, Vector3 LocalRotation, GearHandPose HandPose = GearHandPose.None, bool Bogus = true)
            {
                Transform RightHand = GetBone(Player.m_Animator, HumanBodyBones.RightHand);
                if (RightHand)
                {
                    GameObject Gear;
                    if (Bogus)
                    {
                        Gear = AssetManager.CreateBogusGear(GearName);
                        if (Gear)
                        {
                            Gear.transform.SetParent(RightHand);
                            Gear.transform.localPosition = LocalPosition;
                            Gear.transform.SetLocalEulerAngles(LocalRotation, RotationOrder.OrderXYZ);
                        }
                        Gear.SetActive(false);
                    } else
                    {
                        GameObject Reference = AssetManager.GetAssetFromGame<GameObject>(GearName);
                        Gear = GameObject.Instantiate(Reference);
                        if (Gear)
                        {
                            Gear.transform.SetParent(RightHand);
                            Gear.transform.localPosition = LocalPosition;
                            Gear.transform.SetLocalEulerAngles(LocalRotation, RotationOrder.OrderXYZ);
                        }
                        Gear.SetActive(false);
                    }
                    AddVisualGear(GearName, Gear, HandPose, Player);
                }
            }

            public static GameObject AddPlaceholderHoldingGearFromBundle(Comps.NetworkPlayer Player, string Prefab, string GearName, Vector3 LocalPosition, Vector3 LocalRotation, GearHandPose HandPose = GearHandPose.None)
            {
                Transform RightHand = GetBone(Player.m_Animator, HumanBodyBones.RightHand);
                if (RightHand)
                {
                    GameObject Reference = AssetManager.GetAssetFromBundle<GameObject>(Prefab);

                    if (Reference)
                    {
                        GameObject Gear = GameObject.Instantiate(Reference);
                        if (Gear)
                        {
                            Gear.transform.SetParent(RightHand);
                            Gear.transform.localPosition = LocalPosition;
                            Gear.transform.SetLocalEulerAngles(LocalRotation, RotationOrder.OrderXYZ);
                        }
                        Gear.SetActive(false);
                        AddVisualGear(GearName, Gear, HandPose, Player);
                        return Gear;
                    }
                }
                return null;
            }

            public GameObject AddCookpot(Vector3 Position, Vector3 Rotation, float Scale)
            {
                Transform Head = GetBone(m_Animator, HumanBodyBones.LeftEye);
                GameObject Gear = AssetManager.CreateBogusGear("GEAR_CookingPot");
                if (Gear)
                {
                    Gear.transform.SetParent(Head);
                    Gear.transform.localPosition = Position;
                    Gear.transform.SetLocalEulerAngles(Rotation, RotationOrder.OrderXYZ);
                    Gear.transform.localScale = new Vector3(Scale, Scale, Scale);
                }
                Gear.SetActive(false);
                return Gear;
            }

            public GameObject AddSatchel(Vector3 Position, Vector3 Rotation, float Scale)
            {
                Transform Head = GetBone(m_Animator, HumanBodyBones.LeftShoulder);
                GameObject Gear = AssetManager.CreateBogusGear("GEAR_MooseHideBag");
                if (Gear)
                {
                    Gear.transform.SetParent(Head);
                    Gear.transform.localPosition = Position;
                    Gear.transform.SetLocalEulerAngles(Rotation, RotationOrder.OrderXYZ);
                    Gear.transform.localScale = new Vector3(Scale, Scale, Scale);
                }
                Gear.SetActive(false);
                return Gear;
            }

            public GameObject AddTechPack(Vector3 Position, Vector3 Rotation, float Scale)
            {
                Transform Head = GetBone(m_Animator, HumanBodyBones.UpperChest);
                GameObject Gear = AssetManager.CreateBogusGear("GEAR_TechnicalBackpack");
                if (Gear)
                {
                    Gear.transform.SetParent(Head);
                    Gear.transform.localPosition = Position;
                    Gear.transform.SetLocalEulerAngles(Rotation, RotationOrder.OrderXYZ);
                    Gear.transform.localScale = new Vector3(Scale, Scale, Scale);
                }
                Gear.SetActive(false);
                return Gear;
            }

            public static void AddVisualGear(string GearName, GameObject Obj, GearHandPose HandPose, Comps.NetworkPlayer Player)
            {
                Comps.OtherPlayerGear Gear = Obj.AddComponent<Comps.OtherPlayerGear>();
                Gear.m_GearName = GearName;
                Gear.m_HandPose = HandPose;
                Player.m_VisualGears.Add(Gear);
            }

            public void AddClothingMesh(string GearName)
            {
                Transform T = transform.FindChild(GearName);
                if (T)
                {
                    m_ClothingMeshes.Add(T.gameObject);
                    Renderer Mesh = T.GetComponent<Renderer>();

                    GameObject GearReference = AssetManager.GetAssetFromGame<GameObject>(GearName);
                    Material ReferenceMaterial = null;
                    if(GearReference)
                    {
                        Renderer GearMesh = GearReference.GetComponent<Renderer>();
                        if (GearMesh)
                        {
                            ReferenceMaterial = GearMesh.material;
                        }
                        else
                        {
                            ReferenceMaterial = GearReference.GetComponentInChildren<Renderer>().material;
                        }
                    }

                    Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Material> NewMatsArr = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Material>(Mesh.materials.Length);
                    for (int i = 0; i < NewMatsArr.Length; i++)
                    {
                        NewMatsArr[i] = ReferenceMaterial;
                    }
                    Mesh.SetMaterialArray(NewMatsArr);
                }
            }

            public float GetVoicePeak(float PlayTime, AudioClip AudioClip)
            {
                int SeekPosition = (int)(PlayTime * AudioClip.frequency);

                if (SeekPosition >= AudioClip.samples)
                {
                    return 0;
                }
                int StartIndex = SeekPosition - 64;
                if (StartIndex < 0)
                {
                    return 0;
                }

                Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<float> floatData = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<float>(m_SampleVoiceWindow);
                AudioClip.GetData(floatData, StartIndex);

                float Peak = 0;
                for (int i = 0; i < m_SampleVoiceWindow; i++)
                {
                    float F = floatData[i];
                    Peak += Mathf.Abs(F);
                }
                float Average = (float)Peak / m_SampleVoiceWindow;
                return Average;
            }

            public void AnimateMouth()
            {
                if (m_BottomLip)
                {
                    float PeakVal = 0;
                    if (m_LastVoiceSample)
                    {
                        PeakVal = GetVoicePeak(m_SampleVoiceSeek, m_LastVoiceSample);
                        //SkyCoop.Logger.Log(ConsoleColor.Cyan, $"AnimateMouth m_SampleVoiceSeek {m_SampleVoiceSeek}/{m_LastVoiceSample.length} => PeakVal {PeakVal}");
                    }
                    float InvertedVal = 1 - (PeakVal * m_MouthLerpScaler);
                    Vector3 TargetPosition = new Vector3 (m_BottomLip.localPosition.x, Mathf.Lerp(m_MouthMinY, m_MouthMaxY, InvertedVal), m_BottomLip.localPosition.z);

                    if(m_MouthLerpSmoother != 0)
                    {
                        m_BottomLip.localPosition = Vector3.Lerp(m_BottomLip.localPosition, TargetPosition, Time.deltaTime * m_MouthLerpSmoother);
                    }
                    else
                    {
                        m_BottomLip.localPosition = TargetPosition;
                    }
                }
                if(m_LastVoiceSample)
                {
                    m_SampleVoiceSeek += Time.deltaTime;
                    if(m_SampleVoiceSeek > m_LastVoiceSample.length)
                    {
                        m_SampleVoiceSeek = 0;
                        m_LastVoiceSample = null;
                    }
                }
                if (m_LastRadioSample)
                {
                    m_SampleRadioSeek += Time.deltaTime;
                    if (m_SampleRadioSeek > m_LastRadioSample.length)
                    {
                        m_SampleRadioSeek = 0;
                        m_LastRadioSample = null;
                    }
                }
            }

            public void SetVoiceSampleForAnimation(AudioClip Clip, DataStr.PlayerHearing HearingMode)
            {
                if(HearingMode == PlayerHearing.Radio)
                {
                    m_LastRadioSample = Clip;
                    m_SampleRadioSeek = 0;
                    return;
                }
                
                m_LastVoiceSample = Clip;
                m_SampleVoiceSeek = 0;
            }

            public void UpdateAnimations()
            {
                Vector3 Speed = (gameObject.transform.position - m_LastPosition) / Time.deltaTime;
                Speed.y = 0;
                Vector3 Direction = transform.InverseTransformDirection(Speed);
                m_LastPosition = gameObject.transform.position;

                if (m_Animator && ModMain.s_AppFocus)
                {

                    float AnimatorSpeed = Speed.magnitude;

                    if (!m_VisualData.m_Crouch)
                    {
                        AnimatorSpeed = AnimatorSpeed / 4;
                    }

                    m_Animator.SetFloat("Speed", AnimatorSpeed);

                    m_Animator.SetInteger("Gear", (int)m_GearHandPose);
                    m_Animator.SetInteger("Action", (int)m_Action);

                    float PreviousDirectionX = m_Animator.GetFloat("DirectionX");
                    float PreviousDirectionY = m_Animator.GetFloat("DirectionY");
                    m_Animator.SetBool("IsMoving", Direction.magnitude > m_MinimalSpeed);
                    m_Animator.SetBool("Crouch", m_VisualData.m_Crouch);
                    m_Animator.SetBool("Vehicle", m_VisualData.m_InVehicle);
                    m_Animator.SetFloat("DirectionX", Mathf.Lerp(PreviousDirectionX, Mathf.Clamp(Direction.x, -1, 1), m_Smoother));
                    m_Animator.SetFloat("DirectionY", Mathf.Lerp(PreviousDirectionY, Mathf.Clamp(Direction.z, -1, 1), m_Smoother));
                }

                //SkyCoop.Logger.Log("Player "+m_PlayerID+" Animator Params:");
                //SkyCoop.Logger.Log("Speed "+ m_Animator.GetFloat("Speed"));
                //SkyCoop.Logger.Log("Gear " + m_Animator.GetInteger("Gear"));
                //SkyCoop.Logger.Log("Action " + m_Animator.GetInteger("Action"));
                //SkyCoop.Logger.Log("IsMoving " + m_Animator.GetBool("IsMoving"));
                //SkyCoop.Logger.Log("Crouch " + m_Animator.GetBool("Crouch"));
                //SkyCoop.Logger.Log("DirectionX " + m_Animator.GetFloat("DirectionX"));
                //SkyCoop.Logger.Log("DirectionY " + m_Animator.GetFloat("DirectionY"));
            }

            void LateUpdate()
            {
                AnimateMouth();
            }

            public Vector3 GetOffset()
            {
                if (m_VisualData.m_InVehicle)
                {
                    return m_InVehicleOffset;
                }
                
                
                return Vector3.zero;
            }

            void Update()
            {
                UpdateAnimations();

                // Cause we no more broadcast all the players position constatly to all the clients.
                // Client side need somekind of failsafe.
                // if client won't get any updates about this player in s_InActiveCooldown,
                // This player going to be deactivated from scene.
                if (m_SecondsBeforeHide > 0)
                {
                    m_SecondsBeforeHide -= Time.deltaTime;
                    if (m_SecondsBeforeHide <= 0)
                    {
                        m_SecondsBeforeHide = 0;
                        //gameObject.SetActive(false);
                    }
                }

                if(m_CameraAttention && m_CameraAttention.m_OffsetTranform)
                {
                    m_CameraAttention.m_OffsetTranform.position = new Vector3(m_CameraAttention.m_OffsetTranform.position.x, GetBone(m_Animator, HumanBodyBones.LeftEye).position.y, m_CameraAttention.m_OffsetTranform.position.z);
                }

                Vector3 TargetPosition = m_Position + GetOffset();

                // That way, we can avoid stupid situations when previous position of the objects was too far away
                // would lead to character slide on high speed. This mostly noticable when player loads from Vector3.zero.
                if (Vector3.Distance(transform.position, TargetPosition) > s_InterpolationSkipDistance)
                {
                    transform.position = Vector3.Lerp(transform.position, TargetPosition, Time.deltaTime * s_DeltaMultiplayer);
                }
                else
                {
                    transform.position = TargetPosition;
                }

                //if (m_AudioSourceRadio && m_AudioSourceRadioBG)
                //{
                //    m_AudioSourceRadioBG.gameObject.SetActive(m_LastRadioSample != null);
                //}

                transform.rotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * s_DeltaMultiplayer);
            }
        }
        public class CameraAttention : MonoBehaviour
        {
            public CameraAttention(IntPtr ptr) : base(ptr) { }
            public Transform m_OffsetTranform;

            vp_FPSCamera m_Camera;
            void Start()
            {
                m_Camera = GameManager.GetVpFPSCamera();
                GameManager.GetPlayerAnimationComponent().ShowPlayer(false);
                m_Camera.enabled = false;
            }
            
            void LateUpdate()
            {
                if (m_Camera)
                {
                    if(m_OffsetTranform == null)
                    {
                        m_Camera.transform.position = transform.position;
                        m_Camera.transform.rotation = transform.rotation;
                    }
                    else
                    {
                        m_Camera.transform.position = m_OffsetTranform.position;
                        m_Camera.transform.rotation = m_OffsetTranform.rotation;
                    }
                }
            }

            void OnDestroy()
            {
                GameManager.GetPlayerAnimationComponent().ShowPlayer(true);
                m_Camera.enabled = true;
            }
        }
        public class DeathPackComp : MonoBehaviour
        {
            public DeathPackComp(IntPtr ptr) : base(ptr) { }
            public string m_OwnerName = "";
        }

        public class ContainerDescriptorHook : MonoBehaviour
        {
            public ContainerDescriptorHook(IntPtr ptr) : base(ptr) { }
            public Container m_Container = null;
            public ContainerState m_HookState = ContainerState.Untouched;
            public bool m_EverBeenSearchedByMe = false;
            public bool m_Sent = false;

            public enum ContainerState
            {
                Untouched = 0,
                Inspected,
                Empty,
            }

            public void Start()
            {
                m_Container = GetComponent<Container>();
            }

            void Update()
            {
                if (!m_Sent)
                {
                    if (m_Container)
                    {
                        if (m_Container.m_SearchInProgress)
                        {
                            m_EverBeenSearchedByMe = true;
                        }

                        if (m_EverBeenSearchedByMe && !m_Container.m_SearchInProgress)
                        {
                            if (m_Container.m_Inspected && m_Container.IsEmpty())
                            {
                                if(m_HookState == ContainerState.Untouched)
                                {
                                    ObjectGuid OBJGUID = m_Container.GetComponent<ObjectGuid>();
                                    if (OBJGUID)
                                    {
                                        ClientSend.SendContainerState(OBJGUID.Get(), 2);
                                        m_Sent = true;
                                    }
                                }
                                else
                                {
                                    m_Sent = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        public class NetworkPlayerDummy : MonoBehaviour
        {
            public NetworkPlayerDummy(IntPtr ptr) : base(ptr) { }
            public NetworkPlayer m_Original;
            public ClothingData m_ClothingData;

            public List<GameObject> m_ClothingMeshes = new List<GameObject>();

            public GameObject m_HairMesh = null;
            public GameObject m_BeardMesh = null;
            public GameObject m_EyebrowsMesh = null;

            public bool m_IsMe = false;

            public void ClonePlayer(NetworkPlayer Player)
            {
                m_HairMesh = transform.FindChild("Hair_mesh").gameObject;
                m_BeardMesh = transform.FindChild("Beard_mesh").gameObject;
                m_EyebrowsMesh = transform.FindChild("Eyebrows_mesh").gameObject;
                if (Player != null)
                {
                    m_Original = Player;

                    if (ModMain.Client != null && m_Original.m_PlayerID == ModMain.Client.GetMyId())
                    {
                        m_ClothingData = PlayersManager.m_LocalPlayerData.m_ClothingData;
                    }
                    else
                    {
                        m_ClothingData = m_Original.m_VisualData.m_ClothingData;
                    }
                    foreach (GameObject OriginalMesh in m_Original.m_ClothingMeshes)
                    {
                        AddClothingMesh(OriginalMesh);
                    }
                    UpdateClothing();
                }
            }

            public void AddClothingMesh(GameObject OriginalModelMesh)
            {
                Transform T = transform.FindChild(OriginalModelMesh.name);
                if (T)
                {
                    m_ClothingMeshes.Add(T.gameObject);
                    Renderer Mesh = T.GetComponent<Renderer>();
                    Renderer OriginalMesh = OriginalModelMesh.GetComponent<Renderer>();

                    Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Material> NewMatsArr = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Material>(OriginalMesh.materials.Length);
                    for (int i = 0; i < NewMatsArr.Length; i++)
                    {
                        NewMatsArr[i] = OriginalMesh.materials[i];
                    }
                    Mesh.SetMaterialArray(NewMatsArr);
                }
            }

            public void UpdateClothing()
            {
                if (m_Original)
                {
                    if (m_HairMesh)
                    {
                        m_HairMesh.SetActive(m_Original.m_HairMesh.activeSelf);
                    }
                    if (m_BeardMesh)
                    {
                        m_BeardMesh.SetActive(m_Original.m_BeardMesh.activeSelf);
                    }
                    if (m_EyebrowsMesh)
                    {
                        m_EyebrowsMesh.SetActive(m_Original.m_EyebrowsMesh.activeSelf);
                    }
                }

                ClothingData Data = m_ClothingData;
                foreach (GameObject Mesh in m_ClothingMeshes)
                {
                    bool HasIt = Data.HasThis(Mesh.name);
                    Mesh.SetActive(HasIt);
                }
            }
        }
        public class DangerCircleZone : MonoBehaviour
        {
            public DangerCircleZone(IntPtr ptr) : base(ptr) { }
            public float m_Smoother = 8;
            public Vector3 m_Center = Vector3.zero;
            public float m_TargetScale = 0;


            public Vector3 GetScale()
            {
                return new Vector3(m_TargetScale, m_TargetScale, 4300);
            }

            public void SetForced()
            {
                transform.localScale = GetScale();
                transform.position = m_Center;
            }

            void Update()
            {
                transform.localScale = Vector3.Lerp(transform.localScale, GetScale(), m_Smoother * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, m_Center, m_Smoother * Time.deltaTime);
            }
        }
        public class ForcedFire : MonoBehaviour
        {
            public ForcedFire(IntPtr ptr) : base(ptr) { }

            public Fire m_Fire;

            void Update()
            {
                if(ModMain.Client != null && ModMain.Client.m_Config.m_GameMode == "Lobby")
                {
                    if (m_Fire)
                    {
                        Fire fire = m_Fire;
                        fire.m_StartedByPlayer = false;
                        if (fire.m_FireState != FireState.FullBurn)
                        {
                            fire.FireStateSet(FireState.FullBurn);
                        }
                        fire.m_HeatSource.TurnOn();
                        fire.m_FX.TriggerStage(FireState.FullBurn, true, true);
                        fire.m_FuelHeatIncrease = fire.m_HeatSource.m_MaxTempIncrease;
                        fire.m_ElapsedOnTODSeconds = 0;
                        fire.m_ElapsedOnTODSecondsUnmodified = 0;
                        fire.ForceBurnTimeInMinutes(5);
                        fire.PlayFireLoop(100f);

                        if (fire.m_Campfire != null)
                        {
                            Campfire campFire = fire.m_Campfire.GetComponent<Campfire>();
                            if (campFire.m_State != CampfireState.Lit)
                            {
                                campFire.SetState(CampfireState.Lit);
                            }
                        }
                    }
                }
            }
        }
        public class CardGameProp : MonoBehaviour
        {
            public CardGameProp(IntPtr ptr) : base(ptr) { }

            public string m_GUID = "";

            public void SetInteraction(string InteractionText, string GUID)
            {
                LocalizedString Str = new LocalizedString();
                Str.m_LocalizationID = InteractionText;
                SimpleInteraction SI = gameObject.AddComponent<SimpleInteraction>();
                SI.m_DefaultHoverText = Str;
                SI.HoverText = InteractionText;
                SI.m_CanInteract = true;
                m_GUID = GUID;
            }

            public void TryUse()
            {
                PlayersManager.s_LastTryInteractionObject = gameObject;
                ClientSend.SendTryInteract(m_GUID);
            }
        }

        public class TexasHoldEmPlayer : MonoBehaviour
        {
            public TexasHoldEmPlayer(IntPtr ptr) : base(ptr) { }

            public TexasHoldEmProp m_Game = null;

            public int m_PlayerID = -1;
            public int m_PokerIndex = 0;

            public int m_Bet = 0;
            public int m_Chips = 0;

            public TextMeshPro m_ChipsLable;
            public TextMeshPro m_BetLable;
            public TextMeshPro m_RaisAmount;

            public List<GameObject> m_Cards = new List<GameObject>();

            private bool s_StartCalled = false;

            void Start()
            {
                ManualStart();
            }

            public void ManualStart()
            {
                if (!s_StartCalled)
                {
                    s_StartCalled = true;
                }
                else
                {
                    return;
                }
                
                // 0 Card0
                // 1 Card1
                // 2 Bet
                // 3 Chips

                m_Cards.Add(transform.GetChild(0).gameObject);
                transform.GetChild(0).gameObject.SetActive(false);

                m_Cards.Add(transform.GetChild(1).gameObject);
                transform.GetChild(1).gameObject.SetActive(false);

                m_BetLable = transform.GetChild(2).GetComponent<TextMeshPro>();
                m_ChipsLable = transform.GetChild(3).GetComponent<TextMeshPro>();
            }

            void Update()
            {
                if (m_BetLable)
                {
                    m_BetLable.SetText($"{m_Bet}$");
                }
                if (m_ChipsLable)
                {
                    m_ChipsLable.SetText($"{m_Chips}$");
                }
            }

            public void SetCard(int CardID, int CardType, int CardSuit)
            {
                GameObject CardObj = m_Cards[CardID];
                if (CardType == -1)
                {
                    CardObj.SetActive(false);
                    return;
                }
                Renderer Mesh = CardObj.transform.GetChild(0).GetComponent<Renderer>();
                Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Material> NewMatsArr = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Material>(Mesh.materials.Length);
                for (int i = 0; i < NewMatsArr.Length; i++)
                {
                    NewMatsArr[i] = Mesh.materials[i];
                    if(i == 0)
                    {
                        NewMatsArr[0].mainTexture = AssetManager.GetAssetFromBundle<Texture>($"{CardType}_{CardSuit}");
                    }
                }
                Mesh.SetMaterialArray(NewMatsArr);
                CardObj.SetActive(true);
            }
        }


        public class TexasHoldEmProp : MonoBehaviour
        {
            public TexasHoldEmProp(IntPtr ptr) : base(ptr) { }

            public string m_GUID = "";

            public int m_CurrentPlayerTurn = -1;
            public int m_Dealer = 0;

            public List<TexasHoldEmPlayer> m_Players = new List<TexasHoldEmPlayer>();
            public List<GameObject> m_CommunityCards = new List<GameObject>();
            public List<GameObject> m_JoinObjects = new List<GameObject>();
            public List<GameObject> m_PlayObjects = new List<GameObject>();

            private bool s_StartCalled = false;

            void Start()
            {
                ManualStart();
            }

            public List<int> GetBets()
            {
                List<int> Bets = new List<int>();

                foreach (TexasHoldEmPlayer Player in m_Players)
                {
                    Bets.Add(Player.m_Bet);
                }

                return Bets;
            }

            public int GetMaxBet()
            {
                return GetBets().Max();
            }

            public bool CanCheck(TexasHoldEmPlayer Player)
            {
                return GetMaxBet() <= Player.m_Bet;
            }

            public bool CanRaise(TexasHoldEmPlayer Player)
            {
                if (GetMaxBet() == 0)
                {
                    return false;
                }
                if(GetMaxBet() > Player.m_Bet + Player.m_Chips)
                {
                    return false;
                }
                return true;
            }

            public bool CanCall(TexasHoldEmPlayer Player)
            {
                if(GetMaxBet() == Player.m_Bet)
                {
                    return false;
                }
                
                int callAmount = GetBets().Max() - Player.m_Bet;
                if (callAmount > Player.m_Bet)
                {
                    return false;
                }
                return true;
            }

            public void ManualStart()
            {
                if (!s_StartCalled)
                {
                    s_StartCalled = true;
                }
                else
                {
                    return;
                }
                for (int i = 0; i < 4; i++)
                {
                    GameObject PlayerObj = transform.GetChild(i).gameObject;
                    TexasHoldEmPlayer Comp = PlayerObj.AddComponent<TexasHoldEmPlayer>();
                    Comp.m_PokerIndex = i;
                    Comp.m_Game = this;
                    Comp.ManualStart();
                    m_Players.Add(Comp);
                    PlayerObj.SetActive(false);
                }
                for (int i = 0; i < 5; i++)
                {
                    GameObject Card = transform.GetChild(4).GetChild(i).gameObject;
                    Card.gameObject.SetActive(false);
                    m_CommunityCards.Add(Card);
                }
                for (int i = 0; i < 4; i++)
                {
                    GameObject JoinObj = transform.GetChild(5+i).gameObject;
                    TexasHoldEmJoin Comp = JoinObj.AddComponent<TexasHoldEmJoin>();
                    Comp.SetInteraction($"Join as player {i+1} ", m_GUID, i);
                    m_JoinObjects.Add(JoinObj);
                }
                for (int i = 0; i < 4; i++)
                {
                    GameObject PlayObj = transform.GetChild(9 + i).gameObject;
                    TexasHoldEmPlay Comp = PlayObj.AddComponent<TexasHoldEmPlay>();
                    Comp.SetInteraction($"Play", m_Players[i]);
                    m_PlayObjects.Add(PlayObj);
                }

                // Game checks coliders of child objects, so eh, have to make them parnetless.
                foreach (GameObject Obj in m_JoinObjects)
                {
                    Obj.transform.SetParent(null);
                }
                foreach (GameObject Obj in m_PlayObjects)
                {
                    Obj.transform.SetParent(null);
                }
            }

            public void SetGUID(string GUID)
            {
                m_GUID = GUID;
            }

            public void SetCard(int PokerID, int CardID, int CardType, int CardSuit)
            {
                SkyCoop.Logger.Log($"SetCard (Player {PokerID}) {CardID} {(DataStr.CardType)CardType} of {(DataStr.CardSuit)CardSuit}");
                m_Players[PokerID].SetCard(CardID, CardType, CardSuit);
            }

            public void SetCard(int CardID, int CardType, int CardSuit)
            {
                SkyCoop.Logger.Log($"SetCard (Community) {CardID} {(DataStr.CardType)CardType} of {(DataStr.CardSuit)CardSuit}");
                GameObject CardObj = m_CommunityCards[CardID];
                if (CardType == -1)
                {
                    CardObj.SetActive(false);
                    return;
                }

                
                Renderer Mesh = CardObj.transform.GetChild(0).GetComponent<Renderer>();
                Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Material> NewMatsArr = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Material>(Mesh.materials.Length);
                for (int i = 0; i < NewMatsArr.Length; i++)
                {
                    NewMatsArr[i] = Mesh.materials[i];
                    if (i == 0)
                    {
                        NewMatsArr[0].mainTexture = AssetManager.GetAssetFromBundle<Texture>($"{CardType}_{CardSuit}");
                    }
                }
                Mesh.SetMaterialArray(NewMatsArr);
                CardObj.SetActive(true);
            }

            public void SetCurrentPlayerTurn(int Turn)
            {
                m_CurrentPlayerTurn = Turn;
            }

            public void SetDealer(int Dealer)
            {
                m_Dealer = Dealer;
            }

            public void SetPlayerBet(int PlayerID, int Bet)
            {
                m_Players[PlayerID].m_Bet = Bet;
            }

            public void SetPlayerChips(int PlayerID, int Chips)
            {
                m_Players[PlayerID].m_Chips = Chips;
            }

            public void RegisterPlayer(int PlayerID, int PokerIndex)
            {
                TexasHoldEmPlayer Comp = m_Players[PokerIndex];
                Comp.m_PlayerID = PlayerID;
                Comp.gameObject.SetActive(true);
                m_JoinObjects[PokerIndex].SetActive(false);
            }

            public void OnDestroy()
            {
                foreach (GameObject JoinObj in m_JoinObjects)
                {
                    UnityEngine.Object.Destroy(JoinObj);
                }
            }

            void Update()
            {
                foreach (GameObject PlayObj in m_PlayObjects)
                {
                    TexasHoldEmPlay Play = PlayObj.GetComponent<TexasHoldEmPlay>();
                    if (Play.m_Player)
                    {
                        PlayObj.SetActive(Play.m_Player.m_PokerIndex == m_CurrentPlayerTurn);
                    }
                }
            }
        }

        public class TexasHoldEmJoin : MonoBehaviour
        {
            public TexasHoldEmJoin(IntPtr ptr) : base(ptr) { }

            public string m_GUID = "";
            public int m_PokerID = 0;

            public void SetInteraction(string InteractionText, string GUID, int PokerIndex)
            {
                LocalizedString Str = new LocalizedString();
                Str.m_LocalizationID = InteractionText;
                SimpleInteraction SI = gameObject.AddComponent<SimpleInteraction>();
                SI.m_DefaultHoverText = Str;
                SI.HoverText = InteractionText;
                SI.m_CanInteract = true;
                m_GUID = GUID;
                m_PokerID = PokerIndex;
            }

            public void TryUse()
            {
                ClientSend.SendCardGameAction(m_GUID, 0, m_PokerID);
            }
        }

        public class TexasHoldEmPlay : MonoBehaviour
        {
            public TexasHoldEmPlay(IntPtr ptr) : base(ptr) { }

            public TexasHoldEmPlayer m_Player;

            public void SetInteraction(string InteractionText, TexasHoldEmPlayer Player)
            {
                LocalizedString Str = new LocalizedString();
                Str.m_LocalizationID = InteractionText;
                SimpleInteraction SI = gameObject.AddComponent<SimpleInteraction>();
                SI.m_DefaultHoverText = Str;
                SI.HoverText = InteractionText;
                SI.m_CanInteract = true;
                m_Player = Player;
            }

            public void SendAction(int Action)
            {
                ClientSend.SendCardGameAction(m_Player.m_Game.m_GUID, Action, m_Player.m_PokerIndex);
            }

            public void SendActionAllIN()
            {
                ClientSend.SendCardGameAction(m_Player.m_Game.m_GUID, 4, m_Player.m_PokerIndex, m_Player.m_Chips);
            }

            public void SendActionRaise(int Amount)
            {
                ClientSend.SendCardGameAction(m_Player.m_Game.m_GUID, 4, m_Player.m_PokerIndex, Amount);
            }

            public void OpenPicker()
            {
                Panel_PickUnits Panel = InterfaceManager.GetPanel<Panel_PickUnits>();
                if (Panel)
                {
                    Panel.Enable(true);
                    Panel.m_GearItem = null;
                    Panel.m_numUnits = 1;
                    Panel.m_maxUnits = m_Player.m_Chips;
                    MenuHook.s_RaisBetHook = this;
                    Panel.Refresh();
                }
            }

            public void TryUse()
            {
                Panel_ActionPicker Panel = InterfaceManager.GetPanel<Panel_ActionPicker>();
                if (Panel)
                {
                    Panel.Enable(true);
                    Panel.m_ActionPickerItemDataList.Clear();
                    Action act1 = new Action(() => SendAction(1));
                    Action act2 = new Action(() => SendAction(2));
                    Action act3 = new Action(() => SendAction(3));
                    Action act4 = new Action(() => OpenPicker());

                    Panel.m_ActionPickerItemDataList.Add(new ActionPickerItemData("ico_Radial_decoy", "Fold", act1));

                    if(m_Player && m_Player.m_Game)
                    {
                        if (m_Player.m_Game.CanCheck(m_Player))
                        {
                            Panel.m_ActionPickerItemDataList.Add(new ActionPickerItemData("ico_tab_passTime1", "Check", act2));
                        }
                        if (m_Player.m_Game.CanCall(m_Player))
                        {
                            Panel.m_ActionPickerItemDataList.Add(new ActionPickerItemData("ico_SideMIssions", "Call", act3));
                        }
                        if (m_Player.m_Game.CanRaise(m_Player))
                        {
                            Panel.m_ActionPickerItemDataList.Add(new ActionPickerItemData("ico_clothing_outer", "Raise", act4));
                        }
                    }

                    Panel.m_ObjectInteractedWith = null;
                    Panel.EnableWithCurrentList();
                }
            }
        }
        public class TalkingFish : MonoBehaviour
        {
            public TalkingFish(IntPtr ptr) : base(ptr) { }

            public AudioSource m_AudioSource;
            public Transform m_MouthBottom;
            public float m_MouthMinY = 0.0004f;
            public float m_MouthMaxY = 0.0006f;

            public float m_MothScaler = 40;

            public float GetVoicePeak()
            {
                int SeekPosition = m_AudioSource.timeSamples;

                int StartIndex = SeekPosition - 64;
                if (StartIndex < 0)
                {
                    return 0;
                }

                Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<float> floatData = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<float>(64);
                m_AudioSource.clip.GetData(floatData, StartIndex);

                float Peak = 0;
                for (int i = 0; i < 64; i++)
                {
                    float F = floatData[i];
                    Peak += Mathf.Abs(F);
                }
                float Average = (float)Peak / 64;
                return Average;
            }

            public void SetupMoth()
            {
                m_MouthBottom = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0);
            }

            void LateUpdate()
            {
                if (m_AudioSource && m_MouthBottom)
                {
                    float PeakVal = 0;

                    if(m_AudioSource.isPlaying)
                    {
                        PeakVal = GetVoicePeak();
                    }
                    float InvertedVal = 1 - (PeakVal * m_MothScaler);
                    Vector3 TargetPosition = new Vector3(m_MouthBottom.localPosition.x, Mathf.Lerp(m_MouthMinY, m_MouthMaxY, InvertedVal), m_MouthBottom.localPosition.z);

                    m_MouthBottom.localPosition = TargetPosition;
                }
            }
        }

        public class PropsEditorVisuzlier : MonoBehaviour
        {
            public PropsEditorVisuzlier(IntPtr ptr) : base(ptr) { }

            public int m_IndexHandler = 0;

            public void Place()
            {
                GameManager.GetPlayerManagerComponent().StartPlaceMesh(gameObject, PlaceMeshFlags.None, Il2CppTLD.Placement.PlaceMeshRules.IgnoreCloseObjects);
            }
        }

        public class TeammateBar : MonoBehaviour
        {
            public TeammateBar(IntPtr ptr) : base(ptr) { }

            public int m_IndexHandler = 0;
            public float m_Health = 100;
            public UILabel m_NameLable = null;
            public GameObject m_BuffObj;
            public GameObject m_DebuffObj;
            public UISprite m_DebuffSprite;
            GenericStatusBarSpawner s_Bar;
            StatusBar s_StatusBar;


            void Start()
            {
                s_Bar = GetComponent<GenericStatusBarSpawner>();
                if (s_Bar && s_Bar.m_SpawnedObject)
                {
                    s_StatusBar = s_Bar.m_SpawnedObject.GetComponent<StatusBar>();
                    if (s_StatusBar)
                    {
                        if(m_BuffObj == null) // On case if Start called more than once
                        {
                            m_BuffObj = s_StatusBar.m_BuffObject;
                            s_StatusBar.m_BuffObject = null; // remove control of this object from original StatusBar script.
                        }
                        if(m_DebuffObj == null)
                        {
                            m_DebuffObj = s_StatusBar.m_DebuffObject;
                            s_StatusBar.m_DebuffObject = null;
                            m_DebuffSprite = m_DebuffObj.GetComponent<UISprite>();
                        }
                    }
                }
            }

            void Update()
            {
                if (s_Bar && s_Bar.m_SpawnedObject)
                {
                    SquadHUD.SquadMember Member = SquadHUD.GetMember(m_IndexHandler);
                    s_Bar.m_SpawnedObject.SetActive(Member != null);
                    if (m_NameLable)
                    {
                        m_NameLable.gameObject.SetActive(Member != null);
                    }
                    if (Member != null)
                    {
                        m_Health = Member.m_Health;
                        if (m_NameLable)
                        {
                            m_NameLable.text = CanvasUI.GetPlayerName(Member.m_ID);
                        }
                        if (m_DebuffObj)
                        {
                            m_DebuffObj.SetActive(Member.m_HasDebuffs || Member.m_KnockedDown);

                            if (m_DebuffSprite)
                            {
                                if (Member.m_HasDebuffs && !Member.m_KnockedDown)
                                {
                                    m_DebuffSprite.spriteName = "ico_afflictionGeneric";
                                    m_DebuffSprite.color = InterfaceManager.m_FirstAidRiskColor;
                                }
                                else if(Member.m_KnockedDown)
                                {
                                    m_DebuffSprite.spriteName = "ico_injury_BrokenBody";
                                    m_DebuffSprite.color = InterfaceManager.m_FirstAidRedColor;
                                }
                            }
                        }
                    }
                }
            }
        }

        public class GenericStatusBarSpawnerHook : MonoBehaviour
        {
            public GenericStatusBarSpawnerHook(IntPtr ptr) : base(ptr) { }

            GenericStatusBarSpawner s_Bar;
            List<TeammateBar> s_TeamBars = new List<TeammateBar>();


            void AddTeamBar()
            {
                Panel_HUD HUD = InterfaceManager.GetPanel<Panel_HUD>();


                float BarsSpacing = 60;
                float NamesSpacing = 30;

                if (HUD.m_SmallSizeGroup && HUD.m_SmallSizeGroup.gameObject.activeSelf)
                {
                    BarsSpacing = 30;
                }
                else if(HUD.m_LargeSizeGroup && HUD.m_LargeSizeGroup.gameObject.activeSelf)
                {
                    BarsSpacing = 90;
                }


                if (s_Bar.m_StatusBarType == StatusBar.StatusBarType.Condition && s_Bar.GetComponent<TeammateBar>() == null)
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        GameObject Clone = UnityEngine.Object.Instantiate<GameObject>(s_Bar.gameObject, s_Bar.gameObject.transform.parent);
                        if (Clone)
                        {
                            Clone.name = s_Bar.gameObject.name + $" (Teammate {i})";

                            TeammateBar Bar = Clone.AddComponent<TeammateBar>();
                            Bar.m_IndexHandler = i-1;

                            Clone.transform.localPosition = new Vector3(s_Bar.transform.localPosition.x, s_Bar.transform.localPosition.y + (BarsSpacing * i), s_Bar.transform.localPosition.z);

                            
                            if (HUD && HUD.m_NowhereToHide)
                            {
                                GameObject LableClone = UnityEngine.Object.Instantiate<GameObject>(HUD.m_NowhereToHide.m_WardGlyphRoot.transform.GetChild(1).gameObject, Clone.transform);
                                if (LableClone)
                                {
                                    LableClone.name = "PlayerName";
                                    UILabel Lable = LableClone.GetComponent<UILabel>();
                                    if (Lable)
                                    {
                                        Lable.text = $"Teammate {i}";
                                    }
                                    Bar.m_NameLable = Lable;
                                    UILocalize Loca = Lable.GetComponent<UILocalize>();
                                    if (Loca)
                                    {
                                        UnityEngine.Object.Destroy(Loca);
                                    }
                                    UIAnchor Anch = Lable.GetComponent<UIAnchor>();
                                    if (Loca)
                                    {
                                        UnityEngine.Object.Destroy(Anch);
                                    }
                                    LableClone.transform.localPosition = new Vector3(-15f, NamesSpacing, 0);
                                }
                            }

                            UnityEngine.Object.Destroy(Clone.transform.GetChild(0).gameObject);
                        }
                    }
                }
            }

            void Start()
            {
                s_Bar = GetComponent<GenericStatusBarSpawner>();

                if (s_Bar)
                {
                    if (s_Bar.m_StatusBarType != StatusBar.StatusBarType.Condition)
                    {
                        if (s_Bar.m_SpawnedObject)
                        {
                            s_Bar.m_SpawnedObject.SetActive(!ModMain.IsMultiplayer());
                        }
                    }
                    else
                    {
                        if(s_TeamBars.Count == 0)
                        {
                            AddTeamBar();
                        }
                    }
                }
            }
        }
    }
}
