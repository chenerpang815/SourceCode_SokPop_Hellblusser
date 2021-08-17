using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelEndManager : MonoBehaviour
{
    public static LevelEndManager instance;

    // UI
    [Header("UI")]
    public TextMeshProUGUI levelCompletedText;
    public RectTransform levelCompletedRectTransform;
    public TextMeshProUGUI levelCompletedBackText;
    public RectTransform levelCompletedBackRectTransform;

    public TextMeshProUGUI levelEndCoinsCollectedText;
    public TextMeshProUGUI levelEndTearsCollectedText;
    public TextMeshProUGUI levelEndFiresClearedText;
    public TextMeshProUGUI levelEndProceedText;
    public Image levelEndProceedImage;

    public Image coinFrontImage;
    public Image coinBackImage;
    public TextMeshProUGUI coinCurText;
    public TextMeshProUGUI coinAddText;

    public Image tearFrontImage;
    public Image tearBackImage;
    public TextMeshProUGUI tearCurText;
    public TextMeshProUGUI tearAddText;

    public Color valueColShow, valueColHide;

    public MeshRenderer overlayMeshRenderer;
    public Color overlayColShow, overlayColHide;
    Color overlayColCur;

    // animation
    Vector3 levelCompletedPosOriginal, levelCompletedPosTarget, levelCompletedPosCur;
    int currencyAddRate, currencyAddCounter;
    int currencyAddStartWaitDur, currencyAddStartWaitCounter;
    int currencyAddedWaitDur, currencyAddedWaitCounter;
    int coinsCurrent, coinsTarget, coinsAdd;
    int tearsCurrent, tearsTarget, tearsAdd;

    // state
    [HideInInspector] public bool goingToNextLevel;
    int showScoresDur, showScoresCounter;
    int endStateIndex;
    int endStateRate, endStateCounter;

    void Awake ()
    {
        instance = this;
    }

    void Start ()
    {
        // state
        showScoresDur = 150;
        showScoresCounter = 0;

        endStateIndex = 0;
        endStateRate = 45;
        endStateCounter = 0;

        // animation
        levelCompletedPosOriginal = levelCompletedRectTransform.anchoredPosition;
        levelCompletedPosTarget = new Vector3(0f,-(Screen.height * 1.1f),0f);
        levelCompletedPosCur = levelCompletedPosTarget;
        levelCompletedRectTransform.anchoredPosition = levelCompletedPosCur;

        // currencies
        currencyAddRate = 4;
        currencyAddCounter = 0;

        currencyAddedWaitDur = 30;
        currencyAddedWaitCounter = 0;

        currencyAddStartWaitDur = 30;
        currencyAddStartWaitCounter = 0;

        bool loadCurrenciesFromProgress = true;
        if (loadCurrenciesFromProgress)
        {
            coinsCurrent = (SetupManager.instance.runDataRead.curRunCoinsCollected - SetupManager.instance.runDataRead.curLevelCoinsCollected);
            coinsTarget = SetupManager.instance.runDataRead.curRunCoinsCollected;

            tearsCurrent = (SetupManager.instance.runDataRead.curRunTearsCollected - SetupManager.instance.runDataRead.curLevelTearsCollected);
            tearsTarget = SetupManager.instance.runDataRead.curRunTearsCollected;
           
            //switch ( SetupManager.instance.curRunType )
            //{
            //    case SetupManager.RunType.Normal:
            //        SetupManager.instance.curProgressData.normalRunData.lastLevelCoinsCollected = coinsTarget;
            //        SetupManager.instance.curProgressData.normalRunData.lastLevelTearsCollected = tearsTarget;
            //        break;

            //    case SetupManager.RunType.Endless:
            //        SetupManager.instance.curProgressData.endlessRunData.lastLevelCoinsCollected = coinsTarget;
            //        SetupManager.instance.curProgressData.endlessRunData.lastLevelTearsCollected = tearsTarget;
            //        break;
            //}
        }
        else
        {
            coinsTarget = Mathf.RoundToInt(TommieRandom.instance.RandomRange(30f, 60f));
            coinsCurrent = coinsTarget - Mathf.RoundToInt(TommieRandom.instance.RandomRange(10f,20f));

            tearsTarget = Mathf.RoundToInt(TommieRandom.instance.RandomRange(30f, 60f));
            tearsCurrent = tearsTarget - Mathf.RoundToInt(TommieRandom.instance.RandomRange(10f, 20f));
        }
        coinsAdd = (coinsTarget - coinsCurrent);
        tearsAdd = (tearsTarget - tearsCurrent);

        // overlay
        overlayColCur = overlayColHide;

        // transition out!
        SetupManager.instance.InitStartTransition();
    }

    void Update ()
    {
        bool tryToShow = (!SetupManager.instance.paused);

        // overlay?
        if ( overlayMeshRenderer != null )
        {
            Color overlayColTarget = overlayColHide;
            float overlayColLerpie = 1.25f;
            if ( !goingToNextLevel && !SetupManager.instance.inTransition )
            {
                overlayColTarget = overlayColShow;
            }
            else
            {
                overlayColLerpie = 10f;
            }

            overlayColCur = Color.Lerp(overlayColCur,overlayColTarget,overlayColLerpie * Time.deltaTime);
            overlayMeshRenderer.material.SetColor("_BaseColor",overlayColCur);
        }

        if ( levelCompletedRectTransform != null )
        {
            if (tryToShow)
            {
                levelCompletedPosCur = Vector3.Lerp(levelCompletedPosCur, levelCompletedPosTarget, 5f * Time.deltaTime);
                levelCompletedRectTransform.anchoredPosition = levelCompletedPosCur;
            }
        }
        if ( levelCompletedBackRectTransform != null )
        {
            Vector3 levelCompletedBackP = levelCompletedPosCur;
            float levelCompletedBackOff = 8f;
            levelCompletedBackP.x += levelCompletedBackOff;
            levelCompletedBackP.y -= levelCompletedBackOff;
            levelCompletedBackRectTransform.anchoredPosition = levelCompletedBackP;
        }

        // floor strings
        int floorInt = SetupManager.instance.runDataRead.curFloorIndex;
        string floorString = "";
        switch (floorInt)
        {
            case 1: floorString = "sewer"; break;
            case 2: floorString = "dungeon"; break;
            case 3: floorString = "hell"; break;
        }

        string levelCompletedString = "completed " + floorString + " " + SetupManager.instance.runDataRead.curLevelIndex.ToString();
        if ( levelCompletedText != null )
        {
            levelCompletedText.text = levelCompletedString;
            levelCompletedBackText.text = levelCompletedString;

            bool showLevelCompletedText = (tryToShow);
            levelCompletedText.enabled = showLevelCompletedText;
            levelCompletedBackText.enabled = showLevelCompletedText;
        }

        int showScoresInterval = (showScoresDur / 2);
        if ( showScoresCounter < showScoresDur )
        {
            if (!SetupManager.instance.paused)
            {
                showScoresCounter++;
            }

            if ( showScoresCounter >= (showScoresDur - (showScoresInterval * 1)) )
            {
                levelCompletedPosTarget = levelCompletedPosOriginal;
            }

            levelCompletedText.enabled = tryToShow;
            levelCompletedBackText.enabled = tryToShow;

            levelEndProceedImage.enabled = false;
            levelEndProceedText.enabled = false;

            coinFrontImage.enabled = false;
            coinBackImage.enabled = false;

            tearFrontImage.enabled = false;
            tearBackImage.enabled = false;

            coinCurText.enabled = false;
            coinAddText.enabled = false;

            tearCurText.enabled = false;
            tearAddText.enabled = false;
        }
        else
        {
            if (endStateIndex < 3)
            {
                switch ( endStateIndex )
                {
                    default:
                        if (endStateCounter < endStateRate)
                        {
                            if (!SetupManager.instance.paused)
                            {
                                endStateCounter++;
                            }
                        }
                        else
                        {
                            endStateIndex++;
                            endStateCounter = 0;
                            currencyAddedWaitCounter = 0;
                        }
                        break;

                    case 1:

                        if (currencyAddStartWaitCounter < currencyAddStartWaitDur)
                        {
                            currencyAddStartWaitCounter++;
                        }
                        else
                        {
                            if (coinsCurrent < coinsTarget)
                            {
                                if (currencyAddCounter < currencyAddRate)
                                {
                                    if (!SetupManager.instance.paused)
                                    {
                                        currencyAddCounter++;
                                    }
                                }
                                else
                                {
                                    currencyAddCounter = 0;
                                    coinsCurrent++;
                                    coinsAdd--;

                                    // coin add audio
                                    AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.coinCollectClips), .5f, .9f, .2f, .225f);
                                }
                            }
                            else
                            {
                                if (currencyAddedWaitCounter < currencyAddedWaitDur)
                                {
                                    if (!SetupManager.instance.paused)
                                    {
                                        currencyAddedWaitCounter++;
                                    }
                                }
                                else
                                {
                                    currencyAddStartWaitCounter = 0;
                                    currencyAddedWaitCounter = 0;
                                    endStateIndex++;
                                }
                            }
                        }

                        break;

                    case 2:

                        if (currencyAddStartWaitCounter < currencyAddStartWaitDur)
                        {
                            if (!SetupManager.instance.paused)
                            {
                                currencyAddStartWaitCounter++;
                            }
                        }
                        else
                        {
                            if (tearsCurrent < tearsTarget)
                            {
                                if (currencyAddCounter < currencyAddRate)
                                {
                                    if (!SetupManager.instance.paused)
                                    {
                                        currencyAddCounter++;
                                    }
                                }
                                else
                                {
                                    currencyAddCounter = 0;
                                    tearsCurrent++;
                                    tearsAdd--;

                                    // tear add audio
                                    AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.tearCollectClips), .5f, .9f, .2f, .225f);
                                }
                            }
                            else
                            {
                                if (currencyAddedWaitCounter < currencyAddedWaitDur)
                                {
                                    if (!SetupManager.instance.paused)
                                    {
                                        currencyAddedWaitCounter++;
                                    }
                                }
                                else
                                {
                                    currencyAddStartWaitCounter = 0;
                                    currencyAddedWaitCounter = 0;
                                    endStateIndex++;
                                }
                            }
                        }

                        break;
                }
            }

            switch ( endStateIndex )
            {
                case 0:
                    coinBackImage.enabled = tryToShow;
                    coinFrontImage.enabled = tryToShow;
                    coinCurText.enabled = tryToShow;
                    coinAddText.enabled = false;

                    tearBackImage.enabled = tryToShow;
                    tearFrontImage.enabled = tryToShow;
                    tearCurText.enabled = tryToShow;
                    tearAddText.enabled = false;
                    break;

                case 1:
                    coinBackImage.enabled = tryToShow;
                    coinFrontImage.enabled = tryToShow;
                    coinCurText.enabled = tryToShow;
                    coinAddText.enabled = tryToShow;

                    tearBackImage.enabled = tryToShow;
                    tearFrontImage.enabled = tryToShow;
                    tearCurText.enabled = tryToShow;
                    tearAddText.enabled = false;

                    coinCurText.color = (coinsAdd > 0) ? valueColHide : valueColShow;
                    coinAddText.color = (coinsAdd > 0) ? valueColShow : valueColHide;

                    tearCurText.color = valueColHide;
                    tearAddText.color = valueColHide;
                    break;

                case 2:
                    coinBackImage.enabled = tryToShow;
                    coinFrontImage.enabled = tryToShow;
                    coinCurText.enabled = tryToShow;
                    coinAddText.enabled = false;

                    tearBackImage.enabled = tryToShow;
                    tearFrontImage.enabled = tryToShow;
                    tearCurText.enabled = tryToShow;
                    tearAddText.enabled = tryToShow;

                    tearCurText.color = (tearsAdd > 0) ? valueColHide : valueColShow;
                    tearAddText.color = (tearsAdd > 0) ? valueColShow : valueColHide;

                    coinCurText.color = valueColHide;
                    coinAddText.color = valueColHide;
                    break;

                case 3:

                    coinBackImage.enabled = tryToShow;
                    coinFrontImage.enabled = tryToShow;

                    tearBackImage.enabled = tryToShow;
                    tearFrontImage.enabled = tryToShow;

                    coinCurText.enabled = tryToShow;
                    tearCurText.enabled = tryToShow;

                    coinCurText.color = valueColHide;
                    tearCurText.color = valueColHide;

                    break;
            }

            // update currency values
            if ( coinCurText != null )
            {
                coinCurText.text = coinsCurrent.ToString();
            }
            if ( coinAddText != null )
            {
                coinAddText.text = "+" + coinsAdd.ToString();
                if ( coinsAdd <= 0)
                {
                    coinAddText.text = "";
                }
            }
            if (tearCurText != null)
            {
                tearCurText.text = tearsCurrent.ToString();
            }
            if (tearAddText != null)
            {
                tearAddText.text = "+" + tearsAdd.ToString();
                if (tearsAdd <= 0)
                {
                    tearAddText.text = "";
                }
            }

            // proceed?
            bool showProceed = (endStateIndex >= 3 && tryToShow);
            if ( levelEndProceedImage != null )
            {
                levelEndProceedImage.enabled = showProceed;
                levelEndProceedText.enabled = showProceed;
                if ( showProceed )
                {
                    levelEndProceedText.text = SetupManager.instance.UIInteractionButtonCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - continue";
                }
            }
            if (showProceed)
            {
                if (!goingToNextLevel)
                {
                    if (SetupManager.instance.canInteract && InputManager.instance.interactPressed)
                    {
                        Proceed();
                        goingToNextLevel = true;

                        // audio
                        SetupManager.instance.PlayUISelectSound();
                    }
                }
                else
                {
                    levelEndProceedImage.enabled = false;
                    levelEndProceedText.enabled = false;

                    levelCompletedText.enabled = false;
                    levelCompletedBackText.enabled = false;

                    coinFrontImage.enabled = false;
                    coinBackImage.enabled = false;

                    tearFrontImage.enabled = false;
                    tearBackImage.enabled = false;

                    coinCurText.enabled = false;
                    coinAddText.enabled = false;

                    tearCurText.enabled = false;
                    tearAddText.enabled = false;
                }
            }
        }
    }

    public void Proceed ()
    {
        SetupManager.instance.SetTransition(SetupManager.TransitionMode.In);
        Invoke("LoadNextLevel",SetupManager.instance.sceneLoadWait);
    }

    public void LoadNextLevel ()
    {
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

        // reset input
        InputManager.instance.DisableAllInput();

        // going to next level?
        SetupManager.instance.CheckIfHasToEnterNextFloor();

        // go
        SetupManager.instance.SetGameState(SetupManager.GameState.LevelSelect);
        SceneManager.LoadScene(7 + 1, LoadSceneMode.Single);
    }
}
