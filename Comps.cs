using GameServer;
using MelonLoader;
using UnityEngine;
using static SkyCoop.DataStr;
using static SkyCoop.MyMod;
using Il2Cpp;

namespace SkyCoop
{
    public class Comps
    {
        public static void RegisterComponents()
        {
            ClassInjector.RegisterTypeInIl2Cpp<AnimalUpdates>();
            ClassInjector.RegisterTypeInIl2Cpp<DestoryArrowOnHit>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerBulletDamage>();
            ClassInjector.RegisterTypeInIl2Cpp<ClientProjectile>();
            ClassInjector.RegisterTypeInIl2Cpp<MultiplayerPlayer>();
            ClassInjector.RegisterTypeInIl2Cpp<ContainersSync>();
            ClassInjector.RegisterTypeInIl2Cpp<UiButtonPressHook>();
            ClassInjector.RegisterTypeInIl2Cpp<DestoryStoneOnStop>();
            ClassInjector.RegisterTypeInIl2Cpp<MultiplayerPlayerAnimator>();
            ClassInjector.RegisterTypeInIl2Cpp<MultiplayerPlayerClothingManager>();
            ClassInjector.RegisterTypeInIl2Cpp<DoNotSerializeThis>();
            ClassInjector.RegisterTypeInIl2Cpp<MultiplayerPlayerVoiceChatPlayer>();
            ClassInjector.RegisterTypeInIl2Cpp<DroppedGearDummy>();
            ClassInjector.RegisterTypeInIl2Cpp<IgnoreDropOverride>();
            ClassInjector.RegisterTypeInIl2Cpp<DropFakeOnLeave>();
            ClassInjector.RegisterTypeInIl2Cpp<FakeBed>();
            ClassInjector.RegisterTypeInIl2Cpp<FakeBedDummy>();
            ClassInjector.RegisterTypeInIl2Cpp<AnimalActor>();
            ClassInjector.RegisterTypeInIl2Cpp<AnimalCorpseObject>();
            ClassInjector.RegisterTypeInIl2Cpp<SpawnRegionSimple>();
            ClassInjector.RegisterTypeInIl2Cpp<LobbyHoverNickname>();
            ClassInjector.RegisterTypeInIl2Cpp<CookpotHelmet>();
            ClassInjector.RegisterTypeInIl2Cpp<UiButtonKeyboardPressSkip>();
            ClassInjector.RegisterTypeInIl2Cpp<DeathDropContainer>();
            ClassInjector.RegisterTypeInIl2Cpp<DoorLockedOnKey>();
            ClassInjector.RegisterTypeInIl2Cpp<FakeRockCache>();
            ClassInjector.RegisterTypeInIl2Cpp<LocalVariablesKit>();
            ClassInjector.RegisterTypeInIl2Cpp<ParticleSystemParasite>();
            ClassInjector.RegisterTypeInIl2Cpp<ExpeditionInteractive>();
        }
        
        
        public class DroppedGearDummy : MonoBehaviour
        {
            public DroppedGearDummy(IntPtr ptr) : base(ptr) { }

            public string m_LocalizedDisplayName;
            public int m_SearchKey = 0;
            public DataStr.ExtraDataForDroppedGear m_Extra = new DataStr.ExtraDataForDroppedGear();
        }
        public class IgnoreDropOverride : MonoBehaviour
        {
            public IgnoreDropOverride(IntPtr ptr) : base(ptr) { }
        }
        public class DropFakeOnLeave : MonoBehaviour
        {
            public DropFakeOnLeave(IntPtr ptr) : base(ptr) { }
            public Vector3 m_OldPossition = new Vector3(0, 0, 0);
            public Quaternion m_OldRotation = new Quaternion(0, 0, 0, 0);
        }
        public class FakeBed : MonoBehaviour
        {
            public FakeBed(IntPtr ptr) : base(ptr) { }
        }
        public class FakeBedDummy : MonoBehaviour
        {
            public FakeBedDummy(IntPtr ptr) : base(ptr) { }
            public GameObject m_LinkedFakeObject;
        }

        public class DestoryArrowOnHit : MonoBehaviour
        {
            public DestoryArrowOnHit(IntPtr ptr) : base(ptr) { }
        }
        public class DestoryStoneOnStop : MonoBehaviour
        {
            public DestoryStoneOnStop(IntPtr ptr) : base(ptr) { }
            public GameObject m_Obj = null;
            public Rigidbody m_RB = null;
            public GearItem m_Gear = null;
            public bool m_ShouldSendDrop = false;
            private bool m_SendDrop = false;

            void Update()
            {
                if (m_Obj != null && m_RB != null && m_Gear != null)
                {
                    if (m_RB.isKinematic == true)
                    {
                        if (m_ShouldSendDrop)
                        {
                            if (!m_SendDrop)
                            {
                                m_SendDrop = true;
                                SendDropItem(m_Gear, 0, 0, true);
                            }
                        } else
                        {
                            UnityEngine.Object.Destroy(m_Obj);
                        }
                    }
                }
            }
        }
        public class ClientProjectile : MonoBehaviour
        {
            public ClientProjectile(IntPtr ptr) : base(ptr) { }
            public int m_ClientID = 0;
        }
        public class UiButtonPressHook : MonoBehaviour
        {
            public UiButtonPressHook(IntPtr ptr) : base(ptr) { }
            public int m_CustomId = 0;
        }
        public class UiButtonKeyboardPressSkip : MonoBehaviour
        {
            public UiButtonKeyboardPressSkip(IntPtr ptr) : base(ptr) { }
            public IL2CPP.List<EventDelegate> m_Click;
        }
        public class DeathDropContainer : MonoBehaviour
        {
            public DeathDropContainer(IntPtr ptr) : base(ptr) { }
            public string m_Owner = "UNKNOWN";
        }
        public class DoorLockedOnKey : MonoBehaviour
        {
            public DoorLockedOnKey(IntPtr ptr) : base(ptr) { }
            public string m_Owner = "UNKNOWN";
            public string m_DoorKey = "";
        }
        public class DoNotSerializeThis : MonoBehaviour
        {
            public DoNotSerializeThis(IntPtr ptr) : base(ptr) { }
        }
        public class ContainersSync : MonoBehaviour
        {
            public ContainersSync(IntPtr ptr) : base(ptr) { }
            public GameObject m_Obj = null;
            public Container m_Cont = null;
            public string m_Guid = "";
            public string m_LastAnim = "";
            public bool m_Empty = true;


            void Update()
            {
                if (m_Obj != null)
                {
                    if (m_Cont == null && m_Obj.GetComponent<Container>() != null)
                    {
                        m_Cont = m_Obj.GetComponent<Container>();
                    }
                    if (m_Guid == "" && m_Obj.GetComponent<ObjectGuid>() != null)
                    {
                        m_Guid = m_Obj.GetComponent<ObjectGuid>().Get();
                    }

                    if (m_Guid != "" && m_Cont != null)
                    {
                        if (m_Cont.m_IsCorpse == true)
                        {
                            //UnityEngine.Object.Destroy(m_Obj.GetComponent<ContainersSync>());
                        }
                    }
                }
            }

