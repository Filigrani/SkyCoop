using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using GameServer;
using static SkyCoop.DataStr;
using System.Globalization;
using System.Security.Policy;
#if (DEDICATED)
using System.Numerics;
using TinyJSON;
#else
using MelonLoader.TinyJSON;
using UnityEngine;
#endif


namespace SkyCoop
{
    public class MPSaveManager
    {
        public static bool NoSaveAndLoad = false;
        public static void Log(string LOG)
        {
#if (DEDICATED)
            Logger.Log("[MPSaveManager] " +LOG, Shared.LoggerColor.Blue);
#else
            MelonLoader.MelonLogger.Msg(ConsoleColor.Blue, "[MPSaveManager] " + LOG);
            #endif
        }
        public static void Error(string LOG)
        {
#if (DEDICATED)
            Logger.Log("[MPSaveManager] " +LOG, Shared.LoggerColor.Red);
#else
            MelonLoader.MelonLogger.Msg(ConsoleColor.Red, "[MPSaveManager] " + LOG);
            #endif
        }
        public static int GetSeed()
        {
#if (DEDICATED)
            return Seed;
#else
            return GameManager.m_SceneTransitionData.m_GameRandomSeed;
#endif

        }
        public static Dictionary<string, Dictionary<int, DataStr.DroppedGearItemDataPacket>> RecentVisual = new Dictionary<string, Dictionary<int, DataStr.DroppedGearItemDataPacket>>();
        public static Dictionary<string, Dictionary<int, DataStr.SlicedJsonDroppedGear>> RecentData = new Dictionary<string, Dictionary<int, DataStr.SlicedJsonDroppedGear>>();
        public static Dictionary<string, Dictionary<string, bool>> RecentOpenableThings = new Dictionary<string, Dictionary<string, bool>>();
        public static Dictionary<string, long> UsersSaveHashs = new Dictionary<string, long>();
        public static Dictionary<string, Dictionary<string, string>> LockedDoors = new Dictionary<string, Dictionary<string, string>>();
        public static Dictionary<string, bool> UsedKeys = new Dictionary<string, bool>();
        public static Dictionary<int, LocksmithBlank> Blanks = new Dictionary<int, LocksmithBlank>();
        public static bool SaveHashsChanged = false;
        public static bool LockedDoorsChanged = false;
        public static bool AnimalsKilledChanged = false;
        public static int SaveRecentTimer = 5;
        public static bool Diagnostic = false;
        public static bool SaveLogging = false;
        public static int Seed = 0;
        public static string OldIdsJson = "{\"0\":\"gear_accelerant\",\"1\":\"gear_accelerantgunpowder\",\"2\":\"gear_accelerantkerosene\",\"3\":\"gear_accelerantmedium\",\"4\":\"gear_airlinefoodchick\",\"5\":\"gear_airlinefoodveg\",\"6\":\"gear_arrow\",\"7\":\"gear_arrowhead\",\"8\":\"gear_arrowshaft\",\"9\":\"gear_astridbackpack_hangar\",\"10\":\"gear_astridboots\",\"11\":\"gear_astridgloves\",\"12\":\"gear_astridjacket\",\"13\":\"gear_astridjeans\",\"14\":\"gear_astridsweater\",\"15\":\"gear_astridtoque\",\"16\":\"gear_aurorahatchcode\",\"17\":\"gear_auroraobservationnote\",\"18\":\"gear_backernote1a\",\"19\":\"gear_backernote1b\",\"20\":\"gear_backernote1c\",\"21\":\"gear_backernote2a\",\"22\":\"gear_backernote2b\",\"23\":\"gear_backernote2c\",\"24\":\"gear_backernote3a\",\"25\":\"gear_backernote3b\",\"26\":\"gear_backernote3c\",\"27\":\"gear_backernote4a\",\"28\":\"gear_backernote4b\",\"29\":\"gear_backernote4c\",\"30\":\"gear_balaclava\",\"31\":\"gear_ballisticvest\",\"32\":\"gear_bankmanagerhousekey\",\"33\":\"gear_bankmanagerhousekey\",\"34\":\"gear_bankvaultcode\",\"35\":\"gear_barktinder\",\"36\":\"gear_baseballcap\",\"37\":\"gear_basicboots\",\"38\":\"gear_basicgloves\",\"39\":\"gear_basicshoes\",\"40\":\"gear_basicwintercoat\",\"41\":\"gear_basicwoolhat\",\"42\":\"gear_basicwoolscarf\",\"43\":\"gear_bear_spear_broken\",\"44\":\"gear_bear_spear_complete\",\"45\":\"gear_bear_spear_head\",\"46\":\"gear_bearear\",\"47\":\"gear_bearhide\",\"48\":\"gear_bearhidedried\",\"49\":\"gear_bearquarter\",\"50\":\"gear_bearskinbedroll\",\"51\":\"gear_bearskincoat\",\"52\":\"gear_bearspear\",\"53\":\"gear_bearspearbroken\",\"54\":\"gear_bearspearbrokenstory\",\"55\":\"gear_bearspearrelic\",\"56\":\"gear_bearspearstory\",\"57\":\"gear_bedroll\",\"58\":\"gear_beefjerky\",\"59\":\"gear_birchbarkprepared\",\"60\":\"gear_birchbarktea\",\"61\":\"gear_birchsapling\",\"62\":\"gear_birchsaplingdried\",\"63\":\"gear_blackrockadminnote\",\"64\":\"gear_blackrockammoroomnote\",\"65\":\"gear_blackrockcodenote\",\"66\":\"gear_blackrockconvictnote1\",\"67\":\"gear_blackrockconvictnote2\",\"68\":\"gear_blackrockconvictnote3\",\"69\":\"gear_blackrockmemo\",\"70\":\"gear_blackrocksecuritynote\",\"71\":\"gear_blackrocktowernote\",\"72\":\"gear_blueflare\",\"73\":\"gear_body_dummy\",\"74\":\"gear_boltcutters\",\"75\":\"gear_booka\",\"76\":\"gear_bookarchery\",\"77\":\"gear_bookb\",\"78\":\"gear_bookbopen\",\"79\":\"gear_bookc\",\"80\":\"gear_bookcarcassharvesting\",\"81\":\"gear_bookcooking\",\"82\":\"gear_bookd\",\"83\":\"gear_booke\",\"84\":\"gear_bookeopen\",\"85\":\"gear_bookf\",\"86\":\"gear_bookfirestarting\",\"87\":\"gear_bookfopen\",\"88\":\"gear_bookg\",\"89\":\"gear_bookgunsmithing\",\"90\":\"gear_bookh\",\"91\":\"gear_bookhopen\",\"92\":\"gear_booki\",\"93\":\"gear_bookicefishing\",\"94\":\"gear_booklabe_01\",\"95\":\"gear_booklabe_02\",\"96\":\"gear_booklabe_03\",\"97\":\"gear_booklabe_open_01\",\"98\":\"gear_bookmanual\",\"99\":\"gear_bookmending\",\"100\":\"gear_bookrevolverfirearm\",\"101\":\"gear_bookriflefirearm\",\"102\":\"gear_bookriflefirearmadvanced\",\"103\":\"gear_booktalltalesfishing\",\"104\":\"gear_booktalltalesglowcave\",\"105\":\"gear_booktalltalesstag\",\"106\":\"gear_booktalltalesyeti\",\"107\":\"gear_bottleantibiotics\",\"108\":\"gear_bottlehydrogenperoxide\",\"109\":\"gear_bottlepainkillers\",\"110\":\"gear_bow\",\"111\":\"gear_bowstring\",\"112\":\"gear_bowwood\",\"113\":\"gear_brand\",\"114\":\"gear_brokenarrow\",\"115\":\"gear_brokenrifle\",\"116\":\"gear_bullet\",\"117\":\"gear_cachenote_churchmarshdir\",\"118\":\"gear_campofficecollectible\",\"119\":\"gear_candybar\",\"120\":\"gear_cannedbeans\",\"121\":\"gear_cannedsardines\",\"122\":\"gear_cannerycodenote\",\"123\":\"gear_cannerymemo\",\"124\":\"gear_cannerysurvivalpath\",\"125\":\"gear_canopener\",\"126\":\"gear_canyonclimberscavenote\",\"127\":\"gear_canyondeadclimbernote\",\"128\":\"gear_canyonfishinghutjournal\",\"129\":\"gear_canyonminersnote\",\"130\":\"gear_carbattery\",\"131\":\"gear_cargopants\",\"132\":\"gear_cashbundle\",\"133\":\"gear_cattailplant\",\"134\":\"gear_cattailstalk\",\"135\":\"gear_cattailtinder\",\"136\":\"gear_cavecachenote\",\"137\":\"gear_charcoal\",\"138\":\"gear_churchhymn\",\"139\":\"gear_churchnoteep1\",\"140\":\"gear_climbersjournal\",\"141\":\"gear_climbingsocks\",\"142\":\"gear_cloth\",\"143\":\"gear_clothsheet\",\"144\":\"gear_coal\",\"145\":\"gear_coffeecup\",\"146\":\"gear_coffeetin\",\"147\":\"gear_combatboots\",\"148\":\"gear_combatpants\",\"149\":\"gear_compressionbandage\",\"150\":\"gear_condensedmilk\",\"151\":\"gear_condensedmilk_open\",\"152\":\"gear_convictcorpsenote\",\"153\":\"gear_cookedbigmouthbass_talltales\",\"154\":\"gear_cookedcohosalmon\",\"155\":\"gear_cookedlakewhitefish\",\"156\":\"gear_cookedmeatbear\",\"157\":\"gear_cookedmeatdeer\",\"158\":\"gear_cookedmeatmoose\",\"159\":\"gear_cookedmeatrabbit\",\"160\":\"gear_cookedmeatwolf\",\"161\":\"gear_cookedrainbowtrout\",\"162\":\"gear_cookedsmallmouthbass\",\"163\":\"gear_cookingpot\",\"164\":\"gear_cookingpotdummy\",\"165\":\"gear_cottonhoodie\",\"166\":\"gear_cottonscarf\",\"167\":\"gear_cottonshirt\",\"168\":\"gear_cottonsocks\",\"169\":\"gear_cottonsocksstart\",\"170\":\"gear_cowichansweater\",\"171\":\"gear_crackers\",\"172\":\"gear_crampons\",\"173\":\"gear_crowfeather\",\"174\":\"gear_damcollectible1\",\"175\":\"gear_damcontrolroomcodenote\",\"176\":\"gear_damelevatornotice\",\"177\":\"gear_damexitcodenote\",\"178\":\"gear_damfencekey\",\"179\":\"gear_damgarbageletter\",\"180\":\"gear_damlockerkey\",\"181\":\"gear_damofficekey\",\"182\":\"gear_damtrailerbcrewnote\",\"183\":\"gear_darkwalkerdiary1\",\"184\":\"gear_darkwalkerdiary10\",\"185\":\"gear_darkwalkerdiary11\",\"186\":\"gear_darkwalkerdiary2\",\"187\":\"gear_darkwalkerdiary3\",\"188\":\"gear_darkwalkerdiary4\",\"189\":\"gear_darkwalkerdiary5\",\"190\":\"gear_darkwalkerdiary6\",\"191\":\"gear_darkwalkerdiary7\",\"192\":\"gear_darkwalkerdiary8\",\"193\":\"gear_darkwalkerdiary9\",\"194\":\"gear_darkwalkerid\",\"195\":\"gear_deadmannote1\",\"196\":\"gear_deadmannote2\",\"197\":\"gear_deadmannote3\",\"198\":\"gear_deadmannote4\",\"199\":\"gear_deadmannote5\",\"200\":\"gear_deerskinboots\",\"201\":\"gear_deerskinpants\",\"202\":\"gear_depositboxkey_dk1\",\"203\":\"gear_depositboxkey_dk2\",\"204\":\"gear_depositboxkey_dk3\",\"205\":\"gear_depositboxkey_dk_farmer\",\"206\":\"gear_detonator\",\"207\":\"gear_dogfood\",\"208\":\"gear_dogfood_open\",\"209\":\"gear_downparka\",\"210\":\"gear_downskijacket\",\"211\":\"gear_downvest\",\"212\":\"gear_dustingsulfur\",\"213\":\"gear_earmuffs\",\"214\":\"gear_elevatorcrank\",\"215\":\"gear_elevatorparts\",\"216\":\"gear_elevatorpartsdead\",\"217\":\"gear_emergencykitnote\",\"218\":\"gear_emergencystim\",\"219\":\"gear_energybar\",\"220\":\"gear_ep3_churchartifact\",\"221\":\"gear_ep3_churchflyer\",\"222\":\"gear_ep3_churchmotelletter\",\"223\":\"gear_ep3_churchnewsclipping\",\"224\":\"gear_ep3_churchthankyouletter\",\"225\":\"gear_ep3_churchtheftreport\",\"226\":\"gear_ep3_joplinbunkernote\",\"227\":\"gear_ep3_joplinbunkernote2\",\"228\":\"gear_ep3_joplincachenote\",\"229\":\"gear_ep3_patienthistory01\",\"230\":\"gear_ep3_patienthistory02\",\"231\":\"gear_ep3_patienthistory03\",\"232\":\"gear_ep3_patienthistory04\",\"233\":\"gear_ep3_patienthistory05\",\"234\":\"gear_ep3_patienthistory06\",\"235\":\"gear_ep3foresttalkers_ftcollectible1\",\"236\":\"gear_ep3foresttalkers_ftcollectible2\",\"237\":\"gear_ep3foresttalkers_ftcollectible3\",\"238\":\"gear_ep3foresttalkers_pvcollectible1\",\"239\":\"gear_ep3foresttalkers_pvcollectible2\",\"240\":\"gear_ep3foresttalkers_pvcollectible3\",\"241\":\"gear_ep3hallflyer\",\"242\":\"gear_ep3rosary\",\"243\":\"gear_ep3tomsmap\",\"244\":\"gear_ep4_blackrockescapenote1\",\"245\":\"gear_ep4_blackrocknoisemakernote\",\"246\":\"gear_ep4_blackrockrationsnote\",\"247\":\"gear_ep4_foresttalkerplans\",\"248\":\"gear_ep4_infirmarymeds\",\"249\":\"gear_ep4_infirmarymedsnote\",\"250\":\"gear_ep4_jacecarcollectible\",\"251\":\"gear_ep4_letterforesttalker1\",\"252\":\"gear_ep4_letterforesttalker2\",\"253\":\"gear_ep4_letterforesttalker3\",\"254\":\"gear_ep4_letterforesttalkerfinal\",\"255\":\"gear_ep4_minedangernote\",\"256\":\"gear_ep4_rooftopkey\",\"257\":\"gear_ep4_substationmemo\",\"258\":\"gear_ep4convictcachenote1\",\"259\":\"gear_ep4goldnugget\",\"260\":\"gear_ep4lockerkey1\",\"261\":\"gear_ep4lockerkey2\",\"262\":\"gear_ep4lockerkey3\",\"263\":\"gear_ep4lockerkey8\",\"264\":\"gear_ep4lostworkernote\",\"265\":\"gear_ep4minegatekey\",\"266\":\"gear_ep4powerworkerid1\",\"267\":\"gear_ep4powerworkerid2\",\"268\":\"gear_ep4prisongatekey\",\"269\":\"gear_ep4prosepectornote\",\"270\":\"gear_ep4rumourlateemployee\",\"271\":\"gear_ep4rumourshatchlocation\",\"272\":\"gear_ep4rumourshermitreport\",\"273\":\"gear_ep4rumourslockboxkey\",\"274\":\"gear_ep4rumoursprisonrelease\",\"275\":\"gear_ep4rumoursramblings1\",\"276\":\"gear_ep4rumourswolfreport\",\"277\":\"gear_ep4rumourswolfstudy\",\"278\":\"gear_ep4sawfarmnote\",\"279\":\"gear_ep4steamtunnelcampnote\",\"280\":\"gear_farmerdepositboxkey\",\"281\":\"gear_fireaxe\",\"282\":\"gear_firelog\",\"283\":\"gear_firestriker\",\"284\":\"gear_fishermansweater\",\"285\":\"gear_fishingline\",\"286\":\"gear_fixedrifle\",\"287\":\"gear_flarea\",\"288\":\"gear_flaregun\",\"289\":\"gear_flaregunammosingle\",\"290\":\"gear_flaregunammosingle_survivormission\",\"291\":\"gear_flareguncase_hangar\",\"292\":\"gear_flashlight\",\"293\":\"gear_fleecemittens\",\"294\":\"gear_fleecesweater\",\"295\":\"gear_flintandsteel\",\"296\":\"gear_foodsupplies_hangar\",\"297\":\"gear_foresttalkerbloodyitem\",\"298\":\"gear_foresttalkerdamnote\",\"299\":\"gear_foresttalkerflyer\",\"300\":\"gear_foresttalkerhiddenitem\",\"301\":\"gear_foresttalkermap\",\"302\":\"gear_foresttalkerthankyou\",\"303\":\"gear_foresttalkerthankyou2\",\"304\":\"gear_forgeblueprints\",\"305\":\"gear_forgecachenote\",\"306\":\"gear_fuse_dead_prefab\",\"307\":\"gear_fuse_live_prefab\",\"308\":\"gear_gauntlets\",\"309\":\"gear_gmextrasuppliesnote\",\"310\":\"gear_granolabar\",\"311\":\"gear_greenteacup\",\"312\":\"gear_greenteapackage\",\"313\":\"gear_greymotherboots\",\"314\":\"gear_greymotherpearls\",\"315\":\"gear_greymothertrunkkey\",\"316\":\"gear_greymothertrunkkey\",\"317\":\"gear_gunpowdercan\",\"318\":\"gear_gut\",\"319\":\"gear_gutdried\",\"320\":\"gear_hacksaw\",\"321\":\"gear_hammer\",\"322\":\"gear_handheldshortwave\",\"323\":\"gear_hankhatchcode\",\"324\":\"gear_hankjournal1\",\"325\":\"gear_hankjournal2\",\"326\":\"gear_hanklockboxkey\",\"327\":\"gear_hankneiceletter\",\"328\":\"gear_hardcase\",\"329\":\"gear_hardcase_hangar\",\"330\":\"gear_hardwood\",\"331\":\"gear_hatchet\",\"332\":\"gear_hatchetimprovised\",\"333\":\"gear_hc_ep1_fm_treeroots_dir\",\"334\":\"gear_hc_ep1_ml_alanscave_dir\",\"335\":\"gear_hc_ep1_ml_clearcut_dir\",\"336\":\"gear_hc_ep1_ml_tracksent_dir\",\"337\":\"gear_hc_ep1_rw_hunterlodge_dir\",\"338\":\"gear_hc_ep1_rw_ravineend_dir\",\"339\":\"gear_heavybandage\",\"340\":\"gear_heavyparka\",\"341\":\"gear_heavywoolsweater\",\"342\":\"gear_highqualitytools\",\"343\":\"gear_homemadesoup\",\"344\":\"gear_hook\",\"345\":\"gear_hookandline\",\"346\":\"gear_improvisedhat\",\"347\":\"gear_improvisedmittens\",\"348\":\"gear_insulatedboots\",\"349\":\"gear_insulatedpants\",\"350\":\"gear_insulatedvest\",\"351\":\"gear_insulin\",\"352\":\"gear_jeans\",\"353\":\"gear_jeremiahknife\",\"354\":\"gear_jeremiahscoat\",\"355\":\"gear_jerrycanrusty\",\"356\":\"gear_kerosenelampb\",\"357\":\"gear_ketchupchips\",\"358\":\"gear_knife\",\"359\":\"gear_knifeimprovised\",\"360\":\"gear_knifescrapmetal\",\"361\":\"gear_knifescrapmetal\",\"362\":\"gear_knifescrapmetal_clean\",\"363\":\"gear_knowledgebrbook1\",\"364\":\"gear_knowledgebrbook2\",\"365\":\"gear_knowledgebrbook3\",\"366\":\"gear_knowledgecarterdam\",\"367\":\"gear_knowledgecollapse1\",\"368\":\"gear_knowledgecollapse2\",\"369\":\"gear_knowledgecollapse3\",\"370\":\"gear_knowledgecollapse4\",\"371\":\"gear_knowledgemilton\",\"372\":\"gear_knowledgemysterylake\",\"373\":\"gear_knowledgepvbook1\",\"374\":\"gear_knowledgepvbook2\",\"375\":\"gear_knowledgepvbook3\",\"376\":\"gear_lakecabinkey1\",\"377\":\"gear_lakecabinkey2\",\"378\":\"gear_lakecabinkey3\",\"379\":\"gear_lakecabinkey8\",\"380\":\"gear_lakedeerhuntnote\",\"381\":\"gear_lakeincidentnote\",\"382\":\"gear_lakeletter1\",\"383\":\"gear_lakesflarecachenote\",\"384\":\"gear_laketrailerkey1\",\"385\":\"gear_lakewolfcullnote\",\"386\":\"gear_lampfuel\",\"387\":\"gear_lampfuelfull\",\"388\":\"gear_leather\",\"389\":\"gear_leatherdried\",\"390\":\"gear_leatherhide\",\"391\":\"gear_leatherhidedried\",\"392\":\"gear_leathershoes\",\"393\":\"gear_leatherstrips\",\"394\":\"gear_letterbundle\",\"395\":\"gear_lightparka\",\"396\":\"gear_lilyclimbmap\",\"397\":\"gear_line\",\"398\":\"gear_longunderwear\",\"399\":\"gear_longunderwearstart\",\"400\":\"gear_longunderwearwool\",\"401\":\"gear_lorenotea\",\"402\":\"gear_lorenoteb\",\"403\":\"gear_lorenotec\",\"404\":\"gear_lorenoted\",\"405\":\"gear_mackinawjacket\",\"406\":\"gear_magnifyinglens\",\"407\":\"gear_maplesapling\",\"408\":\"gear_maplesaplingdried\",\"409\":\"gear_maplesyrup\",\"410\":\"gear_medicalsupplies\",\"411\":\"gear_medicalsupplies_hangar\",\"412\":\"gear_militaryparka\",\"413\":\"gear_miltoncaveletter\",\"414\":\"gear_miltondepositboxkey1\",\"415\":\"gear_miltondepositboxkey2\",\"416\":\"gear_miltondepositboxkey3\",\"417\":\"gear_miltonflaregunnote\",\"418\":\"gear_miltongardencache\",\"419\":\"gear_miltonletter1\",\"420\":\"gear_miltonletter2\",\"421\":\"gear_miltonparknotice\",\"422\":\"gear_miltonpostofficecollectible1\",\"423\":\"gear_miltonstorenotice\",\"424\":\"gear_minepipevalve\",\"425\":\"gear_mittens\",\"426\":\"gear_moosehide\",\"427\":\"gear_moosehidebag\",\"428\":\"gear_moosehidecloak\",\"429\":\"gear_moosehidedried\",\"430\":\"gear_moosequarter\",\"431\":\"gear_mountaintownfarmkey\",\"432\":\"gear_mountaintownfarmkey\",\"433\":\"gear_mountaintownfarmnote\",\"434\":\"gear_mountaintownlockboxkey\",\"435\":\"gear_mountaintownlockboxkey\",\"436\":\"gear_mre\",\"437\":\"gear_muklukboots\",\"438\":\"gear_newsprint\",\"439\":\"gear_newsprintbootstuffing\",\"440\":\"gear_newsprintinsulation\",\"441\":\"gear_newsprintroll\",\"442\":\"gear_noisemaker\",\"443\":\"gear_notemcu\",\"444\":\"gear_nylon\",\"445\":\"gear_oldladystolenitem\",\"446\":\"gear_oldmansbearddressing\",\"447\":\"gear_oldmansbeardharvested\",\"448\":\"gear_packmatches\",\"449\":\"gear_paperstack\",\"450\":\"gear_passengermanifest\",\"451\":\"gear_peanutbutter\",\"452\":\"gear_picturefamily_prefab\",\"453\":\"gear_pinnaclecanpeaches\",\"454\":\"gear_plaidshirt\",\"455\":\"gear_planecrashid1\",\"456\":\"gear_planecrashid10\",\"457\":\"gear_planecrashid2\",\"458\":\"gear_planecrashid3\",\"459\":\"gear_planecrashid4\",\"460\":\"gear_planecrashid5\",\"461\":\"gear_planecrashid6\",\"462\":\"gear_planecrashid7\",\"463\":\"gear_planecrashid8\",\"464\":\"gear_planecrashid9\",\"465\":\"gear_plantsurvivalnote\",\"466\":\"gear_postcard_ac_centralspire\",\"467\":\"gear_postcard_ac_topshelf\",\"468\":\"gear_postcard_bi_echoone-radiotower\",\"469\":\"gear_postcard_br_canyon\",\"470\":\"gear_postcard_br_prison\",\"471\":\"gear_postcard_cr_abandonedlookout\",\"472\":\"gear_postcard_fm_muskegoverlook\",\"473\":\"gear_postcard_fm_shortwavetower\",\"474\":\"gear_postcard_ml_forestrylookout\",\"475\":\"gear_postcard_ml_lakeoverlook\",\"476\":\"gear_postcard_mt_radiotower\",\"477\":\"gear_postcard_pv_signalhill\",\"478\":\"gear_postcard_rv_pensive\",\"479\":\"gear_postcard_tm_andrespeak\",\"480\":\"gear_postcard_tm_tailsection\",\"481\":\"gear_premiumwintercoat\",\"482\":\"gear_prisonbusnote\",\"483\":\"gear_prybar\",\"484\":\"gear_pumpkinpie\",\"485\":\"gear_pumpkinpiedarkwalker\",\"486\":\"gear_qualitywintercoat\",\"487\":\"gear_rabbitcarcass\",\"488\":\"gear_rabbitpelt\",\"489\":\"gear_rabbitpeltdried\",\"490\":\"gear_rabbitskinhat\",\"491\":\"gear_rabbitskinmittens\",\"492\":\"gear_radioparts\",\"493\":\"gear_rawbigbass\",\"494\":\"gear_rawbigmouthbass_talltales\",\"495\":\"gear_rawcohosalmon\",\"496\":\"gear_rawlakewhitefish\",\"497\":\"gear_rawmeatbear\",\"498\":\"gear_rawmeatdeer\",\"499\":\"gear_rawmeatmoose\",\"500\":\"gear_rawmeatrabbit\",\"501\":\"gear_rawmeatwolf\",\"502\":\"gear_rawrainbowtrout\",\"503\":\"gear_rawsmallmouthbass\",\"504\":\"gear_reclaimedwoodb\",\"505\":\"gear_recycledcan\",\"506\":\"gear_reishimushroom\",\"507\":\"gear_reishiprepared\",\"508\":\"gear_reishitea\",\"509\":\"gear_revolver\",\"510\":\"gear_revolverammobox\",\"511\":\"gear_revolverammocasing\",\"512\":\"gear_revolverammosingle\",\"513\":\"gear_rifle\",\"514\":\"gear_rifleammobox\",\"515\":\"gear_rifleammocasing\",\"516\":\"gear_rifleammosingle\",\"517\":\"gear_riflecleaningkit\",\"518\":\"gear_riflehuntinglodge\",\"519\":\"gear_rockcache_prefab\",\"520\":\"gear_rope\",\"521\":\"gear_rosehip\",\"522\":\"gear_rosehipsprepared\",\"523\":\"gear_rosehiptea\",\"524\":\"gear_ruralregionfarmkey\",\"525\":\"gear_scarftorn\",\"526\":\"gear_scraplead\",\"527\":\"gear_scrapmetal\",\"528\":\"gear_sewingkit\",\"529\":\"gear_sharpeningstone\",\"530\":\"gear_shedcodenote\",\"531\":\"gear_shovel\",\"532\":\"gear_simpletools\",\"533\":\"gear_skiboots\",\"534\":\"gear_skigloves\",\"535\":\"gear_skijacket\",\"536\":\"gear_snare\",\"537\":\"gear_soda\",\"538\":\"gear_sodaenergy\",\"539\":\"gear_sodagrape\",\"540\":\"gear_sodaorange\",\"541\":\"gear_softwood\",\"542\":\"gear_spearhead\",\"543\":\"gear_spraypaintcan\",\"544\":\"gear_spraypaintcanglypha\",\"545\":\"gear_stagquarter\",\"546\":\"gear_steampipevalve\",\"547\":\"gear_stick\",\"548\":\"gear_stone\",\"549\":\"gear_stumpremover\",\"550\":\"gear_survivalelevatorcrank\",\"551\":\"gear_survivalschoolclothing\",\"552\":\"gear_survivalschooldeerhunt\",\"553\":\"gear_survivalschoolfishing\",\"554\":\"gear_survivalschoolplants\",\"555\":\"gear_survivalschoolrabbits\",\"556\":\"gear_survivalschoolwolfhunt\",\"557\":\"gear_technicalbackpack\",\"558\":\"gear_teeshirt\",\"559\":\"gear_timberwolfquarter\",\"560\":\"gear_tinder\",\"561\":\"gear_tomatosoupcan\",\"562\":\"gear_toque\",\"563\":\"gear_torch\",\"564\":\"gear_tornscarf\",\"565\":\"gear_tornscarffp\",\"566\":\"gear_trailersupplies\",\"567\":\"gear_transponderparts\",\"568\":\"gear_utilitiesbill\",\"569\":\"gear_water1000ml\",\"570\":\"gear_water500ml\",\"571\":\"gear_waterpumpcrank\",\"572\":\"gear_waterpurificationtablets\",\"573\":\"gear_watersupplynotpotable\",\"574\":\"gear_watersupplypotable\",\"575\":\"gear_watertowernote\",\"576\":\"gear_whiskeybottle_a_prefab\",\"577\":\"gear_willboots\",\"578\":\"gear_willbootsstart\",\"579\":\"gear_willpants\",\"580\":\"gear_willpantsstart\",\"581\":\"gear_willparka\",\"582\":\"gear_willparka_table\",\"583\":\"gear_willparka_tossed\",\"584\":\"gear_willparkahangar\",\"585\":\"gear_willshirt\",\"586\":\"gear_willshirtstart\",\"587\":\"gear_willsweater\",\"588\":\"gear_willsweaterstart\",\"589\":\"gear_willtoque\",\"590\":\"gear_wolfcarcass\",\"591\":\"gear_wolfpelt\",\"592\":\"gear_wolfpeltdried\",\"593\":\"gear_wolfquarter\",\"594\":\"gear_wolfskincape\",\"595\":\"gear_wolfskincapetalltales\",\"596\":\"gear_woodmatches\",\"597\":\"gear_woolshirt\",\"598\":\"gear_woolsocks\",\"599\":\"gear_woolsweater\",\"600\":\"gear_woolwrap\",\"601\":\"gear_woolwrapcap\",\"602\":\"gear_workboots\",\"603\":\"gear_workgloves\",\"604\":\"gear_workpants\"}";
        public static Dictionary<int, string> OldIdDict = new Dictionary<int, string>();
        public static bool OldDictReady = false;
        public static float LockpickChance = 0.022f;
        public static float LeadKeyBrokeChance = 0.13f;

