using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

public class IntroManager : MonoBehaviour
{
    // instance
    public static IntroManager instance;

    // info & state
    [HideInInspector] public bool waiting;
    public IntroInfo[] allIntroInfos;
    [HideInInspector] public int introIndex, introContinueWait, introContinueCounter;
    [HideInInspector] public bool done;

    // overlay
    public Transform introVisualsContainer;
    public Transform overlayTransform;
    public MeshRenderer overlayMeshRenderer;
    public Color overlayColShow, overlayColHide;
    Color overlayColCur;
    Vector3 overlaySclOriginal, overlaySclTarget, overlaySclCur;

    List<GameObject> allVisualsObjects;

    // text
    [Header("text")]
    public TextMeshProUGUI waitText;
    public TextMeshProUGUI text;
    int textRate, textCounter, textMax, textIndex;
    public Color textColShow;
    public Color textColHide;

    [System.Serializable]
    public struct IntroInfo
    {
        public GameObject visualsObject;
        public string text;
        public int waitAdd;
    }

    void Awake()
    {
        instance = this;

        // wait?
        waiting = true;
    }

    void Start()
    {
        // initialize
        Init();
    }

    void Init()
    {
        // state
        done = false;
        introIndex = 0;
        introContinueWait = 90;
        introContinueCounter = 0;

        // overlay
        overlayColCur = overlayColHide;
        if ( overlayTransform != null )
        {
            overlaySclOriginal = overlayTransform.localScale;
            overlaySclTarget = overlaySclOriginal;
            overlaySclCur = overlaySclTarget;
        }

        // visuals objects
        allVisualsObjects = new List<GameObject>();
        if ( introVisualsContainer != null )
        {
            foreach ( Transform t in introVisualsContainer )
            {
                if ( t != null && t.gameObject != null )
                {
                    allVisualsObjects.Add(t.gameObject);
                }
            }
        }

        // text
        textRate = 6;
        textCounter = 0;
        textMax = 0;
        textIndex = 0;
        //text.color = textColHide;

        // skip?
        if ( SetupManager.instance.skipIntro || SetupManager.instance.curProgressData.persistentData.sawIntro)
        {
            waiting = false;
            done = true;
            SetupManager.instance.SetGameState(SetupManager.GameState.Title);
        }
    }

    void Update ()
    {
        Hide();
        if (!done)
        {
            HandleIntroInfo();
        }

        if ( waiting )
        {
            if ( InputManager.instance.interactPressed)
            {
                waiting = false;
            }
        }

        // wait text?
        if ( waitText != null )
        {
            if (waiting)
            {
                string baseCol = "<#FFFFFF>";
                waitText.text = baseCol + "press " + SetupManager.instance.UIInteractionButtonCol + InputManager.instance.interactInputStringUse + baseCol + " to start";
            }
            waitText.enabled = waiting;
        }

        // overlay?
        if (overlayMeshRenderer != null)
        {
            Color overlayColTarget = overlayColHide;
            float overlayColLerpie = 1.25f;
            if (!done && !SetupManager.instance.inTransition)
            {
                overlayColTarget = overlayColShow;
            }
            else
            {
                overlayColLerpie = 5f;
            }

            overlayColCur = Color.Lerp(overlayColCur, overlayColTarget, overlayColLerpie * Time.deltaTime);
            overlayMeshRenderer.material.SetColor("_BaseColor", overlayColCur);
        }

        // overlay scaling
        if ( overlayTransform != null )
        {
            overlaySclTarget = overlaySclOriginal;
            if ( done )
            {
                overlaySclTarget *= 8f;
            }
            overlaySclCur = Vector3.Lerp(overlaySclCur,overlaySclTarget,.5f * Time.deltaTime);
            overlayTransform.localScale = overlaySclCur;
        }
    }

    void Hide ()
    {
        for (int i = 0; i < allVisualsObjects.Count; i++)
        {
            allVisualsObjects[i].SetActive(false);
        }
        text.enabled = false;
    }

    void HandleIntroInfo()
    {
        // visuals
        allIntroInfos[introIndex].visualsObject.SetActive(true);

        // text
        Color colTarget = textColShow;
        textMax = allIntroInfos[introIndex].text.Length;
        text.maxVisibleCharacters = textIndex;
        text.text = allIntroInfos[introIndex].text;
        Char[] chars = text.text.ToCharArray();
        text.enabled = true;

        bool textFadeOut = false;

        if (textIndex < textMax)
        {
            if (textCounter < textRate)
            {
                if ( char.IsWhiteSpace(chars[textIndex]) )
                {
                    textCounter = 0;
                    textIndex++;

                    SetupManager.instance.PlayUITextSound();
                }
                if (!waiting)
                {
                    textCounter++;
                }
            }
            else
            {
                textIndex++;
                textCounter = 0;

                SetupManager.instance.PlayUITextSound();
            }
        }
        else
        {
            if (introContinueCounter < (introContinueWait + allIntroInfos[introIndex].waitAdd))
            {
                if (!waiting)
                {
                    introContinueCounter++;
                }

                if ( introContinueCounter > 30 )
                {
                    textFadeOut = true;
                }
            }
            else
            {
                textIndex = 0;
                introContinueCounter = 0;
                if (introIndex < (allIntroInfos.Length - 1))
                {
                    introIndex++;
                }
                else
                {
                    done = true;
                    SetupManager.instance.curProgressData.persistentData.sawIntro = true;
                    SetupManager.instance.SetGameState(SetupManager.GameState.Title);

                    // save
                    SaveManager.instance.WriteToFile(SetupManager.instance.curProgressData);
                }
            }
        }

        if ( textFadeOut )
        {
            colTarget = textColHide;
        }

        float textColLerpie = 5f;
        //text.color = Color.Lerp(text.color,colTarget,textColLerpie * Time.deltaTime);
    }
}
