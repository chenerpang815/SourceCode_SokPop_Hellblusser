using UnityEngine;

public class DoorScript : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // objects
    [Header("objects")]
    public GameObject closedObject;
    public GameObject openObject;

    // type
    public enum Type { Start, End };
    public Type myType;

    // state
    public enum State { Closed, Open };
    public State curState;

    void Start ()
    {
        myTransform = transform;
        myGameObject = gameObject;

        // closed on start
        SetState(State.Closed);
    }

    void Update ()
    {
        // check if player reached end
        if (curState == State.Open && myType == Type.End && (GameManager.instance.playerHasKey || SetupManager.instance.curEncounterType == SetupManager.EncounterType.Small) && !SetupManager.instance.runDataRead.playerReachedEnd)
        {
            CheckIfPlayerReachedEnd();
        }
    }

    void CheckIfPlayerReachedEnd ()
    {
        Vector3 p0 = myTransform.position;
        Vector3 p1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
        p1.y = p0.y;
        Vector3 pA = (p1 - p0).normalized;

        float enterThreshold = 1f;
        float d0 = Vector3.Distance(p0,p1);
        if (d0 <= enterThreshold)
        {
            Vector3 pB = myTransform.right;
            float d1 = Vector3.Dot(pA, pB);
            if (d1 > 0f)
            {
                GameManager.instance.PlayerReachedEnd(true);
            }
        }

        // log
        //Debug.Log("dot: " + d0.ToString() + " || " + Time.time.ToString());
    }

    public void SetState ( State _to )
    {
        curState = _to;

        switch (curState)
        {
            case State.Closed:
                closedObject.SetActive(true);
                openObject.SetActive(false);

                // audio
                PlayDoorClosedSound();
                break;
            case State.Open:
                closedObject.SetActive(false);
                openObject.SetActive(true);

                // open
                PlayDoorOpenSound();
                break;
        }
    }

    public void PlayDoorClosedSound ()
    {
        AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.doorClosed), .6f, .9f, .3f, .325f);
    }

    public void PlayDoorOpenSound ()
    {
        AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.doorOpen), .6f, .9f, .3f, .325f);
    }
}
