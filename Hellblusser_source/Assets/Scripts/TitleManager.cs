using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleManager : MonoBehaviour
{
    // instance
    public static TitleManager instance;

    // UI
    [Header("UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI titleTextBack;

    public RectTransform startGameRectTransform;
    public TextMeshProUGUI startGameText;
    public Image startGameBack;

    public TextMeshProUGUI runInfoText;

    public RectTransform runSelectArrowLeftRectTransform;
    public RectTransform runSelectArrowRightRectTransform;
    public Image runSelectArrowLeft;
    public Image runSelectArrowRight;

    // player
    [Header("player")]
    public PlayerCharacterScreen playerScript;

    // counters
    int hideUIDur, hideUICounter;
    [HideInInspector] public bool showUI;

    // run select
    [HideInInspector] public int runSelectIndex;
    int canSwitchRunIndexDur, canSwitchRunIndexCounter;

    // state
    [HideInInspector] public bool startingGame;

    void Awake ()
    {
        instance = this;
    }

    void Start()
    {
        // state
        startingGame = false;

        // counters
        hideUIDur = 90;
        hideUICounter = 0;

        // transition on start
        SetupManager.instance.InitStartTransition();
    }

    void Update ()
    {
        // hide UI?
        if (hideUICounter < hideUIDur)
        {
            if ( SetupManager.instance.curGameState == SetupManager.GameState.Title && !SetupManager.instance.paused )
            {
                hideUICounter++;
            }
        }
        showUI = (hideUICounter >= hideUIDur && !startingGame && !SetupManager.instance.paused && !SetupManager.instance.hidePlayerUI && !SetupManager.instance.inTransition);

        if ( showUI )
        {
            if ( !SetupManager.instance.curProgressData.persistentData.sawEndlessPopup && SetupManager.instance.curProgressData.persistentData.unlockedEndless )
            {
                SetupManager.instance.SetTutorialPopup(SetupManager.instance.UIPopupBaseCol + "unlocked endless mode!",60);
                SetupManager.instance.curProgressData.persistentData.sawEndlessPopup = true;
            }
        }

        // text
        if ( titleText != null )
        {
            titleText.enabled = (showUI && (SetupManager.instance.tutorialPopupCounter >= SetupManager.instance.tutorialPopupDur));
        }
        if (titleTextBack != null)
        {
            titleTextBack.enabled = (showUI && (SetupManager.instance.tutorialPopupCounter >= SetupManager.instance.tutorialPopupDur));
        }

        // run type selection
        if (showUI && !SetupManager.instance.paused && !SetupManager.instance.inTransition && !startingGame && SetupManager.instance.curGameState != SetupManager.GameState.Intro && (SetupManager.instance.tutorialPopupCounter >= SetupManager.instance.tutorialPopupDur) )
        {
            float hInput = InputManager.instance.moveDirection.x;
            float switchThreshold = .75f;
            if (canSwitchRunIndexCounter < canSwitchRunIndexDur)
            {
                if (Mathf.Abs(hInput) <= switchThreshold)
                {
                    canSwitchRunIndexCounter = canSwitchRunIndexDur;
                }
            }
            if (canSwitchRunIndexCounter >= canSwitchRunIndexDur)
            {
                if (runSelectIndex > 0 && hInput < -switchThreshold)
                {
                    runSelectIndex--;
                    canSwitchRunIndexCounter = 0;
                    UpdateRunSelectArrows();

                    // audio
                    SetupManager.instance.PlayUINavigateSound();
                }
                if (runSelectIndex < 1 && hInput > switchThreshold)
                {
                    runSelectIndex++;
                    canSwitchRunIndexCounter = 0;
                    UpdateRunSelectArrows();

                    // audio
                    SetupManager.instance.PlayUINavigateSound();
                }
            }
        }

        if (startGameText != null)
        {
            startGameText.enabled = (showUI && (SetupManager.instance.tutorialPopupCounter >= SetupManager.instance.tutorialPopupDur));
            if ( showUI )
            {
                string startStringFront = SetupManager.instance.UIInteractionButtonCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol;
                string startString = "";
                switch ( SetupManager.instance.curRunType )
                {
                    case SetupManager.RunType.Normal:
                        startString = startStringFront + ((SetupManager.instance.curProgressData.persistentData.inNormalRun) ? " - resume" : " - start");
                        break;

                    case SetupManager.RunType.Endless:
                        if (SetupManager.instance.curProgressData.persistentData.unlockedEndless)
                        {
                            startString = startStringFront + ((SetupManager.instance.curProgressData.persistentData.inEndlessRun) ? " - resume" : " - start");
                        }
                        else
                        {
                            startString = SetupManager.instance.UILockedCol + "locked";
                        }
                        break;
                }
                startGameText.text = startString;
            }
        }

        if (runInfoText != null )
        {
            string runInfoString = "";
            switch ( SetupManager.instance.curRunType )
            {
                case SetupManager.RunType.Normal:

                    if ( SetupManager.instance.curProgressData.persistentData.inNormalRun )
                    {
                        runInfoString = "normal" + "\n";
                        string floorAdd = "";
                        switch (SetupManager.instance.curProgressData.normalRunData.curFloorIndex)
                        {
                            case 1: floorAdd = "sewer"; break;
                            case 2: floorAdd = "dungeon"; break;
                            case 3: floorAdd = "hell"; break;
                        }
                        string highestFloorAdd = "";
                        switch (SetupManager.instance.curProgressData.persistentData.normalHighestFloorIndex)
                        {
                            case 1: highestFloorAdd = "sewer"; break;
                            case 2: highestFloorAdd = "dungeon"; break;
                            case 3: highestFloorAdd = "hell"; break;
                        }
                        runInfoString += floorAdd + " " + (SetupManager.instance.curProgressData.normalRunData.curLevelIndex + 1).ToString() + "\n";
                        //runInfoString += "best: " + highestFloorAdd + " " + (SetupManager.instance.curProgressData.normalRunData.highestLevelIndex + 1).ToString() + "\n";
                        if (SetupManager.instance.curProgressData.normalRunData.runTimeOnStartOfLevel > 1f)
                        {
                            runInfoString += SetupManager.instance.GetRunPlayTime(SetupManager.instance.curProgressData.normalRunData);
                        }
                    }
                    else
                    {
                        runInfoString = "normal";
                    }

                    break;

                case SetupManager.RunType.Endless:

                    if ( SetupManager.instance.curProgressData.persistentData.unlockedEndless)
                    {
                        if ( SetupManager.instance.curProgressData.persistentData.inEndlessRun )
                        {
                            runInfoString = "endless" + "\n";
                            string floorAdd = "";
                            switch ( SetupManager.instance.curProgressData.endlessRunData.curFloorIndex )
                            {
                                case 1: floorAdd = "sewer"; break;
                                case 2: floorAdd = "dungeon"; break;
                                case 3: floorAdd = "hell"; break;
                            }
                            //string highestFloorAdd = "";
                            //switch (SetupManager.instance.curProgressData.endlessRunData.highestFloorIndex)
                            //{
                            //    case 1: highestFloorAdd = "sewer"; break;
                            //    case 2: highestFloorAdd = "dungeon"; break;
                            //    case 3: highestFloorAdd = "hell"; break;
                            //}
                            runInfoString += "loop " + SetupManager.instance.curProgressData.endlessRunData.curLoopIndex.ToString() + " " + floorAdd + " " + (SetupManager.instance.curProgressData.endlessRunData.curLevelIndex + 1).ToString() + "\n";
                            //runInfoString += "best: " + "loop " + SetupManager.instance.curProgressData.endlessRunData.highestLoopIndex.ToString() + " " + highestFloorAdd + " " + SetupManager.instance.curProgressData.endlessRunData.highestLevelIndex.ToString() + "\n";
                            if (SetupManager.instance.curProgressData.endlessRunData.runTimeOnStartOfLevel > 1f)
                            {
                                runInfoString += SetupManager.instance.GetRunPlayTime(SetupManager.instance.curProgressData.endlessRunData);
                            }
                        }
                        else
                        {
                            runInfoString = "endless";// + "\n";
                            string highestFloorAdd = "";
                            switch (SetupManager.instance.curProgressData.persistentData.endlessHighestFloorIndex)
                            {
                                case 1: highestFloorAdd = "sewer"; break;
                                case 2: highestFloorAdd = "dungeon"; break;
                                case 3: highestFloorAdd = "hell"; break;
                            }
                            if (!(SetupManager.instance.curProgressData.persistentData.endlessHighestFloorIndex == 0 && SetupManager.instance.curProgressData.persistentData.endlessHighestLevelIndex == 0 && SetupManager.instance.curProgressData.persistentData.endlessHighestLevelIndex == 0))
                            {
                                runInfoString += "best: " + "loop " + SetupManager.instance.curProgressData.persistentData.endlessHighestLoopIndex.ToString() + " " + highestFloorAdd + " " + (SetupManager.instance.curProgressData.persistentData.endlessHighestLevelIndex + 1).ToString() + "\n";
                            }
                        }
                    }
                    else
                    {
                        runInfoString = "???";
                    }

                    break;
            }
            runInfoText.text = runInfoString;
            runInfoText.enabled = (showUI && (SetupManager.instance.tutorialPopupCounter >= SetupManager.instance.tutorialPopupDur));
        }

        if ( startGameBack != null )
        {
            startGameBack.enabled = (showUI && (SetupManager.instance.tutorialPopupCounter >= SetupManager.instance.tutorialPopupDur));
        }

        // run selection arrows
        UpdateRunSelectArrows();

        // select run type
        switch (runSelectIndex)
        {
            case 0: SetRunType(SetupManager.RunType.Normal); break;
            case 1: SetRunType(SetupManager.RunType.Endless); break;
        }

        // start game?
        bool canStartGame = (!startingGame && !SetupManager.instance.inTransition && (showUI && (SetupManager.instance.tutorialPopupCounter >= SetupManager.instance.tutorialPopupDur)));
        if (canStartGame)
        {
            if (SetupManager.instance.canInteract && InputManager.instance.interactPressed)
            {
                bool canStartRunType = false;
                switch ( SetupManager.instance.curRunType )
                {
                    case SetupManager.RunType.Normal:
                        canStartRunType = true;
                        break;
                    case SetupManager.RunType.Endless:
                        canStartRunType = (SetupManager.instance.curProgressData.persistentData.unlockedEndless);
                        break;
                }
                if (canStartRunType)
                {
                    SetupManager.instance.SetTransition(SetupManager.TransitionMode.In);
                    Invoke("InitStartGame", SetupManager.instance.sceneLoadWait);
                    startingGame = true;

                    // audio
                    SetupManager.instance.PlayUISelectSound();
                }
            }
        }
    }

    void UpdateRunSelectArrows ()
    {
        float arrowHorPadding = 20f;
        if (runSelectArrowLeft != null)
        {
            runSelectArrowLeftRectTransform.anchoredPosition = startGameRectTransform.anchoredPosition + new Vector2(-((startGameRectTransform.sizeDelta.x * .5f) + arrowHorPadding), 0f);
            runSelectArrowLeft.enabled = ((showUI && (SetupManager.instance.tutorialPopupCounter >= SetupManager.instance.tutorialPopupDur)) && runSelectIndex > 0);
        }
        if (runSelectArrowRight != null)
        {
            runSelectArrowRightRectTransform.anchoredPosition = startGameRectTransform.anchoredPosition + new Vector2(((startGameRectTransform.sizeDelta.x * .5f) + arrowHorPadding), 0f);
            runSelectArrowRight.enabled = ((showUI && (SetupManager.instance.tutorialPopupCounter >= SetupManager.instance.tutorialPopupDur)) && runSelectIndex < 1);
        }
    }

    void InitStartGame ()
    {
        StartGame();
    }

    void StartGame ()
    {
        // progress data
        switch ( SetupManager.instance.curRunType )
        {
            // START OR LOAD A NORMAL RUN
            case SetupManager.RunType.Normal:
                if (SetupManager.instance.curProgressData.persistentData.inNormalRun) // load normal data
                {
                    SetupManager.instance.LoadRun(SetupManager.RunType.Normal);
                }
                else // create new normal run data
                {
                    SetupManager.instance.CreateNewRun(SetupManager.RunType.Normal);
                }
                break;

            // START OR LOAD AN ENDLESS RUN
            case SetupManager.RunType.Endless:
                if (SetupManager.instance.curProgressData.persistentData.inEndlessRun) // load endless data
                {
                    SetupManager.instance.LoadRun(SetupManager.RunType.Endless);
                }
                else // create new endless run data
                {
                    SetupManager.instance.CreateNewRun(SetupManager.RunType.Endless);
                }
                break;
        }

        // save
        SaveManager.instance.WriteToFile(SetupManager.instance.curProgressData);

        // seed
        TommieRandom.instance.__generation_random_index = 0;
        TommieRandom.instance.__random_index = 0;

        // state
        SetupManager.instance.SetGameState(SetupManager.GameState.LevelSelect);

        // clear all particles?
        if ( SetupManager.instance != null )
        {
            SetupManager.instance.ClearAllParticles();
        }

        // clear decals?
        if (DecalManager.instance != null)
        {
            DecalManager.instance.ClearAllDecals();
        }

        // reset input
        InputManager.instance.DisableAllInput();

        // transition
        SetupManager.instance.transitionFactor = 0f;
        SceneManager.LoadScene(7 + 1, LoadSceneMode.Single);
    }

    public void SetRunType ( SetupManager.RunType _to )
    {
        bool wasAlreadyThisType = (SetupManager.instance.curRunType == _to);
        SetupManager.instance.curRunType = _to;
        if ( !wasAlreadyThisType)
        {
            SetupManager.instance.UpdateRunDataRead();
            playerScript.CreateEquipment();

            // log
            //Debug.Log("");
        }
    }
}
