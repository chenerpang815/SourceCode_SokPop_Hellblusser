using UnityEngine;
using System.Collections.Generic;

public class Stuiterbaar : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // graphics
    [Header("graphics")]
    public Transform graphicsTransform;

    public Material matFlicker;
    public MeshRenderer[] graphicsMeshRenderers;
    List<Material[]> meshMaterials;
    List<Material[]> meshFlickerMaterials;

    // owner
    public ProjectileScript.OwnerType myOwnerType;

    // flicker
    int flickerIndex, flickerRateMin, flickerRateMax, flickerRate, flickerCounter, flickerWaitDur, flickerWaitCounter, flickerWaitIndex, flickerWaitCount, flickerWaitCountMax;

    // scale
    Vector3 scaleOriginal, scaleTarget, scaleCur, scaleAdd;

    // layerMasks
    [Header("layerMasks")]
    public LayerMask collideLayerMask;

    // settings
    [Header("settings")]
    public float sideForceFactor;
    public float upForceFactor;
    public float gravityFactor;
    public float radius;
    public Collectible myCollectible;
    public bool preventRandomForceOnStart;
    public float gravityMultiplier;
    public float bounceMultiplier;

    // damage
    public bool dealDamage;
    public float damageDealRadius;
    public NpcCore npcCoreBy;

    // force
    Vector3 posCur;
    [HideInInspector] public Vector3 forceCur;
    float grav;

    // bounce
    int canBounceDur, canBounceCounter;

    // auto clear?
    public bool autoClear;
    [HideInInspector] public int autoClearDur;
    int autoClearCounter;

    // audio
    public enum AudioType { Coin, Tear, Key, Donut, Stone, Metal, Bone };

    [Header("audio")]
    public AudioType myAudioType;
    public float pitchMin;
    public float pitchMax;
    public float volumeMin;
    public float volumeMax;

    void Start()
    {
        myTransform = transform;
        myGameObject = gameObject;

        canBounceDur = 10;
        canBounceCounter = canBounceDur;

        posCur = myTransform.position;

        // meshes
        meshMaterials = new List<Material[]>();
        meshFlickerMaterials = new List<Material[]>();
        for ( int i = 0; i < graphicsMeshRenderers.Length; i ++ )
        {
            Material[] curMat = graphicsMeshRenderers[i].materials;
            meshMaterials.Add(curMat);

            Material[] flickerMat = new Material[curMat.Length];
            for ( int ii = 0; ii < flickerMat.Length; ii ++ )
            {
                flickerMat[ii] = matFlicker;
            }
            meshFlickerMaterials.Add(flickerMat);
        }

        // force on start
        if (!preventRandomForceOnStart)
        {
            float forceSideOffMax = (.1f * sideForceFactor);
            float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
            float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
            float xAdd = TommieRandom.instance.RandomRange(forceSideOffMax * .25f, forceSideOffMax);
            float zAdd = TommieRandom.instance.RandomRange(forceSideOffMax * .25f, forceSideOffMax);
            forceCur.x = (xAdd * xDir);
            forceCur.z = (zAdd * zDir);
        }

        // flicker
        flickerIndex = 0;
        flickerRateMin = 60;
        flickerRateMax = 90;
        flickerRate = Mathf.RoundToInt(TommieRandom.instance.RandomRange(flickerRateMin, flickerRateMax));
        flickerCounter = 0;

        if ( dealDamage )
        {
            flickerCounter = flickerRate;
            float flickerFac = .125f;
            flickerRateMin = Mathf.RoundToInt(flickerRateMin * flickerFac);
            flickerRateMax = Mathf.RoundToInt(flickerRateMax * flickerFac);
        }

        flickerWaitCountMax = 2;
        flickerWaitCount = 0;
        flickerWaitIndex = 0;
        flickerWaitDur = 4;
        flickerWaitCounter = 0;

        // scale
        scaleOriginal = graphicsTransform.localScale;
        scaleTarget = scaleOriginal;
        scaleCur = scaleTarget;

        // random rotation
        SetRandomRotation();
    }

    void Update ()
    {
        if (!SetupManager.instance.inFreeze)
        {
            float lDst = (radius * 1.5f);
            Vector3 l0 = myTransform.position;
            l0.y += lDst;
            Vector3 l1 = myTransform.position;
            l1.y -= lDst;
            if ( Physics.Linecast(l0,l1,SetupManager.instance.lavaOnlyLayerMask) )
            {
                if ( myCollectible != null && myCollectible.myType == Collectible.Type.Key && !GameManager.instance.playerHasKey )
                {
                    GameManager.instance.PlayerFoundKey();
                }

                PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab,myTransform.position,Quaternion.identity,1.5f);
                AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.fireStartClips), .9f, 1.2f, .2f, .225f);
                Clear();
            }

            // auto clear?
            if ( autoClear )
            {
                if ( autoClearCounter < autoClearDur )
                {
                    autoClearCounter++;
                }
                else
                {
                    PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab,myTransform.position,Quaternion.identity,1f);
                    Clear();
                }
            }

            // damage?
            if ( dealDamage )
            {
                CheckIfNeedToDealDamage();
            }

            // stuiter?
            bool cancelStuiterbaar = false;
            if ( myCollectible != null && myCollectible.collected )
            {
                cancelStuiterbaar = true;
            }
            if (!cancelStuiterbaar)
            {
                if (!dealDamage)
                {
                    forceCur = Vector3.Lerp(forceCur, Vector3.zero, 1.25f * Time.deltaTime);
                }
                else
                {
                    forceCur.y = Mathf.Lerp(forceCur.y,0f,1.25f * Time.deltaTime);
                }
                posCur += forceCur;

                myTransform.position = posCur;

                if (canBounceCounter < canBounceDur)
                {
                    canBounceCounter++;
                }
                else
                {
                    grav += (((.125f * gravityFactor) * gravityMultiplier) * Time.deltaTime);
                    posCur.y -= grav;

                    // check down
                    float cDst = radius;
                    Vector3 c0 = myTransform.position;
                    c0.y += cDst;
                    Vector3 c1 = myTransform.position;
                    c1.y -= cDst;
                    if (Physics.Linecast(c0, c1, collideLayerMask))
                    {
                        grav = 0f;

                        float bounceForceMax = .05f;
                        forceCur.y += (TommieRandom.instance.RandomRange(bounceForceMax * .625f, bounceForceMax) * upForceFactor) * bounceMultiplier;

                        canBounceCounter = 0;

                        // scale
                        scaleCur = scaleOriginal;
                        scaleCur.x *= TommieRandom.instance.RandomRange(1.25f, 1.5f);
                        scaleCur.y *= TommieRandom.instance.RandomRange(.5f, .75f);
                        scaleCur.z *= TommieRandom.instance.RandomRange(1.25f, 1.5f);

                        // random rotation
                        SetRandomRotation();

                        // audio
                        PlayImpactAudio();
                    }
                }

                // check in direction
                float fDst = radius;
                Vector3 p0 = myTransform.position;
                Vector3 p1 = p0;
                Vector3 fCheck = forceCur;
                fCheck.y = 0f;
                Vector3 fRaw = fCheck;
                fCheck.Normalize();
                p0 += fCheck * -fDst;
                p1 += fCheck * fDst;
                RaycastHit fHit;
                if (Physics.Linecast(p0, p1, out fHit, collideLayerMask))
                {
                    // physics
                    float m0 = fRaw.magnitude;
                    Vector3 r0 = forceCur.normalized;
                    Vector3 r1 = -fHit.normal.normalized;
                    r1.y = r0.y;
                    Vector3 r2 = Vector3.Reflect(r0, r1).normalized;
                    forceCur.x = r2.x * m0;
                    forceCur.z = r2.z * m0;

                    // particles
                    PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, fHit.point, Quaternion.identity, .75f);

                    // audio
                    PlayImpactAudio();

                    /*
                    // log
                    Debug.DrawLine(fHit.point, fHit.point + r0, Color.magenta, 5f);
                    Debug.DrawLine(fHit.point, fHit.point + r1, Color.cyan, 5f);
                    Debug.DrawLine(fHit.point, fHit.point + r2, Color.green, 5f);
                    */
                }

                // flicker
                if (flickerIndex == 0)
                {
                    if (flickerCounter < flickerRate)
                    {
                        flickerCounter++;
                    }
                    else
                    {
                        flickerRate = Mathf.RoundToInt(TommieRandom.instance.RandomRange(flickerRateMin, flickerRateMax));
                        flickerCounter = 0;

                        flickerIndex = 1;
                        flickerWaitCounter = 0;
                        flickerWaitIndex = 0;
                        flickerWaitCount = 0;

                        SetMaterials(false);
                        //graphicsMeshRenderer.materials = matOriginal;
                    }
                }
                else
                {
                    if (flickerWaitCount < flickerWaitCountMax)
                    {
                        if (flickerWaitCounter < flickerWaitDur)
                        {
                            flickerWaitCounter++;
                        }
                        else
                        {
                            flickerWaitCount++;
                            flickerWaitIndex = (flickerWaitIndex == 0) ? 1 : 0;
                            flickerWaitCounter = 0;

                            SetMaterials(flickerWaitIndex != 0);
                            //graphicsMeshRenderer.materials = (flickerWaitIndex == 0) ? matOriginal : matFlicker;
                        }
                    }
                    else
                    {
                        flickerIndex = 0;
                        flickerCounter = 0;
                        SetMaterials(false);
                        //graphicsMeshRenderer.materials = matOriginal;
                    }
                }

                // scaling
                scaleTarget = scaleOriginal;
                scaleAdd = BasicFunctions.SpringVector(scaleAdd, scaleCur, scaleTarget, .01f, .25f, .5f, .925f);
                scaleCur += scaleAdd;
                graphicsTransform.localScale = scaleCur;
            }
        }
    }

    void CheckIfNeedToDealDamage ()
    {
        // collision?
        Collider[] hitColliders = Physics.OverlapSphere(myTransform.position, radius * damageDealRadius);
        if (hitColliders != null && hitColliders.Length > 0)
        {
            for (int i = 0; i < hitColliders.Length; i++)
            {
                GameObject hitO = hitColliders[i].gameObject;
                if (hitO != null && ((hitO.layer != 11 && hitO.layer != 9 && hitO.layer != 13 && hitO.layer != 14 && hitO.layer != 15) || (myOwnerType == ProjectileScript.OwnerType.Player && hitO.layer == 10) || (myOwnerType == ProjectileScript.OwnerType.Npc && hitO.layer == 8)))
                {
                    bool preventHit = false;
                    if (myOwnerType == ProjectileScript.OwnerType.Npc && hitO.layer == 10)
                    {
                        preventHit = true;
                    }
                    if (myOwnerType == ProjectileScript.OwnerType.Player && hitO.layer == 8)
                    {
                        preventHit = true;
                    }
                    if (myOwnerType == ProjectileScript.OwnerType.None)
                    {
                        preventHit = false;
                    }
                    if (!preventHit)
                    {
                        Vector3 p0 = myTransform.position;
                        Vector3 p1 = myTransform.position;
                        p1.y -= (radius + .05f);

                        // create damage deal object
                        DamageDeal.Target damageTargetType = (myOwnerType == ProjectileScript.OwnerType.Player) ? DamageDeal.Target.AI : DamageDeal.Target.Player;
                        if (myOwnerType == ProjectileScript.OwnerType.None)
                        {
                            damageTargetType = DamageDeal.Target.All;
                        }

                        Npc.AttackData.DamageType damageType = Npc.AttackData.DamageType.Magic;

                        PrefabManager.instance.SpawnDamageDeal(p0, damageDealRadius, 1, damageType, 10, HandManager.instance.myTransform, .325f, true, damageTargetType, npcCoreBy, false,false);

                        // create magic impact effect
                        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.magicImpactParticlesPrefab[0], p0, Quaternion.identity, 1f);

                        // ripple effect
                        Vector3 ppp0 = myTransform.position;
                        Vector3 ppp1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                        float ddd0 = Vector3.Distance(ppp0, ppp1);
                        if (ddd0 <= 4 || myOwnerType == ProjectileScript.OwnerType.Player)
                        {
                            Vector2 ripplePos = Camera.main.WorldToScreenPoint(myTransform.position);
                            SetupManager.instance.SetRippleEffect(ripplePos, 17.5f);
                        }

                        // clear
                        Clear();
                    }
                }
            }
        }
    }

    void SetMaterials ( bool _flicker )
    {
        for ( int i = 0; i < graphicsMeshRenderers.Length; i ++ )
        {
            graphicsMeshRenderers[i].materials = (_flicker) ? meshFlickerMaterials[i] : meshMaterials[i];
        }
    }

    void SetRandomRotation ()
    {
        graphicsTransform.localRotation = Quaternion.Euler(TommieRandom.instance.RandomRange(0f, 360f), TommieRandom.instance.RandomRange(0f, 360f), TommieRandom.instance.RandomRange(0f, 360f));
    }

    public void PlayImpactAudio ()
    {
        AudioClip[] clipsUse = null;
        switch (myAudioType)
        {
            case AudioType.Coin: clipsUse = AudioManager.instance.coinImpactClips; break;
            case AudioType.Tear: clipsUse = AudioManager.instance.tearImpactClips; break;
            case AudioType.Key: clipsUse = AudioManager.instance.coinImpactClips; break;
            case AudioType.Donut: clipsUse = AudioManager.instance.donutImpactClips; break;
            case AudioType.Stone: clipsUse = AudioManager.instance.stoneImpactClips; break;
            case AudioType.Metal: clipsUse = AudioManager.instance.metalImpactClips; break;
            case AudioType.Bone: clipsUse = AudioManager.instance.boneImpactClips; break;
        }
        AudioManager.instance.PlaySoundAtPosition(myTransform.position,BasicFunctions.PickRandomAudioClipFromArray(clipsUse), pitchMin, pitchMax, volumeMin, volumeMax);
    }

    public void SetOwnerType ( ProjectileScript.OwnerType _to )
    {
        myOwnerType = _to;
    }

    public void Clear ()
    {
        if (myGameObject != null)
        {
            Destroy(myGameObject);
        }
    }
}
