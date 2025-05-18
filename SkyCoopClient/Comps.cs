
using UnityEngine;
using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using SkyCoopServer;
using Il2CppTLD.Interactions;
using SkyCoopClient;
using static SkyCoopServer.DataStr;
using Harmony;

namespace SkyCoop
{
    public class Comps
    {
        public static void RegisterComponents()
        {
            ClassInjector.RegisterTypeInIl2Cpp<UiButtonPressHook>();
            ClassInjector.RegisterTypeInIl2Cpp<UiButtonKeyboardPressSkip>();
            ClassInjector.RegisterTypeInIl2Cpp<UiButtonSettingHook>();
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
                string PrefabName = gameObject.name;
                m_LocalizedName = Localization.Get(PrefabName.Replace("GEAR", "GAMEPLAY"));

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
                gameObject.tag = "Flesh";

                //AkSoundEngine.SetSwitch(SWITCHES.MATERIALTAG.GROUP, SWITCHES.MATERIALTAG.SWITCH.FLESH, GameAudioManager.GetSoundEmitterFromGameObject(m_Player.gameObject));

                string ObjName = gameObject.name;
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
                else if (ObjName == "Head")
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
            public List<Collider> m_PlayerColiders = new List<Collider>();

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


            float s_DeltaMultiplayer = 20;
            float s_InterpolationSkipDistance = 15f;
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

            public void CreateColiders()
            {
                CapsuleCollider[] Coliders = gameObject.GetComponentsInChildren<CapsuleCollider>();

                for (int i = 0; i < Coliders.Length; i++)
                {
                    PlayerDamageColider Col = Coliders[i].gameObject.AddComponent<PlayerDamageColider>();
                    Col.m_Player = this;
                    Col.m_ColiderIndex = i;
                }
                GameAudioManager.SetMaterialSwitch("Flesh", gameObject);
                m_PlayerColiders.AddRange(Coliders);
            }

            public void AddAudioSource()
            {
                m_AudioSource3D = gameObject.transform.FindChild("Voice3D").GetComponent<AudioSource>();
                m_AudioSource2D = gameObject.transform.FindChild("Voice2D").GetComponent<AudioSource>();
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
                        Gear.SetActive(true);
                    }
                    AddVisualGear(GearName, Gear, HandPose, Player);
                }
            }

            public static void AddVisualGear(string GearName, GameObject Obj, GearHandPose HandPose, Comps.NetworkPlayer Player)
            {
                Comps.OtherPlayerGear Gear = Obj.AddComponent<Comps.OtherPlayerGear>();
                Gear.m_GearName = GearName;
                Gear.m_HandPose = HandPose;
                Player.m_VisualGears.Add(Gear);
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

            void Update()
            {
                // Cause we no more broadcast all the players position constatly to all the clients.
                // Client side need somekind of failsafe.
                // if client won't get any updates about this player in s_InActiveCooldown,
                // This player going to be deactivated from scene.
                if (m_SecondsBeforeHide > 0)
                {
                    m_SecondsBeforeHide -= Time.deltaTime;
                    if(m_SecondsBeforeHide <= 0)
                    {
                        m_SecondsBeforeHide = 0;
                        //gameObject.SetActive(false);
                    }
                }

                UpdateAnimations();
                
                
                // That way, we can avoid stupid situations when previous position of the objects was too far away
                // would lead to character slide on high speed. This mostly noticable when player loads from Vector3.zero.
                if (Vector3.Distance(transform.position, m_Position) > s_InterpolationSkipDistance)
                {
                    transform.position = Vector3.Lerp(transform.position, m_Position, Time.deltaTime * s_DeltaMultiplayer);
                } else
                {
                    transform.position = m_Position;
                }

                transform.rotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.deltaTime * s_DeltaMultiplayer);
            }
        }
    }
}
