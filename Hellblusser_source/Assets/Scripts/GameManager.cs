using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // player
    [Header("player")]
    public FirstPersonDrifter playerFirstPersonDrifter;

    // camera
    [Header("camera")]
    public Transform cameraTransform;

    // cleared all fires
    [HideInInspector] public bool clearedAllFires;

    // booleans for triggering tutorial popups
    [HideInInspector] public bool fireCollected;
    [HideInInspector] public bool coinCollected;
    [HideInInspector] public bool tearCollected;
    [HideInInspector] public bool donutCollected;
    [HideInInspector] public bool keyCollected;
    [HideInInspector] public bool inRest;
    [HideInInspector] public bool inShop;
    [HideInInspector] public bool secondChanceTriggered;

    // wait
    int gameWaitDur, gameWaitCounter;
    [HideInInspector] public bool inGameWait;
    [HideInInspector] public int countdownRate, countdownCounter, countdownIndex, countdownIndexMax;
    [HideInInspector] public bool inCountdown;
    [HideInInspector] public bool showingLevelIntro;

    // interaction
    public enum InteractionType { LevelEndDoor, Fountain, FountainDoor, StartDoor, ShopDoor, ShopItem, RestCampfire, RestDoor };
    [HideInInspector] public InteractionType curInteractionType;
    [HideInInspector] public string interactionString;
    int interactionCheckDur, interactionCheckCounter;
    [HideInInspector] public int canCheckInteractionDur, canCheckInteractionCounter;
    [HideInInspector] public bool inInteraction;
    [HideInInspector] public InteractionScript curInteractionScript;

    // shop inspect
    [HideInInspector] public string shopInspectString;
    int shopInspectDur, shopInspectCounter;
    [HideInInspector] public bool inShopInspect;

    // player health
    int playerCriticalHealthFlickerIndex, playerCriticalHealthFlickerRate, playerCriticalHealthFlickerCounter;

    // UI
    [Header("UI")]
    public Text playerHealthText;
    public Image playerHealthImage;
    public Text fireLeftText;
    public Text playerFireCountText;
    public Text playerFireInfoText;
    public Image fireLeftCheckImage;
    public Image playerFireImage;
    public RectTransform playerFireRectTransform;
    public RectTransform playerFireTextRectTransform;
    public Image doorIconBackImage;
    public Image doorIconFrontImage;
    public Transform mainCameraTransform;
    public Camera mainCamera;
    public Camera uiCamera;
    public Canvas mainCanvas;
    public RectTransform mainCanvasRectTransform;
    public Color doorIconBackColOn, doorIconBackColOff;
    public Color playerFireBackColNormal, playerFireTextColNormal;
    public Color playerHealthBackColNormal, playerHealthTextColNormal;
    public Text playerFireButtonText;
    public Color playerFireButtonColNormal;
    public Color whiteCol, blackCol;
    public Sprite fireOutSprite;
    public Sprite fireInSprite;
    Vector3 playerFireIconPosOriginal;
    Vector3 playerFireTextPosOriginal;
    float playerFireIconRot;
    public TextMeshProUGUI countdownText;
    public RectTransform countdownRectTransform;
    public Image crosshairImage;
    Vector3 countdownScaleOriginal, countdownScaleTarget, countdownScaleCur, countdownScaleAdd;
    public Image keyInactivePart0;
    public Image keyInactivePart1;
    public Image keyActivePart0;
    public Image keyActivePart1;

    public TextMeshProUGUI pausedText;

    public Image interactionCheckBackImage;
    public TextMeshProUGUI interactionCheckText;

    public Image shopInspectBackImage;
    public TextMeshProUGUI shopInspectText;

    public Image blessingDescriptionBack;
    public TextMeshProUGUI blessingDescriptionText;
    public Image blessingInfoBack;
    public TextMeshProUGUI blessingInfoText;

    public Image shopBrowseDescriptionBack;
    public TextMeshProUGUI shopBrowseDescriptionText;
    public Image shopBrowseInfoBack;
    public TextMeshProUGUI shopBrowseInfoText;
    public Image shopQuitBack;
    public TextMeshProUGUI shopQuitText;

    public Image blessingArrowFrontImage;
    public Image blessingArrowBackImage;
    public Sprite blessingArrowLeftSprite;
    public Sprite blessingArrowRightSprite;
    public RectTransform blessingArrowRectTransform;

    public Image shopBrowseArrowFrontImage;
    public Sprite shopBrowseArrowLeftSprite;
    public Sprite shopBrowseArrowRightSprite;
    public RectTransform shopBrowseArrowRectTransform;

    public Image coinsCollectedImage;
    public TextMeshProUGUI coinsCollectedText;
    public RectTransform coinsCollectedRectTransform;
    Vector3 coinsCollectedPosOriginal, coinsCollectedPosHighlight;

    public Image tearsCollectedImage;
    public TextMeshProUGUI tearsCollectedText;
    public RectTransform tearsCollectedRectTransform;
    Vector3 tearsCollectedPosOriginal, tearsCollectedPosHighlight;

    public TextMeshProUGUI gameOverText;

    public TextMeshProUGUI popupText;
    public Image popupBackImage;

    // player fire change
    [HideInInspector] public int playerFireFlickerIndex, playerFireFlickerRate, playerFireFlickerCounter, playerFireFlickerCountMax, playerFireFlickerCount;
    [HideInInspector] public int playerFireButtonFlickerIndex, playerFireButtonFlickerRate, playerFireButtonFlickerCounter, playerFireButtonFlickerCountMax, playerFireButtonFlickerCount;

    // player health
    [HideInInspector] public int playerHealthFlickerIndex, playerHealthFlickerRate, playerHealthFlickerCounter, playerHealthFlickerCountMax, playerHealthFlickerCount;

    // damage deals
    [HideInInspector] public List<DamageDeal> allDamageDeals;

    // flameable objects
    [HideInInspector] public List<FlameableScript> allFlameableObjects;

    // fire
    [HideInInspector] public List<FireScript> fireScripts;
    [HideInInspector] public int fireLeftCount;

    // player hurt effect
    [HideInInspector] public bool playerHurt;
    [HideInInspector] public bool playerHasKey;
    [HideInInspector] public int playerHurtDur, playerHurtCounter;
    [HideInInspector] public float playerHurtFactorTarget, playerHurtFactorCur, playerHurtFactorBurningTarget, playerHurtFactorBurningCur;
    [HideInInspector] public bool playerInCombat;
    [HideInInspector] public List<NpcCore> playerInCombatWith;

    // compass
    [Header("compass")]
    public Image compassBackImage;
    public Text compassNorthText;
    public Text compassEastText;
    public Text compassSouthText;
    public Text compassWestText;
    public RectTransform compassNorthRectTransform;
    public RectTransform compassEastRectTransform;
    public RectTransform compassSouthRectTransform;
    public RectTransform compassWestRectTransform;
    public Color compassFacingNorthCol, compassFacingEastCol, compassFacingSouthCol, compassFacingWestCol;
    public Color compassNearCol;
    public Color compassFarCol;

    // main camera enable
    int enableMainCamDur, enableMainCamCounter;
    bool enabledMainCam;

    // coins
    [HideInInspector] public List<Vernietigbaar> allVernietigbaarScripts;

    void Awake ()
    {
        instance = this;
    }

    void Start ()
    {
        // flameable objects
        allFlameableObjects = new List<FlameableScript>();

        // state
        switch ( SetupManager.instance.curRunType ) 
        {
            case SetupManager.RunType.Normal:
                SaveManager.instance.GetLastLevelData(ref SetupManager.instance.curProgressData.normalRunData);
                SetupManager.instance.curProgressData.normalRunData.curLevelCoinsCollected = 0;
                SetupManager.instance.curProgressData.normalRunData.curLevelTearsCollected = 0;
                SetupManager.instance.curProgressData.normalRunData.playerReachedEnd = false;
                break;
            case SetupManager.RunType.Endless:
                SaveManager.instance.GetLastLevelData(ref SetupManager.instance.curProgressData.endlessRunData);
                SetupManager.instance.curProgressData.endlessRunData.curLevelCoinsCollected = 0;
                SetupManager.instance.curProgressData.endlessRunData.curLevelTearsCollected = 0;
                SetupManager.instance.curProgressData.endlessRunData.playerReachedEnd = false;
                break;
        }
        playerHasKey = false;

        // update player equipment
        SetupManager.instance.UpdatePlayerEquipment();

        // camera
        if ( mainCamera != null )
        {
            float farClippingUse = ( SetupManager.instance.curEncounterType != SetupManager.EncounterType.Boss ) ? 30f : 60f;
            mainCamera.farClipPlane = farClippingUse;
        }

        // fire
        fireScripts = new List<FireScript>();

        // collected
        //coinsCollected = SetupManager.instance.runDataRead.curRunCoinsCollected;
        //tearsCollected = SetupManager.instance.runDataRead.curRunTearsCollected;

        // player fire
        if (playerFireRectTransform != null)
        {
            playerFireIconPosOriginal = playerFireRectTransform.anchoredPosition;
            playerFireTextPosOriginal = playerFireTextRectTransform.anchoredPosition;
        }

        // player critical health
        playerCriticalHealthFlickerIndex = 0;
        playerCriticalHealthFlickerRate = 8;
        playerCriticalHealthFlickerCounter = 0;

        // damage deals
        allDamageDeals = new List<DamageDeal>();

        // interaction
        canCheckInteractionDur = 2;
        canCheckInteractionCounter = 0;
        interactionCheckDur = 2;
        interactionCheckCounter = interactionCheckDur;
        inInteraction = false;

        // shop inspect
        shopInspectDur = 2;
        shopInspectCounter = shopInspectDur;
        inShopInspect = false;

        // game wait
        gameWaitDur = 35;
        gameWaitCounter = 0;
        inGameWait = true;
        countdownIndex = 0;
        countdownRate = 35;
        countdownCounter = 0;
        inCountdown = true;
        showingLevelIntro = true;

        // tears collected UI
        if ( tearsCollectedRectTransform != null )
        {
            tearsCollectedPosOriginal = tearsCollectedRectTransform.anchoredPosition;
            tearsCollectedPosHighlight = tearsCollectedPosOriginal;
            tearsCollectedPosHighlight.x = 25f;//-550f;//0f;
        }

        // coins collected UI
        if (coinsCollectedRectTransform != null)
        {
            coinsCollectedPosOriginal = coinsCollectedRectTransform.anchoredPosition;
            coinsCollectedPosHighlight = coinsCollectedPosOriginal;
            coinsCollectedPosHighlight.x = 25f;//-550f;//0f;
        }

        // player in combat with
        playerInCombatWith = new List<NpcCore>();

        // fast mode?
        bool skipCountdown = SetupManager.instance.fastMode;//(fastMode || (SetupManager.instance.curGameState == SetupManager.GameState.Fountain || SetupManager.instance.curGameState == SetupManager.GameState.Shop));
        bool skipTransition = (SetupManager.instance.fastMode);
        if ( skipCountdown )
        {
            inGameWait = false;
            showingLevelIntro = false;
            inCountdown = false;

            countdownIndex = 1000;
        }

        if ( skipTransition )
        {
            SetupManager.instance.transitionFactorTarget = 1f;
            SetupManager.instance.transitionFactor = SetupManager.instance.transitionFactorTarget;
        }
        else
        {
            SetupManager.instance.InitStartTransition();
        }

        // vernietigbaar
        allVernietigbaarScripts = new List<Vernietigbaar>();

        // countdown text
        if (countdownRectTransform != null)
        {
            countdownScaleOriginal = countdownRectTransform.localScale;
            countdownScaleTarget = countdownScaleOriginal;
            countdownScaleCur = countdownScaleTarget;
            countdownScaleAdd = Vector3.zero;
        }

        // reset all player input
        if (InputManager.instance != null)
        {
            InputManager.instance.DisableAllInput();
        }

        // enable main camera
        enableMainCamDur = 24;
        enableMainCamCounter = 0;
        enabledMainCam = false;
        //Invoke("EnableMainCamera", .1f);
    }

    void EnableMainCamera ()
    {
        // enable camera?
        if (mainCamera != null)
        {
            mainCamera.enabled = true;
        }
        enabledMainCam = true;
    }

    void Update ()
    {
        if ( !enabledMainCam && ((SetupManager.instance.curGameState == SetupManager.GameState.Level && LevelGeneratorManager.instance.activeLevelGenerator.generatedLevel) || SetupManager.instance.curGameState != SetupManager.GameState.Level) )
        {
            if ( enableMainCamCounter < enableMainCamDur )
            {
                enableMainCamCounter++;
            }
            else
            {
                EnableMainCamera();
            }
        }

        // popups
        CheckForTutorialPopups();

        if (SetupManager.instance.allowHacks)
        {
            // add key hack
            if (InputManager.instance.keyHackPressed)
            {
                PlayerFoundKey();
            }

            // add coin hack?
            if (InputManager.instance.coinHackPressed)
            {
                AddCoinAmount(1);
            }

            // add tear hack?
            if (InputManager.instance.tearHackPressed)
            {
                AddTearAmount(1);
            }

            // time hack
            if (InputManager.instance.timeHackPressed)
            {
                Time.timeScale = (Time.timeScale == 1f) ? 0f : 1f;
            }
        }

        // player hurt factor
        playerHurt = (playerHurtCounter < playerHurtDur || SetupManager.instance.runDataRead.playerDead);
        if ( playerHurtCounter < playerHurtDur )
        {
            if (!SetupManager.instance.inFreeze)
            {
                playerHurtCounter++;
            }
        }
        if ( !playerHurt || !SetupManager.instance.inFreeze )
        {
            playerHurtFactorTarget = 1f;
        }
        playerHurtFactorCur = Mathf.Lerp(playerHurtFactorCur,playerHurtFactorTarget,20f * Time.deltaTime);

        float playerBurnP = BasicFunctions.ConvertRange(SetupManager.instance.playerBurnDamageCounter,0f,SetupManager.instance.playerBurnDamageRate,0f,1f);
        playerHurtFactorBurningTarget = (playerBurnP * .05f);
        playerHurtFactorBurningCur = playerHurtFactorBurningTarget;
        //Debug.Log("playerHurtFactorBurningCur: " + playerHurtFactorBurningCur.ToString() + " || " + Time.time.ToString());
        //Debug.Log("player hurt: " + playerHurt + " || hurtCounter: " + playerHurtCounter.ToString() + "/" + playerHurtDur.ToString() + " || " + Time.time.ToString());

        // player in combat?
        playerInCombat = ( playerInCombatWith != null && playerInCombatWith.Count > 0 );
        //Debug.Log("playerInCombat: " + playerInCombat + " || playerInCombatWithCount: " + playerInCombatWith.Count.ToString() + " || " + Time.time.ToString());

        // fire count
        GetFireCount();

        // UI
        UpdateUI();

        // interaction check
        if (!SetupManager.instance.paused)
        {
            // can check interaction?
            if (canCheckInteractionCounter < canCheckInteractionDur)
            {
                canCheckInteractionCounter++;
            }

            // actual interaction check
            if (interactionCheckCounter < interactionCheckDur)
            {
                interactionCheckCounter++;
            }
            inInteraction = (interactionCheckCounter < interactionCheckDur);
        }

        // shop inspect
        if (!SetupManager.instance.paused)
        {
            if (shopInspectCounter < shopInspectDur)
            {
                shopInspectCounter++;
            }
            inShopInspect = (shopInspectCounter < shopInspectDur);
        }

        // game wait & countdown
        if (gameWaitCounter < gameWaitDur)
        {
            if (!SetupManager.instance.paused && ((SetupManager.instance.curGameState == SetupManager.GameState.Level && LevelGeneratorManager.instance.activeLevelGenerator.generatedLevel) || SetupManager.instance.curGameState != SetupManager.GameState.Level) )
            {
                gameWaitCounter++;
            }
        }
        else
        {
            countdownScaleTarget = countdownScaleOriginal;
            countdownScaleAdd = BasicFunctions.SpringVector(countdownScaleAdd, countdownScaleCur, countdownScaleTarget, .01f, .25f, .5f, .95f);
            countdownScaleCur += countdownScaleAdd;
            countdownRectTransform.localScale = countdownScaleCur;

            string levelString = "";
            switch ( SetupManager.instance.runDataRead.curFloorIndex)
            {
                case 1: levelString += "SEWER "; break;
                case 2: levelString += "DUNGEON "; break;
                case 3: levelString += "HELL "; break;
            }
            levelString += " " + /*SetupManager.instance.runDataRead.curFloorIndex.ToString() + "-" +*/ (SetupManager.instance.runDataRead.curLevelIndex + 1).ToString();
            if ( SetupManager.instance.curRunType == SetupManager.RunType.Endless )
            {
                levelString += "\n" + "loop " + SetupManager.instance.runDataRead.curLoopIndex.ToString();
            }

            if ( SetupManager.instance.curGameState == SetupManager.GameState.Shop )
            {
                levelString = "SHOP";
            }
            if (SetupManager.instance.curGameState == SetupManager.GameState.Rest)
            {
                levelString = "REST";
            }

            string countdownString = "";
            countdownIndexMax = 4;
            switch (SetupManager.instance.curGameState)
            {
                default:
                    countdownIndexMax = 4;
                    switch (countdownIndex)
                    {
                        case 0: countdownString = levelString; break;
                        case 1: countdownString = "READY"; break;
                        case 2: countdownString = "SET"; break;
                        case 3: countdownString = "GO!"; break;
                        case 4: countdownString = ""; break;
                    }
                    break;

                case SetupManager.GameState.Fountain:
                    countdownIndexMax = 1;
                    switch (countdownIndex)
                    {
                        case 0: countdownString = levelString; break;
                        case 1: countdownString = ""; break;
                    }
                    break;

                case SetupManager.GameState.Shop:
                    countdownIndexMax = 1;
                    switch (countdownIndex)
                    {
                        case 0: countdownString = levelString; break;
                        case 1: countdownString = ""; break;
                    }
                    break;
            }

            countdownText.text = countdownString;

            bool showCountdownText = (inCountdown && !SetupManager.instance.paused);
            countdownText.enabled = showCountdownText;

            if (countdownIndex < countdownIndexMax)
            {
                int countdownRateAdd = (countdownIndex == 0) ? 20 : 0;
                if (countdownCounter < (countdownRate + countdownRateAdd))
                {
                    if (!SetupManager.instance.paused && ((SetupManager.instance.curGameState == SetupManager.GameState.Level && LevelGeneratorManager.instance.activeLevelGenerator.generatedLevel) || SetupManager.instance.curGameState != SetupManager.GameState.Level))
                    {
                        countdownCounter++;
                    }
                }
                else
                {
                    countdownIndex++;
                    countdownCounter = 0;
                    countdownRate = 35;
                    if (countdownIndex >= (countdownIndexMax - 1))
                    {
                        inGameWait = false;

                        // reset all player input
                        if (InputManager.instance != null)
                        {
                            InputManager.instance.DisableAllInput();
                        }

                        // player force on start?
                        //if ( playerFirstPersonDrifter != null )
                        //{
                        //    playerFirstPersonDrifter.myImpactReceiver.AddImpact(playerFirstPersonDrifter.myTransform.forward,10f);
                        //}

                        // gore popup invoke
                        Invoke("CheckForShopAndRestPopups",1f);
                    }

                    // audio
                    if (SetupManager.instance != null && SetupManager.instance.curGameState == SetupManager.GameState.Level)
                    {
                        if (countdownIndex < countdownIndexMax)
                        {
                            float pitchBase = .6f + (.2f * countdownIndex);
                            AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.countdownProceedClips), pitchBase, pitchBase + .025f, .05f, .075f);
                            //if (countdownIndex >= (countdownIndexMax - 1))
                            //{
                            //    AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.countdownEndClips), .9f, 1f, .075f, .1f);
                            //}
                            //else
                            //{
                            //    float pitchBase = .6f + (.2f * countdownIndex);
                            //    AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.countdownProceedClips), pitchBase, pitchBase + .025f, .075f, .1f);
                            //}
                        }
                    }

                    // countdown scale
                    countdownScaleCur = countdownScaleOriginal;
                    countdownScaleCur.x *= 1.1f;
                    countdownScaleCur.y *= .75f;
                }
            }
            else
            {
                inCountdown = false;
            }
        }

        // player reached end?
        if ( SetupManager.instance.runDataRead.playerReachedEnd )
        {
            SetupManager.instance.SetFreeze(1);
        }
    }

    void CheckForShopAndRestPopups ()
    {
        // shop popup?
        if (SetupManager.instance.curGameState == SetupManager.GameState.Shop)
        {
            if (!SetupManager.instance.curProgressData.persistentData.sawShopPopup)
            {
                SetupManager.instance.SetTutorialPopup(SetupManager.instance.UIPopupBaseCol + "you can spend coins in the shop" + "\n" + "to buy equipment!", 60);
                SetupManager.instance.curProgressData.persistentData.sawShopPopup = true;
            }
        }

        // rest popup?
        if (SetupManager.instance.curGameState == SetupManager.GameState.Rest)
        {
            if (!SetupManager.instance.curProgressData.persistentData.sawRestPopup)
            {
                SetupManager.instance.SetTutorialPopup(SetupManager.instance.UIPopupBaseCol + "you can rest near the fire" + "\n" + "to regain health!", 60);
                SetupManager.instance.curProgressData.persistentData.sawRestPopup = true;
            }
        }
    }

    void UpdateUI ()
    {
        // popup
        if ( popupBackImage != null )
        {
            bool showPopup = (SetupManager.instance.popupCounter < SetupManager.instance.popupDur && !SetupManager.instance.paused && !playerHurt && !SetupManager.instance.runDataRead.playerDead);
            popupBackImage.enabled = showPopup;
            popupText.enabled = showPopup;
        }

        // player health
        if ( playerHealthText != null )
        {
            playerHealthText.text = SetupManager.instance.runDataRead.playerHealthCur.ToString();
        }

        // player fire count
        if ( playerFireCountText != null )
        {
            playerFireCountText.text = SetupManager.instance.runDataRead.playerFireCur.ToString();
        }

        // fire left count
        if (fireLeftText != null)
        {
            fireLeftText.text = fireLeftCount.ToString();
            fireLeftText.enabled = (fireLeftCount > 0 && !playerHurt && !inGameWait);
        }

        // fire left check image
        if ( fireLeftCheckImage != null )
        {
            fireLeftCheckImage.enabled = (fireLeftCount <= 0 && !playerHurt && !inGameWait);
        }

        // game over text
        if ( gameOverText != null )
        {
            bool showGameOverText = (SetupManager.instance.curGameState == SetupManager.GameState.Level && SetupManager.instance.runDataRead.playerDead);
            gameOverText.enabled = showGameOverText;
        }

        // fire info text
        if ( playerFireInfoText != null )
        {
            switch (HandManager.instance.handScripts[0].curMagicMode)
            {
                case HandScript.MagicMode.In: playerFireInfoText.text = "IN"; break;
                case HandScript.MagicMode.Out: playerFireInfoText.text = "OUT"; break;
            }
        }

        // blessings
        bool showBlessingInfo = (FountainManager.instance != null && !SetupManager.instance.hidePlayerUI && FountainManager.instance.inBlessingPick && !SetupManager.instance.hideUI);
        if (blessingInfoBack != null)
        {
            blessingInfoBack.enabled = showBlessingInfo;
            blessingInfoText.enabled = showBlessingInfo;
            blessingInfoText.text = SetupManager.instance.UIInteractionButtonCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - choose a " + SetupManager.instance.UIBlessingCol + "blessing";
        }
        
        bool showBlessingDescription = (FountainManager.instance != null && !SetupManager.instance.hidePlayerUI && FountainManager.instance.inBlessingPick && !SetupManager.instance.hideUI);
        if (blessingDescriptionBack != null)
        {
            blessingDescriptionBack.enabled = false;//showBlessingDescription;
            blessingDescriptionText.enabled = showBlessingDescription;
        }

        bool showBlessingArrow = (FountainManager.instance != null && !SetupManager.instance.hidePlayerUI && FountainManager.instance.inBlessingPick && !SetupManager.instance.hideUI);
        if (blessingArrowFrontImage != null)
        {
            blessingArrowFrontImage.enabled = showBlessingArrow;
            blessingArrowBackImage.enabled = false;
            //if (showBlessingArrow)
            //{
            //    blessingArrowBackImage.enabled = showBlessingArrow;
            //}
        }

        // shop browse
        bool showShopBrowseArrow = (ShopManager.instance != null && !SetupManager.instance.hidePlayerUI && ShopManager.instance.inItemBrowse && !SetupManager.instance.hideUI);
        if (shopBrowseArrowFrontImage != null)
        {
            shopBrowseArrowFrontImage.enabled = showShopBrowseArrow;
            if (showShopBrowseArrow)
            {
                shopBrowseArrowFrontImage.sprite = (ShopManager.instance.itemBrowseIndex == 0) ? shopBrowseArrowRightSprite : shopBrowseArrowLeftSprite;
                //shopBrowseArrowBackImage.enabled = showShopBrowseArrow;
            }
        }

        bool showShopBrowseInfo = (ShopManager.instance != null && !SetupManager.instance.hidePlayerUI && ShopManager.instance.inItemBrowse && !SetupManager.instance.hideUI && ShopManager.instance.itemBrowseIndex == 0);
        if (shopBrowseInfoBack != null)
        {
            shopBrowseInfoBack.enabled = showShopBrowseInfo;
            shopBrowseInfoText.enabled = showShopBrowseInfo;
            string shopBrowseInfoString;
            if (showShopBrowseInfo)
            {
                int shopCoinCostUse = curInteractionScript.myEquipmentData.shopCoinCost;
                if (SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Thrifty))
                {
                    shopCoinCostUse = Mathf.RoundToInt(shopCoinCostUse * .66f);
                }

                bool playerHasEnoughCoins = (SetupManager.instance.runDataRead.curRunCoinsCollected >= shopCoinCostUse);
                if (playerHasEnoughCoins)
                {
                    shopBrowseInfoString = SetupManager.instance.UIInteractionButtonCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - buy for " + SetupManager.instance.UICoinCol + curInteractionScript.myEquipmentData.shopCoinCost.ToString() + SetupManager.instance.UIInteractionBaseCol + " coins";
                }
                else
                {
                    shopBrowseInfoString = SetupManager.instance.UICoinCol + shopCoinCostUse.ToString() + SetupManager.instance.UIInteractionSecondCol + " coins " + SetupManager.instance.UIInteractionSecondCol + "needed";
                }
                shopBrowseInfoText.text = shopBrowseInfoString;
            }
        }

        bool showShopBrowseDescription = (ShopManager.instance != null && !SetupManager.instance.hidePlayerUI && ShopManager.instance.inItemBrowse && !SetupManager.instance.hideUI);
        if (shopBrowseDescriptionBack != null)
        {
            shopBrowseDescriptionBack.enabled = false;//showShopBrowseDescription;
            shopBrowseDescriptionText.enabled = showShopBrowseDescription;
        }

        bool showShopQuit = (ShopManager.instance != null && !SetupManager.instance.hidePlayerUI && ShopManager.instance.inItemBrowse && !SetupManager.instance.hideUI );
        if (shopQuitBack != null)
        {
            shopQuitBack.enabled = false;
            shopQuitText.enabled = showShopQuit;
            if (showShopQuit)
            {
                shopQuitText.text = InputManager.instance.cancelInputStringUse;
            }
        }

        // paused text?
        if ( pausedText != null )
        {
            bool showPausedText = false;//(SetupManager.instance.paused && !SetupManager.instance.hideUI);
            pausedText.enabled = showPausedText;
        }

        // key?
        bool showKey = (!inCountdown && SetupManager.instance.curEncounterType != SetupManager.EncounterType.Small && !SetupManager.instance.hidePlayerUI && !SetupManager.instance.runDataRead.playerDead && !playerHurt && !inGameWait && !SetupManager.instance.runDataRead.playerReachedEnd && !SetupManager.instance.paused && SetupManager.instance.curGameState != SetupManager.GameState.Fountain && SetupManager.instance.curGameState != SetupManager.GameState.Shop && SetupManager.instance.curGameState != SetupManager.GameState.BlessingPick && SetupManager.instance.curGameState != SetupManager.GameState.ShopBrowse && SetupManager.instance.curGameState != SetupManager.GameState.Rest && !SetupManager.instance.inTransition && !SetupManager.instance.hideUI);
        if ( keyActivePart0 != null && keyActivePart1 != null )
        {
            keyActivePart0.enabled = (showKey && playerHasKey);
            keyActivePart1.enabled = (showKey && playerHasKey);
        }
        if (keyInactivePart0 != null && keyInactivePart1 != null)
        {
            keyInactivePart0.enabled = (showKey && !playerHasKey);
            keyInactivePart1.enabled = (showKey && !playerHasKey);
        }

        // coins
        bool showCoinsCollected = (!inCountdown && !SetupManager.instance.hidePlayerUI && !SetupManager.instance.runDataRead.playerDead && !playerHurt && !inGameWait && (!inInteraction || curInteractionType == InteractionType.ShopItem || SetupManager.instance.curGameState == SetupManager.GameState.ShopBrowse) && !SetupManager.instance.runDataRead.playerReachedEnd && !SetupManager.instance.paused && SetupManager.instance.curGameState != SetupManager.GameState.BlessingPick && SetupManager.instance.curGameState != SetupManager.GameState.Rest && !SetupManager.instance.inTransition && !SetupManager.instance.hideUI);
        if ( coinsCollectedImage != null )
        {
            coinsCollectedImage.enabled = showCoinsCollected;
        }
        if (coinsCollectedText != null)
        {
            coinsCollectedText.enabled = showCoinsCollected;
            coinsCollectedText.text = SetupManager.instance.runDataRead.curRunCoinsCollected.ToString();
        }
        if (coinsCollectedRectTransform != null)
        {
            float coinsCollectedScl = .75f;
            Vector3 coinsCollectedPosSet = coinsCollectedPosOriginal;
            if ((inInteraction && curInteractionType == InteractionType.ShopItem) || SetupManager.instance.curGameState == SetupManager.GameState.ShopBrowse)
            {
                coinsCollectedPosSet = coinsCollectedPosHighlight;
                //coinsCollectedScl = 1f;
            }
            coinsCollectedRectTransform.anchoredPosition = coinsCollectedPosSet;
            coinsCollectedRectTransform.localScale = Vector3.one * coinsCollectedScl;
        }

        // tears
        bool showTearsCollected = (!inCountdown && !SetupManager.instance.hidePlayerUI && !SetupManager.instance.runDataRead.playerDead && !playerHurt && !inGameWait && (!inInteraction || curInteractionType == InteractionType.Fountain) && !SetupManager.instance.runDataRead.playerReachedEnd && !SetupManager.instance.paused && SetupManager.instance.curGameState != SetupManager.GameState.BlessingPick && SetupManager.instance.curGameState != SetupManager.GameState.ShopBrowse && SetupManager.instance.curGameState != SetupManager.GameState.Rest && !SetupManager.instance.inTransition && !SetupManager.instance.hideUI);
        if (tearsCollectedImage != null)
        {
            tearsCollectedImage.enabled = showTearsCollected;
        }
        if (tearsCollectedText != null)
        {
            tearsCollectedText.enabled = showTearsCollected;
            tearsCollectedText.text = SetupManager.instance.runDataRead.curRunTearsCollected.ToString();
        }
        if ( tearsCollectedRectTransform != null )
        {
            float tearsCollectedScl = .75f;
            Vector3 tearsCollectedPosSet = tearsCollectedPosOriginal;
            if ( inInteraction && curInteractionType == InteractionType.Fountain )
            {
                tearsCollectedPosSet = tearsCollectedPosHighlight;
                //tearsCollectedScl = 1f;
            }
            tearsCollectedRectTransform.anchoredPosition = tearsCollectedPosSet;
            tearsCollectedRectTransform.localScale = Vector3.one * tearsCollectedScl;
        }

        // door back icon
        if ( doorIconBackImage != null )
        {
            doorIconBackImage.color = (fireLeftCount > 0) ? doorIconBackColOn : doorIconBackColOff;
            doorIconBackImage.enabled = (!playerHurt && !inGameWait && !SetupManager.instance.runDataRead.playerReachedEnd);
        }
        if ( doorIconFrontImage != null )
        {
            doorIconFrontImage.enabled = (!playerHurt && !inGameWait && !SetupManager.instance.runDataRead.playerReachedEnd);
        }

        // compass
        UpdateCompass();

        // player fire icon & text
        if (playerFireImage != null)
        {
            if (playerFireFlickerCount < playerFireFlickerCountMax)
            {
                if (playerFireFlickerCounter < playerFireFlickerRate)
                {
                    playerFireFlickerCounter++;
                }
                else
                {
                    playerFireFlickerCount++;
                    playerFireFlickerCounter = 0;
                    playerFireFlickerIndex = (playerFireFlickerIndex == 0) ? 1 : 0;
                }
                playerFireImage.color = (playerFireFlickerIndex == 1) ? whiteCol : blackCol;
                playerFireCountText.color = (playerFireFlickerIndex == 0) ? whiteCol : blackCol;
            }
            else
            {
                playerFireImage.color = playerFireBackColNormal;
                playerFireCountText.color = playerFireTextColNormal;
            }
        }

        if (HandManager.instance != null && playerFireImage != null)
        {
            //if (HandManager.instance.handScripts[0].curMagicMode == HandScript.MagicMode.Out)
            if ( HandManager.instance.handScripts[0].curHandState != HandScript.HandState.MagicCollect || HandManager.instance.handScripts[0].magicChargeIndex < 2 )
            {
                playerFireImage.sprite = fireOutSprite;

                playerFireRectTransform.anchoredPosition = playerFireIconPosOriginal;
                playerFireTextRectTransform.anchoredPosition = playerFireTextPosOriginal;
                playerFireRectTransform.localRotation = Quaternion.identity;
            }
            else
            {
                playerFireImage.sprite = fireInSprite;

                Vector3 iconP = playerFireIconPosOriginal;
                iconP.y -= 10f;
                playerFireRectTransform.anchoredPosition = iconP;

                Vector3 textP = playerFireTextPosOriginal;
                //textP.y += 10f;
                playerFireTextRectTransform.anchoredPosition = textP;

                // rotation
                playerFireIconRot -= 4f;
                playerFireRectTransform.localRotation = Quaternion.Euler(0f, 0f, playerFireIconRot);
            }
            bool showPlayerFire = (!playerHurt && !SetupManager.instance.hidePlayerUI && !inGameWait && !inInteraction && !SetupManager.instance.runDataRead.playerReachedEnd && !SetupManager.instance.paused && SetupManager.instance.curGameState != SetupManager.GameState.Fountain && SetupManager.instance.curGameState != SetupManager.GameState.BlessingPick && SetupManager.instance.curGameState != SetupManager.GameState.ShopBrowse && !SetupManager.instance.inTransition && !SetupManager.instance.hideUI);
            playerFireImage.enabled = showPlayerFire;
            playerFireCountText.enabled = showPlayerFire;//(!playerHurt && !SetupManager.instance.hidePlayerUI && !inGameWait && !inInteraction && !playerReachedEnd && !SetupManager.instance.paused && SetupManager.instance.curGameState != SetupManager.GameState.Fountain && SetupManager.instance.curGameState != SetupManager.GameState.Shop && SetupManager.instance.curGameState != SetupManager.GameState.BlessingPick && SetupManager.instance.curGameState != SetupManager.GameState.ShopBrowse && SetupManager.instance.curGameState != SetupManager.GameState.Rest && !SetupManager.instance.inTransition && !SetupManager.instance.hideUI);
        }

        // player fire button text
        if (playerFireButtonText != null)
        {
            if (playerFireButtonFlickerCount < playerFireButtonFlickerCountMax)
            {
                if (playerFireButtonFlickerCounter < playerFireButtonFlickerRate)
                {
                    playerFireButtonFlickerCounter++;
                }
                else
                {
                    playerFireButtonFlickerCount++;
                    playerFireButtonFlickerCounter = 0;
                    playerFireButtonFlickerIndex = (playerFireButtonFlickerIndex == 0) ? 1 : 0;
                }
                playerFireButtonText.color = (playerFireButtonFlickerIndex == 1) ? whiteCol : blackCol;
            }
            else
            {
                playerFireButtonText.color = playerFireButtonColNormal;
            }
            playerFireButtonText.enabled = (!playerHurt && !SetupManager.instance.hidePlayerUI && !inGameWait && !inInteraction && !SetupManager.instance.runDataRead.playerReachedEnd && !SetupManager.instance.paused && SetupManager.instance.curGameState != SetupManager.GameState.Fountain && SetupManager.instance.curGameState != SetupManager.GameState.Shop && SetupManager.instance.curGameState != SetupManager.GameState.BlessingPick && SetupManager.instance.curGameState != SetupManager.GameState.ShopBrowse && !SetupManager.instance.inTransition && !SetupManager.instance.hideUI);
        }

        // player health icon & text
        if (playerHealthImage != null)
        {
            if (playerHealthFlickerCount < playerHealthFlickerCountMax)
            {
                if (playerHealthFlickerCounter < playerHealthFlickerRate)
                {
                    playerHealthFlickerCounter++;
                }
                else
                {
                    playerHealthFlickerCount++;
                    playerHealthFlickerCounter = 0;
                    playerHealthFlickerIndex = (playerHealthFlickerIndex == 0) ? 1 : 0;
                }
                playerHealthImage.color = (playerHealthFlickerIndex == 1) ? whiteCol : blackCol;
                playerHealthText.color = (playerHealthFlickerIndex == 0) ? whiteCol : blackCol;
            }
            else
            {
                playerHealthImage.color = playerHealthBackColNormal;
                playerHealthText.color = playerHealthTextColNormal;
            }
            bool showPlayerHealth = (!inGameWait && !SetupManager.instance.hidePlayerUI && (!inInteraction || SetupManager.instance.curGameState == SetupManager.GameState.Rest) && !SetupManager.instance.runDataRead.playerReachedEnd && !SetupManager.instance.paused && SetupManager.instance.curGameState != SetupManager.GameState.BlessingPick && SetupManager.instance.curGameState != SetupManager.GameState.ShopBrowse && !SetupManager.instance.inTransition && !SetupManager.instance.hideUI);
            bool preventShowHealth = false;
            if (showPlayerHealth)
            {
                if ( SetupManager.instance.runDataRead.playerHealthCur <= 1 )
                {
                    preventShowHealth = true;
                }
                else
                {
                    preventShowHealth = false;
                }
            }
            if ( preventShowHealth )
            {
                if ( playerCriticalHealthFlickerCounter < playerCriticalHealthFlickerRate )
                {
                    playerCriticalHealthFlickerCounter++;
                }
                else
                {
                    playerCriticalHealthFlickerIndex = (playerCriticalHealthFlickerIndex == 0) ? 1 : 0;
                    playerCriticalHealthFlickerCounter = 0;
                }
                showPlayerHealth = (playerCriticalHealthFlickerIndex == 1);
            }
            playerHealthImage.enabled = showPlayerHealth;
            playerHealthText.enabled = showPlayerHealth;
        }

        // crosshair
        if ( crosshairImage != null )
        {
            crosshairImage.enabled = (!playerHurt && !inGameWait && !inInteraction && !SetupManager.instance.paused && SetupManager.instance.curGameState != SetupManager.GameState.BlessingPick && SetupManager.instance.curGameState != SetupManager.GameState.ShopBrowse && !SetupManager.instance.inTransition && !SetupManager.instance.hideUI);
            if ( !inGameWait && countdownIndex < countdownIndexMax )
            {
                crosshairImage.enabled = false;
            }
        }

        // interaction check
        if ( interactionCheckBackImage != null )
        {
            bool showInteraction = (inInteraction && !SetupManager.instance.hidePlayerUI && !SetupManager.instance.runDataRead.playerReachedEnd && !SetupManager.instance.paused && !inCountdown && !inGameWait && !playerHurt && !SetupManager.instance.runDataRead.playerDead && SetupManager.instance.curGameState != SetupManager.GameState.BlessingPick && SetupManager.instance.curGameState != SetupManager.GameState.ShopBrowse && !SetupManager.instance.inTransition && !SetupManager.instance.hideUI);
            interactionCheckBackImage.enabled = showInteraction;
            interactionCheckText.enabled = showInteraction;
        }

        HandleInteractions();

        // shop inspect
        if (shopInspectBackImage != null)
        {
            bool showShopInspect = (inShopInspect && !SetupManager.instance.hidePlayerUI && !SetupManager.instance.runDataRead.playerReachedEnd && !SetupManager.instance.paused && !inCountdown && !inGameWait && !playerHurt && !SetupManager.instance.runDataRead.playerDead && SetupManager.instance.curGameState != SetupManager.GameState.BlessingPick && SetupManager.instance.curGameState != SetupManager.GameState.ShopBrowse && !SetupManager.instance.inTransition && !SetupManager.instance.hideUI);
            shopInspectBackImage.enabled = showShopInspect;
            shopInspectText.enabled = showShopInspect;
        }
    }

    void UpdateCompass ()
    {
        if (compassBackImage != null)
        {
            float numberOfPixelsNorthToNorth = 400f;
            float rationAngleToPixel = numberOfPixelsNorthToNorth / 360f;

            float centerOffY = 42.5f;
            Vector3 centerP = new Vector3(0f, -centerOffY, 0f);
            Vector3 camFwd = mainCameraTransform.forward;
            Vector3 camRight = mainCameraTransform.right;
            camFwd.y = 0f;
            camRight.y = 0f;
            camFwd.Normalize();
            camRight.Normalize();
            Vector3 up = Vector3.up;

            // color
            float facingThreshold = .9f;
            float farThreshold = -.375f;

            // north
            Vector3 n = Vector3.forward;
            float yOff = -centerOffY + 5f;

            Debug.DrawLine(mainCameraTransform.position, mainCameraTransform.position + n, Color.yellow);
            Debug.DrawLine(mainCameraTransform.position, mainCameraTransform.position + camFwd, Color.red);

            Vector3 northPerp = Vector3.Cross(n, camFwd);
            float northDot = Vector3.Dot(n, camFwd);
            float northDir = Vector3.Dot(northPerp, up);
            Vector3 northP = centerP - (new Vector3(Vector3.Angle(camFwd, n) * Mathf.Sign(northDir) * rationAngleToPixel, yOff, 0f));

            if (northDot > facingThreshold)
            {
                compassNorthText.color = compassFacingNorthCol;
            }
            else
            {
                compassNorthText.color = (northDot > farThreshold) ? compassNearCol : compassFarCol;
            }

            // east
            Vector3 e = Vector3.right;
            Vector3 eastPerp = Vector3.Cross(e, camFwd);
            float eastDot = Vector3.Dot(e, camFwd);
            float eastDir = Vector3.Dot(eastPerp, up);
            Vector3 eastP = centerP - (new Vector3(Vector3.Angle(camFwd, e) * Mathf.Sign(eastDir) * rationAngleToPixel, yOff, 0f));

            if (eastDot > facingThreshold)
            {
                compassEastText.color = compassFacingEastCol;
            }
            else
            {
                compassEastText.color = (eastDot > farThreshold) ? compassNearCol : compassFarCol;
            }

            // south
            Vector3 s = -Vector3.forward;
            Vector3 southPerp = Vector3.Cross(s, camFwd);
            float southDot = Vector3.Dot(s, camFwd);
            float southDir = Vector3.Dot(southPerp, up);
            Vector3 southP = centerP - (new Vector3(Vector3.Angle(camFwd, s) * Mathf.Sign(southDir) * rationAngleToPixel, yOff, 0f));

            if (southDot > facingThreshold)
            {
                compassSouthText.color = compassFacingSouthCol;
            }
            else
            {
                compassSouthText.color = (southDot > farThreshold) ? compassNearCol : compassFarCol;
            }

            // west
            Vector3 w = -Vector3.right;
            Vector3 westPerp = Vector3.Cross(w, camFwd);
            float westDot = Vector3.Dot(w, camFwd);
            float westDir = Vector3.Dot(westPerp, up);
            Vector3 westP = centerP - (new Vector3(Vector3.Angle(camFwd, w) * Mathf.Sign(westDir) * rationAngleToPixel, yOff, 0f));

            if (westDot > facingThreshold)
            {
                compassWestText.color = compassFacingWestCol;
            }
            else
            {
                compassWestText.color = (westDot > farThreshold) ? compassNearCol : compassFarCol;
            }

            compassNorthRectTransform.anchoredPosition = northP;
            compassEastRectTransform.anchoredPosition = eastP;
            compassSouthRectTransform.anchoredPosition = southP;
            compassWestRectTransform.anchoredPosition = westP;

            bool showCompass = (!playerHurt && !SetupManager.instance.hidePlayerUI && !inGameWait && !inInteraction && !SetupManager.instance.runDataRead.playerReachedEnd && !SetupManager.instance.paused && SetupManager.instance.curGameState != SetupManager.GameState.BlessingPick && SetupManager.instance.curGameState != SetupManager.GameState.ShopBrowse && SetupManager.instance.curGameState != SetupManager.GameState.Rest && !SetupManager.instance.inTransition && !SetupManager.instance.hideUI);
            compassNorthText.enabled = showCompass;
            compassEastText.enabled = showCompass;
            compassSouthText.enabled = showCompass;
            compassWestText.enabled = showCompass;
            compassBackImage.enabled = showCompass;

            // log
            //Debug.Log("northDot: " + northDot.ToString() + " || n: " + n.ToString() + " || camFwd: " + camFwd.ToString() + " || " + Time.time.ToString());
        }
    }

    public void SetPlayerHurt ( float _target, int _dur )
    {
        playerHurtDur = _dur;
        playerHurtCounter = 0;
        playerHurtFactorTarget = _target;
        playerHurtFactorCur = 1f;
        playerHurt = true;
        if (HandManager.instance != null && HandManager.instance.handScripts != null && HandManager.instance.handScripts.Length > 0)
        {
            for (int i = 0; i < HandManager.instance.handScripts.Length; i++)
            {
                HandScript curHandScript = HandManager.instance.handScripts[i];
                if ( curHandScript != null )
                {
                    curHandScript.SetHandState(HandScript.HandState.Hurt);
                }
            }
        }
    }

    public void GetFireCount ()
    {
        if (fireScripts != null && fireScripts.Count > 0 )
        {
            fireLeftCount = 0;
            for ( int i = 0; i < fireScripts.Count; i ++ )
            {
                if (!fireScripts[i].autoClear)
                {
                    fireLeftCount++;
                }
            }
        }
        else
        {
            fireLeftCount = 0;
        }
    }

    public void SetPlayerStunned ( int _dur )
    {
        playerFirstPersonDrifter.stunnedDur = _dur;
        playerFirstPersonDrifter.stunnedCounter = 0;
    }
       
    public void DealDamageToPlayer ( int _amount, Npc.AttackData.DamageType _damageType )
    {
        ClearAllDamageDeals();

        switch (SetupManager.instance.curRunType)
        {
            case SetupManager.RunType.Normal: SetupManager.instance.curProgressData.normalRunData.playerHealthCur -= _amount; break;
            case SetupManager.RunType.Endless: SetupManager.instance.curProgressData.endlessRunData.playerHealthCur -= _amount; break;
        }

        SetupManager.instance.UpdateRunDataRead();

        int hurtDur = 30;
        SetupManager.instance.SetFreeze(hurtDur);
        SetPlayerHurt(.125f,hurtDur / 2);

        HandManager.instance.handScripts[1].StopMeleeAttack();

        playerFirstPersonDrifter.StopKick();

        if (SetupManager.instance.runDataRead.playerHealthCur <= 0 )
        {
            // player defeated? or get a second chance?
            bool useSecondChance = (!SetupManager.instance.runDataRead.usedSecondChance && SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.SecondChance) );
            if (!useSecondChance)
            {
                SetPlayerDead();
            }
            else
            {
                switch (SetupManager.instance.curRunType)
                {
                    case SetupManager.RunType.Normal:
                        SetupManager.instance.curProgressData.normalRunData.playerHealthCur += BlessingDatabase.instance.secondChanceHealthGain;
                        SetupManager.instance.curProgressData.normalRunData.usedSecondChance = true;

                        SetupManager.instance.SetTutorialPopup(SetupManager.instance.UIPopupBaseCol + "you get a second chance!",60);
                        break;
                    case SetupManager.RunType.Endless:
                        SetupManager.instance.curProgressData.endlessRunData.playerHealthCur += BlessingDatabase.instance.secondChanceHealthGain;
                        SetupManager.instance.curProgressData.endlessRunData.usedSecondChance = true;

                        SetupManager.instance.SetTutorialPopup(SetupManager.instance.UIPopupBaseCol + "you get a second chance!", 60);
                        break;
                }
            }

            // player dead audio
            AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.playerDeadClips), .2f, .4f, .2f, .225f);
        }
        else
        {
            // player hurt audio
            AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.playerHurtClips), .6f, .8f, .2f, .225f);
        }

        SetPlayerHealthFlicker(6);

        // shed tear? cloak of sadness)
        if ( SetupManager.instance.CheckIfItemSpecialActive(EquipmentDatabase.Specials.PlayerShedTear) )
        {
            GameObject tearO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.tearPrefab[0], GameManager.instance.playerFirstPersonDrifter.myTransform.position, Quaternion.identity, 1f);
            Stuiterbaar stuiterbaarScript = tearO.GetComponent<Stuiterbaar>();
            if (stuiterbaarScript != null)
            {
                float spawnSideForceOffMax = .05f;
                float spawnUpForceOffMax = .075f;
                float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                float xAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                float zAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                stuiterbaarScript.forceCur.x += (xAdd * xDir);
                stuiterbaarScript.forceCur.y += TommieRandom.instance.RandomRange(spawnUpForceOffMax * .5f, spawnUpForceOffMax);
                stuiterbaarScript.forceCur.z += (zAdd * zDir);
            }
        }

        // gain fire? (revenge)
        if ( SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Revenge) && SetupManager.instance.runDataRead.playerFireCur < SetupManager.instance.runDataRead.playerFireMax )
        {
            AddPlayerFire(1);
        }

        // short player invulnerability
        SetupManager.instance.SetPlayerInvulnerable(24);
    }

    public void AddPlayerHealth ( int _amount )
    {
        switch ( SetupManager.instance.curRunType )
        {
            case SetupManager.RunType.Normal:
                SetupManager.instance.curProgressData.normalRunData.playerHealthCur += _amount;

                if (!SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Resilient))
                {
                    if (SetupManager.instance.curProgressData.normalRunData.playerHealthCur > SetupManager.instance.curProgressData.normalRunData.playerHealthMax)
                    {
                        SetupManager.instance.curProgressData.normalRunData.playerHealthCur = SetupManager.instance.curProgressData.normalRunData.playerHealthMax;
                    }
                }
                break;

            case SetupManager.RunType.Endless:
                SetupManager.instance.curProgressData.endlessRunData.playerHealthCur += _amount;

                if (!SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Resilient))
                {
                    if (SetupManager.instance.curProgressData.endlessRunData.playerHealthCur > SetupManager.instance.curProgressData.endlessRunData.playerHealthMax)
                    {
                        SetupManager.instance.curProgressData.endlessRunData.playerHealthCur = SetupManager.instance.curProgressData.endlessRunData.playerHealthMax;
                    }
                }
                break;
        }

        SetPlayerHealthFlicker(6);

        // audio
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.playerHealClips),.7f,.9f,.2f,.225f);
    }

    public void SetPlayerDead ()
    {
        switch ( SetupManager.instance.curRunType )
        {
            case SetupManager.RunType.Normal: SetupManager.instance.curProgressData.normalRunData.playerDead = true; break;
            case SetupManager.RunType.Endless: SetupManager.instance.curProgressData.endlessRunData.playerDead = true; break;
        }

        Invoke("GameOverTransition",3f);
        Invoke("ProceedToGameOver",5f);

        // audio
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.playerSetDeadClips),.7f,.9f,.3f,.325f);

        // save
        SaveManager.instance.WriteToFile(SetupManager.instance.curProgressData);

        // log
        Debug.Log("set player dead! || " + Time.time.ToString());
    }

    void GameOverTransition ()
    {
        SetupManager.instance.SetTransition(SetupManager.TransitionMode.In);
    }

    void ProceedToGameOver ()
    {
        // reset all input
        if (InputManager.instance != null)
        {
            InputManager.instance.DisableAllInput();
        }

        // clear all particles
        if (SetupManager.instance != null)
        {
            SetupManager.instance.ClearAllParticles();
        }

        // decals
        if ( DecalManager.instance != null )
        {
            DecalManager.instance.ClearAllDecals();
        }

        // game state
        SetupManager.instance.SetGameState(SetupManager.GameState.GameOver);

        // go
        SceneManager.LoadScene(6 + 1,LoadSceneMode.Single);
    }

    public void CreateDamageIndicator ( int _amount, Vector3 _pos )
    {
        GameObject newDamageIndicatorO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.damageIndicatorPrefab[0], _pos, Quaternion.identity, 1f);
        Transform newDamageIndicatorTr = newDamageIndicatorO.transform;
        newDamageIndicatorTr.parent = mainCanvasRectTransform;

        BasicFunctions.ResetTransform(newDamageIndicatorTr);

        RectTransform rTr = newDamageIndicatorO.GetComponent<RectTransform>();
        rTr.localScale = Vector3.one;

        DamageIndicator newDamageIndicatorScript = newDamageIndicatorO.GetComponent<DamageIndicator>();
        newDamageIndicatorScript.damageAmount = _amount;
        newDamageIndicatorScript.spawnPos = _pos;
        float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f,1f));
        float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
        float maxOff = .25f;
        float xAdd = TommieRandom.instance.RandomRange(maxOff * .25f,maxOff) * xDir;
        float zAdd = TommieRandom.instance.RandomRange(maxOff * .25f,maxOff) * zDir;
        float yAdd = TommieRandom.instance.RandomRange(maxOff * .675f,maxOff);
        newDamageIndicatorScript.forceAdd = new Vector3(xAdd,yAdd,zAdd);
    }

    public void CreateDamageIndicatorString(string _string, Vector3 _pos)
    {
        GameObject newDamageIndicatorO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.damageIndicatorPrefab[0], _pos, Quaternion.identity, 1f);
        Transform newDamageIndicatorTr = newDamageIndicatorO.transform;
        newDamageIndicatorTr.parent = mainCanvasRectTransform;

        BasicFunctions.ResetTransform(newDamageIndicatorTr);

        float scl = .75f;
        RectTransform rTr = newDamageIndicatorO.GetComponent<RectTransform>();
        rTr.localScale = (Vector3.one * scl);

        DamageIndicator newDamageIndicatorScript = newDamageIndicatorO.GetComponent<DamageIndicator>();
        newDamageIndicatorScript.useString = true;
        newDamageIndicatorScript.stringSet = _string;

        newDamageIndicatorScript.spawnPos = _pos;
        float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
        float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
        float maxOff = .25f;
        float xAdd = TommieRandom.instance.RandomRange(maxOff * .25f, maxOff) * xDir;
        float zAdd = TommieRandom.instance.RandomRange(maxOff * .25f, maxOff) * zDir;
        float yAdd = TommieRandom.instance.RandomRange(maxOff * .675f, maxOff);
        newDamageIndicatorScript.forceAdd = new Vector3(xAdd, yAdd, zAdd);
    }

    public void AddPlayerFire ( int _amount )
    {
        bool canAddPlayerFire = (_amount > 0 && SetupManager.instance.runDataRead.playerFireCur < SetupManager.instance.runDataRead.playerFireMax) || (_amount < 0);

        // log
        //Debug.Log("add player fire! || canAdd: " + canAddPlayerFire + " || " + Time.time.ToString());

        if ( canAddPlayerFire )
        {
            switch ( SetupManager.instance.curRunType )
            {
                case SetupManager.RunType.Normal:
                    SetupManager.instance.curProgressData.normalRunData.playerFireCur += _amount;
                    if ( SetupManager.instance.curProgressData.normalRunData.playerFireCur > SetupManager.instance.curProgressData.normalRunData.playerFireMax )
                    {
                        SetupManager.instance.curProgressData.normalRunData.playerFireCur = SetupManager.instance.curProgressData.normalRunData.playerFireMax;
                    }
                    break;
                case SetupManager.RunType.Endless:
                    SetupManager.instance.curProgressData.endlessRunData.playerFireCur += _amount;
                    if (SetupManager.instance.curProgressData.endlessRunData.playerFireCur > SetupManager.instance.curProgressData.endlessRunData.playerFireMax)
                    {
                        SetupManager.instance.curProgressData.endlessRunData.playerFireCur = SetupManager.instance.curProgressData.endlessRunData.playerFireMax;
                    }
                    break;
            }

            // audio
            AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.playerFireAddClips), .6f, .9f, .175f, .2f);
        }

        // UI flicker
        SetPlayerFireFlicker(3);
    }

    public void PlayerFoundKey ()
    {
        playerHasKey = true;

        // audio
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.foundKeyClips), .7f, .9f, .6f, .625f);
    }

    public void PlayerReachedEnd ( bool _loadLevelEnd )
    {
        //Debug.Log("we hebben het gehaald hoera || " + Time.time.ToString());

        bool reachedWithMaxHealth = false;
        bool reachedWithMaxFire = false;

        switch ( SetupManager.instance.curRunType )
        {
            case SetupManager.RunType.Normal:
                SetupManager.instance.curProgressData.normalRunData.playerReachedEnd = true;
                SetupManager.instance.curProgressData.normalRunData.curLevelIndex++;
                SetupManager.instance.curProgressData.normalRunData.encountersHad++;
                SaveManager.instance.StoreLastLevelData(ref SetupManager.instance.curProgressData.normalRunData);

                // achievement?
                if ( SetupManager.instance.curProgressData.normalRunData.playerHealthCur >= SetupManager.instance.curProgressData.normalRunData.playerHealthMax )
                {
                    reachedWithMaxHealth = true;
                }
                if (SetupManager.instance.curProgressData.normalRunData.playerFireCur >= SetupManager.instance.curProgressData.normalRunData.playerFireMax)
                {
                    reachedWithMaxFire = true;
                }
                break;

            case SetupManager.RunType.Endless:
                SetupManager.instance.curProgressData.endlessRunData.playerReachedEnd = true;
                SetupManager.instance.curProgressData.endlessRunData.curLevelIndex++;
                SetupManager.instance.curProgressData.endlessRunData.encountersHad++;
                SaveManager.instance.StoreLastLevelData(ref SetupManager.instance.curProgressData.endlessRunData);

                // achievement?
                if (SetupManager.instance.curProgressData.endlessRunData.playerHealthCur >= SetupManager.instance.curProgressData.endlessRunData.playerHealthMax)
                {
                    reachedWithMaxHealth = true;
                }
                if (SetupManager.instance.curProgressData.endlessRunData.playerFireCur >= SetupManager.instance.curProgressData.endlessRunData.playerFireMax)
                {
                    reachedWithMaxFire = true;
                }
                break;
        }

        // achievmenet?
        if ( reachedWithMaxHealth )
        {
            AchievementHelper.UnlockAchievement("ACHIEVEMENT_SOLID");
        }

        if (reachedWithMaxFire)
        {
            AchievementHelper.UnlockAchievement("ACHIEVEMENT_OVEN");
        }

        if (_loadLevelEnd)
        {
            // transition
            SetupManager.instance.SetTransition(SetupManager.TransitionMode.In);

            // progress data
            //SetupManager.instance.curProgressData.curLevelIndex++;

            // load new level?
            Invoke("LoadNextLevel", SetupManager.instance.sceneLoadWait);
        }

        //GameManager.instance.Playerheal
        //AddPlayerHealth(1);

        // save
        SaveManager.instance.WriteToFile(SetupManager.instance.curProgressData);

        // audio
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.stairsDescendClips), .7f, .9f, .6f, .625f);
    }

    public void LoadNextLevel ()
    {
        // clear all particles?
        SetupManager.instance.ClearAllParticles();

        // clear decals?
        if ( DecalManager.instance != null )
        {
            DecalManager.instance.ClearAllDecals();
        }

        // reset input
        InputManager.instance.DisableAllInput();

        // clear spawners
        SetupManager.instance.ClearAllPropSpawners();
        SetupManager.instance.ClearAllNpcSpawners();
        SetupManager.instance.ClearAllFountainAreas();

        // go
        SetupManager.instance.SetGameState(SetupManager.GameState.LevelEnd);
        SceneManager.LoadScene(3 + 1,LoadSceneMode.Single);
    }

    public void SetPlayerFireFlicker ( int _count )
    {
        playerFireFlickerCountMax = _count;
        playerFireFlickerCount = 0;
        playerFireFlickerIndex = 0;
        playerFireFlickerRate = 6;
        playerFireFlickerCounter = 0;
    }

    public void SetPlayerFireButtonFlicker ( int _count )
    {
        playerFireButtonFlickerCountMax = _count;
        playerFireButtonFlickerCount = 0;
        playerFireButtonFlickerIndex = 0;
        playerFireButtonFlickerRate = 6;
        playerFireButtonFlickerCounter = 0;
    }

    public void SetPlayerHealthFlicker(int _count)
    {
        playerHealthFlickerCountMax = _count;
        playerHealthFlickerCount = 0;
        playerHealthFlickerIndex = 0;
        playerHealthFlickerRate = 6;
        playerHealthFlickerCounter = 0;
    }

    public void AddCoinAmount ( int _amount )
    {
        //coinsCollected += _amount;
        //if ( coinsCollected <= 0 )
        //{
        //    coinsCollected = 0;
        //}

        int coinCount = 0;

        switch ( SetupManager.instance.curRunType )
        {
            case SetupManager.RunType.Normal:

                //if (SetupManager.instance.curProgressData.normalRunData.curLevelCoinsCollected < SetupManager.instance.curLevelMaxCoins)
                {
                    SetupManager.instance.curProgressData.normalRunData.curLevelCoinsCollected += _amount;
                    SetupManager.instance.curProgressData.normalRunData.curRunCoinsCollected += _amount;
                    if ( SetupManager.instance.curProgressData.normalRunData.curLevelCoinsCollected <= 0)
                    {
                        SetupManager.instance.curProgressData.normalRunData.curLevelCoinsCollected = 0;
                    }
                    if (SetupManager.instance.curProgressData.normalRunData.curRunCoinsCollected <= 0)
                    {
                        SetupManager.instance.curProgressData.normalRunData.curRunCoinsCollected = 0;
                    }

                    coinCount = SetupManager.instance.curProgressData.normalRunData.curRunCoinsCollected;
                }

                break;

            case SetupManager.RunType.Endless:

                //if (SetupManager.instance.curProgressData.endlessRunData.curLevelCoinsCollected < SetupManager.instance.curLevelMaxCoins)
                {
                    SetupManager.instance.curProgressData.endlessRunData.curLevelCoinsCollected += _amount;
                    SetupManager.instance.curProgressData.endlessRunData.curRunCoinsCollected += _amount;
                    if (SetupManager.instance.curProgressData.endlessRunData.curLevelCoinsCollected <= 0)
                    {
                        SetupManager.instance.curProgressData.endlessRunData.curLevelCoinsCollected = 0;
                    }
                    if (SetupManager.instance.curProgressData.endlessRunData.curRunCoinsCollected <= 0)
                    {
                        SetupManager.instance.curProgressData.endlessRunData.curRunCoinsCollected = 0;
                    }

                    coinCount = SetupManager.instance.curProgressData.endlessRunData.curRunCoinsCollected;
                }

                break;
        }

        // achievement?
        if (coinCount >= 50)
        {
            AchievementHelper.UnlockAchievement("ACHIEVEMENT_LOTSOFCOINS");
        }
    }

    public void AddTearAmount(int _amount)
    {
        //tearsCollected += _amount;
        //if (tearsCollected <= 0)
        //{
        //    tearsCollected = 0;
        //}

        int tearCount = 0;

        switch ( SetupManager.instance.curRunType )
        {
            case SetupManager.RunType.Normal:

                //if (SetupManager.instance.curProgressData.normalRunData.curLevelTearsCollected < SetupManager.instance.curLevelMaxTears)
                {
                    SetupManager.instance.curProgressData.normalRunData.curLevelTearsCollected += _amount;
                    SetupManager.instance.curProgressData.normalRunData.curRunTearsCollected += _amount;
                    if (SetupManager.instance.curProgressData.normalRunData.curLevelTearsCollected <= 0)
                    {
                        SetupManager.instance.curProgressData.normalRunData.curLevelTearsCollected = 0;
                    }
                    if (SetupManager.instance.curProgressData.normalRunData.curRunTearsCollected <= 0)
                    {
                        SetupManager.instance.curProgressData.normalRunData.curRunTearsCollected = 0;
                    }

                    tearCount = SetupManager.instance.curProgressData.normalRunData.curRunTearsCollected;
                }

                break;

            case SetupManager.RunType.Endless:

                //if (SetupManager.instance.curProgressData.endlessRunData.curLevelTearsCollected < SetupManager.instance.curLevelMaxTears)
                {
                    SetupManager.instance.curProgressData.endlessRunData.curLevelTearsCollected += _amount;
                    SetupManager.instance.curProgressData.endlessRunData.curRunTearsCollected += _amount;
                    if (SetupManager.instance.curProgressData.endlessRunData.curLevelTearsCollected <= 0)
                    {
                        SetupManager.instance.curProgressData.endlessRunData.curLevelTearsCollected = 0;
                    }
                    if (SetupManager.instance.curProgressData.endlessRunData.curRunTearsCollected <= 0)
                    {
                        SetupManager.instance.curProgressData.endlessRunData.curRunTearsCollected = 0;
                    }

                    tearCount = SetupManager.instance.curProgressData.endlessRunData.curRunTearsCollected;
                }

                break;
        }

        // achievement?
        if (tearCount>=50)
        {
            AchievementHelper.UnlockAchievement("ACHIEVEMENT_LOTSOFTEARS");
        }

        SetupManager.instance.UpdateRunDataRead();
    }

    public void CalculateMaxCollectiblesInLevel ()
    {
        int coinsMax = 0;
        int tearsMax = 0;
        if ( allVernietigbaarScripts != null && allVernietigbaarScripts.Count > 0 )
        {
            for ( int i = 0; i < allVernietigbaarScripts.Count; i ++ )
            {
                Vernietigbaar curVernietigbaar = allVernietigbaarScripts[i];
                if (curVernietigbaar != null)
                {
                    switch (curVernietigbaar.myDropType)
                    {
                        case Vernietigbaar.DropType.Coin: coinsMax += curVernietigbaar.coinDropCount; break;
                    }
                }
            }
        }
        if (LevelGeneratorManager.instance.activeLevelGenerator != null && LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs != null && LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs.Count > 0)
        {
            for ( int i = 0; i < LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs.Count; i ++ )
            {
                tearsMax += LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs[i].tearDropCount;
            }
        }
        SetupManager.instance.curLevelMaxCoins = coinsMax;
        SetupManager.instance.curLevelMaxTears = tearsMax;

        // log
        //Debug.Log("coins in level: " + coinsMax.ToString() + " || tears in level: " + tearsMax.ToString() + " || " + Time.time.ToString());
    }

    public void SetCanCheckInteraction ( int _dur )
    {
        canCheckInteractionDur = _dur;
        canCheckInteractionCounter = 0;
    }

    public void SetInteractionType ( InteractionType _to, InteractionScript _script )
    {
        if (canCheckInteractionCounter >= canCheckInteractionDur)
        {
            string baseCol = SetupManager.instance.UIInteractionSecondCol;
            string subCol = SetupManager.instance.UIInteractionSubCol;
            string interactCol = SetupManager.instance.UIInteractionButtonCol;
            string tearCol = SetupManager.instance.UITearCol;
            string coinCol = SetupManager.instance.UICoinCol;

            interactionCheckCounter = 0;
            curInteractionScript = _script;

            switch (_to)
            {
                case InteractionType.LevelEndDoor:
                    string levelEndDoorString;
                    if (SetupManager.instance.curEncounterType != SetupManager.EncounterType.Small)
                    {
                        if (playerHasKey)
                        {
                            levelEndDoorString = interactCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - unlock door";
                        }
                        else
                        {
                            levelEndDoorString = baseCol + "the door is locked";
                        }
                    }
                    else
                    {
                        levelEndDoorString = interactCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - open door";
                    }
                    interactionString = levelEndDoorString;
                    break;

                case InteractionType.Fountain:
                    bool playerHasEnoughTears = (SetupManager.instance.runDataRead.curRunTearsCollected >= SetupManager.instance.blessingTearCost);
                    string fountainCheckString;
                    //fountainCheckString = "run tears: " + SetupManager.instance.runDataRead.curRunTearsCollected + ", cost: " + SetupManager.instance.blessingTearCost.ToString();
                    if (playerHasEnoughTears)
                    {
                        fountainCheckString = interactCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - fill the fountain";
                        fountainCheckString += "\n";
                        fountainCheckString += "with tears";
                        fountainCheckString += "\n";
                        fountainCheckString += subCol + "costs " + tearCol + SetupManager.instance.blessingTearCost.ToString() + subCol + " tears";
                    }
                    else
                    {
                        fountainCheckString = tearCol + SetupManager.instance.blessingTearCost + baseCol + " tears are needed";
                        fountainCheckString += "\n";
                        fountainCheckString += "to fill the fountain";
                    }
                    interactionString = fountainCheckString;
                    break;

                case InteractionType.FountainDoor:
                    interactionString = interactCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - leave";
                    break;

                case InteractionType.RestDoor:
                    interactionString = interactCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - leave";
                    break;

                case InteractionType.ShopDoor:
                    interactionString = interactCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - leave";
                    break;

                case InteractionType.StartDoor:
                    interactionString = baseCol + "let's not go back!";
                    break;

                case InteractionType.ShopItem:
                    int shopCoinCostUse = curInteractionScript.myEquipmentData.shopCoinCost;
                    if ( SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Thrifty) )
                    {
                        shopCoinCostUse = Mathf.RoundToInt(shopCoinCostUse * .66f);
                    }

                    interactionString = interactCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - inspect item";

                    string shopInspectString = "";
                    shopInspectString += baseCol + curInteractionScript.myEquipmentData.name;
                    shopInspectString += "\n";
                    shopInspectString += subCol + "costs " + coinCol + shopCoinCostUse.ToString() + subCol + " coins";
                    SetShopInspect(shopInspectString);
                    break;

                case InteractionType.RestCampfire:
                    string restCampfireString = interactCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - rest";
                    interactionString = restCampfireString;
                    break;
            }
            curInteractionType = _to;

            if (interactionCheckText != null)
            {
                interactionCheckText.text = interactionString;
            }
        }
        else
        {
            interactionCheckCounter = interactionCheckDur;
            inInteraction = false;
        }
    }

    void SetShopInspect ( string _string )
    {
        shopInspectText.text = _string;
        shopInspectCounter = 0;
    }

    void HandleInteractions ()
    {
        if ( inInteraction && !playerHurt && !SetupManager.instance.runDataRead.playerDead)
        {
            bool performInteraction = false;
            if ( SetupManager.instance.canInteract && InputManager.instance.interactPressed )
            {
                switch (curInteractionType)
                {
                    case InteractionType.Fountain:
                        bool playerHasEnoughTears = ( SetupManager.instance.runDataRead.curRunTearsCollected >= SetupManager.instance.blessingTearCost );
                        if (playerHasEnoughTears)
                        {
                            performInteraction = true;
                        }
                        break;

                    case InteractionType.FountainDoor:
                        performInteraction = true;
                        break;

                    case InteractionType.ShopDoor:
                        performInteraction = true;
                        break;

                    case InteractionType.LevelEndDoor:
                        if (playerHasKey || (SetupManager.instance.curEncounterType == SetupManager.EncounterType.Small))
                        {
                            performInteraction = true;
                        }
                        break;

                    case InteractionType.ShopItem:
                        performInteraction = true;
                        break;

                    case InteractionType.RestCampfire:
                        performInteraction = true;
                        break;

                    case InteractionType.RestDoor:
                        performInteraction = true;
                        break;
                }

                if (performInteraction)
                {
                    curInteractionScript.Trigger();
                    //interactionCheckCounter = interactionCheckDur;
                    //inInteraction = false;
                }
            }
        }
    }

    public void ClearAllDamageDeals()
    {
        if (allDamageDeals != null)
        {
            for (int i = allDamageDeals.Count - 1; i >= 0; i--)
            {
                allDamageDeals[i].Clear();
            }
            allDamageDeals.Clear();
        }
    }

    public void SpawnNpc(Npc.Type _type, Vector3 _pos, NpcSpawner _spawnedBy, bool _isBoss, bool _applySpawnOffset, bool _preventTearDrop, MovementTransformContainer _movementTransformContainer, MinionSpawnTransformContainer _minionSpawnTransformContainer, CannonScriptContainer _cannonScriptContainer, NpcCore _minionMaster)
    {
        // load object to spawn, according to npc type
        GameObject npcSpawnO = NpcDatabase.instance.LoadPrefab(_type);

        // position
        float spawnOffMax = .125f;
        Vector3 spawnP = _pos;//myTransform.position;
        spawnP.x += TommieRandom.instance.RandomRange(-spawnOffMax, spawnOffMax);
        spawnP.z += TommieRandom.instance.RandomRange(-spawnOffMax, spawnOffMax);

        if (_applySpawnOffset)
        {
            float spawnOffExtra = (spawnOffMax * 2f);
            spawnP.x += TommieRandom.instance.RandomRange(-spawnOffExtra, spawnOffExtra);
            spawnP.z += TommieRandom.instance.RandomRange(-spawnOffExtra, spawnOffExtra);
        }

        // rotation
        Quaternion spawnR = Quaternion.Euler(0f, TommieRandom.instance.RandomRange(0f, 360f), 0f);

        // spawn
        GameObject npcO = PrefabManager.instance.SpawnPrefabAsGameObject(npcSpawnO, spawnP, spawnR, 1f);
        if (npcO.GetComponent<NpcCore>() != null)
        {
            NpcCore newNpcCore = npcO.GetComponent<NpcCore>();
            newNpcCore.spawnedBy = _spawnedBy;//this;
            if (_spawnedBy != null && _spawnedBy.myNpcCores != null)
            {
                newNpcCore.spawnedByIndex = _spawnedBy.myNpcCores.Count;
            }

            if (_isBoss)
            {
                newNpcCore.isBoss = true;

                // movement point references
                if (_movementTransformContainer != null)
                {
                    newNpcCore.movementTransformContainer = _movementTransformContainer;
                }

                // minion spawn point references
                if (_minionSpawnTransformContainer != null)
                {
                    newNpcCore.minionSpawnTransformContainer = _minionSpawnTransformContainer;
                }

                // cannon script references
                if (_cannonScriptContainer != null)
                {
                    newNpcCore.cannonScriptContainer = _cannonScriptContainer;
                    newNpcCore.cannonScripts = new List<CannonScript>();
                    for ( int i = 0; i < _cannonScriptContainer.cannonScripts.Count; i ++ )
                    {
                        newNpcCore.cannonScripts.Add(_cannonScriptContainer.cannonScripts[i]);
                    }
                }
            }

            if (_preventTearDrop)
            {
                newNpcCore.preventTearDrop = true;
            }

            // is a minion?
            if (_minionMaster != null)
            {
                newNpcCore.minionMaster = _minionMaster;
                newNpcCore.autoAlerted = true;
                _minionMaster.AddMinion(newNpcCore);
            }

            // store
            if (_spawnedBy != null)
            {
                _spawnedBy.myNpcCores.Add(newNpcCore);
                _spawnedBy.myNpcsDefeated.Add(false);
            }
        }

        // increase npc count
        SetupManager.instance.npcCount++;
    }

    public CannonScript GetFurthestCannon ( Vector3 _from )
    {
        CannonScript ret = null;
        float furthestDst = 0f;
        for ( int i = 0; i < LevelGeneratorManager.instance.activeLevelGenerator.allCannonScripts.Count; i ++ )
        {
            CannonScript cannonScriptCheck = LevelGeneratorManager.instance.activeLevelGenerator.allCannonScripts[i];
            if ( cannonScriptCheck != null && cannonScriptCheck.curState == CannonScript.State.Idle )
            {
                Vector3 p0 = _from;
                Vector3 p1 = cannonScriptCheck.myTransform.position;
                p1.y = p0.y;
                float d0 = Vector3.Distance(p0,p1);
                if (d0 > furthestDst)
                {
                    ret = cannonScriptCheck;
                    furthestDst = d0;
                }
            }
        }
        return ret;
    }

    public bool CheckIfFireInRadius ( Vector3 _from, float _radius, int _countWanted )
    {
        int count = 0;
        if ( fireScripts != null && fireScripts.Count > 0 )
        {
            for ( int i = 0; i < fireScripts.Count; i ++ )
            {
                FireScript fireScriptCheck = fireScripts[i];
                if ( fireScriptCheck != null )
                {
                    Vector3 p0 = _from;
                    Vector3 p1 = fireScriptCheck.myTransform.position;
                    p1.y = p0.y;
                    float d0 = Vector3.Distance(p0,p1);
                    if ( d0 <= _radius )
                    {
                        count++;
                    }
                }
            }
        }
        return (count >= _countWanted);
    }

    void CheckForTutorialPopups ()
    {
        if (SetupManager.instance.canShowTutorialPopupCounter >= SetupManager.instance.canShowTutorialPopupDur)
        {
            string baseCol = SetupManager.instance.UIPopupBaseCol;
            string inputCol = SetupManager.instance.UIPopupInputCol;

            // combat popup
            if (!SetupManager.instance.curProgressData.persistentData.sawCombatPopup)
            {
                Vector3 p0 = playerFirstPersonDrifter.myTransform.position;
                if (LevelGeneratorManager.instance != null && LevelGeneratorManager.instance.activeLevelGenerator != null && LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs != null)
                {
                    for (int i = 0; i < LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs.Count; i++)
                    {
                        NpcCore npcCheck = LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs[i];
                        Vector3 p1 = npcCheck.myTransform.position;
                        float d0 = Vector3.Distance(p0, p1);
                        if (d0 <= 4f)
                        {
                            SetupManager.instance.SetTutorialPopup(baseCol + "attack by pressing " + "\n" + inputCol + InputManager.instance.meleeAttackInputStringUse + "\n\n" + baseCol + "block by holding " + "\n" + inputCol + InputManager.instance.blockInputStringUse, 60);
                            SetupManager.instance.curProgressData.persistentData.sawCombatPopup = true;
                        }
                    }
                }
            }

            // fire popup
            if (!SetupManager.instance.curProgressData.persistentData.sawFirePopup && !playerInCombat)
            {
                if (fireScripts != null && fireScripts.Count > 0)
                {
                    for (int i = 0; i < fireScripts.Count; i++)
                    {
                        FireScript fireCheck = fireScripts[i];
                        if (fireCheck != null && fireCheck.canBurnPlayer)
                        {
                            Vector3 f0 = fireCheck.myTransform.position;
                            Vector3 f1 = playerFirstPersonDrifter.myTransform.position;
                            float d0 = Vector3.Distance(f0, f1);
                            if (d0 <= 2f)
                            {
                                SetupManager.instance.SetTutorialPopup(baseCol + "fire is dangerous! absorb it" + "\n" + "by holding " + inputCol + InputManager.instance.magicAttackInputStringUse, 60);
                                SetupManager.instance.curProgressData.persistentData.sawFirePopup = true;
                            }
                        }
                    }
                }
            }

            // fire collected popup
            if (fireCollected)
            {
                if (!SetupManager.instance.curProgressData.persistentData.sawFireCollectPopup)
                {
                    SetupManager.instance.SetTutorialPopup(baseCol + "you absorbed fire! shoot it by" + "\n" + "presing " + inputCol + InputManager.instance.magicAttackInputStringUse, 60);
                    SetupManager.instance.curProgressData.persistentData.sawFireCollectPopup = true;
                }
            }

            // kick popup
            if (!SetupManager.instance.curProgressData.persistentData.sawKickPopup)
            {
                Vector3 p0 = playerFirstPersonDrifter.myTransform.position;
                if (LevelGeneratorManager.instance != null && LevelGeneratorManager.instance.activeLevelGenerator != null && LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs != null)
                {
                    for (int i = 0; i < LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs.Count; i++)
                    {
                        NpcCore npcCheck = LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs[i];
                        if (npcCheck.myType == Npc.Type.GoblinChieftain || npcCheck.myType == Npc.Type.Skeleton || npcCheck.myType == Npc.Type.OrcWarrior)
                        {
                            Vector3 p1 = npcCheck.myTransform.position;
                            float d0 = Vector3.Distance(p0, p1);
                            if (d0 <= 6f)
                            {
                                SetupManager.instance.SetTutorialPopup(baseCol + "kick by pressing " + inputCol + InputManager.instance.kickInputStringUse + "\n" + baseCol + "enemies can't block that!", 60);
                                SetupManager.instance.curProgressData.persistentData.sawKickPopup = true;
                            }
                        }
                    }
                }
            }

            // coin popup
            if (coinCollected)
            {
                if (!SetupManager.instance.curProgressData.persistentData.sawCoinPopup && !playerInCombat)
                {
                    SetupManager.instance.SetTutorialPopup(baseCol + "found a coin! you better" + "\n" + "hold onto it", 60);
                    SetupManager.instance.curProgressData.persistentData.sawCoinPopup = true;
                }
            }

            // tear popup
            if (tearCollected)
            {
                if (!SetupManager.instance.curProgressData.persistentData.sawTearPopup && !playerInCombat)
                {
                    SetupManager.instance.SetTutorialPopup(baseCol + "found a tear! some enemies" + "\n" + "shed them when defeated", 60);
                    SetupManager.instance.curProgressData.persistentData.sawTearPopup = true;
                }
            }

            // donut popup
            if (donutCollected)
            {
                if (!SetupManager.instance.curProgressData.persistentData.sawDonutPopup && !playerInCombat)
                {
                    SetupManager.instance.SetTutorialPopup(baseCol + "found a donut! it heals" + "\n" + "some health", 60);
                    SetupManager.instance.curProgressData.persistentData.sawDonutPopup = true;
                }
            }

            // key popup
            if (keyCollected)
            {
                if (!SetupManager.instance.curProgressData.persistentData.sawKeyPopup)
                {
                    SetupManager.instance.SetTutorialPopup(baseCol + "found a key! now to find" + "\n" + "a door", 60);
                    SetupManager.instance.curProgressData.persistentData.sawKeyPopup = true;
                }
            }

            // fountain popup
            if (!SetupManager.instance.curProgressData.persistentData.sawFountainPopup)
            {
                if (!playerInCombat)
                {
                    if (FountainManager.instance != null && FountainManager.instance.fountainScript != null && !FountainManager.instance.fountainScript.broken)
                    {
                        Transform trUse = (FountainManager.instance.fountainScript.myTransform == null) ? FountainManager.instance.fountainScript.transform : FountainManager.instance.fountainScript.myTransform;
                        Vector3 f0 = playerFirstPersonDrifter.myTransform.position;
                        Vector3 f1 = trUse.position;
                        float d0 = Vector3.Distance(f0, f1);
                        if (d0 <= 3f)
                        {
                            SetupManager.instance.SetTutorialPopup(baseCol + "drop tears in the fountain to" + "\n" + "get blessed by the moon!", 60);
                            SetupManager.instance.curProgressData.persistentData.sawFountainPopup = true;
                        }
                    }
                }
            }
        }
    }
}
