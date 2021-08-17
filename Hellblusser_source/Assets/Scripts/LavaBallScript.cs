using UnityEngine;

public class LavaBallScript : MonoBehaviour
{
    // base
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // state
    int explodeWaitMin, explodeWaitMax, explodeWait, explodeCounter;
    bool exploded;

    // animation
    public Transform graphicsTransform;
    Vector3 sclTarget, sclCur;

    void Start ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // state
        explodeWaitMin = 180;
        explodeWaitMax = 240;
        explodeWait = Mathf.RoundToInt(TommieRandom.instance.RandomRange(explodeWaitMin,explodeWaitMax));
        explodeCounter = 0;
        exploded = false;

        // animation
        if (graphicsTransform != null)
        {
            sclTarget = Vector3.zero;
            sclCur = sclTarget;
            graphicsTransform.localScale = sclCur;
        }
    }

    void Update ()
    {
        if ( !SetupManager.instance.inFreeze )
        {
            // animation?
            if (graphicsTransform != null)
            {
                float sclGrow = (.275f * Time.deltaTime);
                sclTarget.x += sclGrow;
                sclTarget.y += sclGrow;
                sclTarget.z += sclGrow;
                sclCur = sclTarget;
                graphicsTransform.localScale = sclCur;
            }

            // explode?
            if (!exploded)
            {
                if (explodeCounter < explodeWait)
                {
                    explodeCounter++;
                }
                else
                {
                    Explode();
                }
            }
        }
    }

    public void Explode ()
    {
        // spawn lava projectile
        GameObject lavaProjectileO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.lavaProjectilePrefab[0],myTransform.position,Quaternion.identity,1f);
        ProjectileScript lavaProjectileScript = lavaProjectileO.GetComponent<ProjectileScript>();
        if( lavaProjectileScript != null )
        {
            Vector3 d = Vector3.zero;
            d.y = TommieRandom.instance.RandomRange(6f,8f);

            float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f,1f));
            float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f,1f));
            float horOffMax = 1f;
            float xAdd = TommieRandom.instance.RandomRange(horOffMax * .5f,horOffMax) * xDir;
            float zAdd = TommieRandom.instance.RandomRange(horOffMax * .5f, horOffMax) * zDir;
            d.x = xAdd;
            d.z = zAdd;

            lavaProjectileScript.dir = d;
            lavaProjectileScript.speed = 2f;
            lavaProjectileScript.radius = .25f;
            lavaProjectileScript.myOwnerType = ProjectileScript.OwnerType.None;
            lavaProjectileScript.clearDurAdd = 600;
        }

        // particles
        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.lavaBallImpactParticlesPrefab[0],myTransform.position,Quaternion.identity,1f);

        // doei druif
        Clear();

        // done
        exploded = true;
    }

    public void Clear ()
    {
        if ( myGameObject != null )
        {
            Destroy(myGameObject);
        }
    }
}
