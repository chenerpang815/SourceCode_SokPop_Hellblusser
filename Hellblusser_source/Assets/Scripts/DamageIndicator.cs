using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public RectTransform myRectTransform;
    public Text myText;
    public RectTransform myTextRectTransform;
    public Image myImage;

    [HideInInspector] public bool useString;
    [HideInInspector] public string stringSet;

    [HideInInspector] public int damageAmount;

    [HideInInspector] public Vector3 spawnPos;
    [HideInInspector] public Vector3 posCur, forceAdd;

    [HideInInspector] public GameObject myGameObject;

    int clearDur, clearCounter;

    void Start ()
    {
        myGameObject = gameObject;

        posCur = spawnPos;

        clearDur = 60;
        clearCounter = 0;
    }

    void Update ()
    {
        //if (!GameManager.instance.inFreeze)
        {
            // position
            UpdatePosition();

            // rotation
            UpdateRotation();

            // text
            UpdateText();

            // visuals
            UpdateVisuals();

            // clear?
            if ( clearCounter < clearDur )
            {
                clearCounter++;
            }
            else
            {
                Clear();
            }
        }
    }

    void UpdatePosition ()
    {
        forceAdd = Vector3.Lerp(forceAdd,Vector3.zero,17.5f * Time.deltaTime);
        posCur += forceAdd;

        Vector3 p0 = posCur;
        Vector2 p1 = BasicFunctions.WorldToCanvasPosition(GameManager.instance.mainCanvasRectTransform, GameManager.instance.mainCamera, p0);
        myRectTransform.anchoredPosition = p1;
    }

    void UpdateRotation ()
    {
        float t0 = Time.time * 40f;
        float f0 = 2000f * forceAdd.magnitude;
        float zRot = Mathf.Sin(t0) * f0;
        myRectTransform.localRotation = Quaternion.Euler(0f,0f,zRot);
    }

    void UpdateText ()
    {
        if ( myText != null )
        {
            if (!useString)
            {
                myText.text = damageAmount.ToString();
            }
            else
            {
                myText.text = stringSet;
            }
        }
    }

    void UpdateVisuals ()
    {
        Vector3 p0 = posCur;
        Vector3 p1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
        p1.y = p0.y;
        float d0 = Vector3.Distance(p0,p1);
        float threshold0 = 20f;
        bool doShow = (d0 <= threshold0);
        if ( doShow )
        {
            Vector3 p2 = (p1 - p0).normalized;
            float d1 = Vector3.Dot(p2, GameManager.instance.mainCameraTransform.forward);
            float threshold1 = .5f;
            if ( d1 > threshold1)
            {
                doShow = false;
            }
        }
        if (SetupManager.instance.runDataRead.playerDead || GameManager.instance.playerHurt || SetupManager.instance.runDataRead.playerReachedEnd || SetupManager.instance.paused)
        {
            doShow = false;
        }
        if ( SetupManager.instance.tutorialPopupCounter < SetupManager.instance.tutorialPopupDur )
        {
            doShow = false;
        }
        myImage.enabled = false;//doShow;
        myText.enabled = doShow;
    }

    public void Clear ()
    {
        Destroy(myGameObject);
    }
}