        public static Dictionary<string, Dictionary<string, BrokenFurnitureSync>> RecentBrokenFurns = new Dictionary<string, Dictionary<string, BrokenFurnitureSync>>();
        public static Dictionary<string, Dictionary<long, PickedGearSync>> RecentPickedGears = new Dictionary<string, Dictionary<long, PickedGearSync>>();
        public static Dictionary<string, Dictionary<string, int>> RecentlyLootedContainers = new Dictionary<string, Dictionary<string, int>>();
        public static Dictionary<string, Dictionary<string, int>> RecentlyHarvastedPlants = new Dictionary<string, Dictionary<string, int>>();
        public static List<DeathContainerData> DeathCreates = new List<DeathContainerData>();
        public static Dictionary<string, string> BannedUsers = new Dictionary<string, string>();


        public static void SaveGlobalData()
        {
            Log("Dedicated server saving...");
            Dictionary<string, string> GlobalData = new Dictionary<string, string>();
            GlobalData.Add("ropes", JSON.Dump(MyMod.DeployedRopes));
            GlobalData.Add("shelters", JSON.Dump(MyMod.ShowSheltersBuilded));
            int[] saveProxy = { MyMod.MinutesFromStartServer };
            GlobalData.Add("rtt", JSON.Dump(saveProxy));
            GlobalData.Add("deathcreates", JSON.Dump(MyMod.DeathCreates));
            string[] saveProxy2 = { MyMod.OveridedTime };
            GlobalData.Add("gametime", JSON.Dump(saveProxy2));
            string Jonny = JSON.Dump(GlobalData);
            SaveData("GlobalServerData", Jonny, GetSeed());
            Log("Save is done! Next save " + MyMod.DsSavePerioud + " seconds later");
        }

