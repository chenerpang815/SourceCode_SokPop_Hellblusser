using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    // instance
    public static ShopManager instance;

    // options
    [Header("shop options")]
    public List<EquipmentDatabase.Equipment> shopOptions;
    public List<Transform> shopItemShowTransforms;
    public InteractionScript itemBrowseInteractionScript;

    [HideInInspector] public List<Image> shopBrowseItemImages;
    [HideInInspector] public List<Image> shopBrowseItemBackImages;
    [HideInInspector] public List<Image> shopBrowseItemTypeImages;
    [HideInInspector] public List<RectTransform> shopBrowseItemRectTransforms;
    [HideInInspector] public List<GameObject> shopBrowseItemObjects;
    [HideInInspector] public List<Text> shopBrowseItemNames;
    [HideInInspector] public List<Text> shopBrowseItemInfosLeft;
    [HideInInspector] public List<Text> shopBrowseItemInfosRight;

    // colors
    [Header("colors")]
    public Color itemBrowseInspectFrontCol;
    public Color itemBrowseInspectBackCol;
    public Color itemBrowseEquippedFrontCol;
    public Color itemBrowseEquippedBackCol;
    public Color itemBrowseTypeInspectCol;
    public Color itemBrowseTypeEquippedCol;
    public Color itemNameColHide;
    public Color itemNameColShow;
    public Color itemInfoColShop;
    public Color itemInfoColYou;

    // state
    [HideInInspector] public int itemBrowseIndex;
    [HideInInspector] public int canBuyItemDur, canBuyItemCounter;
    [HideInInspector] public int canSwitchPickDur, canSwitchPickCounter;
    [HideInInspector] public bool inItemBrowse;
    [HideInInspector] public bool leaving;

    // colors
    [Header("colors")]
    public Color iconNameColHide;
    public Color iconNameColShow;

    void Awake()
    {
        instance = this;

        // get seed
        SetupManager.instance.SetRunSeedNotLevel();
    }

    void Start ()
    {
        // state
        leaving = false;

        // for coin & tear hacks
        SetupManager.instance.curLevelMaxCoins = 999;
        SetupManager.instance.curLevelMaxTears = 999;

        // counters
        canBuyItemDur = 10;
        canSwitchPickDur = 8;

        // prepare shop options
        PrepareShopOptions();
    }

    void PrepareShopOptions ()
    {
        // init list
        shopOptions = new List<EquipmentDatabase.Equipment>();

        // create temporary lists
        List<EquipmentDatabase.Equipment> allEquipments = new List<EquipmentDatabase.Equipment>();
        List<EquipmentDatabase.Equipment> equipmentsLeft = new List<EquipmentDatabase.Equipment>();

        // exclude list?
        List<EquipmentDatabase.Equipment> equipmentsExclude = new List<EquipmentDatabase.Equipment>();
        equipmentsExclude.Add(EquipmentDatabase.Equipment.Torch);
        equipmentsExclude.Add(EquipmentDatabase.Equipment.BasicWand);
        equipmentsExclude.Add(EquipmentDatabase.Equipment.WhiteCloak);
        equipmentsExclude.Add(EquipmentDatabase.Equipment.BlueCloak);
        equipmentsExclude.Add(EquipmentDatabase.Equipment.IronSword);

        // store all possible equipments
        var enumerator = EquipmentDatabase.instance.equipmentDatas.GetEnumerator();
        while (enumerator.MoveNext())
        {
            EquipmentDatabase.Equipment equipmentCheck = enumerator.Current.Key;
            if (!equipmentsExclude.Contains(equipmentCheck))
            {
                allEquipments.Add(equipmentCheck);
                equipmentsLeft.Add(equipmentCheck);
            }
        }

        // leave out the items we already have
        for (int i = equipmentsLeft.Count - 1; i >= 0; i--)
        {
            if (SetupManager.instance.CheckIfHasEquipment(equipmentsLeft[i]))
            {
                equipmentsLeft.RemoveAt(i);
            }
        }

        bool overrideShopOptions = false;
        if (!overrideShopOptions)
        {
            int pickIndex = 0;
            int equipmentPickCount = 4;//shopBrowseItemRectTransforms.Count + 1;
            while (equipmentsLeft.Count > 1 && shopOptions.Count < equipmentPickCount)
            {
                int rIndex = Mathf.RoundToInt(TommieRandom.instance.GenerationRandomRange(0f, equipmentsLeft.Count - 1,"shop manager pick possible items"));
                EquipmentDatabase.Equipment rEquipment = equipmentsLeft[rIndex];
                EquipmentDatabase.EquipmentData rEquipmentData = EquipmentDatabase.instance.GetEquipmentData(rEquipment);

                bool canPick = false;
                switch (pickIndex)
                {
                    case 0: canPick = (rEquipmentData.slot == EquipmentDatabase.Slot.Body); break;
                    case 1: canPick = (rEquipmentData.slot == EquipmentDatabase.Slot.Ring || rEquipmentData.slot == EquipmentDatabase.Slot.Bracelet); break;
                    case 2: canPick = (rEquipmentData.slot == EquipmentDatabase.Slot.Weapon); break;
                    case 3: canPick = true; break;
                }
                if (canPick)
                {
                    shopOptions.Add(equipmentsLeft[rIndex]);
                    equipmentsLeft.Remove(equipmentsLeft[rIndex]);
                    pickIndex++;
                }
            }
        }
        else
        {
            shopOptions.Add(EquipmentDatabase.Equipment.GoldBlade);
            shopOptions.Add(EquipmentDatabase.Equipment.SimpleMace);
            shopOptions.Add(EquipmentDatabase.Equipment.JumpExplodeBracelet);
            shopOptions.Add(EquipmentDatabase.Equipment.RedCloak);
            //shopOptions.Add(EquipmentDatabase.Equipment.RecoilBracelet);
            //shopOptions.Add(EquipmentDatabase.Equipment.JumpExplodeBracelet);
            //shopOptions.Add(EquipmentDatabase.Equipment.SimpleMace);
            //shopOptions.Add(EquipmentDatabase.Equipment.ExplosiveRing);
        }

        // create objects
        for ( int i = 0; i < shopOptions.Count; i ++ )
        {
            EquipmentDatabase.EquipmentData curEquipmentData = EquipmentDatabase.instance.GetEquipmentData(shopOptions[i]);

            Vector3 posOffUse = Vector3.zero;
            Vector3 rotOffUse = Vector3.zero;
            float sclUse = 1f;
            switch (curEquipmentData.slot)
            {
                case EquipmentDatabase.Slot.Body: posOffUse = EquipmentDatabase.instance.bodyShopPosOff; rotOffUse = EquipmentDatabase.instance.bodyShopRotOff; sclUse = EquipmentDatabase.instance.bodyShopSclFactor; break;
                case EquipmentDatabase.Slot.Bracelet: posOffUse = EquipmentDatabase.instance.braceletShopPosOff; rotOffUse = EquipmentDatabase.instance.braceletShopRotOff; sclUse = EquipmentDatabase.instance.braceletShopSclFactor; break;
                case EquipmentDatabase.Slot.Ring: posOffUse = EquipmentDatabase.instance.ringShopPosOff; rotOffUse = EquipmentDatabase.instance.ringShopRotOff; sclUse = EquipmentDatabase.instance.ringShopSclFactor; break;
                case EquipmentDatabase.Slot.Weapon: posOffUse = EquipmentDatabase.instance.weaponShopPosOff; rotOffUse = EquipmentDatabase.instance.weaponShopRotOff; sclUse = EquipmentDatabase.instance.weaponShopSclFactor; break;
            }

            Vector3 p = shopItemShowTransforms[i].position + posOffUse;
            Quaternion r = shopItemShowTransforms[i].rotation * Quaternion.Euler(rotOffUse);
            float s = .375f;
            s *= sclUse;

            GameObject itemShopO = PrefabManager.instance.SpawnPrefabAsGameObject(curEquipmentData.visualsObject,p,r,s);
            itemShopO.layer = 14;

            InteractionScript itemShopInteractionScript = itemShopO.AddComponent<InteractionScript>();
            itemShopInteractionScript.myInteractionType = GameManager.InteractionType.ShopItem;
            itemShopInteractionScript.myEquipmentData = curEquipmentData;
            itemShopInteractionScript.myEquipmentObject = itemShopO;
            itemShopInteractionScript.myEquipment = shopOptions[i];

            BoxCollider itemShopCollider = itemShopO.AddComponent<BoxCollider>();
            itemShopCollider.center = Vector3.zero;

            float colliderScl = 1f;
            if ( curEquipmentData.slot == EquipmentDatabase.Slot.Weapon )
            {
                colliderScl = 4f;
            }
            itemShopCollider.size = (Vector3.one * colliderScl);
        }

        // state
        canBuyItemCounter = 0;
        itemBrowseIndex = 0;
        inItemBrowse = false;
    }

    public void InitShopBrowse ( InteractionScript _interactionScript )
    {
        shopBrowseItemRectTransforms = new List<RectTransform>();
        shopBrowseItemObjects = new List<GameObject>();
        shopBrowseItemImages = new List<Image>();
        shopBrowseItemBackImages = new List<Image>();
        shopBrowseItemTypeImages = new List<Image>();
        shopBrowseItemNames = new List<Text>();
        shopBrowseItemInfosLeft = new List<Text>();
        shopBrowseItemInfosRight = new List<Text>();
        for (int i = 0; i < 2; i++)
        {
            GameObject newShopBrowseIconO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.shopBrowseItemPrefab[0], Vector3.zero, Quaternion.identity, 1f);
            Transform newShopBrowseIconTr = newShopBrowseIconO.transform;
            newShopBrowseIconTr.parent = GameManager.instance.mainCanvasRectTransform;

            BasicFunctions.ResetTransform(newShopBrowseIconTr);

            RectTransform rTr = newShopBrowseIconO.GetComponent<RectTransform>();
            rTr.localScale = Vector3.one;

            shopBrowseItemRectTransforms.Add(newShopBrowseIconO.GetComponent<RectTransform>());
            shopBrowseItemObjects.Add(newShopBrowseIconO);
            shopBrowseItemImages.Add(newShopBrowseIconTr.Find("frontImage0").GetComponent<Image>());
            shopBrowseItemBackImages.Add(newShopBrowseIconTr.Find("backImage0").GetComponent<Image>());
            shopBrowseItemTypeImages.Add(newShopBrowseIconTr.Find("typeImage0").GetComponent<Image>());
            shopBrowseItemNames.Add(newShopBrowseIconTr.Find("nameText0").GetComponent<Text>());
            shopBrowseItemInfosLeft.Add(newShopBrowseIconTr.Find("infoLeftText0").GetComponent<Text>());
            shopBrowseItemInfosRight.Add(newShopBrowseIconTr.Find("infoRightText0").GetComponent<Text>());
        }

        // state
        canSwitchPickCounter = 0;
        canBuyItemCounter = 0;
        itemBrowseIndex = 0;
        SetupManager.instance.SetGameState(SetupManager.GameState.ShopBrowse);
        itemBrowseInteractionScript = _interactionScript;
        inItemBrowse = true;
    }

    public void LeaveShopBrowse ()
    {
        ClearAllItemBrowses();
        SetupManager.instance.SetHideUI(16);
        SetupManager.instance.SetGameState(SetupManager.GameState.Shop);
        itemBrowseInteractionScript = null;
        inItemBrowse = false;
        canBuyItemCounter = 0;
    }

    void Update ()
    {
        // freeze because leaving?
        if (leaving)
        {
            SetupManager.instance.SetFreeze(1);
        }

        // freeze because browsing?
        if ( inItemBrowse )
        {
            SetupManager.instance.SetFreeze(1);
        }

        // handle item browse
        HandleItemBrowse();
    }

    void HandleItemBrowse ()
    {
        // handle behaviour for possible options
        if (inItemBrowse && GameManager.instance != null && GameManager.instance.curInteractionScript != null )
        {
            int itemBrowseCount = 2;
            float hOff = 840f;
            float hAdd = (hOff / (float)(itemBrowseCount));
            float hStart = (-(float)(itemBrowseCount / 2) * (hAdd * .5f));
            for (int i = 0; i < itemBrowseCount; i++)
            {
                float iFloat = (float)(i);

                bool isSelected = (i == itemBrowseIndex);
                bool slotEmpty = false;

                EquipmentDatabase.EquipmentData curEquipmentData = GameManager.instance.curInteractionScript.myEquipmentData;
                if (i == 1)
                {
                    switch (GameManager.instance.curInteractionScript.myEquipmentData.slot)
                    {
                        case EquipmentDatabase.Slot.Body:
                            slotEmpty = (SetupManager.instance.runDataRead.curBodyEquipmentIndex == -1);
                            if (!slotEmpty)
                            {
                                curEquipmentData = EquipmentDatabase.instance.GetEquipmentData((EquipmentDatabase.Equipment)SetupManager.instance.runDataRead.curBodyEquipmentIndex);
                            }
                            break;
                        case EquipmentDatabase.Slot.Bracelet:
                            slotEmpty = (SetupManager.instance.runDataRead.curBraceletEquipmentIndex == -1);
                            if (!slotEmpty)
                            {
                                curEquipmentData = EquipmentDatabase.instance.GetEquipmentData((EquipmentDatabase.Equipment)SetupManager.instance.runDataRead.curBraceletEquipmentIndex);
                            }
                            break;
                        case EquipmentDatabase.Slot.Ring:
                            slotEmpty = (SetupManager.instance.runDataRead.curRingEquipmentIndex == -1);
                            if (!slotEmpty)
                            {
                                curEquipmentData = EquipmentDatabase.instance.GetEquipmentData((EquipmentDatabase.Equipment)SetupManager.instance.runDataRead.curRingEquipmentIndex);
                            }
                            break;
                        case EquipmentDatabase.Slot.Weapon:
                            slotEmpty = (SetupManager.instance.runDataRead.curWeaponEquipmentIndex == -1);
                            if (!slotEmpty)
                            {
                                curEquipmentData = EquipmentDatabase.instance.GetEquipmentData((EquipmentDatabase.Equipment)SetupManager.instance.runDataRead.curWeaponEquipmentIndex);
                            }
                            break;
                    }
                }

                //shopBrowseItemInfosLeft[i].text = (i == 0) ? "new" : "equipped";
                shopBrowseItemInfosLeft[i].enabled = (i == 0);
                shopBrowseItemInfosRight[i].enabled = (i == 1);
                shopBrowseItemNames[i].text = (slotEmpty) ? "nothing" : curEquipmentData.nameFormatted;
                shopBrowseItemNames[i].color = (isSelected) ? itemNameColShow : itemNameColHide;
                shopBrowseItemInfosLeft[i].color = (isSelected && i == 0) ? itemInfoColShop : itemNameColHide;
                shopBrowseItemInfosRight[i].color = (isSelected && i == 1) ? itemInfoColYou : itemNameColHide;

                // colors
                shopBrowseItemImages[i].color = (isSelected) ? itemBrowseInspectFrontCol : itemBrowseEquippedFrontCol;
                shopBrowseItemBackImages[i].color = (isSelected) ? ((i == 0) ? itemInfoColShop : itemInfoColYou) : itemBrowseEquippedBackCol;

                shopBrowseItemTypeImages[i].color = (isSelected) ? itemBrowseTypeInspectCol : itemBrowseTypeEquippedCol;

                // shop browse description string
                if (isSelected)
                {
                    GameManager.instance.shopBrowseDescriptionText.text = (i == 0 || (i == 1 && !slotEmpty)) ? curEquipmentData.description : "you have nothing equipped";
                }

                // type icon
                Sprite typeSpriteSet = null;
                if (!slotEmpty)
                {
                    switch (curEquipmentData.slot)
                    {
                        case EquipmentDatabase.Slot.Body: typeSpriteSet = EquipmentDatabase.instance.equipmentBodySprite; break;
                        case EquipmentDatabase.Slot.Bracelet: typeSpriteSet = EquipmentDatabase.instance.equipmentBraceletSprite; break;
                        case EquipmentDatabase.Slot.Ring: typeSpriteSet = EquipmentDatabase.instance.equipmentRingSprite; break;
                        case EquipmentDatabase.Slot.Weapon: typeSpriteSet = EquipmentDatabase.instance.equipmentWeaponSprite; break;
                    }
                }
                else
                {
                    typeSpriteSet = EquipmentDatabase.instance.equipmentEmptySprite;
                }
                shopBrowseItemTypeImages[i].sprite = typeSpriteSet;

                // position
                Vector3 p = Vector3.zero;
                p.x = hStart + (hAdd * iFloat);
                p.y = -200f;//120f;

                shopBrowseItemRectTransforms[i].anchoredPosition = p;
            }

            // item browse arrow
            if (GameManager.instance != null && GameManager.instance.shopBrowseArrowRectTransform != null)
            {
                Vector3 arrowP = new Vector3(0f, shopBrowseItemRectTransforms[0].anchoredPosition.y, 0f);
                arrowP.y -= 20f;
                GameManager.instance.shopBrowseArrowRectTransform.anchoredPosition = arrowP;
            }

            // move selection
            if (canSwitchPickCounter < canSwitchPickDur)
            {
                canSwitchPickCounter++;
            }
            else
            {
                float selectThreshold = .25f;
                float hInput = InputManager.instance.moveDirection.x;
                int minIndex = 0;
                int maxIndex = 1;
                if (itemBrowseIndex < maxIndex && (hInput > selectThreshold))
                {
                    itemBrowseIndex++;
                    canSwitchPickCounter = 0;

                    // audio
                    SetupManager.instance.PlayUINavigateSound();
                }
                if (itemBrowseIndex > minIndex && (hInput < -selectThreshold))
                {
                    itemBrowseIndex--;
                    canSwitchPickCounter = 0;

                    // audio
                    SetupManager.instance.PlayUINavigateSound();
                }
            }

            // buy or cancel?
            if ( canBuyItemCounter < canBuyItemDur )
            {
                canBuyItemCounter++;
            }
            else
            {
                // buy?
                if (itemBrowseIndex == 0)
                {
                    int coinCostUse = GameManager.instance.curInteractionScript.myEquipmentData.shopCoinCost;
                    if ( SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Thrifty) )
                    {
                        coinCostUse = Mathf.RoundToInt(coinCostUse * .66f);
                    }

                    if (/*SetupManager.instance.canInteract &&*/ InputManager.instance.interactPressed)
                    {

                        bool playerHasEnoughCoins = ( /*GameManager.instance.coinsCollected*/ SetupManager.instance.runDataRead.curRunCoinsCollected >= coinCostUse);
                        if (playerHasEnoughCoins)
                        {
                            bool isNormalRun = (SetupManager.instance.curRunType == SetupManager.RunType.Normal);

                            InteractionScript curInteractionScript = GameManager.instance.curInteractionScript;
                            EquipmentDatabase.EquipmentData curEquipmentData = curInteractionScript.myEquipmentData;
                            EquipmentDatabase.Equipment curEquipment = curInteractionScript.myEquipment;
                            EquipmentDatabase.EquipmentStats curEquipmentStatas = curEquipmentData.stats;
                            switch (curEquipmentData.slot )
                            {
                                case EquipmentDatabase.Slot.Body:

                                    if (isNormalRun)
                                    {
                                        if ( SetupManager.instance.curProgressData.normalRunData.curBodyEquipmentIndex != -1 )
                                        {
                                            EquipmentDatabase.Equipment prevEquipment = (EquipmentDatabase.Equipment)SetupManager.instance.curProgressData.normalRunData.curBodyEquipmentIndex;
                                            EquipmentDatabase.EquipmentStats prevEquipmentStats = EquipmentDatabase.instance.GetEquipmentData(prevEquipment).stats;
                                            SetupManager.instance.AddPlayerMaxHealth(prevEquipmentStats.maxHealthAdd);
                                            SetupManager.instance.AddPlayerMaxFire(prevEquipmentStats.maxFireAdd);
                                        }
                                        SetupManager.instance.curProgressData.normalRunData.curBodyEquipmentIndex = (int)curEquipment;
                                    }
                                    else
                                    {
                                        if (SetupManager.instance.curProgressData.endlessRunData.curBodyEquipmentIndex != -1)
                                        {
                                            EquipmentDatabase.Equipment prevEquipment = (EquipmentDatabase.Equipment)SetupManager.instance.curProgressData.endlessRunData.curBodyEquipmentIndex;
                                            EquipmentDatabase.EquipmentStats prevEquipmentStats = EquipmentDatabase.instance.GetEquipmentData(prevEquipment).stats;
                                            SetupManager.instance.AddPlayerMaxHealth(prevEquipmentStats.maxHealthAdd);
                                            SetupManager.instance.AddPlayerMaxFire(prevEquipmentStats.maxFireAdd);
                                        }
                                        SetupManager.instance.curProgressData.endlessRunData.curBodyEquipmentIndex = (int)curEquipment;
                                    }

                                    break;

                                case EquipmentDatabase.Slot.Bracelet:

                                    if (isNormalRun)
                                    {
                                        if (SetupManager.instance.curProgressData.normalRunData.curBraceletEquipmentIndex != -1)
                                        {
                                            EquipmentDatabase.Equipment prevEquipment = (EquipmentDatabase.Equipment)SetupManager.instance.curProgressData.normalRunData.curBraceletEquipmentIndex;
                                            EquipmentDatabase.EquipmentStats prevEquipmentStats = EquipmentDatabase.instance.GetEquipmentData(prevEquipment).stats;
                                            SetupManager.instance.AddPlayerMaxHealth(prevEquipmentStats.maxHealthAdd);
                                            SetupManager.instance.AddPlayerMaxFire(prevEquipmentStats.maxFireAdd);
                                        }
                                        SetupManager.instance.curProgressData.normalRunData.curBraceletEquipmentIndex = (int)curEquipment;
                                    }
                                    else
                                    {
                                        if (SetupManager.instance.curProgressData.endlessRunData.curBraceletEquipmentIndex != -1)
                                        {
                                            EquipmentDatabase.Equipment prevEquipment = (EquipmentDatabase.Equipment)SetupManager.instance.curProgressData.endlessRunData.curBraceletEquipmentIndex;
                                            EquipmentDatabase.EquipmentStats prevEquipmentStats = EquipmentDatabase.instance.GetEquipmentData(prevEquipment).stats;
                                            SetupManager.instance.AddPlayerMaxHealth(prevEquipmentStats.maxHealthAdd);
                                            SetupManager.instance.AddPlayerMaxFire(prevEquipmentStats.maxFireAdd);
                                        }
                                        SetupManager.instance.curProgressData.endlessRunData.curBraceletEquipmentIndex = (int)curEquipment;
                                    }

                                    break;

                                case EquipmentDatabase.Slot.Ring:

                                    if (isNormalRun)
                                    {
                                        if (SetupManager.instance.curProgressData.normalRunData.curRingEquipmentIndex != -1)
                                        {
                                            EquipmentDatabase.Equipment prevEquipment = (EquipmentDatabase.Equipment)SetupManager.instance.curProgressData.normalRunData.curRingEquipmentIndex;
                                            EquipmentDatabase.EquipmentStats prevEquipmentStats = EquipmentDatabase.instance.GetEquipmentData(prevEquipment).stats;
                                            SetupManager.instance.AddPlayerMaxHealth(prevEquipmentStats.maxHealthAdd);
                                            SetupManager.instance.AddPlayerMaxFire(prevEquipmentStats.maxFireAdd);
                                        }
                                        SetupManager.instance.curProgressData.normalRunData.curRingEquipmentIndex = (int)curEquipment;
                                    }
                                    else
                                    {
                                        if (SetupManager.instance.curProgressData.endlessRunData.curRingEquipmentIndex != -1)
                                        {
                                            EquipmentDatabase.Equipment prevEquipment = (EquipmentDatabase.Equipment)SetupManager.instance.curProgressData.endlessRunData.curRingEquipmentIndex;
                                            EquipmentDatabase.EquipmentStats prevEquipmentStats = EquipmentDatabase.instance.GetEquipmentData(prevEquipment).stats;
                                            SetupManager.instance.AddPlayerMaxHealth(prevEquipmentStats.maxHealthAdd);
                                            SetupManager.instance.AddPlayerMaxFire(prevEquipmentStats.maxFireAdd);
                                        }
                                        SetupManager.instance.curProgressData.endlessRunData.curRingEquipmentIndex = (int)curEquipment;
                                    }

                                    break;

                                case EquipmentDatabase.Slot.Weapon:

                                    if (isNormalRun)
                                    {
                                        if (SetupManager.instance.curProgressData.normalRunData.curWeaponEquipmentIndex != -1)
                                        {
                                            EquipmentDatabase.Equipment prevEquipment = (EquipmentDatabase.Equipment)SetupManager.instance.curProgressData.normalRunData.curWeaponEquipmentIndex;
                                            EquipmentDatabase.EquipmentStats prevEquipmentStats = EquipmentDatabase.instance.GetEquipmentData(prevEquipment).stats;
                                            SetupManager.instance.AddPlayerMaxHealth(prevEquipmentStats.maxHealthAdd);
                                            SetupManager.instance.AddPlayerMaxFire(prevEquipmentStats.maxFireAdd);
                                        }
                                        SetupManager.instance.curProgressData.normalRunData.curWeaponEquipmentIndex = (int)curEquipment;
                                    }
                                    else
                                    {
                                        if (SetupManager.instance.curProgressData.endlessRunData.curWeaponEquipmentIndex != -1)
                                        {
                                            EquipmentDatabase.Equipment prevEquipment = (EquipmentDatabase.Equipment)SetupManager.instance.curProgressData.endlessRunData.curWeaponEquipmentIndex;
                                            EquipmentDatabase.EquipmentStats prevEquipmentStats = EquipmentDatabase.instance.GetEquipmentData(prevEquipment).stats;
                                            SetupManager.instance.AddPlayerMaxHealth(prevEquipmentStats.maxHealthAdd);
                                            SetupManager.instance.AddPlayerMaxFire(prevEquipmentStats.maxFireAdd);
                                        }
                                        SetupManager.instance.curProgressData.endlessRunData.curWeaponEquipmentIndex = (int)curEquipment;
                                    }

                                    break;
                            }

                            // gold blade achievement
                            if ( curEquipment == EquipmentDatabase.Equipment.GoldBlade )
                            {
                                AchievementHelper.UnlockAchievement("ACHIEVEMENT_GOLDBLADE");
                            }

                            // increasing health of fire?
                            SetupManager.instance.AddPlayerMaxHealth(curEquipmentStatas.maxHealthAdd);
                            SetupManager.instance.AddPlayerMaxFire(curEquipmentStatas.maxFireAdd);

                            // update run data
                            SetupManager.instance.UpdateRunDataRead();

                            HandManager.instance.EquipItem(curEquipment);
                            GameManager.instance.curInteractionScript.ClearEquipment();

                            GameManager.instance.AddCoinAmount(-coinCostUse);

                            // update player equipment
                            SetupManager.instance.UpdatePlayerEquipment();

                            // close shop window
                            LeaveShopBrowse();

                            // audio
                            AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.shopBuyItemClips),.7f,.9f,.3f,.325f);
                            SetupManager.instance.PlayUISelectSound();
                        }
                        else
                        {
                            // speler heeft niet genoeg munten! jammer!!!
                        }
                    }
                }

                // cancel?
                if ( InputManager.instance.cancelPressed )
                {
                    GameManager.instance.curInteractionScript.triggered = false;
                    LeaveShopBrowse();

                    // audio
                    SetupManager.instance.PlayUIBackSound();
                }
            }
        }
    }

    //void EquipItem ( EquipmentDatabase.Equipment _equipment )
    //{
    //    switch ( SetupManager.instance.curRunType )
    //    {
    //        case SetupManager.RunType.Normal:

    //            break;

    //        case SetupManager.RunType.Endless:

    //            break;
    //    }
    //}

    void ClearAllItemBrowses ()
    {
        for (int i = shopBrowseItemObjects.Count - 1; i >= 0; i--)
        {
            Destroy(shopBrowseItemObjects[i]);
        }
        shopBrowseItemObjects.Clear();
        shopBrowseItemRectTransforms.Clear();
        shopBrowseItemImages.Clear();
        shopBrowseItemBackImages.Clear();
        shopBrowseItemTypeImages.Clear();
        shopBrowseItemImages.Clear();
        shopBrowseItemInfosLeft.Clear();
        shopBrowseItemInfosRight.Clear();
    }

    public void InitProceedToNextLevel()
    {
        SetupManager.instance.SetTransition(SetupManager.TransitionMode.In);
        Invoke("ProceedToNextLevel",SetupManager.instance.sceneLoadWait);
        leaving = true;

        // save
        switch ( SetupManager.instance.curRunType )
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

    void ProceedToNextLevel()
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
