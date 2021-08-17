using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    public enum OwnerType { Player, Npc, None };
    public OwnerType myOwnerType;

    [HideInInspector] public NpcCore npcCoreBy;

    [HideInInspector] public bool didHit;

    public bool createFire;
    public FireScript.FireType createFireType;
    public GameObject impactParticlesPrefab;
    public float createFireScaleFactor;
    public float scaleFacMultiplier;
    public float startScaleFac;
    public float gravityMultiplier;
    public bool isSpecial;
    public bool putNpcOutOfSleep;
    public bool isFromCannon;
    public bool isFromFaerie;

    float grav;
    [HideInInspector] public Vector3 dir;
    [HideInInspector] public float speed;
    [HideInInspector] public float radius;

    Vector3 posCur;

    Vector3 sclOriginal, sclTarget, sclCur;
    float sclFacTarget, sclFacCur;

    [HideInInspector] public int clearDur, clearCounter, clearDurAdd;

    int spawnDur, spawnCounter;

    public LayerMask collideLayerMask;
    public LayerMask fireAttachLayerMask;

    public MeshRenderer myMeshRenderer;
    public Material matA, matB, matSpawn;
    int matRate, matCounter;
    int matIndex;

    int spawnPreventHitDur, spawnPreventHitCounter;

    public float damageDealRadiusFactor;
    float damageDealRadius;

    void Start ()
    {
        myTransform = transform;
        myGameObject = gameObject;

        posCur = myTransform.position;

        didHit = false;

        clearDur = 90;
        clearCounter = 0;

        sclOriginal = myTransform.localScale;
        sclTarget = sclOriginal;
        sclTarget.x *= 3f;
        sclTarget.y *= 3f;
        sclTarget.z *= .5f;
        sclCur = sclTarget;

        sclFacTarget = startScaleFac;
        sclFacCur = sclFacTarget;

        matRate = 6;
        matCounter = 0;
        matIndex = 0;

        spawnDur = 2;
        spawnCounter = 0;

        damageDealRadius = 1.5f;
        if ( myOwnerType == OwnerType.Player && SetupManager.instance != null && SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.WildFire) )
        {
            damageDealRadius += BlessingDatabase.instance.wildFireRadiusAdd;
        }
        damageDealRadius *= damageDealRadiusFactor;

        //if ( myOwnerType != OwnerType.Player )
        {
            spawnPreventHitDur = 0;
            spawnPreventHitCounter = 0;
        }

        myMeshRenderer.material = matSpawn;
    }

    void Update ()
    {
        if ( !SetupManager.instance.inFreeze )
        {
            // scaling
            float t0 = Time.time * 20f;
            float f0 = .175f;
            float s0 = Mathf.Sin(t0) * f0;

            sclFacTarget = Mathf.Lerp(sclFacTarget, 2f, 20f * Time.deltaTime);
            sclFacCur = sclFacTarget;

            sclTarget = ((sclOriginal * sclFacCur) * scaleFacMultiplier) + (Vector3.one * s0);
            sclCur = Vector3.Lerp(sclCur, sclTarget, 20f * Time.deltaTime);

            myTransform.localScale = sclCur;

            // position
            grav += ((.05f * gravityMultiplier) * Time.deltaTime);
            posCur.y -= grav;

            posCur += (dir * speed) * Time.deltaTime;

            myTransform.position = posCur;

            // clear?
            if (clearCounter < (clearDur + clearDurAdd))
            {
                clearCounter++;
            }
            else
            {
                Clear();
            }

            // collision?
            if ( spawnPreventHitCounter < spawnPreventHitDur )
            {
                spawnPreventHitCounter++;
            }
            else
            {
                Collider[] hitColliders = Physics.OverlapSphere(myTransform.position, radius);
                if (hitColliders != null && hitColliders.Length > 0)
                {
                    //bool createSmallFirePrefab = true;

                    for (int i = 0; i < hitColliders.Length; i++)
                    {
                        GameObject hitO = hitColliders[i].gameObject;
                        bool hitPlayerOrNpc = (myOwnerType == OwnerType.Player && hitO.layer == 10) || (myOwnerType == OwnerType.Npc && hitO.layer == 8);
                        bool hitSolidOrGround = (hitO.layer == 0 || hitO.layer == 11);
                        if (hitO != null && ((hitO.layer != 9 && hitO.layer != 13 && hitO.layer != 14 && hitO.layer != 15) || hitPlayerOrNpc))
                        {
                            if (!didHit)
                            {
                                if (hitPlayerOrNpc || hitSolidOrGround)
                                {
                                    didHit = true;
                                }
                            }
                            else
                            {
                                return;
                            }

                            //Debug.Log("we raken: " + hitO + ", layer: " + hitO.layer.ToString() + " || " + Time.time.ToString());

                            bool preventHit = false;
                            if (myOwnerType == OwnerType.Npc && hitO.layer == 10)
                            {
                                preventHit = true;
                            }
                            if (myOwnerType == OwnerType.Player && hitO.layer == 8)
                            {
                                preventHit = true;
                            }
                            if (myOwnerType == OwnerType.None)
                            {
                                preventHit = false;
                            }

                            bool gotBlocked = false;
                            if (myOwnerType == OwnerType.Player && hitO.layer == 10)
                            {
                                //Debug.Log("we raken een npc? || " + Time.time.ToString());

                                NpcHitBox npcHitBoxHit = hitO.GetComponent<NpcHitBox>();
                                if (npcHitBoxHit != null)
                                {
                                    // log
                                    //Debug.Log("hitBox gevonden! || " + Time.time.ToString());

                                    NpcCore npcCoreHit = npcHitBoxHit.myNpcCore;
                                    if (npcCoreHit != null)
                                    {
                                        // log
                                        //Debug.Log("npcCore gevonden! || " + Time.time.ToString());

                                        bool doBlockReact = (TommieRandom.instance.RandomValue(1f) <= npcCoreHit.myInfo.stats.reactBlockChance);
                                        if (doBlockReact)
                                        {
                                            // log
                                            //Debug.Log("we gaan reacten! || " + Time.time.ToString());

                                            if (npcCoreHit.curState == NpcCore.State.Chase || npcCoreHit.curState == NpcCore.State.AttackPrepare || npcCoreHit.curState == NpcCore.State.Block)
                                            {
                                                npcCoreHit.InitSetBlock();
                                                if (npcCoreHit.myInfo.stats.canRedirectProjectiles)
                                                {
                                                    preventHit = true;
                                                    myOwnerType = OwnerType.Npc;
                                                    npcCoreBy = npcCoreHit;
                                                    dir.x *= -1f;
                                                    dir.z *= -1f;
                                                    float redirectForce = .375f;
                                                    dir.x *= redirectForce;
                                                    dir.y += .325f;
                                                    dir.z *= redirectForce;
                                                    grav *= .5f;

                                                    // create UI text
                                                    Vector3 indicatorHeightOff = (Vector3.up * npcCoreHit.myInfo.graphics.healthIndicatorOff);
                                                    if (npcCoreHit.myInfo.graphics.flying)
                                                    {
                                                        Vector3 flyOffAdd = (Vector3.up * npcCoreHit.myGraphics.flyOff);
                                                        indicatorHeightOff += flyOffAdd;
                                                    }
                                                    Vector3 damageIndicatorP = npcCoreHit.myGraphics.graphicsTransform.position + indicatorHeightOff;

                                                    Transform damageIndicatorOffTr = GameManager.instance.mainCameraTransform;
                                                    damageIndicatorP += (damageIndicatorOffTr.forward * npcCoreHit.myInfo.graphics.damageIndicatorLocalAdd.z);
                                                    damageIndicatorP += (damageIndicatorOffTr.right * npcCoreHit.myInfo.graphics.damageIndicatorLocalAdd.x);
                                                    damageIndicatorP += (damageIndicatorOffTr.up * npcCoreHit.myInfo.graphics.damageIndicatorLocalAdd.y);
                                                    GameManager.instance.CreateDamageIndicatorString("redirect", damageIndicatorP);

                                                    // effects
                                                    SetupManager.instance.SetFreeze(4);
                                                    PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, myTransform.position, Quaternion.identity, .5f);

                                                    // log
                                                    //Debug.Log("redirect! || " + Time.time.ToString());
                                                }
                                                else if (npcCoreHit.myInfo.stats.canBlockProjectiles)
                                                {
                                                    gotBlocked = true;

                                                    // create UI text
                                                    Vector3 indicatorHeightOff = (Vector3.up * npcCoreHit.myInfo.graphics.healthIndicatorOff);
                                                    if (npcCoreHit.myInfo.graphics.flying)
                                                    {
                                                        Vector3 flyOffAdd = (Vector3.up * npcCoreHit.myGraphics.flyOff);
                                                        indicatorHeightOff += flyOffAdd;
                                                    }
                                                    Vector3 damageIndicatorP = npcCoreHit.myGraphics.graphicsTransform.position + indicatorHeightOff;

                                                    Transform damageIndicatorOffTr = GameManager.instance.mainCameraTransform;
                                                    damageIndicatorP += (damageIndicatorOffTr.forward * npcCoreHit.myInfo.graphics.damageIndicatorLocalAdd.z);
                                                    damageIndicatorP += (damageIndicatorOffTr.right * npcCoreHit.myInfo.graphics.damageIndicatorLocalAdd.x);
                                                    damageIndicatorP += (damageIndicatorOffTr.up * npcCoreHit.myInfo.graphics.damageIndicatorLocalAdd.y);
                                                    GameManager.instance.CreateDamageIndicatorString("block", damageIndicatorP);

                                                    // effects
                                                    SetupManager.instance.SetFreeze(4);
                                                    PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, myTransform.position, Quaternion.identity, .5f);

                                                    // log
                                                    //Debug.Log("geblocked!! || " + Time.time.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (gotBlocked)
                            {
                                preventHit = true;
                                Clear();
                            }

                            if (!preventHit)
                            {
                                //Debug.Log("we gaan stuk waarom, we raken layer: " + hitO.layer.ToString() + " || ownerType: " + myOwnerType + " || " + Time.time.ToString());

                                Vector3 p0 = myTransform.position;
                                Vector3 p1 = myTransform.position;
                                p1.y -= ((radius /** scaleFacMultiplier*/) + .05f);

                                //Debug.DrawLine(p0,p1,Color.green);

                                // create damage deal object
                                DamageDeal.Target damageTargetType = (myOwnerType == OwnerType.Player) ? DamageDeal.Target.AI : DamageDeal.Target.Player;
                                if (myOwnerType == OwnerType.None)
                                {
                                    damageTargetType = DamageDeal.Target.All;
                                }
                                if (putNpcOutOfSleep)
                                {
                                    damageTargetType = DamageDeal.Target.AI;
                                }

                                Npc.AttackData.DamageType damageType = Npc.AttackData.DamageType.Magic;
                                if (isSpecial)
                                {
                                    damageType = Npc.AttackData.DamageType.Special;
                                }

                                PrefabManager.instance.SpawnDamageDeal(p0, damageDealRadius, 1, damageType, 10, HandManager.instance.myTransform, .325f, true, damageTargetType, npcCoreBy, putNpcOutOfSleep, false);

                                // create magic impact effect
                                PrefabManager.instance.SpawnPrefab(PrefabManager.instance.magicImpactParticlesPrefab[0], p0, Quaternion.identity, 1f);

                                RaycastHit cHit;
                                if (Physics.Linecast(p0, p1, out cHit, fireAttachLayerMask))
                                {
                                    Vector3 p = cHit.point;

                                    // ff de normal checken hiero
                                    //Debug.Log("projectile hit normal: " + Vector3.Dot(cHit.normal.normalized,Vector3.up).ToString() + " || " + Time.time.ToString());

                                    // create fire impact effect
                                    if (createFire)
                                    {
                                        GameObject fireImpactO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.firePrefab, p, Quaternion.identity, .5f);
                                        FireScript fireImpactScript = fireImpactO.GetComponent<FireScript>();
                                        if (fireImpactScript != null)
                                        {
                                            fireImpactScript.canBurnPlayer = true;
                                            fireImpactScript.scaleMultiplier = createFireScaleFactor;
                                            fireImpactScript.SetFireType(createFireType);
                                        }
                                    }
                                }
                                else
                                {
                                    // create decal
                                    DecalManager.instance.AddDecal(myTransform.position, radius * 2f);

                                    // burn flameable objects?
                                    if (createFire)
                                    {
                                        for (int ii = 0; ii < GameManager.instance.allFlameableObjects.Count; ii++)
                                        {
                                            FlameableScript curFlameableScript = GameManager.instance.allFlameableObjects[ii];
                                            if (curFlameableScript != null)
                                            {
                                                Vector3 pp0 = myTransform.position;
                                                Vector3 pp1 = curFlameableScript.myTransform.position;
                                                float dd0 = Vector3.Distance(pp0, pp1);
                                                if (dd0 <= (curFlameableScript.radius * .75f))
                                                {
                                                    curFlameableScript.AddFlameableProgress(1f, false, true, createFireType);
                                                }
                                            }
                                        }
                                    }
                                }

                                // ripple effect
                                Vector3 ppp0 = myTransform.position;
                                Vector3 ppp1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                                float ddd0 = Vector3.Distance(ppp0, ppp1);
                                if (ddd0 <= 4 || myOwnerType == OwnerType.Player)
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
        }
    }

    public void SetOwnerType ( OwnerType _to )
    {
        myOwnerType = _to;
    }

    public void Clear ()
    {
        // clear audio
        if (!isFromCannon)
        {
            if (!isFromFaerie)
            {
                AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.fireMagicImpactClips), .6f, .8f, .1f, .125f);
            }
            else
            {
                AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.faerieCastSpellClips), .3f, .5f, .3f, .325f);
            }
        }
        else
        {
            AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.cannonImpactClips), .8f, 1f, .6f, .625f,6f,24f);
        }

        // particles
        float whiteOrbScl = damageDealRadius;//radius * 2f;//1.5f;
        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab,myTransform.position,Quaternion.identity,whiteOrbScl);

        if (impactParticlesPrefab != null)
        {
            PrefabManager.instance.SpawnPrefab(impactParticlesPrefab, myTransform.position, Quaternion.identity, 1f);
        }

        // doei druif
        Destroy(myGameObject);
    }
}
