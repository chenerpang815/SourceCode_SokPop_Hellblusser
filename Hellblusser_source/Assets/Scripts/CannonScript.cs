using UnityEngine;

public class CannonScript : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // references
    [Header("references")]
    public Transform ropeLineRoot;
    public Transform shootRoot;
    public Transform baseTransform;
    public GameObject projectilePrefab;

    // materials
    [Header("materials")]
    public Material ropeMatNormal;
    public Material ropeMatLitA;
    public Material ropeMatLitB;
    int ropeLitFlickerIndex, ropeLitFlickerRate, ropeLitFlickerCounter;

    // fire charge
    [HideInInspector] public FireChargeScript myFireChargeScript;

    // animation
    int fireChargeIndex;
    float fireChargeProgress;
    int justShotDur, justShotCounter;
    Quaternion baseRotOriginal;

    // robe
    [HideInInspector] public LineRenderer ropeLine;
    [HideInInspector] public Vector3[] ropeLinePointsOriginal, ropeLinePointsCur;

    // state
    public enum State { Idle, Load, Shoot };
    public State curState;

    int loadWaitDur, loadWaitCounter;
    int shootWaitDur, shootWaitCounter;

    void Start ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // rope line
        GameObject ropeLineO = new GameObject("ropeO");
        Transform ropeLineTr = ropeLineO.transform;
        BasicFunctions.ResetTransform(ropeLineTr);

        LineRenderer ropeLr = ropeLineO.AddComponent<LineRenderer>();
        ropeLr.material = ropeMatNormal;

        ropeLr.positionCount = 5;
        ropeLr.useWorldSpace = true;

        float ropeW = .1f;
        ropeLr.startWidth = ropeW;
        ropeLr.endWidth = ropeW;

        ropeLr.numCapVertices = 3;

        ropeLine = ropeLr;

        float ropeSegmentLength = .325f;
        ropeLinePointsOriginal = new Vector3[ropeLr.positionCount];
        ropeLinePointsCur = new Vector3[ropeLr.positionCount];
        for ( int i = 0; i < ropeLine.positionCount; i ++ )
        {
            float iFloat = (float)(i);
            Vector3 p = ropeLineRoot.position + (Vector3.up * -(ropeSegmentLength * iFloat));

            ropeLinePointsOriginal[i] = p;
            ropeLinePointsCur[i] = p;
        }

        // fire charge script
        GameObject fireChargeO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.fireChargePrefab,myTransform.position,Quaternion.identity,1f);
        if (fireChargeO != null && fireChargeO.GetComponent<FireChargeScript>() != null)
        {
            myFireChargeScript = fireChargeO.GetComponent<FireChargeScript>();
            myFireChargeScript.scaleFacMultiplier = 14f;
        }

        // state
        SetState(State.Idle);

        // animation
        ropeLitFlickerIndex = 0;
        ropeLitFlickerRate = 8;
        ropeLitFlickerCounter = 0;

        if (baseTransform != null)
        {
            baseRotOriginal = baseTransform.localRotation;
        }

        // store
        Store();
    }

    void Update ()
    {
        if ( !SetupManager.instance.inFreeze )
        {
            if (justShotCounter < justShotDur)
            {
                justShotCounter++;
            }

            // update robe?
            UpdateRobe();

            // update base
            UpdateBase();

            // do things?
            switch ( curState )
            {
                case State.Idle: break;

                case State.Load:
                    if ( loadWaitCounter < loadWaitDur )
                    {
                        loadWaitCounter++;
                    }
                    else
                    {
                        SetState(State.Shoot);
                    }
                    break;

                case State.Shoot:
                    if ( shootWaitCounter < shootWaitDur )
                    {
                        shootWaitCounter++;
                    }
                    else
                    {
                        SetState(State.Idle);
                    }
                    break;
            }
        }
    }

    void UpdateRobe ()
    {
        if ( ropeLine != null )
        {
            for ( int i = 0; i < ropeLine.positionCount; i ++ )
            {
                ropeLinePointsCur[i] = ropeLinePointsOriginal[i];
            }
            ropeLine.SetPositions(ropeLinePointsCur);
        }

        switch ( curState )
        {
            case State.Idle:
                myFireChargeScript.myMeshRenderer.enabled = false;

                ropeLine.material = ropeMatNormal;
                
                break;

            case State.Load:
                myFireChargeScript.myMeshRenderer.enabled = true;

                // material
                if ( ropeLitFlickerCounter < ropeLitFlickerRate )
                {
                    ropeLitFlickerCounter++;
                }
                else
                {
                    ropeLitFlickerIndex = (ropeLitFlickerIndex == 0) ? 1 : 0;
                    ropeLitFlickerCounter = 0;
                }
                ropeLine.material = (ropeLitFlickerIndex == 0) ? ropeMatLitA : ropeMatLitB;

                // fire charge
                int ropeLength = (ropeLinePointsCur.Length - 1);
                if ( fireChargeIndex < ropeLength)
                {
                    fireChargeProgress += .05f;
                    if ( fireChargeProgress > 1f && fireChargeIndex < (ropeLength - 1) )
                    {
                        fireChargeProgress = 0f;
                        fireChargeIndex++;
                    }
                    fireChargeProgress = Mathf.Clamp(fireChargeProgress,0f,1f);
                }

                Vector3 pA = ropeLinePointsCur[ropeLength - fireChargeIndex];
                Vector3 pB = ropeLinePointsCur[(ropeLength - (fireChargeIndex + 1))];
                Vector3 pMid = BasicFunctions.LerpByDistance(pA,pB,Vector3.Distance(pA,pB) * fireChargeProgress);
                myFireChargeScript.myTransform.position = pMid;
                break;

            case State.Shoot:
                myFireChargeScript.myMeshRenderer.enabled = false;
                break;
        }
    }

    void UpdateBase ()
    {
        Quaternion rotOff = Quaternion.identity;
        float justShotP = BasicFunctions.ConvertRange(justShotCounter,0f,justShotDur,1f,0f);
        if (justShotP > .0125f)
        {
            float t0 = Time.time * 20f;
            float f0 = 30f * justShotP;
            float s0 = Mathf.Sin(t0) * f0;
            rotOff = Quaternion.Euler(s0,0f,0f);
        }

        Quaternion rotSet = baseRotOriginal * rotOff;
        baseTransform.localRotation = rotSet;
    }

    public void Lit ()
    {
        SetState(State.Load);
    }

    void InitIdle ()
    {

    }

    void InitLoad ()
    {
        loadWaitCounter = 0;
        loadWaitDur = 90;

        ropeLitFlickerIndex = 0;
        ropeLitFlickerCounter = 0;

        fireChargeProgress = 0f;
        fireChargeIndex = 0;

        // audio
        AudioManager.instance.PlaySoundAtPosition(ropeLinePointsCur[ropeLinePointsCur.Length - 1],BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.cannonLightClips),.8f,1f,.6f,.625f,6f,24f);
    }

    void InitShoot ()
    {
        shootWaitDur = 180;
        shootWaitCounter = 0;

        justShotDur = 30;
        justShotCounter = 0;

        // spawn projectile
        Vector3 projectileSpawnPos = shootRoot.position;

        Quaternion projectileSpawnRot = Quaternion.identity;
        GameObject projectileO = PrefabManager.instance.SpawnPrefabAsGameObject(projectilePrefab, projectileSpawnPos, projectileSpawnRot, .125f);
        ProjectileScript projectileScript = projectileO.GetComponent<ProjectileScript>();

        projectileScript.dir = baseTransform.forward;

        projectileScript.speed = 12f;
        projectileScript.radius = .75f;
        projectileScript.npcCoreBy = null;
        projectileScript.SetOwnerType(ProjectileScript.OwnerType.None);
        projectileScript.isFromCannon = true;

        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.magicImpactParticlesPrefab[0], projectileSpawnPos,Quaternion.identity,3f);

        // cannon shoot audio
        AudioManager.instance.PlaySoundAtPosition(projectileSpawnPos, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.cannonShootClips), .8f, 1f, .7f, .725f,6f,24f);
    }

    public void SetState ( State _to )
    {
        switch ( _to )
        {
            case State.Idle: InitIdle(); break;
            case State.Load: InitLoad(); break;
            case State.Shoot: InitShoot(); break;
        }
        curState = _to;
    }

    public void Store ()
    {
        if ( LevelGeneratorManager.instance != null && LevelGeneratorManager.instance.activeLevelGenerator != null )
        {
            LevelGeneratorManager.instance.activeLevelGenerator.allCannonScripts.Add(this);
        }
    }
}
