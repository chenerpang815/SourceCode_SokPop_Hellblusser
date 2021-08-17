using UnityEngine;

public class LaserScript : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // materials
    [Header("materials")]
    public Material matA;
    public Material matB;

    // animation
    int flickerIndex, flickerRate, flickerCounter;

    // white orb permanent
    [HideInInspector] public Transform whiteOrbTransform;
    [HideInInspector] public GameObject whiteOrbObject;
    [HideInInspector] public MeshRenderer whiteOrbMeshRenderer;
    Vector3 whiteOrbSclOriginal, whiteOrbSclTarget, whiteOrbSclCur;

    // line
    [HideInInspector] public Vector3 shootPointTarget, shootPointCur;
    [HideInInspector] public Transform shootFromTransform;
    LineRenderer myLine;
    GameObject myLineObject;

    // damage
    [HideInInspector] public NpcCore npcCoreBy;
    int spawnDamageDealRate, spawnDamageDealCounter;

    public float widthBase;

    public bool targetPlayer;

    // layerMasks
    public LayerMask hitLayerMask;

    // clear
    [HideInInspector] public int clearDur;
    int clearCounter;

    void Start ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // create line
        GameObject lineO = new GameObject("laserO");
        myLineObject = lineO;

        LineRenderer lineLr = lineO.AddComponent<LineRenderer>();
        lineLr.useWorldSpace = true;
        lineLr.positionCount = 2;
        lineLr.material = matA;
        lineLr.numCapVertices = 3;

        float lineW = widthBase;
        lineLr.startWidth = lineW;
        lineLr.endWidth = lineW;
        lineLr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        myLine = lineLr;

        // create white orb
        whiteOrbObject = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.whiteOrbPermanentPrefab,myTransform.position,Quaternion.identity,1f);
        whiteOrbTransform = whiteOrbObject.transform;
        whiteOrbSclOriginal = whiteOrbTransform.localScale;
        whiteOrbSclTarget = whiteOrbSclOriginal;
        whiteOrbSclCur = whiteOrbSclTarget;

        // damage
        spawnDamageDealRate = 20;
        spawnDamageDealCounter = 0;

        // animation
        flickerIndex = 0;
        flickerRate = 8;
        flickerCounter = 0;

        // clear
        clearCounter = 0;
    }

    void Update ()
    {
        if (!SetupManager.instance.inFreeze && !SetupManager.instance.paused)
        {
            // update line
            UpdateLine();

            // clear?
            if ( npcCoreBy == null || npcCoreBy.curState != NpcCore.State.AttackDo )
            {
                Clear();
            }
            if ( clearCounter < clearDur )
            {
                clearCounter++;
            }
            else
            {
                Clear();
            }
        }
    }

    void UpdateLine ()
    {
        Vector3 p0 = shootFromTransform.position;

        if ( targetPlayer )
        {
            Vector3 d0 = p0;
            Vector3 d1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
            shootPointTarget = d1;
            shootPointTarget.y = 0f;

            shootPointCur = Vector3.Lerp(shootPointCur,shootPointTarget,1.25f * Time.deltaTime);

            float t1 = (Time.time * 5f);
            float f1 = .05f;
            float s1 = Mathf.Sin(t1) * f1;
            float s2 = Mathf.Cos(t1) * f1;
            shootPointCur.x += s1;
            shootPointCur.z += s2;
        }

        Vector3 p1 = shootPointCur; //p0 + (shootPointCur * 100f);

        RaycastHit cHit;
        if ( Physics.Linecast(p0,p1,out cHit,hitLayerMask) )
        {
            p1 = cHit.point;
        }

        // white orb
        if ( whiteOrbTransform != null )
        {
            whiteOrbSclTarget = whiteOrbSclOriginal;
            float tt0 = (Time.time * 40f);
            float ff0 = .25f;
            float ss0 = Mathf.Sin(tt0) * ff0;
            whiteOrbSclTarget *= (1f + ss0);
            whiteOrbSclCur = whiteOrbSclTarget;

            whiteOrbTransform.position = p1;
        }

        if (spawnDamageDealCounter < spawnDamageDealRate)
        {
            spawnDamageDealCounter++;
        }
        else
        {
            // spawn a damage deal orb
            PrefabManager.instance.SpawnDamageDeal(p1, 2f, 1, Npc.AttackData.DamageType.Magic, 10, shootFromTransform, 1f, false, DamageDeal.Target.Player, npcCoreBy,false,false);

            // spawn a blue fire!
            GameObject fireImpactO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.firePrefab, p1, Quaternion.identity, .5f);
            FireScript fireImpactScript = fireImpactO.GetComponent<FireScript>();
            if (fireImpactScript != null)
            {
                fireImpactScript.scaleMultiplier = 2f;
                fireImpactScript.SetFireType(FireScript.FireType.Blue);
            }

            spawnDamageDealCounter = 0;
        }

        // material
        if ( flickerCounter < flickerRate )
        {
            flickerCounter++;
        }
        else
        {
            flickerIndex = (flickerIndex == 0) ? 1 : 0;
            flickerCounter = 0;
        }
        myLine.material = (flickerIndex == 0) ? matA : matB;

        // set points
        myLine.SetPosition(0,p0);
        myLine.SetPosition(1,p1);

        // update width
        float widthCur = widthBase;
        float t0 = Time.time * 80f;
        float f0 = .125f;
        float s0 = Mathf.Sin(t0) * f0;
        widthCur += s0;
        myLine.startWidth = widthCur;
        myLine.endWidth = widthCur;
    }

    public void Clear ()
    {
        if ( whiteOrbObject != null )
        {
            Destroy(whiteOrbObject);
        }
        if ( myLineObject != null )
        {
            Destroy(myLineObject);
        }
        Destroy(myGameObject);
    }
}