        public class KnockData
        {
            public Vector3 m_Position = new Vector3();
            public string m_Scene = "";
            public int m_ClientID = 0;
            public string m_ToScene = "";
            public int m_Timeout = 30;

        }

        public static List<KnockData> DoorEnterRequested = new List<KnockData>();

        public static void AddKnockDoorRequest(int ClientID, string ToScene)
        {
            Log("AddKnockDoorRequest ClinetID " + ClientID + " ToScene " + ToScene);
            foreach (KnockData Knock in DoorEnterRequested)
            {
                if(Knock.m_ClientID == ClientID && Knock.m_ToScene == ToScene)
                {
                    Knock.m_Timeout = 30;
                    return;
                }
            }

            KnockData AddKnock = new KnockData();
            AddKnock.m_ClientID = ClientID;
            AddKnock.m_ToScene = ToScene;

#if (!DEDICATED)
            if (ClientID != 0)
            {
                if (MyMod.playersData[ClientID] == null)
                {
                    return;
                }

                AddKnock.m_Scene = MyMod.playersData[ClientID].m_LevelGuid;
                AddKnock.m_Position = MyMod.playersData[ClientID].m_Position;
            }else{
                AddKnock.m_Scene = MyMod.level_guid;
                AddKnock.m_Position = GameManager.GetPlayerTransform().position;
            }
#else
            if (MyMod.playersData[ClientID] == null)
            {
                return;
            }

            AddKnock.m_Scene = MyMod.playersData[ClientID].m_LevelGuid;
            AddKnock.m_Position = MyMod.playersData[ClientID].m_Position;
#endif


            DoorEnterRequested.Add(AddKnock);

            Log("AddKnock.m_Scene " + AddKnock.m_Scene);
            Log("AddKnock.m_ToScene " + AddKnock.m_ToScene);

            ServerSend.KNOCKKNOCK(ToScene);

#if (!DEDICATED)
            if (MyMod.iAmHost && MyMod.level_guid == ToScene) 
            {
                HUDMessage.AddMessage("Somebody's knocking on the door");
            }
#endif
        }


