using UnityEngine;

public class InteractionScript : MonoBehaviour
{
    public GameManager.InteractionType myInteractionType;
    [HideInInspector] public bool triggered;

    // references
    [Header("references")]
    public FountainScript myFountainScript;
    public DoorScript myDoorScript;
    public EquipmentDatabase.EquipmentData myEquipmentData;
    public EquipmentDatabase.Equipment myEquipment;
    public GameObject myEquipmentObject;

    void Start()
    {
        if (!triggered)
        {
            if (myInteractionType == GameManager.InteractionType.Fountain && SetupManager.instance.runDataRead.blessingsClaimed.Count >= BlessingDatabase.instance.blessingCountMax)
            {
                if (myFountainScript != null)
                {
                    myFountainScript.Break();
                }
                triggered = true;
            }
        }
    }

    public void Trigger ()
    {
        if (!triggered)
        {
            // level end door
            if (myInteractionType == GameManager.InteractionType.LevelEndDoor && myDoorScript != null )
            {
                myDoorScript.SetState(DoorScript.State.Open);
            }

            // fountain door
            if (myInteractionType == GameManager.InteractionType.FountainDoor && FountainManager.instance != null )
            {
                AudioManager.instance.PlayDoorOpenSound();
                FountainManager.instance.InitProceedToNextLevel();
            }

            // shop door
            if (myInteractionType == GameManager.InteractionType.ShopDoor && ShopManager.instance != null)
            {
                AudioManager.instance.PlayDoorOpenSound();
                ShopManager.instance.InitProceedToNextLevel();
            }

            // fountain
            if (myFountainScript != null)
            {
                myFountainScript.Fill();
            }

            // shop item
            if ( myInteractionType == GameManager.InteractionType.ShopItem )
            {
                if ( ShopManager.instance != null && !ShopManager.instance.inItemBrowse )
                {
                    ShopManager.instance.InitShopBrowse(this);
                }
            }

            // rest door
            if (myInteractionType == GameManager.InteractionType.RestDoor && RestManager.instance != null)
            {
                AudioManager.instance.PlayDoorOpenSound();
                RestManager.instance.InitProceedToNextLevel();
            }

            // rest campfire
            if (myInteractionType == GameManager.InteractionType.RestCampfire && RestManager.instance != null)
            {
                //AudioManager.instance.PlayDoorOpenSound();
                //RestManager.instance.InitProceedToNextLevel();

                //SetupManager.instance.curProgressData.playerHealthCur = SetupManager.instance.curProgressData.playerHealthMax;
                int healthDiff = (SetupManager.instance.runDataRead.playerHealthMax - SetupManager.instance.runDataRead.playerHealthCur);
                if ( healthDiff < 0 )
                {
                    healthDiff = 0;
                }
                GameManager.instance.AddPlayerHealth(healthDiff);
                RestManager.instance.didRest = true;
            }

            // done
            triggered = true;
        }
    }

    public void ClearEquipment ()
    {
        if ( myEquipmentObject != null )
        {
            Destroy(myEquipmentObject);
        }
    }
}