            public void CallSync()
            {
                DataStr.ContainerOpenSync sync = new DataStr.ContainerOpenSync();
                sync.m_Guid = m_Guid;

                if (m_LastAnim == "close")
                {
                    sync.m_State = false;
                }
                else
                {
                    sync.m_State = true;
                }

                if (MyMod.iAmHost == true)
                {
                    using (Packet _packet = new Packet((int)ServerPackets.CONTAINEROPEN))
                    {
                        ServerSend.CONTAINEROPEN(0, sync, true);
                    }
                }
                if (MyMod.sendMyPosition == true)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.CONTAINEROPEN))
                    {
                        _packet.Write(sync);
                        MyMod.SendUDPData(_packet);
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

                    if (m_Cont.gameObject != null && GameManager.GetPlayerObject() != null)
                    {
                        float dist = Vector3.Distance(GameManager.GetPlayerObject().transform.position, m_Cont.gameObject.transform.position);
                        if (dist < 30)
                        {
                            m_Cont.PlayContainerOpenSound();
                        }
                    }
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
                    if (m_Cont.gameObject != null && GameManager.GetPlayerObject() != null)
                    {
                        float dist = Vector3.Distance(GameManager.GetPlayerObject().transform.position, m_Cont.gameObject.transform.position);
                        if (dist < 30)
                        {
                            m_Cont.PlayContainerCloseSound();
                        }
                    }
                    m_Cont.m_PendingClose = false;
                    return;
                }
            }
        }

        public class AnimalUpdates : MonoBehaviour
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
            public bool m_CanSync = false;
            public float m_Hp = 100;
            public bool m_Banned = false;
            public bool m_DampingIgnore = false;
            public int LastFoundPlayer = -1;
            public bool m_MyControlled = false;
            public bool m_PickedUp = false;
            public string m_RegionGUID = "";
            public bool m_AddedToRegion = false;
            public bool m_MarkToDestroy = false;
            public List<DataStr.AnimalArrow> m_Arrows = new List<DataStr.AnimalArrow>();
            public int LastDamager = 0;

            void Start()
            {
                nextActionTimeNR = Time.time;
                nextActionSync = Time.time;
                nextActionBloodDrop = Time.time;
                nextActionDampingOn = Time.time + dampingOn_perioud;
            }

            public void ProcessArrows()
            {
                for (int i = 0; i < m_Arrows.Count; i++)
                {
                    AddFakeArrowToAnimal(this, i);
                }
            }

            public void CallSync()
            {
                if (m_Animal != null && m_Banned == false && m_Animal.activeSelf == true && m_PickedUp == false)
                {
                    if (m_Animal.GetComponent<BaseAi>() != null)
                    {
                        BaseAi _AI = m_Animal.GetComponent<BaseAi>();

                        DataStr.AnimalCompactData Dat = GetCompactDataForAnimal(_AI, m_Arrows);
                        DataStr.AnimalAnimsSync AnimDat = GetAnimationDataFromAnimal(_AI);

                        if (Dat.m_GUID == "")
                        {
                            return;
                        }

                        int newController = GetClosestPlayerToAnimal(m_Animal, ReTakeCoolDown, ClientUser.myId, GameManager.GetPlayerTransform().position, levelid);
                        Dat.m_LastController = newController;
                        if (newController != ClientUser.myId)
                        {
                            SendAnimalForValidPlayers(Dat, AnimDat);
                            m_Banned = true;
                            m_MarkToDestroy = true;
                            RecreateAnimalToActor(m_Animal, m_Arrows);
                        }
                        else
                        {
                            SendAnimalForValidPlayers(Dat, AnimDat);
                        }
                    }
                }
            }
            void OnDestroy()
            {
                if (m_Animal)
                {
                    if (m_Animal.GetComponent<ObjectGuid>() != null)
                    {
                        if (m_RegionGUID != "")
                        {
                            GameObject RegionSpawnObj = Il2CppTLD.PDID.PdidTable.GetGameObject(m_RegionGUID);

                            if (RegionSpawnObj != null)
                            {
                                RegionSpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                            }
                        }
                    }
                }
            }
            void MaybeAddToSpawnRegion()
            {
                if (!m_AddedToRegion && m_RegionGUID != "")
                {
                    m_AddedToRegion = true;
                    GameObject RegionSpawnObj = Il2CppTLD.PDID.PdidTable.GetGameObject(m_RegionGUID);
                    if (RegionSpawnObj)
                    {
                        if (m_Animal.GetComponent<ObjectGuid>() != null && RegionSpawnObj.GetComponent<SpawnRegionSimple>() != null)
                        {
                            RegionSpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                            RegionSpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Add(m_Animal.GetComponent<ObjectGuid>().Get(), m_Animal);
                        }
                    }
                }
            }

            public void DropArrows()
            {
                for (int i = 0; i < m_Arrows.Count; i++)
                {
                    GameObject reference = GetGearItemObject("GEAR_Arrow");
                    GameObject obj = UnityEngine.Object.Instantiate<GameObject>(reference, Vector3.zero, Quaternion.identity);

                    SendDropItem(obj.GetComponent<GearItem>(), 0, 0, false, 0, m_Animal);
                }
            }

            void Update()
            {
                if (m_MarkToDestroy == true)
                {
                    UnityEngine.Object.Destroy(m_Animal);
                    return;
                }


                if (m_Animal != null && m_PickedUp == false && m_Banned == false)
                {
                    MaybeAddToSpawnRegion();
                    if (Time.time > nextActionSync)
                    {
                        nextActionSync += actionSync_perioud;
                        if (level_name != "Empty" && level_name != "MainMenu")
                        {
                            if (AnimalsController == true || m_MyControlled == true)
                            {
                                CallSync();
                            }
                        }
                    }
                    if (Time.time > nextActionTimeNR)
                    {
                        nextActionTimeNR += noresponce_perioud;
                        ReTakeCoolDown--;
                        if (ReTakeCoolDown <= 0)
                        {
                            ReTakeCoolDown = 0;
                        }
                        if (AnimalsController == false && m_MyControlled == false)
                        {
                            m_MarkToDestroy = true;
                        }
                    }
                    if (InOnline() && m_Animal.GetComponent<BaseAi>() && m_Animal.GetComponent<ObjectGuid>() && (AnimalsController == true || m_MyControlled == true))
                    {
                        BaseAi bai = m_Animal.GetComponent<BaseAi>();
                        string RightName = GetAnimalPrefabName(m_Animal.name);
                        if (bai.m_CurrentHP <= 0 || bai.m_CurrentMode == AiMode.Dead || bai.m_CurrentMode == AiMode.Stunned)
                        {
                            DropArrows();
                            DataStr.AnimalKilled Corpse = new DataStr.AnimalKilled();
                            Corpse.m_Position = m_Animal.transform.position;
                            Corpse.m_Rotation = m_Animal.transform.rotation;
                            Corpse.m_PrefabName = RightName;
                            Corpse.m_GUID = m_Animal.GetComponent<ObjectGuid>().Get();
                            Corpse.m_LevelGUID = level_guid;
                            Corpse.m_CreatedTime = MinutesFromStartServer;
                            Corpse.m_Knocked = bai.m_CurrentMode == AiMode.Stunned;
                            Corpse.m_RegionGUID = m_RegionGUID;

                            if (!Corpse.m_Knocked)
                            {
                                MelonLogger.Msg("Animal been killed SpawnRegion " + m_RegionGUID);
                            }
                            else
                            {
                                MelonLogger.Msg("Animal been knocked SpawnRegion " + m_RegionGUID);
                            }
                            m_Banned = true;

                            if (iAmHost == true)
                            {
                                m_MarkToDestroy = true;
                                ServerSend.ANIMALCORPSE(0, Corpse, true);
                                ObjectGuidManager.UnRegisterGuid(m_Animal.GetComponent<ObjectGuid>().Get());
                                Shared.OnAnimalKilled(RightName, m_Animal.transform.position, m_Animal.transform.rotation, m_Animal.GetComponent<ObjectGuid>().Get(), level_guid, m_RegionGUID, Corpse.m_Knocked);
                            }
                            else if (sendMyPosition == true)
                            {
                                using (Packet _packet = new Packet((int)ClientPackets.ANIMALKILLED))
                                {
                                    _packet.Write(Corpse);
                                    SendUDPData(_packet);
                                }
                                m_MarkToDestroy = true;
                                ObjectGuidManager.UnRegisterGuid(m_Animal.GetComponent<ObjectGuid>().Get());
                                SpawnAnimalCorpse(RightName, m_Animal.transform.position, m_Animal.transform.rotation, m_Animal.GetComponent<ObjectGuid>().Get(), m_RegionGUID); // So we won't wait responce.
                            }
                        }
                    }
                }
            }
        }

        public class AnimalActor : MonoBehaviour
        {
            public AnimalActor(IntPtr ptr) : base(ptr) { }
            public GameObject m_Animal = null;
            public int NoResponce = 5;
            public int ReTakeCoolDown = 5;

            // Timers
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
            public int AP_AiState; //10
            public bool AP_Corpse; //14
            public bool AP_Dead; //15
            public int AP_DeadSide; //16
            public int AP_DamageBodyPart; //19
            public int AP_AttackId; //23
            public bool AP_Stunned;

            public float m_Hp = 100;
            public bool m_Bleeding = false;
            public int m_ClientController = -1;
            public bool m_Banned = false;
            public bool m_MarkToDestroy = false;
            public bool m_DampingIgnore = false;
            public string m_RegionGUID = "";
            public bool m_AddedToRegion = false;

            int m_AnimParameter_TurnAngle;
            int m_AnimParameter_TurnSpeed;
            int m_AnimParameter_Speed;
            int m_AnimParameter_Wounded;
            int m_AnimParameter_Roll;
            int m_AnimParameter_Pitch;
            int m_AnimParameter_TargetHeading;
            int m_AnimParameter_TargetHeadingSmooth;
            int m_AnimParameter_TapMeter;
            int m_AnimParameter_AiState;
            int m_AnimParameter_Corpse;
            int m_AnimParameter_Dead;
            int m_AnimParameter_DamageSide;
            int m_AnimParameter_DamageBodyPart;
            int m_AnimParameter_AttackId;
            int m_AnimParameter_Stunned;

            public Dictionary<string, int> m_AnimalAnimatorHashes = new Dictionary<string, int>();
            public bool m_AnimalAnimatorHashesReady = false;
            public Animator m_Animator;
            public List<DataStr.AnimalArrow> m_Arrows = new List<DataStr.AnimalArrow>();

            void Start()
            {
                nextActionTimeNR = Time.time;
                nextActionSync = Time.time;
                nextActionBloodDrop = Time.time;
                nextActionDampingOn = Time.time + dampingOn_perioud;
            }
            public void ProcessArrows()
            {
                for (int i = 0; i < m_Arrows.Count; i++)
                {
                    MyMod.AddFakeArrowToAnimal(this, i);
                }
            }

            public void AnimInit()
            {
                m_AnimParameter_TurnAngle = Animator.StringToHash("TurnAngle");
                m_AnimParameter_TurnSpeed = Animator.StringToHash("TurnSpeed");
                m_AnimParameter_Speed = Animator.StringToHash("Speed");
                m_AnimParameter_Wounded = Animator.StringToHash("Wounded");
                m_AnimParameter_Roll = Animator.StringToHash("Roll");
                m_AnimParameter_Pitch = Animator.StringToHash("Pitch");
                m_AnimParameter_TargetHeading = Animator.StringToHash("TargetHeading");
                m_AnimParameter_TargetHeadingSmooth = Animator.StringToHash("TargetHeadingSmooth");
                m_AnimParameter_TapMeter = Animator.StringToHash("TapMeter");
                m_AnimParameter_AiState = Animator.StringToHash("AiState");
                m_AnimParameter_Corpse = Animator.StringToHash("Corpse");
                m_AnimParameter_Dead = Animator.StringToHash("Dead");
                m_AnimParameter_DamageSide = Animator.StringToHash("DamageSide");
                m_AnimParameter_DamageBodyPart = Animator.StringToHash("DamageBodyPart");
                m_AnimParameter_AttackId = Animator.StringToHash("AttackId");
                m_AnimParameter_Stunned = Animator.StringToHash("Stunned");
                m_AnimalAnimatorHashesReady = true;
            }

            void SetAnimations()
            {
                Animator AN = m_Animator;
                if (AN != null)
                {
                    if (!m_AnimalAnimatorHashesReady)
                    {
                        AnimInit();
                    }

                    AN.SetFloat(m_AnimParameter_TurnAngle, AP_TurnAngle);
                    AN.SetFloat(m_AnimParameter_TurnSpeed, AP_TurnSpeed);
                    AN.SetFloat(m_AnimParameter_Speed, AP_Speed);
                    AN.SetFloat(m_AnimParameter_Wounded, AP_Wounded);
                    AN.SetFloat(m_AnimParameter_Roll, AP_Roll);
                    AN.SetFloat(m_AnimParameter_Pitch, AP_Pitch);
                    AN.SetFloat(m_AnimParameter_TargetHeading, AP_TargetHeading);
                    AN.SetFloat(m_AnimParameter_TargetHeadingSmooth, AP_TargetHeadingSmooth);
                    AN.SetFloat(m_AnimParameter_TapMeter, AP_TapMeter);
                    AN.SetInteger(m_AnimParameter_AiState, AP_AiState);
                    AN.SetBool(m_AnimParameter_Corpse, AP_Corpse);
                    AN.SetBool(m_AnimParameter_Dead, AP_Dead);
                    AN.SetInteger(m_AnimParameter_DamageSide, AP_DeadSide);
                    AN.SetInteger(m_AnimParameter_DamageBodyPart, AP_DamageBodyPart);
                    AN.SetInteger(m_AnimParameter_AttackId, AP_AttackId);
                    AN.SetBool(m_AnimParameter_Stunned, AP_Stunned);
                }
            }

            void SetPosition()
            {
                if (m_DampingIgnore == true)
                {
                    m_Animal.transform.position = m_ToGo;
                    m_Animal.transform.rotation = m_ToRotate;
                }
                else
                {
                    m_Animal.transform.position = Vector3.Lerp(m_Animal.transform.position, m_ToGo, Time.deltaTime * DeltaAnimalsMultiplayer);
                    m_Animal.transform.rotation = Quaternion.Lerp(m_Animal.transform.rotation, m_ToRotate, Time.deltaTime * DeltaAnimalsMultiplayer);
                }
            }

            void OnDestroy()
            {
                if (m_Animal)
                {
                    if (m_Animal.GetComponent<ObjectGuid>() != null)
                    {
                        ActorsList.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                        GameAudioManager.StopAllSoundsFromGameObject(m_Animal);
                    }
                    if (m_RegionGUID != "")
                    {
                        GameObject RegionSpawnObj = Il2CppTLD.PDID.PdidTable.GetGameObject(m_RegionGUID);

                        if (RegionSpawnObj != null)
                        {
                            RegionSpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                        }
                    }
                }
            }
            void MaybeAddToSpawnRegion()
            {
                if (!m_AddedToRegion && m_RegionGUID != "")
                {
                    m_AddedToRegion = true;
                    GameObject RegionSpawnObj = Il2CppTLD.PDID.PdidTable.GetGameObject(m_RegionGUID);

                    if (m_Animal.GetComponent<ObjectGuid>() != null)
                    {
                        if (m_RegionGUID != "")
                        {
                            if (RegionSpawnObj != null)
                            {
                                RegionSpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                                RegionSpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Add(m_Animal.GetComponent<ObjectGuid>().Get(), m_Animal);
                            }
                        }
                    }
                }
            }

            public uint m_FeedingAudioID = 0U;
            public uint m_FleeAudioId = 0U;
            public uint m_HoldGroundAudioID = 0U;
            public uint m_IdleAudioId = 0U;
            public uint m_SleepingLoopAudioID = 0U;
            public uint m_StalkingAudioID = 0U;
            public uint m_StalkingLoopAudioID = 0U;
            public uint m_StuggleAudioId = 0U;
            public uint m_WanderAudioId = 0U;
            public uint m_HideAndSeekAudioId = 0U;
            public uint m_JoinPackAudioId = 0U;

            void ExitFeeding()
            {
                if (m_FeedingAudioID == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_FeedingAudioID, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
            }
            void ExitFlee()
            {
                if (m_FleeAudioId == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_FleeAudioId, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_FleeAudioId = 0U;
            }
            void ExitHoldGround()
            {
                if (m_HoldGroundAudioID == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_HoldGroundAudioID, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_HoldGroundAudioID = 0U;
            }
            void ExitIdle()
            {
                if (m_IdleAudioId == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_IdleAudioId, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_IdleAudioId = 0U;
            }
            void ExitInvestigateFood()
            {
                ExitFeeding();
            }
            void ExitSleep()
            {
                if (m_SleepingLoopAudioID == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_SleepingLoopAudioID, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_SleepingLoopAudioID = 0U;
            }
            void ExitStalking()
            {
                if (m_StalkingAudioID != 0U)
                {
                    AkSoundEngine.StopPlayingID(m_StalkingAudioID, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                    m_StalkingAudioID = 0U;
                }
                if (m_StalkingLoopAudioID == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_StalkingLoopAudioID, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_StalkingLoopAudioID = 0U;
            }
            void ExitStruggle()
            {
                if (m_StuggleAudioId == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_StuggleAudioId, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_StuggleAudioId = 0U;
            }
            void ExitWander()
            {
                if (m_WanderAudioId == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_WanderAudioId, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_WanderAudioId = 0U;
            }
            void ExitHideAndSeek()
            {
                if (m_HideAndSeekAudioId == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_HideAndSeekAudioId, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_HideAndSeekAudioId = 0U;
            }
            void ExitJoinPack()
            {
                if (m_JoinPackAudioId == 0U)
                    return;
                AkSoundEngine.StopPlayingID(m_JoinPackAudioId, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                m_JoinPackAudioId = 0U;
            }

            public AiMode m_CurrentMode = AiMode.Idle;
            public AiMode m_NextMode = AiMode.Idle;

            public string m_EnterAttackModeAudio = "";
            public string m_EnterFleeModeAudio = "";
            public string m_HoldGroundAudio = "";
            public string m_IdleAudio = "";
            public string m_SleepingAudio = "";
            public string m_EnterStalkingAudio = "";
            public string m_WanderAudio = "";
            public string m_HideAndSeekAudio = "";
            public string m_JoinPackAudio = "";
            bool m_AudioReady = false;
            void EnterAttack()
            {
                GameAudioManager.Play3DSound(m_EnterAttackModeAudio, m_Animal);
            }
            void EnterFlee()
            {
                if (m_EnterFleeModeAudio == "" || m_FleeAudioId != 0U)
                {
                    return;
                }
                m_FleeAudioId = GameAudioManager.Play3DSound(m_EnterFleeModeAudio, m_Animal);
            }
            void EnterHoldGround()
            {
                if (m_HoldGroundAudio == "")
                {
                    return;
                }

                m_HoldGroundAudioID = GameAudioManager.Play3DSound(m_HoldGroundAudio, m_Animal);
            }
            void EnterIdle()
            {
                if (m_IdleAudio == "")
                {
                    return;
                }
                if (m_IdleAudioId == 0U)
                {
                    m_IdleAudioId = GameAudioManager.Play3DSound(m_IdleAudio, m_Animal);
                }
            }
            void EnterSleep()
            {
                if (m_SleepingAudio == "")
                {
                    return;
                }
                m_SleepingLoopAudioID = GameAudioManager.Play3DSound(m_SleepingAudio, m_Animal);
            }
            void EnterStalking()
            {
                if (m_EnterStalkingAudio == "")
                {
                    return;
                }
                m_StalkingAudioID = GameAudioManager.Play3DSound(m_EnterStalkingAudio, m_Animal);
                m_StalkingLoopAudioID = 0U;
            }
            void EnterWander()
            {
                if (m_WanderAudioId != 0U || m_WanderAudio == "")
                    return;
                m_WanderAudioId = GameAudioManager.Play3DSound(m_WanderAudio, m_Animal);
            }
            void EnterHideAndSeek()
            {
                if (m_HideAndSeekAudioId == 0U && m_HideAndSeekAudio != "")
                {
                    m_HideAndSeekAudioId = GameAudioManager.Play3DSound(m_HideAndSeekAudio, m_Animal);
                }
            }
            void EnterJoinPack()
            {
                if (m_JoinPackAudioId == 0U && m_JoinPackAudio != "")
                {
                    m_JoinPackAudioId = GameAudioManager.Play3DSound(m_JoinPackAudio, m_Animal);
                }
            }

            public void SetAiMode()
            {
                if (m_CurrentMode == m_NextMode)
                {
                    return;
                }

                switch (m_CurrentMode) // Stop Audio Events from current animation
                {
                    case AiMode.Attack:
                        break;
                    case AiMode.Dead:
                        break;
                    case AiMode.Feeding:
                        ExitFeeding();
                        break;
                    case AiMode.Flee:
                        ExitFlee();
                        break;
                    case AiMode.FollowWaypoints:
                        break;
                    case AiMode.HoldGround:
                        ExitHoldGround();
                        break;
                    case AiMode.Idle:
                        ExitIdle();
                        break;
                    case AiMode.Investigate:
                        break;
                    case AiMode.InvestigateFood:
                        ExitInvestigateFood();
                        break;
                    case AiMode.InvestigateSmell:
                        break;
                    case AiMode.Sleep:
                        ExitSleep();
                        break;
                    case AiMode.Stalking:
                        ExitStalking();
                        break;
                    case AiMode.Struggle:
                        ExitStruggle();
                        break;
                    case AiMode.Wander:
                        ExitWander();
                        break;
                    case AiMode.WanderPaused:
                        break;
                    case AiMode.GoToPoint:
                        break;
                    case AiMode.InteractWithProp:
                        break;
                    case AiMode.ScriptedSequence:
                        break;
                    case AiMode.Stunned:
                        break;
                    case AiMode.ScratchingAntlers:
                        break;
                    case AiMode.PatrolPointsOfInterest:
                        break;
                    case AiMode.HideAndSeek:
                        ExitHideAndSeek();
                        break;
                    case AiMode.JoinPack:
                        ExitJoinPack();
                        break;
                    case AiMode.PassingAttack:
                        break;
                    case AiMode.Howl:
                        break;
                }
                switch (m_NextMode) // Play audio for this mode
                {
                    case AiMode.Attack:
                        EnterAttack();
                        break;
                    case AiMode.Dead:
                        ;
                        break;
                    case AiMode.Feeding:
                        break;
                    case AiMode.Flee:
                        EnterFlee();
                        break;
                    case AiMode.FollowWaypoints:
                        break;
                    case AiMode.HoldGround:
                        EnterHoldGround();
                        break;
                    case AiMode.Idle:
                        EnterIdle();
                        break;
                    case AiMode.Investigate:
                        break;
                    case AiMode.InvestigateFood:
                        break;
                    case AiMode.InvestigateSmell:
                        break;
                    case AiMode.Rooted:
                        break;
                    case AiMode.Sleep:
                        this.EnterSleep();
                        break;
                    case AiMode.Stalking:
                        this.EnterStalking();
                        break;
                    case AiMode.Struggle:
                        break;
                    case AiMode.Wander:
                        this.EnterWander();
                        break;
                    case AiMode.WanderPaused:
                        break;
                    case AiMode.GoToPoint:
                        break;
                    case AiMode.InteractWithProp:
                        break;
                    case AiMode.ScriptedSequence:
                        break;
                    case AiMode.Stunned:
                        break;
                    case AiMode.ScratchingAntlers:
                        break;
                    case AiMode.PatrolPointsOfInterest:
                        break;
                    case AiMode.HideAndSeek:
                        EnterHideAndSeek();
                        break;
                    case AiMode.JoinPack:
                        EnterJoinPack();
                        break;
                    case AiMode.PassingAttack:
                        break;
                    case AiMode.Howl:
                        break;
                }
                m_CurrentMode = m_NextMode;
            }

            void SetAudioNames()
            {
                string name = m_Animal.gameObject.name;
                if (name.Contains("Bear"))
                {
                    m_EnterAttackModeAudio = "Play_BearDetect";
                    m_EnterFleeModeAudio = "Play_BearFlee";
                    m_HoldGroundAudio = "Play_BearHoldGround";
                    m_IdleAudio = "Play_BearIdle";
                    m_SleepingAudio = "Play_BearSleeping";
                    m_EnterStalkingAudio = "Play_BearAttack";
                    m_WanderAudio = "Play_BearIdle";
                }
                else if (name.Contains("Wolf") && !name.Contains("gray"))
                {
                    m_EnterAttackModeAudio = "Play_WolfAttackEnter";
                    m_EnterFleeModeAudio = "PLAY_WolfWhine";
                    m_HoldGroundAudio = "Play_WolfGrowlLoop";
                    m_SleepingAudio = "Play_WolfSleeping";
                    m_EnterStalkingAudio = "Play_WolfWarn";
                }
                else if (name.Contains("Wolf") && name.Contains("gray"))
                {
                    m_EnterAttackModeAudio = "Play_TimberwolfAttackEnter";
                    m_EnterFleeModeAudio = "Play_TimberwolfWhine";
                    m_HoldGroundAudio = "Play_TimberwolfGrowlLoop";
                    m_IdleAudio = "Play_TimberwolfIdle";
                    m_SleepingAudio = "Play_TimberwolfSleeping";
                    m_EnterStalkingAudio = "Play_TimberwolfWarn";
                    m_HideAndSeekAudio = "Play_TimberwolfGrowlLoop";
                }
                else if (name.Contains("Rabbit"))
                {
                    m_EnterFleeModeAudio = "Play_RabbitSqueal";
                }
                else if (name.Contains("Moose"))
                {
                    m_EnterAttackModeAudio = "Play_MooseAttack";
                    m_HoldGroundAudio = "Play_MooseAngry";
                    m_EnterStalkingAudio = "Play_MooseAlerted";
                }
                m_AudioReady = true;
            }


            void Update()
            {
                if (m_MarkToDestroy == true)
                {
                    UnityEngine.Object.Destroy(m_Animal);
                    return;
                }

                if (m_Animal != null && m_Banned == false)
                {
                    MaybeAddToSpawnRegion();
                    if (AnimalsController == false || m_ClientController != ClientUser.myId)
                    {
                        SetAnimations();
                        SetPosition();
                    }

                    if (m_ClientController == ClientUser.myId)
                    {
                        m_Banned = true;
                        m_MarkToDestroy = true;
                        RecreateAnimalToSyncable(m_Animal, m_RegionGUID, m_Hp, m_Arrows);
                    }
                    if (!m_AudioReady)
                    {
                        SetAudioNames();
                    }

                    if (Time.time > nextActionTimeNR)
                    {
                        nextActionTimeNR += noresponce_perioud;
                        NoResponce--;
                        if (NoResponce <= 0)
                        {
                            //MelonLogger.Msg(ConsoleColor.Yellow, "Found animal that we not need anymore " + m_Animal.GetComponent<ObjectGuid>().Get());
                            GameAudioManager.StopAllSoundsFromGameObject(m_Animal);
                            m_MarkToDestroy = true;
                        }
                    }
                }
            }
        }

        public class AnimalCorpseObject : MonoBehaviour
        {
            public AnimalCorpseObject(IntPtr ptr) : base(ptr) { }
            public GameObject m_Animal = null;
            public string m_RegionGUID = "";
            public bool m_AddedToRegion = false;
            public void Update()
            {
                if (m_AddedToRegion == false)
                {
                    if (m_RegionGUID != "")
                    {
                        if (m_Animal.GetComponent<ObjectGuid>())
                        {
                            GameObject SpawnObj = Il2CppTLD.PDID.PdidTable.GetGameObject(m_RegionGUID);
                            if (SpawnObj)
                            {
                                if (SpawnObj.GetComponent<SpawnRegionSimple>())
                                {
                                    SpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                                    SpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Add(m_Animal.GetComponent<ObjectGuid>().Get(), m_Animal);
                                    MelonLogger.Msg(ConsoleColor.Blue, "Animal corpse added to SpawnRegion " + m_RegionGUID);
                                    m_AddedToRegion = true;
                                }
                            }
                        }
                    }
                }
            }
            void OnDestroy()
            {
                if (m_Animal.GetComponent<ObjectGuid>() != null)
                {
                    AnimalCorplesList.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                    if (m_RegionGUID != "")
                    {
                        GameObject SpawnObj = Il2CppTLD.PDID.PdidTable.GetGameObject(m_RegionGUID);
                        if (SpawnObj)
                        {
                            if (SpawnObj.GetComponent<SpawnRegionSimple>())
                            {
                                SpawnObj.GetComponent<SpawnRegionSimple>().m_Animals.Remove(m_Animal.GetComponent<ObjectGuid>().Get());
                            }
                        }
                    }
                }
            }
        }
        public class SpawnRegionSimple : MonoBehaviour
        {
            public SpawnRegionSimple(IntPtr ptr) : base(ptr) { }
            public SpawnRegion m_Region;
            public string m_GUID = "";
            public bool m_RolledToBeDisabled = false;
            public bool m_ChanceRolled = false;
            public int m_Spawned = 0;
            public bool m_Banned = false;
            public bool m_CheckedForBan = false;
            public bool m_PendingBanCheck = false;
            public Dictionary<string, GameObject> m_Animals = new Dictionary<string, GameObject>();
            public void SetBanned(bool Banned)
            {
                m_CheckedForBan = true;
                m_PendingBanCheck = false;
                m_Banned = Banned;
                if (m_Banned)
                {
                    MelonLogger.Msg(ConsoleColor.Cyan, "SpawnRegion " + m_Region.GetComponent<ObjectGuid>().Get() + " is banned");
                }
            }
            public void BanCheck()
            {
                m_PendingBanCheck = true;

                if (iAmHost)
                {
                    SetBanned(Shared.CheckSpawnRegionBanned(m_Region.GetComponent<ObjectGuid>().Get()));
                }
                if (sendMyPosition)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.SPAWNREGIONBANCHECK))
                    {
                        _packet.Write(m_Region.GetComponent<ObjectGuid>().Get());
                        SendUDPData(_packet);
                    }
                }
            }
            public void UpdateFromManager()
            {
                if (m_ChanceRolled == false)
                {
                    float newProcent = m_Region.m_ChanceActive;
                    if (m_Region.m_ChanceActive > 85)
                    {
                        newProcent = 85;
                    }

                    //float percent = m_Region.m_ChanceActive * GameManager.GetExperienceModeManagerComponent().GetSpawnRegionChanceActiveScale();
                    int seed = GameManager.GetRandomSeed((int)m_Region.m_Center.x + (int)m_Region.m_Center.y + (int)m_Region.m_Center.z);
                    System.Random RNG = new System.Random(seed);
                    bool Active = RollChanceSeeded(newProcent, RNG);
                    if (!Active)
                    {
                        m_RolledToBeDisabled = true;
                    }
                    m_ChanceRolled = true;
                    //MelonLogger.Msg("[SpawnRegion] Procent "+ m_Region.m_ChanceActive);
                    //MelonLogger.Msg("[SpawnRegion] Scaler " + GameManager.GetExperienceModeManagerComponent().GetSpawnRegionChanceActiveScale());
                    //MelonLogger.Msg("[SpawnRegion] Roll " + percent);
                }
                if (!m_RolledToBeDisabled && !m_Region.SpawningSupppressedByExperienceMode())
                {
                    int WhoClose = AnyOneClose(m_Region.m_Radius + MinimalDistanceForSpawn, m_Region.m_Center);
                    if (WhoClose != -1 && SpawnInBlizzard(m_Region) == true)
                    {
                        if (m_Region != null && AnimalsController == true)
                        {
                            if (m_Spawned < CalculateTargetPopulation(m_Region)) // If animals less than should be
                            {
                                //MelonLogger.Msg("Region "+ m_Region.GetComponent<ObjectGuid>().Get()+" going to spawn, for "+ WhoClose);

                                if (m_CheckedForBan)
                                {
                                    if (!m_Banned)
                                    {
                                        SimulateSpawnFromRegionSpawn(m_Region.GetComponent<ObjectGuid>().Get(), m_Region); // Spawn new animals for this region
                                    }
                                }
                                else if (!m_PendingBanCheck)
                                {
                                    BanCheck();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (m_Region != null)
                        {
                            if (m_Animals.Count > 0)
                            {
                                List<GameObject> ToDelete = new List<GameObject>();
                                foreach (var item in m_Animals)
                                {
                                    GameObject animal = item.Value;
                                    if (animal != null && animal.GetComponent<BaseAi>() != null)
                                    {
                                        BaseAi AI = animal.GetComponent<BaseAi>();
                                        // If animal is valid to unload, unloading it.
                                        if (AI.GetAiMode() != AiMode.Flee && AI.GetAiMode() != AiMode.Dead && AI.GetAiMode() != AiMode.Struggle && (AI.m_CurrentTarget == null || !AI.m_CurrentTarget.IsPlayer()))
                                        {
                                            ToDelete.Add(animal);
                                        }
                                    }
                                }
                                for (int i = 0; i < ToDelete.Count; i++)
                                {
                                    if (ToDelete[i].GetComponent<ObjectGuid>())
                                    {
                                        if (iAmHost == true)
                                        {
                                            ServerSend.ANIMALDELETE(0, ToDelete[i].GetComponent<ObjectGuid>().Get());
                                        }
                                        else if (sendMyPosition == true)
                                        {
                                            using (Packet _packet = new Packet((int)ClientPackets.ANIMALDELETE))
                                            {
                                                _packet.Write(ToDelete[i].GetComponent<ObjectGuid>().Get());
                                                SendUDPData(_packet);
                                            }
                                        }
                                    }
                                    // MelonLogger.Msg("Animal deleted from region " + m_Region.GetComponent<ObjectGuid>().Get()+" Anyone close "+WhoClose);
                                    UnityEngine.Object.Destroy(ToDelete[i]);
                                    m_Spawned--;
                                }
                            }
                        }
                    }
                }
            }
        }

        public class LobbyHoverNickname : MonoBehaviour
        {
            public LobbyHoverNickname(IntPtr ptr) : base(ptr) { }
            public UnityEngine.UI.Button m_Btn;
            public string m_Name = "???";
            public GameObject HoverObj = null;
            public UnityEngine.UI.Text LableObj = null;
            void Update()
            {
                if (m_Btn)
                {
                    if (m_Btn.currentSelectionState == UnityEngine.UI.Selectable.SelectionState.Highlighted)
                    {
                        if (LobbyUI != null)
                        {
                            if (HoverObj == null)
                            {
                                HoverObj = LobbyUI.transform.GetChild(3).gameObject;
                                LableObj = HoverObj.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>();
                            }
                            HoverObj.SetActive(true);
                            HoverObj.transform.position = new Vector3(m_Btn.gameObject.transform.position.x, HoverObj.transform.position.y, HoverObj.gameObject.transform.position.z);
                            LableObj.text = m_Name;
                        }
                    }
                }
            }
        }

        public class CookpotHelmet : MonoBehaviour
        {
            public CookpotHelmet(IntPtr ptr) : base(ptr) { }
            public GearItem m_GearItem;
            public ClothingItem m_ClothingItem;

            void Update()
            {
                if (m_GearItem && m_ClothingItem)
                {
                    if (InterfaceManager.m_Panel_Clothing != null && InterfaceManager.m_Panel_Clothing.isActiveAndEnabled)
                    {
                        m_GearItem.m_ClothingItem = null;
                    }
                    else
                    {
                        m_GearItem.m_ClothingItem = m_ClothingItem;
                    }
                }
            }
        }

        public class MultiplayerPlayer : MonoBehaviour
        {
            public MultiplayerPlayer(IntPtr ptr) : base(ptr) { }

            public int m_ID = 0;
            public List<GameObject> m_DamageColiders = new List<GameObject>();

            //Equipment
            public string m_HoldingItem = "";
            public string m_HoldingFood = "";
            public bool m_LightSourceOn = false;
            public bool m_HasAxe = false;
            public bool m_HasRifle = false;
            public bool m_HasRevolver = false;
            public bool m_HasMedkit = false;
            public int m_Arrows = 0;
            public int m_Flares = 0;
            public int m_BlueFlares = 0;
            public GameObject m_Player = null;
            public string m_BreakingSound = "";
            public uint m_BreakingSoundReference = 0U;
            public int m_BloodLosts = 0;
            public bool m_NeedAntiseptic = false;
            public uint m_HeavyBreathSoundReference = 0U;
            public int m_Character = -1;
            public GameObject m_TorchIgniter = null;

            //Shortcuts for optimized  
            public GameObject hand_r = null;
            public GameObject hand_l = null;
            public GameObject rifle = null;
            public GameObject revolver = null;
            public GameObject quiver = null;
            public GameObject axe = null;
            public GameObject medkit = null;
            public GameObject flares = null;
            public GameObject body = null;
            public GameObject clothing = null;
            public GameObject hip = null;
            public GameObject root = null;
            public GameObject extras = null;
            public GameObject MakenzyHead = null;
            public GameObject AstridHead = null;
            public GameObject bookOpen = null;
            public GameObject bookClosed = null;
            public GameObject meleeDummy = null;

            //Timers
            public float nextActionBloodDrop = 0.0f;
            public float blooddrop_period = 1.3f;

            //Actions
            public bool m_IsBeingInteractedWith = false;
            public float m_InteractTimer = 0.0f;
            public string m_ActionType = "";

            void Start()
            {
                nextActionBloodDrop = Time.time;
            }

            public void UpdateVisualItems()
            {
                if (m_HasRifle == true && m_HoldingItem != "GEAR_Rifle")
                {
                    rifle.SetActive(true);
                }
                else
                {
                    rifle.SetActive(false);
                }
                if (m_HasRevolver == true && m_HoldingItem != "GEAR_Revolver")
                {
                    revolver.SetActive(true);
                }
                else
                {
                    revolver.SetActive(false);
                }

                extras.transform.GetChild(2).gameObject.SetActive(m_HasRevolver);
                axe.SetActive(m_HasAxe);
                medkit.SetActive(m_HasMedkit);

                if (m_Arrows > 0)
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

                if (m_Flares == 0 && m_BlueFlares == 0)
                {
                    flares.SetActive(false);
                }
                else
                {
                    flares.SetActive(true);
                    GameObject redFlares = flares.transform.GetChild(1).gameObject;
                    GameObject blueFlares = flares.transform.GetChild(2).gameObject;

                    int HaveR = m_Flares;
                    int HaveB = m_BlueFlares;

                    //If we hold flare remove 1 from how many do we have.
                    if (m_HoldingItem == "GEAR_FlareA")
                    {
                        HaveR = HaveR - 1;
                    }
                    if (m_HoldingItem == "GEAR_BlueFlare")
                    {
                        HaveB = HaveB - 1;
                    }

                    if (HaveR == 0)
                    {
                        redFlares.SetActive(false);
                    }
                    else
                    {
                        redFlares.SetActive(true);
                    }
                    if (HaveB == 0)
                    {
                        blueFlares.SetActive(false);
                    }
                    else
                    {
                        blueFlares.SetActive(true);
                    }

                    redFlares.transform.GetChild(0).gameObject.SetActive(false);
                    redFlares.transform.GetChild(1).gameObject.SetActive(false);
                    blueFlares.transform.GetChild(0).gameObject.SetActive(false);
                    blueFlares.transform.GetChild(1).gameObject.SetActive(false);

                    if (HaveR > 0 && HaveB > 0)
                    {
                        redFlares.transform.GetChild(0).gameObject.SetActive(true);
                        blueFlares.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    else
                    {
                        if (HaveR > 0)
                        {
                            if (HaveR == 1)
                            {
                                redFlares.transform.GetChild(0).gameObject.SetActive(true);
                            }
                            else if (HaveR > 1)
                            {
                                redFlares.transform.GetChild(0).gameObject.SetActive(true);
                                redFlares.transform.GetChild(1).gameObject.SetActive(true);
                            }
                        }
                        if (HaveB > 0)
                        {
                            if (HaveB == 1)
                            {
                                blueFlares.transform.GetChild(0).gameObject.SetActive(true);
                            }
                            else if (HaveB > 1)
                            {
                                blueFlares.transform.GetChild(0).gameObject.SetActive(true);
                                blueFlares.transform.GetChild(1).gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }

            public void SetupMapMarker()
            {
                int Markers = hand_l.transform.GetChild(2).GetChild(0).childCount;
                Transform MakersRoot = hand_l.transform.GetChild(2).GetChild(0);
                for (int i = 0; i < Markers; i++)
                {
                    MakersRoot.GetChild(i).gameObject.SetActive(false);
                }
                GameRegion Reg = RegionManager.GetCurrentRegion();
                if (Reg == GameRegion.LakeRegion)
                {
                    MakersRoot.GetChild(0).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.RuralRegion)
                {
                    MakersRoot.GetChild(1).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.RiverValleyRegion)
                {
                    MakersRoot.GetChild(3).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.CoastalRegion)
                {
                    MakersRoot.GetChild(4).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.WhalingStationRegion)
                {
                    MakersRoot.GetChild(6).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.TracksRegion)
                {
                    MakersRoot.GetChild(7).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.MarshRegion)
                {
                    MakersRoot.GetChild(8).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.MountainTownRegion)
                {
                    MakersRoot.GetChild(9).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.MarshRegion)
                {
                    MakersRoot.GetChild(10).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.CrashMountainRegion)
                {
                    MakersRoot.GetChild(11).gameObject.SetActive(true);
                }
                if (Reg == GameRegion.AshCanyonRegion)
                {
                    MakersRoot.GetChild(12).gameObject.SetActive(true);
                }
            }

            public void UpdateHeldItem()
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
                //NoiseMaker
                hand_r.transform.GetChild(10).gameObject.SetActive(false);
                //Melee Dummy
                //hand_r.transform.GetChild(11).gameObject.SetActive(false);
                //Map
                hand_l.transform.GetChild(2).gameObject.SetActive(false);
                //Bow
                hand_l.transform.GetChild(3).gameObject.SetActive(false);
                //Bottle
                hand_l.transform.GetChild(4).gameObject.SetActive(false);
                //Food can
                hand_l.transform.GetChild(5).gameObject.SetActive(false);

                string m_AnimState = MyMod.playersData[m_ID].m_AnimState;

                if (m_HoldingFood == "" && m_AnimState != "Igniting")
                {
                    if (m_HoldingItem.StartsWith("GEAR_Flare") != m_HoldingItem.Contains("Gun")) // By some reason, it called GEAR_FlareA, i not know exist FlareB but better check it.
                    {
                        hand_r.transform.GetChild(0).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_WoodMatches" || m_HoldingItem == "GEAR_PackMatches")
                    {
                        hand_r.transform.GetChild(1).gameObject.SetActive(true); hand_l.transform.GetChild(0).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_BlueFlare")
                    {
                        hand_r.transform.GetChild(2).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_Rifle")
                    {
                        hand_r.transform.GetChild(3).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_Revolver")
                    {
                        hand_r.transform.GetChild(4).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem.StartsWith("GEAR_SprayPaintCan")) // By some reason, it called GEAR_SprayPaintCanGlyphA, i not know exist GEAR_SprayPaintCanGlyphB but better check it.
                    {
                        hand_r.transform.GetChild(5).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_Stone")
                    {
                        hand_r.transform.GetChild(6).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem.StartsWith("GEAR_KeroseneLamp"))
                    {
                        hand_r.transform.GetChild(7).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_Torch")
                    {
                        hand_r.transform.GetChild(8).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_Bow")
                    {
                        hand_l.transform.GetChild(3).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "Map")
                    {
                        hand_l.transform.GetChild(2).gameObject.SetActive(true);
                        //SetupMapMarker();
                    }
                    if (m_HoldingItem == "GEAR_FlareGun")
                    {
                        hand_r.transform.GetChild(9).gameObject.SetActive(true);
                    }
                    if (m_HoldingItem == "GEAR_NoiseMaker")
                    {
                        hand_r.transform.GetChild(10).gameObject.SetActive(true);
                    }
                }
                else
                {

                    if (m_HoldingFood != "")
                    {
                        if (m_HoldingFood == "Water")
                        {
                            hand_l.transform.GetChild(4).gameObject.SetActive(true);
                        }
                        else
                        {
                            hand_l.transform.GetChild(5).gameObject.SetActive(true);
                        }
                    }
                    if (m_AnimState == "Igniting")
                    {
                        hand_r.transform.GetChild(1).gameObject.SetActive(true); hand_l.transform.GetChild(0).gameObject.SetActive(true);
                    }
                }

                //Additionals
                if (m_LightSourceOn == true)
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
                hand_r.transform.GetChild(0).GetChild(3).gameObject.SetActive(m_LightSourceOn); //Flare Red Light
                hand_r.transform.GetChild(1).GetChild(4).gameObject.SetActive(m_LightSourceOn); //Matche Light
                hand_r.transform.GetChild(2).GetChild(3).gameObject.SetActive(m_LightSourceOn); //Flare Blue Light
                hand_r.transform.GetChild(7).GetChild(3).gameObject.SetActive(m_LightSourceOn); //Lamp Light
                hand_r.transform.GetChild(8).GetChild(0).gameObject.SetActive(m_LightSourceOn); //Tourch Light
            }

            void UpdateOtherAffictions()
            {
                DataStr.MultiPlayerClientData pD = MyMod.playersData[m_ID];
                m_NeedAntiseptic = pD.m_NeedAntiseptic;
            }

            void BloodLostUpdate()
            {
                DataStr.MultiPlayerClientData pD = MyMod.playersData[m_ID];
                m_BloodLosts = pD.m_BloodLosts;

                if (m_BloodLosts <= 1)
                {
                    blooddrop_period = 1.3f;
                }
                else if (m_BloodLosts == 2)
                {
                    blooddrop_period = 1f;
                }
                else if (m_BloodLosts == 3)
                {
                    blooddrop_period = 0.7f;
                }
                else if (m_BloodLosts >= 4)
                {
                    blooddrop_period = 0.5f;
                }

                if (Time.time > nextActionBloodDrop)
                {
                    nextActionBloodDrop += blooddrop_period;
                    int m_LevelId = pD.m_Levelid;
                    string m_LevelGUID = pD.m_LevelGuid;
                    if (m_BloodLosts > 0 && MyMod.levelid == m_LevelId && MyMod.level_guid == m_LevelGUID)
                    {
                        MyMod.PlayMultiplayer3dAduio("PLAY_BLOODDROPS_3D", m_ID);
                        Vector3 pos = root.transform.position;
                        ++pos.y;
                        Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                        insideUnitCircle.Normalize();
                        Vector2 vector2 = insideUnitCircle * UnityEngine.Random.Range(0.0f, 0.75f);
                        pos.x += vector2.x;
                        pos.z += vector2.y;
                        pos -= root.transform.forward * 0.5f;
                        RaycastHit hitInfo;
                        if (!Physics.Raycast(pos, Vector3.down, out hitInfo, float.PositiveInfinity, Utils.m_PhysicalCollisionLayerMask) || (UnityEngine.Object)hitInfo.collider == (UnityEngine.Object)null)
                            return;
                        Vector3 scale = new Vector3(0.05f, 2f, 0.05f) * UnityEngine.Random.Range(0.5f, 2f);
                        int uvRectangleIndex = 7;
                        if (Utils.RollChance(50f))
                            uvRectangleIndex = 6;
                        DecalProjectorInstance decalCreated = GameManager.GetDynamicDecalsManager().CreateDecal(hitInfo.point, m_Player.transform.rotation.eulerAngles.y, hitInfo.normal, uvRectangleIndex, scale, DecalProjectorType.PlayerBlood, GameManager.GetWeatherComponent().IsIndoorEnvironment());
                    }
                }
            }

            void DoneAction()
            {
                MyMod.LongActionFinished(this, m_ActionType);
                MyMod.LongActionCanceled(this);
            }

            void UpdateHead()
            {
                DataStr.MultiPlayerClientData pD = MyMod.playersData[m_ID];

                if (m_Character != pD.m_Character)
                {
                    MakenzyHead.SetActive(false);
                    AstridHead.SetActive(false);
                    if (pD.m_Character == 0)
                    {
                        MakenzyHead.SetActive(true);
                    }
                    if (pD.m_Character == 1)
                    {
                        AstridHead.SetActive(true);
                    }
                }
            }
            void Update()
            {
                if (m_Player != null)
                {
                    // If hand_r is null, this means all other objects shortcuts null too need write them down.
                    if (hand_r == null)
                    {
                        int clothingChild = 0;
                        int bodyChild = 1;
                        int extrasChild = 2;
                        int hipsChild = 3;
                        int rootChild = 8;
                        clothing = m_Player.transform.GetChild(clothingChild).gameObject;
                        body = m_Player.transform.GetChild(bodyChild).gameObject;
                        extras = m_Player.transform.GetChild(extrasChild).gameObject;
                        hip = m_Player.transform.GetChild(hipsChild).gameObject;
                        root = hip.transform.GetChild(rootChild).gameObject;
                        hand_r = root.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;
                        hand_l = root.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
                        rifle = root.transform.GetChild(0).GetChild(1).gameObject;
                        revolver = root.transform.GetChild(2).GetChild(1).gameObject;
                        quiver = root.transform.GetChild(0).GetChild(2).gameObject;
                        axe = root.transform.GetChild(0).GetChild(3).gameObject;
                        medkit = root.transform.GetChild(0).GetChild(4).gameObject;
                        flares = root.transform.GetChild(0).GetChild(5).gameObject;

                        bookOpen = hand_l.transform.GetChild(6).gameObject;
                        bookClosed = hand_l.transform.GetChild(7).gameObject;
                        meleeDummy = hand_r.transform.GetChild(11).gameObject;

                        MakenzyHead = body.transform.GetChild(1).GetChild(0).gameObject;
                        AstridHead = body.transform.GetChild(1).GetChild(1).gameObject;

                        //GameObject reference = GetGearItemObject("GEAR_PryBar");

                        //if (reference == null)
                        //{
                        //    return;
                        //}
                        //UnityEngine.Object.Instantiate<GameObject>(reference, meleeDummy.transform.position, meleeDummy.transform.rotation, meleeDummy.transform);
                        //meleeDummy.transform.localScale = new Vector3(0.007f, 0.007f, 0.007f);
                    }

                    DataStr.MultiPlayerClientData pD = MyMod.playersData[m_ID];

                    BloodLostUpdate();
                    UpdateOtherAffictions();
                    //UpdateHead();

                    if (m_IsBeingInteractedWith == true)
                    {
                        m_InteractTimer -= Time.deltaTime;

                        if (m_InteractTimer <= 0.0)
                        {
                            DoneAction();
                        }
                    }

                    DataStr.PlayerEquipmentData Edata = pD.m_PlayerEquipmentData;
                    if ((Edata.m_HoldingItem != m_HoldingItem) || (Edata.m_LightSourceOn != m_LightSourceOn))
                    {
                        m_HoldingItem = Edata.m_HoldingItem;
                        m_LightSourceOn = Edata.m_LightSourceOn;
                        UpdateHeldItem();
                        UpdateVisualItems();
                    }
                    if ((Edata.m_HasAxe != m_HasAxe) || (Edata.m_HasMedkit != m_HasMedkit) || (Edata.m_HasRifle != m_HasRifle) || (Edata.m_HasRevolver != m_HasRevolver) || (Edata.m_Arrows != m_Arrows) || (Edata.m_Flares != m_Flares) || (Edata.m_BlueFlares != m_BlueFlares))
                    {
                        m_HasMedkit = Edata.m_HasMedkit;
                        m_HasRifle = Edata.m_HasRifle;
                        m_HasRevolver = Edata.m_HasRevolver;
                        m_Arrows = Edata.m_Arrows;
                        m_Flares = Edata.m_Flares;
                        m_BlueFlares = Edata.m_BlueFlares;
                        UpdateVisualItems();
                    }
                    Vector3 m_XYZ = pD.m_Position;
                    Quaternion m_XYZW = pD.m_Rotation;
                    if (pD.m_Female == false)
                    {
                        m_XYZ = pD.m_Position;
                        m_Player.transform.localScale = new Vector3(1, 1, 1);
                    }
                    else
                    {
                        m_XYZ = new Vector3(pD.m_Position.x, pD.m_Position.y - 0.194f, pD.m_Position.z);
                        m_Player.transform.localScale = new Vector3(0.867f, 1, 0.867f);
                    }
                    if (m_HoldingItem == "Book")
                    {
                        if (pD.m_AnimState == "Idle")
                        {
                            bookOpen.SetActive(true);
                            bookClosed.SetActive(false);
                        }
                        else
                        {
                            bookOpen.SetActive(false);
                            bookClosed.SetActive(true);
                        }
                    }
                    else
                    {
                        bookOpen.SetActive(false);
                        bookClosed.SetActive(false);
                    }

                    Vector3 togo = Vector3.Lerp(m_Player.transform.position, m_XYZ, Time.deltaTime * 20);
                    Quaternion toRot = Quaternion.Lerp(m_Player.transform.rotation, m_XYZW, Time.deltaTime * 20);
                    m_Player.transform.position = togo;
                    m_Player.transform.rotation = toRot;

                    string BreakingAudioData = pD.m_BrakingSounds;

                    if (m_BreakingSound != BreakingAudioData)
                    {
                        m_BreakingSound = BreakingAudioData;

                        if (m_BreakingSoundReference != 0U)
                        {
                            AkSoundEngine.StopPlayingID(m_BreakingSoundReference, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                            m_BreakingSoundReference = 0U;
                        }

                        if (m_BreakingSound != "")
                        {
                            m_BreakingSoundReference = GameAudioManager.PlaySound(BreakingAudioData, m_Player);
                        }
                    }

                    bool m_HeavyBreath = pD.m_HeavyBreath;

                    if (m_HeavyBreath == true)
                    {
                        if (m_HeavyBreathSoundReference == 0U)
                        {
                            m_HeavyBreathSoundReference = GameAudioManager.PlaySound("PLAY_BREATH_3D", m_Player);
                        }
                    }
                    else
                    {
                        if (m_HeavyBreathSoundReference != 0U)
                        {
                            AkSoundEngine.StopPlayingID(m_HeavyBreathSoundReference, GameManager.GetGameAudioManagerComponent().m_StopAudioFadeOutMicroseconds);
                            m_HeavyBreathSoundReference = 0U;
                        }
                    }
                }
            }
        }
        public class MultiplayerPlayerClothingManager : MonoBehaviour
        {
            public MultiplayerPlayerClothingManager(IntPtr ptr) : base(ptr) { }

            public string m_Hat = "";
            public string m_Top = "";
            public string m_Bottom = "";
            public string m_Boots = "";
            public string m_Scarf = "";
            public string m_Balaclava = "";
            public int m_DebugB = 0;
            public int m_DebugT = 0;
            public bool m_Debug = false;

            public GameObject m_Player = null;
            public GameObject clothing = null;
            public GameObject arms_middle = null;
            public GameObject arms_short = null;
            public GameObject arms_tiny = null;
            public GameObject arms_long = null;

            void Update()
            {
                if (m_Player != null)
                {
                    bool firstUpdate = false;
                    if (clothing == null)
                    {
                        int clothingChild = 0;
                        clothing = m_Player.transform.GetChild(clothingChild).gameObject;

                        arms_middle = m_Player.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
                        arms_short = m_Player.transform.GetChild(1).GetChild(0).GetChild(1).gameObject;
                        arms_tiny = m_Player.transform.GetChild(1).GetChild(0).GetChild(2).gameObject;
                        arms_long = m_Player.transform.GetChild(1).GetChild(0).GetChild(3).gameObject;
                        firstUpdate = true;
                    }
                    int m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                    DataStr.PlayerClothingData Cdata = MyMod.playersData[m_ID].m_PlayerClothingData;
                    if ((Cdata.m_Hat != m_Hat) || (Cdata.m_Top != m_Top) || (Cdata.m_Bottom != m_Bottom) || (Cdata.m_Boots != m_Boots) || (Cdata.m_Scarf != m_Scarf) || (Cdata.m_Balaclava != m_Balaclava) || firstUpdate == true)
                    {
                        m_Hat = Cdata.m_Hat;
                        m_Top = Cdata.m_Top;
                        m_Bottom = Cdata.m_Bottom;
                        m_Boots = Cdata.m_Boots;
                        m_Scarf = Cdata.m_Scarf;
                        m_Balaclava = Cdata.m_Balaclava;

                        //MelonLogger.Msg("[Clothing] Updating model of client " + m_ID);
                        //MelonLogger.Msg("[Clothing] Client Model " + m_ID + " Hat " + m_Hat);
                        //MelonLogger.Msg("[Clothing] Client Model " + m_ID + " Torso " + m_Top);
                        //MelonLogger.Msg("[Clothing] Client Model " + m_ID + " Legs " + m_Bottom);
                        //MelonLogger.Msg("[Clothing] Client Model " + m_ID + " Feets " + m_Boots);
                        UpdateClothing();
                    }
                }
            }
            public void UpdateClothing()
            {
                if (clothing != null)
                {
                    int m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                    int hatsCount = clothing.transform.GetChild(0).childCount;
                    int topsCount = clothing.transform.GetChild(1).childCount;
                    int pantsCount = clothing.transform.GetChild(2).childCount;
                    int bootsCount = clothing.transform.GetChild(3).childCount;
                    int Hat = -1;
                    int Top = 0;
                    int Pants = 0;
                    int Boots = 0;
                    int Scarf = 0;
                    string ArmsType = "";
                    string HairVariant = "Full";
                    bool nakedBodyAtBottom = false;
                    int HeadVariant = MyMod.playersData[m_ID].m_Character;

                    GameObject HairShort = null;
                    GameObject HairLong = null;

                    if (m_Balaclava != "")
                    {
                        if (HeadVariant == 0)
                        {
                            HeadVariant = 2;
                        }
                        else if (HeadVariant == 1)
                        {
                            HeadVariant = 3;
                        }
                    }

                    for (int i = 0; i < m_Player.transform.GetChild(1).GetChild(1).childCount; i++)
                    {
                        m_Player.transform.GetChild(1).GetChild(1).GetChild(i).gameObject.SetActive(false);
                    }
                    m_Player.transform.GetChild(1).GetChild(1).GetChild(HeadVariant).gameObject.SetActive(true);

                    if (HeadVariant == 0)
                    {
                        HairLong = m_Player.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(2).gameObject;
                        HairShort = m_Player.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(3).gameObject;
                    }

                    for (int i = 0; i < hatsCount; i++)
                    {
                        clothing.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(false);
                    }
                    for (int i = 0; i < topsCount; i++)
                    {
                        clothing.transform.GetChild(1).transform.GetChild(i).gameObject.SetActive(false);
                    }
                    for (int i = 0; i < pantsCount; i++)
                    {
                        clothing.transform.GetChild(2).transform.GetChild(i).gameObject.SetActive(false);
                    }
                    for (int i = 0; i < bootsCount; i++)
                    {
                        clothing.transform.GetChild(3).transform.GetChild(i).gameObject.SetActive(false);
                    }

                    //Hats
                    if (m_Hat == "")
                    {
                        Hat = -1;
                    }
                    else if (m_Hat.Contains("BaseballCap"))
                    {
                        Hat = 0;
                        if (HeadVariant == 0)
                        {
                            HairVariant = "Short";
                        }
                    }
                    else if (m_Hat.Contains("BasicWoolHat") || m_Hat.Contains("GEAR_WillToque"))
                    {
                        Hat = 1;
                    }
                    else if (m_Hat.Contains("Toque"))
                    {
                        Hat = 4;
                    }
                    else if (m_Hat.Contains("RabbitskinHat"))
                    {
                        Hat = 5;
                    }
                    else if (m_Hat.Contains("ImprovisedHat"))
                    {
                        Hat = 6;
                    }
                    else if (m_Hat.Contains("WoolWrapCap"))
                    {
                        Hat = 7;
                        if (HeadVariant == 0) // Is Makenzy
                        {
                            HairVariant = "Short";
                        }
                    }
                    else
                    {
                        Hat = 1;
                    }

                    //Torso
                    if (m_Top == "")
                    {
                        Top = 0;
                        ArmsType = "None";
                    }
                    else if (m_Top.Contains("CottonShirt"))
                    {
                        Top = 1;
                        ArmsType = "Middle";
                    }
                    else if (m_Top.Contains("AstridSweater") || m_Top.Contains("FleeceSweater"))
                    {
                        Top = 2;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("WillSweater"))
                    {
                        Top = 3;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("FishermanSweater"))
                    {
                        Top = 4;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("HeavyWoolSweater"))
                    {
                        Top = 5;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("PlaidShirt") || m_Top.Contains("MackinawJacket"))
                    {
                        Top = 6;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("CottonHoodie"))
                    {
                        Top = 7;
                        ArmsType = "Tiny";
                    }
                    else if (m_Top.Contains("CowichanSweater"))
                    {
                        Top = 8;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("WillShirt"))
                    {
                        Top = 9;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("WoolShirt"))
                    {
                        Top = 10;
                        ArmsType = "Short";
                    }
                    else if ((m_Top.Contains("WoolSweater") && m_Top.Contains("HeavyWoolSweater") == false))
                    {
                        Top = 11;
                        ArmsType = "Short";
                    }
                    else if (m_Top.Contains("DownVest") || m_Top.Contains("InsulatedVest"))
                    {
                        Top = 12;
                        ArmsType = "Long";
                    }
                    else if (m_Top.Contains("LightParka"))
                    {
                        Top = 13;
                        ArmsType = "Tiny";
                    }
                    else if ((m_Top.Contains("SkiJacket") && m_Top.Contains("Down") == false) || m_Top.Contains("PremiumWinterCoat"))
                    {
                        Top = 14;
                        ArmsType = "Tiny";
                    }
                    else if (m_Top.Contains("DownSkiJacket"))
                    {
                        Top = 15;
                        ArmsType = "Tiny";
                    }
                    else if (m_Top.Contains("BearSkinCoat"))
                    {
                        Top = 16;
                        ArmsType = "Tiny";
                    }
                    else if (m_Top.Contains("MooseHideCloak"))
                    {
                        Top = 17;
                        ArmsType = "Tiny";
                    }
                    else if (m_Top.Contains("WolfSkinCape"))
                    {
                        Top = 18;
                        ArmsType = "Tiny";
                    }
                    else // If missing clotching
                    {
                        Top = 7;
                        ArmsType = "Tiny";
                    }
                    arms_middle.SetActive(false);
                    arms_short.SetActive(false);
                    arms_tiny.SetActive(false);
                    arms_long.SetActive(false);

                    if (ArmsType != "None")
                    {
                        if (ArmsType == "Tiny")
                        {
                            arms_tiny.SetActive(true);
                        }
                        else if (ArmsType == "Short")
                        {
                            arms_short.SetActive(true);
                        }
                        else if (ArmsType == "Middle")
                        {
                            arms_middle.SetActive(true);
                        }
                        else if (ArmsType == "Long")
                        {
                            arms_long.SetActive(true);
                        }
                    }

                    //Pants
                    if (m_Bottom == "")
                    {
                        Pants = 0;
                    }
                    else if (m_Bottom.Contains("LongUnderwear"))
                    {
                        if (m_Bottom.Contains("Wool"))
                        {
                            Pants = 2;
                        }
                        else
                        {
                            Pants = 1;
                        }
                    }
                    else if (m_Bottom.Contains("Jeans") || m_Bottom.Contains("GEAR_WillPants"))
                    {
                        Pants = 3;
                    }
                    else if (m_Bottom.Contains("CombatPants"))
                    {
                        Pants = 4;
                    }
                    else if (m_Bottom.Contains("InsulatedPants"))
                    {
                        Pants = 5;
                    }
                    else if (m_Bottom.Contains("CargoPants"))
                    {
                        Pants = 6;
                    }
                    else if (m_Bottom.Contains("WorkPants"))
                    {
                        Pants = 7;
                    }
                    else if (m_Bottom.Contains("DeerSkinPants"))
                    {
                        Pants = 8;
                    }
                    else // If missing clotching
                    {
                        Pants = 3;
                    }

                    //Boots
                    if (m_Boots == "")
                    {
                        Boots = 0;
                    }
                    else if (m_Boots.Contains("LeatherShoes"))
                    {
                        Boots = 1;
                    }
                    else if (m_Boots.Contains("CombatBoots"))
                    {
                        Boots = 2;
                    }
                    else if (m_Boots.Contains("WorkBoots") || m_Boots.Contains("GEAR_BasicBoots") || m_Boots.Contains("GreyMotherBoots") || m_Boots.Contains("WillBoots") || m_Boots.Contains("AstridBoots") || m_Boots.Contains("MuklukBoots") || m_Boots.Contains("DeerSkinBoots"))
                    {
                        Boots = 3;
                    }
                    else if (m_Boots.Contains("InsulatedBoots"))
                    {
                        Boots = 4;
                    }
                    else if (m_Boots.Contains("CottonSocks"))
                    {
                        Boots = 5;
                    }
                    else if (m_Boots.Contains("WoolSocks"))
                    {
                        Boots = 6;
                    }
                    else if (m_Boots.Contains("ClimbingSocks"))
                    {
                        Boots = 7;
                    }
                    else if (m_Boots.Contains("BasicShoes"))
                    {
                        Boots = 8;
                    }
                    else // If missing clotching
                    {
                        Boots = 3;
                    }

                    if (m_Debug == true)
                    {
                        Top = m_DebugT;
                        Pants = m_DebugB;
                        arms_tiny.SetActive(true);
                    }

                    if (m_Scarf != "")
                    {
                        if (Top == 0)
                        {
                            Scarf = 2;
                        }
                        else
                        {
                            Scarf = 3;
                        }
                    }
                    else
                    {
                        Scarf = -1;
                    }

                    if (Hat != -1)
                    {
                        clothing.transform.GetChild(0).transform.GetChild(Hat).gameObject.SetActive(true);
                    }
                    if (Scarf != -1)
                    {
                        clothing.transform.GetChild(0).transform.GetChild(Scarf).gameObject.SetActive(true);
                    }


                    if (nakedBodyAtBottom == true)
                    {
                        clothing.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
                    }

                    if (HairShort != null && HairLong != null)
                    {
                        if (HairVariant == "Full")
                        {
                            HairShort.SetActive(false);
                            HairLong.SetActive(true);
                        }
                        else if (HairVariant == "Short")
                        {
                            HairShort.SetActive(true);
                            HairLong.SetActive(false);
                        }
                    }

                    clothing.transform.GetChild(1).transform.GetChild(Top).gameObject.SetActive(true);
                    clothing.transform.GetChild(2).transform.GetChild(Pants).gameObject.SetActive(true);
                    clothing.transform.GetChild(3).transform.GetChild(Boots).gameObject.SetActive(true);
                }
            }
        }
        public class MultiplayerPlayerAnimator : MonoBehaviour
        {
            public MultiplayerPlayerAnimator(IntPtr ptr) : base(ptr) { }
            public Animator m_Animer = null;
            public string m_AnimStateHands = "No";
            public string m_PreAnimStateHands = "";
            public string m_AnimStateFingers = "No";
            public bool m_IsDrink = false;
            public bool m_MyDoll = false;

            void Update()
            {
                if (m_Animer != null)
                {
                    GameObject m_Player = m_Animer.gameObject;
                    int m_ID = ClientUser.myId;
                    string HoldingItem;
                    string m_AnimState;
                    if (!m_MyDoll)
                    {
                        m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                        if (MyMod.playersData[m_ID] != null && MyMod.playersData[m_ID].m_Levelid != MyMod.levelid)
                        {
                            return;
                        }
                        m_AnimState = MyMod.playersData[m_ID].m_AnimState;
                        HoldingItem = m_Player.GetComponent<MultiplayerPlayer>().m_HoldingItem;
                    }
                    else
                    {
                        m_AnimState = MyMod.MyAnimState;
                        HoldingItem = MyMod.MyLightSourceName;
                    }
                    int currentTagHash = m_Animer.GetCurrentAnimatorStateInfo(0).tagHash; // This what tag is now
                    int neededTagHash = Animator.StringToHash(m_AnimState); // This is what tag we need.
                    m_Animer.cullingMode = AnimatorCullingMode.AlwaysAnimate;

                    if (m_AnimState == "Fight")
                    {
                        neededTagHash = Animator.StringToHash("Knock");
                    }

                    // MAIN LAYER
                    if (currentTagHash != neededTagHash || (currentTagHash == Animator.StringToHash("Idle") && HoldingItem == "Book"))
                    {
                        if (m_AnimState == "Walk")
                        {
                            m_Animer.Play("Walk", 0);
                        }
                        if (m_AnimState == "Idle")
                        {
                            //string current_anim = m_Animer.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                            //MelonLogger.Msg("Ctrl current animation " + current_anim);
                            if (currentTagHash == Animator.StringToHash("Harvesting"))
                            {
                                m_Animer.Play("StopHarvesting", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("HarvestingStanding"))
                            {
                                m_Animer.Play("StopHarvestingStanding", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("Igniting"))
                            {
                                m_Animer.Play("StopIgniting", 0);
                                m_Player.GetComponent<MultiplayerPlayer>().UpdateHeldItem();
                            }
                            else if (currentTagHash == Animator.StringToHash("Ctrl"))
                            {
                                m_Animer.Play("Sit_To_Idle", 0);
                            }
                            else
                            {
                                if (HoldingItem != "Book")
                                {
                                    m_Animer.Play("Idle", 0);
                                }
                                else
                                {
                                    if (currentTagHash != Animator.StringToHash("ReadingBook"))
                                    {
                                        if (currentTagHash == Animator.StringToHash("Ctrl"))
                                        {
                                            m_Animer.Play("StartReadSit", 0);
                                        }
                                        else
                                        {
                                            m_Animer.Play("StartRead", 0);
                                        }
                                    }
                                }
                            }
                        }
                        if (m_AnimState == "Run")
                        {
                            m_Animer.Play("Run", 0);
                        }
                        if (m_AnimState == "Ctrl")
                        {
                            string current_anim = m_Animer.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                            //MelonLogger.Msg("Ctrl current animation " + current_anim);
                            if (currentTagHash == Animator.StringToHash("Harvesting"))
                            {
                                m_Animer.Play("StopHarvestingSit", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("HarvestingStanding"))
                            {
                                m_Animer.Play("StopHarvestingStandingSit", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("Igniting"))
                            {
                                m_Animer.Play("StopIgnitingSit", 0);
                                m_Player.GetComponent<MultiplayerPlayer>().UpdateHeldItem();
                            }
                            else if (current_anim != "Male Crouch Pose" && current_anim != "Idle_To_Sit")
                            {
                                m_Animer.Play("Idle_To_Sit", 0);
                            }
                            else
                            {
                                m_Animer.Play("Ctrl", 0);
                            }
                        }
                        if (m_AnimState == "Sit")
                        {
                            m_Animer.Play("Sitting Idle", 0);
                        }
                        if (m_AnimState == "Flex")
                        {
                            m_Animer.Play("Samba Dancing", 0);
                        }
                        if (m_AnimState == "Cringe2")
                        {
                            m_Animer.Play("Cringe2", 0);
                        }
                        if (m_AnimState == "Cringe1")
                        {
                            m_Animer.Play("Cringe1", 0);
                        }
                        if (m_AnimState == "Knock" || m_AnimState == "Fight")
                        {
                            m_Animer.Play("Knock", 0);
                        }
                        if (m_AnimState == "Map")
                        {
                            m_Animer.Play("Map", 0);
                        }

                        if (m_AnimState == "Sleep")
                        {
                            m_Animer.Play("Sleep", 0);
                        }
                        if (m_AnimState == "RopeIdle")
                        {
                            string current_anim = m_Animer.GetCurrentAnimatorClipInfo(0)[0].clip.name;

                            if (current_anim == "RopeMove" && current_anim != "RopeStop")
                            {
                                m_Animer.Play("RopeStop", 0);
                            }
                            else
                            {
                                m_Animer.Play("RopeIdle", 0);
                            }
                        }
                        if (m_AnimState == "RopeMoving")
                        {
                            string current_anim = m_Animer.GetCurrentAnimatorClipInfo(0)[0].clip.name;

                            if (current_anim == "RopeIdle" && current_anim != "RopeStart")
                            {
                                m_Animer.Play("RopeStart", 0);
                            }
                            else
                            {
                                m_Animer.Play("RopeMoving", 0);
                            }
                        }
                        if (m_AnimState == "Eating")
                        {
                            if (MyMod.ServerConfig.m_FastConsumption == false)
                            {
                                m_Animer.Play("Eating", 0);
                            }
                            else
                            {
                                m_Animer.Play("EatingFast", 0);
                            }
                        }
                        if (m_AnimState == "Drinking")
                        {
                            if (MyMod.ServerConfig.m_FastConsumption == false)
                            {
                                m_Animer.Play("Drinking", 0);
                            }
                            else
                            {
                                m_Animer.Play("DrinkingFast", 0);
                            }
                        }
                        if (m_AnimState == "Harvesting")
                        {
                            if (currentTagHash == Animator.StringToHash("Idle"))
                            {
                                m_Animer.Play("StartHarvesting", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("Ctrl"))
                            {
                                m_Animer.Play("StartHarvestingSit", 0);
                            }
                            else
                            {
                                m_Animer.Play("Harvesting", 0);
                            }
                        }
                        if (m_AnimState == "HarvestingStanding")
                        {
                            if (currentTagHash == Animator.StringToHash("Idle"))
                            {
                                m_Animer.Play("StartHarvestingStanding", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("Ctrl"))
                            {
                                m_Animer.Play("StartHarvestingStandingSit", 0);
                            }
                            else
                            {
                                m_Animer.Play("HarvestingStanding", 0);
                            }
                        }
                        if (m_AnimState == "Igniting")
                        {
                            m_Player.GetComponent<MultiplayerPlayer>().UpdateHeldItem();
                            if (currentTagHash == Animator.StringToHash("Idle"))
                            {
                                m_Animer.Play("StartIgniting", 0);
                            }
                            else if (currentTagHash == Animator.StringToHash("Ctrl"))
                            {
                                m_Animer.Play("StartIgnitingSit", 0);
                            }
                            else
                            {
                                m_Animer.Play("StartIgniting", 0);
                            }
                        }
                    }

                    // HANDS LAYER

                    if (m_AnimStateHands == "UnAimGunDone")
                    {
                        m_AnimStateHands = "No";
                    }

                    if (m_AnimState == "Drinking" || m_AnimState == "Eating" || m_AnimState == "Harvesting" || m_AnimState == "HarvestingStanding" || m_AnimState == "Igniting")
                    {
                        m_PreAnimStateHands = "";
                        m_AnimStateHands = "No";
                    }
                    else
                    {
                        if (HoldingItem == "GEAR_Rifle")
                        {
                            if (m_AnimState != "Ctrl")
                            {
                                if (m_AnimStateHands != "Rifle" && m_AnimStateHands != "Rifle_Sit" && m_AnimStateHands != "RifleAim")
                                {
                                    m_PreAnimStateHands = "Pick";
                                }
                                if ((m_MyDoll && MyMod.MyIsAiming) || (!m_MyDoll && MyMod.playersData[m_ID].m_Aiming))
                                {
                                    m_AnimStateHands = "RifleAim";
                                }
                                else
                                {
                                    m_AnimStateHands = "Rifle";
                                }
                            }
                            else
                            {
                                if (m_AnimStateHands != "Rifle" && m_AnimStateHands != "Rifle_Sit" && m_AnimStateHands != "RifleAim_Sit")
                                {
                                    m_PreAnimStateHands = "Pick_Sit";
                                }
                                if ((m_MyDoll && MyMod.MyIsAiming) || (!m_MyDoll && MyMod.playersData[m_ID].m_Aiming))
                                {
                                    m_AnimStateHands = "RifleAim_Sit";
                                }
                                else
                                {
                                    m_AnimStateHands = "Rifle_Sit";
                                }
                            }
                        }
                        else if (HoldingItem.StartsWith("GEAR_Revolver"))
                        {
                            if (m_AnimState == "HoldGun")
                            {
                                if (m_AnimStateHands != "HoldGun" && m_AnimStateHands != "HoldGun_Sit")
                                {
                                    m_PreAnimStateHands = "AimGun";
                                }
                                m_AnimStateHands = "HoldGun";
                            }
                            else if (m_AnimState == "HoldGun_Sit")
                            {
                                if (m_AnimStateHands != "HoldGun" && m_AnimStateHands != "HoldGun_Sit")
                                {
                                    m_PreAnimStateHands = "AimGun_Sit";
                                }
                                m_AnimStateHands = "HoldGun_Sit";
                            }
                            else
                            {
                                if (m_AnimStateHands == "HoldGun")
                                {
                                    m_PreAnimStateHands = "UnAimGun";
                                    m_AnimStateHands = "No";
                                }
                                else
                                {
                                    m_AnimStateHands = "No";
                                }
                            }
                        }
                        else if (HoldingItem.StartsWith("GEAR_KeroseneLamp"))
                        {
                            m_PreAnimStateHands = "";
                            m_AnimStateHands = "HoldLantern";
                        }
                        else if (HoldingItem == "GEAR_Bow")
                        {
                            m_AnimStateHands = "Bow";
                        }
                        else
                        {
                            m_PreAnimStateHands = "";
                            m_AnimStateHands = "No";
                        }
                    }

                    int handsTagHash = m_Animer.GetCurrentAnimatorStateInfo(1).tagHash; // This what tag is now
                    int handsNeededTagHash = Animator.StringToHash(m_AnimStateHands); // This is what tag we need.

                    if (handsTagHash != handsNeededTagHash)
                    {
                        if (m_PreAnimStateHands == "")
                        {
                            if (handsTagHash == Animator.StringToHash("Rifle") && m_AnimStateHands == "RifleAim")
                            {
                                m_Animer.Play("StartRifleAim", 1);
                            }
                            else if (handsTagHash == Animator.StringToHash("RifleAim") && m_AnimStateHands == "Rifle")
                            {
                                m_Animer.Play("EndRifleAim", 1);
                            }
                            else if (handsTagHash == Animator.StringToHash("Rifle_Sit") && m_AnimStateHands == "RifleAim_Sit")
                            {
                                m_Animer.Play("StartRifleAim_Sit", 1);
                            }
                            else if (handsTagHash == Animator.StringToHash("RifleAim_Sit") && m_AnimStateHands == "Rifle_Sit")
                            {
                                m_Animer.Play("EndRifleAim_Sit", 1);
                            }
                            else
                            {
                                m_Animer.Play(m_AnimStateHands, 1);
                            }
                        }
                        else
                        {
                            m_Animer.Play(m_PreAnimStateHands, 1);
                            m_PreAnimStateHands = "";
                        }
                    }
                    // FINGERS LAYER

                    if (m_AnimState == "Drinking" || m_AnimState == "Eating" || m_AnimState == "Harvesting" || m_AnimState == "HarvestingStanding" || m_AnimState == "Igniting")
                    {
                        m_AnimStateFingers = "No";
                    }
                    else
                    {
                        if (HoldingItem == "GEAR_Revolver" || HoldingItem == "GEAR_FlareGun")
                        {
                            m_AnimStateFingers = "HoldRevolver";
                        }
                        else if (HoldingItem == "GEAR_Stone" || HoldingItem == "GEAR_FlareA" || HoldingItem == "GEAR_BlueFlare" || HoldingItem == "GEAR_Torch" || HoldingItem == "GEAR_NoiseMaker")
                        {
                            m_AnimStateFingers = "Hold";
                        }
                        else if (HoldingItem.StartsWith("GEAR_SprayPaint"))
                        {
                            m_AnimStateFingers = "HoldCan";
                        }
                        else if (HoldingItem == "GEAR_PackMatches" || HoldingItem == "GEAR_WoodMatches")
                        {
                            m_AnimStateFingers = "HoldMatch";
                        }
                        else
                        {
                            m_AnimStateFingers = "No";
                        }
                    }

                    int fingersTagHash = m_Animer.GetCurrentAnimatorStateInfo(2).tagHash; // This what tag is now
                    int fingersNeededTagHash = Animator.StringToHash(m_AnimStateFingers); // This is what tag we need.

                    if (fingersTagHash != fingersNeededTagHash)
                    {
                        m_Animer.Play(m_AnimStateFingers, 2);
                    }

                    if (!m_MyDoll && (m_AnimState == "Walk" || m_AnimState == "Run"))
                    {
                        GameObject foot_l = m_Player.transform.GetChild(3).GetChild(8).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;
                        GameObject foot_r = m_Player.transform.GetChild(3).GetChild(8).GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject;

                        float main_y = m_Player.transform.position.y; // Y of player object.
                        float leg_y_r = foot_r.transform.position.y;
                        float leg_y_l = foot_l.transform.position.y;
                        float fixed_y_r = main_y - leg_y_r;
                        float fixed_y_l = main_y - leg_y_l;

                        double max = -0.05;
                        double min = -0.057;

                        if (fixed_y_r >= min && fixed_y_r <= max && MyMod.StepState != 1)
                        {
                            MyMod.StepState = 1;
                            MyMod.MaybeLeaveFootPrint(foot_r.transform.position, m_Player, false, 0.0f, false);
                            string ground_Tag = Utils.GetMaterialTagForObjectAtPosition(m_Player, foot_r.transform.position);
                            GameAudioManager.Play3DSound(AK.EVENTS.PLAY_FOOTSTEPSWOLFWALK, m_Player);
                        }
                        if (fixed_y_l >= min && fixed_y_l <= max && MyMod.StepState != 2)
                        {
                            MyMod.StepState = 2;
                            MyMod.MaybeLeaveFootPrint(foot_l.transform.position, m_Player, false, 0.0f, true);
                            string ground_Tag = Utils.GetMaterialTagForObjectAtPosition(m_Player, foot_l.transform.position);
                            GameAudioManager.Play3DSound(AK.EVENTS.PLAY_FOOTSTEPSWOLFWALK, m_Player);
                        }
                    }
                    else
                    {
                        MyMod.StepState = 0;
                    }
                }
            }
            public void Pickup()
            {
                GameObject m_Player = m_Animer.gameObject;
                int m_ID = ClientUser.myId;
                string m_AnimState = MyMod.MyAnimState;
                int armTagHash = m_Animer.GetCurrentAnimatorStateInfo(3).tagHash;
                int armNeededTagHash = Animator.StringToHash("Pickup");

                if (!m_MyDoll)
                {
                    m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                    if (MyMod.playersData[m_ID] == null || MyMod.playersData[m_ID].m_Levelid != MyMod.levelid || MyMod.playersData[m_ID].m_LevelGuid != MyMod.level_guid)
                    {
                        return;
                    }
                    m_AnimState = MyMod.playersData[m_ID].m_AnimState;
                }


                //MelonLogger.Msg("Current state of  " + m_ID + " is "+ m_AnimState);
                //MelonLogger.Msg("ArmTagHash  " + armTagHash + " TagHash we need " + armNeededTagHash);
                if (armTagHash != armNeededTagHash)
                {
                    if (m_AnimState != "Ctrl")
                    {
                        m_Animer.Play("DoPickup", 3);
                        //MelonLogger.Msg("Playing DoPickup");
                    }
                    else
                    {
                        m_Animer.Play("DoSitPickup", 3);
                        //MelonLogger.Msg("Playing DoSitPickup");
                    }
                }
            }
            public void DoLeftHandEmote(string Emote)
            {
                GameObject m_Player = m_Animer.gameObject;
                int m_ID = ClientUser.myId;
                //string m_AnimState = MyMod.MyAnimState;
                int armTagHash = m_Animer.GetCurrentAnimatorStateInfo(3).tagHash;
                int armNeededTagHash = Animator.StringToHash("Pickup");
                int RequiredState = Animator.StringToHash("No");

                //if (!m_MyDoll)
                //{
                //    m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                //    if (MyMod.playersData[m_ID] == null || MyMod.playersData[m_ID].m_Levelid != MyMod.levelid || MyMod.playersData[m_ID].m_LevelGuid != MyMod.level_guid)
                //    {
                //        return;
                //    }
                //    m_AnimState = MyMod.playersData[m_ID].m_AnimState;
                //}

                if (armTagHash == RequiredState)
                {
                    m_Animer.Play(Emote, 3);
                }
            }
            public void MeleeAttack()
            {
                GameObject m_Player = m_Animer.gameObject;
                int m_ID = ClientUser.myId;
                string m_AnimState = MyMod.MyAnimState;
                if (!m_MyDoll)
                {
                    m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                    if (MyMod.playersData[m_ID] == null || MyMod.playersData[m_ID].m_Levelid != MyMod.levelid || MyMod.playersData[m_ID].m_LevelGuid != MyMod.level_guid)
                    {
                        return;
                    }
                    m_AnimState = MyMod.playersData[m_ID].m_AnimState;
                }

                int armTagHash = m_Animer.GetCurrentAnimatorStateInfo(4).tagHash;
                int armNeededTagHash = Animator.StringToHash("Melee");
                if (armTagHash != armNeededTagHash)
                {
                    m_Animer.Play("Melee", 4);
                }
            }
            public void Consumption()
            {
                GameObject m_Player = m_Animer.gameObject;
                int m_ID = ClientUser.myId;

                if (!m_MyDoll)
                {
                    m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                    if (MyMod.playersData[m_ID] == null || MyMod.playersData[m_ID].m_Levelid != MyMod.levelid || MyMod.playersData[m_ID].m_LevelGuid != MyMod.level_guid)
                    {
                        return;
                    }
                    if (m_IsDrink == true)
                    {
                        MyMod.playersData[m_ID].m_AnimState = "Drinking";
                        m_Player.GetComponent<MultiplayerPlayer>().m_HoldingFood = "Water";
                    } else
                    {
                        MyMod.playersData[m_ID].m_AnimState = "Eating";
                        m_Player.GetComponent<MultiplayerPlayer>().m_HoldingFood = "Food";
                    }
                    m_Player.GetComponent<MultiplayerPlayer>().UpdateHeldItem();
                }
            }
            public void StopConsumption()
            {
                GameObject m_Player = m_Animer.gameObject;
                int m_ID = ClientUser.myId;

                if (!m_MyDoll)
                {
                    m_ID = m_Player.GetComponent<MultiplayerPlayer>().m_ID;
                    if (MyMod.playersData[m_ID] == null || MyMod.playersData[m_ID].m_Levelid != MyMod.levelid || MyMod.playersData[m_ID].m_LevelGuid != MyMod.level_guid)
                    {
                        return;
                    }
                    if (m_Player.GetComponent<MultiplayerPlayer>().m_HoldingFood != "")
                    {
                        m_Player.GetComponent<MultiplayerPlayer>().m_HoldingFood = "";
                        m_Player.GetComponent<MultiplayerPlayer>().UpdateHeldItem();
                    }
                }
            }
        }
        public class MultiplayerPlayerVoiceChatPlayer : MonoBehaviour
        {
            public MultiplayerPlayerVoiceChatPlayer(IntPtr ptr) : base(ptr) { }
            public AudioSource aSource;
            public AudioSource aSourceBgNoise;
            public List<DataStr.VoiceChatQueueElement> VoiceQueue = new List<DataStr.VoiceChatQueueElement>();
            public int m_ID = 0;
            public bool m_RadioFilter = false;
            public static float PlayNextIn = 0;
            public static float Trim = 0.03f;
            public bool PlayBgNoise = false;
            public bool IsLastClip = false;
            void Update()
            {
                if (aSource != null)
                {
                    if (Time.time > PlayNextIn)
                    {
                        if (IsLastClip)
                        {
                            IsLastClip = false;
                            PlayBgNoise = false;
                        }
                        if (VoiceQueue.Count > 0)
                        {
                            PlayBgNoise = true;
                            DataStr.VoiceChatQueueElement Voice = VoiceQueue[0];
                            PlayNextIn = Time.time + Voice.m_Length;
                            MyMod.PlayVoiceFromPlayerObject(aSource, Voice.m_VoiceData, m_RadioFilter);
                            VoiceQueue.RemoveAt(0);
                            if (VoiceQueue.Count == 0)
                            {
                                IsLastClip = true;
                            }
                        }
                    }
                }
                if (aSourceBgNoise != null)
                {
                    aSourceBgNoise.enabled = PlayBgNoise;
                }
            }
        }
        public class PlayerBulletDamage : MonoBehaviour
        {
            public PlayerBulletDamage(IntPtr ptr) : base(ptr) { }
            public int m_Damage = 0;
            public GameObject m_Obj = null;
            public int m_ClientId = 0;
            public GameObject m_Player = null;
            public int m_Type = 0; // 0 - Head, 1 - Chest, 2 - Rigth Arm, 3 - Left Arm, 4 - Right Leg, 5 - Left Leg

            public void OnCollisionEnter(Collision col)
            {
                if (col.gameObject.GetComponent<ArrowItem>() != null)
                {
                    ArrowItem ARR = col.gameObject.GetComponent<ArrowItem>();
                    if (col.gameObject.GetComponent<DestoryArrowOnHit>() == null)
                    {
                        ARR.m_ArrowMesh.GetComponent<BoxCollider>().enabled = false;
                        MelonLogger.Msg("Arrow colided other player, and dealing damage " + m_Damage);
                        if (MyMod.sendMyPosition == true)
                        {
                            using (Packet _packet = new Packet((int)ClientPackets.BULLETDAMAGE))
                            {
                                _packet.Write((float)m_Damage);
                                _packet.Write(m_Type);
                                _packet.Write(m_ClientId);
                                _packet.Write(false);
                                MyMod.SendUDPData(_packet);
                            }
                        }
                        if (MyMod.iAmHost == true)
                        {
                            using (Packet _packet = new Packet((int)ServerPackets.BULLETDAMAGE))
                            {
                                ServerSend.BULLETDAMAGE(m_ClientId, (float)m_Damage, m_Type, 0);
                            }
                        }
                    }
                }
            }

            public void SetLocaZone(GameObject t, Comps.MultiplayerPlayer pl, int bodyPart)
            {
                m_Obj = t;
                m_Obj.tag = "Flesh";
                m_Obj.layer = vp_Layer.Container;
                m_Type = bodyPart;

                if (m_Obj.name == "Chest")
                {
                    m_Damage = 50;
                }
                else if (m_Obj.name.StartsWith("arm"))
                {
                    m_Damage = 30;
                }
                else if (m_Obj.name.StartsWith("Head"))
                {
                    m_Damage = 70;
                }
                else if (m_Obj.name.StartsWith("Thigh"))
                {
                    m_Damage = 20;
                }
                m_Player = pl.m_Player;
                pl.m_DamageColiders.Add(m_Obj);
                m_ClientId = pl.m_ID;
                //MelonLogger.Msg(m_Obj.name + " = "+ m_Damage);
            }
        }
        public class FakeRockCache : MonoBehaviour
        {
            public FakeRockCache(IntPtr ptr) : base(ptr) { }
            public string m_GUID = "";
            public string m_Owner = "Unknown";
            public int m_Rocks = 0;
            public int m_Sticks = 0;
            public DataStr.FakeRockCacheVisualData m_VisualData = new DataStr.FakeRockCacheVisualData();

            public void Created(bool Sync = true)
            {
                m_VisualData.m_GUID = m_GUID;
                m_VisualData.m_LevelGUID = level_guid;
                m_VisualData.m_Owner = m_Owner;
                m_VisualData.m_Position = gameObject.transform.position;
                m_VisualData.m_Rotation = gameObject.transform.rotation;

                if (sendMyPosition)
                {
                    if (Sync)
                    {
                        using (Packet _packet = new Packet((int)ClientPackets.ADDROCKCACH))
                        {
                            _packet.Write(m_VisualData);
                            SendUDPData(_packet);
                        }
                    }
                    return;
                }
                MPSaveManager.AddRockCach(m_VisualData, 0);
            }

            public void Open()
            {
                if(gameObject != null && gameObject.GetComponent<Container>())
                {
                    GameManager.GetPlayerManagerComponent().ProcessContainerInteraction(gameObject.GetComponent<Container>());
                }
            }

            public void DismantleFinished()
            {
                if (m_Rocks > 0)
                {
                    GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_Stone", m_Rocks);
                    string message = Localization.Get("GAMEPLAY_Stone") + " (" + m_Rocks + ")";
                    GearMessage.AddMessage("GEAR_Stone", Localization.Get("GAMEPLAY_Harvested"), message);
                }
                if (m_Sticks > 0)
                {
                    GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory("GEAR_Stick", m_Sticks);
                    string message = Localization.Get("GAMEPLAY_Stick") + " (" + m_Sticks + ")";
                    GearMessage.AddMessage("GEAR_Stick", Localization.Get("GAMEPLAY_Harvested"), message);
                }
                UnityEngine.Object.Destroy(gameObject);
                if (sendMyPosition)
                {
                    using (Packet _packet = new Packet((int)ClientPackets.REMOVEROCKCACHFINISHED))
                    {
                        _packet.Write(m_VisualData);
                        SendUDPData(_packet);
                    }
                    return;
                }
                MPSaveManager.RemoveRockCach(m_VisualData, 0);
                ServerSend.REMOVEROCKCACH(0, m_VisualData, 1, m_VisualData.m_LevelGUID);
            }
            public void Remove()
            {
                DataStr.BrokenFurnitureSync furn = new DataStr.BrokenFurnitureSync();
                furn.m_Guid = m_GUID;
                furn.m_ParentGuid = "";
                furn.m_LevelID = levelid;
                furn.m_LevelGUID = GameManager.m_SceneTransitionData.m_SceneSaveFilenameCurrent;
                if (sendMyPosition)
                {
                    DoPleaseWait("Please wait...", "Preparing to dismantle...");
                    PendingRockCahceRemove = this;
                    using (Packet _packet = new Packet((int)ClientPackets.REMOVEROCKCACH))
                    {
                        _packet.Write(m_VisualData);
                        SendUDPData(_packet);
                    }
                    using (Packet _packet = new Packet((int)ClientPackets.FURNBREAKINGGUID))
                    {
                        _packet.Write(furn);
                        SendUDPData(_packet);
                    }
                    return;
                } else
                {
                    for (int i = 0; i < playersData.Count; i++)
                    {
                        if (playersData[i] != null)
                        {
                            if ((playersData[i].m_BrakingObject != null && playersData[i].m_BrakingObject.m_Guid == m_GUID) || (playersData[i].m_Container != null && playersData[i].m_Container.m_Guid == m_GUID))
                            {
                                HUDMessage.AddMessage(playersData[i].m_Name + " INTERACTING WITH THIS!");
                                return;
                            }
                        }
                    }
                }

                bool NotEmpty = MPSaveManager.ContainerNotEmpty(level_guid, m_GUID);
                if (NotEmpty)
                {
                    HUDMessage.AddMessage("Rock cache should be empty!");
                    return;
                }
                ServerSend.FURNBREAKINGGUID(0, furn, true);
                GameManager.s_IsAISuspended = true;
                Pathes.FakeRockCacheCallback = this;
                InterfaceManager.m_Panel_GenericProgressBar.Launch(Localization.Get("GAMEPLAY_BreakingDownProgress"), 2f, 10, 0.0f, "Play_RockCache", (string)null, false, false, null);
            }
        }
        public class LocalVariablesKit : MonoBehaviour
        {
            public LocalVariablesKit(IntPtr ptr) : base(ptr) { }
            public string m_String = "";
            public int m_Int = 0;
            public float m_Float = 0;
            public double m_Double = 0;
        }

        public class ParticleSystemParasite : MonoBehaviour
        {
            public ParticleSystemParasite(IntPtr ptr) : base(ptr) { }
            public ParticleSystem m_ParticleSystem = null;
            void Update()
            {
                if(gameObject != null)
                {
                    if(m_ParticleSystem != null)
                    {
                        if (!m_ParticleSystem.isPlaying)
                        {
                            m_ParticleSystem.Play();
                        }
                    }
                }
            }
        }

        public class ExpeditionInteractive : MonoBehaviour
        {
            public ExpeditionInteractive(IntPtr ptr) : base(ptr) { }
            public string m_ObjectText = "Object";
            public string m_InteractText = "Interacting...";
            public float m_InteractTime = 1f;
            public string m_Tool = "";
            public string m_Material = "";
            public int m_MaterialCount = 1;

            public void Load(ExpeditionInteractiveData Data)
            {
                m_ObjectText = Data.m_ObjectText;
                m_InteractText = Data.m_InteractText;
                m_InteractTime = Data.m_InteractTime;
                m_Tool = Data.m_Tool;
                m_Material = Data.m_Material;
                m_MaterialCount = Data.m_MaterialCount;
                gameObject.transform.position = Data.m_Position;
                gameObject.transform.rotation = Data.m_Rotation;
                gameObject.transform.localScale = Data.m_Scale;
            }

            public void TryInteract()
            {
                if (CanInteract())
                {

                }
            }

            public bool CanInteract()
            {
                bool HaveTool = false;
                if (string.IsNullOrEmpty(m_Tool))
                {
                    HaveTool = true;
                } else
                {
                    HaveTool = GameManager.GetInventoryComponent().HasNonRuinedItem(m_Tool);
                }
                bool HaveMaterials = false;
                if (string.IsNullOrEmpty(m_Material) || m_MaterialCount == 0)
                {
                    HaveMaterials = true;
                }else 
                {
                    int Num = GameManager.GetInventoryComponent().NumGearInInventory(m_Material);

                    if(Num >= m_MaterialCount)
                    {
                        HaveMaterials = true;
                    }
                    HUDMessage.AddMessage("You need ");
                }

                if (!HaveTool && !HaveMaterials)
                {
                    HUDMessage.AddMessage("You need "+ Utils.GetGearDisplayName(m_Tool) + " and x"+m_MaterialCount+" "+ Utils.GetGearDisplayName(m_Material) + "!");
                    GameAudioManager.PlayGUIError();
                } else if(!HaveTool && HaveMaterials) 
                {
                    HUDMessage.AddMessage("You need " + Utils.GetGearDisplayName(m_Tool) + "!");
                    GameAudioManager.PlayGUIError();
                } else if (HaveTool && !HaveMaterials)
                {
                    HUDMessage.AddMessage("You need x" + m_MaterialCount + " " + Utils.GetGearDisplayName(m_Material) + "!");
                    GameAudioManager.PlayGUIError();
                }

                return HaveTool && HaveMaterials;
            }
        }
    }
}