        public static void UpdateKnockDoorRequests()
        {
            for (int i = DoorEnterRequested.Count-1; i > -1; i--)
            {
                KnockData Knock = DoorEnterRequested[i];
                int PID = Knock.m_ClientID;
                string Scene = MyMod.level_guid;
                Vector3 V3 = new Vector3(0,0,0);

                if(PID != 0)
                {
                    if (!Server.clients[PID].IsBusy())
                    {
                        DoorEnterRequested.RemoveAt(i);
                    }else{
                        Scene = MyMod.playersData[PID].m_LevelGuid;
                        V3 = MyMod.playersData[PID].m_Position;
                    }
                }
#if (!DEDICATED)
                else
                {
                    V3 = GameManager.GetPlayerTransform().position;
                }
#endif

                if (Knock.m_Timeout <= 0 || Knock.m_Scene != Scene || Vector3.Distance(V3, Knock.m_Position) > 15)
                {
                    DoorEnterRequested.RemoveAt(i);
                    Log("Knock Removed");
                } else
                {
                    Knock.m_Timeout--;
                }
            }
        }

        public static List<int> GetKnocksOnScene(string Scene)
        {
            List<int> Knocks = new List<int>();
            foreach (KnockData Knock in DoorEnterRequested)
            {
                if (Knock.m_ToScene == Scene)
                {
                    Knocks.Add(Knock.m_ClientID);
                }
            }
            Log("GetKnocksOnScene "+ Scene+"  Knockers "+ Knocks.Count);
            return Knocks;
        }
        public static void ApplyEnterFromKnock(int ClientID, string ToScene)
        {
            Log("ApplyEnterFromKnock ClientID " + ClientID + "  ToScene " + ToScene);
            foreach (KnockData Knock in DoorEnterRequested)
            {
                if (Knock.m_ClientID == ClientID && Knock.m_ToScene == ToScene)
                {
                    DoorEnterRequested.Remove(Knock);
#if (!DEDICATED)
                    if (ClientID != 0)
                    {
                        ServerSend.KNOCKENTER(ClientID, ToScene);
                    }else{
                        MyMod.EnterDoorsByScene(ToScene);
                    }
#else
                    ServerSend.KNOCKENTER(ClientID, ToScene);
#endif
                    return;
                }
            }
        }

        public class LocksmithBlank
        {
            public int m_State = 0;
            public string m_GearName = "";
            public string m_Scene = "";
            public string m_Dropper = "";

            public LocksmithBlank(int State, string Name, string Scene, string Dropper)
            {
                m_State = State;
                m_GearName = Name;
                m_Scene = Scene;
                m_Dropper = Dropper;
            }
        }
        public static void ChangeBlankState(int hash, int newState)
        {
            if (Blanks.ContainsKey(hash))
            {
                LocksmithBlank Blank = GetBlank(hash);

                if(Blank != null)
                {
                    Blank.m_State = newState;
                    Blanks.Remove(hash);
                    Blanks.Add(hash, Blank);
                }
            }
        }
#if (!DEDICATED)
        public static void AlignKey(GearItem key, string KeySeed, string Name)
        {
            if (key)
            {
                if (key.m_ObjectGuid == null)
                {
                    key.m_ObjectGuid = key.gameObject.AddComponent<ObjectGuid>();
                }

                if (Shared.HasNonASCIIChars(Name) || Name.Contains("_"))
                {
                    Name = "Incorrectly named Key";
                } else if (string.IsNullOrEmpty(Name))
                {
                    Name = "Nameless key";
                }

                if (Shared.HasNonASCIIChars(KeySeed) || KeySeed.Contains("_") || string.IsNullOrEmpty(KeySeed))
                {
                    KeySeed = "broken";
                }

                key.m_ObjectGuid.m_Guid = Name + "_" + KeySeed.ToLower();
                key.m_LocalizedDisplayName.m_LocalizationID = Name;
            }
        }
#endif

