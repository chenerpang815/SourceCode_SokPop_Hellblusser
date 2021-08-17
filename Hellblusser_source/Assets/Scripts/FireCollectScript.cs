using UnityEngine;

public class FireCollectScript : MonoBehaviour
{
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    Vector3 posTarget, posCur;
    Vector3 forceCur;

    Vector3 sclOriginal, sclTarget, sclCur;
    float sclFacTarget, sclFacCur;
    float sclMultiplier;

    int spawnDur, spawnCounter;

    [HideInInspector] public NpcCore npcCollectedBy;

    public MeshRenderer myMeshRenderer;
    public Material matA, matB, matSpawn;
    int matRate, matCounter;
    int matIndex;

    [HideInInspector] public Transform targetTransform;

    float lerpieAdd;

    void Start ()
    {
        myTransform = transform;
        myGameObject = gameObject;

        posCur = myTransform.position;

        sclOriginal = myTransform.localScale;
        sclTarget = sclOriginal;
        sclTarget.x *= 3f;
        sclTarget.y *= 3f;
        sclTarget.z *= .5f;
        sclCur = sclTarget;

        sclMultiplier = (npcCollectedBy == null) ? 1f : 8f;

        matRate = 6;
        matCounter = 0;
        matIndex = 0;

        spawnDur = 2;
        spawnCounter = 0;

        float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f,1f));
        float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
        float offMax = .175f;
        float xAdd = TommieRandom.instance.RandomRange(offMax * .125f, offMax * .5f) * xDir;
        float zAdd = TommieRandom.instance.RandomRange(offMax * .125f, offMax * .5f) * zDir;
        forceCur.x = xAdd;
        forceCur.y = TommieRandom.instance.RandomRange(offMax * .5f,offMax);
        forceCur.z = zAdd;

        myMeshRenderer.material = matSpawn;
    }

    void Update ()
    {
        if ( !SetupManager.instance.inFreeze )
        {
            lerpieAdd += .25f;

            // scaling
            float t0 = Time.time * 20f;
            float f0 = .025f;
            float s0 = Mathf.Sin(t0) * f0;

            sclFacTarget = Mathf.Lerp(sclFacTarget, .125f, 20f * Time.deltaTime);
            sclFacCur = (sclFacTarget * sclMultiplier);

            sclTarget = (sclOriginal * sclFacCur) + (Vector3.one * s0);
            sclCur = Vector3.Lerp(sclCur, sclTarget, 20f * Time.deltaTime);

            myTransform.localScale = sclCur;

            // position
            forceCur = Vector3.Lerp(forceCur,Vector3.zero,5f * Time.deltaTime);
            posCur += forceCur;

            float lerpieBase = (npcCollectedBy != null) ? 2.5f : 10f;

            if (forceCur.magnitude < .1f)
            {
                posTarget = targetTransform.position + (Vector3.up * .75f);
                posCur = Vector3.Lerp(posCur, posTarget, (lerpieBase + lerpieAdd) * Time.deltaTime);
            }

            myTransform.position = posCur;

            // material
            if (spawnCounter < spawnDur)
            {
                spawnCounter++;
            }
            else
            {
                if (matCounter < matRate)
                {
                    matCounter++;
                }
                else
                {
                    matCounter = 0;
                    myMeshRenderer.material = (matIndex == 0) ? matB : matA;
                    matIndex = (matIndex == 0) ? 1 : 0;
                }
            }

            // JA ZIJN WE ER NOU AL OF WAT
            Vector3 p0 = posCur;
            Vector3 p1 = posTarget;
            float d0 = Vector3.Distance(p0, p1);
            if (d0 <= .125f)
            {
                if (npcCollectedBy == null)
                {
                    GameManager.instance.AddPlayerFire(1);

                    // fire collect popup?
                    GameManager.instance.fireCollected = true;
                }
                else
                {
                    if ( npcCollectedBy.myInfo.stats.healFromFire )
                    {
                        if (npcCollectedBy.fireCollectedCount < 3)
                        {
                            npcCollectedBy.Heal(1);
                            npcCollectedBy.fireCollectedCount++;
                        }
                    }
                }
                Clear();
            }
        }
    }

    public void Clear ()
    {
        // particles
        //PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab,myTransform.position,Quaternion.identity,1.5f);
        //PrefabManager.instance.SpawnPrefab(PrefabManager.instance.fireImpactParticlesPrefab,myTransform.position,Quaternion.identity,1f);

        Destroy(myGameObject);

        // log
        //Debug.Log("fire collect cleared! || " + Time.time.ToString());
    }
}
