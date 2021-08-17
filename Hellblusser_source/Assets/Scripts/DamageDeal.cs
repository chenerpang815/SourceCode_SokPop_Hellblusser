using UnityEngine;

public class DamageDeal : MonoBehaviour
{
    // base components
    [HideInInspector] public Transform myTransform;
    [HideInInspector] public GameObject myGameObject;

    // type
    //public enum Type { Melee, Magic };
    //public Npc.AttackData.DamageType myDamageType;

    // data
    Vector3 impactDirStored;

    // target
    public enum Target { AI, Player, All };
    public Target myTarget;

    // state
    public enum State { Active, Inactive };
    public State curState;
    [HideInInspector] public bool createdByPlayer;
    [HideInInspector] public bool gotBlocked;
    [HideInInspector] public bool isKick;

    // info
    public Vector3 position;
    public float radius;
    public Info info;
    [HideInInspector] public float impactForce;

    // created by
    public enum HitType { Instant, Delayed };
    public HitType myHitType;

    [HideInInspector] public Transform createdByTransform;
    [HideInInspector] public NpcCore npcCoreBy;
    public bool putOutOfSleep;

    // counters
    [HideInInspector] public int clearDur, clearCounter;

    public struct Info
    {
        public int amount;
        public Npc.AttackData.DamageType damageType;
    }

    public void SetState ( State _to )
    {
        curState = _to;
    }

    void Start ()
    {
        myTransform = transform;
        myGameObject = gameObject;

        gotBlocked = false;
        clearCounter = 0;

        SetState(State.Active);
    }