        public static void ApplyToolOnBlank(int hash, int tool, string KeyName = "", string KeySeed = "")
        {
            LocksmithBlank Blank = GetBlank(hash);
            if (Blank != null)
            {
                string Result = Shared.GetLockSmithProduct(Blank.m_GearName.ToLower(), tool);
                Log("[ApplyToolOnBlank] "+ hash+" "+ Blank.m_GearName+" Tool "+tool);
                Vector3 PlaceV3 = new Vector3(0,0,0);
                Quaternion Rotation = new Quaternion(0,0,0,0);
                string Scene = Blank.m_Scene;

                if(Result != Blank.m_GearName)
                {
                    Log("[ApplyToolOnBlank] Going to replace " + Blank.m_GearName + " on " + Result);
                    Dictionary<int, DataStr.SlicedJsonDroppedGear> LoadedData = LoadDropData(Scene);
                    Dictionary<int, DataStr.DroppedGearItemDataPacket> LoadedVisual = LoadDropVisual(Scene);

                    if(LoadedData == null || LoadedVisual == null)
                    {
                        Log("Wasn't able to load drop directory");
                        return;
                    }

                    DataStr.DroppedGearItemDataPacket oldVisual;
                    if(LoadedVisual.TryGetValue(hash, out oldVisual))
                    {
                        PlaceV3 = oldVisual.m_Position;
                        Rotation = oldVisual.m_Rotation;
                    }else{
                        Log("Wasn't able to find old item");
                        return;
                    }

                    DataStr.SlicedJsonDroppedGear NewGear = new DataStr.SlicedJsonDroppedGear();
                    NewGear.m_GearName = Result.ToLower();
                    NewGear.m_Extra.m_DroppedTime = MyMod.MinutesFromStartServer;
                    NewGear.m_Extra.m_Dropper = Blank.m_Dropper;
                    NewGear.m_Extra.m_GearName = NewGear.m_GearName;
                    NewGear.m_Extra.m_Variant = 4;
                    string GearJson;
                    int SearchKey;
#if (!DEDICATED)
                    GameObject reference = MyMod.GetGearItemObject(NewGear.m_GearName);
                    

                    if (reference != null)
                    {
                        GameObject obj = UnityEngine.Object.Instantiate<GameObject>(reference, PlaceV3, Rotation);
                        GearItem gi = obj.GetComponent<GearItem>();
                        if(!string.IsNullOrEmpty(KeyName) && !string.IsNullOrEmpty(KeySeed))
                        {
                            AlignKey(gi, KeySeed, KeyName);
                        }
                        gi.SkipSpawnChanceRollInitialDecayAndAutoEvolve();
                        obj.name = Blank.m_GearName;

                        GearJson = obj.GetComponent<GearItem>().Serialize();

                        int hashV3 = Shared.GetVectorHash(PlaceV3);
                        int hashRot = Shared.GetQuaternionHash(Rotation);
                        int hashLevelKey = Scene.GetHashCode();
                        SearchKey = hashV3 + hashRot + hashLevelKey;
                        UnityEngine.Object.Destroy(obj);
                    }else{
                        Log("Can't load reference for blank");
                        ChangeBlankState(hash, 0);
                        return;
                    }
#else
                    if (!string.IsNullOrEmpty(KeyName) && !string.IsNullOrEmpty(KeySeed))
                    {
                        GearJson = ResourceIndependent.GetLocksmithGear(NewGear.m_GearName, PlaceV3, Rotation, KeyName, KeySeed);
                    } else
                    {
                        GearJson = ResourceIndependent.GetLocksmithGear(NewGear.m_GearName, PlaceV3, Rotation);
                    }

                    if (string.IsNullOrEmpty(GearJson))
                    {
                        Log("Can't load reference for blank");
                        ChangeBlankState(hash, 0);
                        return;
                    }

                    int hashV3 = Shared.GetVectorHash(PlaceV3);
                    int hashRot = Shared.GetQuaternionHash(Rotation);
                    int hashLevelKey = Scene.GetHashCode();
                    SearchKey = hashV3 + hashRot + hashLevelKey;

#endif

                    DataStr.DroppedGearItemDataPacket GearVisual = new DataStr.DroppedGearItemDataPacket();
                    GearVisual.m_Extra = NewGear.m_Extra;
                    GearVisual.m_GearID = -1;
                    GearVisual.m_Hash = SearchKey;
                    GearVisual.m_LevelGUID = Scene;
                    GearVisual.m_Position = PlaceV3;
                    GearVisual.m_Rotation = Rotation;
                    NewGear.m_Json = GearJson;
                    RemovSpecificGear(hash, Scene); // Remove Data and visual on host
                    ServerSend.PICKDROPPEDGEAR(0, hash, true); // Remove visual data on client
#if (!DEDICATED)
                    if (!MyMod.DedicatedServerAppMode)
                    {
                        GameObject gearObj;
                        MyMod.DroppedGearsObjs.TryGetValue(hash, out gearObj);
                        if (gearObj != null)
                        {
                            MyMod.DroppedGearsObjs.Remove(hash);
                            MyMod.TrackableDroppedGearsObjs.Remove(hash);
                            UnityEngine.Object.Destroy(gearObj); // Remove visual object on host
                        }
                    }
#endif
                    Log("Removed " + hash);

                    AddGearData(Scene, SearchKey, NewGear);
                    AddGearVisual(Scene, GearVisual);
                    AddBlank(SearchKey, Result, Scene, Blank.m_Dropper);
                    Shared.FakeDropItem(GearVisual, true);
                    ServerSend.DROPITEM(0, GearVisual, true);
                    Log("Added " + SearchKey);
                }else{
                    ChangeBlankState(hash, 0);
                }
            }
        }

        public static void AddBlank(int hash, string name, string scene, string Dropper)
        {
            if (!Blanks.ContainsKey(hash))
            {
                Blanks.Add(hash, new LocksmithBlank(0, name, scene, Dropper));
            }
        }

        public static LocksmithBlank GetBlank(int hash)
        {
            if (Blanks.ContainsKey(hash))
            {
                LocksmithBlank B;
                if (Blanks.TryGetValue(hash, out B))
                {
                    return B;
                }
            }
            return null;
        }

        public static bool CanWorkOnBlank(int hash)
        {
            LocksmithBlank Blank = GetBlank(hash);
            if (Blank != null)
            {
                if(Blank.m_State != -1)
                {
                    return true;
                }
            }
            return false;
        }

        public static void LoadNonUnloadables()
        {
            int SaveSeed = GetSeed();
            Log("LoadNonUnloadables Seed "+ SaveSeed);
            string LockedDoorsJSON = LoadData("LockedDoors", SaveSeed);
            string UsersSavesHashJSON = LoadData("UsersSaveHashes", SaveSeed);
            string KilledAnimalsJSON = LoadData("KilledAnimals", SaveSeed);
            string BannedUsersJSON = LoadData("BanList");
            if (!string.IsNullOrEmpty(LockedDoorsJSON))
            {
                LockedDoors = JSON.Load(LockedDoorsJSON).Make<Dictionary<string, Dictionary<string, string>>>();
                foreach (var Dict in LockedDoors)
                {
                    foreach (var item in Dict.Value)
                    {
                        UsedKeys.Add(item.Value, true);
                    }
                }
            }
            if (!string.IsNullOrEmpty(UsersSavesHashJSON))
            {
                UsersSaveHashs = JSON.Load(UsersSavesHashJSON).Make<Dictionary<string, long>>();
            }
            if (!string.IsNullOrEmpty(KilledAnimalsJSON))
            {
                Shared.AnimalsKilled = JSON.Load(KilledAnimalsJSON).Make<Dictionary<string, DataStr.AnimalKilled>>();
            }
            if (!string.IsNullOrEmpty(BannedUsersJSON))
            {
                BannedUsers = JSON.Load(BannedUsersJSON).Make<Dictionary<string, string>>();
            }
        }

        public static string GenerateSeededGUID(int gameSeed, Vector3 v3)
        {
#if (!DEDICATED)
            int _x = (int)v3.x;
            int _y = (int)v3.y;
            int _z = (int)v3.z;
#else
            int _x = (int)v3.X;
            int _y = (int)v3.Y;
            int _z = (int)v3.Z;
#endif
            int v3Int = _x + _y + _z;
            int newSeed = gameSeed + v3Int;
            string _chars = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            System.Random newRNG = new System.Random(newSeed);
            string newGUID = "";
            for (int i = 1; i < 36; i++)
            {
                if (i == 9 || i == 14 || i == 19 || i == 24)
                {
                    newGUID = newGUID + "-";
                }
                int charIndex = newRNG.Next(0, _chars.Length);
                newGUID = newGUID + _chars[charIndex];
            }
            return newGUID;
        }

        public static string GetNewUGUID()
        {
            System.Random RNG = new System.Random();
            int GUIDseed = RNG.Next(1, 999999);
            int GUIDx = RNG.Next(-7000, 999999);
            int GUIDy = RNG.Next(-3000, 999999);
            int GUIDz = RNG.Next(-5370, 999999);

            return GenerateSeededGUID(GUIDseed, new Vector3(GUIDx, GUIDy, GUIDz));
        }

        public static void SetSaveHash(string UGUID, long hash)
        {
            if (UsersSaveHashs.ContainsKey(UGUID))
            {
                UsersSaveHashs.Remove(UGUID);
            }

            UsersSaveHashs.Add(UGUID, hash);
            SaveHashsChanged = true;
        }

        public static bool VerifySaveHash(string UGUID, long hash)
        {
            if (UsersSaveHashs.ContainsKey(UGUID))
            {
                long LastHash;
                if(UsersSaveHashs.TryGetValue(UGUID, out LastHash))
                {
                    Log("GUID " + UGUID+" Provided  hash: "+ hash+" expected "+ LastHash);
                    if(LastHash == hash)
                    {
                        Log("Verified GUID " + UGUID);
                        return true;
                    }else{
                        Log("Incorrect hash");
                        UsersSaveHashs.Remove(UGUID);
                        return false;
                    }
                }
            }else{
                Log("There no saves for GUID "+ UGUID);
                return false;
            }
            Log("No");
            return false;
        }

        public static Dictionary<string, string> GetDoorsOnScene(string Scene)
        {
            Dictionary<string, string> Dict;
            if (!LockedDoors.ContainsKey(Scene))
            {
                Dict = new Dictionary<string, string>();
                LockedDoors.Add(Scene, Dict);
                LockedDoorsChanged = true;
            }else{
                LockedDoors.TryGetValue(Scene, out Dict);
            }
            return Dict;
        }

        public static bool TryUseLeadKey()
        {
            System.Random RNG = new System.Random();
            
            return RNG.NextDouble() > LeadKeyBrokeChance;
        }

        public static bool TryUseKey(string Scene, string DoorKey, string KeySeed)
        {
            Dictionary<string, string> Dict = GetDoorsOnScene(Scene);
            if (Dict.ContainsKey(DoorKey))
            {
                string Seed = "";
                Dict.TryGetValue(DoorKey, out Seed);
                if (Seed == KeySeed)
                {
                    return true;
                }else{
                    return false;
                }
            }else{
                return true;
            }
        }

        public enum UseKeyStatus
        {
            KeyUsed,
            DoorAlreadyLocked,
            Done,
        }

        public static UseKeyStatus AddLockedDoor(string Scene, string DoorKey, string KeySeed)
        {
            Dictionary<string, string> Dict = GetDoorsOnScene(Scene);

            if (UsedKeys.ContainsKey(KeySeed))
            {
                return UseKeyStatus.KeyUsed;
            }

            if (!Dict.ContainsKey(DoorKey))
            {
                Dict.Add(DoorKey, KeySeed);
                UsedKeys.Add(KeySeed, true);
                LockedDoorsChanged = true;
                return UseKeyStatus.Done;
            }else{
                return UseKeyStatus.DoorAlreadyLocked;
            }
        }
        public static void RemoveLockedDoor(string Scene, string DoorKey)
        {
            Dictionary<string, string> Dict = GetDoorsOnScene(Scene);
            if (Dict.ContainsKey(DoorKey))
            {
                string KeySeed;
                if(Dict.TryGetValue(DoorKey, out KeySeed))
                {
                    UsedKeys.Remove(KeySeed);
                }
                
                Dict.Remove(DoorKey);
                LockedDoorsChanged = true;
            }
        }
#if (!DEDICATED)
        public static void TryLockPick(string Scene, string DoorKey, int Picker)
        {
            System.Random RNG = new System.Random();
            bool Swear = true;
            if (RNG.NextDouble() < LockpickChance)
            {
                Swear = false;
                RemoveLockedDoor(Scene, DoorKey);
                string GUID = DoorKey.Split('_')[1];
                ServerSend.REMOVEDOORLOCK(-1, GUID, Scene);

                if (!MyMod.DedicatedServerAppMode)
                {
                    if (MyMod.level_guid == Scene)
                    {
                        MyMod.RemoveLocksFromDoorsByGUID(GUID);
                    }
                }
            }
            if (Picker == 0)
            {
                MyMod.SwearOnLockpickingDone = Swear;
            } else
            {
                ServerSend.LOCKPICK(Picker, Swear);
            }
        }
#else
        public static void TryLockPick(string Scene, string DoorKey, int Picker)
        {
            System.Random RNG = new System.Random();
            bool Swear = true;
            if (RNG.NextDouble() < LockpickChance)
            {
                Swear = false;
                RemoveLockedDoor(Scene, DoorKey);
                string GUID = DoorKey.Split('_')[1];
                ServerSend.REMOVEDOORLOCK(-1, GUID, Scene);
            }
            ServerSend.LOCKPICK(Picker, Swear);
        }
#endif

