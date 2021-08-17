using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class OutroManager : MonoBehaviour
{
    public static OutroManager instance;

    // camera
    [Header("camera")]
    public Transform camTransform;

    // shots
    [Header("shot datas")]
    public ShotData[] shotDatas;

    // lava
    [Header("lava")]
    public MeshRenderer lavaMeshRenderer;

    // UI
    [Header("UI")]
    public TextMeshProUGUI finishedTitleText;
    public RectTransform finishedTitleRectTransform;
    public TextMeshProUGUI finishedTitleBackText;
    public RectTransform finishedTitleBackRectTransform;

    public TextMeshProUGUI proceedText;
    public Image proceedImage;

    public TextMeshProUGUI levelReachedText;
    public TextMeshProUGUI levelHighestReachedText;

    public TextMeshProUGUI playTimeText;

    // end
    int hellBreakWait, hellBreakCounter;

    // animation
    [System.Serializable]
    public struct ShotData
    {
        public Transform camTransform;
        public int shotWait;
    }

    // state
    [HideInInspector] public int shotIndex, shotIndexMax, shotCounter, shotWait;
    [HideInInspector] public bool done;
    [HideInInspector] public bool leaving;

    void Awake ()
    {
        instance = this;
    }

    void Start()
    {
        // lava
        SetupManager.instance.lavaFactor = .075f;

        // transition out!
        SetupManager.instance.InitStartTransition();

        // shots
        done = false;
        leaving = false;
        shotIndexMax = shotDatas.Length;
        shotIndex = 0;
        shotCounter = 0;

        // end
        hellBreakWait = 120;
        hellBreakCounter = 0;
    }

    void Update ()
    {
        bool showUI = (done && !leaving);

        // highest level data
        int highestLoopIndex = 0;
        int highestFloorIndex = 0;
        int highestLevelIndex = 0;
        switch ( SetupManager.instance.curRunType )
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

        // UI
        proceedImage.enabled = showUI;
        proceedText.enabled = showUI;

        string interactionString = "back to title";
        if ( SetupManager.instance.curRunType == SetupManager.RunType.Endless )
        {
            interactionString = "enter next loop";
        }
        proceedText.text = SetupManager.instance.UIInteractionButtonCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - " + interactionString;

        // finished title
        finishedTitleText.enabled = showUI;
        finishedTitleBackText.enabled = showUI;

        // floor strings
        int floorInt = SetupManager.instance.runDataRead.curFloorIndex;
        int floorHighestInt = 0;
        switch (SetupManager.instance.curRunType)
        {
            case SetupManager.RunType.Normal: floorHighestInt = SetupManager.instance.curProgressData.persistentData.normalHighestFloorIndex; break;
            case SetupManager.RunType.Endless: floorHighestInt = SetupManager.instance.curProgressData.persistentData.endlessHighestFloorIndex; break;
        }
        string floorString = "";
        string floorHighestString = "";
        switch (floorInt)
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
        levelReachedText.enabled = showUI;
        string levelReachedString = "level reached: " + SetupManager.instance.lineHeight + SetupManager.instance.alignRight + loopAdd + floorString + " " + (SetupManager.instance.runDataRead.curLevelIndex + 1).ToString() + SetupManager.instance.lineHeightCancel;
        levelReachedText.text = levelReachedString;

        // highest level reached
        string loopHighestAdd = (SetupManager.instance.curRunType == SetupManager.RunType.Endless) ? "loop " + highestLoopIndex.ToString() + " " : "";
        levelHighestReachedText.enabled = showUI;
        string levelHighestReachedString = "best ever: " + SetupManager.instance.lineHeight + SetupManager.instance.alignRight + loopHighestAdd + floorHighestString + " " + (highestLevelIndex + 1).ToString() + SetupManager.instance.lineHeightCancel;
        levelHighestReachedText.text = levelHighestReachedString;

        // playtime
        playTimeText.enabled = showUI;
        string playTimeString = "playtime: " + SetupManager.instance.GetRunPlayTime(SetupManager.instance.runDataRead);
        playTimeText.text = playTimeString;

        // shots
        if (!done)
        {
            HandleShots();
        }
        else
        {
            // new highest level?
            SetupManager.instance.CheckIfReachedNewHighest();

            // proceed?
            if ( InputManager.instance.interactPressed )
            {
                ProceedToTitle();

                SetupManager.instance.PlayUISelectSound();
            }

            // fade out
            if ( hellBreakCounter < hellBreakWait )
            {
                hellBreakCounter++;
            }
            else
            {
                SetupManager.instance.lavaFactor = Mathf.Lerp(SetupManager.instance.lavaFactor,0f,.25f * Time.deltaTime);
            }
        }
    }

    public void HandleShots ()
    {
        ShotData curShotData = shotDatas[shotIndex];
        shotWait = curShotData.shotWait;

        camTransform.position = curShotData.camTransform.position;
        camTransform.rotation = curShotData.camTransform.rotation;

        if (shotIndex < (shotIndexMax - 1))
        {
            if (shotCounter < shotWait)
            {
                shotCounter++;
            }
            else
            {
                shotIndex++;
                shotCounter = 0;
            }
        }
        else
        {
            SetupManager.instance.curProgressData.persistentData.sawOutro = true;
            SetupManager.instance.curProgressData.persistentData.unlockedEndless = true;
            done = true;
        }
    }

    public void ProceedToTitle ()
    {
        leaving = true;
        SetupManager.instance.SetTransition(SetupManager.TransitionMode.In);
        Invoke("LoadTitle", SetupManager.instance.sceneLoadWait);
    }

    public void LoadTitle()
    {
        SetupManager.instance.lavaFactor = 1f;
        SetupManager.instance.defeatedFinalBoss = false;

        // create new progress data, or loop
        SetupManager.GameState gameStateSetTo = SetupManager.GameState.Title;
        int sceneLoadIndex = 1;
        switch ( SetupManager.instance.curRunType )
        {
            case SetupManager.RunType.Normal:

                SetupManager.instance.curProgressData.normalRunData = SaveManager.instance.CreateNewRunData();
                SetupManager.instance.curProgressData.persistentData.inNormalRun = false;

                gameStateSetTo = SetupManager.GameState.Title;
                sceneLoadIndex = 1;

            break;

            case SetupManager.RunType.Endless:

                SetupManager.instance.curProgressData.endlessRunData.curLoopIndex++;
                SetupManager.instance.curProgressData.endlessRunData.curLevelIndex = 0;
                SetupManager.instance.curProgressData.endlessRunData.curFloorIndex = 1;
                SetupManager.instance.curProgressData.endlessRunData.encountersHad = 0;

                AchievementHelper.UnlockAchievement("ACHIEVEMENT_LOOP");

                gameStateSetTo = SetupManager.GameState.LevelSelect;
                sceneLoadIndex = 7;

            break;
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
        SetupManager.instance.SetGameState(gameStateSetTo);

        // save
        SaveManager.instance.WriteToFile(SetupManager.instance.curProgressData);

        // go
        SceneManager.LoadScene(sceneLoadIndex + 1,LoadSceneMode.Single);
    }
}
