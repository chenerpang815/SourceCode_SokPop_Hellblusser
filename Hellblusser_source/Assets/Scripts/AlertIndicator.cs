using UnityEngine;
using UnityEngine.UI;

public class AlertIndicator : MonoBehaviour
{
    [HideInInspector] public GameObject myGameObject;
    [HideInInspector] public NpcCore myNpcCore;

    // UI
    [Header("UI")]
    public RectTransform myRectTransform;
    public Text myText;
    public Image myImage;
    public Image myIconImage;

    // offset
    float yOffTarget, yOffCur;

    // clear
    int clearDur, clearCounter;

    // hide
    int hideDur, hideCounter;

    void Start ()
    {
        myGameObject = gameObject;

        yOffTarget = .325f;
        yOffCur = 0f;

        clearDur = 60;
        clearCounter = 0;

        hideDur = 2;
        hideCounter = 0;

        myImage.enabled = false;
        myIconImage.enabled = false;
        myText.enabled = false;
    }

    void Update ()
    {
        //if (!GameManager.instance.inFreeze)
        {
            // position
            UpdatePosition();

            // text
            UpdateText();

            // visuals
            UpdateVisuals();

            // hide?
            if ( hideCounter < hideDur )
            {
                hideCounter++;
            }

            // clear?
            if ( clearCounter < clearDur && myNpcCore != null )
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
        if (myNpcCore != null)
        {
            Vector3 p0 = myNpcCore.myGraphics.graphicsTransform.position;
            p0.y += myNpcCore.myInfo.graphics.healthIndicatorOff;
            if (myNpcCore.myInfo.graphics.flying)
            {
                Vector3 flyOffAdd = (Vector3.up * myNpcCore.myGraphics.flyOff);
                p0 += flyOffAdd;
            }

            yOffCur = Mathf.Lerp(yOffCur, yOffTarget, 15f * Time.deltaTime);
            p0.y += yOffCur;

            Vector2 p1 = BasicFunctions.WorldToCanvasPosition(GameManager.instance.mainCanvasRectTransform, GameManager.instance.mainCamera, p0);
            myRectTransform.anchoredPosition = p1;
        }
    }

    void UpdateText ()
    {
        if (myNpcCore != null)
        {
            if (myText != null)
            {
                myText.text = "!";
            }
        }
    }

    void UpdateVisuals ()
    {
        if (myNpcCore != null)
        {
            Vector3 p0 = myNpcCore.myTransform.position;
            Vector3 p1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
            p1.y = p0.y;
            float d0 = Vector3.Distance(p0, p1);
            float threshold0 = 10f;
            bool doShow = (d0 <= threshold0);
            if (doShow)
            {
                Vector3 p2 = (p1 - p0).normalized;
                float d1 = Vector3.Dot(p2, GameManager.instance.mainCameraTransform.forward);
                float threshold1 = .5f;
                if (d1 > threshold1)
                {
                    doShow = false;
                }
            }
            if (hideCounter < hideDur)
            {
                doShow = false;
            }
            if (SetupManager.instance.runDataRead.playerDead || GameManager.instance.playerHurt || SetupManager.instance.runDataRead.playerReachedEnd || SetupManager.instance.paused)
            {
                doShow = false;
            }
            if ( SetupManager.instance.tutorialPopupCounter < SetupManager.instance.tutorialPopupDur )
            {
                doShow = false;
            }
            myImage.enabled = doShow;
            myText.enabled = doShow;
            myIconImage.enabled = doShow;
        }
    }

    public void Clear ()
    {
        Destroy(myGameObject);
    }
}