        public static void SaveRecentStuff()
        {
            Stopwatch watch = null;
            if (Diagnostic)
            {
                watch = Stopwatch.StartNew();
            }

            int SaveSeed = GetSeed();
            ValidateRootExits();

            if (SaveHashsChanged)
            {
                SaveHashsChanged = false;
                SaveData("UsersSaveHashes", JSON.Dump(UsersSaveHashs), SaveSeed);
            }
            if (LockedDoorsChanged)
            {
                LockedDoorsChanged = false;
                SaveData("LockedDoors", JSON.Dump(LockedDoors), SaveSeed);
            }
            if (AnimalsKilledChanged)
            {
                AnimalsKilledChanged = false;
                SaveData("KilledAnimals", JSON.Dump(Shared.AnimalsKilled), SaveSeed);
            }

            foreach (var item in RecentVisual)
            {
                SaveData(GetKeyTemplate(SaveKeyTemplateType.DropsVisual, item.Key), JSON.Dump(item.Value), SaveSeed);
            }
            foreach (var item in RecentData)
            {
                SaveData(GetKeyTemplate(SaveKeyTemplateType.DropsData, item.Key), JSON.Dump(item.Value), SaveSeed);
            }
            foreach (var item in RecentOpenableThings)
            {
                SaveData(GetKeyTemplate(SaveKeyTemplateType.Openables, item.Key), JSON.Dump(item.Value), SaveSeed);
            }
            foreach (var item in RecentBrokenFurns)
            {
                SaveData(GetKeyTemplate(SaveKeyTemplateType.Furns, item.Key), JSON.Dump(item.Value), SaveSeed);
            }
            foreach (var item in RecentPickedGears)
            {
                SaveData(GetKeyTemplate(SaveKeyTemplateType.PickedGears, item.Key), JSON.Dump(item.Value), SaveSeed);
            }
            foreach (var item in RecentlyLootedContainers)
            {
                SaveData(GetKeyTemplate(SaveKeyTemplateType.LootedContainers, item.Key), JSON.Dump(item.Value), SaveSeed);
            }
            foreach (var item in RecentlyHarvastedPlants)
            {
                SaveData(GetKeyTemplate(SaveKeyTemplateType.HarvestedPlants, item.Key), JSON.Dump(item.Value), SaveSeed);
            }
            RecentVisual = new Dictionary<string, Dictionary<int, DataStr.DroppedGearItemDataPacket>>();
            RecentData = new Dictionary<string, Dictionary<int, DataStr.SlicedJsonDroppedGear>>();
            RecentOpenableThings = new Dictionary<string, Dictionary<string, bool>>();
            RecentBrokenFurns = new Dictionary<string, Dictionary<string, BrokenFurnitureSync>>();
            RecentPickedGears = new Dictionary<string, Dictionary<long, PickedGearSync>>();
            RecentlyLootedContainers = new Dictionary<string, Dictionary<string, int>>();
            RecentlyHarvastedPlants = new Dictionary<string, Dictionary<string, int>>();

            if (watch != null)
            {
                watch.Stop();
                Log("SaveRecentStuff() Took "+ watch.ElapsedMilliseconds+"ms");
            }
        }

        public static string LoadData(string name, int Seed = 0, bool Compressed = false)
        {
            if (NoSaveAndLoad)
            {
                return "";
            }

            if (SaveLogging)
            {
                Log("Attempt to load " + name);
            }

            
            Stopwatch watch = null;
            if (Diagnostic)
            {
                watch = Stopwatch.StartNew();
            }
            string Result = "";
            string fullPath = GetPathForName(name, Seed);
            if (!File.Exists(fullPath))
            {
                if (watch != null)
                {
                    watch.Stop();
                    Log("LoadData() Took " + watch.ElapsedMilliseconds + "ms");
                }

                if (SaveLogging)
                {
                    Log("File " + fullPath + " not exist");
                }
            }else{
                byte[] FileData = File.ReadAllBytes(fullPath);
                Result = UTF8Encoding.UTF8.GetString(FileData);
                if(watch != null)
                {
                    watch.Stop();
                    Log("LoadData() Took " + watch.ElapsedMilliseconds + "ms");
                }

                if (SaveLogging)
                {
                    Log("Loaded with no problems");
                }
            }

            if (!string.IsNullOrEmpty(Result))
            {
                Result = UpgradeOldJsonFile(Result);
            }

            return Result;
        }

#if (DEDICATED)
        
        public static string AppPath = "";

#if(!DEDICATED_LINUX)
        public static string PathSeparator = @"\";
#else
        public static string PathSeparator = @"/";
#endif
        public static string GetPathForName(string name, int Seed = 0)
        {
            if (NoSaveAndLoad)
            {
                return "";
            }
            if (string.IsNullOrEmpty(AppPath))
            {
                AppPath = AppDomain.CurrentDomain.BaseDirectory;
            }

            if (Seed != 0)
            {
                return AppPath + Seed + PathSeparator + name;
            }

            return AppPath + name;
        }
        public static string GetSeparator()
        {
            return PathSeparator;
        }
#else
        public static string GetPathForName(string name, int Seed = 0)
        {
            if (NoSaveAndLoad)
            {
                return "";
            }
            if (string.IsNullOrEmpty(PersistentDataPath.m_Path))
            {
                PersistentDataPath.Init();
            }

            if (Seed != 0)
            {
                return PersistentDataPath.m_Path + PersistentDataPath.m_PathSeparator + Seed + PersistentDataPath.m_PathSeparator + name;
            }

            return PersistentDataPath.m_Path + PersistentDataPath.m_PathSeparator + name;
        }

        public static string GetSeparator()
        {
            return PersistentDataPath.m_PathSeparator;
        }
#endif

        public static bool SaveData(string name, string content, int Seed = 0, string CustomPath = "", string JustAdd = "")
        {
            if (NoSaveAndLoad)
            {
                return false;
            }
            Stopwatch watch = null;
            if (Diagnostic)
            {
                watch = Stopwatch.StartNew();
            }

            if (SaveLogging)
            {
                Log("Attempt to save " + name);
            }

            
            string pathAndFilename = GetPathForName(name, Seed);


            if (!string.IsNullOrEmpty(CustomPath))
            {
                pathAndFilename = CustomPath;
            }
            if (!string.IsNullOrEmpty(JustAdd))
            {
                pathAndFilename = GetPathForName("", Seed)+GetSeparator()+JustAdd;
            }
            string tempFile = pathAndFilename + "_temp";
            if (File.Exists(tempFile))
                File.Delete(tempFile);
            Stream stream = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            if (stream == null)
            {
                Error("Save failed, writing stream wasn't created");
                return false;
            }
            byte[] data = new UTF8Encoding(true).GetBytes(content);
            stream.Write(data, 0, data.Length);
            stream.Dispose();
            if (File.Exists(pathAndFilename))
                File.Delete(pathAndFilename);
            File.Copy(tempFile, pathAndFilename, true);
            if (File.Exists(tempFile))
                File.Delete(tempFile);
            if (watch != null)
            {
                watch.Stop();
                Log("SaveData() Took " + watch.ElapsedMilliseconds + "ms");
            }

            if (SaveLogging)
            {
                Log("Everything alright! File saved!");
            }

            return true;
        }
        public static void DeleteData(string name, int Seed = 0)
        {
            Log("Attempt to delete " + name);
            try
            {
                string fullPath = GetPathForName(name, Seed);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
                Log("File deleted!");
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
            }
        }

        public enum SaveKeyTemplateType
        {
            Container = 0,
            DropsVisual = 1,
            DropsData = 2,
            Openables = 3,
            Furns = 4,
            PickedGears = 5,
            LootedContainers = 6,
            HarvestedPlants = 7
        }

        public static string GetKeyTemplate(SaveKeyTemplateType T, string Scene, string GUID = "")
        {            
            switch (T)
            {
                case SaveKeyTemplateType.Container:
                    return Scene + "_"+ GUID;
                case SaveKeyTemplateType.DropsVisual:
                    return Scene+ "_DropVisual";
                case SaveKeyTemplateType.Openables:
                    return Scene + "_Open";
                case SaveKeyTemplateType.DropsData:
                    return Scene + "_DropsData";
                case SaveKeyTemplateType.Furns:
                    return Scene + "_Furns";
                case SaveKeyTemplateType.PickedGears:
                    return Scene + "_PickedGears";
                case SaveKeyTemplateType.LootedContainers:
                    return Scene + "_LootedContainers";
                case SaveKeyTemplateType.HarvestedPlants:
                    return Scene + "_HarvestedPlants";
                default:
                    return "_UNKNOWN";
            }
        }
        public static bool IsFileExist(string name, int Seed = 0)
        {
            if (NoSaveAndLoad)
            {
                return false;
            }
            bool exists = File.Exists(GetPathForName(name, Seed));
            return exists;
        }

