using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // position
    Vector3 localPosOriginal, localPosTarget, localPosCur;

    // fire
    [Header("fire")]
    public Transform fireTransform;
    Vector3 firePosOriginal, firePosHide, firePosShow, firePosTarget, firePosCur;

    // layerMasks
    [Header("layerMasks")]
    public LayerMask interactionLayerMask;

    void Start ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // position
        localPosOriginal = myTransform.localPosition;
        localPosTarget = localPosOriginal;
        localPosCur = localPosTarget;

        // fire
        firePosOriginal = fireTransform.localPosition;
        firePosShow = firePosOriginal;
        firePosHide = firePosShow;
        firePosHide.y -= 1f;
        firePosTarget = firePosHide;
        firePosCur = firePosTarget;
        fireTransform.localPosition = firePosCur;
    }

    void Update ()
    {
        // check for interactions
        CheckForInteractions();

        // position
        if ( !SetupManager.instance.inFreeze )
        {
            localPosTarget = localPosOriginal;
            if ( SetupManager.instance.inScreenShake )
            {
                float offMax = .125f;
                localPosTarget.x += TommieRandom.instance.RandomRange(-offMax,offMax);
                localPosTarget.y += TommieRandom.instance.RandomRange(-offMax,offMax);
                localPosTarget.z += TommieRandom.instance.RandomRange(-offMax,offMax);
            }
            localPosCur = Vector3.Lerp(localPosCur,localPosTarget,20f * Time.deltaTime);
            myTransform.localPosition = localPosCur;

            // fire
            if ( fireTransform != null )
            {
                firePosTarget = firePosOriginal;
                firePosTarget.y -= ((1f - SetupManager.instance.playerBurningFac) * .05f);
                firePosCur = firePosTarget;
                fireTransform.localPosition = firePosCur;
            }
        }
    }

    void CheckForInteractions ()
    {
        float interactCheckDst = 2.5f;
        Vector3 c0 = GameManager.instance.mainCameraTransform.position;
        Vector3 c1 = c0 + (GameManager.instance.mainCameraTransform.forward * interactCheckDst);
        RaycastHit cHit;
        if (Physics.Linecast(c0,c1, out cHit, interactionLayerMask))
        {
            Transform cHitTr = cHit.transform;
            if (cHitTr.GetComponent<InteractionScript>() != null)
            {
                InteractionScript interactionCheckScript = cHitTr.GetComponent<InteractionScript>();
                if (interactionCheckScript != null)
                {
                    if (!interactionCheckScript.triggered && GameManager.instance != null )
                    {
                        GameManager.instance.SetInteractionType(interactionCheckScript.myInteractionType,interactionCheckScript);
                    }
                }
            }
        }
    }
}
