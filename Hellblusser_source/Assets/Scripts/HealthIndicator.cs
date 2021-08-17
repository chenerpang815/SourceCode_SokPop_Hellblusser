using UnityEngine;
using UnityEngine.UI;

public class HealthIndicator : MonoBehaviour
{
    [HideInInspector] public GameObject myGameObject;
    [HideInInspector] public NpcCore myNpcCore;

    // UI
    [Header("UI")]
    public RectTransform myRectTransform;
    public Text myText;
    public RectTransform myTextRectTransform;
    public Image myImage;
    public Image myBlockImage;
    public Image myAttackImage;
    public Sprite backImageSpriteNormal;
    public Sprite backImageSpriteFire;

    // sprites
    [Header("sprites")]
    public Sprite attackMeleeSprite;
    public Sprite attackRangedSprite;
    public Sprite attackHealSprite;

    // colors
    [Header("Colors")]
    public Color imageColNormal;
    public Color imageColBlock;
    public Color imageColAttack;
    public Color textColNormal;
    public Color textColBlock;
    public Color textColAttack;
    public Color flickerColA;
    public Color flickerColB;

    // flicker
    int flickerRate, flickerCounter, flickerIndex, flickerCountMax, flickerCount;

    // hide
    int hideDur, hideCounter;

    void Start ()
    {
        myGameObject = gameObject;

        // hide
        hideDur = 60;
        hideCounter = 0;
    }

    void Update ()
    {
        // position
        UpdatePosition();

        // text
        UpdateText();

        // visuals
        UpdateVisuals();

        // flicker
        if (flickerCount < flickerCountMax)
        {
            if (flickerCounter < flickerRate)
            {
                flickerCounter++;
            }
            else
            {
                flickerCounter = 0;
                flickerCount++;
                flickerIndex = (flickerIndex == 0) ? 1 : 0;
            }
        }

        // hide?
        if (myNpcCore.curState == NpcCore.State.Roam || myNpcCore.curState == NpcCore.State.Alerted || myNpcCore.curState == NpcCore.State.Sleeping || myNpcCore.curState == NpcCore.State.Hide )
        {
            hideCounter = 0;
        }
        else
        {
            if ( hideCounter < hideDur )
            {
                hideCounter++;
            }
        }

        // hide?
        if ( SetupManager.instance.tutorialPopupCounter < SetupManager.instance.tutorialPopupDur )
        {
            HideAllContent();
        }

        // clear?
        if ( myNpcCore == null )
        {
            Clear();
        }
    }

    public void HideAllContent ()
    {
        myImage.enabled = false;
        myText.enabled = false;
        myBlockImage.enabled = false;
        myAttackImage.enabled = false;
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
                myText.text = (myNpcCore.health > 0) ? myNpcCore.health.ToString() : "x";
                if ( myNpcCore.myInfo.stats.immortal )
                {
                    myText.text = "?";
                }

                if ( myTextRectTransform != null )
                {
                    myTextRectTransform.anchoredPosition = (myNpcCore.myInfo.stats.healthIsFire) ? new Vector3(0f,-10f,0f) : Vector3.zero;
                }
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
            float threshold0 = 6f + myNpcCore.myInfo.graphics.healthIndicatorExtraRange;
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
                if (myNpcCore.curState == NpcCore.State.Alerted)
                {
                    doShow = false;
                }
                if (hideCounter < hideDur)
                {
                    doShow = false;
                }
            }
            if (SetupManager.instance.runDataRead.playerDead || GameManager.instance.playerHurt || SetupManager.instance.runDataRead.playerReachedEnd || SetupManager.instance.paused)
            {
                doShow = false;
            }
            bool showText = false;
            bool showBlockIcon = false;
            bool showAttackIcon = false;
            if (doShow)
            {
                showText = (myNpcCore.curState != NpcCore.State.Block && myNpcCore.curState != NpcCore.State.AttackPrepare && (myNpcCore.curState != NpcCore.State.AttackDo || (myNpcCore.curState == NpcCore.State.AttackDo && myNpcCore.attackDoCounter >= (myNpcCore.curAttackData.attackDoDur / 2))));
                showBlockIcon = (myNpcCore.curState == NpcCore.State.Block);
                showAttackIcon = (myNpcCore.curState == NpcCore.State.AttackPrepare || (myNpcCore.curState == NpcCore.State.AttackDo && myNpcCore.attackDoCounter <= (myNpcCore.curAttackData.attackDoDur / 2)));

                if ( myNpcCore.curState == NpcCore.State.AttackDo && myNpcCore.curAttackData.attackType == Npc.AttackData.AttackType.HealSelf || myNpcCore.curAttackData.attackType == Npc.AttackData.AttackType.HealRadius )
                {
                    showText = true;
                    showBlockIcon = false;
                    showAttackIcon = false;
                }
            }

            myImage.enabled = doShow;
            myText.enabled = showText;
            myBlockImage.enabled = showBlockIcon;
            myAttackImage.enabled = showAttackIcon;

            // attack sprite?
            if ( showAttackIcon )
            {
                switch ( myNpcCore.curAttackData.attackType )
                {
                    default: myAttackImage.sprite = attackMeleeSprite; break;
                    case Npc.AttackData.AttackType.Ranged: myAttackImage.sprite = attackRangedSprite; break;
                    case Npc.AttackData.AttackType.HealSelf: myAttackImage.sprite = attackHealSprite; break;
                    case Npc.AttackData.AttackType.HealRadius: myAttackImage.sprite = attackHealSprite; break;
                }
            }

            // back sprite?
            if ( myImage != null )
            {
                myImage.sprite = (myNpcCore.myInfo.stats.healthIsFire) ? backImageSpriteFire : backImageSpriteNormal;
            }

            // flicker?
            if (flickerCount < flickerCountMax)
            {
                myImage.color = (flickerIndex == 0) ? flickerColA : flickerColB;
                myText.color = (flickerIndex == 1) ? flickerColA : flickerColB;
            }
            else
            {
                Color imageColSet = imageColNormal;
                Color textColSet = textColNormal;
                switch ( myNpcCore.curState )
                {
                    case NpcCore.State.Block:
                        imageColSet = imageColBlock;
                        textColSet = textColBlock;
                        break;

                    case NpcCore.State.AttackPrepare:
                        imageColSet = imageColAttack;
                        textColSet = textColAttack;
                        break;

                    case NpcCore.State.AttackDo:
                        if (myNpcCore.attackDoCounter <= (myNpcCore.curAttackData.attackDoDur / 2))
                        {
                            imageColSet = imageColAttack;
                            textColSet = textColAttack;
                        }
                        break;
                }
                myImage.color = imageColSet;
                myText.color = textColSet;
            }
        }
    }

    public void InitFlicker ( int _count )
    {
        flickerCount = 0;
        flickerCountMax = _count;
        flickerCounter = 0;
        flickerRate = 3;
        flickerIndex = 0;
    }

    public void Clear ()
    {
        Destroy(myGameObject);
    }
}
