using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class FountainManager : MonoBehaviour
{
    // instance
    public static FountainManager instance;

    // fountain
    [Header("fountain")]
    public FountainScript fountainScript;

    // state
    bool generated;
    int blessingPickCount;
    [HideInInspector] public bool leaving;

    // picking a blessing
    [Header("blessings")]
    public List<BlessingDatabase.Blessing> blessingOptions;
    [HideInInspector] public int canPickBlessingDur, canPickBlessingCounter;
    [HideInInspector] public int canSwitchBlessingPickDur, canSwitchBlessingPickCounter;
    [HideInInspector] public bool inBlessingPick;
    [HideInInspector] public bool pickedBlessing;
    [HideInInspector] public List<Image> blessingIconImages;
    [HideInInspector] public List<Image> blessingIconBackImages;
    [HideInInspector] public List<RectTransform> blessingIconRectTransforms;
    [HideInInspector] public List<GameObject> blessingIconObjects;
    [HideInInspector] public List<Text> blessingIconNames;
    [HideInInspector] public int blessingPickIndex;
    [HideInInspector] public BlessingDatabase.Blessing lastBlessingPicked;

    // colors
    [Header("colors")]
    public Color iconNameColHide;
    public Color iconNameColShow;

    void Awake ()
    {
        instance = this;
    }

    void Start()
    {
        // state
        generated = false;
        leaving = false;

        // for coin & tear hacks
        SetupManager.instance.curLevelMaxCoins = 999;
        SetupManager.instance.curLevelMaxTears = 999;

        // blessings
        //PrepareBlessingOptions();
    }

    void PrepareBlessingOptions()
    {
        pickedBlessing = false;

        // init list
        blessingOptions = new List<BlessingDatabase.Blessing>();

        // create temporary lists
        List<BlessingDatabase.Blessing> allBlessings = new List<BlessingDatabase.Blessing>();
        List<BlessingDatabase.Blessing> blessingsLeft = new List<BlessingDatabase.Blessing>();

        // store all possible blessings
        var enumerator = BlessingDatabase.instance.blessingDatas.GetEnumerator();
        while (enumerator.MoveNext())
        {
            allBlessings.Add(enumerator.Current.Key);
            blessingsLeft.Add(enumerator.Current.Key);
        }

        // leave out the blessings that were already claimed
        for (int i = blessingsLeft.Count - 1; i >= 0; i--)
        {
            if (SetupManager.instance.runDataRead.blessingsClaimed.Contains((int)blessingsLeft[i]))
            {
                blessingsLeft.RemoveAt(i);
            }
        }

        blessingPickCount = 2;
        int blessingTotalLeft = (BlessingDatabase.instance.blessingCountMax - SetupManager.instance.runDataRead.blessingsClaimed.Count);
        if (blessingTotalLeft < 2 )
        {
            blessingPickCount = 1;
            if (blessingTotalLeft < 1)
            {
                blessingPickCount = 0;
            }
        }
        if (blessingPickCount > 0)
        {
            while (blessingsLeft.Count >= 1 && blessingOptions.Count < blessingPickCount)
            {
                //int rIndex = Mathf.RoundToInt(TommieRandom.instance.GenerationRandomRange(0f, blessingsLeft.Count - 1,"fountain manager blessing pick"));
                WeightedRandomBag<int> blessingIndexes = new WeightedRandomBag<int>();
                for (int i = 0; i < blessingsLeft.Count; i++)
                {
                    blessingIndexes.AddEntry(i, 10f);
                }
                int rIndex = blessingIndexes.ChooseGeneration("blessing prepare");

                blessingOptions.Add(blessingsLeft[rIndex]);
                blessingsLeft.Remove(blessingsLeft[rIndex]);
            }
        }

        // counters
        canPickBlessingDur = 10;
        canSwitchBlessingPickDur = 8;
    }

    public void CreateBlessingOptions()
    {
        blessingIconRectTransforms = new List<RectTransform>();
        blessingIconObjects = new List<GameObject>();
        blessingIconImages = new List<Image>();
        blessingIconBackImages = new List<Image>();
        blessingIconNames = new List<Text>();
        for (int i = 0; i < blessingOptions.Count; i++)
        {
            GameObject newBlessingIconO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.blessingIconPrefab[0], Vector3.zero, Quaternion.identity, 1f);
            Transform newBlessingIconTr = newBlessingIconO.transform;
            newBlessingIconTr.parent = GameManager.instance.mainCanvasRectTransform;

            BasicFunctions.ResetTransform(newBlessingIconTr);

            RectTransform rTr = newBlessingIconO.GetComponent<RectTransform>();
            rTr.localScale = Vector3.one;

            blessingIconRectTransforms.Add(newBlessingIconO.GetComponent<RectTransform>());
            blessingIconObjects.Add(newBlessingIconO);
            blessingIconImages.Add(newBlessingIconTr.Find("image0").GetComponent<Image>());
            blessingIconBackImages.Add(newBlessingIconTr.Find("backImage0").GetComponent<Image>());
            blessingIconNames.Add(newBlessingIconTr.Find("nameText0").GetComponent<Text>());
        }

        // game state
        SetupManager.instance.SetGameState(SetupManager.GameState.BlessingPick);

        // done
        canPickBlessingCounter = 0;
        canSwitchBlessingPickCounter = 0;
        blessingPickIndex = 0;
        inBlessingPick = true;
    }

    void ClearAllBlessings()
    {
        for (int i = blessingIconObjects.Count - 1; i >= 0; i--)
        {
            Destroy(blessingIconObjects[i]);
        }
        blessingIconObjects.Clear();
        blessingIconRectTransforms.Clear();
        blessingIconNames.Clear();
        blessingIconImages.Clear();
        blessingIconBackImages.Clear();
    }

    void HandleBlessingPick()
    {
        if (inBlessingPick)
        {
            // current selection
            BlessingDatabase.BlessingData blessingDataSelected = BlessingDatabase.instance.GetBlessingData(blessingOptions[blessingPickIndex]);
            GameManager.instance.blessingDescriptionText.text = blessingDataSelected.description;

            // handle behaviour for possible options
            float hOff = 760f;
            float hAdd = (hOff / (float)(blessingOptions.Count));
            float hStart = (-(float)(blessingOptions.Count / 2) * (hAdd * .5f));
            for (int i = 0; i < blessingOptions.Count; i++)
            {
                float iFloat = (float)(i);

                BlessingDatabase.BlessingData curBlessingData = BlessingDatabase.instance.GetBlessingData(blessingOptions[i]);
                blessingIconNames[i].text = curBlessingData.name;

                // position
                Vector3 p = Vector3.zero;
                p.x = hStart + (hAdd * iFloat);
                if ( blessingOptions.Count <= 1 )
                {
                    p.x = 0f;
                }
                p.y = -320f;//20f;

                blessingIconRectTransforms[i].anchoredPosition = p;

                // name color
                bool isSelected = (i == blessingPickIndex);
                blessingIconNames[i].color = (isSelected) ? iconNameColShow : iconNameColHide;
            }

            // move selection
            if (canSwitchBlessingPickCounter < canSwitchBlessingPickDur)
            {
                canSwitchBlessingPickCounter++;
            }
            else
            {
                float selectThreshold = .25f;
                float hInput = InputManager.instance.moveDirection.x;
                int minIndex = 0;
                int maxIndex = (blessingOptions.Count - 1);
                if (blessingPickIndex < maxIndex && (hInput > selectThreshold))
                {
                    blessingPickIndex++;
                    canSwitchBlessingPickCounter = 0;

                    // audio
                    SetupManager.instance.PlayUINavigateSound();
                }
                if (blessingPickIndex > minIndex && (hInput < -selectThreshold))
                {
                    blessingPickIndex--;
                    canSwitchBlessingPickCounter = 0;

                    // audio
                    SetupManager.instance.PlayUINavigateSound();
                }
            }

            // blessing arrow
            if (GameManager.instance != null && GameManager.instance.blessingArrowRectTransform != null)
            {
                Vector3 arrowP = new Vector3(0f, blessingIconRectTransforms[blessingPickIndex].anchoredPosition.y, 0f);
                arrowP.x = blessingIconRectTransforms[blessingPickIndex].anchoredPosition.x;
                arrowP.y -= 130f;
                GameManager.instance.blessingArrowRectTransform.anchoredPosition = arrowP;
            }

            // pick?
            if (canPickBlessingCounter < canPickBlessingDur)
            {
                canPickBlessingCounter++;
            }
            else
            {
                if (/*SetupManager.instance.canInteract &&*/ InputManager.instance.interactPressed)
                {
                    // store
                    lastBlessingPicked = blessingOptions[blessingPickIndex];

                    switch (SetupManager.instance.curRunType)
                    {
                        case SetupManager.RunType.Normal: SetupManager.instance.curProgressData.normalRunData.blessingsClaimed.Add((int)lastBlessingPicked); break;
                        case SetupManager.RunType.Endless: SetupManager.instance.curProgressData.endlessRunData.blessingsClaimed.Add((int)lastBlessingPicked); break;
                    }

                    // specific behaviour?
                    switch (lastBlessingPicked)
                    {
                        case BlessingDatabase.Blessing.Tank:

                            SetupManager.instance.AddPlayerMaxHealth(BlessingDatabase.instance.tankHealthAdd);

                            break;

                        case BlessingDatabase.Blessing.GlassCannon:

                            SetupManager.instance.AddPlayerMaxHealth(-BlessingDatabase.instance.glassCannonHealthSubtract);

                            break;
                    }

                    // done
                    inBlessingPick = false;
                    SetupManager.instance.SetGameState(SetupManager.GameState.Level);
                    ClearAllBlessings();

                    // run data
                    SetupManager.instance.UpdateRunDataRead();

                    // break fountain?
                    if ( (BlessingDatabase.instance.blessingCountMax - SetupManager.instance.runDataRead.blessingsClaimed.Count) <= 0 && fountainScript != null )
                    {
                        fountainScript.Break();
                    }

                    // blessing achievements?
                    int blessingCount = SetupManager.instance.runDataRead.blessingsClaimed.Count;
                    if ( blessingCount >= 5 )
                    {
                        AchievementHelper.UnlockAchievement("ACHIEVEMENT_BLESSED");
                    }
                    if (blessingCount >= 10)
                    {
                        AchievementHelper.UnlockAchievement("ACHIEVEMENT_DISCIPLE");
                    }
                    if ( blessingCount >= 20 )
                    {
                        AchievementHelper.UnlockAchievement("ACHIEVEMENT_MOONGOD");
                    }

                    // audio
                    AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.blessingPickClips),.9f,1f,.3f,.325f);
                    SetupManager.instance.PlayUISelectSound();
                }
            }
        }
    }

    void Update ()
    {
        if (!generated)
        {
            if (LevelGeneratorManager.instance != null && LevelGeneratorManager.instance.activeLevelGenerator != null && LevelGeneratorManager.instance.activeLevelGenerator.generatedLevel)
            {
                PrepareBlessingOptions();
                generated = true;
            }
        }
        else
        {
            // freeze because leaving?
            if (leaving)
            {
                SetupManager.instance.SetFreeze(1);
            }

            // in blessing pick?
            if (inBlessingPick)
            {
                SetupManager.instance.SetFreeze(1);
            }

            // picking a blessing?
            if (blessingPickCount > 0)
            {
                HandleBlessingPick();
            }
        }
    }

    public void InitProceedToNextLevel ()
    {
        SetupManager.instance.SetTransition(SetupManager.TransitionMode.In);
        Invoke("ProceedToNextLevel", SetupManager.instance.sceneLoadWait);
        leaving = true;
    }

    void ProceedToNextLevel ()
    {
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

        // save
        switch ( SetupManager.instance.curRunType )
        {
            case SetupManager.RunType.Normal:
                SetupManager.instance.curProgressData.normalRunData.lastLevelCoinsCollected = SetupManager.instance.curProgressData.normalRunData.curRunCoinsCollected;
                SetupManager.instance.curProgressData.normalRunData.lastLevelTearsCollected = SetupManager.instance.curProgressData.normalRunData.curRunTearsCollected;
                break;

            case SetupManager.RunType.Endless:
                SetupManager.instance.curProgressData.endlessRunData.lastLevelCoinsCollected = SetupManager.instance.curProgressData.endlessRunData.curRunCoinsCollected;
                SetupManager.instance.curProgressData.endlessRunData.lastLevelTearsCollected = SetupManager.instance.curProgressData.endlessRunData.curRunTearsCollected;
                break;
        }

        // reset input
        InputManager.instance.DisableAllInput();

        // transition
        SetupManager.instance.transitionFactor = 0f;

        // go
        SetupManager.instance.SetGameState(SetupManager.GameState.LevelSelect);
        SceneManager.LoadScene(7 + 1, LoadSceneMode.Single);
    }
}
