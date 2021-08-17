using UnityEngine;

public class FountainScript : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // objects
    [Header("objects")]
    public GameObject emptyObject;
    public GameObject filledObject;
    public GameObject destroyedObject;

    // state
    public enum State { Empty, Filled };
    [HideInInspector] public State curState;
    [HideInInspector] public bool usedFountain;
    [HideInInspector] public bool broken;

    void Awake ()
    {
        // store?
        if (FountainManager.instance != null)
        {
            FountainManager.instance.fountainScript = this;
        }
    }

    void Start ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // state
        usedFountain = false;
        if (!broken)
        {
            SetState(State.Empty);
        }
    }

    public void Fill ()
    {
        // create blessing pick screen
        FountainManager.instance.CreateBlessingOptions();

        // remove tears
        GameManager.instance.AddTearAmount(-SetupManager.instance.blessingTearCost);

        // state
        SetState(State.Filled);

        // audio
        AudioManager.instance.PlaySoundAtPosition(myTransform.position + (Vector3.up * .5f),BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.fountainFillClips),.7f,.9f,.3f,.325f);

        // done
        usedFountain = true;
    }

    public void Break ()
    {
        Transform trUse = (myTransform == null) ? transform : myTransform;
        if ( !broken )
        {
            // particles
            PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab,trUse.position + (Vector3.up * .675f),Quaternion.identity,2f);

            // show or hide certain objects
            emptyObject.SetActive(false);
            filledObject.SetActive(false);
            destroyedObject.SetActive(true);

            // audio
            AudioManager.instance.PlaySoundAtPosition(trUse.position + (Vector3.up * .5f), BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.stoneBreakClips), .7f, .9f, .3f, .325f);

            // done, nice
            broken = true;
        }
    }

    void SetState ( State _to )
    {
        curState = _to;
        switch (_to)
        {
            case State.Empty:
                emptyObject.SetActive(true);
                filledObject.SetActive(false);
                destroyedObject.SetActive(false);
                break;
            case State.Filled:
                emptyObject.SetActive(false);
                filledObject.SetActive(true);
                destroyedObject.SetActive(false);
                break;
        }
    }
}
