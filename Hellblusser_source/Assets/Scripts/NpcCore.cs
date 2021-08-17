using UnityEngine;
using Npc;
using System.Collections;
using System.Collections.Generic;

public class NpcCore : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // spawner
    [HideInInspector] public NpcSpawner spawnedBy;
    [HideInInspector] public int spawnedByIndex;
    [HideInInspector] public bool isConnectedToOtherNpc;
    [HideInInspector] public NpcCore connectedBy;

    [HideInInspector] public List<NpcCore> myMinions;
    [HideInInspector] public NpcCore minionMaster;

    // boss
    [HideInInspector] public bool isBoss;

    // type
    [Header("type")]
    public Type myType;

    // hitbox
    [Header("hitbox")]
    public BoxCollider myHitBox;

    // movement
    [HideInInspector] public MovementTransformContainer movementTransformContainer;

    // minion spawn
    [HideInInspector] public MinionSpawnTransformContainer minionSpawnTransformContainer;

    // dungeon boss
    [HideInInspector] public NpcCore dungeonBossNpcCore;

    // magic skull
    [HideInInspector] public Transform[] eyeTransforms;
    [HideInInspector] public bool lostEyes;
    [HideInInspector] public int justLostEyesDur, justLostEyesCounter;

    // cannon scripts
    [HideInInspector] public CannonScriptContainer cannonScriptContainer;
    [HideInInspector] public List<CannonScript> cannonScripts;

    // graphics
    [Header("graphics")]
    public NpcGraphics myGraphics;

    // physics
    [HideInInspector] public Vector3 forceDirTarget, forceDirCur;
    [HideInInspector] public float forceDirLerpie;
    int hitDur, hitCounter;
    [HideInInspector] public bool hitButImmune;
    [HideInInspector] public int hitFlickerIndex, hitFlickerRate, hitFlickerCounter;

    // attacks
    [HideInInspector] public AttackData lastAttackData;
    [HideInInspector] public AttackData curAttackData;
    [HideInInspector] public int curAttackIndex;
    [HideInInspector] public int curAttackSprayCount, curAttackSprayCounter;
    [HideInInspector] public int attackFireTrailCounter;
    [HideInInspector] public float attackPrepareDurFac, attackDoDurFac;
    [HideInInspector] public float speedBoost;
    Coroutine curRangedAttackCoroutine;

    [HideInInspector] public bool overrideAttackPoint;
    [HideInInspector] public Vector3 attackPointOverride;
    [HideInInspector] public Vector3 attackDir;

    // unit
    [Header("unit")]
    public Unit myUnit;

    // physics
    [Header("physics")]
    public LayerMask bounceLayerMask;

    // health indicator
    [HideInInspector] public HealthIndicator myHealthIndicator;

    // particles
    [HideInInspector] public Vector3 particleSpawnPoint;

    // stats
    [HideInInspector] public Info myInfo;
    [HideInInspector] public int health;
    [HideInInspector] public WeightedRandomBag<AttackData> myAttackDatas;

    // state
    [HideInInspector] public int stageIndex;
    [HideInInspector] public bool enteredNextStage;

    // state
    public enum State
    {
        Roam,
        Chase,
        AttackPrepare,
        AttackDo,
        Alerted,
        Hit,
        Stunned,
        Block,
        LightCannon,
        MoveToPosition,
        Flee,
        Vulnerable,
        Sleeping,
        WakeUp,
        Laugh,
        Hide,
        WakeDungeonBoss,
        CollectFire,
        EnterNextStage
    };

    public State curState;
    [HideInInspector] public bool inCombat;
    [HideInInspector] public int tearDropCount;
    [HideInInspector] public bool preventTearDrop;
    [HideInInspector] public Vector3 moveToPositionTargetPoint;
    [HideInInspector] public State moveToPositionEndState;
    [HideInInspector] public float moveToPositionStopDst;
    [HideInInspector] public bool autoAlerted;

    // alerted
    [HideInInspector] public int alertedDur, alertedCounter;

    // attack prepare
    [HideInInspector] public int attackPrepareCounter;
    [HideInInspector] public Vector3 attackPrepareDir;

    // block
    [HideInInspector] public int blockMinDur, blockMaxDur, blockDur, blockCounter;
    [HideInInspector] public int canBlockDur, canBlockCounter;

    // attack do
    [HideInInspector] public int attackDoCounter;
    [HideInInspector] public bool attackDealtDamage;

    // stunned
    [HideInInspector] public int stunnedDur, stunnedCounter;

    // vulnerable
    [HideInInspector] public int vulnerableDur, vulnerableCounter;

    // collect fire
    [HideInInspector] public int collectFireDur, collectFireCounter;
    public Transform fireCollectPointTransform;
    [HideInInspector] public int fireCollectedCount;

    // enter next stage
    [HideInInspector] public int enterNextStageDur, enterNextStageCounter;
    [HideInInspector] public bool clearingLava, clearedLava;
    [HideInInspector] public int lavaCollectCreateRate, lavaCollectCreateCounter;

    // wakeUp
    [HideInInspector] public int wakeUpDur, wakeUpCounter;
    [HideInInspector] public float wakeUpProgress;

    // laugh
    [HideInInspector] public int laughDur, laughCounter;
    [HideInInspector] public bool didLaugh;

    // hide
    [HideInInspector] public int hideDur, hideCounter;

    // chasing
    [HideInInspector] public Transform chaseTransform;

    // fleeing
    [HideInInspector] public Transform fleeTransform;

    // cannon
    [HideInInspector] public CannonScript cannonScriptTarget;

    // audio
    //[Header("audio")]
    //public AudioSource myAudioSource;
    //public Transform audioSourceTransform;
    //public GameObject audioSourceGameObject;

    AudioClip[] alertClipsUse;
    float alertPitchMin, alertPitchMax;
    float alertVolumeMin, alertVolumeMax;

    AudioClip[] attackPrepareClipsUse;
    float attackPreparePitchMin, attackPreparePitchMax;
    float attackPrepareVolumeMin, attackPrepareVolumeMax;

    AudioClip[] attackDoClipsUse;
    float attackDoPitchMin, attackDoPitchMax;
    float attackDoVolumeMin, attackDoVolumeMax;

    AudioClip[] hurtClipsUse;
    float hurtPitchMin, hurtPitchMax;
    float hurtVolumeMin, hurtVolumeMax;

    AudioClip[] deadClipsUse;
    float deadPitchMin, deadPitchMax;
    float deadVolumeMin, deadVolumeMax;

    // init
    [HideInInspector] public bool initialized;

    void Start ()
    {
        Init();
    }

    void Init ()
    {
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // stage
        stageIndex = 0;

        // load stats
        myInfo = NpcDatabase.instance.LoadInfo(myType);
        health = myInfo.stats.health;

        // gru bestaat?
        if ( minionMaster != null )
        {
            health = Mathf.FloorToInt(health * .5f);
            if ( health < 1 )
            {
                health = 1;
            }
        }

        // become stronger? (endless mode)
        if (SetupManager.instance.curRunType == SetupManager.RunType.Endless)
        {
            health += (3 * SetupManager.instance.runDataRead.curLoopIndex + 1);

            attackPrepareDurFac = 1f - (.1f * SetupManager.instance.runDataRead.curLoopIndex);
            attackPrepareDurFac = Mathf.Clamp(attackPrepareDurFac, .25f, 1f);

            attackDoDurFac = 1f - (.1f * SetupManager.instance.runDataRead.curLoopIndex);
            attackDoDurFac = Mathf.Clamp(attackDoDurFac,.25f,1f);

            speedBoost = (.1f * SetupManager.instance.runDataRead.curLoopIndex);
            speedBoost = Mathf.Clamp(speedBoost,0f,2f);
        }
        else
        {
            attackPrepareDurFac = 1f;
            attackDoDurFac = 1f;
            speedBoost = 0f;
        }

        // load attackDatas
        myAttackDatas = new WeightedRandomBag<AttackData>();
        if ( myInfo.attacks != null && myInfo.attacks.Length > 0 )
        {
            for (int i = 0; i < myInfo.attacks.Length; i++)
            {
                myAttackDatas.AddEntry(myInfo.attacks[i],myInfo.attacks[i].weight);
            }
        }

        // graphics?
        if ( myGraphics != null )
        {
            myGraphics.Init();
        }

        // physics
        forceDirTarget = Vector3.zero;
        forceDirCur = forceDirTarget;
        forceDirLerpie = 2.5f;

        // hitBox
        if ( myHitBox != null )
        {
            myHitBox.center += myInfo.stats.hitBoxCenterOff;
            myHitBox.size += myInfo.stats.hitBoxScaleFac;
        }

        // lava collect
        lavaCollectCreateRate = 6;
        lavaCollectCreateCounter = 0;

        // counters
        hitDur = 0;
        hitCounter = hitDur;
        hitFlickerIndex = 0;
        hitFlickerRate = 6;
        hitFlickerCounter = 0;

        // create UI prompts?
        GameObject newHealthIndicatorO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.healthIndicatorPrefab[0], Vector3.zero, Quaternion.identity, 1f);
        Transform newHealthIndicatorTr = newHealthIndicatorO.transform;
        newHealthIndicatorTr.parent = GameManager.instance.mainCanvasRectTransform;

        BasicFunctions.ResetTransform(newHealthIndicatorTr);

        RectTransform rTr = newHealthIndicatorO.GetComponent<RectTransform>();
        rTr.localScale = Vector3.one;

        myHealthIndicator = newHealthIndicatorO.GetComponent<HealthIndicator>();
        myHealthIndicator.myNpcCore = this;
        myHealthIndicator.HideAllContent();

        // wakeUp
        wakeUpProgress = .25f;

        // state
        State startState;
        switch ( myType )
        {
            default: startState = State.Roam; break;
            case Type.MagicSkull: startState = State.Sleeping; myGraphics.SetScale(wakeUpProgress); break;
        }
        if (myInfo.stats.hideOnStart)
        {
            InitSetHide();
        }
        else
        {
            SetState(startState);
        }

        // store
        if (!LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs.Contains(this))
        {
            LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs.Add(this);
        }

        // audio
        alertClipsUse = myInfo.audio.alertClips;
        alertPitchMin = myInfo.audio.alertPitchMin;
        alertPitchMax = myInfo.audio.alertPitchMax;
        alertVolumeMin = myInfo.audio.alertVolumeMin;
        alertVolumeMax = myInfo.audio.alertVolumeMax;

        attackPrepareClipsUse = myInfo.audio.attackPrepareClips;
        attackPreparePitchMin = myInfo.audio.attackPreparePitchMin;
        attackPreparePitchMax = myInfo.audio.attackPreparePitchMax;
        attackPrepareVolumeMin = myInfo.audio.attackPrepareVolumeMin;
        attackPrepareVolumeMax = myInfo.audio.attackPrepareVolumeMax;

        attackDoClipsUse = myInfo.audio.attackDoClips;
        attackDoPitchMin = myInfo.audio.attackDoPitchMin;
        attackDoPitchMax = myInfo.audio.attackDoPitchMax;
        attackDoVolumeMin = myInfo.audio.attackDoVolumeMin;
        attackDoVolumeMax = myInfo.audio.attackDoVolumeMax;

        hurtClipsUse = myInfo.audio.hurtClips;
        hurtPitchMin = myInfo.audio.hurtPitchMin;
        hurtPitchMax = myInfo.audio.hurtPitchMax;
        hurtVolumeMin = myInfo.audio.hurtVolumeMin;
        hurtVolumeMax = myInfo.audio.hurtVolumeMax;

        deadClipsUse = myInfo.audio.deadClips;
        deadPitchMin = myInfo.audio.deadPitchMin;
        deadPitchMax = myInfo.audio.deadPitchMax;
        deadVolumeMin = myInfo.audio.deadVolumeMin;
        deadVolumeMax = myInfo.audio.deadVolumeMax;

        // tear drop count
        tearDropCount = myInfo.stats.tearDropCount;

        // block
        blockMinDur = 60;
        blockMaxDur = 90;
        blockDur = Mathf.RoundToInt(TommieRandom.instance.RandomRange(blockMinDur,blockMaxDur));
        blockCounter = 0;

        canBlockDur = 16;
        canBlockCounter = 0;

        // eyes
        lostEyes = false;
        justLostEyesDur = 24;
        justLostEyesCounter = justLostEyesDur;

        // mournful blessing
        if (SetupManager.instance != null && SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Mournful))
        {
            if (TommieRandom.instance.RandomValue(1f) <= .33f)
            {
                tearDropCount++;
            }
        }

        // no tear drop?? :(
        if ( preventTearDrop )
        {
            tearDropCount = 0;
        }

        // alerted
        if (autoAlerted)
        {
            InitSetAlerted();
        }

        // define starting attack
        PickRandomAttack();

        // store boss?
        if ( myType == Type.HellLord )
        {
            LevelGeneratorManager.instance.activeLevelGenerator.curBossCore = this;
        }

        // done
        initialized = true;
    }

    void Update ()
    {
        //if ( myType == Type.MagicSkull )
        //{
        //    Debug.Log("lost eyes: " + lostEyes.ToString() + " || " + Time.time.ToString());
        //}

        if ( curState != State.EnterNextStage )
        {
            SetupManager.instance.clearingLava = false;
        }

        if (initialized)
        {
            if (!SetupManager.instance.inFreeze && LevelGeneratorManager.instance.activeLevelGenerator.generatedLevel )
            {
                // connectedBy is gone?
                if (myType == Type.Faerie)
                {
                    isConnectedToOtherNpc = true;
                    connectedBy = spawnedBy.dungeonBossNpcSpawner.myNpcCores[0];
                    if (isConnectedToOtherNpc && connectedBy == null )
                    {
                        Clear();
                    }
                }

                // check if we're bouncing into player?
                CheckIfBouncingIntoPlayer();

                // just lost eyes?
                if ( justLostEyesCounter < justLostEyesDur )
                {
                    justLostEyesCounter++;
                }

                // can block again?
                if ( canBlockCounter < canBlockDur )
                {
                    canBlockCounter++;
                }

                // state
                switch (curState)
                {
                    case State.Roam:

                        if (!inCombat)
                        {
                            Vector3 pp0 = myTransform.position;
                            Vector3 pp1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                            float dd0 = Vector3.Distance(pp0, pp1);
                            float alertDst = 6f + myInfo.stats.alertDstExtra;
                            alertDst += SetupManager.instance.playerEquipmentStatsTotal.alertDstAdd;
                            if (dd0 <= alertDst)
                            {
                                InitSetAlerted();
                            }
                        }

                        break;

                    case State.Alerted:

                        if (alertedCounter < alertedDur)
                        {
                            alertedCounter++;
                        }
                        else
                        {
                            if (myInfo.stats.fleeFromPlayer)
                            {
                                InitSetFleeing(GameManager.instance.playerFirstPersonDrifter.myTransform);
                            }
                            else
                            {
                                InitSetChasing(GameManager.instance.playerFirstPersonDrifter.myTransform);
                            }
                        }

                        break;

                    case State.Hit:
                        if (hitFlickerCounter < hitFlickerRate)
                        {
                            hitFlickerCounter++;
                        }
                        else
                        {
                            hitFlickerCounter = 0;
                            hitFlickerIndex = (hitFlickerIndex == 0) ? 1 : 0;
                        }

                        if (hitCounter < hitDur)
                        {
                            hitCounter++;
                        }
                        else
                        {
                            StopGetHit();
                        }
                        break;

                    case State.AttackPrepare:

                        if (attackPrepareCounter < (curAttackData.attackPrepareDur * attackPrepareDurFac))
                        {
                            attackPrepareCounter++;

                            Vector3 pp0 = myTransform.position;
                            Vector3 pp1 = (myInfo.stats.fleeFromPlayer) ? fleeTransform.position : chaseTransform.position;
                            Vector3 pp2 = (pp1 - pp0).normalized;
                            pp2.y = 0f;
                            attackPrepareDir = pp2;
                        }
                        else
                        {
                            InitSetAttackDo();
                        }

                        break;

                    case State.AttackDo:

                        if (attackDoCounter < curAttackData.attackDoDur)
                        {
                            // continue
                            attackDoCounter++;

                            // laser is in play?
                            if ( curAttackData.attackType == AttackData.AttackType.Ranged && curAttackData.rangedAttackType == AttackData.RangedAttackType.Laser )
                            {
                                SetupManager.instance.SetLaserAttackInPlay();
                            }

                            // leave fire trail?
                            if ( curAttackData.leaveFireTrail && (attackDoCounter < ((curAttackData.attackDoDur * attackDoDurFac) * .75f)) )
                            {
                                if ( attackFireTrailCounter < curAttackData.fireTrailRate )
                                {
                                    attackFireTrailCounter++;
                                }
                                else
                                {
                                    Vector3 fireTrailP = myGraphics.bodyTransform.position;
                                    Quaternion fireTrailR = Quaternion.identity;
                                    float fireTrailScl = .125f;
                                    GameObject trailProjectileO = PrefabManager.instance.SpawnPrefabAsGameObject(curAttackData.fireTrailProjectilePrefab,fireTrailP,fireTrailR,fireTrailScl);
                                    ProjectileScript trailProjectileScript = trailProjectileO.GetComponent<ProjectileScript>();
                                    if ( trailProjectileScript != null )
                                    {
                                        trailProjectileScript.radius = .5f;
                                        trailProjectileScript.npcCoreBy = this;
                                        trailProjectileScript.SetOwnerType(ProjectileScript.OwnerType.Npc);
                                        trailProjectileScript.speed = 0f;
                                        trailProjectileScript.createFire = true;

                                        float offX = TommieRandom.instance.RandomRange(curAttackData.fireTrailDirOffMin.x, curAttackData.fireTrailDirOffMax.x);
                                        float offY = TommieRandom.instance.RandomRange(curAttackData.fireTrailDirOffMin.y, curAttackData.fireTrailDirOffMax.y);
                                        float offZ = TommieRandom.instance.RandomRange(curAttackData.fireTrailDirOffMin.z, curAttackData.fireTrailDirOffMax.z);
                                        Vector3 off = new Vector3(offX,offY,offZ);
                                        trailProjectileScript.dir = off;
                                        trailProjectileScript.clearDurAdd = 600;
                                    }

                                    attackFireTrailCounter = 0;
                                }
                            }

                            // do attack?
                            if (!attackDealtDamage && curAttackData.attackType == AttackData.AttackType.Melee && curAttackData.damage > 0)
                            {
                                float attackSpawnDamageDealFac = .175f;
                                if (attackDoCounter >= ((curAttackData.attackDoDur * attackDoDurFac) * attackSpawnDamageDealFac))
                                {
                                    Vector3 d0 = myTransform.position;
                                    Vector3 d1 = chaseTransform.position;
                                    d1.y = d0.y;
                                    Vector3 d2 = (d1 - d0).normalized;
                                    Vector3 damageDealP = myTransform.position;
                                    damageDealP += (d2 * .5f);
                                    damageDealP.y += .5f;

                                    damageDealP += (myGraphics.graphicsTransform.forward * curAttackData.damageDealSpawnLocalAdd.z);
                                    damageDealP += (myGraphics.graphicsTransform.right * curAttackData.damageDealSpawnLocalAdd.x);
                                    damageDealP += (myGraphics.graphicsTransform.up * curAttackData.damageDealSpawnLocalAdd.y);

                                    PrefabManager.instance.SpawnDamageDeal(damageDealP, 1.5f + curAttackData.damageDealExtraRadius, curAttackData.damage, curAttackData.damageType, 10, myTransform, 1f, true, DamageDeal.Target.Player,this,false,false);

                                    attackDealtDamage = true;
                                }
                            }
                        }
                        else
                        {
                            StopAttackDo();
                        }

                        break;

                    case State.Block:

                        if ( blockCounter < blockDur )
                        {
                            blockCounter++;
                        }
                        else
                        {
                            StopBlock();
                        }

                        break;

                    case State.Stunned:

                        if ( stunnedCounter < stunnedDur )
                        {
                            stunnedCounter++;
                        }
                        else
                        {
                            StopGetStunned();
                        }

                        break;

                    case State.Vulnerable:

                        if (vulnerableCounter < vulnerableDur)
                        {
                            vulnerableCounter++;
                        }
                        else
                        {
                            StopGetVulnerable();
                        }

                        break;


                    case State.CollectFire:

                        {
                            if (collectFireCounter < collectFireDur)
                            {
                                collectFireCounter++;

                                // stop looking?
                                if (collectFireCounter >= (collectFireDur / 3))
                                {
                                    bool stopLooking = false;
                                    if (!GameManager.instance.CheckIfFireInRadius(myTransform.position, 10f, 1))
                                    {
                                        stopLooking = true;
                                    }

                                    // look for nearby fire?
                                    if (!stopLooking)
                                    {
                                        CollectFire(10f,false);
                                    }
                                    else
                                    {
                                        if (collectFireCounter < (collectFireDur - 60))
                                        {
                                            collectFireCounter = (collectFireDur - 60);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                StopCollectFire();
                            }
                        }

                        break;


                    case State.EnterNextStage:

                        if (enterNextStageCounter < enterNextStageDur)
                        {
                            clearingLava = false;
                            enterNextStageCounter++;

                            if (enterNextStageCounter >= (enterNextStageDur / 4))
                            {
                                SetupManager.instance.clearingLava = true;
                                clearingLava = true;
                                clearedLava = true;

                                // collect all fire
                                CollectFire(200f,true);

                                // robes on fire
                                if (!enteredNextStage)
                                {
                                    // set robes on fire?
                                    if (myGraphics != null && myGraphics.robeMeshRenderers != null && myGraphics.robeMeshRenderers.Length > 0)
                                    {
                                        for (int i = 0; i < myGraphics.robeMeshRenderers.Length; i++)
                                        {
                                            MeshRenderer curRobeMeshRenderer = myGraphics.robeMeshRenderers[i];
                                            Material[] robeMats = curRobeMeshRenderer.materials;
                                            for (int ii = 0; ii < robeMats.Length; ii++)
                                            {
                                                robeMats[ii].SetFloat("_OnFire", 1f);
                                            }
                                            curRobeMeshRenderer.materials = robeMats;
                                        }
                                    }
                                }

                                enteredNextStage = true;
                            }
                        }
                        else
                        {
                            //SetupManager.instance.clearingLava = false;
                            clearingLava = false;
                            StopEnterNextStage();
                        }

                        break;

                    case State.WakeUp:

                        if (wakeUpProgress < 1f)
                        {
                            float wakeUpScaleSpd = (.25f * Time.deltaTime);
                            wakeUpProgress += wakeUpScaleSpd;
                        }
                        else
                        {
                            wakeUpProgress = 1f;
                        }
                        myGraphics.SetScale(wakeUpProgress);

                        if (wakeUpCounter < wakeUpDur)
                        {
                            wakeUpCounter++;
                        }
                        else
                        {
                            StopSleeping();
                        }

                        break;

                    case State.Laugh:

                        if (laughCounter < laughDur)
                        {
                            laughCounter ++;
                        }
                        else
                        {
                            didLaugh = true;
                            SetState(State.Chase);

                            NpcCore dungeonBossCore = spawnedBy.dungeonBossNpcSpawner.myNpcCores[0];
                            if (dungeonBossCore.curState == State.Sleeping)
                            {
                                Vector3 dungeonBossP = dungeonBossCore.myTransform.position;
                                InitSetMoveToPosition(dungeonBossP,State.WakeDungeonBoss, 4f);
                                myUnit.LookForPath(dungeonBossP, Grid.instance);
                            }
                        }

                        break;

                    case State.Hide:

                        {
                            if (hideCounter < hideDur)
                            {
                                Vector3 ppp0 = myTransform.position;
                                Vector3 ppp1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                                ppp1.y = ppp0.y;
                                float ddd0 = Vector3.Distance(ppp0,ppp1);
                                float hideContinueThreshold = 14f;
                                if (ddd0 <= hideContinueThreshold)
                                {
                                    hideCounter++;
                                }
                            }
                            else
                            {
                                StopHide();
                            }
                        }

                        break;
                }

                // hit dir
                float forceDirTargetLerpie = 10f;
                if ( curState == State.Hit || curState == State.Stunned )
                {
                    forceDirTargetLerpie = 7.5f;
                }
                float forceDirCurLerpie = 5f;

                forceDirTarget = Vector3.Lerp(forceDirTarget, Vector3.zero, forceDirTargetLerpie * Time.deltaTime);
                forceDirCur = Vector3.Lerp(forceDirCur, forceDirTarget, forceDirCurLerpie * Time.deltaTime);

                float c0 = .25f;
                float y0 = .125f;
                Vector3 p0 = myTransform.position;
                Vector3 p1 = p0;
                p0.y += y0;
                p0 += forceDirCur.normalized * -c0;
                p1.y += y0;
                p1 += forceDirCur.normalized * c0;
                RaycastHit cHit;
                if (Physics.Linecast(p0, p1, out cHit, bounceLayerMask))
                {
                    float m0 = forceDirTarget.magnitude;
                    Vector3 r0 = forceDirTarget.normalized;
                    Vector3 r1 = -cHit.normal.normalized;
                    r1.y = r0.y;
                    Vector3 r2 = Vector3.Reflect(r0, r1).normalized;
                    forceDirTarget = r2 * m0;
                    forceDirTarget.y = 0f;
                    forceDirCur = forceDirTarget;

                    // particles
                    Vector3 particlePoint = cHit.point;
                    particlePoint.y = particleSpawnPoint.y;
                    PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, particlePoint, Quaternion.identity, 1.25f);

                    // create impact particles
                    PrefabManager.instance.SpawnPrefab(PrefabManager.instance.magicImpactParticlesPrefab[2], particlePoint, Quaternion.identity, 1f);

                    // hit vernietigbaar?
                    if ( cHit.transform.GetComponent<Vernietigbaar>() != null )
                    {
                        Vernietigbaar hitVernietigbaar = cHit.transform.GetComponent<Vernietigbaar>();
                        if ( hitVernietigbaar != null )
                        {
                            hitVernietigbaar.Destroy();
                        }
                    }

                    // log
                    Debug.DrawLine(cHit.point, cHit.point + r0, Color.magenta, 5f);
                    Debug.DrawLine(cHit.point, cHit.point + r1, Color.cyan, 5f);
                    Debug.DrawLine(cHit.point, cHit.point + r2, Color.green, 5f);
                }
                Debug.DrawLine(p0,p1,Color.red);

                // particles
                particleSpawnPoint = myTransform.position + (Vector3.up * .25f);
            }
        }
    }

    void CollectFire ( float _radius, bool _fromLava )
    {
        if (GameManager.instance != null && GameManager.instance.fireScripts != null && GameManager.instance.fireScripts.Count > 0)
        {
            for (int i = 0; i < GameManager.instance.fireScripts.Count; i++)
            {
                FireScript fireScriptCheck = GameManager.instance.fireScripts[i];
                if (fireScriptCheck != null && fireScriptCheck.npcConnectedTo != this)
                {
                    Vector3 pp0 = myTransform.position;
                    Vector3 pp1 = fireScriptCheck.myTransform.position;
                    pp1.y = pp0.y;
                    float dd0 = Vector3.Distance(pp0, pp1);
                    if (dd0 <= _radius)
                    {
                        bool doCollect = (TommieRandom.instance.RandomValue(1f) <= .025f);
                        if (doCollect)
                        {
                            fireScriptCheck.Collect(false, this);
                        }
                    }
                }
            }
        }

        if ( _fromLava )
        {
            SetupManager.instance.SetScreenShake(1);

            SetupManager.instance.lavaFactor = Mathf.Lerp(SetupManager.instance.lavaFactor,0f,.25f * Time.deltaTime);

            if (lavaCollectCreateCounter < lavaCollectCreateRate)
            {
                lavaCollectCreateCounter++;
            }
            else
            {
                Vector3 p0 = spawnedBy.enterNextStageMovePoint.myTransform.position;
                Vector3 p1 = TommieRandom.instance.RandomInsideSphere();
                p1.y = 0f;
                p1.Normalize();
                Vector3 p2 = p0 + (p1 * TommieRandom.instance.RandomRange(20f,40f));

                GameObject fireCollectO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.fireCollectPrefab,p2,Quaternion.identity, 1f);
                FireCollectScript fireCollectScript = fireCollectO.GetComponent<FireCollectScript>();
                if (fireCollectScript != null)
                {
                    fireCollectScript.targetTransform = fireCollectPointTransform;
                    fireCollectScript.npcCollectedBy = this;
                }

                lavaCollectCreateCounter = 0;
            }
        }
    }

    void CheckIfBouncingIntoPlayer ()
    {
        if (curState == State.AttackDo)
        {
            if (forceDirCur.magnitude > .0125f)
            {
                float cHeight = GameManager.instance.playerFirstPersonDrifter.myTransform.position.y + .25f;
                float cDst = 1f + myInfo.stats.bounceIntoPlayerCheckDst;
                Vector3 c0 = myTransform.position;
                c0.y = cHeight;
                c0 += (forceDirCur.normalized * -cDst);
                Vector3 c1 = c0;
                c1 += (forceDirCur.normalized * cDst);
                if (Physics.Linecast(c0, c1, SetupManager.instance.playerLayerMask))
                {
                    //DampForce(.025f);
                    float dampFac = .125f;
                    forceDirTarget *= dampFac;
                    forceDirCur *= dampFac;

                    //Debug.Log("dampen die boel! || " + Time.time.ToString());
                }
                Debug.DrawLine(c0, c1, Color.cyan);
            }
        }
    }

    public void IncreaseStageIndex ()
    {
        stageIndex++;

        // robes of hell lord?
        
    }

    public void Heal ( int _amount )
    {
        if (health > 0)
        {
            int maxHealth = myInfo.stats.health;
            if (health < maxHealth)
            {
                health += _amount;
            }
            if (health > maxHealth)
            {
                health = maxHealth;
            }

            // create UI text
            Vector3 indicatorHeightOff = (Vector3.up * myInfo.graphics.healthIndicatorOff);
            if (myInfo.graphics.flying)
            {
                Vector3 flyOffAdd = (Vector3.up * myGraphics.flyOff);
                indicatorHeightOff += flyOffAdd;
            }
            Vector3 damageIndicatorP = myGraphics.graphicsTransform.position + indicatorHeightOff;

            Transform damageIndicatorOffTr = GameManager.instance.mainCameraTransform;
            damageIndicatorP += (damageIndicatorOffTr.forward * myInfo.graphics.damageIndicatorLocalAdd.z);
            damageIndicatorP += (damageIndicatorOffTr.right * myInfo.graphics.damageIndicatorLocalAdd.x);
            damageIndicatorP += (damageIndicatorOffTr.up * myInfo.graphics.damageIndicatorLocalAdd.y);
            GameManager.instance.CreateDamageIndicatorString("heal", damageIndicatorP);

            // health indicator flicker
            if (myHealthIndicator != null)
            {
                myHealthIndicator.InitFlicker(8);
            }

            // heal audio
        }
    }

    public void DealDamage ( int _amount, AttackData.DamageType _type )
    {
        int dmgAmount = _amount;

        if ( SetupManager.instance.playerStrong )
        {
            dmgAmount = 100;
        }

        // check for damage boosting blessings
        switch ( _type )
        {
            case AttackData.DamageType.Melee: if ( SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Warrior) ) { dmgAmount+=BlessingDatabase.instance.warriorDamageBoost; }; break;
            case AttackData.DamageType.Magic: if ( SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Sorcerer) ) { dmgAmount += BlessingDatabase.instance.sorcererDamageBoost; ; }; break;
        }

        // melee damage
        if (_type == AttackData.DamageType.Melee)
        {
            if ( SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Sorcerer) )
            {
                dmgAmount--;
            }
            dmgAmount += SetupManager.instance.playerEquipmentStatsTotal.meleeDamageAdd;
        }

        // magic damage
        if ( _type == AttackData.DamageType.Magic )
        {
            if (SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Warrior))
            {
                dmgAmount--;
            }
            if (!SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Spray))
            {
                dmgAmount += 1;
            }
            if ( SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.HotFire) )
            {
                dmgAmount += 1;
            }
            dmgAmount += SetupManager.instance.playerEquipmentStatsTotal.magicDamageAdd;
        }

        if ( SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.GlassCannon) )
        {
            dmgAmount+=BlessingDatabase.instance.glassCannonDamageBoost;
        }

        if ( dmgAmount < 1 )
        {
            dmgAmount = 1;
        }

        if (health > 0)
        {
            health -= dmgAmount;
        }

        // heal?


        // audio?
        if ( health > 0 )
        {
            PlayHurtAudio();
        }
        else
        {
            //audioSourceTransform.parent = null;
            PlayDeadAudio();
            //Destroy(audioSourceGameObject,2f);
        }

        // set alerted (if not already)
        //if (!inCombat)
        //{
        //    InitSetAlerted();
        //}

        // particles
        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, particleSpawnPoint, Quaternion.identity, 1.5f);

        // create magic impact effect
        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.magicImpactParticlesPrefab[2], particleSpawnPoint, Quaternion.identity, 1f);

        // create indicator prefab
        Vector3 indicatorHeightOff = (Vector3.up * myInfo.graphics.healthIndicatorOff);
        if ( myInfo.graphics.flying )
        {
            Vector3 flyOffAdd = (Vector3.up * myGraphics.flyOff);
            indicatorHeightOff += flyOffAdd;
        }
        Vector3 damageIndicatorP = myGraphics.graphicsTransform.position + indicatorHeightOff;

        Transform damageIndicatorOffTr = GameManager.instance.mainCameraTransform;
        damageIndicatorP += (damageIndicatorOffTr.forward * myInfo.graphics.damageIndicatorLocalAdd.z);
        damageIndicatorP += (damageIndicatorOffTr.right * myInfo.graphics.damageIndicatorLocalAdd.x);
        damageIndicatorP += (damageIndicatorOffTr.up * myInfo.graphics.damageIndicatorLocalAdd.y);

        GameManager.instance.CreateDamageIndicator(dmgAmount,damageIndicatorP);

        // impact type
        AudioManager.instance.PlayAttackImpactSound(_type);

        // regrow fire charges?
        if (_amount > 0)
        {
            Invoke("RequestFireChargeRegrows", .5f);
        }

        // health indicator flicker
        if (myHealthIndicator != null)
        {
            myHealthIndicator.InitFlicker(16);
        }

        // set in combat
        if ( !inCombat )
        {
            chaseTransform = GameManager.instance.playerFirstPersonDrifter.myTransform;
            inCombat = true;
        }

        // log
        //Debug.Log("au ik krijg " + _amount.ToString() + " " + _type + " schade! || " + Time.time.ToString());
    }

    void RequestFireChargeRegrows ()
    {
        if (myInfo.graphics.eyeHasFireCharge)
        {
            for (int i = 0; i < myGraphics.eyeFireChargeScripts.Count; i++)
            {
                FireChargeScript fireChargeScriptCheck = myGraphics.eyeFireChargeScripts[i];
                if (fireChargeScriptCheck != null && fireChargeScriptCheck.regrowWhenNpcGotHit)
                {
                    fireChargeScriptCheck.Regrow();
                }
            }
        }
        if (myInfo.graphics.mouthHasFireCharge)
        {
            for (int i = 0; i < myGraphics.mouthFireChargeScripts.Count; i++)
            {
                FireChargeScript fireChargeScriptCheck = myGraphics.mouthFireChargeScripts[i];
                if (fireChargeScriptCheck != null && fireChargeScriptCheck.regrowWhenNpcGotHit)
                {
                    fireChargeScriptCheck.Regrow();
                }
            }
        }
    }

    public void SetDead ()
    {
        SetupManager.instance.clearingLava = false;

        for ( int i = 0; i < tearDropCount; i ++ )
        {
            GameObject tearO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.tearPrefab[0], myTransform.position, Quaternion.identity, 1f);
            Stuiterbaar stuiterbaarScript = tearO.GetComponent<Stuiterbaar>();
            if (stuiterbaarScript != null)
            {
                float spawnSideForceOffMax = .05f;
                float spawnUpForceOffMax = .075f;
                float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f,1f));
                float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                float xAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                float zAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                stuiterbaarScript.forceCur.x += (xAdd * xDir);
                stuiterbaarScript.forceCur.y += TommieRandom.instance.RandomRange(spawnUpForceOffMax * .5f,spawnUpForceOffMax);
                stuiterbaarScript.forceCur.z += (zAdd * zDir);
            }
        }

        if (myType == Type.Skeleton || myType == Type.RedSkeleton || myType == Type.BlackSkeleton)
        {
            GameObject bonePrefab = null;
            GameObject skullPrefab = null;
            switch (myType)
            {
                case Type.Skeleton: bonePrefab = PrefabManager.instance.whiteBonePrefab[0]; skullPrefab = PrefabManager.instance.whiteSkullPrefab[0]; break;
                case Type.RedSkeleton: bonePrefab = PrefabManager.instance.redBonePrefab[0]; skullPrefab = PrefabManager.instance.redSkullPrefab[0]; break;
                case Type.BlackSkeleton: bonePrefab = PrefabManager.instance.blackBonePrefab[0]; skullPrefab = PrefabManager.instance.blackSkullPrefab[0]; break;
            }

            // spawn bones
            int boneCount = 4;
            for (int ii = 0; ii < boneCount; ii++)
            {
                GameObject boneO = PrefabManager.instance.SpawnPrefabAsGameObject(bonePrefab, myTransform.position + (Vector3.up * .5f), Quaternion.identity, 1f);
                Stuiterbaar stuiterbaarScript = boneO.GetComponent<Stuiterbaar>();
                if (stuiterbaarScript != null)
                {
                    float spawnSideForceOffMax = .05f;
                    float spawnUpForceOffMax = .075f;
                    float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                    float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                    float xAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                    float zAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                    stuiterbaarScript.forceCur.x += (xAdd * xDir);
                    stuiterbaarScript.forceCur.y += TommieRandom.instance.RandomRange(spawnUpForceOffMax * .5f, spawnUpForceOffMax);
                    stuiterbaarScript.forceCur.z += (zAdd * zDir);
                    stuiterbaarScript.autoClearDur = Mathf.RoundToInt(TommieRandom.instance.RandomRange(240f, 300f));
                }
            }

            // spawn skull
            int skullCount = 1;
            for (int ii = 0; ii < skullCount; ii++)
            {
                GameObject skullO = PrefabManager.instance.SpawnPrefabAsGameObject(skullPrefab, myTransform.position + (Vector3.up * .5f), Quaternion.identity, 1f);
                Stuiterbaar stuiterbaarScript = skullO.GetComponent<Stuiterbaar>();
                if (stuiterbaarScript != null)
                {
                    float spawnSideForceOffMax = .05f;
                    float spawnUpForceOffMax = .075f;
                    float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                    float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                    float xAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                    float zAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                    stuiterbaarScript.forceCur.x += (xAdd * xDir);
                    stuiterbaarScript.forceCur.y += TommieRandom.instance.RandomRange(spawnUpForceOffMax * .5f, spawnUpForceOffMax);
                    stuiterbaarScript.forceCur.z += (zAdd * zDir);
                    stuiterbaarScript.autoClearDur = Mathf.RoundToInt(TommieRandom.instance.RandomRange(240f,300f));
                }
            }
        }

        if ( spawnedBy != null )
        {
            spawnedBy.myNpcsDefeated[spawnedByIndex] = true;
        }

        // is a minion?
        if ( minionMaster != null )
        {
            minionMaster.RemoveMinion(this);
        }

        // drop a key?
        bool dropKey = false;
        if ( myType == Type.RatKing || myType == Type.MagicSkull )
        {
            dropKey = true;
        }
        if ( dropKey )
        {
            Vector3 keySpawnPos = myTransform.position + (Vector3.up * .75f);
            for (int i = 0; i < 1; i++)
            {
                GameObject keyO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.keyPrefab[0], keySpawnPos, Quaternion.identity, 1f);
                Stuiterbaar stuiterbaarScript = keyO.GetComponent<Stuiterbaar>();
                if (stuiterbaarScript != null)
                {
                    float spawnSideForceOffMax = .05f;
                    float spawnUpForceOffMax = .075f;
                    float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                    float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                    float xAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                    float zAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                    stuiterbaarScript.forceCur.x += (xAdd * xDir);
                    stuiterbaarScript.forceCur.y += TommieRandom.instance.RandomRange(spawnUpForceOffMax * .5f, spawnUpForceOffMax);
                    stuiterbaarScript.forceCur.z += (zAdd * zDir);
                }
            }
        }

        // explode?
        if ( SetupManager.instance.CheckIfItemSpecialActive(EquipmentDatabase.Specials.EnemiesExplodeOnDefeat) )
        {
            PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab,myTransform.position,Quaternion.identity,2f);
            PrefabManager.instance.SpawnDamageDeal(myTransform.position, 2f, 1, AttackData.DamageType.Magic, 10, HandManager.instance.myTransform, .325f, true, DamageDeal.Target.AI, null, false, false);
            AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.cannonImpactClips), .9f, 1.1f, .3f, .325f);

            //// create UI text
            //Vector3 indicatorHeightOff = (Vector3.up * myInfo.graphics.healthIndicatorOff);
            //if (myInfo.graphics.flying)
            //{
            //    Vector3 flyOffAdd = (Vector3.up * myGraphics.flyOff);
            //    indicatorHeightOff += flyOffAdd;
            //}
            //Vector3 damageIndicatorP = myGraphics.graphicsTransform.position + indicatorHeightOff;

            //Transform damageIndicatorOffTr = GameManager.instance.mainCameraTransform;
            //damageIndicatorP += (damageIndicatorOffTr.forward * myInfo.graphics.damageIndicatorLocalAdd.z);
            //damageIndicatorP += (damageIndicatorOffTr.right * myInfo.graphics.damageIndicatorLocalAdd.x);
            //damageIndicatorP += (damageIndicatorOffTr.up * myInfo.graphics.damageIndicatorLocalAdd.y);
            //GameManager.instance.CreateDamageIndicatorString("explode", damageIndicatorP);
        }

        // drop extra tear?
        if (SetupManager.instance.CheckIfItemSpecialActive(EquipmentDatabase.Specials.EnemiesDropExtraTear))
        {
            GameObject tearO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.tearPrefab[0], myTransform.position, Quaternion.identity, 1f);
            Stuiterbaar stuiterbaarScript = tearO.GetComponent<Stuiterbaar>();
            if (stuiterbaarScript != null)
            {
                float spawnSideForceOffMax = .05f;
                float spawnUpForceOffMax = .075f;
                float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                float xAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                float zAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                stuiterbaarScript.forceCur.x += (xAdd * xDir);
                stuiterbaarScript.forceCur.y += TommieRandom.instance.RandomRange(spawnUpForceOffMax * .5f, spawnUpForceOffMax);
                stuiterbaarScript.forceCur.z += (zAdd * zDir);
            }
        }

        // drop extra coin?
        if (SetupManager.instance.CheckIfItemSpecialActive(EquipmentDatabase.Specials.EnemiesDropExtraCoin))
        {
            GameObject tearO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.coinPrefab[0], myTransform.position, Quaternion.identity, 1f);
            Stuiterbaar stuiterbaarScript = tearO.GetComponent<Stuiterbaar>();
            if (stuiterbaarScript != null)
            {
                float spawnSideForceOffMax = .05f;
                float spawnUpForceOffMax = .075f;
                float xDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                float zDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f, 1f));
                float xAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                float zAdd = TommieRandom.instance.RandomRange(spawnSideForceOffMax * .25f, spawnSideForceOffMax);
                stuiterbaarScript.forceCur.x += (xAdd * xDir);
                stuiterbaarScript.forceCur.y += TommieRandom.instance.RandomRange(spawnUpForceOffMax * .5f, spawnUpForceOffMax);
                stuiterbaarScript.forceCur.z += (zAdd * zDir);
            }
        }

        // draaaaaain gang
        if ( SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.Drainer) )
        {
            if ( TommieRandom.instance.RandomValue(1f) <= .05f )
            {
                GameManager.instance.AddPlayerHealth(1);

                // create UI text
                Vector3 indicatorHeightOff = (Vector3.up * myInfo.graphics.healthIndicatorOff);
                if (myInfo.graphics.flying)
                {
                    Vector3 flyOffAdd = (Vector3.up * myGraphics.flyOff);
                    indicatorHeightOff += flyOffAdd;
                }
                Vector3 damageIndicatorP = myGraphics.graphicsTransform.position + indicatorHeightOff;

                Transform damageIndicatorOffTr = GameManager.instance.mainCameraTransform;
                damageIndicatorP += (damageIndicatorOffTr.forward * myInfo.graphics.damageIndicatorLocalAdd.z);
                damageIndicatorP += (damageIndicatorOffTr.right * myInfo.graphics.damageIndicatorLocalAdd.x);
                damageIndicatorP += (damageIndicatorOffTr.up * myInfo.graphics.damageIndicatorLocalAdd.y);
                GameManager.instance.CreateDamageIndicatorString("drained", damageIndicatorP);
            }
        }

        // drain item effect?
        if (SetupManager.instance.CheckIfItemSpecialActive(EquipmentDatabase.Specials.DrainHealth))
        {
            GameManager.instance.AddPlayerHealth(1);

            // create UI text
            Vector3 indicatorHeightOff = (Vector3.up * myInfo.graphics.healthIndicatorOff);
            if (myInfo.graphics.flying)
            {
                Vector3 flyOffAdd = (Vector3.up * myGraphics.flyOff);
                indicatorHeightOff += flyOffAdd;
            }
            Vector3 damageIndicatorP = myGraphics.graphicsTransform.position + indicatorHeightOff;

            Transform damageIndicatorOffTr = GameManager.instance.mainCameraTransform;
            damageIndicatorP += (damageIndicatorOffTr.forward * myInfo.graphics.damageIndicatorLocalAdd.z);
            damageIndicatorP += (damageIndicatorOffTr.right * myInfo.graphics.damageIndicatorLocalAdd.x);
            damageIndicatorP += (damageIndicatorOffTr.up * myInfo.graphics.damageIndicatorLocalAdd.y);
            GameManager.instance.CreateDamageIndicatorString("drained", damageIndicatorP);
        }

        // fire charge item effect?
        if (SetupManager.instance.CheckIfItemSpecialActive(EquipmentDatabase.Specials.FireCharge))
        {
            GameManager.instance.AddPlayerFire(1);

            // create UI text
            Vector3 indicatorHeightOff = (Vector3.up * myInfo.graphics.healthIndicatorOff);
            if (myInfo.graphics.flying)
            {
                Vector3 flyOffAdd = (Vector3.up * myGraphics.flyOff);
                indicatorHeightOff += flyOffAdd;
            }
            Vector3 damageIndicatorP = myGraphics.graphicsTransform.position + indicatorHeightOff;

            Transform damageIndicatorOffTr = GameManager.instance.mainCameraTransform;
            damageIndicatorP += (damageIndicatorOffTr.forward * myInfo.graphics.damageIndicatorLocalAdd.z);
            damageIndicatorP += (damageIndicatorOffTr.right * myInfo.graphics.damageIndicatorLocalAdd.x);
            damageIndicatorP += (damageIndicatorOffTr.up * myInfo.graphics.damageIndicatorLocalAdd.y);
            GameManager.instance.CreateDamageIndicatorString("charged", damageIndicatorP);
        }

        // audio
        AudioManager.instance.PlaySoundAtPosition(myTransform.position,BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.enemyClearClips),.9f,1.1f,.3f,.325f);

        // freeze because it was an important defeat??
        if ( myType == Type.RatKing || myType == Type.MagicSkull || myType == Type.HellLord )
        {
            SetupManager.instance.SetFreeze(SetupManager.instance.bossDefeatFreeze);
        }

        // was final boss?
        if (!SetupManager.instance.defeatedFinalBoss)
        {
            if (myType == Type.HellLord)
            {
                SetupManager.instance.defeatedFinalBoss = true;
                SetupManager.instance.RequestProceedToOutro();
            }
        }

        // doeg
        Clear();
    }

    public void SetState ( State _to )
    {
        curState = _to;

        switch ( _to )
        {
            case State.WakeDungeonBoss:
                InitWakeDungeonBoss();
            break;
        }

        // log
        //if (myType == Type.HellLord)
        //{
        //    Debug.Log("set " + myInfo.stats.name + " state to: " + _to + " || " + Time.time.ToString());
        //}
    }

    public void PickRandomAttack ()
    {
        switch (myType)
        {
            default: PickRandomAttackData(); break;
            case Type.MagicSkull: MagicSkullPickAttackData(); break;
            case Type.HellLord: HellLordPickAttackData(); break;
        }
    }

    public void StopSleeping ()
    {
        myGraphics.SetScale(1f);
        SetState(State.Roam);
    }

    public void InitGetStunned ( Vector3 _dir, float _speed, int _duration )
    {
        PickRandomAttack();

        forceDirLerpie = 2.5f;

        AddForce(_dir,_speed);

        stunnedDur = _duration;
        stunnedCounter = 0;

        myUnit.StopMoving();

        SetState(State.Stunned);

        // audio
        PlayHurtAudio();
    }

    public void StopGetStunned()
    {
        forceDirTarget = Vector3.zero;
        forceDirCur = forceDirTarget;

        if (inCombat)
        {
            SetState((!myInfo.stats.fleeFromPlayer) ? State.Chase : State.Flee);
        }
        else
        {
            SetState(State.Roam);
        }
    }

    public void InitGetVulnerable ( int _duration )
    {
        PickRandomAttack();

        vulnerableDur = _duration;
        vulnerableCounter = 0;

        myUnit.StopMoving();

        SetState(State.Vulnerable);

        // audio
        PlayHurtAudio();
    }

    public void StopGetVulnerable ()
    {
        if (inCombat)
        {
            SetState((!myInfo.stats.fleeFromPlayer) ? State.Chase : State.Flee);
        }
        else
        {
            SetState(State.Roam);
        }
    }

    public void InitSetCollectFire ()
    {
        // define starting attack
        PickRandomAttack();

        fireCollectedCount = 0;

        collectFireDur = 300;
        collectFireCounter = 0;

        forceDirLerpie = 2.5f;

        myUnit.StopMoving();

        SetState(State.CollectFire);
    }

    public void StopCollectFire ()
    {
        PickRandomAttack();

        if (inCombat)
        {
            SetState((!myInfo.stats.fleeFromPlayer) ? State.Chase : State.Flee);
        }
        else
        {
            SetState(State.Roam);
        }
    }

    public void InitSetEnterNextStage ()
    {
        PickRandomAttack();

        enterNextStageDur = 900;
        enterNextStageCounter = 0;

        enteredNextStage = false;

        forceDirLerpie = 2.5f;

        myUnit.StopMoving();

        SetState(State.EnterNextStage);

        // log
        //Debug.Log("enter next stage! || " + Time.time.ToString());
    }

    public void StopEnterNextStage ()
    {
        PickRandomAttack();

        enteredNextStage = false;

        if (inCombat)
        {
            SetState((!myInfo.stats.fleeFromPlayer) ? State.Chase : State.Flee);
        }
        else
        {
            SetState(State.Roam);
        }
    }

    public void InitGetHit ( Vector3 _dir, float _speed, int _duration, bool _immune )
    {
        PickRandomAttack();

        forceDirLerpie = 2.5f;

        hitButImmune = _immune;
        hitDur = _duration;
        hitCounter = 0;
        hitFlickerIndex = 0;
        hitFlickerCounter = 0;

        myUnit.StopMoving();

        SetState(State.Hit);

        Vector3 d = _dir;
        float sideDir = Mathf.Sign(TommieRandom.instance.RandomRange(-1f,1f));
        float sideOffMax = .25f;
        float sideAdd = TommieRandom.instance.RandomRange(0f,sideOffMax) * sideDir;
        d += (myGraphics.graphicsTransform.right * sideAdd);
        AddForce(d,_speed * .375f);

        // faerie?
        if (myType == Type.Faerie)
        {
            NpcCore dungeonBossCore = spawnedBy.dungeonBossNpcSpawner.myNpcCores[0];
            if (dungeonBossCore.curState == State.Sleeping)
            {
                didLaugh = false;
            }
        }
    }

    public void StopGetHit ()
    {
        forceDirTarget = Vector3.zero;
        forceDirCur = forceDirTarget;

        if (health <= 0)
        {
            SetDead();
        }
        else
        {
            bool preventDefaultBehaviour = false;

            // next stage?
            switch (myType)
            {
                case Type.HellLord:

                    if (stageIndex == 0)
                    {
                        bool enterNextStage = (health <= (myInfo.stats.health / 2));
                        if (enterNextStage)
                        {
                            stageIndex++;

                            InitSetEnterNextStage();

                            preventDefaultBehaviour = true;
                        }
                    }

                    break;
            }

            if (!preventDefaultBehaviour)
            {
                if (inCombat)
                {
                    SetState((!myInfo.stats.fleeFromPlayer) ? State.Chase : State.Flee);
                }
                else
                {
                    SetState(State.Roam);
                }
            }
        }
    }

    public void InitSetHide ()
    {
        hideCounter = 0;
        hideDur = 180;
        SetState(State.Hide);
        myGraphics.SetGraphicsObject(false);
    }

    public void StopHide ()
    {
        myGraphics.SetGraphicsObject(true);
        SetState(State.Roam);

        // 1 witte orb svp
        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab,myGraphics.bodyTransform.position,Quaternion.identity,3f);
    }

    public void InitSetLaugh ()
    {
        laughCounter = 0;
        laughDur = 120;
        SetState(State.Laugh);

        // audio
        AudioManager.instance.PlaySoundAtPosition(myTransform.position + myInfo.stats.hitBoxCenterOff,BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.faerieLaughClips),.7f,.8f,.3f,.325f,2f + myInfo.audio.distanceAdd,8f + myInfo.audio.distanceAdd);
    }

    public void InitSetWakeUp ()
    {
        wakeUpCounter = 0;
        wakeUpDur = 180;
        SetState(State.WakeUp);

        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab,myTransform.position,Quaternion.identity,3f);
        SetupManager.instance.SetFreeze(24);

        // audio
        AudioManager.instance.PlaySoundAtPosition(myTransform.position + myInfo.stats.hitBoxCenterOff, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.magicSkullAwokenClips), .6f, .7f, .5f, .525f, 2f + myInfo.audio.distanceAdd, 8f + myInfo.audio.distanceAdd);
    }

    public void InitSetAlerted ()
    {
        myUnit.StopMoving();

        alertedDur = 24;
        alertedCounter = 0;
        SetState(State.Alerted);
        inCombat = true;

        // create indicator
        CreateAlertIndicator();

        // audio
        PlayAlertedAudio();
    }

    public void InitSetChasing ( Transform _targetTransform )
    {
        chaseTransform = _targetTransform;
        SetState(State.Chase);

        // store
        AddPlayerInCombatWith();
    }

    public void InitSetFleeing ( Transform _targetTransform )
    {
        fleeTransform = _targetTransform;
        SetState(State.Flee);

        // store
        AddPlayerInCombatWith();
    }

    public void SetCannonTarget ( CannonScript _cannonScriptTarget )
    {
        cannonScriptTarget = _cannonScriptTarget;
    }

    public void InitSetCannonLight (CannonScript _cannonScriptTarget )
    {
        PickSpecificAttackData(Npc.AttackData.AttackType.Ranged);

        Vector3 cannonP = cannonScriptTarget.ropeLinePointsCur[cannonScriptTarget.ropeLinePointsCur.Length - 1];
        InitSetAttackPrepare(true, cannonP);

        SetState(State.LightCannon);
    }

    public void InitSetMoveToPosition ( Vector3 _pos, State _endState, float _stopDst )
    {
        moveToPositionTargetPoint = _pos;
        moveToPositionEndState = _endState;
        moveToPositionStopDst = _stopDst;

        myUnit.moveToPositionWaitDur = 30;
        myUnit.moveToPositionWaitCounter = 0;

        SetState(State.MoveToPosition);
    }

    public void InitSetAttackPrepare ( bool _overrideAttackPoint, Vector3 _attackPointOverride )
    {
        if (myInfo.attacks != null && myInfo.attacks.Length > 0)
        {
            if (_overrideAttackPoint)
            {
                attackPointOverride = _attackPointOverride;
            }
            overrideAttackPoint = _overrideAttackPoint;

            attackPrepareCounter = 0;
            myUnit.StopMoving();
            SetState(State.AttackPrepare);

            // audio
            PlayAttackPrepareAudio();
        }
    }

    public void InitSetAttackDo ()
    {
        forceDirLerpie = 10f;

        attackDir = myGraphics.graphicsTransform.forward; //myTransform.forward;
        attackDir.y = 0f;

        attackFireTrailCounter = curAttackData.fireTrailRate;

        attackDoCounter = 0;
        attackDealtDamage = false;
        SetState(State.AttackDo);

        if ( curAttackData.attackType == AttackData.AttackType.MinionSpawn )
        {
            if ( minionSpawnTransformContainer != null )
            {
                for ( int i = 0; i < minionSpawnTransformContainer.minionSpawnTransforms.Count; i ++ )
                {
                    if (myMinions.Count < minionSpawnTransformContainer.minionSpawnTransforms.Count)
                    {
                        WeightedRandomBag<Npc.Type> npcMinionTypesPossible = new WeightedRandomBag<Type>();
                        for (int ii = 0; ii < curAttackData.minionSpawnDatas.Length; ii++)
                        {
                            npcMinionTypesPossible.AddEntry(curAttackData.minionSpawnDatas[ii].type, curAttackData.minionSpawnDatas[ii].weight);
                        }
                        Npc.Type spawnTypeChosen = npcMinionTypesPossible.Choose();

                        Transform spawnTr = minionSpawnTransformContainer.minionSpawnTransforms[i];
                        Vector3 spawnP = spawnTr.position;
                        spawnP.y = myTransform.position.y;
                        Quaternion spawnR = spawnTr.rotation;
                        GameManager.instance.SpawnNpc(spawnTypeChosen, spawnP, spawnedBy, false, false, true, null, null, null, this);
                        PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, spawnP, spawnR, 2f);
                    }
                }
            }
            attackDealtDamage = true;

            // audio
            AudioManager.instance.PlaySoundAtPosition(myTransform.position,BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.spawnMinionClips),.7f,.9f,.5f,.525f, 2f + myInfo.audio.distanceAdd, 8f + myInfo.audio.distanceAdd);
        }

        Vector3 p0 = myTransform.position;
        Vector3 p1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;

        if ( overrideAttackPoint )
        {
            p1 = attackPointOverride;
        }

        p1.y = p0.y;
        float dstToPlayer = Vector3.Distance(p0,p1);
        float dstToPlayerFac = Mathf.Clamp(dstToPlayer * .5f,.1f,1f);
        Vector3 r0 = myTransform.position;
        Vector3 r1 = ( myInfo.stats.fleeFromPlayer ) ? fleeTransform.position : chaseTransform.position;

        if (overrideAttackPoint)
        {
            r1 = attackPointOverride;
        }

        r1.y = r0.y;
        Vector3 r2 = (r1 - r0).normalized;
        AddForce(r2, (4f * curAttackData.fwdForceFac) * dstToPlayerFac);

        // laser?
        if (curAttackData.attackType == AttackData.AttackType.Ranged && curAttackData.rangedAttackType == AttackData.RangedAttackType.Laser )
        {
            SetupManager.instance.SetLaserAttackInPlay();
        }

        // spawn projectile
        if (curAttackData.attackType == AttackData.AttackType.Ranged || curAttackData.attackType == AttackData.AttackType.WakeBoss)
        {
            int shootCount;
            switch ( curAttackData.rangedShootFromType )
            {
                default: shootCount = 1; break;
                case AttackData.RangedShootFromType.Eye: shootCount = myGraphics.eyeTransforms.Length; break;
                case AttackData.RangedShootFromType.Mouth: shootCount = 1; break;
            }

            int repeat = (curAttackData.rangedAttackType == AttackData.RangedAttackType.Single || curAttackData.rangedAttackType == AttackData.RangedAttackType.Laser ) ? 1 : curAttackData.sprayCount;

            StartCoroutine(ShootProjectile(shootCount,repeat));
        }

        // heal self?
        if ( curAttackData.attackType == AttackData.AttackType.HealSelf )
        {
            Heal(curAttackData.healAmount);
        }

        // heal others?
        if ( curAttackData.attackType == AttackData.AttackType.HealRadius )
        {
            if ( LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs != null && LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs.Count > 0 )
            {
                for ( int i = 0; i < LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs.Count; i ++ )
                {
                    NpcCore npcHeal = LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs[i];
                    if ( npcHeal != null && npcHeal != this )
                    {
                        Vector3 healP0 = myTransform.position;
                        Vector3 healP1 = npcHeal.myTransform.position;
                        healP1.y = healP0.y;
                        float d0 = Vector3.Distance(healP0,healP1);
                        if ( d0 <= curAttackData.healRadius )
                        {
                            npcHeal.Heal(curAttackData.healAmount);
                        }
                    }
                }
            }
        }

        // store this attack
        lastAttackData = curAttackData;

        // audio
        PlayAttackDoAudio();
    }

    IEnumerator ShootProjectile ( int _shootCount, int _repeat )
    {
        while ( curAttackSprayCounter < curAttackData.sprayInterval )
        {
            while (SetupManager.instance.inFreeze)
            {
                yield return null;
            }
            curAttackSprayCounter++;
            yield return null;
        }

        while (curAttackSprayCount < _repeat)
        {
            if ( curAttackSprayCount > 0 )
            {
                PlayAttackDoAudio();
            }

            for (int i = 0; i < _shootCount; i++)
            {
                Vector3 projectileSpawnPos;
                switch (curAttackData.rangedShootFromType)
                {
                    // shoot from default position
                    default:
                        projectileSpawnPos = myTransform.position + (Vector3.up * .5f);
                        projectileSpawnPos += myTransform.forward * .5f;
                        break;

                    // shoot from eye(s)
                    case AttackData.RangedShootFromType.Eye:
                        projectileSpawnPos = eyeTransforms[i].position;
                        projectileSpawnPos += myTransform.forward * .5f;
                        break;

                    // shoot from mouth
                    case AttackData.RangedShootFromType.Mouth:
                        projectileSpawnPos = myGraphics.mouthProjectileSpawnTransform.position; //myGraphics.mouthTransforms[0].position;
                        projectileSpawnPos += myTransform.forward * .5f;
                        break;
                }

                if (curAttackData.rangedAttackType == AttackData.RangedAttackType.Laser)
                {
                    Quaternion laserSpawnRot = Quaternion.identity;
                    GameObject laserO = PrefabManager.instance.SpawnPrefabAsGameObject(curAttackData.projectilePrefab, myGraphics.mouthProjectileSpawnTransform.position, laserSpawnRot, .125f);
                    LaserScript laserScript = laserO.GetComponent<LaserScript>();

                    Vector3 shootPointTargetStart = (myTransform.position + (myGraphics.graphicsTransform.forward * 1.25f));
                    shootPointTargetStart.y = 0f;
                    laserScript.shootPointTarget = shootPointTargetStart;
                    laserScript.shootPointCur = shootPointTargetStart;
                    laserScript.targetPlayer = true;
                    laserScript.clearDur = Mathf.RoundToInt(curAttackData.attackDoDur * attackDoDurFac);
                    laserScript.npcCoreBy = this;
                    laserScript.shootFromTransform = myGraphics.mouthProjectileSpawnTransform;
                }
                else
                {
                    Quaternion projectileSpawnRot = Quaternion.identity;
                    GameObject projectileO = PrefabManager.instance.SpawnPrefabAsGameObject(curAttackData.projectilePrefab, projectileSpawnPos, projectileSpawnRot, .125f);
                    if (projectileO != null)
                    {
                        Transform projectileTr = projectileO.transform;

                        // magic projectile?
                        if (projectileO.GetComponent<ProjectileScript>() != null)
                        {
                            ProjectileScript projectileScript = projectileO.GetComponent<ProjectileScript>();

                            Vector3 d2 = attackDir.normalized;
                            d2 += (Vector3.up * .325f);

                            if (curAttackData.facePlayer)
                            {
                                Vector3 pp0 = myTransform.position;
                                Vector3 pp1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                                pp1.y = pp0.y;
                                Vector3 pp2 = (pp1 - pp0).normalized;
                                d2 = pp2;
                            }

                            d2 += (myTransform.forward * TommieRandom.instance.RandomRange(curAttackData.rangedFwdOffMax.x, curAttackData.rangedFwdOffMax.y));
                            d2 += (myTransform.right * TommieRandom.instance.RandomRange(curAttackData.rangedSideOffMax.x, curAttackData.rangedSideOffMax.y));
                            d2 += (myTransform.up * TommieRandom.instance.RandomRange(curAttackData.rangedUpOffMax.x, curAttackData.rangedUpOffMax.y));

                            projectileScript.dir = d2;

                            projectileScript.speed = (7f * curAttackData.rangedSpeedFactor);
                            projectileScript.radius = .25f;
                            projectileScript.gravityMultiplier *= curAttackData.rangedGravityFactor;
                            projectileScript.npcCoreBy = this;
                            projectileScript.SetOwnerType(ProjectileScript.OwnerType.Npc);

                            if ( myType == Type.Faerie )
                            {
                                projectileScript.isFromFaerie = true;
                            }
                        }
                        else if (projectileO.GetComponent<Stuiterbaar>() != null)
                        {
                            projectileTr.localScale = Vector3.one;

                            Stuiterbaar stuiterbaarScript = projectileO.GetComponent<Stuiterbaar>();

                            Vector3 d2 = attackDir.normalized;
                            d2 += (Vector3.up * .325f);

                            if (curAttackData.facePlayer)
                            {
                                Vector3 pp0 = myTransform.position;
                                Vector3 pp1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                                pp1.y = pp0.y;
                                Vector3 pp2 = (pp1 - pp0).normalized;
                                d2 = pp2;
                            }

                            d2 += (myTransform.forward * TommieRandom.instance.RandomRange(curAttackData.rangedFwdOffMax.x, curAttackData.rangedFwdOffMax.y));
                            d2 += (myTransform.right * TommieRandom.instance.RandomRange(curAttackData.rangedSideOffMax.x, curAttackData.rangedSideOffMax.y));
                            d2 += (myTransform.up * TommieRandom.instance.RandomRange(curAttackData.rangedUpOffMax.x, curAttackData.rangedUpOffMax.y));
                            d2 += (Vector3.up * 1f);

                            stuiterbaarScript.forceCur = (d2 * curAttackData.rangedSpeedFactor) * .1f;
                            stuiterbaarScript.gravityMultiplier = curAttackData.rangedGravityFactor;
                            stuiterbaarScript.npcCoreBy = this;
                            stuiterbaarScript.dealDamage = true;
                            stuiterbaarScript.SetOwnerType(ProjectileScript.OwnerType.Npc);
                            stuiterbaarScript.autoClear = true;
                            stuiterbaarScript.autoClearDur = 600;

                            // log
                            //Debug.Log("botje || " + Time.time.ToString());
                        }
                    }
                }
            }

            curAttackSprayCount++;
            curAttackSprayCounter = 0;

            yield return new WaitForSeconds((1f / 60f) * curAttackData.sprayInterval);
        }
    }

    public void LoseEyes ()
    {
        if (!lostEyes)
        {
            PickRandomAttack();

            Vector3 d0 = myTransform.position;
            Vector3 d1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
            d1.y = d0.y;
            Vector3 d2 = (d1 - d0).normalized;

            AddForce(d2,-4f);

            InitGetStunned(-d2,6f,60);

            justLostEyesCounter = 0;
            lostEyes = true;
        }
    }

    public void StopAttackDo ()
    {
        if (inCombat)
        {
            SetState((!myInfo.stats.fleeFromPlayer) ? State.Chase : State.Flee);
        }
        else
        {
            SetState(State.Roam);
        }

        myUnit.StopMoving();

        PickRandomAttack();
    }

    public void InitSetBlock ()
    {
        blockDur = Mathf.RoundToInt(TommieRandom.instance.RandomRange(blockMinDur,blockMaxDur));
        blockCounter = 0;
        myUnit.StopMoving();
        SetState(State.Block);

        // audio
        AudioManager.instance.PlaySoundAtPosition(myTransform.position,BasicFunctions.PickRandomAudioClipFromArray(attackPrepareClipsUse), attackPreparePitchMin, attackPreparePitchMax, attackPrepareVolumeMin, attackPrepareVolumeMax, 2f + myInfo.audio.distanceAdd, 8f + myInfo.audio.distanceAdd);
    }

    public void InitBlockReact (float _blockDelay)
    {
        Invoke("BlockReact",_blockDelay);
    }

    void BlockReact ()
    {
        StopBlock();
        InitSetAttackPrepare(false,Vector3.zero);
    }

    public void StopBlock ()
    {
        canBlockCounter = 0;
        blockCounter = blockDur;
        SetState((!myInfo.stats.fleeFromPlayer) ? State.Chase : State.Flee);
    }

    public void InitWakeDungeonBoss ()
    {
        PickSpecificAttackData(Npc.AttackData.AttackType.WakeBoss);

        Transform dungeonBossTr = spawnedBy.dungeonBossNpcSpawner.myNpcCores[0].myTransform;
        Vector3 dungeonBossP = dungeonBossTr.position;
        InitSetAttackPrepare(true, dungeonBossP);

        // flee after this?
        fleeTransform = dungeonBossTr;//GameManager.instance.playerFirstPersonDrifter.myTransform;
        myInfo.stats.fleeFromPlayer = true;
    }

    public void AddForce ( Vector3 _dir, float _speed )
    {
        Vector3 f = _dir;
        f.x *= _speed;
        f.y *= _speed;
        f.z *= _speed;
        f.y = 0f;
        forceDirTarget = f;
        forceDirCur = forceDirTarget;

        // log
        //Debug.DrawLine(myRigidbody.position,myRigidbody.position + f,Color.white,5f);
    }

    public void DampForce ( float _f )
    {
        forceDirTarget *= _f;
    }

    public void CreateAlertIndicator ()
    {
        GameObject newAlertIndicatorO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.alertIndicatorPrefab[0], myTransform.position, Quaternion.identity, 1f);
        Transform newAlertIndicatorTr = newAlertIndicatorO.transform;
        newAlertIndicatorTr.parent = GameManager.instance.mainCanvasRectTransform;

        BasicFunctions.ResetTransform(newAlertIndicatorTr);

        RectTransform rTr = newAlertIndicatorO.GetComponent<RectTransform>();
        rTr.localScale = Vector3.one;

        AlertIndicator newAlertIndicatorScript = newAlertIndicatorO.GetComponent<AlertIndicator>();
        newAlertIndicatorScript.myNpcCore = this;
    }

    public void Clear ()
    {
        // create white orb
        if (myType == Type.RatKing || myType == Type.MagicSkull || myType == Type.HellLord)
        {
            GameObject bossExplosionO = PrefabManager.instance.SpawnPrefabAsGameObject(PrefabManager.instance.bossExplosionPrefab, particleSpawnPoint + myInfo.stats.defeatWhiteOrbOffset,Quaternion.identity,1f);
            if ( bossExplosionO != null )
            {
                BossExplosion bossExplosionScript = bossExplosionO.GetComponent<BossExplosion>();
                if ( bossExplosionScript != null )
                {
                    bossExplosionScript.orbRadius = (2f + myInfo.stats.defeatWhiteOrbScaleFactor);
                }
            }
        }
        else
        {
            PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, particleSpawnPoint + myInfo.stats.defeatWhiteOrbOffset, Quaternion.identity, 2f + myInfo.stats.defeatWhiteOrbScaleFactor);
        }

        // remove health indicator
        if ( myHealthIndicator != null )
        {
            myHealthIndicator.Clear();
        }

        // remove from list of active npcs
        if (LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs.Contains(this))
        {
            LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs.Remove(this);
        }

        // weg met die vieze voeten
        if ( myInfo.graphics.hasFeet )
        {
            if ( myGraphics.feetObjects != null && myGraphics.feetObjects.Length > 0 )
            {
                for ( int i = myGraphics.feetObjects.Length - 1; i >= 0; i -- )
                {
                    Destroy(myGraphics.feetObjects[i]);
                }
            }
        }

        // DOEI HANDEN DOEEEHOEEI
        if (myInfo.graphics.hasHand)
        {
            if (myGraphics.handObjects != null && myGraphics.handObjects.Length > 0)
            {
                for (int i = myGraphics.handObjects.Length - 1; i >= 0; i--)
                {
                    Destroy(myGraphics.handObjects[i]);
                }
            }
        }

        // zeg maar dag tegen die vleugels
        if (myInfo.graphics.hasWings)
        {
            if (myGraphics.wingObjects != null && myGraphics.wingObjects.Length > 0)
            {
                for (int i = myGraphics.wingObjects.Length - 1; i >= 0; i--)
                {
                    Destroy(myGraphics.wingObjects[i]);
                }
            }
        }

        // fire charge objecten?
        if ( myInfo.graphics.eyeHasFireCharge )
        {
            if ( myGraphics.eyeFireChargeObjects.Count > 0 )
            {
                for ( int i = myGraphics.eyeFireChargeObjects.Count - 1; i >= 0; i -- )
                {
                    Destroy(myGraphics.eyeFireChargeObjects[i]);
                }
            }
        }

        // mouth charge objecten?
        if (myInfo.graphics.mouthHasFireCharge)
        {
            if (myGraphics.mouthFireChargeObjects.Count > 0)
            {
                for (int i = myGraphics.mouthFireChargeObjects.Count - 1; i >= 0; i--)
                {
                    Destroy(myGraphics.mouthFireChargeObjects[i]);
                }
            }
        }

        // equipments
        if (myInfo.graphics.hasEquipment)
        {
            if (myGraphics.equipmentObjects != null && myGraphics.equipmentObjects.Length > 0)
            {
                for (int i = myGraphics.equipmentObjects.Length - 1; i >= 0; i--)
                {
                    if ( myGraphics.equipmentObjects[i].GetComponent<ObjectScript>() != null )
                    {
                        ObjectScript objectScript = myGraphics.equipmentObjects[i].GetComponent<ObjectScript>();
                        if ( objectScript != null )
                        {
                            if ( objectScript.myFlameableScript != null )
                            {
                                objectScript.myFlameableScript.Clear();
                            }
                        }
                    }
                    Destroy(myGraphics.equipmentObjects[i]);
                }
            }
        }

        // remove player in combat with reference
        RemovePlayerInCombatWith();

        // destroy object
        Destroy(myGameObject);
    }

    public void AddPlayerInCombatWith ()
    {
        if ( GameManager.instance != null && GameManager.instance.playerInCombatWith != null && !GameManager.instance.playerInCombatWith.Contains(this) )
        {
            GameManager.instance.playerInCombatWith.Add(this);
        }
    }

    public void RemovePlayerInCombatWith ()
    {
        if (GameManager.instance != null && GameManager.instance.playerInCombatWith != null && GameManager.instance.playerInCombatWith.Count > 0 && GameManager.instance.playerInCombatWith.Contains(this) )
        {
            GameManager.instance.playerInCombatWith.Remove(this);
        }
    }

    public void AddMinion ( NpcCore _minion ) 
    {
        if ( myMinions == null )
        {
            myMinions = new List<NpcCore>();
        }
        myMinions.Add(_minion);
    }

    public void RemoveMinion ( NpcCore _minion )
    {
        if ( myMinions != null && myMinions.Contains(_minion) )
        {
            myMinions.Remove(_minion);
        }
    }

    public void SetAttackData ( AttackData _to )
    {
        curAttackData = _to;
        curAttackSprayCount = 0;
        curAttackSprayCounter = 0;
    }

    public void PickRandomAttackData ()
    {
        if (myInfo.attacks != null && myInfo.attacks.Length > 0)
        {
            SetAttackData(myAttackDatas.Choose());
        }
    }

    public void MagicSkullPickAttackData ()
    {
        if (myInfo.attacks != null && myInfo.attacks.Length > 0)
        {
            bool eyesAreGone = true;
            for (int i = 0; i < myGraphics.eyeFireChargeScripts.Count; i++)
            {
                if (!myGraphics.eyeFireChargeScripts[i].collected)
                {
                    eyesAreGone = false;
                    break;
                }
            }

            WeightedRandomBag<AttackData> possibleAttackDatas = new WeightedRandomBag<AttackData>();
            for (int i = 0; i < myInfo.attacks.Length; i++)
            {
                AttackData attackDataCheck = myInfo.attacks[i];

                bool canAddAttack = false;

                // check for eye attack
                if (attackDataCheck.attackType == AttackData.AttackType.Ranged && attackDataCheck.rangedShootFromType == AttackData.RangedShootFromType.Eye && !eyesAreGone)
                {
                    canAddAttack = true;
                }

                // check for laser attack
                if (attackDataCheck.attackType == AttackData.AttackType.Ranged && attackDataCheck.rangedShootFromType == AttackData.RangedShootFromType.Mouth && eyesAreGone)
                {
                    canAddAttack = true;
                }

                // check for melee attack
                if (attackDataCheck.attackType == AttackData.AttackType.Melee && eyesAreGone)
                {
                    canAddAttack = true;
                }

                if (canAddAttack)
                {
                    possibleAttackDatas.AddEntry(attackDataCheck, attackDataCheck.weight);
                }
            }

            AttackData attackDataChosen = possibleAttackDatas.Choose();
            SetAttackData(attackDataChosen);
        }
    }

    public void HellLordPickAttackData ()
    {
        if (myInfo.attacks != null && myInfo.attacks.Length > 0)
        {
            WeightedRandomBag<AttackData> possibleAttackDatas = new WeightedRandomBag<AttackData>();
            for (int i = 0; i < myInfo.attacks.Length; i++)
            {
                AttackData attackDataCheck = myInfo.attacks[i];

                bool canAddAttack = true;

                // check for spray attack
                if (attackDataCheck.attackType == AttackData.AttackType.Ranged && attackDataCheck.rangedAttackType == AttackData.RangedAttackType.Spray && stageIndex < 1 )
                {
                    canAddAttack = false;
                }

                if (canAddAttack)
                {
                    possibleAttackDatas.AddEntry(attackDataCheck, attackDataCheck.weight);
                }
            }

            AttackData attackDataChosen = possibleAttackDatas.Choose();
            SetAttackData(attackDataChosen);
        }
    }

    public void PickSpecificAttackData ( AttackData.AttackType _type )
    {
        bool foundAttackData = false;
        if (myInfo.attacks.Length > 0)
        {
            for (int i = 0; i < myInfo.attacks.Length; i++)
            {
                AttackData attackDataCheck = myInfo.attacks[i];
                if (!foundAttackData)
                {
                    if (attackDataCheck.attackType == _type)
                    {
                        SetAttackData(attackDataCheck);
                        foundAttackData = true;
                    }
                }
            }
        }
    }

    public void PlayAlertedAudio ()
    {
        AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(alertClipsUse), alertPitchMin, alertPitchMax, alertVolumeMin, alertVolumeMax, 2f + myInfo.audio.distanceAdd, 8f + myInfo.audio.distanceAdd);
    }

    public void PlayHurtAudio ()
    {
        AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(hurtClipsUse), hurtPitchMin, hurtPitchMax, hurtVolumeMin, hurtVolumeMax, 2f + myInfo.audio.distanceAdd, 8f + myInfo.audio.distanceAdd);
    }

    public void PlayDeadAudio ()
    {
        AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(deadClipsUse), deadPitchMin, deadPitchMax, deadVolumeMin, deadVolumeMax, 2f + myInfo.audio.distanceAdd, 8f + myInfo.audio.distanceAdd);
    }

    public void PlayAttackPrepareAudio ()
    {
        AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(attackPrepareClipsUse), attackPreparePitchMin, attackPreparePitchMax, attackPrepareVolumeMin, attackPrepareVolumeMax, 2f + myInfo.audio.distanceAdd, 8f + myInfo.audio.distanceAdd);
    }

    public void PlayAttackDoAudio ()
    {
        AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(attackDoClipsUse), attackDoPitchMin, attackDoPitchMax, attackDoVolumeMin, attackDoVolumeMax, 2f + myInfo.audio.distanceAdd, 8f + myInfo.audio.distanceAdd);
    }
}
