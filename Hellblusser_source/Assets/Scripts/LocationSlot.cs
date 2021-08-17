using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocationSlot : MonoBehaviour
{
    // location type
    public SetupManager.LocationType myLocationType;

    // location index
    [HideInInspector] public int locationIndex, locationSubIndex;

    // position type
    public enum PositionType { Top, Center, Bottom }
    public PositionType myPositionType;

    // state
    public bool[] barHighlight;

    // references
    [Header("references")]
    public RectTransform myRectTransform;
    public Image myBackImage;
    public Image myIconImage;
    public RectTransform[] myBarRectTransforms;
    public Image[] myBarImages;
    public TextMeshProUGUI myMysteryText;

    // sprites
    [Header("sprites")]
    public Sprite levelSprite;
    public Sprite bossLevelSprite;
    public Sprite shopSprite;
    public Sprite fountainSprite;
    public Sprite restSprite;

    // colors
    [Header("colors")]
    public Color backColPlayerNear;
    public Color backColPlayerFar;
    public Color backColSelected;
    public Color backColPlayerLocation;
    public Color backColPlayerVisited;

    public Color iconColActive;
    public Color iconColInactive;
    public Color iconColPlayerLocation;
    public Color iconColSelected;
    public Color iconColPlayerVisited;

    public Color barColNormal;
    public Color barColHighlight;

    void Update ()
    {
        // update icon
        UpdateLocationIcon();

        // update bars
        UpdateBars();
    }

    public void SetVisible ( bool _to )
    {
        myBackImage.enabled = _to;
        //myIconImage.enabled = _to;
        if (locationIndex != (SetupManager.instance.runDataRead.curLevelIndex - 1))
        {
            for (int i = 0; i < myBarImages.Length; i++)
            {
                myBarImages[i].enabled = _to;
            }
        }

        if ( SetupManager.instance.inFreeze && !SetupManager.instance.quitToTitle )
        {
            for (int i = 0; i < myBarImages.Length; i++)
            {
                myBarImages[i].enabled = false;
            }
        }
    }

    public void SetPositionType ( PositionType _to )
    {
        switch (_to)
        {
            case PositionType.Center:
                for ( int i = 0; i < myBarRectTransforms.Length; i ++ )
                {
                    if (locationIndex < (SetupManager.instance.runDataRead.curFloorData.locationCount - 1) && SetupManager.instance.runDataRead.curFloorData.locationTypes[locationIndex + 1].types.Count > 1)
                    {
                        float barDir = (i == 0) ? 1f : -1f;
                        myBarRectTransforms[i].localRotation = Quaternion.Euler(0f, 0f, 45f * barDir);
                    }
                    else
                    {
                        myBarRectTransforms[i].localRotation = Quaternion.Euler(0f, 0f, 0f);
                    }
                }
                break;
            case PositionType.Top:
                for (int i = 0; i < myBarRectTransforms.Length; i++)
                {
                    myBarRectTransforms[i].localRotation = Quaternion.Euler(0f, 0f, -45f);
                }
                break;
            case PositionType.Bottom:
                for (int i = 0; i < myBarRectTransforms.Length; i++)
                {
                    myBarRectTransforms[i].localRotation = Quaternion.Euler(0f, 0f, 45f);
                }
                break;
        }

        myPositionType = _to;
    }

    void UpdateBars ()
    {
        // hide bars?
        //if (!SetupManager.instance.inTransition)
        {
            if (locationIndex == (SetupManager.instance.runDataRead.curFloorData.locationCount - 1))
            {
                for (int i = 0; i < myBarImages.Length; i++)
                {
                    myBarImages[i].enabled = false;
                }
            }
            else
            {
                for (int i = 0; i < myBarImages.Length; i++)
                {
                    bool playerVisited = (i == SetupManager.instance.runDataRead.curFloorData.locationVisitedIndex[locationIndex]);

                    barHighlight[i] = false;
                    if (locationIndex == (SetupManager.instance.runDataRead.curLevelIndex - 1))
                    {
                        switch (myPositionType)
                        {
                            case PositionType.Bottom:
                                if (playerVisited)
                                {
                                    barHighlight[i] = true;
                                }
                                break;
                            case PositionType.Center:
                                if (LevelSelectManager.instance.locationSelectIndex == i)
                                {
                                    barHighlight[i] = true;
                                }
                                break;
                            case PositionType.Top:
                                if (playerVisited)
                                {
                                    barHighlight[i] = true;
                                }

                                //Debug.Log("hmmm speler hier geweest? " + playerVisited + " || " + Time.time.ToString());
                                break;
                        }

                        //if (playerVisited)
                        //{
                        //    myBarImages[i].enabled = barHighlight[i];
                        //}
                    }
                    else
                    {
                        //myBarImages[i].enabled = true;
                    }
                    
                    //myBarImages[i].color = (barHighlight[i]) ? barColHighlight : barColNormal;
                }
            }
        }
    }

    public void UpdateLocationIcon ()
    {
        if (!LevelSelectManager.instance.leaving)
        {
            bool playerNearLocation = (locationIndex == SetupManager.instance.runDataRead.curLevelIndex);
            bool playerPastLocation = (locationIndex < SetupManager.instance.runDataRead.curLevelIndex);
            bool playerVisited = (locationSubIndex == SetupManager.instance.runDataRead.curFloorData.locationVisitedIndex[locationIndex]);
            bool playerAtLocation = (playerVisited && locationIndex == (SetupManager.instance.runDataRead.curLevelIndex - 1));

            // back
            if (myBackImage != null)
            {
                Color backColSet = backColPlayerNear;
                if (!playerNearLocation)
                {
                    backColSet = backColPlayerFar;

                    // log
                    //if (locationIndex == 0)
                    //{
                    //    Debug.Log("player not near location || " + Time.time.ToString());
                    //}
                }
                if (playerNearLocation && !playerAtLocation && LevelSelectManager.instance.locationSelectIndex == locationSubIndex)
                {
                    backColSet = backColSelected;

                    // log
                    //if (locationIndex == 0)
                    //{
                    //    Debug.Log("player near location, not at location, and selectIndex == locationSubIndex || " + Time.time.ToString());
                    //}
                }
                if (playerPastLocation && playerVisited)
                {
                    backColSet = backColPlayerVisited;

                    // log
                    //if (locationIndex == 0)
                    //{
                    //    Debug.Log("player past location || " + Time.time.ToString());
                    //}
                }
                if (playerAtLocation)
                {
                    backColSet = backColPlayerLocation;

                    // log
                    //if (locationIndex == 0)
                    //{
                    //    Debug.Log("player is at location || " + Time.time.ToString());
                    //}
                }
                myBackImage.color = backColSet;
            }

            // icon
            if (myIconImage != null)
            {
                switch (myLocationType)
                {
                    case SetupManager.LocationType.Level: myIconImage.sprite = levelSprite; break;
                    case SetupManager.LocationType.BossLevel: myIconImage.sprite = bossLevelSprite; break;
                    case SetupManager.LocationType.Shop: myIconImage.sprite = shopSprite; break;
                    case SetupManager.LocationType.Fountain: myIconImage.sprite = fountainSprite; break;
                    case SetupManager.LocationType.Rest: myIconImage.sprite = restSprite; break;
                }

                bool showIcon = (!SetupManager.instance.paused && (playerNearLocation || playerPastLocation || (locationIndex == (SetupManager.instance.runDataRead.curFloorData.locationCount - 1))));
                myIconImage.enabled = showIcon;

                myIconImage.color = (SetupManager.instance.runDataRead.curLevelIndex > locationIndex) ? iconColInactive : iconColActive;
                if (playerNearLocation && !playerAtLocation && LevelSelectManager.instance.locationSelectIndex == locationSubIndex)
                {
                    myIconImage.color = iconColSelected;
                }
                if (playerPastLocation && playerVisited)
                {
                    myIconImage.color = iconColPlayerVisited;
                }
                if (playerAtLocation)
                {
                    myIconImage.color = iconColPlayerLocation;
                }
            }

            // mystery text?
            if (myMysteryText != null)
            {
                bool showMysteryText = (!SetupManager.instance.paused && !playerNearLocation && !playerPastLocation && (locationIndex != (SetupManager.instance.runDataRead.curFloorData.locationCount - 1)));
                myMysteryText.enabled = showMysteryText;
            }
        }
    }
}
