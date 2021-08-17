using UnityEngine;
using System.Collections.Generic;

public class GateScript : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // state
    public enum State { Open, Closed };
    public State curState;
    [HideInInspector] public bool allNpcsDefeated;

    // moving part
    public Transform movingPartTransform;
    public BoxCollider gateCollider;
    Vector3 movingPartPosClosed, movingPartPosOpen, movingPartPosTarget, movingPartPosCur;

    // npcs
    [Header("npcs")]
    public List<NpcSpawner> myNpcSpawners;

    void Start()
    {
        myTransform = transform;
        myGameObject = gameObject;

        movingPartPosClosed = movingPartTransform.localPosition;
        movingPartPosOpen = movingPartPosClosed;
        movingPartPosOpen.y += 1f;
        movingPartPosTarget = movingPartPosOpen;
        movingPartPosCur = movingPartPosTarget;
        movingPartTransform.localPosition = movingPartPosCur;
        SetState(State.Open);
    }

    void Update ()
    {
        if (!SetupManager.instance.inFreeze)
        {
            // movement
            float gateMoveDir = (movingPartPosCur.y < movingPartPosTarget.y) ? 1f : -1f;
            float gateMoveThreshold = .025f;
            float gateMoveSpd = (2f * Time.deltaTime);
            if ( !BasicFunctions.IsApproximately(movingPartPosCur.y,movingPartPosTarget.y,gateMoveThreshold) )
            {
                movingPartPosCur.y += (gateMoveSpd * gateMoveDir);
            }
            else
            {
                movingPartPosCur.y = movingPartPosTarget.y;
            }
            movingPartTransform.localPosition = movingPartPosCur;

            // open?
            if ( curState == State.Closed && !SetupManager.instance.noNpcs )
            {
                allNpcsDefeated = true;
                if (myNpcSpawners != null && myNpcSpawners.Count > 0)
                {
                    for (int i = 0; i < myNpcSpawners.Count; i++)
                    {
                        if (!myNpcSpawners[i].CheckIfNpcsDefeated())
                        {
                            allNpcsDefeated = false;
                        }
                    }
                }
                if (allNpcsDefeated)
                {
                    SetState(State.Open);
                }
            }
        }
    }

    public void SetState ( State _to )
    {
        if (curState != _to)
        {
            switch (_to)
            {
                case State.Closed:
                    movingPartPosTarget = movingPartPosClosed;
                    gateCollider.enabled = true;

                    AudioManager.instance.PlaySoundAtPosition(myTransform.position,BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.gateClosed),.6f,.9f,.6f,.625f);
                    break;

                case State.Open:
                    movingPartPosTarget = movingPartPosOpen;
                    gateCollider.enabled = false;

                    AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.gateOpen), .6f, .9f, .6f, .625f);
                    break;
            }

            curState = _to;
        }
    }
}