        public static void CreateFolderIfNotExist(string path)
        {
            if (NoSaveAndLoad)
            {
                return;
            }
            bool exists = Directory.Exists(path);
            if (!exists)
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void ValidateRootExits()
        {
            if (NoSaveAndLoad)
            {
                return;
            }
            int SaveSeed = GetSeed();
            CreateFolderIfNotExist(GetPathForName(SaveSeed + ""));
        }

        public static void SaveContainer(string scene, string GUID, string Content)
        {
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.Container, scene, GUID);
            ValidateRootExits();
            SaveData(Key, Content, SaveSeed);
        }
        public static string LoadContainer(string scene, string GUID)
        {
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.Container, scene, GUID);
            ValidateRootExits();
            return LoadData(Key, SaveSeed, true);
        }
        public static void RemoveContainer(string scene, string GUID)
        {
            Log("Got request to remove "+ GUID);
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.Container, scene, GUID);
            DeleteData(Key, SaveSeed);
        }
        public static bool ContainerNotEmpty(string Scene, string GUID)
        {
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.Container, Scene, GUID);
            return IsFileExist(Key, SaveSeed);
        }
        public static Dictionary<string, bool> LoadOpenableThings(string scene)
        {
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.Openables, scene);

            Dictionary<string, bool> Dict;
            if (RecentOpenableThings.TryGetValue(scene, out Dict))
            {
                return Dict;
            }

            string LoadedContent = LoadData(Key, SaveSeed);
            if (LoadedContent != "")
            {
                return JSON.Load(LoadedContent).Make< Dictionary<string, bool>>();
            }
            return null;
        }
        public static void ChangeOpenableThingState(string scene, string GUID, bool state)
        {
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.Openables, scene);
            Dictionary<string, bool> Dict;
            if (RecentOpenableThings.TryGetValue(scene, out Dict))
            {
                if (Dict.ContainsKey(GUID))
                {
                    Dict.Remove(GUID);
                }
                Dict.Add(GUID, state);
                RecentOpenableThings.Remove(scene);
                RecentOpenableThings.Add(scene, Dict);
                return;
            }else{
                Dict = LoadOpenableThings(scene);
            }

            if (Dict == null)
            {
                Dict = new Dictionary<string, bool>();
            }
            Dict.Remove(GUID);
            Dict.Add(GUID, state);
            RecentOpenableThings.Add(scene, Dict);
            ValidateRootExits();
            SaveData(Key, JSON.Dump(Dict), SaveSeed);
        }

        public static void AddHarvestedPlant(string GUID, string Scene, int Client = 0)
        {
            MPStats.AddPlantHarvested(Server.GetMACByID(Client));
            string Key = GUID;
            int SaveSeed = GetSeed();
            string SaveKey = GetKeyTemplate(SaveKeyTemplateType.HarvestedPlants, Scene);
            Dictionary<string, int> Dict;

            if (RecentlyHarvastedPlants.TryGetValue(Scene, out Dict))
            {
                Dict.Remove(Key);
                Dict.Add(Key, MyMod.MinutesFromStartServer);
                RecentlyHarvastedPlants.Remove(Scene);
                RecentlyHarvastedPlants.Add(Scene, Dict);
                return;
            } else
            {
                Dict = LoadHarvestedPlants(Scene);
            }

            if (Dict == null)
            {
                Dict = new Dictionary<string, int>();
            }
            Dict.Remove(Key);
            Dict.Add(Key, MyMod.MinutesFromStartServer);
            ValidateRootExits();
            SaveData(SaveKey, JSON.Dump(Dict), SaveSeed);
            RecentlyHarvastedPlants.Add(Scene, Dict);
        }

        public static Dictionary<string, int> LoadHarvestedPlants(string scene)
        {
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.HarvestedPlants, scene);

            Dictionary<string, int> Dict;
            if (RecentlyHarvastedPlants.TryGetValue(scene, out Dict))
            {
                return Dict;
            }

            string LoadedContent = LoadData(Key, SaveSeed);
            if (LoadedContent != "")
            {
                return JSON.Load(LoadedContent).Make<Dictionary<string, int>>();
            }
            return null;
        }

        public static void AddLootedContainer(ContainerOpenSync Box, int State = 0, int Looter = 0)
        {
            
            string Scene = Box.m_LevelGUID;
            string Key = Box.m_Guid;
            int SaveSeed = GetSeed();
            string SaveKey = GetKeyTemplate(SaveKeyTemplateType.LootedContainers, Scene);
            Dictionary<string, int> Dict;

            if (RecentlyLootedContainers.TryGetValue(Scene, out Dict))
            {
                if (Dict.ContainsKey(Key))
                {
                    Dict.Remove(Key);
                } else{
                    
                    if(Looter != -1)
                    {
                        MPStats.AddLootedContainer(Server.GetMACByID(Looter));
                    }
                }
                
                Dict.Add(Key, State);
                RecentlyLootedContainers.Remove(Scene);
                RecentlyLootedContainers.Add(Scene, Dict);
                return;
            } else
            {
                Dict = LoadLootedContainersData(Scene);
            }

            if (Dict == null)
            {
                Dict = new Dictionary<string, int>();
            }

            if (Dict.ContainsKey(Key))
            {
                Dict.Remove(Key);
            } else
            {
                if(Looter != -1)
                {
                    MPStats.AddLootedContainer(Server.GetMACByID(Looter));
                }
            }

            
            Dict.Add(Key, State);
            ValidateRootExits();
            SaveData(SaveKey, JSON.Dump(Dict), SaveSeed);
            RecentlyLootedContainers.Add(Scene, Dict);
        }

        public static Dictionary<string, int> LoadLootedContainersData(string scene)
        {
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.LootedContainers, scene);

            Dictionary<string, int> Dict;
            if (RecentlyLootedContainers.TryGetValue(scene, out Dict))
            {
                return Dict;
            }

            string LoadedContent = LoadData(Key, SaveSeed);
            if (LoadedContent != "")
            {
                return JSON.Load(LoadedContent).Make<Dictionary<string, int>>();
            }
            return null;
        }

        public static int AddLootToScene(string Scene)
        {
            Dictionary<string, int> Containers = LoadLootedContainersData(Scene);
            int Updated = 0;
            List<KeyValuePair<string, int>> Buffer = new List<KeyValuePair<string, int>>();
            foreach (var item in Containers)
            {
                Buffer.Add(item);
            }
            for (int i = 0; i < Buffer.Count; i++)
            {
                ContainerOpenSync Box = new ContainerOpenSync();
                Box.m_Guid = Buffer[i].Key;
                Box.m_LevelGUID = Scene;

                if (Buffer[i].Value != 2)
                {
                    AddLootedContainer(Box, 2, -1);
                    ServerSend.CHANGECONTAINERSTATE(0, Buffer[i].Key, 2, Scene, true);
                    Updated++;
                }
            }

            return Updated;
        }
        public static bool AddLootToContainerOnScene(string GUID, string Scene)
        {
            Dictionary<string, int> Containers = LoadLootedContainersData(Scene);

            if (Containers.ContainsKey(GUID))
            {
                ContainerOpenSync Box = new ContainerOpenSync();
                Box.m_Guid = GUID;
                Box.m_LevelGUID = Scene;
                AddLootedContainer(Box, 2, -1);
                ServerSend.CHANGECONTAINERSTATE(0, GUID, 2, Scene, true);
                return true;
            }
            return false;
        }

        public static long GetPickedGearKey(PickedGearSync Gear)
        {
            return GetPickedGearKey(Gear.m_Spawn);
        }
        public static long GetPickedGearKey(Vector3 v3)
        {
            long Key = Shared.GetVectorHashV2(v3);
            return Key;
        }

        public static void AddPickedGear(PickedGearSync Gear, int picker)
        {
            MPStats.AddPickedGear(Server.GetMACByID(picker));
            string Scene = Gear.m_LevelGUID;
            long Key = GetPickedGearKey(Gear);
            int SaveSeed = GetSeed();
            string SaveKey = GetKeyTemplate(SaveKeyTemplateType.PickedGears, Scene);
            Dictionary<long, PickedGearSync> Dict;

            if (RecentPickedGears.TryGetValue(Scene, out Dict))
            {
                Dict.Remove(Key);
                Dict.Add(Key, Gear);
                RecentPickedGears.Remove(Scene);
                RecentPickedGears.Add(Scene, Dict);
                return;
            } else
            {
                Dict = LoadPickedGearsData(Scene);
            }

            if (Dict == null)
            {
                Dict = new Dictionary<long, PickedGearSync>();
            }
            Dict.Remove(Key);
            Dict.Add(Key, Gear);
            ValidateRootExits();
            SaveData(SaveKey, JSON.Dump(Dict), SaveSeed);
            RecentPickedGears.Add(Scene, Dict);
        }

        public static Dictionary<long, PickedGearSync> LoadPickedGearsData(string scene)
        {
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.PickedGears, scene);

            Dictionary<long, PickedGearSync> Dict;
            if (RecentPickedGears.TryGetValue(scene, out Dict))
            {
                return Dict;
            }

            string LoadedContent = LoadData(Key, SaveSeed);
            if (LoadedContent != "")
            {
                return JSON.Load(LoadedContent).Make<Dictionary<long, PickedGearSync>>();
            }
            return null;
        }

        public static void AddBrokenFurn(BrokenFurnitureSync furn)
        {
            string Scene = furn.m_LevelGUID;
            string Key = furn.m_LevelGUID + furn.m_ParentGuid;

            int SaveSeed = GetSeed();
            string SaveKey = GetKeyTemplate(SaveKeyTemplateType.Furns, Scene);
            Dictionary<string, BrokenFurnitureSync> Dict;

            if (RecentBrokenFurns.TryGetValue(Scene, out Dict))
            {
                Dict.Remove(Key);
                Dict.Add(Key, furn);
                RecentBrokenFurns.Remove(Scene);
                RecentBrokenFurns.Add(Scene, Dict);
                return;
            } else
            {
                Dict = LoadFurnsData(Scene);
            }

            if (Dict == null)
            {
                Dict = new Dictionary<string, BrokenFurnitureSync>();
            }
            Dict.Remove(Key);
            Dict.Add(Key, furn);
            ValidateRootExits();
            SaveData(SaveKey, JSON.Dump(Dict), SaveSeed);
            RecentBrokenFurns.Add(Scene, Dict);
        }
        public static Dictionary<string, BrokenFurnitureSync> LoadFurnsData(string scene)
        {
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.Furns, scene);

            Dictionary<string, BrokenFurnitureSync> Dict;
            if (RecentBrokenFurns.TryGetValue(scene, out Dict))
            {
                return Dict;
            }

            string LoadedContent = LoadData(Key, SaveSeed);
            if (LoadedContent != "")
            {
                return JSON.Load(LoadedContent).Make<Dictionary<string, BrokenFurnitureSync>>();
            }
            return null;
        }

        public static Dictionary<int, DataStr.DroppedGearItemDataPacket> LoadDropVisual(string scene)
        {
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.DropsVisual, scene);

            if (!OldDictReady)
            {
                OldIdDict = JSON.Load(OldIdsJson).Make<Dictionary<int, string>>();
                OldIdsJson = "";
                OldDictReady = true;
            }

            Dictionary<int, DataStr.DroppedGearItemDataPacket> Dict;
            if(RecentVisual.TryGetValue(scene, out Dict))
            {
                return Dict;
            }

            string LoadedContent = LoadData(Key, SaveSeed);
            if (LoadedContent != "")
            {
                Dictionary<int, DataStr.DroppedGearItemDataPacket> TempDict;
                TempDict = JSON.Load(LoadedContent).Make<Dictionary<int, DataStr.DroppedGearItemDataPacket>>();
                Dictionary<int, DataStr.DroppedGearItemDataPacket> Reconstructed = new Dictionary<int, DroppedGearItemDataPacket>();

                foreach (var item in TempDict)
                {
                    string GearName = "";
                    if(item.Value.m_GearID != -1)
                    {
                        OldIdDict.TryGetValue(item.Value.m_GearID, out GearName);
                        item.Value.m_Extra.m_GearName = GearName;
                        item.Value.m_GearID = -1;
                    }
                    Reconstructed.Add(item.Key, item.Value);
                }

                return Reconstructed;
            }
            return null;
        }
        public static Dictionary<int, DataStr.SlicedJsonDroppedGear> LoadDropData(string scene)
        {
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.DropsData, scene);

            Dictionary<int, DataStr.SlicedJsonDroppedGear> Dict;
            if(RecentData.TryGetValue(scene, out Dict))
            {
                return Dict;
            }

            string LoadedContent = LoadData(Key, SaveSeed);
            if (LoadedContent != "")
            {
                Dictionary<int, DataStr.SlicedJsonDroppedGear> TempDict;
                TempDict = JSON.Load(LoadedContent).Make<Dictionary<int, DataStr.SlicedJsonDroppedGear>>();
                Dictionary<int, DataStr.SlicedJsonDroppedGear> Reconstructed = new Dictionary<int, SlicedJsonDroppedGear>();

                foreach (var item in TempDict)
                {
                    if (item.Value.m_GearName != "" && item.Value.m_Extra.m_GearName == "")
                    {
                        item.Value.m_Extra.m_GearName = item.Value.m_GearName;
                    }else if(item.Value.m_GearName == "" && item.Value.m_Extra.m_GearName != "")
                    {
                        item.Value.m_GearName = item.Value.m_Extra.m_GearName;
                    }
                    Reconstructed.Add(item.Key, item.Value);
                }

                return Reconstructed;
            }
            return null;
        }
        public static void AddGearVisual(string scene, DataStr.DroppedGearItemDataPacket gear)
        {
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.DropsVisual, scene);
            Dictionary<int, DataStr.DroppedGearItemDataPacket> Dict;
            if (RecentVisual.TryGetValue(scene, out Dict))
            {
                Dict.Remove(gear.m_Hash);
                Dict.Add(gear.m_Hash, gear);
                RecentVisual.Remove(scene);
                RecentVisual.Add(scene, Dict);
                return;
            }else{
                Dict = LoadDropVisual(scene);
            }

            if (Dict == null)
            {
                Dict = new Dictionary<int, DataStr.DroppedGearItemDataPacket>();
            }
            Dict.Remove(gear.m_Hash);
            Dict.Add(gear.m_Hash, gear);
            ValidateRootExits();
            SaveData(Key, JSON.Dump(Dict), SaveSeed);
            RecentVisual.Add(scene, Dict);
        }
        public static void AddGearData(string scene, int hash, DataStr.SlicedJsonDroppedGear GearData)
        {
            int SaveSeed = GetSeed();
            string Key = GetKeyTemplate(SaveKeyTemplateType.DropsData, scene);
            Dictionary<int, DataStr.SlicedJsonDroppedGear> Dict;

            if (RecentData.TryGetValue(scene, out Dict))
            {
                Dict.Remove(hash);
                Dict.Add(hash, GearData);
                RecentData.Remove(scene);
                RecentData.Add(scene, Dict);
                return;
            }else{
                Dict = LoadDropData(scene);
            }

            if (Dict == null)
            {
                Dict = new Dictionary<int, DataStr.SlicedJsonDroppedGear>();
            }
            Dict.Remove(hash);
            Dict.Add(hash, GearData);
            ValidateRootExits();
            SaveData(Key, JSON.Dump(Dict), SaveSeed);
            RecentData.Add(scene, Dict);
        }

        public static void RemovSpecificGear(int Hash, string Scene)
        {
            Log("[RemovSpecificGear] Trying to remove " + Hash);
            Dictionary<int, DataStr.SlicedJsonDroppedGear> Dict;
            if (!RecentData.TryGetValue(Scene, out Dict))
            {
                Dict = LoadDropData(Scene);
            }

            if(Dict != null)
            {
                Dict.Remove(Hash);
                RecentData.Remove(Scene);
                RecentData.Add(Scene, Dict);
            }

            Dictionary<int, DataStr.DroppedGearItemDataPacket> Dict2;
            if (!RecentVisual.TryGetValue(Scene, out Dict2))
            {
                Dict2 = LoadDropVisual(Scene);
            }

            if(Dict2 != null)
            {
                Dict2.Remove(Hash);
                RecentVisual.Remove(Scene);
                RecentVisual.Add(Scene, Dict2);
            }

            if (Blanks.ContainsKey(Hash))
            {
                Blanks.Remove(Hash);
            }
        }

        public static DataStr.SlicedJsonDroppedGear RequestSpecificGear(int Hash, string Scene, bool Remove = true)
        {
            Dictionary<int, DataStr.SlicedJsonDroppedGear> Dict = LoadDropData(Scene);
            DataStr.SlicedJsonDroppedGear Gear = null;
            if (Dict != null)
            {
                if(Dict.TryGetValue(Hash, out Gear))
                {
                    if (Remove)
                    {
                        RemovSpecificGear(Hash, Scene);
                    }
                }
            }
            return Gear;
        }

        public static bool SaveServerCFG(DataStr.ServerSettingsData CFG)
        {
            return SaveData("ServerSettingsData", JSON.Dump(CFG));
        }
        public static DataStr.ServerSettingsData RequestServerCFG()
        {
            string Data = LoadData("ServerSettingsData");
            if (Data != "")
            {
                if (Data.Contains("SkyCoop.MyMod+ServerSettingsData")) // 10.4 ServerSettings file.
                {
                    return null;
                }
                return JSON.Load(Data).Make<DataStr.ServerSettingsData>();
            }else{
                return null;
            }
        }
        public static void SaveMyName(string Name)
        {
            SaveData("MultiplayerNickName", Name);
        }
        public static string LoadMyName()
        {
            string Name = LoadData("MultiplayerNickName");
            return Name;
        }
        public static string GetDictionaryString(Dictionary<string, string> Dict, string Key)
        {
            string Val;
            if(Dict.TryGetValue(Key, out Val))
            {
                return Val;
            }
            return "";
        }
        public static void LoadGlobalData()
        {
            string Data = LoadData("GlobalServerData", GetSeed());
            Dictionary<string, string> GlobalData = new Dictionary<string, string>();
            if (Data != "")
            {
                GlobalData = JSON.Load(Data).Make<Dictionary<string, string>>();
            } else
            {
                return;
            }
            MyMod.DeployedRopes = JSON.Load(GetDictionaryString(GlobalData, "ropes")).Make<List<ClimbingRopeSync>>();
            MyMod.ShowSheltersBuilded = JSON.Load(GetDictionaryString(GlobalData, "shelters")).Make<List<ShowShelterByOther>>();
            int[] saveProxy = JSON.Load(GetDictionaryString(GlobalData, "rtt")).Make<int[]>();
            MyMod.MinutesFromStartServer = saveProxy[0];
            MyMod.DeathCreates = JSON.Load(GetDictionaryString(GlobalData, "deathcreates")).Make<List<DeathContainerData>>();
            string[] saveProxy2 = JSON.Load(GetDictionaryString(GlobalData, "gametime")).Make<string[]>();
            MyMod.OveridedTime = saveProxy2[0];
        }
        public static void SaveJsonSnapshot(string Alias, string JSON)
        {
            CreateFolderIfNotExist(GetPathForName("Snapshots"));
            CreateFolderIfNotExist(GetPathForName(@"Snapshots\"+ Alias));

            DateTime DT = System.DateTime.Now;
            string FileName = DT.Hour.ToString() + "_" + DT.Minute.ToString() + "_" + DT.Second.ToString() + "_" + DT.Millisecond.ToString();
            SaveData(FileName, JSON, 0, GetPathForName(@"Snapshots\" + Alias+@"\"+FileName));
        }
        public static string UpgradeOldJsonFile(string Json, bool Decompress = false)
        {
            if (Decompress)
            {
                Json = Shared.DecompressString(Json);
            }
            Json = Json.Replace("SkyCoop.MyMod", "SkyCoop.DataStr");
            if (Decompress)
            {
                Json = Shared.CompressString(Json);
            }
#if (DEDICATED)
            Json = Json.Replace("UnityEngine.Vector3", "System.Numerics.Vector3");
            Json = Json.Replace("UnityEngine.Quaternion", "System.Numerics.Quaternion");
            Json = Json.Replace("\"x\"", "\"X\"");
            Json = Json.Replace("\"y\"", "\"Y\"");
            Json = Json.Replace("\"z\"", "\"Z\"");
            Json = Json.Replace("\"w\"", "\"W\"");
#else
            Json = Json.Replace("System.Numerics.Vector3", "UnityEngine.Vector3");
            Json = Json.Replace("System.Numerics.Quaternion", "UnityEngine.Quaternion");
            Json = Json.Replace("\"X\"", "\"x\"");
            Json = Json.Replace("\"Y\"", "\"y\"");
            Json = Json.Replace("\"Z\"", "\"z\"");
            Json = Json.Replace("\"W\"", "\"w\"");
#endif

            return Json;
        }

        public static string GetSubNetworkGUID()
        {
            string GUID = "";
            //GUID = LoadData("SubNetworkGUID");
            //if(string.IsNullOrEmpty(GUID))
            //{
            //    GUID = GetNewUGUID();
            //    SaveData("SubNetworkGUID", GUID);
            //}

            GUID = Shared.GetMacAddress();


            return GUID;
        }

        public static void SaveBanned()
        {
            SaveData("BanList", JSON.Dump(BannedUsers));
        }
    }
}