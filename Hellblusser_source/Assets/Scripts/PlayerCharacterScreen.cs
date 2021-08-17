using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacterScreen : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // graphics
    [Header("graphics")]
    public MeshRenderer bodyMeshRenderer;
    public MeshRenderer cloakMeshRenderer;
    public Transform armContainer;
    public List<Transform> armTransforms;
    public List<LineRenderer> armLines;
    public List<MeshRenderer> sleeveMeshRenderers;
    public Transform headBaseTransform;

    // references
    [Header("references")]
    public Transform bodyTransform;
    public Transform braceletRefTransform;
    public Transform ringRefTransform;
    public Transform weaponRefTransform;

    // animation
    public enum State { Sit, Dead, Stand, Kneel };
    public State curState;

    // eyes
    public enum EyeState { Normal, Blink, Dead };
    public EyeState curEyeState;
    public bool forceDeadEyes;

    [Header("eyes")]
    public GameObject[] eyeNormalObjects;
    public GameObject[] eyeBlinkObjects;
    public GameObject[] eyeDeadObjects;

    // feet
    [Header("feet")]
    public Transform[] feetTransforms;

    // legs
    [Header("legs")]
    public Transform legContainer;
    public LineRenderer[] legLines;
    public Transform[] legTransforms;

    // hands
    [Header("hands")]
    public List<Transform> handTransforms;
    public List<MeshFilter> handMeshFilters;
    public Mesh[] handMeshDead;
    public Mesh[] handMeshStand;

    // points
    List<Vector3[]> legPoints;
    List<Vector3[]> armPoints;

    // mouth
    [Header("mouth")]
    public GameObject mouthNormalObject;
    public GameObject mouthDeadObject;

    // robe
    [Header("robe")]
    public Transform robeTransform;
    public MeshFilter robeMeshFilter;
    public Mesh robeMeshSit;
    public Mesh robeMeshDead;
    public Mesh robeMeshStand;
    public Mesh robeMeshKneel;
    public Transform[] robeArmTransforms;

    // blink
    int blinkHoldDur, blinkHoldCounter, blinkRateMin, blinkRateMax, blinkRate, blinkRateCounter, blinkIndex;

    // equipment
    [HideInInspector] public Transform braceletEquippedTransform;
    [HideInInspector] public Transform ringEquippedTransform;
    [HideInInspector] public Transform weaponEquippedTransform;

    [HideInInspector] public GameObject braceletEquippedObject;
    [HideInInspector] public GameObject ringEquippedObject;
    [HideInInspector] public GameObject weaponEquippedObject;

    // data
    int bodyEquipmentIndex;
    int braceletEquipmentIndex;
    int ringEquipmentIndex;
    int weaponEquipmentIndex;

    void Start ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // eyes
        if (SetupManager.instance.curGameState == SetupManager.GameState.GameOver || forceDeadEyes)
        {
            SetEyeState(EyeState.Dead);
        }
        else
        {
            SetEyeState(EyeState.Normal);
        }

        // blink
        blinkIndex = 0;
        blinkHoldDur = 6;
        blinkHoldCounter = 0;

        blinkRateMin = 120;
        blinkRateMax = 360;
        blinkRate = Mathf.RoundToInt(TommieRandom.instance.RandomRange(blinkRateMin,blinkRateMax));
        blinkRateCounter = 0;

        // points
        legPoints = new List<Vector3[]>();
        armPoints = new List<Vector3[]>();
        for ( int i = 0; i < 2; i ++ )
        {
            legPoints.Add(new Vector3[3]);
            armPoints.Add(new Vector3[3]);
        }

        // create equipment
        CreateEquipment();
    }

    void Update ()
    {
        //if (!SetupManager.instance.inFreeze)
        {
            // update eyes
            if (!SetupManager.instance.inFreeze)
            {
                if (SetupManager.instance.curGameState == SetupManager.GameState.GameOver || forceDeadEyes)
                {
                    SetEyeState(EyeState.Dead);
                }
                else
                {
                    if (blinkIndex == 0)
                    {
                        if (blinkRateCounter < blinkRate)
                        {
                            blinkRateCounter++;
                        }
                        else
                        {
                            blinkHoldCounter = 0;
                            blinkIndex = 1;
                        }
                    }
                    else
                    {
                        if (blinkHoldCounter < blinkHoldDur)
                        {
                            blinkHoldCounter++;
                        }
                        else
                        {
                            blinkRateCounter = 0;
                            blinkRate = Mathf.RoundToInt(TommieRandom.instance.RandomRange(blinkRateMin, blinkRateMax));
                            blinkIndex = 0;
                        }
                    }

                    switch (blinkIndex)
                    {
                        case 0: SetEyeState(EyeState.Normal); break;
                        case 1: SetEyeState(EyeState.Blink); break;
                    }
                }
            }

            // update animation
            if (curState != State.Sit)
            {
                UpdateAnimation();
            }

            // update equipment
            UpdateEquipment();
        }
    }

    void UpdateAnimation ()
    {
        // legs
        UpdateLegs();

        // arms
        UpdateArms();

        // robe
        UpdateRobe();

        // update robe arms
        UpdateRobeArms();

        // update hands
        UpdateHands();

        // update head
        UpdateHead();
    }

    void UpdateLegs ()
    {
        for ( int i = 0; i < legLines.Length; i ++ )
        {
            float legDir = (i == 0) ? 1f : -1f;

            Vector3 pStart = Vector3.zero;
            Vector3 pEnd = pStart;
            Vector3 pMid = Vector3.zero;

            switch ( curState )
            {
                case State.Dead:

                    legTransforms[i].localRotation = Quaternion.Euler(0f, 180f, 0f);

                    pStart = Vector3.zero;
                    pEnd = new Vector3(0f,-.475f,0f);
                    pEnd.x += (.05f * legDir);
                    pMid = BasicFunctions.LerpByDistance(pStart,pEnd,Vector3.Distance(pStart,pEnd) * .5f);
                    pMid.z += .025f;

                    break;

                case State.Kneel:

                    legTransforms[i].localRotation = Quaternion.Euler(0f, 180f, 0f);

                    pStart = Vector3.zero;
                    pEnd = new Vector3(0f, -.25f, 0f);
                    pEnd.x += (.05f * legDir);
                    pEnd.z -= .175f;
                    pMid = BasicFunctions.LerpByDistance(pStart, pEnd, Vector3.Distance(pStart, pEnd) * .5f);
                    pMid.x += (.05f * legDir);
                    pMid.z += .375f;
                    pMid.y -= .125f;

                    if (feetTransforms != null)
                    {
                        Vector3 footP0 = legPoints[i][1];
                        Vector3 footP1 = legPoints[i][2];
                        Vector3 footDir = (footP1 - footP0).normalized;
                        footP1.y -= .075f;
                        feetTransforms[i].localPosition = footP1;

                        Quaternion footR = Quaternion.LookRotation(footDir) * Quaternion.Euler(90f, 180f, 0f);
                        feetTransforms[i].localRotation = footR;
                    }

                    break;
            }

            legPoints[i][0] = pStart;
            legPoints[i][1] = pMid;
            legPoints[i][2] = pEnd;
            legLines[i].SetPositions(legPoints[i]);
        }
    }

    void UpdateArms ()
    {
        for (int i = 0; i < armLines.Count; i++)
        {
            float armDir = (i == 0) ? 1f : -1f;

            Vector3 pStart = Vector3.zero;
            Vector3 pEnd = pStart;
            Vector3 pMid = Vector3.zero;

            switch (curState)
            {
                case State.Dead:

                    armTransforms[i].localRotation = Quaternion.Euler(0f,180f,0f);

                    pStart = Vector3.zero;
                    pEnd = new Vector3(.175f * armDir, -.325f, 0f);
                    pMid = BasicFunctions.LerpByDistance(pStart, pEnd, Vector3.Distance(pStart, pEnd) * .5f);
                    pMid.x += (.025f * armDir);
                    pMid.y += .025f;

                    break;

                case State.Kneel:

                    armTransforms[i].localRotation = Quaternion.Euler(0f, 180f, 0f);

                    pStart = Vector3.zero;
                    //pEnd = new Vector3(.15f * armDir, -.325f, 0f);
                    pEnd = legPoints[i][1];
                    pEnd.y -= .25f;

                    pMid = BasicFunctions.LerpByDistance(pStart, pEnd, Vector3.Distance(pStart, pEnd) * .5f);
                    pMid.x += (.025f * armDir);
                    pMid.y += .025f;

                    break;
            }

            armPoints[i][0] = pStart;
            armPoints[i][1] = pMid;
            armPoints[i][2] = pEnd;
            armLines[i].SetPositions(armPoints[i]);
        }
    }

    void UpdateRobe ()
    {
        if (robeMeshFilter != null)
        {
            switch (curState)
            {
                case State.Sit:
                    robeTransform.localRotation = Quaternion.Euler(0f,0f,7f);
                    robeMeshFilter.mesh = robeMeshSit;
                    break;
                case State.Dead:
                    robeTransform.localRotation = Quaternion.identity;
                    robeMeshFilter.mesh = robeMeshDead;
                    break;
                case State.Stand:
                    robeTransform.localRotation = Quaternion.identity;
                    robeMeshFilter.mesh = robeMeshStand;
                    break;
                case State.Kneel:
                    robeTransform.localRotation = Quaternion.identity;
                    robeMeshFilter.mesh = robeMeshKneel;
                    break;
            }
        }
    }

    void UpdateRobeArms ()
    {
        for ( int i = 0; i < armLines.Count; i ++)
        {
            Vector3 p0 = armTransforms[i].TransformPoint(armPoints[i][1]);
            Vector3 p1 = armTransforms[i].TransformPoint(armPoints[i][2]);
            Vector3 p2 = (p1 - p0).normalized;
            robeArmTransforms[i].position = p1 + (p2 * -.0175f);

            Quaternion r0 = Quaternion.LookRotation(p2) * Quaternion.Euler(0f,0f,180f);
            robeArmTransforms[i].rotation = r0;
        }
    }

    void UpdateHands ()
    {
        for ( int i = 0; i < handMeshFilters.Count; i ++ )
        {
            switch ( curState )
            {
                case State.Dead: handMeshFilters[i].mesh = handMeshDead[i]; break;
                case State.Stand: handMeshFilters[i].mesh = handMeshStand[i]; break;
            }
        }
    }

    void UpdateHead ()
    {
        switch ( curState )
        {
            case State.Dead: headBaseTransform.localRotation = Quaternion.Euler(-15f,20f,-5f); break;

            case State.Kneel:

                Quaternion targetRot = Quaternion.Euler(20f,0f,0f);

                if ( (OutroManager.instance.shotIndex == 1 && OutroManager.instance.shotCounter >= (OutroManager.instance.shotWait * .5f)) )
                {
                    targetRot = Quaternion.Euler(-30f,0f,0f);
                }

                if ( OutroManager.instance.shotIndex > 1 )
                {
                    targetRot = Quaternion.Euler(-30f, 0f, 0f);
                }

                headBaseTransform.localRotation = Quaternion.Lerp(headBaseTransform.localRotation,targetRot,2.5f * Time.deltaTime);

                break;
        }
    }

    public void CreateEquipment ()
    {
        //Debug.Log("create equipment for run type: " + SetupManager.instance.curRunType + " || " + Time.time.ToString());

        SetupManager.instance.UpdateRunDataRead();

        Vector3 spawnPoint = new Vector3(0f,-1000f,0f);

        bodyEquipmentIndex = SetupManager.instance.runDataRead.curBodyEquipmentIndex;
        braceletEquipmentIndex = SetupManager.instance.runDataRead.curBraceletEquipmentIndex;
        ringEquipmentIndex = SetupManager.instance.runDataRead.curRingEquipmentIndex;
        weaponEquipmentIndex = SetupManager.instance.runDataRead.curWeaponEquipmentIndex;

        // log
        //Debug.Log("create equipment || body: " + bodyEquipmentIndex.ToString() + " || weapon: " + weaponEquipmentIndex.ToString() + " || ring: " + ringEquipmentIndex.ToString() + " || bracelet: " + braceletEquipmentIndex.ToString() + " || " + Time.time.ToString());

        // bracelet
        if (braceletEquippedObject != null)
        {
            Destroy(braceletEquippedObject);
        }
        if (braceletEquipmentIndex != -1)
        {
            EquipmentDatabase.Equipment braceletEquipment = (EquipmentDatabase.Equipment)braceletEquipmentIndex;
            EquipmentDatabase.EquipmentData braceletEquipmentData = EquipmentDatabase.instance.GetEquipmentData(braceletEquipment);

            float braceletScl = .125f;

            GameObject braceletO = PrefabManager.instance.SpawnPrefabAsGameObject(braceletEquipmentData.visualsObject, spawnPoint, Quaternion.identity, braceletScl);
            braceletEquippedTransform = braceletO.transform;
            braceletEquippedObject = braceletO;

            //Debug.Log("bracelet create: " + braceletEquipment + " || " + Time.time.ToString());
        }

        // ring
        if (ringEquippedObject != null)
        {
            Destroy(ringEquippedObject);
        }
        if (ringEquipmentIndex != -1)
        {
            EquipmentDatabase.Equipment ringEquipment = (EquipmentDatabase.Equipment)ringEquipmentIndex;
            EquipmentDatabase.EquipmentData ringEquipmentData = EquipmentDatabase.instance.GetEquipmentData(ringEquipment);

            float ringScl = .125f;

            GameObject ringO = PrefabManager.instance.SpawnPrefabAsGameObject(ringEquipmentData.visualsObject, spawnPoint, Quaternion.identity, ringScl);
            ringEquippedTransform = ringO.transform;
            ringEquippedObject = ringO;
        }

        // weapon
        if (weaponEquippedObject != null)
        {
            Destroy(weaponEquippedObject);
        }
        if (weaponEquipmentIndex != -1)
        {
            EquipmentDatabase.Equipment weaponEquipment = (EquipmentDatabase.Equipment)weaponEquipmentIndex;
            EquipmentDatabase.EquipmentData weaponEquipmentData = EquipmentDatabase.instance.GetEquipmentData(weaponEquipment);

            float weaponScl = 1f;

            GameObject weaponO = PrefabManager.instance.SpawnPrefabAsGameObject(weaponEquipmentData.visualsObject, spawnPoint, Quaternion.identity, weaponScl);
            weaponEquippedTransform = weaponO.transform;
            weaponEquippedObject = weaponO;
        }

        // body
        if ( bodyEquipmentIndex != -1 )
        {
            EquipmentDatabase.Equipment bodyEquipment = (EquipmentDatabase.Equipment)(bodyEquipmentIndex);
            EquipmentDatabase.EquipmentData bodyEquipmentData = EquipmentDatabase.instance.GetEquipmentData(bodyEquipment);

            if ( bodyMeshRenderer != null )
            {
                Material[] bodyMats = bodyMeshRenderer.materials;
                bodyMats[0] = bodyEquipmentData.matC;
                bodyMeshRenderer.materials = bodyMats;
            }

            if ( cloakMeshRenderer != null )
            {
                Material[] cloakMats = cloakMeshRenderer.materials;
                cloakMats[0] = bodyEquipmentData.matA;
                cloakMats[1] = bodyEquipmentData.matB;
                cloakMeshRenderer.materials = cloakMats;
            }

            if ( armLines != null && armLines.Count > 0 )
            {
                for ( int i = 0; i < armLines.Count; i ++ )
                {
                    Material[] armMats = armLines[i].materials;
                    armMats[0] = bodyEquipmentData.matC;
                    armLines[i].materials = armMats;
                }
            }

            if (sleeveMeshRenderers != null && sleeveMeshRenderers.Count > 0)
            {
                for (int i = 0; i < sleeveMeshRenderers.Count; i++)
                {
                    Material[] sleeveMats = sleeveMeshRenderers[i].materials;
                    sleeveMats[0] = bodyEquipmentData.matA;
                    sleeveMats[2] = bodyEquipmentData.matB;
                    sleeveMeshRenderers[i].materials = sleeveMats;
                }
            }

            //Debug.Log("new body equipment: " + bodyEquipment + " || " + Time.time.ToString());
        }
    }

    void UpdateEquipment ()
    {
        // bracelet
        if ( braceletEquippedTransform != null )
        {
            braceletEquippedTransform.position = braceletRefTransform.position;
            braceletEquippedTransform.rotation = braceletRefTransform.rotation;
            braceletEquippedTransform.localScale = braceletRefTransform.localScale;
        }

        // ring
        if (ringEquippedTransform != null)
        {
            ringEquippedTransform.position = ringRefTransform.position;
            ringEquippedTransform.rotation = ringRefTransform.rotation;
            ringEquippedTransform.localScale = ringRefTransform.localScale;
        }

        // weapon
        if (weaponEquippedTransform != null)
        {
            weaponEquippedTransform.position = weaponRefTransform.position;
            weaponEquippedTransform.rotation = weaponRefTransform.rotation;
            weaponEquippedTransform.localScale = weaponRefTransform.localScale;
        }
    }

    public void SetEyeState ( EyeState _to )
    {
        for (int i = 0; i < 2; i++)
        {
            eyeNormalObjects[i].SetActive(_to == EyeState.Normal);
            eyeBlinkObjects[i].SetActive(_to == EyeState.Blink);
            eyeDeadObjects[i].SetActive(_to == EyeState.Dead);
        }

        mouthNormalObject.SetActive(_to != EyeState.Dead);
        mouthDeadObject.SetActive(_to == EyeState.Dead);

        curEyeState = _to;
    }
}
