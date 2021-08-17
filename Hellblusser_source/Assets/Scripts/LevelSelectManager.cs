using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    // instance
    public static LevelSelectManager instance;

    // UI
    [Header("UI")]
    public Canvas mainCanvas;
    public RectTransform mainCanvasRectTransform;
    public RectTransform locationArrowRectTransform;
    public Image locationArrowBackImage;
    public Image locationArrowFrontImage;

    public Image playerHealthImage;
    public Text playerHealthText;

    public Image coinsCollectedImage;
    public TextMeshProUGUI coinsCollectedText;

    public Image tearsCollectedImage;
    public TextMeshProUGUI tearsCollectedText;

    int locationArrowFlickerIndex, locationArrowFlickerRate, locationArrowFlickerCounter;

    public Image levelSelectInfoBackImage;
    public TextMeshProUGUI levelSelectInfoText;

    public Image proceedBackImage;
    public TextMeshProUGUI proceedText;

    // overlay
    [Header("overlay")]
    public MeshRenderer overlayMeshRenderer;
    public Color overlayColShow, overlayColHide;
    Color overlayColCur;

    // locations
    public List<List<LocationSlot>> locationSlots;
    public List<LocationSlot.PositionType> positionTypes;

    // state
    [HideInInspector] public bool leaving;
    public int locationSelectIndex;
    [HideInInspector] public int locationCanSwitchDur, locationCanSwitchCounter;
    [HideInInspector] public SetupManager.LocationType lastLocationTypeSelected;

    void Awake ()
    {
        instance = this;
    }

    void Start ()
    {
        // get data from run
        switch (SetupManager.instance.curRunType)
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

        // state
        leaving = false;

        // overlay
        overlayColCur = overlayColHide;

        // transition out!
        SetupManager.instance.InitStartTransition();

        // location arrow flicker behaviour
        locationArrowFlickerIndex = 0;
        locationArrowFlickerRate = 16;
        locationArrowFlickerCounter = 0;
        locationSelectIndex = 0;

        // create level select objects
        CreateLevelSelectObjects();
    }

    void CreateLevelSelectObjects ()
    {
        // create objects
        locationSlots = new List<List<LocationSlot>>();
        positionTypes = new List<LocationSlot.PositionType>();
        for (int i = 0; i < SetupManager.instance.runDataRead.curFloorData.locationCount; i++)
        {
            locationSlots.Add(new List<LocationSlot>());

            for (int ii = 0; ii < SetupManager.instance.runDataRead.curFloorData.locationTypes[i].types.Count; ii++)
            {
                GameObject newLocationSlotO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.locationSlotPrefab[0], Vector3.zero, Quaternion.identity, 1f);
                Transform newLocationSlotTr = newLocationSlotO.transform;
                newLocationSlotTr.parent = mainCanvasRectTransform;

                BasicFunctions.ResetTransform(newLocationSlotTr);

                RectTransform rTr = newLocationSlotO.GetComponent<RectTransform>();
                rTr.localScale = Vector3.one;

                LocationSlot newLocationSlotScript = newLocationSlotO.GetComponent<LocationSlot>();
                newLocationSlotScript.locationIndex = i;
                newLocationSlotScript.locationSubIndex = ii;
                newLocationSlotScript.myLocationType = (SetupManager.LocationType)(SetupManager.instance.runDataRead.curFloorData.locationTypes[i].types[ii]);

                int locationTypeCount = SetupManager.instance.runDataRead.curFloorData.locationTypes[i].types.Count;
                if ( locationTypeCount <= 1 )
                {
                    newLocationSlotScript.SetPositionType(LocationSlot.PositionType.Center);
                }
                else
                {
                    newLocationSlotScript.SetPositionType((ii == 0) ? LocationSlot.PositionType.Top : LocationSlot.PositionType.Bottom);
                }

                //if ( i == 0 )
                //{
                //    positionTypes.Add(newLocationSlotScript.myPositionType);
                //}

                locationSlots[i].Add(newLocationSlotScript);
            }
        }
    }

    void Update ()
    {
        // player health text?
        bool showPlayerHealth = (!SetupManager.instance.paused && !SetupManager.instance.inFreeze && !SetupManager.instance.inTransition && (SetupManager.instance.runDataRead.curLevelIndex > 0 || SetupManager.instance.runDataRead.curFloorIndex > 1));
        if ( playerHealthImage != null && playerHealthText != null )
        {
            playerHealthImage.enabled = showPlayerHealth;
            playerHealthText.enabled = showPlayerHealth;
            if (showPlayerHealth)
            {
                playerHealthText.text = SetupManager.instance.runDataRead.playerHealthCur.ToString();
            }
        }

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

            overlayColCur = Color.Lerp(overlayColCur, overlayColTarget, overlayColLerpie * Time.deltaTime);
            overlayMeshRenderer.material.SetColor("_BaseColor", overlayColCur);
        }

        // handle behaviour for possible options
        if (!leaving)
        {
            int locationCount = SetupManager.instance.runDataRead.curFloorData.locationCount;
            //float hOff = 550f;//840f;
            //float hAdd = (hOff / (float)(locationCount));
            //float hStart = (-(float)(locationCount / 2) * (hOff * 1f));

            float hAddSingle = 130f;
            float hAddDual = 75f;
            float hAddTotal = 0f;// (hAddSingle * 4) + (hAddDual * 4);
            for ( int i = 0; i < SetupManager.instance.runDataRead.curFloorData.locationCount; i ++ )
            {
                if (i < (locationCount - 1))
                {
                    if (SetupManager.instance.runDataRead.curFloorData.locationTypes[i + 1].types.Count > 1 || SetupManager.instance.runDataRead.curFloorData.locationTypes[i].types.Count > 1)
                    {
                        hAddTotal += hAddDual;
                    }
                    else
                    {
                        hAddTotal += hAddSingle;
                    }
                }
            }

            float hStart = -(hAddTotal / 2); //-420f;
            float hAdded = 0f;
            for (int i = 0; i < locationCount; i++)
            {
                bool showLocationSlot = (!SetupManager.instance.paused);

                float iFloat = (float)(i);

                //bool isSelected = (i == itemBrowseIndex);
                //float hAdd = (SetupManager.instance.curProgressData.curFloorData.locationTypes[i].Count > 1 && (i > 0 && SetupManager.instance.curProgressData.curFloorData.locationTypes[i - 1].Count > 1)) ? 20f : 130f;
                float hAdd = hAddSingle;//130f;
                if ( i < (locationCount - 1) )
                {
                    if ( SetupManager.instance.runDataRead.curFloorData.locationTypes[i + 1].types.Count > 1 || SetupManager.instance.runDataRead.curFloorData.locationTypes[i].types.Count > 1)
                    {
                        hAdd = hAddDual;//75f;
                    }
                }

                // position
                for (int ii = 0; ii < SetupManager.instance.runDataRead.curFloorData.locationTypes[i].types.Count; ii++)
                {
                    Vector3 p = Vector3.zero;

                    p.x = hStart + hAdded;//(hAdd * iFloat);
                    p.y = 340f; //0f;
                    float yOff = 80f;

                    switch (locationSlots[i][ii].myPositionType)
                    {
                        case LocationSlot.PositionType.Top: p.y += yOff; break;
                        case LocationSlot.PositionType.Center: break;
                        case LocationSlot.PositionType.Bottom: p.y -= yOff; break;
                    }

                    locationSlots[i][ii].myRectTransform.anchoredPosition = p;
                    locationSlots[i][ii].SetVisible(showLocationSlot);
                }

                hAdded += hAdd;
            }

            // location arrow
            if ( locationArrowFlickerCounter < locationArrowFlickerRate )
            {
                locationArrowFlickerCounter++;
            }
            else
            {
                locationArrowFlickerCounter = 0;
                locationArrowFlickerIndex = (locationArrowFlickerIndex == 0) ? 1 : 0; 
            }

            if (locationArrowRectTransform != null)
            {
                Vector3 arrowP = locationSlots[0][0].myRectTransform.anchoredPosition;

                //Debug.Log("locationSlots: " + locationSlots.Count.ToString() + " || " + "level index: " + SetupManager.instance.runDataRead.curLevelIndex.ToString() + " || " + " locationSlot count: " + locationSlots[0].Count.ToString() + " || " + Time.time.ToString());

                LocationSlot curSlot = locationSlots[SetupManager.instance.runDataRead.curLevelIndex][0];
                //Debug.Log("curSlot: " + curSlot + " || rectTransform: " + curSlot.myRectTransform + " || " + Time.time.ToString());
                arrowP.x = curSlot.myRectTransform.anchoredPosition.x;
                arrowP.y -= 140f;
                locationArrowRectTransform.anchoredPosition = arrowP;

                bool showLocationArrow = (locationArrowFlickerIndex == 1 && !SetupManager.instance.paused && !SetupManager.instance.inTransition);
                locationArrowFrontImage.enabled = showLocationArrow;
                locationArrowBackImage.enabled = showLocationArrow;
            }

            // move selection
            if (locationCanSwitchCounter < locationCanSwitchDur)
            {
                locationCanSwitchCounter++;
            }
            else
            {
                float selectThreshold = .25f;
                float vInput = InputManager.instance.moveDirection.y;
                int minIndex = 0;
                int maxIndex = (SetupManager.instance.runDataRead.curFloorData.locationTypes[SetupManager.instance.runDataRead.curLevelIndex].types.Count - 1);
                if (locationSelectIndex < maxIndex && (vInput < -selectThreshold))
                {
                    locationSelectIndex++;
                    locationCanSwitchCounter = 0;

                    // audio
                    SetupManager.instance.PlayUINavigateSound();
                }
                if (locationSelectIndex > minIndex && (vInput > selectThreshold))
                {
                    locationSelectIndex--;
                    locationCanSwitchCounter = 0;

                    // audio
                    SetupManager.instance.PlayUINavigateSound();
                }
            }
        }

        // level select info
        if ( levelSelectInfoBackImage != null )
        {
            bool showLevelSelectInfo = (!leaving && !SetupManager.instance.paused && !SetupManager.instance.inTransition );

            string levelSelectInfoString = "";
            if (SetupManager.instance.curRunType == SetupManager.RunType.Endless)
            {
                levelSelectInfoString += "loop " + SetupManager.instance.runDataRead.curLoopIndex.ToString() + " ";
            }
            switch (SetupManager.instance.runDataRead.curFloorIndex)
            {
                case 1: levelSelectInfoString += "sewer"; break;
                case 2: levelSelectInfoString += "dungeon"; break;
                case 3: levelSelectInfoString += "hell"; break;
            }
            levelSelectInfoString += " " + (SetupManager.instance.runDataRead.curLevelIndex + 1).ToString();
            levelSelectInfoString += "\n";
            switch ( (SetupManager.LocationType)(SetupManager.instance.runDataRead.curFloorData.locationTypes[SetupManager.instance.runDataRead.curLevelIndex].types[locationSelectIndex]) )
            {
                case SetupManager.LocationType.Level: levelSelectInfoString += "encounter"; break;
                case SetupManager.LocationType.BossLevel: levelSelectInfoString += "boss"; break;
                case SetupManager.LocationType.Shop: levelSelectInfoString += "shop"; break;
                case SetupManager.LocationType.Fountain: levelSelectInfoString += "fountain"; break;
                case SetupManager.LocationType.Rest: levelSelectInfoString += "rest"; break;
            }
            levelSelectInfoText.text = levelSelectInfoString;

            levelSelectInfoBackImage.enabled = false;
            levelSelectInfoText.enabled = showLevelSelectInfo;
        }

        // coins & tears
        if ( coinsCollectedImage != null )
        {
            bool showCoinsCollected = (!leaving && !SetupManager.instance.paused && !SetupManager.instance.inTransition && (SetupManager.instance.runDataRead.curLevelIndex > 0 || SetupManager.instance.runDataRead.curFloorIndex > 1));
            coinsCollectedImage.enabled = showCoinsCollected;
            coinsCollectedText.enabled = showCoinsCollected;
            coinsCollectedText.text = SetupManager.instance.runDataRead.curRunCoinsCollected.ToString();
        }
        if (tearsCollectedImage != null)
        {
            bool showTearsCollected = (!leaving && !SetupManager.instance.paused && !SetupManager.instance.inTransition && (SetupManager.instance.runDataRead.curLevelIndex > 0 || SetupManager.instance.runDataRead.curFloorIndex > 1));
            tearsCollectedImage.enabled = showTearsCollected;
            tearsCollectedText.enabled = showTearsCollected;
            tearsCollectedText.text = SetupManager.instance.runDataRead.curRunTearsCollected.ToString();
        }

        // proceed
        if (proceedBackImage != null)
        {
            bool showProceed = (!leaving && !SetupManager.instance.paused && !SetupManager.instance.inTransition);

            string proceedString = SetupManager.instance.UIInteractionButtonCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - SELECT";
            proceedText.text = proceedString;

            proceedBackImage.enabled = showProceed;
            proceedText.enabled = showProceed;

            // select a level/proceed?
            if ( showProceed && !leaving )
            {
                if (SetupManager.instance.canInteract && InputManager.instance.interactPressed)
                {
                    lastLocationTypeSelected = (SetupManager.LocationType)(SetupManager.instance.runDataRead.curFloorData.locationTypes[SetupManager.instance.runDataRead.curLevelIndex].types[locationSelectIndex]);
                    switch ( SetupManager.instance.curRunType )
                    {
                        case SetupManager.RunType.Normal:
                            SetupManager.instance.curProgressData.normalRunData.curFloorData.locationVisitedIndex[SetupManager.instance.runDataRead.curLevelIndex] = locationSelectIndex;
                            break;

                        case SetupManager.RunType.Endless:
                            SetupManager.instance.curProgressData.endlessRunData.curFloorData.locationVisitedIndex[SetupManager.instance.runDataRead.curLevelIndex] = locationSelectIndex;
                            break;
                    }
                    locationSelectIndex = 0;

                    Proceed();
                    leaving = true;

                    // audio
                    SetupManager.instance.PlayUILevelSelectSound();
                }
            }
        }
    }

    public void Proceed ()
    {
        SetupManager.instance.SetTransition(SetupManager.TransitionMode.In);
        Invoke("LoadNextLevel",SetupManager.instance.sceneLoadWait);
    }

    void LoadNextLevel ()
    {
        SetupManager.EncounterType encounterTypeTo = SetupManager.EncounterType.Small;
        SetupManager.GameState gameStateSetTo = SetupManager.GameState.Level;
        int sceneIndexLoad = 2;
        switch (lastLocationTypeSelected)
        {
            case SetupManager.LocationType.Level:
                gameStateSetTo = SetupManager.GameState.Level; sceneIndexLoad = 2;
                encounterTypeTo = SetupManager.EncounterType.Small;
            break;

            case SetupManager.LocationType.BossLevel:
                gameStateSetTo = SetupManager.GameState.Level;
                sceneIndexLoad = 2;
                encounterTypeTo = SetupManager.EncounterType.Boss;
            break;

            case SetupManager.LocationType.Shop: gameStateSetTo = SetupManager.GameState.Shop; sceneIndexLoad = 5; break;
            case SetupManager.LocationType.Fountain: gameStateSetTo = SetupManager.GameState.Fountain; sceneIndexLoad = 4; break;
            case SetupManager.LocationType.Rest: gameStateSetTo = SetupManager.GameState.Rest; sceneIndexLoad = 8; break;
        }
        if ( (SetupManager.instance.runDataRead.curLevelIndex + 1) >= (SetupManager.instance.runDataRead.curFloorData.locationCount) )
        {
            encounterTypeTo = SetupManager.EncounterType.Boss;
        }
        else
        {
            int encountersHadCheck = SetupManager.instance.runDataRead.encountersHad;
            if (encountersHadCheck < 2)
            {
                encounterTypeTo = SetupManager.EncounterType.Small;
            }
            else if (encountersHadCheck >= 2 && encountersHadCheck < 4)
            {
                encounterTypeTo = SetupManager.EncounterType.Medium;
            }
            else if ( encountersHadCheck >= 4)
            {
                encounterTypeTo = SetupManager.EncounterType.Big;
            }
        }
        SetupManager.instance.SetEncounterType(encounterTypeTo);
        SetupManager.instance.SetGameState(gameStateSetTo);
        SceneManager.LoadScene(sceneIndexLoad + 1,LoadSceneMode.Single);
    }
}
