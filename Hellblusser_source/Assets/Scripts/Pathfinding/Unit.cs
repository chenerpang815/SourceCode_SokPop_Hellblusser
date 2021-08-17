using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
	[HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // layerMask
    public LayerMask groundLayerMask;
    [HideInInspector] public float groundHeight;
    [HideInInspector] public float groundHeightExtra;
    [HideInInspector] public Vector3 posTarget, posCur;
    [HideInInspector] public float grav;

    [HideInInspector] public Vector3[] path;
	[HideInInspector] public int targetIndex;

    [HideInInspector] public Vector3 currentWaypoint;
    [HideInInspector] public bool lookingForPath;
	[HideInInspector] public bool moving;
    [HideInInspector] public bool canSetFleeDestination;
    [HideInInspector] public bool wantsToLightCannon;

    // npc core
    public NpcCore myNpcCore;

    // lava
    [HideInInspector] public bool aboveLava;

    // grid
    [HideInInspector] public Grid gridUse;

    // position
    Vector3 posOriginal;

    // roam
    [HideInInspector] public int roamWaitDurMin, roamWaitDurMax, roamWaitDur, roamWaitCounter;
    [HideInInspector] public int chaseRefreshRate, chaseRefreshCounter;
    [HideInInspector] public int moveToPositionRefreshRate, moveToPositionRefreshCounter;
    [HideInInspector] public int cannonLightWaitDur, cannonLightWaitCounter;
    [HideInInspector] public int moveToPositionWaitDur, moveToPositionWaitCounter;
    [HideInInspector] public int chaseWaitDurMin, chaseWaitDurMax, chaseWaitDur, chaseWaitCounter;

    // init
    [HideInInspector] public bool initialized;

    void Awake ()
    {
        initialized = false;
    }

    void Start()
    {
        Init();
    }

    public void Init ()
	{
        // base components
        myTransform = transform;
        myGameObject = gameObject;

        // grid
        gridUse = Grid.instance;

        // roam
        roamWaitDurMin = 32;
        roamWaitDurMax = 64;
        roamWaitDur = Mathf.RoundToInt(TommieRandom.instance.RandomRange(roamWaitDurMin,roamWaitDurMax));
        roamWaitCounter = 0;

        // chase
        chaseRefreshRate = 16;
        chaseRefreshCounter = 0;
        chaseWaitDurMin = 16;
        chaseWaitDurMax = 32;
        chaseWaitDur = Mathf.RoundToInt(TommieRandom.instance.RandomRange(chaseWaitDurMin,chaseWaitDurMax));
        chaseWaitCounter = 0;

        // move to position
        moveToPositionRefreshRate = 16;
        moveToPositionRefreshCounter = 0;

        // position
        posOriginal = myTransform.position;
        posTarget = posOriginal;
        posCur = posTarget;
        groundHeight = posCur.y;

        lookingForPath = false;

        // done
        initialized = true;
    }

    public void LookForPath ( Vector3 _to, Grid _grid )
	{
        if (_grid != null)
        {
            PathRequestManager.RequestPath(posCur, _to, _grid, OnPathFound);
            lookingForPath = true;

            // log
            Debug.DrawLine(myTransform.position,_to,Color.white,2f);
        }
    }

    void SetPath ( Vector3[] _path )
	{
		path = _path;

        // follow path!
        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");
    }

    void Update ()
    {
        if (initialized)
        {
            if (!SetupManager.instance.inFreeze && LevelGeneratorManager.instance.activeLevelGenerator.generatedLevel )
            {
                if ( cannonLightWaitCounter < cannonLightWaitDur )
                {
                    cannonLightWaitCounter++;
                }

                if (myNpcCore.curState != NpcCore.State.Hit && myNpcCore.curState != NpcCore.State.Stunned && myNpcCore.curState != NpcCore.State.AttackDo && myNpcCore.curState != NpcCore.State.Block)
                {
                    // draw path
                    if (path != null)
                    {
                        for (int i = 0; i < path.Length - 1; i++)
                        {
                            Debug.DrawLine(path[i], path[i + 1], Color.red);
                        }
                    }

                    // movement
                    if (!lookingForPath)
                    {
                        switch (myNpcCore.curState)
                        {
                            case NpcCore.State.Roam:
                                if (roamWaitCounter < roamWaitDur)
                                {
                                    Vector3 p0 = posCur;
                                    Vector3 p1 = posTarget;
                                    p1.y = p0.y;
                                    float dstToDesired = Vector3.Distance(p0, p1);
                                    float dstContinue = .1f;
                                    if (dstToDesired <= dstContinue)
                                    {
                                        roamWaitCounter++;
                                        posTarget = posCur;
                                    }
                                }
                                else
                                {
                                    float offMax = 8f;
                                    Vector3 p = posOriginal;
                                    p += TommieRandom.instance.RandomInsideSphere() * TommieRandom.instance.RandomRange(offMax * .25f, offMax);
                                    p.y = 0f;
                                    LookForPath(p, Grid.instance);

                                    if (Grid.instance.GetNearestNode(p).walkable)
                                    {
                                        roamWaitCounter = 0;
                                        roamWaitDur = Mathf.RoundToInt(TommieRandom.instance.RandomRange(roamWaitDurMin, roamWaitDurMax));
                                    }
                                }
                                break;

                            case NpcCore.State.LightCannon:

                                {
                                    Vector3 p0 = posCur;
                                    Vector3 p1 = posTarget;
                                    p1.y = p0.y;
                                    float dstToDesired = Vector3.Distance(p0, p1);
                                    float dstContinue = 2f;
                                    if (dstToDesired <= dstContinue)
                                    {
                                        StopMoving();

                                        myNpcCore.PickSpecificAttackData(Npc.AttackData.AttackType.Ranged);

                                        Vector3 cannonP = myNpcCore.cannonScriptTarget.ropeLinePointsCur[myNpcCore.cannonScriptTarget.ropeLinePointsCur.Length - 1];
                                        myNpcCore.InitSetAttackPrepare(true, cannonP);

                                        // wait for cannon to shoot, before moving on!
                                        cannonLightWaitDur = (myNpcCore.curAttackData.attackPrepareDur + myNpcCore.curAttackData.attackDoDur) + 10;
                                        cannonLightWaitCounter = 0;
                                    }
                                }

                                break;

                            case NpcCore.State.MoveToPosition:

                                if (moveToPositionWaitCounter < moveToPositionWaitDur)
                                {
                                    moveToPositionWaitCounter++;
                                }
                                else
                                {
                                    Vector3 p0 = posCur;
                                    Vector3 p1 = posTarget;
                                    p1.y = p0.y;
                                    float dstToDesired = Vector3.Distance(p0, p1);
                                    float dstContinue = myNpcCore.moveToPositionStopDst;
                                    if (dstToDesired <= dstContinue)
                                    {
                                        StopMoving();

                                        myNpcCore.SetState(myNpcCore.moveToPositionEndState);
                                    }
                                }

                                break;

                            case NpcCore.State.Chase:

                                // we volgen toch altijd de speler dus check maar ff of die referentie er wel is
                                if ( myNpcCore != null && myNpcCore.chaseTransform == null )
                                {
                                    myNpcCore.chaseTransform = GameManager.instance.playerFirstPersonDrifter.myTransform;
                                }

                                // en then doorgaan
                                switch ( myNpcCore.myType )
                                {
                                    default: DefaultChaseBehaviour(); break;
                                    case Npc.Type.MagicSkull: MagicSkullChaseBehaviour(); break;
                                    case Npc.Type.Faerie: FaerieChaseBehaviour(); break;
                                    case Npc.Type.HellLord: HellLordChaseBehaviour(); break;
                                }

                                break;

                            case NpcCore.State.Flee:

                                if (myNpcCore.fleeTransform != null)
                                {
                                    if ( myNpcCore.myType == Npc.Type.RatKing )
                                    {
                                        if( cannonLightWaitCounter < cannonLightWaitDur )
                                        {
                                            myNpcCore.InitSetBlock();
                                            return;
                                        }
                                    }

                                    Vector3 p0 = posCur;
                                    Vector3 p1 = posTarget;
                                    p1.y = p0.y;
                                    float dstToDesired = Vector3.Distance(p0, p1);
                                    float dstContinue = .1f;

                                    if (dstToDesired <= dstContinue)
                                    {
                                        StopMoving();

                                        // update flee position?
                                        Vector3 pp0 = myTransform.position;
                                        Vector3 pp1 = myNpcCore.fleeTransform.position;
                                        float dd0 = Vector3.Distance(pp0, pp1);
                                        float fleeThreshold = 4f;
                                        if (dd0 <= fleeThreshold)
                                        {
                                            float offMax = 6f;
                                            Vector3 p = myNpcCore.fleeTransform.position;
                                            p += TommieRandom.instance.RandomInsideSphere() * TommieRandom.instance.RandomRange(offMax * .5f, offMax);
                                            p.y = 0f;
                                            LookForPath(p, Grid.instance);

                                            /*
                                            if (Grid.instance.GetNearestNode(p).walkable)
                                            {
                                                Vector3 cc0 = myTransform.position;
                                                Vector3 cc1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                                                Vector3 cc2 = p;
                                                float ddA = Vector3.Distance(cc0, cc1);
                                                float ddB = Vector3.Distance(cc0, cc2);
                                                if (ddB > ddA)
                                                {
                                                    canSetFleeDestination = false;
                                                }
                                            }
                                            */
                                        }
                                        else // not close to target we are fleeing from -> do something?
                                        {
                                            switch (myNpcCore.myType)
                                            {
                                                case Npc.Type.RatKing:

                                                    WeightedRandomBag<NpcDatabase.RatKingSpecialBehaviour> ratKingSpecialBehaviours = new WeightedRandomBag<NpcDatabase.RatKingSpecialBehaviour>();
                                                    bool ableToLightCannon = false;
                                                    for ( int i = 0; i < LevelGeneratorManager.instance.activeLevelGenerator.allCannonScripts.Count; i ++ )
                                                    {
                                                        if ( LevelGeneratorManager.instance.activeLevelGenerator.allCannonScripts[i].curState == CannonScript.State.Idle )
                                                        {
                                                            ableToLightCannon = true;
                                                            break;
                                                        }
                                                    }
                                                    if (ableToLightCannon)
                                                    {
                                                        ratKingSpecialBehaviours.AddEntry(NpcDatabase.RatKingSpecialBehaviour.LightCannon, 10);
                                                    }
                                                    bool ableToSpawnMinions = (myNpcCore.myMinions.Count <= (myNpcCore.minionSpawnTransformContainer.minionSpawnTransforms.Count / 2));
                                                    if (ableToSpawnMinions)
                                                    {
                                                        ratKingSpecialBehaviours.AddEntry(NpcDatabase.RatKingSpecialBehaviour.SpawnMinion, 10);
                                                    }
                                                    NpcDatabase.RatKingSpecialBehaviour ratKingSpecialBehaviourPicked = ratKingSpecialBehaviours.Choose();

                                                    switch (ratKingSpecialBehaviourPicked)
                                                    {
                                                        // SPAWN MINIONS?
                                                        case NpcDatabase.RatKingSpecialBehaviour.SpawnMinion:
                                                            if (myNpcCore.minionSpawnTransformContainer != null)
                                                            {
                                                                myNpcCore.PickSpecificAttackData(Npc.AttackData.AttackType.MinionSpawn);
                                                                myNpcCore.InitSetAttackPrepare(false, Vector3.zero);
                                                                return;
                                                            }
                                                        break;

                                                        // PICK A CANNON TO LIGHT?
                                                        case NpcDatabase.RatKingSpecialBehaviour.LightCannon:

                                                            int cannonIndexPick = 0;
                                                            CannonScript cannonScriptPick = myNpcCore.cannonScripts[cannonIndexPick];
                                                            myNpcCore.cannonScripts.RemoveAt(cannonIndexPick);
                                                            myNpcCore.cannonScripts.Insert(myNpcCore.cannonScripts.Count,cannonScriptPick);

                                                            if (cannonScriptPick != null)
                                                            {
                                                                myNpcCore.SetCannonTarget(cannonScriptPick);

                                                                Vector3 cannonP = myNpcCore.cannonScriptTarget.ropeLinePointsCur[myNpcCore.cannonScriptTarget.ropeLinePointsCur.Length - 1];
                                                                cannonP += (myNpcCore.cannonScriptTarget.myTransform.forward * 1f);
                                                                cannonP.y = 0f;

                                                                myNpcCore.InitSetMoveToPosition(cannonP, NpcCore.State.LightCannon,1f);
                                                               
                                                                LookForPath(cannonP, Grid.instance);
                                                                return;

                                                                // log
                                                                //Debug.Log("ik wil graag naar kanon: " + cannonScriptPick + " || " + Time.time.ToString());
                                                            }
                                                        break;
                                                    }

                                                    break;
                                            }
                                        }
                                    }
                                }

                                break;

                            case NpcCore.State.Alerted:
                                StopMoving();
                                break;

                            case NpcCore.State.AttackPrepare:
                                StopMoving();
                                break;

                            case NpcCore.State.AttackDo:
                                StopMoving();
                                break;

                            case NpcCore.State.EnterNextStage:

                                {
                                    LookForPath(myNpcCore.spawnedBy.enterNextStageMovePoint.myTransform.position, Grid.instance);

                                    Vector3 p0 = posCur;
                                    Vector3 p1 = posTarget;
                                    p1.y = p0.y;
                                    float dstToDesired = Vector3.Distance(p0, p1);
                                    float dstContinue = .25f;
                                    if (dstToDesired <= dstContinue)
                                    {
                                        StopMoving();
                                    }
                                }

                                break;
                        }
                    }

                    // set position
                    posCur.y = (groundHeight + groundHeightExtra);
                    //myTransform.position = posCur;
                }
                else
                {
                    posCur = myTransform.position;

                    float bounceSpd = (2f * Time.deltaTime);
                    posCur += (myNpcCore.forceDirCur * bounceSpd);
                    posTarget = posCur;

                    myTransform.position = posCur;
                }

                // check if above lava
                if (!myNpcCore.myInfo.graphics.flying)
                {
                    float ccDst = 4f;
                    Vector3 cc0 = posCur;
                    cc0.y += ccDst;
                    Vector3 cc1 = posCur;
                    cc1.y -= ccDst;
                    RaycastHit ccHit;
                    if (Physics.Linecast(cc0, cc1, out ccHit, SetupManager.instance.lavaLayerMask))
                    {
                        Transform ccHitTr = ccHit.transform;
                        GameObject ccHitO = ccHitTr.gameObject;
                        if (ccHitO != null && ccHitO.layer == 15)
                        {
                            Vector3 ppHit = ccHit.point;
                            if (!aboveLava)
                            {
                                myNpcCore.InitGetHit(myNpcCore.forceDirCur, 2f, 6000, false);
                                aboveLava = true;
                            }
                            else
                            {
                                if (ppHit.y > posCur.y)
                                {
                                    AudioManager.instance.PlaySoundAtPosition(myTransform.position, BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.fireStartClips), .9f, 1.2f, .2f, .225f);
                                    PrefabManager.instance.SpawnPrefab(PrefabManager.instance.lavaBallImpactParticlesPrefab[0], myTransform.position, Quaternion.identity, 1f);
                                    Vector2 ripplePos = Camera.main.WorldToScreenPoint(myTransform.position);
                                    SetupManager.instance.SetRippleEffect(ripplePos, 17.5f);
                                    myNpcCore.SetDead();
                                }
                            }
                        }
                    }
                }
                if ( aboveLava )
                {
                    grav += (.325f * Time.deltaTime);
                    posCur.y -= grav;
                }

                // set position
                if (!aboveLava)
                {
                    if (SetupManager.instance.runDataRead.curFloorIndex == 3)
                    {
                        float cDst = 4f;
                        Vector3 c0 = posCur;
                        c0.y += cDst;
                        Vector3 c1 = posCur;
                        c1.y -= cDst;
                        RaycastHit cHit;
                        if (Physics.Linecast(c0, c1, out cHit, SetupManager.instance.npcGroundLayerMask))
                        {
                            Vector3 pHit = cHit.point;
                            bool pinToGround = true;
                            if (myNpcCore.curState == NpcCore.State.AttackDo && posCur.y > pHit.y)
                            {
                                pinToGround = false;
                            }
                            if (pinToGround)
                            {
                                posCur.y = (pHit.y + groundHeightExtra);
                            }
                        }
                    }
                }
                myTransform.position = posCur;
            }
        }
    }

    public void DefaultChaseBehaviour ()
    {
        if (myNpcCore.chaseTransform != null)
        {
            Vector3 pp0 = myTransform.position;
            Vector3 pp1 = myNpcCore.chaseTransform.position;
            float dd0 = Vector3.Distance(pp0, pp1);

            float chaseStopDstUse = myNpcCore.myInfo.stats.chaseStopDst;
            chaseStopDstUse += myNpcCore.curAttackData.stopChaseDstAdd;
            if (dd0 <= chaseStopDstUse)
            {
                chaseWaitCounter = 0;
                chaseWaitDur = Mathf.RoundToInt(TommieRandom.instance.RandomRange(chaseWaitDurMin, chaseWaitDurMax));
                StopMoving();

                float randomBlockChance = myNpcCore.myInfo.stats.randomBlockChance;
                float blockReactChance = myNpcCore.myInfo.stats.reactBlockChance;

                bool doRandomBlock = (TommieRandom.instance.RandomValue(1f) <= randomBlockChance);
                bool doBlockReact = (HandManager.instance.handScripts[1].inMeleeAttack && TommieRandom.instance.RandomValue(1f) <= blockReactChance);

                bool doBlock = ((doRandomBlock || doBlockReact) && (myNpcCore.canBlockCounter >= myNpcCore.canBlockDur));

                if (!doBlock)
                {
                    Vector3 dot0A = GameManager.instance.mainCameraTransform.position;
                    Vector3 dot0B = myNpcCore.myTransform.position;
                    dot0B.y = dot0A.y;
                    Vector3 dot0 = (dot0B - dot0A).normalized;
                    Vector3 dot1 = GameManager.instance.mainCameraTransform.forward;
                    dot1.y = 0f;
                    dot1.Normalize();
                    float dotProduct0 = Vector3.Dot(dot0, dot1);
                    float visibleThreshold = .75f;
                    //Debug.Log("dotProduct: " + dotProduct0.ToString() + " || threshold: " + visibleThreshold.ToString() + " || " + Time.time.ToString());
                    if (dotProduct0 >= visibleThreshold)
                    {
                        myNpcCore.InitSetAttackPrepare(false, Vector3.zero);
                    }
                }
                else
                {
                    myNpcCore.InitSetBlock();
                }
            }
            else
            {
                if (chaseWaitCounter < chaseWaitDur)
                {
                    chaseWaitCounter++;
                }
                else
                {
                    if (chaseRefreshCounter < chaseRefreshRate)
                    {
                        chaseRefreshCounter++;
                    }
                    else
                    {
                        LookForPath(pp1, Grid.instance);
                        chaseRefreshCounter = 0;
                    }
                }
            }
        }
    }

    public void MagicSkullChaseBehaviour ()
    {
        if (myNpcCore.chaseTransform != null)
        {
            Vector3 pp0 = myTransform.position;
            Vector3 pp1 = myNpcCore.chaseTransform.position;
            float dd0 = Vector3.Distance(pp0, pp1);

            float chaseStopDstUse = myNpcCore.myInfo.stats.chaseStopDst;
            chaseStopDstUse += myNpcCore.curAttackData.stopChaseDstAdd;
            if (dd0 <= chaseStopDstUse)
            {
                chaseWaitCounter = 0;
                chaseWaitDur = Mathf.RoundToInt(TommieRandom.instance.RandomRange(chaseWaitDurMin, chaseWaitDurMax));
                StopMoving();

                Vector3 dot0A = GameManager.instance.mainCameraTransform.position;
                Vector3 dot0B = myNpcCore.myTransform.position;
                dot0B.y = dot0A.y;
                Vector3 dot0 = (dot0B - dot0A).normalized;
                Vector3 dot1 = GameManager.instance.mainCameraTransform.forward;
                dot1.y = 0f;
                dot1.Normalize();

                float dotProduct0 = Vector3.Dot(dot0, dot1);
                float visibleThreshold = .75f;
                if (dotProduct0 >= visibleThreshold)
                {
                    switch ( myNpcCore.lastAttackData.attackType )
                    {
                        case Npc.AttackData.AttackType.Ranged:

                            break;
                            
                    }

                    myNpcCore.InitSetAttackPrepare(false, Vector3.zero);
                }
            }
            else
            {
                if (chaseWaitCounter < chaseWaitDur)
                {
                    chaseWaitCounter++;
                }
                else
                {
                    if (chaseRefreshCounter < chaseRefreshRate)
                    {
                        chaseRefreshCounter++;
                    }
                    else
                    {
                        LookForPath(pp1, Grid.instance);
                        chaseRefreshCounter = 0;
                    }
                }
            }
        }
    }

    public void HellLordChaseBehaviour ()
    {
        //Debug.Log("hell lord in chase, chaseTarget: " + myNpcCore.chaseTransform + " || " + Time.time.ToString());
        
        if (myNpcCore.chaseTransform != null)
        {
            Vector3 pp0 = myTransform.position;
            Vector3 pp1 = myNpcCore.chaseTransform.position;
            float dd0 = Vector3.Distance(pp0, pp1);

            float chaseStopDstUse = myNpcCore.myInfo.stats.chaseStopDst;
            chaseStopDstUse += myNpcCore.curAttackData.stopChaseDstAdd;
            if (dd0 <= chaseStopDstUse)
            {
                chaseWaitCounter = 0;
                chaseWaitDur = Mathf.RoundToInt(TommieRandom.instance.RandomRange(chaseWaitDurMin, chaseWaitDurMax));
                StopMoving();

                Vector3 dot0A = GameManager.instance.mainCameraTransform.position;
                Vector3 dot0B = myNpcCore.myTransform.position;
                dot0B.y = dot0A.y;
                Vector3 dot0 = (dot0B - dot0A).normalized;
                Vector3 dot1 = GameManager.instance.mainCameraTransform.forward;
                dot1.y = 0f;
                dot1.Normalize();

                float dotProduct0 = Vector3.Dot(dot0, dot1);
                float visibleThreshold = .75f;
                if (dotProduct0 >= visibleThreshold)
                {
                    int healNeed = 2;
                    bool canCollectFire = ((myNpcCore.health < myNpcCore.myInfo.stats.health - healNeed) && GameManager.instance.CheckIfFireInRadius(myTransform.position,10f, healNeed));
                    bool doAttack = ((TommieRandom.instance.RandomValue(1f) <= .575f) || !canCollectFire);
                    if (doAttack)
                    {
                        myNpcCore.InitSetAttackPrepare(false, Vector3.zero);
                    }
                    else
                    {
                        myNpcCore.InitSetCollectFire();
                    }
                }
            }
            else
            {
                if (chaseWaitCounter < chaseWaitDur)
                {
                    chaseWaitCounter++;
                }
                else
                {
                    if (chaseRefreshCounter < chaseRefreshRate)
                    {
                        chaseRefreshCounter++;
                    }
                    else
                    {
                        LookForPath(pp1, Grid.instance);
                        chaseRefreshCounter = 0;
                    }
                }
            }
        }
    }

    public void FaerieChaseBehaviour ()
    {
        if (myNpcCore.chaseTransform != null)
        {
            Vector3 pp0 = myTransform.position;
            Vector3 pp1 = myNpcCore.chaseTransform.position;
            float dd0 = Vector3.Distance(pp0, pp1);

            float chaseStopDstUse = myNpcCore.myInfo.stats.chaseStopDst;
            chaseStopDstUse += myNpcCore.curAttackData.stopChaseDstAdd;
            if (dd0 <= chaseStopDstUse)
            {
                chaseWaitCounter = 0;
                chaseWaitDur = Mathf.RoundToInt(TommieRandom.instance.RandomRange(chaseWaitDurMin, chaseWaitDurMax));
                StopMoving();

                Vector3 dot0A = GameManager.instance.mainCameraTransform.position;
                Vector3 dot0B = myNpcCore.myTransform.position;
                dot0B.y = dot0A.y;
                Vector3 dot0 = (dot0B - dot0A).normalized;
                Vector3 dot1 = GameManager.instance.mainCameraTransform.forward;
                dot1.y = 0f;
                dot1.Normalize();

                // move towards dungeon boss
                //Debug.Log("did laugh: " + myNpcCore.didLaugh + " || " + Time.time.ToString());

                if (!myNpcCore.didLaugh)
                {
                    myNpcCore.InitSetLaugh();
                }
                else
                {
                    /*
                    NpcCore dungeonBossCore = myNpcCore.spawnedBy.dungeonBossNpcSpawner.myNpcCores[0];
                    if (dungeonBossCore.curState == NpcCore.State.Sleeping)
                    {
                        Vector3 dungeonBossP = dungeonBossCore.myTransform.position;
                        myNpcCore.InitSetMoveToPosition(dungeonBossP, NpcCore.State.WakeDungeonBoss, 4f);
                        LookForPath(dungeonBossP, Grid.instance);
                        return;
                    }
                    else
                    {
                        // irritante balletjes schieten op spelert
                    }
                    */
                }
            }
            else
            {
                if (chaseWaitCounter < chaseWaitDur)
                {
                    chaseWaitCounter++;
                }
                else
                {
                    if (chaseRefreshCounter < chaseRefreshRate)
                    {
                        chaseRefreshCounter++;
                    }
                    else
                    {
                        LookForPath(pp1, Grid.instance);
                        chaseRefreshCounter = 0;
                    }
                }
            }
        }
    }

    public void StopMoving ()
    {
        if (moving)
        {
            StopCoroutine("FollowPath");
            moving = false;

            lookingForPath = false;
            posCur = myTransform.position;
            posTarget = posCur;
            roamWaitCounter = 0;
        }

        // log
        //if (myNpcCore.myType == Npc.Type.RatKing)
        //{
        //    Debug.Log("STOP MOVING || " + Time.time.ToString());
        //}
    }

    public void OnPathFound ( Vector3[] _newPath, Grid _grid, bool _pathSuccessful )
	{
		if ( _pathSuccessful )
		{
            SetPath(_newPath);
            if (_newPath.Length > 0)
            {
                Node n = Grid.instance.GetNearestNode(_newPath[_newPath.Length - 1]);
                posTarget = Grid.instance.grid[n.gridX, n.gridY].worldPosition;

                // log
                Debug.DrawLine(myTransform.position, posTarget, Color.green, 2f);
            }
        }
        lookingForPath = false;
    }

    IEnumerator FollowPath ()
	{
        if (!SetupManager.instance.inFreeze && !SetupManager.instance.paused)
        {
            if (path != null && path.Length > 0 && (myNpcCore.curState == NpcCore.State.Roam || myNpcCore.curState == NpcCore.State.Chase || myNpcCore.curState == NpcCore.State.Flee || myNpcCore.curState == NpcCore.State.MoveToPosition || myNpcCore.curState == NpcCore.State.EnterNextStage))
            {
                //Debug.Log("gaaaa dan ||" + Time.time.ToString());

                currentWaypoint = path[0];

                targetIndex = 0;
                moving = false;
                while (true)
                {
                    moving = true;

                    float proceedDst = (gridUse.nodeRadius * .0125f);
                    if (BasicFunctions.IsApproximately(myTransform.position.x, currentWaypoint.x, proceedDst) && BasicFunctions.IsApproximately(myTransform.position.z, currentWaypoint.z, proceedDst))
                    {
                        bool stopPath = false;

                        if (!stopPath)
                        {
                            targetIndex++;
                        }

                        // reached the last waypoint?
                        if (targetIndex >= path.Length)
                        {
                            stopPath = true;
                        }

                        if (stopPath)
                        {
                            StopMoving();

                            targetIndex = 0;
                            path = new Vector3[0];
                            yield break;
                        }

                        // set new target waypoint
                        currentWaypoint = path[targetIndex];
                    }

                    // position
                    float movementSpdUse = myNpcCore.myInfo.stats.movementSpd;
                    if ( myNpcCore.myType == Npc.Type.MagicSkull && myNpcCore.lostEyes )
                    {
                        movementSpdUse += 1.5f;
                    }
                    if (myNpcCore.myType == Npc.Type.HellLord && myNpcCore.clearedLava)
                    {
                        movementSpdUse += 1f;
                    }
                    if ( SetupManager.instance.curRunType == SetupManager.RunType.Endless )
                    {
                        movementSpdUse += myNpcCore.speedBoost;
                    }
                    posCur = Vector3.MoveTowards(posCur, currentWaypoint, movementSpdUse * Time.deltaTime);

                    // graphics rotation
                    if (myNpcCore.curState == NpcCore.State.Roam)
                    {
                        myNpcCore.myGraphics.GraphicsFacePosition(currentWaypoint,Vector3.zero);
                    }
                    if (myNpcCore.curState == NpcCore.State.Chase)
                    {
                        Vector3 r0 = currentWaypoint;
                        Vector3 r1 = myNpcCore.chaseTransform.position;
                        Vector3 rTarget = BasicFunctions.LerpByDistance(r0, r1, Vector3.Distance(r0, r1) * .25f);
                        myNpcCore.myGraphics.GraphicsFacePosition(rTarget,Vector3.zero);
                    }
                    if (myNpcCore.curState == NpcCore.State.Flee)
                    {
                        myNpcCore.myGraphics.GraphicsFacePosition(currentWaypoint,Vector3.zero);
                    }
                    if (myNpcCore.curState == NpcCore.State.MoveToPosition)
                    {
                        myNpcCore.myGraphics.GraphicsFacePosition(currentWaypoint,Vector3.zero);
                    }

                    yield return null;
                }
            }
        }
	}
}
