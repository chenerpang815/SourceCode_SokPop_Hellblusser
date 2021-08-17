using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    // state
    [HideInInspector] public bool leaving;

    // UI
    [Header("UI")]
    public TextMeshProUGUI gameOverTitleText;
    public RectTransform gameOverTitleRectTransform;
    public TextMeshProUGUI gameOverTitleBackText;
    public RectTransform gameOverTitleBackRectTransform;

    public TextMeshProUGUI gameOverProceedText;
    public Image gameOverProceedImage;

    public TextMeshProUGUI levelReachedText;
    public TextMeshProUGUI levelHighestReachedText;

    public TextMeshProUGUI playTimeText;

    // overlay
    [Header("overlay")]
    public MeshRenderer overlayMeshRenderer;
    public Color overlayColShow, overlayColHide;
    Color overlayColCur;

    // state
    int showProgressDur, showProgressCounter;
    int proceedShowWaitDur, proceedShowWaitCounter;

    // animation
    Vector3 gameOverPosOriginal, gameOverPosTarget, gameOverPosCur;

    void Awake ()
    {
        instance = this;
    }

    void Start ()
    {
        // state
        leaving = false;
        showProgressDur = 180;
        showProgressCounter = 0;

        proceedShowWaitDur = 120;
        proceedShowWaitCounter = 0;

        // animation
        gameOverPosOriginal = gameOverTitleRectTransform.anchoredPosition;
        gameOverPosTarget = new Vector3(0f, -(Screen.height * 1.1f), 0f);
        gameOverPosCur = gameOverPosTarget;
        gameOverTitleRectTransform.anchoredPosition = gameOverPosCur;

        // overlay
        overlayColCur = overlayColHide;

        // transition out!
        SetupManager.instance.InitStartTransition();
    }

    void Update ()
    {
        bool tryToShow = (!SetupManager.instance.paused);

        // overlay?
        if (overlayMeshRenderer != null)
        {
            Color overlayColTarget = overlayColHide;
            float overlayColLerpie = 1.25f;
            if (!leaving && !SetupManager.instance.inTransition)
            {
                overlayColTarget = overlayColShow;
            }
            else
            {
                overlayColLerpie = 10f;
            }

            if (tryToShow)
            {
                overlayColCur = Color.Lerp(overlayColCur, overlayColTarget, overlayColLerpie * Time.deltaTime);
                overlayMeshRenderer.material.SetColor("_BaseColor", overlayColCur);
            }
        }

        gameOverTitleText.enabled = tryToShow;
        gameOverTitleBackText.enabled = tryToShow;

        int showProgressInterval = (showProgressDur / 2);
        if (showProgressCounter < showProgressDur)
        {
            if (tryToShow)
            {
                showProgressCounter++;
            }

            if (showProgressCounter >= (showProgressDur - (showProgressInterval * 1)))
            {
                gameOverPosTarget = gameOverPosOriginal;
            }
            
            levelReachedText.enabled = false;
            levelHighestReachedText.enabled = false;

            playTimeText.enabled = false;
        }
        else
        {
            // new highest level?
            SetupManager.instance.CheckIfReachedNewHighest();

            // highest level data
            int highestLoopIndex = 0;
            int highestFloorIndex = 0;
            int highestLevelIndex = 0;
            switch (SetupManager.instance.curRunType)
            {
                case SetupManager.RunType.Normal:
                    highestLoopIndex = SetupManager.instance.curProgressData.persistentData.normalHighestLoopIndex;
                    highestFloorIndex = SetupManager.instance.curProgressData.persistentData.normalHighestFloorIndex;
                    highestLevelIndex = SetupManager.instance.curProgressData.persistentData.normalHighestLevelIndex;
                    break;
                case SetupManager.RunType.Endless:
                    highestLoopIndex = SetupManager.instance.curProgressData.persistentData.endlessHighestLoopIndex;
                    highestFloorIndex = SetupManager.instance.curProgressData.persistentData.endlessHighestFloorIndex;
                    highestLevelIndex = SetupManager.instance.curProgressData.persistentData.endlessHighestLevelIndex;
                    break;
            }

            // floor strings
            int floorInt = SetupManager.instance.runDataRead.curFloorIndex;
            int floorHighestInt = 0;
            switch ( SetupManager.instance.curRunType )
            {
                case SetupManager.RunType.Normal: floorHighestInt = SetupManager.instance.curProgressData.persistentData.normalHighestFloorIndex; break;
                case SetupManager.RunType.Endless: floorHighestInt = SetupManager.instance.curProgressData.persistentData.endlessHighestFloorIndex; break;
            }
            string floorString = "";
            string floorHighestString = "";
            switch ( floorInt )
            {
                case 1: floorString = "sewer"; break;
                case 2: floorString = "dungeon"; break;
                case 3: floorString = "hell"; break;
            }
            switch (floorHighestInt)
            {
                case 1: floorHighestString = "sewer"; break;
                case 2: floorHighestString = "dungeon"; break;
                case 3: floorHighestString = "hell"; break;
            }

            // current level reached
            string loopAdd = (SetupManager.instance.curRunType == SetupManager.RunType.Endless) ? "loop " + SetupManager.instance.runDataRead.curLoopIndex.ToString() + " " : "";
            levelReachedText.enabled = tryToShow;
            string levelReachedString = "level reached: " + SetupManager.instance.lineHeight + SetupManager.instance.alignRight + loopAdd + floorString + " " + (SetupManager.instance.runDataRead.curLevelIndex + 1).ToString() + SetupManager.instance.lineHeightCancel;
            levelReachedText.text = levelReachedString;

            // highest level reached
            string loopHighestAdd = (SetupManager.instance.curRunType == SetupManager.RunType.Endless) ? "loop " + highestLoopIndex.ToString() + " " : "";
            levelHighestReachedText.enabled = tryToShow;
            string levelHighestReachedString = "best ever: " + SetupManager.instance.lineHeight + SetupManager.instance.alignRight + loopHighestAdd + floorHighestString + " " + (highestLevelIndex + 1).ToString() + SetupManager.instance.lineHeightCancel;
            levelHighestReachedText.text = levelHighestReachedString;

            // playtime
            playTimeText.enabled = tryToShow;
            string playTimeString = "playtime: " + SetupManager.instance.GetRunPlayTime(SetupManager.instance.runDataRead);
            playTimeText.text = playTimeString;

            // can proceed to title?
            if (tryToShow && proceedShowWaitCounter < proceedShowWaitDur)
            {
                proceedShowWaitCounter++;
            }
        }

        // game over title text animation
        if (tryToShow && gameOverTitleRectTransform != null)
        {
            gameOverPosCur = Vector3.Lerp(gameOverPosCur, gameOverPosTarget, 5f * Time.deltaTime);
            gameOverTitleRectTransform.anchoredPosition = gameOverPosCur;
        }
        if (gameOverTitleBackRectTransform != null)
        {
            Vector3 gameOverBackP = gameOverPosCur;
            float gameOverBackOff = 8f;
            gameOverBackP.x += gameOverBackOff;
            gameOverBackP.y -= gameOverBackOff;
            gameOverTitleBackRectTransform.anchoredPosition = gameOverBackP;
        }

        // proceed
        bool showProceed = (tryToShow && !leaving && proceedShowWaitCounter >= proceedShowWaitDur);
        if (gameOverProceedImage != null)
        {
            gameOverProceedImage.enabled = showProceed;
            gameOverProceedText.enabled = showProceed;
            if (showProceed)
            {
                gameOverProceedText.text = SetupManager.instance.UIInteractionButtonCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - back to title";

                // actually proceed?
                if ( SetupManager.instance.canInteract && InputManager.instance.interactPressed )
                {
                    Proceed();
                    leaving = true;

                    // audio
                    SetupManager.instance.PlayUISelectSound();
                }
            }
        }

        // hide all UI?
        if ( leaving )
        {
            gameOverTitleText.enabled = false;
            gameOverTitleBackText.enabled = false;

            levelReachedText.enabled = false;
            levelHighestReachedText.enabled = false;

            playTimeText.enabled = false;

            gameOverProceedImage.enabled = false;
            gameOverProceedText.enabled = false;
        }
    }

    public void Proceed ()
    {
        SetupManager.instance.SetTransition(SetupManager.TransitionMode.In);
        Invoke("LoadTitle",SetupManager.instance.sceneLoadWait);
    }

    public void LoadTitle ()
    {
        // create new progress data
        //SetupManager.instance.CreateNewProgressData();
        switch ( SetupManager.instance.curRunType )
        {
            case SetupManager.RunType.Normal: SetupManager.instance.curProgressData.normalRunData = SaveManager.instance.CreateNewRunData(); SetupManager.instance.curProgressData.persistentData.inNormalRun = false; break;
            case SetupManager.RunType.Endless: SetupManager.instance.curProgressData.endlessRunData = SaveManager.instance.CreateNewRunData(); SetupManager.instance.curProgressData.persistentData.inEndlessRun = false; break;
        }
        
        // reset input
        InputManager.instance.DisableAllInput();

        // clear all particles?
        if (SetupManager.instance != null)
        {
            SetupManager.instance.ClearAllParticles();
        }

        // clear decals?
        if (DecalManager.instance != null)
        {
            DecalManager.instance.ClearAllDecals();
        }

        // load a specific scene
        SetupManager.GameState gameStateTo = SetupManager.GameState.Title;
        int sceneIndexTo = 1;
        SetupManager.instance.SetGameState(gameStateTo);

        // go
        SceneManager.LoadScene(sceneIndexTo + 1, LoadSceneMode.Single);
    }
}
