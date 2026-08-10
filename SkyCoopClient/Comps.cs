
using Harmony;
using Il2Cpp;
using Il2CppAK;
using Il2CppInterop.Runtime.Injection;
using Il2CppMS.Internal.Xml.XPath;
using Il2CppRewired;
using Il2CppTLD.Cooking;
using Il2CppTLD.Interactions;
using Il2CppTMPro;
using SkyCoopClient;
using SkyCoopServer;
using UnityEngine;
using static Il2Cpp.UIAtlas;
using static Il2CppMono.Security.X509.X520;
using static SkyCoopServer.DataStr;

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
            ClassInjector.RegisterTypeInIl2Cpp<TeammateMapIcon>();
            ClassInjector.RegisterTypeInIl2Cpp<ZoneMapIcon>();
            ClassInjector.RegisterTypeInIl2Cpp<SendGearIfNotDestoryed>();
            ClassInjector.RegisterTypeInIl2Cpp<PropMovemenetPredict>();
            ClassInjector.RegisterTypeInIl2Cpp<ChatMessage>();
            ClassInjector.RegisterTypeInIl2Cpp<GearCookingTarget>();
            ClassInjector.RegisterTypeInIl2Cpp<GearCookingDummy>();
            ClassInjector.RegisterTypeInIl2Cpp<GearCookingVisual>();
            ClassInjector.RegisterTypeInIl2Cpp<CookingSlotVisual>();
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
            public string m_PrefabName = "";
            public string m_GUID = "";
            public string m_LocalizedName = "GearItem";
            public int m_Style = 0;
            public GearCookingVisual m_CookingVisual;

            public SimpleInteraction m_SimpeInteraction = null;

            void Start()
            {
                LocalizedString Str = new LocalizedString();
                Str.m_LocalizationID = m_LocalizedName;

                if (m_SimpeInteraction == null)
                {
                    m_SimpeInteraction = gameObject.AddComponent<SimpleInteraction>();
                    m_SimpeInteraction.m_DefaultHoverText = Str;
                    m_SimpeInteraction.HoverText = m_LocalizedName;
                    m_SimpeInteraction.m_CanInteract = true;
                }
            }

            void FixedUpdate()
            {
                if (m_SimpeInteraction)
                {
                    if(m_CookingVisual == null || string.IsNullOrEmpty(m_CookingVisual.m_CookingResult))
                    {
                        m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID = m_LocalizedName;
                        m_SimpeInteraction.HoverText = m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID;
                    }else
                    {
                        Panel_ActionsRadial Panel = InterfaceManager.GetPanel<Panel_ActionsRadial>();

                        Color RuinedColor = Color.red;
                        Color HotColor = Color.yellow;
                        Color ColdColor = Color.cyan;
                        Color ReadyColor = Color.yellow;

                        if (Panel)
                        {
                            HotColor = Panel.m_FoodColdStatusColor;
                            ColdColor = Panel.m_FoodColdStatusColor;
                            ReadyColor = HotColor;
                        }

                        GearsSync.CookedState State = m_CookingVisual.GetState();

                        if (!string.IsNullOrEmpty(m_CookingVisual.m_CookingResult) && !m_CookingVisual.IsCooking() && State != GearsSync.CookedState.Overcooked)
                        {

                            string ItemName = m_LocalizedName;

                            if (!string.IsNullOrEmpty(m_CookingVisual.m_LocalizedOverrideName))
                            {
                                ItemName = m_CookingVisual.m_LocalizedOverrideName;
                            }

                            string CookedName = ItemName;

                            string NameToUse = ItemName;
                            string Affix = "";
                            string Debug = "";


                            if (!string.IsNullOrEmpty(m_CookingVisual.m_LocalizedCookedName))
                            {
                                CookedName = m_CookingVisual.m_LocalizedCookedName;
                            }

                            if (State == GearsSync.CookedState.Cooked)
                            {
                                if(m_CookingVisual.m_CookingResult == "GoodWater")
                                {
                                    NameToUse = Localization.Get("GAMEPLAY_CookingPotableWater");
                                    Affix = $"\n{Localization.Get("GAMEPLAY_Boiled")}";
                                }
                                else if(m_CookingVisual.m_CookingResult == "BadWater")
                                {
                                    NameToUse = Localization.Get("GAMEPLAY_CookingNonPotableWater");
                                    Affix = $"\n{Localization.Get("GAMEPLAY_Paused")}";
                                    if (GearsSync.s_LiquidCookingDebug)
                                    {
                                        Debug = $" {GearsSync.CalculateWaterVolume_Debug(m_CookingVisual.m_BeingCookedTime, m_CookingVisual.m_Volume)}L";
                                    }
                                }
                                else
                                {
                                    NameToUse = CookedName;
                                    Affix = $"\n{Localization.Get("GAMEPLAY_Cooked")}";
                                }
                            }
                            else
                            {
                                NameToUse = ItemName;

                                if(m_CookingVisual.m_BeingCookedTime > 0)
                                {
                                    Affix = $"\n{Localization.Get("GAMEPLAY_Paused")}";
                                }
                                else
                                {
                                    Affix = "";
                                }
                            }

                            m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID = $"{NameToUse}{Affix}{Debug}";
                            m_SimpeInteraction.HoverText = m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID;
                        }
                        else
                        {
                            if (m_CookingVisual.m_CookingResult == "BadWater")
                            {
                                string TimeLable = Localization.Get("GAMEPLAY_TimeUntilMelted");
                                string DurationString = Utils.GetDurationString(Mathf.CeilToInt(m_CookingVisual.GetHours() * 60));
                                TimeLable = TimeLable.Replace("{time-val}", DurationString);

                                string Debug = "";

                                if (GearsSync.s_LiquidCookingDebug)
                                {
                                    Debug = $" {GearsSync.CalculateWaterVolume_Debug(m_CookingVisual.m_BeingCookedTime, m_CookingVisual.m_Volume)}L";
                                }

                                m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID = $"{Localization.Get("GAMEPLAY_Snow")}\n{TimeLable}{Debug}";
                                m_SimpeInteraction.HoverText = m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID;
                            }
                            else if (m_CookingVisual.m_CookingResult == "GoodWater")
                            {
                                string TimeLable = "";
                                string DurationString = Utils.GetDurationString(Mathf.CeilToInt(m_CookingVisual.GetHours() * 60));

                                switch (State)
                                {
                                    case GearsSync.CookedState.Raw:
                                        TimeLable = Localization.Get("GAMEPLAY_TimeUntilBoiled");
                                        TimeLable = TimeLable.Replace("{time-val}", DurationString);
                                        m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID = $"{Localization.Get("GAMEPLAY_CookingNonPotableWater")}\n{TimeLable}";
                                        break;
                                    case GearsSync.CookedState.Cooked:
                                        TimeLable = Localization.Get("GAMEPLAY_TimeUntilBoiledDry");
                                        TimeLable = TimeLable.Replace("{time-val}", DurationString);
                                        m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID = $"{Localization.Get("GAMEPLAY_CookingPotableWater")}\n{TimeLable}";
                                        break;
                                    case GearsSync.CookedState.Overcooked:
                                        m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID = m_LocalizedName;
                                        break;
                                    default:
                                        break;
                                }
                                m_SimpeInteraction.HoverText = m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID;
                            }
                            else
                            {
                                string TimeLable = "";
                                string DurationString = Utils.GetDurationString(Mathf.CeilToInt(m_CookingVisual.GetHours() * 60));
                                string ItemName = m_LocalizedName;

                                switch (State)
                                {
                                    case GearsSync.CookedState.Raw:
                                        TimeLable = Localization.Get("GAMEPLAY_TimeUntilReady");
                                        TimeLable = TimeLable.Replace("{time-val}", DurationString);
                                        
                                        if (!string.IsNullOrEmpty(m_CookingVisual.m_LocalizedOverrideName))
                                        {
                                            ItemName = m_CookingVisual.m_LocalizedOverrideName;
                                        }
                                        m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID = $"{ItemName}\n{TimeLable}";
                                        break;
                                    case GearsSync.CookedState.Cooked:
                                        TimeLable = Localization.Get("GAMEPLAY_TimeUntilBurned");
                                        TimeLable = TimeLable.Replace("{time-val}", DurationString);

                                        if (!string.IsNullOrEmpty(m_CookingVisual.m_LocalizedOverrideName))
                                        {
                                            ItemName = m_CookingVisual.m_LocalizedOverrideName;
                                        }

                                        if (!string.IsNullOrEmpty(m_CookingVisual.m_LocalizedCookedName))
                                        {
                                            ItemName = m_CookingVisual.m_LocalizedCookedName;
                                        }
                                        m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID = $"{ItemName}\n{TimeLable}";
                                        break;
                                    case GearsSync.CookedState.Overcooked:
                                        m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID = $"{Utils.GetStringFromColor(RuinedColor)}{Localization.Get("GAMEPLAY_InedibleBurnedDebris")}[-]";
                                        break;
                                    default:
                                        break;
                                }
                                m_SimpeInteraction.HoverText = m_SimpeInteraction.m_DefaultHoverText.m_LocalizationID;
                            }
                        }
                    }
                }
            }


        }

        public class GearCookingVisual : MonoBehaviour
        {
            public GearCookingVisual(IntPtr ptr) : base(ptr) { }
            public DroppedGearVisual m_Gear;
            public CookingSlot m_CookingSlot = null;
            public int m_CookingSlotIndex = -1;
            public string m_FireGUID = "";
            public string m_CookingResult = "";
            public float m_CookingTime = 0;
            public float m_BurningTime = 0;
            public float m_BeingCookedTime = 0;
            public float m_Volume = 0;
            public string m_LocalizedCookedName = string.Empty;
            public string m_LocalizedOverrideName = string.Empty;

            public MeshRenderer m_GrubMeshRenderer;
            public MeshFilter m_GrubMeshFilter;

            public GameObject m_RawObject = null;
            public GameObject m_CookingReadyObject = null;
            public Material[] m_RuinedMaterials;
            public Material[] m_CookingPotMaterialsList;
            public Material[] m_CookingPotRawMaterialsList;
            public Mesh m_RawMesh;
            public Mesh m_ReadyMesh;

            public Material[] m_MeltSnowMaterialsList;
            public Material[] m_BoilWaterPotMaterialsList;
            public Material[] m_BoilWaterReadyMaterialsList;

            public Mesh m_SnowMesh;
            public Mesh m_WaterMesh;

            public GameObject m_ParticlesItemCooking;
            public GameObject m_ParticlesItemReady;
            public GameObject m_ParticlesItemRuined;
            public GameObject m_ParticlesSnowMelting;
            public GameObject m_ParticlesWaterBoiling;
            public GameObject m_ParticlesWaterReady;
            public GameObject m_ParticlesWaterRuined;

            public uint m_CookingAudioId = 0U;

            public CookSettings m_CookSettings;
            public Il2CppAK.Wwise.Event m_CookAudio;

            public float m_LastBeingCookedTime = 0;
            public string m_LastCookingResult = "";
            public bool m_IsCookPot = false;
            public CookingPotItem.GrubMeshType m_Style = CookingPotItem.GrubMeshType.Pot;

            public void RelinkCookingSlot()
            {
                if (m_CookingSlotIndex == -1 || string.IsNullOrEmpty(m_FireGUID))
                {
                    if (m_CookingSlot)
                    {
                        CookingSlotVisual VisualHook = m_CookingSlot.gameObject.GetComponent<CookingSlotVisual>();

                        if (VisualHook)
                        {
                            if (VisualHook.m_Gear == this)
                            {
                                VisualHook.m_Gear = null;
                            }
                        }
                    }

                    m_CookingSlot = null;
                }
                else
                {
                    m_CookingSlot = FireHook.GetCookingSlotByIndex(m_FireGUID, m_CookingSlotIndex);

                    if (m_CookingSlot)
                    {
                        CookingSlotVisual VisualHook = m_CookingSlot.gameObject.GetComponent<CookingSlotVisual>();

                        if (VisualHook)
                        {
                            VisualHook.m_Gear = m_Gear;
                        }
                    }
                }
            }

            public bool IsCooking()
            {
                if (m_CookingSlot)
                {
                    FireplaceInteraction FirePlace = m_CookingSlot.GetFireplaceHost();

                    if(FirePlace && FirePlace.Fire)
                    {
                        return FirePlace.Fire.GetFireState() == FireState.FullBurn;
                    }
                }

                return false;
            }

            public void Override(CookingPotItem CookPot)
            {
                if (CookPot)
                {
                    m_IsCookPot = true;
                    m_Style = CookPot.m_GrubMeshType;


                    m_GrubMeshRenderer = CookPot.m_GrubMeshRenderer;
                    m_GrubMeshFilter = CookPot.m_GrubMeshFilter;

                    m_MeltSnowMaterialsList = CookPot.m_MeltSnowMaterialsList;
                    m_BoilWaterPotMaterialsList = CookPot.m_BoilWaterPotMaterialsList;
                    m_BoilWaterReadyMaterialsList = CookPot.m_BoilWaterReadyMaterialsList;

                    m_SnowMesh = CookPot.m_SnowMesh;
                    m_WaterMesh = CookPot.m_WaterMesh;

                    m_ParticlesItemCooking = CookPot.m_ParticlesItemCooking;
                    m_ParticlesItemReady = CookPot.m_ParticlesItemReady;
                    m_ParticlesItemRuined = CookPot.m_ParticlesItemRuined;
                    m_ParticlesSnowMelting = CookPot.m_ParticlesSnowMelting;
                    m_ParticlesWaterBoiling = CookPot.m_ParticlesWaterBoiling;
                    m_ParticlesWaterReady = CookPot.m_ParticlesWaterReady;

                    m_CookSettings = CookPot.m_CookSettings;
                    SetupGrubMesh(GetState());
                }
            }

            public void Override(Cookable Cookable, float Caloreis = 0)
            {
                if (Cookable)
                {
                    if(m_CookingResult != "Warming")
                    {
                        MeshSwapItem Swap = Cookable.gameObject.GetComponent<MeshSwapItem>();

                        if (Swap)
                        {
                            m_RawObject = Swap.m_MeshObjUnopened;
                            m_CookingReadyObject = Swap.m_MeshObjCookingReady;
                        }
                    }

                    if (Cookable.m_CookedPrefab)
                    {
                        m_LocalizedCookedName = Cookable.m_CookedPrefab.DisplayName;
                    }

                    GearsSync.GetCookngTime(Cookable.GetComponent<GearItem>(), Caloreis, out m_CookingTime, out m_BurningTime);

                    m_CookAudio = Cookable.m_CookEvent;

                    m_RuinedMaterials = Cookable.m_RuinedMaterials;
                    SetupGrubMesh(GetState());

                    GameObject Reference = AssetManager.GetAssetFromGame<GameObject>("GEAR_CookingPotDummy");
                    if (Reference)
                    {
                        GameObject DummyObj = GameObject.Instantiate(Reference);
                        if (DummyObj)
                        {
                            CookingPotItem CookPot = DummyObj.GetComponent<CookingPotItem>();

                            if (CookPot)
                            {
                                m_ParticlesItemCooking = CookPot.m_ParticlesItemCooking;
                                m_ParticlesItemReady = CookPot.m_ParticlesItemReady;
                                m_ParticlesItemRuined = CookPot.m_ParticlesItemRuined;
                                m_ParticlesSnowMelting = CookPot.m_ParticlesSnowMelting;
                                m_ParticlesWaterBoiling = CookPot.m_ParticlesWaterBoiling;
                                m_ParticlesWaterReady = CookPot.m_ParticlesWaterReady;

                                if (m_ParticlesItemCooking)
                                {
                                    m_ParticlesItemCooking.transform.SetParent(transform);
                                    m_ParticlesItemCooking.transform.localPosition = Vector3.zero;
                                }
                                if (m_ParticlesItemReady)
                                {
                                    m_ParticlesItemReady.transform.SetParent(transform);
                                    m_ParticlesItemReady.transform.localPosition = Vector3.zero;
                                }
                                if (m_ParticlesItemRuined)
                                {
                                    m_ParticlesItemRuined.transform.SetParent(transform);
                                    m_ParticlesItemRuined.transform.localPosition = Vector3.zero;
                                }
                                if (m_ParticlesSnowMelting)
                                {
                                    m_ParticlesSnowMelting.transform.SetParent(transform);
                                    m_ParticlesSnowMelting.transform.localPosition = Vector3.zero;
                                }
                                if (m_ParticlesWaterBoiling)
                                {
                                    m_ParticlesWaterBoiling.transform.SetParent(transform);
                                    m_ParticlesWaterBoiling.transform.localPosition = Vector3.zero;
                                }
                                if (m_ParticlesWaterReady)
                                {
                                    m_ParticlesWaterReady.transform.SetParent(transform);
                                    m_ParticlesWaterReady.transform.localPosition = Vector3.zero;
                                }
                            }
                            UnityEngine.Object.Destroy(DummyObj);
                        }
                    }
                }
            }

            public void SetupGrubMesh(GearsSync.CookedState State)
            {
                if (string.IsNullOrEmpty(m_CookingResult))
                {
                    if (m_GrubMeshRenderer)
                    {
                        m_GrubMeshRenderer.gameObject.SetActive(false);
                        return;
                    }
                }
                
                if (!string.IsNullOrEmpty(m_CookingResult) && m_CookingResult != "Warming")
                {
                    if (m_CookingResult == "BadWater" || m_CookingResult == "GoodWater")
                    {
                        m_CookAudio = m_CookSettings.m_MeltAndBoilAudio;

                        if (m_CookingResult == "BadWater")
                        {
                            if (State == GearsSync.CookedState.Overcooked)
                            {
                                m_GrubMeshRenderer.gameObject.SetActive(false);
                                return;
                            }

                            m_GrubMeshRenderer.sharedMaterials = m_MeltSnowMaterialsList;
                            m_GrubMeshFilter.sharedMesh = m_SnowMesh;

                            m_GrubMeshRenderer.gameObject.SetActive(true);

                            m_CookingTime = (m_CookSettings.m_MinutesToMeltSnowPerLiter * m_Volume) / 60;
                            m_BurningTime = 0;
                        }
                        else if (m_CookingResult == "GoodWater")
                        {
                            if (State == GearsSync.CookedState.Overcooked)
                            {
                                m_GrubMeshRenderer.gameObject.SetActive(false);
                                return;
                            }

                            if (State == GearsSync.CookedState.Cooked)
                            {
                                m_GrubMeshRenderer.sharedMaterials = m_BoilWaterReadyMaterialsList;
                                m_GrubMeshFilter.sharedMesh = m_WaterMesh;
                            }
                            else
                            {
                                m_GrubMeshRenderer.sharedMaterials = m_BoilWaterPotMaterialsList;
                                m_GrubMeshFilter.sharedMesh = m_WaterMesh;
                            }

                            m_GrubMeshRenderer.gameObject.SetActive(true);

                            m_CookingTime = (m_CookSettings.m_MinutesToBoilWaterPerLiter * m_Volume) / 60;
                            m_BurningTime = m_CookingTime;
                        }
                    }
                    else
                    {
                        if (m_IsCookPot)
                        {
                            if(m_LastCookingResult != m_CookingResult)
                            {
                                m_LastCookingResult = m_CookingResult;

                                GameObject Reference = AssetManager.GetAssetFromGame<GameObject>(m_CookingResult);

                                if (Reference)
                                {
                                    GearItem Gear = Reference.GetComponent<GearItem>();
                                    // Достаём именно так. А не Gear.m_Cookable и т.п. потому что не у всех префабов как оказалось эти переенные назачены в эдиторе
                                    Cookable Cookable = Reference.GetComponent<Cookable>();
                                    FoodItem FoodItem = Reference.GetComponent<FoodItem>();
                                    FoodWeight FoodWeight = Reference.GetComponent<FoodWeight>();

                                    if (Gear)
                                    {
                                        m_LocalizedOverrideName = Gear.DisplayName;
                                    }
                                    GearsSync.GetCookngTime(Cookable, FoodItem, FoodWeight, m_Volume, out m_CookingTime, out m_BurningTime);

                                    if (Cookable)
                                    {
                                        m_RuinedMaterials = Cookable.m_RuinedMaterials;
                                        m_CookingPotRawMaterialsList = Cookable.m_CookingPotRawMaterialsList;
                                        m_CookingPotMaterialsList = Cookable.m_CookingPotMaterialsList;
                                        m_CookAudio = Cookable.m_CookEvent;

                                        if (Cookable.m_CookedPrefab)
                                        {
                                            m_LocalizedCookedName = Cookable.m_CookedPrefab.DisplayName;
                                        }
                                        else
                                        {
                                            m_LocalizedCookedName = string.Empty;
                                        }
                                        switch (m_Style)
                                        {
                                            case CookingPotItem.GrubMeshType.Pot:
                                                m_RawMesh = Cookable.m_MeshRawPotStyle;
                                                m_ReadyMesh = Cookable.m_MeshPotStyle;
                                                break;
                                            case CookingPotItem.GrubMeshType.Can:
                                                m_RawMesh = Cookable.m_MeshRawCanStyle;
                                                m_ReadyMesh = Cookable.m_MeshCanStyle;
                                                break;
                                            case CookingPotItem.GrubMeshType.FryingPan:
                                                m_RawMesh = Cookable.m_MeshRawFryingPanStyle;
                                                m_ReadyMesh = Cookable.m_MeshFryingPanStyle;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }

                            if (m_GrubMeshRenderer)
                            {
                                m_GrubMeshRenderer.gameObject.SetActive(true);

                                if (State == GearsSync.CookedState.Overcooked)
                                {
                                    m_GrubMeshRenderer.sharedMaterials = m_RuinedMaterials;
                                }
                                else
                                {
                                    m_GrubMeshRenderer.sharedMaterials = m_CookingPotMaterialsList;
                                }
                            }
                            if (m_GrubMeshFilter)
                            {
                                if(State == GearsSync.CookedState.Cooked)
                                {
                                    if (m_ReadyMesh)
                                    {
                                        m_GrubMeshFilter.sharedMesh = m_ReadyMesh;
                                    }
                                    else
                                    {
                                        m_GrubMeshFilter.sharedMesh = m_RawMesh;
                                    }
                                }
                                else
                                {
                                    if(m_RawMesh)
                                    {
                                        m_GrubMeshFilter.sharedMesh = m_RawMesh;
                                    }
                                    else
                                    {
                                        m_GrubMeshFilter.sharedMesh = m_ReadyMesh;
                                    }
                                }
                            }
                        }
                        
                        
                        if (m_CookingReadyObject)
                        {
                            if (State == GearsSync.CookedState.Overcooked)
                            {
                                MeshRenderer Renderer = m_CookingReadyObject.GetComponent<MeshRenderer>();
                                if (Renderer)
                                {
                                    Renderer.sharedMaterials = m_RuinedMaterials;
                                }
                            }

                            if (State == GearsSync.CookedState.Cooked)
                            {
                                m_CookingReadyObject.SetActive(true);
                                m_RawObject.SetActive(false);

                            }
                            else
                            {
                                m_CookingReadyObject.SetActive(false);
                                m_RawObject.SetActive(true);
                            }
                        }
                        else
                        {
                            if (m_RawObject)
                            {
                                m_RawObject.SetActive(true);
                                if (State == GearsSync.CookedState.Overcooked)
                                {
                                    MeshRenderer Renderer = m_RawObject.GetComponent<MeshRenderer>();
                                    if (Renderer)
                                    {
                                        Renderer.sharedMaterials = m_RuinedMaterials;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public void OnDestroy()
            {
                if (m_CookingAudioId != 0U)
                {
                    AkSoundEngine.StopPlayingID(m_CookingAudioId, 10);
                    m_CookingAudioId = 0U;
                }
            }

            public float GetHours()
            {
                GearsSync.CookedState State = GetState();

                switch (State)
                {
                    case GearsSync.CookedState.Raw:
                        return m_CookingTime * (1-(m_BeingCookedTime / m_CookingTime));
                    case GearsSync.CookedState.Cooked:
                        if(m_BurningTime > 0)
                        {
                            float Overcooked = m_BeingCookedTime - m_CookingTime;
                            return m_BurningTime * (1-(Overcooked / m_BurningTime));
                        }
                        return 0;
                    case GearsSync.CookedState.Overcooked:
                        return 0;
                    default:
                        return 0;
                }
            }

            public GearsSync.CookedState GetState()
            {
                if(m_BeingCookedTime < m_CookingTime)
                {
                    return GearsSync.CookedState.Raw;
                }
                else
                {
                    if(m_BurningTime > 0)
                    {
                        if(m_BeingCookedTime > m_CookingTime + m_BurningTime)
                        {
                            return GearsSync.CookedState.Overcooked;
                        }
                        else
                        {
                            return GearsSync.CookedState.Cooked;
                        }
                    }
                    else
                    {
                        return GearsSync.CookedState.Cooked;
                    }
                }
            }

            public void UpdateParticles(GearsSync.CookedState State)
            {
                GameObject particlesToTurnOn = null;

                if (IsCooking() && !string.IsNullOrEmpty(m_CookingResult) && State != GearsSync.CookedState.Overcooked)
                {
                    if (m_CookingResult == "BadWater")
                    {
                        if (State == GearsSync.CookedState.Raw)
                        {
                            particlesToTurnOn = m_ParticlesSnowMelting;
                        }
                        else
                        {
                            particlesToTurnOn = m_ParticlesWaterReady;
                        }
                    }
                    else if (m_CookingResult == "GoodWater")
                    {
                        if (State == GearsSync.CookedState.Raw)
                        {
                            particlesToTurnOn = m_ParticlesWaterBoiling;
                        }
                        else
                        {
                            particlesToTurnOn = m_ParticlesWaterReady;
                        }
                    }
                    else
                    {
                        if (State == GearsSync.CookedState.Raw)
                        {
                            particlesToTurnOn = m_ParticlesItemCooking;
                        }
                        else
                        {
                            particlesToTurnOn = m_ParticlesItemReady;
                        }
                    }
                }

                if (m_ParticlesItemCooking)
                {
                    Utils.SetActive(m_ParticlesItemCooking, m_ParticlesItemCooking == particlesToTurnOn);
                }
                if (m_ParticlesItemReady)
                {
                    Utils.SetActive(m_ParticlesItemReady, m_ParticlesItemReady == particlesToTurnOn);
                }
                if (m_ParticlesItemRuined)
                {
                    Utils.SetActive(m_ParticlesItemRuined, m_ParticlesItemRuined == particlesToTurnOn);
                }
                if (m_ParticlesSnowMelting)
                {
                    Utils.SetActive(m_ParticlesSnowMelting, m_ParticlesSnowMelting == particlesToTurnOn);
                }
                if (m_ParticlesWaterBoiling)
                {
                    Utils.SetActive(m_ParticlesWaterBoiling, m_ParticlesWaterBoiling == particlesToTurnOn);
                }
                if (m_ParticlesWaterReady)
                {
                    Utils.SetActive(m_ParticlesWaterReady, m_ParticlesWaterReady == particlesToTurnOn);
                }
                if (m_ParticlesWaterRuined)
                {
                    Utils.SetActive(m_ParticlesWaterRuined, m_ParticlesWaterRuined == particlesToTurnOn);
                }
            }

            public void UpdateAudio(GearsSync.CookedState State)
            {
                if (!string.IsNullOrEmpty(m_CookingResult) && IsCooking() && State != GearsSync.CookedState.Overcooked)
                {
                    if (m_CookingAudioId == 0U)
                    {
                        if (m_CookAudio != null)
                        {
                            m_CookingAudioId = GameAudioManager.Play3DSound(m_CookAudio, gameObject);
                        }
                    }

                    float PercentCooked = 0;
                    float PercentRuined = 0;

                    if(State == GearsSync.CookedState.Raw)
                    {
                        PercentCooked = m_BeingCookedTime / m_CookingTime;
                    }else if(State == GearsSync.CookedState.Cooked)
                    {
                        PercentCooked = 1f;

                        float Overcooked = m_BeingCookedTime - m_CookingTime;
                        PercentRuined = Overcooked / m_BurningTime;
                    }
                    
                    float rtpcValue = PercentCooked * 100f + PercentRuined * 100f;

                    if (m_CookingResult == "BadWater")
                    {
                        rtpcValue = PercentCooked * 100f * 0.5f;
                    }
                    else if (m_CookingResult == "GoodWater")
                    {
                        rtpcValue = 50f + PercentCooked * 100f * 0.5f + PercentRuined * 100f;
                    }
                    GameAudioManager.SetRTPCValue(GAME_PARAMETERS.COOKINGSTATE, rtpcValue, gameObject);
                }
                else
                {
                    if (m_CookingAudioId != 0U)
                    {
                        AkSoundEngine.StopPlayingID(m_CookingAudioId, 10);
                        m_CookingAudioId = 0U;
                    }
                }
            }

            public void Update()
            {
                GearsSync.CookedState State = GetState();
                UpdateAudio(State);
                UpdateParticles(State);

                if(m_LastBeingCookedTime != m_BeingCookedTime)
                {
                    m_LastBeingCookedTime = m_BeingCookedTime;

                    SetupGrubMesh(State);
                }
                if(m_LastCookingResult != m_CookingResult)
                {
                    SetupGrubMesh(State);
                }
            }
        }

        public class GearCookingTarget : MonoBehaviour
        {
            public GearCookingTarget(IntPtr ptr) : base(ptr) { }
            public string m_FireGUID = string.Empty;
            public int m_CookingIndex = 0;
            public GearPlacePoint m_PlacePoint;
            public string m_CookingResult = string.Empty;
            public float m_Volume = 0;
            public float m_TimeBeingCooked = 0;
            public string m_CookpotGUID = string.Empty;
        }

        public class GearCookingDummy : MonoBehaviour
        {
            public GearCookingDummy(IntPtr ptr) : base(ptr) { }
            public string m_RealGearGUID = "";
        }

        public class CookingSlotVisual : MonoBehaviour
        {
            public CookingSlotVisual(IntPtr ptr) : base(ptr) { }

            public DroppedGearVisual m_Gear;
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
                            if(ModMain.Client != null && ModMain.Client.m_Rules.m_CanDropItems)
                            {
                                GearsSync.SendDropItem(GetComponent<GearItem>(), 0, 0, true);
                            }
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
            public float m_Tilt = 0;
            public Vector2 m_TiltLimits = new Vector2(float.NegativeInfinity, float.PositiveInfinity);
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
            public AudioSource m_AudioSourceBloodDrop;
            public List<Collider> m_PlayerColiders = new List<Collider>();
            public GameObject m_Helmet = null;
            public GameObject m_Satchel = null;
            public GameObject m_TechnicalBackpack = null;
            public GameObject m_Vest = null;
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
            public Transform m_TiltTarget = null;

            public float m_BaseFootstepsInterval = 0.5f;
            public float m_BaseMovementSpeed = 6;

            public int m_BloodLosses = 0;
            public float m_NextBloodDrop = 0;

            private bool m_LeftFoot = true;
            private float s_NextFoostep = 0;

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
                Sleep = 7,
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
            }

            public void SetPosition(Vector3 position)
            {
                m_Position = position;
            }

            public void SetRotation(Quaternion rotation)
            {
                m_Rotation = rotation;
            }

            public void SetTilt(float tilt)
            {
                m_Tilt = tilt;
            }

            public void SetVisibile(bool Visible)
            {
                gameObject.SetActive(Visible);
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
                if (m_Vest)
                {
                    m_Vest.SetActive(m_VisualData.m_ClothingData.m_Accs1 == "GEAR_BallisticVest" || m_VisualData.m_ClothingData.m_Accs2 == "GEAR_BallisticVest");
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
                m_Vest = AddVest(new Vector3(0, 0, -0.28f), new Vector3(90, 0, 0), new Vector3(1, 9, 1));

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
                m_AudioSourceBloodDrop = gameObject.transform.FindChild("3DBloodDrop").GetComponent<AudioSource>();
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

            public GameObject AddVest(Vector3 Position, Vector3 Rotation, Vector3 Scale)
            {
                Transform Chest = GetBone(m_Animator, HumanBodyBones.Chest);
                GameObject Gear = AssetManager.CreateBogusGear("GEAR_BallisticVest");
                if (Gear)
                {
                    Rigidbody B = Gear.GetComponent<Rigidbody>();
                    if (B)
                    {
                        UnityEngine.Object.Destroy(B);
                    }
                    BoxCollider Box = Gear.GetComponent<BoxCollider>();
                    if (Box)
                    {
                        UnityEngine.Object.Destroy(Box);
                    }
                    Gear.transform.SetParent(Chest);
                    Gear.transform.localPosition = Position;
                    Gear.transform.SetLocalEulerAngles(Rotation, RotationOrder.OrderXYZ);
                    Gear.transform.localScale = Scale;
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

            public void DoFootStep()
            {
                Transform Foot = null;
                if (m_LeftFoot)
                {
                    Foot = m_Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                }
                else
                {
                    Foot = m_Animator.GetBoneTransform(HumanBodyBones.RightFoot);
                }
                PlayersManager.TryLeaveFootprint(Foot.position, transform, m_LeftFoot, true);
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

                if(Direction.magnitude > m_MinimalSpeed && m_Action != Actions.Death && m_Action != Actions.Knocked)
                {
                    if (s_NextFoostep <= Time.time)
                    {
                        DoFootStep();
                        float NextStepPeriod = m_BaseFootstepsInterval * (m_BaseMovementSpeed / Mathf.Max(Speed.magnitude, 0.1f));
                        s_NextFoostep = Time.time + NextStepPeriod;
                    }
                }

                if(m_BloodLosses > 0)
                {
                    if(m_NextBloodDrop <= Time.time)
                    {
                        float BloodDropRate = 1.6f - 0.3f * m_BloodLosses;

                        if (BloodDropRate < 0.5f)
                        {
                            BloodDropRate = 0.5f;
                        }
                        m_NextBloodDrop = Time.time + BloodDropRate;

                        if (m_AudioSourceBloodDrop)
                        {
                            m_AudioSourceBloodDrop.Play();
                            Vector3 pos = transform.position;
                            ++pos.y;
                            Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                            insideUnitCircle.Normalize();
                            Vector2 vector2 = insideUnitCircle * UnityEngine.Random.Range(0.0f, 0.75f);
                            pos.x += vector2.x;
                            pos.z += vector2.y;
                            pos -= transform.forward * 0.5f;
                            RaycastHit hitInfo;
                            if (Physics.Raycast(pos, Vector3.down, out hitInfo, float.PositiveInfinity, Utils.m_PhysicalCollisionLayerMask) || (UnityEngine.Object)hitInfo.collider == (UnityEngine.Object)null)
                            {
                                Vector3 scale = new Vector3(0.05f, 2f, 0.05f) * UnityEngine.Random.Range(0.5f, 2f);
                                int uvRectangleIndex = 7;
                                if (Utils.RollChance(50f))
                                {
                                    uvRectangleIndex = 6;
                                }
                                GameManager.GetDynamicDecalsManager().CreateDecal(hitInfo.point, transform.rotation.eulerAngles.y, hitInfo.normal, uvRectangleIndex, scale, DecalProjectorType.PlayerBlood, GameManager.GetWeatherComponent().IsIndoorEnvironment());
                            }
                        }
                    }
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

                Vector3 Angle = new Vector3(m_Tilt, 0, 0);

                if (m_Tilt < m_TiltLimits.x)
                {
                    m_Tilt = m_TiltLimits.x;
                }
                else if (m_Tilt > m_TiltLimits.y)
                {
                    m_Tilt = m_TiltLimits.y;
                }

                if (m_Action == Actions.Knocked || m_Action == Actions.Death || m_Action == Actions.Harvesting || m_Action == Actions.Igniting || m_Action == Actions.Sleep)
                {
                    Angle.x = 0;
                }

                if (m_TiltTarget)
                {
                    m_TiltTarget.SetLocalEulerAngles(Angle, RotationOrder.OrderXYZ);
                }

                if (m_CameraAttention)
                {
                    m_CameraAttention.m_Tilt = m_Tilt;
                }
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

                if(m_PlayerID != -1) // if not Victory Dummy
                {
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
        }
        public class CameraAttention : MonoBehaviour
        {
            public CameraAttention(IntPtr ptr) : base(ptr) { }
            public Transform m_OffsetTranform;
            public float m_Tilt = 0;

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
                    Vector3 Euler = m_Camera.transform.localEulerAngles;
                    Euler.x = m_Tilt;

                    m_Camera.transform.SetLocalEulerAngles(Euler, RotationOrder.OrderXYZ);
                }
            }

            void OnDestroy()
            {
                GameManager.GetPlayerAnimationComponent().ShowPlayer(true);

                if (m_Camera)
                {
                    m_Camera.enabled = true;
                }
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

        public class DangerCircleZone : MonoBehaviour
        {
            public DangerCircleZone(IntPtr ptr) : base(ptr) { }
            public float m_Smoother = 8;
            public DataStr.DangerCircleShrinkStateData m_Data;


            public Vector3 GetScale()
            {
                float Radius = m_Data.GetCurrentRadius();

                return new Vector3(Radius, 4300, Radius);
            }

            void Update()
            {
                transform.localScale = Vector3.Lerp(transform.localScale, GetScale(), m_Smoother * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, m_Data.GetCenter().ConvertToUnity(), m_Smoother * Time.deltaTime);
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

        public class TeammateMapIcon : MonoBehaviour
        {
            public TeammateMapIcon(IntPtr ptr) : base(ptr) { }

            public int m_IndexHandler = 0;
            public Panel_Map m_Panel;
            public UISprite m_Sprite;

            void Update()
            {
                if (m_Panel)
                {
                    NetworkPlayer Player = PlayersManager.GetPlayer(m_IndexHandler);

                    if(!m_Panel.IsWorldMapActive() && m_Panel.m_RegionSelectedIndex == m_Panel.GetIndexOfCurrentScene())
                    {
                        if (Player)
                        {
                            m_Sprite.enabled = Player.m_Action != NetworkPlayer.Actions.Death && Player.gameObject.activeSelf && (SquadHUD.IsTeammate(m_IndexHandler) || PlayersManager.s_Spectator);
                        }
                        else
                        {
                            m_Sprite.enabled = false;
                        }
                    }
                    else
                    {
                        m_Sprite.enabled = false;
                    }

                    if (m_Sprite.enabled)
                    {
                        if (m_IndexHandler == PlayersManager.s_SpectateID && PlayersManager.s_Spectator)
                        {
                            m_Sprite.color = Color.cyan;
                        }
                        else if(SquadHUD.IsTeammate(m_IndexHandler))
                        {
                            m_Sprite.color = Color.green;
                        }
                        else
                        {
                            m_Sprite.color = Color.white;
                        }
                    }
                    
                    transform.localPosition = m_Panel.WorldPositionToMapPosition(m_Panel.m_UnlockedRegionNames[m_Panel.m_RegionSelectedIndex], Player.m_Position);
                    transform.localRotation = m_Panel.WorldRotationToMapRotation(m_Panel.m_UnlockedRegionNames[m_Panel.m_RegionSelectedIndex], Player.m_Rotation);
                }
            }
        }

        public class ZoneMapIcon : MonoBehaviour
        {
            public ZoneMapIcon(IntPtr ptr) : base(ptr) { }

            public Panel_Map m_Panel;
            public UISprite m_Sprite;
            public bool m_IsNextZone = false;
            public Vector2 m_RefScale = new Vector2(0.018f, 0.018f);

            // MarshRegion 0.018f x 0.018f
            // WhalingStationRegion 0.0227 x 0.018f
            // CoastalRegion 0.014 x 0.014
            // RuralRegion 0.012 x 0.012

            void Update()
            {
                if (m_Panel)
                {
                    float realRadiusInMeters = 0;
                    Vector3 Position = Vector3.zero;

                    if (!m_Panel.IsWorldMapActive() && m_Panel.m_RegionSelectedIndex == m_Panel.GetIndexOfCurrentScene() && DangerCircleManager.s_DangerCircle)
                    {
                        m_RefScale = DangerCircleManager.s_MapRefScale;
                        if (!m_IsNextZone)
                        {
                            realRadiusInMeters = DangerCircleManager.s_DangerCircle.m_Data.GetCurrentRadius();
                            Position = DangerCircleManager.s_DangerCircle.m_Data.GetCenter().ConvertToUnity();
                            if (DangerCircleManager.s_DangerCircle)
                            {
                                m_Sprite.enabled = true;
                            }
                            else
                            {
                                m_Sprite.enabled = false;
                            }
                        }
                        else
                        {
                            realRadiusInMeters = DangerCircleManager.s_NextZoneRadius;
                            Position = DangerCircleManager.s_NextZoneCenter;
                            if (DangerCircleManager.s_NextZoneRadius == 0 || DangerCircleManager.s_NextZoneCenter == Vector3.zero)
                            {
                                m_Sprite.enabled = false;
                            }
                            else
                            {
                                m_Sprite.enabled = true;
                            }
                        }
                    }
                    else
                    {
                        m_Sprite.enabled = false;
                    }
                    
                    transform.localPosition = m_Panel.WorldPositionToMapPosition(m_Panel.m_UnlockedRegionNames[m_Panel.m_RegionSelectedIndex], Position);

                    string regionName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

                    // x 0.018 y 0.018 карты = реальному соотвествует 1 метру когда m_WorldRadius x 1153.3 y 1159.6

                    Vector2 baseMetersPerUnit = new Vector2(
                        1f / m_RefScale.x, 
                        1f / m_RefScale.y
                    );

                    Vector2 realWorldToMapScale = new Vector2(
                        realRadiusInMeters / baseMetersPerUnit.x,
                        realRadiusInMeters / baseMetersPerUnit.y
                    );

                    transform.localScale = new Vector3(realWorldToMapScale.x, realWorldToMapScale.y, 1);
                }
            }
        }

        public class GenericStatusBarSpawnerHook : MonoBehaviour
        {
            public GenericStatusBarSpawnerHook(IntPtr ptr) : base(ptr) { }

            GenericStatusBarSpawner s_Bar;


            void AddTeamBars()
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
                    for (int i = 1; i <= PlayersDataManager.c_SquadLimit; i++)
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
                    if(s_Bar.m_StatusBarType == StatusBar.StatusBarType.Condition)
                    {
                        AddTeamBars();
                    }
                }
            }

            void Update()
            {
                if (s_Bar)
                {
                    if (s_Bar.m_SpawnedObject)
                    {
                        if (ModMain.IsMultiplayer())
                        {
                            s_Bar.m_SpawnedObject.SetActive(s_Bar.m_StatusBarType == StatusBar.StatusBarType.Condition || s_Bar.m_StatusBarType == StatusBar.StatusBarType.Fatigue);
                        }
                        else
                        {
                            s_Bar.m_SpawnedObject.SetActive(true);
                        }
                    }
                }
            }
        }
        public class SendGearIfNotDestoryed : MonoBehaviour
        {
            public SendGearIfNotDestoryed(IntPtr ptr) : base(ptr) { }
            public GearItem m_Gear = null;
            public bool m_SkipThisFrame = true;
            public bool m_CancleSending = false;
            void Update()
            {
                if (m_CancleSending)
                {
                    return;
                }
                
                if (m_SkipThisFrame)
                {
                    m_SkipThisFrame = false;
                    return;
                }
                
                if (m_Gear)
                {
                    m_CancleSending = true;
                    SkyCoop.Logger.Log($"Gear {m_Gear.name} refused");

                    if (GearsSync.s_LastCookingSlotGearPickedFrom)
                    {
                        CookingSlotVisual CookingSlot = GearsSync.s_LastCookingSlotGearPickedFrom.GetComponent<CookingSlotVisual>();
                        if (CookingSlot && CookingSlot.m_Gear == null)
                        {
                            FireHook.DoCookingAction("DoFirePickerAction", m_Gear, GearsSync.s_LastCookingSlotGearPickedFrom.gameObject, GearsSync.s_LastGearTimeBeingCooked);
                        }
                        else
                        {
                            GearsSync.SendDropItem(m_Gear, 0, 0, false);
                        }
                    }
                    else
                    {
                        GearsSync.SendDropItem(m_Gear, 0, 0, true);
                    }
                    GearsSync.s_LastPickedGearGUID = string.Empty;
                }
            }
        }
        public class PropMovemenetPredict : MonoBehaviour
        {
            public PropMovemenetPredict(IntPtr ptr) : base(ptr) { }
            public Vector3 m_Destination = Vector3.zero;
            public Vector3 VelocityPerSecond = Vector3.zero;

            void Update()
            {
                float distanceToDestination = Vector3.Distance(transform.position, m_Destination);
                float maxMoveDistance = VelocityPerSecond.magnitude * Time.deltaTime;

                if (distanceToDestination <= maxMoveDistance)
                {
                    transform.position = m_Destination;
                    enabled = false;
                }
                else
                {
                    Vector3 direction = (m_Destination - transform.position).normalized;
                    transform.position += direction * maxMoveDistance;
                }
            }
        }
        public class ChatMessage : MonoBehaviour
        {
            public ChatMessage(IntPtr ptr) : base(ptr) { }
            public CanvasGroup m_Group;
            public float m_VisibleTimer = 0;

            void Update()
            {
                if (m_Group)
                {
                    if (CanvasUI.m_ChatIsOpen)
                    {
                        m_Group.alpha = 1;
                    }
                    else
                    {
                        if (m_VisibleTimer == 0)
                        {
                            m_Group.alpha = Mathf.Lerp(m_Group.alpha, 0, Time.deltaTime * 8);
                        }
                        else
                        {
                            m_Group.alpha = 1;
                        }
                    }
                }
                if(m_VisibleTimer > 0)
                {
                    if (!CanvasUI.m_ChatIsOpen)
                    {
                        m_VisibleTimer -= Time.deltaTime;

                        if (m_VisibleTimer <= 0)
                        {
                            m_VisibleTimer = 0;
                        }
                    }
                }
            }
        }
    }
}
