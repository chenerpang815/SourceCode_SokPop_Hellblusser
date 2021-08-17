using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [HideInInspector] public Transform myTransform;

    [Header("alernative transform")]
    public Transform alternativeTransform;

    [Header("rotation offset")]
    public Vector3 rotOffset;

    [Header("lock")]
    public bool lockX;
    public bool lockY;
    public bool lockZ;

    // special
    public bool isOutroMoon;
    Quaternion moonTurnAwayRot;

    public GameObject mouthNormalObject;
    public GameObject mouthHappyObject;

    public GameObject eyesNormalObject;
    public GameObject eyesHappyObject;

    public bool forceFacePlayer;

    // cam
    Camera camMain;
    Transform camMainTransform;

    void Start()
    {
        myTransform = transform;

        camMain = Camera.main;
        if ( camMain != null )
        {
            camMainTransform = camMain.transform;
        }

        moonTurnAwayRot = myTransform.localRotation * Quaternion.Euler(0f,180f,0f);
    }

    void Update ()
    {
        bool moonTurnAway = false;
        if ( isOutroMoon && (OutroManager.instance != null && OutroManager.instance.shotIndex >= 4) )
        {
            moonTurnAway = true;
        }
        if (!isOutroMoon || (isOutroMoon && !moonTurnAway) )
        {
            if (!forceFacePlayer)
            {
                bool mainCameraFound = (camMain != null && camMainTransform != null && camMainTransform && camMain.enabled);//(GameManager.instance != null && GameManager.instance.mainCamera != null && GameManager.instance.mainCamera.enabled);
                Transform trUse = (mainCameraFound) ? camMainTransform/*GameManager.instance.mainCameraTransform*/ : alternativeTransform;
                if (trUse != null)
                {
                    Vector3 r0 = myTransform.position;
                    Vector3 r1 = trUse.position;
                    if (lockX)
                    {
                        r1.y = r0.y;
                    }
                    Vector3 r2 = (r1 - r0).normalized;
                    Quaternion targetRot = Quaternion.LookRotation(r2) * Quaternion.Euler(rotOffset);

                    myTransform.rotation = targetRot;
                }
            }
            else
            {
                Transform trUse = GameManager.instance.mainCameraTransform;
                if (trUse != null)
                {
                    Vector3 r0 = myTransform.position;
                    Vector3 r1 = trUse.position;
                    if (lockX)
                    {
                        r1.y = r0.y;
                    }
                    Vector3 r2 = (r1 - r0).normalized;
                    Quaternion targetRot = Quaternion.LookRotation(r2) * Quaternion.Euler(rotOffset);

                    myTransform.rotation = targetRot;
                }
            }
        }

        if ( moonTurnAway )
        {
            //myTransform.rotation = Quaternion.Lerp(myTransform.rotation,moonTurnAwayRot,.75f * Time.deltaTime);
        }

        if ( isOutroMoon )
        {
            bool showHappyMouth = ((OutroManager.instance.shotIndex == 3 && OutroManager.instance.shotCounter >= (OutroManager.instance.shotWait * .5f)) || OutroManager.instance.shotIndex >= 4);
            mouthNormalObject.SetActive(!showHappyMouth);
            mouthHappyObject.SetActive(showHappyMouth);
            eyesNormalObject.SetActive(!showHappyMouth);
            eyesHappyObject.SetActive(showHappyMouth);
        }
    }
}
