using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    // container
    public GameObject containerObject;
    public GameObject pausedContainerObject;
    public GameObject settingsContainerObject;

    // canvas
    [Header("canvas")]
    public Canvas canvasUse;
    public RectTransform canvasUseRectTransform;

    // UI
    [Header("UI")]
    public Image headerImage;
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI quitText;

    public TextMeshProUGUI tutorialPopupText;
    public Image tutorialPopupBackImage;
    public TextMeshProUGUI tutorialPopupAcceptText;
    public RectTransform tutorialPopupRectTransform;

    public TextMeshProUGUI versionText;
    public RectTransform versionTextRectTransform;

    // colors
    [Header("colors")]
    public Color settingColNormal;
    public Color settingColSelected;
    public Color settingColHide;

    // state
    bool createdSettings;

    public List<Setting> settings;
    public List<GameObject> settingTextObjects;
    public List<RectTransform> settingTextRectTransforms;
    public List<TextMeshProUGUI> settingTexts;

    public enum MenuState { Default, Settings };
    public MenuState curMenuState;
    
    int verIndex, verIndexMin, verIndexMax;
    int horIndex, horIndexMin, horIndexMax;
    int canSwitchIndexWait, canSwitchIndexCounter;

    // setting
    public enum SettingType
    {
        Resolution,
        Fullscreen,
        SFXVolume,
        MusicVolume,
        LookSensitivityX,
        LookSensitivityY,
        InvertYAxis,
        WobbleEffect,
        CameraMotion,
        Resume,
        Settings,
        Quit,
        DeleteSave,
        EndlessFilter,
    };

    [System.Serializable]
    public struct Setting
    {
        public SettingType settingType;
        public int minHorIndex;
        public int maxHorIndex;

        public Setting ( SettingType _settingType, int _minHorIndex, int _maxHorIndex )
        {
            settingType = _settingType;
            minHorIndex = _minHorIndex;
            maxHorIndex = _maxHorIndex;
        }
    }

    void Awake ()
    {
        if ( SetupManager.instance != null )
        {
            SetupManager.instance.menuManager = this;
        }

        // put container in canvas
        if ( canvasUseRectTransform != null && containerObject != null )
        {
            Transform containerTr = containerObject.transform;
            containerTr.parent = canvasUseRectTransform;
            BasicFunctions.ResetTransform(containerTr);
        }

        // hide popup?
        if (tutorialPopupBackImage != null)
        {
            tutorialPopupBackImage.enabled = false;
            tutorialPopupAcceptText.enabled = false;
            tutorialPopupText.enabled = false;
        }

        // clear on start
        ClearSettings();
    }

    void Start ()
    {
        // state
        createdSettings = false;
        verIndex = 0;
        horIndex = 0;

        // counters
        canSwitchIndexWait = 2;
        canSwitchIndexCounter = canSwitchIndexWait;
    }

    void Update ()
    {
        // tutorial popup
        if (tutorialPopupBackImage != null)
        {
            bool showTutorialPopup = (SetupManager.instance.tutorialPopupCounter < SetupManager.instance.tutorialPopupDur && (!SetupManager.instance.paused || SetupManager.instance.aboutToDeleteSave));
            tutorialPopupBackImage.enabled = showTutorialPopup;
            tutorialPopupText.enabled = showTutorialPopup;
            tutorialPopupText.text = SetupManager.instance.tutorialPopupString;
            tutorialPopupRectTransform.anchoredPosition = new Vector3(0f, 80f, 0f);

            tutorialPopupAcceptText.enabled = showTutorialPopup;

            if (!SetupManager.instance.aboutToDeleteSave)
            {
                tutorialPopupAcceptText.text = SetupManager.instance.UIInteractionButtonCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - ok";
            }
            else
            {
                string deleteSaveInputString = "";
                deleteSaveInputString += SetupManager.instance.UIInteractionButtonCol + InputManager.instance.interactInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - yes";
                deleteSaveInputString += "      ";
                deleteSaveInputString += SetupManager.instance.UIInteractionButtonCol + InputManager.instance.cancelInputStringUse + SetupManager.instance.UIInteractionBaseCol + " - no";
                tutorialPopupAcceptText.text = deleteSaveInputString;
            }
        }

        if (createdSettings)
        {
            // quit text
            if ( quitText != null )
            {
                quitText.text = InputManager.instance.cancelInputStringUse;
            }

            // visuals
            SetVisuals(!SetupManager.instance.aboutToDeleteSave);

            // update navigation
            UpdateNavigation();

            // update setting visuals
            UpdateSettingVisuals(false);
        }

        // version text
        if ( versionText != null )
        {
            versionText.text = "version " + Application.version;
            versionText.enabled = (SetupManager.instance != null && SetupManager.instance.paused && curMenuState == MenuState.Default); 
        }
    }

    void UpdateNavigation ()
    {
        if (settings != null && settings.Count > 0)
        {
            float hInput = InputManager.instance.moveDirection.x;
            float vInput = InputManager.instance.moveDirection.y;
            float switchThreshold = .75f;
            if ( SetupManager.instance.tutorialPopupCounter < SetupManager.instance.tutorialPopupDur )
            {
                canSwitchIndexCounter = 0;
            }
            if ( canSwitchIndexCounter < canSwitchIndexWait )
            {
                if ( Mathf.Abs(hInput) <= switchThreshold && Mathf.Abs(vInput) <= switchThreshold )
                {
                    canSwitchIndexCounter = canSwitchIndexWait;
                }
            }
            if ( canSwitchIndexCounter >= canSwitchIndexWait )
            {
                // vertical navigation
                if (verIndex > verIndexMin && vInput >= switchThreshold)
                {
                    verIndex--;
                    SetHorIndex(settings[verIndex].settingType);
                    canSwitchIndexCounter = 0;

                    // audio
                    SetupManager.instance.PlayUINavigateSound();
                }
                if (verIndex < verIndexMax && vInput <= -switchThreshold)
                {
                    verIndex++;
                    SetHorIndex(settings[verIndex].settingType);
                    canSwitchIndexCounter = 0;

                    // audio
                    SetupManager.instance.PlayUINavigateSound();
                }

                // horizontal navigation
                horIndexMin = settings[verIndex].minHorIndex;
                horIndexMax = settings[verIndex].maxHorIndex;
                if (horIndex > horIndexMin && hInput < -switchThreshold)
                {
                    horIndex--;
                    canSwitchIndexCounter = 0;
                    ChangeSetting(settings[verIndex].settingType,-1);

                    // audio
                    SetupManager.instance.PlayUINavigateSound();
                }
                if (horIndex < horIndexMax && hInput > switchThreshold)
                {
                    horIndex++;
                    canSwitchIndexCounter = 0;
                    ChangeSetting(settings[verIndex].settingType,1);

                    // audio
                    SetupManager.instance.PlayUINavigateSound();
                }

                // select?
                if ( InputManager.instance.interactPressed )
                {
                    SelectSetting(settings[verIndex].settingType);

                    // audio
                    SetupManager.instance.PlayUISelectSound();
                }
            }
        }
    }

    void UpdateSettingVisuals ( bool _forceText )
    {
        if (settings != null && settings.Count > 0 && settingTextRectTransforms != null && settingTextRectTransforms.Count > 0 && settingTexts != null && settingTexts.Count > 0)
        {
            int yOffMax = 5;
            int yDiff = Mathf.Abs(yOffMax - verIndex);

            float yAdd = 80f;
            float yStart = -200f;
            for (int i = 0; i < settings.Count; i++)
            {
                bool isSelected = (verIndex == i);
                bool hide = false;

                Setting curSetting = settings[i];
                SettingType curSettingType = curSetting.settingType;
                float iFloat = (float)(i);

                // position
                float y = yStart - (yAdd * iFloat);

                float yOff = 0f;
                if ( verIndex > yOffMax )
                {
                    yOff += ((float)yDiff * yAdd);
                }
                y += yOff;

                if ( verIndex > yOffMax && i < yDiff )
                {
                    hide = true;
                }

                if ( SetupManager.instance.aboutToDeleteSave )
                {
                    hide = true;
                }

                settingTextRectTransforms[i].anchoredPosition = new Vector3(0f,y,0f);

                // text
                if ( isSelected || _forceText )
                {
                    string settingString = "";
                    string stringStart = SetupManager.instance.lineHeight + SetupManager.instance.alignRight;
                    string stringEnd = SetupManager.instance.lineHeightCancel;
                    int horIndexUse = 0;
                    switch (curSettingType)
                    {
                        case SettingType.Resume: settingString += "resume"; break;
                        case SettingType.Settings: settingString += "settings"; break;
                        case SettingType.Quit: settingString += ( SetupManager.instance.curGameState == SetupManager.GameState.Title ) ? "quit game" : "save & quit to title"; break;
                        case SettingType.Resolution: settingString += "resolution"; horIndexUse = SetupManager.instance.curProgressData.settingsData.resolutionIndex; break;
                        case SettingType.Fullscreen: settingString += "fullscreen"; horIndexUse = SetupManager.instance.curProgressData.settingsData.fullscreen; break;
                        case SettingType.LookSensitivityX: settingString += "look sensitivity x"; horIndexUse = SetupManager.instance.curProgressData.settingsData.lookSensitivityX; break;
                        case SettingType.LookSensitivityY: settingString += "look sensitivity y"; horIndexUse = SetupManager.instance.curProgressData.settingsData.lookSensitivityY; break;
                        case SettingType.InvertYAxis: settingString += "invert y-axis"; horIndexUse = SetupManager.instance.curProgressData.settingsData.invertY; break;
                        case SettingType.SFXVolume: settingString += "sound effect volume"; horIndexUse = SetupManager.instance.curProgressData.settingsData.sfxVolIndex; break;
                        case SettingType.MusicVolume: settingString += "music volume"; horIndexUse = SetupManager.instance.curProgressData.settingsData.musicVolIndex; break;
                        case SettingType.WobbleEffect: settingString += "wobble effect"; horIndexUse = SetupManager.instance.curProgressData.settingsData.wobbleEffect; break;
                        case SettingType.CameraMotion: settingString += "camera motion"; horIndexUse = SetupManager.instance.curProgressData.settingsData.cameraMotion; break;
                        case SettingType.EndlessFilter: settingString += "endless filter"; horIndexUse = SetupManager.instance.curProgressData.settingsData.endlessFilter; break;
                        case SettingType.DeleteSave: settingString += "delete save"; break;
                    }

                    if (curMenuState == MenuState.Settings)
                    {
                        string stringAddMin = (horIndexUse > curSetting.minHorIndex) ? "< " : "";
                        string stringAddMax = (horIndexUse < curSetting.maxHorIndex) ? " >" : "";
                        settingString += stringStart + stringAddMin + GetCurrentSettingString(curSettingType) + stringAddMax + stringEnd;
                    }
                    else
                    {
                        settingTexts[i].alignment = TextAlignmentOptions.Center;
                    }
                    settingTexts[i].text = settingString;
                }

                // color
                settingTexts[i].color = (isSelected) ? settingColSelected : settingColNormal;
                //int yDiffAdd = (yDiff > 0) ? yDiff : 0;
                if ( i > yOffMax && !isSelected && i > verIndex )
                {
                    settingTexts[i].color = settingColHide;
                }

                // hide?
                settingTexts[i].enabled = (!hide);
            }
        }
    }

    string GetCurrentSettingString ( SettingType _settingType )
    {
        switch ( _settingType )
        {
            default: return "";
            case SettingType.Resolution: return GetResolutionString(SetupManager.instance.curProgressData.settingsData.resolutionIndex);
            case SettingType.Fullscreen: return GetConditionString(SetupManager.instance.curProgressData.settingsData.fullscreen);
            case SettingType.LookSensitivityX: return GetIndexString(SetupManager.instance.curProgressData.settingsData.lookSensitivityX);
            case SettingType.LookSensitivityY: return GetIndexString(SetupManager.instance.curProgressData.settingsData.lookSensitivityY);
            case SettingType.InvertYAxis: return GetConditionString(SetupManager.instance.curProgressData.settingsData.invertY);
            case SettingType.SFXVolume: return GetIndexString(SetupManager.instance.curProgressData.settingsData.sfxVolIndex);
            case SettingType.MusicVolume: return GetIndexString(SetupManager.instance.curProgressData.settingsData.musicVolIndex);
            case SettingType.WobbleEffect: return GetConditionString(SetupManager.instance.curProgressData.settingsData.wobbleEffect);
            case SettingType.CameraMotion: return GetConditionString(SetupManager.instance.curProgressData.settingsData.cameraMotion);
            case SettingType.EndlessFilter: return GetConditionString(SetupManager.instance.curProgressData.settingsData.endlessFilter);
        }
    }

    void SetHorIndex ( SettingType _nextType )
    {
        switch ( _nextType )
        {
            case SettingType.Resolution: horIndex = SetupManager.instance.curProgressData.settingsData.resolutionIndex; break;
            case SettingType.Fullscreen: horIndex = SetupManager.instance.curProgressData.settingsData.fullscreen; break;
            case SettingType.LookSensitivityX: horIndex = SetupManager.instance.curProgressData.settingsData.lookSensitivityX; break;
            case SettingType.LookSensitivityY: horIndex = SetupManager.instance.curProgressData.settingsData.lookSensitivityY; break;
            case SettingType.InvertYAxis: horIndex = SetupManager.instance.curProgressData.settingsData.invertY; break;
            case SettingType.SFXVolume: horIndex = SetupManager.instance.curProgressData.settingsData.sfxVolIndex; break;
            case SettingType.MusicVolume: horIndex = SetupManager.instance.curProgressData.settingsData.musicVolIndex; break;
            case SettingType.WobbleEffect: horIndex = SetupManager.instance.curProgressData.settingsData.wobbleEffect; break;
            case SettingType.CameraMotion: horIndex = SetupManager.instance.curProgressData.settingsData.cameraMotion; break;
            case SettingType.EndlessFilter: horIndex = SetupManager.instance.curProgressData.settingsData.endlessFilter; break;
        }
    }

    void SelectSetting ( SettingType _settingType )
    {
        switch ( _settingType )
        {
            case SettingType.Resume:
                SetupManager.instance.TogglePaused();
                break;

            case SettingType.Settings:
                ClearSettings();
                SetMenuState(MenuState.Settings);
                CreateSettings();
                break;

            case SettingType.Quit:
                if (SetupManager.instance.curGameState == SetupManager.GameState.Title)
                {
                    SetupManager.instance.InitQuitGame();
                }
                else
                {
                    SetupManager.instance.InitQuitToTitle();
                }
                break;

            case SettingType.DeleteSave:

                SetupManager.instance.aboutToDeleteSave = true;
                SetupManager.instance.SetTutorialPopup(SetupManager.instance.UIPopupBaseCol + "delete savefile?" + "\n" + "can't be undone!", 60);

                break;
        }

        // save
        SaveManager.instance.WriteToFile(SetupManager.instance.curProgressData);
    }

    void ChangeSetting ( SettingType _settingType, int _indexAdd )
    {
        switch ( _settingType )
        {
            case SettingType.Resolution:
                SetupManager.instance.curProgressData.settingsData.resolutionIndex +=_indexAdd;
                Resolution newResolution = Screen.resolutions[SetupManager.instance.curProgressData.settingsData.resolutionIndex];
                Screen.SetResolution(newResolution.width,newResolution.height,Screen.fullScreen);
                break;
            case SettingType.Fullscreen:
                SetupManager.instance.curProgressData.settingsData.fullscreen += _indexAdd;
                Screen.SetResolution(Screen.width,Screen.height,(SetupManager.instance.curProgressData.settingsData.fullscreen) == 1);
                break;
            case SettingType.LookSensitivityX: SetupManager.instance.curProgressData.settingsData.lookSensitivityX += _indexAdd; break;
            case SettingType.LookSensitivityY: SetupManager.instance.curProgressData.settingsData.lookSensitivityY += _indexAdd; break;
            case SettingType.InvertYAxis: SetupManager.instance.curProgressData.settingsData.invertY += _indexAdd; break;
            case SettingType.SFXVolume: SetupManager.instance.curProgressData.settingsData.sfxVolIndex += _indexAdd; break;
            case SettingType.MusicVolume: SetupManager.instance.curProgressData.settingsData.musicVolIndex += _indexAdd; break;
            case SettingType.WobbleEffect: SetupManager.instance.curProgressData.settingsData.wobbleEffect += _indexAdd; break;
            case SettingType.CameraMotion: SetupManager.instance.curProgressData.settingsData.cameraMotion += _indexAdd; break;
            case SettingType.EndlessFilter: SetupManager.instance.curProgressData.settingsData.endlessFilter += _indexAdd; break;
        }

        // save
        SaveManager.instance.WriteToFile(SetupManager.instance.curProgressData);
    }

    string GetConditionString ( int _condition )
    {
        return (_condition == 1) ? "ON" : "OFF";
    }

    string GetResolutionString ( int _index )
    {
        Resolution curResolution = Screen.resolutions[_index];
        return curResolution.width.ToString() + "x" + curResolution.height.ToString();
    }

    string GetIndexString ( int _index )
    {
        string s;
        float f = 1f + ((float)_index / 10f);
        double d = System.Math.Round(f, 2);
        s = d.ToString();
        float off = .05f;
        if (f > (1f - off) && f < (1f + off))
        {
            s = "1.0";
        }
        if (f < off)
        {
            s = "0.0";
        }
        if (f > (2f - off))
        {
            s = "2.0";
        }
        return s;
    }

    public void CreateSettings ()
    {
        settings = new List<Setting>();
        settingTextObjects = new List<GameObject>();
        settingTextRectTransforms = new List<RectTransform>();
        settingTexts = new List<TextMeshProUGUI>();

        // create settings
        switch ( curMenuState )
        {
            // DEFAULT
            case MenuState.Default:

                settings.Add(new Setting(SettingType.Resume,0,1));
                settings.Add(new Setting(SettingType.Settings,0,1));
                settings.Add(new Setting(SettingType.Quit,0,1));

            break;

            // SETTINGS
            case MenuState.Settings:

                settings.Add(new Setting(SettingType.Resolution, 0, Screen.resolutions.Length - 1));
                settings.Add(new Setting(SettingType.Fullscreen, 0, 1));
                settings.Add(new Setting(SettingType.SFXVolume, -10, 10));
                settings.Add(new Setting(SettingType.MusicVolume, -10, 10));
                settings.Add(new Setting(SettingType.LookSensitivityX, -9, 9));
                settings.Add(new Setting(SettingType.LookSensitivityY, -9, 9));
                settings.Add(new Setting(SettingType.InvertYAxis, 0, 1));
                settings.Add(new Setting(SettingType.WobbleEffect, 0, 1));
                settings.Add(new Setting(SettingType.CameraMotion, 0, 1));

                if (SetupManager.instance.curProgressData.persistentData.unlockedEndless)
                {
                    settings.Add(new Setting(SettingType.EndlessFilter, 0, 1));
                }

                if ( SetupManager.instance.curGameState == SetupManager.GameState.Title)
                {
                    settings.Add(new Setting(SettingType.DeleteSave,0,0));
                }

            break;
        }

        // create objects & visuals
        for (int i = 0; i < settings.Count; i++)
        {
            GameObject newSettingO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.settingsTextPrefab[0], Vector3.zero, Quaternion.identity, 1f);
            Transform newSettingTr = newSettingO.transform;
            newSettingTr.parent = canvasUseRectTransform;

            BasicFunctions.ResetTransform(newSettingTr);

            RectTransform rTr = newSettingO.GetComponent<RectTransform>();
            rTr.localScale = Vector3.one;

            settingTextObjects.Add(newSettingO);
            settingTextRectTransforms.Add(rTr);
            settingTexts.Add(newSettingTr.Find("text0").GetComponent<TextMeshProUGUI>());
        }

        // horizontal & vertical index
        verIndex = 0;
        verIndexMin = 0;
        verIndexMax = (settings.Count - 1);

        SetHorIndex(settings[0].settingType);

        // UI
        if ( headerImage != null )
        {
            headerImage.enabled = true;
        }
        if ( headerText != null )
        {
            headerText.enabled = true;
        }
        if ( quitText != null )
        {
            quitText.enabled = true;
        }

        // update setting visuals
        UpdateSettingVisuals(true);

        // done
        createdSettings = true;

        // log
        //Debug.Log("created settings || " + Time.time.ToString());
    }

    void SetVisuals(bool _to)
    {
        if (headerImage != null)
        {
            headerImage.enabled = _to;
        }
        if (headerText != null)
        {
            headerText.enabled = _to;
        }
        if (quitText != null)
        {
            quitText.enabled = _to;
        }
        //if (settings != null && settings.Count > 0 && settingTextRectTransforms != null && settingTextRectTransforms.Count > 0 && settingTexts != null && settingTexts.Count > 0)
        //{
        //    for (int i = 0; i < settings.Count; i++)
        //    {
        //        settingTexts[i].enabled = _to;
        //    }
        //}
    }

    public void ClearSettings ()
    {
        createdSettings = false;

        for ( int i = settingTextObjects.Count - 1; i >= 0; i -- )
        {
            Destroy(settingTextObjects[i]);
        }
        settingTextObjects.Clear();
        settingTextRectTransforms.Clear();
        settingTexts.Clear();
        settings.Clear();

        // UI
        if (headerImage != null)
        {
            headerImage.enabled = false;
        }
        if (headerText != null)
        {
            headerText.enabled = false;
        }
        if ( quitText != null )
        {
            quitText.enabled = false;
        }

        // log
        //Debug.Log("cleared settings || " + Time.time.ToString());
    }

    public void SetVisible ( bool _to )
    {
        if ( containerObject != null )
        {
            containerObject.SetActive(_to);
        }
    }

    public void SetMenuState ( MenuState _to )
    {
        switch (_to)
        {
            case MenuState.Default:
                headerText.text = "paused";
                break;
            case MenuState.Settings:
                headerText.text = "settings";
                break;
        }
        curMenuState = _to;
    }
}
