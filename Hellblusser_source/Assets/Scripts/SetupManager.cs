using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SetupManager : MonoBehaviour
{
    // instance
    public static SetupManager instance;

    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // databases
    [Header("databases")]
    public GameObject databasesObject;

    // music
    [Header("music")]
    public AudioSource titleMusicAudioSource;
    public AudioSource levelCompletedMusicAudioSource;
    public AudioSource shopMusicAudioSource;
    public AudioSource restMusicAudioSource;
    public AudioSource fountainMusicAudioSource;
    public AudioSource gameOverMusicAudioSource;
    public AudioSource outroMusicAudioSource;

    // ambience
    [Header("ambience")]
    public AudioSource sewerAmbienceAudioSource;
    public AudioSource dungeonAmbienceAudioSource;
    public AudioSource hellAmbienceAudioSource;

    // current player items
    public List<EquipmentDatabase.EquipmentData> playerEquipmentDatas;
    public List<EquipmentDatabase.SpecialsPackage> playerEquipmentSpecialPackages;
    public EquipmentDatabase.EquipmentStats playerEquipmentStatsTotal;

    // delete save?
    [HideInInspector] public bool aboutToDeleteSave;

    // laser
    public AudioSource laserAttackAudioSource;
    [HideInInspector] public int laserAttackIsInPlayDur, laserAttackIsInPlayCounter;
    [HideInInspector] public bool laserAttackIsInPlay;

    // popups
    [HideInInspector] public int popupDur, popupCounter;
    [HideInInspector] public string popupString;

    [HideInInspector] public int canShowTutorialPopupDur, canShowTutorialPopupCounter;
    [HideInInspector] public int tutorialPopupDur, tutorialPopupCounter;
    [HideInInspector] public string tutorialPopupString;

    // lava
    [HideInInspector] public float lavaFactor;
    [HideInInspector] public bool clearingLava;

    // menu
    public MenuManager menuManager;

    // fountain areas
    [HideInInspector] public List<FountainArea> allFountainAreas;
    [HideInInspector] public bool playerInFountainArea;

    // prop spawners
    [HideInInspector] public List<PropSpawner> allPropSpawners;

    // npc spawners
    [HideInInspector] public List<NpcSpawner> allNpcSpawners;

    // invulnerable
    [HideInInspector] public int playerInvulnerableDur, playerInvulnerableCounter;
    [HideInInspector] public bool playerInvulnerable;

    // UI hide
    [HideInInspector] public int hideUIDur, hideUICounter;
    [HideInInspector] public bool hideUI;

    // npc count (for testing)
    [HideInInspector] public int npcCount;

    // playtime
    [HideInInspector] public float globalTime;

    // mouse
    [HideInInspector] public Vector3 mousePosition;

    // resolution
    Resolution[] supportedResolutions;
    int resolutionIndex;

    // transition
    public enum TransitionMode { In, Out };
    public TransitionMode curTransitionMode;
    [HideInInspector] public float transitionFactor, transitionFactorTarget;
    [HideInInspector] public bool inTransition;

    // game state
    [HideInInspector] public bool defeatedFinalBoss;
    [HideInInspector] public int canInteractWait, canInteractCounter;
    [HideInInspector] public bool canInteract;
    [HideInInspector] public bool quitToTitle;
    [HideInInspector] public bool quitToReload;

    public enum GameState
    {
        Setup,
        Title,
        Level,
        LevelEnd,
        Fountain,
        Shop,
        BlessingPick,
        GameOver,
        LevelSelect,
        ShopBrowse,
        Rest,
        Intro,
        Outro
    };

    public GameState curGameState;
    [HideInInspector] public bool paused;
    [HideInInspector] public int curLevelMaxCoins;
    [HideInInspector] public int curLevelMaxTears;
    [HideInInspector] public int curLevelMaxFires;
    [HideInInspector] public float fireSpawnChance;

    // settings
    [HideInInspector] public int blessingTearCost;
    [HideInInspector] public float sceneLoadWait;

    [HideInInspector] public float sfxVolFactor;
    [HideInInspector] public float musicVolFactor;

    // saturation, contrast, brightness
    public float saturationAmount;
    public float contrastAmount;
    public float brightnessAmount;

    // hacks
    [Header("hacks")]
    public bool fastMode;
    public bool noNpcs;
    public bool allowHacks;
    public bool playerStrong;
    public bool overrideSceneLoad;
    public bool preventSceneLoad;
    public bool skipIntro;
    [HideInInspector] public int outlineIndex;
    [HideInInspector] public float outlineFactor;
    [HideInInspector] public int renderScaleFactorIndex;
    [HideInInspector] public float renderScaleFactor;
    [HideInInspector] public bool hideMusic;
    [HideInInspector] public bool hidePlayerUI;

    // particles
    [HideInInspector] public List<ParticleSystem> allParticleSystems;

    // ripple effect
    Vector2 ripplePosition;
    float rippleAmount;
    float rippleFriction;

    // screenshake
    [HideInInspector] public bool inScreenShake;
    int screenShakeDur, screenShakeCounter;

    // freeze
    [HideInInspector] public bool inFreeze;
    int freezeDur, freezeCounter;
    [HideInInspector] public int npcHitFreeze;
    [HideInInspector] public int bossDefeatFreeze;

    // colors
    [HideInInspector] public string UIInteractionBaseCol;
    [HideInInspector] public string UIInteractionButtonCol;
    [HideInInspector] public string UIBlessingCol;
    [HideInInspector] public string UITearCol;
    [HideInInspector] public string UICoinCol;
    [HideInInspector] public string UIInteractionSecondCol;
    [HideInInspector] public string UIInteractionSubCol;
    [HideInInspector] public string UILockedCol;
    [HideInInspector] public string UIPopupBaseCol;
    [HideInInspector] public string UIPopupInputCol;

    // text formatting codes
    [HideInInspector] public string alignRight;
    [HideInInspector] public string alignLeft;
    [HideInInspector] public string alignCenter;
    [HideInInspector] public string lineHeight;
    [HideInInspector] public string lineHeightCancel;

    // player near fire?
    [HideInInspector] public bool playerNearFire;
    [HideInInspector] public bool playerBurning;
    [HideInInspector] public float playerBurningFac;
    [HideInInspector] public int playerBurnDamageRate, playerBurnDamageCounter;

    // encounter type
    public enum EncounterType { Small, Medium, Big, Boss };
    public EncounterType curEncounterType;

    public enum EncounterArchetype
    {
        SewerGeneric,
        Slug,
        Goblin,
        HellSkeleton,
        HellGeneric,
        HellFire,
        DungeonSpider,
        DungeonOrc,
        DungeonGeneric,
    };
    public Dictionary<EncounterArchetype,EncounterArchetypeData> encounterArchetypes;

    // location type
    public enum FloorType { Sewer, Dungeon, Hell };
    public enum LocationType { Level, BossLevel, Shop, Fountain, Rest };

    // global layerMask(s)
    [Header("global layerMasks")]
    public LayerMask playerLayerMask;
    public LayerMask lavaLayerMask;
    public LayerMask lavaOnlyLayerMask;
    public LayerMask npcGroundLayerMask;
    public LayerMask propAttachLayerMask;
    public LayerMask groundLayerMask;

    // save data
    public ProgressData curProgressData;
    public RunData runDataRead;
    public enum RunType { Normal, Endless };
    public RunType curRunType;

    [System.Serializable]
    public struct FloorData
    {
        public int locationCount;
        public List<LocationData> locationTypes;
        public List<int> locationVisitedIndex;
    }

    [System.Serializable]
    public struct LocationData
    {
        public List<int> types;
    }

    [System.Serializable]
    public struct EncounterArchetypeData
    {
        public WeightedRandomBag<Npc.Type> npcTypes;
        public WeightedRandomBag<Npc.Type> bossNpcTypes;
    }

    [System.Serializable]
    public struct RunData
    {
        public FloorData curFloorData;

        public int runSeed;
        public float runTime;

        public bool playerReachedEnd;
        public bool playerDead;

        public int playerHealthOnStartOfLevel;
        public int playerFireOnStartOfLevel;
        public int coinsOnStartOfLevel;
        public int tearsOnStartOfLevel;
        public bool usedSecondChanceOnStartOfLevel;
        public float runTimeOnStartOfLevel;
        public int bodyEquipmentIndexOnStartOfLevel;
        public int braceletEquipmentIndexOnStartOfLevel;
        public int ringEquipmentIndexOnStartOfLevel;
        public int weaponEquipmentIndexOnStartOfLevel;
        public List<int> blessingsClaimedOnStartOflevel;

        public int playerHealthMax;
        public int playerHealthCur;

        public int playerFireMax;
        public int playerFireCur;

        public int curLoopIndex;
        public int curFloorIndex;
        public int curLevelIndex;

        public int encountersHad;

        public int curLevelTearsCollected;
        public int curLevelCoinsCollected;
        public int lastLevelTearsCollected;
        public int lastLevelCoinsCollected;
        public int curLevelFiresCleared;

        public int curRunTearsCollected;
        public int curRunCoinsCollected;
        public int curRunFiresCleared;

        public int curRingEquipmentIndex;
        public int curBraceletEquipmentIndex;
        public int curBodyEquipmentIndex;
        public int curWeaponEquipmentIndex;

        public List<int> blessingsClaimed;
        public bool usedSecondChance;
    }

    [System.Serializable]
    public struct ProgressData
    {
        public PersistentData persistentData;
        public RunData normalRunData;
        public RunData endlessRunData;
        public SettingsData settingsData;
    }

    [System.Serializable]
    public struct SettingsData
    {
        public int resolutionIndex;
        public int fullscreen;

        public int sfxVolIndex;
        public int musicVolIndex;

        public int lookSensitivityX;
        public int lookSensitivityY;
        public int invertY;

        public int wobbleEffect;
        public int cameraMotion;
        public int endlessFilter;
    }

    [System.Serializable]
    public struct PersistentData
    {
        public float playTime;
        public bool sawIntro;
        public bool sawOutro;
        public bool unlockedEndless;
        public bool inNormalRun;
        public bool inEndlessRun;

        public bool sawFirePopup;
        public bool sawFireCollectPopup;
        public bool sawKickPopup;
        public bool sawCombatPopup;
        public bool sawCoinPopup;
        public bool sawTearPopup;
        public bool sawDonutPopup;
        public bool sawShopPopup;
        public bool sawFountainPopup;
        public bool sawKeyPopup;
        public bool sawRestPopup;
        public bool sawEndlessPopup;
        public bool sawMaxFirePopup;

        public int normalHighestLoopIndex;
        public int normalHighestFloorIndex;
        public int normalHighestLevelIndex;

        public int endlessHighestLoopIndex;
        public int endlessHighestFloorIndex;
        public int endlessHighestLevelIndex;
    }

    void Awake ()
    {
        // get instance
        instance = this;

        // initialize
        Init();
    }

    void Init ()
    {
        // audio listener
        AudioListener.volume = 0f;

        // player burning
        playerNearFire = false;
        playerBurning = false;
        playerBurningFac = 0f;
        playerBurnDamageRate = 120;
        playerBurnDamageCounter = 0;

        // create encounter archetypes
        CreateEncounterArchetypes();

        // get base components
        myTransform = transform;
        myGameObject = gameObject;
        DontDestroyOnLoad(myGameObject);

        // laser
        laserAttackIsInPlayDur = 2;
        laserAttackIsInPlayCounter = laserAttackIsInPlayDur;
        laserAttackIsInPlay = false;
        laserAttackAudioSource.volume = 0f;

        // final boss defeated?
        defeatedFinalBoss = false;

        // spawners
        allPropSpawners = new List<PropSpawner>();
        allNpcSpawners = new List<NpcSpawner>();

        // fountain areas
        allFountainAreas = new List<FountainArea>();

        // lava
        lavaFactor = 1f;

        // databases
        DontDestroyOnLoad(databasesObject);

        // gamer framerate
        Application.targetFrameRate = 60;

        // v-sync gang
        QualitySettings.vSyncCount = 0;

        // ripple effect
        rippleAmount = 0f;
        rippleFriction = .9f;

        // playtime
        globalTime = 0f;

        // interaction wait
        canInteractWait = 2;
        canInteractCounter = 0;
        canInteract = false;

        // resolutions
        supportedResolutions = Screen.resolutions;
        resolutionIndex = (supportedResolutions.Length - 1);
        Screen.SetResolution(supportedResolutions[resolutionIndex].width, supportedResolutions[resolutionIndex].height, true);

        // run type
        curRunType = RunType.Normal;

        // new progress
        if ( SaveManager.instance.CheckIfFileExists() )
        {
            curProgressData = SaveManager.instance.LoadFromFile();
        }
        else
        {
            curProgressData = SaveManager.instance.CreateNewData();
            SaveManager.instance.WriteToFile(curProgressData);
        }

        //SaveManager.instance.CreateNewData();
        //CreateNewProgressData();

        // saturation, contrast, brightness
        saturationAmount = 1f;
        contrastAmount = 1f;
        brightnessAmount = 1f;

        // outline factor
        outlineIndex = 0;

        // render scale
        renderScaleFactorIndex = 0;

        // amount of collectibles
        curLevelMaxCoins = 999;
        curLevelMaxTears = 999;
        curLevelMaxFires = 999;

        // freeze
        npcHitFreeze = 8;
        bossDefeatFreeze = 180;
        
        // settings
        blessingTearCost = 3;
        sceneLoadWait = .75f;

        // tutorial popup
        canShowTutorialPopupDur = 90;
        canShowTutorialPopupCounter = canShowTutorialPopupDur;

        // colors
        UIInteractionBaseCol = "<#E0C1A8>";
        UIInteractionButtonCol = "<#AA7B7F>";
        UIBlessingCol = "<#AFC8D1>";
        UITearCol = "<#A5B5C3>";
        UICoinCol = "<#AF9D94>";
        UIInteractionSecondCol = "<#595959>";
        UIInteractionSubCol = "<#CBCACA>";
        UILockedCol = "<#756F6D>";
        UIPopupBaseCol = "<#c7bbbf>";//"<#c0b7ac>";
        UIPopupInputCol = "<#8695a6>";//"<#a5868a>";

        // text formatting codes
        alignRight = "<align=\"right\">";
        alignLeft = "<align=\"left\">";
        alignCenter = "<align=\"center\">";
        lineHeight = "<line-height=0.00000000001>" + "\n";
        lineHeightCancel = "</line-height>" + "</align>";

        // music, ff dimmen allemaal!!!!
        shopMusicAudioSource.volume = 0f;
        restMusicAudioSource.volume = 0f;
        fountainMusicAudioSource.volume = 0f;
        levelCompletedMusicAudioSource.volume = 0f;
        gameOverMusicAudioSource.volume = 0f;
        titleMusicAudioSource.volume = 0f;
        outroMusicAudioSource.volume = 0f;

        // ambience... ook ff stil svp
        sewerAmbienceAudioSource.volume = 0f;
        dungeonAmbienceAudioSource.volume = 0f;
        hellAmbienceAudioSource.volume = 0f;

        // proceed
        ProceedToGame();
    }

    public void CreateEncounterArchetypes ()
    {
        encounterArchetypes = new Dictionary<EncounterArchetype,EncounterArchetypeData>();

        // generic archetype
        EncounterArchetypeData sewerGenericArchetypeData = new EncounterArchetypeData();
        sewerGenericArchetypeData.npcTypes = new WeightedRandomBag<Npc.Type>();
        sewerGenericArchetypeData.bossNpcTypes = new WeightedRandomBag<Npc.Type>();

        sewerGenericArchetypeData.npcTypes.AddEntry(Npc.Type.Rat,10);
        sewerGenericArchetypeData.bossNpcTypes.AddEntry(Npc.Type.BigRat, 30);
        encounterArchetypes.Add(EncounterArchetype.SewerGeneric, sewerGenericArchetypeData);

        // slug archetype
        EncounterArchetypeData slugArchetypeData = new EncounterArchetypeData();
        slugArchetypeData.npcTypes = new WeightedRandomBag<Npc.Type>();
        slugArchetypeData.bossNpcTypes = new WeightedRandomBag<Npc.Type>();

        slugArchetypeData.npcTypes.AddEntry(Npc.Type.Slug, 30);
        slugArchetypeData.bossNpcTypes.AddEntry(Npc.Type.BigSlug, 30);
        encounterArchetypes.Add(EncounterArchetype.Slug, slugArchetypeData);

        // goblin archetype
        EncounterArchetypeData goblinArchetypeData = new EncounterArchetypeData();
        goblinArchetypeData.npcTypes = new WeightedRandomBag<Npc.Type>();
        goblinArchetypeData.bossNpcTypes = new WeightedRandomBag<Npc.Type>();

        goblinArchetypeData.npcTypes.AddEntry(Npc.Type.Goblin, 10);
        goblinArchetypeData.npcTypes.AddEntry(Npc.Type.GoblinRanger,10);
        goblinArchetypeData.npcTypes.AddEntry(Npc.Type.GoblinMage, 10);
        goblinArchetypeData.bossNpcTypes.AddEntry(Npc.Type.GoblinChieftain, 30);
        encounterArchetypes.Add(EncounterArchetype.Goblin, goblinArchetypeData);

        // dungeon archetype
        EncounterArchetypeData dungeonGenericArchetypeData = new EncounterArchetypeData();
        dungeonGenericArchetypeData.npcTypes = new WeightedRandomBag<Npc.Type>();
        dungeonGenericArchetypeData.bossNpcTypes = new WeightedRandomBag<Npc.Type>();

        dungeonGenericArchetypeData.npcTypes.AddEntry(Npc.Type.Skeleton, 10);
        dungeonGenericArchetypeData.npcTypes.AddEntry(Npc.Type.Ghost, 10);
        dungeonGenericArchetypeData.npcTypes.AddEntry(Npc.Type.GoblinRanger, 5);
        dungeonGenericArchetypeData.bossNpcTypes.AddEntry(Npc.Type.BlackSkeleton, 30);
        encounterArchetypes.Add(EncounterArchetype.DungeonGeneric, dungeonGenericArchetypeData);

        // orc archetype
        EncounterArchetypeData orcArchetypeData = new EncounterArchetypeData();
        orcArchetypeData.npcTypes = new WeightedRandomBag<Npc.Type>();
        orcArchetypeData.bossNpcTypes = new WeightedRandomBag<Npc.Type>();

        orcArchetypeData.npcTypes.AddEntry(Npc.Type.OrcWarrior, 10);
        orcArchetypeData.npcTypes.AddEntry(Npc.Type.OrcMage, 10);
        orcArchetypeData.bossNpcTypes.AddEntry(Npc.Type.OrcLeader, 30);
        encounterArchetypes.Add(EncounterArchetype.DungeonOrc, orcArchetypeData);

        // spider archetype
        EncounterArchetypeData spiderArchetypeData = new EncounterArchetypeData();
        spiderArchetypeData.npcTypes = new WeightedRandomBag<Npc.Type>();
        spiderArchetypeData.bossNpcTypes = new WeightedRandomBag<Npc.Type>();

        spiderArchetypeData.npcTypes.AddEntry(Npc.Type.MediumSpider, 10);
        spiderArchetypeData.npcTypes.AddEntry(Npc.Type.SmallSpider, 10);
        spiderArchetypeData.npcTypes.AddEntry(Npc.Type.Bat, 5);
        spiderArchetypeData.npcTypes.AddEntry(Npc.Type.Ghost, 5);
        spiderArchetypeData.bossNpcTypes.AddEntry(Npc.Type.BigSpider, 30);
        encounterArchetypes.Add(EncounterArchetype.DungeonSpider, spiderArchetypeData);

        // hell fire archetype
        EncounterArchetypeData hellFireArchetypeData = new EncounterArchetypeData();
        hellFireArchetypeData.npcTypes = new WeightedRandomBag<Npc.Type>();
        hellFireArchetypeData.bossNpcTypes = new WeightedRandomBag<Npc.Type>();

        hellFireArchetypeData.npcTypes.AddEntry(Npc.Type.FireDevil, 10);
        hellFireArchetypeData.npcTypes.AddEntry(Npc.Type.SmallFireRat, 10);
        hellFireArchetypeData.bossNpcTypes.AddEntry(Npc.Type.FireDevil, 30);
        encounterArchetypes.Add(EncounterArchetype.HellFire, hellFireArchetypeData);

        // hell scary archetype
        EncounterArchetypeData hellGenericArchetypeData = new EncounterArchetypeData();
        hellGenericArchetypeData.npcTypes = new WeightedRandomBag<Npc.Type>();
        hellGenericArchetypeData.bossNpcTypes = new WeightedRandomBag<Npc.Type>();

        hellGenericArchetypeData.npcTypes.AddEntry(Npc.Type.HellPopje, 10);
        hellGenericArchetypeData.npcTypes.AddEntry(Npc.Type.HellFlyingFace, 10);
        hellGenericArchetypeData.bossNpcTypes.AddEntry(Npc.Type.FireDevil, 30);
        encounterArchetypes.Add(EncounterArchetype.HellGeneric, hellGenericArchetypeData);

        // hell skeleton archetype
        EncounterArchetypeData hellSkeletonArchetypeData = new EncounterArchetypeData();
        hellSkeletonArchetypeData.npcTypes = new WeightedRandomBag<Npc.Type>();
        hellSkeletonArchetypeData.bossNpcTypes = new WeightedRandomBag<Npc.Type>();

        hellSkeletonArchetypeData.npcTypes.AddEntry(Npc.Type.RedSkeleton, 30);
        hellSkeletonArchetypeData.npcTypes.AddEntry(Npc.Type.FireDevil, 5);
        hellSkeletonArchetypeData.bossNpcTypes.AddEntry(Npc.Type.BlackSkeleton, 30);
        encounterArchetypes.Add(EncounterArchetype.HellSkeleton, hellSkeletonArchetypeData);
    }

    void ProceedToGame ()
    {
        // reset input
        if (InputManager.instance != null)
        {
            InputManager.instance.DisableAllInput();
            //for ( int i = 0; i < HandManager.instance.handScripts.Length; i ++ )
            //{
            //    HandManager.instance.handScripts[i].StopCharge();
            //}
        }

        // go
        if (!preventSceneLoad)
        {
            if (overrideSceneLoad)
            {
                CreateNewRun(RunType.Normal);

                UpdateRunDataRead();

                switch (curRunType)
                {
                    case RunType.Normal:
                        curProgressData.normalRunData.curFloorIndex = 0;
                        curProgressData.normalRunData.curLevelIndex = 0;//(runDataRead.curFloorData.locationCount - 1);
                        curProgressData.normalRunData.encountersHad = 0;
                        //SetEncounterType(EncounterType.Boss);
                        break;

                    case RunType.Endless:
                        curProgressData.endlessRunData.curFloorIndex = 1;
                        curProgressData.endlessRunData.curLevelIndex = 1;//(runDataRead.curFloorData.locationCount);
                        curProgressData.endlessRunData.encountersHad = 1;
                        break;
                }

                UpdateRunDataRead();

                // to outro
                //SetGameState(GameState.Title);
                //SceneManager.LoadScene(9 + 1,LoadSceneMode.Single);

                // to title
                //SetGameState(GameState.Title);
                //SceneManager.LoadScene(1 + 1, LoadSceneMode.Single);

                // to rest
                //SetGameState(GameState.Rest);
                //SceneManager.LoadScene(8 + 1, LoadSceneMode.Single);

                // to game level
                //SetEncounterType((runDataRead.curLevelIndex == (runDataRead.curFloorData.locationCount)) ? EncounterType.Boss : EncounterType.Small);
                //SetGameState(GameState.Level);
                //SceneManager.LoadScene(2 + 1, LoadSceneMode.Single);

                // to shop
                SetGameState(GameState.Shop);
                SceneManager.LoadScene(5 + 1, LoadSceneMode.Single);

                // to fountain
                //SetGameState(GameState.Fountain);
                //SceneManager.LoadScene(4 + 1, LoadSceneMode.Single);

                // to level select
                //curProgressData.curFloorIndex = 1;
                //curProgressData.curLevelIndex = 1;
                //curProgressData.encountersHad = 1;
                //SetGameState(GameState.LevelSelect);
                //SceneManager.LoadScene(7 + 1, LoadSceneMode.Single);

                // to level end
                //SetGameState(GameState.LevelEnd);
                //SceneManager.LoadScene(3 + 1, LoadSceneMode.Single);

                // to game over
                //SetGameState(GameState.GameOver);
                //SceneManager.LoadScene(6 + 1, LoadSceneMode.Single);

                //// to intro
                //SetGameState(GameState.Title);
                //SceneManager.LoadScene(1 + 1,LoadSceneMode.Single);
            }
            else
            {
                SetGameState(GameState.Intro);
                SceneManager.LoadScene(1 + 1, LoadSceneMode.Single);
            }
        }
        else
        {
            InitStartTransition();
        }
    }

    public void UpdateRunDataRead ()
    {
        // define run data we are reading from
        switch (curRunType)
        {
            case RunType.Normal: runDataRead = curProgressData.normalRunData; break;
            case RunType.Endless: runDataRead = curProgressData.endlessRunData; break;
        }
    }

    public void UpdatePlayerEquipment ()
    {
        UpdateRunDataRead();

        playerEquipmentDatas = new List<EquipmentDatabase.EquipmentData>();
        
        // get body equipment
        if ( runDataRead.curBodyEquipmentIndex != -1 )
        {
            EquipmentDatabase.Equipment bodyEquipment = (EquipmentDatabase.Equipment)(runDataRead.curBodyEquipmentIndex);
            playerEquipmentDatas.Add(EquipmentDatabase.instance.GetEquipmentData(bodyEquipment));
        }

        // get weapon equipment
        if (runDataRead.curWeaponEquipmentIndex != -1)
        {
            EquipmentDatabase.Equipment weaponEquipment = (EquipmentDatabase.Equipment)(runDataRead.curWeaponEquipmentIndex);
            playerEquipmentDatas.Add(EquipmentDatabase.instance.GetEquipmentData(weaponEquipment));
        }

        // get ring equipment
        if (runDataRead.curRingEquipmentIndex != -1)
        {
            EquipmentDatabase.Equipment ringEquipment = (EquipmentDatabase.Equipment)(runDataRead.curRingEquipmentIndex);
            playerEquipmentDatas.Add(EquipmentDatabase.instance.GetEquipmentData(ringEquipment));
        }

        // get bracelet equipment
        if (runDataRead.curBraceletEquipmentIndex != -1)
        {
            EquipmentDatabase.Equipment braceletEquipment = (EquipmentDatabase.Equipment)(runDataRead.curBraceletEquipmentIndex);
            playerEquipmentDatas.Add(EquipmentDatabase.instance.GetEquipmentData(braceletEquipment));
        }

        // get total stats boosts
        playerEquipmentStatsTotal = new EquipmentDatabase.EquipmentStats();
        playerEquipmentSpecialPackages = new List<EquipmentDatabase.SpecialsPackage>();

        List<EquipmentDatabase.Specials> playerEquipmentSpecials = new List<EquipmentDatabase.Specials>();
        for ( int i = 0; i < playerEquipmentDatas.Count; i ++ )
        {
            List<EquipmentDatabase.SpecialsPackage> packagesCheck = playerEquipmentDatas[i].specialsPackage;
            if (packagesCheck != null)
            {
                for (int ii = 0; ii < packagesCheck.Count; ii++)
                {
                    EquipmentDatabase.SpecialsPackage curPackage = packagesCheck[ii];
                    playerEquipmentSpecials.Add(curPackage.special);
                }
            }
        }

        for ( int i = 0; i < playerEquipmentDatas.Count; i ++ )
        {
            EquipmentDatabase.EquipmentStats statsCheck = playerEquipmentDatas[i].stats;
            //List<EquipmentDatabase.Specials> specialsCheck = new List<EquipmentDatabase.Specials>(); //playerEquipmentDatas[i].specials;

            playerEquipmentStatsTotal.moveSpeedAdd += statsCheck.moveSpeedAdd;
            playerEquipmentStatsTotal.knockbackAdd += statsCheck.knockbackAdd;
            playerEquipmentStatsTotal.attackSpeedAdd += statsCheck.attackSpeedAdd;
            playerEquipmentStatsTotal.meleeDamageAdd += statsCheck.meleeDamageAdd;
            playerEquipmentStatsTotal.magicDamageAdd += statsCheck.magicDamageAdd;
            playerEquipmentStatsTotal.alertDstAdd += statsCheck.alertDstAdd;
            playerEquipmentStatsTotal.maxHealthAdd += statsCheck.maxHealthAdd;
            playerEquipmentStatsTotal.maxFireAdd += statsCheck.maxFireAdd;

            // store specials?
            List<EquipmentDatabase.SpecialsPackage> packagesCheck = playerEquipmentDatas[i].specialsPackage;
            if (packagesCheck != null)
            {
                for (int ii = 0; ii < packagesCheck.Count; ii++)
                {
                    EquipmentDatabase.SpecialsPackage curPackage = packagesCheck[ii];
                    playerEquipmentSpecialPackages.Add(curPackage);
                }
            }

            /*
            if (playerEquipmentDatas[i].specials != null && playerEquipmentDatas[i].specials.Count > 0)
            {
                for (int ii = 0; ii < playerEquipmentDatas[i].specials.Count; ii++)
                {

                    //if (!playerEquipmentSpecials.Contains(playerEquipmentDatas[i].specials[ii]))
                    //{
                    //    playerEquipmentSpecials.Add(playerEquipmentDatas[i].specials[ii]);
                    //}
                }
            }
            */
        }
    }

    void Update ()
    {
        // update current player equipments
        //UpdatePlayerEquipment();

        // laser?
        if ( !inFreeze && !paused )
        {
            if ( laserAttackIsInPlayCounter < laserAttackIsInPlayDur )
            {
                laserAttackIsInPlayCounter++;
            }
            laserAttackIsInPlay = (laserAttackIsInPlayCounter < laserAttackIsInPlayDur);
            float laserVolTarget = (laserAttackIsInPlay) ? .25f : 0f;
            float laserVolLerpie = 20f;
            laserAttackAudioSource.volume = Mathf.Lerp(laserAttackAudioSource.volume,laserVolTarget,laserVolLerpie * Time.deltaTime);
        }

        // popups
        if ( popupCounter < popupDur )
        {
            popupCounter++;
        }

        if ( quitToReload )
        {
            SetHideUI(10);
        }

        if ( tutorialPopupCounter < tutorialPopupDur )
        {
            SetHideUI(10);
            if (tutorialPopupCounter == 0)
            {
                tutorialPopupCounter++;
            }
            else
            {
                if (!aboutToDeleteSave)
                {
                    if (InputManager.instance.interactPressed)
                    {
                        CloseTutorialPopup();
                    }
                }
                else
                {
                    if ( InputManager.instance.interactPressed )
                    {
                        SaveManager.instance.ClearFile();
                        CloseTutorialPopup();
                        InitQuitToReload();
                    }
                    if ( InputManager.instance.cancelPressed )
                    {
                        CloseTutorialPopup();
                        aboutToDeleteSave = false;
                    }
                }
            }
            SetFreeze(1);
        }
        else
        {
            if (canShowTutorialPopupCounter < canShowTutorialPopupDur)
            {
                canShowTutorialPopupCounter++;
            }
        }

        // check if player in fountain area?
        playerInFountainArea = false;
        if ( allFountainAreas != null && allFountainAreas.Count > 0 )
        {
            for ( int i = 0; i < allFountainAreas.Count; i ++ )
            {
                FountainArea curFountainArea = allFountainAreas[i];
                if ( curFountainArea != null )
                {
                    Vector3 p0 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                    Vector3 p1 = curFountainArea.myTransform.position;
                    float d0 = Vector3.Distance(p0,p1);
                    if ( d0 <= curFountainArea.radius )
                    {
                        playerInFountainArea = true;
                        break;
                    }
                }
            }
        }

        // player near fire?
        if ( !inFreeze && GameManager.instance != null && !GameManager.instance.inGameWait && !GameManager.instance.inCountdown )
        {
            playerNearFire = false;
            playerBurning = false;

            float playerNearFireThreshold = 4f;
            float playerBurnThreshold = 1.75f;
            if ((curGameState == GameState.Level || curGameState == GameState.BlessingPick) && GameManager.instance != null && GameManager.instance.playerFirstPersonDrifter != null)
            {
                if (GameManager.instance.fireScripts != null && GameManager.instance.fireScripts.Count > 0)
                {
                    for (int i = 0; i < GameManager.instance.fireScripts.Count; i++)
                    {
                        if (GameManager.instance.fireScripts[i] != null && GameManager.instance.fireScripts[i].myTransform != null)
                        {
                            Vector3 p0 = GameManager.instance.fireScripts[i].myTransform.position;
                            Vector3 p1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                            p1.y = p0.y;
                            float d0 = Vector3.Distance(p0, p1);
                            if (d0 <= playerNearFireThreshold)
                            {
                                playerNearFire = true;
                            }
                            if ( (d0 <= playerBurnThreshold) && GameManager.instance.fireScripts[i].canBurnPlayer )
                            {
                                playerBurning = true;
                            }
                        }
                    }
                }
            }

            if ( GameManager.instance != null )
            {
                if ( GameManager.instance.inGameWait || GameManager.instance.inCountdown )
                {
                    playerBurningFac = 0f;
                }
            }

            float playerBurningSpd = 2f;
            if ( playerBurning )
            {
                if ( playerBurningFac < 1f )
                {
                    playerBurningFac += ((playerBurningSpd * .75f) * Time.deltaTime);
                }
                else
                {
                    playerBurningFac = 1f;
                }

                // hit player?
                if (playerBurningFac >= 1f)
                {
                    if (playerBurnDamageCounter < playerBurnDamageRate)
                    {
                        playerBurnDamageCounter++;
                    }
                    else
                    {
                        if (GameManager.instance != null)
                        {
                            HandManager.instance.handScripts[1].StopMeleeAttack();
                            GameManager.instance.SetPlayerStunned(3);
                            GameManager.instance.DealDamageToPlayer(1,Npc.AttackData.DamageType.Magic);
                        }
                        playerBurnDamageCounter = 0;
                    }
                }
                else
                {
                    playerBurnDamageCounter = 0;
                }
            }
            else
            {
                playerBurnDamageCounter = 0;

                if (playerBurningFac > 0f)
                {
                    playerBurningFac -= (playerBurningSpd * Time.deltaTime);
                }
                else
                {
                    playerBurningFac = 0f;
                }
            }
        }

        // update run data read
        UpdateRunDataRead();

        // interaction wait
        if ( !inFreeze && !paused && !inTransition )
        {
            if ( canInteractCounter < canInteractWait )
            {
                canInteractCounter++;
            }
        }
        else
        {
            canInteractCounter = 0;
        }
        canInteract = (canInteractCounter >= canInteractWait);

        // update blessing tear cost
        //Debug.Log("runDataRead blessingClaimed: " + runDataRead.blessingsClaimed + " || count: " + runDataRead.blessingsClaimed.Count.ToString() + " || " + Time.time.ToString());

        int blessingCostIncrease = 3;
        blessingTearCost = 3 + (runDataRead.blessingsClaimed.Count * blessingCostIncrease);
        if ( blessingTearCost > 20 )
        {
            blessingTearCost = 20;
        }

        // audioListener
        float audioListenerVolTarget = ( curTransitionMode == TransitionMode.In ) ? 0f : 1f;
        if ( fastMode )
        {
            audioListenerVolTarget = 1f;
        }
        float audioListenerVolLerpie = .5f;
        if ( quitToTitle || quitToReload )
        {
            audioListenerVolTarget = 0f;
            audioListenerVolLerpie = 2.5f;
        }
        AudioListener.volume = Mathf.Lerp(AudioListener.volume,audioListenerVolTarget,audioListenerVolLerpie * Time.deltaTime);

        // update volume factors
        sfxVolFactor = 1f + ((float)(curProgressData.settingsData.sfxVolIndex) * .1f);
        musicVolFactor = 1f + ((float)(curProgressData.settingsData.musicVolIndex) * .1f);

        // ripple effect
        UpdateRippleEffect();

        // music
        UpdateMusic();

        // hide UI?
        if ( hideUICounter < hideUIDur )
        {
            hideUICounter++;
        }
        hideUI = (hideUICounter < hideUIDur);

        // renderScale & outline factor
        renderScaleFactor = (renderScaleFactorIndex == 0) ? 1f : 2f;
        outlineFactor = (outlineIndex == 0) ? 1f : 2f;
        if (allowHacks)
        {
            // render scale factor hack
            if (InputManager.instance.renderScaleToggleHackPressed)
            {
                renderScaleFactorIndex = (renderScaleFactorIndex == 0) ? 1 : 0;
            }
            
            // outline factor hack
            if (InputManager.instance.outlineToggleHackPressed)
            {
                outlineIndex = (outlineIndex == 0) ? 1 : 0;
            }

            /*
            // restart hack
            if (InputManager.instance.restartPressed)
            {
                Restart();
            }
            */

            // music hack
            if ( InputManager.instance.musicToggleHackPressed )
            {
                hideMusic = !hideMusic;
            }

            // player UI hack
            if ( InputManager.instance.playerUIHackPressed )
            {
                hidePlayerUI = !hidePlayerUI;
            }

            // set player dead hack
            if (InputManager.instance.playerDeadHackPressed)
            {
                if (GameManager.instance != null && !runDataRead.playerDead)
                {
                    GameManager.instance.SetPlayerDead();
                }
            }
        }

        // player invulnerable?
        if ( !inFreeze && !paused && !inTransition )
        {
            if ( playerInvulnerableCounter < playerInvulnerableDur )
            {
                playerInvulnerableCounter++;
            }
            playerInvulnerable = (playerInvulnerableCounter < playerInvulnerableDur);
        }

        // playtime
        if ( !paused && !inTransition && (curGameState == GameState.Level || curGameState == GameState.Fountain || curGameState == GameState.Shop) )
        {
            bool preventTimer = false;
            if ( GameManager.instance != null )
            {
                if ( GameManager.instance.playerHurt || runDataRead.playerDead || GameManager.instance.inGameWait || GameManager.instance.inCountdown )
                {
                    preventTimer = true;
                }
            }
            if (!preventTimer)
            {
                globalTime += Time.deltaTime;

                switch ( curRunType )
                {
                    case RunType.Normal: curProgressData.normalRunData.runTime += Time.deltaTime; break;
                    case RunType.Endless: curProgressData.endlessRunData.runTime += Time.deltaTime; break;
                }
            }
        }

        // get mouse position
        mousePosition = InputManager.instance.mousePosition;

        // transition
        UpdateTransition();

        // resolution
        UpdateResolution();

        // freeze
        UpdateFreeze();

        // screenShake
        UpdateScreenShake();

        // paused?
        bool canCancel = (paused || curGameState == GameState.ShopBrowse || curGameState == GameState.BlessingPick);
        if ( InputManager.instance.quitPressed || (canCancel && InputManager.instance.cancelPressed))
        {
            bool canPause = (curGameState != GameState.BlessingPick && curGameState != GameState.ShopBrowse && curGameState != GameState.Intro && curGameState != GameState.Outro && !inTransition && !quitToTitle && !quitToReload && !defeatedFinalBoss && !runDataRead.playerDead);
            if ( GameManager.instance != null && GameManager.instance.inCountdown )
            {
                canPause = false;
            }
            if (GameManager.instance != null && GameManager.instance.playerHurt)
            {
                canPause = false;
            }
            if ( tutorialPopupCounter < tutorialPopupDur )
            {
                canPause = false;
            }
            if (canPause)
            {
                TogglePaused();
            }
        }

        // paused filter & behaviour
        bool showPausedFilter = (paused || (GameManager.instance != null && GameManager.instance.inCountdown) || curGameState == GameState.BlessingPick || curGameState == GameState.ShopBrowse || curGameState == GameState.ShopBrowse || (tutorialPopupCounter < tutorialPopupDur) );
        if ( showPausedFilter )
        {
            saturationAmount = 0f;
            contrastAmount = .99f;
            brightnessAmount = .05f;
        }
        else
        {
            saturationAmount = 1f;
            contrastAmount = 1f;
            brightnessAmount = 1f;
        }
        if ( paused )
        {
            SetFreeze(1);
        }
    }

    public void InitQuitToTitle()
    {
        TogglePaused();
        SetTransition(TransitionMode.In);
        quitToTitle = true;
        Invoke("QuitToTitle", sceneLoadWait);

        // store (level start) data
        switch ( curRunType ) 
        {
            case RunType.Normal: SaveManager.instance.GetLastLevelData(ref curProgressData.normalRunData); break;
            case RunType.Endless: SaveManager.instance.GetLastLevelData(ref curProgressData.endlessRunData); break;
        }

        // save
        SaveManager.instance.WriteToFile(curProgressData);
    }

    void QuitToTitle ()
    {
        // reset all input
        if (InputManager.instance != null)
        {
            InputManager.instance.DisableAllInput();
        }

        // clear all particles
        ClearAllParticles();

        // decals
        if (DecalManager.instance != null)
        {
            DecalManager.instance.ClearAllDecals();
        }

        // generation seed
        //SetRunSeedLevel();
        TommieRandom.instance.__generation_random_index = 0;
        TommieRandom.instance.__random_index = 0;

        // clear spawners
        ClearAllPropSpawners();
        ClearAllNpcSpawners();
        ClearAllFountainAreas();

        quitToTitle = false;
        SetGameState(GameState.Title);
        SceneManager.LoadScene(1 + 1,LoadSceneMode.Single);
    }

    public void InitQuitGame()
    {
        if (paused)
        {
            TogglePaused();
        }
        SetTransition(TransitionMode.In);
        quitToTitle = true;
        Invoke("QuitGame", sceneLoadWait);
    }

    public void InitQuitToReload ()
    {
        TogglePaused();
        SetTransition(TransitionMode.In);
        quitToReload = true;
        Invoke("QuitToReload", sceneLoadWait);
    }

    void QuitToReload ()
    {
        // reset all input
        if (InputManager.instance != null)
        {
            InputManager.instance.DisableAllInput();
        }

        // clear all particles
        ClearAllParticles();

        // decals
        if (DecalManager.instance != null)
        {
            DecalManager.instance.ClearAllDecals();
        }

        // generation seed
        //TommieRandom.instance.SetGenerationSeedHard(0);
        //TommieRandom.instance.__generation_random_index = 0;

        // clear spawners
        ClearAllPropSpawners();
        ClearAllNpcSpawners();
        ClearAllFountainAreas();

        quitToReload = false;
        Destroy(myGameObject);
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void RequestProceedToOutro ()
    {
        Invoke("InitProceedToOutro",6f);
    }

    public void InitProceedToOutro ()
    {
        SetTransition(TransitionMode.In);
        Invoke("ProceedToOutro",sceneLoadWait);
    }

    void ProceedToOutro ()
    {
        // reset all input
        if (InputManager.instance != null)
        {
            InputManager.instance.DisableAllInput();
        }

        // clear all particles
        ClearAllParticles();

        // decals
        if (DecalManager.instance != null)
        {
            DecalManager.instance.ClearAllDecals();
        }

        // clear spawners
        ClearAllPropSpawners();
        ClearAllNpcSpawners();
        ClearAllFountainAreas();

        // unblessed?
        if ( runDataRead.blessingsClaimed.Count <= 0 )
        {
            AchievementHelper.UnlockAchievement("ACHIEVEMENT_UNBLESSED");
        }

        // go
        SetGameState(GameState.Outro);
        SceneManager.LoadScene(9 + 1, LoadSceneMode.Single);
    }

    void QuitGame()
    {
        // reset all input
        if (InputManager.instance != null)
        {
            InputManager.instance.DisableAllInput();
        }

        // clear all particles
        ClearAllParticles();

        // decals
        if (DecalManager.instance != null)
        {
            DecalManager.instance.ClearAllDecals();
        }

        // clear spawners
        ClearAllPropSpawners();
        ClearAllNpcSpawners();
        ClearAllFountainAreas();

        Application.Quit();
    }

    public void SetRippleEffect(Vector2 _pos, float _amount)
    {
        rippleAmount = _amount;
        ripplePosition = _pos;
    }

    void UpdateRippleEffect()
    {
        if (curProgressData.settingsData.wobbleEffect == 1)
        {
            if (!inFreeze || paused || (tutorialPopupCounter < tutorialPopupDur))
            {
                rippleAmount *= rippleFriction;
                Shader.SetGlobalVector("_RipplePosition", ripplePosition);
                Shader.SetGlobalFloat("_RippleAmount", rippleAmount);
            }
        }
        else
        {
            rippleAmount = 0f;
            Shader.SetGlobalVector("_RipplePosition", ripplePosition);
            Shader.SetGlobalFloat("_RippleAmount", 0f);
        }
    }

    public void CheckFireChance ()
    {
        // define fire chance
        switch (runDataRead.curFloorIndex)
        {
            // SEWER
            case 1:
                float sewerFireSpawnChanceAdd = 0f;
                switch (runDataRead.encountersHad)
                {
                    case 1: sewerFireSpawnChanceAdd = 0f; break;
                    case 2: sewerFireSpawnChanceAdd = .075f; break;
                    case 3: sewerFireSpawnChanceAdd = .15f; break;
                }
                if (runDataRead.encountersHad > 3)
                {
                    sewerFireSpawnChanceAdd = .2f;
                }
                fireSpawnChance = (.075f + sewerFireSpawnChanceAdd);
                break;

            // DUNGEON
            case 2:
                float dungeonFireSpawnChanceAdd = 0f;
                switch (runDataRead.encountersHad)
                {
                    case 1: dungeonFireSpawnChanceAdd = 0f; break;
                    case 2: dungeonFireSpawnChanceAdd = .1f; break;
                    case 3: dungeonFireSpawnChanceAdd = .2f; break;
                }
                if (runDataRead.encountersHad > 3)
                {
                    dungeonFireSpawnChanceAdd = .2f;
                }
                fireSpawnChance = (.25f + dungeonFireSpawnChanceAdd);
                break;

            // HELL
            case 3:
                float hellFireSpawnChanceAdd = 0f;
                switch (runDataRead.encountersHad)
                {
                    case 1: hellFireSpawnChanceAdd = 0f; break;
                    case 2: hellFireSpawnChanceAdd = .1f; break;
                    case 3: hellFireSpawnChanceAdd = .2f; break;
                }
                if (runDataRead.encountersHad > 3)
                {
                    hellFireSpawnChanceAdd = .2f;
                }
                fireSpawnChance = (.575f + hellFireSpawnChanceAdd);
                break;
        }
        fireSpawnChance *= .675f;
        //fireSpawnChance = .5f;
    }

    void UpdateMusic ()
    {
        float musicVolLerpie = 2.5f;
        float musicStopThreshold = .025f;

        float ambienceVolLerpie = 2.5f;
        float ambienceStopThreshold = .025f;

        // shop music
        if ( shopMusicAudioSource != null )
        {
            bool playShopMusic = (curGameState == GameState.Shop || curGameState == GameState.ShopBrowse);
            if (!playShopMusic)
            {
                if (shopMusicAudioSource.volume <= musicStopThreshold)
                {
                    shopMusicAudioSource.Stop();
                }
            }
            else
            {
                if (!shopMusicAudioSource.isPlaying)
                {
                    shopMusicAudioSource.Play();
                }
            }
            float shopMusicVolTarget = ( playShopMusic ) ? .75f : 0f;
            shopMusicAudioSource.volume = Mathf.Lerp(shopMusicAudioSource.volume,shopMusicVolTarget * musicVolFactor,musicVolLerpie * Time.deltaTime);
        }

        // rest music
        if ( restMusicAudioSource != null )
        {
            bool playRestMusic = (curGameState == GameState.Rest);
            if (!playRestMusic)
            {
                if (restMusicAudioSource.volume <= musicStopThreshold)
                {
                    restMusicAudioSource.Stop();
                }
            }
            else
            {
                if (!restMusicAudioSource.isPlaying)
                {
                    restMusicAudioSource.Play();
                }
            }
            float restMusicVolTarget = (playRestMusic) ? .25f : 0f;
            restMusicAudioSource.volume = Mathf.Lerp(restMusicAudioSource.volume, restMusicVolTarget * musicVolFactor, musicVolLerpie * Time.deltaTime);
        }

        // fountain music
        if (fountainMusicAudioSource != null)
        {
            bool playFountainMusic = (playerInFountainArea);
            if (!playFountainMusic)
            {
                if (fountainMusicAudioSource.volume <= musicStopThreshold)
                {
                    fountainMusicAudioSource.Stop();
                }
            }
            else
            {
                if (!fountainMusicAudioSource.isPlaying)
                {
                    fountainMusicAudioSource.Play();
                }
            }
            float fountainLerpieFac = (playFountainMusic) ? 1.5f : .25f;
            float fountainMusicVolTarget = (playFountainMusic) ? .5f : 0f;
            fountainMusicAudioSource.volume = Mathf.Lerp(fountainMusicAudioSource.volume, fountainMusicVolTarget * musicVolFactor, (musicVolLerpie * fountainLerpieFac) * Time.deltaTime);
        }

        // level completed
        if ( levelCompletedMusicAudioSource != null )
        {
            bool playLevelCompletedMusic = (curGameState == GameState.LevelEnd || curGameState == GameState.LevelSelect);
            if ( !playLevelCompletedMusic )
            {
                if ( levelCompletedMusicAudioSource.volume <= musicStopThreshold )
                {
                    levelCompletedMusicAudioSource.Stop();
                }
            }
            else
            {
                if ( !levelCompletedMusicAudioSource.isPlaying )
                {
                    levelCompletedMusicAudioSource.Play();
                }
            }
            float levelCompletedMusicVolTarget = (playLevelCompletedMusic) ? .2f : 0f;
            levelCompletedMusicAudioSource.volume = Mathf.Lerp(levelCompletedMusicAudioSource.volume, levelCompletedMusicVolTarget * musicVolFactor, musicVolLerpie * Time.deltaTime);
        }

        // game over
        if (gameOverMusicAudioSource != null)
        {
            bool playGameOverMusic = (curGameState == GameState.GameOver);
            if (!playGameOverMusic)
            {
                if (gameOverMusicAudioSource.volume <= musicStopThreshold)
                {
                    gameOverMusicAudioSource.Stop();
                }
            }
            else
            {
                if (!gameOverMusicAudioSource.isPlaying)
                {
                    gameOverMusicAudioSource.Play();
                }
            }
            float gameOverMusicVolTarget = (playGameOverMusic) ? .375f : 0f;
            gameOverMusicAudioSource.volume = Mathf.Lerp(gameOverMusicAudioSource.volume, gameOverMusicVolTarget * musicVolFactor, musicVolLerpie * Time.deltaTime);
        }

        // title
        if (titleMusicAudioSource != null)
        {
            bool playTitleMusic = (curGameState == GameState.Title || curGameState == GameState.Intro);
            if ( IntroManager.instance != null && IntroManager.instance.waiting )
            {
                playTitleMusic = false;
            }
            if (!playTitleMusic)
            {
                if (titleMusicAudioSource.volume <= musicStopThreshold)
                {
                    titleMusicAudioSource.Stop();
                }
            }
            else
            {
                if (!titleMusicAudioSource.isPlaying)
                {
                    titleMusicAudioSource.Play();
                }
            }
            float titleMusicVolTarget = (playTitleMusic) ? .625f : 0f;
            titleMusicAudioSource.volume = Mathf.Lerp(titleMusicAudioSource.volume, titleMusicVolTarget * musicVolFactor, musicVolLerpie * Time.deltaTime);
        }

        // outro
        if (outroMusicAudioSource != null)
        {
            bool playOutroMusic = (curGameState == GameState.Outro );
            if (!playOutroMusic)
            {
                if (outroMusicAudioSource.volume <= musicStopThreshold)
                {
                    outroMusicAudioSource.Stop();
                }
            }
            else
            {
                if (!outroMusicAudioSource.isPlaying)
                {
                    outroMusicAudioSource.Play();
                }
            }
            float outroMusicVolTarget = (playOutroMusic) ? .425f : 0f;
            outroMusicAudioSource.volume = Mathf.Lerp(outroMusicAudioSource.volume, outroMusicVolTarget * musicVolFactor, musicVolLerpie * Time.deltaTime);
        }

        // sewer ambience
        if (sewerAmbienceAudioSource != null)
        {
            bool playSewerAmbience = (curGameState == GameState.Level && runDataRead.curFloorIndex == 1);
            if (!playSewerAmbience)
            {
                if (sewerAmbienceAudioSource.volume <= ambienceStopThreshold)
                {
                    sewerAmbienceAudioSource.Stop();
                }
            }
            else
            {
                if (!sewerAmbienceAudioSource.isPlaying)
                {
                    sewerAmbienceAudioSource.Play();
                }
            }
            float sewerAmbienceVolTarget = (playSewerAmbience) ? .125f : 0f;
            sewerAmbienceAudioSource.volume = Mathf.Lerp(sewerAmbienceAudioSource.volume, sewerAmbienceVolTarget * sfxVolFactor, ambienceVolLerpie * Time.deltaTime);
        }

        // dungeon ambience
        if (dungeonAmbienceAudioSource != null)
        {
            bool playDungeonAmbience = (curGameState == GameState.Level && runDataRead.curFloorIndex == 2);
            if (!playDungeonAmbience)
            {
                if (dungeonAmbienceAudioSource.volume <= ambienceStopThreshold)
                {
                    dungeonAmbienceAudioSource.Stop();
                }
            }
            else
            {
                if (!dungeonAmbienceAudioSource.isPlaying)
                {
                    dungeonAmbienceAudioSource.Play();
                }
            }
            float dungeonAmbienceVolTarget = (playDungeonAmbience) ? .125f : 0f;
            dungeonAmbienceAudioSource.volume = Mathf.Lerp(dungeonAmbienceAudioSource.volume, dungeonAmbienceVolTarget * sfxVolFactor, ambienceVolLerpie * Time.deltaTime);
        }

        // hell ambience
        if (hellAmbienceAudioSource != null)
        {
            bool playHellAmbience = (curGameState == GameState.Level && runDataRead.curFloorIndex == 3);
            if (!playHellAmbience)
            {
                if (hellAmbienceAudioSource.volume <= ambienceStopThreshold)
                {
                    hellAmbienceAudioSource.Stop();
                }
            }
            else
            {
                if (!hellAmbienceAudioSource.isPlaying)
                {
                    hellAmbienceAudioSource.Play();
                }
            }
            float hellAmbienceVolTarget = (playHellAmbience) ? .125f : 0f;
            hellAmbienceAudioSource.volume = Mathf.Lerp(hellAmbienceAudioSource.volume, hellAmbienceVolTarget * sfxVolFactor, ambienceVolLerpie * Time.deltaTime);
        }

        // hide music?
        if ( hideMusic )
        {
            shopMusicAudioSource.volume = 0f;
            restMusicAudioSource.volume = 0f;
            levelCompletedMusicAudioSource.volume = 0f;
            gameOverMusicAudioSource.volume = 0f;
            titleMusicAudioSource.volume = 0f;
        }
    }

    void UpdateResolution()
    {
        if (allowHacks)
        {
            bool changeResolution = false;
            bool resolutionUp = InputManager.instance.resolutionUpHackPressed;
            bool resolutionDown = InputManager.instance.resolutionDownHackPressed;
            bool toggleFullscreen = InputManager.instance.fullscreenHackPressed;

            if (resolutionUp)
            {
                resolutionIndex++;
                if (resolutionIndex > (supportedResolutions.Length - 1))
                {
                    resolutionIndex = 0;
                }
                changeResolution = true;
            }
            if (resolutionDown)
            {
                resolutionIndex--;
                if (resolutionIndex < 0)
                {
                    resolutionIndex = supportedResolutions.Length - 1;
                }
                changeResolution = true;
            }
            if (changeResolution)
            {
                Screen.SetResolution(supportedResolutions[resolutionIndex].width, supportedResolutions[resolutionIndex].height, Screen.fullScreen);
                //Screen.SetResolution(1920,1080,false);
            }

            // toggle fullscreen
            if (toggleFullscreen)
            {
                Screen.fullScreen = !Screen.fullScreen;
            }
        }
    }

    void UpdateFreeze()
    {
        if (GameManager.instance != null && runDataRead.playerDead)
        {
            SetFreeze(120);
        }
        if (GameManager.instance != null && GameManager.instance.inGameWait)
        {
            SetFreeze(1);
        }
        if (quitToTitle)
        {
            SetFreeze(1);
        }

        if (freezeCounter < freezeDur)
        {
            freezeCounter++;
            inFreeze = true;
        }
        else
        {
            if (inFreeze)
            {
                UnfreezeParticles();
                inFreeze = false;
            }
        }
    }

    public void SetFreeze(int _dur)
    {
        freezeDur = _dur;
        freezeCounter = 0;
        inFreeze = true;
        FreezeParticles();
    }

    void UpdateScreenShake ()
    {
        if (screenShakeCounter < screenShakeDur)
        {
            screenShakeCounter++;
            inScreenShake = true;
        }
        else
        {
            if (inScreenShake)
            {
                inScreenShake = false;
            }
        }
    }

    public void SetScreenShake ( int _dur )
    {
        screenShakeDur = _dur;
        screenShakeCounter = 0;
        inScreenShake = true;
    }

    public void SetPlayerInvulnerable ( int _dur )
    {
        playerInvulnerableDur = _dur;
        playerInvulnerableCounter = 0;
        playerInvulnerable = true;
    }

    public void InitStartTransition ()
    {
        Invoke("InvokeStartTransition",sceneLoadWait);

        // log
        //Debug.Log("start die transitie maar || " + Time.time.ToString());
    }

    void InvokeStartTransition()
    {
        transitionFactor = 0f;
        curTransitionMode = TransitionMode.In;
        SetTransition(TransitionMode.Out);

        // log
        //Debug.Log("daar gaan we dan, transitie tijd || " + Time.time.ToString());
    }

    public void TogglePaused ()
    {
        if ( paused )
        {
            if (menuManager != null)
            {
                switch (menuManager.curMenuState)
                {
                    case MenuManager.MenuState.Default:
                        menuManager.ClearSettings();
                        paused = false;

                        // audio
                        PlayUIBackSound();
                        break;

                    case MenuManager.MenuState.Settings:
                        menuManager.ClearSettings();
                        menuManager.SetMenuState(MenuManager.MenuState.Default);
                        menuManager.CreateSettings();

                        // audio
                        PlayUIBackSound();
                        break;
                }
            }
        }
        else
        {
            menuManager.SetMenuState(MenuManager.MenuState.Default);
            menuManager.CreateSettings();
            paused = true;

            // audio
            PlayUISelectSound();
        }
    }

    void UpdateTransition()
    {
        if (inTransition)
        {
            float transitionSpd = (2.5f * Time.deltaTime);
            if ( fastMode )
            {
                transitionSpd = (100f * Time.deltaTime);
            }

            float transitionDir = (transitionFactor > transitionFactorTarget) ? -1f : 1f;
            float transitionThreshold = .025f;
            if (!BasicFunctions.IsApproximately(transitionFactor, transitionFactorTarget, transitionThreshold))
            {
                transitionFactor += (transitionSpd * transitionDir);
            }
            else
            {
                transitionFactor = transitionFactorTarget;
                inTransition = false;
            }
        }
    }

    public void SetTransition(TransitionMode _mode)
    {
        //if (curTransitionMode != _mode)
        {
            switch (_mode)
            {
                case TransitionMode.In: transitionFactorTarget = 0f; break;
                case TransitionMode.Out: transitionFactorTarget = 1.5f; break;
            }
            curTransitionMode = _mode;
            inTransition = true;
        }

        // log
        //Debug.Log("hopla zet die transitie naar: " + curTransitionMode + " || inTransition: " + inTransition + " || " + Time.time.ToString());
    }

    public void Restart ()
    {
        // reset input
        InputManager.instance.DisableAllInput();

        // clear all particles?
        ClearAllParticles();

        // clear decals?
        if (DecalManager.instance != null)
        {
            DecalManager.instance.ClearAllDecals();
        }

        // clear spawners
        ClearAllPropSpawners();
        ClearAllNpcSpawners();
        ClearAllFountainAreas();

        // go
        transitionFactor = 0f;
        SetTransition(TransitionMode.In);
        inTransition = false;
        //SceneManager.LoadScene(1);
        SceneManager.LoadScene(3,LoadSceneMode.Single);
    }

    public void Quit ()
    {
        Application.Quit();
    }

    public void SetGameState ( GameState _to )
    {
        curGameState = _to;
    }

    public void SetEncounterType ( EncounterType _to )
    {
        curEncounterType = _to;
    }

    public void FreezeParticles()
    {
        if (allParticleSystems != null && allParticleSystems.Count > 0)
        {
            for (int i = 0; i < allParticleSystems.Count; i++)
            {
                allParticleSystems[i].Pause();
            }
        }
    }

    public void UnfreezeParticles()
    {
        if (allParticleSystems != null && allParticleSystems.Count > 0)
        {
            for (int i = 0; i < allParticleSystems.Count; i++)
            {
                allParticleSystems[i].Play();
            }
        }
    }

    public void ClearAllParticles()
    {
        if (allParticleSystems != null && allParticleSystems.Count > 0)
        {
            allParticleSystems.Clear();
        }
    }

    public void ClearAllPropSpawners ()
    {
        if ( allPropSpawners != null && allPropSpawners.Count > 0 )
        {
            for ( int i = allPropSpawners.Count - 1; i >= 0; i -- )
            {
                if (allPropSpawners[i] != null)
                {
                    allPropSpawners[i].Clear();
                }
            }
            allPropSpawners.Clear();
        }
    }

    public void ClearAllNpcSpawners ()
    {
        if (allNpcSpawners != null && allNpcSpawners.Count > 0)
        {
            for (int i = allNpcSpawners.Count - 1; i >= 0; i--)
            {
                if (allNpcSpawners[i] != null)
                {
                    allNpcSpawners[i].Clear();
                }
            }
            allNpcSpawners.Clear();
        }
    }

    public void ClearAllFountainAreas ()
    {
        if (allFountainAreas != null && allFountainAreas.Count > 0)
        {
            for (int i = allFountainAreas.Count - 1; i >= 0; i--)
            {
                if (allFountainAreas[i] != null)
                {
                    allFountainAreas[i].Clear();
                }
            }
            allFountainAreas.Clear();
        }
    }

    public void InitLoadLevelScene ()
    {
        GameManager.instance.PlayerReachedEnd(false);
        SetTransition(TransitionMode.In);
        Invoke("LoadLevelScene",1f);
    }

    public void LoadLevelScene ()
    {
        ClearAllParticles();
        SetGameState(GameState.Level);
        SceneManager.LoadScene(2,LoadSceneMode.Single);
    }

    public bool CheckIfBlessingClaimed ( BlessingDatabase.Blessing _blessing )
    {
        bool ret = false;
        for ( int i = 0; i < runDataRead.blessingsClaimed.Count; i ++ )
        {
            if ( (int)(runDataRead.blessingsClaimed[i]) == (int)(_blessing) )
            {
                return true;
            }
        }
        return ret;
    }

    public bool CheckIfItemSpecialActive(EquipmentDatabase.Specials _special)
    {
        bool foundSpecial = false;
        float chance = 100f;
        if (playerEquipmentSpecialPackages != null && playerEquipmentSpecialPackages.Count > 0)
        {
            for ( int i = 0; i < playerEquipmentSpecialPackages.Count; i ++ )
            {
                if ( playerEquipmentSpecialPackages[i].special == _special )
                {
                    chance = playerEquipmentSpecialPackages[i].chance;
                    foundSpecial = true;
                    break;
                }
            }
        }
        if ( !foundSpecial )
        {
            return false;
        }
        float random = TommieRandom.instance.RandomValue(100f);
        float sneakyBonus = 10f;
        return ((random + sneakyBonus) <= chance);
    }

    public bool CheckIfHasEquipment ( EquipmentDatabase.Equipment _equipment )
    {
        if (runDataRead.curBodyEquipmentIndex == (int)(_equipment) )
        {
            return true;
        }
        if (runDataRead.curRingEquipmentIndex == (int)(_equipment))
        {
            return true;
        }
        if (runDataRead.curBraceletEquipmentIndex == (int)(_equipment))
        {
            return true;
        }
        if (runDataRead.curWeaponEquipmentIndex == (int)(_equipment))
        {
            return true;
        }
        return false;
    }

    public void AddPlayerMaxHealth(int _add)
    {
        int oldHealthMax = runDataRead.playerHealthMax;
        switch ( curRunType )
        {
            case RunType.Normal:
                curProgressData.normalRunData.playerHealthMax += _add;
                curProgressData.normalRunData.playerHealthCur += _add;
                if (!CheckIfBlessingClaimed(BlessingDatabase.Blessing.Resilient) && curProgressData.normalRunData.playerHealthCur > oldHealthMax)
                {
                    curProgressData.normalRunData.playerHealthCur = curProgressData.normalRunData.playerHealthMax;
                }
                if (curProgressData.normalRunData.playerHealthMax < 1)
                {
                    curProgressData.normalRunData.playerHealthMax = 1;
                }
                if (curProgressData.normalRunData.playerHealthCur < 1)
                {
                    curProgressData.normalRunData.playerHealthCur = 1;
                }
                break;

            case RunType.Endless:
                curProgressData.endlessRunData.playerHealthMax += _add;
                curProgressData.endlessRunData.playerHealthCur += _add;
                if (!CheckIfBlessingClaimed(BlessingDatabase.Blessing.Resilient) && curProgressData.endlessRunData.playerHealthCur > oldHealthMax)
                {
                    curProgressData.endlessRunData.playerHealthCur = curProgressData.endlessRunData.playerHealthMax;
                }
                if (curProgressData.endlessRunData.playerHealthMax < 1)
                {
                    curProgressData.endlessRunData.playerHealthMax = 1;
                }
                if (curProgressData.endlessRunData.playerHealthCur < 1)
                {
                    curProgressData.endlessRunData.playerHealthCur = 1;
                }
                break;
        }
    }

    public void AddPlayerMaxFire ( int _add )
    {
        int oldFireMax = runDataRead.playerFireMax;
        switch (curRunType)
        {
            case RunType.Normal:
                curProgressData.normalRunData.playerFireMax += _add;
                //curProgressData.normalRunData.playerFireCur += _add;
                if (curProgressData.normalRunData.playerFireCur > oldFireMax)
                {
                    curProgressData.normalRunData.playerFireCur = curProgressData.normalRunData.playerFireMax;
                }
                if (curProgressData.normalRunData.playerFireMax < 1)
                {
                    curProgressData.normalRunData.playerFireMax = 1;
                }
                //if (curProgressData.normalRunData.playerFireCur < 1)
                //{
                //    curProgressData.normalRunData.playerFireCur = 1;
                //}
                break;

            case RunType.Endless:
                curProgressData.endlessRunData.playerFireMax += _add;
                //curProgressData.endlessRunData.playerFireCur += _add;
                if (curProgressData.endlessRunData.playerFireCur > oldFireMax)
                {
                    curProgressData.endlessRunData.playerFireCur = curProgressData.endlessRunData.playerFireMax;
                }
                if (curProgressData.endlessRunData.playerFireMax < 1)
                {
                    curProgressData.endlessRunData.playerFireMax = 1;
                }
                //if (curProgressData.endlessRunData.playerFireCur < 1)
                //{
                //    curProgressData.endlessRunData.playerFireCur = 1;
                //}
                break;
        }
    }

    public string GetRunPlayTime ( RunData _runData )
    {
        float time = _runData.runTime;

        string ret = "";
        int hours = (int)(time / 3600f);
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);
        //int milliseconds = (int)(_time * 1000f) % 1000;
        for (int i = 0; i < hours; i++)
        {
            minutes -= 60;
        }
        if (hours > 0)
        {
            ret += hours.ToString() + "h ";
        }
        ret += minutes.ToString() + "m ";
        ret += seconds.ToString() + "s ";
        return ret;
    }

    public void SetHideUI ( int _dur )
    {
        hideUIDur = _dur;
        hideUICounter = 0;
        hideUI = true;
    }

    public EncounterArchetypeData GetEncounterArchetypeData ( EncounterArchetype _encounterArchetype )
    {
        return encounterArchetypes[_encounterArchetype];
    }

    void OnApplicationQuit ()
    {
        // reset all input
        if (InputManager.instance != null)
        {
            InputManager.instance.DisableAllInput();
        }

        // clear all particles
        ClearAllParticles();

        // decals
        if (DecalManager.instance != null)
        {
            DecalManager.instance.ClearAllDecals();
        }

        // generation seed
        //TommieRandom.instance.__random_index = 0;
        //TommieRandom.instance.__generation_random_index = 0;
        //TommieRandom.instance.SetSeed(0);
        //TommieRandom.instance.SetGenerationSeedHard(0);

        // clear spawners
        ClearAllPropSpawners();
        ClearAllNpcSpawners();
        ClearAllFountainAreas();
    }

    public void SetRunSeedLevel ()
    {
        TommieRandom.instance.SetSeedHard(0);
        switch (curRunType)
        {
            case RunType.Normal: TommieRandom.instance.SetGenerationSeedHard(curProgressData.normalRunData.runSeed + (curProgressData.normalRunData.curFloorIndex * 20) + ((curProgressData.normalRunData.curLevelIndex + 1) * 20)); break;
            case RunType.Endless: TommieRandom.instance.SetGenerationSeedHard(curProgressData.endlessRunData.runSeed + ((curProgressData.endlessRunData.curLoopIndex + 1) * 20) + (curProgressData.endlessRunData.curFloorIndex * 20) + ((curProgressData.endlessRunData.curLevelIndex + 1) * 20)); break;
        }
    }

    public void SetRunSeedNotLevel ()
    {
        TommieRandom.instance.SetSeedHard(0);
        switch (curRunType)
        {
            case RunType.Normal: TommieRandom.instance.SetGenerationSeedHard(curProgressData.normalRunData.runSeed + (curProgressData.normalRunData.curFloorIndex * 20) + ((curProgressData.normalRunData.curLevelIndex + 1) * 20)); break;
            case RunType.Endless: TommieRandom.instance.SetGenerationSeedHard(curProgressData.endlessRunData.runSeed + ((curProgressData.endlessRunData.curLoopIndex + 1) * 20) + (curProgressData.endlessRunData.curFloorIndex * 20) + ((curProgressData.endlessRunData.curLevelIndex + 1) * 20)); break;
        }
    }

    public void CreateNewRun(RunType _runType)
    {
        // reset some variables
        globalTime = 0f;
        defeatedFinalBoss = false;
        lavaFactor = 1f;

        // in a run!
        switch (_runType)
        {
            case RunType.Normal: curProgressData.persistentData.inNormalRun = true; break;
            case RunType.Endless: curProgressData.persistentData.inEndlessRun = true; break;
        }

        // create new run data
        RunData newRunData = SaveManager.instance.CreateNewRunData();

        // data on (last) level start
        if (_runType == RunType.Normal)
        {
            newRunData.curBodyEquipmentIndex = (int)(EquipmentDatabase.Equipment.BlueCloak);
            newRunData.curWeaponEquipmentIndex = (int)(EquipmentDatabase.Equipment.IronSword);
        }
        if ( _runType == RunType.Endless )
        {
            newRunData.curBodyEquipmentIndex = (int)(EquipmentDatabase.Equipment.WhiteCloak);
        }
        SaveManager.instance.StoreLastLevelData(ref newRunData);

        // store
        switch (_runType)
        {
            case RunType.Normal:
                curProgressData.normalRunData = newRunData;
                break;
            case RunType.Endless:
                curProgressData.endlessRunData = newRunData;
                break;
        }
    }

    public void LoadRun ( RunType _runType )
    {
        RunData loadedRunData = (_runType == RunType.Normal) ? curProgressData.normalRunData : curProgressData.endlessRunData;
        globalTime = loadedRunData.runTime;

        switch (_runType)
        {
            case RunType.Normal: curProgressData.normalRunData = loadedRunData; break;
            case RunType.Endless: curProgressData.endlessRunData = loadedRunData; break;
        }

        UpdateRunDataRead();

        CheckIfHasToEnterNextFloor();

        // log
        //Debug.Log("loaded run weapon: " + curProgressData.normalRunData.curWeaponEquipmentIndex.ToString() + " || " + Time.time.ToString());

        // log
        //Debug.LogError("laad run || " + Time.time.ToString());
    }

    public void SetTutorialPopup ( string _text, int _dur )
    {
        if (canShowTutorialPopupCounter >= canShowTutorialPopupDur)
        {
            tutorialPopupDur = _dur;
            tutorialPopupCounter = 0;
            tutorialPopupString = _text;
            canShowTutorialPopupCounter = 0;

            InputManager.instance.DisableAllInput();
            if (HandManager.instance != null && HandManager.instance.handScripts != null && HandManager.instance.handScripts.Length > 0)
            {
                for (int i = 0; i < HandManager.instance.handScripts.Length; i++)
                {
                    HandManager.instance.StopMagicChargeAudio();
                    HandManager.instance.handScripts[i].StopCharge();
                }
            }

            // audio
            AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.popupOpenClips),.9f,1.2f,.5f,.525f);
        }
    }

    public void CloseTutorialPopup ()
    {
        tutorialPopupCounter = tutorialPopupDur;

        // audio
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.popupCloseClips), .6f, .9f, .5f, .525f);
    }

    public void SetPopup ()
    {

    }

    public void CheckIfReachedNewHighest ()
    {
        switch (SetupManager.instance.curRunType)
        {
            case SetupManager.RunType.Normal:

                if (SetupManager.instance.curProgressData.normalRunData.curFloorIndex > SetupManager.instance.curProgressData.persistentData.normalHighestFloorIndex) // didn't reach this floor before?
                {
                    SetupManager.instance.curProgressData.persistentData.normalHighestFloorIndex = SetupManager.instance.curProgressData.normalRunData.curFloorIndex;
                    SetupManager.instance.curProgressData.persistentData.normalHighestLevelIndex = SetupManager.instance.curProgressData.normalRunData.curLevelIndex;
                }
                else
                {
                    if (SetupManager.instance.curProgressData.normalRunData.curLevelIndex > SetupManager.instance.curProgressData.persistentData.normalHighestLevelIndex) // reached a higher level?
                    {
                        SetupManager.instance.curProgressData.persistentData.normalHighestLevelIndex = SetupManager.instance.curProgressData.normalRunData.curLevelIndex;
                    }
                }

                break;

            case SetupManager.RunType.Endless:

                if (SetupManager.instance.curProgressData.endlessRunData.curLoopIndex > SetupManager.instance.curProgressData.persistentData.endlessHighestLoopIndex)
                {
                    SetupManager.instance.curProgressData.persistentData.endlessHighestLoopIndex = SetupManager.instance.curProgressData.endlessRunData.curLoopIndex;
                    SetupManager.instance.curProgressData.persistentData.endlessHighestFloorIndex = SetupManager.instance.curProgressData.endlessRunData.curFloorIndex;
                    SetupManager.instance.curProgressData.persistentData.endlessHighestLevelIndex = SetupManager.instance.curProgressData.endlessRunData.curLevelIndex;
                }
                else
                {
                    if (SetupManager.instance.curProgressData.endlessRunData.curFloorIndex > SetupManager.instance.curProgressData.persistentData.endlessHighestFloorIndex) // didn't reach this floor before?
                    {
                        SetupManager.instance.curProgressData.persistentData.endlessHighestFloorIndex = SetupManager.instance.curProgressData.endlessRunData.curFloorIndex;
                        SetupManager.instance.curProgressData.persistentData.endlessHighestLevelIndex = SetupManager.instance.curProgressData.endlessRunData.curLevelIndex;
                    }
                    else
                    {
                        if (SetupManager.instance.curProgressData.endlessRunData.curLevelIndex > SetupManager.instance.curProgressData.persistentData.endlessHighestLevelIndex) // reached a higher level?
                        {
                            SetupManager.instance.curProgressData.persistentData.endlessHighestLevelIndex = SetupManager.instance.curProgressData.endlessRunData.curLevelIndex;
                        }
                    }
                }

                break;
        }
    }

    public void CheckIfHasToEnterNextFloor ()
    {
        if (runDataRead.curLevelIndex >= (runDataRead.curFloorData.locationCount))
        {
            switch (curRunType)
            {
                case RunType.Normal:
                    curProgressData.normalRunData.curFloorIndex++;
                    curProgressData.normalRunData.curLevelIndex = 0;
                    curProgressData.normalRunData.encountersHad = 0;

                    // dungeon achievement
                    if ( curProgressData.normalRunData.curFloorIndex >= 2 )
                    {
                        AchievementHelper.UnlockAchievement("ACHIEVEMENT_DUNGEON");
                    }

                    // hell achievement
                    if (curProgressData.normalRunData.curFloorIndex >= 3)
                    {
                        AchievementHelper.UnlockAchievement("ACHIEVEMENT_HELL");
                    }
                    break;

                case RunType.Endless:
                    curProgressData.endlessRunData.curFloorIndex++;
                    curProgressData.endlessRunData.curLevelIndex = 0;
                    curProgressData.endlessRunData.encountersHad = 0;
                    break;
            }
        }
    }

    public void SetLaserAttackInPlay ()
    {
        laserAttackIsInPlayCounter = 0;
        laserAttackIsInPlay = true;
    }

    public void PlayUITextSound()
    {
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.UINavigateClips), 1f, 1.4f, .0675f, .075f);
    }

    public void PlayUINavigateSound ()
    {
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.UINavigateClips),.6f,.9f,.4f,.425f);
    }

    public void PlayUISelectSound()
    {
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.UISelectClips), .6f, .9f, .25f, .275f);
    }

    public void PlayUILevelSelectSound()
    {
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.UILevelSelectClips), .6f, .9f, .3f, .325f);
    }

    public void PlayUIBackSound()
    {
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.UIBackClips), .6f, .9f, .25f, .275f);
    }
}
