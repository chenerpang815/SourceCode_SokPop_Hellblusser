using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class RestManager : MonoBehaviour
{
    // instance
    public static RestManager instance;

    // campfire
    [Header("campfire")]
    public CampfireScript campfireScript;

    // state
    [HideInInspector] public bool didRest;
    [HideInInspector] public bool leaving;

    void Awake ()
    {
        instance = this;

        // get seed
        SetupManager.instance.SetRunSeedNotLevel();
    }

    void Start ()
    {
        // state
        didRest = false;
        leaving = false;

        // for coin & tear hacks
        SetupManager.instance.curLevelMaxCoins = 999;
        SetupManager.instance.curLevelMaxTears = 999;
    }

    void Update ()
    {
        // freeze because leaving?
        if ( leaving )
        {
            SetupManager.instance.SetFreeze(1);
        }
    }

    public void InitProceedToNextLevel ()
    {
        SetupManager.instance.SetTransition(SetupManager.TransitionMode.In);
        Invoke("ProceedToNextLevel", SetupManager.instance.sceneLoadWait);
        leaving = true;

        // save
        switch (SetupManager.instance.curRunType)
        {
            case SetupManager.RunType.Normal:
                SaveManager.instance.StoreLastLevelData(ref SetupManager.instance.curProgressData.normalRunData);
                SetupManager.instance.curProgressData.normalRunData.curLevelIndex++;
                break;
            case SetupManager.RunType.Endless:
                SaveManager.instance.StoreLastLevelData(ref SetupManager.instance.curProgressData.endlessRunData);
                SetupManager.instance.curProgressData.endlessRunData.curLevelIndex++;
                break;
        }
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
        //switch ( SetupManager.instance.curRunType )
        //{
        //    case SetupManager.RunType.Normal:
        //        SetupManager.instance.curProgressData.normalRunData.lastLevelCoinsCollected = SetupManager.instance.curProgressData.normalRunData.curRunCoinsCollected;
        //        SetupManager.instance.curProgressData.normalRunData.lastLevelTearsCollected = SetupManager.instance.curProgressData.normalRunData.curRunTearsCollected;
        //        break;

        //    case SetupManager.RunType.Endless:
        //        SetupManager.instance.curProgressData.endlessRunData.lastLevelCoinsCollected = SetupManager.instance.curProgressData.endlessRunData.curRunCoinsCollected;
        //        SetupManager.instance.curProgressData.endlessRunData.lastLevelTearsCollected = SetupManager.instance.curProgressData.endlessRunData.curRunTearsCollected;
        //        break;
        //}

        // clear spawners
        SetupManager.instance.ClearAllPropSpawners();
        SetupManager.instance.ClearAllNpcSpawners();
        SetupManager.instance.ClearAllFountainAreas();

        // reset input
        InputManager.instance.DisableAllInput();

        // transition
        SetupManager.instance.transitionFactor = 0f;

        // go
        SetupManager.instance.SetGameState(SetupManager.GameState.LevelSelect);
        SceneManager.LoadScene(7 + 1, LoadSceneMode.Single);
    }
}