    void Update ()
    {
        if (!SetupManager.instance.inFreeze)
        {
            if (clearCounter < clearDur)
            {
                clearCounter++;

                bool canDealDamage = ( createdByPlayer ) ? true : (clearCounter >= (clearDur / 3));

                if ( info.amount <= 0 && !putOutOfSleep )
                {
                    canDealDamage = false;
                }

                // deal damage? loop door lijst van actieve npcs, en kijk dan of je ze raakt
                if ( canDealDamage && curState == State.Active)
                {
                    float visibleThreshold = .5f;//.125f;

                    if (npcCoreBy != null && npcCoreBy.myType == Npc.Type.MagicSkull)
                    {
                        myTarget = Target.All;
                    }

                    bool checkForNpcs = (myTarget == Target.AI || myTarget == Target.All);
                    bool checkForPlayer = (myTarget == Target.Player || myTarget == Target.All);

                    if ( myTarget == Target.All )
                    {
                        checkForNpcs = true;
                        checkForPlayer = true;
                    }

                    bool checkForVernietigbaars = true;
                    bool checkForCannons = true;

                    if ( SetupManager.instance.playerInvulnerable )
                    {
                        checkForPlayer = false;
                    }

                    if (checkForNpcs && LevelGeneratorManager.instance != null && LevelGeneratorManager.instance.activeLevelGenerator != null && LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs != null)
                    {
                        for (int i = 0; i < LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs.Count; i++)
                        {
                            NpcCore npcCheck = LevelGeneratorManager.instance.activeLevelGenerator.activeNpcs[i];
                            if ( npcCheck != null && (npcCoreBy == null || (npcCoreBy != null && npcCoreBy != npcCheck)) && ((!putOutOfSleep && npcCheck.curState != NpcCore.State.Sleeping) || (putOutOfSleep && npcCheck.curState == NpcCore.State.Sleeping)) )
                            {
                                Vector3 dot0 = npcCheck.myTransform.position;
                                Vector3 dot1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                                dot1.y = dot0.y;
                                Vector3 dot2 = (dot1 - dot0).normalized;
                                //dot2.y = 0f;
                                dot2.Normalize();

                                Vector3 camFwd = GameManager.instance.mainCameraTransform.forward;
                                camFwd.y = 0f;
                                camFwd.Normalize();

                                float dot3 = Vector3.Dot(-dot2, camFwd);
                                if (dot3 >= visibleThreshold)
                                {
                                    Vector3 p0 = myTransform.position;
                                    Vector3 p1 = npcCheck.myTransform.position;
                                    p1.y = p0.y;

                                    float d0 = Vector3.Distance(p0, p1);
                                    if (d0 <= (radius + npcCheck.myInfo.stats.hitBoxExtraRadius))
                                    {
                                        Vector3 pp0 = createdByTransform.position;
                                        Vector3 pp1 = p1;
                                        pp1.y = pp0.y;
                                        Vector3 pp2 = (pp1 - pp0).normalized;

                                        if (!putOutOfSleep)
                                        {
                                            bool hasImmunity = ((info.damageType == Npc.AttackData.DamageType.Melee && npcCheck.myInfo.stats.meleeImmune) || (info.damageType == Npc.AttackData.DamageType.Magic && npcCheck.myInfo.stats.magicImmune));
                                            if (npcCheck != null && npcCheck.curState == NpcCore.State.Vulnerable)
                                            {
                                                if (info.damageType == Npc.AttackData.DamageType.Magic)
                                                {
                                                    hasImmunity = false;
                                                }
                                            }
                                            if (npcCheck.myInfo.stats.immortal)
                                            {
                                                hasImmunity = true;
                                            }

                                            if (!isKick)
                                            {
                                                if ((info.damageType == Npc.AttackData.DamageType.Melee && npcCheck.curState == NpcCore.State.Block) || (npcCheck.myInfo.stats.immortal))
                                                {
                                                    gotBlocked = true;

                                                    npcCheck.AddForce(pp2, .5f);
                                                    HandManager.instance.handScripts[1].StopMeleeAttack();
                                                    GameManager.instance.playerFirstPersonDrifter.myImpactReceiver.AddImpact(pp2, -20f);
                                                    GameManager.instance.SetPlayerStunned(24);

                                                    if (!npcCheck.myInfo.stats.immortal)
                                                    {
                                                        // npc attacks?
                                                        float blockDelay = .325f;
                                                        npcCheck.InitBlockReact(blockDelay);

                                                        // create UI text
                                                        Vector3 indicatorHeightOff = (Vector3.up * npcCheck.myInfo.graphics.healthIndicatorOff);
                                                        if (npcCheck.myInfo.graphics.flying)
                                                        {
                                                            Vector3 flyOffAdd = (Vector3.up * npcCheck.myGraphics.flyOff);
                                                            indicatorHeightOff += flyOffAdd;
                                                        }
                                                        Vector3 damageIndicatorP = npcCheck.myGraphics.graphicsTransform.position + indicatorHeightOff;

                                                        Transform damageIndicatorOffTr = GameManager.instance.mainCameraTransform;
                                                        damageIndicatorP += (damageIndicatorOffTr.forward * npcCheck.myInfo.graphics.damageIndicatorLocalAdd.z);
                                                        damageIndicatorP += (damageIndicatorOffTr.right * npcCheck.myInfo.graphics.damageIndicatorLocalAdd.x);
                                                        damageIndicatorP += (damageIndicatorOffTr.up * npcCheck.myInfo.graphics.damageIndicatorLocalAdd.y);
                                                        GameManager.instance.CreateDamageIndicatorString("block", damageIndicatorP);
                                                    }
                                                }
                                                else
                                                {
                                                    float impactForceMultiplier = (hasImmunity) ? .5f : 1f;
                                                    if (!hasImmunity)
                                                    {
                                                        npcCheck.DealDamage(info.amount, info.damageType);
                                                    }
                                                    else // create immune text?
                                                    {
                                                        Vector3 indicatorHeightOff = (Vector3.up * npcCheck.myInfo.graphics.healthIndicatorOff);
                                                        if (npcCheck.myInfo.graphics.flying)
                                                        {
                                                            Vector3 flyOffAdd = (Vector3.up * npcCheck.myGraphics.flyOff);
                                                            indicatorHeightOff += flyOffAdd;
                                                        }
                                                        Vector3 damageIndicatorP = npcCheck.myGraphics.graphicsTransform.position + indicatorHeightOff;

                                                        Transform damageIndicatorOffTr = GameManager.instance.mainCameraTransform;
                                                        damageIndicatorP += (damageIndicatorOffTr.forward * npcCheck.myInfo.graphics.damageIndicatorLocalAdd.z);
                                                        damageIndicatorP += (damageIndicatorOffTr.right * npcCheck.myInfo.graphics.damageIndicatorLocalAdd.x);
                                                        damageIndicatorP += (damageIndicatorOffTr.up * npcCheck.myInfo.graphics.damageIndicatorLocalAdd.y);

                                                        GameManager.instance.CreateDamageIndicatorString("IMMUNE", damageIndicatorP);

                                                        // still play some audio?
                                                        npcCheck.PlayHurtAudio();
                                                        AudioManager.instance.PlayAttackImpactSound(info.damageType);
                                                    }

                                                    // particles & effects?
                                                    if (npcCheck.curState != NpcCore.State.EnterNextStage)
                                                    {
                                                        npcCheck.InitGetHit(pp2, 20f * (impactForce * impactForceMultiplier), npcCheck.myInfo.stats.hitDur, hasImmunity);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // stun
                                                Vector3 kickFrom = createdByTransform.position;
                                                Vector3 kickTo = npcCheck.myTransform.position;
                                                kickTo.y = kickFrom.y;
                                                Vector3 kickDir = (kickTo - kickFrom).normalized;
                                                npcCheck.InitGetStunned(kickDir, 8f, 90);

                                                // kick DOES deal damage?
                                                if ( SetupManager.instance.CheckIfItemSpecialActive(EquipmentDatabase.Specials.KickDamage) )
                                                {
                                                    if (!hasImmunity)
                                                    {
                                                        npcCheck.DealDamage(info.amount, info.damageType);
                                                    }
                                                    else
                                                    {
                                                        // still play some audio?
                                                        npcCheck.PlayHurtAudio();
                                                        AudioManager.instance.PlayAttackImpactSound(info.damageType);

                                                        Vector3 indicatorHeightOff = (Vector3.up * npcCheck.myInfo.graphics.healthIndicatorOff);
                                                        if (npcCheck.myInfo.graphics.flying)
                                                        {
                                                            Vector3 flyOffAdd = (Vector3.up * npcCheck.myGraphics.flyOff);
                                                            indicatorHeightOff += flyOffAdd;
                                                        }
                                                        Vector3 damageIndicatorP = npcCheck.myGraphics.graphicsTransform.position + indicatorHeightOff;

                                                        Transform damageIndicatorOffTr = GameManager.instance.mainCameraTransform;
                                                        damageIndicatorP += (damageIndicatorOffTr.forward * npcCheck.myInfo.graphics.damageIndicatorLocalAdd.z);
                                                        damageIndicatorP += (damageIndicatorOffTr.right * npcCheck.myInfo.graphics.damageIndicatorLocalAdd.x);
                                                        damageIndicatorP += (damageIndicatorOffTr.up * npcCheck.myInfo.graphics.damageIndicatorLocalAdd.y);

                                                        GameManager.instance.CreateDamageIndicatorString("IMMUNE", damageIndicatorP);
                                                    }

                                                    GameManager.instance.playerFirstPersonDrifter.myImpactReceiver.AddImpact(GameManager.instance.playerFirstPersonDrifter.myTransform.forward, -30f);

                                                    if (npcCheck.curState != NpcCore.State.EnterNextStage)
                                                    {
                                                        npcCheck.InitGetHit(pp2, 20f * (impactForce * 1f), npcCheck.myInfo.stats.hitDur, hasImmunity);
                                                    }
                                                }

                                                //// target is skeleton?
                                                //if ( npcCheck.myType == Npc.Type.Skeleton || npcCheck.myType == Npc.Type.RedSkeleton || npcCheck.myType == Npc.Type.BlackSkeleton )
                                                //{
                                                //    npcCheck.DealDamage(1,Npc.AttackData.DamageType.Melee);
                                                //    npcCheck.InitGetHit(pp2, 20f * impactForce, npcCheck.myInfo.stats.hitDur, hasImmunity);
                                                //}

                                                // effects
                                                Vector3 kickParticlePos = npcCheck.particleSpawnPoint + npcCheck.myInfo.stats.defeatWhiteOrbOffset;
                                                PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, kickParticlePos,Quaternion.identity,1.5f);

                                                // ripple
                                                Vector2 ripplePos = Camera.main.WorldToScreenPoint(kickParticlePos);
                                                SetupManager.instance.SetRippleEffect(ripplePos, 17.5f);

                                                // freeze
                                                SetupManager.instance.SetFreeze(4);
                                            }

                                            if (createdByPlayer && !isKick )
                                            {
                                                Vector2 ripplePos = Camera.main.WorldToScreenPoint(myTransform.position);
                                                SetupManager.instance.SetRippleEffect(ripplePos, 17.5f);
                                            }

                                            if (gotBlocked)
                                            {
                                                PrefabManager.instance.SpawnPrefab(PrefabManager.instance.magicImpactParticlesPrefab[2], myTransform.position, Quaternion.identity, 1f);
                                            }

                                            // heal from fire?
                                            if ( info.damageType == Npc.AttackData.DamageType.Magic && npcCheck.myInfo.stats.healFromFire )
                                            {
                                                npcCheck.Heal(1);
                                            }
                                        }
                                        else
                                        {
                                            // log
                                            //Debug.Log("uhhh hallo?? || " + Time.time.ToString());

                                            // stop sleeping for npc?
                                            if ( npcCheck != null && npcCheck.curState == NpcCore.State.Sleeping )
                                            {
                                                //npcCheck.StopSleeping();
                                                npcCheck.InitSetWakeUp();
                                            }
                                        }

                                        // hit!
                                        impactDirStored = pp2;
                                        HitTrigger();
                                    }
                                }

                                // log
                                //Debug.Log("hit dot product: " + dot3.ToString() + " || threshold: " + visibleThreshold.ToString() + " || " + Time.time.ToString());
                            }
                        }
                    }
                    if ( checkForPlayer && (npcCoreBy != null || (npcCoreBy == null && myTarget == Target.All)) )
                    {
                        Vector3 p0 = myTransform.position;
                        Vector3 p1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                        Vector3 p2 = (p1 - p0).normalized;
                        float d0 = Vector3.Distance(p0, p1);
                        if (d0 <= radius)
                        {
                            Vector3 pp0 = (npcCoreBy != null ) ? npcCoreBy.myTransform.position : myTransform.position;
                            Vector3 pp1 = p1;
                            pp1.y = pp0.y;
                            Vector3 pp2 = (pp1 - pp0).normalized;

                            bool playerDodgesAttack = (SetupManager.instance.CheckIfBlessingClaimed(BlessingDatabase.Blessing.AgileJumper) && !GameManager.instance.playerFirstPersonDrifter.grounded);
                            if (!playerDodgesAttack)
                            {
                                Vector3 dot0 = (npcCoreBy != null) ? npcCoreBy.myTransform.position : myTransform.position;//npcCoreBy.myTransform.position;
                                Vector3 dot1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                                dot1.y = dot0.y;
                                Vector3 dot2 = (dot1 - dot0).normalized;
                                dot2.Normalize();

                                Vector3 playerCamFwd = GameManager.instance.mainCameraTransform.forward;
                                playerCamFwd.y = 0f;
                                playerCamFwd.Normalize();
                                float dot3 = Vector3.Dot(-dot2,playerCamFwd);
                                float blockDotThreshold = .5f;

                                if (/*info.damageType == Npc.AttackData.DamageType.Melee &&*/ GameManager.instance.playerFirstPersonDrifter.playerBlocking && (dot3 >= blockDotThreshold))
                                {
                                    gotBlocked = true;
                                }
                                else
                                {
                                    // player ignores damage?
                                    if (!SetupManager.instance.CheckIfItemSpecialActive(EquipmentDatabase.Specials.IgnoreDamage))
                                    {
                                        GameManager.instance.DealDamageToPlayer(info.amount, info.damageType);
                                    }

                                    // recoil?
                                    if (SetupManager.instance.CheckIfItemSpecialActive(EquipmentDatabase.Specials.Recoil))
                                    {
                                        npcCoreBy.DealDamage(1, Npc.AttackData.DamageType.Melee);
                                        npcCoreBy.InitGetHit(-pp2, 20f * (impactForce * 1f), npcCoreBy.myInfo.stats.hitDur, false);
                                    }
                                }

                                // particles
                                Vector3 playerHitP = myTransform.position + (pp2 * -.75f);
                                PrefabManager.instance.SpawnPrefab(PrefabManager.instance.whiteOrbPrefab, playerHitP, Quaternion.identity, 1f);
                                if (gotBlocked)
                                {
                                    PrefabManager.instance.SpawnPrefab(PrefabManager.instance.magicImpactParticlesPrefab[2], playerHitP, Quaternion.identity, 1f);
                                }

                                // hit!
                                impactDirStored = pp2;
                                HitTrigger();

                                // npc knockback?
                                if (npcCoreBy != null && gotBlocked && info.damageType == Npc.AttackData.DamageType.Melee )
                                {
                                    npcCoreBy.InitGetStunned(pp2, -10f, 60);
                                }
                            }
                        }
                    }
                    if (checkForVernietigbaars)
                    {
                        for (int i = 0; i < GameManager.instance.allVernietigbaarScripts.Count; i++)
                        {
                            Vernietigbaar vernietigbaarCheck = GameManager.instance.allVernietigbaarScripts[i];
                            if (vernietigbaarCheck != null)
                            {
                                Vector3 dot0 = vernietigbaarCheck.myTransform.position;
                                Vector3 dot1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                                dot1.y = dot0.y;
                                Vector3 dot2 = (dot1 - dot0).normalized;
                                //dot2.y = 0f;
                                dot2.Normalize();

                                Vector3 camFwd = GameManager.instance.mainCameraTransform.forward;
                                camFwd.y = 0f;
                                camFwd.Normalize();

                                float dot3 = Vector3.Dot(-dot2, camFwd);
                                if (dot3 >= visibleThreshold)
                                {
                                    Vector3 p0 = myTransform.position;
                                    Vector3 p1 = vernietigbaarCheck.myTransform.position;
                                    float d0 = Vector3.Distance(p0, p1);
                                    if (d0 <= radius)
                                    {
                                        vernietigbaarCheck.Destroy();

                                        if (createdByPlayer)
                                        {
                                            Vector2 ripplePos = Camera.main.WorldToScreenPoint(myTransform.position);
                                            SetupManager.instance.SetRippleEffect(ripplePos, 17.5f);
                                        }

                                        // hit!
                                        HitTrigger();
                                    }
                                }

                                // log
                                //Debug.Log("hit dot product: " + dot3.ToString() + " || threshold: " + visibleThreshold.ToString() + " || " + Time.time.ToString());
                            }
                        }
                    }
                    if (checkForCannons && LevelGeneratorManager.instance != null && LevelGeneratorManager.instance.activeLevelGenerator != null && LevelGeneratorManager.instance.activeLevelGenerator.allCannonScripts != null && info.damageType == Npc.AttackData.DamageType.Magic)
                    {
                        for (int i = 0; i < LevelGeneratorManager.instance.activeLevelGenerator.allCannonScripts.Count; i++)
                        {
                            CannonScript cannonScriptCheck = LevelGeneratorManager.instance.activeLevelGenerator.allCannonScripts[i];
                            if (cannonScriptCheck != null && cannonScriptCheck.curState == CannonScript.State.Idle )
                            {
                                Vector3 p0 = myTransform.position;
                                Vector3 p1 = cannonScriptCheck.ropeLinePointsCur[cannonScriptCheck.ropeLinePointsCur.Length - 1];
                                float d0 = Vector3.Distance(p0, p1);
                                if (d0 <= radius)
                                {
                                    cannonScriptCheck.Lit();

                                    if (createdByPlayer)
                                    {
                                        Vector2 ripplePos = Camera.main.WorldToScreenPoint(myTransform.position);
                                        SetupManager.instance.SetRippleEffect(ripplePos, 17.5f);
                                    }

                                    // hit!
                                    HitTrigger();
                                }

                                // log
                                //Debug.Log("hit dot product: " + dot3.ToString() + " || threshold: " + visibleThreshold.ToString() + " || " + Time.time.ToString());
                            }
                        }
                    }

                    // vuurtjes uitmaken met zwaard?
                    if (createdByPlayer && info.damageType == Npc.AttackData.DamageType.Melee )
                    {
                        if (GameManager.instance != null && GameManager.instance.fireScripts != null)
                        {
                            for (int i = 0; i < GameManager.instance.fireScripts.Count; i++)
                            {
                                FireScript fireScriptCheck = GameManager.instance.fireScripts[i];
                                if ( fireScriptCheck != null )
                                {
                                    Vector3 p0 = myTransform.position;
                                    Vector3 p1 = fireScriptCheck.myTransform.position;
                                    float d0 = Vector3.Distance(p0,p1);
                                    float clearThreshold = (radius * 1f);
                                    if ( d0 <= clearThreshold )
                                    {
                                        Vector3 dot0 = p0;
                                        Vector3 dot1 = GameManager.instance.playerFirstPersonDrifter.myTransform.position;
                                        dot1.y = dot0.y;
                                        Vector3 dot2 = (dot1 - dot0).normalized;
                                        dot2.y = 0f;
                                        dot2.Normalize();

                                        Vector3 camFwd = GameManager.instance.mainCameraTransform.forward;
                                        camFwd.y = 0f;
                                        camFwd.Normalize();

                                        float dot3 = Vector3.Dot(-dot2, camFwd);

                                        //Debug.Log("dotje: " + dot3.ToString() + " || " + Time.time.ToString());

                                        if (dot3 >= visibleThreshold)
                                        {
                                            fireScriptCheck.Douse();
                                            //HitTrigger();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Clear();
            }
        }
    }

    public void HitTrigger ()
    {
        SetState(State.Inactive);
        Invoke("Hit", .025f);
    }

    public void Hit ()
    {
        if (info.damageType == Npc.AttackData.DamageType.Melee)
        {
            if (npcCoreBy != null)
            {
                float playerForceFac = (GameManager.instance.playerFirstPersonDrifter.playerBlocking) ? .25f : .75f;
                GameManager.instance.playerFirstPersonDrifter.myImpactReceiver.AddImpact(impactDirStored, 20f * playerForceFac);

                //Debug.DrawLine(GameManager.instance.playerFirstPersonDrifter.myTransform.position,GameManager.instance.playerFirstPersonDrifter.myTransform.position + impactDirStored,Color.cyan,10f);

                npcCoreBy.forceDirTarget = Vector3.zero;
                npcCoreBy.forceDirCur = Vector3.zero;

                float hitForceExtra = (gotBlocked) ? .825f : 1f;
                npcCoreBy.AddForce(npcCoreBy.attackPrepareDir, -((5f * npcCoreBy.myInfo.stats.hitForceFactor) * hitForceExtra));
            }

            // kick?
            if ( isKick )
            {
                AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.kickHitImpactClips),.5f,.8f,.7f,.725f);
            }
        }

        //if ( info.damageType == Npc.AttackData.DamageType.Melee )
        {
            // audio?
            if (gotBlocked)
            {
                SetupManager.instance.SetFreeze(6);
                PlayMeleeBlockedAudio();
            }
        }

        if ( myTarget == Target.AI )
        {
            SetupManager.instance.SetFreeze(SetupManager.instance.npcHitFreeze);
        }
    }

    public void PlayMeleeBlockedAudio()
    {
        AudioManager.instance.PlaySoundGlobal(BasicFunctions.PickRandomAudioClipFromArray(AudioManager.instance.meleeBlockedClips),.6f,.9f,.4f,.425f);
    }

    public void Store ()
    {
        if ( GameManager.instance != null && GameManager.instance.allDamageDeals != null )
        {
            GameManager.instance.allDamageDeals.Add(this);
        }
    }

    public void Clear ()
    {
        if (GameManager.instance != null && GameManager.instance.allDamageDeals != null)
        {
            if (GameManager.instance.allDamageDeals.Contains(this))
            {
                GameManager.instance.allDamageDeals.Remove(this);
            }
        }
        Destroy(myGameObject);
    }

    public void SetHitType ( HitType _to )
    {
        myHitType = _to;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position,radius);
    }
}
